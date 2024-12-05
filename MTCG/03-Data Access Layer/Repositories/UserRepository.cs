using System.Data;
using MTCG._03_Data_Access_Layer.Interfaces;
using MTCG._06_Domain.Entities;

namespace MTCG._03_Data_Access_Layer.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDataContext _dataContext;

        public UserRepository(IDataContext dataContext)
        {
            _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        }

        public void AddUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            using IDbConnection connection = _dataContext.CreateConnection();
            connection.Open();

            using IDbCommand command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO mtcgdatabase.public.users 
                (username, password) 
                VALUES (@username, @password) 
                RETURNING userid";

            AddParameterWithValue(command, "@username", DbType.String, user.Username);
            AddParameterWithValue(command, "@password", DbType.String, user.Password);

            user.UserId = Convert.ToInt32(command.ExecuteScalar());
        }

        public User? GetUserById(int userId)
        {
            using IDbConnection connection = _dataContext.CreateConnection();
            connection.Open();

            using IDbCommand command = connection.CreateCommand();
            command.CommandText = @"
                SELECT userid, username, password, elo, gold, wins, losses, authtoken 
                FROM mtcgdatabase.public.users 
                WHERE userid = @userid";

            AddParameterWithValue(command, "@userid", DbType.Int32, userId);

            using IDataReader reader = command.ExecuteReader();
            return reader.Read() ? MapReaderToUser(reader) : null;
        }

        public User? GetUserByUsername(string username)
        {
            using IDbConnection connection = _dataContext.CreateConnection();
            connection.Open();

            using IDbCommand command = connection.CreateCommand();
            command.CommandText = @"
                SELECT userid, username, password, elo, gold, wins, losses, authtoken 
                FROM mtcgdatabase.public.users 
                WHERE username = @username";

            AddParameterWithValue(command, "@username", DbType.String, username);

            using IDataReader reader = command.ExecuteReader();
            return reader.Read() ? MapReaderToUser(reader) : null;
        }
        
        public User? GetUserByAuthToken(string authToken)
        {
            using IDbConnection connection = _dataContext.CreateConnection();
            connection.Open();

            using IDbCommand command = connection.CreateCommand();
        
            command.CommandText = """SELECT "userid", username, password, elo, gold, wins, losses, "authtoken" FROM mtcgdatabase.public.users WHERE "authtoken" = @authtoken""";
            AddParameterWithValue(command, "@authtoken", DbType.String, authToken);
        
            using var reader = command.ExecuteReader();
            return reader.Read() ? MapReaderToUser(reader) : null;
        }
        
        public void UpdateUser(User? user)
        {
            if (user == null || user.UserId == 0)
            {
                throw new InvalidDataException("User not found");
            }
        
            using IDbConnection connection = _dataContext.CreateConnection();
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
        
        public void UpdateUserWithConnection(User user, IDbConnection connection, IDbTransaction transaction)
        {
            if (user == null || user.UserId == 0)
            {
                throw new InvalidDataException("User not found");
            }

            using IDbCommand command = connection.CreateCommand();
            command.Transaction = transaction;

            command.CommandText = @"
        UPDATE mtcgdatabase.public.users 
        SET elo = @elo, gold = @gold, wins = @wins, losses = @losses, ""authtoken"" = @authtoken 
        WHERE ""userid"" = @userid";

            AddParameterWithValue(command, "@elo", DbType.Int32, user.Elo);
            AddParameterWithValue(command, "@gold", DbType.Int32, user.Gold);
            AddParameterWithValue(command, "@wins", DbType.Int32, user.Wins);
            AddParameterWithValue(command, "@losses", DbType.Int32, user.Losses);
            AddParameterWithValue(command, "@authtoken", DbType.String, user.Authorization);
            AddParameterWithValue(command, "@userid", DbType.Int32, user.UserId);

            command.ExecuteNonQuery();
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
                Authorization = reader.IsDBNull(7) ? string.Empty : reader.GetString(7)
            };
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
}
