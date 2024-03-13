using Services.Contracts;
using StackExchange.Redis;
using System.Text.Json;

namespace Services
{
    public class RedisCacheManager(IConnectionMultiplexer connectionMultiplexer) : ICacheService
    {

        private readonly IDatabase _cache = connectionMultiplexer.GetDatabase();
        private readonly IServer _server = connectionMultiplexer.GetServer(connectionMultiplexer.GetEndPoints().First());



        public async Task<bool> SetCachedData(string cacheKey, object data, TimeSpan expirationTime)
        {

            string jsonData = JsonSerializer.Serialize(data);
            return await _cache.StringSetAsync(cacheKey, jsonData, expirationTime);

        }


        public async Task<T?> GetCachedData<T>(string cacheKey)
        {
            string jsonData = await _cache.StringGetAsync(cacheKey);
            if (!string.IsNullOrEmpty(jsonData))
            {
                return JsonSerializer.Deserialize<T>(jsonData);
            }

            return default;
        }



        public async Task<bool> KeyExists(string key) =>
            await _cache.KeyExistsAsync(key);



        public async Task<bool> RemoveCachedData(string cacheKey)
        {
            return await _cache.KeyDeleteAsync(cacheKey);
        }



        public async Task<TimeSpan?> GetRemainingTime(string key)
        {
            if (await _cache.KeyExistsAsync(key))
            {
                var existingExpiration = _cache.KeyTimeToLive(key);
                if (existingExpiration != null && existingExpiration != TimeSpan.MinValue)
                {
                    return existingExpiration.Value;
                }
            }
            return null;
        }
    }
}
