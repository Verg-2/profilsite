using KadirPortfolio.Api.Data;
using KadirPortfolio.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

namespace KadirPortfolio.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AnalyticsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AnalyticsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("stats")]
        public async Task<ActionResult> GetStats()
        {
            var today = DateTime.UtcNow.Date;
            
            // Gerçek "Tekil Ziyaretçi" (Unique Visitors) sayısını bulmak için IP'ye göre filtrele
            var visitorsTotal = await _context.Analytics
                                        .Select(a => a.VisitorIp)
                                        .Distinct()
                                        .CountAsync();
                                        
            var visitorsToday = await _context.Analytics
                                        .Where(a => a.VisitDate >= today)
                                        .Select(a => a.VisitorIp)
                                        .Distinct()
                                        .CountAsync();

            // Sadece silinmemiş (aktif) projeleri ve mesajları say
            var projectCount = await _context.Projects.CountAsync(p => !p.IsDeleted);
            var messageCount = await _context.IletisimMesajlari.CountAsync(m => !m.IsDeleted);

            return Ok(new
            {
                VisitorsTotal = visitorsTotal,
                VisitorsToday = visitorsToday,
                ProjectCount = projectCount,
                MessageCount = messageCount
            });
        }
        
        [HttpGet("health")]
        public async Task<ActionResult<IEnumerable<SystemHealthLog>>> GetHealthLogs()
        {
            return await _context.SystemHealthLogs.OrderByDescending(h => h.LogDate).Take(100).ToListAsync();
        }

        // Normally, this is called by a middleware when visits happen. 
        [HttpPost("track-visit")]
        [AllowAnonymous]
        [EnableRateLimiting("AuthLimiter")]
        public async Task<IActionResult> TrackVisit()
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            
            // Check if visited recently to avoid spam (optional)
            _context.Analytics.Add(new AnalyticsData { VisitorIp = ip, VisitDate = DateTime.UtcNow, Location = "Unknown" });
            await _context.SaveChangesAsync();
            return Ok();
        }
        
        [HttpPost("log-error")]
        [AllowAnonymous] // Tüm ziyaretçilerden gelen hataları yakalayabilmek için açık
        public async Task<IActionResult> LogError([FromBody] SystemHealthLog log)
        {
            if (log == null || string.IsNullOrWhiteSpace(log.ErrorType))
                return BadRequest();

            // Sanitize input to prevent Stored XSS
            log.ErrorType = System.Net.WebUtility.HtmlEncode(log.ErrorType);
            log.Details = System.Net.WebUtility.HtmlEncode(log.Details);
            log.LogDate = DateTime.UtcNow;

            _context.SystemHealthLogs.Add(log);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
