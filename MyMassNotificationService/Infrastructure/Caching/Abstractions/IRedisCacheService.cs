namespace MyMassNotificationService.Infrastructure.Caching.Abstractions
{
    public interface IRedisCacheService
    {
        Task SetStringAsync(string key, string value);
        Task<string> GetStringAsync(string key);
    }
}
