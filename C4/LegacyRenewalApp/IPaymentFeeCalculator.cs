namespace LegacyRenewalApp;

public interface IPaymentFeeCalculator
{
    (decimal Fee, string Note) CalculateFee(string paymentMethod, decimal subtotal, decimal supportFee);
}