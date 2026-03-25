<template>
  <section class="section projects" id="projects">
    <div class="container">
      <h2 class="section-title reveal">Projeler</h2>
      
      <!-- Filtreleme -->
      <div class="filter-bar reveal">
        <button 
          v-for="filter in filters" 
          :key="filter"
          class="filter-btn"
          :class="{ active: activeFilter === filter }"
          @click="setFilter(filter)"
        >
          {{ filter }}
        </button>
      </div>
      
      <!-- Proje Grid -->
      <div class="projects-grid">
        <div 
          v-for="(project, index) in filteredProjects" 
          :key="index"
          class="project-card reveal"
          :style="{ animationDelay: index * 0.1 + 's' }"
        >
          <!-- Glow edge -->
          <div class="card-glow"></div>
          
          <!-- Proje görsel -->
          <div class="project-image" :style="{ background: project.gradient }">
            {{ project.icon }}
          </div>
          
          <!-- Proje içerik -->
          <div class="project-content">
            <h3>{{ project.title }}</h3>
            <p>{{ project.description }}</p>
            
            <!-- Etiketler -->
            <div class="project-tags">
              <span v-for="tag in project.tags" :key="tag" class="tag">
                {{ tag }}
              </span>
            </div>
            
            <!-- Buton -->
            <a href="#" class="btn btn-secondary btn-sm">
              Detaylar
            </a>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup>
import { ref, computed } from 'vue'

const filters = ['Tümü', 'Web', 'Mobil', 'Tasarım']
const activeFilter = ref('Tümü')

const projects = [
  {
    title: 'Kişisel Portföy Sitesi',
    description: 'Modern ve responsive tasarıma sahip kişisel portföy web sitesi. Ateş efektleri ve parallax ile dinamik bir deneyim.',
    icon: '🌐',
    gradient: 'linear-gradient(135deg, #FF3B1D, #FF6B35)',
    tags: ['HTML', 'CSS', 'JavaScript'],
    category: 'Web'
  },
  {
    title: 'E-Ticaret Platformu',
    description: 'Ürün kataloğu, sepete ekleme, ödeme entegrasyonu ve yönetim paneli içeren tam kapsamlı e-ticaret çözümü.',
    icon: '🛒',
    gradient: 'linear-gradient(135deg, #6366F1, #8B5CF6)',
    tags: ['Vue.js', 'Node.js', 'MongoDB'],
    category: 'Web'
  },
  {
    title: 'Mobil Banka Uygulaması',
    description: 'Cross-platform mobil banka uygulaması. Güvenli ödeme, hesap yönetimi ve kullanıcı dostu arayüz.',
    icon: '📱',
    gradient: 'linear-gradient(135deg, #06B6D4, #0EA5E9)',
    tags: ['React Native', 'Firebase', 'Redux'],
    category: 'Mobil'
  },
  {
    title: 'Görev Yönetim App',
    description: 'Kişisel görev listesi uygulaması. Sürükle-bırak arayüzü, kategorilendirme ve bildirim özellikleri.',
    icon: '📝',
    gradient: 'linear-gradient(135deg, #10B981, #34D399)',
    tags: ['Vue.js', 'Vuex', 'Firebase'],
    category: 'Web'
  },
  {
    title: 'UI/UX Tasarım Seti',
    description: 'Modern web siteleri için hazır UI bileşenleri ve tasarım sistemi. Figma\'dan export edilmiş bileşenler.',
    icon: '🎨',
    gradient: 'linear-gradient(135deg, #F59E0B, #FBBF24)',
    tags: ['Figma', 'UI Design', 'Prototip'],
    category: 'Tasarım'
  },
  {
    title: 'Sohbet Uygulaması',
    description: 'Real-time sohbet uygulaması. Grup sohbetleri, medya paylaşımı ve okundu bilgisi özellikleri.',
    icon: '💬',
    gradient: 'linear-gradient(135deg, #EC4899, #F472B6)',
    tags: ['Flutter', 'Firebase', 'WebRTC'],
    category: 'Mobil'
  }
]

const filteredProjects = computed(() => {
  if (activeFilter.value === 'Tümü') {
    return projects
  }
  return projects.filter(p => p.category === activeFilter.value)
})

const setFilter = (filter) => {
  activeFilter.value = filter
}
</script>

<style scoped>
.projects {
  position: relative;
}

/* Filtreleme */
.filter-bar {
  display: flex;
  gap: 12px;
  margin-bottom: 40px;
  flex-wrap: wrap;
}

.filter-btn {
  padding: 10px 20px;
  background: var(--bg-card);
  border: 1px solid var(--border);
  border-radius: 25px;
  color: var(--text-muted);
  font-size: 0.9rem;
  font-weight: 500;
  cursor: pointer;
  transition: all var(--animation-speed);
}

.filter-btn:hover {
  border-color: var(--accent-primary);
  color: var(--text-primary);
}

.filter-btn.active {
  background: linear-gradient(135deg, var(--accent-primary), #FF5722);
  border-color: var(--accent-primary);
  color: #fff;
}

/* Grid */
.projects-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(320px, 1fr));
  gap: var(--grid-gap);
}

/* Proje kartı */
.project-card {
  position: relative;
  background: var(--bg-card);
  border: 1px solid var(--border);
  border-radius: var(--card-radius);
  overflow: hidden;
  transition: all var(--animation-speed) var(--animation-smooth);
}

.project-card:hover {
  transform: translateY(-8px);
  border-color: transparent;
}

/* Glow edge effect */
.card-glow {
  position: absolute;
  inset: 0;
  border-radius: var(--card-radius);
  padding: 1px;
  background: linear-gradient(135deg, var(--accent-primary), var(--accent-secondary));
  /* Standart mask + WebKit için -webkit-mask kullanıyoruz */
  mask:
    linear-gradient(#fff 0 0) content-box,
    linear-gradient(#fff 0 0);
  -webkit-mask:
    linear-gradient(#fff 0 0) content-box,
    linear-gradient(#fff 0 0);
  -webkit-mask-composite: xor;
  mask-composite: exclude;
  opacity: 0;
  transition: opacity var(--animation-speed);
}

.project-card:hover .card-glow {
  opacity: 1;
}

/* Proje görsel */
.project-image {
  height: 180px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 4rem;
}

/* İçerik */
.project-content {
  padding: 24px;
}

.project-content h3 {
  font-size: 1.2rem;
  font-weight: 600;
  margin-bottom: 8px;
  color: var(--text-primary);
}

.project-content p {
  font-size: 0.9rem;
  line-height: 1.6;
  margin-bottom: 16px;
}

/* Etiketler */
.project-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
  margin-bottom: 16px;
}

.tag {
  padding: 4px 12px;
  background: rgba(255, 59, 29, 0.1);
  border: 1px solid rgba(255, 59, 29, 0.3);
  border-radius: 15px;
  font-size: 0.75rem;
  color: var(--accent-secondary);
}

.btn-sm {
  padding: 10px 20px;
  font-size: 0.85rem;
}

/* Animasyon */
.project-card {
  animation: fadeInUp 0.6s ease-out backwards;
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
</style>

