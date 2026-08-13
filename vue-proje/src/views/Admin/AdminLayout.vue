<template>
  <div class="admin-app">
    <!-- Overlay -->
    <div v-if="isMobileMenuOpen" class="admin-sidebar-overlay" @click="isMobileMenuOpen = false"></div>

    <!-- Sidebar -->
    <aside v-if="!isLoginPage" class="admin-sidebar admin-glass" :class="{ 'open': isMobileMenuOpen }">
      <div class="admin-sidebar-header">
        <h2><span class="highlight">Kadir</span>Admin</h2>
      </div>
      
      <nav class="admin-sidebar-nav admin-scroll">
        <router-link @click="isMobileMenuOpen = false" to="/admin" class="admin-nav-link" exact-active-class="active">
          <i class="fas fa-home"></i> <span>Anasayfa</span>
        </router-link>
        <router-link @click="isMobileMenuOpen = false" to="/admin/about" class="admin-nav-link" active-class="active">
          <i class="fas fa-user"></i> <span>Hakkında</span>
        </router-link>
        <router-link @click="isMobileMenuOpen = false" to="/admin/blog" class="admin-nav-link" active-class="active">
          <i class="fas fa-blog"></i> <span>Blog</span>
        </router-link>
        <router-link @click="isMobileMenuOpen = false" to="/admin/skills" class="admin-nav-link" active-class="active">
          <i class="fas fa-star"></i> <span>Yetenekler</span>
        </router-link>
        <router-link @click="isMobileMenuOpen = false" to="/admin/projects" class="admin-nav-link" active-class="active">
          <i class="fas fa-project-diagram"></i> <span>Projeler</span>
        </router-link>
        <router-link @click="isMobileMenuOpen = false" to="/admin/messages" class="admin-nav-link" active-class="active">
          <i class="fas fa-envelope"></i> <span>Mesajlar</span>
        </router-link>
        <router-link @click="isMobileMenuOpen = false" to="/admin/analytics" class="admin-nav-link" active-class="active">
          <i class="fas fa-chart-line"></i> <span>İstatistikler</span>
        </router-link>
        <router-link @click="isMobileMenuOpen = false" to="/admin/health" class="admin-nav-link" active-class="active">
          <i class="fas fa-heartbeat"></i> <span>Sistem Sağlığı</span>
        </router-link>
        <router-link @click="isMobileMenuOpen = false" to="/admin/seo" class="admin-nav-link" active-class="active">
          <i class="fas fa-search"></i> <span>SEO ve GEO</span>
        </router-link>
        <router-link @click="isMobileMenuOpen = false" to="/admin/contact" class="admin-nav-link" active-class="active">
          <i class="fas fa-address-book"></i> <span>İletişim & Sosyal</span>
        </router-link>
        <router-link @click="isMobileMenuOpen = false" to="/admin/api-keys" class="admin-nav-link" active-class="active">
          <i class="fas fa-key"></i> <span>API Yönetimi</span>
        </router-link>
        <router-link @click="isMobileMenuOpen = false" to="/admin/glossary" class="admin-nav-link" active-class="active">
          <i class="fas fa-book"></i> <span>Dinamik Sözlük</span>
        </router-link>
      </nav>

      <div class="admin-sidebar-footer">
        <button @click="toggleTheme" class="admin-nav-link return-link" style="width: 100%; border: none; background: transparent; cursor: pointer; text-align: left; padding: 1rem; margin-bottom: 0.5rem; display: flex; align-items: center; gap: 1rem; color: var(--admin-text-main);">
          <i :class="theme === 'dark' ? 'ph ph-sun' : 'ph ph-moon'" style="font-size: 1.25rem; width: 24px; text-align: center;"></i> 
          <span>{{ theme === 'dark' ? 'Açık Tema' : 'Karanlık Tema' }}</span>
        </button>
        <a href="#" @click.prevent="handleLogout" class="admin-nav-link return-link">
          <i class="fas fa-sign-out-alt"></i> <span>Çıkış Yap</span>
        </a>
      </div>
    </aside>

    <!-- Main Content -->
    <main class="admin-main" :style="isLoginPage ? 'padding: 0;' : ''">
      <header v-if="!isLoginPage" class="admin-header admin-glass">
        <div style="display: flex; align-items: center; gap: 1rem;">
          <button @click="isMobileMenuOpen = !isMobileMenuOpen" class="admin-mobile-toggle">
            <i class="fas fa-bars"></i>
          </button>
          <h1 class="admin-page-title">{{ currentRouteName }}</h1>
        </div>
        <div class="admin-header-actions" style="display: flex; align-items: center; gap: 1.5rem;">
          <!-- Visual Timer -->
          <div v-if="formattedCountdown && !isLoginPage" class="session-timer" :class="{'timer-danger': isTimerDanger}">
            <i class="fas" :class="isRememberMeRef ? 'fa-clock' : 'fa-hourglass-half'"></i>
            <span class="timer-text">{{ formattedCountdown }}</span>
          </div>
          
          <button @click="clearCache" class="admin-btn admin-btn-danger" style="display: flex; align-items: center; gap: 0.5rem; padding: 0.5rem 1rem; border: none; border-radius: var(--admin-radius-sm); background: rgba(255,51,0,0.1); color: #ff3300; cursor: pointer; transition: all 0.3s ease; font-weight: 600;">
            <i class="fas fa-bolt"></i> <span class="hide-mobile">Önbelleği Temizle</span>
          </button>
          <div class="admin-avatar">
            <span>K</span>
          </div>
        </div>
      </header>
      
      <div class="admin-content admin-scroll" :style="isLoginPage ? 'padding: 0; display: flex; align-items: center; justify-content: center; min-height: 100vh;' : ''">
        <router-view></router-view>
      </div>
    </main>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import swal from '@/utils/swal'
import '@/assets/admin.css' // Import the premium design system

const route = useRoute()
const router = useRouter()
const currentRouteName = computed(() => route.meta.title || 'Dashboard Yönetim Paneli')
const isMobileMenuOpen = ref(false)
const isLoginPage = computed(() => route.name === 'AdminLogin')

const handleLogout = () => {
  localStorage.removeItem('token')
  localStorage.removeItem('rememberMe')
  router.push('/admin/login')
}

const clearCache = async () => {
  try {
    const apiUrl = import.meta.env.VITE_API_URL || 'http://localhost:5001/api';
    const token = localStorage.getItem('token');
    const res = await fetch(`${apiUrl}/Cache/clear`, { 
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`
      }
    });
    if(res.ok) {
        await swal.fire({
          title: 'Sistem Yenilendi!',
          text: 'Önbellek (Cache) temizlendi. En güncel veriler tüm dünyaya açıldı.',
          icon: 'success',
          confirmButtonText: 'Tamam',
          background: 'var(--admin-surface)',
          color: 'var(--admin-text-main)'
        });
    } else {
        throw new Error('API Hatası');
    }
  } catch (error) {
    await swal.fire({
      title: 'Hata!',
      text: 'Önbellek temizlenemedi.',
      icon: 'error',
      confirmButtonText: 'Kapat'
    });
  }
}

const theme = ref(localStorage.getItem('theme') || 'dark')

const toggleTheme = () => {
  theme.value = theme.value === 'dark' ? 'light' : 'dark'
  localStorage.setItem('theme', theme.value)
  if (theme.value === 'light') {
    document.documentElement.setAttribute('data-theme', 'light')
  } else {
    document.documentElement.removeAttribute('data-theme')
  }
}

// Visual Countdown & Inactivity Logic
const isRememberMeRef = ref(false)
const sessionExpiresAtRef = ref(0)
const formattedCountdown = ref('')
const isTimerDanger = ref(false)
let countdownInterval = null

const INACTIVITY_LIMIT = 5 * 60 * 1000 // 5 minutes
const inactivityExpiresAt = ref(Date.now() + INACTIVITY_LIMIT)

const handleAutoLogout = async (reason) => {
  clearInterval(countdownInterval)
  removeInactivityListener()
  await swal.fire({
    title: 'Oturum Süresi Doldu',
    text: reason,
    icon: 'warning',
    confirmButtonText: 'Giriş Yap',
    background: 'var(--admin-surface)',
    color: 'var(--admin-text-main)'
  });
  handleLogout()
}

const updateCountdown = () => {
  if (isLoginPage.value) return

  const now = Date.now()
  let diff = 0
  let isGlobalTimeout = false

  if (isRememberMeRef.value) {
    if (!sessionExpiresAtRef.value) return
    diff = sessionExpiresAtRef.value - now
    isGlobalTimeout = true
  } else {
    diff = inactivityExpiresAt.value - now
    if (sessionExpiresAtRef.value && (sessionExpiresAtRef.value - now) < diff) {
      diff = sessionExpiresAtRef.value - now
      isGlobalTimeout = true
    }
  }

  if (diff <= 0) {
    formattedCountdown.value = '00:00'
    isTimerDanger.value = true
    handleAutoLogout(isGlobalTimeout ? "Güvenliğiniz gereği oturum süreniz doldu." : "5 dakikadır işlem yapmadığınız için oturumunuz sonlandırıldı.")
    return
  }

  if (isRememberMeRef.value) {
    isTimerDanger.value = diff <= 5 * 60 * 1000 // Last 5 mins
  } else {
    isTimerDanger.value = diff <= 1 * 60 * 1000 // Last 1 min
  }

  const h = Math.floor(diff / (1000 * 60 * 60)).toString().padStart(2, '0')
  const m = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60)).toString().padStart(2, '0')
  const s = Math.floor((diff % (1000 * 60)) / 1000).toString().padStart(2, '0')
  
  if (h === '00') {
    formattedCountdown.value = `${m}:${s}`
  } else {
    formattedCountdown.value = `${h}:${m}:${s}`
  }
}

const resetTimer = () => {
  inactivityExpiresAt.value = Date.now() + INACTIVITY_LIMIT
  if (!isRememberMeRef.value) {
    updateCountdown()
  }
}

const setupInactivityListener = () => {
  if (isLoginPage.value) return;

  if (!isRememberMeRef.value) {
    window.addEventListener('mousemove', resetTimer)
    window.addEventListener('keydown', resetTimer)
    window.addEventListener('click', resetTimer)
    window.addEventListener('scroll', resetTimer)
    resetTimer()
  } else {
    removeInactivityListener()
  }
}

const removeInactivityListener = () => {
  window.removeEventListener('mousemove', resetTimer)
  window.removeEventListener('keydown', resetTimer)
  window.removeEventListener('click', resetTimer)
  window.removeEventListener('scroll', resetTimer)
}

import { watch } from 'vue'
watch(isLoginPage, (newVal) => {
  if (newVal) {
    removeInactivityListener()
    if (countdownInterval) clearInterval(countdownInterval)
  } else {
    isRememberMeRef.value = localStorage.getItem('rememberMe') === 'true'
    sessionExpiresAtRef.value = parseInt(localStorage.getItem('sessionExpiresAt')) || 0
    inactivityExpiresAt.value = Date.now() + INACTIVITY_LIMIT
    
    setupInactivityListener()
    if (countdownInterval) clearInterval(countdownInterval)
    countdownInterval = setInterval(updateCountdown, 1000)
    updateCountdown()
  }
})

onMounted(() => {
  if (theme.value === 'light') {
    document.documentElement.setAttribute('data-theme', 'light')
  }
  if (!isLoginPage.value) {
    isRememberMeRef.value = localStorage.getItem('rememberMe') === 'true'
    sessionExpiresAtRef.value = parseInt(localStorage.getItem('sessionExpiresAt')) || 0
    inactivityExpiresAt.value = Date.now() + INACTIVITY_LIMIT
    
    setupInactivityListener()
    if (countdownInterval) clearInterval(countdownInterval)
    countdownInterval = setInterval(updateCountdown, 1000)
    updateCountdown()
  }
})

onUnmounted(() => {
  removeInactivityListener()
  if (countdownInterval) clearInterval(countdownInterval)
})
</script>

<style scoped>
/* Layout specific CSS */
.admin-sidebar {
  width: 280px;
  display: flex;
  flex-direction: column;
  border-right: 1px solid var(--admin-border);
  z-index: 10;
}

.admin-sidebar-header {
  padding: 2rem;
  border-bottom: 1px solid var(--admin-border);
}

.admin-sidebar-header h2 {
  font-size: 1.5rem;
  font-weight: 700;
  color: var(--admin-heading);
  letter-spacing: -0.02em;
}

.admin-sidebar-header .highlight {
  color: var(--admin-primary);
}

.admin-sidebar-nav {
  flex: 1;
  padding: 1.5rem 1rem;
  overflow-y: auto;
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.admin-nav-link {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 1rem;
  border-radius: var(--admin-radius-md);
  color: var(--admin-text-muted);
  text-decoration: none;
  font-weight: 500;
  transition: var(--admin-transition);
}

.admin-nav-link i {
  font-size: 1.25rem;
  width: 24px;
  text-align: center;
}

.admin-nav-link:hover {
  background: var(--admin-surface-hover, rgba(255, 255, 255, 0.05));
  color: var(--admin-text-main);
}

.admin-nav-link.active {
  background: var(--admin-btn-secondary-hover, rgba(255, 255, 255, 0.05));
  color: var(--admin-primary);
  border-left: 4px solid var(--admin-primary);
  border-radius: 4px var(--admin-radius-md) var(--admin-radius-md) 4px;
  box-shadow: none;
  padding-left: calc(1rem - 4px); /* Prevent shifting */
}

.admin-sidebar-footer {
  padding: 1.5rem 1rem;
  border-top: 1px solid var(--admin-border);
}

.return-link:hover {
  color: var(--admin-primary);
}

.admin-main {
  flex: 1;
  display: flex;
  flex-direction: column;
  position: relative;
  min-width: 0;
}

.admin-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1.5rem 2.5rem;
  border-bottom: 1px solid var(--admin-border);
  z-index: 5;
}

.admin-avatar {
  width: 42px;
  height: 42px;
  border-radius: 50%;
  background: linear-gradient(135deg, var(--admin-primary), #ff8c00);
  color: #ffffff;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.2rem;
  font-weight: 700;
  box-shadow: 0 4px 12px rgba(255, 51, 0, 0.3);
  border: 2px solid rgba(255, 255, 255, 0.1);
  flex-shrink: 0;
  transition: transform 0.3s ease;
}

.admin-avatar:hover {
  transform: scale(1.05) translateY(-2px);
}

.admin-page-title {
  font-size: 1.25rem;
  font-weight: 600;
  color: var(--admin-heading);
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.admin-content {
  flex: 1;
  padding: 2.5rem;
  overflow-y: auto;
}

.admin-sidebar-overlay {
  display: none;
}
.admin-mobile-toggle {
  display: none;
  background: transparent;
  border: none;
  color: var(--admin-heading);
  font-size: 1.5rem;
  cursor: pointer;
}

@media (max-width: 768px) {
  .admin-mobile-toggle {
    display: flex;
  }
  .admin-sidebar {
    position: fixed;
    top: 0;
    left: -280px;
    height: 100vh;
    transition: left 0.3s ease;
    box-shadow: 10px 0 30px rgba(0,0,0,0.5);
    z-index: 100;
  }
  .admin-sidebar.open {
    left: 0;
  }
  .admin-sidebar-overlay {
    display: block;
    position: fixed;
    inset: 0;
    background: rgba(0, 0, 0, 0.6);
    -webkit-backdrop-filter: blur(4px);
    backdrop-filter: blur(4px);
    z-index: 90;
  }
  .admin-header {
    padding: 1.5rem 1rem;
    flex-wrap: wrap;
    gap: 0.75rem;
  }
  .admin-content {
    padding: 1.5rem 1rem;
  }
}

@media (max-width: 480px) {
  .hide-mobile {
    display: none;
  }
  .admin-header-actions {
    gap: 0.75rem !important;
  }
}

/* Session Timer Widget */
.session-timer {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.4rem 0.8rem;
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid var(--admin-border);
  border-radius: 50px;
  color: var(--admin-text-main);
  font-weight: 600;
  font-size: 0.95rem;
  font-variant-numeric: tabular-nums;
  transition: all 0.3s ease;
  user-select: none;
}
.session-timer i {
  color: var(--admin-primary);
}
.session-timer.timer-danger {
  background: rgba(231, 76, 60, 0.1);
  border-color: rgba(231, 76, 60, 0.3);
  color: #e74c3c;
  animation: pulseTimer 1s infinite;
}
.session-timer.timer-danger i {
  color: #e74c3c;
}
@keyframes pulseTimer {
  0% { transform: scale(1); box-shadow: 0 0 0 0 rgba(231, 76, 60, 0.4); }
  50% { transform: scale(1.05); box-shadow: 0 0 10px 0 rgba(231, 76, 60, 0.2); }
  100% { transform: scale(1); box-shadow: 0 0 0 0 rgba(231, 76, 60, 0); }
}
</style>
