/**
 * =====================================================
 * VUE.JS ANİMASYON SİSTEMİ
 * Her sayfa için özel animasyonlar
 * 
 * Sayfa: index.html - Ateş Partikül Efekti + Mouse Parallax
 * Sayfa: hakkinda.html - Yıldız Efekti + Floating Particles  
 * Sayfa: projects.html - Yükselen Alev Baloncukları
 * Sayfa: contact.html - Dikey Işık Çizgileri
 * Sayfa: yetenekler.html - Floating Particles + Skill Bars
 * =====================================================
 */

// Vue.js'in sayfada yüklü olup olmadığını kontrol ediyoruz.
// Blog ve projects sayfalarında Vue yüklü değil, o yüzden bu koruma gerekli.
// Eğer Vue yoksa boş bir obje kullan, böylece hata vermez.
const VueLib = typeof Vue !== 'undefined' ? Vue : {};
const { createApp, ref, onMounted, onUnmounted, computed } = VueLib;

// ===== ANA ANİMASYON FABRİKASI =====
// Her sayfa için farklı animasyon tipleri oluşturur
class AnimationFactory {
  
  /**
   * ATEŞ PARÇACIKLARI ANİMASYONU (index.html)
   * ----------------------------------------
   * Nasıl çalışır:
   * 1. Canvas üzerinde rastgele partiküller oluşturulur
   * 2. Her partikül yukarı doğru hareket eder (ateş efekti)
   * 3. Renkler rastgele seçilir: turuncu, kırmızı-turuncu, sarı-turuncu
   * 4. Partiküller yukarı çıktıkça yok olur ve yeniden aşağıdan doğar
   * 5. globalCompositeOperation = 'lighter' ile renkler birbirine karışarak parlaklık efekti oluşturur
   */
  static fireParticles(canvasId) {
    const canvas = document.getElementById(canvasId);
    if (!canvas) return () => {};
    
    const ctx = canvas.getContext('2d');
    
    // Canvas boyutunu ayarla
    const resize = () => {
      canvas.width = window.innerWidth;
      canvas.height = window.innerHeight;
    };
    resize();
    window.addEventListener('resize', resize);
    
    // Ateş renk paleti
    const colors = [
      [255, 140, 0],   // Turuncu
      [255, 80, 0],    // Kırmızı-turuncu
      [255, 200, 50]   // Sarı-turuncu
    ];
    
    // Partiküller dizisi
    const particles = [];
    const particleCount = 60;
    
    // Her partikül için özellikler
    const createParticle = (initial = false) => ({
      x: Math.random() * canvas.width,
      y: initial ? Math.random() * canvas.height : canvas.height + Math.random() * 50,
      size: Math.random() * 2.5 + 0.8,
      speedX: (Math.random() - 0.5) * 0.6,
      speedY: Math.random() * -2 - 0.5,
      colorType: Math.floor(Math.random() * 3),
      opacity: Math.random() * 0.5 + 0.3,
      life: Math.random() * 120 + 80,
      maxLife: 0,
      wobble: Math.random() * Math.PI * 2,
      wobbleSpeed: Math.random() * 0.02 + 0.015
    });
    
    // Başlangıç partiküllerini oluştur
    for (let i = 0; i < particleCount; i++) {
      const p = createParticle(true);
      p.maxLife = p.life;
      particles.push(p);
    }
    
    // Animasyon döngüsü
    const animate = () => {
      // Canvas'ı yarı saydam siyah ile boyayarak "iz" efekti oluştur
      ctx.fillStyle = 'rgba(10, 10, 12, 0.18)';
      ctx.fillRect(0, 0, canvas.width, canvas.height);
      
      // Karıştırma modu - ateş efekti için önemli!
      ctx.globalCompositeOperation = 'lighter';
      
      particles.forEach((p, i) => {
        // Hareket hesaplamaları
        p.wobble += p.wobbleSpeed;
        p.x += p.speedX + Math.sin(p.wobble) * 0.4;
        p.y += p.speedY;
        p.life--;
        p.opacity = (p.life / p.maxLife) * 0.7;
        p.size *= 0.997;
        
        // Ekrandan çıkınca yeniden doğur
        if (p.y < -20 || p.life <= 0) {
          particles[i] = createParticle(false);
          particles[i].maxLife = particles[i].life;
          return;
        }
        
        // Partikülü çiz
        const c = colors[p.colorType];
        ctx.beginPath();
        ctx.arc(p.x, p.y, Math.max(0.5, p.size), 0, Math.PI * 2);
        ctx.fillStyle = `rgba(${c[0]}, ${c[1]}, ${c[2]}, ${p.opacity})`;
        ctx.fill();
      });
      
      ctx.globalCompositeOperation = 'source-over';
      requestAnimationFrame(animate);
    };
    
    animate();
    
    // Temizlik fonksiyonu
    return () => {
      window.removeEventListener('resize', resize);
    };
  }
  
  /**
   * YILDIZ EFEKTİ (hakkinda.html)
   * ------------------------------
   * Nasıl çalışır:
   * 1. Arka planda yıldız benzeri küçük noktalar oluşturulur
   * 2. Yıldızlar farklı hızlarla yukarı doğru yüzer
   * 3. Her yıldızın parlaklık değeri (opacity) sürekli değişir (twinkle efekti)
   * 4. Renkler ateş temasına uygun: kırmızı, turuncu, sarı, beyaz
   * 5. Bazı yıldızların etrafında ışık halesi (glow) oluşturulur
   */
  static starParticles(canvasId) {
    const canvas = document.getElementById(canvasId);
    if (!canvas) return () => {};
    
    const ctx = canvas.getContext('2d');
    
    const resize = () => {
      canvas.width = window.innerWidth;
      canvas.height = window.innerHeight;
    };
    resize();
    window.addEventListener('resize', resize);
    
    // Yıldız sınıfı
    class Star {
      constructor() {
        this.reset();
        this.y = Math.random() * canvas.height;
      }
      
      reset() {
        this.x = Math.random() * canvas.width;
        this.y = canvas.height + 10;
        this.size = Math.random() * 2 + 0.5;
        this.speedY = -(Math.random() * 0.5 + 0.2);
        this.speedX = (Math.random() - 0.5) * 0.3;
        this.opacity = 0;
        this.maxOpacity = Math.random() * 0.6 + 0.2;
        this.twinkleSpeed = Math.random() * 0.02 + 0.01;
        this.twinklePhase = Math.random() * Math.PI * 2;
        
        // Ateş renkleri
        const colors = [
          [255, 77, 0],   // Ateş kırmızısı
          [255, 107, 53], // Turuncu
          [255, 165, 0],  // Sarı-turuncu
          [255, 255, 255] // Beyaz
        ];
        this.color = colors[Math.floor(Math.random() * colors.length)];
      }
      
      update() {
        this.y += this.speedY;
        this.x += this.speedX;
        this.twinklePhase += this.twinkleSpeed;
        this.opacity = this.maxOpacity * (0.5 + 0.5 * Math.sin(this.twinklePhase));
        
        if (this.y < -10 || this.x < -10 || this.x > canvas.width + 10) {
          this.reset();
        }
      }
      
      draw() {
        ctx.beginPath();
        ctx.arc(this.x, this.y, this.size, 0, Math.PI * 2);
        ctx.fillStyle = `rgba(${this.color[0]}, ${this.color[1]}, ${this.color[2]}, ${this.opacity})`;
        ctx.fill();
        
        // Işık halesi
        if (this.size > 1.2) {
          const gradient = ctx.createRadialGradient(
            this.x, this.y, 0,
            this.x, this.y, this.size * 4
          );
          gradient.addColorStop(0, `rgba(${this.color[0]}, ${this.color[1]}, ${this.color[2]}, ${this.opacity * 0.3})`);
          gradient.addColorStop(1, 'transparent');
          ctx.fillStyle = gradient;
          ctx.beginPath();
          ctx.arc(this.x, this.y, this.size * 4, 0, Math.PI * 2);
          ctx.fill();
        }
      }
    }
    
    const stars = [];
    for (let i = 0; i < 60; i++) {
      stars.push(new Star());
    }
    
    const animate = () => {
      ctx.clearRect(0, 0, canvas.width, canvas.height);
      
      const bgGradient = ctx.createRadialGradient(
        canvas.width / 2, canvas.height / 2, 0,
        canvas.width / 2, canvas.height / 2, canvas.width
      );
      bgGradient.addColorStop(0, 'rgba(10, 10, 12, 0)');
      bgGradient.addColorStop(1, 'rgba(10, 10, 12, 0.3)');
      ctx.fillStyle = bgGradient;
      ctx.fillRect(0, 0, canvas.width, canvas.height);
      
      stars.forEach(star => {
        star.update();
        star.draw();
      });
      
      requestAnimationFrame(animate);
    };
    
    animate();
    
    return () => {
      window.removeEventListener('resize', resize);
    };
  }
  
  /**
   * YÜKSELEN ALEV BALONCUKLARI (projects.html)
   * --------------------------------------------
   * Nasıl çalışır:
   * 1. Proje sayfası için özel efekt - yükselen baloncuklar
   * 2. Her baloncuk farklı hızla yukarı doğru hareket eder
   * 3. Sinüs dalgası ile yatay sallantı hareketi eklenir
   * 4. Renkler: kırmızı, turuncu, sarı (ateş paleti)
   */
  static risingBubbles(canvasId) {
    const canvas = document.getElementById(canvasId);
    if (!canvas) return () => {};
    
    const ctx = canvas.getContext('2d');
    
    const resize = () => {
      canvas.width = window.innerWidth;
      canvas.height = window.innerHeight;
    };
    resize();
    window.addEventListener('resize', resize);
    
    const particles = [];
    const colors = [
      [255, 59, 29],
      [255, 120, 0],
      [255, 176, 0],
    ];
    
    for (let i = 0; i < 40; i++) {
      const life = Math.random() * 80 + 40;
      particles.push({
        x: Math.random() * canvas.width,
        y: Math.random() * canvas.height,
        size: Math.random() * 3 + 1,
        speedX: (Math.random() - 0.5) * 0.4,
        speedY: -(Math.random() * 1 + 0.5),
        color: colors[Math.floor(Math.random() * colors.length)],
        opacity: Math.random() * 0.3 + 0.1,
        life: life,
        maxLife: life
      });
    }
    
    const animate = () => {
      ctx.clearRect(0, 0, canvas.width, canvas.height);
      ctx.globalCompositeOperation = 'lighter';
      
      particles.forEach((p, i) => {
        p.x += p.speedX + Math.sin(p.life * 0.05) * 0.2;
        p.y += p.speedY;
        p.life--;
        
        if (p.y < -10 || p.life <= 0) {
          particles[i] = {
            x: Math.random() * canvas.width,
            y: canvas.height + 10,
            size: Math.random() * 3 + 1,
            speedX: (Math.random() - 0.5) * 0.4,
            speedY: -(Math.random() * 1 + 0.5),
            color: colors[Math.floor(Math.random() * colors.length)],
            opacity: Math.random() * 0.3 + 0.1,
            life: Math.random() * 80 + 40,
            maxLife: 0
          };
          particles[i].maxLife = particles[i].life;
          return;
        }
        
        const alpha = (p.life / p.maxLife) * p.opacity;
        const [r, g, b] = p.color;
        
        ctx.beginPath();
        ctx.arc(p.x, p.y, p.size, 0, Math.PI * 2);
        ctx.fillStyle = `rgba(${r}, ${g}, ${b}, ${alpha})`;
        ctx.fill();
      });
      
      ctx.globalCompositeOperation = 'source-over';
      requestAnimationFrame(animate);
    };
    
    animate();
    
    return () => {
      window.removeEventListener('resize', resize);
    };
  }
  
  /**
   * DİKEY IŞIK ÇİZGİLERİ (contact.html)
   * -------------------------------------
   * Nasıl çalışır:
   * 1. Ekranda dikey çizgiler oluşturulur
   * 2. Her çizgi farklı hızla yukarı doğru akar
   * 3. Çizgilerin opaklığı rastgele değişir
   * 4. Sinüs dalgası ile hafif yatay sallantı eklenir
   * 5. Gradient renkler ile ışık efekti oluşturulur
   */
  static verticalLines(canvasId) {
    const canvas = document.getElementById(canvasId);
    if (!canvas) return () => {};
    
    const ctx = canvas.getContext('2d');
    
    const resize = () => {
      canvas.width = window.innerWidth;
      canvas.height = window.innerHeight;
    };
    resize();
    window.addEventListener('resize', resize);
    
    const lines = [];
    for (let i = 0; i < 8; i++) {
      lines.push({
        x: Math.random() * canvas.width,
        speedY: Math.random() * 2 + 1,
        length: Math.random() * 100 + 50,
        opacity: Math.random() * 0.3 + 0.1
      });
    }
    
    const animate = () => {
      ctx.clearRect(0, 0, canvas.width, canvas.height);
      ctx.globalCompositeOperation = 'lighter';
      
      lines.forEach(line => {
        line.x += Math.sin(Date.now() * 0.001) * 0.2;
        
        const gradient = ctx.createLinearGradient(0, 0, 0, canvas.height);
        gradient.addColorStop(0, 'transparent');
        gradient.addColorStop(0.5, `rgba(255, 100, 0, ${line.opacity})`);
        gradient.addColorStop(1, 'transparent');
        
        ctx.strokeStyle = gradient;
        ctx.lineWidth = 1;
        ctx.beginPath();
        ctx.moveTo(line.x, 0);
        ctx.lineTo(line.x + 20, canvas.height);
        ctx.stroke();
      });
      
      ctx.globalCompositeOperation = 'source-over';
      requestAnimationFrame(animate);
    };
    
    animate();
    
    return () => {
      window.removeEventListener('resize', resize);
    };
  }
  
  /**
   * YÜZEN PARÇACIKLAR (yetenekler.html)
   * ------------------------------------
   * Nasıl çalışır:
   * 1. Arka planda yüzen küçük parçacıklar
   * 2. Hem yukarı hem yatay hareket
   * 3. Renk paleti ateş temalı
   * 4. Mouse takibi ile etkileşim (parallax benzeri)
   */
  static floatingParticles(canvasId) {
    const canvas = document.getElementById(canvasId);
    if (!canvas) return () => {};
    
    const ctx = canvas.getContext('2d');
    
    const resize = () => {
      canvas.width = window.innerWidth;
      canvas.height = window.innerHeight;
    };
    resize();
    window.addEventListener('resize', resize);
    
    const particles = [];
    const colors = [
      [255, 59, 29],
      [255, 140, 0],
      [255, 176, 0]
    ];
    
    for (let i = 0; i < 50; i++) {
      particles.push({
        x: Math.random() * canvas.width,
        y: Math.random() * canvas.height,
        size: Math.random() * 2 + 0.5,
        speedX: (Math.random() - 0.5) * 0.5,
        speedY: -(Math.random() * 1 + 0.3),
        color: colors[Math.floor(Math.random() * colors.length)],
        opacity: Math.random() * 0.4 + 0.1
      });
    }
    
    const animate = () => {
      ctx.clearRect(0, 0, canvas.width, canvas.height);
      ctx.globalCompositeOperation = 'lighter';
      
      particles.forEach(p => {
        p.x += p.speedX;
        p.y += p.speedY;
        
        // Ekrandan çıkınca yeniden doğur
        if (p.y < -10 || p.x < 0 || p.x > canvas.width) {
          p.x = Math.random() * canvas.width;
          p.y = canvas.height + 10;
        }
        
        ctx.beginPath();
        ctx.arc(p.x, p.y, p.size, 0, Math.PI * 2);
        ctx.fillStyle = `rgba(${p.color[0]}, ${p.color[1]}, ${p.color[2]}, ${p.opacity})`;
        ctx.fill();
      });
      
      ctx.globalCompositeOperation = 'source-over';
      requestAnimationFrame(animate);
    };
    
    animate();
    
    return () => {
      window.removeEventListener('resize', resize);
    };
  }
}

// ===== MOUSE PARALLAX EFEKTİ =====
/**
 * MOUSE PARALLAX (index.html)
 * ---------------------------
 * Nasıl çalışır:
 * 1. Kullanıcı mouse'u hareket ettirdiğinde
 * 2. Profil fotoğrafı ve çerçevesi hafifçe döner (3D efekt)
 * 3. Fare hareketine bağlı olarak X ve Y ekseninde kaydırma
 * 4. Throttle fonksiyonu ile performans optimize edilir
 * 5. Fare ekrandan çıktığında normal pozisyona geri döner
 */
function initMouseParallax() {
  const profileImg = document.getElementById('profile-img');
  const imageGlow = document.getElementById('image-glow');
  const imageFrame = document.querySelector('.image-frame');
  
  // Throttle fonksiyonu - çok sık tetiklenmeyi önler
  function throttle(func, limit) {
    let inThrottle;
    return function(...args) {
      if (!inThrottle) {
        func.apply(this, args);
        inThrottle = true;
        setTimeout(() => inThrottle = false, limit);
      }
    };
  }
  
  const handleMouseMove = throttle((e) => {
    const mouseX = (e.clientX / window.innerWidth) * 2 - 1;
    const mouseY = (e.clientY / window.innerHeight) * 2 - 1;
    
    if (imageFrame) {
      imageFrame.style.transform = 
        `perspective(1000px) rotateY(${mouseX * 8}deg) rotateX(${-mouseY * 5}deg) translateX(${mouseX * 15}px) translateY(${mouseY * 10}px)`;
    }
    
    if (imageGlow) {
      imageGlow.style.transform = `translate(${mouseX * 20}px, ${mouseY * 15}px)`;
    }
    
    if (profileImg) {
      profileImg.style.transform = `translate(${mouseX * -10}px, ${mouseY * -8}px) scale(1.02)`;
    }
  }, 16); // ~60fps
  
  document.addEventListener('mousemove', handleMouseMove);
  
  document.addEventListener('mouseleave', () => {
    if (imageFrame) imageFrame.style.transform = 'perspective(1000px) rotateY(-5deg)';
    if (imageGlow) imageGlow.style.transform = 'translate(0, 0)';
    if (profileImg) profileImg.style.transform = 'translate(0, 0) scale(1)';
  });
  
  return () => {
    document.removeEventListener('mousemove', handleMouseMove);
    document.removeEventListener('mouseleave', () => {});
  };
}

// ===== FADE-IN ANİMASYONU =====
/**
 * FADE-IN EFEKTİ (Tüm sayfalar)
 * -----------------------------
 * Nasıl çalışır:
 * 1. IntersectionObserver API kullanılır
 * 2. Eleman ekrana görünür olduğunda .visible class'ı eklenir
 * 3. CSS transition ile yumuşak fade-in efekti
 * 4. Her eleman için farklı gecikme (staggered animation)
 */
function initFadeIn() {
  const observer = new IntersectionObserver((entries) => {
    entries.forEach(entry => {
      if (entry.isIntersecting) {
        entry.target.classList.add('visible');
        
        // Yetenek çubukları için özel animasyon
        const skillBars = entry.target.querySelectorAll('.skill-progress');
        skillBars.forEach(bar => {
          const targetWidth = bar.getAttribute('data-width');
          setTimeout(() => {
            bar.style.width = targetWidth + '%';
          }, 300);
        });
        
        observer.unobserve(entry.target);
      }
    });
  }, { threshold: 0.1 });
  
  document.querySelectorAll('.fade-in').forEach(el => {
    observer.observe(el);
  });
  
  return () => observer.disconnect();
}

// ===== NAVBAR SCROLL EFEKTİ =====
/**
 * NAVBAR SCROLL EFEKTİ
 * ---------------------
 * Nasıl çalışır:
 * 1. Kullanıcı sayfayı kaydırdığında
 * 2. Navbar arka planı daha opak hale gelir
 * 3. Hafif gölge eklenir
 */
function initNavbarScroll() {
  const navbar = document.querySelector('.navbar');
  if (!navbar) return () => {};
  
  const handleScroll = () => {
    if (window.scrollY > 50) {
      navbar.style.background = 'rgba(10, 10, 12, 0.98)';
      navbar.style.boxShadow = '0 2px 30px rgba(255, 77, 0, 0.1)';
    } else {
      navbar.style.background = 'rgba(10, 10, 12, 0.9)';
      navbar.style.boxShadow = 'none';
    }
  };
  
  window.addEventListener('scroll', handleScroll, { passive: true });
  
  return () => {
    window.removeEventListener('scroll', handleScroll);
  };
}

// ===== SKILL BAR ANİMASYONU =====
/**
 * YETENEK ÇUBUĞU ANİMASYONU (yetenekler.html)
 * --------------------------------------------
 * Nasıl çalışır:
 * 1. Sayfa yüklendiğinde tüm progress barları 0 genişliğinde
 * 2. Kullanıcı yetenekler bölümünü görüntülediğinde
 * 3. Her çubuk yumuşak bir geçişle %100 genişliğine ulaşır
 * 4. CSS transition ile cubic-bezier easing kullanılır
 * 5. Her çubuk için farklı gecikme süresi
 */
function initSkillBars() {
  const skillsSection = document.querySelector('.skills-container');
  if (!skillsSection) return () => {};
  
  const handleScroll = () => {
    const rect = skillsSection.getBoundingClientRect();
    if (rect.top < window.innerHeight * 0.8) {
      document.querySelectorAll('.skill-progress').forEach(bar => {
        const width = bar.getAttribute('data-width');
        if (bar.style.width !== width + '%') {
          setTimeout(() => {
            bar.style.width = width + '%';
          }, 100);
        }
      });
      window.removeEventListener('scroll', handleScroll);
    }
  };
  
  window.addEventListener('scroll', handleScroll);
  handleScroll(); // İlk yüklemede kontrol
  
  return () => {
    window.removeEventListener('scroll', handleScroll);
  };
}

// ===== ANA UYGULAMA OBJESİ =====
// Her sayfanın hangi animasyonlara ihtiyacı olduğunu buradan yönetiyoruz.
// cleanups dizisi → sayfa kapatıldığında temizlik yapabilmek için
// her fonksiyon bir "temizleyici" (cleanup) fonksiyonu döndürür.
const AnimationApp = {
  // Index sayfası: Ateş + Mouse parallax + Fade-in + Navbar scroll
  initIndex() {
    const cleanups = [];
    cleanups.push(AnimationFactory.fireParticles('fire-canvas'));
    cleanups.push(initMouseParallax());
    cleanups.push(initFadeIn());
    cleanups.push(initNavbarScroll());
    return cleanups;
  },
  
  // Hakkında sayfası animasyonları
  initAbout() {
    const cleanups = [];
    cleanups.push(AnimationFactory.starParticles('stars-canvas'));
    cleanups.push(initFadeIn());
    cleanups.push(initNavbarScroll());
    return cleanups;
  },
  
  // Blog sayfası animasyonları
  initBlog() {
    const cleanups = [];
    cleanups.push(AnimationFactory.floatingParticles('particles-canvas'));
    cleanups.push(initFadeIn());
    cleanups.push(initNavbarScroll());
    return cleanups;
  },
  
  // Projeler sayfası animasyonları
  initProjects() {
    const cleanups = [];
    cleanups.push(AnimationFactory.floatingParticles('particles-canvas'));
    cleanups.push(initFadeIn());
    cleanups.push(initNavbarScroll());
    return cleanups;
  },
  
  //İletişim sayfası animasyonları
  initContact() {
    const cleanups = [];
    cleanups.push(AnimationFactory.verticalLines('particles-canvas'));
    cleanups.push(initFadeIn());
    cleanups.push(initNavbarScroll());
    return cleanups;
  },
  
  // Yetenekler sayfası animasyonları
  initSkills() {
    const cleanups = [];
    cleanups.push(AnimationFactory.floatingParticles('particles-canvas'));
    cleanups.push(initFadeIn());
    cleanups.push(initSkillBars());
    cleanups.push(initNavbarScroll());
    return cleanups;
  },
  
  // Tüm temizlik fonksiyonlarını çalıştır
  cleanup(cleanups) {
    cleanups.forEach(fn => fn && fn());
  }
};

// ===== OTOMATİK SAYFA TESPİTİ =====
// DOMContentLoaded → HTML tamamen yüklendiğinde çalışır.
// Sayfada bulunan elemanlara bakarak hangi sayfa olduğumuzu tespit edip
// doğru animasyonları başlatıyoruz (if-else zinciri).
document.addEventListener('DOMContentLoaded', () => {
  let cleanups = [];
  
  if (document.getElementById('fire-canvas')) {
    cleanups = AnimationApp.initIndex();
  } else if (document.getElementById('stars-canvas')) {
    cleanups = AnimationApp.initAbout();
  } else if (document.querySelector('.blog-container')) {
    cleanups = AnimationApp.initBlog();
  } else if (document.querySelector('.projects-container')) {
    cleanups = AnimationApp.initProjects();
  } else if (document.querySelector('.contact-container')) {
    cleanups = AnimationApp.initContact();
  } else if (document.querySelector('.skills-container')) {
    cleanups = AnimationApp.initSkills();
  }
  
  // beforeunload → Kullanıcı sayfadan ayrılırken temizlik yap.
  // Böylece bellekte kalan event listener'lar ve animasyonlar silinir.
  window.addEventListener('beforeunload', () => {
    AnimationApp.cleanup(cleanups);
  });
});

