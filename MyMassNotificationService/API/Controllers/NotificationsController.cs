using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyMassNotificationService.API.DTOs;
using MyMassNotificationService.Application.Interfaces;
using MyMassNotificationService.Application.Options;
using MyMassNotificationService.Domain.Entities;
using MyMassNotificationService.Infrastructure.Data;
using System.Formats.Asn1;
using System.Globalization;
using System.Text.Json;

namespace MyMassNotificationService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly IOutboxService _outboxService;
        private readonly AppDbContext _dbContext;
        private readonly IOptions<KafkaOptions> _kafkaoptions;

        public NotificationsController(IOutboxService outboxService, AppDbContext dbContext, IOptions<KafkaOptions> kafkaoptions)
        {
            _outboxService = outboxService;
            _dbContext = dbContext;
            _kafkaoptions = kafkaoptions;
        }

        [HttpPost("send-notification")]
        public async Task<IActionResult> SendNotification([FromBody] NotificationTemplateDTO template, CancellationToken stoppingToken)
        {
            List<string> recipientEmails = new List<string>(template.RecipientEmails);

            EmailData emailData = new(
                recipientEmails, 
                template.Subject,
                template.Body,
                template.SenderEmail
                );

            string serializedData = JsonSerializer.Serialize<EmailData>(emailData);

            await _outboxService.AddMessageAsync(_kafkaoptions.Value.Topic, string.Empty, serializedData, stoppingToken);
            return Ok("Notifications queued for sending.");
        }
    }

}
