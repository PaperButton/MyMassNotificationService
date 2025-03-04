namespace MyMassNotificationService.Domain.Entities
{
    public class OutboxMessage
    {
        public Guid Id { get; private set; }
        public string Topic { get; private set; }
        public string Key { get; private set; }
        public string Value { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public bool IsProcessed { get; private set; }

        public OutboxMessage(string topic, string key, string value)
        {
            Id = Guid.NewGuid();
            Topic = topic;
            Key = key;
            Value = value;
            CreatedAt = DateTime.UtcNow;
            IsProcessed = false;
        }

        public void MarkAsProcessed()
        {
            IsProcessed = true;
        }
    }
}
