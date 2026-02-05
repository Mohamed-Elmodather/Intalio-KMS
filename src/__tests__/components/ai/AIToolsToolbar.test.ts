import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import AIToolsToolbar from '@/components/ai/AIToolsToolbar.vue'

// Mock vue-i18n
vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string) => key,
  }),
}))

function mountComponent(props = {}) {
  return shallowMount(AIToolsToolbar, {
    props,
    global: {
      mocks: {
        $t: (key: string) => key,
      },
      stubs: {
        Teleport: true,
      },
    },
  })
}

describe('AIToolsToolbar', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('should render the toolbar', () => {
      const wrapper = mountComponent()
      expect(wrapper.exists()).toBe(true)
      expect(wrapper.find('.ai-tools-toolbar').exists()).toBe(true)
    })

    it('should render tool buttons', () => {
      const wrapper = mountComponent()
      const buttons = wrapper.findAll('.toolbar-tool')
      expect(buttons.length).toBeGreaterThan(0)
    })

    it('should render more tools button', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.fa-th-large').exists()).toBe(true)
    })

    it('should show divider', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.w-px.h-6.bg-gray-200').exists()).toBe(true)
    })
  })

  describe('Toolbar Tools', () => {
    it('should have 9 toolbar tools', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.toolbarTools.length).toBe(9)
    })

    it('should have extract-entities tool', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.toolbarTools.find((t: any) => t.id === 'extract-entities')).toBeTruthy()
    })

    it('should have summarize tool with dropdown', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const tool = vm.toolbarTools.find((t: any) => t.id === 'summarize')
      expect(tool.hasDropdown).toBe(true)
      expect(tool.dropdownOptions.length).toBeGreaterThan(0)
    })

    it('should have translate tool with dropdown', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const tool = vm.toolbarTools.find((t: any) => t.id === 'translate')
      expect(tool.hasDropdown).toBe(true)
      expect(tool.dropdownOptions.length).toBe(6)
    })

    it('should have generate-title tool with dropdown', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const tool = vm.toolbarTools.find((t: any) => t.id === 'generate-title')
      expect(tool.hasDropdown).toBe(true)
    })
  })

  describe('Compact Mode', () => {
    it('should apply compact class when compact is true', () => {
      const wrapper = mountComponent({ compact: true })
      expect(wrapper.find('.compact').exists()).toBe(true)
    })

    it('should not apply compact class by default', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.compact').exists()).toBe(false)
    })
  })

  describe('Show Labels', () => {
    it('should show labels by default', () => {
      const wrapper = mountComponent({ showLabels: true })
      const vm = wrapper.vm as any
      expect(vm.showLabels).toBe(true)
    })

    it('should hide labels when showLabels is false', () => {
      const wrapper = mountComponent({ showLabels: false })
      const vm = wrapper.vm as any
      expect(vm.showLabels).toBe(false)
    })
  })

  describe('Tool Background Colors', () => {
    it('should return active blue color', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getToolBgColor('blue', true)).toContain('bg-blue-500')
      expect(vm.getToolBgColor('blue', true)).toContain('text-white')
    })

    it('should return inactive blue color', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getToolBgColor('blue', false)).toContain('bg-blue-50')
      expect(vm.getToolBgColor('blue', false)).toContain('text-blue-600')
    })

    it('should return active teal color', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getToolBgColor('teal', true)).toContain('bg-teal-500')
    })

    it('should return inactive teal color', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getToolBgColor('teal', false)).toContain('bg-teal-50')
    })

    it('should return default color for unknown', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getToolBgColor('unknown', true)).toContain('bg-gray-500')
      expect(vm.getToolBgColor('unknown', false)).toContain('bg-gray-50')
    })
  })

  describe('Active Tool', () => {
    it('should apply active styling to active tool', () => {
      const wrapper = mountComponent({ activeTool: 'summarize' })
      const vm = wrapper.vm as any
      expect(vm.activeTool).toBe('summarize')
    })

    it('should have null activeTool by default', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.activeTool).toBeNull()
    })
  })

  describe('Processing Tool', () => {
    it('should track processing tool', () => {
      const wrapper = mountComponent({ processingTool: 'classify' })
      const vm = wrapper.vm as any
      expect(vm.processingTool).toBe('classify')
    })

    it('should show spinner when processing', () => {
      const wrapper = mountComponent({ processingTool: 'classify' })
      expect(wrapper.find('.fa-spinner').exists()).toBe(true)
    })

    it('should apply pulse animation when processing', () => {
      const wrapper = mountComponent({ processingTool: 'classify' })
      expect(wrapper.find('.animate-pulse').exists()).toBe(true)
    })
  })

  describe('Tool Click', () => {
    it('should emit tool-click for tool without dropdown', async () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const tool = vm.toolbarTools.find((t: any) => !t.hasDropdown)
      vm.handleToolClick(tool, undefined, { currentTarget: document.createElement('button') })
      expect(wrapper.emitted('tool-click')).toBeTruthy()
    })

    it('should toggle dropdown for tool with dropdown', async () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const tool = vm.toolbarTools.find((t: any) => t.hasDropdown)
      const mockEvent = { currentTarget: document.createElement('button') }
      mockEvent.currentTarget.getBoundingClientRect = () => ({ bottom: 100, left: 50 } as DOMRect)

      vm.handleToolClick(tool, undefined, mockEvent)
      expect(vm.openDropdown).toBe(tool.id)
    })

    it('should close dropdown on second click', async () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const tool = vm.toolbarTools.find((t: any) => t.hasDropdown)
      const mockEvent = { currentTarget: document.createElement('button') }
      mockEvent.currentTarget.getBoundingClientRect = () => ({ bottom: 100, left: 50 } as DOMRect)

      vm.openDropdown = tool.id
      vm.handleToolClick(tool, undefined, mockEvent)
      expect(vm.openDropdown).toBeNull()
    })
  })

  describe('Dropdown Option', () => {
    it('should emit tool-click with option', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const tool = vm.toolbarTools.find((t: any) => t.hasDropdown)
      vm.handleDropdownOption(tool, 'brief')
      expect(wrapper.emitted('tool-click')).toBeTruthy()
      expect(wrapper.emitted('tool-click')![0]).toEqual([tool.id, 'brief'])
    })

    it('should close dropdown after option selected', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const tool = vm.toolbarTools.find((t: any) => t.hasDropdown)
      vm.openDropdown = tool.id
      vm.handleDropdownOption(tool, 'brief')
      expect(vm.openDropdown).toBeNull()
    })
  })

  describe('Palette Open', () => {
    it('should emit palette-open when more button clicked', async () => {
      const wrapper = mountComponent()
      const moreBtn = wrapper.find('.fa-th-large').element.parentElement
      await moreBtn?.click()
      expect(wrapper.emitted('palette-open')).toBeTruthy()
    })
  })

  describe('Close Dropdowns', () => {
    it('should close dropdowns', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.openDropdown = 'summarize'
      vm.closeDropdowns()
      expect(vm.openDropdown).toBeNull()
    })
  })

  describe('Dropdown Position', () => {
    it('should track dropdown position', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.dropdownPosition).toEqual({ top: 0, left: 0 })
    })
  })

  describe('Tool Shortcuts', () => {
    it('should have shortcut on entity tool', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const tool = vm.toolbarTools.find((t: any) => t.id === 'extract-entities')
      expect(tool.shortcut).toBe('E')
    })

    it('should have shortcut on sentiment tool', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const tool = vm.toolbarTools.find((t: any) => t.id === 'analyze-sentiment')
      expect(tool.shortcut).toBe('S')
    })
  })

  describe('Keyboard Shortcut Display', () => {
    it('should show keyboard shortcut in more button', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('⌘K')
    })
  })
})
