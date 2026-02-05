import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import AIWorkflowBuilder from '@/components/ai/AIWorkflowBuilder.vue'
import { ref } from 'vue'

// Mock vue-i18n
vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string, params?: any) => params ? `${key} ${JSON.stringify(params)}` : key,
  }),
}))

// Mock workflow store
const mockWorkflows = [
  {
    id: 'workflow-1',
    name: 'Test Workflow',
    description: 'A test workflow',
    icon: 'fas fa-cogs',
    color: 'teal',
    steps: [{ id: 'step-1', operation: 'summarize', name: 'Summarize', config: {}, outputVariable: 'result_1' }],
    isTemplate: false,
    isActive: true,
    createdAt: '2024-01-01',
    updatedAt: '2024-01-01',
    runCount: 5,
  },
  {
    id: 'template-1',
    name: 'Template Workflow',
    description: 'A template workflow',
    icon: 'fas fa-magic',
    color: 'purple',
    steps: [{ id: 'step-1', operation: 'extract-entities', name: 'Extract', config: {}, outputVariable: 'result_1' }],
    isTemplate: true,
    isActive: true,
    createdAt: '2024-01-01',
    updatedAt: '2024-01-01',
    runCount: 0,
  },
]

const mockCreateWorkflow = vi.fn()
const mockUpdateWorkflow = vi.fn()
const mockDeleteWorkflow = vi.fn()
const mockDuplicateWorkflow = vi.fn().mockReturnValue({ ...mockWorkflows[1], id: 'copy-1', isTemplate: false })
const mockExecuteWorkflow = vi.fn().mockResolvedValue({})
const mockInitialize = vi.fn()

vi.mock('@/stores/aiWorkflows', () => ({
  useAIWorkflowsStore: () => ({
    workflows: mockWorkflows,
    templateWorkflows: mockWorkflows.filter(w => w.isTemplate),
    customWorkflows: mockWorkflows.filter(w => !w.isTemplate),
    activeExecution: null,
    initialize: mockInitialize,
    createWorkflow: mockCreateWorkflow,
    updateWorkflow: mockUpdateWorkflow,
    deleteWorkflow: mockDeleteWorkflow,
    duplicateWorkflow: mockDuplicateWorkflow,
    executeWorkflow: mockExecuteWorkflow,
  }),
}))

function mountComponent(props = {}) {
  return shallowMount(AIWorkflowBuilder, {
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
        AIOperationProgress: true,
      },
    },
  })
}

describe('AIWorkflowBuilder', () => {
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
      expect(wrapper.find('.max-w-4xl').exists()).toBe(false)
    })

    it('should render header with sitemap icon', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.fa-sitemap').exists()).toBe(true)
    })

    it('should render close button', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.fa-times').exists()).toBe(true)
    })
  })

  describe('Modes', () => {
    it('should start in list mode', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.mode).toBe('list')
    })

    it('should show create new button in list mode', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.fa-plus').exists()).toBe(true)
    })
  })

  describe('Available Operations', () => {
    it('should have 9 available operations', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.availableOperations.length).toBe(9)
    })

    it('should have extract-entities operation', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.availableOperations.find((o: any) => o.id === 'extract-entities')).toBeTruthy()
    })

    it('should have summarize operation', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.availableOperations.find((o: any) => o.id === 'summarize')).toBeTruthy()
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

    it('should reset state when closed', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.mode = 'edit'
      vm.editingWorkflow = { name: 'test' }
      vm.close()
      expect(vm.mode).toBe('list')
      expect(vm.editingWorkflow).toBeNull()
    })
  })

  describe('Create New Workflow', () => {
    it('should create new workflow object', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.createNewWorkflow()
      expect(vm.editingWorkflow).toBeTruthy()
      expect(vm.editingWorkflow.name).toBe('New Workflow')
      expect(vm.editingWorkflow.steps).toEqual([])
    })

    it('should switch to edit mode', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.createNewWorkflow()
      expect(vm.mode).toBe('edit')
    })
  })

  describe('Edit Workflow', () => {
    it('should edit non-template workflow', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.editWorkflow(mockWorkflows[0])
      expect(vm.editingWorkflow).toBeTruthy()
      expect(vm.editingWorkflow.id).toBe('workflow-1')
      expect(vm.mode).toBe('edit')
    })

    it('should duplicate template workflow for editing', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.editWorkflow(mockWorkflows[1])
      expect(mockDuplicateWorkflow).toHaveBeenCalledWith('template-1')
    })
  })

  describe('Save Workflow', () => {
    it('should call updateWorkflow for existing workflow', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const workflow = { ...mockWorkflows[0] }
      vm.editingWorkflow = workflow
      vm.saveWorkflow()
      expect(mockUpdateWorkflow).toHaveBeenCalledWith('workflow-1', workflow)
    })

    it('should call createWorkflow for new workflow', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.editingWorkflow = { id: '', name: 'New', steps: [] }
      vm.saveWorkflow()
      expect(mockCreateWorkflow).toHaveBeenCalled()
    })

    it('should return to list mode after save', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.editingWorkflow = { id: '', name: 'New', steps: [] }
      vm.mode = 'edit'
      vm.saveWorkflow()
      expect(vm.mode).toBe('list')
    })
  })

  describe('Delete Workflow', () => {
    it('should call deleteWorkflow with confirmation', () => {
      vi.spyOn(window, 'confirm').mockReturnValue(true)
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.deleteWorkflow('workflow-1')
      expect(mockDeleteWorkflow).toHaveBeenCalledWith('workflow-1')
    })

    it('should not delete if not confirmed', () => {
      vi.spyOn(window, 'confirm').mockReturnValue(false)
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.deleteWorkflow('workflow-1')
      expect(mockDeleteWorkflow).not.toHaveBeenCalled()
    })
  })

  describe('Add Step', () => {
    it('should add step to workflow', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.editingWorkflow = { name: 'Test', steps: [] }
      vm.addStep('summarize')
      expect(vm.editingWorkflow.steps.length).toBe(1)
      expect(vm.editingWorkflow.steps[0].operation).toBe('summarize')
    })

    it('should not add step if no editing workflow', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.editingWorkflow = null
      vm.addStep('summarize')
      // Should not throw
    })
  })

  describe('Remove Step', () => {
    it('should remove step from workflow', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.editingWorkflow = {
        name: 'Test',
        steps: [{ id: 'step-1', operation: 'summarize' }],
      }
      vm.removeStep('step-1')
      expect(vm.editingWorkflow.steps.length).toBe(0)
    })
  })

  describe('Move Step', () => {
    it('should move step up', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.editingWorkflow = {
        name: 'Test',
        steps: [
          { id: 'step-1', operation: 'summarize' },
          { id: 'step-2', operation: 'classify' },
        ],
      }
      vm.moveStep(1, 'up')
      expect(vm.editingWorkflow.steps[0].id).toBe('step-2')
    })

    it('should move step down', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.editingWorkflow = {
        name: 'Test',
        steps: [
          { id: 'step-1', operation: 'summarize' },
          { id: 'step-2', operation: 'classify' },
        ],
      }
      vm.moveStep(0, 'down')
      expect(vm.editingWorkflow.steps[0].id).toBe('step-2')
    })

    it('should not move first step up', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.editingWorkflow = {
        name: 'Test',
        steps: [{ id: 'step-1', operation: 'summarize' }],
      }
      vm.moveStep(0, 'up')
      expect(vm.editingWorkflow.steps[0].id).toBe('step-1')
    })

    it('should not move last step down', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.editingWorkflow = {
        name: 'Test',
        steps: [{ id: 'step-1', operation: 'summarize' }],
      }
      vm.moveStep(0, 'down')
      expect(vm.editingWorkflow.steps[0].id).toBe('step-1')
    })
  })

  describe('Run Workflow', () => {
    it('should set running workflow id', async () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      await vm.runWorkflow(mockWorkflows[0])
      expect(mockExecuteWorkflow).toHaveBeenCalled()
    })

    it('should switch to run mode', async () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      // Don't await, just call
      vm.runWorkflow(mockWorkflows[0])
      expect(vm.mode).toBe('run')
    })
  })

  describe('Get Operation Meta', () => {
    it('should return operation metadata', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const meta = vm.getOperationMeta('summarize')
      expect(meta.icon).toBe('fas fa-compress-alt')
      expect(meta.color).toBe('teal')
    })

    it('should return default for unknown operation', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const meta = vm.getOperationMeta('unknown')
      expect(meta.icon).toBe('fas fa-cog')
      expect(meta.color).toBe('gray')
    })
  })

  describe('Get Color Class', () => {
    it('should return blue color class', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getColorClass('blue')).toContain('bg-blue-100')
    })

    it('should return teal color class', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getColorClass('teal')).toContain('bg-teal-100')
    })

    it('should return default for unknown color', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getColorClass('unknown')).toContain('bg-gray-100')
    })
  })

  describe('Initialize', () => {
    it('should call store initialize on mount', () => {
      mountComponent()
      expect(mockInitialize).toHaveBeenCalled()
    })
  })

  describe('Header Titles', () => {
    it('should show workflow title in list mode', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('ai.workflow.title')
    })
  })

  describe('Template Workflows', () => {
    it('should display template workflows', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('ai.workflow.templates')
    })
  })

  describe('Custom Workflows', () => {
    it('should display custom workflows', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('ai.workflow.myWorkflows')
    })
  })
})
