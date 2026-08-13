<template>
  <div class="admin-page-wrapper">
    <div class="admin-page-header">
      <div>
        <h2 class="admin-title">Yetenekler Yönetimi</h2>
        <p class="admin-subtitle">Yetenek kategorilerini ve altındaki becerileri yönetin.</p>
      </div>
      <div style="display: flex; gap: 1rem;">
        <button @click="translateWithAI" class="admin-btn admin-btn-ai" :disabled="aiLoading">
          <i class="fas" :class="aiLoading ? 'fa-spinner fa-spin' : 'fa-magic'"></i> 
          {{ aiLoading ? 'Çevriliyor...' : '✨ AI ile Çevir' }}
        </button>
        <button @click="openTrash" class="admin-btn" style="background: rgba(239, 68, 68, 0.1); color: var(--admin-danger); border: 1px solid rgba(239, 68, 68, 0.2);">
          <i class="fas fa-trash-restore"></i> Çöp Kutusu
        </button>
      </div>
    </div>

    <div v-if="errorMsg" style="background: rgba(239, 68, 68, 0.1); border: 1px solid var(--admin-danger); color: var(--admin-danger); padding: 1rem; border-radius: 8px; margin-bottom: 1.5rem;">
      <i class="fas fa-exclamation-circle"></i> {{ errorMsg }}
    </div>

    <div v-if="loading" style="text-align: center; padding: 3rem; color: var(--admin-primary);">
      <i class="fas fa-spinner fa-spin fa-2x"></i>
      <p style="margin-top: 1rem;">Yetenekler yükleniyor...</p>
    </div>

    <div v-else style="display: grid; grid-template-columns: 1fr; gap: 2rem;">
      
      <!-- New Category Form -->
      <div class="admin-card" style="padding: 1.5rem;">
        <h3 style="font-size: 1.1rem; color: var(--admin-primary); margin-bottom: 1rem;">Yeni Kategori Ekle</h3>
        <form @submit.prevent="addCategory" style="display: flex; flex-wrap: wrap; gap: 1rem; align-items: flex-end;">
          <div class="admin-form-group" style="margin-bottom: 0; flex: 1;">
            <label class="admin-label">Kategori Başlığı [TR]</label>
            <input type="text" v-model="newCat.title" class="admin-input" placeholder="Örn: Frontend" required />
          </div>
          <div class="admin-form-group" style="margin-bottom: 0; flex: 1;">
            <label class="admin-label">Kategori Başlığı [EN]</label>
            <input type="text" v-model="newCat.titleEn" class="admin-input" placeholder="e.g. Frontend" />
          </div>
          <div class="admin-form-group" style="margin-bottom: 0; flex: 1; z-index: 10;">
            <label class="admin-label">Kategori İkonu</label>
            <IconPicker v-model="newCat.icon" mode="icon" />
          </div>
          <button type="submit" class="admin-btn admin-btn-primary">
            <i class="fas fa-plus"></i> Kategori Ekle
          </button>
        </form>
      </div>

      <!-- Categories List -->
      <div v-for="cat in categories" :key="cat.id" class="admin-card">
        <div style="display: flex; justify-content: space-between; align-items: center; border-bottom: 1px solid var(--admin-border); padding-bottom: 1rem; margin-bottom: 1.5rem;">
          <h3 style="font-size: 1.25rem; color: var(--admin-heading); margin: 0; display: flex; align-items: center; gap: 0.75rem;">
            <span v-if="cat.icon">
              <i v-if="cat.icon.startsWith('fa')" :class="cat.icon.includes('|') ? cat.icon.split('|')[0] : cat.icon" style="color: var(--admin-primary);"></i>
              <span v-else>{{ cat.icon }}</span>
            </span>
            {{ cat.title }}
          </h3>
        </div>

        <!-- Add Skill to Category -->
        <form @submit.prevent="addSkill(cat.id)" style="display: flex; flex-wrap: wrap; gap: 1rem; margin-bottom: 2rem; background: var(--admin-surface); padding: 1rem; border-radius: var(--admin-radius-md); border: 1px solid var(--admin-border);">
          <input type="text" v-model="newSkill[cat.id].name" class="admin-input" placeholder="Yetenek Adı [TR] (Örn: İletişim)" required />
          <input type="text" v-model="newSkill[cat.id].nameEn" class="admin-input" placeholder="Yetenek Adı [EN] (Örn: Communication)" />
          <input type="number" v-model="newSkill[cat.id].percentage" class="admin-input" placeholder="Yüzde (0-100)" min="0" max="100" style="width: 120px; flex: 1;" required />
          <input type="color" v-model="newSkill[cat.id].color" class="admin-input" style="width: 60px; padding: 0.25rem;" title="Renk Seçin" />
          <button type="submit" class="admin-btn admin-btn-secondary">Ekle</button>
        </form>

        <!-- Skills List -->
        <div v-if="!cat.skills || cat.skills.length === 0" style="color: var(--admin-text-muted); text-align: center; padding: 1rem;">
          Bu kategoride henüz yetenek yok.
        </div>
        
        <div v-else style="display: grid; grid-template-columns: repeat(auto-fill, minmax(min(100%, 300px), 1fr)); gap: 1rem;">
          <div v-for="skill in cat.skills" :key="skill.id" style="background: var(--admin-surface); border: 1px solid var(--admin-border); padding: 1rem; border-radius: var(--admin-radius-sm); display: flex; align-items: center; min-height: 70px;">
            
            <div v-if="editingSkill === skill.id" style="display: flex; flex-wrap: wrap; gap: 10px; width: 100%; align-items: center;">
              <input type="text" v-model="skill.name" class="admin-input" placeholder="TR" style="flex: 2; padding: 0.5rem;" />
              <input type="text" v-model="skill.nameEn" class="admin-input" placeholder="EN" style="flex: 2; padding: 0.5rem;" />
              <input type="number" v-model="skill.percentage" class="admin-input" style="flex: 1; padding: 0.5rem;" min="0" max="100" />
              <input type="color" v-model="skill.color" style="width: 35px; height: 35px; border: none; border-radius: 4px; cursor: pointer; background: transparent; padding: 0;" />
              <button @click="saveSkillEdit(skill)" class="admin-btn admin-btn-primary" style="padding: 0.5rem 0.75rem;"><i class="fas fa-check"></i></button>
              <button @click="editingSkill = null; loadData()" class="admin-btn admin-btn-secondary" style="padding: 0.5rem 0.75rem;"><i class="ph ph-x"></i></button>
            </div>
            
            <div v-else style="flex: 1; display: flex; justify-content: space-between; align-items: center;">
              <div style="flex: 1;">
                <div style="display: flex; justify-content: space-between; margin-bottom: 0.5rem;">
                  <span style="font-weight: 500; color: var(--admin-heading);">{{ skill.name }}</span>
                  <span style="color: var(--admin-text-muted); font-size: 0.9rem;">%{{ skill.percentage }}</span>
                </div>
                <!-- Progress Bar Preview -->
                <div style="height: 6px; background: var(--admin-border); border-radius: 3px; overflow: hidden;">
                  <div :style="{ width: skill.percentage + '%', backgroundColor: skill.color }" style="height: 100%;"></div>
                </div>
              </div>
              <div style="display: flex; margin-left: 1rem; gap: 0.5rem;">
                <button @click="editingSkill = skill.id" style="background: transparent; border: none; color: var(--admin-secondary); cursor: pointer; padding: 0.5rem;" title="Düzenle">
                  <i class="fas fa-pen"></i>
                </button>
                <button @click="deleteSkill(skill.id, cat.id)" style="background: transparent; border: none; color: var(--admin-danger); cursor: pointer; padding: 0.5rem;" title="Sil">
                  <i class="ph ph-x"></i>
                </button>
              </div>
            </div>

          </div>
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
                <strong style="color: var(--admin-heading); display: block; margin-bottom: 0.25rem;">{{ item.name }}</strong>
                <span style="font-size: 0.8rem; color: var(--admin-text-muted);">{{ item.percentage }}%</span>
              </div>
              <div style="display: flex; gap: 0.5rem;">
                <button @click="restoreSkill(item.id)" class="admin-btn admin-btn-secondary" style="padding: 0.4rem 0.8rem; font-size: 0.8rem;">
                  <i class="fas fa-undo"></i> Kurtar
                </button>
                <button @click="hardDeleteSkill(item.id)" class="admin-btn" style="background: var(--admin-danger); color: white; border: none; padding: 0.4rem 0.8rem; font-size: 0.8rem;">
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
import translationService from '@/services/translationService'
import IconPicker from '@/components/IconPicker.vue'
import swal from '@/utils/swal'

const categories = ref([])
const loading = ref(true)
const aiLoading = ref(false)
const errorMsg = ref('')

const newCat = ref({ title: '', titleEn: '', icon: '' })
const newSkill = ref({})
const editingSkill = ref(null)

const showTrashModal = ref(false)
const trashItems = ref([])
const loadingTrash = ref(false)

const loadData = async () => {
  loading.value = true
  try {
    const res = await api.get('/Skills')
    categories.value = res.data
    // Initialize newSkill forms
    categories.value.forEach(cat => {
      if (!newSkill.value[cat.id]) {
        newSkill.value[cat.id] = { name: '', nameEn: '', percentage: 80, color: '#ff5500' }
      }
    })
  } catch (err) {
    errorMsg.value = 'Veriler yüklenirken hata oluştu.'
  } finally {
    loading.value = false
  }
}

const addCategory = async () => {
  try {
    const res = await api.post('/Skills', newCat.value)
    newCat.value = { title: '', titleEn: '', icon: '' }
    await loadData()
  } catch (err) {
    errorMsg.value = 'Kategori eklenemedi.'
  }
}

const addSkill = async (categoryId) => {
  const skillData = {
    skillCategoryId: categoryId,
    name: newSkill.value[categoryId].name,
    nameEn: newSkill.value[categoryId].nameEn,
    percentage: newSkill.value[categoryId].percentage,
    color: newSkill.value[categoryId].color
  }

  try {
    await api.post('/Skills/items', skillData)
    newSkill.value[categoryId] = { name: '', nameEn: '', percentage: 80, color: '#ff5500' }
    await loadData()
  } catch (err) {
    errorMsg.value = 'Yetenek eklenemedi.'
  }
}

const saveSkillEdit = async (skill) => {
  try {
    await api.put(`/Skills/items/${skill.id}`, skill)
    editingSkill.value = null
    await loadData()
  } catch (err) {
    let detail = err.message;
    if (err.response && err.response.data) {
      detail = typeof err.response.data === 'string' ? err.response.data : JSON.stringify(err.response.data);
    }
    errorMsg.value = 'Yetenek güncellenemedi: ' + detail;
  }
}

const deleteSkill = async (skillId, categoryId) => {
  const result = await swal.fire({
    title: 'Emin misiniz?',
    text: "Bu yetenek Çöp Kutusuna taşınacak.",
    icon: 'warning',
    showCancelButton: true,
    confirmButtonText: 'Evet, Çöpe At!',
    cancelButtonText: 'İptal'
  })

  if (result.isConfirmed) {
    try {
      await api.delete(`/Skills/items/${skillId}`)
      await loadData()
    } catch (err) {
      errorMsg.value = 'Silinemedi.'
    }
  }
}

const translateWithAI = async () => {
  aiLoading.value = true;
  swal.fire({
    title: 'Yapay Zeka Çeviriyor...',
    html: 'Kategoriler ve yetenekler İngilizceye çevriliyor. Lütfen bekleyin...',
    allowOutsideClick: false,
    didOpen: () => {
      swal.showLoading();
    }
  });

  try {
    // Kategori ve Yetenek çevirilerini backend'e kaydet
    for (let cat of categories.value) {
      let catUpdated = false;
      if (cat.title && !cat.titleEn) {
        const res = await translationService.translate(cat.title, 'English', 'Skills');
        cat.titleEn = res?.translatedText || cat.titleEn;
        catUpdated = true;
      }
      
      if (catUpdated) {
        // Kategoriyi güncelle
        await api.put(`/Skills/${cat.id}`, cat);
      }
      
      if (cat.skills) {
        for (let skill of cat.skills) {
          if (skill.name && !skill.nameEn) {
            const res = await translationService.translate(skill.name, 'English', 'Skills');
            skill.nameEn = res?.translatedText || skill.nameEn;
            await saveSkillEdit(skill);
          }
        }
      }
    }
    
    // Yeni ekleme formlarını çevir
    if (newCat.value.title && !newCat.value.titleEn) {
      const res = await translationService.translate(newCat.value.title, 'English', 'Skills');
      newCat.value.titleEn = res?.translatedText || newCat.value.titleEn;
    }
    
    for (const categoryId in newSkill.value) {
      if (newSkill.value[categoryId].name && !newSkill.value[categoryId].nameEn) {
        const res = await translationService.translate(newSkill.value[categoryId].name, 'English', 'Skills');
        newSkill.value[categoryId].nameEn = res?.translatedText || newSkill.value[categoryId].nameEn;
      }
    }
    
    await loadData();
    swal.fire('Başarılı', 'Tüm yetenekler başarıyla İngilizceye çevrildi!', 'success');
  } catch (err) {
    swal.fire('Hata', 'Çeviri sırasında bir hata oluştu.', 'error');
  } finally {
    aiLoading.value = false;
  }
}

const openTrash = async () => {
  showTrashModal.value = true
  await loadTrash()
}

const loadTrash = async () => {
  loadingTrash.value = true
  try {
    const res = await api.get('/Skills/items/trash')
    trashItems.value = res.data
  } catch (error) {
    console.error("Çöp kutusu yüklenemedi", error)
  } finally {
    loadingTrash.value = false
  }
}

const restoreSkill = async (id) => {
  try {
    await api.post(`/Skills/items/${id}/restore`)
    await loadTrash()
    await loadData()
    if(trashItems.value.length === 0) showTrashModal.value = false
  } catch (error) {
    alert("Kurtarma işlemi başarısız.")
  }
}

const hardDeleteSkill = async (id) => {
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
      await api.delete(`/Skills/items/${id}/hard`)
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
