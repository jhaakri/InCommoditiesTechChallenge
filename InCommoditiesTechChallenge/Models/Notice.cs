
namespace InCommoditiesTechChallenge.Models
{  
    public class Notice
    {
        public string? NoticeUrl { get; set; }
        public string? NoticeTypeDescription { get; set; }
        public DateTime PostedDateTime { get; set; }
        public DateTime EffectiveStart { get; set; }
        public DateTime EffectiveEnd { get; set; }
        public int NoticeId { get; set; }
        public string? Subject { get; set; }
        public int Volume { get; set; }
    }
}
