<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useAIServicesStore } from '@/stores/aiServices'

const { t } = useI18n()
import { AILoadingIndicator, AIConfidenceBar, AISuggestionChip } from '@/components/ai'
import {
  CommentsSection,
  RatingStars,
  RelatedContentCarousel,
  BookmarkButton,
  SocialShareButtons
} from '@/components/common'
import { useComments } from '@/composables/useComments'
import { useRatings } from '@/composables/useRatings'
import type { SummarizationResult } from '@/types/ai'

const router = useRouter()
const route = useRoute()
const aiStore = useAIServicesStore()

// Comments / Discussion
const courseIdStr = computed(() => route.params.id as string || '1')
const {
  comments: discussionComments,
  isLoading: discussionLoading,
  loadComments: loadDiscussion,
  addComment: addDiscussionPost
} = useComments('course', courseIdStr.value)

// Course Ratings
const { rating: courseRating, submitRating, loadRating } = useRatings('course', courseIdStr.value)

// Active sidebar tab
const activeSidebarTab = ref<'ai' | 'discussion' | 'certificate'>('ai')

// Certificate state
const showCertificatePreview = ref(false)

// Handle rating
async function handleCourseRating(stars: number) {
  await submitRating(stars)
}

// ============================================================================
// Course Data
// ============================================================================

interface Lesson {
  id: number
  title: string
  duration: string
  completed: boolean
  current?: boolean
  type: 'video' | 'reading' | 'quiz' | 'assignment'
  content?: string
  videoUrl?: string
}

interface Course {
  id: number
  title: string
  description: string
  instructor: string
  instructorInitials: string
  instructorBio: string
  level: string
  levelClass: string
  duration: string
  rating: number
  students: number
  progress: number
  completedLessons: number
  totalLessons: number
  image: string
  tags: string[]
  objectives: string[]
  syllabus: Lesson[]
}

const courseId = computed(() => Number(route.params.id) || 1)

// Mock course data
const courses: Record<number, Course> = {
  1: {
    id: 1,
    title: 'Advanced Data Analytics',
    description: 'Master advanced data analytics techniques including statistical modeling, predictive analytics, and business intelligence. Learn to transform raw data into actionable insights that drive business decisions.',
    instructor: 'Dr. James Wilson',
    instructorInitials: 'JW',
    instructorBio: 'PhD in Data Science from MIT, 15+ years in analytics industry',
    level: 'Advanced',
    levelClass: 'advanced',
    duration: '8 hours',
    rating: 4.8,
    students: 1250,
    progress: 75,
    completedLessons: 9,
    totalLessons: 12,
    image: 'https://images.unsplash.com/photo-1551288049-bebda4e38f71?w=800&h=400&fit=crop',
    tags: ['Data Science', 'Analytics', 'Machine Learning'],
    objectives: [
      'Understand advanced statistical modeling techniques',
      'Apply predictive analytics to real-world scenarios',
      'Build interactive dashboards and visualizations',
      'Master data pipeline architecture concepts'
    ],
    syllabus: [
      { id: 1, title: 'Introduction to Data Analytics', duration: '30 min', completed: true, type: 'video', content: 'Data analytics is the science of analyzing raw data to make conclusions about that information. It involves applying algorithmic or mechanical processes to derive insights.' },
      { id: 2, title: 'Data Collection Methods', duration: '45 min', completed: true, type: 'video', content: 'Learn various methods of collecting data including surveys, web scraping, APIs, and database queries. Understanding data sources is crucial for effective analytics.' },
      { id: 3, title: 'Statistical Analysis Basics', duration: '50 min', completed: true, type: 'reading', content: 'Statistical analysis is a component of data analytics that involves collecting and scrutinizing every data sample in a set of items from which samples can be drawn.' },
      { id: 4, title: 'Data Visualization Techniques', duration: '40 min', completed: true, type: 'video', content: 'Data visualization is the graphical representation of information and data using visual elements like charts, graphs, and maps.' },
      { id: 5, title: 'Advanced Statistical Models', duration: '55 min', completed: true, type: 'video', content: 'Explore advanced statistical models including regression analysis, time series analysis, and multivariate statistics for complex data interpretation.' },
      { id: 6, title: 'Predictive Analytics', duration: '45 min', completed: true, type: 'video', content: 'Predictive analytics uses statistical algorithms and machine learning techniques to identify the likelihood of future outcomes based on historical data.' },
      { id: 7, title: 'Machine Learning Integration', duration: '50 min', completed: true, type: 'video', content: 'Learn how to integrate machine learning models into your analytics workflow for automated pattern recognition and prediction.' },
      { id: 8, title: 'Real-time Data Processing', duration: '40 min', completed: true, type: 'video', content: 'Real-time data processing involves the continual input, processing, and output of data, enabling immediate decision-making based on current information.' },
      { id: 9, title: 'Data Pipeline Architecture', duration: '45 min', completed: true, type: 'video', content: 'Data pipeline architecture encompasses the design and implementation of systems that move data from source to destination for storage, processing, and analysis.' },
      { id: 10, title: 'Business Intelligence Tools', duration: '35 min', completed: false, current: true, type: 'video', content: 'Business intelligence tools are software applications designed to retrieve, analyze, transform, and report data for business intelligence purposes.' },
      { id: 11, title: 'Dashboard Development', duration: '40 min', completed: false, type: 'assignment', content: 'Build an interactive dashboard using the skills learned throughout this course. Present key business metrics in a visually compelling format.' },
      { id: 12, title: 'Final Project & Assessment', duration: '60 min', completed: false, type: 'quiz', content: 'Complete a comprehensive project demonstrating your data analytics skills, followed by a final assessment to test your knowledge.' }
    ]
  },
  2: {
    id: 2,
    title: 'Leadership Essentials',
    description: 'Develop essential leadership skills to inspire and guide teams effectively. Learn communication strategies, conflict resolution, and how to drive organizational success through strong leadership.',
    instructor: 'Maria Garcia',
    instructorInitials: 'MG',
    instructorBio: 'Executive Coach, Former VP at Fortune 500 company',
    level: 'Intermediate',
    levelClass: 'intermediate',
    duration: '6 hours',
    rating: 4.6,
    students: 2340,
    progress: 45,
    completedLessons: 5,
    totalLessons: 11,
    image: 'https://images.unsplash.com/photo-1519389950473-47ba0277781c?w=800&h=400&fit=crop',
    tags: ['Leadership', 'Management', 'Soft Skills'],
    objectives: [
      'Develop your leadership style and presence',
      'Master effective communication strategies',
      'Learn conflict resolution techniques',
      'Build and motivate high-performing teams'
    ],
    syllabus: [
      { id: 1, title: 'What Makes a Great Leader', duration: '25 min', completed: true, type: 'video', content: 'Explore the key characteristics and traits that define great leaders. Understand the difference between management and leadership.' },
      { id: 2, title: 'Communication Strategies', duration: '35 min', completed: true, type: 'video', content: 'Effective communication is the cornerstone of leadership. Learn how to articulate vision, provide feedback, and inspire action.' },
      { id: 3, title: 'Building Trust & Credibility', duration: '30 min', completed: true, type: 'video', content: 'Trust is the foundation of leadership. Discover how to build and maintain credibility with your team and stakeholders.' },
      { id: 4, title: 'Delegation & Empowerment', duration: '40 min', completed: true, type: 'video', content: 'Learn the art of delegation and how to empower team members to take ownership of their work.' },
      { id: 5, title: 'Conflict Resolution', duration: '35 min', completed: true, type: 'video', content: 'Conflict is inevitable in any organization. Develop skills to address and resolve conflicts constructively.' },
      { id: 6, title: 'Motivating Your Team', duration: '30 min', completed: false, current: true, type: 'video', content: 'Understanding what motivates people is crucial for leadership. Learn different motivation theories and their practical applications.' },
      { id: 7, title: 'Decision Making Under Pressure', duration: '40 min', completed: false, type: 'video', content: 'Leaders must make tough decisions. Learn frameworks for making sound decisions even under pressure.' },
      { id: 8, title: 'Leading Remote Teams', duration: '35 min', completed: false, type: 'video', content: 'Remote work is here to stay. Master the unique challenges and opportunities of leading distributed teams.' },
      { id: 9, title: 'Change Management', duration: '45 min', completed: false, type: 'video', content: 'Organizations constantly evolve. Learn how to lead through change and help your team adapt.' },
      { id: 10, title: 'Coaching & Mentoring', duration: '30 min', completed: false, type: 'reading', content: 'Great leaders develop other leaders. Discover effective coaching and mentoring techniques.' },
      { id: 11, title: 'Leadership Assessment', duration: '40 min', completed: false, type: 'quiz', content: 'Complete a comprehensive assessment of your leadership skills and receive personalized recommendations.' }
    ]
  }
}

const course = computed(() => courses[courseId.value] || courses[1])
const currentLesson = ref<Lesson | null>(null)
const selectedLessonIndex = ref(0)

// Set initial lesson
onMounted(async () => {
  const current = course.value.syllabus.find(l => l.current)
  if (current) {
    currentLesson.value = current
    selectedLessonIndex.value = course.value.syllabus.findIndex(l => l.id === current.id)
  } else {
    currentLesson.value = course.value.syllabus[0]
    selectedLessonIndex.value = 0
  }

  // Load discussion and ratings
  await Promise.all([
    loadDiscussion(),
    loadRating()
  ])
})

// ============================================================================
// AI Features - Summarization, Key Concepts, Quiz Generation
// ============================================================================

// AI State
const isGeneratingSummary = ref(false)
const isExtractingConcepts = ref(false)
const isGeneratingQuiz = ref(false)
const showSyllabus = ref(true)
const showAISidebar = ref(true)
const activeAITab = ref<'summary' | 'concepts' | 'quiz' | 'notes'>('summary')
const isVideoFullscreen = ref(false)
const videoContainerRef = ref<HTMLElement | null>(null)

// AI Results
interface CourseSummary {
  brief: string
  keyPoints: string[]
  difficulty: string
  prerequisite: string[]
}

interface KeyConcept {
  term: string
  definition: string
  importance: 'high' | 'medium' | 'low'
  relatedLessons: number[]
}

interface QuizQuestion {
  id: number
  question: string
  options: string[]
  correctAnswer: number
  explanation: string
}

interface AIQuiz {
  title: string
  questions: QuizQuestion[]
  passingScore: number
}

const courseSummary = ref<CourseSummary | null>(null)
const keyConcepts = ref<KeyConcept[]>([])
const aiQuiz = ref<AIQuiz | null>(null)
const userQuizAnswers = ref<Record<number, number>>({})
const showQuizResults = ref(false)
const aiNotes = ref<string>('')

// Mock AI Summary Data
const mockSummaries: Record<number, CourseSummary> = {
  1: {
    brief: 'This comprehensive course covers advanced data analytics from statistical foundations to real-world application. You will learn to extract meaningful insights from complex datasets using modern tools and methodologies.',
    keyPoints: [
      'Master statistical modeling and predictive analytics techniques',
      'Build end-to-end data pipelines for enterprise applications',
      'Create compelling visualizations and interactive dashboards',
      'Apply machine learning for automated pattern recognition',
      'Understand real-time data processing architectures'
    ],
    difficulty: 'Challenging - requires foundational statistics knowledge',
    prerequisite: ['Basic Statistics', 'SQL Fundamentals', 'Python or R Programming']
  },
  2: {
    brief: 'A transformative leadership program designed to develop essential skills for modern leaders. Learn to inspire teams, navigate challenges, and drive organizational success through effective leadership practices.',
    keyPoints: [
      'Develop authentic leadership presence and style',
      'Master communication for influence and inspiration',
      'Build high-trust relationships with teams and stakeholders',
      'Navigate conflict and make decisions under pressure',
      'Lead effectively in remote and hybrid environments'
    ],
    difficulty: 'Moderate - suitable for emerging and experienced leaders',
    prerequisite: ['Management Experience', 'Team Collaboration Skills']
  }
}

// Mock Key Concepts Data
const mockConcepts: Record<number, KeyConcept[]> = {
  1: [
    { term: 'Predictive Analytics', definition: 'Using statistical algorithms and ML techniques to identify future outcomes based on historical data.', importance: 'high', relatedLessons: [6, 7] },
    { term: 'Data Pipeline', definition: 'A series of data processing steps that move data from source to destination for analysis.', importance: 'high', relatedLessons: [9] },
    { term: 'Business Intelligence', definition: 'Technologies and strategies for analyzing business data to support decision-making.', importance: 'high', relatedLessons: [10, 11] },
    { term: 'Statistical Modeling', definition: 'Mathematical representation of real-world processes using statistical methods.', importance: 'medium', relatedLessons: [3, 5] },
    { term: 'Data Visualization', definition: 'Graphical representation of information to communicate insights effectively.', importance: 'medium', relatedLessons: [4, 11] },
    { term: 'Real-time Processing', definition: 'Continuous data processing that enables immediate insights and actions.', importance: 'medium', relatedLessons: [8] }
  ],
  2: [
    { term: 'Servant Leadership', definition: 'Leadership philosophy where the leader\'s main goal is to serve their team.', importance: 'high', relatedLessons: [1, 4] },
    { term: 'Emotional Intelligence', definition: 'The ability to understand and manage your own emotions and those of others.', importance: 'high', relatedLessons: [2, 5, 6] },
    { term: 'Psychological Safety', definition: 'Team environment where members feel safe to take risks and be vulnerable.', importance: 'high', relatedLessons: [3, 5] },
    { term: 'Delegation', definition: 'Assigning responsibility and authority to team members for specific tasks.', importance: 'medium', relatedLessons: [4] },
    { term: 'Change Management', definition: 'Structured approach to transitioning individuals and organizations to a desired future state.', importance: 'medium', relatedLessons: [9] },
    { term: 'Active Listening', definition: 'Fully concentrating on what is being said rather than just passively hearing.', importance: 'medium', relatedLessons: [2, 10] }
  ]
}

// Mock Quiz Data
const mockQuizzes: Record<number, AIQuiz> = {
  1: {
    title: 'Data Analytics Knowledge Check',
    passingScore: 70,
    questions: [
      {
        id: 1,
        question: 'What is the primary purpose of predictive analytics?',
        options: [
          'To clean and organize historical data',
          'To identify future outcomes based on historical patterns',
          'To create visualizations for stakeholders',
          'To store data in a structured format'
        ],
        correctAnswer: 1,
        explanation: 'Predictive analytics uses statistical algorithms and machine learning to identify the likelihood of future outcomes based on historical data.'
      },
      {
        id: 2,
        question: 'Which of the following is NOT a component of a data pipeline?',
        options: [
          'Data extraction from sources',
          'Data transformation and cleaning',
          'User interface design',
          'Data loading to destination'
        ],
        correctAnswer: 2,
        explanation: 'A data pipeline consists of extraction, transformation, and loading (ETL) processes. UI design is not part of the data pipeline architecture.'
      },
      {
        id: 3,
        question: 'What distinguishes real-time data processing from batch processing?',
        options: [
          'Real-time uses more storage',
          'Real-time processes data continuously as it arrives',
          'Batch processing is faster',
          'Real-time only works with structured data'
        ],
        correctAnswer: 1,
        explanation: 'Real-time processing handles data continuously as it arrives, enabling immediate insights, while batch processing handles data in groups at scheduled intervals.'
      },
      {
        id: 4,
        question: 'Which visualization would be most appropriate for showing trends over time?',
        options: [
          'Pie chart',
          'Bar chart',
          'Line chart',
          'Scatter plot'
        ],
        correctAnswer: 2,
        explanation: 'Line charts are ideal for displaying trends over time as they clearly show the progression and direction of data points across a timeline.'
      },
      {
        id: 5,
        question: 'What is the role of Business Intelligence (BI) tools?',
        options: [
          'To write code for data scientists',
          'To analyze and report data for decision-making',
          'To replace human analysts',
          'To generate random data for testing'
        ],
        correctAnswer: 1,
        explanation: 'BI tools are designed to retrieve, analyze, transform, and report data to support informed business decision-making.'
      }
    ]
  },
  2: {
    title: 'Leadership Essentials Quiz',
    passingScore: 70,
    questions: [
      {
        id: 1,
        question: 'What is the foundation of effective leadership according to trust-based leadership theory?',
        options: [
          'Technical expertise',
          'Authority and control',
          'Trust and credibility',
          'Financial incentives'
        ],
        correctAnswer: 2,
        explanation: 'Trust is the foundation of leadership. Without trust, a leader cannot effectively influence or inspire their team.'
      },
      {
        id: 2,
        question: 'Which approach is most effective for resolving workplace conflicts?',
        options: [
          'Avoiding the conflict until it resolves itself',
          'Using authority to impose a solution',
          'Addressing issues directly with empathy and fairness',
          'Letting team members figure it out themselves'
        ],
        correctAnswer: 2,
        explanation: 'Effective conflict resolution involves addressing issues directly while showing empathy and ensuring fair treatment of all parties involved.'
      },
      {
        id: 3,
        question: 'What is psychological safety in a team context?',
        options: [
          'Physical workplace safety measures',
          'Mental health support programs',
          'Environment where members feel safe to take risks',
          'Insurance policies for employees'
        ],
        correctAnswer: 2,
        explanation: 'Psychological safety refers to a team environment where members feel safe to take interpersonal risks, share ideas, and be vulnerable without fear of negative consequences.'
      },
      {
        id: 4,
        question: 'What is the key benefit of effective delegation?',
        options: [
          'Leaders have less work to do',
          'Team members develop skills and take ownership',
          'Projects complete faster automatically',
          'Reduces the need for communication'
        ],
        correctAnswer: 1,
        explanation: 'Effective delegation empowers team members by giving them opportunities to develop skills and take ownership of their work, building a stronger team overall.'
      },
      {
        id: 5,
        question: 'Which leadership style is characterized by prioritizing the needs of team members?',
        options: [
          'Autocratic leadership',
          'Transactional leadership',
          'Servant leadership',
          'Laissez-faire leadership'
        ],
        correctAnswer: 2,
        explanation: 'Servant leadership is a philosophy where the leader\'s primary goal is to serve their team, focusing on their growth, well-being, and success.'
      }
    ]
  }
}

// AI Functions
async function generateCourseSummary() {
  isGeneratingSummary.value = true

  // Simulate AI processing
  await new Promise(resolve => setTimeout(resolve, 1500))

  courseSummary.value = mockSummaries[courseId.value] || mockSummaries[1]
  isGeneratingSummary.value = false
}

async function extractKeyConcepts() {
  isExtractingConcepts.value = true

  // Simulate AI processing
  await new Promise(resolve => setTimeout(resolve, 1200))

  keyConcepts.value = mockConcepts[courseId.value] || mockConcepts[1]
  isExtractingConcepts.value = false
}

async function generateAIQuiz() {
  isGeneratingQuiz.value = true
  userQuizAnswers.value = {}
  showQuizResults.value = false

  // Simulate AI processing
  await new Promise(resolve => setTimeout(resolve, 1800))

  aiQuiz.value = mockQuizzes[courseId.value] || mockQuizzes[1]
  isGeneratingQuiz.value = false
}

function selectQuizAnswer(questionId: number, answerIndex: number) {
  userQuizAnswers.value[questionId] = answerIndex
}

function submitQuiz() {
  showQuizResults.value = true
}

const quizScore = computed(() => {
  if (!aiQuiz.value || !showQuizResults.value) return 0
  let correct = 0
  aiQuiz.value.questions.forEach(q => {
    if (userQuizAnswers.value[q.id] === q.correctAnswer) {
      correct++
    }
  })
  return Math.round((correct / aiQuiz.value.questions.length) * 100)
})

const quizPassed = computed(() => {
  if (!aiQuiz.value) return false
  return quizScore.value >= aiQuiz.value.passingScore
})

function retakeQuiz() {
  userQuizAnswers.value = {}
  showQuizResults.value = false
}

function getImportanceColor(importance: string) {
  switch (importance) {
    case 'high': return 'bg-red-100 text-red-700 border-red-200'
    case 'medium': return 'bg-amber-100 text-amber-700 border-amber-200'
    case 'low': return 'bg-blue-100 text-blue-700 border-blue-200'
    default: return 'bg-gray-100 text-gray-700'
  }
}

// Initialize AI features on mount
onMounted(() => {
  generateCourseSummary()
  extractKeyConcepts()
})

// ============================================================================
// Navigation and Progress
// ============================================================================

function goBack() {
  router.push({ name: 'Learning' })
}

function selectLesson(lesson: Lesson, index: number) {
  currentLesson.value = lesson
  selectedLessonIndex.value = index
}

function markLessonComplete() {
  if (currentLesson.value) {
    const index = course.value.syllabus.findIndex(l => l.id === currentLesson.value!.id)
    if (index !== -1) {
      course.value.syllabus[index].completed = true
      // Move to next lesson
      if (index < course.value.syllabus.length - 1) {
        selectLesson(course.value.syllabus[index + 1], index + 1)
      }
    }
  }
}

function nextLesson() {
  if (selectedLessonIndex.value < course.value.syllabus.length - 1) {
    selectedLessonIndex.value++
    currentLesson.value = course.value.syllabus[selectedLessonIndex.value]
  }
}

function prevLesson() {
  if (selectedLessonIndex.value > 0) {
    selectedLessonIndex.value--
    currentLesson.value = course.value.syllabus[selectedLessonIndex.value]
  }
}

function toggleVideoFullscreen() {
  isVideoFullscreen.value = !isVideoFullscreen.value

  if (isVideoFullscreen.value) {
    // Add escape key listener
    document.addEventListener('keydown', handleEscapeKey)
  } else {
    document.removeEventListener('keydown', handleEscapeKey)
  }
}

function handleEscapeKey(e: KeyboardEvent) {
  if (e.key === 'Escape' && isVideoFullscreen.value) {
    isVideoFullscreen.value = false
    document.removeEventListener('keydown', handleEscapeKey)
  }
}

function getLessonIcon(type: string) {
  switch (type) {
    case 'video': return 'fas fa-play-circle'
    case 'reading': return 'fas fa-book-open'
    case 'quiz': return 'fas fa-question-circle'
    case 'assignment': return 'fas fa-tasks'
    default: return 'fas fa-file'
  }
}

function getLevelBadgeClass(level: string) {
  switch (level.toLowerCase()) {
    case 'beginner': return 'bg-green-500 text-white'
    case 'intermediate': return 'bg-amber-500 text-white'
    case 'advanced': return 'bg-red-500 text-white'
    default: return 'bg-teal-500 text-white'
  }
}

// ============================================================================
// Resources & Materials
// ============================================================================

interface CourseResource {
  id: number
  name: string
  type: 'pdf' | 'xlsx' | 'pptx' | 'zip' | 'doc'
  size: string
  url: string
}

const courseResources = ref<CourseResource[]>([
  { id: 1, name: 'Course Slides - Complete Pack', type: 'pptx', size: '12.5 MB', url: '#' },
  { id: 2, name: 'Practice Exercises & Solutions', type: 'pdf', size: '3.2 MB', url: '#' },
  { id: 3, name: 'Sample Datasets', type: 'zip', size: '45.8 MB', url: '#' },
  { id: 4, name: 'Quick Reference Guide', type: 'pdf', size: '1.1 MB', url: '#' },
  { id: 5, name: 'Cheat Sheet - Key Formulas', type: 'xlsx', size: '856 KB', url: '#' }
])

function getResourceIcon(type: string) {
  switch (type) {
    case 'pdf': return 'fas fa-file-pdf'
    case 'xlsx': return 'fas fa-file-excel'
    case 'pptx': return 'fas fa-file-powerpoint'
    case 'zip': return 'fas fa-file-archive'
    case 'doc': return 'fas fa-file-word'
    default: return 'fas fa-file'
  }
}

function downloadResource(resource: CourseResource) {
  // Simulate download
  console.log('Downloading:', resource.name)
}

// ============================================================================
// Notes Feature
// ============================================================================

const showNotesEditor = ref(false)
const isSavingNotes = ref(false)

async function saveNotes() {
  if (!aiNotes.value.trim()) return
  isSavingNotes.value = true
  await new Promise(resolve => setTimeout(resolve, 800))
  isSavingNotes.value = false
}

// ============================================================================
// Certificate Preview
// ============================================================================

const courseCompleted = computed(() => course.value.progress === 100)

function openCertificatePreview() {
  showCertificatePreview.value = true
}

function closeCertificatePreview() {
  showCertificatePreview.value = false
}

function downloadCertificate() {
  console.log('Downloading certificate...')
  closeCertificatePreview()
}

// ============================================================================
// Estimated Time Remaining
// ============================================================================

const estimatedTimeRemaining = computed(() => {
  const remainingLessons = course.value.syllabus.filter(l => !l.completed)
  let totalMinutes = 0
  remainingLessons.forEach(lesson => {
    const match = lesson.duration.match(/(\d+)/)
    if (match) {
      totalMinutes += parseInt(match[1])
    }
  })

  if (totalMinutes < 60) {
    return `${totalMinutes} min`
  }
  const hours = Math.floor(totalMinutes / 60)
  const mins = totalMinutes % 60
  return mins > 0 ? `${hours}h ${mins}m` : `${hours}h`
})
</script>

<template>
  <div class="course-view-page min-h-screen bg-gray-50">
    <!-- Hero Section -->
    <header class="relative">
      <div class="h-[180px] w-full bg-gradient-to-br from-teal-600 via-teal-500 to-emerald-500"></div>

      <!-- Header Content -->
      <div class="absolute bottom-0 left-0 right-0 px-6 py-5">
        <div>
          <!-- Navigation Row -->
          <div class="header-nav">
            <button @click="goBack" class="back-btn">
              <i class="fas fa-arrow-left"></i>
              <span>Back</span>
            </button>
            <div class="breadcrumb">
              <router-link to="/learning" class="breadcrumb-link">
                <i class="fas fa-graduation-cap"></i>
                Learning
              </router-link>
              <i class="fas fa-chevron-right breadcrumb-sep"></i>
              <span class="breadcrumb-current">{{ course.title }}</span>
            </div>
          </div>

          <!-- Title -->
          <h1 class="text-2xl md:text-3xl font-bold text-white leading-tight mb-2">
            {{ course.title }}
          </h1>

          <!-- Tags -->
          <div class="flex items-center gap-2 flex-wrap">
            <span :class="['px-2.5 py-0.5 rounded-full text-xs font-medium', getLevelBadgeClass(course.level)]">
              {{ course.level }}
            </span>
            <span v-for="tag in course.tags" :key="tag" class="px-2.5 py-0.5 bg-white/20 backdrop-blur-sm text-white rounded-full text-xs">
              {{ tag }}
            </span>
          </div>
        </div>
      </div>
    </header>

    <!-- Metadata Bar -->
    <div class="bg-white border-b border-gray-200 sticky top-0 z-20">
      <div class="px-6 py-3">
        <div class="flex items-center justify-between flex-wrap gap-4">
          <!-- Instructor & Meta -->
          <div class="flex items-center gap-4">
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 rounded-full bg-gradient-to-br from-teal-400 to-teal-600 flex items-center justify-center text-white font-bold">
                {{ course.instructorInitials }}
              </div>
              <div>
                <p class="font-semibold text-gray-900 text-sm">{{ course.instructor }}</p>
                <p class="text-xs text-gray-500">
                  {{ course.instructorBio }}
                </p>
              </div>
            </div>

            <div class="hidden md:flex items-center gap-4 pl-4 border-l border-gray-200 text-sm text-gray-500">
              <span><i class="fas fa-clock mr-1"></i> {{ course.duration }}</span>
              <span><i class="fas fa-users mr-1"></i> {{ course.students.toLocaleString() }} students</span>
              <span><i class="fas fa-star text-amber-400 mr-1"></i> {{ course.rating }}</span>
              <span v-if="estimatedTimeRemaining !== '0 min'" class="text-amber-600">
                <i class="fas fa-hourglass-half mr-1"></i> {{ estimatedTimeRemaining }} left
              </span>
            </div>
          </div>

          <!-- Actions -->
          <div class="flex items-center gap-2">
            <BookmarkButton
              :content-id="course.id.toString()"
              content-type="course"
              size="sm"
            />
            <button
              v-if="courseCompleted"
              @click="openCertificatePreview"
              class="px-3 py-2 rounded-lg text-sm font-medium bg-amber-100 text-amber-700 hover:bg-amber-200 transition-all"
            >
              <i class="fas fa-certificate mr-1"></i>
              Certificate
            </button>
            <SocialShareButtons
              :title="course.title"
              :description="course.description"
              layout="horizontal"
              size="sm"
            />
          </div>
        </div>

        <!-- Progress Bar -->
        <div class="mt-3 pt-3 border-t border-gray-100">
          <div class="flex items-center justify-between text-sm mb-1.5">
            <span class="text-gray-600">{{ course.completedLessons }} of {{ course.totalLessons }} lessons completed</span>
            <span class="font-semibold text-teal-600">{{ course.progress }}%</span>
          </div>
          <div class="h-2 bg-gray-200 rounded-full overflow-hidden">
            <div
              class="h-full bg-gradient-to-r from-teal-500 to-teal-400 rounded-full transition-all duration-500"
              :style="{ width: course.progress + '%' }"
            ></div>
          </div>
        </div>
      </div>
    </div>

    <!-- Main Content -->
    <div :class="['course-content-wrapper', { 'syllabus-collapsed': !showSyllabus, 'ai-collapsed': !showAISidebar }]">
      <!-- Syllabus Sidebar -->
      <div :class="['syllabus-sidebar', { 'collapsed': !showSyllabus }]">
        <button @click="showSyllabus = !showSyllabus" class="syllabus-toggle">
          <i :class="showSyllabus ? 'fas fa-chevron-left' : 'fas fa-chevron-right'"></i>
        </button>

        <div v-if="showSyllabus" class="syllabus-card bg-white rounded-2xl shadow-sm border border-gray-200 overflow-hidden">
          <div class="syllabus-header">
            <h3 class="font-semibold text-gray-900 flex items-center gap-2">
              <i class="fas fa-list text-teal-500"></i>
              Course Content
            </h3>
            <span class="text-xs text-gray-500 bg-gray-100 px-2 py-0.5 rounded-full">{{ course.syllabus.length }} lessons</span>
          </div>

          <div class="syllabus-list">
            <div
              v-for="(lesson, index) in course.syllabus"
              :key="lesson.id"
              :class="['syllabus-item', { 'active': currentLesson?.id === lesson.id, 'completed': lesson.completed }]"
              @click="selectLesson(lesson, index)"
            >
              <div class="lesson-status">
                <i v-if="lesson.completed" class="fas fa-check-circle text-green-500"></i>
                <span v-else class="lesson-number">{{ index + 1 }}</span>
              </div>
              <div class="lesson-info">
                <h4>{{ lesson.title }}</h4>
                <div class="lesson-meta">
                  <i :class="getLessonIcon(lesson.type)"></i>
                  <span>{{ lesson.duration }}</span>
                </div>
              </div>
              <div v-if="lesson.current" class="current-badge">
                <i class="fas fa-play"></i>
              </div>
            </div>
          </div>
        </div>

        <!-- Collapsed state icon -->
        <div v-if="!showSyllabus" class="collapsed-icon">
          <i class="fas fa-list text-teal-500"></i>
        </div>
      </div>

      <!-- Main Lesson Content -->
      <div class="lesson-content">
        <div v-if="currentLesson" class="lesson-viewer bg-white rounded-2xl shadow-sm border border-gray-200 overflow-hidden">
          <!-- Lesson Header -->
          <div class="lesson-header">
            <div class="lesson-title-section">
              <span class="lesson-type-badge">
                <i :class="getLessonIcon(currentLesson.type)"></i>
                {{ currentLesson.type }}
              </span>
              <h2>{{ currentLesson.title }}</h2>
            </div>
            <div class="lesson-actions">
              <button
                v-if="!currentLesson.completed"
                @click="markLessonComplete"
                class="complete-btn"
              >
                <i class="fas fa-check"></i>
                Mark Complete
              </button>
              <span v-else class="completed-badge">
                <i class="fas fa-check-circle"></i>
                Completed
              </span>
            </div>
          </div>

          <!-- Video/Content Area -->
          <div class="lesson-media">
            <div v-if="currentLesson.type === 'video'" class="video-placeholder">
              <img :src="course.image" :alt="currentLesson.title" class="video-thumbnail" />
              <div class="video-overlay">
                <button class="play-btn">
                  <i class="fas fa-play"></i>
                </button>
              </div>
              <div class="video-duration">{{ currentLesson.duration }}</div>
              <button @click="toggleVideoFullscreen" class="fullscreen-btn">
                <i class="fas fa-expand"></i>
              </button>
            </div>
            <div v-else class="reading-content">
              <i :class="[getLessonIcon(currentLesson.type), 'reading-icon']"></i>
            </div>
          </div>

          <!-- Lesson Description -->
          <div class="lesson-description">
            <h3>About This Lesson</h3>
            <p>{{ currentLesson.content }}</p>
          </div>

          <!-- Navigation -->
          <div class="lesson-navigation">
            <button @click="prevLesson" :disabled="selectedLessonIndex === 0" class="nav-btn prev">
              <i class="fas fa-chevron-left"></i>
              Previous
            </button>
            <button @click="nextLesson" :disabled="selectedLessonIndex === course.syllabus.length - 1" class="nav-btn next">
              Next
              <i class="fas fa-chevron-right"></i>
            </button>
          </div>
        </div>

      </div>

      <!-- AI Sidebar -->
      <div :class="['ai-sidebar', { 'collapsed': !showAISidebar }]">
        <button @click="showAISidebar = !showAISidebar" class="ai-sidebar-toggle">
          <i :class="showAISidebar ? 'fas fa-chevron-right' : 'fas fa-chevron-left'"></i>
        </button>

        <div v-if="showAISidebar" class="ai-sidebar-card bg-white rounded-2xl shadow-sm border border-gray-200 overflow-hidden">
          <div class="ai-sidebar-header bg-gradient-to-r from-teal-500 to-teal-600 text-white px-4 py-3">
            <div class="flex items-center gap-2">
              <i class="fas fa-wand-magic-sparkles"></i>
              <h3 class="font-semibold">AI Assistant</h3>
            </div>
          </div>

          <!-- AI Tabs -->
          <div class="ai-tabs">
            <button
              v-for="tab in ['summary', 'concepts', 'quiz', 'notes']"
              :key="tab"
              :class="['ai-tab', { 'active': activeAITab === tab }]"
              @click="activeAITab = tab as typeof activeAITab"
            >
              <i :class="{
                'fas fa-file-alt': tab === 'summary',
                'fas fa-lightbulb': tab === 'concepts',
                'fas fa-question-circle': tab === 'quiz',
                'fas fa-sticky-note': tab === 'notes'
              }"></i>
              <span>{{ tab.charAt(0).toUpperCase() + tab.slice(1) }}</span>
            </button>
          </div>

          <!-- Summary Tab -->
          <div v-if="activeAITab === 'summary'" class="ai-tab-content">
            <div class="ai-action-header">
              <h4>Course Summary</h4>
              <button @click="generateCourseSummary" :disabled="isGeneratingSummary" class="ai-refresh-btn">
                <i :class="isGeneratingSummary ? 'fas fa-spinner fa-spin' : 'fas fa-sync-alt'"></i>
              </button>
            </div>

            <div v-if="isGeneratingSummary" class="ai-loading">
              <AILoadingIndicator />
              <p>Generating summary...</p>
            </div>

            <div v-else-if="courseSummary" class="summary-content">
              <p class="summary-brief">{{ courseSummary.brief }}</p>

              <div class="summary-section">
                <h5><i class="fas fa-key text-teal-500"></i> Key Points</h5>
                <ul class="key-points">
                  <li v-for="point in courseSummary.keyPoints" :key="point">{{ point }}</li>
                </ul>
              </div>

              <div class="summary-section">
                <h5><i class="fas fa-signal text-amber-500"></i> Difficulty</h5>
                <p class="difficulty-text">{{ courseSummary.difficulty }}</p>
              </div>

              <div class="summary-section">
                <h5><i class="fas fa-clipboard-list text-blue-500"></i> Prerequisites</h5>
                <div class="prereq-chips">
                  <span v-for="prereq in courseSummary.prerequisite" :key="prereq" class="prereq-chip">
                    {{ prereq }}
                  </span>
                </div>
              </div>
            </div>
          </div>

          <!-- Concepts Tab -->
          <div v-if="activeAITab === 'concepts'" class="ai-tab-content">
            <div class="ai-action-header">
              <h4>Key Concepts</h4>
              <button @click="extractKeyConcepts" :disabled="isExtractingConcepts" class="ai-refresh-btn">
                <i :class="isExtractingConcepts ? 'fas fa-spinner fa-spin' : 'fas fa-sync-alt'"></i>
              </button>
            </div>

            <div v-if="isExtractingConcepts" class="ai-loading">
              <AILoadingIndicator />
              <p>Extracting key concepts...</p>
            </div>

            <div v-else class="concepts-list">
              <div
                v-for="concept in keyConcepts"
                :key="concept.term"
                class="concept-card"
              >
                <div class="concept-header">
                  <h5>{{ concept.term }}</h5>
                  <span :class="['importance-badge', getImportanceColor(concept.importance)]">
                    {{ concept.importance }}
                  </span>
                </div>
                <p class="concept-definition">{{ concept.definition }}</p>
                <div class="concept-lessons">
                  <span class="text-xs text-gray-500">Related lessons:</span>
                  <div class="lesson-refs">
                    <span
                      v-for="lessonId in concept.relatedLessons"
                      :key="lessonId"
                      class="lesson-ref"
                      @click="selectLesson(course.syllabus[lessonId - 1], lessonId - 1)"
                    >
                      {{ lessonId }}
                    </span>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Quiz Tab -->
          <div v-if="activeAITab === 'quiz'" class="ai-tab-content">
            <div class="ai-action-header">
              <h4>Knowledge Check</h4>
              <button @click="generateAIQuiz" :disabled="isGeneratingQuiz" class="ai-refresh-btn">
                <i :class="isGeneratingQuiz ? 'fas fa-spinner fa-spin' : 'fas fa-sync-alt'"></i>
              </button>
            </div>

            <div v-if="isGeneratingQuiz" class="ai-loading">
              <AILoadingIndicator />
              <p>Generating quiz questions...</p>
            </div>

            <div v-else-if="!aiQuiz" class="quiz-placeholder">
              <i class="fas fa-question-circle text-4xl text-gray-300 mb-3"></i>
              <p class="text-gray-500">Click refresh to generate AI quiz</p>
            </div>

            <div v-else class="quiz-content">
              <!-- Quiz Results -->
              <div v-if="showQuizResults" class="quiz-results">
                <div :class="['results-header', quizPassed ? 'passed' : 'failed']">
                  <i :class="quizPassed ? 'fas fa-trophy' : 'fas fa-times-circle'"></i>
                  <div>
                    <h4>{{ quizPassed ? 'Congratulations!' : 'Keep Learning!' }}</h4>
                    <p>Your score: {{ quizScore }}%</p>
                  </div>
                </div>
                <button @click="retakeQuiz" class="retake-btn">
                  <i class="fas fa-redo"></i>
                  Retake Quiz
                </button>
              </div>

              <!-- Quiz Questions -->
              <div v-else class="quiz-questions">
                <div v-for="(question, qIndex) in aiQuiz.questions" :key="question.id" class="quiz-question">
                  <h5>{{ qIndex + 1 }}. {{ question.question }}</h5>
                  <div class="quiz-options">
                    <button
                      v-for="(option, oIndex) in question.options"
                      :key="oIndex"
                      :class="['quiz-option', { 'selected': userQuizAnswers[question.id] === oIndex }]"
                      @click="selectQuizAnswer(question.id, oIndex)"
                    >
                      <span class="option-letter">{{ String.fromCharCode(65 + oIndex) }}</span>
                      <span>{{ option }}</span>
                    </button>
                  </div>
                </div>

                <button
                  @click="submitQuiz"
                  :disabled="Object.keys(userQuizAnswers).length < aiQuiz.questions.length"
                  class="submit-quiz-btn"
                >
                  <i class="fas fa-check"></i>
                  Submit Answers
                </button>
              </div>
            </div>
          </div>

          <!-- Notes Tab -->
          <div v-if="activeAITab === 'notes'" class="ai-tab-content">
            <div class="ai-action-header">
              <h4>My Notes</h4>
              <button
                @click="saveNotes"
                :disabled="isSavingNotes || !aiNotes.trim()"
                class="save-notes-btn"
              >
                <i :class="isSavingNotes ? 'fas fa-spinner fa-spin' : 'fas fa-save'"></i>
                {{ isSavingNotes ? 'Saving...' : 'Save' }}
              </button>
            </div>
            <div class="notes-container">
              <div class="notes-lesson-info">
                <i class="fas fa-bookmark text-teal-500"></i>
                <span>{{ currentLesson?.title || 'Select a lesson' }}</span>
              </div>
              <textarea
                v-model="aiNotes"
                class="notes-textarea"
                placeholder="Take notes as you learn... These notes are saved per lesson and will help you review later."
              ></textarea>
              <div class="notes-tips">
                <div class="notes-tip">
                  <i class="fas fa-lightbulb text-amber-500"></i>
                  <span>Tip: Summarize key points in your own words for better retention</span>
                </div>
              </div>
            </div>
          </div>

          <!-- Course Rating Section -->
          <div class="course-rating-section p-4 border-t border-gray-200 bg-gray-50 rounded-b-xl">
            <h4 class="text-sm font-semibold text-gray-700 mb-2">Rate this Course</h4>
            <RatingStars
              :model-value="courseRating?.userRating || 0"
              :average="courseRating?.average"
              :count="courseRating?.count"
              size="md"
              :show-count="true"
              @update:model-value="handleCourseRating"
            />
          </div>
        </div>

        <!-- Collapsed state icon -->
        <div v-if="!showAISidebar" class="collapsed-icon">
          <i class="fas fa-wand-magic-sparkles text-teal-500"></i>
        </div>
      </div>
    </div>

    <!-- Full Width Sections Container -->
    <div class="sections-container">
      <!-- About this Course -->
      <div class="bg-white rounded-2xl shadow-sm border border-gray-200 p-6">
        <h3 class="font-semibold text-gray-900 mb-4 flex items-center gap-2">
          <i class="fas fa-info-circle text-teal-500"></i>
          About this Course
        </h3>
        <p class="text-gray-600 leading-relaxed">{{ course.description }}</p>
      </div>

      <!-- Learning Objectives -->
      <div class="bg-white rounded-2xl shadow-sm border border-gray-200 p-6">
        <h3 class="font-semibold text-gray-900 mb-2 flex items-center gap-2">
          <i class="fas fa-bullseye text-teal-500"></i>
          Learning Objectives
        </h3>
        <p class="text-sm text-gray-500 mb-4">What you'll achieve by completing this course</p>
        <div class="objectives-grid">
          <div v-for="(objective, index) in course.objectives" :key="index" class="objective-item">
            <div class="objective-check">
              <i class="fas fa-check"></i>
            </div>
            <span>{{ objective }}</span>
          </div>
        </div>
      </div>

      <!-- Resources & Materials -->
      <div class="bg-white rounded-2xl shadow-sm border border-gray-200 p-6">
        <div class="flex items-center justify-between mb-4">
          <h3 class="font-semibold text-gray-900 flex items-center gap-2">
            <i class="fas fa-folder-open text-teal-500"></i>
            Resources & Materials
          </h3>
          <span class="text-xs text-gray-500 bg-gray-100 px-2.5 py-1 rounded-full">{{ courseResources.length }} files</span>
        </div>
        <div class="resources-grid">
          <div v-for="resource in courseResources" :key="resource.id" class="resource-item">
            <div class="resource-icon" :class="resource.type">
              <i :class="getResourceIcon(resource.type)"></i>
            </div>
            <div class="resource-info">
              <h4>{{ resource.name }}</h4>
              <span class="resource-meta">{{ resource.size }} • {{ resource.type.toUpperCase() }}</span>
            </div>
            <button class="resource-download-btn" @click="downloadResource(resource)">
              <i class="fas fa-download"></i>
            </button>
          </div>
        </div>
      </div>

      <!-- Comments Section -->
      <div class="bg-white rounded-2xl shadow-sm border border-gray-200 p-6">
        <div class="flex items-center justify-between mb-4">
          <h3 class="font-semibold text-gray-900 flex items-center gap-2">
            <i class="fas fa-comments text-teal-500"></i>
            Discussion
          </h3>
          <span class="text-xs text-gray-500 bg-gray-100 px-2.5 py-1 rounded-full">{{ discussionComments.length }} posts</span>
        </div>
        <CommentsSection
          content-type="course"
          :content-id="course.id.toString()"
          :comments="discussionComments"
          :is-loading="discussionLoading"
          @add-comment="addDiscussionPost"
        />
      </div>
    </div>

    <!-- Video Fullscreen Modal -->
    <Teleport to="body">
      <Transition name="modal">
        <div v-if="isVideoFullscreen" class="video-fullscreen-overlay" @click.self="toggleVideoFullscreen">
          <div class="video-fullscreen-container">
            <!-- Close button -->
            <button @click="toggleVideoFullscreen" class="video-fullscreen-close">
              <i class="fas fa-times"></i>
            </button>

            <!-- Video header -->
            <div class="video-fullscreen-header">
              <div class="video-fullscreen-title">
                <span class="lesson-badge">
                  <i class="fas fa-play-circle"></i>
                  Lesson {{ selectedLessonIndex + 1 }}
                </span>
                <h2>{{ currentLesson?.title }}</h2>
              </div>
            </div>

            <!-- Video player -->
            <div class="video-fullscreen-player">
              <img :src="course.image" :alt="currentLesson?.title" class="video-fullscreen-thumbnail" />
              <div class="video-fullscreen-play-overlay">
                <button class="video-fullscreen-play-btn">
                  <i class="fas fa-play"></i>
                </button>
              </div>
            </div>

            <!-- Video controls -->
            <div class="video-fullscreen-controls">
              <div class="video-progress-bar">
                <div class="video-progress-fill" style="width: 35%"></div>
              </div>
              <div class="video-controls-row">
                <div class="video-controls-left">
                  <button class="video-control-btn">
                    <i class="fas fa-play"></i>
                  </button>
                  <button class="video-control-btn">
                    <i class="fas fa-backward"></i>
                  </button>
                  <button class="video-control-btn">
                    <i class="fas fa-forward"></i>
                  </button>
                  <button class="video-control-btn">
                    <i class="fas fa-volume-up"></i>
                  </button>
                  <span class="video-time">0:00 / {{ currentLesson?.duration }}</span>
                </div>
                <div class="video-controls-right">
                  <button class="video-control-btn">
                    <i class="fas fa-cog"></i>
                  </button>
                  <button class="video-control-btn">
                    <i class="fas fa-closed-captioning"></i>
                  </button>
                  <button @click="toggleVideoFullscreen" class="video-control-btn">
                    <i class="fas fa-compress"></i>
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </Transition>
    </Teleport>

    <!-- Certificate Preview Modal -->
    <Teleport to="body">
      <Transition name="modal">
        <div v-if="showCertificatePreview" class="certificate-modal-overlay" @click.self="closeCertificatePreview">
          <div class="certificate-modal">
            <button class="certificate-modal-close" @click="closeCertificatePreview">
              <i class="fas fa-times"></i>
            </button>

            <div class="certificate-preview">
              <div class="certificate-border">
                <div class="certificate-inner">
                  <div class="certificate-logo">
                    <i class="fas fa-graduation-cap"></i>
                  </div>
                  <h2 class="certificate-title">Certificate of Completion</h2>
                  <p class="certificate-subtitle">This is to certify that</p>
                  <h3 class="certificate-name">{{ course.instructor }}</h3>
                  <p class="certificate-text">has successfully completed the course</p>
                  <h4 class="certificate-course">{{ course.title }}</h4>
                  <div class="certificate-details">
                    <div class="certificate-detail">
                      <i class="fas fa-calendar-alt"></i>
                      <span>{{ new Date().toLocaleDateString('en-US', { year: 'numeric', month: 'long', day: 'numeric' }) }}</span>
                    </div>
                    <div class="certificate-detail">
                      <i class="fas fa-clock"></i>
                      <span>{{ course.duration }} of learning</span>
                    </div>
                    <div class="certificate-detail">
                      <i class="fas fa-book"></i>
                      <span>{{ course.totalLessons }} lessons completed</span>
                    </div>
                  </div>
                  <div class="certificate-badge">
                    <i class="fas fa-award"></i>
                    <span>{{ course.level }}</span>
                  </div>
                </div>
              </div>
            </div>

            <div class="certificate-actions">
              <button @click="downloadCertificate" class="download-certificate-btn">
                <i class="fas fa-download"></i>
                Download Certificate
              </button>
              <button @click="closeCertificatePreview" class="close-certificate-btn">
                Close
              </button>
            </div>
          </div>
        </div>
      </Transition>
    </Teleport>
  </div>
</template>

<style scoped>
.course-view-page {
  animation: fadeIn 0.3s ease;
}

@keyframes fadeIn {
  from { opacity: 0; }
  to { opacity: 1; }
}

/* Navigation */
.header-nav {
  display: flex;
  align-items: center;
  gap: 1rem;
  margin-bottom: 1rem;
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
  backdrop-filter: blur(8px);
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

.breadcrumb-current {
  color: white;
  font-weight: 500;
  max-width: 300px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

/* Content Wrapper */
.course-content-wrapper {
  display: grid;
  grid-template-columns: 300px 1fr 340px;
  gap: 1.5rem;
  width: 100%;
  padding: 1.5rem;
  align-items: start;
  transition: grid-template-columns 0.3s ease;
}

.course-content-wrapper.syllabus-collapsed {
  grid-template-columns: 48px 1fr 340px;
}

.course-content-wrapper.ai-collapsed {
  grid-template-columns: 300px 1fr 48px;
}

.course-content-wrapper.syllabus-collapsed.ai-collapsed {
  grid-template-columns: 48px 1fr 48px;
}

/* Syllabus Sidebar */
.syllabus-sidebar {
  position: relative;
  transition: all 0.3s ease;
}

.syllabus-sidebar.collapsed {
  display: flex;
  flex-direction: column;
  align-items: center;
  background: white;
  border-radius: 1rem;
  border: 1px solid #e5e7eb;
  box-shadow: 0 1px 2px rgba(0,0,0,0.05);
  padding: 1rem 0;
}

.syllabus-card {
  width: 100%;
}

.syllabus-toggle {
  position: absolute;
  right: -12px;
  top: 20px;
  width: 24px;
  height: 48px;
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  color: #6b7280;
  z-index: 10;
  box-shadow: 0 1px 3px rgba(0,0,0,0.1);
}

.syllabus-sidebar.collapsed .syllabus-toggle {
  position: relative;
  right: auto;
  top: auto;
  margin-bottom: 1rem;
}

.syllabus-toggle:hover {
  background: #f9fafb;
  color: #14b8a6;
}

.collapsed-icon {
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.25rem;
}

.syllabus-header {
  padding: 1rem 1.25rem;
  border-bottom: 1px solid #e5e7eb;
  display: flex;
  align-items: center;
  justify-content: space-between;
  position: sticky;
  top: 0;
  background: white;
  z-index: 10;
}

.syllabus-list {
  padding: 0.5rem;
}

.syllabus-item {
  display: flex;
  align-items: flex-start;
  gap: 0.75rem;
  padding: 0.75rem;
  border-radius: 10px;
  cursor: pointer;
  transition: all 0.2s;
  margin-bottom: 0.25rem;
}

.syllabus-item:hover {
  background: #f3f4f6;
}

.syllabus-item.active {
  background: #f0fdfa;
  border: 1px solid #99f6e4;
}

.syllabus-item.completed .lesson-info h4 {
  color: #6b7280;
}

.lesson-status {
  width: 24px;
  height: 24px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.lesson-number {
  width: 24px;
  height: 24px;
  background: #e5e7eb;
  color: #6b7280;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.7rem;
  font-weight: 600;
}

.syllabus-item.active .lesson-number {
  background: #14b8a6;
  color: white;
}

.lesson-info {
  flex: 1;
  min-width: 0;
}

.lesson-info h4 {
  font-size: 0.8rem;
  font-weight: 500;
  color: #1f2937;
  margin-bottom: 0.25rem;
  line-height: 1.3;
}

.lesson-meta {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  font-size: 0.7rem;
  color: #9ca3af;
}

.current-badge {
  width: 20px;
  height: 20px;
  background: #14b8a6;
  color: white;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.5rem;
}

/* Lesson Content */
.lesson-content {
  /* No scroll - height matches content */
}

.lesson-viewer {
  /* Card styling applied via Tailwind classes */
}

.lesson-header {
  padding: 1.25rem 1.5rem;
  border-bottom: 1px solid #e5e7eb;
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.lesson-type-badge {
  display: inline-flex;
  align-items: center;
  gap: 0.375rem;
  padding: 0.25rem 0.75rem;
  background: #f3f4f6;
  color: #6b7280;
  border-radius: 20px;
  font-size: 0.7rem;
  font-weight: 500;
  text-transform: capitalize;
  margin-bottom: 0.5rem;
}

.lesson-title-section h2 {
  font-size: 1.25rem;
  font-weight: 700;
  color: #1f2937;
}

.complete-btn {
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

.complete-btn:hover {
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.3);
}

.completed-badge {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  color: #10b981;
  font-size: 0.8rem;
  font-weight: 600;
}

.lesson-media {
  position: relative;
  aspect-ratio: 16/9;
  background: #1f2937;
}

.video-placeholder {
  position: relative;
  width: 100%;
  height: 100%;
}

.video-thumbnail {
  width: 100%;
  height: 100%;
  object-fit: cover;
  opacity: 0.7;
}

.video-overlay {
  position: absolute;
  inset: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(0, 0, 0, 0.3);
}

.play-btn {
  width: 72px;
  height: 72px;
  background: rgba(255, 255, 255, 0.95);
  color: #14b8a6;
  border: none;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.5rem;
  cursor: pointer;
  transition: all 0.3s;
}

.play-btn:hover {
  transform: scale(1.1);
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.3);
}

.video-duration {
  position: absolute;
  bottom: 1rem;
  right: 1rem;
  padding: 0.25rem 0.75rem;
  background: rgba(0, 0, 0, 0.7);
  color: white;
  border-radius: 4px;
  font-size: 0.75rem;
}

.fullscreen-btn {
  position: absolute;
  top: 1rem;
  right: 1rem;
  width: 40px;
  height: 40px;
  background: rgba(0, 0, 0, 0.6);
  border: none;
  border-radius: 8px;
  color: white;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s;
}

.fullscreen-btn:hover {
  background: rgba(0, 0, 0, 0.8);
  transform: scale(1.1);
}

/* Video Fullscreen Modal */
.video-fullscreen-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.95);
  z-index: 9999;
  display: flex;
  align-items: center;
  justify-content: center;
}

.video-fullscreen-container {
  width: 100%;
  height: 100%;
  display: flex;
  flex-direction: column;
  padding: 1rem;
}

.video-fullscreen-close {
  position: absolute;
  top: 1.5rem;
  right: 1.5rem;
  width: 48px;
  height: 48px;
  background: rgba(255, 255, 255, 0.1);
  border: none;
  border-radius: 50%;
  color: white;
  font-size: 1.25rem;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s;
  z-index: 10;
}

.video-fullscreen-close:hover {
  background: rgba(255, 255, 255, 0.2);
}

.video-fullscreen-header {
  padding: 1rem 2rem;
}

.video-fullscreen-title {
  color: white;
}

.video-fullscreen-title .lesson-badge {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.25rem 0.75rem;
  background: rgba(20, 184, 166, 0.2);
  color: #5eead4;
  border-radius: 20px;
  font-size: 0.875rem;
  margin-bottom: 0.5rem;
}

.video-fullscreen-title h2 {
  font-size: 1.5rem;
  font-weight: 600;
  margin: 0;
}

.video-fullscreen-player {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  position: relative;
  margin: 0 2rem;
  border-radius: 16px;
  overflow: hidden;
  background: #000;
}

.video-fullscreen-thumbnail {
  width: 100%;
  height: 100%;
  object-fit: contain;
}

.video-fullscreen-play-overlay {
  position: absolute;
  inset: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(0, 0, 0, 0.3);
}

.video-fullscreen-play-btn {
  width: 80px;
  height: 80px;
  background: rgba(20, 184, 166, 0.9);
  border: none;
  border-radius: 50%;
  color: white;
  font-size: 2rem;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s;
}

.video-fullscreen-play-btn:hover {
  background: #14b8a6;
  transform: scale(1.1);
}

.video-fullscreen-controls {
  padding: 1rem 2rem 2rem;
}

.video-progress-bar {
  width: 100%;
  height: 6px;
  background: rgba(255, 255, 255, 0.2);
  border-radius: 3px;
  margin-bottom: 1rem;
  cursor: pointer;
}

.video-progress-fill {
  height: 100%;
  background: #14b8a6;
  border-radius: 3px;
  transition: width 0.1s;
}

.video-controls-row {
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.video-controls-left,
.video-controls-right {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.video-control-btn {
  width: 40px;
  height: 40px;
  background: transparent;
  border: none;
  border-radius: 8px;
  color: white;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s;
}

.video-control-btn:hover {
  background: rgba(255, 255, 255, 0.1);
}

.video-time {
  color: rgba(255, 255, 255, 0.8);
  font-size: 0.875rem;
  margin-left: 0.5rem;
}

.reading-content {
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
}

.reading-icon {
  font-size: 4rem;
  color: #6b7280;
}

.lesson-description {
  padding: 1.5rem;
}

.lesson-description h3 {
  font-size: 1rem;
  font-weight: 700;
  color: #1f2937;
  margin-bottom: 0.75rem;
}

.lesson-description p {
  color: #4b5563;
  line-height: 1.7;
  font-size: 0.9rem;
}

.lesson-navigation {
  padding: 1rem 1.5rem;
  border-top: 1px solid #e5e7eb;
  display: flex;
  justify-content: space-between;
}

.nav-btn {
  padding: 0.625rem 1.25rem;
  border: 1px solid #d1d5db;
  background: white;
  color: #4b5563;
  border-radius: 8px;
  font-size: 0.8rem;
  font-weight: 500;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  cursor: pointer;
  transition: all 0.2s;
}

.nav-btn:hover:not(:disabled) {
  background: #f3f4f6;
}

.nav-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.nav-btn.next {
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
  border: none;
}

.nav-btn.next:hover:not(:disabled) {
  background: linear-gradient(135deg, #0d9488 0%, #0f766e 100%);
}

/* AI Sidebar */
.ai-sidebar {
  position: relative;
  transition: all 0.3s ease;
}

.ai-sidebar.collapsed {
  display: flex;
  flex-direction: column;
  align-items: center;
  background: white;
  border-radius: 1rem;
  border: 1px solid #e5e7eb;
  box-shadow: 0 1px 2px rgba(0,0,0,0.05);
  padding: 1rem 0;
}

.ai-sidebar-card {
  width: 100%;
}

.ai-sidebar-toggle {
  position: absolute;
  left: -12px;
  top: 20px;
  width: 24px;
  height: 48px;
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  color: #6b7280;
  z-index: 10;
  box-shadow: 0 1px 3px rgba(0,0,0,0.1);
}

.ai-sidebar.collapsed .ai-sidebar-toggle {
  position: relative;
  left: auto;
  top: auto;
  margin-bottom: 1rem;
}

.ai-sidebar-toggle:hover {
  background: #f9fafb;
  color: #14b8a6;
}

.ai-sidebar-content {
  /* No scroll - height matches content */
}

.ai-sidebar-header {
  /* Styled via Tailwind classes */
}

.ai-sidebar-header h3 {
  font-size: 1rem;
  font-weight: 700;
  color: #1f2937;
}

/* AI Tabs */
.ai-tabs {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 0.25rem;
  margin-bottom: 1rem;
  padding: 1rem;
  border-bottom: 1px solid #e5e7eb;
}

.ai-tab {
  padding: 0.5rem;
  background: #f3f4f6;
  border: none;
  border-radius: 8px;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.25rem;
  cursor: pointer;
  transition: all 0.2s;
  font-size: 0.65rem;
  color: #6b7280;
}

.ai-tab i {
  font-size: 0.9rem;
}

.ai-tab:hover {
  background: #e5e7eb;
}

.ai-tab.active {
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
}

.ai-tab-content {
  animation: fadeIn 0.3s ease;
  padding: 0 1rem 1rem;
}

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(10px); }
  to { opacity: 1; transform: translateY(0); }
}

.ai-action-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 1rem;
}

.ai-action-header h4 {
  font-size: 0.9rem;
  font-weight: 700;
  color: #1f2937;
}

.ai-refresh-btn {
  width: 28px;
  height: 28px;
  background: #f3f4f6;
  border: none;
  border-radius: 6px;
  color: #6b7280;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s;
}

.ai-refresh-btn:hover:not(:disabled) {
  background: #e5e7eb;
  color: #14b8a6;
}

.ai-loading {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 2rem;
  color: #6b7280;
  font-size: 0.8rem;
}

/* Summary Tab */
.summary-content {
  font-size: 0.85rem;
}

.summary-brief {
  color: #4b5563;
  line-height: 1.6;
  margin-bottom: 1.25rem;
  padding: 1rem;
  background: #f9fafb;
  border-radius: 10px;
}

.summary-section {
  margin-bottom: 1rem;
}

.summary-section h5 {
  font-size: 0.8rem;
  font-weight: 600;
  color: #374151;
  margin-bottom: 0.5rem;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.key-points {
  list-style: none;
  padding: 0;
  margin: 0;
}

.key-points li {
  position: relative;
  padding-left: 1rem;
  color: #4b5563;
  margin-bottom: 0.375rem;
  font-size: 0.8rem;
  line-height: 1.4;
}

.key-points li::before {
  content: '•';
  position: absolute;
  left: 0;
  color: #14b8a6;
  font-weight: bold;
}

.difficulty-text {
  color: #92400e;
  background: #fef3c7;
  padding: 0.5rem 0.75rem;
  border-radius: 8px;
  font-size: 0.8rem;
}

.prereq-chips {
  display: flex;
  flex-wrap: wrap;
  gap: 0.375rem;
}

.prereq-chip {
  padding: 0.25rem 0.625rem;
  background: #e0f2fe;
  color: #0369a1;
  border-radius: 20px;
  font-size: 0.7rem;
  font-weight: 500;
}

/* Concepts Tab */
.concepts-list {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.concept-card {
  padding: 0.875rem;
  background: #f9fafb;
  border-radius: 10px;
  border: 1px solid #e5e7eb;
}

.concept-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 0.5rem;
}

.concept-header h5 {
  font-size: 0.85rem;
  font-weight: 700;
  color: #1f2937;
}

.importance-badge {
  padding: 0.125rem 0.5rem;
  border-radius: 20px;
  font-size: 0.6rem;
  font-weight: 600;
  text-transform: uppercase;
  border: 1px solid;
}

.concept-definition {
  font-size: 0.75rem;
  color: #6b7280;
  line-height: 1.5;
  margin-bottom: 0.5rem;
}

.concept-lessons {
  padding-top: 0.5rem;
  border-top: 1px solid #e5e7eb;
}

.lesson-refs {
  display: flex;
  gap: 0.25rem;
  margin-top: 0.25rem;
}

.lesson-ref {
  width: 24px;
  height: 24px;
  background: #14b8a6;
  color: white;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.65rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
}

.lesson-ref:hover {
  transform: scale(1.1);
  background: #0d9488;
}

/* Quiz Tab */
.quiz-placeholder {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 2rem;
  text-align: center;
}

.quiz-content {
  font-size: 0.85rem;
}

.quiz-results {
  text-align: center;
  padding: 1.5rem;
}

.results-header {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.75rem;
  padding: 1rem;
  border-radius: 12px;
  margin-bottom: 1rem;
}

.results-header.passed {
  background: #dcfce7;
  color: #166534;
}

.results-header.failed {
  background: #fef2f2;
  color: #991b1b;
}

.results-header i {
  font-size: 2rem;
}

.results-header h4 {
  font-size: 1rem;
  font-weight: 700;
  margin: 0;
}

.results-header p {
  margin: 0;
  font-size: 0.9rem;
}

.retake-btn {
  padding: 0.625rem 1.25rem;
  background: #f3f4f6;
  border: none;
  border-radius: 8px;
  color: #4b5563;
  font-weight: 600;
  font-size: 0.8rem;
  cursor: pointer;
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  transition: all 0.2s;
}

.retake-btn:hover {
  background: #e5e7eb;
}

.quiz-questions {
  display: flex;
  flex-direction: column;
  gap: 1.25rem;
}

.quiz-question h5 {
  font-size: 0.85rem;
  font-weight: 600;
  color: #1f2937;
  margin-bottom: 0.75rem;
  line-height: 1.4;
}

.quiz-options {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.quiz-option {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.625rem 0.875rem;
  background: #f9fafb;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.2s;
  text-align: left;
  font-size: 0.8rem;
  color: #4b5563;
}

.quiz-option:hover {
  background: #f3f4f6;
  border-color: #d1d5db;
}

.quiz-option.selected {
  background: #f0fdfa;
  border-color: #14b8a6;
  color: #0f766e;
}

.option-letter {
  width: 24px;
  height: 24px;
  background: #e5e7eb;
  color: #6b7280;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.7rem;
  font-weight: 700;
  flex-shrink: 0;
}

.quiz-option.selected .option-letter {
  background: #14b8a6;
  color: white;
}

.submit-quiz-btn {
  width: 100%;
  padding: 0.75rem;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
  border: none;
  border-radius: 8px;
  font-weight: 600;
  font-size: 0.85rem;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  transition: all 0.2s;
  margin-top: 1rem;
}

.submit-quiz-btn:hover:not(:disabled) {
  background: linear-gradient(135deg, #0d9488 0%, #0f766e 100%);
}

.submit-quiz-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

/* Notes Tab */
.notes-textarea {
  width: 100%;
  min-height: 300px;
  padding: 1rem;
  border: 1px solid #e5e7eb;
  border-radius: 10px;
  font-size: 0.85rem;
  line-height: 1.6;
  resize: vertical;
  font-family: inherit;
}

.notes-textarea:focus {
  outline: none;
  border-color: #14b8a6;
  box-shadow: 0 0 0 3px rgba(20, 184, 166, 0.1);
}

/* Responsive */
@media (max-width: 1200px) {
  .course-content-wrapper {
    grid-template-columns: 260px 1fr;
  }

  .ai-sidebar {
    position: fixed;
    right: 0;
    top: 200px;
    bottom: 0;
    width: 320px;
    z-index: 100;
    box-shadow: -4px 0 12px rgba(0, 0, 0, 0.1);
  }

  .ai-sidebar.collapsed {
    width: 48px;
  }
}

@media (max-width: 768px) {
  .course-content-wrapper {
    grid-template-columns: 1fr;
  }

  .syllabus-sidebar {
    display: none;
  }

  .ai-sidebar {
    position: fixed;
    right: 0;
    top: auto;
    bottom: 0;
    left: 0;
    width: 100%;
    height: auto;
    max-height: 60vh;
    border-radius: 16px 16px 0 0;
    border: 1px solid #e5e7eb;
    border-bottom: none;
  }

  .ai-sidebar.collapsed {
    width: 100%;
    height: 48px;
  }

  .ai-sidebar-toggle {
    left: 50%;
    top: 0;
    transform: translateX(-50%);
    width: 48px;
    height: 24px;
    border-radius: 0 0 8px 8px;
  }
}

/* Course Tags */
.course-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
  margin-bottom: 1rem;
}

.course-tag {
  padding: 0.25rem 0.75rem;
  background: rgba(255, 255, 255, 0.2);
  color: white;
  border-radius: 20px;
  font-size: 0.75rem;
  font-weight: 500;
  backdrop-filter: blur(4px);
}

/* Additional Header Badges */
.students-badge,
.time-remaining-badge {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  padding: 0.25rem 0.75rem;
  background: rgba(255, 255, 255, 0.15);
  color: white;
  border-radius: 20px;
  font-size: 0.75rem;
}

.time-remaining-badge {
  background: rgba(251, 191, 36, 0.3);
}

.certificate-btn {
  margin-left: auto;
  padding: 0.5rem 1rem;
  background: rgba(255, 255, 255, 0.2);
  color: white;
  border: 1px solid rgba(255, 255, 255, 0.3);
  border-radius: 8px;
  font-size: 0.8rem;
  font-weight: 600;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  cursor: pointer;
  transition: all 0.2s;
}

.certificate-btn:hover {
  background: rgba(255, 255, 255, 0.3);
}

/* Sections Container */
.sections-container {
  width: 100%;
  padding: 2rem;
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

/* Objectives Grid - Full Width */
.objectives-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 1rem;
}

.objective-item {
  display: flex;
  align-items: flex-start;
  gap: 0.75rem;
  padding: 0.875rem;
  background: #f9fafb;
  border-radius: 10px;
  border: 1px solid #f3f4f6;
}

.objective-check {
  width: 24px;
  height: 24px;
  background: linear-gradient(135deg, #10b981 0%, #059669 100%);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 0.65rem;
  flex-shrink: 0;
}

.objective-item span {
  font-size: 0.85rem;
  color: #374151;
  line-height: 1.5;
}

/* Resources Grid - Full Width */
.resources-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
  gap: 1rem;
}

.resource-item {
  display: flex;
  align-items: center;
  gap: 0.875rem;
  padding: 0.875rem;
  background: #f9fafb;
  border-radius: 10px;
  border: 1px solid #f3f4f6;
  transition: all 0.2s;
}

.resource-item:hover {
  background: #f3f4f6;
  border-color: #e5e7eb;
}

.resource-icon {
  width: 40px;
  height: 40px;
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1rem;
  flex-shrink: 0;
}

.resource-icon.pdf {
  background: #fef2f2;
  color: #dc2626;
}

.resource-icon.xlsx {
  background: #ecfdf5;
  color: #059669;
}

.resource-icon.pptx {
  background: #fef3c7;
  color: #d97706;
}

.resource-icon.zip {
  background: #f0f9ff;
  color: #0284c7;
}

.resource-icon.doc {
  background: #eff6ff;
  color: #2563eb;
}

.resource-info {
  flex: 1;
  min-width: 0;
}

.resource-info h4 {
  font-size: 0.85rem;
  font-weight: 600;
  color: #1f2937;
  margin: 0;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.resource-meta {
  font-size: 0.75rem;
  color: #6b7280;
}

.resource-download-btn {
  width: 36px;
  height: 36px;
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  color: #6b7280;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s;
}

.resource-download-btn:hover {
  background: #14b8a6;
  border-color: #14b8a6;
  color: white;
}

/* Notes Tab Enhancements */
.save-notes-btn {
  padding: 0.375rem 0.75rem;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
  border: none;
  border-radius: 6px;
  font-size: 0.75rem;
  font-weight: 600;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 0.375rem;
  transition: all 0.2s;
}

.save-notes-btn:hover:not(:disabled) {
  background: linear-gradient(135deg, #0d9488 0%, #0f766e 100%);
}

.save-notes-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.notes-container {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.notes-lesson-info {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.625rem 0.875rem;
  background: #f0fdfa;
  border-radius: 8px;
  font-size: 0.8rem;
  color: #0f766e;
  font-weight: 500;
}

.notes-tips {
  margin-top: 0.5rem;
}

.notes-tip {
  display: flex;
  align-items: flex-start;
  gap: 0.5rem;
  padding: 0.625rem;
  background: #fffbeb;
  border-radius: 8px;
  font-size: 0.75rem;
  color: #92400e;
}

/* AI Tabs Grid - 5 columns for new notes tab */
.ai-tabs {
  display: grid;
  grid-template-columns: repeat(5, 1fr);
  gap: 0.25rem;
  margin-bottom: 1rem;
}

/* Certificate Modal */
.certificate-modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.7);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  padding: 2rem;
}

.certificate-modal {
  background: white;
  border-radius: 20px;
  max-width: 700px;
  width: 100%;
  max-height: 90vh;
  overflow-y: auto;
  position: relative;
}

.certificate-modal-close {
  position: absolute;
  top: 1rem;
  right: 1rem;
  width: 36px;
  height: 36px;
  background: #f3f4f6;
  border: none;
  border-radius: 50%;
  color: #6b7280;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s;
  z-index: 10;
}

.certificate-modal-close:hover {
  background: #e5e7eb;
  color: #1f2937;
}

.certificate-preview {
  padding: 2rem;
}

.certificate-border {
  border: 3px solid #14b8a6;
  border-radius: 16px;
  padding: 0.5rem;
  background: linear-gradient(135deg, #f0fdfa 0%, #ecfeff 100%);
}

.certificate-inner {
  border: 1px dashed #99f6e4;
  border-radius: 12px;
  padding: 2.5rem;
  text-align: center;
}

.certificate-logo {
  width: 64px;
  height: 64px;
  background: linear-gradient(135deg, #14b8a6 0%, #06b6d4 100%);
  border-radius: 16px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 1.75rem;
  margin: 0 auto 1.5rem;
}

.certificate-title {
  font-size: 1.75rem;
  font-weight: 700;
  color: #0f766e;
  margin: 0 0 0.5rem 0;
}

.certificate-subtitle {
  font-size: 0.9rem;
  color: #6b7280;
  margin: 0 0 0.5rem 0;
}

.certificate-name {
  font-size: 1.5rem;
  font-weight: 700;
  color: #1f2937;
  margin: 0 0 0.5rem 0;
}

.certificate-text {
  font-size: 0.9rem;
  color: #6b7280;
  margin: 0 0 0.5rem 0;
}

.certificate-course {
  font-size: 1.25rem;
  font-weight: 700;
  color: #14b8a6;
  margin: 0 0 1.5rem 0;
}

.certificate-details {
  display: flex;
  justify-content: center;
  gap: 2rem;
  margin-bottom: 1.5rem;
  flex-wrap: wrap;
}

.certificate-detail {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.85rem;
  color: #6b7280;
}

.certificate-detail i {
  color: #14b8a6;
}

.certificate-badge {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem 1.25rem;
  background: linear-gradient(135deg, #fbbf24 0%, #f59e0b 100%);
  color: white;
  border-radius: 20px;
  font-size: 0.85rem;
  font-weight: 600;
}

.certificate-actions {
  padding: 1.5rem 2rem;
  border-top: 1px solid #e5e7eb;
  display: flex;
  justify-content: center;
  gap: 1rem;
}

.download-certificate-btn {
  padding: 0.75rem 1.5rem;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
  border: none;
  border-radius: 10px;
  font-size: 0.9rem;
  font-weight: 600;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  transition: all 0.2s;
}

.download-certificate-btn:hover {
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.3);
}

.close-certificate-btn {
  padding: 0.75rem 1.5rem;
  background: #f3f4f6;
  color: #4b5563;
  border: none;
  border-radius: 10px;
  font-size: 0.9rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
}

.close-certificate-btn:hover {
  background: #e5e7eb;
}

/* Modal Transition */
.modal-enter-active,
.modal-leave-active {
  transition: all 0.3s ease;
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}

.modal-enter-from .certificate-modal,
.modal-leave-to .certificate-modal {
  transform: scale(0.9);
}

/* Responsive for objectives */
@media (max-width: 768px) {
  .objectives-grid {
    grid-template-columns: 1fr;
  }

  .certificate-details {
    flex-direction: column;
    gap: 0.75rem;
  }
}
</style>
