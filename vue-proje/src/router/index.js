import { createRouter, createWebHistory } from 'vue-router'

const routes = [
  { path: '/', name: 'Index', component: () => import('../views/IndexView.vue') },
  { path: '/hakkinda', name: 'Hakkinda', component: () => import('../views/HakkindaView.vue') },
  { path: '/blog', name: 'Blog', component: () => import('../views/BlogView.vue') },
  { path: '/blog/:slug', name: 'BlogDetail', component: () => import('../views/BlogDetailView.vue') },
  { path: '/yetenekler', name: 'Yetenekler', component: () => import('../views/YeteneklerView.vue') },
  { path: '/projects', name: 'Projects', component: () => import('../views/ProjectsView.vue') },
  { path: '/proje/:id', name: 'ProjectDetail', component: () => import('../views/ProjectDetailView.vue') },
  { path: '/contact', name: 'Contact', component: () => import('../views/ContactView.vue') },
  {
    path: '/admin',
    component: () => import('../views/Admin/AdminLayout.vue'),
    children: [
      { path: 'login', name: 'AdminLogin', component: () => import('../views/Admin/LoginView.vue'), meta: { title: 'Admin Giriş' } },
      { path: '', name: 'AdminHome', component: () => import('../views/Admin/HomeSettings.vue'), meta: { title: 'Anasayfa Yönetimi', requiresAuth: true } },
      { path: 'projects', name: 'AdminProjects', component: () => import('../views/Admin/ProjectSettings.vue'), meta: { title: 'Projeler Yönetimi', requiresAuth: true } },
      { path: 'messages', name: 'AdminMessages', component: () => import('../views/Admin/MessagesView.vue'), meta: { title: 'Mesajlar', requiresAuth: true } },
      { path: 'about', name: 'AdminAbout', component: () => import('../views/Admin/AboutSettings.vue'), meta: { title: 'Hakkında Yönetimi', requiresAuth: true } },
      { path: 'blog', name: 'AdminBlog', component: () => import('../views/Admin/BlogSettings.vue'), meta: { title: 'Blog Yönetimi', requiresAuth: true } },
      { path: 'skills', name: 'AdminSkills', component: () => import('../views/Admin/SkillsSettings.vue'), meta: { title: 'Yetenekler Yönetimi', requiresAuth: true } },
      { path: 'analytics', name: 'AdminAnalytics', component: () => import('../views/Admin/AnalyticsSettings.vue'), meta: { title: 'İstatistikler', requiresAuth: true } },
      { path: 'health', name: 'AdminHealth', component: () => import('../views/Admin/HealthSettings.vue'), meta: { title: 'Sistem Sağlığı', requiresAuth: true } },
      { path: 'seo', name: 'AdminSeo', component: () => import('../views/Admin/SeoSettings.vue'), meta: { title: 'SEO ve GEO', requiresAuth: true } },
      { path: 'contact', name: 'AdminContact', component: () => import('../views/Admin/ContactSettings.vue'), meta: { title: 'İletişim & Sosyal', requiresAuth: true } },
      { path: 'api-keys', name: 'AdminApiKeys', component: () => import('../views/Admin/ApiSettings.vue'), meta: { title: 'API Yönetimi', requiresAuth: true } },
      { path: 'media', name: 'AdminMedia', component: () => import('../views/Admin/MediaSettings.vue'), meta: { title: 'Medya Kütüphanesi', requiresAuth: true } },
      { path: 'glossary', name: 'AdminGlossary', component: () => import('../views/Admin/GlossarySettings.vue'), meta: { title: 'Dinamik Sözlük', requiresAuth: true } }
      // Diğer admin sayfaları buraya eklenebilir...
    ]
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

const seoCache = {};
let cachedAboutData = null;

// Sadece Auth Guard (Bloklamaz, anında geçiş yapar) ve Gizli Sayfa Engellemesi
router.beforeEach((to, from, next) => {
  // Admin giriş kontrolü
  if (to.meta.requiresAuth) {
    const token = localStorage.getItem('token');
    if (!token) {
      return next({ name: 'AdminLogin' });
    }
  }

  // Blog sayfası ayarlardan gizlenmişse URL üzerinden girilmesini (ve istek atılmasını) engelle
  if (to.path.startsWith('/blog') && !to.path.startsWith('/admin')) {
    const seoSettings = JSON.parse(localStorage.getItem('seoSettings') || '[]');
    const blogSetting = seoSettings.find(s => (s.route || s.Route) === '/blog');
    
    if (blogSetting && (blogSetting.isVisible === false || blogSetting.IsVisible === false)) {
      return next({ path: '/' }); // Anasayfaya geri yönlendir, böylece API isteği hiç gitmez
    }
  }

  next();
});

// Sayfa geçişi bittikten SONRA SEO'yu arka planda güncelle
router.afterEach((to) => {
  if (!to.path.startsWith('/admin')) {
    (async () => {
      try {
        const path = encodeURIComponent(to.path);
        const apiUrl = import.meta.env.VITE_API_URL || 'https://kadir-api-a4eeeaeygvdxage6.canadaeast-01.azurewebsites.net/api';
        
        // Cache kontrolü
        let seoData = seoCache[path];
        
        if (!seoData) {
          const res = await fetch(`${apiUrl}/SeoSettings/page?route=${path}`);
          if (res.ok) {
            seoData = await res.json();
            seoCache[path] = seoData; // Önbelleğe al
          }
        }

        if (seoData) {
          if (seoData.seoTitle) {
            document.title = seoData.seoTitle;
          }
          
          if (seoData.seoDescription) {
            let metaDesc = document.querySelector('meta[name="description"]');
            if (!metaDesc) {
              metaDesc = document.createElement('meta');
              metaDesc.name = "description";
              document.head.appendChild(metaDesc);
            }
            metaDesc.content = seoData.seoDescription;
          }
          
          if (seoData.geoTitle) {
            let ogTitle = document.querySelector('meta[property="og:title"]');
            if (!ogTitle) {
              ogTitle = document.createElement('meta');
              ogTitle.setAttribute('property', 'og:title');
              document.head.appendChild(ogTitle);
            }
            ogTitle.content = seoData.geoTitle;
          }
          
          if (seoData.geoDescription) {
            let ogDesc = document.querySelector('meta[property="og:description"]');
            if (!ogDesc) {
              ogDesc = document.createElement('meta');
              ogDesc.setAttribute('property', 'og:description');
              document.head.appendChild(ogDesc);
            }
            ogDesc.content = seoData.geoDescription;
          }

          if (seoData.lang) {
            document.documentElement.lang = seoData.lang;
          }

          let ogImage = document.querySelector('meta[property="og:image"]');
          if (!ogImage) {
            ogImage = document.createElement('meta');
            ogImage.setAttribute('property', 'og:image');
            document.head.appendChild(ogImage);
          }
          ogImage.content = 'https://kadir.com/pwa-512x512.png';

          let twitterCard = document.querySelector('meta[name="twitter:card"]');
          if (!twitterCard) {
            twitterCard = document.createElement('meta');
            twitterCard.setAttribute('name', 'twitter:card');
            document.head.appendChild(twitterCard);
          }
          twitterCard.content = 'summary_large_image';

          // JSON-LD Cache & Injection
          let schemaScript = document.querySelector('script[type="application/ld+json"]');
          if (!schemaScript) {
            schemaScript = document.createElement('script');
            schemaScript.type = "application/ld+json";
            document.head.appendChild(schemaScript);
          }
          
          const isProjectRoute = to.path.startsWith('/proje/');
          
          if (!cachedAboutData) {
             const aboutRes = await fetch(`${apiUrl}/AboutSettings`);
             if (aboutRes.ok) {
               cachedAboutData = await aboutRes.json();
             }
          }

          if (cachedAboutData) {
             let schemaData = {
               "@context": "https://schema.org",
               "@type": isProjectRoute ? "SoftwareApplication" : "WebPage",
               "name": seoData.seoTitle || "Kadir Portfolio",
               "url": `https://kadir.com${to.path}`,
               "publisher": {
                 "@type": "Organization",
                 "name": "Kadir Portfolio",
                 "logo": {
                   "@type": "ImageObject",
                   "url": "https://kadir.com/pwa-192x192.png"
                 }
               },
               "datePublished": new Date().toISOString().split('T')[0]
             };

             if (cachedAboutData.isLookingForJob || cachedAboutData.IsLookingForJob) {
               schemaData.jobTitle = "Software Developer (İş Arıyor / Open to Work)";
               schemaData.seeks = {
                 "@type": "Demand",
                 "itemOffered": {
                   "@type": "Service",
                   "name": "Software Development"
                 }
               };
             }
             
             schemaScript.textContent = JSON.stringify(schemaData);
          }
        }
      } catch (err) {
        console.warn("SEO fetch error:", err);
      }
    })();
  }
});

// Eğer Vercel/Sunucu tarafında yeni build alınırsa ve eski JS dosyaları (chunks) silinirse,
// kullanıcının eski sekmesindeki Vue Router sayfaya geçerken chunk bulamayıp hata verir.
// Bu hata durumunda sayfayı zorla yenileyerek yeni build dosyalarını almasını sağlıyoruz.
router.onError((error, to) => {
  if (
    error.message.includes('Failed to fetch dynamically imported module') ||
    error.message.includes('Importing a module script failed')
  ) {
    window.location.href = to.fullPath;
  }
});

export default router

