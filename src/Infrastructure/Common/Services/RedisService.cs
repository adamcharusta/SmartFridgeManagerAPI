using SmartFridgeManagerAPI.Infrastructure.Common.Settings;
using StackExchange.Redis;

namespace SmartFridgeManagerAPI.Infrastructure.Common.Services;

public class RedisService : IRedisService
{
    private readonly IDatabase _database;
    private readonly ConnectionMultiplexer _redis;

    public RedisService(RedisSettings settings)
    {
        _redis = ConnectionMultiplexer.Connect($"{settings.HostName}:{settings.Port}");
        _database = _redis.GetDatabase();
    }

    public async Task<T?> ReadAsync<T>(string key) where T : class
    {
        try
        {
            RedisValue data = await _database.StringGetAsync(key);
            Log.Logger.Information("Get data from redis database.");
            return JsonConvert.DeserializeObject<T>(data.ToString());
        }
        catch (Exception e)
        {
            Log.Logger.Error(e, "An error occurred while reading or creating Redis entry.");
            throw;
        }
    }

    public async Task<bool> DeleteAsync(string key)
    {
        try
        {
            Log.Logger.Information("Remove data from redis database.");
            return await _database.KeyDeleteAsync(key);
        }
        catch (Exception e)
        {
            Log.Logger.Error(e, "An error occurred while deleting Redis entry.");
            throw;
        }
    }

    public async Task<bool> SaveAsync<T>(string key, T value, DateTime expiredDate) where T : class
    {
        try
        {
            TimeSpan expiry = expiredDate - DateTime.Now;

            Guard.Against.NegativeOrZero(expiry.TotalSeconds, "expiredDate must be greater than DateTime.Now");
            string data = JsonConvert.SerializeObject(value);
            Log.Logger.Information("Save data to redis database.");
            return await _database.StringSetAsync(key, data, expiry);
        }
        catch (Exception e)
        {
            Log.Logger.Error(e, "An error occurred while creating or updating Redis entry.");
            throw;
        }
    }

    ~RedisService()
    {
        _redis.Close();
        _redis.Dispose();
    }
}
