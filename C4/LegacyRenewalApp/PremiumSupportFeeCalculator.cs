namespace LegacyRenewalApp;

public class PremiumSupportFeeCalculator : IPremiumSupportFeeCalculator
{
    private static readonly Dictionary<string, decimal> SupportFees = new()
    {
        ["START"] = 250m,
        ["PRO"] = 400m,
        ["ENTERPRISE"] = 700m
    };

    public (decimal Fee, string Note) Calculate(string planCode, bool includePremiumSupport)
    {
        if (!includePremiumSupport) return (0m, "");

        var fee = SupportFees.TryGetValue(planCode, out var value) ? value : 0m;
        return (fee, "premium support included; ");
    }
}