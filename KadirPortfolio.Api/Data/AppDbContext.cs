using Microsoft.EntityFrameworkCore;
using KadirPortfolio.Api.Models;

namespace KadirPortfolio.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<HomeSetting> HomeSettings { get; set; }
        public DbSet<AboutSetting> AboutSettings { get; set; }
        public DbSet<AboutCard> AboutCards { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<BlogCategory> BlogCategories { get; set; }
        public DbSet<SkillCategory> SkillCategories { get; set; }
        public DbSet<SkillItem> SkillItems { get; set; }
        public DbSet<ProjectCategory> ProjectCategories { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<IletisimMesaji> IletisimMesajlari { get; set; }
        public DbSet<AnalyticsData> Analytics { get; set; }
        public DbSet<SystemHealthLog> SystemHealthLogs { get; set; }
        public DbSet<SeoSetting> SeoSettings { get; set; }
        public DbSet<ContactCard> ContactCards { get; set; }
        public DbSet<TranslationMemory> TranslationMemories { get; set; }
        public DbSet<ApiKeyConfig> ApiKeyConfigs { get; set; }
        public DbSet<GlossaryItem> GlossaryItems { get; set; }
        
        public DbSet<AdminUser> AdminUsers { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<AdminDevice> AdminDevices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
