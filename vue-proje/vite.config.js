import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

export default defineConfig({
  plugins: [vue()],
  resolve: {
    alias: {
      '@': '/src',
      '../css': '/css',
      '../js': '/js',
      '../img': '/img'
    }
  },
  server: {
    port: 3001
  }
})

