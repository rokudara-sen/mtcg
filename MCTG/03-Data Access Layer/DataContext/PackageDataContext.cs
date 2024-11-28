using System.Data;
using MCTG._03_Data_Access_Layer.Interfaces;
using MCTG._06_Domain.Entities;
using MCTG._06_Domain.ValueObjects;
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
    
    public void CreatePackage<T>(T entity) where T : class
    {
        if (typeof(T) == typeof(Package))
        {
            CreatePackage(entity as Package);
        }
        else
        {
            throw new NotSupportedException($"Add<{typeof(T).Name}> is not supported");
        }
    }
    
    // private void CreatePackage(Package package)
    // {
    //     using IDbConnection connection = CreateConnection();
    //     connection.Open();
    //     using IDbCommand command = connection.CreateCommand();
    //     
    //     command.CommandText = "INSERT INTO packages (cardSlot1, cardSlot2, cardSlot3, cardSlot4, cardSlot5, cost) VALUES (@cardSlot1, @cardSlot2, @cardSlot3, @cardSlot4, @cardSlot5, @cost) RETURNING packageid";
    //     AddParameterWithValue(command, "@cardSlot1", DbType.String, package.Cards);
    //     AddParameterWithValue(command, "@cardSlot2", DbType.String, package.Cards);
    //     AddParameterWithValue(command, "@cardSlot3", DbType.String, package.Cards);
    //     AddParameterWithValue(command, "@cardSlot4", DbType.String, package.Cards);
    //     AddParameterWithValue(command, "@cardSlot5", DbType.String, package.Cards);
    //     AddParameterWithValue(command, "@cost", DbType.String, package.Cost);
    //     
    //     user.UserId = Convert.ToInt32(command.ExecuteScalar());
    // }
    
    private void AddParameterWithValue(IDbCommand command, string parameterName, DbType type, object value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = parameterName;
        parameter.DbType = type;
        parameter.Value = value ?? DBNull.Value;
        command.Parameters.Add(parameter);
    }
}