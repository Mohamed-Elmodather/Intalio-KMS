<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useReducedMotion } from '@/composables/useReducedMotion'
import { PageHeader, StatsBar, Avatar } from '@/components/base'
import type { BreadcrumbItem } from '@/components/base/PageHeader.vue'
import type { StatItem } from '@/components/base/StatsBar.vue'
import ErrorState from '@/components/base/ErrorState.vue'
import Button from 'primevue/button'
import InputText from 'primevue/inputtext'
import Textarea from 'primevue/textarea'
import Dropdown from 'primevue/dropdown'
import Dialog from 'primevue/dialog'
import Skeleton from 'primevue/skeleton'
import Chips from 'primevue/chips'

const router = useRouter()
const { locale } = useI18n()
const prefersReducedMotion = useReducedMotion()

// State
const loading = ref(true)
const error = ref<Error | null>(null)
const isContentVisible = ref(false)
const showCreateDialog = ref(false)
const showFiltersPanel = ref(false)
const viewMode = ref<'grid' | 'list'>('grid')

// RTL support
const isRTL = computed(() => locale.value === 'ar')

// Animation control
const shouldAnimate = computed(() => !prefersReducedMotion.value)

// Breadcrumbs for PageHeader
const breadcrumbs = computed<BreadcrumbItem[]>(() => [
  { label: 'Home', labelArabic: 'الرئيسية', to: '/dashboard' },
  { label: isRTL.value ? 'الدروس المستفادة' : 'Lessons Learned' }
])

// Configuration objects
const CATEGORY_CONFIG: Record<string, { icon: string; color: string; bgColor: string; label: string; labelAr: string }> = {
  Process: { icon: 'pi pi-cog', color: '#3b82f6', bgColor: 'rgba(59, 130, 246, 0.1)', label: 'Process', labelAr: 'العملية' },
  Technical: { icon: 'pi pi-code', color: '#8b5cf6', bgColor: 'rgba(139, 92, 246, 0.1)', label: 'Technical', labelAr: 'تقني' },
  Communication: { icon: 'pi pi-comments', color: '#10b981', bgColor: 'rgba(16, 185, 129, 0.1)', label: 'Communication', labelAr: 'التواصل' },
  TeamManagement: { icon: 'pi pi-users', color: '#f59e0b', bgColor: 'rgba(245, 158, 11, 0.1)', label: 'Team Management', labelAr: 'إدارة الفريق' },
  RiskManagement: { icon: 'pi pi-shield', color: '#ef4444', bgColor: 'rgba(239, 68, 68, 0.1)', label: 'Risk Management', labelAr: 'إدارة المخاطر' },
  VendorManagement: { icon: 'pi pi-building', color: '#06b6d4', bgColor: 'rgba(6, 182, 212, 0.1)', label: 'Vendor Management', labelAr: 'إدارة الموردين' },
  Other: { icon: 'pi pi-ellipsis-h', color: '#6b7280', bgColor: 'rgba(107, 114, 128, 0.1)', label: 'Other', labelAr: 'أخرى' }
}

const IMPACT_CONFIG: Record<string, { color: string; bgColor: string; icon: string; label: string; labelAr: string }> = {
  Critical: { color: '#dc2626', bgColor: 'rgba(220, 38, 38, 0.1)', icon: 'pi pi-exclamation-triangle', label: 'Critical', labelAr: 'حرج' },
  High: { color: '#f59e0b', bgColor: 'rgba(245, 158, 11, 0.1)', icon: 'pi pi-arrow-up', label: 'High', labelAr: 'عالي' },
  Medium: { color: '#3b82f6', bgColor: 'rgba(59, 130, 246, 0.1)', icon: 'pi pi-minus', label: 'Medium', labelAr: 'متوسط' },
  Low: { color: '#10b981', bgColor: 'rgba(16, 185, 129, 0.1)', icon: 'pi pi-arrow-down', label: 'Low', labelAr: 'منخفض' }
}

const STATUS_CONFIG: Record<string, { color: string; bgColor: string; label: string; labelAr: string }> = {
  Published: { color: '#10b981', bgColor: 'rgba(16, 185, 129, 0.1)', label: 'Published', labelAr: 'منشور' },
  Draft: { color: '#6b7280', bgColor: 'rgba(107, 114, 128, 0.1)', label: 'Draft', labelAr: 'مسودة' },
  InReview: { color: '#f59e0b', bgColor: 'rgba(245, 158, 11, 0.1)', label: 'In Review', labelAr: 'قيد المراجعة' }
}

const filters = reactive({
  search: '',
  category: null as string | null,
  impact: null as string | null,
  sortBy: 'recent'
})

const newLesson = ref({
  title: '',
  titleArabic: '',
  context: '',
  challenge: '',
  solution: '',
  outcome: '',
  recommendations: '',
  category: '',
  impact: '',
  tags: [] as string[]
})

const categoryOptions = Object.entries(CATEGORY_CONFIG).map(([value, config]) => ({
  value,
  label: config.label,
  labelAr: config.labelAr
}))

const impactOptions = Object.entries(IMPACT_CONFIG).map(([value, config]) => ({
  value,
  label: config.label,
  labelAr: config.labelAr
}))

const sortOptions = [
  { label: 'Most Recent', labelAr: 'الأحدث', value: 'recent' },
  { label: 'Most Viewed', labelAr: 'الأكثر مشاهدة', value: 'views' },
  { label: 'Most Useful', labelAr: 'الأكثر فائدة', value: 'useful' },
  { label: 'Highest Impact', labelAr: 'الأعلى تأثيرًا', value: 'impact' }
]

const stats = ref({
  totalLessons: 45,
  publishedLessons: 38,
  totalUseful: 1234,
  totalViews: 8765
})

// StatsBar items
const statsBarItems = computed<StatItem[]>(() => [
  {
    icon: 'pi pi-book',
    value: stats.value.totalLessons,
    label: 'Total Lessons',
    labelArabic: 'إجمالي الدروس',
    colorClass: 'primary'
  },
  {
    icon: 'pi pi-check-circle',
    value: stats.value.publishedLessons,
    label: 'Published',
    labelArabic: 'منشور',
    colorClass: 'success'
  },
  {
    icon: 'pi pi-thumbs-up',
    value: stats.value.totalUseful,
    label: 'Marked Useful',
    labelArabic: 'مفيد',
    colorClass: 'warning'
  },
  {
    icon: 'pi pi-eye',
    value: stats.value.totalViews,
    label: 'Total Views',
    labelArabic: 'المشاهدات',
    colorClass: 'info'
  }
])

interface Lesson {
  id: string
  title: string
  titleArabic?: string
  descriptionPreview: string
  descriptionPreviewAr?: string
  category: string
  impact: string
  status: string
  authorName: string
  authorNameAr?: string
  authorAvatarUrl?: string
  projectName: string
  projectNameAr?: string
  viewCount: number
  usefulCount: number
  isMarkedUseful: boolean
  tags: string[]
  createdAt: string
}

const lessons = ref<Lesson[]>([])

// Computed helpers
const getCategoryConfig = (category: string) => {
  return CATEGORY_CONFIG[category] || CATEGORY_CONFIG.Other
}

const getImpactConfig = (impact: string) => {
  return IMPACT_CONFIG[impact] || IMPACT_CONFIG.Medium
}

const getStatusConfig = (status: string) => {
  return STATUS_CONFIG[status] || STATUS_CONFIG.Draft
}

const getTitle = (lesson: Lesson) => {
  return isRTL.value && lesson.titleArabic ? lesson.titleArabic : lesson.title
}

const getDescription = (lesson: Lesson) => {
  return isRTL.value && lesson.descriptionPreviewAr ? lesson.descriptionPreviewAr : lesson.descriptionPreview
}

const getAuthorName = (lesson: Lesson) => {
  return isRTL.value && lesson.authorNameAr ? lesson.authorNameAr : lesson.authorName
}

const getProjectName = (lesson: Lesson) => {
  return isRTL.value && lesson.projectNameAr ? lesson.projectNameAr : lesson.projectName
}

const formatDate = (dateString: string) => {
  const date = new Date(dateString)
  return new Intl.DateTimeFormat(isRTL.value ? 'ar-SA' : 'en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  }).format(date)
}

const formatRelativeTime = (date: string) => {
  const now = new Date()
  const then = new Date(date)
  const diffMs = now.getTime() - then.getTime()
  const diffDays = Math.floor(diffMs / (1000 * 60 * 60 * 24))

  if (diffDays < 1) return isRTL.value ? 'اليوم' : 'Today'
  if (diffDays === 1) return isRTL.value ? 'أمس' : 'Yesterday'
  if (diffDays < 7) return isRTL.value ? `منذ ${diffDays} أيام` : `${diffDays}d ago`
  if (diffDays < 30) return isRTL.value ? `منذ ${Math.floor(diffDays / 7)} أسبوع` : `${Math.floor(diffDays / 7)}w ago`
  return formatDate(date)
}

const formatNumber = (num: number) => {
  if (num >= 1000) {
    return (num / 1000).toFixed(1) + 'K'
  }
  return num.toString()
}

// Actions
const toggleUseful = (lesson: Lesson, event: Event) => {
  event.stopPropagation()
  lesson.isMarkedUseful = !lesson.isMarkedUseful
  lesson.usefulCount += lesson.isMarkedUseful ? 1 : -1
}

const goToLesson = (id: string) => {
  router.push({ name: 'lesson-detail', params: { id } })
}

const clearFilters = () => {
  filters.search = ''
  filters.category = null
  filters.impact = null
  filters.sortBy = 'recent'
}

const saveAsDraft = () => {
  console.log('Saving as draft:', newLesson.value)
  showCreateDialog.value = false
  resetForm()
}

const submitForReview = () => {
  console.log('Submitting for review:', newLesson.value)
  showCreateDialog.value = false
  resetForm()
}

const resetForm = () => {
  newLesson.value = {
    title: '',
    titleArabic: '',
    context: '',
    challenge: '',
    solution: '',
    outcome: '',
    recommendations: '',
    category: '',
    impact: '',
    tags: []
  }
}

// Load lessons data
const LOADING_DELAY = 600

async function loadLessons() {
  try {
    error.value = null
    loading.value = true

    await new Promise(resolve => setTimeout(resolve, LOADING_DELAY))

    lessons.value = [
    {
      id: '1',
      title: 'Effective Crowd Management at Large Events',
      titleArabic: 'إدارة الحشود الفعالة في الفعاليات الكبيرة',
      descriptionPreview: 'Key insights from managing crowd flow during the opening ceremony rehearsal, including zone-based entry systems and real-time monitoring...',
      descriptionPreviewAr: 'رؤى رئيسية من إدارة تدفق الحشود خلال بروفة حفل الافتتاح، بما في ذلك أنظمة الدخول القائمة على المناطق والمراقبة في الوقت الفعلي...',
      category: 'Process',
      impact: 'High',
      status: 'Published',
      authorName: 'Mohammed Al-Rashid',
      authorNameAr: 'محمد الراشد',
      projectName: 'Opening Ceremony',
      projectNameAr: 'حفل الافتتاح',
      viewCount: 456,
      usefulCount: 89,
      isMarkedUseful: true,
      tags: ['Crowd Management', 'Safety', 'Events', 'Operations'],
      createdAt: new Date(Date.now() - 10 * 24 * 60 * 60 * 1000).toISOString()
    },
    {
      id: '2',
      title: 'Vendor Communication Best Practices',
      titleArabic: 'أفضل ممارسات التواصل مع الموردين',
      descriptionPreview: 'Lessons from coordinating with multiple vendors during venue setup, establishing clear communication channels and escalation procedures...',
      descriptionPreviewAr: 'دروس من التنسيق مع موردين متعددين أثناء إعداد المكان، وإنشاء قنوات اتصال واضحة وإجراءات التصعيد...',
      category: 'VendorManagement',
      impact: 'Medium',
      status: 'Published',
      authorName: 'Sara Ali',
      authorNameAr: 'سارة علي',
      projectName: 'Venue Setup',
      projectNameAr: 'إعداد المكان',
      viewCount: 234,
      usefulCount: 45,
      isMarkedUseful: false,
      tags: ['Vendors', 'Communication', 'Coordination'],
      createdAt: new Date(Date.now() - 15 * 24 * 60 * 60 * 1000).toISOString()
    },
    {
      id: '3',
      title: 'Technology Integration Challenges',
      titleArabic: 'تحديات تكامل التقنية',
      descriptionPreview: 'Technical challenges faced during system integration between ticketing, access control, and crowd monitoring systems...',
      descriptionPreviewAr: 'التحديات التقنية التي واجهتها أثناء تكامل النظام بين أنظمة التذاكر والتحكم في الوصول ومراقبة الحشود...',
      category: 'Technical',
      impact: 'Critical',
      status: 'Published',
      authorName: 'Ahmed Hassan',
      authorNameAr: 'أحمد حسن',
      projectName: 'IT Infrastructure',
      projectNameAr: 'البنية التحتية لتقنية المعلومات',
      viewCount: 567,
      usefulCount: 123,
      isMarkedUseful: false,
      tags: ['Technology', 'Integration', 'Systems', 'IT'],
      createdAt: new Date(Date.now() - 20 * 24 * 60 * 60 * 1000).toISOString()
    },
    {
      id: '4',
      title: 'Emergency Response Coordination',
      titleArabic: 'تنسيق الاستجابة للطوارئ',
      descriptionPreview: 'How we improved emergency response times by implementing a centralized command center and standardized protocols...',
      descriptionPreviewAr: 'كيف قمنا بتحسين أوقات الاستجابة للطوارئ من خلال تنفيذ مركز قيادة مركزي وبروتوكولات موحدة...',
      category: 'RiskManagement',
      impact: 'Critical',
      status: 'Published',
      authorName: 'Fatima Khan',
      authorNameAr: 'فاطمة خان',
      projectName: 'Safety Operations',
      projectNameAr: 'عمليات السلامة',
      viewCount: 678,
      usefulCount: 156,
      isMarkedUseful: true,
      tags: ['Emergency', 'Safety', 'Protocols', 'Response'],
      createdAt: new Date(Date.now() - 25 * 24 * 60 * 60 * 1000).toISOString()
    },
    {
      id: '5',
      title: 'Cross-Team Collaboration Framework',
      titleArabic: 'إطار التعاون بين الفرق',
      descriptionPreview: 'Developing an effective framework for collaboration between stadium operations, security, and hospitality teams...',
      descriptionPreviewAr: 'تطوير إطار فعال للتعاون بين فرق عمليات الملعب والأمن والضيافة...',
      category: 'TeamManagement',
      impact: 'High',
      status: 'Published',
      authorName: 'Omar Ibrahim',
      authorNameAr: 'عمر إبراهيم',
      projectName: 'Operations',
      projectNameAr: 'العمليات',
      viewCount: 345,
      usefulCount: 78,
      isMarkedUseful: false,
      tags: ['Collaboration', 'Teams', 'Framework', 'Operations'],
      createdAt: new Date(Date.now() - 30 * 24 * 60 * 60 * 1000).toISOString()
    },
    {
      id: '6',
      title: 'Media Relations During Crisis',
      titleArabic: 'العلاقات الإعلامية أثناء الأزمات',
      descriptionPreview: 'Strategies for managing media communications during unexpected incidents and maintaining public confidence...',
      descriptionPreviewAr: 'استراتيجيات لإدارة الاتصالات الإعلامية أثناء الحوادث غير المتوقعة والحفاظ على ثقة الجمهور...',
      category: 'Communication',
      impact: 'High',
      status: 'Published',
      authorName: 'Layla Ahmed',
      authorNameAr: 'ليلى أحمد',
      projectName: 'Media Relations',
      projectNameAr: 'العلاقات الإعلامية',
      viewCount: 289,
      usefulCount: 67,
      isMarkedUseful: false,
      tags: ['Media', 'Crisis', 'Communication', 'PR'],
      createdAt: new Date(Date.now() - 35 * 24 * 60 * 60 * 1000).toISOString()
    }
  ]

    loading.value = false

    // Trigger entrance animations
    if (shouldAnimate.value) {
      requestAnimationFrame(() => {
        isContentVisible.value = true
      })
    } else {
      isContentVisible.value = true
    }
  } catch (e) {
    error.value = e as Error
    loading.value = false
    console.error('Failed to load lessons:', e)
  }
}

async function handleRetry() {
  await loadLessons()
}

onMounted(() => {
  loadLessons()
})
</script>

<template>
  <div class="lessons-learned-view" :class="{ 'rtl': isRTL }">
    <!-- Page Header -->
    <PageHeader
      :title="isRTL ? 'الدروس المستفادة' : 'Lessons Learned'"
      :description="isRTL ? 'اجمع وشارك الرؤى القيمة من المشاريع والتجارب' : 'Capture and share valuable insights from projects and experiences'"
      :breadcrumbs="breadcrumbs"
      padding-bottom="xl"
      background-variant="gradient"
      :animated="shouldAnimate"
    >
      <template #actions>
        <Button
          :label="isRTL ? 'إنشاء درس' : 'Create Lesson'"
          icon="pi pi-plus"
          class="header-btn-primary"
          @click="showCreateDialog = true"
        />
      </template>
    </PageHeader>

    <!-- Stats Bar -->
    <StatsBar
      :stats="statsBarItems"
      :loading="loading"
      overlap="lg"
      :animated="shouldAnimate"
      :animate-numbers="shouldAnimate"
      :counter-duration="1200"
    />

    <!-- Toolbar -->
    <div class="toolbar">
      <div class="toolbar-left">
        <div class="search-box">
          <i class="pi pi-search"></i>
          <input
            v-model="filters.search"
            type="text"
            :placeholder="isRTL ? 'البحث في الدروس...' : 'Search lessons...'"
            class="search-input"
          />
          <button v-if="filters.search" class="clear-search" @click="filters.search = ''">
            <i class="pi pi-times"></i>
          </button>
        </div>
      </div>

      <div class="toolbar-center">
        <div class="view-toggle">
          <button
            :class="['toggle-btn', { active: viewMode === 'grid' }]"
            @click="viewMode = 'grid'"
          >
            <i class="pi pi-th-large"></i>
          </button>
          <button
            :class="['toggle-btn', { active: viewMode === 'list' }]"
            @click="viewMode = 'list'"
          >
            <i class="pi pi-list"></i>
          </button>
        </div>
      </div>

      <div class="toolbar-right">
        <Dropdown
          v-model="filters.sortBy"
          :options="sortOptions"
          :optionLabel="isRTL ? 'labelAr' : 'label'"
          optionValue="value"
          class="sort-dropdown"
        />
        <button
          :class="['filter-btn', { active: showFiltersPanel }]"
          @click="showFiltersPanel = !showFiltersPanel"
        >
          <i class="pi pi-filter"></i>
          <span>{{ isRTL ? 'فلتر' : 'Filter' }}</span>
        </button>
      </div>
    </div>

    <!-- Filters Panel -->
    <Transition name="slide">
      <div v-if="showFiltersPanel" class="filters-panel">
        <div class="filter-group">
          <label>{{ isRTL ? 'الفئة' : 'Category' }}</label>
          <div class="filter-chips">
            <button
              v-for="cat in categoryOptions"
              :key="cat.value"
              :class="['filter-chip', { active: filters.category === cat.value }]"
              :style="filters.category === cat.value ? {
                backgroundColor: getCategoryConfig(cat.value).bgColor,
                color: getCategoryConfig(cat.value).color,
                borderColor: getCategoryConfig(cat.value).color
              } : {}"
              @click="filters.category = filters.category === cat.value ? null : cat.value"
            >
              <i :class="getCategoryConfig(cat.value).icon"></i>
              {{ isRTL ? cat.labelAr : cat.label }}
            </button>
          </div>
        </div>

        <div class="filter-group">
          <label>{{ isRTL ? 'التأثير' : 'Impact' }}</label>
          <div class="filter-chips">
            <button
              v-for="imp in impactOptions"
              :key="imp.value"
              :class="['filter-chip', { active: filters.impact === imp.value }]"
              :style="filters.impact === imp.value ? {
                backgroundColor: getImpactConfig(imp.value).bgColor,
                color: getImpactConfig(imp.value).color,
                borderColor: getImpactConfig(imp.value).color
              } : {}"
              @click="filters.impact = filters.impact === imp.value ? null : imp.value"
            >
              <i :class="getImpactConfig(imp.value).icon"></i>
              {{ isRTL ? imp.labelAr : imp.label }}
            </button>
          </div>
        </div>

        <div class="filter-actions">
          <button class="clear-filters-btn" @click="clearFilters">
            <i class="pi pi-times"></i>
            {{ isRTL ? 'مسح الفلاتر' : 'Clear Filters' }}
          </button>
        </div>
      </div>
    </Transition>

    <!-- Loading State -->
    <div v-if="loading" class="loading-grid">
      <div v-for="i in 6" :key="i" class="skeleton-card">
        <Skeleton height="1.5rem" width="40%" class="mb-3" />
        <Skeleton height="1rem" width="80%" class="mb-2" />
        <Skeleton height="1rem" width="60%" class="mb-4" />
        <Skeleton height="0.75rem" width="100%" class="mb-2" />
        <Skeleton height="0.75rem" width="70%" />
      </div>
    </div>

    <!-- Error State -->
    <ErrorState
      v-else-if="error"
      :error="error"
      :title="isRTL ? 'فشل تحميل الدروس' : 'Failed to load lessons'"
      show-retry
      @retry="handleRetry"
    />

    <!-- Lessons Grid/List -->
    <div v-else :class="['lessons-container', viewMode]">
      <div
        v-for="(lesson, index) in lessons"
        :key="lesson.id"
        class="lesson-card"
        :style="{ '--animation-order': index }"
        @click="goToLesson(lesson.id)"
      >
        <!-- Card Header -->
        <div class="card-header">
          <div class="category-badge" :style="{
            color: getCategoryConfig(lesson.category).color,
            backgroundColor: getCategoryConfig(lesson.category).bgColor
          }">
            <i :class="getCategoryConfig(lesson.category).icon"></i>
            <span>{{ isRTL ? getCategoryConfig(lesson.category).labelAr : getCategoryConfig(lesson.category).label }}</span>
          </div>
          <div class="header-right">
            <div class="impact-badge" :style="{
              color: getImpactConfig(lesson.impact).color,
              backgroundColor: getImpactConfig(lesson.impact).bgColor
            }">
              {{ isRTL ? getImpactConfig(lesson.impact).labelAr : getImpactConfig(lesson.impact).label }}
            </div>
            <button
              :class="['useful-btn', { active: lesson.isMarkedUseful }]"
              @click="toggleUseful(lesson, $event)"
            >
              <i :class="lesson.isMarkedUseful ? 'pi pi-thumbs-up-fill' : 'pi pi-thumbs-up'"></i>
            </button>
          </div>
        </div>

        <!-- Card Body -->
        <div class="card-body">
          <h3 class="lesson-title">{{ getTitle(lesson) }}</h3>
          <p class="lesson-description">{{ getDescription(lesson) }}</p>

          <!-- Tags -->
          <div class="lesson-tags">
            <span v-for="tag in lesson.tags.slice(0, 3)" :key="tag" class="tag-chip">
              {{ tag }}
            </span>
            <span v-if="lesson.tags.length > 3" class="more-tags">
              +{{ lesson.tags.length - 3 }}
            </span>
          </div>
        </div>

        <!-- Card Footer -->
        <div class="card-footer">
          <div class="author-section">
            <Avatar
              :image="lesson.authorAvatarUrl"
              :name="lesson.authorName"
              shape="circle"
              size="sm"
              class="author-avatar"
            />
            <div class="author-info">
              <span class="author-name">{{ getAuthorName(lesson) }}</span>
              <span class="post-time">{{ formatRelativeTime(lesson.createdAt) }}</span>
            </div>
          </div>
          <div class="stats-section">
            <span class="stat">
              <i class="pi pi-folder"></i>
              {{ getProjectName(lesson) }}
            </span>
            <span class="stat">
              <i class="pi pi-eye"></i>
              {{ formatNumber(lesson.viewCount) }}
            </span>
            <span class="stat">
              <i class="pi pi-thumbs-up"></i>
              {{ formatNumber(lesson.usefulCount) }}
            </span>
          </div>
        </div>

        <!-- Status Badge (if not published) -->
        <div
          v-if="lesson.status !== 'Published'"
          class="status-badge"
          :style="{
            color: getStatusConfig(lesson.status).color,
            backgroundColor: getStatusConfig(lesson.status).bgColor
          }"
        >
          {{ isRTL ? getStatusConfig(lesson.status).labelAr : getStatusConfig(lesson.status).label }}
        </div>
      </div>

      <!-- Empty State -->
      <div v-if="lessons.length === 0" class="empty-state">
        <div class="empty-icon">
          <i class="pi pi-book"></i>
        </div>
        <h3>{{ isRTL ? 'لا توجد دروس' : 'No lessons found' }}</h3>
        <p>{{ isRTL ? 'كن أول من يشارك رؤاه وتجاربه القيمة.' : 'Be the first to share your valuable insights and experiences.' }}</p>
        <Button
          :label="isRTL ? 'إنشاء درس' : 'Create Lesson'"
          icon="pi pi-plus"
          @click="showCreateDialog = true"
        />
      </div>
    </div>

    <!-- Create Lesson Dialog -->
    <Dialog
      v-model:visible="showCreateDialog"
      :header="isRTL ? 'إنشاء درس جديد' : 'Create New Lesson'"
      modal
      :style="{ width: '700px' }"
      class="create-dialog"
    >
      <div class="dialog-form">
        <div class="form-section">
          <label class="form-label">{{ isRTL ? 'عنوان الدرس' : 'Lesson Title' }} *</label>
          <InputText
            v-model="newLesson.title"
            :placeholder="isRTL ? 'أدخل عنوانًا واضحًا ووصفيًا' : 'Enter a clear, descriptive title'"
            class="form-input"
          />
        </div>

        <div class="form-section">
          <label class="form-label">{{ isRTL ? 'العنوان بالعربية' : 'Title (Arabic)' }}</label>
          <InputText
            v-model="newLesson.titleArabic"
            :placeholder="isRTL ? 'العنوان بالعربية' : 'Arabic title'"
            class="form-input"
            dir="rtl"
          />
        </div>

        <div class="form-section">
          <label class="form-label">{{ isRTL ? 'السياق' : 'Context' }} *</label>
          <Textarea
            v-model="newLesson.context"
            :placeholder="isRTL ? 'صف الموقف أو الخلفية...' : 'Describe the situation or background...'"
            rows="3"
            class="form-input"
          />
        </div>

        <div class="form-section">
          <label class="form-label">{{ isRTL ? 'التحدي' : 'Challenge' }} *</label>
          <Textarea
            v-model="newLesson.challenge"
            :placeholder="isRTL ? 'ما هي المشكلة أو التحدي الذي واجهته؟' : 'What was the problem or challenge faced?'"
            rows="3"
            class="form-input"
          />
        </div>

        <div class="form-section">
          <label class="form-label">{{ isRTL ? 'الحل' : 'Solution' }} *</label>
          <Textarea
            v-model="newLesson.solution"
            :placeholder="isRTL ? 'كيف تمت معالجة التحدي؟' : 'How was the challenge addressed?'"
            rows="3"
            class="form-input"
          />
        </div>

        <div class="form-section">
          <label class="form-label">{{ isRTL ? 'النتيجة' : 'Outcome' }} *</label>
          <Textarea
            v-model="newLesson.outcome"
            :placeholder="isRTL ? 'ما هي النتيجة؟' : 'What was the result?'"
            rows="3"
            class="form-input"
          />
        </div>

        <div class="form-section">
          <label class="form-label">{{ isRTL ? 'التوصيات' : 'Recommendations' }}</label>
          <Textarea
            v-model="newLesson.recommendations"
            :placeholder="isRTL ? 'ماذا توصي للمواقف المماثلة؟' : 'What would you recommend for similar situations?'"
            rows="3"
            class="form-input"
          />
        </div>

        <div class="form-row">
          <div class="form-section half">
            <label class="form-label">{{ isRTL ? 'الفئة' : 'Category' }} *</label>
            <Dropdown
              v-model="newLesson.category"
              :options="categoryOptions"
              :optionLabel="isRTL ? 'labelAr' : 'label'"
              optionValue="value"
              :placeholder="isRTL ? 'اختر الفئة' : 'Select category'"
              class="form-input"
            />
          </div>
          <div class="form-section half">
            <label class="form-label">{{ isRTL ? 'التأثير' : 'Impact' }} *</label>
            <Dropdown
              v-model="newLesson.impact"
              :options="impactOptions"
              :optionLabel="isRTL ? 'labelAr' : 'label'"
              optionValue="value"
              :placeholder="isRTL ? 'اختر التأثير' : 'Select impact'"
              class="form-input"
            />
          </div>
        </div>

        <div class="form-section">
          <label class="form-label">{{ isRTL ? 'الوسوم' : 'Tags' }}</label>
          <Chips
            v-model="newLesson.tags"
            :placeholder="isRTL ? 'أضف وسومًا...' : 'Add tags...'"
            class="form-input"
          />
        </div>
      </div>

      <template #footer>
        <div class="dialog-actions">
          <Button
            :label="isRTL ? 'إلغاء' : 'Cancel'"
            severity="secondary"
            text
            @click="showCreateDialog = false"
          />
          <Button
            :label="isRTL ? 'حفظ كمسودة' : 'Save as Draft'"
            severity="secondary"
            outlined
            @click="saveAsDraft"
          />
          <Button
            :label="isRTL ? 'إرسال للمراجعة' : 'Submit for Review'"
            icon="pi pi-send"
            @click="submitForReview"
          />
        </div>
      </template>
    </Dialog>
  </div>
</template>

<style scoped lang="scss">
// Design System
@use '@/design-system/tokens' as *;
@use '@/design-system/mixins' as *;
@use '@/assets/styles/_variables.scss' as *;
@use '@/assets/styles/_mixins.scss' as legacy;

// ============================================
// LESSONS LEARNED VIEW - MAIN CONTAINER
// Following Universal Spacing Guidelines from _mixins.scss
// ============================================

.lessons-learned-view {
  @include legacy.page-view;
  background: var(--surface-ground);

  &.rtl {
    direction: rtl;
  }
}

// Header button styles (used in PageHeader slot)
.header-btn-primary {
  @include legacy.header-btn-primary;
}

// Toolbar
.toolbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 1rem;
  padding: 1.5rem 2rem;
  margin: 0 -2rem;
  background: var(--surface-card);
  border-bottom: 1px solid var(--surface-border);

  .toolbar-left {
    flex: 1;
    max-width: 400px;
  }

  .search-box {
    @include legacy.toolbar-search;
  }

  .view-toggle {
    @include legacy.type-toggle;
  }

  .toolbar-right {
    display: flex;
    gap: 1rem;
    align-items: center;
  }

  .sort-dropdown {
    min-width: 160px;
  }

  .filter-btn {
    @include legacy.filter-btn;
  }
}

// Filters Panel
.filters-panel {
  @include legacy.filters-panel;
  padding: 1.5rem 2rem;
  margin: 0 -2rem;
  background: var(--surface-50);
  border-bottom: 1px solid var(--surface-border);

  .filter-group {
    margin-bottom: 1rem;

    label {
      display: block;
      font-weight: 600;
      font-size: 0.875rem;
      margin-bottom: 0.75rem;
      color: var(--text-color);
    }
  }

  .filter-chips {
    display: flex;
    flex-wrap: wrap;
    gap: 0.5rem;

    .filter-chip {
      display: inline-flex;
      align-items: center;
      gap: 0.375rem;
      padding: 0.5rem 1rem;
      border: 1px solid var(--surface-border);
      border-radius: 20px;
      background: var(--surface-card);
      color: var(--text-color-secondary);
      font-size: 0.8125rem;
      cursor: pointer;
      transition: all 0.2s ease;

      &:hover {
        border-color: var(--primary-color);
        color: var(--primary-color);
      }

      &.active {
        border-width: 2px;
      }
    }
  }

  .filter-actions {
    margin-top: 1rem;
    padding-top: 1rem;
    border-top: 1px solid var(--surface-border);

    .clear-filters-btn {
      display: inline-flex;
      align-items: center;
      gap: 0.5rem;
      padding: 0.5rem 1rem;
      border: none;
      border-radius: 8px;
      background: transparent;
      color: var(--text-color-secondary);
      font-size: 0.875rem;
      cursor: pointer;

      &:hover {
        background: var(--surface-100);
        color: var(--text-color);
      }
    }
  }
}

// Slide transition
.slide-enter-active,
.slide-leave-active {
  transition: all 0.3s ease;
}

.slide-enter-from,
.slide-leave-to {
  opacity: 0;
  transform: translateY(-10px);
}

// Loading Grid
.loading-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
  gap: 1.5rem;
  padding: 2rem;

  .skeleton-card {
    background: var(--surface-card);
    border-radius: 16px;
    padding: 1.5rem;
  }
}

// Lessons Container
.lessons-container {
  padding: 2rem;

  &.grid {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(380px, 1fr));
    gap: 1.5rem;
  }

  &.list {
    display: flex;
    flex-direction: column;
    gap: 1rem;

    .lesson-card {
      flex-direction: row;
      gap: 1.5rem;

      .card-header {
        flex-direction: column;
        align-items: flex-start;
        gap: 0.5rem;
      }

      .card-body {
        flex: 1;
      }

      .card-footer {
        flex-direction: column;
        align-items: flex-end;
        gap: 0.75rem;
        min-width: 200px;
      }
    }
  }
}

// Lesson Card
.lesson-card {
  @include legacy.card-item-animation;
  position: relative;
  background: var(--surface-card);
  border-radius: 16px;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.04);
  overflow: hidden;
  cursor: pointer;
  transition: all 0.3s ease;
  display: flex;
  flex-direction: column;

  &:hover {
    transform: translateY(-4px);
    box-shadow: 0 8px 32px rgba(0, 0, 0, 0.12);
  }

  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 1.25rem 1.5rem;
    border-bottom: 1px solid var(--surface-border);

    .category-badge {
      display: inline-flex;
      align-items: center;
      gap: 0.375rem;
      padding: 0.375rem 0.75rem;
      border-radius: 20px;
      font-size: 0.8125rem;
      font-weight: 500;

      i {
        font-size: 0.75rem;
      }
    }

    .header-right {
      display: flex;
      align-items: center;
      gap: 0.75rem;
    }

    .impact-badge {
      padding: 0.25rem 0.625rem;
      border-radius: 6px;
      font-size: 0.75rem;
      font-weight: 600;
    }

    .useful-btn {
      width: 36px;
      height: 36px;
      border: none;
      border-radius: 50%;
      background: var(--surface-100);
      color: var(--text-color-secondary);
      cursor: pointer;
      transition: all 0.2s ease;

      &:hover {
        background: var(--surface-200);
      }

      &.active {
        background: rgba(16, 185, 129, 0.1);
        color: #10b981;
      }
    }
  }

  .card-body {
    padding: 1.5rem;
    flex: 1;

    .lesson-title {
      margin: 0 0 0.75rem;
      font-size: 1.125rem;
      font-weight: 600;
      color: var(--text-color);
      line-height: 1.4;
      display: -webkit-box;
      -webkit-line-clamp: 2;
      -webkit-box-orient: vertical;
      overflow: hidden;
    }

    .lesson-description {
      margin: 0 0 1rem;
      font-size: 0.9375rem;
      color: var(--text-color-secondary);
      line-height: 1.6;
      display: -webkit-box;
      -webkit-line-clamp: 3;
      -webkit-box-orient: vertical;
      overflow: hidden;
    }

    .lesson-tags {
      display: flex;
      flex-wrap: wrap;
      gap: 0.5rem;

      .tag-chip {
        padding: 0.25rem 0.625rem;
        background: var(--surface-100);
        border-radius: 6px;
        font-size: 0.75rem;
        color: var(--text-color-secondary);
      }

      .more-tags {
        padding: 0.25rem 0.5rem;
        font-size: 0.75rem;
        color: var(--text-color-secondary);
      }
    }
  }

  .card-footer {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 1rem 1.5rem;
    background: var(--surface-50);
    border-top: 1px solid var(--surface-border);

    .author-section {
      display: flex;
      align-items: center;
      gap: 0.75rem;

      .author-info {
        display: flex;
        flex-direction: column;

        .author-name {
          font-size: 0.875rem;
          font-weight: 500;
          color: var(--text-color);
        }

        .post-time {
          font-size: 0.75rem;
          color: var(--text-color-secondary);
        }
      }
    }

    .stats-section {
      display: flex;
      gap: 1rem;

      .stat {
        display: flex;
        align-items: center;
        gap: 0.375rem;
        font-size: 0.75rem;
        color: var(--text-color-secondary);

        i {
          font-size: 0.75rem;
        }
      }
    }
  }

  .status-badge {
    position: absolute;
    top: 1rem;
    inset-inline-end: 1rem;
    padding: 0.25rem 0.625rem;
    border-radius: 6px;
    font-size: 0.75rem;
    font-weight: 600;
  }
}

@include legacy.staggered-animation-delays('.lesson-card', 12, 0.05s);

// Empty State
.empty-state {
  grid-column: 1 / -1;
  text-align: center;
  padding: 4rem 2rem;
  background: var(--surface-card);
  border-radius: 16px;

  .empty-icon {
    width: 80px;
    height: 80px;
    margin: 0 auto 1.5rem;
    background: var(--surface-100);
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;

    i {
      font-size: 2.5rem;
      color: var(--text-color-secondary);
      opacity: 0.5;
    }
  }

  h3 {
    margin: 0 0 0.5rem;
    font-size: 1.25rem;
    color: var(--text-color);
  }

  p {
    margin: 0 0 1.5rem;
    color: var(--text-color-secondary);
  }
}

// Create Dialog
.create-dialog {
  .dialog-form {
    display: flex;
    flex-direction: column;
    gap: 1.25rem;
    max-height: 60vh;
    overflow-y: auto;
    padding-inline-end: 0.5rem;
  }

  .form-row {
    display: flex;
    gap: 1rem;

    .half {
      flex: 1;
    }
  }

  .form-section {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;

    .form-label {
      font-weight: 500;
      font-size: 0.875rem;
      color: var(--text-color);
    }

    .form-input {
      width: 100%;
    }
  }

  .dialog-actions {
    display: flex;
    justify-content: flex-end;
    gap: 0.75rem;
  }
}

// Responsive
@media (max-width: 768px) {
  .toolbar {
    flex-direction: column;
    padding: 1rem 1.5rem;
    margin: 0 -1.5rem;
    gap: 1rem;

    .toolbar-left {
      width: 100%;
      max-width: none;
    }

    .toolbar-right {
      width: 100%;
      justify-content: space-between;
    }
  }

  .lessons-container {
    padding: 1.5rem;

    &.grid {
      grid-template-columns: 1fr;
    }
  }

  .lesson-card {
    .card-footer {
      flex-direction: column;
      gap: 0.75rem;
      align-items: flex-start;
    }
  }

  .create-dialog {
    :deep(.p-dialog) {
      width: 95vw !important;
      max-width: none;
    }

    .form-row {
      flex-direction: column;
    }
  }
}

// ============================================
// REDUCED MOTION SUPPORT
// ============================================
@media (prefers-reduced-motion: reduce) {
  .lessons-learned-view {
    .lesson-card {
      animation: none !important;
      opacity: 1;
      transform: none;
    }
  }
}
</style>
