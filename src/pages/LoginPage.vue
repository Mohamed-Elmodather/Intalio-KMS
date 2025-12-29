<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { useUIStore } from '@/stores/ui'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'

const router = useRouter()
const route = useRoute()
const authStore = useAuthStore()
const uiStore = useUIStore()

// Form state
const email = ref('')
const password = ref('')
const rememberMe = ref(false)
const showPassword = ref(false)
const isSubmitting = ref(false)
const formError = ref('')

// SSO state
const ssoLoading = ref(false)
const ssoAvailable = ref(false)

// Handle SSO callback
onMounted(async () => {
  // Check if this is an SSO callback
  const code = route.query.code as string
  if (code) {
    ssoLoading.value = true
    const success = await authStore.handleSSOCallback(code)
    if (success) {
      const redirect = route.query.redirect as string || '/'
      router.push(redirect)
    } else {
      formError.value = 'SSO authentication failed. Please try again.'
    }
    ssoLoading.value = false
    return
  }

  // Check SSO availability
  const config = await authStore.getSSOConfig()
  ssoAvailable.value = !!config
})

async function handleLogin() {
  if (!email.value || !password.value) {
    formError.value = 'Please enter both email and password'
    return
  }

  formError.value = ''
  isSubmitting.value = true

  try {
    const success = await authStore.loginWithCredentials({
      email: email.value,
      password: password.value,
      rememberMe: rememberMe.value,
    })

    if (success) {
      uiStore.showSuccess('Welcome back!', `Logged in as ${authStore.user?.displayName}`)
      const redirect = route.query.redirect as string || '/'
      router.push(redirect)
    } else {
      formError.value = authStore.error || 'Invalid email or password'
    }
  } catch {
    formError.value = 'An error occurred. Please try again.'
  } finally {
    isSubmitting.value = false
  }
}

function handleSSO() {
  ssoLoading.value = true
  authStore.initiateSSO()
}
</script>

<template>
  <div class="min-h-screen gradient-bg-animated flex items-center justify-center p-4">
    <div class="w-full max-w-md">
      <!-- Login Card -->
      <div class="glass-card-elevated rounded-3xl p-8 shadow-xl">
        <!-- Logo & Title -->
        <div class="text-center mb-8">
          <img src="/Intalio.png" alt="Intalio" class="h-12 mx-auto mb-4">
          <h1 class="text-2xl font-bold text-gray-900">Welcome Back</h1>
          <p class="text-gray-500 mt-2">Sign in to your Knowledge Hub account</p>
        </div>

        <!-- Loading state for SSO callback -->
        <div v-if="ssoLoading" class="py-12">
          <LoadingSpinner size="lg" text="Authenticating..." />
        </div>

        <template v-else>
          <!-- SSO Button -->
          <button
            v-if="ssoAvailable"
            @click="handleSSO"
            :disabled="isSubmitting"
            class="w-full flex items-center justify-center gap-3 px-6 py-4 bg-white border-2 border-gray-200 rounded-2xl text-gray-700 font-medium hover:border-blue-300 hover:bg-blue-50 transition-all shadow-sm"
          >
            <svg class="w-5 h-5" viewBox="0 0 21 21" xmlns="http://www.w3.org/2000/svg">
              <rect x="1" y="1" width="9" height="9" fill="#f25022"/>
              <rect x="11" y="1" width="9" height="9" fill="#7fba00"/>
              <rect x="1" y="11" width="9" height="9" fill="#00a4ef"/>
              <rect x="11" y="11" width="9" height="9" fill="#ffb900"/>
            </svg>
            <span>Sign in with Microsoft</span>
          </button>

          <!-- OR Divider -->
          <div v-if="ssoAvailable" class="flex items-center gap-4 my-6">
            <div class="flex-1 h-px bg-gray-200"></div>
            <span class="text-sm text-gray-400 font-medium">OR</span>
            <div class="flex-1 h-px bg-gray-200"></div>
          </div>

          <!-- Email/Password Form -->
          <form @submit.prevent="handleLogin" class="space-y-5">
            <!-- Error Message -->
            <div v-if="formError" class="p-4 bg-red-50 border border-red-100 rounded-xl text-sm text-red-600 flex items-center gap-3">
              <i class="fas fa-exclamation-circle"></i>
              <span>{{ formError }}</span>
            </div>

            <!-- Email Input -->
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Email Address</label>
              <div class="relative">
                <i class="fas fa-envelope absolute left-4 top-1/2 -translate-y-1/2 text-gray-400"></i>
                <input
                  v-model="email"
                  type="email"
                  placeholder="you@company.com"
                  class="w-full pl-11 pr-4 py-3.5 bg-gray-50 border border-gray-200 rounded-xl text-gray-900 placeholder-gray-400 focus:bg-white focus:border-teal-400 focus:ring-2 focus:ring-teal-100 outline-none transition-all"
                  :disabled="isSubmitting"
                >
              </div>
            </div>

            <!-- Password Input -->
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Password</label>
              <div class="relative">
                <i class="fas fa-lock absolute left-4 top-1/2 -translate-y-1/2 text-gray-400"></i>
                <input
                  v-model="password"
                  :type="showPassword ? 'text' : 'password'"
                  placeholder="Enter your password"
                  class="w-full pl-11 pr-12 py-3.5 bg-gray-50 border border-gray-200 rounded-xl text-gray-900 placeholder-gray-400 focus:bg-white focus:border-teal-400 focus:ring-2 focus:ring-teal-100 outline-none transition-all"
                  :disabled="isSubmitting"
                >
                <button
                  type="button"
                  @click="showPassword = !showPassword"
                  class="absolute right-4 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600"
                >
                  <i :class="showPassword ? 'fas fa-eye-slash' : 'fas fa-eye'"></i>
                </button>
              </div>
            </div>

            <!-- Remember Me & Forgot Password -->
            <div class="flex items-center justify-between">
              <label class="flex items-center gap-2 cursor-pointer">
                <input
                  v-model="rememberMe"
                  type="checkbox"
                  class="w-4 h-4 rounded border-gray-300 text-teal-600 focus:ring-teal-500"
                >
                <span class="text-sm text-gray-600">Remember me</span>
              </label>
              <a href="#" class="text-sm text-teal-600 hover:text-teal-700 font-medium">
                Forgot password?
              </a>
            </div>

            <!-- Submit Button -->
            <button
              type="submit"
              :disabled="isSubmitting"
              class="w-full py-4 bg-gradient-to-r from-teal-500 to-teal-600 text-white font-semibold rounded-xl shadow-lg shadow-teal-500/25 hover:shadow-teal-500/40 hover:from-teal-600 hover:to-teal-700 transition-all disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center gap-2"
            >
              <template v-if="isSubmitting">
                <div class="w-5 h-5 border-2 border-white/30 border-t-white rounded-full animate-spin"></div>
                <span>Signing in...</span>
              </template>
              <template v-else>
                <i class="fas fa-sign-in-alt"></i>
                <span>Sign In</span>
              </template>
            </button>
          </form>

          <!-- Demo credentials hint (development only) -->
          <div v-if="true" class="mt-6 p-4 bg-blue-50 border border-blue-100 rounded-xl">
            <p class="text-xs text-blue-600 font-medium mb-2">
              <i class="fas fa-info-circle mr-1"></i> Demo Mode
            </p>
            <p class="text-xs text-blue-500">
              Use any email/password to sign in (mock authentication enabled)
            </p>
          </div>
        </template>
      </div>

      <!-- Footer -->
      <div class="text-center mt-6">
        <p class="text-sm text-gray-500">
          Powered by <span class="font-semibold text-teal-600">Intalio</span>
        </p>
        <p class="text-xs text-gray-400 mt-1">Enterprise Knowledge Management Platform</p>
      </div>
    </div>
  </div>
</template>
