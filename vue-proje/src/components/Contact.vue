<template>
  <section class="section contact" id="contact">
    <!-- Arka plan çizgiler -->
    <div class="contact-bg-lines">
      <div class="line" v-for="n in 5" :key="n" :style="{ animationDelay: n * 0.5 + 's' }"></div>
    </div>
    
    <div class="container">
      <h2 class="section-title reveal">İletişim</h2>
      
      <div class="contact-grid">
        <!-- Sol taraf - Bilgiler -->
        <div class="contact-info">
          <!-- E-posta -->
          <a href="mailto:ornek@email.com" class="contact-card reveal">
            <div class="card-icon">📧</div>
            <div class="card-content">
              <h3>E-posta</h3>
              <p>ornek@email.com</p>
            </div>
          </a>

          <!-- Telefon -->
          <a href="tel:+901234567890" class="contact-card reveal">
            <div class="card-icon">📱</div>
            <div class="card-content">
              <h3>Telefon</h3>
              <p>+90 123 456 78 90</p>
            </div>
          </a>

          <!-- Konum -->
          <div class="contact-card reveal">
            <div class="card-icon">📍</div>
            <div class="card-content">
              <h3>Konum</h3>
              <p>İstanbul, Türkiye</p>
            </div>
          </div>

          <!-- GitHub -->
          <a href="https://github.com/kadir" target="_blank" class="contact-card reveal">
            <div class="card-icon">
              <svg width="20" height="20" viewBox="0 0 24 24" fill="currentColor">
                <path d="M12 0c-6.626 0-12 5.373-12 12 0 5.302 3.438 9.8 8.207 11.387.599.111.793-.261.793-.577v-2.234c-3.338.726-4.033-1.416-4.033-1.416-.546-1.387-1.333-1.756-1.333-1.756-1.089-.745.083-.729.083-.729 1.205.084 1.839 1.237 1.839 1.237 1.07 1.834 2.807 1.304 3.492.997.107-.775.418-1.305.762-1.604-2.665-.305-5.467-1.334-5.467-5.931 0-1.311.469-2.381 1.236-3.221-.124-.303-.535-1.524.117-3.176 0 0 1.008-.322 3.301 1.23.957-.266 1.983-.399 3.003-.404 1.02.005 2.047.138 3.006.404 2.291-1.552 3.297-1.23 3.297-1.23.653 1.653.242 2.874.118 3.176.77.84 1.235 1.911 1.235 3.221 0 4.609-2.807 5.624-5.479 5.921.43.372.823 1.102.823 2.222v3.293c0 .319.192.694.801.576 4.765-1.589 8.199-6.086 8.199-11.386 0-6.627-5.373-12-12-12z"/>
              </svg>
            </div>
            <div class="card-content">
              <h3>GitHub</h3>
              <p>github.com/kadir</p>
            </div>
          </a>

          <!-- Sosyal medya -->
          <div class="social-links reveal">
            <a href="https://github.com/kadir" target="_blank" class="social-icon">
              <svg width="20" height="20" viewBox="0 0 24 24" fill="currentColor">
                <path d="M12 0c-6.626 0-12 5.373-12 12 0 5.302 3.438 9.8 8.207 11.387.599.111.793-.261.793-.577v-2.234c-3.338.726-4.033-1.416-4.033-1.416-.546-1.387-1.333-1.756-1.333-1.756-1.089-.745.083-.729.083-.729 1.205.084 1.839 1.237 1.839 1.237 1.07 1.834 2.807 1.304 3.492.997.107-.775.418-1.305.762-1.604-2.665-.305-5.467-1.334-5.467-5.931 0-1.311.469-2.381 1.236-3.221-.124-.303-.535-1.524.117-3.176 0 0 1.008-.322 3.301 1.23.957-.266 1.983-.399 3.003-.404 1.02.005 2.047.138 3.006.404 2.291-1.552 3.297-1.23 3.297-1.23.653 1.653.242 2.874.118 3.176.77.84 1.235 1.911 1.235 3.221 0 4.609-2.807 5.624-5.479 5.921.43.372.823 1.102.823 2.222v3.293c0 .319.192.694.801.576 4.765-1.589 8.199-6.086 8.199-11.386 0-6.627-5.373-12-12-12z"/>
              </svg>
            </a>
            <a href="https://linkedin.com/in/kadir" target="_blank" class="social-icon">
              <svg width="20" height="20" viewBox="0 0 24 24" fill="currentColor">
                <path d="M19 0h-14c-2.761 0-5 2.239-5 5v14c0 2.761 2.239 5 5 5h14c2.762 0 5-2.239 5-5v-14c0-2.761-2.238-5-5-5zm-11 19h-3v-11h3v11zm-1.5-12.268c-.966 0-1.75-.79-1.75-1.764s.784-1.764 1.75-1.764 1.75.79 1.75 1.764-.783 1.764-1.75 1.764zm13.5 12.268h-3v-5.604c0-3.368-4-3.113-4 0v5.604h-3v-11h3v1.765c1.396-2.586 7-2.777 7 2.476v6.759z"/>
              </svg>
            </a>
            <a href="https://twitter.com/kadir" target="_blank" class="social-icon">
              <svg width="20" height="20" viewBox="0 0 24 24" fill="currentColor">
                <path d="M18.244 2.25h3.308l-7.227 8.26 8.502 11.24H16.17l-5.214-6.817L4.99 21.75H1.68l7.73-8.835L1.254 2.25H8.08l4.713 6.231zm-1.161 17.52h1.833L7.084 4.126H5.117z"/>
              </svg>
            </a>
          </div>
        </div>

        <!-- Sağ taraf - Form -->
        <form class="contact-form reveal" @submit.prevent="handleSubmit">
          <div class="form-group">
            <label for="name">Ad Soyad</label>
            <input type="text" id="name" v-model="formData.name" placeholder="Adınızı girin" required>
          </div>

          <div class="form-group">
            <label for="email">E-posta</label>
            <input type="email" id="email" v-model="formData.email" placeholder="E-posta adresiniz" required>
          </div>

          <div class="form-group">
            <label for="subject">Konu</label>
            <input type="text" id="subject" v-model="formData.subject" placeholder="Mesajın konusu" required>
          </div>

          <div class="form-group">
            <label for="message">Mesaj</label>
            <textarea id="message" v-model="formData.message" placeholder="Mesajınızı yazın..." rows="5" required></textarea>
          </div>

          <button type="submit" class="btn btn-primary" style="width: 100%;">
            Gönder
          </button>
        </form>
      </div>
    </div>
  </section>
</template>

<script setup>
import { reactive } from 'vue'

const formData = reactive({
  name: '',
  email: '',
  subject: '',
  message: ''
})

const handleSubmit = () => {
  if (formData.name && formData.email && formData.subject && formData.message) {
    alert('Teşekkürler ' + formData.name + '! Mesajınız gönderildi. 🎉\n\nKonu: ' + formData.subject)
    // Reset form
    formData.name = ''
    formData.email = ''
    formData.subject = ''
    formData.message = ''
  } else {
    alert('Lütfen tüm alanları doldurun!')
  }
}
</script>

< style scoped>
.contact {
  position: relative;
  overflow: hidden;
}

/* Arka plan çizgiler */
.contact-bg-lines {
  position: absolute;
  inset: 0;
  pointer-events: none;
  overflow: hidden;
}

.line {
  position: absolute;
  top: 0;
  bottom: 0;
  width: 1px;
  background: linear-gradient(180deg, transparent, var(--accent-primary), transparent);
  opacity: 0.2;
  animation: flowLine 8s linear infinite;
}

.line:nth-child(1) { left: 10%; }
.line:nth-child(2) { left: 30%; animation-delay: -1s; }
.line:nth-child(3) { left: 50%; animation-delay: -2s; }
.line:nth-child(4) { left: 70%; animation-delay: -3s; }
.line:nth-child(5) { left: 90%; animation-delay: -4s; }

@keyframes flowLine {
  0% { transform: translateY(-100%); }
  100% { transform: translateY(100%); }
}

/* Grid */
.contact-grid {
  display: grid;
  grid-template-columns: 1fr 1.5fr;
  gap: 48px;
  align-items: start;
}

/* İletişim bilgileri */
.contact-info {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.contact-card {
  display: flex;
  align-items: center;
  gap: 16px;
  background: var(--bg-card);
  border: 1px solid var(--border);
  border-radius: var(--card-radius-sm);
  padding: 20px;
  text-decoration: none;
  color: inherit;
  transition: all var(--animation-speed);
}

.contact-card:hover {
  border-color: var(--accent-primary);
  transform: translateX(8px);
  background: var(--bg-card-hover);
}

.contact-card .card-icon {
  width: 48px;
  height: 48px;
  background: linear-gradient(135deg, var(--accent-primary), var(--accent-secondary));
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.25rem;
  flex-shrink: 0;
}

.contact-card .card-content h3 {
  font-size: 1rem;
  font-weight: 600;
  margin-bottom: 4px;
  color: var(--text-primary);
}

.contact-card .card-content p {
  font-size: 0.9rem;
  color: var(--text-muted);
}

/* Sosyal medya */
.social-links {
  display: flex;
  gap: 12px;
  margin-top: 8px;
}

.social-icon {
  width: 44px;
  height: 44px;
  background: var(--bg-card);
  border: 1px solid var(--border);
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: var(--text-muted);
  transition: all var(--animation-speed);
}

.social-icon:hover {
  background: var(--accent-primary);
  border-color: var(--accent-primary);
  color: #fff;
  transform: translateY(-4px);
  box-shadow: 0 8px 20px var(--accent-glow);
}

/* Form */
.contact-form {
  background: var(--bg-card);
  border: 1px solid var(--border);
  border-radius: var(--card-radius);
  padding: 32px;
}

