using CRM_Backend.Security.Email;
using CRM_Backend.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace CRM_Backend.Services.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly EmailSettings _email;
        private readonly SmtpSettings _smtp;

        public NotificationService(
            IOptions<EmailSettings> emailOptions,
            IOptions<SmtpSettings> smtpOptions)
        {
            _email = emailOptions.Value;
            _smtp = smtpOptions.Value;
        }

        public async Task SendPasswordResetAsync(string userEmail, string resetLink)
        {
            var to = string.IsNullOrWhiteSpace(_email.OverrideRecipient)
                ? userEmail
                : _email.OverrideRecipient;

            var message = new MailMessage
            {
                From = new MailAddress(_smtp.From),
                Subject = "Reset your CRM password",
                Body = $"Click the link to reset your password:\n\n{resetLink}",
                IsBodyHtml = false
            };

            message.To.Add(to);

            using var client = new SmtpClient(_smtp.Host, _smtp.Port)
            {
                Credentials = new NetworkCredential(
                    _smtp.Username,
                    _smtp.Password
                ),
                EnableSsl = true
            };

            await client.SendMailAsync(message);
        }
    }
}
