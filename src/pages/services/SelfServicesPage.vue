<script setup lang="ts">
import { ref, computed } from 'vue'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'
import { UnifiedPageLayout } from '@/components/unified'
import type { PageStat, PageAction } from '@/components/unified'

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

const pageStats = computed<PageStat[]>(() => [
  { id: 'open', label: 'Open Requests', value: openRequestsCount.value, icon: 'fas fa-clock', color: 'teal' },
  { id: 'completed', label: 'Completed', value: completedRequestsCount.value, icon: 'fas fa-check-circle', color: 'green' },
  { id: 'resolution', label: 'Avg. Resolution', value: `${avgResolutionTime.value}h`, icon: 'fas fa-bolt', color: 'orange' },
  { id: 'services', label: 'Services', value: totalServices.value, icon: 'fas fa-th-large', color: 'blue' }
])

// Page actions
const pageActions: PageAction[] = [
  { id: 'new-request', label: 'New Request', icon: 'fas fa-plus', variant: 'primary' }
]

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
function handleActionClick(actionId: string) {
  if (actionId === 'new-request') {
    showNewRequest.value = true
  }
}

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
  alert(`Viewing request ${request.id}: ${request.title}`)
}

function askAI(query: string) {
  if (query) {
    // Show AI chat or handle query
    console.log('AI Query:', query)
    aiQuery.value = ''
  }
}
</script>

<template>
  <div>
    <!-- Loading State -->
    <div v-if="isLoading" class="flex items-center justify-center min-h-[400px]">
      <LoadingSpinner size="lg" text="Loading services..." />
    </div>

    <template v-else>
      <!-- Unified Page Layout with Hero -->
      <UnifiedPageLayout
        title="Self-Service Portal"
        title-highlight="Self-Service"
        subtitle="Request services, track progress, and get help"
        icon="fas fa-concierge-bell"
        :stats="pageStats"
        :actions="pageActions"
        hero-variant="default"
        @action-click="handleActionClick"
      >
        <!-- Main Content -->
        <div class="grid grid-cols-1 xl:grid-cols-3 gap-6">
          <!-- Service Catalog -->
          <div class="xl:col-span-2">
            <div class="card-animated">
              <div class="p-5 border-b border-gray-100">
                <div class="flex flex-col md:flex-row md:items-center justify-between gap-4">
                  <h2 class="text-lg font-semibold text-gray-900">Service Catalog</h2>
                  <div class="flex flex-wrap gap-2">
                    <button v-for="cat in categories" :key="cat.id"
                            @click="filterCategory = cat.id"
                            :class="['px-4 py-2 rounded-xl text-sm font-medium transition-all ripple-effect',
                                     filterCategory === cat.id ? 'btn-vibrant' : 'bg-teal-50 text-gray-700 hover:bg-teal-100']">
                      {{ cat.label }}
                    </button>
                  </div>
                </div>
              </div>

              <div class="p-5 grid grid-cols-1 md:grid-cols-2 gap-4">
                <div v-for="service in filteredServices" :key="service.id"
                     @click="selectService(service)"
                     class="list-item-animated p-4 rounded-xl border-2 border-transparent bg-teal-50/50 hover:border-teal-300 hover:bg-teal-100/50 cursor-pointer transition-all group ripple-effect">
                  <div class="flex items-start gap-4">
                    <div :class="['icon-soft w-12 h-12 rounded-xl flex items-center justify-center flex-shrink-0 hover-scale', service.iconBg]">
                      <i :class="[service.icon, service.iconColor, 'text-xl']"></i>
                    </div>
                    <div class="flex-1 min-w-0">
                      <h3 class="font-semibold text-gray-900 group-hover:text-gray-700">{{ service.name }}</h3>
                      <p class="text-sm text-gray-500 mt-1">{{ service.description }}</p>
                      <div class="flex items-center gap-3 mt-2">
                        <span class="text-xs text-gray-400"><i class="fas fa-clock mr-1"></i>{{ service.eta }}</span>
                        <span :class="['text-xs px-2 py-0.5 rounded-full', service.priorityClass]">{{ service.priority }}</span>
                      </div>
                    </div>
                    <i class="fas fa-chevron-right text-gray-300 group-hover:text-gray-500 transition-colors"></i>
                  </div>
                </div>
              </div>
            </div>

            <!-- My Requests -->
            <div class="card-animated mt-6">
              <div class="p-5 border-b border-gray-100 flex items-center justify-between">
                <h2 class="text-lg font-semibold text-gray-900">My Requests</h2>
                <div class="flex items-center gap-2">
                  <select v-model="requestFilter" class="input text-sm py-2">
                    <option value="all">All Status</option>
                    <option value="pending">Pending</option>
                    <option value="in-progress">In Progress</option>
                    <option value="completed">Completed</option>
                  </select>
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
            <div class="card-animated">
              <div class="flex items-center gap-3 mb-4">
                <div class="icon-vibrant w-10 h-10 rounded-xl bg-gradient-to-br from-teal-400 to-teal-600 flex items-center justify-center hover-scale">
                  <i class="fas fa-robot text-white"></i>
                </div>
                <div>
                  <h3 class="font-semibold text-gray-900">AI Assistant</h3>
                  <p class="text-xs text-gray-500">Get instant help</p>
                </div>
              </div>

              <div class="space-y-2 mb-4">
                <button v-for="(query, idx) in quickQueries" :key="idx"
                        @click="askAI(query)"
                        class="list-item-animated w-full p-3 text-left text-sm rounded-xl bg-teal-50 hover:bg-teal-100 text-gray-700 transition-colors ripple-effect">
                  <i class="fas fa-comment-dots text-gray-400 mr-2"></i>{{ query }}
                </button>
              </div>

              <div class="relative">
                <input type="text" v-model="aiQuery" @keydown.enter="askAI(aiQuery)"
                       placeholder="Ask anything..." class="input pr-10">
                <button @click="askAI(aiQuery)" class="absolute right-2 top-1/2 -translate-y-1/2 p-1.5 rounded-lg hover:bg-teal-50 text-gray-500 ripple-effect hover-scale">
                  <i class="fas fa-paper-plane"></i>
                </button>
              </div>
            </div>

            <!-- FAQ -->
            <div class="card-animated">
              <h3 class="font-semibold text-gray-900 mb-4">
                <i class="fas fa-question-circle text-gray-500 mr-2 hover-scale"></i>Frequently Asked
              </h3>
              <div class="space-y-3">
                <div v-for="(faq, idx) in faqs" :key="idx" class="list-item-animated border-b border-gray-100 last:border-0 pb-3 last:pb-0">
                  <button @click="faq.open = !faq.open" class="w-full text-left flex items-center justify-between ripple-effect">
                    <span class="text-sm font-medium text-gray-800">{{ faq.question }}</span>
                    <i :class="['fas text-gray-400 transition-transform hover-scale', faq.open ? 'fa-chevron-up' : 'fa-chevron-down']"></i>
                  </button>
                  <p v-if="faq.open" class="text-sm text-gray-600 mt-2 animate-fadeIn">{{ faq.answer }}</p>
                </div>
              </div>
            </div>

            <!-- Contact Support -->
            <div class="card-animated">
              <h3 class="font-semibold text-gray-900 mb-4">
                <i class="fas fa-headset text-gray-500 mr-2 hover-scale"></i>Need More Help?
              </h3>
              <div class="space-y-3">
                <a href="#" class="list-item-animated flex items-center gap-3 p-3 rounded-xl bg-teal-50 hover:bg-teal-100 transition-colors ripple-effect">
                  <div class="icon-soft w-8 h-8 rounded-lg bg-teal-100 flex items-center justify-center hover-scale">
                    <i class="fas fa-phone text-gray-600"></i>
                  </div>
                  <div>
                    <p class="text-sm font-medium text-gray-800">Call Support</p>
                    <p class="text-xs text-gray-500">+1 (555) 123-4567</p>
                  </div>
                </a>
                <a href="#" class="list-item-animated flex items-center gap-3 p-3 rounded-xl bg-teal-50 hover:bg-teal-100 transition-colors ripple-effect">
                  <div class="icon-soft w-8 h-8 rounded-lg bg-teal-100 flex items-center justify-center hover-scale">
                    <i class="fas fa-envelope text-gray-600"></i>
                  </div>
                  <div>
                    <p class="text-sm font-medium text-gray-800">Email Us</p>
                    <p class="text-xs text-gray-500">support@intalio.com</p>
                  </div>
                </a>
                <a href="#" class="list-item-animated flex items-center gap-3 p-3 rounded-xl bg-teal-50 hover:bg-teal-100 transition-colors ripple-effect">
                  <div class="icon-soft w-8 h-8 rounded-lg bg-teal-100 flex items-center justify-center hover-scale">
                    <i class="fas fa-comments text-gray-700"></i>
                  </div>
                  <div>
                    <p class="text-sm font-medium text-gray-800">Live Chat</p>
                    <p class="text-xs text-gray-500">Available 24/7</p>
                  </div>
                </a>
              </div>
            </div>
          </div>
        </div>
      </UnifiedPageLayout>

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
                <label class="block text-sm font-medium text-gray-700 mb-2">Description</label>
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
    </template>
  </div>
</template>

<style scoped>
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
</style>
