namespace LegacyRenewalApp;

public interface IDiscountStrategy
{
    bool IsApplicable(Customer customer, SubscriptionPlan plan, int seatCount, bool useLoyaltyPoints);

    (decimal Amount, string Note) Calculate(decimal baseAmount, Customer customer, SubscriptionPlan plan, int seatCount,
        bool useLoyaltyPoints);
}