﻿namespace MyMassNotificationService.Domain.Entities
{
    public class NotificationTemplate
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public string Subject { get; set; } 
        public string Body { get; set; }
        public List<string> RecipientEmails { get; set; }
    }

}
