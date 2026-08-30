<template>
  <div class="about-page">
    <canvas id="stars-canvas"></canvas>

    <header class="page-header">
      <h1>{{ lang === 'en' ? 'About' : 'Hakkında' }}</h1>
      <p>{{ lang === 'en' ? 'Do you want to know me better?' : 'Beni daha yakından tanımak ister misiniz?' }}</p>
    </header>

    <main class="content-container">
      <div class="profile-card fade-in" v-if="settings">
        <div class="profile-image" :class="{'no-frame': activeModel3DUrl}" :style="activeModel3DUrl ? 'border: none; box-shadow: none; background: transparent; overflow: visible;' : ''" ref="tiltRef">
          <model-viewer 
            v-if="activeModel3DUrl"
            :src="getFullUrl(activeModel3DUrl)" 
            autoplay
            animation-name="Wave"
            :camera-orbit="cameraOrbit"
            shadow-intensity="1" 
            environment-image="neutral"
            class="profile-img"
            style="outline: none; background: transparent; min-height: 200px; width: 100%; pointer-events: none;"
          >
            <div slot="progress-bar"></div>
          </model-viewer>
          <img v-else-if="settings.profileImageUrl" :src="getFullUrl(settings.profileImageUrl)" alt="Profil Fotoğrafı" />
        </div>
        <div class="profile-info" style="position: relative;">
          <!-- Status Badge -->
          <div v-if="settings.isLookingForJob" class="status-badge">
            <span class="status-dot"></span>
            <div class="status-text">
              <strong>{{ (lang === 'en' && settings.cardTitleEn) ? settings.cardTitleEn : (settings.cardTitle || '1 Yıldır Sektördeyim - İş Arıyorum') }}</strong>
              <span v-if="settings.cardSubtitle" style="font-size: 0.75rem; opacity: 0.8; display: block;">{{ (lang === 'en' && settings.cardSubtitleEn) ? settings.cardSubtitleEn : settings.cardSubtitle }}</span>
            </div>
          </div>

          <h2>{{ (lang === 'en' && settings.mainTitleEn) ? settings.mainTitleEn : (settings.mainTitle || 'Hakkımda') }}</h2>
          <p class="title">{{ (lang === 'en' && settings.subTitleEn) ? settings.subTitleEn : settings.subTitle }}</p>
          <p class="bio">
            {{ (lang === 'en' && settings.bioEn) ? settings.bioEn : settings.bio }}
          </p>
          <a v-if="settings.resumeUrl" :href="getFullUrl(settings.resumeUrl)" target="_blank" class="btn btn-primary" style="margin-top: 15px; display: inline-block;">Özgeçmişi İndir</a>
        </div>
      </div>

      <div class="info-grid" v-if="settings && settings.cards">
        <div class="info-card fade-in" v-for="item in sortedItems" :key="item.id">
          <i v-if="item.icon && (item.icon.includes('fa-') || item.icon.includes('ph-'))" :class="item.icon + ' card-icon'" style="color: #ff4d00;"></i>
          <span v-else-if="item.icon" class="card-icon" style="font-size: 2.5rem; display: block; margin-bottom: 15px;">{{ item.icon }}</span>
          <i v-else class="fa-solid fa-star card-icon" style="color: #ff4d00;"></i>
          <h3>{{ (lang === 'en' && item.titleEn) ? item.titleEn : item.title }}</h3>
          
          <template v-if="item.cardType === 2 && ((lang === 'en' && item.listItemsEn && item.listItemsEn.length) || (item.listItems && item.listItems.length))">
            <ul style="list-style: none; padding: 0; margin-top: 15px; text-align: left; display: flex; flex-direction: column; gap: 12px;">
              <li v-for="(listItem, i) in (lang === 'en' && item.listItemsEn && item.listItemsEn.length ? item.listItemsEn : item.listItems)" :key="i" style="padding-left: 20px; position: relative; font-size: 0.95rem; line-height: 1.5; color: var(--text);">
                <span style="position: absolute; left: 0; top: 2px; color: #ff4d00; font-size: 1.2rem;">&rsaquo;</span>
                {{ listItem }}
              </li>
            </ul>
          </template>
          <template v-else>
            <p style="margin-top: 10px; line-height: 1.6;">{{ (lang === 'en' && item.textEn) ? item.textEn : item.text }}</p>
          </template>
        </div>
      </div>
      <div v-else-if="!settings" class="text-center" style="padding: 2rem; color: #888;">
        <p>Hakkımda bilgisi yükleniyor...</p>
      </div>
    </main>
  </div>
</template>

<script setup>
import { onMounted, onBeforeUnmount, ref, computed, nextTick, inject } from 'vue'
import VanillaTilt from 'vanilla-tilt'
import api from '@/services/api'


const lang = inject('lang', ref('tr'))
const theme = inject('theme', ref('dark'))

const tiltRef = ref(null)
const settings = ref(null)
const homeSettings = ref(null)
const cameraOrbit = ref('0deg 85deg 75%')

const activeModel3DUrl = computed(() => {
  if (!homeSettings.value) return null;
  if (theme.value === 'light') {
    return homeSettings.value.model3DUrlLight || null;
  }
  return homeSettings.value.model3DUrl || null;
});

const handleModelTracking = (e) => {
  if (!activeModel3DUrl.value) return;
  const mouseX = (e.clientX / window.innerWidth) * 2 - 1;
  const mouseY = (e.clientY / window.innerHeight) * 2 - 1;
  const degX = -mouseX * 45;
  const degY = 85 - (mouseY * 15);
  cameraOrbit.value = `${degX}deg ${degY}deg 75%`;
}

const getFullUrl = (url) => {
  if (!url) return ''
  if (url.startsWith('http') || url.startsWith('data:')) return url
  return api.defaults.baseURL.replace('/api', '') + url
}

const fetchSettings = async () => {
  try {
    const res = await api.get('/AboutSettings')
    settings.value = res.data

    try {
      const homeRes = await api.get('/HomeSettings')
      homeSettings.value = homeRes.data
    } catch (e) {
      console.warn('Home settings (for 3D model) could not be loaded on About page')
    }
    
    // VanillaTilt init after DOM updates
    nextTick(() => {
      if (tiltRef.value && !homeSettings.value?.model3DUrl) {
        VanillaTilt.init(tiltRef.value, {
          max: 10,
          speed: 400,
          glare: true,
          "max-glare": 0.1,
          scale: 1.02,
          gyroscope: true
        })
      }
      
      setTimeout(() => {
        const elements = document.querySelectorAll('.about-page .fade-in');
        elements.forEach(el => el.classList.add('visible'));
      }, 100);
    })
  } catch (error) {
    console.error('Hakkında verisi çekilemedi:', error)
  }
}

const sortedItems = computed(() => {
  if (!settings.value || !settings.value.cards) return []
  return [...settings.value.cards]
})

onMounted(() => {
  fetchSettings()
  window.addEventListener('mousemove', handleModelTracking)
})

onBeforeUnmount(() => {
  window.removeEventListener('mousemove', handleModelTracking)
  if (tiltRef.value && tiltRef.value.vanillaTilt) {
    tiltRef.value.vanillaTilt.destroy()
  }
})
</script>
<style scoped>
/* Mevcut stillere ek olarak status badge stilleri */
.status-badge {
  position: absolute;
  top: 0;
  right: 0;
  display: flex;
  align-items: center;
  gap: 0.75rem;
  background: rgba(255, 77, 0, 0.1);
  border: 1px solid rgba(255, 77, 0, 0.3);
  padding: 0.5rem 1rem;
  border-radius: 50px;
  backdrop-filter: blur(8px);
}

.status-dot {
  width: 10px;
  height: 10px;
  background-color: #ff4d00;
  border-radius: 50%;
  box-shadow: 0 0 10px #ff4d00;
  animation: pulse-dot 2s infinite;
}

@keyframes pulse-dot {
  0% { transform: scale(0.95); box-shadow: 0 0 0 0 rgba(255, 77, 0, 0.7); }
  70% { transform: scale(1); box-shadow: 0 0 0 6px rgba(255, 77, 0, 0); }
  100% { transform: scale(0.95); box-shadow: 0 0 0 0 rgba(255, 77, 0, 0); }
}

.status-text strong {
  color: var(--text);
  font-size: 0.85rem;
  font-weight: 600;
  letter-spacing: 0.02em;
}

@media (max-width: 768px) {
  .status-badge {
    position: relative;
    top: auto;
    right: auto;
    margin-bottom: 1.5rem;
    display: inline-flex;
  }
}
</style>
