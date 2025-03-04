using MyMassNotificationService.Domain.Entities;

namespace MyMassNotificationService.Application.Interfaces
{
    public interface IOutboxService
    {
        Task<int> AddMessageAsync(string topic, string key, string value, CancellationToken cancellationToken);
    }
}
