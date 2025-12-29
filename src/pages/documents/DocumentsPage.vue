<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { documentsApi } from '@/api'
import type { Document, DocumentLibrary } from '@/types'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'

const isLoading = ref(true)
const documents = ref<Document[]>([])
const libraries = ref<DocumentLibrary[]>([])
const selectedLibrary = ref<string | null>(null)
const searchQuery = ref('')
const viewMode = ref<'grid' | 'list'>('grid')

const filteredDocuments = computed(() => {
  let result = documents.value

  if (selectedLibrary.value) {
    result = result.filter(d => d.libraryId === selectedLibrary.value)
  }

  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    result = result.filter(d => d.name.toLowerCase().includes(query))
  }

  return result
})

onMounted(async () => {
  try {
    const [docsRes, libsRes] = await Promise.all([
      documentsApi.getDocuments({ pageSize: 50 }),
      documentsApi.getLibraries(),
    ])
    documents.value = docsRes.items
    libraries.value = libsRes
  } catch (error) {
    console.error('Failed to load documents:', error)
  } finally {
    isLoading.value = false
  }
})

function formatFileSize(bytes: number): string {
  if (bytes < 1024) return bytes + ' B'
  if (bytes < 1024 * 1024) return (bytes / 1024).toFixed(1) + ' KB'
  return (bytes / (1024 * 1024)).toFixed(1) + ' MB'
}

function formatDate(dateString: string): string {
  return new Date(dateString).toLocaleDateString('en-US', {
    month: 'short',
    day: 'numeric',
    year: 'numeric',
  })
}

function getFileIcon(type: string): string {
  const icons: Record<string, string> = {
    pdf: 'fas fa-file-pdf text-red-500',
    word: 'fas fa-file-word text-blue-500',
    excel: 'fas fa-file-excel text-green-500',
    powerpoint: 'fas fa-file-powerpoint text-orange-500',
    image: 'fas fa-file-image text-purple-500',
    video: 'fas fa-file-video text-pink-500',
    audio: 'fas fa-file-audio text-indigo-500',
    archive: 'fas fa-file-archive text-yellow-600',
  }
  return icons[type] || 'fas fa-file text-gray-500'
}

function getLibraryIcon(icon?: string): string {
  return icon || 'fas fa-folder'
}
</script>

<template>
  <div class="space-y-6 fade-in">
    <!-- Header -->
    <div class="flex items-center justify-between">
      <div>
        <h1 class="text-2xl font-bold text-gray-900">Document Library</h1>
        <p class="text-gray-500 mt-1">Access and manage your documents</p>
      </div>
      <button class="btn btn-primary">
        <i class="fas fa-upload"></i>
        <span>Upload Document</span>
      </button>
    </div>

    <!-- Libraries -->
    <div class="grid grid-cols-2 md:grid-cols-4 gap-4">
      <button
        v-for="lib in libraries"
        :key="lib.id"
        @click="selectedLibrary = selectedLibrary === lib.id ? null : lib.id"
        :class="[
          'card p-4 text-left transition-all',
          selectedLibrary === lib.id ? 'ring-2 ring-teal-500 bg-teal-50' : 'hover:shadow-md'
        ]"
      >
        <div class="flex items-center gap-3">
          <div
            class="w-10 h-10 rounded-xl flex items-center justify-center text-white"
            :style="{ backgroundColor: lib.color || '#14b8a6' }"
          >
            <i :class="getLibraryIcon(lib.icon)"></i>
          </div>
          <div class="min-w-0">
            <p class="font-medium text-gray-900 truncate">{{ lib.name }}</p>
            <p class="text-xs text-gray-500">{{ lib.documentCount }} files</p>
          </div>
        </div>
      </button>
    </div>

    <!-- Filters -->
    <div class="card p-4">
      <div class="flex flex-col md:flex-row gap-4">
        <div class="flex-1 relative">
          <i class="fas fa-search absolute left-4 top-1/2 -translate-y-1/2 text-gray-400"></i>
          <input
            v-model="searchQuery"
            type="text"
            placeholder="Search documents..."
            class="input pl-10"
          >
        </div>
        <div class="flex gap-1 bg-gray-100 p-1 rounded-lg">
          <button
            @click="viewMode = 'grid'"
            :class="['btn-icon btn-sm', viewMode === 'grid' ? 'bg-white shadow-sm' : '']"
          >
            <i class="fas fa-th-large"></i>
          </button>
          <button
            @click="viewMode = 'list'"
            :class="['btn-icon btn-sm', viewMode === 'list' ? 'bg-white shadow-sm' : '']"
          >
            <i class="fas fa-list"></i>
          </button>
        </div>
      </div>
    </div>

    <!-- Loading -->
    <div v-if="isLoading" class="card p-12">
      <LoadingSpinner size="lg" text="Loading documents..." />
    </div>

    <!-- Empty State -->
    <div v-else-if="filteredDocuments.length === 0" class="card p-12 text-center">
      <i class="fas fa-folder-open text-4xl text-gray-300 mb-4"></i>
      <h3 class="text-lg font-semibold text-gray-700">No documents found</h3>
      <p class="text-gray-500 mt-1">Upload your first document to get started</p>
    </div>

    <!-- Documents Grid -->
    <div v-else :class="viewMode === 'grid' ? 'grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4' : 'space-y-2'">
      <div
        v-for="doc in filteredDocuments"
        :key="doc.id"
        :class="[
          'card-interactive p-4 cursor-pointer',
          viewMode === 'list' ? 'flex items-center gap-4' : 'text-center'
        ]"
      >
        <!-- Icon -->
        <div :class="viewMode === 'grid' ? 'w-16 h-16 mx-auto mb-3 rounded-xl bg-gray-100 flex items-center justify-center' : 'w-12 h-12 rounded-lg bg-gray-100 flex items-center justify-center flex-shrink-0'">
          <i :class="[getFileIcon(doc.type), 'text-2xl']"></i>
        </div>

        <!-- Details -->
        <div :class="viewMode === 'list' ? 'flex-1 min-w-0' : ''">
          <p class="font-medium text-gray-900 truncate">{{ doc.name }}</p>
          <div class="flex items-center gap-2 text-xs text-gray-400 mt-1" :class="viewMode === 'grid' ? 'justify-center' : ''">
            <span>{{ formatFileSize(doc.size) }}</span>
            <span>•</span>
            <span>{{ formatDate(doc.updatedAt) }}</span>
          </div>
        </div>

        <!-- Actions (list view) -->
        <div v-if="viewMode === 'list'" class="flex items-center gap-1">
          <button class="btn-icon btn-ghost btn-sm">
            <i class="fas fa-download"></i>
          </button>
          <button class="btn-icon btn-ghost btn-sm">
            <i class="fas fa-share-alt"></i>
          </button>
          <button class="btn-icon btn-ghost btn-sm">
            <i class="fas fa-ellipsis-v"></i>
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
