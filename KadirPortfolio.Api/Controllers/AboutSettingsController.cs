using KadirPortfolio.Api.Data;
using KadirPortfolio.Api.Models;
using KadirPortfolio.Api.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KadirPortfolio.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AboutSettingsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AboutSettingsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<AboutSetting>> GetAboutSetting()
        {
            var setting = await _context.AboutSettings.Include(a => a.Cards.Where(c => !c.IsDeleted)).FirstOrDefaultAsync();
            if (setting == null)
            {
                return NotFound();
            }
            return setting;
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAboutSetting(UpdateAboutSettingDto dto)
        {
            var existing = await _context.AboutSettings.Include(a => a.Cards.Where(c => !c.IsDeleted)).FirstOrDefaultAsync();
            if (existing == null)
            {
                var newSetting = new AboutSetting
                {
                    MainTitle = dto.MainTitle,
                    MainTitleEn = dto.MainTitleEn,
                    SubTitle = dto.SubTitle,
                    SubTitleEn = dto.SubTitleEn,
                    ProfileImageUrl = dto.ProfileImageUrl,
                    CardTitle = dto.CardTitle,
                    CardTitleEn = dto.CardTitleEn,
                    CardSubtitle = dto.CardSubtitle,
                    CardSubtitleEn = dto.CardSubtitleEn,
                    Bio = dto.Bio,
                    BioEn = dto.BioEn,
                    IsLookingForJob = dto.IsLookingForJob
                };

                if (dto.Cards != null)
                {
                    foreach (var cardDto in dto.Cards)
                    {
                        newSetting.Cards.Add(new AboutCard
                        {
                            CardType = cardDto.CardType,
                            Icon = cardDto.Icon,
                            Title = cardDto.Title,
                            TitleEn = cardDto.TitleEn,
                            Text = cardDto.Text,
                            TextEn = cardDto.TextEn,
                            ListItems = cardDto.ListItems,
                            ListItemsEn = cardDto.ListItemsEn,
                            IsDeleted = cardDto.IsDeleted
                        });
                    }
                }
                _context.AboutSettings.Add(newSetting);
            }
            else
            {
                existing.MainTitle = dto.MainTitle;
                existing.MainTitleEn = dto.MainTitleEn;
                existing.SubTitle = dto.SubTitle;
                existing.SubTitleEn = dto.SubTitleEn;
                existing.ProfileImageUrl = dto.ProfileImageUrl;
                existing.CardTitle = dto.CardTitle;
                existing.CardTitleEn = dto.CardTitleEn;
                existing.CardSubtitle = dto.CardSubtitle;
                existing.CardSubtitleEn = dto.CardSubtitleEn;
                existing.Bio = dto.Bio;
                existing.BioEn = dto.BioEn;
                existing.IsLookingForJob = dto.IsLookingForJob;

                if (dto.Cards != null)
                {
                    foreach (var cardDto in dto.Cards)
                    {
                        if (cardDto.Id == 0)
                        {
                            _context.AboutCards.Add(new AboutCard
                            {
                                AboutSettingId = existing.Id,
                                CardType = cardDto.CardType,
                                Icon = cardDto.Icon,
                                Title = cardDto.Title,
                                TitleEn = cardDto.TitleEn,
                                Text = cardDto.Text,
                                TextEn = cardDto.TextEn,
                                ListItems = cardDto.ListItems,
                                ListItemsEn = cardDto.ListItemsEn,
                                IsDeleted = cardDto.IsDeleted
                            });
                        }
                        else
                        {
                            var existingCard = existing.Cards.FirstOrDefault(c => c.Id == cardDto.Id);
                            if (existingCard != null)
                            {
                                existingCard.CardType = cardDto.CardType;
                                existingCard.Icon = cardDto.Icon;
                                existingCard.Title = cardDto.Title;
                                existingCard.TitleEn = cardDto.TitleEn;
                                existingCard.Text = cardDto.Text;
                                existingCard.TextEn = cardDto.TextEn;
                                existingCard.ListItems = cardDto.ListItems;
                                existingCard.ListItemsEn = cardDto.ListItemsEn;
                                existingCard.IsDeleted = cardDto.IsDeleted;
                            }
                        }
                    }
                }

                _context.AboutSettings.Update(existing);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return BadRequest("Failed to update about settings.");
            }

            return NoContent();
        }

        [HttpPost("cards")]
        public async Task<ActionResult<AboutCard>> PostAboutCard(AboutCard card)
        {
            _context.AboutCards.Add(card);
            await _context.SaveChangesAsync();
            return Ok(card);
        }

        [HttpDelete("cards/{id}")]
        public async Task<IActionResult> DeleteAboutCard(int id)
        {
            var card = await _context.AboutCards.FindAsync(id);
            if (card == null) return NotFound();
            
            card.IsDeleted = true;
            _context.Entry(card).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("cards/trash")]
        public async Task<ActionResult<IEnumerable<AboutCard>>> GetDeletedAboutCards()
        {
            var cards = await _context.AboutCards.Where(c => c.IsDeleted).ToListAsync();
            return Ok(cards);
        }

        [HttpPost("cards/{id}/restore")]
        public async Task<IActionResult> RestoreAboutCard(int id)
        {
            var card = await _context.AboutCards.FindAsync(id);
            if (card == null || !card.IsDeleted) return NotFound();

            card.IsDeleted = false;
            _context.Entry(card).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("cards/{id}/hard")]
        public async Task<IActionResult> HardDeleteAboutCard(int id)
        {
            var card = await _context.AboutCards.FindAsync(id);
            if (card == null) return NotFound();

            _context.AboutCards.Remove(card);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
