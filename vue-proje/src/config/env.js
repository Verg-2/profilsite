// Ortam değişkenlerini tek noktadan oku (Vite: import.meta.env)
export const appEnv = {
  apiBase: import.meta.env.VITE_API_BASE || '',
  analyticsKey: import.meta.env.VITE_ANALYTICS_KEY || '',
  sentryDsn: import.meta.env.VITE_SENTRY_DSN || ''
}

// Kullanım örneği:
// import { appEnv } from '@/config/env'
// fetch(`${appEnv.apiBase}/endpoint`)
