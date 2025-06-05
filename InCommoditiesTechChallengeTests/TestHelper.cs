using InCommoditiesTechChallenge.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;

namespace InCommoditiesTechChallengeTests
{
    public static class TestHelper
    {
        public static AppSettingsModel GetEmptyAppSettingsModel()
        {
            return new AppSettingsModel
            {
                NotificationEmailSettings = new NotificationEmailConfig
                {
                    To = string.Empty
                },
                PipelineSettings = [],
                SmtpSettings = new SmtpConfig
                {
                    Server = string.Empty,
                    Port = 0,
                    Email = string.Empty,
                    Password = string.Empty,
                    EmailRetryTimeout = 0,
                    EmailRetryCount = 0,
                    EnableSsl = false
                }
            };
        }

        public static AppSettingsModel GetValidAppSettingsModel(
            string email = "valid@email.com",
            int emailRetryCount = 3,
            int emailRetryTimeout = 500,
            string password = "sTr0ngP@d",
            int port = 123,
            bool enableSsl = true,
            string server = "smtp.test.info",
            string to = "recipientTo@test.info",
            string cc = "recipientCc@test.info",
            string bcc = "recipientBcc@test.info",
            string subject = "Test Notification"
        )
        {
            var pipelineConfig = new PipelineConfig 
            { 
                Name="NewPipeline",
                EbbUrlNoticeRoot = "rootUrl",
                TradingSignalKeywords="test",
                TradingSignalNumOfDays=1,
                EbbUrls = []
            };

            var pipelineSettings = new Dictionary<string, PipelineConfig>();
            pipelineSettings.Add(pipelineConfig.Name, pipelineConfig);

            return new AppSettingsModel
            {
                 
                PipelineSettings = pipelineSettings,
                SmtpSettings = new SmtpConfig
                {
                    Email = email,
                    EmailRetryCount = emailRetryCount,
                    EmailRetryTimeout = emailRetryTimeout,
                    Password = password,
                    Port = port,
                    Server = server,
                    EnableSsl = enableSsl
                },
                NotificationEmailSettings = new NotificationEmailConfig
                {
                    To = to, // Fix for CS9035: Required member 'NotificationEmailConfig.To' must be set  
                    Cc = cc,
                    Bcc = bcc,
                    Subject = subject
                }
            };
        }

        public static ConfigurationSection GetConfigurationSection()
        {
            var memoryConfigurationProvider = new MemoryConfigurationProvider(new MemoryConfigurationSource());
            var configurationRoot = new ConfigurationRoot(new[] { memoryConfigurationProvider });
            return new ConfigurationSection(configurationRoot, "");
        }
    }
}
