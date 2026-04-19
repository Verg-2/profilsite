<template>
  <div class="projects-page">
    <canvas id="particles-canvas"></canvas>

    <header class="page-header">
      <h1>Projelerim</h1>
      <p>Yaptığım ve üzerinde çalıştığım projeler. Her biri özenle tasarlanmış ve geliştirilmiştir.</p>
    </header>

    <main class="projects-container">
      <div class="filter-bar">
        <button class="filter-btn" :class="{ active: currentFilter === 'Tümü' }" @click="currentFilter = 'Tümü'"><span>Tümü</span></button>
        <button class="filter-btn" :class="{ active: currentFilter === 'Web' }" @click="currentFilter = 'Web'"><span>Web</span></button>
        <button class="filter-btn" :class="{ active: currentFilter === 'Mobil' }" @click="currentFilter = 'Mobil'"><span>Mobil</span></button>
        <button class="filter-btn" :class="{ active: currentFilter === 'Tasarım' }" @click="currentFilter = 'Tasarım'"><span>Tasarım</span></button>
      </div>

      <div class="projects-grid">
        <article class="project-card fade-in" v-show="currentFilter === 'Tümü' || currentFilter === 'Web'">
          <div class="project-image swiper-image-container">
            <Swiper
              @swiper="(swiper) => setSwiperRef(0, swiper)"
              :modules="[Pagination, EffectCreative]"
              :slides-per-view="1"
              :loop="true"
              :pagination="{ clickable: true, dynamicBullets: true }"
              :effect="'creative'"
              :creativeEffect="{
                prev: { shadow: true, translate: ['-20%', 0, -1] },
                next: { translate: ['100%', 0, 0] },
              }"
              :speed="800"
              class="project-swiper"
            >
              <SwiperSlide v-for="(img, idx) in portfolioImages" :key="idx">
                <div class="slide-inner">
                  <img :src="img" alt="Kişisel Portföy Ekran Görüntüsü" class="swiper-slide-img" />
                  <div class="slide-overlay"></div>
                </div>
              </SwiperSlide>
            </Swiper>
          </div>
          <div class="project-content">
            <h3 class="project-title">Kişisel Portföy Sitesi</h3>
            <p class="project-desc">Modern ve responsive tasarıma sahip kişisel portföy web sitesi.</p>
            <div class="project-tags">
              <span class="project-tag">HTML</span><span class="project-tag">CSS</span><span class="project-tag">JavaScript</span>
            </div>
            <router-link to="/proje/kisisel-portfoy" class="project-link">Projeyi Görüntüle →</router-link>
          </div>
        </article>

        <article class="project-card fade-in" v-show="currentFilter === 'Tümü' || currentFilter === 'Web'">
          <div class="project-image swiper-image-container">
            <Swiper
              @swiper="(swiper) => setSwiperRef(1, swiper)"
              :modules="[Pagination, EffectCreative]"
              :slides-per-view="1"
              :loop="true"
              :pagination="{ clickable: true, dynamicBullets: true }"
              :effect="'creative'"
              :creativeEffect="{
                prev: { shadow: true, translate: ['-20%', 0, -1] },
                next: { translate: ['100%', 0, 0] },
              }"
              :speed="800"
              class="project-swiper"
            >
              <SwiperSlide v-for="(img, idx) in ecommerceImages" :key="idx">
                <div class="slide-inner">
                  <img :src="img" alt="E-Ticaret Ekran Görüntüsü" class="swiper-slide-img" />
                  <div class="slide-overlay"></div>
                </div>
              </SwiperSlide>
            </Swiper>
          </div>
          <div class="project-content">
            <h3 class="project-title">E-Ticaret Platformu</h3>
            <p class="project-desc">Tam kapsamlı e-ticaret çözümü.</p>
            <div class="project-tags"><span class="project-tag">Vue.js</span><span class="project-tag">Node.js</span></div>
            <router-link to="/proje/e-ticaret" class="project-link">Projeyi Görüntüle →</router-link>
          </div>
        </article>

        <article class="project-card fade-in" v-show="currentFilter === 'Tümü' || currentFilter === 'Mobil'">
          <div class="project-image swiper-image-container">
            <Swiper
              @swiper="(swiper) => setSwiperRef(2, swiper)"
              :modules="[Pagination, EffectCreative]"
              :slides-per-view="1"
              :loop="true"
              :pagination="{ clickable: true, dynamicBullets: true }"
              :effect="'creative'"
              :creativeEffect="{
                prev: { shadow: true, translate: ['-20%', 0, -1] },
                next: { translate: ['100%', 0, 0] },
              }"
              :speed="800"
              class="project-swiper"
            >
              <SwiperSlide v-for="(img, idx) in bankImages" :key="idx">
                <div class="slide-inner">
                  <img :src="img" alt="Mobil Banka Ekran Görüntüsü" class="swiper-slide-img" />
                  <div class="slide-overlay"></div>
                </div>
              </SwiperSlide>
            </Swiper>
          </div>
          <div class="project-content">
            <h3 class="project-title">Mobil Banka Uygulaması</h3>
            <p class="project-desc">Cross-platform mobil banka uygulaması.</p>
            <div class="project-tags"><span class="project-tag">React Native</span></div>
            <router-link to="/proje/mobil-banka" class="project-link">Projeyi Görüntüle →</router-link>
          </div>
        </article>

        <article class="project-card fade-in" v-show="currentFilter === 'Tümü' || currentFilter === 'Web'">
          <div class="project-image swiper-image-container">
            <Swiper
              @swiper="(swiper) => setSwiperRef(3, swiper)"
              :modules="[Pagination, EffectCreative]"
              :slides-per-view="1"
              :loop="true"
              :pagination="{ clickable: true, dynamicBullets: true }"
              :effect="'creative'"
              :creativeEffect="{
                prev: { shadow: true, translate: ['-20%', 0, -1] },
                next: { translate: ['100%', 0, 0] },
              }"
              :speed="800"
              class="project-swiper"
            >
              <SwiperSlide v-for="(img, idx) in taskImages" :key="idx">
                <div class="slide-inner">
                  <img :src="img" alt="Görev API Ekran Görüntüsü" class="swiper-slide-img" />
                  <div class="slide-overlay"></div>
                </div>
              </SwiperSlide>
            </Swiper>
          </div>
          <div class="project-content">
            <h3 class="project-title">Görev Yönetim App</h3>
            <p class="project-desc">Kişisel görev listesi uygulaması.</p>
            <div class="project-tags"><span class="project-tag">Vue.js</span></div>
            <router-link to="/proje/gorev-yonetim" class="project-link">Projeyi Görüntüle →</router-link>
          </div>
        </article>

        <article class="project-card fade-in" v-show="currentFilter === 'Tümü' || currentFilter === 'Tasarım'">
          <div class="project-image swiper-image-container">
            <Swiper
              @swiper="(swiper) => setSwiperRef(4, swiper)"
              :modules="[Pagination, EffectCreative]"
              :slides-per-view="1"
              :loop="true"
              :pagination="{ clickable: true, dynamicBullets: true }"
              :effect="'creative'"
              :creativeEffect="{
                prev: { shadow: true, translate: ['-20%', 0, -1] },
                next: { translate: ['100%', 0, 0] },
              }"
              :speed="800"
              class="project-swiper"
            >
              <SwiperSlide v-for="(img, idx) in uiuxImages" :key="idx">
                <div class="slide-inner">
                  <img :src="img" alt="UI/UX Tasarım Ekranı" class="swiper-slide-img" />
                  <div class="slide-overlay"></div>
                </div>
              </SwiperSlide>
            </Swiper>
          </div>
          <div class="project-content">
            <h3 class="project-title">UI/UX Tasarım Seti</h3>
            <p class="project-desc">Modern web siteleri için UI bileşenleri.</p>
            <div class="project-tags"><span class="project-tag">Figma</span></div>
            <router-link to="/proje/ui-ux" class="project-link">Projeyi Görüntüle →</router-link>
          </div>
        </article>

        <article class="project-card fade-in" v-show="currentFilter === 'Tümü' || currentFilter === 'Mobil'">
          <div class="project-image swiper-image-container">
            <Swiper
              @swiper="(swiper) => setSwiperRef(5, swiper)"
              :modules="[Pagination, EffectCreative]"
              :slides-per-view="1"
              :loop="true"
              :pagination="{ clickable: true, dynamicBullets: true }"
              :effect="'creative'"
              :creativeEffect="{
                prev: { shadow: true, translate: ['-20%', 0, -1] },
                next: { translate: ['100%', 0, 0] },
              }"
              :speed="800"
              class="project-swiper"
            >
              <SwiperSlide v-for="(img, idx) in chatImages" :key="idx">
                <div class="slide-inner">
                  <img :src="img" alt="Sohbet App Ekranı" class="swiper-slide-img" />
                  <div class="slide-overlay"></div>
                </div>
              </SwiperSlide>
            </Swiper>
          </div>
          <div class="project-content">
            <h3 class="project-title">Sohbet Uygulaması</h3>
            <p class="project-desc">Real-time sohbet uygulaması.</p>
            <div class="project-tags"><span class="project-tag">Flutter</span></div>
            <router-link to="/proje/sohbet-app" class="project-link">Projeyi Görüntüle →</router-link>
          </div>
        </article>
      </div>
    </main>
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from 'vue';
import { Swiper, SwiperSlide } from 'swiper/vue';
import { Pagination, EffectCreative } from 'swiper/modules';

const currentFilter = ref('Tümü');

// Swiper Stilleri
import 'swiper/css';
import 'swiper/css/pagination';
import 'swiper/css/effect-creative';

// Swiper instancelarını tutacağımız dizi ve fonksiyon
const swipers = ref([]);
const setSwiperRef = (index, swiper) => {
  swipers.value[index] = swiper;
};

let autoSlideInterval = null;
let currentSwiperIndex = 0;

onMounted(() => {
  // Her 2.5 saniyede bir, sistem sadece tek bir karttaki slider'ı kaydırır
  // Sıra sürekli 1 -> 2 -> 3 -> ... -> 6 -> 1 olarak devirdaim yapar.
  autoSlideInterval = setInterval(() => {
    if (swipers.value.length > 0) {
      const swiper = swipers.value[currentSwiperIndex];
      
      // Eğer bu kartın swiper'ı sorunsuz yüklendiyse slaytı kaydırır
      if (swiper && !swiper.destroyed) {
        swiper.slideNext();
      }
      
      // Bir sonraki tura diğer karta geçmesi için indeksi artırıyoruz
      currentSwiperIndex++;
      if (currentSwiperIndex >= swipers.value.length) {
        currentSwiperIndex = 0; // Sona ulaştıysa başa sar
      }
    }
  }, 1200); // Akış hızı (1.2 saniye - Daha hızlı akış)
});

onUnmounted(() => {
  // Bileşen (sayfa) yok edilirken temizle (performans opt.)
  if (autoSlideInterval) {
    clearInterval(autoSlideInterval);
  }
});

// Örnek ekran görüntüleri (Portföy sitenize ait resim yollarıyla değiştirebilirsiniz)
const portfolioImages = ref([
  'https://picsum.photos/id/1018/600/400',
  'https://picsum.photos/id/1019/600/400',
  'https://picsum.photos/id/1020/600/400'
]);

const ecommerceImages = ref([
  'https://picsum.photos/id/1021/600/400',
  'https://picsum.photos/id/1022/600/400'
]);

const bankImages = ref([
  'https://picsum.photos/id/1023/600/400',
  'https://picsum.photos/id/1024/600/400'
]);

const taskImages = ref([
  'https://picsum.photos/id/1025/600/400',
  'https://picsum.photos/id/1026/600/400'
]);

const uiuxImages = ref([
  'https://picsum.photos/id/1027/600/400',
  'https://picsum.photos/id/1028/600/400'
]);

const chatImages = ref([
  'https://picsum.photos/id/1029/600/400',
  'https://picsum.photos/id/1031/600/400'
]);

// Animasyonlar page-animations.js ile yönetiliyor
</script>

<style scoped>
/* Swiper Konteynerı */
.swiper-image-container {
  width: 100%;
  height: 220px; /* Fotoğraflar daha estetik görünsün diye genişletildi */
  padding: 0;
  display: block; 
  background: #111115; /* Siyah arkaplan gap çizgisini yumuşatır */
  overflow: hidden;
  border-radius: 20px 20px 0 0;
  transform: translateZ(0); /* Bazı tarayıcılardaki 1px boşluk titremesini engeller */
}



.project-swiper {
  width: 100%;
  height: 100%;
  border-radius: 20px 20px 0 0; /* Sadece en üst iki kenar yuvarlak */
}

/* Resim Kapsayıcısı & Overlay */
.slide-inner {
  position: relative;
  width: 100%;
  height: 100%;
  border-radius: 20px 20px 0 0;
  overflow: hidden;
}

.swiper-slide-img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  display: block; /* display: block olduğu için alt boşluk zaten kalkar, vertical-align uyarısını kaldırdık */
  transform: scale(1.05); /* Ufak zoom animasyonuna başlangıç payı */
  transition: transform 4s ease-out; /* Kayarken minik bir zoom out hissi */
}

.swiper-slide-active .swiper-slide-img {
  transform: scale(1);
}

/* Alttan yukarı doğru gradient ile noktalara harika bir zemin hazırlıyoruz */
.slide-overlay {
  position: absolute;
  inset: 0;
  background: linear-gradient(0deg, rgba(11, 11, 15, 0.9) 0%, rgba(11, 11, 15, 0.2) 40%, transparent 100%);
  pointer-events: none;
}

/* Pagination (Noktalar) İçin Ateş Kırmızısı Glow Teması */
:deep(.swiper-pagination) {
  bottom: 8px !important; /* Noktaları biraz daha yukarı al, ince ayar */
}

:deep(.swiper-pagination-bullet) {
  background: rgba(255, 255, 255, 0.3);
  opacity: 1;
  width: 6px;
  height: 6px;
  transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
}

:deep(.swiper-pagination-bullet-active) {
  background: #FF3B1D;
  width: 18px; /* Aktif noktayı uzatarak bir çizgi/hap formuna sok (modern) */
  border-radius: 4px;
  box-shadow: 0 0 12px rgba(255, 59, 29, 0.8), 0 0 20px rgba(255, 59, 29, 0.4); /* Glow efekti artırıldı */
  transform: scale(1);
}
</style>
