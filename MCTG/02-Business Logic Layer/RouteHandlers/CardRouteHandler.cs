using MCTG._01_Shared;
using MCTG._02_Business_Logic_Layer.Interfaces;
using MCTG._03_Data_Access_Layer.DataContext;
using MCTG._06_Domain.Entities;
using MCTG._06_Domain.ValueObjects;

namespace MCTG._02_Business_Logic_Layer.RouteHandlers;

public class CardRouteHandler : IRouteHandler
{
    private readonly CardDataContext _dataContext = new(GlobalRegistry._connectionString);

    public async Task<OperationResult> CreateCard(Card card)
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

        if (CardAlreadyExist(card))
        {
            return new OperationResult { Success = false, ErrorMessage = "Card already exist" };
        }
        
        _dataContext.Add(card);

        return new OperationResult { Success = true };
    }

    private bool CardAlreadyExist(Card card)
    {
        var tempCard = _dataContext.GetById<Card>(card.Id);
        if (tempCard.Id == card.Id)
            return true;
        return false;
    }
}