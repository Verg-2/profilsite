<template>
  <div class="projects-page">
    <canvas id="particles-canvas"></canvas>

    <header class="page-header">
      <h1>{{ lang === 'en' ? 'My Projects' : 'Projelerim' }}</h1>
      <p>{{ lang === 'en' ? 'Projects I have worked on and completed. Each is carefully designed and developed.' : 'Yaptığım ve üzerinde çalıştığım projeler. Her biri özenle tasarlanmış ve geliştirilmiştir.' }}</p>
    </header>

    <main class="projects-container">
      <!-- MASAÜSTÜ FİLTRELEME (Hap Tasarım) -->
      <div class="filter-bar desktop-filters">
        <button class="filter-btn" :class="{ active: currentFilter === 'Tümü' }" @click="currentFilter = 'Tümü'"><span>{{ lang === 'en' ? 'All' : 'Tümü' }}</span></button>
        <button v-for="cat in categories" :key="cat.id" class="filter-btn" :class="{ active: currentFilter === cat.name }" @click="currentFilter = cat.name">
          <span><i v-if="cat.icon" :class="cat.icon" style="margin-right: 4px;"></i>{{ lang === 'en' && cat.nameEn ? cat.nameEn : cat.name }}</span>
        </button>
      </div>

      <!-- MOBİL FİLTRELEME (Custom Dropdown Tasarımı) -->
      <div class="mobile-filters">
        <div class="custom-dropdown" :class="{ 'is-open': isDropdownOpen }" @click="toggleDropdown">
          <i class="fa-solid fa-layer-group filter-icon"></i>
          <span class="selected-text">{{ currentFilter === 'Tümü' ? (lang === 'en' ? 'All Categories' : 'Tüm Kategoriler') : (lang === 'en' && categories.find(c => c.name === currentFilter)?.nameEn ? categories.find(c => c.name === currentFilter).nameEn : currentFilter) }}</span>
          <i class="fa-solid fa-chevron-down arrow-icon" :style="{ transform: isDropdownOpen ? 'rotate(180deg)' : 'rotate(0)' }"></i>
          
          <transition name="dropdown">
            <div class="dropdown-menu" v-if="isDropdownOpen">
              <div class="dropdown-item" :class="{ active: currentFilter === 'Tümü' }" @click.stop="selectCategory('Tümü')">{{ lang === 'en' ? 'All Categories' : 'Tüm Kategoriler' }}</div>
              <div v-for="cat in categories" :key="cat.id" class="dropdown-item" :class="{ active: currentFilter === cat.name }" @click.stop="selectCategory(cat.name)">{{ lang === 'en' && cat.nameEn ? cat.nameEn : cat.name }}</div>
            </div>
          </transition>
        </div>
      </div>

      <div class="projects-grid" v-if="projects.length > 0">
        <article class="project-card fade-in" v-for="(project, index) in filteredProjects" :key="project.id + '-' + currentFilter">
          <div class="project-image swiper-image-container">
            <Swiper
              :loop="true"
              @swiper="onSwiperInit"
              :pagination="{ clickable: true, dynamicBullets: true }"
              :effect="'creative'"
              :creativeEffect="{
                prev: { shadow: true, translate: ['-20%', 0, -1] },
                next: { translate: ['100%', 0, 0] },
              }"
              :speed="800"
              :observer="true"
              :observeParents="true"
              class="project-swiper"
            >
              <SwiperSlide v-for="(img, idx) in getSliderImages(project)" :key="idx">
                <div class="slide-inner">
                  <!-- Skeleton ve Lazy Load CLS Önlemi -->
                  <img :src="getFullUrl(img)" :alt="project.imageAltText || project.title" loading="lazy" style="aspect-ratio: 16/9; background-color: rgba(255, 255, 255, 0.05);" class="swiper-slide-img" />
                  <div class="slide-overlay"></div>
                </div>
              </SwiperSlide>
            </Swiper>
          </div>
          <div class="project-content">
            <h3 class="project-title">{{ lang === 'en' && project.titleEn ? project.titleEn : project.title }}</h3>
            <p class="project-desc">{{ lang === 'en' && project.summaryEn ? project.summaryEn : project.summary }}</p>
            <div class="project-tags">
              <span class="project-tag" v-for="tag in project.techTags" :key="tag">{{ tag.includes('|') ? tag.split('|')[1] : tag }}</span>
            </div>
            <router-link :to="'/proje/' + project.id" class="btn btn-secondary card-action-btn">{{ lang === 'en' ? 'View Project →' : 'Projeyi Görüntüle →' }}</router-link>
          </div>
        </article>
      </div>
      <div v-else class="text-center" style="padding: 2rem; color: #888;">
        <p>{{ lang === 'en' ? 'No projects added yet.' : 'Henüz proje eklenmemiş.' }}</p>
      </div>
    </main>
  </div>
</template>

<script setup>
import { ref, onMounted, computed, watch, nextTick, onBeforeUnmount, inject } from 'vue';
import { Swiper, SwiperSlide } from 'swiper/vue';
import { Pagination, EffectCreative } from 'swiper/modules';
import api from '@/services/api';
import defaultImg from '@/assets/img/wolff.png';

const lang = inject('lang', ref('tr'));

const currentFilter = ref('Tümü');
const categories = ref([]);
const currentTheme = ref(document.documentElement.getAttribute('data-theme') || 'dark');
let themeObserver = null;

const isDropdownOpen = ref(false);
const toggleDropdown = () => {
  isDropdownOpen.value = !isDropdownOpen.value;
};
const selectCategory = (catName) => {
  currentFilter.value = catName;
  isDropdownOpen.value = false;
};

const projects = ref([]);

const swiperInstances = ref([]);
let globalSlideInterval = null;

const onSwiperInit = (swiper) => {
  swiperInstances.value.push(swiper);
};

const startGlobalSlider = () => {
  if (globalSlideInterval) clearInterval(globalSlideInterval);
  let currentCardIndex = 0;
  
  globalSlideInterval = setInterval(() => {
    if (swiperInstances.value.length === 0) return;
    
    // Geçerli kartın swiper'ını al
    const swiper = swiperInstances.value[currentCardIndex];
    if (swiper && !swiper.destroyed) {
      swiper.slideNext();
    }
    
    // Bir sonraki karta geç, sona gelirse başa dön
    currentCardIndex = (currentCardIndex + 1) % swiperInstances.value.length;
  }, 1200); // Her 1.2 saniyede bir sonraki kart kayar
};

watch(currentFilter, async () => {
  await nextTick();
  // Filtre değişince swiper listesini sıfırla ve yeniden topla
  swiperInstances.value = [];
  
  setTimeout(() => {
    const elements = document.querySelectorAll('.projects-grid .fade-in');
    elements.forEach(el => el.classList.add('visible'));
  }, 50);
});

const getFullUrl = (url) => {
  if (!url) return defaultImg;
  if (url.startsWith('http') || url.startsWith('data:')) return url;
  return api.defaults.baseURL.replace('/api', '') + url;
}

const getSliderImages = (project) => {
  let targetArray = project.imageUrls || [];
  if (currentTheme.value === 'light' && project.lightImageUrls && project.lightImageUrls.length > 0) {
    targetArray = project.lightImageUrls;
  } else if (currentTheme.value === 'dark' && project.darkImageUrls && project.darkImageUrls.length > 0) {
    targetArray = project.darkImageUrls;
  }
  
  let images = targetArray.length > 0 ? [...targetArray] : [defaultImg];
  // Eğer sadece 1 görsel varsa, slider efekti çalışsın diye onu çoğaltıyoruz.
  if (images.length === 1) {
    images.push(images[0]);
    images.push(images[0]); // 3 tane olsun ki slider güzel dönsün
  }
  return images;
}

const fetchProjects = async () => {
  try {
    const [projRes, catRes] = await Promise.all([
      api.get('/Projects'),
      api.get('/Projects/categories')
    ]);
    projects.value = projRes.data.map(p => {
      p.category = catRes.data.find(c => c.id === p.projectCategoryId);
      return p;
    });
    categories.value = catRes.data;
    
    // Veriler yüklendikten sonra slider döngüsünü başlat
    setTimeout(() => {
      startGlobalSlider();
      
      // İlk açılışta kartların görünür olmasını sağla
      const elements = document.querySelectorAll('.projects-grid .fade-in');
      elements.forEach(el => el.classList.add('visible'));
    }, 500);
  } catch (error) {
    console.error("Projeler ve kategoriler yüklenirken hata oluştu", error);
  }
}

// Komponent yok edildiğinde interval'i temizle
onBeforeUnmount(() => {
  if (globalSlideInterval) clearInterval(globalSlideInterval);
  if (themeObserver) themeObserver.disconnect();
});

onMounted(() => {
  themeObserver = new MutationObserver(() => {
    currentTheme.value = document.documentElement.getAttribute('data-theme') || 'dark';
  });
  themeObserver.observe(document.documentElement, { attributes: true, attributeFilter: ['data-theme'] });

  fetchProjects();
});

const filteredProjects = computed(() => {
  if (currentFilter.value === 'Tümü') return projects.value;
  return projects.value.filter(p => p.category?.name === currentFilter.value); 
});

// Swiper Stilleri
import 'swiper/css';
import 'swiper/css/pagination';
import 'swiper/css/effect-creative';
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
