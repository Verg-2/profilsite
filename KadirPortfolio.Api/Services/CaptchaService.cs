namespace KadirPortfolio.Api.Services
{
    public class CaptchaService : ICaptchaService
    {
        private readonly ILogger<CaptchaService> _logger;
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        public CaptchaService(ILogger<CaptchaService> logger, IConfiguration config, HttpClient httpClient)
        {
            _logger = logger;
            _config = config;
            _httpClient = httpClient;
        }

        public async Task<bool> VerifyCaptchaAsync(string token)
        {
            var secretKey = _config["YandexCaptcha:SecretKey"];
            
            // Eğer secret key girilmemişse veya mock kullanılıyorsa yerel test için true dön.
            if (string.IsNullOrEmpty(secretKey) || secretKey == "BURAYA_SECRET_KEY_YAZILACAK")
            {
                _logger.LogInformation("Yandex Captcha mocked (Secret Key bulunamadı).");
                return true; 
            }

            try
            {
                var response = await _httpClient.GetAsync($"https://smartcaptcha.yandexcloud.net/validate?secret={secretKey}&token={token}");
                var result = await response.Content.ReadFromJsonAsync<YandexCaptchaResponse>();
                return result != null && result.status == "ok";
            }
            catch (Exception ex)
            {
                _logger.LogError($"Captcha doğrulama hatası: {ex.Message}");
                return false;
            }
        }

        private class YandexCaptchaResponse
        {
            public string status { get; set; }
            public string message { get; set; }
        }
    }
}
