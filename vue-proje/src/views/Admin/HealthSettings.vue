<template>
  <div class="admin-page-wrapper">
    <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 2rem; flex-wrap: wrap; gap: 1rem;">
      <div>
        <h2 class="admin-title">Sistem Sağlığı &amp; Siber Güvenlik Radar</h2>
        <p class="admin-subtitle">Güvenlik, SEO, Performans ve Sistem Sağlığını derinlemesine analiz edin.</p>
      </div>
      <div style="display: flex; gap: 1rem; flex-wrap: wrap;">
        <button @click="cleanupOrphanedFiles" class="admin-btn" style="background: rgba(239,68,68,0.1); color: var(--admin-danger); border: 1px solid rgba(239,68,68,0.2);" :disabled="isCleaning">
          <i class="fas" :class="isCleaning ? 'fa-spinner fa-spin' : 'fa-broom'"></i>
          {{ isCleaning ? 'Temizleniyor...' : 'Kullanılmayan Medyaları Sil' }}
        </button>
        <button @click="startScan" class="admin-btn admin-btn-primary" :disabled="isScanning">
          <i class="fas" :class="isScanning ? 'fa-spinner fa-spin' : 'fa-satellite-dish'"></i>
          {{ isScanning ? 'Tarama Devam Ediyor...' : 'Kapsamlı Taramayı Başlat' }}
        </button>
      </div>
    </div>
    </div>

    <!-- TABS -->
    <div class="admin-tabs" style="display: flex; gap: 1rem; margin-bottom: 2rem; border-bottom: 1px solid var(--admin-border); padding-bottom: 0.5rem;">
      <button @click="activeTab = 'scan'" :class="['admin-tab-btn', activeTab === 'scan' ? 'active' : '']" style="background: none; border: none; color: var(--admin-text-main); font-size: 1.05rem; font-weight: 600; padding: 0.5rem 1rem; cursor: pointer; border-bottom: 3px solid transparent;">
        <i class="fas fa-radar"></i> Genel Sistem Taraması
      </button>
      <button @click="activeTab = 'ai-code'" :class="['admin-tab-btn', activeTab === 'ai-code' ? 'active' : '']" style="background: none; border: none; color: var(--admin-text-main); font-size: 1.05rem; font-weight: 600; padding: 0.5rem 1rem; cursor: pointer; border-bottom: 3px solid transparent;">
        <i class="fas fa-robot"></i> 🤖 AI Kod Güvenlik Analizörü
      </button>
    </div>

    <!-- Scanner Card -->
    <div v-show="activeTab === 'scan'">
      <div v-if="isScanning || report" class="admin-card" style="margin-bottom: 2rem; overflow: hidden; position: relative; border-color: var(--admin-primary);">
        <div v-if="isScanning" class="radar-sweep"></div>

      <div style="display: flex; flex-wrap: wrap; gap: 2rem; align-items: stretch; position: relative; z-index: 2;">

        <!-- Live Log -->
        <div style="flex: 1; min-width: 280px; background: var(--admin-input-bg); border: 1px solid var(--admin-border); border-radius: var(--admin-radius-md); padding: 1.5rem; display: flex; flex-direction: column; height: 300px;">
          <h3 style="color: var(--admin-primary); margin-bottom: 1rem; font-size: 1.05rem; display: flex; align-items: center; gap: 0.5rem;">
            <i class="fas fa-terminal"></i> Tehdit Monitörü &amp; Canlı Akış
          </h3>
          <div class="admin-scroll" style="flex: 1; overflow-y: auto; display: flex; flex-direction: column; gap: 0.4rem; font-family: monospace; font-size: 0.8rem;" ref="logContainer">
            <div v-for="(log, idx) in liveLogs" :key="idx" :style="{ color: log.color }">
              &gt; {{ log.time }} — {{ log.msg }}
            </div>
            <div v-if="isScanning" style="color: var(--admin-primary); opacity: 0.7;" class="blink">_</div>
          </div>
        </div>

        <!-- Score Circles (4 adet) -->
        <div v-if="report && !isScanning" style="flex: 1; min-width: 280px; display: flex; flex-wrap: wrap; gap: 1rem; justify-content: space-around; align-items: center; background: var(--admin-input-bg); border: 1px solid var(--admin-border); border-radius: var(--admin-radius-md); padding: 1.5rem;">

          <div class="score-circle">
            <svg width="100" height="100" viewBox="0 0 100 100">
              <circle cx="50" cy="50" r="42" fill="none" stroke="var(--admin-border)" stroke-width="6"/>
              <circle cx="50" cy="50" r="42" fill="none" :stroke="getScoreColor(report.securityScore)" stroke-width="6"
                      :stroke-dasharray="2 * Math.PI * 42"
                      :stroke-dashoffset="(2 * Math.PI * 42) * (1 - report.securityScore / 100)"
                      stroke-linecap="round"
                      style="transform:rotate(-90deg);transform-origin:50% 50%;transition:stroke-dashoffset 1.5s ease;"/>
              <text x="50" y="55" text-anchor="middle" font-size="18" font-weight="bold" :fill="getScoreColor(report.securityScore)">{{ report.securityScore }}</text>
            </svg>
            <span class="score-label"><i class="fas fa-shield-alt"></i> GÜVENLİK</span>
          </div>

          <div class="score-circle">
            <svg width="100" height="100" viewBox="0 0 100 100">
              <circle cx="50" cy="50" r="42" fill="none" stroke="var(--admin-border)" stroke-width="6"/>
              <circle cx="50" cy="50" r="42" fill="none" :stroke="getScoreColor(report.seoScore)" stroke-width="6"
                      :stroke-dasharray="2 * Math.PI * 42"
                      :stroke-dashoffset="(2 * Math.PI * 42) * (1 - report.seoScore / 100)"
                      stroke-linecap="round"
                      style="transform:rotate(-90deg);transform-origin:50% 50%;transition:stroke-dashoffset 1.5s ease;"/>
              <text x="50" y="55" text-anchor="middle" font-size="18" font-weight="bold" :fill="getScoreColor(report.seoScore)">{{ report.seoScore }}</text>
            </svg>
            <span class="score-label"><i class="fas fa-search"></i> SEO</span>
          </div>

          <div class="score-circle">
            <svg width="100" height="100" viewBox="0 0 100 100">
              <circle cx="50" cy="50" r="42" fill="none" stroke="var(--admin-border)" stroke-width="6"/>
              <circle cx="50" cy="50" r="42" fill="none" :stroke="getScoreColor(report.performanceScore)" stroke-width="6"
                      :stroke-dasharray="2 * Math.PI * 42"
                      :stroke-dashoffset="(2 * Math.PI * 42) * (1 - report.performanceScore / 100)"
                      stroke-linecap="round"
                      style="transform:rotate(-90deg);transform-origin:50% 50%;transition:stroke-dashoffset 1.5s ease;"/>
              <text x="50" y="55" text-anchor="middle" font-size="18" font-weight="bold" :fill="getScoreColor(report.performanceScore)">{{ report.performanceScore }}</text>
            </svg>
            <span class="score-label"><i class="fas fa-bolt"></i> PERFORMANS</span>
          </div>

          <div class="score-circle">
            <svg width="100" height="100" viewBox="0 0 100 100">
              <circle cx="50" cy="50" r="42" fill="none" stroke="var(--admin-border)" stroke-width="6"/>
              <circle cx="50" cy="50" r="42" fill="none" :stroke="getScoreColor(report.healthScore)" stroke-width="6"
                      :stroke-dasharray="2 * Math.PI * 42"
                      :stroke-dashoffset="(2 * Math.PI * 42) * (1 - report.healthScore / 100)"
                      stroke-linecap="round"
                      style="transform:rotate(-90deg);transform-origin:50% 50%;transition:stroke-dashoffset 1.5s ease;"/>
              <text x="50" y="55" text-anchor="middle" font-size="18" font-weight="bold" :fill="getScoreColor(report.healthScore)">{{ report.healthScore }}</text>
            </svg>
            <span class="score-label"><i class="fas fa-heartbeat"></i> SAĞLIK</span>
          </div>

        </div>

        <!-- Scanning animation -->
        <div v-else-if="isScanning" style="flex: 1; display: flex; flex-direction: column; justify-content: center; align-items: center; text-align: center; min-width: 200px;">
          <div class="radar-circle">
            <i class="fas fa-satellite-dish" style="font-size: 3rem; color: var(--admin-primary);"></i>
          </div>
          <h3 style="color: var(--admin-primary); margin-top: 1.5rem; font-weight: 600;">SİSTEM ANALİZ EDİLİYOR</h3>
          <p style="color: var(--admin-text-muted); font-family: monospace; font-size: 0.95rem; margin-top: 0.5rem;" class="scan-status">{{ currentStatus }}</p>
        </div>

      </div>
    </div>

    <!-- Results -->
    <div v-if="report && !isScanning" class="admin-card">
      <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.5rem; flex-wrap: wrap; gap: 0.75rem;">
        <h3 style="font-size: 1.2rem; color: var(--admin-heading); margin: 0; display: flex; align-items: center; gap: 0.75rem;">
          <i class="fas fa-list-alt"></i> Detaylı Zafiyet Raporu
          <span style="font-size: 0.8rem; color: var(--admin-text-muted); font-weight:400;">({{ filteredIssues.length }} bulgu)</span>
        </h3>
        <div style="display: flex; gap: 0.4rem; flex-wrap: wrap;">
          <button @click="filter='All'"         :class="['admin-btn', filter==='All'         ? 'admin-btn-primary':'admin-btn-secondary']" style="padding:.35rem .8rem;font-size:.82rem;">Tümü</button>
          <button @click="filter='Security'"    :class="['admin-btn', filter==='Security'    ? 'admin-btn-primary':'admin-btn-secondary']" style="padding:.35rem .8rem;font-size:.82rem;">🛡️ Güvenlik</button>
          <button @click="filter='SEO'"         :class="['admin-btn', filter==='SEO'         ? 'admin-btn-primary':'admin-btn-secondary']" style="padding:.35rem .8rem;font-size:.82rem;">🔍 SEO</button>
          <button @click="filter='Performance'" :class="['admin-btn', filter==='Performance' ? 'admin-btn-primary':'admin-btn-secondary']" style="padding:.35rem .8rem;font-size:.82rem;">⚡ Performans</button>
          <button @click="filter='Health'"      :class="['admin-btn', filter==='Health'      ? 'admin-btn-primary':'admin-btn-secondary']" style="padding:.35rem .8rem;font-size:.82rem;">💓 Sağlık</button>
        </div>
      </div>

      <div style="display: flex; flex-direction: column; gap: 0.65rem;">
        <div v-for="(issue, idx) in filteredIssues" :key="idx"
             style="display: flex; gap: 0.85rem; align-items: flex-start; padding: 0.9rem 1.1rem; border-radius: var(--admin-radius-md); border: 1px solid var(--admin-border); background: var(--admin-surface);">

          <div :style="{ color: getSeverityColor(issue.severity), fontSize: '1.3rem', marginTop: '2px', flexShrink: 0 }">
            <i class="fas" :class="getSeverityIcon(issue.severity)"></i>
          </div>

          <div style="flex: 1; min-width: 0;">
            <div style="display: flex; justify-content: space-between; align-items: flex-start; gap: 0.5rem; margin-bottom: 0.3rem; flex-wrap: wrap;">
              <h4 style="font-size: 0.95rem; color: var(--admin-heading); margin: 0; font-weight: 600; line-height: 1.3;">{{ issue.title }}</h4>
              <span :style="{ background: getSeverityBg(issue.severity), color: getSeverityColor(issue.severity), padding: '2px 9px', borderRadius: '20px', fontSize: '0.7rem', fontWeight: 'bold', whiteSpace: 'nowrap', flexShrink: 0 }">
                {{ getCategoryIcon(issue.category) }} {{ issue.category }} · {{ issue.severity }}
              </span>
            </div>
            <p style="color: var(--admin-text-muted); font-size: 0.86rem; line-height: 1.55; margin: 0;">{{ issue.description }}</p>
          </div>
        </div>
      </div>
    </div>
    </div> <!-- End of scan tab -->

    <!-- AI Code Analyzer Tab -->
    <div v-show="activeTab === 'ai-code'" class="admin-card" style="min-height: 500px;">
      <h3 style="color: var(--admin-primary); margin-bottom: 1rem; font-size: 1.2rem; display: flex; align-items: center; gap: 0.5rem;">
        <i class="fas fa-user-secret"></i> Siber Güvenlik Kod İncelemesi
      </h3>
      <p style="color: var(--admin-text-muted); margin-bottom: 1.5rem;">
        İnceletmek istediğiniz kodu (C#, JS, SQL, vb.) aşağıya yapıştırın. Yapay zeka ajanımız kodu çalıştırılmadan statik olarak analiz edip size güvenlik açıklarını raporlayacaktır. İçerisinde gerçek veritabanı şifreleri veya API anahtarları olmamasına özen gösterin.
      </p>

      <!-- Yeni Dosya Okuma Sistemi -->
      <div style="display: flex; gap: 1rem; align-items: flex-end; margin-bottom: 1.5rem; flex-wrap: wrap;">
        <div style="flex: 1; min-width: 250px;">
          <label class="admin-label"><i class="fas fa-folder-open"></i> Sunucudaki Bir Dosyayı Seç (Otomatik Oku)</label>
          <div style="display: flex; gap: 0.5rem;">
            <select v-model="selectedFile" class="admin-input" style="flex: 1;">
              <option value="">-- Projedeki Bir Dosyayı Seçin --</option>
              <option v-for="file in sourceFiles" :key="file.path" :value="file.path">
                {{ file.name }}
              </option>
            </select>
            <button @click="analyzeSelectedFile" class="admin-btn admin-btn-primary" :disabled="isAnalyzingFile || !selectedFile">
              <i class="fas" :class="isAnalyzingFile ? 'fa-spinner fa-spin' : 'fa-robot'"></i> Oku & Tara
            </button>
          </div>
        </div>
        
        <div style="display: flex; align-items: center; gap: 1rem; padding: 0.5rem 1rem; border: 1px dashed var(--admin-border); border-radius: var(--admin-radius-md);">
          <span style="color: var(--admin-text-muted); font-size: 0.9rem;">veya</span>
          <label class="admin-btn admin-btn-secondary" style="cursor: pointer; margin: 0;">
            <i class="fas fa-upload"></i> Bilgisayardan Yükle
            <input type="file" @change="handleFileUpload" accept=".cs,.js,.vue,.json,.txt,.sql" style="display: none;">
          </label>
        </div>
      </div>

      <div class="form-group">
        <label class="admin-label">Kodu Manuel Yapıştır</label>
        <textarea v-model="codeToAnalyze" class="admin-input code-editor" rows="12" placeholder="// Kodu buraya yapıştırın veya yukarıdan dosya yükleyin..." style="font-family: monospace; font-size: 0.95rem; background: #0f111a; color: #a6accd;"></textarea>
      </div>

      <div style="display: flex; justify-content: flex-end; margin-bottom: 2rem; gap: 1rem;">
        <button @click="analyzeFullProject" class="admin-btn admin-btn-danger" :disabled="isAnalyzingProject" style="background: rgba(239, 68, 68, 0.1); color: var(--admin-danger); border: 1px solid rgba(239, 68, 68, 0.3);">
          <i class="fas" :class="isAnalyzingProject ? 'fa-spinner fa-spin' : 'fa-globe'"></i>
          <span v-if="!isAnalyzingProject">Tüm Projeyi Kapsamlı Tara</span>
          <span v-else>Taranıyor...</span>
        </button>
        <button @click="analyzeCode" class="admin-btn admin-btn-primary" :disabled="isAnalyzingCode || !codeToAnalyze.trim()">
          <i class="fas" :class="isAnalyzingCode ? 'fa-spinner fa-spin' : 'fa-robot'"></i> 
          <span v-if="!isAnalyzingCode">Kodu Analiz Et</span>
          <span v-else>Yapay Zeka Analiz Ediyor...</span>
        </button>
      </div>

      <div v-if="analysisReport" class="admin-card" style="background: rgba(16, 185, 129, 0.05); border: 1px solid var(--admin-primary);">
        <h4 style="color: var(--admin-primary); margin-bottom: 1rem; border-bottom: 1px solid var(--admin-border); padding-bottom: 0.5rem;">
          <i class="fas fa-file-contract"></i> Güvenlik Analiz Raporu
        </h4>
        <div class="markdown-body" v-safe-html="formattedAnalysisReport" style="color: var(--admin-text-main); line-height: 1.6; font-size: 0.95rem;"></div>
      </div>
    </div>


</template>

<script setup>
import { ref, computed, nextTick, onMounted } from 'vue'
import api from '@/services/api'
import Swal from 'sweetalert2'
import { marked } from 'marked' // Markdown parser


const isScanning   = ref(false)
const isAnalyzingFile = ref(false)
const isAnalyzingCode = ref(false)
const isAnalyzingProject = ref(false)
const isCleaning   = ref(false)
const report       = ref(null)
const liveLogs     = ref([])
const currentStatus = ref('Bekleniyor...')
const logContainer  = ref(null)
const filter        = ref('All')

// AI Code Analyzer states
const activeTab = ref('scan')
const codeToAnalyze = ref('')
const analysisReport = ref('')
const sourceFiles = ref([])
const selectedFile = ref('')
const fileInput = ref(null)

const fetchSourceFiles = async () => {
  try {
    const res = await api.get('/SystemHealth/source-files')
    sourceFiles.value = res.data
  } catch (err) {
    console.error('Kaynak dosyalar yüklenemedi:', err)
  }
}

onMounted(() => {
  fetchSourceFiles()
})

const formattedAnalysisReport = computed(() => {
  if (!analysisReport.value) return ''
  return marked.parse(analysisReport.value)
})

const filteredIssues = computed(() => {
  if (!report.value?.issues) return []
  if (filter.value === 'All') return report.value.issues
  return report.value.issues.filter(i => i.category === filter.value)
})

const statuses = [
  // Security
  '🛡️  PostgreSQL heartbeat ve ORM-based SQLi kontrolü yapılıyor...',
  '🛡️  Tüm Controller\'lar Reflection ile [Authorize] taramasından geçiyor...',
  '🛡️  HTTP Güvenlik Başlıkları gerçek yanıttan okunuyor (X-Frame, CSP, HSTS, Referrer)...',
  '🛡️  JWT anahtar uzunluğu, algoritma ve varsayılan değer analizi...',
  '🛡️  XSS payload iletişim formuna gönderiliyor, sanitasyon test ediliyor...',
  '🛡️  CORS Origin Spoofing testi – sahte evil-attacker.xyz ile istek atılıyor...',
  '🛡️  Hassas dizin ifşası: /swagger, /.env, /.git, /appsettings taranıyor...',
  '🛡️  Rate Limit, 2FA, Honeypot, Şifre Hash ve Anomali Tespiti doğrulanıyor...',
  // SEO
  '🔍  SPA Prerender durumu ve TTFB değerleri tüm sayfalarda ölçülüyor...',
  '🔍  JSON-LD Schema blokları parse ediliyor, sentaks doğrulanıyor...',
  '🔍  Görsel alt etiketleri, og:image, og:description, Twitter Card kontrol ediliyor...',
  '🔍  Veritabanı SEO kayıtları (başlık, açıklama uzunlukları, GEO) analiz ediliyor...',
  '🔍  Canonical, Hreflang, robots.txt ve sitemap.xml kontrol ediliyor...',
  // Performance
  '⚡  LCP (Largest Contentful Paint) – fetchpriority ve preload sinyalleri aranıyor...',
  '⚡  CLS riski – boyutsuz görseller ve layout shift kaynakları taranıyor...',
  '⚡  Render-blocking script ve GZIP/Brotli sıkıştırma kontrol ediliyor...',
  '⚡  Sunucu bellek kullanımı (GC) ve process uptime ölçülüyor...',
  // Health
  '💓  Çoklu API endpoint TTFB testi çalıştırılıyor...',
  '💓  Upload klasörü disk kullanımı hesaplanıyor...',
  '💓  Son 1 saatteki başarısız istek oranı AuditLog\'dan analiz ediliyor...',
  '✅  Tüm bulgular derleniyor, 4 kategori puanı hesaplanıyor...'
]

const appendLog = (msg, color = 'var(--admin-text-main)') => {
  const time = new Date().toLocaleTimeString('tr-TR', { hour12: false })
  liveLogs.value.push({ time, msg, color })
  nextTick(() => { if (logContainer.value) logContainer.value.scrollTop = logContainer.value.scrollHeight })
}

const startScan = async () => {
  isScanning.value = true
  report.value     = null
  liveLogs.value   = []

  appendLog('🚀 İleri Seviye Güvenlik Radarı başlatıldı.', 'var(--admin-primary)')

  let si = 0
  const interval = setInterval(() => {
    if (si < statuses.length) {
      currentStatus.value = statuses[si]
      const color = statuses[si].startsWith('🛡️') ? '#ef4444'
                  : statuses[si].startsWith('🔍') ? 'var(--admin-secondary)'
                  : statuses[si].startsWith('⚡') ? '#f59e0b'
                  : statuses[si].startsWith('💓') ? 'var(--admin-success)'
                  : 'var(--admin-success)'
      appendLog(statuses[si].replace(/^[🛡️🔍⚡💓✅]+\s+/, ''), color)
      si++
    }
  }, 650)

  try {
    const res = await api.get('/SystemHealth/scan')
    clearInterval(interval)
    currentStatus.value = '✅ Tarama tamamlandı!'
    appendLog('Rapor hazır. 4 kategori skoru hesaplandı.', 'var(--admin-success)')
    report.value = res.data
  } catch (err) {
    clearInterval(interval)
    currentStatus.value = '❌ Tarama başarısız!'
    appendLog('Hata: ' + err.message, 'var(--admin-danger)')
  } finally {
    isScanning.value = false
  }
}

const analyzeCode = async () => {
  if (!codeToAnalyze.value.trim()) return;
  isAnalyzingCode.value = true;
  analysisReport.value = '';
  
  try {
    const res = await api.post('/SystemHealth/analyze-code', { code: codeToAnalyze.value });
    if (res.data?.success) {
      analysisReport.value = res.data.report;
    } else {
      Swal.fire('Hata', 'Analiz işlemi başarısız oldu.', 'error');
    }
  } catch (err) {
    Swal.fire('Hata', err.response?.data?.message || 'Yapay Zeka ile iletişim kurulamadı.', 'error');
  } finally {
    isAnalyzingCode.value = false;
  }
}

const analyzeSelectedFile = async () => {
  if (!selectedFile.value) {
    Swal.fire('Uyarı', 'Lütfen önce bir dosya seçin.', 'warning')
    return;
  }
  isAnalyzingFile.value = true;
  analysisReport.value = '';
  
  try {
    const res = await api.post('/SystemHealth/analyze-file', { filePath: selectedFile.value });
    if (res.data?.success) {
      analysisReport.value = res.data.report;
    } else {
      Swal.fire('Hata', 'Analiz işlemi başarısız oldu.', 'error');
    }
  } catch (err) {
    Swal.fire('Hata', err.response?.data?.message || 'Dosya analizi yapılamadı.', 'error');
  } finally {
    isAnalyzingFile.value = false;
  }
}

const analyzeFullProject = async () => {
  const result = await Swal.fire({
    title: 'Tüm Proje Analiz Edilecek',
    text: 'Sistem tüm backend (.cs) ve frontend (.vue, .js) dosyalarını okuyup analiz edecek. Bu işlem biraz sürebilir, devam etmek istiyor musunuz?',
    icon: 'warning',
    showCancelButton: true,
    confirmButtonColor: 'var(--admin-danger)',
    cancelButtonColor: 'var(--admin-secondary)',
    confirmButtonText: 'Evet, Kapsamlı Tara',
    cancelButtonText: 'İptal',
    background: '#1a1a2e',
    color: '#fff'
  })

  if (!result.isConfirmed) return;

  isAnalyzingProject.value = true;
  analysisReport.value = '';
  
  try {
    const res = await api.post('/SystemHealth/analyze-project');
    if (res.data?.success) {
      analysisReport.value = res.data.report;
    } else {
      Swal.fire('Hata', 'Analiz işlemi başarısız oldu.', 'error');
    }
  } catch (err) {
    Swal.fire('Hata', err.response?.data?.message || 'Yapay Zeka ile iletişim kurulamadı.', 'error');
  } finally {
    isAnalyzingProject.value = false;
  }
}

const handleFileUpload = (e) => {
  const file = e.target.files[0];
  if (!file) return;
  
  const reader = new FileReader();
  reader.onload = (e) => {
    codeToAnalyze.value = e.target.result;
    // Clear selection from dropdown
    selectedFile.value = '';
  };
  reader.readAsText(file);
}

const cleanupOrphanedFiles = async () => {
  isCleaning.value = true
  try {
    const checkRes = await api.post('/Upload/cleanup?execute=false')
    if (checkRes.data?.success) {
      if (checkRes.data.count === 0) {
        await Swal.fire({
          title: 'Zaten Temiz!',
          text: 'Sistemde kullanılmayan çöp dosya bulunamadı.',
          icon: 'info',
          background: '#1a1a2e',
          color: '#fff'
        })
        return
      }
      const fileListHtml = checkRes.data.files.map(f => `<li>${f}</li>`).join('')
      const confirm = await Swal.fire({
        title: `${checkRes.data.count} Adet Çöp Dosya Bulundu`,
        html: `<p style="margin-bottom:10px;">Bu kullanılmayan medya dosyaları sunucudan kalıcı silinecek.</p>
               <ul style="text-align:left;max-height:200px;overflow-y:auto;font-size:.85rem;background:rgba(0,0,0,.05);padding:10px 10px 10px 25px;border-radius:5px;">${fileListHtml}</ul>`,
        icon: 'warning', showCancelButton: true,
        confirmButtonText: '<i class="fas fa-trash"></i> Evet, Hepsini Sil',
        cancelButtonText: 'İptal', confirmButtonColor: 'var(--admin-danger)',
        background: '#1a1a2e',
        color: '#fff'
      })
      if (confirm.isConfirmed) {
        isCleaning.value = true
        const execRes = await api.post('/Upload/cleanup?execute=true')
        if (execRes.data?.success) {
          await Swal.fire({
            title: 'Başarılı!',
            text: execRes.data.message,
            icon: 'success',
            background: '#1a1a2e',
            color: '#fff'
          })
        }
      }
    }
  } catch { 
    Swal.fire({
      title: 'Hata', 
      text: 'Temizlik sırasında sorun oluştu.', 
      icon: 'error',
      background: '#1a1a2e',
      color: '#fff'
    }) 
  }
  finally { isCleaning.value = false }
}

const getCategoryIcon  = (c) => ({ Security: '🛡️', SEO: '🔍', Performance: '⚡', Health: '💓' }[c] ?? '📋')
const getSeverityColor = (s) => s === 'Critical' ? 'var(--admin-danger)' : s === 'Warning' ? 'var(--admin-secondary)' : 'var(--admin-success)'
const getSeverityBg    = (s) => s === 'Critical' ? 'rgba(239,68,68,.12)' : s === 'Warning' ? 'rgba(255,132,94,.12)' : 'rgba(16,185,129,.12)'
const getSeverityIcon  = (s) => s === 'Critical' ? 'fa-times-circle' : s === 'Warning' ? 'fa-exclamation-triangle' : 'fa-check-circle'
const getScoreColor    = (n) => n >= 90 ? 'var(--admin-success)' : n >= 70 ? 'var(--admin-secondary)' : 'var(--admin-danger)'
</script>

<style scoped>
.score-label {
  display: block;
  text-align: center;
  font-weight: 600;
  font-size: 0.75rem;
  color: var(--admin-heading);
  margin-top: 0.6rem;
  letter-spacing: 0.05em;
}
.admin-tab-btn.active {
  color: var(--admin-primary) !important;
  border-bottom-color: var(--admin-primary) !important;
}
.markdown-body h1, .markdown-body h2, .markdown-body h3 {
  color: var(--admin-heading);
  margin-top: 1.5rem;
  margin-bottom: 0.75rem;
}
.markdown-body p {
  margin-bottom: 1rem;
}
.markdown-body pre {
  background: #0f111a;
  padding: 1rem;
  border-radius: 8px;
  overflow-x: auto;
  margin-bottom: 1rem;
}
.markdown-body code {
  font-family: monospace;
  background: rgba(255,255,255,0.1);
  padding: 0.2rem 0.4rem;
  border-radius: 4px;
}
.markdown-body ul, .markdown-body ol {
  padding-left: 1.5rem;
  margin-bottom: 1rem;
}
.markdown-body li {
  margin-bottom: 0.25rem;
}

.blink { animation: blink-anim 1s step-end infinite; }
@keyframes blink-anim { 0%,100%{opacity:1} 50%{opacity:0} }

.radar-circle {
  width: 100px; height: 100px; border-radius: 50%;
  border: 2px solid var(--admin-primary);
  display: flex; align-items: center; justify-content: center;
  position: relative;
}
.radar-circle::before, .radar-circle::after {
  content: ''; position: absolute; border-radius: 50%;
  border: 1px solid var(--admin-primary);
  animation: pulse-ring 2s cubic-bezier(.215,.61,.355,1) infinite;
}
.radar-circle::after { animation-delay: 1s; }
@keyframes pulse-ring { 0%{width:100px;height:100px;opacity:1} 100%{width:300px;height:300px;opacity:0} }

.scan-status { animation: fade-status .6s ease-in-out; }
@keyframes fade-status { 0%{opacity:0;transform:translateY(5px)} 100%{opacity:1;transform:translateY(0)} }

.radar-sweep {
  position: absolute; top: 0; left: 0; width: 100%; height: 2px;
  background: var(--admin-primary);
  box-shadow: 0 0 20px 5px var(--admin-primary-glow);
  animation: sweep 3s linear infinite; z-index: 1;
}
@keyframes sweep { 0%{top:0;opacity:0} 10%{opacity:1} 90%{opacity:1} 100%{top:100%;opacity:0} }
</style>
