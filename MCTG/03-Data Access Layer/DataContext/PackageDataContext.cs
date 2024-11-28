using System.Data;
using MCTG._03_Data_Access_Layer.Interfaces;
using Npgsql;

namespace MCTG._03_Data_Access_Layer.DataContext;

public class PackageDataContext(string connectionString) : IDataContext
{
    public IDbConnection CreateConnection()
    {
        return new NpgsqlConnection(connectionString);
    }

    public void Dispose()
    {
        
    }
}