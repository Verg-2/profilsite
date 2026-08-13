<template>
  <div class="blog-page">
    <canvas id="particles-canvas"></canvas>

    <header class="page-header">
      <h1>{{ lang === 'en' ? 'Blog & Library' : 'Blog & Kitaplık' }}</h1>
      <p>{{ lang === 'en' ? 'My articles, notes, and technical books.' : 'Yazılarım, notlarım ve oluşturduğum teknik kitaplar.' }}</p>
    </header>

    <main class="blog-container">
      <!-- Kategori Filtreleri -->
      <div class="filter-bar" v-if="categories.length > 0">
        <button
          class="filter-btn"
          :class="{ active: activeCategory === 0 }"
          @click="activeCategory = 0"
        >
          <span>{{ lang === 'en' ? 'All Categories' : 'Tüm Kategoriler' }}</span>
        </button>
        <button
          v-for="cat in categories" :key="cat.id"
          class="filter-btn"
          :class="{ active: activeCategory === cat.id }"
          @click="activeCategory = cat.id"
        >
          <span>
            <i v-if="cat.icon" :class="cat.icon" style="margin-right: 4px;"></i>
            {{ lang === 'en' && cat.nameEn ? cat.nameEn : cat.name }}
          </span>
        </button>
      </div>

      <div v-if="filteredPosts.length > 0" class="blog-grid">

        <!-- YAZILAR (article tipi) -->
        <template v-for="post in filteredPosts" :key="post.id">

          <!-- KITAP KARTI (book tipi) -->
          <router-link v-if="post.postType === 'book'" :to="`/blog/${post.id}`" class="book-card-link">
            <div class="book-card fade-in">
              <div class="book-scene">
                <div class="book-3d" :style="{ '--book-color': post.bookColor || '#1a1a2e' }">
                  <!-- Arka kapak -->
                  <div class="book-back"></div>
                  <!-- Sırt -->
                  <div class="book-spine">
                    <span class="book-spine-title">{{ lang === 'en' && post.titleEn ? post.titleEn : post.title }}</span>
                  </div>
                  <!-- Sayfalar -->
                  <div class="book-pages"></div>
                  <!-- Ön kapak -->
                  <div class="book-front">
                    <img v-if="post.coverImageUrl" :src="getFullUrl(post.coverImageUrl)" style="position:absolute; inset:0; width:100%; height:100%; object-fit:cover; z-index:0; opacity:0.85;" />
                    <div class="book-front-inner" style="position:relative; z-index:1; background:rgba(0,0,0,0.65); width:100%; height:100%; box-sizing:border-box;">
                      <div class="book-icon-wrap">
                        <span v-if="post.icon && post.icon.includes('<svg')" v-safe-html="post.icon" class="svg-icon-wrapper" style="font-size:1.8rem; display:flex; justify-content:center;"></span>
                        <i v-else-if="post.icon && post.icon.includes('fa-')" :class="post.icon" style="font-size:1.8rem;color:rgba(255,255,255,0.8);"></i>
                        <span v-else-if="post.icon" style="font-size:1.8rem;color:rgba(255,255,255,0.9);">{{ post.icon }}</span>
                        <i v-else class="fas fa-book" style="font-size:1.8rem;color:rgba(255,255,255,0.8);"></i>
                      </div>
                      <h3 class="book-title">{{ lang === 'en' && post.titleEn ? post.titleEn : post.title }}</h3>
                      <p class="book-summary">{{ lang === 'en' && post.summaryEn ? post.summaryEn : post.summary }}</p>
                      <div class="book-tags">
                        <span v-for="tag in (post.tags || []).slice(0,2)" :key="tag" class="book-tag">{{ tag.includes('|') ? (lang === 'en' ? tag.split('|')[1] : tag.split('|')[0]) : tag }}</span>
                      </div>
                      <div class="book-date">{{ new Date(post.publishDate).toLocaleDateString(lang === 'en' ? 'en-US' : 'tr-TR', { day: 'numeric', month: 'long', year: 'numeric' }) }}</div>
                    </div>
                  </div>
                </div>
              </div>
              <div class="book-card-label">
                <i class="fas fa-book" style="color:var(--accent);"></i>
                {{ lang === 'en' ? 'Read Book' : 'Kitabı Oku' }}
                <i class="fas fa-arrow-right" style="font-size:0.75rem;margin-left:4px;"></i>
              </div>
            </div>
          </router-link>

          <!-- KISA YAZI KARTI -->
          <article v-else class="blog-card fade-in">
            <div class="blog-image">
              <i v-if="post.icon && post.icon.includes('fa-')" :class="post.icon" style="font-size:3rem;color:#ff3b1d;"></i>
              <span v-else-if="post.icon" style="font-size:3rem;">{{ post.icon }}</span>
              <img v-else-if="post.coverImageUrl" :src="getFullUrl(post.coverImageUrl)" alt="Blog Görseli" style="width:100%;height:100%;object-fit:cover;" />
              <span v-else style="font-size:3rem;">📝</span>
            </div>
            <div class="blog-content">
              <div class="blog-date">{{ new Date(post.publishDate).toLocaleDateString(lang === 'en' ? 'en-US' : 'tr-TR', { day: 'numeric', month: 'long', year: 'numeric' }) }}</div>
              <h2 class="blog-title">{{ lang === 'en' && post.titleEn ? post.titleEn : post.title }}</h2>
              <p class="blog-excerpt">{{ lang === 'en' && post.summaryEn ? post.summaryEn : post.summary }}</p>
              <router-link :to="`/blog/${post.id}`" class="btn btn-primary card-action-btn">{{ lang === 'en' ? 'Read More →' : 'Devamını Oku →' }}</router-link>
            </div>
          </article>

        </template>
      </div>

      <div v-else class="text-center" style="padding:3rem;color:#888;">
        <p>{{ lang === 'en' ? 'No content added yet.' : 'Henüz içerik eklenmemiş.' }}</p>
      </div>
    </main>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted, watch, nextTick, inject } from 'vue'
import api from '@/services/api'

const lang = inject('lang', ref('tr'))

const posts = ref([])
const categories = ref([])
const activeCategory = ref(0)

const filteredPosts = computed(() => {
  let filtered = posts.value

  if (activeCategory.value !== 0) {
    filtered = filtered.filter(p => p.blogCategoryId === activeCategory.value)
  }

  return filtered
})

const getFullUrl = (url) => {
  if (!url) return ''
  if (url.startsWith('http') || url.startsWith('data:')) return url
  return api.defaults.baseURL.replace('/api', '') + url
}

const windowWidth = ref(window.innerWidth)
const windowHeight = ref(window.innerHeight)
const handleResize = () => {
  windowWidth.value = window.innerWidth
  windowHeight.value = window.innerHeight
}

const fetchPosts = async () => {
  try {
    const [postsRes, catsRes] = await Promise.all([
      api.get('/BlogPosts'),
      api.get('/BlogPosts/categories')
    ])
    posts.value = postsRes.data
    categories.value = catsRes.data
    setTimeout(() => {
      document.querySelectorAll('.fade-in').forEach(el => el.classList.add('visible'))
    }, 100)
  } catch (e) { console.error(e) }
}

watch(activeCategory, async () => {
  await nextTick()
  setTimeout(() => {
    document.querySelectorAll('.fade-in').forEach(el => el.classList.add('visible'))
  }, 50)
})

onMounted(() => {
  window.addEventListener('resize', handleResize)
  fetchPosts()
})

onUnmounted(() => {
  window.removeEventListener('resize', handleResize)
})
</script>

<style scoped>
/* ─── Filtre Bar ─── */
.filter-bar {
  display: flex;
  gap: 0.75rem;
  margin-bottom: 2.5rem;
  flex-wrap: wrap;
}
.filter-btn {
  padding: 0.5rem 1.2rem;
  border-radius: 999px;
  border: 1px solid var(--border);
  background: transparent;
  color: var(--text-muted);
  cursor: pointer;
  font-size: 0.88rem;
  font-weight: 600;
  transition: all 0.2s;
  display: flex;
  align-items: center;
  gap: 0.4rem;
}
.filter-btn:hover, .filter-btn.active {
  background: var(--accent);
  color: #fff;
  border-color: var(--accent);
}

/* ─── Kitap Kartı ─── */
.book-card-link { text-decoration: none; display: block; }
.book-card {
  padding: 2rem 1.5rem 1.5rem;
  display: flex;
  flex-direction: column;
  align-items: center;
  background: var(--card-bg);
  border: 1px solid var(--border);
  border-radius: 16px;
  cursor: pointer;
  transition: transform 0.3s, box-shadow 0.3s;
  height: 100%;
}
.book-card:hover { transform: translateY(-8px); box-shadow: 0 20px 40px rgba(0,0,0,0.4); }

/* ─── 3D Kitap ─── */
.book-scene {
  width: 180px;
  height: 220px;
  perspective: 800px;
  margin-bottom: 1.5rem;
}
.book-3d {
  width: 180px;
  height: 220px;
  position: relative;
  transform-style: preserve-3d;
  transform: rotateY(-20deg) rotateX(5deg);
  transition: transform 0.6s ease;
}
.book-card:hover .book-3d { transform: rotateY(-30deg) rotateX(8deg); }

/* Ön kapak */
.book-front {
  position: absolute;
  width: 180px;
  height: 220px;
  background: var(--book-color, #1a1a2e);
  border-radius: 0 4px 4px 0;
  backface-visibility: hidden;
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: inset -3px 0 10px rgba(0,0,0,0.3);
  overflow: hidden;
}
.book-front::before {
  content: '';
  position: absolute;
  left: 0; top: 0;
  width: 4px; height: 100%;
  background: rgba(0,0,0,0.25);
}
.book-front-inner {
  padding: 0.75rem 0.5rem 0.75rem 1.8rem; /* Sol cilt kismindan uzaklastirmak icin left-padding eklendi */
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  text-align: center;
  gap: 0.35rem;
  z-index: 1;
  height: 100%;
  box-sizing: border-box;
}
.svg-icon-wrapper :deep(svg) { width: 1em; height: 1em; display: block; }
.book-icon-wrap { margin-bottom: 0; flex-shrink: 0; }
.book-title {
  font-size: 0.85rem;
  font-weight: 800;
  color: #ffffff;
  line-height: 1.2;
  margin: 0;
  text-shadow: 0 2px 4px rgba(0,0,0,1);
  flex-shrink: 0;
}
.book-summary {
  font-size: 0.65rem;
  color: rgba(255,255,255,0.9);
  line-height: 1.4;
  margin: 0;
  text-shadow: 0 1px 3px rgba(0,0,0,0.8);
  display: -webkit-box;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;
  overflow: hidden;
  flex-shrink: 0;
}
.book-tags { display: flex; gap: 3px; flex-wrap: wrap; justify-content: center; margin-top: 0; flex-shrink: 0; }
.book-tag {
  font-size: 0.6rem;
  padding: 1px 6px;
  border-radius: 999px;
  background: color-mix(in srgb, var(--book-color, var(--accent)) 40%, transparent);
  border: 1px solid color-mix(in srgb, var(--book-color, var(--accent)) 60%, transparent);
  color: rgba(255,255,255,0.9);
}
.book-date { font-size: 0.6rem; color: rgba(255,255,255,0.7); margin-top: 0; font-weight: 500; flex-shrink: 0; }
.book-page-count { font-size: 0.6rem; color: color-mix(in srgb, var(--book-color, #4facfe) 70%, white); margin-top: 0; display: flex; align-items: center; gap: 3px; font-weight: 600; text-shadow: 0 1px 2px rgba(0,0,0,0.5); flex-shrink: 0; }

/* Sırt */
.book-spine {
  position: absolute;
  width: 30px;
  height: 220px;
  background: color-mix(in srgb, var(--book-color, #1a1a2e) 80%, black 20%);
  transform: rotateY(90deg) translateZ(-15px) translateX(-15px);
  display: flex;
  align-items: center;
  justify-content: center;
}
.book-spine-title {
  writing-mode: vertical-rl;
  text-orientation: mixed;
  transform: rotate(180deg);
  color: rgba(255,255,255,0.7);
  font-size: 0.7rem;
  font-weight: 700;
  letter-spacing: 1px;
  max-height: 180px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

/* Arka kapak */
.book-back {
  position: absolute;
  width: 180px;
  height: 220px;
  background: color-mix(in srgb, var(--book-color, #1a1a2e) 70%, black 30%);
  transform: translateZ(-30px);
  border-radius: 0 4px 4px 0;
}

/* Sayfa kalınlığı illüzyonu */
.book-pages {
  position: absolute;
  width: 174px;
  height: 216px;
  left: 3px;
  top: 2px;
  background: repeating-linear-gradient(to right, #e8e0d0, #e8e0d0 1px, #f5f0e8 1px, #f5f0e8 3px);
  transform: translateZ(-3px);
  border-radius: 0 2px 2px 0;
}

.book-card-label {
  font-size: 0.82rem;
  color: var(--text-muted);
  display: flex;
  align-items: center;
  gap: 0.4rem;
  font-weight: 600;
  margin-top: 0.5rem;
  transition: color 0.2s;
}
.book-card:hover .book-card-label { color: var(--accent); }
</style>
