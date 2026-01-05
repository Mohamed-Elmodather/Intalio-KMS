<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'

const router = useRouter()

// State
const isLoading = ref(false)
const showQuickPollModal = ref(false)
const activeTab = ref('active')
const searchQuery = ref('')
const selectedCategory = ref('all')

// Interfaces
interface PollOption {
  label: string
  percentage: number
  isWinner?: boolean
}

interface FeaturedPoll {
  id: string
  question: string
  description: string
  options: PollOption[]
  totalVotes: number
  endsIn: string
  author: string
  anonymous: boolean
  hasVoted: boolean
  selectedOption: number | null
}

interface PollForYou {
  id: number
  question: string
  category: string
  categoryClass: string
  categoryIcon: string
  totalVotes: number
  endsIn: string
  urgencyClass: string
}

interface TrendingPoll {
  id: number
  question: string
  totalVotes: number
  weeklyGrowth: number
  responseRate: number
}

interface ActivePoll {
  id: number
  question: string
  category: string
  categoryClass: string
  categoryIcon: string
  author: string
  anonymous: boolean
  options: PollOption[]
  totalVotes: number
  responseRate: number
  endsIn: string
  urgencyClass: string
  hasVoted: boolean
  selectedOption: number | null
}

interface MyPoll {
  id: number
  title: string
  status: string
  statusClass: string
  responses: number
  created: string
}

interface CompletedPoll {
  id: number
  question: string
  options: PollOption[]
  totalVotes: number
  endedDate: string
}

interface QuickPoll {
  question: string
  options: string[]
  anonymous: boolean
}

// Stats data
const stats = ref({
  active: 8,
  responses: 2847,
  created: 45,
  participation: 82
})

// Tabs
const tabs = ref([
  { id: 'active', label: 'Active', icon: 'fas fa-play-circle', count: 8 },
  { id: 'my-polls', label: 'My Polls', icon: 'fas fa-user', count: 12 },
  { id: 'completed', label: 'Completed', icon: 'fas fa-check-circle', count: 23 }
])

// Categories
const categories = ref([
  { id: 'all', label: 'All Polls', icon: 'fas fa-th-large', count: 45 },
  { id: 'hr', label: 'HR & Culture', icon: 'fas fa-users', count: 12 },
  { id: 'product', label: 'Product', icon: 'fas fa-box', count: 8 },
  { id: 'team', label: 'Team', icon: 'fas fa-user-friends', count: 10 },
  { id: 'feedback', label: 'Feedback', icon: 'fas fa-comment-dots', count: 9 },
  { id: 'company', label: 'Company', icon: 'fas fa-building', count: 6 }
])

// Featured Poll
const featuredPoll = ref<FeaturedPoll>({
  id: 'featured',
  question: 'What should be our top priority for Q1 2025?',
  description: "Help us decide the company's main focus for the upcoming quarter. Your input matters!",
  options: [
    { label: 'Product Innovation', percentage: 42 },
    { label: 'Customer Experience', percentage: 31 },
    { label: 'Team Growth', percentage: 18 },
    { label: 'Market Expansion', percentage: 9 }
  ],
  totalVotes: 456,
  endsIn: 'Ends in 3 days',
  author: 'Leadership Team',
  anonymous: true,
  hasVoted: false,
  selectedOption: null
})

// Polls For You
const pollsForYou = ref<PollForYou[]>([
  {
    id: 1,
    question: "What's your preferred remote work schedule?",
    category: 'HR',
    categoryClass: 'hr',
    categoryIcon: 'fas fa-users',
    totalVotes: 234,
    endsIn: '2 days',
    urgencyClass: 'safe'
  },
  {
    id: 2,
    question: 'Which feature should we build next?',
    category: 'Product',
    categoryClass: 'product',
    categoryIcon: 'fas fa-box',
    totalVotes: 189,
    endsIn: '5 hours',
    urgencyClass: 'urgent'
  },
  {
    id: 3,
    question: 'Rate our new onboarding process',
    category: 'Feedback',
    categoryClass: 'feedback',
    categoryIcon: 'fas fa-comment-dots',
    totalVotes: 67,
    endsIn: '1 week',
    urgencyClass: 'safe'
  },
  {
    id: 4,
    question: 'Best day for team lunch?',
    category: 'Team',
    categoryClass: 'team',
    categoryIcon: 'fas fa-user-friends',
    totalVotes: 45,
    endsIn: '12 hours',
    urgencyClass: 'warning'
  }
])

// Trending Polls
const trendingPolls = ref<TrendingPoll[]>([
  { id: 1, question: 'Should we adopt a 4-day work week?', totalVotes: 892, weeklyGrowth: 156, responseRate: 94 },
  { id: 2, question: 'Best team building activity for winter?', totalVotes: 567, weeklyGrowth: 89, responseRate: 78 },
  { id: 3, question: 'New office location preferences', totalVotes: 445, weeklyGrowth: 67, responseRate: 65 },
  { id: 4, question: 'Preferred lunch vendors for catering', totalVotes: 334, weeklyGrowth: 45, responseRate: 72 },
  { id: 5, question: 'Training topics for next quarter', totalVotes: 289, weeklyGrowth: 34, responseRate: 58 },
  { id: 6, question: 'Company swag preferences', totalVotes: 256, weeklyGrowth: 28, responseRate: 62 }
])

// Active Polls
const activePolls = ref<ActivePoll[]>([
  {
    id: 1,
    question: "What's your preferred meeting time?",
    category: 'Team',
    categoryClass: 'team',
    categoryIcon: 'fas fa-user-friends',
    author: 'Sarah Chen',
    anonymous: false,
    options: [
      { label: 'Morning (9-11 AM)', percentage: 45, isWinner: true },
      { label: 'Afternoon (2-4 PM)', percentage: 35 },
      { label: 'No preference', percentage: 20 }
    ],
    totalVotes: 156,
    responseRate: 78,
    endsIn: '2 days',
    urgencyClass: 'safe',
    hasVoted: true,
    selectedOption: 0
  },
  {
    id: 2,
    question: 'Which team building activity would you prefer?',
    category: 'HR',
    categoryClass: 'hr',
    categoryIcon: 'fas fa-users',
    author: 'HR Team',
    anonymous: true,
    options: [
      { label: 'Virtual game night', percentage: 0 },
      { label: 'Outdoor adventure', percentage: 0 },
      { label: 'Cooking class', percentage: 0 },
      { label: 'Escape room', percentage: 0 }
    ],
    totalVotes: 89,
    responseRate: 45,
    endsIn: '5 days',
    urgencyClass: 'safe',
    hasVoted: false,
    selectedOption: null
  },
  {
    id: 3,
    question: 'Rate the new office coffee machine',
    category: 'Feedback',
    categoryClass: 'feedback',
    categoryIcon: 'fas fa-comment-dots',
    author: 'Facilities',
    anonymous: false,
    options: [
      { label: 'Excellent', percentage: 52, isWinner: true },
      { label: 'Good', percentage: 30 },
      { label: 'Needs improvement', percentage: 18 }
    ],
    totalVotes: 234,
    responseRate: 92,
    endsIn: 'Tomorrow',
    urgencyClass: 'warning',
    hasVoted: true,
    selectedOption: 0
  },
  {
    id: 4,
    question: 'Should we implement flexible Fridays?',
    category: 'Company',
    categoryClass: 'company',
    categoryIcon: 'fas fa-building',
    author: 'Leadership',
    anonymous: true,
    options: [
      { label: 'Yes, every Friday', percentage: 0 },
      { label: 'Yes, twice a month', percentage: 0 },
      { label: 'No preference', percentage: 0 }
    ],
    totalVotes: 67,
    responseRate: 34,
    endsIn: '3 hours',
    urgencyClass: 'urgent',
    hasVoted: false,
    selectedOption: null
  },
  {
    id: 5,
    question: 'Preferred documentation tool?',
    category: 'Product',
    categoryClass: 'product',
    categoryIcon: 'fas fa-box',
    author: 'Engineering',
    anonymous: false,
    options: [
      { label: 'Notion', percentage: 48, isWinner: true },
      { label: 'Confluence', percentage: 32 },
      { label: 'GitBook', percentage: 20 }
    ],
    totalVotes: 145,
    responseRate: 72,
    endsIn: '4 days',
    urgencyClass: 'safe',
    hasVoted: true,
    selectedOption: 1
  },
  {
    id: 6,
    question: 'Best time for all-hands meeting?',
    category: 'Team',
    categoryClass: 'team',
    categoryIcon: 'fas fa-user-friends',
    author: 'Operations',
    anonymous: false,
    options: [
      { label: 'Monday 10 AM', percentage: 0 },
      { label: 'Wednesday 2 PM', percentage: 0 },
      { label: 'Friday 11 AM', percentage: 0 }
    ],
    totalVotes: 23,
    responseRate: 12,
    endsIn: '6 hours',
    urgencyClass: 'urgent',
    hasVoted: false,
    selectedOption: null
  }
])

// My Polls
const myPolls = ref<MyPoll[]>([
  { id: 1, title: 'Q4 Priority Features Survey', status: 'Active', statusClass: 'active', responses: 145, created: '2 days ago' },
  { id: 2, title: 'Remote Work Satisfaction', status: 'Draft', statusClass: 'draft', responses: 0, created: '1 week ago' },
  { id: 3, title: 'Training Topics Interest', status: 'Completed', statusClass: 'completed', responses: 289, created: '2 weeks ago' },
  { id: 4, title: 'New Office Layout Preferences', status: 'Active', statusClass: 'active', responses: 78, created: '3 days ago' },
  { id: 5, title: 'Holiday Party Planning', status: 'Scheduled', statusClass: 'scheduled', responses: 0, created: '1 day ago' },
  { id: 6, title: 'Tool Stack Preferences', status: 'Completed', statusClass: 'completed', responses: 167, created: '1 month ago' }
])

// Completed Polls
const completedPolls = ref<CompletedPoll[]>([
  {
    id: 1,
    question: 'Best day for all-hands meetings?',
    options: [
      { label: 'Wednesday', percentage: 55 },
      { label: 'Friday', percentage: 30 },
      { label: 'Monday', percentage: 15 }
    ],
    totalVotes: 312,
    endedDate: 'Dec 15'
  },
  {
    id: 2,
    question: 'Preferred communication tool?',
    options: [
      { label: 'Slack', percentage: 62 },
      { label: 'Teams', percentage: 28 },
      { label: 'Email', percentage: 10 }
    ],
    totalVotes: 456,
    endedDate: 'Dec 10'
  },
  {
    id: 3,
    question: 'Annual retreat location?',
    options: [
      { label: 'Beach Resort', percentage: 48 },
      { label: 'Mountain Lodge', percentage: 35 },
      { label: 'City Hotel', percentage: 17 }
    ],
    totalVotes: 278,
    endedDate: 'Dec 5'
  },
  {
    id: 4,
    question: 'Preferred snack options for office?',
    options: [
      { label: 'Healthy snacks', percentage: 52 },
      { label: 'Mixed variety', percentage: 38 },
      { label: 'Traditional snacks', percentage: 10 }
    ],
    totalVotes: 189,
    endedDate: 'Nov 28'
  }
])

// Quick Poll form
const quickPoll = ref<QuickPoll>({
  question: '',
  options: ['', ''],
  anonymous: false
})

// Computed
const filteredActivePolls = computed(() => {
  let polls = activePolls.value
  if (selectedCategory.value !== 'all') {
    polls = polls.filter(p => p.category.toLowerCase() === selectedCategory.value)
  }
  if (searchQuery.value) {
    const q = searchQuery.value.toLowerCase()
    polls = polls.filter(p => p.question.toLowerCase().includes(q))
  }
  return polls
})

// Methods
function selectFeaturedOption(idx: number) {
  if (!featuredPoll.value.hasVoted) {
    featuredPoll.value.selectedOption = idx
  }
}

function selectOption(pollId: number, optionIdx: number) {
  const poll = activePolls.value.find(p => p.id === pollId)
  if (poll && !poll.hasVoted) {
    poll.selectedOption = optionIdx
  }
}

function addQuickPollOption() {
  if (quickPoll.value.options.length < 5) {
    quickPoll.value.options.push('')
  }
}

function removeQuickPollOption(idx: number) {
  if (quickPoll.value.options.length > 2) {
    quickPoll.value.options.splice(idx, 1)
  }
}

function createQuickPoll() {
  // Simulate poll creation
  alert('Poll created successfully!')
  showQuickPollModal.value = false
  quickPoll.value = { question: '', options: ['', ''], anonymous: false }
}

function goToCreatePoll() {
  router.push({ name: 'PollCreate' })
}
</script>

<template>
  <div class="space-y-6">
    <!-- Loading State -->
    <div v-if="isLoading" class="flex items-center justify-center py-20">
      <LoadingSpinner size="lg" text="Loading polls..." />
    </div>

    <template v-else>
      <!-- Page Hero Section -->
      <div class="hero-section fade-in-up">
        <div class="hero-content">
          <div class="hero-left">
            <div class="hero-header">
              <div class="hero-icon">
                <i class="fas fa-poll"></i>
              </div>
              <div>
                <h1 class="hero-title"><span class="hero-title-highlight">Polls</span> & Surveys</h1>
                <p class="hero-subtitle">Gather feedback and insights from your team</p>
              </div>
            </div>
            <button @click="goToCreatePoll" class="hero-btn">
              <i class="fas fa-plus"></i>
              <span>Create New Poll</span>
            </button>
          </div>

          <!-- Stats Cards -->
          <div class="hero-stats">
            <div class="stat-card-hero">
              <div class="stat-card-hero-icon teal">
                <i class="fas fa-poll"></i>
              </div>
              <div class="stat-card-hero-content">
                <p class="stat-card-hero-value">{{ stats.active }}</p>
                <p class="stat-card-hero-label">Active Polls</p>
              </div>
            </div>
            <div class="stat-card-hero">
              <div class="stat-card-hero-icon blue">
                <i class="fas fa-vote-yea"></i>
              </div>
              <div class="stat-card-hero-content">
                <p class="stat-card-hero-value">{{ stats.responses.toLocaleString() }}</p>
                <p class="stat-card-hero-label">Responses</p>
              </div>
            </div>
            <div class="stat-card-hero">
              <div class="stat-card-hero-icon orange">
                <i class="fas fa-chart-pie"></i>
              </div>
              <div class="stat-card-hero-content">
                <p class="stat-card-hero-value">{{ stats.created }}</p>
                <p class="stat-card-hero-label">Polls Created</p>
              </div>
            </div>
            <div class="stat-card-hero">
              <div class="stat-card-hero-icon purple">
                <i class="fas fa-users"></i>
              </div>
              <div class="stat-card-hero-content">
                <p class="stat-card-hero-value">{{ stats.participation }}%</p>
                <p class="stat-card-hero-label">Participation</p>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Featured Poll -->
      <section v-if="featuredPoll" class="featured-poll mb-10 fade-in-up" style="animation-delay: 0.1s">
        <div class="flex flex-col lg:flex-row gap-6">
          <div class="flex-1">
            <div class="flex items-center gap-3 mb-4">
              <span class="featured-badge">
                <i class="fas fa-star"></i> Featured Poll
              </span>
              <span v-if="featuredPoll.anonymous" class="featured-badge">
                <i class="fas fa-user-secret"></i> Anonymous
              </span>
            </div>
            <h2 class="text-xl lg:text-2xl font-bold mb-2">{{ featuredPoll.question }}</h2>
            <p class="text-white/80 text-sm mb-4">{{ featuredPoll.description }}</p>
            <div class="flex items-center gap-4 text-sm text-white/70">
              <span><i class="fas fa-users mr-1"></i> {{ featuredPoll.totalVotes }} responses</span>
              <span><i class="fas fa-clock mr-1"></i> {{ featuredPoll.endsIn }}</span>
              <span><i class="fas fa-user mr-1"></i> By {{ featuredPoll.author }}</span>
            </div>
          </div>
          <div class="lg:w-80 space-y-2">
            <div
              v-for="(option, idx) in featuredPoll.options"
              :key="idx"
              :class="['featured-option', { selected: featuredPoll.selectedOption === idx }]"
              @click="selectFeaturedOption(idx)"
            >
              <div class="radio-circle"></div>
              <span class="flex-1">{{ option.label }}</span>
              <span v-if="featuredPoll.hasVoted" class="text-sm font-semibold">{{ option.percentage }}%</span>
            </div>
            <button
              v-if="!featuredPoll.hasVoted"
              class="w-full mt-4 py-3 bg-white text-teal-700 font-semibold rounded-xl hover:bg-white/90 transition-all"
            >
              Submit Vote
            </button>
          </div>
        </div>
      </section>

      <!-- Category Pills -->
      <section class="mb-10 fade-in-up" style="animation-delay: 0.15s">
        <div class="category-scroll scrollbar-elegant">
          <button
            v-for="cat in categories"
            :key="cat.id"
            :class="['category-pill', { active: selectedCategory === cat.id }]"
            @click="selectedCategory = cat.id"
          >
            <i :class="cat.icon"></i>
            {{ cat.label }}
            <span v-if="cat.count" class="text-xs opacity-70">({{ cat.count }})</span>
          </button>
        </div>
      </section>

      <!-- Polls For You -->
      <section class="mb-10 fade-in-up" style="animation-delay: 0.2s">
        <div class="section-header-polls">
          <h2 class="section-title-polls">
            <i class="fas fa-hand-point-right"></i>
            Polls For You
          </h2>
          <a href="#" class="text-sm text-teal-600 hover:text-teal-700 font-medium">View All <i class="fas fa-arrow-right ml-1"></i></a>
        </div>
        <div class="polls-scroll scrollbar-elegant">
          <div v-for="poll in pollsForYou" :key="poll.id" class="poll-scroll-card">
            <div class="flex items-center justify-between mb-3">
              <span :class="['poll-category-tag', poll.categoryClass]">
                <i :class="poll.categoryIcon"></i> {{ poll.category }}
              </span>
              <span :class="['deadline-badge', poll.urgencyClass]">
                <i class="fas fa-clock"></i> {{ poll.endsIn }}
              </span>
            </div>
            <h3 class="font-semibold text-gray-900 mb-3 line-clamp-2">{{ poll.question }}</h3>
            <div class="flex items-center justify-between text-sm text-gray-500">
              <span><i class="fas fa-users mr-1"></i> {{ poll.totalVotes }} votes</span>
              <button class="px-3 py-1.5 bg-teal-50 text-teal-600 rounded-lg font-medium hover:bg-teal-100 transition-all">
                Vote Now
              </button>
            </div>
          </div>
          <!-- Quick Poll Card -->
          <div class="poll-scroll-card quick-poll-card" @click="showQuickPollModal = true">
            <div class="quick-poll-icon">
              <i class="fas fa-plus"></i>
            </div>
            <h3 class="font-semibold text-gray-900 mb-1">Quick Poll</h3>
            <p class="text-sm text-gray-500">Create a poll in seconds</p>
          </div>
        </div>
      </section>

      <!-- Trending Polls -->
      <section class="mb-10 fade-in-up" style="animation-delay: 0.25s">
        <div class="section-header-polls">
          <h2 class="section-title-polls">
            <i class="fas fa-fire"></i>
            Trending
          </h2>
        </div>
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          <div v-for="poll in trendingPolls" :key="poll.id" class="trending-card">
            <div class="flex-1 min-w-0">
              <h4 class="font-medium text-gray-900 text-sm truncate">{{ poll.question }}</h4>
              <div class="flex items-center gap-3 text-xs text-gray-500 mt-1">
                <span><i class="fas fa-users mr-1"></i>{{ poll.totalVotes }}</span>
                <span><i class="fas fa-chart-line mr-1"></i>+{{ poll.weeklyGrowth }}%</span>
              </div>
            </div>
            <div class="response-circle">
              <svg width="44" height="44">
                <circle class="bg" cx="22" cy="22" r="18"></circle>
                <circle
                  class="progress"
                  cx="22"
                  cy="22"
                  r="18"
                  :stroke-dasharray="113"
                  :stroke-dashoffset="113 - (113 * poll.responseRate / 100)"
                ></circle>
              </svg>
              <span class="value">{{ poll.responseRate }}%</span>
            </div>
          </div>
        </div>
      </section>

      <!-- Filter Bar -->
      <section class="filter-bar fade-in-up" style="animation-delay: 0.3s">
        <div class="tab-pills">
          <button
            v-for="tab in tabs"
            :key="tab.id"
            :class="['tab-pill', { active: activeTab === tab.id }]"
            @click="activeTab = tab.id"
          >
            <i :class="tab.icon"></i>
            {{ tab.label }}
            <span class="tab-count">{{ tab.count }}</span>
          </button>
        </div>
        <div class="search-input-polls">
          <i class="fas fa-search text-gray-400"></i>
          <input type="text" v-model="searchQuery" placeholder="Search polls...">
        </div>
        <div class="filter-dropdown">
          <i class="fas fa-sort-amount-down text-gray-400"></i>
          <span>Most Recent</span>
          <i class="fas fa-chevron-down text-gray-400 text-xs"></i>
        </div>
      </section>

      <!-- Active Polls Grid -->
      <div v-if="activeTab === 'active'" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        <div
          v-for="(poll, index) in filteredActivePolls"
          :key="poll.id"
          class="poll-card fade-in-up"
          :style="{ animationDelay: (0.35 + index * 0.05) + 's' }"
        >
          <!-- Urgency Bar -->
          <div :class="['urgency-bar', poll.urgencyClass]"></div>

          <div class="poll-card-header">
            <div class="flex items-center justify-between mb-3">
              <span :class="['poll-category-tag', poll.categoryClass]">
                <i :class="poll.categoryIcon"></i> {{ poll.category }}
              </span>
              <div class="flex items-center gap-2">
                <span v-if="poll.anonymous" class="anonymous-badge">
                  <i class="fas fa-user-secret"></i> Anonymous
                </span>
                <span :class="['deadline-badge', poll.urgencyClass]">
                  <i class="fas fa-clock"></i> {{ poll.endsIn }}
                </span>
              </div>
            </div>
            <h3 class="font-semibold text-gray-900 mb-1">{{ poll.question }}</h3>
            <p class="text-xs text-gray-500">By {{ poll.author }}</p>
          </div>

          <div class="poll-card-body space-y-2">
            <div
              v-for="(option, idx) in poll.options"
              :key="idx"
              :class="['poll-option', { voted: poll.hasVoted, selected: poll.selectedOption === idx }]"
              @click="!poll.hasVoted && selectOption(poll.id, idx)"
            >
              <template v-if="poll.hasVoted">
                <div
                  class="progress-bg"
                  :class="{ winner: option.isWinner }"
                  :style="{ width: option.percentage + '%' }"
                ></div>
                <div class="option-content">
                  <span class="text-sm text-gray-700">
                    {{ option.label }}
                    <span v-if="option.isWinner" class="winner-badge">
                      <i class="fas fa-trophy"></i> Leading
                    </span>
                  </span>
                  <span class="text-sm font-semibold text-gray-600">{{ option.percentage }}%</span>
                </div>
              </template>
              <template v-else>
                <div class="option-radio"></div>
                <span class="text-sm text-gray-700">{{ option.label }}</span>
              </template>
            </div>
          </div>

          <div class="poll-card-footer flex items-center justify-between">
            <div class="flex items-center gap-3 text-sm text-gray-500">
              <span><i class="fas fa-users mr-1"></i> {{ poll.totalVotes }}</span>
              <div class="response-circle" style="width: 32px; height: 32px;">
                <svg width="32" height="32">
                  <circle class="bg" cx="16" cy="16" r="13" stroke-width="2"></circle>
                  <circle
                    class="progress"
                    cx="16"
                    cy="16"
                    r="13"
                    stroke-width="2"
                    :stroke-dasharray="82"
                    :stroke-dashoffset="82 - (82 * poll.responseRate / 100)"
                  ></circle>
                </svg>
                <span class="value" style="font-size: 0.5rem;">{{ poll.responseRate }}%</span>
              </div>
            </div>
            <button v-if="!poll.hasVoted" class="btn-vibrant btn-sm ripple-effect">Vote</button>
            <button v-else class="btn btn-secondary btn-sm ripple-effect">Results</button>
          </div>
        </div>
      </div>

      <!-- My Polls -->
      <div v-if="activeTab === 'my-polls'" class="my-polls-card fade-in-up" style="animation-delay: 0.35s">
        <div class="p-4 border-b border-gray-100 flex items-center justify-between">
          <h3 class="font-semibold text-gray-900">Your Polls</h3>
          <div class="flex items-center gap-2">
            <button class="px-3 py-1.5 text-sm bg-gray-100 rounded-lg hover:bg-gray-200 transition-all">
              <i class="fas fa-filter mr-1"></i> Filter
            </button>
          </div>
        </div>
        <div class="divide-y divide-gray-100">
          <div
            v-for="(poll, index) in myPolls"
            :key="poll.id"
            class="my-poll-row fade-in-up"
            :style="{ animationDelay: (0.4 + index * 0.05) + 's' }"
          >
            <div class="flex items-center gap-4 flex-1 min-w-0">
              <div class="w-10 h-10 rounded-lg bg-gradient-to-br from-teal-100 to-teal-200 flex items-center justify-center flex-shrink-0">
                <i class="fas fa-poll text-teal-600"></i>
              </div>
              <div class="flex-1 min-w-0">
                <h4 class="font-medium text-gray-900 truncate">{{ poll.title }}</h4>
                <p class="text-xs text-gray-500">Created {{ poll.created }}</p>
              </div>
            </div>
            <div class="flex items-center gap-4">
              <span :class="['status-pill', poll.statusClass]">{{ poll.status }}</span>
              <div class="text-center" style="min-width: 60px;">
                <p class="font-semibold text-gray-900">{{ poll.responses }}</p>
                <p class="text-xs text-gray-500">responses</p>
              </div>
              <div class="flex items-center gap-1">
                <button class="p-2 rounded-lg hover:bg-teal-50 text-gray-500 hover:text-teal-600 transition-all">
                  <i class="fas fa-chart-bar"></i>
                </button>
                <button class="p-2 rounded-lg hover:bg-teal-50 text-gray-500 hover:text-teal-600 transition-all">
                  <i class="fas fa-edit"></i>
                </button>
                <button class="p-2 rounded-lg hover:bg-teal-50 text-gray-500 hover:text-teal-600 transition-all">
                  <i class="fas fa-share-alt"></i>
                </button>
                <button class="p-2 rounded-lg hover:bg-red-50 text-gray-500 hover:text-red-600 transition-all">
                  <i class="fas fa-trash"></i>
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Completed Polls -->
      <div v-if="activeTab === 'completed'" class="grid grid-cols-1 md:grid-cols-2 gap-6">
        <div
          v-for="(poll, index) in completedPolls"
          :key="poll.id"
          class="completed-poll-card fade-in-up"
          :style="{ animationDelay: (0.35 + index * 0.05) + 's' }"
        >
          <div class="completed-poll-header">
            <div class="flex items-center justify-between mb-2">
              <span class="badge badge-secondary">Completed</span>
              <span class="text-xs text-gray-500"><i class="far fa-calendar-check mr-1"></i>Ended {{ poll.endedDate }}</span>
            </div>
            <h3 class="font-semibold text-gray-900">{{ poll.question }}</h3>
          </div>
          <div class="completed-poll-body space-y-2">
            <div
              v-for="(option, idx) in poll.options"
              :key="idx"
              class="poll-option voted"
            >
              <div
                class="progress-bg"
                :class="{ winner: idx === 0 }"
                :style="{ width: option.percentage + '%' }"
              ></div>
              <div class="option-content">
                <span class="text-sm text-gray-700">
                  {{ option.label }}
                  <span v-if="idx === 0" class="winner-badge">
                    <i class="fas fa-trophy"></i> Winner
                  </span>
                </span>
                <span class="text-sm font-semibold text-gray-600">{{ option.percentage }}%</span>
              </div>
            </div>
          </div>
          <div class="p-4 flex items-center justify-between border-t border-gray-100">
            <span class="text-sm text-gray-500"><i class="fas fa-users mr-1"></i> {{ poll.totalVotes }} total votes</span>
            <div class="flex items-center gap-2">
              <button class="btn btn-secondary btn-sm ripple-effect">
                <i class="fas fa-chart-bar mr-1"></i> Analytics
              </button>
              <button class="btn btn-secondary btn-sm ripple-effect">
                <i class="fas fa-download mr-1"></i> Export
              </button>
            </div>
          </div>
        </div>
      </div>
    </template>

    <!-- Quick Poll Modal -->
    <Teleport to="body">
      <div v-if="showQuickPollModal" class="modal-overlay" @click.self="showQuickPollModal = false">
        <div class="modal-content" style="max-width: 500px;">
          <div class="modal-header">
            <h3 class="section-title-polls"><i class="fas fa-bolt"></i>Quick Poll</h3>
            <button class="modal-close" @click="showQuickPollModal = false">
              <i class="fas fa-times"></i>
            </button>
          </div>
          <div class="modal-body space-y-4">
            <div>
              <label class="form-label">Question</label>
              <input type="text" v-model="quickPoll.question" class="form-input" placeholder="What would you like to ask?">
            </div>
            <div class="space-y-2">
              <label class="form-label">Options</label>
              <div v-for="(opt, idx) in quickPoll.options" :key="idx" class="flex items-center gap-2">
                <input type="text" v-model="quickPoll.options[idx]" class="form-input flex-1" :placeholder="'Option ' + (idx + 1)">
                <button
                  v-if="quickPoll.options.length > 2"
                  @click="removeQuickPollOption(idx)"
                  class="p-2 text-gray-400 hover:text-red-500 transition-all"
                >
                  <i class="fas fa-times"></i>
                </button>
              </div>
              <button
                v-if="quickPoll.options.length < 5"
                @click="addQuickPollOption"
                class="text-sm text-teal-600 hover:text-teal-700 font-medium"
              >
                <i class="fas fa-plus mr-1"></i> Add Option
              </button>
            </div>
            <div class="flex items-center gap-4">
              <label class="flex items-center gap-2 cursor-pointer">
                <input type="checkbox" v-model="quickPoll.anonymous" class="checkbox">
                <span class="text-sm text-gray-600">Anonymous voting</span>
              </label>
            </div>
          </div>
          <div class="modal-footer">
            <button class="btn btn-secondary" @click="showQuickPollModal = false">Cancel</button>
            <button class="btn-vibrant" @click="createQuickPoll">
              <i class="fas fa-paper-plane mr-1"></i> Create Poll
            </button>
          </div>
        </div>
      </div>
    </Teleport>
  </div>
</template>

<style scoped>
/* Featured Poll */
.featured-poll {
  background: linear-gradient(135deg, #0d9488 0%, #14b8a6 50%, #2dd4bf 100%);
  border-radius: 1.5rem;
  padding: 2rem;
  color: white;
  position: relative;
  overflow: hidden;
}
.featured-poll::before {
  content: '';
  position: absolute;
  top: 0;
  right: 0;
  width: 200px;
  height: 200px;
  background: radial-gradient(circle, rgba(255,255,255,0.1) 0%, transparent 70%);
}
.featured-badge {
  display: inline-flex;
  align-items: center;
  gap: 0.375rem;
  background: rgba(255,255,255,0.2);
  backdrop-filter: blur(10px);
  padding: 0.375rem 0.75rem;
  border-radius: 2rem;
  font-size: 0.75rem;
  font-weight: 600;
}
.featured-option {
  background: rgba(255,255,255,0.15);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255,255,255,0.2);
  border-radius: 0.75rem;
  padding: 0.875rem 1rem;
  cursor: pointer;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  gap: 0.75rem;
}
.featured-option:hover {
  background: rgba(255,255,255,0.25);
  transform: translateX(4px);
}
.featured-option.selected {
  background: rgba(255,255,255,0.3);
  border-color: white;
}
.featured-option .radio-circle {
  width: 20px;
  height: 20px;
  border: 2px solid rgba(255,255,255,0.5);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.3s ease;
}
.featured-option.selected .radio-circle {
  border-color: white;
  background: white;
}
.featured-option.selected .radio-circle::after {
  content: '';
  width: 8px;
  height: 8px;
  background: #0d9488;
  border-radius: 50%;
}

/* Quick Poll Card */
.quick-poll-card {
  background: linear-gradient(135deg, #f8fafc 0%, #f0fdfa 100%);
  border: 2px dashed #99f6e4;
  border-radius: 1.5rem;
  padding: 2rem;
  text-align: center;
  transition: all 0.3s ease;
  cursor: pointer;
}
.quick-poll-card:hover {
  border-color: #14b8a6;
  background: linear-gradient(135deg, #f0fdfa 0%, #ccfbf1 100%);
  transform: translateY(-2px);
}
.quick-poll-icon {
  width: 64px;
  height: 64px;
  background: linear-gradient(135deg, #14b8a6, #0d9488);
  border-radius: 1rem;
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 0 auto 1rem;
  color: white;
  font-size: 1.5rem;
}

/* Category Pills */
.category-scroll {
  display: flex;
  gap: 0.75rem;
  overflow-x: auto;
  padding-bottom: 0.5rem;
  -webkit-overflow-scrolling: touch;
}
.category-scroll::-webkit-scrollbar {
  height: 4px;
}
.category-scroll::-webkit-scrollbar-track {
  background: #f1f5f9;
  border-radius: 2px;
}
.category-scroll::-webkit-scrollbar-thumb {
  background: #cbd5e1;
  border-radius: 2px;
}
.category-pill {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.625rem 1rem;
  border-radius: 2rem;
  font-size: 0.875rem;
  font-weight: 500;
  white-space: nowrap;
  cursor: pointer;
  transition: all 0.3s ease;
  border: 1px solid transparent;
}
.category-pill.active {
  background: linear-gradient(135deg, #14b8a6, #0d9488);
  color: white;
  box-shadow: 0 4px 15px rgba(20, 184, 166, 0.3);
}
.category-pill:not(.active) {
  background: white;
  color: #64748b;
  border-color: #e2e8f0;
}
.category-pill:not(.active):hover {
  border-color: #14b8a6;
  color: #0d9488;
}

/* Poll Card Enhanced */
.poll-card {
  background: white;
  border-radius: 1.25rem;
  overflow: hidden;
  box-shadow: 0 4px 20px rgba(0,0,0,0.05);
  transition: all 0.3s ease;
  position: relative;
}
.poll-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 12px 40px rgba(0,0,0,0.1);
}
.poll-card-header {
  padding: 1.25rem 1.25rem 0;
}
.poll-card-body {
  padding: 1rem 1.25rem;
}
.poll-card-footer {
  padding: 1rem 1.25rem;
  background: #f8fafc;
  border-top: 1px solid #f1f5f9;
}
.urgency-bar {
  height: 4px;
  background: linear-gradient(90deg, #22c55e, #22c55e);
}
.urgency-bar.warning {
  background: linear-gradient(90deg, #f59e0b, #eab308);
}
.urgency-bar.urgent {
  background: linear-gradient(90deg, #ef4444, #f97316);
  animation: pulse-urgent 2s ease-in-out infinite;
}
@keyframes pulse-urgent {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.7; }
}
.poll-category-tag {
  display: inline-flex;
  align-items: center;
  gap: 0.25rem;
  padding: 0.25rem 0.625rem;
  border-radius: 1rem;
  font-size: 0.6875rem;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.025em;
}
.poll-category-tag.hr { background: #fef3c7; color: #d97706; }
.poll-category-tag.product { background: #dbeafe; color: #2563eb; }
.poll-category-tag.team { background: #f3e8ff; color: #9333ea; }
.poll-category-tag.feedback { background: #dcfce7; color: #16a34a; }
.poll-category-tag.company { background: #fce7f3; color: #db2777; }
.poll-category-tag.general { background: #e0f2fe; color: #0284c7; }

.deadline-badge {
  display: inline-flex;
  align-items: center;
  gap: 0.375rem;
  font-size: 0.75rem;
  font-weight: 500;
  padding: 0.25rem 0.5rem;
  border-radius: 0.375rem;
}
.deadline-badge.safe { background: #dcfce7; color: #16a34a; }
.deadline-badge.warning { background: #fef3c7; color: #d97706; }
.deadline-badge.urgent { background: #fee2e2; color: #dc2626; }

.anonymous-badge {
  display: inline-flex;
  align-items: center;
  gap: 0.25rem;
  font-size: 0.6875rem;
  color: #64748b;
  background: #f1f5f9;
  padding: 0.25rem 0.5rem;
  border-radius: 0.375rem;
}

/* Poll Option Enhanced */
.poll-option {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.75rem 1rem;
  border-radius: 0.75rem;
  cursor: pointer;
  transition: all 0.2s ease;
  position: relative;
  overflow: hidden;
}
.poll-option:not(.voted) {
  background: #f8fafc;
  border: 1px solid #e2e8f0;
}
.poll-option:not(.voted):hover {
  background: #f0fdfa;
  border-color: #99f6e4;
}
.poll-option.voted {
  background: transparent;
}
.poll-option .option-radio {
  width: 18px;
  height: 18px;
  border: 2px solid #cbd5e1;
  border-radius: 50%;
  flex-shrink: 0;
  transition: all 0.2s ease;
}
.poll-option:hover .option-radio {
  border-color: #14b8a6;
}
.poll-option.selected .option-radio {
  border-color: #14b8a6;
  background: #14b8a6;
  box-shadow: inset 0 0 0 3px white;
}
.poll-option .progress-bg {
  position: absolute;
  left: 0;
  top: 0;
  bottom: 0;
  background: linear-gradient(90deg, #ccfbf1, #99f6e4);
  border-radius: 0.75rem;
  z-index: 0;
  transition: width 0.6s ease;
}
.poll-option .progress-bg.winner {
  background: linear-gradient(90deg, #99f6e4, #5eead4);
}
.poll-option .option-content {
  position: relative;
  z-index: 1;
  flex: 1;
  display: flex;
  justify-content: space-between;
  align-items: center;
}
.winner-badge {
  display: inline-flex;
  align-items: center;
  gap: 0.25rem;
  font-size: 0.625rem;
  font-weight: 700;
  color: #0d9488;
  background: #ccfbf1;
  padding: 0.125rem 0.375rem;
  border-radius: 0.25rem;
  margin-left: 0.5rem;
}

/* Response Rate Circle */
.response-circle {
  width: 44px;
  height: 44px;
  position: relative;
}
.response-circle svg {
  transform: rotate(-90deg);
}
.response-circle circle {
  fill: none;
  stroke-width: 3;
}
.response-circle .bg { stroke: #e5e7eb; }
.response-circle .progress { stroke: #14b8a6; stroke-linecap: round; }
.response-circle .value {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  font-size: 0.625rem;
  font-weight: 700;
  color: #0d9488;
}

/* Polls For You Scroll */
.polls-scroll {
  display: flex;
  gap: 1rem;
  overflow-x: auto;
  padding: 0.5rem 0;
  -webkit-overflow-scrolling: touch;
}
.polls-scroll::-webkit-scrollbar {
  height: 6px;
}
.polls-scroll::-webkit-scrollbar-track {
  background: #f1f5f9;
  border-radius: 3px;
}
.polls-scroll::-webkit-scrollbar-thumb {
  background: #cbd5e1;
  border-radius: 3px;
}
.poll-scroll-card {
  flex: 0 0 300px;
  background: white;
  border-radius: 1rem;
  padding: 1.25rem;
  box-shadow: 0 4px 15px rgba(0,0,0,0.05);
  transition: all 0.3s ease;
}
.poll-scroll-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(0,0,0,0.1);
}

/* Trending Section */
.trending-card {
  position: relative;
  background: white;
  border-radius: 1rem;
  padding: 1rem;
  display: flex;
  align-items: center;
  gap: 1rem;
  box-shadow: 0 2px 10px rgba(0,0,0,0.05);
  transition: all 0.3s ease;
}
.trending-card:hover {
  transform: translateX(4px);
  box-shadow: 0 4px 15px rgba(0,0,0,0.1);
}

/* Section Titles with Visual Effects */
.section-title-polls {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  font-size: 1.125rem;
  font-weight: 600;
  color: #111827;
}

.section-title-polls i {
  width: 1.75rem;
  height: 1.75rem;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 0.5rem;
  font-size: 0.875rem;
  transition: all 0.3s ease;
}

.section-title-polls i.fa-hand-point-right {
  background: linear-gradient(135deg, #f0fdfa 0%, #ccfbf1 100%);
  box-shadow: 0 2px 8px rgba(20, 184, 166, 0.25);
  color: #14b8a6;
}

.section-title-polls i.fa-fire {
  background: linear-gradient(135deg, #fef2f2 0%, #fee2e2 100%);
  box-shadow: 0 2px 8px rgba(239, 68, 68, 0.25);
  color: #ef4444;
}

.section-title-polls i.fa-bolt {
  background: linear-gradient(135deg, #fefce8 0%, #fef9c3 100%);
  box-shadow: 0 2px 8px rgba(234, 179, 8, 0.25);
  color: #eab308;
}

.section-title-polls i.fa-poll {
  background: linear-gradient(135deg, #eef2ff 0%, #e0e7ff 100%);
  box-shadow: 0 2px 8px rgba(99, 102, 241, 0.25);
  color: #6366f1;
}

.section-header-polls {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 1rem;
}

.section-header-polls:hover .section-title-polls i {
  transform: scale(1.1) rotate(-5deg);
}

/* Filter Bar */
.filter-bar {
  background: linear-gradient(135deg, #f0fdfa 0%, #ccfbf1 50%, #99f6e4 100%);
  border-radius: 1rem;
  padding: 1rem 1.25rem;
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: 1rem;
  border: none;
  margin-bottom: 1.5rem;
}
.search-input-polls {
  flex: 1;
  min-width: 200px;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  background: #f8fafc;
  border: 1px solid #e2e8f0;
  border-radius: 0.75rem;
  padding: 0.625rem 1rem;
  transition: all 0.3s ease;
}
.search-input-polls:focus-within {
  border-color: #14b8a6;
  background: white;
  box-shadow: 0 0 0 3px rgba(20, 184, 166, 0.1);
}
.search-input-polls input {
  flex: 1;
  border: none;
  background: transparent;
  outline: none;
  font-size: 0.875rem;
}
.filter-dropdown {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.625rem 1rem;
  background: #f8fafc;
  border: 1px solid #e2e8f0;
  border-radius: 0.75rem;
  font-size: 0.875rem;
  cursor: pointer;
  transition: all 0.3s ease;
}
.filter-dropdown:hover {
  border-color: #14b8a6;
}

/* Tab Pills */
.tab-pills {
  display: flex;
  background: #f1f5f9;
  border-radius: 0.75rem;
  padding: 0.25rem;
}
.tab-pill {
  padding: 0.625rem 1.25rem;
  border-radius: 0.625rem;
  font-size: 0.875rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}
.tab-pill.active {
  background: white;
  color: #0d9488;
  box-shadow: 0 2px 8px rgba(0,0,0,0.08);
}
.tab-pill:not(.active) {
  color: #64748b;
}
.tab-pill:not(.active):hover {
  color: #0d9488;
}
.tab-count {
  background: #e2e8f0;
  padding: 0.125rem 0.5rem;
  border-radius: 1rem;
  font-size: 0.75rem;
}
.tab-pill.active .tab-count {
  background: #ccfbf1;
  color: #0d9488;
}

/* My Polls Table Enhanced */
.my-polls-card {
  background: white;
  border-radius: 1rem;
  overflow: hidden;
  box-shadow: 0 4px 20px rgba(0,0,0,0.05);
}
.my-poll-row {
  display: flex;
  align-items: center;
  padding: 1rem 1.25rem;
  border-bottom: 1px solid #f1f5f9;
  transition: all 0.2s ease;
}
.my-poll-row:last-child {
  border-bottom: none;
}
.my-poll-row:hover {
  background: #f8fafc;
}
.status-pill {
  padding: 0.25rem 0.75rem;
  border-radius: 2rem;
  font-size: 0.75rem;
  font-weight: 600;
}
.status-pill.active { background: #dcfce7; color: #16a34a; }
.status-pill.draft { background: #fef3c7; color: #d97706; }
.status-pill.completed { background: #e2e8f0; color: #64748b; }
.status-pill.scheduled { background: #dbeafe; color: #2563eb; }

/* Completed Poll */
.completed-poll-card {
  background: white;
  border-radius: 1rem;
  overflow: hidden;
  box-shadow: 0 4px 15px rgba(0,0,0,0.05);
}
.completed-poll-header {
  padding: 1.25rem;
  background: linear-gradient(135deg, #f8fafc, #f0fdfa);
  border-bottom: 1px solid #e2e8f0;
}
.completed-poll-body {
  padding: 1.25rem;
}

/* Line Clamp */
.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

/* Animations */
.fade-in-up {
  animation: fadeInUp 0.5s ease forwards;
  opacity: 0;
}
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

/* Modal Overlay */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 50;
  padding: 1rem;
}
.modal-content {
  background: white;
  border-radius: 1rem;
  width: 100%;
  max-height: 90vh;
  overflow-y: auto;
  box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.25);
}
.modal-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 1.25rem;
  border-bottom: 1px solid #e2e8f0;
}
.modal-close {
  width: 32px;
  height: 32px;
  border-radius: 0.5rem;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #64748b;
  transition: all 0.2s ease;
}
.modal-close:hover {
  background: #f1f5f9;
  color: #0f172a;
}
.modal-body {
  padding: 1.25rem;
}
.modal-footer {
  display: flex;
  justify-content: flex-end;
  gap: 0.75rem;
  padding: 1.25rem;
  border-top: 1px solid #e2e8f0;
}

/* Form Elements */
.form-label {
  display: block;
  font-size: 0.875rem;
  font-weight: 500;
  color: #374151;
  margin-bottom: 0.5rem;
}
.form-input {
  width: 100%;
  padding: 0.625rem 1rem;
  border: 1px solid #e2e8f0;
  border-radius: 0.75rem;
  font-size: 0.875rem;
  transition: all 0.2s ease;
}
.form-input:focus {
  outline: none;
  border-color: #14b8a6;
  box-shadow: 0 0 0 3px rgba(20, 184, 166, 0.1);
}
.checkbox {
  width: 18px;
  height: 18px;
  border-radius: 0.25rem;
  border: 2px solid #cbd5e1;
  accent-color: #14b8a6;
}
</style>
