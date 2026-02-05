import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import ComparisonSideBySide from '@/components/ai/ComparisonSideBySide.vue'
import { ref } from 'vue'

// Mock vue-i18n
vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string) => key,
    locale: ref('en'),
  }),
}))

const mockSelectedItems = [
  {
    id: '1',
    title: 'Test Document',
    type: 'document',
    description: 'A test document description',
    thumbnail: null,
    metadata: {
      author: 'John Doe',
      date: '2024-01-15',
      size: 1048576, // 1MB
      category: 'Reports',
      tags: ['test', 'document', 'report', 'example'],
    },
  },
  {
    id: '2',
    title: 'Test Article',
    type: 'article',
    description: 'A test article description',
    thumbnail: 'https://example.com/image.jpg',
    metadata: {
      author: 'Jane Smith',
      date: '2024-02-20',
      duration: 180, // 3 minutes
      tags: ['news', 'article'],
    },
  },
]

const mockRemoveItem = vi.fn()

vi.mock('@/stores/comparison', () => ({
  useComparisonStore: () => ({
    selectedItems: mockSelectedItems,
    removeItem: mockRemoveItem,
  }),
}))

function mountComponent() {
  return shallowMount(ComparisonSideBySide, {
    global: {
      mocks: {
        $t: (key: string) => key,
      },
    },
  })
}

describe('ComparisonSideBySide', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('should render the component', () => {
      const wrapper = mountComponent()
      expect(wrapper.exists()).toBe(true)
      expect(wrapper.find('.comparison-side-by-side').exists()).toBe(true)
    })

    it('should render item cards', () => {
      const wrapper = mountComponent()
      const cards = wrapper.findAll('.w-72')
      expect(cards.length).toBe(2)
    })

    it('should render item titles', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('Test Document')
      expect(wrapper.text()).toContain('Test Article')
    })

    it('should render item descriptions', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('A test document description')
      expect(wrapper.text()).toContain('A test article description')
    })
  })

  describe('Type Config', () => {
    it('should get document type config', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const config = vm.getTypeConfig('document')
      expect(config.icon).toBe('fas fa-file-alt')
      expect(config.bg).toBe('bg-blue-50')
      expect(config.text).toBe('text-blue-600')
    })

    it('should get article type config', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const config = vm.getTypeConfig('article')
      expect(config.icon).toBe('fas fa-newspaper')
      expect(config.bg).toBe('bg-purple-50')
      expect(config.text).toBe('text-purple-600')
    })

    it('should get media type config', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const config = vm.getTypeConfig('media')
      expect(config.icon).toBe('fas fa-photo-video')
      expect(config.bg).toBe('bg-pink-50')
    })

    it('should get event type config', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const config = vm.getTypeConfig('event')
      expect(config.icon).toBe('fas fa-calendar-alt')
      expect(config.bg).toBe('bg-orange-50')
    })

    it('should return document config for unknown type', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const config = vm.getTypeConfig('unknown')
      expect(config.icon).toBe('fas fa-file-alt')
    })
  })

  describe('Format Size', () => {
    it('should format bytes', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.formatSize(500)).toBe('500 B')
    })

    it('should format kilobytes', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.formatSize(2048)).toBe('2.0 KB')
    })

    it('should format megabytes', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.formatSize(1048576)).toBe('1.0 MB')
    })

    it('should return dash for undefined size', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.formatSize(undefined)).toBe('-')
    })

    it('should return dash for zero size', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.formatSize(0)).toBe('-')
    })
  })

  describe('Format Duration', () => {
    it('should format seconds', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.formatDuration(45)).toBe('0:45')
    })

    it('should format minutes and seconds', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.formatDuration(180)).toBe('3:00')
    })

    it('should pad seconds with zero', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.formatDuration(65)).toBe('1:05')
    })

    it('should return dash for undefined duration', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.formatDuration(undefined)).toBe('-')
    })

    it('should return dash for zero duration', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.formatDuration(0)).toBe('-')
    })
  })

  describe('Format Date', () => {
    it('should format date string', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const result = vm.formatDate('2024-01-15')
      expect(result).toBeTruthy()
      expect(result).not.toBe('-')
    })

    it('should return dash for undefined date', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.formatDate(undefined)).toBe('-')
    })

    it('should return dash for empty date', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.formatDate('')).toBe('-')
    })
  })

  describe('Metadata Display', () => {
    it('should show author metadata', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('John Doe')
      expect(wrapper.find('.fa-user').exists()).toBe(true)
    })

    it('should show date metadata', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.fa-calendar').exists()).toBe(true)
    })

    it('should show size metadata', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.fa-file').exists()).toBe(true)
    })

    it('should show category metadata', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('Reports')
      expect(wrapper.find('.fa-folder').exists()).toBe(true)
    })
  })

  describe('Tags Display', () => {
    it('should show tags', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('test')
      expect(wrapper.text()).toContain('document')
      expect(wrapper.text()).toContain('report')
    })

    it('should show +N for extra tags', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('+1') // 4 tags, showing 3, +1 more
    })
  })

  describe('Thumbnail', () => {
    it('should show thumbnail image when available', () => {
      const wrapper = mountComponent()
      const img = wrapper.find('img')
      expect(img.exists()).toBe(true)
      expect(img.attributes('src')).toBe('https://example.com/image.jpg')
    })

    it('should show fallback icon when no thumbnail', () => {
      const wrapper = mountComponent()
      // Document has no thumbnail, should show icon
      const fallbackIcons = wrapper.findAll('.text-4xl.text-gray-300')
      expect(fallbackIcons.length).toBeGreaterThan(0)
    })
  })

  describe('Remove Item', () => {
    it('should call removeItem when remove button clicked', async () => {
      const wrapper = mountComponent()
      const removeBtn = wrapper.find('.fa-times.text-sm').element.parentElement
      await removeBtn?.click()
      expect(mockRemoveItem).toHaveBeenCalled()
    })
  })

  describe('Comparison Hint', () => {
    it('should show comparison hint when 2+ items', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('comparison.quickTips.title')
    })

    it('should show tips', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('comparison.quickTips.tip1')
      expect(wrapper.text()).toContain('comparison.quickTips.tip2')
      expect(wrapper.text()).toContain('comparison.quickTips.tip3')
    })

    it('should show lightbulb icon', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.fa-lightbulb').exists()).toBe(true)
    })
  })
})
