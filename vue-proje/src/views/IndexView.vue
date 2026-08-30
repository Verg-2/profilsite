<template>
  <!-- Extracted exactly from pages/index.html <div id="app"> content -->
  <section class="hero" id="hero">
    <canvas id="fire-canvas" ref="fireCanvas"></canvas>
    <div class="hero-content">
        <div class="hero-text" v-if="settings">
          <h1 class="hero-title">
            <div v-if="preTitle" class="hero-pre-title">{{ preTitle }}</div>
            <span class="highlight">{{ heroTitle?.split('|')[0] }}</span><br v-if="heroTitle?.includes('|')">
            <span class="hero-profession" v-if="heroTitle?.includes('|')" v-safe-html="heroTitle.split('|')[1]"></span>
          </h1>
          <p class="hero-subtitle">
            {{ heroSubtitle }}
          </p>
          <div class="hero-buttons">
            <router-link :to="settings.buttonUrl || '/projects'" class="btn btn-secondary" v-if="buttonText">
              <span>{{ buttonText }}</span>
            </router-link>
            <router-link :to="settings.secondaryButtonUrl || '/contact'" class="btn btn-secondary" v-if="secondaryButtonText">
              <span>{{ secondaryButtonText }}</span>
            </router-link>
          </div>
        </div>
        <div class="hero-image" v-if="settings">
          <div class="image-frame" :class="{'no-frame': activeModel3DUrl}" :style="activeModel3DUrl ? 'border: none; box-shadow: none; background: transparent; overflow: visible;' : ''" ref="tiltRef">
            <model-viewer 
              v-if="activeModel3DUrl"
              :src="getFullUrl(activeModel3DUrl)" 
              :poster="getFullUrl(settings.profileImageUrl) || defaultImg"
              autoplay
              animation-name="Wave"
              :camera-orbit="cameraOrbit"
              shadow-intensity="1" 
              environment-image="neutral"
              class="profile-img"
              id="profile-model"
              style="outline: none; background: transparent; min-height: 200px; width: 100%; pointer-events: none;"
            >
              <div slot="progress-bar"></div>
            </model-viewer>
            <img v-else :src="getFullUrl(settings.profileImageUrl) || defaultImg" alt="Profil Fotoğrafı" class="profile-img" id="profile-img" />
            <div class="image-glow" id="image-glow" v-if="!activeModel3DUrl"></div>
          </div>
        </div>
    </div>
  </section>
</template>

<script setup>
import { onMounted, onBeforeUnmount, ref, computed, nextTick, inject } from 'vue'
import VanillaTilt from 'vanilla-tilt'
import api from '@/services/api'
import defaultImg from '@/assets/img/wolff.png'
import { initPageAnimations, cleanupPageAnimations } from '@/assets/js/page-animations'

const lang = inject('lang', ref('tr'))
const theme = inject('theme', ref('dark'))

// Fire canvas ref for animations.js
const fireCanvas = ref(null)
const tiltRef = ref(null)
const settings = ref(null)
const cameraOrbit = ref('0deg 85deg 75%')

const handleModelTracking = (e) => {
  if (!activeModel3DUrl.value) return;
  
  // Calculate relative mouse position (-1 to 1)
  const mouseX = (e.clientX / window.innerWidth) * 2 - 1;
  const mouseY = (e.clientY / window.innerHeight) * 2 - 1;
  
  // X axis: rotate left/right up to 45 degrees (Inverted to face mouse)
  const degX = -mouseX * 45;
  // Y axis: base 85 degrees, look up/down by 15 degrees (Inverted to face mouse)
  const degY = 85 - (mouseY * 15);
  
  cameraOrbit.value = `${degX}deg ${degY}deg 75%`;
}

const preTitle = computed(() => lang.value === 'en' && settings.value?.preTitleEn ? settings.value.preTitleEn : settings.value?.preTitle)
const heroTitle = computed(() => lang.value === 'en' && settings.value?.heroTitleEn ? settings.value.heroTitleEn : settings.value?.heroTitle)
const heroSubtitle = computed(() => lang.value === 'en' && settings.value?.heroSubtitleEn ? settings.value.heroSubtitleEn : settings.value?.heroSubtitle)
const buttonText = computed(() => lang.value === 'en' && settings.value?.buttonTextEn ? settings.value.buttonTextEn : settings.value?.buttonText)
const secondaryButtonText = computed(() => lang.value === 'en' && settings.value?.secondaryButtonTextEn ? settings.value.secondaryButtonTextEn : settings.value?.secondaryButtonText)

const activeModel3DUrl = computed(() => {
  if (!settings.value) return null;
  if (theme.value === 'light' && settings.value.model3DUrlLight) {
    return settings.value.model3DUrlLight;
  }
  return settings.value.model3DUrl;
});

const getFullUrl = (url) => {
  if (!url) return ''
  if (url.startsWith('http') || url.startsWith('data:')) return url
  return api.defaults.baseURL.replace('/api', '') + url
}

const formatTitle = (title) => {
  return title; // No longer used, but kept to prevent errors if referenced
}

const fetchSettings = async () => {
  if (window.__homeSettingsCache) {
    settings.value = window.__homeSettingsCache
    return
  }
  try {
    const res = await api.get('/HomeSettings')
    settings.value = res.data
    window.__homeSettingsCache = res.data
  } catch (error) {
    console.error('Anasayfa verisi çekilemedi:', error)
  }
}

onMounted(async () => {
  await fetchSettings()
  
  // DOM'un v-if="settings" ile güncellenmesini bekle
  await nextTick()
  
  // Profil resmi DOM'a eklendiği için animasyonları tekrar başlat
  cleanupPageAnimations()
  initPageAnimations()
  
  if (tiltRef.value && !settings.value?.model3DUrl) {
    VanillaTilt.init(tiltRef.value, {
      max: 15,
      speed: 400,
      glare: true,
      "max-glare": 0.3,
      scale: 1.05,
      gyroscope: true
    })
  }
  
  // Mouse tracking listener
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
.image-frame.no-frame {
  background: transparent !important;
  border: none !important;
  box-shadow: none !important;
}
</style>
