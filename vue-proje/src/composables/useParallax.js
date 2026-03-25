/**
 * useParallax.js - Mouse Parallax Efekti Composable
 * 
 * Kullanım:
 * const { updateParallax, resetParallax } = useParallax(targetRef, options)
 */

import { ref, onMounted, onUnmounted } from 'vue'

export function useParallax(targetRef, options = {}) {
  const {
    strength = 15,      // Parallax gücü
    enabled = true,    // Efekt aktif mi
    throttle = 16     // ms cinsinden throttle
  } = options

  const currentX = ref(0)
  const currentY = ref(0)
  const targetX = ref(0)
  const targetY = ref(0)
  
  let animationFrame = null
  let throttleTimer = null

  // Mouse pozisyonunu güncelle
  const handleMouseMove = (e) => {
    if (!enabled) return
    
    // Throttle
    if (throttleTimer) return
    
    throttleTimer = setTimeout(() => {
      throttleTimer = null
    }, throttle)
    
    // Normalize mouse position (-1 to 1)
    targetX.value = (e.clientX / window.innerWidth) * 2 - 1
    targetY.value = (e.clientY / window.innerHeight) * 2 - 1
  }

  // Yumuşak takip animasyonu
  const animate = () => {
    if (!enabled) return
    
    // Lerp - yumuşak geçiş
    const ease = 0.1
    currentX.value += (targetX.value - currentX.value) * ease
    currentY.value += (targetY.value - currentY.value) * ease
    
    // Elementi hareket ettir
    if (targetRef.value) {
      const moveX = currentX.value * strength
      const moveY = currentY.value * strength
      
      targetRef.value.style.transform = `
        translateX(${moveX}px) 
        translateY(${moveY}px)
        rotateY(${currentX.value * 5}deg)
        rotateX(${-currentY.value * 3}deg)
      `
    }
    
    animationFrame = requestAnimationFrame(animate)
  }

  // Parallax'ı sıfırla
  const resetParallax = () => {
    targetX.value = 0
    targetY.value = 0
  }

  onMounted(() => {
    if (enabled) {
      window.addEventListener('mousemove', handleMouseMove)
      animate()
    }
  })

  onUnmounted(() => {
    window.removeEventListener('mousemove', handleMouseMove)
    if (animationFrame) {
      cancelAnimationFrame(animationFrame)
    }
    if (throttleTimer) {
      clearTimeout(throttleTimer)
    }
  })

  return {
    currentX,
    currentY,
    updateParallax: handleMouseMove,
    resetParallax
  }
}

