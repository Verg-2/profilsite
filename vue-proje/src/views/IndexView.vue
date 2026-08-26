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
          <div class="image-frame" ref="tiltRef">
            <model-viewer 
              v-if="settings.model3DUrl"
              :src="getFullUrl(settings.model3DUrl)" 
              auto-rotate 
              camera-controls 
              shadow-intensity="1" 
              environment-image="neutral"
              class="profile-img"
              id="profile-img"
              style="outline: none; cursor: grab; background: transparent;"
            ></model-viewer>
            <img v-else :src="getFullUrl(settings.profileImageUrl) || defaultImg" alt="Profil Fotoğrafı" class="profile-img" id="profile-img" />
            <div class="image-glow" id="image-glow"></div>
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

// Fire canvas ref for animations.js
const fireCanvas = ref(null)
const tiltRef = ref(null)
const settings = ref(null)

const preTitle = computed(() => lang.value === 'en' && settings.value?.preTitleEn ? settings.value.preTitleEn : settings.value?.preTitle)
const heroTitle = computed(() => lang.value === 'en' && settings.value?.heroTitleEn ? settings.value.heroTitleEn : settings.value?.heroTitle)
const heroSubtitle = computed(() => lang.value === 'en' && settings.value?.heroSubtitleEn ? settings.value.heroSubtitleEn : settings.value?.heroSubtitle)
const buttonText = computed(() => lang.value === 'en' && settings.value?.buttonTextEn ? settings.value.buttonTextEn : settings.value?.buttonText)
const secondaryButtonText = computed(() => lang.value === 'en' && settings.value?.secondaryButtonTextEn ? settings.value.secondaryButtonTextEn : settings.value?.secondaryButtonText)

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
  
  if (tiltRef.value) {
    VanillaTilt.init(tiltRef.value, {
      max: 15,
      speed: 400,
      glare: true,
      "max-glare": 0.3,
      scale: 1.05,
      gyroscope: true
    })
  }
})

onBeforeUnmount(() => {
  if (tiltRef.value && tiltRef.value.vanillaTilt) {
    tiltRef.value.vanillaTilt.destroy()
  }
})
</script>

