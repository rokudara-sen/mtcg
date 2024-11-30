using System.Text.Json.Serialization;
using MTCG._01_Shared.Enums;

namespace MTCG._06_Domain.ValueObjects;

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
    
    [JsonPropertyName("Name")]
    public string CardName { get; set; }
    
    [JsonPropertyName("Damage")]
    public int Damage { get; set; }
    
    public ElementType ElementType { get; set; }
    
    public CardType CardType { get; set; }
    
    [JsonPropertyName("Id")]
    public string Id { get; set; }
}