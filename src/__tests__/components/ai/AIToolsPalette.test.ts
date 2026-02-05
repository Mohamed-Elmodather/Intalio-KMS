import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import AIToolsPalette from '@/components/ai/AIToolsPalette.vue'

// Mock vue-i18n
vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string) => key,
  }),
}))

function mountComponent(props = {}) {
  return shallowMount(AIToolsPalette, {
    props: {
      modelValue: true,
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

describe('AIToolsPalette', () => {
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
      expect(wrapper.find('.max-w-2xl').exists()).toBe(false)
    })

    it('should render search input', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('input').exists()).toBe(true)
    })

    it('should render wand icon', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.fa-wand-magic-sparkles').exists()).toBe(true)
    })

    it('should render close button', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.fa-times').exists()).toBe(true)
    })
  })

  describe('AI Tools', () => {
    it('should have 9 AI tools', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.aiTools.length).toBe(9)
    })

    it('should have extract-entities tool', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.aiTools.find((t: any) => t.id === 'extract-entities')).toBeTruthy()
    })

    it('should have analyze-sentiment tool', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.aiTools.find((t: any) => t.id === 'analyze-sentiment')).toBeTruthy()
    })

    it('should have summarize tool', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.aiTools.find((t: any) => t.id === 'summarize')).toBeTruthy()
    })

    it('should have classify tool', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.aiTools.find((t: any) => t.id === 'classify')).toBeTruthy()
    })

    it('should have ocr tool', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.aiTools.find((t: any) => t.id === 'ocr')).toBeTruthy()
    })

    it('should have translate tool', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.aiTools.find((t: any) => t.id === 'translate')).toBeTruthy()
    })

    it('should have generate-title tool', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.aiTools.find((t: any) => t.id === 'generate-title')).toBeTruthy()
    })

    it('should have auto-tag tool', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.aiTools.find((t: any) => t.id === 'auto-tag')).toBeTruthy()
    })

    it('should have smart-search tool', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.aiTools.find((t: any) => t.id === 'smart-search')).toBeTruthy()
    })
  })

  describe('Categories', () => {
    it('should have 4 categories', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(Object.keys(vm.categories).length).toBe(4)
    })

    it('should have analysis category', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.categories.analysis).toBeTruthy()
    })

    it('should have content category', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.categories.content).toBeTruthy()
    })

    it('should have extraction category', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.categories.extraction).toBeTruthy()
    })

    it('should have search category', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.categories.search).toBeTruthy()
    })
  })

  describe('Filtering', () => {
    it('should return all tools when search is empty', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.searchQuery = ''
      expect(vm.filteredTools.length).toBe(9)
    })

    it('should filter tools by name', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.searchQuery = 'summar'
      expect(vm.filteredTools.length).toBeGreaterThan(0)
      expect(vm.filteredTools.find((t: any) => t.id === 'summarize')).toBeTruthy()
    })

    it('should filter tools by category', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.searchQuery = 'analysis'
      expect(vm.filteredTools.length).toBeGreaterThan(0)
    })
  })

  describe('Grouped Tools', () => {
    it('should group tools by category', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.groupedTools).toBeTruthy()
      expect(typeof vm.groupedTools).toBe('object')
    })
  })

  describe('Flat Tools', () => {
    it('should return flat list of tools', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(Array.isArray(vm.flatTools)).toBe(true)
      expect(vm.flatTools.length).toBeGreaterThan(0)
    })
  })

  describe('Tool Colors', () => {
    it('should return blue color classes', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getToolColor('blue')).toContain('bg-blue-100')
    })

    it('should return pink color classes', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getToolColor('pink')).toContain('bg-pink-100')
    })

    it('should return teal color classes', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getToolColor('teal')).toContain('bg-teal-100')
    })

    it('should return default color for unknown', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getToolColor('unknown')).toContain('bg-gray-100')
    })
  })

  describe('Selection', () => {
    it('should track selected index', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.selectedIndex).toBe(0)
    })

    it('should check if tool is selected', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const firstTool = vm.flatTools[0]
      expect(vm.isSelected(firstTool)).toBe(true)
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

    it('should close when backdrop clicked', async () => {
      const wrapper = mountComponent()
      const backdrop = wrapper.find('.bg-black\\/50')
      await backdrop.trigger('click')
      expect(wrapper.emitted('update:modelValue')).toBeTruthy()
    })

    it('should close when close button clicked', async () => {
      const wrapper = mountComponent()
      const closeBtn = wrapper.find('.fa-times').element.parentElement
      await closeBtn?.click()
      expect(wrapper.emitted('update:modelValue')).toBeTruthy()
    })
  })

  describe('Select Tool', () => {
    it('should emit select when tool selected', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const tool = vm.aiTools[0]
      vm.selectTool(tool)
      expect(wrapper.emitted('select')).toBeTruthy()
      expect(wrapper.emitted('select')![0][0]).toEqual(tool)
    })

    it('should close after selecting tool', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const tool = vm.aiTools[0]
      vm.selectTool(tool)
      expect(wrapper.emitted('update:modelValue')).toBeTruthy()
    })
  })

  describe('Keyboard Navigation', () => {
    it('should have handleKeydown function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.handleKeydown).toBe('function')
    })

    it('should handle Escape key', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.handleKeydown({ key: 'Escape' })
      expect(wrapper.emitted('update:modelValue')).toBeTruthy()
    })

    it('should handle ArrowDown key', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const initialIndex = vm.selectedIndex
      vm.handleKeydown({ key: 'ArrowDown', preventDefault: vi.fn() })
      expect(vm.selectedIndex).toBe(initialIndex + 1)
    })

    it('should handle ArrowUp key', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.selectedIndex = 2
      vm.handleKeydown({ key: 'ArrowUp', preventDefault: vi.fn() })
      expect(vm.selectedIndex).toBe(1)
    })

    it('should not go below 0 on ArrowUp', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.selectedIndex = 0
      vm.handleKeydown({ key: 'ArrowUp', preventDefault: vi.fn() })
      expect(vm.selectedIndex).toBe(0)
    })

    it('should handle Enter key', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.handleKeydown({ key: 'Enter', preventDefault: vi.fn() })
      expect(wrapper.emitted('select')).toBeTruthy()
    })
  })

  describe('Footer', () => {
    it('should show keyboard hints', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('ai.navigate')
      expect(wrapper.text()).toContain('common.select')
      expect(wrapper.text()).toContain('common.close')
    })

    it('should show tools count', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('ai.toolsAvailable')
    })
  })
})
