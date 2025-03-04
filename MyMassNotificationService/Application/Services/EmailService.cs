using Microsoft.Extensions.Options;
using MyMassNotificationService.Application.Options;
using Polly;
using Polly.Retry;
using MailKit.Net.Smtp;
using MimeKit;
using MyMassNotificationService.Application.Interfaces;

namespace MyMassNotificationService.Application.Services
{
    public class EmailService: IEmailService
    {
        private readonly EmailOptions _emailOptions;
        private readonly ILogger<EmailService> _logger;
        private readonly AsyncRetryPolicy _retryPolicy;

        public EmailService(IOptions<EmailOptions> emailOptions, ILogger<EmailService> logger)
        {
            _emailOptions = emailOptions.Value;
            _logger = logger;

            _retryPolicy = Policy
                .Handle<Exception>() 
                .WaitAndRetryAsync(
                    retryCount: 3, 
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(1), 
                    onRetry: (exception, timeSpan, retryAttempt, context) =>
                    {
                        _logger.LogWarning($"Попытка {retryAttempt} отправки email не удалась из-за {exception.Message}. Ожидание {timeSpan} перед следующей попыткой.");
                    });
        }

        public async Task SendEmailAsync(string recipientEmail, string subject, string body)
        {
            try
            {
                await _retryPolicy.ExecuteAsync(async () =>
                {
                    var emailMessage = new MimeMessage();
                    emailMessage.From.Add(new MailboxAddress("Notification System", _emailOptions.From));
                    emailMessage.To.Add(new MailboxAddress("", recipientEmail));
                    emailMessage.Subject = subject;
                    emailMessage.Body = new TextPart("html") { Text = body };

                    using (var client = new SmtpClient())
                    {
                        await client.ConnectAsync(_emailOptions.SmtpServer, _emailOptions.Port, true);
                        await client.AuthenticateAsync(_emailOptions.Username, _emailOptions.Password);
                        await client.SendAsync(emailMessage);
                        await client.DisconnectAsync(true);
                    }
                });

                _logger.LogInformation($"Email успешно отправлен на {recipientEmail}.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось отправить email на {recipientEmail} после нескольких попыток: {ex.Message}");
                throw; // Важно пробросить исключение, чтобы уведомить вызывающий код о неудаче
            }
        }

        public async Task SendEmailsAsync(IEnumerable<string> recipientEmails, string subject, string body)
        {
            foreach (var email in recipientEmails)
            {
                await SendEmailAsync(email, subject, body);
            }
        }
    }
}
