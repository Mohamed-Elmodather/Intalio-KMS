<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useReducedMotion } from '@/composables/useReducedMotion'
import { Avatar } from '@/components/base'
import ErrorState from '@/components/base/ErrorState.vue'
import Button from 'primevue/button'
import Textarea from 'primevue/textarea'
import Dialog from 'primevue/dialog'
import InputText from 'primevue/inputtext'
import Skeleton from 'primevue/skeleton'

const route = useRoute()
const router = useRouter()
const { locale } = useI18n()
const prefersReducedMotion = useReducedMotion()

// Animation control
const shouldAnimate = computed(() => !prefersReducedMotion.value)
const isContentVisible = ref(false)

// State
const loading = ref(true)
const error = ref<Error | null>(null)
const newComment = ref('')
const submitting = ref(false)
const showShareDialog = ref(false)
const shareLink = ref('')

// RTL support
const isRTL = computed(() => locale.value === 'ar')

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

const IMPACT_CONFIG: Record<string, { color: string; bgColor: string; label: string; labelAr: string }> = {
  Critical: { color: '#dc2626', bgColor: 'rgba(220, 38, 38, 0.1)', label: 'Critical', labelAr: 'حرج' },
  High: { color: '#f59e0b', bgColor: 'rgba(245, 158, 11, 0.1)', label: 'High', labelAr: 'عالي' },
  Medium: { color: '#3b82f6', bgColor: 'rgba(59, 130, 246, 0.1)', label: 'Medium', labelAr: 'متوسط' },
  Low: { color: '#10b981', bgColor: 'rgba(16, 185, 129, 0.1)', label: 'Low', labelAr: 'منخفض' }
}

const STATUS_CONFIG: Record<string, { color: string; bgColor: string; label: string; labelAr: string }> = {
  Published: { color: '#10b981', bgColor: 'rgba(16, 185, 129, 0.1)', label: 'Published', labelAr: 'منشور' },
  Draft: { color: '#6b7280', bgColor: 'rgba(107, 114, 128, 0.1)', label: 'Draft', labelAr: 'مسودة' },
  PendingReview: { color: '#f59e0b', bgColor: 'rgba(245, 158, 11, 0.1)', label: 'Pending Review', labelAr: 'قيد المراجعة' }
}

const SECTION_CONFIG = {
  context: { icon: 'pi pi-info-circle', color: '#3b82f6', labelEn: 'Context', labelAr: 'السياق' },
  challenge: { icon: 'pi pi-exclamation-triangle', color: '#f59e0b', labelEn: 'Challenge', labelAr: 'التحدي' },
  solution: { icon: 'pi pi-lightbulb', color: '#10b981', labelEn: 'Solution', labelAr: 'الحل' },
  outcome: { icon: 'pi pi-check-circle', color: '#8b5cf6', labelEn: 'Outcome', labelAr: 'النتيجة' },
  recommendations: { icon: 'pi pi-star', color: '#d4af37', labelEn: 'Recommendations', labelAr: 'التوصيات' }
}

interface Lesson {
  id: string
  title: string
  titleArabic?: string
  context: string
  contextArabic?: string
  challenge: string
  challengeArabic?: string
  solution: string
  solutionArabic?: string
  outcome: string
  outcomeArabic?: string
  recommendations?: string
  recommendationsArabic?: string
  category: string
  impact: string
  status: string
  authorId: string
  authorName: string
  authorNameAr?: string
  authorAvatarUrl?: string
  authorJobTitle?: string
  authorJobTitleAr?: string
  projectId: string
  projectName: string
  projectNameAr?: string
  viewCount: number
  usefulCount: number
  isMarkedUseful: boolean
  tags: string[]
  createdAt: string
  updatedAt?: string
}

interface Comment {
  id: string
  content: string
  authorName: string
  authorNameAr?: string
  authorAvatarUrl?: string
  likeCount: number
  isLiked: boolean
  createdAt: string
}

interface RelatedLesson {
  id: string
  title: string
  titleAr?: string
  category: string
  usefulCount: number
}

const lesson = ref<Lesson | null>(null)
const comments = ref<Comment[]>([])
const relatedLessons = ref<RelatedLesson[]>([])

// Helpers
const getCategoryConfig = (category: string) => CATEGORY_CONFIG[category] || CATEGORY_CONFIG.Other
const getImpactConfig = (impact: string) => IMPACT_CONFIG[impact] || IMPACT_CONFIG.Medium
const getStatusConfig = (status: string) => STATUS_CONFIG[status] || STATUS_CONFIG.Draft

const getTitle = () => isRTL.value && lesson.value?.titleArabic ? lesson.value.titleArabic : lesson.value?.title
const getText = (en?: string, ar?: string) => isRTL.value && ar ? ar : en
const getAuthorName = (item: { authorName: string; authorNameAr?: string }) => isRTL.value && item.authorNameAr ? item.authorNameAr : item.authorName

const formatDate = (dateString: string) => {
  return new Intl.DateTimeFormat(isRTL.value ? 'ar-SA' : 'en-US', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  }).format(new Date(dateString))
}

const formatRelativeTime = (date: string) => {
  const now = new Date()
  const then = new Date(date)
  const diffMs = now.getTime() - then.getTime()
  const diffDays = Math.floor(diffMs / (1000 * 60 * 60 * 24))

  if (diffDays < 1) return isRTL.value ? 'اليوم' : 'Today'
  if (diffDays === 1) return isRTL.value ? 'أمس' : 'Yesterday'
  if (diffDays < 7) return isRTL.value ? `منذ ${diffDays} أيام` : `${diffDays}d ago`
  return formatDate(date)
}

const formatNumber = (num: number) => {
  if (num >= 1000) return (num / 1000).toFixed(1) + 'K'
  return num.toString()
}

// Actions
const toggleUseful = () => {
  if (!lesson.value) return
  lesson.value.isMarkedUseful = !lesson.value.isMarkedUseful
  lesson.value.usefulCount += lesson.value.isMarkedUseful ? 1 : -1
}

const postComment = async () => {
  if (!newComment.value.trim()) return

  submitting.value = true
  await new Promise(resolve => setTimeout(resolve, 500))

  comments.value.unshift({
    id: String(Date.now()),
    content: newComment.value,
    authorName: 'Current User',
    authorNameAr: 'المستخدم الحالي',
    likeCount: 0,
    isLiked: false,
    createdAt: new Date().toISOString()
  })

  newComment.value = ''
  submitting.value = false
}

const likeComment = (comment: Comment) => {
  comment.isLiked = !comment.isLiked
  comment.likeCount += comment.isLiked ? 1 : -1
}

const goToLesson = (id: string) => {
  router.push({ name: 'lesson-detail', params: { id } })
}

const goBack = () => {
  router.push({ name: 'lessons-learned' })
}

const openShareDialog = () => {
  shareLink.value = window.location.href
  showShareDialog.value = true
}

const copyShareLink = async () => {
  try {
    await navigator.clipboard.writeText(shareLink.value)
  } catch {
    // Fallback
  }
}

onMounted(async () => {
  try {
    error.value = null
    await new Promise(resolve => setTimeout(resolve, 600))

  lesson.value = {
    id: route.params.id as string,
    title: 'Effective Crowd Management at Large Events',
    titleArabic: 'إدارة الحشود الفعالة في الفعاليات الكبيرة',
    context: 'During the opening ceremony rehearsal, we had approximately 15,000 attendees. This was our first large-scale crowd management test for the AFC Asian Cup 2027. The event was held at King Abdullah Sports City, and we needed to ensure smooth entry, movement within the venue, and safe exit for all attendees.',
    contextArabic: 'خلال بروفة حفل الافتتاح، كان لدينا حوالي 15,000 حاضر. كان هذا أول اختبار لإدارة الحشود على نطاق واسع لكأس آسيا 2027. أقيم الحدث في مدينة الملك عبدالله الرياضية.',
    challenge: 'Managing crowd flow at entry points became congested, causing delays of up to 45 minutes for some attendees. Communication between security teams was inconsistent, leading to confusion about crowd density in different areas. Signage was insufficient in some areas, causing attendees to gather in bottleneck points.',
    challengeArabic: 'أصبح تدفق الحشود عند نقاط الدخول مكتظاً، مما تسبب في تأخيرات تصل إلى 45 دقيقة. كان التواصل بين فرق الأمن غير متسق، مما أدى إلى ارتباك حول كثافة الحشود.',
    solution: 'We implemented a zone-based entry system with dedicated lanes for different ticket types. Added real-time radio communication protocols with standardized codes. Deployed crowd density monitoring cameras and dashboards. Installed additional directional signage at all decision points. Created mobile response teams.',
    solutionArabic: 'قمنا بتنفيذ نظام دخول قائم على المناطق مع ممرات مخصصة لأنواع التذاكر المختلفة. أضفنا بروتوكولات اتصال راديو في الوقت الفعلي. نشرنا كاميرات مراقبة كثافة الحشود.',
    outcome: 'Entry times reduced by 60% in subsequent events. Attendee satisfaction scores improved from 3.2 to 4.5 out of 5. Zero safety incidents reported during follow-up events. Communication response times improved from 3 minutes to under 30 seconds.',
    outcomeArabic: 'انخفضت أوقات الدخول بنسبة 60% في الفعاليات اللاحقة. تحسنت درجات رضا الحضور من 3.2 إلى 4.5 من 5. لم تُبلغ عن أي حوادث تتعلق بالسلامة.',
    recommendations: '1. Always conduct crowd simulations before large events using historical data\n2. Establish clear communication protocols with all teams\n3. Deploy crowd monitoring technology at all major venues\n4. Train staff on crowd management best practices\n5. Create contingency plans for different crowd density scenarios\n6. Ensure sufficient multilingual signage',
    recommendationsArabic: '1. إجراء محاكاة للحشود دائماً قبل الفعاليات الكبيرة\n2. وضع بروتوكولات اتصال واضحة مع جميع الفرق\n3. نشر تقنية مراقبة الحشود في جميع الأماكن الرئيسية\n4. تدريب الموظفين على أفضل ممارسات إدارة الحشود',
    category: 'Process',
    impact: 'High',
    status: 'Published',
    authorId: '1',
    authorName: 'Mohammed Al-Rashid',
    authorNameAr: 'محمد الراشد',
    authorJobTitle: 'Operations Director',
    authorJobTitleAr: 'مدير العمليات',
    projectId: '1',
    projectName: 'Opening Ceremony',
    projectNameAr: 'حفل الافتتاح',
    viewCount: 456,
    usefulCount: 89,
    isMarkedUseful: true,
    tags: ['Crowd Management', 'Safety', 'Events', 'Operations', 'Venue Management'],
    createdAt: new Date(Date.now() - 10 * 24 * 60 * 60 * 1000).toISOString(),
    updatedAt: new Date(Date.now() - 2 * 24 * 60 * 60 * 1000).toISOString()
  }

  comments.value = [
    {
      id: '1',
      content: 'This is extremely valuable information. We applied similar strategies at our venue and saw immediate improvements. Thank you for documenting this so thoroughly!',
      authorName: 'Sara Ali',
      authorNameAr: 'سارة علي',
      likeCount: 12,
      isLiked: true,
      createdAt: new Date(Date.now() - 5 * 24 * 60 * 60 * 1000).toISOString()
    },
    {
      id: '2',
      content: 'The zone-based entry system worked great for us as well. One additional tip: having staff at queue ends helped manage expectations.',
      authorName: 'Ahmed Hassan',
      authorNameAr: 'أحمد حسن',
      likeCount: 8,
      isLiked: false,
      createdAt: new Date(Date.now() - 3 * 24 * 60 * 60 * 1000).toISOString()
    },
    {
      id: '3',
      content: 'Would you be able to share the specific crowd monitoring technology you used? We are evaluating options.',
      authorName: 'Fatima Khan',
      authorNameAr: 'فاطمة خان',
      likeCount: 5,
      isLiked: false,
      createdAt: new Date(Date.now() - 1 * 24 * 60 * 60 * 1000).toISOString()
    }
  ]

  relatedLessons.value = [
    { id: '2', title: 'Emergency Evacuation Procedures', titleAr: 'إجراءات الإخلاء في حالات الطوارئ', category: 'RiskManagement', usefulCount: 78 },
    { id: '3', title: 'Security Checkpoint Optimization', titleAr: 'تحسين نقاط التفتيش الأمني', category: 'Process', usefulCount: 65 },
    { id: '4', title: 'VIP Access Management', titleAr: 'إدارة وصول كبار الشخصيات', category: 'Process', usefulCount: 52 }
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
  }
})

function handleRetry() {
  // Re-run onMounted logic by reloading the page
  window.location.reload()
}
</script>

<template>
  <div class="lesson-detail-view" :class="{ 'rtl': isRTL }">
    <!-- Error State -->
    <ErrorState
      v-if="error"
      :error="error"
      :title="isRTL ? 'فشل تحميل تفاصيل الدرس' : 'Failed to load lesson details'"
      show-retry
      size="lg"
      @retry="handleRetry"
    />

    <template v-else>
    <!-- Page Header -->
    <header class="page-header">
      <div class="header-background">
        <div class="bg-gradient"></div>
        <div class="bg-pattern"></div>
      </div>
      <div class="header-content">
        <div class="header-left">
          <nav class="breadcrumb">
            <button class="breadcrumb-link" @click="$router.push({ name: 'dashboard' })">
              <i class="pi pi-home"></i>
              <span>{{ isRTL ? 'الرئيسية' : 'Home' }}</span>
            </button>
            <i class="pi pi-chevron-right breadcrumb-separator"></i>
            <button class="breadcrumb-link" @click="goBack">
              <span>{{ isRTL ? 'الدروس المستفادة' : 'Lessons Learned' }}</span>
            </button>
            <i class="pi pi-chevron-right breadcrumb-separator"></i>
            <span class="breadcrumb-current">{{ isRTL ? 'تفاصيل الدرس' : 'Lesson Detail' }}</span>
          </nav>
        </div>
        <div class="header-actions">
          <Button
            :label="isRTL ? 'العودة' : 'Back'"
            :icon="isRTL ? 'pi pi-arrow-right' : 'pi pi-arrow-left'"
            class="header-btn-secondary"
            @click="goBack"
          />
        </div>
      </div>
    </header>

    <!-- Loading State -->
    <div v-if="loading" class="loading-state">
      <div class="loading-main">
        <Skeleton height="2rem" width="60%" class="mb-3" />
        <div class="loading-badges">
          <Skeleton height="1.5rem" width="80px" />
          <Skeleton height="1.5rem" width="60px" />
          <Skeleton height="1.5rem" width="70px" />
        </div>
        <div class="loading-author">
          <Skeleton shape="circle" size="3rem" />
          <div>
            <Skeleton height="1rem" width="150px" class="mb-2" />
            <Skeleton height="0.75rem" width="100px" />
          </div>
        </div>
        <div class="loading-sections">
          <Skeleton height="120px" v-for="i in 4" :key="i" class="mb-4" />
        </div>
      </div>
      <div class="loading-sidebar">
        <Skeleton height="200px" class="mb-4" />
        <Skeleton height="180px" />
      </div>
    </div>

    <!-- Content -->
    <div v-else-if="lesson" class="lesson-content">
      <!-- Main Column -->
      <main class="lesson-main">
        <!-- Lesson Header -->
        <article class="lesson-article">
          <div class="article-header">
            <div class="badges-row">
              <span
                class="category-badge"
                :style="{
                  color: getCategoryConfig(lesson.category).color,
                  backgroundColor: getCategoryConfig(lesson.category).bgColor
                }"
              >
                <i :class="getCategoryConfig(lesson.category).icon"></i>
                {{ isRTL ? getCategoryConfig(lesson.category).labelAr : getCategoryConfig(lesson.category).label }}
              </span>
              <span
                class="impact-badge"
                :style="{
                  color: getImpactConfig(lesson.impact).color,
                  backgroundColor: getImpactConfig(lesson.impact).bgColor
                }"
              >
                {{ isRTL ? getImpactConfig(lesson.impact).labelAr : getImpactConfig(lesson.impact).label }} {{ isRTL ? 'التأثير' : 'Impact' }}
              </span>
              <span
                class="status-badge"
                :style="{
                  color: getStatusConfig(lesson.status).color,
                  backgroundColor: getStatusConfig(lesson.status).bgColor
                }"
              >
                {{ isRTL ? getStatusConfig(lesson.status).labelAr : getStatusConfig(lesson.status).label }}
              </span>
            </div>

            <h1 class="article-title">{{ getTitle() }}</h1>

            <div class="author-section">
              <Avatar
                :image="lesson.authorAvatarUrl"
                :name="lesson.authorName"
                shape="circle"
                size="lg"
                class="author-avatar"
              />
              <div class="author-info">
                <span class="author-name">{{ getAuthorName(lesson) }}</span>
                <span class="author-title">{{ getText(lesson.authorJobTitle, lesson.authorJobTitleAr) }}</span>
                <div class="meta-row">
                  <span class="meta-item">
                    <i class="pi pi-folder"></i>
                    {{ getText(lesson.projectName, lesson.projectNameAr) }}
                  </span>
                  <span class="meta-item">
                    <i class="pi pi-calendar"></i>
                    {{ formatDate(lesson.createdAt) }}
                  </span>
                </div>
              </div>
            </div>
          </div>

          <!-- Lesson Sections -->
          <div class="article-sections">
            <!-- Context -->
            <section class="lesson-section">
              <div class="section-header" :style="{ '--section-color': SECTION_CONFIG.context.color }">
                <div class="section-icon">
                  <i :class="SECTION_CONFIG.context.icon"></i>
                </div>
                <h2>{{ isRTL ? SECTION_CONFIG.context.labelAr : SECTION_CONFIG.context.labelEn }}</h2>
              </div>
              <div class="section-content">
                <p>{{ getText(lesson.context, lesson.contextArabic) }}</p>
              </div>
            </section>

            <!-- Challenge -->
            <section class="lesson-section">
              <div class="section-header" :style="{ '--section-color': SECTION_CONFIG.challenge.color }">
                <div class="section-icon">
                  <i :class="SECTION_CONFIG.challenge.icon"></i>
                </div>
                <h2>{{ isRTL ? SECTION_CONFIG.challenge.labelAr : SECTION_CONFIG.challenge.labelEn }}</h2>
              </div>
              <div class="section-content">
                <p>{{ getText(lesson.challenge, lesson.challengeArabic) }}</p>
              </div>
            </section>

            <!-- Solution -->
            <section class="lesson-section">
              <div class="section-header" :style="{ '--section-color': SECTION_CONFIG.solution.color }">
                <div class="section-icon">
                  <i :class="SECTION_CONFIG.solution.icon"></i>
                </div>
                <h2>{{ isRTL ? SECTION_CONFIG.solution.labelAr : SECTION_CONFIG.solution.labelEn }}</h2>
              </div>
              <div class="section-content">
                <p>{{ getText(lesson.solution, lesson.solutionArabic) }}</p>
              </div>
            </section>

            <!-- Outcome -->
            <section class="lesson-section">
              <div class="section-header" :style="{ '--section-color': SECTION_CONFIG.outcome.color }">
                <div class="section-icon">
                  <i :class="SECTION_CONFIG.outcome.icon"></i>
                </div>
                <h2>{{ isRTL ? SECTION_CONFIG.outcome.labelAr : SECTION_CONFIG.outcome.labelEn }}</h2>
              </div>
              <div class="section-content">
                <p>{{ getText(lesson.outcome, lesson.outcomeArabic) }}</p>
              </div>
            </section>

            <!-- Recommendations -->
            <section v-if="lesson.recommendations" class="lesson-section recommendations">
              <div class="section-header" :style="{ '--section-color': SECTION_CONFIG.recommendations.color }">
                <div class="section-icon">
                  <i :class="SECTION_CONFIG.recommendations.icon"></i>
                </div>
                <h2>{{ isRTL ? SECTION_CONFIG.recommendations.labelAr : SECTION_CONFIG.recommendations.labelEn }}</h2>
              </div>
              <div class="section-content">
                <p class="recommendations-text">{{ getText(lesson.recommendations, lesson.recommendationsArabic) }}</p>
              </div>
            </section>
          </div>

          <!-- Tags -->
          <div class="tags-section">
            <h3>{{ isRTL ? 'الوسوم' : 'Tags' }}</h3>
            <div class="tags-list">
              <span v-for="tag in lesson.tags" :key="tag" class="tag-chip">
                <i class="pi pi-tag"></i>
                {{ tag }}
              </span>
            </div>
          </div>

          <!-- Actions Bar -->
          <div class="actions-bar">
            <div class="actions-left">
              <button
                :class="['action-btn primary', { active: lesson.isMarkedUseful }]"
                @click="toggleUseful"
              >
                <i :class="lesson.isMarkedUseful ? 'pi pi-thumbs-up-fill' : 'pi pi-thumbs-up'"></i>
                <span>{{ lesson.isMarkedUseful ? (isRTL ? 'مفيد' : 'Useful') : (isRTL ? 'تحديد كمفيد' : 'Mark Useful') }}</span>
                <span class="count">{{ formatNumber(lesson.usefulCount) }}</span>
              </button>
              <button class="action-btn" @click="openShareDialog">
                <i class="pi pi-share-alt"></i>
                <span>{{ isRTL ? 'مشاركة' : 'Share' }}</span>
              </button>
              <button class="action-btn">
                <i class="pi pi-print"></i>
                <span>{{ isRTL ? 'طباعة' : 'Print' }}</span>
              </button>
            </div>
            <div class="stats-display">
              <span class="stat">
                <i class="pi pi-eye"></i>
                {{ formatNumber(lesson.viewCount) }} {{ isRTL ? 'مشاهدة' : 'views' }}
              </span>
            </div>
          </div>
        </article>

        <!-- Comments Section -->
        <section class="comments-section">
          <div class="section-title">
            <h3>
              <i class="pi pi-comments"></i>
              {{ isRTL ? 'التعليقات' : 'Comments' }}
              <span class="count-badge">{{ comments.length }}</span>
            </h3>
          </div>

          <!-- Comment Input -->
          <div class="comment-input-box">
            <Avatar name="User" shape="circle" size="sm" class="input-avatar" />
            <div class="input-wrapper">
              <Textarea
                v-model="newComment"
                :placeholder="isRTL ? 'شارك أفكارك...' : 'Share your thoughts...'"
                rows="3"
                autoResize
                class="comment-textarea"
              />
              <div class="input-actions">
                <Button
                  :label="isRTL ? 'نشر التعليق' : 'Post Comment'"
                  icon="pi pi-send"
                  :loading="submitting"
                  :disabled="!newComment.trim()"
                  @click="postComment"
                  class="submit-btn"
                />
              </div>
            </div>
          </div>

          <!-- Comments List -->
          <div v-if="comments.length > 0" class="comments-list">
            <div
              v-for="(comment, index) in comments"
              :key="comment.id"
              class="comment-card"
              :style="{ '--animation-order': index }"
            >
              <Avatar
                :image="comment.authorAvatarUrl"
                :name="comment.authorName"
                shape="circle"
                size="sm"
                class="comment-avatar"
              />
              <div class="comment-body">
                <div class="comment-header">
                  <span class="author-name">{{ getAuthorName(comment) }}</span>
                  <span class="comment-time">{{ formatRelativeTime(comment.createdAt) }}</span>
                </div>
                <p class="comment-text">{{ comment.content }}</p>
                <div class="comment-actions">
                  <button
                    :class="['comment-action', { active: comment.isLiked }]"
                    @click="likeComment(comment)"
                  >
                    <i :class="comment.isLiked ? 'pi pi-heart-fill' : 'pi pi-heart'"></i>
                    <span>{{ comment.likeCount }}</span>
                  </button>
                  <button class="comment-action">
                    <i class="pi pi-reply"></i>
                    <span>{{ isRTL ? 'رد' : 'Reply' }}</span>
                  </button>
                </div>
              </div>
            </div>
          </div>

          <div v-else class="empty-comments">
            <i class="pi pi-comments"></i>
            <p>{{ isRTL ? 'لا توجد تعليقات بعد. كن أول من يعلق!' : 'No comments yet. Be the first to comment!' }}</p>
          </div>
        </section>
      </main>

      <!-- Sidebar -->
      <aside class="lesson-sidebar">
        <!-- Quick Stats -->
        <div class="sidebar-card">
          <h4>{{ isRTL ? 'إحصائيات سريعة' : 'Quick Stats' }}</h4>
          <div class="stats-grid">
            <div class="stat-item">
              <div class="stat-icon"><i class="pi pi-eye"></i></div>
              <div class="stat-info">
                <span class="stat-value">{{ formatNumber(lesson.viewCount) }}</span>
                <span class="stat-label">{{ isRTL ? 'مشاهدة' : 'Views' }}</span>
              </div>
            </div>
            <div class="stat-item">
              <div class="stat-icon"><i class="pi pi-thumbs-up"></i></div>
              <div class="stat-info">
                <span class="stat-value">{{ formatNumber(lesson.usefulCount) }}</span>
                <span class="stat-label">{{ isRTL ? 'مفيد' : 'Useful' }}</span>
              </div>
            </div>
            <div class="stat-item">
              <div class="stat-icon"><i class="pi pi-comments"></i></div>
              <div class="stat-info">
                <span class="stat-value">{{ comments.length }}</span>
                <span class="stat-label">{{ isRTL ? 'تعليقات' : 'Comments' }}</span>
              </div>
            </div>
            <div class="stat-item">
              <div class="stat-icon"><i class="pi pi-calendar"></i></div>
              <div class="stat-info">
                <span class="stat-value">{{ formatRelativeTime(lesson.createdAt) }}</span>
                <span class="stat-label">{{ isRTL ? 'تم الإنشاء' : 'Created' }}</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Related Lessons -->
        <div class="sidebar-card">
          <h4>{{ isRTL ? 'دروس ذات صلة' : 'Related Lessons' }}</h4>
          <div class="related-list">
            <div
              v-for="related in relatedLessons"
              :key="related.id"
              class="related-item"
              @click="goToLesson(related.id)"
            >
              <div class="related-badge" :style="{
                color: getCategoryConfig(related.category).color,
                backgroundColor: getCategoryConfig(related.category).bgColor
              }">
                <i :class="getCategoryConfig(related.category).icon"></i>
              </div>
              <div class="related-info">
                <span class="related-title">{{ getText(related.title, related.titleAr) }}</span>
                <span class="related-meta">
                  <i class="pi pi-thumbs-up"></i>
                  {{ related.usefulCount }}
                </span>
              </div>
              <i :class="isRTL ? 'pi pi-chevron-left' : 'pi pi-chevron-right'" class="related-arrow"></i>
            </div>
          </div>
        </div>

        <!-- Project Info -->
        <div class="sidebar-card">
          <h4>{{ isRTL ? 'المشروع' : 'Project' }}</h4>
          <div class="project-info">
            <div class="project-icon">
              <i class="pi pi-folder"></i>
            </div>
            <span class="project-name">{{ getText(lesson.projectName, lesson.projectNameAr) }}</span>
          </div>
        </div>
      </aside>
    </div>

    <!-- Share Dialog -->
    <Dialog
      v-model:visible="showShareDialog"
      :header="isRTL ? 'مشاركة الدرس' : 'Share Lesson'"
      :style="{ width: '450px' }"
      modal
      class="share-dialog"
    >
      <div class="share-content">
        <div class="share-link-box">
          <InputText v-model="shareLink" readonly class="share-input" />
          <Button icon="pi pi-copy" @click="copyShareLink" class="copy-btn" />
        </div>
        <div class="share-socials">
          <button class="social-btn twitter"><i class="pi pi-twitter"></i></button>
          <button class="social-btn linkedin"><i class="pi pi-linkedin"></i></button>
          <button class="social-btn email"><i class="pi pi-envelope"></i></button>
        </div>
      </div>
    </Dialog>
    </template>
  </div>
</template>

<style scoped lang="scss">
// Design System
@use '@/design-system/tokens' as *;
@use '@/design-system/mixins' as *;
@use '@/assets/styles/_variables.scss' as *;
@use '@/assets/styles/_mixins.scss' as legacy;

// ============================================
// LESSON DETAIL VIEW - MAIN CONTAINER
// Following Universal Spacing Guidelines from _mixins.scss
// ============================================

.lesson-detail-view {
  @include legacy.page-view;
  background: var(--surface-ground);

  &.rtl {
    direction: rtl;

    .breadcrumb-separator {
      transform: rotate(180deg);
    }
  }
}

// Header button styles (used in PageHeader slot)
.header-btn-secondary {
  @include legacy.header-btn-secondary;
}

// Loading State
.loading-state {
  display: grid;
  grid-template-columns: 1fr 320px;
  gap: 2rem;
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 2rem;

  .loading-main {
    background: var(--surface-card);
    border-radius: 16px;
    padding: 2rem;
  }

  .loading-badges {
    display: flex;
    gap: 0.5rem;
    margin-bottom: 1.5rem;
  }

  .loading-author {
    display: flex;
    gap: 1rem;
    align-items: center;
    margin-bottom: 2rem;
  }

  .loading-sidebar {
    display: flex;
    flex-direction: column;
    gap: 1rem;
  }
}

// Content Layout
.lesson-content {
  display: grid;
  grid-template-columns: 1fr 320px;
  gap: 2rem;
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 2rem 3rem;
}

// Lesson Article
.lesson-article {
  background: var(--surface-card);
  border-radius: 16px;
  box-shadow: 0 4px 24px rgba(0, 0, 0, 0.06);
  overflow: hidden;
  animation: slideUp 0.5s ease-out;

  .article-header {
    padding: 2rem;
    border-bottom: 1px solid var(--surface-border);

    .badges-row {
      display: flex;
      flex-wrap: wrap;
      gap: 0.5rem;
      margin-bottom: 1rem;

      .category-badge,
      .impact-badge,
      .status-badge {
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
    }

    .article-title {
      font-size: 1.75rem;
      font-weight: 700;
      color: var(--text-color);
      margin: 0 0 1.5rem;
      line-height: 1.3;
    }

    .author-section {
      display: flex;
      gap: 1rem;

      .author-avatar {
        flex-shrink: 0;
      }

      .author-info {
        display: flex;
        flex-direction: column;
        gap: 0.25rem;

        .author-name {
          font-weight: 600;
          color: var(--text-color);
        }

        .author-title {
          font-size: 0.875rem;
          color: var(--text-color-secondary);
        }

        .meta-row {
          display: flex;
          flex-wrap: wrap;
          gap: 1rem;
          margin-top: 0.25rem;

          .meta-item {
            display: flex;
            align-items: center;
            gap: 0.375rem;
            font-size: 0.8125rem;
            color: var(--text-color-secondary);

            i {
              font-size: 0.75rem;
            }
          }
        }
      }
    }
  }

  .article-sections {
    padding: 2rem;
  }
}

// Lesson Sections
.lesson-section {
  margin-bottom: 1.5rem;
  padding: 1.5rem;
  background: var(--surface-50);
  border-radius: 12px;
  border-inline-start: 4px solid var(--section-color);

  &:last-child {
    margin-bottom: 0;
  }

  &.recommendations {
    background: linear-gradient(135deg, var(--surface-50) 0%, rgba(212, 175, 55, 0.05) 100%);
  }

  .section-header {
    display: flex;
    align-items: center;
    gap: 0.75rem;
    margin-bottom: 1rem;

    .section-icon {
      width: 36px;
      height: 36px;
      border-radius: 10px;
      background: var(--surface-card);
      display: flex;
      align-items: center;
      justify-content: center;

      i {
        font-size: 1rem;
        color: var(--section-color);
      }
    }

    h2 {
      margin: 0;
      font-size: 1.125rem;
      font-weight: 600;
      color: var(--text-color);
    }
  }

  .section-content {
    p {
      margin: 0;
      line-height: 1.8;
      color: var(--text-color);
      white-space: pre-line;
    }

    .recommendations-text {
      font-weight: 500;
    }
  }
}

// Tags Section
.tags-section {
  padding: 0 2rem 1.5rem;

  h3 {
    margin: 0 0 1rem;
    font-size: 1rem;
    font-weight: 600;
    color: var(--text-color);
  }

  .tags-list {
    display: flex;
    flex-wrap: wrap;
    gap: 0.5rem;

    .tag-chip {
      display: inline-flex;
      align-items: center;
      gap: 0.375rem;
      padding: 0.5rem 1rem;
      background: var(--surface-100);
      border-radius: 20px;
      font-size: 0.8125rem;
      color: var(--text-color-secondary);

      i {
        font-size: 0.7rem;
      }
    }
  }
}

// Actions Bar
.actions-bar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1.5rem 2rem;
  background: var(--surface-50);
  border-top: 1px solid var(--surface-border);

  .actions-left {
    display: flex;
    gap: 0.75rem;
  }

  .action-btn {
    display: flex;
    align-items: center;
    gap: 0.5rem;
    padding: 0.625rem 1rem;
    border: 1px solid var(--surface-border);
    border-radius: 8px;
    background: var(--surface-card);
    color: var(--text-color-secondary);
    cursor: pointer;
    transition: all 0.2s ease;
    font-size: 0.875rem;

    .count {
      padding: 0.125rem 0.5rem;
      background: var(--surface-100);
      border-radius: 10px;
      font-size: 0.75rem;
    }

    &:hover {
      border-color: var(--primary-color);
      color: var(--primary-color);
    }

    &.primary.active {
      background: rgba(16, 185, 129, 0.1);
      border-color: #10b981;
      color: #10b981;

      .count {
        background: rgba(16, 185, 129, 0.2);
      }
    }
  }

  .stats-display {
    .stat {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      font-size: 0.875rem;
      color: var(--text-color-secondary);
    }
  }
}

// Comments Section
.comments-section {
  margin-top: 2rem;
  background: var(--surface-card);
  border-radius: 16px;
  box-shadow: 0 4px 24px rgba(0, 0, 0, 0.06);
  padding: 2rem;

  .section-title h3 {
    display: flex;
    align-items: center;
    gap: 0.75rem;
    margin: 0 0 1.5rem;
    font-size: 1.25rem;
    font-weight: 600;

    i {
      color: var(--primary-color);
    }

    .count-badge {
      background: var(--primary-color);
      color: white;
      padding: 0.25rem 0.625rem;
      border-radius: 20px;
      font-size: 0.75rem;
    }
  }

  .comment-input-box {
    display: flex;
    gap: 1rem;
    margin-bottom: 2rem;
    padding-bottom: 2rem;
    border-bottom: 1px solid var(--surface-border);

    .input-wrapper {
      flex: 1;

      .comment-textarea {
        width: 100%;
        border-radius: 12px;
      }

      .input-actions {
        display: flex;
        justify-content: flex-end;
        margin-top: 0.75rem;

        .submit-btn {
          border-radius: 8px;
        }
      }
    }
  }

  .comments-list {
    display: flex;
    flex-direction: column;
    gap: 1rem;
  }

  .comment-card {
    display: flex;
    gap: 1rem;
    padding: 1.25rem;
    background: var(--surface-50);
    border-radius: 12px;
    animation: slideUp 0.4s ease-out;
    animation-delay: calc(var(--animation-order) * 0.1s);
    animation-fill-mode: backwards;

    .comment-avatar {
      flex-shrink: 0;
    }

    .comment-body {
      flex: 1;

      .comment-header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        margin-bottom: 0.5rem;

        .author-name {
          font-weight: 600;
          color: var(--text-color);
        }

        .comment-time {
          font-size: 0.8125rem;
          color: var(--text-color-secondary);
        }
      }

      .comment-text {
        margin: 0 0 0.75rem;
        line-height: 1.6;
        color: var(--text-color);
      }

      .comment-actions {
        display: flex;
        gap: 0.75rem;

        .comment-action {
          display: flex;
          align-items: center;
          gap: 0.375rem;
          padding: 0.375rem 0.75rem;
          border: none;
          border-radius: 6px;
          background: var(--surface-card);
          color: var(--text-color-secondary);
          font-size: 0.8125rem;
          cursor: pointer;
          transition: all 0.2s ease;

          &:hover {
            background: var(--surface-100);
          }

          &.active {
            color: #dc2626;
          }
        }
      }
    }
  }

  .empty-comments {
    text-align: center;
    padding: 2rem;
    color: var(--text-color-secondary);

    i {
      font-size: 2rem;
      margin-bottom: 0.75rem;
      opacity: 0.5;
    }

    p {
      margin: 0;
    }
  }
}

// Sidebar
.lesson-sidebar {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.sidebar-card {
  background: var(--surface-card);
  border-radius: 16px;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.04);
  padding: 1.5rem;

  h4 {
    margin: 0 0 1rem;
    font-size: 0.8125rem;
    font-weight: 600;
    text-transform: uppercase;
    letter-spacing: 0.05em;
    color: var(--text-color-secondary);
  }

  .stats-grid {
    display: grid;
    grid-template-columns: repeat(2, 1fr);
    gap: 1rem;

    .stat-item {
      display: flex;
      align-items: center;
      gap: 0.75rem;

      .stat-icon {
        width: 40px;
        height: 40px;
        border-radius: 10px;
        background: var(--surface-100);
        display: flex;
        align-items: center;
        justify-content: center;

        i {
          color: var(--primary-color);
        }
      }

      .stat-info {
        display: flex;
        flex-direction: column;

        .stat-value {
          font-size: 1.125rem;
          font-weight: 700;
          color: var(--text-color);
        }

        .stat-label {
          font-size: 0.75rem;
          color: var(--text-color-secondary);
        }
      }
    }
  }

  .related-list {
    display: flex;
    flex-direction: column;
    gap: 0.75rem;

    .related-item {
      display: flex;
      align-items: center;
      gap: 0.75rem;
      padding: 0.75rem;
      background: var(--surface-50);
      border-radius: 10px;
      cursor: pointer;
      transition: all 0.2s ease;

      &:hover {
        background: var(--surface-100);
        transform: translateX(4px);

        .rtl & {
          transform: translateX(-4px);
        }
      }

      .related-badge {
        width: 32px;
        height: 32px;
        border-radius: 8px;
        display: flex;
        align-items: center;
        justify-content: center;
        flex-shrink: 0;

        i {
          font-size: 0.875rem;
        }
      }

      .related-info {
        flex: 1;
        min-width: 0;

        .related-title {
          display: block;
          font-size: 0.875rem;
          font-weight: 500;
          color: var(--text-color);
          white-space: nowrap;
          overflow: hidden;
          text-overflow: ellipsis;
        }

        .related-meta {
          display: flex;
          align-items: center;
          gap: 0.25rem;
          font-size: 0.75rem;
          color: var(--text-color-secondary);
        }
      }

      .related-arrow {
        font-size: 0.75rem;
        color: var(--text-color-secondary);
      }
    }
  }

  .project-info {
    display: flex;
    align-items: center;
    gap: 1rem;
    padding: 0.75rem;
    background: var(--surface-50);
    border-radius: 10px;

    .project-icon {
      width: 40px;
      height: 40px;
      border-radius: 10px;
      background: rgba(212, 175, 55, 0.1);
      display: flex;
      align-items: center;
      justify-content: center;

      i {
        color: #d4af37;
      }
    }

    .project-name {
      font-weight: 500;
      color: var(--text-color);
    }
  }
}

// Share Dialog
.share-dialog {
  .share-content {
    .share-link-box {
      display: flex;
      gap: 0.5rem;
      margin-bottom: 1.5rem;

      .share-input {
        flex: 1;
        border-radius: 8px;
      }

      .copy-btn {
        border-radius: 8px;
      }
    }

    .share-socials {
      display: flex;
      justify-content: center;
      gap: 1rem;

      .social-btn {
        width: 48px;
        height: 48px;
        border: none;
        border-radius: 50%;
        cursor: pointer;
        transition: all 0.2s ease;
        display: flex;
        align-items: center;
        justify-content: center;

        i {
          font-size: 1.25rem;
          color: white;
        }

        &.twitter {
          background: #1da1f2;
          &:hover { background: #0d8ddb; }
        }

        &.linkedin {
          background: #0077b5;
          &:hover { background: #006097; }
        }

        &.email {
          background: #6b7280;
          &:hover { background: #4b5563; }
        }
      }
    }
  }
}

// Animation
@keyframes slideUp {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

// Responsive
@media (max-width: 1024px) {
  .loading-state,
  .lesson-content {
    grid-template-columns: 1fr;
  }

  .lesson-sidebar {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
    gap: 1rem;
  }
}

@media (max-width: 768px) {
  .lesson-content {
    padding: 0 1rem 2rem;
  }

  .lesson-article {
    .article-header {
      padding: 1.5rem;

      .article-title {
        font-size: 1.5rem;
      }

      .author-section {
        flex-direction: column;
        gap: 0.75rem;
      }
    }

    .article-sections {
      padding: 1.5rem;
    }
  }

  .lesson-section {
    padding: 1rem;

    .section-header {
      .section-icon {
        width: 32px;
        height: 32px;
      }

      h2 {
        font-size: 1rem;
      }
    }
  }

  .tags-section {
    padding: 0 1.5rem 1rem;
  }

  .actions-bar {
    flex-direction: column;
    gap: 1rem;
    padding: 1rem 1.5rem;

    .actions-left {
      width: 100%;
      flex-wrap: wrap;
      justify-content: center;
    }

    .stats-display {
      width: 100%;
      justify-content: center;
    }
  }

  .comments-section {
    padding: 1.5rem;

    .comment-input-box {
      flex-direction: column;
      gap: 0.75rem;
    }

    .comment-card {
      flex-direction: column;
      padding: 1rem;
    }
  }
}
</style>
