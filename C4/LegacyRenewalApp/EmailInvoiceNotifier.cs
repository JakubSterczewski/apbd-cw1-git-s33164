namespace LegacyRenewalApp;

public class EmailInvoiceNotifier : IInvoiceNotifier
{
    private readonly IBillingGateway _billingGateway;

    public EmailInvoiceNotifier(IBillingGateway billingGateway)
    {
        _billingGateway = billingGateway;
    }

    public void Notify(Customer customer, RenewalInvoice invoice, string planCode)
    {
        if (string.IsNullOrWhiteSpace(customer.Email))
            return;

        var subject = "Subscription renewal invoice";
        var body =
            $"Hello {customer.FullName}, your renewal for plan {planCode} " +
            $"has been prepared. Final amount: {invoice.FinalAmount:F2}.";

        _billingGateway.SendEmail(customer.Email, subject, body);
    }
}