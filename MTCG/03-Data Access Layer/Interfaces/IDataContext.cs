using System.Data;

namespace MTCG._03_Data_Access_Layer.Interfaces;

public interface IDataContext : IDisposable
{
    public IDbConnection CreateConnection();

}