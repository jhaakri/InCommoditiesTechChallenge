using InCommoditiesTechChallenge.Models;
using InCommoditiesTechChallenge.Scrapers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace InCommoditiesTechChallenge
{
    public interface IDataFetcher
    {
        void Dispose();
        Task<List<Notice>> FetchDataAsync(string pipelineKey);
    }

    public class DataFetcher : IDataFetcher
    {
        private readonly ILogger<DataFetcher> _logger;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private IScraper<AnrScraper> _anrScraper;
        private IScraper<SabineScraper> _sabineScraper;        

        public DataFetcher(ILogger<DataFetcher> logger, IConfiguration configuration, IScraper<AnrScraper> anrScraper, IScraper<SabineScraper> sabineScraper)
        {
            _configuration= configuration;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null");
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
            _anrScraper = anrScraper ?? throw new ArgumentNullException(nameof(anrScraper), "anrScraper cannot be null");
            _sabineScraper = sabineScraper ?? throw new ArgumentNullException(nameof(sabineScraper), "sabineScraper cannot be null");
        }

        public async Task<List<Notice>> FetchDataAsync(string pipelineKey)
        {
            var appSettings = _configuration.Get<AppSettingsModel>();

            if (appSettings == null || appSettings.PipelineSettings == null)
            {
                _logger.LogError("Application settings are not configured correctly.");
                throw new InvalidOperationException("Application settings are not configured correctly.");
            }

            var allNoticeList = new List<Notice>();

            if (!appSettings.PipelineSettings.TryGetValue(pipelineKey, out var pipelineSettings) || pipelineSettings == null)
            {
                _logger.LogWarning($"Pipeline {pipelineKey ?? ""} is not configured correctly.");
                return allNoticeList;
            }

            if (pipelineSettings.EbbUrls == null || !pipelineSettings.EbbUrls.Any())
            {
                _logger.LogWarning($"Pipeline {pipelineKey ?? ""} is not configured correctly.");
                return allNoticeList;
            }

            foreach (var url in pipelineSettings.EbbUrls)
            {
                var urlValue = url.Value;
                if (!string.IsNullOrEmpty(urlValue))
                {
                    HttpResponseMessage httpResponse = await _httpClient.GetAsync(urlValue);
                    httpResponse.EnsureSuccessStatusCode();
                    string htmlContent = await httpResponse.Content.ReadAsStringAsync();

                    // Call the appropriate scraper based on the pipeline key
                    var notices = new List<Notice>();

                    if (pipelineKey.Equals("Anr", StringComparison.CurrentCultureIgnoreCase))
                    {
                        notices = _anrScraper.ScrapeData(htmlContent);
                    }
                    else if (pipelineKey.Equals("SabinePipeline", StringComparison.CurrentCultureIgnoreCase))
                    {
                        notices = _sabineScraper.ScrapeData(htmlContent);
                    }

                    if (notices.Any())
                    {
                        allNoticeList.AddRange(notices);
                    }
                }
            }

            return allNoticeList;
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
