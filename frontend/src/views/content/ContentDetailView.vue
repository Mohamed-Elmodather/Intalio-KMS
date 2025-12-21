<script setup lang="ts">
/**
 * ContentDetailView - Article/Content Detail Page
 *
 * Displays a full article with hero image, content, author info, and related articles.
 * Uses Design System components, tokens, and mixins.
 */
import { ref, computed, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRoute, useRouter } from 'vue-router'
import { useReducedMotion } from '@/composables/useReducedMotion'
import Tag from 'primevue/tag'
import Skeleton from 'primevue/skeleton'

// Base Components
import ErrorState from '@/components/base/ErrorState.vue'

const { t, locale } = useI18n()
const route = useRoute()
const router = useRouter()
const prefersReducedMotion = useReducedMotion()

const isRTL = computed(() => locale.value === 'ar')
const isLoading = ref(true)
const isContentVisible = ref(false)
const error = ref<Error | null>(null)

// Animation control
const shouldAnimate = computed(() => !prefersReducedMotion.value)
const LOADING_DELAY = 600

interface ArticleAuthor {
  name: string
  nameArabic?: string
  role?: string
  roleArabic?: string
  avatar?: string | null
}

interface ArticleData {
  id: string
  title: string
  titleArabic: string
  content: string
  contentArabic: string
  summary: string
  summaryArabic: string
  category: string
  categoryArabic: string
  tags: Array<{ id: string; name: string; nameArabic: string; color: string }>
  status: string
  author: ArticleAuthor
  createdAt: Date
  updatedAt: Date
  publishedAt: Date
  views: number
  readTime: number
  commentCount: number
  thumbnailUrl?: string
}

const article = ref<ArticleData>({
  id: '1',
  title: 'AFC Asian Cup 2027 Venues Announced',
  titleArabic: 'الإعلان عن ملاعب كأس آسيا 2027',
  summary: 'Saudi Arabia unveils world-class stadiums for the tournament, featuring state-of-the-art facilities.',
  summaryArabic: 'المملكة العربية السعودية تكشف عن ملاعب عالمية المستوى للبطولة',
  content: `
    <p>The Asian Football Confederation has officially announced the venues for the AFC Asian Cup 2027, to be hosted in Saudi Arabia. This marks a historic moment for Asian football as the Kingdom prepares to welcome teams and fans from across the continent.</p>

    <h2>World-Class Facilities</h2>
    <p>The tournament will feature five state-of-the-art stadiums, each equipped with cutting-edge technology and designed to provide an unparalleled experience for players and spectators alike. The venues have been strategically selected across different regions of Saudi Arabia to maximize accessibility and showcase the nation's diverse landscape.</p>

    <h2>Stadium Highlights</h2>
    <ul>
      <li><strong>King Fahd International Stadium</strong> - The flagship venue with a capacity of 68,000, featuring advanced cooling technology</li>
      <li><strong>Prince Sultan Stadium</strong> - A newly constructed marvel with retractable roof technology</li>
      <li><strong>Al-Awwal Park</strong> - Historic venue recently renovated to meet FIFA standards</li>
      <li><strong>King Abdullah Sports City</strong> - Multi-purpose complex with world-class training facilities</li>
      <li><strong>Jeddah Superdome</strong> - Coastal venue offering stunning views of the Red Sea</li>
    </ul>

    <h2>Sustainability Commitment</h2>
    <p>In line with Saudi Arabia's Vision 2030, all venues will incorporate sustainable design elements, including solar power integration, water recycling systems, and energy-efficient lighting. The organizing committee has pledged to make this the most environmentally conscious Asian Cup in history.</p>

    <blockquote>
      "We are committed to delivering a world-class tournament that will leave a lasting legacy for Saudi Arabia and Asian football as a whole."
      <cite>— Tournament Director</cite>
    </blockquote>

    <h2>Transportation & Access</h2>
    <p>A comprehensive transportation plan has been developed to ensure smooth movement between venues. This includes dedicated shuttle services, expanded public transit options, and special provisions for fans with disabilities.</p>

    <p>Ticket sales are expected to begin in early 2026, with priority access for AFC member association fans and local supporters.</p>
  `,
  contentArabic: `
    <p>أعلن الاتحاد الآسيوي لكرة القدم رسمياً عن ملاعب كأس آسيا 2027 التي ستستضيفها المملكة العربية السعودية. تمثل هذه اللحظة تاريخية لكرة القدم الآسيوية حيث تستعد المملكة لاستقبال الفرق والمشجعين من جميع أنحاء القارة.</p>

    <h2>مرافق عالمية المستوى</h2>
    <p>ستضم البطولة خمسة ملاعب حديثة، كل منها مجهز بأحدث التقنيات ومصمم لتوفير تجربة لا مثيل لها للاعبين والمتفرجين على حد سواء.</p>

    <h2>أبرز الملاعب</h2>
    <ul>
      <li><strong>استاد الملك فهد الدولي</strong> - الملعب الرئيسي بسعة 68,000 متفرج</li>
      <li><strong>استاد الأمير سلطان</strong> - تحفة معمارية جديدة بسقف قابل للطي</li>
      <li><strong>الأول بارك</strong> - ملعب تاريخي تم تجديده حديثاً</li>
      <li><strong>مدينة الملك عبدالله الرياضية</strong> - مجمع رياضي متعدد الأغراض</li>
      <li><strong>قبة جدة</strong> - ملعب ساحلي يطل على البحر الأحمر</li>
    </ul>
  `,
  category: 'Tournament News',
  categoryArabic: 'أخبار البطولة',
  tags: [
    { id: '1', name: 'Venues', nameArabic: 'الملاعب', color: '#00ae8d' },
    { id: '2', name: 'Tournament', nameArabic: 'البطولة', color: '#3b82f6' },
    { id: '3', name: 'Saudi Arabia', nameArabic: 'السعودية', color: '#10b981' }
  ],
  status: 'published',
  author: {
    name: 'Mohammed Al-Rashid',
    nameArabic: 'محمد الراشد',
    role: 'Senior Sports Editor',
    roleArabic: 'محرر رياضي أول',
    avatar: null
  },
  createdAt: new Date('2024-12-01T10:00:00Z'),
  updatedAt: new Date('2024-12-01T14:30:00Z'),
  publishedAt: new Date('2024-12-01T10:00:00Z'),
  views: 15420,
  readTime: 5,
  commentCount: 45,
  thumbnailUrl: 'https://images.unsplash.com/photo-1522778119026-d647f0596c20?w=1200&h=600&fit=crop'
})

const relatedArticles = ref([
  {
    id: '2',
    title: 'Volunteer Registration Opens',
    titleArabic: 'فتح باب التسجيل للمتطوعين',
    thumbnailUrl: 'https://images.unsplash.com/photo-1559027615-cd4628902d4a?w=400&h=300&fit=crop',
    category: 'Announcements',
    publishedAt: '2024-11-28'
  },
  {
    id: '3',
    title: 'Teams Qualification Path Revealed',
    titleArabic: 'الكشف عن مسار تأهل المنتخبات',
    thumbnailUrl: 'https://images.unsplash.com/photo-1574629810360-7efbbe195018?w=400&h=300&fit=crop',
    category: 'Tournament News',
    publishedAt: '2024-11-25'
  },
  {
    id: '4',
    title: 'Sustainability Initiatives',
    titleArabic: 'مبادرات الاستدامة',
    thumbnailUrl: 'https://images.unsplash.com/photo-1518531933037-91b2f5f229cc?w=400&h=300&fit=crop',
    category: 'Features',
    publishedAt: '2024-11-20'
  }
])

const getTitle = computed(() => isRTL.value ? article.value.titleArabic : article.value.title)
const getSummary = computed(() => isRTL.value ? article.value.summaryArabic : article.value.summary)
const getContent = computed(() => isRTL.value ? article.value.contentArabic : article.value.content)
const getCategory = computed(() => isRTL.value ? article.value.categoryArabic : article.value.category)
const getAuthorName = computed(() => isRTL.value ? article.value.author.nameArabic : article.value.author.name)
const getAuthorRole = computed(() => isRTL.value ? article.value.author.roleArabic : article.value.author.role)

const formatDate = (date: Date | string) => {
  const d = new Date(date)
  return d.toLocaleDateString(locale.value === 'ar' ? 'ar-SA' : 'en-US', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  })
}

const formatViewCount = (count: number) => {
  if (count >= 1000) {
    return (count / 1000).toFixed(1) + 'K'
  }
  return count.toString()
}

function editArticle() {
  router.push(`/content/edit/${route.params.slug}`)
}

function goBack() {
  router.push('/content')
}

function viewRelated(id: string) {
  router.push({ name: 'content-view', params: { id: id } })
}

function shareArticle(platform: string) {
  const url = window.location.href
  const title = getTitle.value

  const shareUrls: Record<string, string> = {
    twitter: `https://twitter.com/intent/tweet?text=${encodeURIComponent(title)}&url=${encodeURIComponent(url)}`,
    facebook: `https://www.facebook.com/sharer/sharer.php?u=${encodeURIComponent(url)}`,
    linkedin: `https://www.linkedin.com/sharing/share-offsite/?url=${encodeURIComponent(url)}`,
    copy: url
  }

  if (platform === 'copy') {
    navigator.clipboard.writeText(url)
  } else {
    window.open(shareUrls[platform], '_blank', 'width=600,height=400')
  }
}

async function loadArticle() {
  try {
    error.value = null
    // Simulate API call
    await new Promise(resolve => setTimeout(resolve, LOADING_DELAY))
    isLoading.value = false

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
    isLoading.value = false
  }
}

function handleRetry() {
  isLoading.value = true
  loadArticle()
}

onMounted(() => {
  loadArticle()
})
</script>

<template>
  <div
    class="content-detail-view"
    :class="{
      'content-detail-view--rtl': isRTL,
      'content-detail-view--animated': shouldAnimate,
      'content-detail-view--visible': isContentVisible
    }"
  >
    <!-- Error State -->
    <ErrorState
      v-if="error"
      :error="error"
      :title="isRTL ? 'فشل تحميل المقال' : 'Failed to load article'"
      show-retry
      size="lg"
      @retry="handleRetry"
    />

    <!-- Loading State -->
    <div v-else-if="isLoading" class="loading-container">
      <div class="loading-header">
        <Skeleton height="300px" class="hero-skeleton" />
      </div>
      <div class="loading-content">
        <Skeleton width="30%" height="20px" class="mb-3" />
        <Skeleton width="90%" height="40px" class="mb-4" />
        <Skeleton width="100%" height="16px" class="mb-2" />
        <Skeleton width="100%" height="16px" class="mb-2" />
        <Skeleton width="80%" height="16px" class="mb-4" />
        <Skeleton width="100%" height="200px" />
      </div>
    </div>

    <!-- Article Content -->
    <article v-else class="article-container" :class="{ 'article-container--visible': isContentVisible }">
      <!-- Hero Section -->
      <header class="article-hero">
        <div class="hero-background">
          <img
            v-if="article.thumbnailUrl"
            :src="article.thumbnailUrl"
            :alt="getTitle"
            class="hero-image"
          />
          <div class="hero-overlay"></div>
        </div>

        <div class="hero-content">
          <!-- Breadcrumb & Actions -->
          <div class="hero-top">
            <nav class="breadcrumb">
              <button class="back-btn" @click="goBack">
                <i class="pi pi-arrow-left"></i>
                {{ t('common.back') }}
              </button>
              <span class="separator">/</span>
              <span class="current">{{ getCategory }}</span>
            </nav>
            <div class="hero-actions">
              <button class="action-btn" @click="editArticle" :title="t('common.edit')">
                <i class="pi pi-pencil"></i>
              </button>
              <button class="action-btn" @click="shareArticle('copy')" :title="locale === 'ar' ? 'نسخ الرابط' : 'Copy Link'">
                <i class="pi pi-link"></i>
              </button>
            </div>
          </div>

          <!-- Article Meta -->
          <div class="hero-meta">
            <Tag :value="getCategory" severity="info" class="category-tag" />
            <span class="status-badge" :class="article.status">
              {{ article.status === 'published' ? (locale === 'ar' ? 'منشور' : 'Published') : (locale === 'ar' ? 'مسودة' : 'Draft') }}
            </span>
          </div>

          <!-- Title -->
          <h1 class="article-title">{{ getTitle }}</h1>
          <p class="article-summary">{{ getSummary }}</p>

          <!-- Article Stats -->
          <div class="hero-stats">
            <span class="stat">
              <i class="pi pi-calendar"></i>
              {{ formatDate(article.publishedAt) }}
            </span>
            <span class="stat">
              <i class="pi pi-clock"></i>
              {{ article.readTime }} {{ locale === 'ar' ? 'دقائق للقراءة' : 'min read' }}
            </span>
            <span class="stat">
              <i class="pi pi-eye"></i>
              {{ formatViewCount(article.views) }} {{ locale === 'ar' ? 'مشاهدة' : 'views' }}
            </span>
            <span class="stat">
              <i class="pi pi-comments"></i>
              {{ article.commentCount }} {{ locale === 'ar' ? 'تعليق' : 'comments' }}
            </span>
          </div>
        </div>
      </header>

      <!-- Main Content Area -->
      <div class="article-layout">
        <!-- Content Column -->
        <main class="article-main">
          <!-- Author Card -->
          <div class="author-card">
            <div class="author-avatar">
              <span class="avatar-text">{{ (article.author.name || 'U').charAt(0) }}</span>
            </div>
            <div class="author-info">
              <span class="author-name">{{ getAuthorName }}</span>
              <span class="author-role">{{ getAuthorRole }}</span>
            </div>
            <button class="follow-btn">
              <i class="pi pi-user-plus"></i>
              {{ locale === 'ar' ? 'متابعة' : 'Follow' }}
            </button>
          </div>

          <!-- Article Body -->
          <div class="article-body" v-html="getContent"></div>

          <!-- Tags Section -->
          <div class="article-tags">
            <span class="tags-label">
              <i class="pi pi-tag"></i>
              {{ locale === 'ar' ? 'الوسوم' : 'Tags' }}
            </span>
            <div class="tags-list">
              <span
                v-for="tag in article.tags"
                :key="tag.id"
                class="tag-chip"
                :style="{ '--tag-color': tag.color }"
              >
                {{ isRTL ? tag.nameArabic : tag.name }}
              </span>
            </div>
          </div>

          <!-- Share Section -->
          <div class="share-section">
            <span class="share-label">{{ locale === 'ar' ? 'شارك المقال' : 'Share this article' }}</span>
            <div class="share-buttons">
              <button class="share-btn twitter" @click="shareArticle('twitter')">
                <i class="pi pi-twitter"></i>
              </button>
              <button class="share-btn facebook" @click="shareArticle('facebook')">
                <i class="pi pi-facebook"></i>
              </button>
              <button class="share-btn linkedin" @click="shareArticle('linkedin')">
                <i class="pi pi-linkedin"></i>
              </button>
              <button class="share-btn copy" @click="shareArticle('copy')">
                <i class="pi pi-copy"></i>
              </button>
            </div>
          </div>
        </main>

        <!-- Sidebar -->
        <aside class="article-sidebar">
          <!-- Related Articles -->
          <div class="sidebar-section">
            <h3 class="section-title">
              <i class="pi pi-book"></i>
              {{ locale === 'ar' ? 'مقالات ذات صلة' : 'Related Articles' }}
            </h3>
            <div class="related-articles">
              <article
                v-for="related in relatedArticles"
                :key="related.id"
                class="related-card"
                @click="viewRelated(related.id)"
              >
                <div class="related-image">
                  <img :src="related.thumbnailUrl" :alt="isRTL ? related.titleArabic : related.title" />
                </div>
                <div class="related-content">
                  <span class="related-category">{{ related.category }}</span>
                  <h4 class="related-title">{{ isRTL ? related.titleArabic : related.title }}</h4>
                  <span class="related-date">{{ formatDate(related.publishedAt) }}</span>
                </div>
              </article>
            </div>
          </div>

          <!-- Quick Actions -->
          <div class="sidebar-section">
            <h3 class="section-title">
              <i class="pi pi-bolt"></i>
              {{ locale === 'ar' ? 'إجراءات سريعة' : 'Quick Actions' }}
            </h3>
            <div class="quick-actions">
              <button class="quick-action-btn" @click="editArticle">
                <i class="pi pi-pencil"></i>
                {{ t('common.edit') }}
              </button>
              <button class="quick-action-btn">
                <i class="pi pi-bookmark"></i>
                {{ locale === 'ar' ? 'حفظ' : 'Bookmark' }}
              </button>
              <button class="quick-action-btn">
                <i class="pi pi-print"></i>
                {{ locale === 'ar' ? 'طباعة' : 'Print' }}
              </button>
            </div>
          </div>
        </aside>
      </div>
    </article>
  </div>
</template>

<style scoped lang="scss">
// Design System
@use '@/design-system/mixins' as *;
// Note: Tokens available via global injection from vite.config.ts

.content-detail-view {
  min-height: 100vh;

  &--rtl {
    direction: rtl;

    .breadcrumb .back-btn i {
      transform: rotate(180deg);
    }

    .related-card:hover {
      transform: translateX(-4px);
    }
  }
}

// ============================================
// LOADING STATE
// ============================================
.loading-container {
  .loading-header {
    margin: (-$spacing-6) 0 $spacing-6;

    .hero-skeleton {
      border-radius: 0 0 $radius-2xl $radius-2xl;
    }
  }

  .loading-content {
    max-width: 800px;
    margin: 0 auto;
    padding: $spacing-6;
  }
}

// ============================================
// HERO SECTION
// ============================================
.article-hero {
  position: relative;
  margin: (-$spacing-6) 0 $spacing-6;
  min-height: 420px;
  display: flex;
  flex-direction: column;
  justify-content: flex-end;
  border-radius: 0 0 $radius-2xl $radius-2xl;
  overflow: hidden;

  .hero-background {
    position: absolute;
    inset: 0;
    z-index: 0;

    .hero-image {
      width: 100%;
      height: 100%;
      object-fit: cover;
    }

    .hero-overlay {
      position: absolute;
      inset: 0;
      background: linear-gradient(
        to top,
        rgba(0, 0, 0, 0.85) 0%,
        rgba(0, 0, 0, 0.5) 40%,
        rgba(0, 0, 0, 0.2) 70%,
        rgba(0, 0, 0, 0.3) 100%
      );
    }
  }

  .hero-content {
    position: relative;
    z-index: 1;
    padding: $spacing-8 $spacing-6;
    color: white;
  }

  .hero-top {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: $spacing-6;
  }

  .breadcrumb {
    display: flex;
    align-items: center;
    gap: $spacing-2;
    font-size: $text-sm;
    color: rgba(255, 255, 255, 0.8);

    .back-btn {
      display: flex;
      align-items: center;
      gap: $spacing-2;
      padding: $spacing-2 $spacing-3;
      background: rgba(255, 255, 255, 0.1);
      border: 1px solid rgba(255, 255, 255, 0.2);
      border-radius: $radius-lg;
      color: white;
      cursor: pointer;
      transition: all $transition-fast;

      &:hover {
        background: rgba(255, 255, 255, 0.2);
      }
    }

    .separator {
      opacity: 0.5;
    }

    .current {
      color: white;
    }
  }

  .hero-actions {
    display: flex;
    gap: $spacing-2;

    .action-btn {
      width: 40px;
      height: 40px;
      display: flex;
      align-items: center;
      justify-content: center;
      background: rgba(255, 255, 255, 0.1);
      border: 1px solid rgba(255, 255, 255, 0.2);
      border-radius: $radius-lg;
      color: white;
      cursor: pointer;
      transition: all $transition-fast;

      &:hover {
        background: rgba(255, 255, 255, 0.2);
        transform: translateY(-2px);
      }
    }
  }

  .hero-meta {
    display: flex;
    align-items: center;
    gap: $spacing-3;
    margin-bottom: $spacing-4;

    .category-tag {
      font-size: $text-xs;
    }

    .status-badge {
      padding: $spacing-1 $spacing-3;
      font-size: $text-xs;
      font-weight: 500;
      border-radius: $radius-full;

      &.published {
        background: rgba($success-500, 0.2);
        color: $success-100;
      }

      &.draft {
        background: rgba($warning-500, 0.2);
        color: $warning-100;
      }
    }
  }

  .article-title {
    margin: 0 0 $spacing-3 0;
    font-size: $text-4xl;
    font-weight: 700;
    line-height: 1.2;
    letter-spacing: -0.02em;
  }

  .article-summary {
    margin: 0 0 $spacing-6 0;
    font-size: $text-lg;
    color: rgba(255, 255, 255, 0.85);
    line-height: 1.6;
    max-width: 700px;
  }

  .hero-stats {
    display: flex;
    flex-wrap: wrap;
    gap: $spacing-4;

    .stat {
      display: flex;
      align-items: center;
      gap: $spacing-2;
      font-size: $text-sm;
      color: rgba(255, 255, 255, 0.7);

      i {
        font-size: $text-xs;
      }
    }
  }
}

// ============================================
// ARTICLE LAYOUT
// ============================================
.article-layout {
  display: grid;
  grid-template-columns: 1fr 320px;
  gap: $spacing-8;
  max-width: 1200px;
  margin: -$spacing-8 auto 0;
  padding-top: $spacing-2;
  position: relative;
  z-index: 2;
}

// ============================================
// MAIN CONTENT
// ============================================
.article-main {
  min-width: 0;
}

.author-card {
  display: flex;
  align-items: center;
  gap: $spacing-4;
  padding: $spacing-4;
  background: var(--surface-card);
  border-radius: $radius-xl;
  box-shadow: $shadow-sm;
  margin-bottom: $spacing-6;

  .author-avatar {
    width: 48px;
    height: 48px;
    display: flex;
    align-items: center;
    justify-content: center;
    background: $gradient-primary;
    border-radius: $radius-full;

    .avatar-text {
      color: white;
      font-size: $text-lg;
      font-weight: 600;
    }
  }

  .author-info {
    flex: 1;
    display: flex;
    flex-direction: column;
    gap: $spacing-1;

    .author-name {
      font-weight: 600;
      color: var(--text-color);
    }

    .author-role {
      font-size: $text-sm;
      color: var(--text-color-secondary);
    }
  }

  .follow-btn {
    display: flex;
    align-items: center;
    gap: $spacing-2;
    padding: $spacing-2 $spacing-4;
    background: $intalio-teal-50;
    border: 1px solid $intalio-teal-200;
    border-radius: $radius-lg;
    color: $intalio-teal-600;
    font-size: $text-sm;
    font-weight: 500;
    cursor: pointer;
    transition: all $transition-fast;

    &:hover {
      background: $intalio-teal-100;
      border-color: $intalio-teal-300;
    }
  }
}

.article-body {
  background: var(--surface-card);
  padding: $spacing-8;
  border-radius: $radius-xl;
  box-shadow: $shadow-sm;
  line-height: 1.8;
  font-size: $text-base;
  color: var(--text-color);

  :deep(h2) {
    margin: $spacing-8 0 $spacing-4 0;
    font-size: $text-2xl;
    font-weight: 600;
    color: var(--text-color);

    &:first-child {
      margin-top: 0;
    }
  }

  :deep(h3) {
    margin: $spacing-6 0 $spacing-3 0;
    font-size: $text-xl;
    font-weight: 600;
  }

  :deep(p) {
    margin-bottom: $spacing-4;
    color: var(--text-color-secondary);
  }

  :deep(ul), :deep(ol) {
    margin: $spacing-4 0;
    padding-inline-start: $spacing-6;

    li {
      margin-bottom: $spacing-2;
      color: var(--text-color-secondary);
    }
  }

  :deep(blockquote) {
    margin: $spacing-6 0;
    padding: $spacing-4 $spacing-6;
    background: $intalio-teal-50;
    border-inline-start: 4px solid $intalio-teal-500;
    border-radius: 0 $radius-lg $radius-lg 0;
    font-style: italic;
    color: $intalio-teal-800;

    cite {
      display: block;
      margin-top: $spacing-2;
      font-size: $text-sm;
      font-style: normal;
      color: $intalio-teal-600;
    }
  }

  :deep(strong) {
    color: var(--text-color);
    font-weight: 600;
  }
}

.article-tags {
  display: flex;
  align-items: center;
  gap: $spacing-4;
  margin-top: $spacing-6;
  padding: $spacing-4;
  background: var(--surface-card);
  border-radius: $radius-xl;
  box-shadow: $shadow-sm;

  .tags-label {
    display: flex;
    align-items: center;
    gap: $spacing-2;
    font-size: $text-sm;
    font-weight: 500;
    color: var(--text-color-secondary);

    i {
      font-size: $text-xs;
    }
  }

  .tags-list {
    display: flex;
    flex-wrap: wrap;
    gap: $spacing-2;
  }

  .tag-chip {
    padding: $spacing-1 $spacing-3;
    background: color-mix(in srgb, var(--tag-color) 10%, transparent);
    color: var(--tag-color);
    font-size: $text-sm;
    font-weight: 500;
    border-radius: $radius-full;
    transition: all $transition-fast;

    &:hover {
      background: color-mix(in srgb, var(--tag-color) 20%, transparent);
    }
  }
}

.share-section {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-top: $spacing-4;
  padding: $spacing-4;
  background: var(--surface-card);
  border-radius: $radius-xl;
  box-shadow: $shadow-sm;

  .share-label {
    font-size: $text-sm;
    font-weight: 500;
    color: var(--text-color-secondary);
  }

  .share-buttons {
    display: flex;
    gap: $spacing-2;
  }

  .share-btn {
    width: 40px;
    height: 40px;
    display: flex;
    align-items: center;
    justify-content: center;
    border: none;
    border-radius: $radius-lg;
    cursor: pointer;
    transition: all $transition-fast;

    &:hover {
      transform: translateY(-2px);
    }

    &.twitter {
      background: #1da1f2;
      color: white;
    }

    &.facebook {
      background: #4267b2;
      color: white;
    }

    &.linkedin {
      background: #0077b5;
      color: white;
    }

    &.copy {
      background: var(--surface-ground);
      color: var(--text-color);

      &:hover {
        background: var(--surface-100);
      }
    }
  }
}

// ============================================
// SIDEBAR
// ============================================
.article-sidebar {
  position: sticky;
  top: $spacing-6;
  height: fit-content;
  align-self: start;
  max-height: calc(100vh - $header-height - $spacing-12);
  overflow-y: auto;
}

.sidebar-section {
  background: var(--surface-card);
  border-radius: $radius-xl;
  padding: $spacing-5;
  box-shadow: $shadow-sm;
  margin-bottom: $spacing-4;

  .section-title {
    display: flex;
    align-items: center;
    gap: $spacing-2;
    margin: 0 0 $spacing-4 0;
    font-size: $text-base;
    font-weight: 600;
    color: var(--text-color);

    i {
      color: $intalio-teal-500;
    }
  }
}

.related-articles {
  display: flex;
  flex-direction: column;
  gap: $spacing-3;
}

.related-card {
  display: flex;
  gap: $spacing-3;
  padding: $spacing-3;
  background: var(--surface-ground);
  border-radius: $radius-lg;
  cursor: pointer;
  transition: all $transition-fast;

  &:hover {
    background: var(--surface-100);
    transform: translateX(4px);

    .related-title {
      color: $intalio-teal-600;
    }
  }

  .related-image {
    width: 80px;
    height: 60px;
    border-radius: $radius-md;
    overflow: hidden;
    flex-shrink: 0;

    img {
      width: 100%;
      height: 100%;
      object-fit: cover;
    }
  }

  .related-content {
    flex: 1;
    min-width: 0;
  }

  .related-category {
    font-size: $text-xs;
    color: $intalio-teal-600;
    font-weight: 500;
  }

  .related-title {
    margin: $spacing-1 0;
    font-size: $text-sm;
    font-weight: 500;
    color: var(--text-color);
    line-height: 1.3;
    display: -webkit-box;
    -webkit-line-clamp: 2;
    -webkit-box-orient: vertical;
    overflow: hidden;
    transition: color $transition-fast;
  }

  .related-date {
    font-size: $text-xs;
    color: var(--text-color-secondary);
  }
}

.quick-actions {
  display: flex;
  flex-direction: column;
  gap: $spacing-2;
}

.quick-action-btn {
  display: flex;
  align-items: center;
  gap: $spacing-2;
  padding: $spacing-3;
  background: var(--surface-ground);
  border: none;
  border-radius: $radius-lg;
  font-size: $text-sm;
  color: var(--text-color);
  cursor: pointer;
  transition: all $transition-fast;

  i {
    color: var(--text-color-secondary);
  }

  &:hover {
    background: $intalio-teal-50;
    color: $intalio-teal-600;

    i {
      color: $intalio-teal-500;
    }
  }
}

// ============================================
// RESPONSIVE
// ============================================
@media (max-width: 1024px) {
  .article-layout {
    grid-template-columns: 1fr;
  }

  .article-sidebar {
    position: static;
  }
}

@media (max-width: 768px) {
  .article-hero {
    min-height: 360px;
    margin: (-$spacing-4) 0 $spacing-4;

    .hero-content {
      padding: $spacing-6 $spacing-4;
    }

    .article-title {
      font-size: $text-2xl;
    }

    .article-summary {
      font-size: $text-base;
    }

    .hero-top {
      flex-direction: column;
      align-items: flex-start;
      gap: $spacing-3;
    }
  }

  .author-card {
    flex-wrap: wrap;

    .follow-btn {
      width: 100%;
      justify-content: center;
    }
  }

  .article-body {
    padding: $spacing-5;
  }

  .share-section {
    flex-direction: column;
    gap: $spacing-3;
    align-items: flex-start;
  }
}

// ============================================
// ANIMATIONS
// ============================================

// Animated entrance states
.content-detail-view--animated {
  .article-hero,
  .article-main,
  .article-sidebar {
    opacity: 0;
  }

  .article-hero {
    transform: translateY(-20px);
  }

  .article-main,
  .article-sidebar {
    transform: translateY(20px);
  }

  &.content-detail-view--visible {
    .article-hero {
      animation: heroFadeIn 0.6s $ease-out-expo forwards;
    }

    .article-main {
      animation: contentFadeIn 0.5s ease-out 0.2s forwards;
    }

    .article-sidebar {
      animation: contentFadeIn 0.5s ease-out 0.35s forwards;
    }

    .related-card {
      opacity: 0;
      transform: translateX(-10px);
      animation: relatedFadeIn 0.4s ease-out forwards;

      @for $i from 1 through 5 {
        &:nth-child(#{$i}) {
          animation-delay: #{0.4 + ($i * 0.1)}s;
        }
      }
    }
  }
}

@keyframes heroFadeIn {
  from {
    opacity: 0;
    transform: translateY(-20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

@keyframes contentFadeIn {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

@keyframes relatedFadeIn {
  from {
    opacity: 0;
    transform: translateX(-10px);
  }
  to {
    opacity: 1;
    transform: translateX(0);
  }
}

// ============================================
// RTL ANIMATION SUPPORT
// ============================================
.content-detail-view--rtl {
  // Reverse entrance animation direction
  &.content-detail-view--animated .related-card {
    transform: translateX(10px);
  }
}

// ============================================
// REDUCED MOTION
// ============================================
@media (prefers-reduced-motion: reduce) {
  .content-detail-view--animated {
    .article-hero,
    .article-main,
    .article-sidebar,
    .related-card {
      opacity: 1 !important;
      transform: none !important;
      animation: none !important;
    }
  }

  .action-btn:hover,
  .share-btn:hover,
  .related-card:hover {
    transform: none !important;
  }
}
</style>
