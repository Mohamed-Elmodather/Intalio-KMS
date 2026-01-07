<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()

// Loading state
const isLoading = ref(false)

// Tab state
const activeTab = ref('my-courses')
const searchQuery = ref('')
const categoryFilter = ref('')
const levelFilter = ref('')
const courseFilter = ref('all')
const viewMode = ref<'grid' | 'list'>('grid')

// Stats
const overallProgress = ref(68)
const completedCourses = ref(8)
const totalCourses = ref(12)
const learningHours = ref(45)
const certificates = ref(5)
const streak = ref(12)
const totalEnrolled = ref(156)

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
    instructorInitials: 'JW',
    progress: 75,
    completedLessons: 9,
    totalLessons: 12,
    level: 'Advanced',
    levelClass: 'advanced',
    status: 'In Progress',
    statusClass: 'in-progress',
    icon: 'fas fa-chart-bar',
    gradientClass: 'from-teal-500 to-teal-700',
    duration: '8 hours',
    rating: 4.8,
    students: 1250,
    image: 'https://images.unsplash.com/photo-1551288049-bebda4e38f71?w=400&h=300&fit=crop',
    tags: ['Data Science', 'Analytics']
  },
  {
    id: 2,
    title: 'Leadership Essentials',
    instructor: 'Maria Garcia',
    instructorInitials: 'MG',
    progress: 45,
    completedLessons: 5,
    totalLessons: 11,
    level: 'Intermediate',
    levelClass: 'intermediate',
    status: 'In Progress',
    statusClass: 'in-progress',
    icon: 'fas fa-crown',
    gradientClass: 'from-amber-500 to-orange-600',
    duration: '6 hours',
    rating: 4.6,
    students: 2340,
    image: 'https://images.unsplash.com/photo-1519389950473-47ba0277781c?w=400&h=300&fit=crop',
    tags: ['Leadership', 'Management']
  },
  {
    id: 3,
    title: 'Cybersecurity Fundamentals',
    instructor: 'Alex Thompson',
    instructorInitials: 'AT',
    progress: 100,
    completedLessons: 8,
    totalLessons: 8,
    level: 'Beginner',
    levelClass: 'beginner',
    status: 'Completed',
    statusClass: 'completed',
    icon: 'fas fa-shield-alt',
    gradientClass: 'from-blue-500 to-indigo-600',
    duration: '4 hours',
    rating: 4.9,
    students: 3450,
    image: 'https://images.unsplash.com/photo-1550751827-4bd374c3f58b?w=400&h=300&fit=crop',
    tags: ['Security', 'IT']
  },
  {
    id: 4,
    title: 'Effective Communication',
    instructor: 'Sarah Chen',
    instructorInitials: 'SC',
    progress: 0,
    completedLessons: 0,
    totalLessons: 10,
    level: 'Beginner',
    levelClass: 'beginner',
    status: 'Not Started',
    statusClass: 'not-started',
    icon: 'fas fa-comments',
    gradientClass: 'from-purple-500 to-pink-600',
    duration: '5 hours',
    rating: 4.7,
    students: 1890,
    image: 'https://images.unsplash.com/photo-1552664730-d307ca884978?w=400&h=300&fit=crop',
    tags: ['Soft Skills', 'Communication']
  },
  {
    id: 5,
    title: 'Project Management Pro',
    instructor: 'David Lee',
    instructorInitials: 'DL',
    progress: 30,
    completedLessons: 4,
    totalLessons: 15,
    level: 'Intermediate',
    levelClass: 'intermediate',
    status: 'In Progress',
    statusClass: 'in-progress',
    icon: 'fas fa-tasks',
    gradientClass: 'from-emerald-500 to-teal-600',
    duration: '10 hours',
    rating: 4.5,
    students: 2780,
    image: 'https://images.unsplash.com/photo-1507925921958-8a62f3d1a50d?w=400&h=300&fit=crop',
    tags: ['Project Management', 'Agile']
  },
])

// Categories
const categories = ref([
  { id: 'tech', name: 'Technology', icon: 'fas fa-laptop-code', color: '#0d9488', courseCount: 24 },
  { id: 'leadership', name: 'Leadership', icon: 'fas fa-crown', color: '#f59e0b', courseCount: 18 },
  { id: 'compliance', name: 'Compliance', icon: 'fas fa-clipboard-check', color: '#6366f1', courseCount: 12 },
  { id: 'soft-skills', name: 'Soft Skills', icon: 'fas fa-brain', color: '#ec4899', courseCount: 15 },
  { id: 'data', name: 'Data Science', icon: 'fas fa-chart-line', color: '#3b82f6', courseCount: 20 },
  { id: 'security', name: 'Security', icon: 'fas fa-shield-alt', color: '#ef4444', courseCount: 8 },
])

// Recommended courses
const recommendedCourses = ref([
  { id: 10, title: 'Machine Learning Basics', duration: '4 hours', level: 'Intermediate', matchScore: 95, icon: 'fas fa-robot', instructor: 'Dr. Sarah Mitchell' },
  { id: 11, title: 'Strategic Planning', duration: '3 hours', level: 'Advanced', matchScore: 88, icon: 'fas fa-chess', instructor: 'Michael Brown' },
  { id: 12, title: 'Public Speaking', duration: '2 hours', level: 'Beginner', matchScore: 82, icon: 'fas fa-microphone', instructor: 'Emma Wilson' },
])

// Catalog courses
const catalogCourses = ref([
  { id: 10, title: 'Machine Learning Basics', instructor: 'Dr. Sarah Mitchell', instructorInitials: 'SM', level: 'Intermediate', levelClass: 'intermediate', duration: '4 hours', lessons: 12, icon: 'fas fa-robot', gradientClass: 'from-violet-500 to-purple-600', rating: 4.8, students: 3200, image: 'https://images.unsplash.com/photo-1677442136019-21780ecad995?w=400&h=300&fit=crop', tags: ['AI', 'ML'] },
  { id: 11, title: 'Strategic Planning', instructor: 'Michael Brown', instructorInitials: 'MB', level: 'Advanced', levelClass: 'advanced', duration: '3 hours', lessons: 8, icon: 'fas fa-chess', gradientClass: 'from-slate-600 to-gray-800', rating: 4.6, students: 1890, image: 'https://images.unsplash.com/photo-1454165804606-c3d57bc86b40?w=400&h=300&fit=crop', tags: ['Strategy', 'Planning'] },
  { id: 12, title: 'Public Speaking', instructor: 'Emma Wilson', instructorInitials: 'EW', level: 'Beginner', levelClass: 'beginner', duration: '2 hours', lessons: 6, icon: 'fas fa-microphone', gradientClass: 'from-rose-500 to-pink-600', rating: 4.7, students: 4500, image: 'https://images.unsplash.com/photo-1475721027785-f74eccf877e2?w=400&h=300&fit=crop', tags: ['Speaking', 'Presentation'] },
  { id: 13, title: 'Cloud Architecture', instructor: 'David Kim', instructorInitials: 'DK', level: 'Advanced', levelClass: 'advanced', duration: '8 hours', lessons: 20, icon: 'fas fa-cloud', gradientClass: 'from-sky-500 to-blue-600', rating: 4.9, students: 2100, image: 'https://images.unsplash.com/photo-1451187580459-43490279c0fa?w=400&h=300&fit=crop', tags: ['Cloud', 'AWS'] },
  { id: 14, title: 'UX Design Principles', instructor: 'Lisa Park', instructorInitials: 'LP', level: 'Intermediate', levelClass: 'intermediate', duration: '5 hours', lessons: 14, icon: 'fas fa-pencil-ruler', gradientClass: 'from-fuchsia-500 to-purple-600', rating: 4.8, students: 2890, image: 'https://images.unsplash.com/photo-1561070791-2526d30994b5?w=400&h=300&fit=crop', tags: ['Design', 'UX'] },
  { id: 15, title: 'Financial Analysis', instructor: 'Robert Chen', instructorInitials: 'RC', level: 'Intermediate', levelClass: 'intermediate', duration: '6 hours', lessons: 16, icon: 'fas fa-chart-pie', gradientClass: 'from-green-500 to-emerald-600', rating: 4.5, students: 1670, image: 'https://images.unsplash.com/photo-1554224155-6726b3ff858f?w=400&h=300&fit=crop', tags: ['Finance', 'Analysis'] },
])

// Learning paths
const learningPaths = ref([
  {
    id: 1,
    title: 'Data Science Professional',
    description: 'Master data analysis, visualization, and machine learning fundamentals',
    level: 'Advanced',
    levelClass: 'advanced',
    courses: 5,
    duration: '40 hours',
    enrolled: 1234,
    progress: 60,
    icon: 'fas fa-chart-line',
    color: '#0d9488',
    image: 'https://images.unsplash.com/photo-1551288049-bebda4e38f71?w=400&h=200&fit=crop'
  },
  {
    id: 2,
    title: 'Leadership Excellence',
    description: 'Develop essential leadership skills for managing teams effectively',
    level: 'Intermediate',
    levelClass: 'intermediate',
    courses: 4,
    duration: '24 hours',
    enrolled: 2456,
    progress: undefined,
    icon: 'fas fa-users-cog',
    color: '#f59e0b',
    image: 'https://images.unsplash.com/photo-1519389950473-47ba0277781c?w=400&h=200&fit=crop'
  },
  {
    id: 3,
    title: 'Full Stack Developer',
    description: 'Learn front-end and back-end development from scratch',
    level: 'Beginner',
    levelClass: 'beginner',
    courses: 8,
    duration: '80 hours',
    enrolled: 3789,
    progress: undefined,
    icon: 'fas fa-code',
    color: '#6366f1',
    image: 'https://images.unsplash.com/photo-1555066931-4365d14bab8c?w=400&h=200&fit=crop'
  },
  {
    id: 4,
    title: 'Cloud Solutions Architect',
    description: 'Design and implement scalable cloud infrastructure solutions',
    level: 'Advanced',
    levelClass: 'advanced',
    courses: 6,
    duration: '50 hours',
    enrolled: 1567,
    progress: 25,
    icon: 'fas fa-cloud',
    color: '#3b82f6',
    image: 'https://images.unsplash.com/photo-1451187580459-43490279c0fa?w=400&h=200&fit=crop'
  },
])

// Earned certificates
const earnedCertificates = ref([
  { id: 1, title: 'Cybersecurity Fundamentals', course: 'Cybersecurity Fundamentals', date: 'Dec 15, 2024', credentialId: 'AFC-CS-2024-001', icon: 'fas fa-shield-alt', color: '#3b82f6' },
  { id: 2, title: 'Project Management Basics', course: 'Project Management Pro', date: 'Nov 28, 2024', credentialId: 'AFC-PM-2024-002', icon: 'fas fa-tasks', color: '#10b981' },
  { id: 3, title: 'Effective Communication', course: 'Effective Communication', date: 'Nov 10, 2024', credentialId: 'AFC-EC-2024-003', icon: 'fas fa-comments', color: '#8b5cf6' },
  { id: 4, title: 'Data Privacy Compliance', course: 'Data Privacy & GDPR', date: 'Oct 22, 2024', credentialId: 'AFC-DP-2024-004', icon: 'fas fa-user-shield', color: '#ef4444' },
  { id: 5, title: 'Time Management Mastery', course: 'Time Management', date: 'Sep 15, 2024', credentialId: 'AFC-TM-2024-005', icon: 'fas fa-clock', color: '#f59e0b' },
])

// Computed
const filteredCatalog = computed(() => {
  let result = [...catalogCourses.value]
  if (searchQuery.value) {
    const q = searchQuery.value.toLowerCase()
    result = result.filter(c => c.title.toLowerCase().includes(q) || c.instructor.toLowerCase().includes(q))
  }
  if (levelFilter.value) {
    result = result.filter(c => c.level.toLowerCase() === levelFilter.value)
  }
  if (categoryFilter.value) {
    // In a real app, courses would have category IDs
    result = result.filter(c => c.tags.some(t => t.toLowerCase().includes(categoryFilter.value)))
  }
  return result
})

const filteredEnrolledCourses = computed(() => {
  if (courseFilter.value === 'all') return enrolledCourses.value
  if (courseFilter.value === 'in-progress') return enrolledCourses.value.filter(c => c.progress > 0 && c.progress < 100)
  if (courseFilter.value === 'completed') return enrolledCourses.value.filter(c => c.progress === 100)
  if (courseFilter.value === 'not-started') return enrolledCourses.value.filter(c => c.progress === 0)
  return enrolledCourses.value
})

// Navigate to course
function navigateToCourse(courseId: number) {
  router.push({ name: 'CourseView', params: { id: courseId } })
}

function getLevelColor(level: string) {
  switch (level.toLowerCase()) {
    case 'beginner': return '#10b981'
    case 'intermediate': return '#3b82f6'
    case 'advanced': return '#8b5cf6'
    default: return '#6b7280'
  }
}
</script>

<template>
  <div class="page-view">
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
            <i class="fas fa-graduation-cap"></i>
          </div>
          <p class="stat-value-mini">{{ completedCourses }}/{{ totalCourses }}</p>
          <p class="stat-label-mini">Completed</p>
        </div>
        <div class="stat-card-square">
          <div class="stat-icon-box">
            <i class="fas fa-clock"></i>
          </div>
          <p class="stat-value-mini">{{ learningHours }}h</p>
          <p class="stat-label-mini">Learning</p>
        </div>
        <div class="stat-card-square">
          <div class="stat-icon-box">
            <i class="fas fa-certificate"></i>
          </div>
          <p class="stat-value-mini">{{ certificates }}</p>
          <p class="stat-label-mini">Certificates</p>
        </div>
        <div class="stat-card-square">
          <div class="stat-icon-box">
            <i class="fas fa-fire"></i>
          </div>
          <p class="stat-value-mini">{{ streak }}</p>
          <p class="stat-label-mini">Day Streak</p>
        </div>
      </div>

      <div class="relative px-8 py-8">
        <div class="px-3 py-1 bg-white/20 backdrop-blur-sm rounded-full text-white text-xs font-semibold inline-flex items-center gap-2 mb-4">
          <i class="fas fa-graduation-cap"></i>
          AFC Asian Cup 2027
        </div>

        <h1 class="text-3xl font-bold text-white mb-2">Learning & Development</h1>
        <p class="text-teal-100 max-w-lg">Expand your skills with curated courses, learning paths, and professional certifications.</p>

        <div class="flex flex-wrap gap-3 mt-6">
          <button @click="activeTab = 'catalog'" class="px-5 py-2.5 bg-white text-teal-600 rounded-xl font-semibold text-sm flex items-center gap-2 hover:bg-teal-50 transition-all shadow-lg">
            <i class="fas fa-compass"></i>
            Browse Courses
          </button>
          <button v-if="currentCourse" @click="navigateToCourse(currentCourse.id)" class="px-5 py-2.5 bg-white/20 backdrop-blur-sm border border-white/30 text-white rounded-xl font-semibold text-sm hover:bg-white/30 transition-all flex items-center gap-2">
            <i class="fas fa-play"></i>
            Continue Learning
          </button>
        </div>
      </div>
    </div>

    <!-- Main Content -->
    <div class="px-8 py-6">
      <!-- Tab Navigation -->
      <div class="flex items-center gap-2 mb-8 overflow-x-auto pb-2 scrollbar-hide">
        <button v-for="tab in tabs" :key="tab.id"
                @click="activeTab = tab.id"
                :class="[
                  'px-4 py-2.5 rounded-xl font-medium text-sm transition-all whitespace-nowrap flex items-center gap-2',
                  activeTab === tab.id
                    ? 'bg-gradient-to-r from-teal-500 to-teal-600 text-white shadow-lg shadow-teal-500/30'
                    : 'bg-white text-gray-600 hover:bg-gray-50 border border-gray-200'
                ]">
          <i :class="tab.icon"></i>
          {{ tab.label }}
          <span v-if="tab.count"
                :class="[
                  'px-1.5 py-0.5 text-[10px] rounded-full font-semibold',
                  activeTab === tab.id ? 'bg-white/20 text-white' : 'bg-teal-100 text-teal-600'
                ]">
            {{ tab.count }}
          </span>
        </button>
      </div>

      <!-- My Courses Tab -->
      <div v-if="activeTab === 'my-courses'" class="space-y-6">
        <!-- Section Header -->
        <div class="flex items-center justify-between">
          <div class="flex items-center gap-3">
            <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-teal-500 to-teal-600 flex items-center justify-center shadow-lg shadow-teal-200">
              <i class="fas fa-book-reader text-white text-sm"></i>
            </div>
            <div>
              <h2 class="text-lg font-bold text-gray-900">My Enrolled Courses</h2>
              <p class="text-xs text-gray-500">{{ filteredEnrolledCourses.length }} courses</p>
            </div>
          </div>
          <div class="flex items-center gap-3">
            <select v-model="courseFilter" class="text-xs px-3 py-2 rounded-lg border border-gray-200 bg-white text-gray-700 focus:outline-none focus:ring-2 focus:ring-teal-500 cursor-pointer">
              <option value="all">All Courses</option>
              <option value="in-progress">In Progress</option>
              <option value="completed">Completed</option>
              <option value="not-started">Not Started</option>
            </select>
            <div class="flex items-center bg-white rounded-lg border border-gray-200 p-1">
              <button @click="viewMode = 'grid'" :class="['px-3 py-1.5 rounded-md text-xs font-medium transition-all', viewMode === 'grid' ? 'bg-teal-500 text-white' : 'text-gray-500 hover:bg-gray-100']">
                <i class="fas fa-th-large"></i>
              </button>
              <button @click="viewMode = 'list'" :class="['px-3 py-1.5 rounded-md text-xs font-medium transition-all', viewMode === 'list' ? 'bg-teal-500 text-white' : 'text-gray-500 hover:bg-gray-100']">
                <i class="fas fa-list"></i>
              </button>
            </div>
          </div>
        </div>

        <!-- Course Grid -->
        <div class="content-area p-6">
          <div v-if="viewMode === 'grid'" class="grid grid-cols-[repeat(auto-fill,minmax(320px,1fr))] gap-5">
            <div v-for="course in filteredEnrolledCourses" :key="course.id"
                 @click="navigateToCourse(course.id)"
                 class="course-card group bg-white rounded-2xl overflow-hidden cursor-pointer transition-all duration-300 hover:-translate-y-1.5 border border-gray-100 shadow-sm hover:shadow-lg hover:border-teal-200">
              <!-- Card Image -->
              <div class="relative h-40 overflow-hidden">
                <img :src="course.image" :alt="course.title" class="w-full h-full object-cover transition-transform duration-500 group-hover:scale-110">
                <div class="absolute inset-0 bg-gradient-to-t from-black/60 via-black/20 to-transparent"></div>

                <!-- Status Badge -->
                <div class="absolute top-3 left-3">
                  <span :class="['course-status-badge', course.statusClass]">
                    {{ course.status }}
                  </span>
                </div>

                <!-- Progress Overlay -->
                <div class="absolute bottom-0 left-0 right-0 h-1 bg-black/20">
                  <div class="h-full bg-teal-400 transition-all" :style="{ width: course.progress + '%' }"></div>
                </div>
              </div>

              <!-- Card Content -->
              <div class="p-4">
                <!-- Meta Info -->
                <div class="flex items-center gap-3 text-[11px] text-gray-400 mb-2">
                  <span class="flex items-center gap-1">
                    <i class="fas fa-clock text-[9px]"></i>
                    {{ course.duration }}
                  </span>
                  <span class="flex items-center gap-1">
                    <i class="fas fa-play-circle text-[9px]"></i>
                    {{ course.completedLessons }}/{{ course.totalLessons }} lessons
                  </span>
                </div>

                <!-- Title -->
                <h3 class="text-sm font-semibold text-gray-800 mb-2 line-clamp-2 group-hover:text-teal-600 transition-colors leading-snug">
                  {{ course.title }}
                </h3>

                <!-- Level Badge -->
                <div class="mb-3">
                  <span :class="['course-level-badge', course.levelClass]">
                    {{ course.level }}
                  </span>
                </div>

                <!-- Tags -->
                <div class="flex flex-wrap gap-1.5 mb-3">
                  <span v-for="tag in course.tags" :key="tag"
                        class="px-2 py-0.5 bg-gray-100 text-gray-600 text-[10px] font-medium rounded-full hover:bg-teal-50 hover:text-teal-600 transition-colors">
                    {{ tag }}
                  </span>
                </div>

                <!-- Footer -->
                <div class="flex items-center justify-between pt-3 border-t border-gray-100">
                  <!-- Instructor -->
                  <div class="flex items-center gap-2">
                    <div class="w-7 h-7 rounded-full bg-gradient-to-br from-teal-400 to-cyan-500 flex items-center justify-center text-white text-[10px] font-semibold shadow-sm">
                      {{ course.instructorInitials }}
                    </div>
                    <span class="text-xs text-gray-600 font-medium">{{ course.instructor }}</span>
                  </div>

                  <!-- Stats -->
                  <div class="flex items-center gap-3 text-[11px] text-gray-400">
                    <span class="flex items-center gap-1">
                      <i class="fas fa-star text-amber-400 text-[9px]"></i>
                      {{ course.rating }}
                    </span>
                    <span class="font-semibold text-teal-600">{{ course.progress }}%</span>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- List View -->
          <div v-else class="space-y-3">
            <div v-for="course in filteredEnrolledCourses" :key="course.id"
                 @click="navigateToCourse(course.id)"
                 class="group flex gap-4 p-4 bg-white rounded-2xl cursor-pointer transition-all duration-300 border border-gray-100 shadow-sm hover:shadow-md hover:border-teal-200">
              <!-- Thumbnail -->
              <div class="relative w-40 h-24 flex-shrink-0 rounded-xl overflow-hidden">
                <img :src="course.image" :alt="course.title" class="w-full h-full object-cover transition-transform duration-300 group-hover:scale-110">
                <div class="absolute bottom-0 left-0 right-0 h-1 bg-black/20">
                  <div class="h-full bg-teal-400" :style="{ width: course.progress + '%' }"></div>
                </div>
              </div>

              <!-- Content -->
              <div class="flex-1 min-w-0">
                <div class="flex flex-wrap items-center gap-2 mb-1.5">
                  <span :class="['course-level-badge', course.levelClass]">{{ course.level }}</span>
                  <span :class="['course-status-badge', course.statusClass]">{{ course.status }}</span>
                </div>
                <h3 class="text-sm font-semibold text-gray-800 mb-1 truncate group-hover:text-teal-600 transition-colors">
                  {{ course.title }}
                </h3>
                <p class="text-xs text-gray-500 mb-2">{{ course.instructor }}</p>
                <div class="flex items-center gap-4 text-[11px] text-gray-400">
                  <span><i class="fas fa-clock text-[9px] mr-1"></i>{{ course.duration }}</span>
                  <span><i class="fas fa-play-circle text-[9px] mr-1"></i>{{ course.completedLessons }}/{{ course.totalLessons }} lessons</span>
                  <span><i class="fas fa-star text-amber-400 text-[9px] mr-1"></i>{{ course.rating }}</span>
                </div>
              </div>

              <!-- Progress & Action -->
              <div class="flex flex-col items-end justify-between">
                <span class="text-lg font-bold text-teal-600">{{ course.progress }}%</span>
                <button class="px-4 py-2 bg-gradient-to-r from-teal-500 to-teal-600 text-white rounded-lg text-xs font-medium hover:from-teal-600 hover:to-teal-700 transition-all">
                  {{ course.progress > 0 && course.progress < 100 ? 'Continue' : course.progress === 100 ? 'Review' : 'Start' }}
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Browse Catalog Tab -->
      <div v-if="activeTab === 'catalog'" class="space-y-6">
        <!-- Search & Filters -->
        <div class="content-area p-4">
          <div class="flex flex-wrap gap-3">
            <div class="flex-1 min-w-[250px] relative">
              <i class="fas fa-search absolute left-3 top-1/2 -translate-y-1/2 text-gray-400 text-sm"></i>
              <input type="text" v-model="searchQuery" placeholder="Search courses..."
                     class="w-full pl-9 pr-4 py-2 bg-white border border-gray-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-teal-500 focus:border-transparent">
            </div>
            <select v-model="categoryFilter" class="text-xs px-3 py-2 rounded-lg border border-gray-200 bg-white text-gray-700 focus:outline-none focus:ring-2 focus:ring-teal-500 cursor-pointer">
              <option value="">All Categories</option>
              <option v-for="cat in categories" :key="cat.id" :value="cat.id">{{ cat.name }}</option>
            </select>
            <select v-model="levelFilter" class="text-xs px-3 py-2 rounded-lg border border-gray-200 bg-white text-gray-700 focus:outline-none focus:ring-2 focus:ring-teal-500 cursor-pointer">
              <option value="">All Levels</option>
              <option value="beginner">Beginner</option>
              <option value="intermediate">Intermediate</option>
              <option value="advanced">Advanced</option>
            </select>
          </div>
        </div>

        <!-- AI Recommendations -->
        <div class="content-area p-5 border-l-4 border-teal-500">
          <div class="flex items-center gap-3 mb-4">
            <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-teal-500 to-teal-600 flex items-center justify-center shadow-lg">
              <i class="fas fa-robot text-white text-sm"></i>
            </div>
            <div>
              <h3 class="font-semibold text-gray-900 text-sm">Recommended for You</h3>
              <p class="text-[11px] text-gray-500">Based on your interests and learning history</p>
            </div>
          </div>
          <div class="grid grid-cols-1 md:grid-cols-3 gap-3">
            <div v-for="course in recommendedCourses" :key="course.id"
                 class="flex items-center gap-3 p-3 bg-teal-50/50 rounded-xl cursor-pointer hover:bg-teal-100/50 transition-colors border border-teal-100">
              <div class="w-10 h-10 rounded-lg bg-white flex items-center justify-center flex-shrink-0 shadow-sm">
                <i :class="[course.icon, 'text-teal-600']"></i>
              </div>
              <div class="flex-1 min-w-0">
                <h4 class="font-medium text-gray-900 text-xs truncate">{{ course.title }}</h4>
                <p class="text-[10px] text-gray-500">{{ course.duration }} · {{ course.level }}</p>
              </div>
              <span class="px-2 py-0.5 bg-green-100 text-green-600 text-[10px] font-semibold rounded-full">{{ course.matchScore }}%</span>
            </div>
          </div>
        </div>

        <!-- Categories Grid -->
        <div class="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-6 gap-3">
          <div v-for="cat in categories" :key="cat.id"
               @click="categoryFilter = categoryFilter === cat.id ? '' : cat.id"
               :class="[
                 'p-4 rounded-xl cursor-pointer transition-all text-center border',
                 categoryFilter === cat.id
                   ? 'bg-teal-50 border-teal-200 shadow-md'
                   : 'bg-white border-gray-100 hover:border-teal-200 hover:shadow-md'
               ]">
            <div class="w-10 h-10 rounded-lg mx-auto mb-2 flex items-center justify-center" :style="{ backgroundColor: cat.color + '15' }">
              <i :class="[cat.icon, 'text-lg']" :style="{ color: cat.color }"></i>
            </div>
            <h3 class="font-semibold text-gray-900 text-xs mb-0.5">{{ cat.name }}</h3>
            <p class="text-[10px] text-gray-500">{{ cat.courseCount }} courses</p>
          </div>
        </div>

        <!-- All Courses Section -->
        <div class="content-area p-6">
          <div class="flex items-center justify-between mb-5">
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-teal-500 to-teal-600 flex items-center justify-center shadow-lg shadow-teal-200">
                <i class="fas fa-compass text-white text-sm"></i>
              </div>
              <div>
                <h2 class="text-lg font-bold text-gray-900">All Courses</h2>
                <p class="text-xs text-gray-500">{{ filteredCatalog.length }} courses available</p>
              </div>
            </div>
          </div>

          <div class="grid grid-cols-[repeat(auto-fill,minmax(320px,1fr))] gap-5">
            <div v-for="course in filteredCatalog" :key="course.id"
                 class="course-card group bg-white rounded-2xl overflow-hidden cursor-pointer transition-all duration-300 hover:-translate-y-1.5 border border-gray-100 shadow-sm hover:shadow-lg hover:border-teal-200">
              <!-- Card Image -->
              <div class="relative h-40 overflow-hidden">
                <img :src="course.image" :alt="course.title" class="w-full h-full object-cover transition-transform duration-500 group-hover:scale-110">
                <div class="absolute inset-0 bg-gradient-to-t from-black/60 via-black/20 to-transparent"></div>

                <!-- Level Badge -->
                <div class="absolute top-3 left-3">
                  <span :class="['course-level-badge', course.levelClass]">
                    {{ course.level }}
                  </span>
                </div>

                <!-- Enroll Button on Hover -->
                <div class="absolute bottom-3 right-3 opacity-0 group-hover:opacity-100 transition-all duration-300 translate-y-2 group-hover:translate-y-0">
                  <button class="px-3 py-1.5 bg-white text-teal-600 text-xs font-semibold rounded-full flex items-center gap-1.5 shadow-md hover:bg-teal-500 hover:text-white transition-colors">
                    <span>Enroll</span>
                    <i class="fas fa-arrow-right text-[10px]"></i>
                  </button>
                </div>
              </div>

              <!-- Card Content -->
              <div class="p-4">
                <!-- Meta Info -->
                <div class="flex items-center gap-3 text-[11px] text-gray-400 mb-2">
                  <span class="flex items-center gap-1">
                    <i class="fas fa-clock text-[9px]"></i>
                    {{ course.duration }}
                  </span>
                  <span class="flex items-center gap-1">
                    <i class="fas fa-play-circle text-[9px]"></i>
                    {{ course.lessons }} lessons
                  </span>
                </div>

                <!-- Title -->
                <h3 class="text-sm font-semibold text-gray-800 mb-2 line-clamp-2 group-hover:text-teal-600 transition-colors leading-snug">
                  {{ course.title }}
                </h3>

                <!-- Tags -->
                <div class="flex flex-wrap gap-1.5 mb-3">
                  <span v-for="tag in course.tags" :key="tag"
                        class="px-2 py-0.5 bg-gray-100 text-gray-600 text-[10px] font-medium rounded-full">
                    {{ tag }}
                  </span>
                </div>

                <!-- Footer -->
                <div class="flex items-center justify-between pt-3 border-t border-gray-100">
                  <!-- Instructor -->
                  <div class="flex items-center gap-2">
                    <div class="w-7 h-7 rounded-full bg-gradient-to-br from-teal-400 to-cyan-500 flex items-center justify-center text-white text-[10px] font-semibold shadow-sm">
                      {{ course.instructorInitials }}
                    </div>
                    <span class="text-xs text-gray-600 font-medium">{{ course.instructor }}</span>
                  </div>

                  <!-- Stats -->
                  <div class="flex items-center gap-3 text-[11px] text-gray-400">
                    <span class="flex items-center gap-1">
                      <i class="fas fa-star text-amber-400 text-[9px]"></i>
                      {{ course.rating }}
                    </span>
                    <span class="flex items-center gap-1">
                      <i class="fas fa-users text-[9px]"></i>
                      {{ course.students >= 1000 ? (course.students / 1000).toFixed(1) + 'k' : course.students }}
                    </span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Learning Paths Tab -->
      <div v-if="activeTab === 'paths'" class="space-y-6">
        <div class="content-area p-6">
          <div class="flex items-center gap-3 mb-6">
            <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-teal-500 to-teal-600 flex items-center justify-center shadow-lg shadow-teal-200">
              <i class="fas fa-route text-white text-sm"></i>
            </div>
            <div>
              <h2 class="text-lg font-bold text-gray-900">Learning Paths</h2>
              <p class="text-xs text-gray-500">Structured learning journeys to master specific skills</p>
            </div>
          </div>

          <div class="grid grid-cols-1 lg:grid-cols-2 gap-5">
            <div v-for="path in learningPaths" :key="path.id"
                 class="group bg-white rounded-2xl overflow-hidden border border-gray-100 shadow-sm hover:shadow-lg hover:border-teal-200 transition-all cursor-pointer">
              <!-- Path Image -->
              <div class="relative h-32 overflow-hidden">
                <img :src="path.image" :alt="path.title" class="w-full h-full object-cover">
                <div class="absolute inset-0 bg-gradient-to-r from-black/70 via-black/50 to-transparent"></div>
                <div class="absolute inset-0 p-4 flex items-center">
                  <div class="w-14 h-14 rounded-xl flex items-center justify-center mr-4" :style="{ backgroundColor: path.color }">
                    <i :class="[path.icon, 'text-white text-xl']"></i>
                  </div>
                  <div>
                    <span :class="['course-level-badge', path.levelClass]">{{ path.level }}</span>
                    <h3 class="text-lg font-bold text-white mt-1">{{ path.title }}</h3>
                  </div>
                </div>
              </div>

              <!-- Path Content -->
              <div class="p-4">
                <p class="text-xs text-gray-600 mb-3 line-clamp-2">{{ path.description }}</p>

                <div class="flex flex-wrap items-center gap-3 text-[11px] text-gray-500 mb-4">
                  <span class="flex items-center gap-1"><i class="fas fa-book text-[9px]"></i>{{ path.courses }} courses</span>
                  <span class="flex items-center gap-1"><i class="fas fa-clock text-[9px]"></i>{{ path.duration }}</span>
                  <span class="flex items-center gap-1"><i class="fas fa-users text-[9px]"></i>{{ path.enrolled.toLocaleString() }} enrolled</span>
                </div>

                <div class="flex items-center justify-between">
                  <div v-if="path.progress !== undefined" class="flex-1 mr-4">
                    <div class="flex items-center justify-between text-xs mb-1">
                      <span class="text-gray-600">Progress</span>
                      <span class="font-semibold text-teal-600">{{ path.progress }}%</span>
                    </div>
                    <div class="h-1.5 bg-gray-100 rounded-full overflow-hidden">
                      <div class="h-full bg-gradient-to-r from-teal-400 to-teal-500 rounded-full" :style="{ width: path.progress + '%' }"></div>
                    </div>
                  </div>
                  <button :class="[
                    'px-4 py-2 rounded-lg text-xs font-semibold transition-all flex items-center gap-2',
                    path.progress !== undefined
                      ? 'bg-gradient-to-r from-teal-500 to-teal-600 text-white hover:from-teal-600 hover:to-teal-700'
                      : 'bg-white border border-teal-500 text-teal-600 hover:bg-teal-50'
                  ]">
                    <i :class="path.progress !== undefined ? 'fas fa-play' : 'fas fa-plus'"></i>
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
        <div class="content-area p-6">
          <div class="flex items-center gap-3 mb-6">
            <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-amber-400 to-orange-500 flex items-center justify-center shadow-lg shadow-amber-200">
              <i class="fas fa-certificate text-white text-sm"></i>
            </div>
            <div>
              <h2 class="text-lg font-bold text-gray-900">My Certificates</h2>
              <p class="text-xs text-gray-500">{{ earnedCertificates.length }} certificates earned</p>
            </div>
          </div>

          <div v-if="earnedCertificates.length > 0" class="grid grid-cols-[repeat(auto-fill,minmax(320px,1fr))] gap-5">
            <div v-for="cert in earnedCertificates" :key="cert.id"
                 class="group bg-white rounded-2xl overflow-hidden border border-gray-100 shadow-sm hover:shadow-lg hover:border-amber-200 transition-all">
              <!-- Certificate Visual -->
              <div class="relative h-36 bg-gradient-to-br from-amber-400 via-orange-400 to-amber-500 flex items-center justify-center overflow-hidden">
                <div class="absolute inset-0 opacity-10">
                  <div class="absolute top-0 right-0 w-32 h-32 border-4 border-white/30 rounded-full -translate-y-1/2 translate-x-1/2"></div>
                  <div class="absolute bottom-0 left-0 w-24 h-24 border-4 border-white/30 rounded-full translate-y-1/2 -translate-x-1/2"></div>
                </div>
                <div class="text-center relative z-10">
                  <div class="w-16 h-16 mx-auto mb-2 rounded-full bg-white/90 flex items-center justify-center shadow-lg">
                    <i :class="[cert.icon, 'text-2xl']" :style="{ color: cert.color }"></i>
                  </div>
                  <p class="text-white/80 text-[10px] font-medium uppercase tracking-wider">Certificate of Completion</p>
                </div>
              </div>

              <!-- Certificate Info -->
              <div class="p-4">
                <h3 class="font-semibold text-gray-900 text-sm mb-1">{{ cert.title }}</h3>
                <p class="text-xs text-gray-500 mb-2">{{ cert.course }}</p>
                <div class="flex items-center gap-3 text-[11px] text-gray-400 mb-4">
                  <span class="flex items-center gap-1">
                    <i class="fas fa-calendar text-[9px]"></i>
                    {{ cert.date }}
                  </span>
                  <span class="flex items-center gap-1">
                    <i class="fas fa-fingerprint text-[9px]"></i>
                    {{ cert.credentialId }}
                  </span>
                </div>
                <div class="flex items-center gap-2">
                  <button class="flex-1 px-3 py-2 bg-gradient-to-r from-teal-500 to-teal-600 text-white rounded-lg text-xs font-medium hover:from-teal-600 hover:to-teal-700 transition-all flex items-center justify-center gap-1.5">
                    <i class="fas fa-eye text-[10px]"></i> View
                  </button>
                  <button class="flex-1 px-3 py-2 bg-white border border-gray-200 text-gray-700 rounded-lg text-xs font-medium hover:bg-gray-50 transition-all flex items-center justify-center gap-1.5">
                    <i class="fas fa-download text-[10px]"></i> Download
                  </button>
                  <button class="px-3 py-2 bg-white border border-gray-200 text-gray-700 rounded-lg text-xs font-medium hover:bg-gray-50 transition-all">
                    <i class="fas fa-share-alt text-[10px]"></i>
                  </button>
                </div>
              </div>
            </div>
          </div>

          <!-- Empty State -->
          <div v-else class="text-center py-16">
            <div class="w-20 h-20 rounded-full bg-gray-100 flex items-center justify-center mx-auto mb-4">
              <i class="fas fa-certificate text-gray-300 text-3xl"></i>
            </div>
            <h3 class="font-semibold text-gray-900 mb-2">No certificates yet</h3>
            <p class="text-gray-500 text-sm mb-6 max-w-md mx-auto">Complete courses to earn certificates and showcase your achievements</p>
            <button @click="activeTab = 'catalog'" class="px-6 py-2.5 bg-gradient-to-r from-teal-500 to-teal-600 text-white rounded-xl font-medium text-sm hover:from-teal-600 hover:to-teal-700 transition-all">
              Browse Courses
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.page-view {
  min-height: 100vh;
  background: linear-gradient(180deg, #f0fdfa 0%, #f8fafc 15%, #ffffff 100%);
}

/* Hero Section */
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
  gap: 8px;
  text-align: center;
  transition: all 0.3s ease;
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
  font-size: 20px;
  transition: transform 0.3s ease;
}

.stat-value-mini {
  font-size: 24px;
  font-weight: 700;
  color: white;
  line-height: 1;
}

.stat-label-mini {
  font-size: 11px;
  color: rgba(255, 255, 255, 0.7);
  line-height: 1;
}

/* Animated Circles */
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
    transform: translate(33%, -50%);
  }
  25% {
    transform: translate(28%, -45%);
  }
  50% {
    transform: translate(35%, -55%);
  }
  75% {
    transform: translate(30%, -48%);
  }
}

@keyframes drift-2 {
  0%, 100% {
    transform: translate(-30%, 50%);
  }
  33% {
    transform: translate(-25%, 45%);
  }
  66% {
    transform: translate(-35%, 55%);
  }
}

@keyframes drift-3 {
  0%, 100% {
    transform: translate(0, -50%);
  }
  50% {
    transform: translate(10%, -45%);
  }
}

/* Content Area */
.content-area {
  background: white;
  border-radius: 1rem;
  border: 1px solid #f3f4f6;
  box-shadow: 0 1px 3px 0 rgb(0 0 0 / 0.1), 0 1px 2px -1px rgb(0 0 0 / 0.1);
}

/* Course Cards */
.course-card {
  position: relative;
}

/* Level Badges */
.course-level-badge {
  display: inline-flex;
  align-items: center;
  padding: 0.25rem 0.5rem;
  font-size: 0.625rem;
  font-weight: 600;
  border-radius: 0.1875rem;
  text-transform: uppercase;
  letter-spacing: 0.025em;
}

.course-level-badge.beginner {
  background: #dcfce7;
  color: #16a34a;
}

.course-level-badge.intermediate {
  background: #dbeafe;
  color: #2563eb;
}

.course-level-badge.advanced {
  background: #ede9fe;
  color: #7c3aed;
}

/* Status Badges */
.course-status-badge {
  display: inline-flex;
  align-items: center;
  padding: 0.25rem 0.5rem;
  font-size: 0.625rem;
  font-weight: 600;
  border-radius: 0.1875rem;
  text-transform: uppercase;
  letter-spacing: 0.025em;
}

.course-status-badge.in-progress {
  background: #dbeafe;
  color: #2563eb;
}

.course-status-badge.completed {
  background: #dcfce7;
  color: #16a34a;
}

.course-status-badge.not-started {
  background: #f3f4f6;
  color: #6b7280;
}

/* Scrollbar Hide */
.scrollbar-hide::-webkit-scrollbar {
  display: none;
}

.scrollbar-hide {
  -ms-overflow-style: none;
  scrollbar-width: none;
}

/* Responsive */
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
    font-size: 14px;
  }

  .stat-value-mini {
    font-size: 16px;
  }

  .stat-label-mini {
    font-size: 8px;
  }
}

@media (max-width: 640px) {
  .stats-top-right {
    grid-template-columns: repeat(2, 1fr);
  }
}
</style>
