<template>
  <div class="login-page">
    <div class="login-card">
      
      <!-- Step 1: Login Form -->
      <div v-if="!require2Fa" class="fade-in">
        <div class="brand">
          <div class="brand-icon">
            <div class="circle"></div>
            <div class="square"></div>
          </div>
          <div class="brand-text">KADIR<span class="light">Admin</span></div>
        </div>
        
        <h1 class="title">Yönetim Paneline<br>Giriş</h1>

        <form @submit.prevent="handleLogin" class="form">
          <!-- Honeypot -->
          <input type="text" v-model="form.usernameHoneypot" style="display:none" autocomplete="off" />

          <!-- Email Input -->
          <div class="input-container" :class="{ focused: focusedField === 'email' }">
            <i class="ph ph-user"></i>
            <input 
              type="email" 
              v-model="form.email" 
              placeholder="Username / E-mail" 
              @focus="focusedField = 'email'"
              @blur="focusedField = null"
              required 
            />
          </div>

          <!-- Password Input -->
          <div class="input-container" :class="{ focused: focusedField === 'password' }">
            <i class="ph ph-key"></i>
            <input 
              :type="showPassword ? 'text' : 'password'" 
              v-model="form.password" 
              placeholder="Password" 
              @focus="focusedField = 'password'"
              @blur="focusedField = null"
              required 
            />
            <button type="button" class="toggle-password" @click="showPassword = !showPassword">
              <i class="ph ph-eye"></i> show
            </button>
          </div>

          <!-- Beni Hatırla -->
          <div class="remember-me-container">
            <label class="custom-checkbox">
              <input type="checkbox" v-model="form.rememberMe" />
              <span class="checkmark">
                <i class="ph ph-check" v-if="form.rememberMe"></i>
              </span>
              <span class="label-text">Beni Hatırla</span>
            </label>
          </div>

          <!-- Captcha Container -->
          <div class="captcha-wrapper">
            <div id="captcha-container"></div>
          </div>

          <button type="submit" class="submit-button" :disabled="isLoading || !isCaptchaChecked">
            {{ isLoading ? 'BEKLEYIN...' : 'SİSTEME GİRİŞ' }} <i v-if="!isLoading" class="ph ph-arrow-up-right"></i>
          </button>

          <!-- Alt linkler kaldırıldı -->
        </form>
      </div>

      <!-- Step 2: 2FA Form -->
      <div v-else class="fade-in">
        <div class="brand" style="margin-bottom: 15px;">
          <div class="brand-icon">
            <i class="ph ph-shield-check" style="color: #fff; font-size: 24px;"></i>
          </div>
        </div>
        
        <h1 class="title" style="margin-bottom: 10px;">Güvenlik Onayı</h1>
        <p style="text-align: center; color: #9CA3AF; margin-bottom: 25px; font-size: 14px;">E-postanıza gönderilen 6 haneli kodu girin.</p>

        <div class="countdown-display">{{ formattedTime }}</div>

        <form @submit.prevent="handle2Fa" class="form">
          <div class="otp-boxes">
            <input 
              v-for="(val, index) in 6" 
              :key="index"
              type="text"
              maxlength="1"
              class="otp-box"
              v-model="form.otp[index]"
              @input="focusNext(index, $event)"
              @keydown.delete="focusPrev(index, $event)"
              :ref="el => { if(el) otpRefs[index] = el }"
            />
          </div>

          <button type="submit" class="submit-button" :disabled="isVerifying || otpString.length < 6">
            {{ isVerifying ? 'DOĞRULANIYOR...' : 'ONAYLA' }} <i v-if="!isVerifying" class="ph ph-check"></i>
          </button>
          
          <div class="footer-links" style="margin-top: 15px;">
            <a href="#" @click.prevent="require2Fa = false">İptal Et ve Geri Dön</a>
          </div>
        </form>
      </div>

      <!-- Error Alert -->
      <transition name="fade">
        <div v-if="error" class="error-alert">
          <i class="ph ph-warning-circle"></i> {{ error }}
        </div>
      </transition>

    </div>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onUnmounted, nextTick, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import api from '../../services/api';

const router = useRouter();
const isLoading = ref(false);
const isVerifying = ref(false);
const error = ref('');
const require2Fa = ref(false);
const isCaptchaChecked = ref(false);
const focusedField = ref(null);
const showPassword = ref(false);

const form = reactive({
  email: '',
  password: '',
  usernameHoneypot: '',
  captchaToken: '',
  rememberMe: false,
  otp: ['', '', '', '', '', '']
});

window.onCaptchaSuccess = (token) => {
  form.captchaToken = token;
  isCaptchaChecked.value = true;
};

let captchaWidgetId = null;

onMounted(() => {
  // Yandex SmartCaptcha'nın yüklenmesini bekle ve manuel olarak render et (Sayfa yenileme sorununu çözer)
  const initCaptcha = () => {
    if (window.smartCaptcha) {
      captchaWidgetId = window.smartCaptcha.render('captcha-container', {
        sitekey: 'ysc1_kHFlfZAwmYFwg96y9TxaLmUKnMTSI0QnBZeVi6SN336e35dc',
        callback: window.onCaptchaSuccess,
      });
    } else {
      setTimeout(initCaptcha, 200); // 200ms aralıklarla scriptin yüklenmesini bekle
    }
  };
  
  // Eğer kullanıcı 2FA ekranında değilse (Yani Captcha ekranındaysa) render et
  if (!require2Fa.value) {
    initCaptcha();
  }
});

const otpRefs = ref([]);
const otpString = computed(() => form.otp.join(''));

let timer = null;
const timeLeft = ref(180);

const formattedTime = computed(() => {
  const m = Math.floor(timeLeft.value / 60).toString().padStart(2, '0');
  const s = (timeLeft.value % 60).toString().padStart(2, '0');
  return `${m}:${s}`;
});

const startTimer = () => {
  timeLeft.value = 180;
  if (timer) clearInterval(timer);
  timer = setInterval(() => {
    if (timeLeft.value > 0) {
      timeLeft.value--;
    } else {
      clearInterval(timer);
      error.value = "2FA kodunun süresi doldu. Lütfen tekrar giriş yapın.";
      require2Fa.value = false;
    }
  }, 1000);
};

onUnmounted(() => {
  if (timer) clearInterval(timer);
});

const focusNext = (index, event) => {
  if (event.target.value && index < 5) {
    nextTick(() => {
      otpRefs.value[index + 1]?.focus();
    });
  }
};

const focusPrev = (index, event) => {
  if (!event.target.value && index > 0) {
    nextTick(() => {
      otpRefs.value[index - 1]?.focus();
    });
  }
};

const handleLogin = async () => {
  if (form.usernameHoneypot) {
    error.value = "Bot tespiti! İstek reddedildi.";
    return;
  }

  error.value = '';
  isLoading.value = true;

  try {
    const response = await api.post('/auth/login', form);
    if (response.data.success) {
      if (response.data.require2Fa) {
        require2Fa.value = true;
        startTimer();
      } else if (response.data.token) {
        localStorage.setItem('token', response.data.token);
        localStorage.setItem('rememberMe', form.rememberMe);
        const expiryMs = form.rememberMe ? (24 * 60 * 60 * 1000) : (2 * 60 * 60 * 1000);
        localStorage.setItem('sessionExpiresAt', Date.now() + expiryMs);
        router.push('/admin');
      }
    }
  } catch (err) {
    error.value = err.response?.data?.message || 'Giriş işlemi başarısız.';
  } finally {
    isLoading.value = false;
  }
};

const handle2Fa = async () => {
  error.value = '';
  isVerifying.value = true;

  try {
    const response = await api.post('/auth/verify-2fa', {
      email: form.email,
      code: otpString.value,
      rememberMe: form.rememberMe
    });

    if (response.data.success) {
      localStorage.setItem('token', response.data.token);
      localStorage.setItem('rememberMe', form.rememberMe);
      const expiryMs = form.rememberMe ? (24 * 60 * 60 * 1000) : (2 * 60 * 60 * 1000);
      localStorage.setItem('sessionExpiresAt', Date.now() + expiryMs);
      router.push('/admin');
    }
  } catch (err) {
    error.value = err.response?.data?.message || '2FA doğrulaması başarısız.';
  } finally {
    isVerifying.value = false;
  }
};
</script>

<style scoped>

.login-page {
  min-height: 100vh;
  width: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  background-color: #161921; /* Koyu slate gri */
  background-image: 
    radial-gradient(circle at 10% 20%, rgba(255, 255, 255, 0.03) 0%, transparent 40%),
    radial-gradient(circle at 90% 80%, rgba(255, 255, 255, 0.02) 0%, transparent 40%),
    radial-gradient(circle at 50% 50%, rgba(255, 107, 0, 0.01) 0%, transparent 60%);
  font-family: 'Inter', sans-serif;
  padding: 20px;
  box-sizing: border-box;
}

.login-card {
  width: 100%;
  max-width: 480px; /* Genişletildi */
  background: rgba(255, 255, 255, 0.05); /* Şeffaf beyaz cam */
  backdrop-filter: blur(24px);
  -webkit-backdrop-filter: blur(24px);
  border: 1px solid rgba(255, 255, 255, 0.1); /* İnce beyaz kenarlık */
  border-radius: 20px;
  padding: 45px 50px;
  box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.5);
  box-sizing: border-box;
  position: relative;
}

.fade-in {
  animation: fadeUp 0.5s ease forwards;
}

@keyframes fadeUp {
  from { opacity: 0; transform: translateY(10px); }
  to { opacity: 1; transform: translateY(0); }
}

.brand {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 10px;
  margin-bottom: 25px;
}

.brand-icon {
  position: relative;
  width: 24px;
  height: 24px;
}
.brand-icon .circle {
  width: 15px;
  height: 15px;
  background: #fff;
  border-radius: 50%;
  position: absolute;
  top: 0;
  right: 0;
}
.brand-icon .square {
  width: 14px;
  height: 14px;
  background: #fff;
  border-radius: 3px;
  position: absolute;
  bottom: 0;
  left: 0;
}

.brand-text {
  color: #fff;
  font-weight: 600;
  font-size: 1.3rem;
  letter-spacing: 0.5px;
}
.brand-text .light {
  font-weight: 300;
  margin-left: 5px;
}

.title {
  text-align: center;
  color: #fff;
  font-size: 2.2rem;
  font-weight: 500;
  line-height: 1.2;
  margin-bottom: 35px;
  letter-spacing: -0.5px;
}

.form {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

/* Tam yuvarlak form kutuları */
.input-container {
  display: flex;
  align-items: center;
  background: rgba(0, 0, 0, 0.2); /* Koyu iç zemin */
  border: 1px solid rgba(255, 255, 255, 0.1); /* Gri kenarlık */
  border-radius: 50px; /* Hap şeklinde */
  height: 56px;
  padding: 0 20px;
  transition: all 0.3s ease;
  position: relative;
}

.input-container.focused {
  border-color: #FF6B00; /* Turuncu odak kenarlığı */
  background: rgba(0, 0, 0, 0.3);
}

.input-container i.ph-user, .input-container i.ph-key {
  color: #9CA3AF;
  font-size: 20px;
  margin-right: 12px;
  transition: color 0.3s ease;
}
.input-container.focused i.ph-user, .input-container.focused i.ph-key {
  color: #FF6B00; /* Aktif ikon rengi */
}

.input-container input {
  flex: 1;
  background: transparent !important;
  border: none;
  color: #fff;
  font-size: 16px;
  outline: none;
  height: 100%;
  width: 100%;
}
.input-container input::placeholder {
  color: #6B7280;
}

/* Chrome autofill (Sarı veya beyaz background'u ezmek için) */
.input-container input:-webkit-autofill,
.input-container input:-webkit-autofill:hover, 
.input-container input:-webkit-autofill:focus, 
.input-container input:-webkit-autofill:active {
  -webkit-box-shadow: 0 0 0 50px #1a1e26 inset !important; /* Arka plana uygun koyu renk */
  -webkit-text-fill-color: white !important;
  transition: background-color 5000s ease-in-out 0s;
}

.toggle-password {
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 30px;
  color: #9CA3AF;
  padding: 5px 12px;
  font-size: 13px;
  display: flex;
  align-items: center;
  gap: 6px;
  cursor: pointer;
  transition: all 0.2s;
  outline: none;
}
.toggle-password:hover {
  background: rgba(255, 255, 255, 0.1);
  color: #fff;
}

/* Beni Hatırla Checkbox */
.remember-me-container {
  display: flex;
  align-items: center;
  margin-top: 5px;
  margin-bottom: 15px;
  padding-left: 5px;
}
.custom-checkbox {
  display: flex;
  align-items: center;
  gap: 10px;
  cursor: pointer;
  user-select: none;
}
.custom-checkbox input {
  display: none;
}
.checkmark {
  width: 20px;
  height: 20px;
  background: rgba(0, 0, 0, 0.2);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 6px;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s;
  color: #FF6B00;
  font-size: 14px;
}
.custom-checkbox input:checked + .checkmark {
  border-color: #FF6B00;
  background: rgba(255, 107, 0, 0.1);
}
.label-text {
  color: #9CA3AF;
  font-size: 14px;
  transition: color 0.2s;
}
.custom-checkbox:hover .label-text {
  color: #fff;
}

/* Captcha Ortalama */
.captcha-wrapper {
  display: flex;
  justify-content: center;
  margin-top: 5px;
  margin-bottom: 5px;
}

.submit-button {
  background: #FF6B00; /* Daima Turuncu */
  color: #fff;
  border: none;
  border-radius: 50px; /* Hap şeklinde */
  height: 56px;
  font-size: 16px;
  font-weight: 600;
  text-transform: uppercase;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  transition: all 0.3s ease;
  margin-top: 5px;
}
.submit-button:disabled {
  opacity: 0.5; /* Buton grileşmez, sadece soluklaşır */
  cursor: not-allowed;
}
.submit-button:not(:disabled):hover {
  background: #ff7a1a;
  transform: translateY(-2px);
  box-shadow: 0 10px 20px rgba(255, 107, 0, 0.3);
}

.footer-links {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 15px;
  margin-top: 15px;
}
.footer-links a {
  color: #9CA3AF;
  text-decoration: none;
  font-size: 14px;
  transition: color 0.2s;
}
.footer-links a:hover {
  color: #fff;
}
.footer-links .divider {
  color: #4B5563;
  font-size: 14px;
}

/* 2FA Kutucukları */
.countdown-display {
  text-align: center;
  font-size: 2.5rem;
  font-weight: 300;
  color: #fff;
  margin-bottom: 25px;
  font-variant-numeric: tabular-nums;
}

.otp-boxes {
  display: flex;
  justify-content: space-between;
  gap: 8px;
  margin-bottom: 20px;
}

.otp-box {
  width: 50px;
  height: 60px;
  background: rgba(0, 0, 0, 0.2);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 12px;
  text-align: center;
  color: #fff;
  font-size: 20px;
  font-weight: 500;
  outline: none;
  transition: all 0.2s;
}
.otp-box:focus {
  border-color: #FF6B00;
  background: rgba(0, 0, 0, 0.4);
}

.error-alert {
  margin-top: 20px;
  padding: 15px;
  background: rgba(239, 68, 68, 0.1);
  border: 1px solid rgba(239, 68, 68, 0.2);
  border-radius: 12px;
  color: #FCA5A5;
  font-size: 14px;
  display: flex;
  align-items: center;
  gap: 8px;
}

/* Mobil Uyumluluk */
@media (max-width: 768px) {
  .login-card {
    padding: 35px 25px;
  }
  .title {
    font-size: 1.8rem;
  }
  .otp-box {
    width: 42px;
    height: 52px;
    font-size: 18px;
  }
}
@media (max-width: 400px) {
  .otp-box {
    width: 38px;
    height: 48px;
  }
}
</style>
