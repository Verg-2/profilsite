<template>
  <div class="app-wrapper">
    <!-- Desktop Navbar -->
    <nav class="navbar desktop-nav">
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
          <li v-for="link in desktopLinks" :key="link.path">
            <router-link :to="link.path">{{ link.name }}</router-link>
          </li>
          <li>
            <button @click.stop="toggleTheme" class="theme-toggle-btn" aria-label="Temayı Değiştir">
              <i :class="theme === 'dark' ? 'ph ph-sun' : 'ph ph-moon'"></i>
            </button>
          </li>
        </ul>
      </div>
    </nav>

    <!-- MUHTEŞEM EFEKTLİ MOBİL ALT NAVİGASYON -->
    <nav class="mobile-bottom-nav">
      <router-link 
        v-for="link in mobileLinks" 
        :key="link.path" 
        :to="link.path" 
        class="nav-item"
        active-class="active"
      >
        <div class="icon-wrapper" :class="{ 'home-icon': link.path === '/' }">
          <i :class="link.icon"></i>
        </div>
        <span class="label">{{ link.name }}</span>
      </router-link>

      <button @click="toggleTheme" class="nav-item theme-nav-btn" aria-label="Temayı Değiştir">
        <div class="icon-wrapper">
          <i :class="theme === 'dark' ? 'ph ph-sun' : 'ph ph-moon'"></i>
        </div>
        <span class="label">Tema</span>
      </button>
    </nav>

    <!-- Aktif route'un bileşeni burada gösterilir. -->
    <router-view class="page-content" />
  </div>
</template>

<script setup>
import { nextTick, onBeforeUnmount, onMounted, watch, ref } from 'vue'
import { useRoute } from 'vue-router'
import '@/assets/style.css'
import { cleanupPageAnimations, initPageAnimations } from '@/assets/js/page-animations'

const route = useRoute()
const isMenuOpen = ref(false)
const theme = ref(localStorage.getItem('theme') || 'dark')

// Masaüstü için tam liste
const desktopLinks = [
  { name: 'Anasayfa', path: '/' },
  { name: 'Hakkında', path: '/hakkinda' },
  { name: 'Blog', path: '/blog' },
  { name: 'Yetenekler', path: '/yetenekler' },
  { name: 'Projeler', path: '/projects' },
  { name: 'İletişim', path: '/contact' }
]

// Mobil için 6 link + Tema Butonu (Anasayfa tam ortada olacak şekilde ayarlandı)
const mobileLinks = [
  { name: 'Hakkında', path: '/hakkinda', icon: 'ph ph-user' },
  { name: 'Blog', path: '/blog', icon: 'ph ph-article' },
  { name: 'Yetenekler', path: '/yetenekler', icon: 'ph ph-lightning' },
  { name: 'Anasayfa', path: '/', icon: 'ph ph-house' }, // Ortada (Index 3)
  { name: 'Projeler', path: '/projects', icon: 'ph ph-code' },
  { name: 'İletişim', path: '/contact', icon: 'ph ph-envelope-simple' }
]

function toggleTheme(event) {
  const switchTheme = () => {
    theme.value = theme.value === 'dark' ? 'light' : 'dark'
    localStorage.setItem('theme', theme.value)
    if (theme.value === 'light') {
      document.documentElement.setAttribute('data-theme', 'light')
    } else {
      document.documentElement.removeAttribute('data-theme')
    }
    // Refresh canvas colors on theme change
    refreshAnimations()
  }

  // Modern tarayıcılarda çok şık bir geçiş (View Transitions API)
  if (!document.startViewTransition) {
    switchTheme()
    return
  }

  // Tıklanan ikonun tam merkezini hesapla
  let x = window.innerWidth / 2
  let y = window.innerHeight / 2
  
  if (event && event.currentTarget) {
    const rect = event.currentTarget.getBoundingClientRect()
    x = rect.left + rect.width / 2
    y = rect.top + rect.height / 2
  } else if (event && event.clientX) {
    x = event.clientX
    y = event.clientY
  }

  const endRadius = Math.hypot(Math.max(x, window.innerWidth - x), Math.max(y, window.innerHeight - y))

  const transition = document.startViewTransition(() => {
    switchTheme()
  })

  transition.ready.then(() => {
    document.documentElement.animate(
      [
        { clipPath: `circle(0px at ${x}px ${y}px)` },
        { clipPath: `circle(${endRadius}px at ${x}px ${y}px)` }
      ],
      {
        duration: 600,
        easing: 'ease-in-out',
        pseudoElement: '::view-transition-new(root)'
      }
    )
  })
}

async function refreshAnimations() {
  cleanupPageAnimations()
  await nextTick()
  initPageAnimations()
}

onMounted(() => {
  if (theme.value === 'light') {
    document.documentElement.setAttribute('data-theme', 'light')
  }
  refreshAnimations()
})

watch(() => route.fullPath, () => {
  refreshAnimations()
})

onBeforeUnmount(() => {
  cleanupPageAnimations()
})
</script>

<style scoped>
/* Genel sargı */
.app-wrapper {
  min-height: 100vh;
  position: relative;
}

/* Tema Butonu Masaüstü Stili */
.theme-toggle-btn {
  background: transparent;
  border: 1px solid var(--border);
  color: var(--text);
  font-size: 1.2rem;
  cursor: pointer;
  width: 36px;
  height: 36px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.3s ease;
  margin-left: 1rem;
}
.theme-toggle-btn:hover {
  background: var(--primary);
  color: #fff;
  border-color: var(--primary);
  transform: translateY(-2px);
}
.theme-nav-btn {
  background: transparent;
  border: none;
  cursor: pointer;
}
.theme-nav-btn .icon-wrapper {
  color: var(--accent);
}
/* ==========================================================
   MOBILE BOTTOM NAVIGATION BAR (MUHTEŞEM EFEKTLİ)
   ========================================================== */
.mobile-bottom-nav {
  display: none; /* Masaüstünde gizli */
  position: fixed;
  bottom: 1.5rem;
  left: 50%;
  transform: translateX(-50%);
  width: calc(100% - 2rem);
  max-width: 500px;
  height: 70px;
  /* Arkaplanı biraz daha koyulaştırarak (opacity 0.75) daha tok bir görünüm verdik */
  background: rgba(10, 10, 12, 0.75);
  backdrop-filter: blur(30px) saturate(150%);
  -webkit-backdrop-filter: blur(30px) saturate(150%);
  border: 1px solid rgba(255, 77, 0, 0.25);
  border-radius: 24px;
  z-index: 2000;
  padding: 0 0.5rem;
  /* Altında tema rengine uygun şık ve hafif bir yansıma (glow) gölgesi */
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.7), 0 10px 25px rgba(255, 77, 0, 0.15), inset 0 1px 0 rgba(255, 255, 255, 0.1);
}

/* Mobil görünüm için media query (Telefonlar) */
@media (max-width: 768px) {
  /* Eski hamburger menü ve linkleri telefonda gizle, logoyu bırak */
  .desktop-nav .nav-links,
  .desktop-nav .menu-toggle {
    display: none !important;
  }

  .mobile-bottom-nav {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }
  
  /* Sayfa içeriklerinin alt menünün altında kalmaması için */
  .page-content {
    padding-bottom: 100px !important; 
  }
}

/* İtem Stilleri */
.mobile-bottom-nav .nav-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  flex: 1;
  text-decoration: none;
  color: var(--text-muted);
  position: relative;
  transition: all 0.4s cubic-bezier(0.34, 1.56, 0.64, 1);
  height: 100%;
  -webkit-tap-highlight-color: transparent;
}

/* İkon Çerçevesi */
.icon-wrapper {
  position: relative;
  display: flex;
  align-items: center;
  justify-content: center;
  width: 46px;
  height: 46px;
  border-radius: 50%;
  transition: all 0.5s cubic-bezier(0.34, 1.56, 0.64, 1);
  font-size: 1.5rem;
  z-index: 2;
  color: rgba(255, 255, 255, 0.6);
}

/* Metin Etiketi */
.label {
  font-size: 0.65rem;
  font-weight: 600;
  margin-top: 2px;
  opacity: 0;
  position: absolute;
  bottom: 8px;
  transition: all 0.4s cubic-bezier(0.34, 1.56, 0.64, 1);
  transform: translateY(10px);
  color: var(--primary);
  pointer-events: none;
}

/* =========================================
   AKTİF DURUM & DIŞARI FIRLAMA EFEKTİ
   ========================================= */
.nav-item.active .icon-wrapper {
  background: linear-gradient(135deg, var(--primary), var(--accent));
  color: #fff;
  /* İkonu yukarı doğru fırlat (öne çıksın) */
  transform: translateY(-24px);
  /* Daha yumuşak, temiz bir gölge (Halka veya sert ateş olmadan) */
  box-shadow: 0 12px 20px rgba(0, 0, 0, 0.25);
}

.nav-item.active .icon-wrapper i {
  animation: popIcon 0.5s cubic-bezier(0.34, 1.56, 0.64, 1) forwards;
}

.nav-item.active .label {
  opacity: 1;
  transform: translateY(0);
}

/* Tıklanınca ikonun zıplama animasyonu */
@keyframes popIcon {
  0% { transform: scale(1); }
  50% { transform: scale(1.25) rotate(-10deg); }
  75% { transform: scale(0.95) rotate(5deg); }
  100% { transform: scale(1) rotate(0); }
}


</style>
