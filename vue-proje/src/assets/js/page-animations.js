/*
  Bu dosya pages/js/animations.js mantigini Vue Router uyumlu hale getirir.
  Statik HTML tarafinda DOMContentLoaded bir kez calisiyordu.
  Vue SPA'de ise route degisir ama sayfa yeniden yuklenmez.
  Bu nedenle her route gecisinden sonra animasyonlari elle kurup temizliyoruz.
*/

let cleanups = []

function getThemeColor(darkColor, lightColor) {
  return document.documentElement.getAttribute('data-theme') === 'light' ? lightColor : darkColor
}

function addCleanup(fn) {
  if (typeof fn === 'function') {
    cleanups.push(fn)
  }
}

function throttle(func, limit) {
  let inThrottle = false

  return function throttled(...args) {
    if (inThrottle) return

    func.apply(this, args)
    inThrottle = true

    setTimeout(() => {
      inThrottle = false
    }, limit)
  }
}

function setupCanvas(canvas) {
  if (!canvas) return null

  const ctx = canvas.getContext('2d')
  if (!ctx) return null

  const resize = () => {
    canvas.width = window.innerWidth
    canvas.height = window.innerHeight
  }

  resize()
  window.addEventListener('resize', resize)
  addCleanup(() => window.removeEventListener('resize', resize))

  return { canvas, ctx }
}

function fireParticles(canvasId) {
  const setup = setupCanvas(document.getElementById(canvasId))
  if (!setup) return

  const { canvas, ctx } = setup
  const isLight = document.documentElement.getAttribute('data-theme') === 'light'
  const colors = isLight
    ? [
        [240, 90, 40],
        [216, 69, 24],
        [245, 166, 35],
      ]
    : [
        [255, 140, 0],
        [255, 80, 0],
        [255, 200, 50],
      ]

  const particles = []
  const particleCount = 60
  let animationId = null
  let isVisible = true

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
    wobbleSpeed: Math.random() * 0.02 + 0.015,
  })

  for (let index = 0; index < particleCount; index += 1) {
    const particle = createParticle(true)
    particle.maxLife = particle.life
    particles.push(particle)
  }

  const animate = () => {
    if (!isVisible) {
      animationId = null
      return
    }

    ctx.fillStyle = getThemeColor('rgba(10, 10, 12, 0.18)', 'rgba(250, 250, 250, 0.18)')
    ctx.fillRect(0, 0, canvas.width, canvas.height)
    ctx.globalCompositeOperation = isLight ? 'source-over' : 'lighter'

    for (let index = 0; index < particles.length; index += 1) {
      const particle = particles[index]

      particle.wobble += particle.wobbleSpeed
      particle.x += particle.speedX + Math.sin(particle.wobble) * 0.4
      particle.y += particle.speedY
      particle.life -= 1
      particle.opacity = (particle.life / particle.maxLife) * 0.7
      particle.size *= 0.997

      if (particle.y < -20 || particle.life <= 0) {
        const nextParticle = createParticle(false)
        nextParticle.maxLife = nextParticle.life
        particles[index] = nextParticle
        continue
      }

      const color = colors[particle.colorType]
      ctx.beginPath()
      ctx.arc(particle.x, particle.y, Math.max(0.5, particle.size), 0, Math.PI * 2)
      ctx.fillStyle = `rgba(${color[0]}, ${color[1]}, ${color[2]}, ${particle.opacity})`
      ctx.fill()
    }

    ctx.globalCompositeOperation = 'source-over'
    animationId = requestAnimationFrame(animate)
  }

  const handleVisibility = () => {
    isVisible = !document.hidden

    if (isVisible && !animationId) {
      animate()
    }
  }

  document.addEventListener('visibilitychange', handleVisibility)
  animate()

  addCleanup(() => {
    document.removeEventListener('visibilitychange', handleVisibility)
    if (animationId) cancelAnimationFrame(animationId)
  })
}

function initMouseParallax() {
  const profileImg = document.getElementById('profile-img')
  const imageGlow = document.getElementById('image-glow')
  const imageFrame = document.querySelector('.image-frame')

  if (!profileImg && !imageGlow && !imageFrame) return

  const handleMouseMove = throttle((event) => {
    const mouseX = (event.clientX / window.innerWidth) * 2 - 1
    const mouseY = (event.clientY / window.innerHeight) * 2 - 1

    if (imageFrame) {
      imageFrame.style.transform =
        `perspective(1000px) rotateY(${mouseX * 8}deg) rotateX(${-mouseY * 5}deg) ` +
        `translateX(${mouseX * 15}px) translateY(${mouseY * 10}px)`
    }

    if (imageGlow) {
      imageGlow.style.transform = `translate(${mouseX * 20}px, ${mouseY * 15}px)`
    }

    if (profileImg) {
      profileImg.style.transform = `translate(${mouseX * -10}px, ${mouseY * -8}px) scale(1.02)`
    }
  }, 16)

  const handleMouseLeave = () => {
    if (imageFrame) imageFrame.style.transform = 'perspective(1000px) rotateY(-5deg)'
    if (imageGlow) imageGlow.style.transform = 'translate(0, 0)'
    if (profileImg) profileImg.style.transform = 'translate(0, 0) scale(1)'
  }

  document.addEventListener('mousemove', handleMouseMove)
  document.addEventListener('mouseleave', handleMouseLeave)

  addCleanup(() => {
    document.removeEventListener('mousemove', handleMouseMove)
    document.removeEventListener('mouseleave', handleMouseLeave)
  })
}

function initFadeIn() {
  const fadeInElements = document.querySelectorAll('.fade-in')
  if (!fadeInElements.length) return

  const observer = new IntersectionObserver((entries) => {
    entries.forEach((entry) => {
      if (!entry.isIntersecting) return

      entry.target.classList.add('visible')

      const skillBars = entry.target.querySelectorAll('.skill-progress')
      skillBars.forEach((bar) => {
        const targetWidth = bar.getAttribute('data-width')
        const delay = bar.getAttribute('data-delay')
        if (!targetWidth) return

        if (delay) {
          bar.style.transitionDelay = `${delay}ms`
        }

        setTimeout(() => {
          bar.classList.add('animate')
          bar.style.width = `${targetWidth}%`
        }, 300)
      })

      observer.unobserve(entry.target)
    })
  }, { threshold: 0.1 })

  fadeInElements.forEach((element) => observer.observe(element))
  addCleanup(() => observer.disconnect())
}

function initNavbarScroll() {
  const navbar = document.querySelector('.navbar')
  if (!navbar) return

  const handleScroll = () => {
    if (window.scrollY > 50) {
      navbar.style.background = getThemeColor('rgba(10, 10, 12, 0.98)', 'rgba(250, 250, 250, 0.98)')
      navbar.style.boxShadow = getThemeColor('0 2px 30px rgba(255, 77, 0, 0.1)', '0 2px 30px rgba(255, 87, 34, 0.1)')
    } else {
      navbar.style.background = getThemeColor('rgba(10, 10, 12, 0.9)', 'rgba(250, 250, 250, 0.9)')
      navbar.style.boxShadow = 'none'
    }
  }

  window.addEventListener('scroll', handleScroll, { passive: true })
  handleScroll()
  addCleanup(() => window.removeEventListener('scroll', handleScroll))
}

function initSkillBars() {
  const skillsSection = document.querySelector('.skills-container')
  if (!skillsSection) return

  const handleScroll = () => {
    const rect = skillsSection.getBoundingClientRect()

    if (rect.top < window.innerHeight * 0.8) {
      document.querySelectorAll('.skill-progress').forEach((bar) => {
        const width = bar.getAttribute('data-width')
        const delay = bar.getAttribute('data-delay')
        if (width && bar.style.width !== `${width}%`) {
          if (delay) {
            bar.style.transitionDelay = `${delay}ms`
          }
          setTimeout(() => {
            bar.classList.add('animate')
            bar.style.width = `${width}%`
          }, 100)
        }
      })

      window.removeEventListener('scroll', handleScroll)
    }
  }

  window.addEventListener('scroll', handleScroll, { passive: true })
  handleScroll()
  addCleanup(() => window.removeEventListener('scroll', handleScroll))
}

function starParticles(canvasId) {
  const setup = setupCanvas(document.getElementById(canvasId))
  if (!setup) return

  const { canvas, ctx } = setup
  let animationId = null

  class Star {
    constructor() {
      this.reset()
      this.y = Math.random() * canvas.height
    }

    reset() {
      this.x = Math.random() * canvas.width
      this.y = canvas.height + 10
      this.size = Math.random() * 2 + 0.5
      this.speedY = -(Math.random() * 0.5 + 0.2)
      this.speedX = (Math.random() - 0.5) * 0.3
      this.opacity = 0
      this.maxOpacity = Math.random() * 0.6 + 0.2
      this.twinkleSpeed = Math.random() * 0.02 + 0.01
      this.twinklePhase = Math.random() * Math.PI * 2

      const isLight = document.documentElement.getAttribute('data-theme') === 'light'
      const colors = isLight 
        ? [
            [240, 90, 40],
            [255, 132, 94],
            [252, 160, 32],
            [100, 100, 100],
          ]
        : [
            [255, 77, 0],
            [255, 107, 53],
            [255, 165, 0],
            [255, 255, 255],
          ]

      this.color = colors[Math.floor(Math.random() * colors.length)]
    }

    update() {
      this.y += this.speedY
      this.x += this.speedX
      this.twinklePhase += this.twinkleSpeed
      this.opacity = this.maxOpacity * (0.5 + 0.5 * Math.sin(this.twinklePhase))

      if (this.y < -10 || this.x < -10 || this.x > canvas.width + 10) {
        this.reset()
      }
    }

    draw() {
      ctx.beginPath()
      ctx.arc(this.x, this.y, this.size, 0, Math.PI * 2)
      ctx.fillStyle = `rgba(${this.color[0]}, ${this.color[1]}, ${this.color[2]}, ${this.opacity})`
      ctx.fill()

      if (this.size > 1.2) {
        const gradient = ctx.createRadialGradient(this.x, this.y, 0, this.x, this.y, this.size * 4)
        gradient.addColorStop(0, `rgba(${this.color[0]}, ${this.color[1]}, ${this.color[2]}, ${this.opacity * 0.3})`)
        gradient.addColorStop(1, 'transparent')

        ctx.fillStyle = gradient
        ctx.beginPath()
        ctx.arc(this.x, this.y, this.size * 4, 0, Math.PI * 2)
        ctx.fill()
      }
    }
  }

  const stars = Array.from({ length: 60 }, () => new Star())

  const animate = () => {
    ctx.clearRect(0, 0, canvas.width, canvas.height)

    const bgGradient = ctx.createRadialGradient(
      canvas.width / 2,
      canvas.height / 2,
      0,
      canvas.width / 2,
      canvas.height / 2,
      canvas.width,
    )

    bgGradient.addColorStop(0, getThemeColor('rgba(10, 10, 12, 0)', 'rgba(250, 250, 250, 0)'))
    bgGradient.addColorStop(1, getThemeColor('rgba(10, 10, 12, 0.3)', 'rgba(250, 250, 250, 0.3)'))
    ctx.fillStyle = bgGradient
    ctx.fillRect(0, 0, canvas.width, canvas.height)

    stars.forEach((star) => {
      star.update()
      star.draw()
    })

    animationId = requestAnimationFrame(animate)
  }

  animate()
  addCleanup(() => animationId && cancelAnimationFrame(animationId))
}

function floatingParticles(canvasId) {
  const setup = setupCanvas(document.getElementById(canvasId))
  if (!setup) return

  const { canvas, ctx } = setup
  const isLight = document.documentElement.getAttribute('data-theme') === 'light'
  const colors = isLight
    ? [
        [240, 90, 40],
        [255, 132, 94],
        [252, 160, 32],
      ]
    : [
        [255, 59, 29],
        [255, 140, 0],
        [255, 176, 0],
      ]

  const particles = Array.from({ length: 50 }, () => ({
    x: Math.random() * canvas.width,
    y: Math.random() * canvas.height,
    size: Math.random() * 2 + 0.5,
    speedX: (Math.random() - 0.5) * 0.5,
    speedY: -(Math.random() * 1 + 0.3),
    color: colors[Math.floor(Math.random() * colors.length)],
    opacity: Math.random() * 0.4 + 0.1,
  }))

  let animationId = null

  const animate = () => {
    ctx.clearRect(0, 0, canvas.width, canvas.height)
    ctx.globalCompositeOperation = isLight ? 'source-over' : 'lighter'

    particles.forEach((particle) => {
      particle.x += particle.speedX
      particle.y += particle.speedY

      if (particle.y < -10 || particle.x < 0 || particle.x > canvas.width) {
        particle.x = Math.random() * canvas.width
        particle.y = canvas.height + 10
      }

      ctx.beginPath()
      ctx.arc(particle.x, particle.y, particle.size, 0, Math.PI * 2)
      ctx.fillStyle = `rgba(${particle.color[0]}, ${particle.color[1]}, ${particle.color[2]}, ${particle.opacity})`
      ctx.fill()
    })

    ctx.globalCompositeOperation = 'source-over'
    animationId = requestAnimationFrame(animate)
  }

  animate()
  addCleanup(() => animationId && cancelAnimationFrame(animationId))
}

function verticalLines(canvasId) {
  const setup = setupCanvas(document.getElementById(canvasId))
  if (!setup) return

  const { canvas, ctx } = setup
  const lines = Array.from({ length: 8 }, () => ({
    x: Math.random() * canvas.width,
    speedY: Math.random() * 2 + 1,
    length: Math.random() * 100 + 50,
    opacity: Math.random() * 0.3 + 0.1,
  }))

  let animationId = null

  const animate = () => {
    ctx.clearRect(0, 0, canvas.width, canvas.height)
    const isLight = document.documentElement.getAttribute('data-theme') === 'light'
    ctx.globalCompositeOperation = isLight ? 'source-over' : 'lighter'

    lines.forEach((line) => {
      line.x += Math.sin(Date.now() * 0.001) * 0.2

      const gradient = ctx.createLinearGradient(0, 0, 0, canvas.height)
      gradient.addColorStop(0, 'transparent')
      const colorRGB = document.documentElement.getAttribute('data-theme') === 'light' ? '240, 90, 40' : '255, 100, 0'
      gradient.addColorStop(0.5, `rgba(${colorRGB}, ${line.opacity})`)
      gradient.addColorStop(1, 'transparent')

      ctx.strokeStyle = gradient
      ctx.lineWidth = 1
      ctx.beginPath()
      ctx.moveTo(line.x, 0)
      ctx.lineTo(line.x + 20, canvas.height)
      ctx.stroke()
    })

    ctx.globalCompositeOperation = 'source-over'
    animationId = requestAnimationFrame(animate)
  }

  animate()
  addCleanup(() => animationId && cancelAnimationFrame(animationId))
}

export function initPageAnimations() {
  if (document.getElementById('fire-canvas')) {
    fireParticles('fire-canvas')
    initMouseParallax()
  } else if (document.getElementById('stars-canvas')) {
    starParticles('stars-canvas')
  } else if (document.querySelector('.contact-container')) {
    verticalLines('particles-canvas')
  } else if (document.querySelector('.blog-container')) {
    floatingParticles('particles-canvas')
  } else if (document.querySelector('.projects-container') || document.querySelector('.project-detail-page')) {
    floatingParticles('particles-canvas')
  } else if (document.querySelector('.skills-container')) {
    floatingParticles('particles-canvas')
    initSkillBars()
  }

  initFadeIn()
  initNavbarScroll()
}

export function cleanupPageAnimations() {
  cleanups.forEach((cleanup) => cleanup())
  cleanups = []
}
