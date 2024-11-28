using System.Data;
using MCTG._03_Data_Access_Layer.Interfaces;
using MCTG._06_Domain.ValueObjects;
using Npgsql;

namespace MCTG._03_Data_Access_Layer.DataContext;

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
    
    private void AddCard(Card card)
    {
        using IDbConnection connection = CreateConnection();
        connection.Open();
        using IDbCommand command = connection.CreateCommand();

        command.CommandText = "INSERT INTO cards (cardName, baseDamage, elementType, cardType, identifier) VALUES (@cardName, @baseDamage, @elementType, @cardType, @identifier) RETURNING cardid";
        AddParameterWithValue(command, "@cardName", DbType.String, card.CardName);
        AddParameterWithValue(command, "@baseDamage", DbType.Int32, card.BaseDamage);
        AddParameterWithValue(command, "@elementType", DbType.String, card.ElementType);
        AddParameterWithValue(command, "@cardType", DbType.String, card.CardType);
        AddParameterWithValue(command, "@identifier", DbType.String, card.Identifier);
        
        
        card.CardId = Convert.ToInt32(command.ExecuteScalar());
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