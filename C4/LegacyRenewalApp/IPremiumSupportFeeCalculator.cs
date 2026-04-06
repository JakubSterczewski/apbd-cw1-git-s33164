namespace LegacyRenewalApp;

public interface IPremiumSupportFeeCalculator
{
    (decimal Fee, string Note) Calculate(string planCode, bool includePremiumSupport);
}