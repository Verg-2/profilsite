<template>
  <div>
    <!--
      Global navbar:
      Statik HTML'deki ortak menuyu Vue Router ile tek dosyada yonetiyoruz.
      Bu sayede pages klasorundeki HTML'leri bozmeden Vue tarafinda ayni deneyimi kuruyoruz.
    -->
    <nav class="navbar">
      <div class="navbar-inner">
        <router-link to="/" class="logo">Kadir</router-link>

        <button
          class="menu-toggle"
          type="button"
          :aria-expanded="isMenuOpen ? 'true' : 'false'"
          aria-label="Menüyü aç/kapat"
          @click="isMenuOpen = !isMenuOpen"
        >
          <span></span>
          <span></span>
          <span></span>
        </button>

        <ul class="nav-links" :class="{ open: isMenuOpen }" @click="isMenuOpen = false">
          <li><router-link to="/">Anasayfa</router-link></li>
          <li><router-link to="/hakkinda">Hakkında</router-link></li>
          <li><router-link to="/blog">Blog</router-link></li>
          <li><router-link to="/yetenekler">Yetenekler</router-link></li>
          <li><router-link to="/projects">Projeler</router-link></li>
          <li><router-link to="/contact">İletişim</router-link></li>
        </ul>
      </div>
    </nav>

    <!-- Aktif route'un bileşeni burada gösterilir. -->
    <router-view />
  </div>
</template>

<script setup>
import { nextTick, onBeforeUnmount, onMounted, watch, ref } from 'vue'
import { useRoute } from 'vue-router'
import '@/assets/style.css'
import { cleanupPageAnimations, initPageAnimations } from '@/assets/js/page-animations'

const route = useRoute()
const isMenuOpen = ref(false)

async function refreshAnimations() {
  // Vue Router sayfa degistirdiginde eski listener'lari temizlemez,
  // bu yuzden once aktif animasyonlari kapatiyoruz.
  cleanupPageAnimations()

  // Yeni route'un DOM'u tamamen olustuktan sonra animasyonlari tekrar bagliyoruz.
  await nextTick()
  initPageAnimations()
}

onMounted(() => {
  refreshAnimations()
})

watch(() => route.fullPath, () => {
  refreshAnimations()
})

onBeforeUnmount(() => {
  cleanupPageAnimations()
})
</script>
