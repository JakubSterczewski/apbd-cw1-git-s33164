namespace LegacyRenewalApp;

public interface ITaxCalculator
{
    decimal CalculateTax(string country);
}