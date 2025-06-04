using HtmlAgilityPack;
using InCommoditiesTechChallenge.Models;

namespace InCommoditiesTechChallenge.Scrapers
{
    public class SabineScraper: IScraper<SabineScraper>
    {
        public List<Notice> ScrapeData(string htmlContent)
        {
            var noNoticesRowQuery = "/html/body/div[1]/table/tr[2]/td[1]";//This location contains text to determine if there are any notices
            var noNoticesText = "there are no effective notices to display";//Text displayed when there are no notices
            var noticeRowsQuery = "/html/body/div[2]/table/tr"; //This location contains the actual notices
            var noticeTypeDescriptionQuery = ".//td[1]";
            var noticeUrlQuery = ".//td[1]/a";
            var subjectQuery = ".//td[6]";
            var postedDateTimeQuery = ".//td[2]";
            var effectiveStartQuery = ".//td[3]";
            var effectiveEndQuery = ".//td[4]";
            var noticeIdQuery = ".//td[5]";

            var data = new List<Notice>();

            var doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);

            var noDataRow = doc.DocumentNode.SelectSingleNode(noNoticesRowQuery);

            if (noDataRow == null
                || !noDataRow.InnerText.Contains(noNoticesText, StringComparison.CurrentCultureIgnoreCase))
            {
                var noticeRows = doc.DocumentNode.SelectNodes(noticeRowsQuery);
                if (noticeRows != null)
                {
                    foreach (var row in noticeRows)
                    {
                        DateTime postedDateTime;
                        DateTime effectiveStart;
                        DateTime effectiveEnd;
                        int noticeId;
                        var noticeTypeDescription = CleanStringData(row.SelectSingleNode(noticeTypeDescriptionQuery)?.InnerText);
                        var noticeUrl = CleanStringData(row.SelectSingleNode(noticeUrlQuery)?.InnerText);
                        var subject = CleanStringData(row.SelectSingleNode(subjectQuery)?.InnerText.Trim());

                        if (DateTime.TryParse(row.SelectSingleNode(postedDateTimeQuery)?.InnerText.Trim(), out postedDateTime)
                            && DateTime.TryParse(row.SelectSingleNode(effectiveStartQuery)?.InnerText.Trim(), out effectiveStart)
                            && DateTime.TryParse(row.SelectSingleNode(effectiveEndQuery)?.InnerText.Trim(), out effectiveEnd)
                            && int.TryParse(row.SelectSingleNode(noticeIdQuery)?.InnerText.Trim(), out noticeId))
                        {
                            var notice = new Notice
                            {
                                NoticeTypeDescription = noticeTypeDescription,
                                NoticeUrl = noticeUrl,
                                PostedDateTime = postedDateTime,
                                EffectiveStart = effectiveStart,
                                EffectiveEnd = effectiveEnd,
                                NoticeId = noticeId,
                                Subject = subject
                            };
                            data.Add(notice);
                        }
                    }
                }
            }

            return data;
        }

        private string CleanStringData(string data)
        {
            return data?.Trim() ?? string.Empty;
        }
    }   
}
