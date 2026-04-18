namespace C6.Repositories;

public interface IProductRepository
{
    Task<bool> TestConnectionAsync();
}