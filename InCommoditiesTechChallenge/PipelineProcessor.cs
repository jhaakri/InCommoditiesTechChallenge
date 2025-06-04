using InCommoditiesTechChallenge.Models;
using InCommoditiesTechChallenge.NotificationSenders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace InCommoditiesTechChallenge
{
    public interface IPipelineProcessor
    {
        void ProcessPipeline(string pipelineKey);
    }

    public class PipelineProcessor:IPipelineProcessor
    {
        private IEmailNotificationSender _emailNotificationSender;
        private IMimeMessageBuilder _mimeMessageBuilder;
        private IDataFetcher _dataFetcher;
        private AppSettingsModel _appSettings;
        private ILogger<PipelineProcessor> _logger;

        public PipelineProcessor(ILogger<PipelineProcessor> logger, IConfiguration configuration, IDataFetcher dataFetcher ,IEmailNotificationSender emailNotificationSender, IMimeMessageBuilder mimeMessageBuilder) 
        {
            _appSettings = configuration.Get<AppSettingsModel>() ?? throw new ArgumentNullException(nameof(configuration), "configuration cannot be null");
            _emailNotificationSender = emailNotificationSender ?? throw new ArgumentNullException(nameof(emailNotificationSender), "EmailNotificationSender cannot be null");
            _dataFetcher = dataFetcher ?? throw new ArgumentNullException(nameof(dataFetcher), "DataFetcher cannot be null");
            _mimeMessageBuilder = mimeMessageBuilder ?? throw new ArgumentNullException(nameof(mimeMessageBuilder), "MimeMessageBuilder cannot be null");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null");
        }

        //public async void ProcessPipeline(KeyValuePair<string, PipelineConfig> pipelineSettings)
        public async void ProcessPipeline(string pipelineKey)
        {
            try
            {
                var pipelineSettings = _appSettings.PipelineSettings[pipelineKey];
                
                //Ingest
                _logger.LogInformation($"Fetch data for {pipelineKey} pipeline started.");
                var allNotices = _dataFetcher.FetchDataAsync(pipelineKey)?.Result;

                if (allNotices == null || !allNotices.Any())
                {
                    _logger.LogInformation($"No data found for {pipelineKey} pipeline.");
                    return;
                }

                _logger.LogInformation($"{allNotices.Count} notices found for {pipelineKey} pipeline.");

                
                //Notice Type description contains at least one of the defined keywords
                var keywords = pipelineSettings.TradingSignalKeywords?.Split(",")?.ToList() ?? [];
                var numOfDays = pipelineSettings.TradingSignalNumOfDays; //Posted Date is within the specified number of days

                allNotices = allNotices?.Where(n => n.PostedDateTime >= DateTime.Now.AddDays(-numOfDays))?.ToList();
                allNotices = allNotices?.Where(n => (n.NoticeTypeDescription != null && keywords.Any(n.NoticeTypeDescription.Contains))
                || (n.Subject != null && keywords.Any(n.Subject.Contains)))?.ToList();
                
                if (allNotices == null || !allNotices.Any())
                {
                    _logger.LogInformation($"No notices found for {pipelineKey} pipeline.");
                    return;
                }

                _logger.LogInformation($"{allNotices.Count} trading signal(s) detected for {pipelineKey} pipeline.");

                //Send email notification
                var notificationEmailSettings = _appSettings?.NotificationEmailSettings 
                    ?? throw new ArgumentNullException("EmailNotification settings not configured correctly in PipelineSettings section of appsettings.json");

                _mimeMessageBuilder.BuildFrom(notificationEmailSettings.From);
                _mimeMessageBuilder.BuildTo(notificationEmailSettings.To?.Split(",")?.ToList());
                _mimeMessageBuilder.BuildSubject(notificationEmailSettings.Subject);
                _mimeMessageBuilder.BuildBody(pipelineSettings.Name?? pipelineKey, allNotices);
                _mimeMessageBuilder.BuildCc(notificationEmailSettings.Cc?.Split(",")?.ToList());
                _mimeMessageBuilder.BuildBcc(notificationEmailSettings.Bcc?.Split(",")?.ToList());

                var emailMessage = _mimeMessageBuilder.GetMessage();

                _logger.LogInformation($"Send notifications for {pipelineKey} pipeline started.");
                bool success = await _emailNotificationSender.SendEmailAsync(emailMessage);

                if (success)
                {
                    _logger.LogInformation($"Send notifications for {pipelineKey} pipeline completed successfully.");
                }
                else
                {
                    _logger.LogWarning($"Send notifications for {pipelineKey} pipeline failed.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception: " + ex.Message);
            }

            _logger.LogInformation($"Processing {pipelineKey} pipeline completed successfully");
        }
    }
}
