using MyMassNotificationService.Infrastructure.Caching.Abstractions;
using StackExchange.Redis;

namespace MyMassNotificationService.Infrastructure.Caching
{
    public class RedisCacheService: IRedisCacheService
    {
        private readonly ConnectionMultiplexer _redis;

        public RedisCacheService(string configuration)
        {
            _redis = ConnectionMultiplexer.Connect(configuration);
        }

        public async Task SetStringAsync(string key, string value)
        {
            var database = _redis.GetDatabase();
            await database.StringSetAsync(key, value);
        }

        public async Task<string> GetStringAsync(string key)
        {
            var database = _redis.GetDatabase();
            return await database.StringGetAsync(key);
        }

    }
}
