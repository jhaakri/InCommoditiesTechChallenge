using InCommoditiesTechChallenge.Models;
using InCommoditiesTechChallenge.NotificationSenders;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using Moq;
using Newtonsoft.Json;

namespace InCommoditiesTechChallengeTests.NotificationSenders
{
    [TestClass]
    public sealed class EmailNotificationSenderTests
    {
        [TestMethod]
        public void SendEmailAsyncNoRecipientNoEmailSent()
        {
            //Email shouold not be sent if there are no recepients specified in the To section of the message

            // Arrange  
            var loggerMock = new Mock<ILogger<EmailNotificationSender>>();
            var smtpClientMock = new Mock<ISmtpClient>();
            var configurationMock = new Mock<IConfiguration>();
            
            var emailNotificationSender = new EmailNotificationSender(loggerMock.Object, smtpClientMock.Object, configurationMock.Object);

            var message = new MimeMessage(); // No recipients added to the message.To

            // Act  
            var result = emailNotificationSender.SendEmailAsync(message).Result;

            // Assert  
            Assert.IsNotNull(result);
            Assert.IsFalse(result);
        }

        [TestMethod]        
        public void SendEmailAsyncWithRecipientEmailSent()
        {
            // Email should be sent if there are recipients specified in the To section of the message

            // Arrange  
            AppSettingsModel appSettings = TestHelper.GetValidAppSettingsModel();

            var configBuilder = new ConfigurationBuilder();
            using var appSettingsStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(appSettings)));
            var config = configBuilder.AddJsonStream(appSettingsStream).Build();

            var loggerMock = new Mock<ILogger<EmailNotificationSender>>();
            var smtpClientMock = new Mock<ISmtpClient>();
            
            var emailNotificationSender = new EmailNotificationSender(loggerMock.Object, smtpClientMock.Object, config);

            var message = new MimeMessage();
            message.To.Add(MailboxAddress.Parse("test@email.info")); // recipient added to the message

            // Act  
            var result = emailNotificationSender.SendEmailAsync(message).Result;

            // Assert  
            Assert.IsNotNull(result);
            Assert.IsTrue(result);
        }
    }
}
