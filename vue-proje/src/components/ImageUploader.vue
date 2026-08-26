<template>
  <div class="image-uploader">
    <label class="admin-label">{{ label }}</label>
    
    <div 
      class="dropzone" 
      :class="{ 'is-dragover': isDragOver }"
      @dragover.prevent="isDragOver = true"
      @dragleave.prevent="isDragOver = false"
      @drop.prevent="handleDrop"
      @click="triggerFileInput"
    >
      <input 
        type="file" 
        ref="fileInput" 
        class="hidden-input" 
        accept="image/*"
        @change="handleFileChange"
      />
      
      <div v-if="previewUrl" class="preview-container">
        <img :src="getFullUrl(previewUrl)" alt="Preview" class="image-preview" />
        <div class="preview-overlay">
          <i class="fas fa-camera" @click.stop="triggerFileInput"></i>
          <span @click.stop="triggerFileInput">Değiştir</span>
          <button type="button" class="btn btn-danger clear-btn" @click.stop="clearFile" style="margin-top: 10px; padding: 5px 15px; font-size: 0.9rem;">
            <i class="fas fa-trash"></i> Temizle
          </button>
        </div>
      </div>
      <div v-else class="placeholder">
        <i class="fas fa-cloud-upload-alt upload-icon"></i>
        <p>Görseli sürükleyin veya <span class="highlight">seçmek için tıklayın</span></p>
      </div>
    </div>
    
    <!-- Upload Progress/Error -->
    <div v-if="uploading" class="upload-status text-primary">
      <i class="fas fa-spinner fa-spin"></i> Yükleniyor...
    </div>
    <div v-if="errorMsg" class="upload-status text-danger">
      <i class="fas fa-exclamation-triangle"></i> {{ errorMsg }}
    </div>
  </div>
</template>

<script setup>
import { ref, watch } from 'vue'
import api from '@/services/api'

const props = defineProps({
  modelValue: {
    type: String,
    default: ''
  },
  label: {
    type: String,
    default: 'Görsel Yükle'
  }
})

const emit = defineEmits(['update:modelValue'])

const fileInput = ref(null)
const isDragOver = ref(false)
const previewUrl = ref(props.modelValue)
const uploading = ref(false)
const errorMsg = ref('')

// Watch for external changes
watch(() => props.modelValue, (newVal) => {
  previewUrl.value = newVal
})

const triggerFileInput = () => {
  fileInput.value.click()
}

const clearFile = () => {
  previewUrl.value = ''
  emit('update:modelValue', '')
}

const handleDrop = (e) => {
  isDragOver.value = false
  const files = e.dataTransfer.files
  if (files.length > 0) {
    uploadFile(files[0])
  }
}

const handleFileChange = (e) => {
  const files = e.target.files
  if (files.length > 0) {
    uploadFile(files[0])
  }
}

const getFullUrl = (url) => {
  if (!url) return ''
  if (url.startsWith('http') || url.startsWith('data:')) return url
  // Use the same base URL as API but without /api
  return api.defaults.baseURL.replace('/api', '') + url
}

const uploadFile = async (file) => {
  if (!file.type.startsWith('image/')) {
    errorMsg.value = 'Lütfen sadece görsel (image) dosyası yükleyin.'
    return
  }

  const formData = new FormData()
  formData.append('file', file)
  
  uploading.value = true
  errorMsg.value = ''
  
  try {
    const res = await api.post('/Upload', formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    })
    
    if (res.data && res.data.url) {
      previewUrl.value = res.data.url
      emit('update:modelValue', res.data.url)
    }
  } catch (err) {
    console.error("Upload error:", err)
    errorMsg.value = 'Görsel yüklenirken bir hata oluştu.'
  } finally {
    uploading.value = false
    // Reset file input
    if (fileInput.value) fileInput.value.value = ''
  }
}
</script>

<style scoped>
.image-uploader {
  margin-bottom: 1.5rem;
}

.dropzone {
  border: 2px dashed var(--admin-border);
  border-radius: var(--admin-radius-md);
  background: var(--admin-btn-secondary-bg);
  min-height: 160px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: var(--admin-transition);
  position: relative;
  overflow: hidden;
}

.dropzone:hover, .dropzone.is-dragover {
  border-color: var(--admin-primary);
  background: rgba(255, 85, 0, 0.05);
}

.hidden-input {
  display: none;
}

.placeholder {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.5rem;
  color: var(--admin-text-muted);
}

.upload-icon {
  font-size: 2.5rem;
  color: var(--admin-primary);
  opacity: 0.8;
  margin-bottom: 0.5rem;
}

.highlight {
  color: var(--admin-primary);
  font-weight: 500;
}

.preview-container {
  position: absolute;
  top: 0; left: 0; right: 0; bottom: 0;
  width: 100%;
  height: 100%;
}

.image-preview {
  width: 100%;
  height: 100%;
  object-fit: contain;
  background: #000;
}

.preview-overlay {
  position: absolute;
  top: 0; left: 0; right: 0; bottom: 0;
  background: rgba(0,0,0,0.6);
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  color: #fff;
  opacity: 0;
  transition: opacity 0.3s ease;
  font-size: 1.2rem;
  gap: 0.5rem;
}

.preview-container:hover .preview-overlay {
  opacity: 1;
}

.upload-status {
  margin-top: 0.5rem;
  font-size: 0.85rem;
}

.text-primary { color: var(--admin-primary); }
.text-danger { color: var(--admin-danger); }
</style>
