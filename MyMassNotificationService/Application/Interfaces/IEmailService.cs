namespace MyMassNotificationService.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string recipientEmail, string subject, string body);
        Task SendEmailsAsync(IEnumerable<string> recipientEmails, string subject, string body);
    }
}
