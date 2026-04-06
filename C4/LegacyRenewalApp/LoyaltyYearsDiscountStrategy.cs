namespace LegacyRenewalApp;

public class LoyaltyYearsDiscountStrategy : IDiscountStrategy
{
    public bool IsApplicable(Customer customer, SubscriptionPlan plan, int seatCount, bool useLoyaltyPoints)
    {
        return customer.YearsWithCompany >= 2;
    }

    public (decimal Amount, string Note) Calculate(decimal baseAmount, Customer customer, SubscriptionPlan plan,
        int seatCount,
        bool useLoyaltyPoints)
    {
        if (customer.YearsWithCompany >= 5)
            return (baseAmount * 0.07m, "long-term loyalty discount; ");

        return (baseAmount * 0.03m, "basic loyalty discount; ");
    }
}