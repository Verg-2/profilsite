using KadirPortfolio.Api.Data;
using KadirPortfolio.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KadirPortfolio.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class HomeSettingsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public HomeSettingsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<HomeSetting>> GetHomeSetting()
        {
            var setting = await _context.HomeSettings.FirstOrDefaultAsync();
            if (setting == null)
            {
                return NotFound();
            }
            return setting;
        }

        [HttpPut]
        public async Task<IActionResult> UpdateHomeSetting(HomeSetting homeSetting)
        {
            var existing = await _context.HomeSettings.FirstOrDefaultAsync();
            if (existing == null)
            {
                _context.HomeSettings.Add(homeSetting);
            }
            else
            {
                existing.HeroTitle = homeSetting.HeroTitle;
                existing.HeroTitleEn = homeSetting.HeroTitleEn;
                existing.HeroSubtitle = homeSetting.HeroSubtitle;
                existing.HeroSubtitleEn = homeSetting.HeroSubtitleEn;
                existing.ProfileImageUrl = homeSetting.ProfileImageUrl;
                existing.PreTitle = homeSetting.PreTitle;
                existing.PreTitleEn = homeSetting.PreTitleEn;
                existing.ButtonText = homeSetting.ButtonText;
                existing.ButtonTextEn = homeSetting.ButtonTextEn;
                existing.ButtonUrl = homeSetting.ButtonUrl;
                existing.SecondaryButtonText = homeSetting.SecondaryButtonText;
                existing.SecondaryButtonTextEn = homeSetting.SecondaryButtonTextEn;
                existing.SecondaryButtonUrl = homeSetting.SecondaryButtonUrl;
                existing.LightCursor = homeSetting.LightCursor;
                existing.DarkCursor = homeSetting.DarkCursor;
                existing.Model3DUrl = homeSetting.Model3DUrl;
                existing.Model3DUrlLight = homeSetting.Model3DUrlLight;
                _context.HomeSettings.Update(existing);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return BadRequest("Failed to update home settings.");
            }

            return NoContent();
        }
    }
}
