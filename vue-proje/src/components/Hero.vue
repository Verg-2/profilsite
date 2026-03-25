<template>
  <section class="hero" id="hero">
    <!-- Arka plan gradient -->
    <div class="hero-bg">
      <div class="hero-gradient-1"></div>
      <div class="hero-gradient-2"></div>
    </div>
    
    <div class="container hero-content">
      <!-- Sol taraf - Metin -->
      <div class="hero-text" ref="textRef">
        <span class="hero-label">Merhaba, ben</span>
        <h1 class="hero-title">
          <span class="text-gradient">Kadir</span>
          <br>
          Frontend Developer
        </h1>
        <p class="hero-subtitle">
          Modern ve kullanıcı dostu web uygulamaları oluşturuyorum. 
          Temiz kod, yaratıcı tasarım ve mükemmel kullanıcı deneyimi 
          benim önceliklerim arasında.
        </p>
        <div class="hero-buttons">
          <a href="#projects" class="btn btn-primary" @click.prevent="scrollTo('projects')">
            Projelerim
          </a>
          <a href="#contact" class="btn btn-secondary" @click.prevent="scrollTo('contact')">
            İletişim
          </a>
        </div>
      </div>
      
      <!-- Sağ taraf - Kurt kartı -->
      <div class="hero-image" ref="cardRef">
        <div 
          class="wolf-card" 
          @mouseenter="handleMouseEnter"
          @mouseleave="handleMouseLeave"
          @mousemove="handleMouseMove"
        >
          <!-- Glow efekti -->
          <div class="wolf-glow" :style="glowStyle"></div>
          
          <!-- Profil görüntüsü -->
          <img 
            src="../assets/wolff.png" 
            alt="Profil Fotoğrafı" 
            class="wolf-image"
            ref="imageRef"
          />
          
          <!-- Heat shimmer overlay -->
          <div class="heat-shimmer"></div>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup>
import { ref, computed } from 'vue'

// Refs: HTML tarafındaki belirli kutuları (DOM elemanlarını) Vue içinde yakalamak için kullanıyoruz.
// Böylece bu elemanların boyutuna/konumuna göre hesaplama yapabiliriz.
const cardRef = ref(null)
const imageRef = ref(null)
const textRef = ref(null)

// Mouse state: Fare konumunu ve kartın üzerinde olup olmadığını saklıyoruz.
// Bu sayede kart üzerinde gezerken 3D/parallax efekti verebiliyoruz.
const mouseX = ref(0)
const mouseY = ref(0)
const isHovering = ref(false)

// Scroll to section
// Kullanıcı butona tıkladığında sayfada ilgili ID'ye sahip bölüme yumuşak geçiş yapar.
const scrollTo = (id) => {
  const element = document.getElementById(id)
  if (element) {
    element.scrollIntoView({ behavior: 'smooth' })
  }
}

// Mouse handlers
// Kartın üzerine girince/çıkınca ve kart üzerinde hareket edince
// mouseX ve mouseY değerlerini güncelliyoruz.
const handleMouseEnter = () => {
  isHovering.value = true
}

const handleMouseLeave = () => {
  isHovering.value = false
  mouseX.value = 0
  mouseY.value = 0
}

const handleMouseMove = (e) => {
  if (!cardRef.value) return
  
  // Kartın boyutuna ve pozisyonuna göre imlecin -1 ile 1 arasında normalize edilmiş
  // X ve Y değerlerini hesaplıyoruz. Bu değerler CSS transform için kullanılacak.
  const rect = cardRef.value.getBoundingClientRect()
  mouseX.value = ((e.clientX - rect.left) / rect.width) * 2 - 1
  mouseY.value = ((e.clientY - rect.top) / rect.height) * 2 - 1
}

// Computed styles
// Fare konumuna göre glow (ışık) katmanının ne kadar kayacağını hesaplıyoruz.
// computed kullanarak, mouseX/mouseY her değiştiğinde otomatik yeniden hesaplanmasını sağlıyoruz.
const glowStyle = computed(() => {
  const x = mouseX.value * 30
  const y = mouseY.value * 20
  const intensity = isHovering.value ? 1 : 0.6
  
  return {
    transform: `translate(${x}px, ${y}px) scale(${intensity})`
  }
})
</script>

<style scoped>
/* Hero Bölümü */
.hero {
  min-height: 100vh;
  display: flex;
  align-items: center;
  position: relative;
  padding-top: var(--navbar-height);
  overflow: hidden;
}

/* Arka plan */
.hero-bg {
  position: absolute;
  inset: 0;
  z-index: 0;
  pointer-events: none;
}

.hero-gradient-1 {
  position: absolute;
  top: 0;
  left: 0;
  width: 60%;
  height: 100%;
  background: radial-gradient(ellipse at 30% 30%, rgba(255, 59, 29, 0.12) 0%, transparent 60%);
}

.hero-gradient-2 {
  position: absolute;
  top: 0;
  right: 0;
  width: 40%;
  height: 100%;
  background: radial-gradient(ellipse at 70% 70%, rgba(255, 176, 0, 0.08) 0%, transparent 50%);
}

/* İçerik */
.hero-content {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 60px;
  align-items: center;
  position: relative;
  z-index: 1;
}

/* Metin */
.hero-text {
  animation: fadeInUp 0.8s ease-out;
}

.hero-label {
  display: inline-block;
  font-size: 0.95rem;
  font-weight: 600;
  color: var(--accent-secondary);
  letter-spacing: 2px;
  text-transform: uppercase;
  margin-bottom: 16px;
}

.hero-title {
  font-size: clamp(2.5rem, 6vw, 4.5rem);
  font-weight: 800;
  line-height: 1.1;
  margin-bottom: 24px;
}

.hero-subtitle {
  font-size: 1.1rem;
  color: var(--text-muted);
  max-width: 480px;
  margin-bottom: 32px;
  line-height: 1.7;
}

.hero-buttons {
  display: flex;
  gap: 16px;
  flex-wrap: wrap;
}

/* Kurt kartı */
.hero-image {
  display: flex;
  justify-content: center;
  align-items: center;
  perspective: 1000px;
}

.wolf-card {
  position: relative;
  width: 100%;
  max-width: 380px;
  aspect-ratio: 3/4;
  border-radius: var(--card-radius);
  overflow: hidden;
  background: linear-gradient(135deg, rgba(255, 59, 29, 0.2), rgba(255, 176, 0, 0.1));
  border: 1px solid var(--border);
  box-shadow: 
    0 30px 60px rgba(0, 0, 0, 0.5),
    0 0 80px rgba(255, 59, 29, 0.15);
  transform-style: preserve-3d;
  transition: transform 0.3s ease;
  cursor: pointer;
}

.wolf-card:hover {
  transform: perspective(1000px) rotateY(0deg) rotateX(0deg);
}

.wolf-glow {
  position: absolute;
  inset: -50%;
  background: radial-gradient(circle at 50% 50%, rgba(255, 59, 29, 0.4), transparent 60%);
  pointer-events: none;
  transition: transform 0.15s ease;
  filter: blur(30px);
}

.wolf-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
  filter: sepia(10%) saturate(1.2);
  transition: transform 0.3s ease;
}

.wolf-card:hover .wolf-image {
  transform: scale(1.05);
}

/* Heat shimmer efekti */
.heat-shimmer {
  position: absolute;
  inset: 0;
  background: linear-gradient(
    180deg,
    transparent 0%,
    rgba(255, 100, 0, 0.03) 50%,
    transparent 100%
  );
  animation: shimmer 3s ease-in-out infinite;
  pointer-events: none;
}

@keyframes shimmer {
  0%, 100% { opacity: 0.5; }
  50% { opacity: 1; }
}

@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(30px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

/* Responsive */
@media (max-width: 968px) {
  .hero-content {
    grid-template-columns: 1fr;
    text-align: center;
    padding-top: 100px;
    padding-bottom: 60px;
  }
  
  .hero-text {
    order: 2;
  }
  
  .hero-image {
    order: 1;
    margin-bottom: 40px;
  }
  
  .hero-subtitle {
    margin-left: auto;
    margin-right: auto;
  }
  
  .hero-buttons {
    justify-content: center;
  }
  
  .wolf-card {
    max-width: 300px;
  }
}

@media (max-width: 480px) {
  .hero-title {
    font-size: 2rem;
  }
  
  .wolf-card {
    max-width: 260px;
  }
}
</style>

