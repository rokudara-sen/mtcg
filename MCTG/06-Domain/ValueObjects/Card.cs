using MCTG._01_Shared.Enums;

namespace MCTG._06_Domain.ValueObjects;

public class Card
{
    public Card(int cardId, string cardName, int baseDamage, ElementType elementType, CardType cardType, string identifier)
    {
        CardId = cardId;
        CardName = cardName;
        BaseDamage = baseDamage;
        ElementType = elementType;
        CardType = cardType;
        Identifier = identifier;
    }
    
    public int CardId { get; set; }
    
    public string CardName { get; set; }
    
    public int BaseDamage { get; set; }
    
    public ElementType ElementType { get; set; }
    
    public CardType CardType { get; set; }
    
    public string Identifier { get; set; }
}