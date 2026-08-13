using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KadirPortfolio.Api.Data;
using KadirPortfolio.Api.Models;
using Microsoft.AspNetCore.Authorization;

namespace KadirPortfolio.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GlossaryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GlossaryController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GlossaryItem>>> GetGlossaryItems()
        {
            return await _context.GlossaryItems.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<GlossaryItem>> CreateGlossaryItem(GlossaryItem item)
        {
            _context.GlossaryItems.Add(item);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetGlossaryItems), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGlossaryItem(int id, GlossaryItem item)
        {
            if (id != item.Id)
            {
                return BadRequest();
            }

            _context.Entry(item).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GlossaryItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGlossaryItem(int id)
        {
            var item = await _context.GlossaryItems.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            _context.GlossaryItems.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GlossaryItemExists(int id)
        {
            return _context.GlossaryItems.Any(e => e.Id == id);
        }
    }
}
