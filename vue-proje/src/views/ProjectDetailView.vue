<template>
  <div class="project-detail-page">
    <canvas id="particles-canvas"></canvas>

    <!-- 1. HERO ALANI (Tam Genişlik - Video veya Slider) -->
    <section class="detail-hero" v-if="(activeImages && activeImages.length > 0 && activeImages[0] !== defaultImg) || activeVideoUrl || activeModelUrl">
      
      <!-- EĞER 3D MODEL VARSA MODELİ GÖSTER -->
      <div v-if="activeModelUrl" class="hero-video-container" style="background: transparent; z-index: 10;">
        <model-viewer 
          :src="getFullUrl(activeModelUrl)" 
          auto-rotate 
          autoplay
          camera-controls 
          shadow-intensity="1" 
          environment-image="neutral"
          style="width: 100%; height: 100%; outline: none; background: transparent; cursor: grab;"
        ></model-viewer>
      </div>

      <!-- EĞER VİDEO VARSA VİDEOYU GÖSTER -->
      <div v-else-if="activeVideoUrl" class="hero-video-container">
        
        <!-- YENİ: BULANIK ARKA PLAN KATMANI (Empty space'i şık bir şekilde doldurur) -->
        <div class="blurred-bg yt-blurred-bg" 
             v-if="isYouTube(activeVideoUrl)" 
             :style="{ backgroundImage: `url(https://img.youtube.com/vi/${getYouTubeId(activeVideoUrl)}/hqdefault.jpg)` }">
        </div>
        <video 
          v-else 
          :src="getFullUrl(activeVideoUrl)" 
          autoplay muted loop playsinline 
          preload="metadata"
          poster="data:image/gif;base64,R0lGODlhAQABAIAAAAAAAP///yH5BAEAAAAALAAAAAABAAEAAAIBRAA7"
          class="hero-video blurred-bg"
          :aria-label="projectData.videoAriaLabel || projectData.title + ' Tanıtım Videosu'"
        ></video>

        <!-- Tıklamaları engellemek için transparan katman -->
        <div class="video-overlay-blocker"></div>
        <iframe 
          v-if="isYouTube(activeVideoUrl)"
          :src="getYouTubeEmbedUrl(activeVideoUrl)" 
          frameborder="0" 
          allow="autoplay; fullscreen; picture-in-picture" 
          class="hero-video yt-video"
        ></iframe>
        <video 
          v-else 
          :src="getFullUrl(activeVideoUrl)" 
          autoplay 
          muted 
          loop 
          playsinline 
          preload="metadata"
          poster="data:image/gif;base64,R0lGODlhAQABAIAAAAAAAP///yH5BAEAAAAALAAAAAABAAEAAAIBRAA7"
          class="hero-video"
          :aria-label="projectData.videoAriaLabel || projectData.title + ' Tanıtım Videosu'"
        ></video>
      </div>

      <!-- VİDEO YOKSA RESİM SLIDERI GÖSTER -->
      <Swiper
        v-else
        :modules="[Autoplay, Pagination, EffectCreative]"
        :slides-per-view="1"
        :loop="true"
        :grab-cursor="true"
        :autoplay="{ delay: 4000, disableOnInteraction: false }"
        :pagination="{ clickable: true, dynamicBullets: true }"
        :effect="'creative'"
        :creativeEffect="{
          prev: { shadow: true, translate: ['-20%', 0, -1] },
          next: { translate: ['100%', 0, 0] },
        }"
        :speed="1400"
        :observer="true"
        :observeParents="true"
        class="detail-hero-swiper"
      >
        <SwiperSlide v-for="(img, idx) in activeImages" :key="idx">
          <div class="hero-slide-inner">
            <img :src="img" :alt="projectData.imageAltText || `${projectData.title} Ekran Görüntüsü`" loading="lazy" style="aspect-ratio: 16/9; background-color: rgba(255, 255, 255, 0.05);" class="hero-slide-img" />
          </div>
        </SwiperSlide>
      </Swiper>
      
      <!-- Video veya Resmin Üzerine Binen, Scroll'da Kararan Kalkan -->
      <div class="hero-dark-overlay"></div>
    </section>

    <!-- İÇERİK ALANI (Z-Index: 2, Slider'ın üstüne biner) -->
    <div class="content-section">
      <!-- Duman/Erime Efekti İçin Gradient Maske -->
      <div class="fade-mask"></div>

      <main class="detail-container">

        <!-- YENİ: MODERN BAŞLIK VE KATEGORİ ALANI -->
        <header class="project-header">
        <span class="project-category">{{ lang === 'en' && projectData.categoryEn ? projectData.categoryEn : projectData.category }}</span>
        <h1 class="project-main-title">{{ lang === 'en' && projectData.titleEn ? projectData.titleEn : projectData.title }}</h1>
      </header>
      
      <section class="tech-section" v-if="projectData.techTags && projectData.techTags.length > 0">
        <h2 class="section-heading">{{ lang === 'en' ? 'Technologies Used' : 'Kullanılan Teknolojiler' }}</h2>
        <div class="tech-badges">
          <div v-for="(tech, tIdx) in projectData.techTags" :key="tIdx" class="tech-badge">
            <i v-if="tech.includes('|')" :class="tech.split('|')[0]"></i>
            <span>{{ tech.includes('|') ? tech.split('|')[1] : tech }}</span>
          </div>
        </div>
      </section>

      <!-- 3. PROJE DETAY METNİ -->
      <section class="info-section">
        <div class="info-block" v-if="(lang === 'en' && projectData.aimEn) || projectData.aim">
          <div class="info-header">
            <div class="info-icon"><i class="fa-solid fa-bullseye"></i></div>
            <h3>{{ lang === 'en' ? 'Project Aim' : 'Projenin Amacı' }}</h3>
          </div>
          <div class="info-text" v-safe-html="lang === 'en' && projectData.aimEn ? projectData.aimEn : projectData.aim"></div>
        </div>
        <div class="info-block" v-if="(lang === 'en' && projectData.challengesAndSolutionsEn) || projectData.challengesAndSolutions">
          <div class="info-header">
            <div class="info-icon"><i class="fa-solid fa-bolt"></i></div>
            <h3>{{ lang === 'en' ? 'Challenges and Solutions' : 'Zorluklar ve Çözümler' }}</h3>
          </div>
          <div class="info-text" v-safe-html="lang === 'en' && projectData.challengesAndSolutionsEn ? projectData.challengesAndSolutionsEn : projectData.challengesAndSolutions"></div>
        </div>
        
        <!-- Eğer content veya summary varsa tam genişlikte göster (eski fallback) -->
        <div class="info-block" style="grid-column: 1 / -1;" v-if="!projectData.aim && !projectData.challengesAndSolutions && !projectData.aimEn && !projectData.challengesAndSolutionsEn && (projectData.content || projectData.summary || projectData.contentEn || projectData.summaryEn)">
          <div class="info-header">
            <div class="info-icon"><i class="fa-solid fa-bullseye"></i></div>
            <h3>{{ lang === 'en' ? 'Project Details' : 'Proje Detayları' }}</h3>
          </div>
          <div class="info-text" v-safe-html="lang === 'en' && (projectData.contentEn || projectData.summaryEn) ? (projectData.contentEn || projectData.summaryEn) : (projectData.content || projectData.summary)"></div>
        </div>
      </section>
      
      <!-- Geri Dönüş Aksiyonu -->
      <div class="back-action">
        <router-link to="/projects" class="btn-back">
          <i class="fa-solid fa-arrow-left-long"></i> {{ lang === 'en' ? 'Back to Projects' : 'Projelere Dön' }}
        </router-link>
      </div>

      </main>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, nextTick, onUnmounted, computed, inject } from 'vue';
import { useRoute } from 'vue-router';
import { Swiper, SwiperSlide } from 'swiper/vue';
import { Autoplay, Pagination, EffectCreative } from 'swiper/modules';
import api from '@/services/api';
import defaultImg from '@/assets/img/wolff.png';
import { gsap } from 'gsap';
import { ScrollTrigger } from 'gsap/ScrollTrigger';

gsap.registerPlugin(ScrollTrigger);

// Swiper Stilleri
import 'swiper/css';
import 'swiper/css/pagination';
import 'swiper/css/effect-creative';

const route = useRoute();
const projectId = route.params.id; // Router'dan dinamik ID çekiyoruz
const projectData = ref({});
const lang = inject('lang', ref('tr'));

const currentTheme = ref(document.documentElement.getAttribute('data-theme') || 'dark');
let themeObserver = null;

const activeVideoUrl = computed(() => {
  const data = projectData.value;
  if (!data || Object.keys(data).length === 0) return '';
  if (currentTheme.value === 'light' && data.lightVideoUrl) return data.lightVideoUrl;
  if (currentTheme.value === 'dark' && data.darkVideoUrl) return data.darkVideoUrl;
  return data.videoUrl || '';
});

const activeModelUrl = computed(() => {
  const data = projectData.value;
  if (!data || Object.keys(data).length === 0) return '';
  return data.model3DUrl || data.Model3DUrl || '';
});

const activeImages = computed(() => {
  const data = projectData.value;
  if (!data || Object.keys(data).length === 0) return [defaultImg, defaultImg, defaultImg];
  
  let targetArray = data.imageUrls || [];
  if (currentTheme.value === 'light' && data.lightImageUrls && data.lightImageUrls.length > 0) {
    targetArray = data.lightImageUrls;
  } else if (currentTheme.value === 'dark' && data.darkImageUrls && data.darkImageUrls.length > 0) {
    targetArray = data.darkImageUrls;
  }
  
  if (!targetArray || targetArray.length === 0) {
    return [defaultImg, defaultImg, defaultImg];
  }
  
  let imgs = targetArray.map(img => getFullUrl(img));
  if (imgs.length === 1) {
    imgs.push(imgs[0]);
    imgs.push(imgs[0]);
  }
  return imgs;
});

const getFullUrl = (url) => {
  if (!url) return defaultImg;
  if (url.startsWith('http') || url.startsWith('data:')) return url;
  return api.defaults.baseURL.replace('/api', '') + url;
}

const isYouTube = (url) => {
  if (!url) return false
  return url.includes('youtube.com') || url.includes('youtu.be')
}

const getYouTubeId = (url) => {
  if (!url) return '';
  if (url.includes('youtube.com/watch?v=')) {
    return url.split('v=')[1].split('&')[0];
  } else if (url.includes('youtu.be/')) {
    return url.split('youtu.be/')[1].split('?')[0];
  }
  return '';
}

const getYouTubeEmbedUrl = (url) => {
  const videoId = getYouTubeId(url);
  if (!videoId) return '';
  // controls=0 gizler, disablekb=1 klavye kısayollarını kapatır, rel=0 önerilenleri gizler
  // playsinline=1 mobil tam ekranı engeller, iv_load_policy=3 video ek açıklamalarını gizler
  return `https://www.youtube.com/embed/${videoId}?autoplay=1&mute=1&loop=1&playlist=${videoId}&controls=0&disablekb=1&rel=0&modestbranding=1&playsinline=1&iv_load_policy=3&showinfo=0&fs=0`
}

const fetchProject = async () => {
  try {
    const [res, catRes] = await Promise.all([
      api.get(`/Projects/${projectId}`),
      api.get('/Projects/categories')
    ]);
    const data = res.data;
    const categories = catRes.data;
    
    // Find category name
    const cat = categories.find(c => c.id === data.projectCategoryId);
    data.category = cat ? cat.name : 'Proje';
    data.categoryEn = cat && cat.nameEn ? cat.nameEn : 'Project';
    
    projectData.value = data;
    
    initAnimations();
  } catch (error) {
    console.error('Proje detayı yüklenirken hata oluştu:', error);
  }
}

onMounted(() => {
  window.scrollTo(0, 0); // Sayfa açılınca en üste ışınlan
  
  // Theme Observer for dynamic media
  themeObserver = new MutationObserver(() => {
    currentTheme.value = document.documentElement.getAttribute('data-theme') || 'dark';
  });
  themeObserver.observe(document.documentElement, { attributes: true, attributeFilter: ['data-theme'] });
  
  fetchProject();
});

const initAnimations = () => {
  nextTick(() => {
    // 1. HERO SLIDER PARALLAX EFEKTİ (Daha Uzun Mesafe, Tok ve Ağır)
    if (document.querySelector('.hero-slide-img')) {
      gsap.fromTo('.hero-slide-img', 
        { scale: 1.15 },
        { 
          scale: 1,
          ease: "none",
          scrollTrigger: {
            trigger: '.content-section',
            start: 'top 100%', 
            end: 'top -50%', // Kaydırma mesafesi muazzam uzatıldı (Çok geç biter)
            scrub: 3.5 // Pürüzsüz momentum (Aşırı Tok)
          }
        }
      );
    }

    gsap.to('.hero-dark-overlay', {
      opacity: 0.9, 
      ease: "none",
      scrollTrigger: {
        trigger: '.content-section',
        start: 'top 100%',
        end: 'top -50%',
        scrub: 3.5
      }
    });

    const isLight = document.documentElement.getAttribute('data-theme') === 'light';
    const glowColor = isLight ? 'rgba(240, 90, 40,' : 'rgba(255, 59, 29,';

    // 2. K HARFİ HİZASI VE DİNAMİK ATEŞ GLOW
    gsap.fromTo('.project-main-title',
      { filter: `drop-shadow(0 0 0px ${glowColor} 0))` },
      { 
        filter: `drop-shadow(0 0 50px ${glowColor} 0.6))`,
        ease: 'none',
        scrollTrigger: {
          trigger: '.project-main-title',
          start: 'top 85%',   
          end: 'top -10%',     
          scrub: 3.5
        }
      }
    );

    // KUSURSUZ SENKRONİZASYON (MASTER TIMELINE - MESAFESİ CİDDİ UZATILMIŞ)
    // Öğelerin sırasının karışmasını engelleyen yapının kaydırma alanı büyütüldü:
    const contentTl = gsap.timeline({
      scrollTrigger: {
        trigger: '.content-section',
        start: 'top 85%',
        end: 'bottom 100%', // Animasyon, sayfanın altına inildiğinde %100 tamamlansın (Opacity yarım kalmasın)
        scrub: 1 // Daha tepkisel ve pürüzsüz
      }
    });

    contentTl
      // 3. Başlık (Aşağıdan gelme mesafesi 150'den 200'e çıkarıldı)
      .fromTo('.project-header', { y: 200, opacity: 0 }, { y: 0, opacity: 1, ease: 'none', duration: 1.5 })
      // 4. Teknolojiler Yazısı 
      .fromTo('.section-heading', { y: 150, opacity: 0 }, { y: 0, opacity: 1, ease: 'none', duration: 1.2 }, "-=0.9")
      // 5. Rozetler (Gelme mesafesi 150)
      .fromTo('.tech-badge', { y: 150, opacity: 0, scale: 0.8 }, { y: 0, opacity: 1, scale: 1, ease: 'back.out(1.2)', stagger: 0.2, duration: 1.5 }, "-=0.8")
      // 6. Bilgi Blokları (Derinlik 200)
      .fromTo('.info-block', { y: 200, opacity: 0 }, { y: 0, opacity: 1, ease: 'none', stagger: 0.3, duration: 1.8 }, "-=1.1")
      // 7. Geri Dönüş
      .fromTo('.btn-back', { y: 100, opacity: 0 }, { y: 0, opacity: 1, ease: 'none', duration: 1.2 }, "-=0.9");
  });
};

onUnmounted(() => {
  ScrollTrigger.getAll().forEach(t => t.kill()); // Component kapanınca trigger'ları temizle
  if (themeObserver) {
    themeObserver.disconnect();
  }
});
</script>

<style scoped>
.project-detail-page {
  position: relative;
  min-height: 200vh; /* Sticky kaydırma alanı olması için yükseklik arttı */
  background: var(--dark-bg); /* Dark-Tech Temel Arka Plan */
}

/* === 1. HERO SLIDER EKRANI === */
/* STICKY PARALLAX EFEKTİ İÇİN */
.detail-hero {
  position: sticky;
  top: 0;
  z-index: 1; /* Altta kalacak, kaybolmayacak */
  width: 100%;
  height: 100vh; /* Tam ekran yüksekliği */
  box-shadow: 0 20px 40px rgba(0,0,0,0.8);
}


.detail-hero-swiper {
  width: 100%;
  height: 100%;
}

.hero-video-container {
  width: 100%;
  height: 100%;
  overflow: hidden;
  background: var(--dark-bg);
  position: relative;
  display: flex;
  align-items: center;
  justify-content: center;
  transform: translateZ(0); /* Donanımsal hızlandırma - Tearing'i önler */
  will-change: transform;
  animation: videoFadeIn 1.5s ease forwards;
}

@keyframes videoFadeIn {
  0% { opacity: 0; }
  80% { opacity: 0; } /* 1.2 saniye boyunca tamamen gizli kalır, flashı saklar */
  100% { opacity: 1; }
}

/* === YENİ: BULANIK ARKA PLAN KATMANI === */
.blurred-bg {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  object-fit: cover !important;
  filter: blur(25px) brightness(0.4); /* Siyah boşluğu arka plandaki videonun renkleriyle doldurur */
  transform: scale(1.15) translateZ(0); /* Kenarlardaki blur sızmasını engeller */
  z-index: 0;
  pointer-events: none;
}
.yt-blurred-bg {
  background-size: cover;
  background-position: center;
}

.hero-video {
  width: 100%;
  height: 100%;
  object-fit: contain; /* ASLA KIRPMA (Dik veya yatay tüm videolar ekrana sığsın) */
  pointer-events: none !important; /* Etkileşimi kapat */
  transform: translateZ(0); /* Tearing önleyici */
  border: none !important;
  outline: none !important;
  z-index: 1; /* Bulanık katmanın üstünde */
}

.yt-video {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%) scale(1.15) translateZ(0); /* Tearing'i kesin olarak engellemek için GPU'ya zorla */
  width: 100vw;
  height: 56.25vw; /* 16:9 aspect ratio */
  min-height: 100vh;
  min-width: 177.77vh; /* 16:9 aspect ratio */
  pointer-events: none !important; /* Etkileşimi tamamen kapat */
  backface-visibility: hidden;
  perspective: 1000px;
  z-index: 1; /* Bulanık arkaplanın üzerinde kalması için */
}

.video-overlay-blocker {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  z-index: 10; /* Videonun üzerine çıkar */
  background: transparent;
}

.hero-dark-overlay {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: var(--dark-bg);
  opacity: 0;
  pointer-events: none;
  z-index: 20; /* Tüm içeriğin ve slider'ın üstüne çıkar */
}

.hero-slide-inner {
  position: relative;
  width: 100%;
  height: 100%;
  background: var(--dark-bg);
  overflow: hidden;
}

/* Ken Burns Efekti - Ağır zoom out katar */
.hero-slide-img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  display: block;
  /* GSAP to_ animasyonu ile scale ediliyor */
  transform-origin: center center;
}

/* === YENİ: OVERLAY BÖLÜMÜ === */
.content-section {
  position: relative;
  z-index: 2; /* Slider'ın üzerine biniyor */
  background: var(--dark-bg); /* Temel arka plan rengimiz ile tam kapanır */
  margin-top: 100vh; /* Ekranın 100%'ünü geçip slider'a aşağıdan eklenmesi için başlat noktası */
  padding-bottom: 120px;
}

/* YUMUŞAK GEÇİŞ: Dumandan çıkıyormuş efekti için Maske */
.fade-mask {
  position: absolute;
  top: -150px;
  left: 0;
  width: 100%;
  height: 152px; /* Sub-pixel rendering (1px boşluk) çizgisini kapatmak için 2px taşırıldı */
  background: linear-gradient(to bottom, transparent 0%, var(--dark-bg) 100%);
  pointer-events: none; /* Tıklamayı engellememek için */
}

/* === YENİ: MODERN BAŞLIK ALANI === */
.project-header {
  margin-bottom: 50px;
}

.project-category {
  display: inline-flex;
  align-items: center;
  color: var(--accent);
  background: var(--fire-glow-soft);
  padding: 8px 24px; /* Biraz daha hacimli */
  border-radius: 30px;
  border: 1px solid var(--fire-glow);
  font-size: 0.9rem;
  font-weight: 700;
  letter-spacing: 2px;
  text-transform: uppercase;
  margin-bottom: 24px;
  box-shadow: 0 0 20px var(--fire-glow-soft);
}

.project-main-title {
  font-size: clamp(2.5rem, 5vw, 4.5rem);
  font-weight: 800;
  line-height: 1.1;
  margin: 0;
  background: linear-gradient(135deg, var(--text) 0%, var(--text-muted) 100%); /* Modern gri-beyaz gradient */
  -webkit-background-clip: text;
  background-clip: text;
  -webkit-text-fill-color: transparent;
  filter: drop-shadow(0 0 20px var(--fire-glow)); /* Ateş kırmızısı parlama (glow) efekti */
}

/* SWIPER Paginaton Kapsülü (Dribbble Tarzı) */
:deep(.detail-hero-swiper .swiper-pagination) {
  bottom: 40px !important;
}
:deep(.detail-hero-swiper .swiper-pagination-bullet) {
  background: rgba(255, 255, 255, 0.5);
  width: 8px;
  height: 8px;
  transition: all 0.4s cubic-bezier(0.2, 0.8, 0.2, 1);
  box-shadow: 0 0 5px rgba(0,0,0,0.8);
}
:deep(.detail-hero-swiper .swiper-pagination-bullet-active) {
  background: var(--primary); /* Ateş Kırmızı */
  width: 32px; /* Geniş çubuk */
  border-radius: 4px;
  box-shadow: 0 0 15px var(--fire-glow), 0 0 30px var(--fire-glow-soft);
}


/* === 2. BÖLÜM: TEKNOLOJİ ROZETLERİ === */
.detail-container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 80px 2rem 0; /* İçerik için üst boşluk oluştur, hizalamayı bozmadan */
}

.tech-section {
  margin-bottom: 80px;
}

.section-heading {
  font-size: 1.75rem;
  color: var(--text);
  margin-bottom: 30px;
  position: relative;
  display: inline-block;
  padding-bottom: 10px;
}
.section-heading::after {
  content: '';
  position: absolute;
  bottom: 0;
  left: 0;
  width: 50px;
  height: 4px;
  background: linear-gradient(90deg, var(--primary), var(--accent));
  border-radius: 2px;
}

.tech-badges {
  display: flex;
  flex-wrap: wrap;
  gap: 16px;
}

.tech-badge {
  display: inline-flex;
  align-items: center;
  gap: 12px;
  padding: 14px 28px;
  background: var(--card-bg);
  border: 1px solid var(--border);
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.05); /* Rozetlere hafif derinlik */
  border-radius: 40px; /* Hap formunda yuvarlak */
  transition: translate 0.4s ease, 
              scale 0.4s ease,
              border-color 0.4s ease, 
              box-shadow 0.4s ease, 
              background 0.4s ease;
  position: relative;
  overflow: hidden;
  cursor: pointer;
  will-change: transform, opacity; /* GPU hızlandırması ekleyerek titremeyi durdur */
}

/* Light Tema Özel Düzeltmeler */
:global([data-theme="light"]) .tech-badge {
  border-color: #d1cbc1; /* Açık temada daha belirgin border */
  background: #ffffff;
}

:global([data-theme="light"]) .tech-badge i {
  color: var(--primary); /* Açık temada ikonlar ateş kırmızısı/turuncu olsun */
  filter: drop-shadow(0 0 2px rgba(240, 90, 40, 0.3));
}

:global([data-theme="dark"]) .tech-badge i {
  color: #ffffff; /* Koyu temada ikonlar beyaz olsun */
  filter: drop-shadow(0 0 6px currentColor);
}

/* Hover'da Hafif Ateş Glow ve Ekstra Pürüzsüz Sıçrama */
.tech-badge::before {
  content: '';
  position: absolute;
  inset: 0;
  background: linear-gradient(135deg, var(--fire-glow-soft), transparent);
  opacity: 0;
  transition: opacity 0.6s ease;
}
.tech-badge:hover {
  translate: 0 -8px; /* GSAP Transform'u bozmadan üstüne eklenir */
  scale: 1.02;
  border-color: var(--primary);
  box-shadow: 0 15px 30px rgba(0, 0, 0, 0.1), 0 0 20px var(--fire-glow-soft); 
  background: var(--card-bg);
}
.tech-badge:hover::before {
  opacity: 1;
}

.tech-badge i {
  font-size: 1.6rem;
  position: relative;
  z-index: 1;
}
.tech-badge span {
  font-size: 1rem;
  font-weight: 600;
  color: var(--text);
  position: relative;
  z-index: 1;
  letter-spacing: 0.5px;
}


/* === 3. BÖLÜM: BİLGİ DETAYLARI === */
.info-section {
  display: grid;
  grid-template-columns: 1fr;
  gap: 30px;
}
@media (min-width: 800px) {
  .info-section {
    grid-template-columns: repeat(2, 1fr);
  }
}

.info-block {
  background: var(--card-bg);
  border: 1px solid var(--border);
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.06); /* Kartların arkaplandan ayrışmasını sağlayan gölge */
  border-radius: 24px;
  padding: 40px 32px;
  transition: border-color 0.4s ease, box-shadow 0.4s ease, translate 0.4s ease;
  will-change: transform, opacity;
  position: relative;
  overflow: hidden;
}

/* Hakkında (About) kartlarındaki o meşhur Hover Sol Çizgi Efekti */
.info-block::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  width: 4px;
  height: 100%;
  background: linear-gradient(180deg, var(--primary), var(--accent));
  opacity: 0;
  transition: opacity 0.4s ease;
}

.info-block:hover {
  translate: 0 -8px; /* Yukarı doğru tatlıca kalkma */
  border-color: var(--primary); /* Turuncu yerine ateş kırmızısı vurgu */
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.15);
}

.info-block:hover::before {
  opacity: 1;
}

.info-header {
  display: flex;
  align-items: center;
  gap: 18px;
  margin-bottom: 24px;
}

.info-icon {
  width: 52px;
  height: 52px;
  background: linear-gradient(135deg, var(--primary), var(--accent));
  border-radius: 16px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.4rem;
  color: #FFF;
  box-shadow: 0 10px 20px var(--fire-glow-soft);
}

.info-header h3 {
  font-size: 1.4rem;
  font-weight: 700;
  color: var(--text);
  margin: 0;
}

.info-text {
  color: var(--text-muted);
  font-size: 1.05rem;
  line-height: 1.85;
  margin: 0;
}

/* ÇÖZÜM MADDELERİ */
.solution-list {
  list-style: none;
  padding: 0;
  margin: 24px 0 0 0;
  display: flex;
  flex-direction: column;
  gap: 14px;
}
.solution-list li {
  position: relative;
  display: flex;
  align-items: flex-start;
  gap: 14px;
  color: var(--text);
  font-size: 1.05rem;
  line-height: 1.6;
}
.solution-list li i {
  margin-top: 5px;
  font-size: 1rem;
}

/* === 4. GERİ DÖN AKSİYONU === */
.back-action {
  margin-top: 80px;
  display: flex;
  justify-content: center;
}

.btn-back {
  display: inline-flex;
  align-items: center;
  gap: 12px;
  padding: 16px 36px;
  background: var(--card-bg);
  color: var(--text);
  font-size: 1.05rem;
  font-weight: 600;
  text-decoration: none;
  border: 1px solid var(--border);
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.04);
  border-radius: 50px;
  transition: border-color 0.4s, background 0.4s, color 0.4s, box-shadow 0.4s; /* Transform ve opacity'yi transition'dan çıkardık (GSAP kullanacak) */
  will-change: transform, opacity;
}
.btn-back:hover {
  background: var(--fire-glow-soft); /* Ateş kırmızısı hafif dokunuş */
  border-color: var(--primary);
  color: var(--hover-text-color);
  translate: -6px 0; /* Dinamik sola kayma */
  box-shadow: 0 10px 25px var(--fire-glow-soft);
}
.btn-back i {
  transition: translate 0.4s ease;
}
.btn-back:hover i {
  translate: -5px 0; /* İkon da içeriden kayar */
}

/* === RESPONSIVE (MEDYA) TASARIM KURALLARI === */
@media (max-width: 1024px) {
  .project-main-title {
    font-size: clamp(2rem, 4vw, 3.5rem);
  }
  .info-section {
    flex-direction: column;
    gap: 30px;
  }
  
  /* VİDEO BOŞLUKSUZ (BULANIK ZEMİNLE) TAM GÖSTERİM */
  .yt-video {
    min-height: auto !important;
    min-width: auto !important;
    width: 100vw !important;
    height: 56.25vw !important;
    top: 50% !important;
    transform: translate(-50%, -50%) scale(1.02) translateZ(0) !important; 
  }
  .hero-video:not(.blurred-bg), .hero-slide-img {
    object-fit: contain !important; /* Yanlardan kesilmesini engeller */
  }
}

@media (max-width: 768px) {
  .detail-container {
    padding: 40px 1.5rem 0;
  }
  .project-main-title {
    font-size: 2.2rem;
  }
  .tech-badge {
    padding: 10px 20px;
    font-size: 0.9rem;
  }
  .info-header h3 {
    font-size: 1.2rem;
  }
  .info-icon {
    width: 42px;
    height: 42px;
    font-size: 1.1rem;
  }
  .info-block {
    padding: 24px;
  }
}

@media (max-width: 480px) {
  .project-main-title {
    font-size: 1.8rem;
  }
  .detail-container {
    padding: 30px 1rem 0;
  }
  .tech-badges {
    gap: 10px;
  }
  .tech-badge {
    padding: 8px 16px;
    font-size: 0.85rem;
  }
  .yt-video {
    transform: translate(-50%, -50%) scale(1) translateZ(0); /* Sıfır kırpma */
  }
}
</style>