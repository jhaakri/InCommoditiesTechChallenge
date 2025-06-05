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
        private IConfiguration _configuration;
        private ILogger<EmailNotificationSender> _logger;

        public EmailNotificationSender(ILogger<EmailNotificationSender> logger, ISmtpClient smtpClient, IConfiguration configuration)
        {
            _smtpClient = smtpClient ?? throw new ArgumentNullException(nameof(smtpClient), "SmtpClient cannot be null");
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration), "Configuration cannot be null");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null");
        }

        public async Task<bool> SendEmailAsync(MimeMessage message)
        {
            // Email cannot be sent if no recipients are specified
            if (message.To?.Any() != true)
            {
                _logger.LogWarning("No recipients specified for the email. Email will not be sent.");
                return false;
            }                       
                     
            var smtpConfig = _configuration.GetSection("SmtpSettings")?.Get<SmtpConfig>();

            if (smtpConfig == null)
            {
                _logger.LogError("SMTP configuration is not set in the application settings.");
                return false;
            }

            _smtpClient.Connect(smtpConfig.Server, smtpConfig.Port, smtpConfig.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);
            _smtpClient.Authenticate(smtpConfig.Email, smtpConfig.Password);
           
            var retryAttempts = 0;
            var maxAttempts = smtpConfig.EmailRetryCount > 0 ? smtpConfig.EmailRetryCount : 1;
            Exception? lastException = null;

            for (; retryAttempts < maxAttempts; retryAttempts++)
            {
                try
                {
                    if (!_smtpClient.IsConnected)
                    {
                        await _smtpClient.ConnectAsync(
                            smtpConfig.Server,
                            smtpConfig.Port,
                            smtpConfig.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None
                        );
                    }

                    if (!_smtpClient.IsAuthenticated)
                    {
                        await _smtpClient.AuthenticateAsync(smtpConfig.Email, smtpConfig.Password);
                    }

                    await _smtpClient.SendAsync(message);
                    await _smtpClient.DisconnectAsync(true);
                    return true;
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    _logger.LogWarning($"Attempt {retryAttempts + 1} to send email failed: {ex.Message}");

                    if (retryAttempts < maxAttempts - 1)
                    {
                        await Task.Delay(smtpConfig.EmailRetryTimeout);
                    }
                }
            }

            _logger.LogError($"Failed to send email after {retryAttempts} attempts: {lastException?.Message}");
            return false;          
        }
    }
}
