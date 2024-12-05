namespace MTCG._06_Domain.Entities;

public class Package
{
    public Package(List<Card> cards, int cost = 5)
    {
        Cards = cards ?? throw new ArgumentNullException(nameof(cards));
        Cost = cost;
    }

    public List<Card> Cards { get; set; }
    public int PackageId { get; set; }
    public int Cost { get; private set; }
}
