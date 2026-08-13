using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using KadirPortfolio.Api.Data;
using KadirPortfolio.Api.Models;
using KadirPortfolio.Api.Services;

namespace KadirPortfolio.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Sadece admin erişebilir
    public class ApiKeysController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IEncryptionService _encryptionService;

        public ApiKeysController(AppDbContext context, IEncryptionService encryptionService)
        {
            _context = context;
            _encryptionService = encryptionService;
        }

        // GET: api/ApiKeys
        [HttpGet]
        public async Task<IActionResult> GetApiKeys()
        {
            var keys = await _context.ApiKeyConfigs
                .Select(k => new
                {
                    k.Id,
                    k.Alias,
                    k.AssignedTask,
                    k.Provider,
                    k.BaseUrl,
                    k.ModelName,
                    k.IsActive,
                    k.RequestCount,
                    k.TotalTokensUsed,
                    k.LastUsedDate,
                    k.LastError,
                    k.LastErrorDate,
                    k.CreatedAt
                })
                .OrderByDescending(k => k.CreatedAt)
                .ToListAsync();

            return Ok(new { success = true, data = keys });
        }

        public class ApiKeyDto
        {
            public string Alias { get; set; } = string.Empty;
            public string KeyValue { get; set; } = string.Empty;
            public string AssignedTask { get; set; } = "Genel";
            public string Provider { get; set; } = "Google";
            public string? BaseUrl { get; set; }
            public string? ModelName { get; set; }
        }

        // POST: api/ApiKeys
        [HttpPost]
        public async Task<IActionResult> AddApiKey([FromBody] ApiKeyDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Alias) || string.IsNullOrWhiteSpace(request.KeyValue))
            {
                return BadRequest(new { success = false, message = "Alias ve KeyValue zorunludur." });
            }

            // Şifreleme işlemi
            var (cipherText, iv) = _encryptionService.Encrypt(request.KeyValue);

            var newKey = new ApiKeyConfig
            {
                Alias = request.Alias,
                KeyValue = cipherText,
                IV = iv,
                AssignedTask = request.AssignedTask,
                Provider = request.Provider,
                BaseUrl = request.BaseUrl,
                ModelName = request.ModelName,
                IsActive = true
            };

            _context.ApiKeyConfigs.Add(newKey);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "API anahtarı başarıyla eklendi." });
        }

        // PUT: api/ApiKeys/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateApiKey(int id, [FromBody] ApiKeyDto request)
        {
            var key = await _context.ApiKeyConfigs.FindAsync(id);
            if (key == null) return NotFound(new { success = false, message = "API anahtarı bulunamadı." });

            key.Alias = request.Alias;
            key.AssignedTask = request.AssignedTask;
            key.Provider = request.Provider;
            key.BaseUrl = request.BaseUrl;
            key.ModelName = request.ModelName;

            if (!string.IsNullOrWhiteSpace(request.KeyValue))
            {
                // Sadece yeni bir şifre girildiyse şifreyi güncelle
                var (cipherText, iv) = _encryptionService.Encrypt(request.KeyValue);
                key.KeyValue = cipherText;
                key.IV = iv;
            }

            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "API anahtarı başarıyla güncellendi." });
        }
        
        [HttpPut("{id}/toggle")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var key = await _context.ApiKeyConfigs.FindAsync(id);
            if (key == null) return NotFound(new { success = false, message = "API anahtarı bulunamadı." });

            key.IsActive = !key.IsActive;
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "API anahtarı durumu değiştirildi." });
        }

        // DELETE: api/ApiKeys/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApiKey(int id)
        {
            var key = await _context.ApiKeyConfigs.FindAsync(id);
            if (key == null) return NotFound(new { success = false, message = "API anahtarı bulunamadı." });

            _context.ApiKeyConfigs.Remove(key);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "API anahtarı başarıyla silindi." });
        }
    }
}
