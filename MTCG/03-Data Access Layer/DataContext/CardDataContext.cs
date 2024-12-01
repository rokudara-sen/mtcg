using System.Data;
using MTCG._01_Shared.Enums;
using MTCG._03_Data_Access_Layer.Interfaces;
using MTCG._06_Domain.ValueObjects;
using Npgsql;

namespace MTCG._03_Data_Access_Layer.DataContext;

public class CardDataContext(string connectionString) : IDataContext
{
    public IDbConnection CreateConnection()
    {
        return new NpgsqlConnection(connectionString);
    }
    
    public void Dispose()
    {
        throw new NotImplementedException();
    }
    
    public void Add<T>(T entity) where T : class?
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }
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
            return GetCardById(id) as T;
        }
        else
        {
            throw new NotSupportedException($"Get<{typeof(T).Name}> is not supported.");
        }
    }
    
    private void AddCard(Card? card)
    {
        using IDbConnection connection = CreateConnection();
        connection.Open();
        using IDbCommand command = connection.CreateCommand();
        command.CommandText = @"INSERT INTO mtcgdatabase.public.cards (""cardName"", ""baseDamage"", ""elementType"", ""cardType"", identifier) 
                                    VALUES (@cardName, @damage, @elementType, @cardType, @identifier) RETURNING cardid";
        AddParameterWithValue(command, "@cardName", DbType.String, card.CardName);
        AddParameterWithValue(command, "@damage", DbType.Int32, card.Damage);
        AddParameterWithValue(command, "@elementType", DbType.Int32, (int)card.ElementType);
        AddParameterWithValue(command, "@cardType", DbType.Int32, (int)card.CardType);
        AddParameterWithValue(command, "@identifier", DbType.String, card.Id);
        
        
        card.CardId = Convert.ToInt32(command.ExecuteScalar());
    }

    private Card? GetCardById(string id)
    {
        using IDbConnection connection = CreateConnection();
        connection.Open();
        using IDbCommand command = connection.CreateCommand();
        
        command.CommandText = "SELECT identifier FROM mtcgdatabase.public.cards WHERE identifier = @id";
        AddParameterWithValue(command, "@id", DbType.String, id);
        
        using IDataReader reader = command.ExecuteReader();
        if (reader.Read())
        {
            return MapReaderToCard(reader);
        }
        return null;
    }
    
    private Card MapReaderToCard(IDataReader reader)
    {
        return new Card
        {
            CardId = reader.GetInt32(0),
            CardName = reader.GetString(1),
            Damage = reader.GetInt32(2),
            ElementType = (ElementType)reader.GetInt32(3),
            CardType = (CardType)reader.GetInt32(4),
            Id = reader.GetString(5)
        };
    }
    
    private void AddParameterWithValue(IDbCommand command, string parameterName, DbType type, object value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = parameterName;
        parameter.DbType = type;
        parameter.Value = value ?? DBNull.Value;
        command.Parameters.Add(parameter);
    }
}