using MTCG._01_Shared;
using MTCG._01_Shared.Enums;
using MTCG._02_Business_Logic_Layer.Interfaces;
using MTCG._03_Data_Access_Layer.DataContext;
using MTCG._06_Domain.Entities;
using MTCG._06_Domain.ValueObjects;

namespace MTCG._02_Business_Logic_Layer.RouteHandlers;

public class CardRouteHandler : IRouteHandler
{
    private readonly CardDataContext _dataContext = new(GlobalRegistry._connectionString);

    public OperationResult CreateCard(Card? card)
    {
        if (card == null)
        {
            return new OperationResult { Success = false, ErrorMessage = "Card cannot be null." };
        }

        if (string.IsNullOrWhiteSpace(card.CardName) || card.Damage <= 0 || string.IsNullOrWhiteSpace(card.ElementType.ToString()) ||
            string.IsNullOrWhiteSpace(card.CardType.ToString()) || string.IsNullOrWhiteSpace(card.Id))
        {
            return new OperationResult { Success = false, ErrorMessage = "Invalid card." };
        }

        if (CardAlreadyExist(card))
        {
            return new OperationResult { Success = false, ErrorMessage = "Card already exist." };
        }
        
        SpecifyCardElement(card);
        SpecifyCardType(card);
        _dataContext.Add(card);

        return new OperationResult { Success = true };
    }

    public bool CardAlreadyExist(Card card)
    {
        var tempCard = _dataContext.GetByStringId<Card>(card.Id);
        return tempCard != null;
    }

    private void SpecifyCardType(Card card)
    {
        if (card.CardName.Contains("Spell"))
        {
            card.CardType = CardType.SpellCard;
        }
        else if (card.CardName.Contains("Goblin") || card.CardName.Contains("Dragon") || card.CardName.Contains("Elf") || card.CardName.Contains("Knight") || card.CardName.Contains("Kraken") || card.CardName.Contains("Wizard") || card.CardName.Contains("Ork"))
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
            card.ElementType = ElementType.Water;
        }
        else
        {
            card.ElementType = ElementType.NotDefined;
        }
    }
}