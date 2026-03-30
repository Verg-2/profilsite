import { createRouter, createWebHistory } from 'vue-router'

const routes = [
  { path: '/', name: 'Index', component: () => import('../views/IndexView.vue') },
  { path: '/hakkinda', name: 'Hakkinda', component: () => import('../views/HakkindaView.vue') },
  { path: '/blog', name: 'Blog', component: () => import('../views/BlogView.vue') },
  { path: '/yetenekler', name: 'Yetenekler', component: () => import('../views/YeteneklerView.vue') },
  { path: '/projects', name: 'Projects', component: () => import('../views/ProjectsView.vue') },
  { path: '/contact', name: 'Contact', component: () => import('../views/ContactView.vue') }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router

