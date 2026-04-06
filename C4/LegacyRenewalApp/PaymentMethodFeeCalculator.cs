namespace LegacyRenewalApp;

public class PaymentMethodFeeCalculator : IPaymentFeeCalculator
{
    private static readonly Dictionary<string, (decimal Rate, string Note)> Fees = new()
    {
        ["CARD"] = (0.23m, "card payment fee; "),
        ["BANK_TRANSFER"] = (0.19m, "bank transfer fee; "),
        ["PAYPAL"] = (0.21m, "paypal fee; "),
        ["INVOICE"] = (0.25m, "invoice payment; ")
    };

    public (decimal Fee, string Note) CalculateFee(string paymentMethod, decimal subtotal, decimal supportFee)
    {
        if (!Fees.TryGetValue(paymentMethod, out var entry))
            throw new ArgumentException("Unsupported payment method");

        return ((subtotal + supportFee) * entry.Rate, entry.Note);
    }
}