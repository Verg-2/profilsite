<template>
  <!-- Extracted exactly from pages/index.html <div id="app"> content -->
  <section class="hero" id="hero">
    <canvas id="fire-canvas" ref="fireCanvas"></canvas>
    <div class="hero-content">
        <div class="hero-text">
          <p class="greeting">Merhaba, ben</p>
          <h1 class="hero-title">
            <span class="highlight">Kadir</span><br />
            Backend Developer
          </h1>
          <p class="hero-subtitle">
            Modern web uygulamalarının arkasında çalışan güçlü backend sistemleri geliştiriyorum. Temiz ve sürdürülebilir kod yazmaya, güvenli ve ölçeklenebilir mimariler oluşturmaya önem veriyorum. Amacım, kullanıcıların fark etmese bile sorunsuz çalışan hızlı ve güvenilir sistemler oluşturmak.
          </p>
          <div class="hero-buttons">
            <router-link to="/projects" class="btn btn-primary">
              <span>Projelerim</span>
            </router-link>
            <router-link to="/contact" class="btn btn-secondary">
              <span>İletişim</span>
            </router-link>
          </div>
        </div>
        <div class="hero-image">
          <div class="image-frame">
            <img src="@/assets/img/wolff.png" alt="Profil Fotoğrafı" class="profile-img" id="profile-img" />
            <div class="image-glow" id="image-glow"></div>
          </div>
        </div>
    </div>
  </section>
</template>

<script setup>
import { onMounted, ref } from 'vue'

// Fire canvas ref for animations.js
const fireCanvas = ref(null)

onMounted(() => {
  // Trigger index animations (app.js fire + parallax)
  // animations.js detects #fire-canvas and initIndex()
})
</script>

