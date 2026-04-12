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
      <h1>İletişime Geç</h1>
      <p>Bir proje için işbirliği yapmak veya sadece selam vermek ister misin?</p>
    </header>

    <main class="contact-container">
      <div class="contact-section">
        <div class="contact-card fade-in">
          <div class="contact-icon">
            <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
              <path d="M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0 1 18 0z"></path>
              <circle cx="12" cy="10" r="3"></circle>
            </svg>
          </div>
          <div class="contact-info">
            <h3>Konum</h3>
            <p>Adana, Türkiye</p>
          </div>
        </div>

        <a href="https://www.linkedin.com/in/kadir-altundag" target="_blank" rel="noopener noreferrer" class="contact-card fade-in">
          <div class="contact-icon">
            <svg width="20" height="20" viewBox="0 0 24 24" fill="currentColor">
              <path d="M19 0h-14c-2.761 0-5 2.239-5 5v14c0 2.761 2.239 5 5 5h14c2.762 0 5-2.239 5-5v-14c0-2.761-2.238-5-5-5zm-11 19h-3v-11h3v11zm-1.5-12.268c-.966 0-1.75-.79-1.75-1.764s.784-1.764 1.75-1.764 1.75.79 1.75 1.764-.783 1.764-1.75 1.764zm13.5 12.268h-3v-5.604c0-3.368-4-3.113-4 0v5.604h-3v-11h3v1.765c1.396-2.586 7-2.777 7 2.476v6.759z" />
            </svg>
          </div>
          <div class="contact-info">
            <h3>LinkedIn</h3>
            <p>kadir-altundag</p>
          </div>
        </a>

        <a href="https://github.com/Verg-2" target="_blank" rel="noopener noreferrer" class="contact-card fade-in">
          <div class="contact-icon">
            <svg width="20" height="20" viewBox="0 0 24 24" fill="currentColor">
              <path d="M12 0c-6.626 0-12 5.373-12 12 0 5.302 3.438 9.8 8.207 11.387.599.111.793-.261.793-.577v-2.234c-3.338.726-4.033-1.416-4.033-1.416-.546-1.387-1.333-1.756-1.333-1.756-1.089-.745.083-.729.083-.729 1.205.084 1.839 1.237 1.839 1.237 1.07 1.834 2.807 1.304 3.492.997.107-.775.418-1.305.762-1.604-2.665-.305-5.467-1.334-5.467-5.931 0-1.311.469-2.381 1.236-3.221-.124-.303-.535-1.524.117-3.176 0 0 1.008-.322 3.301 1.23.957-.266 1.983-.399 3.003-.404 1.02.005 2.047.138 3.006.404 2.291-1.552 3.297-1.23 3.297-1.23.653 1.653.242 2.874.118 3.176.77.84 1.235 1.911 1.235 3.221 0 4.609-2.807 5.624-5.479 5.921.43.372.823 1.102.823 2.222v3.293c0 .319.192.694.801.576 4.765-1.589 8.199-6.086 8.199-11.386 0-6.627-5.373-12-12-12z" />
            </svg>
          </div>
          <div class="contact-info">
            <h3>GitHub</h3>
            <p>github.com/Verg-2</p>
          </div>
        </a>

        <a href="https://www.instagram.com/kadiraltundag__" target="_blank" rel="noopener noreferrer" class="contact-card fade-in">
          <div class="contact-icon">
            <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
              <rect x="2" y="2" width="20" height="20" rx="5" ry="5"></rect>
              <path d="M16 11.37A4 4 0 1 1 12.63 8 4 4 0 0 1 16 11.37z"></path>
              <line x1="17.5" y1="6.5" x2="17.51" y2="6.5"></line>
            </svg>
          </div>
          <div class="contact-info">
            <h3>Instagram</h3>
            <p>instagram.com/kadiraltundag__</p>
          </div>
        </a>
      </div>

      <form class="form-card fade-in" id="contact-form" @submit.prevent="handleSubmit">
        <h2 class="form-title">Mesaj Gönder</h2>

        <p v-if="state.successMessage" class="form-success">{{ state.successMessage }}</p>
        <p v-if="state.errorMessage" class="form-error">{{ state.errorMessage }}</p>

        <div class="form-group">
          <label for="name">Ad</label>
          <input
            type="text"
            id="name"
            placeholder="Adınızı girin"
            v-model="form.ad"
            required
          />
        </div>

        <div class="form-group">
          <label for="soyad">Soyad</label>
          <input
            type="text"
            id="soyad"
            placeholder="Soyadınızı girin"
            v-model="form.soyad"
            required
          />
        </div>

        <div class="form-group">
          <label for="email">E-posta</label>
          <input
            type="email"
            id="email"
            placeholder="E-posta adresiniz"
            v-model="form.email"
            required
          />
        </div>

        <div class="form-group">
          <label for="message">Mesaj</label>
          <textarea
            id="message"
            placeholder="Mesajınızı yazın..."
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
          {{ state.loading ? 'Gönderiliyor...' : 'Gönder' }}
        </button>
      </form>
    </main>
  </div>
</template>

<script setup>
import { reactive } from 'vue'
import { appEnv } from '@/config/env'

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
    state.errorMessage = 'Sunucu yapılandırması eksik. Lütfen VITE_API_BASE değerini .env.local dosyasında tanımlayın.'
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
    console.error("Server response was:", response.status, data);

    if (!response.ok || data.success === false) {
      let hataMesaji = data?.mesaj || data?.title || 'Mesaj gönderilemedi.'
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

    state.successMessage = data?.mesaj || 'Mesajınız iletildi.'

    form.ad = ''
    form.soyad = ''
    form.email = ''
    form.mesaj = ''
    form.webSitesi = ''
  } catch (error) {
    state.errorMessage = 'Sunucuya ulaşılamadı. Lütfen daha sonra tekrar deneyin.'
  } finally {
    state.loading = false
  }
}
</script>
