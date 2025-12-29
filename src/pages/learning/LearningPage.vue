<script setup lang="ts">
import { ref, computed } from 'vue'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'

// Loading state
const isLoading = ref(false)

// Tab state
const activeTab = ref('my-courses')
const searchQuery = ref('')
const categoryFilter = ref('')
const levelFilter = ref('')
const courseFilter = ref('all')

// Stats
const overallProgress = ref(68)
const completedCourses = ref(8)
const totalCourses = ref(12)
const learningHours = ref(45)
const certificates = ref(5)
const streak = ref(12)

// Tabs
const tabs = ref([
  { id: 'my-courses', label: 'My Courses', icon: 'fas fa-book-reader', count: 5 },
  { id: 'catalog', label: 'Browse Catalog', icon: 'fas fa-compass', count: null },
  { id: 'paths', label: 'Learning Paths', icon: 'fas fa-route', count: null },
  { id: 'certificates', label: 'Certificates', icon: 'fas fa-certificate', count: 5 },
])

// Current course in progress
const currentCourse = ref({
  id: 1,
  title: 'Advanced Data Analytics',
  nextLesson: 'Lesson 8: Predictive Modeling',
  progress: 75,
  icon: 'fas fa-chart-bar',
  iconBg: 'bg-teal-100',
  iconColor: 'text-teal-600'
})

// Enrolled courses
const enrolledCourses = ref([
  {
    id: 1,
    title: 'Advanced Data Analytics',
    instructor: 'Dr. James Wilson',
    progress: 75,
    completedLessons: 9,
    totalLessons: 12,
    level: 'Advanced',
    levelClass: 'badge-purple',
    status: 'In Progress',
    statusClass: 'badge-blue',
    icon: 'fas fa-chart-bar',
    gradientClass: 'from-teal-500 to-teal-700'
  },
  {
    id: 2,
    title: 'Leadership Essentials',
    instructor: 'Maria Garcia',
    progress: 45,
    completedLessons: 5,
    totalLessons: 11,
    level: 'Intermediate',
    levelClass: 'badge-blue',
    status: 'In Progress',
    statusClass: 'badge-blue',
    icon: 'fas fa-crown',
    gradientClass: 'from-teal-400 to-teal-600'
  },
  {
    id: 3,
    title: 'Cybersecurity Fundamentals',
    instructor: 'Alex Thompson',
    progress: 100,
    completedLessons: 8,
    totalLessons: 8,
    level: 'Beginner',
    levelClass: 'badge-green',
    status: 'Completed',
    statusClass: 'badge-green',
    icon: 'fas fa-shield-alt',
    gradientClass: 'from-teal-600 to-teal-800'
  },
  {
    id: 4,
    title: 'Effective Communication',
    instructor: 'Sarah Chen',
    progress: 0,
    completedLessons: 0,
    totalLessons: 10,
    level: 'Beginner',
    levelClass: 'badge-green',
    status: 'Not Started',
    statusClass: 'badge-teal',
    icon: 'fas fa-comments',
    gradientClass: 'from-teal-500 to-emerald-600'
  },
])

// Categories
const categories = ref([
  { id: 'tech', name: 'Technology', icon: 'fas fa-laptop-code', iconBg: 'bg-teal-100', iconColor: 'text-teal-600', courseCount: 24 },
  { id: 'leadership', name: 'Leadership', icon: 'fas fa-crown', iconBg: 'bg-teal-50', iconColor: 'text-teal-600', courseCount: 18 },
  { id: 'compliance', name: 'Compliance', icon: 'fas fa-clipboard-check', iconBg: 'bg-teal-100', iconColor: 'text-teal-700', courseCount: 12 },
  { id: 'soft-skills', name: 'Soft Skills', icon: 'fas fa-brain', iconBg: 'bg-teal-50', iconColor: 'text-teal-700', courseCount: 15 },
])

// Recommended courses
const recommendedCourses = ref([
  { id: 10, title: 'Machine Learning Basics', duration: '4 hours', level: 'Intermediate', matchScore: 95, icon: 'fas fa-robot', iconBg: 'bg-teal-100', iconColor: 'text-teal-600' },
  { id: 11, title: 'Strategic Planning', duration: '3 hours', level: 'Advanced', matchScore: 88, icon: 'fas fa-chess', iconBg: 'bg-teal-50', iconColor: 'text-teal-700' },
  { id: 12, title: 'Public Speaking', duration: '2 hours', level: 'Beginner', matchScore: 82, icon: 'fas fa-microphone', iconBg: 'bg-teal-100', iconColor: 'text-teal-600' },
])

// Catalog courses
const catalogCourses = ref([
  { id: 10, title: 'Machine Learning Basics', instructor: 'Dr. Sarah Mitchell', level: 'Intermediate', levelClass: 'badge-blue', duration: '4 hours', lessons: 12, icon: 'fas fa-robot', gradientClass: 'from-teal-500 to-teal-700' },
  { id: 11, title: 'Strategic Planning', instructor: 'Michael Brown', level: 'Advanced', levelClass: 'badge-purple', duration: '3 hours', lessons: 8, icon: 'fas fa-chess', gradientClass: 'from-teal-400 to-cyan-600' },
  { id: 12, title: 'Public Speaking', instructor: 'Emma Wilson', level: 'Beginner', levelClass: 'badge-green', duration: '2 hours', lessons: 6, icon: 'fas fa-microphone', gradientClass: 'from-teal-500 to-emerald-500' },
  { id: 13, title: 'Project Management Pro', instructor: 'David Lee', level: 'Intermediate', levelClass: 'badge-blue', duration: '5 hours', lessons: 15, icon: 'fas fa-tasks', gradientClass: 'from-teal-500 to-green-600' },
])

// Learning paths
const learningPaths = ref([
  {
    id: 1,
    title: 'Data Science Professional',
    description: 'Master data analysis, visualization, and machine learning fundamentals',
    level: 'Advanced',
    levelClass: 'badge-primary',
    courses: 5,
    duration: '40 hours',
    enrolled: 1234,
    progress: 60,
    icon: 'fas fa-chart-line',
    iconBg: 'bg-teal-100',
    iconColor: 'text-teal-600'
  },
  {
    id: 2,
    title: 'Leadership Excellence',
    description: 'Develop essential leadership skills for managing teams effectively',
    level: 'Intermediate',
    levelClass: 'badge-primary',
    courses: 4,
    duration: '24 hours',
    enrolled: 2456,
    progress: undefined,
    icon: 'fas fa-users-cog',
    iconBg: 'bg-teal-50',
    iconColor: 'text-teal-600'
  },
  {
    id: 3,
    title: 'Full Stack Developer',
    description: 'Learn front-end and back-end development from scratch',
    level: 'Beginner',
    levelClass: 'badge-primary',
    courses: 8,
    duration: '80 hours',
    enrolled: 3789,
    progress: undefined,
    icon: 'fas fa-code',
    iconBg: 'bg-teal-100',
    iconColor: 'text-teal-700'
  },
])

// Earned certificates
const earnedCertificates = ref([
  { id: 1, title: 'Cybersecurity Fundamentals', course: 'Completed Dec 15, 2024', date: 'Dec 15, 2024' },
  { id: 2, title: 'Project Management Basics', course: 'Completed Nov 28, 2024', date: 'Nov 28, 2024' },
  { id: 3, title: 'Effective Communication', course: 'Completed Nov 10, 2024', date: 'Nov 10, 2024' },
  { id: 4, title: 'Data Privacy Compliance', course: 'Completed Oct 22, 2024', date: 'Oct 22, 2024' },
  { id: 5, title: 'Time Management Mastery', course: 'Completed Sep 15, 2024', date: 'Sep 15, 2024' },
])

// Computed
const filteredCatalog = computed(() => {
  let result = [...catalogCourses.value]
  if (searchQuery.value) {
    const q = searchQuery.value.toLowerCase()
    result = result.filter(c => c.title.toLowerCase().includes(q))
  }
  if (levelFilter.value) {
    result = result.filter(c => c.level.toLowerCase() === levelFilter.value)
  }
  return result
})

// Navigate to course
function navigateToCourse(courseId: number) {
  // In a real app, this would use vue-router
  console.log('Navigate to course:', courseId)
}
</script>

<template>
  <div class="space-y-6">
    <!-- Loading State -->
    <div v-if="isLoading" class="flex items-center justify-center py-20">
      <LoadingSpinner size="lg" text="Loading learning content..." />
    </div>

    <template v-else>
      <!-- Page Header with Stats -->
      <div class="grid grid-cols-1 lg:grid-cols-4 gap-6 mb-10">
        <!-- Welcome Card -->
        <div class="lg:col-span-2 card-animated stagger-1 rounded-3xl p-8 relative overflow-hidden">
          <div class="absolute top-0 right-0 w-48 h-48 bg-gradient-to-br from-teal-400/20 to-transparent rounded-full -translate-y-1/2 translate-x-1/4"></div>
          <div class="relative">
            <h1 class="text-2xl font-bold text-gray-900 mb-2">Continue Learning</h1>
            <p class="text-gray-600 mb-4">You're making great progress! Keep it up.</p>

            <div v-if="currentCourse" class="flex items-center gap-4 p-4 bg-white/50 rounded-xl mb-4">
              <div :class="['w-16 h-16 rounded-xl flex items-center justify-center icon-soft', currentCourse.iconBg]">
                <i :class="[currentCourse.icon, 'text-2xl', currentCourse.iconColor]"></i>
              </div>
              <div class="flex-1">
                <h3 class="font-semibold text-gray-900">{{ currentCourse.title }}</h3>
                <p class="text-sm text-gray-500">{{ currentCourse.nextLesson }}</p>
                <div class="flex items-center gap-2 mt-1">
                  <div class="progress flex-1 h-1.5">
                    <div class="progress-bar progress-animated" :style="{ width: currentCourse.progress + '%' }"></div>
                  </div>
                  <span class="text-xs text-gray-600 font-medium">{{ currentCourse.progress }}%</span>
                </div>
              </div>
            </div>

            <button @click="navigateToCourse(currentCourse?.id)" class="btn btn-vibrant ripple">
              <i class="fas fa-play"></i>
              Resume Course
            </button>
          </div>
        </div>

        <!-- Progress Ring -->
        <div class="card-animated stagger-2 rounded-2xl p-6 flex flex-col items-center justify-center">
          <div class="relative mb-4">
            <svg class="progress-ring w-32 h-32">
              <circle class="text-gray-200" stroke="currentColor" stroke-width="8" fill="transparent" r="45" cx="64" cy="64"/>
              <circle class="progress-ring__circle text-teal-500" stroke="currentColor" stroke-width="8" fill="transparent" r="45" cx="64" cy="64"
                      :style="{ strokeDashoffset: 283 - (283 * overallProgress / 100) }"/>
            </svg>
            <div class="absolute inset-0 flex items-center justify-center">
              <span class="text-3xl font-bold text-gray-900">{{ overallProgress }}%</span>
            </div>
          </div>
          <p class="text-gray-600 text-center">Overall Progress</p>
          <p class="text-xs text-gray-400">{{ completedCourses }} of {{ totalCourses }} courses completed</p>
        </div>

        <!-- Quick Stats -->
        <div class="card-animated stagger-3 rounded-2xl p-6 space-y-4">
          <div class="flex items-center gap-3 list-item-animated rounded-lg p-2">
            <div class="w-10 h-10 rounded-xl icon-soft flex items-center justify-center">
              <i class="fas fa-clock text-teal-600"></i>
            </div>
            <div>
              <p class="text-2xl font-bold text-gray-900">{{ learningHours }}h</p>
              <p class="text-xs text-gray-500">Learning Time</p>
            </div>
          </div>
          <div class="flex items-center gap-3 list-item-animated rounded-lg p-2">
            <div class="w-10 h-10 rounded-xl icon-soft flex items-center justify-center">
              <i class="fas fa-certificate text-teal-600"></i>
            </div>
            <div>
              <p class="text-2xl font-bold text-gray-900">{{ certificates }}</p>
              <p class="text-xs text-gray-500">Certificates Earned</p>
            </div>
          </div>
          <div class="flex items-center gap-3 list-item-animated rounded-lg p-2">
            <div class="w-10 h-10 rounded-xl icon-soft flex items-center justify-center">
              <i class="fas fa-fire text-teal-600"></i>
            </div>
            <div>
              <p class="text-2xl font-bold text-gray-900">{{ streak }} days</p>
              <p class="text-xs text-gray-500">Learning Streak</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Tab Navigation -->
      <div class="flex items-center gap-4 mb-10 overflow-x-auto scrollbar-elegant pb-2 fade-in-up">
        <button v-for="tab in tabs" :key="tab.id"
                @click="activeTab = tab.id"
                :class="['px-5 py-2.5 rounded-xl font-medium transition-all whitespace-nowrap ripple',
                         activeTab === tab.id
                            ? 'btn-vibrant text-white shadow-lg shadow-teal-500/30'
                            : 'bg-white/60 text-gray-700 hover:bg-white']">
          <i :class="[tab.icon, 'mr-2']"></i>
          {{ tab.label }}
          <span v-if="tab.count" class="ml-1 px-1.5 py-0.5 text-xs rounded-full"
                :class="activeTab === tab.id ? 'bg-white/20' : 'bg-teal-100'">
            {{ tab.count }}
          </span>
        </button>
      </div>

      <!-- My Courses Tab -->
      <div v-if="activeTab === 'my-courses'" class="space-y-6">
        <div class="card-animated slide-in-left rounded-2xl p-6">
          <div class="flex items-center justify-between mb-6">
            <h2 class="text-xl font-bold text-gray-900">My Enrolled Courses</h2>
            <div class="flex items-center gap-3">
              <select v-model="courseFilter" class="input select w-40">
                <option value="all">All</option>
                <option value="in-progress">In Progress</option>
                <option value="completed">Completed</option>
                <option value="not-started">Not Started</option>
              </select>
            </div>
          </div>

          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            <div v-for="course in enrolledCourses" :key="course.id"
                 class="card-animated rounded-2xl overflow-hidden">
              <div class="h-32 bg-gradient-to-br relative" :class="course.gradientClass">
                <div class="absolute inset-0 flex items-center justify-center">
                  <i :class="[course.icon, 'text-white text-4xl opacity-50']"></i>
                </div>
                <div class="absolute top-3 right-3">
                  <span :class="['badge', course.statusClass]">{{ course.status }}</span>
                </div>
              </div>
              <div class="p-5">
                <span :class="['badge mb-2', course.levelClass]">{{ course.level }}</span>
                <h3 class="font-semibold text-gray-900 mb-1">{{ course.title }}</h3>
                <p class="text-xs text-gray-500 mb-3">{{ course.instructor }}</p>

                <div class="mb-4">
                  <div class="flex justify-between text-xs mb-1">
                    <span class="text-gray-600">Progress</span>
                    <span class="text-gray-700 font-semibold">{{ course.progress }}%</span>
                  </div>
                  <div class="progress">
                    <div class="progress-bar progress-animated" :style="{ width: course.progress + '%' }"></div>
                  </div>
                </div>

                <div class="flex items-center justify-between">
                  <span class="text-xs text-gray-500">
                    <i class="fas fa-play-circle mr-1"></i>{{ course.completedLessons }}/{{ course.totalLessons }} lessons
                  </span>
                  <button @click="navigateToCourse(course.id)" class="btn btn-vibrant ripple btn-sm">
                    {{ course.progress > 0 ? 'Continue' : 'Start' }}
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Browse Catalog Tab -->
      <div v-if="activeTab === 'catalog'" class="space-y-6">
        <!-- Search & Filters -->
        <div class="card-animated slide-in-right rounded-2xl p-6">
          <div class="flex flex-wrap gap-4">
            <div class="flex-1 min-w-[250px]">
              <div class="relative">
                <i class="fas fa-search absolute left-4 top-1/2 -translate-y-1/2 text-gray-400"></i>
                <input type="text" v-model="searchQuery" placeholder="Search courses..." class="input pl-10">
              </div>
            </div>
            <select v-model="categoryFilter" class="input select w-48">
              <option value="">All Categories</option>
              <option v-for="cat in categories" :key="cat.id" :value="cat.id">{{ cat.name }}</option>
            </select>
            <select v-model="levelFilter" class="input select w-40">
              <option value="">All Levels</option>
              <option value="beginner">Beginner</option>
              <option value="intermediate">Intermediate</option>
              <option value="advanced">Advanced</option>
            </select>
          </div>
        </div>

        <!-- AI Recommendations -->
        <div class="card-animated stagger-1 rounded-2xl p-6 border-l-4 border-teal-500">
          <div class="flex items-center gap-3 mb-4">
            <div class="w-10 h-10 rounded-xl icon-vibrant flex items-center justify-center ai-glow">
              <i class="fas fa-robot text-white"></i>
            </div>
            <div>
              <h3 class="font-semibold text-gray-900">Recommended for You</h3>
              <p class="text-xs text-gray-500">Based on your interests and learning history</p>
            </div>
          </div>
          <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div v-for="course in recommendedCourses" :key="course.id"
                 class="flex items-center gap-3 p-3 list-item-animated bg-teal-50/50 rounded-xl cursor-pointer">
              <div :class="['w-12 h-12 rounded-xl icon-soft flex items-center justify-center flex-shrink-0', course.iconBg]">
                <i :class="[course.icon, 'text-lg text-teal-600']"></i>
              </div>
              <div class="flex-1 min-w-0">
                <h4 class="font-medium text-gray-900 text-sm truncate">{{ course.title }}</h4>
                <p class="text-xs text-gray-500">{{ course.duration }} - {{ course.level }}</p>
              </div>
              <span class="badge badge-green text-xs">{{ course.matchScore }}% match</span>
            </div>
          </div>
        </div>

        <!-- Course Categories -->
        <div class="grid grid-cols-2 md:grid-cols-4 gap-4 mb-10">
          <div v-for="cat in categories" :key="cat.id"
               @click="categoryFilter = cat.id"
               :class="['card-animated ripple rounded-2xl p-5 cursor-pointer transition-all',
                        categoryFilter === cat.id ? 'ring-2 ring-teal-500' : 'hover:shadow-lg']">
            <div :class="['w-12 h-12 rounded-xl icon-soft flex items-center justify-center mb-3', cat.iconBg]">
              <i :class="[cat.icon, 'text-xl text-teal-600']"></i>
            </div>
            <h3 class="font-semibold text-gray-900 mb-1">{{ cat.name }}</h3>
            <p class="text-xs text-gray-500">{{ cat.courseCount }} courses</p>
          </div>
        </div>

        <!-- All Courses Grid -->
        <div class="card-animated stagger-2 rounded-2xl p-6">
          <h2 class="text-xl font-bold text-gray-900 mb-6">All Courses</h2>
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
            <div v-for="course in filteredCatalog" :key="course.id"
                 class="card-animated rounded-2xl overflow-hidden cursor-pointer">
              <div class="h-32 bg-gradient-to-br relative" :class="course.gradientClass">
                <div class="absolute inset-0 flex items-center justify-center">
                  <i :class="[course.icon, 'text-white text-4xl opacity-50']"></i>
                </div>
              </div>
              <div class="p-5">
                <span :class="['badge mb-2', course.levelClass]">{{ course.level }}</span>
                <h3 class="font-semibold text-gray-900 mb-1">{{ course.title }}</h3>
                <p class="text-xs text-gray-500 mb-3">{{ course.instructor }}</p>
                <div class="flex items-center justify-between text-xs text-gray-500">
                  <span><i class="fas fa-clock mr-1"></i>{{ course.duration }}</span>
                  <span><i class="fas fa-play-circle mr-1"></i>{{ course.lessons }} lessons</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Learning Paths Tab -->
      <div v-if="activeTab === 'paths'" class="space-y-6">
        <div class="card-animated slide-in-left rounded-2xl p-6">
          <h2 class="text-xl font-bold text-gray-900 mb-6">Learning Paths</h2>
          <p class="text-gray-600 mb-6">Structured learning journeys to master specific skills</p>

          <div class="space-y-4">
            <div v-for="path in learningPaths" :key="path.id"
                 class="card-animated list-item-animated rounded-2xl p-6">
              <div class="flex flex-col md:flex-row md:items-center gap-4">
                <div :class="['w-16 h-16 rounded-2xl icon-soft flex items-center justify-center flex-shrink-0', path.iconBg]">
                  <i :class="[path.icon, 'text-2xl text-teal-600']"></i>
                </div>
                <div class="flex-1">
                  <div class="flex items-center gap-2 mb-1">
                    <h3 class="font-semibold text-gray-900 text-lg">{{ path.title }}</h3>
                    <span :class="['badge', path.levelClass]">{{ path.level }}</span>
                  </div>
                  <p class="text-sm text-gray-600 mb-2">{{ path.description }}</p>
                  <div class="flex flex-wrap items-center gap-4 text-xs text-gray-500">
                    <span><i class="fas fa-book mr-1"></i>{{ path.courses }} courses</span>
                    <span><i class="fas fa-clock mr-1"></i>{{ path.duration }}</span>
                    <span><i class="fas fa-users mr-1"></i>{{ path.enrolled }} enrolled</span>
                  </div>
                </div>
                <div class="flex flex-col items-end gap-2">
                  <div v-if="path.progress !== undefined" class="text-right">
                    <span class="text-sm font-semibold text-gray-700">{{ path.progress }}% complete</span>
                    <div class="progress w-32 mt-1">
                      <div class="progress-bar progress-animated" :style="{ width: path.progress + '%' }"></div>
                    </div>
                  </div>
                  <button :class="['btn ripple', path.progress !== undefined ? 'btn-vibrant' : 'btn-secondary']">
                    {{ path.progress !== undefined ? 'Continue' : 'Enroll' }}
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Certificates Tab -->
      <div v-if="activeTab === 'certificates'" class="space-y-6">
        <div class="card-animated slide-in-right rounded-2xl p-6">
          <h2 class="text-xl font-bold text-gray-900 mb-6">My Certificates</h2>

          <div v-if="earnedCertificates.length > 0" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            <div v-for="cert in earnedCertificates" :key="cert.id"
                 class="card-animated rounded-2xl overflow-hidden">
              <div class="h-40 bg-gradient-to-br from-yellow-400 to-orange-500 relative flex items-center justify-center">
                <i class="fas fa-certificate text-white text-6xl opacity-50"></i>
                <div class="absolute inset-0 flex items-center justify-center">
                  <div class="w-20 h-20 rounded-full bg-white/90 flex items-center justify-center">
                    <i class="fas fa-award text-yellow-500 text-3xl"></i>
                  </div>
                </div>
              </div>
              <div class="p-5">
                <h3 class="font-semibold text-gray-900 mb-1">{{ cert.title }}</h3>
                <p class="text-sm text-gray-600 mb-2">{{ cert.course }}</p>
                <p class="text-xs text-gray-500 mb-4">
                  <i class="fas fa-calendar mr-1"></i>Earned {{ cert.date }}
                </p>
                <div class="flex items-center gap-2">
                  <button class="btn btn-vibrant ripple btn-sm flex-1">
                    <i class="fas fa-eye"></i> View
                  </button>
                  <button class="btn btn-secondary ripple btn-sm flex-1">
                    <i class="fas fa-download"></i> Download
                  </button>
                  <button class="btn btn-secondary ripple btn-sm">
                    <i class="fas fa-share"></i>
                  </button>
                </div>
              </div>
            </div>
          </div>

          <!-- Empty State -->
          <div v-else class="text-center py-12">
            <div class="w-16 h-16 rounded-full bg-gray-100 flex items-center justify-center mx-auto mb-4">
              <i class="fas fa-certificate text-gray-400 text-2xl"></i>
            </div>
            <h3 class="font-semibold text-gray-900 mb-2">No certificates yet</h3>
            <p class="text-gray-600 mb-4">Complete courses to earn certificates and showcase your achievements</p>
            <button @click="activeTab = 'catalog'" class="btn btn-vibrant ripple">
              Browse Courses
            </button>
          </div>
        </div>
      </div>
    </template>
  </div>
</template>

<style scoped>
/* Progress ring styles */
.progress-ring {
  transform: rotate(-90deg);
}

.progress-ring__circle {
  stroke-dasharray: 283;
  stroke-dashoffset: 283;
  transition: stroke-dashoffset 0.5s ease;
}

/* Progress animated shimmer effect */
.progress-animated {
  background: linear-gradient(90deg, #14b8a6, #0d9488, #5eead4, #14b8a6);
  background-size: 300% 100%;
  animation: progressShimmer 2s linear infinite;
}

@keyframes progressShimmer {
  0% { background-position: 0% 50%; }
  100% { background-position: 300% 50%; }
}

/* AI glow effect */
.ai-glow {
  box-shadow: 0 0 20px rgba(20, 184, 166, 0.4);
}

/* Badge colors not in main styles */
.badge-purple {
  background: #f5f3ff;
  color: #8b5cf6;
  padding: 0.25rem 0.75rem;
  border-radius: 9999px;
  font-size: 0.75rem;
  font-weight: 600;
}

.badge-teal {
  background: #ccfbf1;
  color: #0d9488;
  padding: 0.25rem 0.75rem;
  border-radius: 9999px;
  font-size: 0.75rem;
  font-weight: 600;
}

/* Scrollbar styling */
.scrollbar-elegant::-webkit-scrollbar {
  height: 6px;
}

.scrollbar-elegant::-webkit-scrollbar-track {
  background: #f5f5f5;
  border-radius: 3px;
}

.scrollbar-elegant::-webkit-scrollbar-thumb {
  background: #d4d4d4;
  border-radius: 3px;
}

.scrollbar-elegant::-webkit-scrollbar-thumb:hover {
  background: #a3a3a3;
}

/* Select input styling */
.select {
  appearance: none;
  background-image: url("data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' fill='none' viewBox='0 0 20 20'%3e%3cpath stroke='%236b7280' stroke-linecap='round' stroke-linejoin='round' stroke-width='1.5' d='M6 8l4 4 4-4'/%3e%3c/svg%3e");
  background-position: right 0.5rem center;
  background-repeat: no-repeat;
  background-size: 1.5em 1.5em;
  padding-right: 2.5rem;
}
</style>
