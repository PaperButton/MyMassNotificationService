namespace MyMassNotificationService.Domain.Events
{
    public class MessageSentEvent
    {
        public Guid MessageId { get; }
        public string Topic { get; }
        public string Key { get; }
        public string Value { get; }
        public DateTime SentAt { get; }

        public MessageSentEvent(Guid messageId, string topic, string key, string value)
        {
            MessageId = messageId;
            Topic = topic;
            Key = key;
            Value = value;
            SentAt = DateTime.UtcNow;
        }
    }
}
