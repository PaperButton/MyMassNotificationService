namespace MyMassNotificationService.Infrastructure.Caching
{
    public class RedisOptions
    {
        public List<string> Endpoints { get; set; }
        public bool AbortOnConnectFail { get; set; }
        public int SyncTimeout { get; set; }

    }
}
