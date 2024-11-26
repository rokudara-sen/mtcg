using MCTG._01_Shared.Enums;

namespace MCTG._06_Domain.ValueObjects;

public class Card
{
    public Card(string cardId, string cardName, int baseDamage, ElementType elementType, CardType cardType)
    {
        CardId = cardId;
        CardName = cardName;
        BaseDamage = baseDamage;
        ElementType = elementType;
        CardType = cardType;
    }
    
    public string CardId { get; set; }
    
    public string CardName { get; set; }
    
    public int BaseDamage { get; set; }
    
    public ElementType ElementType { get; set; }
    
    public CardType CardType { get; set; }
}