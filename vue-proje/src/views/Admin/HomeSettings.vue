<template>
  <div class="admin-page-wrapper">
    <div class="admin-page-header">
      <div>
        <h2 class="admin-title">Anasayfa Yönetimi</h2>
        <p class="admin-subtitle">Sitenizin karşılama ekranındaki metinleri ve görselleri buradan güncelleyebilirsiniz.</p>
      </div>
      <div style="display: flex; gap: 10px; align-items: center;">
        <label class="toggle-switch-inline" style="display:flex; align-items:center; gap:8px; cursor:pointer; background:rgba(255,255,255,0.05); padding:6px 12px; border-radius:8px; border:1px solid var(--admin-border);">
          <span style="color:var(--admin-text-main); font-size:0.9rem; font-weight:500;">Sitede Göster</span>
          <div class="toggle-switch" style="transform: scale(0.9); margin:0;">
            <input type="checkbox" v-model="pageVisibility" @change="saveVisibility">
            <span class="slider round"></span>
          </div>
        </label>
        <button @click="translateWithAI" class="admin-btn admin-btn-ai" :disabled="aiLoading">
          <i class="fas" :class="aiLoading ? 'fa-spinner fa-spin' : 'fa-magic'"></i> 
          {{ aiLoading ? 'Çevriliyor...' : '✨ AI ile Çevir' }}
        </button>
        <button @click="saveData" class="admin-btn admin-btn-primary" :disabled="loading">
          <i class="fas" :class="loading ? 'fa-spinner fa-spin' : 'fa-save'"></i> 
          {{ loading ? 'Kaydediliyor...' : 'Değişiklikleri Kaydet' }}
        </button>
      </div>
    </div>

    <div v-if="errorMsg" style="background: rgba(239, 68, 68, 0.1); border: 1px solid var(--admin-danger); color: var(--admin-danger); padding: 1rem; border-radius: 8px; margin-bottom: 1.5rem;">
      <i class="fas fa-exclamation-circle"></i> {{ errorMsg }}
    </div>

    <div v-if="successMsg" style="background: rgba(16, 185, 129, 0.1); border: 1px solid var(--admin-success); color: var(--admin-success); padding: 1rem; border-radius: 8px; margin-bottom: 1.5rem;">
      <i class="fas fa-check-circle"></i> {{ successMsg }}
    </div>

    <div class="admin-card">
      <h3 style="font-size: 1.1rem; color: var(--admin-primary); margin-bottom: 1.5rem; border-bottom: 1px solid var(--admin-border); padding-bottom: 0.75rem;">
        Kahraman (Hero) Alanı Ayarları
      </h3>
      
      <form @submit.prevent="saveData">
        <div class="admin-grid-2-col">
          <div class="admin-form-group">
            <label class="admin-label">Üst Başlık (Pre-Title) [TR]</label>
            <input type="text" v-model="form.preTitle" placeholder="Örn: MERHABA, BEN" class="admin-input" />
          </div>
          <div class="admin-form-group">
            <label class="admin-label">Üst Başlık [EN]</label>
            <input type="text" v-model="form.preTitleEn" placeholder="Örn: HELLO, I AM" class="admin-input" />
          </div>

          <div class="admin-form-group">
            <label class="admin-label">Ana Başlık (Turuncu İsim) [TR]</label>
            <input type="text" v-model="form.name" placeholder="Örn: Kadir" class="admin-input" />
          </div>
          <div class="admin-form-group">
            <label class="admin-label">Ana Başlık [EN]</label>
            <input type="text" v-model="form.nameEn" placeholder="Örn: Kadir" class="admin-input" />
          </div>

          <div class="admin-form-group">
            <label class="admin-label">Alt Unvan (Beyaz Başlık) [TR]</label>
            <textarea v-model="form.profession" placeholder="Örn: Backend<br>Developer" class="admin-input" rows="2"></textarea>
          </div>
          <div class="admin-form-group">
            <label class="admin-label">Alt Unvan [EN]</label>
            <textarea v-model="form.professionEn" placeholder="Örn: Backend<br>Developer" class="admin-input" rows="2"></textarea>
          </div>
        </div>

        <div class="admin-grid-2-col" style="margin-top: 1.5rem;">
          <div class="admin-form-group">
            <label class="admin-label">Açıklama (Gri Alt Metin) [TR]</label>
            <textarea v-model="form.heroSubtitle" placeholder="Örn: Modern web uygulamalarının arkasında..." class="admin-input" rows="4"></textarea>
          </div>
          <div class="admin-form-group">
            <label class="admin-label">Açıklama [EN]</label>
            <textarea v-model="form.heroSubtitleEn" placeholder="English description..." class="admin-input" rows="4"></textarea>
          </div>
        </div>

        <div class="admin-grid-2-col" style="margin-top: 1.5rem;">
          <ImageUploader v-model="form.profileImageUrl" label="Profil Görseli (Kahraman Alanı)" />
          <div style="display: flex; flex-direction: column; gap: 1rem;">
            <FileUploader v-model="form.model3DUrl" accept=".glb,.gltf" label="3D Karakter / Model (Karanlık Tema)" />
            <FileUploader v-model="form.model3DUrlLight" accept=".glb,.gltf" label="3D Karakter / Model (Açık Tema)" />
          </div>
        </div>

        <div class="admin-grid-2-col">
          <div class="admin-form-group">
            <label class="admin-label">1. Buton Metni [TR]</label>
            <input type="text" v-model="form.buttonText" placeholder="Örn: Projelerim" class="admin-input" />
          </div>
          <div class="admin-form-group">
            <label class="admin-label">1. Buton Metni [EN]</label>
            <input type="text" v-model="form.buttonTextEn" placeholder="Örn: My Projects" class="admin-input" />
          </div>

          <div class="admin-form-group">
            <label class="admin-label">1. Buton Linki</label>
            <input type="text" v-model="form.buttonUrl" placeholder="Örn: /projects" class="admin-input" />
          </div>
          <div></div> <!-- Boşluk -->
          
          <div class="admin-form-group">
            <label class="admin-label">2. Buton Metni [TR]</label>
            <input type="text" v-model="form.secondaryButtonText" placeholder="Örn: İletişim" class="admin-input" />
          </div>
          <div class="admin-form-group">
            <label class="admin-label">2. Buton Metni [EN]</label>
            <input type="text" v-model="form.secondaryButtonTextEn" placeholder="Örn: Contact" class="admin-input" />
          </div>

          <div class="admin-form-group">
            <label class="admin-label">2. Buton Linki</label>
            <input type="text" v-model="form.secondaryButtonUrl" placeholder="Örn: /contact" class="admin-input" />
          </div>
        </div>
      </form>
    </div>

    <!-- İmleç Ayarları -->
    <div class="admin-card" style="margin-top: 1.5rem;">
      <h3 style="font-size: 1.1rem; color: var(--admin-primary); margin-bottom: 1.5rem; border-bottom: 1px solid var(--admin-border); padding-bottom: 0.75rem;">
        Tema İmleç (Cursor) Ayarları
      </h3>
      <p style="color: var(--admin-text-muted); font-size: 0.9rem; margin-bottom: 1.5rem;">
        Sitenin fare imlecini emojilerle veya bir ikonla değiştirebilirsiniz. Sadece bir emoji veya ikon sınıfı yazıp Ekle demeniz yeterli.
      </p>
      
      <div class="admin-grid-2-col">
        <div class="admin-form-group">
          <label class="admin-label">Açık Tema İmleci (Emoji/İkon)</label>
          <IconPicker v-model="form.lightCursor" mode="emoji" />
        </div>

        <div class="admin-form-group">
          <label class="admin-label">Karanlık Tema İmleci (Emoji/İkon)</label>
          <IconPicker v-model="form.darkCursor" mode="emoji" />
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import api from '@/services/api'
import translationService from '@/services/translationService'
import ImageUploader from '@/components/ImageUploader.vue'
import FileUploader from '@/components/FileUploader.vue'
import IconPicker from '@/components/IconPicker.vue'

const loading = ref(false)
const aiLoading = ref(false)
const errorMsg = ref('')
const successMsg = ref('')

const pageVisibility = ref(true)
const seoData = ref(null)

const form = ref({
  id: 0,
  name: '',
  nameEn: '',
  profession: '',
  professionEn: '',
  heroTitle: '',
  heroTitleEn: '',
  heroSubtitle: '',
  heroSubtitleEn: '',
  profileImageUrl: '',
  model3DUrl: '',
  model3DUrlLight: '',
  preTitle: '',
  preTitleEn: '',
  buttonText: '',
  buttonTextEn: '',
  buttonUrl: '',
  secondaryButtonText: '',
  secondaryButtonTextEn: '',
  secondaryButtonUrl: '',
  lightCursor: '',
  darkCursor: ''
})

const loadData = async () => {
  try {
    const res = await api.get('/HomeSettings')
    if (res.data) {
      form.value = { ...res.data, name: '', profession: '', nameEn: '', professionEn: '' }
      
      const parts = (form.value.heroTitle || '').split('|')
      form.value.name = parts[0] || ''
      form.value.profession = parts.length > 1 ? parts.slice(1).join('|') : ''

      const partsEn = (form.value.heroTitleEn || '').split('|')
      form.value.nameEn = partsEn[0] || ''
      form.value.professionEn = partsEn.length > 1 ? partsEn.slice(1).join('|') : ''
    }
  } catch (err) {
    if (err.response && err.response.status === 404) {
      // It's empty, ignore 404
    } else {
      errorMsg.value = 'Veriler yüklenirken bir hata oluştu.'
    }
  }
  
  try {
    const seoRes = await api.get('/SeoSettings/page?route=/')
    if(seoRes.data) {
      seoData.value = seoRes.data
      pageVisibility.value = seoData.value.isVisible !== false && seoData.value.IsVisible !== false
    }
  } catch (err) {
    console.error('SEO görünürlük ayarı alınamadı')
  }
}

const translateWithAI = async () => {
  aiLoading.value = true;
  errorMsg.value = '';
  
  try {
    if (form.value.preTitle && !form.value.preTitleEn) {
      const res = await translationService.translate(form.value.preTitle, 'English', 'Home');
      form.value.preTitleEn = res?.translatedText || form.value.preTitleEn;
    }
    if (form.value.name && !form.value.nameEn) {
      const res = await translationService.translate(form.value.name, 'English', 'Home');
      form.value.nameEn = res?.translatedText || form.value.nameEn;
    }
    if (form.value.profession && !form.value.professionEn) {
      const res = await translationService.translate(form.value.profession, 'English', 'Home');
      form.value.professionEn = res?.translatedText || form.value.professionEn;
    }
    if (form.value.heroSubtitle && !form.value.heroSubtitleEn) {
      const res = await translationService.translate(form.value.heroSubtitle, 'English', 'Home');
      form.value.heroSubtitleEn = res?.translatedText || form.value.heroSubtitleEn;
    }
    if (form.value.buttonText && !form.value.buttonTextEn) {
      const res = await translationService.translate(form.value.buttonText, 'English', 'Home');
      form.value.buttonTextEn = res?.translatedText || form.value.buttonTextEn;
    }
    if (form.value.secondaryButtonText && !form.value.secondaryButtonTextEn) {
      const res = await translationService.translate(form.value.secondaryButtonText, 'English', 'Home');
      form.value.secondaryButtonTextEn = res?.translatedText || form.value.secondaryButtonTextEn;
    }
    successMsg.value = 'Metinler başarıyla İngilizceye çevrildi!';
    setTimeout(() => { successMsg.value = '' }, 3000);
  } catch (err) {
    errorMsg.value = 'Çeviri sırasında hata oluştu: ' + (err.response?.data?.message || err.message);
  } finally {
    aiLoading.value = false;
  }
}

const saveData = async () => {
  loading.value = true
  errorMsg.value = ''
  successMsg.value = ''
  
  form.value.heroTitle = form.value.name;
  if(form.value.profession) {
    form.value.heroTitle += '|' + form.value.profession;
  }

  form.value.heroTitleEn = form.value.nameEn;
  if(form.value.professionEn) {
    form.value.heroTitleEn += '|' + form.value.professionEn;
  }
  
  // ASP.NET JSON binding çakışmasını (camelCase vs PascalCase) önlemek için yeni yüklenen linkleri her iki versiyona da eşitle.
  if (form.value.model3DUrl) form.value.Model3DUrl = form.value.model3DUrl;
  if (form.value.model3DUrlLight) form.value.Model3DUrlLight = form.value.model3DUrlLight;

  try {
    await api.put('/HomeSettings', form.value)
    
    if (seoData.value) {
       seoData.value.isVisible = pageVisibility.value;
       await api.post('/SeoSettings', seoData.value);
    } else {
       // if it doesn't exist, create minimal
       await api.post('/SeoSettings', { route: '/', isVisible: pageVisibility.value });
    }

    // Cache temizle
    window.__homeSettingsCache = null;
    localStorage.removeItem('homeSettings');

    successMsg.value = 'Anasayfa ayarları başarıyla kaydedildi!'
    setTimeout(() => { successMsg.value = '' }, 3000)
  } catch (err) {
    errorMsg.value = 'Ayarlar kaydedilirken hata oluştu: ' + (err.response?.data || err.message)
  } finally {
    loading.value = false
  }
}

const saveVisibility = async () => {
  try {
    if (seoData.value) {
       seoData.value.isVisible = pageVisibility.value;
       await api.post('/SeoSettings', seoData.value);
    } else {
       await api.post('/SeoSettings', { route: '/', isVisible: pageVisibility.value });
    }
  } catch (e) {
    console.error('Görünürlük kaydedilemedi', e)
  }
}

onMounted(() => {
  loadData()
})
</script>
