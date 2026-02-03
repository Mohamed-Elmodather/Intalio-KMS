<script setup lang="ts">
import { ref, computed } from 'vue'
import { useI18n } from 'vue-i18n'
import PageHeroHeader from '@/components/common/PageHeroHeader.vue'
import { useAIServicesStore } from '@/stores/aiServices'
import { AILoadingIndicator, AIConfidenceBar } from '@/components/ai'

const { t } = useI18n()
const aiStore = useAIServicesStore()

// Helper function to safely get max value from array
function safeMax(arr: number[] | undefined, fallback = 1): number {
  if (!arr || arr.length === 0) return fallback
  return Math.max(...arr)
}

// Helper function to safely get array value by index
function safeGet<T>(arr: T[] | undefined, index: number, fallback: T): T {
  if (!arr || index < 0 || index >= arr.length) return fallback
  return arr[index] ?? fallback
}

// State
const isLoading = ref(false)
const selectedPeriod = ref('30d')
const selectedMetric = ref('views')
const selectedDepartment = ref('all')
const selectedContentType = ref('all')
const hoveredSegment = ref<string | null>(null)

// Period options (Date ranges)
const periodOptions = computed(() => [
  { value: '7d', label: t('analytics.periods.sevenDays') },
  { value: '30d', label: t('analytics.periods.thirtyDays') },
  { value: '90d', label: t('analytics.periods.quarter') },
  { value: 'ytd', label: t('analytics.periods.ytd') }
])

// Content type filters
const contentTypes = computed(() => [
  { value: 'all', label: t('analytics.contentTypes.all') },
  { value: 'articles', label: t('analytics.contentTypes.articles') },
  { value: 'courses', label: t('analytics.contentTypes.courses') },
  { value: 'events', label: t('analytics.contentTypes.events') }
])

// Content Engagement Data
const contentEngagement = computed(() => [
  { name: t('analytics.engagementTypes.articles'), views: 4200, interactions: 1800, color: '#14b8a6', colorLight: '#5eead4' },
  { name: t('analytics.engagementTypes.courses'), views: 2800, interactions: 1500, color: '#3b82f6', colorLight: '#93c5fd' },
  { name: t('analytics.engagementTypes.events'), views: 1900, interactions: 1400, color: '#8b5cf6', colorLight: '#c4b5fd' },
  { name: t('analytics.engagementTypes.documents'), views: 3100, interactions: 980, color: '#f59e0b', colorLight: '#fcd34d' },
  { name: t('analytics.engagementTypes.polls'), views: 1200, interactions: 890, color: '#ec4899', colorLight: '#f9a8d4' },
  { name: t('analytics.engagementTypes.videos'), views: 1600, interactions: 720, color: '#10b981', colorLight: '#6ee7b7' }
])

// Get max value for chart scaling
const maxEngagementValue = computed(() => {
  return Math.max(...contentEngagement.value.map((c: { views: number }) => c.views))
})

// Department filters
const departments = computed(() => [
  { id: 'all', name: t('analytics.departments.allDepartments') },
  { id: 'engineering', name: t('analytics.departments.engineering') },
  { id: 'marketing', name: t('analytics.departments.marketing') },
  { id: 'sales', name: t('analytics.departments.sales') },
  { id: 'hr', name: t('analytics.departments.humanResources') },
  { id: 'finance', name: t('analytics.departments.finance') },
  { id: 'operations', name: t('analytics.departments.operations') }
])

// Executive Stats (Hero)
const stats = ref({
  platformAdoption: 78,
  activeUsers: 842,
  totalContent: 1247,
  engagementScore: 8.4
})

// Hero stats for PageHeroHeader component
const heroStats = computed(() => [
  { icon: 'fas fa-chart-pie', value: stats.value.platformAdoption + '%', label: t('analytics.platformAdoption') },
  { icon: 'fas fa-users', value: stats.value.activeUsers.toLocaleString(), label: t('analytics.activeUsers') },
  { icon: 'fas fa-file-alt', value: stats.value.totalContent.toLocaleString(), label: t('analytics.totalContent') },
  { icon: 'fas fa-star', value: stats.value.engagementScore, label: t('analytics.engagementScore') }
])

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
const categoryBreakdown = computed(() => [
  { name: t('analytics.categories.engineering'), value: 28, color: '#14b8a6' },
  { name: t('analytics.categories.marketing'), value: 18, color: '#3b82f6' },
  { name: t('analytics.categories.hr'), value: 15, color: '#ec4899' },
  { name: t('analytics.categories.sales'), value: 14, color: '#8b5cf6' },
  { name: t('analytics.categories.product'), value: 13, color: '#06b6d4' },
  { name: t('analytics.categories.operations'), value: 12, color: '#10b981' }
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

interface ColorClasses {
  bg: string
  icon: string
  text: string
  shadow: string
}

function getColorClasses(color: string): ColorClasses {
  const colors: Record<string, ColorClasses> = {
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
  return colors[color] ?? colors.teal!
}

function downloadReport(report: any) {
  alert(`Downloading ${report.name}...`)
}

function exportData(format: string) {
  alert(`Exporting data as ${format}...`)
}

// ============================================================================
// AI Features - State & Functions
// ============================================================================

// AI State
const showNLQueryModal = ref(false)
const showAIReportModal = ref(false)
const showTrendPredictions = ref(false)
const showAnomalyPanel = ref(false)
const isProcessingQuery = ref(false)
const isGeneratingReport = ref(false)
const nlQuery = ref('')
const nlQueryResult = ref<NLQueryResult | null>(null)

// AI Interfaces
interface NLQueryResult {
  query: string
  interpretation: string
  answer: string
  dataPoints: { label: string; value: string }[]
  confidence: number
  suggestedQueries: string[]
}

interface TrendPrediction {
  id: string
  metric: string
  currentValue: string
  predictedValue: string
  timeframe: string
  confidence: number
  trend: 'up' | 'down' | 'stable'
  insight: string
}

interface MetricAnomaly {
  id: string
  metric: string
  anomalyType: 'spike' | 'drop' | 'pattern_change'
  severity: 'low' | 'medium' | 'high'
  description: string
  value: string
  expectedRange: string
  detectedAt: string
}

interface AIGeneratedReport {
  id: string
  title: string
  summary: string
  highlights: string[]
  recommendations: string[]
  generatedAt: Date
}

// AI Data
const trendPredictions = ref<TrendPrediction[]>([
  {
    id: '1',
    metric: 'Platform Traffic',
    currentValue: '35,000',
    predictedValue: '52,000',
    timeframe: 'Next 30 days',
    confidence: 0.87,
    trend: 'up',
    insight: 'Expected increase due to upcoming tournament opening'
  },
  {
    id: '2',
    metric: 'Document Downloads',
    currentValue: '8,492',
    predictedValue: '12,100',
    timeframe: 'Next 30 days',
    confidence: 0.82,
    trend: 'up',
    insight: 'Policy documents demand rising as event approaches'
  },
  {
    id: '3',
    metric: 'Course Completions',
    currentValue: '456',
    predictedValue: '680',
    timeframe: 'Next 30 days',
    confidence: 0.79,
    trend: 'up',
    insight: 'Staff training acceleration expected'
  },
  {
    id: '4',
    metric: 'User Engagement',
    currentValue: '8.4/10',
    predictedValue: '8.1/10',
    timeframe: 'Next 30 days',
    confidence: 0.74,
    trend: 'down',
    insight: 'Slight dip expected during transition period'
  }
])

const metricAnomalies = ref<MetricAnomaly[]>([
  {
    id: '1',
    metric: 'Document Views',
    anomalyType: 'spike',
    severity: 'medium',
    description: 'Unusual spike in Venue Guidelines views',
    value: '847% above normal',
    expectedRange: '200-400 views/day',
    detectedAt: '2 hours ago'
  },
  {
    id: '2',
    metric: 'Login Frequency',
    anomalyType: 'pattern_change',
    severity: 'low',
    description: 'Login pattern shift detected - more evening activity',
    value: '+34% evening logins',
    expectedRange: 'Peak at 9-11 AM',
    detectedAt: '1 day ago'
  }
])

const generatedReport = ref<AIGeneratedReport | null>(null)

// Example NL queries
const exampleQueries = [
  'What was our top performing content last month?',
  'Show me engagement trends for Engineering department',
  'Compare article views vs course completions this quarter',
  'Which content type has the highest completion rate?',
  'What are the peak usage hours for the platform?'
]

// AI Functions
async function processNLQuery() {
  if (!nlQuery.value.trim()) return

  isProcessingQuery.value = true

  try {
    await new Promise(resolve => setTimeout(resolve, 1500))

    nlQueryResult.value = {
      query: nlQuery.value,
      interpretation: `Analyzing "${nlQuery.value}" across platform metrics and content data`,
      answer: `Based on the analysis, the top performing content last month was "Tournament Regulations 2027" with 12,450 views and a 4.8 rating. This document saw a 34% increase in engagement compared to the previous month, driven primarily by the Engineering and Operations departments.`,
      dataPoints: [
        { label: 'Top Content', value: 'Tournament Regulations 2027' },
        { label: 'Total Views', value: '12,450' },
        { label: 'Rating', value: '4.8/5' },
        { label: 'Month-over-Month', value: '+34%' },
        { label: 'Primary Audience', value: 'Engineering, Operations' }
      ],
      confidence: 0.91,
      suggestedQueries: [
        'What content should we create next?',
        'Show department breakdown for this content',
        'Compare with previous quarter'
      ]
    }
  } finally {
    isProcessingQuery.value = false
  }
}

function useExampleQuery(query: string) {
  nlQuery.value = query
  processNLQuery()
}

async function generateAIReport() {
  isGeneratingReport.value = true
  showAIReportModal.value = true

  try {
    await new Promise(resolve => setTimeout(resolve, 2000))

    generatedReport.value = {
      id: '1',
      title: 'AI-Generated Analytics Summary',
      summary: 'Platform performance remains strong with 78% adoption rate. Content engagement increased by 23% this period, driven primarily by tournament preparation materials. User activity patterns indicate healthy engagement across all departments.',
      highlights: [
        'Platform adoption reached 78%, exceeding target by 8%',
        'Article engagement up 34% driven by tournament content',
        'Engineering department leads with 92% adoption rate',
        'Document downloads increased 23% month-over-month',
        'Average session duration increased to 12.4 minutes'
      ],
      recommendations: [
        'Create more video content for Operations department - low engagement detected',
        'Schedule content updates during peak hours (9-11 AM) for maximum visibility',
        'Consider gamification for Finance department to boost adoption from 58%',
        'Develop mobile-optimized content - 34% increase in mobile usage detected',
        'Archive low-performing content older than 6 months to improve search relevance'
      ],
      generatedAt: new Date()
    }
  } finally {
    isGeneratingReport.value = false
  }
}

function dismissAnomaly(id: string) {
  metricAnomalies.value = metricAnomalies.value.filter(a => a.id !== id)
}

function getTrendIcon(trend: string): string {
  switch (trend) {
    case 'up': return 'fas fa-arrow-trend-up'
    case 'down': return 'fas fa-arrow-trend-down'
    default: return 'fas fa-minus'
  }
}

function getTrendColor(trend: string): string {
  switch (trend) {
    case 'up': return 'text-green-600 bg-green-100'
    case 'down': return 'text-red-600 bg-red-100'
    default: return 'text-gray-600 bg-gray-100'
  }
}

function getAnomalySeverityColor(severity: string): string {
  switch (severity) {
    case 'high': return 'bg-red-100 border-red-200 text-red-700'
    case 'medium': return 'bg-amber-100 border-amber-200 text-amber-700'
    case 'low': return 'bg-blue-100 border-blue-200 text-blue-700'
    default: return 'bg-gray-100 border-gray-200 text-gray-700'
  }
}
</script>

<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Hero Section -->
    <PageHeroHeader
      :stats="heroStats"
      badge-icon="fas fa-chart-pie"
      :title="$t('analytics.title')"
      :subtitle="$t('analytics.subtitle')"
    >
      <template #actions>
        <button @click="exportData('PDF')" class="px-5 py-2.5 bg-white text-teal-600 rounded-xl font-semibold text-sm flex items-center gap-2 hover:bg-teal-50 transition-all shadow-lg">
          <i class="fas fa-download"></i>
          {{ $t('analytics.exportReport') }}
        </button>
        <button class="px-5 py-2.5 bg-white/20 backdrop-blur-sm border border-white/30 text-white rounded-xl font-semibold text-sm hover:bg-white/30 transition-all flex items-center gap-2">
          <i class="fas fa-calendar-alt"></i>
          {{ $t('analytics.scheduleReport') }}
        </button>
        <!-- AI Buttons -->
        <button @click="showNLQueryModal = true" class="px-5 py-2.5 bg-gradient-to-r from-emerald-400 to-teal-400 text-white rounded-xl font-semibold text-sm hover:from-emerald-500 hover:to-teal-500 transition-all flex items-center gap-2 shadow-lg shadow-teal-500/20">
          <i class="fas fa-comment-dots"></i>
          {{ $t('analytics.askAI') }}
        </button>
        <button @click="generateAIReport" class="px-5 py-2.5 bg-white/20 backdrop-blur-sm border border-white/30 text-white rounded-xl font-semibold text-sm hover:bg-white/30 transition-all flex items-center gap-2">
          <i class="fas fa-wand-magic-sparkles"></i>
          {{ $t('analytics.aiReport') }}
        </button>
        <!-- AI Quick Stats -->
        <div class="flex flex-wrap gap-4 mt-4 w-full">
          <button @click="showTrendPredictions = true" class="px-4 py-2 bg-white/10 backdrop-blur-sm rounded-xl border border-white/20 text-white text-sm flex items-center gap-2 hover:bg-white/20 transition-all">
            <i class="fas fa-crystal-ball text-purple-300"></i>
            <span>{{ trendPredictions.length }} {{ $t('analytics.predictions') }}</span>
          </button>
          <button @click="showAnomalyPanel = true" class="px-4 py-2 bg-white/10 backdrop-blur-sm rounded-xl border border-white/20 text-white text-sm flex items-center gap-2 hover:bg-white/20 transition-all">
            <i class="fas fa-exclamation-triangle text-amber-300"></i>
            <span>{{ metricAnomalies.length }} {{ $t('analytics.anomalies') }}</span>
          </button>
        </div>
      </template>
    </PageHeroHeader>

    <!-- Main Content -->
    <div class="px-8 py-6 space-y-6">
      <!-- Period Selector -->
      <div class="flex items-center justify-between">
        <div class="flex items-center gap-2">
          <span class="text-sm font-medium text-gray-600">{{ $t('analytics.timePeriod') }}:</span>
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
            {{ $t('analytics.filter') }}
          </button>
          <button class="px-3 py-1.5 bg-white border border-gray-200 rounded-lg text-sm text-gray-600 hover:bg-gray-50 flex items-center gap-2">
            <i class="fas fa-sync-alt text-xs"></i>
            {{ $t('analytics.refresh') }}
          </button>
        </div>
      </div>

      <!-- Platform Health Metrics (KPI Cards) -->
      <div class="mb-4">
        <h2 class="text-lg font-bold text-gray-900 flex items-center gap-3 mb-4">
          <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-teal-500 to-teal-600 flex items-center justify-center shadow-lg shadow-teal-200">
            <i class="fas fa-tachometer-alt text-white text-sm"></i>
          </div>
          <span>{{ $t('analytics.platformHealthMetrics') }}</span>
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
              :style="{ height: (val / safeMax(kpi.sparkline) * 100) + '%', opacity: 0.3 + (idx / kpi.sparkline.length) * 0.7 }"
            ></div>
          </div>
        </div>
      </div>

      <!-- Charts Row -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <!-- Platform Usage Trends Chart -->
        <div class="lg:col-span-2 bg-white rounded-2xl shadow-sm border border-gray-100">
          <div class="px-5 py-4 border-b border-gray-100">
            <h2 class="text-lg font-bold text-gray-900 flex items-center gap-3">
              <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-teal-500 to-teal-600 flex items-center justify-center shadow-lg shadow-teal-200">
                <i class="fas fa-chart-area text-white text-sm"></i>
              </div>
              <div>
                <span class="block">{{ $t('analytics.platformUsageTrends') }}</span>
                <span class="text-xs font-medium text-gray-500">{{ $t('analytics.activeUsersVsTarget') }}</span>
              </div>
            </h2>
          </div>
          <div class="p-5">
            <!-- Chart Area -->
            <div class="h-56 flex items-end justify-between gap-2 mb-4">
              <div v-for="(val, idx) in trafficData.datasets[0]?.data ?? []" :key="idx" class="flex-1 flex flex-col items-center">
                <div class="w-full flex gap-0.5 items-end justify-center h-44">
                  <div
                    class="w-2/5 bg-teal-500 rounded-t transition-all hover:bg-teal-600"
                    :style="{ height: (val / safeMax(trafficData.datasets[0]?.data) * 100) + '%' }"
                  ></div>
                  <div
                    class="w-2/5 bg-gray-300 rounded-t transition-all hover:bg-gray-400"
                    :style="{ height: (safeGet(trafficData.datasets[1]?.data, idx, 0) / safeMax(trafficData.datasets[0]?.data) * 100) + '%' }"
                  ></div>
                </div>
                <span class="text-[10px] text-gray-400 mt-2">{{ safeGet(trafficData.labels, idx, '') }}</span>
              </div>
            </div>
            <!-- Legend -->
            <div class="flex items-center justify-center gap-6 pt-2 border-t border-gray-100">
              <div class="flex items-center gap-2">
                <span class="w-3 h-3 rounded-full bg-teal-500"></span>
                <span class="text-xs text-gray-500">{{ $t('analytics.activeUsers') }}</span>
              </div>
              <div class="flex items-center gap-2">
                <span class="w-3 h-3 rounded-full bg-gray-300"></span>
                <span class="text-xs text-gray-500">{{ $t('analytics.target') }}</span>
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
                <span class="block">{{ $t('analytics.thisWeeksHighlights') }}</span>
                <span class="text-xs font-medium text-gray-500">{{ $t('analytics.tournamentRecords') }}</span>
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
        <div class="bg-white rounded-2xl shadow-sm border border-gray-100">
          <div class="px-5 py-4 border-b border-gray-100">
            <div class="flex flex-wrap items-center justify-between gap-3">
              <h2 class="text-lg font-bold text-gray-900 flex items-center gap-3">
                <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-blue-500 to-blue-600 flex items-center justify-center shadow-lg shadow-blue-200">
                  <i class="fas fa-book-reader text-white text-sm"></i>
                </div>
                <div>
                  <span class="block">{{ $t('analytics.contentEngagementByType') }}</span>
                  <span class="text-xs font-medium text-gray-500">{{ $t('analytics.viewsVsInteractions') }}</span>
                </div>
              </h2>
              <div class="flex items-center gap-1">
                <button
                  v-for="type in contentTypes"
                  :key="type.value"
                  @click="selectedContentType = type.value"
                  :class="[
                    'px-3 py-1.5 rounded-lg text-xs font-medium transition-all',
                    selectedContentType === type.value ? 'bg-teal-500 text-white shadow-sm' : 'bg-gray-50 text-gray-600 hover:bg-gray-100'
                  ]"
                >
                  {{ type.label }}
                </button>
              </div>
            </div>
          </div>
          <div class="p-5">
            <!-- Enhanced Bar Chart -->
            <div class="h-56 flex items-end justify-between gap-3">
              <div
                v-for="item in contentEngagement"
                :key="item.name"
                class="flex-1 flex flex-col items-center group"
              >
                <!-- Value Labels (show on hover) -->
                <div class="opacity-0 group-hover:opacity-100 transition-opacity mb-1 text-center">
                  <span class="text-[10px] font-semibold" :style="{ color: item.color }">{{ (item.views / 1000).toFixed(1) }}K</span>
                </div>
                <!-- Bar Container -->
                <div class="w-full flex gap-1 items-end justify-center h-40">
                  <!-- Views Bar -->
                  <div
                    class="engagement-bar w-5 rounded-t-md transition-all duration-300 group-hover:w-6 cursor-pointer relative"
                    :style="{
                      height: (item.views / maxEngagementValue * 100) + '%',
                      background: `linear-gradient(to top, ${item.color}, ${item.colorLight})`
                    }"
                  >
                    <div class="absolute -top-6 left-1/2 -translate-x-1/2 opacity-0 group-hover:opacity-100 transition-opacity whitespace-nowrap">
                      <span class="text-[9px] font-bold px-1.5 py-0.5 rounded bg-gray-800 text-white">{{ item.views.toLocaleString() }}</span>
                    </div>
                  </div>
                  <!-- Interactions Bar -->
                  <div
                    class="engagement-bar w-5 rounded-t-md transition-all duration-300 group-hover:w-6 cursor-pointer relative"
                    :style="{
                      height: (item.interactions / maxEngagementValue * 100) + '%',
                      background: `linear-gradient(to top, ${item.color}80, ${item.colorLight}80)`
                    }"
                  >
                  </div>
                </div>
                <!-- Label -->
                <div class="mt-3 text-center">
                  <span class="text-[11px] font-medium text-gray-600 group-hover:text-gray-900 transition-colors">{{ item.name }}</span>
                </div>
              </div>
            </div>
            <!-- Legend -->
            <div class="flex items-center justify-center gap-8 mt-5 pt-4 border-t border-gray-100">
              <div class="flex items-center gap-2">
                <div class="w-4 h-3 rounded-sm bg-gradient-to-r from-teal-500 to-teal-300"></div>
                <span class="text-xs font-medium text-gray-600">{{ $t('analytics.views') }}</span>
              </div>
              <div class="flex items-center gap-2">
                <div class="w-4 h-3 rounded-sm bg-gradient-to-r from-teal-500/50 to-teal-300/50"></div>
                <span class="text-xs font-medium text-gray-600">{{ $t('analytics.interactions') }}</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Department Participation (Donut Chart) -->
        <div class="bg-white rounded-2xl shadow-sm border border-gray-100">
          <div class="px-5 py-4 border-b border-gray-100">
            <h2 class="text-lg font-bold text-gray-900 flex items-center gap-3">
              <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-purple-500 to-purple-600 flex items-center justify-center shadow-lg shadow-purple-200">
                <i class="fas fa-building text-white text-sm"></i>
              </div>
              <div>
                <span class="block">{{ $t('analytics.departmentParticipation') }}</span>
                <span class="text-xs font-medium text-gray-500">{{ $t('analytics.platformAdoptionByTeam') }}</span>
              </div>
            </h2>
          </div>
          <div class="p-5">
            <div class="flex items-center gap-6">
              <!-- Donut Chart -->
              <div class="relative w-72 h-72 flex-shrink-0">
                <svg viewBox="0 0 100 100" class="w-full h-full -rotate-90 donut-chart">
                  <!-- Background circle -->
                  <circle cx="50" cy="50" r="40" fill="none" stroke="#f1f5f9" stroke-width="8" />
                  <!-- Segments -->
                  <circle
                    v-for="(cat, idx) in categoryBreakdown"
                    :key="cat.name"
                    cx="50"
                    cy="50"
                    r="40"
                    fill="none"
                    :stroke="cat.color"
                    stroke-width="8"
                    :stroke-dasharray="(cat.value / totalCategoryValue * 251.33) + ' 251.33'"
                    :stroke-dashoffset="-categoryBreakdown.slice(0, idx).reduce((sum, c) => sum + (c.value / totalCategoryValue * 251.33), 0)"
                    class="donut-segment transition-all duration-500 cursor-pointer"
                    stroke-linecap="round"
                    :style="{ '--segment-color': cat.color, animationDelay: (idx * 0.1) + 's' }"
                    @mouseenter="hoveredSegment = cat.name"
                    @mouseleave="hoveredSegment = null"
                  />
                </svg>
                <!-- Center Content -->
                <div class="absolute inset-0 flex items-center justify-center flex-col">
                  <div class="w-36 h-36 rounded-full bg-white shadow-inner flex items-center justify-center flex-col">
                    <span class="text-4xl font-bold text-gray-900">{{ totalCategoryValue }}%</span>
                    <span class="text-xs text-gray-500 uppercase tracking-wide">{{ $t('analytics.active') }}</span>
                  </div>
                </div>
              </div>
              <!-- Legend with Progress -->
              <div class="flex-1 space-y-2.5">
                <div
                  v-for="cat in categoryBreakdown"
                  :key="cat.name"
                  class="group p-2 rounded-lg transition-all cursor-pointer"
                  :class="hoveredSegment === cat.name ? 'bg-gray-50 scale-[1.02]' : 'hover:bg-gray-50'"
                  @mouseenter="hoveredSegment = cat.name"
                  @mouseleave="hoveredSegment = null"
                >
                  <div class="flex items-center justify-between mb-1">
                    <div class="flex items-center gap-2">
                      <span class="w-2.5 h-2.5 rounded-full transition-transform group-hover:scale-125" :style="{ backgroundColor: cat.color }"></span>
                      <span class="text-xs font-medium text-gray-700 group-hover:text-gray-900">{{ cat.name }}</span>
                    </div>
                    <span class="text-xs font-bold" :style="{ color: cat.color }">{{ cat.value }}%</span>
                  </div>
                  <div class="w-full h-1.5 bg-gray-100 rounded-full overflow-hidden">
                    <div
                      class="h-full rounded-full transition-all duration-500 progress-bar-animated"
                      :style="{ width: cat.value + '%', backgroundColor: cat.color }"
                    ></div>
                  </div>
                </div>
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
                <span class="block">{{ $t('analytics.topPerformingContent') }}</span>
                <span class="text-xs font-medium text-gray-500">{{ $t('analytics.topPlayersAndPerformers') }}</span>
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
                  <th class="text-start px-5 py-3 text-[11px] font-semibold text-gray-500 uppercase tracking-wider">{{ $t('analytics.content') }}</th>
                  <th class="text-start px-5 py-3 text-[11px] font-semibold text-gray-500 uppercase tracking-wider">{{ $t('analytics.views') }}</th>
                  <th class="text-start px-5 py-3 text-[11px] font-semibold text-gray-500 uppercase tracking-wider">{{ $t('analytics.engagement') }}</th>
                  <th class="text-start px-5 py-3 text-[11px] font-semibold text-gray-500 uppercase tracking-wider">{{ $t('analytics.completion') }}</th>
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
                <span class="block">{{ $t('analytics.topContent') }}</span>
                <span class="text-xs font-medium text-gray-500">{{ $t('analytics.byViewsAndRating') }}</span>
              </div>
            </h2>
            <button class="text-sm text-teal-600 hover:text-teal-700 font-medium flex items-center gap-1">
              {{ $t('analytics.viewAll') }}
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
                <p class="text-xs text-gray-500">{{ $t('analytics.views').toLowerCase() }}</p>
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
                <span class="block">{{ $t('analytics.recentActivity') }}</span>
                <span class="text-xs font-medium text-gray-500">{{ $t('analytics.userInteractions') }}</span>
              </div>
            </h2>
            <button class="text-sm text-teal-600 hover:text-teal-700 font-medium flex items-center gap-1">
              {{ $t('analytics.viewAll') }}
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
        <!-- {{ $t('analytics.aiPoweredInsights') }} -->
        <div class="insights-card rounded-2xl p-6 text-white relative overflow-hidden">
          <div class="relative z-10">
            <h3 class="text-lg font-bold mb-4 flex items-center gap-2">
              <i class="fas fa-lightbulb text-teal-300"></i>
              {{ $t('analytics.aiPoweredInsights') }}
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
                <span class="block">{{ $t('analytics.teamLeaderboard') }}</span>
                <span class="text-xs font-medium text-gray-500">{{ $t('analytics.topContributingTeams') }}</span>
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
                <p class="text-[10px] text-gray-400">{{ $t('analytics.points') }}</p>
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
                <span class="block">{{ $t('analytics.departmentStats') }}</span>
                <span class="text-xs font-medium text-gray-500">{{ $t('analytics.adoptionOverview') }}</span>
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
              <span class="block">{{ $t('analytics.generatedReports') }}</span>
              <span class="text-xs font-medium text-gray-500">{{ $t('analytics.downloadPastReports') }}</span>
            </div>
          </h2>
          <button class="px-4 py-2 bg-gradient-to-r from-teal-500 to-teal-600 text-white rounded-lg text-sm font-medium hover:from-teal-600 hover:to-teal-700 transition-all flex items-center gap-2 shadow-sm shadow-teal-200">
            <i class="fas fa-plus"></i>
            {{ $t('analytics.generateNew') }}
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
              {{ $t('analytics.download') }}
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- AI Natural Language Query Modal -->
    <Teleport to="body">
      <div v-if="showNLQueryModal" class="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
        <div class="bg-white rounded-2xl shadow-2xl w-full max-w-2xl max-h-[85vh] overflow-hidden">
          <div class="p-5 border-b border-gray-100 bg-gradient-to-r from-teal-500 to-emerald-500">
            <div class="flex items-center justify-between">
              <div class="flex items-center gap-3">
                <div class="w-10 h-10 rounded-xl bg-white/20 flex items-center justify-center">
                  <i class="fas fa-comment-dots text-white"></i>
                </div>
                <div>
                  <h3 class="font-semibold text-white">Ask AI About Analytics</h3>
                  <p class="text-xs text-white/80">Natural language queries for your data</p>
                </div>
              </div>
              <button @click="showNLQueryModal = false" class="p-2 hover:bg-white/20 rounded-lg transition-colors">
                <i class="fas fa-times text-white"></i>
              </button>
            </div>
          </div>

          <div class="p-5">
            <!-- Query Input -->
            <div class="relative mb-4">
              <input v-model="nlQuery"
                     @keyup.enter="processNLQuery"
                     type="text"
                     placeholder="Ask a question about your analytics data..."
                     class="w-full pl-12 pr-24 py-4 border border-gray-200 rounded-xl focus:ring-2 focus:ring-teal-500 focus:border-teal-500 text-gray-900">
              <i class="fas fa-search absolute left-4 top-1/2 -translate-y-1/2 text-gray-400"></i>
              <button @click="processNLQuery"
                      :disabled="isProcessingQuery || !nlQuery.trim()"
                      class="absolute right-2 top-1/2 -translate-y-1/2 px-4 py-2 bg-teal-500 text-white rounded-lg text-sm font-medium hover:bg-teal-600 disabled:opacity-50 disabled:cursor-not-allowed transition-colors">
                <i v-if="isProcessingQuery" class="fas fa-circle-notch fa-spin"></i>
                <span v-else>Ask</span>
              </button>
            </div>

            <!-- Example Queries -->
            <div v-if="!nlQueryResult" class="mb-4">
              <p class="text-xs text-gray-500 mb-2">Try asking:</p>
              <div class="flex flex-wrap gap-2">
                <button v-for="query in exampleQueries" :key="query"
                        @click="useExampleQuery(query)"
                        class="px-3 py-1.5 bg-gray-100 rounded-full text-xs text-gray-600 hover:bg-teal-100 hover:text-teal-700 transition-colors">
                  {{ query }}
                </button>
              </div>
            </div>

            <!-- Query Result -->
            <div v-if="nlQueryResult" class="space-y-4">
              <div class="p-4 bg-gray-50 rounded-xl">
                <div class="flex items-center gap-2 mb-2">
                  <i class="fas fa-robot text-teal-500"></i>
                  <span class="text-xs text-gray-500">{{ nlQueryResult.interpretation }}</span>
                </div>
                <p class="text-sm text-gray-700 leading-relaxed">{{ nlQueryResult.answer }}</p>
              </div>

              <!-- Data Points -->
              <div class="grid grid-cols-2 md:grid-cols-3 gap-3">
                <div v-for="point in nlQueryResult.dataPoints" :key="point.label"
                     class="p-3 bg-white border border-gray-200 rounded-xl">
                  <p class="text-xs text-gray-500">{{ point.label }}</p>
                  <p class="text-lg font-bold text-gray-900">{{ point.value }}</p>
                </div>
              </div>

              <!-- Confidence -->
              <div class="flex items-center gap-3">
                <span class="text-xs text-gray-500">Confidence:</span>
                <div class="flex-1 h-2 bg-gray-200 rounded-full overflow-hidden">
                  <div class="h-full bg-teal-500 rounded-full" :style="{ width: `${nlQueryResult.confidence * 100}%` }"></div>
                </div>
                <span class="text-xs font-bold text-teal-600">{{ Math.round(nlQueryResult.confidence * 100) }}%</span>
              </div>

              <!-- Suggested Follow-ups -->
              <div>
                <p class="text-xs text-gray-500 mb-2">Related questions:</p>
                <div class="flex flex-wrap gap-2">
                  <button v-for="query in nlQueryResult.suggestedQueries" :key="query"
                          @click="useExampleQuery(query)"
                          class="px-3 py-1.5 bg-teal-50 rounded-full text-xs text-teal-600 hover:bg-teal-100 transition-colors">
                    {{ query }}
                  </button>
                </div>
              </div>
            </div>
          </div>

          <div class="p-4 border-t border-gray-100 bg-gray-50 flex justify-end">
            <button @click="showNLQueryModal = false"
                    class="px-4 py-2 bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200 transition-colors text-sm">
              Close
            </button>
          </div>
        </div>
      </div>
    </Teleport>

    <!-- AI Trend Predictions Modal -->
    <Teleport to="body">
      <div v-if="showTrendPredictions" class="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
        <div class="bg-white rounded-2xl shadow-2xl w-full max-w-2xl max-h-[80vh] overflow-hidden">
          <div class="p-5 border-b border-gray-100 bg-gradient-to-r from-purple-500 to-indigo-500">
            <div class="flex items-center justify-between">
              <div class="flex items-center gap-3">
                <div class="w-10 h-10 rounded-xl bg-white/20 flex items-center justify-center">
                  <i class="fas fa-crystal-ball text-white"></i>
                </div>
                <div>
                  <h3 class="font-semibold text-white">{{ $t('analytics.aiPoweredInsights') }}</h3>
                  <p class="text-xs text-white/80">Forecasted metrics based on historical data</p>
                </div>
              </div>
              <button @click="showTrendPredictions = false" class="p-2 hover:bg-white/20 rounded-lg transition-colors">
                <i class="fas fa-times text-white"></i>
              </button>
            </div>
          </div>

          <div class="p-5 overflow-y-auto max-h-[60vh]">
            <div class="space-y-4">
              <div v-for="prediction in trendPredictions" :key="prediction.id"
                   class="p-4 rounded-xl border border-gray-200 hover:shadow-md transition-all">
                <div class="flex items-start gap-4">
                  <div :class="['w-12 h-12 rounded-xl flex items-center justify-center', getTrendColor(prediction.trend)]">
                    <i :class="[getTrendIcon(prediction.trend), 'text-lg']"></i>
                  </div>
                  <div class="flex-1">
                    <h4 class="font-semibold text-gray-900">{{ prediction.metric }}</h4>
                    <div class="flex items-center gap-4 mt-2">
                      <div>
                        <p class="text-xs text-gray-500">Current</p>
                        <p class="text-lg font-bold text-gray-700">{{ prediction.currentValue }}</p>
                      </div>
                      <i class="fas fa-arrow-right text-gray-300"></i>
                      <div>
                        <p class="text-xs text-gray-500">Predicted</p>
                        <p :class="['text-lg font-bold', prediction.trend === 'up' ? 'text-green-600' : prediction.trend === 'down' ? 'text-red-600' : 'text-gray-600']">
                          {{ prediction.predictedValue }}
                        </p>
                      </div>
                    </div>
                    <p class="text-sm text-gray-600 mt-2">{{ prediction.insight }}</p>
                    <div class="flex items-center gap-3 mt-2">
                      <span class="text-xs text-gray-400">{{ prediction.timeframe }}</span>
                      <span class="text-xs text-purple-600 font-medium">{{ Math.round(prediction.confidence * 100) }}% confidence</span>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div class="p-4 border-t border-gray-100 bg-gray-50 flex justify-end">
            <button @click="showTrendPredictions = false"
                    class="px-4 py-2 bg-purple-500 text-white rounded-lg hover:bg-purple-600 transition-colors text-sm">
              Close
            </button>
          </div>
        </div>
      </div>
    </Teleport>

    <!-- AI Anomaly Panel Modal -->
    <Teleport to="body">
      <div v-if="showAnomalyPanel" class="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
        <div class="bg-white rounded-2xl shadow-2xl w-full max-w-lg max-h-[80vh] overflow-hidden">
          <div class="p-5 border-b border-gray-100 bg-gradient-to-r from-amber-500 to-orange-500">
            <div class="flex items-center justify-between">
              <div class="flex items-center gap-3">
                <div class="w-10 h-10 rounded-xl bg-white/20 flex items-center justify-center">
                  <i class="fas fa-exclamation-triangle text-white"></i>
                </div>
                <div>
                  <h3 class="font-semibold text-white">{{ $t('analytics.detectedAnomalies') }}</h3>
                  <p class="text-xs text-white/80">Unusual patterns in your metrics</p>
                </div>
              </div>
              <button @click="showAnomalyPanel = false" class="p-2 hover:bg-white/20 rounded-lg transition-colors">
                <i class="fas fa-times text-white"></i>
              </button>
            </div>
          </div>

          <div class="p-5 overflow-y-auto max-h-[55vh]">
            <div v-if="metricAnomalies.length === 0" class="text-center py-8">
              <i class="fas fa-check-circle text-green-500 text-4xl mb-3"></i>
              <p class="text-gray-600">No anomalies detected. All metrics are within expected ranges.</p>
            </div>

            <div v-else class="space-y-3">
              <div v-for="anomaly in metricAnomalies" :key="anomaly.id"
                   :class="['p-4 rounded-xl border', getAnomalySeverityColor(anomaly.severity)]">
                <div class="flex items-start justify-between">
                  <div class="flex-1">
                    <div class="flex items-center gap-2 mb-1">
                      <span class="font-semibold">{{ anomaly.metric }}</span>
                      <span class="px-2 py-0.5 rounded-full text-xs font-medium capitalize" :class="getAnomalySeverityColor(anomaly.severity)">
                        {{ anomaly.severity }}
                      </span>
                    </div>
                    <p class="text-sm opacity-90">{{ anomaly.description }}</p>
                    <div class="flex items-center gap-4 mt-2 text-xs opacity-75">
                      <span>Value: <strong>{{ anomaly.value }}</strong></span>
                      <span>Expected: {{ anomaly.expectedRange }}</span>
                    </div>
                    <p class="text-xs opacity-60 mt-1">Detected {{ anomaly.detectedAt }}</p>
                  </div>
                  <button @click="dismissAnomaly(anomaly.id)" class="p-1 hover:bg-white/50 rounded transition-colors">
                    <i class="fas fa-times text-sm"></i>
                  </button>
                </div>
              </div>
            </div>
          </div>

          <div class="p-4 border-t border-gray-100 bg-gray-50 flex justify-end">
            <button @click="showAnomalyPanel = false"
                    class="px-4 py-2 bg-amber-500 text-white rounded-lg hover:bg-amber-600 transition-colors text-sm">
              Close
            </button>
          </div>
        </div>
      </div>
    </Teleport>

    <!-- AI Generated Report Modal -->
    <Teleport to="body">
      <div v-if="showAIReportModal" class="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
        <div class="bg-white rounded-2xl shadow-2xl w-full max-w-2xl max-h-[85vh] overflow-hidden">
          <div class="p-5 border-b border-gray-100 bg-gradient-to-r from-teal-500 to-emerald-500">
            <div class="flex items-center justify-between">
              <div class="flex items-center gap-3">
                <div class="w-10 h-10 rounded-xl bg-white/20 flex items-center justify-center">
                  <i class="fas fa-wand-magic-sparkles text-white"></i>
                </div>
                <div>
                  <h3 class="font-semibold text-white">{{ $t('analytics.aiGeneratedReport') }}</h3>
                  <p class="text-xs text-white/80">Automated analytics summary</p>
                </div>
              </div>
              <button @click="showAIReportModal = false" class="p-2 hover:bg-white/20 rounded-lg transition-colors">
                <i class="fas fa-times text-white"></i>
              </button>
            </div>
          </div>

          <div class="p-5 overflow-y-auto max-h-[60vh]">
            <div v-if="isGeneratingReport" class="text-center py-12">
              <i class="fas fa-circle-notch fa-spin text-teal-500 text-4xl mb-4"></i>
              <p class="text-gray-600">Analyzing data and generating report...</p>
              <p class="text-xs text-gray-400 mt-2">This may take a few seconds</p>
            </div>

            <div v-else-if="generatedReport" class="space-y-6">
              <div>
                <h4 class="text-xl font-bold text-gray-900">{{ generatedReport.title }}</h4>
                <p class="text-xs text-gray-500 mt-1">{{ $t('analytics.generated') }} {{ generatedReport.generatedAt.toLocaleString() }}</p>
              </div>

              <div class="p-4 bg-teal-50 rounded-xl border border-teal-100">
                <h5 class="font-semibold text-teal-800 mb-2">Executive Summary</h5>
                <p class="text-sm text-teal-700">{{ generatedReport.summary }}</p>
              </div>

              <div>
                <h5 class="font-semibold text-gray-900 mb-3 flex items-center gap-2">
                  <i class="fas fa-star text-amber-500"></i>
                  Key Highlights
                </h5>
                <ul class="space-y-2">
                  <li v-for="(highlight, idx) in generatedReport.highlights" :key="idx"
                      class="flex items-start gap-2 text-sm text-gray-700">
                    <i class="fas fa-check-circle text-green-500 mt-0.5"></i>
                    <span>{{ highlight }}</span>
                  </li>
                </ul>
              </div>

              <div>
                <h5 class="font-semibold text-gray-900 mb-3 flex items-center gap-2">
                  <i class="fas fa-lightbulb text-amber-500"></i>
                  AI Recommendations
                </h5>
                <ul class="space-y-2">
                  <li v-for="(rec, idx) in generatedReport.recommendations" :key="idx"
                      class="flex items-start gap-2 text-sm text-gray-700">
                    <span class="w-5 h-5 rounded-full bg-purple-100 text-purple-600 flex items-center justify-center text-xs font-bold flex-shrink-0">{{ idx + 1 }}</span>
                    <span>{{ rec }}</span>
                  </li>
                </ul>
              </div>
            </div>
          </div>

          <div class="p-4 border-t border-gray-100 bg-gray-50 flex justify-between items-center">
            <p class="text-xs text-gray-500">
              <i class="fas fa-robot mr-1"></i>
              AI-generated based on current period data
            </p>
            <div class="flex gap-2">
              <button @click="showAIReportModal = false"
                      class="px-4 py-2 bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200 transition-colors text-sm">
                Close
              </button>
              <button v-if="generatedReport"
                      class="px-4 py-2 bg-teal-500 text-white rounded-lg hover:bg-teal-600 transition-colors text-sm flex items-center gap-2">
                <i class="fas fa-download"></i>
                Export PDF
              </button>
            </div>
          </div>
        </div>
      </div>
    </Teleport>
  </div>
</template>

<style scoped>
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

/* Engagement Bar Styles */
.engagement-bar {
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  position: relative;
}

.engagement-bar:hover {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  transform: scaleY(1.02);
  transform-origin: bottom;
}

/* Bar Animation on Load */
@keyframes barGrow {
  from {
    transform: scaleY(0);
    opacity: 0;
  }
  to {
    transform: scaleY(1);
    opacity: 1;
  }
}

.engagement-bar {
  animation: barGrow 0.6s cubic-bezier(0.4, 0, 0.2, 1) forwards;
  transform-origin: bottom;
}

.engagement-bar:nth-child(1) { animation-delay: 0.1s; }
.engagement-bar:nth-child(2) { animation-delay: 0.15s; }

/* Donut Chart Styles */
.donut-chart {
  filter: drop-shadow(0 4px 6px rgba(0, 0, 0, 0.05));
}

.donut-segment {
  transform-origin: center;
  transition: all 0.3s ease;
}

.donut-segment:hover {
  stroke-width: 14;
  filter: brightness(1.1);
}

/* Donut segment animation */
@keyframes donutDraw {
  from {
    stroke-dashoffset: 238.76;
  }
}

.donut-segment {
  animation: donutDraw 1s ease-out forwards;
}

/* Progress bar animation */
@keyframes progressGrow {
  from {
    width: 0;
  }
}

.progress-bar-animated {
  animation: progressGrow 0.8s ease-out forwards;
}
</style>
