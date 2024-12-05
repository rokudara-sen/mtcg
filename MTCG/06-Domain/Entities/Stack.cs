namespace MTCG._06_Domain.Entities;

public class Stack
{
    public Stack(List<Card> playerStack)
    {
        PlayerStack = playerStack;
    }
    
    public List<Card> PlayerStack { get; set; }
}