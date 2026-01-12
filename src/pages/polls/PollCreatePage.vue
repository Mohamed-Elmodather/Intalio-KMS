<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useUIStore } from '@/stores/ui'
import { useAIServicesStore } from '@/stores/aiServices'
import { AILoadingIndicator, AISuggestionChip } from '@/components/ai'

const router = useRouter()
const uiStore = useUIStore()
const aiStore = useAIServicesStore()

const question = ref('')
const options = ref(['', ''])
const allowMultiple = ref(false)
const isAnonymous = ref(true)
const endDate = ref('')

// AI State
const showAISuggestionsModal = ref(false)
const isGeneratingSuggestions = ref(false)
const aiSuggestions = ref<AIPollSuggestion[]>([])
const selectedCategory = ref('general')

// AI Interfaces
interface AIPollSuggestion {
  id: string
  question: string
  options: string[]
  category: string
  purpose: string
}

// AI Categories
const aiCategories = [
  { id: 'general', label: 'General', icon: 'fas fa-th-large' },
  { id: 'feedback', label: 'Feedback', icon: 'fas fa-comment-dots' },
  { id: 'team', label: 'Team Building', icon: 'fas fa-users' },
  { id: 'product', label: 'Product', icon: 'fas fa-box' },
  { id: 'hr', label: 'HR & Culture', icon: 'fas fa-building' }
]

// Mock AI Suggestions
const mockAISuggestions: Record<string, AIPollSuggestion[]> = {
  general: [
    { id: '1', question: 'What day works best for team meetings?', options: ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday'], category: 'general', purpose: 'Optimize scheduling' },
    { id: '2', question: 'How satisfied are you with the current work environment?', options: ['Very satisfied', 'Satisfied', 'Neutral', 'Dissatisfied', 'Very dissatisfied'], category: 'general', purpose: 'Measure satisfaction' }
  ],
  feedback: [
    { id: '3', question: 'How would you rate the recent company update presentation?', options: ['Excellent', 'Good', 'Average', 'Below average', 'Poor'], category: 'feedback', purpose: 'Evaluate communications' },
    { id: '4', question: 'What aspect of our product needs the most improvement?', options: ['User interface', 'Performance', 'Features', 'Documentation', 'Support'], category: 'feedback', purpose: 'Prioritize improvements' }
  ],
  team: [
    { id: '5', question: 'What type of team building activity would you prefer?', options: ['Outdoor activities', 'Game nights', 'Workshops', 'Volunteer events', 'Casual dinners'], category: 'team', purpose: 'Plan team activities' },
    { id: '6', question: 'How often should we have team social events?', options: ['Weekly', 'Bi-weekly', 'Monthly', 'Quarterly', 'As needed'], category: 'team', purpose: 'Set social frequency' }
  ],
  product: [
    { id: '7', question: 'Which feature should we prioritize in the next release?', options: ['Dashboard redesign', 'Mobile app', 'Integration APIs', 'Reporting tools', 'Performance improvements'], category: 'product', purpose: 'Guide roadmap' },
    { id: '8', question: 'How do you primarily use our product?', options: ['Daily tasks', 'Project management', 'Collaboration', 'Reporting', 'Other'], category: 'product', purpose: 'Understand usage' }
  ],
  hr: [
    { id: '9', question: 'What benefit would you value most?', options: ['Health insurance', 'Remote work', 'Learning budget', 'Extra PTO', 'Wellness programs'], category: 'hr', purpose: 'Improve benefits' },
    { id: '10', question: 'How would you rate the onboarding experience?', options: ['Excellent', 'Good', 'Average', 'Needs improvement', 'Poor'], category: 'hr', purpose: 'Evaluate onboarding' }
  ]
}

// AI Functions
async function generatePollSuggestions() {
  isGeneratingSuggestions.value = true
  showAISuggestionsModal.value = true

  try {
    await new Promise(resolve => setTimeout(resolve, 1200))
    aiSuggestions.value = mockAISuggestions[selectedCategory.value] || mockAISuggestions.general
  } catch (error) {
    console.error('Failed to generate suggestions:', error)
  } finally {
    isGeneratingSuggestions.value = false
  }
}

function applySuggestion(suggestion: AIPollSuggestion) {
  question.value = suggestion.question
  options.value = [...suggestion.options]
  showAISuggestionsModal.value = false
  uiStore.showSuccess('AI Suggestion Applied', 'Poll question and options have been filled in')
}

function selectCategory(categoryId: string) {
  selectedCategory.value = categoryId
  generatePollSuggestions()
}

function addOption() {
  if (options.value.length < 6) {
    options.value.push('')
  }
}

function removeOption(index: number) {
  if (options.value.length > 2) {
    options.value.splice(index, 1)
  }
}

function createPoll() {
  if (!question.value.trim()) {
    uiStore.showError('Question is required')
    return
  }

  uiStore.showSuccess('Poll created', 'Your poll is now live')
  router.push({ name: 'Polls' })
}

function goBack() {
  router.push({ name: 'Polls' })
}
</script>

<template>
  <div class="max-w-2xl mx-auto space-y-6 fade-in">
    <!-- Header -->
    <div class="flex items-center justify-between">
      <div class="flex items-center gap-4">
        <button @click="goBack" class="btn-icon btn-ghost">
          <i class="fas fa-arrow-left"></i>
        </button>
        <div>
          <h1 class="text-2xl font-bold text-gray-900">Create Poll</h1>
          <p class="text-gray-500">Gather opinions from your team</p>
        </div>
      </div>
      <!-- AI Generate Button -->
      <button @click="generatePollSuggestions"
              class="px-4 py-2 bg-gradient-to-r from-teal-500 to-emerald-500 text-white rounded-xl font-medium text-sm flex items-center gap-2 hover:from-teal-600 hover:to-emerald-600 transition-all shadow-lg">
        <i class="fas fa-wand-magic-sparkles"></i>
        AI Suggest Poll
      </button>
    </div>

    <!-- Form -->
    <div class="card p-6 space-y-6">
      <!-- Question -->
      <div>
        <div class="flex items-center justify-between mb-2">
          <label class="block text-sm font-medium text-gray-700">Question</label>
          <button @click="generatePollSuggestions"
                  class="px-3 py-1 text-xs font-medium text-teal-600 bg-teal-50 rounded-lg hover:bg-teal-100 transition-colors flex items-center gap-1">
            <i class="fas fa-wand-magic-sparkles"></i>
            AI Generate
          </button>
        </div>
        <input
          v-model="question"
          type="text"
          placeholder="What would you like to ask?"
          class="input"
        >
      </div>

      <!-- Options -->
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-2">Options</label>
        <div class="space-y-3">
          <div v-for="(option, index) in options" :key="index" class="flex gap-2">
            <input
              v-model="options[index]"
              type="text"
              :placeholder="`Option ${index + 1}`"
              class="input flex-1"
            >
            <button
              v-if="options.length > 2"
              @click="removeOption(index)"
              class="btn-icon btn-ghost text-red-500"
            >
              <i class="fas fa-times"></i>
            </button>
          </div>
        </div>
        <button
          v-if="options.length < 6"
          @click="addOption"
          class="btn btn-ghost btn-sm mt-3"
        >
          <i class="fas fa-plus"></i>
          <span>Add Option</span>
        </button>
      </div>

      <!-- Settings -->
      <div class="space-y-4">
        <label class="flex items-center gap-3 cursor-pointer">
          <input v-model="allowMultiple" type="checkbox" class="w-4 h-4 rounded border-gray-300 text-teal-600">
          <span class="text-sm text-gray-700">Allow multiple selections</span>
        </label>
        <label class="flex items-center gap-3 cursor-pointer">
          <input v-model="isAnonymous" type="checkbox" class="w-4 h-4 rounded border-gray-300 text-teal-600">
          <span class="text-sm text-gray-700">Anonymous voting</span>
        </label>
      </div>

      <!-- End Date -->
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-2">End Date (optional)</label>
        <input v-model="endDate" type="date" class="input">
      </div>

      <!-- Actions -->
      <div class="flex items-center gap-3 pt-4 border-t border-gray-100">
        <button @click="goBack" class="btn btn-secondary">Cancel</button>
        <button @click="createPoll" class="btn btn-primary flex-1">
          <i class="fas fa-check"></i>
          <span>Create Poll</span>
        </button>
      </div>
    </div>

    <!-- AI Poll Suggestions Modal -->
    <Teleport to="body">
      <div v-if="showAISuggestionsModal" class="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
        <div class="bg-white rounded-2xl shadow-2xl w-full max-w-2xl max-h-[80vh] overflow-hidden">
          <div class="p-6 border-b border-gray-100">
            <div class="flex items-center justify-between">
              <div class="flex items-center gap-3">
                <div class="w-10 h-10 bg-gradient-to-br from-teal-500 to-emerald-500 rounded-xl flex items-center justify-center">
                  <i class="fas fa-wand-magic-sparkles text-white"></i>
                </div>
                <div>
                  <h3 class="text-lg font-semibold text-gray-900">AI Poll Suggestions</h3>
                  <p class="text-sm text-gray-500">Choose a poll template to get started</p>
                </div>
              </div>
              <button @click="showAISuggestionsModal = false" class="p-2 hover:bg-gray-100 rounded-lg transition-colors">
                <i class="fas fa-times text-gray-400"></i>
              </button>
            </div>
          </div>

          <!-- Category Tabs -->
          <div class="px-6 py-3 border-b border-gray-100 flex gap-2 overflow-x-auto">
            <button v-for="cat in aiCategories" :key="cat.id"
                    @click="selectCategory(cat.id)"
                    :class="['px-4 py-2 rounded-lg text-sm font-medium transition-all flex items-center gap-2 whitespace-nowrap',
                             selectedCategory === cat.id
                               ? 'bg-teal-100 text-teal-700'
                               : 'bg-gray-100 text-gray-600 hover:bg-gray-200']">
              <i :class="cat.icon"></i>
              {{ cat.label }}
            </button>
          </div>

          <div class="p-6 overflow-y-auto max-h-[50vh]">
            <AILoadingIndicator v-if="isGeneratingSuggestions" message="Generating poll suggestions..." />

            <div v-else-if="aiSuggestions.length > 0" class="space-y-4">
              <div v-for="suggestion in aiSuggestions" :key="suggestion.id"
                   class="border border-gray-200 rounded-xl p-4 hover:border-teal-300 hover:bg-teal-50/30 transition-all cursor-pointer"
                   @click="applySuggestion(suggestion)">
                <div class="flex items-start justify-between mb-3">
                  <h4 class="font-semibold text-gray-900 flex-1">{{ suggestion.question }}</h4>
                  <span class="px-2 py-1 bg-teal-100 text-teal-700 rounded-full text-xs font-medium">
                    {{ suggestion.purpose }}
                  </span>
                </div>
                <div class="flex flex-wrap gap-2">
                  <span v-for="(opt, idx) in suggestion.options" :key="idx"
                        class="px-3 py-1 bg-gray-100 text-gray-700 rounded-full text-sm">
                    {{ opt }}
                  </span>
                </div>
              </div>
            </div>

            <div v-else class="text-center py-8 text-gray-500">
              <i class="fas fa-poll text-4xl mb-3 text-gray-300"></i>
              <p>No suggestions available. Try a different category.</p>
            </div>
          </div>

          <div class="p-4 border-t border-gray-100 flex justify-end gap-3">
            <button @click="generatePollSuggestions"
                    class="px-4 py-2 text-teal-600 hover:bg-teal-50 rounded-lg transition-colors flex items-center gap-2">
              <i class="fas fa-rotate"></i> Refresh
            </button>
            <button @click="showAISuggestionsModal = false"
                    class="px-4 py-2 bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200 transition-colors">
              Cancel
            </button>
          </div>
        </div>
      </div>
    </Teleport>
  </div>
</template>
