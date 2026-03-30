<template>
  <div class="blog-page">
    <canvas id="particles-canvas"></canvas>

    <header class="page-header">
      <h1>Blog Yazılarım</h1>
      <p>Web geliştirme, tasarım ve teknoloji hakkında düşüncelerim ve deneyimlerim.</p>
    </header>

    <main class="blog-container">
      <div class="blog-grid">
        <article class="blog-card fade-in">
          <div class="blog-image">💻</div>
          <div class="blog-content">
            <div class="blog-date">15 Ocak 2024</div>
            <h2 class="blog-title">Modern Web Geliştirme Trendleri</h2>
            <p class="blog-excerpt">2024'te web geliştirme dünyasında neler oluyor? Yeni framework'ler, performans optimizasyonları...</p>
            <a href="#" class="blog-read-more">Devamını Oku →</a>
          </div>
        </article>

        <article class="blog-card fade-in">
          <div class="blog-image">🎨</div>
          <div class="blog-content">
            <div class="blog-date">10 Ocak 2024</div>
            <h2 class="blog-title">CSS Animasyonları İpuçları</h2>
            <p class="blog-excerpt">CSS ile profesyonel animasyonlar nasıl yapılır? Performanslı ve göz alıcı efektler...</p>
            <a href="#" class="blog-read-more">Devamını Oku →</a>
          </div>
        </article>

        <article class="blog-card fade-in">
          <div class="blog-image">🚀</div>
          <div class="blog-content">
            <div class="blog-date">5 Ocak 2024</div>
            <h2 class="blog-title">JavaScript Performans İpuçları</h2>
            <p class="blog-excerpt">JavaScript kodlarınızı nasıl daha hızlı yaparsınız? Debounce, throttle teknikleri...</p>
            <a href="#" class="blog-read-more">Devamını Oku →</a>
          </div>
        </article>

        <article class="blog-card fade-in">
          <div class="blog-image">🔧</div>
          <div class="blog-content">
            <div class="blog-date">28 Aralık 2023</div>
            <h2 class="blog-title">Vue.js ile Başlangıç</h2>
            <p class="blog-excerpt">Vue.js öğrenmek isteyenler için başlangıç rehberi. Temel kavramlar ve örnek kodlar...</p>
            <a href="#" class="blog-read-more">Devamını Oku →</a>
          </div>
        </article>

        <article class="blog-card fade-in">
          <div class="blog-image">📱</div>
          <div class="blog-content">
            <div class="blog-date">20 Aralık 2023</div>
            <h2 class="blog-title">Responsive Tasarım İlkeleri</h2>
            <p class="blog-excerpt">Her cihazda güzel görünen web siteleri nasıl yapılır? Mobile-first yaklaşım...</p>
            <a href="#" class="blog-read-more">Devamını Oku →</a>
          </div>
        </article>

        <article class="blog-card fade-in">
          <div class="blog-image">⚡</div>
          <div class="blog-content">
            <div class="blog-date">15 Aralık 2023</div>
            <h2 class="blog-title">Web Performans Optimizasyonu</h2>
            <p class="blog-excerpt">Web sitenizi hızlandırmak için yapmanız gerekenler. Lazy loading, caching...</p>
            <a href="#" class="blog-read-more">Devamını Oku →</a>
          </div>
        </article>
      </div>
    </main>
  </div>
</template>

<script setup>
// Animasyonlar page-animations.js tarafından yönetiliyor
</script>
