<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()

// Loading state
const isLoading = ref(false)

// Continue Learning carousel ref
const continueLearningRef = ref<HTMLElement | null>(null)

// Featured course carousel
const currentFeaturedCourseIndex = ref(0)
const featuredCourseInterval = ref<number | null>(null)

// Featured course from all enrolled courses
const featuredCourse = computed(() =>
  enrolledCourses.value[currentFeaturedCourseIndex.value] || enrolledCourses.value[0]
)

// Other courses (excluding the currently featured one)
const otherCourses = computed(() =>
  enrolledCourses.value.filter((_, idx) => idx !== currentFeaturedCourseIndex.value).slice(0, 4)
)

// Keep inProgressCourses for stats
const inProgressCourses = computed(() =>
  enrolledCourses.value.filter(c => c.progress > 0 && c.progress < 100)
)

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
    tags: ['Data Science', 'Analytics'],
    description: 'Master advanced data analytics techniques including statistical modeling, predictive analytics, and business intelligence. Learn to transform raw data into actionable insights.',
    saved: false,
    syllabus: [
      { id: 1, title: 'Introduction to Data Analytics', duration: '30 min', completed: true },
      { id: 2, title: 'Data Collection Methods', duration: '45 min', completed: true },
      { id: 3, title: 'Statistical Analysis Basics', duration: '50 min', completed: true },
      { id: 4, title: 'Data Visualization Techniques', duration: '40 min', completed: true },
      { id: 5, title: 'Advanced Statistical Models', duration: '55 min', completed: true },
      { id: 6, title: 'Predictive Analytics', duration: '45 min', completed: true },
      { id: 7, title: 'Machine Learning Integration', duration: '50 min', completed: true },
      { id: 8, title: 'Real-time Data Processing', duration: '40 min', completed: true },
      { id: 9, title: 'Data Pipeline Architecture', duration: '45 min', completed: true },
      { id: 10, title: 'Business Intelligence Tools', duration: '35 min', completed: false, current: true },
      { id: 11, title: 'Dashboard Development', duration: '40 min', completed: false },
      { id: 12, title: 'Final Project & Assessment', duration: '60 min', completed: false }
    ]
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
    tags: ['Leadership', 'Management'],
    description: 'Develop essential leadership skills to inspire and guide teams effectively. Learn communication strategies, conflict resolution, and how to drive organizational success.',
    saved: true,
    syllabus: [
      { id: 1, title: 'What Makes a Great Leader', duration: '25 min', completed: true },
      { id: 2, title: 'Communication Strategies', duration: '35 min', completed: true },
      { id: 3, title: 'Building Trust & Credibility', duration: '30 min', completed: true },
      { id: 4, title: 'Delegation & Empowerment', duration: '40 min', completed: true },
      { id: 5, title: 'Conflict Resolution', duration: '35 min', completed: true },
      { id: 6, title: 'Motivating Your Team', duration: '30 min', completed: false, current: true },
      { id: 7, title: 'Decision Making Under Pressure', duration: '40 min', completed: false },
      { id: 8, title: 'Leading Remote Teams', duration: '35 min', completed: false },
      { id: 9, title: 'Change Management', duration: '45 min', completed: false },
      { id: 10, title: 'Coaching & Mentoring', duration: '30 min', completed: false },
      { id: 11, title: 'Leadership Assessment', duration: '40 min', completed: false }
    ]
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
    tags: ['Security', 'IT'],
    description: 'Build a solid foundation in cybersecurity principles. Understand common threats, learn protective measures, and develop skills to safeguard digital assets and information.',
    saved: false,
    syllabus: [
      { id: 1, title: 'Introduction to Cybersecurity', duration: '30 min', completed: true },
      { id: 2, title: 'Common Threats & Vulnerabilities', duration: '35 min', completed: true },
      { id: 3, title: 'Network Security Basics', duration: '40 min', completed: true },
      { id: 4, title: 'Password & Authentication', duration: '25 min', completed: true },
      { id: 5, title: 'Data Encryption', duration: '35 min', completed: true },
      { id: 6, title: 'Phishing & Social Engineering', duration: '30 min', completed: true },
      { id: 7, title: 'Security Best Practices', duration: '25 min', completed: true },
      { id: 8, title: 'Final Assessment', duration: '40 min', completed: true }
    ]
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
    tags: ['Soft Skills', 'Communication'],
    description: 'Enhance your communication abilities for professional success. Master verbal and written communication, presentations, and interpersonal skills for effective workplace interactions.',
    saved: false,
    syllabus: [
      { id: 1, title: 'The Art of Communication', duration: '30 min', completed: false },
      { id: 2, title: 'Active Listening Skills', duration: '25 min', completed: false },
      { id: 3, title: 'Non-Verbal Communication', duration: '35 min', completed: false },
      { id: 4, title: 'Written Communication', duration: '40 min', completed: false },
      { id: 5, title: 'Presentation Skills', duration: '45 min', completed: false },
      { id: 6, title: 'Handling Difficult Conversations', duration: '30 min', completed: false },
      { id: 7, title: 'Cross-Cultural Communication', duration: '35 min', completed: false },
      { id: 8, title: 'Persuasion Techniques', duration: '30 min', completed: false },
      { id: 9, title: 'Feedback & Criticism', duration: '25 min', completed: false },
      { id: 10, title: 'Final Project', duration: '40 min', completed: false }
    ]
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
    tags: ['Project Management', 'Agile'],
    description: 'Become a proficient project manager with comprehensive training in Agile, Scrum, and traditional methodologies. Learn to plan, execute, and deliver successful projects on time and budget.',
    saved: false,
    syllabus: [
      { id: 1, title: 'Project Management Overview', duration: '30 min', completed: true },
      { id: 2, title: 'Project Lifecycle Phases', duration: '40 min', completed: true },
      { id: 3, title: 'Stakeholder Management', duration: '35 min', completed: true },
      { id: 4, title: 'Scope Definition', duration: '45 min', completed: true },
      { id: 5, title: 'Work Breakdown Structure', duration: '40 min', completed: false, current: true },
      { id: 6, title: 'Schedule Development', duration: '50 min', completed: false },
      { id: 7, title: 'Resource Planning', duration: '35 min', completed: false },
      { id: 8, title: 'Budget Management', duration: '45 min', completed: false },
      { id: 9, title: 'Risk Assessment', duration: '40 min', completed: false },
      { id: 10, title: 'Quality Management', duration: '35 min', completed: false },
      { id: 11, title: 'Agile Methodologies', duration: '50 min', completed: false },
      { id: 12, title: 'Scrum Framework', duration: '45 min', completed: false },
      { id: 13, title: 'Kanban Principles', duration: '30 min', completed: false },
      { id: 14, title: 'Team Collaboration Tools', duration: '35 min', completed: false },
      { id: 15, title: 'Capstone Project', duration: '60 min', completed: false }
    ]
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
  {
    id: 10,
    title: 'Machine Learning Basics',
    instructor: 'Dr. Sarah Mitchell',
    instructorInitials: 'SM',
    duration: '4 hours',
    totalLessons: 12,
    level: 'Intermediate',
    levelClass: 'intermediate',
    matchScore: 95,
    rating: 4.8,
    students: 3200,
    image: 'https://images.unsplash.com/photo-1677442136019-21780ecad995?w=400&h=300&fit=crop',
    tags: ['AI', 'Machine Learning'],
    saved: false
  },
  {
    id: 11,
    title: 'Strategic Planning',
    instructor: 'Michael Brown',
    instructorInitials: 'MB',
    duration: '3 hours',
    totalLessons: 8,
    level: 'Advanced',
    levelClass: 'advanced',
    matchScore: 88,
    rating: 4.6,
    students: 1890,
    image: 'https://images.unsplash.com/photo-1454165804606-c3d57bc86b40?w=400&h=300&fit=crop',
    tags: ['Strategy', 'Business'],
    saved: false
  },
  {
    id: 12,
    title: 'Public Speaking Mastery',
    instructor: 'Emma Wilson',
    instructorInitials: 'EW',
    duration: '2 hours',
    totalLessons: 6,
    level: 'Beginner',
    levelClass: 'beginner',
    matchScore: 82,
    rating: 4.7,
    students: 4500,
    image: 'https://images.unsplash.com/photo-1475721027785-f74eccf877e2?w=400&h=300&fit=crop',
    tags: ['Speaking', 'Communication'],
    saved: true
  },
  {
    id: 13,
    title: 'Cloud Architecture Fundamentals',
    instructor: 'David Kim',
    instructorInitials: 'DK',
    duration: '8 hours',
    totalLessons: 20,
    level: 'Advanced',
    levelClass: 'advanced',
    matchScore: 78,
    rating: 4.9,
    students: 2100,
    image: 'https://images.unsplash.com/photo-1451187580459-43490279c0fa?w=400&h=300&fit=crop',
    tags: ['Cloud', 'AWS'],
    saved: false
  }
])

// Catalog courses (All Courses)
const allCourses = ref([
  { id: 10, title: 'Machine Learning Basics', instructor: 'Dr. Sarah Mitchell', instructorInitials: 'SM', level: 'Intermediate', levelClass: 'intermediate', duration: '4 hours', lessons: 12, category: 'Technology', rating: 4.8, students: 3200, image: 'https://images.unsplash.com/photo-1677442136019-21780ecad995?w=400&h=300&fit=crop', tags: ['AI', 'ML'], saved: false, isNew: true },
  { id: 11, title: 'Strategic Planning', instructor: 'Michael Brown', instructorInitials: 'MB', level: 'Advanced', levelClass: 'advanced', duration: '3 hours', lessons: 8, category: 'Business', rating: 4.6, students: 1890, image: 'https://images.unsplash.com/photo-1454165804606-c3d57bc86b40?w=400&h=300&fit=crop', tags: ['Strategy', 'Planning'], saved: false, isNew: false },
  { id: 12, title: 'Public Speaking Mastery', instructor: 'Emma Wilson', instructorInitials: 'EW', level: 'Beginner', levelClass: 'beginner', duration: '2 hours', lessons: 6, category: 'Soft Skills', rating: 4.7, students: 4500, image: 'https://images.unsplash.com/photo-1475721027785-f74eccf877e2?w=400&h=300&fit=crop', tags: ['Speaking', 'Communication'], saved: true, isNew: false },
  { id: 13, title: 'Cloud Architecture Fundamentals', instructor: 'David Kim', instructorInitials: 'DK', level: 'Advanced', levelClass: 'advanced', duration: '8 hours', lessons: 20, category: 'Technology', rating: 4.9, students: 2100, image: 'https://images.unsplash.com/photo-1451187580459-43490279c0fa?w=400&h=300&fit=crop', tags: ['Cloud', 'AWS'], saved: false, isNew: true },
  { id: 14, title: 'UX Design Principles', instructor: 'Lisa Park', instructorInitials: 'LP', level: 'Intermediate', levelClass: 'intermediate', duration: '5 hours', lessons: 14, category: 'Design', rating: 4.8, students: 2890, image: 'https://images.unsplash.com/photo-1561070791-2526d30994b5?w=400&h=300&fit=crop', tags: ['Design', 'UX'], saved: false, isNew: false },
  { id: 15, title: 'Financial Analysis', instructor: 'Robert Chen', instructorInitials: 'RC', level: 'Intermediate', levelClass: 'intermediate', duration: '6 hours', lessons: 16, category: 'Finance', rating: 4.5, students: 1670, image: 'https://images.unsplash.com/photo-1554224155-6726b3ff858f?w=400&h=300&fit=crop', tags: ['Finance', 'Analysis'], saved: false, isNew: false },
  { id: 16, title: 'Agile Project Management', instructor: 'Jennifer Adams', instructorInitials: 'JA', level: 'Intermediate', levelClass: 'intermediate', duration: '4 hours', lessons: 10, category: 'Business', rating: 4.7, students: 3450, image: 'https://images.unsplash.com/photo-1552664730-d307ca884978?w=400&h=300&fit=crop', tags: ['Agile', 'Scrum'], saved: false, isNew: true },
  { id: 17, title: 'Python for Data Science', instructor: 'Dr. James Wilson', instructorInitials: 'JW', level: 'Beginner', levelClass: 'beginner', duration: '6 hours', lessons: 18, category: 'Technology', rating: 4.9, students: 5200, image: 'https://images.unsplash.com/photo-1526379095098-d400fd0bf935?w=400&h=300&fit=crop', tags: ['Python', 'Data Science'], saved: true, isNew: false },
  { id: 18, title: 'Digital Marketing Essentials', instructor: 'Sophia Martinez', instructorInitials: 'SM', level: 'Beginner', levelClass: 'beginner', duration: '3 hours', lessons: 8, category: 'Marketing', rating: 4.6, students: 2780, image: 'https://images.unsplash.com/photo-1460925895917-afdab827c52f?w=400&h=300&fit=crop', tags: ['Marketing', 'Digital'], saved: false, isNew: false },
])

// All Courses section state
const allCoursesSearch = ref('')
const allCoursesLevelFilter = ref<string[]>([])
const allCoursesCategoryFilter = ref<string[]>([])
const allCoursesViewMode = ref<'grid' | 'list'>('grid')
const showLevelFilter = ref(false)
const showCategoryFilterDropdown = ref(false)
const allCoursesSortBy = ref('popular')
const showAllCoursesSortDropdown = ref(false)

const courseLevelOptions = [
  { id: 'beginner', label: 'Beginner', color: 'text-green-500' },
  { id: 'intermediate', label: 'Intermediate', color: 'text-blue-500' },
  { id: 'advanced', label: 'Advanced', color: 'text-purple-500' }
]

const courseCategoryOptions = ['Technology', 'Business', 'Soft Skills', 'Design', 'Finance', 'Marketing']

const allCoursesSortOptions = [
  { value: 'popular', label: 'Most Popular', icon: 'fas fa-fire' },
  { value: 'rating', label: 'Highest Rated', icon: 'fas fa-star' },
  { value: 'newest', label: 'Newest', icon: 'fas fa-clock' },
  { value: 'title', label: 'Title A-Z', icon: 'fas fa-sort-alpha-down' }
]

const filteredAllCourses = computed(() => {
  let courses = [...allCourses.value]

  // Search filter
  if (allCoursesSearch.value) {
    const search = allCoursesSearch.value.toLowerCase()
    courses = courses.filter(c =>
      c.title.toLowerCase().includes(search) ||
      c.instructor.toLowerCase().includes(search) ||
      c.tags.some(t => t.toLowerCase().includes(search))
    )
  }

  // Level filter
  if (allCoursesLevelFilter.value.length > 0) {
    courses = courses.filter(c => allCoursesLevelFilter.value.includes(c.levelClass))
  }

  // Category filter
  if (allCoursesCategoryFilter.value.length > 0) {
    courses = courses.filter(c => allCoursesCategoryFilter.value.includes(c.category))
  }

  // Sort
  switch (allCoursesSortBy.value) {
    case 'popular':
      courses.sort((a, b) => b.students - a.students)
      break
    case 'rating':
      courses.sort((a, b) => b.rating - a.rating)
      break
    case 'newest':
      courses.sort((a, b) => (b.isNew ? 1 : 0) - (a.isNew ? 1 : 0))
      break
    case 'title':
      courses.sort((a, b) => a.title.localeCompare(b.title))
      break
  }

  return courses
})

function toggleLevelFilterOption(level: string) {
  const index = allCoursesLevelFilter.value.indexOf(level)
  if (index === -1) {
    allCoursesLevelFilter.value.push(level)
  } else {
    allCoursesLevelFilter.value.splice(index, 1)
  }
}

function toggleCategoryFilterOption(category: string) {
  const index = allCoursesCategoryFilter.value.indexOf(category)
  if (index === -1) {
    allCoursesCategoryFilter.value.push(category)
  } else {
    allCoursesCategoryFilter.value.splice(index, 1)
  }
}

function toggleSaveAllCourse(courseId: number) {
  const course = allCourses.value.find(c => c.id === courseId)
  if (course) {
    course.saved = !course.saved
  }
}

// Catalog courses (keep for backward compatibility)
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

function toggleSaveCourse(courseId: number) {
  const course = enrolledCourses.value.find(c => c.id === courseId)
  if (course) {
    course.saved = !course.saved
  }
}

function toggleSaveRecommended(courseId: number) {
  const course = recommendedCourses.value.find(c => c.id === courseId)
  if (course) {
    course.saved = !course.saved
  }
}

function getLevelColor(level: string) {
  switch (level.toLowerCase()) {
    case 'beginner': return '#10b981'
    case 'intermediate': return '#3b82f6'
    case 'advanced': return '#8b5cf6'
    default: return '#6b7280'
  }
}

// Scroll Continue Learning carousel
function scrollContinueLearning(direction: 'left' | 'right') {
  if (continueLearningRef.value) {
    const scrollAmount = 320
    const currentScroll = continueLearningRef.value.scrollLeft
    continueLearningRef.value.scrollTo({
      left: direction === 'left' ? currentScroll - scrollAmount : currentScroll + scrollAmount,
      behavior: 'smooth'
    })
  }
}

// Featured course carousel functions
function nextFeaturedCourse() {
  currentFeaturedCourseIndex.value = (currentFeaturedCourseIndex.value + 1) % enrolledCourses.value.length
}

function prevFeaturedCourse() {
  currentFeaturedCourseIndex.value = currentFeaturedCourseIndex.value === 0
    ? enrolledCourses.value.length - 1
    : currentFeaturedCourseIndex.value - 1
}

function goToFeaturedCourse(index: number) {
  currentFeaturedCourseIndex.value = index
}

function startFeaturedAutoPlay() {
  if (featuredCourseInterval.value) return
  featuredCourseInterval.value = window.setInterval(() => {
    nextFeaturedCourse()
  }, 5000)
}

function pauseFeaturedAutoPlay() {
  if (featuredCourseInterval.value) {
    clearInterval(featuredCourseInterval.value)
    featuredCourseInterval.value = null
  }
}

function resumeFeaturedAutoPlay() {
  startFeaturedAutoPlay()
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
        <!-- Featured Courses Section -->
        <div v-if="enrolledCourses.length > 0" class="featured-courses-section">
          <div class="featured-courses-header">
            <div class="flex items-center gap-3">
              <div class="featured-courses-icon">
                <i class="fas fa-star"></i>
              </div>
              <div>
                <h2 class="featured-courses-title">Featured Courses</h2>
                <p class="featured-courses-subtitle">Explore our top recommended courses</p>
              </div>
            </div>
            <div class="featured-courses-nav">
              <span class="featured-courses-badge">
                <i class="fas fa-graduation-cap"></i>{{ enrolledCourses.length }} Courses
              </span>
            </div>
          </div>

          <!-- Featured Grid: Main + Up Next -->
          <div class="featured-course-grid">
            <!-- Main Featured Card with Carousel -->
            <div class="featured-course-wrapper" @mouseenter="pauseFeaturedAutoPlay" @mouseleave="resumeFeaturedAutoPlay">
              <div class="featured-course-card group" @click="navigateToCourse(featuredCourse.id)">
                <transition name="carousel-fade" mode="out-in">
                  <img :key="featuredCourse.id" :src="featuredCourse.image" :alt="featuredCourse.title" class="featured-course-img" />
                </transition>
                <div class="featured-course-overlay"></div>

                <!-- Badges -->
                <div class="featured-course-badges">
                  <span class="badge-featured"><i class="fas fa-star"></i> Featured</span>
                  <span :class="['badge-level', featuredCourse.levelClass]">{{ featuredCourse.level }}</span>
                </div>

                <!-- Save Button -->
                <button
                  class="featured-save-btn"
                  :class="{ 'saved': featuredCourse.saved }"
                  @click.stop="toggleSaveCourse(featuredCourse.id)"
                >
                  <i :class="featuredCourse.saved ? 'fas fa-bookmark' : 'far fa-bookmark'"></i>
                </button>

                <!-- Play Button -->
                <div class="featured-course-play">
                  <i class="fas fa-play"></i>
                </div>

                <!-- Content -->
                <div class="featured-course-content">
                  <!-- Tags -->
                  <div class="featured-course-tags">
                    <span v-for="tag in featuredCourse.tags" :key="tag" class="featured-tag">{{ tag }}</span>
                  </div>

                  <h3 class="featured-course-title">{{ featuredCourse.title }}</h3>

                  <!-- Description -->
                  <p class="featured-course-description">{{ featuredCourse.description }}</p>

                  <!-- Stats Row -->
                  <div class="featured-course-stats">
                    <div class="featured-stat">
                      <i class="fas fa-clock"></i>
                      <span>{{ featuredCourse.duration }}</span>
                    </div>
                    <div class="featured-stat">
                      <i class="fas fa-play-circle"></i>
                      <span>{{ featuredCourse.totalLessons }} lessons</span>
                    </div>
                    <div class="featured-stat">
                      <i class="fas fa-star text-amber-400"></i>
                      <span>{{ featuredCourse.rating }} rating</span>
                    </div>
                    <div class="featured-stat">
                      <i class="fas fa-users"></i>
                      <span>{{ featuredCourse.students?.toLocaleString() }} students</span>
                    </div>
                  </div>

                  <!-- Instructor & Action -->
                  <div class="featured-course-footer">
                    <div class="featured-course-author">
                      <div class="author-avatar">{{ featuredCourse.instructorInitials }}</div>
                      <div class="author-info">
                        <span class="author-name">{{ featuredCourse.instructor }}</span>
                        <span class="author-role">Instructor</span>
                      </div>
                    </div>
                    <button class="featured-enroll-btn" @click.stop="navigateToCourse(featuredCourse.id)">
                      <i class="fas fa-play"></i> Start Learning
                    </button>
                  </div>
                </div>

                <!-- Carousel Navigation -->
                <button v-if="enrolledCourses.length > 1" class="course-carousel-arrow course-carousel-prev" @click.stop="prevFeaturedCourse">
                  <i class="fas fa-chevron-left"></i>
                </button>
                <button v-if="enrolledCourses.length > 1" class="course-carousel-arrow course-carousel-next" @click.stop="nextFeaturedCourse">
                  <i class="fas fa-chevron-right"></i>
                </button>

                <!-- Carousel Dots -->
                <div v-if="enrolledCourses.length > 1" class="course-carousel-dots">
                  <button
                    v-for="(course, index) in enrolledCourses"
                    :key="course.id"
                    :class="['course-carousel-dot', { active: index === currentFeaturedCourseIndex }]"
                    @click.stop="goToFeaturedCourse(index)"
                  ></button>
                </div>
              </div>
            </div>

            <!-- Up Next Column -->
            <div class="up-next-courses">
              <div class="up-next-header">
                <h3 class="up-next-title"><i class="fas fa-th-list"></i> More Courses</h3>
                <span class="up-next-count">{{ otherCourses.length }} courses</span>
              </div>
              <div class="up-next-list">
                <div v-for="(course, index) in otherCourses" :key="course.id"
                     @click="goToFeaturedCourse(enrolledCourses.findIndex(c => c.id === course.id))"
                     class="up-next-course-card group">
                  <!-- Thumbnail -->
                  <div class="up-next-thumb">
                    <img :src="course.image" :alt="course.title" class="up-next-img" />
                    <div class="up-next-overlay"></div>
                    <div class="up-next-play"><i class="fas fa-play"></i></div>
                    <span class="up-next-duration">{{ course.duration }}</span>
                  </div>
                  <!-- Save Button on Card Edge -->
                  <button
                    class="up-next-save-btn"
                    :class="{ 'saved': course.saved }"
                    @click.stop="toggleSaveCourse(course.id)"
                  >
                    <i :class="course.saved ? 'fas fa-bookmark' : 'far fa-bookmark'"></i>
                  </button>

                  <!-- Info -->
                  <div class="up-next-info">
                    <div class="up-next-badges">
                      <span :class="['up-next-level', course.levelClass]">{{ course.level }}</span>
                    </div>
                    <h4 class="up-next-course-title">{{ course.title }}</h4>
                    <div class="up-next-instructor">
                      <div class="up-next-avatar">{{ course.instructorInitials }}</div>
                      <span>{{ course.instructor }}</span>
                    </div>
                    <div class="up-next-footer">
                      <div class="up-next-meta">
                        <span><i class="fas fa-play-circle"></i> {{ course.totalLessons }} lessons</span>
                        <span><i class="fas fa-star text-amber-400"></i> {{ course.rating }}</span>
                      </div>
                      <button class="up-next-view-btn" @click.stop="goToFeaturedCourse(enrolledCourses.findIndex(c => c.id === course.id))">
                        <i class="fas fa-eye"></i> View
                      </button>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Recommended For You Section -->
        <div class="recommended-section">
          <div class="recommended-header">
            <div class="recommended-title-wrap">
              <div class="recommended-icon">
                <i class="fas fa-sparkles"></i>
              </div>
              <div>
                <h2 class="recommended-title">Recommended for You</h2>
                <p class="recommended-subtitle">Based on your learning history and interests</p>
              </div>
            </div>
            <button class="recommended-see-all">
              See All <i class="fas fa-arrow-right"></i>
            </button>
          </div>
          <div class="recommended-grid">
            <div v-for="course in recommendedCourses" :key="course.id" class="recommended-card">
              <!-- Save Button -->
              <button
                class="recommended-save-btn"
                :class="{ 'saved': course.saved }"
                @click.stop="toggleSaveRecommended(course.id)"
              >
                <i :class="course.saved ? 'fas fa-bookmark' : 'far fa-bookmark'"></i>
              </button>
              <!-- Match Badge -->
              <div class="recommended-match">
                <i class="fas fa-bullseye"></i> {{ course.matchScore }}% Match
              </div>
              <!-- Image -->
              <div class="recommended-image">
                <img :src="course.image" :alt="course.title" />
                <div class="recommended-overlay"></div>
                <span class="recommended-duration">{{ course.duration }}</span>
              </div>
              <!-- Content -->
              <div class="recommended-content">
                <div class="recommended-tags">
                  <span v-for="tag in course.tags" :key="tag" class="recommended-tag">{{ tag }}</span>
                  <span :class="['recommended-level', course.levelClass]">{{ course.level }}</span>
                </div>
                <h3 class="recommended-course-title">{{ course.title }}</h3>
                <div class="recommended-instructor">
                  <div class="recommended-avatar">{{ course.instructorInitials }}</div>
                  <span>{{ course.instructor }}</span>
                </div>
                <div class="recommended-footer">
                  <div class="recommended-stats">
                    <span><i class="fas fa-play-circle"></i> {{ course.totalLessons }}</span>
                    <span><i class="fas fa-star text-amber-400"></i> {{ course.rating }}</span>
                    <span><i class="fas fa-users"></i> {{ (course.students / 1000).toFixed(1) }}k</span>
                  </div>
                  <button class="recommended-enroll-btn">
                    <i class="fas fa-plus"></i> Enroll
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- All Courses Section -->
        <div class="all-courses-wrapper bg-white rounded-2xl shadow-sm border border-gray-100 overflow-hidden">
          <!-- Section Header / Toolbar -->
          <div class="border-b border-gray-100">
            <!-- Top Row - Title and Primary Actions -->
            <div class="px-4 py-3 flex items-center justify-between">
              <h2 class="text-lg font-bold text-gray-900 flex items-center gap-3">
                <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-indigo-500 to-indigo-600 flex items-center justify-center shadow-lg shadow-indigo-200">
                  <i class="fas fa-graduation-cap text-white text-sm"></i>
                </div>
                <div>
                  <span class="block">All Courses</span>
                  <span class="text-xs font-medium text-gray-500">{{ filteredAllCourses.length }} courses available</span>
                </div>
              </h2>
              <div class="flex items-center gap-2">
                <button class="px-4 py-2 bg-gradient-to-r from-teal-500 to-teal-600 text-white rounded-lg text-sm font-medium hover:from-teal-600 hover:to-teal-700 transition-all flex items-center gap-2 shadow-sm shadow-teal-200">
                  <i class="fas fa-filter"></i>
                  Browse Categories
                </button>
              </div>
            </div>

            <!-- Bottom Row - Search, Filters, View Options -->
            <div class="px-4 py-2 bg-gray-50/50 flex flex-wrap items-center gap-3">
              <!-- Search -->
              <div class="min-w-[200px] max-w-md relative">
                <i class="fas fa-search absolute left-3 top-1/2 -translate-y-1/2 text-gray-400 text-sm"></i>
                <input
                  v-model="allCoursesSearch"
                  type="text"
                  placeholder="Search courses..."
                  class="w-full pl-9 pr-4 py-2 bg-white border border-gray-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-teal-500 focus:border-transparent transition-all"
                >
                <button v-if="allCoursesSearch" @click="allCoursesSearch = ''" class="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600">
                  <i class="fas fa-times text-xs"></i>
                </button>
              </div>

              <!-- Level Filter Dropdown -->
              <div class="relative">
                <button
                  @click="showLevelFilter = !showLevelFilter"
                  :class="[
                    'flex items-center gap-2 px-3 py-1.5 rounded-lg text-xs font-medium transition-all border',
                    allCoursesLevelFilter.length > 0 ? 'bg-teal-50 border-teal-200 text-teal-700' : 'bg-white border-gray-200 text-gray-600 hover:bg-gray-50'
                  ]"
                >
                  <i class="fas fa-layer-group text-sm"></i>
                  <span>{{ allCoursesLevelFilter.length > 0 ? `${allCoursesLevelFilter.length} Levels` : 'Level' }}</span>
                  <i :class="showLevelFilter ? 'fas fa-chevron-up' : 'fas fa-chevron-down'" class="text-[10px] ml-1"></i>
                </button>

                <!-- Dropdown Menu -->
                <div
                  v-if="showLevelFilter"
                  class="absolute left-0 top-full mt-2 w-56 bg-white rounded-xl shadow-lg border border-gray-100 py-2 z-50"
                >
                  <div class="px-3 py-1.5 text-xs font-semibold text-gray-400 uppercase tracking-wider">Select Levels</div>
                  <div class="max-h-48 overflow-y-auto">
                    <button
                      v-for="level in courseLevelOptions"
                      :key="level.id"
                      @click="toggleLevelFilterOption(level.id)"
                      :class="[
                        'w-full px-3 py-2 text-left text-sm flex items-center gap-3 transition-colors',
                        allCoursesLevelFilter.includes(level.id) ? 'bg-teal-50 text-teal-700' : 'text-gray-700 hover:bg-gray-50'
                      ]"
                    >
                      <div :class="[
                        'w-4 h-4 rounded border-2 flex items-center justify-center transition-all',
                        allCoursesLevelFilter.includes(level.id) ? 'bg-teal-500 border-teal-500' : 'border-gray-300'
                      ]">
                        <i v-if="allCoursesLevelFilter.includes(level.id)" class="fas fa-check text-white text-[8px]"></i>
                      </div>
                      <span class="flex-1">{{ level.label }}</span>
                    </button>
                  </div>
                  <div class="my-2 border-t border-gray-100"></div>
                  <div class="px-3 flex gap-2">
                    <button
                      @click="allCoursesLevelFilter = []; showLevelFilter = false"
                      class="flex-1 px-3 py-1.5 text-xs font-medium text-gray-600 bg-gray-100 rounded-lg hover:bg-gray-200 transition-colors"
                    >
                      Clear All
                    </button>
                    <button
                      @click="showLevelFilter = false"
                      class="flex-1 px-3 py-1.5 text-xs font-medium text-white bg-teal-500 rounded-lg hover:bg-teal-600 transition-colors"
                    >
                      Apply
                    </button>
                  </div>
                </div>
                <div v-if="showLevelFilter" @click="showLevelFilter = false" class="fixed inset-0 z-40"></div>
              </div>

              <!-- Category Filter Dropdown -->
              <div class="relative">
                <button
                  @click="showCategoryFilterDropdown = !showCategoryFilterDropdown"
                  :class="[
                    'flex items-center gap-2 px-3 py-1.5 rounded-lg text-xs font-medium transition-all border',
                    allCoursesCategoryFilter.length > 0 ? 'bg-teal-50 border-teal-200 text-teal-700' : 'bg-white border-gray-200 text-gray-600 hover:bg-gray-50'
                  ]"
                >
                  <i class="fas fa-folder text-sm"></i>
                  <span>{{ allCoursesCategoryFilter.length > 0 ? `${allCoursesCategoryFilter.length} Categories` : 'Category' }}</span>
                  <i :class="showCategoryFilterDropdown ? 'fas fa-chevron-up' : 'fas fa-chevron-down'" class="text-[10px] ml-1"></i>
                </button>

                <!-- Dropdown Menu -->
                <div
                  v-if="showCategoryFilterDropdown"
                  class="absolute left-0 top-full mt-2 w-64 bg-white rounded-xl shadow-lg border border-gray-100 py-2 z-50"
                >
                  <div class="px-3 py-1.5 text-xs font-semibold text-gray-400 uppercase tracking-wider">Select Categories</div>
                  <div class="max-h-48 overflow-y-auto">
                    <button
                      v-for="cat in courseCategoryOptions"
                      :key="cat"
                      @click="toggleCategoryFilterOption(cat)"
                      :class="[
                        'w-full px-3 py-2 text-left text-sm flex items-center gap-3 transition-colors',
                        allCoursesCategoryFilter.includes(cat) ? 'bg-teal-50 text-teal-700' : 'text-gray-700 hover:bg-gray-50'
                      ]"
                    >
                      <div :class="[
                        'w-4 h-4 rounded border-2 flex items-center justify-center transition-all',
                        allCoursesCategoryFilter.includes(cat) ? 'bg-teal-500 border-teal-500' : 'border-gray-300'
                      ]">
                        <i v-if="allCoursesCategoryFilter.includes(cat)" class="fas fa-check text-white text-[8px]"></i>
                      </div>
                      <span class="flex-1">{{ cat }}</span>
                    </button>
                  </div>
                  <div class="my-2 border-t border-gray-100"></div>
                  <div class="px-3 flex gap-2">
                    <button
                      @click="allCoursesCategoryFilter = []; showCategoryFilterDropdown = false"
                      class="flex-1 px-3 py-1.5 text-xs font-medium text-gray-600 bg-gray-100 rounded-lg hover:bg-gray-200 transition-colors"
                    >
                      Clear All
                    </button>
                    <button
                      @click="showCategoryFilterDropdown = false"
                      class="flex-1 px-3 py-1.5 text-xs font-medium text-white bg-teal-500 rounded-lg hover:bg-teal-600 transition-colors"
                    >
                      Apply
                    </button>
                  </div>
                </div>
                <div v-if="showCategoryFilterDropdown" @click="showCategoryFilterDropdown = false" class="fixed inset-0 z-40"></div>
              </div>

              <!-- Sort Options -->
              <div class="relative">
                <button
                  @click="showAllCoursesSortDropdown = !showAllCoursesSortDropdown"
                  class="flex items-center gap-2 px-3 py-1.5 rounded-lg text-xs font-medium transition-all border bg-white border-gray-200 text-gray-600 hover:bg-gray-50"
                >
                  <i :class="[allCoursesSortOptions.find(o => o.value === allCoursesSortBy)?.icon, 'text-sm text-teal-500']"></i>
                  <span>{{ allCoursesSortOptions.find(o => o.value === allCoursesSortBy)?.label }}</span>
                  <i :class="showAllCoursesSortDropdown ? 'fas fa-chevron-up' : 'fas fa-chevron-down'" class="text-[10px] ml-1"></i>
                </button>

                <!-- Dropdown Menu -->
                <div
                  v-if="showAllCoursesSortDropdown"
                  class="absolute left-0 top-full mt-2 w-48 bg-white rounded-xl shadow-lg border border-gray-100 py-2 z-50"
                >
                  <div class="px-3 py-1.5 text-xs font-semibold text-gray-400 uppercase tracking-wider">Sort By</div>
                  <div class="max-h-64 overflow-y-auto">
                    <button
                      v-for="option in allCoursesSortOptions"
                      :key="option.value"
                      @click="allCoursesSortBy = option.value; showAllCoursesSortDropdown = false"
                      :class="[
                        'w-full px-3 py-2 text-left text-sm flex items-center gap-3 transition-colors',
                        allCoursesSortBy === option.value ? 'bg-teal-50 text-teal-700' : 'text-gray-700 hover:bg-gray-50'
                      ]"
                    >
                      <i :class="[option.icon, 'text-sm w-4', allCoursesSortBy === option.value ? 'text-teal-500' : 'text-gray-400']"></i>
                      <span class="flex-1">{{ option.label }}</span>
                      <i v-if="allCoursesSortBy === option.value" class="fas fa-check text-teal-500 text-xs"></i>
                    </button>
                  </div>
                </div>
                <div v-if="showAllCoursesSortDropdown" @click="showAllCoursesSortDropdown = false" class="fixed inset-0 z-40"></div>
              </div>

              <!-- View Toggle -->
              <div class="flex items-center gap-0.5 bg-white border border-gray-200 rounded-lg p-1 ml-auto">
                <button
                  @click="allCoursesViewMode = 'grid'"
                  :class="['px-2.5 py-1 rounded-md transition-all', allCoursesViewMode === 'grid' ? 'bg-teal-500 text-white' : 'text-gray-500 hover:bg-gray-100']"
                  title="Grid view"
                >
                  <i class="fas fa-th-large text-xs"></i>
                </button>
                <button
                  @click="allCoursesViewMode = 'list'"
                  :class="['px-2.5 py-1 rounded-md transition-all', allCoursesViewMode === 'list' ? 'bg-teal-500 text-white' : 'text-gray-500 hover:bg-gray-100']"
                  title="List view"
                >
                  <i class="fas fa-list text-xs"></i>
                </button>
              </div>
            </div>
          </div>

          <!-- Main Content Area -->
          <div class="p-4">
            <!-- Grid View -->
            <div v-if="allCoursesViewMode === 'grid'" class="all-courses-grid">
              <div v-for="course in filteredAllCourses" :key="course.id"
                   class="all-course-card group bg-white rounded-2xl overflow-hidden cursor-pointer transition-all duration-300 hover:-translate-y-1.5 border border-gray-100 shadow-sm hover:shadow-lg hover:border-teal-200">
                <!-- Card Thumbnail -->
                <div class="relative aspect-video">
                  <img :src="course.image" :alt="course.title" class="absolute inset-0 w-full h-full object-cover" />
                  <div class="absolute inset-0 bg-gradient-to-t from-black/50 via-black/10 to-transparent"></div>

                  <!-- Save Button -->
                  <button
                    @click.stop="toggleSaveAllCourse(course.id)"
                    :class="[
                      'absolute top-2 right-2 w-8 h-8 rounded-full flex items-center justify-center transition-all z-10',
                      course.saved
                        ? 'bg-teal-500 text-white'
                        : 'bg-white/90 text-gray-600 opacity-0 group-hover:opacity-100 hover:bg-teal-500 hover:text-white'
                    ]"
                  >
                    <i :class="course.saved ? 'fas fa-bookmark' : 'far fa-bookmark'" class="text-sm"></i>
                  </button>

                  <!-- Badges -->
                  <div class="absolute bottom-3 left-3 flex gap-1.5 z-10">
                    <span v-if="course.isNew" class="all-course-new-badge">
                      <i class="fas fa-sparkles"></i> New
                    </span>
                  </div>

                  <!-- Duration Badge -->
                  <div class="absolute bottom-3 right-3 all-course-duration-badge z-10">
                    <i class="fas fa-clock"></i> {{ course.duration }}
                  </div>

                  <!-- Play Button -->
                  <div class="all-course-play-btn">
                    <i class="fas fa-play"></i>
                  </div>
                </div>

                <!-- Card Body -->
                <div class="p-4">
                  <!-- Meta Info -->
                  <div class="flex items-center gap-3 text-[11px] text-gray-400 mb-2">
                    <span class="flex items-center gap-1">
                      <i class="fas fa-play-circle text-[9px]"></i>
                      {{ course.lessons }} lessons
                    </span>
                    <span class="flex items-center gap-1">
                      <i class="fas fa-signal text-[9px]"></i>
                      {{ course.level }}
                    </span>
                  </div>

                  <!-- Title -->
                  <h3 class="text-sm font-semibold text-gray-800 mb-2 line-clamp-2 group-hover:text-teal-600 transition-colors leading-snug">
                    {{ course.title }}
                  </h3>

                  <!-- Category -->
                  <p class="text-xs text-gray-500 mb-3">
                    <span class="px-2 py-0.5 bg-teal-50 text-teal-600 text-[10px] font-medium rounded-full">
                      {{ course.category }}
                    </span>
                  </p>

                  <!-- Tags -->
                  <div class="flex flex-wrap gap-1.5 mb-3">
                    <span
                      v-for="tag in course.tags.slice(0, 3)"
                      :key="tag"
                      class="px-2 py-0.5 bg-gray-100 text-gray-600 text-[10px] font-medium rounded-full hover:bg-teal-50 hover:text-teal-600 transition-colors"
                    >
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
                      <span class="flex items-center gap-1 hover:text-amber-500 transition-colors">
                        <i class="fas fa-star text-[9px] text-amber-400"></i>
                        {{ course.rating }}
                      </span>
                      <span class="flex items-center gap-1 hover:text-teal-500 transition-colors">
                        <i class="fas fa-users text-[9px]"></i>
                        {{ (course.students / 1000).toFixed(1) }}k
                      </span>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- List View -->
            <div v-else class="all-courses-list">
              <div v-for="course in filteredAllCourses" :key="course.id"
                   class="all-course-list-item cursor-pointer group">
                <!-- Thumbnail -->
                <div class="all-course-list-thumbnail">
                  <img :src="course.image" :alt="course.title" class="absolute inset-0 w-full h-full object-cover rounded-lg" />
                  <div class="absolute inset-0 bg-gradient-to-t from-black/40 to-transparent rounded-lg"></div>
                  <div class="all-course-list-play-overlay">
                    <i class="fas fa-play"></i>
                  </div>
                  <div class="all-course-list-duration">{{ course.duration }}</div>
                </div>

                <!-- Content -->
                <div class="all-course-list-content">
                  <div class="all-course-list-header">
                    <div class="all-course-list-badges">
                      <span :class="['all-course-level-badge', course.levelClass]">{{ course.level }}</span>
                      <span class="all-course-category-badge">{{ course.category }}</span>
                      <span v-if="course.isNew" class="all-course-list-new-badge">
                        <i class="fas fa-sparkles"></i> New
                      </span>
                    </div>
                    <h3 class="all-course-list-title">{{ course.title }}</h3>
                  </div>
                  <div class="all-course-list-footer">
                    <div class="all-course-list-instructor">
                      <div class="all-course-list-avatar">{{ course.instructorInitials }}</div>
                      <span>{{ course.instructor }}</span>
                    </div>
                    <div class="all-course-list-stats">
                      <span><i class="fas fa-play-circle"></i> {{ course.lessons }} lessons</span>
                      <span><i class="fas fa-star text-amber-400"></i> {{ course.rating }}</span>
                      <span><i class="fas fa-users"></i> {{ (course.students / 1000).toFixed(1) }}k</span>
                    </div>
                  </div>
                </div>

                <!-- Actions -->
                <div class="all-course-list-actions">
                  <button
                    :class="['all-course-list-action-btn', { saved: course.saved }]"
                    @click.stop="toggleSaveAllCourse(course.id)"
                    title="Save"
                  >
                    <i :class="course.saved ? 'fas fa-bookmark' : 'far fa-bookmark'"></i>
                  </button>
                  <button class="all-course-list-action-btn" title="Enroll">
                    <i class="fas fa-plus"></i>
                  </button>
                </div>
              </div>
            </div>

            <!-- Empty State -->
            <div v-if="filteredAllCourses.length === 0" class="all-courses-empty">
              <div class="all-courses-empty-icon">
                <i class="fas fa-search"></i>
              </div>
              <h3 class="all-courses-empty-title">No courses found</h3>
              <p class="all-courses-empty-text">Try adjusting your filters or search query</p>
              <button @click="allCoursesSearch = ''; allCoursesLevelFilter = []; allCoursesCategoryFilter = []" class="all-courses-clear-btn">
                <i class="fas fa-undo mr-2"></i> Clear Filters
              </button>
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

/* Featured Courses Section */
.featured-courses-section {
  background: linear-gradient(135deg, #f0fdfa 0%, #f8fafc 100%);
  border-radius: 1rem;
  padding: 1.25rem;
  border: 1px solid #e2e8f0;
}

.featured-courses-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 1rem;
}

.featured-courses-icon {
  width: 2.5rem;
  height: 2.5rem;
  border-radius: 0.75rem;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.3);
}

.featured-courses-icon i {
  color: white;
  font-size: 0.875rem;
}

.featured-courses-title {
  font-size: 1rem;
  font-weight: 700;
  color: #0f172a;
  margin: 0;
}

.featured-courses-subtitle {
  font-size: 0.6875rem;
  color: #64748b;
  margin: 0;
}

.featured-courses-nav {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.featured-courses-badge {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  padding: 0.375rem 0.75rem;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
  font-size: 0.6875rem;
  font-weight: 600;
  border-radius: 9999px;
}

.featured-courses-badge i {
  font-size: 0.625rem;
}

/* Featured Course Grid */
.featured-course-grid {
  display: grid;
  grid-template-columns: 1fr;
  gap: 1rem;
}

@media (min-width: 768px) {
  .featured-course-grid {
    grid-template-columns: 1.8fr 1fr;
  }
}

.featured-course-wrapper {
  position: relative;
}

/* Main Featured Card */
.featured-course-card {
  position: relative;
  border-radius: 0.875rem;
  overflow: hidden;
  cursor: pointer;
  aspect-ratio: 16/9;
  background: #1e293b;
}

.featured-course-img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.5s ease;
}

.featured-course-card:hover .featured-course-img {
  transform: scale(1.05);
}

.featured-course-overlay {
  position: absolute;
  inset: 0;
  background: linear-gradient(to top, rgba(0,0,0,0.85) 0%, rgba(0,0,0,0.3) 50%, transparent 100%);
}

.featured-course-badges {
  position: absolute;
  top: 0.75rem;
  left: 0.75rem;
  display: flex;
  gap: 0.5rem;
  z-index: 5;
}

.badge-continue {
  padding: 0.25rem 0.625rem;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
  font-size: 0.625rem;
  font-weight: 600;
  border-radius: 0.25rem;
  display: flex;
  align-items: center;
  gap: 0.25rem;
}

.badge-featured {
  padding: 0.25rem 0.625rem;
  background: linear-gradient(135deg, #f59e0b 0%, #d97706 100%);
  color: white;
  font-size: 0.625rem;
  font-weight: 600;
  border-radius: 0.25rem;
  display: flex;
  align-items: center;
  gap: 0.25rem;
  box-shadow: 0 2px 8px rgba(245, 158, 11, 0.3);
}

.badge-featured i {
  font-size: 0.5rem;
}

/* Save Button */
.featured-save-btn {
  position: absolute;
  top: 0.75rem;
  right: 0.75rem;
  width: 2.25rem;
  height: 2.25rem;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.95);
  border: none;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s ease;
  z-index: 10;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
}

.featured-save-btn:hover {
  transform: scale(1.1);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
}

.featured-save-btn i {
  font-size: 0.9375rem;
  color: #64748b;
  transition: all 0.2s ease;
}

.featured-save-btn:hover i {
  color: #14b8a6;
}

.featured-save-btn.saved {
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
}

.featured-save-btn.saved i {
  color: white;
}

.featured-save-btn.saved:hover {
  background: linear-gradient(135deg, #0d9488 0%, #0f766e 100%);
}

.badge-level {
  padding: 0.25rem 0.5rem;
  font-size: 0.5625rem;
  font-weight: 600;
  border-radius: 0.25rem;
  text-transform: uppercase;
}

.badge-level.beginner {
  background: #dcfce7;
  color: #16a34a;
}

.badge-level.intermediate {
  background: #dbeafe;
  color: #2563eb;
}

.badge-level.advanced {
  background: #ede9fe;
  color: #7c3aed;
}

.featured-course-play {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: 4rem;
  height: 4rem;
  border-radius: 50%;
  background: rgba(255,255,255,0.95);
  display: flex;
  align-items: center;
  justify-content: center;
  opacity: 0;
  transition: all 0.3s ease;
  box-shadow: 0 8px 24px rgba(0,0,0,0.3);
  z-index: 5;
}

.featured-course-card:hover .featured-course-play {
  opacity: 1;
  transform: translate(-50%, -50%) scale(1.1);
}

.featured-course-play i {
  color: #0d9488;
  font-size: 1.25rem;
  margin-left: 3px;
}

.featured-course-progress-bar {
  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
  height: 4px;
  background: rgba(255,255,255,0.2);
  z-index: 5;
}

.featured-course-progress-fill {
  height: 100%;
  background: linear-gradient(90deg, #14b8a6, #0d9488);
  transition: width 0.5s ease;
}

.featured-course-content {
  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
  padding: 1.25rem;
  z-index: 5;
  background: linear-gradient(to top, rgba(0,0,0,0.9) 0%, rgba(0,0,0,0.7) 60%, transparent 100%);
}

/* Tags */
.featured-course-tags {
  display: flex;
  gap: 0.5rem;
  margin-bottom: 0.625rem;
  flex-wrap: wrap;
}

.featured-tag {
  padding: 0.25rem 0.625rem;
  background: rgba(20, 184, 166, 0.2);
  color: #5eead4;
  font-size: 0.625rem;
  font-weight: 600;
  border-radius: 9999px;
  border: 1px solid rgba(20, 184, 166, 0.3);
}

.featured-course-title {
  font-size: 1.25rem;
  font-weight: 700;
  color: white;
  margin: 0 0 0.5rem 0;
  line-height: 1.3;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

/* Description */
.featured-course-description {
  font-size: 0.8125rem;
  color: rgba(255, 255, 255, 0.8);
  margin: 0 0 0.75rem 0;
  line-height: 1.5;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

/* Stats Row */
.featured-course-stats {
  display: flex;
  gap: 1rem;
  margin-bottom: 0.875rem;
  flex-wrap: wrap;
}

.featured-stat {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  font-size: 0.75rem;
  color: rgba(255, 255, 255, 0.85);
}

.featured-stat i {
  font-size: 0.6875rem;
  color: #5eead4;
}

/* Footer */
.featured-course-footer {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 1rem;
}

.featured-course-author {
  display: flex;
  align-items: center;
  gap: 0.625rem;
}

.featured-course-author .author-avatar {
  width: 2.25rem;
  height: 2.25rem;
  border-radius: 50%;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 0.75rem;
  font-weight: 600;
  border: 2px solid rgba(255, 255, 255, 0.2);
}

.featured-course-author .author-info {
  display: flex;
  flex-direction: column;
}

.featured-course-author .author-name {
  font-size: 0.8125rem;
  font-weight: 600;
  color: white;
}

.featured-course-author .author-role {
  font-size: 0.6875rem;
  color: rgba(255, 255, 255, 0.6);
}

/* Enroll Button */
.featured-enroll-btn {
  padding: 0.625rem 1.25rem;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
  font-size: 0.8125rem;
  font-weight: 600;
  border: none;
  border-radius: 0.5rem;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  transition: all 0.2s ease;
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.3);
}

.featured-enroll-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(20, 184, 166, 0.4);
}

.featured-enroll-btn i {
  font-size: 0.6875rem;
}

/* Carousel Arrows */
.course-carousel-arrow {
  position: absolute;
  top: 50%;
  transform: translateY(-50%);
  width: 2.25rem;
  height: 2.25rem;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(255, 255, 255, 0.9);
  border: none;
  border-radius: 50%;
  color: #0f172a;
  font-size: 0.75rem;
  cursor: pointer;
  opacity: 0;
  transition: all 0.3s ease;
  z-index: 10;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
}

.featured-course-card:hover .course-carousel-arrow {
  opacity: 1;
}

.course-carousel-arrow:hover {
  background: white;
  transform: translateY(-50%) scale(1.1);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
}

.course-carousel-prev {
  left: 0.75rem;
}

.course-carousel-next {
  right: 0.75rem;
}

/* Carousel Dots */
.course-carousel-dots {
  position: absolute;
  bottom: 0.75rem;
  left: 50%;
  transform: translateX(-50%);
  display: flex;
  gap: 0.5rem;
  z-index: 10;
}

.course-carousel-dot {
  width: 0.5rem;
  height: 0.5rem;
  border-radius: 50%;
  border: none;
  background: rgba(255, 255, 255, 0.5);
  cursor: pointer;
  transition: all 0.3s ease;
  padding: 0;
}

.course-carousel-dot:hover {
  background: rgba(255, 255, 255, 0.8);
}

.course-carousel-dot.active {
  background: white;
  width: 1.5rem;
  border-radius: 0.25rem;
}

/* Carousel Fade Transition */
.carousel-fade-enter-active,
.carousel-fade-leave-active {
  transition: opacity 0.4s ease;
}

.carousel-fade-enter-from,
.carousel-fade-leave-to {
  opacity: 0;
}

/* Up Next Column */
.up-next-courses {
  display: flex;
  flex-direction: column;
  min-width: 0;
  background: white;
  border-radius: 0.75rem;
  padding: 0.875rem;
  border: 1px solid #e2e8f0;
}

.up-next-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 0.75rem;
}

.up-next-title {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.875rem;
  font-weight: 700;
  color: #0f172a;
  margin: 0;
}

.up-next-title i {
  color: #14b8a6;
  font-size: 0.75rem;
}

.up-next-count {
  font-size: 0.625rem;
  color: #64748b;
  background: #f1f5f9;
  padding: 0.25rem 0.5rem;
  border-radius: 9999px;
}

.up-next-list {
  display: flex;
  flex-direction: column;
  gap: 0.625rem;
  flex: 1;
}

/* Up Next Card */
.up-next-course-card {
  display: flex;
  gap: 0.75rem;
  padding: 0.625rem;
  background: linear-gradient(135deg, #f8fafc 0%, #ffffff 100%);
  border: 1px solid #e2e8f0;
  border-radius: 0.75rem;
  cursor: pointer;
  transition: all 0.3s ease;
  position: relative;
}

.up-next-course-card:hover {
  border-color: #14b8a6;
  box-shadow: 0 8px 20px rgba(20, 184, 166, 0.12);
  transform: translateY(-2px);
  background: linear-gradient(135deg, #f0fdfa 0%, #ffffff 100%);
}

.up-next-thumb {
  position: relative;
  width: 100px;
  border-radius: 0.5rem;
  overflow: hidden;
  flex-shrink: 0;
  background: #1e293b;
  align-self: stretch;
}

.up-next-img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.4s ease;
}

.up-next-course-card:hover .up-next-img {
  transform: scale(1.1);
}

.up-next-overlay {
  position: absolute;
  inset: 0;
  background: linear-gradient(to top, rgba(0,0,0,0.4) 0%, transparent 60%);
  opacity: 0;
  transition: opacity 0.3s ease;
}

.up-next-course-card:hover .up-next-overlay {
  opacity: 1;
}

.up-next-play {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%) scale(0.8);
  width: 1.75rem;
  height: 1.75rem;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(255,255,255,0.95);
  border-radius: 50%;
  opacity: 0;
  transition: all 0.3s ease;
  box-shadow: 0 2px 8px rgba(0,0,0,0.2);
}

.up-next-course-card:hover .up-next-play {
  opacity: 1;
  transform: translate(-50%, -50%) scale(1);
}

.up-next-play i {
  color: #0d9488;
  font-size: 0.625rem;
  margin-left: 1px;
}

.up-next-duration {
  position: absolute;
  bottom: 4px;
  right: 4px;
  padding: 0.1875rem 0.375rem;
  background: rgba(0,0,0,0.85);
  color: white;
  font-size: 0.5625rem;
  font-weight: 600;
  border-radius: 0.25rem;
}

.up-next-save-btn {
  position: absolute;
  top: 0.5rem;
  right: 0.5rem;
  width: 1.75rem;
  height: 1.75rem;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.95);
  border: 1px solid #e2e8f0;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s ease;
  z-index: 5;
  opacity: 0;
  transform: scale(0.8);
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.1);
}

.up-next-course-card:hover .up-next-save-btn {
  opacity: 1;
  transform: scale(1);
}

.up-next-save-btn:hover {
  transform: scale(1.15);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.2);
}

.up-next-save-btn i {
  font-size: 0.75rem;
  color: #64748b;
  transition: all 0.2s ease;
}

.up-next-save-btn:hover i {
  color: #14b8a6;
}

.up-next-save-btn.saved {
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  border-color: transparent;
  opacity: 1;
  transform: scale(1);
}

.up-next-save-btn.saved i {
  color: white;
}

.up-next-save-btn.saved:hover {
  background: linear-gradient(135deg, #0d9488 0%, #0f766e 100%);
}

.up-next-progress {
  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
  height: 3px;
  background: rgba(255,255,255,0.3);
}

.up-next-progress-fill {
  height: 100%;
  background: linear-gradient(90deg, #14b8a6, #0d9488);
  border-radius: 0 0 0.5rem 0.5rem;
}

.up-next-info {
  flex: 1;
  min-width: 0;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
}

.up-next-badges {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-bottom: 0.25rem;
}

.up-next-level {
  display: inline-block;
  padding: 0.125rem 0.375rem;
  font-size: 0.5625rem;
  font-weight: 600;
  border-radius: 0.1875rem;
  text-transform: uppercase;
}

.up-next-level.beginner {
  background: #dcfce7;
  color: #16a34a;
}

.up-next-level.intermediate {
  background: #dbeafe;
  color: #2563eb;
}

.up-next-level.advanced {
  background: #ede9fe;
  color: #7c3aed;
}

.up-next-progress-badge {
  font-size: 0.625rem;
  font-weight: 700;
  color: #0d9488;
}

.up-next-course-title {
  font-size: 0.8125rem;
  font-weight: 600;
  color: #0f172a;
  margin: 0 0 0.375rem 0;
  line-height: 1.3;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
  transition: color 0.2s ease;
}

.up-next-course-card:hover .up-next-course-title {
  color: #0d9488;
}

.up-next-instructor {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  margin-bottom: 0.5rem;
}

.up-next-avatar {
  width: 1.25rem;
  height: 1.25rem;
  border-radius: 50%;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 0.5rem;
  font-weight: 600;
}

.up-next-instructor span {
  font-size: 0.625rem;
  color: #64748b;
}

.up-next-footer {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 0.5rem;
}

.up-next-meta {
  display: flex;
  gap: 0.625rem;
  font-size: 0.5625rem;
  color: #94a3b8;
}

.up-next-meta i {
  margin-right: 0.125rem;
}

.up-next-meta span:first-child i {
  color: #14b8a6;
}

.up-next-resume-btn {
  padding: 0.3125rem 0.625rem;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
  font-size: 0.5625rem;
  font-weight: 600;
  border: none;
  border-radius: 0.375rem;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 0.25rem;
  transition: all 0.2s ease;
  opacity: 0;
  transform: translateX(8px);
}

.up-next-course-card:hover .up-next-resume-btn {
  opacity: 1;
  transform: translateX(0);
}

.up-next-resume-btn:hover {
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.35);
  transform: scale(1.05);
}

.up-next-resume-btn i {
  font-size: 0.5rem;
}

.up-next-view-btn {
  padding: 0.3125rem 0.625rem;
  background: linear-gradient(135deg, #6366f1 0%, #4f46e5 100%);
  color: white;
  font-size: 0.5625rem;
  font-weight: 600;
  border: none;
  border-radius: 0.375rem;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 0.25rem;
  transition: all 0.2s ease;
  opacity: 0;
  transform: translateX(8px);
}

.up-next-course-card:hover .up-next-view-btn {
  opacity: 1;
  transform: translateX(0);
}

.up-next-view-btn:hover {
  box-shadow: 0 4px 12px rgba(99, 102, 241, 0.35);
  transform: scale(1.05);
}

.up-next-view-btn i {
  font-size: 0.5rem;
}

/* Recommended Section */
.recommended-section {
  background: linear-gradient(135deg, #f8fafc 0%, #ffffff 100%);
  border: 1px solid #e2e8f0;
  border-radius: 1rem;
  padding: 1.25rem;
}

.recommended-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 1rem;
}

.recommended-title-wrap {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.recommended-icon {
  width: 2.5rem;
  height: 2.5rem;
  border-radius: 0.75rem;
  background: linear-gradient(135deg, #f59e0b 0%, #d97706 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 4px 12px rgba(245, 158, 11, 0.3);
}

.recommended-icon i {
  color: white;
  font-size: 0.875rem;
}

.recommended-title {
  font-size: 1.125rem;
  font-weight: 700;
  color: #1e293b;
  margin: 0;
}

.recommended-subtitle {
  font-size: 0.75rem;
  color: #64748b;
  margin: 0;
}

.recommended-see-all {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem 1rem;
  background: transparent;
  border: 1px solid #e2e8f0;
  border-radius: 0.5rem;
  font-size: 0.75rem;
  font-weight: 600;
  color: #64748b;
  cursor: pointer;
  transition: all 0.2s ease;
}

.recommended-see-all:hover {
  background: #f1f5f9;
  color: #0d9488;
  border-color: #0d9488;
}

.recommended-see-all i {
  font-size: 0.625rem;
  transition: transform 0.2s ease;
}

.recommended-see-all:hover i {
  transform: translateX(3px);
}

.recommended-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 1rem;
}

.recommended-card {
  position: relative;
  background: white;
  border: 1px solid #e2e8f0;
  border-radius: 0.75rem;
  overflow: hidden;
  transition: all 0.3s ease;
  cursor: pointer;
}

.recommended-card:hover {
  border-color: #14b8a6;
  box-shadow: 0 8px 24px rgba(20, 184, 166, 0.15);
  transform: translateY(-4px);
}

.recommended-save-btn {
  position: absolute;
  top: 0.5rem;
  right: 0.5rem;
  width: 2rem;
  height: 2rem;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.95);
  border: 1px solid #e2e8f0;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s ease;
  z-index: 10;
  opacity: 0;
  transform: scale(0.8);
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.1);
}

.recommended-card:hover .recommended-save-btn {
  opacity: 1;
  transform: scale(1);
}

.recommended-save-btn:hover {
  transform: scale(1.1);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

.recommended-save-btn i {
  font-size: 0.75rem;
  color: #64748b;
  transition: all 0.2s ease;
}

.recommended-save-btn:hover i {
  color: #14b8a6;
}

.recommended-save-btn.saved {
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  border-color: transparent;
  opacity: 1;
  transform: scale(1);
}

.recommended-save-btn.saved i {
  color: white;
}

.recommended-match {
  position: absolute;
  top: 0.5rem;
  left: 0.5rem;
  padding: 0.25rem 0.5rem;
  background: linear-gradient(135deg, #10b981 0%, #059669 100%);
  color: white;
  font-size: 0.625rem;
  font-weight: 600;
  border-radius: 0.375rem;
  z-index: 5;
  display: flex;
  align-items: center;
  gap: 0.25rem;
}

.recommended-match i {
  font-size: 0.5rem;
}

.recommended-image {
  position: relative;
  height: 120px;
  overflow: hidden;
}

.recommended-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.4s ease;
}

.recommended-card:hover .recommended-image img {
  transform: scale(1.08);
}

.recommended-overlay {
  position: absolute;
  inset: 0;
  background: linear-gradient(to top, rgba(0,0,0,0.5) 0%, transparent 60%);
}

.recommended-duration {
  position: absolute;
  bottom: 0.5rem;
  right: 0.5rem;
  padding: 0.25rem 0.5rem;
  background: rgba(0,0,0,0.85);
  color: white;
  font-size: 0.625rem;
  font-weight: 600;
  border-radius: 0.25rem;
}

.recommended-content {
  padding: 0.875rem;
}

.recommended-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 0.375rem;
  margin-bottom: 0.5rem;
}

.recommended-tag {
  padding: 0.125rem 0.375rem;
  background: #f1f5f9;
  color: #64748b;
  font-size: 0.5625rem;
  font-weight: 500;
  border-radius: 0.25rem;
}

.recommended-level {
  padding: 0.125rem 0.375rem;
  font-size: 0.5625rem;
  font-weight: 600;
  border-radius: 0.25rem;
  text-transform: uppercase;
}

.recommended-level.beginner {
  background: #dcfce7;
  color: #16a34a;
}

.recommended-level.intermediate {
  background: #dbeafe;
  color: #2563eb;
}

.recommended-level.advanced {
  background: #ede9fe;
  color: #7c3aed;
}

.recommended-course-title {
  font-size: 0.875rem;
  font-weight: 700;
  color: #1e293b;
  margin: 0 0 0.5rem 0;
  line-height: 1.3;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.recommended-instructor {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-bottom: 0.75rem;
}

.recommended-avatar {
  width: 1.5rem;
  height: 1.5rem;
  border-radius: 50%;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
  font-size: 0.5rem;
  font-weight: 600;
  display: flex;
  align-items: center;
  justify-content: center;
}

.recommended-instructor span {
  font-size: 0.6875rem;
  color: #64748b;
}

.recommended-footer {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding-top: 0.625rem;
  border-top: 1px solid #f1f5f9;
}

.recommended-stats {
  display: flex;
  align-items: center;
  gap: 0.625rem;
}

.recommended-stats span {
  display: flex;
  align-items: center;
  gap: 0.25rem;
  font-size: 0.625rem;
  color: #64748b;
}

.recommended-stats i {
  font-size: 0.5rem;
}

.recommended-enroll-btn {
  padding: 0.375rem 0.75rem;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
  border: none;
  border-radius: 0.375rem;
  font-size: 0.625rem;
  font-weight: 600;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 0.25rem;
  transition: all 0.2s ease;
}

.recommended-enroll-btn:hover {
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.35);
  transform: scale(1.05);
}

.recommended-enroll-btn i {
  font-size: 0.5rem;
}

/* All Courses Section */
.all-courses-wrapper {
  margin-bottom: 1.5rem;
}

.all-courses-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
  gap: 1.25rem;
}

.all-course-card {
  position: relative;
}

.all-course-new-badge {
  display: inline-flex;
  align-items: center;
  gap: 0.25rem;
  padding: 0.25rem 0.5rem;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
  font-size: 0.625rem;
  font-weight: 700;
  border-radius: 0.375rem;
  text-transform: uppercase;
  box-shadow: 0 2px 8px rgba(20, 184, 166, 0.4);
}

.all-course-duration-badge {
  display: inline-flex;
  align-items: center;
  gap: 0.25rem;
  padding: 0.25rem 0.5rem;
  background: rgba(0, 0, 0, 0.75);
  backdrop-filter: blur(4px);
  color: white;
  font-size: 0.625rem;
  font-weight: 600;
  border-radius: 0.375rem;
}

.all-course-play-btn {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%) scale(0.8);
  width: 3rem;
  height: 3rem;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(255, 255, 255, 0.95);
  border-radius: 50%;
  opacity: 0;
  transition: all 0.3s ease;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.2);
}

.all-course-card:hover .all-course-play-btn {
  opacity: 1;
  transform: translate(-50%, -50%) scale(1);
}

.all-course-play-btn i {
  color: #0d9488;
  font-size: 1rem;
  margin-left: 2px;
}

/* All Courses List View */
.all-courses-list {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.all-course-list-item {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 0.75rem;
  background: white;
  border: 1px solid #e2e8f0;
  border-radius: 0.75rem;
  transition: all 0.2s ease;
}

.all-course-list-item:hover {
  border-color: #14b8a6;
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.1);
}

.all-course-list-thumbnail {
  position: relative;
  width: 160px;
  height: 90px;
  flex-shrink: 0;
  border-radius: 0.5rem;
  overflow: hidden;
}

.all-course-list-play-overlay {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%) scale(0.8);
  width: 2.5rem;
  height: 2.5rem;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(255, 255, 255, 0.95);
  border-radius: 50%;
  opacity: 0;
  transition: all 0.3s ease;
}

.all-course-list-item:hover .all-course-list-play-overlay {
  opacity: 1;
  transform: translate(-50%, -50%) scale(1);
}

.all-course-list-play-overlay i {
  color: #0d9488;
  font-size: 0.75rem;
  margin-left: 2px;
}

.all-course-list-duration {
  position: absolute;
  bottom: 0.375rem;
  right: 0.375rem;
  padding: 0.125rem 0.375rem;
  background: rgba(0, 0, 0, 0.75);
  color: white;
  font-size: 0.5625rem;
  font-weight: 600;
  border-radius: 0.25rem;
}

.all-course-list-content {
  flex: 1;
  min-width: 0;
}

.all-course-list-header {
  margin-bottom: 0.5rem;
}

.all-course-list-badges {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-bottom: 0.375rem;
}

.all-course-level-badge {
  padding: 0.125rem 0.375rem;
  font-size: 0.5625rem;
  font-weight: 600;
  border-radius: 0.25rem;
  text-transform: uppercase;
}

.all-course-level-badge.beginner {
  background: #dcfce7;
  color: #16a34a;
}

.all-course-level-badge.intermediate {
  background: #dbeafe;
  color: #2563eb;
}

.all-course-level-badge.advanced {
  background: #ede9fe;
  color: #7c3aed;
}

.all-course-category-badge {
  padding: 0.125rem 0.375rem;
  background: #f0fdfa;
  color: #0d9488;
  font-size: 0.5625rem;
  font-weight: 600;
  border-radius: 0.25rem;
}

.all-course-list-new-badge {
  display: inline-flex;
  align-items: center;
  gap: 0.25rem;
  padding: 0.125rem 0.375rem;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
  font-size: 0.5625rem;
  font-weight: 700;
  border-radius: 0.25rem;
  text-transform: uppercase;
}

.all-course-list-title {
  font-size: 0.875rem;
  font-weight: 600;
  color: #1e293b;
  margin: 0;
  transition: color 0.2s ease;
}

.all-course-list-item:hover .all-course-list-title {
  color: #0d9488;
}

.all-course-list-footer {
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.all-course-list-instructor {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.all-course-list-avatar {
  width: 1.5rem;
  height: 1.5rem;
  border-radius: 50%;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
  font-size: 0.5rem;
  font-weight: 600;
  display: flex;
  align-items: center;
  justify-content: center;
}

.all-course-list-instructor span {
  font-size: 0.6875rem;
  color: #64748b;
}

.all-course-list-stats {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.all-course-list-stats span {
  display: flex;
  align-items: center;
  gap: 0.25rem;
  font-size: 0.6875rem;
  color: #64748b;
}

.all-course-list-stats i {
  font-size: 0.5rem;
}

.all-course-list-actions {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.all-course-list-action-btn {
  width: 2rem;
  height: 2rem;
  border-radius: 0.5rem;
  background: #f1f5f9;
  border: none;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s ease;
  color: #64748b;
}

.all-course-list-action-btn:hover {
  background: #e2e8f0;
  color: #0d9488;
}

.all-course-list-action-btn.saved {
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
}

.all-course-list-action-btn i {
  font-size: 0.75rem;
}

/* All Courses Empty State */
.all-courses-empty {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 3rem 1rem;
  text-align: center;
}

.all-courses-empty-icon {
  width: 4rem;
  height: 4rem;
  border-radius: 50%;
  background: #f1f5f9;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 1rem;
}

.all-courses-empty-icon i {
  font-size: 1.5rem;
  color: #94a3b8;
}

.all-courses-empty-title {
  font-size: 1rem;
  font-weight: 600;
  color: #1e293b;
  margin: 0 0 0.5rem 0;
}

.all-courses-empty-text {
  font-size: 0.875rem;
  color: #64748b;
  margin: 0 0 1rem 0;
}

.all-courses-clear-btn {
  padding: 0.5rem 1rem;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
  border: none;
  border-radius: 0.5rem;
  font-size: 0.75rem;
  font-weight: 600;
  cursor: pointer;
  display: flex;
  align-items: center;
  transition: all 0.2s ease;
}

.all-courses-clear-btn:hover {
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.35);
}

/* Responsive */
@media (max-width: 767px) {
  .featured-course-content {
    padding: 1rem;
  }

  .featured-course-title {
    font-size: 1rem;
  }

  .featured-course-description {
    display: none;
  }

  .featured-course-stats {
    gap: 0.625rem;
  }

  .featured-stat {
    font-size: 0.6875rem;
  }

  .featured-enroll-btn {
    padding: 0.5rem 0.875rem;
    font-size: 0.75rem;
  }

  .up-next-courses {
    padding: 0.75rem;
  }

  .up-next-list {
    flex-direction: row;
    overflow-x: auto;
    gap: 0.75rem;
    padding-bottom: 0.5rem;
  }

  .up-next-course-card {
    flex: 0 0 260px;
    flex-direction: column;
  }

  .up-next-thumb {
    width: 100%;
    height: 120px;
  }

  .up-next-resume-btn,
  .up-next-view-btn {
    opacity: 1;
    transform: translateX(0);
  }

  .up-next-save-btn {
    opacity: 1;
    transform: scale(1);
  }

  .recommended-section {
    padding: 1rem;
  }

  .recommended-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 0.75rem;
  }

  .recommended-see-all {
    width: 100%;
    justify-content: center;
  }

  .recommended-grid {
    grid-template-columns: repeat(2, 1fr);
    gap: 0.75rem;
  }

  .recommended-image {
    height: 100px;
  }

  .recommended-content {
    padding: 0.75rem;
  }

  .recommended-course-title {
    font-size: 0.75rem;
  }

  .recommended-instructor {
    display: none;
  }

  .recommended-stats {
    gap: 0.5rem;
  }

  .recommended-enroll-btn {
    padding: 0.25rem 0.5rem;
    font-size: 0.5625rem;
  }

  .recommended-save-btn {
    opacity: 1;
    transform: scale(1);
  }

  /* All Courses Responsive */
  .all-courses-grid {
    grid-template-columns: 1fr;
  }

  .all-course-list-item {
    flex-direction: column;
    align-items: flex-start;
  }

  .all-course-list-thumbnail {
    width: 100%;
    height: 140px;
  }

  .all-course-list-footer {
    flex-direction: column;
    align-items: flex-start;
    gap: 0.5rem;
  }

  .all-course-list-actions {
    width: 100%;
    justify-content: flex-end;
  }
}
</style>
