<template>
  <section id="blog" class="blog-section reveal">
    <div class="page-header">
      <h1>Blog Yazılarım</h1>
      <p>Web geliştirme, tasarım ve teknoloji hakkında düşüncelerim ve deneyimlerim.</p>
    </div>

    <div class="blog-container">
      <div class="blog-grid">
        <article 
          v-for="(post, index) in posts" 
          :key="index" 
          class="blog-card"
        >
          <div class="blog-image">{{ post.emoji }}</div>
          <div class="blog-content">
            <div class="blog-date">{{ post.date }}</div>
            <h2 class="blog-title">{{ post.title }}</h2>
            <p class="blog-excerpt">{{ post.excerpt }}</p>
            <a href="#" class="blog-read-more">Devamını Oku →</a>
          </div>
        </article>
      </div>
    </div>
  </section>
</template>

<script setup>
import { ref } from 'vue'

const posts = ref([
  {
    emoji: '💻',
    date: '15 Ocak 2024',
    title: 'Modern Web Geliştirme Trendleri',
    excerpt: "2024'te web geliştirme dünyasında neler oluyor? Yeni framework'ler, performans optimizasyonları..."
  },
  {
    emoji: '🎨',
    date: '10 Ocak 2024',
    title: 'CSS Animasyonları İpuçları',
    excerpt: 'CSS ile profesyonel animasyonlar nasıl yapılır? Performanslı ve göz alıcı efektler...'
  },
  {
    emoji: '🚀',
    date: '5 Ocak 2024',
    title: 'JavaScript Performans İpuçları',
    excerpt: 'JavaScript kodlarınızı nasıl daha hızlı yaparsınız? Debounce, throttle teknikleri...'
  },
  {
    emoji: '🔧',
    date: '28 Aralık 2023',
    title: 'Vue.js ile Başlangıç',
    excerpt: 'Vue.js öğrenmek isteyenler için başlangıç rehberi. Temel kavramlar ve örnek kodlar...'
  },
  {
    emoji: '📱',
    date: '20 Aralık 2023',
    title: 'Responsive Tasarım İlkeleri',
    excerpt: 'Her cihazda güzel görünen web siteleri nasıl yapılır? Mobile-first yaklaşım...'
  },
  {
    emoji: '⚡',
    date: '15 Aralık 2023',
    title: 'Web Performans Optimizasyonu',
    excerpt: 'Web sitenizi hızlandırmak için yapmanız gerekenler. Lazy loading, caching...'
  }
])
</script>

<style scoped>
.blog-section {
  padding-top: 80px;
}

.page-header {
  padding: 60px 24px 80px;
  text-align: center;
  position: relative;
  overflow: hidden;
}

.page-header::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: 
    radial-gradient(ellipse at 30% 20%, rgba(255, 59, 29, 0.15) 0%, transparent 50%),
    radial-gradient(ellipse at 70% 80%, rgba(255, 176, 0, 0.08) 0%, transparent 40%);
  pointer-events: none;
}

.page-header h1 {
  font-size: clamp(2.5rem, 6vw, 4rem);
  font-weight: 800;
  margin-bottom: 16px;
  background: linear-gradient(135deg, var(--accent-primary), var(--accent-secondary));
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  position: relative;
}

.page-header p {
  color: var(--text-muted);
  max-width: 600px;
  margin: 0 auto;
  font-size: 1.1rem;
  position: relative;
}

.blog-container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 24px 80px;
}

.blog-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(340px, 1fr));
  gap: 24px;
}

.blog-card {
  background: var(--bg-card);
  border: 1px solid var(--border);
  border-radius: 20px;
  overflow: hidden;
  transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
  position: relative;
}

.blog-card::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 3px;
  background: linear-gradient(90deg, var(--accent-primary), var(--accent-secondary));
  transform: scaleX(0);
  transition: transform 0.4s ease;
}

.blog-card:hover {
  transform: translateY(-8px);
  border-color: rgba(255, 59, 29, 0.3);
  box-shadow: 0 25px 50px rgba(0, 0, 0, 0.4);
}

.blog-card:hover::before {
  transform: scaleX(1);
}

.blog-image {
  height: 180px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 3.5rem;
  position: relative;
  overflow: hidden;
}

.blog-image::after {
  content: '';
  position: absolute;
  inset: 0;
  background: linear-gradient(180deg, transparent 50%, rgba(0,0,0,0.3) 100%);
}

.blog-content {
  padding: 24px;
}

.blog-date {
  font-size: 0.8rem;
  color: var(--accent-secondary);
  font-weight: 600;
  margin-bottom: 8px;
  display: flex;
  align-items: center;
  gap: 8px;
}

.blog-date::before {
  content: '';
  width: 8px;
  height: 8px;
  background: var(--accent-primary);
  border-radius: 50%;
}

.blog-title {
  font-size: 1.25rem;
  font-weight: 700;
  margin-bottom: 12px;
  line-height: 1.4;
  color: var(--text-primary);
}

.blog-excerpt {
  font-size: 0.95rem;
  color: var(--text-muted);
  line-height: 1.7;
  margin-bottom: 16px;
}

.blog-read-more {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  color: var(--accent-primary);
  font-weight: 600;
  font-size: 0.9rem;
  transition: gap 0.3s ease;
  text-decoration: none;
}

.blog-read-more:hover {
  gap: 12px;
  color: var(--accent-secondary);
}

/* Reveal animasyonları için */
.reveal {
  opacity: 0;
  transform: translateY(30px);
  transition: all 0.8s ease;
}

.reveal.visible {
  opacity: 1;
  transform: translateY(0);
}
</style>