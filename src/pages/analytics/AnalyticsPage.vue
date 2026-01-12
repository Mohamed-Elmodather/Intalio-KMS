<script setup lang="ts">
import { ref, computed } from 'vue'

// State
const isLoading = ref(false)
const selectedPeriod = ref('30d')
const selectedMetric = ref('views')

// Period options
const periodOptions = [
  { value: '7d', label: 'Last 7 Days' },
  { value: '30d', label: 'Last 30 Days' },
  { value: '90d', label: 'Last 90 Days' },
  { value: '12m', label: 'Last 12 Months' }
]

// Stats data
const stats = ref({
  totalViews: 124589,
  viewsGrowth: 12.5,
  engagementRate: 78.4,
  engagementGrowth: 5.2,
  activeUsers: 3456,
  usersGrowth: 8.7,
  contentPublished: 245,
  contentGrowth: 15.3
})

// KPI Cards data
const kpiCards = ref([
  {
    id: 'views',
    title: 'Total Views',
    value: '124.5K',
    change: '+12.5%',
    changeType: 'positive',
    icon: 'fas fa-eye',
    color: 'teal',
    sparkline: [30, 40, 35, 50, 49, 60, 70, 91, 85, 95, 100, 110]
  },
  {
    id: 'engagement',
    title: 'Engagement Rate',
    value: '78.4%',
    change: '+5.2%',
    changeType: 'positive',
    icon: 'fas fa-heart',
    color: 'pink',
    sparkline: [60, 65, 70, 68, 72, 75, 73, 78, 76, 80, 78, 82]
  },
  {
    id: 'users',
    title: 'Active Users',
    value: '3,456',
    change: '+8.7%',
    changeType: 'positive',
    icon: 'fas fa-users',
    color: 'blue',
    sparkline: [20, 25, 30, 35, 32, 40, 45, 50, 48, 55, 60, 65]
  },
  {
    id: 'content',
    title: 'Content Published',
    value: '245',
    change: '+15.3%',
    changeType: 'positive',
    icon: 'fas fa-file-alt',
    color: 'purple',
    sparkline: [10, 15, 12, 18, 20, 25, 22, 30, 28, 35, 40, 45]
  }
])

// Traffic chart data (simulated)
const trafficData = ref({
  labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
  datasets: [
    { label: 'Page Views', data: [12000, 15000, 18000, 16000, 21000, 19000, 24000, 28000, 25000, 30000, 32000, 35000] },
    { label: 'Unique Visitors', data: [8000, 10000, 12000, 11000, 14000, 13000, 16000, 19000, 17000, 20000, 22000, 24000] }
  ]
})

// Content performance data
const contentPerformance = ref([
  { id: 1, title: 'Tournament Regulations 2027', type: 'Document', views: 12450, downloads: 3240, rating: 4.8 },
  { id: 2, title: 'Opening Ceremony Guide', type: 'Article', views: 9876, downloads: 0, rating: 4.9 },
  { id: 3, title: 'Stadium Safety Protocols', type: 'Document', views: 8543, downloads: 2150, rating: 4.7 },
  { id: 4, title: 'Team Registration Process', type: 'Video', views: 7234, downloads: 0, rating: 4.6 },
  { id: 5, title: 'Media Accreditation FAQ', type: 'Article', views: 6123, downloads: 0, rating: 4.5 }
])

// User activity data
const userActivity = ref([
  { id: 1, user: 'Ahmed Hassan', action: 'Downloaded', item: 'Tournament Regulations', time: '2 mins ago', avatar: null },
  { id: 2, user: 'Sarah Chen', action: 'Viewed', item: 'Opening Ceremony Guide', time: '5 mins ago', avatar: null },
  { id: 3, user: 'Mohammed Ali', action: 'Shared', item: 'Stadium Safety Protocols', time: '12 mins ago', avatar: null },
  { id: 4, user: 'Emily Johnson', action: 'Commented on', item: 'Team Registration Process', time: '18 mins ago', avatar: null },
  { id: 5, user: 'Carlos Rodriguez', action: 'Uploaded', item: 'New Media Assets', time: '25 mins ago', avatar: null }
])

// Category breakdown
const categoryBreakdown = ref([
  { name: 'Documents', value: 45, color: '#14b8a6' },
  { name: 'Articles', value: 28, color: '#8b5cf6' },
  { name: 'Media', value: 15, color: '#f59e0b' },
  { name: 'Events', value: 12, color: '#3b82f6' }
])

// Reports list
const reports = ref([
  { id: 1, name: 'Monthly Analytics Report', type: 'PDF', date: 'Jan 2027', size: '2.4 MB', icon: 'fas fa-file-pdf' },
  { id: 2, name: 'User Engagement Summary', type: 'Excel', date: 'Jan 2027', size: '1.8 MB', icon: 'fas fa-file-excel' },
  { id: 3, name: 'Content Performance Report', type: 'PDF', date: 'Dec 2026', size: '3.1 MB', icon: 'fas fa-file-pdf' },
  { id: 4, name: 'Traffic Analysis Q4', type: 'PDF', date: 'Dec 2026', size: '2.7 MB', icon: 'fas fa-file-pdf' }
])

// Computed
const totalCategoryValue = computed(() => {
  return categoryBreakdown.value.reduce((sum, cat) => sum + cat.value, 0)
})

// Methods
function formatNumber(num: number): string {
  if (num >= 1000000) return (num / 1000000).toFixed(1) + 'M'
  if (num >= 1000) return (num / 1000).toFixed(1) + 'K'
  return num.toString()
}

function getColorClasses(color: string) {
  const colors: Record<string, { bg: string; icon: string; text: string; shadow: string }> = {
    teal: { bg: 'from-teal-500 to-teal-600', icon: 'bg-teal-100', text: 'text-teal-600', shadow: 'shadow-teal-200' },
    pink: { bg: 'from-pink-500 to-pink-600', icon: 'bg-pink-100', text: 'text-pink-600', shadow: 'shadow-pink-200' },
    blue: { bg: 'from-blue-500 to-blue-600', icon: 'bg-blue-100', text: 'text-blue-600', shadow: 'shadow-blue-200' },
    purple: { bg: 'from-purple-500 to-purple-600', icon: 'bg-purple-100', text: 'text-purple-600', shadow: 'shadow-purple-200' }
  }
  return colors[color] || colors.teal
}

function downloadReport(report: any) {
  alert(`Downloading ${report.name}...`)
}

function exportData(format: string) {
  alert(`Exporting data as ${format}...`)
}
</script>

<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Hero Section -->
    <div class="hero-gradient relative overflow-hidden">
      <!-- Decorative elements with animations -->
      <div class="circle-drift-1 absolute top-0 right-0 w-96 h-96 bg-white/5 rounded-full"></div>
      <div class="circle-drift-2 absolute bottom-0 left-0 w-64 h-64 bg-white/5 rounded-full"></div>
      <div class="circle-drift-3 absolute top-1/2 right-1/4 w-32 h-32 bg-white/10 rounded-full"></div>

      <!-- Stats - Absolute Top Right -->
      <div class="stats-top-right">
        <div class="stat-card-square">
          <div class="stat-icon-box">
            <i class="fas fa-eye"></i>
          </div>
          <p class="stat-value-mini">{{ formatNumber(stats.totalViews) }}</p>
          <p class="stat-label-mini">Views</p>
        </div>
        <div class="stat-card-square">
          <div class="stat-icon-box">
            <i class="fas fa-chart-line"></i>
          </div>
          <p class="stat-value-mini">{{ stats.engagementRate }}%</p>
          <p class="stat-label-mini">Engagement</p>
        </div>
        <div class="stat-card-square">
          <div class="stat-icon-box">
            <i class="fas fa-users"></i>
          </div>
          <p class="stat-value-mini">{{ formatNumber(stats.activeUsers) }}</p>
          <p class="stat-label-mini">Users</p>
        </div>
        <div class="stat-card-square">
          <div class="stat-icon-box">
            <i class="fas fa-file-alt"></i>
          </div>
          <p class="stat-value-mini">{{ stats.contentPublished }}</p>
          <p class="stat-label-mini">Content</p>
        </div>
      </div>

      <div class="relative px-8 py-8">
        <div class="px-3 py-1 bg-white/20 backdrop-blur-sm rounded-full text-white text-xs font-semibold inline-flex items-center gap-2 mb-4">
          <i class="fas fa-chart-pie"></i>
          AFC Asian Cup 2027
        </div>

        <h1 class="text-3xl font-bold text-white mb-2">Analytics & Reporting</h1>
        <p class="text-teal-100 max-w-lg">Track performance metrics, user engagement, and content analytics across the platform.</p>

        <div class="flex flex-wrap gap-3 mt-6">
          <button @click="exportData('PDF')" class="px-5 py-2.5 bg-white text-teal-600 rounded-xl font-semibold text-sm flex items-center gap-2 hover:bg-teal-50 transition-all shadow-lg">
            <i class="fas fa-download"></i>
            Export Report
          </button>
          <button class="px-5 py-2.5 bg-white/20 backdrop-blur-sm border border-white/30 text-white rounded-xl font-semibold text-sm hover:bg-white/30 transition-all flex items-center gap-2">
            <i class="fas fa-calendar-alt"></i>
            Schedule Report
          </button>
        </div>
      </div>
    </div>

    <!-- Main Content -->
    <div class="px-8 py-6 space-y-6">
      <!-- Period Selector -->
      <div class="flex items-center justify-between">
        <div class="flex items-center gap-2">
          <span class="text-sm font-medium text-gray-600">Time Period:</span>
          <div class="flex items-center bg-white border border-gray-200 rounded-lg p-1">
            <button
              v-for="period in periodOptions"
              :key="period.value"
              @click="selectedPeriod = period.value"
              :class="[
                'px-3 py-1.5 rounded-md text-xs font-medium transition-all',
                selectedPeriod === period.value ? 'bg-teal-500 text-white shadow-sm' : 'text-gray-600 hover:bg-gray-100'
              ]"
            >
              {{ period.label }}
            </button>
          </div>
        </div>
        <div class="flex items-center gap-2">
          <button class="px-3 py-1.5 bg-white border border-gray-200 rounded-lg text-sm text-gray-600 hover:bg-gray-50 flex items-center gap-2">
            <i class="fas fa-filter text-xs"></i>
            Filter
          </button>
          <button class="px-3 py-1.5 bg-white border border-gray-200 rounded-lg text-sm text-gray-600 hover:bg-gray-50 flex items-center gap-2">
            <i class="fas fa-sync-alt text-xs"></i>
            Refresh
          </button>
        </div>
      </div>

      <!-- KPI Cards -->
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        <div
          v-for="kpi in kpiCards"
          :key="kpi.id"
          class="bg-white rounded-2xl p-5 shadow-sm border border-gray-100 hover:shadow-lg hover:border-teal-200 transition-all cursor-pointer group"
        >
          <div class="flex items-start justify-between mb-4">
            <div :class="['w-12 h-12 rounded-xl bg-gradient-to-br flex items-center justify-center shadow-lg', getColorClasses(kpi.color).bg, getColorClasses(kpi.color).shadow]">
              <i :class="[kpi.icon, 'text-white text-lg']"></i>
            </div>
            <span :class="[
              'px-2 py-1 rounded-full text-xs font-semibold',
              kpi.changeType === 'positive' ? 'bg-green-100 text-green-700' : 'bg-red-100 text-red-700'
            ]">
              {{ kpi.change }}
            </span>
          </div>
          <p class="text-2xl font-bold text-gray-900 mb-1">{{ kpi.value }}</p>
          <p class="text-sm text-gray-500">{{ kpi.title }}</p>

          <!-- Mini Sparkline -->
          <div class="mt-4 h-10 flex items-end gap-0.5">
            <div
              v-for="(val, idx) in kpi.sparkline"
              :key="idx"
              :class="['flex-1 rounded-t transition-all group-hover:opacity-100', getColorClasses(kpi.color).text.replace('text-', 'bg-')]"
              :style="{ height: (val / Math.max(...kpi.sparkline) * 100) + '%', opacity: 0.3 + (idx / kpi.sparkline.length) * 0.7 }"
            ></div>
          </div>
        </div>
      </div>

      <!-- Charts Row -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <!-- Traffic Overview Chart -->
        <div class="lg:col-span-2 bg-white rounded-2xl shadow-sm border border-gray-100 overflow-hidden">
          <div class="px-5 py-4 border-b border-gray-100 flex items-center justify-between">
            <h2 class="text-lg font-bold text-gray-900 flex items-center gap-3">
              <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-teal-500 to-teal-600 flex items-center justify-center shadow-lg shadow-teal-200">
                <i class="fas fa-chart-area text-white text-sm"></i>
              </div>
              <div>
                <span class="block">Traffic Overview</span>
                <span class="text-xs font-medium text-gray-500">Page views and unique visitors</span>
              </div>
            </h2>
            <div class="flex items-center gap-4">
              <div class="flex items-center gap-2">
                <span class="w-3 h-3 rounded-full bg-teal-500"></span>
                <span class="text-xs text-gray-500">Page Views</span>
              </div>
              <div class="flex items-center gap-2">
                <span class="w-3 h-3 rounded-full bg-purple-500"></span>
                <span class="text-xs text-gray-500">Unique Visitors</span>
              </div>
            </div>
          </div>
          <div class="p-5">
            <!-- Chart Placeholder -->
            <div class="h-64 flex items-end justify-between gap-2 px-4">
              <div v-for="(val, idx) in trafficData.datasets[0].data" :key="idx" class="flex-1 flex flex-col items-center gap-1">
                <div class="w-full flex flex-col gap-1">
                  <div
                    class="w-full bg-teal-500 rounded-t transition-all hover:bg-teal-600"
                    :style="{ height: (val / Math.max(...trafficData.datasets[0].data) * 180) + 'px' }"
                  ></div>
                  <div
                    class="w-full bg-purple-400 rounded-t transition-all hover:bg-purple-500"
                    :style="{ height: (trafficData.datasets[1].data[idx] / Math.max(...trafficData.datasets[0].data) * 180) + 'px' }"
                  ></div>
                </div>
                <span class="text-[10px] text-gray-400 mt-2">{{ trafficData.labels[idx] }}</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Category Breakdown -->
        <div class="bg-white rounded-2xl shadow-sm border border-gray-100 overflow-hidden">
          <div class="px-5 py-4 border-b border-gray-100">
            <h2 class="text-lg font-bold text-gray-900 flex items-center gap-3">
              <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-purple-500 to-purple-600 flex items-center justify-center shadow-lg shadow-purple-200">
                <i class="fas fa-chart-pie text-white text-sm"></i>
              </div>
              <div>
                <span class="block">Content by Category</span>
                <span class="text-xs font-medium text-gray-500">Distribution overview</span>
              </div>
            </h2>
          </div>
          <div class="p-5">
            <!-- Donut Chart Placeholder -->
            <div class="relative w-40 h-40 mx-auto mb-6">
              <svg viewBox="0 0 100 100" class="w-full h-full -rotate-90">
                <circle cx="50" cy="50" r="40" fill="none" stroke="#f3f4f6" stroke-width="12" />
                <circle
                  v-for="(cat, idx) in categoryBreakdown"
                  :key="cat.name"
                  cx="50"
                  cy="50"
                  r="40"
                  fill="none"
                  :stroke="cat.color"
                  stroke-width="12"
                  :stroke-dasharray="(cat.value / totalCategoryValue * 251.2) + ' 251.2'"
                  :stroke-dashoffset="-categoryBreakdown.slice(0, idx).reduce((sum, c) => sum + (c.value / totalCategoryValue * 251.2), 0)"
                  class="transition-all duration-500"
                />
              </svg>
              <div class="absolute inset-0 flex items-center justify-center flex-col">
                <span class="text-2xl font-bold text-gray-900">{{ totalCategoryValue }}</span>
                <span class="text-xs text-gray-500">Total</span>
              </div>
            </div>
            <!-- Legend -->
            <div class="space-y-2">
              <div v-for="cat in categoryBreakdown" :key="cat.name" class="flex items-center justify-between">
                <div class="flex items-center gap-2">
                  <span class="w-3 h-3 rounded-full" :style="{ backgroundColor: cat.color }"></span>
                  <span class="text-sm text-gray-700">{{ cat.name }}</span>
                </div>
                <span class="text-sm font-semibold text-gray-900">{{ cat.value }}%</span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Content Performance & Activity -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <!-- Top Content -->
        <div class="bg-white rounded-2xl shadow-sm border border-gray-100 overflow-hidden">
          <div class="px-5 py-4 border-b border-gray-100 flex items-center justify-between">
            <h2 class="text-lg font-bold text-gray-900 flex items-center gap-3">
              <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-amber-400 to-amber-500 flex items-center justify-center shadow-lg shadow-amber-200">
                <i class="fas fa-trophy text-white text-sm"></i>
              </div>
              <div>
                <span class="block">Top Performing Content</span>
                <span class="text-xs font-medium text-gray-500">By views and engagement</span>
              </div>
            </h2>
            <button class="text-sm text-teal-600 hover:text-teal-700 font-medium flex items-center gap-1">
              View All
              <i class="fas fa-arrow-right text-xs"></i>
            </button>
          </div>
          <div class="divide-y divide-gray-100">
            <div
              v-for="(item, idx) in contentPerformance"
              :key="item.id"
              class="px-5 py-3 flex items-center gap-4 hover:bg-gray-50 transition-colors cursor-pointer"
            >
              <div class="w-8 h-8 rounded-lg bg-gray-100 flex items-center justify-center text-sm font-bold text-gray-500">
                {{ idx + 1 }}
              </div>
              <div class="flex-1 min-w-0">
                <p class="text-sm font-medium text-gray-900 truncate">{{ item.title }}</p>
                <p class="text-xs text-gray-500">{{ item.type }}</p>
              </div>
              <div class="text-right">
                <p class="text-sm font-semibold text-gray-900">{{ formatNumber(item.views) }}</p>
                <p class="text-xs text-gray-500">views</p>
              </div>
              <div class="flex items-center gap-1">
                <i class="fas fa-star text-amber-400 text-xs"></i>
                <span class="text-sm font-medium text-gray-700">{{ item.rating }}</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Recent Activity -->
        <div class="bg-white rounded-2xl shadow-sm border border-gray-100 overflow-hidden">
          <div class="px-5 py-4 border-b border-gray-100 flex items-center justify-between">
            <h2 class="text-lg font-bold text-gray-900 flex items-center gap-3">
              <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-blue-500 to-blue-600 flex items-center justify-center shadow-lg shadow-blue-200">
                <i class="fas fa-history text-white text-sm"></i>
              </div>
              <div>
                <span class="block">Recent Activity</span>
                <span class="text-xs font-medium text-gray-500">User interactions</span>
              </div>
            </h2>
            <button class="text-sm text-teal-600 hover:text-teal-700 font-medium flex items-center gap-1">
              View All
              <i class="fas fa-arrow-right text-xs"></i>
            </button>
          </div>
          <div class="divide-y divide-gray-100">
            <div
              v-for="activity in userActivity"
              :key="activity.id"
              class="px-5 py-3 flex items-center gap-4 hover:bg-gray-50 transition-colors"
            >
              <div class="w-10 h-10 rounded-full bg-gradient-to-br from-teal-100 to-teal-200 flex items-center justify-center">
                <span class="text-sm font-semibold text-teal-700">{{ activity.user.split(' ').map(n => n[0]).join('') }}</span>
              </div>
              <div class="flex-1 min-w-0">
                <p class="text-sm text-gray-900">
                  <span class="font-medium">{{ activity.user }}</span>
                  <span class="text-gray-500"> {{ activity.action }} </span>
                  <span class="font-medium text-teal-600">{{ activity.item }}</span>
                </p>
                <p class="text-xs text-gray-400">{{ activity.time }}</p>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Reports Section -->
      <div class="bg-white rounded-2xl shadow-sm border border-gray-100 overflow-hidden">
        <div class="px-5 py-4 border-b border-gray-100 flex items-center justify-between">
          <h2 class="text-lg font-bold text-gray-900 flex items-center gap-3">
            <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-green-500 to-green-600 flex items-center justify-center shadow-lg shadow-green-200">
              <i class="fas fa-file-export text-white text-sm"></i>
            </div>
            <div>
              <span class="block">Generated Reports</span>
              <span class="text-xs font-medium text-gray-500">Download past reports</span>
            </div>
          </h2>
          <button class="px-4 py-2 bg-gradient-to-r from-teal-500 to-teal-600 text-white rounded-lg text-sm font-medium hover:from-teal-600 hover:to-teal-700 transition-all flex items-center gap-2 shadow-sm shadow-teal-200">
            <i class="fas fa-plus"></i>
            Generate New
          </button>
        </div>
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4 p-5">
          <div
            v-for="report in reports"
            :key="report.id"
            @click="downloadReport(report)"
            class="p-4 rounded-xl bg-gray-50 border border-gray-100 hover:border-teal-200 hover:bg-teal-50 cursor-pointer transition-all group"
          >
            <div class="flex items-start gap-3">
              <div :class="[
                'w-10 h-10 rounded-lg flex items-center justify-center transition-transform group-hover:scale-110',
                report.type === 'PDF' ? 'bg-red-100 text-red-600' : 'bg-green-100 text-green-600'
              ]">
                <i :class="[report.icon, 'text-lg']"></i>
              </div>
              <div class="flex-1 min-w-0">
                <p class="text-sm font-medium text-gray-900 truncate group-hover:text-teal-600 transition-colors">{{ report.name }}</p>
                <p class="text-xs text-gray-500 mt-1">{{ report.date }} • {{ report.size }}</p>
              </div>
            </div>
            <button class="mt-3 w-full px-3 py-1.5 bg-white border border-gray-200 rounded-lg text-xs font-medium text-gray-600 hover:bg-teal-500 hover:text-white hover:border-teal-500 transition-all flex items-center justify-center gap-2">
              <i class="fas fa-download"></i>
              Download
            </button>
          </div>
        </div>
      </div>
    </div>
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
</style>
