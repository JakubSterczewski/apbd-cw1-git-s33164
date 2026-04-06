namespace LegacyRenewalApp;

public class SubscriptionRenewalService
{
    private readonly IBillingGateway _billingGateway;
    private readonly ICustomerRepository _customerRepository;
    private readonly IDiscountCalculator _discountCalculator;
    private readonly IInvoiceNotifier _invoiceNotifier;
    private readonly IPaymentFeeCalculator _paymentFeeCalculator;
    private readonly ISubscriptionPlanRepository _planRepository;
    private readonly IPremiumSupportFeeCalculator _supportFeeCalculator;
    private readonly ITaxCalculator _taxCalculator;

    public SubscriptionRenewalService() : this(
        new CustomerRepository(),
        new SubscriptionPlanRepository(),
        new BillingGatewayAdapter(),
        new DiscountCalculator(new IDiscountStrategy[]
        {
            new SegmentDiscountStrategy(),
            new LoyaltyYearsDiscountStrategy(),
            new SeatCountDiscountStrategy(),
            new LoyaltyPointsDiscountStrategy()
        }),
        new PremiumSupportFeeCalculator(),
        new PaymentMethodFeeCalculator(),
        new TaxRateCalculator(),
        new EmailInvoiceNotifier(new BillingGatewayAdapter()))
    {
    }

    public SubscriptionRenewalService(ICustomerRepository customerRepository,
        ISubscriptionPlanRepository planRepository, IBillingGateway billingGateway,
        IDiscountCalculator discountCalculator, IPremiumSupportFeeCalculator supportFeeCalculator,
        IPaymentFeeCalculator paymentFeeCalculator, ITaxCalculator taxCalculator, IInvoiceNotifier invoiceNotifier)
    {
        _customerRepository = customerRepository;
        _planRepository = planRepository;
        _billingGateway = billingGateway;
        _discountCalculator = discountCalculator;
        _supportFeeCalculator = supportFeeCalculator;
        _paymentFeeCalculator = paymentFeeCalculator;
        _taxCalculator = taxCalculator;
        _invoiceNotifier = invoiceNotifier;
    }

    public RenewalInvoice CreateRenewalInvoice(
        int customerId,
        string planCode,
        int seatCount,
        string paymentMethod,
        bool includePremiumSupport,
        bool useLoyaltyPoints)
    {
        CreateRenewalInvoiceValidator.Validate(customerId, planCode, seatCount, paymentMethod);

        var normalizedPlanCode = planCode.Trim().ToUpperInvariant();
        var normalizedPaymentMethod = paymentMethod.Trim().ToUpperInvariant();

        var customer = _customerRepository.GetById(customerId);
        var plan = _planRepository.GetByCode(normalizedPlanCode);

        if (!customer.IsActive) throw new InvalidOperationException("Inactive customers cannot renew subscriptions");

        var baseAmount = plan.MonthlyPricePerSeat * seatCount * 12m + plan.SetupFee;
        var notes = string.Empty;

        var (discountAmount, discountNotes) =
            _discountCalculator.Calculate(baseAmount, customer, plan, seatCount, useLoyaltyPoints);
        notes += discountNotes;

        var subtotalAfterDiscount = baseAmount - discountAmount;
        if (subtotalAfterDiscount < 300m)
        {
            subtotalAfterDiscount = 300m;
            notes += "minimum discounted subtotal applied; ";
        }

        var (supportFee, supportNote) = _supportFeeCalculator.Calculate(normalizedPlanCode, includePremiumSupport);
        notes += supportNote;

        var (paymentFee, paymentNote) =
            _paymentFeeCalculator.CalculateFee(normalizedPaymentMethod, subtotalAfterDiscount, supportFee);
        notes += paymentNote;

        var taxRate = _taxCalculator.CalculateTax(customer.Country);
        var taxBase = subtotalAfterDiscount + supportFee + paymentFee;
        var taxAmount = taxBase * taxRate;
        var finalAmount = taxBase + taxAmount;

        if (finalAmount < 500m)
        {
            finalAmount = 500m;
            notes += "minimum invoice amount applied; ";
        }

        var invoice = new RenewalInvoice
        {
            InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{customerId}-{normalizedPlanCode}",
            CustomerName = customer.FullName,
            PlanCode = normalizedPlanCode,
            PaymentMethod = normalizedPaymentMethod,
            SeatCount = seatCount,
            BaseAmount = Math.Round(baseAmount, 2, MidpointRounding.AwayFromZero),
            DiscountAmount = Math.Round(discountAmount, 2, MidpointRounding.AwayFromZero),
            SupportFee = Math.Round(supportFee, 2, MidpointRounding.AwayFromZero),
            PaymentFee = Math.Round(paymentFee, 2, MidpointRounding.AwayFromZero),
            TaxAmount = Math.Round(taxAmount, 2, MidpointRounding.AwayFromZero),
            FinalAmount = Math.Round(finalAmount, 2, MidpointRounding.AwayFromZero),
            Notes = notes.Trim(),
            GeneratedAt = DateTime.UtcNow
        };

        _billingGateway.SaveInvoice(invoice);

        _invoiceNotifier.Notify(customer, invoice, normalizedPlanCode);

        return invoice;
    }
}