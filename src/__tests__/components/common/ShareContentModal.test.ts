import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import ShareContentModal from '@/components/common/ShareContentModal.vue'

// Mock vue-i18n
vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string) => key,
  }),
}))

// Mock sharing composable
const mockCopyLink = vi.fn()
const mockShareToTwitter = vi.fn()
const mockShareToLinkedIn = vi.fn()
const mockShareToFacebook = vi.fn()
const mockShareViaEmail = vi.fn()

vi.mock('@/composables/useSharing', () => ({
  useSharing: () => ({
    copyLink: mockCopyLink,
    shareToTwitter: mockShareToTwitter,
    shareToLinkedIn: mockShareToLinkedIn,
    shareToFacebook: mockShareToFacebook,
    shareViaEmail: mockShareViaEmail,
    copySuccess: false,
  }),
}))

// Mock window.open
const mockWindowOpen = vi.fn()
Object.defineProperty(window, 'open', { value: mockWindowOpen })

function mountComponent(props = {}) {
  return shallowMount(ShareContentModal, {
    props: {
      modelValue: true,
      title: 'Test Article',
      ...props,
    },
    global: {
      mocks: {
        $t: (key: string) => key,
      },
      stubs: {
        Teleport: true,
        Transition: true,
      },
    },
  })
}

describe('ShareContentModal', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('should render when modelValue is true', () => {
      const wrapper = mountComponent({ modelValue: true })
      expect(wrapper.exists()).toBe(true)
    })

    it('should not render content when modelValue is false', () => {
      const wrapper = mountComponent({ modelValue: false })
      expect(wrapper.find('.modal-content').exists()).toBe(false)
    })

    it('should render header', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('common.share')
    })

    it('should render close button', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.fa-times').exists()).toBe(true)
    })
  })

  describe('Content Preview', () => {
    it('should display title', () => {
      const wrapper = mountComponent({ title: 'My Article' })
      expect(wrapper.text()).toContain('My Article')
    })

    it('should display description when provided', () => {
      const wrapper = mountComponent({ description: 'Article description' })
      expect(wrapper.text()).toContain('Article description')
    })

    it('should display image when provided', () => {
      const wrapper = mountComponent({ image: 'https://example.com/image.jpg' })
      const img = wrapper.find('img[alt="Test Article"]')
      expect(img.exists()).toBe(true)
    })

    it('should display fallback icon when no image', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.fa-share-nodes').exists()).toBe(true)
    })
  })

  describe('Share Options', () => {
    it('should have 5 share options', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.shareOptions.length).toBe(5)
    })

    it('should have Twitter option', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.shareOptions.find((o: any) => o.id === 'twitter')).toBeTruthy()
    })

    it('should have LinkedIn option', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.shareOptions.find((o: any) => o.id === 'linkedin')).toBeTruthy()
    })

    it('should have Facebook option', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.shareOptions.find((o: any) => o.id === 'facebook')).toBeTruthy()
    })

    it('should have WhatsApp option', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.shareOptions.find((o: any) => o.id === 'whatsapp')).toBeTruthy()
    })

    it('should have Email option', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.shareOptions.find((o: any) => o.id === 'email')).toBeTruthy()
    })
  })

  describe('Social Icons', () => {
    it('should render Twitter icon', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.fa-x-twitter').exists()).toBe(true)
    })

    it('should render LinkedIn icon', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.fa-linkedin-in').exists()).toBe(true)
    })

    it('should render Facebook icon', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.fa-facebook-f').exists()).toBe(true)
    })

    it('should render WhatsApp icon', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.fa-whatsapp').exists()).toBe(true)
    })

    it('should render Email icon', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.fa-envelope').exists()).toBe(true)
    })
  })

  describe('Copy Link', () => {
    it('should render copy link section', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('common.copyLink')
    })

    it('should display share URL', () => {
      const wrapper = mountComponent({ url: 'https://example.com/article' })
      expect(wrapper.text()).toContain('https://example.com/article')
    })

    it('should call copyLink on click', () => {
      const wrapper = mountComponent({ url: 'https://example.com/article' })
      const vm = wrapper.vm as any
      vm.handleCopyLink()
      expect(mockCopyLink).toHaveBeenCalledWith('https://example.com/article')
    })

    it('should show copy button', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.fa-copy').exists()).toBe(true)
    })
  })

  describe('QR Code', () => {
    it('should have QR toggle button', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.fa-qrcode').exists()).toBe(true)
    })

    it('should toggle QR code visibility', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.showQR).toBe(false)
      vm.toggleQR()
      expect(vm.showQR).toBe(true)
    })

    it('should compute QR code URL', () => {
      const wrapper = mountComponent({ url: 'https://example.com' })
      const vm = wrapper.vm as any
      expect(vm.qrCodeUrl).toContain('api.qrserver.com')
      expect(vm.qrCodeUrl).toContain('https%3A%2F%2Fexample.com')
    })
  })

  describe('Close', () => {
    it('should emit update:modelValue false when close called', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.close()
      expect(wrapper.emitted('update:modelValue')).toBeTruthy()
      expect(wrapper.emitted('update:modelValue')![0]).toEqual([false])
    })

    it('should reset QR view on close', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.showQR = true
      vm.close()
      expect(vm.showQR).toBe(false)
    })
  })

  describe('WhatsApp Share', () => {
    it('should have shareToWhatsApp function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.shareToWhatsApp).toBe('function')
    })

    it('should open WhatsApp share link', () => {
      const wrapper = mountComponent({ title: 'Test', url: 'https://example.com' })
      const vm = wrapper.vm as any
      vm.shareToWhatsApp()
      expect(mockWindowOpen).toHaveBeenCalled()
      expect(mockWindowOpen.mock.calls[0][0]).toContain('wa.me')
    })
  })

  describe('Share Data', () => {
    it('should compute share data correctly', () => {
      const wrapper = mountComponent({
        title: 'Test Article',
        description: 'Test description',
        url: 'https://example.com',
      })
      const vm = wrapper.vm as any
      expect(vm.shareData).toEqual({
        title: 'Test Article',
        description: 'Test description',
        url: 'https://example.com',
      })
    })

    it('should use window.location.href as default URL', () => {
      const wrapper = mountComponent({ title: 'Test' })
      const vm = wrapper.vm as any
      expect(vm.shareUrl).toBe(window.location.href)
    })
  })

  describe('Content Type', () => {
    it('should display content type', () => {
      const wrapper = mountComponent({ contentType: 'article' })
      expect(wrapper.text()).toContain('article')
    })

    it('should use default content type', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.contentType).toBe('content')
    })
  })

  describe('Footer', () => {
    it('should render footer with close button', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('common.close')
    })
  })

  describe('Watch isOpen', () => {
    it('should reset QR view when modal closes', async () => {
      const wrapper = mountComponent({ modelValue: true })
      const vm = wrapper.vm as any
      vm.showQR = true
      await wrapper.setProps({ modelValue: false })
      expect(vm.showQR).toBe(false)
    })
  })
})
