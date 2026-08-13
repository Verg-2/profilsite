using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection;
using System.Collections;

namespace KadirPortfolio.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CacheController : ControllerBase
    {
        private readonly IMemoryCache _memoryCache;

        public CacheController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        [Authorize]
        [HttpPost("clear")]
        public IActionResult ClearCache()
        {
            // .NET Core'da IMemoryCache'in tamamını temizlemenin resmi bir metodu yoktur.
            // Sadece bilinen anahtarları silebilir veya reflection ile zorla temizleyebiliriz.
            // Biz burada sadece bilinen anahtarları siliyoruz.
            _memoryCache.Remove("Projects_All");
            _memoryCache.Remove("Home_Settings");
            _memoryCache.Remove("About_Settings");

            return Ok(new { success = true, message = "Önbellek (Cache) başarıyla temizlendi." });
        }
    }
}
