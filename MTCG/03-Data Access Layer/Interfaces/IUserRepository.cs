using MTCG._06_Domain.Entities;

namespace MTCG._03_Data_Access_Layer.Interfaces;

public interface IUserRepository
{
    void AddUser(User user);
    User? GetUserById(int userId);
    User? GetUserByUsername(string username);
    User? GetUserByAuthToken(string token);
    public void UpdateUser(User? user);
}