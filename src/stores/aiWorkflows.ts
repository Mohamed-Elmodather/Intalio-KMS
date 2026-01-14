import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { AIOperationType } from '@/types/ai'

// ============================================================================
// Types
// ============================================================================

export interface WorkflowStep {
  id: string
  operation: AIOperationType
  name: string
  config: Record<string, any>
  outputVariable?: string
  inputFrom?: string // Reference to previous step's output
}

export interface WorkflowExecution {
  id: string
  workflowId: string
  status: 'pending' | 'running' | 'completed' | 'failed' | 'cancelled'
  currentStep: number
  results: Record<string, any>[]
  startedAt: string
  completedAt?: string
  error?: string
}

export interface AIWorkflow {
  id: string
  name: string
  description: string
  icon: string
  color: string
  steps: WorkflowStep[]
  isTemplate: boolean
  isActive: boolean
  createdAt: string
  updatedAt: string
  runCount: number
  lastRunAt?: string
}

// ============================================================================
// Store
// ============================================================================

export const useAIWorkflowsStore = defineStore('aiWorkflows', () => {
  // State
  const workflows = ref<AIWorkflow[]>([])
  const executions = ref<WorkflowExecution[]>([])
  const activeExecutionId = ref<string | null>(null)

  // ============================================================================
  // Computed
  // ============================================================================

  const customWorkflows = computed(() =>
    workflows.value.filter(w => !w.isTemplate)
  )

  const templateWorkflows = computed(() =>
    workflows.value.filter(w => w.isTemplate)
  )

  const activeExecution = computed(() =>
    executions.value.find(e => e.id === activeExecutionId.value)
  )

  const isExecuting = computed(() =>
    executions.value.some(e => e.status === 'running')
  )

  // ============================================================================
  // Actions
  // ============================================================================

  /**
   * Initialize with template workflows
   */
  function initializeTemplates() {
    const templates: AIWorkflow[] = [
      {
        id: 'template-doc-processing',
        name: 'Document Processing',
        description: 'Extract text, summarize, classify, and auto-tag documents',
        icon: 'fas fa-file-invoice',
        color: 'teal',
        steps: [
          { id: '1', operation: 'ocr', name: 'Extract Text', config: {}, outputVariable: 'extractedText' },
          { id: '2', operation: 'summarize', name: 'Generate Summary', config: { type: 'detailed' }, inputFrom: 'extractedText', outputVariable: 'summary' },
          { id: '3', operation: 'classify', name: 'Classify Content', config: {}, inputFrom: 'extractedText', outputVariable: 'classification' },
          { id: '4', operation: 'auto-tag', name: 'Generate Tags', config: {}, inputFrom: 'extractedText', outputVariable: 'tags' }
        ],
        isTemplate: true,
        isActive: true,
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString(),
        runCount: 0
      },
      {
        id: 'template-content-audit',
        name: 'Content Audit',
        description: 'Analyze sentiment, extract entities, and classify content',
        icon: 'fas fa-clipboard-check',
        color: 'purple',
        steps: [
          { id: '1', operation: 'analyze-sentiment', name: 'Analyze Sentiment', config: {}, outputVariable: 'sentiment' },
          { id: '2', operation: 'extract-entities', name: 'Extract Entities', config: {}, outputVariable: 'entities' },
          { id: '3', operation: 'classify', name: 'Classify', config: {}, outputVariable: 'classification' }
        ],
        isTemplate: true,
        isActive: true,
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString(),
        runCount: 0
      },
      {
        id: 'template-multi-translate',
        name: 'Multi-Language Translation',
        description: 'Translate content to multiple languages simultaneously',
        icon: 'fas fa-globe',
        color: 'green',
        steps: [
          { id: '1', operation: 'translate', name: 'Translate to Arabic', config: { targetLanguage: 'ar' }, outputVariable: 'arabic' },
          { id: '2', operation: 'translate', name: 'Translate to French', config: { targetLanguage: 'fr' }, outputVariable: 'french' },
          { id: '3', operation: 'translate', name: 'Translate to Spanish', config: { targetLanguage: 'es' }, outputVariable: 'spanish' }
        ],
        isTemplate: true,
        isActive: true,
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString(),
        runCount: 0
      },
      {
        id: 'template-research',
        name: 'Research Assistant',
        description: 'Search, summarize, and extract key information',
        icon: 'fas fa-microscope',
        color: 'blue',
        steps: [
          { id: '1', operation: 'smart-search', name: 'Smart Search', config: {}, outputVariable: 'searchResults' },
          { id: '2', operation: 'summarize', name: 'Summarize Results', config: { type: 'bullet' }, inputFrom: 'searchResults', outputVariable: 'summary' },
          { id: '3', operation: 'extract-entities', name: 'Extract Key Entities', config: {}, inputFrom: 'searchResults', outputVariable: 'entities' }
        ],
        isTemplate: true,
        isActive: true,
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString(),
        runCount: 0
      }
    ]

    // Only add templates that don't exist
    for (const template of templates) {
      if (!workflows.value.find(w => w.id === template.id)) {
        workflows.value.push(template)
      }
    }
  }

  /**
   * Create a new workflow
   */
  function createWorkflow(workflow: Omit<AIWorkflow, 'id' | 'createdAt' | 'updatedAt' | 'runCount'>): AIWorkflow {
    const newWorkflow: AIWorkflow = {
      ...workflow,
      id: `workflow-${Date.now()}`,
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
      runCount: 0
    }

    workflows.value.push(newWorkflow)
    saveToStorage()
    return newWorkflow
  }

  /**
   * Update an existing workflow
   */
  function updateWorkflow(id: string, updates: Partial<AIWorkflow>) {
    const index = workflows.value.findIndex(w => w.id === id)
    if (index !== -1 && !workflows.value[index].isTemplate) {
      workflows.value[index] = {
        ...workflows.value[index],
        ...updates,
        updatedAt: new Date().toISOString()
      }
      saveToStorage()
    }
  }

  /**
   * Delete a workflow
   */
  function deleteWorkflow(id: string) {
    const index = workflows.value.findIndex(w => w.id === id)
    if (index !== -1 && !workflows.value[index].isTemplate) {
      workflows.value.splice(index, 1)
      saveToStorage()
    }
  }

  /**
   * Duplicate a workflow (including templates)
   */
  function duplicateWorkflow(id: string): AIWorkflow | null {
    const original = workflows.value.find(w => w.id === id)
    if (!original) return null

    return createWorkflow({
      ...original,
      name: `${original.name} (Copy)`,
      isTemplate: false,
      isActive: true
    })
  }

  /**
   * Add a step to a workflow
   */
  function addStep(workflowId: string, step: Omit<WorkflowStep, 'id'>) {
    const workflow = workflows.value.find(w => w.id === workflowId)
    if (workflow && !workflow.isTemplate) {
      const newStep: WorkflowStep = {
        ...step,
        id: `step-${Date.now()}`
      }
      workflow.steps.push(newStep)
      workflow.updatedAt = new Date().toISOString()
      saveToStorage()
    }
  }

  /**
   * Remove a step from a workflow
   */
  function removeStep(workflowId: string, stepId: string) {
    const workflow = workflows.value.find(w => w.id === workflowId)
    if (workflow && !workflow.isTemplate) {
      const index = workflow.steps.findIndex(s => s.id === stepId)
      if (index !== -1) {
        workflow.steps.splice(index, 1)
        workflow.updatedAt = new Date().toISOString()
        saveToStorage()
      }
    }
  }

  /**
   * Reorder steps in a workflow
   */
  function reorderSteps(workflowId: string, fromIndex: number, toIndex: number) {
    const workflow = workflows.value.find(w => w.id === workflowId)
    if (workflow && !workflow.isTemplate) {
      const [step] = workflow.steps.splice(fromIndex, 1)
      workflow.steps.splice(toIndex, 0, step)
      workflow.updatedAt = new Date().toISOString()
      saveToStorage()
    }
  }

  /**
   * Execute a workflow
   */
  async function executeWorkflow(
    workflowId: string,
    input: string,
    onProgress?: (step: number, total: number, result: any) => void
  ): Promise<WorkflowExecution> {
    const workflow = workflows.value.find(w => w.id === workflowId)
    if (!workflow) {
      throw new Error('Workflow not found')
    }

    const execution: WorkflowExecution = {
      id: `exec-${Date.now()}`,
      workflowId,
      status: 'running',
      currentStep: 0,
      results: [],
      startedAt: new Date().toISOString()
    }

    executions.value.push(execution)
    activeExecutionId.value = execution.id

    try {
      let currentInput = input
      const results: Record<string, any> = {}

      for (let i = 0; i < workflow.steps.length; i++) {
        const step = workflow.steps[i]
        execution.currentStep = i

        // Simulate AI operation (in real implementation, call actual AI services)
        await new Promise(resolve => setTimeout(resolve, 1000 + Math.random() * 1000))

        // Mock result
        const result = {
          stepId: step.id,
          operation: step.operation,
          success: true,
          output: `Result of ${step.name}: Mock output for ${step.operation}`,
          timestamp: new Date().toISOString()
        }

        execution.results.push(result)

        if (step.outputVariable) {
          results[step.outputVariable] = result.output
        }

        // Use output for next step if specified
        if (step.inputFrom && results[step.inputFrom]) {
          currentInput = results[step.inputFrom]
        }

        onProgress?.(i + 1, workflow.steps.length, result)
      }

      execution.status = 'completed'
      execution.completedAt = new Date().toISOString()

      // Update workflow stats
      workflow.runCount++
      workflow.lastRunAt = new Date().toISOString()
      saveToStorage()

    } catch (error) {
      execution.status = 'failed'
      execution.error = error instanceof Error ? error.message : 'Unknown error'
      execution.completedAt = new Date().toISOString()
    }

    activeExecutionId.value = null
    return execution
  }

  /**
   * Cancel a running execution
   */
  function cancelExecution(executionId: string) {
    const execution = executions.value.find(e => e.id === executionId)
    if (execution && execution.status === 'running') {
      execution.status = 'cancelled'
      execution.completedAt = new Date().toISOString()
      if (activeExecutionId.value === executionId) {
        activeExecutionId.value = null
      }
    }
  }

  /**
   * Clear execution history
   */
  function clearExecutions() {
    executions.value = []
    activeExecutionId.value = null
  }

  // ============================================================================
  // Persistence
  // ============================================================================

  function saveToStorage() {
    try {
      const customOnly = workflows.value.filter(w => !w.isTemplate)
      localStorage.setItem('ai_workflows', JSON.stringify(customOnly))
    } catch (error) {
      console.error('Failed to save workflows:', error)
    }
  }

  function loadFromStorage() {
    try {
      const stored = localStorage.getItem('ai_workflows')
      if (stored) {
        const customWorkflows = JSON.parse(stored) as AIWorkflow[]
        workflows.value = [...workflows.value.filter(w => w.isTemplate), ...customWorkflows]
      }
    } catch (error) {
      console.error('Failed to load workflows:', error)
    }
  }

  function initialize() {
    initializeTemplates()
    loadFromStorage()
  }

  // ============================================================================
  // Return
  // ============================================================================

  return {
    // State
    workflows,
    executions,
    activeExecutionId,

    // Computed
    customWorkflows,
    templateWorkflows,
    activeExecution,
    isExecuting,

    // Actions
    initialize,
    createWorkflow,
    updateWorkflow,
    deleteWorkflow,
    duplicateWorkflow,
    addStep,
    removeStep,
    reorderSteps,
    executeWorkflow,
    cancelExecution,
    clearExecutions,

    // Persistence
    saveToStorage,
    loadFromStorage
  }
})
