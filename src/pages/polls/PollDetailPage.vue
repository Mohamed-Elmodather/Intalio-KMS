<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useComments } from '@/composables/useComments'
import { useRelatedContent } from '@/composables/useRelatedContent'
import { useSharing } from '@/composables/useSharing'
import { CommentsSection, SocialShareButtons, RelatedContentCarousel, AuthorCard, BookmarkButton, AddToCollectionModal } from '@/components/common'
import type { Author } from '@/types/detail-pages'

// Text constants for localization
const textConstants = {
  // Loading
  loadingPoll: 'Loading poll...',

  // Navigation
  back: 'Back',
  polls: 'Polls',

  // Status
  statusActive: 'Active',
  statusScheduled: 'Scheduled',
  statusCompleted: 'Completed',
  statusDraft: 'Draft',
  anonymous: 'Anonymous',

  // Stats
  votes: 'votes',
  participation: 'participation',
  options: 'options',

  // Voting
  castYourVote: 'Cast Your Vote',
  results: 'Results',
  pollOptions: 'Poll Options',
  yourChoice: 'Your choice',
  leading: 'Leading',
  submitVote: 'Submit Vote',
  submitting: 'Submitting...',
  selectOption: 'Please select an option first',

  // Time
  ended: 'Ended',
  remaining: 'remaining',
  daysRemaining: 'd remaining',
  hoursRemaining: 'h remaining',
  minutesRemaining: 'm remaining',

  // Analytics
  aiSentimentAnalysis: 'AI Sentiment Analysis',
  sentimentInsights: 'Real-time sentiment analysis of poll responses',
  voteDistribution: 'Vote Distribution',
  votesOverTime: 'Votes over the past week',

  // Poll Details Sidebar
  pollDetails: 'Poll Details',
  targetAudience: 'Target Audience',
  resultsVisibility: 'Results Visibility',
  always: 'Always visible',
  afterVote: 'After voting',
  afterEnd: 'After poll ends',
  multipleAnswers: 'Multiple Answers',
  allowed: 'Allowed',
  notAllowed: 'Not allowed',
  pollType: 'Poll Type',
  anonymousPoll: 'Anonymous Poll',
  publicPoll: 'Public Poll',

  // Author
  aboutAuthor: 'About the Author',

  // Tags
  tags: 'Tags',

  // Header Actions
  download: 'Export',
  collection: 'Collection',
  share: 'Share',

  // Share
  sharePoll: 'Share this Poll',
  copyLink: 'Copy Link',
  linkCopied: 'Link copied to clipboard!',

  // Related
  relatedPolls: 'Related Polls',

  // Comments
  discussion: 'Discussion',
  comments: 'comments',

  // CTA
  createPollCTA: 'Want to create your own poll?',
  createPollDesc: 'Gather feedback from your team with custom polls and surveys.',
  createNewPoll: 'Create New Poll',
}

const route = useRoute()
const router = useRouter()

// Poll interfaces
interface PollOption {
  id: string
  text: string
  votes: number
  percentage: number
  sentiment?: {
    label: string
    score: number
    color: string
  }
}

interface Poll {
  id: string
  question: string
  description: string
  options: PollOption[]
  totalVotes: number
  author: Author
  category: string
  categoryIcon: string
  status: 'active' | 'scheduled' | 'completed' | 'draft'
  isAnonymous: boolean
  allowMultiple: boolean
  showResults: 'always' | 'after_vote' | 'after_end'
  createdAt: Date
  startDate: Date
  endDate: Date
  hasVoted: boolean
  userVote: string | null
  participationRate: number
  targetAudience: string
  tags: string[]
}

// State
const poll = ref<Poll | null>(null)
const isLoading = ref(true)
const isVoting = ref(false)
const selectedOption = ref<string | null>(null)
const showShareModal = ref(false)
const showAnalytics = ref(false)
const showAddToCollection = ref(false)

// Comments
const {
  comments,
  isLoading: commentsLoading,
  loadComments,
  addComment
} = useComments('poll', route.params.id as string)

// Related content
const {
  relatedItems,
  loadRelatedContent,
  isLoading: relatedLoading
} = useRelatedContent('poll', route.params.id as string)

// Sharing
const { copyLink, shareToTwitter, shareToLinkedIn } = useSharing()

// Computed
const pollId = computed(() => route.params.id as string)

const statusConfig = computed(() => {
  if (!poll.value) return { label: '', class: '', icon: '' }
  const configs: Record<string, { label: string; class: string; icon: string }> = {
    active: { label: 'Active', class: 'bg-green-100 text-green-700', icon: 'fas fa-play-circle' },
    scheduled: { label: 'Scheduled', class: 'bg-blue-100 text-blue-700', icon: 'fas fa-calendar-alt' },
    completed: { label: 'Completed', class: 'bg-gray-100 text-gray-700', icon: 'fas fa-check-circle' },
    draft: { label: 'Draft', class: 'bg-yellow-100 text-yellow-700', icon: 'fas fa-file-alt' }
  }
  return configs[poll.value.status] || configs.draft
})

const timeRemaining = computed(() => {
  if (!poll.value || poll.value.status === 'completed') return null
  const now = new Date()
  const end = new Date(poll.value.endDate)
  const diff = end.getTime() - now.getTime()

  if (diff <= 0) return 'Ended'

  const days = Math.floor(diff / (1000 * 60 * 60 * 24))
  const hours = Math.floor((diff % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60))
  const minutes = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60))

  if (days > 0) return `${days}d ${hours}h remaining`
  if (hours > 0) return `${hours}h ${minutes}m remaining`
  return `${minutes}m remaining`
})

const canVote = computed(() => {
  if (!poll.value) return false
  return poll.value.status === 'active' && !poll.value.hasVoted
})

const canSeeResults = computed(() => {
  if (!poll.value) return false
  if (poll.value.showResults === 'always') return true
  if (poll.value.showResults === 'after_vote' && poll.value.hasVoted) return true
  if (poll.value.showResults === 'after_end' && poll.value.status === 'completed') return true
  return false
})

const winningOption = computed(() => {
  if (!poll.value || poll.value.options.length === 0) return null
  return poll.value.options.reduce((max, opt) => opt.votes > max.votes ? opt : max)
})

// Vote distribution chart data
const voteDistribution = computed(() => {
  if (!poll.value) return []
  const now = new Date()
  const start = new Date(poll.value.createdAt)
  const totalDays = Math.ceil((now.getTime() - start.getTime()) / (1000 * 60 * 60 * 24))

  // Generate mock daily distribution
  const data = []
  for (let i = 0; i < Math.min(totalDays, 7); i++) {
    const date = new Date(start)
    date.setDate(date.getDate() + i)
    data.push({
      date: date.toLocaleDateString('en-US', { weekday: 'short' }),
      votes: Math.floor(Math.random() * (poll.value.totalVotes / 3)) + 10
    })
  }
  return data
})

const maxVotes = computed(() => {
  return Math.max(...voteDistribution.value.map(d => d.votes), 1)
})

// Mock data loader
async function loadPoll() {
  isLoading.value = true
  try {
    await new Promise(resolve => setTimeout(resolve, 800))

    poll.value = {
      id: pollId.value,
      question: 'What should be our top priority for Q1 2025?',
      description: 'Help us decide the company\'s main focus for the upcoming quarter. Your input directly influences our strategic planning and resource allocation. We value every team member\'s perspective in shaping our future direction.',
      options: [
        {
          id: '1',
          text: 'Product Innovation',
          votes: 234,
          percentage: 42,
          sentiment: { label: 'Very Positive', score: 0.85, color: 'text-green-600' }
        },
        {
          id: '2',
          text: 'Customer Experience',
          votes: 173,
          percentage: 31,
          sentiment: { label: 'Positive', score: 0.72, color: 'text-green-500' }
        },
        {
          id: '3',
          text: 'Team Growth & Development',
          votes: 100,
          percentage: 18,
          sentiment: { label: 'Neutral', score: 0.55, color: 'text-gray-500' }
        },
        {
          id: '4',
          text: 'Market Expansion',
          votes: 50,
          percentage: 9,
          sentiment: { label: 'Positive', score: 0.65, color: 'text-green-500' }
        }
      ],
      totalVotes: 557,
      author: {
        id: 'u1',
        name: 'Sarah Johnson',
        initials: 'SJ',
        role: 'Head of Strategy',
        department: 'Executive Office',
        avatar: '',
        articlesCount: 12,
        followersCount: 234,
        isFollowing: false
      },
      category: 'Company Strategy',
      categoryIcon: 'fas fa-building',
      status: 'active',
      isAnonymous: false,
      allowMultiple: false,
      showResults: 'after_vote',
      createdAt: new Date(Date.now() - 5 * 24 * 60 * 60 * 1000),
      startDate: new Date(Date.now() - 5 * 24 * 60 * 60 * 1000),
      endDate: new Date(Date.now() + 3 * 24 * 60 * 60 * 1000),
      hasVoted: false,
      userVote: null,
      participationRate: 68,
      targetAudience: 'All Employees',
      tags: ['Strategy', 'Q1 2025', 'Planning', 'Company-Wide']
    }

    // Load comments and related
    await Promise.all([
      loadComments(),
      loadRelatedContent(4)
    ])
  } finally {
    isLoading.value = false
  }
}

async function submitVote() {
  if (!selectedOption.value || !poll.value || isVoting.value) return

  isVoting.value = true
  try {
    await new Promise(resolve => setTimeout(resolve, 600))

    // Update the poll with vote
    const optionIndex = poll.value.options.findIndex(o => o.id === selectedOption.value)
    if (optionIndex >= 0) {
      poll.value.options[optionIndex].votes++
      poll.value.totalVotes++

      // Recalculate percentages
      poll.value.options.forEach(opt => {
        opt.percentage = Math.round((opt.votes / poll.value!.totalVotes) * 100)
      })
    }

    poll.value.hasVoted = true
    poll.value.userVote = selectedOption.value
  } finally {
    isVoting.value = false
  }
}

function selectOption(optionId: string) {
  if (!canVote.value) return
  selectedOption.value = selectedOption.value === optionId ? null : optionId
}

function formatDate(date: Date): string {
  return new Date(date).toLocaleDateString('en-US', {
    month: 'long',
    day: 'numeric',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

function getBarColor(index: number): string {
  const colors = [
    'bg-teal-500',
    'bg-blue-500',
    'bg-purple-500',
    'bg-orange-500',
    'bg-pink-500',
    'bg-indigo-500'
  ]
  return colors[index % colors.length]
}

function goBack() {
  router.back()
}

function exportPoll() {
  if (!poll.value) return

  // Create CSV content for poll results
  const headers = ['Option', 'Votes', 'Percentage']
  const rows = poll.value.options.map(opt => [opt.text, opt.votes, `${opt.percentage}%`])

  const csvContent = [
    `Poll: ${poll.value.question}`,
    `Total Votes: ${poll.value.totalVotes}`,
    `Status: ${poll.value.status}`,
    '',
    headers.join(','),
    ...rows.map(row => row.join(','))
  ].join('\n')

  const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' })
  const link = document.createElement('a')
  link.href = URL.createObjectURL(blob)
  link.download = `poll-${poll.value.id}-results.csv`
  link.click()
  URL.revokeObjectURL(link.href)
}

function sharePoll() {
  if (!poll.value) return
  if (navigator.share) {
    navigator.share({
      title: poll.value.question,
      text: poll.value.description,
      url: window.location.href
    })
  } else {
    navigator.clipboard.writeText(window.location.href)
    alert(textConstants.linkCopied)
  }
}

// Initialize
onMounted(() => {
  loadPoll()
})

// Watch for route changes
watch(() => route.params.id, () => {
  if (route.params.id) {
    loadPoll()
  }
})
</script>

<template>
  <div class="poll-detail-page min-h-screen bg-gray-50">
    <!-- Loading State -->
    <div v-if="isLoading" class="flex items-center justify-center min-h-screen">
      <div class="text-center">
        <i class="fas fa-spinner fa-spin text-4xl text-teal-500 mb-4"></i>
        <p class="text-gray-600">{{ textConstants.loadingPoll }}</p>
      </div>
    </div>

    <!-- Content -->
    <template v-else-if="poll">
      <!-- Enhanced Header with Gradient -->
      <header class="poll-detail-header">
        <!-- Decorative Background -->
        <div class="header-decor">
          <div class="decor-orb decor-orb-1"></div>
          <div class="decor-orb decor-orb-2"></div>
          <div class="decor-pattern"></div>
        </div>

        <div class="header-content">
          <!-- Navigation Row -->
          <div class="header-nav">
            <button
              @click="goBack"
              class="back-btn"
            >
              <i class="fas fa-arrow-left"></i>
              <span>{{ textConstants.back }}</span>
            </button>
            <div class="breadcrumb">
              <router-link to="/polls" class="breadcrumb-link">
                <i class="fas fa-poll"></i>
                {{ textConstants.polls }}
              </router-link>
              <i class="fas fa-chevron-right breadcrumb-sep"></i>
              <span class="breadcrumb-category">
                <i :class="poll.categoryIcon"></i>
                {{ poll.category }}
              </span>
            </div>
          </div>

          <!-- Main Header Content -->
          <div class="header-main">
            <div class="header-left">
              <!-- Status & Time Badges -->
              <div class="header-badges">
                <span :class="['status-badge', poll.status]">
                  <span class="status-dot"></span>
                  <i :class="statusConfig.icon"></i>
                  {{ statusConfig.label }}
                </span>
                <span v-if="poll.isAnonymous" class="anon-badge">
                  <i class="fas fa-user-secret"></i>
                  {{ textConstants.anonymous }}
                </span>
                <span v-if="timeRemaining" class="time-badge">
                  <i class="fas fa-hourglass-half hourglass-icon"></i>
                  {{ timeRemaining }}
                </span>
              </div>

              <!-- Poll Title -->
              <h1 class="poll-title">{{ poll.question }}</h1>
              <p class="poll-desc">{{ poll.description }}</p>

              <!-- Quick Stats -->
              <div class="header-stats">
                <div class="stat-item">
                  <i class="fas fa-users"></i>
                  <span class="stat-value">{{ poll.totalVotes.toLocaleString() }}</span>
                  <span class="stat-label">{{ textConstants.votes }}</span>
                </div>
                <div class="stat-item">
                  <i class="fas fa-chart-pie"></i>
                  <span class="stat-value">{{ poll.participationRate }}%</span>
                  <span class="stat-label">{{ textConstants.participation }}</span>
                </div>
                <div class="stat-item">
                  <i class="fas fa-list-ul"></i>
                  <span class="stat-value">{{ poll.options.length }}</span>
                  <span class="stat-label">{{ textConstants.options }}</span>
                </div>
              </div>
            </div>

            <!-- Header Actions -->
            <div class="header-actions">
              <button
                @click="exportPoll"
                class="px-4 py-2 bg-transparent text-white border border-white/30 rounded-xl font-medium hover:bg-white/10 transition-all flex items-center gap-2"
                title="Export Poll Results"
              >
                <i class="fas fa-download"></i>
                <span class="hidden sm:inline">{{ textConstants.download }}</span>
              </button>
              <button
                @click="showAddToCollection = true"
                class="px-4 py-2 bg-transparent text-white border border-white/30 rounded-xl font-medium hover:bg-white/10 transition-all flex items-center gap-2"
                title="Add to Collection"
              >
                <i class="fas fa-folder-plus"></i>
                <span class="hidden sm:inline">{{ textConstants.collection }}</span>
              </button>
              <button
                @click="sharePoll"
                class="px-4 py-2 bg-transparent text-white border border-white/30 rounded-xl font-medium hover:bg-white/10 transition-all flex items-center gap-2"
                title="Share Poll"
              >
                <i class="fas fa-share-alt"></i>
                <span class="hidden sm:inline">{{ textConstants.share }}</span>
              </button>
              <BookmarkButton
                :content-id="poll.id"
                content-type="poll"
                size="md"
                variant="icon"
                class="text-white"
              />
            </div>
          </div>
        </div>
      </header>

      <!-- Main Content -->
      <main class="px-6 py-8">
        <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
          <!-- Left Column - Poll Content -->
          <div class="lg:col-span-2 space-y-6">
            <!-- Voting Options Card -->
            <div class="bg-white rounded-2xl shadow-sm border border-gray-200 overflow-hidden">
              <div class="p-6">
                <h3 class="text-lg font-semibold text-gray-900 mb-4 flex items-center gap-2">
                  <i class="fas fa-vote-yea text-teal-500"></i>
                  {{ canVote ? textConstants.castYourVote : (poll.hasVoted ? textConstants.results : textConstants.pollOptions) }}
                </h3>

                <!-- Options -->
                <div class="space-y-3 mt-6">
                  <div
                    v-for="(option, index) in poll.options"
                    :key="option.id"
                    @click="selectOption(option.id)"
                    :class="[
                      'relative rounded-xl border-2 transition-all overflow-hidden',
                      canVote ? 'cursor-pointer hover:border-teal-400' : 'cursor-default',
                      selectedOption === option.id ? 'border-teal-500 bg-teal-50' : 'border-gray-200 bg-white',
                      poll.userVote === option.id ? 'border-teal-500 bg-teal-50' : ''
                    ]"
                  >
                    <!-- Progress Bar Background -->
                    <div
                      v-if="canSeeResults"
                      :class="[
                        'absolute inset-0 transition-all duration-700',
                        getBarColor(index)
                      ]"
                      :style="{
                        width: `${option.percentage}%`,
                        opacity: 0.15
                      }"
                    ></div>

                    <div class="relative p-4 flex items-center justify-between">
                      <div class="flex items-center gap-3">
                        <!-- Radio/Check -->
                        <div :class="[
                          'w-6 h-6 rounded-full border-2 flex items-center justify-center transition-colors',
                          selectedOption === option.id || poll.userVote === option.id
                            ? 'border-teal-500 bg-teal-500'
                            : 'border-gray-300'
                        ]">
                          <i
                            v-if="selectedOption === option.id || poll.userVote === option.id"
                            class="fas fa-check text-white text-xs"
                          ></i>
                        </div>

                        <span class="font-medium text-gray-900">{{ option.text }}</span>

                        <!-- Winner Badge -->
                        <span
                          v-if="poll.status === 'completed' && winningOption?.id === option.id"
                          class="px-2 py-0.5 bg-yellow-100 text-yellow-700 text-xs font-medium rounded-full"
                        >
                          <i class="fas fa-trophy mr-1"></i>
                          Winner
                        </span>

                        <!-- User's Vote -->
                        <span
                          v-if="poll.userVote === option.id"
                          class="px-2 py-0.5 bg-teal-100 text-teal-700 text-xs font-medium rounded-full"
                        >
                          Your Vote
                        </span>
                      </div>

                      <!-- Results -->
                      <div v-if="canSeeResults" class="flex items-center gap-3">
                        <span class="text-sm text-gray-500">{{ option.votes }} votes</span>
                        <span :class="[
                          'text-lg font-bold min-w-[3rem] text-right',
                          winningOption?.id === option.id ? 'text-teal-600' : 'text-gray-700'
                        ]">
                          {{ option.percentage }}%
                        </span>
                      </div>
                    </div>
                  </div>
                </div>

                <!-- Vote Button -->
                <div v-if="canVote" class="mt-6">
                  <button
                    @click="submitVote"
                    :disabled="!selectedOption || isVoting"
                    :class="[
                      'w-full py-3 rounded-xl font-semibold text-white transition-all',
                      selectedOption
                        ? 'bg-teal-500 hover:bg-teal-600'
                        : 'bg-gray-300 cursor-not-allowed'
                    ]"
                  >
                    <i v-if="isVoting" class="fas fa-spinner fa-spin mr-2"></i>
                    {{ isVoting ? textConstants.submitting : textConstants.submitVote }}
                  </button>
                </div>

                <!-- Already Voted Message -->
                <div v-else-if="poll.hasVoted" class="mt-6 p-4 bg-teal-50 rounded-xl text-center">
                  <i class="fas fa-check-circle text-teal-500 text-xl mb-2"></i>
                  <p class="text-teal-700 font-medium">Thanks for voting!</p>
                  <p class="text-sm text-teal-600">Your response has been recorded.</p>
                </div>

                <!-- Stats Summary -->
                <div class="mt-6 pt-6 border-t border-gray-200 flex items-center justify-between text-sm">
                  <div class="flex items-center gap-4">
                    <span class="text-gray-500">
                      <i class="fas fa-users mr-1"></i>
                      {{ poll.totalVotes.toLocaleString() }} {{ textConstants.votes }}
                    </span>
                    <span class="text-gray-500">
                      <i class="fas fa-chart-pie mr-1"></i>
                      {{ poll.participationRate }}% {{ textConstants.participation }}
                    </span>
                  </div>
                  <span class="text-gray-500">
                    <i :class="[poll.categoryIcon, 'mr-1']"></i>
                    {{ poll.category }}
                  </span>
                </div>
              </div>
            </div>

            <!-- AI Sentiment Analysis -->
            <div v-if="canSeeResults" class="bg-white rounded-2xl shadow-sm border border-gray-200 p-6">
              <div class="flex items-center gap-3 mb-4">
                <div class="w-10 h-10 bg-purple-100 rounded-lg flex items-center justify-center">
                  <i class="fas fa-brain text-purple-600"></i>
                </div>
                <div>
                  <h3 class="font-semibold text-gray-900">{{ textConstants.aiSentimentAnalysis }}</h3>
                  <p class="text-sm text-gray-500">{{ textConstants.sentimentInsights }}</p>
                </div>
              </div>

              <div class="space-y-3">
                <div
                  v-for="option in poll.options"
                  :key="option.id"
                  class="flex items-center justify-between p-3 bg-gray-50 rounded-lg"
                >
                  <span class="text-gray-700">{{ option.text }}</span>
                  <div class="flex items-center gap-2">
                    <span :class="['text-sm font-medium', option.sentiment?.color]">
                      {{ option.sentiment?.label }}
                    </span>
                    <div class="w-16 h-2 bg-gray-200 rounded-full overflow-hidden">
                      <div
                        class="h-full bg-gradient-to-r from-teal-400 to-teal-600 rounded-full transition-all duration-500"
                        :style="{ width: `${(option.sentiment?.score || 0) * 100}%` }"
                      ></div>
                    </div>
                  </div>
                </div>
              </div>

              <!-- AI Summary -->
              <div class="mt-4 p-4 bg-purple-50 rounded-xl">
                <p class="text-purple-800 text-sm">
                  <i class="fas fa-lightbulb mr-2"></i>
                  <strong>AI Insight:</strong> The majority sentiment leans towards growth-oriented options.
                  "Product Innovation" shows the strongest positive sentiment, indicating team alignment with
                  forward-thinking strategies for Q1 2025.
                </p>
              </div>
            </div>

            <!-- Vote Distribution Over Time -->
            <div v-if="canSeeResults" class="bg-white rounded-2xl shadow-sm border border-gray-200 p-6">
              <div class="flex items-center justify-between mb-4">
                <div class="flex items-center gap-3">
                  <div class="w-10 h-10 bg-blue-100 rounded-lg flex items-center justify-center">
                    <i class="fas fa-chart-line text-blue-600"></i>
                  </div>
                  <div>
                    <h3 class="font-semibold text-gray-900">Vote Activity</h3>
                    <p class="text-sm text-gray-500">Responses over the past week</p>
                  </div>
                </div>
                <button
                  @click="showAnalytics = !showAnalytics"
                  class="text-sm text-teal-600 hover:text-teal-700"
                >
                  {{ showAnalytics ? 'Hide' : 'Show' }} Details
                </button>
              </div>

              <!-- Simple Bar Chart -->
              <div class="flex items-end gap-2 h-32">
                <div
                  v-for="(day, index) in voteDistribution"
                  :key="index"
                  class="flex-1 flex flex-col items-center gap-1"
                >
                  <div
                    class="w-full bg-teal-500 rounded-t-lg transition-all duration-500 hover:bg-teal-600"
                    :style="{ height: `${(day.votes / maxVotes) * 100}%` }"
                    :title="`${day.votes} votes`"
                  ></div>
                  <span class="text-xs text-gray-500">{{ day.date }}</span>
                </div>
              </div>

              <!-- Expanded Analytics -->
              <div v-if="showAnalytics" class="mt-4 pt-4 border-t border-gray-200 grid grid-cols-3 gap-4">
                <div class="text-center">
                  <p class="text-2xl font-bold text-gray-900">{{ poll.totalVotes }}</p>
                  <p class="text-sm text-gray-500">Total Votes</p>
                </div>
                <div class="text-center">
                  <p class="text-2xl font-bold text-gray-900">{{ poll.participationRate }}%</p>
                  <p class="text-sm text-gray-500">Participation</p>
                </div>
                <div class="text-center">
                  <p class="text-2xl font-bold text-gray-900">{{ poll.options.length }}</p>
                  <p class="text-sm text-gray-500">Options</p>
                </div>
              </div>
            </div>

            <!-- Comments Section -->
            <div class="bg-white rounded-2xl shadow-sm border border-gray-200 p-6">
              <CommentsSection
                content-type="poll"
                :content-id="poll.id"
                :comments="comments"
                :is-loading="commentsLoading"
                @add-comment="addComment"
              />
            </div>

            <!-- Related Polls -->
            <div class="bg-white rounded-2xl shadow-sm border border-gray-200 p-6">
              <RelatedContentCarousel
                content-type="poll"
                :content-id="poll.id"
                :title="textConstants.relatedPolls"
                :limit="4"
              />
            </div>
          </div>

          <!-- Right Column - Sidebar -->
          <div class="space-y-6">
            <!-- Author Card -->
            <div class="bg-white rounded-2xl shadow-sm border border-gray-200 p-6">
              <h3 class="font-semibold text-gray-900 mb-4">{{ textConstants.aboutAuthor }}</h3>
              <AuthorCard :author="poll.author" variant="compact" />
            </div>

            <!-- Poll Details -->
            <div class="bg-white rounded-2xl shadow-sm border border-gray-200 p-6">
              <h3 class="font-semibold text-gray-900 mb-4">{{ textConstants.pollDetails }}</h3>
              <div class="space-y-4">
                <div class="flex items-center justify-between">
                  <span class="text-gray-500">
                    <i class="fas fa-calendar-plus w-5 mr-2"></i>
                    Created
                  </span>
                  <span class="text-gray-900 text-sm">{{ formatDate(poll.createdAt) }}</span>
                </div>
                <div class="flex items-center justify-between">
                  <span class="text-gray-500">
                    <i class="fas fa-calendar-check w-5 mr-2"></i>
                    Ends
                  </span>
                  <span class="text-gray-900 text-sm">{{ formatDate(poll.endDate) }}</span>
                </div>
                <div class="flex items-center justify-between">
                  <span class="text-gray-500">
                    <i class="fas fa-users w-5 mr-2"></i>
                    Target
                  </span>
                  <span class="text-gray-900 text-sm">{{ poll.targetAudience }}</span>
                </div>
                <div class="flex items-center justify-between">
                  <span class="text-gray-500">
                    <i class="fas fa-eye w-5 mr-2"></i>
                    Results
                  </span>
                  <span class="text-gray-900 text-sm capitalize">{{ poll.showResults.replace('_', ' ') }}</span>
                </div>
                <div class="flex items-center justify-between">
                  <span class="text-gray-500">
                    <i class="fas fa-check-double w-5 mr-2"></i>
                    Multiple
                  </span>
                  <span class="text-gray-900 text-sm">{{ poll.allowMultiple ? 'Yes' : 'No' }}</span>
                </div>
              </div>
            </div>

            <!-- Tags -->
            <div class="bg-white rounded-2xl shadow-sm border border-gray-200 p-6">
              <h3 class="font-semibold text-gray-900 mb-4">{{ textConstants.tags }}</h3>
              <div class="flex flex-wrap gap-2">
                <span
                  v-for="tag in poll.tags"
                  :key="tag"
                  class="px-3 py-1 bg-gray-100 text-gray-700 rounded-full text-sm hover:bg-gray-200 cursor-pointer transition-colors"
                >
                  {{ tag }}
                </span>
              </div>
            </div>

            <!-- Share Section -->
            <div class="bg-white rounded-2xl shadow-sm border border-gray-200 p-6">
              <h3 class="font-semibold text-gray-900 mb-4">{{ textConstants.sharePoll }}</h3>
              <SocialShareButtons
                :title="poll.question"
                :description="poll.description"
                layout="horizontal"
                size="md"
              />
            </div>

            <!-- Quick Actions -->
            <div class="bg-gradient-to-br from-teal-500 to-teal-600 rounded-2xl p-6 text-white">
              <h3 class="font-semibold mb-2">{{ textConstants.createPollCTA }}</h3>
              <p class="text-teal-100 text-sm mb-4">
                {{ textConstants.createPollDesc }}
              </p>
              <router-link
                to="/polls/new"
                class="inline-flex items-center gap-2 px-4 py-2 bg-white text-teal-600 rounded-lg font-medium hover:bg-teal-50 transition-colors"
              >
                <i class="fas fa-plus"></i>
                {{ textConstants.createNewPoll }}
              </router-link>
            </div>
          </div>
        </div>
      </main>
    </template>

    <!-- Share Modal -->
    <Teleport to="body">
      <div
        v-if="showShareModal && poll"
        class="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4"
        @click.self="showShareModal = false"
      >
        <div class="bg-white rounded-2xl max-w-md w-full p-6">
          <div class="flex items-center justify-between mb-4">
            <h3 class="text-lg font-semibold text-gray-900">Share Poll</h3>
            <button @click="showShareModal = false" class="p-2 hover:bg-gray-100 rounded-lg">
              <i class="fas fa-times text-gray-500"></i>
            </button>
          </div>

          <p class="text-gray-600 mb-4">{{ poll.question }}</p>

          <SocialShareButtons
            :title="poll.question"
            :description="poll.description"
            layout="vertical"
            size="lg"
            :show-labels="true"
          />
        </div>
      </div>
    </Teleport>

    <!-- Add to Collection Modal -->
    <AddToCollectionModal
      v-if="poll"
      :show="showAddToCollection"
      :content-id="poll.id"
      content-type="poll"
      :content-title="poll.question"
      @close="showAddToCollection = false"
    />
  </div>
</template>

<style scoped>
.poll-detail-page {
  animation: fadeIn 0.3s ease;
}

@keyframes fadeIn {
  from { opacity: 0; }
  to { opacity: 1; }
}

/* Enhanced Header Styles */
.poll-detail-header {
  background: linear-gradient(135deg, #0f766e 0%, #0d9488 50%, #14b8a6 100%);
  position: relative;
  overflow: hidden;
}

/* Decorative Elements */
.header-decor {
  position: absolute;
  inset: 0;
  pointer-events: none;
  overflow: hidden;
}

.decor-orb {
  position: absolute;
  border-radius: 50%;
  filter: blur(60px);
  opacity: 0.3;
}

.decor-orb-1 {
  width: 300px;
  height: 300px;
  background: linear-gradient(135deg, #5eead4, #99f6e4);
  top: -100px;
  right: -50px;
  animation: orb-float 8s ease-in-out infinite;
}

.decor-orb-2 {
  width: 200px;
  height: 200px;
  background: linear-gradient(135deg, #2dd4bf, #5eead4);
  bottom: -80px;
  left: 10%;
  animation: orb-float 10s ease-in-out infinite reverse;
}

.decor-pattern {
  position: absolute;
  inset: 0;
  background-image: radial-gradient(rgba(255, 255, 255, 0.1) 1px, transparent 1px);
  background-size: 24px 24px;
  opacity: 0.5;
}

@keyframes orb-float {
  0%, 100% { transform: translate(0, 0); }
  50% { transform: translate(20px, -20px); }
}

/* Header Content */
.header-content {
  position: relative;
  z-index: 2;
  padding: 1.5rem 2rem 2rem;
}

/* Navigation Row */
.header-nav {
  display: flex;
  align-items: center;
  gap: 1rem;
  margin-bottom: 1.5rem;
}

.back-btn {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem 1rem;
  background: rgba(255, 255, 255, 0.15);
  border: 1px solid rgba(255, 255, 255, 0.2);
  border-radius: 0.75rem;
  color: white;
  font-size: 0.875rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s ease;
}

.back-btn:hover {
  background: rgba(255, 255, 255, 0.25);
  transform: translateX(-2px);
}

.breadcrumb {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.875rem;
}

.breadcrumb-link {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  color: rgba(255, 255, 255, 0.8);
  text-decoration: none;
  transition: color 0.2s ease;
}

.breadcrumb-link:hover {
  color: white;
}

.breadcrumb-sep {
  color: rgba(255, 255, 255, 0.4);
  font-size: 0.625rem;
}

.breadcrumb-category {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  color: white;
  font-weight: 500;
}

/* Main Header Content */
.header-main {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: 2rem;
}

.header-left {
  flex: 1;
  max-width: 700px;
}

/* Badges */
.header-badges {
  display: flex;
  flex-wrap: wrap;
  gap: 0.75rem;
  margin-bottom: 1rem;
}

.status-badge {
  display: inline-flex;
  align-items: center;
  gap: 0.375rem;
  padding: 0.375rem 0.875rem;
  background: rgba(255, 255, 255, 0.2);
  border-radius: 2rem;
  font-size: 0.8125rem;
  font-weight: 600;
  color: white;
  backdrop-filter: blur(8px);
}

.status-badge.active {
  background: rgba(34, 197, 94, 0.25);
  border: 1px solid rgba(34, 197, 94, 0.4);
}

.status-badge.completed {
  background: rgba(148, 163, 184, 0.25);
  border: 1px solid rgba(148, 163, 184, 0.4);
}

.status-badge.scheduled {
  background: rgba(59, 130, 246, 0.25);
  border: 1px solid rgba(59, 130, 246, 0.4);
}

.status-badge.draft {
  background: rgba(234, 179, 8, 0.25);
  border: 1px solid rgba(234, 179, 8, 0.4);
}

.status-dot {
  width: 6px;
  height: 6px;
  border-radius: 50%;
  background: currentColor;
  animation: pulse-dot 2s ease-in-out infinite;
}

.status-badge.active .status-dot {
  background: #22c55e;
}

@keyframes pulse-dot {
  0%, 100% { opacity: 1; transform: scale(1); }
  50% { opacity: 0.5; transform: scale(1.2); }
}

.anon-badge {
  display: inline-flex;
  align-items: center;
  gap: 0.375rem;
  padding: 0.375rem 0.875rem;
  background: rgba(139, 92, 246, 0.25);
  border: 1px solid rgba(139, 92, 246, 0.4);
  border-radius: 2rem;
  font-size: 0.8125rem;
  font-weight: 500;
  color: white;
}

.time-badge {
  display: inline-flex;
  align-items: center;
  gap: 0.375rem;
  padding: 0.375rem 0.875rem;
  background: rgba(255, 255, 255, 0.15);
  border: 1px solid rgba(255, 255, 255, 0.2);
  border-radius: 2rem;
  font-size: 0.8125rem;
  font-weight: 500;
  color: white;
}

.hourglass-icon {
  animation: hourglass-flip 3s ease-in-out infinite;
}

@keyframes hourglass-flip {
  0%, 45% { transform: rotate(0deg); }
  50%, 95% { transform: rotate(180deg); }
  100% { transform: rotate(360deg); }
}

/* Poll Title & Description */
.poll-title {
  font-size: 1.75rem;
  font-weight: 700;
  color: white;
  line-height: 1.3;
  margin-bottom: 0.5rem;
}

.poll-desc {
  font-size: 1rem;
  color: rgba(255, 255, 255, 0.85);
  line-height: 1.5;
  margin-bottom: 1.5rem;
}

/* Header Stats */
.header-stats {
  display: flex;
  gap: 2rem;
}

.stat-item {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  color: rgba(255, 255, 255, 0.9);
  font-size: 0.875rem;
}

.stat-item i {
  color: rgba(255, 255, 255, 0.7);
}

.stat-value {
  font-weight: 700;
  color: white;
}

.stat-label {
  color: rgba(255, 255, 255, 0.7);
}

/* Header Actions */
.header-actions {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  flex-shrink: 0;
}

/* Poll Options Animation */
.poll-detail-page :deep(.space-y-3 > div) {
  animation: slide-in 0.3s ease-out;
  animation-fill-mode: both;
}

.poll-detail-page :deep(.space-y-3 > div:nth-child(1)) { animation-delay: 0.05s; }
.poll-detail-page :deep(.space-y-3 > div:nth-child(2)) { animation-delay: 0.1s; }
.poll-detail-page :deep(.space-y-3 > div:nth-child(3)) { animation-delay: 0.15s; }
.poll-detail-page :deep(.space-y-3 > div:nth-child(4)) { animation-delay: 0.2s; }
.poll-detail-page :deep(.space-y-3 > div:nth-child(5)) { animation-delay: 0.25s; }

@keyframes slide-in {
  from {
    opacity: 0;
    transform: translateX(-20px);
  }
  to {
    opacity: 1;
    transform: translateX(0);
  }
}

/* Card Hover Effects */
.poll-detail-page :deep(.bg-white.rounded-2xl) {
  transition: box-shadow 0.3s ease, transform 0.3s ease;
}

.poll-detail-page :deep(.bg-white.rounded-2xl:hover) {
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.08);
}

/* Stats item hover */
.stat-item {
  transition: transform 0.2s ease;
}

.stat-item:hover {
  transform: scale(1.05);
}

/* Responsive */
@media (max-width: 768px) {
  .header-content {
    padding: 1rem 1.5rem 1.5rem;
  }

  .header-main {
    flex-direction: column;
    gap: 1rem;
  }

  .header-actions {
    flex-wrap: wrap;
    justify-content: flex-start;
    margin-top: 1rem;
  }

  .poll-title {
    font-size: 1.375rem;
  }

  .header-stats {
    flex-wrap: wrap;
    gap: 1rem;
  }
}
</style>
