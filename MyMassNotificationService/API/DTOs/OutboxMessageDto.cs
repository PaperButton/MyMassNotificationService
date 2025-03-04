namespace MyMassNotificationService.API.DTOs
{
    public class OutboxMessageDto
    {
        public Guid Id { get; set; }
        public string Topic { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsProcessed { get; set; }
    }
}
