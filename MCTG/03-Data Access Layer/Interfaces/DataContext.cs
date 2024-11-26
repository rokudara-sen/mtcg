namespace MCTG._03_Data_Access_Layer.Interfaces;

public interface DataContext
{
    void Add<T>(T entity) where T : class;
    void Remove<T>(T entity) where T : class;
    void Update<T>(T entity) where T : class;
    void Save();
}