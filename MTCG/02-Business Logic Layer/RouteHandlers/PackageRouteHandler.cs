using MTCG._02_Business_Logic_Layer.Interfaces;
using MTCG._03_Data_Access_Layer.Interfaces;
using MTCG._06_Domain.Entities;

namespace MTCG._02_Business_Logic_Layer.RouteHandlers
{
    public class PackageRouteHandler : IRouteHandler
    {
        private readonly IPackageRepository _packageRepository;
        private readonly UserRouteHandler _userRouteHandler;

        public PackageRouteHandler(IPackageRepository packageRepository,
            UserRouteHandler userRouteHandler)
        {
            _packageRepository = packageRepository ?? throw new ArgumentNullException(nameof(packageRepository));
            _userRouteHandler = userRouteHandler ?? throw new ArgumentNullException(nameof(userRouteHandler));
        }

        public OperationResult CreatePackage(User? user, Package package)
        {
            if (!_userRouteHandler.IsValidUser(user))
            {
                return new OperationResult { Success = false, ErrorMessage = "User data is invalid." };
            }

            if (user == null || !CheckIfAdmin(user))
            {
                return new OperationResult { Success = false, ErrorMessage = "Not Authorized" };
            }

            _packageRepository.AddPackage(package);

            return new OperationResult { Success = true };
        }
        public bool CheckIfAdmin(User? user)
        {
            return user is { Authorization: "admin-mtcgToken" };
        }
    }
}
