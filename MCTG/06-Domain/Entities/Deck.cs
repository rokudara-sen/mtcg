using MCTG._06_Domain.ValueObjects;

namespace MCTG._06_Domain.Entities;

public class Deck
{
    public Deck(List<Card> playerDeck)
    {
        playerDeck = PlayerDeck;
    }
    
    public List<Card> PlayerDeck { get; set; }
}