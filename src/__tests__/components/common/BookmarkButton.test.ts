import { describe, it, expect, vi, beforeEach } from 'vitest'
import { mount } from '@vue/test-utils'
import { ref } from 'vue'
import BookmarkButton from '@/components/common/BookmarkButton.vue'

// Mock vue-i18n
vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string) => {
      const translations: Record<string, string> = {
        'common.removeFromSaved': 'Remove from saved',
        'common.saveForLater': 'Save for later',
        'common.saved': 'Saved',
        'common.removedFromSaved': 'Removed from saved',
        'common.save': 'Save',
      }
      return translations[key] || key
    },
  }),
}))

// Mock the useBookmarks composable
const mockIsBookmarked = vi.fn()
const mockToggleBookmark = vi.fn()
const mockLoadBookmarks = vi.fn()

vi.mock('@/composables/useBookmarks', () => ({
  useBookmarks: () => ({
    isBookmarked: mockIsBookmarked,
    toggleBookmark: mockToggleBookmark,
    loadBookmarks: mockLoadBookmarks,
  }),
}))

describe('BookmarkButton', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    mockIsBookmarked.mockReturnValue(false)
    mockToggleBookmark.mockResolvedValue(true)
  })

  const defaultProps = {
    contentId: 'article-123',
    contentType: 'article' as const,
  }

  describe('Icon Variant (default)', () => {
    it('should render as icon button by default', () => {
      const wrapper = mount(BookmarkButton, {
        props: defaultProps,
        global: {
          mocks: {
            $t: (key: string) => key,
          },
        },
      })
      expect(wrapper.find('button').exists()).toBe(true)
    })

    it('should show empty bookmark icon when not bookmarked', () => {
      mockIsBookmarked.mockReturnValue(false)
      const wrapper = mount(BookmarkButton, {
        props: defaultProps,
        global: {
          mocks: {
            $t: (key: string) => key,
          },
        },
      })
      expect(wrapper.find('.far.fa-bookmark').exists()).toBe(true)
    })

    it('should show filled bookmark icon when bookmarked', () => {
      mockIsBookmarked.mockReturnValue(true)
      const wrapper = mount(BookmarkButton, {
        props: defaultProps,
        global: {
          mocks: {
            $t: (key: string) => key,
          },
        },
      })
      expect(wrapper.find('.fas.fa-bookmark').exists()).toBe(true)
    })

    it('should have teal styling when bookmarked', () => {
      mockIsBookmarked.mockReturnValue(true)
      const wrapper = mount(BookmarkButton, {
        props: defaultProps,
        global: {
          mocks: {
            $t: (key: string) => key,
          },
        },
      })
      expect(wrapper.find('button').classes()).toContain('bg-teal-100')
      expect(wrapper.find('button').classes()).toContain('text-teal-600')
    })

    it('should have gray styling when not bookmarked', () => {
      mockIsBookmarked.mockReturnValue(false)
      const wrapper = mount(BookmarkButton, {
        props: defaultProps,
        global: {
          mocks: {
            $t: (key: string) => key,
          },
        },
      })
      expect(wrapper.find('button').classes()).toContain('bg-gray-100')
      expect(wrapper.find('button').classes()).toContain('text-gray-500')
    })
  })

  describe('Button Variant', () => {
    it('should render as button variant', () => {
      const wrapper = mount(BookmarkButton, {
        props: { ...defaultProps, variant: 'button' },
        global: {
          mocks: {
            $t: (key: string) => key,
          },
        },
      })
      expect(wrapper.find('button').classes()).toContain('rounded-lg')
    })

    it('should show label when showLabel is true', () => {
      const wrapper = mount(BookmarkButton, {
        props: { ...defaultProps, variant: 'button', showLabel: true },
        global: {
          mocks: {
            $t: (key: string) => {
              if (key === 'common.save') return 'Save'
              if (key === 'common.saved') return 'Saved'
              return key
            },
          },
        },
      })
      expect(wrapper.text()).toContain('Save')
    })

    it('should show "Saved" label when bookmarked', () => {
      mockIsBookmarked.mockReturnValue(true)
      const wrapper = mount(BookmarkButton, {
        props: { ...defaultProps, variant: 'button', showLabel: true },
        global: {
          mocks: {
            $t: (key: string) => {
              if (key === 'common.save') return 'Save'
              if (key === 'common.saved') return 'Saved'
              return key
            },
          },
        },
      })
      expect(wrapper.text()).toContain('Saved')
    })
  })

  describe('Size Variants', () => {
    it('should apply sm size classes for icon variant', () => {
      const wrapper = mount(BookmarkButton, {
        props: { ...defaultProps, size: 'sm' },
        global: {
          mocks: {
            $t: (key: string) => key,
          },
        },
      })
      expect(wrapper.find('button').classes()).toContain('w-8')
      expect(wrapper.find('button').classes()).toContain('h-8')
    })

    it('should apply md size classes by default', () => {
      const wrapper = mount(BookmarkButton, {
        props: defaultProps,
        global: {
          mocks: {
            $t: (key: string) => key,
          },
        },
      })
      expect(wrapper.find('button').classes()).toContain('w-10')
      expect(wrapper.find('button').classes()).toContain('h-10')
    })

    it('should apply lg size classes', () => {
      const wrapper = mount(BookmarkButton, {
        props: { ...defaultProps, size: 'lg' },
        global: {
          mocks: {
            $t: (key: string) => key,
          },
        },
      })
      expect(wrapper.find('button').classes()).toContain('w-12')
      expect(wrapper.find('button').classes()).toContain('h-12')
    })
  })

  describe('Interactions', () => {
    it('should call toggleBookmark when clicked', async () => {
      const wrapper = mount(BookmarkButton, {
        props: defaultProps,
        global: {
          mocks: {
            $t: (key: string) => key,
          },
        },
      })
      await wrapper.find('button').trigger('click')

      expect(mockToggleBookmark).toHaveBeenCalledWith('article-123', 'article')
    })

    it('should emit bookmarked event after toggle', async () => {
      mockToggleBookmark.mockResolvedValue(true)
      const wrapper = mount(BookmarkButton, {
        props: defaultProps,
        global: {
          mocks: {
            $t: (key: string) => key,
          },
        },
      })
      await wrapper.find('button').trigger('click')
      await vi.waitFor(() => {
        expect(wrapper.emitted('bookmarked')).toBeDefined()
      })
    })

    it('should show spinner while loading', async () => {
      mockToggleBookmark.mockImplementation(
        () => new Promise((resolve) => setTimeout(() => resolve(true), 100))
      )
      const wrapper = mount(BookmarkButton, {
        props: defaultProps,
        global: {
          mocks: {
            $t: (key: string) => key,
          },
        },
      })

      wrapper.find('button').trigger('click')
      await wrapper.vm.$nextTick()

      expect(wrapper.find('.fa-spinner').exists()).toBe(true)
    })

    it('should disable button while loading', async () => {
      mockToggleBookmark.mockImplementation(
        () => new Promise((resolve) => setTimeout(() => resolve(true), 100))
      )
      const wrapper = mount(BookmarkButton, {
        props: defaultProps,
        global: {
          mocks: {
            $t: (key: string) => key,
          },
        },
      })

      wrapper.find('button').trigger('click')
      await wrapper.vm.$nextTick()

      expect(wrapper.find('button').attributes('disabled')).toBeDefined()
    })
  })

  describe('Lifecycle', () => {
    it('should load bookmarks on mount', () => {
      mount(BookmarkButton, {
        props: defaultProps,
        global: {
          mocks: {
            $t: (key: string) => key,
          },
        },
      })
      expect(mockLoadBookmarks).toHaveBeenCalled()
    })
  })
})
