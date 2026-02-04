import { describe, it, expect, vi, beforeEach } from 'vitest'
import { mount } from '@vue/test-utils'
import { ref } from 'vue'
import SocialShareButtons from '@/components/common/SocialShareButtons.vue'

// Mock vue-i18n
vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string) => {
      const translations: Record<string, string> = {
        'common.email': 'Email',
        'common.shareOn': 'Share on {name}',
        'common.copied': 'Copied!',
        'common.copyLink': 'Copy link',
        'common.copy': 'Copy',
      }
      return translations[key] || key
    },
  }),
}))

// Mock the useSharing composable
const mockCopyLink = vi.fn()
const mockShareToTwitter = vi.fn()
const mockShareToLinkedIn = vi.fn()
const mockShareToFacebook = vi.fn()
const mockShareViaEmail = vi.fn()
const mockCopySuccess = ref(false)

vi.mock('@/composables/useSharing', () => ({
  useSharing: () => ({
    copyLink: mockCopyLink,
    shareToTwitter: mockShareToTwitter,
    shareToLinkedIn: mockShareToLinkedIn,
    shareToFacebook: mockShareToFacebook,
    shareViaEmail: mockShareViaEmail,
    copySuccess: mockCopySuccess,
  }),
}))

describe('SocialShareButtons', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    mockCopySuccess.value = false
  })

  const defaultProps = {
    title: 'Test Article',
    description: 'Article description',
    url: 'https://example.com/article',
  }

  describe('Rendering', () => {
    it('should render social share buttons container', () => {
      const wrapper = mount(SocialShareButtons, {
        props: defaultProps,
        global: {
          mocks: {
            $t: (key: string) => key,
          },
        },
      })
      expect(wrapper.find('.social-share-buttons').exists()).toBe(true)
    })

    it('should render Twitter button', () => {
      const wrapper = mount(SocialShareButtons, {
        props: defaultProps,
        global: {
          mocks: {
            $t: (key: string) => key,
          },
        },
      })
      expect(wrapper.find('.fa-twitter').exists()).toBe(true)
    })

    it('should render LinkedIn button', () => {
      const wrapper = mount(SocialShareButtons, {
        props: defaultProps,
        global: {
          mocks: {
            $t: (key: string) => key,
          },
        },
      })
      expect(wrapper.find('.fa-linkedin-in').exists()).toBe(true)
    })

    it('should render Facebook button', () => {
      const wrapper = mount(SocialShareButtons, {
        props: defaultProps,
        global: {
          mocks: {
            $t: (key: string) => key,
          },
        },
      })
      expect(wrapper.find('.fa-facebook-f').exists()).toBe(true)
    })

    it('should render Email button', () => {
      const wrapper = mount(SocialShareButtons, {
        props: defaultProps,
        global: {
          mocks: {
            $t: (key: string) => key,
          },
        },
      })
      expect(wrapper.find('.fa-envelope').exists()).toBe(true)
    })

    it('should render Copy Link button', () => {
      const wrapper = mount(SocialShareButtons, {
        props: defaultProps,
        global: {
          mocks: {
            $t: (key: string) => key,
          },
        },
      })
      expect(wrapper.find('.fa-link').exists()).toBe(true)
    })

    it('should render 5 buttons total (4 social + 1 copy)', () => {
      const wrapper = mount(SocialShareButtons, {
        props: defaultProps,
        global: {
          mocks: {
            $t: (key: string) => key,
          },
        },
      })
      expect(wrapper.findAll('button')).toHaveLength(5)
    })
  })

  describe('Layout Variants', () => {
    it('should apply horizontal layout by default', () => {
      const wrapper = mount(SocialShareButtons, {
        props: defaultProps,
        global: {
          mocks: {
            $t: (key: string) => key,
          },
        },
      })
      expect(wrapper.find('.social-share-buttons').classes()).toContain('flex-row')
    })

    it('should apply vertical layout', () => {
      const wrapper = mount(SocialShareButtons, {
        props: { ...defaultProps, layout: 'vertical' },
        global: {
          mocks: {
            $t: (key: string) => key,
          },
        },
      })
      expect(wrapper.find('.social-share-buttons').classes()).toContain('flex-col')
    })
  })

  describe('Size Variants', () => {
    it('should apply sm size classes', () => {
      const wrapper = mount(SocialShareButtons, {
        props: { ...defaultProps, size: 'sm' },
        global: {
          mocks: {
            $t: (key: string) => key,
          },
        },
      })
      const button = wrapper.find('button')
      expect(button.classes()).toContain('w-8')
      expect(button.classes()).toContain('h-8')
    })

    it('should apply md size classes by default', () => {
      const wrapper = mount(SocialShareButtons, {
        props: defaultProps,
        global: {
          mocks: {
            $t: (key: string) => key,
          },
        },
      })
      const button = wrapper.find('button')
      expect(button.classes()).toContain('w-10')
      expect(button.classes()).toContain('h-10')
    })

    it('should apply lg size classes', () => {
      const wrapper = mount(SocialShareButtons, {
        props: { ...defaultProps, size: 'lg' },
        global: {
          mocks: {
            $t: (key: string) => key,
          },
        },
      })
      const button = wrapper.find('button')
      expect(button.classes()).toContain('w-12')
      expect(button.classes()).toContain('h-12')
    })
  })

  describe('Labels', () => {
    it('should not show labels by default', () => {
      const wrapper = mount(SocialShareButtons, {
        props: defaultProps,
        global: {
          mocks: {
            $t: (key: string) => key,
          },
        },
      })
      expect(wrapper.text()).not.toContain('Twitter')
    })

    it('should show labels when showLabels is true', () => {
      const wrapper = mount(SocialShareButtons, {
        props: { ...defaultProps, showLabels: true },
        global: {
          mocks: {
            $t: (key: string) => key,
          },
        },
      })
      expect(wrapper.text()).toContain('Twitter')
      expect(wrapper.text()).toContain('LinkedIn')
      expect(wrapper.text()).toContain('Facebook')
    })
  })

  describe('Interactions', () => {
    it('should call shareToTwitter when Twitter button clicked', async () => {
      const wrapper = mount(SocialShareButtons, {
        props: defaultProps,
        global: {
          mocks: {
            $t: (key: string) => key,
          },
        },
      })
      const twitterBtn = wrapper.findAll('button')[0]
      await twitterBtn.trigger('click')

      expect(mockShareToTwitter).toHaveBeenCalled()
    })

    it('should call shareToLinkedIn when LinkedIn button clicked', async () => {
      const wrapper = mount(SocialShareButtons, {
        props: defaultProps,
        global: {
          mocks: {
            $t: (key: string) => key,
          },
        },
      })
      const linkedInBtn = wrapper.findAll('button')[1]
      await linkedInBtn.trigger('click')

      expect(mockShareToLinkedIn).toHaveBeenCalled()
    })

    it('should call shareToFacebook when Facebook button clicked', async () => {
      const wrapper = mount(SocialShareButtons, {
        props: defaultProps,
        global: {
          mocks: {
            $t: (key: string) => key,
          },
        },
      })
      const facebookBtn = wrapper.findAll('button')[2]
      await facebookBtn.trigger('click')

      expect(mockShareToFacebook).toHaveBeenCalled()
    })

    it('should call shareViaEmail when Email button clicked', async () => {
      const wrapper = mount(SocialShareButtons, {
        props: defaultProps,
        global: {
          mocks: {
            $t: (key: string) => key,
          },
        },
      })
      const emailBtn = wrapper.findAll('button')[3]
      await emailBtn.trigger('click')

      expect(mockShareViaEmail).toHaveBeenCalled()
    })

    it('should call copyLink when Copy button clicked', async () => {
      const wrapper = mount(SocialShareButtons, {
        props: defaultProps,
        global: {
          mocks: {
            $t: (key: string) => key,
          },
        },
      })
      const copyBtn = wrapper.findAll('button')[4]
      await copyBtn.trigger('click')

      expect(mockCopyLink).toHaveBeenCalledWith('https://example.com/article')
    })
  })

  describe('Copy Success State', () => {
    it('should show check icon when copy successful', () => {
      mockCopySuccess.value = true
      const wrapper = mount(SocialShareButtons, {
        props: defaultProps,
        global: {
          mocks: {
            $t: (key: string) => key,
          },
        },
      })
      expect(wrapper.find('.fa-check').exists()).toBe(true)
    })

    it('should show green styling when copy successful', () => {
      mockCopySuccess.value = true
      const wrapper = mount(SocialShareButtons, {
        props: defaultProps,
        global: {
          mocks: {
            $t: (key: string) => key,
          },
        },
      })
      const copyBtn = wrapper.findAll('button')[4]
      expect(copyBtn.classes()).toContain('bg-green-100')
      expect(copyBtn.classes()).toContain('text-green-600')
    })
  })
})
