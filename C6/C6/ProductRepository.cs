using C6.Repositories;

namespace C6;

// Najpierw instalujemy pakiet NuGet:
// dotnet add package Microsoft.Data.SqlClient

using Microsoft.Data.SqlClient;

public class ProductRepository : IProductRepository
{
    // Connection string wstrzykujemy przez konstruktor (DI!)
    private readonly string _connectionString;

    public ProductRepository(IConfiguration configuration)
    {
        // Pobieramy connection string z appsettings.json
        _connectionString = configuration.GetConnectionString("DefaultConnection")
                            ?? throw new InvalidOperationException(
                                "Brak ConnectionString 'DefaultConnection' w konfiguracji!");

        Console.WriteLine(configuration.GetConnectionString("DefaultConnection"));
    }

    public async Task<bool> TestConnectionAsync()
    {
        try
        {
            // using gwarantuje zamknięcie połączenia
            using var connection = new SqlConnection(_connectionString);
              
            // Asynchroniczne otwarcie połączenia
            await connection.OpenAsync();
              
            // Jeśli dotarliśmy tutaj — połączenie działa!
            // connection.State powinno zwrócić ConnectionState.Open
            Console.WriteLine($"Stan połączenia: {connection.State}");
            Console.WriteLine($"Wersja serwera: {connection.ServerVersion}");
              
            return true;
        }
        catch (SqlException ex)
        {
            // SqlException — błąd specyficzny dla SQL Server
            Console.WriteLine($"Błąd połączenia z bazą: {ex.Message}");
            return false;
        }
    } // <-- connection.Dispose() — połączenie zamknięte
}