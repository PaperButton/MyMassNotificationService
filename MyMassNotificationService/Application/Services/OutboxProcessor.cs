using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyMassNotificationService.Application.Interfaces;
using MyMassNotificationService.Application.Options;
using MyMassNotificationService.Domain.Entities;
using MyMassNotificationService.Infrastructure.Data;
using MyMassNotificationService.Infrastructure.Messaging;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace MyMassNotificationService.Application.Services
{
    public class OutboxProcessor : BackgroundService
    {
        private readonly ILogger<OutboxProcessor> _logger;
        private readonly KafkaNotificationProducer _kafkaProducer;
        private readonly IOptions<WebAPIOutboxOptions> _webAPIOutboxoptionsoptions;
        private readonly IHttpClientFactory _httpClientFactory;


        public OutboxProcessor(ILogger<OutboxProcessor> logger, KafkaNotificationProducer kafkaNotificationProducer, 
            IHttpClientFactory httpClientFactory,IOptions<WebAPIOutboxOptions> webAPIOutboxoptions)
        {
            _logger = logger;
            _kafkaProducer = kafkaNotificationProducer;
            _webAPIOutboxoptionsoptions = webAPIOutboxoptions;
            _httpClientFactory = httpClientFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var httpClient = _httpClientFactory.CreateClient();
                    var messages = await GetUnprocessedOutboxMessages(httpClient);

                    if (messages.Count == 0)
                    {
                        // Если нет сообщений для обработки, ждем перед следующей проверкой
                        await Task.Delay(500, stoppingToken); // Ждем 0.5 секунд перед следующей итерацией
                        continue;
                    }

                    foreach (var message in messages)
                    {
                        try
                        {
                            await SendToKafka(message);

                            await MarkAsProcessed(httpClient, message.Id);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Ошибка при обработке сообщения {message.Id}: {ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при обработке сообщений из Outbox: {ex.Message}");
                }
            }
        }

        private async Task SendToKafka(OutboxMessage message)
        {
            await _kafkaProducer.SendNotificationAsync(message.Topic, message.Key, message.Value);
        }

        private async Task<List<OutboxMessage>> GetUnprocessedOutboxMessages(HttpClient httpClient)
        {
            HttpResponseMessage response = await httpClient.GetAsync($"{_webAPIOutboxoptionsoptions.Value.BaseURL}api/Outbox/Unprocessed");

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch
            {
                _logger.LogError($"Error when getting unprocessed kafka messages outboxProcessor. UTC:{DateTime.UtcNow}");
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<OutboxMessage>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        private async Task MarkAsProcessed(HttpClient httpClient, Guid messageId)
        {
            var response = await httpClient.PostAsync($"{_webAPIOutboxoptionsoptions.Value.BaseURL}api/Outbox/MarkAsProcessed/{messageId}", null);
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch
            {
                _logger.LogError($"Error when marking of delete UTC:{DateTime.UtcNow}");
            }
        }
    }
}
