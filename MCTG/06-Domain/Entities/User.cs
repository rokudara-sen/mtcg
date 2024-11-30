using System.Diagnostics;
using MCTG._06_Domain.ValueObjects;

namespace MCTG._06_Domain.Entities;

public class User
{
    public User()
    {
        
    }

    public User(string username, string password)
    {
        UserId = 0;
        Username = username;
        Password = password;
        Elo = 100;
        Gold = 20;
        PlayerDeck = new Deck(new List<Card>());
        Wins = 0;
        Losses = 0;
    }
    
    public int UserId { get; set; }
    public string Username { get; set; }
    
    public string Password { get; set; }
    
    public int Elo { get; set; }
    
    public int Gold { get; set; }
    
    public Deck PlayerDeck { get; set; }
    
    public int Wins { get; set; }
    
    public int Losses { get; set; }
    
    public string Authorization { get; set; }
}