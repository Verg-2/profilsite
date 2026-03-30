<template>
  <div class="skills-page">
    <canvas id="particles-canvas"></canvas>

    <header class="page-header">
      <h1>Yeteneklerim</h1>
      <p>Yılların getirdiği deneyim ve sürekli öğrenme ile edindiğim beceriler.</p>
    </header>

    <main class="skills-container">
      <div class="skill-category fade-in">
        <h3>💻 Yazılım Teknolojileri</h3>
        <div class="skill-item">
          <div class="skill-info"><span class="skill-name">HTML/CSS</span><span class="skill-percent">30%</span></div>
          <div class="skill-bar"><div class="skill-progress frontend" data-width="30" data-delay="1000"></div></div>
        </div>
        <div class="skill-item">
          <div class="skill-info"><span class="skill-name">JavaScript</span><span class="skill-percent">20%</span></div>
          <div class="skill-bar"><div class="skill-progress frontend" data-width="20" data-delay="2000"></div></div>
        </div>
        <div class="skill-item">
          <div class="skill-info"><span class="skill-name">Vue.js</span><span class="skill-percent">10%</span></div>
          <div class="skill-bar"><div class="skill-progress frontend" data-width="10" data-delay="3000"></div></div>
        </div>
        <div class="skill-item">
          <div class="skill-info"><span class="skill-name">Kotlin</span><span class="skill-percent">30%</span></div>
          <div class="skill-bar"><div class="skill-progress frontend" data-width="30" data-delay="4000"></div></div>
        </div>
        <div class="skill-item">
          <div class="skill-info"><span class="skill-name">C#</span><span class="skill-percent">30%</span></div>
          <div class="skill-bar"><div class="skill-progress frontend" data-width="30" data-delay="5000"></div></div>
        </div>
        <div class="skill-item">
          <div class="skill-info"><span class="skill-name">PHP</span><span class="skill-percent">75%</span></div>
          <div class="skill-bar"><div class="skill-progress frontend" data-width="75" data-delay="6000"></div></div>
        </div>
      </div>

      <div class="skill-category fade-in">
        <h3>🛠️ Araçlar</h3>
        <div class="skill-item">
          <div class="skill-info"><span class="skill-name">Git</span><span class="skill-percent">90%</span></div>
          <div class="skill-bar"><div class="skill-progress tools" data-width="90" data-delay="1000"></div></div>
        </div>
        <div class="skill-item">
          <div class="skill-info"><span class="skill-name">Docker</span><span class="skill-percent">70%</span></div>
          <div class="skill-bar"><div class="skill-progress tools" data-width="70" data-delay="2000"></div></div>
        </div>
        <div class="skill-item">
          <div class="skill-info"><span class="skill-name">Figma</span><span class="skill-percent">85%</span></div>
          <div class="skill-bar"><div class="skill-progress tools" data-width="85" data-delay="3000"></div></div>
        </div>
        <div class="skill-item">
          <div class="skill-info"><span class="skill-name">VS Code</span><span class="skill-percent">95%</span></div>
          <div class="skill-bar"><div class="skill-progress tools" data-width="95" data-delay="4000"></div></div>
        </div>
        <div class="skill-item">
          <div class="skill-info"><span class="skill-name">Linux</span><span class="skill-percent">75%</span></div>
          <div class="skill-bar"><div class="skill-progress tools" data-width="75" data-delay="5000"></div></div>
        </div>
      </div>

      <div class="skill-category fade-in">
        <h3>🤝 Yetkinlikler</h3>
        <div class="skill-item">
          <div class="skill-info"><span class="skill-name">Problem Çözme</span><span class="skill-percent">92%</span></div>
          <div class="skill-bar"><div class="skill-progress soft" data-width="92" data-delay="1000"></div></div>
        </div>
        <div class="skill-item">
          <div class="skill-info"><span class="skill-name">Takım Çalışması</span><span class="skill-percent">88%</span></div>
          <div class="skill-bar"><div class="skill-progress soft" data-width="88" data-delay="2000"></div></div>
        </div>
        <div class="skill-item">
          <div class="skill-info"><span class="skill-name">İletişim</span><span class="skill-percent">85%</span></div>
          <div class="skill-bar"><div class="skill-progress soft" data-width="85" data-delay="3000"></div></div>
        </div>
        <div class="skill-item">
          <div class="skill-info"><span class="skill-name">Analitik Düşünce</span><span class="skill-percent">90%</span></div>
          <div class="skill-bar"><div class="skill-progress soft" data-width="90" data-delay="4000"></div></div>
        </div>
        <div class="skill-item">
          <div class="skill-info"><span class="skill-name">Öğrenme</span><span class="skill-percent">95%</span></div>
          <div class="skill-bar"><div class="skill-progress soft" data-width="95" data-delay="5000"></div></div>
        </div>
      </div>

      <div class="skill-category fade-in">
        <h3>🗄️ Veri Tabanları</h3>
        <div class="skill-item">
          <div class="skill-info"><span class="skill-name">MySQL</span><span class="skill-percent">82%</span></div>
          <div class="skill-bar"><div class="skill-progress database" data-width="82" data-delay="1000"></div></div>
        </div>
        <div class="skill-item">
          <div class="skill-info"><span class="skill-name">PostgreSQL</span><span class="skill-percent">76%</span></div>
          <div class="skill-bar"><div class="skill-progress database" data-width="76" data-delay="2000"></div></div>
        </div>
        <div class="skill-item">
          <div class="skill-info"><span class="skill-name">MongoDB</span><span class="skill-percent">72%</span></div>
          <div class="skill-bar"><div class="skill-progress database" data-width="72" data-delay="3000"></div></div>
        </div>
        <div class="skill-item">
          <div class="skill-info"><span class="skill-name">SQL Server</span><span class="skill-percent">70%</span></div>
          <div class="skill-bar"><div class="skill-progress database" data-width="70" data-delay="4000"></div></div>
        </div>
      </div>
    </main>
  </div>
</template>

<script setup>
// Progress animasyonları page-animations.js içindeki initSkillBars ile tetikleniyor
</script>
