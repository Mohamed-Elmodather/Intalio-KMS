<!--
  ============================================
  PAGE TEMPLATE - AFC Asian Cup 2027 KMS Portal
  ============================================

  This template demonstrates the RECOMMENDED structure using the Design System
  base components. Use this as a reference when building new pages.

  COMPONENTS AVAILABLE:
  - PageHeader: Page header with breadcrumb, title, description, actions
  - StatsBar: Floating stats bar with animated metrics
  - Widget: Dashboard widget wrapper with header, tabs, body
  - BaseCard: Flexible card component
  - BaseButton: Button with variants
  - BaseInput: Input with variants

  IMPORTS:
  import { PageHeader, StatsBar, Widget, BaseCard, BaseButton, BaseInput } from '@/components/base'

  ============================================
-->

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRouter } from 'vue-router'
import Skeleton from 'primevue/skeleton'

// Import Base Components
import {
  PageHeader,
  StatsBar,
  Widget,
  BaseCard,
  BaseButton,
  BaseInput
} from '@/components/base'

// Import types
import type { BreadcrumbItem, StatItem, WidgetTab } from '@/components/base'

const { locale } = useI18n()
// Router available for navigation if needed
void useRouter()

// RTL support
const isRTL = computed(() => locale.value === 'ar')

// Loading state
const loading = ref(true)

// Search
const searchQuery = ref('')

// Widget tabs
const chartPeriod = ref('6months')
const chartTabs: WidgetTab[] = [
  { label: '6 Months', value: '6months' },
  { label: 'Year', value: 'year' }
]

// Breadcrumb navigation
const breadcrumbs = computed<BreadcrumbItem[]>(() => [
  {
    label: 'Home',
    labelArabic: 'الرئيسية',
    to: '/dashboard',
    icon: 'pi pi-home'
  },
  {
    label: 'Page Name',
    labelArabic: 'اسم الصفحة'
  }
])

// Stats data
const stats = computed<StatItem[]>(() => [
  {
    icon: 'pi pi-file',
    value: 42,
    label: 'Items',
    labelArabic: 'العناصر',
    colorClass: 'primary'
  },
  {
    icon: 'pi pi-check-circle',
    value: 18,
    label: 'Completed',
    labelArabic: 'مكتمل',
    colorClass: 'success'
  },
  {
    icon: 'pi pi-clock',
    value: 5,
    label: 'Pending',
    labelArabic: 'قيد الانتظار',
    colorClass: 'warning',
    trend: {
      value: '+2',
      direction: 'up'
    }
  }
])

// Sample card data
const sampleCards = ref([
  { id: 1, title: 'Card One', description: 'This is the first card with some content.' },
  { id: 2, title: 'Card Two', description: 'This is the second card with different content.' },
  { id: 3, title: 'Card Three', description: 'This is the third card showcasing the component.' }
])

// Actions
const handlePrimaryAction = () => {
  console.log('Primary action clicked')
}

const handleSecondaryAction = () => {
  console.log('Secondary action clicked')
}

const handleCardClick = (card: typeof sampleCards.value[0]) => {
  console.log('Card clicked:', card.id)
}

onMounted(async () => {
  // Simulate loading
  await new Promise(resolve => setTimeout(resolve, 600))
  loading.value = false
})
</script>

<template>
  <div class="page-template-view" :class="{ 'rtl': isRTL }">
    <!--
      ============================================
      PAGE HEADER COMPONENT
      ============================================
      Props:
      - title: Page title
      - titleArabic: Arabic title
      - description: Page description
      - breadcrumbs: Array of breadcrumb items
      - paddingBottom: 'sm' | 'md' | 'lg' | 'xl' (for stats bar overlap)

      Slots:
      - actions: Header action buttons
      - extra: Additional content below description
    -->
    <PageHeader
      :title="isRTL ? 'عنوان الصفحة' : 'Page Title'"
      :description="isRTL ? 'وصف مختصر للصفحة' : 'Brief description of the page purpose'"
      :breadcrumbs="breadcrumbs"
      padding-bottom="lg"
    >
      <template #actions>
        <BaseButton variant="secondary" @click="handleSecondaryAction">
          {{ isRTL ? 'ثانوي' : 'Secondary' }}
        </BaseButton>
        <BaseButton variant="primary" icon="pi pi-plus" @click="handlePrimaryAction">
          {{ isRTL ? 'إجراء رئيسي' : 'Primary Action' }}
        </BaseButton>
      </template>
    </PageHeader>

    <!--
      ============================================
      STATS BAR COMPONENT
      ============================================
      Props:
      - stats: Array of stat items
      - loading: Show skeleton loading
      - overlap: 'sm' | 'md' | 'lg' (negative margin top)
      - animated: Enable stagger animation
    -->
    <StatsBar :stats="stats" :loading="loading" overlap="lg" />

    <!--
      ============================================
      LOADING STATE
      ============================================
    -->
    <div v-if="loading" class="main-content">
      <div class="loading-grid">
        <Skeleton height="200px" borderRadius="16px" />
        <Skeleton height="200px" borderRadius="16px" />
        <Skeleton height="200px" borderRadius="16px" />
      </div>
    </div>

    <!--
      ============================================
      MAIN CONTENT
      ============================================
    -->
    <div v-else class="main-content">
      <!-- Search Input Example -->
      <div class="search-section">
        <BaseInput
          v-model="searchQuery"
          type="search"
          variant="elevated"
          :placeholder="isRTL ? 'البحث...' : 'Search...'"
          icon="pi pi-search"
          clearable
        />
      </div>

      <!--
        ============================================
        WIDGET COMPONENT EXAMPLES
        ============================================
        Props:
        - title: Widget title
        - icon: Title icon
        - tabs: Array of tab objects
        - activeTab: Current tab value (v-model)
        - loading: Show loading skeleton
        - span: Grid column span
        - collapsible: Allow collapsing

        Slots:
        - title-extra: Extra content after title
        - action: Header action buttons
        - default: Widget body content
        - footer: Widget footer
      -->
      <div class="widgets-grid">
        <!-- Widget with Tabs -->
        <Widget
          :title="isRTL ? 'الإحصائيات' : 'Statistics'"
          icon="pi pi-chart-line"
          :tabs="chartTabs"
          v-model:active-tab="chartPeriod"
          :span="8"
        >
          <div class="chart-placeholder">
            <i class="pi pi-chart-bar"></i>
            <p>{{ isRTL ? 'رسم بياني' : 'Chart content goes here' }}</p>
            <span>{{ isRTL ? 'الفترة' : 'Period' }}: {{ chartPeriod }}</span>
          </div>
        </Widget>

        <!-- Widget with Action -->
        <Widget
          :title="isRTL ? 'النشاط الأخير' : 'Recent Activity'"
          icon="pi pi-history"
          :span="4"
        >
          <template #action>
            <BaseButton variant="text" size="sm">
              {{ isRTL ? 'عرض الكل' : 'View All' }}
            </BaseButton>
          </template>

          <div class="activity-list">
            <div class="activity-item" v-for="i in 3" :key="i">
              <div class="activity-icon">
                <i class="pi pi-user"></i>
              </div>
              <div class="activity-content">
                <p><strong>User {{ i }}</strong> performed an action</p>
                <span>{{ i }} hours ago</span>
              </div>
            </div>
          </div>
        </Widget>
      </div>

      <!--
        ============================================
        BASE CARD COMPONENT EXAMPLES
        ============================================
        Props:
        - variant: 'base' | 'elevated' | 'interactive' | 'flat' | 'glass'
        - padding: 'none' | 'sm' | 'md' | 'lg' | 'xl'
        - loading: Show skeleton
        - noHover: Disable hover effects

        Slots:
        - header: Card header
        - default: Card body
        - footer: Card footer
      -->
      <h2 class="section-title">{{ isRTL ? 'أمثلة البطاقات' : 'Card Examples' }}</h2>

      <div class="cards-grid">
        <!-- Interactive Card -->
        <BaseCard
          v-for="card in sampleCards"
          :key="card.id"
          variant="interactive"
          @click="handleCardClick(card)"
        >
          <template #header>
            <div class="card-header-content">
              <i class="pi pi-folder"></i>
              <span>{{ card.title }}</span>
            </div>
          </template>

          <p class="card-description">{{ card.description }}</p>

          <template #footer>
            <div class="card-footer-content">
              <BaseButton variant="ghost" size="sm" icon="pi pi-eye">
                View
              </BaseButton>
              <BaseButton variant="text" size="sm" icon="pi pi-ellipsis-h" icon-only />
            </div>
          </template>
        </BaseCard>
      </div>

      <!--
        ============================================
        BUTTON VARIANTS SHOWCASE
        ============================================
      -->
      <h2 class="section-title">{{ isRTL ? 'أنواع الأزرار' : 'Button Variants' }}</h2>

      <div class="buttons-showcase">
        <BaseButton variant="primary">Primary</BaseButton>
        <BaseButton variant="secondary">Secondary</BaseButton>
        <BaseButton variant="ghost">Ghost</BaseButton>
        <BaseButton variant="outline">Outline</BaseButton>
        <BaseButton variant="danger">Danger</BaseButton>
        <BaseButton variant="success">Success</BaseButton>
        <BaseButton variant="text">Text</BaseButton>
        <BaseButton variant="primary" icon="pi pi-plus" icon-only rounded />
        <BaseButton variant="primary" :loading="true">Loading</BaseButton>
      </div>

      <!--
        ============================================
        INPUT VARIANTS SHOWCASE
        ============================================
      -->
      <h2 class="section-title">{{ isRTL ? 'أنواع الحقول' : 'Input Variants' }}</h2>

      <div class="inputs-showcase">
        <BaseInput
          label="Default Input"
          placeholder="Enter text..."
          helper-text="This is a helper text"
        />
        <BaseInput
          variant="elevated"
          label="Elevated Input"
          placeholder="Elevated style..."
          icon="pi pi-user"
        />
        <BaseInput
          variant="filled"
          label="Filled Input"
          placeholder="Filled style..."
        />
        <BaseInput
          label="With Error"
          placeholder="Invalid input..."
          error="This field is required"
        />
      </div>
    </div>
  </div>
</template>

<style scoped lang="scss">
@use '@/design-system/tokens' as *;
@use '@/design-system/mixins' as *;

// ============================================
// PAGE CONTAINER
// ============================================
.page-template-view {
  min-height: 100vh;
  background: $slate-50;

  &.rtl {
    direction: rtl;
  }
}

// ============================================
// MAIN CONTENT
// ============================================
.main-content {
  padding: 0 $spacing-6 $spacing-6;

  @media (max-width: $breakpoint-md) {
    padding: 0 $spacing-4 $spacing-4;
  }
}

// ============================================
// LOADING STATE
// ============================================
.loading-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: $spacing-6;

  @media (max-width: $breakpoint-md) {
    grid-template-columns: 1fr;
  }
}

// ============================================
// SEARCH SECTION
// ============================================
.search-section {
  max-width: 400px;
  margin-bottom: $spacing-6;
}

// ============================================
// WIDGETS GRID
// ============================================
.widgets-grid {
  display: grid;
  grid-template-columns: repeat(12, 1fr);
  gap: $spacing-6;
  margin-bottom: $spacing-8;

  @media (max-width: $breakpoint-lg) {
    grid-template-columns: repeat(6, 1fr);
  }

  @media (max-width: $breakpoint-md) {
    grid-template-columns: 1fr;
  }
}

// Chart placeholder
.chart-placeholder {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 200px;
  background: $slate-50;
  border-radius: $radius-lg;
  color: $slate-500;

  i {
    font-size: 2.5rem;
    margin-bottom: $spacing-3;
  }

  p {
    margin: 0;
    font-size: $font-size-sm;
  }

  span {
    font-size: $font-size-xs;
    color: $slate-400;
    margin-top: $spacing-2;
  }
}

// Activity list
.activity-list {
  display: flex;
  flex-direction: column;
  gap: $spacing-4;
}

.activity-item {
  display: flex;
  gap: $spacing-3;
}

.activity-icon {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 36px;
  height: 36px;
  background: $slate-100;
  border-radius: $radius-lg;
  color: $slate-500;
  flex-shrink: 0;

  i {
    font-size: 0.875rem;
  }
}

.activity-content {
  flex: 1;

  p {
    font-size: $font-size-sm;
    color: $slate-700;
    margin: 0;

    strong {
      color: $slate-900;
    }
  }

  span {
    font-size: $font-size-xs;
    color: $slate-400;
  }
}

// ============================================
// SECTION TITLES
// ============================================
.section-title {
  font-size: $font-size-lg;
  font-weight: $font-weight-semibold;
  color: $slate-900;
  margin: 0 0 $spacing-4;
}

// ============================================
// CARDS GRID
// ============================================
.cards-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: $spacing-5;
  margin-bottom: $spacing-8;
}

.card-header-content {
  display: flex;
  align-items: center;
  gap: $spacing-2;

  i {
    color: $brand-500;
  }
}

.card-description {
  color: $slate-600;
  font-size: $font-size-sm;
  line-height: 1.5;
  margin: 0;
}

.card-footer-content {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

// ============================================
// BUTTONS SHOWCASE
// ============================================
.buttons-showcase {
  display: flex;
  flex-wrap: wrap;
  gap: $spacing-3;
  margin-bottom: $spacing-8;
  padding: $spacing-5;
  background: white;
  border-radius: $radius-xl;
  border: 1px solid $slate-100;
}

// ============================================
// INPUTS SHOWCASE
// ============================================
.inputs-showcase {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
  gap: $spacing-5;
  padding: $spacing-5;
  background: white;
  border-radius: $radius-xl;
  border: 1px solid $slate-100;
}
</style>
