using System.Data;
using MTCG._03_Data_Access_Layer.Interfaces;
using MTCG._06_Domain.Entities;
using Npgsql;

namespace MTCG._03_Data_Access_Layer.Repositories;

public class StackRepository(string connectionString) : IDataContext
{
    public IDbConnection CreateConnection()
    {
        return new NpgsqlConnection(connectionString);
    }
    
    public void Dispose()
    {
    }
    
    public void Add<T>(T entity) where T : class
    {
        if (typeof(T) == typeof(Card))
        {
            AddCard(entity as Card);
        }
        else
        {
            throw new NotSupportedException($"Add<{typeof(T).Name}> is not supported");
        }
    }

    public void Remove<T>(int tableId) where T : class
    {
        throw new NotImplementedException();
    }

    public void Update<T>(T entity) where T : class
    {
        throw new NotImplementedException();
    }

    public T? GetById<T>(int tableId) where T : class
    {
        throw new NotImplementedException();
    }

    public T? GetByStringId<T>(string id) where T : class
    {
        if (typeof(T) == typeof(Card))
        {
            return GetByIdInStash(id) as T;
        }
        else
        {
            throw new NotSupportedException($"Get<{typeof(T).Name}> is not supported.");
        }
    }

    public void IncreaseCardAmount<T>(string id)
    {
        if (typeof(T) == typeof(Card))
        {
            IncreaseCardAmountInStash(id);
        }
        else
        {
            throw new NotSupportedException($"Get<{typeof(T).Name}> is not supported.");
        }
    }
    
    private void AddCard(Card card)
    {
        using IDbConnection connection = CreateConnection();
        connection.Open();
        using IDbCommand command = connection.CreateCommand();
        
        
        card.CardId = Convert.ToInt32(command.ExecuteScalar());
    }
    
    private void IncreaseCardAmountInStash(string id)
    {
        using IDbConnection connection = CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            using IDbCommand command = connection.CreateCommand();
            command.Transaction = transaction;

            command.CommandText = @"UPDATE mtcgdatabase.public.stacks s SET amount = @amount FROM mtcgdatabase.public.cards c WHERE s.cardid = c.cardid AND c.identifier = @id";
            AddParameterWithValue(command, "@amount", DbType.Int32, GetCurrentCardAmountInStash(id) + 1);
            AddParameterWithValue(command, "@id", DbType.Int32, id);
            command.ExecuteNonQuery();
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    private int GetCurrentCardAmountInStash(string id)
    {
        using IDbConnection connection = CreateConnection();
        connection.Open();
        using IDbCommand command = connection.CreateCommand();
        
        command.CommandText = "SELECT s.amount FROM mtcgdatabase.public.stacks s JOIN mtcgdatabase.public.cards c ON s.cardid WHERE c.identifier = @id";
        AddParameterWithValue(command, "@id", DbType.Int32, id);
        
        object? result = command.ExecuteScalar();
        return result != null ? Convert.ToInt32(result) : 0;
    }

    private Card? GetByIdInStash(string id)
    {
        using IDbConnection connection = CreateConnection();
        connection.Open();
        using IDbCommand command = connection.CreateCommand();
        
        command.CommandText = "SELECT c.identifier FROM mtcgdatabase.public.stacks s JOIN mtcgdatabase.public.cards c ON s.cardid WHERE c.identifier = @id";
        AddParameterWithValue(command, "@id", DbType.Int32, id);
        
        using IDataReader reader = command.ExecuteReader();
        if (reader.Read())
        {
            return MapReaderToCard(reader);
        }
        return null;
    }
    
    private static Card MapReaderToCard(IDataReader reader)
    {
        return new Card
        {
            Id = reader.GetString(5),
        };
    }
    
    private static void AddParameterWithValue(IDbCommand command, string parameterName, DbType type, object value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = parameterName;
        parameter.DbType = type;
        parameter.Value = value ?? DBNull.Value;
        command.Parameters.Add(parameter);
    }
}