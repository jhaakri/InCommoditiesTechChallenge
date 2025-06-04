using InCommoditiesTechChallenge.Models;
using MimeKit;

namespace InCommoditiesTechChallenge
{
    public interface IMimeMessageBuilder
    {
        void BuildFrom(string from);
        void BuildTo(List<string> to);
        void BuildCc(List<string> cc);
        void BuildBcc(List<string> bcc);
        void BuildBody(string pipeline, List<Notice> notices);
        void BuildSubject(string subject);

        MimeMessage GetMessage();

    }
}
