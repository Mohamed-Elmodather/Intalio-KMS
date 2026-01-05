<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'

const router = useRouter()
const route = useRoute()

const isLoading = ref(true)
const document = ref<any>(null)

// Mock document data - in production, this would come from an API
const mockDocuments = [
  {
    id: 1,
    name: 'AFC Asian Cup 2027 Schedule.pdf',
    description: 'Complete match schedule for all 51 games of the AFC Asian Cup 2027, including group stage, knockout rounds, and the final.',
    size: '4.2 MB',
    sizeBytes: 4404019,
    type: 'PDF',
    mimeType: 'application/pdf',
    icon: 'fas fa-file-pdf',
    iconBg: 'bg-rose-100',
    iconColor: 'text-rose-600',
    gradientFrom: 'from-rose-500',
    gradientTo: 'to-rose-600',
    createdAt: '2024-01-15T10:30:00Z',
    updatedAt: '2024-01-20T14:45:00Z',
    author: { name: 'AFC Media Team', initials: 'AM', color: '#006847' },
    library: 'Tournament Documents',
    tags: ['schedule', 'matches', 'official'],
    downloads: 1245,
    views: 3890,
    version: '2.1',
    pages: 24
  },
  {
    id: 2,
    name: 'Stadium Operations Manual.docx',
    description: 'Comprehensive operations manual covering all stadium procedures, security protocols, and event management guidelines.',
    size: '12.5 MB',
    sizeBytes: 13107200,
    type: 'Word',
    mimeType: 'application/vnd.openxmlformats-officedocument.wordprocessingml.document',
    icon: 'fas fa-file-word',
    iconBg: 'bg-blue-100',
    iconColor: 'text-blue-600',
    gradientFrom: 'from-blue-500',
    gradientTo: 'to-blue-600',
    createdAt: '2024-01-10T09:00:00Z',
    updatedAt: '2024-01-18T16:30:00Z',
    author: { name: 'Operations Team', initials: 'OT', color: '#2563eb' },
    library: 'Operations',
    tags: ['stadium', 'operations', 'manual'],
    downloads: 567,
    views: 1234,
    version: '3.0',
    pages: 156
  },
  {
    id: 3,
    name: 'Team Statistics 2027.xlsx',
    description: 'Detailed statistics for all participating teams including player data, historical performance, and tournament predictions.',
    size: '3.8 MB',
    sizeBytes: 3984588,
    type: 'Excel',
    mimeType: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
    icon: 'fas fa-file-excel',
    iconBg: 'bg-emerald-100',
    iconColor: 'text-emerald-600',
    gradientFrom: 'from-emerald-500',
    gradientTo: 'to-emerald-600',
    createdAt: '2024-01-12T11:15:00Z',
    updatedAt: '2024-01-19T10:00:00Z',
    author: { name: 'Statistics Dept', initials: 'SD', color: '#059669' },
    library: 'Analytics',
    tags: ['statistics', 'teams', 'data'],
    downloads: 892,
    views: 2156,
    version: '1.5',
    pages: null
  },
  {
    id: 4,
    name: 'Opening Ceremony Plan.pptx',
    description: 'Detailed presentation outlining the opening ceremony sequence, performers, technical requirements, and timeline.',
    size: '28.4 MB',
    sizeBytes: 29780582,
    type: 'PowerPoint',
    mimeType: 'application/vnd.openxmlformats-officedocument.presentationml.presentation',
    icon: 'fas fa-file-powerpoint',
    iconBg: 'bg-orange-100',
    iconColor: 'text-orange-600',
    gradientFrom: 'from-orange-500',
    gradientTo: 'to-orange-600',
    createdAt: '2024-01-08T14:00:00Z',
    updatedAt: '2024-01-17T09:30:00Z',
    author: { name: 'Events Team', initials: 'ET', color: '#ea580c' },
    library: 'Events',
    tags: ['ceremony', 'opening', 'presentation'],
    downloads: 234,
    views: 678,
    version: '4.2',
    pages: 48
  }
]

const documentId = computed(() => Number(route.params.id))

onMounted(() => {
  // Simulate API call
  setTimeout(() => {
    document.value = mockDocuments.find(d => d.id === documentId.value) || mockDocuments[0]
    isLoading.value = false
  }, 500)
})

function goBack() {
  router.push('/documents')
}

function downloadDocument() {
  if (document.value) {
    alert(`Downloading: ${document.value.name}`)
  }
}

function shareDocument() {
  if (document.value) {
    navigator.clipboard.writeText(window.location.href)
    alert('Link copied to clipboard!')
  }
}

function printDocument() {
  window.print()
}

function formatDate(dateString: string): string {
  return new Date(dateString).toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

function formatShortDate(dateString: string): string {
  return new Date(dateString).toLocaleDateString('en-US', {
    month: 'short',
    day: 'numeric',
    year: 'numeric'
  })
}

const relatedDocuments = computed(() => {
  if (!document.value) return []
  return mockDocuments.filter(d => d.id !== document.value.id).slice(0, 3)
})
</script>

<template>
  <div class="min-h-screen bg-gray-50/50">
    <!-- Loading State -->
    <div v-if="isLoading" class="flex items-center justify-center min-h-[60vh]">
      <div class="text-center">
        <div class="w-12 h-12 border-4 border-teal-500 border-t-transparent rounded-full animate-spin mx-auto mb-4"></div>
        <p class="text-gray-500">Loading document...</p>
      </div>
    </div>

    <!-- Document Content -->
    <div v-else-if="document" class="max-w-6xl mx-auto px-6 py-8 space-y-6">
      <!-- Back Button -->
      <button @click="goBack" class="inline-flex items-center gap-2 text-gray-600 hover:text-teal-600 transition-colors group">
        <i class="fas fa-arrow-left group-hover:-translate-x-1 transition-transform"></i>
        <span>Back to Documents</span>
      </button>

      <!-- Main Content Grid -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <!-- Left Column - Document Preview -->
        <div class="lg:col-span-2 space-y-6">
          <!-- Document Header Card -->
          <div class="bg-white rounded-2xl shadow-sm border border-gray-100 overflow-hidden">
            <!-- Gradient Header -->
            <div :class="['h-32 bg-gradient-to-br', document.gradientFrom, document.gradientTo, 'relative']">
              <div class="absolute inset-0 bg-black/10"></div>
              <div class="absolute bottom-4 left-6 right-6 flex items-end justify-between">
                <div class="flex items-center gap-4">
                  <div class="w-16 h-16 bg-white rounded-xl shadow-lg flex items-center justify-center">
                    <i :class="[document.icon, document.iconColor, 'text-2xl']"></i>
                  </div>
                  <div class="text-white">
                    <span class="px-2 py-1 bg-white/20 backdrop-blur-sm rounded-full text-xs font-medium">
                      {{ document.type }}
                    </span>
                  </div>
                </div>
              </div>
            </div>

            <!-- Document Info -->
            <div class="p-6">
              <h1 class="text-2xl font-bold text-gray-900 mb-2">{{ document.name }}</h1>
              <p class="text-gray-600 mb-6">{{ document.description }}</p>

              <!-- Action Buttons -->
              <div class="flex flex-wrap gap-3">
                <button @click="downloadDocument" class="flex-1 sm:flex-none px-6 py-3 bg-teal-500 hover:bg-teal-600 text-white rounded-xl font-medium flex items-center justify-center gap-2 transition-colors shadow-lg shadow-teal-200">
                  <i class="fas fa-download"></i>
                  <span>Download</span>
                </button>
                <button @click="shareDocument" class="px-4 py-3 bg-gray-100 hover:bg-gray-200 text-gray-700 rounded-xl font-medium flex items-center justify-center gap-2 transition-colors">
                  <i class="fas fa-share-alt"></i>
                  <span>Share</span>
                </button>
                <button @click="printDocument" class="px-4 py-3 bg-gray-100 hover:bg-gray-200 text-gray-700 rounded-xl font-medium flex items-center justify-center gap-2 transition-colors">
                  <i class="fas fa-print"></i>
                  <span>Print</span>
                </button>
                <button class="px-4 py-3 bg-gray-100 hover:bg-gray-200 text-gray-700 rounded-xl font-medium flex items-center justify-center gap-2 transition-colors">
                  <i class="far fa-bookmark"></i>
                </button>
              </div>
            </div>
          </div>

          <!-- Document Preview -->
          <div class="bg-white rounded-2xl shadow-sm border border-gray-100 p-6">
            <h2 class="text-lg font-semibold text-gray-900 mb-4 flex items-center gap-2">
              <i class="fas fa-eye text-teal-500"></i>
              Preview
            </h2>
            <div class="aspect-[4/3] bg-gray-100 rounded-xl flex items-center justify-center border-2 border-dashed border-gray-200">
              <div class="text-center">
                <div :class="[document.iconBg, 'w-20 h-20 rounded-2xl flex items-center justify-center mx-auto mb-4']">
                  <i :class="[document.icon, document.iconColor, 'text-3xl']"></i>
                </div>
                <p class="text-gray-500 mb-4">Preview not available</p>
                <button @click="downloadDocument" class="px-4 py-2 bg-teal-500 hover:bg-teal-600 text-white rounded-lg text-sm font-medium transition-colors">
                  Download to View
                </button>
              </div>
            </div>
          </div>
        </div>

        <!-- Right Column - Metadata -->
        <div class="space-y-6">
          <!-- Document Details -->
          <div class="bg-white rounded-2xl shadow-sm border border-gray-100 p-6">
            <h2 class="text-lg font-semibold text-gray-900 mb-4 flex items-center gap-2">
              <i class="fas fa-info-circle text-teal-500"></i>
              Details
            </h2>
            <div class="space-y-4">
              <div class="flex items-center justify-between py-2 border-b border-gray-100">
                <span class="text-gray-500 text-sm">Size</span>
                <span class="font-medium text-gray-900">{{ document.size }}</span>
              </div>
              <div class="flex items-center justify-between py-2 border-b border-gray-100">
                <span class="text-gray-500 text-sm">Type</span>
                <span class="font-medium text-gray-900">{{ document.type }}</span>
              </div>
              <div v-if="document.pages" class="flex items-center justify-between py-2 border-b border-gray-100">
                <span class="text-gray-500 text-sm">Pages</span>
                <span class="font-medium text-gray-900">{{ document.pages }}</span>
              </div>
              <div class="flex items-center justify-between py-2 border-b border-gray-100">
                <span class="text-gray-500 text-sm">Version</span>
                <span class="font-medium text-gray-900">v{{ document.version }}</span>
              </div>
              <div class="flex items-center justify-between py-2 border-b border-gray-100">
                <span class="text-gray-500 text-sm">Library</span>
                <span class="font-medium text-teal-600">{{ document.library }}</span>
              </div>
              <div class="flex items-center justify-between py-2 border-b border-gray-100">
                <span class="text-gray-500 text-sm">Downloads</span>
                <span class="font-medium text-gray-900">{{ document.downloads.toLocaleString() }}</span>
              </div>
              <div class="flex items-center justify-between py-2">
                <span class="text-gray-500 text-sm">Views</span>
                <span class="font-medium text-gray-900">{{ document.views.toLocaleString() }}</span>
              </div>
            </div>
          </div>

          <!-- Author -->
          <div class="bg-white rounded-2xl shadow-sm border border-gray-100 p-6">
            <h2 class="text-lg font-semibold text-gray-900 mb-4 flex items-center gap-2">
              <i class="fas fa-user text-teal-500"></i>
              Author
            </h2>
            <div class="flex items-center gap-3">
              <div class="w-12 h-12 rounded-xl flex items-center justify-center text-white font-semibold"
                   :style="{ backgroundColor: document.author.color }">
                {{ document.author.initials }}
              </div>
              <div>
                <p class="font-medium text-gray-900">{{ document.author.name }}</p>
                <p class="text-sm text-gray-500">Uploaded {{ formatShortDate(document.createdAt) }}</p>
              </div>
            </div>
          </div>

          <!-- Tags -->
          <div class="bg-white rounded-2xl shadow-sm border border-gray-100 p-6">
            <h2 class="text-lg font-semibold text-gray-900 mb-4 flex items-center gap-2">
              <i class="fas fa-tags text-teal-500"></i>
              Tags
            </h2>
            <div class="flex flex-wrap gap-2">
              <span v-for="tag in document.tags" :key="tag"
                    class="px-3 py-1.5 bg-teal-50 text-teal-700 rounded-full text-sm font-medium hover:bg-teal-100 cursor-pointer transition-colors">
                #{{ tag }}
              </span>
            </div>
          </div>

          <!-- Dates -->
          <div class="bg-white rounded-2xl shadow-sm border border-gray-100 p-6">
            <h2 class="text-lg font-semibold text-gray-900 mb-4 flex items-center gap-2">
              <i class="fas fa-clock text-teal-500"></i>
              Activity
            </h2>
            <div class="space-y-3">
              <div class="flex items-start gap-3">
                <div class="w-8 h-8 rounded-lg bg-green-100 flex items-center justify-center flex-shrink-0">
                  <i class="fas fa-plus text-green-600 text-xs"></i>
                </div>
                <div>
                  <p class="text-sm font-medium text-gray-900">Created</p>
                  <p class="text-xs text-gray-500">{{ formatDate(document.createdAt) }}</p>
                </div>
              </div>
              <div class="flex items-start gap-3">
                <div class="w-8 h-8 rounded-lg bg-blue-100 flex items-center justify-center flex-shrink-0">
                  <i class="fas fa-edit text-blue-600 text-xs"></i>
                </div>
                <div>
                  <p class="text-sm font-medium text-gray-900">Last Updated</p>
                  <p class="text-xs text-gray-500">{{ formatDate(document.updatedAt) }}</p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Related Documents -->
      <div class="bg-white rounded-2xl shadow-sm border border-gray-100 p-6">
        <h2 class="text-lg font-semibold text-gray-900 mb-4 flex items-center gap-2">
          <i class="fas fa-folder-open text-teal-500"></i>
          Related Documents
        </h2>
        <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div v-for="doc in relatedDocuments" :key="doc.id"
               @click="router.push(`/documents/${doc.id}`)"
               class="flex items-center gap-4 p-4 rounded-xl border border-gray-100 hover:border-teal-200 hover:shadow-md cursor-pointer transition-all group">
            <div :class="[doc.iconBg, 'w-12 h-12 rounded-xl flex items-center justify-center flex-shrink-0 transition-transform group-hover:scale-110']">
              <i :class="[doc.icon, doc.iconColor, 'text-xl']"></i>
            </div>
            <div class="flex-1 min-w-0">
              <h4 class="font-medium text-gray-900 truncate group-hover:text-teal-600 transition-colors">{{ doc.name }}</h4>
              <p class="text-sm text-gray-500">{{ doc.size }} • {{ doc.type }}</p>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Not Found -->
    <div v-else class="flex items-center justify-center min-h-[60vh]">
      <div class="text-center">
        <i class="fas fa-file-circle-xmark text-6xl text-gray-300 mb-4"></i>
        <h2 class="text-xl font-semibold text-gray-700 mb-2">Document Not Found</h2>
        <p class="text-gray-500 mb-6">The document you're looking for doesn't exist or has been removed.</p>
        <button @click="goBack" class="px-6 py-3 bg-teal-500 hover:bg-teal-600 text-white rounded-xl font-medium transition-colors">
          Back to Documents
        </button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.fade-in {
  animation: fadeIn 0.3s ease-out;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}
</style>
