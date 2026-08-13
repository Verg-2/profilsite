<template>
  <div class="skills-page">
    <canvas id="particles-canvas"></canvas>

    <header class="page-header">
      <h1>{{ lang === 'en' ? 'My Skills' : 'Yeteneklerim' }}</h1>
      <p>{{ lang === 'en' ? 'Skills acquired through years of experience and continuous learning.' : 'Yılların getirdiği deneyim ve sürekli öğrenme ile edindiğim beceriler.' }}</p>
    </header>

    <main class="skills-container" v-if="skills.length > 0">
      <div class="skill-category fade-in" v-for="cat in skills" :key="cat.id">
        <h3>
          <span v-if="cat.icon" style="margin-right: 8px;">
            <i v-if="cat.icon.startsWith('fa')" :class="cat.icon.includes('|') ? cat.icon.split('|')[0] : cat.icon"></i>
            <span v-else>{{ cat.icon }}</span>
          </span>
          {{ lang === 'en' && cat.titleEn ? cat.titleEn : cat.title }}
        </h3>
        <div class="skill-item" v-for="(item, idx) in cat.skills" :key="item.id">
          <div class="skill-info"><span class="skill-name">{{ lang === 'en' && item.nameEn ? item.nameEn : item.name }}</span><span class="skill-percent" :style="{ color: item.color }">{{ item.percentage }}%</span></div>
          <div class="skill-bar">
            <div class="skill-progress" 
                 :class="{ 'animate': animatedItems[item.id] }"
                 :style="{ 
                   background: `linear-gradient(90deg, ${item.color}99, ${item.color})`,
                   boxShadow: `0 0 15px ${item.color}66`,
                   width: item.percentage + '%' 
                 }">
            </div>
          </div>
        </div>
      </div>
    </main>
    <main v-else class="text-center" style="padding: 2rem; color: #888;">
      <p>{{ lang === 'en' ? 'No skills added yet.' : 'Henüz yetenek eklenmemiş.' }}</p>
    </main>
  </div>
</template>

<script setup>
import { ref, onMounted, inject } from 'vue';
import api from '@/services/api';

const lang = inject('lang', ref('tr'));

const skills = ref([]);
const animatedItems = ref({});

const fetchSkills = async () => {
  try {
    const res = await api.get('/Skills');
    skills.value = res.data;
    
    let delay = 150;
    skills.value.forEach(cat => {
      cat.skills.forEach(skill => {
        setTimeout(() => {
          animatedItems.value[skill.id] = true;
        }, delay);
        delay += 150;
      });
    });
    
    setTimeout(() => {
      const elements = document.querySelectorAll('.skills-page .fade-in');
      elements.forEach(el => el.classList.add('visible'));
    }, 100);
  } catch (error) {
    console.error("Yetenekler yüklenirken hata oluştu", error);
  }
}

onMounted(() => {
  fetchSkills();
});
</script>
