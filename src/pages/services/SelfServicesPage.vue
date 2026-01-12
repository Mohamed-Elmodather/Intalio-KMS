<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'
import { useAIServicesStore } from '@/stores/aiServices'
import { AILoadingIndicator, AISuggestionChip } from '@/components/ai'

const router = useRouter()

// Initialize AI store
const aiStore = useAIServicesStore()

// State
const showNewRequest = ref(false)
const selectedService = ref<any>(null)
const filterCategory = ref('all')
const requestFilter = ref('all')
const aiQuery = ref('')
const isLoading = ref(false)

// Stats data
const stats = ref({
  itRequests: 12,
  hrRequests: 8,
  facilitiesRequests: 5,
  financeRequests: 3
})

// Page stats for hero section
// Stats matching static portal
const openRequestsCount = ref(3)
const completedRequestsCount = ref(24)
const avgResolutionTime = ref(4)
const totalServices = ref(15)


// Categories
const categories = ref([
  { id: 'all', label: 'All Services' },
  { id: 'it', label: 'IT' },
  { id: 'hr', label: 'HR' },
  { id: 'facilities', label: 'Facilities' },
  { id: 'finance', label: 'Finance' }
])

// Services data
const services = ref([
  { id: 1, name: 'Password Reset', description: 'Reset your account password', category: 'it', icon: 'fas fa-key', iconBg: 'bg-teal-100', iconColor: 'text-teal-600', eta: '< 1 hour', priority: 'Quick', priorityClass: 'bg-teal-100 text-teal-700' },
  { id: 2, name: 'Software Request', description: 'Request new software installation', category: 'it', icon: 'fas fa-download', iconBg: 'bg-teal-100', iconColor: 'text-teal-600', eta: '2-3 days', priority: 'Standard', priorityClass: 'bg-teal-100 text-teal-700' },
  { id: 3, name: 'Hardware Request', description: 'Request new equipment or peripherals', category: 'it', icon: 'fas fa-desktop', iconBg: 'bg-teal-100', iconColor: 'text-teal-600', eta: '5-7 days', priority: 'Standard', priorityClass: 'bg-teal-100 text-teal-700' },
  { id: 4, name: 'VPN Access', description: 'Request VPN access for remote work', category: 'it', icon: 'fas fa-shield-alt', iconBg: 'bg-teal-100', iconColor: 'text-teal-600', eta: '1-2 days', priority: 'Standard', priorityClass: 'bg-teal-100 text-teal-700' },
  { id: 5, name: 'Leave Request', description: 'Submit vacation or sick leave', category: 'hr', icon: 'fas fa-calendar-alt', iconBg: 'bg-teal-100', iconColor: 'text-teal-600', eta: '1-2 days', priority: 'Standard', priorityClass: 'bg-teal-100 text-teal-700' },
  { id: 6, name: 'Benefits Inquiry', description: 'Questions about benefits and insurance', category: 'hr', icon: 'fas fa-heart', iconBg: 'bg-teal-100', iconColor: 'text-teal-600', eta: '< 1 day', priority: 'Quick', priorityClass: 'bg-teal-100 text-teal-700' },
  { id: 7, name: 'Training Request', description: 'Request professional development training', category: 'hr', icon: 'fas fa-graduation-cap', iconBg: 'bg-teal-100', iconColor: 'text-teal-600', eta: '3-5 days', priority: 'Standard', priorityClass: 'bg-teal-100 text-teal-700' },
  { id: 8, name: 'Room Booking', description: 'Reserve meeting rooms and spaces', category: 'facilities', icon: 'fas fa-door-open', iconBg: 'bg-teal-100', iconColor: 'text-teal-700', eta: 'Instant', priority: 'Quick', priorityClass: 'bg-teal-100 text-teal-700' },
  { id: 9, name: 'Maintenance Request', description: 'Report facility issues', category: 'facilities', icon: 'fas fa-tools', iconBg: 'bg-teal-100', iconColor: 'text-teal-700', eta: '1-3 days', priority: 'Standard', priorityClass: 'bg-teal-100 text-teal-700' },
  { id: 10, name: 'Expense Report', description: 'Submit expense reimbursement', category: 'finance', icon: 'fas fa-receipt', iconBg: 'bg-teal-100', iconColor: 'text-teal-600', eta: '3-5 days', priority: 'Standard', priorityClass: 'bg-teal-100 text-teal-700' },
  { id: 11, name: 'Travel Request', description: 'Request travel approval', category: 'finance', icon: 'fas fa-plane', iconBg: 'bg-teal-100', iconColor: 'text-teal-600', eta: '2-3 days', priority: 'Standard', priorityClass: 'bg-teal-100 text-teal-700' }
])

// My Requests data
const myRequests = ref([
  {
    id: 'SR-1234',
    title: 'New Laptop Request',
    description: 'Requesting MacBook Pro for development work',
    status: 'In Progress',
    statusClass: 'badge-info',
    date: '2 days ago',
    icon: 'fas fa-laptop',
    iconBg: 'bg-teal-100',
    iconColor: 'text-teal-600',
    steps: [
      { label: 'Submitted', completed: true },
      { label: 'Approved', completed: true },
      { label: 'Processing', completed: false },
      { label: 'Delivered', completed: false }
    ]
  },
  {
    id: 'SR-1235',
    title: 'VPN Access Request',
    description: 'Need VPN access for remote work',
    status: 'Pending',
    statusClass: 'badge-warning',
    date: '1 day ago',
    icon: 'fas fa-shield-alt',
    iconBg: 'bg-teal-100',
    iconColor: 'text-teal-600',
    steps: [
      { label: 'Submitted', completed: true },
      { label: 'Review', completed: false },
      { label: 'Setup', completed: false },
      { label: 'Complete', completed: false }
    ]
  },
  {
    id: 'SR-1230',
    title: 'Software Installation - Figma',
    description: 'Request to install Figma desktop app',
    status: 'Completed',
    statusClass: 'badge-success',
    date: '1 week ago',
    icon: 'fas fa-download',
    iconBg: 'bg-teal-100',
    iconColor: 'text-teal-600',
    steps: [
      { label: 'Submitted', completed: true },
      { label: 'Approved', completed: true },
      { label: 'Installed', completed: true },
      { label: 'Complete', completed: true }
    ]
  }
])

// Quick Queries for AI
const quickQueries = ref([
  'How do I reset my password?',
  'What are my leave balances?',
  'How to book a meeting room?'
])

// FAQs
const faqs = ref([
  { question: 'How long does IT support take?', answer: 'Most IT requests are resolved within 24-48 hours. Critical issues are prioritized and handled immediately.', open: false },
  { question: 'Can I track my request status?', answer: 'Yes! All requests can be tracked in the "My Requests" section. You\'ll also receive email notifications for status updates.', open: false },
  { question: 'How do I escalate a request?', answer: 'If your request is urgent, use the priority flag when submitting or contact the helpdesk directly.', open: false },
  { question: 'What if I need to cancel a request?', answer: 'You can cancel pending requests from the request detail page. Completed or in-progress requests may require contacting support.', open: false }
])

// Priorities
const priorities = ref([
  { value: 'low', label: 'Low' },
  { value: 'medium', label: 'Medium' },
  { value: 'high', label: 'High' }
])

// New Request Form
const newRequest = ref({
  subject: '',
  priority: 'medium',
  description: ''
})

// Computed properties
const filteredServices = computed(() => {
  if (filterCategory.value === 'all') return services.value
  return services.value.filter(s => s.category === filterCategory.value)
})

const filteredRequests = computed(() => {
  if (requestFilter.value === 'all') return myRequests.value
  const statusMap: Record<string, string> = {
    'pending': 'Pending',
    'in-progress': 'In Progress',
    'completed': 'Completed'
  }
  return myRequests.value.filter(r => r.status === statusMap[requestFilter.value])
})

// Methods
function selectService(service: any) {
  selectedService.value = service
  showNewRequest.value = true
}

function closeNewRequest() {
  showNewRequest.value = false
  selectedService.value = null
  newRequest.value = { subject: '', priority: 'medium', description: '' }
}

function submitRequest() {
  alert('Request submitted successfully!')
  closeNewRequest()
}

function showRequestDetail(request: any) {
  router.push({ name: 'ServiceRequestDetail', params: { id: request.id } })
}

function askAI(query: string) {
  if (query) {
    // Show AI chat or handle query
    processAIQuery(query)
    aiQuery.value = ''
  }
}

// ============================================================================
// AI Features State & Functions
// ============================================================================

// AI State
const showAIRecommendationsPanel = ref(false)
const showAutoFillModal = ref(false)
const isProcessingAI = ref(false)
const isGeneratingAutoFill = ref(false)
const aiResponse = ref('')
const aiRecommendations = ref<AIServiceRecommendation[]>([])
const autoFillSuggestions = ref<AutoFillSuggestion | null>(null)

// AI Interfaces
interface AIServiceRecommendation {
  id: string
  serviceId: number
  serviceName: string
  reason: string
  confidence: number
  matchType: 'exact' | 'related' | 'suggested'
}

interface AutoFillSuggestion {
  subject: string
  description: string
  priority: string
  additionalInfo: string[]
}

// Mock AI Data
const mockAIResponses: Record<string, string> = {
  'password': 'To reset your password, go to IT Services > Password Reset. The process takes less than 1 hour. You\'ll receive an email with reset instructions.',
  'leave': 'You can check your leave balance in HR Services > Leave Request. Current balance: 15 vacation days, 5 sick days remaining.',
  'room': 'To book a meeting room, go to Facilities > Room Booking. Available rooms can be reserved instantly through the calendar system.',
  'vpn': 'VPN access can be requested through IT Services. Processing takes 1-2 business days. Make sure to specify your work-from-home schedule.',
  'default': 'I can help you find the right service. Try asking about password reset, leave balance, room booking, or VPN access.'
}

const mockRecommendations: AIServiceRecommendation[] = [
  { id: '1', serviceId: 1, serviceName: 'Password Reset', reason: 'Most commonly requested IT service', confidence: 0.95, matchType: 'exact' },
  { id: '2', serviceId: 4, serviceName: 'VPN Access', reason: 'Popular among remote workers', confidence: 0.85, matchType: 'related' },
  { id: '3', serviceId: 5, serviceName: 'Leave Request', reason: 'Upcoming holiday season', confidence: 0.78, matchType: 'suggested' }
]

const mockAutoFillSuggestion: AutoFillSuggestion = {
  subject: 'Request for new development laptop',
  description: 'I am requesting a new laptop for my development work. My current machine is experiencing performance issues that affect productivity.',
  priority: 'medium',
  additionalInfo: [
    'Preferred model: MacBook Pro 14" or equivalent',
    'Required software: VS Code, Docker, Node.js',
    'Current laptop age: 3+ years',
    'Impact: Reduced build times and better multitasking'
  ]
}

// AI Functions
async function processAIQuery(query: string) {
  isProcessingAI.value = true

  try {
    await new Promise(resolve => setTimeout(resolve, 800))

    const lowerQuery = query.toLowerCase()
    if (lowerQuery.includes('password')) {
      aiResponse.value = mockAIResponses['password']
    } else if (lowerQuery.includes('leave') || lowerQuery.includes('vacation')) {
      aiResponse.value = mockAIResponses['leave']
    } else if (lowerQuery.includes('room') || lowerQuery.includes('meeting')) {
      aiResponse.value = mockAIResponses['room']
    } else if (lowerQuery.includes('vpn') || lowerQuery.includes('remote')) {
      aiResponse.value = mockAIResponses['vpn']
    } else {
      aiResponse.value = mockAIResponses['default']
    }

    // Also generate recommendations
    await fetchAIRecommendations()
  } catch (error) {
    console.error('AI query failed:', error)
    aiResponse.value = 'Sorry, I couldn\'t process your request. Please try again.'
  } finally {
    isProcessingAI.value = false
  }
}

async function fetchAIRecommendations() {
  showAIRecommendationsPanel.value = true

  try {
    await new Promise(resolve => setTimeout(resolve, 600))
    aiRecommendations.value = mockRecommendations
  } catch (error) {
    console.error('Failed to fetch recommendations:', error)
  }
}

async function generateAutoFill() {
  isGeneratingAutoFill.value = true
  showAutoFillModal.value = true

  try {
    await new Promise(resolve => setTimeout(resolve, 1000))
    autoFillSuggestions.value = mockAutoFillSuggestion
  } catch (error) {
    console.error('Auto-fill generation failed:', error)
  } finally {
    isGeneratingAutoFill.value = false
  }
}

function applyAutoFill() {
  if (autoFillSuggestions.value) {
    newRequest.value.subject = autoFillSuggestions.value.subject
    newRequest.value.description = autoFillSuggestions.value.description
    newRequest.value.priority = autoFillSuggestions.value.priority
    showAutoFillModal.value = false
  }
}

function selectRecommendedService(recommendation: AIServiceRecommendation) {
  const service = services.value.find(s => s.id === recommendation.serviceId)
  if (service) {
    selectService(service)
    showAIRecommendationsPanel.value = false
  }
}

function getMatchTypeColor(type: string) {
  switch (type) {
    case 'exact': return 'bg-green-100 text-green-700'
    case 'related': return 'bg-blue-100 text-blue-700'
    case 'suggested': return 'bg-amber-100 text-amber-700'
    default: return 'bg-gray-100 text-gray-700'
  }
}
</script>

<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Loading State -->
    <div v-if="isLoading" class="flex items-center justify-center min-h-[400px]">
      <LoadingSpinner size="lg" text="Loading services..." />
    </div>

    <template v-else>
      <!-- Hero Section - Documents Style -->
      <div class="hero-gradient relative overflow-hidden">
        <!-- Decorative elements with animations -->
        <div class="circle-drift-1 absolute top-0 right-0 w-96 h-96 bg-white/5 rounded-full"></div>
        <div class="circle-drift-2 absolute bottom-0 left-0 w-64 h-64 bg-white/5 rounded-full"></div>
        <div class="circle-drift-3 absolute top-1/2 right-1/4 w-32 h-32 bg-white/10 rounded-full"></div>

        <!-- Stats - Absolute Top Right -->
        <div class="stats-top-right">
          <div class="stat-card-square">
            <div class="stat-icon-box">
              <i class="fas fa-clock"></i>
            </div>
            <p class="stat-value-mini">{{ openRequestsCount }}</p>
            <p class="stat-label-mini">Open</p>
          </div>
          <div class="stat-card-square">
            <div class="stat-icon-box">
              <i class="fas fa-check-circle"></i>
            </div>
            <p class="stat-value-mini">{{ completedRequestsCount }}</p>
            <p class="stat-label-mini">Completed</p>
          </div>
          <div class="stat-card-square">
            <div class="stat-icon-box">
              <i class="fas fa-bolt"></i>
            </div>
            <p class="stat-value-mini">{{ avgResolutionTime }}h</p>
            <p class="stat-label-mini">Avg. Time</p>
          </div>
          <div class="stat-card-square">
            <div class="stat-icon-box">
              <i class="fas fa-th-large"></i>
            </div>
            <p class="stat-value-mini">{{ totalServices }}</p>
            <p class="stat-label-mini">Services</p>
          </div>
        </div>

        <div class="relative px-8 py-8">
          <div class="px-3 py-1 bg-white/20 backdrop-blur-sm rounded-full text-white text-xs font-semibold inline-flex items-center gap-2 mb-4">
            <i class="fas fa-concierge-bell"></i>
            AFC Asian Cup 2027
          </div>

          <h1 class="text-3xl font-bold text-white mb-2">Self-Service Portal</h1>
          <p class="text-teal-100 max-w-lg">Request services, track progress, and get instant help from our support team.</p>

          <div class="flex flex-wrap gap-3 mt-6">
            <button @click="showNewRequest = true" class="px-5 py-2.5 bg-white text-teal-600 rounded-xl font-semibold text-sm flex items-center gap-2 hover:bg-teal-50 transition-all shadow-lg">
              <i class="fas fa-plus"></i>
              New Request
            </button>
            <button class="px-5 py-2.5 bg-white/20 backdrop-blur-sm border border-white/30 text-white rounded-xl font-semibold text-sm hover:bg-white/30 transition-all flex items-center gap-2">
              <i class="fas fa-history"></i>
              View History
            </button>
            <!-- AI Action Buttons -->
            <button @click="fetchAIRecommendations"
                    class="px-5 py-2.5 bg-gradient-to-r from-teal-500/20 to-emerald-500/20 backdrop-blur-sm text-white rounded-xl font-semibold text-sm flex items-center gap-2 hover:from-teal-500/30 hover:to-emerald-500/30 transition-all border border-white/20">
              <i class="fas fa-wand-magic-sparkles"></i>
              Smart Suggestions
            </button>
          </div>
        </div>
      </div>

      <!-- Main Content -->
      <div class="px-8 py-6 space-y-6">
        <div class="grid grid-cols-1 xl:grid-cols-3 gap-6">
          <!-- Service Catalog -->
          <div class="xl:col-span-2 space-y-6">
            <div class="bg-white rounded-2xl shadow-sm border border-gray-100 overflow-hidden">
              <div class="border-b border-gray-100">
                <!-- Header Row -->
                <div class="px-5 py-4 flex items-center justify-between">
                  <h2 class="text-lg font-bold text-gray-900 flex items-center gap-3">
                    <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-teal-500 to-teal-600 flex items-center justify-center shadow-lg shadow-teal-200">
                      <i class="fas fa-th-large text-white text-sm"></i>
                    </div>
                    <div>
                      <span class="block">Service Catalog</span>
                      <span class="text-xs font-medium text-gray-500">{{ filteredServices.length }} services available</span>
                    </div>
                  </h2>
                  <button @click="showNewRequest = true" class="px-4 py-2 bg-gradient-to-r from-teal-500 to-teal-600 text-white rounded-lg text-sm font-medium hover:from-teal-600 hover:to-teal-700 transition-all flex items-center gap-2 shadow-sm shadow-teal-200">
                    <i class="fas fa-plus"></i>
                    New Request
                  </button>
                </div>
                <!-- Filter Row -->
                <div class="px-5 py-2 bg-gray-50/50 flex flex-wrap items-center gap-2">
                  <button v-for="cat in categories" :key="cat.id"
                          @click="filterCategory = cat.id"
                          :class="['px-3 py-1.5 rounded-lg text-xs font-medium transition-all',
                                   filterCategory === cat.id ? 'bg-teal-500 text-white shadow-sm' : 'bg-white border border-gray-200 text-gray-600 hover:bg-gray-50 hover:border-gray-300']">
                    {{ cat.label }}
                  </button>
                </div>
              </div>

              <div class="p-5 grid grid-cols-1 md:grid-cols-2 gap-4">
                <div v-for="service in filteredServices" :key="service.id"
                     @click="selectService(service)"
                     class="group p-4 rounded-xl bg-white border border-gray-100 hover:border-teal-200 hover:shadow-lg cursor-pointer transition-all">
                  <div class="flex items-start gap-4">
                    <div class="w-12 h-12 rounded-xl bg-gradient-to-br from-teal-50 to-teal-100 flex items-center justify-center flex-shrink-0 transition-transform group-hover:scale-110 group-hover:shadow-md">
                      <i :class="[service.icon, 'text-teal-600 text-xl']"></i>
                    </div>
                    <div class="flex-1 min-w-0">
                      <h3 class="font-semibold text-gray-900 group-hover:text-teal-600 transition-colors">{{ service.name }}</h3>
                      <p class="text-sm text-gray-500 mt-1">{{ service.description }}</p>
                      <div class="flex items-center gap-3 mt-2">
                        <span class="text-xs text-gray-400 flex items-center gap-1"><i class="fas fa-clock text-[10px]"></i>{{ service.eta }}</span>
                        <span class="text-xs px-2 py-0.5 rounded-full bg-teal-50 text-teal-700">{{ service.priority }}</span>
                      </div>
                    </div>
                    <i class="fas fa-arrow-right text-gray-300 group-hover:text-teal-500 group-hover:translate-x-1 transition-all"></i>
                  </div>
                </div>
              </div>
            </div>

            <!-- My Requests -->
            <div class="bg-white rounded-2xl shadow-sm border border-gray-100 overflow-hidden">
              <div class="border-b border-gray-100">
                <div class="px-5 py-4 flex items-center justify-between">
                  <h2 class="text-lg font-bold text-gray-900 flex items-center gap-3">
                    <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-blue-500 to-blue-600 flex items-center justify-center shadow-lg shadow-blue-200">
                      <i class="fas fa-clipboard-list text-white text-sm"></i>
                    </div>
                    <div>
                      <span class="block">My Requests</span>
                      <span class="text-xs font-medium text-gray-500">{{ filteredRequests.length }} requests</span>
                    </div>
                  </h2>
                  <div class="flex items-center gap-2">
                    <select v-model="requestFilter" class="px-3 py-1.5 bg-white border border-gray-200 rounded-lg text-sm text-gray-600 focus:outline-none focus:ring-2 focus:ring-teal-500 focus:border-transparent">
                      <option value="all">All Status</option>
                      <option value="pending">Pending</option>
                      <option value="in-progress">In Progress</option>
                      <option value="completed">Completed</option>
                    </select>
                  </div>
                </div>
              </div>

              <div class="divide-y divide-gray-100">
                <div v-for="request in filteredRequests" :key="request.id"
                     @click="showRequestDetail(request)"
                     class="list-item-animated p-4 hover:bg-teal-50/50 cursor-pointer transition-colors ripple-effect">
                  <div class="flex items-center gap-4">
                    <div :class="['icon-soft w-10 h-10 rounded-xl flex items-center justify-center hover-scale', request.iconBg]">
                      <i :class="[request.icon, request.iconColor]"></i>
                    </div>
                    <div class="flex-1 min-w-0">
                      <div class="flex items-center gap-2">
                        <h3 class="font-medium text-gray-900">{{ request.title }}</h3>
                        <span class="text-xs text-gray-400">#{{ request.id }}</span>
                      </div>
                      <p class="text-sm text-gray-500 truncate">{{ request.description }}</p>
                    </div>
                    <div class="text-right flex-shrink-0">
                      <span :class="['badge', request.statusClass]">{{ request.status }}</span>
                      <p class="text-xs text-gray-400 mt-1">{{ request.date }}</p>
                    </div>
                  </div>

                  <!-- Progress Timeline -->
                  <div class="mt-4 ml-14">
                    <div class="flex items-center gap-2">
                      <div v-for="(step, idx) in request.steps" :key="idx" class="flex items-center">
                        <div :class="['w-6 h-6 rounded-full flex items-center justify-center text-xs',
                                     step.completed ? 'bg-teal-500 text-white' : 'bg-gray-100 text-gray-400']">
                          <i v-if="step.completed" class="fas fa-check"></i>
                          <span v-else>{{ idx + 1 }}</span>
                        </div>
                        <div v-if="idx < request.steps.length - 1"
                             :class="['w-12 h-0.5', step.completed ? 'bg-teal-500' : 'bg-gray-100']"></div>
                      </div>
                    </div>
                    <div class="flex gap-2 mt-1 text-xs text-gray-500">
                      <span v-for="(step, idx) in request.steps" :key="idx"
                            :class="['w-6', idx < request.steps.length - 1 ? 'mr-12' : '']">
                        {{ step.label }}
                      </span>
                    </div>
                  </div>
                </div>
              </div>

              <div v-if="filteredRequests.length === 0" class="p-8 text-center">
                <div class="w-16 h-16 rounded-full bg-teal-100 flex items-center justify-center mx-auto mb-4">
                  <i class="fas fa-inbox text-teal-400 text-2xl"></i>
                </div>
                <p class="text-gray-500">No requests found</p>
              </div>
            </div>
          </div>

          <!-- Right Sidebar -->
          <div class="space-y-6">
            <!-- AI Assistant Widget -->
            <div class="bg-white rounded-2xl p-5 shadow-sm border border-gray-100">
              <div class="flex items-center gap-3 mb-4">
                <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-purple-500 to-purple-600 flex items-center justify-center shadow-lg shadow-purple-200">
                  <i class="fas fa-robot text-white"></i>
                </div>
                <div>
                  <h3 class="font-bold text-gray-900">AI Assistant</h3>
                  <p class="text-xs text-gray-500">Get instant help</p>
                </div>
              </div>

              <div class="space-y-2 mb-4">
                <button v-for="(query, idx) in quickQueries" :key="idx"
                        @click="askAI(query)"
                        class="w-full p-3 text-left text-sm rounded-xl bg-gray-50 hover:bg-teal-50 hover:border-teal-200 border border-gray-100 text-gray-700 transition-all">
                  <i class="fas fa-comment-dots text-teal-500 mr-2"></i>{{ query }}
                </button>
              </div>

              <div class="relative">
                <input type="text" v-model="aiQuery" @keydown.enter="askAI(aiQuery)"
                       placeholder="Ask anything..."
                       class="w-full pl-4 pr-10 py-2.5 bg-gray-50 border border-gray-200 rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-teal-500 focus:border-transparent transition-all">
                <button @click="askAI(aiQuery)" class="absolute right-2 top-1/2 -translate-y-1/2 p-1.5 rounded-lg hover:bg-teal-100 text-teal-500 transition-colors">
                  <i class="fas fa-paper-plane"></i>
                </button>
              </div>
            </div>

            <!-- FAQ -->
            <div class="bg-white rounded-2xl p-5 shadow-sm border border-gray-100">
              <div class="flex items-center gap-3 mb-4">
                <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-amber-400 to-amber-500 flex items-center justify-center shadow-lg shadow-amber-200">
                  <i class="fas fa-question text-white"></i>
                </div>
                <div>
                  <h3 class="font-bold text-gray-900">FAQ</h3>
                  <p class="text-xs text-gray-500">Common questions</p>
                </div>
              </div>
              <div class="space-y-2">
                <div v-for="(faq, idx) in faqs" :key="idx" class="rounded-xl border border-gray-100 overflow-hidden">
                  <button @click="faq.open = !faq.open" class="w-full p-3 text-left flex items-center justify-between bg-gray-50 hover:bg-gray-100 transition-colors">
                    <span class="text-sm font-medium text-gray-800">{{ faq.question }}</span>
                    <i :class="['fas text-gray-400 transition-transform text-xs', faq.open ? 'fa-chevron-up' : 'fa-chevron-down']"></i>
                  </button>
                  <div v-if="faq.open" class="p-3 text-sm text-gray-600 bg-white border-t border-gray-100">{{ faq.answer }}</div>
                </div>
              </div>
            </div>

            <!-- Contact Support -->
            <div class="bg-white rounded-2xl p-5 shadow-sm border border-gray-100">
              <div class="flex items-center gap-3 mb-4">
                <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-green-500 to-green-600 flex items-center justify-center shadow-lg shadow-green-200">
                  <i class="fas fa-headset text-white"></i>
                </div>
                <div>
                  <h3 class="font-bold text-gray-900">Need Help?</h3>
                  <p class="text-xs text-gray-500">Contact support</p>
                </div>
              </div>
              <div class="space-y-2">
                <a href="#" class="flex items-center gap-3 p-3 rounded-xl bg-gray-50 hover:bg-teal-50 border border-gray-100 hover:border-teal-200 transition-all group">
                  <div class="w-9 h-9 rounded-lg bg-teal-100 flex items-center justify-center group-hover:scale-110 transition-transform">
                    <i class="fas fa-phone text-teal-600 text-sm"></i>
                  </div>
                  <div>
                    <p class="text-sm font-medium text-gray-800 group-hover:text-teal-600 transition-colors">Call Support</p>
                    <p class="text-xs text-gray-500">+1 (555) 123-4567</p>
                  </div>
                </a>
                <a href="#" class="flex items-center gap-3 p-3 rounded-xl bg-gray-50 hover:bg-teal-50 border border-gray-100 hover:border-teal-200 transition-all group">
                  <div class="w-9 h-9 rounded-lg bg-blue-100 flex items-center justify-center group-hover:scale-110 transition-transform">
                    <i class="fas fa-envelope text-blue-600 text-sm"></i>
                  </div>
                  <div>
                    <p class="text-sm font-medium text-gray-800 group-hover:text-teal-600 transition-colors">Email Us</p>
                    <p class="text-xs text-gray-500">support@afc.com</p>
                  </div>
                </a>
                <a href="#" class="flex items-center gap-3 p-3 rounded-xl bg-gray-50 hover:bg-teal-50 border border-gray-100 hover:border-teal-200 transition-all group">
                  <div class="w-9 h-9 rounded-lg bg-purple-100 flex items-center justify-center group-hover:scale-110 transition-transform">
                    <i class="fas fa-comments text-purple-600 text-sm"></i>
                  </div>
                  <div>
                    <p class="text-sm font-medium text-gray-800 group-hover:text-teal-600 transition-colors">Live Chat</p>
                    <p class="text-xs text-gray-500">Available 24/7</p>
                  </div>
                </a>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- New Request Modal -->
      <div v-if="showNewRequest" class="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4 fade-in-up">
        <div class="card-animated w-full max-w-2xl max-h-[90vh] overflow-y-auto">
          <div class="p-5 border-b border-gray-100 flex items-center justify-between sticky top-0 bg-white/80 backdrop-blur-sm">
            <h2 class="text-xl font-semibold text-gray-900">
              {{ selectedService ? selectedService.name : 'New Service Request' }}
            </h2>
            <button @click="closeNewRequest" class="p-2 rounded-lg hover:bg-teal-50 text-gray-500 ripple-effect hover-scale">
              <i class="fas fa-times"></i>
            </button>
          </div>

          <div class="p-6">
            <!-- Service Selection -->
            <div v-if="!selectedService" class="grid grid-cols-2 gap-4">
              <div v-for="service in services" :key="service.id"
                   @click="selectedService = service"
                   class="list-item-animated p-4 rounded-xl border-2 border-gray-100 hover:border-gray-300 cursor-pointer transition-all ripple-effect">
                <div :class="['icon-soft w-10 h-10 rounded-lg flex items-center justify-center mb-3 hover-scale', service.iconBg]">
                  <i :class="[service.icon, service.iconColor]"></i>
                </div>
                <h3 class="font-medium text-gray-900">{{ service.name }}</h3>
                <p class="text-xs text-gray-500 mt-1">{{ service.description }}</p>
              </div>
            </div>

            <!-- Request Form -->
            <form v-else @submit.prevent="submitRequest" class="space-y-5">
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">Subject</label>
                <input type="text" v-model="newRequest.subject" class="input" placeholder="Brief description of your request" required>
              </div>

              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">Priority</label>
                <div class="flex gap-3">
                  <label v-for="p in priorities" :key="p.value"
                         :class="['flex-1 p-3 rounded-xl border-2 cursor-pointer text-center transition-all',
                                  newRequest.priority === p.value ? 'border-teal-500 bg-teal-50' : 'border-gray-100 hover:border-gray-200']">
                    <input type="radio" v-model="newRequest.priority" :value="p.value" class="hidden">
                    <span :class="['text-sm font-medium', newRequest.priority === p.value ? 'text-teal-700' : 'text-gray-500']">{{ p.label }}</span>
                  </label>
                </div>
              </div>

              <div>
                <div class="flex items-center justify-between mb-2">
                  <label class="block text-sm font-medium text-gray-700">Description</label>
                  <button @click="generateAutoFill" type="button"
                          class="px-3 py-1 text-xs font-medium text-teal-600 bg-teal-50 rounded-lg hover:bg-teal-100 transition-colors flex items-center gap-1">
                    <i class="fas fa-wand-magic-sparkles"></i>
                    AI Auto-fill
                  </button>
                </div>
                <textarea v-model="newRequest.description" class="input" rows="4" placeholder="Provide detailed information about your request..." required></textarea>
              </div>

              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">Attachments</label>
                <div class="border-2 border-dashed border-gray-200 rounded-xl p-6 text-center hover:border-gray-400 transition-colors cursor-pointer">
                  <i class="fas fa-cloud-upload-alt text-3xl text-gray-300 mb-2"></i>
                  <p class="text-sm text-gray-500">Drop files here or click to upload</p>
                  <p class="text-xs text-gray-400 mt-1">Max 10MB per file</p>
                </div>
              </div>

              <div class="flex gap-3 pt-4">
                <button type="button" @click="selectedService = null" class="btn btn-secondary flex-1 ripple-effect">
                  <i class="fas fa-arrow-left"></i> Back
                </button>
                <button type="submit" class="btn btn-vibrant flex-1 ripple-effect">
                  <i class="fas fa-paper-plane"></i> Submit Request
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>

      <!-- AI Recommendations Panel -->
      <Teleport to="body">
        <div v-if="showAIRecommendationsPanel" class="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
          <div class="bg-white rounded-2xl shadow-2xl w-full max-w-lg max-h-[80vh] overflow-hidden">
            <div class="p-6 border-b border-gray-100">
              <div class="flex items-center justify-between">
                <div class="flex items-center gap-3">
                  <div class="w-10 h-10 bg-gradient-to-br from-teal-500 to-emerald-500 rounded-xl flex items-center justify-center">
                    <i class="fas fa-wand-magic-sparkles text-white"></i>
                  </div>
                  <div>
                    <h3 class="text-lg font-semibold text-gray-900">Smart Suggestions</h3>
                    <p class="text-sm text-gray-500">AI-recommended services for you</p>
                  </div>
                </div>
                <button @click="showAIRecommendationsPanel = false" class="p-2 hover:bg-gray-100 rounded-lg transition-colors">
                  <i class="fas fa-times text-gray-400"></i>
                </button>
              </div>
            </div>

            <div class="p-6 overflow-y-auto max-h-[60vh]">
              <!-- AI Response -->
              <div v-if="aiResponse" class="bg-gradient-to-r from-teal-50 to-emerald-50 rounded-xl p-4 mb-4">
                <div class="flex items-start gap-3">
                  <div class="w-8 h-8 bg-teal-500 rounded-lg flex items-center justify-center flex-shrink-0">
                    <i class="fas fa-robot text-white text-sm"></i>
                  </div>
                  <p class="text-gray-700 text-sm">{{ aiResponse }}</p>
                </div>
              </div>

              <!-- Recommended Services -->
              <div v-if="aiRecommendations.length > 0" class="space-y-3">
                <h4 class="text-sm font-semibold text-gray-500 uppercase">Recommended Services</h4>
                <div v-for="rec in aiRecommendations" :key="rec.id"
                     class="border border-gray-200 rounded-xl p-4 hover:border-teal-300 hover:bg-teal-50/30 transition-all cursor-pointer"
                     @click="selectRecommendedService(rec)">
                  <div class="flex items-start justify-between">
                    <div class="flex-1">
                      <div class="flex items-center gap-2 mb-1">
                        <h5 class="font-semibold text-gray-900">{{ rec.serviceName }}</h5>
                        <span :class="['px-2 py-0.5 rounded-full text-xs font-medium capitalize', getMatchTypeColor(rec.matchType)]">
                          {{ rec.matchType }}
                        </span>
                      </div>
                      <p class="text-sm text-gray-600">{{ rec.reason }}</p>
                    </div>
                    <div class="text-right ml-4">
                      <div class="text-lg font-bold text-teal-600">{{ Math.round(rec.confidence * 100) }}%</div>
                      <div class="text-xs text-gray-500">Match</div>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <div class="p-4 border-t border-gray-100 flex justify-end gap-3">
              <button @click="fetchAIRecommendations"
                      class="px-4 py-2 text-teal-600 hover:bg-teal-50 rounded-lg transition-colors flex items-center gap-2">
                <i class="fas fa-rotate"></i> Refresh
              </button>
              <button @click="showAIRecommendationsPanel = false"
                      class="px-4 py-2 bg-teal-500 text-white rounded-lg hover:bg-teal-600 transition-colors">
                Close
              </button>
            </div>
          </div>
        </div>
      </Teleport>

      <!-- AI Auto-fill Modal -->
      <Teleport to="body">
        <div v-if="showAutoFillModal" class="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
          <div class="bg-white rounded-2xl shadow-2xl w-full max-w-lg max-h-[80vh] overflow-hidden">
            <div class="p-6 border-b border-gray-100">
              <div class="flex items-center justify-between">
                <div class="flex items-center gap-3">
                  <div class="w-10 h-10 bg-gradient-to-br from-purple-500 to-pink-500 rounded-xl flex items-center justify-center">
                    <i class="fas fa-pen-fancy text-white"></i>
                  </div>
                  <div>
                    <h3 class="text-lg font-semibold text-gray-900">AI Auto-fill</h3>
                    <p class="text-sm text-gray-500">Suggested request details</p>
                  </div>
                </div>
                <button @click="showAutoFillModal = false" class="p-2 hover:bg-gray-100 rounded-lg transition-colors">
                  <i class="fas fa-times text-gray-400"></i>
                </button>
              </div>
            </div>

            <div class="p-6 overflow-y-auto max-h-[60vh]">
              <AILoadingIndicator v-if="isGeneratingAutoFill" message="Generating form suggestions..." />

              <div v-else-if="autoFillSuggestions" class="space-y-4">
                <div>
                  <label class="text-xs font-semibold text-gray-500 uppercase">Subject</label>
                  <p class="mt-1 p-3 bg-gray-50 rounded-lg text-gray-900">{{ autoFillSuggestions.subject }}</p>
                </div>

                <div>
                  <label class="text-xs font-semibold text-gray-500 uppercase">Description</label>
                  <p class="mt-1 p-3 bg-gray-50 rounded-lg text-gray-700 text-sm">{{ autoFillSuggestions.description }}</p>
                </div>

                <div>
                  <label class="text-xs font-semibold text-gray-500 uppercase">Priority</label>
                  <p class="mt-1 px-3 py-1 bg-amber-100 text-amber-700 rounded-full text-sm inline-block capitalize">{{ autoFillSuggestions.priority }}</p>
                </div>

                <div>
                  <label class="text-xs font-semibold text-gray-500 uppercase">Additional Information</label>
                  <ul class="mt-2 space-y-2">
                    <li v-for="(info, idx) in autoFillSuggestions.additionalInfo" :key="idx"
                        class="flex items-start gap-2 text-sm text-gray-600">
                      <i class="fas fa-check-circle text-purple-500 mt-0.5"></i>
                      {{ info }}
                    </li>
                  </ul>
                </div>
              </div>
            </div>

            <div class="p-4 border-t border-gray-100 flex justify-end gap-3">
              <button @click="generateAutoFill"
                      class="px-4 py-2 text-purple-600 hover:bg-purple-50 rounded-lg transition-colors flex items-center gap-2">
                <i class="fas fa-rotate"></i> Regenerate
              </button>
              <button @click="applyAutoFill"
                      class="px-4 py-2 bg-purple-500 text-white rounded-lg hover:bg-purple-600 transition-colors flex items-center gap-2">
                <i class="fas fa-check"></i> Apply
              </button>
            </div>
          </div>
        </div>
      </Teleport>
    </template>
  </div>
</template>

<style scoped>
/* Hero Gradient */
.hero-gradient {
  background: linear-gradient(135deg, #0d9488 0%, #14b8a6 50%, #10b981 100%);
}

/* Stats Top Right */
.stats-top-right {
  position: absolute;
  top: 1rem;
  right: 2rem;
  display: flex;
  gap: 0.75rem;
  z-index: 10;
}

.stat-card-square {
  background: rgba(255, 255, 255, 0.15);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.2);
  border-radius: 1rem;
  padding: 0.75rem 1rem;
  min-width: 90px;
  text-align: center;
  transition: all 0.3s ease;
}

.stat-card-square:hover {
  background: rgba(255, 255, 255, 0.25);
  transform: translateY(-2px);
}

.stat-icon-box {
  width: 2rem;
  height: 2rem;
  background: rgba(255, 255, 255, 0.2);
  border-radius: 0.5rem;
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 0 auto 0.5rem;
  color: white;
  font-size: 0.875rem;
}

.stat-value-mini {
  font-size: 1.25rem;
  font-weight: 700;
  color: white;
  line-height: 1;
}

.stat-label-mini {
  font-size: 0.65rem;
  color: rgba(255, 255, 255, 0.8);
  margin-top: 0.25rem;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

/* Decorative Circle Animations */
.circle-drift-1 {
  animation: drift1 20s ease-in-out infinite;
}

.circle-drift-2 {
  animation: drift2 25s ease-in-out infinite;
}

.circle-drift-3 {
  animation: drift3 15s ease-in-out infinite;
}

@keyframes drift1 {
  0%, 100% { transform: translate(0, 0) scale(1); }
  25% { transform: translate(-30px, 20px) scale(1.05); }
  50% { transform: translate(-20px, -30px) scale(0.95); }
  75% { transform: translate(20px, -20px) scale(1.02); }
}

@keyframes drift2 {
  0%, 100% { transform: translate(0, 0) scale(1); }
  33% { transform: translate(40px, -20px) scale(1.1); }
  66% { transform: translate(-30px, 30px) scale(0.9); }
}

@keyframes drift3 {
  0%, 100% { transform: translate(0, 0) rotate(0deg); }
  50% { transform: translate(30px, 30px) rotate(180deg); }
}

/* Icon soft style for this page */
.icon-soft {
  background: linear-gradient(135deg, #f0fdfa, #ccfbf1);
  color: #0d9488;
}

/* Animate fadeIn for FAQ answers */
.animate-fadeIn {
  animation: fadeIn 0.3s ease-out;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(-5px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

/* Badge styles */
.badge {
  display: inline-flex;
  align-items: center;
  padding: 0.25rem 0.75rem;
  font-size: 0.75rem;
  font-weight: 500;
  border-radius: 9999px;
}

.badge-info {
  background-color: #dbeafe;
  color: #1e40af;
}

.badge-warning {
  background-color: #fef3c7;
  color: #92400e;
}

.badge-success {
  background-color: #d1fae5;
  color: #065f46;
}

/* Button styles */
.btn {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  padding: 0.625rem 1.25rem;
  font-size: 0.875rem;
  font-weight: 500;
  border-radius: 0.75rem;
  transition: all 0.2s;
  cursor: pointer;
}

.btn-secondary {
  background-color: #f3f4f6;
  color: #374151;
  border: 1px solid #e5e7eb;
}

.btn-secondary:hover {
  background-color: #e5e7eb;
}

/* Input styles */
.input {
  width: 100%;
  padding: 0.75rem 1rem;
  border: 1px solid #e5e7eb;
  border-radius: 0.75rem;
  font-size: 0.875rem;
  transition: all 0.2s;
  background-color: white;
}

.input:focus {
  outline: none;
  border-color: #14b8a6;
  box-shadow: 0 0 0 3px rgba(20, 184, 166, 0.1);
}

textarea.input {
  resize: vertical;
  min-height: 100px;
}

select.input {
  appearance: none;
  background-image: url("data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' fill='none' viewBox='0 0 20 20'%3e%3cpath stroke='%236b7280' stroke-linecap='round' stroke-linejoin='round' stroke-width='1.5' d='M6 8l4 4 4-4'/%3e%3c/svg%3e");
  background-position: right 0.5rem center;
  background-repeat: no-repeat;
  background-size: 1.5em 1.5em;
  padding-right: 2.5rem;
}

/* AI Feature Styles */
.ai-action-btn {
  transition: all 0.2s ease;
}

.ai-action-btn:hover {
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.2);
}

.ai-recommendation-card {
  transition: all 0.2s ease;
}

.ai-recommendation-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.1);
}

.ai-pulse {
  animation: aiPulse 2s ease-in-out infinite;
}

@keyframes aiPulse {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.7; }
}

.ai-gradient-text {
  background: linear-gradient(135deg, #14b8a6 0%, #10b981 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}
</style>
