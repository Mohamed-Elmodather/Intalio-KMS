<script setup lang="ts">
import { ref, computed } from 'vue'

// State
const isLoading = ref(false)
const selectedPeriod = ref('30d')
const selectedMetric = ref('views')
const selectedDepartment = ref('all')
const selectedContentType = ref('all')

// Period options (Date ranges)
const periodOptions = [
  { value: '7d', label: '7 Days' },
  { value: '30d', label: '30 Days' },
  { value: '90d', label: 'Quarter' },
  { value: 'ytd', label: 'YTD' }
]

// Content type filters
const contentTypes = [
  { value: 'all', label: 'All' },
  { value: 'articles', label: 'Articles' },
  { value: 'courses', label: 'Courses' },
  { value: 'events', label: 'Events' }
]

// Department filters
const departments = [
  { id: 'all', name: 'All Departments' },
  { id: 'engineering', name: 'Engineering' },
  { id: 'marketing', name: 'Marketing' },
  { id: 'sales', name: 'Sales' },
  { id: 'hr', name: 'Human Resources' },
  { id: 'finance', name: 'Finance' },
  { id: 'operations', name: 'Operations' }
]

// Executive Stats (Hero)
const stats = ref({
  platformAdoption: 78,
  activeUsers: 842,
  totalContent: 1247,
  engagementScore: 8.4
})

// Tournament KPIs - Asia Cup Statistics
const kpiCards = ref([
  {
    id: 'team-score',
    title: 'Highest Team Score',
    value: '354',
    suffix: '/6',
    change: '',
    changeType: 'neutral',
    icon: 'fas fa-futbol',
    color: 'teal',
    sparkline: [280, 295, 310, 320, 298, 315, 340, 325, 354, 310, 305, 320]
  },
  {
    id: 'most-runs',
    title: 'Most Runs (India)',
    value: '2,847',
    suffix: '',
    change: '+12.3%',
    changeType: 'positive',
    icon: 'fas fa-chart-line',
    color: 'blue',
    sparkline: [2100, 2200, 2350, 2400, 2500, 2580, 2650, 2700, 2750, 2800, 2820, 2847]
  },
  {
    id: 'most-wickets',
    title: 'Most Wickets (Pakistan)',
    value: '198',
    suffix: '',
    change: '+8.7%',
    changeType: 'positive',
    icon: 'fas fa-bullseye',
    color: 'purple',
    sparkline: [150, 155, 160, 165, 170, 175, 180, 185, 188, 192, 195, 198]
  },
  {
    id: 'titles',
    title: 'Tournament Titles',
    value: '7',
    suffix: ' (India)',
    change: '',
    changeType: 'neutral',
    icon: 'fas fa-trophy',
    color: 'amber',
    sparkline: [1, 2, 3, 3, 4, 5, 5, 6, 6, 7, 7, 7]
  }
])

// Tournament Records (Weekly Highlights)
const tournamentRecords = ref([
  { id: 1, label: 'Highest Individual Score', value: '183', icon: 'fas fa-star', iconBg: 'bg-teal-50', iconColor: 'text-teal-600' },
  { id: 2, label: 'Best Bowling', value: '6/19', icon: 'fas fa-bullseye', iconBg: 'bg-blue-50', iconColor: 'text-blue-600' },
  { id: 3, label: 'Most Centuries', value: '18', icon: 'fas fa-medal', iconBg: 'bg-purple-50', iconColor: 'text-purple-600' },
  { id: 4, label: 'Most Sixes', value: '145', icon: 'fas fa-bolt', iconBg: 'bg-orange-50', iconColor: 'text-orange-600' }
])

// Tournament Insights
const tournamentInsights = ref([
  { id: 1, icon: 'fas fa-chart-line', text: '<strong>India leads</strong> with 7 Asia Cup titles, followed by Sri Lanka with 6.' },
  { id: 2, icon: 'fas fa-futbol', text: '<strong>Pakistan holds</strong> the record for highest team total - 354/6 vs Bangladesh (2023).' },
  { id: 3, icon: 'fas fa-trophy', text: '<strong>Virat Kohli</strong> is the highest run-scorer in Asia Cup history with 1,782 runs.' },
  { id: 4, icon: 'fas fa-star', text: '<strong>Dubai Stadium</strong> has hosted the most Asia Cup matches in the last decade.' }
])

// Team Statistics (Top Contributors/Teams)
const teamStatistics = ref([
  { id: 1, name: 'India', initials: 'IND', department: '7 Titles', contributions: 89, gradient: 'bg-gradient-to-br from-teal-500 to-teal-700' },
  { id: 2, name: 'Sri Lanka', initials: 'SL', department: '6 Titles', contributions: 86, gradient: 'bg-gradient-to-br from-blue-500 to-blue-700' },
  { id: 3, name: 'Pakistan', initials: 'PAK', department: '2 Titles', contributions: 84, gradient: 'bg-gradient-to-br from-purple-500 to-purple-700' },
  { id: 4, name: 'Bangladesh', initials: 'BAN', department: '0 Titles', contributions: 72, gradient: 'bg-gradient-to-br from-orange-500 to-orange-700' },
  { id: 5, name: 'Afghanistan', initials: 'AFG', department: '0 Titles', contributions: 68, gradient: 'bg-gradient-to-br from-pink-500 to-pink-700' }
])

// Department Stats
const departmentStats = ref([
  { id: 1, name: 'Engineering', initials: 'EN', employees: 156, adoption: 92, articles: 87, events: 12, gradient: 'bg-gradient-to-br from-teal-500 to-teal-700', color: '#14b8a6' },
  { id: 2, name: 'Marketing', initials: 'MK', employees: 48, adoption: 88, articles: 124, events: 18, gradient: 'bg-gradient-to-br from-blue-500 to-blue-700', color: '#3b82f6' },
  { id: 3, name: 'Sales', initials: 'SL', employees: 72, adoption: 76, articles: 45, events: 8, gradient: 'bg-gradient-to-br from-purple-500 to-purple-700', color: '#8b5cf6' },
  { id: 4, name: 'Human Resources', initials: 'HR', employees: 24, adoption: 95, articles: 156, events: 24, gradient: 'bg-gradient-to-br from-pink-500 to-pink-700', color: '#ec4899' },
  { id: 5, name: 'Finance', initials: 'FN', employees: 32, adoption: 58, articles: 28, events: 4, gradient: 'bg-gradient-to-br from-orange-500 to-orange-700', color: '#f97316' },
  { id: 6, name: 'Operations', initials: 'OP', employees: 64, adoption: 71, articles: 52, events: 10, gradient: 'bg-gradient-to-br from-emerald-500 to-emerald-700', color: '#10b981' }
])

// Traffic chart data (simulated)
const trafficData = ref({
  labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
  datasets: [
    { label: 'Page Views', data: [12000, 15000, 18000, 16000, 21000, 19000, 24000, 28000, 25000, 30000, 32000, 35000] },
    { label: 'Unique Visitors', data: [8000, 10000, 12000, 11000, 14000, 13000, 16000, 19000, 17000, 20000, 22000, 24000] }
  ]
})

// Top Performers - Asia Cup Stars
const topPerformers = ref([
  { id: 1, title: 'Virat Kohli', author: 'India', stats: '1,782 runs', icon: 'fas fa-user', iconBg: 'bg-amber-50', iconColor: 'text-amber-600', matches: '15 Matches', engagement: 98, completion: 89, color: '#f59e0b' },
  { id: 2, title: 'Babar Azam', author: 'Pakistan', stats: '1,456 runs', icon: 'fas fa-user', iconBg: 'bg-emerald-50', iconColor: 'text-emerald-600', matches: '14 Matches', engagement: 95, completion: 87, color: '#10b981' },
  { id: 3, title: 'Shaheen Afridi', author: 'Pakistan', stats: '42 wickets', icon: 'fas fa-user', iconBg: 'bg-red-50', iconColor: 'text-red-600', matches: '13 Matches', engagement: 92, completion: 85, color: '#ef4444' },
  { id: 4, title: 'Jasprit Bumrah', author: 'India', stats: '38 wickets', icon: 'fas fa-user', iconBg: 'bg-blue-50', iconColor: 'text-blue-600', matches: '12 Matches', engagement: 90, completion: 83, color: '#3b82f6' },
  { id: 5, title: 'Rashid Khan', author: 'Afghanistan', stats: '35 wickets', icon: 'fas fa-user', iconBg: 'bg-purple-50', iconColor: 'text-purple-600', matches: '11 Matches', engagement: 88, completion: 80, color: '#8b5cf6' }
])

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

// Department Participation breakdown
const categoryBreakdown = ref([
  { name: 'Engineering', value: 28, color: '#14b8a6' },
  { name: 'Marketing', value: 18, color: '#3b82f6' },
  { name: 'HR', value: 15, color: '#ec4899' },
  { name: 'Sales', value: 14, color: '#8b5cf6' },
  { name: 'Product', value: 13, color: '#06b6d4' },
  { name: 'Operations', value: 12, color: '#10b981' }
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
    purple: { bg: 'from-purple-500 to-purple-600', icon: 'bg-purple-100', text: 'text-purple-600', shadow: 'shadow-purple-200' },
    amber: { bg: 'from-amber-500 to-amber-600', icon: 'bg-amber-100', text: 'text-amber-600', shadow: 'shadow-amber-200' },
    green: { bg: 'from-green-500 to-green-600', icon: 'bg-green-100', text: 'text-green-600', shadow: 'shadow-green-200' },
    emerald: { bg: 'from-emerald-500 to-emerald-600', icon: 'bg-emerald-100', text: 'text-emerald-600', shadow: 'shadow-emerald-200' },
    red: { bg: 'from-red-500 to-red-600', icon: 'bg-red-100', text: 'text-red-600', shadow: 'shadow-red-200' },
    orange: { bg: 'from-orange-500 to-orange-600', icon: 'bg-orange-100', text: 'text-orange-600', shadow: 'shadow-orange-200' }
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
            <i class="fas fa-users"></i>
          </div>
          <p class="stat-value-mini">{{ stats.platformAdoption }}%</p>
          <p class="stat-label-mini">Adoption</p>
        </div>
        <div class="stat-card-square">
          <div class="stat-icon-box">
            <i class="fas fa-user-check"></i>
          </div>
          <p class="stat-value-mini">{{ stats.activeUsers }}</p>
          <p class="stat-label-mini">Active Users</p>
        </div>
        <div class="stat-card-square">
          <div class="stat-icon-box">
            <i class="fas fa-file-alt"></i>
          </div>
          <p class="stat-value-mini">{{ formatNumber(stats.totalContent) }}</p>
          <p class="stat-label-mini">Content</p>
        </div>
        <div class="stat-card-square">
          <div class="stat-icon-box">
            <i class="fas fa-chart-line"></i>
          </div>
          <p class="stat-value-mini">{{ stats.engagementScore }}/10</p>
          <p class="stat-label-mini">Engagement</p>
        </div>
      </div>

      <div class="relative px-8 py-8">
        <div class="px-3 py-1 bg-white/20 backdrop-blur-sm rounded-full text-white text-xs font-semibold inline-flex items-center gap-2 mb-4">
          <i class="fas fa-chart-pie"></i>
          AFC Asian Cup 2027
        </div>

        <h1 class="text-3xl font-bold text-white mb-2">Knowledge Hub Analytics</h1>
        <p class="text-teal-100 max-w-lg">Track platform adoption, content engagement, and employee participation across all departments.</p>

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

      <!-- Platform Health Metrics (KPI Cards) -->
      <div class="mb-4">
        <h2 class="text-lg font-bold text-gray-900 flex items-center gap-3 mb-4">
          <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-teal-500 to-teal-600 flex items-center justify-center shadow-lg shadow-teal-200">
            <i class="fas fa-tachometer-alt text-white text-sm"></i>
          </div>
          <span>Platform Health Metrics</span>
        </h2>
      </div>
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        <div
          v-for="kpi in kpiCards"
          :key="kpi.id"
          class="kpi-card bg-white rounded-2xl p-5 shadow-sm border border-gray-100 hover:shadow-lg transition-all cursor-pointer group"
          :style="`--accent-color: ${getColorClasses(kpi.color).shadow.replace('shadow-', '')}`"
        >
          <div class="flex items-start justify-between mb-4">
            <div :class="['w-12 h-12 rounded-xl bg-gradient-to-br flex items-center justify-center shadow-lg', getColorClasses(kpi.color).bg, getColorClasses(kpi.color).shadow]">
              <i :class="[kpi.icon, 'text-white text-lg']"></i>
            </div>
            <span v-if="kpi.change" :class="[
              'px-2 py-1 rounded-full text-xs font-semibold',
              kpi.changeType === 'positive' ? 'bg-green-100 text-green-700' : kpi.changeType === 'negative' ? 'bg-red-100 text-red-700' : 'bg-gray-100 text-gray-600'
            ]">
              <i v-if="kpi.changeType === 'positive'" class="fas fa-arrow-up mr-1 text-[10px]"></i>
              <i v-else-if="kpi.changeType === 'negative'" class="fas fa-arrow-down mr-1 text-[10px]"></i>
              {{ kpi.change }}
            </span>
          </div>
          <p class="text-2xl font-bold text-gray-900 mb-1">{{ kpi.value }}<span class="text-lg text-gray-500">{{ kpi.suffix }}</span></p>
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
        <!-- Platform Usage Trends Chart -->
        <div class="lg:col-span-2 bg-white rounded-2xl shadow-sm border border-gray-100 overflow-hidden">
          <div class="px-5 py-4 border-b border-gray-100 flex items-center justify-between">
            <h2 class="text-lg font-bold text-gray-900 flex items-center gap-3">
              <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-teal-500 to-teal-600 flex items-center justify-center shadow-lg shadow-teal-200">
                <i class="fas fa-chart-area text-white text-sm"></i>
              </div>
              <div>
                <span class="block">Platform Usage Trends</span>
                <span class="text-xs font-medium text-gray-500">Active users vs target</span>
              </div>
            </h2>
            <div class="flex items-center gap-4">
              <div class="flex items-center gap-2">
                <span class="w-3 h-3 rounded-full bg-teal-500"></span>
                <span class="text-xs text-gray-500">Active Users</span>
              </div>
              <div class="flex items-center gap-2">
                <span class="w-3 h-3 rounded-full bg-gray-300"></span>
                <span class="text-xs text-gray-500">Target</span>
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
                    class="w-full bg-gray-300 rounded-t transition-all hover:bg-gray-400"
                    :style="{ height: (trafficData.datasets[1].data[idx] / Math.max(...trafficData.datasets[0].data) * 180) + 'px' }"
                  ></div>
                </div>
                <span class="text-[10px] text-gray-400 mt-2">{{ trafficData.labels[idx] }}</span>
              </div>
            </div>
          </div>
        </div>

        <!-- This Week's Highlights (Tournament Records) -->
        <div class="bg-white rounded-2xl shadow-sm border border-gray-100 overflow-hidden">
          <div class="px-5 py-4 border-b border-gray-100">
            <h2 class="text-lg font-bold text-gray-900 flex items-center gap-3">
              <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-amber-500 to-amber-600 flex items-center justify-center shadow-lg shadow-amber-200">
                <i class="fas fa-bolt text-white text-sm"></i>
              </div>
              <div>
                <span class="block">This Week's Highlights</span>
                <span class="text-xs font-medium text-gray-500">Tournament Records</span>
              </div>
            </h2>
          </div>
          <div class="p-4 space-y-3">
            <div v-for="record in tournamentRecords" :key="record.id" class="flex items-center gap-3 p-3 rounded-xl bg-gray-50 hover:bg-teal-50 transition-colors">
              <div :class="['w-10 h-10 rounded-lg flex items-center justify-center', record.iconBg]">
                <i :class="[record.icon, record.iconColor]"></i>
              </div>
              <div class="flex-1">
                <p class="text-xl font-bold text-gray-900">{{ record.value }}</p>
                <p class="text-xs text-gray-500">{{ record.label }}</p>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Content Engagement Row -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <!-- Content Engagement by Type -->
        <div class="bg-white rounded-2xl shadow-sm border border-gray-100 overflow-hidden">
          <div class="px-5 py-4 border-b border-gray-100 flex items-center justify-between">
            <h2 class="text-lg font-bold text-gray-900 flex items-center gap-3">
              <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-blue-500 to-blue-600 flex items-center justify-center shadow-lg shadow-blue-200">
                <i class="fas fa-book-reader text-white text-sm"></i>
              </div>
              <div>
                <span class="block">Content Engagement by Type</span>
                <span class="text-xs font-medium text-gray-500">Views vs Interactions</span>
              </div>
            </h2>
            <div class="flex items-center gap-2">
              <button
                v-for="type in contentTypes"
                :key="type.value"
                @click="selectedContentType = type.value"
                :class="[
                  'px-3 py-1 rounded-lg text-xs font-medium transition-all',
                  selectedContentType === type.value ? 'bg-teal-500 text-white' : 'bg-gray-100 text-gray-600 hover:bg-gray-200'
                ]"
              >
                {{ type.label }}
              </button>
            </div>
          </div>
          <div class="p-5">
            <!-- Bar Chart Placeholder -->
            <div class="h-48 flex items-end justify-around gap-4 px-4">
              <div v-for="(item, idx) in ['Articles', 'Courses', 'Events', 'Documents', 'Polls', 'Videos']" :key="idx" class="flex-1 flex flex-col items-center gap-1">
                <div class="w-full flex gap-1 items-end justify-center">
                  <div class="w-5 bg-teal-500 rounded-t hover:bg-teal-600 transition-all" :style="{ height: [120, 90, 60, 100, 40, 50][idx] + 'px' }"></div>
                  <div class="w-5 bg-blue-400 rounded-t hover:bg-blue-500 transition-all" :style="{ height: [55, 45, 42, 30, 27, 22][idx] + 'px' }"></div>
                </div>
                <span class="text-[10px] text-gray-400 mt-2">{{ item }}</span>
              </div>
            </div>
            <div class="flex items-center justify-center gap-6 mt-4">
              <div class="flex items-center gap-2">
                <span class="w-3 h-3 rounded bg-teal-500"></span>
                <span class="text-xs text-gray-500">Views</span>
              </div>
              <div class="flex items-center gap-2">
                <span class="w-3 h-3 rounded bg-blue-400"></span>
                <span class="text-xs text-gray-500">Interactions</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Department Participation (Donut Chart) -->
        <div class="bg-white rounded-2xl shadow-sm border border-gray-100 overflow-hidden">
          <div class="px-5 py-4 border-b border-gray-100">
            <h2 class="text-lg font-bold text-gray-900 flex items-center gap-3">
              <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-purple-500 to-purple-600 flex items-center justify-center shadow-lg shadow-purple-200">
                <i class="fas fa-building text-white text-sm"></i>
              </div>
              <div>
                <span class="block">Department Participation</span>
                <span class="text-xs font-medium text-gray-500">Platform adoption by team</span>
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

      <!-- Top Performers & Activity Row -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <!-- Top Performing Content (Full Width Table) -->
        <div class="lg:col-span-2 bg-white rounded-2xl shadow-sm border border-gray-100 overflow-hidden">
          <div class="px-5 py-4 border-b border-gray-100 flex items-center justify-between">
            <h2 class="text-lg font-bold text-gray-900 flex items-center gap-3">
              <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-orange-500 to-orange-600 flex items-center justify-center shadow-lg shadow-orange-200">
                <i class="fas fa-fire text-white text-sm"></i>
              </div>
              <div>
                <span class="block">Top Performing Content</span>
                <span class="text-xs font-medium text-gray-500">Top players & performers</span>
              </div>
            </h2>
            <div class="flex items-center gap-2">
              <button
                v-for="type in contentTypes"
                :key="type.value"
                @click="selectedContentType = type.value"
                :class="[
                  'px-3 py-1 rounded-lg text-xs font-medium transition-all',
                  selectedContentType === type.value ? 'bg-teal-500 text-white' : 'bg-gray-100 text-gray-600 hover:bg-gray-200'
                ]"
              >
                {{ type.label }}
              </button>
            </div>
          </div>
          <div class="overflow-x-auto">
            <table class="w-full">
              <thead>
                <tr class="bg-gray-50 border-b border-gray-100">
                  <th class="text-left px-5 py-3 text-[11px] font-semibold text-gray-500 uppercase tracking-wider">Content</th>
                  <th class="text-left px-5 py-3 text-[11px] font-semibold text-gray-500 uppercase tracking-wider">Views</th>
                  <th class="text-left px-5 py-3 text-[11px] font-semibold text-gray-500 uppercase tracking-wider">Engagement</th>
                  <th class="text-left px-5 py-3 text-[11px] font-semibold text-gray-500 uppercase tracking-wider">Completion</th>
                </tr>
              </thead>
              <tbody class="divide-y divide-gray-50">
                <tr
                  v-for="performer in topPerformers"
                  :key="performer.id"
                  class="hover:bg-gray-50 transition-colors cursor-pointer"
                >
                  <td class="px-5 py-3">
                    <div class="flex items-center gap-3">
                      <div :class="['w-10 h-10 rounded-xl flex items-center justify-center', performer.iconBg]">
                        <i :class="[performer.icon, performer.iconColor]"></i>
                      </div>
                      <div>
                        <p class="text-sm font-semibold text-gray-900">{{ performer.title }}</p>
                        <p class="text-xs text-gray-500">{{ performer.author }} · {{ performer.stats }}</p>
                      </div>
                    </div>
                  </td>
                  <td class="px-5 py-3">
                    <p class="text-sm font-medium text-gray-900">{{ performer.matches }}</p>
                  </td>
                  <td class="px-5 py-3">
                    <div class="flex items-center gap-2">
                      <div class="w-24 h-1.5 bg-gray-100 rounded-full overflow-hidden">
                        <div class="h-full rounded-full transition-all" :style="{ width: performer.engagement + '%', backgroundColor: performer.color }"></div>
                      </div>
                      <span class="text-xs text-gray-600">{{ performer.engagement }}%</span>
                    </div>
                  </td>
                  <td class="px-5 py-3">
                    <span :class="['text-sm font-medium', performer.completion > 70 ? 'text-green-600' : performer.completion > 40 ? 'text-amber-600' : 'text-gray-500']">
                      {{ performer.completion }}%
                    </span>
                  </td>
                </tr>
              </tbody>
            </table>
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
                <span class="block">Top Content</span>
                <span class="text-xs font-medium text-gray-500">By views and rating</span>
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

      <!-- Insights, Team Leaderboard & Department Stats Row -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <!-- AI-Powered Insights -->
        <div class="insights-card rounded-2xl p-6 text-white relative overflow-hidden">
          <div class="relative z-10">
            <h3 class="text-lg font-bold mb-4 flex items-center gap-2">
              <i class="fas fa-lightbulb text-teal-300"></i>
              AI-Powered Insights
            </h3>
            <div class="space-y-4">
              <div v-for="insight in tournamentInsights" :key="insight.id" class="flex items-start gap-3 pb-3 border-b border-white/10 last:border-0">
                <div class="w-7 h-7 rounded-lg bg-teal-500/20 flex items-center justify-center flex-shrink-0">
                  <i :class="[insight.icon, 'text-teal-300 text-xs']"></i>
                </div>
                <p class="text-sm text-gray-200 leading-relaxed" v-html="insight.text"></p>
              </div>
            </div>
          </div>
        </div>

        <!-- Team Leaderboard -->
        <div class="bg-white rounded-2xl shadow-sm border border-gray-100 overflow-hidden">
          <div class="px-5 py-4 border-b border-gray-100">
            <h2 class="text-lg font-bold text-gray-900 flex items-center gap-3">
              <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-purple-500 to-purple-600 flex items-center justify-center shadow-lg shadow-purple-200">
                <i class="fas fa-crown text-white text-sm"></i>
              </div>
              <div>
                <span class="block">Team Leaderboard</span>
                <span class="text-xs font-medium text-gray-500">Top contributing teams</span>
              </div>
            </h2>
          </div>
          <div class="p-4 space-y-2">
            <div v-for="(team, idx) in teamStatistics" :key="team.id" class="flex items-center gap-3 p-2 rounded-xl hover:bg-gray-50 transition-colors">
              <div :class="[
                'w-7 h-7 rounded-lg flex items-center justify-center text-xs font-bold',
                idx === 0 ? 'bg-gradient-to-br from-amber-400 to-amber-500 text-white' :
                idx === 1 ? 'bg-gradient-to-br from-gray-300 to-gray-400 text-white' :
                idx === 2 ? 'bg-gradient-to-br from-orange-400 to-orange-500 text-white' :
                'bg-gray-100 text-gray-500'
              ]">
                {{ idx + 1 }}
              </div>
              <div :class="['w-9 h-9 rounded-full flex items-center justify-center text-white text-xs font-semibold', team.gradient]">
                {{ team.initials }}
              </div>
              <div class="flex-1">
                <p class="text-sm font-medium text-gray-900">{{ team.name }}</p>
                <p class="text-xs text-gray-500">{{ team.department }}</p>
              </div>
              <div class="text-right">
                <p class="text-sm font-bold text-gray-900">{{ team.contributions }}</p>
                <p class="text-[10px] text-gray-400">points</p>
              </div>
            </div>
          </div>
        </div>

        <!-- Department Stats Summary -->
        <div class="bg-white rounded-2xl shadow-sm border border-gray-100 overflow-hidden">
          <div class="px-5 py-4 border-b border-gray-100">
            <h2 class="text-lg font-bold text-gray-900 flex items-center gap-3">
              <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-emerald-500 to-emerald-600 flex items-center justify-center shadow-lg shadow-emerald-200">
                <i class="fas fa-building text-white text-sm"></i>
              </div>
              <div>
                <span class="block">Department Stats</span>
                <span class="text-xs font-medium text-gray-500">Adoption overview</span>
              </div>
            </h2>
          </div>
          <div class="p-4 space-y-3">
            <div v-for="dept in departmentStats.slice(0, 4)" :key="dept.id" class="p-3 rounded-xl bg-gray-50 hover:bg-teal-50 transition-colors">
              <div class="flex items-center justify-between mb-2">
                <div class="flex items-center gap-2">
                  <div :class="['w-8 h-8 rounded-lg flex items-center justify-center text-white text-xs font-semibold', dept.gradient]">
                    {{ dept.initials }}
                  </div>
                  <span class="text-sm font-medium text-gray-900">{{ dept.name }}</span>
                </div>
                <span class="text-sm font-bold" :style="{ color: dept.color }">{{ dept.adoption }}%</span>
              </div>
              <div class="w-full h-1.5 bg-gray-200 rounded-full overflow-hidden">
                <div class="h-full rounded-full transition-all" :style="{ width: dept.adoption + '%', backgroundColor: dept.color }"></div>
              </div>
              <div class="flex items-center justify-between mt-2 text-[10px] text-gray-400">
                <span>{{ dept.employees }} employees</span>
                <span>{{ dept.articles }} articles · {{ dept.events }} events</span>
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

/* KPI Card with accent top border */
.kpi-card {
  position: relative;
  overflow: hidden;
}

.kpi-card::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 3px;
  background: linear-gradient(90deg, var(--accent-color, #14b8a6), var(--accent-color-end, #0d9488));
}

.kpi-card:hover {
  transform: translateY(-4px);
  border-color: #ccfbf1;
}

/* Insights Card (Dark) */
.insights-card {
  background: linear-gradient(135deg, #0f172a 0%, #1e293b 100%);
}

.insights-card::before {
  content: '';
  position: absolute;
  top: -50%;
  right: -50%;
  width: 100%;
  height: 100%;
  background: radial-gradient(circle, rgba(20, 184, 166, 0.3) 0%, transparent 60%);
  pointer-events: none;
}

/* Animation for fade-in */
@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.animate-in {
  animation: fadeInUp 0.5s cubic-bezier(0.4, 0, 0.2, 1) forwards;
}
</style>
