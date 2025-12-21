<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import Tag from 'primevue/tag'
import Skeleton from 'primevue/skeleton'
import InputText from 'primevue/inputtext'
import Button from 'primevue/button'
import type { Article } from '@/types'
import { useReducedMotion } from '@/composables/useReducedMotion'
import ErrorState from '@/components/base/ErrorState.vue'
import { PageHeader, StatsBar, EmptyState } from '@/components/base'
import type { BreadcrumbItem } from '@/components/base/PageHeader.vue'
import type { StatItem } from '@/components/base/StatsBar.vue'

const { t, locale } = useI18n()
const router = useRouter()
const prefersReducedMotion = useReducedMotion()

const loading = ref(true)
const error = ref<Error | null>(null)
const articles = ref<Article[]>([])
const search = ref('')
const selectedCategory = ref<string | null>(null)
const selectedType = ref<string | null>(null)
const activeView = ref<'grid' | 'list'>('grid')
const currentPage = ref(1)
const itemsPerPage = ref(6)
const showFilters = ref(false)
const isContentVisible = ref(false)

// Animation control
const shouldAnimate = computed(() => !prefersReducedMotion.value)

// Breadcrumbs for PageHeader
const breadcrumbs = computed<BreadcrumbItem[]>(() => [
  { label: 'Home', labelArabic: 'الرئيسية', to: '/dashboard' },
  { label: t('content.articles') }
])

// Stats for StatsBar
const stats = computed<StatItem[]>(() => [
  {
    icon: 'pi pi-file-edit',
    value: filteredArticles.value.length,
    label: 'Total Articles',
    labelArabic: 'إجمالي المقالات',
    colorClass: 'primary'
  },
  {
    icon: 'pi pi-star-fill',
    value: articles.value.filter(a => a.isFeatured).length,
    label: 'Featured',
    labelArabic: 'مميزة',
    colorClass: 'warning'
  },
  {
    icon: 'pi pi-eye',
    value: formatViewCount(articles.value.reduce((sum, a) => sum + (a.viewCount || 0), 0)),
    label: 'Total Views',
    labelArabic: 'المشاهدات',
    colorClass: 'info'
  }
])

// Category options for filter
const categoryOptions = computed(() => [
  { value: null, label: locale.value === 'ar' ? 'جميع التصنيفات' : 'All Categories' },
  { value: 'tournament-news', label: locale.value === 'ar' ? 'أخبار البطولة' : 'Tournament News' },
  { value: 'announcements', label: locale.value === 'ar' ? 'الإعلانات' : 'Announcements' },
  { value: 'features', label: locale.value === 'ar' ? 'المقالات المميزة' : 'Features' },
  { value: 'behind-the-scenes', label: locale.value === 'ar' ? 'خلف الكواليس' : 'Behind the Scenes' }
])

// Type options for filter
const typeOptions = computed(() => [
  { value: null, label: locale.value === 'ar' ? 'جميع الأنواع' : 'All Types' },
  { value: 'news', label: locale.value === 'ar' ? 'أخبار' : 'News' },
  { value: 'article', label: locale.value === 'ar' ? 'مقال' : 'Article' },
  { value: 'announcement', label: locale.value === 'ar' ? 'إعلان' : 'Announcement' }
])

// Per page options
const perPageOptions = [
  { value: 6, label: '6' },
  { value: 12, label: '12' },
  { value: 24, label: '24' },
  { value: 48, label: '48' }
]

// Count active filters
const activeFiltersCount = computed(() => {
  let count = 0
  if (selectedCategory.value) count++
  if (selectedType.value) count++
  return count
})

// Clear all filters
const clearAllFilters = () => {
  selectedCategory.value = null
  selectedType.value = null
  search.value = ''
  currentPage.value = 1
}

// Reset pagination on filter change
const onFilterChange = () => {
  currentPage.value = 1
}

const filteredArticles = computed(() => {
  let result = articles.value

  if (search.value) {
    const searchLower = search.value.toLowerCase()
    result = result.filter(a =>
      a.title.toLowerCase().includes(searchLower) ||
      (a.titleArabic && a.titleArabic.includes(search.value))
    )
  }

  if (selectedCategory.value) {
    result = result.filter(a => a.categoryName?.toLowerCase().includes(selectedCategory.value!.replace('-', ' ')))
  }

  if (selectedType.value) {
    result = result.filter(a => a.type === selectedType.value)
  }

  return result
})

const paginatedArticles = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage.value
  return filteredArticles.value.slice(start, start + itemsPerPage.value)
})

const totalFilteredPages = computed(() => Math.ceil(filteredArticles.value.length / itemsPerPage.value) || 1)

// Pagination info text
const paginationInfo = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage.value + 1
  const end = Math.min(currentPage.value * itemsPerPage.value, filteredArticles.value.length)
  return { start, end, total: filteredArticles.value.length }
})

const formatDate = (date: string) => {
  return new Date(date).toLocaleDateString(locale.value === 'ar' ? 'ar-SA' : 'en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  })
}

const getRelativeTime = (date: string) => {
  if (!date) return ''
  const now = new Date()
  const published = new Date(date)
  const diffDays = Math.floor((now.getTime() - published.getTime()) / (1000 * 60 * 60 * 24))

  if (diffDays === 0) return locale.value === 'ar' ? 'اليوم' : 'Today'
  if (diffDays === 1) return locale.value === 'ar' ? 'أمس' : 'Yesterday'
  if (diffDays < 7) return locale.value === 'ar' ? `منذ ${diffDays} أيام` : `${diffDays} days ago`
  return formatDate(date)
}

const getTitle = (article: Article) => {
  return locale.value === 'ar' && article.titleArabic ? article.titleArabic : article.title
}

const getSummary = (article: Article) => {
  return locale.value === 'ar' && article.summaryArabic ? article.summaryArabic : article.summary
}

const formatViewCount = (count: number) => {
  if (count >= 1000) {
    return (count / 1000).toFixed(1) + 'K'
  }
  return count.toString()
}

const handleCardClick = (article: Article) => {
  router.push({ name: 'content-view', params: { id: article.slug } })
}

const createArticle = () => {
  router.push({ name: 'content-create' })
}

const getTypeSeverity = (type: string | undefined): "success" | "info" | "warn" | "danger" | "secondary" | "contrast" | undefined => {
  if (!type) return 'secondary'
  const severities: Record<string, "success" | "info" | "warn" | "danger" | "secondary" | "contrast"> = {
    'news': 'info',
    'article': 'success',
    'announcement': 'warn'
  }
  return severities[type] || 'secondary'
}

// Color utilities - reserved for future category badges
const _getCategoryColor = (category: string | null): string => {
  if (!category) return 'linear-gradient(135deg, #94a3b8 0%, #64748b 100%)'
  const colors: Record<string, string> = {
    'tournament-news': 'linear-gradient(135deg, #00c9a7 0%, #00ae8d 100%)',
    'announcements': 'linear-gradient(135deg, #fbbf24 0%, #f59e0b 100%)',
    'features': 'linear-gradient(135deg, #a78bfa 0%, #8b5cf6 100%)',
    'behind-the-scenes': 'linear-gradient(135deg, #f472b6 0%, #ec4899 100%)'
  }
  return colors[category] || 'linear-gradient(135deg, #94a3b8 0%, #64748b 100%)'
}

const _getTypeColor = (type: string | null): string => {
  if (!type) return 'linear-gradient(135deg, #94a3b8 0%, #64748b 100%)'
  const colors: Record<string, string> = {
    'news': 'linear-gradient(135deg, #60a5fa 0%, #3b82f6 100%)',
    'article': 'linear-gradient(135deg, #34d399 0%, #10b981 100%)',
    'announcement': 'linear-gradient(135deg, #fbbf24 0%, #f59e0b 100%)'
  }
  return colors[type] || 'linear-gradient(135deg, #94a3b8 0%, #64748b 100%)'
}

// Suppress unused variable warnings
void _getCategoryColor
void _getTypeColor

const goToPage = (page: number) => {
  if (page >= 1 && page <= totalFilteredPages.value) {
    currentPage.value = page
  }
}

const getVisiblePages = computed(() => {
  const pages: (number | string)[] = []
  const total = totalFilteredPages.value

  if (total <= 5) {
    for (let i = 1; i <= total; i++) pages.push(i)
  } else {
    pages.push(1)
    if (currentPage.value > 3) pages.push('...')

    const start = Math.max(2, currentPage.value - 1)
    const end = Math.min(total - 1, currentPage.value + 1)

    for (let i = start; i <= end; i++) pages.push(i)

    if (currentPage.value < total - 2) pages.push('...')
    pages.push(total)
  }

  return pages
})

// Error handling
async function handleRetry() {
  error.value = null
  loading.value = true
  await new Promise(resolve => setTimeout(resolve, 800))
  loading.value = false
  if (shouldAnimate.value) {
    requestAnimationFrame(() => {
      isContentVisible.value = true
    })
  } else {
    isContentVisible.value = true
  }
}

onMounted(async () => {
  await new Promise(resolve => setTimeout(resolve, 800))

  // Trigger entrance animations
  if (shouldAnimate.value) {
    requestAnimationFrame(() => {
      isContentVisible.value = true
    })
  } else {
    isContentVisible.value = true
  }

  articles.value = [
    {
      id: '1',
      title: 'AFC Asian Cup 2027 Venues Announced',
      titleArabic: 'الإعلان عن ملاعب كأس آسيا 2027',
      summary: 'Saudi Arabia unveils world-class stadiums for the tournament, featuring state-of-the-art facilities and cutting-edge technology.',
      summaryArabic: 'المملكة العربية السعودية تكشف عن ملاعب عالمية المستوى للبطولة',
      slug: 'afc-asian-cup-2027-venues-announced',
      type: 'news',
      status: 'published',
      thumbnailUrl: 'https://images.unsplash.com/photo-1522778119026-d647f0596c20?w=800&h=600&fit=crop',
      authorName: 'Mohammed Al-Rashid',
      categoryName: 'Tournament News',
      isFeatured: true,
      viewCount: 15420,
      commentCount: 45,
      publishedAt: '2024-12-01T10:00:00Z',
      tags: [
        { id: '1', name: 'Venues', nameArabic: 'الملاعب', slug: 'venues', color: '#00ae8d' },
        { id: '2', name: 'Tournament', nameArabic: 'البطولة', slug: 'tournament', color: '#808080' }
      ]
    },
    {
      id: '2',
      title: 'Volunteer Registration Opens',
      titleArabic: 'فتح باب التسجيل للمتطوعين',
      summary: 'Join the team and be part of history. Registration is now open for volunteers across all regions.',
      summaryArabic: 'انضم إلى الفريق وكن جزءاً من التاريخ',
      slug: 'volunteer-registration-opens',
      type: 'announcement',
      status: 'published',
      thumbnailUrl: 'https://images.unsplash.com/photo-1559027615-cd4628902d4a?w=800&h=600&fit=crop',
      authorName: 'Sara Ali',
      categoryName: 'Announcements',
      isFeatured: true,
      viewCount: 8500,
      commentCount: 120,
      publishedAt: '2024-11-28T14:30:00Z',
      tags: [
        { id: '3', name: 'Volunteers', nameArabic: 'المتطوعين', slug: 'volunteers', color: '#1976D2' }
      ]
    },
    {
      id: '3',
      title: 'Behind the Scenes: Stadium Construction',
      titleArabic: 'خلف الكواليس: بناء الملاعب',
      summary: 'An exclusive look at the making of world-class venues for the tournament with unprecedented access.',
      summaryArabic: 'نظرة حصرية على بناء الملاعب العالمية للبطولة',
      slug: 'behind-the-scenes-stadium-construction',
      type: 'article',
      status: 'published',
      thumbnailUrl: 'https://images.unsplash.com/photo-1489944440615-453fc2b6a9a9?w=800&h=600&fit=crop',
      authorName: 'Ahmed Hassan',
      categoryName: 'Features',
      isFeatured: false,
      viewCount: 3200,
      commentCount: 28,
      publishedAt: '2024-11-25T09:00:00Z',
      tags: []
    },
    {
      id: '4',
      title: 'Ticketing System Launch Announcement',
      titleArabic: 'إعلان إطلاق نظام التذاكر',
      summary: 'Get ready for ticket sales! The official ticketing platform launches next month with exclusive early access.',
      summaryArabic: 'استعد لمبيعات التذاكر! منصة التذاكر الرسمية تنطلق الشهر القادم',
      slug: 'ticketing-system-launch',
      type: 'announcement',
      status: 'published',
      thumbnailUrl: 'https://images.unsplash.com/photo-1540747913346-19e32dc3e97e?w=800&h=600&fit=crop',
      authorName: 'Fatima Al-Zahrani',
      categoryName: 'Announcements',
      isFeatured: false,
      viewCount: 12300,
      commentCount: 89,
      publishedAt: '2024-11-20T08:00:00Z',
      tags: [
        { id: '4', name: 'Tickets', nameArabic: 'التذاكر', slug: 'tickets', color: '#9C27B0' }
      ]
    },
    {
      id: '5',
      title: 'Teams Qualification Path Revealed',
      titleArabic: 'الكشف عن مسار تأهل المنتخبات',
      summary: 'Full breakdown of qualification rounds and participating teams in the AFC Asian Cup 2027.',
      summaryArabic: 'تفاصيل كاملة عن جولات التأهل والمنتخبات المشاركة',
      slug: 'teams-qualification-path',
      type: 'news',
      status: 'published',
      thumbnailUrl: 'https://images.unsplash.com/photo-1574629810360-7efbbe195018?w=800&h=600&fit=crop',
      authorName: 'Khalid Omar',
      categoryName: 'Tournament News',
      isFeatured: false,
      viewCount: 9800,
      commentCount: 156,
      publishedAt: '2024-11-15T12:00:00Z',
      tags: [
        { id: '5', name: 'Teams', nameArabic: 'الفرق', slug: 'teams', color: '#FF5722' }
      ]
    },
    {
      id: '6',
      title: 'Sustainability Initiatives for the Tournament',
      titleArabic: 'مبادرات الاستدامة للبطولة',
      summary: 'How the AFC Asian Cup 2027 is setting new standards for environmentally conscious sporting events.',
      summaryArabic: 'كيف تضع كأس آسيا 2027 معايير جديدة للأحداث الرياضية الصديقة للبيئة',
      slug: 'sustainability-initiatives',
      type: 'article',
      status: 'published',
      thumbnailUrl: 'https://images.unsplash.com/photo-1518531933037-91b2f5f229cc?w=800&h=600&fit=crop',
      authorName: 'Nora Abdullah',
      categoryName: 'Features',
      isFeatured: false,
      viewCount: 4500,
      commentCount: 34,
      publishedAt: '2024-11-10T16:00:00Z',
      tags: [
        { id: '6', name: 'Sustainability', nameArabic: 'الاستدامة', slug: 'sustainability', color: '#4CAF50' }
      ]
    },
    {
      id: '7',
      title: 'Security Measures and Fan Safety',
      titleArabic: 'التدابير الأمنية وسلامة المشجعين',
      summary: 'Comprehensive security protocols to ensure a safe and enjoyable experience for all attendees.',
      summaryArabic: 'بروتوكولات أمنية شاملة لضمان تجربة آمنة وممتعة لجميع الحضور',
      slug: 'security-measures-fan-safety',
      type: 'news',
      status: 'published',
      thumbnailUrl: 'https://images.unsplash.com/photo-1516321497487-e288fb19713f?w=800&h=600&fit=crop',
      authorName: 'Omar Sayed',
      categoryName: 'Tournament News',
      isFeatured: false,
      viewCount: 6700,
      commentCount: 42,
      publishedAt: '2024-11-05T11:00:00Z',
      tags: []
    },
    {
      id: '8',
      title: 'Media Accreditation Process',
      titleArabic: 'عملية اعتماد وسائل الإعلام',
      summary: 'Guidelines and requirements for media personnel covering the AFC Asian Cup 2027.',
      summaryArabic: 'إرشادات ومتطلبات لموظفي وسائل الإعلام الذين يغطون كأس آسيا 2027',
      slug: 'media-accreditation-process',
      type: 'announcement',
      status: 'published',
      thumbnailUrl: 'https://images.unsplash.com/photo-1504711434969-e33886168f5c?w=800&h=600&fit=crop',
      authorName: 'Layla Hassan',
      categoryName: 'Announcements',
      isFeatured: false,
      viewCount: 3100,
      commentCount: 18,
      publishedAt: '2024-11-01T09:00:00Z',
      tags: []
    }
  ]

  loading.value = false
})
</script>

<template>
  <div class="content-list-view">
    <!-- Page Header -->
    <PageHeader
      :title="t('content.articles')"
      :title-arabic="locale === 'ar' ? 'المقالات' : undefined"
      :description="t('content.browseArticles')"
      :description-arabic="locale === 'ar' ? 'تصفح واستكشف المقالات' : undefined"
      :breadcrumbs="breadcrumbs"
      padding-bottom="xl"
      background-variant="gradient"
      :animated="shouldAnimate"
    >
      <template #actions>
        <Button
          :label="t('content.createArticle')"
          icon="pi pi-plus"
          class="p-button-raised"
          @click="createArticle"
        />
      </template>
    </PageHeader>

    <!-- Stats Bar -->
    <StatsBar
      :stats="stats"
      :loading="loading"
      overlap="lg"
      :animated="shouldAnimate"
      :animate-numbers="shouldAnimate"
      :counter-duration="1200"
    />

    <!-- Premium Toolbar -->
    <div class="toolbar">
      <div class="toolbar-row">
        <div class="toolbar-left">
          <!-- Search -->
          <div class="search-box">
            <i class="pi pi-search"></i>
            <InputText
              v-model="search"
              :placeholder="locale === 'ar' ? 'البحث في المقالات...' : 'Search articles...'"
              class="search-input"
              @input="onFilterChange"
            />
          </div>

          <!-- Filter Button -->
          <button
            class="filter-btn"
            :class="{ active: showFilters, 'has-filters': activeFiltersCount > 0 }"
            @click="showFilters = !showFilters"
          >
            <i class="pi pi-filter"></i>
            <span>{{ locale === 'ar' ? 'تصفية' : 'Filters' }}</span>
            <span v-if="activeFiltersCount > 0" class="filter-badge">{{ activeFiltersCount }}</span>
          </button>
        </div>

        <div class="toolbar-right">
          <!-- Items Per Page -->
          <div class="per-page-control">
            <span class="per-page-label">{{ locale === 'ar' ? 'عرض' : 'Show' }}</span>
            <select
              v-model="itemsPerPage"
              @change="onFilterChange"
              class="per-page-select"
            >
              <option v-for="option in perPageOptions" :key="option.value" :value="option.value">
                {{ option.label }}
              </option>
            </select>
            <span class="per-page-label">{{ locale === 'ar' ? 'عنصر' : 'items' }}</span>
          </div>

          <!-- View Toggle -->
          <div class="view-toggle">
            <button
              :class="{ active: activeView === 'grid' }"
              @click="activeView = 'grid'"
              :aria-label="locale === 'ar' ? 'عرض شبكي' : 'Grid view'"
            >
              <i class="pi pi-th-large"></i>
            </button>
            <button
              :class="{ active: activeView === 'list' }"
              @click="activeView = 'list'"
              :aria-label="locale === 'ar' ? 'عرض قائمة' : 'List view'"
            >
              <i class="pi pi-list"></i>
            </button>
          </div>
        </div>
      </div>

      <!-- Expanded Filters Panel -->
      <Transition name="filter-panel">
        <div v-if="showFilters" class="filters-panel">
        <div class="filter-group">
          <label class="filter-label">{{ locale === 'ar' ? 'التصنيف' : 'Category' }}</label>
          <div class="filter-options">
            <button
              v-for="option in categoryOptions"
              :key="option.value ?? 'all'"
              :class="['filter-chip', { active: selectedCategory === option.value }]"
              @click="selectedCategory = option.value; onFilterChange()"
            >
              {{ option.label }}
            </button>
          </div>
        </div>

        <div class="filter-group">
          <label class="filter-label">{{ locale === 'ar' ? 'النوع' : 'Type' }}</label>
          <div class="filter-options">
            <button
              v-for="option in typeOptions"
              :key="option.value ?? 'all'"
              :class="['filter-chip', { active: selectedType === option.value }]"
              @click="selectedType = option.value; onFilterChange()"
            >
              {{ option.label }}
            </button>
          </div>
        </div>

        <div class="filter-actions" v-if="activeFiltersCount > 0 || search">
          <button class="clear-filters-btn" @click="clearAllFilters">
            <i class="pi pi-times"></i>
            {{ locale === 'ar' ? 'مسح الكل' : 'Clear All' }}
          </button>
        </div>
        </div>
      </Transition>

      <!-- Active Filters Tags -->
      <div v-if="(activeFiltersCount > 0 || search) && !showFilters" class="active-filters">
        <span class="active-filters-label">{{ locale === 'ar' ? 'التصفية النشطة:' : 'Active filters:' }}</span>

        <span v-if="search" class="filter-tag">
          <i class="pi pi-search"></i>
          "{{ search }}"
          <button @click="search = ''; onFilterChange()"><i class="pi pi-times"></i></button>
        </span>

        <span v-if="selectedCategory" class="filter-tag">
          {{ categoryOptions.find(o => o.value === selectedCategory)?.label }}
          <button @click="selectedCategory = null; onFilterChange()"><i class="pi pi-times"></i></button>
        </span>

        <span v-if="selectedType" class="filter-tag">
          {{ typeOptions.find(o => o.value === selectedType)?.label }}
          <button @click="selectedType = null; onFilterChange()"><i class="pi pi-times"></i></button>
        </span>

        <button class="clear-all-link" @click="clearAllFilters">
          {{ locale === 'ar' ? 'مسح الكل' : 'Clear all' }}
        </button>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="skeleton-grid">
      <div v-for="i in 6" :key="i" class="skeleton-card" :style="{ animationDelay: `${i * 0.1}s` }">
        <Skeleton height="200px" class="skeleton-image" />
        <div class="skeleton-content">
          <Skeleton width="30%" height="20px" class="mb-2" />
          <Skeleton width="90%" height="24px" class="mb-2" />
          <Skeleton width="100%" height="16px" class="mb-1" />
          <Skeleton width="70%" height="16px" class="mb-3" />
          <div class="skeleton-footer">
            <Skeleton width="40%" height="16px" />
            <Skeleton width="25%" height="16px" />
          </div>
        </div>
      </div>
    </div>

    <!-- Error State -->
    <ErrorState
      v-else-if="error"
      :error="error"
      :title="locale === 'ar' ? 'فشل تحميل المحتوى' : 'Failed to load content'"
      show-retry
      @retry="handleRetry"
    />

    <!-- Empty State -->
    <EmptyState
      v-else-if="filteredArticles.length === 0"
      icon="pi-search"
      title="No Results Found"
      title-arabic="لا توجد نتائج"
      description="Try adjusting your search or filter criteria"
      description-arabic="جرب تغيير معايير البحث أو الفلاتر"
      action-label="Reset Filters"
      action-label-arabic="إعادة تعيين الفلاتر"
      action-icon="pi-refresh"
      variant="search"
      @action="search = ''; selectedCategory = null; selectedType = null"
    />

    <!-- Articles Grid -->
    <div
      v-else
      class="articles-container"
      :class="{
        'list-view': activeView === 'list',
        'articles-container--animated': shouldAnimate,
        'articles-container--visible': isContentVisible
      }"
    >
      <article
        v-for="(article, index) in paginatedArticles"
        :key="article.id"
        class="article-card"
        :class="{ featured: article.isFeatured }"
        :style="shouldAnimate ? { '--card-index': index } : undefined"
        @click="handleCardClick(article)"
        role="button"
        tabindex="0"
        @keydown.enter="handleCardClick(article)"
      >
        <!-- Card Image -->
        <div class="card-media">
          <img
            v-if="article.thumbnailUrl"
            :src="article.thumbnailUrl"
            :alt="getTitle(article)"
            class="card-image"
            loading="lazy"
          />
          <div v-else class="placeholder-image">
            <i class="pi pi-image"></i>
          </div>

          <!-- Overlay badges -->
          <div class="card-badges">
            <span v-if="article.isFeatured" class="badge featured-badge">
              <i class="pi pi-star-fill"></i>
              {{ locale === 'ar' ? 'مميز' : 'Featured' }}
            </span>
            <Tag
              :value="article.type || 'article'"
              :severity="getTypeSeverity(article.type)"
              class="type-badge"
            />
          </div>

          <!-- Hover overlay -->
          <div class="card-overlay">
            <span class="read-more">
              <i class="pi pi-arrow-right"></i>
              {{ locale === 'ar' ? 'قراءة المزيد' : 'Read More' }}
            </span>
          </div>
        </div>

        <!-- Card Content -->
        <div class="card-body">
          <div class="card-meta">
            <span class="category">
              <i class="pi pi-folder"></i>
              {{ article.categoryName }}
            </span>
            <span class="publish-date">{{ getRelativeTime(article.publishedAt || '') }}</span>
          </div>

          <h3 class="card-title">{{ getTitle(article) }}</h3>
          <p class="card-excerpt">{{ getSummary(article) }}</p>

          <!-- Tags -->
          <div v-if="article.tags?.length" class="card-tags">
            <span
              v-for="tag in article.tags.slice(0, 3)"
              :key="tag.id"
              class="tag"
              :style="{ '--tag-color': tag.color }"
            >
              {{ locale === 'ar' && tag.nameArabic ? tag.nameArabic : tag.name }}
            </span>
          </div>

          <!-- Footer -->
          <div class="card-footer">
            <div class="author-info">
              <div class="author-avatar">
                <i class="pi pi-user"></i>
              </div>
              <span class="author-name">{{ article.authorName }}</span>
            </div>
            <div class="engagement-stats">
              <span class="stat">
                <i class="pi pi-eye"></i>
                {{ formatViewCount(article.viewCount || 0) }}
              </span>
              <span class="stat">
                <i class="pi pi-comments"></i>
                {{ article.commentCount }}
              </span>
            </div>
          </div>
        </div>
      </article>
    </div>

    <!-- Pagination -->
    <div v-if="!loading && filteredArticles.length > 0 && totalFilteredPages > 1" class="pagination-wrapper">
      <div class="pagination-info">
        <span>
          {{ locale === 'ar' ? `عرض ${paginationInfo.start} إلى ${paginationInfo.end} من ${paginationInfo.total} عنصر` : `Showing ${paginationInfo.start} to ${paginationInfo.end} of ${paginationInfo.total} items` }}
        </span>
      </div>

      <div class="pagination-controls">
        <button
          class="page-btn nav-btn"
          :disabled="currentPage === 1"
          @click="goToPage(currentPage - 1)"
          :aria-label="locale === 'ar' ? 'الصفحة السابقة' : 'Previous page'"
        >
          <i :class="locale === 'ar' ? 'pi pi-angle-right' : 'pi pi-angle-left'"></i>
        </button>

        <template v-for="page in getVisiblePages" :key="page">
          <span v-if="page === '...'" class="page-ellipsis">...</span>
          <button
            v-else
            class="page-btn"
            :class="{ active: currentPage === page }"
            @click="goToPage(page as number)"
          >
            {{ page }}
          </button>
        </template>

        <button
          class="page-btn nav-btn"
          :disabled="currentPage === totalFilteredPages"
          @click="goToPage(currentPage + 1)"
          :aria-label="locale === 'ar' ? 'الصفحة التالية' : 'Next page'"
        >
          <i :class="locale === 'ar' ? 'pi pi-angle-left' : 'pi pi-angle-right'"></i>
        </button>
      </div>
    </div>
  </div>
</template>

<style scoped lang="scss">
@use '@/assets/styles/_variables.scss' as *;
@use '@/assets/styles/_mixins.scss' as *;

// ============================================
// PAGE CONTAINER - Uses standard mixin
// ============================================
.content-list-view {
  @include page-view;
}

// ============================================
// PREMIUM TOOLBAR
// ============================================
.toolbar {
  @include toolbar-container;
}

.toolbar-row {
  @include toolbar-row;
}

.toolbar-left {
  @include toolbar-section-left;
}

.toolbar-right {
  @include toolbar-section-right;
}

.search-box {
  @include toolbar-search(280px);
}

.search-input {
  @include toolbar-search-input;
}

.filter-btn {
  @include filter-btn;

  .filter-badge {
    @include filter-badge;
  }
}

.per-page-control {
  @include per-page-control;
}

.per-page-label {
  @include per-page-label;
}

.per-page-select {
  @include per-page-select;
}

.view-toggle {
  @include premium-view-toggle;
}

// Filters Panel
.filters-panel {
  @include filters-panel;
}

.filter-group {
  @include filters-panel-group;
}

.filter-label {
  @include filters-panel-label;
}

.filter-options {
  @include filters-panel-options;
}

.filter-chip {
  @include filter-chip;
}

.filter-actions {
  display: flex;
  align-items: flex-end;
  margin-inline-start: auto;
}

.clear-filters-btn {
  @include clear-filters-btn;
}

// Active Filters Row
.active-filters {
  @include active-filters-row;
}

.active-filters-label {
  @include active-filters-label;
}

.filter-tag {
  @include filter-tag;
}

.clear-all-link {
  @include clear-all-link;
}

// ============================================
// SKELETON LOADING
// ============================================
.skeleton-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(340px, 1fr));
  gap: $spacing-6;

  .skeleton-card {
    background: var(--surface-card);
    border-radius: $radius-xl;
    overflow: hidden;
    animation: pulse 1.5s ease-in-out infinite;

    .skeleton-image {
      border-radius: 0;
    }

    .skeleton-content {
      padding: $spacing-5;
    }

    .skeleton-footer {
      display: flex;
      justify-content: space-between;
      margin-top: $spacing-4;
      padding-top: $spacing-4;
      border-top: 1px solid var(--surface-border);
    }
  }
}

// ============================================
// ARTICLES GRID
// ============================================
.articles-container {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(340px, 1fr));
  gap: $spacing-6;

  &.list-view {
    grid-template-columns: 1fr;

    .article-card {
      flex-direction: row;

      .card-media {
        width: 280px;
        min-width: 280px;
        height: 200px;
      }

      .card-body {
        flex: 1;
      }
    }
  }
}

.article-card {
  display: flex;
  flex-direction: column;
  background: var(--surface-card);
  border-radius: $radius-xl;
  overflow: hidden;
  cursor: pointer;
  transition: all $transition-normal $ease-out;
  animation: fadeInUp 0.4s ease-out both;
  box-shadow: $shadow-sm;
  border: 1px solid var(--surface-border);

  &:hover {
    transform: translateY(-6px);
    box-shadow: $shadow-elevated-lg;
    border-color: $intalio-teal-200;

    .card-image {
      transform: scale(1.05);
    }

    .card-overlay {
      opacity: 1;
    }

    .card-title {
      color: $intalio-teal-600;
    }
  }

  &:focus {
    outline: none;
    box-shadow: 0 0 0 3px rgba($intalio-teal-500, 0.3);
  }

  &.featured {
    border: 2px solid $intalio-teal-200;
    background: linear-gradient(135deg, var(--surface-card) 0%, rgba($intalio-teal-50, 0.3) 100%);

    &:hover {
      border-color: $intalio-teal-400;
    }
  }
}

// Card Media
.card-media {
  position: relative;
  height: 220px;
  overflow: hidden;
  background: var(--surface-ground);

  .card-image {
    width: 100%;
    height: 100%;
    object-fit: cover;
    transition: transform 0.5s $ease-out;
  }

  .placeholder-image {
    width: 100%;
    height: 100%;
    display: flex;
    align-items: center;
    justify-content: center;
    background: linear-gradient(135deg, var(--surface-ground) 0%, var(--surface-100) 100%);

    i {
      font-size: 3rem;
      color: var(--text-color-secondary);
      opacity: 0.5;
    }
  }

  .card-badges {
    position: absolute;
    top: $spacing-3;
    inset-inline-start: $spacing-3;
    display: flex;
    flex-direction: column;
    gap: $spacing-2;
    z-index: 2;
  }

  .featured-badge {
    display: inline-flex;
    align-items: center;
    gap: $spacing-1;
    padding: $spacing-1 $spacing-3;
    background: linear-gradient(135deg, #fbbf24 0%, #f59e0b 100%);
    color: white;
    font-size: $text-xs;
    font-weight: 600;
    border-radius: $radius-full;
    box-shadow: 0 2px 8px rgba(245, 158, 11, 0.4);

    i {
      font-size: 0.625rem;
    }
  }

  .type-badge {
    font-size: $text-xs;
    padding: $spacing-1 $spacing-2;
  }

  .card-overlay {
    position: absolute;
    inset: 0;
    background: linear-gradient(to top, rgba(0, 0, 0, 0.7) 0%, transparent 50%);
    display: flex;
    align-items: flex-end;
    justify-content: center;
    padding: $spacing-4;
    opacity: 0;
    transition: opacity $transition-normal;

    .read-more {
      display: flex;
      align-items: center;
      gap: $spacing-2;
      color: white;
      font-size: $text-sm;
      font-weight: 500;

      i {
        transition: transform $transition-fast;
      }
    }

    &:hover .read-more i {
      transform: translateX(4px);
    }
  }
}

// Card Body
.card-body {
  padding: $spacing-5;
  display: flex;
  flex-direction: column;
  gap: $spacing-3;
  flex: 1;
}

.card-meta {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-size: $text-xs;

  .category {
    display: flex;
    align-items: center;
    gap: $spacing-1;
    color: $intalio-teal-600;
    font-weight: 500;

    i {
      font-size: 0.75rem;
    }
  }

  .publish-date {
    color: var(--text-color-secondary);
  }
}

.card-title {
  margin: 0;
  font-size: $text-lg;
  font-weight: 600;
  line-height: 1.4;
  color: var(--text-color);
  transition: color $transition-fast;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.card-excerpt {
  margin: 0;
  font-size: $text-sm;
  line-height: 1.6;
  color: var(--text-color-secondary);
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.card-tags {
  display: flex;
  flex-wrap: wrap;
  gap: $spacing-2;

  .tag {
    padding: $spacing-1 $spacing-2;
    background: color-mix(in srgb, var(--tag-color) 12%, transparent);
    color: var(--tag-color);
    font-size: $text-xs;
    font-weight: 500;
    border-radius: $radius-md;
  }
}

.card-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: auto;
  padding-top: $spacing-4;
  border-top: 1px solid var(--surface-border);
}

.author-info {
  display: flex;
  align-items: center;
  gap: $spacing-2;

  .author-avatar {
    width: 28px;
    height: 28px;
    display: flex;
    align-items: center;
    justify-content: center;
    background: $intalio-teal-100;
    border-radius: $radius-full;
    color: $intalio-teal-600;

    i {
      font-size: $text-xs;
    }
  }

  .author-name {
    font-size: $text-sm;
    color: var(--text-color-secondary);
  }
}

.engagement-stats {
  display: flex;
  gap: $spacing-3;

  .stat {
    display: flex;
    align-items: center;
    gap: $spacing-1;
    font-size: $text-xs;
    color: var(--text-color-secondary);

    i {
      font-size: 0.75rem;
      color: var(--text-color-secondary);
    }
  }
}

// ============================================
// PAGINATION
// ============================================
.pagination-wrapper {
  @include pagination-wrapper;
}

.pagination-info {
  @include pagination-info;
}

.pagination-controls {
  @include pagination-controls;
}

.page-btn {
  @include pagination-btn;
}

.page-ellipsis {
  @include pagination-ellipsis;
}

// ============================================
// RESPONSIVE
// ============================================
@media (max-width: $breakpoint-lg) {
  .toolbar-row {
    flex-wrap: wrap;
  }

  .toolbar-left {
    order: 1;
    flex-basis: 100%;
  }

  .toolbar-right {
    order: 2;
    width: 100%;
    justify-content: space-between;
    margin-top: $spacing-2;
  }
}

@media (max-width: $breakpoint-md) {
  .toolbar-row {
    flex-direction: column;
    align-items: stretch;
    gap: $spacing-3;
  }

  .toolbar-left {
    flex-direction: column;
    align-items: stretch;
    gap: $spacing-3;
  }

  .toolbar-right {
    flex-direction: row;
    justify-content: space-between;
  }

  .search-box {
    width: 100%;
  }

  .filter-btn {
    width: 100%;
    justify-content: center;
  }

  .per-page-control {
    flex: 1;
  }

  .filters-panel {
    flex-direction: column;
  }

  .filter-group {
    width: 100%;
  }

  .articles-container {
    grid-template-columns: 1fr;

    &.list-view .article-card {
      flex-direction: column;

      .card-media {
        width: 100%;
        height: 200px;
      }
    }
  }

  .pagination-wrapper {
    flex-direction: column;
    gap: $spacing-3;
    padding: $spacing-4;
  }

  .pagination-info {
    order: 2;
    text-align: center;
  }

  .pagination-controls {
    order: 1;
    width: 100%;
    justify-content: center;
  }

  .page-btn {
    min-width: 36px;
    height: 36px;
  }
}

// ============================================
// ANIMATIONS
// ============================================
@keyframes fadeIn {
  from { opacity: 0; }
  to { opacity: 1; }
}

@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

@keyframes pulse {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.5; }
}

@keyframes cardStaggerFadeIn {
  from {
    opacity: 0;
    transform: translateY(24px) scale(0.96);
  }
  to {
    opacity: 1;
    transform: translateY(0) scale(1);
  }
}

// ============================================
// FILTER PANEL TRANSITION
// ============================================
.filter-panel-enter-active {
  animation: filterPanelSlideDown 0.3s ease-out forwards;
}

.filter-panel-leave-active {
  animation: filterPanelSlideUp 0.25s ease-in forwards;
}

@keyframes filterPanelSlideDown {
  from {
    opacity: 0;
    transform: translateY(-12px);
    max-height: 0;
  }
  to {
    opacity: 1;
    transform: translateY(0);
    max-height: 300px;
  }
}

@keyframes filterPanelSlideUp {
  from {
    opacity: 1;
    transform: translateY(0);
    max-height: 300px;
  }
  to {
    opacity: 0;
    transform: translateY(-12px);
    max-height: 0;
  }
}

// ============================================
// ARTICLES CONTAINER STAGGER ANIMATION
// ============================================
.articles-container {
  // Animation state - cards hidden initially
  &--animated {
    .article-card {
      opacity: 0;
      transform: translateY(24px) scale(0.96);
    }

    // When visible, animate cards in with stagger
    &.articles-container--visible {
      .article-card {
        animation: cardStaggerFadeIn 0.5s ease-out forwards;
        animation-delay: calc(var(--card-index, 0) * 80ms);
      }
    }
  }

  // Ensure cards without animation are visible
  &:not(.articles-container--animated) {
    .article-card {
      opacity: 1;
      transform: none;
      animation: none;
    }
  }
}

// ============================================
// RTL ANIMATION SUPPORT
// ============================================
.rtl {
  // Reverse hover animation direction for read-more icon
  .article-card:hover .read-more i {
    transform: translateX(-4px);
  }
}

// ============================================
// REDUCED MOTION SUPPORT
// ============================================
@media (prefers-reduced-motion: reduce) {
  .articles-container {
    &--animated {
      .article-card {
        opacity: 1;
        transform: none;
        animation: none !important;
      }
    }
  }

  .filter-panel-enter-active,
  .filter-panel-leave-active {
    animation: none !important;
  }

  .filters-panel {
    transition: none !important;
  }

  .article-card {
    animation: none !important;
    transition: background-color $transition-fast, box-shadow $transition-fast;

    &:hover {
      transform: none;
    }
  }

  .skeleton-card {
    animation: none !important;
  }
}
</style>
