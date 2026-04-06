namespace LegacyRenewalApp;

public class TaxRateCalculator : ITaxCalculator
{
    private static readonly Dictionary<string, decimal> Rates = new()
    {
        ["Poland"] = 0.23m,
        ["Germany"] = 0.19m,
        ["Czech Republic"] = 0.21m,
        ["Norway"] = 0.25m
    };

    public decimal CalculateTax(string country)
    {
        return Rates.TryGetValue(country, out var rate) ? rate : 0.20m;
    }
}