<template>
  <canvas 
    ref="canvasRef" 
    class="particles-canvas"
    :style="{ opacity: opacity }"
  ></canvas>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from 'vue'

const props = defineProps({
  opacity: {
    type: Number,
    default: 0.6
  },
  particleCount: {
    type: Number,
    default: 50
  }
})

const canvasRef = ref(null)
let ctx = null
let animationFrame = null
let particles = []
let isVisible = true

// Visibility API
const handleVisibilityChange = () => {
  isVisible = !document.hidden
  if (isVisible && !animationFrame) {
    animate()
  }
}

// Create particle
function createParticle(initial = false) {
  const canvas = canvasRef.value
  if (!canvas) return null
  
  return {
    x: Math.random() * canvas.width,
    y: initial ? Math.random() * canvas.height : canvas.height + Math.random() * 50,
    size: Math.random() * 2 + 0.5,
    speedX: (Math.random() - 0.5) * 0.4,
    speedY: -(Math.random() * 1.2 + 0.6),
    color: getRandomColor(),
    opacity: Math.random() * 0.4 + 0.2,
    life: Math.random() * 80 + 60,
    maxLife: 0,
    wobble: Math.random() * Math.PI * 2,
    wobbleSpeed: Math.random() * 0.02 + 0.01
  }
}

// Get random fire color
function getRandomColor() {
  const colors = [
    [255, 59, 29],   // Red
    [255, 100, 0],   // Orange
    [255, 150, 0],   // Amber
    [255, 200, 50]   // Yellow
  ]
  return colors[Math.floor(Math.random() * colors.length)]
}

// Resize canvas
function resizeCanvas() {
  const canvas = canvasRef.value
  if (!canvas) return
  
  canvas.width = window.innerWidth
  canvas.height = window.innerHeight
}

// Animation loop
function animate() {
  if (!isVisible) return
  
  const canvas = canvasRef.value
  if (!canvas || !ctx) {
    animationFrame = requestAnimationFrame(animate)
    return
  }
  
  // Clear with trail effect
  ctx.fillStyle = `rgba(11, 11, 15, 0.15)`
  ctx.fillRect(0, 0, canvas.width, canvas.height)
  
  // Additive blending for glow
  ctx.globalCompositeOperation = 'lighter'
  
  for (let i = 0; i < particles.length; i++) {
    const p = particles[i]
    if (!p) continue
    
    // Update position
    p.wobble += p.wobbleSpeed
    p.x += p.speedX + Math.sin(p.wobble) * 0.25
    p.y += p.speedY
    p.life--
    p.opacity = (p.life / p.maxLife) * 0.5
    p.size *= 0.998
    
    // Reset if off screen
    if (p.y < -20 || p.life <= 0) {
      particles[i] = createParticle(false)
      if (particles[i]) {
        particles[i].maxLife = particles[i].life
      }
      continue
    }
    
    // Draw
    const [r, g, b] = p.color
    ctx.beginPath()
    ctx.arc(p.x, p.y, Math.max(0.2, p.size), 0, Math.PI * 2)
    ctx.fillStyle = `rgba(${r}, ${g}, ${b}, ${p.opacity})`
    ctx.fill()
  }
  
  ctx.globalCompositeOperation = 'source-over'
  animationFrame = requestAnimationFrame(animate)
}

// Initialize
function init() {
  const canvas = canvasRef.value
  if (!canvas) return
  
  resizeCanvas()
  ctx = canvas.getContext('2d')
  
  // Create particles
  particles = []
  for (let i = 0; i < props.particleCount; i++) {
    const p = createParticle(true)
    if (p) {
      p.maxLife = p.life
      particles.push(p)
    }
  }
  
  // Event listeners
  window.addEventListener('resize', resizeCanvas)
  document.addEventListener('visibilitychange', handleVisibilityChange)
  
  // Start animation
  animate()
}

// Destroy
function destroy() {
  if (animationFrame) {
    cancelAnimationFrame(animationFrame)
    animationFrame = null
  }
  
  window.removeEventListener('resize', resizeCanvas)
  document.removeEventListener('visibilitychange', handleVisibilityChange)
  
  particles = []
}

onMounted(() => {
  init()
})

onUnmounted(() => {
  destroy()
})
</script>

<style scoped>
.particles-canvas {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  pointer-events: none;
  z-index: 0;
}
</style>

