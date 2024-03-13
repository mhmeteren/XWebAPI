
namespace Entities.CacheModels
{
    public class CacheInfo (string Key, TimeSpan ExpirationTime)
    {
        public string Key = Key;
        public TimeSpan ExpirationTime = ExpirationTime;
    }
}
