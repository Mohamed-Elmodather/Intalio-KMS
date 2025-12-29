<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'

// Time of day greeting
const timeOfDay = computed(() => {
  const hour = new Date().getHours()
  if (hour < 12) return 'morning'
  if (hour < 17) return 'afternoon'
  return 'evening'
})

// Stats data
const stats = ref([
  { label: 'Total Articles', value: '2,847', numValue: 2847, displayValue: 0, icon: 'fas fa-newspaper', iconBg: 'bg-primary-100', iconColor: 'text-primary-600', trend: '12%', trendUp: true, link: '/articles', linkText: 'View Articles' },
  { label: 'Active Users', value: '1,234', numValue: 1234, displayValue: 0, icon: 'fas fa-users', iconBg: 'bg-blue-100', iconColor: 'text-blue-600', trend: '8%', trendUp: true, link: '/collaboration', linkText: 'View Users' },
  { label: 'Documents', value: '8,492', numValue: 8492, displayValue: 0, icon: 'fas fa-file-alt', iconBg: 'bg-violet-100', iconColor: 'text-violet-600', trend: '23%', trendUp: true, link: '/documents', linkText: 'View Documents' },
  { label: 'Courses Completed', value: '456', numValue: 456, displayValue: 0, icon: 'fas fa-graduation-cap', iconBg: 'bg-amber-100', iconColor: 'text-amber-600', trend: '5%', trendUp: false, link: '/learning', linkText: 'View Courses' },
])

// Format number with commas
const formatNumber = (num: number) => {
  return num.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',')
}

// Animated counter function
const animateCounter = (stat: any, duration = 2000) => {
  const target = stat.numValue
  const startTime = performance.now()
  const startValue = 0

  const easeOutQuart = (t: number) => 1 - Math.pow(1 - t, 4)

  const updateCounter = (currentTime: number) => {
    const elapsed = currentTime - startTime
    const progress = Math.min(elapsed / duration, 1)
    const easedProgress = easeOutQuart(progress)

    stat.displayValue = Math.floor(startValue + (target - startValue) * easedProgress)

    if (progress < 1) {
      requestAnimationFrame(updateCounter)
    } else {
      stat.displayValue = target
    }
  }

  requestAnimationFrame(updateCounter)
}

// AI Actions
const aiActions = ref([
  { icon: 'fas fa-wand-magic-sparkles', label: 'Generate article summary' },
  { icon: 'fas fa-language', label: 'Translate content' },
  { icon: 'fas fa-magnifying-glass-plus', label: 'Find related documents' },
  { icon: 'fas fa-chart-line', label: 'Analyze engagement' },
])

// Recent Articles
const recentArticles = ref([
  {
    id: 1,
    title: 'Best Practices for Remote Team Collaboration',
    excerpt: 'Discover proven strategies to enhance productivity and communication in distributed teams.',
    category: 'Collaboration',
    categoryClass: 'bg-blue-100 text-blue-600',
    readTime: '5 min read',
    icon: 'fas fa-users',
    iconBg: 'bg-blue-100',
    iconColor: 'text-blue-500',
    author: { name: 'Sarah Chen', initials: 'SC' },
    date: '2 hours ago'
  },
  {
    id: 2,
    title: 'Q4 Security Compliance Guidelines',
    excerpt: 'Updated security protocols and compliance requirements for all departments.',
    category: 'Security',
    categoryClass: 'bg-rose-100 text-rose-600',
    readTime: '8 min read',
    icon: 'fas fa-shield-halved',
    iconBg: 'bg-rose-100',
    iconColor: 'text-rose-500',
    author: { name: 'Mike Johnson', initials: 'MJ' },
    date: '5 hours ago'
  },
  {
    id: 3,
    title: 'New Product Launch Training Materials',
    excerpt: 'Comprehensive training guide for the upcoming product release.',
    category: 'Training',
    categoryClass: 'bg-emerald-100 text-emerald-600',
    readTime: '12 min read',
    icon: 'fas fa-rocket',
    iconBg: 'bg-emerald-100',
    iconColor: 'text-emerald-500',
    author: { name: 'Emily Davis', initials: 'ED' },
    date: 'Yesterday'
  }
])

// Upcoming Events
const upcomingEvents = ref([
  {
    id: 1,
    title: 'All-Hands Town Hall Meeting',
    month: 'Dec',
    day: '24',
    time: '10:00 AM - 11:30 AM',
    attendees: [
      { color: '#14B8A6' },
      { color: '#3B82F6' },
      { color: '#8B5CF6' },
      { color: '#F59E0B' },
      { color: '#EF4444' }
    ]
  },
  {
    id: 2,
    title: 'Product Demo & Q&A Session',
    month: 'Dec',
    day: '26',
    time: '2:00 PM - 3:00 PM',
    attendees: [
      { color: '#10B981' },
      { color: '#6366F1' },
      { color: '#EC4899' }
    ]
  },
  {
    id: 3,
    title: 'Year-End Review Workshop',
    month: 'Dec',
    day: '30',
    time: '9:00 AM - 12:00 PM',
    attendees: [
      { color: '#F59E0B' },
      { color: '#14B8A6' }
    ]
  }
])

// Active Polls
const activePolls = ref([
  {
    id: 1,
    question: 'Preferred format for weekly updates?',
    options: [
      { label: 'Video briefing', votes: 45 },
      { label: 'Newsletter email', votes: 32 },
      { label: 'Slack summary', votes: 23 }
    ],
    totalVotes: 234,
    endsIn: 'Ends in 2 days'
  }
])

// Learning Courses
const learningCourses = ref([
  {
    id: 1,
    title: 'Advanced Data Analytics',
    instructor: 'Dr. James Wilson',
    progress: 75,
    icon: 'fas fa-chart-bar',
    iconBg: 'bg-violet-100',
    iconColor: 'text-violet-600'
  },
  {
    id: 2,
    title: 'Leadership Essentials',
    instructor: 'Maria Garcia',
    progress: 45,
    icon: 'fas fa-crown',
    iconBg: 'bg-amber-100',
    iconColor: 'text-amber-600'
  },
  {
    id: 3,
    title: 'Cybersecurity Fundamentals',
    instructor: 'Alex Thompson',
    progress: 90,
    icon: 'fas fa-lock',
    iconBg: 'bg-rose-100',
    iconColor: 'text-rose-600'
  }
])

// Media Items
const mediaItems = ref([
  {
    id: 1,
    title: 'CEO Vision 2025 Address',
    type: 'video',
    duration: '15:42',
    views: '1.2K',
    gradientClass: 'bg-gradient-to-br from-primary-400 to-primary-600'
  },
  {
    id: 2,
    title: 'Product Innovation Series',
    type: 'video',
    duration: '28:15',
    views: '856',
    gradientClass: 'bg-gradient-to-br from-blue-400 to-blue-600'
  },
  {
    id: 3,
    title: 'Weekly Strategy Podcast',
    type: 'audio',
    duration: '45:00',
    views: '432',
    gradientClass: 'bg-gradient-to-br from-violet-400 to-violet-600'
  },
  {
    id: 4,
    title: 'Customer Success Stories',
    type: 'video',
    duration: '12:30',
    views: '678',
    gradientClass: 'bg-gradient-to-br from-amber-400 to-amber-600'
  }
])

// Recent Documents
const recentDocuments = ref([
  {
    id: 1,
    name: 'Q4 Financial Report 2024.xlsx',
    size: '2.4 MB',
    modified: '2 hours ago',
    icon: 'fas fa-file-excel',
    iconBg: 'bg-emerald-100',
    iconColor: 'text-emerald-600'
  },
  {
    id: 2,
    name: 'Brand Guidelines v3.pdf',
    size: '8.7 MB',
    modified: 'Yesterday',
    icon: 'fas fa-file-pdf',
    iconBg: 'bg-rose-100',
    iconColor: 'text-rose-600'
  },
  {
    id: 3,
    name: 'Project Roadmap 2025.pptx',
    size: '5.2 MB',
    modified: '3 days ago',
    icon: 'fas fa-file-powerpoint',
    iconBg: 'bg-orange-100',
    iconColor: 'text-orange-600'
  },
  {
    id: 4,
    name: 'Employee Handbook.docx',
    size: '1.8 MB',
    modified: 'Last week',
    icon: 'fas fa-file-word',
    iconBg: 'bg-blue-100',
    iconColor: 'text-blue-600'
  }
])

// Team Activities
const teamActivities = ref([
  {
    id: 1,
    user: { name: 'Sarah Chen', initials: 'SC', color: '#8B5CF6' },
    action: 'published a new article',
    target: '"Remote Work Best Practices"',
    time: '5 minutes ago',
    actionIcon: 'fas fa-pen',
    actionBg: 'bg-violet-100',
    actionColor: 'text-violet-600'
  },
  {
    id: 2,
    user: { name: 'Mike Johnson', initials: 'MJ', color: '#3B82F6' },
    action: 'uploaded document to',
    target: 'IT Knowledge Base',
    time: '15 minutes ago',
    actionIcon: 'fas fa-upload',
    actionBg: 'bg-blue-100',
    actionColor: 'text-blue-600'
  },
  {
    id: 3,
    user: { name: 'Emily Davis', initials: 'ED', color: '#10B981' },
    action: 'completed course',
    target: '"Leadership Essentials"',
    time: '1 hour ago',
    actionIcon: 'fas fa-check',
    actionBg: 'bg-emerald-100',
    actionColor: 'text-emerald-600'
  },
  {
    id: 4,
    user: { name: 'Alex Thompson', initials: 'AT', color: '#F59E0B' },
    action: 'created new event',
    target: '"Q1 Planning Session"',
    time: '2 hours ago',
    actionIcon: 'fas fa-calendar-plus',
    actionBg: 'bg-amber-100',
    actionColor: 'text-amber-600'
  }
])

// Self Services
const selfServices = ref([
  { id: 1, label: 'IT Support', icon: 'fas fa-headset', iconBg: 'bg-blue-100', iconColor: 'text-blue-600' },
  { id: 2, label: 'HR Requests', icon: 'fas fa-user-tie', iconBg: 'bg-violet-100', iconColor: 'text-violet-600' },
  { id: 3, label: 'Book Room', icon: 'fas fa-door-open', iconBg: 'bg-emerald-100', iconColor: 'text-emerald-600' },
  { id: 4, label: 'Expenses', icon: 'fas fa-receipt', iconBg: 'bg-amber-100', iconColor: 'text-amber-600' },
  { id: 5, label: 'Time Off', icon: 'fas fa-umbrella-beach', iconBg: 'bg-primary-100', iconColor: 'text-primary-600' },
  { id: 6, label: 'Feedback', icon: 'fas fa-comment-dots', iconBg: 'bg-rose-100', iconColor: 'text-rose-600' }
])

// Carousel State
const currentSlide = ref(0)
const isAutoPlaying = ref(true)
let carouselInterval: number | null = null

const recentUpdates = ref([
  {
    id: 1,
    type: 'announcement',
    typeLabel: 'Announcement',
    typeIcon: 'fas fa-bullhorn',
    title: 'Company-Wide Town Hall Meeting Next Week',
    description: 'Join us for our quarterly town hall meeting where the leadership team will share updates on company performance, upcoming initiatives, and answer your questions.',
    icon: 'fas fa-users',
    gradientClass: 'bg-gradient-to-br from-amber-400 to-orange-500',
    date: 'Dec 23, 2024',
    author: { name: 'CEO Office', initials: 'CO', role: 'Executive Team', color: '#f59e0b' },
    views: '1,247',
    comments: '34',
    link: '/events',
    actionText: 'View Details'
  },
  {
    id: 2,
    type: 'article',
    typeLabel: 'New Article',
    typeIcon: 'fas fa-newspaper',
    title: 'Best Practices for Remote Team Collaboration in 2025',
    description: 'Discover the latest strategies and tools for effective remote collaboration. This comprehensive guide covers communication best practices and project management tips.',
    icon: 'fas fa-laptop-house',
    gradientClass: 'bg-gradient-to-br from-blue-400 to-indigo-500',
    date: 'Dec 22, 2024',
    author: { name: 'Sarah Chen', initials: 'SC', role: 'HR Director', color: '#3b82f6' },
    views: '856',
    comments: '23',
    link: '/articles',
    actionText: 'Read Article'
  },
  {
    id: 3,
    type: 'event',
    typeLabel: 'Upcoming Event',
    typeIcon: 'fas fa-calendar-star',
    title: 'Annual Holiday Celebration & Awards Ceremony',
    description: 'Join us for our annual holiday celebration featuring dinner, entertainment, and the presentation of employee excellence awards. RSVP by December 20th.',
    icon: 'fas fa-champagne-glasses',
    gradientClass: 'bg-gradient-to-br from-emerald-400 to-teal-500',
    date: 'Dec 28, 2024',
    author: { name: 'Events Team', initials: 'ET', role: 'Culture & Events', color: '#10b981' },
    views: '2,103',
    comments: '67',
    link: '/events',
    actionText: 'RSVP Now'
  }
])

// Carousel Methods
const nextSlide = () => {
  currentSlide.value = (currentSlide.value + 1) % recentUpdates.value.length
}

const prevSlide = () => {
  currentSlide.value = currentSlide.value === 0
    ? recentUpdates.value.length - 1
    : currentSlide.value - 1
}

const goToSlide = (index: number) => {
  currentSlide.value = index
}

const startAutoPlay = () => {
  if (carouselInterval) clearInterval(carouselInterval)
  carouselInterval = window.setInterval(() => {
    if (isAutoPlaying.value) {
      nextSlide()
    }
  }, 5000)
}

const pauseCarousel = () => {
  isAutoPlaying.value = false
}

const resumeCarousel = () => {
  isAutoPlaying.value = true
}

const toggleAutoPlay = () => {
  isAutoPlaying.value = !isAutoPlaying.value
}

// Lifecycle
onMounted(() => {
  // Start counter animation
  setTimeout(() => {
    stats.value.forEach((stat, index) => {
      setTimeout(() => {
        animateCounter(stat, 1500)
      }, index * 200)
    })
  }, 500)

  // Start carousel
  startAutoPlay()
})

onUnmounted(() => {
  if (carouselInterval) clearInterval(carouselInterval)
})
</script>

<template>
  <div>
    <!-- Welcome Section -->
    <div class="card-animated rounded-2xl p-8 mb-8 relative overflow-hidden stagger-1">
      <!-- Decorative elements -->
      <div class="welcome-decoration w-64 h-64 -top-32 -right-32" style="animation-delay: 0s;"></div>
      <div class="welcome-decoration w-48 h-48 -bottom-24 -left-24" style="animation-delay: 2s;"></div>
      <div class="welcome-decoration w-32 h-32 top-1/2 right-1/4" style="animation-delay: 4s;"></div>

      <div class="relative flex items-center justify-between">
        <div>
          <h1 class="text-2xl font-bold text-gray-900 mb-2 fade-in-up" style="animation-delay: 0.3s;">
            Good {{ timeOfDay }}, Ahmed <span class="text-xl inline-block icon-float">👋</span>
          </h1>
          <p class="text-gray-500 max-w-lg fade-in-up" style="animation-delay: 0.4s;">
            You have <span class="font-medium text-primary-600">3 pending approvals</span> and
            <span class="font-medium text-primary-600">2 new courses</span> waiting for you.
          </p>
          <div class="flex gap-3 mt-6 fade-in-up" style="animation-delay: 0.5s;">
            <router-link to="/learning" class="px-5 py-2.5 btn-vibrant text-white rounded-xl font-medium text-sm flex items-center gap-2 ripple">
              <i class="fas fa-play text-xs"></i>
              Continue Learning
            </router-link>
            <router-link to="/articles/new" class="px-5 py-2.5 bg-white border border-gray-200 text-gray-700 rounded-xl font-medium text-sm hover:bg-primary-50 hover:border-primary-200 hover:text-primary-700 transition-all flex items-center gap-2 ripple">
              <i class="fas fa-plus text-xs"></i>
              Create Content
            </router-link>
          </div>
        </div>
        <div class="hidden lg:flex items-center justify-center scale-in" style="animation-delay: 0.6s;">
          <div class="w-36 h-36 rounded-2xl bg-gradient-to-br from-primary-400 to-primary-600 flex items-center justify-center shadow-2xl shadow-primary-500/30 icon-float">
            <i class="fas fa-lightbulb text-white text-5xl"></i>
          </div>
        </div>
      </div>
    </div>

    <!-- Recent Updates Carousel -->
    <div class="card-animated rounded-2xl p-6 mb-8 stagger-1" @mouseenter="pauseCarousel" @mouseleave="resumeCarousel">
      <div class="flex items-center justify-between mb-4">
        <h2 class="text-lg font-semibold text-gray-900 flex items-center gap-3">
          <div class="icon-vibrant w-9 h-9 rounded-xl flex items-center justify-center">
            <i class="fas fa-bullhorn text-white text-sm"></i>
          </div>
          Recent Updates
        </h2>
        <div class="flex items-center gap-2">
          <span class="text-xs text-gray-500">{{ currentSlide + 1 }} / {{ recentUpdates.length }}</span>
          <button @click="toggleAutoPlay" class="p-2 rounded-lg hover:bg-primary-50 text-gray-500 hover:text-primary-600 transition-all">
            <i :class="isAutoPlaying ? 'fas fa-pause' : 'fas fa-play'" class="text-xs"></i>
          </button>
        </div>
      </div>

      <!-- Progress bar -->
      <div class="h-1 bg-gray-100 rounded-full mb-4 overflow-hidden">
        <div class="slide-progress rounded-full" :key="currentSlide" v-if="isAutoPlaying"></div>
      </div>

      <div class="updates-carousel">
        <!-- Navigation Buttons -->
        <button @click="prevSlide" class="carousel-nav-btn prev">
          <i class="fas fa-chevron-left"></i>
        </button>
        <button @click="nextSlide" class="carousel-nav-btn next">
          <i class="fas fa-chevron-right"></i>
        </button>

        <!-- Slides -->
        <div class="carousel-track" :style="{ transform: 'translateX(-' + (currentSlide * 100) + '%)' }">
          <div v-for="(update, index) in recentUpdates" :key="update.id"
               class="carousel-slide"
               :class="{ active: index === currentSlide }">
            <div class="flex flex-col lg:flex-row gap-6 p-4">
              <!-- Image/Icon Side -->
              <div class="lg:w-1/3">
                <div class="relative h-48 lg:h-full min-h-[180px] rounded-2xl overflow-hidden"
                     :class="update.gradientClass">
                  <div class="absolute inset-0 flex items-center justify-center">
                    <div class="w-20 h-20 rounded-2xl bg-white/20 backdrop-blur-sm flex items-center justify-center">
                      <i :class="[update.icon, 'text-4xl text-white']"></i>
                    </div>
                  </div>
                  <div class="absolute top-4 left-4">
                    <span :class="['update-badge', update.type]">
                      <i :class="update.typeIcon"></i>
                      {{ update.typeLabel }}
                    </span>
                  </div>
                  <div class="absolute bottom-4 right-4 px-3 py-1 rounded-lg bg-black/40 backdrop-blur-sm text-white text-xs">
                    {{ update.date }}
                  </div>
                </div>
              </div>

              <!-- Content Side -->
              <div class="lg:w-2/3 flex flex-col justify-center">
                <h3 class="text-xl font-bold text-gray-900 mb-3">{{ update.title }}</h3>
                <p class="text-gray-600 mb-4 line-clamp-3">{{ update.description }}</p>
                <div class="flex items-center gap-4 mb-4">
                  <div class="flex items-center gap-2">
                    <div class="w-8 h-8 rounded-lg flex items-center justify-center text-white text-xs font-semibold"
                         :style="{ backgroundColor: update.author.color }">
                      {{ update.author.initials }}
                    </div>
                    <div>
                      <p class="text-sm font-medium text-gray-900">{{ update.author.name }}</p>
                      <p class="text-xs text-gray-500">{{ update.author.role }}</p>
                    </div>
                  </div>
                  <div class="flex items-center gap-3 text-xs text-gray-500">
                    <span><i class="fas fa-eye mr-1"></i>{{ update.views }}</span>
                    <span><i class="fas fa-comment mr-1"></i>{{ update.comments }}</span>
                  </div>
                </div>
                <div class="flex gap-3">
                  <router-link :to="update.link" class="px-5 py-2.5 btn-vibrant text-white rounded-xl font-medium text-sm flex items-center gap-2">
                    <i class="fas fa-arrow-right text-xs"></i>
                    {{ update.actionText }}
                  </router-link>
                  <button class="px-4 py-2.5 bg-white border border-gray-200 text-gray-700 rounded-xl font-medium text-sm hover:bg-primary-50 hover:border-primary-200 hover:text-primary-700 transition-all">
                    <i class="fas fa-bookmark"></i>
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Dots Navigation -->
      <div class="carousel-dots">
        <button v-for="(update, index) in recentUpdates" :key="'dot-' + index"
                @click="goToSlide(index)"
                class="carousel-dot"
                :class="{ active: index === currentSlide }">
        </button>
      </div>
    </div>

    <!-- Stats Grid -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-5 mb-8">
      <div v-for="(stat, index) in stats" :key="stat.label"
           class="stat-card card-animated rounded-2xl p-5"
           :style="{ animationDelay: index * 100 + 'ms' }">
        <div class="flex items-center justify-between mb-4">
          <div :class="['w-11 h-11 rounded-xl flex items-center justify-center', stat.iconBg]">
            <i :class="[stat.icon, stat.iconColor]"></i>
          </div>
          <div class="flex items-center gap-1 text-xs" :class="stat.trendUp ? 'text-emerald-600' : 'text-rose-600'">
            <i :class="stat.trendUp ? 'fas fa-arrow-up' : 'fas fa-arrow-down'"></i>
            {{ stat.trend }}
          </div>
        </div>
        <div class="stat-value text-3xl font-bold text-gray-900">{{ formatNumber(stat.displayValue) }}</div>
        <div class="text-sm text-gray-500 mt-1">{{ stat.label }}</div>
        <router-link :to="stat.link" class="text-xs text-primary-600 font-medium mt-3 flex items-center gap-1 hover:text-primary-700">
          {{ stat.linkText }} <i class="fas fa-arrow-right text-[10px]"></i>
        </router-link>
      </div>
    </div>

    <!-- Main Grid Row 1 -->
    <div class="grid grid-cols-1 lg:grid-cols-3 gap-6 mb-8">
      <!-- Recent Articles -->
      <div class="lg:col-span-2 card-animated rounded-2xl p-6 stagger-2">
        <div class="flex items-center justify-between mb-5">
          <h2 class="text-lg font-semibold text-gray-900 flex items-center gap-3">
            <div class="icon-vibrant w-9 h-9 rounded-xl flex items-center justify-center">
              <i class="fas fa-newspaper text-white text-sm"></i>
            </div>
            Recent Articles
          </h2>
          <router-link to="/articles" class="text-sm text-primary-600 hover:text-primary-700 font-medium flex items-center gap-1 group">
            View All <i class="fas fa-arrow-right text-xs group-hover:translate-x-1 transition-transform"></i>
          </router-link>
        </div>
        <div class="space-y-3">
          <div v-for="article in recentArticles" :key="article.id"
               class="list-item-animated flex gap-4 p-4 rounded-xl cursor-pointer">
            <div class="w-16 h-16 rounded-xl flex-shrink-0 flex items-center justify-center transition-transform hover:scale-110 hover:rotate-3"
                 :class="article.iconBg">
              <i :class="[article.icon, 'text-xl', article.iconColor]"></i>
            </div>
            <div class="flex-1 min-w-0">
              <div class="flex items-center gap-2 mb-1">
                <span :class="['text-xs font-medium px-2 py-0.5 rounded-md', article.categoryClass]">
                  {{ article.category }}
                </span>
                <span class="text-xs text-gray-400">{{ article.readTime }}</span>
              </div>
              <h4 class="font-medium text-gray-900 hover:text-primary-600 transition-colors truncate">
                {{ article.title }}
              </h4>
              <p class="text-sm text-gray-500 mt-1 line-clamp-1">{{ article.excerpt }}</p>
              <div class="flex items-center gap-3 mt-2">
                <div class="flex items-center gap-2">
                  <div class="w-5 h-5 rounded-md bg-gradient-to-br from-primary-400 to-primary-600 flex items-center justify-center text-white text-[10px] font-semibold">
                    {{ article.author.initials }}
                  </div>
                  <span class="text-xs text-gray-500">{{ article.author.name }}</span>
                </div>
                <span class="text-xs text-gray-400">{{ article.date }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- AI Assistant -->
      <div class="card-animated rounded-2xl p-6 relative overflow-hidden stagger-3">
        <div class="welcome-decoration w-24 h-24 -top-12 -right-12"></div>
        <div class="relative">
          <div class="flex items-center gap-3 mb-5">
            <div class="w-11 h-11 rounded-xl bg-gradient-to-br from-primary-400 to-primary-600 flex items-center justify-center ai-glow">
              <i class="fas fa-robot text-white"></i>
            </div>
            <div>
              <h3 class="font-semibold text-gray-900">AI Assistant</h3>
              <p class="text-xs text-gray-500">Powered by AI</p>
            </div>
          </div>

          <div class="space-y-2 mb-5">
            <button v-for="(action, index) in aiActions" :key="action.label"
                    class="w-full text-left p-3 rounded-xl bg-gray-50 hover:bg-primary-50 transition-all group flex items-center gap-3"
                    :style="{ animationDelay: (index * 0.1) + 's' }">
              <i :class="[action.icon, 'text-gray-400 group-hover:text-primary-500 transition-colors']"></i>
              <span class="text-sm text-gray-600 group-hover:text-gray-900 transition-colors">{{ action.label }}</span>
            </button>
          </div>

          <div class="relative">
            <input type="text"
                   placeholder="Ask anything..."
                   class="w-full px-4 py-3 pr-12 rounded-xl bg-gray-50 border border-gray-100 focus:bg-white focus:border-primary-300 focus:ring-4 focus:ring-primary-100 outline-none text-sm text-gray-900 placeholder-gray-400 transition-all">
            <button class="absolute right-2 top-1/2 -translate-y-1/2 w-8 h-8 rounded-lg btn-vibrant text-white flex items-center justify-center">
              <i class="fas fa-paper-plane text-xs"></i>
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Main Grid Row 2 -->
    <div class="grid grid-cols-1 lg:grid-cols-3 gap-6 mb-8">
      <!-- Events -->
      <div class="card-animated rounded-2xl p-6 stagger-3">
        <div class="flex items-center justify-between mb-5">
          <h2 class="text-base font-semibold text-gray-900 flex items-center gap-3">
            <div class="icon-soft w-9 h-9 rounded-xl flex items-center justify-center">
              <i class="fas fa-calendar-alt text-sm"></i>
            </div>
            Upcoming Events
          </h2>
          <router-link to="/events" class="text-sm text-primary-600 font-medium hover:text-primary-700">View All</router-link>
        </div>
        <div class="space-y-3">
          <div v-for="event in upcomingEvents" :key="event.id"
               class="list-item-animated flex gap-3 p-3 rounded-xl cursor-pointer">
            <div class="w-12 h-12 rounded-xl bg-gradient-to-br from-primary-50 to-primary-100 flex flex-col items-center justify-center flex-shrink-0 transition-transform hover:scale-110">
              <span class="text-[10px] font-semibold text-primary-600 uppercase">{{ event.month }}</span>
              <span class="text-lg font-bold text-gray-900 -mt-0.5">{{ event.day }}</span>
            </div>
            <div class="flex-1 min-w-0">
              <h4 class="font-medium text-gray-900 text-sm truncate">{{ event.title }}</h4>
              <p class="text-xs text-gray-500 mt-0.5">{{ event.time }}</p>
              <div class="flex -space-x-1.5 mt-2">
                <div v-for="(attendee, idx) in event.attendees.slice(0, 3)" :key="idx"
                     class="w-5 h-5 rounded-md border border-white transition-transform hover:scale-110 hover:z-10"
                     :style="{ backgroundColor: attendee.color }">
                </div>
                <div v-if="event.attendees.length > 3"
                     class="w-5 h-5 rounded-md bg-gray-100 flex items-center justify-center text-[9px] text-gray-500 font-medium border border-white">
                  +{{ event.attendees.length - 3 }}
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Polls -->
      <div class="card-animated rounded-2xl p-6 stagger-4">
        <div class="flex items-center justify-between mb-5">
          <h2 class="text-base font-semibold text-gray-900 flex items-center gap-3">
            <div class="icon-soft w-9 h-9 rounded-xl flex items-center justify-center">
              <i class="fas fa-chart-pie text-sm"></i>
            </div>
            Active Polls
          </h2>
          <router-link to="/polls" class="text-sm text-primary-600 font-medium hover:text-primary-700">View All</router-link>
        </div>
        <div class="space-y-4">
          <div v-for="poll in activePolls" :key="poll.id" class="p-4 rounded-xl bg-gradient-to-br from-gray-50 to-primary-50/30">
            <h4 class="font-medium text-gray-900 text-sm mb-3">{{ poll.question }}</h4>
            <div class="space-y-2">
              <div v-for="option in poll.options" :key="option.label">
                <div class="flex items-center justify-between text-xs mb-1">
                  <span class="text-gray-600">{{ option.label }}</span>
                  <span class="text-gray-900 font-medium">{{ option.votes }}%</span>
                </div>
                <div class="h-2 bg-gray-200 rounded-full overflow-hidden">
                  <div class="h-full progress-animated rounded-full" :style="{ width: option.votes + '%' }"></div>
                </div>
              </div>
            </div>
            <div class="flex items-center justify-between mt-3 pt-3 border-t border-gray-200/50">
              <span class="text-xs text-gray-500">{{ poll.totalVotes }} votes</span>
              <span class="text-xs text-primary-600 font-medium">{{ poll.endsIn }}</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Learning Progress -->
      <div class="card-animated rounded-2xl p-6 stagger-5">
        <div class="flex items-center justify-between mb-5">
          <h2 class="text-base font-semibold text-gray-900 flex items-center gap-3">
            <div class="icon-soft w-9 h-9 rounded-xl flex items-center justify-center">
              <i class="fas fa-graduation-cap text-sm"></i>
            </div>
            My Learning
          </h2>
          <router-link to="/learning" class="text-sm text-primary-600 font-medium hover:text-primary-700">Browse</router-link>
        </div>
        <div class="space-y-3">
          <div v-for="course in learningCourses" :key="course.id"
               class="list-item-animated p-4 rounded-xl bg-gray-50 hover:bg-primary-50/50 cursor-pointer">
            <div class="flex items-start gap-3">
              <div :class="['w-10 h-10 rounded-xl flex items-center justify-center transition-transform hover:scale-110', course.iconBg]">
                <i :class="[course.icon, course.iconColor]"></i>
              </div>
              <div class="flex-1 min-w-0">
                <h4 class="font-medium text-gray-900 text-sm truncate">{{ course.title }}</h4>
                <p class="text-xs text-gray-500 mt-0.5">{{ course.instructor }}</p>
              </div>
            </div>
            <div class="mt-3">
              <div class="flex items-center justify-between text-xs mb-1">
                <span class="text-gray-500">Progress</span>
                <span class="text-primary-600 font-semibold">{{ course.progress }}%</span>
              </div>
              <div class="h-2 bg-gray-200 rounded-full overflow-hidden">
                <div class="h-full progress-animated rounded-full" :style="{ width: course.progress + '%' }"></div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Main Grid Row 3 -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-8">
      <!-- Media Center -->
      <div class="card-animated rounded-2xl p-6 stagger-4">
        <div class="flex items-center justify-between mb-5">
          <h2 class="text-base font-semibold text-gray-900 flex items-center gap-3">
            <div class="icon-soft w-9 h-9 rounded-xl flex items-center justify-center">
              <i class="fas fa-play-circle text-sm"></i>
            </div>
            Media Center
          </h2>
          <router-link to="/media" class="text-sm text-primary-600 font-medium hover:text-primary-700">View All</router-link>
        </div>
        <div class="grid grid-cols-2 gap-4">
          <div v-for="media in mediaItems" :key="media.id" class="media-card cursor-pointer">
            <div class="relative rounded-xl overflow-hidden aspect-video mb-2 shadow-lg">
              <div class="absolute inset-0" :class="media.gradientClass"></div>
              <div class="absolute inset-0 flex items-center justify-center">
                <div class="w-10 h-10 rounded-full bg-white/20 backdrop-blur-sm flex items-center justify-center transition-transform hover:scale-110">
                  <i :class="media.type === 'audio' ? 'fas fa-headphones' : 'fas fa-play'" class="text-white text-sm"></i>
                </div>
              </div>
              <div class="absolute bottom-2 right-2 px-2 py-0.5 rounded bg-black/60 text-white text-xs">
                {{ media.duration }}
              </div>
            </div>
            <h4 class="font-medium text-gray-900 text-sm truncate">{{ media.title }}</h4>
            <p class="text-xs text-gray-500 mt-0.5">{{ media.views }} views</p>
          </div>
        </div>
      </div>

      <!-- Recent Documents -->
      <div class="card-animated rounded-2xl p-6 stagger-5">
        <div class="flex items-center justify-between mb-5">
          <h2 class="text-base font-semibold text-gray-900 flex items-center gap-3">
            <div class="icon-soft w-9 h-9 rounded-xl flex items-center justify-center">
              <i class="fas fa-folder-open text-sm"></i>
            </div>
            Recent Documents
          </h2>
          <router-link to="/documents" class="text-sm text-primary-600 font-medium hover:text-primary-700">View All</router-link>
        </div>
        <div class="space-y-2">
          <div v-for="doc in recentDocuments" :key="doc.id"
               class="list-item-animated flex items-center gap-3 p-3 rounded-xl cursor-pointer">
            <div :class="['w-10 h-10 rounded-xl flex items-center justify-center transition-transform hover:scale-110', doc.iconBg]">
              <i :class="[doc.icon, doc.iconColor]"></i>
            </div>
            <div class="flex-1 min-w-0">
              <h4 class="font-medium text-gray-900 text-sm truncate">{{ doc.name }}</h4>
              <p class="text-xs text-gray-500">{{ doc.size }} • {{ doc.modified }}</p>
            </div>
            <button class="w-8 h-8 rounded-lg bg-gray-100 hover:bg-primary-100 flex items-center justify-center text-gray-400 hover:text-primary-600 transition-all">
              <i class="fas fa-download text-xs"></i>
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Main Grid Row 4 -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <!-- Team Activity -->
      <div class="card-animated rounded-2xl p-6 stagger-5">
        <div class="flex items-center justify-between mb-5">
          <h2 class="text-base font-semibold text-gray-900 flex items-center gap-3">
            <div class="icon-soft w-9 h-9 rounded-xl flex items-center justify-center">
              <i class="fas fa-clock-rotate-left text-sm"></i>
            </div>
            Team Activity
          </h2>
          <button class="text-sm text-primary-600 font-medium hover:text-primary-700">View All</button>
        </div>
        <div class="space-y-3">
          <div v-for="activity in teamActivities" :key="activity.id"
               class="flex items-start gap-3 p-3 rounded-xl hover:bg-gray-50 transition-all">
            <div class="w-9 h-9 rounded-lg flex items-center justify-center text-white text-xs font-semibold flex-shrink-0"
                 :style="{ backgroundColor: activity.user.color }">
              {{ activity.user.initials }}
            </div>
            <div class="flex-1 min-w-0">
              <p class="text-sm text-gray-900">
                <span class="font-medium">{{ activity.user.name }}</span>
                <span class="text-gray-500"> {{ activity.action }} </span>
                <span class="font-medium text-primary-600">{{ activity.target }}</span>
              </p>
              <p class="text-xs text-gray-400 mt-1">{{ activity.time }}</p>
            </div>
            <div :class="['w-7 h-7 rounded-lg flex items-center justify-center', activity.actionBg]">
              <i :class="[activity.actionIcon, 'text-xs', activity.actionColor]"></i>
            </div>
          </div>
        </div>
      </div>

      <!-- Quick Self Services -->
      <div class="card-animated rounded-2xl p-6 stagger-6">
        <div class="flex items-center justify-between mb-5">
          <h2 class="text-base font-semibold text-gray-900 flex items-center gap-3">
            <div class="icon-soft w-9 h-9 rounded-xl flex items-center justify-center">
              <i class="fas fa-grid-2 text-sm"></i>
            </div>
            Quick Services
          </h2>
          <router-link to="/self-services" class="text-sm text-primary-600 font-medium hover:text-primary-700">View All</router-link>
        </div>
        <div class="grid grid-cols-3 gap-3">
          <button v-for="service in selfServices" :key="service.id"
                  class="flex flex-col items-center gap-2 p-4 rounded-xl bg-gray-50 hover:bg-primary-50 transition-all group">
            <div :class="['w-10 h-10 rounded-xl flex items-center justify-center transition-transform group-hover:scale-110', service.iconBg]">
              <i :class="[service.icon, service.iconColor]"></i>
            </div>
            <span class="text-xs font-medium text-gray-700 group-hover:text-gray-900 text-center">{{ service.label }}</span>
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
/* Carousel Styles */
.updates-carousel {
  position: relative;
  overflow: hidden;
}

.carousel-track {
  display: flex;
  transition: transform 0.5s ease-in-out;
}

.carousel-slide {
  min-width: 100%;
  flex-shrink: 0;
}

.carousel-nav-btn {
  position: absolute;
  top: 50%;
  transform: translateY(-50%);
  width: 40px;
  height: 40px;
  border-radius: 12px;
  background: white;
  border: 1px solid #e5e7eb;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  z-index: 10;
  transition: all 0.2s;
  color: #6b7280;
}

.carousel-nav-btn:hover {
  background: #f0fdfa;
  border-color: #14b8a6;
  color: #14b8a6;
}

.carousel-nav-btn.prev {
  left: 0;
}

.carousel-nav-btn.next {
  right: 0;
}

.carousel-dots {
  display: flex;
  justify-content: center;
  gap: 8px;
  margin-top: 16px;
}

.carousel-dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  background: #e5e7eb;
  border: none;
  cursor: pointer;
  transition: all 0.2s;
}

.carousel-dot.active {
  background: #14b8a6;
  width: 24px;
  border-radius: 4px;
}

.update-badge {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 6px 12px;
  border-radius: 8px;
  font-size: 12px;
  font-weight: 500;
  backdrop-filter: blur(8px);
}

.update-badge.announcement {
  background: rgba(245, 158, 11, 0.9);
  color: white;
}

.update-badge.article {
  background: rgba(59, 130, 246, 0.9);
  color: white;
}

.update-badge.event {
  background: rgba(16, 185, 129, 0.9);
  color: white;
}

.update-badge.course {
  background: rgba(139, 92, 246, 0.9);
  color: white;
}

.update-badge.policy {
  background: rgba(239, 68, 68, 0.9);
  color: white;
}

.slide-progress {
  height: 100%;
  background: linear-gradient(90deg, #14b8a6, #0d9488);
  animation: slideProgress 5s linear;
}

@keyframes slideProgress {
  from { width: 0%; }
  to { width: 100%; }
}

/* Progress bar animation */
.progress-animated {
  background: linear-gradient(90deg, #14b8a6, #0d9488);
  transition: width 1s ease-out;
}

/* Icon soft style */
.icon-soft {
  background: linear-gradient(135deg, #f0fdfa, #ccfbf1);
  color: #0d9488;
}

/* Media card hover */
.media-card:hover .rounded-xl {
  transform: scale(1.02);
  transition: transform 0.3s ease;
}
</style>
