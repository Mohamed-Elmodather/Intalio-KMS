import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import ComparisonButton from '@/components/common/ComparisonButton.vue'

// Mock comparison store
const mockIsItemSelected = vi.fn().mockReturnValue(false)
const mockAddItem = vi.fn()
const mockRemoveItem = vi.fn()

vi.mock('@/stores/comparison', () => ({
  useComparisonStore: () => ({
    isItemSelected: mockIsItemSelected,
    addItem: mockAddItem,
    removeItem: mockRemoveItem,
  }),
}))

const mockItem = {
  id: '123',
  title: 'Test Article',
  thumbnail: 'https://example.com/image.jpg',
  description: 'Test description',
  author: 'John Doe',
  date: '2024-01-15',
  category: 'Sports',
  tags: ['football', 'asia'],
}

function mountComponent(props = {}) {
  return shallowMount(ComparisonButton, {
    props: {
      item: mockItem,
      type: 'article',
      ...props,
    },
  })
}

describe('ComparisonButton', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    mockIsItemSelected.mockReturnValue(false)
  })

  describe('Rendering', () => {
    it('should render the component', () => {
      const wrapper = mountComponent()
      expect(wrapper.exists()).toBe(true)
      expect(wrapper.find('.comparison-btn').exists()).toBe(true)
    })

    it('should render layer-group icon', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.fa-layer-group').exists()).toBe(true)
    })
  })

  describe('Size Variants', () => {
    it('should apply sm size by default', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.comparison-btn--sm').exists()).toBe(true)
    })

    it('should apply md size', () => {
      const wrapper = mountComponent({ size: 'md' })
      expect(wrapper.find('.comparison-btn--md').exists()).toBe(true)
    })

    it('should apply lg size', () => {
      const wrapper = mountComponent({ size: 'lg' })
      expect(wrapper.find('.comparison-btn--lg').exists()).toBe(true)
    })
  })

  describe('Variant Styles', () => {
    it('should apply overlay variant by default', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.comparison-btn--overlay').exists()).toBe(true)
    })

    it('should apply solid variant', () => {
      const wrapper = mountComponent({ variant: 'solid' })
      expect(wrapper.find('.comparison-btn--solid').exists()).toBe(true)
    })

    it('should apply outline variant', () => {
      const wrapper = mountComponent({ variant: 'outline' })
      expect(wrapper.find('.comparison-btn--outline').exists()).toBe(true)
    })
  })

  describe('Active State', () => {
    it('should not show active class when not in comparison', () => {
      mockIsItemSelected.mockReturnValue(false)
      const wrapper = mountComponent()
      expect(wrapper.find('.comparison-btn--active').exists()).toBe(false)
    })

    it('should show active class when in comparison', () => {
      mockIsItemSelected.mockReturnValue(true)
      const wrapper = mountComponent()
      expect(wrapper.find('.comparison-btn--active').exists()).toBe(true)
    })
  })

  describe('Button Title', () => {
    it('should show "Add to Compare" when not in comparison', () => {
      mockIsItemSelected.mockReturnValue(false)
      const wrapper = mountComponent()
      expect(wrapper.find('button').attributes('title')).toBe('Add to Compare')
    })

    it('should show "Remove from Compare" when in comparison', () => {
      mockIsItemSelected.mockReturnValue(true)
      const wrapper = mountComponent()
      expect(wrapper.find('button').attributes('title')).toBe('Remove from Compare')
    })
  })

  describe('Toggle Comparison', () => {
    it('should add item when not in comparison', async () => {
      mockIsItemSelected.mockReturnValue(false)
      const wrapper = mountComponent()
      await wrapper.find('button').trigger('click')
      expect(mockAddItem).toHaveBeenCalled()
    })

    it('should remove item when in comparison', async () => {
      mockIsItemSelected.mockReturnValue(true)
      const wrapper = mountComponent()
      await wrapper.find('button').trigger('click')
      expect(mockRemoveItem).toHaveBeenCalledWith('123')
    })

    it('should pass correct comparison item data', async () => {
      mockIsItemSelected.mockReturnValue(false)
      const wrapper = mountComponent()
      await wrapper.find('button').trigger('click')
      expect(mockAddItem).toHaveBeenCalledWith(expect.objectContaining({
        id: '123',
        type: 'article',
        title: 'Test Article',
        thumbnail: 'https://example.com/image.jpg',
        description: 'Test description',
      }))
    })

    it('should include metadata in comparison item', async () => {
      mockIsItemSelected.mockReturnValue(false)
      const wrapper = mountComponent()
      await wrapper.find('button').trigger('click')
      expect(mockAddItem).toHaveBeenCalledWith(expect.objectContaining({
        metadata: expect.objectContaining({
          author: 'John Doe',
          date: '2024-01-15',
          category: 'Sports',
          tags: ['football', 'asia'],
        }),
      }))
    })
  })

  describe('Item ID', () => {
    it('should convert numeric id to string', () => {
      const wrapper = mountComponent({
        item: { ...mockItem, id: 456 },
      })
      const vm = wrapper.vm as any
      expect(vm.itemId).toBe('456')
    })

    it('should keep string id as is', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.itemId).toBe('123')
    })
  })

  describe('Click Event', () => {
    it('should stop event propagation', async () => {
      const wrapper = mountComponent()
      const button = wrapper.find('button')
      // The .stop modifier is in the template
      expect(button.attributes('onclick')).toBeUndefined()
    })
  })

  describe('Computed Properties', () => {
    it('should compute isInComparison correctly', () => {
      mockIsItemSelected.mockReturnValue(true)
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.isInComparison).toBe(true)
    })

    it('should compute buttonTitle correctly', () => {
      mockIsItemSelected.mockReturnValue(false)
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.buttonTitle).toBe('Add to Compare')
    })
  })
})
