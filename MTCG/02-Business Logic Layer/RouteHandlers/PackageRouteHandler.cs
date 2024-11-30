using MTCG._01_Shared;
using MTCG._02_Business_Logic_Layer.Interfaces;
using MTCG._03_Data_Access_Layer.DataContext;
using MTCG._06_Domain.Entities;
using MTCG._06_Domain.ValueObjects;

namespace MTCG._02_Business_Logic_Layer.RouteHandlers;

public class PackageRouteHandler : IRouteHandler
{
    private readonly PackageDataContext _dataContext = new(GlobalRegistry._connectionString);
    private readonly UserRouteHandler _userRouteHandler = new();
    
    public OperationResult CreatePackage(User user, Package package)
    {
        if (!_userRouteHandler.IsValidUser(user))
        {
            return new OperationResult { Success = false, ErrorMessage = "User data is invalid." };
        }
        var tempUser = _userRouteHandler.GetUserByAuthToken(user.Authorization);
        if (tempUser == null || !CheckIfAdmin(tempUser))
        {
            return new OperationResult { Success = false, ErrorMessage = "Not Authorized" };
        }
       
        foreach (var card in package.Cards)
        {
            _dataContext.Add(card);
        }
        
        _dataContext.Add(package);

        return new OperationResult { Success = true };
    }

    public bool CheckIfAdmin(User? user)
    {
        if (user == null || string.IsNullOrWhiteSpace(user.Authorization))
        {
            return false;
        }
            
        var tempUser = _userRouteHandler.GetUserByAuthToken(user.Authorization);
        return tempUser is { Authorization: "admin-mtcgToken" };
    }
}