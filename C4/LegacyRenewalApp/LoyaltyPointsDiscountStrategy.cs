namespace LegacyRenewalApp;

public class LoyaltyPointsDiscountStrategy : IDiscountStrategy
{
    public bool IsApplicable(Customer customer, SubscriptionPlan plan, int seatCount, bool useLoyaltyPoints)
    {
        return useLoyaltyPoints && customer.LoyaltyPoints > 0;
    }

    public (decimal Amount, string Note) Calculate(decimal baseAmount, Customer customer, SubscriptionPlan plan,
        int seatCount, bool useLoyaltyPoints)
    {
        var pointsToUse = Math.Min(customer.LoyaltyPoints, 200);
        return (pointsToUse, $"loyalty points used: {pointsToUse}; ");
    }
}