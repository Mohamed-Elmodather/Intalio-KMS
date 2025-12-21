<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useReducedMotion } from '@/composables/useReducedMotion'
import ErrorState from '@/components/base/ErrorState.vue'
import { Avatar } from '@/components/base'
import Button from 'primevue/button'
import AvatarGroup from 'primevue/avatargroup'
import InputText from 'primevue/inputtext'
import Textarea from 'primevue/textarea'
import Skeleton from 'primevue/skeleton'
import Dialog from 'primevue/dialog'

const { locale } = useI18n()
const route = useRoute()
const router = useRouter()

const loading = ref(true)
const error = ref<Error | null>(null)
const submitting = ref(false)
const replyContent = ref('')
const replyingTo = ref<string | null>(null)
const showShareDialog = ref(false)
const shareLink = ref('')
const prefersReducedMotion = useReducedMotion()
const isContentVisible = ref(false)

// RTL support
const isRTL = computed(() => locale.value === 'ar')

// Animation control
const shouldAnimate = computed(() => !prefersReducedMotion.value)

// Type configurations
const DISCUSSION_TYPE_CONFIG: Record<string, { icon: string; color: string; bgColor: string; label: string; labelAr: string }> = {
  Discussion: { icon: 'pi pi-comments', color: '#3b82f6', bgColor: 'rgba(59, 130, 246, 0.1)', label: 'Discussion', labelAr: 'نقاش' },
  Question: { icon: 'pi pi-question-circle', color: '#f59e0b', bgColor: 'rgba(245, 158, 11, 0.1)', label: 'Question', labelAr: 'سؤال' },
  Announcement: { icon: 'pi pi-megaphone', color: '#ef4444', bgColor: 'rgba(239, 68, 68, 0.1)', label: 'Announcement', labelAr: 'إعلان' },
  Poll: { icon: 'pi pi-chart-bar', color: '#10b981', bgColor: 'rgba(16, 185, 129, 0.1)', label: 'Poll', labelAr: 'استطلاع' },
  Idea: { icon: 'pi pi-lightbulb', color: '#8b5cf6', bgColor: 'rgba(139, 92, 246, 0.1)', label: 'Idea', labelAr: 'فكرة' }
}

const STATUS_CONFIG: Record<string, { color: string; bgColor: string; label: string; labelAr: string }> = {
  Active: { color: '#10b981', bgColor: 'rgba(16, 185, 129, 0.1)', label: 'Active', labelAr: 'نشط' },
  Resolved: { color: '#3b82f6', bgColor: 'rgba(59, 130, 246, 0.1)', label: 'Resolved', labelAr: 'تم الحل' },
  Closed: { color: '#6b7280', bgColor: 'rgba(107, 114, 128, 0.1)', label: 'Closed', labelAr: 'مغلق' }
}

interface Comment {
  id: string
  content: string
  authorId: string
  authorName: string
  authorNameAr?: string
  authorAvatarUrl?: string
  authorJobTitle?: string
  authorJobTitleAr?: string
  likeCount: number
  replyCount: number
  isLiked: boolean
  isAcceptedAnswer: boolean
  replies: Comment[]
  createdAt: string
}

interface Discussion {
  id: string
  title: string
  titleArabic?: string
  content: string
  contentArabic?: string
  communityId: string
  communityName: string
  communityNameAr?: string
  communitySlug: string
  communityIcon?: string
  authorId: string
  authorName: string
  authorNameAr?: string
  authorAvatarUrl?: string
  authorJobTitle?: string
  authorJobTitleAr?: string
  type: string
  status: string
  isPinned: boolean
  isLocked: boolean
  viewCount: number
  replyCount: number
  likeCount: number
  isLiked: boolean
  isFollowing: boolean
  tags: string[]
  participants: { id: string; name: string; avatarUrl?: string }[]
  createdAt: string
  updatedAt?: string
}

const discussion = ref<Discussion | null>(null)
const comments = ref<Comment[]>([])

// Helper functions
const getTitle = (disc: Discussion | null) => {
  if (!disc) return ''
  return isRTL.value && disc.titleArabic ? disc.titleArabic : disc.title
}

const getContent = (disc: Discussion | null) => {
  if (!disc) return ''
  return isRTL.value && disc.contentArabic ? disc.contentArabic : disc.content
}

const getAuthorName = (item: { authorName: string; authorNameAr?: string }) => {
  return isRTL.value && item.authorNameAr ? item.authorNameAr : item.authorName
}

const getAuthorTitle = (item: { authorJobTitle?: string; authorJobTitleAr?: string }) => {
  return isRTL.value && item.authorJobTitleAr ? item.authorJobTitleAr : item.authorJobTitle
}

const getCommunityName = (disc: Discussion | null) => {
  if (!disc) return ''
  return isRTL.value && disc.communityNameAr ? disc.communityNameAr : disc.communityName
}

const getTypeConfig = (type: string) => {
  return DISCUSSION_TYPE_CONFIG[type] || DISCUSSION_TYPE_CONFIG.Discussion
}

const getStatusConfig = (status: string) => {
  return STATUS_CONFIG[status] || STATUS_CONFIG.Active
}

const formatDate = (date: string) => {
  return new Date(date).toLocaleDateString(isRTL.value ? 'ar-SA' : 'en-US', {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

const formatRelativeTime = (date: string) => {
  const now = new Date()
  const then = new Date(date)
  const diffMs = now.getTime() - then.getTime()
  const diffMinutes = Math.floor(diffMs / (1000 * 60))
  const diffHours = Math.floor(diffMinutes / 60)
  const diffDays = Math.floor(diffHours / 24)

  if (diffMinutes < 1) return isRTL.value ? 'الآن' : 'Just now'
  if (diffMinutes < 60) return isRTL.value ? `منذ ${diffMinutes} دقيقة` : `${diffMinutes}m ago`
  if (diffHours < 24) return isRTL.value ? `منذ ${diffHours} ساعة` : `${diffHours}h ago`
  if (diffDays < 7) return isRTL.value ? `منذ ${diffDays} يوم` : `${diffDays}d ago`
  return formatDate(date)
}

// Actions
const toggleLike = async () => {
  if (!discussion.value) return
  discussion.value.isLiked = !discussion.value.isLiked
  discussion.value.likeCount += discussion.value.isLiked ? 1 : -1
}

const toggleFollow = async () => {
  if (!discussion.value) return
  discussion.value.isFollowing = !discussion.value.isFollowing
}

const likeComment = async (comment: Comment) => {
  comment.isLiked = !comment.isLiked
  comment.likeCount += comment.isLiked ? 1 : -1
}

const markAsAnswer = async (comment: Comment) => {
  // Unmark any existing accepted answer
  comments.value.forEach(c => {
    c.isAcceptedAnswer = false
    c.replies.forEach(r => r.isAcceptedAnswer = false)
  })
  comment.isAcceptedAnswer = true
}

const startReply = (commentId: string | null) => {
  replyingTo.value = commentId
}

const cancelReply = () => {
  replyingTo.value = null
  replyContent.value = ''
}

const submitReply = async () => {
  if (!replyContent.value.trim()) return

  submitting.value = true
  await new Promise(resolve => setTimeout(resolve, 500))

  const newComment: Comment = {
    id: Date.now().toString(),
    content: replyContent.value,
    authorId: 'current-user',
    authorName: 'Current User',
    authorNameAr: 'المستخدم الحالي',
    authorJobTitle: 'Team Member',
    authorJobTitleAr: 'عضو الفريق',
    likeCount: 0,
    replyCount: 0,
    isLiked: false,
    isAcceptedAnswer: false,
    replies: [],
    createdAt: new Date().toISOString()
  }

  if (replyingTo.value) {
    const parent = comments.value.find(c => c.id === replyingTo.value)
    if (parent) {
      parent.replies.push(newComment)
      parent.replyCount++
    }
  } else {
    comments.value.unshift(newComment)
    if (discussion.value) discussion.value.replyCount++
  }

  replyContent.value = ''
  replyingTo.value = null
  submitting.value = false
}

const openShareDialog = () => {
  shareLink.value = window.location.href
  showShareDialog.value = true
}

const copyShareLink = async () => {
  try {
    await navigator.clipboard.writeText(shareLink.value)
  } catch {
    // Fallback for older browsers
  }
}

const goBack = () => {
  if (discussion.value?.communitySlug) {
    router.push({ name: 'community-detail', params: { slug: discussion.value.communitySlug } })
  } else {
    router.push({ name: 'communities' })
  }
}

// Load discussion data
const LOADING_DELAY = 600

async function loadDiscussion() {
  try {
    error.value = null
    loading.value = true

    await new Promise(resolve => setTimeout(resolve, LOADING_DELAY))

    discussion.value = {
    id: route.params.id as string,
    title: 'Stadium Security Protocols Update',
    titleArabic: 'تحديث بروتوكولات أمن الملاعب',
    content: `<h2>Security Protocol Review</h2>
<p>Team, we need to review and update our security protocols for the upcoming matches. Key areas to address:</p>
<ul>
<li>Entry point screening procedures</li>
<li>VIP area access control</li>
<li>Emergency evacuation routes</li>
<li>Communication protocols during incidents</li>
</ul>
<p>Please share your feedback and suggestions below. <strong>@Sara Ali</strong> can you coordinate with the security team?</p>
<p>We should aim to have the updated protocols ready by next week.</p>`,
    contentArabic: `<h2>مراجعة بروتوكولات الأمن</h2>
<p>الفريق، نحتاج إلى مراجعة وتحديث بروتوكولات الأمان الخاصة بنا للمباريات القادمة. المجالات الرئيسية التي يجب معالجتها:</p>
<ul>
<li>إجراءات الفحص عند نقاط الدخول</li>
<li>التحكم في الوصول إلى منطقة كبار الشخصيات</li>
<li>طرق الإخلاء في حالات الطوارئ</li>
<li>بروتوكولات الاتصال أثناء الحوادث</li>
</ul>
<p>يرجى مشاركة ملاحظاتكم واقتراحاتكم أدناه. <strong>@سارة علي</strong> هل يمكنك التنسيق مع فريق الأمن؟</p>
<p>يجب أن نهدف إلى أن تكون البروتوكولات المحدثة جاهزة بحلول الأسبوع المقبل.</p>`,
    communityId: '1',
    communityName: 'Stadium Operations',
    communityNameAr: 'عمليات الملاعب',
    communitySlug: 'stadium-operations',
    communityIcon: 'pi pi-building',
    authorId: '1',
    authorName: 'Mohammed Al-Rashid',
    authorNameAr: 'محمد الراشد',
    authorAvatarUrl: '/avatars/mohammed.jpg',
    authorJobTitle: 'Operations Director',
    authorJobTitleAr: 'مدير العمليات',
    type: 'Discussion',
    status: 'Active',
    isPinned: true,
    isLocked: false,
    viewCount: 234,
    replyCount: 18,
    likeCount: 45,
    isLiked: true,
    isFollowing: true,
    tags: ['Security', 'Protocols', 'Urgent'],
    participants: [
      { id: '1', name: 'Mohammed Al-Rashid', avatarUrl: '/avatars/mohammed.jpg' },
      { id: '2', name: 'Sara Ali', avatarUrl: '/avatars/sara.jpg' },
      { id: '3', name: 'Ahmed Hassan' },
      { id: '4', name: 'Fatima Noor' },
      { id: '5', name: 'Ali Mahmoud' }
    ],
    createdAt: new Date(Date.now() - 3 * 24 * 60 * 60 * 1000).toISOString(),
    updatedAt: new Date(Date.now() - 2 * 24 * 60 * 60 * 1000).toISOString()
  }

  comments.value = [
    {
      id: '1',
      content: "Great initiative! I'll coordinate with the security team right away. We should also consider adding metal detector protocols at all entry points. I've already reached out to the venue security leads.",
      authorId: '2',
      authorName: 'Sara Ali',
      authorNameAr: 'سارة علي',
      authorAvatarUrl: '/avatars/sara.jpg',
      authorJobTitle: 'Project Manager',
      authorJobTitleAr: 'مدير المشروع',
      likeCount: 12,
      replyCount: 2,
      isLiked: true,
      isAcceptedAnswer: false,
      replies: [
        {
          id: '1-1',
          content: "Agreed. Let's schedule a meeting this week to discuss the details. How about Thursday at 2 PM?",
          authorId: '1',
          authorName: 'Mohammed Al-Rashid',
          authorNameAr: 'محمد الراشد',
          authorAvatarUrl: '/avatars/mohammed.jpg',
          authorJobTitle: 'Operations Director',
          authorJobTitleAr: 'مدير العمليات',
          likeCount: 5,
          replyCount: 0,
          isLiked: false,
          isAcceptedAnswer: false,
          replies: [],
          createdAt: new Date(Date.now() - 4 * 60 * 60 * 1000).toISOString()
        },
        {
          id: '1-2',
          content: 'Thursday works for me. I will send calendar invites to the team.',
          authorId: '2',
          authorName: 'Sara Ali',
          authorNameAr: 'سارة علي',
          authorAvatarUrl: '/avatars/sara.jpg',
          authorJobTitle: 'Project Manager',
          authorJobTitleAr: 'مدير المشروع',
          likeCount: 3,
          replyCount: 0,
          isLiked: false,
          isAcceptedAnswer: false,
          replies: [],
          createdAt: new Date(Date.now() - 3 * 60 * 60 * 1000).toISOString()
        }
      ],
      createdAt: new Date(Date.now() - 6 * 60 * 60 * 1000).toISOString()
    },
    {
      id: '2',
      content: 'We should also update the emergency evacuation signage. I noticed some signs are outdated during my last venue walkthrough.',
      authorId: '3',
      authorName: 'Ahmed Hassan',
      authorNameAr: 'أحمد حسن',
      authorJobTitle: 'Operations Coordinator',
      authorJobTitleAr: 'منسق العمليات',
      likeCount: 8,
      replyCount: 0,
      isLiked: false,
      isAcceptedAnswer: false,
      replies: [],
      createdAt: new Date(Date.now() - 4 * 60 * 60 * 1000).toISOString()
    },
    {
      id: '3',
      content: 'I suggest we create a checklist for security audits. This will help us track compliance across all venues systematically.',
      authorId: '4',
      authorName: 'Fatima Noor',
      authorNameAr: 'فاطمة نور',
      authorJobTitle: 'Quality Analyst',
      authorJobTitleAr: 'محلل الجودة',
      likeCount: 15,
      replyCount: 0,
      isLiked: true,
      isAcceptedAnswer: true,
      replies: [],
      createdAt: new Date(Date.now() - 2 * 60 * 60 * 1000).toISOString()
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
    console.error('Failed to load discussion:', e)
  }
}

async function handleRetry() {
  await loadDiscussion()
}

onMounted(() => {
  loadDiscussion()
})
</script>

<template>
  <div class="discussion-view" :class="{ 'rtl': isRTL }">
    <!-- Page Header with Gradient -->
    <header class="page-header">
      <div class="header-background">
        <div class="bg-gradient"></div>
        <div class="bg-pattern"></div>
      </div>
      <div class="header-content">
        <div class="header-left">
          <nav class="breadcrumb" v-if="discussion">
            <button class="breadcrumb-link" @click="router.push({ name: 'dashboard' })">
              <i class="pi pi-home"></i>
              <span>{{ isRTL ? 'الرئيسية' : 'Home' }}</span>
            </button>
            <i class="pi pi-chevron-right breadcrumb-separator"></i>
            <button class="breadcrumb-link" @click="router.push({ name: 'communities' })">
              <span>{{ isRTL ? 'المجتمعات' : 'Communities' }}</span>
            </button>
            <i class="pi pi-chevron-right breadcrumb-separator"></i>
            <button class="breadcrumb-link" @click="router.push({ name: 'community-detail', params: { slug: discussion.communitySlug } })">
              <span>{{ getCommunityName(discussion) }}</span>
            </button>
            <i class="pi pi-chevron-right breadcrumb-separator"></i>
            <span class="breadcrumb-current">{{ isRTL ? 'النقاش' : 'Discussion' }}</span>
          </nav>
          <h1 class="page-title" v-if="discussion">{{ getTitle(discussion) }}</h1>
          <p class="page-description" v-if="discussion">
            {{ isRTL ? 'مجتمع' : 'Community' }}: {{ getCommunityName(discussion) }}
          </p>
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
        <Skeleton height="2.5rem" width="70%" class="mb-4" />
        <div class="loading-meta">
          <Skeleton shape="circle" size="3rem" />
          <div>
            <Skeleton height="1rem" width="150px" class="mb-2" />
            <Skeleton height="0.75rem" width="100px" />
          </div>
        </div>
        <Skeleton height="200px" class="mb-4 mt-4" />
      </div>
      <div class="loading-comments">
        <Skeleton height="100px" v-for="i in 3" :key="i" class="mb-3" />
      </div>
    </div>

    <!-- Error State -->
    <ErrorState
      v-else-if="error"
      :error="error"
      :title="isRTL ? 'فشل تحميل النقاش' : 'Failed to load discussion'"
      show-retry
      @retry="handleRetry"
    />

    <!-- Discussion Content -->
    <div v-else-if="discussion" class="discussion-content">
      <!-- Main Discussion Article -->
      <article class="discussion-article">
        <!-- Article Header -->
        <div class="article-header">
          <div class="badges-row">
            <span
              class="type-badge"
              :style="{
                color: getTypeConfig(discussion.type).color,
                backgroundColor: getTypeConfig(discussion.type).bgColor
              }"
            >
              <i :class="getTypeConfig(discussion.type).icon"></i>
              {{ isRTL ? getTypeConfig(discussion.type).labelAr : getTypeConfig(discussion.type).label }}
            </span>
            <span
              class="status-badge"
              :style="{
                color: getStatusConfig(discussion.status).color,
                backgroundColor: getStatusConfig(discussion.status).bgColor
              }"
            >
              {{ isRTL ? getStatusConfig(discussion.status).labelAr : getStatusConfig(discussion.status).label }}
            </span>
            <span v-if="discussion.isPinned" class="pinned-badge">
              <i class="pi pi-thumbtack"></i>
              {{ isRTL ? 'مثبت' : 'Pinned' }}
            </span>
            <span v-if="discussion.isLocked" class="locked-badge">
              <i class="pi pi-lock"></i>
              {{ isRTL ? 'مغلق' : 'Locked' }}
            </span>
          </div>

          <h1 class="article-title">{{ getTitle(discussion) }}</h1>

          <div class="author-section">
            <Avatar
              :image="discussion.authorAvatarUrl"
              :name="discussion.authorName"
              shape="circle"
              size="lg"
              class="author-avatar"
            />
            <div class="author-info">
              <span class="author-name">{{ getAuthorName(discussion) }}</span>
              <span class="author-title">{{ getAuthorTitle(discussion) }}</span>
              <div class="post-meta">
                <span class="post-date">
                  <i class="pi pi-calendar"></i>
                  {{ formatDate(discussion.createdAt) }}
                </span>
                <span v-if="discussion.updatedAt" class="updated-date">
                  <i class="pi pi-refresh"></i>
                  {{ isRTL ? 'تم التحديث' : 'Updated' }} {{ formatRelativeTime(discussion.updatedAt) }}
                </span>
              </div>
            </div>
          </div>
        </div>

        <!-- Article Body -->
        <div class="article-body" v-html="getContent(discussion)"></div>

        <!-- Tags -->
        <div v-if="discussion.tags.length" class="article-tags">
          <span v-for="tag in discussion.tags" :key="tag" class="tag-chip">
            <i class="pi pi-tag"></i>
            {{ tag }}
          </span>
        </div>

        <!-- Stats & Actions Bar -->
        <div class="article-footer">
          <div class="stats-section">
            <div class="stat-item">
              <i class="pi pi-eye"></i>
              <span class="stat-value">{{ discussion.viewCount }}</span>
              <span class="stat-label">{{ isRTL ? 'مشاهدة' : 'Views' }}</span>
            </div>
            <div class="stat-item">
              <i class="pi pi-comments"></i>
              <span class="stat-value">{{ discussion.replyCount }}</span>
              <span class="stat-label">{{ isRTL ? 'رد' : 'Replies' }}</span>
            </div>
            <div class="stat-item">
              <i class="pi pi-heart"></i>
              <span class="stat-value">{{ discussion.likeCount }}</span>
              <span class="stat-label">{{ isRTL ? 'إعجاب' : 'Likes' }}</span>
            </div>
          </div>

          <div class="participants-section" v-if="discussion.participants.length > 0">
            <span class="participants-label">{{ isRTL ? 'المشاركون' : 'Participants' }}</span>
            <AvatarGroup>
              <Avatar
                v-for="participant in discussion.participants.slice(0, 5)"
                :key="participant.id"
                :image="participant.avatarUrl"
                :name="participant.name"
                shape="circle"
                size="sm"
              />
              <Avatar
                v-if="discussion.participants.length > 5"
                :name="`+${discussion.participants.length - 5}`"
                shape="circle"
                size="sm"
                class="more-avatar"
              />
            </AvatarGroup>
          </div>

          <div class="actions-section">
            <button
              class="action-btn"
              :class="{ active: discussion.isLiked }"
              @click="toggleLike"
            >
              <i :class="discussion.isLiked ? 'pi pi-heart-fill' : 'pi pi-heart'"></i>
              <span>{{ discussion.isLiked ? (isRTL ? 'معجب' : 'Liked') : (isRTL ? 'إعجاب' : 'Like') }}</span>
            </button>
            <button
              class="action-btn"
              :class="{ active: discussion.isFollowing }"
              @click="toggleFollow"
            >
              <i :class="discussion.isFollowing ? 'pi pi-bell-slash' : 'pi pi-bell'"></i>
              <span>{{ discussion.isFollowing ? (isRTL ? 'إلغاء المتابعة' : 'Unfollow') : (isRTL ? 'متابعة' : 'Follow') }}</span>
            </button>
            <button class="action-btn" @click="openShareDialog">
              <i class="pi pi-share-alt"></i>
              <span>{{ isRTL ? 'مشاركة' : 'Share' }}</span>
            </button>
          </div>
        </div>
      </article>

      <!-- Reply Box -->
      <div v-if="!discussion.isLocked" class="reply-section">
        <div class="reply-header">
          <i class="pi pi-comment"></i>
          <h3>{{ isRTL ? 'اترك ردًا' : 'Leave a Reply' }}</h3>
        </div>
        <div class="reply-editor">
          <Textarea
            v-model="replyContent"
            :placeholder="isRTL ? 'شارك أفكارك...' : 'Share your thoughts...'"
            rows="4"
            autoResize
            class="reply-textarea"
          />
          <div class="reply-toolbar">
            <div class="formatting-hints">
              <span><i class="pi pi-at"></i> {{ isRTL ? 'إشارة' : 'Mention' }}</span>
              <span><i class="pi pi-hashtag"></i> {{ isRTL ? 'وسم' : 'Tag' }}</span>
            </div>
            <Button
              :label="isRTL ? 'نشر الرد' : 'Post Reply'"
              icon="pi pi-send"
              :loading="submitting && !replyingTo"
              :disabled="!replyContent.trim()"
              @click="submitReply"
              class="submit-btn"
            />
          </div>
        </div>
      </div>

      <div v-else class="locked-notice">
        <i class="pi pi-lock"></i>
        <span>{{ isRTL ? 'هذا النقاش مغلق ولا يمكن إضافة ردود جديدة' : 'This discussion is locked. New replies are not allowed.' }}</span>
      </div>

      <!-- Comments Section -->
      <div class="comments-section">
        <div class="section-header">
          <h3>
            <i class="pi pi-comments"></i>
            {{ isRTL ? 'الردود' : 'Replies' }}
            <span class="count-badge">{{ comments.length }}</span>
          </h3>
          <div class="sort-options">
            <button class="sort-btn active">
              <i class="pi pi-clock"></i>
              {{ isRTL ? 'الأحدث' : 'Newest' }}
            </button>
            <button class="sort-btn">
              <i class="pi pi-thumbs-up"></i>
              {{ isRTL ? 'الأكثر إعجابًا' : 'Most Liked' }}
            </button>
          </div>
        </div>

        <div v-if="comments.length === 0" class="empty-comments">
          <div class="empty-icon">
            <i class="pi pi-comments"></i>
          </div>
          <h4>{{ isRTL ? 'لا توجد ردود بعد' : 'No replies yet' }}</h4>
          <p>{{ isRTL ? 'كن أول من يشارك رأيه!' : 'Be the first to share your thoughts!' }}</p>
        </div>

        <div
          v-else
          class="comments-list"
          :class="{
            'comments-list--animated': shouldAnimate,
            'comments-list--visible': isContentVisible
          }"
        >
          <div
            v-for="(comment, index) in comments"
            :key="comment.id"
            class="comment-thread"
            :style="shouldAnimate ? { '--comment-index': index } : undefined"
          >
            <!-- Main Comment Card -->
            <div class="comment-card" :class="{ 'accepted-answer': comment.isAcceptedAnswer }">
              <div v-if="comment.isAcceptedAnswer" class="accepted-banner">
                <i class="pi pi-check-circle"></i>
                {{ isRTL ? 'الإجابة المقبولة' : 'Accepted Answer' }}
              </div>

              <div class="comment-main">
                <Avatar
                  :image="comment.authorAvatarUrl"
                  :name="comment.authorName"
                  shape="circle"
                  size="lg"
                  class="comment-avatar"
                />
                <div class="comment-body">
                  <div class="comment-header">
                    <div class="author-info">
                      <span class="author-name">{{ getAuthorName(comment) }}</span>
                      <span class="author-title">{{ getAuthorTitle(comment) }}</span>
                    </div>
                    <span class="comment-time">{{ formatRelativeTime(comment.createdAt) }}</span>
                  </div>
                  <div class="comment-content">{{ comment.content }}</div>
                  <div class="comment-actions">
                    <button
                      class="comment-action-btn like-btn"
                      :class="{ active: comment.isLiked, 'animate-pop': comment.isLiked && shouldAnimate }"
                      @click="likeComment(comment)"
                    >
                      <i :class="comment.isLiked ? 'pi pi-heart-fill' : 'pi pi-heart'"></i>
                      <span>{{ comment.likeCount }}</span>
                    </button>
                    <button
                      v-if="!discussion.isLocked"
                      class="comment-action-btn"
                      @click="startReply(comment.id)"
                    >
                      <i class="pi pi-reply"></i>
                      <span>{{ isRTL ? 'رد' : 'Reply' }}</span>
                    </button>
                    <button
                      v-if="discussion.type === 'Question' && !comment.isAcceptedAnswer"
                      class="comment-action-btn accept-btn"
                      @click="markAsAnswer(comment)"
                    >
                      <i class="pi pi-check"></i>
                      <span>{{ isRTL ? 'قبول كإجابة' : 'Accept' }}</span>
                    </button>
                  </div>

                  <!-- Inline Reply Form -->
                  <div v-if="replyingTo === comment.id" class="inline-reply-form">
                    <Textarea
                      v-model="replyContent"
                      :placeholder="isRTL ? 'اكتب ردك...' : 'Write your reply...'"
                      rows="3"
                      autoResize
                      class="inline-textarea"
                    />
                    <div class="inline-reply-actions">
                      <Button
                        :label="isRTL ? 'إلغاء' : 'Cancel'"
                        severity="secondary"
                        text
                        size="small"
                        @click="cancelReply"
                      />
                      <Button
                        :label="isRTL ? 'نشر' : 'Post'"
                        size="small"
                        :loading="submitting"
                        :disabled="!replyContent.trim()"
                        @click="submitReply"
                      />
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- Nested Replies -->
            <div v-if="comment.replies.length" class="nested-replies">
              <div v-for="reply in comment.replies" :key="reply.id" class="reply-card">
                <div class="reply-connector"></div>
                <Avatar
                  :image="reply.authorAvatarUrl"
                  :name="reply.authorName"
                  shape="circle"
                  size="sm"
                  class="reply-avatar"
                />
                <div class="reply-body">
                  <div class="reply-header">
                    <div class="author-info">
                      <span class="author-name">{{ getAuthorName(reply) }}</span>
                      <span class="author-title">{{ getAuthorTitle(reply) }}</span>
                    </div>
                    <span class="reply-time">{{ formatRelativeTime(reply.createdAt) }}</span>
                  </div>
                  <div class="reply-content">{{ reply.content }}</div>
                  <div class="reply-actions">
                    <button
                      class="comment-action-btn"
                      :class="{ active: reply.isLiked }"
                      @click="likeComment(reply)"
                    >
                      <i :class="reply.isLiked ? 'pi pi-heart-fill' : 'pi pi-heart'"></i>
                      <span>{{ reply.likeCount }}</span>
                    </button>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Share Dialog -->
    <Dialog
      v-model:visible="showShareDialog"
      :header="isRTL ? 'مشاركة النقاش' : 'Share Discussion'"
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
          <button class="social-btn twitter">
            <i class="pi pi-twitter"></i>
          </button>
          <button class="social-btn linkedin">
            <i class="pi pi-linkedin"></i>
          </button>
          <button class="social-btn email">
            <i class="pi pi-envelope"></i>
          </button>
        </div>
      </div>
    </Dialog>
  </div>
</template>

<style scoped lang="scss">
@use '@/assets/styles/_variables.scss' as *;
@use '@/assets/styles/_mixins.scss' as *;

// ============================================
// DISCUSSION VIEW - MAIN CONTAINER
// Following Universal Spacing Guidelines from _mixins.scss
// ============================================

.discussion-view {
  @include page-view;
  background: var(--surface-ground);

  &.rtl {
    direction: rtl;

    .breadcrumb-sep {
      transform: rotate(180deg);
    }
  }
}

// Header button styles (used in PageHeader slot)
.header-btn-secondary {
  @include header-btn-secondary;
}

// Loading State
.loading-state {
  max-width: 900px;
  margin: 0 auto;
  padding: 0 2rem;

  .loading-main {
    background: var(--surface-card);
    border-radius: 16px;
    padding: 2rem;
    margin-bottom: 2rem;
  }

  .loading-meta {
    display: flex;
    align-items: center;
    gap: 1rem;
  }

  .loading-comments {
    display: flex;
    flex-direction: column;
    gap: 1rem;
  }
}

// Discussion Content
.discussion-content {
  max-width: 900px;
  margin: 0 auto;
  padding: 0 2rem 3rem;
}

// Discussion Article
.discussion-article {
  background: var(--surface-card);
  border-radius: 16px;
  box-shadow: 0 4px 24px rgba(0, 0, 0, 0.06);
  overflow: hidden;
  margin-bottom: 2rem;
  animation: slideUp 0.5s ease-out;

  .article-header {
    padding: 2rem 2rem 1.5rem;
    border-bottom: 1px solid var(--surface-border);

    .badges-row {
      display: flex;
      flex-wrap: wrap;
      gap: 0.5rem;
      margin-bottom: 1rem;
    }

    .type-badge,
    .status-badge,
    .pinned-badge,
    .locked-badge {
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

    .pinned-badge {
      color: #f59e0b;
      background: rgba(245, 158, 11, 0.1);
    }

    .locked-badge {
      color: #6b7280;
      background: rgba(107, 114, 128, 0.1);
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
      align-items: flex-start;

      .author-avatar {
        flex-shrink: 0;
      }

      .author-info {
        display: flex;
        flex-direction: column;
        gap: 0.25rem;

        .author-name {
          font-weight: 600;
          font-size: 1rem;
          color: var(--text-color);
        }

        .author-title {
          font-size: 0.875rem;
          color: var(--text-color-secondary);
        }

        .post-meta {
          display: flex;
          flex-wrap: wrap;
          gap: 1rem;
          margin-top: 0.25rem;

          .post-date,
          .updated-date {
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

  .article-body {
    padding: 2rem;
    line-height: 1.8;
    font-size: 1rem;
    color: var(--text-color);

    :deep(h2) {
      font-size: 1.25rem;
      font-weight: 600;
      margin: 1.5rem 0 0.75rem;
      color: var(--text-color);
    }

    :deep(p) {
      margin-bottom: 1rem;
    }

    :deep(ul),
    :deep(ol) {
      padding-inline-start: 1.5rem;
      margin-bottom: 1rem;
    }

    :deep(li) {
      margin-bottom: 0.5rem;
    }

    :deep(strong) {
      color: #d4af37;
    }
  }

  .article-tags {
    display: flex;
    flex-wrap: wrap;
    gap: 0.5rem;
    padding: 0 2rem 1.5rem;

    .tag-chip {
      display: inline-flex;
      align-items: center;
      gap: 0.375rem;
      padding: 0.375rem 0.75rem;
      background: var(--surface-100);
      border-radius: 20px;
      font-size: 0.8125rem;
      color: var(--text-color-secondary);

      i {
        font-size: 0.7rem;
      }
    }
  }

  .article-footer {
    padding: 1.5rem 2rem;
    background: var(--surface-50);
    border-top: 1px solid var(--surface-border);
    display: flex;
    flex-wrap: wrap;
    gap: 1.5rem;
    align-items: center;

    .stats-section {
      display: flex;
      gap: 1.5rem;

      .stat-item {
        display: flex;
        align-items: center;
        gap: 0.375rem;
        color: var(--text-color-secondary);

        i {
          font-size: 1rem;
        }

        .stat-value {
          font-weight: 600;
          color: var(--text-color);
        }

        .stat-label {
          font-size: 0.8125rem;
        }
      }
    }

    .participants-section {
      display: flex;
      align-items: center;
      gap: 0.75rem;
      padding-inline-start: 1.5rem;
      border-inline-start: 1px solid var(--surface-border);

      .participants-label {
        font-size: 0.8125rem;
        color: var(--text-color-secondary);
      }

      .more-avatar {
        background: var(--primary-color);
        color: white;
        font-size: 0.75rem;
      }
    }

    .actions-section {
      display: flex;
      gap: 0.5rem;
      margin-inline-start: auto;

      .action-btn {
        display: flex;
        align-items: center;
        gap: 0.5rem;
        padding: 0.5rem 1rem;
        border: 1px solid var(--surface-border);
        border-radius: 8px;
        background: var(--surface-card);
        color: var(--text-color-secondary);
        cursor: pointer;
        transition: all 0.2s ease;
        font-size: 0.875rem;

        &:hover {
          border-color: var(--primary-color);
          color: var(--primary-color);
        }

        &.active {
          background: rgba(220, 38, 38, 0.1);
          border-color: #dc2626;
          color: #dc2626;

          &:nth-child(2) {
            background: rgba(59, 130, 246, 0.1);
            border-color: #3b82f6;
            color: #3b82f6;
          }
        }
      }
    }
  }
}

// Reply Section
.reply-section {
  background: var(--surface-card);
  border-radius: 16px;
  box-shadow: 0 4px 24px rgba(0, 0, 0, 0.06);
  padding: 1.5rem 2rem;
  margin-bottom: 2rem;

  .reply-header {
    display: flex;
    align-items: center;
    gap: 0.75rem;
    margin-bottom: 1rem;

    i {
      font-size: 1.25rem;
      color: var(--primary-color);
    }

    h3 {
      margin: 0;
      font-size: 1.125rem;
      font-weight: 600;
    }
  }

  .reply-editor {
    .reply-textarea {
      width: 100%;
      border-radius: 12px;

      :deep(.p-inputtextarea) {
        border-radius: 12px;
      }
    }

    .reply-toolbar {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-top: 1rem;

      .formatting-hints {
        display: flex;
        gap: 1rem;

        span {
          display: flex;
          align-items: center;
          gap: 0.375rem;
          font-size: 0.8125rem;
          color: var(--text-color-secondary);
        }
      }

      .submit-btn {
        border-radius: 8px;
      }
    }
  }
}

.locked-notice {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.75rem;
  padding: 1.5rem;
  background: var(--surface-100);
  border-radius: 12px;
  margin-bottom: 2rem;
  color: var(--text-color-secondary);

  i {
    font-size: 1.25rem;
  }
}

// Comments Section
.comments-section {
  .section-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 1.5rem;

    h3 {
      display: flex;
      align-items: center;
      gap: 0.75rem;
      margin: 0;
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
        font-weight: 600;
      }
    }

    .sort-options {
      display: flex;
      gap: 0.5rem;

      .sort-btn {
        display: flex;
        align-items: center;
        gap: 0.375rem;
        padding: 0.5rem 0.75rem;
        border: 1px solid var(--surface-border);
        border-radius: 8px;
        background: var(--surface-card);
        color: var(--text-color-secondary);
        font-size: 0.8125rem;
        cursor: pointer;
        transition: all 0.2s ease;

        &:hover,
        &.active {
          background: var(--primary-color);
          border-color: var(--primary-color);
          color: white;
        }
      }
    }
  }

  .empty-comments {
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

    h4 {
      margin: 0 0 0.5rem;
      font-size: 1.25rem;
      color: var(--text-color);
    }

    p {
      margin: 0;
      color: var(--text-color-secondary);
    }
  }

  .comments-list {
    display: flex;
    flex-direction: column;
    gap: 1.5rem;
  }
}

// Comment Thread
.comment-thread {
  animation: slideUp 0.4s ease-out;
  animation-delay: calc(var(--animation-order) * 0.1s);
  animation-fill-mode: backwards;
}

.comment-card {
  background: var(--surface-card);
  border-radius: 16px;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.04);
  overflow: hidden;
  transition: all 0.3s ease;

  &:hover {
    box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08);
  }

  &.accepted-answer {
    border: 2px solid #10b981;

    .accepted-banner {
      display: flex;
      align-items: center;
      justify-content: center;
      gap: 0.5rem;
      padding: 0.5rem;
      background: linear-gradient(135deg, #10b981 0%, #059669 100%);
      color: white;
      font-size: 0.8125rem;
      font-weight: 600;

      i {
        font-size: 0.875rem;
      }
    }
  }

  .accepted-banner {
    display: none;
  }

  .comment-main {
    display: flex;
    gap: 1rem;
    padding: 1.5rem;

    .comment-avatar {
      flex-shrink: 0;
    }

    .comment-body {
      flex: 1;
      min-width: 0;

      .comment-header {
        display: flex;
        justify-content: space-between;
        align-items: flex-start;
        margin-bottom: 0.75rem;

        .author-info {
          .author-name {
            display: block;
            font-weight: 600;
            color: var(--text-color);
          }

          .author-title {
            font-size: 0.8125rem;
            color: var(--text-color-secondary);
          }
        }

        .comment-time {
          font-size: 0.8125rem;
          color: var(--text-color-secondary);
        }
      }

      .comment-content {
        line-height: 1.7;
        color: var(--text-color);
        margin-bottom: 1rem;
      }

      .comment-actions {
        display: flex;
        gap: 0.75rem;

        .comment-action-btn {
          display: flex;
          align-items: center;
          gap: 0.375rem;
          padding: 0.375rem 0.75rem;
          border: none;
          border-radius: 6px;
          background: var(--surface-100);
          color: var(--text-color-secondary);
          font-size: 0.8125rem;
          cursor: pointer;
          transition: all 0.2s ease;

          &:hover {
            background: var(--surface-200);
          }

          &.active {
            background: rgba(220, 38, 38, 0.1);
            color: #dc2626;
          }

          &.accept-btn {
            &:hover {
              background: rgba(16, 185, 129, 0.1);
              color: #10b981;
            }
          }
        }
      }

      .inline-reply-form {
        margin-top: 1rem;
        padding-top: 1rem;
        border-top: 1px solid var(--surface-border);

        .inline-textarea {
          width: 100%;
          margin-bottom: 0.75rem;
        }

        .inline-reply-actions {
          display: flex;
          justify-content: flex-end;
          gap: 0.5rem;
        }
      }
    }
  }
}

// Nested Replies
.nested-replies {
  margin-inline-start: 3rem;
  padding-top: 0.75rem;
  display: flex;
  flex-direction: column;
  gap: 0.75rem;

  .reply-card {
    display: flex;
    gap: 1rem;
    position: relative;
    padding: 1rem 1.25rem;
    background: var(--surface-50);
    border-radius: 12px;
    border: 1px solid var(--surface-border);

    .reply-connector {
      position: absolute;
      top: -0.75rem;
      inset-inline-start: 1.5rem;
      width: 2px;
      height: calc(100% + 0.75rem);
      background: var(--surface-border);

      &::before {
        content: '';
        position: absolute;
        top: 0;
        inset-inline-start: 0;
        width: 1rem;
        height: 2px;
        background: var(--surface-border);
      }
    }

    .reply-avatar {
      flex-shrink: 0;
      width: 2.5rem !important;
      height: 2.5rem !important;
    }

    .reply-body {
      flex: 1;
      min-width: 0;

      .reply-header {
        display: flex;
        justify-content: space-between;
        align-items: flex-start;
        margin-bottom: 0.5rem;

        .author-info {
          .author-name {
            display: block;
            font-weight: 600;
            font-size: 0.9375rem;
            color: var(--text-color);
          }

          .author-title {
            font-size: 0.75rem;
            color: var(--text-color-secondary);
          }
        }

        .reply-time {
          font-size: 0.75rem;
          color: var(--text-color-secondary);
        }
      }

      .reply-content {
        line-height: 1.6;
        font-size: 0.9375rem;
        color: var(--text-color);
        margin-bottom: 0.75rem;
      }

      .reply-actions {
        .comment-action-btn {
          display: flex;
          align-items: center;
          gap: 0.375rem;
          padding: 0.25rem 0.5rem;
          border: none;
          border-radius: 4px;
          background: transparent;
          color: var(--text-color-secondary);
          font-size: 0.75rem;
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

          &:hover {
            background: #0d8ddb;
          }
        }

        &.linkedin {
          background: #0077b5;

          &:hover {
            background: #006097;
          }
        }

        &.email {
          background: #6b7280;

          &:hover {
            background: #4b5563;
          }
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

@keyframes commentCascade {
  from {
    opacity: 0;
    transform: translateY(24px) scale(0.97);
  }
  to {
    opacity: 1;
    transform: translateY(0) scale(1);
  }
}

@keyframes reactionPop {
  0% {
    transform: scale(1);
  }
  30% {
    transform: scale(1.3);
  }
  50% {
    transform: scale(0.9);
  }
  70% {
    transform: scale(1.1);
  }
  100% {
    transform: scale(1);
  }
}

@keyframes heartBeat {
  0%, 100% {
    transform: scale(1);
  }
  25% {
    transform: scale(1.1);
  }
  50% {
    transform: scale(1);
  }
  75% {
    transform: scale(1.05);
  }
}

// ============================================
// COMMENT CASCADE ANIMATION
// ============================================
.comments-list {
  &--animated {
    .comment-thread {
      opacity: 0;
      transform: translateY(24px) scale(0.97);
    }

    &.comments-list--visible {
      .comment-thread {
        animation: commentCascade 0.5s ease-out forwards;
        animation-delay: calc(var(--comment-index, 0) * 120ms);
      }
    }
  }

  &:not(.comments-list--animated) {
    .comment-thread {
      opacity: 1;
      transform: none;
      animation: none;
    }
  }
}

// Remove old animation-order based animation
.comment-thread {
  animation: none;
  animation-fill-mode: unset;
}

// ============================================
// REACTION POP ANIMATION
// ============================================
.like-btn {
  &.animate-pop {
    i {
      animation: reactionPop 0.4s ease-out;
    }
  }

  &.active i {
    color: #dc2626;
  }
}

// ============================================
// ENHANCED COMMENT CARD HOVER
// ============================================
.comment-card {
  &:hover {
    box-shadow:
      0 8px 24px rgba(0, 0, 0, 0.1),
      0 4px 12px rgba(0, 0, 0, 0.05);
    transform: translateY(-2px);
  }
}

// ============================================
// NESTED REPLY EXPAND ANIMATION
// ============================================
.nested-replies {
  .reply-card {
    transition: all 0.3s ease;

    &:hover {
      background: var(--surface-100);
      transform: translateX(4px);
    }
  }
}

// RTL adjustments
.rtl {
  .nested-replies .reply-card:hover {
    transform: translateX(-4px);
  }
}

// ============================================
// REDUCED MOTION SUPPORT
// ============================================
@media (prefers-reduced-motion: reduce) {
  .comments-list {
    &--animated {
      .comment-thread {
        opacity: 1;
        transform: none;
        animation: none !important;
      }
    }
  }

  .comment-thread {
    animation: none !important;
  }

  .like-btn.animate-pop i {
    animation: none !important;
  }

  .comment-card {
    transition: box-shadow 0.15s;

    &:hover {
      transform: none;
    }
  }

  .nested-replies .reply-card {
    transition: background-color 0.15s;

    &:hover {
      transform: none;
    }
  }

  .discussion-article {
    animation: none !important;
  }
}

// Responsive
@media (max-width: 768px) {
  .discussion-content {
    padding: 0 1rem 2rem;
  }

  .discussion-article {
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

    .article-body {
      padding: 1.5rem;
    }

    .article-tags {
      padding: 0 1.5rem 1rem;
    }

    .article-footer {
      padding: 1rem 1.5rem;
      flex-direction: column;
      gap: 1rem;

      .stats-section {
        width: 100%;
        justify-content: center;
      }

      .participants-section {
        padding-inline-start: 0;
        border-inline-start: none;
        padding-top: 1rem;
        border-top: 1px solid var(--surface-border);
        width: 100%;
        justify-content: center;
      }

      .actions-section {
        width: 100%;
        justify-content: center;
        margin-inline-start: 0;
      }
    }
  }

  .reply-section {
    padding: 1rem 1.5rem;

    .reply-toolbar {
      flex-direction: column;
      gap: 1rem;

      .formatting-hints {
        justify-content: center;
      }

      .submit-btn {
        width: 100%;
      }
    }
  }

  .comments-section {
    .section-header {
      flex-direction: column;
      gap: 1rem;
      align-items: flex-start;
    }
  }

  .comment-card {
    .comment-main {
      padding: 1rem;
      flex-direction: column;
      gap: 0.75rem;
    }
  }

  .nested-replies {
    margin-inline-start: 1rem;

    .reply-card {
      padding: 0.75rem 1rem;
    }
  }
}
</style>
