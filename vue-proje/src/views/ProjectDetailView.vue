<template>
  <div class="project-detail-page">
    <canvas id="particles-canvas"></canvas>

    <!-- 1. HERO SLIDER EKRANI (Tam Genişlik) -->
    <section class="detail-hero">
      <Swiper
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
        class="detail-hero-swiper"
      >
        <SwiperSlide v-for="(img, idx) in projectData.images" :key="idx">
          <div class="hero-slide-inner">
            <img :src="img" :alt="`${projectData.title} Ekran Görüntüsü`" class="hero-slide-img" />
          </div>
        </SwiperSlide>
      </Swiper>
    </section>

    <!-- İÇERİK ALANI (Z-Index: 2, Slider'ın üstüne biner) -->
    <div class="content-section">
      <!-- Duman/Erime Efekti İçin Gradient Maske -->
      <div class="fade-mask"></div>

      <main class="detail-container">

        <!-- YENİ: MODERN BAŞLIK VE KATEGORİ ALANI -->
        <header class="project-header">
        <span class="project-category">{{ projectData.category }}</span>
        <h1 class="project-main-title">{{ projectData.title }}</h1>
      </header>
      
      <!-- 2. TEKNOLOJİ ROZETLERİ (Glow Badges) -->
      <section class="tech-section">
        <h2 class="section-heading">Kullanılan Teknolojiler</h2>
        <div class="tech-badges">
          <div v-for="(tech, tIdx) in projectData.technologies" :key="tIdx" class="tech-badge">
            <i :class="tech.icon" :style="{ color: tech.color }"></i>
            <span>{{ tech.name }}</span>
          </div>
        </div>
      </section>

      <!-- 3. PROJE DETAY METNİ -->
      <section class="info-section">
        
        <div class="info-block">
          <div class="info-header">
            <div class="info-icon"><i class="fa-solid fa-bullseye"></i></div>
            <h3>Projenin Amacı</h3>
          </div>
          <p class="info-text">{{ projectData.purpose }}</p>
        </div>

        <div class="info-block">
          <div class="info-header">
            <div class="info-icon"><i class="fa-solid fa-bolt"></i></div>
            <h3>Zorluklar ve Çözümler</h3>
          </div>
          <p class="info-text">{{ projectData.challenges }}</p>
          <ul class="solution-list">
             <li v-for="(sol, sIdx) in projectData.solutions" :key="sIdx">
               <i class="fa-solid fa-check" style="color: var(--accent);"></i> {{ sol }}
             </li>
          </ul>
        </div>
      </section>
      
      <!-- Geri Dönüş Aksiyonu -->
      <div class="back-action">
        <router-link to="/projects" class="btn-back">
          <i class="fa-solid fa-arrow-left-long"></i> Projelere Dön
        </router-link>
      </div>

      </main>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, nextTick, onUnmounted } from 'vue';
import { useRoute } from 'vue-router';
import { Swiper, SwiperSlide } from 'swiper/vue';
import { Autoplay, Pagination, EffectCreative } from 'swiper/modules';
import gsap from 'gsap';
import ScrollTrigger from 'gsap/ScrollTrigger';

gsap.registerPlugin(ScrollTrigger);

// Swiper Stilleri
import 'swiper/css';
import 'swiper/css/pagination';
import 'swiper/css/effect-creative';

const route = useRoute();
const projectId = route.params.id; // Router'dan dinamik ID çekiyoruz

// Gerçek hayatta bu data Vuex, Pinia veya API'den (Axios ile) gelir.
// Portfolyo için veritabanı yapısını mock (sahte) obje olarak tasarladım:
const mockDatabase = {
  'kisisel-portfoy': {
    title: 'Kişisel Portföy Sitesi',
    category: 'Web Geliştirme & UI/UX',
    images: [
      'https://picsum.photos/id/1018/1920/1080', // Büyük slider (Hero) için 1920x1080 yatay görseller
      'https://picsum.photos/id/1019/1920/1080',
      'https://picsum.photos/id/1020/1920/1080'
    ],
    technologies: [
      { name: 'Vue 3', icon: 'fa-brands fa-vuejs', color: '#42b883' },
      { name: 'Vite', icon: 'fa-solid fa-bolt', color: '#646CFF' },
      { name: 'JavaScript', icon: 'fa-brands fa-js', color: '#f7df1e' },
      { name: 'CSS3 / Variables', icon: 'fa-brands fa-css3-alt', color: '#264de4' },
      { name: 'Swiper.js', icon: 'fa-solid fa-sliders', color: '#6332f6' }
    ],
    purpose: 'Kendi yeteneklerimi, tecrübelerimi ve dijital dünyadaki ayak izimi tüm dünyaya modern ve "Dark-Tech" temalı bir kod mimarisi ile sergilemek. Marka kimliğimi (Kadir Portfolyo) en iyi şekilde yansıtacak etkileşimli, responsive ve fütüristik bir arayüz kurgulamak.',
    challenges: 'Kullanıcının sayfada gezinirken sıkılmamasını sağlayacak akıcı animasyonlar (scroll-reveal, micro-interactions, ateş efekti) tasarlamak ve bunları performans/RAM kaybı yaşamadan Vue 3 ekosistemi içinde senkronize optimize etmek en büyük teknik süreçti.',
    solutions: [
      'Vue Composition API kullanılarak reaktif ve temiz / ayrıştırılmış bir kod mimarisi oluşturuldu.',
      'Swiper.js entegrasyonunda setInterval (Orkestrasyon) sistemi kurgulanarak performans artırıldı.',
      'Özel CSS maskelemeleri (mask, -webkit-mask) ve gradient oyunlarıyla "glow" hissi veren tasarım sistemi geliştirildi.'
    ]
  },
  'e-ticaret': {
    title: 'E-Ticaret Platformu',
    category: 'Full-Stack Çözüm',
    images: ['https://picsum.photos/id/1021/1920/1080', 'https://picsum.photos/id/1022/1920/1080'],
    technologies: [
      { name: '.NET Core', icon: 'fa-brands fa-windows', color: '#512BD4' },
      { name: 'Vue.js', icon: 'fa-brands fa-vuejs', color: '#42b883' },
      { name: 'SQL Server', icon: 'fa-solid fa-database', color: '#CC2927' }
    ],
    purpose: 'Müşteriler için uçtan uca, sepetten ödemeye kadar kusursuz bir alışveriş e-ticaret deneyimi sunmak.',
    challenges: 'Ödeme entegrasyonu tarafındaki güvenlik prensipleri, sepet senkronizasyonu ve yetkilendirme (tokenlaşma).',
    solutions: ['Mimari katmanlara paylaştırıldı', 'RabbitMQ ile asenkron sipariş kuyruğu oluşturuldu.', 'Vue 3 & Pinia ile sepet reaktivitesi sağlandı.']
  },
  'mobil-banka': {
    title: 'Mobil Banka Uygulaması',
    category: 'Mobil Uygulama (Cross-Platform)',
    images: ['https://picsum.photos/id/1023/1920/1080', 'https://picsum.photos/id/1024/1920/1080'],
    technologies: [
      { name: 'React Native', icon: 'fa-brands fa-react', color: '#61DAFB' },
      { name: 'Redux', icon: 'fa-solid fa-code-branch', color: '#764ABC' },
      { name: 'Node.js API', icon: 'fa-brands fa-node', color: '#339933' }
    ],
    purpose: 'Kullanıcıların hesaplarını kolayca yönetebilecekleri, yenilikçi ve ultra korumalı (biometrik) bir Fintech / mobil bankacılık deneyimi kurgulamak.',
    challenges: 'Farklı cihazlar (iOS ve Android) arası UI tutarsızlıkları ve JWT ile güvenli REST iletişim.',
    solutions: ['React Native Reanimated ve UI kütüphaneleriyle pixel-perfect tasarım.', 'Socket.io ile anlık bildirim sistemi.', 'Biyometrik doğrulama entegrasyonu.']
  },
  'gorev-yonetim': {
    title: 'Görev Yönetim App',
    category: 'Web Uygulaması',
    images: ['https://picsum.photos/id/1025/1920/1080', 'https://picsum.photos/id/1026/1920/1080'],
    technologies: [
      { name: 'Vue 3', icon: 'fa-brands fa-vuejs', color: '#42b883' },
      { name: 'Firebase', icon: 'fa-solid fa-fire', color: '#FFCA28' },
      { name: 'Tailwind CSS', icon: 'fa-solid fa-wind', color: '#38B2AC' }
    ],
    purpose: 'Kişisel ve ekip tabanlı görevleri (To-Do) modern ve akıcı (Kanban board) bir yöntemle takip etmeyi sağlayan bulut tabanlı bir verimlilik aracı geliştirmek.',
    challenges: 'Görevlerin sürükle-bırak (Drag&Drop) yapılırken veritabanına olan anlık yansımaları ve state senkronizasyonu.',
    solutions: ['VueDraggable kütüphanesi kullanıldı.', 'Firebase Realtime Database ile sıfır gecikmeli state güncellemesi sağlandı.']
  },
  'ui-ux': {
    title: 'UI/UX Tasarım Seti',
    category: 'Arayüz / Deneyim Tasarımı',
    images: ['https://picsum.photos/id/1027/1920/1080', 'https://picsum.photos/id/1028/1920/1080'],
    technologies: [
      { name: 'Figma', icon: 'fa-brands fa-figma', color: '#F24E1E' },
      { name: 'Wireframing', icon: 'fa-solid fa-pen-ruler', color: '#A259FF' },
      { name: 'Prototyping', icon: 'fa-solid fa-wand-magic-sparkles', color: '#1ABCFE' }
    ],
    purpose: 'Gelecekteki projelerde tekrar tekrar kullanılabilecek, modern (Dark-tech) ve global UX standartlarına (Apple, Google) uygun, detaylı bir "Design System" tasarlamak.',
    challenges: 'Her elementin responsive varyasyonlarını (butonlar, kartlar, gridler) düşünmek ve marka kimliğini bozmadan tutarlı typography kurgulamak.',
    solutions: ['Figma Variables ve Auto-Layout v5 kullanılarak akıllı bileşen tipleri oluşturuldu.', 'Tasarım tokenleri (renkler, aralıklar) standardize edildi.']
  },
  'sohbet-app': {
    title: 'Sohbet Uygulaması',
    category: 'Mobil Uygulama (Real-Time)',
    images: ['https://picsum.photos/id/1029/1920/1080', 'https://picsum.photos/id/1031/1920/1080'],
    technologies: [
      { name: 'Flutter', icon: 'fa-solid fa-mobile-screen', color: '#02569B' },
      { name: 'Dart', icon: 'fa-solid fa-code', color: '#0175C2' },
      { name: 'WebRTC', icon: 'fa-solid fa-tower-broadcast', color: '#333333' }
    ],
    purpose: 'İnsanların anlık olarak mesajlaşıp görüntülü konuşabileceği, WhatsApp alternatiflerine benzer ölçeklenebilir ve cross-platform (iOS/Android/Web) bir iletişim aracı geliştirmek.',
    challenges: 'Anlık mesajların (WebSockets) verimli iletilmesi ve Görüntülü görüşmeler (P2P) için bağlantı noktası optimizasyonu.',
    solutions: ['Flutter ile tek codebase üzerinden tüm platformlar desteklendi.', 'Görüntü aktarımı için WebRTC p2p (peer-to-peer) kullanıldı.']
  }
};

// URL /proje/e-ticaret yerine tanımsız gelirse default portfolyo getir
const projectData = ref(mockDatabase[projectId] || mockDatabase['kisisel-portfoy']);

onMounted(() => {
  window.scrollTo(0, 0); // Sayfa açılınca en üste ışınlan
  
  nextTick(() => {
    // 1. HERO SLIDER PARALLAX EFEKTİ (Daha Uzun Mesafe, Tok ve Ağır)
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

    gsap.to('.detail-hero', {
      opacity: 0.1, 
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
});

onUnmounted(() => {
  ScrollTrigger.getAll().forEach(t => t.kill()); // Component kapanınca trigger'ları temizle
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
  height: 150px;
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
  filter: drop-shadow(0 0 6px currentColor); /* İkon renginde dış ışıltı atar (glow) */
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
</style>