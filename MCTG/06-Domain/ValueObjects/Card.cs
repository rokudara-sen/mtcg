using MCTG._01_Shared.Enums;

namespace MCTG._06_Domain.ValueObjects;

public class Card
{
    public Card(int cardId = -1, string cardName = "NULL", int damage = -1, ElementType elementType = ElementType.None, CardType cardType = CardType.None, string id = "NULL")
    {
        CardId = cardId;
        CardName = cardName;
        Damage = damage;
        ElementType = elementType;
        CardType = cardType;
        Id = id;
    }
    
    public int CardId { get; set; }
    
    public string CardName { get; set; }
    
    public int Damage { get; set; }
    
    public ElementType ElementType { get; set; }
    
    public CardType CardType { get; set; }
    
    public string Id { get; set; }
}