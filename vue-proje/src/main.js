import { createApp } from 'vue'
import App from './App.vue'
import './style.css'

// Create Vue app
const app = createApp(App)

// Global error handler
app.config.errorHandler = (err, vm, info) => {
  console.error('Vue Error:', err)
  console.error('Info:', info)
}

// Mount app
app.mount('#app')

