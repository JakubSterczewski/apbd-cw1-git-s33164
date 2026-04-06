using System.Text;

namespace LegacyRenewalApp;

public class DiscountCalculator : IDiscountCalculator
{
    private readonly IEnumerable<IDiscountStrategy> _strategies;

    public DiscountCalculator(IEnumerable<IDiscountStrategy> strategies)
    {
        _strategies = strategies;
    }

    public (decimal TotalDiscount, string Notes) Calculate(decimal baseAmount, Customer customer, SubscriptionPlan plan,
        int seatCount, bool useLoyaltyPoints)
    {
        var total = 0m;
        var notes = "";

        foreach (var strategy in _strategies)
            if (strategy.IsApplicable(customer, plan, seatCount, useLoyaltyPoints))
            {
                var (amount, note) = strategy.Calculate(baseAmount, customer, plan, seatCount, useLoyaltyPoints);
                total += amount;
                notes += note;
            }

        return (total, notes);
    }
}