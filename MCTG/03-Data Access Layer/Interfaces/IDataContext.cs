using System.Data;

namespace MCTG._03_Data_Access_Layer.Interfaces;

public interface IDataContext : IDisposable
{
    void Add<T>(T entity) where T : class;
    void Remove<T>(int id) where T : class;
    void Update<T>(T entity) where T : class;
    T GetById<T>(int id) where T : class;
    T GetByUsername<T>(string username) where T : class;
    T GetByAuthToken<T>(string authToken) where T : class;
    IEnumerable<T> GetAll<T>() where T : class;

}