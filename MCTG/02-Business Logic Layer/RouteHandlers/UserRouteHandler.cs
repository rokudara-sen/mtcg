using MCTG._01_Shared;
using MCTG._02_Business_Logic_Layer.Interfaces;
using MCTG._03_Data_Access_Layer.DataContext;
using MCTG._06_Domain.Entities;

namespace MCTG._02_Business_Logic_Layer.RouteHandlers;

public class UserRouteHandler : IRouteHandler
{
    private readonly UserDataContext _dataContext = new(GlobalRegistry._connectionString);

    public async Task<OperationResult> RegisterUserAsync(UserCredentials credentials)
    {
        var existingUser = GetUserByUsername(credentials.Username);
        if (existingUser != null)
        {
            return new OperationResult { Success = false, ErrorMessage = "User already exists." };
        }
        
        credentials.Password = BCrypt.Net.BCrypt.HashPassword(credentials.Password);

        var user = new User(credentials.Username, credentials.Password);
        _dataContext.Add(user);

        return new OperationResult { Success = true };
    }

    public async Task<OperationResult> LoginUserAsync(UserCredentials credentials)
    {
        var user = GetUserByUsername(credentials.Username);
        if (user == null)
        {
            return new OperationResult { Success = false, ErrorMessage = "Invalid username or password." };
        }
        
        if (!BCrypt.Net.BCrypt.Verify(credentials.Password, user.Password))
        {
            return new OperationResult { Success = false, ErrorMessage = "Invalid username or password." };
        }
        
        user.Authorization = GenerateAuthToken(user.Username);
        
        _dataContext.Update(user);

        return new OperationResult { Success = true, Data = user.Authorization };
    }

    public User GetUserByAuthToken(string authToken)
    {
        return _dataContext.GetByAuthToken<User>(authToken);
    }
    
    public User GetUserByUsername(string username)
    {
        return _dataContext.GetByUsername<User>(username);
    }

    public OperationResult UpdateUser(string authToken, User updatedUser)
    {
        var existingUser = GetUserByAuthToken(authToken);
        if (existingUser == null)
        {
            return new OperationResult { Success = false, ErrorMessage = "User not found." };
        }
        
        existingUser.Elo = updatedUser.Elo;
        existingUser.Gold = updatedUser.Gold;
        existingUser.Wins = updatedUser.Wins;
        existingUser.Losses = updatedUser.Losses;
        
        _dataContext.Update(existingUser);

        return new OperationResult { Success = true };
    }

    private string GenerateAuthToken(string username)
    {
        return $"{username}-mtcgToken";
    }
}