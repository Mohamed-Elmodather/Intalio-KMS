<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useUIStore } from '@/stores/ui'

const router = useRouter()
const uiStore = useUIStore()

// Form state
const title = ref('')
const excerpt = ref('')
const content = ref('')
const category = ref('')
const tags = ref<string[]>([])
const coverImage = ref('')
const isDraft = ref(true)
const isSubmitting = ref(false)

// Categories (mock)
const categories = [
  { id: '1', name: 'Company News' },
  { id: '2', name: 'HR & Policies' },
  { id: '3', name: 'Technology' },
  { id: '4', name: 'Training' },
]

async function saveArticle(publish = false) {
  if (!title.value.trim()) {
    uiStore.showError('Title is required')
    return
  }

  isSubmitting.value = true

  // Simulate API call
  await new Promise(resolve => setTimeout(resolve, 1000))

  if (publish) {
    uiStore.showSuccess('Article published', 'Your article is now live')
  } else {
    uiStore.showSuccess('Draft saved', 'Your changes have been saved')
  }

  isSubmitting.value = false
  router.push({ name: 'Articles' })
}

function goBack() {
  router.push({ name: 'Articles' })
}
</script>

<template>
  <div class="max-w-4xl mx-auto space-y-6 fade-in">
    <!-- Header -->
    <div class="flex items-center justify-between">
      <div class="flex items-center gap-4">
        <button @click="goBack" class="btn-icon btn-ghost">
          <i class="fas fa-arrow-left"></i>
        </button>
        <div>
          <h1 class="text-2xl font-bold text-gray-900">New Article</h1>
          <p class="text-gray-500">Create a new knowledge article</p>
        </div>
      </div>
      <div class="flex items-center gap-3">
        <span v-if="isDraft" class="badge badge-secondary">Draft</span>
        <button @click="saveArticle(false)" :disabled="isSubmitting" class="btn btn-secondary">
          <i class="fas fa-save"></i>
          <span>Save Draft</span>
        </button>
        <button @click="saveArticle(true)" :disabled="isSubmitting" class="btn btn-primary">
          <i class="fas fa-paper-plane"></i>
          <span>Publish</span>
        </button>
      </div>
    </div>

    <!-- Editor -->
    <div class="card p-6 space-y-6">
      <!-- Title -->
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-2">Title</label>
        <input
          v-model="title"
          type="text"
          placeholder="Enter article title..."
          class="w-full text-2xl font-bold border-0 border-b border-gray-200 pb-2 focus:border-teal-500 focus:ring-0 outline-none"
        >
      </div>

      <!-- Excerpt -->
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-2">Excerpt</label>
        <textarea
          v-model="excerpt"
          placeholder="Write a brief summary..."
          rows="2"
          class="input resize-none"
        ></textarea>
      </div>

      <!-- Category & Tags -->
      <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2">Category</label>
          <select v-model="category" class="input">
            <option value="">Select category...</option>
            <option v-for="cat in categories" :key="cat.id" :value="cat.id">
              {{ cat.name }}
            </option>
          </select>
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2">Cover Image URL</label>
          <input
            v-model="coverImage"
            type="url"
            placeholder="https://example.com/image.jpg"
            class="input"
          >
        </div>
      </div>

      <!-- Content Editor -->
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-2">Content</label>
        <div class="border border-gray-200 rounded-xl overflow-hidden">
          <!-- Toolbar -->
          <div class="flex items-center gap-1 p-2 bg-gray-50 border-b border-gray-200">
            <button class="btn-icon btn-ghost btn-sm"><i class="fas fa-bold"></i></button>
            <button class="btn-icon btn-ghost btn-sm"><i class="fas fa-italic"></i></button>
            <button class="btn-icon btn-ghost btn-sm"><i class="fas fa-underline"></i></button>
            <div class="w-px h-6 bg-gray-200 mx-1"></div>
            <button class="btn-icon btn-ghost btn-sm"><i class="fas fa-heading"></i></button>
            <button class="btn-icon btn-ghost btn-sm"><i class="fas fa-list-ul"></i></button>
            <button class="btn-icon btn-ghost btn-sm"><i class="fas fa-list-ol"></i></button>
            <div class="w-px h-6 bg-gray-200 mx-1"></div>
            <button class="btn-icon btn-ghost btn-sm"><i class="fas fa-link"></i></button>
            <button class="btn-icon btn-ghost btn-sm"><i class="fas fa-image"></i></button>
            <button class="btn-icon btn-ghost btn-sm"><i class="fas fa-code"></i></button>
          </div>
          <!-- Editor Area -->
          <textarea
            v-model="content"
            placeholder="Start writing your article..."
            rows="15"
            class="w-full p-4 text-gray-800 outline-none resize-none"
          ></textarea>
        </div>
      </div>
    </div>
  </div>
</template>
