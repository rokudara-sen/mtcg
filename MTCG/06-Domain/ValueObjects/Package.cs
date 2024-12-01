using System.Collections.Concurrent;

namespace MTCG._06_Domain.ValueObjects;

public class Package
{
    public Package(List<Card> cards)
    {
        Cards = cards;
        Cost = 5;
    }
    
    public List<Card> Cards { get; set; }
    
    public int PackageId { get; set; }
    public int Cost { get; private set; }
}