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
  <div class="min-h-screen bg-gray-50">
    <!-- Loading State -->
    <div v-if="isLoading" class="flex items-center justify-center py-20">
      <LoadingSpinner size="lg" text="Loading polls..." />
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
              <i class="fas fa-poll"></i>
            </div>
            <p class="stat-value-mini">{{ stats.active }}</p>
            <p class="stat-label-mini">Active Polls</p>
          </div>
          <div class="stat-card-square">
            <div class="stat-icon-box">
              <i class="fas fa-vote-yea"></i>
            </div>
            <p class="stat-value-mini">{{ stats.responses.toLocaleString() }}</p>
            <p class="stat-label-mini">Responses</p>
          </div>
          <div class="stat-card-square">
            <div class="stat-icon-box">
              <i class="fas fa-chart-pie"></i>
            </div>
            <p class="stat-value-mini">{{ stats.created }}</p>
            <p class="stat-label-mini">Created</p>
          </div>
          <div class="stat-card-square">
            <div class="stat-icon-box">
              <i class="fas fa-users"></i>
            </div>
            <p class="stat-value-mini">{{ stats.participation }}%</p>
            <p class="stat-label-mini">Participation</p>
          </div>
        </div>

        <div class="relative px-8 py-8">
          <div class="px-3 py-1 bg-white/20 backdrop-blur-sm rounded-full text-white text-xs font-semibold inline-flex items-center gap-2 mb-4">
            <i class="fas fa-poll"></i>
            AFC Asian Cup 2027
          </div>

          <h1 class="text-3xl font-bold text-white mb-2">Polls & Surveys</h1>
          <p class="text-teal-100 max-w-lg">Gather feedback and insights from your team. Create polls, collect responses, and analyze results.</p>

          <div class="flex flex-wrap gap-3 mt-6">
            <button @click="goToCreatePoll" class="px-5 py-2.5 bg-white text-teal-600 rounded-xl font-semibold text-sm flex items-center gap-2 hover:bg-teal-50 transition-all shadow-lg">
              <i class="fas fa-plus"></i>
              Create New Poll
            </button>
            <button @click="showQuickPollModal = true" class="px-5 py-2.5 bg-white/20 backdrop-blur-sm border border-white/30 text-white rounded-xl font-semibold text-sm hover:bg-white/30 transition-all flex items-center gap-2">
              <i class="fas fa-bolt"></i>
              Quick Poll
            </button>
          </div>
        </div>
      </div>

      <!-- Main Content Area -->
      <div class="px-8 py-6 space-y-6">
      <!-- Featured Poll - Premium Design -->
      <section v-if="featuredPoll" class="featured-poll-premium fade-in-up" style="animation-delay: 0.1s">
        <!-- Floating Decorative Elements -->
        <div class="featured-poll-decor">
          <div class="decor-orb decor-orb-1"></div>
          <div class="decor-orb decor-orb-2"></div>
          <div class="decor-orb decor-orb-3"></div>
          <div class="decor-shape decor-shape-1"></div>
          <div class="decor-shape decor-shape-2"></div>
          <div class="decor-pattern"></div>
        </div>

        <div class="featured-poll-content">
          <!-- Left Content -->
          <div class="featured-poll-left">
            <!-- Badges Row -->
            <div class="featured-badges-row">
              <span class="featured-badge-premium">
                <span class="badge-icon-glow">
                  <i class="fas fa-star"></i>
                </span>
                <span>Featured Poll</span>
              </span>
              <span v-if="featuredPoll.anonymous" class="featured-badge-premium secondary">
                <i class="fas fa-user-secret"></i>
                <span>Anonymous</span>
              </span>
              <span class="featured-badge-premium live">
                <span class="live-dot"></span>
                <span>Live</span>
              </span>
            </div>

            <!-- Question -->
            <h2 class="featured-poll-question">{{ featuredPoll.question }}</h2>
            <p class="featured-poll-description">{{ featuredPoll.description }}</p>

            <!-- Stats Row -->
            <div class="featured-stats-row">
              <div class="featured-stat">
                <div class="featured-stat-icon">
                  <i class="fas fa-users"></i>
                </div>
                <div class="featured-stat-content">
                  <span class="featured-stat-value">{{ featuredPoll.totalVotes.toLocaleString() }}</span>
                  <span class="featured-stat-label">responses</span>
                </div>
              </div>
              <div class="featured-stat">
                <div class="featured-stat-icon timer">
                  <i class="fas fa-hourglass-half"></i>
                </div>
                <div class="featured-stat-content">
                  <span class="featured-stat-value countdown">3</span>
                  <span class="featured-stat-label">days left</span>
                </div>
              </div>
              <div class="featured-stat">
                <div class="featured-stat-icon author">
                  <i class="fas fa-crown"></i>
                </div>
                <div class="featured-stat-content">
                  <span class="featured-stat-value">{{ featuredPoll.author }}</span>
                  <span class="featured-stat-label">created by</span>
                </div>
              </div>
            </div>
          </div>

          <!-- Right Content - Voting Options -->
          <div class="featured-poll-right">
            <div class="voting-card">
              <div class="voting-header">
                <span class="voting-title">Cast Your Vote</span>
                <span class="voting-progress">
                  <i class="fas fa-chart-pie"></i>
                  {{ featuredPoll.hasVoted ? 'Voted' : 'Select an option' }}
                </span>
              </div>

              <div class="voting-options">
                <div
                  v-for="(option, idx) in featuredPoll.options"
                  :key="idx"
                  :class="['voting-option', {
                    selected: featuredPoll.selectedOption === idx,
                    voted: featuredPoll.hasVoted,
                    leading: featuredPoll.hasVoted && option.percentage >= 40
                  }]"
                  @click="selectFeaturedOption(idx)"
                >
                  <!-- Progress Bar Background -->
                  <div
                    v-if="featuredPoll.hasVoted"
                    class="voting-progress-bar"
                    :style="{ width: option.percentage + '%' }"
                  ></div>

                  <!-- Option Content -->
                  <div class="voting-option-content">
                    <div class="voting-radio">
                      <div class="radio-inner"></div>
                    </div>
                    <span class="voting-label">{{ option.label }}</span>
                    <div v-if="featuredPoll.hasVoted" class="voting-percentage">
                      <span class="percentage-value">{{ option.percentage }}</span>
                      <span class="percentage-sign">%</span>
                    </div>
                    <div v-if="featuredPoll.hasVoted && option.percentage >= 40" class="leading-badge">
                      <i class="fas fa-crown"></i>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Submit Button -->
              <button
                v-if="!featuredPoll.hasVoted"
                :disabled="featuredPoll.selectedOption === null"
                class="voting-submit-btn"
              >
                <span class="btn-bg"></span>
                <span class="btn-content">
                  <i class="fas fa-paper-plane"></i>
                  <span>Submit Vote</span>
                </span>
              </button>

              <!-- Voted Confirmation -->
              <div v-else class="voting-confirmed">
                <div class="confirmed-icon">
                  <i class="fas fa-check-circle"></i>
                </div>
                <span>Your vote has been recorded</span>
              </div>
            </div>
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

      <!-- Trending Polls - Enhanced -->
      <section class="trending-section mb-10 fade-in-up" style="animation-delay: 0.25s">
        <!-- Section Header -->
        <div class="trending-header">
          <div class="trending-title-group">
            <div class="trending-icon-wrapper">
              <i class="fas fa-fire"></i>
              <div class="fire-glow"></div>
            </div>
            <div>
              <h2 class="trending-main-title">Trending Now</h2>
              <p class="trending-subtitle">Most popular polls this week</p>
            </div>
          </div>
          <a href="#" class="trending-view-all">
            View All
            <i class="fas fa-arrow-right"></i>
          </a>
        </div>

        <!-- Trending Grid -->
        <div class="trending-grid">
          <div
            v-for="(poll, index) in trendingPolls"
            :key="poll.id"
            :class="['trending-card-enhanced', { 'top-three': index < 3 }]"
          >
            <!-- Rank Badge -->
            <div :class="['rank-badge', `rank-${index + 1}`]">
              <span v-if="index < 3" class="rank-icon">
                <i :class="index === 0 ? 'fas fa-crown' : index === 1 ? 'fas fa-medal' : 'fas fa-award'"></i>
              </span>
              <span v-else class="rank-number">#{{ index + 1 }}</span>
            </div>

            <!-- Card Content -->
            <div class="trending-card-content">
              <h4 class="trending-question">{{ poll.question }}</h4>

              <!-- Stats Row -->
              <div class="trending-stats">
                <div class="trending-stat">
                  <i class="fas fa-users"></i>
                  <span>{{ poll.totalVotes.toLocaleString() }}</span>
                </div>
                <div class="trending-stat growth">
                  <i class="fas fa-arrow-trend-up"></i>
                  <span>+{{ poll.weeklyGrowth }}%</span>
                </div>
              </div>
            </div>

            <!-- Vote Now Button -->
            <button class="trending-vote-btn">
              <span class="btn-text">Vote Now</span>
              <i class="fas fa-arrow-right"></i>
            </button>
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
/* ============================================
   Hero Section - Documents Style
   ============================================ */
.hero-gradient {
  background: linear-gradient(135deg, #0d9488 0%, #14b8a6 50%, #10b981 100%);
}

.stats-top-right {
  position: absolute;
  top: 24px;
  right: 32px;
  display: flex;
  align-items: flex-start;
  gap: 12px;
  z-index: 10;
}

.stat-card-square {
  background: rgba(255, 255, 255, 0.15);
  backdrop-filter: blur(8px);
  border-radius: 16px;
  width: 115px;
  height: 115px;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 6px;
  transition: all 0.3s ease;
  border: 1px solid rgba(255, 255, 255, 0.1);
  cursor: default;
}

.stat-card-square:hover {
  background: rgba(255, 255, 255, 0.25);
  transform: translateY(-4px) scale(1.02);
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.15);
}

.stat-card-square:hover .stat-icon-box {
  transform: scale(1.1);
}

.stat-icon-box {
  color: white;
  font-size: 1.25rem;
  transition: transform 0.3s ease;
}

.stat-value-mini {
  font-size: 1.5rem;
  font-weight: 700;
  color: white;
  line-height: 1;
}

.stat-label-mini {
  font-size: 0.6875rem;
  color: rgba(255, 255, 255, 0.8);
  text-transform: uppercase;
  letter-spacing: 0.05em;
  font-weight: 500;
}

/* Circle drift animations */
.circle-drift-1 {
  animation: drift-1 20s ease-in-out infinite;
}

.circle-drift-2 {
  animation: drift-2 25s ease-in-out infinite;
}

.circle-drift-3 {
  animation: drift-3 18s ease-in-out infinite;
}

@keyframes drift-1 {
  0%, 100% {
    transform: translate(0, 0) scale(1);
  }
  25% {
    transform: translate(-30px, 20px) scale(1.05);
  }
  50% {
    transform: translate(-20px, 40px) scale(0.95);
  }
  75% {
    transform: translate(10px, 20px) scale(1.02);
  }
}

@keyframes drift-2 {
  0%, 100% {
    transform: translate(0, 0) scale(1);
  }
  33% {
    transform: translate(40px, -30px) scale(1.1);
  }
  66% {
    transform: translate(20px, -50px) scale(0.9);
  }
}

@keyframes drift-3 {
  0%, 100% {
    transform: translate(0, 0) scale(1);
  }
  50% {
    transform: translate(-20px, -20px) scale(1.15);
  }
}

/* Responsive adjustments */
@media (max-width: 1024px) {
  .stats-top-right {
    position: relative;
    top: auto;
    right: auto;
    margin: 24px 32px 0;
    display: grid;
    grid-template-columns: repeat(4, 1fr);
    gap: 12px;
  }

  .stat-card-square {
    width: 100%;
    height: 80px;
  }

  .stat-icon-box {
    font-size: 1rem;
  }

  .stat-value-mini {
    font-size: 1.25rem;
  }
}

@media (max-width: 640px) {
  .stats-top-right {
    grid-template-columns: repeat(2, 1fr);
    margin: 16px 24px 0;
  }
}

/* ============================================
   Featured Poll - Premium Design
   ============================================ */
.featured-poll-premium {
  position: relative;
  background: linear-gradient(135deg, #0d9488 0%, #0f766e 40%, #115e59 100%);
  border-radius: 2rem;
  padding: 2.5rem;
  color: white;
  overflow: hidden;
  box-shadow:
    0 25px 50px -12px rgba(13, 148, 136, 0.4),
    0 0 0 1px rgba(255, 255, 255, 0.1) inset;
}

/* Decorative Elements */
.featured-poll-decor {
  position: absolute;
  inset: 0;
  overflow: hidden;
  pointer-events: none;
}

.decor-orb {
  position: absolute;
  border-radius: 50%;
  filter: blur(60px);
  opacity: 0.4;
  animation: float 8s ease-in-out infinite;
}

.decor-orb-1 {
  width: 300px;
  height: 300px;
  background: radial-gradient(circle, #5eead4 0%, transparent 70%);
  top: -100px;
  right: -50px;
  animation-delay: 0s;
}

.decor-orb-2 {
  width: 200px;
  height: 200px;
  background: radial-gradient(circle, #2dd4bf 0%, transparent 70%);
  bottom: -80px;
  left: 10%;
  animation-delay: 2s;
}

.decor-orb-3 {
  width: 150px;
  height: 150px;
  background: radial-gradient(circle, #99f6e4 0%, transparent 70%);
  top: 50%;
  left: 40%;
  animation-delay: 4s;
}

.decor-shape {
  position: absolute;
  border: 2px solid rgba(255, 255, 255, 0.1);
  border-radius: 1rem;
  transform: rotate(45deg);
}

.decor-shape-1 {
  width: 80px;
  height: 80px;
  top: 20%;
  right: 15%;
  animation: spin-slow 20s linear infinite;
}

.decor-shape-2 {
  width: 40px;
  height: 40px;
  bottom: 30%;
  left: 5%;
  animation: spin-slow 15s linear infinite reverse;
}

.decor-pattern {
  position: absolute;
  inset: 0;
  background-image:
    radial-gradient(circle at 20% 80%, rgba(255,255,255,0.03) 1px, transparent 1px),
    radial-gradient(circle at 80% 20%, rgba(255,255,255,0.03) 1px, transparent 1px);
  background-size: 60px 60px;
}

@keyframes float {
  0%, 100% { transform: translateY(0) scale(1); }
  50% { transform: translateY(-20px) scale(1.05); }
}

@keyframes spin-slow {
  from { transform: rotate(45deg); }
  to { transform: rotate(405deg); }
}

/* Content Layout */
.featured-poll-content {
  position: relative;
  z-index: 1;
  display: flex;
  flex-direction: column;
  gap: 2rem;
}

@media (min-width: 1024px) {
  .featured-poll-content {
    flex-direction: row;
    align-items: stretch;
  }
}

/* Left Section */
.featured-poll-left {
  flex: 1;
  display: flex;
  flex-direction: column;
}

/* Badges Row */
.featured-badges-row {
  display: flex;
  flex-wrap: wrap;
  gap: 0.75rem;
  margin-bottom: 1.5rem;
}

.featured-badge-premium {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  background: rgba(255, 255, 255, 0.15);
  backdrop-filter: blur(10px);
  padding: 0.5rem 1rem;
  border-radius: 2rem;
  font-size: 0.8125rem;
  font-weight: 600;
  border: 1px solid rgba(255, 255, 255, 0.2);
  transition: all 0.3s ease;
}

.featured-badge-premium:hover {
  background: rgba(255, 255, 255, 0.25);
  transform: translateY(-2px);
}

.featured-badge-premium .badge-icon-glow {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 20px;
  height: 20px;
  background: linear-gradient(135deg, #fbbf24, #f59e0b);
  border-radius: 50%;
  font-size: 0.625rem;
  color: white;
  box-shadow: 0 0 10px rgba(251, 191, 36, 0.5);
  animation: pulse-glow 2s ease-in-out infinite;
}

@keyframes pulse-glow {
  0%, 100% { box-shadow: 0 0 10px rgba(251, 191, 36, 0.5); }
  50% { box-shadow: 0 0 20px rgba(251, 191, 36, 0.8); }
}

.featured-badge-premium.secondary {
  background: rgba(148, 163, 184, 0.2);
}

.featured-badge-premium.live {
  background: rgba(239, 68, 68, 0.2);
  border-color: rgba(239, 68, 68, 0.3);
}

.live-dot {
  width: 8px;
  height: 8px;
  background: #ef4444;
  border-radius: 50%;
  animation: pulse-live 1.5s ease-in-out infinite;
}

@keyframes pulse-live {
  0%, 100% { opacity: 1; transform: scale(1); }
  50% { opacity: 0.5; transform: scale(1.2); }
}

/* Question */
.featured-poll-question {
  font-size: 1.75rem;
  font-weight: 800;
  line-height: 1.3;
  margin-bottom: 0.75rem;
  text-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
}

@media (min-width: 1024px) {
  .featured-poll-question {
    font-size: 2rem;
  }
}

.featured-poll-description {
  font-size: 1rem;
  color: rgba(255, 255, 255, 0.8);
  line-height: 1.6;
  margin-bottom: 2rem;
  max-width: 500px;
}

/* Stats Row */
.featured-stats-row {
  display: flex;
  flex-wrap: wrap;
  gap: 1.5rem;
  margin-top: auto;
}

.featured-stat {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.featured-stat-icon {
  width: 44px;
  height: 44px;
  background: rgba(255, 255, 255, 0.15);
  backdrop-filter: blur(10px);
  border-radius: 0.75rem;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1rem;
  border: 1px solid rgba(255, 255, 255, 0.1);
}

.featured-stat-icon.timer {
  background: rgba(251, 191, 36, 0.2);
  color: #fcd34d;
}

.featured-stat-icon.timer i {
  animation: hourglass 3s ease-in-out infinite;
}

@keyframes hourglass {
  0%, 100% { transform: rotate(0deg); }
  25% { transform: rotate(180deg); }
  50% { transform: rotate(180deg); }
  75% { transform: rotate(360deg); }
}

.featured-stat-icon.author {
  background: rgba(167, 139, 250, 0.2);
  color: #c4b5fd;
}

.featured-stat-content {
  display: flex;
  flex-direction: column;
}

.featured-stat-value {
  font-size: 1.125rem;
  font-weight: 700;
}

.featured-stat-value.countdown {
  font-size: 1.5rem;
  font-weight: 800;
  color: #fcd34d;
}

.featured-stat-label {
  font-size: 0.75rem;
  color: rgba(255, 255, 255, 0.6);
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

/* Right Section - Voting Card */
.featured-poll-right {
  flex: 0 0 360px;
}

.voting-card {
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(20px);
  border-radius: 1.5rem;
  padding: 1.5rem;
  border: 1px solid rgba(255, 255, 255, 0.15);
}

.voting-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 1.25rem;
  padding-bottom: 1rem;
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
}

.voting-title {
  font-size: 1.125rem;
  font-weight: 700;
}

.voting-progress {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.8125rem;
  color: rgba(255, 255, 255, 0.7);
}

/* Voting Options */
.voting-options {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.voting-option {
  position: relative;
  background: rgba(255, 255, 255, 0.08);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 1rem;
  padding: 1rem 1.25rem;
  cursor: pointer;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  overflow: hidden;
}

.voting-option:hover:not(.voted) {
  background: rgba(255, 255, 255, 0.15);
  border-color: rgba(255, 255, 255, 0.3);
  transform: translateX(4px);
}

.voting-option.selected:not(.voted) {
  background: rgba(255, 255, 255, 0.2);
  border-color: rgba(255, 255, 255, 0.5);
  box-shadow: 0 0 20px rgba(255, 255, 255, 0.1);
}

.voting-option.voted {
  cursor: default;
}

.voting-option.leading {
  border-color: rgba(251, 191, 36, 0.5);
  background: rgba(251, 191, 36, 0.1);
}

/* Progress Bar */
.voting-progress-bar {
  position: absolute;
  left: 0;
  top: 0;
  bottom: 0;
  background: linear-gradient(90deg, rgba(94, 234, 212, 0.3), rgba(45, 212, 191, 0.2));
  border-radius: 1rem;
  transition: width 0.8s cubic-bezier(0.4, 0, 0.2, 1);
}

.voting-option.leading .voting-progress-bar {
  background: linear-gradient(90deg, rgba(251, 191, 36, 0.3), rgba(245, 158, 11, 0.2));
}

/* Option Content */
.voting-option-content {
  position: relative;
  z-index: 1;
  display: flex;
  align-items: center;
  gap: 0.875rem;
}

.voting-radio {
  width: 22px;
  height: 22px;
  border: 2px solid rgba(255, 255, 255, 0.4);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
  transition: all 0.3s ease;
}

.voting-option:hover:not(.voted) .voting-radio {
  border-color: rgba(255, 255, 255, 0.7);
}

.voting-option.selected .voting-radio {
  border-color: white;
  background: white;
}

.voting-radio .radio-inner {
  width: 10px;
  height: 10px;
  background: transparent;
  border-radius: 50%;
  transition: all 0.3s ease;
}

.voting-option.selected .radio-inner {
  background: #0d9488;
}

.voting-option.voted .voting-radio {
  border-color: rgba(255, 255, 255, 0.3);
}

.voting-option.voted.selected .voting-radio {
  border-color: rgba(94, 234, 212, 0.8);
  background: rgba(94, 234, 212, 0.3);
}

.voting-option.voted.selected .radio-inner {
  background: #5eead4;
}

.voting-label {
  flex: 1;
  font-size: 0.9375rem;
  font-weight: 500;
}

.voting-percentage {
  display: flex;
  align-items: baseline;
  font-weight: 700;
}

.percentage-value {
  font-size: 1.25rem;
}

.percentage-sign {
  font-size: 0.875rem;
  opacity: 0.7;
}

.leading-badge {
  width: 28px;
  height: 28px;
  background: linear-gradient(135deg, #fbbf24, #f59e0b);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.75rem;
  color: white;
  box-shadow: 0 2px 10px rgba(251, 191, 36, 0.4);
  animation: crown-bounce 2s ease-in-out infinite;
}

@keyframes crown-bounce {
  0%, 100% { transform: translateY(0); }
  50% { transform: translateY(-3px); }
}

/* Submit Button */
.voting-submit-btn {
  position: relative;
  width: 100%;
  margin-top: 1.25rem;
  padding: 1rem 1.5rem;
  background: transparent;
  border: none;
  border-radius: 1rem;
  cursor: pointer;
  overflow: hidden;
  transition: all 0.3s ease;
}

.voting-submit-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.voting-submit-btn .btn-bg {
  position: absolute;
  inset: 0;
  background: linear-gradient(135deg, #ffffff 0%, #f0fdfa 100%);
  border-radius: 1rem;
  transition: all 0.3s ease;
}

.voting-submit-btn:not(:disabled):hover .btn-bg {
  transform: scale(1.02);
  box-shadow: 0 10px 30px rgba(255, 255, 255, 0.3);
}

.voting-submit-btn .btn-content {
  position: relative;
  z-index: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.625rem;
  color: #0d9488;
  font-size: 1rem;
  font-weight: 700;
}

.voting-submit-btn:not(:disabled):hover .btn-content i {
  animation: fly 0.5s ease-in-out;
}

@keyframes fly {
  0%, 100% { transform: translateX(0) translateY(0); }
  50% { transform: translateX(3px) translateY(-3px); }
}

/* Voted Confirmation */
.voting-confirmed {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.75rem;
  margin-top: 1.25rem;
  padding: 1rem;
  background: rgba(94, 234, 212, 0.15);
  border-radius: 1rem;
  border: 1px solid rgba(94, 234, 212, 0.3);
  font-size: 0.9375rem;
  font-weight: 500;
  color: #5eead4;
}

.confirmed-icon {
  width: 32px;
  height: 32px;
  background: rgba(94, 234, 212, 0.2);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1rem;
  animation: check-pop 0.5s ease-out;
}

@keyframes check-pop {
  0% { transform: scale(0); }
  50% { transform: scale(1.2); }
  100% { transform: scale(1); }
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

/* ============================================
   Trending Section - Enhanced
   ============================================ */
.trending-section {
  background: linear-gradient(135deg, #f0fdfa 0%, #ccfbf1 50%, #f0fdfa 100%);
  border-radius: 1.5rem;
  padding: 2rem;
  border: 1px solid rgba(20, 184, 166, 0.1);
}

.trending-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 1.5rem;
}

.trending-title-group {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.trending-icon-wrapper {
  position: relative;
  width: 48px;
  height: 48px;
  background: linear-gradient(135deg, #f97316 0%, #ea580c 100%);
  border-radius: 1rem;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.25rem;
  color: white;
  box-shadow: 0 4px 15px rgba(249, 115, 22, 0.4);
}

.trending-icon-wrapper i {
  animation: flame-flicker 1.5s ease-in-out infinite;
  position: relative;
  z-index: 1;
}

.fire-glow {
  position: absolute;
  inset: -4px;
  background: radial-gradient(circle, rgba(249, 115, 22, 0.4) 0%, transparent 70%);
  border-radius: 1rem;
  animation: glow-pulse 2s ease-in-out infinite;
}

@keyframes flame-flicker {
  0%, 100% { transform: scale(1) rotate(0deg); }
  25% { transform: scale(1.1) rotate(-3deg); }
  50% { transform: scale(1) rotate(3deg); }
  75% { transform: scale(1.05) rotate(-2deg); }
}

@keyframes glow-pulse {
  0%, 100% { opacity: 0.5; transform: scale(1); }
  50% { opacity: 1; transform: scale(1.1); }
}

.trending-main-title {
  font-size: 1.375rem;
  font-weight: 700;
  color: #1f2937;
  margin: 0;
}

.trending-subtitle {
  font-size: 0.875rem;
  color: #6b7280;
  margin: 0.25rem 0 0;
}

.trending-view-all {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.875rem;
  font-weight: 600;
  color: #0d9488;
  text-decoration: none;
  padding: 0.625rem 1rem;
  border-radius: 0.75rem;
  background: rgba(20, 184, 166, 0.1);
  transition: all 0.3s ease;
}

.trending-view-all:hover {
  background: rgba(20, 184, 166, 0.2);
  transform: translateX(4px);
}

.trending-view-all i {
  transition: transform 0.3s ease;
}

.trending-view-all:hover i {
  transform: translateX(4px);
}

/* Trending Grid */
.trending-grid {
  display: grid;
  grid-template-columns: repeat(1, 1fr);
  gap: 1rem;
}

@media (min-width: 768px) {
  .trending-grid {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (min-width: 1024px) {
  .trending-grid {
    grid-template-columns: repeat(3, 1fr);
  }
}

/* Trending Card */
.trending-card-enhanced {
  position: relative;
  background: white;
  border-radius: 1.25rem;
  padding: 1.25rem;
  display: flex;
  align-items: center;
  gap: 1rem;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.05);
  border: 1px solid rgba(0, 0, 0, 0.04);
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  cursor: pointer;
  overflow: hidden;
}

.trending-card-enhanced::before {
  content: '';
  position: absolute;
  inset: 0;
  background: linear-gradient(135deg, transparent 0%, rgba(20, 184, 166, 0.03) 100%);
  opacity: 0;
  transition: opacity 0.3s ease;
}

.trending-card-enhanced:hover {
  transform: translateY(-4px);
  box-shadow: 0 12px 30px rgba(20, 184, 166, 0.15);
  border-color: rgba(20, 184, 166, 0.2);
}

.trending-card-enhanced:hover::before {
  opacity: 1;
}

.trending-card-enhanced.top-three {
  border-left: 3px solid;
}

.trending-card-enhanced.top-three:nth-child(1) {
  border-left-color: #0d9488;
  background: linear-gradient(135deg, #f0fdfa 0%, white 30%);
}

.trending-card-enhanced.top-three:nth-child(2) {
  border-left-color: #14b8a6;
  background: linear-gradient(135deg, #f0fdfa 0%, white 30%);
}

.trending-card-enhanced.top-three:nth-child(3) {
  border-left-color: #2dd4bf;
  background: linear-gradient(135deg, #f0fdfa 0%, white 30%);
}

/* Rank Badge */
.rank-badge {
  flex-shrink: 0;
  width: 40px;
  height: 40px;
  border-radius: 0.75rem;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 700;
  transition: all 0.3s ease;
}

.rank-badge.rank-1 {
  background: linear-gradient(135deg, #0d9488 0%, #0f766e 100%);
  color: white;
  box-shadow: 0 4px 12px rgba(13, 148, 136, 0.4);
}

.rank-badge.rank-1 i {
  animation: crown-shine 2s ease-in-out infinite;
}

@keyframes crown-shine {
  0%, 100% { filter: drop-shadow(0 0 0 transparent); }
  50% { filter: drop-shadow(0 0 6px rgba(255, 255, 255, 0.8)); }
}

.rank-badge.rank-2 {
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.4);
}

.rank-badge.rank-3 {
  background: linear-gradient(135deg, #2dd4bf 0%, #14b8a6 100%);
  color: white;
  box-shadow: 0 4px 12px rgba(45, 212, 191, 0.4);
}

.rank-badge.rank-4,
.rank-badge.rank-5,
.rank-badge.rank-6 {
  background: #f0fdfa;
  color: #0d9488;
}

.rank-icon {
  font-size: 1rem;
}

.rank-number {
  font-size: 0.875rem;
}

.trending-card-enhanced:hover .rank-badge {
  transform: scale(1.1) rotate(-5deg);
}

/* Card Content */
.trending-card-content {
  flex: 1;
  min-width: 0;
  position: relative;
  z-index: 1;
}

.trending-question {
  font-size: 0.9375rem;
  font-weight: 600;
  color: #1f2937;
  margin: 0 0 0.5rem;
  line-height: 1.4;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
  transition: color 0.3s ease;
}

.trending-card-enhanced:hover .trending-question {
  color: #0d9488;
}

.trending-stats {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.trending-stat {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  font-size: 0.75rem;
  color: #6b7280;
}

.trending-stat i {
  font-size: 0.6875rem;
}

.trending-stat.growth {
  color: #10b981;
  font-weight: 600;
}

.trending-stat.growth i {
  animation: trend-up 1s ease-in-out infinite;
}

@keyframes trend-up {
  0%, 100% { transform: translateY(0); }
  50% { transform: translateY(-2px); }
}

/* Progress Circle */
.trending-progress {
  position: relative;
  flex-shrink: 0;
  width: 56px;
  height: 56px;
  z-index: 1;
}

.trending-progress svg {
  transform: rotate(-90deg);
}

.trending-progress .progress-bg {
  fill: none;
  stroke: #f3f4f6;
  stroke-width: 4;
}

.trending-progress .progress-ring {
  fill: none;
  stroke: #14b8a6;
  stroke-width: 4;
  stroke-linecap: round;
  transition: stroke-dashoffset 0.8s cubic-bezier(0.4, 0, 0.2, 1);
}

.trending-progress .progress-ring.high-rate {
  stroke: #10b981;
}

.progress-value {
  position: absolute;
  inset: 0;
  display: flex;
  align-items: center;
  justify-content: center;
}

.value-number {
  font-size: 1rem;
  font-weight: 700;
  color: #1f2937;
}

.value-percent {
  font-size: 0.625rem;
  font-weight: 600;
  color: #9ca3af;
}

/* Vote Now Button */
.trending-vote-btn {
  flex-shrink: 0;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.625rem 1rem;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
  border: none;
  border-radius: 0.75rem;
  font-size: 0.8125rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.3);
  position: relative;
  z-index: 1;
  overflow: hidden;
}

.trending-vote-btn::before {
  content: '';
  position: absolute;
  inset: 0;
  background: linear-gradient(135deg, #0d9488 0%, #0f766e 100%);
  opacity: 0;
  transition: opacity 0.3s ease;
}

.trending-vote-btn:hover {
  transform: scale(1.05);
  box-shadow: 0 6px 20px rgba(20, 184, 166, 0.4);
}

.trending-vote-btn:hover::before {
  opacity: 1;
}

.trending-vote-btn .btn-text,
.trending-vote-btn i {
  position: relative;
  z-index: 1;
}

.trending-vote-btn i {
  font-size: 0.75rem;
  transition: transform 0.3s ease;
}

.trending-vote-btn:hover i {
  transform: translateX(3px);
}

.trending-vote-btn:active {
  transform: scale(0.98);
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
