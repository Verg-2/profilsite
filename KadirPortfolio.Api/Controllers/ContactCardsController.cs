using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KadirPortfolio.Api.Data;
using KadirPortfolio.Api.Models;

namespace KadirPortfolio.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactCardsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ContactCardsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/ContactCards
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContactCard>>> GetContactCards()
        {
            return await _context.ContactCards.OrderBy(c => c.OrderIndex).ToListAsync();
        }

        // GET: api/ContactCards/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ContactCard>> GetContactCard(int id)
        {
            var contactCard = await _context.ContactCards.FindAsync(id);

            if (contactCard == null)
            {
                return NotFound();
            }

            return contactCard;
        }

        // PUT: api/ContactCards/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContactCard(int id, ContactCard contactCard)
        {
            if (id != contactCard.Id)
            {
                return BadRequest();
            }

            _context.Entry(contactCard).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactCardExists(id))
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

        // POST: api/ContactCards
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ContactCard>> PostContactCard(ContactCard contactCard)
        {
            _context.ContactCards.Add(contactCard);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetContactCard), new { id = contactCard.Id }, contactCard);
        }

        // DELETE: api/ContactCards/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContactCard(int id)
        {
            var contactCard = await _context.ContactCards.FindAsync(id);
            if (contactCard == null)
            {
                return NotFound();
            }

            _context.ContactCards.Remove(contactCard);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ContactCardExists(int id)
        {
            return _context.ContactCards.Any(e => e.Id == id);
        }
    }
}
