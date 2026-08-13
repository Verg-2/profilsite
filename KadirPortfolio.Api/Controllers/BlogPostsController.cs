using KadirPortfolio.Api.Data;
using KadirPortfolio.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KadirPortfolio.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlogPostsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BlogPostsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<ActionResult<IEnumerable<BlogPost>>> GetBlogPosts()
        {
            return await _context.BlogPosts.Include(b => b.Category).Where(b => !b.IsDeleted).OrderByDescending(b => b.PublishDate).ToListAsync();
        }

        [HttpGet("{id}")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<ActionResult<BlogPost>> GetBlogPost(int id)
        {
            var blogPost = await _context.BlogPosts.Include(b => b.Category).FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);

            if (blogPost == null)
            {
                return NotFound();
            }

            return blogPost;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<BlogPost>> PostBlogPost(BlogPost blogPost)
        {
            if(blogPost.PublishDate == default)
                blogPost.PublishDate = DateTime.UtcNow;

            _context.BlogPosts.Add(blogPost);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBlogPost), new { id = blogPost.Id }, blogPost);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBlogPost(int id, BlogPost blogPost)
        {
            if (id != blogPost.Id)
            {
                return BadRequest();
            }

            _context.Entry(blogPost).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BlogPostExists(id))
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

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlogPost(int id)
        {
            var blogPost = await _context.BlogPosts.FindAsync(id);
            if (blogPost == null)
            {
                return NotFound();
            }

            blogPost.IsDeleted = true;
            _context.Entry(blogPost).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize]
        [HttpGet("trash")]
        public async Task<ActionResult<IEnumerable<BlogPost>>> GetDeletedBlogPosts()
        {
            var blogPosts = await _context.BlogPosts.Include(b => b.Category).Where(b => b.IsDeleted).OrderByDescending(b => b.PublishDate).ToListAsync();
            return Ok(blogPosts);
        }

        [Authorize]
        [HttpPost("{id}/restore")]
        public async Task<IActionResult> RestoreBlogPost(int id)
        {
            var blogPost = await _context.BlogPosts.FindAsync(id);
            if (blogPost == null || !blogPost.IsDeleted) return NotFound();

            blogPost.IsDeleted = false;
            _context.Entry(blogPost).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}/hard")]
        public async Task<IActionResult> HardDeleteBlogPost(int id)
        {
            var blogPost = await _context.BlogPosts.FindAsync(id);
            if (blogPost == null) return NotFound();

            _context.BlogPosts.Remove(blogPost);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BlogPostExists(int id)
        {
            return _context.BlogPosts.Any(e => e.Id == id);
        }

        // --- Category Management ---

        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<BlogCategory>>> GetBlogCategories()
        {
            return await _context.BlogCategories.ToListAsync();
        }

        [Authorize]
        [HttpPost("categories")]
        public async Task<ActionResult<BlogCategory>> PostBlogCategory(BlogCategory category)
        {
            _context.BlogCategories.Add(category);
            await _context.SaveChangesAsync();
            return Ok(category);
        }

        [Authorize]
        [HttpDelete("categories/{id}")]
        public async Task<IActionResult> DeleteBlogCategory(int id)
        {
            var category = await _context.BlogCategories.FindAsync(id);
            if (category == null) return NotFound();

            _context.BlogCategories.Remove(category);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
