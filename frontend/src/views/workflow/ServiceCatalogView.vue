<template>
  <div class="service-catalog" :class="{ 'rtl': isRTL }">
    <!-- Error State -->
    <ErrorState
      v-if="error"
      :error="error"
      :title="isRTL ? 'فشل تحميل كتالوج الخدمات' : 'Failed to load service catalog'"
      show-retry
      size="lg"
      @retry="handleRetry"
    />

    <template v-else>
    <!-- Header -->
    <div class="catalog-header">
      <div class="header-content">
        <h1>{{ $t('workflow.serviceCatalog') }}</h1>
        <p class="text-muted">{{ $t('workflow.serviceCatalogDescription') }}</p>
      </div>
      <div class="header-search">
        <IconField iconPosition="left">
          <InputIcon class="pi pi-search" />
          <InputText
            v-model="searchQuery"
            :placeholder="$t('workflow.searchServices')"
            class="search-input"
          />
        </IconField>
      </div>
    </div>

    <!-- Featured Services -->
    <section v-if="featuredServices.length > 0 && !searchQuery" class="featured-section">
      <h2>{{ $t('workflow.featuredServices') }}</h2>
      <div class="featured-grid">
        <Card
          v-for="service in featuredServices"
          :key="service.id"
          class="featured-card"
          @click="openService(service)"
        >
          <template #header>
            <div class="featured-icon" :style="{ backgroundColor: getCategoryColor(service.categoryName) }">
              <i :class="service.icon || 'pi pi-cog'"></i>
            </div>
          </template>
          <template #title>{{ service.name }}</template>
          <template #subtitle>{{ service.categoryName }}</template>
          <template #content>
            <p>{{ service.description }}</p>
          </template>
          <template #footer>
            <div class="service-meta">
              <Tag :value="getServiceTypeLabel(service.type)" :severity="getServiceTypeSeverity(service.type)" />
              <span class="request-count">
                <i class="pi pi-inbox"></i>
                {{ service.requestCount }}
              </span>
            </div>
          </template>
        </Card>
      </div>
    </section>

    <!-- Main Content -->
    <div class="catalog-content">
      <!-- Categories Sidebar -->
      <aside class="categories-sidebar">
        <h3>{{ $t('workflow.categories') }}</h3>
        <div class="category-list">
          <div
            class="category-item"
            :class="{ active: !selectedCategory }"
            @click="selectCategory(null)"
          >
            <i class="pi pi-th-large"></i>
            <span>{{ $t('workflow.allServices') }}</span>
            <Badge :value="totalServiceCount" severity="secondary" />
          </div>
          <div
            v-for="category in categories"
            :key="category.id"
            class="category-item"
            :class="{ active: selectedCategory?.id === category.id }"
            @click="selectCategory(category)"
          >
            <i :class="category.icon || 'pi pi-folder'"></i>
            <span>{{ category.name }}</span>
            <Badge :value="category.serviceCount" severity="secondary" />
          </div>
        </div>
      </aside>

      <!-- Services Grid -->
      <div class="services-section">
        <!-- Filters Bar -->
        <div class="filters-bar">
          <div class="results-info">
            <span v-if="selectedCategory">
              {{ $t('workflow.servicesInCategory', { category: selectedCategory.name, count: filteredServices.length }) }}
            </span>
            <span v-else>
              {{ $t('workflow.allServicesCount', { count: filteredServices.length }) }}
            </span>
          </div>
          <div class="filter-actions">
            <Dropdown
              v-model="selectedType"
              :options="serviceTypeOptions"
              optionLabel="label"
              optionValue="value"
              :placeholder="$t('workflow.filterByType')"
              showClear
              class="type-filter"
            />
            <SelectButton
              v-model="viewMode"
              :options="viewModeOptions"
              optionLabel="icon"
              optionValue="value"
              class="view-toggle"
            >
              <template #option="{ option }">
                <i :class="option.icon"></i>
              </template>
            </SelectButton>
          </div>
        </div>

        <!-- Loading -->
        <div v-if="isLoading" class="loading-state">
          <ProgressSpinner />
          <p>{{ $t('common.loading') }}</p>
        </div>

        <!-- Empty State -->
        <div v-else-if="filteredServices.length === 0" class="empty-state">
          <i class="pi pi-inbox"></i>
          <h3>{{ $t('workflow.noServicesFound') }}</h3>
          <p>{{ $t('workflow.tryDifferentFilters') }}</p>
        </div>

        <!-- Grid View -->
        <div v-else-if="viewMode === 'grid'" class="services-grid">
          <Card
            v-for="service in filteredServices"
            :key="service.id"
            class="service-card"
            @click="openService(service)"
          >
            <template #header>
              <div class="service-icon">
                <i :class="service.icon || 'pi pi-cog'"></i>
              </div>
            </template>
            <template #title>{{ service.name }}</template>
            <template #subtitle>{{ service.categoryName }}</template>
            <template #content>
              <p class="service-description">{{ service.description }}</p>
            </template>
            <template #footer>
              <div class="card-footer">
                <Tag :value="getServiceTypeLabel(service.type)" :severity="getServiceTypeSeverity(service.type)" size="small" />
                <Button
                  :label="$t('workflow.request')"
                  icon="pi pi-plus"
                  size="small"
                  @click.stop="createRequest(service)"
                />
              </div>
            </template>
          </Card>
        </div>

        <!-- List View -->
        <div v-else class="services-list">
          <div
            v-for="service in filteredServices"
            :key="service.id"
            class="service-list-item"
            @click="openService(service)"
          >
            <div class="service-icon">
              <i :class="service.icon || 'pi pi-cog'"></i>
            </div>
            <div class="service-info">
              <h4>{{ service.name }}</h4>
              <p>{{ service.description }}</p>
              <div class="service-meta">
                <span class="category">
                  <i class="pi pi-folder"></i>
                  {{ service.categoryName }}
                </span>
                <Tag :value="getServiceTypeLabel(service.type)" :severity="getServiceTypeSeverity(service.type)" size="small" />
              </div>
            </div>
            <div class="service-actions">
              <span class="request-count">
                <i class="pi pi-inbox"></i>
                {{ service.requestCount }} {{ $t('workflow.requests') }}
              </span>
              <Button
                :label="$t('workflow.request')"
                icon="pi pi-plus"
                @click.stop="createRequest(service)"
              />
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Service Detail Dialog -->
    <Dialog
      v-model:visible="showServiceDialog"
      :header="selectedService?.name"
      :modal="true"
      :style="{ width: '600px' }"
      class="service-dialog"
    >
      <div v-if="selectedService" class="service-detail">
        <div class="detail-header">
          <div class="detail-icon">
            <i :class="selectedService.icon || 'pi pi-cog'"></i>
          </div>
          <div class="detail-info">
            <Tag :value="selectedService.categoryName" severity="info" />
            <Tag :value="getServiceTypeLabel(selectedService.type)" :severity="getServiceTypeSeverity(selectedService.type)" />
          </div>
        </div>

        <div class="detail-description">
          <p>{{ selectedService.description }}</p>
        </div>

        <div v-if="selectedService.slaResponseHours || selectedService.slaResolutionHours" class="sla-info">
          <h4>{{ $t('workflow.sla') }}</h4>
          <div class="sla-items">
            <div v-if="selectedService.slaResponseHours" class="sla-item">
              <i class="pi pi-reply"></i>
              <span>{{ $t('workflow.responseTime') }}: {{ selectedService.slaResponseHours }}h</span>
            </div>
            <div v-if="selectedService.slaResolutionHours" class="sla-item">
              <i class="pi pi-check-circle"></i>
              <span>{{ $t('workflow.resolutionTime') }}: {{ selectedService.slaResolutionHours }}h</span>
            </div>
          </div>
        </div>

        <div v-if="selectedService.tags?.length" class="tags-section">
          <Tag v-for="tag in selectedService.tags" :key="tag" :value="tag" severity="secondary" />
        </div>
      </div>

      <template #footer>
        <Button :label="$t('common.close')" text @click="showServiceDialog = false" />
        <Button
          :label="$t('workflow.submitRequest')"
          icon="pi pi-plus"
          @click="createRequest(selectedService)"
        />
      </template>
    </Dialog>
    </template>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useReducedMotion } from '@/composables/useReducedMotion'
import ErrorState from '@/components/base/ErrorState.vue'

const { t, locale } = useI18n()
const router = useRouter()
const prefersReducedMotion = useReducedMotion()

// RTL support
const isRTL = computed(() => locale.value === 'ar')
const shouldAnimate = computed(() => !prefersReducedMotion.value)
const isContentVisible = ref(false)

// State
const isLoading = ref(true)
const error = ref<Error | null>(null)
const searchQuery = ref('')
const selectedCategory = ref<any>(null)
const selectedType = ref<string | null>(null)
const viewMode = ref('grid')
const showServiceDialog = ref(false)
const selectedService = ref<any>(null)

// Data
const categories = ref<any[]>([])
const services = ref<any[]>([])
const featuredServices = ref<any[]>([])

// Options
const serviceTypeOptions = computed(() => [
  { label: t('workflow.standard'), value: 'Standard' },
  { label: t('workflow.express'), value: 'Express' },
  { label: t('workflow.automated'), value: 'Automated' }
])

const viewModeOptions = [
  { icon: 'pi pi-th-large', value: 'grid' },
  { icon: 'pi pi-list', value: 'list' }
]

// Computed
const totalServiceCount = computed(() => services.value.length)

const filteredServices = computed(() => {
  let result = services.value

  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    result = result.filter(s =>
      s.name.toLowerCase().includes(query) ||
      s.description?.toLowerCase().includes(query)
    )
  }

  if (selectedCategory.value) {
    result = result.filter(s => s.categoryId === selectedCategory.value.id)
  }

  if (selectedType.value) {
    result = result.filter(s => s.type === selectedType.value)
  }

  return result
})

// Methods
const selectCategory = (category: any) => {
  selectedCategory.value = category
}

const openService = (service: any) => {
  selectedService.value = service
  showServiceDialog.value = true
}

const createRequest = (service: any) => {
  showServiceDialog.value = false
  router.push({ name: 'create-request', query: { serviceId: service.id } })
}

const getServiceTypeLabel = (type: string) => {
  const labels: Record<string, string> = {
    Standard: t('workflow.standard'),
    Express: t('workflow.express'),
    Automated: t('workflow.automated'),
    External: t('workflow.external')
  }
  return labels[type] || type
}

const getServiceTypeSeverity = (type: string) => {
  const severities: Record<string, string> = {
    Standard: 'info',
    Express: 'warning',
    Automated: 'success',
    External: 'secondary'
  }
  return severities[type] || 'info'
}

const getCategoryColor = (categoryName: string) => {
  const colors = ['#3B82F6', '#10B981', '#F59E0B', '#EF4444', '#8B5CF6', '#EC4899']
  const hash = categoryName.split('').reduce((a, b) => a + b.charCodeAt(0), 0)
  return colors[hash % colors.length]
}

// Data loading with error handling
async function loadServices() {
  try {
    error.value = null
    // Simulate API call
    await new Promise(resolve => setTimeout(resolve, 600))
    // TODO: Load categories and services from API
    categories.value = []
    services.value = []
    featuredServices.value = []
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
  isContentVisible.value = false
  loadServices()
}

// Load data
onMounted(() => {
  loadServices()
})
</script>

<style scoped lang="scss">
@use '@/assets/styles/_variables.scss' as *;
@use '@/assets/styles/_mixins.scss' as *;

.service-catalog {
  @include page-view;
}

.catalog-header {
  background: linear-gradient(135deg, $intalio-teal-500, $intalio-teal-700);
  color: white;
  padding: $spacing-8;
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  gap: $spacing-4;
  margin: (-$spacing-6) (-$spacing-6) $spacing-6 (-$spacing-6);

  .header-content {
    h1 {
      margin: 0 0 $spacing-2 0;
      font-size: $font-size-2xl;
      font-weight: $font-weight-bold;
    }

    .text-muted {
      opacity: 0.9;
      margin: 0;
    }
  }

  .search-input {
    width: 300px;
  }
}

.featured-section {
  padding: $spacing-8;
  max-width: 1400px;
  margin: 0 auto;

  h2 {
    margin-bottom: $spacing-6;
    font-size: $font-size-xl;
    font-weight: $font-weight-semibold;
    color: $slate-900;
  }

  .featured-grid {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
    gap: $spacing-6;
  }

  .featured-card {
    cursor: pointer;
    transition: transform 0.2s ease, box-shadow 0.2s ease;

    &:hover {
      transform: translateY(-4px);
      box-shadow: $shadow-lg;
    }

    .featured-icon {
      height: 100px;
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 2.5rem;
      color: white;
      border-radius: $radius-lg $radius-lg 0 0;
    }
  }
}

.catalog-content {
  max-width: 1400px;
  margin: 0 auto;
  padding: 0 $spacing-8 $spacing-8;
  display: grid;
  grid-template-columns: 280px 1fr;
  gap: $spacing-8;

  @media (max-width: 968px) {
    grid-template-columns: 1fr;
    padding: 0 $spacing-4 $spacing-4;
  }
}

.categories-sidebar {
  @include card-base;
  padding: $spacing-6;
  height: fit-content;
  position: sticky;
  top: $spacing-4;

  h3 {
    margin: 0 0 $spacing-4 0;
    font-size: $font-size-base;
    font-weight: $font-weight-semibold;
    color: $slate-900;
  }

  .category-list {
    display: flex;
    flex-direction: column;
    gap: $spacing-2;
  }

  .category-item {
    display: flex;
    align-items: center;
    gap: $spacing-3;
    padding: $spacing-3 $spacing-4;
    border-radius: $radius-lg;
    cursor: pointer;
    transition: background-color 0.2s ease;
    color: $slate-700;

    &:hover {
      background-color: $slate-100;
    }

    &.active {
      background-color: $intalio-teal-50;
      color: $intalio-teal-600;
    }

    i {
      font-size: $font-size-lg;
    }

    span {
      flex: 1;
    }
  }
}

.services-section {
  .filters-bar {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: $spacing-6;
    flex-wrap: wrap;
    gap: $spacing-4;

    .results-info {
      color: $slate-600;
    }

    .filter-actions {
      display: flex;
      gap: $spacing-3;
      align-items: center;
    }

    .type-filter {
      width: 180px;
    }
  }

  .loading-state, .empty-state {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    padding: $spacing-16 $spacing-8;
    text-align: center;

    i {
      font-size: 3rem;
      color: $slate-400;
      margin-bottom: $spacing-4;
    }

    h3 {
      color: $slate-900;
      margin: 0 0 $spacing-2 0;
    }

    p {
      color: $slate-500;
      margin: 0;
    }
  }

  .services-grid {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
    gap: $spacing-6;
  }

  .service-card {
    cursor: pointer;
    transition: transform 0.2s ease, box-shadow 0.2s ease;

    &:hover {
      transform: translateY(-2px);
      box-shadow: $shadow-md;
    }

    .service-icon {
      height: 60px;
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: $font-size-2xl;
      color: $intalio-teal-500;
      background: $intalio-teal-50;
      border-radius: $radius-lg $radius-lg 0 0;
    }

    .service-description {
      display: -webkit-box;
      -webkit-line-clamp: 2;
      -webkit-box-orient: vertical;
      overflow: hidden;
      color: $slate-500;
      font-size: $font-size-sm;
    }

    .card-footer {
      display: flex;
      justify-content: space-between;
      align-items: center;
    }
  }

  .services-list {
    display: flex;
    flex-direction: column;
    gap: $spacing-4;
  }

  .service-list-item {
    display: flex;
    gap: $spacing-4;
    padding: $spacing-5;
    @include card-base;
    cursor: pointer;
    transition: box-shadow 0.2s ease;

    &:hover {
      box-shadow: $shadow-md;
    }

    .service-icon {
      width: 60px;
      height: 60px;
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: $font-size-2xl;
      color: $intalio-teal-500;
      background: $intalio-teal-50;
      border-radius: $radius-xl;
      flex-shrink: 0;
    }

    .service-info {
      flex: 1;

      h4 {
        margin: 0 0 $spacing-2 0;
        font-weight: $font-weight-semibold;
        color: $slate-900;
      }

      p {
        margin: 0 0 $spacing-3 0;
        color: $slate-500;
        font-size: $font-size-sm;
      }

      .service-meta {
        display: flex;
        gap: $spacing-3;
        align-items: center;

        .category {
          display: flex;
          align-items: center;
          gap: $spacing-1;
          font-size: $font-size-sm;
          color: $slate-500;
        }
      }
    }

    .service-actions {
      display: flex;
      flex-direction: column;
      align-items: flex-end;
      gap: $spacing-3;

      .request-count {
        font-size: $font-size-sm;
        color: $slate-500;
        display: flex;
        align-items: center;
        gap: $spacing-1;
      }
    }
  }
}

.service-dialog {
  .service-detail {
    .detail-header {
      display: flex;
      gap: $spacing-4;
      align-items: flex-start;
      margin-bottom: $spacing-6;

      .detail-icon {
        width: 60px;
        height: 60px;
        display: flex;
        align-items: center;
        justify-content: center;
        font-size: $font-size-2xl;
        color: $intalio-teal-500;
        background: $intalio-teal-50;
        border-radius: $radius-xl;
      }

      .detail-info {
        display: flex;
        gap: $spacing-2;
      }
    }

    .detail-description {
      margin-bottom: $spacing-6;

      p {
        color: $slate-600;
        line-height: 1.6;
      }
    }

    .sla-info {
      background: $slate-50;
      padding: $spacing-4;
      border-radius: $radius-lg;
      margin-bottom: $spacing-6;

      h4 {
        margin: 0 0 $spacing-3 0;
        font-size: $font-size-sm;
        font-weight: $font-weight-semibold;
        color: $slate-900;
      }

      .sla-items {
        display: flex;
        gap: $spacing-6;
      }

      .sla-item {
        display: flex;
        align-items: center;
        gap: $spacing-2;
        font-size: $font-size-sm;
        color: $slate-600;
      }
    }

    .tags-section {
      display: flex;
      gap: $spacing-2;
      flex-wrap: wrap;
    }
  }
}
</style>
