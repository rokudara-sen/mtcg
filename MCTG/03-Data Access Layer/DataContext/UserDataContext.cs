using System.Data;
using MCTG._03_Data_Access_Layer.Interfaces;
using MCTG._06_Domain.Entities;
using Npgsql;

namespace MCTG._03_Data_Access_Layer.DataContext;

public class UserDataContext : IDataContext
{
    private readonly string _connectionString;
    
    public UserDataContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection CreateConnection()
    {
        return new NpgsqlConnection(_connectionString);
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

    public void Remove<T>(int id) where T : class
    {
        if (typeof(T) == typeof(User))
        {
            RemoveUser(id);
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

    public T GetById<T>(int id) where T : class
    {
        if (typeof(T) == typeof(User))
        {
            return GetUserById(id) as T;
        }
        else
        {
            throw new NotSupportedException($"Get<{typeof(T).Name}> is not supported.");
        }
    }
    
    public T GetByUsername<T>(string username) where T : class
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
    
    public T GetByAuthToken<T>(string authToken) where T : class
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

    private void AddUser(User user)
    {
        using IDbConnection connection = CreateConnection();
        connection.Open();
        using IDbCommand command = connection.CreateCommand();
        
        command.CommandText = "INSERT INTO users (username, password) VALUES (@username, @password) RETURNING id";
        AddParameterWithValue(command, "@username", DbType.String, user.Username);
        AddParameterWithValue(command, "@password", DbType.String, user.Password);
        
        user.UserId = Convert.ToInt32(command.ExecuteScalar());
    }

    private void RemoveUser(int userId)
    {
        using IDbConnection connection = CreateConnection();
        connection.Open();
        using IDbCommand command = connection.CreateCommand();
        
        command.CommandText = "DELETE FROM users WHERE id = @id";
        AddParameterWithValue(command, "@id", DbType.Int32, userId);
        
        command.ExecuteNonQuery();
    }

    private void UpdateUser(User user)
    {
        if (user.UserId == null)
        {
            throw new ArgumentNullException("User id cannot be null");
        }
        
        using IDbConnection connection = CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            using IDbCommand command = connection.CreateCommand();
            command.Transaction = transaction;

            command.CommandText = @"UPDATE users SET elo = @elo, gold = @gold, wins = @wins, losses = @losses, ""authToken"" = @authToken WHERE ""userID"" = @id";
            AddParameterWithValue(command, "@elo", DbType.String, user.Elo);
            AddParameterWithValue(command, "@gold", DbType.Int32, user.Gold);
            AddParameterWithValue(command, "@wins", DbType.Int32, user.Wins);
            AddParameterWithValue(command, "@losses", DbType.Int32, user.Losses);
            AddParameterWithValue(command, "@authToken", DbType.String, user.AuthToken);
            command.ExecuteNonQuery();
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    private User GetUserById(int userId)
    {
        using IDbConnection connection = CreateConnection();
        connection.Open();
        using IDbCommand command = connection.CreateCommand();
        
        command.CommandText = "SELECT userID, username, password, elo, gold, wins, losses, authtoken FROM users WHERE id = @id";
        AddParameterWithValue(command, "@id", DbType.Int32, userId);
        
        using IDataReader reader = command.ExecuteReader();
        if (reader.Read())
        {
            return MapReaderToUser(reader);
        }
        return null;
    }
    
    private User GetUserByUsername(string username)
    {
        using IDbConnection connection = CreateConnection();
        connection.Open();
        using IDbCommand command = connection.CreateCommand();
        
        command.CommandText = "SELECT userID, username, password, elo, gold, wins, losses, authtoken FROM users WHERE username = @username";
        AddParameterWithValue(command, "@username", DbType.Int32, username);
        
        using IDataReader reader = command.ExecuteReader();
        if (reader.Read())
        {
            return MapReaderToUser(reader);
        }
        return null;
    }
    
    private User GetUserByAuthToken(string authToken)
    {
        using IDbConnection connection = CreateConnection();
        connection.Open();
        using IDbCommand command = connection.CreateCommand();
        
        command.CommandText = @"SELECT ""userID"", username, password, elo, gold, wins, losses, ""authToken"" FROM users WHERE ""authToken"" = @authToken";
        AddParameterWithValue(command, "@authToken", DbType.String, authToken);
        
        using IDataReader reader = command.ExecuteReader();
        if (reader.Read())
        {
            return MapReaderToUser(reader);
        }
        return null;
    }
    
    private User MapReaderToUser(IDataReader reader)
    {
        return new User
        {
            UserId = reader.GetInt32(0),
            Username = reader.GetString(1),
            Password = reader.GetString(2),
            Elo = reader.GetInt32(3),
            Gold = reader.GetInt32(4),
            Wins = reader.GetInt32(5),
            Losses = reader.GetInt32(6),
            AuthToken = reader.IsDBNull(7) ? null : reader.GetString(7)
        };
    }


    private IEnumerable<User> GetAllUsers()
    {
        var users = new List<User>();
        
        using IDbConnection connection = CreateConnection();
        connection.Open();
        using IDbCommand command = connection.CreateCommand();
        
        command.CommandText = "SELECT userID, username, password, elo, gold, wins, losses, authtoken FROM users";
        
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
                AuthToken = reader.GetString(7)
            });
        }
        
        return users;
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