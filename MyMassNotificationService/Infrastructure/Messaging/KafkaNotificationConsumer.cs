using Confluent.Kafka;
using Microsoft.Extensions.Options;
using MyMassNotificationService.Application.Interfaces;
using MyMassNotificationService.Application.Options;
using MyMassNotificationService.Application.Services;
using MyMassNotificationService.Domain.Entities;
using System.Text.Json;

namespace MyMassNotificationService.Infrastructure.Messaging
{
    public class KafkaNotificationConsumer : BackgroundService
    {
        private readonly ILogger<KafkaNotificationConsumer> _logger;
        private readonly ConsumerConfig _config;
        private readonly IOptions<KafkaOptions> _kafkaOptions;
        private readonly IEmailServiceFactory _emailServiceFactory;

        public KafkaNotificationConsumer(ILogger<KafkaNotificationConsumer> logger, IOptions<KafkaOptions> kafkaOptions, 
            IEmailServiceFactory emailServiceFactory)
        {
            _logger = logger;
            _kafkaOptions = kafkaOptions;
            _config = new ConsumerConfig
            {
                BootstrapServers = kafkaOptions.Value.BootstrapServers,
                GroupId = kafkaOptions.Value.GroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };
            _emailServiceFactory = emailServiceFactory;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var consumer = new ConsumerBuilder<string, string>(_config).Build();
            consumer.Subscribe(_kafkaOptions.Value.Topic);

            stoppingToken.Register(() => consumer.Close());

            return Task.Run(() =>
            {
                try
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        try
                        {
                            var consumeResult = consumer.Consume(stoppingToken);
                            var message = consumeResult.Message;

                            try
                            {
                                // Обрабатываем сообщение
                                ProcessMessage(message.Key, message.Value).GetAwaiter().GetResult();

                                // Фиксируем смещение вручную после успешной обработки
                                consumer.Commit(consumeResult);
                                _logger.LogInformation($"Сообщение обработано и смещение зафиксировано: {consumeResult.Offset}");
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError($"Ошибка при обработке сообщения: {ex.Message}");
                            }
                        }
                        catch (ConsumeException ex)
                        {
                            _logger.LogError($"Ошибка при потреблении сообщения: {ex.Error.Reason}");
                        }
                    }
                }
                catch (OperationCanceledException ex)
                {
                    _logger.LogError($"Ошибка при потреблении сообщения: {ex.Message}");
                }
                finally
                {
                    consumer.Close();
                }
            }, stoppingToken);
        }

        private async Task ProcessMessage(string key, string value)
        {
            var emailService = _emailServiceFactory.Create();

            var emailData = JsonSerializer.Deserialize<EmailData>(value); 

            if (emailData != null)
            {
                await emailService.SendEmailsAsync(emailData.RecipientEmails, emailData.Subject, emailData.Body);
            }
        }
    }

}
