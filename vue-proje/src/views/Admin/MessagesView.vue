<template>
  <div class="admin-page-wrapper">
    <div class="admin-page-header">
      <div>
        <h2 class="admin-title">Gelen Mesajlar</h2>
        <p class="admin-subtitle">Sitenizin iletişim formundan gelen mesajları görüntüleyin.</p>
      </div>
      <button @click="openTrash" class="admin-btn" style="background: rgba(239, 68, 68, 0.1); color: var(--admin-danger); border: 1px solid rgba(239, 68, 68, 0.2);">
        <i class="fas fa-trash-restore"></i> Çöp Kutusu
      </button>
    </div>

    <div v-if="errorMsg" style="background: rgba(239, 68, 68, 0.1); border: 1px solid var(--admin-danger); color: var(--admin-danger); padding: 1rem; border-radius: 8px; margin-bottom: 1.5rem;">
      <i class="fas fa-exclamation-circle"></i> {{ errorMsg }}
    </div>

    <div v-if="loading" style="text-align: center; padding: 3rem; color: var(--admin-primary);">
      <i class="fas fa-spinner fa-spin fa-2x"></i>
      <p style="margin-top: 1rem;">Mesajlar yükleniyor...</p>
    </div>

    <div v-else-if="messages.length === 0" style="text-align: center; padding: 3rem; background: var(--admin-surface); border: 1px solid var(--admin-border); border-radius: var(--admin-radius-lg);">
      <i class="fas fa-inbox" style="font-size: 3rem; color: var(--admin-text-muted); margin-bottom: 1rem;"></i>
      <p style="color: var(--admin-text-muted);">Gelen kutunuz boş.</p>
    </div>

    <div v-else style="display: grid; grid-template-columns: 1fr; gap: 1rem;">
      <div v-for="msg in messages" :key="msg.id" class="admin-card" style="padding: 1.5rem; display: flex; flex-direction: column;">
        <div style="display: flex; flex-wrap: wrap; gap: 1rem; justify-content: space-between; align-items: flex-start; margin-bottom: 1rem;">
          <div>
            <h3 style="font-size: 1.1rem; color: var(--admin-heading); margin-bottom: 0.25rem;">{{ msg.ad }} {{ msg.soyad }}</h3>
            <div style="display: flex; gap: 1rem; font-size: 0.85rem; color: var(--admin-text-muted); flex-wrap: wrap;">
              <span><i class="fas fa-envelope" style="color: var(--admin-primary);"></i> <a :href="'mailto:' + msg.email" style="color: inherit; text-decoration: none;">{{ msg.email }}</a></span>
              <span v-if="msg.webSitesi"><i class="fas fa-globe" style="color: var(--admin-secondary);"></i> <a :href="msg.webSitesi" target="_blank" style="color: inherit; text-decoration: none;">{{ msg.webSitesi }}</a></span>
              <span v-if="msg.gonderimTarihi"><i class="fas fa-clock" style="color: var(--admin-text-muted);"></i> {{ formatDate(msg.gonderimTarihi) }}</span>
            </div>
          </div>
          <button @click="deleteMessage(msg.id)" class="admin-btn" style="background: rgba(239, 68, 68, 0.1); color: var(--admin-danger); border: 1px solid rgba(239, 68, 68, 0.2); padding: 0.5rem 1rem; font-size: 0.85rem;">
            <i class="fas fa-trash"></i> Sil
          </button>
        </div>
        
        <div style="background: var(--admin-surface); padding: 1rem; border-radius: var(--admin-radius-md); border-left: 3px solid var(--admin-primary); color: var(--admin-text-main); line-height: 1.6;">
          {{ msg.mesaj }}
        </div>
      </div>
    </div>
    </div>

    <!-- Trash Modal -->
    <div v-if="showTrashModal" style="position: fixed; top: 0; left: 0; right: 0; bottom: 0; background: rgba(0,0,0,0.8); z-index: 1000; display: flex; align-items: center; justify-content: center;">
      <div class="admin-card" style="width: 600px; max-width: 90%; max-height: 80vh; display: flex; flex-direction: column;">
        <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.5rem;">
          <h3 style="font-size: 1.2rem; color: var(--admin-danger); margin: 0;"><i class="fas fa-trash-alt"></i> Çöp Kutusu</h3>
          <button @click="showTrashModal = false" style="background: transparent; border: none; color: var(--admin-text-muted); cursor: pointer; font-size: 1.2rem;">
            <i class="fas fa-times"></i>
          </button>
        </div>
        
        <div v-if="loadingTrash" style="text-align: center; padding: 2rem; color: var(--admin-primary);">
          <i class="fas fa-spinner fa-spin fa-2x"></i>
        </div>
        
        <div v-else-if="trashItems.length === 0" style="text-align: center; padding: 2rem; color: var(--admin-text-muted);">
          Çöp kutusu boş.
        </div>
        
        <div v-else style="overflow-y: auto;" class="admin-scroll">
          <ul style="list-style: none; padding: 0; margin: 0;">
            <li v-for="item in trashItems" :key="item.id" style="padding: 1rem; border-bottom: 1px solid var(--admin-border); display: flex; justify-content: space-between; align-items: center;">
              <div>
                <strong style="color: var(--admin-heading); display: block; margin-bottom: 0.25rem;">{{ item.ad }} {{ item.soyad }}</strong>
                <span style="font-size: 0.8rem; color: var(--admin-text-muted);">{{ item.email }}</span>
                <span v-if="item.gonderimTarihi" style="font-size: 0.75rem; color: var(--admin-text-muted); display: block; margin-top: 2px;"><i class="fas fa-clock"></i> {{ formatDate(item.gonderimTarihi) }}</span>
              </div>
              <div style="display: flex; gap: 0.5rem;">
                <button @click="restoreMessage(item.id)" class="admin-btn admin-btn-secondary" style="padding: 0.4rem 0.8rem; font-size: 0.8rem;">
                  <i class="fas fa-undo"></i> Kurtar
                </button>
                <button @click="hardDeleteMessage(item.id)" class="admin-btn" style="background: var(--admin-danger); color: white; border: none; padding: 0.4rem 0.8rem; font-size: 0.8rem;">
                  Kalıcı Sil
                </button>
              </div>
            </li>
          </ul>
        </div>
      </div>
    </div>
 
</template>

<script setup>
import { ref, onMounted } from 'vue'
import api from '@/services/api'
import swal from '@/utils/swal'

const messages = ref([])
const loading = ref(true)
const errorMsg = ref('')

const showTrashModal = ref(false)
const trashItems = ref([])
const loadingTrash = ref(false)

const formatDate = (dateStr) => {
  if (!dateStr) return '';
  const d = new Date(dateStr);
  return d.toLocaleString('tr-TR', { 
    day: '2-digit', 
    month: 'long', 
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  });
}

const loadData = async () => {
  loading.value = true
  try {
    const res = await api.get('/Iletisim')
    messages.value = res.data
  } catch (err) {
    if (err.response && err.response.status === 404) {
      messages.value = []
    } else {
      errorMsg.value = 'Mesajlar yüklenirken bir hata oluştu.'
    }
  } finally {
    loading.value = false
  }
}

const deleteMessage = async (id) => {
  const result = await swal.fire({
    title: 'Emin misiniz?',
    text: "Bu mesaj Çöp Kutusuna taşınacak.",
    icon: 'warning',
    showCancelButton: true,
    confirmButtonText: 'Evet, Çöpe At!',
    cancelButtonText: 'İptal'
  })

  if (result.isConfirmed) {
    try {
      await api.delete(`/Iletisim/${id}`)
      await loadData()
    } catch (err) {
      errorMsg.value = 'Silme işlemi başarısız.'
    }
  }
}

const openTrash = async () => {
  showTrashModal.value = true
  await loadTrash()
}

const loadTrash = async () => {
  loadingTrash.value = true
  try {
    const res = await api.get('/Iletisim/trash')
    trashItems.value = res.data
  } catch (error) {
    console.error("Çöp kutusu yüklenemedi", error)
  } finally {
    loadingTrash.value = false
  }
}

const restoreMessage = async (id) => {
  try {
    await api.post(`/Iletisim/${id}/restore`)
    await loadTrash()
    await loadData()
    if(trashItems.value.length === 0) showTrashModal.value = false
  } catch (error) {
    alert("Kurtarma işlemi başarısız.")
  }
}

const hardDeleteMessage = async (id) => {
  const confirm = await swal.fire({
    title: 'Kalıcı Olarak Sil?',
    text: 'Bu işlem geri alınamaz!',
    icon: 'error',
    showCancelButton: true,
    confirmButtonText: 'Evet, Tamamen Sil',
    cancelButtonText: 'İptal'
  })

  if (confirm.isConfirmed) {
    try {
      await api.delete(`/Iletisim/${id}/hard`)
      await loadTrash()
      if(trashItems.value.length === 0) showTrashModal.value = false
    } catch (error) {
      alert("Kalıcı silme işlemi başarısız.")
    }
  }
}

onMounted(() => {
  loadData()
})
</script>
