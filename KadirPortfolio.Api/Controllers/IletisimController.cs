using KadirPortfolio.Api.Models;
using KadirPortfolio.Api.Services;
using KadirPortfolio.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

namespace KadirPortfolio.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class IletisimController : ControllerBase
    {
        private readonly ITelegramService _telegramService;
        private readonly ILogger<IletisimController> _logger;
        private readonly AppDbContext _context;

        public IletisimController(
            ITelegramService telegramService,
            ILogger<IletisimController> logger,
            AppDbContext context)
        {
            _telegramService = telegramService;
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetMesajlar()
        {
            var mesajlar = await _context.IletisimMesajlari.Where(m => !m.IsDeleted).OrderByDescending(m => m.GonderimTarihi).ToListAsync();
            return Ok(mesajlar);
        }

        [EnableRateLimiting("IletisimLimiti")]
        [HttpPost("gonder")]
        [AllowAnonymous]
        public async Task<IActionResult> MesajGonder([FromBody] IletisimMesaji model)
        {
            if (!ModelState.IsValid)
            {
                var hatalar = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new
                {
                    success = false,
                    mesaj = "Girilen bilgiler hatalı.",
                    hatalar
                });
            }

            if (!string.IsNullOrWhiteSpace(model.WebSitesi))
            {
                _logger.LogWarning("Bot engellendi. Honeypot alanı dolu.");
                return Ok(new
                {
                    success = true,
                    mesaj = "Güvenlik kontrolü başarılı."
                });
            }

            try
            {
                // Veritabanına kaydet
                model.GonderimTarihi = DateTime.UtcNow;
                _context.IletisimMesajlari.Add(model);
                await _context.SaveChangesAsync();

                var referansNo = await _telegramService.MesajGonderAsync(model);

                return Ok(new
                {
                    success = true,
                    mesaj = "Mesajınız başarıyla iletildi.",
                    referansNo
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    mesaj = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Yapılandırma hatası.");
                return StatusCode(500, new
                {
                    success = false,
                    mesaj = "Sistem yapılandırmasında bir sorun var."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İletişim mesajı gönderilirken hata oluştu.");
                return StatusCode(500, new
                {
                    success = false,
                    mesaj = "Sistemde beklenmeyen bir hata oluştu. Lütfen daha sonra tekrar deneyin."
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMesaj(int id)
        {
            var mesaj = await _context.IletisimMesajlari.FindAsync(id);
            if (mesaj == null) return NotFound();

            mesaj.IsDeleted = true;
            _context.Entry(mesaj).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("trash")]
        public async Task<ActionResult<IEnumerable<IletisimMesaji>>> GetDeletedMesajlar()
        {
            var mesajlar = await _context.IletisimMesajlari.Where(m => m.IsDeleted).OrderByDescending(m => m.GonderimTarihi).ToListAsync();
            return Ok(mesajlar);
        }

        [HttpPost("{id}/restore")]
        public async Task<IActionResult> RestoreMesaj(int id)
        {
            var mesaj = await _context.IletisimMesajlari.FindAsync(id);
            if (mesaj == null || !mesaj.IsDeleted) return NotFound();

            mesaj.IsDeleted = false;
            _context.Entry(mesaj).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}/hard")]
        public async Task<IActionResult> HardDeleteMesaj(int id)
        {
            var mesaj = await _context.IletisimMesajlari.FindAsync(id);
            if (mesaj == null) return NotFound();

            _context.IletisimMesajlari.Remove(mesaj);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}