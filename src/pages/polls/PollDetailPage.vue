<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useComments } from '@/composables/useComments'
import { useRelatedContent } from '@/composables/useRelatedContent'
import { useSharing } from '@/composables/useSharing'
import { CommentsSection, SocialShareButtons, RelatedContentCarousel, AuthorCard, BookmarkButton } from '@/components/common'
import type { Author } from '@/types/detail-pages'

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
        <p class="text-gray-600">Loading poll...</p>
      </div>
    </div>

    <!-- Content -->
    <template v-else-if="poll">
      <!-- Header -->
      <header class="bg-white border-b border-gray-200 sticky top-0 z-30">
        <div class="px-6 py-4">
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
                  <router-link to="/polls" class="hover:text-teal-600">Polls</router-link>
                  <i class="fas fa-chevron-right text-xs"></i>
                  <span>{{ poll.category }}</span>
                </div>
                <h1 class="text-xl font-semibold text-gray-900">Poll Details</h1>
              </div>
            </div>

            <div class="flex items-center gap-3">
              <BookmarkButton
                :content-id="poll.id"
                content-type="poll"
                size="sm"
              />
              <button
                @click="showShareModal = true"
                class="p-2 hover:bg-gray-100 rounded-lg transition-colors"
              >
                <i class="fas fa-share-alt text-gray-600"></i>
              </button>
              <button class="p-2 hover:bg-gray-100 rounded-lg transition-colors">
                <i class="fas fa-ellipsis-v text-gray-600"></i>
              </button>
            </div>
          </div>
        </div>
      </header>

      <!-- Main Content -->
      <main class="px-6 py-8">
        <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
          <!-- Left Column - Poll Content -->
          <div class="lg:col-span-2 space-y-6">
            <!-- Poll Question Card -->
            <div class="bg-white rounded-2xl shadow-sm border border-gray-200 overflow-hidden">
              <!-- Status Banner -->
              <div :class="[
                'px-6 py-3 flex items-center justify-between',
                poll.status === 'active' ? 'bg-gradient-to-r from-teal-500 to-teal-600' : 'bg-gray-100'
              ]">
                <div class="flex items-center gap-2">
                  <span :class="[
                    'px-3 py-1 rounded-full text-sm font-medium',
                    statusConfig.class
                  ]">
                    <i :class="[statusConfig.icon, 'mr-1']"></i>
                    {{ statusConfig.label }}
                  </span>
                  <span v-if="poll.isAnonymous" class="px-3 py-1 bg-white/20 rounded-full text-sm text-white">
                    <i class="fas fa-user-secret mr-1"></i>
                    Anonymous
                  </span>
                </div>
                <div v-if="timeRemaining" :class="[
                  'text-sm font-medium',
                  poll.status === 'active' ? 'text-white' : 'text-gray-600'
                ]">
                  <i class="fas fa-clock mr-1"></i>
                  {{ timeRemaining }}
                </div>
              </div>

              <!-- Question -->
              <div class="p-6">
                <div class="flex items-start gap-3 mb-4">
                  <div class="w-10 h-10 bg-teal-100 rounded-lg flex items-center justify-center">
                    <i class="fas fa-poll text-teal-600"></i>
                  </div>
                  <div class="flex-1">
                    <h2 class="text-2xl font-bold text-gray-900 mb-2">
                      {{ poll.question }}
                    </h2>
                    <p class="text-gray-600">{{ poll.description }}</p>
                  </div>
                </div>

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
                    {{ isVoting ? 'Submitting...' : 'Submit Vote' }}
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
                      {{ poll.totalVotes.toLocaleString() }} votes
                    </span>
                    <span class="text-gray-500">
                      <i class="fas fa-chart-pie mr-1"></i>
                      {{ poll.participationRate }}% participation
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
                  <h3 class="font-semibold text-gray-900">AI Sentiment Analysis</h3>
                  <p class="text-sm text-gray-500">Understanding the sentiment behind each option</p>
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
                title="Related Polls"
                :limit="4"
              />
            </div>
          </div>

          <!-- Right Column - Sidebar -->
          <div class="space-y-6">
            <!-- Author Card -->
            <div class="bg-white rounded-2xl shadow-sm border border-gray-200 p-6">
              <h3 class="font-semibold text-gray-900 mb-4">Created by</h3>
              <AuthorCard :author="poll.author" variant="compact" />
            </div>

            <!-- Poll Details -->
            <div class="bg-white rounded-2xl shadow-sm border border-gray-200 p-6">
              <h3 class="font-semibold text-gray-900 mb-4">Poll Details</h3>
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
              <h3 class="font-semibold text-gray-900 mb-4">Tags</h3>
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
              <h3 class="font-semibold text-gray-900 mb-4">Share this Poll</h3>
              <SocialShareButtons
                :title="poll.question"
                :description="poll.description"
                layout="horizontal"
                size="md"
              />
            </div>

            <!-- Quick Actions -->
            <div class="bg-gradient-to-br from-teal-500 to-teal-600 rounded-2xl p-6 text-white">
              <h3 class="font-semibold mb-2">Want to create your own poll?</h3>
              <p class="text-teal-100 text-sm mb-4">
                Gather feedback from your team with custom polls and surveys.
              </p>
              <router-link
                to="/polls/new"
                class="inline-flex items-center gap-2 px-4 py-2 bg-white text-teal-600 rounded-lg font-medium hover:bg-teal-50 transition-colors"
              >
                <i class="fas fa-plus"></i>
                Create Poll
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
</style>
