using System.Collections.Concurrent;

namespace MCTG._06_Domain.ValueObjects;

public class Package
{
    public Package(ConcurrentBag<Card> cards)
    {
        Cards = cards;
        Cost = 5;
    }
    
    public ConcurrentBag<Card> Cards { get; private set; }
    
    public int Cost { get; private set; }
}