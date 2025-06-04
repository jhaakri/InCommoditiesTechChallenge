using InCommoditiesTechChallenge.Models;
using MimeKit;

namespace InCommoditiesTechChallenge
{
    public class MimeMessageBuilder: IMimeMessageBuilder
    {
        private MimeMessage _mimeMessage = new();

        public MimeMessageBuilder() { }

        public void BuildFrom(string from)
        {
            if (MailboxAddress.TryParse(from, out MailboxAddress emailFrom))
            {
                _mimeMessage.From.Add(emailFrom);
                _mimeMessage.Sender = emailFrom;
            }
        }

        public void BuildBody(string pipeline, List<Notice> notices)
        {
            var strBody = "<!DOCTYPE html><html><body>";

            foreach (Notice notice in notices)
            {
                string body = $"<b>Title</b>: {notice.Subject}<br />" +
                              $"<b>Type</b>: {notice.NoticeTypeDescription}<br />" +
                              $"<b>Affected Pipeline</b>: {pipeline}<br />" +
                              $"<b>Volume</b>: {notice.Volume}<br />" +
                              $"<b>Link</b>: <a href='{notice.NoticeUrl}'>View Notice</a><br /><br />";
                
                strBody += body;
            }

            strBody += "</body></html>";

            _mimeMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = strBody
            };
        }

        public void BuildTo(List<string> to)
        {
            foreach (var recipient in to)
            {
                if (MailboxAddress.TryParse(recipient, out MailboxAddress emailTo))
                {
                    _mimeMessage.To.Add(emailTo);
                }
            }
        }

        public void BuildCc(List<string> cc)
        {
            if (cc?.Any() == true)
            {
                foreach (var recipient in cc)
                {
                    if (MailboxAddress.TryParse(recipient, out MailboxAddress emailCc))
                    {
                        _mimeMessage.Cc.Add(emailCc);
                    }
                }
            }
        }

        public void BuildBcc(List<string> bcc)
        {
            if (bcc?.Any() == true)
            {
                foreach (var recipient in bcc)
                {
                    if (MailboxAddress.TryParse(recipient, out MailboxAddress emailBcc))
                    {
                        _mimeMessage.Bcc.Add(emailBcc);
                    }
                }
            }
        }

        public void BuildSubject(string subject)
        {
            _mimeMessage.Subject = subject ?? string.Empty;
        }

        public void Reset()
        {
            _mimeMessage = new MimeMessage();
        }

        public MimeMessage GetMessage()
        {
            MimeMessage message = _mimeMessage;
            Reset();
            return message;
        }
    }
}
