using System.Data;
using MTCG._06_Domain.Entities;

namespace MTCG._03_Data_Access_Layer.Interfaces;

public interface IStackRepository
{
    void AddCardsToUserStack(int userId, List<Card> cards, IDbConnection connection, IDbTransaction transaction);
}