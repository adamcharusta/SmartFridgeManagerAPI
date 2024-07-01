namespace SmartFridgeManagerAPI.Infrastructure.Common.Redis;

public interface IRedisService
{
    Task<bool> SaveAsync<T>(string key, T value, DateTime expiredDate) where T : class;
    Task<T?> ReadAsync<T>(string key) where T : class;
    Task<bool> DeleteAsync(string key);
}
