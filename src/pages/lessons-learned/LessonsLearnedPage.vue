<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useI18n } from 'vue-i18n'
import PageHeroHeader from '@/components/common/PageHeroHeader.vue'
import EmptyState from '@/components/common/EmptyState.vue'
import Pagination from '@/components/common/Pagination.vue'
import CategoryBadge from '@/components/common/CategoryBadge.vue'
import StatusBadge from '@/components/common/StatusBadge.vue'
import TagBadge from '@/components/common/TagBadge.vue'

const router = useRouter()
const route = useRoute()
const { t, locale } = useI18n()
const isRTL = computed(() => locale.value === 'ar')

// ============================================
// TYPES
// ============================================
type LessonStatus = 'Draft' | 'PendingReview' | 'Approved' | 'Rejected' | 'Published' | 'ActionsPending' | 'ActionsComplete' | 'Verified' | 'Archived'
type LessonPriority = 'Low' | 'Medium' | 'High' | 'Critical'
type LessonCategory = 'Process' | 'Technical' | 'Communication' | 'Leadership' | 'TeamManagement' | 'RiskManagement' | 'StakeholderManagement' | 'BudgetManagement' | 'QualityAssurance' | 'VendorManagement' | 'Other'
type ActionStatus = 'Open' | 'InProgress' | 'Completed' | 'Verified' | 'Cancelled'
type RootCauseMethod = 'FiveWhys' | 'Fishbone' | 'FaultTree' | 'ParetoAnalysis' | 'Other'

interface LessonAction {
  id: string
  description: string
  descriptionArabic?: string
  assigneeName: string
  status: ActionStatus
  priority: string
  dueDate: string
  startedAt?: string
  completedAt?: string
  completionNotes?: string
  verifiedAt?: string
  verifiedByName?: string
  affectedDocumentTitle?: string
  affectedDocumentType?: string
  isOverdue: boolean
  reminderCount: number
  escalatedAt?: string
}

interface LessonLearned {
  id: string
  title: string
  titleArabic?: string
  description: string
  descriptionArabic?: string
  summary: string
  content: string
  context: string
  challenge: string
  solution: string
  outcome: string
  recommendations?: string[]
  whatWentWell?: string
  rootCause?: string
  rootCauseMethod?: RootCauseMethod
  category: LessonCategory
  impact: LessonPriority
  status: LessonStatus
  projectPhase?: string
  impactType?: string
  authorName: string
  authorAvatarUrl?: string
  authorInitials: string
  authorDepartment: string
  isAnonymous: boolean
  processOwnerName?: string
  projectName?: string
  communityName?: string
  occurredAt?: string
  viewCount: number
  usefulCount: number
  likeCount: number
  commentCount: number
  isLiked: boolean
  isBookmarked: boolean
  isFeatured: boolean
  tags: string[]
  totalActions: number
  completedActions: number
  overdueActions: number
  createdAt: string
  actions?: LessonAction[]
  appliedInProjects?: string[]
}

interface AnalyticsOverview {
  totalLessons: number
  lessonsPublishedInPeriod: number
  lessonsByStatus: Record<string, number>
  lessonsByCategory: Record<string, number>
  totalActions: number
  openActions: number
  completedActions: number
  overdueActions: number
  actionCompletionRate: number
  averageTimeToCompleteActionDays: number
  lessonsWithoutProcessOwner: number
}

// ============================================
// STATE
// ============================================
const isLoading = ref(false)
const currentView = ref<'list' | 'analytics' | 'create'>('list')
const searchQuery = ref('')
const selectedCategory = ref<string>('all')
const selectedStatus = ref<string>('all')
const selectedImpact = ref<string>('all')
const viewMode = ref<'grid' | 'list'>('grid')
const showCreateModal = ref(false)
const showDetailModal = ref(false)
const selectedLesson = ref<LessonLearned | null>(null)
const showActionModal = ref(false)
const activeTab = ref<'details' | 'actions' | 'rootcause' | 'comments'>('details')

// New lesson form
const newLesson = ref({
  title: '',
  category: '',
  impact: '',
  projectName: '',
  projectPhase: '',
  summary: '',
  context: '',
  challenge: '',
  solution: '',
  outcome: '',
  rootCause: '',
  tags: '',
  isAnonymous: false
})

function submitNewLesson(status: LessonStatus) {
  if (!newLesson.value.title || !newLesson.value.category || !newLesson.value.impact || !newLesson.value.summary || !newLesson.value.challenge || !newLesson.value.solution) {
    return
  }
  const tagsList = newLesson.value.tags.split(',').map(t => t.trim()).filter(Boolean)
  const id = String(lessons.value.length + 1)
  lessons.value.unshift({
    id,
    title: newLesson.value.title,
    summary: newLesson.value.summary,
    description: newLesson.value.summary,
    content: [newLesson.value.context, newLesson.value.challenge, newLesson.value.solution, newLesson.value.outcome].filter(Boolean).join('\n\n'),
    context: newLesson.value.context,
    challenge: newLesson.value.challenge,
    solution: newLesson.value.solution,
    outcome: newLesson.value.outcome,
    category: newLesson.value.category as LessonCategory,
    impact: newLesson.value.impact as LessonPriority,
    status,
    projectName: newLesson.value.projectName || undefined,
    projectPhase: newLesson.value.projectPhase || undefined,
    authorName: newLesson.value.isAnonymous ? 'Anonymous' : 'Current User',
    authorInitials: newLesson.value.isAnonymous ? 'AN' : 'CU',
    authorDepartment: 'My Department',
    isAnonymous: newLesson.value.isAnonymous,
    viewCount: 0,
    usefulCount: 0,
    likeCount: 0,
    commentCount: 0,
    isLiked: false,
    isBookmarked: false,
    isFeatured: false,
    tags: tagsList,
    totalActions: 0,
    completedActions: 0,
    overdueActions: 0,
    createdAt: new Date().toISOString().split('T')[0]
  })
  // Reset form
  newLesson.value = { title: '', category: '', impact: '', projectName: '', projectPhase: '', summary: '', context: '', challenge: '', solution: '', outcome: '', rootCause: '', tags: '', isAnonymous: false }
  showCreateModal.value = false
}

// Pagination
const currentPage = ref(1)
const pageSize = ref(12)

// New state
const showBookmarkedOnly = ref(false)
const lessonsSortBy = ref('newest')
const lessonsSortOrder = ref<'asc' | 'desc'>('desc')
const selectedCategories = ref<string[]>([])
const selectedStatuses = ref<string[]>([])
const selectedImpacts = ref<string[]>([])
const selectedTags = ref<string[]>([])
const showCategoryDropdown = ref(false)
const showStatusDropdown = ref(false)
const showImpactDropdown = ref(false)
const showTagsDropdown = ref(false)
const showSortDropdown = ref(false)
const currentFeaturedIndex = ref(0)
const featuredInterval = ref<number | null>(null)
const showLessonDetailModal = ref(false)
const selectedLessonForModal = ref<LessonLearned | null>(null)

// ============================================
// MOCK DATA
// ============================================
const lessons = ref<LessonLearned[]>([
  {
    id: '1',
    title: 'Effective Crowd Management at Large Events',
    titleArabic: 'إدارة الحشود الفعالة في الفعاليات الكبيرة',
    description: 'Key insights from managing crowd flow during the opening ceremony rehearsal.',
    summary: 'Key insights from managing crowd flow during the opening ceremony rehearsal.',
    content: 'During the opening ceremony rehearsal, we had approximately 15,000 attendees. Managing crowd flow at entry points became congested, causing delays of up to 45 minutes. We implemented a zone-based entry system with dedicated lanes for different ticket types.',
    context: 'During the opening ceremony rehearsal, we had approximately 15,000 attendees.',
    challenge: 'Managing crowd flow at entry points became congested, causing delays of up to 45 minutes.',
    solution: 'We implemented a zone-based entry system with dedicated lanes for different ticket types.',
    outcome: 'Entry times reduced by 60%. Attendee satisfaction scores improved significantly.',
    recommendations: ['Conduct crowd simulations before large events', 'Establish clear communication protocols', 'Implement zone-based entry systems'],
    whatWentWell: 'The zone-based entry system was immediately effective. Team coordination exceeded expectations.',
    rootCause: 'Single entry point bottleneck with no differentiation by ticket type',
    rootCauseMethod: 'FiveWhys',
    category: 'Process',
    impact: 'High',
    status: 'ActionsPending',
    projectPhase: 'Execution',
    impactType: 'Schedule',
    authorName: 'Mohammed Al-Rashid',
    authorInitials: 'MR',
    authorDepartment: 'Operations',
    isAnonymous: false,
    processOwnerName: 'Khalid Al-Mansoori',
    projectName: 'Opening Ceremony',
    occurredAt: '2026-02-15',
    viewCount: 456,
    usefulCount: 89,
    likeCount: 89,
    commentCount: 12,
    isLiked: true,
    isBookmarked: false,
    isFeatured: true,
    tags: ['Crowd Management', 'Safety', 'Events', 'Operations'],
    totalActions: 3,
    completedActions: 1,
    overdueActions: 1,
    createdAt: '2026-03-06',
    appliedInProjects: ['Stadium B Setup', 'Fan Zone Operations'],
    actions: [
      { id: 'a1', description: 'Update crowd management SOP to include zone-based entry', assigneeName: 'Khalid Al-Mansoori', status: 'InProgress', priority: 'High', dueDate: '2026-03-30', startedAt: '2026-03-13', affectedDocumentTitle: 'Crowd Management SOP v2.1', affectedDocumentType: 'Document', isOverdue: false, reminderCount: 0 },
      { id: 'a2', description: 'Deploy crowd density monitoring sensors at all entry points', assigneeName: 'Sara Ali', status: 'Open', priority: 'Urgent', dueDate: '2026-03-14', isOverdue: true, reminderCount: 2, escalatedAt: '2026-03-15' },
      { id: 'a3', description: 'Conduct radio communication training for security teams', assigneeName: 'Ahmed Hassan', status: 'Completed', priority: 'Normal', dueDate: '2026-03-06', startedAt: '2026-02-24', completedAt: '2026-03-04', completionNotes: 'Completed 4 sessions covering 120 staff', isOverdue: false, reminderCount: 0 }
    ]
  },
  {
    id: '2',
    title: 'Vendor Communication Best Practices',
    titleArabic: 'أفضل ممارسات التواصل مع الموردين',
    description: 'Lessons from coordinating with multiple vendors during venue setup.',
    summary: 'Lessons from coordinating with multiple vendors during venue setup.',
    content: 'Multi-vendor coordination for 8 stadium venues simultaneously. Miscommunication between vendors caused overlapping work schedules and resource conflicts. Established a unified vendor portal with shared calendars and daily standups.',
    context: 'Multi-vendor coordination for 8 stadium venues simultaneously.',
    challenge: 'Miscommunication between vendors caused overlapping work schedules and resource conflicts.',
    solution: 'Established a unified vendor portal with shared calendars and daily standups.',
    outcome: 'Vendor-related delays reduced by 75%. Zero resource conflicts in subsequent phases.',
    recommendations: ['Mandate vendor portal usage for all vendors', 'Conduct weekly cross-vendor sync meetings', 'Assign dedicated vendor liaison officers'],
    rootCause: 'No centralized communication channel between vendor teams',
    rootCauseMethod: 'Fishbone',
    category: 'VendorManagement',
    impact: 'Medium',
    status: 'Published',
    projectPhase: 'Planning',
    impactType: 'Cost',
    authorName: 'Sara Ali',
    authorInitials: 'SA',
    authorDepartment: 'Procurement',
    isAnonymous: false,
    processOwnerName: 'Fatima Khalil',
    projectName: 'Venue Setup',
    viewCount: 234,
    usefulCount: 45,
    likeCount: 45,
    commentCount: 8,
    isLiked: false,
    isBookmarked: true,
    isFeatured: false,
    tags: ['Vendors', 'Communication', 'Coordination'],
    totalActions: 2,
    completedActions: 2,
    overdueActions: 0,
    createdAt: '2026-03-01'
  },
  {
    id: '3',
    title: 'Technology Integration Challenges',
    titleArabic: 'تحديات تكامل التقنية',
    description: 'Technical challenges faced during ticketing system integration.',
    summary: 'Technical challenges faced during ticketing system integration.',
    content: 'Integration of 3 separate ticketing APIs into unified platform. API rate limits and data format inconsistencies caused 4-hour outage on test day. Implemented message queue buffering, data normalization layer, and circuit breakers.',
    context: 'Integration of 3 separate ticketing APIs into unified platform.',
    challenge: 'API rate limits and data format inconsistencies caused 4-hour outage on test day.',
    solution: 'Implemented message queue buffering, data normalization layer, and circuit breakers.',
    outcome: 'Zero outages in production. 99.97% uptime achieved.',
    recommendations: ['Perform load testing against third-party API rate limits', 'Implement circuit breaker patterns for all external integrations', 'Create data normalization layers early in integration projects'],
    whatWentWell: 'Circuit breaker pattern prevented cascade failures. Monitoring alerts caught issues early.',
    rootCause: 'No load testing performed against third-party API rate limits',
    rootCauseMethod: 'FiveWhys',
    category: 'Technical',
    impact: 'Critical',
    status: 'Verified',
    projectPhase: 'Execution',
    impactType: 'Quality',
    authorName: 'Ahmed Hassan',
    authorInitials: 'AH',
    authorDepartment: 'Engineering',
    isAnonymous: false,
    processOwnerName: 'Ahmed Hassan',
    projectName: 'IT Infrastructure',
    viewCount: 567,
    usefulCount: 123,
    likeCount: 123,
    commentCount: 23,
    isLiked: true,
    isBookmarked: false,
    isFeatured: true,
    tags: ['Technology', 'Integration', 'Systems', 'API'],
    totalActions: 4,
    completedActions: 4,
    overdueActions: 0,
    createdAt: '2026-02-20',
    appliedInProjects: ['Media Center Platform', 'Broadcast Systems']
  },
  {
    id: '4',
    title: 'Safety Protocol Gaps During Night Events',
    titleArabic: 'فجوات بروتوكول السلامة خلال الفعاليات الليلية',
    description: 'Critical safety gaps identified during first night match.',
    summary: 'Critical safety gaps identified during first night match.',
    content: 'First night match with 40,000 attendees. Emergency lighting and signage tested. Emergency exit signage not visible under reduced lighting. Evacuation path markings faded. Installed phosphorescent markers, LED emergency strips, and updated evacuation maps.',
    context: 'First night match with 40,000 attendees. Emergency lighting and signage tested.',
    challenge: 'Emergency exit signage not visible under reduced lighting. Evacuation path markings faded.',
    solution: 'Installed phosphorescent markers, LED emergency strips, and updated evacuation maps.',
    outcome: 'All emergency paths now visible in complete darkness. Evacuation drill time improved 40%.',
    recommendations: ['Include night-conditions testing in all safety audits', 'Install phosphorescent markers at all emergency exits', 'Conduct quarterly evacuation drills under various lighting conditions'],
    rootCause: 'Safety audit did not include night-conditions testing',
    rootCauseMethod: 'FaultTree',
    category: 'RiskManagement',
    impact: 'Critical',
    status: 'ActionsComplete',
    projectPhase: 'Operations',
    impactType: 'Safety',
    authorName: 'Anonymous',
    authorInitials: 'AN',
    authorDepartment: 'Safety',
    isAnonymous: true,
    processOwnerName: 'Omar Al-Farsi',
    projectName: 'Match Operations',
    viewCount: 892,
    usefulCount: 201,
    likeCount: 201,
    commentCount: 31,
    isLiked: true,
    isBookmarked: true,
    isFeatured: false,
    tags: ['Safety', 'Emergency', 'Night Events', 'Compliance'],
    totalActions: 5,
    completedActions: 5,
    overdueActions: 0,
    createdAt: '2026-02-10'
  },
  {
    id: '5',
    title: 'Effective Stakeholder Communication During Project Delays',
    description: 'Transparent and proactive stakeholder communication proved critical when facing unexpected project delays during the infrastructure upgrade phase.',
    summary: 'Transparent and proactive stakeholder communication proved critical when facing unexpected project delays during the infrastructure upgrade phase.',
    content: 'During the infrastructure upgrade phase, unexpected soil conditions caused a 3-week delay. Rather than waiting for the situation to resolve, the project team immediately engaged all stakeholders with transparent updates, revised timelines, and mitigation plans. This proactive approach maintained trust and allowed dependent teams to adjust their schedules accordingly. Weekly visual dashboards were introduced to communicate complex timeline changes in an easily digestible format.',
    context: 'Infrastructure upgrade phase with multiple dependent workstreams and senior stakeholder oversight.',
    challenge: 'Unexpected soil conditions caused a 3-week delay threatening cascading impacts across dependent teams.',
    solution: 'Implemented proactive communication strategy with transparent updates, revised timelines, visual dashboards, and mitigation plans shared with all stakeholders.',
    outcome: 'Maintained stakeholder trust despite delays. Dependent teams adjusted schedules with minimal disruption. No escalations from senior management.',
    recommendations: ['Create communication templates for common delay scenarios', 'Establish escalation protocols early in project lifecycle', 'Use visual dashboards for communicating complex timelines'],
    category: 'Communication',
    impact: 'High',
    status: 'Published',
    projectPhase: 'Execution',
    impactType: 'Schedule',
    authorName: 'Sarah Chen',
    authorInitials: 'SC',
    authorDepartment: 'Project Management',
    isAnonymous: false,
    isFeatured: true,
    viewCount: 234,
    usefulCount: 45,
    likeCount: 45,
    commentCount: 12,
    isLiked: false,
    isBookmarked: false,
    tags: ['Stakeholder Management', 'Communication', 'Crisis Management'],
    totalActions: 0,
    completedActions: 0,
    overdueActions: 0,
    createdAt: '2026-01-15'
  },
  {
    id: '6',
    title: 'API Integration Best Practices for Large-Scale Systems',
    description: 'Comprehensive lessons from integrating multiple third-party APIs into the central platform, focusing on resilience patterns and graceful degradation.',
    summary: 'Comprehensive lessons from integrating multiple third-party APIs into the central platform, focusing on resilience patterns and graceful degradation.',
    content: 'During the integration of 12 third-party APIs into our central platform, we discovered that traditional synchronous request patterns were insufficient for handling the scale and variability of external services. By adopting an event-driven architecture with circuit breakers, bulkheads, and retry policies, we achieved 99.99% availability even when individual upstream services experienced outages. Key insight: design for failure from day one rather than retrofitting resilience patterns.',
    context: 'Central platform integration with 12 third-party APIs serving 50,000+ concurrent users.',
    challenge: 'Synchronous API calls caused cascading failures when any single upstream service degraded.',
    solution: 'Adopted event-driven architecture with circuit breakers, bulkheads, retry policies, and graceful degradation patterns.',
    outcome: 'Achieved 99.99% platform availability. Zero cascading failures in production over 6 months.',
    recommendations: ['Design for failure from day one in all integration projects', 'Implement circuit breakers for every external dependency', 'Use event-driven patterns for cross-service communication'],
    category: 'Technical',
    impact: 'Critical',
    status: 'Verified',
    projectPhase: 'Execution',
    impactType: 'Quality',
    authorName: 'James Wilson',
    authorInitials: 'JW',
    authorDepartment: 'Engineering',
    isAnonymous: false,
    isFeatured: true,
    viewCount: 456,
    usefulCount: 89,
    likeCount: 89,
    commentCount: 23,
    isLiked: true,
    isBookmarked: false,
    tags: ['API', 'Integration', 'Architecture', 'Resilience'],
    totalActions: 2,
    completedActions: 2,
    overdueActions: 0,
    createdAt: '2026-01-12',
    appliedInProjects: ['Payment Gateway v2', 'Mobile App Refresh']
  },
  {
    id: '7',
    title: 'Database Migration Strategies Without Downtime',
    description: 'Lessons learned from migrating a 2TB production database to a new schema without any service interruption using blue-green deployment strategies.',
    summary: 'Lessons learned from migrating a 2TB production database to a new schema without any service interruption using blue-green deployment strategies.',
    content: 'Migrating a 2TB production database with 500+ tables required careful planning to avoid downtime. We used a blue-green deployment strategy with dual-write capabilities, shadow traffic testing, and incremental cutover. The migration was completed over a 2-week period with zero downtime and zero data loss. Key lesson: invest heavily in automated verification and rollback procedures before starting any migration.',
    context: 'Production database migration of 2TB across 500+ tables serving real-time transaction processing.',
    challenge: 'Traditional migration approaches would require 8+ hours of downtime, which was unacceptable for 24/7 operations.',
    solution: 'Implemented blue-green deployment with dual-write capabilities, shadow traffic testing, and incremental cutover procedures.',
    outcome: 'Zero downtime migration completed over 2 weeks. Zero data loss. All automated verification checks passed.',
    recommendations: ['Invest in automated verification and rollback procedures before migration', 'Use dual-write patterns to maintain data consistency during transition', 'Run shadow traffic tests for at least one week before cutover'],
    category: 'Technical',
    impact: 'Critical',
    status: 'Published',
    projectPhase: 'Execution',
    impactType: 'Quality',
    authorName: 'Alex Thompson',
    authorInitials: 'AT',
    authorDepartment: 'Infrastructure',
    isAnonymous: false,
    isFeatured: false,
    viewCount: 567,
    usefulCount: 112,
    likeCount: 112,
    commentCount: 31,
    isLiked: false,
    isBookmarked: false,
    tags: ['Database', 'Migration', 'DevOps'],
    totalActions: 1,
    completedActions: 1,
    overdueActions: 0,
    createdAt: '2026-01-05'
  },
  {
    id: '8',
    title: 'Security Best Practices for Public-Facing Applications',
    description: 'Critical security lessons from a penetration testing exercise that revealed vulnerabilities in our public-facing web applications.',
    summary: 'Critical security lessons from a penetration testing exercise that revealed vulnerabilities in our public-facing web applications.',
    content: 'A comprehensive penetration testing exercise on our public-facing applications revealed 23 vulnerabilities, including 3 critical ones related to authentication bypass and SQL injection. The remediation effort took 6 weeks and resulted in a complete overhaul of our security review process. We now mandate security reviews at every stage of the SDLC, automated SAST/DAST scanning in CI/CD pipelines, and quarterly penetration tests. Key lesson: security must be built in, not bolted on.',
    context: 'Penetration testing of 8 public-facing web applications ahead of major launch event.',
    challenge: '23 vulnerabilities discovered including 3 critical authentication bypass and SQL injection issues.',
    solution: 'Complete security process overhaul: mandatory security reviews at every SDLC stage, automated SAST/DAST in CI/CD, quarterly pen tests.',
    outcome: 'All 23 vulnerabilities remediated. Zero critical findings in subsequent quarterly pen test. Security review time reduced by 40% through automation.',
    recommendations: ['Mandate security reviews at every stage of the development lifecycle', 'Integrate automated SAST/DAST scanning into CI/CD pipelines', 'Conduct quarterly penetration tests on all public-facing applications'],
    category: 'Technical',
    impact: 'Critical',
    status: 'Published',
    projectPhase: 'Operations',
    impactType: 'Safety',
    authorName: 'Lisa Park',
    authorInitials: 'LP',
    authorDepartment: 'Security',
    isAnonymous: false,
    isFeatured: false,
    viewCount: 423,
    usefulCount: 98,
    likeCount: 98,
    commentCount: 27,
    isLiked: false,
    isBookmarked: false,
    tags: ['Security', 'Application Security', 'Best Practices'],
    totalActions: 3,
    completedActions: 2,
    overdueActions: 1,
    createdAt: '2025-12-20'
  }
])

const analytics = ref<AnalyticsOverview>({
  totalLessons: 87,
  lessonsPublishedInPeriod: 8,
  lessonsByStatus: { Draft: 5, PendingReview: 3, Published: 45, ActionsPending: 15, ActionsComplete: 8, Verified: 6, Archived: 3, Approved: 2 },
  lessonsByCategory: { Process: 22, Technical: 18, Communication: 12, TeamManagement: 10, RiskManagement: 8, VendorManagement: 7, QualityAssurance: 5, Other: 5 },
  totalActions: 156,
  openActions: 34,
  completedActions: 89,
  overdueActions: 11,
  actionCompletionRate: 71.2,
  averageTimeToCompleteActionDays: 18.5,
  lessonsWithoutProcessOwner: 14
})

// ============================================
// COMPUTED
// ============================================
const filteredLessons = computed(() => {
  let result = [...lessons.value]

  // Search filter
  if (searchQuery.value) {
    const q = searchQuery.value.toLowerCase()
    result = result.filter(l =>
      l.title.toLowerCase().includes(q) ||
      l.description.toLowerCase().includes(q) ||
      l.tags.some(t => t.toLowerCase().includes(q))
    )
  }

  // Bookmarked filter
  if (showBookmarkedOnly.value) {
    result = result.filter(l => l.isBookmarked)
  }

  // Legacy single-select filters (backward compat)
  if (selectedCategory.value !== 'all') result = result.filter(l => l.category === selectedCategory.value)
  if (selectedStatus.value !== 'all') result = result.filter(l => l.status === selectedStatus.value)
  if (selectedImpact.value !== 'all') result = result.filter(l => l.impact === selectedImpact.value)

  // Multi-select filters
  if (selectedCategories.value.length > 0) {
    result = result.filter(l => selectedCategories.value.includes(l.category))
  }
  if (selectedStatuses.value.length > 0) {
    result = result.filter(l => selectedStatuses.value.includes(l.status))
  }
  if (selectedImpacts.value.length > 0) {
    result = result.filter(l => selectedImpacts.value.includes(l.impact))
  }
  if (selectedTags.value.length > 0) {
    result = result.filter(l => l.tags.some(tag => selectedTags.value.includes(tag)))
  }

  // Sort logic
  switch (lessonsSortBy.value) {
    case 'newest':
      result.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
      break
    case 'popular':
      result.sort((a, b) => b.viewCount - a.viewCount)
      break
    case 'liked':
      result.sort((a, b) => b.likeCount - a.likeCount)
      break
    case 'title':
      result.sort((a, b) => a.title.localeCompare(b.title))
      break
  }

  if (lessonsSortOrder.value === 'asc') {
    result.reverse()
  }

  return result
})

const totalPages = computed(() => Math.ceil(filteredLessons.value.length / pageSize.value))
const paginatedLessons = computed(() => {
  const start = (currentPage.value - 1) * pageSize.value
  return filteredLessons.value.slice(start, start + pageSize.value)
})

const featuredLessons = computed(() => lessons.value.filter(l => l.isFeatured).slice(0, 5))

const currentFeaturedLesson = computed(() => {
  if (featuredLessons.value.length === 0) return null
  return featuredLessons.value[currentFeaturedIndex.value % featuredLessons.value.length]
})

const nextFeaturedLessons = computed(() => {
  const featured = featuredLessons.value
  if (featured.length <= 1) return []
  const result: LessonLearned[] = []
  for (let i = 1; i <= 3; i++) {
    const idx = (currentFeaturedIndex.value + i) % featured.length
    result.push(featured[idx])
  }
  return result
})

const allLessonsTags = computed(() => {
  const tagSet = new Set<string>()
  lessons.value.forEach(l => l.tags.forEach(tag => tagSet.add(tag)))
  return Array.from(tagSet).sort()
})

const categoryStats = computed(() => {
  const stats: Record<string, number> = {}
  lessons.value.forEach(l => {
    stats[l.category] = (stats[l.category] || 0) + 1
  })
  return stats
})

const activeLessonsFiltersCount = computed(() => {
  let count = 0
  if (searchQuery.value) count++
  if (showBookmarkedOnly.value) count++
  if (selectedCategories.value.length > 0) count += selectedCategories.value.length
  if (selectedStatuses.value.length > 0) count += selectedStatuses.value.length
  if (selectedImpacts.value.length > 0) count += selectedImpacts.value.length
  if (selectedTags.value.length > 0) count += selectedTags.value.length
  return count
})

const lessonsStats = computed(() => ({
  total: lessons.value.length,
  totalLikes: lessons.value.reduce((sum, l) => sum + l.likeCount, 0),
  bookmarked: lessons.value.filter(l => l.isBookmarked).length,
  totalViews: lessons.value.reduce((sum, l) => sum + l.viewCount, 0)
}))

// ============================================
// HELPERS
// ============================================
const statusConfig: Record<string, { label: string; color: string; icon: string }> = {
  Draft: { label: 'Draft', color: 'bg-gray-100 text-gray-700', icon: 'fas fa-pencil-alt' },
  PendingReview: { label: 'Pending Review', color: 'bg-yellow-100 text-yellow-700', icon: 'fas fa-clock' },
  Approved: { label: 'Approved', color: 'bg-blue-100 text-blue-700', icon: 'fas fa-check' },
  Rejected: { label: 'Rejected', color: 'bg-red-100 text-red-700', icon: 'fas fa-times' },
  Published: { label: 'Published', color: 'bg-green-100 text-green-700', icon: 'fas fa-globe' },
  ActionsPending: { label: 'Actions Pending', color: 'bg-orange-100 text-orange-700', icon: 'fas fa-tasks' },
  ActionsComplete: { label: 'Actions Complete', color: 'bg-teal-100 text-teal-700', icon: 'fas fa-check-double' },
  Verified: { label: 'Verified', color: 'bg-emerald-100 text-emerald-700', icon: 'fas fa-shield-alt' },
  Archived: { label: 'Archived', color: 'bg-slate-100 text-slate-700', icon: 'fas fa-archive' }
}

const impactConfig: Record<string, { label: string; color: string }> = {
  Low: { label: 'Low', color: 'bg-green-100 text-green-700' },
  Medium: { label: 'Medium', color: 'bg-amber-100 text-amber-700' },
  High: { label: 'High', color: 'bg-orange-100 text-orange-700' },
  Critical: { label: 'Critical', color: 'bg-red-100 text-red-700' }
}

const actionStatusConfig: Record<string, { label: string; color: string; icon: string }> = {
  Open: { label: 'Open', color: 'text-gray-600', icon: 'fas fa-circle' },
  InProgress: { label: 'In Progress', color: 'text-blue-600', icon: 'fas fa-spinner' },
  Completed: { label: 'Completed', color: 'text-green-600', icon: 'fas fa-check-circle' },
  Verified: { label: 'Verified', color: 'text-emerald-600', icon: 'fas fa-shield-alt' },
  Cancelled: { label: 'Cancelled', color: 'text-gray-400', icon: 'fas fa-ban' }
}

const rootCauseMethodLabels: Record<string, string> = {
  FiveWhys: '5 Whys Analysis',
  Fishbone: 'Fishbone (Ishikawa)',
  FaultTree: 'Fault Tree Analysis',
  ParetoAnalysis: 'Pareto Analysis',
  Other: 'Other Method'
}

const categoryOptions = [
  { id: 'Process', label: 'Process', icon: 'fas fa-cogs', color: 'text-blue-600' },
  { id: 'Technical', label: 'Technical', icon: 'fas fa-microchip', color: 'text-purple-600' },
  { id: 'Communication', label: 'Communication', icon: 'fas fa-comments', color: 'text-cyan-600' },
  { id: 'Leadership', label: 'Leadership', icon: 'fas fa-chess-king', color: 'text-amber-600' },
  { id: 'RiskManagement', label: 'Risk Management', icon: 'fas fa-shield-alt', color: 'text-red-600' },
  { id: 'TeamManagement', label: 'Team Management', icon: 'fas fa-users', color: 'text-indigo-600' },
  { id: 'VendorManagement', label: 'Vendor Management', icon: 'fas fa-handshake', color: 'text-orange-600' },
  { id: 'QualityAssurance', label: 'Quality Assurance', icon: 'fas fa-clipboard-check', color: 'text-teal-600' },
  { id: 'Other', label: 'Other', icon: 'fas fa-ellipsis-h', color: 'text-gray-600' }
]

const sortOptions = [
  { value: 'newest', label: 'Newest First', icon: 'fas fa-calendar-alt' },
  { value: 'popular', label: 'Most Viewed', icon: 'fas fa-eye' },
  { value: 'liked', label: 'Most Liked', icon: 'fas fa-heart' },
  { value: 'title', label: 'Title A-Z', icon: 'fas fa-sort-alpha-down' }
]

// ============================================
// FUNCTIONS
// ============================================
function getActionProgress(lesson: LessonLearned) {
  if (lesson.totalActions === 0) return 0
  return Math.round((lesson.completedActions / lesson.totalActions) * 100)
}

function openDetail(lesson: LessonLearned) {
  selectedLesson.value = lesson
  activeTab.value = 'details'
  showDetailModal.value = true
}

// Workflow transitions
const workflowTransitions: Record<LessonStatus, { actions: { label: string; icon: string; targetStatus: LessonStatus; color: string; confirm?: string }[] }> = {
  Draft: {
    actions: [
      { label: 'Submit for Review', icon: 'fas fa-paper-plane', targetStatus: 'PendingReview', color: 'bg-blue-500 hover:bg-blue-600 text-white' }
    ]
  },
  PendingReview: {
    actions: [
      { label: 'Approve', icon: 'fas fa-check', targetStatus: 'Approved', color: 'bg-green-500 hover:bg-green-600 text-white' },
      { label: 'Reject', icon: 'fas fa-times', targetStatus: 'Rejected', color: 'bg-red-500 hover:bg-red-600 text-white', confirm: 'Are you sure you want to reject this lesson?' },
      { label: 'Return to Draft', icon: 'fas fa-undo', targetStatus: 'Draft', color: 'bg-gray-200 hover:bg-gray-300 text-gray-700' }
    ]
  },
  Approved: {
    actions: [
      { label: 'Publish', icon: 'fas fa-globe', targetStatus: 'Published', color: 'bg-teal-500 hover:bg-teal-600 text-white' },
      { label: 'Return to Review', icon: 'fas fa-undo', targetStatus: 'PendingReview', color: 'bg-gray-200 hover:bg-gray-300 text-gray-700' }
    ]
  },
  Rejected: {
    actions: [
      { label: 'Resubmit for Review', icon: 'fas fa-redo', targetStatus: 'PendingReview', color: 'bg-blue-500 hover:bg-blue-600 text-white' },
      { label: 'Return to Draft', icon: 'fas fa-pencil-alt', targetStatus: 'Draft', color: 'bg-gray-200 hover:bg-gray-300 text-gray-700' }
    ]
  },
  Published: {
    actions: [
      { label: 'Mark Actions Pending', icon: 'fas fa-tasks', targetStatus: 'ActionsPending', color: 'bg-orange-500 hover:bg-orange-600 text-white' },
      { label: 'Archive', icon: 'fas fa-archive', targetStatus: 'Archived', color: 'bg-gray-200 hover:bg-gray-300 text-gray-700', confirm: 'Are you sure you want to archive this lesson?' }
    ]
  },
  ActionsPending: {
    actions: [
      { label: 'Mark Actions Complete', icon: 'fas fa-check-double', targetStatus: 'ActionsComplete', color: 'bg-teal-500 hover:bg-teal-600 text-white' }
    ]
  },
  ActionsComplete: {
    actions: [
      { label: 'Verify', icon: 'fas fa-shield-alt', targetStatus: 'Verified', color: 'bg-emerald-500 hover:bg-emerald-600 text-white' },
      { label: 'Reopen Actions', icon: 'fas fa-undo', targetStatus: 'ActionsPending', color: 'bg-gray-200 hover:bg-gray-300 text-gray-700' }
    ]
  },
  Verified: {
    actions: [
      { label: 'Archive', icon: 'fas fa-archive', targetStatus: 'Archived', color: 'bg-gray-200 hover:bg-gray-300 text-gray-700' }
    ]
  },
  Archived: {
    actions: [
      { label: 'Restore to Published', icon: 'fas fa-undo', targetStatus: 'Published', color: 'bg-blue-500 hover:bg-blue-600 text-white' }
    ]
  }
}

const workflowSteps: { status: LessonStatus; label: string; icon: string }[] = [
  { status: 'Draft', label: 'Draft', icon: 'fas fa-pencil-alt' },
  { status: 'PendingReview', label: 'Review', icon: 'fas fa-clock' },
  { status: 'Approved', label: 'Approved', icon: 'fas fa-check' },
  { status: 'Published', label: 'Published', icon: 'fas fa-globe' },
  { status: 'ActionsPending', label: 'Actions', icon: 'fas fa-tasks' },
  { status: 'ActionsComplete', label: 'Complete', icon: 'fas fa-check-double' },
  { status: 'Verified', label: 'Verified', icon: 'fas fa-shield-alt' }
]

function getWorkflowStepIndex(status: LessonStatus): number {
  if (status === 'Rejected') return 1
  if (status === 'Archived') return 7
  return workflowSteps.findIndex(s => s.status === status)
}

function changeStatus(lesson: LessonLearned, targetStatus: LessonStatus, confirmMsg?: string) {
  if (confirmMsg && !confirm(confirmMsg)) return
  lesson.status = targetStatus
}

function getAvailableActions(status: LessonStatus) {
  return workflowTransitions[status]?.actions || []
}

function formatDate(date?: string) {
  if (!date) return '—'
  return new Date(date).toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' })
}

// Featured carousel
function nextFeaturedLesson() {
  if (featuredLessons.value.length === 0) return
  currentFeaturedIndex.value = (currentFeaturedIndex.value + 1) % featuredLessons.value.length
}

function prevFeaturedLesson() {
  if (featuredLessons.value.length === 0) return
  currentFeaturedIndex.value = (currentFeaturedIndex.value - 1 + featuredLessons.value.length) % featuredLessons.value.length
}

function goToFeaturedLesson(index: number) {
  currentFeaturedIndex.value = index
}

function startFeaturedLessonAutoPlay() {
  if (featuredInterval.value) return
  featuredInterval.value = window.setInterval(() => {
    nextFeaturedLesson()
  }, 5000)
}

function pauseFeaturedLessonAutoPlay() {
  if (featuredInterval.value) {
    clearInterval(featuredInterval.value)
    featuredInterval.value = null
  }
}

function resumeFeaturedLessonAutoPlay() {
  startFeaturedLessonAutoPlay()
}

// Multi-select filters
function toggleCategoryFilter(category: string) {
  const idx = selectedCategories.value.indexOf(category)
  if (idx === -1) {
    selectedCategories.value.push(category)
  } else {
    selectedCategories.value.splice(idx, 1)
  }
  currentPage.value = 1
}

function toggleStatusFilter(status: string) {
  const idx = selectedStatuses.value.indexOf(status)
  if (idx === -1) {
    selectedStatuses.value.push(status)
  } else {
    selectedStatuses.value.splice(idx, 1)
  }
  currentPage.value = 1
}

function toggleImpactFilter(impact: string) {
  const idx = selectedImpacts.value.indexOf(impact)
  if (idx === -1) {
    selectedImpacts.value.push(impact)
  } else {
    selectedImpacts.value.splice(idx, 1)
  }
  currentPage.value = 1
}

function toggleTagFilter(tag: string) {
  const idx = selectedTags.value.indexOf(tag)
  if (idx === -1) {
    selectedTags.value.push(tag)
  } else {
    selectedTags.value.splice(idx, 1)
  }
  currentPage.value = 1
}

// Checkers
function isCategorySelected(category: string): boolean {
  return selectedCategories.value.includes(category)
}

function isStatusSelected(status: string): boolean {
  return selectedStatuses.value.includes(status)
}

function isImpactSelected(impact: string): boolean {
  return selectedImpacts.value.includes(impact)
}

function isTagSelected(tag: string): boolean {
  return selectedTags.value.includes(tag)
}

// Social
function toggleLessonLike(lesson: LessonLearned) {
  lesson.isLiked = !lesson.isLiked
  lesson.likeCount += lesson.isLiked ? 1 : -1
}

function toggleLessonBookmark(lesson: LessonLearned) {
  lesson.isBookmarked = !lesson.isBookmarked
}

// Clear all filters
function clearAllFilters() {
  searchQuery.value = ''
  selectedCategory.value = 'all'
  selectedStatus.value = 'all'
  selectedImpact.value = 'all'
  selectedCategories.value = []
  selectedStatuses.value = []
  selectedImpacts.value = []
  selectedTags.value = []
  showBookmarkedOnly.value = false
  lessonsSortBy.value = 'newest'
  lessonsSortOrder.value = 'desc'
  currentPage.value = 1
}

// Featured carousel lesson detail
function openLessonDetail(lesson: LessonLearned) {
  selectedLessonForModal.value = lesson
  showLessonDetailModal.value = true
}

function closeLessonDetail() {
  showLessonDetailModal.value = false
  selectedLessonForModal.value = null
}

// Category info helper
function getCategoryInfo(category: string) {
  return categoryOptions.find(c => c.id === category) || { id: category, label: category, icon: 'fas fa-tag', color: 'text-gray-600' }
}

// Export functions (LL-20)
function exportToCSV() {
  const headers = ['Title', 'Category', 'Impact', 'Status', 'Author', 'Project', 'Phase', 'Created', 'Views', 'Likes', 'Actions Total', 'Actions Completed', 'Actions Overdue']
  const rows = filteredLessons.value.map(l => [
    l.title, l.category, l.impact, l.status, l.isAnonymous ? 'Anonymous' : l.authorName,
    l.projectName || '', l.projectPhase || '', l.createdAt, l.viewCount, l.likeCount,
    l.totalActions, l.completedActions, l.overdueActions
  ])
  const csv = [headers.join(','), ...rows.map(r => r.map(v => `"${String(v).replace(/"/g, '""')}"`).join(','))].join('\n')
  const blob = new Blob([csv], { type: 'text/csv' })
  const url = URL.createObjectURL(blob)
  const a = document.createElement('a')
  a.href = url
  a.download = `lessons-learned-${new Date().toISOString().split('T')[0]}.csv`
  a.click()
  URL.revokeObjectURL(url)
}

function exportLessonPDF(lesson: LessonLearned) {
  const printWindow = window.open('', '_blank')
  if (!printWindow) return
  const actionsHtml = lesson.actions?.length ? `
    <h3 style="font-size: 14px; font-weight: 700; color: #0d9488; margin: 20px 0 10px; border-bottom: 1px solid #e5e7eb; padding-bottom: 6px;">Action Items (${lesson.completedActions}/${lesson.totalActions} completed)</h3>
    <table style="width: 100%; border-collapse: collapse; font-size: 12px;">
      <thead><tr style="background: #f9fafb;">
        <th style="text-align: left; padding: 8px; border: 1px solid #e5e7eb;">Description</th>
        <th style="text-align: left; padding: 8px; border: 1px solid #e5e7eb;">Assignee</th>
        <th style="text-align: left; padding: 8px; border: 1px solid #e5e7eb;">Status</th>
        <th style="text-align: left; padding: 8px; border: 1px solid #e5e7eb;">Due Date</th>
        <th style="text-align: left; padding: 8px; border: 1px solid #e5e7eb;">Document</th>
      </tr></thead>
      <tbody>${lesson.actions.map(a => `<tr>
        <td style="padding: 8px; border: 1px solid #e5e7eb;">${a.description}</td>
        <td style="padding: 8px; border: 1px solid #e5e7eb;">${a.assigneeName}</td>
        <td style="padding: 8px; border: 1px solid #e5e7eb;${a.isOverdue ? ' color: #dc2626; font-weight: 600;' : ''}">${a.status}${a.isOverdue ? ' (OVERDUE)' : ''}</td>
        <td style="padding: 8px; border: 1px solid #e5e7eb;">${a.dueDate}</td>
        <td style="padding: 8px; border: 1px solid #e5e7eb;">${a.affectedDocumentTitle ? a.affectedDocumentTitle + (a.affectedDocumentType ? ' (' + a.affectedDocumentType + ')' : '') : '—'}</td>
      </tr>`).join('')}</tbody>
    </table>` : ''
  const recsHtml = lesson.recommendations?.length ? `
    <h3 style="font-size: 14px; font-weight: 700; color: #0d9488; margin: 20px 0 10px; border-bottom: 1px solid #e5e7eb; padding-bottom: 6px;">Recommendations</h3>
    <ul style="margin: 0; padding-left: 20px;">${lesson.recommendations.map(r => `<li style="font-size: 12px; color: #374151; margin-bottom: 4px;">${r}</li>`).join('')}</ul>` : ''
  printWindow.document.write(`<!DOCTYPE html><html><head><title>${lesson.title}</title><style>body{font-family:-apple-system,BlinkMacSystemFont,sans-serif;padding:40px;max-width:900px;margin:0 auto;}@media print{body{padding:20px;}}</style></head><body>
    <div style="display: flex; gap: 8px; margin-bottom: 12px;">
      <span style="background: #f0fdfa; color: #0d9488; padding: 4px 12px; border-radius: 12px; font-size: 12px; font-weight: 600;">${lesson.status}</span>
      <span style="background: #fef3c7; color: #92400e; padding: 4px 12px; border-radius: 12px; font-size: 12px; font-weight: 600;">${lesson.impact} Impact</span>
      <span style="background: #f3f4f6; color: #374151; padding: 4px 12px; border-radius: 12px; font-size: 12px; font-weight: 600;">${lesson.category}</span>
      ${lesson.projectPhase ? `<span style="background: #e0e7ff; color: #4338ca; padding: 4px 12px; border-radius: 12px; font-size: 12px; font-weight: 600;">${lesson.projectPhase}</span>` : ''}
    </div>
    <h1 style="font-size: 22px; color: #111827; margin: 0 0 8px;">${lesson.title}</h1>
    <p style="font-size: 13px; color: #6b7280; margin: 0 0 16px;">${lesson.summary || lesson.description}</p>
    <div style="display: flex; gap: 16px; font-size: 12px; color: #9ca3af; margin-bottom: 20px; padding-bottom: 12px; border-bottom: 1px solid #e5e7eb;">
      <span>Author: ${lesson.isAnonymous ? 'Anonymous' : lesson.authorName}</span>
      <span>Project: ${lesson.projectName || 'N/A'}</span>
      ${lesson.processOwnerName ? `<span>Process Owner: ${lesson.processOwnerName}</span>` : ''}
      <span>Created: ${lesson.createdAt}</span>
    </div>
    ${lesson.context ? `<div style="margin-bottom: 14px;"><strong style="font-size: 13px; color: #2563eb;">Context</strong><p style="font-size: 12px; color: #4b5563; margin: 4px 0 0; line-height: 1.6;">${lesson.context}</p></div>` : ''}
    ${lesson.challenge ? `<div style="margin-bottom: 14px;"><strong style="font-size: 13px; color: #dc2626;">Challenge</strong><p style="font-size: 12px; color: #4b5563; margin: 4px 0 0; line-height: 1.6;">${lesson.challenge}</p></div>` : ''}
    ${lesson.whatWentWell ? `<div style="margin-bottom: 14px;"><strong style="font-size: 13px; color: #16a34a;">What Went Well</strong><p style="font-size: 12px; color: #4b5563; margin: 4px 0 0; line-height: 1.6;">${lesson.whatWentWell}</p></div>` : ''}
    ${lesson.solution ? `<div style="margin-bottom: 14px;"><strong style="font-size: 13px; color: #d97706;">Solution</strong><p style="font-size: 12px; color: #4b5563; margin: 4px 0 0; line-height: 1.6;">${lesson.solution}</p></div>` : ''}
    ${lesson.outcome ? `<div style="margin-bottom: 14px;"><strong style="font-size: 13px; color: #0d9488;">Outcome</strong><p style="font-size: 12px; color: #4b5563; margin: 4px 0 0; line-height: 1.6;">${lesson.outcome}</p></div>` : ''}
    ${lesson.rootCause ? `<div style="margin-bottom: 14px; padding: 12px; background: #fef2f2; border-radius: 8px; border-left: 4px solid #dc2626;"><strong style="font-size: 13px; color: #dc2626;">Root Cause${lesson.rootCauseMethod ? ' (' + (rootCauseMethodLabels[lesson.rootCauseMethod] || lesson.rootCauseMethod) + ')' : ''}</strong><p style="font-size: 12px; color: #4b5563; margin: 4px 0 0; line-height: 1.6;">${lesson.rootCause}</p></div>` : ''}
    ${recsHtml}
    ${actionsHtml}
    ${lesson.tags.length ? `<div style="margin-top: 16px; display: flex; gap: 6px; flex-wrap: wrap;">${lesson.tags.map(t => `<span style="background: #f0fdfa; color: #0d9488; padding: 2px 10px; border-radius: 10px; font-size: 11px; font-weight: 500;">${t}</span>`).join('')}</div>` : ''}
    <div style="margin-top: 20px; padding-top: 12px; border-top: 1px solid #e5e7eb; font-size: 10px; color: #9ca3af;">Generated on ${new Date().toLocaleDateString()} | Views: ${lesson.viewCount} | Useful: ${lesson.usefulCount} | Likes: ${lesson.likeCount}</div>
  </body></html>`)
  printWindow.document.close()
  setTimeout(() => printWindow.print(), 500)
}

function exportToPDF() {
  // Client-side print-friendly view
  const printWindow = window.open('', '_blank')
  if (!printWindow) return
  const lessonsHtml = filteredLessons.value.map(l => `
    <div style="page-break-inside: avoid; margin-bottom: 24px; padding: 16px; border: 1px solid #e5e7eb; border-radius: 8px;">
      <div style="display: flex; gap: 8px; margin-bottom: 8px;">
        <span style="background: #f0fdfa; color: #0d9488; padding: 2px 8px; border-radius: 12px; font-size: 12px; font-weight: 600;">${l.status}</span>
        <span style="background: #fef3c7; color: #92400e; padding: 2px 8px; border-radius: 12px; font-size: 12px; font-weight: 600;">${l.impact} Impact</span>
        <span style="background: #f3f4f6; color: #374151; padding: 2px 8px; border-radius: 12px; font-size: 12px; font-weight: 600;">${l.category}</span>
      </div>
      <h3 style="font-size: 16px; font-weight: 700; margin: 0 0 4px;">${l.title}</h3>
      <p style="font-size: 13px; color: #6b7280; margin: 0 0 8px;">${l.summary || l.description}</p>
      ${l.context ? `<div style="margin-bottom: 6px;"><strong style="font-size: 12px; color: #374151;">Context:</strong> <span style="font-size: 12px; color: #6b7280;">${l.context}</span></div>` : ''}
      ${l.challenge ? `<div style="margin-bottom: 6px;"><strong style="font-size: 12px; color: #374151;">Challenge:</strong> <span style="font-size: 12px; color: #6b7280;">${l.challenge}</span></div>` : ''}
      ${l.solution ? `<div style="margin-bottom: 6px;"><strong style="font-size: 12px; color: #374151;">Solution:</strong> <span style="font-size: 12px; color: #6b7280;">${l.solution}</span></div>` : ''}
      ${l.outcome ? `<div style="margin-bottom: 6px;"><strong style="font-size: 12px; color: #374151;">Outcome:</strong> <span style="font-size: 12px; color: #6b7280;">${l.outcome}</span></div>` : ''}
      ${l.rootCause ? `<div style="margin-bottom: 6px;"><strong style="font-size: 12px; color: #dc2626;">Root Cause:</strong> <span style="font-size: 12px; color: #6b7280;">${l.rootCause}</span></div>` : ''}
      <div style="font-size: 11px; color: #9ca3af; margin-top: 8px;">
        Author: ${l.isAnonymous ? 'Anonymous' : l.authorName} | Project: ${l.projectName || 'N/A'} | Created: ${l.createdAt} | Views: ${l.viewCount} | Actions: ${l.completedActions}/${l.totalActions}
      </div>
    </div>
  `).join('')
  printWindow.document.write(`<!DOCTYPE html><html><head><title>Lessons Learned Report</title><style>body{font-family:-apple-system,BlinkMacSystemFont,sans-serif;padding:40px;max-width:900px;margin:0 auto;}h1{color:#0d9488;border-bottom:2px solid #0d9488;padding-bottom:8px;}@media print{body{padding:20px;}}</style></head><body><h1>Lessons Learned Report</h1><p style="color:#6b7280;margin-bottom:24px;">Generated on ${new Date().toLocaleDateString()} | ${filteredLessons.value.length} lessons</p>${lessonsHtml}</body></html>`)
  printWindow.document.close()
  setTimeout(() => printWindow.print(), 500)
}

// Proactive dissemination (LL-9)
const showDisseminateModal = ref(false)
const disseminateLesson = ref<LessonLearned | null>(null)
const disseminateTeams = ref<string[]>([])
const availableTeams = ref([
  { id: 'operations', name: 'Operations', icon: 'fas fa-cogs' },
  { id: 'engineering', name: 'Engineering', icon: 'fas fa-code' },
  { id: 'safety', name: 'Safety & Security', icon: 'fas fa-shield-alt' },
  { id: 'procurement', name: 'Procurement', icon: 'fas fa-shopping-cart' },
  { id: 'project-mgmt', name: 'Project Management', icon: 'fas fa-tasks' },
  { id: 'communications', name: 'Communications', icon: 'fas fa-bullhorn' },
  { id: 'it', name: 'IT & Infrastructure', icon: 'fas fa-server' },
  { id: 'hr', name: 'Human Resources', icon: 'fas fa-users' }
])

function openDisseminate(lesson: LessonLearned) {
  disseminateLesson.value = lesson
  disseminateTeams.value = []
  showDisseminateModal.value = true
}

function sendDissemination() {
  if (disseminateTeams.value.length === 0) return
  // In production this would call an API
  showDisseminateModal.value = false
  disseminateLesson.value = null
}

// Periodic review sessions (LL-11)
const showReviewSessionModal = ref(false)
const showSessionsListModal = ref(false)
const reviewSession = ref({
  title: '',
  type: 'pause-and-learn',
  date: '',
  project: '',
  facilitator: '',
  participants: '',
  notes: ''
})

interface ReviewSession {
  id: string
  title: string
  type: string
  date: string
  project: string
  facilitator: string
  participants: string[]
  status: 'Scheduled' | 'In Progress' | 'Completed' | 'Cancelled'
  lessonsCount: number
  createdAt: string
}

const scheduledSessions = ref<ReviewSession[]>([
  { id: 's1', title: 'Q1 Safety & Operations Review', type: 'after-action-review', date: '2026-03-25', project: 'Match Operations', facilitator: 'Omar Al-Farsi', participants: ['Mohammed Al-Rashid', 'Sara Ali', 'Khalid Al-Mansoori'], status: 'Scheduled', lessonsCount: 0, createdAt: '2026-03-15' },
  { id: 's2', title: 'Venue Setup Retrospective', type: 'retrospective', date: '2026-03-10', project: 'Venue Setup', facilitator: 'Fatima Khalil', participants: ['Sara Ali', 'Ahmed Hassan'], status: 'Completed', lessonsCount: 3, createdAt: '2026-03-05' }
])

const sessionTypes = [
  { id: 'pause-and-learn', label: 'Pause and Learn', icon: 'fas fa-pause-circle', description: 'Mid-project reflection session' },
  { id: 'after-action-review', label: 'After Action Review', icon: 'fas fa-clipboard-check', description: 'Post-event/milestone review' },
  { id: 'retrospective', label: 'Retrospective', icon: 'fas fa-redo', description: 'Sprint/iteration review' },
  { id: 'knowledge-cafe', label: 'Knowledge Café', icon: 'fas fa-coffee', description: 'Open knowledge sharing session' }
]

function getSessionTypeInfo(typeId: string) {
  return sessionTypes.find(s => s.id === typeId) || sessionTypes[0]
}

const sessionStatusConfig: Record<string, { color: string; icon: string }> = {
  Scheduled: { color: 'bg-blue-100 text-blue-700', icon: 'fas fa-calendar' },
  'In Progress': { color: 'bg-amber-100 text-amber-700', icon: 'fas fa-play-circle' },
  Completed: { color: 'bg-green-100 text-green-700', icon: 'fas fa-check-circle' },
  Cancelled: { color: 'bg-gray-100 text-gray-500', icon: 'fas fa-ban' }
}

function scheduleReviewSession() {
  if (!reviewSession.value.title || !reviewSession.value.date) return
  const participantsList = reviewSession.value.participants ? reviewSession.value.participants.split(',').map(p => p.trim()).filter(Boolean) : []
  scheduledSessions.value.unshift({
    id: 's' + (scheduledSessions.value.length + 1),
    title: reviewSession.value.title,
    type: reviewSession.value.type,
    date: reviewSession.value.date,
    project: reviewSession.value.project,
    facilitator: reviewSession.value.facilitator,
    participants: participantsList,
    status: 'Scheduled',
    lessonsCount: 0,
    createdAt: new Date().toISOString().split('T')[0]
  })
  showReviewSessionModal.value = false
  showSessionsListModal.value = true
  reviewSession.value = { title: '', type: 'pause-and-learn', date: '', project: '', facilitator: '', participants: '', notes: '' }
}

function startSession(session: ReviewSession) {
  session.status = 'In Progress'
}

function completeSession(session: ReviewSession) {
  session.status = 'Completed'
}

function cancelSession(session: ReviewSession) {
  if (!confirm('Cancel this session?')) return
  session.status = 'Cancelled'
}

function captureLessonFromSession(session: ReviewSession) {
  showSessionsListModal.value = false
  newLesson.value.projectName = session.project
  newLesson.value.context = `Captured during "${session.title}" (${getSessionTypeInfo(session.type).label}) on ${session.date}`
  showCreateModal.value = true
}

// AI-assisted lesson capture (LL-16)
const isAISuggesting = ref(false)
const aiSuggestions = ref<{ field: string; suggestion: string }[]>([])

async function getAISuggestions() {
  if (!newLesson.value.challenge) return
  isAISuggesting.value = true
  // Simulate AI processing
  await new Promise(resolve => setTimeout(resolve, 1500))
  aiSuggestions.value = []
  if (newLesson.value.challenge.toLowerCase().includes('communication') || newLesson.value.challenge.toLowerCase().includes('delay')) {
    aiSuggestions.value.push(
      { field: 'rootCause', suggestion: 'Lack of structured communication channels between stakeholder groups led to information asymmetry and delayed decision-making.' },
      { field: 'category', suggestion: 'Communication' },
      { field: 'solution', suggestion: 'Establish a centralized communication hub with defined escalation paths, regular status cadence, and stakeholder-specific dashboards.' }
    )
  } else if (newLesson.value.challenge.toLowerCase().includes('security') || newLesson.value.challenge.toLowerCase().includes('vulnerability')) {
    aiSuggestions.value.push(
      { field: 'rootCause', suggestion: 'Insufficient security review coverage in the development lifecycle allowed vulnerabilities to reach production.' },
      { field: 'category', suggestion: 'Technical' },
      { field: 'solution', suggestion: 'Implement mandatory security reviews at each SDLC stage with automated SAST/DAST scanning in CI/CD pipeline.' }
    )
  } else {
    aiSuggestions.value.push(
      { field: 'rootCause', suggestion: 'Process gaps in planning phase led to insufficient risk assessment and resource allocation.' },
      { field: 'solution', suggestion: 'Introduce structured risk assessment templates and mandatory review checkpoints at each project phase gate.' }
    )
  }
  isAISuggesting.value = false
}

function applyAISuggestion(suggestion: { field: string; suggestion: string }) {
  if (suggestion.field === 'category') {
    newLesson.value.category = suggestion.suggestion
  } else if (suggestion.field === 'solution') {
    newLesson.value.solution = suggestion.suggestion
  }
  aiSuggestions.value = aiSuggestions.value.filter(s => s !== suggestion)
}

// Project kickoff integration (LL-10)
const showRelatedLessonsPanel = ref(false)

const relatedLessonsForProject = computed(() => {
  if (!newLesson.value.projectName && !newLesson.value.category) return []
  return lessons.value.filter(l => {
    if (newLesson.value.projectName && l.projectName?.toLowerCase().includes(newLesson.value.projectName.toLowerCase())) return true
    if (newLesson.value.category && l.category === newLesson.value.category) return true
    return false
  }).slice(0, 5)
})

// Follow/Watch (LL-19)
const followedLessons = ref<Set<string>>(new Set())

function toggleFollow(lessonId: string) {
  if (followedLessons.value.has(lessonId)) {
    followedLessons.value.delete(lessonId)
  } else {
    followedLessons.value.add(lessonId)
  }
}

function isFollowing(lessonId: string) {
  return followedLessons.value.has(lessonId)
}

// Notification preferences (LL-17)
const showNotificationPrefsModal = ref(false)
const showExportMenu = ref(false)
const notificationPrefs = ref({
  onSubmitted: true,
  onApproved: true,
  onRejected: true,
  onPublished: true,
  onActionAssigned: true,
  onActionOverdue: true,
  onActionsComplete: true,
  onCommentAdded: true,
  onFollowedUpdate: true
})

// ============================================
// LIFECYCLE
// ============================================
onMounted(() => {
  startFeaturedLessonAutoPlay()
  // Handle deep-links from Events page
  const action = route.query.action as string
  const project = route.query.project as string
  if (action === 'new-lesson') {
    newLesson.value.projectName = project || ''
    showCreateModal.value = true
  } else if (action === 'schedule-review') {
    reviewSession.value.project = project || ''
    showReviewSessionModal.value = true
  }
})

onUnmounted(() => {
  if (featuredInterval.value) {
    clearInterval(featuredInterval.value)
    featuredInterval.value = null
  }
})
</script>

<template>
  <div class="page-view">
    <!-- Hero Header -->
    <PageHeroHeader
      :stats="[
        { icon: 'fas fa-lightbulb', value: String(analytics.totalLessons), label: 'Lessons' },
        { icon: 'fas fa-check-double', value: analytics.actionCompletionRate + '%', label: 'Completion' },
        { icon: 'fas fa-tasks', value: String(analytics.completedActions), label: 'Actions Done' },
        { icon: 'fas fa-heart', value: String(lessonsStats.totalLikes), label: 'Likes' }
      ]"
      badge-icon="fas fa-lightbulb"
      badge-text="Lessons Learned"
      title="Lessons Learned"
      subtitle="Capture, track, and apply organizational knowledge from project experiences"
    >
      <template #actions>
        <button @click="showCreateModal = true" class="px-5 py-2.5 bg-white text-teal-600 rounded-xl font-semibold text-sm flex items-center gap-2 hover:bg-teal-50 transition-all shadow-lg">
          <i class="fas fa-plus"></i>New Lesson
        </button>
      </template>
    </PageHeroHeader>

    <div class="px-6 py-6">
      <!-- View Tabs -->
      <div class="flex items-center gap-2 mb-6">
        <button @click="currentView = 'list'" :class="[currentView === 'list' ? 'bg-teal-500 text-white shadow-md' : 'bg-white text-gray-600 border border-gray-200 hover:bg-gray-50', 'px-5 py-2 rounded-lg text-sm font-semibold transition-all']">
          <i class="fas fa-list-ul mr-2"></i>Lessons
        </button>
        <button @click="currentView = 'analytics'" :class="[currentView === 'analytics' ? 'bg-teal-500 text-white shadow-md' : 'bg-white text-gray-600 border border-gray-200 hover:bg-gray-50', 'px-5 py-2 rounded-lg text-sm font-semibold transition-all']">
          <i class="fas fa-chart-bar mr-2"></i>Analytics
        </button>
        <div class="ml-auto flex items-center gap-2">
          <button @click="showSessionsListModal = true" class="px-4 py-2 rounded-lg text-sm font-medium bg-white text-gray-600 border border-gray-200 hover:bg-teal-50 hover:text-teal-600 hover:border-teal-200 transition-all relative">
            <i class="fas fa-users mr-1.5"></i>Sessions
            <span v-if="scheduledSessions.filter(s => s.status === 'Scheduled').length > 0" class="absolute -top-1 -right-1 w-5 h-5 bg-purple-500 text-white text-[10px] font-bold rounded-full flex items-center justify-center">{{ scheduledSessions.filter(s => s.status === 'Scheduled').length }}</span>
          </button>
          <button @click="showNotificationPrefsModal = true" class="px-4 py-2 rounded-lg text-sm font-medium bg-white text-gray-600 border border-gray-200 hover:bg-teal-50 hover:text-teal-600 hover:border-teal-200 transition-all" title="Notification Preferences">
            <i class="fas fa-bell mr-1.5"></i>Alerts
          </button>
          <div class="relative export-dropdown">
            <button @click="showExportMenu = !showExportMenu" class="px-4 py-2 rounded-lg text-sm font-medium bg-white text-gray-600 border border-gray-200 hover:bg-teal-50 hover:text-teal-600 hover:border-teal-200 transition-all">
              <i class="fas fa-download mr-1.5"></i>Export
              <i class="fas fa-chevron-down text-[10px] ml-1"></i>
            </button>
            <div v-if="showExportMenu" class="absolute right-0 top-full mt-1 w-48 bg-white rounded-xl shadow-lg border border-gray-100 py-1 z-50">
              <button @click="exportToCSV(); showExportMenu = false" class="w-full px-4 py-2.5 text-left text-sm text-gray-700 hover:bg-teal-50 hover:text-teal-700 flex items-center gap-2.5">
                <i class="fas fa-file-csv text-green-500"></i>Export as CSV
              </button>
              <button @click="exportToPDF(); showExportMenu = false" class="w-full px-4 py-2.5 text-left text-sm text-gray-700 hover:bg-teal-50 hover:text-teal-700 flex items-center gap-2.5">
                <i class="fas fa-file-pdf text-red-500"></i>Export as PDF
              </button>
            </div>
            <div v-if="showExportMenu" @click="showExportMenu = false" class="fixed inset-0 z-40"></div>
          </div>
        </div>
      </div>

      <!-- Lessons View -->
      <div v-if="currentView === 'list'">

          <!-- Featured Insights Section -->
          <div v-if="featuredLessons.length > 0 && activeLessonsFiltersCount === 0" class="bg-white rounded-2xl border border-gray-100 shadow-sm p-6 mb-6" @mouseenter="pauseFeaturedLessonAutoPlay" @mouseleave="resumeFeaturedLessonAutoPlay">
            <!-- Section Header -->
            <div class="flex items-center justify-between mb-5">
              <div class="flex items-center gap-3">
                <div class="w-12 h-12 rounded-xl bg-gradient-to-br from-teal-500 to-teal-600 flex items-center justify-center text-white shadow-lg shadow-teal-200">
                  <i class="fas fa-sparkles text-lg"></i>
                </div>
                <div>
                  <h2 class="text-lg font-bold text-gray-900">Featured Insights</h2>
                  <p class="text-sm text-gray-500">Discover top lessons from across the organization</p>
                </div>
              </div>
              <div class="flex items-center gap-3">
                <span class="text-sm font-semibold text-teal-600 bg-teal-50 px-3 py-1 rounded-full">{{ currentFeaturedIndex + 1 }} / {{ featuredLessons.length }}</span>
                <button @click="prevFeaturedLesson" class="w-9 h-9 rounded-lg border border-gray-200 bg-white hover:bg-gradient-to-br hover:from-teal-500 hover:to-teal-600 hover:border-transparent hover:text-white hover:scale-105 flex items-center justify-center text-gray-500 transition-all">
                  <i class="fas fa-chevron-left text-xs"></i>
                </button>
                <button @click="nextFeaturedLesson" class="w-9 h-9 rounded-lg border border-gray-200 bg-white hover:bg-gradient-to-br hover:from-teal-500 hover:to-teal-600 hover:border-transparent hover:text-white hover:scale-105 flex items-center justify-center text-gray-500 transition-all">
                  <i class="fas fa-chevron-right text-xs"></i>
                </button>
              </div>
            </div>

            <!-- Featured Content: Two Column Layout -->
            <div v-if="currentFeaturedLesson" class="featured-grid">
              <!-- Main Featured Card -->
              <div class="featured-main flex flex-col gap-4">
                <div class="relative rounded-2xl overflow-hidden cursor-pointer transition-all duration-400 hover:-translate-y-1.5 hover:shadow-2xl" style="background: linear-gradient(135deg, #0f766e 0%, #0d9488 50%, #14b8a6 100%); min-height: 420px;" @click="openLessonDetail(currentFeaturedLesson)">
                  <!-- Decorative Pattern -->
                  <div class="absolute inset-0 opacity-5" style="background-image: url('data:image/svg+xml,%3Csvg width=&quot;60&quot; height=&quot;60&quot; viewBox=&quot;0 0 60 60&quot; xmlns=&quot;http://www.w3.org/2000/svg&quot;%3E%3Cg fill=&quot;none&quot; fill-rule=&quot;evenodd&quot;%3E%3Cg fill=&quot;%23ffffff&quot; fill-opacity=&quot;1&quot;%3E%3Cpath d=&quot;M36 34v-4h-2v4h-4v2h4v4h2v-4h4v-2h-4zm0-30V0h-2v4h-4v2h4v4h2V6h4V4h-4zM6 34v-4H4v4H0v2h4v4h2v-4h4v-2H6zM6 4V0H4v4H0v2h4v4h2V6h4V4H6z&quot;/%3E%3C/g%3E%3C/g%3E%3C/svg%3E');"></div>
                  <div class="absolute top-0 right-0 w-[60%] h-full opacity-15" style="background: radial-gradient(circle at top right, white 0%, transparent 70%);"></div>

                  <div class="relative z-10 p-8 flex flex-col h-full text-white" style="min-height: 420px;">
                    <!-- Badges -->
                    <div class="flex flex-wrap items-center gap-2.5 mb-5">
                      <span class="px-3 py-1.5 bg-white/20 backdrop-blur-md rounded-full text-sm font-semibold">
                        <i class="fas fa-star text-yellow-300 mr-1.5"></i>Featured
                      </span>
                      <span :class="[currentFeaturedLesson.impact === 'Critical' ? 'bg-red-500/90' : currentFeaturedLesson.impact === 'High' ? 'bg-orange-500/90' : currentFeaturedLesson.impact === 'Medium' ? 'bg-amber-500/90' : 'bg-green-500/90', 'px-3 py-1.5 rounded-full text-sm font-semibold']">
                        <i class="fas fa-flag mr-1.5"></i>{{ currentFeaturedLesson.impact }}
                      </span>
                      <span class="px-3 py-1.5 bg-white/15 backdrop-blur-sm rounded-full text-sm font-medium">
                        <i :class="[getCategoryInfo(currentFeaturedLesson.category).icon, 'mr-1.5']"></i>
                        {{ getCategoryInfo(currentFeaturedLesson.category).label }}
                      </span>
                    </div>

                    <!-- Title & Summary -->
                    <h3 class="text-2xl font-bold mb-3 leading-tight">{{ currentFeaturedLesson.title }}</h3>
                    <p class="text-white/85 text-base mb-5 leading-relaxed line-clamp-2">{{ currentFeaturedLesson.summary }}</p>

                    <!-- Impact Highlight -->
                    <div v-if="currentFeaturedLesson.outcome" class="flex items-start gap-3 bg-white/12 backdrop-blur-sm rounded-xl p-4 mb-5 border border-white/10">
                      <div class="w-10 h-10 rounded-lg bg-white/20 flex items-center justify-center shrink-0">
                        <i class="fas fa-bolt"></i>
                      </div>
                      <div>
                        <div class="text-xs font-semibold uppercase tracking-wider text-white/70 mb-1">Key Impact</div>
                        <p class="text-sm text-white/90 leading-relaxed">{{ currentFeaturedLesson.outcome }}</p>
                      </div>
                    </div>

                    <!-- Recommendations Preview -->
                    <div v-if="currentFeaturedLesson.recommendations?.length" class="mb-5">
                      <div class="flex items-center gap-2 text-sm font-semibold text-white/80 mb-3">
                        <i class="fas fa-lightbulb text-yellow-300"></i>Quick Takeaways
                      </div>
                      <div class="space-y-2">
                        <div v-for="(rec, idx) in currentFeaturedLesson.recommendations.slice(0, 2)" :key="idx" class="flex items-start gap-3 bg-white/10 rounded-lg px-3.5 py-2.5">
                          <span class="w-6 h-6 rounded-full bg-white/20 flex items-center justify-center text-xs font-bold shrink-0">{{ idx + 1 }}</span>
                          <span class="text-sm leading-snug">{{ rec }}</span>
                        </div>
                      </div>
                    </div>

                    <!-- Footer -->
                    <div class="mt-auto pt-5 border-t border-white/15 flex items-center justify-between">
                      <div class="flex items-center gap-3">
                        <div class="w-11 h-11 rounded-xl bg-white/20 flex items-center justify-center text-sm font-semibold">
                          {{ currentFeaturedLesson.authorInitials }}
                        </div>
                        <div>
                          <div class="text-sm font-semibold">{{ currentFeaturedLesson.authorName }}</div>
                          <div class="text-xs text-white/70">{{ currentFeaturedLesson.authorDepartment }}</div>
                        </div>
                      </div>
                      <div class="flex items-center gap-4 text-sm text-white/80">
                        <span><i class="fas fa-eye mr-1"></i>{{ currentFeaturedLesson.viewCount }}</span>
                        <span><i class="fas fa-heart mr-1"></i>{{ currentFeaturedLesson.likeCount }}</span>
                        <span><i class="fas fa-comment mr-1"></i>{{ currentFeaturedLesson.commentCount }}</span>
                      </div>
                    </div>

                    <!-- CTA -->
                    <button class="mt-4 inline-flex items-center justify-center gap-2 px-5 py-2.5 bg-white border-none rounded-lg text-sm font-semibold text-teal-700 hover:bg-teal-50 hover:-translate-y-0.5 hover:shadow-lg transition-all">
                      <span>Read Full Lesson</span>
                      <i class="fas fa-arrow-right"></i>
                    </button>
                  </div>
                </div>

                <!-- Progress Dots -->
                <div class="flex items-center justify-center gap-2">
                  <button v-for="(_, idx) in featuredLessons" :key="idx" @click="goToFeaturedLesson(idx)" :class="[idx === currentFeaturedIndex ? 'w-7 bg-gradient-to-r from-teal-500 to-teal-600' : 'w-2.5 bg-gray-200 hover:bg-gray-300', 'h-2.5 rounded-full transition-all duration-300']"></button>
                </div>
              </div>

              <!-- Up Next Sidebar -->
              <div class="featured-sidebar flex-col bg-gray-50 rounded-2xl p-5 border border-gray-100">
                <div class="flex items-center gap-2.5 text-sm font-bold text-gray-900 mb-4 pb-3 border-b border-gray-200">
                  <i class="fas fa-layer-group text-teal-500"></i>
                  <span>Up Next</span>
                </div>

                <div class="flex flex-col gap-3 flex-1">
                  <div v-for="(nextLesson, idx) in nextFeaturedLessons" :key="nextLesson.id" @click="goToFeaturedLesson((currentFeaturedIndex + idx + 1) % featuredLessons.length)" class="group flex items-start gap-3.5 p-4 bg-white rounded-xl border border-gray-100 cursor-pointer transition-all hover:border-teal-300 hover:shadow-md hover:translate-x-1">
                    <div class="w-7 h-7 rounded-lg bg-gradient-to-br from-teal-500 to-teal-600 text-white flex items-center justify-center text-xs font-bold shrink-0">
                      {{ idx + 1 }}
                    </div>
                    <div class="flex-1 min-w-0">
                      <div class="flex items-center gap-2 mb-1.5">
                        <span class="text-[11px] text-gray-400"><i :class="[getCategoryInfo(nextLesson.category).icon, 'mr-1 text-teal-500']"></i>{{ getCategoryInfo(nextLesson.category).label }}</span>
                        <span :class="[nextLesson.impact === 'Critical' ? 'bg-red-100 text-red-700' : nextLesson.impact === 'High' ? 'bg-orange-100 text-orange-700' : nextLesson.impact === 'Medium' ? 'bg-amber-100 text-amber-700' : 'bg-green-100 text-green-700', 'px-1.5 py-0.5 rounded text-[10px] font-semibold']">
                          {{ nextLesson.impact }}
                        </span>
                      </div>
                      <h4 class="text-sm font-semibold text-gray-900 line-clamp-2 group-hover:text-teal-600 transition-colors leading-snug">{{ nextLesson.title }}</h4>
                      <p class="text-xs text-gray-500 line-clamp-2 mt-1 leading-relaxed">{{ nextLesson.summary }}</p>
                      <div class="flex items-center justify-between mt-2">
                        <span class="text-xs text-gray-400 flex items-center gap-1.5">
                          <span class="w-5 h-5 rounded-md bg-gradient-to-br from-teal-400 to-teal-500 text-white flex items-center justify-center text-[9px] font-semibold">{{ nextLesson.authorInitials }}</span>
                          {{ nextLesson.authorName }}
                        </span>
                        <span class="text-xs text-gray-400"><i class="fas fa-eye mr-1"></i>{{ nextLesson.viewCount }}</span>
                      </div>
                    </div>
                    <div class="w-7 h-7 rounded-lg bg-gray-100 flex items-center justify-center text-gray-400 group-hover:bg-teal-500 group-hover:text-white transition-all shrink-0 mt-1">
                      <i class="fas fa-chevron-right text-[10px]"></i>
                    </div>
                  </div>
                </div>

                <!-- View All -->
                <button class="mt-3 w-full flex items-center justify-center gap-2 py-3 border border-teal-500 text-teal-600 bg-white hover:bg-teal-500 hover:text-white rounded-xl text-sm font-semibold transition-all">
                  <span>View All Featured</span>
                  <i class="fas fa-arrow-right text-xs"></i>
                </button>
              </div>
            </div>
          </div>

          <!-- Filter Toolbar -->
          <div class="bg-white rounded-2xl shadow-sm border border-gray-100 mb-4">
            <div class="px-4 py-3 bg-gray-50/50 flex flex-wrap items-center gap-3">
              <!-- Search -->
              <div class="relative flex-1 max-w-xl">
                <i class="fas fa-search absolute start-3 top-1/2 -translate-y-1/2 text-gray-400 text-xs"></i>
                <input v-model="searchQuery" type="text" placeholder="Search lessons..." class="w-full ps-9 pe-8 py-1.5 rounded-lg border border-gray-200 text-sm focus:outline-none focus:ring-2 focus:ring-teal-200 focus:border-teal-300 bg-white" />
                <button v-if="searchQuery" @click="searchQuery = ''; currentPage = 1" class="absolute end-2 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600">
                  <i class="fas fa-times text-xs"></i>
                </button>
              </div>

              <!-- Category Dropdown -->
              <div class="relative">
                <button @click="showCategoryDropdown = !showCategoryDropdown" :class="[selectedCategories.length > 0 ? 'bg-teal-50 border-teal-200 text-teal-700' : 'bg-white border-gray-200 text-gray-600 hover:bg-gray-50', 'flex items-center gap-2 px-3 py-1.5 rounded-lg text-xs font-medium transition-all border']">
                  <i class="fas fa-layer-group"></i>
                  <span>Category</span>
                  <span v-if="selectedCategories.length > 0" class="bg-teal-500 text-white text-[10px] rounded-full w-4 h-4 flex items-center justify-center">{{ selectedCategories.length }}</span>
                  <i class="fas fa-chevron-down text-[10px]"></i>
                </button>
                <div v-if="showCategoryDropdown" @click="showCategoryDropdown = false" class="fixed inset-0 z-40"></div>
                <div v-if="showCategoryDropdown" class="absolute start-0 top-full mt-2 w-64 bg-white rounded-xl shadow-lg border border-gray-100 py-2 z-50">
                  <div class="px-3 py-2 border-b border-gray-100">
                    <div class="text-xs font-semibold text-gray-700">Filter by Category</div>
                  </div>
                  <div class="max-h-60 overflow-y-auto py-1">
                    <label v-for="cat in categoryOptions" :key="cat.id" class="flex items-center gap-3 px-3 py-2 hover:bg-gray-50 cursor-pointer">
                      <input type="checkbox" :checked="isCategorySelected(cat.id)" @change="toggleCategoryFilter(cat.id)" class="rounded border-gray-300 text-teal-600 focus:ring-teal-500" />
                      <i :class="[cat.icon, cat.color, 'text-sm']"></i>
                      <span class="text-sm text-gray-700 flex-1">{{ cat.label }}</span>
                      <span class="text-xs text-gray-400">{{ categoryStats[cat.id] || 0 }}</span>
                    </label>
                  </div>
                  <div class="px-3 py-2 border-t border-gray-100 flex items-center justify-between">
                    <button @click="selectedCategories = []; currentPage = 1" class="text-xs text-gray-500 hover:text-gray-700">Clear</button>
                    <button @click="showCategoryDropdown = false" class="text-xs font-medium text-teal-600 hover:text-teal-700">Apply</button>
                  </div>
                </div>
              </div>

              <!-- Status Dropdown -->
              <div class="relative">
                <button @click="showStatusDropdown = !showStatusDropdown" :class="[selectedStatuses.length > 0 ? 'bg-teal-50 border-teal-200 text-teal-700' : 'bg-white border-gray-200 text-gray-600 hover:bg-gray-50', 'flex items-center gap-2 px-3 py-1.5 rounded-lg text-xs font-medium transition-all border']">
                  <i class="fas fa-flag"></i>
                  <span>Status</span>
                  <span v-if="selectedStatuses.length > 0" class="bg-teal-500 text-white text-[10px] rounded-full w-4 h-4 flex items-center justify-center">{{ selectedStatuses.length }}</span>
                  <i class="fas fa-chevron-down text-[10px]"></i>
                </button>
                <div v-if="showStatusDropdown" @click="showStatusDropdown = false" class="fixed inset-0 z-40"></div>
                <div v-if="showStatusDropdown" class="absolute start-0 top-full mt-2 w-64 bg-white rounded-xl shadow-lg border border-gray-100 py-2 z-50">
                  <div class="px-3 py-2 border-b border-gray-100">
                    <div class="text-xs font-semibold text-gray-700">Filter by Status</div>
                  </div>
                  <div class="max-h-60 overflow-y-auto py-1">
                    <label v-for="(config, key) in statusConfig" :key="key" class="flex items-center gap-3 px-3 py-2 hover:bg-gray-50 cursor-pointer">
                      <input type="checkbox" :checked="isStatusSelected(String(key))" @change="toggleStatusFilter(String(key))" class="rounded border-gray-300 text-teal-600 focus:ring-teal-500" />
                      <i :class="[config.icon, 'text-sm']"></i>
                      <span class="text-sm text-gray-700 flex-1">{{ config.label }}</span>
                    </label>
                  </div>
                  <div class="px-3 py-2 border-t border-gray-100 flex items-center justify-between">
                    <button @click="selectedStatuses = []; currentPage = 1" class="text-xs text-gray-500 hover:text-gray-700">Clear</button>
                    <button @click="showStatusDropdown = false" class="text-xs font-medium text-teal-600 hover:text-teal-700">Apply</button>
                  </div>
                </div>
              </div>

              <!-- Impact Dropdown -->
              <div class="relative">
                <button @click="showImpactDropdown = !showImpactDropdown" :class="[selectedImpacts.length > 0 ? 'bg-teal-50 border-teal-200 text-teal-700' : 'bg-white border-gray-200 text-gray-600 hover:bg-gray-50', 'flex items-center gap-2 px-3 py-1.5 rounded-lg text-xs font-medium transition-all border']">
                  <i class="fas fa-bolt"></i>
                  <span>Impact</span>
                  <span v-if="selectedImpacts.length > 0" class="bg-teal-500 text-white text-[10px] rounded-full w-4 h-4 flex items-center justify-center">{{ selectedImpacts.length }}</span>
                  <i class="fas fa-chevron-down text-[10px]"></i>
                </button>
                <div v-if="showImpactDropdown" @click="showImpactDropdown = false" class="fixed inset-0 z-40"></div>
                <div v-if="showImpactDropdown" class="absolute start-0 top-full mt-2 w-64 bg-white rounded-xl shadow-lg border border-gray-100 py-2 z-50">
                  <div class="px-3 py-2 border-b border-gray-100">
                    <div class="text-xs font-semibold text-gray-700">Filter by Impact</div>
                  </div>
                  <div class="py-1">
                    <label v-for="(config, key) in impactConfig" :key="key" class="flex items-center gap-3 px-3 py-2 hover:bg-gray-50 cursor-pointer">
                      <input type="checkbox" :checked="isImpactSelected(String(key))" @change="toggleImpactFilter(String(key))" class="rounded border-gray-300 text-teal-600 focus:ring-teal-500" />
                      <span :class="[config.color, 'px-2 py-0.5 rounded text-xs font-semibold']">{{ config.label }}</span>
                    </label>
                  </div>
                  <div class="px-3 py-2 border-t border-gray-100 flex items-center justify-between">
                    <button @click="selectedImpacts = []; currentPage = 1" class="text-xs text-gray-500 hover:text-gray-700">Clear</button>
                    <button @click="showImpactDropdown = false" class="text-xs font-medium text-teal-600 hover:text-teal-700">Apply</button>
                  </div>
                </div>
              </div>

              <!-- Tags Dropdown -->
              <div class="relative">
                <button @click="showTagsDropdown = !showTagsDropdown" :class="[selectedTags.length > 0 ? 'bg-teal-50 border-teal-200 text-teal-700' : 'bg-white border-gray-200 text-gray-600 hover:bg-gray-50', 'flex items-center gap-2 px-3 py-1.5 rounded-lg text-xs font-medium transition-all border']">
                  <i class="fas fa-tags"></i>
                  <span>Tags</span>
                  <span v-if="selectedTags.length > 0" class="bg-teal-500 text-white text-[10px] rounded-full w-4 h-4 flex items-center justify-center">{{ selectedTags.length }}</span>
                  <i class="fas fa-chevron-down text-[10px]"></i>
                </button>
                <div v-if="showTagsDropdown" @click="showTagsDropdown = false" class="fixed inset-0 z-40"></div>
                <div v-if="showTagsDropdown" class="absolute start-0 top-full mt-2 w-64 bg-white rounded-xl shadow-lg border border-gray-100 py-2 z-50">
                  <div class="px-3 py-2 border-b border-gray-100">
                    <div class="text-xs font-semibold text-gray-700">Filter by Tags</div>
                  </div>
                  <div class="max-h-60 overflow-y-auto py-1">
                    <label v-for="tag in allLessonsTags" :key="tag" class="flex items-center gap-3 px-3 py-2 hover:bg-gray-50 cursor-pointer">
                      <input type="checkbox" :checked="isTagSelected(tag)" @change="toggleTagFilter(tag)" class="rounded border-gray-300 text-teal-600 focus:ring-teal-500" />
                      <span class="text-sm text-gray-700 flex-1">{{ tag }}</span>
                    </label>
                  </div>
                  <div class="px-3 py-2 border-t border-gray-100 flex items-center justify-between">
                    <button @click="selectedTags = []; currentPage = 1" class="text-xs text-gray-500 hover:text-gray-700">Clear</button>
                    <button @click="showTagsDropdown = false" class="text-xs font-medium text-teal-600 hover:text-teal-700">Apply</button>
                  </div>
                </div>
              </div>

              <!-- Bookmarked Toggle -->
              <button @click="showBookmarkedOnly = !showBookmarkedOnly; currentPage = 1" :class="[showBookmarkedOnly ? 'bg-teal-50 border-teal-200 text-teal-700' : 'bg-white border-gray-200 text-gray-600 hover:bg-gray-50', 'flex items-center gap-2 px-3 py-1.5 rounded-lg text-xs font-medium transition-all border']">
                <i :class="[showBookmarkedOnly ? 'fas fa-bookmark' : 'far fa-bookmark']"></i>
                <span>Bookmarked</span>
              </button>

              <!-- Spacer -->
              <div class="ms-auto"></div>

              <!-- Sort Dropdown -->
              <div class="relative flex items-center gap-1">
                <div class="relative">
                  <button @click="showSortDropdown = !showSortDropdown" class="flex items-center gap-2 px-3 py-1.5 rounded-lg text-xs font-medium bg-white border border-gray-200 text-gray-600 hover:bg-gray-50 transition-all">
                    <i class="fas fa-sort"></i>
                    <span>{{ sortOptions.find(o => o.value === lessonsSortBy)?.label || 'Sort' }}</span>
                    <i class="fas fa-chevron-down text-[10px]"></i>
                  </button>
                  <div v-if="showSortDropdown" @click="showSortDropdown = false" class="fixed inset-0 z-40"></div>
                  <div v-if="showSortDropdown" class="absolute end-0 top-full mt-2 w-48 bg-white rounded-xl shadow-lg border border-gray-100 py-2 z-50">
                    <button v-for="option in sortOptions" :key="option.value" @click="lessonsSortBy = option.value; showSortDropdown = false; currentPage = 1" :class="[lessonsSortBy === option.value ? 'bg-teal-50 text-teal-700' : 'text-gray-700 hover:bg-gray-50', 'w-full flex items-center gap-2 px-3 py-2 text-sm']">
                      <i :class="[option.icon, 'w-4 text-center text-xs']"></i>
                      <span>{{ option.label }}</span>
                      <i v-if="lessonsSortBy === option.value" class="fas fa-check ms-auto text-teal-500 text-xs"></i>
                    </button>
                  </div>
                </div>
                <button @click="lessonsSortOrder = lessonsSortOrder === 'asc' ? 'desc' : 'asc'; currentPage = 1" class="flex items-center justify-center w-8 h-8 rounded-lg text-xs bg-white border border-gray-200 text-gray-600 hover:bg-gray-50 transition-all">
                  <i :class="[lessonsSortOrder === 'asc' ? 'fas fa-sort-amount-up' : 'fas fa-sort-amount-down']"></i>
                </button>
              </div>

              <!-- View Mode Toggle -->
              <div class="flex items-center bg-gray-100 rounded-lg p-0.5">
                <button @click="viewMode = 'grid'" :class="[viewMode === 'grid' ? 'bg-teal-500 shadow-sm text-white' : 'text-gray-500 hover:text-gray-700', 'w-8 h-7 rounded-md flex items-center justify-center text-xs transition-all']">
                  <i class="fas fa-th-large"></i>
                </button>
                <button @click="viewMode = 'list'" :class="[viewMode === 'list' ? 'bg-teal-500 shadow-sm text-white' : 'text-gray-500 hover:text-gray-700', 'w-8 h-7 rounded-md flex items-center justify-center text-xs transition-all']">
                  <i class="fas fa-list"></i>
                </button>
              </div>
            </div>
          </div>

          <!-- Active Filters Bar -->
          <div v-if="activeLessonsFiltersCount > 0" class="flex items-center gap-3 mb-4 p-3 bg-white rounded-xl border border-gray-100 shadow-sm">
            <div class="flex items-center gap-2 text-xs text-gray-500 shrink-0">
              <i class="fas fa-filter text-teal-500"></i>
              <span class="font-medium">Filters</span>
              <span class="bg-teal-100 text-teal-700 text-[10px] font-bold rounded-full w-5 h-5 flex items-center justify-center">{{ activeLessonsFiltersCount }}</span>
            </div>
            <div class="flex flex-wrap gap-2 flex-1">
              <!-- Search chip -->
              <span v-if="searchQuery" class="inline-flex items-center gap-1.5 px-2.5 py-1 bg-blue-50 text-blue-700 rounded-lg text-xs font-medium">
                <i class="fas fa-search text-[10px]"></i>
                "{{ searchQuery }}"
                <button @click="searchQuery = ''; currentPage = 1" class="hover:text-blue-900"><i class="fas fa-times text-[10px]"></i></button>
              </span>
              <!-- Category chips -->
              <span v-for="cat in selectedCategories" :key="'cat-' + cat" class="inline-flex items-center gap-1.5 px-2.5 py-1 bg-teal-50 text-teal-700 rounded-lg text-xs font-medium">
                <i :class="[getCategoryInfo(cat).icon, 'text-[10px]']"></i>
                {{ getCategoryInfo(cat).label }}
                <button @click="toggleCategoryFilter(cat)" class="hover:text-teal-900"><i class="fas fa-times text-[10px]"></i></button>
              </span>
              <!-- Status chips -->
              <span v-for="status in selectedStatuses" :key="'status-' + status" class="inline-flex items-center gap-1.5 px-2.5 py-1 bg-purple-50 text-purple-700 rounded-lg text-xs font-medium">
                <i :class="[statusConfig[status]?.icon || 'fas fa-circle', 'text-[10px]']"></i>
                {{ statusConfig[status]?.label || status }}
                <button @click="toggleStatusFilter(status)" class="hover:text-purple-900"><i class="fas fa-times text-[10px]"></i></button>
              </span>
              <!-- Impact chips -->
              <span v-for="impact in selectedImpacts" :key="'impact-' + impact" class="inline-flex items-center gap-1.5 px-2.5 py-1 bg-teal-50 text-teal-700 rounded-lg text-xs font-medium">
                <i class="fas fa-bolt text-[10px]"></i>
                {{ impact }}
                <button @click="toggleImpactFilter(impact)" class="hover:text-teal-900"><i class="fas fa-times text-[10px]"></i></button>
              </span>
              <!-- Tag chips -->
              <span v-for="tag in selectedTags" :key="'tag-' + tag" class="inline-flex items-center gap-1.5 px-2.5 py-1 bg-teal-50 text-teal-700 rounded-lg text-xs font-medium">
                <i class="fas fa-tag text-[10px]"></i>
                {{ tag }}
                <button @click="toggleTagFilter(tag)" class="hover:text-teal-900"><i class="fas fa-times text-[10px]"></i></button>
              </span>
              <!-- Bookmarked chip -->
              <span v-if="showBookmarkedOnly" class="inline-flex items-center gap-1.5 px-2.5 py-1 bg-indigo-50 text-indigo-700 rounded-lg text-xs font-medium">
                <i class="fas fa-bookmark text-[10px]"></i>
                Bookmarked
                <button @click="showBookmarkedOnly = false; currentPage = 1" class="hover:text-indigo-900"><i class="fas fa-times text-[10px]"></i></button>
              </span>
            </div>
            <button @click="clearAllFilters" class="text-xs text-gray-500 hover:text-red-600 font-medium shrink-0 transition-colors">
              <i class="fas fa-times-circle mr-1"></i>Clear All
            </button>
          </div>

          <!-- Grid View -->
          <div v-if="viewMode === 'grid'" class="grid grid-cols-[repeat(auto-fill,minmax(320px,1fr))] gap-4">
            <article v-for="lesson in paginatedLessons" :key="lesson.id" @click="openDetail(lesson)" class="group bg-white rounded-xl p-4 cursor-pointer transition-all duration-300 hover:-translate-y-1 border border-gray-200 shadow-sm hover:shadow-md hover:border-teal-200">
              <!-- Header -->
              <div class="flex items-center justify-between mb-3">
                <div class="flex items-center gap-2">
                  <div class="w-8 h-8 rounded-lg bg-teal-50 flex items-center justify-center">
                    <i :class="[getCategoryInfo(lesson.category).icon, getCategoryInfo(lesson.category).color, 'text-sm']"></i>
                  </div>
                  <span :class="[statusConfig[lesson.status]?.color || 'bg-gray-100 text-gray-700', 'px-2 py-0.5 rounded-full text-[10px] font-semibold']">
                    {{ statusConfig[lesson.status]?.label || lesson.status }}
                  </span>
                  <span :class="[impactConfig[lesson.impact]?.color || 'bg-gray-100 text-gray-700', 'px-2 py-0.5 rounded-full text-[10px] font-semibold']">
                    {{ lesson.impact }}
                  </span>
                </div>
                <button @click.stop="toggleLessonBookmark(lesson)" class="w-7 h-7 rounded-lg flex items-center justify-center text-sm transition-colors" :class="lesson.isBookmarked ? 'text-teal-500 bg-teal-50' : 'text-gray-300 hover:text-teal-400 hover:bg-teal-50'">
                  <i :class="lesson.isBookmarked ? 'fas fa-bookmark' : 'far fa-bookmark'"></i>
                </button>
              </div>

              <!-- Category & Date -->
              <div class="flex items-center gap-2 mb-2 text-xs text-gray-400">
                <span>{{ getCategoryInfo(lesson.category).label }}</span>
                <span class="w-1 h-1 rounded-full bg-gray-300"></span>
                <span>{{ formatDate(lesson.createdAt) }}</span>
              </div>

              <!-- Title -->
              <h3 class="text-sm font-semibold text-gray-900 line-clamp-2 mb-1 group-hover:text-teal-600 transition-colors">{{ lesson.title }}</h3>

              <!-- Description -->
              <p class="text-xs text-gray-500 line-clamp-2 mb-3">{{ lesson.description }}</p>

              <!-- Tags -->
              <div class="flex flex-wrap gap-1.5 mb-3">
                <span v-for="tag in lesson.tags.slice(0, 3)" :key="tag" class="px-2 py-0.5 bg-gray-100 text-gray-600 rounded-md text-[10px] font-medium">{{ tag }}</span>
                <span v-if="lesson.tags.length > 3" class="px-2 py-0.5 bg-gray-100 text-gray-400 rounded-md text-[10px] font-medium">+{{ lesson.tags.length - 3 }}</span>
              </div>

              <!-- Footer: Author + Stats -->
              <div class="flex items-center justify-between pt-3 border-t border-gray-100">
                <div class="flex items-center gap-2">
                  <div class="w-6 h-6 rounded-full bg-gradient-to-br from-teal-400 to-cyan-500 flex items-center justify-center text-white text-[10px] font-bold">
                    {{ lesson.authorInitials }}
                  </div>
                  <span class="text-xs text-gray-600 font-medium">{{ lesson.authorName }}</span>
                </div>
                <div class="flex items-center gap-3 text-xs text-gray-400">
                  <span><i class="fas fa-eye mr-1"></i>{{ lesson.viewCount }}</span>
                  <span :class="lesson.isLiked ? 'text-red-500' : ''"><i :class="[lesson.isLiked ? 'fas fa-heart' : 'far fa-heart', 'mr-1']"></i>{{ lesson.likeCount }}</span>
                  <span><i class="far fa-comment mr-1"></i>{{ lesson.commentCount }}</span>
                </div>
              </div>

              <!-- Action Progress Bar -->
              <div v-if="lesson.totalActions > 0" class="mt-3 pt-3 border-t border-gray-100">
                <div class="flex items-center justify-between text-[10px] text-gray-500 mb-1">
                  <span>Actions: {{ lesson.completedActions }}/{{ lesson.totalActions }}</span>
                  <span>{{ getActionProgress(lesson) }}%</span>
                </div>
                <div class="w-full h-1.5 bg-gray-100 rounded-full overflow-hidden">
                  <div class="h-full rounded-full transition-all duration-500" :class="[getActionProgress(lesson) === 100 ? 'bg-green-500' : lesson.overdueActions > 0 ? 'bg-red-400' : 'bg-teal-500']" :style="{ width: getActionProgress(lesson) + '%' }"></div>
                </div>
              </div>
            </article>
          </div>

          <!-- List View -->
          <div v-if="viewMode === 'list'" class="space-y-3">
            <article v-for="lesson in paginatedLessons" :key="lesson.id" @click="openDetail(lesson)" class="group bg-white rounded-xl p-5 cursor-pointer transition-all duration-200 hover:shadow-md border border-gray-200 hover:border-teal-200">
              <div class="flex items-start gap-4">
                <!-- Left: Status indicator -->
                <div class="shrink-0 flex flex-col items-center gap-2">
                  <div class="w-10 h-10 rounded-xl bg-teal-50 flex items-center justify-center">
                    <i :class="[getCategoryInfo(lesson.category).icon, getCategoryInfo(lesson.category).color, 'text-lg']"></i>
                  </div>
                </div>

                <!-- Center: Content -->
                <div class="flex-1 min-w-0">
                  <!-- Badges row -->
                  <div class="flex items-center gap-2 mb-2">
                    <span :class="[statusConfig[lesson.status]?.color || 'bg-gray-100 text-gray-700', 'px-2.5 py-0.5 rounded-full text-xs font-semibold']">
                      <i :class="[statusConfig[lesson.status]?.icon, 'mr-1']"></i>
                      {{ statusConfig[lesson.status]?.label || lesson.status }}
                    </span>
                    <span :class="[impactConfig[lesson.impact]?.color || 'bg-gray-100 text-gray-700', 'px-2.5 py-0.5 rounded-full text-xs font-semibold']">
                      {{ lesson.impact }} Impact
                    </span>
                    <span v-if="lesson.isFeatured" class="px-2 py-0.5 bg-teal-100 text-teal-700 rounded-full text-xs font-semibold">
                      <i class="fas fa-star mr-1"></i>Featured
                    </span>
                  </div>

                  <!-- Title -->
                  <h3 class="text-base font-semibold text-gray-900 mb-1 group-hover:text-teal-600 transition-colors">{{ lesson.title }}</h3>

                  <!-- Description -->
                  <p class="text-sm text-gray-500 line-clamp-2 mb-3">{{ lesson.description }}</p>

                  <!-- Author & Meta -->
                  <div class="flex items-center gap-4 text-xs text-gray-400">
                    <div class="flex items-center gap-1.5">
                      <div class="w-5 h-5 rounded-full bg-gradient-to-br from-teal-400 to-cyan-500 flex items-center justify-center text-white text-[8px] font-bold">
                        {{ lesson.authorInitials }}
                      </div>
                      <span class="text-gray-600 font-medium">{{ lesson.authorName }}</span>
                    </div>
                    <span><i class="fas fa-calendar-alt mr-1"></i>{{ formatDate(lesson.createdAt) }}</span>
                    <span><i class="fas fa-layer-group mr-1"></i>{{ getCategoryInfo(lesson.category).label }}</span>
                    <span><i class="fas fa-eye mr-1"></i>{{ lesson.viewCount }}</span>
                    <span :class="lesson.isLiked ? 'text-red-500' : ''"><i :class="[lesson.isLiked ? 'fas fa-heart' : 'far fa-heart', 'mr-1']"></i>{{ lesson.likeCount }}</span>
                    <span><i class="far fa-comment mr-1"></i>{{ lesson.commentCount }}</span>
                  </div>

                  <!-- Tags -->
                  <div class="flex flex-wrap gap-1.5 mt-2">
                    <span v-for="tag in lesson.tags" :key="tag" class="px-2 py-0.5 bg-gray-100 text-gray-600 rounded-md text-[10px] font-medium">{{ tag }}</span>
                  </div>
                </div>

                <!-- Right: Actions sidebar -->
                <div class="shrink-0 flex flex-col items-end gap-3">
                  <!-- Bookmark button -->
                  <button @click.stop="toggleLessonBookmark(lesson)" class="w-8 h-8 rounded-lg flex items-center justify-center text-sm transition-colors" :class="lesson.isBookmarked ? 'text-teal-500 bg-teal-50' : 'text-gray-300 hover:text-teal-400 hover:bg-teal-50'">
                    <i :class="lesson.isBookmarked ? 'fas fa-bookmark' : 'far fa-bookmark'"></i>
                  </button>

                  <!-- Action Progress -->
                  <div v-if="lesson.totalActions > 0" class="text-center">
                    <div class="text-lg font-bold" :class="[getActionProgress(lesson) === 100 ? 'text-green-600' : lesson.overdueActions > 0 ? 'text-red-600' : 'text-teal-600']">
                      {{ getActionProgress(lesson) }}%
                    </div>
                    <div class="text-[10px] text-gray-400">{{ lesson.completedActions }}/{{ lesson.totalActions }} actions</div>
                    <div class="w-16 h-1.5 bg-gray-100 rounded-full overflow-hidden mt-1">
                      <div class="h-full rounded-full transition-all" :class="[getActionProgress(lesson) === 100 ? 'bg-green-500' : lesson.overdueActions > 0 ? 'bg-red-400' : 'bg-teal-500']" :style="{ width: getActionProgress(lesson) + '%' }"></div>
                    </div>
                  </div>
                </div>
              </div>
            </article>
          </div>

          <!-- Empty State -->
          <div v-if="paginatedLessons.length === 0" class="text-center py-16">
            <div class="w-16 h-16 rounded-2xl bg-teal-50 flex items-center justify-center mx-auto mb-4">
              <i class="fas fa-lightbulb text-2xl text-teal-400"></i>
            </div>
            <h3 class="text-lg font-semibold text-gray-900 mb-2">No lessons found</h3>
            <p class="text-sm text-gray-500 mb-4">Try adjusting your filters or search criteria</p>
            <button @click="clearAllFilters" class="px-4 py-2 bg-teal-50 text-teal-600 rounded-lg text-sm font-medium hover:bg-teal-100 transition-colors">
              <i class="fas fa-times-circle mr-2"></i>Clear All Filters
            </button>
          </div>

          <!-- Pagination -->
          <div v-if="totalPages > 1" class="flex items-center justify-between mt-6">
            <div class="text-xs text-gray-500">
              Showing {{ (currentPage - 1) * pageSize + 1 }} - {{ Math.min(currentPage * pageSize, filteredLessons.length) }} of {{ filteredLessons.length }} lessons
            </div>
            <div class="flex items-center gap-1">
              <button @click="currentPage = Math.max(1, currentPage - 1)" :disabled="currentPage === 1" class="w-8 h-8 rounded-lg flex items-center justify-center text-xs border border-gray-200 text-gray-600 hover:bg-gray-50 disabled:opacity-40 disabled:cursor-not-allowed transition-colors">
                <i class="fas fa-chevron-left"></i>
              </button>
              <template v-for="page in totalPages" :key="page">
                <button v-if="page === 1 || page === totalPages || (page >= currentPage - 1 && page <= currentPage + 1)" @click="currentPage = page" :class="[page === currentPage ? 'bg-teal-500 text-white border-teal-500' : 'border-gray-200 text-gray-600 hover:bg-gray-50', 'w-8 h-8 rounded-lg flex items-center justify-center text-xs border transition-colors']">
                  {{ page }}
                </button>
                <span v-else-if="page === currentPage - 2 || page === currentPage + 2" class="text-gray-400 text-xs px-1">...</span>
              </template>
              <button @click="currentPage = Math.min(totalPages, currentPage + 1)" :disabled="currentPage === totalPages" class="w-8 h-8 rounded-lg flex items-center justify-center text-xs border border-gray-200 text-gray-600 hover:bg-gray-50 disabled:opacity-40 disabled:cursor-not-allowed transition-colors">
                <i class="fas fa-chevron-right"></i>
              </button>
            </div>
          </div>
        </div>

      <!-- Analytics View -->
      <div v-if="currentView === 'analytics'" class="space-y-6">

        <!-- KPI Cards Row -->
        <div class="grid grid-cols-2 md:grid-cols-4 gap-4">
          <div class="bg-white rounded-2xl p-5 border border-gray-100 shadow-sm relative overflow-hidden">
            <div class="absolute top-0 right-0 w-20 h-20 bg-gradient-to-bl from-teal-50 to-transparent rounded-bl-full"></div>
            <div class="relative">
              <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-teal-500 to-teal-600 flex items-center justify-center text-white mb-3 shadow-lg shadow-teal-200">
                <i class="fas fa-lightbulb"></i>
              </div>
              <div class="text-3xl font-bold text-gray-900">{{ analytics.totalLessons }}</div>
              <div class="text-xs text-gray-500 mt-1">Total Lessons</div>
              <div class="flex items-center gap-1 mt-2 text-xs font-medium text-teal-600">
                <i class="fas fa-arrow-up text-[10px]"></i>
                <span>+{{ analytics.lessonsPublishedInPeriod }} this period</span>
              </div>
            </div>
          </div>
          <div class="bg-white rounded-2xl p-5 border border-gray-100 shadow-sm relative overflow-hidden">
            <div class="absolute top-0 right-0 w-20 h-20 bg-gradient-to-bl from-green-50 to-transparent rounded-bl-full"></div>
            <div class="relative">
              <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-green-500 to-emerald-600 flex items-center justify-center text-white mb-3 shadow-lg shadow-green-200">
                <i class="fas fa-check-double"></i>
              </div>
              <div class="text-3xl font-bold text-gray-900">{{ analytics.actionCompletionRate }}%</div>
              <div class="text-xs text-gray-500 mt-1">Action Completion</div>
              <div class="w-full bg-gray-100 rounded-full h-1.5 mt-2">
                <div class="bg-gradient-to-r from-green-400 to-emerald-500 h-1.5 rounded-full transition-all" :style="{ width: analytics.actionCompletionRate + '%' }"></div>
              </div>
            </div>
          </div>
          <div class="bg-white rounded-2xl p-5 border border-gray-100 shadow-sm relative overflow-hidden">
            <div class="absolute top-0 right-0 w-20 h-20 bg-gradient-to-bl from-red-50 to-transparent rounded-bl-full"></div>
            <div class="relative">
              <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-red-500 to-rose-600 flex items-center justify-center text-white mb-3 shadow-lg shadow-red-200">
                <i class="fas fa-exclamation-triangle"></i>
              </div>
              <div class="text-3xl font-bold text-gray-900">{{ analytics.overdueActions }}</div>
              <div class="text-xs text-gray-500 mt-1">Overdue Actions</div>
              <div class="flex items-center gap-1 mt-2 text-xs font-medium text-red-600">
                <i class="fas fa-clock text-[10px]"></i>
                <span>Requires attention</span>
              </div>
            </div>
          </div>
          <div class="bg-white rounded-2xl p-5 border border-gray-100 shadow-sm relative overflow-hidden">
            <div class="absolute top-0 right-0 w-20 h-20 bg-gradient-to-bl from-amber-50 to-transparent rounded-bl-full"></div>
            <div class="relative">
              <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-amber-500 to-orange-600 flex items-center justify-center text-white mb-3 shadow-lg shadow-amber-200">
                <i class="fas fa-user-slash"></i>
              </div>
              <div class="text-3xl font-bold text-gray-900">{{ analytics.lessonsWithoutProcessOwner }}</div>
              <div class="text-xs text-gray-500 mt-1">No Process Owner</div>
              <div class="flex items-center gap-1 mt-2 text-xs font-medium text-amber-600">
                <i class="fas fa-info-circle text-[10px]"></i>
                <span>Needs assignment</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Two Column: Status Distribution + Completion Gauge -->
        <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <!-- Lessons by Status -->
          <div class="bg-white rounded-2xl border border-gray-100 shadow-sm overflow-hidden">
            <div class="px-6 py-4 border-b border-gray-100 flex items-center justify-between">
              <h3 class="text-sm font-bold text-gray-900 flex items-center gap-2">
                <div class="w-8 h-8 rounded-lg bg-teal-50 flex items-center justify-center">
                  <i class="fas fa-chart-pie text-teal-500 text-sm"></i>
                </div>
                Lessons by Status
              </h3>
              <span class="text-xs text-gray-400">{{ analytics.totalLessons }} total</span>
            </div>
            <div class="p-6">
              <!-- Visual bar chart -->
              <div class="space-y-3">
                <div v-for="(count, status) in analytics.lessonsByStatus" :key="status" class="group">
                  <div class="flex items-center justify-between mb-1.5">
                    <div class="flex items-center gap-2.5">
                      <i :class="[statusConfig[String(status)]?.icon, 'text-xs w-4 text-center']" :style="{ color: statusConfig[String(status)]?.color?.includes('green') ? '#16a34a' : statusConfig[String(status)]?.color?.includes('yellow') ? '#ca8a04' : statusConfig[String(status)]?.color?.includes('red') ? '#dc2626' : statusConfig[String(status)]?.color?.includes('blue') ? '#2563eb' : statusConfig[String(status)]?.color?.includes('orange') ? '#ea580c' : statusConfig[String(status)]?.color?.includes('teal') ? '#0d9488' : statusConfig[String(status)]?.color?.includes('emerald') ? '#059669' : statusConfig[String(status)]?.color?.includes('slate') ? '#475569' : '#6b7280' }"></i>
                      <span class="text-sm font-medium text-gray-700">{{ statusConfig[String(status)]?.label || status }}</span>
                    </div>
                    <span class="text-sm font-bold text-gray-900">{{ count }}</span>
                  </div>
                  <div class="w-full bg-gray-100 rounded-full h-2.5 overflow-hidden">
                    <div class="h-2.5 rounded-full transition-all duration-700 group-hover:opacity-80" :class="String(status) === 'Published' ? 'bg-gradient-to-r from-green-400 to-green-500' : String(status) === 'ActionsPending' ? 'bg-gradient-to-r from-orange-400 to-orange-500' : String(status) === 'Draft' ? 'bg-gradient-to-r from-gray-300 to-gray-400' : String(status) === 'PendingReview' ? 'bg-gradient-to-r from-yellow-400 to-yellow-500' : String(status) === 'ActionsComplete' ? 'bg-gradient-to-r from-teal-400 to-teal-500' : String(status) === 'Verified' ? 'bg-gradient-to-r from-emerald-400 to-emerald-500' : String(status) === 'Archived' ? 'bg-gradient-to-r from-slate-300 to-slate-400' : 'bg-gradient-to-r from-blue-400 to-blue-500'" :style="{ width: Math.max(4, (count / analytics.totalLessons) * 100) + '%' }"></div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Action Completion Gauge + Summary -->
          <div class="bg-white rounded-2xl border border-gray-100 shadow-sm overflow-hidden">
            <div class="px-6 py-4 border-b border-gray-100 flex items-center justify-between">
              <h3 class="text-sm font-bold text-gray-900 flex items-center gap-2">
                <div class="w-8 h-8 rounded-lg bg-green-50 flex items-center justify-center">
                  <i class="fas fa-tasks text-green-500 text-sm"></i>
                </div>
                Action Tracking
              </h3>
              <span class="text-xs text-gray-400">{{ analytics.totalActions }} actions</span>
            </div>
            <div class="p-6">
              <!-- Circular Gauge -->
              <div class="flex items-center justify-center mb-6">
                <div class="relative w-44 h-44">
                  <svg class="w-44 h-44 transform -rotate-90" viewBox="0 0 120 120">
                    <circle cx="60" cy="60" r="50" fill="none" stroke="#f1f5f9" stroke-width="12" />
                    <circle cx="60" cy="60" r="50" fill="none" stroke="url(#gaugeGrad)" stroke-width="12" stroke-linecap="round" :stroke-dasharray="314" :stroke-dashoffset="314 - (314 * analytics.actionCompletionRate / 100)" class="transition-all duration-1000" />
                    <defs>
                      <linearGradient id="gaugeGrad" x1="0%" y1="0%" x2="100%" y2="0%">
                        <stop offset="0%" stop-color="#14b8a6" />
                        <stop offset="100%" stop-color="#10b981" />
                      </linearGradient>
                    </defs>
                  </svg>
                  <div class="absolute inset-0 flex flex-col items-center justify-center">
                    <span class="text-3xl font-bold text-gray-900">{{ analytics.actionCompletionRate }}%</span>
                    <span class="text-xs text-gray-500 mt-0.5">Completion</span>
                  </div>
                </div>
              </div>
              <!-- Action Stats Grid -->
              <div class="grid grid-cols-2 gap-3">
                <div class="bg-blue-50 rounded-xl p-3.5 text-center">
                  <div class="text-xl font-bold text-blue-700">{{ analytics.totalActions }}</div>
                  <div class="text-[11px] text-blue-500 font-medium mt-0.5">Total</div>
                </div>
                <div class="bg-green-50 rounded-xl p-3.5 text-center">
                  <div class="text-xl font-bold text-green-700">{{ analytics.completedActions }}</div>
                  <div class="text-[11px] text-green-500 font-medium mt-0.5">Completed</div>
                </div>
                <div class="bg-amber-50 rounded-xl p-3.5 text-center">
                  <div class="text-xl font-bold text-amber-700">{{ analytics.openActions }}</div>
                  <div class="text-[11px] text-amber-500 font-medium mt-0.5">Open</div>
                </div>
                <div class="bg-red-50 rounded-xl p-3.5 text-center">
                  <div class="text-xl font-bold text-red-700">{{ analytics.overdueActions }}</div>
                  <div class="text-[11px] text-red-500 font-medium mt-0.5">Overdue</div>
                </div>
              </div>
              <!-- Avg Time -->
              <div class="mt-4 flex items-center justify-center gap-2 text-sm text-gray-500 bg-gray-50 rounded-xl py-2.5">
                <i class="fas fa-clock text-gray-400"></i>
                Avg. completion time: <span class="font-bold text-gray-900">{{ analytics.averageTimeToCompleteActionDays }} days</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Category Distribution (full width) -->
        <div class="bg-white rounded-2xl border border-gray-100 shadow-sm overflow-hidden">
          <div class="px-6 py-4 border-b border-gray-100 flex items-center justify-between">
            <h3 class="text-sm font-bold text-gray-900 flex items-center gap-2">
              <div class="w-8 h-8 rounded-lg bg-indigo-50 flex items-center justify-center">
                <i class="fas fa-layer-group text-indigo-500 text-sm"></i>
              </div>
              Lessons by Category
            </h3>
            <span class="text-xs text-gray-400">{{ Object.keys(analytics.lessonsByCategory).length }} categories</span>
          </div>
          <div class="p-6">
            <div class="grid grid-cols-2 md:grid-cols-4 gap-4">
              <div v-for="(count, category) in analytics.lessonsByCategory" :key="category" class="group bg-gray-50 hover:bg-white rounded-xl p-4 border border-transparent hover:border-gray-200 hover:shadow-md transition-all cursor-default">
                <div class="flex items-center gap-3 mb-3">
                  <div class="w-9 h-9 rounded-lg bg-white group-hover:bg-gradient-to-br group-hover:from-teal-500 group-hover:to-teal-600 flex items-center justify-center shadow-sm transition-all">
                    <i :class="[getCategoryInfo(String(category)).icon, 'text-sm text-teal-500 group-hover:text-white transition-colors']"></i>
                  </div>
                  <div class="text-2xl font-bold text-gray-900">{{ count }}</div>
                </div>
                <div class="text-sm font-medium text-gray-700 mb-2">{{ getCategoryInfo(String(category)).label }}</div>
                <div class="w-full bg-gray-200 rounded-full h-1.5">
                  <div class="bg-gradient-to-r from-teal-400 to-teal-500 h-1.5 rounded-full" :style="{ width: Math.max(8, (count / Math.max(...Object.values(analytics.lessonsByCategory))) * 100) + '%' }"></div>
                </div>
                <div class="text-[11px] text-gray-400 mt-1.5">{{ Math.round((count / analytics.totalLessons) * 100) }}% of total</div>
              </div>
            </div>
          </div>
        </div>
      </div>
      </div>

    <!-- Detail Modal -->
    <Teleport to="body">
      <div v-if="showDetailModal && selectedLesson" class="fixed inset-0 z-[100] flex items-center justify-center p-4">
        <div class="absolute inset-0 bg-black/50 backdrop-blur-sm" @click="showDetailModal = false"></div>
        <div class="relative w-full max-w-3xl max-h-[90vh] bg-white rounded-2xl shadow-2xl overflow-hidden flex flex-col">
          <!-- Modal Header -->
          <div class="px-6 py-4 border-b border-gray-100 flex items-center justify-between shrink-0">
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 rounded-xl bg-teal-50 flex items-center justify-center">
                <i :class="[getCategoryInfo(selectedLesson.category).icon, getCategoryInfo(selectedLesson.category).color, 'text-lg']"></i>
              </div>
              <div>
                <h2 class="text-lg font-bold text-gray-900 line-clamp-1">{{ selectedLesson.title }}</h2>
                <div class="flex items-center gap-2 mt-0.5">
                  <span :class="[statusConfig[selectedLesson.status]?.color || 'bg-gray-100 text-gray-700', 'px-2 py-0.5 rounded-full text-[10px] font-semibold']">
                    {{ statusConfig[selectedLesson.status]?.label || selectedLesson.status }}
                  </span>
                  <span :class="[impactConfig[selectedLesson.impact]?.color || 'bg-gray-100 text-gray-700', 'px-2 py-0.5 rounded-full text-[10px] font-semibold']">
                    {{ selectedLesson.impact }} Impact
                  </span>
                </div>
              </div>
            </div>
            <button @click="showDetailModal = false" class="w-8 h-8 rounded-lg flex items-center justify-center text-gray-400 hover:text-gray-600 hover:bg-gray-100 transition-colors">
              <i class="fas fa-times"></i>
            </button>
          </div>

          <!-- Workflow Progress -->
          <div class="px-6 py-3 bg-gray-50 border-b border-gray-100 shrink-0">
            <div class="flex items-center gap-1">
              <template v-for="(step, idx) in workflowSteps" :key="step.status">
                <div class="flex items-center gap-1.5" :class="idx <= getWorkflowStepIndex(selectedLesson.status) ? 'opacity-100' : 'opacity-40'">
                  <div class="w-7 h-7 rounded-full flex items-center justify-center text-[10px] transition-all" :class="step.status === selectedLesson.status ? 'bg-gradient-to-br from-teal-500 to-teal-600 text-white shadow-md shadow-teal-200 ring-2 ring-teal-200' : idx < getWorkflowStepIndex(selectedLesson.status) ? 'bg-teal-100 text-teal-600' : 'bg-gray-200 text-gray-400'">
                    <i :class="step.icon"></i>
                  </div>
                  <span class="text-[10px] font-medium hidden sm:inline" :class="step.status === selectedLesson.status ? 'text-teal-700 font-bold' : idx < getWorkflowStepIndex(selectedLesson.status) ? 'text-teal-600' : 'text-gray-400'">{{ step.label }}</span>
                </div>
                <div v-if="idx < workflowSteps.length - 1" class="flex-1 h-0.5 min-w-[12px] rounded-full transition-all" :class="idx < getWorkflowStepIndex(selectedLesson.status) ? 'bg-teal-300' : 'bg-gray-200'"></div>
              </template>
            </div>
            <div v-if="selectedLesson.status === 'Rejected'" class="mt-2 flex items-center gap-2 text-xs text-red-600 bg-red-50 px-3 py-1.5 rounded-lg">
              <i class="fas fa-times-circle"></i>
              <span>This lesson was rejected and needs revision before resubmitting.</span>
            </div>
          </div>

          <!-- Tabs -->
          <div class="px-6 border-b border-gray-100 flex items-center gap-1 shrink-0">
            <button @click="activeTab = 'details'" :class="[activeTab === 'details' ? 'border-teal-500 text-teal-600' : 'border-transparent text-gray-500 hover:text-gray-700', 'px-4 py-2.5 text-sm font-medium border-b-2 transition-colors']">
              <i class="fas fa-info-circle mr-1.5"></i>Details
            </button>
            <button @click="activeTab = 'actions'" :class="[activeTab === 'actions' ? 'border-teal-500 text-teal-600' : 'border-transparent text-gray-500 hover:text-gray-700', 'px-4 py-2.5 text-sm font-medium border-b-2 transition-colors']">
              <i class="fas fa-tasks mr-1.5"></i>Actions
              <span v-if="selectedLesson.totalActions > 0" class="ml-1 bg-gray-100 text-gray-600 text-[10px] rounded-full px-1.5 py-0.5">{{ selectedLesson.totalActions }}</span>
            </button>
            <button @click="activeTab = 'rootcause'" :class="[activeTab === 'rootcause' ? 'border-teal-500 text-teal-600' : 'border-transparent text-gray-500 hover:text-gray-700', 'px-4 py-2.5 text-sm font-medium border-b-2 transition-colors']">
              <i class="fas fa-search mr-1.5"></i>Root Cause
            </button>
          </div>

          <!-- Tab Content -->
          <div class="flex-1 overflow-y-auto p-6">
            <!-- Details Tab -->
            <div v-if="activeTab === 'details'" class="space-y-6">
              <!-- Author & Meta -->
              <div class="flex items-center gap-4 p-4 bg-gray-50 rounded-xl">
                <div class="w-12 h-12 rounded-full bg-gradient-to-br from-teal-400 to-cyan-500 flex items-center justify-center text-white font-bold">
                  {{ selectedLesson.authorInitials }}
                </div>
                <div class="flex-1">
                  <div class="font-semibold text-gray-900">{{ selectedLesson.isAnonymous ? 'Anonymous' : selectedLesson.authorName }}</div>
                  <div class="text-xs text-gray-500">{{ selectedLesson.authorDepartment }} &middot; {{ formatDate(selectedLesson.createdAt) }}</div>
                </div>
                <div class="flex items-center gap-3 text-xs text-gray-400">
                  <span><i class="fas fa-eye mr-1"></i>{{ selectedLesson.viewCount }}</span>
                  <span><i class="fas fa-heart mr-1"></i>{{ selectedLesson.likeCount }}</span>
                  <span><i class="fas fa-comment mr-1"></i>{{ selectedLesson.commentCount }}</span>
                </div>
              </div>

              <!-- Project Info -->
              <div v-if="selectedLesson.projectName || selectedLesson.projectPhase" class="grid grid-cols-2 gap-4">
                <div v-if="selectedLesson.projectName" class="p-3 bg-teal-50 rounded-xl">
                  <div class="text-[10px] font-semibold text-teal-600 uppercase tracking-wider mb-1">Project</div>
                  <div class="text-sm font-medium text-gray-900">{{ selectedLesson.projectName }}</div>
                </div>
                <div v-if="selectedLesson.projectPhase" class="p-3 bg-blue-50 rounded-xl">
                  <div class="text-[10px] font-semibold text-blue-600 uppercase tracking-wider mb-1">Phase</div>
                  <div class="text-sm font-medium text-gray-900">{{ selectedLesson.projectPhase }}</div>
                </div>
              </div>

              <!-- Process Owner -->
              <div v-if="selectedLesson.processOwnerName" class="p-3 bg-green-50 rounded-xl">
                <div class="text-[10px] font-semibold text-green-600 uppercase tracking-wider mb-1">Process Owner</div>
                <div class="text-sm font-medium text-gray-900">{{ selectedLesson.processOwnerName }}</div>
              </div>

              <!-- Context -->
              <div v-if="selectedLesson.context">
                <h4 class="text-sm font-semibold text-gray-700 mb-2"><i class="fas fa-info-circle mr-2 text-blue-500"></i>Context</h4>
                <p class="text-sm text-gray-600 leading-relaxed">{{ selectedLesson.context }}</p>
              </div>

              <!-- Challenge -->
              <div v-if="selectedLesson.challenge">
                <h4 class="text-sm font-semibold text-gray-700 mb-2"><i class="fas fa-exclamation-triangle mr-2 text-red-500"></i>Challenge</h4>
                <p class="text-sm text-gray-600 leading-relaxed">{{ selectedLesson.challenge }}</p>
              </div>

              <!-- Solution -->
              <div v-if="selectedLesson.solution">
                <h4 class="text-sm font-semibold text-gray-700 mb-2"><i class="fas fa-check-circle mr-2 text-green-500"></i>Solution</h4>
                <p class="text-sm text-gray-600 leading-relaxed">{{ selectedLesson.solution }}</p>
              </div>

              <!-- Outcome -->
              <div v-if="selectedLesson.outcome">
                <h4 class="text-sm font-semibold text-gray-700 mb-2"><i class="fas fa-bullseye mr-2 text-teal-500"></i>Outcome</h4>
                <p class="text-sm text-gray-600 leading-relaxed">{{ selectedLesson.outcome }}</p>
              </div>

              <!-- What Went Well -->
              <div v-if="selectedLesson.whatWentWell">
                <h4 class="text-sm font-semibold text-gray-700 mb-2"><i class="fas fa-thumbs-up mr-2 text-emerald-500"></i>What Went Well</h4>
                <p class="text-sm text-gray-600 leading-relaxed">{{ selectedLesson.whatWentWell }}</p>
              </div>

              <!-- Recommendations -->
              <div v-if="selectedLesson.recommendations && selectedLesson.recommendations.length > 0">
                <h4 class="text-sm font-semibold text-gray-700 mb-2"><i class="fas fa-lightbulb mr-2 text-teal-500"></i>Recommendations</h4>
                <ul class="space-y-2">
                  <li v-for="(rec, idx) in selectedLesson.recommendations" :key="idx" class="flex items-start gap-2 text-sm text-gray-600">
                    <i class="fas fa-check text-green-500 mt-0.5 text-xs"></i>
                    <span>{{ rec }}</span>
                  </li>
                </ul>
              </div>

              <!-- Applied in Projects (LL-13) -->
              <div v-if="selectedLesson.appliedInProjects?.length" class="bg-indigo-50 rounded-xl p-4 border border-indigo-100">
                <h4 class="text-sm font-semibold text-indigo-800 mb-2"><i class="fas fa-project-diagram mr-2"></i>Applied in Projects</h4>
                <div class="flex flex-wrap gap-2">
                  <span v-for="proj in selectedLesson.appliedInProjects" :key="proj" class="px-3 py-1 bg-white text-indigo-700 rounded-lg text-xs font-medium border border-indigo-200">
                    <i class="fas fa-folder mr-1"></i>{{ proj }}
                  </span>
                </div>
              </div>

              <!-- Tags -->
              <div>
                <h4 class="text-sm font-semibold text-gray-700 mb-2"><i class="fas fa-tags mr-2 text-gray-400"></i>Tags</h4>
                <div class="flex flex-wrap gap-2">
                  <span v-for="tag in selectedLesson.tags" :key="tag" class="px-2.5 py-1 bg-gray-100 text-gray-600 rounded-lg text-xs font-medium">{{ tag }}</span>
                </div>
              </div>
            </div>

            <!-- Actions Tab -->
            <div v-if="activeTab === 'actions'" class="space-y-4">
              <!-- Actions Progress Summary -->
              <div v-if="selectedLesson.totalActions > 0" class="p-4 bg-gray-50 rounded-xl mb-4">
                <div class="flex items-center justify-between mb-2">
                  <span class="text-sm font-semibold text-gray-700">Action Progress</span>
                  <span class="text-sm font-bold" :class="[getActionProgress(selectedLesson) === 100 ? 'text-green-600' : 'text-teal-600']">{{ getActionProgress(selectedLesson) }}%</span>
                </div>
                <div class="w-full h-2 bg-gray-200 rounded-full overflow-hidden">
                  <div class="h-full rounded-full transition-all" :class="[getActionProgress(selectedLesson) === 100 ? 'bg-green-500' : 'bg-teal-500']" :style="{ width: getActionProgress(selectedLesson) + '%' }"></div>
                </div>
                <div class="flex items-center gap-4 mt-2 text-xs text-gray-500">
                  <span>{{ selectedLesson.completedActions }} completed</span>
                  <span>{{ selectedLesson.totalActions - selectedLesson.completedActions }} remaining</span>
                  <span v-if="selectedLesson.overdueActions > 0" class="text-red-600 font-medium"><i class="fas fa-exclamation-triangle mr-0.5"></i>{{ selectedLesson.overdueActions }} overdue</span>
                  <span v-if="selectedLesson.actions?.some(a => a.escalatedAt)" class="text-orange-600 font-medium"><i class="fas fa-arrow-up mr-0.5"></i>{{ selectedLesson.actions?.filter(a => a.escalatedAt).length }} escalated</span>
                </div>
              </div>
              <!-- Escalation Alert Banner -->
              <div v-if="selectedLesson.actions?.some(a => a.escalatedAt)" class="flex items-center gap-3 p-3 bg-red-50 border border-red-200 rounded-xl text-sm">
                <div class="w-8 h-8 rounded-lg bg-red-100 flex items-center justify-center shrink-0">
                  <i class="fas fa-bell text-red-500"></i>
                </div>
                <div>
                  <div class="font-semibold text-red-800">Escalation Active</div>
                  <div class="text-xs text-red-600">{{ selectedLesson.actions?.filter(a => a.escalatedAt).length }} action(s) have been escalated due to overdue deadlines. Total {{ selectedLesson.actions?.filter(a => a.isOverdue).length }} overdue action(s) with {{ selectedLesson.actions?.reduce((sum, a) => sum + a.reminderCount, 0) }} reminders sent.</div>
                </div>
              </div>

              <!-- Actions List -->
              <div v-if="selectedLesson.actions && selectedLesson.actions.length > 0" class="space-y-3">
                <div v-for="action in selectedLesson.actions" :key="action.id" class="p-4 bg-white border border-gray-200 rounded-xl">
                  <div class="flex items-start justify-between mb-2">
                    <div class="flex items-center gap-2">
                      <i :class="[actionStatusConfig[action.status]?.icon, actionStatusConfig[action.status]?.color, 'text-sm']"></i>
                      <span class="text-xs font-semibold" :class="actionStatusConfig[action.status]?.color">
                        {{ actionStatusConfig[action.status]?.label || action.status }}
                      </span>
                      <span v-if="action.isOverdue" class="px-2 py-0.5 bg-red-100 text-red-700 rounded-full text-[10px] font-semibold">
                        <i class="fas fa-exclamation-triangle mr-1"></i>Overdue
                      </span>
                    </div>
                    <span class="text-xs text-gray-400">Due: {{ formatDate(action.dueDate) }}</span>
                  </div>
                  <p class="text-sm text-gray-700 mb-2">{{ action.description }}</p>
                  <div class="flex items-center gap-4 text-xs text-gray-400">
                    <span><i class="fas fa-user mr-1"></i>{{ action.assigneeName }}</span>
                    <span v-if="action.priority" class="font-medium">Priority: {{ action.priority }}</span>
                    <span v-if="action.affectedDocumentTitle" class="flex items-center gap-1 px-2 py-0.5 bg-blue-50 text-blue-600 rounded-md">
                      <i class="fas fa-file-alt"></i>{{ action.affectedDocumentTitle }}
                      <span v-if="action.affectedDocumentType" class="text-blue-400">({{ action.affectedDocumentType }})</span>
                    </span>
                  </div>
                  <div v-if="action.completionNotes" class="mt-2 p-2 bg-green-50 rounded-lg text-xs text-green-700">
                    <i class="fas fa-check-circle mr-1"></i>{{ action.completionNotes }}
                  </div>
                  <div v-if="action.escalatedAt" class="mt-2 p-2 bg-red-50 rounded-lg text-xs text-red-700">
                    <i class="fas fa-arrow-up mr-1"></i>Escalated on {{ formatDate(action.escalatedAt) }} &middot; {{ action.reminderCount }} reminders sent
                  </div>
                </div>
              </div>

              <!-- No Actions -->
              <div v-else class="text-center py-8">
                <div class="w-12 h-12 rounded-xl bg-gray-100 flex items-center justify-center mx-auto mb-3">
                  <i class="fas fa-tasks text-xl text-gray-400"></i>
                </div>
                <p class="text-sm text-gray-500">No actions have been defined for this lesson.</p>
              </div>
            </div>

            <!-- Root Cause Tab -->
            <div v-if="activeTab === 'rootcause'" class="space-y-6">
              <div v-if="selectedLesson.rootCause">
                <div v-if="selectedLesson.rootCauseMethod" class="mb-4 p-3 bg-blue-50 rounded-xl">
                  <div class="text-[10px] font-semibold text-blue-600 uppercase tracking-wider mb-1">Analysis Method</div>
                  <div class="text-sm font-medium text-gray-900">{{ rootCauseMethodLabels[selectedLesson.rootCauseMethod] || selectedLesson.rootCauseMethod }}</div>
                </div>
                <h4 class="text-sm font-semibold text-gray-700 mb-2"><i class="fas fa-search mr-2 text-red-500"></i>Root Cause</h4>
                <p class="text-sm text-gray-600 leading-relaxed p-4 bg-red-50 rounded-xl border border-red-100">{{ selectedLesson.rootCause }}</p>
              </div>
              <div v-else class="text-center py-8">
                <div class="w-12 h-12 rounded-xl bg-gray-100 flex items-center justify-center mx-auto mb-3">
                  <i class="fas fa-search text-xl text-gray-400"></i>
                </div>
                <p class="text-sm text-gray-500">No root cause analysis has been recorded for this lesson.</p>
              </div>
            </div>
          </div>

          <!-- Workflow Actions + Social Footer -->
          <div class="px-6 py-4 border-t border-gray-100 shrink-0 space-y-3">
            <!-- Workflow Actions -->
            <div v-if="getAvailableActions(selectedLesson.status).length > 0" class="flex items-center gap-2 flex-wrap">
              <span class="text-xs text-gray-400 font-medium mr-1">Actions:</span>
              <button
                v-for="action in getAvailableActions(selectedLesson.status)"
                :key="action.targetStatus"
                @click="changeStatus(selectedLesson, action.targetStatus, action.confirm)"
                :class="[action.color, 'px-4 py-2 rounded-lg text-sm font-semibold transition-all shadow-sm hover:shadow-md flex items-center gap-2']"
              >
                <i :class="action.icon"></i>
                {{ action.label }}
              </button>
            </div>
            <!-- Social Actions -->
            <div class="flex items-center gap-3">
              <button @click.stop="toggleLessonLike(selectedLesson)" class="flex items-center gap-2 px-3 py-1.5 rounded-lg text-sm transition-colors" :class="selectedLesson.isLiked ? 'bg-red-50 text-red-600' : 'bg-gray-100 text-gray-600 hover:bg-gray-200'">
                <i :class="selectedLesson.isLiked ? 'fas fa-heart' : 'far fa-heart'"></i>
                {{ selectedLesson.likeCount }}
              </button>
              <button @click.stop="toggleLessonBookmark(selectedLesson)" class="flex items-center gap-2 px-3 py-1.5 rounded-lg text-sm transition-colors" :class="selectedLesson.isBookmarked ? 'bg-teal-50 text-teal-600' : 'bg-gray-100 text-gray-600 hover:bg-gray-200'">
                <i :class="selectedLesson.isBookmarked ? 'fas fa-bookmark' : 'far fa-bookmark'"></i>
                {{ selectedLesson.isBookmarked ? 'Bookmarked' : 'Bookmark' }}
              </button>
              <button class="flex items-center gap-2 px-3 py-1.5 rounded-lg text-sm bg-gray-100 text-gray-600 hover:bg-gray-200 transition-colors">
                <i class="fas fa-share-alt"></i> Share
              </button>
              <button @click="openDisseminate(selectedLesson)" class="flex items-center gap-2 px-3 py-1.5 rounded-lg text-sm bg-blue-50 text-blue-600 hover:bg-blue-100 transition-colors">
                <i class="fas fa-bullhorn"></i> Notify Teams
              </button>
              <button @click="toggleFollow(selectedLesson.id)" class="flex items-center gap-2 px-3 py-1.5 rounded-lg text-sm transition-colors" :class="isFollowing(selectedLesson.id) ? 'bg-purple-50 text-purple-600' : 'bg-gray-100 text-gray-600 hover:bg-gray-200'">
                <i :class="isFollowing(selectedLesson.id) ? 'fas fa-bell' : 'far fa-bell'"></i>
                {{ isFollowing(selectedLesson.id) ? 'Following' : 'Follow' }}
              </button>
              <button @click="exportLessonPDF(selectedLesson)" class="flex items-center gap-2 px-3 py-1.5 rounded-lg text-sm bg-red-50 text-red-600 hover:bg-red-100 transition-colors">
                <i class="fas fa-file-pdf"></i> Export PDF
              </button>
            </div>
          </div>
        </div>
      </div>
    </Teleport>

    <!-- Featured Detail Modal -->
    <Teleport to="body">
      <div v-if="showLessonDetailModal && selectedLessonForModal" class="fixed inset-0 z-[100] flex items-center justify-center p-4">
        <div class="absolute inset-0 bg-black/50 backdrop-blur-sm" @click="closeLessonDetail"></div>
        <div class="relative w-full max-w-2xl max-h-[85vh] bg-white rounded-2xl shadow-2xl overflow-hidden flex flex-col">
          <!-- Modal Header -->
          <div class="px-6 py-4 border-b border-gray-100 flex items-center justify-between shrink-0 bg-gradient-to-r from-teal-50 to-cyan-50">
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-teal-400 to-cyan-500 flex items-center justify-center text-white">
                <i :class="[getCategoryInfo(selectedLessonForModal.category).icon]"></i>
              </div>
              <div>
                <div class="flex items-center gap-2 mb-0.5">
                  <span class="px-2 py-0.5 bg-teal-100 text-teal-700 rounded-full text-[10px] font-semibold"><i class="fas fa-star mr-1"></i>Featured</span>
                  <span :class="[impactConfig[selectedLessonForModal.impact]?.color || 'bg-gray-100 text-gray-700', 'px-2 py-0.5 rounded-full text-[10px] font-semibold']">{{ selectedLessonForModal.impact }}</span>
                </div>
                <h2 class="text-base font-bold text-gray-900 line-clamp-1">{{ selectedLessonForModal.title }}</h2>
              </div>
            </div>
            <button @click="closeLessonDetail" class="w-8 h-8 rounded-lg flex items-center justify-center text-gray-400 hover:text-gray-600 hover:bg-gray-100 transition-colors">
              <i class="fas fa-times"></i>
            </button>
          </div>

          <!-- Content -->
          <div class="flex-1 overflow-y-auto p-6 space-y-5">
            <!-- Summary -->
            <div>
              <h4 class="text-xs font-semibold text-gray-500 uppercase tracking-wider mb-2">Summary</h4>
              <p class="text-sm text-gray-700 leading-relaxed">{{ selectedLessonForModal.summary }}</p>
            </div>

            <!-- Content -->
            <div v-if="selectedLessonForModal.content">
              <h4 class="text-xs font-semibold text-gray-500 uppercase tracking-wider mb-2">Full Content</h4>
              <p class="text-sm text-gray-600 leading-relaxed">{{ selectedLessonForModal.content }}</p>
            </div>

            <!-- Impact / Outcome -->
            <div v-if="selectedLessonForModal.outcome" class="p-4 bg-teal-50 rounded-xl border border-teal-100">
              <h4 class="text-xs font-semibold text-teal-700 uppercase tracking-wider mb-2"><i class="fas fa-bullseye mr-1"></i>Impact & Outcome</h4>
              <p class="text-sm text-gray-700 leading-relaxed">{{ selectedLessonForModal.outcome }}</p>
            </div>

            <!-- Recommendations -->
            <div v-if="selectedLessonForModal.recommendations && selectedLessonForModal.recommendations.length > 0">
              <h4 class="text-xs font-semibold text-gray-500 uppercase tracking-wider mb-2"><i class="fas fa-lightbulb mr-1 text-teal-500"></i>Recommendations</h4>
              <ul class="space-y-2">
                <li v-for="(rec, idx) in selectedLessonForModal.recommendations" :key="idx" class="flex items-start gap-2 text-sm text-gray-600">
                  <i class="fas fa-check-circle text-green-500 mt-0.5 text-xs"></i>
                  <span>{{ rec }}</span>
                </li>
              </ul>
            </div>

            <!-- Author & Stats -->
            <div class="flex items-center gap-4 p-4 bg-gray-50 rounded-xl">
              <div class="w-10 h-10 rounded-full bg-gradient-to-br from-teal-400 to-cyan-500 flex items-center justify-center text-white font-bold text-sm">
                {{ selectedLessonForModal.authorInitials }}
              </div>
              <div class="flex-1">
                <div class="font-medium text-gray-900 text-sm">{{ selectedLessonForModal.authorName }}</div>
                <div class="text-xs text-gray-500">{{ selectedLessonForModal.authorDepartment }} &middot; {{ formatDate(selectedLessonForModal.createdAt) }}</div>
              </div>
              <div class="flex items-center gap-3 text-xs text-gray-400">
                <span><i class="fas fa-eye mr-1"></i>{{ selectedLessonForModal.viewCount }}</span>
                <span><i class="fas fa-heart mr-1"></i>{{ selectedLessonForModal.likeCount }}</span>
                <span><i class="fas fa-comment mr-1"></i>{{ selectedLessonForModal.commentCount }}</span>
              </div>
            </div>
          </div>

          <!-- Social Actions Footer -->
          <div class="flex items-center justify-between px-6 py-4 border-t border-gray-100 shrink-0">
            <div class="flex items-center gap-3">
              <button @click.stop="toggleLessonLike(selectedLessonForModal)" class="flex items-center gap-2 px-3 py-1.5 rounded-lg text-sm transition-colors" :class="selectedLessonForModal.isLiked ? 'bg-red-50 text-red-600' : 'bg-gray-100 text-gray-600 hover:bg-gray-200'">
                <i :class="selectedLessonForModal.isLiked ? 'fas fa-heart' : 'far fa-heart'"></i>
                {{ selectedLessonForModal.likeCount }}
              </button>
              <button @click.stop="toggleLessonBookmark(selectedLessonForModal)" class="flex items-center gap-2 px-3 py-1.5 rounded-lg text-sm transition-colors" :class="selectedLessonForModal.isBookmarked ? 'bg-teal-50 text-teal-600' : 'bg-gray-100 text-gray-600 hover:bg-gray-200'">
                <i :class="selectedLessonForModal.isBookmarked ? 'fas fa-bookmark' : 'far fa-bookmark'"></i>
                {{ selectedLessonForModal.isBookmarked ? 'Bookmarked' : 'Bookmark' }}
              </button>
              <button class="flex items-center gap-2 px-3 py-1.5 rounded-lg text-sm bg-gray-100 text-gray-600 hover:bg-gray-200 transition-colors">
                <i class="fas fa-share-alt"></i> Share
              </button>
            </div>
            <button @click="openDetail(selectedLessonForModal); closeLessonDetail()" class="px-4 py-1.5 bg-teal-500 text-white rounded-lg text-sm font-medium hover:bg-teal-600 transition-colors">
              View Full Details <i class="fas fa-arrow-right ml-1"></i>
            </button>
          </div>
        </div>
      </div>
    </Teleport>

    <!-- Create Lesson Modal -->
    <Teleport to="body">
      <div v-if="showCreateModal" class="fixed inset-0 z-[100] flex items-center justify-center p-4">
        <div class="absolute inset-0 bg-black/50 backdrop-blur-sm" @click="showCreateModal = false"></div>
        <div class="relative w-full max-w-3xl max-h-[90vh] bg-white rounded-2xl shadow-2xl overflow-hidden flex flex-col">
          <!-- Header -->
          <div class="px-6 py-4 border-b border-gray-100 flex items-center justify-between shrink-0">
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-teal-500 to-teal-600 flex items-center justify-center text-white shadow-lg shadow-teal-200">
                <i class="fas fa-plus"></i>
              </div>
              <div>
                <h2 class="text-lg font-bold text-gray-900">New Lesson Learned</h2>
                <p class="text-xs text-gray-500">Capture knowledge from project experiences</p>
              </div>
            </div>
            <button @click="showCreateModal = false" class="w-8 h-8 rounded-lg flex items-center justify-center text-gray-400 hover:text-gray-600 hover:bg-gray-100 transition-colors">
              <i class="fas fa-times"></i>
            </button>
          </div>

          <!-- Form Body -->
          <div class="flex-1 overflow-y-auto p-6 space-y-5">
            <!-- Title -->
            <div>
              <label class="block text-sm font-semibold text-gray-700 mb-1.5">Title <span class="text-red-500">*</span></label>
              <input v-model="newLesson.title" type="text" placeholder="e.g., Effective Crowd Management at Large Events" class="w-full px-4 py-2.5 border border-gray-200 rounded-xl text-sm focus:ring-2 focus:ring-teal-200 focus:border-teal-400 transition-all" />
            </div>

            <!-- Two columns: Category + Impact -->
            <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-semibold text-gray-700 mb-1.5">Category <span class="text-red-500">*</span></label>
                <select v-model="newLesson.category" class="w-full px-4 py-2.5 border border-gray-200 rounded-xl text-sm focus:ring-2 focus:ring-teal-200 focus:border-teal-400 transition-all bg-white">
                  <option value="">Select category...</option>
                  <option v-for="cat in categoryOptions" :key="cat.id" :value="cat.id">{{ cat.label }}</option>
                </select>
              </div>
              <div>
                <label class="block text-sm font-semibold text-gray-700 mb-1.5">Impact Level <span class="text-red-500">*</span></label>
                <select v-model="newLesson.impact" class="w-full px-4 py-2.5 border border-gray-200 rounded-xl text-sm focus:ring-2 focus:ring-teal-200 focus:border-teal-400 transition-all bg-white">
                  <option value="">Select impact...</option>
                  <option value="Low">Low</option>
                  <option value="Medium">Medium</option>
                  <option value="High">High</option>
                  <option value="Critical">Critical</option>
                </select>
              </div>
            </div>

            <!-- Two columns: Project + Phase -->
            <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-semibold text-gray-700 mb-1.5">Project Name</label>
                <input v-model="newLesson.projectName" type="text" placeholder="e.g., Opening Ceremony" class="w-full px-4 py-2.5 border border-gray-200 rounded-xl text-sm focus:ring-2 focus:ring-teal-200 focus:border-teal-400 transition-all" />
              </div>
              <div>
                <label class="block text-sm font-semibold text-gray-700 mb-1.5">Project Phase</label>
                <select v-model="newLesson.projectPhase" class="w-full px-4 py-2.5 border border-gray-200 rounded-xl text-sm focus:ring-2 focus:ring-teal-200 focus:border-teal-400 transition-all bg-white">
                  <option value="">Select phase...</option>
                  <option value="Planning">Planning</option>
                  <option value="Execution">Execution</option>
                  <option value="Operations">Operations</option>
                  <option value="Closure">Closure</option>
                </select>
              </div>
            </div>

            <!-- Summary -->
            <div>
              <label class="block text-sm font-semibold text-gray-700 mb-1.5">Summary <span class="text-red-500">*</span></label>
              <textarea v-model="newLesson.summary" rows="2" placeholder="Brief summary of the lesson learned..." class="w-full px-4 py-2.5 border border-gray-200 rounded-xl text-sm focus:ring-2 focus:ring-teal-200 focus:border-teal-400 transition-all resize-none"></textarea>
            </div>

            <!-- Context -->
            <div>
              <label class="block text-sm font-semibold text-gray-700 mb-1.5">Context</label>
              <textarea v-model="newLesson.context" rows="2" placeholder="Describe the situation or background..." class="w-full px-4 py-2.5 border border-gray-200 rounded-xl text-sm focus:ring-2 focus:ring-teal-200 focus:border-teal-400 transition-all resize-none"></textarea>
            </div>

            <!-- Challenge -->
            <div>
              <label class="block text-sm font-semibold text-gray-700 mb-1.5">Challenge <span class="text-red-500">*</span></label>
              <textarea v-model="newLesson.challenge" rows="2" placeholder="What was the challenge or problem faced?" class="w-full px-4 py-2.5 border border-gray-200 rounded-xl text-sm focus:ring-2 focus:ring-teal-200 focus:border-teal-400 transition-all resize-none"></textarea>
            </div>

            <!-- AI Suggestions (LL-16) -->
            <div v-if="newLesson.challenge.length > 20" class="bg-gradient-to-r from-teal-50 to-cyan-50 rounded-xl p-4 border border-teal-100">
              <div class="flex items-center justify-between mb-2">
                <div class="flex items-center gap-2 text-sm font-semibold text-teal-700">
                  <i class="fas fa-wand-magic-sparkles"></i>AI Assistant
                </div>
                <button @click="getAISuggestions" :disabled="isAISuggesting" class="px-3 py-1.5 bg-teal-500 text-white text-xs font-semibold rounded-lg hover:bg-teal-600 disabled:opacity-50 transition-all">
                  <i :class="isAISuggesting ? 'fas fa-spinner fa-spin' : 'fas fa-magic'" class="mr-1"></i>
                  {{ isAISuggesting ? 'Analyzing...' : 'Get AI Suggestions' }}
                </button>
              </div>
              <div v-if="aiSuggestions.length > 0" class="space-y-2 mt-3">
                <div v-for="(suggestion, idx) in aiSuggestions" :key="idx" class="flex items-start gap-3 bg-white rounded-lg p-3 border border-teal-100">
                  <div class="w-6 h-6 rounded-full bg-teal-100 flex items-center justify-center shrink-0 mt-0.5">
                    <i class="fas fa-lightbulb text-teal-600 text-[10px]"></i>
                  </div>
                  <div class="flex-1 min-w-0">
                    <div class="text-[10px] font-semibold text-teal-600 uppercase tracking-wider mb-1">Suggested {{ suggestion.field }}</div>
                    <p class="text-sm text-gray-700 leading-relaxed">{{ suggestion.suggestion }}</p>
                  </div>
                  <button @click="applyAISuggestion(suggestion)" class="px-2.5 py-1 bg-teal-50 text-teal-600 text-xs font-semibold rounded-lg hover:bg-teal-100 shrink-0 transition-colors">
                    Apply
                  </button>
                </div>
              </div>
            </div>

            <!-- Solution -->
            <div>
              <label class="block text-sm font-semibold text-gray-700 mb-1.5">Solution <span class="text-red-500">*</span></label>
              <textarea v-model="newLesson.solution" rows="2" placeholder="How was the challenge addressed?" class="w-full px-4 py-2.5 border border-gray-200 rounded-xl text-sm focus:ring-2 focus:ring-teal-200 focus:border-teal-400 transition-all resize-none"></textarea>
            </div>

            <!-- Outcome -->
            <div>
              <label class="block text-sm font-semibold text-gray-700 mb-1.5">Outcome</label>
              <textarea v-model="newLesson.outcome" rows="2" placeholder="What were the results?" class="w-full px-4 py-2.5 border border-gray-200 rounded-xl text-sm focus:ring-2 focus:ring-teal-200 focus:border-teal-400 transition-all resize-none"></textarea>
            </div>

            <!-- Related Past Lessons (LL-10) -->
            <div v-if="relatedLessonsForProject.length > 0" class="bg-amber-50 rounded-xl p-4 border border-amber-100">
              <div class="flex items-center gap-2 text-sm font-semibold text-amber-800 mb-3">
                <i class="fas fa-history"></i>Related Past Lessons
                <span class="text-xs font-normal text-amber-600">({{ relatedLessonsForProject.length }} found)</span>
              </div>
              <div class="space-y-2">
                <div v-for="related in relatedLessonsForProject" :key="related.id" class="flex items-center gap-3 bg-white rounded-lg p-2.5 border border-amber-100">
                  <div class="w-7 h-7 rounded-lg bg-amber-100 flex items-center justify-center shrink-0">
                    <i class="fas fa-lightbulb text-amber-600 text-xs"></i>
                  </div>
                  <div class="flex-1 min-w-0">
                    <div class="text-sm font-medium text-gray-900 truncate">{{ related.title }}</div>
                    <div class="text-xs text-gray-500">{{ related.category }} · {{ related.impact }} · {{ related.projectName || 'N/A' }}</div>
                  </div>
                  <span :class="[statusConfig[related.status]?.color || 'bg-gray-100 text-gray-700', 'px-2 py-0.5 rounded-full text-[10px] font-semibold shrink-0']">{{ statusConfig[related.status]?.label || related.status }}</span>
                </div>
              </div>
            </div>

            <!-- Tags -->
            <div>
              <label class="block text-sm font-semibold text-gray-700 mb-1.5">Tags</label>
              <input v-model="newLesson.tags" type="text" placeholder="Comma separated, e.g., Safety, Events, Operations" class="w-full px-4 py-2.5 border border-gray-200 rounded-xl text-sm focus:ring-2 focus:ring-teal-200 focus:border-teal-400 transition-all" />
            </div>

            <!-- Anonymous -->
            <label class="flex items-center gap-3 cursor-pointer">
              <input v-model="newLesson.isAnonymous" type="checkbox" class="w-4 h-4 text-teal-500 border-gray-300 rounded focus:ring-teal-400" />
              <span class="text-sm text-gray-700">Submit anonymously</span>
            </label>
          </div>

          <!-- Footer -->
          <div class="px-6 py-4 border-t border-gray-100 flex items-center justify-between shrink-0 bg-gray-50">
            <button @click="showCreateModal = false" class="px-5 py-2.5 text-sm font-medium text-gray-600 hover:text-gray-800 hover:bg-gray-200 rounded-xl transition-colors">
              Cancel
            </button>
            <div class="flex items-center gap-3">
              <button @click="submitNewLesson('Draft')" class="px-5 py-2.5 text-sm font-semibold text-teal-600 bg-teal-50 hover:bg-teal-100 rounded-xl transition-colors">
                <i class="fas fa-save mr-1.5"></i>Save Draft
              </button>
              <button @click="submitNewLesson('PendingReview')" class="px-5 py-2.5 text-sm font-semibold text-white bg-gradient-to-r from-teal-500 to-teal-600 hover:from-teal-600 hover:to-teal-700 rounded-xl shadow-lg shadow-teal-200 transition-all">
                <i class="fas fa-paper-plane mr-1.5"></i>Submit for Review
              </button>
            </div>
          </div>
        </div>
      </div>
    </Teleport>

    <!-- Disseminate Modal (LL-9) -->
    <Teleport to="body">
      <div v-if="showDisseminateModal && disseminateLesson" class="fixed inset-0 z-[100] flex items-center justify-center p-4">
        <div class="absolute inset-0 bg-black/50 backdrop-blur-sm" @click="showDisseminateModal = false"></div>
        <div class="relative w-full max-w-lg bg-white rounded-2xl shadow-2xl overflow-hidden">
          <div class="px-6 py-4 border-b border-gray-100 flex items-center justify-between">
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-blue-500 to-blue-600 flex items-center justify-center text-white shadow-lg shadow-blue-200">
                <i class="fas fa-bullhorn"></i>
              </div>
              <div>
                <h2 class="text-lg font-bold text-gray-900">Notify Teams</h2>
                <p class="text-xs text-gray-500">Disseminate this lesson to relevant teams</p>
              </div>
            </div>
            <button @click="showDisseminateModal = false" class="w-8 h-8 rounded-lg flex items-center justify-center text-gray-400 hover:text-gray-600 hover:bg-gray-100"><i class="fas fa-times"></i></button>
          </div>
          <div class="p-6">
            <div class="mb-4 p-3 bg-gray-50 rounded-xl">
              <div class="text-sm font-semibold text-gray-900">{{ disseminateLesson.title }}</div>
              <div class="text-xs text-gray-500 mt-1">{{ disseminateLesson.category }} · {{ disseminateLesson.impact }} Impact</div>
            </div>
            <div class="text-sm font-semibold text-gray-700 mb-3">Select teams to notify:</div>
            <div class="grid grid-cols-2 gap-2">
              <button v-for="team in availableTeams" :key="team.id" @click="disseminateTeams.includes(team.id) ? disseminateTeams.splice(disseminateTeams.indexOf(team.id), 1) : disseminateTeams.push(team.id)" :class="[disseminateTeams.includes(team.id) ? 'bg-blue-50 border-blue-300 text-blue-700' : 'bg-white border-gray-200 text-gray-600 hover:bg-gray-50', 'flex items-center gap-2.5 px-3 py-2.5 rounded-xl border text-sm font-medium transition-all']">
                <i :class="[team.icon, 'text-sm w-4']"></i>
                <span>{{ team.name }}</span>
                <i v-if="disseminateTeams.includes(team.id)" class="fas fa-check text-[10px] ml-auto"></i>
              </button>
            </div>
          </div>
          <div class="px-6 py-4 border-t border-gray-100 flex items-center justify-between bg-gray-50">
            <span class="text-xs text-gray-500">{{ disseminateTeams.length }} team(s) selected</span>
            <div class="flex gap-2">
              <button @click="showDisseminateModal = false" class="px-4 py-2 text-sm font-medium text-gray-600 hover:bg-gray-200 rounded-xl transition-colors">Cancel</button>
              <button @click="sendDissemination" :disabled="disseminateTeams.length === 0" class="px-4 py-2 text-sm font-semibold text-white bg-blue-500 hover:bg-blue-600 disabled:opacity-50 rounded-xl shadow-sm transition-all">
                <i class="fas fa-paper-plane mr-1.5"></i>Send Notifications
              </button>
            </div>
          </div>
        </div>
      </div>
    </Teleport>

    <!-- Sessions List Modal -->
    <Teleport to="body">
      <div v-if="showSessionsListModal" class="fixed inset-0 z-[100] flex items-center justify-center p-4">
        <div class="absolute inset-0 bg-black/50 backdrop-blur-sm" @click="showSessionsListModal = false"></div>
        <div class="relative w-full max-w-2xl max-h-[85vh] bg-white rounded-2xl shadow-2xl overflow-hidden flex flex-col">
          <div class="px-6 py-4 border-b border-gray-100 flex items-center justify-between shrink-0">
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-purple-500 to-purple-600 flex items-center justify-center text-white shadow-lg shadow-purple-200">
                <i class="fas fa-users"></i>
              </div>
              <div>
                <h2 class="text-lg font-bold text-gray-900">Review Sessions</h2>
                <p class="text-xs text-gray-500">{{ scheduledSessions.length }} sessions · {{ scheduledSessions.filter(s => s.status === 'Scheduled').length }} upcoming</p>
              </div>
            </div>
            <div class="flex items-center gap-2">
              <button @click="showSessionsListModal = false; showReviewSessionModal = true" class="px-3 py-1.5 bg-purple-500 text-white text-sm font-semibold rounded-lg hover:bg-purple-600 transition-colors">
                <i class="fas fa-plus mr-1"></i>New Session
              </button>
              <button @click="showSessionsListModal = false" class="w-8 h-8 rounded-lg flex items-center justify-center text-gray-400 hover:text-gray-600 hover:bg-gray-100"><i class="fas fa-times"></i></button>
            </div>
          </div>
          <div class="flex-1 overflow-y-auto p-6">
            <div v-if="scheduledSessions.length === 0" class="text-center py-12">
              <div class="w-14 h-14 rounded-2xl bg-purple-50 flex items-center justify-center mx-auto mb-4">
                <i class="fas fa-calendar-plus text-2xl text-purple-400"></i>
              </div>
              <p class="text-sm font-medium text-gray-900 mb-1">No sessions yet</p>
              <p class="text-xs text-gray-500 mb-4">Schedule a review session to start capturing lessons collaboratively</p>
              <button @click="showSessionsListModal = false; showReviewSessionModal = true" class="px-4 py-2 bg-purple-500 text-white text-sm font-semibold rounded-lg hover:bg-purple-600 transition-colors">
                <i class="fas fa-plus mr-1.5"></i>Schedule First Session
              </button>
            </div>
            <div v-else class="space-y-3">
              <div v-for="session in scheduledSessions" :key="session.id" class="bg-white border border-gray-200 rounded-xl p-4 hover:border-purple-200 hover:shadow-sm transition-all">
                <div class="flex items-start justify-between gap-3">
                  <div class="flex items-start gap-3 flex-1 min-w-0">
                    <div class="w-10 h-10 rounded-xl flex items-center justify-center shrink-0" :class="session.status === 'Completed' ? 'bg-green-50 text-green-600' : session.status === 'In Progress' ? 'bg-amber-50 text-amber-600' : session.status === 'Cancelled' ? 'bg-gray-100 text-gray-400' : 'bg-purple-50 text-purple-600'">
                      <i :class="getSessionTypeInfo(session.type).icon"></i>
                    </div>
                    <div class="flex-1 min-w-0">
                      <div class="flex items-center gap-2 mb-1">
                        <h4 class="text-sm font-semibold text-gray-900 truncate">{{ session.title }}</h4>
                        <span :class="[sessionStatusConfig[session.status]?.color || 'bg-gray-100 text-gray-700', 'px-2 py-0.5 rounded-full text-[10px] font-semibold shrink-0']">
                          <i :class="[sessionStatusConfig[session.status]?.icon, 'mr-1']"></i>{{ session.status }}
                        </span>
                      </div>
                      <div class="flex items-center gap-3 text-xs text-gray-500 mb-2">
                        <span><i class="fas fa-tag mr-1 text-purple-400"></i>{{ getSessionTypeInfo(session.type).label }}</span>
                        <span><i class="fas fa-calendar mr-1"></i>{{ session.date }}</span>
                        <span v-if="session.project"><i class="fas fa-folder mr-1"></i>{{ session.project }}</span>
                      </div>
                      <div class="flex items-center gap-3 text-xs text-gray-400">
                        <span v-if="session.facilitator"><i class="fas fa-user-tie mr-1"></i>{{ session.facilitator }}</span>
                        <span><i class="fas fa-users mr-1"></i>{{ session.participants.length }} participants</span>
                        <span v-if="session.lessonsCount > 0" class="text-teal-600 font-medium"><i class="fas fa-lightbulb mr-1"></i>{{ session.lessonsCount }} lessons captured</span>
                      </div>
                    </div>
                  </div>
                  <div class="flex items-center gap-1.5 shrink-0">
                    <button v-if="session.status === 'Scheduled'" @click="startSession(session)" class="px-2.5 py-1.5 bg-amber-50 text-amber-600 text-xs font-medium rounded-lg hover:bg-amber-100 transition-colors" title="Start Session">
                      <i class="fas fa-play"></i>
                    </button>
                    <button v-if="session.status === 'In Progress'" @click="completeSession(session)" class="px-2.5 py-1.5 bg-green-50 text-green-600 text-xs font-medium rounded-lg hover:bg-green-100 transition-colors" title="Complete">
                      <i class="fas fa-check"></i>
                    </button>
                    <button v-if="session.status === 'Scheduled' || session.status === 'In Progress'" @click="captureLessonFromSession(session)" class="px-2.5 py-1.5 bg-teal-50 text-teal-600 text-xs font-medium rounded-lg hover:bg-teal-100 transition-colors" title="Capture Lesson">
                      <i class="fas fa-lightbulb"></i>
                    </button>
                    <button v-if="session.status === 'Scheduled'" @click="cancelSession(session)" class="px-2.5 py-1.5 bg-gray-50 text-gray-400 text-xs font-medium rounded-lg hover:bg-red-50 hover:text-red-500 transition-colors" title="Cancel">
                      <i class="fas fa-ban"></i>
                    </button>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </Teleport>

    <!-- Review Session Modal (LL-11) -->
    <Teleport to="body">
      <div v-if="showReviewSessionModal" class="fixed inset-0 z-[100] flex items-center justify-center p-4">
        <div class="absolute inset-0 bg-black/50 backdrop-blur-sm" @click="showReviewSessionModal = false"></div>
        <div class="relative w-full max-w-lg bg-white rounded-2xl shadow-2xl overflow-hidden">
          <div class="px-6 py-4 border-b border-gray-100 flex items-center justify-between">
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-purple-500 to-purple-600 flex items-center justify-center text-white shadow-lg shadow-purple-200">
                <i class="fas fa-users"></i>
              </div>
              <div>
                <h2 class="text-lg font-bold text-gray-900">Schedule Review Session</h2>
                <p class="text-xs text-gray-500">Organize a lessons learned capture session</p>
              </div>
            </div>
            <button @click="showReviewSessionModal = false" class="w-8 h-8 rounded-lg flex items-center justify-center text-gray-400 hover:text-gray-600 hover:bg-gray-100"><i class="fas fa-times"></i></button>
          </div>
          <div class="p-6 space-y-4">
            <div>
              <label class="block text-sm font-semibold text-gray-700 mb-1.5">Session Type</label>
              <div class="grid grid-cols-2 gap-2">
                <button v-for="st in sessionTypes" :key="st.id" @click="reviewSession.type = st.id" :class="[reviewSession.type === st.id ? 'bg-purple-50 border-purple-300 text-purple-700' : 'bg-white border-gray-200 text-gray-600 hover:bg-gray-50', 'flex flex-col items-start px-3 py-2.5 rounded-xl border text-left transition-all']">
                  <div class="flex items-center gap-2 text-sm font-semibold"><i :class="st.icon"></i>{{ st.label }}</div>
                  <div class="text-[10px] text-gray-400 mt-0.5">{{ st.description }}</div>
                </button>
              </div>
            </div>
            <div>
              <label class="block text-sm font-semibold text-gray-700 mb-1.5">Session Title <span class="text-red-500">*</span></label>
              <input v-model="reviewSession.title" type="text" placeholder="e.g., Q1 Safety Review" class="w-full px-4 py-2.5 border border-gray-200 rounded-xl text-sm focus:ring-2 focus:ring-purple-200 focus:border-purple-400 transition-all" />
            </div>
            <div class="grid grid-cols-2 gap-3">
              <div>
                <label class="block text-sm font-semibold text-gray-700 mb-1.5">Date <span class="text-red-500">*</span></label>
                <input v-model="reviewSession.date" type="date" class="w-full px-4 py-2.5 border border-gray-200 rounded-xl text-sm focus:ring-2 focus:ring-purple-200 focus:border-purple-400 transition-all" />
              </div>
              <div>
                <label class="block text-sm font-semibold text-gray-700 mb-1.5">Project</label>
                <input v-model="reviewSession.project" type="text" placeholder="Project name" class="w-full px-4 py-2.5 border border-gray-200 rounded-xl text-sm focus:ring-2 focus:ring-purple-200 focus:border-purple-400 transition-all" />
              </div>
            </div>
            <div>
              <label class="block text-sm font-semibold text-gray-700 mb-1.5">Facilitator</label>
              <input v-model="reviewSession.facilitator" type="text" placeholder="Session facilitator name" class="w-full px-4 py-2.5 border border-gray-200 rounded-xl text-sm focus:ring-2 focus:ring-purple-200 focus:border-purple-400 transition-all" />
            </div>
            <div>
              <label class="block text-sm font-semibold text-gray-700 mb-1.5">Participants</label>
              <input v-model="reviewSession.participants" type="text" placeholder="Comma-separated names" class="w-full px-4 py-2.5 border border-gray-200 rounded-xl text-sm focus:ring-2 focus:ring-purple-200 focus:border-purple-400 transition-all" />
            </div>
          </div>
          <div class="px-6 py-4 border-t border-gray-100 flex items-center justify-end gap-2 bg-gray-50">
            <button @click="showReviewSessionModal = false" class="px-4 py-2 text-sm font-medium text-gray-600 hover:bg-gray-200 rounded-xl transition-colors">Cancel</button>
            <button @click="scheduleReviewSession" class="px-5 py-2 text-sm font-semibold text-white bg-gradient-to-r from-purple-500 to-purple-600 hover:from-purple-600 hover:to-purple-700 rounded-xl shadow-lg shadow-purple-200 transition-all">
              <i class="fas fa-calendar-check mr-1.5"></i>Schedule Session
            </button>
          </div>
        </div>
      </div>
    </Teleport>

    <!-- Notification Preferences Modal (LL-17) -->
    <Teleport to="body">
      <div v-if="showNotificationPrefsModal" class="fixed inset-0 z-[100] flex items-center justify-center p-4">
        <div class="absolute inset-0 bg-black/50 backdrop-blur-sm" @click="showNotificationPrefsModal = false"></div>
        <div class="relative w-full max-w-md bg-white rounded-2xl shadow-2xl overflow-hidden">
          <div class="px-6 py-4 border-b border-gray-100 flex items-center justify-between">
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-amber-500 to-orange-500 flex items-center justify-center text-white shadow-lg shadow-amber-200">
                <i class="fas fa-bell"></i>
              </div>
              <div>
                <h2 class="text-lg font-bold text-gray-900">Notification Preferences</h2>
                <p class="text-xs text-gray-500">Choose which lesson events to receive alerts for</p>
              </div>
            </div>
            <button @click="showNotificationPrefsModal = false" class="w-8 h-8 rounded-lg flex items-center justify-center text-gray-400 hover:text-gray-600 hover:bg-gray-100"><i class="fas fa-times"></i></button>
          </div>
          <div class="p-6 space-y-1">
            <label class="flex items-center justify-between p-3 rounded-xl hover:bg-gray-50 cursor-pointer transition-colors">
              <div class="flex items-center gap-3">
                <i class="fas fa-paper-plane text-blue-500 w-5 text-center"></i>
                <div>
                  <div class="text-sm font-medium text-gray-900">Lesson Submitted</div>
                  <div class="text-[11px] text-gray-400">When a new lesson is submitted for review</div>
                </div>
              </div>
              <input v-model="notificationPrefs.onSubmitted" type="checkbox" class="w-4 h-4 text-teal-500 rounded border-gray-300 focus:ring-teal-400" />
            </label>
            <label class="flex items-center justify-between p-3 rounded-xl hover:bg-gray-50 cursor-pointer transition-colors">
              <div class="flex items-center gap-3">
                <i class="fas fa-check-circle text-green-500 w-5 text-center"></i>
                <div>
                  <div class="text-sm font-medium text-gray-900">Lesson Approved</div>
                  <div class="text-[11px] text-gray-400">When your lesson is approved</div>
                </div>
              </div>
              <input v-model="notificationPrefs.onApproved" type="checkbox" class="w-4 h-4 text-teal-500 rounded border-gray-300 focus:ring-teal-400" />
            </label>
            <label class="flex items-center justify-between p-3 rounded-xl hover:bg-gray-50 cursor-pointer transition-colors">
              <div class="flex items-center gap-3">
                <i class="fas fa-times-circle text-red-500 w-5 text-center"></i>
                <div>
                  <div class="text-sm font-medium text-gray-900">Lesson Rejected</div>
                  <div class="text-[11px] text-gray-400">When your lesson is rejected with feedback</div>
                </div>
              </div>
              <input v-model="notificationPrefs.onRejected" type="checkbox" class="w-4 h-4 text-teal-500 rounded border-gray-300 focus:ring-teal-400" />
            </label>
            <label class="flex items-center justify-between p-3 rounded-xl hover:bg-gray-50 cursor-pointer transition-colors">
              <div class="flex items-center gap-3">
                <i class="fas fa-globe text-teal-500 w-5 text-center"></i>
                <div>
                  <div class="text-sm font-medium text-gray-900">Lesson Published</div>
                  <div class="text-[11px] text-gray-400">When a lesson is published to the organization</div>
                </div>
              </div>
              <input v-model="notificationPrefs.onPublished" type="checkbox" class="w-4 h-4 text-teal-500 rounded border-gray-300 focus:ring-teal-400" />
            </label>
            <label class="flex items-center justify-between p-3 rounded-xl hover:bg-gray-50 cursor-pointer transition-colors">
              <div class="flex items-center gap-3">
                <i class="fas fa-user-check text-indigo-500 w-5 text-center"></i>
                <div>
                  <div class="text-sm font-medium text-gray-900">Action Assigned</div>
                  <div class="text-[11px] text-gray-400">When you are assigned an action item</div>
                </div>
              </div>
              <input v-model="notificationPrefs.onActionAssigned" type="checkbox" class="w-4 h-4 text-teal-500 rounded border-gray-300 focus:ring-teal-400" />
            </label>
            <label class="flex items-center justify-between p-3 rounded-xl hover:bg-gray-50 cursor-pointer transition-colors">
              <div class="flex items-center gap-3">
                <i class="fas fa-exclamation-triangle text-red-500 w-5 text-center"></i>
                <div>
                  <div class="text-sm font-medium text-gray-900">Action Overdue</div>
                  <div class="text-[11px] text-gray-400">When an action passes its due date</div>
                </div>
              </div>
              <input v-model="notificationPrefs.onActionOverdue" type="checkbox" class="w-4 h-4 text-teal-500 rounded border-gray-300 focus:ring-teal-400" />
            </label>
            <label class="flex items-center justify-between p-3 rounded-xl hover:bg-gray-50 cursor-pointer transition-colors">
              <div class="flex items-center gap-3">
                <i class="fas fa-check-double text-emerald-500 w-5 text-center"></i>
                <div>
                  <div class="text-sm font-medium text-gray-900">Actions Complete</div>
                  <div class="text-[11px] text-gray-400">When all actions for a lesson are completed</div>
                </div>
              </div>
              <input v-model="notificationPrefs.onActionsComplete" type="checkbox" class="w-4 h-4 text-teal-500 rounded border-gray-300 focus:ring-teal-400" />
            </label>
            <label class="flex items-center justify-between p-3 rounded-xl hover:bg-gray-50 cursor-pointer transition-colors">
              <div class="flex items-center gap-3">
                <i class="fas fa-comment text-cyan-500 w-5 text-center"></i>
                <div>
                  <div class="text-sm font-medium text-gray-900">New Comment</div>
                  <div class="text-[11px] text-gray-400">When someone comments on your lesson</div>
                </div>
              </div>
              <input v-model="notificationPrefs.onCommentAdded" type="checkbox" class="w-4 h-4 text-teal-500 rounded border-gray-300 focus:ring-teal-400" />
            </label>
            <label class="flex items-center justify-between p-3 rounded-xl hover:bg-gray-50 cursor-pointer transition-colors">
              <div class="flex items-center gap-3">
                <i class="fas fa-bell text-purple-500 w-5 text-center"></i>
                <div>
                  <div class="text-sm font-medium text-gray-900">Followed Lesson Update</div>
                  <div class="text-[11px] text-gray-400">When a lesson you follow has a status change</div>
                </div>
              </div>
              <input v-model="notificationPrefs.onFollowedUpdate" type="checkbox" class="w-4 h-4 text-teal-500 rounded border-gray-300 focus:ring-teal-400" />
            </label>
          </div>
          <div class="px-6 py-4 border-t border-gray-100 flex items-center justify-end gap-2 bg-gray-50">
            <button @click="showNotificationPrefsModal = false" class="px-5 py-2 text-sm font-semibold text-white bg-gradient-to-r from-teal-500 to-teal-600 rounded-xl shadow-sm transition-all hover:from-teal-600 hover:to-teal-700">
              Save Preferences
            </button>
          </div>
        </div>
      </div>
    </Teleport>
  </div>
</template>

<style scoped>
/* Featured Grid Layout */
.featured-grid {
  display: grid;
  grid-template-columns: 1fr;
  gap: 1.5rem;
}

@media (min-width: 1024px) {
  .featured-grid {
    grid-template-columns: 1fr 380px;
  }
}

.featured-sidebar {
  display: none;
}

@media (min-width: 1024px) {
  .featured-sidebar {
    display: flex;
  }
}

/* Featured Insights Section */
.ll-featured-section {
  background: white;
  border-radius: 1.5rem;
  padding: 1.75rem;
  margin-bottom: 1.5rem;
  border: 1px solid #e5e7eb;
  box-shadow: 0 4px 24px rgba(0, 0, 0, 0.06);
}

.ll-featured-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.5rem; }
.ll-featured-title-wrap { display: flex; align-items: center; gap: 1rem; }
.ll-featured-icon { width: 52px; height: 52px; border-radius: 14px; background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%); display: flex; align-items: center; justify-content: center; font-size: 1.5rem; color: white; box-shadow: 0 4px 14px rgba(20, 184, 166, 0.35); }
.ll-featured-title { font-size: 1.5rem; font-weight: 700; color: #111827; margin: 0; }
.ll-featured-subtitle { font-size: 0.9rem; color: #6b7280; margin: 0.25rem 0 0; }
.ll-featured-nav { display: flex; align-items: center; gap: 0.75rem; }
.ll-featured-counter { font-size: 0.85rem; font-weight: 600; color: #14b8a6; background: #f0fdfa; padding: 0.5rem 1rem; border-radius: 9999px; }

.ll-nav-btn { width: 40px; height: 40px; border-radius: 12px; border: 1px solid #e5e7eb; background: white; color: #6b7280; display: flex; align-items: center; justify-content: center; cursor: pointer; transition: all 0.2s ease; }
.ll-nav-btn:hover:not(:disabled) { background: #14b8a6; border-color: #14b8a6; color: white; transform: scale(1.05); }
.ll-nav-btn:disabled { opacity: 0.4; cursor: not-allowed; }

/* Two Column Layout */
.ll-featured-content { display: grid; grid-template-columns: 1fr 380px; gap: 1.5rem; }

/* Main Featured Card */
.ll-main-card-wrap { display: flex; flex-direction: column; gap: 1rem; }
.ll-main-card { position: relative; border-radius: 1.25rem; overflow: hidden; cursor: pointer; transition: all 0.4s ease; background: linear-gradient(135deg, #0f766e 0%, #0d9488 50%, #14b8a6 100%); min-height: 420px; }
.ll-main-card:hover { transform: translateY(-6px); box-shadow: 0 20px 50px rgba(20, 184, 166, 0.3); }
.ll-card-glow { position: absolute; top: -50%; right: -20%; width: 60%; height: 100%; background: radial-gradient(circle, rgba(255,255,255,0.15) 0%, transparent 70%); pointer-events: none; }
.ll-card-pattern { position: absolute; inset: 0; background-image: url("data:image/svg+xml,%3Csvg width='60' height='60' viewBox='0 0 60 60' xmlns='http://www.w3.org/2000/svg'%3E%3Cg fill='none' fill-rule='evenodd'%3E%3Cg fill='%23ffffff' fill-opacity='0.05'%3E%3Cpath d='M36 34v-4h-2v4h-4v2h4v4h2v-4h4v-2h-4zm0-30V0h-2v4h-4v2h4v4h2V6h4V4h-4zM6 34v-4H4v4H0v2h4v4h2v-4h4v-2H6zM6 4V0H4v4H0v2h4v4h2V6h4V4H6z'/%3E%3C/g%3E%3C/g%3E%3C/svg%3E"); pointer-events: none; }
.ll-main-card-inner { position: relative; z-index: 1; padding: 2rem; color: white; display: flex; flex-direction: column; height: 100%; min-height: 420px; }
.ll-main-top { display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 1.5rem; }
.ll-main-badges { display: flex; gap: 0.625rem; flex-wrap: wrap; }
.ll-main-featured-badge { display: inline-flex; align-items: center; gap: 0.375rem; padding: 0.5rem 1rem; background: rgba(255, 255, 255, 0.2); backdrop-filter: blur(8px); border-radius: 9999px; font-size: 0.8rem; font-weight: 600; }
.ll-main-featured-badge i { color: #99f6e4; }
.ll-main-impact-badge { display: inline-flex; align-items: center; gap: 0.375rem; padding: 0.5rem 1rem; border-radius: 9999px; font-size: 0.8rem; font-weight: 600; text-transform: capitalize; }
.ll-main-impact-badge.impact-Critical { background: rgba(239, 68, 68, 0.9); }
.ll-main-impact-badge.impact-High { background: rgba(249, 115, 22, 0.9); }
.ll-main-impact-badge.impact-Medium { background: rgba(245, 158, 11, 0.9); }
.ll-main-impact-badge.impact-Low { background: rgba(34, 197, 94, 0.9); }
.ll-main-category { display: inline-flex; align-items: center; gap: 0.375rem; padding: 0.5rem 1rem; background: rgba(255, 255, 255, 0.15); backdrop-filter: blur(4px); border-radius: 9999px; font-size: 0.8rem; font-weight: 500; }
.ll-main-content { flex: 1; }
.ll-main-title { font-size: 1.75rem; font-weight: 700; margin: 0 0 0.75rem; line-height: 1.3; }
.ll-main-summary { font-size: 1.05rem; opacity: 0.9; line-height: 1.6; margin: 0 0 1.25rem; }

/* Impact Highlight in Featured Card */
.ll-main-impact { display: flex; align-items: flex-start; gap: 1rem; padding: 1rem 1.25rem; background: rgba(255, 255, 255, 0.12); border-radius: 1rem; margin-bottom: 1.25rem; border: 1px solid rgba(255, 255, 255, 0.1); }
.ll-impact-icon-wrap { width: 40px; height: 40px; border-radius: 10px; background: rgba(255, 255, 255, 0.2); display: flex; align-items: center; justify-content: center; font-size: 1rem; flex-shrink: 0; }
.ll-impact-content { display: flex; flex-direction: column; gap: 0.25rem; }
.ll-impact-label { font-size: 0.75rem; font-weight: 600; text-transform: uppercase; letter-spacing: 0.5px; opacity: 0.8; }
.ll-impact-text { font-size: 0.95rem; line-height: 1.5; }

/* Takeaways */
.ll-main-takeaways { margin-bottom: 1.25rem; }
.ll-takeaway-header { display: flex; align-items: center; gap: 0.5rem; font-size: 0.85rem; font-weight: 600; margin-bottom: 0.75rem; opacity: 0.9; }
.ll-takeaway-header i { color: #99f6e4; }
.ll-takeaway-list { display: flex; flex-direction: column; gap: 0.5rem; }
.ll-takeaway-item { display: flex; align-items: flex-start; gap: 0.75rem; padding: 0.625rem 0.875rem; background: rgba(255, 255, 255, 0.1); border-radius: 0.75rem; }
.ll-takeaway-num { width: 24px; height: 24px; border-radius: 50%; background: rgba(255, 255, 255, 0.2); display: flex; align-items: center; justify-content: center; font-size: 0.75rem; font-weight: 700; flex-shrink: 0; }
.ll-takeaway-text { font-size: 0.9rem; line-height: 1.4; }

/* Footer */
.ll-main-footer { display: flex; justify-content: space-between; align-items: center; padding-top: 1.25rem; border-top: 1px solid rgba(255, 255, 255, 0.15); margin-bottom: 1rem; }
.ll-main-author { display: flex; align-items: center; gap: 0.75rem; }
.ll-main-avatar { width: 44px; height: 44px; border-radius: 12px; background: rgba(255, 255, 255, 0.2); display: flex; align-items: center; justify-content: center; font-size: 0.9rem; font-weight: 600; }
.ll-main-author-info { display: flex; flex-direction: column; }
.ll-main-author-name { font-size: 0.95rem; font-weight: 600; }
.ll-main-author-meta { font-size: 0.8rem; opacity: 0.8; }
.ll-main-stats { display: flex; gap: 1rem; }
.ll-main-stat { display: flex; align-items: center; gap: 0.375rem; font-size: 0.9rem; opacity: 0.9; }

/* CTA */
.ll-main-cta { display: inline-flex; align-items: center; justify-content: center; gap: 0.5rem; padding: 0.625rem 1.25rem; background: white; border: none; border-radius: 0.625rem; font-size: 0.85rem; font-weight: 600; color: #0d9488; cursor: pointer; transition: all 0.2s ease; }
.ll-main-cta:hover { background: #f0fdfa; transform: translateY(-2px); box-shadow: 0 8px 20px rgba(0, 0, 0, 0.15); }
.ll-main-cta i { transition: transform 0.2s ease; }
.ll-main-cta:hover i { transform: translateX(4px); }

/* Progress Dots */
.ll-progress-dots { display: flex; justify-content: center; gap: 0.5rem; }
.ll-progress-dot { width: 10px; height: 10px; border-radius: 50%; border: none; background: #e5e7eb; cursor: pointer; transition: all 0.2s ease; padding: 0; }
.ll-progress-dot:hover { background: #99f6e4; }
.ll-progress-dot.active { background: #14b8a6; width: 32px; border-radius: 5px; }

/* Next Lessons Sidebar */
.ll-next-lessons { display: flex; flex-direction: column; background: #f8fafc; border-radius: 1.25rem; padding: 1.25rem; border: 1px solid #e5e7eb; }
.ll-next-header { display: flex; align-items: center; gap: 0.625rem; font-size: 1rem; font-weight: 700; color: #111827; margin-bottom: 1rem; padding-bottom: 0.75rem; border-bottom: 1px solid #e5e7eb; }
.ll-next-header i { color: #14b8a6; }
.ll-next-list { display: flex; flex-direction: column; gap: 0.75rem; flex: 1; }
.ll-next-card { display: flex; align-items: flex-start; gap: 0.875rem; padding: 1rem; background: white; border-radius: 1rem; border: 1px solid #e5e7eb; cursor: pointer; transition: all 0.25s ease; }
.ll-next-card:hover { border-color: #14b8a6; box-shadow: 0 4px 16px rgba(20, 184, 166, 0.12); transform: translateX(4px); }
.ll-next-index { width: 28px; height: 28px; border-radius: 8px; background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%); color: white; display: flex; align-items: center; justify-content: center; font-size: 0.8rem; font-weight: 700; flex-shrink: 0; }
.ll-next-content { flex: 1; min-width: 0; }
.ll-next-meta { display: flex; align-items: center; gap: 0.5rem; margin-bottom: 0.375rem; }
.ll-next-category { display: inline-flex; align-items: center; gap: 0.25rem; font-size: 0.7rem; color: #6b7280; }
.ll-next-category i { font-size: 0.65rem; color: #14b8a6; }
.ll-next-impact { font-size: 0.65rem; font-weight: 600; text-transform: capitalize; padding: 0.125rem 0.5rem; border-radius: 9999px; }
.ll-next-impact.impact-Critical { background: #fee2e2; color: #dc2626; }
.ll-next-impact.impact-High { background: #ffedd5; color: #ea580c; }
.ll-next-impact.impact-Medium { background: #fef3c7; color: #d97706; }
.ll-next-impact.impact-Low { background: #dcfce7; color: #16a34a; }
.ll-next-title { font-size: 0.9rem; font-weight: 600; color: #111827; margin: 0 0 0.375rem; line-height: 1.3; display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; }
.ll-next-summary { font-size: 0.8rem; color: #6b7280; margin: 0 0 0.5rem; line-height: 1.4; display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; }
.ll-next-footer { display: flex; align-items: center; justify-content: space-between; }
.ll-next-author { display: flex; align-items: center; gap: 0.375rem; font-size: 0.75rem; color: #6b7280; }
.ll-next-avatar { width: 20px; height: 20px; border-radius: 6px; background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%); color: white; display: flex; align-items: center; justify-content: center; font-size: 0.6rem; font-weight: 600; }
.ll-next-stats { display: flex; align-items: center; gap: 0.25rem; font-size: 0.75rem; color: #9ca3af; }
.ll-next-arrow { width: 28px; height: 28px; border-radius: 8px; background: #f3f4f6; display: flex; align-items: center; justify-content: center; color: #9ca3af; font-size: 0.7rem; flex-shrink: 0; transition: all 0.2s ease; }
.ll-next-card:hover .ll-next-arrow { background: #14b8a6; color: white; }

/* View All Button */
.ll-view-all-btn { display: flex; align-items: center; justify-content: center; gap: 0.5rem; width: 100%; padding: 0.875rem; margin-top: 0.75rem; background: white; border: 1px solid #14b8a6; border-radius: 0.75rem; font-size: 0.85rem; font-weight: 600; color: #14b8a6; cursor: pointer; transition: all 0.2s ease; }
.ll-view-all-btn:hover { background: #14b8a6; color: white; }
.ll-view-all-btn i { transition: transform 0.2s ease; }
.ll-view-all-btn:hover i { transform: translateX(4px); }

/* Modal styles */
.ll-modal-overlay { position: fixed; inset: 0; background: rgba(0, 0, 0, 0.5); backdrop-filter: blur(4px); display: flex; align-items: center; justify-content: center; z-index: 1000; padding: 2rem; }
.ll-modal { background: white; border-radius: 1.25rem; width: 100%; max-width: 720px; max-height: 90vh; overflow: hidden; display: flex; flex-direction: column; box-shadow: 0 25px 50px rgba(0, 0, 0, 0.25); }
.ll-modal-header { display: flex; justify-content: space-between; align-items: flex-start; padding: 1.25rem 1.5rem; border-bottom: 1px solid #e5e7eb; background: linear-gradient(135deg, #f0fdfa 0%, #ccfbf1 100%); }
.ll-modal-badges { display: flex; gap: 0.5rem; flex-wrap: wrap; }
.ll-modal-impact { display: flex; align-items: center; gap: 0.375rem; padding: 0.375rem 0.75rem; border-radius: 9999px; font-size: 0.75rem; font-weight: 600; text-transform: capitalize; }
.ll-modal-impact.impact-Critical { background: #fee2e2; color: #dc2626; }
.ll-modal-impact.impact-High { background: #ffedd5; color: #ea580c; }
.ll-modal-impact.impact-Medium { background: #fef3c7; color: #d97706; }
.ll-modal-impact.impact-Low { background: #dcfce7; color: #16a34a; }
.ll-modal-category { display: flex; align-items: center; gap: 0.375rem; padding: 0.375rem 0.75rem; background: rgba(255, 255, 255, 0.6); border-radius: 9999px; font-size: 0.75rem; font-weight: 500; color: #374151; }
.ll-modal-close { width: 36px; height: 36px; display: flex; align-items: center; justify-content: center; border: none; background: rgba(255, 255, 255, 0.6); border-radius: 50%; color: #6b7280; cursor: pointer; transition: all 0.2s ease; }
.ll-modal-close:hover { background: white; color: #374151; }
.ll-modal-body { flex: 1; overflow-y: auto; padding: 1.5rem; }
.ll-modal-title { font-size: 1.5rem; font-weight: 700; color: #111827; margin: 0 0 1rem; line-height: 1.3; }
.ll-modal-meta { display: flex; align-items: center; justify-content: space-between; gap: 1rem; margin-bottom: 1.5rem; flex-wrap: wrap; }
.ll-modal-author { display: flex; align-items: center; gap: 0.75rem; }
.ll-modal-avatar { width: 44px; height: 44px; border-radius: 50%; background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%); color: white; display: flex; align-items: center; justify-content: center; font-size: 0.9rem; font-weight: 600; }
.ll-modal-author-name { display: block; font-size: 0.95rem; font-weight: 500; color: #374151; }
.ll-modal-author-dept { display: block; font-size: 0.8rem; color: #9ca3af; }
.ll-modal-info { display: flex; gap: 1rem; flex-wrap: wrap; }
.ll-modal-info span { display: flex; align-items: center; gap: 0.375rem; font-size: 0.85rem; color: #6b7280; }
.ll-modal-stats { display: flex; gap: 1.5rem; padding: 1rem; background: #f9fafb; border-radius: 0.75rem; margin-bottom: 1.5rem; }
.ll-modal-stats span { display: flex; align-items: center; gap: 0.375rem; font-size: 0.85rem; color: #6b7280; }
.ll-modal-section { margin-bottom: 1.5rem; }
.ll-modal-section h4 { display: flex; align-items: center; gap: 0.5rem; font-size: 0.9rem; font-weight: 600; color: #374151; margin: 0 0 0.75rem; }
.ll-modal-section h4 i { color: #14b8a6; }
.ll-modal-section p { font-size: 0.9rem; color: #4b5563; line-height: 1.7; margin: 0; }
.ll-impact-highlight { padding: 1rem; background: linear-gradient(135deg, #f0fdfa 0%, #ccfbf1 100%); border-radius: 0.75rem; font-size: 0.95rem; color: #0f766e; font-weight: 500; line-height: 1.5; border-left: 4px solid #14b8a6; }
.ll-recommendations { list-style: none; padding: 0; margin: 0; display: flex; flex-direction: column; gap: 0.625rem; }
.ll-recommendations li { display: flex; align-items: flex-start; gap: 0.75rem; padding: 0.75rem 1rem; background: #f9fafb; border-radius: 0.75rem; font-size: 0.9rem; color: #374151; line-height: 1.5; }
.ll-recommendations li i { color: #14b8a6; margin-top: 0.25rem; flex-shrink: 0; }
.ll-modal-tags { display: flex; flex-wrap: wrap; gap: 0.5rem; }
.ll-modal-tag { padding: 0.375rem 0.75rem; background: #f0fdfa; color: #0f766e; border-radius: 9999px; font-size: 0.75rem; font-weight: 500; border: 1px solid #99f6e4; }
.ll-modal-footer { display: flex; align-items: center; justify-content: space-between; padding: 1rem 1.5rem; border-top: 1px solid #e5e7eb; background: #f9fafb; }
.ll-modal-actions { display: flex; gap: 0.75rem; }
.ll-action-btn { display: flex; align-items: center; gap: 0.5rem; padding: 0.5rem 1rem; border: 1px solid #e5e7eb; border-radius: 0.5rem; background: white; font-size: 0.85rem; color: #6b7280; cursor: pointer; transition: all 0.2s ease; }
.ll-action-btn:hover { background: #f3f4f6; color: #374151; }
.ll-action-btn.active { border-color: #14b8a6; color: #0d9488; background: #f0fdfa; }
.ll-close-btn { padding: 0.5rem 1.25rem; background: #14b8a6; color: white; border: none; border-radius: 0.5rem; font-size: 0.85rem; font-weight: 600; cursor: pointer; transition: all 0.2s ease; }
.ll-close-btn:hover { background: #0d9488; }

/* Responsive */
@media (max-width: 1024px) {
  .ll-featured-content { grid-template-columns: 1fr; }
  .ll-next-lessons { display: none; }
  .ll-featured-header { flex-direction: column; gap: 1rem; align-items: flex-start; }
}

@media (max-width: 768px) {
  .ll-featured-section { padding: 1.25rem; }
  .ll-main-card-inner { padding: 1.5rem; min-height: auto; }
  .ll-main-title { font-size: 1.35rem; }
  .ll-main-summary { font-size: 0.95rem; }
  .ll-main-footer { flex-direction: column; gap: 1rem; align-items: flex-start; }
  .ll-modal { max-width: 95%; }
}
</style>
