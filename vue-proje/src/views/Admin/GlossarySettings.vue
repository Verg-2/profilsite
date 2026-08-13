<template>
  <div class="admin-page-wrapper">
    <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 2rem;">
      <h2 class="admin-title">📖 Dinamik Sözlük (Çeviri Kalkanı)</h2>
      <button @click="openModal()" class="admin-btn admin-btn-primary">
        <i class="fas fa-plus"></i> Yeni Kelime Ekle
      </button>
    </div>

    <p style="color: var(--admin-text-muted); margin-bottom: 2rem;">
      Yapay zekanın ve çeviri motorunun yanlış çevirmesini istemediğiniz teknik terimleri buradan belirleyebilirsiniz. 
      Örneğin "Framework" kelimesinin her zaman "Framework" veya "Yazılım İskeleti" olarak çevrilmesini zorlayabilirsiniz.
    </p>

    <!-- Loading State -->
    <div v-if="loading" style="text-align: center; padding: 3rem; color: var(--admin-text-muted);">
      <i class="fas fa-spinner fa-spin fa-2x"></i>
      <p style="margin-top: 1rem;">Sözlük yükleniyor...</p>
    </div>

    <!-- Data Table -->
    <div v-else class="admin-card" style="padding: 0; overflow: hidden;">
      <div v-if="items.length === 0" style="padding: 3rem; text-align: center; color: var(--admin-text-muted);">
        <i class="fas fa-book fa-3x" style="margin-bottom: 1rem; opacity: 0.5;"></i>
        <p>Sözlükte henüz hiç kelime bulunmuyor.</p>
      </div>

      <table v-else class="admin-table" style="width: 100%; border-collapse: collapse;">
        <thead>
          <tr style="background: rgba(0,0,0,0.2); border-bottom: 1px solid var(--admin-border);">
            <th style="padding: 1rem; text-align: left; color: var(--admin-text-muted);">Orijinal Kelime</th>
            <th style="padding: 1rem; text-align: left; color: var(--admin-text-muted);">Zorunlu Çeviri Karşılığı</th>
            <th style="padding: 1rem; text-align: center; color: var(--admin-text-muted);">Durum</th>
            <th style="padding: 1rem; text-align: right; color: var(--admin-text-muted);">İşlemler</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="item in items" :key="item.id" style="border-bottom: 1px solid var(--admin-border);">
            <td style="padding: 1rem; font-weight: bold; color: var(--admin-heading);">{{ item.originalTerm }}</td>
            <td style="padding: 1rem; color: var(--admin-primary);">{{ item.targetTerm }}</td>
            <td style="padding: 1rem; text-align: center;">
              <span v-if="item.isActive" style="background: rgba(16, 185, 129, 0.1); color: #10b981; padding: 4px 8px; border-radius: 4px; font-size: 0.8rem;">Aktif</span>
              <span v-else style="background: rgba(239, 68, 68, 0.1); color: #ef4444; padding: 4px 8px; border-radius: 4px; font-size: 0.8rem;">Pasif</span>
            </td>
            <td style="padding: 1rem; text-align: right;">
              <button @click="openModal(item)" class="admin-btn" style="background: transparent; color: var(--admin-text-main); padding: 0.5rem; margin-right: 0.5rem;">
                <i class="fas fa-edit"></i>
              </button>
              <button @click="deleteItem(item.id)" class="admin-btn" style="background: transparent; color: var(--admin-danger); padding: 0.5rem;">
                <i class="fas fa-trash"></i>
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Modal -->
    <div v-if="showModal" class="modal-overlay" style="position: fixed; top: 0; left: 0; width: 100%; height: 100%; background: rgba(0,0,0,0.7); display: flex; align-items: center; justify-content: center; z-index: 1000;">
      <div class="admin-card" style="width: 100%; max-width: 500px; padding: 2rem;">
        <h3 style="margin-bottom: 1.5rem; color: var(--admin-heading);">
          {{ editingItem.id ? 'Kelime Düzenle' : 'Yeni Kelime Ekle' }}
        </h3>

        <div class="form-group" style="margin-bottom: 1rem;">
          <label class="admin-label">Orijinal Kelime (Metinde geçen)</label>
          <input type="text" v-model="editingItem.originalTerm" class="admin-input" placeholder="Örn: Framework">
        </div>

        <div class="form-group" style="margin-bottom: 1.5rem;">
          <label class="admin-label">Çevrilmesi İstenen Karşılık</label>
          <input type="text" v-model="editingItem.targetTerm" class="admin-input" placeholder="Örn: Framework">
        </div>

        <div class="form-group" style="margin-bottom: 2rem;">
          <label class="admin-label" style="display: flex; align-items: center; gap: 0.5rem; cursor: pointer;">
            <input type="checkbox" v-model="editingItem.isActive">
            Bu kelime kuralı aktif olsun
          </label>
        </div>

        <div style="display: flex; justify-content: flex-end; gap: 1rem;">
          <button @click="closeModal" class="admin-btn admin-btn-secondary">İptal</button>
          <button @click="saveItem" class="admin-btn admin-btn-primary" :disabled="saving">
            <i class="fas" :class="saving ? 'fa-spinner fa-spin' : 'fa-save'"></i> {{ saving ? 'Kaydediliyor...' : 'Kaydet' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import api from '@/services/api'
import Swal from 'sweetalert2'

const items = ref([])
const loading = ref(true)
const saving = ref(false)
const showModal = ref(false)
const editingItem = ref({
  id: 0,
  originalTerm: '',
  targetTerm: '',
  isActive: true
})

const fetchItems = async () => {
  loading.value = true
  try {
    const res = await api.get('/Glossary')
    items.value = res.data
  } catch (err) {
    Swal.fire('Hata', 'Sözlük yüklenemedi.', 'error')
  } finally {
    loading.value = false
  }
}

const openModal = (item = null) => {
  if (item) {
    editingItem.value = { ...item }
  } else {
    editingItem.value = { id: 0, originalTerm: '', targetTerm: '', isActive: true }
  }
  showModal.value = true
}

const closeModal = () => {
  showModal.value = false
}

const saveItem = async () => {
  if (!editingItem.value.originalTerm.trim() || !editingItem.value.targetTerm.trim()) {
    Swal.fire('Uyarı', 'Lütfen her iki kelimeyi de girin.', 'warning')
    return
  }
  
  saving.value = true
  try {
    if (editingItem.value.id) {
      await api.put(`/Glossary/${editingItem.value.id}`, editingItem.value)
    } else {
      await api.post('/Glossary', editingItem.value)
    }
    Swal.fire('Başarılı', 'Kelime kaydedildi!', 'success')
    closeModal()
    fetchItems()
  } catch (err) {
    Swal.fire('Hata', 'Kaydedilirken sorun oluştu.', 'error')
  } finally {
    saving.value = false
  }
}

const deleteItem = async (id) => {
  const result = await Swal.fire({
    title: 'Emin misiniz?',
    text: "Bu kural sözlükten kalıcı olarak silinecek!",
    icon: 'warning',
    showCancelButton: true,
    confirmButtonColor: 'var(--admin-danger)',
    cancelButtonColor: 'var(--admin-secondary)',
    confirmButtonText: 'Evet, Sil!',
    cancelButtonText: 'İptal',
    background: '#1a1a2e',
    color: '#fff'
  })

  if (result.isConfirmed) {
    try {
      await api.delete(`/Glossary/${id}`)
      Swal.fire('Silindi!', 'Kelime kuralı silindi.', 'success')
      fetchItems()
    } catch (err) {
      Swal.fire('Hata', 'Silinirken bir sorun oluştu.', 'error')
    }
  }
}

onMounted(() => {
  fetchItems()
})
</script>
