/*using MTCG._01_Shared;
using MTCG._02_Business_Logic_Layer.Interfaces;
using MTCG._03_Data_Access_Layer.DataContext;
using MTCG._06_Domain.Entities;
using MTCG._06_Domain.ValueObjects;

namespace MTCG._02_Business_Logic_Layer.RouteHandlers;

public class StackRouteHandler : IRouteHandler
{
    private readonly StackRepository _repository = new(GlobalRegistry._connectionString);
    private readonly CardRouteHandler _cardRouteHandler = new();

    public async Task<OperationResult> AddCardToStash(Card card)
    {
        if (card == null)
        {
            return new OperationResult { Success = false, ErrorMessage = "Card cannot be null" };
        }

        if (string.IsNullOrWhiteSpace(card.CardName) || card.Damage <= 0 || string.IsNullOrWhiteSpace(card.ElementType.ToString()) ||
            string.IsNullOrWhiteSpace(card.CardType.ToString()) || string.IsNullOrWhiteSpace(card.Id))
        {
            return new OperationResult { Success = false, ErrorMessage = "Invalid card" };
        }

        if (!_cardRouteHandler.CardAlreadyExist(card))
        {
            return new OperationResult { Success = false, ErrorMessage = "Invalid card" };
        }

        if (CardAlreadyExistInStash(card))
        {
            IncreaseCardAmountInStash(card);
            return new OperationResult { Success = true };
        }
        
        _repository.Add(card);

        return new OperationResult { Success = true };
    }
    
    private bool CardAlreadyExistInStash(Card card)
    {
        var tempCard = _repository.GetByStringId<Card>(card.Id);
        if (tempCard == null || tempCard.Id != card.Id)
            return false;
        return tempCard.Id == card.Id;
    }
    
    private void IncreaseCardAmountInStash(Card card)
    {
        _repository.IncreaseCardAmount<Card>(card.Id);
    }
}*/