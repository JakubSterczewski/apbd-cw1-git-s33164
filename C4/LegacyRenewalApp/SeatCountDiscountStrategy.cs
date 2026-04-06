namespace LegacyRenewalApp;

public class SeatCountDiscountStrategy : IDiscountStrategy
{
    public bool IsApplicable(Customer customer, SubscriptionPlan plan, int seatCount, bool useLoyaltyPoints)
    {
        return seatCount >= 10;
    }

    public (decimal Amount, string Note) Calculate(decimal baseAmount, Customer customer, SubscriptionPlan plan,
        int seatCount, bool useLoyaltyPoints)
    {
        if (seatCount >= 50)
            return (baseAmount * 0.12m, "large team discount; ");

        if (seatCount >= 20)
            return (baseAmount * 0.08m, "medium team discount; ");

        return (baseAmount * 0.04m, "small team discount; ");
    }
}