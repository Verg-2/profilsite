<template>
  <div id="app-container">
    <!-- Arka plan partikülleri -->
    <ParticlesCanvas :particle-count="50" :opacity="0.5" />
    
    <!-- Navigasyon -->
    <Navbar />
    
    <!-- Ana içerik -->
    <main>
      <Hero />
      <div v-reveal><About /></div>
      <div v-reveal><Blog /></div>
      <div v-reveal><Skills /></div>
      <div v-reveal><Projects /></div>
      <div v-reveal><Contact /></div>
    </main>
    
  
  </div>
</template>

<script setup>
import { onMounted } from 'vue'
import { useReveal, revealDirective } from './composables/useReveal'

import Navbar from './components/Navbar.vue'
import Hero from './components/Hero.vue'
import About from './components/About.vue'
import Blog from './components/Blog.vue'
import Skills from './components/Skills.vue'
import Projects from './components/Projects.vue'
import Contact from './components/Contact.vue'
import ParticlesCanvas from './components/ParticlesCanvas.vue'

// Direktifi setup script içinde register ediyoruz
const vReveal = revealDirective

// Veya programatik kullanım (opsiyonel, v-reveal daha kolay)
const { observeAll } = useReveal()

onMounted(() => {
  // Eğer v-reveal dışında, class="reveal" olan başka elemanlar varsa:
  // observeAll()
})
</script>

<style>
/* App container */
#app-container {
  min-height: 100vh;
  position: relative;
}

/* Main content */
main {
  position: relative;
  z-index: 1;
}
</style>

