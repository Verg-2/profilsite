/**
 * useParticles.js - Ember Partikül Efekti Composable
 * 
 * Kullanım:
 * const { initParticles, destroyParticles } = useParticles(canvasRef, options)
 */

export function useParticles(canvasRef, options = {}) {
  const {
    count = 60,           // Partikül sayısı
    enabled = true,       // Efekt aktif mi
    colors = [            // Ateş renkleri
      [255, 59, 29],     // Kırmızı
      [255, 120, 0],     // Turuncu
      [255, 176, 0],     // Amber
      [255, 220, 100]    // Sarı
    ]
  } = options

  let ctx = null
  let animationFrame = null
  let particles = []
  let isVisible = true

  // Visibility API - sekme değişince durdur
  const handleVisibilityChange = () => {
    isVisible = !document.hidden
    if (isVisible && !animationFrame) {
      animate()
    }
  }

  // Partikül oluştur
  function createParticle(initial = false) {
    const canvas = canvasRef.value
    if (!canvas) return null
    
    return {
      x: Math.random() * canvas.width,
      y: initial ? Math.random() * canvas.height : canvas.height + Math.random() * 50,
      size: Math.random() * 2.5 + 0.8,
      speedX: (Math.random() - 0.5) * 0.5,
      speedY: -(Math.random() * 1.5 + 0.8),
      color: colors[Math.floor(Math.random() * colors.length)],
      opacity: Math.random() * 0.5 + 0.3,
      life: Math.random() * 100 + 80,
      maxLife: 0,
      wobble: Math.random() * Math.PI * 2,
      wobbleSpeed: Math.random() * 0.02 + 0.01
    }
  }

  // Canvas boyutlandır
  function resizeCanvas() {
    const canvas = canvasRef.value
    if (!canvas) return
    
    canvas.width = window.innerWidth
    canvas.height = window.innerHeight
    ctx = canvas.getContext('2d')
  }

  // Animasyon döngüsü
  function animate() {
    if (!isVisible || !enabled) return
    
    const canvas = canvasRef.value
    if (!canvas || !ctx) {
      animationFrame = requestAnimationFrame(animate)
      return
    }

    // Temizle - yarı saydam siyah ile iz efekti
    ctx.fillStyle = 'rgba(11, 11, 15, 0.15)'
    ctx.fillRect(0, 0, canvas.width, canvas.height)

    // Karıştırma modu - parlaklık için
    ctx.globalCompositeOperation = 'lighter'

    // Her partikülü güncelle ve çiz
    for (let i = 0; i < particles.length; i++) {
      const p = particles[i]
      if (!p) continue

      // Hareket
      p.wobble += p.wobbleSpeed
      p.x += p.speedX + Math.sin(p.wobble) * 0.3
      p.y += p.speedY
      p.life--
      p.opacity = (p.life / p.maxLife) * 0.6
      p.size *= 0.998

      // Yeniden oluştur
      if (p.y < -20 || p.life <= 0) {
        particles[i] = createParticle(false)
        if (particles[i]) {
          particles[i].maxLife = particles[i].life
        }
        continue
      }

      // Çiz
      const [r, g, b] = p.color
      ctx.beginPath()
      ctx.arc(p.x, p.y, Math.max(0.3, p.size), 0, Math.PI * 2)
      ctx.fillStyle = `rgba(${r}, ${g}, ${b}, ${p.opacity})`
      ctx.fill()
    }

    ctx.globalCompositeOperation = 'source-over'
    animationFrame = requestAnimationFrame(animate)
  }

  // Başlat
  function initParticles() {
    if (!enabled) return
    
    const canvas = canvasRef.value
    if (!canvas) return
    
    resizeCanvas()
    ctx = canvas.getContext('2d')
    
    // Partikülleri oluştur
    particles = []
    for (let i = 0; i < count; i++) {
      const p = createParticle(true)
      if (p) {
        p.maxLife = p.life
        particles.push(p)
      }
    }

    // Event listeners
    window.addEventListener('resize', resizeCanvas)
    document.addEventListener('visibilitychange', handleVisibilityChange)

    // Animasyonu başlat
    animate()
  }

  // Temizle
  function destroyParticles() {
    if (animationFrame) {
      cancelAnimationFrame(animationFrame)
      animationFrame = null
    }
    
    window.removeEventListener('resize', resizeCanvas)
    document.removeEventListener('visibilitychange', handleVisibilityChange)
    
    particles = []
  }

  return {
    initParticles,
    destroyParticles
  }
}

