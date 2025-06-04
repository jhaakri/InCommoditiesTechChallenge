
namespace InCommoditiesTechChallenge.Models
{
    public class PipelineConfig
    {
        public required string Name { get; set; }
        public required string EbbUrlNoticeRoot { get; set; }
        public required Dictionary<string, string> EbbUrls { get; set; }
    }
}
