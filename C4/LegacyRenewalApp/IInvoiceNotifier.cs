namespace LegacyRenewalApp;

public interface IInvoiceNotifier
{
    void Notify(Customer customer, RenewalInvoice invoice, string planCode);
}