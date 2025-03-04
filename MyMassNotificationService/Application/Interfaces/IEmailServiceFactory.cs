namespace MyMassNotificationService.Application.Interfaces
{
    public interface IEmailServiceFactory
    {
        IEmailService Create();
    }
}
