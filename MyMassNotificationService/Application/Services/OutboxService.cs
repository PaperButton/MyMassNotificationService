using Microsoft.Extensions.Options;
using MyMassNotificationService.Application.Interfaces;
using MyMassNotificationService.Application.Options;
using MyMassNotificationService.Domain.Entities;

namespace MyMassNotificationService.Application.Services
{
    public class OutboxService : IOutboxService
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<WebAPIOutboxOptions> _webAPIOutboxoptionsoptions;
        private readonly ILogger<OutboxService> _logger;

        public OutboxService(HttpClient httpClient, IOptions<WebAPIOutboxOptions> webAPIOutboxoptions, ILogger<OutboxService> logger)
        {
            _httpClient = httpClient;
            _webAPIOutboxoptionsoptions = webAPIOutboxoptions;
            _logger = logger;
        }

        public async Task<int> AddMessageAsync(string topic, string key, string value, CancellationToken cancellationToken)
        {
            var message = new OutboxMessage(topic, key, value);
            var response = await _httpClient.PutAsJsonAsync<OutboxMessage>($"{_webAPIOutboxoptionsoptions.Value.BaseURL}api/Outbox/Messages", message, cancellationToken);
            
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch
            {
                _logger.LogError($"Error when adding a new kafka message in outbox service UTC:{DateTime.UtcNow}");
            }

            return 1;//1 означает успех операции
        }
    }
}
