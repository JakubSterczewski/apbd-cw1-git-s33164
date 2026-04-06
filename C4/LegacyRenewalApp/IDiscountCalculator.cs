namespace LegacyRenewalApp;

public interface IDiscountCalculator
{
    (decimal TotalDiscount, string Notes) Calculate(decimal baseAmount,Customer customer,SubscriptionPlan plan, int seatCount, bool useLoyaltyPoints);
}