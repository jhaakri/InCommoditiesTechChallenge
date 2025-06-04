
namespace InCommoditiesTechChallenge.Models
{    
    public class AppSettingsModel
    {
        public required Dictionary<string,PipelineConfig> PipelineSettings { get; set; }
        public required SmtpConfig SmtpSettings { get; set; }
        public required NotificationEmailConfig NotificationEmailSettings { get; set; }        
    }
}
