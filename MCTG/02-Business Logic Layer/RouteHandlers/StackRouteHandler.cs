using MCTG._01_Shared;
using MCTG._02_Business_Logic_Layer.Interfaces;
using MCTG._03_Data_Access_Layer.DataContext;
using MCTG._06_Domain.Entities;
using MCTG._06_Domain.ValueObjects;

namespace MCTG._02_Business_Logic_Layer.RouteHandlers;

public class StackRouteHandler : IRouteHandler
{
    private readonly StackDataContext _dataContext = new(GlobalRegistry._connectionString);
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
        
        _dataContext.Add(card);

        return new OperationResult { Success = true };
    }
    
    private bool CardAlreadyExistInStash(Card card)
    {
        var tempCard = _dataContext.GetById<Card>(card.Id);
        if (tempCard.Id == card.Id)
            return true;
        return false;
    }
    
    private void IncreaseCardAmountInStash(Card card)
    {
        _dataContext.IncreaseCardAmount<Card>(card.Id);
    }
}