namespace LegacyRenewalApp;

public class SegmentDiscountStrategy : IDiscountStrategy
{
    private static readonly Dictionary<string, (decimal Rate, string Note)> SegmentRates = new()
    {
        ["Silver"] = (0.05m, "silver discount; "),
        ["Gold"] = (0.10m, "gold discount; "),
        ["Platinum"] = (0.15m, "platinum discount; ")
    };

    public bool IsApplicable(Customer customer, SubscriptionPlan plan, int seatCount, bool useLoyaltyPoints)
    {
        return SegmentRates.ContainsKey(customer.Segment) ||
               (customer.Segment == "Education" && plan.IsEducationEligible);
    }

    public (decimal Amount, string Note) Calculate(decimal baseAmount, Customer customer, SubscriptionPlan plan,
        int seatCount, bool useLoyaltyPoints)
    {
        if (SegmentRates.TryGetValue(customer.Segment, out var entry))
            return (baseAmount * entry.Rate, entry.Note);

        return (baseAmount * 0.20m, "education discount; ");
    }
}