using System.Data;
using MTCG._03_Data_Access_Layer.Interfaces;
using Npgsql;

namespace MTCG._03_Data_Access_Layer.DataContext;

public class DataContext : IDataContext
{
    private readonly string _connectionString;

    public DataContext(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public IDbConnection CreateConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }

    public void Dispose()
    {
        // Dispose resources if needed
    }
}