using KadirPortfolio.Api.Data;
using KadirPortfolio.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KadirPortfolio.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SkillsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SkillsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SkillCategory>>> GetSkillCategories()
        {
            return await _context.SkillCategories.Include(s => s.Skills.Where(item => !item.IsDeleted)).ToListAsync();
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<SkillCategory>> PostSkillCategory(SkillCategory category)
        {
            _context.SkillCategories.Add(category);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetSkillCategories), new { id = category.Id }, category);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSkillCategory(int id, SkillCategory category)
        {
            if (id != category.Id) return BadRequest();
            _context.Entry(category).State = EntityState.Modified;
            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.SkillCategories.Any(e => e.Id == id)) return NotFound();
                else throw;
            }
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSkillCategory(int id)
        {
            var category = await _context.SkillCategories.FindAsync(id);
            if (category == null) return NotFound();
            _context.SkillCategories.Remove(category);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        
        [Authorize]
        [HttpPost("items")]
        public async Task<ActionResult<SkillItem>> PostSkillItem(SkillItem item)
        {
            _context.SkillItems.Add(item);
            await _context.SaveChangesAsync();
            return Ok(item);
        }

        [Authorize]
        [HttpPut("items/{id}")]
        public async Task<IActionResult> PutSkillItem(int id, SkillItem item)
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
                if (!SkillItemExists(id))
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

        private bool SkillItemExists(int id)
        {
            return _context.SkillItems.Any(e => e.Id == id);
        }

        [Authorize]
        [HttpDelete("items/{id}")]
        public async Task<IActionResult> DeleteSkillItem(int id)
        {
            var item = await _context.SkillItems.FindAsync(id);
            if (item == null) return NotFound();
            
            item.IsDeleted = true;
            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [Authorize]
        [HttpGet("items/trash")]
        public async Task<ActionResult<IEnumerable<SkillItem>>> GetDeletedSkillItems()
        {
            var items = await _context.SkillItems.Where(i => i.IsDeleted).ToListAsync();
            return Ok(items);
        }

        [Authorize]
        [HttpPost("items/{id}/restore")]
        public async Task<IActionResult> RestoreSkillItem(int id)
        {
            var item = await _context.SkillItems.FindAsync(id);
            if (item == null || !item.IsDeleted) return NotFound();

            item.IsDeleted = false;
            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [Authorize]
        [HttpDelete("items/{id}/hard")]
        public async Task<IActionResult> HardDeleteSkillItem(int id)
        {
            var item = await _context.SkillItems.FindAsync(id);
            if (item == null) return NotFound();

            _context.SkillItems.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
