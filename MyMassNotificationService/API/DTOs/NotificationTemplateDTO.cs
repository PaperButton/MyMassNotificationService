namespace MyMassNotificationService.API.DTOs
{
    public class NotificationTemplateDTO
    {
        public string Name { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<string> RecipientEmails { get; set; }
        public string SenderEmail { get; set; }
    }
}
