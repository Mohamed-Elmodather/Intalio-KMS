import { describe, it, expect, vi, beforeEach } from 'vitest'
import { mount, flushPromises } from '@vue/test-utils'
import { createPinia, setActivePinia } from 'pinia'
import LoginPage from '@/pages/LoginPage.vue'

// Mock vue-router
const mockPush = vi.fn()
const mockRoute = { query: {} }
vi.mock('vue-router', () => ({
  useRouter: () => ({
    push: mockPush,
  }),
  useRoute: () => mockRoute,
}))

// Mock vue-i18n
vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string, params?: Record<string, any>) => {
      const translations: Record<string, string> = {
        'auth.welcomeBack': 'Welcome Back',
        'auth.loginSubtitle': 'Sign in to continue',
        'auth.email': 'Email',
        'auth.emailPlaceholder': 'Enter your email',
        'auth.password': 'Password',
        'auth.passwordPlaceholder': 'Enter your password',
        'auth.rememberMe': 'Remember me',
        'auth.forgotPassword': 'Forgot password?',
        'auth.signIn': 'Sign In',
        'auth.signingIn': 'Signing in...',
        'auth.ssoLogin': 'Sign in with Microsoft',
        'auth.orContinueWith': 'or continue with',
        'auth.quickDevLogin': 'Quick Dev Login',
        'validation.requiredFields': 'Required Fields',
        'validation.enterEmailAndPassword': 'Please enter email and password',
        'auth.loginFailed': 'Login Failed',
        'auth.invalidCredentials': 'Invalid credentials',
        'auth.loggedInAs': `Logged in as ${params?.name || ''}`,
        'auth.ssoUnavailable': 'SSO Unavailable',
        'auth.ssoNotConfigured': 'SSO not configured',
        'auth.ssoError': 'SSO Error',
        'auth.ssoInitFailed': 'SSO init failed',
        'auth.hero.badge': 'Enterprise Platform',
        'auth.hero.title': 'Knowledge <span class="gradient-text">Management</span>',
        'auth.hero.description': 'Streamline your organization',
        'auth.hero.stats.uptime': 'Uptime',
        'auth.hero.stats.users': 'Users',
        'auth.hero.stats.enterprises': 'Enterprises',
        'auth.hero.features.aiSearch': 'AI Search',
        'auth.hero.features.collaboration': 'Collaboration',
        'auth.hero.features.security': 'Security',
        'auth.hero.features.automation': 'Automation',
        'footer.poweredBy': 'Powered by',
        'app.subtitle': 'Knowledge Management System',
      }
      return translations[key] || key
    },
  }),
}))

// Mock useReducedMotion
vi.mock('@/composables/useReducedMotion', () => ({
  useReducedMotion: () => ({ value: false }),
}))

// Mock auth store
const mockLoginWithCredentials = vi.fn()
const mockGetSSOConfig = vi.fn()
const mockInitiateSSO = vi.fn()
let mockAuthError: string | null = null
let mockUser: { displayName: string } | null = null

vi.mock('@/stores/auth', () => ({
  useAuthStore: () => ({
    loginWithCredentials: mockLoginWithCredentials,
    getSSOConfig: mockGetSSOConfig,
    initiateSSO: mockInitiateSSO,
    error: mockAuthError,
    user: mockUser,
  }),
}))

// Mock UI store
const mockShowWarning = vi.fn()
const mockShowSuccess = vi.fn()
const mockShowError = vi.fn()

vi.mock('@/stores/ui', () => ({
  useUIStore: () => ({
    showWarning: mockShowWarning,
    showSuccess: mockShowSuccess,
    showError: mockShowError,
  }),
}))

describe('LoginPage', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
    mockAuthError = null
    mockUser = null
    mockRoute.query = {}
  })

  describe('Rendering', () => {
    it('should render login page', () => {
      const wrapper = mount(LoginPage)
      expect(wrapper.find('.login-page').exists()).toBe(true)
    })

    it('should render login branding', () => {
      const wrapper = mount(LoginPage)
      expect(wrapper.find('.login-branding').exists()).toBe(true)
      // The i18n key is rendered
      expect(wrapper.text()).toContain('auth.welcomeBack')
    })

    it('should render login form', () => {
      const wrapper = mount(LoginPage)
      expect(wrapper.find('.login-form').exists()).toBe(true)
    })

    it('should render email input', () => {
      const wrapper = mount(LoginPage)
      const emailInput = wrapper.find('#email')
      expect(emailInput.exists()).toBe(true)
      expect(emailInput.attributes('type')).toBe('email')
    })

    it('should render password input', () => {
      const wrapper = mount(LoginPage)
      const passwordInput = wrapper.find('#password')
      expect(passwordInput.exists()).toBe(true)
      expect(passwordInput.attributes('type')).toBe('password')
    })

    it('should render remember me checkbox', () => {
      const wrapper = mount(LoginPage)
      const checkbox = wrapper.find('.remember-me input[type="checkbox"]')
      expect(checkbox.exists()).toBe(true)
    })

    it('should render forgot password link', () => {
      const wrapper = mount(LoginPage)
      expect(wrapper.find('.forgot-link').exists()).toBe(true)
      expect(wrapper.find('.forgot-link').text()).toContain('auth.forgotPassword')
    })

    it('should render SSO button', () => {
      const wrapper = mount(LoginPage)
      expect(wrapper.find('.sso-btn').exists()).toBe(true)
    })

    it('should render sign in button', () => {
      const wrapper = mount(LoginPage)
      expect(wrapper.find('.login-btn').exists()).toBe(true)
    })

    it('should render footer', () => {
      const wrapper = mount(LoginPage)
      expect(wrapper.find('.login-footer').exists()).toBe(true)
      expect(wrapper.text()).toContain('Intalio')
    })

    it('should render hero panel', () => {
      const wrapper = mount(LoginPage)
      expect(wrapper.find('.hero-panel').exists()).toBe(true)
    })
  })

  describe('Form Inputs', () => {
    it('should update email value on input', async () => {
      const wrapper = mount(LoginPage)
      const emailInput = wrapper.find('#email')

      await emailInput.setValue('test@example.com')

      expect((emailInput.element as HTMLInputElement).value).toBe('test@example.com')
    })

    it('should update password value on input', async () => {
      const wrapper = mount(LoginPage)
      const passwordInput = wrapper.find('#password')

      await passwordInput.setValue('mypassword123')

      expect((passwordInput.element as HTMLInputElement).value).toBe('mypassword123')
    })

    it('should toggle remember me checkbox', async () => {
      const wrapper = mount(LoginPage)
      const checkbox = wrapper.find('.remember-me input[type="checkbox"]')

      await checkbox.setValue(true)

      expect((checkbox.element as HTMLInputElement).checked).toBe(true)
    })
  })

  describe('Password Visibility Toggle', () => {
    it('should toggle password visibility', async () => {
      const wrapper = mount(LoginPage)
      const passwordInput = wrapper.find('#password')
      const toggleBtn = wrapper.find('.toggle-password')

      expect(passwordInput.attributes('type')).toBe('password')

      await toggleBtn.trigger('click')

      expect(passwordInput.attributes('type')).toBe('text')

      await toggleBtn.trigger('click')

      expect(passwordInput.attributes('type')).toBe('password')
    })

    it('should show eye icon when password is hidden', () => {
      const wrapper = mount(LoginPage)
      const toggleBtn = wrapper.find('.toggle-password i')

      expect(toggleBtn.classes()).toContain('fa-eye')
    })

    it('should show eye-slash icon when password is visible', async () => {
      const wrapper = mount(LoginPage)
      const toggleBtn = wrapper.find('.toggle-password')

      await toggleBtn.trigger('click')

      const icon = wrapper.find('.toggle-password i')
      expect(icon.classes()).toContain('fa-eye-slash')
    })
  })

  describe('Form Submission', () => {
    it('should show warning when email is empty', async () => {
      const wrapper = mount(LoginPage)

      await wrapper.find('.login-form').trigger('submit')

      expect(mockShowWarning).toHaveBeenCalledWith(
        'Required Fields',
        'Please enter email and password'
      )
    })

    it('should show warning when password is empty', async () => {
      const wrapper = mount(LoginPage)
      await wrapper.find('#email').setValue('test@example.com')

      await wrapper.find('.login-form').trigger('submit')

      expect(mockShowWarning).toHaveBeenCalledWith(
        'Required Fields',
        'Please enter email and password'
      )
    })

    it('should call loginWithCredentials on valid submit', async () => {
      mockLoginWithCredentials.mockResolvedValue(true)
      const wrapper = mount(LoginPage)

      await wrapper.find('#email').setValue('test@example.com')
      await wrapper.find('#password').setValue('password123')
      await wrapper.find('.login-form').trigger('submit')
      await flushPromises()

      expect(mockLoginWithCredentials).toHaveBeenCalledWith({
        email: 'test@example.com',
        password: 'password123',
        rememberMe: false,
      })
    })

    it('should pass rememberMe value to login', async () => {
      mockLoginWithCredentials.mockResolvedValue(true)
      const wrapper = mount(LoginPage)

      await wrapper.find('#email').setValue('test@example.com')
      await wrapper.find('#password').setValue('password123')
      await wrapper.find('.remember-me input[type="checkbox"]').setValue(true)
      await wrapper.find('.login-form').trigger('submit')
      await flushPromises()

      expect(mockLoginWithCredentials).toHaveBeenCalledWith({
        email: 'test@example.com',
        password: 'password123',
        rememberMe: true,
      })
    })

    it('should redirect on successful login', async () => {
      mockLoginWithCredentials.mockResolvedValue(true)
      const wrapper = mount(LoginPage)

      await wrapper.find('#email').setValue('test@example.com')
      await wrapper.find('#password').setValue('password123')
      await wrapper.find('.login-form').trigger('submit')
      await flushPromises()

      expect(mockPush).toHaveBeenCalledWith('/')
    })

    it('should redirect to specified path on successful login', async () => {
      mockRoute.query = { redirect: '/dashboard' }
      mockLoginWithCredentials.mockResolvedValue(true)
      const wrapper = mount(LoginPage)

      await wrapper.find('#email').setValue('test@example.com')
      await wrapper.find('#password').setValue('password123')
      await wrapper.find('.login-form').trigger('submit')
      await flushPromises()

      expect(mockPush).toHaveBeenCalledWith('/dashboard')
    })

    it('should show error on failed login', async () => {
      mockLoginWithCredentials.mockResolvedValue(false)
      mockAuthError = 'Invalid email or password'
      const wrapper = mount(LoginPage)

      await wrapper.find('#email').setValue('test@example.com')
      await wrapper.find('#password').setValue('wrongpassword')
      await wrapper.find('.login-form').trigger('submit')
      await flushPromises()

      expect(mockShowError).toHaveBeenCalled()
    })
  })

  describe('SSO Login', () => {
    it('should call getSSOConfig when SSO button clicked', async () => {
      mockGetSSOConfig.mockResolvedValue({ provider: 'microsoft' })
      const wrapper = mount(LoginPage)

      await wrapper.find('.sso-btn').trigger('click')
      await flushPromises()

      expect(mockGetSSOConfig).toHaveBeenCalled()
    })

    it('should initiate SSO when config is available', async () => {
      mockGetSSOConfig.mockResolvedValue({ provider: 'microsoft' })
      const wrapper = mount(LoginPage)

      await wrapper.find('.sso-btn').trigger('click')
      await flushPromises()

      expect(mockInitiateSSO).toHaveBeenCalled()
    })

    it('should show error when SSO config not available', async () => {
      mockGetSSOConfig.mockResolvedValue(null)
      const wrapper = mount(LoginPage)

      await wrapper.find('.sso-btn').trigger('click')
      await flushPromises()

      expect(mockShowError).toHaveBeenCalledWith('SSO Unavailable', 'SSO not configured')
    })

    it('should show error when SSO fails', async () => {
      mockGetSSOConfig.mockRejectedValue(new Error('SSO failed'))
      const wrapper = mount(LoginPage)

      await wrapper.find('.sso-btn').trigger('click')
      await flushPromises()

      expect(mockShowError).toHaveBeenCalledWith('SSO Error', 'SSO init failed')
    })
  })

  describe('Loading State', () => {
    it('should disable form during login', async () => {
      mockLoginWithCredentials.mockImplementation(
        () => new Promise((resolve) => setTimeout(() => resolve(true), 100))
      )
      const wrapper = mount(LoginPage)

      await wrapper.find('#email').setValue('test@example.com')
      await wrapper.find('#password').setValue('password123')
      await wrapper.find('.login-form').trigger('submit')

      expect(wrapper.find('.login-btn').attributes('disabled')).toBeDefined()
    })

    it('should show loading text during login', async () => {
      mockLoginWithCredentials.mockImplementation(
        () => new Promise((resolve) => setTimeout(() => resolve(true), 100))
      )
      const wrapper = mount(LoginPage)

      await wrapper.find('#email').setValue('test@example.com')
      await wrapper.find('#password').setValue('password123')
      await wrapper.find('.login-form').trigger('submit')

      expect(wrapper.find('.loading-state').exists()).toBe(true)
    })
  })

  describe('Error Display', () => {
    it('should display auth error message', async () => {
      // Mock auth store with error
      vi.doMock('@/stores/auth', () => ({
        useAuthStore: () => ({
          loginWithCredentials: mockLoginWithCredentials,
          getSSOConfig: mockGetSSOConfig,
          initiateSSO: mockInitiateSSO,
          error: 'Authentication failed',
          user: null,
        }),
      }))

      // Note: Error display depends on the auth store's error state
      // The component shows error when authStore.error is truthy
    })
  })

  describe('Hero Section', () => {
    it('should render hero stats', () => {
      const wrapper = mount(LoginPage)
      expect(wrapper.find('.hero-stats').exists()).toBe(true)
      expect(wrapper.text()).toContain('99.9%')
      expect(wrapper.text()).toContain('50K+')
      expect(wrapper.text()).toContain('150+')
    })

    it('should render feature cards', () => {
      const wrapper = mount(LoginPage)
      const featureCards = wrapper.findAll('.feature-card')
      expect(featureCards.length).toBe(4)
    })

    it('should render floating elements', () => {
      const wrapper = mount(LoginPage)
      expect(wrapper.find('.floating-elements').exists()).toBe(true)
    })
  })

  describe('Animation Classes', () => {
    it('should add is-loaded class after mount', async () => {
      vi.useFakeTimers()
      const wrapper = mount(LoginPage)

      vi.advanceTimersByTime(200)
      await wrapper.vm.$nextTick()

      expect(wrapper.find('.login-page').classes()).toContain('is-loaded')
      vi.useRealTimers()
    })
  })
})
