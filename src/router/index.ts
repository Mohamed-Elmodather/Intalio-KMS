import { createRouter, createWebHistory } from 'vue-router'
import routes from './routes'
import { authGuard, titleGuard } from './guards'

const router = createRouter({
  history: createWebHistory(),
  routes,
  scrollBehavior(_to, _from, savedPosition) {
    if (savedPosition) {
      return savedPosition
    }
    return { top: 0 }
  },
})

// Navigation guards
router.beforeEach(authGuard)
router.afterEach(titleGuard)

export default router
