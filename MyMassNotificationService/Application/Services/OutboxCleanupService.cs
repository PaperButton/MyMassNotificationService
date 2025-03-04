using Microsoft.Extensions.Options;
using MyMassNotificationService.Application.Options;

namespace MyMassNotificationService.Application.Services
{
    public class OutboxCleanupService : BackgroundService
    {
        private readonly ILogger<OutboxCleanupService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOptions<WebAPIOutboxOptions> _webAPIOutboxoptionsoptions;

        public OutboxCleanupService(ILogger<OutboxCleanupService> logger, IHttpClientFactory httpClientFactory,
            IOptions<WebAPIOutboxOptions> webAPIOutboxoptions)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _webAPIOutboxoptionsoptions = webAPIOutboxoptions;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var httpClient = _httpClientFactory.CreateClient();
                    var result = await DeleteUnprocessedOutboxMessages(httpClient);

                    if (!result)
                        throw new Exception($"error when deleting outbox messages. UtcNow:{ DateTime.UtcNow.ToString() }");
                    
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при очистке Outbox: {ex.Message}");
                }

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken); 
            }
        }

        private async Task<bool> DeleteUnprocessedOutboxMessages(HttpClient httpClient)
        {
            HttpResponseMessage response = await httpClient.DeleteAsync($"{_webAPIOutboxoptionsoptions.Value.BaseURL}api/Outbox/Messages");

            try
            {
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch
            {
                _logger.LogError($"Error when deleting kafka message outboxClenUpService. UTC:{DateTime.UtcNow}");
                return false;
            }
        }
    }
}
