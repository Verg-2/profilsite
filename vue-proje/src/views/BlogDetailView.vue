<template>
  <div class="blog-page">
    <canvas id="particles-canvas"></canvas>

    <!-- ╔═══════════════════════════════╗ -->
    <!-- ║  KİTAP MODU (postType=book)  ║ -->
    <!-- ╚═══════════════════════════════╝ -->
    <div v-if="post && post.postType === 'book'" class="book-reader-wrapper">

      <!-- Kapak Ekranı (ilk açıldığında gösterilir) -->
      <div v-if="showCover" class="book-cover-screen" @click="openBook" :style="`--bc: ${post.bookColor || '#1a1a2e'}`">
        <div class="cover-book-3d">
          <div class="cover-book-spine">
            <span>{{ lang === 'en' && post.titleEn ? post.titleEn : post.title }}</span>
          </div>
          <div class="cover-book-pages"></div>
          <div class="cover-book-front">
            <img v-if="post.coverImageUrl" :src="getFullUrl(post.coverImageUrl)" style="position:absolute; inset:0; width:100%; height:100%; object-fit:cover; z-index:0; opacity:0.85;" />
            <div class="cover-content" :style="post.coverImageUrl ? 'background:linear-gradient(to bottom, rgba(0,0,0,0.2), rgba(0,0,0,0.7));' : ''">
              <span v-if="post.icon && post.icon.includes('<svg')" v-safe-html="post.icon" class="svg-icon-wrapper" style="font-size: 2.2rem; display:flex; justify-content:center; filter: drop-shadow(0 2px 4px rgba(0,0,0,1)) drop-shadow(0 0 10px rgba(0,0,0,0.6));"></span>
              <i v-else-if="post.icon && post.icon.includes('fa-')" :class="post.icon" class="cover-icon"></i>
              <span v-else-if="post.icon" style="font-size: 2.2rem; color: rgba(255,255,255,0.9);">{{ post.icon }}</span>
              <i v-else class="fas fa-book cover-icon"></i>
              <h1 class="cover-title">{{ lang === 'en' && post.titleEn ? post.titleEn : post.title }}</h1>
              <p class="cover-summary">{{ lang === 'en' && post.summaryEn ? post.summaryEn : post.summary }}</p>
              <div class="cover-tags">
                <span v-for="(tag, idx) in (post.tags||[])" :key="tag" class="cover-tag">
                  {{ lang === 'en' ? (post.tagsEn && post.tagsEn[idx] ? post.tagsEn[idx] : (tag.includes('|') ? tag.split('|')[1] : tag)) : (tag.includes('|') ? tag.split('|')[0] : tag) }}
                </span>
              </div>
              <div class="cover-date">{{ new Date(post.publishDate).toLocaleDateString(lang === 'en' ? 'en-US' : 'tr-TR', {day:'numeric',month:'long',year:'numeric'}) }}</div>
            </div>
          </div>
        </div>
        <div class="cover-open-hint">
          <i class="fas fa-hand-pointer"></i>
          {{ lang === 'en' ? 'Click to open the book' : 'Kitabı açmak için tıklayın' }}
        </div>
      </div>

      <!-- Kitap Okuyucu -->
      <div class="book-reader" :style="showCover ? 'position: absolute; visibility: hidden; pointer-events: none; opacity: 0; width: 100%; top: 0; left: 0; z-index: -100;' : ''">

        <!-- Üst Bar -->
        <div class="reader-topbar">
          <router-link to="/blog" class="reader-back-btn">
            <i class="fas fa-arrow-left"></i> Blog
          </router-link>
          <div class="reader-title-bar">
            <i class="fas fa-book" style="color:var(--accent);margin-right:6px;"></i>
            {{ lang === 'en' && post.titleEn ? post.titleEn : post.title }}
          </div>
          <div class="reader-page-info">
            {{ lang === 'en' ? 'Page' : 'Sayfa' }} {{ displayPageNumbers }} / {{ displayTotalPages }}
          </div>
        </div>


        <!-- Kitap Gövdesi (Gerçek Roman Formatı - 3D FlipBook) -->
        <div class="novel-reader-wrapper">
          <div 
            class="novel-container" 
            ref="novelContainer"
            @touchstart="onTouchStart"
            @touchend="onTouchEnd"
          >
            <!-- 3D Book Layout -->
            <div class="book-spread" :class="{ 'is-mobile': isMobile }">
              <!-- Sol Sayfa -->
              <div class="page-half left-half">
                <div class="clip-box">
                  <div class="col-wrapper" :style="{ transform: `translateX(calc(-${leftColIndex * 100}% - ${leftColIndex * 32}px))` }">
                    <div class="novel-content">
                      <div class="printed-toc" v-if="toc.length > 0">
                        <h2 style="text-align:center;color:#ff3b1d;margin-bottom:2rem;break-inside:avoid;">{{ lang === 'en' ? 'Table of Contents' : 'İçindekiler' }}</h2>
                        <div class="toc-line" v-for="item in toc" :key="item.id" @click.stop="scrollToHeading(item.id)">
                          <span :style="item.level === 3 ? 'padding-left:1.5rem;font-size:0.9em;color:#aaa' : ''">{{ item.title }}</span>
                          <span class="toc-dots"></span>
                          <span style="font-weight:bold;color:#ff3b1d;">{{ item.pageNum || '...' }}</span>
                        </div>
                      </div>
                      <div v-safe-html="renderedBookContent"></div>
                    </div>
                  </div>
                </div>
                <div class="printed-page-number">{{ leftColIndex + 1 }}</div>
              </div>
              
              <!-- Sağ Sayfa -->
              <div class="page-half right-half" v-if="!isMobile">
                <div class="clip-box">
                  <div class="col-wrapper" :style="{ transform: `translateX(calc(-${rightColIndex * 100}% - ${rightColIndex * 32}px))` }">
                    <div class="novel-content">
                      <div class="printed-toc" v-if="toc.length > 0">
                        <h2 style="text-align:center;color:#ff3b1d;margin-bottom:2rem;break-inside:avoid;">{{ lang === 'en' ? 'Table of Contents' : 'İçindekiler' }}</h2>
                        <div class="toc-line" v-for="item in toc" :key="item.id" @click.stop="scrollToHeading(item.id)">
                          <span :style="item.level === 3 ? 'padding-left:1.5rem;font-size:0.9em;color:#aaa' : ''">{{ item.title }}</span>
                          <span class="toc-dots"></span>
                          <span style="font-weight:bold;color:#ff3b1d;">{{ item.pageNum || '...' }}</span>
                        </div>
                      </div>
                      <div v-safe-html="renderedBookContent"></div>
                    </div>
                  </div>
                </div>
                <div class="printed-page-number">{{ rightColIndex + 1 }}</div>
              </div>

              <!-- Animasyonlu Çevrilen Yaprak (Flipping Page) -->
              <div v-show="isFlipping" 
                   class="flip-page" 
                   :class="[flipDirection === 'next' ? 'flip-next' : 'flip-prev', isFlippingActive ? 'is-active' : '']">
                <!-- Ön Yüz -->
                <div class="flip-face flip-front">
                  <div class="clip-box">
                    <div class="col-wrapper" :style="{ transform: `translateX(calc(-${flipFrontColIndex * 100}% - ${flipFrontColIndex * 32}px))` }">
                      <div class="novel-content">
                        <div class="printed-toc" v-if="toc.length > 0">
                          <h2 style="text-align:center;color:#ff3b1d;margin-bottom:2rem;break-inside:avoid;">{{ lang === 'en' ? 'Table of Contents' : 'İçindekiler' }}</h2>
                          <div class="toc-line" v-for="item in toc" :key="item.id" @click.stop="scrollToHeading(item.id)">
                            <span :style="item.level === 3 ? 'padding-left:1.5rem;font-size:0.9em;color:#aaa' : ''">{{ item.title }}</span>
                            <span class="toc-dots"></span>
                            <span style="font-weight:bold;color:#ff3b1d;">{{ item.pageNum || '...' }}</span>
                          </div>
                        </div>
                        <div v-safe-html="renderedBookContent"></div>
                      </div>
                    </div>
                  </div>
                  <div class="printed-page-number">{{ flipFrontColIndex + 1 }}</div>
                </div>
                <!-- Arka Yüz -->
                <div class="flip-face flip-back">
                  <div class="clip-box">
                    <div class="col-wrapper" :style="{ transform: `translateX(calc(-${flipBackColIndex * 100}% - ${flipBackColIndex * 32}px))` }">
                      <div class="novel-content">
                        <div class="printed-toc" v-if="toc.length > 0">
                          <h2 style="text-align:center;color:#ff3b1d;margin-bottom:2rem;break-inside:avoid;">{{ lang === 'en' ? 'Table of Contents' : 'İçindekiler' }}</h2>
                          <div class="toc-line" v-for="item in toc" :key="item.id" @click.stop="scrollToHeading(item.id)">
                            <span :style="item.level === 3 ? 'padding-left:1.5rem;font-size:0.9em;color:#aaa' : ''">{{ item.title }}</span>
                            <span class="toc-dots"></span>
                            <span style="font-weight:bold;color:#ff3b1d;">{{ item.pageNum || '...' }}</span>
                          </div>
                        </div>
                        <div v-safe-html="renderedBookContent"></div>
                      </div>
                    </div>
                  </div>
                  <div class="printed-page-number" :style="flipDirection === 'next' ? 'transform: rotateY(180deg);' : 'transform: rotateY(-180deg);'">{{ flipBackColIndex + 1 }}</div>
                </div>
              </div>
              
              <!-- Orta Cilt Çizgisi -->
              <div class="book-spine-line" v-if="!isMobile"></div>
            </div>
          </div>
          
          <!-- Roman Kontrolleri -->
          <div class="novel-controls-wrapper">
            <div class="novel-controls">
              <button class="nav-btn prev-btn" @click="prevPage" :disabled="currentPageIndex === 0 || isFlipping">
                <i class="fas fa-chevron-left"></i> {{ lang === 'en' ? 'Prev' : 'Önceki' }}
              </button>
              
              <div class="novel-progress">
                <span class="progress-text">{{ lang === 'en' ? 'Page' : 'Sayfa' }} {{ displayPageNumbers }} / {{ displayTotalPages }}</span>
                <div class="progress-bar-bg">
                  <div class="progress-bar-fill" :style="{ width: progressPercentage + '%' }"></div>
                </div>
              </div>

              <button class="nav-btn next-btn" @click="nextPage" :disabled="currentPageIndex >= maxPageIndex || isFlipping">
                {{ lang === 'en' ? 'Next' : 'Sonraki' }} <i class="fas fa-chevron-right"></i>
              </button>             
              
              <button class="ctrl-btn ctrl-toc" @click="tocOpen = !tocOpen" title="İçindekiler">
                <i class="fas fa-list-ul"></i>
              </button>
            </div>
            
            <!-- İçindekiler (TOC) Dropdown -->
            <transition name="fade-slide">
              <div class="toc-dropdown" v-if="tocOpen" style="bottom: 100%; margin-bottom: 1rem;">
                <div class="toc-header">
                  <span><i class="fas fa-list-ul" style="color:var(--accent);margin-right:6px;"></i>{{ lang === 'en' ? 'Table of Contents' : 'İçindekiler' }}</span>
                  <button @click="tocOpen = false" style="background:none;border:none;color:var(--text-muted);cursor:pointer;font-size:1.2rem;">×</button>
                </div>
                <div class="toc-list">
                  <div
                    v-for="(item, i) in toc" :key="i"
                    class="toc-item"
                    :class="{ 'toc-sub': item.level === 3 }"
                    @click="scrollToHeading(item.id)"
                  >
                    <span class="toc-item-title">{{ item.title }}</span>
                  </div>
                </div>
              </div>
            </transition>
          </div>
        </div>
      </div>
    </div>

    <!-- ╔═════════════════════════════════╗ -->
    <!-- ║  YAZI MODU (postType=article)  ║ -->
    <!-- ╚═════════════════════════════════╝ -->
    <div v-else-if="post">
      <header class="page-header fade-in">
        <div class="post-badge">
          {{ post.tags && post.tags.length ? (post.tags[0].includes('|') ? (lang === 'en' ? post.tags[0].split('|')[1] : post.tags[0].split('|')[0]) : post.tags[0]).toUpperCase() + ' • ' : '' }}{{ new Date(post.publishDate).toLocaleDateString(lang === 'en' ? 'en-US' : 'tr-TR', { day:'numeric', month:'long', year:'numeric' }) }}
        </div>
        <h1>{{ lang === 'en' && post.titleEn ? post.titleEn : post.title }}</h1>
        <p style="color:var(--text-muted)">{{ lang === 'en' && post.summaryEn ? post.summaryEn : post.summary }}</p>
      </header>

      <main class="blog-container">
        <article class="blog-card fade-in" style="cursor:auto;padding:3rem 2rem;width:100%;display:block;box-sizing:border-box;background:var(--card-bg);border:1px solid var(--border);border-radius:16px;">

          <div v-if="post.coverImageUrl" style="margin-bottom:2rem;text-align:center;">
            <img :src="getFullUrl(post.coverImageUrl)" alt="Blog Görseli" style="max-width:100%;border-radius:8px;" />
          </div>

          <div v-if="post.techIcons && post.techIcons.length" class="tech-icons-row">
            <div v-for="(tech, tIdx) in post.techIcons" :key="tIdx" class="tech-icon-chip">
              <i v-if="tech.includes('|')" :class="tech.split('|')[0]" style="color:var(--accent)"></i>
              <span>{{ tech.includes('|') ? tech.split('|')[1] : tech }}</span>
            </div>
          </div>

          <div class="article-body" v-safe-html="renderedArticle"></div>

          <div v-if="post.proTip" class="protip-box">
            <h4><i class="fa-solid fa-bolt" style="color:var(--accent);"></i> {{ lang === 'en' ? 'Pro Tip' : 'Pratik İpucu' }}</h4>
            <p>{{ post.proTip }}</p>
          </div>

        </article>

        <div class="fade-in" style="margin-top:3rem;text-align:center;margin-bottom:4rem;">
          <router-link to="/blog" class="btn btn-primary" style="font-size:1.1rem;padding:14px 40px;display:inline-flex;align-items:center;">
            <i class="fa-solid fa-arrow-left-long" style="margin-right:12px;"></i> {{ lang === 'en' ? 'Back to Blog' : 'Blog\'a Dön' }}
          </router-link>
        </div>
      </main>
    </div>

    <div v-else class="text-center" style="padding:4rem;color:var(--text-muted);">
      <i class="fas fa-spinner fa-spin fa-2x"></i>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted, nextTick, inject, watch } from 'vue'
import { useRoute } from 'vue-router'
import api from '@/services/api'
import { initPageAnimations, cleanupPageAnimations } from '@/assets/js/page-animations'
import { marked } from 'marked'
import hljs from 'highlight.js'
import 'highlight.js/styles/atom-one-dark.css'

const route = useRoute()
const slug  = route.params.slug
const post  = ref(null)
const lang  = inject('lang', ref('tr'))

// ─── Kitap Okuyucu Durumu ────────────────────────────
const showCover   = ref(true)
const tocOpen     = ref(false)
const toc         = ref([])

const novelContainer = ref(null)
const novelContent = ref(null)
const currentPageIndex = ref(0)
const totalPages = ref(1)
const renderedBookContent = ref('')

// Dokunmatik (Swipe) işlemleri için
let touchStartX = 0

// ─── Markdown ve Kod Renklendirme (Marked + Highlight.js) ───
const renderer = new marked.Renderer();

renderer.code = (token) => {
  const code = token.text || '';
  let language = token.lang || '';
  
  // Highlight.js dil haritalaması
  if (language === 'vue') language = 'xml';
  if (language === 'html') language = 'xml';
  if (language === 'js') language = 'javascript';
  if (language === 'ts') language = 'typescript';
  if (language === 'c#') language = 'csharp';
  
  const validLanguage = hljs.getLanguage(language) ? language : 'plaintext';
  const highlighted = hljs.highlight(validLanguage, code).value;
  
  let displayLang = language || 'text';
  if (displayLang.toLowerCase() === 'csharp' || displayLang.toLowerCase() === 'c#') displayLang = 'C#';
  else if (displayLang.toLowerCase() === 'javascript' || displayLang.toLowerCase() === 'js') displayLang = 'JavaScript';
  else if (displayLang.toLowerCase() === 'typescript' || displayLang.toLowerCase() === 'ts') displayLang = 'TypeScript';
  else if (displayLang.toLowerCase() === 'html') displayLang = 'HTML';
  else if (displayLang.toLowerCase() === 'css') displayLang = 'CSS';
  else if (displayLang.toLowerCase() === 'vue') displayLang = 'Vue';
  
  return `<div class="mac-code-block">
    <div class="mac-header">
      <div class="mac-buttons">
        <span class="mac-btn close"></span>
        <span class="mac-btn minimize"></span>
        <span class="mac-btn maximize"></span>
      </div>
      <span class="mac-lang">${displayLang}</span>
      <button class="mac-copy" onclick="navigator.clipboard.writeText(decodeURIComponent('${encodeURIComponent(code)}')); this.innerHTML='<i class=\\\'fas fa-check\\\'></i>'; setTimeout(()=>this.innerHTML='<i class=\\\'far fa-copy\\\'></i>',2000)"><i class="far fa-copy"></i> Kopyala</button>
    </div>
    <pre class="mac-pre"><code class="hljs">${highlighted}</code></pre>
  </div>`;
};

let headingCounter = 0;
let currentToc = [];
renderer.heading = (token) => {
  const text = token.text;
  const level = token.depth;
  const id = `novel-heading-${headingCounter++}`;
  
  if (level === 2 || level === 3) {
    currentToc.push({ id, title: text.replace(/<[^>]+>/g, ''), level });
  }
  
  if (level === 1) return `<h1 id="${id}">${text}</h1>`;
  if (level === 2) return `<h2 id="${id}">${text}</h2>`;
  if (level === 3) return `<h3 id="${id}">${text}</h3>`;
  return `<h${level} id="${id}">${text}</h${level}>`;
};

marked.setOptions({
  renderer: renderer,
  breaks: true,
  gfm: true
});

const renderMarkdown = (md) => {
  if (!md) return '';
  return marked.parse(md);
}

// ─── Yardımcılar ─────────────────────────────────────
const getFullUrl = (url) => {
  if (!url) return ''
  if (url.startsWith('http') || url.startsWith('data:')) return url
  return api.defaults.baseURL.replace('/api', '') + url
}

const renderedArticle = computed(() => renderMarkdown(lang.value === 'en' && post.value?.contentEn ? post.value.contentEn : (post.value?.content || '')))

// ─── Roman Modu İşlemleri ─────────────────────────────
const buildTOCAndRender = (content) => {
  if (!content) return
  
  headingCounter = 0
  currentToc = []
  
  renderedBookContent.value = renderMarkdown(content)
  toc.value = [...currentToc]
}


// ─── 3D Sayfa Hesaplama ve Çevirme (Flip) ────────────────
const isMobile = ref(false)
const flipDirection = ref(null)
const isFlipping = ref(false)
const isFlippingActive = ref(false)

const flipFrontColIndex = ref(0)
const flipBackColIndex = ref(0)
const leftColIndex = ref(0)
const rightColIndex = ref(0)

const updateColIndices = (pageIdx) => {
  if (isMobile.value) {
    leftColIndex.value = pageIdx
  } else {
    leftColIndex.value = pageIdx * 2
    rightColIndex.value = pageIdx * 2 + 1
  }
}

const checkMobile = () => {
  isMobile.value = window.innerWidth < 768
  updateColIndices(currentPageIndex.value)
}

const calculateTocPages = () => {
  const wrapper = document.querySelector('.col-wrapper')
  const contentEl = document.querySelector('.novel-content')
  if (!wrapper || !contentEl) return
  
  const clientWidth = wrapper.clientWidth
  const gap = 32
  toc.value.forEach(item => {
    const el = contentEl.querySelector(`#${item.id}`)
    if (el) {
      const rect = el.getBoundingClientRect()
      const containerRect = contentEl.getBoundingClientRect()
      const absoluteLeft = rect.left - containerRect.left
      const targetCol = Math.round(absoluteLeft / (clientWidth + gap))
      item.pageNum = targetCol + 1
    }
  })
}

const calculatePages = () => {
  // Sadece ilk sütun kapsayıcısının genişliğinden toplam sütun(sayfa) sayısını buluyoruz
  const wrapper = document.querySelector('.col-wrapper')
  const contentEl = document.querySelector('.novel-content')
  if (!wrapper || !contentEl) return
  
  const clientWidth = wrapper.clientWidth
  const scrollWidth = contentEl.scrollWidth
  const gap = 32
  // Tarayıcılar bazen scrollWidth içine fazladan gap ekleyebiliyor veya yarım piksellik marginler
  // Math.ceil ile birleştiğinde yepyeni bomboş bir sayfa (+1) yaratılmasına sebep oluyor.
  // Bu yüzden Math.round kullanmak, gerçek dolu sütun sayısını bulmak için en kusursuz yoldur.
  totalPages.value = Math.max(1, Math.round(scrollWidth / (clientWidth + gap)))
  
  checkMobile()
  calculateTocPages()
  
  if (currentPageIndex.value > maxPageIndex.value) {
    currentPageIndex.value = maxPageIndex.value
    updateColIndices(currentPageIndex.value)
  }
}

const maxPageIndex = computed(() => {
  if (totalPages.value <= 1) return 0
  return isMobile.value ? totalPages.value - 1 : Math.ceil(totalPages.value / 2) - 1
})

const displayPageNumbers = computed(() => {
  if (isMobile.value) return currentPageIndex.value + 1
  const left = currentPageIndex.value * 2 + 1
  const right = currentPageIndex.value * 2 + 2
  if (right > totalPages.value) return left
  return `${left}-${right}`
})

const displayTotalPages = computed(() => totalPages.value)

const progressPercentage = computed(() => {
  if (maxPageIndex.value <= 0) return 100
  return (currentPageIndex.value / maxPageIndex.value) * 100
})

const nextPage = () => {
  if (isFlipping.value || currentPageIndex.value >= maxPageIndex.value) return
  
  flipDirection.value = 'next'
  
  if (isMobile.value) {
     flipFrontColIndex.value = currentPageIndex.value
     flipBackColIndex.value = currentPageIndex.value + 1
     leftColIndex.value = currentPageIndex.value + 1 // Alt zemin hemen yeni sayfaya geçer
  } else {
     flipFrontColIndex.value = currentPageIndex.value * 2 + 1 // Eski sağ sayfa
     flipBackColIndex.value = (currentPageIndex.value + 1) * 2 // Yeni sol sayfa
     rightColIndex.value = (currentPageIndex.value + 1) * 2 + 1 // Alt zemin hemen yeni sağ sayfa olur
  }
  
  isFlipping.value = true
  
  // Animasyonu başlatmak için Vue reaktivitesini bekle
  requestAnimationFrame(() => {
    requestAnimationFrame(() => {
      isFlippingActive.value = true
    })
  })
  
  setTimeout(() => {
    currentPageIndex.value++
    updateColIndices(currentPageIndex.value)
    isFlipping.value = false
    isFlippingActive.value = false
  }, 600) // Animasyon süresi (0.6s)
}

const prevPage = () => {
  if (isFlipping.value || currentPageIndex.value <= 0) return
  
  flipDirection.value = 'prev'
  
  if (isMobile.value) {
     flipFrontColIndex.value = currentPageIndex.value
     flipBackColIndex.value = currentPageIndex.value - 1
     leftColIndex.value = currentPageIndex.value - 1
  } else {
     flipFrontColIndex.value = currentPageIndex.value * 2 // Eski sol sayfa
     flipBackColIndex.value = (currentPageIndex.value - 1) * 2 + 1 // Yeni sağ sayfa
     leftColIndex.value = (currentPageIndex.value - 1) * 2 // Alt zemin hemen yeni sol sayfa olur
  }
  
  isFlipping.value = true
  
  requestAnimationFrame(() => {
    requestAnimationFrame(() => {
      isFlippingActive.value = true
    })
  })
  
  setTimeout(() => {
    currentPageIndex.value--
    updateColIndices(currentPageIndex.value)
    isFlipping.value = false
    isFlippingActive.value = false
  }, 600)
}

const onTouchStart = (e) => { touchStartX = e.changedTouches[0].screenX }
const onTouchEnd = (e) => {
  const diff = touchStartX - e.changedTouches[0].screenX
  if (diff > 50) nextPage()
  else if (diff < -50) prevPage()
}

const scrollToHeading = (id) => {
  tocOpen.value = false
  const wrapper = document.querySelector('.col-wrapper')
  if (!wrapper) return
  const el = document.getElementById(id)
  if (el) {
    const rect = el.getBoundingClientRect()
    const containerRect = document.querySelector('.novel-content').getBoundingClientRect()
    // İçeriğin başlangıç noktasına olan uzaklık
    const absoluteLeft = rect.left - containerRect.left
    const gap = 32 // css column-gap
    const targetCol = Math.round(absoluteLeft / (wrapper.clientWidth + gap))
    
    let targetPage = isMobile.value ? targetCol : Math.floor(targetCol / 2)
    if (targetPage >= 0 && targetPage <= maxPageIndex.value) {
      currentPageIndex.value = targetPage
      updateColIndices(targetPage)
    }
  }
}

const openBook = () => {
  showCover.value = false
  nextTick(() => {
    setTimeout(calculatePages, 100)
  })
}

// getPageCount fonksiyonu kaldırıldı, yerine gerçek DOM hesaplaması olan displayTotalPages kullanılıyor.

// ─── Veri Çekme ──────────────────────────────────────
const fetchPost = async () => {
  try {
    const res = await api.get(`/BlogPosts/${slug}`)
    post.value = res.data
    if (post.value.postType === 'book') {
      const contentToRender = lang.value === 'en' && post.value.contentEn ? post.value.contentEn : post.value.content
      buildTOCAndRender(contentToRender)
      // DOM güncellendikten sonra sayfa sayısını hesapla
      nextTick(() => {
        setTimeout(() => {
          calculatePages()
          // Resimler yüklendikten sonra tekrar hesapla
          const images = document.querySelectorAll('.novel-content img')
          images.forEach(img => {
            if (!img.complete) {
              img.addEventListener('load', () => setTimeout(calculatePages, 100))
            }
          })
        }, 300)
      })
    }
    await nextTick()
    document.querySelectorAll('.fade-in').forEach(el => el.classList.add('visible'))
  } catch (e) { console.error(e) }
}

watch(lang, () => {
  if (post.value?.postType === 'book') {
    const contentToRender = lang.value === 'en' && post.value.contentEn ? post.value.contentEn : post.value.content
    buildTOCAndRender(contentToRender)
    nextTick(() => {
      setTimeout(() => {
        calculatePages()
      }, 100)
    })
  }
})

let resizeTimer
const onResize = () => {
  clearTimeout(resizeTimer)
  resizeTimer = setTimeout(() => {
    calculatePages()
  }, 200)
}

onMounted(() => {
  window.scrollTo(0, 0)
  fetchPost()
  window.addEventListener('resize', onResize)
  nextTick(() => {
    cleanupPageAnimations()
    initPageAnimations()
  })
})

onUnmounted(() => {
  window.removeEventListener('resize', onResize)
  clearTimeout(resizeTimer)
})
</script>

<style scoped>
/* ─── Genel ─── */
.post-badge {
  display: inline-block;
  padding: 6px 16px;
  border-radius: 999px;
  background: rgba(255, 77, 0, 0.12);
  color: var(--accent);
  margin-bottom: 1rem;
  font-size: 0.85rem;
  font-weight: 800;
  border: 1px solid rgba(255,77,0,0.25);
  text-transform: uppercase;
  letter-spacing: 1px;
}
.tech-icons-row { display: flex; gap: 1rem; margin-bottom: 2rem; flex-wrap: wrap; }
.tech-icon-chip {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  padding: 8px 16px;
  background: var(--dark-bg);
  border: 1px solid var(--border);
  border-radius: 4px;
  font-weight: 600;
  color: var(--text);
}
.protip-box {
  margin-top: 3rem;
  background: var(--dark-bg);
  border: 1px solid var(--border);
  border-left: 4px solid var(--accent);
  padding: 1.5rem;
  border-radius: 0 8px 8px 0;
}
.protip-box h4 { color: var(--text); margin: 0 0 0.5rem; display: flex; align-items: center; gap: 8px; }
.protip-box p  { margin: 0; color: var(--text-muted); }

/* ─── Makale İçerik ─── */
.article-body :deep(h1) { color: #fff; font-size: 1.8rem; margin: 2rem 0 1rem; }
.article-body :deep(h2) { color: var(--accent); font-size: 1.4rem; margin: 2rem 0 0.75rem; border-bottom: 1px solid var(--border); padding-bottom: 0.4rem; }
.article-body :deep(h3) { color: #e0e0e0; font-size: 1.15rem; margin: 1.5rem 0 0.5rem; }
.article-body :deep(p)  { color: var(--text); line-height: 1.8; margin: 0.75rem 0; }
.article-body :deep(strong) { color: #fff; }
.article-body :deep(em) { color: #aaa; font-style: italic; }
.article-body :deep(.inline-code) { background: rgba(255,59,29,0.1); color: var(--accent); padding: 2px 7px; border-radius: 4px; font-family: monospace; }
.article-body :deep(.code-block) { background: #0d0d0d; border: 1px solid #333; border-radius: 8px; padding: 1.2rem; overflow-x: auto; margin: 1rem 0; }
.article-body :deep(code) { font-family: 'Fira Code', monospace; font-size: 0.88rem; color: #e0e0e0; }
.article-body :deep(blockquote) { border-left: 3px solid var(--accent); padding: 0.5rem 1rem; color: #aaa; font-style: italic; margin: 1rem 0; }
.article-body :deep(li) { color: var(--text); margin: 0.3rem 0 0.3rem 1.5rem; line-height: 1.7; }
.article-body :deep(hr) { border: none; border-top: 1px solid var(--border); margin: 2rem 0; }

/* ╔════════════════════════╗
   ║  KİTAP OKUYUCU CSS    ║
   ╚════════════════════════╝ */

.book-reader-wrapper { min-height: 100vh; }

/* ─── Kapak Ekranı ─── */
.book-cover-screen {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  gap: 3rem;
  background: radial-gradient(ellipse at center, rgba(255,59,29,0.06) 0%, transparent 70%);
}

.cover-book-3d {
  width: 280px;
  height: 380px;
  position: relative;
  transform-style: preserve-3d;
  transform: rotateY(-25deg) rotateX(8deg);
  transition: transform 0.5s ease;
  filter: drop-shadow(-30px 30px 60px rgba(0,0,0,0.7));
}
.book-cover-screen:hover .cover-book-3d { transform: rotateY(-15deg) rotateX(5deg) scale(1.03); }

.cover-book-front {
  position: absolute;
  width: 280px; height: 380px;
  background: var(--bc, #1a1a2e);
  border-radius: 0 6px 6px 0;
  display: flex; align-items: center; justify-content: center;
  box-shadow: inset -4px 0 15px rgba(0,0,0,0.3);
  overflow: hidden;
}
.cover-book-front::before {
  content: '';
  position: absolute;
  left: 0; top: 0;
  width: 5px; height: 100%;
  background: rgba(0,0,0,0.3);
}
.cover-book-spine {
  position: absolute;
  width: 40px; height: 380px;
  background: color-mix(in srgb, var(--bc, #1a1a2e) 75%, black 25%);
  transform: rotateY(90deg) translateZ(-20px) translateX(-20px);
  display: flex; align-items: center; justify-content: center;
}
.cover-book-spine span {
  writing-mode: vertical-rl;
  transform: rotate(180deg);
  color: rgba(255,255,255,0.6);
  font-size: 0.8rem;
  font-weight: 700;
  letter-spacing: 1px;
  max-height: 340px;
  overflow: hidden;
}
.cover-book-pages {
  position: absolute;
  width: 272px; height: 374px;
  left: 4px; top: 3px;
  background: repeating-linear-gradient(to right, #e8e0d0, #e8e0d0 1px, #f5f0e8 1px, #f5f0e8 3px);
  transform: translateZ(-6px);
  border-radius: 0 3px 3px 0;
}

.cover-content {
  display: flex; flex-direction: column;
  align-items: center; text-align: center;
  padding: 1.5rem 0.5rem 1.5rem 4rem; /* Sol cilt kısmından daha da uzaklaştırmak için artırıldı */
  gap: 0.75rem; z-index: 1;
  width: 100%; height: 100%; box-sizing: border-box; justify-content: center;
}
.svg-icon-wrapper :deep(svg) { width: 1em; height: 1em; display: block; }
.cover-icon { font-size: 2.2rem; color: #ffffff; text-shadow: 0 2px 4px rgba(0,0,0,1), 0 0 10px rgba(0,0,0,0.6); }
.cover-title { font-size: 1.25rem; font-weight: 900; color: #ffffff; line-height: 1.3; margin: 0; word-break: break-word; text-shadow: 0 2px 4px rgba(0,0,0,1), 0 0 12px rgba(0,0,0,0.7); display: -webkit-box; -webkit-line-clamp: 3; -webkit-box-orient: vertical; overflow: hidden; }
.cover-summary { font-size: 0.75rem; color: #ffffff; font-weight: 500; line-height: 1.5; margin: 0; text-shadow: 0 1px 3px rgba(0,0,0,1), 0 0 8px rgba(0,0,0,0.8); display: -webkit-box; -webkit-line-clamp: 4; -webkit-box-orient: vertical; overflow: hidden; word-break: break-word; }
.cover-tags { display: flex; gap: 4px; flex-wrap: wrap; justify-content: center; }
.cover-tag { font-size: 0.65rem; padding: 2px 8px; border-radius: 999px; background: color-mix(in srgb, var(--bc) 40%, transparent); border: 1px solid color-mix(in srgb, var(--bc) 60%, transparent); color: rgba(255,255,255,0.9); }
.cover-date { font-size: 0.68rem; color: rgba(255,255,255,0.7); font-weight: 500; }

.cover-open-hint {
  display: flex; align-items: center; gap: 0.5rem;
  color: var(--text-muted);
  font-size: 0.88rem;
  animation: pulse 2s infinite;
}
@keyframes pulse { 0%,100%{opacity:0.5;} 50%{opacity:1;} }

/* ─── Okuyucu Layout ─── */
.book-reader { max-width: 1100px; margin: 0 auto; padding: 1rem 1.5rem 4rem; }

.reader-topbar {
  display: flex; align-items: center; justify-content: space-between;
  padding: 1rem 0; margin-bottom: 1.5rem;
  border-bottom: 1px solid var(--border);
  gap: 1rem;
}
.reader-back-btn {
  color: var(--text-muted);
  text-decoration: none;
  display: flex; align-items: center; gap: 6px;
  font-size: 0.85rem; font-weight: 600;
  transition: color 0.2s;
  white-space: nowrap;
}
.reader-back-btn:hover { color: var(--accent); }
.reader-title-bar {
  font-weight: 700; color: var(--text);
  font-size: 0.95rem;
  text-overflow: ellipsis; overflow: hidden; white-space: nowrap;
}
.reader-page-info { color: var(--text-muted); font-size: 0.82rem; white-space: nowrap; }

/* ─── Alt İçindekiler Dropdown ─── */
.toc-dropdown {
  position: absolute;
  bottom: calc(100% + 15px); /* Kontrol çubuğunun hemen ÜSTÜNDE açılır */
  left: 50%;
  transform: translateX(-50%);
  width: 90%;
  max-width: 450px;
  max-height: 55vh;
  background: var(--card-bg);
  border: 1px solid rgba(255,255,255,0.08);
  border-radius: 12px;
  box-shadow: 0 -10px 40px rgba(0,0,0,0.6);
  z-index: 1000;
  display: flex; flex-direction: column;
  overflow: hidden;
  backdrop-filter: blur(10px);
}
.fade-slide-enter-active, .fade-slide-leave-active { transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1); }
.fade-slide-enter-from, .fade-slide-leave-to { opacity: 0; transform: translate(-50%, 15px); }
.toc-header {
  display: flex; align-items: center; justify-content: space-between;
  padding: 1.2rem 1.5rem;
  border-bottom: 1px solid var(--border);
  font-weight: 700; color: var(--text);
}

/* ─── Yeni Roman Gövdesi (Novel Format - 3D Flip) ─── */
.novel-reader-wrapper {
  display: flex;
  flex-direction: column;
  align-items: center;
  width: 100%;
  position: relative;
}

.novel-container {
  width: 100%;
  height: 650px;
  max-height: calc(100vh - 150px);
  min-height: 400px;
  background: var(--card-bg);
  border: 1px solid var(--border);
  border-radius: 12px;
  box-shadow: inset 0 0 20px rgba(0,0,0,0.3), 0 25px 50px rgba(0,0,0,0.5);
  position: relative;
  /* 3D derinliği için perspective */
  perspective: 2500px;
}

.book-spread {
  width: 100%;
  height: 100%;
  position: relative;
  display: flex;
  transform-style: preserve-3d;
}

.book-spread.is-mobile .page-half {
  width: 100%;
}

.page-half {
  width: 50%;
  height: 100%;
  position: relative;
  overflow: hidden;
  box-sizing: border-box;
  background: var(--card-bg);
  background-image: linear-gradient(to right, rgba(0,0,0,0.08) 0%, rgba(255,255,255,0.01) 5%, rgba(255,255,255,0.01) 95%, rgba(0,0,0,0.08) 100%);
}

.clip-box {
  position: absolute;
  top: 3rem;
  bottom: 3rem;
  overflow: hidden;
}

.left-half {
  border-right: 1px solid rgba(0,0,0,0.5);
}
.left-half .clip-box { left: 4rem; right: 2rem; }

.right-half {
  border-left: 1px solid rgba(255,255,255,0.05);
}
.right-half .clip-box { left: 2rem; right: 4rem; }

.col-wrapper {
  width: 100%;
  height: 100%;
}

/* ─── 3D Flip Animasyon Yapısı ─── */
.flip-page {
  position: absolute;
  top: 0;
  width: 50%;
  height: 100%;
  transform-style: preserve-3d;
  -webkit-transform-style: preserve-3d;
  z-index: 10;
  transition: transform 0.6s cubic-bezier(0.645, 0.045, 0.355, 1);
  will-change: transform; /* Performans optimizasyonu */
}

.flip-page.flip-next {
  right: 0;
  transform-origin: left center;
}
.flip-page.flip-next.is-active {
  transform: rotateY(-180deg) translateZ(0);
}

.flip-page.flip-prev {
  left: 0;
  transform-origin: right center;
}
.flip-page.flip-prev.is-active {
  transform: rotateY(180deg) translateZ(0);
}

.book-spread.is-mobile .flip-page {
  width: 100%;
}
.book-spread.is-mobile .flip-page.flip-next {
  right: 0;
  transform-origin: left center;
}
.book-spread.is-mobile .flip-page.flip-prev {
  left: 0;
  transform-origin: right center;
}
.book-spread.is-mobile .flip-page.flip-prev.is-active {
  transform: rotateY(90deg) translateZ(0); 
}
.book-spread.is-mobile .flip-page.flip-next.is-active {
  transform: rotateY(-90deg) translateZ(0); 
}

.book-spread.is-mobile .clip-box {
  left: 1.5rem;
  right: 1.5rem;
}

.flip-face {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  backface-visibility: hidden;
  -webkit-backface-visibility: hidden;
  transform: translateZ(0); /* Donanım hızlandırmayı zorlar */
  will-change: transform;
  box-sizing: border-box;
  background: var(--card-bg);
  overflow: hidden;
  background-image: linear-gradient(to right, rgba(0,0,0,0.08) 0%, rgba(255,255,255,0.01) 5%, rgba(255,255,255,0.01) 95%, rgba(0,0,0,0.08) 100%);
}

.flip-next .flip-back {
  transform: rotateY(180deg); 
}
.flip-prev .flip-back {
  transform: rotateY(-180deg); 
}

/* Clip-box for flip faces */
.flip-next .flip-front .clip-box { left: 2rem; right: 4rem; }
.flip-next .flip-back .clip-box { left: 4rem; right: 2rem; }

.flip-prev .flip-front .clip-box { left: 4rem; right: 2rem; }
.flip-prev .flip-back .clip-box { left: 2rem; right: 4rem; }

.book-spine-line {
  position: absolute;
  top: 0;
  bottom: 0;
  left: 50%;
  width: 40px;
  transform: translateX(-50%);
  background: linear-gradient(to right, transparent 0%, rgba(0,0,0,0.2) 45%, rgba(0,0,0,0.6) 50%, rgba(255,255,255,0.1) 52%, transparent 100%);
  z-index: 5;
  pointer-events: none;
}

.novel-content {
  column-count: 1; 
  column-gap: 32px; 
  height: 100%;
  width: 100%;
  font-family: 'Inter', 'Segoe UI', 'Roboto', 'Helvetica Neue', sans-serif;
  font-size: 16px; 
  line-height: 1.8;
  text-align: justify;
  color: #e5e7eb;
  letter-spacing: 0.3px;
  box-sizing: border-box;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-rendering: optimizeLegibility;
  transform: translateZ(0);
  backface-visibility: hidden;
}

.novel-content * {
  max-width: 100%;
}

.printed-toc {
  break-after: always;
  page-break-after: always;
  margin-bottom: 2rem;
  padding-bottom: 2rem;
  border-bottom: 2px dashed rgba(255,255,255,0.1);
  width: 100%;
}
.toc-line {
  display: flex;
  align-items: center;
  margin-bottom: 0.8rem;
  cursor: pointer;
  transition: opacity 0.2s;
}
.toc-line:hover {
  opacity: 0.7;
}
.toc-dots {
  flex: 1;
  border-bottom: 1px dotted rgba(255,255,255,0.3);
  margin: 0 10px;
  position: relative;
  top: -4px;
}
.printed-page-number {
  position: absolute;
  bottom: 0.8rem;
  left: 0;
  width: 100%;
  text-align: center;
  font-size: 0.85rem;
  font-weight: bold;
  color: rgba(255, 255, 255, 0.4);
  font-family: 'Garamond', serif;
  pointer-events: none;
}

/* Sütun içerisindeki kural ihlallerini önle */
.novel-content p {
  margin-bottom: 1.5em;
  text-indent: 1.5em; /* Roman satır başı girintisi */
}
.novel-content h1, .novel-content h2, .novel-content h3 {
  text-indent: 0;
  break-inside: avoid;
  margin-top: 0;
}
.novel-content img {
  max-width: 100%;
  border-radius: 8px;
  break-inside: avoid-column;
}
.novel-content pre {
  break-inside: avoid-column; /* Kod bloklarını sayfa ortasından bölme */
  white-space: pre-wrap;
  font-size: 0.85rem;
}

/* ─── Roman Kontrolleri (Alt Kısım) ─── */
.novel-controls-wrapper {
  position: relative;
  width: 100%;
  max-width: 600px;
  margin-top: 1.5rem;
}

.novel-controls {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 0.5rem;
  width: 100%;
}

.novel-progress {
  flex: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.5rem;
  min-width: 0;
}

.progress-text {
  font-size: 0.8rem;
  color: var(--text-muted);
  font-weight: 600;
  letter-spacing: 1px;
  white-space: nowrap;
}

.nav-btn {
  background: transparent;
  border: none;
  color: var(--text);
  font-size: 0.95rem;
  font-weight: 600;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 8px 12px;
  border-radius: 8px;
  transition: all 0.2s;
  white-space: nowrap;
}
.nav-btn:hover:not(:disabled) {
  background: rgba(255, 255, 255, 0.05);
  color: var(--accent);
}
.nav-btn:disabled {
  opacity: 0.3;
  cursor: not-allowed;
}

.progress-bar-bg {
  width: 100%;
  height: 6px;
  background: rgba(255, 255, 255, 0.1);
  border-radius: 4px;
  cursor: pointer;
  position: relative;
  overflow: hidden;
}

.progress-bar-fill {
  height: 100%;
  background: var(--accent);
  border-radius: 4px;
  transition: width 0.2s ease-out;
}

/* ─── Butonlar ─── */
.ctrl-btn {
  width: 44px; height: 44px;
  border-radius: 50%;
  border: 1px solid var(--border);
  background: var(--card-bg);
  color: var(--text);
  font-size: 1.1rem;
  cursor: pointer;
  display: flex; align-items: center; justify-content: center;
  transition: all 0.2s;
}
.ctrl-btn:hover:not(:disabled) {
  background: var(--primary);
  color: #fff;
  border-color: var(--primary);
  transform: translateY(-2px);
}
.ctrl-btn:disabled { opacity: 0.4; cursor: not-allowed; }

.toc-list { flex: 1; overflow-y: auto; padding: 1rem 0; }
.toc-item {
  display: flex; align-items: center; gap: 0.75rem;
  padding: 0.6rem 1.5rem;
  cursor: pointer; transition: background 0.15s;
  color: var(--text);
  font-size: 0.88rem;
}
.toc-item:hover { background: var(--border); }
.toc-item.toc-active { background: rgba(255,59,29,0.08); color: var(--accent); }
.toc-sub { padding-left: 2.5rem; color: var(--text-muted); font-size: 0.82rem; }
.toc-page-num {
  min-width: 24px; height: 24px;
  background: var(--accent); color: #fff;
  border-radius: 50%; font-size: 0.72rem; font-weight: 700;
  display: flex; align-items: center; justify-content: center;
  flex-shrink: 0;
}
.toc-item-title { flex: 1; }

/* ─── Kitap Gövdesi ─── */
.book-body {
  display: grid;
  grid-template-columns: 1fr 4px 4px 1fr;
  height: calc(100vh - 180px); /* Ekrana tam sığması için */
  min-height: 400px;
  max-height: 800px;
  background: var(--card-bg);
  border: 1px solid var(--border);
  border-radius: 12px;
  overflow: hidden;
  box-shadow: 0 20px 60px rgba(0,0,0,0.4);
  position: relative;
}

.book-left-panel, .book-right-panel {
  padding: 2.5rem 2rem;
  height: 100%; /* book-body yüksekliğini al */
  overflow-y: auto; /* İçerik taşarsa aşağı kaydır */
  overflow-x: hidden;
  position: relative;
}

/* Scrollbar tasarımı */
.book-left-panel::-webkit-scrollbar,
.book-right-panel::-webkit-scrollbar {
  width: 4px;
}
.book-left-panel::-webkit-scrollbar-thumb,
.book-right-panel::-webkit-scrollbar-thumb {
  background: var(--border);
  border-radius: 10px;
}
.book-left-panel::-webkit-scrollbar-track,
.book-right-panel::-webkit-scrollbar-track {
  background: transparent;
}
.book-left-panel { border-right: none; background: rgba(255,255,255,0.01); }
.book-right-panel {
  cursor: pointer;
  transition: background 0.2s;
}
.book-right-panel:hover { background: rgba(255,255,255,0.02); }

.book-spine-center {
  width: 8px;
  background: linear-gradient(to right, rgba(0,0,0,0.3), rgba(0,0,0,0.05), rgba(0,0,0,0.3));
  grid-column: span 2;
}

.book-left-content, .book-right-content {
  height: 100%;
  display: flex; flex-direction: column;
}

.page-num-bottom {
  margin-top: auto;
  padding-top: 1rem;
  border-top: 1px solid var(--border);
  color: var(--text-muted);
  font-size: 0.75rem;
}

/* ─── Sayfa Çevirme ─── */
.page-fold-zone {
  position: absolute;
  right: 50%;
  top: 0; bottom: 0;
  width: 0;
  background: var(--bc, #1a1a2e);
  transform-origin: right center;
  transition: width 0.4s ease, opacity 0.4s;
  opacity: 0;
  pointer-events: none;
}
.page-fold-zone.folding {
  width: 50%;
  opacity: 0.8;
}

.turn-hint {
  position: absolute;
  bottom: 1.5rem; right: 1.5rem;
  color: var(--text-muted);
  font-size: 1.3rem;
  animation: hint-pulse 2s infinite;
}
@keyframes hint-pulse { 0%,100%{opacity:0.3;transform:translateX(0);} 50%{opacity:0.7;transform:translateX(4px);} }

/* ─── İçindekiler (Sayfa İçi) ─── */
.toc-inside { padding: 0.5rem 0; }
.toc-inside-title {
  font-size: 1.1rem; font-weight: 800;
  color: var(--accent); margin-bottom: 1.5rem;
  display: flex; align-items: center; gap: 0.5rem;
  border-bottom: 1px solid var(--border); padding-bottom: 0.75rem;
}
.toc-inside-list { display: flex; flex-direction: column; gap: 0.5rem; }
.toc-inside-item {
  display: flex; align-items: baseline; gap: 0.5rem;
  cursor: pointer; padding: 0.25rem 0;
  transition: color 0.2s;
}
.toc-inside-item:hover .toc-inside-text { color: var(--accent); }
.toc-inside-sub { padding-left: 1.5rem; }
.toc-inside-num {
  min-width: 22px; height: 22px; border-radius: 50%;
  background: var(--accent); color: #fff;
  font-size: 0.7rem; font-weight: 700;
  display: flex; align-items: center; justify-content: center;
  flex-shrink: 0;
}
.toc-inside-text { flex: 1; color: var(--text); font-size: 0.88rem; font-weight: 600; }
.toc-inside-dots { flex: 1; border-bottom: 1px dotted var(--border); margin: 0 0.5rem; }
.toc-inside-page { color: var(--text-muted); font-size: 0.78rem; }

/* ─── Sayfa İçerik ─── */
.novel-content :deep(h1), .article-body :deep(h1) { color: var(--accent); font-size: 1.3rem; margin: 0 0 1.2rem; border-bottom: 1px solid var(--border); padding-bottom: 0.5rem; display: flex; align-items: center; }
.novel-content :deep(h2), .article-body :deep(h2) { 
  font-size: 1.25rem; 
  margin: 1.8rem 0 0.8rem; 
  border-bottom: 1px dashed rgba(255,255,255,0.1); 
  padding-bottom: 0.4rem; 
  display: flex; 
  align-items: center; 
  background: linear-gradient(90deg, #ff6b6b, #feca57);
  -webkit-background-clip: text;
  background-clip: text;
  -webkit-text-fill-color: transparent;
  font-weight: 800;
}
.novel-content :deep(h3), .article-body :deep(h3) { 
  font-size: 1.05rem; 
  margin: 1.2rem 0 0.5rem; 
  display: flex; 
  align-items: center; 
  color: #48dbfb;
  font-weight: 700;
}
.novel-content :deep(p), .article-body :deep(p)  { color: #e5e7eb; line-height: 1.7; margin: 0.6rem 0; font-size: 16px; font-style: normal; }
.novel-content :deep(strong), .article-body :deep(strong) { color: #fff; background: rgba(255,255,255,0.05); padding: 0 4px; border-radius: 4px; font-weight: 700; }
.novel-content :deep(.inline-code), .article-body :deep(.inline-code) { background: rgba(255,59,29,0.1); color: var(--accent); padding: 1px 5px; border-radius: 4px; font-family: monospace; font-size: 0.84em; border: 1px solid rgba(255,59,29,0.2); }
.novel-content :deep(blockquote), .article-body :deep(blockquote) { border-left: 3px solid var(--accent); padding: 0.4rem 1rem; color: #aaa; font-style: italic; margin: 1rem 0; background: linear-gradient(90deg, rgba(255,59,29,0.05), transparent); border-radius: 0 6px 6px 0; }
.novel-content :deep(li), .article-body :deep(li) { color: #e5e7eb; margin: 0.25rem 0 0.25rem 1.2rem; font-size: 16px; line-height: 1.7; font-style: normal; }
.novel-content :deep(hr), .article-body :deep(hr) { border: none; border-top: 1px solid var(--border); margin: 1.2rem 0; }
.novel-content :deep(> *:last-child), .article-body :deep(> *:last-child) { margin-bottom: 0 !important; padding-bottom: 0 !important; }

/* ─── VS Code Tarzı Kod Blokları ─── */
.novel-content :deep(.mac-code-block), .article-body :deep(.mac-code-block) {
  background: #1e1e1e;
  border: 1px solid #333;
  border-radius: 8px;
  margin: 1.2rem 0;
  overflow: hidden;
  box-shadow: 0 4px 15px rgba(0,0,0,0.3);
  max-width: 100%;
  break-inside: avoid-column;
}
.novel-content :deep(.mac-header), .article-body :deep(.mac-header) {
  background: #252526;
  padding: 8px 16px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  border-bottom: 1px solid #333;
}
.novel-content :deep(.mac-lang), .article-body :deep(.mac-lang) {
  color: #9cdcfe;
  font-family: monospace;
  font-size: 0.85rem;
  font-weight: 600;
  text-transform: lowercase;
}
.novel-content :deep(.mac-copy), .article-body :deep(.mac-copy) {
  background: transparent;
  border: none;
  color: #858585;
  cursor: pointer;
  font-size: 0.8rem;
  display: flex;
  align-items: center;
  gap: 6px;
  transition: color 0.2s;
}
.novel-content :deep(.mac-copy:hover), .article-body :deep(.mac-copy:hover) { color: #d4d4d4; }
.novel-content :deep(.mac-copy:hover), .article-body :deep(.mac-copy:hover) { color: #d4d4d4; }
.novel-content :deep(.mac-buttons), .article-body :deep(.mac-buttons) { display: flex; gap: 6px; align-items: center; }
.novel-content :deep(.mac-btn), .article-body :deep(.mac-btn) { width: 12px; height: 12px; border-radius: 50%; display: inline-block; }
.novel-content :deep(.mac-btn.close), .article-body :deep(.mac-btn.close) { background: #ff5f56; }
.novel-content :deep(.mac-btn.minimize), .article-body :deep(.mac-btn.minimize) { background: #ffbd2e; }
.novel-content :deep(.mac-btn.maximize), .article-body :deep(.mac-btn.maximize) { background: #27c93f; }

.novel-content :deep(.mac-pre), .article-body :deep(.mac-pre) {
  margin: 0;
  padding: 1rem;
  overflow-x: auto;
  overflow-y: auto;
  max-height: 350px;
  scrollbar-width: thin;
  scrollbar-color: #444 #1e1e1e;
}

/* Scrollbar stilleri global style tagine taşındı */
.novel-content :deep(.mac-pre code), .article-body :deep(.mac-pre code) {
  font-family: 'Fira Code', 'Consolas', monospace;
  font-size: 0.85rem;
  line-height: 1.5;
  color: #d4d4d4; /* VS Code varsayılan yazı rengi */
}
.novel-content :deep(.hljs-comment), .article-body :deep(.hljs-comment) { color: #6a9955; font-style: italic; }
.novel-content :deep(.hljs-string), .article-body :deep(.hljs-string), .novel-content :deep(.hljs-meta-string), .article-body :deep(.hljs-meta-string) { color: #ce9178; }
.novel-content :deep(.hljs-number), .article-body :deep(.hljs-number) { color: #b5cea8; }
.novel-content :deep(.hljs-keyword), .article-body :deep(.hljs-keyword) { color: #569cd6; font-weight: bold; }
.novel-content :deep(.hljs-built_in), .article-body :deep(.hljs-built_in) { color: #4ec9b0; }
.novel-content :deep(.hljs-title.function_), .article-body :deep(.hljs-title.function_), .novel-content :deep(.hljs-title.class_), .article-body :deep(.hljs-title.class_) { color: #dcdcaa; }
.novel-content :deep(.hljs-tag), .article-body :deep(.hljs-tag) { color: #808080; }
.novel-content :deep(.hljs-name), .article-body :deep(.hljs-name) { color: #569cd6; }
.novel-content :deep(.hljs-attr), .article-body :deep(.hljs-attr), .novel-content :deep(.hljs-attribute), .article-body :deep(.hljs-attribute) { color: #9cdcfe; }
.novel-content :deep(.hljs-selector-tag), .article-body :deep(.hljs-selector-tag) { color: #dcdcaa; }
.novel-content :deep(.hljs-property), .article-body :deep(.hljs-property) { color: #9cdcfe; }
.novel-content :deep(.hljs-literal), .article-body :deep(.hljs-literal) { color: #569cd6; }
.novel-content :deep(.hljs-variable), .article-body :deep(.hljs-variable) { color: #9cdcfe; }
.novel-content :deep(.hljs-type), .article-body :deep(.hljs-type) { color: #4ec9b0; }
.novel-content :deep(.hljs-params), .article-body :deep(.hljs-params) { color: #9cdcfe; }

/* ─── Alt Kontroller (Şık Tasarım) ─── */
.reader-controls {
  display: flex; align-items: center; justify-content: space-between;
  margin-top: 0.5rem; padding: 0.6rem 1.5rem;
  background: rgba(255,255,255,0.02);
  border: 1px solid rgba(255,255,255,0.05);
  border-radius: 12px;
  gap: 1rem;
  box-shadow: 0 10px 30px rgba(0,0,0,0.2), inset 0 1px 0 rgba(255,255,255,0.05);
  backdrop-filter: blur(10px);
  width: 100%;
}
.ctrl-btn {
  padding: 0.6rem 1.2rem;
  background: rgba(255,255,255,0.03);
  border: 1px solid rgba(255,255,255,0.08);
  color: var(--text);
  border-radius: 8px;
  cursor: pointer;
  font-size: 0.85rem;
  font-weight: 600;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  display: flex; align-items: center; gap: 0.5rem;
  white-space: nowrap;
}
.ctrl-btn:not(:disabled):hover { 
  background: var(--accent); 
  border-color: var(--accent); 
  color: #fff; 
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(255, 59, 29, 0.3);
}
.ctrl-btn:disabled { opacity: 0.3; cursor: default; }
.ctrl-toc { 
  border-color: rgba(255, 59, 29, 0.3); 
  color: var(--accent); 
  background: rgba(255, 59, 29, 0.05);
}
.ctrl-toc:hover {
  background: var(--accent) !important;
  color: #fff !important;
}
.page-dots { display: flex; align-items: center; gap: 8px; flex-wrap: wrap; }
.dot {
  width: 8px; height: 8px; border-radius: 50%;
  background: rgba(255,255,255,0.15); cursor: pointer; transition: all 0.3s;
}
.dot:hover { background: rgba(255,255,255,0.4); }
.dot.active { background: var(--accent); transform: scale(1.4); box-shadow: 0 0 10px var(--accent); }

/* Mobil Alt Kontrol Yerleşimi (Dar ekranlarda üst üste binmeyi önler) */
@media (max-width: 480px) {
  .novel-controls {
    flex-wrap: wrap;
    justify-content: center;
  }
  .prev-btn { order: 1; flex: 1; justify-content: center; }
  .ctrl-toc { order: 2; margin: 0 0.5rem; }
  .next-btn { order: 3; flex: 1; justify-content: center; }
  
  .novel-progress {
    order: 4;
    flex: 0 0 100%;
    margin-top: 0.5rem;
  }
}

/* ─── AÇIK TEMA (Light Theme) ─── */
html[data-theme='light'] .novel-container {
  background: #fdfbf7;
  border: 1px solid #e0dbce;
  box-shadow: inset 0 0 20px rgba(0,0,0,0.05), 0 25px 50px rgba(0,0,0,0.15);
}
html[data-theme='light'] .novel-container .page-half,
html[data-theme='light'] .novel-container .flip-face {
  background: #fdfbf7;
  background-image: linear-gradient(to right, rgba(0,0,0,0.04) 0%, rgba(255,255,255,0.4) 5%, rgba(255,255,255,0.4) 95%, rgba(0,0,0,0.04) 100%);
}
html[data-theme='light'] .novel-container .left-half { border-right: 1px solid rgba(0,0,0,0.15); }
html[data-theme='light'] .novel-container .right-half { border-left: 1px solid rgba(255,255,255,0.8); }
html[data-theme='light'] .novel-container .book-spine-line {
  background: linear-gradient(to right, transparent 0%, rgba(0,0,0,0.1) 45%, rgba(0,0,0,0.25) 50%, rgba(255,255,255,0.6) 52%, transparent 100%);
}

html[data-theme='light'] .novel-container .novel-content { color: #2c3e50; }
html[data-theme='light'] .novel-container .novel-content :deep(p), 
html[data-theme='light'] .novel-container .novel-content :deep(li) { color: #34495e; font-weight: 500; }
html[data-theme='light'] .novel-container .novel-content :deep(h1),
html[data-theme='light'] .novel-container .novel-content :deep(h2),
html[data-theme='light'] .novel-container .novel-content :deep(h3) { color: #1a252f; }
html[data-theme='light'] .novel-container .novel-content :deep(h2) {
  background: none;
  -webkit-text-fill-color: #2c3e50;
  border-bottom: 1px dashed rgba(0,0,0,0.15);
}
html[data-theme='light'] .novel-container .novel-content :deep(strong) { color: #000; background: rgba(0,0,0,0.05); }
html[data-theme='light'] .novel-container .printed-page-number { color: rgba(0,0,0,0.5); }
html[data-theme='light'] .novel-container .toc-line { color: #333; }
html[data-theme='light'] .novel-container .toc-dots { border-bottom: 1px dotted rgba(0,0,0,0.25); }
html[data-theme='light'] .novel-container .printed-toc h2 { color: #e74c3c !important; }
</style>

<style>
/* 
  Global (Scoped Olmayan) Scrollbar Stilleri
  v-html ile renderlanan elementlere (pseudo-class) etki etmesi için global olmalıdır.
*/
.mac-pre::-webkit-scrollbar {
  height: 10px;
  width: 10px;
}
.mac-pre::-webkit-scrollbar-track {
  background: #1e1e1e;
  border-radius: 4px;
}
.mac-pre::-webkit-scrollbar-thumb {
  background: #4a4a4a;
  border-radius: 4px;
  border: 2px solid #1e1e1e; /* İç boşluk efekti için */
}
.mac-pre::-webkit-scrollbar-thumb:hover {
  background: #666;
}
.mac-pre::-webkit-scrollbar-corner {
  background: #1e1e1e;
}
</style>
