<template>
  <div class="icon-picker">
    <div class="selected-icon-preview" @click="isOpen = !isOpen">
      <template v-if="modelValue">
        <span v-if="modelValue.includes('<svg')" v-safe-html="modelValue" class="svg-icon-wrapper" style="font-size: 1.5rem; display: flex; align-items: center;"></span>
        <span v-else-if="mode === 'emoji' || (!modelValue.includes('fa-') && !modelValue.includes('ph-'))" style="font-size: 1.5rem;">{{ modelValue }}</span>
        <i v-else :class="modelValue" style="font-size: 1.2rem;"></i>
      </template>
      <span v-else>{{ mode === 'emoji' ? 'Emoji Seç...' : 'İkon Seç...' }}</span>
      <i class="fas fa-chevron-down" style="margin-left: auto;"></i>
    </div>
    
    <div v-if="isOpen" class="icon-dropdown">
      <div class="icon-grid">
        <div 
          v-for="item in processedIcons" 
          :key="item.value" 
          class="icon-item" 
          :class="{ active: modelValue === item.value }"
          :title="item.title"
          @click="selectIcon(item.value)"
        >
          <span v-if="item.value.includes('<svg')" v-safe-html="item.value" class="svg-icon-wrapper"></span>
          <span v-else-if="mode === 'emoji' || (!item.value.includes('fa-') && !item.value.includes('ph-'))">{{ item.value }}</span>
          <i v-else :class="item.value"></i>

          <button type="button" v-if="item.isCustom" @click.stop="deleteCustomIcon(item.value)" class="delete-icon-btn" title="Sil">
            <i class="ph ph-x"></i>
          </button>
        </div>
      </div>
      
      <div class="custom-icon-input" v-if="mode === 'icon'">
        <div style="display: flex; gap: 8px; margin-bottom: 8px;">
          <input type="text" v-model="customIconTitle" placeholder="İkon Adı (Örn: X Logo)" class="admin-input" style="padding: 6px 10px; font-size: 0.85rem; flex: 1;" />
        </div>
        <div style="display: flex; gap: 8px;">
          <input type="text" v-model="customIcon" placeholder="Sınıf Adı veya <svg> Kodu" class="admin-input" style="padding: 6px 10px; font-size: 0.85rem; flex: 2;" />
          <button type="button" @click="addCustomIcon" class="admin-btn admin-btn-secondary" style="padding: 6px 12px; font-size: 0.85rem;">Ekle</button>
        </div>
      </div>

      <div class="custom-icon-input" v-if="mode === 'emoji'">
        <div style="display: flex; gap: 8px; margin-bottom: 8px;">
          <input type="text" v-model="customIconTitle" placeholder="Emoji Adı (Örn: Yıldız)" class="admin-input" style="padding: 6px 10px; font-size: 0.85rem; flex: 1;" />
        </div>
        <div style="display: flex; gap: 8px;">
          <input type="text" v-model="customIcon" placeholder="Örn: 🦄 veya <svg> Kodu" class="admin-input" style="padding: 6px 10px; font-size: 0.85rem; flex: 2;" />
          <button type="button" @click="addCustomIcon" class="admin-btn admin-btn-secondary" style="padding: 6px 12px; font-size: 0.85rem;">Ekle</button>
        </div>
      </div>

      <div style="text-align: center; margin-top: 10px; border-top: 1px solid var(--admin-border); padding-top: 8px;">
        <button type="button" @click.stop="selectIcon('')" style="background: transparent; border: none; color: var(--admin-text-main); font-size: 0.8rem; cursor: pointer;">
          <i class="fas fa-eraser"></i> Seçimi Kaldır (Standart)
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'

const props = defineProps({
  modelValue: {
    type: String,
    default: ''
  },
  mode: {
    type: String,
    default: 'icon'
  }
})

const emit = defineEmits(['update:modelValue'])

const isOpen = ref(false)
const customIcon = ref('')
const customIconTitle = ref('')
const customIconsList = ref([])

// Kapsamlı Yazılım Dilleri, Araçlar, Veritabanları
const defaultFontAwesomeIcons = [
  // Phosphor Icons (Hakkında ve genel kullanım için)
  'ph ph-user', 'ph ph-briefcase', 'ph ph-graduation-cap', 'ph ph-code',
  'ph ph-terminal', 'ph ph-paint-brush', 'ph ph-rocket-launch', 'ph ph-globe-hemisphere-west',
  'ph ph-database', 'ph ph-cpu', 'ph ph-cloud', 'ph ph-device-mobile',
  'ph ph-laptop', 'ph ph-desktop', 'ph ph-gear', 'ph ph-wrench',
  'ph ph-lightbulb', 'ph ph-trend-up', 'ph ph-chart-bar', 'ph ph-target',
  'ph ph-certificate', 'ph ph-book-open', 'ph ph-medal', 'ph ph-star',
  'ph ph-lightning', 'ph ph-fire', 'ph ph-heart', 'ph ph-coffee',
  'ph ph-folder', 'ph ph-files', 'ph ph-pen-nib', 'ph ph-layout',
  
  // FontAwesome Icons
  'fa-brands fa-vuejs', 'fa-brands fa-react', 'fa-brands fa-angular', 
  'fa-brands fa-js', 'fa-brands fa-python', 'fa-brands fa-java', 
  'fa-brands fa-php', 'fa-brands fa-swift', 'fa-brands fa-golang',
  'fa-brands fa-rust', 'fa-brands fa-html5', 'fa-brands fa-css3-alt',
  'fa-brands fa-node-js', 'fa-brands fa-npm', 'fa-brands fa-yarn',
  'fa-brands fa-docker', 'fa-brands fa-aws', 'fa-brands fa-linux', 
  
  // Social Media & Contact Icons
  'fa-brands fa-linkedin', 'fa-brands fa-linkedin-in', 'fa-brands fa-github', 
  'fa-brands fa-gitlab', 'fa-brands fa-instagram', 'fa-brands fa-twitter', 
  'fa-brands fa-x-twitter', 'fa-brands fa-facebook', 'fa-brands fa-youtube', 
  'fa-brands fa-discord', 'fa-brands fa-tiktok', 'fa-brands fa-twitch', 
  'fa-brands fa-whatsapp', 'fa-brands fa-telegram', 'fa-brands fa-medium', 
  'fa-brands fa-dribbble', 'fa-brands fa-behance', 'fa-solid fa-location-dot',
  'fa-solid fa-map-location-dot', 'fa-solid fa-envelope', 'fa-solid fa-phone',
  
  // Diğer Araçlar
  'fa-brands fa-figma', 'fa-brands fa-sass', 'fa-brands fa-less',
  'fa-brands fa-bootstrap', 'fa-brands fa-windows', 'fa-brands fa-apple',
  'fa-brands fa-android', 'fa-brands fa-ubuntu', 'fa-brands fa-codepen',
  'fa-solid fa-database', 'fa-solid fa-server', 'fa-solid fa-code', 
  'fa-solid fa-terminal', 'fa-solid fa-bolt', 'fa-solid fa-wand-magic-sparkles', 
  'fa-solid fa-palette', 'fa-solid fa-mobile-screen', 'fa-solid fa-globe', 
  'fa-solid fa-cloud', 'fa-solid fa-laptop-code', 'fa-solid fa-microchip', 
  'fa-solid fa-bug', 'fa-solid fa-rocket', 'fa-solid fa-cogs', 'fa-solid fa-shield-halved',
  'fa-solid fa-toolbox', 'fa-solid fa-briefcase', 'fa-solid fa-brain',
  'fa-solid fa-gears', 'fa-solid fa-layer-group', 'fa-solid fa-screwdriver-wrench'
]

const defaultEmojis = [
  '🚀', '🎨', '💡', '💾', '🐙', '🐳', '💻', '⚙️', '🔥', '⚡', '🌟', '🛠️', 
  '📈', '📱', '🌐', '🛡️', '📦', '🔑', '🎯', '🧠', '🔬', '🎮', '🎧', '📸',
  '📚', '✏️', '🏆', '💎', '🧩', '🧪', '🧬', '📊', '📝', '🔒', '🔓', '✅',
  '🚧', '♻️', '🌍', '🛸', '🤖', '👾'
]

const fontAwesomeIcons = ref([...defaultFontAwesomeIcons])
const emojis = ref([...defaultEmojis])

onMounted(() => {
  const savedV2 = localStorage.getItem('customAdminItemsV2')
  if (savedV2) {
    try {
      customIconsList.value = JSON.parse(savedV2)
    } catch(e) {}
  } else {
    // Eski versiyonları al ve V2 formatına çevir
    let oldCustoms = [];
    const savedIcons = localStorage.getItem('customAdminIcons')
    if (savedIcons) {
      try {
        const parsed = JSON.parse(savedIcons)
        if (Array.isArray(parsed)) {
          parsed.forEach(val => oldCustoms.push({ value: val, title: val, mode: 'icon' }))
        }
      } catch(e) {}
    }
    const savedEmojis = localStorage.getItem('customAdminEmojis')
    if (savedEmojis) {
      try {
        const parsed = JSON.parse(savedEmojis)
        if (Array.isArray(parsed)) {
          parsed.forEach(val => oldCustoms.push({ value: val, title: val, mode: 'emoji' }))
        }
      } catch(e) {}
    }
    if (oldCustoms.length > 0) {
      customIconsList.value = oldCustoms;
      localStorage.setItem('customAdminItemsV2', JSON.stringify(customIconsList.value));
    }
  }
})

const processedIcons = computed(() => {
  const base = props.mode === 'emoji' ? emojis.value : fontAwesomeIcons.value;
  
  const formattedBase = base.map(val => {
     let title = val;
     if (val.includes('fa-')) {
       let parts = val.split('-');
       title = parts[parts.length - 1]; 
       title = title.charAt(0).toUpperCase() + title.slice(1);
     } else if (val.includes('ph-')) {
       title = val.replace('ph ph-', '');
       title = title.charAt(0).toUpperCase() + title.slice(1);
     }
     return { value: val, title, isCustom: false }
  });

  const customs = customIconsList.value.filter(c => c.mode === props.mode).map(c => ({
    value: c.value,
    title: c.title || 'Özel İkon',
    isCustom: true
  }))

  return [...customs, ...formattedBase];
})

const addCustomIcon = () => {
  let val = customIcon.value;
  let title = customIconTitle.value;
  if (!val) return;
  val = val.trim();
  
  if (!title) {
    if (val.includes('<svg')) title = 'Özel SVG İkonu'
    else title = val;
  }
  
  const exists = customIconsList.value.find(c => c.value === val);
  if (!exists) {
    customIconsList.value.push({ value: val, title, mode: props.mode });
    localStorage.setItem('customAdminItemsV2', JSON.stringify(customIconsList.value));
  }
  
  selectIcon(val);
}

const deleteCustomIcon = (val) => {
  if(confirm('Bu özel ikonu silmek istediğinize emin misiniz?')) {
    customIconsList.value = customIconsList.value.filter(c => c.value !== val);
    localStorage.setItem('customAdminItemsV2', JSON.stringify(customIconsList.value));
    if (props.modelValue === val) {
       emit('update:modelValue', '');
    }
  }
}

const selectIcon = (icon) => {
  if (icon !== undefined && icon !== null) {
    icon = icon.trim()
    emit('update:modelValue', icon)
  }
  customIcon.value = ''
  customIconTitle.value = ''
  isOpen.value = false
}
</script>

<style scoped>
.icon-picker {
  position: relative;
  width: 100%;
}
.selected-icon-preview {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 12px 16px;
  background: var(--admin-surface);
  border: 1px solid var(--admin-border);
  border-radius: 8px;
  color: var(--admin-text-main);
  cursor: pointer;
  transition: all 0.2s;
}
.selected-icon-preview:hover {
  border-color: rgba(255, 77, 0, 0.5);
}
.icon-dropdown {
  position: absolute;
  top: 100%;
  left: 0;
  width: 320px;
  max-width: 90vw;
  background: var(--admin-surface);
  border: 1px solid var(--admin-border);
  border-radius: 8px;
  margin-top: 8px;
  padding: 12px;
  z-index: 100;
  box-shadow: 0 10px 30px rgba(0,0,0,0.6);
}
.icon-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(45px, 1fr));
  gap: 8px;
  max-height: 220px;
  overflow-y: auto;
  margin-bottom: 12px;
  padding-right: 5px;
}
.icon-grid::-webkit-scrollbar {
  width: 6px;
}
.icon-grid::-webkit-scrollbar-track {
  background: rgba(255,255,255,0.02);
  border-radius: 6px;
}
.icon-grid::-webkit-scrollbar-thumb {
  background-color: var(--admin-border);
  border-radius: 6px;
}
.icon-item {
  display: flex;
  align-items: center;
  justify-content: center;
  height: 45px;
  border-radius: 6px;
  background: rgba(255,255,255,0.03);
  cursor: pointer;
  transition: all 0.2s;
  font-size: 1.4rem;
  color: var(--admin-text-muted);
  border: 1px solid transparent;
  position: relative;
}

.delete-icon-btn {
  position: absolute;
  top: -4px;
  right: -4px;
  background: var(--admin-danger);
  color: white;
  border: none;
  border-radius: 50%;
  width: 18px;
  height: 18px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.7rem;
  cursor: pointer;
  opacity: 0;
  transform: scale(0.8);
  transition: all 0.2s;
  box-shadow: 0 2px 4px rgba(0,0,0,0.5);
}

.icon-item:hover .delete-icon-btn {
  opacity: 1;
  transform: scale(1);
}

.delete-icon-btn:hover {
  background: #ff3333;
  transform: scale(1.1) !important;
}

.svg-icon-wrapper :deep(svg) {
  width: 1.4rem;
  height: 1.4rem;
  display: block;
}
.icon-item:hover, .icon-item.active {
  background: rgba(255, 77, 0, 0.1);
  color: var(--admin-primary);
  border: 1px solid var(--admin-primary);
}
.custom-icon-input {
  display: flex;
  gap: 8px;
  border-top: 1px solid var(--admin-border);
  padding-top: 12px;
}
</style>
