/**
 * useReveal.js - Scroll Reveal Efekti Composable
 * 
 * Kullanım:
 * const { observe, unobserve } = useReveal(callback)
 * 
 * Veya doğrudan:
 * <div v-reveal>...</div>
 */

import { onMounted, onUnmounted } from 'vue'

export function useReveal(options = {}) {
  const {
    threshold = 0.1,    // Element görünürlük eşiği
    rootMargin = '0px', // Margin
    enabled = true      // Efekt aktif mi
  } = options

  let observer = null

  // Kullanıcı azaltılmış hareket tercih ediyor mu?
  const prefersReducedMotion = () => {
    return window.matchMedia('(prefers-reduced-motion: reduce)').matches
  }

  // Intersection Observer callback
  const handleIntersect = (entries) => {
    entries.forEach(entry => {
      if (entry.isIntersecting) {
        // visible class ekle
        entry.target.classList.add('visible')
        
        // Bir kez tetiklendikten sonra gözlemlemeyi bırak
        if (observer) {
          observer.unobserve(entry.target)
        }
      }
    })
  }

  // Elementi gözlemle
  const observe = (element) => {
    if (!enabled || prefersReducedMotion() || !observer || !element) return
    
    element.classList.add('reveal')
    observer.observe(element)
  }

  // Elementi gözlemlemeyi bırak
  const unobserve = (element) => {
    if (!observer || !element) return
    observer.unobserve(element)
  }

  // Tüm .reveal sınıflı elementleri gözlemle
  const observeAll = (selector = '.reveal') => {
    if (!enabled || prefersReducedMotion()) return
    
    const elements = document.querySelectorAll(selector)
    elements.forEach(el => {
      el.classList.add('reveal')
      observer.observe(el)
    })
  }

  onMounted(() => {
    if (!enabled) return
    
    // Azaltılmış hareket tercihi varsa tüm elementleri görünür yap
    if (prefersReducedMotion()) {
      document.querySelectorAll('.reveal').forEach(el => {
        el.classList.add('visible')
      })
      return
    }

    // Observer oluştur
    observer = new IntersectionObserver(handleIntersect, {
      threshold,
      rootMargin
    })
  })

  onUnmounted(() => {
    if (observer) {
      observer.disconnect()
      observer = null
    }
  })

  return {
    observe,
    unobserve,
    observeAll
  }
}

// Directive olarak kullanmak için
export const revealDirective = {
  mounted(el) {
    el.classList.add('reveal')
    
    const observer = new IntersectionObserver((entries) => {
      entries.forEach(entry => {
        if (entry.isIntersecting) {
          el.classList.add('visible')
          observer.unobserve(el)
        }
      })
    }, { threshold: 0.1 })
    
    observer.observe(el)
    
    // Cleanup için elementte sakla
    el._revealObserver = observer
  },
  unmounted(el) {
    if (el._revealObserver) {
      el._revealObserver.disconnect()
    }
  }
}

