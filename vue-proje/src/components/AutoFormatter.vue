<template>
  <div class="auto-formatter-container">
    <div class="formatter-header">
      <i class="fas fa-magic" style="color: #f1c40f; font-size: 1.5rem;"></i>
      <div>
        <h3 style="margin: 0; color: var(--admin-heading);">Sihirli Metin Formatlayıcı</h3>
        <p style="margin: 0; font-size: 0.85rem; color: var(--admin-text-muted);">Düz metninizi saniyeler içinde mükemmel Markdown'a dönüştürün.</p>
      </div>
    </div>

    <div class="formatter-grid">
      <!-- SOL PANEL: TANIMLAMALAR -->
      <div class="definitions-panel">
        <h4 class="panel-title"><i class="fas fa-list-ul"></i> Hedef Kelimeler & Kodlar</h4>
        
        <!-- H2 Başlıkları -->
        <div class="form-group">
          <label>Ana Başlıklar (Her satıra bir başlık)</label>
          <p class="help-text">Bu listedeki kelimeler ana metinde bulunup kocaman <b>Renkli Başlığa</b> (Sayfa Bölücüye) dönüştürülecek.</p>
          <textarea v-model="h2List" class="admin-input" rows="4" placeholder="Giriş&#10;Birinci Bölüm&#10;Sonuç"></textarea>
        </div>

        <!-- H3 Alt Başlıkları -->
        <div class="form-group">
          <label>Alt Başlıklar (Her satıra bir başlık)</label>
          <p class="help-text">Bu kelimeler <b>Alt Başlığa</b> dönüştürülecek.</p>
          <textarea v-model="h3List" class="admin-input" rows="2" placeholder="Önemli Notlar&#10;Detaylar"></textarea>
        </div>

        <!-- Silinecek (Gereksiz) Metinler -->
        <div class="form-group" style="border-color: rgba(231,76,60,0.5); background: rgba(231,76,60,0.05);">
          <label style="color: #e74c3c; display: flex; justify-content: space-between; align-items: center;">
            <span><i class="fas fa-trash-alt"></i> Silinecek (Çöp) İfadeler</span>
            <button @click.prevent="cleanTrashPhrases" class="btn-small" 
                    :class="{'danger': deleteBtnState === 'idle', 'success': deleteBtnState === 'success', 'warning': deleteBtnState === 'not_found'}"
                    style="transition: all 0.3s; min-width: 110px;">
              <span v-if="deleteBtnState === 'idle'"><i class="fas fa-eraser"></i> Hemen Sil</span>
              <span v-else-if="deleteBtnState === 'success'"><i class="fas fa-check"></i> Silindi!</span>
              <span v-else-if="deleteBtnState === 'not_found'"><i class="fas fa-search"></i> Bulunamadı</span>
            </button>
          </label>
          <p class="help-text">Bu kutuya yazdığınız kelimeler veya cümleler metinde bulunduğunda silinir. "Hemen Sil" butonuna basarak ana metinden anında silebilirsiniz.</p>
          <textarea v-model="deleteList" class="admin-input" rows="3" placeholder="Örn: Here is the translation:"></textarea>
        </div>

        <!-- Kod Parçaları -->
        <div class="form-group">
          <label style="display: flex; justify-content: space-between; align-items: center;">
            <span>Kod Parçacıkları</span>
            <button @click.prevent="addCodeSnippet" class="btn-small"><i class="fas fa-plus"></i> Kod Ekle</button>
          </label>
          <p class="help-text">Bu metinler ana metinde bulunup siyah <b>Kod Kutusuna</b> alınacak.</p>
          
          <div v-for="(code, index) in codeSnippets" :key="index" class="code-snippet-box">
            <div style="display:flex; justify-content:space-between; margin-bottom: 0.3rem;">
              <input type="text" v-model="code.lang" class="admin-input small-input" style="width: 100px;" placeholder="Dil (js, cs)" />
              <button @click.prevent="removeCodeSnippet(index)" class="btn-small danger"><i class="fas fa-times"></i></button>
            </div>
            <textarea v-model="code.text" class="admin-input" rows="2" placeholder="Kod parçasını buraya yapıştırın..."></textarea>
          </div>
          <div v-if="codeSnippets.length === 0" class="empty-state">Henüz kod eklenmedi.</div>
        </div>
      </div>

      <!-- SAĞ PANEL: HAM METİN VE DÖNÜŞÜM -->
      <div class="text-panel">
        <h4 class="panel-title"><i class="fas fa-align-left"></i> Ana Metin (Ham Hali)</h4>
        <p class="help-text">Hiçbir formatı olmayan, düz kitabınızın tamamını buraya yapıştırın.</p>
        
        <div style="display:flex; gap:0.5rem; align-items:center; margin-bottom: 0.75rem; background: rgba(0,0,0,0.2); padding: 0.5rem 0.5rem 0.5rem 1rem; border-radius: 6px; border: 1px solid var(--admin-border);">
          <i class="fas fa-search" style="color:var(--admin-text-muted);"></i>
          <input type="text" v-model="searchQuery" @keyup.enter="findText" class="admin-input small-input" placeholder="Metin içinde kelime bul (Örn: Singleton)..." style="flex:1; margin-bottom: 0; border: none; background: transparent; box-shadow: none; outline: none; color: var(--admin-text);" />
          <button @click.prevent="findText" class="btn-small" style="background:var(--admin-primary); white-space: nowrap; padding: 0.4rem 1rem; border-radius: 4px;"><i class="fas fa-arrow-right"></i> Bul / İleri</button>
        </div>
        
        <textarea ref="rawTextAreaRef" v-model="rawText" class="admin-input raw-text-area" placeholder="Bütün kitabın / yazının metnini buraya yapıştırın..."></textarea>

        <button @click.prevent="applyFormatting" class="btn-magic" :disabled="!rawText.trim()">
          <i class="fas fa-bolt"></i> BAŞLIKLARI VE KODLARI EŞLEŞTİR (DÖNÜŞTÜR)
        </button>
      </div>
    </div>

    <!-- SONUÇ EKRANI -->
    <div v-if="resultMarkdown" class="result-section">
      <div class="result-header">
        <h4 style="margin:0; color:#2ecc71;"><i class="fas fa-check-circle"></i> Dönüşüm Başarılı! (Markdown Oluşturuldu)</h4>
        <button @click.prevent="copyResult" class="btn-small"><i class="fas fa-copy"></i> Kopyala</button>
      </div>
      <p class="help-text">İşte başlıklarınızın ve kodlarınızın otomatik ayarlandığı, yayına hazır metniniz. Bu metin otomatik olarak yazı alanınıza aktarılmıştır.</p>
      <textarea readonly :value="resultMarkdown" class="admin-input result-area"></textarea>
    </div>
  </div>
</template>

<script setup>
import { ref, watch } from 'vue'

const props = defineProps({
  modelValue: {
    type: String,
    default: ''
  }
})

const emit = defineEmits(['update:modelValue'])

// Tanımlamalar
const h2List = ref('')
const h3List = ref('')
const deleteList = ref('')
const codeSnippets = ref([])

// Ana metin
const rawText = ref(props.modelValue || '')
const resultMarkdown = ref('')

// Arama İşlevleri
const rawTextAreaRef = ref(null)
const searchQuery = ref('')
let searchStartIndex = 0

// Textarea içindeki metnin tam piksel (scroll) pozisyonunu hesaplayan sihirli fonksiyon
const getCaretCoordinates = (element, position) => {
  const div = document.createElement('div')
  const style = getComputedStyle(element)
  
  for (const prop of style) {
    div.style[prop] = style[prop]
  }
  
  div.style.position = 'absolute'
  div.style.visibility = 'hidden'
  div.style.whiteSpace = 'pre-wrap'
  div.style.wordWrap = 'break-word'
  div.style.overflow = 'hidden'
  div.style.top = '0'
  div.style.left = '0'
  div.style.width = element.clientWidth + 'px'
  
  div.textContent = element.value.substring(0, position)
  
  const span = document.createElement('span')
  span.textContent = element.value.substring(position, position + 1) || '.'
  div.appendChild(span)
  
  document.body.appendChild(div)
  const top = span.offsetTop
  document.body.removeChild(div)
  
  return top
}

const findText = () => {
  if (!searchQuery.value) return
  
  const query = searchQuery.value.trim().replace(/\s+/g, ' ')
  let escaped = query.replace(/[.*+?^${}()|[\]\\]/g, '\\$&')
  
  escaped = escaped.replace(/[iİıI]/g, '[iİıI]')
                   .replace(/[gGğĞ]/g, '[gGğĞ]')
                   .replace(/[sSşŞ]/g, '[sSşŞ]')
                   .replace(/[cCçÇ]/g, '[cCçÇ]')
                   .replace(/[oOöÖ]/g, '[oOöÖ]')
                   .replace(/[uUüÜ]/g, '[uUüÜ]')
  
  const regexStr = escaped.split(' ').join('\\s+')
  const regex = new RegExp(regexStr, 'gi')
  
  regex.lastIndex = searchStartIndex
  let match = regex.exec(rawText.value)
  
  if (!match && searchStartIndex > 0) {
    regex.lastIndex = 0
    searchStartIndex = 0
    match = regex.exec(rawText.value)
  }
  
  if (match) {
    const index = match.index
    const matchLength = match[0].length
    
    setTimeout(() => {
      const el = rawTextAreaRef.value
      if (el) {
        // Doğru yere kaydırmak için piksel hesabı yap
        const offsetTop = getCaretCoordinates(el, index)
        // Kutuyu kelimenin biraz üstünden başlat (ortalama)
        el.scrollTop = Math.max(0, offsetTop - 100)
        
        // Odaklan ve seçimi yap
        el.focus()
        el.setSelectionRange(index, index + matchLength)
      }
    }, 50)
    
    searchStartIndex = index + matchLength
  } else {
    alert("'" + searchQuery.value + "' metin içinde bulunamadı.")
  }
}

// 1. DIŞARIDAN (Örn: Yapay Zeka Çevirisi veya 'Çeviriyi Sil' butonu) gelen değişiklikleri yakala
watch(() => props.modelValue, (newVal) => {
  if (newVal !== rawText.value) {
    rawText.value = newVal || ''
  }
})

// 2. İÇERİDEN (Textarea'ya elle bir şey yazıldığında) anında parent'a (BlogSettings'e) haber ver
watch(rawText, (newVal) => {
  emit('update:modelValue', newVal)
  searchStartIndex = 0
})

const addCodeSnippet = () => {
  codeSnippets.value.push({ lang: '', text: '' })
}

const removeCodeSnippet = (index) => {
  codeSnippets.value.splice(index, 1)
}

const deleteBtnState = ref('idle') // 'idle', 'success', 'not_found'

const cleanTrashPhrases = () => {
  if (!rawText.value || !deleteList.value.trim()) return

  let newText = rawText.value
  const deleteItems = deleteList.value.split('\n').map(x => x.trim()).filter(x => x)
  
  if (deleteItems.length > 0) {
    deleteItems.forEach(delItem => {
      let escapedItem = escapeRegExp(delItem)
      escapedItem = escapedItem.replace(/[iİıI]/g, '[iİıI]')
                               .replace(/[gGğĞ]/g, '[gGğĞ]')
                               .replace(/[sSşŞ]/g, '[sSşŞ]')
                               .replace(/[cCçÇ]/g, '[cCçÇ]')
                               .replace(/[oOöÖ]/g, '[oOöÖ]')
                               .replace(/[uUüÜ]/g, '[uUüÜ]')
                               .replace(/\s+/g, '\\s+') // BOŞLUKLARI EN SON ESNEK YAP KI 's' HARFİ BOZULMASIN
      
      const regex = new RegExp(escapedItem, 'gi')
      newText = newText.replace(regex, '')
    })
    
    // Bozuk kalan fazladan boşlukları düzelt
    newText = newText.replace(/\n{3,}/g, '\n\n').trim()
    
    if (newText !== rawText.value) {
      rawText.value = newText
      deleteBtnState.value = 'success'
    } else {
      deleteBtnState.value = 'not_found'
    }
    
    setTimeout(() => {
      deleteBtnState.value = 'idle'
    }, 2000)
  }
}

const applyFormatting = () => {
  if (!rawText.value) return

  let processedText = rawText.value

  // 0. Çöp İfadeleri Temizle (Delete List)
  const deleteItems = deleteList.value.split('\n').map(x => x.trim()).filter(x => x)
  if (deleteItems.length > 0) {
    deleteItems.forEach(delItem => {
      let escapedItem = escapeRegExp(delItem)
      // Hem Türkçe hem İngilizce karakterlerde esneklik sağlamak için
      escapedItem = escapedItem.replace(/[iİıI]/g, '[iİıI]')
                               .replace(/[gGğĞ]/g, '[gGğĞ]')
                               .replace(/[sSşŞ]/g, '[sSşŞ]')
                               .replace(/[cCçÇ]/g, '[cCçÇ]')
                               .replace(/[oOöÖ]/g, '[oOöÖ]')
                               .replace(/[uUüÜ]/g, '[uUüÜ]')
                               .replace(/\s+/g, '\\s+') // BOŞLUKLARI EN SON ESNEK YAP KI 's' HARFİ BOZULMASIN
      
      const regex = new RegExp(escapedItem, 'gi')
      processedText = processedText.replace(regex, '')
    })
  }

  // 1. Önce Kodları Eşleştir (Kodların içinde başlık kelimeleri geçebileceği için önce kodlar ayrıştırılmalı)
  codeSnippets.value.forEach(snippet => {
    const textToFind = snippet.text.trim()
    if (!textToFind) return
    
    // Güvenli regex oluşturmak için özel karakterleri kaçır (escape)
    const escapedText = escapeRegExp(textToFind)
    const regex = new RegExp(`(${escapedText})`, 'g')
    const lang = snippet.lang.trim() || 'text'
    
    processedText = processedText.replace(regex, `\n\n\`\`\`${lang}\n$1\n\`\`\`\n\n`)
  })

  // 2. Ana Başlıkları (H2) Eşleştir
  const h2Items = h2List.value.split('\n').map(x => x.trim()).filter(x => x)
  h2Items.forEach(item => {
    const escapedItem = escapeRegExp(item)
    // Sadece başında ve sonunda boşluk/satır sonu olan kelimeleri veya doğrudan kelime grubunu eşleştir
    // Kod bloklarının içindekileri etkilememesi için basit bir yaklaşım kullanıyoruz.
    const regex = new RegExp(`^([ \\t]*)(${escapedItem})([ \\t]*)$`, 'gm')
    processedText = processedText.replace(regex, `$1## $2$3`)
  })

  // 3. Alt Başlıkları (H3) Eşleştir
  const h3Items = h3List.value.split('\n').map(x => x.trim()).filter(x => x)
  h3Items.forEach(item => {
    const escapedItem = escapeRegExp(item)
    const regex = new RegExp(`^([ \\t]*)(${escapedItem})([ \\t]*)$`, 'gm')
    processedText = processedText.replace(regex, `$1### $2$3`)
  })

  // Bozuk satır boşluklarını düzelt (4-5 tane üst üste boş satır olmasın)
  processedText = processedText.replace(/\n{3,}/g, '\n\n')

  resultMarkdown.value = processedText
  emit('update:modelValue', processedText)
}

const copyResult = () => {
  navigator.clipboard.writeText(resultMarkdown.value)
  alert('Panoya kopyalandı!')
}

// Regex için özel karakterleri kaçırma (escape)
function escapeRegExp(string) {
  return string.replace(/[.*+?^${}()|[\]\\]/g, '\\$&') // $& means the whole matched string
}
</script>

<style scoped>
.auto-formatter-container {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
  background: var(--admin-surface);
  border: 1px solid var(--admin-border);
  border-radius: 8px;
  padding: 1.5rem;
}

.formatter-header {
  display: flex;
  align-items: center;
  gap: 1rem;
  border-bottom: 1px solid var(--admin-border);
  padding-bottom: 1rem;
}

.formatter-grid {
  display: grid;
  grid-template-columns: 350px 1fr;
  gap: 2rem;
}

@media (max-width: 900px) {
  .formatter-grid {
    grid-template-columns: 1fr;
  }
}

.panel-title {
  color: var(--admin-primary);
  margin-top: 0;
  margin-bottom: 0.5rem;
  font-size: 1.1rem;
}

.help-text {
  font-size: 0.8rem;
  color: var(--admin-text-muted);
  margin-bottom: 0.8rem;
  line-height: 1.4;
}

.form-group {
  margin-bottom: 1.5rem;
  background: var(--admin-btn-secondary-bg);
  padding: 1rem;
  border-radius: 6px;
  border: 1px dashed var(--admin-border);
}
.form-group label {
  display: block;
  font-weight: 700;
  color: var(--admin-text);
  margin-bottom: 0.4rem;
  font-size: 0.9rem;
}

.btn-small {
  background: var(--admin-primary);
  color: white;
  border: none;
  padding: 0.3rem 0.6rem;
  font-size: 0.75rem;
  border-radius: 4px;
  cursor: pointer;
}
.btn-small.danger {
  background: var(--admin-danger);
}
.btn-small.success {
  background: #2ecc71;
  color: white;
}
.btn-small.warning {
  background: #f39c12;
  color: white;
}

.small-input {
  padding: 0.2rem 0.4rem;
  font-size: 0.8rem;
  margin: 0;
}

.code-snippet-box {
  background: var(--admin-input-bg);
  padding: 0.5rem;
  border-radius: 4px;
  margin-bottom: 0.5rem;
  border-left: 3px solid #f1c40f;
}

.empty-state {
  text-align: center;
  font-size: 0.8rem;
  color: var(--admin-text-muted);
  padding: 1rem;
  border: 1px dashed var(--admin-border);
  border-radius: 4px;
}

.text-panel {
  display: flex;
  flex-direction: column;
}

.raw-text-area {
  flex: 1;
  min-height: 400px;
  font-family: monospace;
  background: var(--admin-input-bg);
  border: 1px solid var(--admin-border);
}

/* Bulunan kelimeyi fosforlu kalemle çizilmiş gibi gösterir */
.raw-text-area::selection {
  background: #f1c40f;
  color: #000;
}

.btn-magic {
  margin-top: 1rem;
  padding: 1rem;
  font-size: 1.1rem;
  font-weight: 900;
  background: linear-gradient(45deg, var(--admin-primary), #9b59b6);
  color: #fff;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  text-shadow: 0 1px 2px rgba(0,0,0,0.5);
  box-shadow: 0 4px 15px rgba(231, 76, 60, 0.3);
  transition: all 0.3s;
  white-space: normal;
  line-height: 1.4;
}
.btn-magic:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(231, 76, 60, 0.5);
}
.btn-magic:disabled {
  opacity: 0.5;
  cursor: not-allowed;
  background: var(--admin-border);
  box-shadow: none;
}

.result-section {
  margin-top: 2rem;
  border-top: 2px dashed #2ecc71;
  padding-top: 1.5rem;
}
.result-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.5rem;
}
.result-area {
  min-height: 250px;
  font-family: monospace;
  background: rgba(46, 204, 113, 0.05);
  border: 1px solid #2ecc71;
  color: var(--admin-text-main);
}
</style>

<style>
/* Global olarak fosforlu kalem efekti (Tarayıcı önceliklerini ezmek için) */
textarea.raw-text-area::selection {
  background-color: #f1c40f !important;
  color: #000 !important;
}
textarea.raw-text-area::-moz-selection {
  background-color: #f1c40f !important;
  color: #000 !important;
}
</style>
