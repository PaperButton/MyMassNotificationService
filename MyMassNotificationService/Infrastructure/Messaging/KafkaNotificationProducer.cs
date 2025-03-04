using Confluent.Kafka;
using Microsoft.Extensions.Options;
using MyMassNotificationService.Application.Options;

namespace MyMassNotificationService.Infrastructure.Messaging
{
    public class KafkaNotificationProducer
    {
        private readonly ProducerConfig _config;

        public KafkaNotificationProducer(IOptions<KafkaOptions> options)
        {
            _config = new ProducerConfig { BootstrapServers = options.Value.BootstrapServers }; 
        }

        public async Task SendNotificationAsync(string topic, string key, string value)
        {
            using var producer = new ProducerBuilder<string, string>(_config).Build();
            var message = new Message<string, string> { Key = key, Value = value };
            await producer.ProduceAsync(topic, message);
        }
    }
}
