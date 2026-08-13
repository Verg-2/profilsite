using KadirPortfolio.Api.Models;
using KadirPortfolio.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KadirPortfolio.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SystemHealthController : ControllerBase
    {
        private readonly IScannerService _scannerService;
        private readonly ILogger<SystemHealthController> _logger;

        public SystemHealthController(IScannerService scannerService, ILogger<SystemHealthController> logger)
        {
            _scannerService = scannerService;
            _logger = logger;
        }

        [HttpGet("scan")]
        public async Task<ActionResult<ScanReport>> RunScan()
        {
            var report = await _scannerService.RunFullScanAsync();
            return Ok(report);
        }

        [HttpGet("source-files")]
        public IActionResult GetSourceFiles()
        {
            try
            {
                var files = new List<object>();
                var backendPath = Directory.GetCurrentDirectory();
                var frontendPath = Path.Combine(Directory.GetParent(backendPath)?.FullName ?? "", "vue-proje", "src");

                if (Directory.Exists(backendPath))
                {
                    var csFiles = Directory.GetFiles(backendPath, "*.cs", SearchOption.AllDirectories)
                        .Where(f => !f.Contains("\\obj\\") && !f.Contains("\\bin\\") && !f.Contains("\\Migrations\\"));
                    foreach(var f in csFiles) files.Add(new { path = f, name = "[Backend] " + Path.GetFileName(f) });
                }

                if (Directory.Exists(frontendPath))
                {
                    var vueFiles = Directory.GetFiles(frontendPath, "*.*", SearchOption.AllDirectories)
                        .Where(f => f.EndsWith(".vue") || f.EndsWith(".js"));
                    foreach(var f in vueFiles) files.Add(new { path = f, name = "[Frontend] " + Path.GetFileName(f) });
                }

                return Ok(files);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Dosya listesi alınırken hata oluştu.");
                return StatusCode(500, new { success = false, message = "Sistem hatası oluştu. İşlem şu anda gerçekleştirilemiyor." });
            }
        }

        public class CodeAnalysisRequest
        {
            public string Code { get; set; } = string.Empty;
        }

        [HttpPost("analyze-code")]
        public async Task<IActionResult> AnalyzeCode([FromBody] CodeAnalysisRequest request, [FromServices] IAiTranslationService aiService)
        {
            if (string.IsNullOrWhiteSpace(request.Code))
            {
                return BadRequest("Kod alanı boş olamaz.");
            }

            try
            {
                var result = await aiService.AnalyzeCodeSecurityAsync(request.Code);
                return Ok(new { success = true, report = result });
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Kod analizi sırasında hata oluştu.");
                return StatusCode(500, new { success = false, message = "Sistem hatası oluştu. İşlem şu anda gerçekleştirilemiyor." });
            }
        }

        public class FileAnalysisRequest
        {
            public string FilePath { get; set; } = string.Empty;
        }

        [HttpPost("analyze-file")]
        public async Task<IActionResult> AnalyzeFile([FromBody] FileAnalysisRequest request, [FromServices] IAiTranslationService aiService)
        {
            if (string.IsNullOrWhiteSpace(request.FilePath))
            {
                return BadRequest(new { success = false, message = "Dosya yolu boş olamaz." });
            }

            string backendDir = System.IO.Path.GetFullPath(System.IO.Directory.GetCurrentDirectory());
            string frontendDir = System.IO.Path.GetFullPath(System.IO.Path.Combine(backendDir, "..", "vue-proje", "src"));

            string fullPath = System.IO.Path.GetFullPath(request.FilePath);

            bool isInsideBackend = fullPath.StartsWith(backendDir + System.IO.Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
            bool isInsideFrontend = fullPath.StartsWith(frontendDir + System.IO.Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);

            if (!isInsideBackend && !isInsideFrontend)
            {
                return BadRequest(new { success = false, message = "Yetkisiz dizin erişimi engellendi." });
            }

            // GÜVENLİK ÖNLEMİ: Kritik dosyaları ve izin verilmeyen uzantıları engelle
            string fileName = System.IO.Path.GetFileName(fullPath).ToLowerInvariant();
            string extension = System.IO.Path.GetExtension(fullPath).ToLowerInvariant();
            var allowedExtensions = new[] { ".cs", ".vue", ".js" };

            if (fileName == ".env" || fileName == "appsettings.json" || fileName.Contains("config") || !System.Linq.Enumerable.Contains(allowedExtensions, extension))
            {
                return BadRequest(new { success = false, message = "Bu dosya tipini analiz etmeye yetkiniz yok!" });
            }

            if (!System.IO.File.Exists(fullPath))
            {
                return BadRequest(new { success = false, message = "Dosya bulunamadı." });
            }

            try
            {
                var code = await System.IO.File.ReadAllTextAsync(fullPath);
                // Eğer dosya çok büyükse engelle (Örn: 200KB'dan büyükse)
                if (code.Length > 200000) return BadRequest(new { success = false, message = "Bu dosya yapay zeka analizi için çok büyük!" });

                var result = await aiService.AnalyzeCodeSecurityAsync(code);
                return Ok(new { success = true, report = result });
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Dosya analizi sırasında hata oluştu.");
                return StatusCode(500, new { success = false, message = "Sistem hatası oluştu. İşlem şu anda gerçekleştirilemiyor." });
            }
        }

        [HttpPost("analyze-project")]
        public async Task<IActionResult> AnalyzeProject([FromServices] IAiTranslationService aiService)
        {
            try
            {
                var backendPath = Directory.GetCurrentDirectory();
                var frontendPath = Path.Combine(Directory.GetParent(backendPath)?.FullName ?? "", "vue-proje", "src");
                
                var codeBuilder = new System.Text.StringBuilder();
                codeBuilder.AppendLine("Aşağıda tüm projenin kaynak kodları (Backend ve Frontend) tek bir paket halinde verilmiştir.");
                codeBuilder.AppendLine("Amacınız bu projeyi bir siber güvenlik uzmanı gibi UÇTAN UCA statik analize (SAST) tabi tutmaktır.");
                codeBuilder.AppendLine("Özellikle bir dosyadan çıkan verinin diğer bir dosyada yarattığı güvenlik zafiyetlerine (Zincirleme zafiyetler) odaklanın.");
                codeBuilder.AppendLine("========================================================\n");

                // Get Backend Files
                if (Directory.Exists(backendPath))
                {
                    var csFiles = Directory.GetFiles(backendPath, "*.cs", SearchOption.AllDirectories)
                        .Where(f => !f.Contains("\\obj\\") && !f.Contains("\\bin\\") && !f.Contains("\\Migrations\\") && !f.Contains("\\Properties\\"));
                    
                    foreach(var f in csFiles)
                    {
                        var content = await System.IO.File.ReadAllTextAsync(f);
                        codeBuilder.AppendLine($"\n--- DOSYA: [Backend] {Path.GetFileName(f)} ---");
                        codeBuilder.AppendLine(content);
                    }
                }

                // Get Frontend Files
                if (Directory.Exists(frontendPath))
                {
                    var vueFiles = Directory.GetFiles(frontendPath, "*.*", SearchOption.AllDirectories)
                        .Where(f => (f.EndsWith(".vue") || f.EndsWith(".js")) && !f.Contains("\\assets\\"));
                    
                    foreach(var f in vueFiles)
                    {
                        var content = await System.IO.File.ReadAllTextAsync(f);
                        codeBuilder.AppendLine($"\n--- DOSYA: [Frontend] {Path.GetFileName(f)} ---");
                        codeBuilder.AppendLine(content);
                    }
                }

                var fullCode = codeBuilder.ToString();
                
                // Kapasite güvenlik kontrolü (Gemini 1.5 Flash çok yüksek kapasiteli ama yine de sunucu belleğini koruyalım, maks ~3MB)
                if (fullCode.Length > 3000000) 
                {
                    fullCode = fullCode.Substring(0, 3000000) + "\n\n[UYARI: PROJE KODU ÇOK BÜYÜK OLDUĞU İÇİN KESİLDİ.]";
                }

                var result = await aiService.AnalyzeCodeSecurityAsync(fullCode);
                return Ok(new { success = true, report = result });
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Tüm proje analizi sırasında hata oluştu.");
                return StatusCode(500, new { success = false, message = "Sistem hatası oluştu. İşlem şu anda gerçekleştirilemiyor." });
            }
        }
    }
}
