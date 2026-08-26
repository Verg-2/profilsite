using KadirPortfolio.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace KadirPortfolio.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Sadece admin erişebilir
    public class TranslationController : ControllerBase
    {
        private readonly IAiTranslationService _translationService;

        public TranslationController(IAiTranslationService translationService)
        {
            _translationService = translationService;
        }

        public class TranslationRequest
        {
            public string Text { get; set; } = string.Empty;
            public string TargetLanguage { get; set; } = "English";
            public string Section { get; set; } = "Genel";
        }

        [AllowAnonymous]
        [HttpGet("Nuke")]
        public async Task<IActionResult> NukeMemory([FromServices] KadirPortfolio.Api.Data.AppDbContext dbContext)
        {
            var memories = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(dbContext.TranslationMemories);
            dbContext.TranslationMemories.RemoveRange(memories);
            await dbContext.SaveChangesAsync();
            return Ok("Nuked all " + memories.Count + " memories. You can now translate again.");
        }

        [HttpPost("Translate")]
        public async Task<IActionResult> Translate([FromBody] TranslationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Text))
            {
                return BadRequest(new { success = false, message = "Çevrilecek metin boş olamaz." });
            }

            var result = await _translationService.TranslateAsync(request.Text, request.TargetLanguage, request.Section);
            
            if (result.StartsWith("[Çeviri Hatası"))
            {
                return BadRequest(new { success = false, message = result });
            }

            return Ok(new { success = true, translatedText = result });
        }
        public class RefineRequest
        {
            public string Text { get; set; } = string.Empty;
            public string ExistingTranslation { get; set; } = string.Empty;
            public string TargetLanguage { get; set; } = "English";
            public string Section { get; set; } = "Genel";
            public string? UserHint { get; set; }
        }

        [HttpPost("Refine")]
        public async Task<IActionResult> Refine([FromBody] RefineRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Text) || string.IsNullOrWhiteSpace(request.ExistingTranslation))
            {
                return BadRequest(new { success = false, message = "Çevrilecek metin ve mevcut çeviri boş olamaz." });
            }

            var result = await _translationService.RefineTranslationAsync(request.Text, request.ExistingTranslation, request.TargetLanguage, request.Section, request.UserHint);
            
            if (result.StartsWith("[Çeviri Hatası"))
            {
                return BadRequest(new { success = false, message = result });
            }

            return Ok(new { success = true, translatedText = result });
        }
        [HttpGet("DumpKeys")]
        public async Task<IActionResult> DumpKeys([FromServices] KadirPortfolio.Api.Data.AppDbContext dbContext)
        {
            var keys = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(dbContext.ApiKeyConfigs);
            var safeKeys = keys.Select(k => new { k.Id, k.Alias, k.Provider, k.AssignedTask, k.IsActive, k.LastError }).ToList();
            return Ok(safeKeys);
        }
    }
}
