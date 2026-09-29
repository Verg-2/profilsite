<template>
  <div class="admin-page">
    <div class="admin-page-header">
      <div>
        <h2 class="admin-page-title">Sertifikalarım</h2>
        <p class="admin-page-description">Sertifika ekle, sil veya güncelle.</p>
      </div>
      <div style="display:flex; gap:10px;">
        <button v-if="editMode" @click="resetForm" class="admin-btn admin-btn-secondary">
          İptal Et
        </button>
      </div>
    </div>

    <div class="admin-card admin-glass">
      <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px;">
        <h3 class="admin-subtitle" style="margin: 0;">{{ editMode ? 'Sertifikayı Düzenle' : 'Yeni Sertifika Ekle' }}</h3>
        
        <!-- YENİ EKLENEN AI ÇEVİRİ BUTONU -->
        <button @click="translateWithAI" class="admin-btn admin-btn-ai" :disabled="aiLoading">
          <i class="fas" :class="aiLoading ? 'fa-spinner fa-spin' : 'fa-magic'"></i> 
          {{ aiLoading ? 'Çevriliyor...' : '✨ AI ile Çevir' }}
        </button>
      </div>
      
      <div class="admin-grid-1-2">
        <div>
          <h4 style="margin-bottom:10px; color:#ff4d00;">🇹🇷 Türkçe Bilgiler</h4>
          <div class="admin-form-group">
            <label class="admin-label">Sertifika Adı</label>
            <input v-model="newCertificate.name" type="text" class="admin-input" />
          </div>
          <div class="admin-form-group">
            <label class="admin-label">Alınma Tarihi</label>
            <input v-model="newCertificate.date" type="text" class="admin-input" />
          </div>
          <div class="admin-form-group">
            <label class="admin-label">Açıklama</label>
            <textarea v-model="newCertificate.description" class="admin-input" rows="3"></textarea>
          </div>
        </div>

        <div>
          <h4 style="margin-bottom:10px; color:#4a90e2;">🇬🇧 İngilizce Çeviriler</h4>
          <div class="admin-form-group">
            <label class="admin-label">Sertifika Adı (İngilizce)</label>
            <input v-model="newCertificate.nameEn" type="text" class="admin-input" />
          </div>
          <div class="admin-form-group">
            <label class="admin-label">Alınma Tarihi (İngilizce)</label>
            <input v-model="newCertificate.dateEn" type="text" class="admin-input" />
          </div>
          <div class="admin-form-group">
            <label class="admin-label">Açıklama (İngilizce)</label>
            <textarea v-model="newCertificate.descriptionEn" class="admin-input" rows="3"></textarea>
          </div>
        </div>
      </div>

      <div class="admin-form-group" style="margin-top:15px;">
        <label class="admin-label">Resim Linki</label>
        <input v-model="newCertificate.imageUrl" type="text" class="admin-input" />
      </div>

      <button @click="saveCertificate" class="admin-btn admin-btn-primary" :disabled="loading" style="margin-top: 1rem;">
        <i class="fas" :class="loading ? 'fa-spinner fa-spin' : (editMode ? 'fa-edit' : 'fa-save')"></i> 
        {{ loading ? 'İşleniyor...' : (editMode ? 'Güncelle' : 'Sertifika Ekle') }}
      </button>
    </div>

    <div class="admin-card admin-glass" style="margin-top: 2rem;">
      <h3 class="admin-subtitle">Mevcut Sertifikalar</h3>
      
      <div class="grid-list" style="display: grid; grid-template-columns: repeat(auto-fill, minmax(280px, 1fr)); gap: 20px;">
        <div v-for="cert in certificates" :key="cert.id" class="item-card admin-glass" style="padding: 1rem; border-radius: 12px; display: flex; flex-direction: column;">
          <img :src="cert.imageUrl" alt="Sertifika" style="width: 100%; height: 180px; object-fit: cover; border-radius: 8px; margin-bottom: 10px;" />
          <h4 style="color: var(--admin-text-main);">{{ cert.name }}</h4>
          <p style="color: var(--admin-text-muted); font-size: 0.9rem;">{{ cert.date }}</p>
          
          <div style="display:flex; gap:10px; margin-top: 15px;">
            <button @click="editCertificate(cert)" class="admin-btn admin-btn-primary" style="flex:1; justify-content: center;">
              <i class="fas fa-pen"></i> Düzenle
            </button>
            <button @click="deleteCertificate(cert.id)" class="admin-btn admin-btn-secondary" style="flex:1; justify-content: center; color: #ff6b6b;">
              <i class="fas fa-trash"></i> Sil
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import api from '@/services/api';
import translationService from '@/services/translationService';
import swal from 'sweetalert2';

const certificates = ref([]);
const loading = ref(false);
const aiLoading = ref(false);
const editMode = ref(false);
const editingId = ref(null);

const newCertificate = ref({
    name: '', nameEn: '',
    date: '', dateEn: '',
    description: '', descriptionEn: '',
    imageUrl: '', order: 0
});

const fetchCertificates = async () => {
    try {
        const response = await api.get('/Certificates');
        certificates.value = response.data;
    } catch (error) {
        console.error("Sertifikalar yüklenemedi!");
    }
};

const translateWithAI = async () => {
    if (!newCertificate.value.name && !newCertificate.value.description) {
        swal.fire('Hata', 'Çevrilecek metin bulunamadı. Lütfen Türkçe kısımları doldurun.', 'error');
        return;
    }

    aiLoading.value = true;
    swal.fire({
        title: 'Yapay Zeka Çeviriyor...',
        html: 'Lütfen bekleyin...',
        allowOutsideClick: false,
        didOpen: () => {
            swal.showLoading();
        }
    });

    try {
        if (newCertificate.value.name && !newCertificate.value.nameEn) {
            const res = await translationService.translate(newCertificate.value.name, 'English', 'Certificates');
            newCertificate.value.nameEn = res?.translatedText || '';
        }
        if (newCertificate.value.date && !newCertificate.value.dateEn) {
            const res = await translationService.translate(newCertificate.value.date, 'English', 'Certificates');
            newCertificate.value.dateEn = res?.translatedText || '';
        }
        if (newCertificate.value.description && !newCertificate.value.descriptionEn) {
            const res = await translationService.translate(newCertificate.value.description, 'English', 'Certificates');
            newCertificate.value.descriptionEn = res?.translatedText || '';
        }
        
        swal.close();
        
        // SweetAlert kütüphanesinin mini notification Toast özelliği
        const Toast = swal.mixin({
            toast: true,
            position: 'top-end',
            showConfirmButton: false,
            timer: 3000,
            timerProgressBar: true
        });
        Toast.fire({ icon: 'success', title: 'Çeviri tamamlandı!' });
        
    } catch (error) {
        const errorMsg = error.response?.data?.message || 'Çeviri sırasında bir sorun oluştu.';
        swal.fire('Hata', errorMsg, 'error');
    } finally {
        aiLoading.value = false;
    }
};

const editCertificate = (cert) => {
    editMode.value = true;
    editingId.value = cert.id;
    newCertificate.value = { ...cert };
    window.scrollTo({ top: 0, behavior: 'smooth' });
};

const resetForm = () => {
    editMode.value = false;
    editingId.value = null;
    newCertificate.value = { name: '', nameEn: '', date: '', dateEn: '', description: '', descriptionEn: '', imageUrl: '', order: 0 };
};

const saveCertificate = async () => {
    if (!newCertificate.value.name || !newCertificate.value.imageUrl) {
        alert("Lütfen en azından Türkçe isim ve resim linki girin.");
        return;
    }
    
    loading.value = true;
    try {
        if (editMode.value) {
            await api.put(`/Certificates/${editingId.value}`, newCertificate.value);
        } else {
            await api.post('/Certificates', newCertificate.value);
        }
        resetForm();
        await fetchCertificates();
    } catch (error) {
        alert("İşlem sırasında hata oluştu!");
    } finally {
        loading.value = false;
    }
};

const deleteCertificate = async (id) => {
    if (confirm("Silmek istediğinize emin misiniz?")) {
        try {
            await api.delete(`/Certificates/${id}`);
            await fetchCertificates();
        } catch (error) {
            alert("Silinirken hata oluştu!");
        }
    }
};

onMounted(() => {
    fetchCertificates();
});
</script>