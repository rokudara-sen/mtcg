using System.Text.Json.Serialization;
using MTCG._01_Shared.Enums;

namespace MTCG._06_Domain.ValueObjects;

public class Card
{
    public Card(int cardId = -1, string cardName = "", float damage = -1.0f, ElementType elementType = ElementType.NotDefined, CardType cardType = CardType.NotDefined, string id = "")
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
    public float Damage { get; set; }
    
    public ElementType ElementType { get; set; }
    
    public CardType CardType { get; set; }
    
    [JsonPropertyName("Id")]
    public string Id { get; set; }
}