using InCommoditiesTechChallenge.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace InCommoditiesTechChallenge.NotificationSenders
{
    public interface IEmailNotificationSender
    {
        Task<bool> SendEmailAsync(MimeMessage message);
    }

    public class EmailNotificationSender: IEmailNotificationSender
    {
        private ISmtpClient _smtpClient;   
        private SmtpConfig _smptconfig;
        private ILogger<EmailNotificationSender> _logger;

        public EmailNotificationSender(ILogger<EmailNotificationSender> logger, ISmtpClient smtpClient, IConfiguration configuration)
        {
            _smtpClient = smtpClient ?? throw new ArgumentNullException(nameof(smtpClient), "SmtpClient cannot be null");
            _smptconfig = configuration?.Get<AppSettingsModel>()?.SmtpSettings ?? throw new ArgumentNullException(nameof(smtpClient), "SmtpSettings could not be found");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null");
        }

        public async Task<bool> SendEmailAsync(MimeMessage message)
        {
            // Email cannot be sent if no recipients are specified
            if (message.To?.Any() != true)
            {
                return false;
            }                       
                           
            _smtpClient.Connect(_smptconfig.Server, _smptconfig.Port, _smptconfig.enableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);
            _smtpClient.Authenticate(_smptconfig.Email, _smptconfig.Password);

            var retryAttempts = 0;
            try
            {
                await _smtpClient.SendAsync(message);
                _smtpClient.Disconnect(true);
            }
            catch(Exception ex)
            {
                //If email fails to send, retry it based on retry count and timeout specified in the appsettings
                retryAttempts++;
                if(retryAttempts>= _smptconfig.EmailRetryCount)
                {
                    _logger.LogError($"Failed to send email after {retryAttempts} attempts: {ex.Message}");
                    throw;
                }
                await Task.Delay(_smptconfig.EmailRetryTimeout);
            }

            return true;            
        }
    }
}
