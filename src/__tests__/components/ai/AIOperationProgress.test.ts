import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import AIOperationProgress from '@/components/ai/AIOperationProgress.vue'

// Mock vue-i18n
vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string, params?: any) => params ? `${key} ${JSON.stringify(params)}` : key,
  }),
}))

function mountComponent(props = {}) {
  return shallowMount(AIOperationProgress, {
    props: {
      operation: 'summarize',
      status: 'processing',
      ...props,
    },
    global: {
      mocks: {
        $t: (key: string) => key,
      },
    },
  })
}

describe('AIOperationProgress', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('should render the component', () => {
      const wrapper = mountComponent()
      expect(wrapper.exists()).toBe(true)
      expect(wrapper.find('.ai-operation-progress').exists()).toBe(true)
    })

    it('should render operation name', () => {
      const wrapper = mountComponent({ operation: 'summarize' })
      const vm = wrapper.vm as any
      expect(vm.meta.name).toContain('ai.operations.generatingSummary')
    })
  })

  describe('Operation Types', () => {
    it('should show extract-entities metadata', () => {
      const wrapper = mountComponent({ operation: 'extract-entities' })
      const vm = wrapper.vm as any
      expect(vm.meta.icon).toBe('fas fa-tags')
      expect(vm.meta.color).toBe('blue')
    })

    it('should show analyze-sentiment metadata', () => {
      const wrapper = mountComponent({ operation: 'analyze-sentiment' })
      const vm = wrapper.vm as any
      expect(vm.meta.icon).toBe('fas fa-smile')
      expect(vm.meta.color).toBe('pink')
    })

    it('should show summarize metadata', () => {
      const wrapper = mountComponent({ operation: 'summarize' })
      const vm = wrapper.vm as any
      expect(vm.meta.icon).toBe('fas fa-compress-alt')
      expect(vm.meta.color).toBe('teal')
    })

    it('should show classify metadata', () => {
      const wrapper = mountComponent({ operation: 'classify' })
      const vm = wrapper.vm as any
      expect(vm.meta.icon).toBe('fas fa-folder-tree')
      expect(vm.meta.color).toBe('purple')
    })

    it('should show translate metadata', () => {
      const wrapper = mountComponent({ operation: 'translate' })
      const vm = wrapper.vm as any
      expect(vm.meta.icon).toBe('fas fa-language')
      expect(vm.meta.color).toBe('green')
    })

    it('should show ocr metadata', () => {
      const wrapper = mountComponent({ operation: 'ocr' })
      const vm = wrapper.vm as any
      expect(vm.meta.icon).toBe('fas fa-file-image')
      expect(vm.meta.color).toBe('amber')
    })

    it('should show generate-title metadata', () => {
      const wrapper = mountComponent({ operation: 'generate-title' })
      const vm = wrapper.vm as any
      expect(vm.meta.icon).toBe('fas fa-heading')
      expect(vm.meta.color).toBe('indigo')
    })

    it('should show auto-tag metadata', () => {
      const wrapper = mountComponent({ operation: 'auto-tag' })
      const vm = wrapper.vm as any
      expect(vm.meta.icon).toBe('fas fa-hashtag')
      expect(vm.meta.color).toBe('cyan')
    })

    it('should show smart-search metadata', () => {
      const wrapper = mountComponent({ operation: 'smart-search' })
      const vm = wrapper.vm as any
      expect(vm.meta.icon).toBe('fas fa-search')
      expect(vm.meta.color).toBe('orange')
    })

    it('should show chat metadata', () => {
      const wrapper = mountComponent({ operation: 'chat' })
      const vm = wrapper.vm as any
      expect(vm.meta.icon).toBe('fas fa-comment')
      expect(vm.meta.color).toBe('teal')
    })

    it('should show default metadata for unknown operation', () => {
      const wrapper = mountComponent({ operation: 'unknown-operation' as any })
      const vm = wrapper.vm as any
      expect(vm.meta.icon).toBe('fas fa-cog')
      expect(vm.meta.color).toBe('gray')
    })
  })

  describe('Status States', () => {
    it('should show pending status message', () => {
      const wrapper = mountComponent({ status: 'pending' })
      const vm = wrapper.vm as any
      expect(vm.statusMessage).toContain('ai.status.waitingToStart')
    })

    it('should show processing status message', () => {
      const wrapper = mountComponent({ status: 'processing', progress: 0 })
      const vm = wrapper.vm as any
      expect(vm.statusMessage).toContain('ai.status.processing')
    })

    it('should show processing with progress message', () => {
      const wrapper = mountComponent({ status: 'processing', progress: 50 })
      const vm = wrapper.vm as any
      expect(vm.statusMessage).toContain('ai.status.percentComplete')
    })

    it('should show success status message', () => {
      const wrapper = mountComponent({ status: 'success' })
      const vm = wrapper.vm as any
      expect(vm.statusMessage).toContain('ai.status.completedSuccessfully')
    })

    it('should show error status message', () => {
      const wrapper = mountComponent({ status: 'error' })
      const vm = wrapper.vm as any
      expect(vm.statusMessage).toContain('ai.status.errorOccurred')
    })

    it('should show custom message when provided', () => {
      const wrapper = mountComponent({ status: 'processing', message: 'Custom message' })
      const vm = wrapper.vm as any
      expect(vm.statusMessage).toBe('Custom message')
    })
  })

  describe('Progress Bar', () => {
    it('should show progress bar when processing', () => {
      const wrapper = mountComponent({ status: 'processing' })
      expect(wrapper.find('.bg-white\\/50.rounded-full').exists()).toBe(true)
    })

    it('should not show progress bar when not processing', () => {
      const wrapper = mountComponent({ status: 'success' })
      expect(wrapper.find('.w-full.bg-white\\/50.rounded-full').exists()).toBe(false)
    })

    it('should show progress percentage when progress > 0', () => {
      const wrapper = mountComponent({ status: 'processing', progress: 50 })
      expect(wrapper.text()).toContain('50%')
    })
  })

  describe('Status Icons', () => {
    it('should show check icon on success', () => {
      const wrapper = mountComponent({ status: 'success' })
      expect(wrapper.find('.fa-check').exists()).toBe(true)
    })

    it('should show error icon on error', () => {
      const wrapper = mountComponent({ status: 'error' })
      expect(wrapper.find('.fa-exclamation-triangle').exists()).toBe(true)
    })

    it('should show success status icon container', () => {
      const wrapper = mountComponent({ status: 'success' })
      expect(wrapper.find('.bg-green-100.text-green-600').exists()).toBe(true)
    })

    it('should show error status icon container', () => {
      const wrapper = mountComponent({ status: 'error' })
      expect(wrapper.find('.bg-red-100.text-red-600').exists()).toBe(true)
    })
  })

  describe('Cancel Button', () => {
    it('should show cancel button when processing and showCancel is true', () => {
      const wrapper = mountComponent({ status: 'processing', showCancel: true })
      expect(wrapper.find('.fa-times').exists()).toBe(true)
    })

    it('should hide cancel button when showCancel is false', () => {
      const wrapper = mountComponent({ status: 'processing', showCancel: false })
      const buttons = wrapper.findAll('button')
      expect(buttons.length).toBe(0)
    })

    it('should emit cancel when cancel button clicked', async () => {
      const wrapper = mountComponent({ status: 'processing', showCancel: true })
      const cancelBtn = wrapper.find('button')
      await cancelBtn.trigger('click')
      expect(wrapper.emitted('cancel')).toBeTruthy()
    })

    it('should not show cancel button when status is not processing', () => {
      const wrapper = mountComponent({ status: 'success', showCancel: true })
      const buttons = wrapper.findAll('button')
      expect(buttons.length).toBe(0)
    })
  })

  describe('Color Classes', () => {
    it('should apply correct color classes for teal', () => {
      const wrapper = mountComponent({ operation: 'summarize' })
      const vm = wrapper.vm as any
      expect(vm.colorClasses.bg).toContain('from-teal-50')
      expect(vm.colorClasses.progress).toBe('bg-teal-500')
      expect(vm.colorClasses.icon).toBe('bg-teal-500')
    })

    it('should apply correct color classes for blue', () => {
      const wrapper = mountComponent({ operation: 'extract-entities' })
      const vm = wrapper.vm as any
      expect(vm.colorClasses.bg).toContain('from-blue-50')
      expect(vm.colorClasses.progress).toBe('bg-blue-500')
    })

    it('should apply error border on error status', () => {
      const wrapper = mountComponent({ status: 'error' })
      expect(wrapper.find('.border-red-200').exists()).toBe(true)
    })
  })

  describe('Animation Classes', () => {
    it('should apply pulse animation to icon container when processing', () => {
      const wrapper = mountComponent({ status: 'processing' })
      const iconContainer = wrapper.find('.w-10.h-10.rounded-lg')
      expect(iconContainer.classes()).toContain('animate-pulse')
    })

    it('should apply spin animation to icon when processing', () => {
      const wrapper = mountComponent({ status: 'processing' })
      const icon = wrapper.find('.w-10.h-10.rounded-lg i')
      expect(icon.classes()).toContain('animate-spin')
    })
  })
})
