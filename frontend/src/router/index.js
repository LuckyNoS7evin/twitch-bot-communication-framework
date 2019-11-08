import Vue from 'vue'
import VueRouter from 'vue-router'
import Home from '@/views/Home.vue'
import { oidcManager } from '@/plugins/oidc'

Vue.use(VueRouter)

const routes = [
  {
    path: '/',
    name: 'home',
    component: Home
  },
  {
    path: '/settings',
    name: 'settings',
    component: () => import(/* webpackChunkName: "settings" */ '@/views/Settings.vue'),
    meta: {
      requiresAuth: true
    }
  },
  {
    path: '/oidc-callback',
    name: 'OidcCallback',
    component: () => import(/* webpackChunkName: "oidc-callback" */ '@/views/OidcCallback.vue')
  }
]

const router = new VueRouter({
  mode: 'history',
  base: process.env.BASE_URL,
  routes
})

router.beforeEach((to, from, next) => {
  const requiresAuth = to.matched.some(record => record.meta.requiresAuth)
  if (requiresAuth) {
    oidcManager.getUser().then(user => {
      if (user == null) {
        router.push({ name: 'home' })
      } else {
        next()
      }
    })
  } else {
    next()
  }
})

export default router
