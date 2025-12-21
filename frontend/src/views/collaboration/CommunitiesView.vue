<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useReducedMotion } from '@/composables/useReducedMotion'
import ErrorState from '@/components/base/ErrorState.vue'
import { PageHeader, StatsBar, Avatar } from '@/components/base'
import type { BreadcrumbItem } from '@/components/base/PageHeader.vue'
import type { StatItem } from '@/components/base/StatsBar.vue'
import Button from 'primevue/button'
import InputText from 'primevue/inputtext'
import AvatarGroup from 'primevue/avatargroup'
import Skeleton from 'primevue/skeleton'
import Dialog from 'primevue/dialog'
import Textarea from 'primevue/textarea'
import Dropdown from 'primevue/dropdown'

const { locale } = useI18n()
const router = useRouter()
const prefersReducedMotion = useReducedMotion()

const isRTL = computed(() => locale.value === 'ar')
const loading = ref(true)
const error = ref<Error | null>(null)
const isContentVisible = ref(false)

// Animation control
const shouldAnimate = computed(() => !prefersReducedMotion.value)

// Breadcrumbs for PageHeader
const breadcrumbs = computed<BreadcrumbItem[]>(() => [
  { label: 'Home', labelArabic: 'الرئيسية', to: '/dashboard' },
  { label: isRTL.value ? 'المجتمعات' : 'Communities' }
])
const searchQuery = ref('')
const selectedFilter = ref<'all' | 'my'>('all')
const selectedType = ref<string>('all')
const selectedVisibility = ref<string>('all')
const showFilters = ref(false)
const viewMode = ref<'grid' | 'list'>('grid')
const showCreateDialog = ref(false)

interface Community {
  id: string
  name: string
  nameArabic?: string
  description?: string
  descriptionArabic?: string
  slug: string
  iconUrl?: string
  coverUrl?: string
  type: 'General' | 'Project' | 'Department' | 'WorkingGroup'
  visibility: 'Public' | 'Private' | 'Secret'
  memberCount: number
  discussionCount: number
  isMember: boolean
  recentMembers?: { name: string; avatarUrl?: string }[]
  lastActivity?: Date
}

const communities = ref<Community[]>([])

const newCommunity = ref({
  name: '',
  nameArabic: '',
  description: '',
  descriptionArabic: '',
  type: 'General',
  visibility: 'Public'
})

// Filter toggle options
const filterToggleOptions = computed(() => [
  { value: 'all', label: isRTL.value ? 'جميع المجتمعات' : 'All Communities', icon: 'pi-th-large' },
  { value: 'my', label: isRTL.value ? 'مجتمعاتي' : 'My Communities', icon: 'pi-user' }
])

// Type options for filter panel
const typeOptions = computed(() => [
  { label: isRTL.value ? 'جميع الأنواع' : 'All Types', value: 'all' },
  { label: isRTL.value ? 'عام' : 'General', value: 'General' },
  { label: isRTL.value ? 'مشروع' : 'Project', value: 'Project' },
  { label: isRTL.value ? 'قسم' : 'Department', value: 'Department' },
  { label: isRTL.value ? 'فريق عمل' : 'Working Group', value: 'WorkingGroup' }
])

// Visibility options for filter panel
const visibilityOptions = computed(() => [
  { label: isRTL.value ? 'جميع الرؤية' : 'All Visibility', value: 'all' },
  { label: isRTL.value ? 'عام' : 'Public', value: 'Public' },
  { label: isRTL.value ? 'خاص' : 'Private', value: 'Private' },
  { label: isRTL.value ? 'سري' : 'Secret', value: 'Secret' }
])

// Type configuration
const TYPE_CONFIG: Record<string, { icon: string; color: string; bgColor: string; label: string; labelAr: string }> = {
  General: { icon: 'pi pi-globe', color: '#3b82f6', bgColor: 'rgba(59, 130, 246, 0.1)', label: 'General', labelAr: 'عام' },
  Project: { icon: 'pi pi-briefcase', color: '#10b981', bgColor: 'rgba(16, 185, 129, 0.1)', label: 'Project', labelAr: 'مشروع' },
  Department: { icon: 'pi pi-building', color: '#f59e0b', bgColor: 'rgba(245, 158, 11, 0.1)', label: 'Department', labelAr: 'قسم' },
  WorkingGroup: { icon: 'pi pi-users', color: '#8b5cf6', bgColor: 'rgba(139, 92, 246, 0.1)', label: 'Working Group', labelAr: 'فريق عمل' }
}

// Visibility configuration
const VISIBILITY_CONFIG: Record<string, { icon: string; label: string; labelAr: string }> = {
  Public: { icon: 'pi pi-globe', label: 'Public', labelAr: 'عام' },
  Private: { icon: 'pi pi-lock', label: 'Private', labelAr: 'خاص' },
  Secret: { icon: 'pi pi-eye-slash', label: 'Secret', labelAr: 'سري' }
}

// Stats
const stats = computed(() => {
  const all = communities.value
  const my = all.filter(c => c.isMember)
  return {
    total: all.length,
    myCommunities: my.length,
    totalMembers: all.reduce((sum, c) => sum + c.memberCount, 0),
    totalDiscussions: all.reduce((sum, c) => sum + c.discussionCount, 0)
  }
})

// StatsBar items
const statsBarItems = computed<StatItem[]>(() => [
  {
    icon: 'pi pi-users',
    value: stats.value.total,
    label: 'Total Communities',
    labelArabic: 'إجمالي المجتمعات',
    colorClass: 'primary'
  },
  {
    icon: 'pi pi-user-plus',
    value: stats.value.myCommunities,
    label: 'My Communities',
    labelArabic: 'مجتمعاتي',
    colorClass: 'success'
  },
  {
    icon: 'pi pi-user',
    value: stats.value.totalMembers,
    label: 'Total Members',
    labelArabic: 'إجمالي الأعضاء',
    colorClass: 'info'
  },
  {
    icon: 'pi pi-comments',
    value: stats.value.totalDiscussions,
    label: 'Discussions',
    labelArabic: 'المناقشات',
    colorClass: 'warning'
  }
])

// Active filters count
const activeFiltersCount = computed(() => {
  let count = 0
  if (selectedFilter.value !== 'all') count++
  if (selectedType.value !== 'all') count++
  if (selectedVisibility.value !== 'all') count++
  return count
})

// Filtered communities
const filteredCommunities = computed(() => {
  let result = [...communities.value]

  // Filter by my communities
  if (selectedFilter.value === 'my') {
    result = result.filter(c => c.isMember)
  }

  // Filter by type
  if (selectedType.value !== 'all') {
    result = result.filter(c => c.type === selectedType.value)
  }

  // Filter by visibility
  if (selectedVisibility.value !== 'all') {
    result = result.filter(c => c.visibility === selectedVisibility.value)
  }

  // Filter by search
  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    result = result.filter(c =>
      c.name.toLowerCase().includes(query) ||
      c.nameArabic?.toLowerCase().includes(query) ||
      c.description?.toLowerCase().includes(query)
    )
  }

  return result
})

// Helpers
const getName = (community: Community) => {
  return isRTL.value && community.nameArabic ? community.nameArabic : community.name
}

const getDescription = (community: Community) => {
  return isRTL.value && community.descriptionArabic ? community.descriptionArabic : community.description
}

const getTypeConfig = (type: string) => TYPE_CONFIG[type] || TYPE_CONFIG.General
const getVisibilityConfig = (visibility: string) => VISIBILITY_CONFIG[visibility] || VISIBILITY_CONFIG.Public

// Actions
const navigateToCommunity = (community: Community) => {
  router.push({ name: 'community-detail', params: { slug: community.slug } })
}

const joinCommunity = async (community: Community, event: Event) => {
  event.stopPropagation()
  community.isMember = true
  community.memberCount++
}

const clearAllFilters = () => {
  searchQuery.value = ''
  selectedFilter.value = 'all'
  selectedType.value = 'all'
  selectedVisibility.value = 'all'
}

const createCommunity = async () => {
  showCreateDialog.value = false
  newCommunity.value = {
    name: '',
    nameArabic: '',
    description: '',
    descriptionArabic: '',
    type: 'General',
    visibility: 'Public'
  }
}

// Load communities data
const LOADING_DELAY = 600

async function loadCommunities() {
  try {
    error.value = null
    loading.value = true

    await new Promise(resolve => setTimeout(resolve, LOADING_DELAY))

    communities.value = [
      {
        id: '1',
        name: 'Stadium Operations',
        nameArabic: 'عمليات الملاعب',
        description: 'Coordination hub for all stadium operations teams across all venues',
        descriptionArabic: 'مركز التنسيق لجميع فرق عمليات الملاعب في جميع الأماكن',
        slug: 'stadium-operations',
        type: 'WorkingGroup',
        visibility: 'Private',
        memberCount: 156,
        discussionCount: 89,
        isMember: true,
        recentMembers: [
          { name: 'Mohammed', avatarUrl: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Mohammed' },
          { name: 'Sara', avatarUrl: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Sara' },
          { name: 'Ahmed', avatarUrl: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Ahmed' }
        ],
        lastActivity: new Date(Date.now() - 1000 * 60 * 30)
      },
      {
        id: '2',
        name: 'Volunteer Community',
        nameArabic: 'مجتمع المتطوعين',
        description: 'Connect with fellow volunteers and stay updated on opportunities',
        descriptionArabic: 'تواصل مع زملائك المتطوعين وابق على اطلاع بالفرص',
        slug: 'volunteer-community',
        type: 'General',
        visibility: 'Public',
        memberCount: 2500,
        discussionCount: 456,
        isMember: true,
        recentMembers: [
          { name: 'Fatima', avatarUrl: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Fatima' },
          { name: 'Khalid', avatarUrl: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Khalid' },
          { name: 'Noura', avatarUrl: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Noura' }
        ],
        lastActivity: new Date(Date.now() - 1000 * 60 * 5)
      },
      {
        id: '3',
        name: 'IT & Technology',
        nameArabic: 'تقنية المعلومات',
        description: 'Technical discussions, support requests, and knowledge sharing',
        descriptionArabic: 'النقاشات التقنية وطلبات الدعم ومشاركة المعرفة',
        slug: 'it-technology',
        type: 'Department',
        visibility: 'Private',
        memberCount: 85,
        discussionCount: 234,
        isMember: true,
        recentMembers: [
          { name: 'Omar', avatarUrl: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Omar' },
          { name: 'Layla', avatarUrl: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Layla' }
        ],
        lastActivity: new Date(Date.now() - 1000 * 60 * 60 * 2)
      },
      {
        id: '4',
        name: 'Media & Communications',
        nameArabic: 'الإعلام والاتصالات',
        description: 'Media team collaboration space for press and communications',
        descriptionArabic: 'مساحة تعاون فريق الإعلام للصحافة والاتصالات',
        slug: 'media-communications',
        type: 'Department',
        visibility: 'Private',
        memberCount: 45,
        discussionCount: 123,
        isMember: false,
        lastActivity: new Date(Date.now() - 1000 * 60 * 60 * 24)
      },
      {
        id: '5',
        name: 'Sustainability Initiative',
        nameArabic: 'مبادرة الاستدامة',
        description: 'Green initiatives and sustainability projects for the tournament',
        descriptionArabic: 'المبادرات الخضراء ومشاريع الاستدامة للبطولة',
        slug: 'sustainability',
        type: 'Project',
        visibility: 'Public',
        memberCount: 67,
        discussionCount: 34,
        isMember: false,
        recentMembers: [
          { name: 'Hassan', avatarUrl: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Hassan' }
        ],
        lastActivity: new Date(Date.now() - 1000 * 60 * 60 * 48)
      },
      {
        id: '6',
        name: 'Security Operations',
        nameArabic: 'العمليات الأمنية',
        description: 'Security coordination and incident management',
        descriptionArabic: 'التنسيق الأمني وإدارة الحوادث',
        slug: 'security-operations',
        type: 'WorkingGroup',
        visibility: 'Secret',
        memberCount: 32,
        discussionCount: 78,
        isMember: false,
        lastActivity: new Date(Date.now() - 1000 * 60 * 60 * 12)
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
    console.error('Failed to load communities:', e)
  }
}

async function handleRetry() {
  await loadCommunities()
}

onMounted(() => {
  loadCommunities()
})
</script>

<template>
  <div class="communities-view" :class="{ rtl: isRTL }">
    <!-- Page Header -->
    <PageHeader
      :title="isRTL ? 'المجتمعات' : 'Communities'"
      :description="isRTL ? 'تواصل وتعاون مع الفرق والمجموعات عبر المنظمة' : 'Connect and collaborate with teams and groups across the organization'"
      :breadcrumbs="breadcrumbs"
      padding-bottom="xl"
      background-variant="gradient"
      :animated="shouldAnimate"
    >
      <template #actions>
        <Button
          :label="isRTL ? 'إنشاء مجتمع' : 'Create Community'"
          icon="pi pi-plus"
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

    <!-- Premium Toolbar -->
    <div class="toolbar">
      <div class="toolbar-row">
        <div class="toolbar-left">
          <!-- Search -->
          <div class="search-box">
            <i class="pi pi-search"></i>
            <InputText
              v-model="searchQuery"
              :placeholder="isRTL ? 'البحث في المجتمعات...' : 'Search communities...'"
              class="search-input"
            />
          </div>

          <!-- Filter Toggle -->
          <div class="type-toggle">
            <button
              v-for="option in filterToggleOptions"
              :key="option.value"
              :class="{ active: selectedFilter === option.value }"
              @click="selectedFilter = option.value as 'all' | 'my'"
            >
              <i :class="['pi', option.icon]"></i>
              <span>{{ option.label }}</span>
            </button>
          </div>

          <!-- Filter Button -->
          <button
            class="filter-btn"
            :class="{ active: showFilters, 'has-filters': activeFiltersCount > 0 }"
            @click="showFilters = !showFilters"
          >
            <i class="pi pi-filter"></i>
            <span>{{ isRTL ? 'تصفية' : 'Filters' }}</span>
            <span v-if="activeFiltersCount > 0" class="filter-badge">{{ activeFiltersCount }}</span>
          </button>
        </div>

        <div class="toolbar-right">
          <!-- View Toggle -->
          <div class="view-toggle">
            <button
              :class="{ active: viewMode === 'grid' }"
              @click="viewMode = 'grid'"
              :title="isRTL ? 'عرض شبكي' : 'Grid view'"
            >
              <i class="pi pi-th-large"></i>
            </button>
            <button
              :class="{ active: viewMode === 'list' }"
              @click="viewMode = 'list'"
              :title="isRTL ? 'عرض قائمة' : 'List view'"
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
          <label class="filter-label">{{ isRTL ? 'النوع' : 'Type' }}</label>
          <div class="filter-options">
            <button
              v-for="option in typeOptions"
              :key="option.value"
              :class="['filter-chip', { active: selectedType === option.value }]"
              @click="selectedType = option.value"
            >
              {{ option.label }}
            </button>
          </div>
        </div>

        <div class="filter-group">
          <label class="filter-label">{{ isRTL ? 'الرؤية' : 'Visibility' }}</label>
          <div class="filter-options">
            <button
              v-for="option in visibilityOptions"
              :key="option.value"
              :class="['filter-chip', { active: selectedVisibility === option.value }]"
              @click="selectedVisibility = option.value"
            >
              {{ option.label }}
            </button>
          </div>
        </div>

        <div class="filter-actions" v-if="activeFiltersCount > 0 || searchQuery">
          <button class="clear-filters-btn" @click="clearAllFilters">
            <i class="pi pi-times"></i>
            {{ isRTL ? 'مسح الكل' : 'Clear All' }}
          </button>
        </div>
        </div>
      </Transition>

      <!-- Active Filters Tags -->
      <div v-if="(activeFiltersCount > 0 || searchQuery) && !showFilters" class="active-filters">
        <span class="active-filters-label">{{ isRTL ? 'التصفية النشطة:' : 'Active filters:' }}</span>

        <span v-if="searchQuery" class="filter-tag">
          <i class="pi pi-search"></i>
          "{{ searchQuery }}"
          <button @click="searchQuery = ''"><i class="pi pi-times"></i></button>
        </span>

        <span v-if="selectedFilter !== 'all'" class="filter-tag">
          {{ filterToggleOptions.find(o => o.value === selectedFilter)?.label }}
          <button @click="selectedFilter = 'all'"><i class="pi pi-times"></i></button>
        </span>

        <span v-if="selectedType !== 'all'" class="filter-tag">
          {{ typeOptions.find(o => o.value === selectedType)?.label }}
          <button @click="selectedType = 'all'"><i class="pi pi-times"></i></button>
        </span>

        <span v-if="selectedVisibility !== 'all'" class="filter-tag">
          {{ visibilityOptions.find(o => o.value === selectedVisibility)?.label }}
          <button @click="selectedVisibility = 'all'"><i class="pi pi-times"></i></button>
        </span>

        <button class="clear-all-link" @click="clearAllFilters">
          {{ isRTL ? 'مسح الكل' : 'Clear all' }}
        </button>
      </div>
    </div>

    <!-- Content Area -->
    <div class="content-area">
      <!-- Loading State -->
      <div v-if="loading" class="loading-grid" :class="{ 'list-view': viewMode === 'list' }">
        <div v-for="i in 6" :key="i" class="skeleton-card">
          <Skeleton height="100px" class="skeleton-header" />
          <div class="skeleton-body">
            <Skeleton width="70%" height="20px" class="mb-2" />
            <Skeleton width="100%" height="14px" class="mb-2" />
            <Skeleton width="50%" height="14px" />
          </div>
        </div>
      </div>

      <!-- Error State -->
      <ErrorState
        v-else-if="error"
        :error="error"
        :title="isRTL ? 'فشل تحميل المجتمعات' : 'Failed to load communities'"
        show-retry
        @retry="handleRetry"
      />

      <!-- Empty State -->
      <div v-else-if="filteredCommunities.length === 0" class="empty-state">
        <div class="empty-icon">
          <i class="pi pi-users"></i>
        </div>
        <h3>{{ isRTL ? 'لا توجد مجتمعات' : 'No Communities Found' }}</h3>
        <p>{{ activeFiltersCount > 0 || searchQuery ? (isRTL ? 'جرب تعديل الفلاتر' : 'Try adjusting your filters') : (isRTL ? 'كن أول من ينشئ مجتمعاً!' : 'Be the first to create a community!') }}</p>
        <Button
          v-if="activeFiltersCount > 0 || searchQuery"
          :label="isRTL ? 'مسح الفلاتر' : 'Clear Filters'"
          icon="pi pi-times"
          outlined
          @click="clearAllFilters"
        />
        <Button
          v-else
          :label="isRTL ? 'إنشاء مجتمع' : 'Create Community'"
          icon="pi pi-plus"
          @click="showCreateDialog = true"
        />
      </div>

      <!-- Communities Grid/List -->
      <div
        v-else
        class="communities-grid"
        :class="{
          'list-view': viewMode === 'list',
          'communities-grid--animated': shouldAnimate,
          'communities-grid--visible': isContentVisible
        }"
      >
        <div
          v-for="(community, index) in filteredCommunities"
          :key="community.id"
          class="community-card"
          :class="{ 'is-member': community.isMember }"
          :style="shouldAnimate ? { '--card-index': index } : undefined"
          @click="navigateToCommunity(community)"
        >
          <!-- Card Header -->
          <div class="card-header" :style="{ background: getTypeConfig(community.type).bgColor }">
            <div class="community-icon" :style="{ color: getTypeConfig(community.type).color }">
              <i :class="getTypeConfig(community.type).icon"></i>
            </div>
            <div class="card-badges">
              <span v-if="community.isMember" class="member-badge">
                <i class="pi pi-check"></i>
                {{ isRTL ? 'عضو' : 'Member' }}
              </span>
              <span class="visibility-badge">
                <i :class="getVisibilityConfig(community.visibility).icon"></i>
              </span>
            </div>
          </div>

          <!-- Card Body -->
          <div class="card-body">
            <h3 class="card-title">{{ getName(community) }}</h3>
            <p class="card-description">{{ getDescription(community) }}</p>

            <div class="card-meta">
              <span class="type-tag" :style="{ color: getTypeConfig(community.type).color, background: getTypeConfig(community.type).bgColor }">
                {{ isRTL ? getTypeConfig(community.type).labelAr : getTypeConfig(community.type).label }}
              </span>
              <div class="stats">
                <span class="stat">
                  <i class="pi pi-users"></i>
                  {{ community.memberCount.toLocaleString() }}
                </span>
                <span class="stat">
                  <i class="pi pi-comments"></i>
                  {{ community.discussionCount }}
                </span>
              </div>
            </div>

            <!-- Recent Members -->
            <div v-if="community.recentMembers?.length" class="recent-members">
              <AvatarGroup>
                <Avatar
                  v-for="(member, idx) in community.recentMembers.slice(0, 3)"
                  :key="idx"
                  :image="member.avatarUrl"
                  :name="member.name"
                  shape="circle"
                  size="sm"
                />
                <Avatar
                  v-if="community.memberCount > 3"
                  :name="`+${community.memberCount - 3}`"
                  shape="circle"
                  size="sm"
                  class="overflow-avatar"
                />
              </AvatarGroup>
            </div>
          </div>

          <!-- Card Footer -->
          <div class="card-footer">
            <Button
              v-if="community.isMember"
              :label="isRTL ? 'عرض المجتمع' : 'View Community'"
              icon="pi pi-arrow-right"
              iconPos="right"
              text
              size="small"
              class="view-btn"
            />
            <Button
              v-else
              :label="isRTL ? 'انضمام' : 'Join'"
              icon="pi pi-user-plus"
              size="small"
              class="join-btn"
              @click="joinCommunity(community, $event)"
            />
          </div>
        </div>
      </div>
    </div>

    <!-- Create Community Dialog -->
    <Dialog
      v-model:visible="showCreateDialog"
      :header="isRTL ? 'إنشاء مجتمع جديد' : 'Create New Community'"
      :style="{ width: '500px' }"
      modal
      class="create-dialog"
    >
      <div class="create-form">
        <div class="form-field">
          <label>{{ isRTL ? 'اسم المجتمع' : 'Community Name' }} (English)</label>
          <InputText v-model="newCommunity.name" class="w-full" />
        </div>
        <div class="form-field">
          <label>{{ isRTL ? 'اسم المجتمع' : 'Community Name' }} (العربية)</label>
          <InputText v-model="newCommunity.nameArabic" class="w-full" dir="rtl" />
        </div>
        <div class="form-field">
          <label>{{ isRTL ? 'الوصف' : 'Description' }}</label>
          <Textarea v-model="newCommunity.description" rows="3" class="w-full" />
        </div>
        <div class="form-row">
          <div class="form-field">
            <label>{{ isRTL ? 'النوع' : 'Type' }}</label>
            <Dropdown
              v-model="newCommunity.type"
              :options="typeOptions.filter(o => o.value !== 'all')"
              optionLabel="label"
              optionValue="value"
              class="w-full"
            />
          </div>
          <div class="form-field">
            <label>{{ isRTL ? 'الرؤية' : 'Visibility' }}</label>
            <Dropdown
              v-model="newCommunity.visibility"
              :options="visibilityOptions.filter(o => o.value !== 'all')"
              optionLabel="label"
              optionValue="value"
              class="w-full"
            />
          </div>
        </div>
      </div>
      <template #footer>
        <Button
          :label="isRTL ? 'إلغاء' : 'Cancel'"
          severity="secondary"
          text
          @click="showCreateDialog = false"
        />
        <Button
          :label="isRTL ? 'إنشاء' : 'Create'"
          icon="pi pi-check"
          @click="createCommunity"
        />
      </template>
    </Dialog>
  </div>
</template>

<style lang="scss" scoped>
@use '@/assets/styles/_variables.scss' as *;
@use '@/assets/styles/_mixins.scss' as *;

// ============================================
// COMMUNITIES VIEW - MAIN CONTAINER
// Following Universal Spacing Guidelines from _mixins.scss
// ============================================

.communities-view {
  @include page-view;

  &.rtl {
    direction: rtl;
  }
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

.type-toggle {
  @include type-toggle;
}

.filter-btn {
  @include filter-btn;

  .filter-badge {
    @include filter-badge;
  }
}

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

.view-toggle {
  @include premium-view-toggle;
}

// ============================================
// CONTENT AREA
// ============================================

.content-area {
  @include content-area;
}

// Loading State
.loading-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
  gap: $spacing-5;

  &.list-view {
    grid-template-columns: 1fr;
  }
}

.skeleton-card {
  background: #fff;
  border-radius: $radius-xl;
  overflow: hidden;
  box-shadow: $shadow-card;

  .skeleton-header {
    border-radius: 0;
  }

  .skeleton-body {
    padding: $spacing-4;
  }
}

// Empty State
.empty-state {
  @include empty-state;
  padding: 4rem;
}

.empty-icon {
  @include empty-state-icon;
}

.empty-state h3 {
  @include empty-state-title;
}

.empty-state p {
  @include empty-state-text;
  margin-bottom: $spacing-4;
}

// ============================================
// COMMUNITY CARDS
// ============================================

.communities-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
  gap: $spacing-5;

  &.list-view {
    grid-template-columns: 1fr;

    .community-card {
      display: flex;
      flex-direction: row;

      .card-header {
        width: 120px;
        min-height: auto;
        border-radius: $radius-xl 0 0 $radius-xl;
      }

      .card-body {
        flex: 1;
        padding: $spacing-4;
      }

      .card-footer {
        display: flex;
        align-items: center;
        padding: $spacing-4;
        border-top: none;
        border-inline-start: 1px solid $slate-100;
      }

      .recent-members {
        display: none;
      }
    }
  }
}

.community-card {
  background: #fff;
  border-radius: $radius-xl;
  overflow: hidden;
  box-shadow: $shadow-card;
  cursor: pointer;
  transition: all $transition-base;
  border: 2px solid transparent;

  @include card-item-animation;

  &:hover {
    box-shadow: $shadow-card-hover;
    transform: translateY(-4px);
    border-color: $intalio-teal-200;

    .card-header .community-icon {
      transform: scale(1.1);
    }
  }

  &.is-member {
    .card-header {
      position: relative;

      &::after {
        content: '';
        position: absolute;
        bottom: 0;
        left: 0;
        right: 0;
        height: 3px;
        background: $success-500;
      }
    }
  }
}

.card-header {
  padding: $spacing-5;
  min-height: 100px;
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  position: relative;

  .community-icon {
    width: 56px;
    height: 56px;
    background: #fff;
    border-radius: $radius-lg;
    display: flex;
    align-items: center;
    justify-content: center;
    font-size: 1.5rem;
    box-shadow: $shadow-sm;
    transition: transform $transition-base;
  }

  .card-badges {
    display: flex;
    gap: $spacing-2;
  }

  .member-badge {
    display: inline-flex;
    align-items: center;
    gap: $spacing-1;
    padding: $spacing-1 $spacing-2;
    background: $success-500;
    color: #fff;
    font-size: $text-xs;
    font-weight: $font-weight-semibold;
    border-radius: $radius-full;
  }

  .visibility-badge {
    width: 28px;
    height: 28px;
    display: flex;
    align-items: center;
    justify-content: center;
    background: rgba(#fff, 0.9);
    border-radius: $radius-full;
    color: $slate-600;
    font-size: $text-sm;
  }
}

.card-body {
  padding: $spacing-5;

  .card-title {
    margin: 0 0 $spacing-2;
    font-size: $text-lg;
    font-weight: $font-weight-semibold;
    color: $slate-900;
  }

  .card-description {
    margin: 0 0 $spacing-4;
    font-size: $text-sm;
    color: $slate-600;
    line-height: 1.5;
    display: -webkit-box;
    -webkit-line-clamp: 2;
    -webkit-box-orient: vertical;
    overflow: hidden;
  }

  .card-meta {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: $spacing-4;

    .type-tag {
      padding: $spacing-1 $spacing-2;
      font-size: $text-xs;
      font-weight: $font-weight-medium;
      border-radius: $radius-md;
    }

    .stats {
      display: flex;
      gap: $spacing-3;

      .stat {
        display: flex;
        align-items: center;
        gap: $spacing-1;
        font-size: $text-sm;
        color: $slate-500;

        i {
          font-size: 0.875rem;
        }
      }
    }
  }

  .recent-members {
    padding-top: $spacing-4;
    border-top: 1px solid $slate-100;

    :deep(.p-avatar-group) {
      .p-avatar {
        border: 2px solid #fff;
        margin-inline-start: -8px;

        &:first-child {
          margin-inline-start: 0;
        }
      }

      .overflow-avatar {
        background: $slate-200;
        color: $slate-600;
        font-size: $text-xs;
        font-weight: $font-weight-semibold;
      }
    }
  }
}

.card-footer {
  padding: 0 $spacing-5 $spacing-5;
  display: flex;
  gap: $spacing-2;

  .view-btn {
    color: $intalio-teal-600;

    &:hover {
      background: $intalio-teal-50;
    }
  }

  .join-btn {
    background: $gradient-primary;
    border: none;

    &:hover {
      background: $gradient-primary-hover;
    }
  }
}

// Staggered animation for cards
@include staggered-animation-delays('.community-card', 12, 0.05s);

// ============================================
// CREATE DIALOG
// ============================================

.create-form {
  display: flex;
  flex-direction: column;
  gap: $spacing-4;

  .form-field {
    display: flex;
    flex-direction: column;
    gap: $spacing-2;

    label {
      font-size: $text-sm;
      font-weight: $font-weight-medium;
      color: $slate-700;
    }
  }

  .form-row {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: $spacing-4;
  }
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

  .type-toggle {
    overflow-x: auto;
    -webkit-overflow-scrolling: touch;

    &::-webkit-scrollbar {
      display: none;
    }
  }
}

@media (max-width: $breakpoint-md) {
  .communities-view {
    padding: $spacing-4;
  }

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

  .type-toggle {
    width: 100%;
    justify-content: center;

    button {
      flex: 1;
      justify-content: center;
    }
  }

  .filter-btn {
    width: 100%;
    justify-content: center;
  }

  .filters-panel {
    flex-direction: column;
  }

  .filter-group {
    width: 100%;
  }

  .communities-grid {
    grid-template-columns: 1fr;

    &.list-view .community-card {
      flex-direction: column;

      .card-header {
        width: 100%;
        border-radius: $radius-xl $radius-xl 0 0;
      }

      .card-footer {
        border-inline-start: none;
        border-top: 1px solid $slate-100;
      }
    }
  }

  .create-form .form-row {
    grid-template-columns: 1fr;
  }
}

// Animations
@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
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
// COMMUNITIES GRID STAGGER ANIMATION
// ============================================
.communities-grid {
  // Animation state - cards hidden initially
  &--animated {
    .community-card {
      opacity: 0;
      transform: translateY(24px) scale(0.96);
    }

    // When visible, animate cards in with stagger
    &.communities-grid--visible {
      .community-card {
        animation: cardStaggerFadeIn 0.5s ease-out forwards;
        animation-delay: calc(var(--card-index, 0) * 80ms);
      }
    }
  }

  // Ensure cards without animation are visible
  &:not(.communities-grid--animated) {
    .community-card {
      opacity: 1;
      transform: none;
      animation: none;
    }
  }
}

// ============================================
// ENHANCED CARD HOVER DEPTH
// ============================================
.community-card {
  &:hover {
    box-shadow:
      0 20px 40px -10px rgba(0, 0, 0, 0.15),
      0 10px 20px -5px rgba(0, 0, 0, 0.1),
      0 0 0 1px rgba($intalio-teal-500, 0.1);
    transform: translateY(-6px) scale(1.01);

    .card-header .community-icon {
      transform: scale(1.15) rotate(-3deg);
      box-shadow:
        0 8px 20px -4px rgba(0, 0, 0, 0.2),
        0 4px 8px -2px rgba(0, 0, 0, 0.1);
    }

    .card-title {
      color: $intalio-teal-600;
    }

    // Avatar stack expand on hover
    :deep(.p-avatar-group) {
      .p-avatar {
        margin-inline-start: -4px;
        transition: margin $transition-fast;

        &:first-child {
          margin-inline-start: 0;
        }
      }
    }
  }

  &:active {
    transform: translateY(-4px) scale(1);
    box-shadow:
      0 10px 20px -5px rgba(0, 0, 0, 0.12),
      0 5px 10px -3px rgba(0, 0, 0, 0.08);
  }
}

// ============================================
// REDUCED MOTION SUPPORT
// ============================================
@media (prefers-reduced-motion: reduce) {
  .communities-grid {
    &--animated {
      .community-card {
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

  .community-card {
    animation: none !important;
    transition: background-color $transition-fast, box-shadow $transition-fast, border-color $transition-fast;

    &:hover {
      transform: none;

      .card-header .community-icon {
        transform: none;
      }

      :deep(.p-avatar-group) .p-avatar {
        margin-inline-start: -8px;

        &:first-child {
          margin-inline-start: 0;
        }
      }
    }

    &:active {
      transform: none;
    }
  }

  .skeleton-card {
    animation: none !important;
  }
}
</style>
