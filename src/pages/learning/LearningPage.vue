<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import PageHeroHeader from '@/components/common/PageHeroHeader.vue'
import ViewAllButton from '@/components/common/ViewAllButton.vue'
import EmptyState from '@/components/common/EmptyState.vue'
import Pagination from '@/components/common/Pagination.vue'
import CategoryBadge from '@/components/common/CategoryBadge.vue'
import StatusBadge from '@/components/common/StatusBadge.vue'
import TagBadge from '@/components/common/TagBadge.vue'
import ShareContentModal from '@/components/common/ShareContentModal.vue'
import SkeletonLoader from '@/components/common/SkeletonLoader.vue'
import { useAIServicesStore } from '@/stores/aiServices'
import { AILoadingIndicator, AIConfidenceBar, AISuggestionChip } from '@/components/ai'
import type { ClassificationResult, NERResult } from '@/types/ai'

const router = useRouter()
const { t } = useI18n()
const aiStore = useAIServicesStore()

// Loading state
const isLoading = ref(false)

// ============================================================================
// AI FEATURES - Personalized Recommendations, Skill Gap Analysis, Learning Paths
// ============================================================================

// AI Processing States
const isAIAnalyzing = ref(false)
const isGeneratingRecommendations = ref(false)
const isAnalyzingSkillGaps = ref(false)
const isGeneratingLearningPath = ref(false)
const showAIInsightsPanel = ref(false)
const showSkillGapModal = ref(false)
const showAILearningPathModal = ref(false)

// AI Personalized Recommendations
interface AIPersonalizedCourse {
  courseId: number
  title: string
  reason: string
  matchScore: number
  skillsGained: string[]
  estimatedTime: string
  priority: 'high' | 'medium' | 'low'
}

const aiPersonalizedRecommendations = ref<AIPersonalizedCourse[]>([])

// AI Skill Gap Analysis
interface SkillGap {
  skill: string
  currentLevel: number
  targetLevel: number
  gap: number
  category: string
  priority: 'critical' | 'high' | 'medium' | 'low'
  relatedCourses: number[]
}

interface SkillGapAnalysis {
  overallReadiness: number
  strengths: string[]
  weaknesses: string[]
  gaps: SkillGap[]
  recommendedFocus: string[]
  estimatedTimeToClose: string
}

const skillGapAnalysis = ref<SkillGapAnalysis | null>(null)

// AI Learning Path Suggestions
interface AILearningPathStep {
  order: number
  courseId: number
  title: string
  duration: string
  skills: string[]
  dependencies: number[]
  milestone: string
}

interface AILearningPath {
  id: string
  title: string
  description: string
  goal: string
  totalDuration: string
  steps: AILearningPathStep[]
  expectedOutcomes: string[]
  careerImpact: string
  confidence: number
}

const aiLearningPaths = ref<AILearningPath[]>([])
const selectedAIPath = ref<AILearningPath | null>(null)

// AI Insights for the user
interface AILearningInsight {
  id: string
  type: 'tip' | 'milestone' | 'recommendation' | 'warning' | 'achievement'
  title: string
  message: string
  actionLabel?: string
  actionCourseId?: number
  icon: string
  color: string
}

const aiInsights = ref<AILearningInsight[]>([])

// Mock AI data for skill gap analysis
const mockSkillGapAnalysis: SkillGapAnalysis = {
  overallReadiness: 72,
  strengths: ['Data Analysis', 'Project Management', 'Communication', 'Leadership Basics'],
  weaknesses: ['Machine Learning', 'Cloud Architecture', 'Advanced Security'],
  gaps: [
    { skill: 'Machine Learning', currentLevel: 25, targetLevel: 80, gap: 55, category: 'Technology', priority: 'critical', relatedCourses: [10] },
    { skill: 'Cloud Architecture', currentLevel: 30, targetLevel: 75, gap: 45, category: 'Technology', priority: 'high', relatedCourses: [13] },
    { skill: 'Advanced Cybersecurity', currentLevel: 60, targetLevel: 90, gap: 30, category: 'Security', priority: 'high', relatedCourses: [3] },
    { skill: 'Strategic Planning', currentLevel: 45, targetLevel: 70, gap: 25, category: 'Business', priority: 'medium', relatedCourses: [11] },
    { skill: 'UX Design Principles', currentLevel: 20, targetLevel: 50, gap: 30, category: 'Design', priority: 'medium', relatedCourses: [14] },
    { skill: 'Public Speaking', currentLevel: 55, targetLevel: 80, gap: 25, category: 'Soft Skills', priority: 'low', relatedCourses: [12] }
  ],
  recommendedFocus: ['Machine Learning', 'Cloud Architecture', 'Advanced Cybersecurity'],
  estimatedTimeToClose: '45-60 hours'
}

// Mock AI personalized recommendations
const mockAIRecommendations: AIPersonalizedCourse[] = [
  {
    courseId: 10,
    title: 'Machine Learning Basics',
    reason: 'Based on your Data Analytics progress, ML is the natural next step to advance your data science career.',
    matchScore: 96,
    skillsGained: ['ML Fundamentals', 'Python ML Libraries', 'Model Training'],
    estimatedTime: '4 hours',
    priority: 'high'
  },
  {
    courseId: 13,
    title: 'Cloud Architecture Fundamentals',
    reason: 'Your project management skills combined with cloud knowledge will unlock DevOps opportunities.',
    matchScore: 89,
    skillsGained: ['AWS', 'Cloud Design', 'Scalability'],
    estimatedTime: '8 hours',
    priority: 'high'
  },
  {
    courseId: 11,
    title: 'Strategic Planning',
    reason: 'Complement your leadership training with strategic thinking to prepare for senior roles.',
    matchScore: 85,
    skillsGained: ['Strategy Development', 'Business Planning', 'Decision Making'],
    estimatedTime: '3 hours',
    priority: 'medium'
  },
  {
    courseId: 14,
    title: 'UX Design Principles',
    reason: 'Understanding UX will enhance your data visualization and project delivery skills.',
    matchScore: 78,
    skillsGained: ['User Research', 'Design Thinking', 'Prototyping'],
    estimatedTime: '5 hours',
    priority: 'medium'
  }
]

// Mock AI learning paths
const mockAILearningPaths: AILearningPath[] = [
  {
    id: 'path-data-leader',
    title: 'Data Science Leader',
    description: 'Become a data science team leader combining technical expertise with management skills',
    goal: 'Lead data science initiatives and teams',
    totalDuration: '25 hours',
    steps: [
      { order: 1, courseId: 1, title: 'Advanced Data Analytics', duration: '8h', skills: ['Analytics', 'Visualization'], dependencies: [], milestone: 'Analytics Expert' },
      { order: 2, courseId: 10, title: 'Machine Learning Basics', duration: '4h', skills: ['ML', 'Python'], dependencies: [1], milestone: 'ML Practitioner' },
      { order: 3, courseId: 17, title: 'Python for Data Science', duration: '6h', skills: ['Python', 'Data Manipulation'], dependencies: [1], milestone: 'Python Developer' },
      { order: 4, courseId: 2, title: 'Leadership Essentials', duration: '6h', skills: ['Leadership', 'Team Management'], dependencies: [], milestone: 'Team Leader' },
      { order: 5, courseId: 11, title: 'Strategic Planning', duration: '3h', skills: ['Strategy', 'Planning'], dependencies: [4], milestone: 'Strategic Thinker' }
    ],
    expectedOutcomes: ['Lead data science projects', 'Build and manage analytics teams', 'Drive data-driven decisions'],
    careerImpact: 'Positions you for Data Science Manager or Analytics Director roles',
    confidence: 94
  },
  {
    id: 'path-tech-architect',
    title: 'Technical Architect',
    description: 'Master cloud and system architecture for enterprise solutions',
    goal: 'Design scalable enterprise architectures',
    totalDuration: '30 hours',
    steps: [
      { order: 1, courseId: 13, title: 'Cloud Architecture Fundamentals', duration: '8h', skills: ['AWS', 'Cloud Design'], dependencies: [], milestone: 'Cloud Certified' },
      { order: 2, courseId: 3, title: 'Cybersecurity Fundamentals', duration: '4h', skills: ['Security', 'Compliance'], dependencies: [], milestone: 'Security Aware' },
      { order: 3, courseId: 5, title: 'Project Management Pro', duration: '10h', skills: ['Agile', 'Project Delivery'], dependencies: [], milestone: 'PM Certified' },
      { order: 4, courseId: 11, title: 'Strategic Planning', duration: '3h', skills: ['Strategy', 'Business Alignment'], dependencies: [3], milestone: 'Strategic Planner' }
    ],
    expectedOutcomes: ['Design cloud-native architectures', 'Lead technical transformations', 'Ensure security compliance'],
    careerImpact: 'Opens doors to Solutions Architect and CTO track positions',
    confidence: 88
  },
  {
    id: 'path-people-leader',
    title: 'People & Culture Leader',
    description: 'Develop comprehensive people management and organizational skills',
    goal: 'Excel in people management and organizational development',
    totalDuration: '20 hours',
    steps: [
      { order: 1, courseId: 2, title: 'Leadership Essentials', duration: '6h', skills: ['Leadership', 'Motivation'], dependencies: [], milestone: 'Emerging Leader' },
      { order: 2, courseId: 4, title: 'Effective Communication', duration: '5h', skills: ['Communication', 'Presentation'], dependencies: [], milestone: 'Communicator' },
      { order: 3, courseId: 12, title: 'Public Speaking Mastery', duration: '2h', skills: ['Public Speaking', 'Confidence'], dependencies: [2], milestone: 'Speaker' },
      { order: 4, courseId: 11, title: 'Strategic Planning', duration: '3h', skills: ['Strategy', 'Vision'], dependencies: [1], milestone: 'Strategic Leader' }
    ],
    expectedOutcomes: ['Build high-performing teams', 'Drive organizational culture', 'Communicate vision effectively'],
    careerImpact: 'Prepares for HR Director, VP of People, or COO roles',
    confidence: 91
  }
]

// Mock AI insights
const mockAIInsights: AILearningInsight[] = [
  {
    id: 'insight-1',
    type: 'milestone',
    title: 'Almost There!',
    message: 'You\'re 75% through Advanced Data Analytics. Complete 3 more lessons to earn your certificate!',
    actionLabel: 'Continue Course',
    actionCourseId: 1,
    icon: 'fas fa-trophy',
    color: 'amber'
  },
  {
    id: 'insight-2',
    type: 'recommendation',
    title: 'Skill Boost Opportunity',
    message: 'Based on your progress, Machine Learning Basics would increase your market value by 23%.',
    actionLabel: 'View Course',
    actionCourseId: 10,
    icon: 'fas fa-rocket',
    color: 'teal'
  },
  {
    id: 'insight-3',
    type: 'tip',
    title: 'Learning Pattern Detected',
    message: 'You learn best between 9-11 AM. Schedule your next session during this peak time!',
    icon: 'fas fa-lightbulb',
    color: 'blue'
  },
  {
    id: 'insight-4',
    type: 'achievement',
    title: 'Streak Champion',
    message: 'You\'ve maintained a 12-day learning streak! Keep it going for bonus rewards.',
    icon: 'fas fa-fire',
    color: 'orange'
  },
  {
    id: 'insight-5',
    type: 'warning',
    title: 'Course Expiring Soon',
    message: 'Your Leadership Essentials enrollment expires in 14 days. Resume to keep your progress.',
    actionLabel: 'Resume Now',
    actionCourseId: 2,
    icon: 'fas fa-exclamation-triangle',
    color: 'red'
  }
]

// AI Functions
async function generateAIRecommendations() {
  isGeneratingRecommendations.value = true

  // Simulate AI processing
  await new Promise(resolve => setTimeout(resolve, 1200))

  aiPersonalizedRecommendations.value = mockAIRecommendations
  isGeneratingRecommendations.value = false
}

async function analyzeSkillGaps() {
  isAnalyzingSkillGaps.value = true

  // Simulate AI analysis
  await new Promise(resolve => setTimeout(resolve, 1500))

  skillGapAnalysis.value = mockSkillGapAnalysis
  isAnalyzingSkillGaps.value = false
  showSkillGapModal.value = true
}

async function generateAILearningPaths() {
  isGeneratingLearningPath.value = true

  // Simulate AI processing
  await new Promise(resolve => setTimeout(resolve, 1800))

  aiLearningPaths.value = mockAILearningPaths
  isGeneratingLearningPath.value = false
  showAILearningPathModal.value = true
}

async function fetchAIInsights() {
  // Simulate fetching insights
  await new Promise(resolve => setTimeout(resolve, 800))
  aiInsights.value = mockAIInsights
}

function selectAIPath(path: AILearningPath) {
  selectedAIPath.value = path
}

function enrollInAIPath(path: AILearningPath) {
  // Logic to enroll user in all courses of the path
  console.log('Enrolling in AI-suggested path:', path.title)
  showAILearningPathModal.value = false
}

function dismissInsight(insightId: string) {
  aiInsights.value = aiInsights.value.filter(i => i.id !== insightId)
}

function getInsightColor(color: string) {
  const colors: Record<string, string> = {
    'teal': 'bg-teal-50 border-teal-200 text-teal-700',
    'blue': 'bg-blue-50 border-blue-200 text-blue-700',
    'amber': 'bg-amber-50 border-amber-200 text-amber-700',
    'orange': 'bg-orange-50 border-orange-200 text-orange-700',
    'red': 'bg-red-50 border-red-200 text-red-700',
    'purple': 'bg-purple-50 border-purple-200 text-purple-700'
  }
  return colors[color] || colors['teal']
}

function getInsightIconBg(color: string) {
  const colors: Record<string, string> = {
    'teal': 'bg-teal-500',
    'blue': 'bg-blue-500',
    'amber': 'bg-amber-500',
    'orange': 'bg-orange-500',
    'red': 'bg-red-500',
    'purple': 'bg-purple-500'
  }
  return colors[color] || colors['teal']
}

function getPriorityColor(priority: string) {
  switch (priority) {
    case 'critical': return 'bg-red-100 text-red-700 border-red-200'
    case 'high': return 'bg-orange-100 text-orange-700 border-orange-200'
    case 'medium': return 'bg-blue-100 text-blue-700 border-blue-200'
    case 'low': return 'bg-gray-100 text-gray-700 border-gray-200'
    default: return 'bg-gray-100 text-gray-700'
  }
}

function getSkillGapColor(gap: number) {
  if (gap >= 50) return 'text-red-500'
  if (gap >= 30) return 'text-orange-500'
  if (gap >= 15) return 'text-amber-500'
  return 'text-green-500'
}

// Initialize AI features on mount
onMounted(() => {
  generateAIRecommendations()
  fetchAIInsights()
})

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
const currentView = ref<'all' | 'my-courses' | 'paths' | 'lessons-learned' | 'certificates'>('all')
const viewMode = ref<'grid' | 'list'>('grid')

// Share modal
const showShareModal = ref(false)
const shareCourseData = ref<any>(null)
const shareCourseUrl = computed(() => {
  if (!shareCourseData.value) return ''
  return `${window.location.origin}/learning/courses/${shareCourseData.value.id}`
})

function shareCourse(course: any) {
  shareCourseData.value = course
  showShareModal.value = true
}

// Stats
const overallProgress = ref(68)
const completedCourses = ref(8)
const totalCourses = ref(12)
const learningHours = ref(45)
const certificates = ref(5)
const streak = ref(12)

// Hero stats for PageHeroHeader component
const heroStats = computed(() => [
  { icon: 'fas fa-graduation-cap', value: `${completedCourses.value}/${totalCourses.value}`, label: t('common.completed') },
  { icon: 'fas fa-clock', value: `${learningHours.value}h`, label: t('nav.learning') },
  { icon: 'fas fa-certificate', value: certificates.value, label: t('learning.certificates') },
  { icon: 'fas fa-fire', value: streak.value, label: t('learning.dayStreak') }
])
const totalEnrolled = ref(156)

// View options (Collections style)
const viewOptions = ref([
  { id: 'all', name: 'All Courses', icon: 'fas fa-graduation-cap', color: 'text-teal-500' },
  { id: 'my-courses', name: 'My Courses', icon: 'fas fa-book-reader', color: 'text-blue-500' },
  { id: 'paths', name: 'Learning Paths', icon: 'fas fa-route', color: 'text-indigo-500' },
  { id: 'lessons-learned', name: 'Lessons Learned', icon: 'fas fa-lightbulb', color: 'text-amber-500' },
  { id: 'certificates', name: 'Certificates', icon: 'fas fa-certificate', color: 'text-teal-500' },
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

// ============================================
// LESSONS LEARNED DATA
// ============================================

type LessonPriority = 'low' | 'medium' | 'high' | 'critical'
type LessonCategory = 'technical' | 'process' | 'communication' | 'leadership' | 'project' | 'other'

interface LessonLearned {
  id: string
  title: string
  summary: string
  content: string
  category: LessonCategory
  priority: LessonPriority
  tags: string[]
  author: {
    id: string
    name: string
    initials: string
    avatar?: string
    department?: string
  }
  createdAt: string
  viewCount: number
  likeCount: number
  commentCount: number
  isLiked?: boolean
  isBookmarked?: boolean
  isFeatured?: boolean
  projectName?: string
  impact?: string
  recommendations?: string[]
}

const lessonsLearned = ref<LessonLearned[]>([
  {
    id: '1',
    title: 'Effective Stakeholder Communication During Project Delays',
    summary: 'Key strategies for maintaining stakeholder trust and engagement when facing unexpected project timeline extensions.',
    content: 'When our venue construction project faced a 3-month delay due to supply chain issues, we learned that proactive, transparent communication was essential. We implemented weekly stakeholder updates, created a visual timeline dashboard, and held bi-weekly town halls. The key was acknowledging issues early and presenting mitigation plans alongside problems.',
    category: 'communication',
    priority: 'high',
    tags: ['Stakeholder Management', 'Communication', 'Crisis Management'],
    author: { id: '1', name: 'Sarah Chen', initials: 'SC', department: 'Project Management' },
    createdAt: 'Jan 15, 2026',
    viewCount: 234,
    likeCount: 45,
    commentCount: 12,
    isLiked: true,
    isBookmarked: false,
    isFeatured: true,
    projectName: 'Stadium Construction Phase 2',
    impact: 'Maintained 95% stakeholder satisfaction despite delays',
    recommendations: ['Create communication templates for different scenarios', 'Establish escalation protocols early', 'Use visual dashboards for complex timelines']
  },
  {
    id: '2',
    title: 'API Integration Best Practices for Large-Scale Systems',
    summary: 'Lessons from integrating multiple third-party APIs including error handling, rate limiting, and fallback strategies.',
    content: 'During the ticketing system integration, we connected with 12 different payment gateways and 8 regional ticketing partners. Key learnings included implementing circuit breakers, maintaining detailed API versioning, and creating comprehensive fallback mechanisms. We reduced integration failures by 78% after implementing these practices.',
    category: 'technical',
    priority: 'critical',
    tags: ['API', 'Integration', 'Architecture', 'Resilience'],
    author: { id: '2', name: 'James Wilson', initials: 'JW', department: 'Engineering' },
    createdAt: 'Jan 12, 2026',
    viewCount: 456,
    likeCount: 89,
    commentCount: 23,
    isLiked: false,
    isBookmarked: true,
    isFeatured: true,
    projectName: 'Unified Ticketing Platform',
    impact: '78% reduction in integration failures, 99.9% uptime achieved',
    recommendations: ['Implement circuit breaker patterns', 'Version all APIs from day one', 'Build comprehensive fallback mechanisms', 'Monitor API health in real-time']
  },
  {
    id: '3',
    title: 'Building High-Performance Remote Teams',
    summary: 'Practical insights on fostering collaboration, maintaining culture, and driving productivity in distributed teams.',
    content: 'Managing a team of 45 people across 6 time zones for the Asian Cup 2027 project required rethinking our collaboration approach. We established core overlap hours, implemented async-first communication, and created virtual team rituals. Regular in-person meetups every quarter helped maintain team cohesion.',
    category: 'leadership',
    priority: 'high',
    tags: ['Remote Work', 'Team Building', 'Leadership', 'Culture'],
    author: { id: '3', name: 'Maria Garcia', initials: 'MG', department: 'Operations' },
    createdAt: 'Jan 10, 2026',
    viewCount: 312,
    likeCount: 67,
    commentCount: 18,
    isLiked: true,
    isBookmarked: true,
    isFeatured: false,
    projectName: 'Global Operations Setup',
    impact: 'Team productivity increased by 32%, retention rate improved to 94%',
    recommendations: ['Define core overlap hours for all time zones', 'Document everything asynchronously', 'Invest in quarterly in-person gatherings', 'Use video for important discussions']
  },
  {
    id: '4',
    title: 'Agile Sprint Planning: What Works and What Doesn\'t',
    summary: 'Refined approaches to sprint planning based on real-world experience across multiple project types.',
    content: 'After 18 months of sprints across 5 different project teams, we identified patterns that consistently led to successful delivery. Two-week sprints with proper backlog grooming sessions, limiting work-in-progress, and having clear definition of done were game-changers. We also learned that sprint planning should never exceed 2 hours.',
    category: 'process',
    priority: 'medium',
    tags: ['Agile', 'Sprint Planning', 'Scrum', 'Best Practices'],
    author: { id: '4', name: 'David Lee', initials: 'DL', department: 'Product' },
    createdAt: 'Jan 8, 2026',
    viewCount: 189,
    likeCount: 34,
    commentCount: 8,
    isLiked: false,
    isBookmarked: false,
    isFeatured: false,
    projectName: 'Multiple Projects',
    impact: 'Sprint completion rate improved from 65% to 89%',
    recommendations: ['Keep sprints to 2 weeks maximum', 'Groom backlog before sprint planning', 'Limit WIP to team capacity', 'Time-box planning to 2 hours']
  },
  {
    id: '5',
    title: 'Database Migration Strategies Without Downtime',
    summary: 'Step-by-step approach to migrating production databases while maintaining zero downtime and data integrity.',
    content: 'Migrating 2.5TB of production data from legacy Oracle to PostgreSQL required careful planning. We used dual-write patterns, implemented data validation checksums, and ran parallel systems for 3 weeks before the cutover. The blue-green deployment strategy allowed instant rollback capability throughout the migration.',
    category: 'technical',
    priority: 'critical',
    tags: ['Database', 'Migration', 'DevOps', 'Zero Downtime'],
    author: { id: '5', name: 'Alex Thompson', initials: 'AT', department: 'Infrastructure' },
    createdAt: 'Jan 5, 2026',
    viewCount: 567,
    likeCount: 112,
    commentCount: 31,
    isLiked: true,
    isBookmarked: false,
    isFeatured: true,
    projectName: 'Database Modernization',
    impact: 'Zero downtime achieved, 40% cost reduction in database licensing',
    recommendations: ['Use dual-write pattern during transition', 'Implement data validation checksums', 'Run parallel systems before cutover', 'Always have rollback capability']
  },
  {
    id: '6',
    title: 'Managing Scope Creep in Complex Projects',
    summary: 'Proven techniques for identifying, preventing, and handling scope creep while maintaining client satisfaction.',
    content: 'The media center project started with 45 requirements and ended with 127. We learned to implement strict change control processes, maintain a parking lot for future features, and communicate the impact of changes clearly. Weekly scope reviews and a change request board helped keep everyone aligned.',
    category: 'project',
    priority: 'high',
    tags: ['Project Management', 'Scope Management', 'Change Control'],
    author: { id: '6', name: 'Emma Wilson', initials: 'EW', department: 'Project Management' },
    createdAt: 'Jan 3, 2026',
    viewCount: 278,
    likeCount: 56,
    commentCount: 15,
    isLiked: false,
    isBookmarked: true,
    isFeatured: false,
    projectName: 'Media Center Development',
    impact: 'Reduced unplanned scope changes by 60%',
    recommendations: ['Implement formal change control process', 'Maintain a parking lot for future ideas', 'Communicate impact of every change', 'Hold weekly scope review meetings']
  },
  {
    id: '7',
    title: 'Vendor Management for Large-Scale Events',
    summary: 'Best practices for coordinating multiple vendors while ensuring quality, timeline adherence, and cost control.',
    content: 'Coordinating 35+ vendors for the opening ceremony required a centralized vendor management system, clear SLAs, and regular performance reviews. We created vendor scorecards, established communication channels, and built contingency plans for each critical vendor.',
    category: 'project',
    priority: 'high',
    tags: ['Vendor Management', 'Events', 'Procurement', 'SLA'],
    author: { id: '7', name: 'Robert Kim', initials: 'RK', department: 'Procurement' },
    createdAt: 'Dec 28, 2025',
    viewCount: 198,
    likeCount: 42,
    commentCount: 9,
    isLiked: false,
    isBookmarked: false,
    isFeatured: false,
    projectName: 'Opening Ceremony Planning',
    impact: '100% on-time delivery from critical vendors',
    recommendations: ['Create vendor scorecards', 'Establish clear SLAs upfront', 'Build contingency plans for critical vendors', 'Hold regular performance reviews']
  },
  {
    id: '8',
    title: 'Security Best Practices for Public-Facing Applications',
    summary: 'Essential security measures learned from building high-traffic, public-facing tournament applications.',
    content: 'With millions of expected users, security was paramount. We implemented defense-in-depth, conducted regular penetration testing, and established bug bounty programs. Key learnings included the importance of input validation, secure session management, and real-time threat monitoring.',
    category: 'technical',
    priority: 'critical',
    tags: ['Security', 'Application Security', 'Best Practices'],
    author: { id: '8', name: 'Lisa Park', initials: 'LP', department: 'Security' },
    createdAt: 'Dec 20, 2025',
    viewCount: 423,
    likeCount: 98,
    commentCount: 27,
    isLiked: true,
    isBookmarked: true,
    isFeatured: false,
    projectName: 'Fan Portal Security',
    impact: 'Zero security breaches, passed all compliance audits',
    recommendations: ['Implement defense-in-depth strategy', 'Conduct regular penetration testing', 'Establish bug bounty program', 'Monitor threats in real-time']
  }
])

// Lessons Learned filter & search state
const lessonsSearch = ref('')
const selectedLessonsCategories = ref<string[]>([])
const selectedLessonsPriorities = ref<string[]>([])
const selectedLessonsTags = ref<string[]>([])
const showBookmarkedOnly = ref(false)
const lessonsSortBy = ref('newest')
const lessonsSortOrder = ref<'asc' | 'desc'>('desc')
const showLessonsCategoryDropdown = ref(false)
const showLessonsPriorityDropdown = ref(false)
const showLessonsTagsDropdown = ref(false)
const showLessonsSortDropdown = ref(false)
const lessonsViewMode = ref<'grid' | 'list'>('grid')
const lessonsCurrentPage = ref(1)
const lessonsItemsPerPage = ref(12)
const lessonsItemsPerPageOptions = [6, 12, 24, 48]

// Legacy compatibility (keep for backward compat with some components)
const lessonsCategoryFilter = ref<LessonCategory | 'all'>('all')
const lessonsPriorityFilter = ref<LessonPriority | 'all'>('all')

// Lessons category options
const lessonsCategoryOptions = [
  { id: 'all', label: 'All Categories', icon: 'fas fa-th-large', color: 'text-gray-500' },
  { id: 'technical', label: 'Technical', icon: 'fas fa-code', color: 'text-blue-500' },
  { id: 'process', label: 'Process', icon: 'fas fa-cogs', color: 'text-purple-500' },
  { id: 'communication', label: 'Communication', icon: 'fas fa-comments', color: 'text-teal-500' },
  { id: 'leadership', label: 'Leadership', icon: 'fas fa-crown', color: 'text-indigo-500' },
  { id: 'project', label: 'Project', icon: 'fas fa-tasks', color: 'text-cyan-500' },
  { id: 'other', label: 'Other', icon: 'fas fa-folder', color: 'text-gray-500' }
]

// Lessons priority options
const lessonsPriorityOptions = [
  { id: 'all', label: 'All Priorities', color: 'bg-gray-100 text-gray-700' },
  { id: 'critical', label: 'Critical', color: 'bg-red-100 text-red-700' },
  { id: 'high', label: 'High', color: 'bg-orange-100 text-orange-700' },
  { id: 'medium', label: 'Medium', color: 'bg-amber-100 text-amber-700' },
  { id: 'low', label: 'Low', color: 'bg-green-100 text-green-700' }
]

// Lessons sort options
const lessonsSortOptions = [
  { value: 'newest', label: 'Newest First', icon: 'fas fa-clock' },
  { value: 'popular', label: 'Most Popular', icon: 'fas fa-fire' },
  { value: 'liked', label: 'Most Liked', icon: 'fas fa-heart' },
  { value: 'title', label: 'Title A-Z', icon: 'fas fa-sort-alpha-down' }
]

// All unique tags from lessons
const allLessonsTags = computed(() => {
  const tags = new Set<string>()
  lessonsLearned.value.forEach(l => l.tags.forEach(t => tags.add(t)))
  return Array.from(tags).sort()
})

// Helper functions for multi-select filters
function toggleLessonsCategoryFilter(categoryId: string) {
  const index = selectedLessonsCategories.value.indexOf(categoryId)
  if (index > -1) {
    selectedLessonsCategories.value.splice(index, 1)
  } else {
    selectedLessonsCategories.value.push(categoryId)
  }
  lessonsCurrentPage.value = 1
}

function toggleLessonsPriorityFilter(priorityId: string) {
  const index = selectedLessonsPriorities.value.indexOf(priorityId)
  if (index > -1) {
    selectedLessonsPriorities.value.splice(index, 1)
  } else {
    selectedLessonsPriorities.value.push(priorityId)
  }
  lessonsCurrentPage.value = 1
}

function toggleLessonsTagFilter(tag: string) {
  const index = selectedLessonsTags.value.indexOf(tag)
  if (index > -1) {
    selectedLessonsTags.value.splice(index, 1)
  } else {
    selectedLessonsTags.value.push(tag)
  }
  lessonsCurrentPage.value = 1
}

function isLessonsCategorySelected(categoryId: string) {
  return selectedLessonsCategories.value.includes(categoryId)
}

function isLessonsPrioritySelected(priorityId: string) {
  return selectedLessonsPriorities.value.includes(priorityId)
}

function isLessonsTagSelected(tag: string) {
  return selectedLessonsTags.value.includes(tag)
}

// Filtered lessons
const filteredLessons = computed(() => {
  let lessons = [...lessonsLearned.value]

  // Search filter
  if (lessonsSearch.value.trim()) {
    const search = lessonsSearch.value.toLowerCase()
    lessons = lessons.filter(l =>
      l.title.toLowerCase().includes(search) ||
      l.summary.toLowerCase().includes(search) ||
      l.tags.some(t => t.toLowerCase().includes(search))
    )
  }

  // Category filter (multi-select)
  if (selectedLessonsCategories.value.length > 0) {
    lessons = lessons.filter(l => selectedLessonsCategories.value.includes(l.category))
  }

  // Priority filter (multi-select)
  if (selectedLessonsPriorities.value.length > 0) {
    lessons = lessons.filter(l => selectedLessonsPriorities.value.includes(l.priority))
  }

  // Tags filter (multi-select)
  if (selectedLessonsTags.value.length > 0) {
    lessons = lessons.filter(l => selectedLessonsTags.value.some(tag => l.tags.includes(tag)))
  }

  // Bookmarked filter
  if (showBookmarkedOnly.value) {
    lessons = lessons.filter(l => l.isBookmarked)
  }

  // Sort
  const multiplier = lessonsSortOrder.value === 'asc' ? 1 : -1
  switch (lessonsSortBy.value) {
    case 'newest':
      // Already sorted by date in mock data
      break
    case 'popular':
      lessons.sort((a, b) => (b.viewCount - a.viewCount) * multiplier)
      break
    case 'liked':
      lessons.sort((a, b) => (b.likeCount - a.likeCount) * multiplier)
      break
    case 'title':
      lessons.sort((a, b) => a.title.localeCompare(b.title) * multiplier)
      break
  }

  return lessons
})

// Paginated lessons
const paginatedLessons = computed(() => {
  const start = (lessonsCurrentPage.value - 1) * lessonsItemsPerPage.value
  const end = start + lessonsItemsPerPage.value
  return filteredLessons.value.slice(start, end)
})

// Total pages for lessons
const lessonsTotalPages = computed(() =>
  Math.ceil(filteredLessons.value.length / lessonsItemsPerPage.value) || 1
)

// Pagination functions
function lessonsGoToPage(page: number) {
  lessonsCurrentPage.value = page
}

function lessonsPrevPage() {
  if (lessonsCurrentPage.value > 1) {
    lessonsCurrentPage.value--
  }
}

function lessonsNextPage() {
  if (lessonsCurrentPage.value < lessonsTotalPages.value) {
    lessonsCurrentPage.value++
  }
}

function changeLessonsItemsPerPage(value: number) {
  lessonsItemsPerPage.value = value
  lessonsCurrentPage.value = 1
}

// Lessons stats
const lessonsStats = computed(() => ({
  total: lessonsLearned.value.length,
  thisMonth: lessonsLearned.value.filter(l => l.createdAt.includes('Jan')).length,
  critical: lessonsLearned.value.filter(l => l.priority === 'critical').length,
  bookmarked: lessonsLearned.value.filter(l => l.isBookmarked).length,
  totalViews: lessonsLearned.value.reduce((sum, l) => sum + l.viewCount, 0),
  totalLikes: lessonsLearned.value.reduce((sum, l) => sum + l.likeCount, 0)
}))

// Featured lessons
const featuredLessons = computed(() =>
  lessonsLearned.value.filter(l => l.isFeatured).slice(0, 5)
)

// Featured lessons carousel state
const currentFeaturedLessonIndex = ref(0)
const featuredLessonInterval = ref<number | null>(null)

const currentFeaturedLesson = computed(() =>
  featuredLessons.value[currentFeaturedLessonIndex.value] || featuredLessons.value[0]
)

function nextFeaturedLesson() {
  if (featuredLessons.value.length > 0) {
    currentFeaturedLessonIndex.value = (currentFeaturedLessonIndex.value + 1) % featuredLessons.value.length
  }
}

function prevFeaturedLesson() {
  if (featuredLessons.value.length > 0) {
    currentFeaturedLessonIndex.value = currentFeaturedLessonIndex.value === 0
      ? featuredLessons.value.length - 1
      : currentFeaturedLessonIndex.value - 1
  }
}

function goToFeaturedLesson(index: number) {
  currentFeaturedLessonIndex.value = index
}

function startFeaturedLessonAutoPlay() {
  if (featuredLessonInterval.value) return
  featuredLessonInterval.value = window.setInterval(() => {
    nextFeaturedLesson()
  }, 5000)
}

function pauseFeaturedLessonAutoPlay() {
  if (featuredLessonInterval.value) {
    clearInterval(featuredLessonInterval.value)
    featuredLessonInterval.value = null
  }
}

function resumeFeaturedLessonAutoPlay() {
  startFeaturedLessonAutoPlay()
}

// Next lessons for sidebar (excluding current)
const nextFeaturedLessons = computed(() => {
  if (featuredLessons.value.length <= 1) return []
  const lessons = []
  for (let i = 1; i <= 3; i++) {
    const idx = (currentFeaturedLessonIndex.value + i) % featuredLessons.value.length
    if (idx !== currentFeaturedLessonIndex.value) {
      lessons.push({ ...featuredLessons.value[idx], displayIndex: idx })
    }
  }
  return lessons.slice(0, 3)
})

// Category stats for quick filter chips
const categoryStats = computed(() => {
  const stats: Record<string, number> = {}
  lessonsLearned.value.forEach(l => {
    stats[l.category] = (stats[l.category] || 0) + 1
  })
  return stats
})

// Detail modal state
const showLessonDetailModal = ref(false)
const selectedLessonForModal = ref<LessonLearned | null>(null)

// Open lesson detail modal
function openLessonDetail(lesson: LessonLearned) {
  selectedLessonForModal.value = lesson
  showLessonDetailModal.value = true
}

// Close lesson detail modal
function closeLessonDetail() {
  showLessonDetailModal.value = false
  selectedLessonForModal.value = null
}

// Get lesson category info
function getLessonCategoryInfo(category: LessonCategory) {
  return lessonsCategoryOptions.find(c => c.id === category) || lessonsCategoryOptions[0]
}

// Get lesson priority color
function getLessonPriorityColor(priority: LessonPriority) {
  switch (priority) {
    case 'critical': return 'bg-red-100 text-red-700 border-red-200'
    case 'high': return 'bg-orange-100 text-orange-700 border-orange-200'
    case 'medium': return 'bg-amber-100 text-amber-700 border-amber-200'
    case 'low': return 'bg-green-100 text-green-700 border-green-200'
    default: return 'bg-gray-100 text-gray-700 border-gray-200'
  }
}

// Toggle lesson like
function toggleLessonLike(lessonId: string) {
  const lesson = lessonsLearned.value.find(l => l.id === lessonId)
  if (lesson) {
    lesson.isLiked = !lesson.isLiked
    lesson.likeCount += lesson.isLiked ? 1 : -1
  }
}

// Toggle lesson bookmark
function toggleLessonBookmark(lessonId: string) {
  const lesson = lessonsLearned.value.find(l => l.id === lessonId)
  if (lesson) {
    lesson.isBookmarked = !lesson.isBookmarked
  }
}

// Selected lesson for detail view
const selectedLesson = ref<LessonLearned | null>(null)

// View lesson detail (expand card)
function viewLessonDetail(lesson: LessonLearned) {
  selectedLesson.value = selectedLesson.value?.id === lesson.id ? null : lesson
}

// Clear lessons filters
function clearLessonsFilters() {
  lessonsSearch.value = ''
  selectedLessonsCategories.value = []
  selectedLessonsPriorities.value = []
  selectedLessonsTags.value = []
  showBookmarkedOnly.value = false
  lessonsSortBy.value = 'newest'
  lessonsSortOrder.value = 'desc'
  lessonsCurrentPage.value = 1
}

// Active lessons filters count
const activeLessonsFiltersCount = computed(() => {
  let count = 0
  if (lessonsSearch.value.trim()) count++
  count += selectedLessonsCategories.value.length
  count += selectedLessonsPriorities.value.length
  count += selectedLessonsTags.value.length
  if (showBookmarkedOnly.value) count++
  return count
})

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
    <PageHeroHeader
      :stats="heroStats"
      badge-icon="fas fa-graduation-cap"
      :badge-text="$t('app.name')"
      :title="$t('learning.title')"
      :subtitle="$t('learning.subtitle')"
    >
      <template #actions>
        <button v-if="currentCourse" @click="navigateToCourse(currentCourse.id)" class="px-5 py-2.5 bg-white text-teal-600 rounded-xl font-semibold text-sm flex items-center gap-2 hover:bg-teal-50 transition-all shadow-lg">
          <i class="fas fa-play"></i>
          {{ $t('learning.continueLearning') }}
        </button>
        <!-- AI Action Buttons -->
        <button
          @click="analyzeSkillGaps"
          :disabled="isAnalyzingSkillGaps"
          class="px-5 py-2.5 bg-gradient-to-r from-purple-500 to-indigo-500 text-white rounded-xl font-semibold text-sm flex items-center gap-2 hover:from-purple-600 hover:to-indigo-600 transition-all shadow-lg disabled:opacity-50"
        >
          <i :class="isAnalyzingSkillGaps ? 'fas fa-spinner fa-spin' : 'fas fa-wand-magic-sparkles'"></i>
          {{ isAnalyzingSkillGaps ? $t('learning.analyzing') : $t('learning.skillGapAnalysis') }}
        </button>
        <button
          @click="generateAILearningPaths"
          :disabled="isGeneratingLearningPath"
          class="px-5 py-2.5 bg-gradient-to-r from-teal-500 to-cyan-500 text-white rounded-xl font-semibold text-sm flex items-center gap-2 hover:from-teal-600 hover:to-cyan-600 transition-all shadow-lg disabled:opacity-50"
        >
          <i :class="isGeneratingLearningPath ? 'fas fa-spinner fa-spin' : 'fas fa-route'"></i>
          {{ isGeneratingLearningPath ? $t('learning.generating') : $t('learning.aiLearningPaths') }}
        </button>
      </template>
    </PageHeroHeader>

    <!-- AI Insights Panel -->
    <div v-if="aiInsights.length > 0" class="ai-insights-bar">
      <div class="ai-insights-header">
        <div class="flex items-center gap-2">
          <div class="ai-icon-badge">
            <i class="fas fa-wand-magic-sparkles"></i>
          </div>
          <span class="font-semibold text-gray-800">{{ $t('learning.aiInsights') }}</span>
          <span class="text-xs px-2 py-0.5 bg-teal-100 text-teal-700 rounded-full">{{ aiInsights.length }} new</span>
        </div>
        <button @click="showAIInsightsPanel = !showAIInsightsPanel" class="text-gray-500 hover:text-gray-700">
          <i :class="showAIInsightsPanel ? 'fas fa-chevron-up' : 'fas fa-chevron-down'"></i>
        </button>
      </div>
      <transition name="slide">
        <div v-if="showAIInsightsPanel" class="ai-insights-content">
          <div
            v-for="insight in aiInsights"
            :key="insight.id"
            :class="['ai-insight-card', getInsightColor(insight.color)]"
          >
            <div class="flex items-start gap-3">
              <div :class="['ai-insight-icon', getInsightIconBg(insight.color)]">
                <i :class="insight.icon"></i>
              </div>
              <div class="flex-1">
                <h4 class="font-semibold text-sm">{{ insight.title }}</h4>
                <p class="text-xs mt-0.5 opacity-90">{{ insight.message }}</p>
                <button
                  v-if="insight.actionLabel"
                  @click="navigateToCourse(insight.actionCourseId!)"
                  class="mt-2 text-xs font-semibold underline hover:no-underline"
                >
                  {{ insight.actionLabel }}
                </button>
              </div>
              <button @click="dismissInsight(insight.id)" class="text-gray-400 hover:text-gray-600 p-1">
                <i class="fas fa-times text-xs"></i>
              </button>
            </div>
          </div>
        </div>
      </transition>
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

        <!-- Featured Courses Section (All Courses view only) -->
        <div v-if="currentView === 'all' && enrolledCourses.length > 0" class="featured-courses-section">
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
                  <StatusBadge status="featured" variant="gradient" size="sm" />
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
                    <TagBadge v-for="tag in featuredCourse.tags" :key="tag" :tag="tag" size="xs" variant="glass" />
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
            <ViewAllButton size="sm" />
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
                  <TagBadge v-for="tag in course.tags" :key="tag" :tag="tag" size="xs" />
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
            <ViewAllButton size="sm" />
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
        <div v-if="currentView === 'all' || currentView === 'my-courses'" class="all-courses-wrapper bg-white rounded-2xl shadow-sm border border-gray-100 overflow-hidden">
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
                    <StatusBadge v-if="course.enrolled && course.statusClass === 'completed'" status="completed" size="xs" />
                    <StatusBadge v-else-if="course.enrolled && course.statusClass === 'in-progress'" status="in-progress" size="xs" />
                    <StatusBadge v-else-if="course.enrolled" status="pending" :label="course.status" size="xs" />
                    <StatusBadge v-if="course.isNew" status="new" size="xs" variant="gradient" />
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
                    <CategoryBadge :category="course.category" size="xs" />
                  </p>

                  <!-- Tags -->
                  <div class="flex flex-wrap gap-1.5 mb-3">
                    <TagBadge
                      v-for="tag in course.tags.slice(0, 3)"
                      :key="tag"
                      :tag="tag"
                      size="xs"
                    />
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
                      <CategoryBadge :category="course.category" size="xs" />
                      <StatusBadge v-if="course.enrolled && course.statusClass === 'completed'" status="completed" size="xs" />
                      <StatusBadge v-else-if="course.enrolled && course.statusClass === 'in-progress'" status="in-progress" size="xs" />
                      <StatusBadge v-else-if="course.enrolled" status="pending" :label="course.status" size="xs" />
                      <StatusBadge v-if="course.isNew" status="new" size="xs" variant="gradient" />
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
            <Pagination
              v-if="displayedCourses.length > 0"
              v-model:current-page="coursesCurrentPage"
              v-model:items-per-page="coursesItemsPerPage"
              :total-items="displayedCourses.length"
              :items-per-page-options="coursesItemsPerPageOptions"
              class="mt-4"
            />
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
            <ViewAllButton size="sm" />
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
                    <StatusBadge v-if="path.isEnrolled && path.progress === 100" status="completed" size="xs" />
                    <StatusBadge v-else-if="path.isEnrolled && path.progress > 0" status="in-progress" size="xs" />
                    <StatusBadge v-else-if="path.isEnrolled" status="pending" label="Not Started" size="xs" />
                    <StatusBadge v-if="path.enrolled > 1000" status="popular" size="xs" variant="gradient" />
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
                    <TagBadge
                      v-for="skill in path.skills.slice(0, 3)"
                      :key="skill"
                      :tag="skill"
                      size="xs"
                    />
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
            <EmptyState
              v-if="paginatedPaths.length === 0"
              icon="fas fa-route"
              title="No learning paths found"
              description="Try adjusting your filter selection"
              action-label="Reset Filters"
              action-icon="fas fa-redo"
              @action="clearAllPathsFilters"
            />

            <!-- Pagination Footer -->
            <Pagination
              v-if="filteredPaths.length > 0"
              v-model:current-page="pathsCurrentPage"
              v-model:items-per-page="pathsItemsPerPage"
              :total-items="filteredPaths.length"
              :items-per-page-options="pathsItemsPerPageOptions"
              class="mt-4"
            />
          </div>
        </div>
      </div>

      <!-- Lessons Learned Section -->
      <div v-if="currentView === 'lessons-learned'" class="lessons-learned-section">

        <!-- Featured Insights Section -->
        <div v-if="featuredLessons.length > 0" class="ll-featured-section" @mouseenter="pauseFeaturedLessonAutoPlay" @mouseleave="resumeFeaturedLessonAutoPlay">
          <!-- Section Header -->
          <div class="ll-featured-header">
            <div class="ll-featured-title-wrap">
              <div class="ll-featured-icon">
                <i class="fas fa-sparkles"></i>
              </div>
              <div>
                <h2 class="ll-featured-title">Featured Insights</h2>
                <p class="ll-featured-subtitle">Discover top lessons from our knowledge base</p>
              </div>
            </div>
            <div class="ll-featured-nav">
              <span class="ll-featured-counter">{{ currentFeaturedLessonIndex + 1 }} / {{ featuredLessons.length }}</span>
              <button @click="prevFeaturedLesson" class="ll-nav-btn" :disabled="featuredLessons.length <= 1">
                <i class="fas fa-chevron-left"></i>
              </button>
              <button @click="nextFeaturedLesson" class="ll-nav-btn" :disabled="featuredLessons.length <= 1">
                <i class="fas fa-chevron-right"></i>
              </button>
            </div>
          </div>

          <!-- Two Column Layout -->
          <div class="ll-featured-content">
            <!-- Main Featured Card -->
            <div class="ll-main-card-wrap">
              <div v-if="currentFeaturedLesson" class="ll-main-card" @click="openLessonDetail(currentFeaturedLesson)">
                <!-- Decorative Elements -->
                <div class="ll-card-glow"></div>
                <div class="ll-card-pattern"></div>

                <div class="ll-main-card-inner">
                  <!-- Top Section -->
                  <div class="ll-main-top">
                    <div class="ll-main-badges">
                      <span class="ll-main-featured-badge">
                        <i class="fas fa-star"></i> Featured
                      </span>
                      <span :class="['ll-main-priority', 'priority-' + currentFeaturedLesson.priority]">
                        <i class="fas fa-flag"></i> {{ currentFeaturedLesson.priority }}
                      </span>
                    </div>
                    <span class="ll-main-category">
                      <i :class="getLessonCategoryInfo(currentFeaturedLesson.category).icon"></i>
                      {{ getLessonCategoryInfo(currentFeaturedLesson.category).label }}
                    </span>
                  </div>

                  <!-- Content Section -->
                  <div class="ll-main-content">
                    <h3 class="ll-main-title">{{ currentFeaturedLesson.title }}</h3>
                    <p class="ll-main-summary">{{ currentFeaturedLesson.summary }}</p>

                    <!-- Impact Highlight -->
                    <div v-if="currentFeaturedLesson.impact" class="ll-main-impact">
                      <div class="ll-impact-icon-wrap">
                        <i class="fas fa-bolt"></i>
                      </div>
                      <div class="ll-impact-content">
                        <span class="ll-impact-label">Key Impact</span>
                        <span class="ll-impact-text">{{ currentFeaturedLesson.impact }}</span>
                      </div>
                    </div>

                    <!-- Recommendations Preview -->
                    <div v-if="currentFeaturedLesson.recommendations?.length" class="ll-main-takeaways">
                      <div class="ll-takeaway-header">
                        <i class="fas fa-lightbulb"></i>
                        <span>Quick Takeaways</span>
                      </div>
                      <div class="ll-takeaway-list">
                        <div v-for="(rec, idx) in currentFeaturedLesson.recommendations.slice(0, 2)" :key="idx" class="ll-takeaway-item">
                          <span class="ll-takeaway-num">{{ idx + 1 }}</span>
                          <span class="ll-takeaway-text">{{ rec }}</span>
                        </div>
                      </div>
                    </div>
                  </div>

                  <!-- Footer Section -->
                  <div class="ll-main-footer">
                    <div class="ll-main-author">
                      <div class="ll-main-avatar">{{ currentFeaturedLesson.author.initials }}</div>
                      <div class="ll-main-author-info">
                        <span class="ll-main-author-name">{{ currentFeaturedLesson.author.name }}</span>
                        <span class="ll-main-author-meta">{{ currentFeaturedLesson.author.department }}</span>
                      </div>
                    </div>
                    <div class="ll-main-stats">
                      <span class="ll-main-stat"><i class="fas fa-eye"></i> {{ currentFeaturedLesson.viewCount }}</span>
                      <span class="ll-main-stat"><i class="fas fa-heart"></i> {{ currentFeaturedLesson.likeCount }}</span>
                      <span class="ll-main-stat"><i class="fas fa-comment"></i> {{ currentFeaturedLesson.commentCount }}</span>
                    </div>
                  </div>

                  <!-- CTA Button -->
                  <button class="ll-main-cta">
                    <span>Read Full Lesson</span>
                    <i class="fas fa-arrow-right"></i>
                  </button>
                </div>
              </div>

              <!-- Progress Dots -->
              <div class="ll-progress-dots">
                <button
                  v-for="(_, index) in featuredLessons"
                  :key="index"
                  @click="goToFeaturedLesson(index)"
                  :class="['ll-progress-dot', { active: index === currentFeaturedLessonIndex }]"
                ></button>
              </div>
            </div>

            <!-- Next Lessons Sidebar -->
            <div class="ll-next-lessons">
              <div class="ll-next-header">
                <i class="fas fa-layer-group"></i>
                <span>Up Next</span>
              </div>

              <div class="ll-next-list">
                <div
                  v-for="(lesson, idx) in nextFeaturedLessons"
                  :key="lesson.id"
                  class="ll-next-card"
                  @click="goToFeaturedLesson(lesson.displayIndex)"
                >
                  <div class="ll-next-index">{{ idx + 1 }}</div>
                  <div class="ll-next-content">
                    <div class="ll-next-meta">
                      <span class="ll-next-category">
                        <i :class="getLessonCategoryInfo(lesson.category).icon"></i>
                        {{ getLessonCategoryInfo(lesson.category).label }}
                      </span>
                      <span :class="['ll-next-priority', 'priority-' + lesson.priority]">{{ lesson.priority }}</span>
                    </div>
                    <h4 class="ll-next-title">{{ lesson.title }}</h4>
                    <p class="ll-next-summary">{{ lesson.summary }}</p>
                    <div class="ll-next-footer">
                      <span class="ll-next-author">
                        <span class="ll-next-avatar">{{ lesson.author.initials }}</span>
                        {{ lesson.author.name }}
                      </span>
                      <span class="ll-next-stats">
                        <i class="fas fa-eye"></i> {{ lesson.viewCount }}
                      </span>
                    </div>
                  </div>
                  <div class="ll-next-arrow">
                    <i class="fas fa-chevron-right"></i>
                  </div>
                </div>
              </div>

              <!-- View All Link -->
              <button class="ll-view-all-btn">
                <span>View All Featured</span>
                <i class="fas fa-arrow-right"></i>
              </button>
            </div>
          </div>
        </div>

        <!-- All Lessons Section - Matching ArticlesPage Design -->
        <div class="px-6 pb-6 all-lessons-section">
          <!-- Section Header -->
          <div class="flex items-center justify-between mb-4">
            <h2 class="text-lg font-bold text-gray-900 flex items-center gap-3">
              <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-teal-500 to-teal-600 flex items-center justify-center shadow-lg shadow-teal-200">
                <i class="fas fa-lightbulb text-white text-sm"></i>
              </div>
              <div>
                <span class="block">All Lessons</span>
                <span class="text-xs font-medium text-gray-500">{{ filteredLessons.length }} lessons found</span>
              </div>
            </h2>
          </div>

          <!-- Toolbar (ArticlesPage Style) -->
          <div class="bg-white rounded-2xl shadow-sm border border-gray-100 mb-4">
            <div class="px-4 py-3 bg-gray-50/50 rounded-2xl flex flex-wrap items-center gap-3">
              <!-- Search -->
              <div class="relative flex-1 max-w-xl">
                <div class="relative">
                  <i class="fas fa-search absolute left-3 top-1/2 -translate-y-1/2 text-sm text-gray-400"></i>
                  <input
                    v-model="lessonsSearch"
                    type="text"
                    placeholder="Search lessons..."
                    @input="lessonsCurrentPage = 1"
                    class="w-full pl-9 pr-20 py-2 text-sm rounded-lg bg-white border border-gray-200 focus:ring-2 focus:ring-teal-500 focus:border-transparent focus:outline-none transition-all"
                  >
                  <div class="absolute right-2 top-1/2 -translate-y-1/2 flex items-center gap-1">
                    <button
                      v-if="lessonsSearch"
                      @click="lessonsSearch = ''; lessonsCurrentPage = 1"
                      class="p-1 rounded text-gray-400 hover:text-gray-600 transition-colors"
                    >
                      <i class="fas fa-times text-xs"></i>
                    </button>
                  </div>
                </div>
              </div>

              <!-- Category Filter (Multi-select with checkboxes) -->
              <div class="relative">
                <button
                  @click="showLessonsCategoryDropdown = !showLessonsCategoryDropdown"
                  :class="[
                    'flex items-center gap-2 px-3 py-1.5 rounded-lg text-xs font-medium transition-all border',
                    selectedLessonsCategories.length > 0 ? 'bg-teal-50 border-teal-200 text-teal-700' : 'bg-white border-gray-200 text-gray-600 hover:bg-gray-50'
                  ]"
                >
                  <i class="fas fa-layer-group text-sm"></i>
                  <span>{{ selectedLessonsCategories.length > 0 ? `Category (${selectedLessonsCategories.length})` : 'Category' }}</span>
                  <i :class="showLessonsCategoryDropdown ? 'fas fa-chevron-up' : 'fas fa-chevron-down'" class="text-[10px] ml-1"></i>
                </button>

                <div v-if="showLessonsCategoryDropdown" @click="showLessonsCategoryDropdown = false" class="fixed inset-0 z-40"></div>

                <div
                  v-if="showLessonsCategoryDropdown"
                  class="absolute left-0 top-full mt-2 w-64 bg-white rounded-xl shadow-lg border border-gray-100 py-2 z-50"
                >
                  <div class="px-3 py-1.5 text-xs font-semibold text-gray-400 uppercase tracking-wider">Categories</div>
                  <div class="max-h-48 overflow-y-auto">
                    <button
                      v-for="cat in lessonsCategoryOptions.filter(c => c.id !== 'all')"
                      :key="cat.id"
                      @click="toggleLessonsCategoryFilter(cat.id)"
                      :class="[
                        'w-full px-3 py-2 text-left text-sm flex items-center gap-3 transition-colors',
                        isLessonsCategorySelected(cat.id) ? 'bg-teal-50 text-teal-700' : 'text-gray-700 hover:bg-gray-50'
                      ]"
                    >
                      <div :class="[
                        'w-4 h-4 rounded border-2 flex items-center justify-center transition-all',
                        isLessonsCategorySelected(cat.id) ? 'bg-teal-500 border-teal-500' : 'border-gray-300'
                      ]">
                        <i v-if="isLessonsCategorySelected(cat.id)" class="fas fa-check text-white text-[8px]"></i>
                      </div>
                      <i :class="[cat.icon, 'text-teal-500 text-sm']"></i>
                      <span class="flex-1">{{ cat.label }}</span>
                      <span class="text-xs text-gray-400">{{ categoryStats[cat.id] || 0 }}</span>
                    </button>
                  </div>

                  <div class="my-2 border-t border-gray-100"></div>

                  <div class="px-3 flex gap-2">
                    <button
                      @click="selectedLessonsCategories = []; lessonsCurrentPage = 1; showLessonsCategoryDropdown = false"
                      class="flex-1 px-3 py-1.5 text-xs font-medium text-gray-600 bg-gray-100 rounded-lg hover:bg-gray-200 transition-colors"
                    >
                      Clear
                    </button>
                    <button
                      @click="showLessonsCategoryDropdown = false"
                      class="flex-1 px-3 py-1.5 text-xs font-medium text-white bg-teal-500 rounded-lg hover:bg-teal-600 transition-colors"
                    >
                      Apply
                    </button>
                  </div>
                </div>
              </div>

              <!-- Priority Filter (Multi-select with checkboxes) -->
              <div class="relative">
                <button
                  @click="showLessonsPriorityDropdown = !showLessonsPriorityDropdown"
                  :class="[
                    'flex items-center gap-2 px-3 py-1.5 rounded-lg text-xs font-medium transition-all border',
                    selectedLessonsPriorities.length > 0 ? 'bg-teal-50 border-teal-200 text-teal-700' : 'bg-white border-gray-200 text-gray-600 hover:bg-gray-50'
                  ]"
                >
                  <i class="fas fa-flag text-sm"></i>
                  <span>{{ selectedLessonsPriorities.length > 0 ? `Priority (${selectedLessonsPriorities.length})` : 'Priority' }}</span>
                  <i :class="showLessonsPriorityDropdown ? 'fas fa-chevron-up' : 'fas fa-chevron-down'" class="text-[10px] ml-1"></i>
                </button>

                <div v-if="showLessonsPriorityDropdown" @click="showLessonsPriorityDropdown = false" class="fixed inset-0 z-40"></div>

                <div
                  v-if="showLessonsPriorityDropdown"
                  class="absolute left-0 top-full mt-2 w-56 bg-white rounded-xl shadow-lg border border-gray-100 py-2 z-50"
                >
                  <div class="px-3 py-1.5 text-xs font-semibold text-gray-400 uppercase tracking-wider">Priorities</div>
                  <div class="max-h-48 overflow-y-auto">
                    <button
                      v-for="pri in lessonsPriorityOptions.filter(p => p.id !== 'all')"
                      :key="pri.id"
                      @click="toggleLessonsPriorityFilter(pri.id)"
                      :class="[
                        'w-full px-3 py-2 text-left text-sm flex items-center gap-3 transition-colors',
                        isLessonsPrioritySelected(pri.id) ? 'bg-teal-50 text-teal-700' : 'text-gray-700 hover:bg-gray-50'
                      ]"
                    >
                      <div :class="[
                        'w-4 h-4 rounded border-2 flex items-center justify-center transition-all',
                        isLessonsPrioritySelected(pri.id) ? 'bg-teal-500 border-teal-500' : 'border-gray-300'
                      ]">
                        <i v-if="isLessonsPrioritySelected(pri.id)" class="fas fa-check text-white text-[8px]"></i>
                      </div>
                      <span :class="['w-2.5 h-2.5 rounded-full', pri.id === 'critical' ? 'bg-red-500' : pri.id === 'high' ? 'bg-orange-500' : pri.id === 'medium' ? 'bg-amber-500' : 'bg-green-500']"></span>
                      <span class="flex-1 capitalize">{{ pri.label }}</span>
                    </button>
                  </div>

                  <div class="my-2 border-t border-gray-100"></div>

                  <div class="px-3 flex gap-2">
                    <button
                      @click="selectedLessonsPriorities = []; lessonsCurrentPage = 1; showLessonsPriorityDropdown = false"
                      class="flex-1 px-3 py-1.5 text-xs font-medium text-gray-600 bg-gray-100 rounded-lg hover:bg-gray-200 transition-colors"
                    >
                      Clear
                    </button>
                    <button
                      @click="showLessonsPriorityDropdown = false"
                      class="flex-1 px-3 py-1.5 text-xs font-medium text-white bg-teal-500 rounded-lg hover:bg-teal-600 transition-colors"
                    >
                      Apply
                    </button>
                  </div>
                </div>
              </div>

              <!-- Tags Filter (Multi-select) -->
              <div class="relative">
                <button
                  @click="showLessonsTagsDropdown = !showLessonsTagsDropdown"
                  :class="[
                    'flex items-center gap-2 px-3 py-1.5 rounded-lg text-xs font-medium transition-all border',
                    selectedLessonsTags.length > 0 ? 'bg-teal-50 border-teal-200 text-teal-700' : 'bg-white border-gray-200 text-gray-600 hover:bg-gray-50'
                  ]"
                >
                  <i class="fas fa-tags text-sm"></i>
                  <span>{{ selectedLessonsTags.length > 0 ? `Tags (${selectedLessonsTags.length})` : 'Tags' }}</span>
                  <i :class="showLessonsTagsDropdown ? 'fas fa-chevron-up' : 'fas fa-chevron-down'" class="text-[10px] ml-1"></i>
                </button>

                <div v-if="showLessonsTagsDropdown" @click="showLessonsTagsDropdown = false" class="fixed inset-0 z-40"></div>

                <div
                  v-if="showLessonsTagsDropdown"
                  class="absolute left-0 top-full mt-2 w-56 bg-white rounded-xl shadow-lg border border-gray-100 py-2 z-50"
                >
                  <div class="px-3 py-1.5 text-xs font-semibold text-gray-400 uppercase tracking-wider">Tags</div>
                  <div class="max-h-48 overflow-y-auto">
                    <button
                      v-for="tag in allLessonsTags"
                      :key="tag"
                      @click="toggleLessonsTagFilter(tag)"
                      :class="[
                        'w-full px-3 py-2 text-left text-sm flex items-center gap-3 transition-colors',
                        isLessonsTagSelected(tag) ? 'bg-teal-50 text-teal-700' : 'text-gray-700 hover:bg-gray-50'
                      ]"
                    >
                      <div :class="[
                        'w-4 h-4 rounded border-2 flex items-center justify-center transition-all',
                        isLessonsTagSelected(tag) ? 'bg-teal-500 border-teal-500' : 'border-gray-300'
                      ]">
                        <i v-if="isLessonsTagSelected(tag)" class="fas fa-check text-white text-[8px]"></i>
                      </div>
                      <i class="fas fa-tag text-teal-500 text-sm"></i>
                      <span class="flex-1">{{ tag }}</span>
                    </button>
                  </div>

                  <div class="my-2 border-t border-gray-100"></div>

                  <div class="px-3 flex gap-2">
                    <button
                      @click="selectedLessonsTags = []; lessonsCurrentPage = 1; showLessonsTagsDropdown = false"
                      class="flex-1 px-3 py-1.5 text-xs font-medium text-gray-600 bg-gray-100 rounded-lg hover:bg-gray-200 transition-colors"
                    >
                      Clear
                    </button>
                    <button
                      @click="showLessonsTagsDropdown = false"
                      class="flex-1 px-3 py-1.5 text-xs font-medium text-white bg-teal-500 rounded-lg hover:bg-teal-600 transition-colors"
                    >
                      Apply
                    </button>
                  </div>
                </div>
              </div>

              <!-- Bookmarked Filter -->
              <button
                @click="showBookmarkedOnly = !showBookmarkedOnly; lessonsCurrentPage = 1"
                :class="[
                  'flex items-center gap-2 px-3 py-1.5 rounded-lg text-xs font-medium transition-all border',
                  showBookmarkedOnly ? 'bg-teal-50 border-teal-200 text-teal-700' : 'bg-white border-gray-200 text-gray-600 hover:bg-gray-50'
                ]"
              >
                <i :class="showBookmarkedOnly ? 'fas fa-bookmark' : 'far fa-bookmark'" class="text-sm"></i>
                <span>Bookmarked</span>
              </button>

              <!-- Sort Options with Order Toggle -->
              <div class="relative ml-auto flex items-center">
                <button
                  @click="showLessonsSortDropdown = !showLessonsSortDropdown"
                  class="flex items-center gap-2 px-3 py-1.5 rounded-l-lg text-xs font-medium transition-all border border-r-0 bg-white border-gray-200 text-gray-600 hover:bg-gray-50"
                >
                  <i :class="[lessonsSortOptions.find(o => o.value === lessonsSortBy)?.icon || 'fas fa-sort', 'text-sm text-teal-500']"></i>
                  <span>{{ lessonsSortOptions.find(o => o.value === lessonsSortBy)?.label || 'Sort' }}</span>
                  <i :class="showLessonsSortDropdown ? 'fas fa-chevron-up' : 'fas fa-chevron-down'" class="text-[10px] ml-1"></i>
                </button>
                <button
                  @click="lessonsSortOrder = lessonsSortOrder === 'asc' ? 'desc' : 'asc'"
                  class="flex items-center justify-center w-8 h-8 rounded-r-lg text-xs font-medium transition-all border bg-white border-gray-200 text-gray-600 hover:bg-gray-50 hover:text-teal-600"
                  :title="lessonsSortOrder === 'asc' ? 'Ascending order - Click for descending' : 'Descending order - Click for ascending'"
                >
                  <i :class="lessonsSortOrder === 'asc' ? 'fas fa-arrow-up' : 'fas fa-arrow-down'" class="text-sm text-teal-500"></i>
                </button>

                <div v-if="showLessonsSortDropdown" @click="showLessonsSortDropdown = false" class="fixed inset-0 z-40"></div>

                <div
                  v-if="showLessonsSortDropdown"
                  class="absolute left-0 top-full mt-2 w-48 bg-white rounded-xl shadow-lg border border-gray-100 py-2 z-50"
                >
                  <div class="px-3 py-1.5 text-xs font-semibold text-gray-400 uppercase tracking-wider">Sort By</div>
                  <div class="max-h-64 overflow-y-auto">
                    <button
                      v-for="option in lessonsSortOptions"
                      :key="option.value"
                      @click="lessonsSortBy = option.value; showLessonsSortDropdown = false"
                      :class="[
                        'w-full px-3 py-2 text-left text-sm flex items-center gap-3 transition-colors',
                        lessonsSortBy === option.value ? 'bg-teal-50 text-teal-700' : 'text-gray-700 hover:bg-gray-50'
                      ]"
                    >
                      <i :class="[option.icon, 'text-sm w-4', lessonsSortBy === option.value ? 'text-teal-500' : 'text-gray-400']"></i>
                      <span class="flex-1">{{ option.label }}</span>
                      <i v-if="lessonsSortBy === option.value" class="fas fa-check text-teal-500 text-xs"></i>
                    </button>
                  </div>
                </div>
              </div>

              <!-- View Toggle -->
              <div class="flex items-center gap-0.5 bg-white border border-gray-200 rounded-lg p-1">
                <button
                  @click="lessonsViewMode = 'grid'"
                  :class="['px-2.5 py-1 rounded-md transition-all', lessonsViewMode === 'grid' ? 'bg-teal-500 text-white' : 'text-gray-500 hover:bg-gray-100']"
                  title="Grid view"
                >
                  <i class="fas fa-th-large text-xs"></i>
                </button>
                <button
                  @click="lessonsViewMode = 'list'"
                  :class="['px-2.5 py-1 rounded-md transition-all', lessonsViewMode === 'list' ? 'bg-teal-500 text-white' : 'text-gray-500 hover:bg-gray-100']"
                  title="List view"
                >
                  <i class="fas fa-list text-xs"></i>
                </button>
              </div>
            </div>
          </div>

          <!-- Active Filters Bar -->
          <div v-if="activeLessonsFiltersCount > 0" class="flex items-center gap-3 mb-4 p-3 bg-white rounded-xl border border-gray-100 shadow-sm">
            <div class="flex items-center gap-2 px-2 py-1 bg-gray-100 rounded-lg">
              <i class="fas fa-filter text-gray-400 text-xs"></i>
              <span class="text-xs font-medium text-gray-600">Active Filters</span>
            </div>
            <div class="flex flex-wrap gap-2 flex-1">
              <!-- Search Filter -->
              <span v-if="lessonsSearch" class="px-2.5 py-1 bg-gray-100 text-gray-700 rounded-lg text-xs font-medium flex items-center gap-1.5 border border-gray-200">
                <i class="fas fa-search text-[10px]"></i>
                "{{ lessonsSearch }}"
                <button @click="lessonsSearch = ''; lessonsCurrentPage = 1" class="ml-1 hover:text-gray-900 hover:bg-gray-200 rounded-full w-4 h-4 flex items-center justify-center"><i class="fas fa-times text-[10px]"></i></button>
              </span>
              <!-- Category Filters (multiple) -->
              <span
                v-for="catId in selectedLessonsCategories"
                :key="'cat-' + catId"
                class="px-2.5 py-1 bg-teal-50 text-teal-700 rounded-lg text-xs font-medium flex items-center gap-1.5 border border-teal-100"
              >
                <i class="fas fa-layer-group text-[10px]"></i>
                {{ getLessonCategoryInfo(catId as LessonCategory).label }}
                <button @click="toggleLessonsCategoryFilter(catId)" class="ml-1 hover:text-teal-900 hover:bg-teal-100 rounded-full w-4 h-4 flex items-center justify-center"><i class="fas fa-times text-[10px]"></i></button>
              </span>
              <!-- Priority Filters (multiple) -->
              <span
                v-for="priId in selectedLessonsPriorities"
                :key="'pri-' + priId"
                class="px-2.5 py-1 bg-teal-50 text-teal-700 rounded-lg text-xs font-medium flex items-center gap-1.5 border border-teal-100"
              >
                <i class="fas fa-flag text-[10px]"></i>
                {{ priId }}
                <button @click="toggleLessonsPriorityFilter(priId)" class="ml-1 hover:text-teal-900 hover:bg-teal-100 rounded-full w-4 h-4 flex items-center justify-center"><i class="fas fa-times text-[10px]"></i></button>
              </span>
              <!-- Tag Filters (multiple) -->
              <span
                v-for="tag in selectedLessonsTags"
                :key="'tag-' + tag"
                class="px-2.5 py-1 bg-teal-50 text-teal-700 rounded-lg text-xs font-medium flex items-center gap-1.5 border border-teal-100"
              >
                <i class="fas fa-tag text-[10px]"></i>
                {{ tag }}
                <button @click="toggleLessonsTagFilter(tag)" class="ml-1 hover:text-teal-900 hover:bg-teal-100 rounded-full w-4 h-4 flex items-center justify-center"><i class="fas fa-times text-[10px]"></i></button>
              </span>
              <!-- Bookmarked Filter -->
              <span
                v-if="showBookmarkedOnly"
                class="px-2.5 py-1 bg-amber-50 text-amber-700 rounded-lg text-xs font-medium flex items-center gap-1.5 border border-amber-100"
              >
                <i class="fas fa-bookmark text-[10px]"></i>
                Bookmarked
                <button @click="showBookmarkedOnly = false; lessonsCurrentPage = 1" class="ml-1 hover:text-amber-900 hover:bg-amber-100 rounded-full w-4 h-4 flex items-center justify-center"><i class="fas fa-times text-[10px]"></i></button>
              </span>
            </div>
            <button @click="clearLessonsFilters" class="px-3 py-1.5 text-xs font-medium text-red-600 hover:text-red-700 hover:bg-red-50 rounded-lg transition-colors flex items-center gap-1.5">
              <i class="fas fa-times-circle"></i>
              Clear all
            </button>
          </div>

          <!-- Content Area -->
          <div class="bg-white rounded-2xl border border-gray-100 shadow-sm p-5">
            <div v-if="paginatedLessons.length > 0">
              <!-- Grid View -->
              <div v-if="lessonsViewMode === 'grid'" class="grid grid-cols-[repeat(auto-fill,minmax(320px,1fr))] gap-4">
                <article
                  v-for="lesson in paginatedLessons"
                  :key="lesson.id"
                  @click="openLessonDetail(lesson)"
                  :class="[
                    'group bg-white rounded-xl p-4 cursor-pointer transition-all duration-300 hover:-translate-y-1 border',
                    lesson.isFeatured
                      ? 'border-teal-200 shadow-md hover:shadow-lg hover:border-teal-300 bg-gradient-to-br from-teal-50/30 to-white'
                      : 'border-gray-200 shadow-sm hover:shadow-md hover:border-teal-200'
                  ]"
                >
                  <!-- Card Header -->
                  <div class="flex items-start justify-between gap-3 mb-3">
                    <!-- Category Icon -->
                    <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-teal-500 to-cyan-500 flex items-center justify-center flex-shrink-0 shadow-sm">
                      <i :class="[getLessonCategoryInfo(lesson.category).icon, 'text-white text-sm']"></i>
                    </div>

                    <!-- Badges & Actions -->
                    <div class="flex items-center gap-2">
                      <!-- Featured Badge -->
                      <StatusBadge v-if="lesson.isFeatured" status="featured" size="xs" variant="gradient" />
                      <!-- Priority Badge -->
                      <CategoryBadge :category="lesson.priority" :category-id="lesson.priority" size="xs" />
                      <!-- Bookmark Button -->
                      <button
                        @click.stop="toggleLessonBookmark(lesson.id)"
                        :class="[
                          'w-7 h-7 rounded-lg flex items-center justify-center transition-all',
                          lesson.isBookmarked ? 'bg-teal-100 text-teal-600' : 'bg-gray-100 text-gray-400 hover:bg-teal-50 hover:text-teal-500'
                        ]"
                      >
                        <i :class="lesson.isBookmarked ? 'fas fa-bookmark' : 'far fa-bookmark'" class="text-xs"></i>
                      </button>
                    </div>
                  </div>

                  <!-- Category & Date -->
                  <div class="flex items-center gap-3 text-[11px] text-gray-400 mb-2">
                    <span class="flex items-center gap-1 text-teal-600 font-medium">
                      <i :class="[getLessonCategoryInfo(lesson.category).icon, 'text-[9px]']"></i>
                      {{ getLessonCategoryInfo(lesson.category).label }}
                    </span>
                    <span class="flex items-center gap-1">
                      <i class="fas fa-calendar text-[9px]"></i>
                      {{ lesson.createdAt }}
                    </span>
                  </div>

                  <!-- Title -->
                  <h3 class="text-sm font-semibold text-gray-800 mb-2 line-clamp-2 group-hover:text-teal-600 transition-colors leading-snug">
                    {{ lesson.title }}
                  </h3>

                  <!-- Summary -->
                  <p class="text-xs text-gray-500 mb-3 line-clamp-2 leading-relaxed">
                    {{ lesson.summary }}
                  </p>

                  <!-- Tags -->
                  <div class="flex flex-wrap gap-1.5 mb-3">
                    <TagBadge
                      v-for="tag in lesson.tags?.slice(0, 3)"
                      :key="tag"
                      :tag="tag"
                      size="xs"
                    />
                    <span v-if="lesson.tags?.length > 3" class="px-2 py-0.5 bg-teal-50 text-teal-600 text-[10px] font-medium rounded-full">
                      +{{ lesson.tags.length - 3 }}
                    </span>
                  </div>

                  <!-- Footer -->
                  <div class="flex items-center justify-between pt-3 border-t border-gray-100">
                    <!-- Author -->
                    <div class="flex items-center gap-2">
                      <div class="w-6 h-6 rounded-full bg-gradient-to-br from-teal-400 to-cyan-500 flex items-center justify-center text-white text-[9px] font-semibold">
                        {{ lesson.author?.initials || 'U' }}
                      </div>
                      <span class="text-xs text-gray-600 font-medium">{{ lesson.author?.name }}</span>
                    </div>

                    <!-- Stats -->
                    <div class="flex items-center gap-3 text-[11px] text-gray-400">
                      <span class="flex items-center gap-1">
                        <i class="fas fa-eye text-[9px]"></i>
                        {{ lesson.viewCount }}
                      </span>
                      <span class="flex items-center gap-1">
                        <i class="fas fa-heart text-[9px]"></i>
                        {{ lesson.likeCount || 0 }}
                      </span>
                      <span class="flex items-center gap-1">
                        <i class="fas fa-comment text-[9px]"></i>
                        {{ lesson.commentCount || 0 }}
                      </span>
                    </div>
                  </div>
                </article>
              </div>

              <!-- List View -->
              <div v-else class="space-y-3">
                <article
                  v-for="lesson in paginatedLessons"
                  :key="lesson.id"
                  @click="openLessonDetail(lesson)"
                  :class="[
                    'group flex gap-4 p-4 bg-white rounded-xl cursor-pointer transition-all duration-300 border',
                    lesson.isFeatured
                      ? 'border-teal-200 shadow-md hover:shadow-lg hover:border-teal-300 bg-gradient-to-r from-teal-50/30 to-white'
                      : 'border-gray-200 shadow-sm hover:shadow-md hover:border-teal-200'
                  ]"
                >
                  <!-- Category Icon -->
                  <div class="w-12 h-12 rounded-xl bg-gradient-to-br from-teal-500 to-cyan-500 flex items-center justify-center flex-shrink-0 shadow-sm">
                    <i :class="[getLessonCategoryInfo(lesson.category).icon, 'text-white text-lg']"></i>
                  </div>

                  <!-- Content -->
                  <div class="flex-1 min-w-0">
                    <!-- Header Badges -->
                    <div class="flex flex-wrap items-center gap-2 mb-1.5">
                      <CategoryBadge :category="getLessonCategoryInfo(lesson.category).label" :icon="getLessonCategoryInfo(lesson.category).icon" size="xs" />
                      <StatusBadge v-if="lesson.isFeatured" status="featured" size="xs" variant="gradient" />
                      <CategoryBadge :category="lesson.priority" :category-id="lesson.priority" size="xs" />
                      <span class="text-[10px] text-gray-400 flex items-center gap-1">
                        <i class="fas fa-calendar text-[8px]"></i>
                        {{ lesson.createdAt }}
                      </span>
                    </div>

                    <!-- Title -->
                    <h3 class="text-sm font-semibold text-gray-800 mb-1 truncate group-hover:text-teal-600 transition-colors">
                      {{ lesson.title }}
                    </h3>

                    <!-- Excerpt -->
                    <p class="text-xs text-gray-500 mb-2 line-clamp-1">
                      {{ lesson.summary }}
                    </p>

                    <!-- Footer -->
                    <div class="flex items-center justify-between">
                      <!-- Author & Date -->
                      <div class="flex items-center gap-2">
                        <div class="w-6 h-6 rounded-full bg-gradient-to-br from-teal-400 to-cyan-500 flex items-center justify-center text-white text-[9px] font-semibold">
                          {{ lesson.author?.initials || 'U' }}
                        </div>
                        <span class="text-[11px] text-gray-500">
                          {{ lesson.author?.name }}
                        </span>
                      </div>

                      <!-- Stats & Tags -->
                      <div class="flex items-center gap-3">
                        <div class="hidden sm:flex items-center gap-1.5">
                          <span
                            v-for="tag in lesson.tags?.slice(0, 2)"
                            :key="tag"
                            class="px-1.5 py-0.5 bg-gray-100 text-gray-500 text-[10px] rounded-full"
                          >
                            {{ tag }}
                          </span>
                        </div>
                        <div class="flex items-center gap-2 text-[11px] text-gray-400">
                          <span class="flex items-center gap-1">
                            <i class="fas fa-eye text-[9px]"></i>
                            {{ lesson.viewCount }}
                          </span>
                          <span class="flex items-center gap-1">
                            <i class="fas fa-heart text-[9px]"></i>
                            {{ lesson.likeCount || 0 }}
                          </span>
                        </div>
                      </div>
                    </div>
                  </div>

                  <!-- Actions -->
                  <div class="flex flex-col gap-2">
                    <button
                      @click.stop="toggleLessonBookmark(lesson.id)"
                      :class="[
                        'w-8 h-8 rounded-lg flex items-center justify-center transition-colors',
                        lesson.isBookmarked ? 'bg-teal-50 text-teal-600' : 'bg-gray-50 text-gray-400 hover:bg-teal-50 hover:text-teal-600'
                      ]"
                      title="Bookmark"
                    >
                      <i :class="lesson.isBookmarked ? 'fas fa-bookmark' : 'far fa-bookmark'" class="text-xs"></i>
                    </button>
                  </div>
                </article>
              </div>

              <!-- Pagination Footer -->
              <Pagination
                v-model:current-page="lessonsCurrentPage"
                v-model:items-per-page="lessonsItemsPerPage"
                :total-items="filteredLessons.length"
                :items-per-page-options="lessonsItemsPerPageOptions"
                class="mt-4"
              />
            </div>

            <!-- Empty State -->
            <EmptyState
              v-else
              icon="fas fa-search"
              title="No lessons found"
              description="Try adjusting your search or filters"
              action-label="Clear Filters"
              action-icon="fas fa-rotate"
              size="lg"
              @action="clearLessonsFilters"
            />
          </div>
        </div>

        <!-- Lesson Detail Modal -->
        <Teleport to="body">
          <div v-if="showLessonDetailModal && selectedLessonForModal" class="ll-modal-overlay" @click.self="closeLessonDetail">
            <div class="ll-modal">
              <div class="ll-modal-header" :class="'cat-' + selectedLessonForModal.category">
                <div class="ll-modal-badges">
                  <span :class="['ll-modal-priority', 'priority-' + selectedLessonForModal.priority]">
                    <i class="fas fa-flag"></i> {{ selectedLessonForModal.priority }} Priority
                  </span>
                  <span class="ll-modal-category">
                    <i :class="getLessonCategoryInfo(selectedLessonForModal.category).icon"></i>
                    {{ getLessonCategoryInfo(selectedLessonForModal.category).label }}
                  </span>
                </div>
                <button @click="closeLessonDetail" class="ll-modal-close">
                  <i class="fas fa-times"></i>
                </button>
              </div>

              <div class="ll-modal-body">
                <h2 class="ll-modal-title">{{ selectedLessonForModal.title }}</h2>

                <div class="ll-modal-meta">
                  <div class="ll-modal-author">
                    <div class="ll-modal-avatar">{{ selectedLessonForModal.author.initials }}</div>
                    <div>
                      <span class="ll-modal-author-name">{{ selectedLessonForModal.author.name }}</span>
                      <span class="ll-modal-author-dept">{{ selectedLessonForModal.author.department }}</span>
                    </div>
                  </div>
                  <div class="ll-modal-info">
                    <span><i class="fas fa-calendar"></i> {{ selectedLessonForModal.createdAt }}</span>
                    <span v-if="selectedLessonForModal.projectName"><i class="fas fa-folder"></i> {{ selectedLessonForModal.projectName }}</span>
                  </div>
                </div>

                <div class="ll-modal-stats">
                  <span><i class="fas fa-eye"></i> {{ selectedLessonForModal.viewCount }} views</span>
                  <span><i class="fas fa-heart"></i> {{ selectedLessonForModal.likeCount }} likes</span>
                  <span><i class="fas fa-comment"></i> {{ selectedLessonForModal.commentCount }} comments</span>
                </div>

                <div class="ll-modal-section">
                  <h4><i class="fas fa-align-left"></i> Summary</h4>
                  <p>{{ selectedLessonForModal.summary }}</p>
                </div>

                <div class="ll-modal-section">
                  <h4><i class="fas fa-book-open"></i> Full Lesson</h4>
                  <p>{{ selectedLessonForModal.content }}</p>
                </div>

                <div v-if="selectedLessonForModal.impact" class="ll-modal-impact">
                  <h4><i class="fas fa-chart-line"></i> Impact</h4>
                  <div class="ll-impact-highlight">{{ selectedLessonForModal.impact }}</div>
                </div>

                <div v-if="selectedLessonForModal.recommendations?.length" class="ll-modal-section">
                  <h4><i class="fas fa-lightbulb"></i> Key Recommendations</h4>
                  <ul class="ll-recommendations">
                    <li v-for="(rec, idx) in selectedLessonForModal.recommendations" :key="idx">
                      <i class="fas fa-check-circle"></i>
                      <span>{{ rec }}</span>
                    </li>
                  </ul>
                </div>

                <div class="ll-modal-tags">
                  <span v-for="tag in selectedLessonForModal.tags" :key="tag" class="ll-modal-tag">{{ tag }}</span>
                </div>
              </div>

              <div class="ll-modal-footer">
                <div class="ll-modal-actions">
                  <button
                    @click="toggleLessonLike(selectedLessonForModal.id)"
                    class="ll-action-btn"
                    :class="{ 'active': selectedLessonForModal.isLiked }"
                  >
                    <i :class="selectedLessonForModal.isLiked ? 'fas fa-heart' : 'far fa-heart'"></i>
                    {{ selectedLessonForModal.isLiked ? 'Liked' : 'Like' }}
                  </button>
                  <button
                    @click="toggleLessonBookmark(selectedLessonForModal.id)"
                    class="ll-action-btn"
                    :class="{ 'active': selectedLessonForModal.isBookmarked }"
                  >
                    <i :class="selectedLessonForModal.isBookmarked ? 'fas fa-bookmark' : 'far fa-bookmark'"></i>
                    {{ selectedLessonForModal.isBookmarked ? 'Bookmarked' : 'Bookmark' }}
                  </button>
                  <button class="ll-action-btn">
                    <i class="fas fa-share-alt"></i> Share
                  </button>
                </div>
                <button @click="closeLessonDetail" class="ll-close-btn">Close</button>
              </div>
            </div>
          </div>
        </Teleport>
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
            <Pagination
              v-if="filteredCertificates.length > 0"
              v-model:current-page="certCurrentPage"
              v-model:items-per-page="certItemsPerPage"
              :total-items="filteredCertificates.length"
              :items-per-page-options="certItemsPerPageOptions"
              class="mt-4"
            />
          </div>
        </div>
      </div>
    </div>

    <!-- AI Personalized Recommendations Section -->
    <div v-if="aiPersonalizedRecommendations.length > 0 && currentView === 'my-courses'" class="ai-recommendations-section mx-8 mb-6">
      <div class="ai-section-header">
        <div class="flex items-center gap-3">
          <div class="ai-section-icon">
            <i class="fas fa-wand-magic-sparkles"></i>
          </div>
          <div>
            <h2 class="text-xl font-bold text-gray-900">AI-Powered Recommendations</h2>
            <p class="text-sm text-gray-500">Personalized courses based on your learning journey</p>
          </div>
        </div>
        <button
          @click="generateAIRecommendations"
          :disabled="isGeneratingRecommendations"
          class="px-4 py-2 bg-gradient-to-r from-teal-500 to-cyan-500 text-white rounded-lg text-sm font-medium flex items-center gap-2 hover:from-teal-600 hover:to-cyan-600 transition-all disabled:opacity-50"
        >
          <i :class="isGeneratingRecommendations ? 'fas fa-spinner fa-spin' : 'fas fa-sync-alt'"></i>
          {{ isGeneratingRecommendations ? 'Refreshing...' : 'Refresh' }}
        </button>
      </div>

      <div v-if="isGeneratingRecommendations" class="ai-loading-state">
        <AILoadingIndicator />
        <p class="text-gray-500 mt-2">Analyzing your learning patterns...</p>
      </div>

      <div v-else class="ai-recommendations-grid">
        <div
          v-for="rec in aiPersonalizedRecommendations"
          :key="rec.courseId"
          class="ai-recommendation-card"
          @click="navigateToCourse(rec.courseId)"
        >
          <div class="ai-rec-header">
            <div class="ai-match-score">
              <AIConfidenceBar :value="rec.matchScore / 100" size="sm" />
              <span class="text-sm font-bold text-teal-600">{{ rec.matchScore }}% Match</span>
            </div>
            <span :class="['ai-priority-badge', rec.priority === 'high' ? 'priority-high' : 'priority-medium']">
              <i :class="rec.priority === 'high' ? 'fas fa-fire' : 'fas fa-star'"></i>
              {{ rec.priority === 'high' ? 'High Priority' : 'Recommended' }}
            </span>
          </div>

          <h3 class="ai-rec-title">{{ rec.title }}</h3>

          <div class="ai-rec-reason">
            <i class="fas fa-lightbulb text-amber-500"></i>
            <p>{{ rec.reason }}</p>
          </div>

          <div class="ai-rec-skills">
            <span class="text-xs font-medium text-gray-500 mb-1.5 block">Skills you'll gain:</span>
            <div class="flex flex-wrap gap-1.5">
              <AISuggestionChip
                v-for="skill in rec.skillsGained"
                :key="skill"
                :label="skill"
                size="sm"
              />
            </div>
          </div>

          <div class="ai-rec-footer">
            <span class="text-sm text-gray-500">
              <i class="fas fa-clock mr-1"></i>
              {{ rec.estimatedTime }}
            </span>
            <button class="ai-rec-enroll-btn">
              <i class="fas fa-plus"></i>
              Enroll Now
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Skill Gap Analysis Modal -->
    <div v-if="showSkillGapModal" class="modal-overlay" @click.self="showSkillGapModal = false">
      <div class="skill-gap-modal">
        <div class="modal-header bg-gradient-to-r from-purple-500 to-indigo-500">
          <div class="flex items-center gap-3">
            <div class="w-12 h-12 bg-white/20 rounded-xl flex items-center justify-center">
              <i class="fas fa-wand-magic-sparkles text-white text-xl"></i>
            </div>
            <div>
              <h2 class="text-xl font-bold text-white">AI Skill Gap Analysis</h2>
              <p class="text-purple-100 text-sm">Personalized assessment of your skill development</p>
            </div>
          </div>
          <button @click="showSkillGapModal = false" class="text-white/70 hover:text-white">
            <i class="fas fa-times text-xl"></i>
          </button>
        </div>

        <div v-if="skillGapAnalysis" class="modal-body">
          <!-- Overall Readiness -->
          <div class="skill-overview">
            <div class="readiness-score">
              <div class="readiness-circle">
                <svg viewBox="0 0 100 100">
                  <circle cx="50" cy="50" r="45" fill="none" stroke="#e5e7eb" stroke-width="8" />
                  <circle
                    cx="50" cy="50" r="45" fill="none" stroke="url(#gradient)" stroke-width="8"
                    :stroke-dasharray="skillGapAnalysis.overallReadiness * 2.83 + ', 283'"
                    stroke-linecap="round"
                    transform="rotate(-90 50 50)"
                  />
                  <defs>
                    <linearGradient id="gradient" x1="0%" y1="0%" x2="100%">
                      <stop offset="0%" stop-color="#8b5cf6" />
                      <stop offset="100%" stop-color="#6366f1" />
                    </linearGradient>
                  </defs>
                </svg>
                <div class="readiness-value">
                  <span class="text-3xl font-bold text-gray-900">{{ skillGapAnalysis.overallReadiness }}%</span>
                  <span class="text-xs text-gray-500">Career Ready</span>
                </div>
              </div>
              <p class="text-sm text-gray-600 mt-3 text-center">Based on industry standards and your learning goals</p>
            </div>

            <div class="strength-weakness">
              <div class="sw-section strengths">
                <h4><i class="fas fa-check-circle text-green-500"></i> Your Strengths</h4>
                <div class="sw-chips">
                  <span v-for="s in skillGapAnalysis.strengths" :key="s" class="sw-chip strength">{{ s }}</span>
                </div>
              </div>
              <div class="sw-section weaknesses">
                <h4><i class="fas fa-exclamation-circle text-amber-500"></i> Areas to Improve</h4>
                <div class="sw-chips">
                  <span v-for="w in skillGapAnalysis.weaknesses" :key="w" class="sw-chip weakness">{{ w }}</span>
                </div>
              </div>
            </div>
          </div>

          <!-- Skill Gaps List -->
          <div class="skill-gaps-list">
            <h3 class="text-lg font-bold text-gray-900 mb-4">Skill Gap Details</h3>
            <div class="skill-gap-items">
              <div v-for="gap in skillGapAnalysis.gaps" :key="gap.skill" class="skill-gap-item">
                <div class="gap-header">
                  <div class="flex items-center gap-2">
                    <span class="font-semibold text-gray-800">{{ gap.skill }}</span>
                    <span :class="['priority-tag', getPriorityColor(gap.priority)]">{{ gap.priority }}</span>
                  </div>
                  <span class="text-xs text-gray-500">{{ gap.category }}</span>
                </div>
                <div class="gap-bars">
                  <div class="gap-bar-container">
                    <div class="gap-bar-label">
                      <span>Current</span>
                      <span class="font-medium">{{ gap.currentLevel }}%</span>
                    </div>
                    <div class="gap-bar">
                      <div class="gap-bar-fill current" :style="{ width: gap.currentLevel + '%' }"></div>
                    </div>
                  </div>
                  <div class="gap-bar-container">
                    <div class="gap-bar-label">
                      <span>Target</span>
                      <span class="font-medium">{{ gap.targetLevel }}%</span>
                    </div>
                    <div class="gap-bar">
                      <div class="gap-bar-fill target" :style="{ width: gap.targetLevel + '%' }"></div>
                    </div>
                  </div>
                </div>
                <div class="gap-delta" :class="getSkillGapColor(gap.gap)">
                  <i class="fas fa-arrow-up"></i>
                  {{ gap.gap }}% gap to close
                </div>
              </div>
            </div>
          </div>

          <!-- Recommended Focus -->
          <div class="recommended-focus">
            <h4><i class="fas fa-bullseye text-teal-500"></i> Recommended Focus Areas</h4>
            <div class="focus-items">
              <span v-for="focus in skillGapAnalysis.recommendedFocus" :key="focus" class="focus-item">
                {{ focus }}
              </span>
            </div>
            <p class="time-estimate">
              <i class="fas fa-clock"></i>
              Estimated time to close gaps: <strong>{{ skillGapAnalysis.estimatedTimeToClose }}</strong>
            </p>
          </div>
        </div>

        <div class="modal-footer">
          <button @click="showSkillGapModal = false" class="btn-secondary">Close</button>
          <button @click="generateAILearningPaths(); showSkillGapModal = false" class="btn-primary">
            <i class="fas fa-route"></i>
            Get AI Learning Path
          </button>
        </div>
      </div>
    </div>

    <!-- AI Learning Path Modal -->
    <div v-if="showAILearningPathModal" class="modal-overlay" @click.self="showAILearningPathModal = false">
      <div class="ai-path-modal">
        <div class="modal-header bg-gradient-to-r from-teal-500 to-cyan-500">
          <div class="flex items-center gap-3">
            <div class="w-12 h-12 bg-white/20 rounded-xl flex items-center justify-center">
              <i class="fas fa-route text-white text-xl"></i>
            </div>
            <div>
              <h2 class="text-xl font-bold text-white">AI-Generated Learning Paths</h2>
              <p class="text-teal-100 text-sm">Personalized roadmaps to achieve your career goals</p>
            </div>
          </div>
          <button @click="showAILearningPathModal = false" class="text-white/70 hover:text-white">
            <i class="fas fa-times text-xl"></i>
          </button>
        </div>

        <div class="modal-body">
          <div class="ai-paths-container">
            <!-- Path Selection -->
            <div class="ai-paths-list">
              <div
                v-for="path in aiLearningPaths"
                :key="path.id"
                :class="['ai-path-card', { 'selected': selectedAIPath?.id === path.id }]"
                @click="selectAIPath(path)"
              >
                <div class="path-confidence">
                  <AIConfidenceBar :value="path.confidence / 100" size="sm" />
                  <span class="text-xs font-semibold text-teal-600">{{ path.confidence }}% match</span>
                </div>
                <h3 class="path-title">{{ path.title }}</h3>
                <p class="path-description">{{ path.description }}</p>
                <div class="path-meta">
                  <span><i class="fas fa-book"></i> {{ path.steps.length }} courses</span>
                  <span><i class="fas fa-clock"></i> {{ path.totalDuration }}</span>
                </div>
              </div>
            </div>

            <!-- Path Details -->
            <div v-if="selectedAIPath" class="ai-path-details">
              <div class="path-details-header">
                <h3>{{ selectedAIPath.title }}</h3>
                <span class="path-goal"><i class="fas fa-bullseye"></i> {{ selectedAIPath.goal }}</span>
              </div>

              <!-- Path Steps -->
              <div class="path-steps">
                <div
                  v-for="(step, index) in selectedAIPath.steps"
                  :key="step.courseId"
                  class="path-step"
                >
                  <div class="step-number">{{ index + 1 }}</div>
                  <div class="step-connector" v-if="index < selectedAIPath.steps.length - 1"></div>
                  <div class="step-content">
                    <div class="step-header">
                      <h4>{{ step.title }}</h4>
                      <span class="step-duration">{{ step.duration }}</span>
                    </div>
                    <div class="step-skills">
                      <span v-for="skill in step.skills" :key="skill" class="step-skill">{{ skill }}</span>
                    </div>
                    <div class="step-milestone">
                      <i class="fas fa-trophy text-amber-500"></i>
                      <span>Milestone: {{ step.milestone }}</span>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Expected Outcomes -->
              <div class="path-outcomes">
                <h4><i class="fas fa-check-circle text-green-500"></i> Expected Outcomes</h4>
                <ul>
                  <li v-for="outcome in selectedAIPath.expectedOutcomes" :key="outcome">{{ outcome }}</li>
                </ul>
              </div>

              <!-- Career Impact -->
              <div class="path-impact">
                <i class="fas fa-rocket text-purple-500"></i>
                <p>{{ selectedAIPath.careerImpact }}</p>
              </div>
            </div>
          </div>
        </div>

        <div class="modal-footer">
          <button @click="showAILearningPathModal = false" class="btn-secondary">Close</button>
          <button
            v-if="selectedAIPath"
            @click="enrollInAIPath(selectedAIPath)"
            class="btn-primary"
          >
            <i class="fas fa-rocket"></i>
            Start This Path
          </button>
        </div>
      </div>
    </div>

    <!-- Share Modal -->
    <ShareContentModal
      v-model="showShareModal"
      :title="shareCourseData?.title || ''"
      :description="shareCourseData?.description"
      :image="shareCourseData?.thumbnail"
      :url="shareCourseUrl"
      content-type="Course"
    />
  </div>
</template>

<style scoped>
.page-view {
  min-height: 100vh;
  background: linear-gradient(180deg, #f0fdfa 0%, #f8fafc 15%, #ffffff 100%);
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
    inset-inline-end: auto;
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

/* ============================================
   LESSONS LEARNED SECTION STYLES - ENHANCED
   ============================================ */

/* Main Section */
.lessons-learned-section {
  padding: 0;
}

/* Featured Insights Section - Enhanced */
.ll-featured-section {
  background: white;
  border-radius: 1.5rem;
  padding: 1.75rem;
  margin-bottom: 1.5rem;
  border: 1px solid #e5e7eb;
  box-shadow: 0 4px 24px rgba(0, 0, 0, 0.06);
}

.ll-featured-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
}

.ll-featured-title-wrap {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.ll-featured-icon {
  width: 52px;
  height: 52px;
  border-radius: 14px;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.5rem;
  color: white;
  box-shadow: 0 4px 14px rgba(20, 184, 166, 0.35);
}

.ll-featured-title {
  font-size: 1.5rem;
  font-weight: 700;
  color: #111827;
  margin: 0;
}

.ll-featured-subtitle {
  font-size: 0.9rem;
  color: #6b7280;
  margin: 0.25rem 0 0;
}

.ll-featured-nav {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.ll-featured-counter {
  font-size: 0.85rem;
  font-weight: 600;
  color: #14b8a6;
  background: #f0fdfa;
  padding: 0.5rem 1rem;
  border-radius: 9999px;
}

.ll-nav-btn {
  width: 40px;
  height: 40px;
  border-radius: 12px;
  border: 1px solid #e5e7eb;
  background: white;
  color: #6b7280;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all 0.2s ease;
}

.ll-nav-btn:hover:not(:disabled) {
  background: #14b8a6;
  border-color: #14b8a6;
  color: white;
  transform: scale(1.05);
}

.ll-nav-btn:disabled {
  opacity: 0.4;
  cursor: not-allowed;
}

/* Two Column Layout */
.ll-featured-content {
  display: grid;
  grid-template-columns: 1fr 380px;
  gap: 1.5rem;
}

/* Main Featured Card */
.ll-main-card-wrap {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.ll-main-card {
  position: relative;
  border-radius: 1.25rem;
  overflow: hidden;
  cursor: pointer;
  transition: all 0.4s ease;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 50%, #115e59 100%);
  min-height: 420px;
}

.ll-main-card:hover {
  transform: translateY(-6px);
  box-shadow: 0 20px 50px rgba(20, 184, 166, 0.3);
}

.ll-card-glow {
  position: absolute;
  top: -50%;
  right: -20%;
  width: 60%;
  height: 100%;
  background: radial-gradient(circle, rgba(255,255,255,0.15) 0%, transparent 70%);
  pointer-events: none;
}

.ll-card-pattern {
  position: absolute;
  inset: 0;
  background-image: url("data:image/svg+xml,%3Csvg width='60' height='60' viewBox='0 0 60 60' xmlns='http://www.w3.org/2000/svg'%3E%3Cg fill='none' fill-rule='evenodd'%3E%3Cg fill='%23ffffff' fill-opacity='0.05'%3E%3Cpath d='M36 34v-4h-2v4h-4v2h4v4h2v-4h4v-2h-4zm0-30V0h-2v4h-4v2h4v4h2V6h4V4h-4zM6 34v-4H4v4H0v2h4v4h2v-4h4v-2H6zM6 4V0H4v4H0v2h4v4h2V6h4V4H6z'/%3E%3C/g%3E%3C/g%3E%3C/svg%3E");
  pointer-events: none;
}

.ll-main-card-inner {
  position: relative;
  z-index: 1;
  padding: 2rem;
  color: white;
  display: flex;
  flex-direction: column;
  height: 100%;
  min-height: 420px;
}

.ll-main-top {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 1.5rem;
}

.ll-main-badges {
  display: flex;
  gap: 0.625rem;
  flex-wrap: wrap;
}

.ll-main-featured-badge {
  display: inline-flex;
  align-items: center;
  gap: 0.375rem;
  padding: 0.5rem 1rem;
  background: rgba(255, 255, 255, 0.2);
  backdrop-filter: blur(8px);
  border-radius: 9999px;
  font-size: 0.8rem;
  font-weight: 600;
}

.ll-main-featured-badge i {
  color: #fde68a;
}

.ll-main-priority {
  display: inline-flex;
  align-items: center;
  gap: 0.375rem;
  padding: 0.5rem 1rem;
  border-radius: 9999px;
  font-size: 0.8rem;
  font-weight: 600;
  text-transform: capitalize;
}

.ll-main-priority.priority-critical { background: rgba(239, 68, 68, 0.9); }
.ll-main-priority.priority-high { background: rgba(249, 115, 22, 0.9); }
.ll-main-priority.priority-medium { background: rgba(245, 158, 11, 0.9); }
.ll-main-priority.priority-low { background: rgba(34, 197, 94, 0.9); }

.ll-main-category {
  display: inline-flex;
  align-items: center;
  gap: 0.375rem;
  padding: 0.5rem 1rem;
  background: rgba(255, 255, 255, 0.15);
  backdrop-filter: blur(4px);
  border-radius: 9999px;
  font-size: 0.8rem;
  font-weight: 500;
}

.ll-main-content {
  flex: 1;
}

.ll-main-title {
  font-size: 1.75rem;
  font-weight: 700;
  margin: 0 0 0.75rem;
  line-height: 1.3;
}

.ll-main-summary {
  font-size: 1.05rem;
  opacity: 0.9;
  line-height: 1.6;
  margin: 0 0 1.25rem;
}

.ll-main-impact {
  display: flex;
  align-items: flex-start;
  gap: 1rem;
  padding: 1rem 1.25rem;
  background: rgba(255, 255, 255, 0.12);
  border-radius: 1rem;
  margin-bottom: 1.25rem;
  border: 1px solid rgba(255, 255, 255, 0.1);
}

.ll-impact-icon-wrap {
  width: 40px;
  height: 40px;
  border-radius: 10px;
  background: rgba(255, 255, 255, 0.2);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1rem;
  flex-shrink: 0;
}

.ll-impact-content {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.ll-impact-label {
  font-size: 0.75rem;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  opacity: 0.8;
}

.ll-impact-text {
  font-size: 0.95rem;
  line-height: 1.5;
}

.ll-main-takeaways {
  margin-bottom: 1.25rem;
}

.ll-takeaway-header {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.85rem;
  font-weight: 600;
  margin-bottom: 0.75rem;
  opacity: 0.9;
}

.ll-takeaway-header i {
  color: #fde68a;
}

.ll-takeaway-list {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.ll-takeaway-item {
  display: flex;
  align-items: flex-start;
  gap: 0.75rem;
  padding: 0.625rem 0.875rem;
  background: rgba(255, 255, 255, 0.1);
  border-radius: 0.75rem;
}

.ll-takeaway-num {
  width: 24px;
  height: 24px;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.2);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.75rem;
  font-weight: 700;
  flex-shrink: 0;
}

.ll-takeaway-text {
  font-size: 0.9rem;
  line-height: 1.4;
}

.ll-main-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding-top: 1.25rem;
  border-top: 1px solid rgba(255, 255, 255, 0.15);
  margin-bottom: 1rem;
}

.ll-main-author {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.ll-main-avatar {
  width: 44px;
  height: 44px;
  border-radius: 12px;
  background: rgba(255, 255, 255, 0.2);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.9rem;
  font-weight: 600;
}

.ll-main-author-info {
  display: flex;
  flex-direction: column;
}

.ll-main-author-name {
  font-size: 0.95rem;
  font-weight: 600;
}

.ll-main-author-meta {
  font-size: 0.8rem;
  opacity: 0.8;
}

.ll-main-stats {
  display: flex;
  gap: 1rem;
}

.ll-main-stat {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  font-size: 0.9rem;
  opacity: 0.9;
}

.ll-main-cta {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  padding: 0.625rem 1.25rem;
  background: white;
  border: none;
  border-radius: 0.625rem;
  font-size: 0.85rem;
  font-weight: 600;
  color: #0d9488;
  cursor: pointer;
  transition: all 0.2s ease;
}

.ll-main-cta:hover {
  background: #f0fdfa;
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.15);
}

.ll-main-cta i {
  transition: transform 0.2s ease;
}

.ll-main-cta:hover i {
  transform: translateX(4px);
}

/* Progress Dots */
.ll-progress-dots {
  display: flex;
  justify-content: center;
  gap: 0.5rem;
}

.ll-progress-dot {
  width: 10px;
  height: 10px;
  border-radius: 50%;
  border: none;
  background: #e5e7eb;
  cursor: pointer;
  transition: all 0.2s ease;
  padding: 0;
}

.ll-progress-dot:hover {
  background: #99f6e4;
}

.ll-progress-dot.active {
  background: #14b8a6;
  width: 32px;
  border-radius: 5px;
}

/* Next Lessons Sidebar */
.ll-next-lessons {
  display: flex;
  flex-direction: column;
  background: #f8fafc;
  border-radius: 1.25rem;
  padding: 1.25rem;
  border: 1px solid #e5e7eb;
}

.ll-next-header {
  display: flex;
  align-items: center;
  gap: 0.625rem;
  font-size: 1rem;
  font-weight: 700;
  color: #111827;
  margin-bottom: 1rem;
  padding-bottom: 0.75rem;
  border-bottom: 1px solid #e5e7eb;
}

.ll-next-header i {
  color: #14b8a6;
}

.ll-next-list {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
  flex: 1;
}

.ll-next-card {
  display: flex;
  align-items: flex-start;
  gap: 0.875rem;
  padding: 1rem;
  background: white;
  border-radius: 1rem;
  border: 1px solid #e5e7eb;
  cursor: pointer;
  transition: all 0.25s ease;
}

.ll-next-card:hover {
  border-color: #14b8a6;
  box-shadow: 0 4px 16px rgba(20, 184, 166, 0.12);
  transform: translateX(4px);
}

.ll-next-index {
  width: 28px;
  height: 28px;
  border-radius: 8px;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.8rem;
  font-weight: 700;
  flex-shrink: 0;
}

.ll-next-content {
  flex: 1;
  min-width: 0;
}

.ll-next-meta {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-bottom: 0.375rem;
}

.ll-next-category {
  display: inline-flex;
  align-items: center;
  gap: 0.25rem;
  font-size: 0.7rem;
  color: #6b7280;
}

.ll-next-category i {
  font-size: 0.65rem;
  color: #14b8a6;
}

.ll-next-priority {
  font-size: 0.65rem;
  font-weight: 600;
  text-transform: capitalize;
  padding: 0.125rem 0.5rem;
  border-radius: 9999px;
}

.ll-next-priority.priority-critical { background: #fee2e2; color: #dc2626; }
.ll-next-priority.priority-high { background: #ffedd5; color: #ea580c; }
.ll-next-priority.priority-medium { background: #fef3c7; color: #d97706; }
.ll-next-priority.priority-low { background: #dcfce7; color: #16a34a; }

.ll-next-title {
  font-size: 0.9rem;
  font-weight: 600;
  color: #111827;
  margin: 0 0 0.375rem;
  line-height: 1.3;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.ll-next-summary {
  font-size: 0.8rem;
  color: #6b7280;
  margin: 0 0 0.5rem;
  line-height: 1.4;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.ll-next-footer {
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.ll-next-author {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  font-size: 0.75rem;
  color: #6b7280;
}

.ll-next-avatar {
  width: 20px;
  height: 20px;
  border-radius: 6px;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.6rem;
  font-weight: 600;
}

.ll-next-stats {
  display: flex;
  align-items: center;
  gap: 0.25rem;
  font-size: 0.75rem;
  color: #9ca3af;
}

.ll-next-arrow {
  width: 28px;
  height: 28px;
  border-radius: 8px;
  background: #f3f4f6;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #9ca3af;
  font-size: 0.7rem;
  flex-shrink: 0;
  transition: all 0.2s ease;
}

.ll-next-card:hover .ll-next-arrow {
  background: #14b8a6;
  color: white;
}

.ll-view-all-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  width: 100%;
  padding: 0.875rem;
  margin-top: 0.75rem;
  background: white;
  border: 1px solid #14b8a6;
  border-radius: 0.75rem;
  font-size: 0.85rem;
  font-weight: 600;
  color: #14b8a6;
  cursor: pointer;
  transition: all 0.2s ease;
}

.ll-view-all-btn:hover {
  background: #14b8a6;
  color: white;
}

.ll-view-all-btn i {
  transition: transform 0.2s ease;
}

.ll-view-all-btn:hover i {
  transform: translateX(4px);
}

/* Responsive for Featured Section */
@media (max-width: 1024px) {
  .ll-featured-content {
    grid-template-columns: 1fr;
  }

  .ll-next-lessons {
    display: none;
  }

  .ll-featured-header {
    flex-direction: column;
    gap: 1rem;
    align-items: flex-start;
  }
}

@media (max-width: 768px) {
  .ll-featured-section {
    padding: 1.25rem;
  }

  .ll-main-card-inner {
    padding: 1.5rem;
    min-height: auto;
  }

  .ll-main-title {
    font-size: 1.35rem;
  }

  .ll-main-summary {
    font-size: 0.95rem;
  }

  .ll-main-footer {
    flex-direction: column;
    gap: 1rem;
    align-items: flex-start;
  }
}

/* Legacy carousel styles - keep for compatibility */
.ll-carousel-priority {
  display: inline-flex;
  align-items: center;
  gap: 0.375rem;
  padding: 0.375rem 0.875rem;
  border-radius: 9999px;
  font-size: 0.75rem;
  font-weight: 600;
  text-transform: capitalize;
}

.ll-carousel-priority.priority-critical { background: rgba(239, 68, 68, 0.9); }
.ll-carousel-priority.priority-high { background: rgba(249, 115, 22, 0.9); }
.ll-carousel-priority.priority-medium { background: rgba(245, 158, 11, 0.9); }
.ll-carousel-priority.priority-low { background: rgba(34, 197, 94, 0.9); }

.ll-carousel-category {
  display: inline-flex;
  align-items: center;
  gap: 0.375rem;
  padding: 0.375rem 0.875rem;
  background: rgba(255, 255, 255, 0.2);
  backdrop-filter: blur(4px);
  border-radius: 9999px;
  font-size: 0.75rem;
  font-weight: 500;
}

.ll-carousel-card-title {
  font-size: 1.5rem;
  font-weight: 700;
  margin: 0 0 0.75rem;
  line-height: 1.3;
}

.ll-carousel-card-summary {
  font-size: 1rem;
  opacity: 0.9;
  line-height: 1.6;
  margin: 0 0 1rem;
}

.ll-carousel-impact {
  display: flex;
  align-items: flex-start;
  gap: 0.625rem;
  padding: 0.875rem 1rem;
  background: rgba(255, 255, 255, 0.15);
  border-radius: 0.75rem;
  margin-bottom: 1rem;
}

.ll-carousel-impact i {
  font-size: 1rem;
  margin-top: 0.125rem;
}

.ll-carousel-impact span {
  font-size: 0.9rem;
  line-height: 1.5;
}

.ll-carousel-recommendations {
  margin-bottom: 1rem;
}

.ll-reco-label {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.85rem;
  font-weight: 600;
  margin-bottom: 0.625rem;
  opacity: 0.9;
}

.ll-carousel-recommendations ul {
  list-style: none;
  padding: 0;
  margin: 0;
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.ll-carousel-recommendations li {
  display: flex;
  align-items: flex-start;
  gap: 0.5rem;
  font-size: 0.85rem;
  opacity: 0.9;
  line-height: 1.4;
  padding-left: 1rem;
  position: relative;
}

.ll-carousel-recommendations li::before {
  content: '→';
  position: absolute;
  left: 0;
  opacity: 0.7;
}

.ll-carousel-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: auto;
  padding-top: 1rem;
  border-top: 1px solid rgba(255, 255, 255, 0.2);
}

.ll-carousel-author {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.ll-carousel-avatar {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.25);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.85rem;
  font-weight: 600;
}

.ll-carousel-author-info {
  display: flex;
  flex-direction: column;
}

.ll-carousel-author-name {
  font-size: 0.9rem;
  font-weight: 600;
}

.ll-carousel-author-meta {
  font-size: 0.8rem;
  opacity: 0.8;
}

.ll-carousel-stats {
  display: flex;
  gap: 1rem;
}

.ll-carousel-stats span {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  font-size: 0.85rem;
  opacity: 0.9;
}

.ll-carousel-cta {
  padding: 1rem 1.75rem 1.5rem;
  background: rgba(0, 0, 0, 0.1);
}

.ll-carousel-read-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.625rem;
  width: 100%;
  padding: 0.875rem 1.5rem;
  background: white;
  border: none;
  border-radius: 0.75rem;
  font-size: 0.95rem;
  font-weight: 600;
  color: #111827;
  cursor: pointer;
  transition: all 0.2s ease;
}

.ll-carousel-read-btn:hover {
  background: #f9fafb;
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

@media (max-width: 768px) {
  .ll-carousel-header {
    flex-direction: column;
    gap: 1rem;
    align-items: flex-start;
  }

  .ll-carousel-card {
    min-height: auto;
  }

  .ll-carousel-card-title {
    font-size: 1.25rem;
  }

  .ll-carousel-card-summary {
    font-size: 0.9rem;
  }

  .ll-carousel-footer {
    flex-direction: column;
    gap: 1rem;
    align-items: flex-start;
  }
}

/* Stats Bar */
.ll-stats-bar {
  display: flex;
  align-items: center;
  gap: 0;
  padding: 1rem 1.5rem;
  background: white;
  border-radius: 1rem;
  border: 1px solid #e5e7eb;
  margin-bottom: 1.5rem;
}

.ll-stat-item {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  flex: 1;
}

.ll-stat-icon {
  width: 40px;
  height: 40px;
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1rem;
}

.ll-stat-icon.total { background: #ccfbf4; color: #14b8a6; }
.ll-stat-icon.views { background: #dbeafe; color: #3b82f6; }
.ll-stat-icon.likes { background: #fee2e2; color: #ef4444; }
.ll-stat-icon.critical { background: #fee2e2; color: #ef4444; }
.ll-stat-icon.bookmarked { background: #ccfbf4; color: #14b8a6; }

.ll-stat-info {
  display: flex;
  flex-direction: column;
}

.ll-stat-value {
  font-size: 1.25rem;
  font-weight: 700;
  color: #111827;
  line-height: 1;
}

.ll-stat-label {
  font-size: 0.75rem;
  color: #6b7280;
  margin-top: 0.25rem;
}

.ll-stat-divider {
  width: 1px;
  height: 40px;
  background: #e5e7eb;
  margin: 0 1rem;
}

/* Category Chips */
.ll-category-chips {
  display: flex;
  gap: 0.5rem;
  flex-wrap: wrap;
  margin-bottom: 1.5rem;
}

.ll-category-chip {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem 1rem;
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 9999px;
  font-size: 0.8rem;
  color: #6b7280;
  cursor: pointer;
  transition: all 0.2s ease;
}

.ll-category-chip:hover {
  border-color: #d1d5db;
  background: #f9fafb;
}

.ll-category-chip.active {
  border-color: transparent;
  font-weight: 500;
}

.ll-category-chip.active.chip-technical { background: #dbeafe; color: #1d4ed8; }
.ll-category-chip.active.chip-process { background: #f3e8ff; color: #7c3aed; }
.ll-category-chip.active.chip-communication { background: #ccfbf1; color: #0f766e; }
.ll-category-chip.active.chip-leadership { background: #e0e7ff; color: #4338ca; }
.ll-category-chip.active.chip-project { background: #cffafe; color: #0e7490; }
.ll-category-chip.active.chip-other { background: #f3f4f6; color: #374151; }

.ll-chip-count {
  padding: 0.125rem 0.5rem;
  background: rgba(0, 0, 0, 0.08);
  border-radius: 9999px;
  font-size: 0.7rem;
  font-weight: 600;
}

.ll-category-chip.active .ll-chip-count {
  background: rgba(0, 0, 0, 0.1);
}

/* Toolbar */
.ll-toolbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 1rem;
  margin-bottom: 1.5rem;
  flex-wrap: wrap;
}

.ll-search-wrap {
  position: relative;
  flex: 1;
  max-width: 400px;
}

.ll-search-wrap i.fa-search {
  position: absolute;
  left: 1rem;
  top: 50%;
  transform: translateY(-50%);
  color: #9ca3af;
  font-size: 0.875rem;
}

.ll-search-wrap input {
  width: 100%;
  padding: 0.75rem 2.5rem 0.75rem 2.75rem;
  border: 1px solid #e5e7eb;
  border-radius: 0.875rem;
  font-size: 0.875rem;
  background: white;
  transition: all 0.2s ease;
}

.ll-search-wrap input:focus {
  outline: none;
  border-color: #14b8a6;
  box-shadow: 0 0 0 3px rgba(20, 184, 166, 0.1);
}

.ll-search-clear {
  position: absolute;
  right: 0.75rem;
  top: 50%;
  transform: translateY(-50%);
  width: 22px;
  height: 22px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 50%;
  background: #f3f4f6;
  color: #6b7280;
  border: none;
  cursor: pointer;
  transition: all 0.2s ease;
}

.ll-search-clear:hover {
  background: #e5e7eb;
  color: #374151;
}

.ll-toolbar-actions {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

/* Filter Dropdown */
.ll-filter-dropdown {
  position: relative;
}

.ll-filter-btn {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem 1rem;
  border: 1px solid #e5e7eb;
  border-radius: 0.75rem;
  background: white;
  font-size: 0.85rem;
  color: #374151;
  cursor: pointer;
  transition: all 0.2s ease;
  text-transform: capitalize;
}

.ll-filter-btn:hover {
  border-color: #d1d5db;
  background: #f9fafb;
}

.ll-filter-btn.active {
  border-color: #14b8a6;
  background: #f0fdfa;
  color: #0f766e;
}

.ll-dropdown-menu {
  position: absolute;
  top: calc(100% + 4px);
  left: 0;
  min-width: 180px;
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 0.75rem;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.12);
  z-index: 50;
  padding: 0.5rem;
}

.ll-dropdown-menu.right {
  left: auto;
  right: 0;
}

.ll-dropdown-item {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  width: 100%;
  padding: 0.625rem 0.875rem;
  border: none;
  background: transparent;
  font-size: 0.85rem;
  color: #374151;
  cursor: pointer;
  border-radius: 0.5rem;
  transition: all 0.2s ease;
  text-transform: capitalize;
}

.ll-dropdown-item:hover {
  background: #f3f4f6;
}

.ll-dropdown-item.active {
  background: #f0fdfa;
  color: #0f766e;
}

.ll-dropdown-item .fa-check {
  margin-left: auto;
  color: #14b8a6;
}

.ll-priority-dot {
  width: 10px;
  height: 10px;
  border-radius: 50%;
}

.ll-priority-dot.priority-all { background: #9ca3af; }
.ll-priority-dot.priority-critical { background: #ef4444; }
.ll-priority-dot.priority-high { background: #f97316; }
.ll-priority-dot.priority-medium { background: #f59e0b; }
.ll-priority-dot.priority-low { background: #22c55e; }

/* Active Filters */
.ll-active-filters {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.75rem 1rem;
  background: #f0fdfa;
  border: 1px solid #5eead4;
  border-radius: 0.75rem;
  margin-bottom: 1.5rem;
  flex-wrap: wrap;
}

.ll-filters-label {
  font-size: 0.8rem;
  font-weight: 500;
  color: #0f766e;
  display: flex;
  align-items: center;
  gap: 0.375rem;
}

.ll-filter-chips {
  display: flex;
  gap: 0.5rem;
  flex-wrap: wrap;
}

.ll-active-chip {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  padding: 0.375rem 0.75rem;
  background: white;
  border: 1px solid #5eead4;
  border-radius: 9999px;
  font-size: 0.75rem;
  color: #0f766e;
}

.ll-active-chip button {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 16px;
  height: 16px;
  border: none;
  background: transparent;
  color: #14b8a6;
  cursor: pointer;
  padding: 0;
  margin-left: 0.25rem;
}

.ll-active-chip button:hover {
  color: #ef4444;
}

.ll-clear-all {
  margin-left: auto;
  padding: 0.375rem 0.75rem;
  border: none;
  background: transparent;
  font-size: 0.8rem;
  color: #0f766e;
  cursor: pointer;
  font-weight: 500;
  transition: all 0.2s ease;
}

.ll-clear-all:hover {
  color: #ef4444;
}

/* Results Header */
.ll-results-header {
  margin-bottom: 1rem;
}

.ll-results-count {
  font-size: 0.9rem;
  color: #6b7280;
}

/* Grid */
.ll-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 1.5rem;
}

/* Card */
.ll-card {
  background: white;
  border-radius: 1rem;
  border: 1px solid #e5e7eb;
  overflow: hidden;
  cursor: pointer;
  transition: all 0.3s ease;
  display: flex;
  flex-direction: column;
  position: relative;
}

.ll-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 12px 40px rgba(0, 0, 0, 0.1);
  border-color: #14b8a6;
}

.ll-card.featured {
  border-color: #5eead4;
}

.ll-card-accent {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 4px;
}

.ll-card-accent.cat-technical { background: linear-gradient(90deg, #3b82f6, #60a5fa); }
.ll-card-accent.cat-process { background: linear-gradient(90deg, #8b5cf6, #a78bfa); }
.ll-card-accent.cat-communication { background: linear-gradient(90deg, #14b8a6, #2dd4bf); }
.ll-card-accent.cat-leadership { background: linear-gradient(90deg, #6366f1, #818cf8); }
.ll-card-accent.cat-project { background: linear-gradient(90deg, #06b6d4, #22d3ee); }
.ll-card-accent.cat-other { background: linear-gradient(90deg, #6b7280, #9ca3af); }

.ll-card-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  padding: 1.25rem 1.25rem 0;
}

.ll-card-badges {
  display: flex;
  gap: 0.5rem;
  flex-wrap: wrap;
}

.ll-priority-tag {
  padding: 0.25rem 0.625rem;
  border-radius: 9999px;
  font-size: 0.7rem;
  font-weight: 600;
  text-transform: capitalize;
}

.ll-priority-tag.priority-critical { background: #fee2e2; color: #dc2626; }
.ll-priority-tag.priority-high { background: #ffedd5; color: #ea580c; }
.ll-priority-tag.priority-medium { background: #fef3c7; color: #d97706; }
.ll-priority-tag.priority-low { background: #dcfce7; color: #16a34a; }

.ll-featured-tag {
  display: flex;
  align-items: center;
  gap: 0.25rem;
  padding: 0.25rem 0.625rem;
  background: #ccfbf4;
  color: #14b8a6;
  border-radius: 9999px;
  font-size: 0.7rem;
  font-weight: 600;
}

.ll-bookmark-btn {
  width: 32px;
  height: 32px;
  display: flex;
  align-items: center;
  justify-content: center;
  border: none;
  background: #f3f4f6;
  border-radius: 0.5rem;
  color: #9ca3af;
  cursor: pointer;
  transition: all 0.2s ease;
}

.ll-bookmark-btn:hover {
  background: #e5e7eb;
  color: #14b8a6;
}

.ll-bookmark-btn.active {
  background: #ccfbf4;
  color: #14b8a6;
}

/* Card Body */
.ll-card-body {
  padding: 1rem 1.25rem;
  flex: 1;
}

.ll-card-category {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  font-size: 0.75rem;
  color: #6b7280;
  margin-bottom: 0.625rem;
}

.ll-card-category i {
  font-size: 0.7rem;
}

.ll-project-name {
  color: #9ca3af;
}

.ll-card-title {
  font-size: 1rem;
  font-weight: 600;
  color: #111827;
  line-height: 1.4;
  margin: 0 0 0.5rem;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.ll-card-summary {
  font-size: 0.85rem;
  color: #6b7280;
  line-height: 1.5;
  margin: 0 0 0.875rem;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.ll-impact-box {
  display: flex;
  align-items: flex-start;
  gap: 0.5rem;
  padding: 0.625rem 0.75rem;
  background: #f0fdf4;
  border-radius: 0.5rem;
  margin-bottom: 0.875rem;
}

.ll-impact-box i {
  color: #22c55e;
  font-size: 0.8rem;
  margin-top: 0.125rem;
}

.ll-impact-box span {
  font-size: 0.75rem;
  color: #166534;
  line-height: 1.4;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.ll-card-tags {
  display: flex;
  gap: 0.375rem;
  flex-wrap: wrap;
}

.ll-tag {
  padding: 0.25rem 0.5rem;
  background: #f3f4f6;
  border-radius: 0.375rem;
  font-size: 0.7rem;
  color: #6b7280;
}

.ll-tag-more {
  padding: 0.25rem 0.5rem;
  background: #e5e7eb;
  border-radius: 0.375rem;
  font-size: 0.7rem;
  color: #374151;
  font-weight: 500;
}

/* Card Footer */
.ll-card-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem 1.25rem;
  border-top: 1px solid #f3f4f6;
  background: #fafafa;
}

.ll-card-author {
  display: flex;
  align-items: center;
  gap: 0.625rem;
}

.ll-avatar {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.7rem;
  font-weight: 600;
}

.ll-author-details {
  display: flex;
  flex-direction: column;
}

.ll-name {
  font-size: 0.8rem;
  font-weight: 500;
  color: #374151;
}

.ll-meta {
  font-size: 0.7rem;
  color: #9ca3af;
}

.ll-card-stats {
  display: flex;
  gap: 0.75rem;
}

.ll-stat {
  display: flex;
  align-items: center;
  gap: 0.25rem;
  font-size: 0.75rem;
  color: #9ca3af;
}

.ll-stat i {
  font-size: 0.7rem;
}

.ll-like-btn {
  border: none;
  background: transparent;
  cursor: pointer;
  padding: 0;
  transition: all 0.2s ease;
}

.ll-like-btn:hover {
  color: #ef4444;
}

.ll-like-btn.liked {
  color: #ef4444;
}

/* Empty State */
.ll-empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 4rem 2rem;
  text-align: center;
  background: white;
  border-radius: 1rem;
  border: 1px solid #e5e7eb;
}

.ll-empty-icon {
  width: 80px;
  height: 80px;
  border-radius: 50%;
  background: linear-gradient(135deg, #ccfbf4 0%, #99f6e4 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 1.5rem;
}

.ll-empty-icon i {
  font-size: 2rem;
  color: #14b8a6;
}

.ll-empty-state h3 {
  font-size: 1.25rem;
  font-weight: 600;
  color: #111827;
  margin: 0 0 0.5rem;
}

.ll-empty-state p {
  font-size: 0.9rem;
  color: #6b7280;
  margin: 0 0 1.5rem;
}

.ll-empty-btn {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem 1.5rem;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
  border: none;
  border-radius: 0.75rem;
  font-size: 0.9rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s ease;
}

.ll-empty-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.3);
}

/* Modal */
.ll-modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.5);
  backdrop-filter: blur(4px);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  padding: 2rem;
}

.ll-modal {
  background: white;
  border-radius: 1.25rem;
  width: 100%;
  max-width: 720px;
  max-height: 90vh;
  overflow: hidden;
  display: flex;
  flex-direction: column;
  box-shadow: 0 25px 50px rgba(0, 0, 0, 0.25);
}

.ll-modal-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  padding: 1.25rem 1.5rem;
  border-bottom: 1px solid #e5e7eb;
}

.ll-modal-header.cat-technical { background: linear-gradient(135deg, #dbeafe 0%, #bfdbfe 100%); }
.ll-modal-header.cat-process { background: linear-gradient(135deg, #f3e8ff 0%, #e9d5ff 100%); }
.ll-modal-header.cat-communication { background: linear-gradient(135deg, #ccfbf1 0%, #99f6e4 100%); }
.ll-modal-header.cat-leadership { background: linear-gradient(135deg, #e0e7ff 0%, #c7d2fe 100%); }
.ll-modal-header.cat-project { background: linear-gradient(135deg, #cffafe 0%, #a5f3fc 100%); }
.ll-modal-header.cat-other { background: linear-gradient(135deg, #f3f4f6 0%, #e5e7eb 100%); }

.ll-modal-badges {
  display: flex;
  gap: 0.5rem;
  flex-wrap: wrap;
}

.ll-modal-priority {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  padding: 0.375rem 0.75rem;
  border-radius: 9999px;
  font-size: 0.75rem;
  font-weight: 600;
  text-transform: capitalize;
}

.ll-modal-priority.priority-critical { background: #fee2e2; color: #dc2626; }
.ll-modal-priority.priority-high { background: #ffedd5; color: #ea580c; }
.ll-modal-priority.priority-medium { background: #fef3c7; color: #d97706; }
.ll-modal-priority.priority-low { background: #dcfce7; color: #16a34a; }

.ll-modal-category {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  padding: 0.375rem 0.75rem;
  background: rgba(255, 255, 255, 0.6);
  border-radius: 9999px;
  font-size: 0.75rem;
  font-weight: 500;
  color: #374151;
}

.ll-modal-close {
  width: 36px;
  height: 36px;
  display: flex;
  align-items: center;
  justify-content: center;
  border: none;
  background: rgba(255, 255, 255, 0.6);
  border-radius: 50%;
  color: #6b7280;
  cursor: pointer;
  transition: all 0.2s ease;
}

.ll-modal-close:hover {
  background: white;
  color: #374151;
}

.ll-modal-body {
  flex: 1;
  overflow-y: auto;
  padding: 1.5rem;
}

.ll-modal-title {
  font-size: 1.5rem;
  font-weight: 700;
  color: #111827;
  margin: 0 0 1rem;
  line-height: 1.3;
}

.ll-modal-meta {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 1rem;
  margin-bottom: 1.5rem;
  flex-wrap: wrap;
}

.ll-modal-author {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.ll-modal-avatar {
  width: 44px;
  height: 44px;
  border-radius: 50%;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.9rem;
  font-weight: 600;
}

.ll-modal-author-name {
  display: block;
  font-size: 0.95rem;
  font-weight: 500;
  color: #374151;
}

.ll-modal-author-dept {
  display: block;
  font-size: 0.8rem;
  color: #9ca3af;
}

.ll-modal-info {
  display: flex;
  gap: 1rem;
  flex-wrap: wrap;
}

.ll-modal-info span {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  font-size: 0.85rem;
  color: #6b7280;
}

.ll-modal-stats {
  display: flex;
  gap: 1.5rem;
  padding: 1rem;
  background: #f9fafb;
  border-radius: 0.75rem;
  margin-bottom: 1.5rem;
}

.ll-modal-stats span {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  font-size: 0.85rem;
  color: #6b7280;
}

.ll-modal-section {
  margin-bottom: 1.5rem;
}

.ll-modal-section h4 {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.9rem;
  font-weight: 600;
  color: #374151;
  margin: 0 0 0.75rem;
}

.ll-modal-section h4 i {
  color: #14b8a6;
}

.ll-modal-section p {
  font-size: 0.9rem;
  color: #4b5563;
  line-height: 1.6;
  margin: 0;
}

.ll-modal-impact {
  margin-bottom: 1.5rem;
}

.ll-modal-impact h4 {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.9rem;
  font-weight: 600;
  color: #374151;
  margin: 0 0 0.75rem;
}

.ll-modal-impact h4 i {
  color: #22c55e;
}

.ll-impact-highlight {
  padding: 1rem;
  background: #f0fdf4;
  border-left: 3px solid #22c55e;
  border-radius: 0.5rem;
  font-size: 0.9rem;
  color: #166534;
  line-height: 1.6;
}

.ll-recommendations {
  list-style: none;
  padding: 0;
  margin: 0;
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.ll-recommendations li {
  display: flex;
  align-items: flex-start;
  gap: 0.625rem;
  padding: 0.75rem 1rem;
  background: #f0fdfa;
  border-radius: 0.5rem;
  font-size: 0.85rem;
  color: #0f766e;
}

.ll-recommendations li i {
  color: #14b8a6;
  font-size: 0.875rem;
  margin-top: 0.125rem;
}

.ll-modal-tags {
  display: flex;
  gap: 0.5rem;
  flex-wrap: wrap;
}

.ll-modal-tag {
  padding: 0.375rem 0.75rem;
  background: #f3f4f6;
  border-radius: 9999px;
  font-size: 0.8rem;
  color: #6b7280;
}

.ll-modal-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem 1.5rem;
  border-top: 1px solid #e5e7eb;
  background: #fafafa;
}

.ll-modal-actions {
  display: flex;
  gap: 0.5rem;
}

.ll-action-btn {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.625rem 1rem;
  border: 1px solid #e5e7eb;
  border-radius: 0.75rem;
  background: white;
  font-size: 0.85rem;
  color: #374151;
  cursor: pointer;
  transition: all 0.2s ease;
}

.ll-action-btn:hover {
  background: #f9fafb;
  border-color: #d1d5db;
}

.ll-action-btn.active {
  background: #ccfbf4;
  border-color: #5eead4;
  color: #0f766e;
}

.ll-close-btn {
  padding: 0.625rem 1.25rem;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  border: none;
  border-radius: 0.75rem;
  font-size: 0.85rem;
  font-weight: 600;
  color: white;
  cursor: pointer;
  transition: all 0.2s ease;
}

.ll-close-btn:hover {
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.3);
}

/* Responsive */
@media (max-width: 1200px) {
  .ll-grid {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 768px) {
  .ll-grid {
    grid-template-columns: 1fr;
  }
  .ll-stats-bar {
    flex-wrap: wrap;
    gap: 1rem;
  }
  .ll-stat-divider {
    display: none;
  }
  .ll-stat-item {
    flex: 0 0 calc(50% - 0.5rem);
  }
  .ll-toolbar {
    flex-direction: column;
    align-items: stretch;
  }
  .ll-search-wrap {
    max-width: none;
  }
  .ll-toolbar-actions {
    justify-content: flex-start;
  }
  .ll-modal {
    max-height: 100vh;
    border-radius: 0;
  }
  .ll-modal-overlay {
    padding: 0;
  }
}

/* ============================================
   END LESSONS LEARNED SECTION STYLES
   ============================================ */

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

.cert-stat-total .cert-stat-glow { background: radial-gradient(circle, #14b8a6 0%, transparent 70%); }
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

.cert-stat-total .cert-stat-icon { background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%); }
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

.cert-stat-total .cert-stat-badge { background: #ccfbf1; color: #0d9488; }
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
  background: linear-gradient(180deg, #f0fdfa 0%, white 100%);
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
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 1.125rem;
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.3);
}

.cert-header-icon-ring {
  position: absolute;
  inset: -4px;
  border-radius: 1.25rem;
  border: 2px solid rgba(20, 184, 166, 0.2);
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
  border-color: #14b8a6;
  box-shadow: 0 0 0 3px rgba(20, 184, 166, 0.1);
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
  background: #ccfbf1;
  border-color: #5eead4;
  color: #115e59;
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
  background: #ccfbf1;
  color: #115e59;
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
  background: #14b8a6;
  border-color: #14b8a6;
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
  color: #14b8a6;
}

.cert-dropdown-option .check-icon {
  margin-left: auto;
  font-size: 0.75rem;
  color: #14b8a6;
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
  color: #14b8a6;
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
  color: #14b8a6;
  font-size: 0.75rem;
  cursor: pointer;
  transition: all 0.2s ease;
}

.cert-sort-order-btn:hover {
  background: #ccfbf1;
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
  background: #14b8a6;
  color: white;
}

/* Active Filters */
.cert-active-filters {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.875rem 1.5rem;
  background: #f0fdfa;
  border-bottom: 1px solid #ccfbf1;
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
  background: #ccfbf1;
  color: #115e59;
  border: 1px solid #5eead4;
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
  border-color: #5eead4;
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
  color: #14b8a6;
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
  background: linear-gradient(135deg, #ccfbf1 0%, #99f6e4 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 0 auto 1.5rem;
}

.cert-empty-icon i {
  font-size: 2rem;
  color: #14b8a6;
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
  border-color: #14b8a6;
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
  background: #ccfbf1;
  color: #115e59;
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

/* ============================================================================
   AI FEATURES STYLES
   ============================================================================ */

/* AI Insights Bar */
.ai-insights-bar {
  background: linear-gradient(135deg, #f0fdfa 0%, #f0f9ff 100%);
  border-bottom: 1px solid rgba(20, 184, 166, 0.1);
  padding: 0.75rem 2rem;
}

.ai-insights-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.ai-icon-badge {
  width: 32px;
  height: 32px;
  background: linear-gradient(135deg, #14b8a6 0%, #06b6d4 100%);
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 0.875rem;
}

.ai-insights-content {
  display: flex;
  gap: 1rem;
  margin-top: 0.75rem;
  padding-top: 0.75rem;
  border-top: 1px solid rgba(20, 184, 166, 0.1);
  overflow-x: auto;
  padding-bottom: 0.5rem;
}

.ai-insight-card {
  min-width: 280px;
  padding: 0.75rem 1rem;
  border-radius: 12px;
  border: 1px solid;
  flex-shrink: 0;
}

.ai-insight-icon {
  width: 32px;
  height: 32px;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 0.75rem;
  flex-shrink: 0;
}

/* AI Recommendations Section */
.ai-recommendations-section {
  background: white;
  border-radius: 16px;
  border: 1px solid #e5e7eb;
  padding: 1.5rem;
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.05);
}

.ai-section-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 1.5rem;
  padding-bottom: 1rem;
  border-bottom: 1px solid #f3f4f6;
}

.ai-section-icon {
  width: 44px;
  height: 44px;
  background: linear-gradient(135deg, #14b8a6 0%, #06b6d4 100%);
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 1.125rem;
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.3);
}

.ai-loading-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 3rem;
}

.ai-recommendations-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 1.25rem;
}

.ai-recommendation-card {
  background: linear-gradient(135deg, #ffffff 0%, #f8fafc 100%);
  border: 1px solid #e5e7eb;
  border-radius: 16px;
  padding: 1.25rem;
  cursor: pointer;
  transition: all 0.3s ease;
}

.ai-recommendation-card:hover {
  border-color: #14b8a6;
  box-shadow: 0 8px 24px rgba(20, 184, 166, 0.15);
  transform: translateY(-4px);
}

.ai-rec-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 0.75rem;
}

.ai-match-score {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.ai-priority-badge {
  padding: 0.25rem 0.75rem;
  border-radius: 20px;
  font-size: 0.7rem;
  font-weight: 600;
  display: flex;
  align-items: center;
  gap: 0.25rem;
}

.ai-priority-badge.priority-high {
  background: linear-gradient(135deg, #fef3c7 0%, #fde68a 100%);
  color: #92400e;
}

.ai-priority-badge.priority-medium {
  background: linear-gradient(135deg, #e0f2fe 0%, #bae6fd 100%);
  color: #0369a1;
}

.ai-rec-title {
  font-size: 1.125rem;
  font-weight: 700;
  color: #1f2937;
  margin-bottom: 0.75rem;
}

.ai-rec-reason {
  display: flex;
  gap: 0.5rem;
  padding: 0.75rem;
  background: #fffbeb;
  border-radius: 10px;
  margin-bottom: 1rem;
}

.ai-rec-reason p {
  font-size: 0.8rem;
  color: #78350f;
  line-height: 1.4;
}

.ai-rec-skills {
  margin-bottom: 1rem;
}

.ai-rec-footer {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding-top: 0.75rem;
  border-top: 1px solid #f3f4f6;
}

.ai-rec-enroll-btn {
  padding: 0.5rem 1rem;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
  border: none;
  border-radius: 8px;
  font-size: 0.8rem;
  font-weight: 600;
  display: flex;
  align-items: center;
  gap: 0.375rem;
  cursor: pointer;
  transition: all 0.2s;
}

.ai-rec-enroll-btn:hover {
  background: linear-gradient(135deg, #0d9488 0%, #0f766e 100%);
  transform: scale(1.02);
}

/* Modal Styles */
.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.5);
  backdrop-filter: blur(4px);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  padding: 2rem;
}

.skill-gap-modal,
.ai-path-modal {
  background: white;
  border-radius: 20px;
  max-width: 900px;
  width: 100%;
  max-height: 85vh;
  overflow: hidden;
  display: flex;
  flex-direction: column;
  box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.25);
}

.modal-header {
  padding: 1.5rem 2rem;
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.modal-body {
  flex: 1;
  overflow-y: auto;
  padding: 1.5rem 2rem;
}

.modal-footer {
  padding: 1rem 2rem;
  background: #f9fafb;
  border-top: 1px solid #e5e7eb;
  display: flex;
  justify-content: flex-end;
  gap: 0.75rem;
}

.btn-secondary {
  padding: 0.625rem 1.25rem;
  background: white;
  border: 1px solid #d1d5db;
  border-radius: 10px;
  color: #4b5563;
  font-weight: 600;
  font-size: 0.875rem;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-secondary:hover {
  background: #f3f4f6;
}

.btn-primary {
  padding: 0.625rem 1.25rem;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  border: none;
  border-radius: 10px;
  color: white;
  font-weight: 600;
  font-size: 0.875rem;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  transition: all 0.2s;
}

.btn-primary:hover {
  background: linear-gradient(135deg, #0d9488 0%, #0f766e 100%);
  transform: translateY(-1px);
}

/* Skill Gap Modal Styles */
.skill-overview {
  display: grid;
  grid-template-columns: 200px 1fr;
  gap: 2rem;
  margin-bottom: 2rem;
  padding-bottom: 1.5rem;
  border-bottom: 1px solid #e5e7eb;
}

.readiness-score {
  display: flex;
  flex-direction: column;
  align-items: center;
}

.readiness-circle {
  position: relative;
  width: 140px;
  height: 140px;
}

.readiness-circle svg {
  width: 100%;
  height: 100%;
}

.readiness-value {
  position: absolute;
  inset: 0;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
}

.strength-weakness {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.sw-section h4 {
  font-size: 0.875rem;
  font-weight: 600;
  color: #374151;
  margin-bottom: 0.5rem;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.sw-chips {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
}

.sw-chip {
  padding: 0.375rem 0.75rem;
  border-radius: 20px;
  font-size: 0.75rem;
  font-weight: 500;
}

.sw-chip.strength {
  background: #dcfce7;
  color: #166534;
}

.sw-chip.weakness {
  background: #fef3c7;
  color: #92400e;
}

.skill-gaps-list {
  margin-bottom: 1.5rem;
}

.skill-gap-items {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.skill-gap-item {
  background: #f9fafb;
  border-radius: 12px;
  padding: 1rem;
}

.gap-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 0.75rem;
}

.priority-tag {
  padding: 0.25rem 0.5rem;
  border-radius: 6px;
  font-size: 0.65rem;
  font-weight: 600;
  text-transform: uppercase;
  border: 1px solid;
}

.gap-bars {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.gap-bar-container {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.gap-bar-label {
  display: flex;
  justify-content: space-between;
  font-size: 0.75rem;
  color: #6b7280;
}

.gap-bar {
  height: 8px;
  background: #e5e7eb;
  border-radius: 4px;
  overflow: hidden;
}

.gap-bar-fill {
  height: 100%;
  border-radius: 4px;
  transition: width 0.5s ease;
}

.gap-bar-fill.current {
  background: linear-gradient(90deg, #f59e0b 0%, #f97316 100%);
}

.gap-bar-fill.target {
  background: linear-gradient(90deg, #22c55e 0%, #16a34a 100%);
}

.gap-delta {
  display: flex;
  align-items: center;
  gap: 0.25rem;
  font-size: 0.75rem;
  font-weight: 600;
  margin-top: 0.5rem;
}

.recommended-focus {
  background: linear-gradient(135deg, #f0fdfa 0%, #ecfeff 100%);
  border-radius: 12px;
  padding: 1rem;
}

.recommended-focus h4 {
  font-size: 0.875rem;
  font-weight: 600;
  color: #0f766e;
  margin-bottom: 0.75rem;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.focus-items {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
  margin-bottom: 0.75rem;
}

.focus-item {
  padding: 0.375rem 0.75rem;
  background: white;
  border: 1px solid #99f6e4;
  border-radius: 20px;
  font-size: 0.8rem;
  font-weight: 500;
  color: #0f766e;
}

.time-estimate {
  font-size: 0.8rem;
  color: #4b5563;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

/* AI Learning Path Modal */
.ai-paths-container {
  display: grid;
  grid-template-columns: 300px 1fr;
  gap: 1.5rem;
  min-height: 400px;
}

.ai-paths-list {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.ai-path-card {
  padding: 1rem;
  background: #f9fafb;
  border: 2px solid transparent;
  border-radius: 12px;
  cursor: pointer;
  transition: all 0.2s;
}

.ai-path-card:hover {
  background: #f0fdfa;
  border-color: #99f6e4;
}

.ai-path-card.selected {
  background: linear-gradient(135deg, #f0fdfa 0%, #ecfeff 100%);
  border-color: #14b8a6;
}

.path-confidence {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-bottom: 0.5rem;
}

.path-title {
  font-size: 1rem;
  font-weight: 700;
  color: #1f2937;
  margin-bottom: 0.25rem;
}

.path-description {
  font-size: 0.75rem;
  color: #6b7280;
  line-height: 1.4;
  margin-bottom: 0.5rem;
}

.path-meta {
  display: flex;
  gap: 1rem;
  font-size: 0.7rem;
  color: #9ca3af;
}

.path-meta span {
  display: flex;
  align-items: center;
  gap: 0.25rem;
}

.ai-path-details {
  background: #f9fafb;
  border-radius: 12px;
  padding: 1.5rem;
  overflow-y: auto;
}

.path-details-header {
  margin-bottom: 1.5rem;
}

.path-details-header h3 {
  font-size: 1.25rem;
  font-weight: 700;
  color: #1f2937;
  margin-bottom: 0.5rem;
}

.path-goal {
  display: inline-flex;
  align-items: center;
  gap: 0.375rem;
  padding: 0.375rem 0.75rem;
  background: #f0fdf4;
  color: #166534;
  border-radius: 20px;
  font-size: 0.8rem;
  font-weight: 500;
}

.path-steps {
  margin-bottom: 1.5rem;
}

.path-step {
  position: relative;
  display: flex;
  gap: 1rem;
  padding-bottom: 1.5rem;
}

.step-number {
  width: 32px;
  height: 32px;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.875rem;
  font-weight: 700;
  flex-shrink: 0;
  z-index: 1;
}

.step-connector {
  position: absolute;
  left: 15px;
  top: 32px;
  width: 2px;
  height: calc(100% - 32px);
  background: #d1d5db;
}

.step-content {
  flex: 1;
  background: white;
  border-radius: 10px;
  padding: 1rem;
  border: 1px solid #e5e7eb;
}

.step-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 0.5rem;
}

.step-header h4 {
  font-size: 0.9rem;
  font-weight: 600;
  color: #1f2937;
}

.step-duration {
  font-size: 0.75rem;
  color: #6b7280;
  background: #f3f4f6;
  padding: 0.25rem 0.5rem;
  border-radius: 4px;
}

.step-skills {
  display: flex;
  flex-wrap: wrap;
  gap: 0.375rem;
  margin-bottom: 0.5rem;
}

.step-skill {
  padding: 0.25rem 0.5rem;
  background: #e0f2fe;
  color: #0369a1;
  border-radius: 4px;
  font-size: 0.7rem;
  font-weight: 500;
}

.step-milestone {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  font-size: 0.75rem;
  color: #78350f;
}

.path-outcomes {
  background: white;
  border-radius: 10px;
  padding: 1rem;
  margin-bottom: 1rem;
}

.path-outcomes h4 {
  font-size: 0.875rem;
  font-weight: 600;
  color: #166534;
  margin-bottom: 0.5rem;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.path-outcomes ul {
  list-style: none;
  padding: 0;
  margin: 0;
}

.path-outcomes li {
  position: relative;
  padding-left: 1rem;
  font-size: 0.8rem;
  color: #4b5563;
  margin-bottom: 0.375rem;
}

.path-outcomes li::before {
  content: '•';
  position: absolute;
  left: 0;
  color: #22c55e;
}

.path-impact {
  display: flex;
  align-items: flex-start;
  gap: 0.75rem;
  padding: 1rem;
  background: linear-gradient(135deg, #faf5ff 0%, #f5f3ff 100%);
  border-radius: 10px;
  border: 1px solid #e9d5ff;
}

.path-impact p {
  font-size: 0.85rem;
  color: #6b21a8;
  line-height: 1.4;
}

/* Slide Transition */
.slide-enter-active,
.slide-leave-active {
  transition: all 0.3s ease;
}

.slide-enter-from,
.slide-leave-to {
  opacity: 0;
  transform: translateY(-10px);
}

/* Responsive */
@media (max-width: 768px) {
  .skill-overview {
    grid-template-columns: 1fr;
  }

  .ai-paths-container {
    grid-template-columns: 1fr;
  }

  .ai-insights-content {
    flex-direction: column;
  }

  .ai-insight-card {
    min-width: auto;
  }

  .ai-recommendations-grid {
    grid-template-columns: 1fr;
  }
}
</style>
