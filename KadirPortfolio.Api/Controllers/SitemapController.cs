using KadirPortfolio.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace KadirPortfolio.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SitemapController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly string _baseUrl = "https://kadir.com"; // Gerçek domain ile değiştirilmeli

        public SitemapController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Produces("application/xml")]
        public async Task<IActionResult> GetSitemap()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

            // Statik Sayfalar
            var staticRoutes = new[] { "", "hakkinda", "blog", "projects", "yetenekler", "contact" };
            foreach (var route in staticRoutes)
            {
                var loc = string.IsNullOrEmpty(route) ? _baseUrl : $"{_baseUrl}/{route}";
                sb.AppendLine("  <url>");
                sb.AppendLine($"    <loc>{loc}</loc>");
                sb.AppendLine("    <changefreq>weekly</changefreq>");
                sb.AppendLine(route == "" ? "    <priority>1.0</priority>" : "    <priority>0.8</priority>");
                sb.AppendLine("  </url>");
            }

            // Dinamik Projeler
            var projects = await _context.Projects.Select(p => p.Id).ToListAsync();
            foreach (var projectId in projects)
            {
                sb.AppendLine("  <url>");
                sb.AppendLine($"    <loc>{_baseUrl}/projects/{projectId}</loc>");
                sb.AppendLine("    <changefreq>monthly</changefreq>");
                sb.AppendLine("    <priority>0.7</priority>");
                sb.AppendLine("  </url>");
            }

            // Dinamik Blog Yazıları
            var blogs = await _context.BlogPosts.Select(b => b.Id).ToListAsync();
            foreach (var blogId in blogs)
            {
                sb.AppendLine("  <url>");
                sb.AppendLine($"    <loc>{_baseUrl}/blog/{blogId}</loc>");
                sb.AppendLine("    <changefreq>monthly</changefreq>");
                sb.AppendLine("    <priority>0.7</priority>");
                sb.AppendLine("  </url>");
            }

            sb.AppendLine("</urlset>");

            return Content(sb.ToString(), "application/xml", Encoding.UTF8);
        }
    }
}
