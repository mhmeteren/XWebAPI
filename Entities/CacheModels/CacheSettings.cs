

namespace Entities.CacheModels
{
    public class CacheSettings
    {
        public static readonly CacheInfo UserRefreshToken = new("UserRefreshToken::Username:{0}", TimeSpan.FromMinutes(1));
    }
}
