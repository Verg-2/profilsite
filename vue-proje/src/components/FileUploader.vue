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
        :accept="accept"
        @change="handleFileChange"
      />
      
      <div v-if="previewUrl" class="preview-container">
        <div class="file-preview">
          <i class="fas fa-file-alt file-icon"></i>
          <span class="file-name">{{ getFileName(previewUrl) }}</span>
        </div>
        <div class="preview-overlay">
          <i class="fas fa-upload"></i>
          <span>Değiştir</span>
        </div>
      </div>
      <div v-else class="placeholder">
        <i class="fas fa-cloud-upload-alt upload-icon"></i>
        <p>Dosyayı sürükleyin veya <span class="highlight">seçmek için tıklayın</span></p>
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
    default: 'Dosya Yükle'
  },
  accept: {
    type: String,
    default: '*/*'
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
  return api.defaults.baseURL.replace('/api', '') + url
}

const getFileName = (url) => {
  if (!url) return ''
  const parts = url.split('/')
  return parts[parts.length - 1]
}

const uploadFile = async (file) => {
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
    errorMsg.value = (err.response?.data) || 'Dosya yüklenirken bir hata oluştu.'
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

.file-preview {
  width: 100%;
  height: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  background: var(--admin-bg);
  color: var(--admin-primary);
}

.file-icon {
  font-size: 3rem;
  margin-bottom: 0.5rem;
}

.file-name {
  font-size: 0.9rem;
  max-width: 90%;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
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
