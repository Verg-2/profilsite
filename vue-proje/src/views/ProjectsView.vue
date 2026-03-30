<template>
  <div class="projects-page">
    <canvas id="particles-canvas"></canvas>

    <header class="page-header">
      <h1>Projelerim</h1>
      <p>Yaptığım ve üzerinde çalıştığım projeler. Her biri özenle tasarlanmış ve geliştirilmiştir.</p>
    </header>

    <main class="projects-container">
      <div class="filter-bar">
        <button class="filter-btn active"><span>Tümü</span></button>
        <button class="filter-btn"><span>Web</span></button>
        <button class="filter-btn"><span>Mobil</span></button>
        <button class="filter-btn"><span>Tasarım</span></button>
      </div>

      <div class="projects-grid">
        <article class="project-card fade-in">
          <div class="project-image" style="background: linear-gradient(135deg, #FF3B1D, #FF6B35);">🌐</div>
          <div class="project-content">
            <h3 class="project-title">Kişisel Portföy Sitesi</h3>
            <p class="project-desc">Modern ve responsive tasarıma sahip kişisel portföy web sitesi.</p>
            <div class="project-tags">
              <span class="project-tag">HTML</span><span class="project-tag">CSS</span><span class="project-tag">JavaScript</span>
            </div>
            <a href="#" class="project-link">Projeyi Görüntüle →</a>
          </div>
        </article>

        <article class="project-card fade-in">
          <div class="project-image" style="background: linear-gradient(135deg, #6366F1, #8B5CF6);">🛒</div>
          <div class="project-content">
            <h3 class="project-title">E-Ticaret Platformu</h3>
            <p class="project-desc">Tam kapsamlı e-ticaret çözümü.</p>
            <div class="project-tags"><span class="project-tag">Vue.js</span><span class="project-tag">Node.js</span></div>
            <a href="#" class="project-link">Projeyi Görüntüle →</a>
          </div>
        </article>

        <article class="project-card fade-in">
          <div class="project-image" style="background: linear-gradient(135deg, #06B6D4, #0EA5E9);">📱</div>
          <div class="project-content">
            <h3 class="project-title">Mobil Banka Uygulaması</h3>
            <p class="project-desc">Cross-platform mobil banka uygulaması.</p>
            <div class="project-tags"><span class="project-tag">React Native</span></div>
            <a href="#" class="project-link">Projeyi Görüntüle →</a>
          </div>
        </article>

        <article class="project-card fade-in">
          <div class="project-image" style="background: linear-gradient(135deg, #10B981, #34D399);">📝</div>
          <div class="project-content">
            <h3 class="project-title">Görev Yönetim App</h3>
            <p class="project-desc">Kişisel görev listesi uygulaması.</p>
            <div class="project-tags"><span class="project-tag">Vue.js</span></div>
            <a href="#" class="project-link">Projeyi Görüntüle →</a>
          </div>
        </article>

        <article class="project-card fade-in">
          <div class="project-image" style="background: linear-gradient(135deg, #F59E0B, #FBBF24);">🎨</div>
          <div class="project-content">
            <h3 class="project-title">UI/UX Tasarım Seti</h3>
            <p class="project-desc">Modern web siteleri için UI bileşenleri.</p>
            <div class="project-tags"><span class="project-tag">Figma</span></div>
            <a href="#" class="project-link">Projeyi Görüntüle →</a>
          </div>
        </article>

        <article class="project-card fade-in">
          <div class="project-image" style="background: linear-gradient(135deg, #EC4899, #F472B6);">💬</div>
          <div class="project-content">
            <h3 class="project-title">Sohbet Uygulaması</h3>
            <p class="project-desc">Real-time sohbet uygulaması.</p>
            <div class="project-tags"><span class="project-tag">Flutter</span></div>
            <a href="#" class="project-link">Projeyi Görüntüle →</a>
          </div>
        </article>
      </div>
    </main>
  </div>
</template>

<script setup>
// Animasyonlar page-animations.js ile yönetiliyor
</script>
