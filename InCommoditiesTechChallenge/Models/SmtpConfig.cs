
namespace InCommoditiesTechChallenge.Models
{
    public class SmtpConfig
    {
        public required string Server { get; set; }
        public required int Port { get; set; }
        public required string Email { get; set; }
        public required string  Password { get; set; }
        public Boolean enableSsl { get; set; } = true;
        public required int EmailRetryTimeout { get; set; }
        public required int EmailRetryCount { get; set; }

    }
}
