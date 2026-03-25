<template>
  <section class="section about" id="about">
    <div class="container">
      <h2 class="section-title reveal">Hakkında</h2>
      
      <div class="about-grid">
        <!-- Kart 1 -->
        <div class="about-card reveal">
          <div class="card-icon">
            <!-- Hedef / Odak ikon u - kod ve arayüze odaklanma -->
            <svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
              <circle cx="12" cy="12" r="7" stroke="white" stroke-width="1.6" opacity="0.85"/>
              <circle cx="12" cy="12" r="3" fill="white" opacity="0.95"/>
              <path d="M12 3V1" stroke="white" stroke-width="1.6" stroke-linecap="round"/>
              <path d="M21 12H23" stroke="white" stroke-width="1.6" stroke-linecap="round"/>
              <path d="M12 23V21" stroke="white" stroke-width="1.6" stroke-linecap="round"/>
              <path d="M1 12H3" stroke="white" stroke-width="1.6" stroke-linecap="round"/>
            </svg>
          </div>
          <h3>Neler Yapıyorum?</h3>
          <p>
            Web siteleri, web uygulamaları ve kullanıcı arayüzleri tasarlayıp geliştiriyorum. 
            Modern teknolojileri takip ediyor ve en iyi çözümleri üretmeye çalışıyorum.
          </p>
        </div>

        <!-- Kart 2 -->
        <div class="about-card reveal">
          <div class="card-icon">
            <!-- Fikir / Yaklaşım ikonu - düşünce ve çözüm odaklı çalışma -->
            <svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
              <path d="M12 3C8.962 3 6.5 5.462 6.5 8.5C6.5 10.4 7.46 11.96 8.86 12.86C9.32 13.16 9.6 13.68 9.6 14.24V15H14.4V14.24C14.4 13.68 14.68 13.16 15.14 12.86C16.54 11.96 17.5 10.4 17.5 8.5C17.5 5.462 15.038 3 12 3Z" stroke="white" stroke-width="1.6" stroke-linecap="round" stroke-linejoin="round"/>
              <path d="M10 19H14" stroke="white" stroke-width="1.6" stroke-linecap="round"/>
              <path d="M11 21H13" stroke="white" stroke-width="1.6" stroke-linecap="round"/>
            </svg>
          </div>
          <h3>Yaklaşımım</h3>
          <p>
            Her proje benzersizdir. Müşteri ihtiyaçlarını anlayarak, 
            en uygun ve ölçeklenebilir çözümleri sunuyorum. 
            Kod kalitesi ve performans benim için önemli.
          </p>
        </div>

        <!-- Kart 3 -->
        <div class="about-card reveal">
          <div class="card-icon">
            <!-- Hedefler / Büyüme ikonu - yeni teknolojiler ve ileri gitme -->
            <svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
              <path d="M14.5 2.5C11 3 7.5 6.5 7 10L4.5 12.5C4 13 3.8 13.7 4.1 14.3L5.5 17.2C5.9 18 7 18.1 7.6 17.4L9.5 15.5C13 15 16.5 11.5 17 8L19.2 5.8C19.9 5.1 19.8 4 19 3.6L16.1 2.2C15.5 1.9 14.8 2.1 14.5 2.5Z" stroke="white" stroke-width="1.6" stroke-linecap="round" stroke-linejoin="round"/>
              <path d="M7 20C7.5 18.5 8.5 17.5 10 17" stroke="white" stroke-width="1.6" stroke-linecap="round"/>
            </svg>
          </div>
          <h3>Hedeflerim</h3>
          <p>
            Sürekli öğrenmeye ve kendimi geliştirmeye açığım. 
            Yeni teknolojiler öğrenmek, zorlu projeler üzerinde çalışmak 
            beni motive ediyor.
          </p>
        </div>
      </div>

      <!-- Timeline (visual effect) -->
      <div class="timeline">
        <!--
          timeline-line: Çizginin dolu kısmı.
          Genişliği scroll konumuna göre (timelinePos) yüzde olarak değişiyor.
        -->
        <div class="timeline-line" :style="{ width: timelinePos + '%' }"></div>
        <div class="timeline-dot" :style="{ left: timelinePos + '%' }"></div>
      </div>
    </div>
  </section>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from 'vue'

const timelinePos = ref(0)
let scrollHandler = null

// Timeline animasyonu
const handleScroll = () => {
  const section = document.getElementById('about')
  if (!section) return
  
  const rect = section.getBoundingClientRect()
  const windowHeight = window.innerHeight
  
  // Section görünürlüğüne göre pozisyon hesapla
  if (rect.top < windowHeight && rect.bottom > 0) {
    const progress = Math.min(1, Math.max(0, (windowHeight - rect.top) / (windowHeight + rect.height)))
    timelinePos.value = progress * 100
  }
}

onMounted(() => {
  scrollHandler = handleScroll
  window.addEventListener('scroll', scrollHandler, { passive: true })
})

onUnmounted(() => {
  if (scrollHandler) {
    window.removeEventListener('scroll', scrollHandler)
  }
})
</script>

<style scoped>
.about {
  position: relative;
  z-index: 0;
  overflow: hidden;
}

/* Arka planı, projenin ateş/kızıl temasına uyumlu hafif bir gradient ile destekliyoruz */
.about::before {
  content: '';
  position: absolute;
  inset: 0;
  pointer-events: none;
  z-index: -1;
  background:
    radial-gradient(ellipse at 15% 0%, rgba(255, 59, 29, 0.14) 0%, transparent 55%),
    radial-gradient(ellipse at 85% 100%, rgba(255, 176, 0, 0.10) 0%, transparent 55%);
}

.about-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  gap: var(--grid-gap);
}

.about-card {
  background: var(--bg-card);
  border: 1px solid var(--border);
  border-radius: var(--card-radius);
  padding: 32px;
  transition: all var(--animation-speed) var(--animation-smooth);
}

.about-card:hover {
  border-color: var(--accent-primary);
  transform: translateY(-4px);
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.3);
}

.card-icon {
  width: 56px;
  height: 56px;
  border-radius: 14px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.5rem;
  margin-bottom: 20px;
  border: 2px solid transparent;
  border-color: linear-gradient(135deg, var(--accent-primary), var(--accent-secondary));;
}

/* SVG ikonların kare kutunun tam ortasında ve orantılı görünmesi için */
.card-icon svg {
  display: block;
  width: 60%;
  height: 60%;
}

.about-card h3 {
  font-size: 1.25rem;
  font-weight: 600;
  margin-bottom: 12px;
  color: var(--text-primary);
}

.about-card p {
  font-size: 0.95rem;
  line-height: 1.7;
  color: var(--text-muted);
}

/* Timeline efekti */
.timeline {
  position: relative;
  height: 4px;
  background: var(--border);
  border-radius: 2px;
  margin-top: 60px;
  max-width: 600px;
  margin-left: auto;
  margin-right: auto;
}

.timeline-line {
  position: absolute;
  top: 0;
  left: 0;
  height: 100%;
  background: linear-gradient(90deg, var(--accent-primary), var(--accent-secondary));
  border-radius: 2px;
  /* Başlangıçta genişlik 0, scroll oldukça JS ile artırıyoruz */
  width: 0;
  transition: width 0.1s linear;
}

.timeline-dot {
  position: absolute;
  top: 50%;
  width: 16px;
  height: 16px;
  background: var(--accent-primary);
  border-radius: 50%;
  transform: translate(-50%, -50%);
  box-shadow: 0 0 20px var(--accent-glow);
  transition: left 0.1s linear;
}
</style>

