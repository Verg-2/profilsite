<template>
  <div class="admin-page-wrapper">
    <div class="admin-page-header">
      <div>
        <h2 class="admin-title">Hakkında Yönetimi</h2>
        <p class="admin-subtitle">Kişisel bilgilerinizi, biyografinizi ve bilgi kartlarınızı düzenleyin.</p>
      </div>
      <div style="display: flex; gap: 1rem;">
        <button @click="translateWithAI" class="admin-btn admin-btn-ai" :disabled="aiLoading">
          <i class="fas" :class="aiLoading ? 'fa-spinner fa-spin' : 'fa-magic'"></i> 
          {{ aiLoading ? 'Çevriliyor...' : '✨ AI ile Çevir' }}
        </button>
        <button @click="openTrash" class="admin-btn" style="background: rgba(239, 68, 68, 0.1); color: var(--admin-danger); border: 1px solid rgba(239, 68, 68, 0.2);">
          <i class="fas fa-trash-restore"></i> Çöp Kutusu
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

    <div class="admin-grid-1-2">
      
      <!-- Profil ve Temel Bilgiler -->
      <div style="display: flex; flex-direction: column; gap: 2rem;">
        <div class="admin-card">
          <h3 style="font-size: 1.1rem; color: var(--admin-primary); margin-bottom: 1.5rem; border-bottom: 1px solid var(--admin-border); padding-bottom: 0.75rem;">
            Profil Kartı Görseli
          </h3>
          <ImageUploader v-model="form.profileImageUrl" label="Hakkında Sayfası Fotoğrafı" />
        </div>

        <div class="admin-card">
          <h3 style="font-size: 1.1rem; color: var(--admin-primary); margin-bottom: 1.5rem; border-bottom: 1px solid var(--admin-border); padding-bottom: 0.75rem;">
            Çalışma Durumu (Status)
          </h3>

          <!-- İş Arıyorum Şalteri -->
          <div class="admin-form-group" style="background: rgba(255, 140, 0, 0.05); padding: 1.5rem; border-radius: 8px; border: 1px solid rgba(255, 140, 0, 0.2); margin-bottom: 1.5rem;">
            <div style="display: flex; justify-content: space-between; align-items: center;">
              <div>
                <label class="admin-label" style="color: #ff8c00; font-size: 1.1rem; display:flex; align-items:center; gap:0.5rem;"><i class="fas fa-rocket"></i> "İş / Staj Arıyorum" Acil Durum Şalteri</label>
                <p style="font-size: 0.85rem; color: var(--admin-text-muted); margin-top: 0.5rem; max-width: 400px; line-height: 1.5;">
                  Bu şalteri açtığınızda ön yüzde "1 Yıldır Sektördeyim - İş Arıyorum" rozeti yanar ve Google'a (JSON-LD) "iş arıyor" sinyali ateşlenir.
                </p>
              </div>
              <label style="position: relative; display: inline-block; width: 60px; height: 34px; flex-shrink: 0;">
                <input type="checkbox" v-model="form.isLookingForJob" style="opacity: 0; width: 0; height: 0;">
                <span :style="{
                  position: 'absolute', cursor: 'pointer', top: 0, left: 0, right: 0, bottom: 0, 
                  backgroundColor: form.isLookingForJob ? '#ff8c00' : '#4b5563', transition: '.4s', borderRadius: '34px'
                }">
                  <span :style="{
                    position: 'absolute', content: '\'\'', height: '26px', width: '26px', left: '4px', bottom: '4px',
                    backgroundColor: 'white', transition: '.4s', borderRadius: '50%',
                    transform: form.isLookingForJob ? 'translateX(26px)' : 'translateX(0)'
                  }"></span>
                </span>
              </label>
            </div>
          </div>

          <div class="admin-form-group">
            <label class="admin-label">Mevcut Durum [TR]</label>
            <input type="text" v-model="form.cardTitle" placeholder="Örn: Açık / İş Arıyor / Freelance" class="admin-input" />
          </div>
          <div class="admin-form-group">
            <label class="admin-label">Mevcut Durum [EN]</label>
            <input type="text" v-model="form.cardTitleEn" placeholder="Örn: Open to work / Freelance" class="admin-input" />
          </div>
          <div class="admin-form-group">
            <label class="admin-label">Durum Detayı [TR]</label>
            <input type="text" v-model="form.cardSubtitle" placeholder="Örn: Tam Zamanlı Fırsatlara Açığım" class="admin-input" />
          </div>
          <div class="admin-form-group">
            <label class="admin-label">Durum Detayı [EN]</label>
            <input type="text" v-model="form.cardSubtitleEn" placeholder="Örn: Open to full-time opportunities" class="admin-input" />
          </div>
        </div>
      </div>

      <!-- Biyografi -->
      <div class="admin-card">
        <h3 style="font-size: 1.1rem; color: var(--admin-primary); margin-bottom: 1.5rem; border-bottom: 1px solid var(--admin-border); padding-bottom: 0.75rem;">
          Genel Biyografi
        </h3>
        
        <div class="admin-form-group">
          <label class="admin-label">Sayfa Ana Başlık [TR]</label>
          <input type="text" v-model="form.mainTitle" placeholder="Örn: Hakkımda" class="admin-input" />
        </div>
        <div class="admin-form-group">
          <label class="admin-label">Sayfa Ana Başlık [EN]</label>
          <input type="text" v-model="form.mainTitleEn" placeholder="Örn: About Me" class="admin-input" />
        </div>

        <div class="admin-form-group">
          <label class="admin-label">Sayfa Alt Başlık (Giriş Metni) [TR]</label>
          <input type="text" v-model="form.subTitle" placeholder="Örn: Tutkulu bir geliştiriciyim..." class="admin-input" />
        </div>
        <div class="admin-form-group">
          <label class="admin-label">Sayfa Alt Başlık [EN]</label>
          <input type="text" v-model="form.subTitleEn" placeholder="Örn: I am a passionate developer..." class="admin-input" />
        </div>

        <div class="admin-form-group">
          <label class="admin-label">Detaylı Biyografi (Bio) [TR]</label>
          <textarea v-model="form.bio" placeholder="Kendinizi ve vizyonunuzu detaylı anlatın..." class="admin-input" style="min-height: 250px;"></textarea>
        </div>
        <div class="admin-form-group">
          <label class="admin-label">Detaylı Biyografi [EN]</label>
          <textarea v-model="form.bioEn" placeholder="Describe yourself and your vision in detail..." class="admin-input" style="min-height: 250px;"></textarea>
        </div>
      </div>

    </div>

    <!-- Dinamik Bilgi Kartları -->
    <div class="admin-card" style="margin-top: 2rem;">
      <div style="display: flex; flex-wrap: wrap; gap: 1rem; justify-content: space-between; align-items: center; border-bottom: 1px solid var(--admin-border); padding-bottom: 0.75rem; margin-bottom: 1.5rem;">
        <h3 style="font-size: 1.1rem; color: var(--admin-primary); margin: 0;">Alt Bilgi Kartları (Eğitim, Deneyim vb.)</h3>
        <button @click="addCard" type="button" class="admin-btn admin-btn-primary" style="padding: 0.6rem 1.2rem; font-size: 0.85rem; border-radius: 6px; display: flex; align-items: center; gap: 0.5rem; height: auto;">
          <i class="fas fa-plus"></i> Yeni Kart Ekle
        </button>
      </div>

      <div v-if="form.cards.length === 0" style="text-align: center; color: var(--admin-text-muted); padding: 2rem;">
        Henüz eklenmiş bir kart yok. "Yeni Kart Ekle" butonuna tıklayarak ekleyebilirsiniz.
      </div>

      <div style="display: grid; grid-template-columns: repeat(auto-fill, minmax(min(100%, 300px), 1fr)); gap: 1.5rem;">
        <div v-for="(card, index) in form.cards" :key="index" style="background: var(--admin-surface); border: 1px solid var(--admin-border); border-radius: var(--admin-radius-md); padding: 1.5rem; position: relative;">
          
          <button @click="removeCard(index)" type="button" style="position: absolute; top: 1rem; right: 1rem; background: transparent; border: none; color: var(--admin-danger); cursor: pointer;">
            <i class="fas fa-trash-alt"></i>
          </button>

          <div class="admin-form-group">
            <label class="admin-label">Kart Türü</label>
            <select v-model="card.cardType" @change="onCardTypeChange(card)" class="admin-input" style="padding: 0.5rem;">
              <option :value="1">Normal Kart (Sadece Metin)</option>
              <option :value="2">Liste Kartı (Madde Madde)</option>
            </select>
          </div>

          <div class="admin-form-group">
            <label class="admin-label">İkon (FontAwesome / Emoji)</label>
            <IconPicker v-model="card.icon" />
          </div>

          <div class="admin-form-group">
            <label class="admin-label">Kart Başlığı [TR]</label>
            <input type="text" v-model="card.title" placeholder="Örn: Eğitim" class="admin-input" style="padding: 0.5rem;" />
          </div>
          <div class="admin-form-group">
            <label class="admin-label">Kart Başlığı [EN]</label>
            <input type="text" v-model="card.titleEn" placeholder="Örn: Education" class="admin-input" style="padding: 0.5rem;" />
          </div>

          <div v-if="card.cardType === 1" class="admin-form-group">
            <label class="admin-label">Açıklama [TR]</label>
            <textarea v-model="card.text" class="admin-input" style="min-height: 80px; padding: 0.5rem;"></textarea>
          </div>
          <div v-if="card.cardType === 1" class="admin-form-group">
            <label class="admin-label">Açıklama [EN]</label>
            <textarea v-model="card.textEn" class="admin-input" style="min-height: 80px; padding: 0.5rem;"></textarea>
          </div>

          <div v-if="card.cardType === 2" class="admin-form-group">
            <label class="admin-label">Liste Elemanları [TR] (Virgülle veya Alt Alta)</label>
            <textarea v-model="card.rawListItems" @input="updateListItems(card)" class="admin-input" style="min-height: 80px; padding: 0.5rem;" placeholder="Madde 1, Madde 2, Madde 3"></textarea>
          </div>
          <div v-if="card.cardType === 2" class="admin-form-group">
            <label class="admin-label">Liste Elemanları [EN]</label>
            <textarea v-model="card.rawListItemsEn" @input="updateListItemsEn(card)" class="admin-input" style="min-height: 80px; padding: 0.5rem;" placeholder="Item 1, Item 2, Item 3"></textarea>
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
                <strong style="color: var(--admin-heading); display: block; margin-bottom: 0.25rem;">
                  <i v-if="item.icon" :class="item.icon.includes('|') ? item.icon.split('|')[0] : item.icon" style="margin-right: 5px;"></i>
                  {{ item.title }}
                </strong>
                <span style="font-size: 0.8rem; color: var(--admin-text-muted);">{{ item.cardType === 1 ? 'Normal Kart' : 'Liste Kartı' }}</span>
              </div>
              <div style="display: flex; gap: 0.5rem;">
                <button @click="restoreCard(item.id)" class="admin-btn admin-btn-secondary" style="padding: 0.4rem 0.8rem; font-size: 0.8rem;">
                  <i class="fas fa-undo"></i> Kurtar
                </button>
                <button @click="hardDeleteCard(item.id)" class="admin-btn" style="background: var(--admin-danger); color: white; border: none; padding: 0.4rem 0.8rem; font-size: 0.8rem;">
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
import ImageUploader from '@/components/ImageUploader.vue'
import IconPicker from '@/components/IconPicker.vue'
import swal from '@/utils/swal'

const loading = ref(false)
const aiLoading = ref(false)
const errorMsg = ref('')
const successMsg = ref('')

const form = ref({
  id: 0,
  mainTitle: '',
  mainTitleEn: '',
  subTitle: '',
  subTitleEn: '',
  profileImageUrl: '',
  cardTitle: '',
  cardTitleEn: '',
  cardSubtitle: '',
  cardSubtitleEn: '',
  bio: '',
  bioEn: '',
  isLookingForJob: false,
  cards: []
})

const showTrashModal = ref(false)
const trashItems = ref([])
const loadingTrash = ref(false)

const loadData = async () => {
  try {
    const res = await api.get('/AboutSettings')
    if (res.data) {
      const data = { ...res.data }
      if (!data.cards) {
        data.cards = []
      }
      // Initialize rawListItems for easy editing
      data.cards.forEach(card => {
        if (card.listItems && card.listItems.length > 0) {
          card.rawListItems = card.listItems.join('\n')
        } else {
          card.rawListItems = ''
        }
        if (card.listItemsEn && card.listItemsEn.length > 0) {
          card.rawListItemsEn = card.listItemsEn.join('\n')
        } else {
          card.rawListItemsEn = ''
        }
      })
      form.value = data
    }
  } catch (err) {
    if (err.response && err.response.status === 404) {
      // Empty, ignore
    } else {
      errorMsg.value = 'Veriler yüklenirken bir hata oluştu.'
    }
  }
}

const addCard = () => {
  try {
    if (!form.value.cards || !Array.isArray(form.value.cards)) {
      form.value.cards = []
    }
    form.value.cards.push({
      id: 0,
      aboutSettingId: form.value.id || 0,
      cardType: 1,
      icon: '',
      title: '',
      titleEn: '',
      text: '',
      textEn: '',
      listItems: [],
      listItemsEn: [],
      rawListItems: '',
      rawListItemsEn: ''
    })
    console.log("Card added successfully. Total cards:", form.value.cards.length);
  } catch (err) {
    console.error("Error adding card:", err);
    errorMsg.value = "Kart eklenirken bir hata oluştu: " + err.message;
  }
}

const removeCard = async (index) => {
  const card = form.value.cards[index]
  if (card.id > 0) {
    const result = await swal.fire({
      title: 'Emin misiniz?',
      text: "Bu kart Çöp Kutusuna taşınacak.",
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Evet, Çöpe At!',
      cancelButtonText: 'İptal'
    })
    
    if (result.isConfirmed) {
      try {
        await api.delete(`/AboutSettings/cards/${card.id}`)
        form.value.cards.splice(index, 1)
        successMsg.value = 'Kart çöp kutusuna taşındı.'
        setTimeout(() => { successMsg.value = '' }, 3000)
      } catch (err) {
        errorMsg.value = 'Kart silinirken hata: ' + (err.response?.data || err.message)
      }
    }
  } else {
    form.value.cards.splice(index, 1)
  }
}

const openTrash = async () => {
  showTrashModal.value = true
  await loadTrash()
}

const loadTrash = async () => {
  loadingTrash.value = true
  try {
    const res = await api.get('/AboutSettings/cards/trash')
    trashItems.value = res.data
  } catch (error) {
    console.error("Çöp kutusu yüklenemedi", error)
  } finally {
    loadingTrash.value = false
  }
}

const restoreCard = async (id) => {
  try {
    await api.post(`/AboutSettings/cards/${id}/restore`)
    await loadTrash()
    await loadData()
    if(trashItems.value.length === 0) showTrashModal.value = false
  } catch (error) {
    alert("Kurtarma işlemi başarısız.")
  }
}

const hardDeleteCard = async (id) => {
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
      await api.delete(`/AboutSettings/cards/${id}/hard`)
      await loadTrash()
      if(trashItems.value.length === 0) showTrashModal.value = false
    } catch (error) {
      alert("Kalıcı silme işlemi başarısız.")
    }
  }
}

const onCardTypeChange = (card) => {
  if (card.cardType === 2) {
    if (!card.rawListItems && card.text) {
      card.rawListItems = card.text;
      updateListItems(card);
    }
  } else if (card.cardType === 1) {
    if (!card.text && card.rawListItems) {
      card.text = card.rawListItems;
    }
  }
}

const updateListItems = (card) => {
  if (card.rawListItems) {
    // Virgül veya yeni satır ile ayır
    card.listItems = card.rawListItems.split(/[,\n]+/).map(item => item.trim()).filter(item => item !== '')
  } else {
    card.listItems = []
  }
}

const updateListItemsEn = (card) => {
  if (card.rawListItemsEn) {
    card.listItemsEn = card.rawListItemsEn.split(/[,\n]+/).map(item => item.trim()).filter(item => item !== '')
  } else {
    card.listItemsEn = []
  }
}

const translateWithAI = async () => {
  aiLoading.value = true;
  errorMsg.value = '';
  
  try {
    if (form.value.mainTitle && !form.value.mainTitleEn) {
      const res = await translationService.translate(form.value.mainTitle, 'English', 'About');
      form.value.mainTitleEn = res?.translatedText || form.value.mainTitleEn;
    }
    if (form.value.subTitle && !form.value.subTitleEn) {
      const res = await translationService.translate(form.value.subTitle, 'English', 'About');
      form.value.subTitleEn = res?.translatedText || form.value.subTitleEn;
    }
    if (form.value.cardTitle && !form.value.cardTitleEn) {
      const res = await translationService.translate(form.value.cardTitle, 'English', 'About');
      form.value.cardTitleEn = res?.translatedText || form.value.cardTitleEn;
    }
    if (form.value.cardSubtitle && !form.value.cardSubtitleEn) {
      const res = await translationService.translate(form.value.cardSubtitle, 'English', 'About');
      form.value.cardSubtitleEn = res?.translatedText || form.value.cardSubtitleEn;
    }
    if (form.value.bio && !form.value.bioEn) {
      const res = await translationService.translate(form.value.bio, 'English', 'About');
      form.value.bioEn = res?.translatedText || form.value.bioEn;
    }
    
    // Kartların çevirisi
    if (form.value.cards && form.value.cards.length > 0) {
      for (let card of form.value.cards) {
        if (card.title && !card.titleEn) {
          const res = await translationService.translate(card.title, 'English', 'About');
          card.titleEn = res?.translatedText || card.titleEn;
        }
        if (card.cardType === 1 && card.text && !card.textEn) {
          const res = await translationService.translate(card.text, 'English', 'About');
          card.textEn = res?.translatedText || card.textEn;
        }
        if (card.cardType === 2 && card.rawListItems && !card.rawListItemsEn) {
          const res = await translationService.translate(card.rawListItems, 'English', 'About');
          card.rawListItemsEn = res?.translatedText || card.rawListItemsEn;
          updateListItemsEn(card);
        }
      }
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
  
  try {
    // Kartların liste öğelerini (listItems) formatla
    if (form.value.cards && form.value.cards.length > 0) {
      form.value.cards.forEach(card => {
        if (card.cardType === 2) {
          updateListItems(card);
          updateListItemsEn(card);
        }
      });
    }

    // Ana ayarları (ve içindeki kartları) tek seferde kaydet
    await api.put('/AboutSettings', form.value)
    
    // Güncel veriyi tekrar çek
    await loadData()

    successMsg.value = 'Hakkında ayarları başarıyla kaydedildi!'
    setTimeout(() => { successMsg.value = '' }, 3000)
  } catch (err) {
    errorMsg.value = 'Ayarlar kaydedilirken hata oluştu: ' + (err.response?.data || err.message)
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  loadData()
})
</script>
