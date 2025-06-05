
namespace InCommoditiesTechChallenge.Models
{
    public class NotificationEmailConfig
    {
        public string From { get; set; }
        public required string To { get; set; }
        public string? Cc { get; set; }
        public string? Bcc { get; set; }
        public string? Subject { get; set; }
    }
}
