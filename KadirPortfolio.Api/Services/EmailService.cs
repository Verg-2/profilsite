using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MailKit.Net.Smtp;
using MimeKit;
using MailKit.Security;

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

            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Kadir Portfolio Sistem", email));
                message.To.Add(new MailboxAddress(to, to));
                message.Subject = subject;

                var builder = new BodyBuilder { HtmlBody = body };
                message.Body = builder.ToMessageBody();

                using var client = new SmtpClient();
                client.Timeout = 10000; // 10 seconds timeout

                await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(email, password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation($"Email başarıyla gönderildi: {to}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Email gönderme hatası: {ex.Message}");
                throw new Exception($"E-posta sunucusuna bağlanılamadı: {ex.Message}");
            }
        }
    }
}
