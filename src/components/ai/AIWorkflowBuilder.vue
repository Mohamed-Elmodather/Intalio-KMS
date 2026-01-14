<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useAIWorkflowsStore, type AIWorkflow, type WorkflowStep } from '@/stores/aiWorkflows'
import type { AIOperationType } from '@/types/ai'
import AIOperationProgress from './AIOperationProgress.vue'

interface Props {
  modelValue: boolean
  workflowId?: string | null
}

const props = withDefaults(defineProps<Props>(), {
  workflowId: null
})

const emit = defineEmits<{
  'update:modelValue': [value: boolean]
  'run': [workflow: AIWorkflow]
}>()

const workflowsStore = useAIWorkflowsStore()

// Local state
const mode = ref<'list' | 'edit' | 'run'>('list')
const editingWorkflow = ref<AIWorkflow | null>(null)
const runningWorkflowId = ref<string | null>(null)
const runProgress = ref({ step: 0, total: 0 })
const inputContent = ref('')

// Available operations
const availableOperations: { id: AIOperationType; name: string; icon: string; color: string }[] = [
  { id: 'extract-entities', name: 'Extract Entities', icon: 'fas fa-tags', color: 'blue' },
  { id: 'analyze-sentiment', name: 'Sentiment Analysis', icon: 'fas fa-smile', color: 'pink' },
  { id: 'summarize', name: 'Summarize', icon: 'fas fa-compress-alt', color: 'teal' },
  { id: 'classify', name: 'Classify', icon: 'fas fa-folder-tree', color: 'purple' },
  { id: 'ocr', name: 'OCR', icon: 'fas fa-file-image', color: 'amber' },
  { id: 'translate', name: 'Translate', icon: 'fas fa-language', color: 'green' },
  { id: 'generate-title', name: 'Generate Titles', icon: 'fas fa-heading', color: 'indigo' },
  { id: 'auto-tag', name: 'Auto-Tag', icon: 'fas fa-hashtag', color: 'cyan' },
  { id: 'smart-search', name: 'Smart Search', icon: 'fas fa-search', color: 'orange' }
]

// Initialize store on mount
workflowsStore.initialize()

// Watch for workflowId changes
watch(() => props.workflowId, (id) => {
  if (id) {
    const workflow = workflowsStore.workflows.find(w => w.id === id)
    if (workflow) {
      editingWorkflow.value = JSON.parse(JSON.stringify(workflow))
      mode.value = 'edit'
    }
  }
})

function close() {
  emit('update:modelValue', false)
  mode.value = 'list'
  editingWorkflow.value = null
  runningWorkflowId.value = null
}

function createNewWorkflow() {
  editingWorkflow.value = {
    id: '',
    name: 'New Workflow',
    description: '',
    icon: 'fas fa-cogs',
    color: 'teal',
    steps: [],
    isTemplate: false,
    isActive: true,
    createdAt: '',
    updatedAt: '',
    runCount: 0
  }
  mode.value = 'edit'
}

function editWorkflow(workflow: AIWorkflow) {
  if (workflow.isTemplate) {
    // Duplicate template for editing
    const copy = workflowsStore.duplicateWorkflow(workflow.id)
    if (copy) {
      editingWorkflow.value = JSON.parse(JSON.stringify(copy))
      mode.value = 'edit'
    }
  } else {
    editingWorkflow.value = JSON.parse(JSON.stringify(workflow))
    mode.value = 'edit'
  }
}

function saveWorkflow() {
  if (!editingWorkflow.value) return

  if (editingWorkflow.value.id) {
    workflowsStore.updateWorkflow(editingWorkflow.value.id, editingWorkflow.value)
  } else {
    workflowsStore.createWorkflow(editingWorkflow.value)
  }

  mode.value = 'list'
  editingWorkflow.value = null
}

function deleteWorkflow(id: string) {
  if (confirm('Are you sure you want to delete this workflow?')) {
    workflowsStore.deleteWorkflow(id)
  }
}

function addStep(operation: AIOperationType) {
  if (!editingWorkflow.value) return

  const op = availableOperations.find(o => o.id === operation)
  if (!op) return

  const step: WorkflowStep = {
    id: `step-${Date.now()}`,
    operation,
    name: op.name,
    config: {},
    outputVariable: `result_${editingWorkflow.value.steps.length + 1}`
  }

  editingWorkflow.value.steps.push(step)
}

function removeStep(stepId: string) {
  if (!editingWorkflow.value) return
  editingWorkflow.value.steps = editingWorkflow.value.steps.filter(s => s.id !== stepId)
}

function moveStep(fromIndex: number, direction: 'up' | 'down') {
  if (!editingWorkflow.value) return

  const toIndex = direction === 'up' ? fromIndex - 1 : fromIndex + 1
  if (toIndex < 0 || toIndex >= editingWorkflow.value.steps.length) return

  const steps = [...editingWorkflow.value.steps]
  const [step] = steps.splice(fromIndex, 1)
  steps.splice(toIndex, 0, step)
  editingWorkflow.value.steps = steps
}

async function runWorkflow(workflow: AIWorkflow) {
  runningWorkflowId.value = workflow.id
  runProgress.value = { step: 0, total: workflow.steps.length }
  mode.value = 'run'

  try {
    await workflowsStore.executeWorkflow(
      workflow.id,
      inputContent.value || 'Sample input content',
      (step, total, result) => {
        runProgress.value = { step, total }
      }
    )
  } finally {
    runningWorkflowId.value = null
  }
}

function getOperationMeta(operation: AIOperationType) {
  return availableOperations.find(o => o.id === operation) || {
    name: operation,
    icon: 'fas fa-cog',
    color: 'gray'
  }
}

function getColorClass(color: string): string {
  const colors: Record<string, string> = {
    blue: 'bg-blue-100 text-blue-600',
    pink: 'bg-pink-100 text-pink-600',
    teal: 'bg-teal-100 text-teal-600',
    purple: 'bg-purple-100 text-purple-600',
    amber: 'bg-amber-100 text-amber-600',
    green: 'bg-green-100 text-green-600',
    indigo: 'bg-indigo-100 text-indigo-600',
    cyan: 'bg-cyan-100 text-cyan-600',
    orange: 'bg-orange-100 text-orange-600'
  }
  return colors[color] || 'bg-gray-100 text-gray-600'
}
</script>

<template>
  <Teleport to="body">
    <Transition name="modal">
      <div
        v-if="modelValue"
        class="fixed inset-0 z-[70] flex items-center justify-center p-4"
      >
        <!-- Backdrop -->
        <div class="absolute inset-0 bg-black/50 backdrop-blur-sm" @click="close"></div>

        <!-- Modal -->
        <div class="relative w-full max-w-4xl max-h-[85vh] bg-white rounded-2xl shadow-2xl flex flex-col overflow-hidden">
          <!-- Header -->
          <div class="flex items-center justify-between px-6 py-4 border-b border-gray-100">
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-purple-500 to-indigo-500 flex items-center justify-center">
                <i class="fas fa-sitemap text-white"></i>
              </div>
              <div>
                <h2 class="text-lg font-semibold text-gray-900">
                  {{ mode === 'list' ? 'AI Workflows' : mode === 'edit' ? 'Edit Workflow' : 'Running Workflow' }}
                </h2>
                <p class="text-sm text-gray-500">
                  {{ mode === 'list' ? 'Chain AI operations together' : mode === 'edit' ? 'Configure workflow steps' : 'Executing operations...' }}
                </p>
              </div>
            </div>

            <div class="flex items-center gap-2">
              <button
                v-if="mode === 'edit'"
                @click="mode = 'list'"
                class="px-3 py-1.5 text-sm text-gray-600 hover:bg-gray-100 rounded-lg transition-colors"
              >
                Cancel
              </button>
              <button
                @click="close"
                class="w-8 h-8 rounded-lg text-gray-400 hover:text-gray-600 hover:bg-gray-100 flex items-center justify-center transition-colors"
              >
                <i class="fas fa-times"></i>
              </button>
            </div>
          </div>

          <!-- Content -->
          <div class="flex-1 overflow-auto p-6">
            <!-- List Mode -->
            <template v-if="mode === 'list'">
              <!-- Create New Button -->
              <button
                @click="createNewWorkflow"
                class="w-full p-4 rounded-xl border-2 border-dashed border-gray-200 hover:border-teal-300 hover:bg-teal-50/50 transition-all flex items-center justify-center gap-2 text-gray-500 hover:text-teal-600 mb-6"
              >
                <i class="fas fa-plus"></i>
                <span class="font-medium">Create New Workflow</span>
              </button>

              <!-- Template Workflows -->
              <div v-if="workflowsStore.templateWorkflows.length" class="mb-6">
                <h3 class="text-sm font-semibold text-gray-500 uppercase tracking-wide mb-3">
                  Templates
                </h3>
                <div class="grid grid-cols-2 gap-3">
                  <div
                    v-for="workflow in workflowsStore.templateWorkflows"
                    :key="workflow.id"
                    class="workflow-card group"
                  >
                    <div class="flex items-start gap-3">
                      <div :class="['w-10 h-10 rounded-xl flex items-center justify-center', getColorClass(workflow.color)]">
                        <i :class="workflow.icon"></i>
                      </div>
                      <div class="flex-1 min-w-0">
                        <h4 class="font-medium text-gray-800 truncate">{{ workflow.name }}</h4>
                        <p class="text-xs text-gray-500 line-clamp-2">{{ workflow.description }}</p>
                        <div class="flex items-center gap-2 mt-2">
                          <span class="text-[10px] text-gray-400">
                            {{ workflow.steps.length }} steps
                          </span>
                        </div>
                      </div>
                    </div>
                    <div class="flex items-center gap-2 mt-3 pt-3 border-t border-gray-100">
                      <button
                        @click="editWorkflow(workflow)"
                        class="flex-1 py-1.5 text-xs font-medium text-gray-600 hover:text-gray-800 bg-gray-100 hover:bg-gray-200 rounded-lg transition-colors"
                      >
                        <i class="fas fa-copy mr-1"></i>
                        Use Template
                      </button>
                      <button
                        @click="runWorkflow(workflow)"
                        class="flex-1 py-1.5 text-xs font-medium text-white bg-teal-500 hover:bg-teal-600 rounded-lg transition-colors"
                      >
                        <i class="fas fa-play mr-1"></i>
                        Run
                      </button>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Custom Workflows -->
              <div v-if="workflowsStore.customWorkflows.length">
                <h3 class="text-sm font-semibold text-gray-500 uppercase tracking-wide mb-3">
                  My Workflows
                </h3>
                <div class="space-y-2">
                  <div
                    v-for="workflow in workflowsStore.customWorkflows"
                    :key="workflow.id"
                    class="workflow-card group flex items-center"
                  >
                    <div :class="['w-10 h-10 rounded-xl flex items-center justify-center flex-shrink-0', getColorClass(workflow.color)]">
                      <i :class="workflow.icon"></i>
                    </div>
                    <div class="flex-1 min-w-0 ml-3">
                      <h4 class="font-medium text-gray-800 truncate">{{ workflow.name }}</h4>
                      <p class="text-xs text-gray-500 truncate">{{ workflow.steps.length }} steps • Run {{ workflow.runCount }} times</p>
                    </div>
                    <div class="flex items-center gap-1 opacity-0 group-hover:opacity-100 transition-opacity">
                      <button
                        @click="editWorkflow(workflow)"
                        class="w-8 h-8 rounded-lg text-gray-400 hover:text-gray-600 hover:bg-gray-100 flex items-center justify-center transition-colors"
                        title="Edit"
                      >
                        <i class="fas fa-edit text-sm"></i>
                      </button>
                      <button
                        @click="runWorkflow(workflow)"
                        class="w-8 h-8 rounded-lg text-teal-500 hover:text-teal-600 hover:bg-teal-50 flex items-center justify-center transition-colors"
                        title="Run"
                      >
                        <i class="fas fa-play text-sm"></i>
                      </button>
                      <button
                        @click="deleteWorkflow(workflow.id)"
                        class="w-8 h-8 rounded-lg text-red-400 hover:text-red-600 hover:bg-red-50 flex items-center justify-center transition-colors"
                        title="Delete"
                      >
                        <i class="fas fa-trash text-sm"></i>
                      </button>
                    </div>
                  </div>
                </div>
              </div>
            </template>

            <!-- Edit Mode -->
            <template v-else-if="mode === 'edit' && editingWorkflow">
              <!-- Workflow Info -->
              <div class="grid grid-cols-2 gap-4 mb-6">
                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-1">Name</label>
                  <input
                    v-model="editingWorkflow.name"
                    type="text"
                    class="w-full px-3 py-2 rounded-lg border border-gray-200 focus:border-teal-500 focus:ring-2 focus:ring-teal-500/20 outline-none transition-all"
                    placeholder="Workflow name"
                  />
                </div>
                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-1">Description</label>
                  <input
                    v-model="editingWorkflow.description"
                    type="text"
                    class="w-full px-3 py-2 rounded-lg border border-gray-200 focus:border-teal-500 focus:ring-2 focus:ring-teal-500/20 outline-none transition-all"
                    placeholder="What does this workflow do?"
                  />
                </div>
              </div>

              <!-- Steps -->
              <div class="mb-6">
                <h3 class="text-sm font-semibold text-gray-700 mb-3">Workflow Steps</h3>

                <div v-if="editingWorkflow.steps.length === 0" class="text-center py-8 bg-gray-50 rounded-xl">
                  <i class="fas fa-layer-group text-3xl text-gray-300 mb-2"></i>
                  <p class="text-gray-500">No steps added yet</p>
                  <p class="text-xs text-gray-400">Add operations below to build your workflow</p>
                </div>

                <div v-else class="space-y-2">
                  <div
                    v-for="(step, index) in editingWorkflow.steps"
                    :key="step.id"
                    class="flex items-center gap-2 p-3 bg-gray-50 rounded-xl group"
                  >
                    <span class="w-6 h-6 rounded-full bg-gray-200 text-gray-600 text-xs font-medium flex items-center justify-center">
                      {{ index + 1 }}
                    </span>
                    <div :class="['w-8 h-8 rounded-lg flex items-center justify-center', getColorClass(getOperationMeta(step.operation).color)]">
                      <i :class="getOperationMeta(step.operation).icon" class="text-sm"></i>
                    </div>
                    <span class="flex-1 font-medium text-gray-700">{{ step.name }}</span>
                    <div class="flex items-center gap-1 opacity-0 group-hover:opacity-100 transition-opacity">
                      <button
                        @click="moveStep(index, 'up')"
                        :disabled="index === 0"
                        class="w-6 h-6 rounded text-gray-400 hover:text-gray-600 disabled:opacity-30 disabled:cursor-not-allowed"
                      >
                        <i class="fas fa-chevron-up text-xs"></i>
                      </button>
                      <button
                        @click="moveStep(index, 'down')"
                        :disabled="index === editingWorkflow.steps.length - 1"
                        class="w-6 h-6 rounded text-gray-400 hover:text-gray-600 disabled:opacity-30 disabled:cursor-not-allowed"
                      >
                        <i class="fas fa-chevron-down text-xs"></i>
                      </button>
                      <button
                        @click="removeStep(step.id)"
                        class="w-6 h-6 rounded text-red-400 hover:text-red-600"
                      >
                        <i class="fas fa-times text-xs"></i>
                      </button>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Add Operations -->
              <div>
                <h3 class="text-sm font-semibold text-gray-700 mb-3">Add Operation</h3>
                <div class="grid grid-cols-3 gap-2">
                  <button
                    v-for="op in availableOperations"
                    :key="op.id"
                    @click="addStep(op.id)"
                    class="flex items-center gap-2 p-2.5 rounded-lg border border-gray-200 hover:border-teal-300 hover:bg-teal-50/50 transition-all text-left"
                  >
                    <div :class="['w-8 h-8 rounded-lg flex items-center justify-center', getColorClass(op.color)]">
                      <i :class="op.icon" class="text-sm"></i>
                    </div>
                    <span class="text-sm font-medium text-gray-700">{{ op.name }}</span>
                  </button>
                </div>
              </div>
            </template>

            <!-- Run Mode -->
            <template v-else-if="mode === 'run'">
              <div class="space-y-4">
                <div class="text-center mb-6">
                  <div class="w-16 h-16 rounded-full bg-teal-100 flex items-center justify-center mx-auto mb-3">
                    <i class="fas fa-cogs text-2xl text-teal-600 animate-spin"></i>
                  </div>
                  <p class="text-gray-600">
                    Processing step {{ runProgress.step }} of {{ runProgress.total }}
                  </p>
                </div>

                <div v-if="workflowsStore.activeExecution" class="space-y-2">
                  <div
                    v-for="(result, index) in workflowsStore.activeExecution.results"
                    :key="index"
                    class="p-3 bg-green-50 rounded-lg border border-green-200"
                  >
                    <div class="flex items-center gap-2">
                      <i class="fas fa-check-circle text-green-500"></i>
                      <span class="font-medium text-green-700">Step {{ index + 1 }} Complete</span>
                    </div>
                    <p class="text-sm text-green-600 mt-1">{{ result.output }}</p>
                  </div>
                </div>
              </div>
            </template>
          </div>

          <!-- Footer -->
          <div v-if="mode === 'edit'" class="flex items-center justify-end gap-2 px-6 py-4 border-t border-gray-100 bg-gray-50/50">
            <button
              @click="mode = 'list'"
              class="px-4 py-2 text-gray-600 hover:bg-gray-100 rounded-lg transition-colors"
            >
              Cancel
            </button>
            <button
              @click="saveWorkflow"
              :disabled="!editingWorkflow?.name || editingWorkflow?.steps.length === 0"
              class="px-4 py-2 bg-teal-500 text-white rounded-lg hover:bg-teal-600 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <i class="fas fa-save mr-2"></i>
              Save Workflow
            </button>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<style scoped>
.modal-enter-active,
.modal-leave-active {
  transition: all 0.3s ease;
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}

.modal-enter-from .relative,
.modal-leave-to .relative {
  transform: scale(0.95);
}

.workflow-card {
  @apply p-4 bg-white rounded-xl border border-gray-100 hover:border-teal-200 hover:shadow-md transition-all;
}
</style>
