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
        if (card != null)
        {
            return new OperationResult { Success = false, ErrorMessage = "Card cannot be null" };
        }

        if (card.CardId == null || card.CardName == null || card.BaseDamage == null || card.ElementType == null ||
            card.CardType == null || card.Identifier == null)
        {
            return new OperationResult { Success = false, ErrorMessage = "Invalid card" };
        }
       
        // createPackageShit

        return new OperationResult { Success = true };
    }
}