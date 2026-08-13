using System;
using System.Linq;
using KadirPortfolio.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace KadirPortfolio.Api.Data
{
    public static class DataSeeder
    {
        public static void Seed(AppDbContext context, Microsoft.Extensions.Configuration.IConfiguration config)
        {
            // 0. AdminUser
            if (!context.AdminUsers.Any())
            {
                var adminEmail = config["AdminSettings:DefaultEmail"];
                var adminPassword = config["AdminSettings:DefaultPassword"];

                if (string.IsNullOrEmpty(adminEmail) || string.IsNullOrEmpty(adminPassword))
                {
                    throw new InvalidOperationException("Kritik Hata: Admin varsayılan e-posta ve şifresi tanımlanmadan veritabanı seed edilemez!");
                }

                var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<AdminUser>();
                var admin = new AdminUser
                {
                    Email = adminEmail,
                    CreatedAt = DateTime.UtcNow
                };
                admin.PasswordHash = hasher.HashPassword(admin, adminPassword);
                context.AdminUsers.Add(admin);
            }

            // 1. HomeSettings
            if (!context.HomeSettings.Any())
            {
                context.HomeSettings.Add(new HomeSetting
                {
                    PreTitle = "MERHABA, BEN",
                    HeroTitle = "Kadir",
                    HeroSubtitle = "Backend Developer",
                    ProfileImageUrl = "/img/wolff.png",
                    ButtonText = "Projelerim",
                    ButtonUrl = "/projects",
                    SecondaryButtonText = "İletişim",
                    SecondaryButtonUrl = "/contact"
                });
            }

            // 2. AboutSettings
            if (!context.AboutSettings.Any())
            {
                var aboutSetting = new AboutSetting
                {
                    MainTitle = "Kadir Kimdir?",
                    SubTitle = "Modern, Yaratıcı ve Çözüm Odaklı Bir Geliştirici",
                    Bio = "Yazılım dünyasına adım attığımdan beri sürekli öğreniyor ve üretiyorum. Amacım sadece kod yazmak değil, aynı zamanda kullanıcıların hayatına dokunan, estetik ve performanslı dijital deneyimler yaratmak.",
                    ProfileImageUrl = "/img/wolff.png",
                    CardTitle = "2 Yıldır Sektördeyim",
                    CardSubtitle = "Tam Zamanlı Geliştirici",
                    Cards = new System.Collections.Generic.List<AboutCard>
                    {
                        new AboutCard { CardType = 1, Title = "Neler Yapıyorum?", Icon = "fas fa-code", Text = "Farklı sektörlerdeki müşteriler için özelleştirilmiş web çözümleri, e-ticaret altyapıları ve mobil entegrasyonlar geliştirdim." },
                        new AboutCard { CardType = 1, Title = "Yaklaşımım", Icon = "fas fa-bullseye", Text = "Kullanıcı deneyimini her zaman ön planda tutarak, en son teknolojileri projelerime dahil etmeyi bir prensip haline getirdim." },
                        new AboutCard { CardType = 1, Title = "Hedeflerim", Icon = "fas fa-rocket", Text = "Her gün yeni bir şeyler öğrenmek, yeni trendleri takip etmek ve açık kaynak projelerde yer almak." },
                        new AboutCard { CardType = 1, Title = "Nelerden Hoşlanırım?", Icon = "fas fa-heart", Text = "Kahve eşliğinde yeni teknolojileri keşfetmekten, oyun geliştirmekten ve müzik dinlemekten." },
                        new AboutCard { CardType = 1, Title = "Büyüme Yolculuğum", Icon = "fas fa-chart-line", Text = "Hatalarımdan ders çıkararak, kod incelemelerinde geri bildirim alarak kendimi sürekli geliştiriyorum." },
                        new AboutCard { CardType = 1, Title = "İşbirliği", Icon = "fas fa-handshake", Text = "Takım çalışmasına yatkınım. İletişimi şeffaf ve anlaşılır tutarak ekip içindeki sinerjiyi artırmaya odaklanırım." }
                    }
                };
                context.AboutSettings.Add(aboutSetting);
            }

            // 3. BlogPosts
            if (!context.BlogPosts.Any())
            {
                context.BlogPosts.AddRange(
                    new BlogPost
                    {
                        Title = "Modern Web Geliştirme Trendleri",
                        Summary = "2024'te performans, geliştirici deneyimi ve kullanıcı etkileşimi açısından öne çıkan yaklaşım ve pratikler.",
                        Content = "<p>Web dünyası artık sadece 'çalışıyor' demenin ötesine geçti. Kullanıcılar akıcılık, hız ve tutarlılık bekliyor; bu yüzden geliştirme sürecine performans bir gereksinim olarak dahil edilmeli.</p><p>Component-first yaklaşım; UI parçalarını bağımsız düşünmeyi, tekrar kullanılabilir hale getirmeyi ve uzun vadede bakım maliyetini ciddi şekilde düşürmeyi sağlar.</p><p>Animasyon tarafında hedef 'gösterişli' olmaktan çok 'maliyet/etki dengesi' kurmak olmalı. GPU uyumlu dönüşümler, ölçülebilir süreler ve reflow’u artıran özelliklerden kaçınmak kritik.</p>",
                        ProTip = "Yeni bir feature eklerken “en kötü senaryoda” ne olur sorusunu sor. Sonra performans ölçümüyle karar ver.",
                        CoverImageUrl = "",
                        Icon = "🚀",
                        PublishDate = DateTime.UtcNow.AddDays(-140),
                        TechIcons = new System.Collections.Generic.List<string> { "fa-brands fa-vuejs|Vue.js", "fa-solid fa-bolt|Vite", "fa-solid fa-wand-magic-sparkles|GSAP" },
                        Tags = new System.Collections.Generic.List<string> { "Web Geliştirme" }
                    },
                    new BlogPost
                    {
                        Title = "CSS Animasyonları İpuçları",
                        Summary = "CSS ile akıcı animasyonlar yaparken hangi özellikler hız kazandırır, hangileri riskli?",
                        Content = "<p>CSS animasyonlarında amaç sadece görsellik değil; kullanıcı için 'bekleme' hissini azaltmak. Bu yüzden animasyonları kısa, tutarlı ve hedef odaklı tutmak gerekir.</p><p>transform ve opacity; layout'u yeniden hesaplatmadan ilerlediği için performans açısından genelde daha sağlıklıdır. Özellikle hover/scroll efektlerinde bu ikisini merkeze almak iyi bir başlangıçtır.</p><p>Aşırı will-change veya çok fazla aynı anda çalışan animasyonlar cihazı yavaşlatabilir. Bu nedenle animasyonları parçalayarak ve tetikleme koşullarını optimize ederek ilerleyin.</p>",
                        ProTip = "Animasyon eklediğin bileşenleri birlikte test et: en kötü 3 senaryoda bile akıcı mı kalıyor?",
                        CoverImageUrl = "",
                        Icon = "🎨",
                        PublishDate = DateTime.UtcNow.AddDays(-145), // 10 Ocak 2024
                        TechIcons = new System.Collections.Generic.List<string> { "fa-solid fa-code|CSS Variables", "fa-solid fa-palette|Design Tokens" },
                        Tags = new System.Collections.Generic.List<string> { "UI / Animasyon" }
                    },
                    new BlogPost
                    {
                        Title = "Vue 3 Composition API",
                        Summary = "Vue 3'te temiz ve sürdürülebilir kod yazmak için pratik bir başlangıç rehberi.",
                        Content = "<p>Vue 3'te en büyük kazanç; mantığı component içine sıkıştırmak yerine 'composable' olarak yeniden kullanabilmek.</p><p>Props ve emit akışı, veri kimin sorumluluğunda sorusunu netleştirir. Bu sayede kod büyüdükçe dağılmaz.</p>",
                        ProTip = "Önce küçük bileşenler, sonra composable'lar: refactor maliyetini düşür.",
                        CoverImageUrl = "",
                        Icon = "💡",
                        PublishDate = DateTime.UtcNow.AddDays(-150),
                        TechIcons = new System.Collections.Generic.List<string> { "fa-brands fa-vuejs|Vue.js" },
                        Tags = new System.Collections.Generic.List<string> { "Frontend" }
                    },
                    new BlogPost
                    {
                        Title = "PostgreSQL İpuçları",
                        Summary = "Veritabanı performansını arttırmak ve güvenli sorgular yazmak için pratik ipuçları.",
                        Content = "<p>Veritabanı sorgularının hızını arttırmak için doğru indeksleme (Indexing) çok önemlidir.</p>",
                        ProTip = "Gereksiz tabloları joinlemekten kaçının, EXPLAIN kullanarak sorgu maliyetini analiz edin.",
                        CoverImageUrl = "",
                        Icon = "💾",
                        PublishDate = DateTime.UtcNow.AddDays(-160),
                        TechIcons = new System.Collections.Generic.List<string> { "fa-solid fa-database|PostgreSQL" },
                        Tags = new System.Collections.Generic.List<string> { "Backend" }
                    },
                    new BlogPost
                    {
                        Title = "Git ve GitHub İpuçları",
                        Summary = "Takım çalışmalarında Git kullanımı, branch yapıları ve merge süreçleri.",
                        Content = "<p>Version kontrolü sadece kod yedeği almak değil, projenin tarihçesini temiz bir şekilde okumaktır.</p>",
                        ProTip = "Commit mesajlarınızı anlaşılır tutun: Conventional Commits kullanmaya özen gösterin.",
                        CoverImageUrl = "",
                        Icon = "🐙",
                        PublishDate = DateTime.UtcNow.AddDays(-170),
                        TechIcons = new System.Collections.Generic.List<string> { "fa-brands fa-github|GitHub", "fa-brands fa-git-alt|Git" },
                        Tags = new System.Collections.Generic.List<string> { "Araçlar" }
                    },
                    new BlogPost
                    {
                        Title = "Docker ile Mikroservisler",
                        Summary = "Uygulamalarınızı izole ederek dağıtım süreçlerinizi hızlandırın.",
                        Content = "<p>Docker container mimarisi sayesinde uygulamanız tüm ortamlarda aynı şekilde çalışır.</p>",
                        ProTip = "İmaj boyutlarını düşük tutmak için Alpine tabanlı image'lar tercih edin.",
                        CoverImageUrl = "",
                        Icon = "🐳",
                        PublishDate = DateTime.UtcNow.AddDays(-180),
                        TechIcons = new System.Collections.Generic.List<string> { "fa-brands fa-docker|Docker" },
                        Tags = new System.Collections.Generic.List<string> { "DevOps" }
                    }
                );
            }

            // 4. Skills
            if (!context.SkillCategories.Any())
            {
                var tech = new SkillCategory
                {
                    Title = "Yazılım Teknolojileri",
                    Icon = "fa-solid fa-code",
                    Skills = new System.Collections.Generic.List<SkillItem>
                    {
                        new SkillItem { Name = "C# / .NET Core", Percentage = 95, Color = "#512BD4" },
                        new SkillItem { Name = "HTML / CSS", Percentage = 30, Color = "#264de4" },
                        new SkillItem { Name = "JavaScript / Vue.js", Percentage = 80, Color = "#f7df1e" }
                    }
                };

                var tools = new SkillCategory
                {
                    Title = "Araçlar",
                    Icon = "fa-solid fa-toolbox",
                    Skills = new System.Collections.Generic.List<SkillItem>
                    {
                        new SkillItem { Name = "Git", Percentage = 90, Color = "#f34f29" },
                        new SkillItem { Name = "Docker", Percentage = 75, Color = "#0db7ed" },
                        new SkillItem { Name = "VS Code / Visual Studio", Percentage = 95, Color = "#007ACC" }
                    }
                };

                var skills = new SkillCategory
                {
                    Title = "Yetkinlikler",
                    Icon = "fa-solid fa-brain",
                    Skills = new System.Collections.Generic.List<SkillItem>
                    {
                        new SkillItem { Name = "Problem Çözme", Percentage = 92, Color = "#ff6b35" },
                        new SkillItem { Name = "Takım Çalışması", Percentage = 90, Color = "#42b883" },
                        new SkillItem { Name = "Zaman Yönetimi", Percentage = 85, Color = "#00d2ff" }
                    }
                };

                var databases = new SkillCategory
                {
                    Title = "Veri Tabanları",
                    Icon = "fa-solid fa-database",
                    Skills = new System.Collections.Generic.List<SkillItem>
                    {
                        new SkillItem { Name = "SQL Server", Percentage = 85, Color = "#CC2927" },
                        new SkillItem { Name = "PostgreSQL", Percentage = 80, Color = "#336791" },
                        new SkillItem { Name = "MongoDB", Percentage = 70, Color = "#47A248" }
                    }
                };

                context.SkillCategories.AddRange(tech, tools, skills, databases);
            }

            // 5. Projects
            if (!context.Projects.Any())
            {
                var categoryWeb = new ProjectCategory { Name = "WEB GELİŞTİRME & UI/UX" };
                var categoryApi = new ProjectCategory { Name = "API & BACKEND" };
                var categoryMobile = new ProjectCategory { Name = "MOBİL UYGULAMA" };
                var categoryDesign = new ProjectCategory { Name = "TASARIM" };

                context.ProjectCategories.AddRange(categoryWeb, categoryApi, categoryMobile, categoryDesign);
                context.SaveChanges();

                context.Projects.AddRange(
                    new Project
                    {
                        Title = "Kişisel Portföy Sitesi",
                        Summary = "Modern ve Dark-Tech temalı kişisel web sitesi.",
                        ProjectCategoryId = categoryWeb.Id,
                        Aim = "Kendi yeteneklerimi, tecrübelerimi ve dijital dünyadaki ayak izimi tüm dünyaya modern ve “Dark-Tech” temalı bir kod mimarisi ile sergilemek. Marka kimliğimi en iyi şekilde yansıtacak etkileşimli, responsive ve fütüristik bir arayüz kurgulamak.",
                        ChallengesAndSolutions = "<ul class=\"solution-list\"><li><i class=\"fa-solid fa-check\" style=\"color: var(--primary)\"></i> Kullanıcının sayfada gezinirken sıkılmamasını sağlayacak akıcı animasyonlar (scroll-reveal) tasarlamak ve bunları performans/RAM kaybı yaşamadan Vue 3 ekosistemi içinde senkronize optimize etmek en büyük teknik süreçti.</li><li><i class=\"fa-solid fa-check\" style=\"color: var(--primary)\"></i> Vue Composition API kullanılarak reaktif ve temiz ayrıştırılmış bir kod mimarisi oluşturuldu.</li><li><i class=\"fa-solid fa-check\" style=\"color: var(--primary)\"></i> Swiper.js entegrasyonunda setInterval sistemi kurgulanarak performans artırıldı.</li><li><i class=\"fa-solid fa-check\" style=\"color: var(--primary)\"></i> Özel CSS maskelemeleri (mask, -webkit-mask) ve gradient oyunlarıyla “glow” hissi veren tasarım sistemi geliştirildi.</li></ul>",
                        TechTags = new System.Collections.Generic.List<string> { "fa-brands fa-vuejs|Vue 3", "fa-solid fa-bolt|Vite", "fa-brands fa-js|JavaScript", "fa-brands fa-css3-alt|CSS3 / Variables", "fa-solid fa-layer-group|Swiper.js" },
                        ImageUrls = new System.Collections.Generic.List<string> { "https://picsum.photos/id/1018/1920/1080", "https://picsum.photos/id/1019/1920/1080" }
                    },
                    new Project
                    {
                        Title = "E-Ticaret Platformu REST API",
                        Summary = "Uçtan uca kusursuz alışveriş deneyimi sunan e-ticaret platformu altyapısı.",
                        ProjectCategoryId = categoryApi.Id,
                        Aim = "Müşteriler için sepetten ödemeye kadar sorunsuz bir süreç yaratan yüksek performanslı API altyapısı geliştirmek.",
                        ChallengesAndSolutions = "<ul class=\"solution-list\"><li><i class=\"fa-solid fa-check\" style=\"color: var(--primary)\"></i> Ödeme güvenliği ve RabbitMQ asenkron kuyruk yapısı entegre edildi.</li></ul>",
                        TechTags = new System.Collections.Generic.List<string> { "fa-brands fa-windows|.NET Core 8", "fa-solid fa-database|SQL Server", "fa-solid fa-server|RabbitMQ" },
                        ImageUrls = new System.Collections.Generic.List<string> { "https://picsum.photos/id/1021/1920/1080", "https://picsum.photos/id/1022/1920/1080" }
                    },
                    new Project
                    {
                        Title = "Fintech Mobil Banka Uygulaması",
                        Summary = "Fintech mobil bankacılık deneyimi kurgusu.",
                        ProjectCategoryId = categoryMobile.Id,
                        Aim = "Kullanıcıların hesaplarını kolayca yönetebilecekleri güvenli mobil uygulama.",
                        ChallengesAndSolutions = "<ul class=\"solution-list\"><li><i class=\"fa-solid fa-check\" style=\"color: var(--primary)\"></i> Farklı cihazlar arası UI tutarsızlıkları Flutter kullanılarak aşıldı.</li></ul>",
                        TechTags = new System.Collections.Generic.List<string> { "fa-solid fa-mobile-screen|Flutter", "fa-brands fa-node-js|Node.js API" },
                        ImageUrls = new System.Collections.Generic.List<string> { "https://picsum.photos/id/1023/1920/1080" }
                    },
                    new Project
                    {
                        Title = "Kurumsal Firma Web Sitesi",
                        Summary = "B2B kurumsal iletişim için tasarlanmış web platformu.",
                        ProjectCategoryId = categoryWeb.Id,
                        Aim = "Firmanın hizmetlerini müşterilerine net bir şekilde aktarmasını sağlamak.",
                        ChallengesAndSolutions = "<ul class=\"solution-list\"><li><i class=\"fa-solid fa-check\" style=\"color: var(--primary)\"></i> Çoklu dil desteği i18n ile projeye entegre edildi.</li></ul>",
                        TechTags = new System.Collections.Generic.List<string> { "fa-brands fa-html5|HTML5", "fa-brands fa-js|JavaScript", "fa-brands fa-css3-alt|Tailwind CSS" },
                        ImageUrls = new System.Collections.Generic.List<string> { "https://picsum.photos/id/1031/1920/1080" }
                    },
                    new Project
                    {
                        Title = "Stok Yönetim Paneli",
                        Summary = "Depo ve stok süreçlerini dijitalleştiren SaaS yazılımı.",
                        ProjectCategoryId = categoryApi.Id,
                        Aim = "Firmaların depo giriş çıkışlarını anlık takip edebilmesi.",
                        ChallengesAndSolutions = "<ul class=\"solution-list\"><li><i class=\"fa-solid fa-check\" style=\"color: var(--primary)\"></i> SignalR ile anlık bildirim sistemi kurgulandı.</li></ul>",
                        TechTags = new System.Collections.Generic.List<string> { "fa-brands fa-vuejs|Vue.js", "fa-brands fa-windows|.NET Core", "fa-solid fa-database|PostgreSQL" },
                        ImageUrls = new System.Collections.Generic.List<string> { "https://picsum.photos/id/1041/1920/1080" }
                    },
                    new Project
                    {
                        Title = "UI/UX Tasarım Konseptleri",
                        Summary = "Dribbble ve Behance için hazırlanmış tasarım örnekleri.",
                        ProjectCategoryId = categoryDesign.Id,
                        Aim = "Modern arayüz trendlerini yakalamak ve portföye tasarım yeteneklerini eklemek.",
                        ChallengesAndSolutions = "<ul class=\"solution-list\"><li><i class=\"fa-solid fa-check\" style=\"color: var(--primary)\"></i> Figma ile interaktif prototipler oluşturuldu.</li></ul>",
                        TechTags = new System.Collections.Generic.List<string> { "fa-brands fa-figma|Figma", "fa-solid fa-palette|Adobe XD" },
                        ImageUrls = new System.Collections.Generic.List<string> { "https://picsum.photos/id/1051/1920/1080" }
                    }
                );
            }



            context.SaveChanges();
        }
    }
}
