<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useUIStore } from '@/stores/ui'

const router = useRouter()
const uiStore = useUIStore()

const question = ref('')
const options = ref(['', ''])
const allowMultiple = ref(false)
const isAnonymous = ref(true)
const endDate = ref('')

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
    <div class="flex items-center gap-4">
      <button @click="goBack" class="btn-icon btn-ghost">
        <i class="fas fa-arrow-left"></i>
      </button>
      <div>
        <h1 class="text-2xl font-bold text-gray-900">Create Poll</h1>
        <p class="text-gray-500">Gather opinions from your team</p>
      </div>
    </div>

    <!-- Form -->
    <div class="card p-6 space-y-6">
      <!-- Question -->
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-2">Question</label>
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
  </div>
</template>
