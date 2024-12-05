using MTCG._02_Business_Logic_Layer.Interfaces;
using MTCG._03_Data_Access_Layer.Interfaces;
using MTCG._06_Domain.Entities;
using MTCG._06_Domain.Enums;

namespace MTCG._02_Business_Logic_Layer.RouteHandlers
{
    public class CardRouteHandler : IRouteHandler
    {
        private readonly ICardRepository _cardRepository;
        
        public CardRouteHandler(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository ?? throw new ArgumentNullException(nameof(cardRepository));
        }

        public OperationResult CreateCard(Card? card)
        {
            if (card == null)
            {
                return new OperationResult { Success = false, ErrorMessage = "Card cannot be null." };
            }

            if (string.IsNullOrWhiteSpace(card.CardName) || card.Damage <= 0 || string.IsNullOrWhiteSpace(card.Id))
            {
                return new OperationResult { Success = false, ErrorMessage = "Invalid card." };
            }

            if (CardAlreadyExists(card))
            {
                return new OperationResult { Success = false, ErrorMessage = "Card already exists." };
            }

            SpecifyCardElement(card);
            SpecifyCardType(card);
            _cardRepository.AddCard(card);

            return new OperationResult { Success = true };
        }

        private bool CardAlreadyExists(Card card)
        {
            var tempCard = _cardRepository.GetCardByIdentifier(card.Id);
            return tempCard != null;
        }

        private void SpecifyCardType(Card card)
        {
            if (card.CardName.Contains("Spell"))
            {
                card.CardType = CardType.SpellCard;
            }
            else if (card.CardName.Contains("Goblin") || card.CardName.Contains("Dragon") || card.CardName.Contains("Elf") ||
                     card.CardName.Contains("Knight") || card.CardName.Contains("Kraken") || card.CardName.Contains("Wizard") ||
                     card.CardName.Contains("Ork"))
            {
                card.CardType = CardType.MonsterCard;
            }
            else
            {
                card.CardType = CardType.NotDefined;
            }
        }

        private void SpecifyCardElement(Card card)
        {
            if (card.CardName.Contains("Water"))
            {
                card.ElementType = ElementType.Water;
            }
            else if (card.CardName.Contains("Fire"))
            {
                card.ElementType = ElementType.Fire;
            }
            else if (card.CardName.Contains("Normal"))
            {
                card.ElementType = ElementType.Normal;
            }
            else
            {
                card.ElementType = ElementType.NotDefined;
            }
        }
    }
}
