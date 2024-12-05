using MTCG._06_Domain.Entities;

namespace MTCG._03_Data_Access_Layer.Interfaces;

public interface IPackageRepository
{
    void AddPackage(Package package);
    Package GetPackageById(string packageId);
}