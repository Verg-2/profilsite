using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using KadirPortfolio.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace KadirPortfolio.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public UploadController(IWebHostEnvironment env)
        {
            _env = env;
        }

        private static readonly Dictionary<string, string> AllowedImageMimeTypes = new()
        {
            { ".jpg", "image/jpeg" },
            { ".jpeg", "image/jpeg" },
            { ".png", "image/png" },
            { ".webp", "image/webp" }
        };

        private static readonly Dictionary<string, string> AllowedMediaMimeTypes = new()
        {
            { ".mp4", "video/mp4" },
            { ".webm", "video/webm" },
            { ".gif", "image/gif" },
            { ".glb", "model/gltf-binary" },
            { ".gltf", "model/gltf+json" }
        };

        private static readonly Dictionary<string, byte[]> MediaSignatures = new()
        {
            { ".mp4", new byte[] { 0x00, 0x00, 0x00, 0x18, 0x66, 0x74, 0x79, 0x70 } }, // ftyp
            { ".webm", new byte[] { 0x1A, 0x45, 0xDF, 0xA3 } }, // EBML
            { ".gif", new byte[] { 0x47, 0x49, 0x46, 0x38 } }  // GIF8
        };

        private bool ValidateMediaHeader(IFormFile file, string ext)
        {
            if (!MediaSignatures.ContainsKey(ext)) return true; // Signature unknown, allow it (e.g. .glb)

            using var stream = file.OpenReadStream();
            var signature = MediaSignatures[ext];
            var buffer = new byte[signature.Length];
            stream.Read(buffer, 0, buffer.Length);

            return buffer.SequenceEqual(signature);
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Lütfen bir dosya seçin.");

            if (file.Length > 10 * 1024 * 1024) // 10 MB Sınırı
                return BadRequest("Dosya boyutu çok büyük (Max 10MB).");

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var uploadsFolder = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads");
            
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName = Guid.NewGuid().ToString("N") + ext;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // 1. Resim Dosyası Doğrulama ve WebP'ye Dönüştürme
            if (AllowedImageMimeTypes.ContainsKey(ext))
            {
                if (file.ContentType != AllowedImageMimeTypes[ext])
                    return BadRequest("Geçersiz içerik tipi (MIME mismatch).");

                try
                {
                    using var image = await Image.LoadAsync(file.OpenReadStream());
                    if (image.Width > 1920)
                    {
                        image.Mutate(x => x.Resize(new ResizeOptions
                        {
                            Size = new Size(1920, 0),
                            Mode = ResizeMode.Max
                        }));
                    }
                    
                    var safeWebpName = Guid.NewGuid().ToString("N") + ".webp";
                    var safeWebpPath = Path.Combine(uploadsFolder, safeWebpName);
                    await image.SaveAsWebpAsync(safeWebpPath, new WebpEncoder { Quality = 60 });
                    
                    return Ok(new { url = $"/uploads/{safeWebpName}" });
                }
                catch (Exception)
                {
                    return BadRequest("Geçersiz veya bozuk resim dosyası.");
                }
            }
            // 2. Diğer Medya Dosyalarını Doğrulama
            else if (AllowedMediaMimeTypes.ContainsKey(ext))
            {
                if (file.ContentType != AllowedMediaMimeTypes[ext])
                    return BadRequest("Geçersiz medya içerik tipi.");

                if (!ValidateMediaHeader(file, ext))
                {
                    return BadRequest("Dosya içeriği belirtilen formatla uyuşmuyor (Sahte Uzantı Tespit Edildi).");
                }

                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await file.CopyToAsync(fileStream);
                }
            }
            else
            {
                return BadRequest("Desteklenmeyen dosya formatı.");
            }

            return Ok(new { url = $"/uploads/{uniqueFileName}" });
        }

        [HttpDelete]
        public IActionResult DeleteImage([FromQuery] string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl)) return BadRequest("Geçersiz URL.");

            var fileName = Path.GetFileName(fileUrl);
            var uploadsFolder = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads");
            var filePath = Path.Combine(uploadsFolder, fileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
                return Ok(new { success = true, message = "Fiziksel dosya silindi." });
            }

            return NotFound("Dosya bulunamadı.");
        }

        [HttpPost("cleanup")]
        public async Task<IActionResult> CleanupOrphanedFiles([FromServices] AppDbContext context, [FromQuery] bool execute = false)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads");
            if (!Directory.Exists(uploadsFolder)) return Ok(new { success = true, deletedCount = 0, files = new List<string>() });

            // 1. Veritabanından tüm aktif dosya linklerini topla
            var validUrls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var homeSettings = await context.HomeSettings.FirstOrDefaultAsync();
            if (homeSettings?.ProfileImageUrl != null) validUrls.Add(Path.GetFileName(homeSettings.ProfileImageUrl));

            var aboutSettings = await context.AboutSettings.FirstOrDefaultAsync();
            if (aboutSettings?.ProfileImageUrl != null) validUrls.Add(Path.GetFileName(aboutSettings.ProfileImageUrl));

            var projects = await context.Projects.ToListAsync();
            foreach (var p in projects)
            {
                if (p.ImageUrls != null) 
                    foreach (var img in p.ImageUrls) validUrls.Add(Path.GetFileName(img));
                if (p.LightImageUrls != null) 
                    foreach (var img in p.LightImageUrls) validUrls.Add(Path.GetFileName(img));
                if (p.DarkImageUrls != null) 
                    foreach (var img in p.DarkImageUrls) validUrls.Add(Path.GetFileName(img));
                
                if (!string.IsNullOrEmpty(p.VideoUrl)) validUrls.Add(Path.GetFileName(p.VideoUrl));
                if (!string.IsNullOrEmpty(p.LightVideoUrl)) validUrls.Add(Path.GetFileName(p.LightVideoUrl));
                if (!string.IsNullOrEmpty(p.DarkVideoUrl)) validUrls.Add(Path.GetFileName(p.DarkVideoUrl));
                if (!string.IsNullOrEmpty(p.Model3DUrl)) validUrls.Add(Path.GetFileName(p.Model3DUrl));
            }

            var blogs = await context.BlogPosts.ToListAsync();
            foreach (var b in blogs)
            {
                if (!string.IsNullOrEmpty(b.CoverImageUrl)) validUrls.Add(Path.GetFileName(b.CoverImageUrl));
            }

            // Frontend taraması için Vue dosyalarının içeriklerini birleştir
            var vueSrcPath = Path.Combine(_env.ContentRootPath, "..", "vue-proje", "src");
            var allFrontendContentBuilder = new System.Text.StringBuilder();
            if (Directory.Exists(vueSrcPath))
            {
                var vueFiles = Directory.GetFiles(vueSrcPath, "*.*", SearchOption.AllDirectories)
                                        .Where(f => f.EndsWith(".vue") || f.EndsWith(".js") || f.EndsWith(".css"));
                foreach (var vf in vueFiles)
                {
                    allFrontendContentBuilder.AppendLine(await System.IO.File.ReadAllTextAsync(vf));
                }
            }
            string allFrontendContent = allFrontendContentBuilder.ToString();

            var allFiles = Directory.GetFiles(uploadsFolder);
            var filesToDelete = new List<string>();

            foreach (var filePath in allFiles)
            {
                var fileName = Path.GetFileName(filePath);
                if (!validUrls.Contains(fileName))
                {
                    // Ayrıca frontend kodlarında statik olarak kullanılıyor mu diye bak
                    if (!string.IsNullOrEmpty(allFrontendContent) && allFrontendContent.Contains(fileName))
                    {
                        continue; // Frontend'de kullanılıyor, silme!
                    }

                    filesToDelete.Add(fileName);
                }
            }

            if (!execute)
            {
                return Ok(new { success = true, files = filesToDelete, count = filesToDelete.Count });
            }

            int deletedCount = 0;
            foreach (var fileName in filesToDelete)
            {
                try
                {
                    var filePath = Path.Combine(uploadsFolder, fileName);
                    System.IO.File.Delete(filePath);
                    deletedCount++;
                }
                catch
                {
                    // Ignore lock errors
                }
            }

            return Ok(new { success = true, deletedCount, message = $"{deletedCount} adet kullanılmayan (çöp) dosya sunucudan temizlendi." });
        }
    }
}
