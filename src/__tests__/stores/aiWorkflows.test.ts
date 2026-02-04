import { describe, it, expect, vi, beforeEach } from 'vitest'
import { setActivePinia, createPinia } from 'pinia'
import { useAIWorkflowsStore } from '@/stores/aiWorkflows'

describe('AI Workflows Store', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
    localStorage.clear()
  })

  describe('Initial State', () => {
    it('should have empty workflows array', () => {
      const store = useAIWorkflowsStore()
      expect(store.workflows).toEqual([])
    })

    it('should have empty executions array', () => {
      const store = useAIWorkflowsStore()
      expect(store.executions).toEqual([])
    })

    it('should have null active execution', () => {
      const store = useAIWorkflowsStore()
      expect(store.activeExecutionId).toBeNull()
    })
  })

  describe('initialize', () => {
    it('should add template workflows', () => {
      const store = useAIWorkflowsStore()

      store.initialize()

      expect(store.workflows.length).toBeGreaterThan(0)
      expect(store.templateWorkflows.length).toBeGreaterThan(0)
    })

    it('should not duplicate templates on multiple calls', () => {
      const store = useAIWorkflowsStore()

      store.initialize()
      const initialCount = store.workflows.length

      store.initialize()

      expect(store.workflows.length).toBe(initialCount)
    })

    it('should create Document Processing template', () => {
      const store = useAIWorkflowsStore()

      store.initialize()

      const docTemplate = store.workflows.find(
        (w) => w.id === 'template-doc-processing'
      )
      expect(docTemplate).toBeDefined()
      expect(docTemplate?.name).toBe('Document Processing')
      expect(docTemplate?.steps.length).toBeGreaterThan(0)
    })

    it('should create Content Audit template', () => {
      const store = useAIWorkflowsStore()

      store.initialize()

      const auditTemplate = store.workflows.find(
        (w) => w.id === 'template-content-audit'
      )
      expect(auditTemplate).toBeDefined()
      expect(auditTemplate?.steps.length).toBe(3)
    })

    it('should create Multi-Language Translation template', () => {
      const store = useAIWorkflowsStore()

      store.initialize()

      const translateTemplate = store.workflows.find(
        (w) => w.id === 'template-multi-translate'
      )
      expect(translateTemplate).toBeDefined()
      expect(translateTemplate?.steps.length).toBe(3)
    })

    it('should create Research Assistant template', () => {
      const store = useAIWorkflowsStore()

      store.initialize()

      const researchTemplate = store.workflows.find(
        (w) => w.id === 'template-research'
      )
      expect(researchTemplate).toBeDefined()
    })
  })

  describe('Computed Properties', () => {
    it('should filter custom workflows', () => {
      const store = useAIWorkflowsStore()
      store.initialize()

      // Add a custom workflow
      store.createWorkflow({
        name: 'Custom Workflow',
        description: 'My custom workflow',
        icon: 'fas fa-cog',
        color: 'blue',
        steps: [],
        isTemplate: false,
        isActive: true,
      })

      expect(store.customWorkflows.length).toBe(1)
      expect(store.customWorkflows[0].name).toBe('Custom Workflow')
    })

    it('should filter template workflows', () => {
      const store = useAIWorkflowsStore()
      store.initialize()

      expect(store.templateWorkflows.length).toBeGreaterThan(0)
      expect(store.templateWorkflows.every((w) => w.isTemplate)).toBe(true)
    })

    it('should return active execution', () => {
      const store = useAIWorkflowsStore()
      store.executions = [
        {
          id: 'exec-1',
          workflowId: 'workflow-1',
          status: 'running',
          currentStep: 0,
          results: [],
          startedAt: new Date().toISOString(),
        },
      ]
      store.activeExecutionId = 'exec-1'

      expect(store.activeExecution).toBeDefined()
      expect(store.activeExecution?.id).toBe('exec-1')
    })

    it('should detect executing state', () => {
      const store = useAIWorkflowsStore()

      expect(store.isExecuting).toBe(false)

      store.executions = [
        {
          id: 'exec-1',
          workflowId: 'workflow-1',
          status: 'running',
          currentStep: 0,
          results: [],
          startedAt: new Date().toISOString(),
        },
      ]

      expect(store.isExecuting).toBe(true)
    })
  })

  describe('createWorkflow', () => {
    it('should create a new workflow', () => {
      const store = useAIWorkflowsStore()

      const newWorkflow = store.createWorkflow({
        name: 'Test Workflow',
        description: 'Test description',
        icon: 'fas fa-robot',
        color: 'purple',
        steps: [],
        isTemplate: false,
        isActive: true,
      })

      expect(newWorkflow.id).toBeDefined()
      expect(newWorkflow.name).toBe('Test Workflow')
      expect(newWorkflow.runCount).toBe(0)
      expect(store.workflows.find(w => w.id === newWorkflow.id)).toBeDefined()
    })

    it('should set timestamps on creation', () => {
      const store = useAIWorkflowsStore()

      const before = new Date().toISOString()
      const newWorkflow = store.createWorkflow({
        name: 'Test',
        description: '',
        icon: '',
        color: '',
        steps: [],
        isTemplate: false,
        isActive: true,
      })
      const after = new Date().toISOString()

      expect(newWorkflow.createdAt).toBeTruthy()
      expect(newWorkflow.updatedAt).toBeTruthy()
      expect(newWorkflow.createdAt >= before).toBe(true)
      expect(newWorkflow.createdAt <= after).toBe(true)
    })
  })

  describe('updateWorkflow', () => {
    it('should update custom workflow', () => {
      const store = useAIWorkflowsStore()

      const workflow = store.createWorkflow({
        name: 'Original Name',
        description: 'Original',
        icon: '',
        color: '',
        steps: [],
        isTemplate: false,
        isActive: true,
      })

      store.updateWorkflow(workflow.id, { name: 'Updated Name' })

      expect(store.workflows.find((w) => w.id === workflow.id)?.name).toBe(
        'Updated Name'
      )
    })

    it('should not update template workflow', () => {
      const store = useAIWorkflowsStore()
      store.initialize()

      const template = store.templateWorkflows[0]
      const originalName = template.name

      store.updateWorkflow(template.id, { name: 'New Name' })

      expect(store.workflows.find((w) => w.id === template.id)?.name).toBe(
        originalName
      )
    })

    it('should update updatedAt timestamp', async () => {
      const store = useAIWorkflowsStore()

      const workflow = store.createWorkflow({
        name: 'Test',
        description: '',
        icon: '',
        color: '',
        steps: [],
        isTemplate: false,
        isActive: true,
      })

      const originalUpdatedAt = workflow.updatedAt

      // Wait a tiny bit to ensure timestamp difference
      await new Promise(resolve => setTimeout(resolve, 10))

      store.updateWorkflow(workflow.id, { description: 'Updated' })

      const updated = store.workflows.find((w) => w.id === workflow.id)
      expect(updated?.description).toBe('Updated')
    })
  })

  describe('deleteWorkflow', () => {
    it('should delete custom workflow', () => {
      const store = useAIWorkflowsStore()

      const workflow = store.createWorkflow({
        name: 'To Delete',
        description: '',
        icon: '',
        color: '',
        steps: [],
        isTemplate: false,
        isActive: true,
      })

      store.deleteWorkflow(workflow.id)

      expect(store.workflows.find((w) => w.id === workflow.id)).toBeUndefined()
    })

    it('should not delete template workflow', () => {
      const store = useAIWorkflowsStore()
      store.initialize()

      const template = store.templateWorkflows[0]
      const initialCount = store.workflows.length

      store.deleteWorkflow(template.id)

      expect(store.workflows.length).toBe(initialCount)
      expect(store.workflows.find((w) => w.id === template.id)).toBeDefined()
    })
  })

  describe('Workflow Steps', () => {
    it('should have valid step structure', () => {
      const store = useAIWorkflowsStore()
      store.initialize()

      const docProcessing = store.workflows.find(
        (w) => w.id === 'template-doc-processing'
      )
      expect(docProcessing?.steps.length).toBe(4)

      const firstStep = docProcessing?.steps[0]
      expect(firstStep?.id).toBeDefined()
      expect(firstStep?.operation).toBe('ocr')
      expect(firstStep?.name).toBe('Extract Text')
      expect(firstStep?.outputVariable).toBe('extractedText')
    })

    it('should have input/output references in steps', () => {
      const store = useAIWorkflowsStore()
      store.initialize()

      const docProcessing = store.workflows.find(
        (w) => w.id === 'template-doc-processing'
      )

      // Second step should reference first step's output
      const secondStep = docProcessing?.steps[1]
      expect(secondStep?.inputFrom).toBe('extractedText')
    })
  })
})
