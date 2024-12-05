using MTCG._06_Domain.Entities;

namespace MTCG._03_Data_Access_Layer.Interfaces;

public interface ICardRepository
{
    void AddCard(Card card);
    Card? GetCardById(int cardId);
    Card? GetCardByIdentifier(string identifier);
}