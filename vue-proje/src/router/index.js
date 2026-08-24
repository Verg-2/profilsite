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
      { path: 'glossary', name: 'AdminGlossary', component: () => import('../views/Admin/GlossarySettings.vue'), meta: { title: 'Dinamik Sözlük', requiresAuth: true } }
      // Diğer admin sayfaları buraya eklenebilir...
    ]
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

// Dinamik SEO Enjeksiyonu ve Auth Guard
router.beforeEach(async (to, from, next) => {
  // --- AUTH GUARD ---
  if (to.meta.requiresAuth) {
    const token = localStorage.getItem('token');
    if (!token) {
      return next({ name: 'AdminLogin' });
    }
  }

  // Frontend'de sayfa değiştiğinde backend'den o sayfanın SEO verisini çek.
  if (!to.path.startsWith('/admin')) {
    try {
      // Lazy import axios or use fetch to avoid circular dependency with api.js
      const path = encodeURIComponent(to.path);
      const apiUrl = import.meta.env.VITE_API_URL || 'https://profilsite.onrender.com/api';
      const res = await fetch(`${apiUrl}/SeoSettings/${path}`);
      if (res.ok) {
        const seoData = await res.json();
        if (seoData && seoData.seoTitle) {
          document.title = seoData.seoTitle;
        }
        
        // Meta Description update
        if (seoData && seoData.seoDescription) {
          let metaDesc = document.querySelector('meta[name="description"]');
          if (!metaDesc) {
            metaDesc = document.createElement('meta');
            metaDesc.name = "description";
            document.head.appendChild(metaDesc);
          }
          metaDesc.content = seoData.seoDescription;
        }
        
        // GEO / OpenGraph Title update
        if (seoData && seoData.geoTitle) {
          let ogTitle = document.querySelector('meta[property="og:title"]');
          if (!ogTitle) {
            ogTitle = document.createElement('meta');
            ogTitle.setAttribute('property', 'og:title');
            document.head.appendChild(ogTitle);
          }
          ogTitle.content = seoData.geoTitle;
        }
        
        // GEO / OpenGraph Description update
        if (seoData && seoData.geoDescription) {
          let ogDesc = document.querySelector('meta[property="og:description"]');
          if (!ogDesc) {
            ogDesc = document.createElement('meta');
            ogDesc.setAttribute('property', 'og:description');
            document.head.appendChild(ogDesc);
          }
          ogDesc.content = seoData.geoDescription;
        }

        // Lang update
        if (seoData && seoData.lang) {
          document.documentElement.lang = seoData.lang;
        }

        // Add OG Image
        let ogImage = document.querySelector('meta[property="og:image"]');
        if (!ogImage) {
          ogImage = document.createElement('meta');
          ogImage.setAttribute('property', 'og:image');
          document.head.appendChild(ogImage);
        }
        ogImage.content = 'https://kadir.com/pwa-512x512.png';

        // Add Twitter Card
        let twitterCard = document.querySelector('meta[name="twitter:card"]');
        if (!twitterCard) {
          twitterCard = document.createElement('meta');
          twitterCard.setAttribute('name', 'twitter:card');
          document.head.appendChild(twitterCard);
        }
        twitterCard.content = 'summary_large_image';

        // Dinamik JSON-LD Injection
        let schemaScript = document.querySelector('script[type="application/ld+json"]');
        if (!schemaScript) {
          schemaScript = document.createElement('script');
          schemaScript.type = "application/ld+json";
          document.head.appendChild(schemaScript);
        }
        
        const isProjectRoute = to.path.startsWith('/proje/');
        try {
           const apiUrl = import.meta.env.VITE_API_URL || 'https://profilsite.onrender.com/api';
           const aboutRes = await fetch(`${apiUrl}/AboutSettings`);
           if (aboutRes.ok) {
             const aboutData = await aboutRes.json();
             
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

             if (aboutData.isLookingForJob || aboutData.IsLookingForJob) {
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
        } catch (e) {
           console.warn("JSON-LD fetch error", e);
        }

      }
    } catch (err) {
      console.warn("SEO fetch error:", err);
    }
  }
  next();
})

export default router

