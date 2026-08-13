using System.Text.Json;
using System.Text;
using KadirPortfolio.Api.Models;
using Microsoft.Extensions.Options;

namespace KadirPortfolio.Api.Services
{
    public class TelegramService : ITelegramService
    {
        private readonly TelegramAyarlari _ayarlar;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<TelegramService> _logger;

        public TelegramService(
            IOptions<TelegramAyarlari> ayarlar,
            IHttpClientFactory httpClientFactory,
            ILogger<TelegramService> logger)
        {
            _ayarlar = ayarlar.Value;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<string> MesajGonderAsync(IletisimMesaji model)
        {
            if (string.IsNullOrEmpty(_ayarlar.BotToken) || string.IsNullOrEmpty(_ayarlar.ChatId))
            {
                _logger.LogError("Telegram BotToken veya ChatId tanımlanmamış.");
                return "Telegram ayarları eksik.";
            }

            try
            {
                var tarihStr = model.GonderimTarihi != default ? model.GonderimTarihi.AddHours(3).ToString("dd MMMM yyyy, HH:mm") : DateTime.UtcNow.AddHours(3).ToString("dd MMMM yyyy, HH:mm");
                
                var safeAd = System.Net.WebUtility.HtmlEncode(model.Ad);
                var safeSoyad = System.Net.WebUtility.HtmlEncode(model.Soyad);
                var safeEmail = System.Net.WebUtility.HtmlEncode(model.Email);
                var safeMesaj = System.Net.WebUtility.HtmlEncode(model.Mesaj);

                var mesaj = $"<b>Yeni İletişim Mesajı</b>\n\n" +
                            $"<b>Tarih:</b> {tarihStr}\n" +
                            $"<b>Ad Soyad:</b> {safeAd} {safeSoyad}\n" +
                            $"<b>E-posta:</b> {safeEmail}\n" +
                            $"<b>Mesaj:</b>\n{safeMesaj}";

                var url = $"https://api.telegram.org/bot{_ayarlar.BotToken}/sendMessage";
                var payload = new
                {
                    chat_id = _ayarlar.ChatId,
                    text = mesaj,
                    parse_mode = "HTML"
                };

                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                
                var client = _httpClientFactory.CreateClient();
                var response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    return "Mesaj başarıyla gönderildi.";
                }
                
                _logger.LogError($"Telegram API hatası: {response.StatusCode}");
                return "Mesaj gönderilirken bir hata oluştu.";
            }
            catch (Exception ex)
            {
                _logger.LogError($"Telegram gönderim hatası: {ex.Message}");
                return "Mesaj gönderilirken bir hata oluştu.";
            }
        }
    }
}