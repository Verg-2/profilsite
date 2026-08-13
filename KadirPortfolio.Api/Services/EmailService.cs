using System.Net;
using System.Net.Mail;

namespace KadirPortfolio.Api.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IConfiguration _config;

        public EmailService(ILogger<EmailService> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                var host = _config["EmailSettings:Host"] ?? "smtp.gmail.com";
                var port = int.Parse(_config["EmailSettings:Port"] ?? "587");
                var email = _config["EmailSettings:Email"];
                var password = _config["EmailSettings:Password"];

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || password == "GMAIL_UYGULAMA_SIFRENIZI_YAZIN")
                {
                    _logger.LogWarning("Email ayarları (Email veya Password) eksik. Konsola yazdırılıyor:");
                    _logger.LogWarning($"TO: {to} | SUBJECT: {subject} | BODY: {body}");
                    return;
                }

                using var client = new SmtpClient(host, port)
                {
                    Credentials = new NetworkCredential(email, password),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(email, "Kadir Portfolio Sistem"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(to);

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation($"Email başarıyla gönderildi: {to}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Email gönderme hatası: {ex.Message}");
            }
        }
    }
}
