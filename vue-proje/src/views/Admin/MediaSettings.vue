<template>
  <div class="admin-page">
    <div class="admin-page-header">
      <div>
        <h2 class="admin-page-title">Medya Kütüphanesi</h2>
        <p class="admin-page-description">Sunucuda yüklü olan tüm resim, 3D model ve videoları yönetin.</p>
      </div>
      <div class="header-actions">
        <button @click="fetchFiles" class="admin-btn admin-btn-secondary">
          <i class="fas fa-sync-alt"></i> Yenile
        </button>
      </div>
    </div>

    <div v-if="loading" class="admin-loading">
      <i class="fas fa-spinner fa-spin"></i> Yükleniyor...
    </div>
    
    <div v-else-if="files.length === 0" class="admin-empty-state">
      <i class="fas fa-folder-open empty-icon"></i>
      <h3>Medya Bulunamadı</h3>
      <p>Sunucunuzda yüklü herhangi bir dosya bulunmuyor.</p>
    </div>

    <div v-else class="media-grid">
      <div v-for="file in files" :key="file.name" class="media-card admin-glass" @dblclick="selectedFile = file">
        <div class="media-preview">
          <img v-if="file.type === 'image'" :src="getFullUrl(file.url)" :alt="file.name" loading="lazy" />
          <div v-else-if="file.type === '3d'" class="type-icon 3d-icon">
            <i class="fas fa-cube"></i>
          </div>
          <div v-else-if="file.type === 'video'" class="type-icon video-icon">
            <i class="fas fa-video"></i>
          </div>
          <div v-else class="type-icon file-icon">
            <i class="fas fa-file"></i>
          </div>
        </div>
        
        <div class="media-info">
          <div class="media-name" :title="file.name">{{ file.name }}</div>
          <div class="media-meta">
            <span>{{ formatBytes(file.size) }}</span>
            <span>{{ formatDate(file.created) }}</span>
          </div>
        </div>
        
        <div class="media-actions">
          <button @click="copyUrl(getFullUrl(file.url))" class="admin-btn-icon" title="Linki Kopyala">
            <i class="fas fa-copy"></i>
          </button>
          <button @click="deleteFile(file)" class="admin-btn-icon text-danger" title="Kalıcı Olarak Sil">
            <i class="fas fa-trash-alt"></i>
          </button>
        </div>
      </div>
    </div>

    <!-- Preview Modal -->
    <div v-if="selectedFile" class="media-modal-overlay" @click.self="selectedFile = null">
      <div class="media-modal-content admin-glass">
        <button class="close-modal-btn" @click="selectedFile = null"><i class="fas fa-times"></i></button>
        
        <div class="media-modal-body">
          <img v-if="selectedFile.type === 'image'" :src="getFullUrl(selectedFile.url)" :alt="selectedFile.name" />
          <model-viewer 
            v-else-if="selectedFile.type === '3d'"
            :src="getFullUrl(selectedFile.url)" 
            autoplay
            camera-controls
            auto-rotate
            shadow-intensity="1"
            class="modal-3d-viewer"
          ></model-viewer>
          <video v-else-if="selectedFile.type === 'video'" :src="getFullUrl(selectedFile.url)" controls autoplay></video>
          <div v-else class="unsupported-preview">
            <i class="fas fa-file"></i>
            <p>Önizleme desteklenmiyor</p>
            <a :href="getFullUrl(selectedFile.url)" target="_blank" class="admin-btn admin-btn-primary">Dosyayı Aç</a>
          </div>
        </div>
        <div class="media-modal-footer">
          <span class="file-name">{{ selectedFile.name }}</span>
          <span class="file-size">{{ formatBytes(selectedFile.size) }}</span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import api from '@/services/api'
import swal from '@/utils/swal'

const files = ref([])
const loading = ref(true)
const selectedFile = ref(null)

const getFullUrl = (url) => {
  if (!url) return ''
  if (url.startsWith('http') || url.startsWith('data:')) return url
  return api.defaults.baseURL.replace('/api', '') + url
}

const formatBytes = (bytes, decimals = 2) => {
  if (!+bytes) return '0 Bytes'
  const k = 1024
  const dm = decimals < 0 ? 0 : decimals
  const sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB', 'YB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return `${parseFloat((bytes / Math.pow(k, i)).toFixed(dm))} ${sizes[i]}`
}

const formatDate = (dateString) => {
  const options = { year: 'numeric', month: 'short', day: 'numeric', hour: '2-digit', minute: '2-digit' }
  return new Date(dateString).toLocaleDateString('tr-TR', options)
}

const fetchFiles = async () => {
  loading.value = true
  try {
    const res = await api.get('/Upload/list')
    files.value = res.data || []
  } catch (error) {
    console.error('Dosyalar yüklenirken hata oluştu:', error)
    swal.fire({
      title: 'Hata',
      text: 'Dosya listesi alınamadı.',
      icon: 'error'
    })
  } finally {
    loading.value = false
  }
}

const copyUrl = (url) => {
  navigator.clipboard.writeText(url).then(() => {
    swal.fire({
      toast: true,
      position: 'top-end',
      showConfirmButton: false,
      timer: 3000,
      icon: 'success',
      title: 'Link kopyalandı!'
    })
  })
}

const deleteFile = async (file) => {
  const result = await swal.fire({
    title: 'Emin misiniz?',
    text: `${file.name} adlı dosya sunucudan KALICI OLARAK silinecektir!`,
    icon: 'warning',
    showCancelButton: true,
    confirmButtonColor: '#ff3300',
    cancelButtonColor: '#3085d6',
    confirmButtonText: 'Evet, Sil!',
    cancelButtonText: 'İptal',
    background: 'var(--admin-surface)',
    color: 'var(--admin-text-main)'
  })

  if (result.isConfirmed) {
    try {
      await api.delete(`/Upload?fileUrl=${encodeURIComponent(file.url)}`)
      
      files.value = files.value.filter(f => f.name !== file.name)
      
      swal.fire({
        toast: true,
        position: 'top-end',
        showConfirmButton: false,
        timer: 3000,
        icon: 'success',
        title: 'Dosya başarıyla silindi.'
      })
    } catch (error) {
      console.error('Dosya silinirken hata oluştu:', error)
      swal.fire({
        title: 'Hata',
        text: 'Dosya silinemedi. Başka bir işlem tarafından kullanılıyor olabilir.',
        icon: 'error'
      })
    }
  }
}

onMounted(() => {
  fetchFiles()
})
</script>

<style scoped>
.media-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
  gap: 1.5rem;
  margin-top: 1rem;
}

.media-card {
  display: flex;
  flex-direction: column;
  border-radius: var(--admin-radius-md);
  overflow: hidden;
  transition: transform 0.3s ease, box-shadow 0.3s ease;
  background: var(--admin-surface);
  border: 1px solid var(--admin-border);
  cursor: pointer;
}

.media-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 10px 20px rgba(0,0,0,0.1);
  border-color: var(--admin-primary);
}

.media-preview {
  height: 150px;
  background: var(--admin-bg);
  display: flex;
  align-items: center;
  justify-content: center;
  overflow: hidden;
  position: relative;
  border-bottom: 1px solid var(--admin-border);
}

.media-preview img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.type-icon {
  font-size: 3rem;
  color: var(--admin-text-muted);
}

.type-icon.3d-icon { color: #8e44ad; }
.type-icon.video-icon { color: #e74c3c; }

.media-info {
  padding: 1rem;
  flex: 1;
}

.media-name {
  font-size: 0.9rem;
  font-weight: 600;
  color: var(--admin-text-main);
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  margin-bottom: 0.5rem;
}

.media-meta {
  display: flex;
  justify-content: space-between;
  font-size: 0.8rem;
  color: var(--admin-text-muted);
}

.media-actions {
  display: flex;
  justify-content: flex-end;
  gap: 0.5rem;
  padding: 0.5rem 1rem 1rem;
}

.admin-btn-icon {
  background: transparent;
  border: none;
  color: var(--admin-text-muted);
  cursor: pointer;
  font-size: 1.1rem;
  transition: color 0.2s ease;
  padding: 0.3rem;
}

.admin-btn-icon:hover {
  color: var(--admin-primary);
}

.admin-btn-icon.text-danger:hover {
  color: #ff3300;
}

/* Modal Styles */
.media-modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.85);
  backdrop-filter: blur(5px);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  padding: 2rem;
}

.media-modal-content {
  background: var(--admin-surface);
  border-radius: var(--admin-radius-lg);
  border: 1px solid var(--admin-border);
  width: 100%;
  max-width: 900px;
  max-height: 90vh;
  display: flex;
  flex-direction: column;
  position: relative;
  overflow: hidden;
  box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.5);
}

.close-modal-btn {
  position: absolute;
  top: 1rem;
  right: 1rem;
  background: rgba(0, 0, 0, 0.5);
  color: #fff;
  border: none;
  width: 40px;
  height: 40px;
  border-radius: 50%;
  font-size: 1.2rem;
  cursor: pointer;
  z-index: 10;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: background 0.3s ease;
}

.close-modal-btn:hover {
  background: #ff3300;
}

.media-modal-body {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  overflow: hidden;
  min-height: 400px;
  padding: 2rem;
  background: var(--admin-bg);
}

.media-modal-body img, .media-modal-body video {
  max-width: 100%;
  max-height: 70vh;
  object-fit: contain;
  border-radius: var(--admin-radius-md);
}

.modal-3d-viewer {
  width: 100%;
  height: 70vh;
  background: transparent;
  outline: none;
}

.unsupported-preview {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1rem;
  color: var(--admin-text-muted);
}
.unsupported-preview i { font-size: 4rem; }

.media-modal-footer {
  padding: 1.5rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
  border-top: 1px solid var(--admin-border);
  background: var(--admin-surface);
}

.media-modal-footer .file-name {
  font-weight: 600;
  color: var(--admin-text-main);
  word-break: break-all;
}

.media-modal-footer .file-size {
  color: var(--admin-primary);
  font-weight: 700;
  white-space: nowrap;
}
</style>
