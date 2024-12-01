using System.Data;

namespace MTCG._03_Data_Access_Layer.Interfaces;

public interface IDataContext : IDisposable
{
    public IDbConnection CreateConnection();

    public void Dispose();

    public void Add<T>(T entity) where T : class;

    public void Remove<T>(int tableId) where T : class;

    public void Update<T>(T entity) where T : class;

    public T? GetById<T>(int tableId) where T : class;
    
    public T? GetByStringId<T>(string id) where T : class;
}