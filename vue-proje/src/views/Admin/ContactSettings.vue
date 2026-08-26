<template>
  <div class="admin-page fade-in">
    <div class="admin-header">
      <h1 class="admin-title">İletişim & Sosyal Medya Ayarları</h1>
      <div style="display: flex; gap: 10px; align-items: center;">
        <label class="toggle-switch-inline" style="display:flex; align-items:center; gap:8px; cursor:pointer; background:rgba(255,255,255,0.05); padding:6px 12px; border-radius:8px; border:1px solid var(--admin-border);">
          <span style="color:var(--admin-text-main); font-size:0.9rem; font-weight:500;">Sitede Göster</span>
          <div class="toggle-switch" style="transform: scale(0.9); margin:0;">
            <input type="checkbox" v-model="pageVisibility" @change="saveVisibility">
            <span class="slider round"></span>
          </div>
        </label>
        <button @click="showAddModal = true" class="admin-btn admin-btn-primary">
          <i class="fas fa-plus"></i> Yeni Ekle
        </button>
      </div>
    </div>

    <div class="admin-card">
      <div v-if="loading" style="padding: 20px; text-align: center;">Yükleniyor...</div>
      <div v-else-if="errorMsg" class="form-error" style="margin-bottom: 20px;">{{ errorMsg }}</div>
      
      <table class="admin-table" v-else>
        <thead>
          <tr>
            <th>İkon</th>
            <th>Başlık</th>
            <th>Alt Başlık</th>
            <th>Link (Url)</th>
            <th>Sıra</th>
            <th>İşlemler</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="card in cards" :key="card.id">
            <td style="font-size: 1.5rem; text-align: center;">
              <span v-if="card.icon && card.icon.startsWith('<svg')" v-safe-html="card.icon" style="width: 1em; height: 1em; display: inline-flex; align-items: center; justify-content: center;"></span>
              <i v-else :class="card.icon"></i>
            </td>
            <td>{{ card.title }}</td>
            <td>{{ card.subtitle }}</td>
            <td>{{ card.url || '-' }}</td>
            <td>{{ card.orderIndex }}</td>
            <td>
              <div class="action-buttons">
                <button @click="editCard(card)" class="admin-btn admin-btn-secondary" style="padding: 6px 12px; font-size: 0.85rem;">
                  <i class="fas fa-edit"></i> Düzenle
                </button>
                <button @click="deleteCard(card.id)" class="admin-btn" style="background: var(--admin-danger); color: white; padding: 6px 12px; font-size: 0.85rem; border: none; border-radius: 6px;">
                  <i class="fas fa-trash"></i> Sil
                </button>
              </div>
            </td>
          </tr>
          <tr v-if="cards.length === 0">
            <td colspan="6" style="text-align: center; padding: 20px; color: var(--admin-text-muted);">Henüz kart eklenmemiş.</td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Ekleme / Düzenleme Modalı -->
    <teleport to="body">
      <div v-if="showAddModal" class="modal-overlay" @click="closeModal">
        <div class="modal-content admin-card" @click.stop>
          <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 35px; border-bottom: 1px solid var(--admin-border); padding-bottom: 20px;">
            <h2 style="color: var(--admin-primary); margin: 0; font-size: 1.8rem; font-weight: 700;">
              <i class="ph ph-address-book" style="margin-right: 12px;"></i>
              {{ isEditing ? 'İletişim Kartını Düzenle' : 'Yeni İletişim Kartı Ekle' }}
            </h2>
            <button @click="closeModal" style="background: transparent; border: none; color: var(--admin-text-muted); cursor: pointer; font-size: 2rem; transition: color 0.2s;" onmouseover="this.style.color='var(--admin-danger)'" onmouseout="this.style.color='var(--admin-text-muted)'">
              <i class="ph ph-x"></i>
            </button>
          </div>

          <form @submit.prevent="saveCard">
            <div class="modal-grid">
              <div class="admin-form-group">
                <label class="admin-label" style="font-size: 1.1rem; margin-bottom: 12px;">Başlık [TR] <span class="required">*</span></label>
                <div class="input-wrapper">
                  <i class="ph ph-text-t"></i>
                  <input type="text" v-model="form.title" class="admin-input modal-input-large" placeholder="Örn: Konum, LinkedIn..." required />
                </div>
              </div>

              <div class="admin-form-group">
                <label class="admin-label" style="font-size: 1.1rem; margin-bottom: 12px;">Başlık [EN]</label>
                <div class="input-wrapper">
                  <i class="ph ph-text-t"></i>
                  <input type="text" v-model="form.titleEn" class="admin-input modal-input-large" placeholder="Örn: Location, LinkedIn..." />
                </div>
              </div>

              <div class="admin-form-group">
                <label class="admin-label" style="font-size: 1.1rem; margin-bottom: 12px;">Alt Başlık [TR]</label>
                <div class="input-wrapper">
                  <i class="ph ph-subtitles"></i>
                  <input type="text" v-model="form.subtitle" class="admin-input modal-input-large" placeholder="Örn: Adana / @kullanici" />
                </div>
              </div>

              <div class="admin-form-group">
                <label class="admin-label" style="font-size: 1.1rem; margin-bottom: 12px;">Alt Başlık [EN]</label>
                <div class="input-wrapper">
                  <i class="ph ph-subtitles"></i>
                  <input type="text" v-model="form.subtitleEn" class="admin-input modal-input-large" placeholder="Örn: Adana / @user" />
                </div>
              </div>

              <div class="admin-form-group full-width">
                <label class="admin-label" style="font-size: 1.1rem; margin-bottom: 12px;">URL (Link)</label>
                <div class="input-wrapper">
                  <i class="ph ph-link"></i>
                  <input type="text" v-model="form.url" class="admin-input modal-input-large" placeholder="Örn: https://linkedin.com/in/... (Konum için boş bırakın)" />
                </div>
              </div>

              <div class="admin-form-group">
                <label class="admin-label" style="font-size: 1.1rem; margin-bottom: 12px;">İkon (Sınıf) <span class="required">*</span></label>
                <IconPicker v-model="form.icon" mode="icon" />
              </div>

              <div class="admin-form-group">
                <label class="admin-label" style="font-size: 1.1rem; margin-bottom: 12px;">Sıra Numarası</label>
                <div class="input-wrapper">
                  <i class="ph ph-sort-ascending"></i>
                  <input type="number" v-model="form.orderIndex" class="admin-input modal-input-large" />
                </div>
              </div>
            </div>

            <div class="modal-actions">
                <button @click="translateWithAI" class="admin-btn admin-btn-ai" :disabled="aiLoading">
                  <i class="fas" :class="aiLoading ? 'fa-spinner fa-spin' : 'fa-magic'"></i> 
                  {{ aiLoading ? 'Çevriliyor...' : '✨ AI ile Çevir' }}
                </button>
              <button type="button" @click="closeModal" class="admin-btn admin-btn-secondary btn-cancel">İptal</button>
              <button type="submit" class="admin-btn admin-btn-primary btn-save" :disabled="saving">
                <i class="ph ph-check-circle" v-if="!saving"></i>
                <i class="fas fa-spinner fa-spin" v-else></i>
                {{ saving ? 'Kaydediliyor...' : 'Değişiklikleri Kaydet' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </teleport>

    <!-- Modern Toast Notification -->
    <teleport to="body">
      <div v-if="toastMessage" class="modern-toast" :class="{ 'toast-error': toastIsError }">
        <i :class="toastIsError ? 'ph ph-warning-circle' : 'ph ph-check-circle'"></i>
        <span>{{ toastMessage }}</span>
      </div>
    </teleport>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import api from '@/services/api'
import translationService from '@/services/translationService'
import IconPicker from '@/components/IconPicker.vue'

const cards = ref([])
const loading = ref(false)
const aiLoading = ref(false)
const errorMsg = ref('')
const saving = ref(false)

const pageVisibility = ref(true)
const seoData = ref(null)

const showAddModal = ref(false)
const isEditing = ref(false)
const currentId = ref(null)

const toastMessage = ref('')
const toastIsError = ref(false)

const showToast = (message, isError = false) => {
  toastMessage.value = message;
  toastIsError.value = isError;
  setTimeout(() => {
    toastMessage.value = '';
  }, 3000);
}

const form = ref({
  title: '',
  titleEn: '',
  subtitle: '',
  subtitleEn: '',
  url: '',
  icon: 'ph ph-link',
  orderIndex: 0
})

const loadData = async () => {
  loading.value = true
  errorMsg.value = ''
  try {
    const res = await api.get('/ContactCards')
    cards.value = res.data || []
    
    // SEO
    try {
      const seoRes = await api.get('/SeoSettings/page?route=/contact')
      if (seoRes.data) {
        seoData.value = seoRes.data
        pageVisibility.value = seoData.value.isVisible !== false && seoData.value.IsVisible !== false
      }
    } catch (e) {}

  } catch (err) {
    errorMsg.value = 'Veriler yüklenirken hata oluştu.'
  } finally {
    loading.value = false
  }
}

const closeModal = () => {
  showAddModal.value = false
  isEditing.value = false
  currentId.value = null
  form.value = {
    title: '',
    titleEn: '',
    subtitle: '',
    subtitleEn: '',
    url: '',
    icon: 'ph ph-link',
    orderIndex: cards.value.length
  }
}

const editCard = (card) => {
  isEditing.value = true
  currentId.value = card.id
  form.value = { ...card }
  showAddModal.value = true
}

const translateWithAI = async () => {
  aiLoading.value = true;
  try {
    if (form.value.title && !form.value.titleEn) {
      const res = await translationService.translate(form.value.title, 'English', 'Contact');
      form.value.titleEn = res?.translatedText || form.value.titleEn;
    }
    if (form.value.subtitle && !form.value.subtitleEn) {
      const res = await translationService.translate(form.value.subtitle, 'English', 'Contact');
      form.value.subtitleEn = res?.translatedText || form.value.subtitleEn;
    }
    showToast('Metinler başarıyla İngilizceye çevrildi!');
  } catch (err) {
    showToast('Çeviri sırasında hata oluştu.', true);
  } finally {
    aiLoading.value = false;
  }
}

const deleteCard = async (id) => {
  if (!confirm('Bu kartı silmek istediğinize emin misiniz?')) return
  
  try {
    await api.delete(`/ContactCards/${id}`)
    await loadData()
    showToast('Kayıt başarıyla silindi!')
  } catch (err) {
    showToast('Silme işlemi başarısız oldu.', true)
  }
}

const saveCard = async () => {
  saving.value = true
  try {
    if (isEditing.value) {
      await api.put(`/ContactCards/${currentId.value}`, {
        id: currentId.value,
        ...form.value
      })
    } else {
      await api.post('/ContactCards', form.value)
    }
    closeModal()
    await loadData()
    showToast('Kayıt başarıyla tamamlandı!')
  } catch (err) {
    showToast('Kaydetme işlemi başarısız oldu.', true)
  } finally {
    saving.value = false
  }
}

const saveVisibility = async () => {
  try {
    if (seoData.value) {
       seoData.value.isVisible = pageVisibility.value;
       await api.post('/SeoSettings', seoData.value);
    } else {
       await api.post('/SeoSettings', { route: '/contact', isVisible: pageVisibility.value });
    }
  } catch (e) {
    console.error('Görünürlük kaydedilemedi', e)
  }
}

onMounted(() => {
  loadData()
})
</script>

<style scoped>
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.75);
  display: flex;
  justify-content: center;
  align-items: flex-start; /* Changed from center to fix cutoff */
  z-index: 1000;
  backdrop-filter: blur(8px);
  overflow-y: auto;
  padding: 5vh 20px; /* Space on top and bottom */
}

.modal-content {
  width: 100%;
  max-width: 900px; /* Super wide */
  background: var(--admin-surface);
  padding: 50px; /* More padding */
  border-radius: 20px;
  border: 1px solid rgba(255, 255, 255, 0.08);
  box-shadow: 0 30px 60px -15px rgba(0, 0, 0, 0.6), inset 0 1px 0 rgba(255, 255, 255, 0.05);
  margin: auto;
}

.modal-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 30px 40px; /* Bigger gaps between columns */
}

.full-width {
  grid-column: 1 / -1;
}

.required {
  color: var(--admin-danger);
  margin-left: 4px;
}

.input-wrapper {
  position: relative;
  display: flex;
  align-items: center;
}

.input-wrapper i {
  position: absolute;
  left: 18px;
  top: 50%;
  transform: translateY(-50%);
  color: var(--admin-text-muted);
  font-size: 1.5rem;
}

.input-wrapper .admin-input {
  padding-left: 54px;
  width: 100%;
  background: var(--admin-bg);
  border: 1px solid var(--admin-border);
  transition: all 0.3s ease;
}

.modal-input-large {
  padding: 16px 20px 16px 54px !important;
  font-size: 1.1rem !important;
  border-radius: 12px !important;
}

.input-wrapper .admin-input:focus {
  border-color: var(--admin-primary);
  box-shadow: 0 0 0 3px rgba(255, 94, 0, 0.15);
}

.modal-actions {
  display: flex;
  justify-content: flex-end;
  gap: 16px;
  margin-top: 36px;
  border-top: 1px solid rgba(255, 255, 255, 0.05);
  padding-top: 24px;
}

.btn-cancel {
  padding: 12px 28px;
  border-radius: 10px;
  font-weight: 500;
  background: transparent;
  border: 1px solid var(--admin-border);
  color: var(--admin-text-muted);
  transition: all 0.2s;
}

.btn-cancel:hover {
  background: rgba(255, 255, 255, 0.05);
  color: var(--admin-text-main);
}

.btn-save {
  padding: 12px 36px;
  border-radius: 10px;
  font-weight: 600;
  display: flex;
  align-items: center;
  gap: 8px;
  box-shadow: 0 8px 16px rgba(255, 94, 0, 0.25);
  transition: all 0.2s;
}

.btn-save:hover {
  transform: translateY(-2px);
  box-shadow: 0 12px 24px rgba(255, 94, 0, 0.35);
}

.action-buttons {
  display: flex;
  gap: 10px;
}

.modern-toast {
  position: fixed;
  bottom: 30px;
  right: 30px;
  background: var(--admin-surface);
  color: var(--admin-text-main);
  padding: 16px 24px;
  border-radius: 12px;
  box-shadow: 0 10px 25px rgba(0,0,0,0.5);
  display: flex;
  align-items: center;
  gap: 12px;
  font-size: 1.1rem;
  font-weight: 500;
  z-index: 10000;
  border-left: 4px solid var(--admin-primary);
  animation: slideInRight 0.3s ease-out;
}

.modern-toast i {
  font-size: 1.5rem;
  color: var(--admin-primary);
}

.modern-toast.toast-error {
  border-left-color: var(--admin-danger);
}
.modern-toast.toast-error i {
  color: var(--admin-danger);
}

@keyframes slideInRight {
  from { transform: translateX(100%); opacity: 0; }
  to { transform: translateX(0); opacity: 1; }
}
</style>
