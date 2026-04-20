<template>
  <div class="about-page">
    <canvas id="stars-canvas"></canvas>

    <header class="page-header">
      <h1>Hakkında</h1>
      <p>Beni daha yakından tanımak ister misiniz?</p>
    </header>

    <main class="content-container">
      <div class="profile-card fade-in">
        <div class="profile-image" ref="tiltRef">
          <img src="@/assets/img/wolff.png" alt="Profil Fotoğrafı" />
        </div>
        <div class="profile-info">
          <h2>Merhaba, ben Kadir</h2>
          <p class="title">Frontend Developer & UI/UX Meraklısı</p>
          <p class="bio">
            Yazılım geliştirme dünyasına olan tutkum, lise yıllarında bir HTML kitabıyla başladı.
            O günden bu yana sürekli öğrenmeye ve kendimi geliştirmeye devam ediyorum.
            Modern web teknolojileri, temiz kod yazımı ve kullanıcı deneyimi tasarımı
            benim öncelikli ilgi alanlarım. Her projede en iyi çözümü bulmak için
            çözüm odaklı düşünmeyi ilke edindim.
          </p>
        </div>
      </div>

      <div class="info-grid">
        <div class="info-card fade-in">
          <i class="fas fa-code card-icon" style="color: #ff4d00;"></i>
          <h3>Neler Yapıyorum?</h3>
          <p>Web siteleri, web uygulamaları ve kullanıcı arayüzleri tasarlayıp geliştiriyorum. Modern teknolojileri takip ediyor ve en iyi çözümleri üretmeye çalışıyorum. Her projede performans ve kullanılabilirlik ön planda.</p>
        </div>

        <div class="info-card fade-in">
          <i class="fas fa-lightbulb card-icon" style="color: #ff6b35;"></i>
          <h3>Yaklaşımım</h3>
          <p>Her proje benzersizdir. Müşteri ihtiyaçlarını anlayarak, en uygun ve ölçeklenebilir çözümleri sunuyorum. Kod kalitesi ve performans benim için önemli. Temiz, okunabilir ve sürdürülebilir kod yazmayı tercih ediyorum.</p>
        </div>

        <div class="info-card fade-in">
          <i class="fas fa-rocket card-icon" style="color: #ffa500;"></i>
          <h3>Hedeflerim</h3>
          <p>Sürekli öğrenmeye ve kendimi geliştirmeye açığım. Yeni teknolojiler öğrenmek, zorlu projeler üzerinde çalışmak beni motive ediyor. Uzun vadede full-stack geliştirici olmak ve kendi ürünlerimi oluşturmak istiyorum.</p>
        </div>

        <div class="info-card fade-in">
          <i class="fas fa-heart card-icon" style="color: #ff4d00;"></i>
          <h3>Nelerden Hoşlanırım?</h3>
          <p>Yazılım geliştirme dışında kendimi geliştirebileceğim yeni konular öğrenmek ve farklı alanları keşfetmek hoşuma gider. Zaman zaman satranç oynamak, stratejik düşünme becerilerimi geliştiren aktivitelerden biridir. Bunun yanında futbol oynamak veya maç izlemek de keyif aldığım aktiviteler arasında yer alır. Günlük hayatımda dolu dolu ve verimli sohbetler etmeyi severim. Boş zamanlarımda ise ara sıra telefon ve bilgisayar oyunları oynayarak vakit geçiririm.</p>
        </div>

        <div class="info-card fade-in">
          <i class="fas fa-seedling card-icon" style="color: #ff6b35;"></i>
          <h3>Büyüme Yolculuğum</h3>
          <p>Her gün bir şeyler öğrenmeye çalışıyorum. Hatalarımdan ders çıkarıyor, daha iyi bir geliştirici olmak için çabalıyorum. Başarıya giden yolun sürekli öğrenmeden geçtiğine inanıyorum.</p>
        </div>

        <div class="info-card fade-in">
          <i class="fas fa-handshake card-icon" style="color: #ffa500;"></i>
          <h3>İşbirliği</h3>
          <p>Takım çalışmasına açığım ve farklı bakış açılarından öğrenmeyi seviyorum. Bir projede birlikte çalışmak isterseniz, her zaman iletişime geçebilirsiniz. Birlikte harika işler çıkarabiliriz!</p>
        </div>
      </div>
    </main>
  </div>
</template>

<script setup>
import { onMounted, onBeforeUnmount, ref } from 'vue'
import VanillaTilt from 'vanilla-tilt'

const tiltRef = ref(null)

onMounted(() => {
  if (tiltRef.value) {
    VanillaTilt.init(tiltRef.value, {
      max: 15,
      speed: 400,
      glare: true,
      "max-glare": 0.3,
      scale: 1.05,
      gyroscope: true
    })
  }
})

onBeforeUnmount(() => {
  if (tiltRef.value && tiltRef.value.vanillaTilt) {
    tiltRef.value.vanillaTilt.destroy()
  }
})
</script>
