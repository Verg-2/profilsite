<template>
  <div class="admin-page-wrapper">
    <div class="admin-page-header">
      <div>
        <h2 class="admin-title">API Yönetimi (Load Balancer)</h2>
        <p class="admin-subtitle">Sisteminize eklenen API anahtarlarını, görevlerini ve kullanım istatistiklerini buradan yönetin.</p>
      </div>
      <button @click="openAddModal" class="admin-btn admin-btn-primary">
        <i class="fas fa-plus"></i> Yeni API Key Ekle
      </button>
    </div>

    <div v-if="errorMsg" class="alert-error"><i class="fas fa-exclamation-circle"></i> {{ errorMsg }}</div>
    <div v-if="successMsg" class="alert-success"><i class="fas fa-check-circle"></i> {{ successMsg }}</div>

    <!-- Analytics Dashboard Cards -->
    <div v-if="!loading && apiKeys.length > 0" style="display: grid; grid-template-columns: repeat(auto-fit, minmax(250px, 1fr)); gap: 1.5rem; margin-bottom: 2rem;">
      <div class="admin-card" style="display: flex; align-items: center; gap: 1rem;">
        <div style="background: rgba(59,130,246,0.1); color: var(--admin-primary); width: 60px; height: 60px; border-radius: 15px; display: flex; align-items: center; justify-content: center; font-size: 1.8rem;">
          <i class="fas fa-network-wired"></i>
        </div>
        <div>
          <h4 style="margin: 0; color: var(--admin-text-muted); font-size: 0.9rem;">Toplam İstek</h4>
          <p style="margin: 0.2rem 0 0 0; font-size: 1.5rem; font-weight: 700; color: var(--admin-heading);">{{ totalRequests.toLocaleString() }}</p>
        </div>
      </div>
      <div class="admin-card" style="display: flex; align-items: center; gap: 1rem;">
        <div style="background: rgba(139,92,246,0.1); color: #8b5cf6; width: 60px; height: 60px; border-radius: 15px; display: flex; align-items: center; justify-content: center; font-size: 1.8rem;">
          <i class="fas fa-coins"></i>
        </div>
        <div>
          <h4 style="margin: 0; color: var(--admin-text-muted); font-size: 0.9rem;">Harcanan Token</h4>
          <p style="margin: 0.2rem 0 0 0; font-size: 1.5rem; font-weight: 700; color: var(--admin-heading);">{{ totalTokens.toLocaleString() }}</p>
        </div>
      </div>
      <div class="admin-card" style="display: flex; align-items: center; gap: 1rem;">
        <div style="background: rgba(16,185,129,0.1); color: var(--admin-success); width: 60px; height: 60px; border-radius: 15px; display: flex; align-items: center; justify-content: center; font-size: 1.8rem;">
          <i class="fas fa-dollar-sign"></i>
        </div>
        <div>
          <h4 style="margin: 0; color: var(--admin-text-muted); font-size: 0.9rem;">Tahmini Toplam Maliyet</h4>
          <p style="margin: 0.2rem 0 0 0; font-size: 1.5rem; font-weight: 700; color: var(--admin-heading);">${{ totalCost.toFixed(4) }}</p>
        </div>
      </div>
    </div>

    <div v-if="loading" style="text-align:center;padding:3rem;color:var(--admin-primary);">
      <i class="fas fa-spinner fa-spin fa-2x"></i>
    </div>

    <div v-else-if="apiKeys.length === 0" style="text-align:center;padding:3rem;background:var(--admin-surface);border:1px solid var(--admin-border);border-radius:var(--admin-radius-lg);">
      <i class="fas fa-key" style="font-size:3rem;color:var(--admin-text-muted);display:block;margin-bottom:1rem;"></i>
      <p style="color:var(--admin-text-muted);">Henüz sisteme eklenmiş bir API anahtarı bulunmuyor.</p>
    </div>

    <div v-else class="admin-card" style="padding: 0; overflow: hidden;">
      <table style="width: 100%; border-collapse: collapse; text-align: left;">
        <thead>
          <tr style="background: var(--admin-surface-hover); border-bottom: 1px solid var(--admin-border);">
            <th style="padding: 1rem; color: var(--admin-text-muted); font-weight: 600;">Durum</th>
            <th style="padding: 1rem; color: var(--admin-text-muted); font-weight: 600;">Anonim İsim (Alias)</th>
            <th style="padding: 1rem; color: var(--admin-text-muted); font-weight: 600;">Sağlayıcı</th>
            <th style="padding: 1rem; color: var(--admin-text-muted); font-weight: 600;">Görev / Sayfa</th>
            <th style="padding: 1rem; color: var(--admin-text-muted); font-weight: 600;">Kullanım Sayısı</th>
            <th style="padding: 1rem; color: var(--admin-text-muted); font-weight: 600;">Harcanan Token</th>
            <th style="padding: 1rem; color: var(--admin-text-muted); font-weight: 600;">Tahmini Maliyet</th>
            <th style="padding: 1rem; color: var(--admin-text-muted); font-weight: 600;">Son Kullanım</th>
            <th style="padding: 1rem; color: var(--admin-text-muted); font-weight: 600; text-align: right;">İşlemler</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="key in apiKeys" :key="key.id" style="border-bottom: 1px solid var(--admin-border); transition: background 0.2s;" onmouseover="this.style.background='var(--admin-surface-hover)'" onmouseout="this.style.background='transparent'">
            <td style="padding: 1rem;">
              <div style="display: flex; align-items: center; gap: 0.5rem;">
                <span @click="toggleStatus(key.id)" style="cursor: pointer;" :title="key.isActive ? 'Devre Dışı Bırak' : 'Aktifleştir'">
                  <i v-if="key.isActive" class="fas fa-toggle-on" style="color: var(--admin-success); font-size: 1.5rem;"></i>
                  <i v-else class="fas fa-toggle-off" style="color: var(--admin-text-muted); font-size: 1.5rem;"></i>
                </span>
                
                <span v-if="!key.lastError" class="badge-success" title="Sağlıklı" style="background: rgba(16, 185, 129, 0.1); color: #10b981; padding: 0.2rem 0.5rem; border-radius: 12px; font-size: 0.75rem; border: 1px solid rgba(16,185,129,0.2);">
                  <i class="fas fa-check-circle"></i>
                </span>
                <span v-else-if="key.lastError.includes('402') || key.lastError.includes('credit') || key.lastError.includes('balance')" class="badge-error" :title="key.lastError" style="background: rgba(239, 68, 68, 0.1); color: #ef4444; padding: 0.2rem 0.5rem; border-radius: 12px; font-size: 0.75rem; border: 1px solid rgba(239,68,68,0.2); cursor: help;">
                  <i class="fas fa-wallet"></i> Bitti
                </span>
                <span v-else-if="key.lastError.includes('429') || key.lastError.includes('rate limit')" class="badge-warning" :title="key.lastError" style="background: rgba(245, 158, 11, 0.1); color: #f59e0b; padding: 0.2rem 0.5rem; border-radius: 12px; font-size: 0.75rem; border: 1px solid rgba(245,158,11,0.2); cursor: help;">
                  <i class="fas fa-hourglass-half"></i> Limit
                </span>
                <span v-else class="badge-error" :title="key.lastError" style="background: rgba(239, 68, 68, 0.1); color: #ef4444; padding: 0.2rem 0.5rem; border-radius: 12px; font-size: 0.75rem; border: 1px solid rgba(239,68,68,0.2); cursor: help;">
                  <i class="fas fa-exclamation-triangle"></i> Hata
                </span>
              </div>
            </td>
            <td style="padding: 1rem; font-weight: 500; color: var(--admin-heading);">
              <i class="fas fa-shield-alt" style="color: var(--admin-primary); margin-right: 0.5rem;" title="AES ile Şifreli"></i>
              {{ key.alias }}
            </td>
            <td style="padding: 1rem; color: var(--admin-heading);">
              <span v-if="key.provider === 'Google'" style="color: #ea4335;"><i class="fab fa-google"></i> Google</span>
              <span v-else-if="key.provider === 'GoogleTranslateFree'" style="color: #4285F4;"><i class="fas fa-language"></i> Google Translate (Ücretsiz)</span>
              <span v-else-if="key.provider === 'OpenAI'" style="color: #10a37f;"><i class="fas fa-robot"></i> OpenAI / Custom</span>
              <span v-else-if="key.provider === 'Anthropic'" style="color: #d97757;"><i class="fas fa-brain"></i> Anthropic</span>
              <span v-else-if="key.provider === 'Groq'" style="color: #f55036;"><i class="fas fa-bolt"></i> Groq</span>
              <span v-else-if="key.provider === 'Cloudflare'" style="color: #f38020;"><i class="fas fa-cloud"></i> Cloudflare AI</span>
              <span v-else style="color: var(--admin-primary);"><i class="fas fa-microchip"></i> {{ key.provider }}</span>
            </td>
            <td style="padding: 1rem;">
              <span style="background: rgba(59,130,246,0.1); color: #60a5fa; border: 1px solid rgba(59,130,246,0.2); padding: 0.2rem 0.6rem; border-radius: 20px; font-size: 0.8rem; font-weight: 600;">
                {{ key.assignedTask }}
              </span>
            </td>
            <td style="padding: 1rem; color: var(--admin-heading);">
              {{ key.requestCount }} istek
            </td>
            <td style="padding: 1rem; color: var(--admin-heading);">
              <span style="color: var(--admin-primary); font-weight: 600;">{{ (key.totalTokensUsed || 0).toLocaleString() }}</span> token
            </td>
            <td style="padding: 1rem; color: var(--admin-heading);">
              <span style="color: var(--admin-success); font-weight: 600;">
                ${{ calculateCost(key.provider, key.modelName, key.totalTokensUsed).toFixed(4) }}
              </span>
            </td>
            <td style="padding: 1rem; color: var(--admin-text-muted); font-size: 0.9rem;">
              {{ formatDate(key.lastUsedDate) || 'Hiç kullanılmadı' }}
            </td>
            <td style="padding: 1rem; text-align: right; white-space: nowrap;">
              <div style="display: flex; justify-content: flex-end; gap: 0.5rem; align-items: center;">
                <button @click="openEditModal(key)" class="admin-btn admin-btn-secondary" style="padding: 0.4rem 0.8rem; margin-right: 0;">
                  <i class="fas fa-pen"></i>
                </button>
                <button @click="deleteKey(key.id)" class="admin-btn" style="background:rgba(239,68,68,0.1); color:var(--admin-danger); border:1px solid rgba(239,68,68,0.2); padding:0.4rem 0.8rem;">
                  <i class="fas fa-trash"></i>
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Modal -->
    <div v-if="showModal" style="position: fixed; inset: 0; background: rgba(0,0,0,0.8); z-index: 1000; display: flex; align-items: center; justify-content: center; padding: 1rem;">
      <div class="admin-card" style="width: 500px; max-width: 100%; max-height: 90vh; overflow-y: auto; display: flex; flex-direction: column;">
        <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.5rem;">
          <h3 style="margin: 0; color: var(--admin-heading); font-size: 1.2rem;">
            {{ currentKey.id ? 'API Anahtarı Düzenle' : 'Yeni API Anahtarı Ekle' }}
          </h3>
          <button @click="showModal = false" style="background: transparent; border: none; color: var(--admin-text-muted); cursor: pointer; font-size: 1.2rem;">
            <i class="fas fa-times"></i>
          </button>
        </div>

        <form @submit.prevent="saveKey" style="display: flex; flex-direction: column; gap: 1rem;">
          <div class="admin-form-group">
            <label class="admin-label">Anonim İsim (Alias) *</label>
            <input type="text" v-model="currentKey.alias" class="admin-input" placeholder="Örn: Yedek-Key-1" required />
          </div>

          <div class="admin-form-group">
            <label class="admin-label">Gerçek API Key (Şifrelenecek) {{ currentKey.id ? '(Değiştirmeyecekseniz boş bırakın)' : '*' }}</label>
            <div style="position: relative;">
              <i class="fas fa-lock" style="position: absolute; left: 12px; top: 14px; color: var(--admin-text-muted);"></i>
              <input type="password" v-model="currentKey.keyValue" class="admin-input" style="padding-left: 2.2rem;" placeholder="sk-gemini-..." :required="!currentKey.id && currentKey.provider !== 'GoogleTranslateFree'" :disabled="currentKey.provider === 'GoogleTranslateFree'" />
            </div>
            <p style="font-size: 0.8rem; color: var(--admin-text-muted); margin-top: 0.5rem; margin-bottom: 0;">
              <i class="fas fa-info-circle"></i> Bu anahtar veritabanına kaydedilmeden önce AES-256 ile şifrelenecektir. Google Translate Ücretsiz seçilirse anahtar gerekmez.
            </p>
          </div>

          <div class="admin-form-group">
            <label class="admin-label">Yapay Zeka Sağlayıcısı *</label>
            <select v-model="currentKey.provider" @change="onProviderChange" class="admin-input" required>
              <option value="Google">Google (Gemini Pro, Flash vs.)</option>
              <option value="GoogleTranslateFree">Google Translate (Ücretsiz / Limitsiz)</option>
              <option value="OpenAI">OpenAI (ChatGPT)</option>
              <option value="Groq">Groq (Aşırı Hızlı)</option>
              <option value="DeepSeek">DeepSeek</option>
              <option value="Anthropic">Anthropic (Claude)</option>
              <option value="Mistral">Mistral AI</option>
              <option value="OpenRouter">OpenRouter</option>
              <option value="TogetherAI">Together AI</option>
              <option value="HuggingFace">Hugging Face</option>
              <option value="Cloudflare">Cloudflare AI</option>
              <option value="Custom">Diğer (Özel OpenAI Uyumlu Sunucu)</option>
            </select>
          </div>

          <div v-if="['OpenAI', 'Groq', 'DeepSeek', 'Mistral', 'OpenRouter', 'TogetherAI', 'HuggingFace', 'Cloudflare', 'Custom', 'Anthropic'].includes(currentKey.provider) && currentKey.provider !== 'Google'" class="admin-form-group">
            <label class="admin-label">Base URL (Opsiyonel)</label>
            <input type="text" v-model="currentKey.baseUrl" class="admin-input" placeholder="API adresini girin..." />
            <p style="font-size: 0.8rem; color: var(--admin-text-muted); margin-top: 0.5rem; margin-bottom: 0;">
              Seçtiğiniz sağlayıcının standart API adresi otomatik ayarlanır. (Not: Cloudflare için linkteki {account_id} kısmına kendi id'nizi yazmalısınız).
            </p>
          </div>

          <div v-if="['OpenAI', 'Groq', 'DeepSeek', 'Mistral', 'OpenRouter', 'TogetherAI', 'HuggingFace', 'Cloudflare', 'Custom', 'Anthropic'].includes(currentKey.provider) && currentKey.provider !== 'Google'" class="admin-form-group">
            <label class="admin-label">Model Adı (Opsiyonel)</label>
            <input type="text" v-model="currentKey.modelName" class="admin-input" placeholder="Kullanılacak modeli girin..." />
            <p style="font-size: 0.8rem; color: var(--admin-text-muted); margin-top: 0.5rem; margin-bottom: 0;">
              Boş bırakılırsa sağlayıcının standart modeli kullanılır.
            </p>
          </div>

          <div class="admin-form-group">
            <label class="admin-label">Sorumlu Olduğu Görev (Section) *</label>
            <select v-model="currentKey.assignedTask" class="admin-input" required>
              <option value="Genel">Genel (Tüm Sayfalar / Fallback)</option>
              <option value="Home">Anasayfa (Home)</option>
              <option value="About">Hakkımda (About)</option>
              <option value="Project">Projeler (Project)</option>
              <option value="Blog">Blog</option>
              <option value="Contact">İletişim (Contact)</option>
              <option value="Seo">SEO (Seo)</option>
              <option value="Skills">Yetenekler (Skills)</option>
              <option value="QaExpert">Ürün Kontrol Uzmanı (QA - Çeviri Denetimi)</option>
              <option value="SecurityAnalyzer">Siber Güvenlik Analizörü (SAST)</option>
            </select>
          </div>

          <div style="display: flex; justify-content: flex-end; gap: 1rem; margin-top: 1rem;">
            <button type="button" @click="showModal = false" class="admin-btn admin-btn-secondary">İptal</button>
            <button type="submit" class="admin-btn admin-btn-primary" :disabled="saving">
              <i class="fas" :class="saving ? 'fa-spinner fa-spin' : 'fa-save'"></i> {{ saving ? 'Kaydediliyor...' : 'Kaydet' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import api from '@/services/api'
import swal from '@/utils/swal'

const apiKeys = ref([])
const loading = ref(true)
const saving = ref(false)
const showModal = ref(false)
const errorMsg = ref('')
const successMsg = ref('')

const currentKey = ref({
  id: 0,
  alias: '',
  keyValue: '',
  assignedTask: 'Genel',
  provider: 'Google',
  baseUrl: '',
  modelName: ''
})

const calculateCost = (provider, modelName, tokens) => {
  if (!tokens || tokens === 0) return 0;
  
  let costPer1M = 0;
  const p = provider?.toLowerCase() || '';
  const m = modelName?.toLowerCase() || '';
  
  if (p === 'google' || p === 'gemini') {
    costPer1M = 0.07; // Gemini 1.5 Flash
    if (m.includes('pro')) costPer1M = 1.25;
  } else if (p === 'openai') {
    costPer1M = 5.00; // GPT-4o
    if (m.includes('mini') || m.includes('3.5')) costPer1M = 0.15;
  } else if (p === 'anthropic') {
    costPer1M = 3.00; // Sonnet
    if (m.includes('haiku')) costPer1M = 0.25;
    if (m.includes('opus')) costPer1M = 15.00;
  } else if (p === 'deepseek') {
    costPer1M = 0.14; // DeepSeek
  } else if (p === 'groq') {
    costPer1M = 0.05; // Groq/Llama
  } else if (p === 'mistral') {
    costPer1M = 0.20; // Mistral
    if (m.includes('large')) costPer1M = 2.00;
  } else if (p === 'custom' || p === 'moonshot') {
    costPer1M = 1.00; // Kimi approx
  } else {
    costPer1M = 0.50; // Generic default
  }
  
  return (tokens / 1000000) * costPer1M;
};

const totalTokens = computed(() => {
  return apiKeys.value.reduce((sum, key) => sum + (key.totalTokensUsed || 0), 0);
});

const totalRequests = computed(() => {
  return apiKeys.value.reduce((sum, key) => sum + (key.requestCount || 0), 0);
});

const totalCost = computed(() => {
  return apiKeys.value.reduce((sum, key) => sum + calculateCost(key.provider, key.modelName, key.totalTokensUsed), 0);
});

const formatDate = (dateString) => {
  if (!dateString) return null;
  const date = new Date(dateString);
  return date.toLocaleString('tr-TR', { 
    year: 'numeric', 
    month: 'short', 
    day: 'numeric', 
    hour: '2-digit', 
    minute: '2-digit' 
  });
}

const loadKeys = async () => {
  loading.value = true;
  try {
    const res = await api.get('/ApiKeys');
    apiKeys.value = res.data?.data || [];
  } catch (error) {
    errorMsg.value = 'API anahtarları yüklenirken hata oluştu.';
  } finally {
    loading.value = false;
  }
}

const onProviderChange = () => {
  const p = currentKey.value.provider;
  if (p === 'GoogleTranslateFree') {
    currentKey.value.baseUrl = '';
    currentKey.value.modelName = '';
    currentKey.value.keyValue = 'FREE_BACKDOOR';
  } else if (p === 'Google') {
    currentKey.value.baseUrl = '';
    currentKey.value.modelName = '';
  } else if (p === 'OpenAI') {
    currentKey.value.baseUrl = 'https://api.openai.com/v1/chat/completions';
    currentKey.value.modelName = 'gpt-3.5-turbo';
  } else if (p === 'Groq') {
    currentKey.value.baseUrl = 'https://api.groq.com/openai/v1/chat/completions';
    currentKey.value.modelName = 'llama-3.1-8b-instant';
  } else if (p === 'DeepSeek') {
    currentKey.value.baseUrl = 'https://api.deepseek.com/chat/completions';
    currentKey.value.modelName = 'deepseek-chat';
  } else if (p === 'Mistral') {
    currentKey.value.baseUrl = 'https://api.mistral.ai/v1/chat/completions';
    currentKey.value.modelName = 'mistral-large-latest';
  } else if (p === 'Anthropic') {
    currentKey.value.baseUrl = 'https://api.anthropic.com/v1/messages';
    currentKey.value.modelName = 'claude-3-haiku-20240307';
  } else if (p === 'OpenRouter') {
    currentKey.value.baseUrl = 'https://openrouter.ai/api/v1/chat/completions';
    currentKey.value.modelName = 'google/gemini-pro'; // OpenRouter model is required, example given
  } else if (p === 'TogetherAI') {
    currentKey.value.baseUrl = 'https://api.together.xyz/v1/chat/completions';
    currentKey.value.modelName = 'meta-llama/Llama-3-8b-chat-hf';
  } else if (p === 'HuggingFace') {
    currentKey.value.baseUrl = 'https://router.huggingface.co/v1/chat/completions';
    currentKey.value.modelName = 'meta-llama/Meta-Llama-3-8B-Instruct';
  } else if (p === 'Cloudflare') {
    currentKey.value.baseUrl = 'https://api.cloudflare.com/client/v4/accounts/BURAYA_ACCOUNT_ID_YAZIN/ai/v1/chat/completions';
    currentKey.value.modelName = '@cf/meta/llama-3-8b-instruct';
  } else {
    currentKey.value.baseUrl = '';
    currentKey.value.modelName = '';
  }
}

const openAddModal = () => {
  currentKey.value = { id: 0, alias: '', keyValue: '', assignedTask: 'Genel', provider: 'Google', baseUrl: '', modelName: '' };
  showModal.value = true;
}

const openEditModal = (key) => {
  currentKey.value = { ...key, keyValue: '' }; // Şifreyi göstermiyoruz
  showModal.value = true;
}

const saveKey = async () => {
  saving.value = true;
  errorMsg.value = '';
  try {
    if (currentKey.value.id > 0) {
      await api.put(`/ApiKeys/${currentKey.value.id}`, currentKey.value);
      successMsg.value = 'API anahtarı başarıyla güncellendi.';
    } else {
      await api.post('/ApiKeys', currentKey.value);
      successMsg.value = 'Yeni API anahtarı başarıyla eklendi ve şifrelendi.';
    }
    showModal.value = false;
    await loadKeys();
    setTimeout(() => { successMsg.value = '' }, 3000);
  } catch (error) {
    errorMsg.value = error.response?.data?.message || 'Kaydedilirken hata oluştu.';
    setTimeout(() => { errorMsg.value = '' }, 3000);
  } finally {
    saving.value = false;
  }
}

const toggleStatus = async (id) => {
  try {
    await api.put(`/ApiKeys/${id}/toggle`);
    await loadKeys();
  } catch (error) {
    alert("Durum güncellenirken hata oluştu.");
  }
}

const deleteKey = async (id) => {
  const result = await swal.fire({
    title: 'Emin misiniz?',
    text: "Bu API anahtarını tamamen silmek üzeresiniz.",
    icon: 'warning',
    showCancelButton: true,
    confirmButtonText: 'Evet, Sil',
    cancelButtonText: 'İptal'
  });

  if (result.isConfirmed) {
    try {
      await api.delete(`/ApiKeys/${id}`);
      successMsg.value = 'API anahtarı silindi.';
      await loadKeys();
      setTimeout(() => { successMsg.value = '' }, 3000);
    } catch (error) {
      errorMsg.value = 'Silinirken hata oluştu.';
      setTimeout(() => { errorMsg.value = '' }, 3000);
    }
  }
}

onMounted(() => {
  loadKeys();
})
</script>

<style scoped>
.alert-error {
  background: rgba(239,68,68,0.1);
  border: 1px solid var(--admin-danger);
  color: var(--admin-danger);
  padding: 1rem;
  border-radius: 8px;
  margin-bottom: 1.5rem;
}
.alert-success {
  background: rgba(16,185,129,0.1);
  border: 1px solid var(--admin-success);
  color: var(--admin-success);
  padding: 1rem;
  border-radius: 8px;
  margin-bottom: 1.5rem;
}
</style>
