using System.Data;
using MTCG._06_Domain.Entities;
using MTCG._03_Data_Access_Layer.Interfaces;
using MTCG._06_Domain.ValueObjects;
using Npgsql;

namespace MTCG._03_Data_Access_Layer.DataContext;

public class PackageDataContext(string connectionString) : IDataContext
{
    private readonly CardDataContext _cardDataContext = new CardDataContext(connectionString);
    public IDbConnection CreateConnection()
    {
        return new NpgsqlConnection(connectionString);
    }

    public void Dispose()
    {
        
    }
    
    public void Add<T>(T entity) where T : class
    {
        if (typeof(T) == typeof(Package))
        {
            AddPackage(entity as Package);
        }
        else if (typeof(T) == typeof(Card))
        {
            _cardDataContext.Add(entity as Card);
        }
        else
        {
            throw new NotSupportedException($"Add<{typeof(T).Name}> is not supported");
        }
    }
    
    private void AddPackage(Package? package)
    {
        if (package == null)
        {
            throw new InvalidDataException("Package class cannot be null");
        }
        if (package.Cards.Count != 5)
        {
            throw new ArgumentException("A package must contain exactly 5 cards.");
        }
        var cardIds = package.Cards.Select(card => card.CardId).ToList();

        using IDbConnection connection = CreateConnection();
        connection.Open();
        using IDbCommand command = connection.CreateCommand();
        
        command.CommandText = @"
                INSERT INTO mtcgdatabase.public.packages (""cardSlot1"", ""cardSlot2"", ""cardSlot3"", ""cardSlot4"", ""cardSlot5"", cost) 
                VALUES (@cardSlot1, @cardSlot2, @cardSlot3, @cardSlot4, @cardSlot5, @cost) 
                RETURNING packageid";
            
        AddParameterWithValue(command, "@cardSlot1", DbType.Int32, cardIds[0]);
        AddParameterWithValue(command, "@cardSlot2", DbType.Int32, cardIds[1]);
        AddParameterWithValue(command, "@cardSlot3", DbType.Int32, cardIds[2]);
        AddParameterWithValue(command, "@cardSlot4", DbType.Int32, cardIds[3]);
        AddParameterWithValue(command, "@cardSlot5", DbType.Int32, cardIds[4]);
        AddParameterWithValue(command, "@cost", DbType.Int32, package.Cost);
        
        var packageIdObj = command.ExecuteScalar();
        if (packageIdObj != null && int.TryParse(packageIdObj.ToString(), out int packageId))
        {
            package.PackageId = packageId;
        }
        else
        {
            throw new Exception("Failed to retrieve the package ID after insertion.");
        }
    }
    
    private void AddParameterWithValue(IDbCommand command, string parameterName, DbType type, object value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = parameterName;
        parameter.DbType = type;
        parameter.Value = value;
        command.Parameters.Add(parameter);
    }
}