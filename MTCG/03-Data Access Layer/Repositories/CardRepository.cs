using System.Data;
using MTCG._03_Data_Access_Layer.Interfaces;
using MTCG._06_Domain.Entities;
using MTCG._06_Domain.Enums;

namespace MTCG._03_Data_Access_Layer.Repositories
{
    public class CardRepository : ICardRepository
    {
        private readonly IDataContext _dataContext;

        public CardRepository(IDataContext dataContext)
        {
            _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        }

        public void AddCard(Card card)
        {
            if (card == null)
                throw new ArgumentNullException(nameof(card));

            using IDbConnection connection = _dataContext.CreateConnection();
            connection.Open();

            using IDbCommand command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO mtcgdatabase.public.cards 
                (""cardName"", ""baseDamage"", ""elementType"", ""cardType"", identifier) 
                VALUES (@cardName, @damage, @elementType, @cardType, @identifier) 
                RETURNING cardid";

            AddParameterWithValue(command, "@cardName", DbType.String, card.CardName);
            AddParameterWithValue(command, "@damage", DbType.Int32, card.Damage);
            AddParameterWithValue(command, "@elementType", DbType.Int32, (int)card.ElementType);
            AddParameterWithValue(command, "@cardType", DbType.Int32, (int)card.CardType);
            AddParameterWithValue(command, "@identifier", DbType.String, card.Id);

            card.CardId = Convert.ToInt32(command.ExecuteScalar());
        }

        public Card? GetCardById(int cardId)
        {
            using IDbConnection connection = _dataContext.CreateConnection();
            connection.Open();

            using IDbCommand command = connection.CreateCommand();
            command.CommandText = @"
                SELECT cardid, ""cardName"", ""baseDamage"", ""elementType"", ""cardType"", identifier 
                FROM mtcgdatabase.public.cards 
                WHERE cardid = @cardId";

            AddParameterWithValue(command, "@cardId", DbType.Int32, cardId);

            using IDataReader reader = command.ExecuteReader();
            return reader.Read() ? MapReaderToCard(reader) : null;
        }

        public Card? GetCardByIdentifier(string identifier)
        {
            using IDbConnection connection = _dataContext.CreateConnection();
            connection.Open();

            using IDbCommand command = connection.CreateCommand();
            command.CommandText = @"
                SELECT cardid, ""cardName"", ""baseDamage"", ""elementType"", ""cardType"", identifier 
                FROM mtcgdatabase.public.cards 
                WHERE identifier = @identifier";

            AddParameterWithValue(command, "@identifier", DbType.String, identifier);

            using IDataReader reader = command.ExecuteReader();
            return reader.Read() ? MapReaderToCard(reader) : null;
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
            parameter.Value = value;
            command.Parameters.Add(parameter);
        }
    }
}
