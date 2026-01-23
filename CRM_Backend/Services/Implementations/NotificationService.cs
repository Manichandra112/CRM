namespace CRM_Backend.Services.Implementations
{
    using CRM_Backend.Security.Email;
    using CRM_Backend.Services.Interfaces;
    using Microsoft.Extensions.Options;

    public class NotificationService : INotificationService
    {
        private readonly EmailSettings _settings;

        public NotificationService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public Task SendPasswordResetAsync(string userEmail, string resetLink)
        {
            var targetEmail = string.IsNullOrWhiteSpace(_settings.OverrideRecipient)
                ? userEmail
                : _settings.OverrideRecipient;

            // DEV behavior
            Console.WriteLine(
                $"[DEV EMAIL] To: {targetEmail}\nReset link: {resetLink}"
            );

            return Task.CompletedTask;
        }
    }

}
