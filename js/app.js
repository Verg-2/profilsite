/* ============================================
  MODERN PROFİL SAYFASI - PERFORMANS OPTİMİZE EDİLMİŞ
  Ateş Kırmızısı Efektler

  Bu dosya, anasayfadaki:
  - Arka plandaki ateş partikül animasyonunu
  - Profil fotoğrafı parallax/mouse efektini
  - Navbar scroll efektini
  - Bölümlerin yumuşak geçiş (smooth scroll) davranışını
  - İletişim formu doğrulamasını
  kontrol eder.

  Yani sayfanın canlı ve interaktif hissettiren tüm dinamik kısımları buradan yönetiliyor.
  ============================================ */

// ===== CANVAS VE GENEL DEĞİŞKENLER =====
// Ateş efektini çizeceğimiz <canvas> elemanını yakalıyoruz.
const canvas = document.getElementById('fire-canvas');
// canvas bulunamazsa (herhangi bir hata almamak için) tüm animasyon kodlarını koşula bağlı çalıştıracağız.
const ctx = canvas ? canvas.getContext('2d') : null;

// Performans için kullanılan ortak değişkenler
let particles = [];              // Ekranda göreceğimiz tüm ateş parçacıkları bu dizide tutulur
const particleCount = 60;        // Aynı anda gösterilecek partikül (ateş tanesi) sayısı
let animationId = null;          // requestAnimationFrame id'sini saklarız, durdurmak için lazım
let isVisible = true;            // Sekme görünür mü? (Başka sekmeye geçince animasyonu durdurmak için)

// ===== LAZY LOADING - GÖRSELLER =====
/* 
   Burada IntersectionObserver kullanarak sadece ekranda görünen görselleri yükleyebilirdik.
   Şu an HTML tarafında "data-src" kullanan bir görsel olmadığı için bu kodu aktif etmiyoruz.
   Eğer ileride çok sayıda büyük resim eklenirse, bu bölümü tekrar aktif edip
   sadece görünür olan resimleri yükleyerek performans kazanabilirsin.
*/

// ===== CANVAS BOYUTLANDIRMA =====
function resizeCanvas() {
  // canvas yoksa (örneğin farklı bir sayfada bu JS dosyası yüklendiyse) hiçbir şey yapma
  if (!canvas) return;
  canvas.width = window.innerWidth;
  canvas.height = window.innerHeight;
}

// Sayfa açıldığında bir kere boyutlandırıyoruz ve pencere yeniden boyutlanınca tekrar hesaplıyoruz.
resizeCanvas();
window.addEventListener('resize', resizeCanvas);

// ===== ATEŞ PARÇACIKLARI (OPTİMİZE) =====
// Her ateş tanesini (parçacığı) temsil eden JS nesnesini oluşturan fonksiyon.
// Class yerine basit bir fonksiyon kullanmak hem daha hafif hem de yeterli.
function createParticle(initial = false) {
  return {
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
  };
}

// Sayfa açıldığında başlangıç partiküllerini oluşturuyoruz ki ekran boş görünmesin.
for (let i = 0; i < particleCount; i++) {
  const p = createParticle(true);
  p.maxLife = p.life;
  particles.push(p);
}

// ===== ANİMASYON DÖNGÜSÜ =====
function animate() {
  // Sekme görünür değilse ya da canvas/ctx yoksa çizim yapmıyoruz.
  if (!isVisible || !canvas || !ctx) return;
  
  // Canvas'ı tam silmek yerine yarı saydam siyah bir katman ile boyuyoruz.
  // Böylece ateş hareket ederken hoş bir "iz" efekti oluşuyor.
  ctx.fillStyle = 'rgba(10, 10, 12, 0.18)';
  ctx.fillRect(0, 0, canvas.width, canvas.height);
  
  // Karıştırma modu - ateş efekti
  ctx.globalCompositeOperation = 'lighter';
  
  // Renk paleti: ateş tonlarına yakın 3 farklı renk kullanıyoruz.
  const colors = [
    [255, 140, 0],   // Turuncu
    [255, 80, 0],    // Kırmızı-turuncu
    [255, 200, 50]   // Sarı-turuncu
  ];
  
  // Her partikülü güncelle ve çiz
  for (let i = 0; i < particles.length; i++) {
    const p = particles[i];
    
    // Hareket ve yaşlanma hesapları
    p.wobble += p.wobbleSpeed;
    p.x += p.speedX + Math.sin(p.wobble) * 0.4;
    p.y += p.speedY;
    p.life--;
    p.opacity = (p.life / p.maxLife) * 0.7;
    p.size *= 0.997;
    
    // Parçacık ekranın üstünden çıkarsa veya ömrü bittiyse, aşağıdan tekrar doğuruyoruz.
    if (p.y < -20 || p.life <= 0) {
      const newP = createParticle(false);
      newP.maxLife = newP.life;
      particles[i] = newP;
      continue;
    }
    
    // Parçacığı daire (küçük yuvarlak) olarak çiziyoruz.
    const c = colors[p.colorType];
    ctx.beginPath();
    ctx.arc(p.x, p.y, Math.max(0.5, p.size), 0, Math.PI * 2);
    ctx.fillStyle = `rgba(${c[0]}, ${c[1]}, ${c[2]}, ${p.opacity})`;
    ctx.fill();
  }
  
  ctx.globalCompositeOperation = 'source-over';
  
  // Performans: requestAnimationFrame kullan
  animationId = requestAnimationFrame(animate);
}

// Sayfa yüklendiğinde ateş animasyonunu tek sefer başlatıyoruz.
// Daha sonra requestAnimationFrame kendi kendine animate fonksiyonunu çağırmaya devam eder.
animate();

// ===== THROTTLE FONKSİYONU =====
// Bu yardımcı fonksiyon, çok sık tetiklenen event'lerin (scroll, mousemove gibi)
// belirlediğimiz aralıkta bir kez çalışmasını sağlar. Böylece performans korunur.
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

// ===== MOUSE PARALLAX (THROTTLED) =====
// Kullanıcının farenin hareketine göre profil görselini hafifçe hareket ettiriyoruz.
// Bu, 3D/parallax hissi verir ve kartı daha canlı gösterir.
const profileImg = document.getElementById('profile-img');
const imageGlow = document.getElementById('image-glow');
const imageFrame = document.querySelector('.image-frame');

// Throttle edilmiş mouse hareketi - 50ms sınırla
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

// Mouse sayfadan çıktığında tüm transform değerlerini sıfırlıyoruz ki görüntü normal hale gelsin.
document.addEventListener('mouseleave', () => {
  if (imageFrame) imageFrame.style.transform = 'perspective(1000px) rotateY(-5deg)';
  if (imageGlow) imageGlow.style.transform = 'translate(0, 0)';
  if (profileImg) profileImg.style.transform = 'translate(0, 0) scale(1)';
});

// ===== NAVBAR SCROLL (THROTTLED) =====
// Kullanıcı sayfayı aşağı kaydırdığında navbar'ın arka planı ve gölgesi değişiyor.
// Böylece hem daha okunur hem de yukarıda sabit duran bir başlık etkisi veriyor.
const handleScroll = throttle(() => {
  const navbar = document.querySelector('.navbar');
  if (navbar) {
    if (window.scrollY > 50) {
      navbar.style.background = 'rgba(10, 10, 12, 0.98)';
      navbar.style.boxShadow = '0 2px 30px rgba(255, 77, 0, 0.1)';
    } else {
      navbar.style.background = 'rgba(10, 10, 12, 0.9)';
      navbar.style.boxShadow = 'none';
    }
  }
}, 100); // 100ms sınır

window.addEventListener('scroll', handleScroll, { passive: true });

// ===== FADE-IN ANİMASYONU (INTERSECTION OBSERVER) =====
// .fade-in sınıfına sahip elemanlar ekranda görünmeye başladıkça
// yumuşak bir şekilde aşağıdan yukarı doğru beliriyor.
const observerOptions = {
  root: null,
  rootMargin: '0px',
  threshold: 0.1
};

const observer = new IntersectionObserver((entries) => {
  entries.forEach(entry => {
    if (entry.isIntersecting) {
      entry.target.classList.add('visible');
      
      // Yetenek çubukları animasyonu
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
}, observerOptions);

document.querySelectorAll('.fade-in').forEach(el => {
  observer.observe(el);
});

// ===== FORM GÖNDERİMİ =====
// İletişim formu gönderildiğinde basit bir "teşekkür" mesajı gösteriyoruz
// ve tüm alanlar dolu değilse kullanıcıyı uyarıyoruz.
const contactForm = document.getElementById('contact-form');
if (contactForm) {
  contactForm.addEventListener('submit', (e) => {
    e.preventDefault();
    
    const name = document.getElementById('name').value;
    const email = document.getElementById('email').value;
    const message = document.getElementById('message').value;
    
    if (name && email && message) {
      alert('Teşekkürler ' + name + '! Mesajınız gönderildi. 💌');
      contactForm.reset();
    } else {
      alert('Lütfen tüm alanları doldurun!');
    }
  });
}

// ===== YUMUŞAK SCROLL =====
// Sayfadaki dahili linkler (href="#...") tıklandığında tarayıcıyı
// o bölüme yumuşak bir animasyonla kaydırıyoruz.
document.querySelectorAll('a[href^="#"]').forEach(anchor => {
  anchor.addEventListener('click', function(e) {
    e.preventDefault();
    const target = document.querySelector(this.getAttribute('href'));
    if (target) {
      target.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
  });
});

// ===== PERFORMANS İZLEME =====
// Sayfanın ne kadar sürede yüklendiğini konsola yazar.
// Bu tamamen geliştiriciye yönelik, kullanıcı görmez.
window.addEventListener('load', () => {
  const loadTime = performance.now();
  console.log('⚡ Sayfa ' + loadTime.toFixed(2) + 'ms\'de yüklendi');
});

// ===== SAYFA GÖRÜNÜRLÜK API =====
// Kullanıcı başka sekmeye geçtiğinde animasyonu durdurup,
// geri geldiğinde yeniden başlatıyoruz. Böylece gereksiz CPU tüketmiyoruz.
document.addEventListener('visibilitychange', () => {
  isVisible = !document.hidden;
  if (isVisible && !animationId) {
    animate();
  }
});

// ===== WILL-CHANGE OPTİMİZASYONU =====
// Bazı elemanlara "will-change: transform" atayarak tarayıcıya
// bu elemanların sık sık hareket edeceğini söylüyoruz.
// Tarayıcı bu bilgiyi kullanarak GPU optimizasyonu yapabilir.
const animatedElements = document.querySelectorAll('.hero-image, .image-frame, .profile-img');
animatedElements.forEach(el => {
  el.style.willChange = 'transform';
});

// Sayfa kapatılmadan hemen önce animasyonu ve observer'ı temizliyoruz
// ki bellekte gereksiz iş kalmasın.
window.addEventListener('beforeunload', () => {
  if (animationId) {
    cancelAnimationFrame(animationId);
  }
  observer.disconnect();
});

