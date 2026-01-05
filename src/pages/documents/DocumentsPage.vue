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

const documentStats = computed(() => ({
  totalDocuments: documents.value.length,
  totalLibraries: libraries.value.length,
  totalSize: documents.value.reduce((acc, doc) => acc + (doc.size || 0), 0),
  recentUploads: documents.value.filter(d => {
    const uploadDate = new Date(d.createdAt)
    const weekAgo = new Date()
    weekAgo.setDate(weekAgo.getDate() - 7)
    return uploadDate > weekAgo
  }).length
}))

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
  if (bytes < 1024 * 1024 * 1024) return (bytes / (1024 * 1024)).toFixed(1) + ' MB'
  return (bytes / (1024 * 1024 * 1024)).toFixed(1) + ' GB'
}

function formatTotalSize(bytes: number): string {
  if (bytes < 1024 * 1024) return (bytes / 1024).toFixed(0) + ' KB'
  if (bytes < 1024 * 1024 * 1024) return (bytes / (1024 * 1024)).toFixed(1) + ' MB'
  return (bytes / (1024 * 1024 * 1024)).toFixed(1) + ' GB'
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
  <div class="page-view">
    <!-- Hero Section -->
    <div class="hero-section fade-in-up">
      <div class="hero-content">
        <div class="hero-left">
          <div class="hero-header">
            <div class="hero-icon">
              <i class="fas fa-folder-open"></i>
            </div>
            <div>
              <h1 class="hero-title"><span class="hero-title-highlight">Document</span> Library</h1>
              <p class="hero-subtitle">Access and manage your documents</p>
            </div>
          </div>
          <button class="hero-btn">
            <i class="fas fa-upload"></i>
            <span>Upload Document</span>
          </button>
        </div>

        <!-- Stats -->
        <div class="hero-stats">
          <div class="stat-card-hero">
            <div class="stat-card-hero-icon teal">
              <i class="fas fa-file-alt"></i>
            </div>
            <div class="stat-card-hero-content">
              <p class="stat-card-hero-value">{{ documentStats.totalDocuments }}</p>
              <p class="stat-card-hero-label">Documents</p>
            </div>
          </div>
          <div class="stat-card-hero">
            <div class="stat-card-hero-icon blue">
              <i class="fas fa-folder"></i>
            </div>
            <div class="stat-card-hero-content">
              <p class="stat-card-hero-value">{{ documentStats.totalLibraries }}</p>
              <p class="stat-card-hero-label">Libraries</p>
            </div>
          </div>
          <div class="stat-card-hero">
            <div class="stat-card-hero-icon orange">
              <i class="fas fa-hdd"></i>
            </div>
            <div class="stat-card-hero-content">
              <p class="stat-card-hero-value">{{ formatTotalSize(documentStats.totalSize) }}</p>
              <p class="stat-card-hero-label">Total Size</p>
            </div>
          </div>
          <div class="stat-card-hero">
            <div class="stat-card-hero-icon purple">
              <i class="fas fa-clock"></i>
            </div>
            <div class="stat-card-hero-content">
              <p class="stat-card-hero-value">{{ documentStats.recentUploads }}</p>
              <p class="stat-card-hero-label">This Week</p>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="px-6 space-y-6">
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
  </div>
</template>
