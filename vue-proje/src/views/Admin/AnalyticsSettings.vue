<template>
  <div class="admin-page-wrapper">
    <div class="admin-page-header">
      <div>
        <h2 class="admin-title">İstatistikler ve Analiz</h2>
        <p class="admin-subtitle">Sitenizin genel trafiğini ve etkileşim oranlarını görüntüleyin.</p>
      </div>
      <button @click="loadData" class="admin-btn admin-btn-secondary" :disabled="loading">
        <i class="fas" :class="loading ? 'fa-spinner fa-spin' : 'fa-sync-alt'"></i> Yenile
      </button>
    </div>

    <div v-if="errorMsg" style="background: rgba(239, 68, 68, 0.1); border: 1px solid var(--admin-danger); color: var(--admin-danger); padding: 1rem; border-radius: 8px; margin-bottom: 1.5rem;">
      <i class="fas fa-exclamation-circle"></i> {{ errorMsg }}
    </div>

    <div v-if="loading" style="text-align: center; padding: 3rem; color: var(--admin-primary);">
      <i class="fas fa-spinner fa-spin fa-2x"></i>
      <p style="margin-top: 1rem;">Veriler yükleniyor...</p>
    </div>

    <div v-else style="display: grid; grid-template-columns: repeat(auto-fit, minmax(min(100%, 240px), 1fr)); gap: 1.5rem; margin-bottom: 2rem;">
      
      <div class="admin-card" style="display: flex; align-items: center; gap: 1.5rem; padding: 2rem;">
        <div style="background: rgba(0, 212, 255, 0.1); color: var(--admin-secondary); width: 60px; height: 60px; border-radius: 12px; display: flex; align-items: center; justify-content: center; font-size: 2rem;">
          <i class="fas fa-users"></i>
        </div>
        <div>
          <h3 style="font-size: 0.9rem; color: var(--admin-text-muted); text-transform: uppercase; letter-spacing: 0.05em; margin-bottom: 0.25rem;">Toplam Ziyaretçi</h3>
          <p style="font-size: 2rem; font-weight: 700; color: var(--admin-heading);">{{ stats.visitorsTotal }}</p>
        </div>
      </div>

      <div class="admin-card" style="display: flex; align-items: center; gap: 1.5rem; padding: 2rem;">
        <div style="background: rgba(255, 85, 0, 0.1); color: var(--admin-primary); width: 60px; height: 60px; border-radius: 12px; display: flex; align-items: center; justify-content: center; font-size: 2rem;">
          <i class="fas fa-user-clock"></i>
        </div>
        <div>
          <h3 style="font-size: 0.9rem; color: var(--admin-text-muted); text-transform: uppercase; letter-spacing: 0.05em; margin-bottom: 0.25rem;">Bugünkü Ziyaretçi</h3>
          <p style="font-size: 2rem; font-weight: 700; color: var(--admin-heading);">{{ stats.visitorsToday }}</p>
        </div>
      </div>

      <div class="admin-card" style="display: flex; align-items: center; gap: 1.5rem; padding: 2rem;">
        <div style="background: rgba(16, 185, 129, 0.1); color: var(--admin-success); width: 60px; height: 60px; border-radius: 12px; display: flex; align-items: center; justify-content: center; font-size: 2rem;">
          <i class="fas fa-project-diagram"></i>
        </div>
        <div>
          <h3 style="font-size: 0.9rem; color: var(--admin-text-muted); text-transform: uppercase; letter-spacing: 0.05em; margin-bottom: 0.25rem;">Toplam Proje</h3>
          <p style="font-size: 2rem; font-weight: 700; color: var(--admin-heading);">{{ stats.projectCount }}</p>
        </div>
      </div>

      <div class="admin-card" style="display: flex; align-items: center; gap: 1.5rem; padding: 2rem;">
        <div style="background: rgba(16, 185, 129, 0.1); color: #8b5cf6; width: 60px; height: 60px; border-radius: 12px; display: flex; align-items: center; justify-content: center; font-size: 2rem;">
          <i class="fas fa-envelope"></i>
        </div>
        <div>
          <h3 style="font-size: 0.9rem; color: var(--admin-text-muted); text-transform: uppercase; letter-spacing: 0.05em; margin-bottom: 0.25rem;">Gelen Mesaj</h3>
          <p style="font-size: 2rem; font-weight: 700; color: var(--admin-heading);">{{ stats.messageCount }}</p>
        </div>
      </div>

    </div>

    <!-- System Health Overview -->
    <div v-if="!loading" class="admin-card">
      <h3 style="font-size: 1.25rem; color: var(--admin-heading); margin-bottom: 1.5rem;">Sistem Sağlığı Logları (Son 100)</h3>
      
      <div v-if="healthLogs.length === 0" style="text-align: center; color: var(--admin-text-muted); padding: 2rem;">
        Sistemde kayıtlı hata logu bulunmuyor. Her şey yolunda!
      </div>
      
      <div v-else style="overflow-x: auto;">
        <table style="width: 100%; border-collapse: collapse; text-align: left;">
          <thead>
            <tr style="border-bottom: 1px solid var(--admin-border); color: var(--admin-text-muted); font-size: 0.9rem;">
              <th style="padding: 1rem;">Tarih</th>
              <th style="padding: 1rem;">Hata Türü</th>
              <th style="padding: 1rem;">Detay</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="log in healthLogs" :key="log.id" style="border-bottom: 1px solid rgba(255,255,255,0.05);">
              <td style="padding: 1rem; color: #d1d5db; white-space: nowrap;">{{ formatDate(log.logDate) }}</td>
              <td style="padding: 1rem;">
                <span style="background: rgba(239,68,68,0.1); color: var(--admin-danger); padding: 0.25rem 0.5rem; border-radius: 4px; font-size: 0.85rem;">{{ log.errorType }}</span>
              </td>
              <td style="padding: 1rem; color: #9ca3af;">{{ log.details }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import api from '@/services/api'

const stats = ref({ visitorsTotal: 0, visitorsToday: 0, projectCount: 0, messageCount: 0 })
const healthLogs = ref([])
const loading = ref(true)
const errorMsg = ref('')

const formatDate = (dateString) => {
  if (!dateString) return ''
  const d = new Date(dateString)
  return d.toLocaleString('tr-TR')
}

const loadData = async () => {
  loading.value = true
  errorMsg.value = ''
  try {
    const [statsRes, healthRes] = await Promise.all([
      api.get('/Analytics/stats'),
      api.get('/Analytics/health')
    ])
    stats.value = statsRes.data
    healthLogs.value = healthRes.data
  } catch (err) {
    errorMsg.value = 'Analiz verileri yüklenirken bir hata oluştu.'
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  loadData()
})
</script>
