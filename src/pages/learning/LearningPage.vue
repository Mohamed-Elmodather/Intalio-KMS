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
    totalCourses: 5,
    completedCourses: 3,
    duration: '40 hours',
    enrolled: 1234,
    progress: 60,
    icon: 'fas fa-chart-line',
    color: '#0d9488',
    image: 'https://images.unsplash.com/photo-1551288049-bebda4e38f71?w=400&h=200&fit=crop',
    skills: ['Python', 'ML', 'Data Viz'],
    isEnrolled: true,
    rating: 4.8,
    lastActivity: '2 days ago',
    courses: [
      { title: 'Python Fundamentals', completed: true },
      { title: 'Data Analysis with Pandas', completed: true },
      { title: 'Machine Learning Basics', completed: true },
      { title: 'Deep Learning Intro', completed: false },
      { title: 'Capstone Project', completed: false }
    ]
  },
  {
    id: 2,
    title: 'Leadership Excellence',
    description: 'Develop essential leadership skills for managing teams effectively',
    level: 'Intermediate',
    levelClass: 'intermediate',
    totalCourses: 4,
    completedCourses: 0,
    duration: '24 hours',
    enrolled: 2456,
    progress: 0,
    icon: 'fas fa-users-cog',
    color: '#f59e0b',
    image: 'https://images.unsplash.com/photo-1519389950473-47ba0277781c?w=400&h=200&fit=crop',
    skills: ['Management', 'Communication', 'Strategy'],
    isEnrolled: false,
    rating: 4.7,
    lastActivity: null,
    courses: [
      { title: 'Leadership Foundations', completed: false },
      { title: 'Team Management', completed: false },
      { title: 'Strategic Thinking', completed: false },
      { title: 'Conflict Resolution', completed: false }
    ]
  },
  {
    id: 3,
    title: 'Full Stack Developer',
    description: 'Learn front-end and back-end development from scratch',
    level: 'Beginner',
    levelClass: 'beginner',
    totalCourses: 8,
    completedCourses: 0,
    duration: '80 hours',
    enrolled: 3789,
    progress: 0,
    icon: 'fas fa-code',
    color: '#6366f1',
    image: 'https://images.unsplash.com/photo-1555066931-4365d14bab8c?w=400&h=200&fit=crop',
    skills: ['HTML/CSS', 'JavaScript', 'React', 'Node.js'],
    isEnrolled: false,
    rating: 4.9,
    lastActivity: null,
    courses: [
      { title: 'HTML & CSS Basics', completed: false },
      { title: 'JavaScript Fundamentals', completed: false },
      { title: 'React Development', completed: false },
      { title: 'Node.js Backend', completed: false },
      { title: 'Database Design', completed: false },
      { title: 'API Development', completed: false },
      { title: 'Testing & Deployment', completed: false },
      { title: 'Final Project', completed: false }
    ]
  },
  {
    id: 4,
    title: 'Cloud Solutions Architect',
    description: 'Design and implement scalable cloud infrastructure solutions',
    level: 'Advanced',
    levelClass: 'advanced',
    totalCourses: 6,
    completedCourses: 2,
    duration: '50 hours',
    enrolled: 1567,
    progress: 25,
    icon: 'fas fa-cloud',
    color: '#3b82f6',
    image: 'https://images.unsplash.com/photo-1451187580459-43490279c0fa?w=400&h=200&fit=crop',
    skills: ['AWS', 'Azure', 'Docker', 'Kubernetes'],
    isEnrolled: true,
    rating: 4.6,
    lastActivity: '1 week ago',
    courses: [
      { title: 'Cloud Fundamentals', completed: true },
      { title: 'AWS Services Deep Dive', completed: true },
      { title: 'Containerization', completed: false },
      { title: 'Kubernetes Orchestration', completed: false },
      { title: 'Security & Compliance', completed: false },
      { title: 'Architecture Project', completed: false }
    ]
  },
  {
    id: 5,
    title: 'Digital Marketing Mastery',
    description: 'Learn modern digital marketing strategies and analytics',
    level: 'Intermediate',
    levelClass: 'intermediate',
    totalCourses: 5,
    completedCourses: 0,
    duration: '35 hours',
    enrolled: 1890,
    progress: 0,
    icon: 'fas fa-bullhorn',
    color: '#ec4899',
    image: 'https://images.unsplash.com/photo-1460925895917-afdab827c52f?w=400&h=200&fit=crop',
    skills: ['SEO', 'Social Media', 'Analytics', 'Content'],
    isEnrolled: false,
    rating: 4.5,
    lastActivity: null,
    courses: [
      { title: 'Digital Marketing Foundations', completed: false },
      { title: 'SEO Optimization', completed: false },
      { title: 'Social Media Strategy', completed: false },
      { title: 'Content Marketing', completed: false },
      { title: 'Analytics & Reporting', completed: false }
    ]
  },
  {
    id: 6,
    title: 'Cybersecurity Specialist',
    description: 'Protect systems and data with comprehensive security knowledge',
    level: 'Advanced',
    levelClass: 'advanced',
    totalCourses: 7,
    completedCourses: 0,
    duration: '60 hours',
    enrolled: 2234,
    progress: 0,
    icon: 'fas fa-shield-alt',
    color: '#ef4444',
    image: 'https://images.unsplash.com/photo-1550751827-4bd374c3f58b?w=400&h=200&fit=crop',
    skills: ['Network Security', 'Ethical Hacking', 'Compliance'],
    isEnrolled: false,
    rating: 4.8,
    lastActivity: null,
    courses: [
      { title: 'Security Fundamentals', completed: false },
      { title: 'Network Security', completed: false },
      { title: 'Ethical Hacking', completed: false },
      { title: 'Incident Response', completed: false },
      { title: 'Compliance & Governance', completed: false },
      { title: 'Cloud Security', completed: false },
      { title: 'Security Capstone', completed: false }
    ]
  }
])

// Earned certificates
const earnedCertificates = ref([
  { id: 1, title: 'Cybersecurity Fundamentals', course: 'Cybersecurity Fundamentals', date: 'Dec 15, 2024', credentialId: 'AFC-CS-2024-001', icon: 'fas fa-shield-alt', color: '#3b82f6', instructor: 'Alex Thompson', hours: 4, score: 92, skills: ['Security Basics', 'Threat Detection'] },
  { id: 2, title: 'Project Management Basics', course: 'Project Management Pro', date: 'Nov 28, 2024', credentialId: 'AFC-PM-2024-002', icon: 'fas fa-tasks', color: '#10b981', instructor: 'David Lee', hours: 10, score: 88, skills: ['Agile', 'Scrum', 'Planning'] },
  { id: 3, title: 'Effective Communication', course: 'Effective Communication', date: 'Nov 10, 2024', credentialId: 'AFC-EC-2024-003', icon: 'fas fa-comments', color: '#8b5cf6', instructor: 'Sarah Chen', hours: 5, score: 95, skills: ['Presentation', 'Writing'] },
  { id: 4, title: 'Data Privacy Compliance', course: 'Data Privacy & GDPR', date: 'Oct 22, 2024', credentialId: 'AFC-DP-2024-004', icon: 'fas fa-user-shield', color: '#ef4444', instructor: 'Michael Brown', hours: 6, score: 90, skills: ['GDPR', 'Compliance'] },
  { id: 5, title: 'Time Management Mastery', course: 'Time Management', date: 'Sep 15, 2024', credentialId: 'AFC-TM-2024-005', icon: 'fas fa-clock', color: '#f59e0b', instructor: 'Emma Wilson', hours: 3, score: 87, skills: ['Productivity', 'Planning'] },
])

// Path filter
const pathFilter = ref('all')

// Computed for paths
const filteredPaths = computed(() => {
  if (pathFilter.value === 'enrolled') return learningPaths.value.filter(p => p.isEnrolled)
  if (pathFilter.value === 'available') return learningPaths.value.filter(p => !p.isEnrolled)
  return learningPaths.value
})

const myEnrolledPaths = computed(() => learningPaths.value.filter(p => p.isEnrolled))

// Certificate stats
const certificateStats = computed(() => ({
  total: earnedCertificates.value.length,
  thisYear: earnedCertificates.value.filter(c => c.date.includes('2024')).length,
  totalHours: earnedCertificates.value.reduce((sum, c) => sum + c.hours, 0),
  avgScore: Math.round(earnedCertificates.value.reduce((sum, c) => sum + c.score, 0) / earnedCertificates.value.length)
}))

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
        <!-- Continue Learning Hero Card -->
        <div v-if="currentCourse" class="content-area overflow-hidden">
          <div class="flex items-stretch">
            <!-- Left: Course Image with Play Button -->
            <div class="relative w-32 md:w-48 flex-shrink-0 overflow-hidden">
              <img :src="enrolledCourses[0]?.image" :alt="currentCourse.title" class="w-full h-full object-cover">
              <div class="absolute inset-0 bg-gradient-to-r from-transparent to-black/30"></div>
              <div class="absolute inset-0 flex items-center justify-center">
                <button @click="navigateToCourse(currentCourse.id)" class="w-10 h-10 md:w-12 md:h-12 rounded-full bg-white/95 hover:bg-white flex items-center justify-center shadow-xl transition-all hover:scale-110 group">
                  <i class="fas fa-play text-teal-600 text-sm md:text-base ml-0.5 group-hover:text-teal-700"></i>
                </button>
              </div>
            </div>

            <!-- Right: Content -->
            <div class="flex-1 p-3 md:p-5 bg-gradient-to-br from-teal-500 to-teal-600">
              <div class="flex flex-wrap items-center gap-2 mb-2">
                <span class="px-2 py-0.5 bg-white/20 text-white text-[10px] font-semibold rounded-full backdrop-blur-sm flex items-center gap-1">
                  <i class="fas fa-bolt text-[9px]"></i>Continue Learning
                </span>
                <span class="px-2 py-0.5 bg-amber-400 text-amber-900 text-[10px] font-semibold rounded-full flex items-center gap-1">
                  <i class="fas fa-fire text-[9px]"></i>{{ streak }} Day Streak
                </span>
              </div>

              <h2 class="text-base md:text-lg font-bold text-white mb-1">{{ currentCourse.title }}</h2>
              <p class="text-teal-100 text-xs mb-3">{{ currentCourse.nextLesson }}</p>

              <!-- Progress Bar -->
              <div class="mb-3 max-w-sm">
                <div class="flex items-center justify-between text-[11px] text-white/80 mb-1">
                  <span>Progress</span>
                  <span class="font-semibold text-white">{{ currentCourse.progress }}%</span>
                </div>
                <div class="h-1.5 bg-white/20 rounded-full overflow-hidden">
                  <div class="h-full bg-white rounded-full transition-all duration-500" :style="{ width: currentCourse.progress + '%' }"></div>
                </div>
              </div>

              <!-- Quick Stats & Action -->
              <div class="flex flex-wrap items-center justify-between gap-3">
                <div class="flex items-center gap-3 text-white/80 text-[11px]">
                  <span class="flex items-center gap-1"><i class="fas fa-clock text-[9px]"></i>8h</span>
                  <span class="flex items-center gap-1"><i class="fas fa-play-circle text-[9px]"></i>9/12</span>
                  <span class="flex items-center gap-1"><i class="fas fa-star text-amber-300 text-[9px]"></i>4.8</span>
                </div>
                <button @click="navigateToCourse(currentCourse.id)" class="px-4 py-2 bg-white text-teal-600 rounded-lg font-semibold text-xs hover:bg-teal-50 transition-all shadow-md flex items-center gap-1.5">
                  <i class="fas fa-play text-[10px]"></i>
                  Resume
                </button>
              </div>
            </div>
          </div>
        </div>

        <!-- Quick Stats Row -->
        <div class="grid grid-cols-2 md:grid-cols-4 gap-3">
          <div class="content-area p-3 md:p-4 flex items-center gap-3">
            <div class="w-9 h-9 md:w-10 md:h-10 rounded-xl bg-blue-100 flex items-center justify-center flex-shrink-0">
              <i class="fas fa-book-open text-blue-600 text-xs md:text-sm"></i>
            </div>
            <div>
              <p class="text-base md:text-lg font-bold text-gray-900">{{ enrolledCourses.filter(c => c.progress > 0 && c.progress < 100).length }}</p>
              <p class="text-[10px] md:text-[11px] text-gray-500">In Progress</p>
            </div>
          </div>
          <div class="content-area p-3 md:p-4 flex items-center gap-3">
            <div class="w-9 h-9 md:w-10 md:h-10 rounded-xl bg-teal-100 flex items-center justify-center flex-shrink-0">
              <i class="fas fa-check-circle text-teal-600 text-xs md:text-sm"></i>
            </div>
            <div>
              <p class="text-base md:text-lg font-bold text-gray-900">{{ enrolledCourses.filter(c => c.progress === 100).length }}</p>
              <p class="text-[10px] md:text-[11px] text-gray-500">Completed</p>
            </div>
          </div>
          <div class="content-area p-3 md:p-4 flex items-center gap-3">
            <div class="w-9 h-9 md:w-10 md:h-10 rounded-xl bg-gray-100 flex items-center justify-center flex-shrink-0">
              <i class="fas fa-clock text-gray-600 text-xs md:text-sm"></i>
            </div>
            <div>
              <p class="text-base md:text-lg font-bold text-gray-900">{{ enrolledCourses.filter(c => c.progress === 0).length }}</p>
              <p class="text-[10px] md:text-[11px] text-gray-500">Not Started</p>
            </div>
          </div>
          <div class="content-area p-3 md:p-4 flex items-center gap-3">
            <div class="w-9 h-9 md:w-10 md:h-10 rounded-xl bg-amber-100 flex items-center justify-center flex-shrink-0">
              <i class="fas fa-graduation-cap text-amber-600 text-xs md:text-sm"></i>
            </div>
            <div>
              <p class="text-base md:text-lg font-bold text-gray-900">{{ enrolledCourses.length }}</p>
              <p class="text-[10px] md:text-[11px] text-gray-500">Total Enrolled</p>
            </div>
          </div>
        </div>

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
                 class="course-card group bg-white rounded-2xl overflow-hidden cursor-pointer transition-all duration-300 hover:-translate-y-1.5 border border-gray-100 shadow-sm hover:shadow-xl hover:border-teal-200">
              <!-- Card Image -->
              <div class="relative h-40 overflow-hidden">
                <img :src="course.image" :alt="course.title" class="w-full h-full object-cover transition-transform duration-500 group-hover:scale-110">
                <div class="absolute inset-0 bg-gradient-to-t from-black/60 via-black/20 to-transparent"></div>

                <!-- Status Badge -->
                <div class="absolute top-3 left-3 flex items-center gap-2">
                  <span :class="['course-status-badge', course.statusClass]">
                    {{ course.status }}
                  </span>
                </div>

                <!-- Play Button on Hover -->
                <div class="absolute inset-0 flex items-center justify-center opacity-0 group-hover:opacity-100 transition-all">
                  <div class="w-12 h-12 rounded-full bg-white/90 flex items-center justify-center shadow-lg">
                    <i class="fas fa-play text-teal-600 ml-0.5"></i>
                  </div>
                </div>

                <!-- Progress Overlay -->
                <div class="absolute bottom-0 left-0 right-0 h-1.5 bg-black/30">
                  <div class="h-full bg-gradient-to-r from-teal-400 to-teal-500 transition-all" :style="{ width: course.progress + '%' }"></div>
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
                    <span class="font-bold text-teal-600 text-sm">{{ course.progress }}%</span>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- List View -->
          <div v-else class="space-y-3">
            <div v-for="course in filteredEnrolledCourses" :key="course.id"
                 @click="navigateToCourse(course.id)"
                 class="group flex gap-4 p-4 bg-white rounded-2xl cursor-pointer transition-all duration-300 border border-gray-100 shadow-sm hover:shadow-lg hover:border-teal-200">
              <!-- Thumbnail -->
              <div class="relative w-44 h-28 flex-shrink-0 rounded-xl overflow-hidden">
                <img :src="course.image" :alt="course.title" class="w-full h-full object-cover transition-transform duration-300 group-hover:scale-110">
                <div class="absolute inset-0 bg-black/20 group-hover:bg-black/30 transition-all"></div>
                <div class="absolute inset-0 flex items-center justify-center opacity-0 group-hover:opacity-100 transition-all">
                  <div class="w-10 h-10 rounded-full bg-white/90 flex items-center justify-center shadow-lg">
                    <i class="fas fa-play text-teal-600 text-sm ml-0.5"></i>
                  </div>
                </div>
                <div class="absolute bottom-0 left-0 right-0 h-1.5 bg-black/30">
                  <div class="h-full bg-gradient-to-r from-teal-400 to-teal-500" :style="{ width: course.progress + '%' }"></div>
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
                <div class="flex items-center gap-4 text-[11px] text-gray-400 mb-2">
                  <span><i class="fas fa-clock text-[9px] mr-1"></i>{{ course.duration }}</span>
                  <span><i class="fas fa-play-circle text-[9px] mr-1"></i>{{ course.completedLessons }}/{{ course.totalLessons }} lessons</span>
                  <span><i class="fas fa-star text-amber-400 text-[9px] mr-1"></i>{{ course.rating }}</span>
                </div>
                <!-- Tags -->
                <div class="flex flex-wrap gap-1.5">
                  <span v-for="tag in course.tags" :key="tag"
                        class="px-2 py-0.5 bg-gray-100 text-gray-500 text-[10px] font-medium rounded-full">
                    {{ tag }}
                  </span>
                </div>
              </div>

              <!-- Progress & Action -->
              <div class="flex flex-col items-end justify-between">
                <div class="text-right">
                  <span class="text-xl font-bold text-teal-600">{{ course.progress }}%</span>
                  <p class="text-[10px] text-gray-400">complete</p>
                </div>
                <button class="px-5 py-2.5 bg-gradient-to-r from-teal-500 to-teal-600 text-white rounded-xl text-xs font-semibold hover:from-teal-600 hover:to-teal-700 transition-all shadow-lg shadow-teal-200 flex items-center gap-2">
                  <i :class="course.progress > 0 && course.progress < 100 ? 'fas fa-play' : course.progress === 100 ? 'fas fa-redo' : 'fas fa-play'" class="text-[10px]"></i>
                  {{ course.progress > 0 && course.progress < 100 ? 'Continue' : course.progress === 100 ? 'Review' : 'Start' }}
                </button>
              </div>
            </div>
          </div>

          <!-- Empty State -->
          <div v-if="filteredEnrolledCourses.length === 0" class="text-center py-16">
            <div class="w-20 h-20 rounded-full bg-gradient-to-br from-teal-100 to-cyan-100 flex items-center justify-center mx-auto mb-4">
              <i class="fas fa-book-open text-teal-400 text-3xl"></i>
            </div>
            <h3 class="font-semibold text-gray-900 mb-2">No courses found</h3>
            <p class="text-gray-500 text-sm mb-6 max-w-md mx-auto">Try changing your filter or browse our catalog to find new courses</p>
            <button @click="activeTab = 'catalog'" class="px-6 py-2.5 bg-gradient-to-r from-teal-500 to-teal-600 text-white rounded-xl font-semibold text-sm hover:from-teal-600 hover:to-teal-700 transition-all shadow-lg shadow-teal-200 flex items-center gap-2 mx-auto">
              <i class="fas fa-compass"></i>
              Browse Catalog
            </button>
          </div>
        </div>
      </div>

      <!-- Browse Catalog Tab -->
      <div v-if="activeTab === 'catalog'" class="space-y-6">
        <!-- Search & Filters Bar -->
        <div class="content-area p-4">
          <div class="flex flex-wrap gap-3 items-center">
            <div class="flex-1 min-w-[280px] relative">
              <i class="fas fa-search absolute left-4 top-1/2 -translate-y-1/2 text-gray-400 text-sm"></i>
              <input type="text" v-model="searchQuery" placeholder="Search courses, topics, or instructors..."
                     class="w-full pl-11 pr-4 py-3 bg-gray-50 border border-gray-200 rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-teal-500 focus:border-transparent focus:bg-white transition-all">
            </div>
            <select v-model="categoryFilter" class="text-xs px-4 py-3 rounded-xl border border-gray-200 bg-white text-gray-700 focus:outline-none focus:ring-2 focus:ring-teal-500 cursor-pointer">
              <option value="">All Categories</option>
              <option v-for="cat in categories" :key="cat.id" :value="cat.id">{{ cat.name }}</option>
            </select>
            <select v-model="levelFilter" class="text-xs px-4 py-3 rounded-xl border border-gray-200 bg-white text-gray-700 focus:outline-none focus:ring-2 focus:ring-teal-500 cursor-pointer">
              <option value="">All Levels</option>
              <option value="beginner">Beginner</option>
              <option value="intermediate">Intermediate</option>
              <option value="advanced">Advanced</option>
            </select>
            <div class="flex items-center gap-2 ml-auto">
              <span class="text-xs text-gray-500">{{ filteredCatalog.length }} courses</span>
            </div>
          </div>
        </div>

        <!-- Featured Course Banner -->
        <div class="content-area overflow-hidden">
          <div class="flex flex-col lg:flex-row">
            <!-- Featured Image -->
            <div class="relative w-full lg:w-2/5 h-56 lg:h-auto">
              <img src="https://images.unsplash.com/photo-1677442136019-21780ecad995?w=600&h=400&fit=crop" alt="Featured Course" class="w-full h-full object-cover">
              <div class="absolute inset-0 bg-gradient-to-r from-violet-600/90 to-purple-600/70 lg:bg-gradient-to-t"></div>
              <div class="absolute top-4 left-4 flex items-center gap-2">
                <span class="px-3 py-1 bg-amber-400 text-amber-900 text-[10px] font-bold rounded-full uppercase">
                  <i class="fas fa-star mr-1"></i>Featured
                </span>
                <span class="px-3 py-1 bg-white/20 backdrop-blur-sm text-white text-[10px] font-semibold rounded-full">
                  <i class="fas fa-fire mr-1"></i>Trending
                </span>
              </div>
            </div>
            <!-- Featured Content -->
            <div class="flex-1 p-6 bg-gradient-to-br from-violet-600 to-purple-700">
              <div class="flex items-center gap-2 mb-3">
                <span class="course-level-badge intermediate">Intermediate</span>
                <span class="px-2 py-0.5 bg-white/20 text-white text-[10px] font-medium rounded">NEW</span>
              </div>
              <h2 class="text-2xl font-bold text-white mb-3">Machine Learning Basics</h2>
              <p class="text-purple-100 text-sm mb-4 max-w-lg">Master the fundamentals of machine learning with hands-on projects. Learn neural networks, deep learning, and AI concepts from industry experts.</p>

              <div class="flex flex-wrap items-center gap-4 mb-5 text-white/80 text-xs">
                <span class="flex items-center gap-1.5"><i class="fas fa-clock"></i>4 hours</span>
                <span class="flex items-center gap-1.5"><i class="fas fa-play-circle"></i>12 lessons</span>
                <span class="flex items-center gap-1.5"><i class="fas fa-star text-amber-300"></i>4.8 (3,200 reviews)</span>
                <span class="flex items-center gap-1.5"><i class="fas fa-users"></i>3.2k enrolled</span>
              </div>

              <div class="flex items-center gap-4 mb-5">
                <div class="flex items-center gap-2">
                  <div class="w-10 h-10 rounded-full bg-white/20 flex items-center justify-center text-white font-semibold text-sm">SM</div>
                  <div>
                    <p class="text-white text-sm font-medium">Dr. Sarah Mitchell</p>
                    <p class="text-purple-200 text-[11px]">AI Research Lead</p>
                  </div>
                </div>
              </div>

              <div class="flex items-center gap-3">
                <button class="px-6 py-3 bg-white text-purple-600 rounded-xl font-semibold text-sm hover:bg-purple-50 transition-all shadow-lg flex items-center gap-2">
                  <i class="fas fa-play"></i>
                  Start Learning
                </button>
                <button class="px-4 py-3 bg-white/20 backdrop-blur-sm border border-white/30 text-white rounded-xl font-semibold text-sm hover:bg-white/30 transition-all flex items-center gap-2">
                  <i class="fas fa-bookmark"></i>
                  Save
                </button>
              </div>
            </div>
          </div>
        </div>

        <!-- AI Recommendations -->
        <div class="content-area p-5 border-l-4 border-teal-500">
          <div class="flex items-center justify-between mb-4">
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-teal-500 to-teal-600 flex items-center justify-center shadow-lg shadow-teal-200">
                <i class="fas fa-robot text-white text-sm"></i>
              </div>
              <div>
                <h3 class="font-semibold text-gray-900 text-sm">Recommended for You</h3>
                <p class="text-[11px] text-gray-500">Based on your interests and learning history</p>
              </div>
            </div>
            <button class="text-teal-600 text-xs font-medium hover:text-teal-700 flex items-center gap-1">
              View All <i class="fas fa-arrow-right text-[10px]"></i>
            </button>
          </div>
          <div class="grid grid-cols-1 md:grid-cols-3 gap-3">
            <div v-for="course in recommendedCourses" :key="course.id"
                 class="group flex items-center gap-3 p-3 bg-gradient-to-br from-teal-50 to-cyan-50/50 rounded-xl cursor-pointer hover:from-teal-100 hover:to-cyan-100/50 transition-all border border-teal-100 hover:border-teal-200 hover:shadow-md">
              <div class="w-12 h-12 rounded-xl bg-white flex items-center justify-center flex-shrink-0 shadow-sm group-hover:shadow-md transition-all">
                <i :class="[course.icon, 'text-teal-600 text-lg']"></i>
              </div>
              <div class="flex-1 min-w-0">
                <h4 class="font-semibold text-gray-900 text-xs truncate group-hover:text-teal-700">{{ course.title }}</h4>
                <p class="text-[10px] text-gray-500">{{ course.duration }} · {{ course.level }}</p>
                <p class="text-[10px] text-gray-400 truncate">{{ course.instructor }}</p>
              </div>
              <div class="flex flex-col items-end gap-1">
                <span class="px-2 py-0.5 bg-green-100 text-green-600 text-[10px] font-bold rounded-full">{{ course.matchScore }}% match</span>
                <i class="fas fa-arrow-right text-teal-400 text-[10px] opacity-0 group-hover:opacity-100 transition-all"></i>
              </div>
            </div>
          </div>
        </div>

        <!-- Categories Grid -->
        <div>
          <div class="flex items-center justify-between mb-4">
            <h3 class="font-semibold text-gray-900 text-sm">Browse by Category</h3>
            <button class="text-teal-600 text-xs font-medium hover:text-teal-700 flex items-center gap-1">
              All Categories <i class="fas fa-arrow-right text-[10px]"></i>
            </button>
          </div>
          <div class="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-6 gap-3">
            <div v-for="cat in categories" :key="cat.id"
                 @click="categoryFilter = categoryFilter === cat.id ? '' : cat.id"
                 :class="[
                   'group p-4 rounded-xl cursor-pointer transition-all text-center border hover:scale-105',
                   categoryFilter === cat.id
                     ? 'bg-teal-50 border-teal-300 shadow-lg shadow-teal-100'
                     : 'bg-white border-gray-100 hover:border-teal-200 hover:shadow-lg'
                 ]">
              <div class="w-12 h-12 rounded-xl mx-auto mb-2 flex items-center justify-center transition-transform group-hover:scale-110" :style="{ backgroundColor: cat.color + '15' }">
                <i :class="[cat.icon, 'text-xl']" :style="{ color: cat.color }"></i>
              </div>
              <h3 class="font-semibold text-gray-900 text-xs mb-0.5">{{ cat.name }}</h3>
              <p class="text-[10px] text-gray-500">{{ cat.courseCount }} courses</p>
            </div>
          </div>
        </div>

        <!-- New & Trending Section -->
        <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <!-- New Courses -->
          <div class="content-area p-5">
            <div class="flex items-center gap-3 mb-4">
              <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-emerald-400 to-teal-500 flex items-center justify-center shadow-lg shadow-emerald-200">
                <i class="fas fa-sparkles text-white text-sm"></i>
              </div>
              <div>
                <h3 class="font-semibold text-gray-900 text-sm">New Courses</h3>
                <p class="text-[11px] text-gray-500">Just added this week</p>
              </div>
            </div>
            <div class="space-y-3">
              <div v-for="course in catalogCourses.slice(0, 3)" :key="'new-' + course.id"
                   class="group flex gap-3 p-3 bg-gray-50 rounded-xl cursor-pointer hover:bg-teal-50 transition-all">
                <div class="relative w-20 h-14 flex-shrink-0 rounded-lg overflow-hidden">
                  <img :src="course.image" :alt="course.title" class="w-full h-full object-cover">
                  <div class="absolute inset-0 bg-black/20 group-hover:bg-black/10 transition-all"></div>
                </div>
                <div class="flex-1 min-w-0">
                  <div class="flex items-center gap-2 mb-1">
                    <span :class="['course-level-badge', course.levelClass]" style="padding: 2px 6px; font-size: 9px;">{{ course.level }}</span>
                    <span class="px-1.5 py-0.5 bg-emerald-100 text-emerald-600 text-[9px] font-semibold rounded">NEW</span>
                  </div>
                  <h4 class="font-medium text-gray-900 text-xs truncate group-hover:text-teal-600">{{ course.title }}</h4>
                  <p class="text-[10px] text-gray-500">{{ course.instructor }} · {{ course.duration }}</p>
                </div>
                <div class="flex items-center">
                  <i class="fas fa-chevron-right text-gray-300 group-hover:text-teal-500 text-xs"></i>
                </div>
              </div>
            </div>
          </div>

          <!-- Trending Courses -->
          <div class="content-area p-5">
            <div class="flex items-center gap-3 mb-4">
              <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-orange-400 to-red-500 flex items-center justify-center shadow-lg shadow-orange-200">
                <i class="fas fa-fire text-white text-sm"></i>
              </div>
              <div>
                <h3 class="font-semibold text-gray-900 text-sm">Trending Now</h3>
                <p class="text-[11px] text-gray-500">Most popular this month</p>
              </div>
            </div>
            <div class="space-y-3">
              <div v-for="(course, idx) in catalogCourses.slice(3, 6)" :key="'trending-' + course.id"
                   class="group flex gap-3 p-3 bg-gray-50 rounded-xl cursor-pointer hover:bg-orange-50 transition-all">
                <div class="w-8 h-8 rounded-lg bg-orange-100 flex items-center justify-center flex-shrink-0">
                  <span class="text-orange-600 font-bold text-sm">{{ idx + 1 }}</span>
                </div>
                <div class="relative w-16 h-12 flex-shrink-0 rounded-lg overflow-hidden">
                  <img :src="course.image" :alt="course.title" class="w-full h-full object-cover">
                </div>
                <div class="flex-1 min-w-0">
                  <h4 class="font-medium text-gray-900 text-xs truncate group-hover:text-orange-600">{{ course.title }}</h4>
                  <div class="flex items-center gap-2 text-[10px] text-gray-500">
                    <span><i class="fas fa-star text-amber-400"></i> {{ course.rating }}</span>
                    <span>{{ course.students >= 1000 ? (course.students / 1000).toFixed(1) + 'k' : course.students }} enrolled</span>
                  </div>
                </div>
                <div class="flex items-center">
                  <i class="fas fa-arrow-trend-up text-orange-400 text-sm"></i>
                </div>
              </div>
            </div>
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
            <div class="flex items-center gap-2">
              <select class="text-xs px-3 py-2 rounded-lg border border-gray-200 bg-white text-gray-700 focus:outline-none focus:ring-2 focus:ring-teal-500 cursor-pointer">
                <option>Most Popular</option>
                <option>Highest Rated</option>
                <option>Newest</option>
                <option>Duration: Short to Long</option>
              </select>
            </div>
          </div>

          <div class="grid grid-cols-[repeat(auto-fill,minmax(300px,1fr))] gap-5">
            <div v-for="course in filteredCatalog" :key="course.id"
                 class="course-card group bg-white rounded-2xl overflow-hidden cursor-pointer transition-all duration-300 hover:-translate-y-1.5 border border-gray-100 shadow-sm hover:shadow-xl hover:border-teal-200">
              <!-- Card Image -->
              <div class="relative h-40 overflow-hidden">
                <img :src="course.image" :alt="course.title" class="w-full h-full object-cover transition-transform duration-500 group-hover:scale-110">
                <div class="absolute inset-0 bg-gradient-to-t from-black/70 via-black/30 to-transparent"></div>

                <!-- Badges -->
                <div class="absolute top-3 left-3 flex items-center gap-2">
                  <span :class="['course-level-badge', course.levelClass]">
                    {{ course.level }}
                  </span>
                </div>

                <!-- Wishlist Button -->
                <button class="absolute top-3 right-3 w-8 h-8 bg-white/80 hover:bg-white rounded-full flex items-center justify-center shadow-lg opacity-0 group-hover:opacity-100 transition-all">
                  <i class="far fa-heart text-gray-600 hover:text-red-500 text-sm"></i>
                </button>

                <!-- Play Button on Hover -->
                <div class="absolute inset-0 flex items-center justify-center opacity-0 group-hover:opacity-100 transition-all">
                  <div class="w-14 h-14 rounded-full bg-white/90 flex items-center justify-center shadow-xl">
                    <i class="fas fa-play text-teal-600 text-lg ml-1"></i>
                  </div>
                </div>

                <!-- Bottom Info Overlay -->
                <div class="absolute bottom-3 left-3 right-3 flex items-center justify-between">
                  <div class="flex items-center gap-2">
                    <span class="px-2 py-1 bg-black/50 backdrop-blur-sm text-white text-[10px] font-medium rounded-lg">
                      <i class="fas fa-clock mr-1"></i>{{ course.duration }}
                    </span>
                    <span class="px-2 py-1 bg-black/50 backdrop-blur-sm text-white text-[10px] font-medium rounded-lg">
                      <i class="fas fa-play-circle mr-1"></i>{{ course.lessons }} lessons
                    </span>
                  </div>
                </div>
              </div>

              <!-- Card Content -->
              <div class="p-4">
                <!-- Title -->
                <h3 class="text-sm font-semibold text-gray-800 mb-2 line-clamp-2 group-hover:text-teal-600 transition-colors leading-snug h-10">
                  {{ course.title }}
                </h3>

                <!-- Tags -->
                <div class="flex flex-wrap gap-1.5 mb-3">
                  <span v-for="tag in course.tags" :key="tag"
                        class="px-2 py-0.5 bg-gray-100 text-gray-600 text-[10px] font-medium rounded-full hover:bg-teal-50 hover:text-teal-600 transition-colors">
                    {{ tag }}
                  </span>
                </div>

                <!-- Rating & Students -->
                <div class="flex items-center gap-3 text-[11px] mb-3">
                  <span class="flex items-center gap-1 text-amber-500 font-semibold">
                    <i class="fas fa-star text-[10px]"></i>
                    {{ course.rating }}
                  </span>
                  <span class="text-gray-400">({{ course.students >= 1000 ? (course.students / 1000).toFixed(1) + 'k' : course.students }} students)</span>
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

                  <!-- Enroll Button -->
                  <button class="px-3 py-1.5 bg-gradient-to-r from-teal-500 to-teal-600 text-white text-xs font-semibold rounded-lg hover:from-teal-600 hover:to-teal-700 transition-all shadow-md flex items-center gap-1.5">
                    <i class="fas fa-plus text-[10px]"></i>
                    Enroll
                  </button>
                </div>
              </div>
            </div>
          </div>

          <!-- Load More Button -->
          <div class="text-center mt-8">
            <button class="px-8 py-3 bg-white border-2 border-teal-500 text-teal-600 rounded-xl font-semibold text-sm hover:bg-teal-50 transition-all flex items-center gap-2 mx-auto">
              <i class="fas fa-plus"></i>
              Load More Courses
            </button>
          </div>
        </div>
      </div>

      <!-- Learning Paths Tab -->
      <div v-if="activeTab === 'paths'" class="space-y-6">
        <!-- My Enrolled Paths -->
        <div v-if="myEnrolledPaths.length > 0" class="content-area p-6 border-l-4 border-teal-500">
          <div class="flex items-center justify-between mb-5">
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-teal-500 to-teal-600 flex items-center justify-center shadow-lg shadow-teal-200">
                <i class="fas fa-road text-white text-sm"></i>
              </div>
              <div>
                <h2 class="text-lg font-bold text-gray-900">My Learning Paths</h2>
                <p class="text-xs text-gray-500">Continue your journey</p>
              </div>
            </div>
          </div>

          <div class="grid grid-cols-1 lg:grid-cols-2 gap-5">
            <div v-for="path in myEnrolledPaths" :key="path.id"
                 class="group bg-gradient-to-br from-gray-50 to-white rounded-2xl overflow-hidden border border-gray-100 shadow-sm hover:shadow-xl hover:border-teal-200 transition-all">
              <!-- Path Header with Image -->
              <div class="relative h-28 overflow-hidden">
                <img :src="path.image" :alt="path.title" class="w-full h-full object-cover">
                <div class="absolute inset-0 bg-gradient-to-r from-black/80 via-black/60 to-black/40"></div>
                <div class="absolute inset-0 p-4 flex items-center justify-between">
                  <div class="flex items-center gap-3">
                    <div class="w-12 h-12 rounded-xl flex items-center justify-center shadow-lg" :style="{ backgroundColor: path.color }">
                      <i :class="[path.icon, 'text-white text-lg']"></i>
                    </div>
                    <div>
                      <div class="flex items-center gap-2 mb-1">
                        <span :class="['course-level-badge', path.levelClass]">{{ path.level }}</span>
                        <span class="px-2 py-0.5 bg-teal-500/80 text-white text-[9px] font-semibold rounded">ENROLLED</span>
                      </div>
                      <h3 class="text-base font-bold text-white">{{ path.title }}</h3>
                    </div>
                  </div>
                  <!-- Progress Circle -->
                  <div class="relative w-14 h-14">
                    <svg class="w-full h-full -rotate-90" viewBox="0 0 36 36">
                      <circle cx="18" cy="18" r="16" fill="none" stroke="rgba(255,255,255,0.2)" stroke-width="3"/>
                      <circle cx="18" cy="18" r="16" fill="none" stroke="white" stroke-width="3" stroke-linecap="round"
                              :stroke-dasharray="100" :stroke-dashoffset="100 - path.progress"/>
                    </svg>
                    <div class="absolute inset-0 flex items-center justify-center">
                      <span class="text-white text-xs font-bold">{{ path.progress }}%</span>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Path Content -->
              <div class="p-4">
                <!-- Course Progress Breakdown -->
                <div class="mb-4">
                  <div class="flex items-center justify-between text-xs text-gray-600 mb-2">
                    <span>{{ path.completedCourses }} of {{ path.totalCourses }} courses completed</span>
                    <span v-if="path.lastActivity" class="text-gray-400">Last activity: {{ path.lastActivity }}</span>
                  </div>
                  <div class="space-y-1.5">
                    <div v-for="(course, idx) in path.courses.slice(0, 4)" :key="idx"
                         class="flex items-center gap-2 text-[11px]">
                      <i :class="[course.completed ? 'fas fa-check-circle text-teal-500' : 'far fa-circle text-gray-300']"></i>
                      <span :class="course.completed ? 'text-gray-600 line-through' : 'text-gray-700'">{{ course.title }}</span>
                    </div>
                    <div v-if="path.courses.length > 4" class="text-[10px] text-gray-400 ml-5">
                      +{{ path.courses.length - 4 }} more courses
                    </div>
                  </div>
                </div>

                <!-- Skills -->
                <div class="flex flex-wrap gap-1.5 mb-4">
                  <span v-for="skill in path.skills" :key="skill"
                        class="px-2 py-0.5 text-[10px] font-medium rounded-full"
                        :style="{ backgroundColor: path.color + '15', color: path.color }">
                    {{ skill }}
                  </span>
                </div>

                <!-- Footer -->
                <div class="flex items-center justify-between pt-3 border-t border-gray-100">
                  <div class="flex items-center gap-3 text-[11px] text-gray-500">
                    <span class="flex items-center gap-1"><i class="fas fa-clock text-[9px]"></i>{{ path.duration }}</span>
                    <span class="flex items-center gap-1"><i class="fas fa-star text-amber-400 text-[9px]"></i>{{ path.rating }}</span>
                  </div>
                  <button class="px-4 py-2 bg-gradient-to-r from-teal-500 to-teal-600 text-white rounded-lg text-xs font-semibold hover:from-teal-600 hover:to-teal-700 transition-all flex items-center gap-2 shadow-lg shadow-teal-200">
                    <i class="fas fa-play text-[10px]"></i>
                    Continue
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- All Learning Paths -->
        <div class="content-area p-6">
          <div class="flex items-center justify-between mb-5">
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-indigo-500 to-purple-600 flex items-center justify-center shadow-lg shadow-indigo-200">
                <i class="fas fa-route text-white text-sm"></i>
              </div>
              <div>
                <h2 class="text-lg font-bold text-gray-900">Explore Learning Paths</h2>
                <p class="text-xs text-gray-500">{{ filteredPaths.length }} structured learning journeys</p>
              </div>
            </div>
            <div class="flex items-center gap-2">
              <select v-model="pathFilter" class="text-xs px-3 py-2 rounded-lg border border-gray-200 bg-white text-gray-700 focus:outline-none focus:ring-2 focus:ring-teal-500 cursor-pointer">
                <option value="all">All Paths</option>
                <option value="enrolled">My Enrolled</option>
                <option value="available">Available</option>
              </select>
            </div>
          </div>

          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-5">
            <div v-for="path in filteredPaths" :key="path.id"
                 class="group bg-white rounded-2xl overflow-hidden border border-gray-100 shadow-sm hover:shadow-xl hover:border-teal-200 transition-all cursor-pointer">
              <!-- Path Image -->
              <div class="relative h-36 overflow-hidden">
                <img :src="path.image" :alt="path.title" class="w-full h-full object-cover transition-transform duration-500 group-hover:scale-110">
                <div class="absolute inset-0 bg-gradient-to-t from-black/80 via-black/40 to-transparent"></div>

                <!-- Badges -->
                <div class="absolute top-3 left-3 flex items-center gap-2">
                  <span :class="['course-level-badge', path.levelClass]">{{ path.level }}</span>
                  <span v-if="path.isEnrolled" class="px-2 py-0.5 bg-teal-500 text-white text-[9px] font-semibold rounded">ENROLLED</span>
                </div>

                <!-- Icon & Title Overlay -->
                <div class="absolute bottom-3 left-3 right-3">
                  <div class="flex items-center gap-3">
                    <div class="w-10 h-10 rounded-lg flex items-center justify-center shadow-lg" :style="{ backgroundColor: path.color }">
                      <i :class="[path.icon, 'text-white']"></i>
                    </div>
                    <h3 class="text-sm font-bold text-white flex-1">{{ path.title }}</h3>
                  </div>
                </div>
              </div>

              <!-- Path Content -->
              <div class="p-4">
                <p class="text-xs text-gray-600 mb-3 line-clamp-2">{{ path.description }}</p>

                <!-- Skills Tags -->
                <div class="flex flex-wrap gap-1.5 mb-3">
                  <span v-for="skill in path.skills.slice(0, 3)" :key="skill"
                        class="px-2 py-0.5 bg-gray-100 text-gray-600 text-[10px] font-medium rounded-full">
                    {{ skill }}
                  </span>
                  <span v-if="path.skills.length > 3" class="px-2 py-0.5 bg-gray-100 text-gray-500 text-[10px] rounded-full">
                    +{{ path.skills.length - 3 }}
                  </span>
                </div>

                <!-- Stats -->
                <div class="flex flex-wrap items-center gap-3 text-[11px] text-gray-500 mb-4">
                  <span class="flex items-center gap-1"><i class="fas fa-book text-[9px]"></i>{{ path.totalCourses }} courses</span>
                  <span class="flex items-center gap-1"><i class="fas fa-clock text-[9px]"></i>{{ path.duration }}</span>
                  <span class="flex items-center gap-1"><i class="fas fa-users text-[9px]"></i>{{ path.enrolled.toLocaleString() }}</span>
                  <span class="flex items-center gap-1"><i class="fas fa-star text-amber-400 text-[9px]"></i>{{ path.rating }}</span>
                </div>

                <!-- Progress or Enroll -->
                <div v-if="path.isEnrolled && path.progress > 0" class="mb-4">
                  <div class="flex items-center justify-between text-xs mb-1">
                    <span class="text-gray-600">Progress</span>
                    <span class="font-semibold text-teal-600">{{ path.progress }}%</span>
                  </div>
                  <div class="h-1.5 bg-gray-100 rounded-full overflow-hidden">
                    <div class="h-full bg-gradient-to-r from-teal-400 to-teal-500 rounded-full transition-all" :style="{ width: path.progress + '%' }"></div>
                  </div>
                </div>

                <!-- Action Button -->
                <button :class="[
                  'w-full px-4 py-2.5 rounded-xl text-xs font-semibold transition-all flex items-center justify-center gap-2',
                  path.isEnrolled
                    ? 'bg-gradient-to-r from-teal-500 to-teal-600 text-white hover:from-teal-600 hover:to-teal-700 shadow-lg shadow-teal-200'
                    : 'bg-white border-2 border-teal-500 text-teal-600 hover:bg-teal-50'
                ]">
                  <i :class="path.isEnrolled ? 'fas fa-play' : 'fas fa-plus'" class="text-[10px]"></i>
                  {{ path.isEnrolled ? (path.progress > 0 ? 'Continue Path' : 'Start Path') : 'Enroll in Path' }}
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Certificates Tab -->
      <div v-if="activeTab === 'certificates'" class="space-y-6">
        <!-- Certificate Stats -->
        <div class="grid grid-cols-2 md:grid-cols-4 gap-4">
          <div class="content-area p-4 text-center">
            <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-amber-400 to-orange-500 flex items-center justify-center mx-auto mb-2 shadow-lg shadow-amber-200">
              <i class="fas fa-award text-white text-sm"></i>
            </div>
            <p class="text-2xl font-bold text-gray-900">{{ certificateStats.total }}</p>
            <p class="text-xs text-gray-500">Total Certificates</p>
          </div>
          <div class="content-area p-4 text-center">
            <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-teal-400 to-teal-600 flex items-center justify-center mx-auto mb-2 shadow-lg shadow-teal-200">
              <i class="fas fa-calendar-check text-white text-sm"></i>
            </div>
            <p class="text-2xl font-bold text-gray-900">{{ certificateStats.thisYear }}</p>
            <p class="text-xs text-gray-500">Earned This Year</p>
          </div>
          <div class="content-area p-4 text-center">
            <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-blue-400 to-indigo-600 flex items-center justify-center mx-auto mb-2 shadow-lg shadow-blue-200">
              <i class="fas fa-clock text-white text-sm"></i>
            </div>
            <p class="text-2xl font-bold text-gray-900">{{ certificateStats.totalHours }}h</p>
            <p class="text-xs text-gray-500">Learning Hours</p>
          </div>
          <div class="content-area p-4 text-center">
            <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-purple-400 to-pink-600 flex items-center justify-center mx-auto mb-2 shadow-lg shadow-purple-200">
              <i class="fas fa-chart-line text-white text-sm"></i>
            </div>
            <p class="text-2xl font-bold text-gray-900">{{ certificateStats.avgScore }}%</p>
            <p class="text-xs text-gray-500">Average Score</p>
          </div>
        </div>

        <!-- Certificates Grid -->
        <div class="content-area p-6">
          <div class="flex items-center justify-between mb-6">
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-amber-400 to-orange-500 flex items-center justify-center shadow-lg shadow-amber-200">
                <i class="fas fa-certificate text-white text-sm"></i>
              </div>
              <div>
                <h2 class="text-lg font-bold text-gray-900">My Certificates</h2>
                <p class="text-xs text-gray-500">Showcase your achievements</p>
              </div>
            </div>
            <button class="px-4 py-2 bg-white border border-gray-200 text-gray-700 rounded-lg text-xs font-medium hover:bg-gray-50 transition-all flex items-center gap-2">
              <i class="fas fa-download text-[10px]"></i>
              Download All
            </button>
          </div>

          <div v-if="earnedCertificates.length > 0" class="grid grid-cols-[repeat(auto-fill,minmax(340px,1fr))] gap-5">
            <div v-for="cert in earnedCertificates" :key="cert.id"
                 class="group bg-white rounded-2xl overflow-hidden border border-gray-100 shadow-sm hover:shadow-xl hover:border-amber-200 transition-all">
              <!-- Certificate Visual -->
              <div class="relative h-40 overflow-hidden">
                <!-- Decorative Background -->
                <div class="absolute inset-0 bg-gradient-to-br from-amber-400 via-orange-400 to-amber-500"></div>
                <div class="absolute inset-0">
                  <div class="absolute top-0 right-0 w-40 h-40 bg-white/10 rounded-full -translate-y-1/2 translate-x-1/2"></div>
                  <div class="absolute bottom-0 left-0 w-32 h-32 bg-white/10 rounded-full translate-y-1/2 -translate-x-1/2"></div>
                  <div class="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-64 h-64 border border-white/10 rounded-full"></div>
                </div>

                <!-- Certificate Content -->
                <div class="absolute inset-0 flex flex-col items-center justify-center p-4">
                  <div class="w-14 h-14 rounded-full bg-white shadow-xl flex items-center justify-center mb-2">
                    <i :class="[cert.icon, 'text-xl']" :style="{ color: cert.color }"></i>
                  </div>
                  <p class="text-white/70 text-[9px] font-semibold uppercase tracking-widest mb-1">Certificate of Completion</p>
                  <h3 class="font-bold text-white text-center text-sm px-4">{{ cert.title }}</h3>
                </div>

                <!-- Score Badge -->
                <div class="absolute top-3 right-3 px-2 py-1 bg-white/90 rounded-lg shadow-lg">
                  <div class="flex items-center gap-1">
                    <i class="fas fa-star text-amber-500 text-[10px]"></i>
                    <span class="text-xs font-bold text-gray-800">{{ cert.score }}%</span>
                  </div>
                </div>

                <!-- Verified Badge -->
                <div class="absolute top-3 left-3 px-2 py-1 bg-teal-500 rounded-lg shadow-lg flex items-center gap-1">
                  <i class="fas fa-check-circle text-white text-[10px]"></i>
                  <span class="text-[10px] font-semibold text-white">Verified</span>
                </div>
              </div>

              <!-- Certificate Info -->
              <div class="p-4">
                <!-- Course & Instructor -->
                <div class="flex items-start justify-between mb-3">
                  <div>
                    <p class="text-xs text-gray-600 mb-0.5">{{ cert.course }}</p>
                    <p class="text-[11px] text-gray-400">Instructor: {{ cert.instructor }}</p>
                  </div>
                  <span class="px-2 py-0.5 bg-gray-100 text-gray-500 text-[10px] font-medium rounded">
                    {{ cert.hours }}h
                  </span>
                </div>

                <!-- Skills Earned -->
                <div class="flex flex-wrap gap-1.5 mb-3">
                  <span v-for="skill in cert.skills" :key="skill"
                        class="px-2 py-0.5 text-[10px] font-medium rounded-full"
                        :style="{ backgroundColor: cert.color + '15', color: cert.color }">
                    {{ skill }}
                  </span>
                </div>

                <!-- Metadata -->
                <div class="flex items-center gap-4 text-[11px] text-gray-400 mb-4 pt-3 border-t border-gray-100">
                  <span class="flex items-center gap-1">
                    <i class="fas fa-calendar text-[9px]"></i>
                    {{ cert.date }}
                  </span>
                  <span class="flex items-center gap-1">
                    <i class="fas fa-fingerprint text-[9px]"></i>
                    {{ cert.credentialId }}
                  </span>
                </div>

                <!-- Actions -->
                <div class="flex items-center gap-2">
                  <button class="flex-1 px-3 py-2.5 bg-gradient-to-r from-teal-500 to-teal-600 text-white rounded-xl text-xs font-semibold hover:from-teal-600 hover:to-teal-700 transition-all flex items-center justify-center gap-1.5 shadow-lg shadow-teal-200">
                    <i class="fas fa-eye text-[10px]"></i> View
                  </button>
                  <button class="flex-1 px-3 py-2.5 bg-white border border-gray-200 text-gray-700 rounded-xl text-xs font-semibold hover:bg-gray-50 transition-all flex items-center justify-center gap-1.5">
                    <i class="fas fa-download text-[10px]"></i> PDF
                  </button>
                  <button class="px-3 py-2.5 bg-[#0077b5] text-white rounded-xl text-xs font-semibold hover:bg-[#006699] transition-all flex items-center justify-center gap-1.5" title="Share on LinkedIn">
                    <i class="fab fa-linkedin text-sm"></i>
                  </button>
                  <button class="px-3 py-2.5 bg-white border border-gray-200 text-gray-700 rounded-xl text-xs font-semibold hover:bg-gray-50 transition-all" title="Copy Link">
                    <i class="fas fa-link text-[10px]"></i>
                  </button>
                </div>
              </div>
            </div>
          </div>

          <!-- Empty State -->
          <div v-else class="text-center py-16">
            <div class="w-20 h-20 rounded-full bg-gradient-to-br from-amber-100 to-orange-100 flex items-center justify-center mx-auto mb-4">
              <i class="fas fa-certificate text-amber-400 text-3xl"></i>
            </div>
            <h3 class="font-semibold text-gray-900 mb-2">No certificates yet</h3>
            <p class="text-gray-500 text-sm mb-6 max-w-md mx-auto">Complete courses to earn certificates and showcase your achievements to employers</p>
            <button @click="activeTab = 'catalog'" class="px-6 py-2.5 bg-gradient-to-r from-teal-500 to-teal-600 text-white rounded-xl font-semibold text-sm hover:from-teal-600 hover:to-teal-700 transition-all shadow-lg shadow-teal-200 flex items-center gap-2 mx-auto">
              <i class="fas fa-compass"></i>
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
