public interface IDataBase
{
    Task SaveAsync<T>(string key, T data);
    Task<T> LoadAsync<T>(string key);
    Task DeleteAsync(string key);
}