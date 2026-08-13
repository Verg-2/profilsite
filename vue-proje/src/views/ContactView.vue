<template>
  <div class="contact-page">
    <canvas id="particles-canvas"></canvas>

    <div class="bg-lines">
      <div class="bg-line"></div>
      <div class="bg-line"></div>
      <div class="bg-line"></div>
      <div class="bg-line"></div>
    </div>

    <header class="page-header">
      <h1>{{ lang === 'en' ? 'Contact Me' : 'İletişime Geç' }}</h1>
      <p>{{ lang === 'en' ? 'Would you like to collaborate on a project or just say hi?' : 'Bir proje için işbirliği yapmak veya sadece selam vermek ister misin?' }}</p>
    </header>

    <main class="contact-container">
      <div class="contact-section">
        <component
          v-for="card in contactCards"
          :key="card.id"
          :is="card.url ? 'a' : 'div'"
          :href="card.url ? card.url : null"
          :target="card.url ? '_blank' : null"
          :rel="card.url ? 'noopener noreferrer' : null"
          class="contact-card fade-in"
        >
          <div class="contact-icon" style="font-size: 1.5rem; display: flex; align-items: center; justify-content: center;">
            <span v-if="card.icon && card.icon.startsWith('<svg')" v-safe-html="card.icon" style="width: 1em; height: 1em; display: inline-flex; align-items: center; justify-content: center;"></span>
            <i v-else :class="card.icon"></i>
          </div>
          <div class="contact-info">
            <h3>{{ lang === 'en' && card.titleEn ? card.titleEn : card.title }}</h3>
            <p>{{ lang === 'en' && card.subtitleEn ? card.subtitleEn : card.subtitle }}</p>
          </div>
        </component>
      </div>

      <form class="form-card fade-in" id="contact-form" @submit.prevent="handleSubmit">
        <h2 class="form-title">{{ lang === 'en' ? 'Send a Message' : 'Mesaj Gönder' }}</h2>

        <p v-if="state.successMessage" class="form-success">{{ state.successMessage }}</p>
        <p v-if="state.errorMessage" class="form-error">{{ state.errorMessage }}</p>

        <div class="form-group">
          <label for="name">{{ lang === 'en' ? 'First Name' : 'Ad' }}</label>
          <input
            type="text"
            id="name"
            :placeholder="lang === 'en' ? 'Enter your first name' : 'Adınızı girin'"
            v-model="form.ad"
            required
          />
        </div>

        <div class="form-group">
          <label for="soyad">{{ lang === 'en' ? 'Last Name' : 'Soyad' }}</label>
          <input
            type="text"
            id="soyad"
            :placeholder="lang === 'en' ? 'Enter your last name' : 'Soyadınızı girin'"
            v-model="form.soyad"
            required
          />
        </div>

        <div class="form-group">
          <label for="email">{{ lang === 'en' ? 'Email' : 'E-posta' }}</label>
          <input
            type="email"
            id="email"
            :placeholder="lang === 'en' ? 'Your email address' : 'E-posta adresiniz'"
            v-model="form.email"
            required
          />
        </div>

        <div class="form-group message-group">
          <label for="message">{{ lang === 'en' ? 'Message' : 'Mesaj' }}</label>
          <textarea
            id="message"
            :placeholder="lang === 'en' ? 'Write your message...' : 'Mesajınızı yazın...'"
            rows="5"
            v-model="form.mesaj"
            required
          ></textarea>
        </div>

        <!-- Honeypot (botlar için gizli alan) -->
        <div style="display:none;">
          <label for="website">Web Sitesi</label>
          <input id="website" type="text" v-model="form.webSitesi" tabindex="-1" autocomplete="off" />
        </div>

        <button type="submit" class="submit-btn" :disabled="state.loading">
          {{ state.loading ? (lang === 'en' ? 'Sending...' : 'Gönderiliyor...') : (lang === 'en' ? 'Send' : 'Gönder') }}
        </button>
      </form>
    </main>
  </div>
</template>

<script setup>
import { reactive, ref, onMounted, inject } from 'vue'
import { appEnv } from '@/config/env'
import api from '@/services/api'

const lang = inject('lang', ref('tr'))
const contactCards = ref([])

onMounted(async () => {
  try {
    const res = await api.get('/ContactCards')
    contactCards.value = res.data || []
  } catch (err) {
    console.error('ContactCards fetch error:', err)
  }

  setTimeout(() => {
    const elements = document.querySelectorAll('.contact-page .fade-in');
    elements.forEach(el => el.classList.add('visible'));
  }, 100);
})

const API_BASE = appEnv.apiBase

const form = reactive({
  ad: '',
  soyad: '',
  email: '',
  mesaj: '',
  webSitesi: '' // honeypot
})

const state = reactive({
  loading: false,
  successMessage: '',
  errorMessage: ''
})

async function handleSubmit() {
  state.successMessage = ''
  state.errorMessage = ''

  if (!API_BASE) {
    state.errorMessage = lang.value === 'en' ? 'Server configuration is missing.' : 'Sunucu yapılandırması eksik. Lütfen VITE_API_BASE değerini .env.local dosyasında tanımlayın.'
    return
  }

  state.loading = true

  try {
    const response = await fetch(`${API_BASE}/api/iletisim/gonder`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        Ad: form.ad,
        Soyad: form.soyad,
        Email: form.email,
        Mesaj: form.mesaj,
        WebSitesi: form.webSitesi
      })
    })

    const data = await response.json().catch(() => ({}))
    if (!response.ok || data.success === false) {
      let hataMesaji = data?.mesaj || data?.title || (lang.value === 'en' ? 'Failed to send message.' : 'Mesaj gönderilemedi.')
      let detay = ''
      
      // ASP.NET Core tarzı validation hatalarını yakala
      if (data?.errors) {
        const errorList = Object.values(data.errors).flat()
        detay = ` Detay: ${errorList.join(' ')}`
      } else if (Array.isArray(data?.hatalar) && data.hatalar.length) {
        detay = ` Detay: ${data.hatalar.join(' ')}`
      }

      state.errorMessage = `${hataMesaji}${detay}`
      return
    }

    state.successMessage = data?.mesaj || (lang.value === 'en' ? 'Your message has been sent.' : 'Mesajınız iletildi.')

    form.ad = ''
    form.soyad = ''
    form.email = ''
    form.mesaj = ''
    form.webSitesi = ''
  } catch (error) {
    state.errorMessage = lang.value === 'en' ? 'Server unreachable. Please try again later.' : 'Sunucuya ulaşılamadı. Lütfen daha sonra tekrar deneyin.'
  } finally {
    state.loading = false
  }
}
</script>
