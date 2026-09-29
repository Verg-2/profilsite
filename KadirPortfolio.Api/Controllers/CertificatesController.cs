using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KadirPortfolio.Api.Data;
using KadirPortfolio.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Polly;
namespace KadirPortfolio.Api.Controllers
{
  [Route("api/[controller]")]    [ApiController]
    public class CertificatesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CertificatesController(AppDbContext context)
        {
            _context = context;
        }
          // GET: api/Certificates (Herkes görebilir)
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Certificate>>>GetCertificates()
        {
            return await _context.Certificates.OrderBy(c=>c.Order).ToListAsync();
        }
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Certificate>>PostCertificate(Certificate certificate)
    
        {
            _context.Certificates.Add(certificate);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCertificates),new {id=certificate.Id},certificate);

        }
                // DELETE: api/Certificates/5 (Sadece sen/admin silebilir)
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult>  DeleteCertificate(int id)
        {
            var certificate = await
            _context.Certificates.FindAsync(id);
            if (certificate == null) return NotFound();
            _context.Certificates.Remove(certificate);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // PUT: api/Certificates/5 (Sadece sen/admin güncelleyebilir)
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateCertificate(int id, Certificate certificate)
        {
            if (id != certificate.Id)
            {
                return BadRequest(); 
            }

            _context.Entry(certificate).State = EntityState.Modified; 
            await _context.SaveChangesAsync(); 

            return NoContent(); 
        }
    }

}