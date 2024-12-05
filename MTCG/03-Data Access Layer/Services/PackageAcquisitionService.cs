using MTCG._03_Data_Access_Layer.Interfaces;
using MTCG._06_Domain.Entities;

namespace MTCG._03_Data_Access_Layer.Services;

public class PackageAcquisitionService
{
    private readonly IPackageRepository _packageRepository;
    private readonly IUserRepository _userRepository;
    private readonly IStackRepository _stackRepository;
    private readonly IDataContext _dataContext;

    public PackageAcquisitionService(
        IPackageRepository packageRepository,
        IUserRepository userRepository,
        IStackRepository stackRepository,
        IDataContext dataContext)
    {
        _packageRepository = packageRepository ?? throw new ArgumentNullException(nameof(packageRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _stackRepository = stackRepository ?? throw new ArgumentNullException(nameof(stackRepository));
        _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
    }

    public OperationResult AcquirePackage(User? user)
    {
        if (user == null)
            return new OperationResult { Success = false, ErrorMessage = "Invalid user." };

        int packageCost = 5;

        if (user.Gold < packageCost)
        {
            return new OperationResult { Success = false, ErrorMessage = "Not enough money." };
        }

        using (var connection = _dataContext.CreateConnection())
        {
            connection.Open();
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var package = _packageRepository.GetNextAvailablePackage(connection, transaction);

                    if (package == null)
                    {
                        transaction.Rollback();
                        return new OperationResult { Success = false, ErrorMessage = "No packages available." };
                    }
                    
                    user.Gold -= packageCost;
                    _userRepository.UpdateUserWithConnection(user, connection, transaction);
                    
                    _packageRepository.RemovePackage(package.PackageId, connection, transaction);
                    
                    _stackRepository.AddCardsToUserStack(user.UserId, package.Cards, connection, transaction);

                    transaction.Commit();
                    return new OperationResult { Success = true };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new OperationResult { Success = false, ErrorMessage = ex.Message };
                }
            }
        }
    }
}