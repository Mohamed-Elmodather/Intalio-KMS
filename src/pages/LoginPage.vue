<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { useUIStore } from '@/stores/ui'
import { useReducedMotion } from '@/composables/useReducedMotion'

const router = useRouter()
const route = useRoute()
const authStore = useAuthStore()
const uiStore = useUIStore()

const prefersReducedMotion = useReducedMotion()
const shouldAnimate = computed(() => !prefersReducedMotion.value)

const email = ref('')
const password = ref('')
const rememberMe = ref(false)
const showPassword = ref(false)
const isLoading = ref(false)
const isPageLoaded = ref(false)

onMounted(() => {
  if (shouldAnimate.value) {
    setTimeout(() => {
      isPageLoaded.value = true
    }, 100)
  } else {
    isPageLoaded.value = true
  }
})

const redirectPath = computed(() => {
  const redirect = route.query.redirect as string
  return redirect || '/'
})

const isDev = import.meta.env.DEV

async function handleDevLogin() {
  isLoading.value = true

  try {
    email.value = 'ahmed.alharbi@afc2027.com'
    password.value = 'dev-password'

    const success = await authStore.loginWithCredentials({
      email: email.value,
      password: password.value,
      rememberMe: rememberMe.value
    })

    if (success) {
      router.push(redirectPath.value)
    }
  } finally {
    isLoading.value = false
  }
}

async function handleLogin() {
  if (!email.value || !password.value) {
    uiStore.showWarning('Required Fields', 'Please enter both email and password')
    return
  }

  isLoading.value = true

  try {
    const success = await authStore.loginWithCredentials({
      email: email.value,
      password: password.value,
      rememberMe: rememberMe.value
    })

    if (success) {
      uiStore.showSuccess('Welcome back!', `Logged in as ${authStore.user?.displayName}`)
      router.push(redirectPath.value)
    } else {
      uiStore.showError('Login Failed', authStore.error || 'Invalid email or password')
    }
  } finally {
    isLoading.value = false
  }
}

async function handleSSOLogin() {
  isLoading.value = true

  try {
    const config = await authStore.getSSOConfig()
    if (config) {
      authStore.initiateSSO()
    } else {
      uiStore.showError('SSO Unavailable', 'Single Sign-On is not configured')
      isLoading.value = false
    }
  } catch {
    uiStore.showError('SSO Error', 'Failed to initiate SSO login')
    isLoading.value = false
  }
}
</script>

<template>
  <div class="login-page" :class="{ 'is-loaded': isPageLoaded }">
    <!-- Left Panel - Login Form -->
    <div class="login-panel">
      <!-- Login Container -->
      <div class="login-container">
        <!-- Logo & Branding -->
        <div class="login-branding">
          <div class="logo-wrapper">
            <img src="@/assets/images/logo.png" alt="Intalio" class="logo" />
          </div>
          <div class="brand-text">
            <h1>Welcome Back</h1>
            <p>Sign in to your Knowledge Hub account</p>
          </div>
        </div>

        <!-- Glass Card -->
        <div class="login-card">
          <!-- Error Message -->
          <div v-if="authStore.error" class="error-message">
            <i class="fas fa-exclamation-circle"></i>
            <span>{{ authStore.error }}</span>
          </div>

          <!-- Dev Login Button -->
          <button
            v-if="isDev"
            class="dev-login-btn"
            :disabled="isLoading"
            @click="handleDevLogin"
          >
            <i class="fas fa-bolt"></i>
            <span>Quick Dev Login</span>
          </button>

          <div v-if="isDev" class="divider">
            <span>or continue with</span>
          </div>

          <!-- SSO Button -->
          <button
            class="sso-btn"
            :disabled="isLoading"
            @click="handleSSOLogin"
          >
            <svg class="microsoft-icon" viewBox="0 0 21 21" xmlns="http://www.w3.org/2000/svg">
              <rect x="1" y="1" width="9" height="9" fill="#f25022"/>
              <rect x="11" y="1" width="9" height="9" fill="#7fba00"/>
              <rect x="1" y="11" width="9" height="9" fill="#00a4ef"/>
              <rect x="11" y="11" width="9" height="9" fill="#ffb900"/>
            </svg>
            <span>Sign in with Microsoft</span>
          </button>

          <div class="divider">
            <span>or sign in with email</span>
          </div>

          <!-- Login Form -->
          <form @submit.prevent="handleLogin" class="login-form">
            <div class="form-group">
              <label for="email">Email Address</label>
              <div class="input-field">
                <span class="field-icon">
                  <i class="fas fa-envelope"></i>
                </span>
                <input
                  id="email"
                  v-model="email"
                  type="email"
                  placeholder="you@company.com"
                  autocomplete="email"
                  :disabled="isLoading"
                />
              </div>
            </div>

            <div class="form-group">
              <label for="password">Password</label>
              <div class="input-field">
                <span class="field-icon">
                  <i class="fas fa-lock"></i>
                </span>
                <input
                  id="password"
                  v-model="password"
                  :type="showPassword ? 'text' : 'password'"
                  placeholder="Enter your password"
                  autocomplete="current-password"
                  :disabled="isLoading"
                />
                <button
                  type="button"
                  class="toggle-password"
                  @click="showPassword = !showPassword"
                  tabindex="-1"
                >
                  <i :class="showPassword ? 'fas fa-eye-slash' : 'fas fa-eye'"></i>
                </button>
              </div>
            </div>

            <div class="form-options">
              <label class="remember-me">
                <input type="checkbox" v-model="rememberMe" :disabled="isLoading" />
                <span>Remember me</span>
              </label>
              <a href="#" class="forgot-link">Forgot password?</a>
            </div>

            <button
              type="submit"
              class="login-btn"
              :disabled="isLoading"
            >
              <span v-if="!isLoading">Sign In</span>
              <span v-else class="loading-state">
                <i class="fas fa-spinner fa-spin"></i>
                Signing in...
              </span>
            </button>
          </form>
        </div>

        <!-- Footer -->
        <div class="login-footer">
          <p>Powered by <strong>Intalio</strong></p>
          <p class="footer-sub">Knowledge Management System</p>
        </div>
      </div>
    </div>

    <!-- Right Panel - Hero Section -->
    <div class="hero-panel">
      <!-- Multi-layer Animated Background -->
      <div class="hero-background">
        <div class="gradient-mesh"></div>
        <div class="orb-container">
          <div class="gradient-orb orb-1"></div>
          <div class="gradient-orb orb-2"></div>
          <div class="gradient-orb orb-3"></div>
          <div class="gradient-orb orb-4"></div>
        </div>
        <div class="grid-pattern"></div>
        <div class="noise-overlay"></div>
      </div>

      <!-- Hero Content -->
      <div class="login-hero-content">
        <div class="hero-badge">
          <span class="badge-dot"></span>
          <span>Enterprise Platform</span>
        </div>

        <h2>Unlock Your<br/><span class="gradient-text">Knowledge Potential</span></h2>
        <p>Transform how your organization captures, shares, and leverages collective intelligence with our AI-powered platform.</p>

        <!-- Stats Row -->
        <div class="hero-stats">
          <div class="stat-item">
            <span class="stat-value">99.9%</span>
            <span class="stat-label">Uptime</span>
          </div>
          <div class="stat-divider"></div>
          <div class="stat-item">
            <span class="stat-value">50K+</span>
            <span class="stat-label">Users</span>
          </div>
          <div class="stat-divider"></div>
          <div class="stat-item">
            <span class="stat-value">150+</span>
            <span class="stat-label">Enterprises</span>
          </div>
        </div>

        <!-- Feature Cards Grid -->
        <div class="feature-grid">
          <div class="feature-card">
            <div class="feature-icon">
              <i class="fas fa-search"></i>
            </div>
            <span>AI-Powered Search</span>
          </div>
          <div class="feature-card">
            <div class="feature-icon">
              <i class="fas fa-share-alt"></i>
            </div>
            <span>Real-time Collaboration</span>
          </div>
          <div class="feature-card">
            <div class="feature-icon">
              <i class="fas fa-shield-alt"></i>
            </div>
            <span>Enterprise Security</span>
          </div>
          <div class="feature-card">
            <div class="feature-icon">
              <i class="fas fa-cogs"></i>
            </div>
            <span>Workflow Automation</span>
          </div>
        </div>
      </div>

      <!-- Floating Elements -->
      <div class="floating-elements">
        <div class="float-element el-1">
          <i class="fas fa-file-pdf"></i>
        </div>
        <div class="float-element el-2">
          <i class="fas fa-folder"></i>
        </div>
        <div class="float-element el-3">
          <i class="fas fa-chart-bar"></i>
        </div>
        <div class="float-element el-4">
          <i class="fas fa-users"></i>
        </div>
        <div class="float-element el-5">
          <i class="fas fa-calendar"></i>
        </div>
      </div>

      <div class="hero-fade"></div>
    </div>
  </div>
</template>

<style lang="scss" scoped>
@use '@/assets/styles/_variables.scss' as *;
@use '@/assets/styles/_animations.scss' as *;

// ============================================
// LOGIN PAGE - WORLD-CLASS DESIGN
// ============================================

.login-page {
  display: flex;
  min-height: 100vh;
  background: $slate-50;
  overflow: hidden;

  &:not(.is-loaded) {
    .login-branding,
    .login-card,
    .login-footer {
      opacity: 0;
      transform: translateY(20px);
    }
    .login-hero-content,
    .floating-elements {
      opacity: 0;
    }
  }

  &.is-loaded {
    .login-branding {
      animation: fadeInUp 600ms $ease-out-expo forwards;
      animation-delay: 100ms;
    }
    .login-card {
      animation: fadeInUp 600ms $ease-out-expo forwards;
      animation-delay: 200ms;
    }
    .login-footer {
      animation: fadeInUp 600ms $ease-out-expo forwards;
      animation-delay: 300ms;
    }
    .login-hero-content {
      animation: fadeInRight 800ms $ease-out-expo forwards;
      animation-delay: 400ms;
    }
    .floating-elements {
      animation: fadeIn 1000ms $ease-out-expo forwards;
      animation-delay: 800ms;
    }
  }
}

// ============================================
// LEFT PANEL - LOGIN FORM
// ============================================

.login-panel {
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  width: 100%;
  min-height: 100vh;
  padding: $spacing-6;
  position: relative;
  z-index: 2;
  background: linear-gradient(180deg, #ffffff 0%, $slate-50 100%);

  @media (min-width: $breakpoint-lg) {
    width: 45%;
    min-width: 500px;
    max-width: 600px;
    padding: $spacing-8;
  }
}

.login-container {
  width: 100%;
  max-width: 400px;
}

// ============================================
// BRANDING
// ============================================

.login-branding {
  text-align: center;
  margin-bottom: $spacing-8;
}

.logo-wrapper {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  margin-bottom: $spacing-6;

  .logo {
    height: 48px;
    width: auto;
  }
}

.brand-text {
  h1 {
    font-size: $font-size-2xl;
    font-weight: $font-weight-bold;
    color: $slate-900;
    margin-bottom: $spacing-2;
    letter-spacing: -0.02em;
  }

  p {
    font-size: $font-size-base;
    color: $slate-500;
  }
}

// ============================================
// LOGIN CARD - GLASS MORPHISM
// ============================================

.login-card {
  background: rgba(255, 255, 255, 0.9);
  backdrop-filter: blur(16px);
  -webkit-backdrop-filter: blur(16px);
  border: 1px solid rgba(255, 255, 255, 0.8);
  padding: $spacing-8;
  border-radius: $radius-2xl;
  box-shadow: $shadow-elevated-lg;
}

.error-message {
  display: flex;
  align-items: center;
  gap: $spacing-3;
  padding: $spacing-4;
  background: $error-50;
  border: 1px solid rgba($error-500, 0.2);
  border-radius: $radius-lg;
  color: $error-700;
  font-size: $font-size-sm;
  margin-bottom: $spacing-6;

  i {
    font-size: 1.25rem;
    color: $error-500;
  }
}

// ============================================
// BUTTONS
// ============================================

.dev-login-btn,
.sso-btn,
.login-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: $spacing-3;
  width: 100%;
  padding: $spacing-4 $spacing-5;
  border: none;
  border-radius: $radius-xl;
  font-size: $font-size-base;
  font-weight: $font-weight-semibold;
  cursor: pointer;
  transition: all $transition-base;

  i {
    font-size: 1rem;
  }
}

.dev-login-btn {
  background: linear-gradient(135deg, $success-500 0%, $success-600 100%);
  color: white;
  box-shadow: 0 4px 14px -3px rgba($success-500, 0.4);

  &:hover:not(:disabled) {
    transform: translateY(-2px);
    box-shadow: 0 8px 20px -4px rgba($success-500, 0.5);
  }

  &:disabled {
    opacity: 0.7;
    cursor: not-allowed;
  }
}

.sso-btn {
  background: $slate-900;
  color: white;
  box-shadow: $shadow-elevated-sm;

  .microsoft-icon {
    width: 20px;
    height: 20px;
  }

  &:hover:not(:disabled) {
    background: $slate-800;
    transform: translateY(-2px);
    box-shadow: $shadow-elevated-md;
  }

  &:disabled {
    opacity: 0.7;
    cursor: not-allowed;
  }
}

.login-btn {
  background: $gradient-primary;
  color: white;
  box-shadow: $shadow-teal-sm;
  margin-top: $spacing-6;

  &:hover:not(:disabled) {
    background: $gradient-primary-hover;
    transform: translateY(-2px);
    box-shadow: $shadow-teal-md;
  }

  &:disabled {
    opacity: 0.7;
    cursor: not-allowed;
    transform: none;
  }

  .loading-state {
    display: flex;
    align-items: center;
    justify-content: center;
    gap: $spacing-2;

    i {
      font-size: 1rem;
    }
  }
}

// ============================================
// DIVIDER
// ============================================

.divider {
  display: flex;
  align-items: center;
  gap: $spacing-4;
  margin: $spacing-6 0;

  &::before,
  &::after {
    content: '';
    flex: 1;
    height: 1px;
    background: $slate-200;
  }

  span {
    font-size: $font-size-sm;
    color: $slate-400;
    white-space: nowrap;
  }
}

// ============================================
// FORM STYLES
// ============================================

.login-form {
  display: flex;
  flex-direction: column;
  gap: $spacing-5;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: $spacing-2;

  label {
    font-size: $font-size-sm;
    font-weight: $font-weight-medium;
    color: $slate-700;
  }
}

.input-field {
  display: flex;
  align-items: center;
  background: $slate-50;
  border: 2px solid $slate-200;
  border-radius: $radius-xl;
  overflow: hidden;
  transition: all $transition-fast;

  &:hover {
    border-color: $slate-300;
    background: $slate-100;
  }

  &:focus-within {
    background: white;
    border-color: $intalio-teal-500;
    box-shadow: $shadow-focus-ring;
  }

  .field-icon {
    display: flex;
    align-items: center;
    justify-content: center;
    width: 52px;
    height: 52px;
    background: rgba($intalio-teal-500, 0.08);
    color: $intalio-teal-600;
    flex-shrink: 0;
    border-inline-end: 1px solid $slate-200;
    transition: all $transition-fast;

    i {
      font-size: 1.1rem;
    }
  }

  &:focus-within .field-icon {
    background: rgba($intalio-teal-500, 0.12);
    color: $intalio-teal-700;
    border-color: transparent;
  }

  input {
    flex: 1;
    padding: $spacing-4;
    font-size: $font-size-base;
    color: $slate-900;
    background: transparent;
    border: none;
    outline: none;

    &::placeholder {
      color: $slate-400;
    }
  }

  .toggle-password {
    display: flex;
    align-items: center;
    justify-content: center;
    width: 48px;
    height: 52px;
    background: transparent;
    border: none;
    color: $slate-400;
    cursor: pointer;
    transition: color $transition-fast;

    &:hover {
      color: $slate-600;
    }
  }
}

.form-options {
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.remember-me {
  display: flex;
  align-items: center;
  gap: $spacing-2;
  cursor: pointer;

  input[type="checkbox"] {
    width: 16px;
    height: 16px;
    accent-color: $intalio-teal-500;
    cursor: pointer;
  }

  span {
    font-size: $font-size-sm;
    color: $slate-600;
  }
}

.forgot-link {
  font-size: $font-size-sm;
  color: $intalio-teal-600;
  font-weight: $font-weight-medium;
  text-decoration: none;
  transition: color $transition-fast;

  &:hover {
    color: $intalio-teal-700;
    text-decoration: underline;
  }
}

// ============================================
// FOOTER
// ============================================

.login-footer {
  text-align: center;
  margin-top: $spacing-8;
  color: $slate-500;

  p {
    font-size: $font-size-sm;
    margin: $spacing-1 0;

    strong {
      color: $intalio-teal-600;
      font-weight: $font-weight-semibold;
    }
  }

  .footer-sub {
    font-size: $font-size-xs;
    color: $slate-400;
  }
}

// ============================================
// RIGHT PANEL - HERO SECTION
// ============================================

.hero-panel {
  display: none;
  position: relative;
  flex: 1;
  background: linear-gradient(135deg, $intalio-teal-900 0%, $intalio-teal-700 50%, $intalio-teal-600 100%);
  overflow: hidden;

  @media (min-width: $breakpoint-lg) {
    display: flex;
    align-items: center;
    justify-content: center;
  }
}

// ============================================
// HERO BACKGROUND - MULTI-LAYER EFFECTS
// ============================================

.hero-background {
  position: absolute;
  inset: 0;
  overflow: hidden;
}

.gradient-mesh {
  position: absolute;
  inset: 0;
  background:
    radial-gradient(ellipse 80% 50% at 20% 40%, rgba($intalio-teal-400, 0.3) 0%, transparent 50%),
    radial-gradient(ellipse 60% 80% at 80% 20%, rgba(#00c9a7, 0.25) 0%, transparent 50%),
    radial-gradient(ellipse 50% 50% at 60% 80%, rgba($intalio-teal-300, 0.2) 0%, transparent 50%);
}

.orb-container {
  position: absolute;
  inset: 0;
}

.gradient-orb {
  position: absolute;
  border-radius: 50%;
  filter: blur(60px);
  opacity: 0.6;

  &.orb-1 {
    width: 400px;
    height: 400px;
    background: radial-gradient(circle, rgba($intalio-teal-300, 0.8) 0%, transparent 70%);
    top: -10%;
    right: -5%;
    animation: float 15s ease-in-out infinite;
  }

  &.orb-2 {
    width: 300px;
    height: 300px;
    background: radial-gradient(circle, rgba(#00c9a7, 0.7) 0%, transparent 70%);
    bottom: 10%;
    left: -5%;
    animation: float 18s ease-in-out infinite reverse;
  }

  &.orb-3 {
    width: 250px;
    height: 250px;
    background: radial-gradient(circle, rgba($intalio-teal-400, 0.6) 0%, transparent 70%);
    top: 40%;
    right: 20%;
    animation: float 12s ease-in-out infinite;
    animation-delay: -3s;
  }

  &.orb-4 {
    width: 200px;
    height: 200px;
    background: radial-gradient(circle, rgba(#00d4aa, 0.5) 0%, transparent 70%);
    bottom: 30%;
    right: 40%;
    animation: float 20s ease-in-out infinite;
    animation-delay: -7s;
  }
}

.grid-pattern {
  position: absolute;
  inset: 0;
  background-image:
    linear-gradient(rgba(255, 255, 255, 0.03) 1px, transparent 1px),
    linear-gradient(90deg, rgba(255, 255, 255, 0.03) 1px, transparent 1px);
  background-size: 40px 40px;
  mask-image: radial-gradient(ellipse 80% 80% at 50% 50%, black 40%, transparent 100%);
}

.noise-overlay {
  position: absolute;
  inset: 0;
  opacity: 0.03;
  background-image: url("data:image/svg+xml,%3Csvg viewBox='0 0 256 256' xmlns='http://www.w3.org/2000/svg'%3E%3Cfilter id='noise'%3E%3CfeTurbulence type='fractalNoise' baseFrequency='0.9' numOctaves='4' stitchTiles='stitch'/%3E%3C/filter%3E%3Crect width='100%25' height='100%25' filter='url(%23noise)'/%3E%3C/svg%3E");
}

.hero-fade {
  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
  height: 200px;
  background: linear-gradient(to top, rgba($intalio-teal-900, 0.5) 0%, transparent 100%);
  pointer-events: none;
}

// ============================================
// HERO CONTENT
// ============================================

.login-hero-content {
  position: relative;
  z-index: 2;
  text-align: center;
  padding: $spacing-8;
  max-width: 520px;
}

.hero-badge {
  display: inline-flex;
  align-items: center;
  gap: $spacing-2;
  padding: $spacing-2 $spacing-4;
  background: rgba(white, 0.1);
  backdrop-filter: blur(8px);
  border: 1px solid rgba(white, 0.15);
  border-radius: $radius-full;
  margin-bottom: $spacing-6;

  .badge-dot {
    width: 8px;
    height: 8px;
    background: #00ff9d;
    border-radius: 50%;
    animation: pulseSoft 2s ease-in-out infinite;
    box-shadow: 0 0 10px rgba(#00ff9d, 0.5);
  }

  span {
    font-size: $font-size-sm;
    font-weight: $font-weight-medium;
    color: rgba(white, 0.9);
    letter-spacing: 0.02em;
  }
}

.login-hero-content h2 {
  font-size: 2.75rem;
  font-weight: $font-weight-bold;
  color: white;
  line-height: 1.15;
  margin-bottom: $spacing-5;
  letter-spacing: -0.03em;

  .gradient-text {
    background: linear-gradient(135deg, #00ff9d 0%, #00d4aa 50%, #4dffd4 100%);
    -webkit-background-clip: text;
    -webkit-text-fill-color: transparent;
    background-clip: text;
  }
}

.login-hero-content > p {
  font-size: $font-size-base;
  color: rgba(white, 0.75);
  line-height: 1.7;
  margin-bottom: $spacing-8;
}

// ============================================
// HERO STATS
// ============================================

.hero-stats {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: $spacing-6;
  margin-bottom: $spacing-8;
  padding: $spacing-5 $spacing-6;
  background: rgba(white, 0.08);
  backdrop-filter: blur(12px);
  border: 1px solid rgba(white, 0.1);
  border-radius: $radius-2xl;
}

.stat-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: $spacing-1;
}

.stat-value {
  font-size: $font-size-2xl;
  font-weight: $font-weight-bold;
  color: white;
  letter-spacing: -0.02em;
}

.stat-label {
  font-size: $font-size-xs;
  color: rgba(white, 0.6);
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.stat-divider {
  width: 1px;
  height: 40px;
  background: rgba(white, 0.15);
}

// ============================================
// FEATURE GRID
// ============================================

.feature-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: $spacing-3;
}

.feature-card {
  display: flex;
  align-items: center;
  gap: $spacing-3;
  padding: $spacing-4;
  background: rgba(white, 0.06);
  backdrop-filter: blur(8px);
  border: 1px solid rgba(white, 0.08);
  border-radius: $radius-xl;
  transition: all $transition-base;

  &:hover {
    background: rgba(white, 0.12);
    border-color: rgba(white, 0.15);
    transform: translateY(-2px);
  }

  .feature-icon {
    display: flex;
    align-items: center;
    justify-content: center;
    width: 40px;
    height: 40px;
    background: rgba(white, 0.1);
    border-radius: $radius-lg;
    flex-shrink: 0;

    i {
      font-size: 1rem;
      color: #00ff9d;
    }
  }

  span {
    font-size: $font-size-sm;
    font-weight: $font-weight-medium;
    color: rgba(white, 0.9);
    text-align: left;
  }
}

// ============================================
// FLOATING ELEMENTS
// ============================================

.floating-elements {
  position: absolute;
  inset: 0;
  pointer-events: none;
  overflow: hidden;
}

.float-element {
  position: absolute;
  display: flex;
  align-items: center;
  justify-content: center;
  width: 48px;
  height: 48px;
  background: rgba(white, 0.08);
  backdrop-filter: blur(8px);
  border: 1px solid rgba(white, 0.1);
  border-radius: $radius-xl;
  color: rgba(white, 0.6);

  i {
    font-size: 1.25rem;
  }

  &.el-1 {
    top: 12%;
    left: 10%;
    animation: float 8s ease-in-out infinite;
  }

  &.el-2 {
    top: 20%;
    right: 15%;
    animation: float 10s ease-in-out infinite;
    animation-delay: -2s;
  }

  &.el-3 {
    bottom: 35%;
    left: 8%;
    animation: float 9s ease-in-out infinite;
    animation-delay: -4s;
  }

  &.el-4 {
    bottom: 15%;
    right: 12%;
    animation: float 7s ease-in-out infinite;
    animation-delay: -1s;
  }

  &.el-5 {
    top: 50%;
    right: 8%;
    animation: float 11s ease-in-out infinite;
    animation-delay: -6s;
  }
}

// ============================================
// MOBILE RESPONSIVE
// ============================================

@media (max-width: #{$breakpoint-lg - 1px}) {
  .login-panel {
    padding: $spacing-6 $spacing-4;

    &::before {
      content: '';
      position: absolute;
      top: 0;
      left: 0;
      right: 0;
      height: 200px;
      background: $gradient-primary-subtle;
      z-index: -1;
    }
  }

  .login-card {
    padding: $spacing-6;
  }

  .brand-text h1 {
    font-size: $font-size-xl;
  }
}

// ============================================
// REDUCED MOTION SUPPORT
// ============================================

@media (prefers-reduced-motion: reduce) {
  .login-page {
    &.is-loaded {
      .login-branding,
      .login-card,
      .login-footer,
      .login-hero-content,
      .floating-elements {
        animation: none !important;
        opacity: 1;
        transform: none;
      }
    }
  }

  .gradient-orb,
  .float-element {
    animation: none !important;
  }

  .hero-badge .badge-dot {
    animation: none !important;
  }

  .feature-card {
    &:hover {
      transform: none;
    }
  }
}
</style>
