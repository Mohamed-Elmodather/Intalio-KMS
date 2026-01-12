<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'
import { useAIServicesStore } from '@/stores/aiServices'
import { AILoadingIndicator, AISuggestionChip, AIConfidenceBar } from '@/components/ai'

const router = useRouter()
const aiStore = useAIServicesStore()

// Loading state
const isLoading = ref(false)

// Modal state
const showEditProfile = ref(false)

// Activity filter
const activityFilter = ref('all')

// User data
const user = ref({
  name: 'Ahmed Imam',
  initials: 'AI',
  role: 'Product Director',
  department: 'Product Management',
  location: 'Dubai, UAE',
  joinDate: 'March 2022',
  email: 'ahmed.imam@intalio.com',
  phone: '+971 50 123 4567',
  bio: 'Experienced product leader with 10+ years in enterprise software. Passionate about building user-centric products that drive business value. Leading the Intalio Knowledge Hub initiative to transform how organizations manage and share knowledge.',
  skills: ['Product Strategy', 'UX Design', 'Agile/Scrum', 'Data Analytics', 'Team Leadership', 'Enterprise Software', 'AI/ML Integration']
})

// Stats
const stats = ref({
  articles: 24,
  comments: 156,
  documents: 38,
  courses: 12
})

// Followers/Following
const followersCount = ref(234)
const followingCount = ref(156)
const mutualConnections = ref(45)
const followers = ref([
  { id: 1, name: 'Sarah Chen', initials: 'SC', role: 'Engineering Lead', isFollowing: true },
  { id: 2, name: 'Mike Johnson', initials: 'MJ', role: 'Senior Designer', isFollowing: true },
  { id: 3, name: 'Emily Davis', initials: 'ED', role: 'Product Manager', isFollowing: false },
  { id: 4, name: 'Alex Thompson', initials: 'AT', role: 'Developer', isFollowing: true },
  { id: 5, name: 'Lisa Wang', initials: 'LW', role: 'UX Designer', isFollowing: false }
])

// Content Authored
const authoredContent = ref({
  articles: [
    { id: 1, title: 'Complete Employee Onboarding Guide 2024', views: 1250, likes: 89, date: '2 days ago' },
    { id: 2, title: 'Building Effective Remote Teams', views: 980, likes: 67, date: '1 week ago' },
    { id: 3, title: 'Product Strategy Best Practices', views: 2100, likes: 156, date: '2 weeks ago' }
  ],
  documents: [
    { id: 1, title: 'Q4 Product Roadmap', downloads: 234, date: '3 days ago' },
    { id: 2, title: 'Team Performance Review Template', downloads: 189, date: '1 week ago' }
  ],
  polls: [
    { id: 1, title: 'Team Meeting Time Preferences', votes: 45, status: 'completed' },
    { id: 2, title: 'Office Return Policy Survey', votes: 78, status: 'active' }
  ]
})

// Active content tab
const activeContentTab = ref<'articles' | 'documents' | 'polls'>('articles')

// Edit form
const editForm = ref({
  firstName: 'Ahmed',
  lastName: 'Imam',
  title: 'Product Director',
  bio: user.value.bio,
  phone: '+971 50 123 4567',
  location: 'Dubai, UAE'
})

// Contribution data for chart
const contributionData = ref([30, 45, 60, 75, 50, 85, 90, 70, 55, 80, 95, 60])

// Activities
const activities = ref([
  {
    type: 'Article',
    typeBadge: 'badge-blue',
    icon: 'fas fa-file-alt',
    iconBg: 'bg-blue-500',
    iconColor: 'text-white',
    title: 'Published "Complete Employee Onboarding Guide 2024"',
    description: '',
    time: '2 hours ago'
  },
  {
    type: 'Comment',
    typeBadge: 'badge-purple',
    icon: 'fas fa-comment',
    iconBg: 'bg-purple-500',
    iconColor: 'text-white',
    title: 'Commented on "Remote Work Best Practices"',
    description: 'Great insights on async communication!',
    time: '5 hours ago'
  },
  {
    type: 'Course',
    typeBadge: 'badge-green',
    icon: 'fas fa-graduation-cap',
    iconBg: 'bg-green-500',
    iconColor: 'text-white',
    title: 'Completed "Advanced Product Management"',
    description: '',
    time: 'Yesterday'
  },
  {
    type: 'Document',
    typeBadge: 'badge-orange',
    icon: 'fas fa-file-upload',
    iconBg: 'bg-orange-500',
    iconColor: 'text-white',
    title: 'Uploaded "Q4 Product Roadmap"',
    description: '',
    time: '2 days ago'
  }
])

// Badges
const badges = ref([
  { id: 1, name: 'Top Author', icon: 'fas fa-pen-fancy', bg: 'bg-blue-100', color: 'text-blue-600', description: 'Published 20+ articles' },
  { id: 2, name: 'Mentor', icon: 'fas fa-hands-helping', bg: 'bg-purple-100', color: 'text-purple-600', description: 'Helped 50+ colleagues' },
  { id: 3, name: 'Fast Learner', icon: 'fas fa-rocket', bg: 'bg-green-100', color: 'text-green-600', description: 'Completed 10 courses' },
  { id: 4, name: 'Innovator', icon: 'fas fa-lightbulb', bg: 'bg-yellow-100', color: 'text-yellow-600', description: 'Submitted 5 ideas' },
  { id: 5, name: 'Team Player', icon: 'fas fa-users', bg: 'bg-pink-100', color: 'text-pink-600', description: 'Active collaborator' },
  { id: 6, name: 'Explorer', icon: 'fas fa-compass', bg: 'bg-teal-100', color: 'text-teal-600', description: 'Used all features' }
])

const totalBadges = ref(15)

// Learning Progress
const learningProgress = ref([
  { id: 1, name: 'Leadership Essentials', progress: 85 },
  { id: 2, name: 'Data-Driven Decisions', progress: 60 },
  { id: 3, name: 'AI Fundamentals', progress: 40 }
])

// Certificates
const certificates = ref([
  { id: 1, name: 'Product Management Professional', date: 'Dec 2024' },
  { id: 2, name: 'Agile Scrum Master', date: 'Nov 2024' },
  { id: 3, name: 'UX Design Foundations', date: 'Oct 2024' }
])

// Team Members
const teamMembers = ref([
  { id: 1, name: 'Sarah Chen', initials: 'SC', color: '#8B5CF6', role: 'Engineering Lead', online: true },
  { id: 2, name: 'Mike Johnson', initials: 'MJ', color: '#3B82F6', role: 'Senior Designer', online: true },
  { id: 3, name: 'Emily Davis', initials: 'ED', color: '#10B981', role: 'Product Manager', online: false },
  { id: 4, name: 'Alex Thompson', initials: 'AT', color: '#F59E0B', role: 'Developer', online: true }
])

// Methods
const goToSettings = () => {
  router.push('/settings')
}

const goToLearning = () => {
  router.push('/learning')
}

const saveProfile = () => {
  showEditProfile.value = false
  // In a real app, this would save to the backend
  alert('Profile updated successfully!')
}

// ==================== AI FEATURES ====================

// AI State
const showAIProfileModal = ref(false)
const showAISkillsModal = ref(false)
const showAIInsightsModal = ref(false)
const isGeneratingProfile = ref(false)
const isAnalyzingSkills = ref(false)
const isLoadingInsights = ref(false)

// AI Interfaces
interface AIProfileSummary {
  professional: string
  strengths: string[]
  expertise: string[]
  impactStatement: string
  confidence: number
}

interface AISkillAnalysis {
  detectedSkills: Array<{
    name: string
    level: 'beginner' | 'intermediate' | 'advanced' | 'expert'
    confidence: number
    source: string
  }>
  suggestedSkills: Array<{
    name: string
    reason: string
    relevance: number
  }>
  skillGaps: Array<{
    skill: string
    importance: string
    recommendation: string
  }>
}

interface AIProfileInsight {
  id: string
  type: 'strength' | 'growth' | 'recommendation' | 'achievement'
  title: string
  description: string
  metric?: string
  actionLabel?: string
}

// AI Generated Data
const aiProfileSummary = ref<AIProfileSummary | null>(null)
const aiSkillAnalysis = ref<AISkillAnalysis | null>(null)
const aiProfileInsights = ref<AIProfileInsight[]>([])

// Mock AI Profile Summary
const mockProfileSummary: AIProfileSummary = {
  professional: 'Visionary Product Director with 10+ years of experience driving enterprise software innovation. Known for transforming complex business requirements into user-centric solutions that deliver measurable ROI. Currently spearheading the Intalio Knowledge Hub initiative, demonstrating exceptional leadership in digital transformation.',
  strengths: [
    'Strategic product vision and roadmap development',
    'Cross-functional team leadership and mentorship',
    'Data-driven decision making with strong analytical skills',
    'Stakeholder communication and executive presentations'
  ],
  expertise: [
    'Enterprise SaaS product management',
    'Agile transformation and process optimization',
    'AI/ML integration in business applications',
    'User experience design and research'
  ],
  impactStatement: 'Consistently delivered products that increased user engagement by 40% and reduced operational costs by 25% across multiple organizations.',
  confidence: 0.92
}

// Mock AI Skill Analysis
const mockSkillAnalysis: AISkillAnalysis = {
  detectedSkills: [
    { name: 'Product Strategy', level: 'expert', confidence: 0.95, source: 'Articles published, course completions' },
    { name: 'Team Leadership', level: 'expert', confidence: 0.92, source: 'Team collaborations, mentoring activities' },
    { name: 'UX Design', level: 'advanced', confidence: 0.88, source: 'Articles, course: UX Design Foundations' },
    { name: 'Data Analytics', level: 'advanced', confidence: 0.85, source: 'Course: Data-Driven Decisions (60% complete)' },
    { name: 'Agile/Scrum', level: 'expert', confidence: 0.94, source: 'Certificate: Agile Scrum Master' },
    { name: 'AI/ML Integration', level: 'intermediate', confidence: 0.72, source: 'Course: AI Fundamentals (40% complete)' }
  ],
  suggestedSkills: [
    { name: 'Cloud Architecture', reason: 'Highly relevant for enterprise product leaders in 2024', relevance: 0.85 },
    { name: 'Product-Led Growth', reason: 'Emerging methodology aligned with your expertise', relevance: 0.82 },
    { name: 'OKR Management', reason: 'Complements your strategic planning skills', relevance: 0.78 }
  ],
  skillGaps: [
    { skill: 'Technical Architecture', importance: 'High for product directors', recommendation: 'Consider "System Design for PMs" course' },
    { skill: 'Financial Modeling', importance: 'Essential for product business cases', recommendation: 'Take "Product Economics 101" course' }
  ]
}

// Mock AI Profile Insights
const mockProfileInsights: AIProfileInsight[] = [
  {
    id: '1',
    type: 'achievement',
    title: 'Top 10% Contributor',
    description: 'Your article publication rate puts you in the top 10% of all knowledge contributors this quarter.',
    metric: '24 articles published'
  },
  {
    id: '2',
    type: 'strength',
    title: 'Exceptional Engagement',
    description: 'Your content receives 3x more comments than average, indicating high-value contributions.',
    metric: '156 total comments received'
  },
  {
    id: '3',
    type: 'growth',
    title: 'Learning Momentum',
    description: 'You\'ve completed 4 courses this quarter. At this pace, you\'ll earn 3 more certificates by year end.',
    metric: '12 courses completed'
  },
  {
    id: '4',
    type: 'recommendation',
    title: 'Skill Development Opportunity',
    description: 'Based on your interests, "Advanced AI for Product Managers" would complement your current learning path.',
    actionLabel: 'View Course'
  },
  {
    id: '5',
    type: 'strength',
    title: 'Knowledge Leader',
    description: 'Your documents have been accessed 500+ times, making you a key knowledge resource in your department.',
    metric: '38 documents shared'
  }
]

// AI Functions
async function generateAIProfileSummary() {
  isGeneratingProfile.value = true
  showAIProfileModal.value = true

  try {
    await new Promise(resolve => setTimeout(resolve, 1500))
    aiProfileSummary.value = mockProfileSummary
  } catch (error) {
    console.error('Failed to generate AI profile summary:', error)
  } finally {
    isGeneratingProfile.value = false
  }
}

async function analyzeSkills() {
  isAnalyzingSkills.value = true
  showAISkillsModal.value = true

  try {
    await new Promise(resolve => setTimeout(resolve, 1800))
    aiSkillAnalysis.value = mockSkillAnalysis
  } catch (error) {
    console.error('Failed to analyze skills:', error)
  } finally {
    isAnalyzingSkills.value = false
  }
}

async function loadAIInsights() {
  isLoadingInsights.value = true
  showAIInsightsModal.value = true

  try {
    await new Promise(resolve => setTimeout(resolve, 1200))
    aiProfileInsights.value = mockProfileInsights
  } catch (error) {
    console.error('Failed to load AI insights:', error)
  } finally {
    isLoadingInsights.value = false
  }
}

function applyAISummary() {
  if (aiProfileSummary.value) {
    user.value.bio = aiProfileSummary.value.professional
    showAIProfileModal.value = false
    alert('AI-generated summary applied to your profile!')
  }
}

function addSuggestedSkill(skill: { name: string }) {
  if (!user.value.skills.includes(skill.name)) {
    user.value.skills.push(skill.name)
    alert(`"${skill.name}" added to your skills!`)
  }
}

function getSkillLevelColor(level: string): string {
  const colors: Record<string, string> = {
    'beginner': 'bg-gray-100 text-gray-700',
    'intermediate': 'bg-blue-100 text-blue-700',
    'advanced': 'bg-teal-100 text-teal-700',
    'expert': 'bg-purple-100 text-purple-700'
  }
  return colors[level] || 'bg-gray-100 text-gray-700'
}

function getInsightIcon(type: string): string {
  const icons: Record<string, string> = {
    'strength': 'fas fa-star',
    'growth': 'fas fa-chart-line',
    'recommendation': 'fas fa-lightbulb',
    'achievement': 'fas fa-trophy'
  }
  return icons[type] || 'fas fa-info-circle'
}

function getInsightColor(type: string): string {
  const colors: Record<string, string> = {
    'strength': 'text-yellow-600 bg-yellow-100',
    'growth': 'text-green-600 bg-green-100',
    'recommendation': 'text-blue-600 bg-blue-100',
    'achievement': 'text-purple-600 bg-purple-100'
  }
  return colors[type] || 'text-gray-600 bg-gray-100'
}
</script>

<template>
  <div>
    <!-- Loading State -->
    <div v-if="isLoading" class="flex items-center justify-center min-h-[60vh]">
      <LoadingSpinner size="lg" text="Loading profile..." />
    </div>

    <template v-else>
      <!-- Breadcrumb -->
      <nav class="flex items-center gap-2 text-sm text-gray-500 mb-6">
        <router-link to="/" class="hover:text-primary-600 transition-colors">
          <i class="fas fa-home"></i>
        </router-link>
        <i class="fas fa-chevron-right text-xs"></i>
        <span class="text-gray-900 font-medium">Profile</span>
      </nav>

      <!-- Profile Header -->
      <div class="card-animated fade-in-up rounded-2xl overflow-hidden mb-10" style="animation-delay: 0.1s">
        <!-- Cover Photo -->
        <div class="h-48 bg-gradient-to-r from-teal-500 via-teal-600 to-teal-700 relative">
          <button class="btn-vibrant ripple absolute bottom-4 right-4 px-4 py-2 bg-white/20 backdrop-blur-sm rounded-lg text-white text-sm hover:bg-white/30 transition-colors">
            <i class="fas fa-camera mr-2 icon-soft"></i>Change Cover
          </button>
        </div>

        <!-- Profile Info -->
        <div class="px-6 pb-6">
          <div class="flex flex-col md:flex-row md:items-end gap-4 -mt-16 relative z-10">
            <div class="relative">
              <div class="w-32 h-32 rounded-2xl bg-gradient-to-br from-teal-400 to-teal-600 flex items-center justify-center text-white text-4xl font-bold border-4 border-white shadow-xl">
                {{ user.initials }}
              </div>
              <button class="btn-vibrant ripple absolute -bottom-1 -right-1 w-10 h-10 rounded-full bg-teal-500 text-white flex items-center justify-center shadow-lg hover:bg-teal-600 transition-colors">
                <i class="fas fa-camera icon-vibrant"></i>
              </button>
              <span class="absolute top-2 right-2 w-4 h-4 bg-green-500 rounded-full border-2 border-white"></span>
            </div>

            <div class="flex-1 md:pb-2">
              <h1 class="text-2xl font-bold text-teal-900">{{ user.name }}</h1>
              <p class="text-teal-600">{{ user.role }}</p>
              <div class="flex flex-wrap items-center gap-4 mt-2 text-sm text-teal-500">
                <span><i class="fas fa-building mr-1 icon-soft"></i>{{ user.department }}</span>
                <span><i class="fas fa-map-marker-alt mr-1 icon-soft"></i>{{ user.location }}</span>
                <span><i class="fas fa-calendar mr-1 icon-soft"></i>Joined {{ user.joinDate }}</span>
              </div>
            </div>

            <div class="flex gap-3">
              <!-- AI Insights Button -->
              <button @click="loadAIInsights"
                      class="px-4 py-2 bg-gradient-to-r from-teal-500 to-emerald-500 text-white rounded-xl font-medium text-sm flex items-center gap-2 hover:from-teal-600 hover:to-emerald-600 transition-all shadow-lg">
                <i class="fas fa-wand-magic-sparkles"></i>
                AI Insights
              </button>
              <button @click="goToSettings" class="btn btn-secondary btn-vibrant ripple">
                <i class="fas fa-cog icon-soft"></i> Settings
              </button>
              <button @click="showEditProfile = true" class="btn btn-primary btn-vibrant ripple">
                <i class="fas fa-edit icon-vibrant"></i> Edit Profile
              </button>
            </div>
          </div>
        </div>
      </div>

      <div class="grid grid-cols-1 xl:grid-cols-3 gap-6">
        <!-- Left Column -->
        <div class="xl:col-span-2 space-y-6">
          <!-- About -->
          <div class="card-animated fade-in-up rounded-2xl p-6" style="animation-delay: 0.2s">
            <div class="flex items-center justify-between mb-4">
              <h2 class="text-lg font-semibold text-teal-900">About</h2>
              <button @click="generateAIProfileSummary"
                      class="px-3 py-1.5 text-xs font-medium text-teal-600 bg-teal-50 rounded-lg hover:bg-teal-100 transition-colors flex items-center gap-1.5">
                <i class="fas fa-wand-magic-sparkles"></i>
                AI Generate Bio
              </button>
            </div>
            <p class="text-teal-700 leading-relaxed">{{ user.bio }}</p>

            <!-- Skills -->
            <div class="mt-6">
              <div class="flex items-center justify-between mb-3">
                <h3 class="text-sm font-semibold text-teal-500 uppercase tracking-wider">Skills & Expertise</h3>
                <button @click="analyzeSkills"
                        class="px-3 py-1 text-xs font-medium text-teal-600 bg-teal-50 rounded-lg hover:bg-teal-100 transition-colors flex items-center gap-1">
                  <i class="fas fa-brain"></i>
                  AI Analyze Skills
                </button>
              </div>
              <div class="flex flex-wrap gap-2">
                <span
                  v-for="skill in user.skills"
                  :key="skill"
                  class="px-3 py-1.5 bg-teal-50 rounded-full text-sm text-teal-700 hover:bg-teal-100 transition-colors cursor-pointer"
                >
                  {{ skill }}
                </span>
              </div>
            </div>

            <!-- Contact Info -->
            <div class="mt-6 grid grid-cols-1 md:grid-cols-2 gap-4">
              <div class="flex items-center gap-3 list-item-animated" style="animation-delay: 0.25s">
                <div class="w-10 h-10 rounded-lg bg-blue-100 flex items-center justify-center">
                  <i class="fas fa-envelope text-blue-600 icon-vibrant"></i>
                </div>
                <div>
                  <p class="text-xs text-teal-500">Email</p>
                  <p class="text-sm text-teal-800">{{ user.email }}</p>
                </div>
              </div>
              <div class="flex items-center gap-3 list-item-animated" style="animation-delay: 0.3s">
                <div class="w-10 h-10 rounded-lg bg-green-100 flex items-center justify-center">
                  <i class="fas fa-phone text-green-600 icon-vibrant"></i>
                </div>
                <div>
                  <p class="text-xs text-teal-500">Phone</p>
                  <p class="text-sm text-teal-800">{{ user.phone }}</p>
                </div>
              </div>
            </div>
          </div>

          <!-- Activity Timeline -->
          <div class="card-animated fade-in-up rounded-2xl p-6" style="animation-delay: 0.3s">
            <div class="flex items-center justify-between mb-6">
              <h2 class="text-lg font-semibold text-teal-900">Recent Activity</h2>
              <select v-model="activityFilter" class="input text-sm py-2 w-auto">
                <option value="all">All Activity</option>
                <option value="articles">Articles</option>
                <option value="comments">Comments</option>
                <option value="courses">Courses</option>
              </select>
            </div>

            <div class="relative">
              <!-- Timeline line -->
              <div class="absolute left-5 top-0 bottom-0 w-0.5 bg-teal-100"></div>

              <div
                v-for="(activity, idx) in activities"
                :key="idx"
                class="relative pl-14 pb-8 last:pb-0 list-item-animated"
                :style="{ 'animation-delay': (0.35 + idx * 0.05) + 's' }"
              >
                <div :class="['absolute left-2.5 w-5 h-5 rounded-full flex items-center justify-center', activity.iconBg]">
                  <i :class="[activity.icon, 'text-xs', activity.iconColor, 'icon-vibrant']"></i>
                </div>
                <div class="card-animated rounded-xl p-4 ml-2 hover:shadow-md transition-all ripple">
                  <div class="flex items-center justify-between mb-1">
                    <span :class="['badge text-xs', activity.typeBadge]">{{ activity.type }}</span>
                    <span class="text-xs text-teal-400">{{ activity.time }}</span>
                  </div>
                  <p class="text-teal-900 font-medium">{{ activity.title }}</p>
                  <p v-if="activity.description" class="text-sm text-teal-600 mt-1">{{ activity.description }}</p>
                </div>
              </div>
            </div>

            <button class="btn-vibrant ripple w-full mt-4 py-3 text-center text-teal-600 hover:text-teal-800 font-medium">
              View All Activity <i class="fas fa-arrow-right ml-1 icon-soft"></i>
            </button>
          </div>

          <!-- Contributions -->
          <div class="card-animated fade-in-up rounded-2xl p-6" style="animation-delay: 0.4s">
            <h2 class="text-lg font-semibold text-teal-900 mb-6">Contributions</h2>

            <div class="grid grid-cols-3 md:grid-cols-6 gap-4 mb-6">
              <div class="text-center p-4 rounded-xl bg-teal-50 list-item-animated ripple" style="animation-delay: 0.45s">
                <p class="text-2xl font-bold text-teal-600">{{ stats.articles }}</p>
                <p class="text-sm text-teal-500">Articles</p>
              </div>
              <div class="text-center p-4 rounded-xl bg-purple-50 list-item-animated ripple cursor-pointer hover:bg-purple-100" style="animation-delay: 0.46s">
                <p class="text-2xl font-bold text-purple-600">{{ followersCount }}</p>
                <p class="text-sm text-purple-500">Followers</p>
              </div>
              <div class="text-center p-4 rounded-xl bg-indigo-50 list-item-animated ripple cursor-pointer hover:bg-indigo-100" style="animation-delay: 0.47s">
                <p class="text-2xl font-bold text-indigo-600">{{ followingCount }}</p>
                <p class="text-sm text-indigo-500">Following</p>
              </div>
              <div class="text-center p-4 rounded-xl bg-blue-50 list-item-animated ripple" style="animation-delay: 0.5s">
                <p class="text-2xl font-bold text-blue-600">{{ stats.comments }}</p>
                <p class="text-sm text-teal-500">Comments</p>
              </div>
              <div class="text-center p-4 rounded-xl bg-purple-50 list-item-animated ripple" style="animation-delay: 0.55s">
                <p class="text-2xl font-bold text-purple-600">{{ stats.documents }}</p>
                <p class="text-sm text-teal-500">Documents</p>
              </div>
              <div class="text-center p-4 rounded-xl bg-green-50 list-item-animated ripple" style="animation-delay: 0.6s">
                <p class="text-2xl font-bold text-green-600">{{ stats.courses }}</p>
                <p class="text-sm text-teal-500">Courses Completed</p>
              </div>
            </div>

            <!-- Contribution Chart (Placeholder) -->
            <div class="h-32 bg-teal-50 rounded-xl flex items-center justify-center">
              <div class="flex items-end gap-1 h-24">
                <div
                  v-for="(val, idx) in contributionData"
                  :key="idx"
                  :style="{ height: val + '%' }"
                  class="w-3 bg-teal-400 rounded-t hover:bg-teal-500 transition-colors cursor-pointer"
                ></div>
              </div>
            </div>
            <p class="text-center text-sm text-teal-500 mt-2">Last 12 months contribution activity</p>
          </div>

          <!-- Content Authored -->
          <div class="card-animated fade-in-up rounded-2xl p-6" style="animation-delay: 0.5s">
            <div class="flex items-center justify-between mb-6">
              <h2 class="text-lg font-semibold text-teal-900">Content Authored</h2>
              <div class="flex gap-1 bg-teal-50 rounded-lg p-1">
                <button
                  @click="activeContentTab = 'articles'"
                  :class="[
                    'px-3 py-1.5 text-sm font-medium rounded-md transition-all',
                    activeContentTab === 'articles'
                      ? 'bg-white text-teal-700 shadow-sm'
                      : 'text-teal-600 hover:text-teal-800'
                  ]"
                >
                  Articles
                </button>
                <button
                  @click="activeContentTab = 'documents'"
                  :class="[
                    'px-3 py-1.5 text-sm font-medium rounded-md transition-all',
                    activeContentTab === 'documents'
                      ? 'bg-white text-teal-700 shadow-sm'
                      : 'text-teal-600 hover:text-teal-800'
                  ]"
                >
                  Documents
                </button>
                <button
                  @click="activeContentTab = 'polls'"
                  :class="[
                    'px-3 py-1.5 text-sm font-medium rounded-md transition-all',
                    activeContentTab === 'polls'
                      ? 'bg-white text-teal-700 shadow-sm'
                      : 'text-teal-600 hover:text-teal-800'
                  ]"
                >
                  Polls
                </button>
              </div>
            </div>

            <!-- Articles Tab -->
            <div v-if="activeContentTab === 'articles'" class="space-y-3">
              <router-link
                v-for="(article, idx) in authoredContent.articles"
                :key="article.id"
                :to="`/articles/${article.id}`"
                class="block p-4 rounded-xl bg-teal-50/50 hover:bg-teal-50 transition-all group list-item-animated ripple"
                :style="{ 'animation-delay': (0.55 + idx * 0.05) + 's' }"
              >
                <div class="flex items-start justify-between">
                  <div class="flex-1">
                    <h3 class="font-medium text-teal-900 group-hover:text-teal-700 transition-colors">
                      {{ article.title }}
                    </h3>
                    <div class="flex items-center gap-4 mt-2 text-sm text-teal-500">
                      <span><i class="fas fa-eye mr-1"></i>{{ article.views.toLocaleString() }} views</span>
                      <span><i class="fas fa-heart mr-1"></i>{{ article.likes }} likes</span>
                      <span><i class="fas fa-clock mr-1"></i>{{ article.date }}</span>
                    </div>
                  </div>
                  <i class="fas fa-chevron-right text-teal-400 group-hover:text-teal-600 transition-colors mt-1"></i>
                </div>
              </router-link>
              <button class="w-full py-2 text-center text-sm text-teal-600 hover:text-teal-800 font-medium">
                View All Articles <i class="fas fa-arrow-right ml-1"></i>
              </button>
            </div>

            <!-- Documents Tab -->
            <div v-if="activeContentTab === 'documents'" class="space-y-3">
              <router-link
                v-for="(doc, idx) in authoredContent.documents"
                :key="doc.id"
                :to="`/documents/${doc.id}`"
                class="block p-4 rounded-xl bg-purple-50/50 hover:bg-purple-50 transition-all group list-item-animated ripple"
                :style="{ 'animation-delay': (0.55 + idx * 0.05) + 's' }"
              >
                <div class="flex items-start justify-between">
                  <div class="flex items-start gap-3 flex-1">
                    <div class="w-10 h-10 rounded-lg bg-purple-100 flex items-center justify-center flex-shrink-0">
                      <i class="fas fa-file-alt text-purple-600"></i>
                    </div>
                    <div>
                      <h3 class="font-medium text-teal-900 group-hover:text-purple-700 transition-colors">
                        {{ doc.title }}
                      </h3>
                      <div class="flex items-center gap-4 mt-1 text-sm text-teal-500">
                        <span><i class="fas fa-download mr-1"></i>{{ doc.downloads }} downloads</span>
                        <span><i class="fas fa-clock mr-1"></i>{{ doc.date }}</span>
                      </div>
                    </div>
                  </div>
                  <i class="fas fa-chevron-right text-teal-400 group-hover:text-purple-600 transition-colors mt-3"></i>
                </div>
              </router-link>
              <button class="w-full py-2 text-center text-sm text-teal-600 hover:text-teal-800 font-medium">
                View All Documents <i class="fas fa-arrow-right ml-1"></i>
              </button>
            </div>

            <!-- Polls Tab -->
            <div v-if="activeContentTab === 'polls'" class="space-y-3">
              <router-link
                v-for="(poll, idx) in authoredContent.polls"
                :key="poll.id"
                :to="`/polls/${poll.id}`"
                class="block p-4 rounded-xl bg-blue-50/50 hover:bg-blue-50 transition-all group list-item-animated ripple"
                :style="{ 'animation-delay': (0.55 + idx * 0.05) + 's' }"
              >
                <div class="flex items-start justify-between">
                  <div class="flex items-start gap-3 flex-1">
                    <div class="w-10 h-10 rounded-lg bg-blue-100 flex items-center justify-center flex-shrink-0">
                      <i class="fas fa-poll text-blue-600"></i>
                    </div>
                    <div>
                      <h3 class="font-medium text-teal-900 group-hover:text-blue-700 transition-colors">
                        {{ poll.title }}
                      </h3>
                      <div class="flex items-center gap-4 mt-1 text-sm text-teal-500">
                        <span><i class="fas fa-vote-yea mr-1"></i>{{ poll.votes }} votes</span>
                        <span :class="[
                          'px-2 py-0.5 rounded-full text-xs font-medium',
                          poll.status === 'active' ? 'bg-green-100 text-green-700' : 'bg-gray-100 text-gray-600'
                        ]">
                          {{ poll.status === 'active' ? 'Active' : 'Completed' }}
                        </span>
                      </div>
                    </div>
                  </div>
                  <i class="fas fa-chevron-right text-teal-400 group-hover:text-blue-600 transition-colors mt-3"></i>
                </div>
              </router-link>
              <button class="w-full py-2 text-center text-sm text-teal-600 hover:text-teal-800 font-medium">
                View All Polls <i class="fas fa-arrow-right ml-1"></i>
              </button>
            </div>
          </div>
        </div>

        <!-- Right Column -->
        <div class="space-y-6">
          <!-- Badges & Achievements -->
          <div class="card-animated fade-in-up rounded-2xl p-6" style="animation-delay: 0.2s">
            <h2 class="text-lg font-semibold text-teal-900 mb-4">Badges & Achievements</h2>
            <div class="grid grid-cols-3 gap-4">
              <div
                v-for="(badge, idx) in badges"
                :key="badge.id"
                class="text-center group cursor-pointer list-item-animated ripple"
                :title="badge.description"
                :style="{ 'animation-delay': (0.25 + idx * 0.05) + 's' }"
              >
                <div :class="['w-14 h-14 rounded-xl mx-auto mb-2 flex items-center justify-center transition-transform group-hover:scale-110', badge.bg]">
                  <i :class="[badge.icon, badge.color, 'text-xl', 'icon-vibrant']"></i>
                </div>
                <p class="text-xs font-medium text-teal-700">{{ badge.name }}</p>
              </div>
            </div>
            <button class="btn-vibrant ripple w-full mt-4 py-2 text-center text-sm text-teal-600 hover:text-teal-800">
              View All Badges ({{ totalBadges }})
            </button>
          </div>

          <!-- Learning Progress -->
          <div class="card-animated fade-in-up rounded-2xl p-6" style="animation-delay: 0.3s">
            <h2 class="text-lg font-semibold text-teal-900 mb-4">Learning Progress</h2>
            <div class="space-y-4">
              <div
                v-for="(course, idx) in learningProgress"
                :key="course.id"
                class="list-item-animated"
                :style="{ 'animation-delay': (0.35 + idx * 0.05) + 's' }"
              >
                <div class="flex items-center justify-between mb-1">
                  <span class="text-sm text-teal-700">{{ course.name }}</span>
                  <span class="text-sm font-medium text-teal-600">{{ course.progress }}%</span>
                </div>
                <div class="progress">
                  <div class="progress-bar" :style="{ width: course.progress + '%' }"></div>
                </div>
              </div>
            </div>
            <button @click="goToLearning" class="btn-vibrant ripple block w-full mt-4 py-2 text-center text-sm text-teal-600 hover:text-teal-800">
              View All Courses <i class="fas fa-arrow-right ml-1 icon-soft"></i>
            </button>
          </div>

          <!-- Certificates -->
          <div class="card-animated fade-in-up rounded-2xl p-6" style="animation-delay: 0.4s">
            <h2 class="text-lg font-semibold text-teal-900 mb-4">Certificates</h2>
            <div class="space-y-3">
              <div
                v-for="(cert, idx) in certificates"
                :key="cert.id"
                class="flex items-center gap-3 p-3 rounded-xl bg-teal-50 hover:bg-teal-100 transition-colors cursor-pointer list-item-animated ripple"
                :style="{ 'animation-delay': (0.45 + idx * 0.05) + 's' }"
              >
                <div class="w-10 h-10 rounded-lg bg-gradient-to-br from-yellow-400 to-yellow-600 flex items-center justify-center">
                  <i class="fas fa-certificate text-white icon-vibrant"></i>
                </div>
                <div class="flex-1">
                  <p class="text-sm font-medium text-teal-900">{{ cert.name }}</p>
                  <p class="text-xs text-teal-500">{{ cert.date }}</p>
                </div>
                <button class="btn-vibrant ripple p-2 rounded-lg hover:bg-teal-200 text-teal-600">
                  <i class="fas fa-download icon-soft"></i>
                </button>
              </div>
            </div>
          </div>

          <!-- Team Members -->
          <div class="card-animated fade-in-up rounded-2xl p-6" style="animation-delay: 0.5s">
            <h2 class="text-lg font-semibold text-teal-900 mb-4">Team</h2>
            <div class="space-y-3">
              <div
                v-for="(member, idx) in teamMembers"
                :key="member.id"
                class="flex items-center gap-3 p-2 rounded-xl hover:bg-teal-50 transition-colors cursor-pointer list-item-animated ripple"
                :style="{ 'animation-delay': (0.55 + idx * 0.05) + 's' }"
              >
                <div class="relative">
                  <div
                    class="w-10 h-10 rounded-full flex items-center justify-center text-white font-medium"
                    :style="{ backgroundColor: member.color }"
                  >
                    {{ member.initials }}
                  </div>
                  <span
                    :class="[
                      'absolute -bottom-0.5 -right-0.5 w-3 h-3 rounded-full border-2 border-white',
                      member.online ? 'bg-green-500' : 'bg-gray-400'
                    ]"
                  ></span>
                </div>
                <div>
                  <p class="text-sm font-medium text-teal-900">{{ member.name }}</p>
                  <p class="text-xs text-teal-500">{{ member.role }}</p>
                </div>
              </div>
            </div>
          </div>

          <!-- Followers & Following -->
          <div class="card-animated fade-in-up rounded-2xl p-6" style="animation-delay: 0.6s">
            <div class="flex items-center justify-between mb-4">
              <h2 class="text-lg font-semibold text-teal-900">Connections</h2>
              <span class="text-xs text-teal-500">{{ mutualConnections }} mutual</span>
            </div>

            <!-- Quick Stats -->
            <div class="grid grid-cols-2 gap-3 mb-4">
              <div class="text-center p-3 rounded-xl bg-purple-50 cursor-pointer hover:bg-purple-100 transition-colors">
                <p class="text-lg font-bold text-purple-600">{{ followersCount }}</p>
                <p class="text-xs text-purple-500">Followers</p>
              </div>
              <div class="text-center p-3 rounded-xl bg-indigo-50 cursor-pointer hover:bg-indigo-100 transition-colors">
                <p class="text-lg font-bold text-indigo-600">{{ followingCount }}</p>
                <p class="text-xs text-indigo-500">Following</p>
              </div>
            </div>

            <!-- Recent Followers -->
            <p class="text-xs font-semibold text-teal-500 uppercase tracking-wider mb-3">Recent Followers</p>
            <div class="space-y-2">
              <div
                v-for="(follower, idx) in followers.slice(0, 4)"
                :key="follower.id"
                class="flex items-center justify-between p-2 rounded-xl hover:bg-teal-50 transition-colors list-item-animated"
                :style="{ 'animation-delay': (0.65 + idx * 0.05) + 's' }"
              >
                <div class="flex items-center gap-3">
                  <div class="w-9 h-9 rounded-full bg-gradient-to-br from-teal-400 to-teal-600 flex items-center justify-center text-white text-xs font-medium">
                    {{ follower.initials }}
                  </div>
                  <div>
                    <p class="text-sm font-medium text-teal-900">{{ follower.name }}</p>
                    <p class="text-xs text-teal-500">{{ follower.role }}</p>
                  </div>
                </div>
                <button
                  :class="[
                    'px-3 py-1 text-xs font-medium rounded-full transition-colors',
                    follower.isFollowing
                      ? 'bg-teal-100 text-teal-700 hover:bg-teal-200'
                      : 'bg-teal-500 text-white hover:bg-teal-600'
                  ]"
                >
                  {{ follower.isFollowing ? 'Following' : 'Follow Back' }}
                </button>
              </div>
            </div>

            <button class="btn-vibrant ripple w-full mt-4 py-2 text-center text-sm text-teal-600 hover:text-teal-800">
              View All Connections <i class="fas fa-arrow-right ml-1 icon-soft"></i>
            </button>
          </div>
        </div>
      </div>

      <!-- Edit Profile Modal -->
      <Teleport to="body">
        <div v-if="showEditProfile" class="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
          <div class="card-animated fade-in-up rounded-2xl w-full max-w-2xl max-h-[90vh] overflow-y-auto bg-white">
            <div class="p-5 border-b border-teal-100 flex items-center justify-between sticky top-0 bg-white/80 backdrop-blur-sm">
              <h2 class="text-xl font-semibold text-teal-900">Edit Profile</h2>
              <button @click="showEditProfile = false" class="btn-vibrant ripple p-2 rounded-lg hover:bg-teal-100 text-teal-500">
                <i class="fas fa-times icon-soft"></i>
              </button>
            </div>
            <form @submit.prevent="saveProfile" class="p-6 space-y-5">
              <div class="grid grid-cols-2 gap-4">
                <div>
                  <label class="block text-sm font-medium text-teal-700 mb-2">First Name</label>
                  <input type="text" v-model="editForm.firstName" class="input" required>
                </div>
                <div>
                  <label class="block text-sm font-medium text-teal-700 mb-2">Last Name</label>
                  <input type="text" v-model="editForm.lastName" class="input" required>
                </div>
              </div>
              <div>
                <label class="block text-sm font-medium text-teal-700 mb-2">Job Title</label>
                <input type="text" v-model="editForm.title" class="input">
              </div>
              <div>
                <label class="block text-sm font-medium text-teal-700 mb-2">Bio</label>
                <textarea v-model="editForm.bio" class="input" rows="4"></textarea>
              </div>
              <div>
                <label class="block text-sm font-medium text-teal-700 mb-2">Phone</label>
                <input type="tel" v-model="editForm.phone" class="input">
              </div>
              <div>
                <label class="block text-sm font-medium text-teal-700 mb-2">Location</label>
                <input type="text" v-model="editForm.location" class="input">
              </div>
              <div class="flex gap-3 pt-4">
                <button type="button" @click="showEditProfile = false" class="btn btn-secondary btn-vibrant ripple flex-1">Cancel</button>
                <button type="submit" class="btn btn-primary btn-vibrant ripple flex-1">Save Changes</button>
              </div>
            </form>
          </div>
        </div>

        <!-- AI Profile Summary Modal -->
        <div v-if="showAIProfileModal" class="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
          <div class="bg-white rounded-2xl shadow-2xl w-full max-w-2xl max-h-[85vh] overflow-hidden">
            <div class="p-6 border-b border-gray-100">
              <div class="flex items-center justify-between">
                <div class="flex items-center gap-3">
                  <div class="w-10 h-10 bg-gradient-to-br from-teal-500 to-emerald-500 rounded-xl flex items-center justify-center">
                    <i class="fas fa-wand-magic-sparkles text-white"></i>
                  </div>
                  <div>
                    <h3 class="text-lg font-semibold text-gray-900">AI Profile Summary</h3>
                    <p class="text-sm text-gray-500">Professional bio generated from your activities</p>
                  </div>
                </div>
                <button @click="showAIProfileModal = false" class="p-2 hover:bg-gray-100 rounded-lg transition-colors">
                  <i class="fas fa-times text-gray-400"></i>
                </button>
              </div>
            </div>

            <div class="p-6 overflow-y-auto max-h-[60vh]">
              <AILoadingIndicator v-if="isGeneratingProfile" message="Analyzing your profile data..." />

              <div v-else-if="aiProfileSummary" class="space-y-6">
                <!-- Confidence -->
                <div class="flex items-center gap-3 p-3 bg-teal-50 rounded-xl">
                  <AIConfidenceBar :value="aiProfileSummary.confidence" />
                  <span class="text-sm text-teal-700">{{ Math.round(aiProfileSummary.confidence * 100) }}% confidence based on your activities</span>
                </div>

                <!-- Professional Summary -->
                <div>
                  <h4 class="text-sm font-semibold text-gray-500 uppercase tracking-wider mb-3">Professional Summary</h4>
                  <p class="text-gray-800 leading-relaxed bg-gray-50 p-4 rounded-xl border border-gray-200">
                    {{ aiProfileSummary.professional }}
                  </p>
                </div>

                <!-- Strengths -->
                <div>
                  <h4 class="text-sm font-semibold text-gray-500 uppercase tracking-wider mb-3">Key Strengths</h4>
                  <ul class="space-y-2">
                    <li v-for="(strength, idx) in aiProfileSummary.strengths" :key="idx"
                        class="flex items-start gap-2 text-gray-700">
                      <i class="fas fa-check-circle text-teal-500 mt-1"></i>
                      {{ strength }}
                    </li>
                  </ul>
                </div>

                <!-- Areas of Expertise -->
                <div>
                  <h4 class="text-sm font-semibold text-gray-500 uppercase tracking-wider mb-3">Areas of Expertise</h4>
                  <div class="flex flex-wrap gap-2">
                    <span v-for="(exp, idx) in aiProfileSummary.expertise" :key="idx"
                          class="px-3 py-1.5 bg-teal-100 text-teal-700 rounded-full text-sm">
                      {{ exp }}
                    </span>
                  </div>
                </div>

                <!-- Impact Statement -->
                <div>
                  <h4 class="text-sm font-semibold text-gray-500 uppercase tracking-wider mb-3">Impact Statement</h4>
                  <div class="p-4 bg-gradient-to-r from-teal-50 to-emerald-50 rounded-xl border border-teal-200">
                    <i class="fas fa-quote-left text-teal-300 text-xl mb-2"></i>
                    <p class="text-gray-800 italic">{{ aiProfileSummary.impactStatement }}</p>
                  </div>
                </div>
              </div>
            </div>

            <div class="p-4 border-t border-gray-100 flex justify-end gap-3">
              <button @click="generateAIProfileSummary"
                      class="px-4 py-2 text-teal-600 hover:bg-teal-50 rounded-lg transition-colors flex items-center gap-2">
                <i class="fas fa-rotate"></i> Regenerate
              </button>
              <button @click="showAIProfileModal = false"
                      class="px-4 py-2 bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200 transition-colors">
                Cancel
              </button>
              <button @click="applyAISummary"
                      class="px-4 py-2 bg-gradient-to-r from-teal-500 to-emerald-500 text-white rounded-lg hover:from-teal-600 hover:to-emerald-600 transition-colors flex items-center gap-2">
                <i class="fas fa-check"></i> Apply to Profile
              </button>
            </div>
          </div>
        </div>

        <!-- AI Skills Analysis Modal -->
        <div v-if="showAISkillsModal" class="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
          <div class="bg-white rounded-2xl shadow-2xl w-full max-w-3xl max-h-[85vh] overflow-hidden">
            <div class="p-6 border-b border-gray-100">
              <div class="flex items-center justify-between">
                <div class="flex items-center gap-3">
                  <div class="w-10 h-10 bg-gradient-to-br from-purple-500 to-pink-500 rounded-xl flex items-center justify-center">
                    <i class="fas fa-brain text-white"></i>
                  </div>
                  <div>
                    <h3 class="text-lg font-semibold text-gray-900">AI Skills Analysis</h3>
                    <p class="text-sm text-gray-500">Skills detected from your activity patterns</p>
                  </div>
                </div>
                <button @click="showAISkillsModal = false" class="p-2 hover:bg-gray-100 rounded-lg transition-colors">
                  <i class="fas fa-times text-gray-400"></i>
                </button>
              </div>
            </div>

            <div class="p-6 overflow-y-auto max-h-[60vh]">
              <AILoadingIndicator v-if="isAnalyzingSkills" message="Analyzing your skills from activities..." />

              <div v-else-if="aiSkillAnalysis" class="space-y-6">
                <!-- Detected Skills -->
                <div>
                  <h4 class="text-sm font-semibold text-gray-500 uppercase tracking-wider mb-3">
                    <i class="fas fa-check-circle text-green-500 mr-2"></i>Detected Skills
                  </h4>
                  <div class="space-y-3">
                    <div v-for="skill in aiSkillAnalysis.detectedSkills" :key="skill.name"
                         class="p-4 bg-gray-50 rounded-xl border border-gray-200 hover:border-teal-300 transition-colors">
                      <div class="flex items-center justify-between mb-2">
                        <div class="flex items-center gap-3">
                          <span class="font-medium text-gray-900">{{ skill.name }}</span>
                          <span :class="['px-2 py-0.5 rounded-full text-xs font-medium capitalize', getSkillLevelColor(skill.level)]">
                            {{ skill.level }}
                          </span>
                        </div>
                        <div class="flex items-center gap-2">
                          <AIConfidenceBar :value="skill.confidence" size="sm" />
                          <span class="text-xs text-gray-500">{{ Math.round(skill.confidence * 100) }}%</span>
                        </div>
                      </div>
                      <p class="text-sm text-gray-500">
                        <i class="fas fa-info-circle mr-1"></i>{{ skill.source }}
                      </p>
                    </div>
                  </div>
                </div>

                <!-- Suggested Skills -->
                <div>
                  <h4 class="text-sm font-semibold text-gray-500 uppercase tracking-wider mb-3">
                    <i class="fas fa-lightbulb text-yellow-500 mr-2"></i>Suggested Skills to Add
                  </h4>
                  <div class="grid grid-cols-1 md:grid-cols-2 gap-3">
                    <div v-for="skill in aiSkillAnalysis.suggestedSkills" :key="skill.name"
                         class="p-4 bg-yellow-50 rounded-xl border border-yellow-200 hover:border-yellow-400 transition-colors cursor-pointer"
                         @click="addSuggestedSkill(skill)">
                      <div class="flex items-center justify-between mb-2">
                        <span class="font-medium text-gray-900">{{ skill.name }}</span>
                        <span class="text-xs text-yellow-700 bg-yellow-100 px-2 py-0.5 rounded-full">
                          {{ Math.round(skill.relevance * 100) }}% relevant
                        </span>
                      </div>
                      <p class="text-sm text-gray-600">{{ skill.reason }}</p>
                      <button class="mt-2 text-xs text-teal-600 font-medium flex items-center gap-1 hover:text-teal-700">
                        <i class="fas fa-plus"></i> Add to Profile
                      </button>
                    </div>
                  </div>
                </div>

                <!-- Skill Gaps -->
                <div>
                  <h4 class="text-sm font-semibold text-gray-500 uppercase tracking-wider mb-3">
                    <i class="fas fa-exclamation-triangle text-orange-500 mr-2"></i>Skill Gap Analysis
                  </h4>
                  <div class="space-y-3">
                    <div v-for="gap in aiSkillAnalysis.skillGaps" :key="gap.skill"
                         class="p-4 bg-orange-50 rounded-xl border border-orange-200">
                      <div class="flex items-center justify-between mb-2">
                        <span class="font-medium text-gray-900">{{ gap.skill }}</span>
                        <span class="text-xs text-orange-700">{{ gap.importance }}</span>
                      </div>
                      <p class="text-sm text-gray-600">
                        <i class="fas fa-graduation-cap mr-1 text-orange-500"></i>{{ gap.recommendation }}
                      </p>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <div class="p-4 border-t border-gray-100 flex justify-end gap-3">
              <button @click="analyzeSkills"
                      class="px-4 py-2 text-teal-600 hover:bg-teal-50 rounded-lg transition-colors flex items-center gap-2">
                <i class="fas fa-rotate"></i> Re-analyze
              </button>
              <button @click="showAISkillsModal = false"
                      class="px-4 py-2 bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200 transition-colors">
                Close
              </button>
            </div>
          </div>
        </div>

        <!-- AI Profile Insights Modal -->
        <div v-if="showAIInsightsModal" class="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
          <div class="bg-white rounded-2xl shadow-2xl w-full max-w-2xl max-h-[85vh] overflow-hidden">
            <div class="p-6 border-b border-gray-100">
              <div class="flex items-center justify-between">
                <div class="flex items-center gap-3">
                  <div class="w-10 h-10 bg-gradient-to-br from-teal-500 to-emerald-500 rounded-xl flex items-center justify-center">
                    <i class="fas fa-chart-line text-white"></i>
                  </div>
                  <div>
                    <h3 class="text-lg font-semibold text-gray-900">AI Profile Insights</h3>
                    <p class="text-sm text-gray-500">Personalized insights based on your activity</p>
                  </div>
                </div>
                <button @click="showAIInsightsModal = false" class="p-2 hover:bg-gray-100 rounded-lg transition-colors">
                  <i class="fas fa-times text-gray-400"></i>
                </button>
              </div>
            </div>

            <div class="p-6 overflow-y-auto max-h-[60vh]">
              <AILoadingIndicator v-if="isLoadingInsights" message="Generating personalized insights..." />

              <div v-else-if="aiProfileInsights.length > 0" class="space-y-4">
                <div v-for="insight in aiProfileInsights" :key="insight.id"
                     class="p-4 rounded-xl border border-gray-200 hover:border-teal-300 hover:shadow-md transition-all">
                  <div class="flex items-start gap-4">
                    <div :class="['w-10 h-10 rounded-xl flex items-center justify-center flex-shrink-0', getInsightColor(insight.type)]">
                      <i :class="getInsightIcon(insight.type)"></i>
                    </div>
                    <div class="flex-1">
                      <div class="flex items-center justify-between mb-1">
                        <h4 class="font-semibold text-gray-900">{{ insight.title }}</h4>
                        <span v-if="insight.metric" class="text-xs text-teal-600 bg-teal-50 px-2 py-1 rounded-full">
                          {{ insight.metric }}
                        </span>
                      </div>
                      <p class="text-sm text-gray-600">{{ insight.description }}</p>
                      <button v-if="insight.actionLabel"
                              class="mt-2 text-sm text-teal-600 font-medium flex items-center gap-1 hover:text-teal-700">
                        {{ insight.actionLabel }} <i class="fas fa-arrow-right text-xs"></i>
                      </button>
                    </div>
                  </div>
                </div>
              </div>

              <div v-else class="text-center py-8 text-gray-500">
                <i class="fas fa-chart-pie text-4xl mb-3 text-gray-300"></i>
                <p>No insights available yet. Keep contributing to unlock personalized insights!</p>
              </div>
            </div>

            <div class="p-4 border-t border-gray-100 flex justify-end gap-3">
              <button @click="loadAIInsights"
                      class="px-4 py-2 text-teal-600 hover:bg-teal-50 rounded-lg transition-colors flex items-center gap-2">
                <i class="fas fa-rotate"></i> Refresh
              </button>
              <button @click="showAIInsightsModal = false"
                      class="px-4 py-2 bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200 transition-colors">
                Close
              </button>
            </div>
          </div>
        </div>
      </Teleport>
    </template>
  </div>
</template>

<style scoped>
/* Badge styles for activity types */
.badge-blue {
  background-color: #dbeafe;
  color: #1d4ed8;
}

.badge-purple {
  background-color: #ede9fe;
  color: #7c3aed;
}

.badge-green {
  background-color: #dcfce7;
  color: #15803d;
}

.badge-orange {
  background-color: #ffedd5;
  color: #c2410c;
}

/* Progress bar styles */
.progress {
  height: 6px;
  background-color: #e5e7eb;
  border-radius: 9999px;
  overflow: hidden;
}

.progress-bar {
  height: 100%;
  background: linear-gradient(90deg, #14b8a6, #0d9488);
  border-radius: 9999px;
  transition: width 0.5s ease-out;
}

/* Cover photo button icon styling */
.h-48 .icon-soft {
  color: inherit;
}
</style>
