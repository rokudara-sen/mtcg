using System.Data;
using MTCG._03_Data_Access_Layer.Interfaces;
using MTCG._06_Domain.Entities;

namespace MTCG._03_Data_Access_Layer.Repositories;

public class StackRepository : IStackRepository
{
    private readonly IDataContext _dataContext;

    public StackRepository(IDataContext dataContext)
    {
        _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
    }

    public void AddCardsToUserStack(int userId, List<Card> cards, IDbConnection connection, IDbTransaction transaction)
    {
        foreach (var card in cards)
        {
            using IDbCommand command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = @"
                INSERT INTO mtcgdatabase.public.stacks (userid, cardid, amount)
                VALUES (@userId, @cardId, 1)
                ON CONFLICT (userid, cardid) DO UPDATE SET amount = mtcgdatabase.public.stacks.amount + 1";

            AddParameterWithValue(command, "@userId", DbType.Int32, userId);
            AddParameterWithValue(command, "@cardId", DbType.Int32, card.CardId);

            command.ExecuteNonQuery();
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