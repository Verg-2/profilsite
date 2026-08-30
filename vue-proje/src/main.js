import '@/assets/style.css'
import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import '@fortawesome/fontawesome-free/css/all.min.css'

import api from '@/services/api'
import DOMPurify from 'dompurify'

const app = createApp(App)

// Global Safe HTML Directive for XSS Protection
app.directive('safe-html', {
  mounted(el, binding) {
    el.innerHTML = DOMPurify.sanitize(binding.value);
  },
  updated(el, binding) {
    el.innerHTML = DOMPurify.sanitize(binding.value);
  }
});

// Güvenlik: Üretim ortamında tüm konsol çıktılarını kapat ve hataları backend'e gizlice logla
if (import.meta.env.PROD) {
  console.log = () => {};
  console.info = () => {};
  console.warn = () => {};
  console.error = () => {};
  console.debug = () => {};

  window.addEventListener('error', (event) => {
    api.post('/Analytics/log-error', {
      errorType: 'WINDOW_ERROR',
      details: event.message || 'Bilinmeyen Hata'
    }).catch(() => {});
  });

  window.addEventListener('unhandledrejection', (event) => {
    api.post('/Analytics/log-error', {
      errorType: 'PROMISE_REJECTION',
      details: event.reason?.message || 'Bilinmeyen Promise Hatası'
    }).catch(() => {});
  });
}

// Global Vue Error Handler - Hataları SystemHealthLogs'a gönder
app.config.errorHandler = (err, instance, info) => {
  if (!import.meta.env.PROD) {
    console.error('Vue Global Error:', err)
  }
  
  api.post('/Analytics/log-error', {
    errorType: 'VUE_ERROR',
    details: `Info: ${info} | Message: ${err.message}`
  }).catch(() => { /* Sessizce yut */ })
}

app.use(router).mount('#app')

