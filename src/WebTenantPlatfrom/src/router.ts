import auth from './auth'
import { createRouter, createWebHashHistory } from 'vue-router'
import type { RouteRecordRaw } from 'vue-router'

import defaultLayout from './layouts/side-nav-outer-toolbar.vue'
import simpleLayout from './layouts/single-card.vue'

function loadView(view: string) {
  return () => import(`./views/${view}.vue`)
}

const routes: RouteRecordRaw[] = [
  {
    path: '/dashboard',
    name: 'dashboard',
    meta: {
      requiresAuth: true,
      layout: defaultLayout
    },
    component: () => import('./views/dashboard/DashboardView.vue')
  },
  {
    path: '/platform-users',
    name: 'platform-users',
    meta: {
      requiresAuth: true,
      layout: defaultLayout
    },
    component: () => import('./views/platform-users/PlatformUsersView.vue')
  },
  {
    path: '/platform-roles',
    name: 'platform-roles',
    meta: {
      requiresAuth: true,
      layout: defaultLayout
    },
    component: () => import('./views/platform-roles/PlatformRolesView.vue')
  },
  {
    path: '/platform-permissions',
    name: 'platform-permissions',
    meta: {
      requiresAuth: true,
      layout: defaultLayout
    },
    component: () => import('./views/platform-permissions/PlatformPermissionsView.vue')
  },
  {
    path: '/tenants',
    name: 'tenants',
    meta: {
      requiresAuth: true,
      layout: defaultLayout
    },
    component: loadView('home-page')
  },
  {
    path: '/tenant-info',
    name: 'tenant-info',
    meta: {
      requiresAuth: true,
      layout: defaultLayout
    },
    component: loadView('home-page')
  },
  {
    path: '/tenant-resources',
    name: 'tenant-resources',
    meta: {
      requiresAuth: true,
      layout: defaultLayout
    },
    component: loadView('home-page')
  },
  {
    path: '/tenant-config',
    name: 'tenant-config',
    meta: {
      requiresAuth: true,
      layout: defaultLayout
    },
    component: loadView('home-page')
  },
  {
    path: '/packages',
    name: 'packages',
    meta: {
      requiresAuth: true,
      layout: defaultLayout
    },
    component: loadView('home-page')
  },
  {
    path: '/subscriptions',
    name: 'subscriptions',
    meta: {
      requiresAuth: true,
      layout: defaultLayout
    },
    component: loadView('home-page')
  },
  {
    path: '/billing',
    name: 'billing',
    meta: {
      requiresAuth: true,
      layout: defaultLayout
    },
    component: loadView('home-page')
  },
  {
    path: '/platform-operations',
    name: 'platform-operations',
    meta: {
      requiresAuth: true,
      layout: defaultLayout
    },
    component: loadView('home-page')
  },
  {
    path: '/api-integration',
    name: 'api-integration',
    meta: {
      requiresAuth: true,
      layout: defaultLayout
    },
    component: loadView('home-page')
  },
  {
    path: '/audit-logs',
    name: 'audit-logs',
    meta: {
      requiresAuth: true,
      layout: defaultLayout
    },
    component: loadView('home-page')
  },
  {
    path: '/notifications',
    name: 'notifications',
    meta: {
      requiresAuth: true,
      layout: defaultLayout
    },
    component: loadView('home-page')
  },
  {
    path: '/storage',
    name: 'storage',
    meta: {
      requiresAuth: true,
      layout: defaultLayout
    },
    component: loadView('home-page')
  },
  {
    path: '/profile',
    name: 'profile',
    meta: {
      requiresAuth: true,
      layout: defaultLayout
    },
    component: loadView('profile-page')
  },
  {
    path: '/login-form',
    name: 'login-form',
    meta: {
      requiresAuth: false,
      title: 'Sign In'
    },
    component: () => import('./views/login/LoginView.vue')
  },
  {
    path: '/reset-password',
    name: 'reset-password',
    meta: {
      requiresAuth: false,
      layout: simpleLayout,
      title: 'Reset Password',
      description: 'Please enter the email address that you used to register, and we will send you a link to reset your password via Email.'
    },
    component: loadView('reset-password-form')
  },
  {
    path: '/create-account',
    name: 'create-account',
    meta: {
      requiresAuth: false,
      layout: simpleLayout,
      title: 'Sign Up'
    },
    component: loadView('create-account-form')
  },
  {
    path: '/change-password/:recoveryCode',
    name: 'change-password',
    meta: {
      requiresAuth: false,
      layout: simpleLayout,
      title: 'Change Password'
    },
    component: loadView('change-password-form')
  },
  {
    path: '/',
    redirect: '/dashboard'
  },
  {
    path: '/recovery',
    redirect: '/dashboard'
  },
  {
    path: '/:pathMatch(.*)*',
    redirect: '/dashboard'
  }
]

const router = createRouter({
  routes,
  history: createWebHashHistory()
})

router.beforeEach((to, _from, next) => {
  if (to.name === 'login-form' && auth.loggedIn()) {
    next({ name: 'dashboard' })
    return
  }

  if (to.matched.some(record => record.meta.requiresAuth)) {
    if (!auth.loggedIn()) {
      next({
        name: 'login-form',
        query: { redirect: to.fullPath }
      })
    } else {
      next()
    }
  } else {
    next()
  }
})

export default router
