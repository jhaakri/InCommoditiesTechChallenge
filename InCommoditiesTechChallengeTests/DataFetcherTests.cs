using InCommoditiesTechChallenge;
using InCommoditiesTechChallenge.Models;
using InCommoditiesTechChallenge.NotificationSenders;
using InCommoditiesTechChallenge.Scrapers;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;

namespace InCommoditiesTechChallengeTests
{
    [TestClass ]
    public class DataFetcherTests
    {
        [TestMethod]
        public void FetchDataAsyncNoPipelineConfigReturnsFault()
        {
            //If the Pipeline config is not defined then the method should not complete successfully

            // Arrange  
            var loggerMock = new Mock<ILogger<EmailNotificationSender>>();
            var smtpClientMock = new Mock<ISmtpClient>();

            var appsettingsModel = TestHelper.GetEmptyAppSettingsModel();
            var configBuilder = new ConfigurationBuilder();
            using var appSettingsStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(appsettingsModel)));
            var config = configBuilder.AddJsonStream(appSettingsStream).Build();

            var dataFetcher = new DataFetcher(new Mock<ILogger<DataFetcher>>().Object, config,
                new Mock<IScraper<AnrScraper>>().Object, new Mock<IScraper<SabineScraper>>().Object);

            // Act  
            var result = dataFetcher.FetchDataAsync("");

            // Assert  
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Status == TaskStatus.Faulted);
        }

        [TestMethod]
        public void FetchDataAsyncNoEbbUrlsReturnsEmptyList()
        {
            //If there are no EbbUrls defined then the method should return an empty list of notices

            // Arrange  
            var loggerMock = new Mock<ILogger<EmailNotificationSender>>();
            var smtpClientMock = new Mock<ISmtpClient>();

            AppSettingsModel appSettings = TestHelper.GetValidAppSettingsModel();

            var configBuilder = new ConfigurationBuilder();
            using var appSettingsStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(appSettings)));
            var config = configBuilder.AddJsonStream(appSettingsStream).Build();

            var dataFetcher = new DataFetcher(new Mock<ILogger<DataFetcher>>().Object, config,
                new Mock<IScraper<AnrScraper>>().Object, new Mock<IScraper<SabineScraper>>().Object);

            // Act  
            var result = dataFetcher.FetchDataAsync("");

            // Assert  
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Status == TaskStatus.RanToCompletion);
            Assert.IsFalse(result.Result.Any());
        }
    }
}
