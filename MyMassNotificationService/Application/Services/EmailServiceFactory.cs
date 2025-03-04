using MyMassNotificationService.Application.Interfaces;

namespace MyMassNotificationService.Application.Services
{
    public class EmailServiceFactory : IEmailServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public EmailServiceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IEmailService Create()
        {
            return _serviceProvider.GetRequiredService<IEmailService>();
        }
    }
}
