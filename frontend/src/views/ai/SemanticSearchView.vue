<script setup lang="ts">
import { ref } from 'vue'
import { useI18n } from 'vue-i18n'
import Card from 'primevue/card'
import Button from 'primevue/button'
import InputText from 'primevue/inputtext'
import Checkbox from 'primevue/checkbox'
import Slider from 'primevue/slider'
import Tag from 'primevue/tag'
import Divider from 'primevue/divider'
import ProgressSpinner from 'primevue/progressspinner'

const { t, locale } = useI18n()

// State
const searchQuery = ref('')
const isSearching = ref(false)
const searchResults = ref<any[]>([])
const selectedScopes = ref<string[]>(['documents', 'articles', 'media'])

// Advanced options
const showAdvancedOptions = ref(false)
const similarityThreshold = ref(70)
const maxResults = ref(20)

// Computed
const scopeOptions = [
  { value: 'documents', label: 'المستندات', icon: 'pi-file' },
  { value: 'articles', label: 'المقالات', icon: 'pi-book' },
  { value: 'media', label: 'الوسائط', icon: 'pi-images' },
  { value: 'discussions', label: 'المناقشات', icon: 'pi-comments' }
]

// Methods
async function performSearch() {
  if (!searchQuery.value.trim()) return

  isSearching.value = true
  try {
    // TODO: Call API
    await new Promise(resolve => setTimeout(resolve, 1500))
    searchResults.value = [
      {
        id: '1',
        type: 'document',
        title: 'دليل العمليات التشغيلية',
        titleEn: 'Operations Manual',
        snippet: 'يحتوي هذا المستند على الإجراءات التفصيلية للعمليات اليومية...',
        similarity: 0.95,
        source: 'مكتبة المستندات',
        createdAt: new Date(),
        author: 'أحمد حسن'
      },
      {
        id: '2',
        type: 'article',
        title: 'أفضل الممارسات في إدارة الفعاليات',
        titleEn: 'Event Management Best Practices',
        snippet: 'تعرف على أهم الممارسات المتبعة في إدارة الفعاليات الرياضية الكبرى...',
        similarity: 0.88,
        source: 'قاعدة المعرفة',
        createdAt: new Date(Date.now() - 86400000),
        author: 'فاطمة الراشد'
      },
      {
        id: '3',
        type: 'media',
        title: 'فيديو تدريبي - إجراءات السلامة',
        titleEn: 'Training Video - Safety Procedures',
        snippet: 'فيديو تدريبي شامل يوضح إجراءات السلامة في الملاعب...',
        similarity: 0.82,
        source: 'مكتبة الوسائط',
        createdAt: new Date(Date.now() - 172800000),
        author: 'محمد علي'
      },
      {
        id: '4',
        type: 'discussion',
        title: 'نقاش: تحسين تجربة الجمهور',
        titleEn: 'Discussion: Improving Fan Experience',
        snippet: 'مناقشة حول طرق تحسين تجربة الجمهور خلال المباريات...',
        similarity: 0.75,
        source: 'منتدى العمليات',
        createdAt: new Date(Date.now() - 259200000),
        author: 'سارة أحمد'
      }
    ]
  } catch (error) {
    console.error('Search failed:', error)
  } finally {
    isSearching.value = false
  }
}

function getTypeIcon(type: string): string {
  switch (type) {
    case 'document': return 'pi-file'
    case 'article': return 'pi-book'
    case 'media': return 'pi-images'
    case 'discussion': return 'pi-comments'
    default: return 'pi-file'
  }
}

function getTypeSeverity(type: string): "success" | "info" | "warn" | "danger" | "secondary" | "contrast" | undefined {
  switch (type) {
    case 'document': return 'info'
    case 'article': return 'success'
    case 'media': return 'warn'
    case 'discussion': return 'secondary'
    default: return 'secondary'
  }
}

function formatDate(date: Date): string {
  return new Intl.DateTimeFormat(locale.value, {
    dateStyle: 'medium'
  }).format(date)
}

function handleKeyPress(event: KeyboardEvent) {
  if (event.key === 'Enter') {
    performSearch()
  }
}
</script>

<template>
  <div class="semantic-search-view">
    <!-- Header -->
    <div class="text-center mb-5">
      <h1 class="text-3xl font-bold m-0">{{ t('ai.semanticSearch.title') }}</h1>
      <p class="text-color-secondary mt-2 text-lg">{{ t('ai.semanticSearch.subtitle') }}</p>
    </div>

    <!-- Search Box -->
    <Card class="mb-4">
      <template #content>
        <div class="flex flex-column gap-3">
          <!-- Main Search Input -->
          <div class="p-inputgroup">
            <span class="p-inputgroup-addon">
              <i class="pi pi-search"></i>
            </span>
            <InputText
              v-model="searchQuery"
              :placeholder="t('ai.semanticSearch.placeholder')"
              class="text-lg"
              @keypress="handleKeyPress"
            />
            <Button
              :label="t('ai.semanticSearch.search')"
              icon="pi pi-search"
              @click="performSearch"
              :loading="isSearching"
            />
          </div>

          <!-- Scope Selection -->
          <div class="flex flex-wrap gap-3">
            <div
              v-for="scope in scopeOptions"
              :key="scope.value"
              class="flex align-items-center"
            >
              <Checkbox
                v-model="selectedScopes"
                :inputId="scope.value"
                :value="scope.value"
              />
              <label :for="scope.value" class="ml-2 cursor-pointer">
                <i :class="['pi', scope.icon, 'mr-1']"></i>
                {{ scope.label }}
              </label>
            </div>
          </div>

          <!-- Advanced Options Toggle -->
          <div>
            <Button
              :label="t('ai.semanticSearch.advancedOptions')"
              :icon="showAdvancedOptions ? 'pi pi-chevron-up' : 'pi pi-chevron-down'"
              text
              @click="showAdvancedOptions = !showAdvancedOptions"
            />
          </div>

          <!-- Advanced Options Panel -->
          <div v-if="showAdvancedOptions" class="surface-ground p-3 border-round">
            <div class="grid">
              <div class="col-12 md:col-6">
                <label class="block mb-2">
                  {{ t('ai.semanticSearch.similarityThreshold') }}: {{ similarityThreshold }}%
                </label>
                <Slider v-model="similarityThreshold" :min="50" :max="100" class="w-full" />
              </div>
              <div class="col-12 md:col-6">
                <label class="block mb-2">
                  {{ t('ai.semanticSearch.maxResults') }}: {{ maxResults }}
                </label>
                <Slider v-model="maxResults" :min="5" :max="50" class="w-full" />
              </div>
            </div>
          </div>
        </div>
      </template>
    </Card>

    <!-- Loading State -->
    <div v-if="isSearching" class="text-center py-5">
      <ProgressSpinner />
      <p class="mt-3">{{ t('ai.semanticSearch.searching') }}</p>
    </div>

    <!-- Results -->
    <div v-else-if="searchResults.length > 0">
      <div class="flex justify-content-between align-items-center mb-3">
        <span class="text-color-secondary">
          {{ t('ai.semanticSearch.resultsFound', { count: searchResults.length }) }}
        </span>
      </div>

      <div class="flex flex-column gap-3">
        <Card
          v-for="result in searchResults"
          :key="result.id"
          class="search-result-card cursor-pointer hover:shadow-3 transition-all transition-duration-200"
        >
          <template #content>
            <div class="flex gap-3">
              <!-- Type Icon -->
              <div
                class="flex-shrink-0 w-3rem h-3rem border-round flex align-items-center justify-content-center"
                :class="{
                  'bg-blue-100 text-blue-600': result.type === 'document',
                  'bg-green-100 text-green-600': result.type === 'article',
                  'bg-yellow-100 text-yellow-600': result.type === 'media',
                  'bg-gray-100 text-gray-600': result.type === 'discussion'
                }"
              >
                <i :class="['pi', getTypeIcon(result.type), 'text-xl']"></i>
              </div>

              <!-- Content -->
              <div class="flex-1">
                <div class="flex align-items-start justify-content-between mb-2">
                  <div>
                    <h3 class="m-0 text-lg font-semibold">{{ result.title }}</h3>
                    <div class="flex align-items-center gap-2 mt-1">
                      <Tag :value="result.type" :severity="getTypeSeverity(result.type)" size="small" />
                      <span class="text-sm text-color-secondary">{{ result.source }}</span>
                    </div>
                  </div>
                  <div class="text-right">
                    <div class="flex align-items-center gap-1">
                      <i class="pi pi-percentage text-sm text-color-secondary"></i>
                      <span class="font-bold" :class="{
                        'text-green-600': result.similarity >= 0.9,
                        'text-blue-600': result.similarity >= 0.8 && result.similarity < 0.9,
                        'text-yellow-600': result.similarity < 0.8
                      }">
                        {{ Math.round(result.similarity * 100) }}%
                      </span>
                    </div>
                    <span class="text-xs text-color-secondary">{{ t('ai.semanticSearch.similarity') }}</span>
                  </div>
                </div>

                <p class="m-0 text-color-secondary line-clamp-2">{{ result.snippet }}</p>

                <Divider class="my-2" />

                <div class="flex align-items-center justify-content-between text-sm text-color-secondary">
                  <div class="flex align-items-center gap-2">
                    <i class="pi pi-user"></i>
                    <span>{{ result.author }}</span>
                  </div>
                  <div class="flex align-items-center gap-2">
                    <i class="pi pi-calendar"></i>
                    <span>{{ formatDate(result.createdAt) }}</span>
                  </div>
                </div>
              </div>
            </div>
          </template>
        </Card>
      </div>
    </div>

    <!-- Empty State -->
    <div v-else-if="searchQuery && !isSearching" class="text-center py-5">
      <i class="pi pi-search text-5xl text-color-secondary mb-3"></i>
      <h3>{{ t('ai.semanticSearch.noResults') }}</h3>
      <p class="text-color-secondary">{{ t('ai.semanticSearch.tryDifferentQuery') }}</p>
    </div>

    <!-- Initial State -->
    <div v-else class="text-center py-5">
      <i class="pi pi-sparkles text-5xl text-primary mb-3"></i>
      <h3>{{ t('ai.semanticSearch.initialTitle') }}</h3>
      <p class="text-color-secondary">{{ t('ai.semanticSearch.initialSubtitle') }}</p>

      <div class="mt-4">
        <h4 class="mb-3">{{ t('ai.semanticSearch.suggestions') }}</h4>
        <div class="flex flex-wrap justify-content-center gap-2">
          <Button
            label="إجراءات السلامة"
            outlined
            size="small"
            @click="searchQuery = 'إجراءات السلامة'; performSearch()"
          />
          <Button
            label="خطة الفعاليات"
            outlined
            size="small"
            @click="searchQuery = 'خطة الفعاليات'; performSearch()"
          />
          <Button
            label="تدريب الموظفين"
            outlined
            size="small"
            @click="searchQuery = 'تدريب الموظفين'; performSearch()"
          />
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped lang="scss">
@use '@/assets/styles/_variables.scss' as *;
@use '@/assets/styles/_mixins.scss' as *;

.semantic-search-view {
  @include page-view;
  padding: $spacing-6;
  max-width: 900px;
  margin: 0 auto;
}

.search-result-card:hover {
  transform: translateY(-2px);
}

.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

// Reduced motion support
@media (prefers-reduced-motion: reduce) {
  .search-result-card:hover {
    transform: none;
  }
}
</style>
