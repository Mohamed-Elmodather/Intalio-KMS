<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useComments } from '@/composables/useComments'
import { useRatings } from '@/composables/useRatings'
import { CommentsSection, RatingStars, BookmarkButton } from '@/components/common'
import type { Author, ServiceRequestStep } from '@/types/detail-pages'

const route = useRoute()
const router = useRouter()

// Service Request interface
interface ServiceRequest {
  id: string
  title: string
  description: string
  category: string
  categoryIcon: string
  priority: 'low' | 'medium' | 'high' | 'urgent'
  status: 'submitted' | 'in_review' | 'processing' | 'pending_info' | 'completed' | 'cancelled'
  requestedBy: Author
  assignedTo: Author | null
  createdAt: Date
  updatedAt: Date
  expectedCompletion: Date | null
  completedAt: Date | null
  steps: ServiceRequestStep[]
  attachments: Attachment[]
  timeline: TimelineEvent[]
  rating: number | null
  serviceType: string
  referenceNumber: string
  department: string
}

interface Attachment {
  id: string
  name: string
  type: string
  size: string
  url: string
  uploadedAt: Date
}

interface TimelineEvent {
  id: string
  type: 'status_change' | 'comment' | 'attachment' | 'assignment' | 'info_request'
  title: string
  description: string
  author: Author | null
  createdAt: Date
  metadata?: Record<string, string>
}

// State
const request = ref<ServiceRequest | null>(null)
const isLoading = ref(true)
const showCancelModal = ref(false)
const showRatingModal = ref(false)
const newMessage = ref('')
const isSendingMessage = ref(false)

// Comments
const {
  comments,
  isLoading: commentsLoading,
  loadComments,
  addComment
} = useComments('service-request', route.params.id as string)

// Ratings
const { rating, submitRating } = useRatings('service-request', route.params.id as string)

// Computed
const requestId = computed(() => route.params.id as string)

const statusConfig = computed(() => {
  if (!request.value) return { label: '', class: '', icon: '', bgClass: '' }
  const configs: Record<string, { label: string; class: string; icon: string; bgClass: string }> = {
    submitted: { label: 'Submitted', class: 'text-blue-600', icon: 'fas fa-paper-plane', bgClass: 'bg-blue-100' },
    in_review: { label: 'In Review', class: 'text-yellow-600', icon: 'fas fa-search', bgClass: 'bg-yellow-100' },
    processing: { label: 'Processing', class: 'text-purple-600', icon: 'fas fa-cog fa-spin', bgClass: 'bg-purple-100' },
    pending_info: { label: 'Pending Information', class: 'text-orange-600', icon: 'fas fa-question-circle', bgClass: 'bg-orange-100' },
    completed: { label: 'Completed', class: 'text-green-600', icon: 'fas fa-check-circle', bgClass: 'bg-green-100' },
    cancelled: { label: 'Cancelled', class: 'text-gray-600', icon: 'fas fa-times-circle', bgClass: 'bg-gray-100' }
  }
  return configs[request.value.status] || configs.submitted
})

const priorityConfig = computed(() => {
  if (!request.value) return { label: '', class: '', icon: '' }
  const configs: Record<string, { label: string; class: string; icon: string }> = {
    low: { label: 'Low', class: 'bg-gray-100 text-gray-700', icon: 'fas fa-arrow-down' },
    medium: { label: 'Medium', class: 'bg-blue-100 text-blue-700', icon: 'fas fa-minus' },
    high: { label: 'High', class: 'bg-orange-100 text-orange-700', icon: 'fas fa-arrow-up' },
    urgent: { label: 'Urgent', class: 'bg-red-100 text-red-700', icon: 'fas fa-exclamation' }
  }
  return configs[request.value.priority] || configs.medium
})

const currentStepIndex = computed(() => {
  if (!request.value) return 0
  return request.value.steps.findIndex(s => s.status === 'current')
})

const progressPercentage = computed(() => {
  if (!request.value) return 0
  const completed = request.value.steps.filter(s => s.status === 'completed').length
  return Math.round((completed / request.value.steps.length) * 100)
})

const canCancel = computed(() => {
  if (!request.value) return false
  return ['submitted', 'in_review'].includes(request.value.status)
})

const canRate = computed(() => {
  if (!request.value) return false
  return request.value.status === 'completed' && !request.value.rating
})

// Mock data loader
async function loadRequest() {
  isLoading.value = true
  try {
    await new Promise(resolve => setTimeout(resolve, 800))

    request.value = {
      id: requestId.value,
      title: 'IT Equipment Request - New Laptop',
      description: 'Request for a new MacBook Pro 16" for software development work. Current machine is 4 years old and experiencing performance issues with modern development tools and Docker containers.',
      category: 'IT Equipment',
      categoryIcon: 'fas fa-laptop',
      priority: 'high',
      status: 'processing',
      requestedBy: {
        id: 'u1',
        name: 'Ahmed Hassan',
        initials: 'AH',
        role: 'Senior Developer',
        department: 'Engineering',
        avatar: '',
        isFollowing: false
      },
      assignedTo: {
        id: 'u2',
        name: 'Sarah IT Support',
        initials: 'SI',
        role: 'IT Support Specialist',
        department: 'IT Department',
        avatar: '',
        isFollowing: false
      },
      createdAt: new Date(Date.now() - 5 * 24 * 60 * 60 * 1000),
      updatedAt: new Date(Date.now() - 1 * 24 * 60 * 60 * 1000),
      expectedCompletion: new Date(Date.now() + 7 * 24 * 60 * 60 * 1000),
      completedAt: null,
      steps: [
        { id: '1', title: 'Submitted', description: 'Request submitted successfully', status: 'completed', completedAt: new Date(Date.now() - 5 * 24 * 60 * 60 * 1000) },
        { id: '2', title: 'In Review', description: 'Request reviewed by IT manager', status: 'completed', completedAt: new Date(Date.now() - 4 * 24 * 60 * 60 * 1000) },
        { id: '3', title: 'Approved', description: 'Budget approved by finance', status: 'completed', completedAt: new Date(Date.now() - 3 * 24 * 60 * 60 * 1000) },
        { id: '4', title: 'Processing', description: 'Equipment being procured', status: 'current' },
        { id: '5', title: 'Ready', description: 'Equipment ready for pickup', status: 'pending' },
        { id: '6', title: 'Completed', description: 'Equipment delivered', status: 'pending' }
      ],
      attachments: [
        { id: '1', name: 'Current_Equipment_Photo.jpg', type: 'image', size: '2.4 MB', url: '#', uploadedAt: new Date(Date.now() - 5 * 24 * 60 * 60 * 1000) },
        { id: '2', name: 'Performance_Issues_Report.pdf', type: 'pdf', size: '156 KB', url: '#', uploadedAt: new Date(Date.now() - 5 * 24 * 60 * 60 * 1000) },
        { id: '3', name: 'Manager_Approval_Email.msg', type: 'email', size: '45 KB', url: '#', uploadedAt: new Date(Date.now() - 4 * 24 * 60 * 60 * 1000) }
      ],
      timeline: [
        {
          id: '1',
          type: 'status_change',
          title: 'Request Submitted',
          description: 'New IT equipment request has been submitted.',
          author: null,
          createdAt: new Date(Date.now() - 5 * 24 * 60 * 60 * 1000)
        },
        {
          id: '2',
          type: 'assignment',
          title: 'Assigned to Sarah IT Support',
          description: 'Request has been assigned for processing.',
          author: { id: 'sys', name: 'System', initials: 'SY', role: 'Automated', isFollowing: false },
          createdAt: new Date(Date.now() - 5 * 24 * 60 * 60 * 1000)
        },
        {
          id: '3',
          type: 'comment',
          title: 'Review Started',
          description: 'I will review this request and get back to you shortly with options.',
          author: { id: 'u2', name: 'Sarah IT Support', initials: 'SI', role: 'IT Support', isFollowing: false },
          createdAt: new Date(Date.now() - 4 * 24 * 60 * 60 * 1000)
        },
        {
          id: '4',
          type: 'status_change',
          title: 'Status Changed to In Review',
          description: 'Request is now being reviewed.',
          author: null,
          createdAt: new Date(Date.now() - 4 * 24 * 60 * 60 * 1000)
        },
        {
          id: '5',
          type: 'comment',
          title: 'Approval Request Sent',
          description: 'Budget approval request sent to Finance department. MacBook Pro 16" M3 Pro configuration recommended.',
          author: { id: 'u2', name: 'Sarah IT Support', initials: 'SI', role: 'IT Support', isFollowing: false },
          createdAt: new Date(Date.now() - 3 * 24 * 60 * 60 * 1000)
        },
        {
          id: '6',
          type: 'status_change',
          title: 'Budget Approved',
          description: 'Finance has approved the equipment budget.',
          author: null,
          createdAt: new Date(Date.now() - 3 * 24 * 60 * 60 * 1000)
        },
        {
          id: '7',
          type: 'status_change',
          title: 'Processing Started',
          description: 'Equipment order has been placed with vendor.',
          author: null,
          createdAt: new Date(Date.now() - 1 * 24 * 60 * 60 * 1000)
        },
        {
          id: '8',
          type: 'comment',
          title: 'Order Update',
          description: 'Your MacBook Pro has been ordered. Expected delivery in 5-7 business days. I will notify you when it arrives.',
          author: { id: 'u2', name: 'Sarah IT Support', initials: 'SI', role: 'IT Support', isFollowing: false },
          createdAt: new Date(Date.now() - 1 * 24 * 60 * 60 * 1000)
        }
      ],
      rating: null,
      serviceType: 'Equipment Request',
      referenceNumber: 'SR-2025-00847',
      department: 'IT Department'
    }

    await loadComments()
  } finally {
    isLoading.value = false
  }
}

async function sendMessage() {
  if (!newMessage.value.trim() || isSendingMessage.value) return

  isSendingMessage.value = true
  try {
    await new Promise(resolve => setTimeout(resolve, 500))

    // Add to timeline
    if (request.value) {
      request.value.timeline.push({
        id: `msg-${Date.now()}`,
        type: 'comment',
        title: 'Message Sent',
        description: newMessage.value,
        author: request.value.requestedBy,
        createdAt: new Date()
      })
    }

    newMessage.value = ''
  } finally {
    isSendingMessage.value = false
  }
}

async function cancelRequest() {
  if (!request.value) return

  await new Promise(resolve => setTimeout(resolve, 500))
  request.value.status = 'cancelled'
  showCancelModal.value = false
}

async function handleRating(stars: number) {
  if (!request.value) return

  await submitRating(stars)
  request.value.rating = stars
  showRatingModal.value = false
}

function formatDate(date: Date): string {
  return new Date(date).toLocaleDateString('en-US', {
    month: 'short',
    day: 'numeric',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

function formatRelativeTime(date: Date): string {
  const now = new Date()
  const diff = now.getTime() - new Date(date).getTime()
  const minutes = Math.floor(diff / (1000 * 60))
  const hours = Math.floor(diff / (1000 * 60 * 60))
  const days = Math.floor(diff / (1000 * 60 * 60 * 24))

  if (minutes < 60) return `${minutes}m ago`
  if (hours < 24) return `${hours}h ago`
  return `${days}d ago`
}

function getFileIcon(type: string): string {
  const icons: Record<string, string> = {
    image: 'fas fa-image text-green-500',
    pdf: 'fas fa-file-pdf text-red-500',
    email: 'fas fa-envelope text-blue-500',
    document: 'fas fa-file-word text-blue-600',
    spreadsheet: 'fas fa-file-excel text-green-600'
  }
  return icons[type] || 'fas fa-file text-gray-500'
}

function getTimelineIcon(type: string): string {
  const icons: Record<string, string> = {
    status_change: 'fas fa-exchange-alt',
    comment: 'fas fa-comment',
    attachment: 'fas fa-paperclip',
    assignment: 'fas fa-user-check',
    info_request: 'fas fa-question-circle'
  }
  return icons[type] || 'fas fa-circle'
}

function goBack() {
  router.back()
}

// Initialize
onMounted(() => {
  loadRequest()
})

watch(() => route.params.id, () => {
  if (route.params.id) {
    loadRequest()
  }
})
</script>

<template>
  <div class="service-request-page min-h-screen bg-gray-50">
    <!-- Loading State -->
    <div v-if="isLoading" class="flex items-center justify-center min-h-screen">
      <div class="text-center">
        <i class="fas fa-spinner fa-spin text-4xl text-teal-500 mb-4"></i>
        <p class="text-gray-600">Loading request details...</p>
      </div>
    </div>

    <!-- Content -->
    <template v-else-if="request">
      <!-- Header -->
      <header class="bg-white border-b border-gray-200 sticky top-0 z-30">
        <div class="max-w-6xl mx-auto px-4 py-4">
          <div class="flex items-center justify-between">
            <div class="flex items-center gap-4">
              <button
                @click="goBack"
                class="p-2 hover:bg-gray-100 rounded-lg transition-colors"
              >
                <i class="fas fa-arrow-left text-gray-600"></i>
              </button>
              <div>
                <div class="flex items-center gap-2 text-sm text-gray-500 mb-1">
                  <router-link to="/self-services" class="hover:text-teal-600">Self Services</router-link>
                  <i class="fas fa-chevron-right text-xs"></i>
                  <span>{{ request.referenceNumber }}</span>
                </div>
                <h1 class="text-xl font-semibold text-gray-900">Service Request</h1>
              </div>
            </div>

            <div class="flex items-center gap-3">
              <BookmarkButton
                :content-id="request.id"
                content-type="service"
                size="sm"
              />
              <button
                v-if="canCancel"
                @click="showCancelModal = true"
                class="px-4 py-2 text-red-600 hover:bg-red-50 rounded-lg transition-colors"
              >
                Cancel Request
              </button>
              <button
                v-if="canRate"
                @click="showRatingModal = true"
                class="px-4 py-2 bg-teal-500 text-white rounded-lg hover:bg-teal-600 transition-colors"
              >
                <i class="fas fa-star mr-2"></i>
                Rate Service
              </button>
            </div>
          </div>
        </div>
      </header>

      <!-- Main Content -->
      <main class="max-w-6xl mx-auto px-4 py-8">
        <!-- Status Banner -->
        <div :class="[
          'rounded-2xl p-6 mb-8',
          statusConfig.bgClass
        ]">
          <div class="flex items-center justify-between">
            <div class="flex items-center gap-4">
              <div :class="[
                'w-14 h-14 rounded-xl flex items-center justify-center',
                statusConfig.class,
                'bg-white'
              ]">
                <i :class="[statusConfig.icon, 'text-2xl']"></i>
              </div>
              <div>
                <p class="text-sm font-medium text-gray-600">Current Status</p>
                <p :class="['text-2xl font-bold', statusConfig.class]">
                  {{ statusConfig.label }}
                </p>
              </div>
            </div>
            <div class="text-right">
              <p class="text-sm text-gray-600">Reference Number</p>
              <p class="text-lg font-mono font-semibold text-gray-900">{{ request.referenceNumber }}</p>
            </div>
          </div>
        </div>

        <!-- Progress Tracker -->
        <div class="bg-white rounded-2xl shadow-sm border border-gray-200 p-6 mb-8">
          <div class="flex items-center justify-between mb-4">
            <h2 class="text-lg font-semibold text-gray-900">Progress</h2>
            <span class="text-sm text-gray-500">{{ progressPercentage }}% Complete</span>
          </div>

          <!-- Progress Bar -->
          <div class="h-2 bg-gray-200 rounded-full mb-6 overflow-hidden">
            <div
              class="h-full bg-gradient-to-r from-teal-400 to-teal-600 rounded-full transition-all duration-500"
              :style="{ width: `${progressPercentage}%` }"
            ></div>
          </div>

          <!-- Steps -->
          <div class="relative">
            <div class="absolute top-5 left-5 right-5 h-0.5 bg-gray-200"></div>
            <div class="flex justify-between relative">
              <div
                v-for="(step, index) in request.steps"
                :key="step.id"
                class="flex flex-col items-center"
              >
                <div :class="[
                  'w-10 h-10 rounded-full flex items-center justify-center border-2 z-10 transition-all',
                  step.status === 'completed' ? 'bg-teal-500 border-teal-500 text-white' :
                  step.status === 'current' ? 'bg-white border-teal-500 text-teal-500' :
                  'bg-white border-gray-300 text-gray-400'
                ]">
                  <i v-if="step.status === 'completed'" class="fas fa-check"></i>
                  <i v-else-if="step.status === 'current'" class="fas fa-circle text-xs"></i>
                  <span v-else class="text-sm">{{ index + 1 }}</span>
                </div>
                <p :class="[
                  'text-xs mt-2 text-center max-w-[80px]',
                  step.status === 'current' ? 'text-teal-600 font-medium' : 'text-gray-500'
                ]">
                  {{ step.title }}
                </p>
              </div>
            </div>
          </div>
        </div>

        <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
          <!-- Left Column -->
          <div class="lg:col-span-2 space-y-6">
            <!-- Request Details -->
            <div class="bg-white rounded-2xl shadow-sm border border-gray-200 p-6">
              <h2 class="text-lg font-semibold text-gray-900 mb-4">Request Details</h2>

              <div class="flex items-start gap-4 mb-4">
                <div class="w-12 h-12 bg-teal-100 rounded-xl flex items-center justify-center">
                  <i :class="[request.categoryIcon, 'text-teal-600 text-xl']"></i>
                </div>
                <div>
                  <h3 class="text-xl font-semibold text-gray-900">{{ request.title }}</h3>
                  <div class="flex items-center gap-3 mt-1">
                    <span class="text-sm text-gray-500">{{ request.category }}</span>
                    <span :class="[
                      'px-2 py-0.5 rounded-full text-xs font-medium',
                      priorityConfig.class
                    ]">
                      <i :class="[priorityConfig.icon, 'mr-1']"></i>
                      {{ priorityConfig.label }} Priority
                    </span>
                  </div>
                </div>
              </div>

              <p class="text-gray-600 leading-relaxed mb-6">
                {{ request.description }}
              </p>

              <!-- Info Grid -->
              <div class="grid grid-cols-2 gap-4 pt-4 border-t border-gray-200">
                <div>
                  <p class="text-sm text-gray-500">Service Type</p>
                  <p class="font-medium text-gray-900">{{ request.serviceType }}</p>
                </div>
                <div>
                  <p class="text-sm text-gray-500">Department</p>
                  <p class="font-medium text-gray-900">{{ request.department }}</p>
                </div>
                <div>
                  <p class="text-sm text-gray-500">Created</p>
                  <p class="font-medium text-gray-900">{{ formatDate(request.createdAt) }}</p>
                </div>
                <div>
                  <p class="text-sm text-gray-500">Expected Completion</p>
                  <p class="font-medium text-gray-900">
                    {{ request.expectedCompletion ? formatDate(request.expectedCompletion) : 'TBD' }}
                  </p>
                </div>
              </div>
            </div>

            <!-- Activity Timeline -->
            <div class="bg-white rounded-2xl shadow-sm border border-gray-200 p-6">
              <h2 class="text-lg font-semibold text-gray-900 mb-6">Activity Timeline</h2>

              <div class="space-y-6">
                <div
                  v-for="event in [...request.timeline].reverse()"
                  :key="event.id"
                  class="flex gap-4"
                >
                  <!-- Icon -->
                  <div class="flex-shrink-0">
                    <div :class="[
                      'w-10 h-10 rounded-full flex items-center justify-center',
                      event.type === 'comment' ? 'bg-blue-100 text-blue-600' :
                      event.type === 'status_change' ? 'bg-green-100 text-green-600' :
                      event.type === 'assignment' ? 'bg-purple-100 text-purple-600' :
                      'bg-gray-100 text-gray-600'
                    ]">
                      <i :class="getTimelineIcon(event.type)"></i>
                    </div>
                  </div>

                  <!-- Content -->
                  <div class="flex-1 pb-6 border-b border-gray-100 last:border-0 last:pb-0">
                    <div class="flex items-start justify-between">
                      <div>
                        <p class="font-medium text-gray-900">{{ event.title }}</p>
                        <p v-if="event.author" class="text-sm text-gray-500">
                          by {{ event.author.name }}
                        </p>
                      </div>
                      <span class="text-xs text-gray-400">{{ formatRelativeTime(event.createdAt) }}</span>
                    </div>
                    <p class="text-gray-600 mt-2">{{ event.description }}</p>
                  </div>
                </div>
              </div>

              <!-- Message Input -->
              <div class="mt-6 pt-6 border-t border-gray-200">
                <h3 class="font-medium text-gray-900 mb-3">Send Message to Assignee</h3>
                <div class="flex gap-3">
                  <input
                    v-model="newMessage"
                    type="text"
                    placeholder="Type your message..."
                    class="flex-1 px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-teal-500 focus:border-teal-500"
                    @keyup.enter="sendMessage"
                  >
                  <button
                    @click="sendMessage"
                    :disabled="!newMessage.trim() || isSendingMessage"
                    :class="[
                      'px-4 py-2 rounded-lg font-medium transition-colors',
                      newMessage.trim()
                        ? 'bg-teal-500 text-white hover:bg-teal-600'
                        : 'bg-gray-200 text-gray-400 cursor-not-allowed'
                    ]"
                  >
                    <i v-if="isSendingMessage" class="fas fa-spinner fa-spin"></i>
                    <i v-else class="fas fa-paper-plane"></i>
                  </button>
                </div>
              </div>
            </div>

            <!-- Attachments -->
            <div class="bg-white rounded-2xl shadow-sm border border-gray-200 p-6">
              <h2 class="text-lg font-semibold text-gray-900 mb-4">Attachments</h2>

              <div class="space-y-3">
                <div
                  v-for="attachment in request.attachments"
                  :key="attachment.id"
                  class="flex items-center justify-between p-3 bg-gray-50 rounded-lg hover:bg-gray-100 transition-colors"
                >
                  <div class="flex items-center gap-3">
                    <i :class="[getFileIcon(attachment.type), 'text-xl']"></i>
                    <div>
                      <p class="font-medium text-gray-900">{{ attachment.name }}</p>
                      <p class="text-sm text-gray-500">{{ attachment.size }}</p>
                    </div>
                  </div>
                  <button class="p-2 text-gray-500 hover:text-teal-600 hover:bg-white rounded-lg transition-colors">
                    <i class="fas fa-download"></i>
                  </button>
                </div>
              </div>

              <!-- Upload Button -->
              <button class="mt-4 w-full py-3 border-2 border-dashed border-gray-300 rounded-xl text-gray-500 hover:border-teal-400 hover:text-teal-600 transition-colors">
                <i class="fas fa-plus mr-2"></i>
                Add Attachment
              </button>
            </div>

            <!-- Comments -->
            <div class="bg-white rounded-2xl shadow-sm border border-gray-200 p-6">
              <CommentsSection
                content-type="service-request"
                :content-id="request.id"
                :comments="comments"
                :is-loading="commentsLoading"
                @add-comment="addComment"
              />
            </div>
          </div>

          <!-- Right Column - Sidebar -->
          <div class="space-y-6">
            <!-- Requested By -->
            <div class="bg-white rounded-2xl shadow-sm border border-gray-200 p-6">
              <h3 class="font-semibold text-gray-900 mb-4">Requested By</h3>
              <div class="flex items-center gap-3">
                <div class="w-12 h-12 bg-gradient-to-br from-teal-400 to-teal-600 rounded-xl flex items-center justify-center text-white font-medium">
                  {{ request.requestedBy.initials }}
                </div>
                <div>
                  <p class="font-medium text-gray-900">{{ request.requestedBy.name }}</p>
                  <p class="text-sm text-gray-500">{{ request.requestedBy.role }}</p>
                  <p class="text-sm text-gray-400">{{ request.requestedBy.department }}</p>
                </div>
              </div>
            </div>

            <!-- Assigned To -->
            <div class="bg-white rounded-2xl shadow-sm border border-gray-200 p-6">
              <h3 class="font-semibold text-gray-900 mb-4">Assigned To</h3>
              <div v-if="request.assignedTo" class="flex items-center gap-3">
                <div class="w-12 h-12 bg-gradient-to-br from-blue-400 to-blue-600 rounded-xl flex items-center justify-center text-white font-medium">
                  {{ request.assignedTo.initials }}
                </div>
                <div>
                  <p class="font-medium text-gray-900">{{ request.assignedTo.name }}</p>
                  <p class="text-sm text-gray-500">{{ request.assignedTo.role }}</p>
                  <p class="text-sm text-gray-400">{{ request.assignedTo.department }}</p>
                </div>
              </div>
              <p v-else class="text-gray-500 text-sm">Not yet assigned</p>
            </div>

            <!-- SLA Information -->
            <div class="bg-white rounded-2xl shadow-sm border border-gray-200 p-6">
              <h3 class="font-semibold text-gray-900 mb-4">SLA Information</h3>
              <div class="space-y-3">
                <div class="flex items-center justify-between">
                  <span class="text-gray-500">Response Time</span>
                  <span class="text-green-600 font-medium">
                    <i class="fas fa-check-circle mr-1"></i>
                    Met (2h)
                  </span>
                </div>
                <div class="flex items-center justify-between">
                  <span class="text-gray-500">Resolution Target</span>
                  <span class="text-gray-900">7 business days</span>
                </div>
                <div class="flex items-center justify-between">
                  <span class="text-gray-500">Days Remaining</span>
                  <span class="text-orange-600 font-medium">5 days</span>
                </div>
              </div>
            </div>

            <!-- Rating (if completed) -->
            <div v-if="request.status === 'completed'" class="bg-white rounded-2xl shadow-sm border border-gray-200 p-6">
              <h3 class="font-semibold text-gray-900 mb-4">Service Rating</h3>
              <div v-if="request.rating" class="text-center">
                <RatingStars :model-value="request.rating" :readonly="true" size="lg" />
                <p class="text-sm text-gray-500 mt-2">Thank you for your feedback!</p>
              </div>
              <div v-else class="text-center">
                <p class="text-gray-500 mb-3">How was your experience?</p>
                <button
                  @click="showRatingModal = true"
                  class="px-4 py-2 bg-teal-500 text-white rounded-lg hover:bg-teal-600 transition-colors"
                >
                  Rate Now
                </button>
              </div>
            </div>

            <!-- Quick Actions -->
            <div class="bg-gradient-to-br from-teal-500 to-teal-600 rounded-2xl p-6 text-white">
              <h3 class="font-semibold mb-2">Need Help?</h3>
              <p class="text-teal-100 text-sm mb-4">
                Contact our support team for assistance with your request.
              </p>
              <a
                href="mailto:support@company.com"
                class="inline-flex items-center gap-2 px-4 py-2 bg-white text-teal-600 rounded-lg font-medium hover:bg-teal-50 transition-colors"
              >
                <i class="fas fa-envelope"></i>
                Contact Support
              </a>
            </div>
          </div>
        </div>
      </main>
    </template>

    <!-- Cancel Modal -->
    <Teleport to="body">
      <div
        v-if="showCancelModal"
        class="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4"
        @click.self="showCancelModal = false"
      >
        <div class="bg-white rounded-2xl max-w-md w-full p-6">
          <div class="flex items-center gap-4 mb-4">
            <div class="w-12 h-12 bg-red-100 rounded-xl flex items-center justify-center">
              <i class="fas fa-exclamation-triangle text-red-600 text-xl"></i>
            </div>
            <div>
              <h3 class="text-lg font-semibold text-gray-900">Cancel Request</h3>
              <p class="text-sm text-gray-500">This action cannot be undone</p>
            </div>
          </div>

          <p class="text-gray-600 mb-6">
            Are you sure you want to cancel this service request? Any progress made will be lost.
          </p>

          <div class="flex gap-3">
            <button
              @click="showCancelModal = false"
              class="flex-1 px-4 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors"
            >
              Keep Request
            </button>
            <button
              @click="cancelRequest"
              class="flex-1 px-4 py-2 bg-red-500 text-white rounded-lg hover:bg-red-600 transition-colors"
            >
              Cancel Request
            </button>
          </div>
        </div>
      </div>
    </Teleport>

    <!-- Rating Modal -->
    <Teleport to="body">
      <div
        v-if="showRatingModal"
        class="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4"
        @click.self="showRatingModal = false"
      >
        <div class="bg-white rounded-2xl max-w-md w-full p-6">
          <div class="text-center mb-6">
            <div class="w-16 h-16 bg-teal-100 rounded-full flex items-center justify-center mx-auto mb-4">
              <i class="fas fa-star text-teal-600 text-2xl"></i>
            </div>
            <h3 class="text-lg font-semibold text-gray-900">Rate Your Experience</h3>
            <p class="text-gray-500 mt-1">How was the service you received?</p>
          </div>

          <div class="flex justify-center mb-6">
            <RatingStars
              :model-value="0"
              size="lg"
              @update:model-value="handleRating"
            />
          </div>

          <button
            @click="showRatingModal = false"
            class="w-full py-2 text-gray-600 hover:text-gray-800"
          >
            Skip for now
          </button>
        </div>
      </div>
    </Teleport>
  </div>
</template>

<style scoped>
.service-request-page {
  animation: fadeIn 0.3s ease;
}

@keyframes fadeIn {
  from { opacity: 0; }
  to { opacity: 1; }
}
</style>
