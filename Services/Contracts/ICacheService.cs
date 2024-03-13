
namespace Services.Contracts
{
    public interface ICacheService
    {
        Task<bool> SetCachedData(string cacheKey, object data, TimeSpan expirationTime);


        Task<T?> GetCachedData<T>(string cacheKey);
        Task<TimeSpan?> GetRemainingTime(string key);


        Task<bool> RemoveCachedData(string cacheKey);
        Task<bool> KeyExists(string key);
    }
}
