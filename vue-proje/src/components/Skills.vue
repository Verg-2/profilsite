<template>
  <section id="skills" class="skills-section reveal" :class="{ 'visible': animated }">
    <div class="page-header">
      <h1>Yeteneklerim</h1>
      <p>Yılların getirdiği deneyim ve sürekli öğrenme ile edindiğim beceriler.</p>
    </div>

    <div class="skills-container">
      <div class="skill-category">
        <h3>💻 Frontend</h3>
        <div v-for="skill in frontendSkills" :key="skill.name" class="skill-item">
          <div class="skill-info">
            <span class="skill-name">{{ skill.name }}</span>
            <span class="skill-percent">{{ skill.percent }}%</span>
          </div>
          <div class="skill-bar">
            <div 
              class="skill-progress frontend"
              :style="{ 
                width: animated ? skill.percent + '%' : '0%',
                transitionDelay: skill.delay + 'ms' 
              }"
            ></div>
          </div>
        </div>
      </div>
      </div>
  </section>
</template>

