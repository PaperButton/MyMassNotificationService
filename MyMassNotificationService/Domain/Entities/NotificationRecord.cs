using System.Net;

namespace MyMassNotificationService.Domain.Entities
{
    public class NotificationRecord
    {
        public int Id { get; set; }
        public int UserId { get; set; } // Ссылка на пользователя, которому отправлено уведомление
        public User User { get; set; } 
        public int NotificationTemplateId { get; set; } 
        public NotificationTemplate NotificationTemplate { get; set; } 
        public DateTime SentAt { get; set; } 
        public HttpStatusCode Status { get; set; } // Статус отправки (успешно, ошибка и т.д.)
    }

}
