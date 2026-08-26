<template>
  <div class="admin-page-wrapper">
    <div class="admin-page-header">
      <div>
        <h2 class="admin-title">Dinamik SEO & GEO Yönetimi</h2>
        <p class="admin-subtitle">Tüm sayfalarınızın arama motoru ve bölgesel etiketlerini yönetin.</p>
      </div>
      <div style="display: flex; gap: 10px;">
        <button @click="translateWithAI" class="admin-btn admin-btn-ai" :disabled="aiLoading">
          <i class="fas" :class="aiLoading ? 'fa-spinner fa-spin' : 'fa-magic'"></i> 
          {{ aiLoading ? 'Çevriliyor...' : '✨ AI ile Çevir' }}
        </button>
        <button @click="saveSettings" class="admin-btn admin-btn-primary" :disabled="isSaving">
          <i class="fas fa-save"></i> {{ isSaving ? 'Kaydediliyor...' : 'Değişiklikleri Kaydet' }}
        </button>
      </div>
    </div>

    <!-- Route Selector -->
    <div class="admin-card" style="margin-bottom: 2rem;">
      <h3 style="color: var(--admin-heading); margin-bottom: 1rem; font-size: 1.1rem;">
        <i class="fas fa-route" style="color: var(--admin-primary); margin-right: 0.5rem;"></i>
        Düzenlenecek Sayfayı (Rotayı) Seçin
      </h3>
      <div class="form-group" style="margin: 0;">
        <select v-model="selectedRoute" @change="fetchSettings" class="admin-input" style="font-size: 1.1rem; padding: 1rem;">
          <optgroup v-for="group in groupedRoutes" :key="group.label" :label="group.label">
            <option v-for="route in group.routes" :key="route.value" :value="route.value">
              {{ route.label }}
            </option>
          </optgroup>
        </select>
      </div>
    </div>

    <!-- Forms -->
    <div v-if="!isLoading" class="admin-grid-2-col">
      
      <!-- SEO Settings -->
      <div class="admin-card">
        <h3 style="color: var(--admin-heading); margin-bottom: 1.5rem; font-size: 1.1rem; padding-bottom: 0.5rem; border-bottom: 1px solid var(--admin-border);">
          <i class="fab fa-google" style="color: var(--admin-primary); margin-right: 0.5rem;"></i>
          Google / SEO Etiketleri
        </h3>
        
        <div class="form-group">
          <label class="form-label">SEO Başlığı (Title) [TR]</label>
          <input v-model="form.seoTitle" type="text" class="admin-input" placeholder="Sayfa başlığı (Maks 60 karakter)">
          <small style="color: var(--admin-text-muted); display: block; margin-top: 0.25rem;">Arama sonuçlarında görünen mavi başlık.</small>
        </div>
        <div class="form-group">
          <label class="form-label">SEO Başlığı (Title) [EN]</label>
          <input v-model="form.seoTitleEn" type="text" class="admin-input" placeholder="Page title (Max 60 chars)">
        </div>

        <div class="form-group">
          <label class="form-label">SEO Metni (Meta Description) [TR]</label>
          <textarea v-model="form.seoDescription" class="admin-input" rows="4" placeholder="Sayfa açıklaması (Maks 160 karakter)"></textarea>
          <small style="color: var(--admin-text-muted); display: block; margin-top: 0.25rem;">Arama sonuçlarındaki açıklama metni.</small>
        </div>
        <div class="form-group">
          <label class="form-label">SEO Metni (Meta Description) [EN]</label>
          <textarea v-model="form.seoDescriptionEn" class="admin-input" rows="4" placeholder="Page description (Max 160 chars)"></textarea>
        </div>
      </div>

      <!-- GEO Settings -->
      <div class="admin-card">
        <h3 style="color: var(--admin-heading); margin-bottom: 1.5rem; font-size: 1.1rem; padding-bottom: 0.5rem; border-bottom: 1px solid var(--admin-border);">
          <i class="fas fa-globe" style="color: var(--admin-primary); margin-right: 0.5rem;"></i>
          Bölgesel (GEO) & Sosyal Etiketler
        </h3>
        
        <div class="form-group">
          <label class="form-label">GEO Başlığı (og:title / Local) [TR]</label>
          <input v-model="form.geoTitle" type="text" class="admin-input" placeholder="Sosyal medya / Bölgesel başlık">
        </div>
        <div class="form-group">
          <label class="form-label">GEO Başlığı [EN]</label>
          <input v-model="form.geoTitleEn" type="text" class="admin-input" placeholder="Social media title">
        </div>

        <div class="form-group">
          <label class="form-label">GEO Metni (og:description / Local) [TR]</label>
          <textarea v-model="form.geoDescription" class="admin-input" rows="4" placeholder="Sosyal medya / Bölgesel açıklama"></textarea>
        </div>
        <div class="form-group">
          <label class="form-label">GEO Metni [EN]</label>
          <textarea v-model="form.geoDescriptionEn" class="admin-input" rows="4" placeholder="Social media description"></textarea>
        </div>

        <div class="form-group">
          <label class="form-label">Dil Etiketi (Lang)</label>
          <input v-model="form.lang" type="text" class="admin-input" placeholder="Örn: tr, en, de" style="max-width: 150px;">
        </div>
      </div>
    </div>
    
    <!-- SERP Live Preview -->
    <div v-if="!isLoading" class="admin-card" style="margin-top: 2rem;">
      <h3 style="color: var(--admin-heading); margin-bottom: 1.5rem; font-size: 1.1rem; padding-bottom: 0.5rem; border-bottom: 1px solid var(--admin-border);">
        <i class="fab fa-google" style="color: var(--admin-primary); margin-right: 0.5rem;"></i>
        Google Canlı Önizleme (SERP Live Preview)
      </h3>
      
      <div style="background: #ffffff; padding: 20px; border-radius: 8px; font-family: Arial, sans-serif; max-width: 650px; box-shadow: 0 4px 6px rgba(0,0,0,0.1);">
        <div style="display: flex; align-items: center; margin-bottom: 5px;">
          <div style="width: 28px; height: 28px; background: #f1f3f4; border-radius: 50%; display: flex; align-items: center; justify-content: center; margin-right: 12px;">
            <i class="fas fa-globe" style="color: #5f6368; font-size: 14px;"></i>
          </div>
          <div>
            <div style="font-size: 14px; color: #202124;">kadir.com</div>
            <div style="font-size: 12px; color: #4d5156;">https://kadir.com{{ form.route === '/' ? '' : form.route }}</div>
          </div>
        </div>
        <div :style="{ color: form.seoTitle.length > 60 ? '#d93025' : '#1a0dab', fontSize: '20px', lineHeight: '26px', cursor: 'pointer', marginBottom: '3px', textDecoration: 'none' }">
          {{ form.seoTitle || 'Google\'da Görünecek Mükemmel Başlığınızı Girin' }}
        </div>
        <div :style="{ color: form.seoDescription.length > 160 ? '#d93025' : '#4d5156', fontSize: '14px', lineHeight: '22px' }">
          {{ form.seoDescription || 'Google arama sonuçlarında kullanıcıların göreceği açıklama metni. Burayı boş bırakırsanız Google otomatik olarak sayfa içinden bir metin seçecektir.' }}
        </div>
      </div>
      
      <div style="margin-top: 1rem; display: flex; gap: 1.5rem; font-weight: 500;">
        <span :style="{ color: form.seoTitle.length > 60 ? 'var(--admin-danger)' : 'var(--admin-success)' }">
          <i :class="form.seoTitle.length > 60 ? 'fas fa-exclamation-triangle' : 'fas fa-check-circle'"></i> Başlık Karakter: {{ form.seoTitle.length }}/60
        </span>
        <span :style="{ color: form.seoDescription.length > 160 ? 'var(--admin-danger)' : 'var(--admin-success)' }">
          <i :class="form.seoDescription.length > 160 ? 'fas fa-exclamation-triangle' : 'fas fa-check-circle'"></i> Açıklama Karakter: {{ form.seoDescription.length }}/160
        </span>
      </div>
    </div>
    
    <div v-else class="admin-card" style="text-align: center; padding: 3rem;">
      <i class="fas fa-spinner fa-spin fa-2x" style="color: var(--admin-primary);"></i>
      <p style="margin-top: 1rem; color: var(--admin-text-muted);">SEO verileri yükleniyor...</p>
    </div>

    <!-- Kaydedilen SEO Ayarları Listesi -->
    <div class="admin-card" style="margin-top: 2rem;">
      <h3 style="color: var(--admin-heading); margin-bottom: 1.5rem; font-size: 1.2rem; display: flex; align-items: center; justify-content: space-between;">
        <span><i class="ph ph-list-dashes" style="color: var(--admin-primary); margin-right: 0.5rem;"></i> Kaydedilen SEO / GEO Yapılandırmaları</span>
        <span style="font-size: 0.9rem; font-weight: normal; color: var(--admin-text-muted);">Toplam: {{ savedSettings.length }}</span>
      </h3>

      <div class="table-responsive">
        <table class="admin-table">
          <thead>
            <tr>
              <th>Rota (Sayfa)</th>
              <th>SEO Başlığı</th>
              <th>SEO Açıklaması</th>
              <th>Dil</th>
              <th style="text-align: center;">Durum</th>
              <th style="width: 150px; text-align: center;">İşlemler</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="item in savedSettings" :key="item.id || item.Id">
              <td><strong>{{ item.route || item.Route }}</strong></td>
              <td>{{ item.seoTitle || item.SeoTitle || '-' }}</td>
              <td><span style="display: inline-block; max-width: 250px; white-space: nowrap; overflow: hidden; text-overflow: ellipsis;">{{ item.seoDescription || item.SeoDescription || '-' }}</span></td>
              <td><span class="lang-badge">{{ item.lang || item.Lang }}</span></td>
              <td style="text-align: center;">
                <i v-if="item.isVisible !== false && item.IsVisible !== false" class="fas fa-eye" style="color: var(--admin-success, #2ecc71);" title="Görünür"></i>
                <i v-else class="fas fa-eye-slash" style="color: var(--admin-danger, #e74c3c);" title="Gizli"></i>
              </td>
              <td style="text-align: center;">
                <div class="action-buttons" style="justify-content: center;">
                  <button @click="editSavedSetting(item.route || item.Route)" class="admin-btn admin-btn-secondary" style="padding: 6px 12px; font-size: 0.85rem;" title="Düzenle">
                    <i class="fas fa-edit"></i>
                  </button>
                  <button @click="deleteSavedSetting(item.route || item.Route)" class="admin-btn" style="background: var(--admin-danger); color: white; padding: 6px 12px; font-size: 0.85rem; border: none; border-radius: 6px;" title="Sil">
                    <i class="fas fa-trash"></i>
                  </button>
                </div>
              </td>
            </tr>
            <tr v-if="savedSettings.length === 0">
              <td colspan="5" style="text-align: center; padding: 2rem; color: var(--admin-text-muted);">
                Henüz kaydedilmiş özel bir SEO ayarı bulunmuyor.
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

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
import { ref, computed, onMounted } from 'vue'
import api from '@/services/api'
import translationService from '@/services/translationService'

const selectedRoute = ref('/')
const isLoading = ref(false)
const isSaving = ref(false)
const aiLoading = ref(false)

const projects = ref([])
const blogs = ref([])
const savedSettings = ref([])

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
  route: '/',
  seoTitle: '',
  seoTitleEn: '',
  seoDescription: '',
  seoDescriptionEn: '',
  geoTitle: '',
  geoTitleEn: '',
  geoDescription: '',
  geoDescriptionEn: '',
  lang: 'tr',
  isVisible: true
})

const groupedRoutes = computed(() => {
  const staticRoutes = [
    { value: '/', label: 'Ana Sayfa (/)' },
    { value: '/hakkinda', label: 'Hakkında (/hakkinda)' },
    { value: '/projects', label: 'Projeler (/projects)' },
    { value: '/yetenekler', label: 'Yetenekler (/yetenekler)' },
    { value: '/blog', label: 'Blog Ana Sayfa (/blog)' },
    { value: '/contact', label: 'İletişim (/contact)' }
  ];

  const projectRoutes = projects.value.map(p => ({
    value: `/proje/${p.id}`,
    label: `${p.title} (/proje/${p.id})`
  }));

  const blogRoutes = blogs.value.map(b => ({
    value: `/blog/${b.id}`,
    label: `${b.title} (/blog/${b.id})`
  }));

  // Sadece henüz kaydedilmemiş (veya şu an düzenlemekte olduğumuz) rotaları göster
  const isSaved = (routeVal) => savedSettings.value.some(s => (s.route || s.Route) === routeVal);

  const filterRoutes = (routes) => routes.filter(r => !isSaved(r.value) || r.value === selectedRoute.value);

  return [
    { label: 'Sabit Sayfalar', routes: filterRoutes(staticRoutes) },
    { label: 'Projeler (Dinamik)', routes: filterRoutes(projectRoutes) },
    { label: 'Blog Yazıları (Dinamik)', routes: filterRoutes(blogRoutes) }
  ].filter(g => g.routes.length > 0);
})

const fetchSettings = async () => {
  if (!selectedRoute.value) return
  isLoading.value = true
  try {
    const path = encodeURIComponent(selectedRoute.value)
    const res = await api.get(`/SeoSettings/page?route=${path}`)
    
    // Hem C# PascalCase hem JS camelCase destekle
    const data = res.data && (res.data.id || res.data.Id) ? res.data : null;
    
    if (data) {
      form.value = {
        route: selectedRoute.value,
        seoTitle: data.seoTitle || data.SeoTitle || '',
        seoTitleEn: data.seoTitleEn || data.SeoTitleEn || '',
        seoDescription: data.seoDescription || data.SeoDescription || '',
        seoDescriptionEn: data.seoDescriptionEn || data.SeoDescriptionEn || '',
        geoTitle: data.geoTitle || data.GeoTitle || '',
        geoTitleEn: data.geoTitleEn || data.GeoTitleEn || '',
        geoDescription: data.geoDescription || data.GeoDescription || '',
        geoDescriptionEn: data.geoDescriptionEn || data.GeoDescriptionEn || '',
        lang: data.lang || data.Lang || 'tr',
        isVisible: data.isVisible !== undefined ? data.isVisible : (data.IsVisible !== undefined ? data.IsVisible : true)
      }
    } else {
      const existing = savedSettings.value.find(s => (s.route || s.Route) === selectedRoute.value)
      if (existing) {
         form.value = {
          route: selectedRoute.value,
          seoTitle: existing.seoTitle || existing.SeoTitle || '',
          seoTitleEn: existing.seoTitleEn || existing.SeoTitleEn || '',
          seoDescription: existing.seoDescription || existing.SeoDescription || '',
          seoDescriptionEn: existing.seoDescriptionEn || existing.SeoDescriptionEn || '',
          geoTitle: existing.geoTitle || existing.GeoTitle || '',
          geoTitleEn: existing.geoTitleEn || existing.GeoTitleEn || '',
          geoDescription: existing.geoDescription || existing.GeoDescription || '',
          geoDescriptionEn: existing.geoDescriptionEn || existing.GeoDescriptionEn || '',
          lang: existing.lang || existing.Lang || 'tr',
          isVisible: existing.isVisible !== undefined ? existing.isVisible : (existing.IsVisible !== undefined ? existing.IsVisible : true)
         }
      } else {
         form.value = {
          route: selectedRoute.value,
          seoTitle: '',
          seoTitleEn: '',
          seoDescription: '',
          seoDescriptionEn: '',
          geoTitle: '',
          geoTitleEn: '',
          geoDescription: '',
          geoDescriptionEn: '',
          lang: 'tr',
          isVisible: true
         }
      }
    }
  } catch (error) {
    console.error("SEO Ayarları çekilemedi:", error)
  } finally {
    isLoading.value = false
  }
}

const translateWithAI = async () => {
  aiLoading.value = true;
  try {
    if (form.value.seoTitle && !form.value.seoTitleEn) {
      const res = await translationService.translate(form.value.seoTitle, 'English', 'Seo');
      form.value.seoTitleEn = res?.translatedText || form.value.seoTitleEn;
    }
    if (form.value.seoDescription && !form.value.seoDescriptionEn) {
      const res = await translationService.translate(form.value.seoDescription, 'English', 'Seo');
      form.value.seoDescriptionEn = res?.translatedText || form.value.seoDescriptionEn;
    }
    if (form.value.geoTitle && !form.value.geoTitleEn) {
      const res = await translationService.translate(form.value.geoTitle, 'English', 'Seo');
      form.value.geoTitleEn = res?.translatedText || form.value.geoTitleEn;
    }
    if (form.value.geoDescription && !form.value.geoDescriptionEn) {
      const res = await translationService.translate(form.value.geoDescription, 'English', 'Seo');
      form.value.geoDescriptionEn = res?.translatedText || form.value.geoDescriptionEn;
    }
    showToast('Metinler başarıyla İngilizceye çevrildi!');
  } catch (err) {
    showToast('Çeviri sırasında hata oluştu.', true);
  } finally {
    aiLoading.value = false;
  }
}

const saveSettings = async () => {
  isSaving.value = true
  try {
    form.value.route = selectedRoute.value
    await api.post('/SeoSettings', form.value)
    showToast('SEO Ayarları başarıyla kaydedildi!')
    await loadSavedSettings() // Listeyi yenile
  } catch (error) {
    showToast('Kaydetme başarısız oldu.', true)
  } finally {
    isSaving.value = false
  }
}

const loadSavedSettings = async () => {
  try {
    const res = await api.get('/SeoSettings/GetAll');
    savedSettings.value = res.data || [];
  } catch (e) {
    console.error("Kaydedilen SEO ayarları yüklenemedi", e);
  }
}

const editSavedSetting = (route) => {
  selectedRoute.value = route;
  fetchSettings();
  window.scrollTo({ top: 0, behavior: 'smooth' }); // Formu yukarıya kaydır
}

const deleteSavedSetting = async (route) => {
  if (!confirm(`${route} rotası için SEO ayarını silmek istediğinize emin misiniz?`)) return;
  try {
    const path = encodeURIComponent(route);
    await api.delete(`/SeoSettings/page?route=${path}`);
    showToast('Ayar başarıyla silindi!');
    await loadSavedSettings();
    if (selectedRoute.value === route) {
      fetchSettings(); // Formu da temizle
    }
  } catch (e) {
    showToast('Silme işlemi başarısız oldu.', true);
  }
}

const loadDynamicEntities = async () => {
  try {
    const [projRes, blogRes] = await Promise.all([
      api.get('/Projects'),
      api.get('/BlogPosts')
    ]);
    projects.value = projRes.data || [];
    blogs.value = blogRes.data || [];
  } catch (e) {
    console.error("Projeler ve bloglar yüklenemedi", e);
  }
}

onMounted(async () => {
  await Promise.all([
    loadDynamicEntities(),
    loadSavedSettings()
  ]);
  fetchSettings();
})
</script>

<style scoped>
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

.table-responsive {
  overflow-x: auto;
}

.admin-table {
  width: 100%;
  border-collapse: collapse;
  margin-top: 1rem;
}

.admin-table th, .admin-table td {
  padding: 1.2rem;
  text-align: left;
  border-bottom: 1px solid var(--admin-border);
}

.admin-table th {
  font-weight: 600;
  color: var(--admin-text-muted);
  font-size: 0.95rem;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.admin-table tr:hover {
  background: rgba(255, 255, 255, 0.02);
}

.action-buttons {
  display: flex;
  gap: 10px;
}

.lang-badge {
  background: rgba(255, 94, 0, 0.1);
  color: var(--admin-primary);
  padding: 4px 10px;
  border-radius: 20px;
  font-weight: 600;
  font-size: 0.85rem;
  text-transform: uppercase;
}
</style>
