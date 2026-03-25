# 🔥 Kadir Portfolio - Tasarım Dokümantasyonu

## 1. Tasarım Açıklaması

**Tema:** Cyber / Molten Fire / Premium Glassmorphism

Bu portfolio, koyu tema üzerine ateş kırmızısı ve amber vurgularla modern, premium bir görünüm sunar. Glassmorphism kartlar, neon glow efektleri ve yumuşak partikül animasyonları bir araya gelerek "wow" etkisi yaratır.

### Renk Paleti
```css
--bg-primary: #0B0B0F      /* Ana arka plan - çok koyu */
--bg-card: #11111A         /* Kart arka planı */
--accent-primary: #FF3B1D  /* Ateş kırmızısı */
--accent-secondary: #FFB000 /* Amber/turuncu */
--text-primary: #F4F6FF    /* Ana metin */
--text-muted: #8B8B9A      /* Soluk metin */
--glow: rgba(255, 59, 29, 0.4) /* Glow efekti */
```

---

## 2. Dosya Yapısı

```
vue-proje/
├── index.html              # Entry point
├── package.json             # Vue + Vite config
├── vite.config.js
├── src/
│   ├── main.js             # Vue app başlatma
│   ├── App.vue             # Ana bileşen
│   ├── style.css           # Global stiller + CSS değişkenleri
│   ├── assets/
│   │   └── wolff.png       # Profil resmi
│   ├── composables/        # Reusable logic
│   │   ├── useParallax.js  # Mouse parallax efekti
│   │   ├── useParticles.js # Ember partikül efekti
│   │   └── useReveal.js    # Scroll reveal efekti
│   └── components/
│       ├── Navbar.vue      # Navigasyon
│       ├── Hero.vue        # Hero bölümü
│       ├── ParticlesCanvas.vue # Arka plan partikülleri
│       ├── About.vue       # Hakkında
│       ├── Projects.vue    # Projeler
│       └── Contact.vue     # İletişim
```

---

## 3. Bileşenler

### App.vue
Ana layout. Tüm sayfaları tek sayfada render eder. Scroll ile navigasyon.

### Hero.vue
- Sol: "Merhaba, ben Kadir" label + büyük başlık
- Sağ: Kurt kartı + glow + ember particles
- Mouse parallax efekti

### ParticlesCanvas.vue
Canvas tabanlı ateş/kor partikül efekti. Düşük CPU kullanımı.

### About.vue
Timeline animasyonu + gradient shift efekti

### Projects.vue
Filtreleme + glow edge tracing efekti

### Contact.vue
Form + focus glow + akan çizgiler arka plan

---

## 4. Ayarlar (tek yerde)

Tüm ayarlar `src/style.css` dosyasındaki CSS değişkenlerinde:

```css
:root {
  /* Renkler */
  --bg-primary: #0B0B0F;
  --accent-primary: #FF3B1D;
  --accent-secondary: #FFB000;
  
  /* Hız */
  --animation-speed: 0.3s;
  --parallax-strength: 15;
  
  /* Boyutlar */
  --card-radius: 28px;
  --max-width: 1280px;
  
  /* Efekt yoğunluğu */
  --glow-intensity: 0.4;
  --particle-count: 60;
}
```

---

## 5. Performans Kurallar

- ✅ requestAnimationFrame kullan
- ✅ Event listener'ları throttle et
- ✅ prefers-reduced-motion desteği
- ✅ GPU hızlandırma (will-change)
- ✅ Düşük cihazlarda degrade

