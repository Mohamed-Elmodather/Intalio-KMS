<script setup lang="ts">
import { ref, computed, watch } from 'vue'
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

// View state
const currentView = ref<'all' | 'my-courses' | 'paths' | 'certificates'>('all')
const viewMode = ref<'grid' | 'list'>('grid')

// Stats
const overallProgress = ref(68)
const completedCourses = ref(8)
const totalCourses = ref(12)
const learningHours = ref(45)
const certificates = ref(5)
const streak = ref(12)
const totalEnrolled = ref(156)

// View options (Collections style)
const viewOptions = ref([
  { id: 'all', name: 'All Courses', icon: 'fas fa-graduation-cap', color: 'text-teal-500' },
  { id: 'my-courses', name: 'My Courses', icon: 'fas fa-book-reader', color: 'text-blue-500' },
  { id: 'paths', name: 'Learning Paths', icon: 'fas fa-route', color: 'text-indigo-500' },
  { id: 'certificates', name: 'Certificates', icon: 'fas fa-certificate', color: 'text-amber-500' },
])

// Filtered courses based on current view
const filteredCourses = computed(() => {
  let result = [...enrolledCourses.value]

  switch (currentView.value) {
    case 'my-courses':
      // All enrolled courses
      break
    case 'all':
    default:
      // Show all available courses (enrolled + browse)
      break
  }

  return result
})

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

// Trending courses
const trendingCourses = ref([
  {
    id: 10,
    title: 'Machine Learning Basics',
    instructor: 'Dr. Sarah Mitchell',
    instructorInitials: 'SM',
    duration: '4 hours',
    totalLessons: 12,
    level: 'Intermediate',
    levelClass: 'intermediate',
    trendingRank: 1,
    viewsThisWeek: 12500,
    rating: 4.8,
    students: 3200,
    image: 'https://images.unsplash.com/photo-1677442136019-21780ecad995?w=400&h=300&fit=crop',
    tags: ['AI', 'Machine Learning'],
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
    trendingRank: 2,
    viewsThisWeek: 9800,
    rating: 4.7,
    students: 4500,
    image: 'https://images.unsplash.com/photo-1475721027785-f74eccf877e2?w=400&h=300&fit=crop',
    tags: ['Speaking', 'Communication'],
    saved: true
  },
  {
    id: 17,
    title: 'Python for Data Science',
    instructor: 'Dr. James Wilson',
    instructorInitials: 'JW',
    duration: '6 hours',
    totalLessons: 18,
    level: 'Beginner',
    levelClass: 'beginner',
    trendingRank: 3,
    viewsThisWeek: 8200,
    rating: 4.9,
    students: 5200,
    image: 'https://images.unsplash.com/photo-1526379095098-d400fd0bf935?w=400&h=300&fit=crop',
    tags: ['Python', 'Data Science'],
    saved: false
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
    trendingRank: 4,
    viewsThisWeek: 6500,
    rating: 4.9,
    students: 2100,
    image: 'https://images.unsplash.com/photo-1451187580459-43490279c0fa?w=400&h=300&fit=crop',
    tags: ['Cloud', 'AWS'],
    saved: false
  }
])

// Recommended courses (for My Courses tab)
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
    id: 14,
    title: 'UX Design Principles',
    instructor: 'Lisa Park',
    instructorInitials: 'LP',
    duration: '5 hours',
    totalLessons: 14,
    level: 'Intermediate',
    levelClass: 'intermediate',
    matchScore: 82,
    rating: 4.8,
    students: 2890,
    image: 'https://images.unsplash.com/photo-1561070791-2526d30994b5?w=400&h=300&fit=crop',
    tags: ['Design', 'UX'],
    saved: true
  },
  {
    id: 16,
    title: 'Agile Project Management',
    instructor: 'Jennifer Adams',
    instructorInitials: 'JA',
    duration: '4 hours',
    totalLessons: 10,
    level: 'Intermediate',
    levelClass: 'intermediate',
    matchScore: 78,
    rating: 4.7,
    students: 3450,
    image: 'https://images.unsplash.com/photo-1552664730-d307ca884978?w=400&h=300&fit=crop',
    tags: ['Agile', 'Scrum'],
    saved: false
  }
])

// Catalog courses (All Courses)
const allCourses = ref([
  // Enrolled courses
  { id: 1, title: 'Advanced Data Analytics', instructor: 'Dr. James Wilson', instructorInitials: 'JW', level: 'Advanced', levelClass: 'advanced', duration: '8 hours', lessons: 12, completedLessons: 9, category: 'Technology', rating: 4.8, students: 1250, image: 'https://images.unsplash.com/photo-1551288049-bebda4e38f71?w=400&h=300&fit=crop', tags: ['Data Science', 'Analytics'], saved: false, isNew: false, enrolled: true, progress: 75, status: 'In Progress', statusClass: 'in-progress', sharedWithMe: false },
  { id: 2, title: 'Leadership Essentials', instructor: 'Maria Garcia', instructorInitials: 'MG', level: 'Intermediate', levelClass: 'intermediate', duration: '6 hours', lessons: 11, completedLessons: 5, category: 'Business', rating: 4.6, students: 2340, image: 'https://images.unsplash.com/photo-1519389950473-47ba0277781c?w=400&h=300&fit=crop', tags: ['Leadership', 'Management'], saved: true, isNew: false, enrolled: true, progress: 45, status: 'In Progress', statusClass: 'in-progress', sharedWithMe: true, sharedBy: 'John Smith' },
  { id: 3, title: 'Cybersecurity Fundamentals', instructor: 'Alex Thompson', instructorInitials: 'AT', level: 'Beginner', levelClass: 'beginner', duration: '4 hours', lessons: 8, completedLessons: 8, category: 'Technology', rating: 4.9, students: 3450, image: 'https://images.unsplash.com/photo-1550751827-4bd374c3f58b?w=400&h=300&fit=crop', tags: ['Security', 'IT'], saved: false, isNew: false, enrolled: true, progress: 100, status: 'Completed', statusClass: 'completed', sharedWithMe: false },
  { id: 4, title: 'Effective Communication', instructor: 'Sarah Chen', instructorInitials: 'SC', level: 'Beginner', levelClass: 'beginner', duration: '5 hours', lessons: 10, completedLessons: 0, category: 'Soft Skills', rating: 4.7, students: 1890, image: 'https://images.unsplash.com/photo-1552664730-d307ca884978?w=400&h=300&fit=crop', tags: ['Soft Skills', 'Communication'], saved: false, isNew: false, enrolled: true, progress: 0, status: 'Not Started', statusClass: 'not-started', sharedWithMe: true, sharedBy: 'Emily Davis' },
  { id: 5, title: 'Project Management Pro', instructor: 'David Lee', instructorInitials: 'DL', level: 'Intermediate', levelClass: 'intermediate', duration: '10 hours', lessons: 15, completedLessons: 4, category: 'Business', rating: 4.5, students: 2780, image: 'https://images.unsplash.com/photo-1507925921958-8a62f3d1a50d?w=400&h=300&fit=crop', tags: ['Project Management', 'Agile'], saved: false, isNew: false, enrolled: true, progress: 30, status: 'In Progress', statusClass: 'in-progress', sharedWithMe: false },
  // Non-enrolled courses
  { id: 10, title: 'Machine Learning Basics', instructor: 'Dr. Sarah Mitchell', instructorInitials: 'SM', level: 'Intermediate', levelClass: 'intermediate', duration: '4 hours', lessons: 12, category: 'Technology', rating: 4.8, students: 3200, image: 'https://images.unsplash.com/photo-1677442136019-21780ecad995?w=400&h=300&fit=crop', tags: ['AI', 'ML'], saved: false, isNew: true, enrolled: false, sharedWithMe: true, sharedBy: 'Mike Johnson' },
  { id: 11, title: 'Strategic Planning', instructor: 'Michael Brown', instructorInitials: 'MB', level: 'Advanced', levelClass: 'advanced', duration: '3 hours', lessons: 8, category: 'Business', rating: 4.6, students: 1890, image: 'https://images.unsplash.com/photo-1454165804606-c3d57bc86b40?w=400&h=300&fit=crop', tags: ['Strategy', 'Planning'], saved: false, isNew: false, enrolled: false, sharedWithMe: false },
  { id: 12, title: 'Public Speaking Mastery', instructor: 'Emma Wilson', instructorInitials: 'EW', level: 'Beginner', levelClass: 'beginner', duration: '2 hours', lessons: 6, category: 'Soft Skills', rating: 4.7, students: 4500, image: 'https://images.unsplash.com/photo-1475721027785-f74eccf877e2?w=400&h=300&fit=crop', tags: ['Speaking', 'Communication'], saved: true, isNew: false, enrolled: false, sharedWithMe: false },
  { id: 13, title: 'Cloud Architecture Fundamentals', instructor: 'David Kim', instructorInitials: 'DK', level: 'Advanced', levelClass: 'advanced', duration: '8 hours', lessons: 20, category: 'Technology', rating: 4.9, students: 2100, image: 'https://images.unsplash.com/photo-1451187580459-43490279c0fa?w=400&h=300&fit=crop', tags: ['Cloud', 'AWS'], saved: false, isNew: true, enrolled: false, sharedWithMe: true, sharedBy: 'Sarah Wilson' },
  { id: 14, title: 'UX Design Principles', instructor: 'Lisa Park', instructorInitials: 'LP', level: 'Intermediate', levelClass: 'intermediate', duration: '5 hours', lessons: 14, category: 'Design', rating: 4.8, students: 2890, image: 'https://images.unsplash.com/photo-1561070791-2526d30994b5?w=400&h=300&fit=crop', tags: ['Design', 'UX'], saved: false, isNew: false, enrolled: false, sharedWithMe: false },
  { id: 15, title: 'Financial Analysis', instructor: 'Robert Chen', instructorInitials: 'RC', level: 'Intermediate', levelClass: 'intermediate', duration: '6 hours', lessons: 16, category: 'Finance', rating: 4.5, students: 1670, image: 'https://images.unsplash.com/photo-1554224155-6726b3ff858f?w=400&h=300&fit=crop', tags: ['Finance', 'Analysis'], saved: false, isNew: false, enrolled: false, sharedWithMe: false },
  { id: 16, title: 'Agile Project Management', instructor: 'Jennifer Adams', instructorInitials: 'JA', level: 'Intermediate', levelClass: 'intermediate', duration: '4 hours', lessons: 10, category: 'Business', rating: 4.7, students: 3450, image: 'https://images.unsplash.com/photo-1552664730-d307ca884978?w=400&h=300&fit=crop', tags: ['Agile', 'Scrum'], saved: false, isNew: true, enrolled: false, sharedWithMe: false },
  { id: 17, title: 'Python for Data Science', instructor: 'Dr. James Wilson', instructorInitials: 'JW', level: 'Beginner', levelClass: 'beginner', duration: '6 hours', lessons: 18, category: 'Technology', rating: 4.9, students: 5200, image: 'https://images.unsplash.com/photo-1526379095098-d400fd0bf935?w=400&h=300&fit=crop', tags: ['Python', 'Data Science'], saved: true, isNew: false, enrolled: false, sharedWithMe: false },
  { id: 18, title: 'Digital Marketing Essentials', instructor: 'Sophia Martinez', instructorInitials: 'SM', level: 'Beginner', levelClass: 'beginner', duration: '3 hours', lessons: 8, category: 'Marketing', rating: 4.6, students: 2780, image: 'https://images.unsplash.com/photo-1460925895917-afdab827c52f?w=400&h=300&fit=crop', tags: ['Marketing', 'Digital'], saved: false, isNew: false, enrolled: false, sharedWithMe: false },
])

// All Courses section state
const allCoursesSearch = ref('')
const allCoursesLevelFilter = ref<string[]>([])
const allCoursesCategoryFilter = ref<string[]>([])
const allCoursesViewMode = ref<'grid' | 'list'>('grid')
const showLevelFilter = ref(false)
const showCategoryFilterDropdown = ref(false)
const allCoursesSortBy = ref('popular')
const allCoursesSortOrder = ref<'asc' | 'desc'>('desc')
const showAllCoursesSortDropdown = ref(false)
const allCoursesEnrollmentFilter = ref<string[]>([])
const showEnrollmentFilter = ref(false)
const selectedStatusFilters = ref<string[]>([])
const showStatusFilter = ref(false)

const courseEnrollmentOptions = [
  { id: 'enrolled', label: 'My Enrolled', color: 'text-teal-500' },
  { id: 'not-enrolled', label: 'Not Enrolled', color: 'text-gray-500' }
]

const courseProgressOptions = [
  { id: 'in-progress', label: 'In Progress', color: 'text-blue-500' },
  { id: 'completed', label: 'Completed', color: 'text-green-500' },
  { id: 'not-started', label: 'Not Started', color: 'text-gray-500' }
]

// Status filter options (like Documents page)
const statusFilterOptions = [
  { id: 'saved', label: 'My Saved', icon: 'fas fa-bookmark', color: 'text-amber-500' },
  { id: 'shared', label: 'Shared with me', icon: 'fas fa-share-alt', color: 'text-purple-500' }
]

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

// Active Filters Count
const activeFiltersCount = computed(() => {
  let count = 0
  if (allCoursesSearch.value) count++
  count += allCoursesLevelFilter.value.length
  count += allCoursesCategoryFilter.value.length
  count += allCoursesEnrollmentFilter.value.length
  count += selectedStatusFilters.value.length
  return count
})

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

  // Progress filter (enrollment + progress status)
  if (allCoursesEnrollmentFilter.value.length > 0) {
    courses = courses.filter(c => {
      if (allCoursesEnrollmentFilter.value.includes('enrolled') && c.enrolled) return true
      if (allCoursesEnrollmentFilter.value.includes('not-enrolled') && !c.enrolled) return true
      if (allCoursesEnrollmentFilter.value.includes('in-progress') && c.enrolled && (c.progress ?? 0) > 0 && (c.progress ?? 0) < 100) return true
      if (allCoursesEnrollmentFilter.value.includes('completed') && c.enrolled && (c.progress ?? 0) === 100) return true
      if (allCoursesEnrollmentFilter.value.includes('not-started') && c.enrolled && (c.progress ?? 0) === 0) return true
      return false
    })
  }

  // Status filter (saved/shared)
  if (selectedStatusFilters.value.length > 0) {
    courses = courses.filter(c => {
      if (selectedStatusFilters.value.includes('saved') && c.saved) return true
      if (selectedStatusFilters.value.includes('shared') && c.sharedWithMe) return true
      return false
    })
  }

  // Sort
  const multiplier = allCoursesSortOrder.value === 'asc' ? 1 : -1
  switch (allCoursesSortBy.value) {
    case 'popular':
      courses.sort((a, b) => (b.students - a.students) * multiplier)
      break
    case 'rating':
      courses.sort((a, b) => (b.rating - a.rating) * multiplier)
      break
    case 'newest':
      courses.sort((a, b) => ((b.isNew ? 1 : 0) - (a.isNew ? 1 : 0)) * multiplier)
      break
    case 'title':
      courses.sort((a, b) => a.title.localeCompare(b.title) * multiplier)
      break
  }

  return courses
})

// My Courses filtered (enrolled, recommended/new, saved, or shared with me)
const myCoursesFiltered = computed(() => {
  // First filter to only courses that are enrolled, saved, shared, or new (recommended)
  let courses = allCourses.value.filter(c => c.enrolled || c.saved || c.sharedWithMe || c.isNew)

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

  // Progress filter (enrollment + progress status)
  if (allCoursesEnrollmentFilter.value.length > 0) {
    courses = courses.filter(c => {
      if (allCoursesEnrollmentFilter.value.includes('enrolled') && c.enrolled) return true
      if (allCoursesEnrollmentFilter.value.includes('not-enrolled') && !c.enrolled) return true
      if (allCoursesEnrollmentFilter.value.includes('in-progress') && c.enrolled && (c.progress ?? 0) > 0 && (c.progress ?? 0) < 100) return true
      if (allCoursesEnrollmentFilter.value.includes('completed') && c.enrolled && (c.progress ?? 0) === 100) return true
      if (allCoursesEnrollmentFilter.value.includes('not-started') && c.enrolled && (c.progress ?? 0) === 0) return true
      return false
    })
  }

  // Status filter (saved/shared)
  if (selectedStatusFilters.value.length > 0) {
    courses = courses.filter(c => {
      if (selectedStatusFilters.value.includes('saved') && c.saved) return true
      if (selectedStatusFilters.value.includes('shared') && c.sharedWithMe) return true
      return false
    })
  }

  // Sort
  const multiplier = allCoursesSortOrder.value === 'asc' ? 1 : -1
  switch (allCoursesSortBy.value) {
    case 'popular':
      courses.sort((a, b) => (b.students - a.students) * multiplier)
      break
    case 'rating':
      courses.sort((a, b) => (b.rating - a.rating) * multiplier)
      break
    case 'newest':
      courses.sort((a, b) => ((b.isNew ? 1 : 0) - (a.isNew ? 1 : 0)) * multiplier)
      break
    case 'title':
      courses.sort((a, b) => a.title.localeCompare(b.title) * multiplier)
      break
  }

  return courses
})

// Get the appropriate courses based on current view
const displayedCourses = computed(() => {
  return currentView.value === 'my-courses' ? myCoursesFiltered.value : filteredAllCourses.value
})

// ============================================
// PAGINATION STATE
// ============================================
const coursesCurrentPage = ref(1)
const coursesItemsPerPage = ref(10)
const coursesItemsPerPageOptions = [5, 10, 20, 50, 100]

// ============================================
// PAGINATION COMPUTED PROPERTIES
// ============================================
const coursesTotalPages = computed(() => Math.ceil(displayedCourses.value.length / coursesItemsPerPage.value))

const paginatedCourses = computed(() => {
  const start = (coursesCurrentPage.value - 1) * coursesItemsPerPage.value
  return displayedCourses.value.slice(start, start + coursesItemsPerPage.value)
})

const coursesPaginationStart = computed(() => {
  if (displayedCourses.value.length === 0) return 0
  return (coursesCurrentPage.value - 1) * coursesItemsPerPage.value + 1
})

const coursesPaginationEnd = computed(() => Math.min(coursesCurrentPage.value * coursesItemsPerPage.value, displayedCourses.value.length))

// ============================================
// PAGINATION FUNCTIONS
// ============================================
function goToCoursesPage(page: number) {
  if (page >= 1 && page <= coursesTotalPages.value) {
    coursesCurrentPage.value = page
  }
}

function nextCoursesPage() {
  if (coursesCurrentPage.value < coursesTotalPages.value) {
    coursesCurrentPage.value++
  }
}

function prevCoursesPage() {
  if (coursesCurrentPage.value > 1) {
    coursesCurrentPage.value--
  }
}

function changeCoursesItemsPerPage(count: number) {
  coursesItemsPerPage.value = count
  coursesCurrentPage.value = 1
}

// Reset pagination when view or filters change
watch([currentView, allCoursesSearch, allCoursesLevelFilter, allCoursesCategoryFilter, allCoursesEnrollmentFilter, selectedStatusFilters], () => {
  coursesCurrentPage.value = 1
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

function toggleEnrollmentFilterOption(option: string) {
  const index = allCoursesEnrollmentFilter.value.indexOf(option)
  if (index === -1) {
    allCoursesEnrollmentFilter.value.push(option)
  } else {
    allCoursesEnrollmentFilter.value.splice(index, 1)
  }
}

function toggleStatusFilter(status: string) {
  const index = selectedStatusFilters.value.indexOf(status)
  if (index > -1) {
    selectedStatusFilters.value.splice(index, 1)
  } else {
    selectedStatusFilters.value.push(status)
  }
}

function isStatusSelected(status: string): boolean {
  return selectedStatusFilters.value.includes(status)
}

function toggleLevelFilter(level: string) {
  const index = allCoursesLevelFilter.value.indexOf(level)
  if (index > -1) {
    allCoursesLevelFilter.value.splice(index, 1)
  } else {
    allCoursesLevelFilter.value.push(level)
  }
}

function toggleCategoryFilter(cat: string) {
  const index = allCoursesCategoryFilter.value.indexOf(cat)
  if (index > -1) {
    allCoursesCategoryFilter.value.splice(index, 1)
  } else {
    allCoursesCategoryFilter.value.push(cat)
  }
}

function getEnrollmentLabel(option: string): string {
  const labels: Record<string, string> = {
    'enrolled': 'My Enrolled',
    'not-enrolled': 'Not Enrolled',
    'in-progress': 'In Progress',
    'completed': 'Completed',
    'not-started': 'Not Started'
  }
  return labels[option] || option
}

function clearAllFilters() {
  allCoursesSearch.value = ''
  allCoursesLevelFilter.value = []
  allCoursesCategoryFilter.value = []
  allCoursesEnrollmentFilter.value = []
  selectedStatusFilters.value = []
}

function toggleSaveAllCourse(courseId: number) {
  const course = allCourses.value.find(c => c.id === courseId)
  if (course) {
    course.saved = !course.saved
  }
}

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
  { id: 6, title: 'Leadership Excellence', course: 'Leadership Development', date: 'Aug 20, 2024', credentialId: 'AFC-LD-2024-006', icon: 'fas fa-crown', color: '#6366f1', instructor: 'James Rodriguez', hours: 8, score: 94, skills: ['Leadership', 'Team Building', 'Coaching'] },
  { id: 7, title: 'Cloud Computing Basics', course: 'AWS Cloud Fundamentals', date: 'Jul 12, 2024', credentialId: 'AFC-CC-2024-007', icon: 'fas fa-cloud', color: '#06b6d4', instructor: 'Lisa Park', hours: 12, score: 91, skills: ['AWS', 'Cloud', 'DevOps'] },
  { id: 8, title: 'Excel Advanced Analytics', course: 'Excel Mastery', date: 'Jun 05, 2024', credentialId: 'AFC-EA-2024-008', icon: 'fas fa-file-excel', color: '#22c55e', instructor: 'Tom Anderson', hours: 6, score: 89, skills: ['Excel', 'Data Analysis', 'Macros'] },
])

// Certificate filter & search state
const certSearch = ref('')
const certSortBy = ref('date')
const certSortOrder = ref<'asc' | 'desc'>('desc')
const certViewMode = ref<'grid' | 'list'>('grid')
const certScoreFilter = ref('all')
const showCertSortDropdown = ref(false)
const showCertScoreFilter = ref(false)

// Certificate pagination state
const certCurrentPage = ref(1)
const certItemsPerPage = ref(6)
const certItemsPerPageOptions = [6, 12, 24]

// Certificate sort options
const certSortOptions = [
  { value: 'date', label: 'Date Earned', icon: 'fas fa-calendar' },
  { value: 'score', label: 'Highest Score', icon: 'fas fa-star' },
  { value: 'title', label: 'Title A-Z', icon: 'fas fa-sort-alpha-down' },
  { value: 'hours', label: 'Most Hours', icon: 'fas fa-clock' },
]

// Certificate score filter options
const certScoreOptions = [
  { value: 'all', label: 'All Scores', min: 0, max: 100 },
  { value: '90+', label: '90% and above', min: 90, max: 100 },
  { value: '80-89', label: '80% - 89%', min: 80, max: 89 },
  { value: 'below80', label: 'Below 80%', min: 0, max: 79 },
]

// Path filter & search state
const pathFilter = ref('all')
const pathsSearch = ref('')
const pathsLevelFilter = ref<string[]>([])
const pathsSortBy = ref('popular')
const pathsSortOrder = ref<'asc' | 'desc'>('desc')
const pathsViewMode = ref<'grid' | 'list'>('grid')

// Paths filter dropdown states
const showPathsLevelFilter = ref(false)
const showPathsEnrollmentFilter = ref(false)
const showPathsSortDropdown = ref(false)

// Paths pagination state
const pathsCurrentPage = ref(1)
const pathsItemsPerPage = ref(10)
const pathsItemsPerPageOptions = [5, 10, 20, 50]

// Paths sort options
const pathsSortOptions = [
  { value: 'popular', label: 'Most Popular', icon: 'fas fa-fire' },
  { value: 'rating', label: 'Highest Rated', icon: 'fas fa-star' },
  { value: 'newest', label: 'Newest', icon: 'fas fa-clock' },
  { value: 'title', label: 'Title A-Z', icon: 'fas fa-sort-alpha-down' },
  { value: 'courses', label: 'Most Courses', icon: 'fas fa-book' },
]

// Path level options (derived from learningPaths)
const pathLevelOptions = computed(() => {
  const levels = [...new Set(learningPaths.value.map(p => p.level))]
  return levels.map(l => ({ id: l.toLowerCase(), label: l }))
})

// Computed for filtered paths
const filteredPaths = computed(() => {
  let paths = learningPaths.value

  // Filter by enrollment status
  if (pathFilter.value === 'enrolled') {
    paths = paths.filter(p => p.isEnrolled)
  } else if (pathFilter.value === 'available') {
    paths = paths.filter(p => !p.isEnrolled)
  }

  // Filter by search
  if (pathsSearch.value) {
    const search = pathsSearch.value.toLowerCase()
    paths = paths.filter(p =>
      p.title.toLowerCase().includes(search) ||
      p.description.toLowerCase().includes(search) ||
      p.skills.some((s: string) => s.toLowerCase().includes(search))
    )
  }

  // Filter by level
  if (pathsLevelFilter.value.length > 0) {
    paths = paths.filter(p => pathsLevelFilter.value.includes(p.level.toLowerCase()))
  }

  // Sort
  paths = [...paths].sort((a, b) => {
    let comparison = 0
    switch (pathsSortBy.value) {
      case 'popular':
        comparison = b.enrolled - a.enrolled
        break
      case 'rating':
        comparison = b.rating - a.rating
        break
      case 'newest':
        comparison = 0 // Keep original order for now
        break
      case 'title':
        comparison = a.title.localeCompare(b.title)
        break
      case 'courses':
        comparison = b.totalCourses - a.totalCourses
        break
    }
    return pathsSortOrder.value === 'asc' ? -comparison : comparison
  })

  return paths
})

// Paginated paths
const pathsTotalPages = computed(() => Math.ceil(filteredPaths.value.length / pathsItemsPerPage.value))

const paginatedPaths = computed(() => {
  const start = (pathsCurrentPage.value - 1) * pathsItemsPerPage.value
  return filteredPaths.value.slice(start, start + pathsItemsPerPage.value)
})

const pathsPaginationStart = computed(() => {
  return (pathsCurrentPage.value - 1) * pathsItemsPerPage.value + 1
})
const pathsPaginationEnd = computed(() => Math.min(pathsCurrentPage.value * pathsItemsPerPage.value, filteredPaths.value.length))

// Paths pagination functions
function goToPathsPage(page: number) {
  if (page >= 1 && page <= pathsTotalPages.value) {
    pathsCurrentPage.value = page
  }
}

function nextPathsPage() {
  if (pathsCurrentPage.value < pathsTotalPages.value) {
    pathsCurrentPage.value++
  }
}

function prevPathsPage() {
  if (pathsCurrentPage.value > 1) {
    pathsCurrentPage.value--
  }
}

function changePathsItemsPerPage(value: number) {
  pathsItemsPerPage.value = value
  pathsCurrentPage.value = 1
}

// Paths filter functions
function togglePathsLevelFilter(levelId: string) {
  const index = pathsLevelFilter.value.indexOf(levelId)
  if (index > -1) {
    pathsLevelFilter.value.splice(index, 1)
  } else {
    pathsLevelFilter.value.push(levelId)
  }
}

// Active paths filters count
const activePathsFiltersCount = computed(() => {
  let count = 0
  if (pathsSearch.value) count++
  count += pathsLevelFilter.value.length
  if (pathFilter.value !== 'all') count++
  return count
})

// Clear all paths filters
function clearAllPathsFilters() {
  pathsSearch.value = ''
  pathsLevelFilter.value = []
  pathFilter.value = 'all'
  pathsCurrentPage.value = 1
}

// Watch for filter changes to reset pagination
watch([pathFilter, pathsSearch, pathsLevelFilter], () => {
  pathsCurrentPage.value = 1
})

const myEnrolledPaths = computed(() => learningPaths.value.filter(p => p.isEnrolled))

// Certificate stats
const certificateStats = computed(() => ({
  total: earnedCertificates.value.length,
  thisYear: earnedCertificates.value.filter(c => c.date.includes('2024')).length,
  totalHours: earnedCertificates.value.reduce((sum, c) => sum + c.hours, 0),
  avgScore: Math.round(earnedCertificates.value.reduce((sum, c) => sum + c.score, 0) / earnedCertificates.value.length)
}))

// Filtered certificates
const filteredCertificates = computed(() => {
  let certs = [...earnedCertificates.value]

  // Filter by search
  if (certSearch.value.trim()) {
    const search = certSearch.value.toLowerCase()
    certs = certs.filter(c =>
      c.title.toLowerCase().includes(search) ||
      c.course.toLowerCase().includes(search) ||
      c.instructor.toLowerCase().includes(search) ||
      c.skills.some((s: string) => s.toLowerCase().includes(search))
    )
  }

  // Filter by score
  if (certScoreFilter.value !== 'all') {
    const option = certScoreOptions.find(o => o.value === certScoreFilter.value)
    if (option) {
      certs = certs.filter(c => c.score >= option.min && c.score <= option.max)
    }
  }

  // Sort
  certs.sort((a, b) => {
    let comparison = 0
    switch (certSortBy.value) {
      case 'date':
        comparison = new Date(b.date).getTime() - new Date(a.date).getTime()
        break
      case 'score':
        comparison = b.score - a.score
        break
      case 'title':
        comparison = a.title.localeCompare(b.title)
        break
      case 'hours':
        comparison = b.hours - a.hours
        break
    }
    return certSortOrder.value === 'asc' ? -comparison : comparison
  })

  return certs
})

// Certificate pagination
const certTotalPages = computed(() => Math.ceil(filteredCertificates.value.length / certItemsPerPage.value))

const paginatedCertificates = computed(() => {
  const start = (certCurrentPage.value - 1) * certItemsPerPage.value
  return filteredCertificates.value.slice(start, start + certItemsPerPage.value)
})

const certPaginationStart = computed(() => {
  if (filteredCertificates.value.length === 0) return 0
  return (certCurrentPage.value - 1) * certItemsPerPage.value + 1
})

const certPaginationEnd = computed(() => {
  return Math.min(certCurrentPage.value * certItemsPerPage.value, filteredCertificates.value.length)
})

// Active certificate filters count
const activeCertFiltersCount = computed(() => {
  let count = 0
  if (certSearch.value.trim()) count++
  if (certScoreFilter.value !== 'all') count++
  return count
})

// Certificate pagination functions
function goToCertPage(page: number) {
  certCurrentPage.value = page
}

function nextCertPage() {
  if (certCurrentPage.value < certTotalPages.value) {
    certCurrentPage.value++
  }
}

function prevCertPage() {
  if (certCurrentPage.value > 1) {
    certCurrentPage.value--
  }
}

function changeCertItemsPerPage(value: number) {
  certItemsPerPage.value = value
  certCurrentPage.value = 1
}

function clearAllCertFilters() {
  certSearch.value = ''
  certScoreFilter.value = 'all'
  certSortBy.value = 'date'
  certSortOrder.value = 'desc'
  certCurrentPage.value = 1
}

// Reset cert pagination when filters change
watch([certSearch, certScoreFilter], () => {
  certCurrentPage.value = 1
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

function toggleSaveTrending(courseId: number) {
  const course = trendingCourses.value.find(c => c.id === courseId)
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
          <button v-if="currentCourse" @click="navigateToCourse(currentCourse.id)" class="px-5 py-2.5 bg-white text-teal-600 rounded-xl font-semibold text-sm flex items-center gap-2 hover:bg-teal-50 transition-all shadow-lg">
            <i class="fas fa-play"></i>
            Continue Learning
          </button>
        </div>
      </div>
    </div>

    <!-- View Navigation -->
    <div class="view-nav">
      <button
        v-for="view in viewOptions"
        :key="view.id"
        @click="currentView = view.id as any"
        :class="[
          'view-nav-btn',
          currentView === view.id ? 'active' : ''
        ]"
      >
        <i :class="[view.icon, view.color]"></i>
        <span>{{ view.name }}</span>
      </button>
    </div>

    <!-- Main Content -->
    <div class="px-8 py-6">

      <!-- Courses Content -->
      <div v-if="currentView !== 'paths' && currentView !== 'certificates'" class="space-y-6">
        <!-- Continue Learning Section (My Courses view) -->
        <div v-if="currentView === 'my-courses' && inProgressCourses.length > 0" class="continue-learning-section">
          <!-- Section Header -->
          <div class="continue-learning-header">
            <div class="flex items-center gap-4">
              <div class="continue-learning-icon">
                <i class="fas fa-play"></i>
                <div class="icon-pulse"></div>
              </div>
              <div>
                <h2 class="text-xl font-bold text-gray-900">Continue Learning</h2>
                <p class="text-sm text-gray-500">Pick up where you left off</p>
              </div>
            </div>
            <div class="flex items-center gap-3">
              <div class="px-4 py-2 bg-teal-50 rounded-xl flex items-center gap-2">
                <div class="w-2 h-2 bg-teal-500 rounded-full animate-pulse"></div>
                <span class="text-sm font-semibold text-teal-700">{{ inProgressCourses.length }} In Progress</span>
              </div>
            </div>
          </div>

          <!-- Continue Learning Cards -->
          <div class="continue-learning-grid">
            <div
              v-for="(course, index) in inProgressCourses"
              :key="'continue-' + course.id"
              @click="navigateToCourse(course.id)"
              class="continue-card group"
              :style="{ animationDelay: index * 0.1 + 's' }"
            >
              <!-- Card Image Section -->
              <div class="continue-card-image">
                <img :src="course.image" :alt="course.title">
                <div class="image-overlay"></div>

                <!-- Floating Play Button -->
                <div class="play-button-wrapper">
                  <div class="play-button">
                    <i class="fas fa-play"></i>
                  </div>
                </div>

                <!-- Top Actions -->
                <div class="card-top-actions">
                  <button @click.stop="toggleSaveCourse(course.id)" class="save-btn" :class="{ saved: course.saved }">
                    <i :class="course.saved ? 'fas fa-bookmark' : 'far fa-bookmark'"></i>
                  </button>
                </div>

                <!-- Progress Ring -->
                <div class="progress-ring-wrapper">
                  <svg class="progress-ring" viewBox="0 0 36 36">
                    <path class="progress-ring-bg" d="M18 2.0845 a 15.9155 15.9155 0 0 1 0 31.831 a 15.9155 15.9155 0 0 1 0 -31.831" />
                    <path class="progress-ring-fill" :stroke-dasharray="course.progress + ', 100'" d="M18 2.0845 a 15.9155 15.9155 0 0 1 0 31.831 a 15.9155 15.9155 0 0 1 0 -31.831" />
                  </svg>
                  <span class="progress-text">{{ course.progress }}%</span>
                </div>
              </div>

              <!-- Card Content -->
              <div class="continue-card-content">
                <!-- Level & Duration -->
                <div class="card-meta">
                  <span :class="['level-badge', course.levelClass]">
                    <i :class="course.levelClass === 'beginner' ? 'fas fa-seedling' : course.levelClass === 'intermediate' ? 'fas fa-chart-line' : 'fas fa-rocket'"></i>
                    {{ course.level }}
                  </span>
                  <span class="duration-badge">
                    <i class="fas fa-clock"></i>
                    {{ course.duration }}
                  </span>
                </div>

                <!-- Title -->
                <h4 class="course-title">{{ course.title }}</h4>

                <!-- Instructor -->
                <div class="instructor-row">
                  <div class="instructor-avatar" :style="{ background: `linear-gradient(135deg, ${course.gradientClass.includes('teal') ? '#14b8a6' : course.gradientClass.includes('amber') ? '#f59e0b' : '#3b82f6'} 0%, ${course.gradientClass.includes('teal') ? '#0d9488' : course.gradientClass.includes('amber') ? '#d97706' : '#2563eb'} 100%)` }">
                    {{ course.instructorInitials }}
                  </div>
                  <span class="instructor-name">{{ course.instructor }}</span>
                </div>

                <!-- Current Lesson -->
                <div class="current-lesson">
                  <div class="lesson-indicator">
                    <i class="fas fa-play-circle"></i>
                    <span>Up Next</span>
                  </div>
                  <p class="lesson-title">{{ course.syllabus?.find(l => l.current)?.title || 'Continue course' }}</p>
                </div>

                <!-- Progress Bar -->
                <div class="progress-section">
                  <div class="progress-info">
                    <span class="lessons-count">
                      <i class="fas fa-check-circle text-teal-500"></i>
                      {{ course.completedLessons }} of {{ course.totalLessons }} lessons
                    </span>
                    <span class="time-left">~{{ Math.ceil((course.totalLessons - course.completedLessons) * 0.5) }}h left</span>
                  </div>
                  <div class="progress-bar">
                    <div class="progress-fill" :style="{ width: course.progress + '%' }">
                      <div class="progress-glow"></div>
                    </div>
                  </div>
                </div>

                <!-- Action Button -->
                <button class="continue-btn">
                  <span>Continue Learning</span>
                  <i class="fas fa-arrow-right"></i>
                </button>
              </div>
            </div>
          </div>
        </div>

        <!-- Featured Courses Section (Other views) -->
        <div v-if="currentView !== 'my-courses' && enrolledCourses.length > 0" class="featured-courses-section">
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

        <!-- Trending Section (All Courses tab only) -->
        <div v-if="currentView === 'all'" class="trending-section">
          <div class="trending-header">
            <div class="trending-title-wrap">
              <div class="trending-icon">
                <i class="fas fa-fire-flame-curved"></i>
              </div>
              <div>
                <h2 class="trending-title">Trending Now</h2>
                <p class="trending-subtitle">Popular courses this week</p>
              </div>
            </div>
            <button class="trending-see-all">
              See All <i class="fas fa-arrow-right"></i>
            </button>
          </div>
          <div class="trending-grid">
            <div v-for="course in trendingCourses" :key="course.id" class="trending-card">
              <!-- Save Button -->
              <button
                class="trending-save-btn"
                :class="{ 'saved': course.saved }"
                @click.stop="toggleSaveTrending(course.id)"
              >
                <i :class="course.saved ? 'fas fa-bookmark' : 'far fa-bookmark'"></i>
              </button>
              <!-- Trending Rank Badge -->
              <div class="trending-rank">
                <i class="fas fa-fire"></i> #{{ course.trendingRank }} Trending
              </div>
              <!-- Image -->
              <div class="trending-image">
                <img :src="course.image" :alt="course.title" />
                <div class="trending-overlay"></div>
                <span class="trending-duration">{{ course.duration }}</span>
              </div>
              <!-- Content -->
              <div class="trending-content">
                <div class="trending-tags">
                  <span v-for="tag in course.tags" :key="tag" class="trending-tag">{{ tag }}</span>
                  <span :class="['trending-level', course.levelClass]">{{ course.level }}</span>
                </div>
                <h3 class="trending-course-title">{{ course.title }}</h3>
                <div class="trending-instructor">
                  <div class="trending-avatar">{{ course.instructorInitials }}</div>
                  <span>{{ course.instructor }}</span>
                </div>
                <div class="trending-footer">
                  <div class="trending-stats">
                    <span><i class="fas fa-eye"></i> {{ (course.viewsThisWeek / 1000).toFixed(1) }}k views</span>
                    <span><i class="fas fa-star text-amber-400"></i> {{ course.rating }}</span>
                    <span><i class="fas fa-users"></i> {{ (course.students / 1000).toFixed(1) }}k</span>
                  </div>
                  <button class="trending-enroll-btn">
                    <i class="fas fa-plus"></i> Enroll
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Recommended For You Section (My Courses tab only) -->
        <div v-if="currentView === 'my-courses'" class="recommended-section">
          <div class="recommended-header">
            <div class="recommended-title-wrap">
              <div class="recommended-icon">
                <i class="fas fa-lightbulb"></i>
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
                <div :class="[
                  'w-10 h-10 rounded-xl flex items-center justify-center shadow-lg',
                  currentView === 'my-courses' ? 'bg-gradient-to-br from-teal-500 to-teal-600 shadow-teal-200' : 'bg-gradient-to-br from-teal-500 to-teal-600 shadow-teal-200'
                ]">
                  <i :class="currentView === 'my-courses' ? 'fas fa-book-reader text-white text-sm' : 'fas fa-graduation-cap text-white text-sm'"></i>
                </div>
                <div>
                  <span class="block">{{ currentView === 'my-courses' ? 'My Courses' : 'All Courses' }}</span>
                  <span class="text-xs font-medium text-gray-500">{{ displayedCourses.length }} courses available</span>
                </div>
              </h2>
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

              <!-- Enrollment Filter Dropdown -->
              <div class="relative">
                <button
                  @click="showEnrollmentFilter = !showEnrollmentFilter"
                  :class="[
                    'flex items-center gap-2 px-3 py-1.5 rounded-lg text-xs font-medium transition-all border',
                    allCoursesEnrollmentFilter.length > 0 ? 'bg-teal-50 border-teal-200 text-teal-700' : 'bg-white border-gray-200 text-gray-600 hover:bg-gray-50'
                  ]"
                >
                  <i class="fas fa-user-graduate text-sm"></i>
                  <span>{{ allCoursesEnrollmentFilter.length > 0 ? `${allCoursesEnrollmentFilter.length} Enrollment` : 'Enrollment' }}</span>
                  <i :class="showEnrollmentFilter ? 'fas fa-chevron-up' : 'fas fa-chevron-down'" class="text-[10px] ml-1"></i>
                </button>

                <!-- Dropdown Menu -->
                <div
                  v-if="showEnrollmentFilter"
                  class="absolute left-0 top-full mt-2 w-56 bg-white rounded-xl shadow-lg border border-gray-100 py-2 z-50"
                >
                  <div class="px-3 py-1.5 text-xs font-semibold text-gray-400 uppercase tracking-wider">Enrollment</div>
                  <div class="max-h-48 overflow-y-auto">
                    <button
                      v-for="option in courseEnrollmentOptions"
                      :key="option.id"
                      @click="toggleEnrollmentFilterOption(option.id)"
                      :class="[
                        'w-full px-3 py-2 text-left text-sm flex items-center gap-3 transition-colors',
                        allCoursesEnrollmentFilter.includes(option.id) ? 'bg-teal-50 text-teal-700' : 'text-gray-700 hover:bg-gray-50'
                      ]"
                    >
                      <div :class="[
                        'w-4 h-4 rounded border-2 flex items-center justify-center transition-all',
                        allCoursesEnrollmentFilter.includes(option.id) ? 'bg-teal-500 border-teal-500' : 'border-gray-300'
                      ]">
                        <i v-if="allCoursesEnrollmentFilter.includes(option.id)" class="fas fa-check text-white text-[8px]"></i>
                      </div>
                      <span class="flex-1">{{ option.label }}</span>
                    </button>
                  </div>
                  <div class="px-3 py-1.5 text-xs font-semibold text-gray-400 uppercase tracking-wider border-t border-gray-100 mt-2 pt-2">Progress</div>
                  <div class="max-h-48 overflow-y-auto">
                    <button
                      v-for="option in courseProgressOptions"
                      :key="option.id"
                      @click="toggleEnrollmentFilterOption(option.id)"
                      :class="[
                        'w-full px-3 py-2 text-left text-sm flex items-center gap-3 transition-colors',
                        allCoursesEnrollmentFilter.includes(option.id) ? 'bg-teal-50 text-teal-700' : 'text-gray-700 hover:bg-gray-50'
                      ]"
                    >
                      <div :class="[
                        'w-4 h-4 rounded border-2 flex items-center justify-center transition-all',
                        allCoursesEnrollmentFilter.includes(option.id) ? 'bg-teal-500 border-teal-500' : 'border-gray-300'
                      ]">
                        <i v-if="allCoursesEnrollmentFilter.includes(option.id)" class="fas fa-check text-white text-[8px]"></i>
                      </div>
                      <span class="flex-1">{{ option.label }}</span>
                    </button>
                  </div>
                  <div class="my-2 border-t border-gray-100"></div>
                  <div class="px-3 flex gap-2">
                    <button
                      @click="allCoursesEnrollmentFilter = []; showEnrollmentFilter = false"
                      class="flex-1 px-3 py-1.5 text-xs font-medium text-gray-600 bg-gray-100 rounded-lg hover:bg-gray-200 transition-colors"
                    >
                      Clear All
                    </button>
                    <button
                      @click="showEnrollmentFilter = false"
                      class="flex-1 px-3 py-1.5 text-xs font-medium text-white bg-teal-500 rounded-lg hover:bg-teal-600 transition-colors"
                    >
                      Apply
                    </button>
                  </div>
                </div>
                <div v-if="showEnrollmentFilter" @click="showEnrollmentFilter = false" class="fixed inset-0 z-40"></div>
              </div>

              <!-- Saved & Shared Filter Dropdown (My Courses only) -->
              <div v-if="currentView === 'my-courses'" class="relative">
                <button
                  @click="showStatusFilter = !showStatusFilter"
                  :class="[
                    'flex items-center gap-2 px-3 py-1.5 rounded-lg text-xs font-medium transition-all border',
                    selectedStatusFilters.length > 0 ? 'bg-teal-50 border-teal-200 text-teal-700' : 'bg-white border-gray-200 text-gray-600 hover:bg-gray-50'
                  ]"
                >
                  <i class="fas fa-bookmark text-sm"></i>
                  <span>{{ selectedStatusFilters.length > 0 ? `${selectedStatusFilters.length} Saved & Shared` : 'Saved & Shared' }}</span>
                  <i :class="showStatusFilter ? 'fas fa-chevron-up' : 'fas fa-chevron-down'" class="text-[10px] ml-1"></i>
                </button>

                <!-- Dropdown Menu -->
                <div
                  v-if="showStatusFilter"
                  class="absolute left-0 top-full mt-2 w-56 bg-white rounded-xl shadow-lg border border-gray-100 py-2 z-50"
                >
                  <div class="px-3 py-1.5 text-xs font-semibold text-gray-400 uppercase tracking-wider">Filter by Status</div>
                  <div class="max-h-48 overflow-y-auto">
                    <button
                      v-for="option in statusFilterOptions"
                      :key="option.id"
                      @click="toggleStatusFilter(option.id)"
                      :class="[
                        'w-full px-3 py-2 text-left text-sm flex items-center gap-3 transition-colors',
                        isStatusSelected(option.id) ? 'bg-teal-50 text-teal-700' : 'text-gray-700 hover:bg-gray-50'
                      ]"
                    >
                      <div :class="[
                        'w-4 h-4 rounded border-2 flex items-center justify-center transition-all',
                        isStatusSelected(option.id) ? 'bg-teal-500 border-teal-500' : 'border-gray-300'
                      ]">
                        <i v-if="isStatusSelected(option.id)" class="fas fa-check text-white text-[8px]"></i>
                      </div>
                      <i :class="[option.icon, option.color]"></i>
                      <span class="flex-1">{{ option.label }}</span>
                    </button>
                  </div>

                  <div class="my-2 border-t border-gray-100"></div>

                  <div class="px-3 flex gap-2">
                    <button
                      @click="selectedStatusFilters = []; showStatusFilter = false"
                      class="flex-1 px-3 py-1.5 text-xs font-medium text-gray-600 bg-gray-100 rounded-lg hover:bg-gray-200 transition-colors"
                    >
                      Clear All
                    </button>
                    <button
                      @click="showStatusFilter = false"
                      class="flex-1 px-3 py-1.5 text-xs font-medium text-white bg-teal-500 rounded-lg hover:bg-teal-600 transition-colors"
                    >
                      Apply
                    </button>
                  </div>
                </div>

                <!-- Click outside to close -->
                <div v-if="showStatusFilter" @click="showStatusFilter = false" class="fixed inset-0 z-40"></div>
              </div>

              <!-- Sort Options with Order Toggle -->
              <div class="relative ml-auto flex items-center">
                <button
                  @click="showAllCoursesSortDropdown = !showAllCoursesSortDropdown"
                  class="flex items-center gap-2 px-3 py-1.5 rounded-l-lg text-xs font-medium transition-all border border-r-0 bg-white border-gray-200 text-gray-600 hover:bg-gray-50"
                >
                  <i :class="[allCoursesSortOptions.find(o => o.value === allCoursesSortBy)?.icon, 'text-sm text-teal-500']"></i>
                  <span>{{ allCoursesSortOptions.find(o => o.value === allCoursesSortBy)?.label }}</span>
                  <i :class="showAllCoursesSortDropdown ? 'fas fa-chevron-up' : 'fas fa-chevron-down'" class="text-[10px] ml-1"></i>
                </button>
                <button
                  @click="allCoursesSortOrder = allCoursesSortOrder === 'asc' ? 'desc' : 'asc'"
                  class="flex items-center justify-center w-8 h-8 rounded-r-lg text-xs font-medium transition-all border bg-white border-gray-200 text-gray-600 hover:bg-gray-50 hover:text-teal-600"
                  :title="allCoursesSortOrder === 'asc' ? 'Ascending order - Click for descending' : 'Descending order - Click for ascending'"
                >
                  <i :class="allCoursesSortOrder === 'asc' ? 'fas fa-arrow-up' : 'fas fa-arrow-down'" class="text-sm text-teal-500"></i>
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
              <div class="flex items-center gap-0.5 bg-white border border-gray-200 rounded-lg p-1">
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

          <!-- Active Filters Bar -->
          <div v-if="activeFiltersCount > 0" class="flex items-center gap-3 mb-4 p-3 bg-white rounded-xl border border-gray-100 shadow-sm mx-4 mt-4">
            <div class="flex items-center gap-2 px-2 py-1 bg-gray-100 rounded-lg">
              <i class="fas fa-filter text-gray-400 text-xs"></i>
              <span class="text-xs font-medium text-gray-600">Active Filters</span>
            </div>
            <div class="flex flex-wrap gap-2 flex-1">
              <!-- Search Filter -->
              <span v-if="allCoursesSearch" class="px-2.5 py-1 bg-gray-100 text-gray-700 rounded-lg text-xs font-medium flex items-center gap-1.5 border border-gray-200">
                <i class="fas fa-search text-[10px]"></i>
                "{{ allCoursesSearch }}"
                <button @click="allCoursesSearch = ''" class="ml-1 hover:text-gray-900 hover:bg-gray-200 rounded-full w-4 h-4 flex items-center justify-center"><i class="fas fa-times text-[10px]"></i></button>
              </span>
              <!-- Level Filters -->
              <span
                v-for="level in allCoursesLevelFilter"
                :key="'level-' + level"
                class="px-2.5 py-1 bg-teal-50 text-teal-700 rounded-lg text-xs font-medium flex items-center gap-1.5 border border-teal-100"
              >
                <i class="fas fa-signal text-[10px]"></i>
                {{ level.charAt(0).toUpperCase() + level.slice(1) }}
                <button @click="toggleLevelFilter(level)" class="ml-1 hover:text-teal-900 hover:bg-teal-100 rounded-full w-4 h-4 flex items-center justify-center"><i class="fas fa-times text-[10px]"></i></button>
              </span>
              <!-- Category Filters -->
              <span
                v-for="cat in allCoursesCategoryFilter"
                :key="'cat-' + cat"
                class="px-2.5 py-1 bg-teal-50 text-teal-700 rounded-lg text-xs font-medium flex items-center gap-1.5 border border-teal-100"
              >
                <i class="fas fa-layer-group text-[10px]"></i>
                {{ cat }}
                <button @click="toggleCategoryFilter(cat)" class="ml-1 hover:text-teal-900 hover:bg-teal-100 rounded-full w-4 h-4 flex items-center justify-center"><i class="fas fa-times text-[10px]"></i></button>
              </span>
              <!-- Enrollment/Progress Filters -->
              <span
                v-for="option in allCoursesEnrollmentFilter"
                :key="'enrollment-' + option"
                class="px-2.5 py-1 bg-blue-50 text-blue-700 rounded-lg text-xs font-medium flex items-center gap-1.5 border border-blue-100"
              >
                <i class="fas fa-tasks text-[10px]"></i>
                {{ getEnrollmentLabel(option) }}
                <button @click="toggleEnrollmentFilterOption(option)" class="ml-1 hover:text-blue-900 hover:bg-blue-100 rounded-full w-4 h-4 flex items-center justify-center"><i class="fas fa-times text-[10px]"></i></button>
              </span>
              <!-- Status Filters (Saved & Shared) -->
              <span
                v-for="status in selectedStatusFilters"
                :key="'status-' + status"
                class="px-2.5 py-1 bg-amber-50 text-amber-700 rounded-lg text-xs font-medium flex items-center gap-1.5 border border-amber-100"
              >
                <i :class="[status === 'saved' ? 'fas fa-bookmark' : 'fas fa-share-alt', 'text-[10px]']"></i>
                {{ status === 'saved' ? 'My Saved' : 'Shared with me' }}
                <button @click="toggleStatusFilter(status)" class="ml-1 hover:text-amber-900 hover:bg-amber-100 rounded-full w-4 h-4 flex items-center justify-center"><i class="fas fa-times text-[10px]"></i></button>
              </span>
            </div>
            <button @click="clearAllFilters" class="px-3 py-1.5 text-xs font-medium text-red-600 hover:text-red-700 hover:bg-red-50 rounded-lg transition-colors flex items-center gap-1.5">
              <i class="fas fa-times-circle"></i>
              Clear all
            </button>
          </div>

          <!-- Main Content Area -->
          <div class="p-4">
            <!-- Grid View -->
            <div v-if="allCoursesViewMode === 'grid'" class="all-courses-grid">
              <div v-for="course in paginatedCourses" :key="course.id"
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
                  <div class="absolute top-2 left-2 flex gap-1.5 z-10">
                    <span v-if="course.enrolled" :class="['px-2 py-0.5 text-[10px] font-semibold rounded-full', course.statusClass === 'completed' ? 'bg-green-500 text-white' : course.statusClass === 'in-progress' ? 'bg-blue-500 text-white' : 'bg-gray-500 text-white']">
                      {{ course.status }}
                    </span>
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

                  <!-- Progress Bar for Enrolled Courses -->
                  <div v-if="course.enrolled" class="absolute bottom-0 left-0 right-0 h-1.5 bg-black/30 z-10">
                    <div class="h-full bg-gradient-to-r from-teal-400 to-teal-500 transition-all" :style="{ width: course.progress + '%' }"></div>
                  </div>
                </div>

                <!-- Card Body -->
                <div class="p-4">
                  <!-- Meta Info -->
                  <div class="flex items-center gap-3 text-[11px] text-gray-400 mb-2">
                    <span class="flex items-center gap-1">
                      <i class="fas fa-play-circle text-[9px]"></i>
                      {{ course.enrolled ? `${course.completedLessons}/${course.lessons}` : course.lessons }} lessons
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
                      <span v-if="course.enrolled" class="font-bold text-teal-600 text-sm">
                        {{ course.progress }}%
                      </span>
                      <span v-else class="flex items-center gap-1 hover:text-teal-500 transition-colors">
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
              <div v-for="course in paginatedCourses" :key="course.id"
                   class="all-course-list-item cursor-pointer group">
                <!-- Thumbnail -->
                <div class="all-course-list-thumbnail">
                  <img :src="course.image" :alt="course.title" class="absolute inset-0 w-full h-full object-cover rounded-lg" />
                  <div class="absolute inset-0 bg-gradient-to-t from-black/40 to-transparent rounded-lg"></div>
                  <div class="all-course-list-play-overlay">
                    <i class="fas fa-play"></i>
                  </div>
                  <div class="all-course-list-duration">{{ course.duration }}</div>
                  <!-- Progress Bar for Enrolled Courses -->
                  <div v-if="course.enrolled" class="absolute bottom-0 left-0 right-0 h-1 bg-black/30 rounded-b-lg">
                    <div class="h-full bg-gradient-to-r from-teal-400 to-teal-500 transition-all rounded-bl-lg" :style="{ width: course.progress + '%' }"></div>
                  </div>
                </div>

                <!-- Content -->
                <div class="all-course-list-content">
                  <div class="all-course-list-header">
                    <div class="all-course-list-badges">
                      <span :class="['all-course-level-badge', course.levelClass]">{{ course.level }}</span>
                      <span class="all-course-category-badge">{{ course.category }}</span>
                      <span v-if="course.enrolled" :class="['px-2 py-0.5 text-[10px] font-semibold rounded-full', course.statusClass === 'completed' ? 'bg-green-100 text-green-700' : course.statusClass === 'in-progress' ? 'bg-blue-100 text-blue-700' : 'bg-gray-100 text-gray-700']">
                        {{ course.status }}
                      </span>
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
                      <span><i class="fas fa-play-circle"></i> {{ course.enrolled ? `${course.completedLessons}/${course.lessons}` : course.lessons }} lessons</span>
                      <span><i class="fas fa-star text-amber-400"></i> {{ course.rating }}</span>
                      <span v-if="course.enrolled" class="font-semibold text-teal-600">{{ course.progress }}%</span>
                      <span v-else><i class="fas fa-users"></i> {{ (course.students / 1000).toFixed(1) }}k</span>
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
                  <button v-if="!course.enrolled" class="all-course-list-action-btn" title="Enroll">
                    <i class="fas fa-plus"></i>
                  </button>
                  <button v-else class="all-course-list-action-btn text-teal-500" :title="(course.progress ?? 0) > 0 && (course.progress ?? 0) < 100 ? 'Continue' : (course.progress ?? 0) === 100 ? 'Review' : 'Start'">
                    <i :class="(course.progress ?? 0) > 0 && (course.progress ?? 0) < 100 ? 'fas fa-play' : (course.progress ?? 0) === 100 ? 'fas fa-redo' : 'fas fa-play'"></i>
                  </button>
                </div>
              </div>
            </div>

            <!-- Empty State -->
            <div v-if="displayedCourses.length === 0" class="all-courses-empty">
              <div class="all-courses-empty-icon">
                <i class="fas fa-search"></i>
              </div>
              <h3 class="all-courses-empty-title">No courses found</h3>
              <p class="all-courses-empty-text">{{ currentView === 'my-courses' ? 'You haven\'t enrolled, saved, or been shared any courses yet' : 'Try adjusting your filters or search query' }}</p>
              <button @click="allCoursesSearch = ''; allCoursesLevelFilter = []; allCoursesCategoryFilter = []; allCoursesEnrollmentFilter = []; selectedStatusFilters = []" class="all-courses-clear-btn">
                <i class="fas fa-undo mr-2"></i> Clear Filters
              </button>
            </div>

            <!-- Pagination Footer -->
            <div v-if="displayedCourses.length > 0" class="mt-4 px-4 py-3 bg-white rounded-2xl border border-gray-100 shadow-sm">
              <div class="flex items-center justify-between flex-wrap gap-3">
                <!-- Left: Stats & Items Per Page -->
                <div class="flex items-center gap-4 flex-wrap">
                  <span class="text-xs text-gray-500">
                    Showing <span class="font-semibold text-gray-700">{{ coursesPaginationStart }}</span>
                    to <span class="font-semibold text-gray-700">{{ coursesPaginationEnd }}</span>
                    of <span class="font-semibold text-gray-700">{{ displayedCourses.length }}</span> courses
                  </span>

                  <!-- Items Per Page Selector -->
                  <div class="flex items-center gap-2">
                    <span class="text-xs text-gray-500">Show:</span>
                    <select
                      v-model="coursesItemsPerPage"
                      @change="changeCoursesItemsPerPage(Number(($event.target as HTMLSelectElement).value))"
                      class="text-xs px-2 py-1.5 rounded-lg border border-gray-200 bg-white text-gray-700 focus:outline-none focus:ring-2 focus:ring-teal-500 focus:border-teal-500 cursor-pointer"
                    >
                      <option v-for="option in coursesItemsPerPageOptions" :key="option" :value="option">
                        {{ option }}
                      </option>
                    </select>
                    <span class="text-xs text-gray-500">per page</span>
                  </div>
                </div>

                <!-- Right: Pagination Controls -->
                <div class="flex items-center gap-2">
                  <button
                    @click="prevCoursesPage"
                    :disabled="coursesCurrentPage === 1"
                    :class="[
                      'px-3 py-1.5 text-xs rounded-lg transition-colors flex items-center gap-1.5 border',
                      coursesCurrentPage === 1
                        ? 'text-gray-300 border-gray-100 cursor-not-allowed'
                        : 'text-gray-500 hover:text-gray-700 hover:bg-gray-100 border-gray-200'
                    ]"
                  >
                    <i class="fas fa-chevron-left text-[10px]"></i>
                    Previous
                  </button>

                  <!-- Page Numbers -->
                  <div class="flex items-center gap-1">
                    <template v-for="page in coursesTotalPages" :key="page">
                      <button
                        v-if="page === 1 || page === coursesTotalPages || (page >= coursesCurrentPage - 1 && page <= coursesCurrentPage + 1)"
                        @click="goToCoursesPage(page)"
                        :class="[
                          'w-8 h-8 text-xs rounded-lg transition-colors flex items-center justify-center',
                          page === coursesCurrentPage
                            ? 'font-medium text-teal-600 bg-teal-50'
                            : 'text-gray-500 hover:text-gray-700 hover:bg-gray-100'
                        ]"
                      >
                        {{ page }}
                      </button>
                      <span
                        v-else-if="page === coursesCurrentPage - 2 || page === coursesCurrentPage + 2"
                        class="text-xs text-gray-400 px-1"
                      >...</span>
                    </template>
                  </div>

                  <button
                    @click="nextCoursesPage"
                    :disabled="coursesCurrentPage === coursesTotalPages"
                    :class="[
                      'px-3 py-1.5 text-xs rounded-lg transition-colors flex items-center gap-1.5 border',
                      coursesCurrentPage === coursesTotalPages
                        ? 'text-gray-300 border-gray-100 cursor-not-allowed'
                        : 'text-gray-500 hover:text-gray-700 hover:bg-gray-100 border-gray-200'
                    ]"
                  >
                    Next
                    <i class="fas fa-chevron-right text-[10px]"></i>
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Learning Paths Section -->
      <div v-if="currentView === 'paths'" class="space-y-6">
        <!-- My Enrolled Paths - Premium Section -->
        <div v-if="myEnrolledPaths.length > 0" class="my-paths-section">
          <div class="my-paths-header">
            <div class="my-paths-title-wrap">
              <div class="my-paths-icon">
                <i class="fas fa-road"></i>
              </div>
              <div>
                <h2 class="my-paths-title">My Learning Paths</h2>
                <p class="my-paths-subtitle">Continue your journey • {{ myEnrolledPaths.length }} active paths</p>
              </div>
            </div>
            <button class="my-paths-see-all">
              View All <i class="fas fa-arrow-right"></i>
            </button>
          </div>

          <div class="my-paths-grid">
            <div v-for="path in myEnrolledPaths" :key="path.id" class="my-path-card">
              <!-- Progress Ring Badge -->
              <div class="my-path-progress-ring">
                <svg viewBox="0 0 36 36">
                  <circle cx="18" cy="18" r="16" fill="none" stroke="rgba(20, 184, 166, 0.15)" stroke-width="3"/>
                  <circle cx="18" cy="18" r="16" fill="none" stroke="#14b8a6" stroke-width="3" stroke-linecap="round"
                          :stroke-dasharray="100" :stroke-dashoffset="100 - path.progress" class="progress-ring-circle"/>
                </svg>
                <span class="progress-ring-text">{{ path.progress }}%</span>
              </div>

              <!-- Path Image -->
              <div class="my-path-image">
                <img :src="path.image" :alt="path.title">
                <div class="my-path-overlay"></div>
                <div class="my-path-badges">
                  <span :class="['my-path-level', path.levelClass]">{{ path.level }}</span>
                  <span class="my-path-enrolled-badge"><i class="fas fa-check-circle"></i> Enrolled</span>
                </div>
              </div>

              <!-- Path Content -->
              <div class="my-path-content">
                <!-- Icon & Title -->
                <div class="my-path-header-info">
                  <div class="my-path-icon-wrap" :style="{ backgroundColor: path.color }">
                    <i :class="[path.icon, 'text-white']"></i>
                  </div>
                  <div class="my-path-title-section">
                    <h3 class="my-path-title">{{ path.title }}</h3>
                    <p class="my-path-activity" v-if="path.lastActivity">
                      <i class="fas fa-clock"></i> Last activity: {{ path.lastActivity }}
                    </p>
                  </div>
                </div>

                <!-- Course Progress Breakdown -->
                <div class="my-path-courses">
                  <div class="my-path-courses-header">
                    <span class="courses-count">{{ path.completedCourses }}/{{ path.totalCourses }} completed</span>
                    <div class="courses-progress-bar">
                      <div class="courses-progress-fill" :style="{ width: (path.completedCourses / path.totalCourses * 100) + '%' }"></div>
                    </div>
                  </div>
                  <div class="my-path-courses-list">
                    <div v-for="(course, idx) in path.courses.slice(0, 3)" :key="idx"
                         :class="['course-item', { completed: course.completed }]">
                      <i :class="[course.completed ? 'fas fa-check-circle' : 'far fa-circle']"></i>
                      <span>{{ course.title }}</span>
                    </div>
                    <div v-if="path.courses.length > 3" class="courses-more">
                      +{{ path.courses.length - 3 }} more courses
                    </div>
                  </div>
                </div>

                <!-- Skills Tags -->
                <div class="my-path-skills">
                  <span v-for="skill in path.skills.slice(0, 3)" :key="skill"
                        class="skill-tag" :style="{ backgroundColor: path.color + '15', color: path.color }">
                    {{ skill }}
                  </span>
                  <span v-if="path.skills.length > 3" class="skill-more">+{{ path.skills.length - 3 }}</span>
                </div>

                <!-- Footer -->
                <div class="my-path-footer">
                  <div class="my-path-stats">
                    <span><i class="fas fa-clock"></i> {{ path.duration }}</span>
                    <span><i class="fas fa-star"></i> {{ path.rating }}</span>
                    <span><i class="fas fa-book"></i> {{ path.totalCourses }} courses</span>
                  </div>
                  <button class="my-path-continue-btn">
                    <i class="fas fa-play"></i>
                    Continue
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Explore Learning Paths Section (Using All Courses Card Style) -->
        <div class="all-courses-wrapper bg-white rounded-2xl shadow-sm border border-gray-100 overflow-hidden">
          <!-- Section Header / Toolbar -->
          <div class="border-b border-gray-100">
            <!-- Top Row - Title -->
            <div class="px-4 py-3 flex items-center justify-between">
              <h2 class="text-lg font-bold text-gray-900 flex items-center gap-3">
                <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-teal-500 to-teal-600 flex items-center justify-center shadow-lg shadow-teal-200">
                  <i class="fas fa-route text-white text-sm"></i>
                </div>
                <div>
                  <span class="block">Explore Learning Paths</span>
                  <span class="text-xs font-medium text-gray-500">{{ filteredPaths.length }} learning paths available</span>
                </div>
              </h2>
            </div>

            <!-- Bottom Row - Search, Filters, View Options -->
            <div class="px-4 py-2 bg-gray-50/50 flex flex-wrap items-center gap-3">
              <!-- Search -->
              <div class="min-w-[200px] max-w-md relative">
                <i class="fas fa-search absolute left-3 top-1/2 -translate-y-1/2 text-gray-400 text-sm"></i>
                <input
                  v-model="pathsSearch"
                  type="text"
                  placeholder="Search learning paths..."
                  class="w-full pl-9 pr-4 py-2 bg-white border border-gray-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-teal-500 focus:border-transparent transition-all"
                >
                <button v-if="pathsSearch" @click="pathsSearch = ''" class="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600">
                  <i class="fas fa-times text-xs"></i>
                </button>
              </div>

              <!-- Level Filter Dropdown -->
              <div class="relative">
                <button
                  @click="showPathsLevelFilter = !showPathsLevelFilter"
                  :class="[
                    'flex items-center gap-2 px-3 py-1.5 rounded-lg text-xs font-medium transition-all border',
                    pathsLevelFilter.length > 0 ? 'bg-teal-50 border-teal-200 text-teal-700' : 'bg-white border-gray-200 text-gray-600 hover:bg-gray-50'
                  ]"
                >
                  <i class="fas fa-layer-group text-sm"></i>
                  <span>{{ pathsLevelFilter.length > 0 ? `${pathsLevelFilter.length} Levels` : 'Level' }}</span>
                  <i :class="showPathsLevelFilter ? 'fas fa-chevron-up' : 'fas fa-chevron-down'" class="text-[10px] ml-1"></i>
                </button>

                <!-- Dropdown Menu -->
                <div
                  v-if="showPathsLevelFilter"
                  class="absolute left-0 top-full mt-2 w-56 bg-white rounded-xl shadow-lg border border-gray-100 py-2 z-50"
                >
                  <div class="px-3 py-1.5 text-xs font-semibold text-gray-400 uppercase tracking-wider">Select Levels</div>
                  <div class="max-h-48 overflow-y-auto">
                    <button
                      v-for="level in pathLevelOptions"
                      :key="level.id"
                      @click="togglePathsLevelFilter(level.id)"
                      :class="[
                        'w-full px-3 py-2 text-left text-sm flex items-center gap-3 transition-colors',
                        pathsLevelFilter.includes(level.id) ? 'bg-teal-50 text-teal-700' : 'text-gray-700 hover:bg-gray-50'
                      ]"
                    >
                      <div :class="[
                        'w-4 h-4 rounded border-2 flex items-center justify-center transition-all',
                        pathsLevelFilter.includes(level.id) ? 'bg-teal-500 border-teal-500' : 'border-gray-300'
                      ]">
                        <i v-if="pathsLevelFilter.includes(level.id)" class="fas fa-check text-white text-[8px]"></i>
                      </div>
                      <span class="flex-1">{{ level.label }}</span>
                    </button>
                  </div>
                  <div class="my-2 border-t border-gray-100"></div>
                  <div class="px-3 flex gap-2">
                    <button
                      @click="pathsLevelFilter = []; showPathsLevelFilter = false"
                      class="flex-1 px-3 py-1.5 text-xs font-medium text-gray-600 bg-gray-100 rounded-lg hover:bg-gray-200 transition-colors"
                    >
                      Clear All
                    </button>
                    <button
                      @click="showPathsLevelFilter = false"
                      class="flex-1 px-3 py-1.5 text-xs font-medium text-white bg-teal-500 rounded-lg hover:bg-teal-600 transition-colors"
                    >
                      Apply
                    </button>
                  </div>
                </div>
                <div v-if="showPathsLevelFilter" @click="showPathsLevelFilter = false" class="fixed inset-0 z-40"></div>
              </div>

              <!-- Enrollment Filter Dropdown -->
              <div class="relative">
                <button
                  @click="showPathsEnrollmentFilter = !showPathsEnrollmentFilter"
                  :class="[
                    'flex items-center gap-2 px-3 py-1.5 rounded-lg text-xs font-medium transition-all border',
                    pathFilter !== 'all' ? 'bg-blue-50 border-blue-200 text-blue-700' : 'bg-white border-gray-200 text-gray-600 hover:bg-gray-50'
                  ]"
                >
                  <i class="fas fa-route text-sm"></i>
                  <span>{{ pathFilter === 'all' ? 'All Paths' : pathFilter === 'enrolled' ? 'My Enrolled' : 'Available' }}</span>
                  <i :class="showPathsEnrollmentFilter ? 'fas fa-chevron-up' : 'fas fa-chevron-down'" class="text-[10px] ml-1"></i>
                </button>
                <!-- Dropdown Panel -->
                <div
                  v-if="showPathsEnrollmentFilter"
                  class="absolute left-0 top-full mt-2 w-48 bg-white rounded-xl shadow-lg border border-gray-100 py-2 z-50"
                >
                  <div class="px-3 py-1.5 text-xs font-semibold text-gray-400 uppercase tracking-wider">Filter by Status</div>
                  <div class="max-h-64 overflow-y-auto">
                    <button
                      @click="pathFilter = 'all'; showPathsEnrollmentFilter = false"
                      :class="[
                        'w-full px-3 py-2 text-left text-sm flex items-center gap-3 transition-colors',
                        pathFilter === 'all' ? 'bg-blue-50 text-blue-700' : 'text-gray-700 hover:bg-gray-50'
                      ]"
                    >
                      <div :class="[
                        'w-4 h-4 rounded-full border-2 flex items-center justify-center transition-all',
                        pathFilter === 'all' ? 'bg-blue-500 border-blue-500' : 'border-gray-300'
                      ]">
                        <i v-if="pathFilter === 'all'" class="fas fa-circle text-white text-[6px]"></i>
                      </div>
                      <i class="fas fa-layer-group text-sm w-4" :class="pathFilter === 'all' ? 'text-blue-500' : 'text-gray-400'"></i>
                      <span class="flex-1">All Paths</span>
                    </button>
                    <button
                      @click="pathFilter = 'enrolled'; showPathsEnrollmentFilter = false"
                      :class="[
                        'w-full px-3 py-2 text-left text-sm flex items-center gap-3 transition-colors',
                        pathFilter === 'enrolled' ? 'bg-blue-50 text-blue-700' : 'text-gray-700 hover:bg-gray-50'
                      ]"
                    >
                      <div :class="[
                        'w-4 h-4 rounded-full border-2 flex items-center justify-center transition-all',
                        pathFilter === 'enrolled' ? 'bg-blue-500 border-blue-500' : 'border-gray-300'
                      ]">
                        <i v-if="pathFilter === 'enrolled'" class="fas fa-circle text-white text-[6px]"></i>
                      </div>
                      <i class="fas fa-user-check text-sm w-4" :class="pathFilter === 'enrolled' ? 'text-blue-500' : 'text-gray-400'"></i>
                      <span class="flex-1">My Enrolled</span>
                    </button>
                    <button
                      @click="pathFilter = 'available'; showPathsEnrollmentFilter = false"
                      :class="[
                        'w-full px-3 py-2 text-left text-sm flex items-center gap-3 transition-colors',
                        pathFilter === 'available' ? 'bg-blue-50 text-blue-700' : 'text-gray-700 hover:bg-gray-50'
                      ]"
                    >
                      <div :class="[
                        'w-4 h-4 rounded-full border-2 flex items-center justify-center transition-all',
                        pathFilter === 'available' ? 'bg-blue-500 border-blue-500' : 'border-gray-300'
                      ]">
                        <i v-if="pathFilter === 'available'" class="fas fa-circle text-white text-[6px]"></i>
                      </div>
                      <i class="fas fa-unlock text-sm w-4" :class="pathFilter === 'available' ? 'text-blue-500' : 'text-gray-400'"></i>
                      <span class="flex-1">Available</span>
                    </button>
                  </div>
                </div>
                <div v-if="showPathsEnrollmentFilter" @click="showPathsEnrollmentFilter = false" class="fixed inset-0 z-40"></div>
              </div>

              <!-- Sort Options with Order Toggle -->
              <div class="relative ml-auto flex items-center">
                <button
                  @click="showPathsSortDropdown = !showPathsSortDropdown"
                  class="flex items-center gap-2 px-3 py-1.5 rounded-l-lg text-xs font-medium transition-all border border-r-0 bg-white border-gray-200 text-gray-600 hover:bg-gray-50"
                >
                  <i :class="[pathsSortOptions.find(o => o.value === pathsSortBy)?.icon, 'text-sm text-teal-500']"></i>
                  <span>{{ pathsSortOptions.find(o => o.value === pathsSortBy)?.label }}</span>
                  <i :class="showPathsSortDropdown ? 'fas fa-chevron-up' : 'fas fa-chevron-down'" class="text-[10px] ml-1"></i>
                </button>
                <button
                  @click="pathsSortOrder = pathsSortOrder === 'asc' ? 'desc' : 'asc'"
                  class="flex items-center justify-center w-8 h-8 rounded-r-lg text-xs font-medium transition-all border bg-white border-gray-200 text-gray-600 hover:bg-gray-50 hover:text-teal-600"
                  :title="pathsSortOrder === 'asc' ? 'Ascending order - Click for descending' : 'Descending order - Click for ascending'"
                >
                  <i :class="pathsSortOrder === 'asc' ? 'fas fa-arrow-up' : 'fas fa-arrow-down'" class="text-sm text-teal-500"></i>
                </button>

                <!-- Dropdown Menu -->
                <div
                  v-if="showPathsSortDropdown"
                  class="absolute left-0 top-full mt-2 w-48 bg-white rounded-xl shadow-lg border border-gray-100 py-2 z-50"
                >
                  <div class="px-3 py-1.5 text-xs font-semibold text-gray-400 uppercase tracking-wider">Sort By</div>
                  <div class="max-h-64 overflow-y-auto">
                    <button
                      v-for="option in pathsSortOptions"
                      :key="option.value"
                      @click="pathsSortBy = option.value; showPathsSortDropdown = false"
                      :class="[
                        'w-full px-3 py-2 text-left text-sm flex items-center gap-3 transition-colors',
                        pathsSortBy === option.value ? 'bg-teal-50 text-teal-700' : 'text-gray-700 hover:bg-gray-50'
                      ]"
                    >
                      <i :class="[option.icon, 'text-sm w-4', pathsSortBy === option.value ? 'text-teal-500' : 'text-gray-400']"></i>
                      <span class="flex-1">{{ option.label }}</span>
                      <i v-if="pathsSortBy === option.value" class="fas fa-check text-teal-500 text-xs"></i>
                    </button>
                  </div>
                </div>
                <div v-if="showPathsSortDropdown" @click="showPathsSortDropdown = false" class="fixed inset-0 z-40"></div>
              </div>

              <!-- View Toggle -->
              <div class="flex items-center gap-0.5 bg-white border border-gray-200 rounded-lg p-1">
                <button
                  @click="pathsViewMode = 'grid'"
                  :class="['px-2.5 py-1 rounded-md transition-all', pathsViewMode === 'grid' ? 'bg-teal-500 text-white' : 'text-gray-500 hover:bg-gray-100']"
                  title="Grid view"
                >
                  <i class="fas fa-th-large text-xs"></i>
                </button>
                <button
                  @click="pathsViewMode = 'list'"
                  :class="['px-2.5 py-1 rounded-md transition-all', pathsViewMode === 'list' ? 'bg-teal-500 text-white' : 'text-gray-500 hover:bg-gray-100']"
                  title="List view"
                >
                  <i class="fas fa-list text-xs"></i>
                </button>
              </div>
            </div>
          </div>

          <!-- Active Filters Bar -->
          <div v-if="activePathsFiltersCount > 0" class="flex items-center gap-3 mb-4 p-3 bg-white rounded-xl border border-gray-100 shadow-sm mx-4 mt-4">
            <div class="flex items-center gap-2 px-2 py-1 bg-gray-100 rounded-lg">
              <i class="fas fa-filter text-gray-400 text-xs"></i>
              <span class="text-xs font-medium text-gray-600">Active Filters</span>
            </div>
            <div class="flex flex-wrap gap-2 flex-1">
              <!-- Search Filter -->
              <span v-if="pathsSearch" class="px-2.5 py-1 bg-gray-100 text-gray-700 rounded-lg text-xs font-medium flex items-center gap-1.5 border border-gray-200">
                <i class="fas fa-search text-[10px]"></i>
                "{{ pathsSearch }}"
                <button @click="pathsSearch = ''" class="ml-1 hover:text-gray-900 hover:bg-gray-200 rounded-full w-4 h-4 flex items-center justify-center"><i class="fas fa-times text-[10px]"></i></button>
              </span>
              <!-- Level Filters -->
              <span
                v-for="level in pathsLevelFilter"
                :key="'path-level-' + level"
                class="px-2.5 py-1 bg-teal-50 text-teal-700 rounded-lg text-xs font-medium flex items-center gap-1.5 border border-teal-100"
              >
                <i class="fas fa-signal text-[10px]"></i>
                {{ level.charAt(0).toUpperCase() + level.slice(1) }}
                <button @click="togglePathsLevelFilter(level)" class="ml-1 hover:text-teal-900 hover:bg-teal-100 rounded-full w-4 h-4 flex items-center justify-center"><i class="fas fa-times text-[10px]"></i></button>
              </span>
              <!-- Enrollment Filter -->
              <span v-if="pathFilter !== 'all'" class="px-2.5 py-1 bg-blue-50 text-blue-700 rounded-lg text-xs font-medium flex items-center gap-1.5 border border-blue-100">
                <i class="fas fa-user-graduate text-[10px]"></i>
                {{ pathFilter === 'enrolled' ? 'My Enrolled' : 'Available' }}
                <button @click="pathFilter = 'all'" class="ml-1 hover:text-blue-900 hover:bg-blue-100 rounded-full w-4 h-4 flex items-center justify-center"><i class="fas fa-times text-[10px]"></i></button>
              </span>
            </div>
            <button @click="clearAllPathsFilters" class="px-3 py-1.5 text-xs font-medium text-red-600 hover:text-red-700 hover:bg-red-50 rounded-lg transition-colors flex items-center gap-1.5">
              <i class="fas fa-times-circle"></i>
              Clear all
            </button>
          </div>

          <!-- Paths Grid -->
          <div class="p-4">
            <div class="all-courses-grid">
              <div v-for="path in paginatedPaths" :key="path.id"
                   class="all-course-card group bg-white rounded-2xl overflow-hidden cursor-pointer transition-all duration-300 hover:-translate-y-1.5 border border-gray-100 shadow-sm hover:shadow-lg hover:border-teal-200">
                <!-- Card Thumbnail -->
                <div class="relative aspect-video">
                  <img :src="path.image" :alt="path.title" class="absolute inset-0 w-full h-full object-cover" />
                  <div class="absolute inset-0 bg-gradient-to-t from-black/50 via-black/10 to-transparent"></div>

                  <!-- Save Button -->
                  <button
                    class="absolute top-2 right-2 w-8 h-8 rounded-full flex items-center justify-center transition-all z-10 bg-white/90 text-gray-600 opacity-0 group-hover:opacity-100 hover:bg-teal-500 hover:text-white"
                  >
                    <i class="far fa-bookmark text-sm"></i>
                  </button>

                  <!-- Badges -->
                  <div class="absolute top-2 left-2 flex gap-1.5 z-10">
                    <span v-if="path.isEnrolled" :class="['px-2 py-0.5 text-[10px] font-semibold rounded-full', path.progress === 100 ? 'bg-green-500 text-white' : path.progress > 0 ? 'bg-blue-500 text-white' : 'bg-gray-500 text-white']">
                      {{ path.progress === 100 ? 'Completed' : path.progress > 0 ? 'In Progress' : 'Not Started' }}
                    </span>
                    <span v-if="path.enrolled > 1000" class="all-course-new-badge" style="background: linear-gradient(135deg, #f97316 0%, #ea580c 100%);">
                      <i class="fas fa-fire"></i> Popular
                    </span>
                  </div>

                  <!-- Duration Badge -->
                  <div class="absolute bottom-3 right-3 all-course-duration-badge z-10">
                    <i class="fas fa-clock"></i> {{ path.duration }}
                  </div>

                  <!-- Play Button -->
                  <div class="all-course-play-btn">
                    <i class="fas fa-play"></i>
                  </div>

                  <!-- Progress Bar for Enrolled Paths -->
                  <div v-if="path.isEnrolled" class="absolute bottom-0 left-0 right-0 h-1.5 bg-black/30 z-10">
                    <div class="h-full bg-gradient-to-r from-teal-400 to-teal-500 transition-all" :style="{ width: path.progress + '%' }"></div>
                  </div>
                </div>

                <!-- Card Body -->
                <div class="p-4">
                  <!-- Meta Info -->
                  <div class="flex items-center gap-3 text-[11px] text-gray-400 mb-2">
                    <span class="flex items-center gap-1">
                      <i class="fas fa-book text-[9px]"></i>
                      {{ path.totalCourses }} courses
                    </span>
                    <span class="flex items-center gap-1">
                      <i class="fas fa-signal text-[9px]"></i>
                      {{ path.level }}
                    </span>
                  </div>

                  <!-- Title -->
                  <h3 class="text-sm font-semibold text-gray-800 mb-2 line-clamp-2 group-hover:text-teal-600 transition-colors leading-snug">
                    {{ path.title }}
                  </h3>

                  <!-- Description -->
                  <p class="text-xs text-gray-500 mb-3 line-clamp-2">{{ path.description }}</p>

                  <!-- Skills Tags -->
                  <div class="flex flex-wrap gap-1.5 mb-3">
                    <span
                      v-for="skill in path.skills.slice(0, 3)"
                      :key="skill"
                      class="px-2 py-0.5 bg-gray-100 text-gray-600 text-[10px] font-medium rounded-full hover:bg-teal-50 hover:text-teal-600 transition-colors"
                    >
                      {{ skill }}
                    </span>
                    <span v-if="path.skills.length > 3" class="px-2 py-0.5 bg-gray-100 text-gray-500 text-[10px] rounded-full">
                      +{{ path.skills.length - 3 }}
                    </span>
                  </div>

                  <!-- Footer -->
                  <div class="flex items-center justify-between pt-3 border-t border-gray-100">
                    <!-- Path Icon -->
                    <div class="flex items-center gap-2">
                      <div class="w-7 h-7 rounded-full flex items-center justify-center text-white text-[10px] font-semibold shadow-sm" :style="{ background: `linear-gradient(135deg, ${path.color} 0%, ${path.color}dd 100%)` }">
                        <i :class="[path.icon, 'text-xs']"></i>
                      </div>
                      <span class="text-xs text-gray-600 font-medium">Learning Path</span>
                    </div>

                    <!-- Stats -->
                    <div class="flex items-center gap-3 text-[11px] text-gray-400">
                      <span class="flex items-center gap-1 hover:text-amber-500 transition-colors">
                        <i class="fas fa-star text-[9px] text-amber-400"></i>
                        {{ path.rating }}
                      </span>
                      <span v-if="path.isEnrolled" class="font-bold text-teal-600 text-sm">
                        {{ path.progress }}%
                      </span>
                      <span v-else class="flex items-center gap-1 hover:text-teal-500 transition-colors">
                        <i class="fas fa-users text-[9px]"></i>
                        {{ (path.enrolled / 1000).toFixed(1) }}k
                      </span>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- Empty State -->
            <div v-if="paginatedPaths.length === 0" class="all-courses-empty">
              <div class="all-courses-empty-icon">
                <i class="fas fa-route"></i>
              </div>
              <h3 class="all-courses-empty-title">No learning paths found</h3>
              <p class="all-courses-empty-text">Try adjusting your filter selection</p>
              <button @click="clearAllPathsFilters" class="all-courses-clear-btn">
                <i class="fas fa-redo"></i> Reset Filters
              </button>
            </div>

            <!-- Pagination Footer -->
            <div v-if="filteredPaths.length > 0" class="mt-4 px-4 py-3 bg-white rounded-2xl border border-gray-100 shadow-sm">
              <div class="flex items-center justify-between flex-wrap gap-3">
                <!-- Left: Stats & Items Per Page -->
                <div class="flex items-center gap-4 flex-wrap">
                  <span class="text-xs text-gray-500">
                    Showing <span class="font-semibold text-gray-700">{{ pathsPaginationStart }}</span>
                    to <span class="font-semibold text-gray-700">{{ pathsPaginationEnd }}</span>
                    of <span class="font-semibold text-gray-700">{{ filteredPaths.length }}</span> paths
                  </span>

                  <!-- Items Per Page Selector -->
                  <div class="flex items-center gap-2">
                    <span class="text-xs text-gray-500">Show:</span>
                    <select
                      v-model="pathsItemsPerPage"
                      @change="changePathsItemsPerPage(Number(($event.target as HTMLSelectElement).value))"
                      class="text-xs px-2 py-1.5 rounded-lg border border-gray-200 bg-white text-gray-700 focus:outline-none focus:ring-2 focus:ring-teal-500 focus:border-teal-500 cursor-pointer"
                    >
                      <option v-for="option in pathsItemsPerPageOptions" :key="option" :value="option">
                        {{ option }}
                      </option>
                    </select>
                    <span class="text-xs text-gray-500">per page</span>
                  </div>
                </div>

                <!-- Right: Pagination Controls -->
                <div class="flex items-center gap-2">
                  <button
                    @click="prevPathsPage"
                    :disabled="pathsCurrentPage === 1"
                    :class="[
                      'px-3 py-1.5 text-xs rounded-lg transition-colors flex items-center gap-1.5 border',
                      pathsCurrentPage === 1
                        ? 'text-gray-300 border-gray-100 cursor-not-allowed'
                        : 'text-gray-500 hover:text-gray-700 hover:bg-gray-100 border-gray-200'
                    ]"
                  >
                    <i class="fas fa-chevron-left text-[10px]"></i>
                    Previous
                  </button>

                  <!-- Page Numbers -->
                  <div class="flex items-center gap-1">
                    <template v-for="page in pathsTotalPages" :key="page">
                      <button
                        v-if="page === 1 || page === pathsTotalPages || (page >= pathsCurrentPage - 1 && page <= pathsCurrentPage + 1)"
                        @click="goToPathsPage(page)"
                        :class="[
                          'w-8 h-8 text-xs rounded-lg transition-colors flex items-center justify-center',
                          page === pathsCurrentPage
                            ? 'font-medium text-teal-600 bg-teal-50'
                            : 'text-gray-500 hover:text-gray-700 hover:bg-gray-100'
                        ]"
                      >
                        {{ page }}
                      </button>
                      <span
                        v-else-if="page === pathsCurrentPage - 2 || page === pathsCurrentPage + 2"
                        class="text-xs text-gray-400 px-1"
                      >...</span>
                    </template>
                  </div>

                  <button
                    @click="nextPathsPage"
                    :disabled="pathsCurrentPage === pathsTotalPages"
                    :class="[
                      'px-3 py-1.5 text-xs rounded-lg transition-colors flex items-center gap-1.5 border',
                      pathsCurrentPage === pathsTotalPages
                        ? 'text-gray-300 border-gray-100 cursor-not-allowed'
                        : 'text-gray-500 hover:text-gray-700 hover:bg-gray-100 border-gray-200'
                    ]"
                  >
                    Next
                    <i class="fas fa-chevron-right text-[10px]"></i>
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Certificates Section - Premium Enhanced -->
      <div v-if="currentView === 'certificates'" class="space-y-6">
        <!-- Premium Stats Cards -->
        <div class="cert-stats-section">
          <div class="cert-stats-grid">
            <div class="cert-stat-card cert-stat-total">
              <div class="cert-stat-glow"></div>
              <div class="cert-stat-icon">
                <i class="fas fa-award"></i>
              </div>
              <div class="cert-stat-content">
                <p class="cert-stat-value">{{ certificateStats.total }}</p>
                <p class="cert-stat-label">Total Certificates</p>
              </div>
              <div class="cert-stat-badge">
                <i class="fas fa-trophy"></i>
              </div>
            </div>
            <div class="cert-stat-card cert-stat-year">
              <div class="cert-stat-glow"></div>
              <div class="cert-stat-icon">
                <i class="fas fa-calendar-check"></i>
              </div>
              <div class="cert-stat-content">
                <p class="cert-stat-value">{{ certificateStats.thisYear }}</p>
                <p class="cert-stat-label">Earned This Year</p>
              </div>
              <div class="cert-stat-badge">
                <i class="fas fa-fire"></i>
              </div>
            </div>
            <div class="cert-stat-card cert-stat-hours">
              <div class="cert-stat-glow"></div>
              <div class="cert-stat-icon">
                <i class="fas fa-clock"></i>
              </div>
              <div class="cert-stat-content">
                <p class="cert-stat-value">{{ certificateStats.totalHours }}<span class="cert-stat-unit">h</span></p>
                <p class="cert-stat-label">Learning Hours</p>
              </div>
              <div class="cert-stat-badge">
                <i class="fas fa-hourglass-half"></i>
              </div>
            </div>
            <div class="cert-stat-card cert-stat-score">
              <div class="cert-stat-glow"></div>
              <div class="cert-stat-icon">
                <i class="fas fa-chart-line"></i>
              </div>
              <div class="cert-stat-content">
                <p class="cert-stat-value">{{ certificateStats.avgScore }}<span class="cert-stat-unit">%</span></p>
                <p class="cert-stat-label">Average Score</p>
              </div>
              <div class="cert-stat-badge">
                <i class="fas fa-star"></i>
              </div>
            </div>
          </div>
        </div>

        <!-- Certificates Main Section -->
        <div class="cert-main-section">
          <!-- Header Row -->
          <div class="cert-header">
            <div class="cert-header-left">
              <div class="cert-header-icon">
                <i class="fas fa-certificate"></i>
                <div class="cert-header-icon-ring"></div>
              </div>
              <div>
                <h2 class="cert-header-title">My Certificates</h2>
                <p class="cert-header-subtitle">{{ filteredCertificates.length }} achievements earned</p>
              </div>
            </div>
            <div class="cert-header-actions">
              <button class="cert-download-all-btn">
                <i class="fas fa-download"></i>
                Download All
              </button>
              <button class="cert-share-all-btn">
                <i class="fab fa-linkedin"></i>
                Share Profile
              </button>
            </div>
          </div>

          <!-- Toolbar Row -->
          <div class="cert-toolbar">
            <!-- Search -->
            <div class="cert-search-wrap">
              <i class="fas fa-search cert-search-icon"></i>
              <input
                v-model="certSearch"
                type="text"
                placeholder="Search certificates..."
                class="cert-search-input"
              />
              <button v-if="certSearch" @click="certSearch = ''" class="cert-search-clear">
                <i class="fas fa-times"></i>
              </button>
            </div>

            <!-- Score Filter Dropdown -->
            <div class="relative">
              <button
                @click="showCertScoreFilter = !showCertScoreFilter"
                :class="[
                  'cert-filter-btn',
                  certScoreFilter !== 'all' ? 'active' : ''
                ]"
              >
                <i class="fas fa-star"></i>
                <span>{{ certScoreOptions.find(o => o.value === certScoreFilter)?.label }}</span>
                <i :class="showCertScoreFilter ? 'fas fa-chevron-up' : 'fas fa-chevron-down'" class="chevron"></i>
              </button>
              <!-- Dropdown Panel -->
              <div v-if="showCertScoreFilter" class="cert-dropdown-panel">
                <div class="cert-dropdown-header">Filter by Score</div>
                <div class="cert-dropdown-options">
                  <button
                    v-for="option in certScoreOptions"
                    :key="option.value"
                    @click="certScoreFilter = option.value; showCertScoreFilter = false"
                    :class="['cert-dropdown-option', { active: certScoreFilter === option.value }]"
                  >
                    <div :class="['cert-radio', { checked: certScoreFilter === option.value }]">
                      <i v-if="certScoreFilter === option.value" class="fas fa-circle"></i>
                    </div>
                    <span>{{ option.label }}</span>
                  </button>
                </div>
              </div>
              <div v-if="showCertScoreFilter" @click="showCertScoreFilter = false" class="fixed inset-0 z-40"></div>
            </div>

            <!-- Sort Dropdown -->
            <div class="relative ml-auto flex items-center">
              <button
                @click="showCertSortDropdown = !showCertSortDropdown"
                class="cert-sort-btn"
              >
                <i :class="[certSortOptions.find(o => o.value === certSortBy)?.icon]"></i>
                <span>{{ certSortOptions.find(o => o.value === certSortBy)?.label }}</span>
                <i :class="showCertSortDropdown ? 'fas fa-chevron-up' : 'fas fa-chevron-down'" class="chevron"></i>
              </button>
              <button
                @click="certSortOrder = certSortOrder === 'asc' ? 'desc' : 'asc'"
                class="cert-sort-order-btn"
                :title="certSortOrder === 'asc' ? 'Ascending' : 'Descending'"
              >
                <i :class="certSortOrder === 'asc' ? 'fas fa-arrow-up' : 'fas fa-arrow-down'"></i>
              </button>

              <!-- Sort Dropdown Panel -->
              <div v-if="showCertSortDropdown" class="cert-dropdown-panel sort-panel">
                <div class="cert-dropdown-header">Sort By</div>
                <div class="cert-dropdown-options">
                  <button
                    v-for="option in certSortOptions"
                    :key="option.value"
                    @click="certSortBy = option.value; showCertSortDropdown = false"
                    :class="['cert-dropdown-option', { active: certSortBy === option.value }]"
                  >
                    <i :class="[option.icon, 'option-icon']"></i>
                    <span>{{ option.label }}</span>
                    <i v-if="certSortBy === option.value" class="fas fa-check check-icon"></i>
                  </button>
                </div>
              </div>
              <div v-if="showCertSortDropdown" @click="showCertSortDropdown = false" class="fixed inset-0 z-40"></div>
            </div>

            <!-- View Toggle -->
            <div class="cert-view-toggle">
              <button
                @click="certViewMode = 'grid'"
                :class="['cert-view-btn', { active: certViewMode === 'grid' }]"
                title="Grid view"
              >
                <i class="fas fa-th-large"></i>
              </button>
              <button
                @click="certViewMode = 'list'"
                :class="['cert-view-btn', { active: certViewMode === 'list' }]"
                title="List view"
              >
                <i class="fas fa-list"></i>
              </button>
            </div>
          </div>

          <!-- Active Filters Bar -->
          <div v-if="activeCertFiltersCount > 0" class="cert-active-filters">
            <div class="cert-filter-label">
              <i class="fas fa-filter"></i>
              <span>Active Filters</span>
            </div>
            <div class="cert-filter-tags">
              <span v-if="certSearch" class="cert-filter-tag search">
                <i class="fas fa-search"></i>
                "{{ certSearch }}"
                <button @click="certSearch = ''"><i class="fas fa-times"></i></button>
              </span>
              <span v-if="certScoreFilter !== 'all'" class="cert-filter-tag score">
                <i class="fas fa-star"></i>
                {{ certScoreOptions.find(o => o.value === certScoreFilter)?.label }}
                <button @click="certScoreFilter = 'all'"><i class="fas fa-times"></i></button>
              </span>
            </div>
            <button @click="clearAllCertFilters" class="cert-clear-all-btn">
              <i class="fas fa-times-circle"></i>
              Clear all
            </button>
          </div>

          <!-- Certificates Grid -->
          <div class="cert-grid-container">
            <div v-if="paginatedCertificates.length > 0" :class="['cert-grid', certViewMode]">
              <div v-for="cert in paginatedCertificates" :key="cert.id"
                   :class="['cert-card', certViewMode]">
                <!-- Certificate Visual Header -->
                <div class="cert-card-visual" :style="{ '--cert-color': cert.color }">
                  <!-- Decorative Elements -->
                  <div class="cert-visual-bg"></div>
                  <div class="cert-visual-pattern">
                    <div class="pattern-circle c1"></div>
                    <div class="pattern-circle c2"></div>
                    <div class="pattern-ring"></div>
                  </div>

                  <!-- Content -->
                  <div class="cert-visual-content">
                    <div class="cert-icon-wrap">
                      <i :class="cert.icon"></i>
                    </div>
                    <p class="cert-visual-label">Certificate of Completion</p>
                    <h3 class="cert-visual-title">{{ cert.title }}</h3>
                  </div>

                  <!-- Badges -->
                  <div class="cert-score-badge">
                    <i class="fas fa-star"></i>
                    <span>{{ cert.score }}%</span>
                  </div>
                  <div class="cert-verified-badge">
                    <i class="fas fa-check-circle"></i>
                    <span>Verified</span>
                  </div>
                </div>

                <!-- Certificate Body -->
                <div class="cert-card-body">
                  <!-- Course Info -->
                  <div class="cert-course-info">
                    <div>
                      <p class="cert-course-name">{{ cert.course }}</p>
                      <p class="cert-instructor">
                        <i class="fas fa-chalkboard-teacher"></i>
                        {{ cert.instructor }}
                      </p>
                    </div>
                    <span class="cert-hours-badge">
                      <i class="fas fa-clock"></i>
                      {{ cert.hours }}h
                    </span>
                  </div>

                  <!-- Skills -->
                  <div class="cert-skills">
                    <span
                      v-for="skill in cert.skills"
                      :key="skill"
                      class="cert-skill-tag"
                      :style="{ '--skill-color': cert.color }"
                    >
                      {{ skill }}
                    </span>
                  </div>

                  <!-- Metadata -->
                  <div class="cert-metadata">
                    <span>
                      <i class="fas fa-calendar-alt"></i>
                      {{ cert.date }}
                    </span>
                    <span>
                      <i class="fas fa-fingerprint"></i>
                      {{ cert.credentialId }}
                    </span>
                  </div>

                  <!-- Actions -->
                  <div class="cert-actions">
                    <button class="cert-action-btn primary">
                      <i class="fas fa-eye"></i>
                      <span>View</span>
                    </button>
                    <button class="cert-action-btn secondary">
                      <i class="fas fa-download"></i>
                      <span>PDF</span>
                    </button>
                    <button class="cert-action-btn linkedin">
                      <i class="fab fa-linkedin"></i>
                    </button>
                    <button class="cert-action-btn link">
                      <i class="fas fa-link"></i>
                    </button>
                  </div>
                </div>
              </div>
            </div>

            <!-- Empty State -->
            <div v-else class="cert-empty-state">
              <div class="cert-empty-icon">
                <i class="fas fa-certificate"></i>
              </div>
              <h3 class="cert-empty-title">No certificates found</h3>
              <p class="cert-empty-text">
                {{ activeCertFiltersCount > 0 ? 'Try adjusting your filter selection' : 'Complete courses to earn certificates and showcase your achievements' }}
              </p>
              <button v-if="activeCertFiltersCount > 0" @click="clearAllCertFilters" class="cert-empty-btn">
                <i class="fas fa-redo"></i>
                Reset Filters
              </button>
              <button v-else @click="currentView = 'all'" class="cert-empty-btn">
                <i class="fas fa-book-reader"></i>
                Browse Courses
              </button>
            </div>

            <!-- Pagination Footer -->
            <div v-if="filteredCertificates.length > 0" class="cert-pagination">
              <div class="cert-pagination-info">
                <span>
                  Showing <strong>{{ certPaginationStart }}</strong>
                  to <strong>{{ certPaginationEnd }}</strong>
                  of <strong>{{ filteredCertificates.length }}</strong> certificates
                </span>
                <div class="cert-per-page">
                  <span>Show:</span>
                  <select
                    v-model="certItemsPerPage"
                    @change="changeCertItemsPerPage(Number(($event.target as HTMLSelectElement).value))"
                  >
                    <option v-for="option in certItemsPerPageOptions" :key="option" :value="option">
                      {{ option }}
                    </option>
                  </select>
                  <span>per page</span>
                </div>
              </div>

              <div class="cert-pagination-controls">
                <button
                  @click="prevCertPage"
                  :disabled="certCurrentPage === 1"
                  class="cert-page-btn"
                >
                  <i class="fas fa-chevron-left"></i>
                  Previous
                </button>

                <div class="cert-page-numbers">
                  <template v-for="page in certTotalPages" :key="page">
                    <button
                      v-if="page === 1 || page === certTotalPages || (page >= certCurrentPage - 1 && page <= certCurrentPage + 1)"
                      @click="goToCertPage(page)"
                      :class="['cert-page-num', { active: page === certCurrentPage }]"
                    >
                      {{ page }}
                    </button>
                    <span
                      v-else-if="page === certCurrentPage - 2 || page === certCurrentPage + 2"
                      class="cert-page-dots"
                    >...</span>
                  </template>
                </div>

                <button
                  @click="nextCertPage"
                  :disabled="certCurrentPage === certTotalPages"
                  class="cert-page-btn"
                >
                  Next
                  <i class="fas fa-chevron-right"></i>
                </button>
              </div>
            </div>
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

/* Continue Learning Section */
.continue-learning-section {
  background: linear-gradient(135deg, #f0fdfa 0%, #f8fafc 100%);
  border-radius: 24px;
  padding: 24px;
  border: 1px solid #e0f2f1;
}

.continue-learning-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 24px;
}

.continue-learning-icon {
  position: relative;
  width: 48px;
  height: 48px;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  border-radius: 14px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 18px;
  box-shadow: 0 8px 24px rgba(20, 184, 166, 0.3);
}

.icon-pulse {
  position: absolute;
  inset: -4px;
  border-radius: 18px;
  border: 2px solid #14b8a6;
  animation: pulse-ring 2s ease-out infinite;
}

@keyframes pulse-ring {
  0% { transform: scale(1); opacity: 0.5; }
  100% { transform: scale(1.2); opacity: 0; }
}

.continue-learning-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
  gap: 20px;
}

.continue-card {
  background: white;
  border-radius: 20px;
  overflow: hidden;
  border: 1px solid #e5e7eb;
  cursor: pointer;
  transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
  animation: card-enter 0.5s ease-out backwards;
}

@keyframes card-enter {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.continue-card:hover {
  transform: translateY(-8px);
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.12);
  border-color: #14b8a6;
}

.continue-card-image {
  position: relative;
  height: 160px;
  overflow: hidden;
}

.continue-card-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.6s ease;
}

.continue-card:hover .continue-card-image img {
  transform: scale(1.1);
}

.image-overlay {
  position: absolute;
  inset: 0;
  background: linear-gradient(180deg, rgba(0,0,0,0) 0%, rgba(0,0,0,0.4) 100%);
}

.play-button-wrapper {
  position: absolute;
  inset: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  opacity: 0;
  transition: opacity 0.3s ease;
}

.continue-card:hover .play-button-wrapper {
  opacity: 1;
}

.play-button {
  width: 56px;
  height: 56px;
  background: rgba(255, 255, 255, 0.95);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #14b8a6;
  font-size: 20px;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.2);
  transform: scale(0.8);
  transition: all 0.3s ease;
}

.continue-card:hover .play-button {
  transform: scale(1);
}

.play-button:hover {
  background: #14b8a6;
  color: white;
}

.card-top-actions {
  position: absolute;
  top: 12px;
  right: 12px;
  display: flex;
  gap: 8px;
}

.save-btn {
  width: 36px;
  height: 36px;
  background: rgba(255, 255, 255, 0.9);
  backdrop-filter: blur(8px);
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #64748b;
  border: none;
  cursor: pointer;
  transition: all 0.2s ease;
}

.save-btn:hover, .save-btn.saved {
  background: #14b8a6;
  color: white;
}

.progress-ring-wrapper {
  position: absolute;
  bottom: 12px;
  left: 12px;
  width: 52px;
  height: 52px;
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(8px);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.progress-ring {
  width: 44px;
  height: 44px;
  transform: rotate(-90deg);
}

.progress-ring-bg {
  fill: none;
  stroke: #e5e7eb;
  stroke-width: 3;
}

.progress-ring-fill {
  fill: none;
  stroke: #14b8a6;
  stroke-width: 3;
  stroke-linecap: round;
  transition: stroke-dasharray 0.6s ease;
}

.progress-text {
  position: absolute;
  font-size: 10px;
  font-weight: 700;
  color: #0f766e;
}

.continue-card-content {
  padding: 20px;
}

.card-meta {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 12px;
}

.level-badge {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  padding: 4px 10px;
  border-radius: 20px;
  font-size: 11px;
  font-weight: 600;
}

.level-badge.beginner {
  background: #dcfce7;
  color: #15803d;
}

.level-badge.intermediate {
  background: #fef3c7;
  color: #b45309;
}

.level-badge.advanced {
  background: #fee2e2;
  color: #dc2626;
}

.duration-badge {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  padding: 4px 10px;
  background: #f1f5f9;
  border-radius: 20px;
  font-size: 11px;
  font-weight: 500;
  color: #64748b;
}

.course-title {
  font-size: 16px;
  font-weight: 700;
  color: #1e293b;
  margin-bottom: 12px;
  line-height: 1.4;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
  transition: color 0.2s ease;
}

.continue-card:hover .course-title {
  color: #0f766e;
}

.instructor-row {
  display: flex;
  align-items: center;
  gap: 10px;
  margin-bottom: 16px;
}

.instructor-avatar {
  width: 28px;
  height: 28px;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 10px;
  font-weight: 600;
}

.instructor-name {
  font-size: 13px;
  color: #64748b;
  font-weight: 500;
}

.current-lesson {
  background: linear-gradient(135deg, #f0fdfa 0%, #ecfdf5 100%);
  border-radius: 12px;
  padding: 12px;
  margin-bottom: 16px;
  border: 1px solid #d1fae5;
}

.lesson-indicator {
  display: flex;
  align-items: center;
  gap: 6px;
  margin-bottom: 6px;
}

.lesson-indicator i {
  color: #14b8a6;
  font-size: 12px;
}

.lesson-indicator span {
  font-size: 10px;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  color: #0f766e;
}

.lesson-title {
  font-size: 13px;
  font-weight: 600;
  color: #1e293b;
  margin: 0;
  display: -webkit-box;
  -webkit-line-clamp: 1;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.progress-section {
  margin-bottom: 16px;
}

.progress-info {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 8px;
}

.lessons-count {
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: 12px;
  color: #64748b;
  font-weight: 500;
}

.time-left {
  font-size: 11px;
  color: #94a3b8;
  font-weight: 500;
}

.progress-bar {
  height: 6px;
  background: #e5e7eb;
  border-radius: 10px;
  overflow: hidden;
}

.progress-fill {
  height: 100%;
  background: linear-gradient(90deg, #14b8a6 0%, #0d9488 100%);
  border-radius: 10px;
  position: relative;
  transition: width 0.6s ease;
}

.progress-glow {
  position: absolute;
  right: 0;
  top: 50%;
  transform: translateY(-50%);
  width: 20px;
  height: 20px;
  background: #14b8a6;
  border-radius: 50%;
  filter: blur(8px);
  opacity: 0.6;
}

.continue-btn {
  width: 100%;
  padding: 12px 20px;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
  border: none;
  border-radius: 12px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  transition: all 0.3s ease;
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.3);
}

.continue-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(20, 184, 166, 0.4);
}

.continue-btn i {
  transition: transform 0.3s ease;
}

.continue-btn:hover i {
  transform: translateX(4px);
}

/* View Navigation */
.view-nav {
  display: flex;
  gap: 0.5rem;
  padding: 1rem 2rem;
  background: white;
  border-bottom: 1px solid #e5e7eb;
  margin: 0;
}

.view-nav-btn {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.625rem 1rem;
  font-size: 0.875rem;
  font-weight: 500;
  color: #64748b;
  background: transparent;
  border: 1px solid transparent;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.2s;
}

.view-nav-btn:hover {
  background: #f1f5f9;
  color: #334155;
}

.view-nav-btn.active {
  background: #f0fdfa;
  color: #0f766e;
  border-color: #99f6e4;
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

/* Trending Section */
.trending-section {
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 1rem;
  padding: 1.25rem;
  box-shadow: 0 1px 2px 0 rgb(0 0 0 / 0.05);
}

.trending-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 1rem;
}

.trending-title-wrap {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.trending-icon {
  width: 2.5rem;
  height: 2.5rem;
  border-radius: 0.75rem;
  background: linear-gradient(135deg, #f97316 0%, #ea580c 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 4px 12px rgba(249, 115, 22, 0.3);
  animation: pulse-fire 2s ease-in-out infinite;
}

@keyframes pulse-fire {
  0%, 100% { box-shadow: 0 4px 12px rgba(249, 115, 22, 0.3); }
  50% { box-shadow: 0 4px 20px rgba(249, 115, 22, 0.5); }
}

.trending-icon i {
  color: white;
  font-size: 0.875rem;
}

.trending-title {
  font-size: 1.125rem;
  font-weight: 700;
  color: #1e293b;
  margin: 0;
}

.trending-subtitle {
  font-size: 0.75rem;
  color: #64748b;
  margin: 0;
}

.trending-see-all {
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

.trending-see-all:hover {
  background: #fff7ed;
  color: #ea580c;
  border-color: #ea580c;
}

.trending-see-all i {
  font-size: 0.625rem;
  transition: transform 0.2s ease;
}

.trending-see-all:hover i {
  transform: translateX(3px);
}

.trending-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 1rem;
}

.trending-card {
  position: relative;
  background: white;
  border: 1px solid #e2e8f0;
  border-radius: 0.75rem;
  overflow: hidden;
  transition: all 0.3s ease;
  cursor: pointer;
}

.trending-card:hover {
  border-color: #f97316;
  box-shadow: 0 8px 24px rgba(249, 115, 22, 0.15);
  transform: translateY(-4px);
}

.trending-save-btn {
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

.trending-card:hover .trending-save-btn {
  opacity: 1;
  transform: scale(1);
}

.trending-save-btn:hover {
  transform: scale(1.1);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

.trending-save-btn i {
  font-size: 0.75rem;
  color: #64748b;
  transition: all 0.2s ease;
}

.trending-save-btn:hover i {
  color: #f97316;
}

.trending-save-btn.saved {
  background: linear-gradient(135deg, #f97316 0%, #ea580c 100%);
  border-color: transparent;
  opacity: 1;
  transform: scale(1);
}

.trending-save-btn.saved i {
  color: white;
}

.trending-rank {
  position: absolute;
  top: 0.5rem;
  left: 0.5rem;
  padding: 0.25rem 0.5rem;
  background: linear-gradient(135deg, #f97316 0%, #ea580c 100%);
  color: white;
  font-size: 0.625rem;
  font-weight: 600;
  border-radius: 0.375rem;
  z-index: 5;
  display: flex;
  align-items: center;
  gap: 0.25rem;
}

.trending-rank i {
  font-size: 0.5rem;
}

.trending-image {
  position: relative;
  height: 120px;
  overflow: hidden;
}

.trending-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.4s ease;
}

.trending-card:hover .trending-image img {
  transform: scale(1.08);
}

.trending-overlay {
  position: absolute;
  inset: 0;
  background: linear-gradient(to top, rgba(0,0,0,0.5) 0%, transparent 60%);
}

.trending-duration {
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

.trending-content {
  padding: 0.875rem;
}

.trending-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 0.375rem;
  margin-bottom: 0.5rem;
}

.trending-tag {
  padding: 0.125rem 0.375rem;
  background: #f1f5f9;
  color: #64748b;
  font-size: 0.5625rem;
  font-weight: 500;
  border-radius: 0.25rem;
}

.trending-level {
  padding: 0.125rem 0.375rem;
  font-size: 0.5625rem;
  font-weight: 600;
  border-radius: 0.25rem;
  text-transform: uppercase;
}

.trending-level.beginner {
  background: #dcfce7;
  color: #16a34a;
}

.trending-level.intermediate {
  background: #dbeafe;
  color: #2563eb;
}

.trending-level.advanced {
  background: #ede9fe;
  color: #7c3aed;
}

.trending-course-title {
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

.trending-instructor {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-bottom: 0.75rem;
}

.trending-avatar {
  width: 1.5rem;
  height: 1.5rem;
  border-radius: 50%;
  background: linear-gradient(135deg, #f97316 0%, #ea580c 100%);
  color: white;
  font-size: 0.5rem;
  font-weight: 600;
  display: flex;
  align-items: center;
  justify-content: center;
}

.trending-instructor span {
  font-size: 0.6875rem;
  color: #64748b;
}

.trending-footer {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding-top: 0.625rem;
  border-top: 1px solid #f1f5f9;
}

.trending-stats {
  display: flex;
  align-items: center;
  gap: 0.625rem;
}

.trending-stats span {
  display: flex;
  align-items: center;
  gap: 0.25rem;
  font-size: 0.625rem;
  color: #64748b;
}

.trending-stats i {
  font-size: 0.5rem;
}

.trending-enroll-btn {
  padding: 0.375rem 0.75rem;
  background: linear-gradient(135deg, #f97316 0%, #ea580c 100%);
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

.trending-enroll-btn:hover {
  box-shadow: 0 4px 12px rgba(249, 115, 22, 0.35);
  transform: scale(1.05);
}

.trending-enroll-btn i {
  font-size: 0.5rem;
}

/* Recommended Section */
.recommended-section {
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 1rem;
  padding: 1.25rem;
  box-shadow: 0 1px 2px 0 rgb(0 0 0 / 0.05);
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
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.3);
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
  background: #f0fdfa;
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

/* ========================================
   MY LEARNING PATHS SECTION
   ======================================== */
.my-paths-section {
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 1rem;
  padding: 1.25rem;
  box-shadow: 0 1px 2px 0 rgb(0 0 0 / 0.05);
  border-left: 4px solid #14b8a6;
}

.my-paths-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 1.25rem;
}

.my-paths-title-wrap {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.my-paths-icon {
  width: 2.5rem;
  height: 2.5rem;
  border-radius: 0.75rem;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.3);
  animation: pulse-path 2s ease-in-out infinite;
}

@keyframes pulse-path {
  0%, 100% { box-shadow: 0 4px 12px rgba(20, 184, 166, 0.3); }
  50% { box-shadow: 0 4px 20px rgba(20, 184, 166, 0.5); }
}

.my-paths-icon i {
  color: white;
  font-size: 0.875rem;
}

.my-paths-title {
  font-size: 1.125rem;
  font-weight: 700;
  color: #1e293b;
  margin: 0;
}

.my-paths-subtitle {
  font-size: 0.75rem;
  color: #64748b;
  margin: 0;
}

.my-paths-see-all {
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

.my-paths-see-all:hover {
  background: #f0fdfa;
  color: #0d9488;
  border-color: #14b8a6;
}

.my-paths-see-all i {
  font-size: 0.625rem;
  transition: transform 0.2s ease;
}

.my-paths-see-all:hover i {
  transform: translateX(3px);
}

.my-paths-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 1.25rem;
}

.my-path-card {
  position: relative;
  background: white;
  border: 1px solid #e2e8f0;
  border-radius: 1rem;
  overflow: hidden;
  transition: all 0.3s ease;
  cursor: pointer;
}

.my-path-card:hover {
  border-color: #14b8a6;
  box-shadow: 0 12px 32px rgba(20, 184, 166, 0.15);
  transform: translateY(-4px);
}

/* Progress Ring */
.my-path-progress-ring {
  position: absolute;
  top: 0.75rem;
  right: 0.75rem;
  width: 3rem;
  height: 3rem;
  z-index: 10;
  background: rgba(255, 255, 255, 0.95);
  border-radius: 50%;
  padding: 0.25rem;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

.my-path-progress-ring svg {
  width: 100%;
  height: 100%;
  transform: rotate(-90deg);
}

.progress-ring-circle {
  transition: stroke-dashoffset 0.5s ease;
}

.progress-ring-text {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  font-size: 0.625rem;
  font-weight: 700;
  color: #0d9488;
}

/* Path Image */
.my-path-image {
  position: relative;
  height: 120px;
  overflow: hidden;
}

.my-path-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.5s ease;
}

.my-path-card:hover .my-path-image img {
  transform: scale(1.08);
}

.my-path-overlay {
  position: absolute;
  inset: 0;
  background: linear-gradient(135deg, rgba(20, 184, 166, 0.1) 0%, rgba(13, 148, 136, 0.2) 100%);
  pointer-events: none;
}

.my-path-badges {
  position: absolute;
  top: 0.75rem;
  left: 0.75rem;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.my-path-level {
  padding: 0.25rem 0.5rem;
  font-size: 0.625rem;
  font-weight: 600;
  border-radius: 0.375rem;
  text-transform: uppercase;
}

.my-path-level.beginner {
  background: #dcfce7;
  color: #166534;
}

.my-path-level.intermediate {
  background: #fef3c7;
  color: #92400e;
}

.my-path-level.advanced {
  background: #fee2e2;
  color: #991b1b;
}

.my-path-enrolled-badge {
  padding: 0.25rem 0.5rem;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
  font-size: 0.625rem;
  font-weight: 600;
  border-radius: 0.375rem;
  display: flex;
  align-items: center;
  gap: 0.25rem;
}

.my-path-enrolled-badge i {
  font-size: 0.5rem;
}

/* Path Content */
.my-path-content {
  padding: 1rem;
}

.my-path-header-info {
  display: flex;
  align-items: flex-start;
  gap: 0.75rem;
  margin-bottom: 1rem;
}

.my-path-icon-wrap {
  width: 2.5rem;
  height: 2.5rem;
  min-width: 2.5rem;
  border-radius: 0.625rem;
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.15);
}

.my-path-icon-wrap i {
  font-size: 1rem;
}

.my-path-title-section {
  flex: 1;
  min-width: 0;
}

.my-path-title {
  font-size: 0.9375rem;
  font-weight: 700;
  color: #1e293b;
  margin: 0 0 0.25rem 0;
  line-height: 1.3;
}

.my-path-activity {
  font-size: 0.6875rem;
  color: #94a3b8;
  margin: 0;
  display: flex;
  align-items: center;
  gap: 0.25rem;
}

.my-path-activity i {
  font-size: 0.5rem;
}

/* Course Progress Breakdown */
.my-path-courses {
  margin-bottom: 1rem;
  padding: 0.75rem;
  background: #f8fafc;
  border-radius: 0.5rem;
}

.my-path-courses-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 0.75rem;
}

.courses-count {
  font-size: 0.6875rem;
  font-weight: 600;
  color: #64748b;
}

.courses-progress-bar {
  width: 100px;
  height: 4px;
  background: #e2e8f0;
  border-radius: 2px;
  overflow: hidden;
}

.courses-progress-fill {
  height: 100%;
  background: linear-gradient(90deg, #14b8a6 0%, #0d9488 100%);
  border-radius: 2px;
  transition: width 0.5s ease;
}

.my-path-courses-list {
  display: flex;
  flex-direction: column;
  gap: 0.375rem;
}

.course-item {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.6875rem;
  color: #475569;
}

.course-item i {
  font-size: 0.625rem;
  color: #cbd5e1;
}

.course-item.completed {
  color: #94a3b8;
}

.course-item.completed span {
  text-decoration: line-through;
}

.course-item.completed i {
  color: #14b8a6;
}

.courses-more {
  font-size: 0.625rem;
  color: #94a3b8;
  padding-left: 1.125rem;
}

/* Skills */
.my-path-skills {
  display: flex;
  flex-wrap: wrap;
  gap: 0.375rem;
  margin-bottom: 1rem;
}

.skill-tag {
  padding: 0.25rem 0.5rem;
  font-size: 0.625rem;
  font-weight: 500;
  border-radius: 9999px;
}

.skill-more {
  padding: 0.25rem 0.5rem;
  font-size: 0.625rem;
  font-weight: 500;
  border-radius: 9999px;
  background: #f1f5f9;
  color: #64748b;
}

/* Footer */
.my-path-footer {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding-top: 0.75rem;
  border-top: 1px solid #f1f5f9;
}

.my-path-stats {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.my-path-stats span {
  display: flex;
  align-items: center;
  gap: 0.25rem;
  font-size: 0.6875rem;
  color: #64748b;
}

.my-path-stats i {
  font-size: 0.5625rem;
  color: #94a3b8;
}

.my-path-stats i.fa-star {
  color: #f59e0b;
}

.my-path-continue-btn {
  padding: 0.5rem 1rem;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
  border: none;
  border-radius: 0.5rem;
  font-size: 0.6875rem;
  font-weight: 600;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 0.375rem;
  transition: all 0.2s ease;
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.25);
}

.my-path-continue-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 16px rgba(20, 184, 166, 0.35);
}

.my-path-continue-btn i {
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

  .trending-section {
    padding: 1rem;
  }

  .trending-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 0.75rem;
  }

  .trending-see-all {
    width: 100%;
    justify-content: center;
  }

  .trending-grid {
    grid-template-columns: repeat(2, 1fr);
    gap: 0.75rem;
  }

  .trending-image {
    height: 100px;
  }

  .trending-content {
    padding: 0.75rem;
  }

  .trending-course-title {
    font-size: 0.75rem;
  }

  .trending-instructor {
    display: none;
  }

  .trending-stats {
    gap: 0.5rem;
  }

  .trending-enroll-btn {
    padding: 0.25rem 0.5rem;
    font-size: 0.5625rem;
  }

  .trending-save-btn {
    opacity: 1;
    transform: scale(1);
  }

  /* Recommended Section Responsive */
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

  /* My Learning Paths Responsive */
  .my-paths-section {
    padding: 1rem;
  }

  .my-paths-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 0.75rem;
  }

  .my-paths-see-all {
    width: 100%;
    justify-content: center;
  }

  .my-paths-grid {
    grid-template-columns: 1fr;
    gap: 1rem;
  }

  .my-path-card {
    border-radius: 0.75rem;
  }

  .my-path-image {
    height: 100px;
  }

  .my-path-progress-ring {
    width: 2.5rem;
    height: 2.5rem;
  }

  .progress-ring-text {
    font-size: 0.5rem;
  }

  .my-path-content {
    padding: 0.75rem;
  }

  .my-path-header-info {
    flex-direction: column;
    gap: 0.5rem;
  }

  .my-path-icon-wrap {
    width: 2rem;
    height: 2rem;
    min-width: 2rem;
  }

  .my-path-icon-wrap i {
    font-size: 0.75rem;
  }

  .my-path-title {
    font-size: 0.875rem;
  }

  .my-path-courses {
    padding: 0.5rem;
  }

  .my-path-courses-header {
    flex-direction: column;
    gap: 0.5rem;
    align-items: flex-start;
  }

  .courses-progress-bar {
    width: 100%;
  }

  .my-path-footer {
    flex-direction: column;
    gap: 0.75rem;
    align-items: stretch;
  }

  .my-path-stats {
    justify-content: center;
  }

  .my-path-continue-btn {
    width: 100%;
    justify-content: center;
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

/* Mobile breakpoint */
@media (max-width: 640px) {
  /* My Learning Paths Mobile */
  .my-paths-grid {
    grid-template-columns: 1fr;
  }

  .my-path-image {
    height: 90px;
  }

  .my-path-progress-ring {
    width: 2.25rem;
    height: 2.25rem;
  }

  .my-path-courses-list {
    display: none;
  }

  .courses-more {
    display: block;
    padding-left: 0;
  }

  .my-path-skills {
    display: none;
  }
}

/* ========================================
   CERTIFICATES SECTION - PREMIUM STYLES
   ======================================== */

/* Stats Section */
.cert-stats-section {
  padding: 0;
}

.cert-stats-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 1rem;
}

.cert-stat-card {
  position: relative;
  background: white;
  border-radius: 1.25rem;
  padding: 1.5rem;
  display: flex;
  align-items: center;
  gap: 1rem;
  border: 1px solid rgba(0, 0, 0, 0.05);
  overflow: hidden;
  transition: all 0.3s ease;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.04);
}

.cert-stat-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 12px 24px rgba(0, 0, 0, 0.1);
}

.cert-stat-card:hover .cert-stat-glow {
  opacity: 0.15;
}

.cert-stat-card:hover .cert-stat-icon {
  transform: scale(1.1);
}

.cert-stat-glow {
  position: absolute;
  top: -50%;
  right: -50%;
  width: 150%;
  height: 150%;
  border-radius: 50%;
  opacity: 0.08;
  transition: opacity 0.3s ease;
  pointer-events: none;
}

.cert-stat-total .cert-stat-glow { background: radial-gradient(circle, #f59e0b 0%, transparent 70%); }
.cert-stat-year .cert-stat-glow { background: radial-gradient(circle, #14b8a6 0%, transparent 70%); }
.cert-stat-hours .cert-stat-glow { background: radial-gradient(circle, #3b82f6 0%, transparent 70%); }
.cert-stat-score .cert-stat-glow { background: radial-gradient(circle, #8b5cf6 0%, transparent 70%); }

.cert-stat-icon {
  width: 3.5rem;
  height: 3.5rem;
  border-radius: 1rem;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.25rem;
  color: white;
  transition: transform 0.3s ease;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

.cert-stat-total .cert-stat-icon { background: linear-gradient(135deg, #f59e0b 0%, #d97706 100%); }
.cert-stat-year .cert-stat-icon { background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%); }
.cert-stat-hours .cert-stat-icon { background: linear-gradient(135deg, #3b82f6 0%, #2563eb 100%); }
.cert-stat-score .cert-stat-icon { background: linear-gradient(135deg, #8b5cf6 0%, #7c3aed 100%); }

.cert-stat-content {
  flex: 1;
}

.cert-stat-value {
  font-size: 1.75rem;
  font-weight: 700;
  color: #1f2937;
  line-height: 1;
  margin-bottom: 0.25rem;
}

.cert-stat-unit {
  font-size: 1rem;
  font-weight: 600;
  color: #6b7280;
}

.cert-stat-label {
  font-size: 0.75rem;
  color: #6b7280;
}

.cert-stat-badge {
  position: absolute;
  top: 0.75rem;
  right: 0.75rem;
  width: 1.5rem;
  height: 1.5rem;
  border-radius: 0.5rem;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.625rem;
}

.cert-stat-total .cert-stat-badge { background: #fef3c7; color: #d97706; }
.cert-stat-year .cert-stat-badge { background: #ccfbf1; color: #0d9488; }
.cert-stat-hours .cert-stat-badge { background: #dbeafe; color: #2563eb; }
.cert-stat-score .cert-stat-badge { background: #ede9fe; color: #7c3aed; }

/* Main Section */
.cert-main-section {
  background: white;
  border-radius: 1.5rem;
  border: 1px solid rgba(0, 0, 0, 0.05);
  overflow: hidden;
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.04);
}

/* Header */
.cert-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 1.5rem;
  border-bottom: 1px solid #f3f4f6;
  background: linear-gradient(180deg, #fffbeb 0%, white 100%);
}

.cert-header-left {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.cert-header-icon {
  position: relative;
  width: 3rem;
  height: 3rem;
  border-radius: 1rem;
  background: linear-gradient(135deg, #f59e0b 0%, #d97706 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 1.125rem;
  box-shadow: 0 4px 12px rgba(245, 158, 11, 0.3);
}

.cert-header-icon-ring {
  position: absolute;
  inset: -4px;
  border-radius: 1.25rem;
  border: 2px solid rgba(245, 158, 11, 0.2);
  animation: pulse-ring 2s ease-in-out infinite;
}

@keyframes pulse-ring {
  0%, 100% { transform: scale(1); opacity: 1; }
  50% { transform: scale(1.1); opacity: 0.5; }
}

.cert-header-title {
  font-size: 1.25rem;
  font-weight: 700;
  color: #1f2937;
}

.cert-header-subtitle {
  font-size: 0.75rem;
  color: #6b7280;
}

.cert-header-actions {
  display: flex;
  gap: 0.75rem;
}

.cert-download-all-btn,
.cert-share-all-btn {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem 1rem;
  border-radius: 0.75rem;
  font-size: 0.75rem;
  font-weight: 600;
  transition: all 0.2s ease;
  cursor: pointer;
}

.cert-download-all-btn {
  background: white;
  border: 1px solid #e5e7eb;
  color: #374151;
}

.cert-download-all-btn:hover {
  background: #f9fafb;
  border-color: #d1d5db;
}

.cert-share-all-btn {
  background: #0077b5;
  border: none;
  color: white;
}

.cert-share-all-btn:hover {
  background: #006699;
}

/* Toolbar */
.cert-toolbar {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 1rem 1.5rem;
  border-bottom: 1px solid #f3f4f6;
  background: #fafafa;
}

.cert-search-wrap {
  position: relative;
  flex: 0 0 280px;
}

.cert-search-icon {
  position: absolute;
  left: 0.875rem;
  top: 50%;
  transform: translateY(-50%);
  color: #9ca3af;
  font-size: 0.75rem;
}

.cert-search-input {
  width: 100%;
  padding: 0.5rem 2rem 0.5rem 2.25rem;
  border: 1px solid #e5e7eb;
  border-radius: 0.75rem;
  font-size: 0.75rem;
  background: white;
  transition: all 0.2s ease;
}

.cert-search-input:focus {
  outline: none;
  border-color: #f59e0b;
  box-shadow: 0 0 0 3px rgba(245, 158, 11, 0.1);
}

.cert-search-clear {
  position: absolute;
  right: 0.5rem;
  top: 50%;
  transform: translateY(-50%);
  width: 1.25rem;
  height: 1.25rem;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  background: #e5e7eb;
  color: #6b7280;
  font-size: 0.625rem;
  border: none;
  cursor: pointer;
  transition: all 0.2s ease;
}

.cert-search-clear:hover {
  background: #d1d5db;
  color: #374151;
}

.cert-filter-btn {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem 0.875rem;
  border: 1px solid #e5e7eb;
  border-radius: 0.75rem;
  background: white;
  font-size: 0.75rem;
  font-weight: 500;
  color: #374151;
  cursor: pointer;
  transition: all 0.2s ease;
}

.cert-filter-btn:hover {
  background: #f9fafb;
}

.cert-filter-btn.active {
  background: #fef3c7;
  border-color: #fcd34d;
  color: #92400e;
}

.cert-filter-btn .chevron {
  font-size: 0.625rem;
  margin-left: 0.25rem;
}

.cert-dropdown-panel {
  position: absolute;
  left: 0;
  top: 100%;
  margin-top: 0.5rem;
  width: 200px;
  background: white;
  border-radius: 0.875rem;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.15);
  border: 1px solid #e5e7eb;
  z-index: 50;
  overflow: hidden;
}

.cert-dropdown-panel.sort-panel {
  left: auto;
  right: 0;
}

.cert-dropdown-header {
  padding: 0.75rem 1rem;
  font-size: 0.625rem;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  color: #9ca3af;
}

.cert-dropdown-options {
  max-height: 240px;
  overflow-y: auto;
}

.cert-dropdown-option {
  width: 100%;
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.625rem 1rem;
  font-size: 0.8125rem;
  color: #374151;
  background: none;
  border: none;
  cursor: pointer;
  transition: all 0.15s ease;
  text-align: left;
}

.cert-dropdown-option:hover {
  background: #f9fafb;
}

.cert-dropdown-option.active {
  background: #fef3c7;
  color: #92400e;
}

.cert-radio {
  width: 1rem;
  height: 1rem;
  border-radius: 50%;
  border: 2px solid #d1d5db;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s ease;
}

.cert-radio.checked {
  background: #f59e0b;
  border-color: #f59e0b;
}

.cert-radio i {
  font-size: 0.375rem;
  color: white;
}

.cert-dropdown-option .option-icon {
  width: 1rem;
  color: #9ca3af;
}

.cert-dropdown-option.active .option-icon {
  color: #f59e0b;
}

.cert-dropdown-option .check-icon {
  margin-left: auto;
  font-size: 0.75rem;
  color: #f59e0b;
}

.cert-sort-btn {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem 0.875rem;
  border: 1px solid #e5e7eb;
  border-right: none;
  border-radius: 0.75rem 0 0 0.75rem;
  background: white;
  font-size: 0.75rem;
  font-weight: 500;
  color: #374151;
  cursor: pointer;
  transition: all 0.2s ease;
}

.cert-sort-btn:hover {
  background: #f9fafb;
}

.cert-sort-btn i:first-child {
  color: #f59e0b;
}

.cert-sort-btn .chevron {
  font-size: 0.625rem;
  margin-left: 0.25rem;
}

.cert-sort-order-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 2rem;
  height: 2rem;
  border: 1px solid #e5e7eb;
  border-radius: 0 0.75rem 0.75rem 0;
  background: white;
  color: #f59e0b;
  font-size: 0.75rem;
  cursor: pointer;
  transition: all 0.2s ease;
}

.cert-sort-order-btn:hover {
  background: #fef3c7;
}

.cert-view-toggle {
  display: flex;
  align-items: center;
  gap: 0.125rem;
  padding: 0.25rem;
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 0.75rem;
}

.cert-view-btn {
  padding: 0.375rem 0.625rem;
  border-radius: 0.5rem;
  background: none;
  border: none;
  color: #9ca3af;
  font-size: 0.75rem;
  cursor: pointer;
  transition: all 0.2s ease;
}

.cert-view-btn:hover {
  color: #6b7280;
  background: #f3f4f6;
}

.cert-view-btn.active {
  background: #f59e0b;
  color: white;
}

/* Active Filters */
.cert-active-filters {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.875rem 1.5rem;
  background: #fffbeb;
  border-bottom: 1px solid #fef3c7;
}

.cert-filter-label {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.375rem 0.75rem;
  background: white;
  border-radius: 0.5rem;
  font-size: 0.6875rem;
  font-weight: 600;
  color: #6b7280;
  border: 1px solid #e5e7eb;
}

.cert-filter-label i {
  color: #9ca3af;
}

.cert-filter-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
  flex: 1;
}

.cert-filter-tag {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  padding: 0.375rem 0.625rem;
  border-radius: 0.5rem;
  font-size: 0.6875rem;
  font-weight: 500;
}

.cert-filter-tag.search {
  background: #f3f4f6;
  color: #374151;
  border: 1px solid #e5e7eb;
}

.cert-filter-tag.score {
  background: #fef3c7;
  color: #92400e;
  border: 1px solid #fcd34d;
}

.cert-filter-tag button {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 1rem;
  height: 1rem;
  border-radius: 50%;
  background: rgba(0, 0, 0, 0.1);
  border: none;
  cursor: pointer;
  margin-left: 0.25rem;
  transition: all 0.2s ease;
}

.cert-filter-tag button:hover {
  background: rgba(0, 0, 0, 0.2);
}

.cert-filter-tag button i {
  font-size: 0.5rem;
}

.cert-clear-all-btn {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  padding: 0.375rem 0.75rem;
  background: none;
  border: none;
  color: #dc2626;
  font-size: 0.6875rem;
  font-weight: 500;
  cursor: pointer;
  border-radius: 0.5rem;
  transition: all 0.2s ease;
}

.cert-clear-all-btn:hover {
  background: #fef2f2;
}

/* Grid Container */
.cert-grid-container {
  padding: 1.5rem;
}

.cert-grid {
  display: grid;
  gap: 1.25rem;
}

.cert-grid.grid {
  grid-template-columns: repeat(auto-fill, minmax(340px, 1fr));
}

.cert-grid.list {
  grid-template-columns: 1fr;
}

/* Certificate Card */
.cert-card {
  background: white;
  border-radius: 1.25rem;
  overflow: hidden;
  border: 1px solid #e5e7eb;
  transition: all 0.3s ease;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.04);
}

.cert-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 16px 32px rgba(0, 0, 0, 0.1);
  border-color: #fcd34d;
}

.cert-card.list {
  display: flex;
  flex-direction: row;
}

.cert-card.list .cert-card-visual {
  width: 280px;
  min-width: 280px;
  height: auto;
}

.cert-card.list .cert-card-body {
  flex: 1;
  display: flex;
  flex-direction: column;
}

/* Card Visual */
.cert-card-visual {
  position: relative;
  height: 160px;
  overflow: hidden;
}

.cert-visual-bg {
  position: absolute;
  inset: 0;
  background: linear-gradient(135deg, var(--cert-color) 0%, color-mix(in srgb, var(--cert-color) 80%, #000) 100%);
}

.cert-visual-pattern {
  position: absolute;
  inset: 0;
  pointer-events: none;
}

.pattern-circle {
  position: absolute;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.1);
}

.pattern-circle.c1 {
  width: 120px;
  height: 120px;
  top: -30%;
  right: -10%;
}

.pattern-circle.c2 {
  width: 80px;
  height: 80px;
  bottom: -20%;
  left: -5%;
}

.pattern-ring {
  position: absolute;
  width: 200px;
  height: 200px;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 50%;
}

.cert-visual-content {
  position: absolute;
  inset: 0;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 1rem;
  z-index: 2;
}

.cert-icon-wrap {
  width: 3.5rem;
  height: 3.5rem;
  border-radius: 50%;
  background: white;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 0.75rem;
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.2);
}

.cert-icon-wrap i {
  font-size: 1.25rem;
  color: var(--cert-color);
}

.cert-visual-label {
  font-size: 0.5625rem;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.1em;
  color: rgba(255, 255, 255, 0.7);
  margin-bottom: 0.25rem;
}

.cert-visual-title {
  font-size: 0.9375rem;
  font-weight: 700;
  color: white;
  text-align: center;
  line-height: 1.3;
  max-width: 90%;
}

.cert-score-badge {
  position: absolute;
  top: 0.75rem;
  right: 0.75rem;
  display: flex;
  align-items: center;
  gap: 0.25rem;
  padding: 0.25rem 0.5rem;
  background: rgba(255, 255, 255, 0.95);
  border-radius: 0.5rem;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
  z-index: 3;
}

.cert-score-badge i {
  font-size: 0.625rem;
  color: #f59e0b;
}

.cert-score-badge span {
  font-size: 0.6875rem;
  font-weight: 700;
  color: #1f2937;
}

.cert-verified-badge {
  position: absolute;
  top: 0.75rem;
  left: 0.75rem;
  display: flex;
  align-items: center;
  gap: 0.25rem;
  padding: 0.25rem 0.5rem;
  background: #10b981;
  border-radius: 0.5rem;
  box-shadow: 0 2px 8px rgba(16, 185, 129, 0.3);
  z-index: 3;
}

.cert-verified-badge i {
  font-size: 0.625rem;
  color: white;
}

.cert-verified-badge span {
  font-size: 0.625rem;
  font-weight: 600;
  color: white;
}

/* Card Body */
.cert-card-body {
  padding: 1rem;
}

.cert-course-info {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  margin-bottom: 0.75rem;
}

.cert-course-name {
  font-size: 0.8125rem;
  font-weight: 600;
  color: #374151;
  margin-bottom: 0.25rem;
}

.cert-instructor {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  font-size: 0.6875rem;
  color: #6b7280;
}

.cert-instructor i {
  font-size: 0.625rem;
  color: #9ca3af;
}

.cert-hours-badge {
  display: flex;
  align-items: center;
  gap: 0.25rem;
  padding: 0.25rem 0.5rem;
  background: #f3f4f6;
  border-radius: 0.375rem;
  font-size: 0.625rem;
  font-weight: 500;
  color: #6b7280;
}

.cert-hours-badge i {
  font-size: 0.5625rem;
}

.cert-skills {
  display: flex;
  flex-wrap: wrap;
  gap: 0.375rem;
  margin-bottom: 0.75rem;
}

.cert-skill-tag {
  padding: 0.25rem 0.5rem;
  background: color-mix(in srgb, var(--skill-color) 10%, white);
  color: var(--skill-color);
  border-radius: 0.375rem;
  font-size: 0.625rem;
  font-weight: 500;
}

.cert-metadata {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 0.75rem 0;
  border-top: 1px solid #f3f4f6;
  margin-bottom: 0.75rem;
}

.cert-metadata span {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  font-size: 0.6875rem;
  color: #6b7280;
}

.cert-metadata i {
  font-size: 0.625rem;
  color: #9ca3af;
}

.cert-actions {
  display: flex;
  gap: 0.5rem;
}

.cert-action-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.375rem;
  padding: 0.625rem 0.875rem;
  border-radius: 0.75rem;
  font-size: 0.6875rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s ease;
  border: none;
}

.cert-action-btn.primary {
  flex: 1;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.25);
}

.cert-action-btn.primary:hover {
  transform: translateY(-1px);
  box-shadow: 0 6px 16px rgba(20, 184, 166, 0.35);
}

.cert-action-btn.secondary {
  flex: 1;
  background: white;
  border: 1px solid #e5e7eb;
  color: #374151;
}

.cert-action-btn.secondary:hover {
  background: #f9fafb;
}

.cert-action-btn.linkedin {
  background: #0077b5;
  color: white;
  padding: 0.625rem;
}

.cert-action-btn.linkedin:hover {
  background: #006699;
}

.cert-action-btn.link {
  background: white;
  border: 1px solid #e5e7eb;
  color: #6b7280;
  padding: 0.625rem;
}

.cert-action-btn.link:hover {
  background: #f9fafb;
  color: #374151;
}

/* Empty State */
.cert-empty-state {
  text-align: center;
  padding: 4rem 2rem;
}

.cert-empty-icon {
  width: 5rem;
  height: 5rem;
  border-radius: 50%;
  background: linear-gradient(135deg, #fef3c7 0%, #fde68a 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 0 auto 1.5rem;
}

.cert-empty-icon i {
  font-size: 2rem;
  color: #f59e0b;
}

.cert-empty-title {
  font-size: 1.125rem;
  font-weight: 600;
  color: #1f2937;
  margin-bottom: 0.5rem;
}

.cert-empty-text {
  font-size: 0.875rem;
  color: #6b7280;
  max-width: 24rem;
  margin: 0 auto 1.5rem;
}

.cert-empty-btn {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem 1.5rem;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
  border: none;
  border-radius: 0.75rem;
  font-size: 0.875rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s ease;
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.25);
}

.cert-empty-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 16px rgba(20, 184, 166, 0.35);
}

/* Pagination */
.cert-pagination {
  display: flex;
  align-items: center;
  justify-content: space-between;
  flex-wrap: wrap;
  gap: 1rem;
  margin-top: 1.5rem;
  padding: 1rem;
  background: #fafafa;
  border-radius: 1rem;
  border: 1px solid #f3f4f6;
}

.cert-pagination-info {
  display: flex;
  align-items: center;
  gap: 1.5rem;
  flex-wrap: wrap;
}

.cert-pagination-info > span {
  font-size: 0.75rem;
  color: #6b7280;
}

.cert-pagination-info strong {
  font-weight: 600;
  color: #374151;
}

.cert-per-page {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.75rem;
  color: #6b7280;
}

.cert-per-page select {
  padding: 0.375rem 0.625rem;
  border: 1px solid #e5e7eb;
  border-radius: 0.5rem;
  background: white;
  font-size: 0.75rem;
  color: #374151;
  cursor: pointer;
}

.cert-per-page select:focus {
  outline: none;
  border-color: #f59e0b;
}

.cert-pagination-controls {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.cert-page-btn {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  padding: 0.5rem 0.875rem;
  border: 1px solid #e5e7eb;
  border-radius: 0.5rem;
  background: white;
  font-size: 0.75rem;
  color: #6b7280;
  cursor: pointer;
  transition: all 0.2s ease;
}

.cert-page-btn:hover:not(:disabled) {
  background: #f9fafb;
  color: #374151;
}

.cert-page-btn:disabled {
  opacity: 0.4;
  cursor: not-allowed;
}

.cert-page-btn i {
  font-size: 0.625rem;
}

.cert-page-numbers {
  display: flex;
  align-items: center;
  gap: 0.25rem;
}

.cert-page-num {
  width: 2rem;
  height: 2rem;
  display: flex;
  align-items: center;
  justify-content: center;
  border: none;
  border-radius: 0.5rem;
  background: none;
  font-size: 0.75rem;
  color: #6b7280;
  cursor: pointer;
  transition: all 0.2s ease;
}

.cert-page-num:hover {
  background: #f3f4f6;
  color: #374151;
}

.cert-page-num.active {
  background: #fef3c7;
  color: #92400e;
  font-weight: 600;
}

.cert-page-dots {
  font-size: 0.75rem;
  color: #9ca3af;
  padding: 0 0.25rem;
}

/* Responsive - Tablet */
@media (max-width: 1024px) {
  .cert-stats-grid {
    grid-template-columns: repeat(2, 1fr);
  }

  .cert-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 1rem;
  }

  .cert-header-actions {
    width: 100%;
  }

  .cert-download-all-btn,
  .cert-share-all-btn {
    flex: 1;
    justify-content: center;
  }

  .cert-toolbar {
    flex-wrap: wrap;
  }

  .cert-search-wrap {
    flex: 1 1 100%;
    order: -1;
    margin-bottom: 0.5rem;
  }

  .cert-card.list {
    flex-direction: column;
  }

  .cert-card.list .cert-card-visual {
    width: 100%;
    height: 160px;
  }
}

/* Responsive - Mobile */
@media (max-width: 640px) {
  .cert-stats-grid {
    grid-template-columns: 1fr;
  }

  .cert-stat-card {
    padding: 1rem;
  }

  .cert-stat-icon {
    width: 2.5rem;
    height: 2.5rem;
    font-size: 1rem;
  }

  .cert-stat-value {
    font-size: 1.5rem;
  }

  .cert-grid.grid {
    grid-template-columns: 1fr;
  }

  .cert-pagination {
    flex-direction: column;
    align-items: stretch;
  }

  .cert-pagination-info {
    justify-content: center;
  }

  .cert-pagination-controls {
    justify-content: center;
  }
}
</style>
