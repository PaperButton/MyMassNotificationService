using MyMassNotificationService.Domain.Entities;

namespace MyMassNotificationService.Domain.Aggregates
{
    public class OutboxAggregate
    {
        private readonly List<OutboxMessage> _messages;

        public OutboxAggregate()
        {
            _messages = new List<OutboxMessage>();
        }

        public void AddMessage(string topic, string key, string value)
        {
            var message = new OutboxMessage(topic, key, value);
            _messages.Add(message);
        }

        public IEnumerable<OutboxMessage> GetMessages() => _messages;

        public void MarkAsProcessed(Guid messageId)
        {
            var message = _messages.Find(m => m.Id == messageId);
            message?.MarkAsProcessed();
        }
    }
}
