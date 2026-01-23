namespace CRM_Backend.Services.Interfaces;

public interface INotificationService
{
    Task SendPasswordResetAsync(string toEmail, string resetLink);
}
