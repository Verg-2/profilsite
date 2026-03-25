<template>
  <div id="app-container">
    <!-- Arka plan partikülleri -->
    <ParticlesCanvas :particle-count="50" :opacity="0.5" />
    
    <!-- Navigasyon -->
    <Navbar />
    
    <!-- Ana içerik -->
    <main>
      <Hero />
      <About />
      <Projects />
      <Contact />
    </main>
    
    <!-- Footer -->
    <footer>
      <p>Tasarım ve Kodlama <span class="heart">♥</span> 2024</p>
    </footer>
  </div>
</template>

<script setup>
import { onMounted } from 'vue'
import Navbar from './components/Navbar.vue'
import Hero from './components/Hero.vue'
import About from './components/About.vue'
import Projects from './components/Projects.vue'
import Contact from './components/Contact.vue'
import ParticlesCanvas from './components/ParticlesCanvas.vue'

// Scroll reveal animasyonu
// Bu kısım, .reveal sınıfına sahip bölümlerin sayfada aşağı doğru inerken
// yumuşak bir şekilde görünmesini sağlar. IntersectionObserver ile
// elementler ekranda görünür olduğunda onlara .visible sınıfı ekliyoruz.
onMounted(() => {
  // Tüm .reveal sınıflı elementleri gözlemle
  const observer = new IntersectionObserver((entries) => {
    entries.forEach(entry => {
      if (entry.isIntersecting) {
        entry.target.classList.add('visible')
        observer.unobserve(entry.target)
      }
    })
  }, {
    threshold: 0.1,
    rootMargin: '0px'
  })
  
  document.querySelectorAll('.reveal').forEach(el => {
    observer.observe(el)
  })
  
  // Azaltılmış hareket tercihi kontrolü
  // Eğer kullanıcı tarayıcı ayarlarından "reduce motion" seçmişse
  // animasyonları çalıştırmak yerine tüm .reveal elemanlarını direkt görünür yapıyoruz.
  if (window.matchMedia('(prefers-reduced-motion: reduce)').matches) {
    document.querySelectorAll('.reveal').forEach(el => {
      el.classList.add('visible')
    })
  }
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

