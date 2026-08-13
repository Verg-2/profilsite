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

// Global Vue Error Handler - Hataları SystemHealthLogs'a gönder
app.config.errorHandler = (err, instance, info) => {
  console.error('Vue Global Error:', err)
  
  api.post('/Analytics/log-error', {
    errorType: 'VUE_ERROR',
    details: `Info: ${info} | Message: ${err.message}`
  }).catch(() => { /* Sessizce yut */ })
}

app.use(router).mount('#app')

