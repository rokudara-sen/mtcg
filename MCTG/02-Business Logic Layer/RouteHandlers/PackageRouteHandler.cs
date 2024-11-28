using MCTG._01_Shared;
using MCTG._02_Business_Logic_Layer.Interfaces;
using MCTG._03_Data_Access_Layer.DataContext;
using MCTG._06_Domain.Entities;

namespace MCTG._02_Business_Logic_Layer.RouteHandlers;

public class PackageRouteHandler : IRouteHandler
{
    private readonly PackageDataContext _dataContext = new(GlobalRegistry._connectionString);
    private readonly UserRouteHandler _userRouteHandler = new();

    public async Task<OperationResult> CreatePackageAsync(User user)
    {
        var tempUser = _userRouteHandler.GetUserByAuthToken(user.Authorization);
        if (tempUser != null || !CheckIfAdmin(tempUser))
        {
            return new OperationResult { Success = false, ErrorMessage = "Not Authorized" };
        }
       
        // createPackageShit

        return new OperationResult { Success = true };
    }

    private bool CheckIfAdmin(User user)
    {
        var tempUser = _userRouteHandler.GetUserByAuthToken(user.Authorization);
        return tempUser != null && tempUser.Authorization == "admin-mtcgToken";
    }
}