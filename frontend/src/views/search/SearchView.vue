<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import InputText from 'primevue/inputtext'
import Button from 'primevue/button'
import TabView from 'primevue/tabview'
import TabPanel from 'primevue/tabpanel'
import Checkbox from 'primevue/checkbox'
import Tag from 'primevue/tag'
import Skeleton from 'primevue/skeleton'

const { t, locale } = useI18n()
const route = useRoute()
const router = useRouter()

const searchQuery = ref((route.query.q as string) || '')
const loading = ref(false)
const activeTab = ref(0)

// Filters
const selectedTypes = ref<string[]>([])
const selectedDateRange = ref<string | null>(null)

// Results
const results = ref<{
  articles: any[]
  documents: any[]
  users: any[]
  total: number
}>({
  articles: [],
  documents: [],
  users: [],
  total: 0
})

const typeFilters = [
  { label: 'Articles', labelAr: 'المقالات', value: 'article' },
  { label: 'Documents', labelAr: 'المستندات', value: 'document' },
  { label: 'Users', labelAr: 'المستخدمين', value: 'user' }
]

const dateFilters = [
  { label: 'Any time', labelAr: 'أي وقت', value: null },
  { label: 'Past 24 hours', labelAr: 'آخر 24 ساعة', value: '24h' },
  { label: 'Past week', labelAr: 'الأسبوع الماضي', value: '7d' },
  { label: 'Past month', labelAr: 'الشهر الماضي', value: '30d' },
  { label: 'Past year', labelAr: 'السنة الماضية', value: '1y' }
]

const localizedTypeFilters = computed(() =>
  typeFilters.map(f => ({
    ...f,
    label: locale.value === 'ar' ? f.labelAr : f.label
  }))
)

const localizedDateFilters = computed(() =>
  dateFilters.map(f => ({
    ...f,
    label: locale.value === 'ar' ? f.labelAr : f.label
  }))
)

const search = async () => {
  if (!searchQuery.value.trim()) return

  loading.value = true
  router.push({ query: { q: searchQuery.value } })

  // Simulated search
  await new Promise(resolve => setTimeout(resolve, 800))

  results.value = {
    articles: [
      {
        id: '1',
        title: 'AFC Asian Cup 2027 Venues Announced',
        titleArabic: 'الإعلان عن ملاعب كأس آسيا 2027',
        summary: 'Saudi Arabia unveils world-class stadiums for the tournament',
        summaryArabic: 'المملكة العربية السعودية تكشف عن ملاعب عالمية',
        type: 'news',
        slug: 'afc-venues-announced',
        publishedAt: '2024-12-01',
        authorName: 'Mohammed Al-Rashid',
        highlightedContent: `...the <mark>${searchQuery.value}</mark> will host matches across multiple world-class stadiums...`
      },
      {
        id: '2',
        title: 'Tournament Operations Update',
        titleArabic: 'تحديث عمليات البطولة',
        summary: 'Latest updates on tournament preparations',
        type: 'article',
        slug: 'tournament-operations-update',
        publishedAt: '2024-11-28',
        authorName: 'Sara Ali',
        highlightedContent: `...preparations for the <mark>${searchQuery.value}</mark> are progressing well...`
      }
    ],
    documents: [
      {
        id: '1',
        name: 'Tournament Operations Manual',
        nameArabic: 'دليل عمليات البطولة',
        fileName: 'tournament-operations-manual.pdf',
        fileExtension: '.pdf',
        fileSize: 5242880,
        libraryName: 'Operations',
        version: '2.1',
        updatedAt: '2024-12-01',
        highlightedContent: `...section about <mark>${searchQuery.value}</mark> procedures...`
      }
    ],
    users: [
      {
        id: '1',
        displayName: 'Mohammed Al-Rashid',
        displayNameArabic: 'محمد الراشد',
        email: 'mohammed.alrashid@intalio.com',
        jobTitle: 'Operations Director',
        department: 'Operations'
      }
    ],
    total: 4
  }

  loading.value = false
}

const getTitle = (item: any) => {
  if (item.title) {
    return locale.value === 'ar' && item.titleArabic ? item.titleArabic : item.title
  }
  return locale.value === 'ar' && item.nameArabic ? item.nameArabic : item.name
}

const formatDate = (date: string) => {
  return new Date(date).toLocaleDateString(locale.value === 'ar' ? 'ar-SA' : 'en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  })
}

const formatFileSize = (bytes: number) => {
  if (bytes === 0) return '0 B'
  const k = 1024
  const sizes = ['B', 'KB', 'MB', 'GB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return parseFloat((bytes / Math.pow(k, i)).toFixed(1)) + ' ' + sizes[i]
}

const getFileIcon = (extension: string) => {
  const iconMap: Record<string, string> = {
    '.pdf': 'pi pi-file-pdf text-red-500',
    '.docx': 'pi pi-file-word text-blue-500',
    '.xlsx': 'pi pi-file-excel text-green-500'
  }
  return iconMap[extension] || 'pi pi-file'
}

const navigateToArticle = (article: any) => {
  router.push({ name: 'content-view', params: { id: article.slug } })
}

const navigateToDocument = (doc: any) => {
  router.push({ name: 'document-detail', params: { id: doc.id } })
}

const navigateToUser = (user: any) => {
  router.push({ name: 'user-profile', params: { id: user.id } })
}

watch(() => route.query.q, (newQuery) => {
  if (newQuery) {
    searchQuery.value = newQuery as string
    search()
  }
})

onMounted(() => {
  if (searchQuery.value) {
    search()
  }
})
</script>

<template>
  <div class="search-view">
    <!-- Search Header -->
    <div class="search-header">
      <h1>{{ t('search.title') }}</h1>
      <div class="search-box">
        <i class="pi pi-search"></i>
        <InputText
          v-model="searchQuery"
          :placeholder="t('search.placeholder')"
          class="search-input"
          @keyup.enter="search"
        />
        <Button
          :label="t('search.search')"
          icon="pi pi-search"
          @click="search"
        />
      </div>
    </div>

    <!-- Results Area -->
    <div v-if="searchQuery" class="search-content">
      <!-- Filters Sidebar -->
      <aside class="filters-sidebar">
        <div class="filter-section">
          <h3>{{ t('search.filterByType') }}</h3>
          <div class="filter-options">
            <div v-for="type in localizedTypeFilters" :key="type.value" class="filter-option">
              <Checkbox
                v-model="selectedTypes"
                :inputId="type.value"
                :value="type.value"
              />
              <label :for="type.value">{{ type.label }}</label>
            </div>
          </div>
        </div>

        <div class="filter-section">
          <h3>{{ t('search.filterByDate') }}</h3>
          <div class="filter-options">
            <div v-for="date in localizedDateFilters" :key="date.value || 'any'" class="filter-option">
              <input
                type="radio"
                :id="date.value || 'any'"
                v-model="selectedDateRange"
                :value="date.value"
                name="dateRange"
              />
              <label :for="date.value || 'any'">{{ date.label }}</label>
            </div>
          </div>
        </div>
      </aside>

      <!-- Results -->
      <main class="results-main">
        <!-- Loading State -->
        <div v-if="loading" class="loading-state">
          <div v-for="i in 3" :key="i" class="skeleton-result">
            <Skeleton width="70%" height="1.5rem" class="mb-2" />
            <Skeleton width="100%" height="1rem" class="mb-2" />
            <Skeleton width="40%" height="1rem" />
          </div>
        </div>

        <!-- Results Content -->
        <div v-else-if="results.total > 0">
          <p class="results-count">
            {{ t('search.resultsFound', { count: results.total }) }}
            <span class="query-text">"{{ searchQuery }}"</span>
          </p>

          <TabView v-model:activeIndex="activeTab">
            <!-- All Results -->
            <TabPanel value="0" :header="`${t('search.all')} (${results.total})`">
              <!-- Articles -->
              <div v-if="results.articles.length" class="result-section">
                <h3 class="section-title">
                  <i class="pi pi-file-edit"></i>
                  {{ t('search.articles') }}
                </h3>
                <div
                  v-for="article in results.articles"
                  :key="article.id"
                  class="result-item"
                  @click="navigateToArticle(article)"
                >
                  <div class="result-header">
                    <h4>{{ getTitle(article) }}</h4>
                    <Tag :value="article.type" severity="info" />
                  </div>
                  <p class="result-excerpt" v-html="article.highlightedContent"></p>
                  <div class="result-meta">
                    <span><i class="pi pi-user"></i> {{ article.authorName }}</span>
                    <span><i class="pi pi-calendar"></i> {{ formatDate(article.publishedAt) }}</span>
                  </div>
                </div>
              </div>

              <!-- Documents -->
              <div v-if="results.documents.length" class="result-section">
                <h3 class="section-title">
                  <i class="pi pi-folder"></i>
                  {{ t('search.documents') }}
                </h3>
                <div
                  v-for="doc in results.documents"
                  :key="doc.id"
                  class="result-item document-item"
                  @click="navigateToDocument(doc)"
                >
                  <i :class="getFileIcon(doc.fileExtension)" class="doc-icon"></i>
                  <div class="doc-content">
                    <div class="result-header">
                      <h4>{{ getTitle(doc) }}</h4>
                      <span class="file-info">{{ doc.fileName }} • {{ formatFileSize(doc.fileSize) }}</span>
                    </div>
                    <p class="result-excerpt" v-html="doc.highlightedContent"></p>
                    <div class="result-meta">
                      <span><i class="pi pi-folder"></i> {{ doc.libraryName }}</span>
                      <span><i class="pi pi-tag"></i> v{{ doc.version }}</span>
                      <span><i class="pi pi-calendar"></i> {{ formatDate(doc.updatedAt) }}</span>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Users -->
              <div v-if="results.users.length" class="result-section">
                <h3 class="section-title">
                  <i class="pi pi-users"></i>
                  {{ t('search.people') }}
                </h3>
                <div
                  v-for="user in results.users"
                  :key="user.id"
                  class="result-item user-item"
                  @click="navigateToUser(user)"
                >
                  <div class="user-avatar">
                    <i class="pi pi-user"></i>
                  </div>
                  <div class="user-info">
                    <h4>{{ locale === 'ar' && user.displayNameArabic ? user.displayNameArabic : user.displayName }}</h4>
                    <p>{{ user.jobTitle }} • {{ user.department }}</p>
                    <span class="user-email">{{ user.email }}</span>
                  </div>
                </div>
              </div>
            </TabPanel>

            <!-- Articles Tab -->
            <TabPanel value="1" :header="`${t('search.articles')} (${results.articles.length})`">
              <div
                v-for="article in results.articles"
                :key="article.id"
                class="result-item"
                @click="navigateToArticle(article)"
              >
                <div class="result-header">
                  <h4>{{ getTitle(article) }}</h4>
                  <Tag :value="article.type" severity="info" />
                </div>
                <p class="result-excerpt" v-html="article.highlightedContent"></p>
                <div class="result-meta">
                  <span><i class="pi pi-user"></i> {{ article.authorName }}</span>
                  <span><i class="pi pi-calendar"></i> {{ formatDate(article.publishedAt) }}</span>
                </div>
              </div>
            </TabPanel>

            <!-- Documents Tab -->
            <TabPanel value="2" :header="`${t('search.documents')} (${results.documents.length})`">
              <div
                v-for="doc in results.documents"
                :key="doc.id"
                class="result-item document-item"
                @click="navigateToDocument(doc)"
              >
                <i :class="getFileIcon(doc.fileExtension)" class="doc-icon"></i>
                <div class="doc-content">
                  <h4>{{ getTitle(doc) }}</h4>
                  <p class="result-excerpt" v-html="doc.highlightedContent"></p>
                  <div class="result-meta">
                    <span>{{ doc.fileName }} • {{ formatFileSize(doc.fileSize) }}</span>
                  </div>
                </div>
              </div>
            </TabPanel>

            <!-- People Tab -->
            <TabPanel value="3" :header="`${t('search.people')} (${results.users.length})`">
              <div
                v-for="user in results.users"
                :key="user.id"
                class="result-item user-item"
                @click="navigateToUser(user)"
              >
                <div class="user-avatar">
                  <i class="pi pi-user"></i>
                </div>
                <div class="user-info">
                  <h4>{{ user.displayName }}</h4>
                  <p>{{ user.jobTitle }} • {{ user.department }}</p>
                </div>
              </div>
            </TabPanel>
          </TabView>
        </div>

        <!-- No Results -->
        <div v-else class="no-results">
          <i class="pi pi-search"></i>
          <h3>{{ t('search.noResults') }}</h3>
          <p>{{ t('search.tryDifferentKeywords') }}</p>
        </div>
      </main>
    </div>

    <!-- Initial State -->
    <div v-else class="initial-state">
      <i class="pi pi-search"></i>
      <h2>{{ t('search.startSearching') }}</h2>
      <p>{{ t('search.searchDescription') }}</p>
    </div>
  </div>
</template>

<style scoped lang="scss">
@use '@/assets/styles/_variables.scss' as *;
@use '@/assets/styles/_mixins.scss' as *;

.search-view {
  @include page-view;
  max-width: 1200px;
  margin: 0 auto;
  padding: $spacing-6;
}

.search-header {
  margin-bottom: $spacing-8;

  h1 {
    margin: 0 0 $spacing-4 0;
    font-size: $font-size-2xl;
    font-weight: $font-weight-semibold;
    color: $slate-900;
  }

  .search-box {
    display: flex;
    gap: $spacing-2;
    position: relative;

    i.pi-search {
      position: absolute;
      inset-inline-start: $spacing-4;
      top: 50%;
      transform: translateY(-50%);
      color: $slate-400;
      z-index: 1;
    }

    .search-input {
      flex: 1;
      padding-inline-start: $spacing-10;
      font-size: $font-size-base;
    }
  }
}

.search-content {
  display: grid;
  grid-template-columns: 250px 1fr;
  gap: $spacing-8;
}

.filters-sidebar {
  .filter-section {
    margin-bottom: $spacing-6;

    h3 {
      font-size: $font-size-sm;
      font-weight: $font-weight-semibold;
      color: $slate-500;
      margin: 0 0 $spacing-3 0;
      text-transform: uppercase;
    }

    .filter-options {
      display: flex;
      flex-direction: column;
      gap: $spacing-2;
    }

    .filter-option {
      display: flex;
      align-items: center;
      gap: $spacing-2;

      label {
        cursor: pointer;
        color: $slate-700;
      }
    }
  }
}

.results-main {
  .results-count {
    margin: 0 0 $spacing-6 0;
    color: $slate-500;

    .query-text {
      color: $slate-900;
      font-weight: $font-weight-medium;
    }
  }
}

.result-section {
  margin-bottom: $spacing-8;

  .section-title {
    display: flex;
    align-items: center;
    gap: $spacing-2;
    font-size: $font-size-base;
    font-weight: $font-weight-semibold;
    margin: 0 0 $spacing-4 0;
    padding-bottom: $spacing-2;
    border-bottom: 1px solid $slate-200;
    color: $slate-900;
  }
}

.result-item {
  padding: $spacing-4;
  @include card-base;
  margin-bottom: $spacing-3;
  cursor: pointer;
  transition: all 0.2s ease;

  &:hover {
    border-color: $intalio-teal-500;
    box-shadow: $shadow-md;
  }

  .result-header {
    display: flex;
    justify-content: space-between;
    align-items: flex-start;
    gap: $spacing-4;
    margin-bottom: $spacing-2;

    h4 {
      margin: 0;
      font-size: $font-size-base;
      font-weight: $font-weight-semibold;
      color: $intalio-teal-600;
    }

    .file-info {
      font-size: $font-size-xs;
      color: $slate-500;
    }
  }

  .result-excerpt {
    margin: 0 0 $spacing-3 0;
    font-size: $font-size-sm;
    color: $slate-600;
    line-height: 1.5;

    :deep(mark) {
      background: $warning-100;
      padding: 0 2px;
      border-radius: $radius-sm;
    }
  }

  .result-meta {
    display: flex;
    gap: $spacing-4;
    font-size: $font-size-xs;
    color: $slate-500;

    span {
      display: flex;
      align-items: center;
      gap: $spacing-1;
    }
  }

  &.document-item {
    display: flex;
    gap: $spacing-4;

    .doc-icon {
      font-size: 2rem;
    }

    .doc-content {
      flex: 1;
    }
  }

  &.user-item {
    display: flex;
    gap: $spacing-4;
    align-items: center;

    .user-avatar {
      width: 48px;
      height: 48px;
      border-radius: 50%;
      background: $slate-100;
      display: flex;
      align-items: center;
      justify-content: center;

      i {
        font-size: $font-size-2xl;
        color: $slate-400;
      }
    }

    .user-info {
      h4 {
        margin: 0;
        font-size: $font-size-base;
        color: $slate-900;
      }

      p {
        margin: $spacing-1 0;
        font-size: $font-size-sm;
        color: $slate-500;
      }

      .user-email {
        font-size: $font-size-xs;
        color: $intalio-teal-500;
      }
    }
  }
}

.loading-state {
  .skeleton-result {
    padding: $spacing-4;
    margin-bottom: $spacing-3;
    @include card-base;
  }
}

.no-results,
.initial-state {
  text-align: center;
  padding: $spacing-16 $spacing-8;
  color: $slate-500;

  i {
    font-size: 4rem;
    margin-bottom: $spacing-4;
    opacity: 0.5;
  }

  h2, h3 {
    margin: 0 0 $spacing-2 0;
    color: $slate-900;
  }

  p {
    margin: 0;
  }
}

@media (max-width: 768px) {
  .search-content {
    grid-template-columns: 1fr;
  }

  .filters-sidebar {
    display: none;
  }

  .search-header .search-box {
    flex-direction: column;

    .search-input {
      width: 100%;
    }
  }
}
</style>
