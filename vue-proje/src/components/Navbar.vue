<template>
  <nav class="navbar" :class="{ scrolled: isScrolled }">
    <div class="navbar-inner">
      <a href="#" class="logo" @click.prevent="scrollToTop">Kadir</a>
      <button
        class="nav-toggle"
        type="button"
        :aria-expanded="isMenuOpen"
        aria-label="Menüyü aç/kapat"
        @click="toggleMenu"
      >
        <span></span>
        <span></span>
        <span></span>
      </button>
      
      <ul class="nav-links" :class="{ open: isMenuOpen }">
        <li><a href="#hero" @click.prevent="scrollTo('hero')">Anasayfa</a></li>
        <li><a href="#about" @click.prevent="scrollTo('about')">Hakkında</a></li>
        <li><a href="#blog" @click.prevent="scrollTo('blog')">Blog</a></li>
        <li><a href="#skills" @click.prevent="scrollTo('skills')">Yetenekler</a></li>
        <li><a href="#projects" @click.prevent="scrollTo('projects')">Projeler</a></li>
        <li><a href="#contact" @click.prevent="scrollTo('contact')">İletişim</a></li>
      </ul>
    </div>
  </nav>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from 'vue'

// isScrolled: Sayfa biraz aşağı kaydırıldığında navbar'a ekstra bir sınıf verip
// arka planını/gölgesini değiştirmek için kullanıyoruz.
const isScrolled = ref(false)
const isMenuOpen = ref(false)

// Scroll event handler
// Kullanıcı sayfayı kaydırdığında, scrollY > 50 ise navbar'a "scrolled" sınıfını ekler.
const handleScroll = () => {
  isScrolled.value = window.scrollY > 50
}

// Scroll to section
// Navigasyondaki linklere tıklanınca ilgili bölümün ID'sine göre sayfayı kaydırır.
const scrollTo = (id) => {
  const element = document.getElementById(id)
  if (element) {
    element.scrollIntoView({ behavior: 'smooth' })
    isMenuOpen.value = false
  }
}

// Scroll to top
// Logo'ya tıklanınca sayfanın en üstüne yumuşak bir animasyonla gider.
const scrollToTop = () => {
  window.scrollTo({ top: 0, behavior: 'smooth' })
  isMenuOpen.value = false
}

const toggleMenu = () => {
  isMenuOpen.value = !isMenuOpen.value
}

const handleResize = () => {
  if (window.innerWidth > 860) {
    isMenuOpen.value = false
  }
}

onMounted(() => {
  window.addEventListener('scroll', handleScroll, { passive: true })
  window.addEventListener('resize', handleResize, { passive: true })
})

onUnmounted(() => {
  window.removeEventListener('scroll', handleScroll)
  window.removeEventListener('resize', handleResize)
})
</script>

<style scoped>
/* Navbar styles are in global style.css */
</style>

