using InCommoditiesTechChallenge.Models;

namespace InCommoditiesTechChallenge.Scrapers
{
    public interface IScraper<T> where T : class, IScraper<T>
    {
        public List<Notice> ScrapeData(string htmlContent);
    }
}
