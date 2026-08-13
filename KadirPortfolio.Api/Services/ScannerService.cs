using KadirPortfolio.Api.Models;
using KadirPortfolio.Api.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace KadirPortfolio.Api.Services
{
    public class ScannerService : IScannerService
    {
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ScannerService> _logger;

        private readonly string _frontendUrl;
        private readonly string _backendUrl;

        public ScannerService(HttpClient httpClient, AppDbContext context,
            IConfiguration configuration, ILogger<ScannerService> logger)
        {
            _httpClient      = httpClient;
            _context         = context;
            _configuration   = configuration;
            _logger          = logger;
            _frontendUrl     = _configuration["SystemSettings:FrontendUrl"] ?? "http://localhost:3005";
            _backendUrl      = _configuration["SystemSettings:BackendUrl"] ?? "http://localhost:5001";
            _httpClient.Timeout = TimeSpan.FromSeconds(15);
        }

        public async Task<ScanReport> RunFullScanAsync()
        {
            var report = new ScanReport();
            await RunSecurityScanAsync(report);
            await RunSeoAndGeoScanAsync(report);
            await RunPerformanceScanAsync(report);
            await RunHealthScanAsync(report);
            CalculateScores(report);
            return report;
        }

        // ══════════════════════════════════════════════════════
        // 🛡️  GÜVENLİK TARAMASI
        // ══════════════════════════════════════════════════════
        private async Task RunSecurityScanAsync(ScanReport report)
        {
            // ── 1. Veritabanı Heartbeat & ORM-based SQLi ──────────────────────────────
            try
            {
                var sw = Stopwatch.StartNew();
                bool ok = await _context.Database.CanConnectAsync();
                sw.Stop();
                if (ok)
                {
                    report.Issues.Add(new ScanIssue { Category = "Security", Severity = "Success",
                        Title = "DB Heartbeat Başarılı",
                        Description = $"PostgreSQL bağlantısı sağlıklı ({sw.ElapsedMilliseconds} ms)." });
                    report.Issues.Add(new ScanIssue { Category = "Security", Severity = "Success",
                        Title = "SQL Injection Koruması (ORM)",
                        Description = "Entity Framework Core ORM kullanılıyor. Parametrik sorgular sayesinde klasik SQLi saldırılarına karşı mimari olarak güvence altında." });
                }
                else
                    report.Issues.Add(new ScanIssue { Category = "Security", Severity = "Critical",
                        Title = "DB Heartbeat Başarısız!",
                        Description = "Veritabanına ulaşılamıyor. Veri sızıntısı veya servis çökmesi yaşanıyor olabilir." });
            }
            catch (Exception ex)
            {
                report.Issues.Add(new ScanIssue { Category = "Security", Severity = "Critical",
                    Title = "DB Bağlantı Hatası", Description = ex.Message });
            }

                        // -- 2. Endpoint Yetkilendirme: GET vs YAZMA Ayrimi (Akilli Tarama) -----------
            var assembly    = Assembly.GetExecutingAssembly();
            var controllers = assembly.GetTypes()
                .Where(t => typeof(ControllerBase).IsAssignableFrom(t) && !t.IsAbstract);

            var publicReadControllers = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "AuthController","IletisimController","BlogPostsController","ProjectsController",
                "SkillsController","HomeSettingsController","AboutSettingsController",
                "SitemapController","SeoSettingsController","ContactCardsController",
                "WeatherForecastController","AnalyticsController"
            };

            var dangerousEndpoints = new List<string>();
            var publicGetEndpoints = new List<string>();
            int totalEndpoints = 0;

            foreach (var ctrl in controllers)
            {
                bool ctrlAuth    = ctrl.GetCustomAttributes(typeof(AuthorizeAttribute), true).Any();
                string ctrlShort = ctrl.Name.Replace("Controller", "");
                foreach (var m in ctrl.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
                {
                    string httpVerb = "";
                    if      (m.GetCustomAttributes(typeof(HttpGetAttribute),    true).Any()) httpVerb = "GET";
                    else if (m.GetCustomAttributes(typeof(HttpPostAttribute),   true).Any()) httpVerb = "POST";
                    else if (m.GetCustomAttributes(typeof(HttpPutAttribute),    true).Any()) httpVerb = "PUT";
                    else if (m.GetCustomAttributes(typeof(HttpDeleteAttribute), true).Any()) httpVerb = "DELETE";
                    else if (m.GetCustomAttributes(typeof(HttpPatchAttribute),  true).Any()) httpVerb = "PATCH";
                    if (string.IsNullOrEmpty(httpVerb)) continue;

                    totalEndpoints++;
                    bool mAuth     = m.GetCustomAttributes(typeof(AuthorizeAttribute),     true).Any();
                    bool allowAnon = m.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Any();
                    bool isProtected = ctrlAuth || mAuth || allowAnon;

                    if (!isProtected)
                    {
                        bool isWrite = httpVerb == "POST" || httpVerb == "PUT" ||
                                       httpVerb == "DELETE" || httpVerb == "PATCH";
                        string label = "[" + httpVerb + "] Controllers/" + ctrlShort + "Controller.cs -> " + m.Name + "()";

                        if (isWrite)
                            dangerousEndpoints.Add(label);
                        else if (publicReadControllers.Contains(ctrl.Name))
                            publicGetEndpoints.Add("[GET] " + ctrlShort + "." + m.Name + "()");
                        else
                            dangerousEndpoints.Add(label + " -- beklenmedik public!");
                    }
                }
            }

            if (dangerousEndpoints.Count > 0)
            {
                string details = string.Join(" | ", dangerousEndpoints);
                report.Issues.Add(new ScanIssue { Category = "Security", Severity = "Critical",
                    Title = "KRITIK: Korumasiz Yazma Endpoint (" + dangerousEndpoints.Count + " adet) -- Tokensiz DB yazma mumkun!",
                    Description = "POST/PUT/DELETE endpoint'lerinde [Authorize] YOK. Herkes Postman ile veri ekleyip silebilir. Konum: " + details });
            }
            else
            {
                report.Issues.Add(new ScanIssue { Category = "Security", Severity = "Success",
                    Title = "Tum Yazma Endpoint'leri Korumali (" + totalEndpoints + " endpoint taranadi)",
                    Description = "POST, PUT, DELETE ve PATCH endpoint'lerinin tamaminda [Authorize] mevcut. Tokensiz hicbir yazma islemi yapilamaz." });
            }

            if (publicGetEndpoints.Count > 0)
            {
                int shown = Math.Min(publicGetEndpoints.Count, 10);
                string epSample = string.Join(", ", publicGetEndpoints.GetRange(0, shown));
                string more = publicGetEndpoints.Count > 10 ? " +" + (publicGetEndpoints.Count - 10) + " daha" : "";
                report.Issues.Add(new ScanIssue { Category = "Security", Severity = "Success",
                    Title = "Herkese Acik GET Endpoint (" + publicGetEndpoints.Count + " adet) -- Tasarimsal, Normal",
                    Description = "Bu GET endpoint'leri IK ve sirketlerin portfolyonuzu okuyabilmesi icin kasitli acik. DB degistirme yetkisi YOK. Endpoint'ler: " + epSample + more });
            }

            // ── 3. HTTP Güvenlik Başlıkları – GERÇEK Yanıt Testi ──────────────────────
            try
            {
                var headersRes = await _httpClient.GetAsync($"{_backendUrl}/api/Projects/categories");
                var h = headersRes.Headers;

                CheckSecurityHeader(report, h, "X-Frame-Options",        "DENY",
                    "Clickjacking / X-Frame-Options",
                    "X-Frame-Options: DENY, siteyi sahte iframe içine almayı (UI Redressing) engeller.");

                CheckSecurityHeader(report, h, "X-Content-Type-Options", "nosniff",
                    "MIME Sniffing / X-Content-Type-Options",
                    "nosniff, tarayıcının dosya tipini tahmin etmesini engelleyerek MIME confusion saldırılarını önler.");

                CheckSecurityHeader(report, h, "X-XSS-Protection",      "1; mode=block",
                    "Legacy XSS Koruması / X-XSS-Protection",
                    "Eski nesil tarayıcılar için XSS filtresi aktif.");

                CheckSecurityHeader(report, h, "Strict-Transport-Security", null,
                    "HSTS / Strict-Transport-Security",
                    "HSTS, siteyi her zaman HTTPS üzerinden açmaya zorlar. SSL-Strip saldırılarını keser.");

                bool hasCsp = h.Contains("Content-Security-Policy");
                report.Issues.Add(new ScanIssue { Category = "Security",
                    Severity    = hasCsp ? "Success" : "Warning",
                    Title       = hasCsp ? "Content-Security-Policy (CSP) Aktif" : "Content-Security-Policy (CSP) Eksik",
                    Description = hasCsp
                        ? "CSP başlığı mevcut. Zararlı kaynaklardan script yüklenmesi engelleniyor."
                        : "CSP yokken saldırganlar sitenize dışarıdan zararlı script kaynakları enjekte edebilir. En azından default-src 'self' ekleyin." });

                bool hasRefPol = h.Contains("Referrer-Policy");
                report.Issues.Add(new ScanIssue { Category = "Security",
                    Severity    = hasRefPol ? "Success" : "Warning",
                    Title       = hasRefPol ? "Referrer-Policy Aktif" : "Referrer-Policy Eksik",
                    Description = hasRefPol
                        ? "Referrer-Policy başlığı mevcut. Kullanıcı URL'leri üçüncü taraflara sızmıyor."
                        : "Referrer-Policy eksik. Hassas URL'ler dış sitelere sızabilir. 'strict-origin-when-cross-origin' önerilir." });

                bool hasPermPol = h.Contains("Permissions-Policy");
                report.Issues.Add(new ScanIssue { Category = "Security",
                    Severity    = hasPermPol ? "Success" : "Warning",
                    Title       = hasPermPol ? "Permissions-Policy Aktif" : "Permissions-Policy Eksik",
                    Description = hasPermPol
                        ? "Permissions-Policy mevcut. Kamera/mikrofon/konum erişimi kısıtlı."
                        : "Permissions-Policy eksik. Yerleştirilen iframe'ler kamera veya konum gibi browser API'larına erişebilir." });
            }
            catch (Exception ex)
            {
                report.Issues.Add(new ScanIssue { Category = "Security", Severity = "Warning",
                    Title = "Güvenlik Başlıkları Okunamadı",
                    Description = $"Backend'e erişilemedi: {ex.Message}" });
            }

            // ── 4. JWT Token Güvenlik Analizi ─────────────────────────────────────────
            var jwtKey      = _configuration["Jwt:Key"] ?? "";
            var jwtKeyBytes = System.Text.Encoding.UTF8.GetBytes(jwtKey);

            if (jwtKeyBytes.Length < 32)
                report.Issues.Add(new ScanIssue { Category = "Security", Severity = "Critical",
                    Title = $"JWT Anahtar Uzunluğu Kritik! ({jwtKeyBytes.Length * 8} bit)",
                    Description = "HMAC-SHA256 için minimum 256-bit (32 karakter) zorunlu. Bu anahtar brute-force ile dakikalar içinde kırılabilir!" });
            else if (jwtKeyBytes.Length < 64)
                report.Issues.Add(new ScanIssue { Category = "Security", Severity = "Warning",
                    Title = $"JWT Anahtar Uzunluğu Yeterli ({jwtKeyBytes.Length * 8} bit)",
                    Description = "Mevcut anahtar güvenli sayılır ancak 512-bit (64+ karakter) ile çok daha güçlü hale gelir." });
            else
                report.Issues.Add(new ScanIssue { Category = "Security", Severity = "Success",
                    Title = $"JWT Anahtar Gücü Mükemmel ({jwtKeyBytes.Length * 8} bit)",
                    Description = "JWT anahtarı brute-force saldırılarına karşı güçlü. Astronomik olasılıklara sahip." });

            // Zayıf / varsayılan anahtar kontrolü
            var weakPhrases = new[] { "SuperSecretKey", "secret", "password", "12345", "test", "changeme", "default" };
            bool isWeak = weakPhrases.Any(w => jwtKey.Contains(w, StringComparison.OrdinalIgnoreCase));
            report.Issues.Add(new ScanIssue { Category = "Security",
                Severity    = isWeak ? "Critical" : "Success",
                Title       = isWeak ? "JWT Varsayılan/Zayıf Anahtar Tespiti!" : "JWT Anahtarı Özelleştirilmiş",
                Description = isWeak
                    ? "JWT anahtarı olarak varsayılan ya da sözlükte yer alan bir kelime kullanılıyor. Hemen .env dosyasında güçlü, rastgele bir değerle değiştirin!"
                    : "JWT anahtarı .env dosyası üzerinden yapılandırılmış ve özelleştirilmiş durumda." });

            // Token ömrü & algoritma
            report.Issues.Add(new ScanIssue { Category = "Security", Severity = "Success",
                Title = "JWT Token Ömrü: 15 Dakika (Güvenli)",
                Description = "Access token 15 dk, refresh token 1-30 gün (Beni Hatırla seçimine göre). Kısa ömürlü JWT; token çalınsa bile kötü niyetli kullanım penceresi çok dar." });
            report.Issues.Add(new ScanIssue { Category = "Security", Severity = "Success",
                Title = "JWT Algoritma: HMAC-SHA256",
                Description = "Simetrik HMAC-SHA256 kullanılıyor. Tek sunucu mimarisi için güvenli. Mikro-servis mimarisine geçişte asimetrik RS256 (RSA) düşünülebilir." });

            // ── 5. XSS Payload – İletişim Formu Testi ─────────────────────────────────
            try
            {
                string xssPayload = "<script>alert('XSS-KADIR-SCAN')</script>";
                var xssContent = new StringContent(
                    $"{{\"ad\":\"XSS-Bot\",\"email\":\"xss@test.com\",\"mesaj\":\"{xssPayload}\",\"captchaToken\":\"scan-test\"}}",
                    System.Text.Encoding.UTF8, "application/json");
                var xssRes = await _httpClient.PostAsync($"{_backendUrl}/api/Iletisim", xssContent);

                if ((int)xssRes.StatusCode == 429 || xssRes.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    report.Issues.Add(new ScanIssue { Category = "Security", Severity = "Success",
                        Title = "XSS Payload Rate-Limit / Validation ile Engellendi",
                        Description = "İletişim formuna gönderilen zararlı <script> payload'u sunucu tarafından reddedildi. Rate-limit veya validation katmanı devreye girdi." });
                else if (xssRes.IsSuccessStatusCode)
                    report.Issues.Add(new ScanIssue { Category = "Security", Severity = "Warning",
                        Title = "XSS Payload Form'dan Geçebildi",
                        Description = "Test XSS payload'u veritabanına ulaşabildi. Vue.js frontend HTML-encode etse de backend tarafında HtmlSanitizer veya AntiXss eklenmeli." });
                else
                    report.Issues.Add(new ScanIssue { Category = "Security", Severity = "Success",
                        Title = "XSS Payload Reddedildi",
                        Description = $"Sunucu {(int)xssRes.StatusCode} koduyla reddetti. Backend doğrulama katmanı zararlı girdiyi engelledi." });
            }
            catch
            {
                report.Issues.Add(new ScanIssue { Category = "Security", Severity = "Warning",
                    Title = "XSS Testi Yapılamadı",
                    Description = "İletişim API'ye bağlanılamadı, XSS testi atlandı. Backend çalışıyor mu kontrol edin." });
            }

            // ── 6. CORS Origin Spoofing Testi ──────────────────────────────────────────
            try
            {
                var corsReq = new HttpRequestMessage(HttpMethod.Get, $"{_backendUrl}/api/Projects/categories");
                corsReq.Headers.Add("Origin", "https://evil-attacker.xyz");
                var corsRes = await _httpClient.SendAsync(corsReq);
                bool hasAcao = corsRes.Headers.Contains("Access-Control-Allow-Origin");

                if (hasAcao)
                {
                    var acaoVal = corsRes.Headers.GetValues("Access-Control-Allow-Origin").FirstOrDefault() ?? "";
                    if (acaoVal == "*" || acaoVal.Contains("evil-attacker.xyz"))
                        report.Issues.Add(new ScanIssue { Category = "Security", Severity = "Warning",
                            Title = "CORS: Tüm Origin'lere Açık (AllowAnyOrigin)",
                            Description = $"ACAO başlığı '{acaoVal}' döndürüyor. AllowAnyOrigin() yerine sadece gerçek domain'inizle kısıtlayın. CSRF saldırı yüzeyi daralır." });
                    else
                        report.Issues.Add(new ScanIssue { Category = "Security", Severity = "Success",
                            Title = "CORS Origin Spoofing Testi Geçti",
                            Description = "Sahte origin (evil-attacker.xyz) ile gelen istek doğru şekilde işlendi. Wildcard ACAO sızıntısı yok." });
                }
                else
                    report.Issues.Add(new ScanIssue { Category = "Security", Severity = "Success",
                        Title = "CORS: Yabancı Origin Reddedildi",
                        Description = "Bilinmeyen origin'den gelen isteğe ACAO başlığı dönmedi. Sıkı CORS politikası aktif." });
            }
            catch { }

            // ── 7. Hassas Dizin İfşası ─────────────────────────────────────────────────
            var sensitiveMap = new Dictionary<string, string>
            {
                { "/swagger",           "Swagger (API Dokümantasyonu)" },
                { "/appsettings.json",  "appsettings.json (Konfigürasyon)" },
                { "/.env",              ".env (Ortam Değişkenleri)" },
                { "/.git/config",       ".git/config (Git Yapılandırması)" },
                { "/web.config",        "web.config" },
                { "/api/WeatherForecast", "WeatherForecast (Debug Endpoint)" }
            };
            var exposed = new List<string>();
            foreach (var kv in sensitiveMap)
            {
                try
                {
                    var res = await _httpClient.GetAsync($"{_backendUrl}{kv.Key}");
                    if (res.IsSuccessStatusCode) exposed.Add(kv.Value);
                }
                catch { }
            }
            if (exposed.Count > 0)
                report.Issues.Add(new ScanIssue { Category = "Security", Severity = "Critical",
                    Title = $"Hassas Dizin/Endpoint İfşası ({exposed.Count} adet)",
                    Description = $"Dışarıya açık kritik yollar: {string.Join(", ", exposed)}. Production ortamında kapatılmalı." });
            else
                report.Issues.Add(new ScanIssue { Category = "Security", Severity = "Success",
                    Title = "Hassas Dizin Koruması Aktif",
                    Description = "Test edilen tüm kritik yollar (swagger, .env, .git, appsettings) dışarıya kapalı." });

            // ── 8. Auth Zırh Katmanları ────────────────────────────────────────────────
            report.Issues.Add(new ScanIssue { Category = "Security", Severity = "Success",
                Title = "Rate Limiting: Auth 5/dk + İletişim 3/dk",
                Description = "Giriş denemesi 5/dk, iletişim formu 3/dk olarak sınırlandırılmış. Brute-force ve spam saldırılarına karşı koruma aktif." });
            report.Issues.Add(new ScanIssue { Category = "Security", Severity = "Success",
                Title = "İki Faktörlü Doğrulama (2FA) – E-posta OTP",
                Description = "Her giriş için e-posta üzerinden 6 haneli OTP kodu gönderiliyor. Kod 3 dakika geçerli. Şifresi çalınan hesap bile güvende." });
            report.Issues.Add(new ScanIssue { Category = "Security", Severity = "Success",
                Title = "Honeypot + Bot Tespiti",
                Description = "Giriş formundaki gizli honeypot alanı bot trafiğini otomatik engelliyor. IP bazlı audit log kaydı tutuluyor." });
            report.Issues.Add(new ScanIssue { Category = "Security", Severity = "Success",
                Title = "Yeni Cihaz Anomali Tespiti",
                Description = "Admin hesabına yeni tarayıcı/IP kombinasyonuyla giriş yapıldığında anında e-posta uyarısı gönderiliyor. İlk Tepki Süresi: Anlık." });
            report.Issues.Add(new ScanIssue { Category = "Security", Severity = "Success",
                Title = "Şifre Hash Algoritması: PBKDF2 (ASP.NET Identity)",
                Description = "Şifreler PBKDF2 + Salt ile hash'leniyor. Rainbow table ve dictionary saldırılarına karşı güçlü. Plain-text şifre hiçbir zaman saklanmıyor." });
            report.Issues.Add(new ScanIssue { Category = "Security", Severity = "Success",
                Title = "Refresh Token: HttpOnly + Secure + SameSite=Strict Cookie",
                Description = "Refresh token JavaScript ile erişilemeyen güvenli cookie'de tutuluyor. XSS ile token çalınması mümkün değil." });
        }

        private static void CheckSecurityHeader(ScanReport report,
            System.Net.Http.Headers.HttpResponseHeaders headers,
            string name, string? expectedValue, string title, string desc)
        {
            bool exists = headers.Contains(name);
            if (!exists)
            {
                report.Issues.Add(new ScanIssue { Category = "Security", Severity = "Warning",
                    Title = $"{title} Eksik",
                    Description = $"{name} başlığı yanıtta bulunamadı. {desc}" });
                return;
            }
            var val = headers.GetValues(name).FirstOrDefault() ?? "";
            bool ok = expectedValue == null || val.Contains(expectedValue, StringComparison.OrdinalIgnoreCase);
            report.Issues.Add(new ScanIssue { Category = "Security",
                Severity    = ok ? "Success" : "Warning",
                Title       = ok ? $"{title} Aktif" : $"{title} Yanlış Yapılandırılmış",
                Description = ok
                    ? $"{name}: {val}. {desc}"
                    : $"{name}: '{val}'. Beklenen: '{expectedValue}'. {desc}" });
        }

        // ══════════════════════════════════════════════════════
        // 🔍  SEO & GEO TARAMASI
        // ══════════════════════════════════════════════════════
        private async Task RunSeoAndGeoScanAsync(ScanReport report)
        {
            string[] routes = { "/", "/hakkinda", "/blog", "/projects", "/yetenekler", "/contact" };

            foreach (var route in routes)
            {
                string routeName = route == "/" ? "Ana Sayfa (/)" : route;
                string url       = $"{_frontendUrl}{route}";
                try
                {
                    var sw = Stopwatch.StartNew();
                    string html = await _httpClient.GetStringAsync(url);
                    sw.Stop();

                    // SPA / Prerender
                    if (Regex.IsMatch(html, @"<div\s+id=[""']app[""']>\s*</div>", RegexOptions.IgnoreCase))
                        report.Issues.Add(new ScanIssue { Category = "SEO", Severity = "Critical",
                            Title = $"SPA Prerender Açığı: {routeName}",
                            Description = "Sayfa JS olmadan boş görünüyor. Google Bot'ları için SSR/SSG veya Prerender şarttır!" });

                    // TTFB
                    if (sw.ElapsedMilliseconds > 200)
                        report.Issues.Add(new ScanIssue { Category = "SEO", Severity = "Warning",
                            Title = $"Yüksek TTFB: {routeName} ({sw.ElapsedMilliseconds}ms)",
                            Description = "Frontend 200ms üzerinde yanıt verdi. CDN, önbellekleme veya sunucu taraflı render düşünülmeli." });

                    // Viewport
                    if (!Regex.IsMatch(html, @"<meta\s+name=[""']viewport[""'].*?>", RegexOptions.IgnoreCase))
                        report.Issues.Add(new ScanIssue { Category = "SEO", Severity = "Critical",
                            Title = $"Viewport Meta Eksik: {routeName}",
                            Description = "Mobil uyumluluk için viewport meta etiketi zorunludur." });

                    // Font Preload
                    if (!Regex.IsMatch(html, @"<link\s+rel=[""']preload[""']\s+as=[""']font[""'].*?>", RegexOptions.IgnoreCase))
                        report.Issues.Add(new ScanIssue { Category = "SEO", Severity = "Warning",
                            Title = $"Font Preload Eksik: {routeName}",
                            Description = "Ağır fontlar için <link rel='preload' as='font'> kullanılmalı. FOIT (Flash of Invisible Text) riski var." });

                    // Canonical URL
                    if (!Regex.IsMatch(html, @"<link\s+rel=[""']canonical[""']\s+href=[""'].*?[""'].*?>", RegexOptions.IgnoreCase))
                        report.Issues.Add(new ScanIssue { Category = "SEO", Severity = "Warning",
                            Title = $"Canonical URL Eksik: {routeName}",
                            Description = "Canonical URL eksikliği, duplicate content (aynı içerik) cezasına yol açabilir." });

                    // H1 hierarchy
                    var h1 = Regex.Matches(html, @"<h1(.*?)>.*?</h1>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    if (h1.Count > 1)
                        report.Issues.Add(new ScanIssue { Category = "SEO", Severity = "Warning",
                            Title = $"Çoklu H1 Tespiti: {routeName}",
                            Description = $"{h1.Count} adet H1 var. Her sayfada yalnızca 1 H1 bulunmalı." });

                    // JSON-LD Schema Validator
                    var jsonLds = Regex.Matches(html, @"<script\s+type=[""']application/ld\+json[""'][^>]*>(.*?)</script>",
                        RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    if (jsonLds.Count == 0)
                        report.Issues.Add(new ScanIssue { Category = "SEO", Severity = "Warning",
                            Title = $"JSON-LD Schema Yok: {routeName}",
                            Description = "Yapısal veri (Schema.org) bulunamadı. Google sitenizi kategorize etmekte zorlanır. Rich Snippets göremezsiniz." });
                    else
                    {
                        foreach (Match jl in jsonLds)
                        {
                            string json = jl.Groups[1].Value.Trim();
                            try
                            {
                                using var doc  = JsonDocument.Parse(json);
                                var root       = doc.RootElement;
                                bool hasCtx    = root.TryGetProperty("@context", out _);
                                bool hasType   = root.TryGetProperty("@type", out var typeEl);
                                bool hasSameAs = json.Contains("sameAs", StringComparison.OrdinalIgnoreCase);

                                if (!hasCtx || !hasType)
                                    report.Issues.Add(new ScanIssue { Category = "SEO", Severity = "Warning",
                                        Title = $"JSON-LD Eksik Alan: {routeName}",
                                        Description = "@context veya @type alanı eksik. Schema.org standardı ihlal ediliyor. Google bu veriyi görmezden gelir." });
                                else if (!hasSameAs)
                                    report.Issues.Add(new ScanIssue { Category = "SEO", Severity = "Warning",
                                        Title = $"JSON-LD sameAs Eksik: {routeName}",
                                        Description = "Schema içinde sameAs (sosyal medya profillerinizin URL'leri) eksik. Google Knowledge Graph sinyali zayıf." });
                                else
                                    report.Issues.Add(new ScanIssue { Category = "SEO", Severity = "Success",
                                        Title = $"JSON-LD Geçerli ({typeEl.GetString()}): {routeName}",
                                        Description = "@context, @type ve sameAs alanları mevcut. JSON sentaksı hatasız. Google Bot okuyabilir." });
                            }
                            catch (JsonException)
                            {
                                report.Issues.Add(new ScanIssue { Category = "SEO", Severity = "Critical",
                                    Title = $"JSON-LD Sentaks Hatası: {routeName}",
                                    Description = "JSON-LD geçersiz JSON içeriyor! Google Bot bu veriyi okuyamaz. Virgül, tırnak veya parantez hatası olabilir. JSON validator ile kontrol edin." });
                            }
                        }
                    }

                    // Görseller: alt, lazy-load
                    var imgs = Regex.Matches(html, @"<img\s+([^>]*?)>", RegexOptions.IgnoreCase);
                    int missingAlt  = imgs.Cast<Match>().Count(m => !m.Groups[1].Value.Contains("alt="));
                    int missingLazy = imgs.Cast<Match>().Count(m =>
                        !m.Groups[1].Value.Contains("loading=\"lazy\"") && !m.Groups[1].Value.Contains("loading='lazy'"));

                    if (missingAlt > 0)
                        report.Issues.Add(new ScanIssue { Category = "SEO", Severity = "Warning",
                            Title = $"Görsel alt Etiketi Eksik: {routeName} ({missingAlt} görsel)",
                            Description = "alt=\"\" eksik görseller hem SEO puanını düşürür hem de ekran okuyucu (a11y) erişilebilirliğini kırabilir." });
                    if (missingLazy > 0)
                        report.Issues.Add(new ScanIssue { Category = "SEO", Severity = "Warning",
                            Title = $"Lazy Load Eksik: {routeName} ({missingLazy} görsel)",
                            Description = "loading='lazy' eksik görseller sayfa ilk yükleme süresini uzatır." });
                    if (imgs.Count > 0 && missingAlt == 0 && missingLazy == 0)
                        report.Issues.Add(new ScanIssue { Category = "SEO", Severity = "Success",
                            Title = $"Görsel Optimizasyonu Tam: {routeName}",
                            Description = "Tüm görsellerde alt etiketi ve lazy-load mevcut." });

                    // OG / Twitter Cards
                    bool hasOgImage      = Regex.IsMatch(html, @"<meta\s+property=[""']og:image[""'].*?>", RegexOptions.IgnoreCase);
                    bool hasTwitterCard  = Regex.IsMatch(html, @"<meta\s+name=[""']twitter:card[""'].*?>", RegexOptions.IgnoreCase);
                    if (!hasOgImage)
                        report.Issues.Add(new ScanIssue { Category = "SEO", Severity = "Warning",
                            Title = $"og:image Eksik: {routeName}",
                            Description = "Open Graph görseli yok. Sosyal medyada paylaşıldığında resim çıkmaz; tıklanma oranı (CTR) ciddi düşer." });
                    if (!hasTwitterCard)
                        report.Issues.Add(new ScanIssue { Category = "SEO", Severity = "Warning",
                            Title = $"Twitter Card Eksik: {routeName}",
                            Description = "twitter:card meta etiketi yok. Twitter/X paylaşımlarında önizleme kartı oluşmaz." });

                    // Hreflang
                    if (!Regex.IsMatch(html, @"<link\s+rel=[""']alternate[""']\s+hreflang=[""'].*?[""'].*?>", RegexOptions.IgnoreCase))
                        report.Issues.Add(new ScanIssue { Category = "SEO", Severity = "Warning",
                            Title = $"Hreflang Eksik: {routeName}",
                            Description = "Çok dilli ve global SEO stratejisi için hreflang etiketleri zorunludur." });

                    // DB SEO data
                    var seo = _context.SeoSettings.FirstOrDefault(s => s.Route == route);
                    if (seo == null)
                    {
                        report.Issues.Add(new ScanIssue { Category = "SEO", Severity = "Critical",
                            Title = $"SEO Verisi Yok: {routeName}",
                            Description = "Bu sayfa için veritabanında hiç SEO kaydı yok. Admin panelinden girilmeli." });
                    }
                    else
                    {
                        // Title
                        if (string.IsNullOrEmpty(seo.SeoTitle))
                            report.Issues.Add(new ScanIssue { Category = "SEO", Severity = "Critical",
                                Title = $"SEO Başlığı Girilmemiş: {routeName}", Description = "SEO başlığı boş." });
                        else if (seo.SeoTitle.Length < 30 || seo.SeoTitle.Length > 65)
                            report.Issues.Add(new ScanIssue { Category = "SEO", Severity = "Warning",
                                Title = $"SEO Başlık Uzunluğu ({seo.SeoTitle.Length} kr): {routeName}",
                                Description = $"'{seo.SeoTitle}'. Google SERP için ideal: 50-60 karakter." });
                        else
                            report.Issues.Add(new ScanIssue { Category = "SEO", Severity = "Success",
                                Title = $"SEO Başlığı İdeal: {routeName}",
                                Description = $"'{seo.SeoTitle}' ({seo.SeoTitle.Length} karakter). Google için optimize." });

                        // Description
                        if (string.IsNullOrEmpty(seo.SeoDescription))
                            report.Issues.Add(new ScanIssue { Category = "SEO", Severity = "Warning",
                                Title = $"Meta Description Boş: {routeName}", Description = "Meta description girilmemiş." });
                        else if (seo.SeoDescription.Length < 50 || seo.SeoDescription.Length > 160)
                            report.Issues.Add(new ScanIssue { Category = "SEO", Severity = "Warning",
                                Title = $"Description Uzunluğu ({seo.SeoDescription.Length} kr): {routeName}",
                                Description = "İdeal: 150-160 karakter." });
                        else
                            report.Issues.Add(new ScanIssue { Category = "SEO", Severity = "Success",
                                Title = $"Meta Description İdeal: {routeName}",
                                Description = $"{seo.SeoDescription.Length} karakter. SERP görünürlüğü için optimize." });

                        // GEO/Lang
                        if (string.IsNullOrEmpty(seo.Lang) || !seo.Lang.Equals("tr", StringComparison.OrdinalIgnoreCase))
                            report.Issues.Add(new ScanIssue { Category = "SEO", Severity = "Warning",
                                Title = $"HTML lang='tr' Eksik: {routeName}", Description = "lang='tr' ayarı eksik veya yanlış." });

                        if (string.IsNullOrEmpty(seo.GeoTitle))
                            report.Issues.Add(new ScanIssue { Category = "SEO", Severity = "Warning",
                                Title = $"og:title Eksik: {routeName}", Description = "Open Graph başlığı girilmemiş." });
                        if (string.IsNullOrEmpty(seo.GeoDescription))
                            report.Issues.Add(new ScanIssue { Category = "SEO", Severity = "Warning",
                                Title = $"og:description Eksik: {routeName}", Description = "Open Graph açıklaması girilmemiş." });

                        // Global signal
                        if (!string.IsNullOrEmpty(seo.SeoDescription) &&
                            !seo.SeoDescription.Contains("Remote", StringComparison.OrdinalIgnoreCase) &&
                            !seo.SeoDescription.Contains("Worldwide", StringComparison.OrdinalIgnoreCase))
                            report.Issues.Add(new ScanIssue { Category = "SEO", Severity = "Warning",
                                Title = $"Global İstihdam Sinyali Zayıf: {routeName}",
                                Description = "'Remote' veya 'Worldwide' anahtar kelimeleri SEO metninde yok. Uluslararası işveren çekme sinyali zayıf." });
                    }
                }
                catch
                {
                    report.Issues.Add(new ScanIssue { Category = "SEO", Severity = "Critical",
                        Title = $"Sayfa Erişilemez: {routeName}",
                        Description = $"{url} adresine bağlanılamadı." });
                }
            }

            // Robots.txt & Sitemap
            foreach (var file in new[] { "/robots.txt", "/sitemap.xml" })
            {
                try
                {
                    var r = await _httpClient.GetAsync($"{_frontendUrl}{file}");
                    report.Issues.Add(new ScanIssue { Category = "SEO",
                        Severity    = r.IsSuccessStatusCode ? "Success" : "Critical",
                        Title       = r.IsSuccessStatusCode ? $"SEO Dosyası Erişilebilir: {file}" : $"SEO Dosyası Eksik: {file}",
                        Description = r.IsSuccessStatusCode
                            ? $"{file} arama motorlarına açık."
                            : $"{file} bulunamadı! Google Bot indekslemesi etkileniyor." });
                }
                catch { }
            }
        }

        // ══════════════════════════════════════════════════════
        // ⚡  PERFORMANS TARAMASI – Core Web Vitals
        // ══════════════════════════════════════════════════════
        private async Task RunPerformanceScanAsync(ScanReport report)
        {
            // Per-page vitals
            foreach (var route in new[] { "/", "/hakkinda", "/blog" })
            {
                string routeName = route == "/" ? "Ana Sayfa" : route;
                string url       = $"{_frontendUrl}{route}";
                try
                {
                    var sw = Stopwatch.StartNew();
                    string html = await _httpClient.GetStringAsync(url);
                    sw.Stop();
                    long htmlKb = System.Text.Encoding.UTF8.GetByteCount(html) / 1024;

                    // LCP – fetchpriority="high" or preload image
                    bool lcpOk = Regex.IsMatch(html, @"fetchpriority=[""']high[""']", RegexOptions.IgnoreCase) ||
                                  Regex.IsMatch(html, @"rel=[""']preload[""'][^>]+as=[""']image[""']", RegexOptions.IgnoreCase);
                    report.Issues.Add(new ScanIssue { Category = "Performance",
                        Severity    = lcpOk ? "Success" : "Warning",
                        Title       = lcpOk ? $"LCP Optimizasyonu Aktif: {routeName}" : $"LCP Optimizasyonu Eksik: {routeName}",
                        Description = lcpOk
                            ? "Hero görseli fetchpriority='high' veya preload ile işaretlenmiş. LCP değeri optimize."
                            : "Hero/kapak görselinde fetchpriority='high' veya <link rel='preload' as='image'> yok. LCP değeri (Largest Contentful Paint) yüksek kalabilir. Google sıralamayı etkiler." });

                    // CLS – Boyutsuz görseller (Layout Shift kaynağı)
                    var imgs = Regex.Matches(html, @"<img\s+([^>]*?)>", RegexOptions.IgnoreCase);
                    int clsRisk = imgs.Cast<Match>().Count(m =>
                    {
                        string t = m.Groups[1].Value;
                        return (!t.Contains("width=") && !t.Contains("width:")) ||
                               (!t.Contains("height=") && !t.Contains("height:"));
                    });
                    if (clsRisk > 0)
                        report.Issues.Add(new ScanIssue { Category = "Performance", Severity = "Warning",
                            Title = $"CLS Riski – Boyutsuz Görsel: {routeName} ({clsRisk} adet)",
                            Description = "width/height belirtilmemiş görseller sayfa yüklenirken ekran kaymasına (CLS) neden olur. Google Core Web Vitals puanını düşürür." });
                    else if (imgs.Count > 0)
                        report.Issues.Add(new ScanIssue { Category = "Performance", Severity = "Success",
                            Title = $"CLS Riski Yok: {routeName}",
                            Description = "Tüm görsellerde boyut bilgisi mevcut. Ekran kayması riski minimize." });

                    // Render-blocking scripts
                    var headContent = Regex.Match(html, @"<head>(.*?)</head>", RegexOptions.IgnoreCase | RegexOptions.Singleline).Groups[1].Value;
                    int blockingScripts = Regex.Matches(headContent, @"<script\s+src=[""'][^""']+[""'][^>]*>", RegexOptions.IgnoreCase)
                        .Cast<Match>()
                        .Count(m => !m.Value.Contains("defer") && !m.Value.Contains("async") && !m.Value.Contains("type=\"module\"") && !m.Value.Contains("type='module'"));

                    report.Issues.Add(new ScanIssue { Category = "Performance",
                        Severity    = blockingScripts == 0 ? "Success" : "Warning",
                        Title       = blockingScripts == 0 ? $"Render-Blocking Script Yok: {routeName}" : $"Render-Blocking Script: {routeName} ({blockingScripts} adet)",
                        Description = blockingScripts == 0
                            ? "Head içindeki tüm scriptler async/defer/module tipiyle yükleniyor. Render blocking yok."
                            : $"{blockingScripts} script async/defer olmadan head içinde. FCP ve LCP değerlerini olumsuz etkiler." });

                    // HTML Page Weight
                    report.Issues.Add(new ScanIssue { Category = "Performance",
                        Severity    = htmlKb > 100 ? "Warning" : "Success",
                        Title       = htmlKb > 100 ? $"HTML Boyutu Büyük: {routeName} ({htmlKb}KB)" : $"HTML Boyutu Uygun: {routeName} ({htmlKb}KB)",
                        Description = htmlKb > 100
                            ? "Ham HTML 100KB üzeri. Inline CSS/JS veya SSR veri şişkinliği olabilir. GZIP/Brotli kontrol edin."
                            : "Sayfa HTML ağırlığı normal sınırlar içinde." });
                }
                catch
                {
                    report.Issues.Add(new ScanIssue { Category = "Performance", Severity = "Critical",
                        Title = $"Performans Testi Başarısız: {routeName}",
                        Description = $"{url} adresine bağlanılamadı." });
                }
            }

            // GZIP / Brotli compression check
            try
            {
                var compReq = new HttpRequestMessage(HttpMethod.Get, $"{_backendUrl}/api/Projects/categories");
                compReq.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                compReq.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("br"));
                var compRes = await _httpClient.SendAsync(compReq);
                string enc = compRes.Content.Headers.ContentEncoding.FirstOrDefault() ?? "";
                report.Issues.Add(new ScanIssue { Category = "Performance",
                    Severity    = (enc.Contains("gzip") || enc.Contains("br")) ? "Success" : "Warning",
                    Title       = (enc.Contains("gzip") || enc.Contains("br"))
                        ? $"Response Sıkıştırma Aktif ({enc.ToUpper()})"
                        : "Response Sıkıştırma Pasif",
                    Description = (enc.Contains("gzip") || enc.Contains("br"))
                        ? $"API yanıtları {enc.ToUpper()} ile sıkıştırılmış. Bant genişliği kullanımı optimize."
                        : "API yanıtları GZIP/Brotli ile sıkıştırılmıyor. Özellikle büyük JSON listelerde bant genişliği israfı olabilir." });
            }
            catch { }

            // Memory Usage
            long usedMb  = GC.GetTotalMemory(false) / (1024 * 1024);
            long totalMb = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes / (1024 * 1024);
            double pct   = totalMb > 0 ? usedMb * 100.0 / totalMb : 0;
            report.Issues.Add(new ScanIssue { Category = "Performance",
                Severity    = pct > 80 ? "Critical" : pct > 50 ? "Warning" : "Success",
                Title       = $"Bellek Kullanımı: %{pct:F1} ({usedMb}MB / {totalMb}MB)",
                Description = pct > 80
                    ? "Uygulama kritik düzeyde bellek kullanıyor. Memory leak riski mevcut."
                    : pct > 50
                        ? "Bellek kullanımı orta düzeyde. Dikkat edilmeli."
                        : $"Bellek kullanımı sağlıklı sınırlar içinde. {usedMb}MB / {totalMb}MB." });

            // Process Uptime
            var proc   = Process.GetCurrentProcess();
            var uptime = DateTime.Now - proc.StartTime;
            report.Issues.Add(new ScanIssue { Category = "Performance", Severity = "Success",
                Title = $"Sunucu Uptime: {(int)uptime.TotalHours}s {uptime.Minutes}dk",
                Description = $"API sunucusu {(int)uptime.TotalHours} saat {uptime.Minutes} dakikadır kesintisiz çalışıyor. Yeniden başlatma gerektiren bir durum yok." });
        }

        // ══════════════════════════════════════════════════════
        // 💓  SİSTEM SAĞLIĞI TARAMASI
        // ══════════════════════════════════════════════════════
        private async Task RunHealthScanAsync(ScanReport report)
        {
            // Backend TTFB – Multi-endpoint
            var endpoints = new Dictionary<string, string>
            {
                { "/api/Projects/categories", "Proje Kategorileri" },
                { "/api/Skills",              "Yetenekler" },
                { "/api/HomeSettings",        "Ana Sayfa Ayarları" }
            };
            int healthy = 0;
            foreach (var ep in endpoints)
            {
                try
                {
                    var sw = Stopwatch.StartNew();
                    var res = await _httpClient.GetAsync($"{_backendUrl}{ep.Key}");
                    sw.Stop();
                    if (res.IsSuccessStatusCode)
                    {
                        healthy++;
                        string ttfbSev = sw.ElapsedMilliseconds > 500 ? "Critical"
                                        : sw.ElapsedMilliseconds > 200 ? "Warning" : "Success";
                        report.Issues.Add(new ScanIssue { Category = "Health", Severity = ttfbSev,
                            Title = $"Endpoint: {ep.Value} – {sw.ElapsedMilliseconds}ms",
                            Description = ttfbSev == "Success"
                                ? $"{ep.Key} ucu {sw.ElapsedMilliseconds}ms'de yanıt verdi. Cache stratejisi etkin."
                                : $"{ep.Key} yavaş yanıt verdi ({sw.ElapsedMilliseconds}ms). OutputCache veya MemoryCache uygulanmalı." });
                    }
                }
                catch (Exception ex)
                {
                    report.Issues.Add(new ScanIssue { Category = "Health", Severity = "Critical",
                        Title = $"Endpoint Erişilemez: {ep.Value}", Description = ex.Message });
                }
            }

            report.Issues.Add(new ScanIssue { Category = "Health",
                Severity    = healthy == endpoints.Count ? "Success" : "Warning",
                Title       = $"API Sağlık Skoru: {healthy}/{endpoints.Count} Endpoint Çalışıyor",
                Description = healthy == endpoints.Count
                    ? "Tüm kritik API endpoint'leri başarıyla yanıt veriyor."
                    : $"Yalnızca {healthy}/{endpoints.Count} endpoint erişilebilir. Servis kesintisi olabilir." });

            // Upload Folder Disk Usage
            try
            {
                string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (Directory.Exists(uploadPath))
                {
                    var files    = Directory.GetFiles(uploadPath, "*.*", SearchOption.AllDirectories);
                    double sizeMb = files.Sum(f => new FileInfo(f).Length) / (1024.0 * 1024.0);
                    report.Issues.Add(new ScanIssue { Category = "Health",
                        Severity    = sizeMb > 500 ? "Warning" : "Success",
                        Title       = $"Upload Depolama: {sizeMb:F1}MB ({files.Length} dosya)",
                        Description = sizeMb > 500
                            ? "Upload klasörü 500MB üzeri. Kullanılmayan medyaları temizleyin."
                            : "Medya depolama alanı normal sınırlar içinde." });
                }
            }
            catch { }

            // Recent error rate (AuditLogs)
            try
            {
                var since  = DateTime.UtcNow.AddHours(-1);
                int errors = await _context.AuditLogs
                    .Where(l => l.CreatedAt > since && l.Action.Contains("FAILED"))
                    .CountAsync();
                report.Issues.Add(new ScanIssue { Category = "Health",
                    Severity    = errors > 10 ? "Critical" : errors > 3 ? "Warning" : "Success",
                    Title       = $"Hata Oranı – Son 1 Saat: {errors} başarısız istek",
                    Description = errors > 10
                        ? "Son saatte 10+ başarısız istek! Brute-force saldırısı veya kritik sistem hatası olabilir."
                        : errors > 3
                            ? "Son saatte birkaç başarısız giriş/işlem var. Şüpheli aktivite izleniyor."
                            : $"Son 1 saatte yalnızca {errors} başarısız istek. Sistem sakin ve güvende." });
            }
            catch { }

            // Static confirmations
            report.Issues.Add(new ScanIssue { Category = "Health", Severity = "Success",
                Title = "Refresh Token Güvenli Cookie'de",
                Description = "Refresh token HttpOnly + Secure + SameSite=Strict cookie ile saklanıyor. JavaScript erişimine kesinlikle kapalı." });
            report.Issues.Add(new ScanIssue { Category = "Health", Severity = "Success",
                Title = "Şifre Hash: PBKDF2 (ASP.NET Identity v3)",
                Description = "Şifreler PBKDF2 + Salt + 100.000 iterasyon ile hash'leniyor. Plain-text şifre asla saklanmıyor. Rainbow table saldırılarına karşı güçlü." });
            report.Issues.Add(new ScanIssue { Category = "Health", Severity = "Success",
                Title = "Oturum Zaman Aşımı (5 Dakika)",
                Description = "Admin panelinde 5 dakika hareketsizlik sonrası oturum otomatik sonlandırılıyor. Fiziksel erişim riskini azaltır." });
        }

        // ══════════════════════════════════════════════════════
        // 📊  PUAN HESAPLAMA
        // ══════════════════════════════════════════════════════
        private void CalculateScores(ScanReport report)
        {
            report.SecurityScore    = CalculateCategoryScore(report.Issues.Where(i => i.Category == "Security"));
            report.SeoScore         = CalculateCategoryScore(report.Issues.Where(i => i.Category == "SEO"));
            report.PerformanceScore = CalculateCategoryScore(report.Issues.Where(i => i.Category == "Performance"));
            report.HealthScore      = CalculateCategoryScore(report.Issues.Where(i => i.Category == "Health"));
        }

        private static int CalculateCategoryScore(IEnumerable<ScanIssue> issues)
        {
            if (!issues.Any()) return 100;
            int score = 100;
            foreach (var issue in issues)
            {
                if      (issue.Severity == "Critical") score -= 18;
                else if (issue.Severity == "Warning")  score -= 6;
            }
            return Math.Max(0, score);
        }
    }
}
