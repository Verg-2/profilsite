using KadirPortfolio.Api.Data;
using KadirPortfolio.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace KadirPortfolio.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;
        private const string CacheKey = "Projects_All";

        public ProjectsController(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<ProjectCategory>>> GetProjectCategories()
        {
            return await _context.ProjectCategories.ToListAsync();
        }

        [Authorize]
        [HttpPost("categories")]
        public async Task<ActionResult<ProjectCategory>> PostProjectCategory(ProjectCategory category)
        {
            _context.ProjectCategories.Add(category);
            await _context.SaveChangesAsync();
            return Ok(category);
        }

        [Authorize]
        [HttpDelete("categories/{id}")]
        public async Task<IActionResult> DeleteProjectCategory(int id)
        {
            var category = await _context.ProjectCategories.FindAsync(id);
            if (category == null) return NotFound();

            _context.ProjectCategories.Remove(category);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [Authorize]
        [HttpPut("categories/{id}")]
        public async Task<IActionResult> PutProjectCategory(int id, ProjectCategory category)
        {
            if (id != category.Id) return BadRequest();

            _context.Entry(category).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
        {
            if (!_cache.TryGetValue(CacheKey, out List<Project>? projects))
            {
                projects = await _context.Projects.Include(p => p.Category).Where(p => !p.IsDeleted).ToListAsync();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromHours(1))
                    .SetAbsoluteExpiration(TimeSpan.FromHours(24));

                _cache.Set(CacheKey, projects, cacheEntryOptions);
            }

            return Ok(projects);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetProject(int id)
        {
            // Try to get from cache first
            if (_cache.TryGetValue(CacheKey, out List<Project>? cachedProjects))
            {
                var cachedProject = cachedProjects?.FirstOrDefault(p => p.Id == id);
                if (cachedProject != null) return Ok(cachedProject);
            }

            var project = await _context.Projects.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

            if (project == null)
            {
                return NotFound();
            }

            return Ok(project);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Project>> PostProject(Project project)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            _cache.Remove(CacheKey);

            return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProject(int id, Project project)
        {
            Console.WriteLine($"[DEBUG] PutProject called with id={id}, project.Id={project.Id}");
            if (id != project.Id)
            {
                Console.WriteLine("[DEBUG] BadRequest because id != project.Id");
                return BadRequest();
            }

            _context.Entry(project).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _cache.Remove(CacheKey);
                Console.WriteLine("[DEBUG] SaveChangesAsync success");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine($"[DEBUG] DbUpdateConcurrencyException: {ex.Message}");
                if (!ProjectExists(id))
                {
                    Console.WriteLine("[DEBUG] ProjectExists is false. Returning NotFound");
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
        public async Task<IActionResult> DeleteProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            project.IsDeleted = true;
            _context.Entry(project).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            _cache.Remove(CacheKey);

            return NoContent();
        }

        [Authorize]
        [HttpGet("trash")]
        public async Task<ActionResult<IEnumerable<Project>>> GetDeletedProjects()
        {
            var projects = await _context.Projects.Include(p => p.Category).Where(p => p.IsDeleted).ToListAsync();
            return Ok(projects);
        }

        [Authorize]
        [HttpPost("{id}/restore")]
        public async Task<IActionResult> RestoreProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null || !project.IsDeleted) return NotFound();

            project.IsDeleted = false;
            _context.Entry(project).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            _cache.Remove(CacheKey);
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}/hard")]
        public async Task<IActionResult> HardDeleteProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null) return NotFound();

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            _cache.Remove(CacheKey);

            return NoContent();
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }
    }
}
