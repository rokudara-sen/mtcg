using System.Data;
using MTCG._03_Data_Access_Layer.Interfaces;
using MTCG._06_Domain.Entities;

namespace MTCG._03_Data_Access_Layer.Repositories
{
    public class PackageRepository : IPackageRepository
    {
        private readonly IDataContext _dataContext;
        private readonly ICardRepository _cardRepository;

        public PackageRepository(IDataContext dataContext, ICardRepository cardRepository)
        {
            _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            _cardRepository = cardRepository ?? throw new ArgumentNullException(nameof(cardRepository));
        }

        public void AddPackage(Package package)
        {
            if (package == null)
                throw new ArgumentNullException(nameof(package));

            if (package.Cards.Count != 5)
                throw new ArgumentException("A package must contain exactly 5 cards.");

            // Ensure all cards exist
            foreach (var card in package.Cards)
            {
                var existingCard = _cardRepository.GetCardByIdentifier(card.Id);
                if (existingCard == null)
                {
                    _cardRepository.AddCard(card);
                }
                else
                {
                    card.CardId = existingCard.CardId;
                }
            }

            var cardIds = package.Cards.Select(c => c.CardId).ToList();

            using IDbConnection connection = _dataContext.CreateConnection();
            connection.Open();

            using IDbCommand command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO mtcgdatabase.public.packages 
                (""cardSlot1"", ""cardSlot2"", ""cardSlot3"", ""cardSlot4"", ""cardSlot5"", cost) 
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

        public Package GetPackageById(string packageId)
        {
            throw new NotImplementedException();
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
