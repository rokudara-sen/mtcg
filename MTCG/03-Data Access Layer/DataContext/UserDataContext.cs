using System.Data;
using MTCG._03_Data_Access_Layer.Interfaces;
using MTCG._06_Domain.Entities;
using Npgsql;

namespace MTCG._03_Data_Access_Layer.DataContext;

public class UserDataContext(string connectionString) : IDataContext
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
        if (typeof(T) == typeof(User))
        {
            AddUser(entity as User);
        }
        else
        {
            throw new NotSupportedException($"Add<{typeof(T).Name}> is not supported");
        }
    }

    public void Remove<T>(int userId) where T : class
    {
        if (typeof(T) == typeof(User))
        {
            RemoveUser(userId);
        }
        else
        {
            throw new NotSupportedException($"Remove<{typeof(T).Name}> is not supported.");
        }
    }

    public void Update<T>(T entity) where T : class
    {
        if (typeof(T) == typeof(User))
        {
            UpdateUser(entity as User);
        }
        else
        {
            throw new NotSupportedException($"Update<{typeof(T).Name}> is not supported.");
        }
    }

    public T? GetById<T>(int userId) where T : class
    {
        if (typeof(T) == typeof(User))
        {
            return GetUserById(userId) as T;
        }
        else
        {
            throw new NotSupportedException($"Get<{typeof(T).Name}> is not supported.");
        }
    }

    public T? GetByStringId<T>(string id) where T : class
    {
        throw new NotImplementedException();
    }

    public T? GetByUsername<T>(string username) where T : class
    {
        if (typeof(T) == typeof(User))
        {
            return GetUserByUsername(username) as T;
        }
        else
        {
            throw new NotSupportedException($"Get<{typeof(T).Name}> is not supported.");
        }
    }
    
    public T? GetByAuthToken<T>(string authToken) where T : class
    {
        if (typeof(T) == typeof(User))
        {
            return GetUserByAuthToken(authToken) as T;
        }
        else
        {
            throw new NotSupportedException($"Get<{typeof(T).Name}> is not supported.");
        }
    }

    public IEnumerable<T> GetAll<T>() where T : class
    {
        if (typeof(T) == typeof(User))
        {
            return GetAllUsers().Cast<T>();
        }
        else
        {
            throw new NotSupportedException($"GetAll<{typeof(T).Name}> is not supported.");
        }
    }

    private void AddUser(User? user)
    {
        if (user == null)
        {
            throw new InvalidDataException("User class cannot be null");
        }
        using IDbConnection connection = CreateConnection();
        connection.Open();
        using IDbCommand command = connection.CreateCommand();
        
        command.CommandText = "INSERT INTO mtcgdatabase.public.users (username, password) VALUES (@username, @password) RETURNING userid";
        AddParameterWithValue(command, "@username", DbType.String, user.Username);
        AddParameterWithValue(command, "@password", DbType.String, user.Password);
        
        user.UserId = Convert.ToInt32(command.ExecuteScalar());
    }

    private void RemoveUser(int userId)
    {
        using IDbConnection connection = CreateConnection();
        connection.Open();
        using IDbCommand command = connection.CreateCommand();
        
        command.CommandText = "DELETE FROM users WHERE userid = @userid";
        AddParameterWithValue(command, "@id", DbType.Int32, userId);
        
        command.ExecuteNonQuery();
    }
    
    /*Method to update all User specific data. Is recquired for battle
     logic and other stuff to keep track of accurate Gold, Wins, losses, etc.*/
    private void UpdateUser(User? user)
    {
        if (user == null || user.UserId == 0)
        {
            throw new InvalidDataException("User not found");
        }
        
        using IDbConnection connection = CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            using IDbCommand command = connection.CreateCommand();
            command.Transaction = transaction;

            command.CommandText = @"UPDATE mtcgdatabase.public.users SET elo = @elo, gold = @gold, wins = @wins, losses = @losses, ""authtoken"" = @authtoken WHERE ""userid"" = @userid";
            AddParameterWithValue(command, "@elo", DbType.Int32, user.Elo);
            AddParameterWithValue(command, "@gold", DbType.Int32, user.Gold);
            AddParameterWithValue(command, "@wins", DbType.Int32, user.Wins);
            AddParameterWithValue(command, "@losses", DbType.Int32, user.Losses);
            AddParameterWithValue(command, "@authtoken", DbType.String, user.Authorization);
            AddParameterWithValue(command, "@userid", DbType.Int32, user.UserId);
            command.ExecuteNonQuery();
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    private User? GetUserById(int userId)
    {
        using IDbConnection connection = CreateConnection();
        connection.Open();
        using IDbCommand command = connection.CreateCommand();
        
        command.CommandText = "SELECT userid, username, password, elo, gold, wins, losses, authtoken FROM mtcgdatabase.public.users WHERE userid = @userid";
        AddParameterWithValue(command, "@userid", DbType.Int32, userId);
        
        using IDataReader reader = command.ExecuteReader();
        if (reader.Read())
        {
            return MapReaderToUser(reader);
        }
        return null;
    }
    
    private User? GetUserByUsername(string username)
    {
        using IDbConnection connection = CreateConnection();
        connection.Open();
        using IDbCommand command = connection.CreateCommand();
        
        command.CommandText = "SELECT userid, username, password, elo, gold, wins, losses, authtoken FROM mtcgdatabase.public.users WHERE username = @username";
        AddParameterWithValue(command, "@username", DbType.String, username);
        
        using IDataReader reader = command.ExecuteReader();
        if (reader.Read())
        {
            return MapReaderToUser(reader);
        }
        return null;
    }
    
    private User? GetUserByAuthToken(string authToken)
    {
        using IDbConnection connection = CreateConnection();
        connection.Open();
        using IDbCommand command = connection.CreateCommand();
        
        command.CommandText = """SELECT "userid", username, password, elo, gold, wins, losses, "authtoken" FROM mtcgdatabase.public.users WHERE "authtoken" = @authtoken""";
        AddParameterWithValue(command, "@authtoken", DbType.String, authToken);
        
        using var reader = command.ExecuteReader();
        return reader.Read() ? MapReaderToUser(reader) : null;
    }
    
    
    /*Maps the new values to a User class class so that the most
     current values from the database can be used*/
    private User MapReaderToUser(IDataReader reader)
    {
        return new User()
        {
            UserId = reader.GetInt32(0),
            Username = reader.GetString(1),
            Password = reader.GetString(2),
            Elo = reader.GetInt32(3),
            Gold = reader.GetInt32(4),
            Wins = reader.GetInt32(5),
            Losses = reader.GetInt32(6),
            Authorization = reader.IsDBNull(7) ? string.Empty : reader.GetString(7)
        };
    }


    private IEnumerable<User> GetAllUsers()
    {
        var users = new List<User>();
        
        using IDbConnection connection = CreateConnection();
        connection.Open();
        using IDbCommand command = connection.CreateCommand();
        
        command.CommandText = "SELECT userid, username, password, elo, gold, wins, losses, authtoken FROM mtcgdatabase.public.users";
        
        using IDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            users.Add(new User
            {
                UserId = reader.GetInt32(0),
                Username = reader.GetString(1),
                Password = reader.GetString(2),
                Elo = reader.GetInt32(3),
                Gold = reader.GetInt32(4),
                Wins = reader.GetInt32(5),
                Losses = reader.GetInt32(6),
                Authorization = reader.GetString(7)
            });
        }
        
        return users;
    }
    
    /*Method to bind parameters with their respective
     value so that the code stays rather clean instead of
     it being cluttered.*/
    private void AddParameterWithValue(IDbCommand command, string parameterName, DbType type, object value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = parameterName;
        parameter.DbType = type;
        parameter.Value = value;
        command.Parameters.Add(parameter);
    }
}