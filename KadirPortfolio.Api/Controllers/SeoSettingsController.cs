using KadirPortfolio.Api.Data;
using KadirPortfolio.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KadirPortfolio.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeoSettingsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SeoSettingsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/SeoSettings/page?route=/contact
        [HttpGet("page")]
        public async Task<ActionResult<SeoSetting>> GetSeoSetting([FromQuery] string route)
        {
            if (string.Equals(route, "GetAll", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Invalid route parameter.");

            route = Uri.UnescapeDataString(route);

            // Normalize route to always start with /
            if (!route.StartsWith("/")) route = "/" + route;

            var setting = await _context.SeoSettings.FirstOrDefaultAsync(s => s.Route == route);
            if (setting == null)
            {
                // Return a default empty setting for the route so frontend doesn't crash
                return Ok(new SeoSetting { Route = route });
            }

            return Ok(setting);
        }

        // GET: api/SeoSettings/GetAll
        [HttpGet("GetAll", Order = -1)]
        public async Task<ActionResult<IEnumerable<SeoSetting>>> GetAllSeoSettings()
        {
            var settings = await _context.SeoSettings.ToListAsync();
            return Ok(settings);
        }

        // POST/PUT: api/SeoSettings
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SaveSeoSetting([FromBody] SeoSetting setting)
        {
            if (string.IsNullOrEmpty(setting.Route))
                return BadRequest("Route is required");

            var existing = await _context.SeoSettings.FirstOrDefaultAsync(s => s.Route == setting.Route);
            
            if (existing != null)
            {
                existing.SeoTitle = setting.SeoTitle ?? "";
                existing.SeoTitleEn = setting.SeoTitleEn ?? "";
                existing.SeoDescription = setting.SeoDescription ?? "";
                existing.SeoDescriptionEn = setting.SeoDescriptionEn ?? "";
                existing.GeoTitle = setting.GeoTitle ?? "";
                existing.GeoTitleEn = setting.GeoTitleEn ?? "";
                existing.GeoDescription = setting.GeoDescription ?? "";
                existing.GeoDescriptionEn = setting.GeoDescriptionEn ?? "";
                existing.Lang = setting.Lang ?? "tr";
                existing.IsVisible = setting.IsVisible;
                
                _context.SeoSettings.Update(existing);
            }
            else
            {
                _context.SeoSettings.Add(setting);
            }

            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "SEO/GEO ayarları başarıyla kaydedildi." });
        }

        // DELETE: api/SeoSettings/page?route=/contact
        [Authorize]
        [HttpDelete("page")]
        public async Task<IActionResult> DeleteSeoSetting([FromQuery] string route)
        {
            route = Uri.UnescapeDataString(route);
            if (!route.StartsWith("/")) route = "/" + route;

            var setting = await _context.SeoSettings.FirstOrDefaultAsync(s => s.Route == route);
            if (setting == null)
            {
                return NotFound("Ayar bulunamadı.");
            }

            _context.SeoSettings.Remove(setting);
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Ayar silindi." });
        }
    }
}
