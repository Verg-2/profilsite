<template>
  <canvas 
    ref="canvasRef" 
    class="particles-canvas"
    :style="{ opacity: opacity }"
  ></canvas>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from 'vue'
import { useParticles } from '../composables/useParticles'

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

const { initParticles, destroyParticles } = useParticles(canvasRef, {
  count: props.particleCount,
  colors: [
    [255, 59, 29],   // Red
    [255, 100, 0],   // Orange
    [255, 150, 0],   // Amber
    [255, 200, 50]   // Yellow
  ]
})

onMounted(() => {
  initParticles()
})

onUnmounted(() => {
  destroyParticles()
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

