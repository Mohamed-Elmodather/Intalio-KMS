<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import PageHeroHeader from '@/components/common/PageHeroHeader.vue'

const router = useRouter()

// ============================================
// CORE STATE
// ============================================
const searchQuery = ref('')
const selectedType = ref('all')
const sortBy = ref('recent')
const showSortDropdown = ref(false)
const showCreateModal = ref(false)

// Create Space form state
const newSpace = ref({
  nameEn: '',
  nameAr: '',
  description: '',
  type: 'team',
  isPublic: true,
  icon: 'fas fa-folder',
  color: '#14b8a6'
})

// ============================================
// DATA
// ============================================
const spaceTypes = ref([
  { id: 'all', label: 'All Spaces', icon: 'fas fa-globe' },
  { id: 'personal', label: 'Personal', icon: 'fas fa-user' },
  { id: 'team', label: 'Team', icon: 'fas fa-users' },
  { id: 'department', label: 'Department', icon: 'fas fa-building' },
  { id: 'project', label: 'Project', icon: 'fas fa-project-diagram' },
  { id: 'public', label: 'Public', icon: 'fas fa-globe-americas' }
])

const sortOptions = ref([
  { value: 'recent', label: 'Recently Active', icon: 'fas fa-clock' },
  { value: 'name', label: 'Name A-Z', icon: 'fas fa-sort-alpha-down' },
  { value: 'members', label: 'Most Members', icon: 'fas fa-users' },
  { value: 'content', label: 'Most Content', icon: 'fas fa-file-alt' }
])

const currentSortOption = computed(() => {
  return sortOptions.value.find(opt => opt.value === sortBy.value) ?? sortOptions.value[0]!
})

const iconOptions = [
  'fas fa-folder', 'fas fa-users', 'fas fa-building', 'fas fa-cogs',
  'fas fa-shield-halved', 'fas fa-calendar-alt', 'fas fa-laptop-code',
  'fas fa-book-open', 'fas fa-chart-bar', 'fas fa-lightbulb',
  'fas fa-heart', 'fas fa-rocket', 'fas fa-globe', 'fas fa-star',
  'fas fa-flag', 'fas fa-bell'
]

const colorOptions = [
  '#14b8a6', '#3b82f6', '#8b5cf6', '#ec4899',
  '#f59e0b', '#ef4444', '#10b981', '#6366f1',
  '#f97316', '#06b6d4', '#84cc16', '#a855f7'
]

const spaces = ref([
  {
    id: 1,
    name: 'HR Portal',
    nameAr: '\u0628\u0648\u0627\u0628\u0629 \u0627\u0644\u0645\u0648\u0627\u0631\u062f \u0627\u0644\u0628\u0634\u0631\u064a\u0629',
    description: 'Human resources policies, benefits information, onboarding guides, and employee wellness resources.',
    type: 'department',
    isPublic: true,
    icon: 'fas fa-users',
    color: '#8b5cf6',
    members: 156,
    articles: 48,
    documents: 32,
    lastActivity: '2h ago',
    owner: { name: 'Lisa Wong', initials: 'LW' },
    memberAvatars: [
      { initials: 'LW', color: '#8b5cf6' },
      { initials: 'SC', color: '#3b82f6' },
      { initials: 'MJ', color: '#10b981' },
      { initials: 'RG', color: '#ec4899' }
    ],
    isFavorite: true
  },
  {
    id: 2,
    name: 'Engineering Hub',
    nameAr: '\u0645\u0631\u0643\u0632 \u0627\u0644\u0647\u0646\u062f\u0633\u0629',
    description: 'Technical documentation, architecture decisions, coding standards, and engineering best practices.',
    type: 'team',
    isPublic: false,
    icon: 'fas fa-laptop-code',
    color: '#3b82f6',
    members: 89,
    articles: 124,
    documents: 67,
    lastActivity: '30m ago',
    owner: { name: 'Alex Thompson', initials: 'AT' },
    memberAvatars: [
      { initials: 'AT', color: '#3b82f6' },
      { initials: 'DK', color: '#14b8a6' },
      { initials: 'ED', color: '#f59e0b' },
      { initials: 'JR', color: '#ef4444' }
    ],
    isFavorite: true
  },
  {
    id: 3,
    name: 'Operations Center',
    nameAr: '\u0645\u0631\u0643\u0632 \u0627\u0644\u0639\u0645\u0644\u064a\u0627\u062a',
    description: 'Operational procedures, workflow documentation, venue management, and logistics coordination.',
    type: 'department',
    isPublic: true,
    icon: 'fas fa-cogs',
    color: '#f59e0b',
    members: 72,
    articles: 36,
    documents: 54,
    lastActivity: '1h ago',
    owner: { name: 'Mike Johnson', initials: 'MJ' },
    memberAvatars: [
      { initials: 'MJ', color: '#f59e0b' },
      { initials: 'SC', color: '#8b5cf6' },
      { initials: 'NP', color: '#3b82f6' }
    ],
    isFavorite: false
  },
  {
    id: 4,
    name: 'Safety & Compliance',
    nameAr: '\u0627\u0644\u0633\u0644\u0627\u0645\u0629 \u0648\u0627\u0644\u0627\u0645\u062a\u062b\u0627\u0644',
    description: 'Safety protocols, regulatory compliance documents, risk assessments, and audit guidelines.',
    type: 'project',
    isPublic: false,
    icon: 'fas fa-shield-halved',
    color: '#ef4444',
    members: 45,
    articles: 28,
    documents: 41,
    lastActivity: '4h ago',
    owner: { name: 'Emily Davis', initials: 'ED' },
    memberAvatars: [
      { initials: 'ED', color: '#ef4444' },
      { initials: 'LW', color: '#8b5cf6' },
      { initials: 'AT', color: '#3b82f6' }
    ],
    isFavorite: false
  },
  {
    id: 5,
    name: 'Event Planning',
    nameAr: '\u062a\u062e\u0637\u064a\u0637 \u0627\u0644\u0641\u0639\u0627\u0644\u064a\u0627\u062a',
    description: 'Event schedules, planning templates, vendor management, and ceremony coordination resources.',
    type: 'project',
    isPublic: true,
    icon: 'fas fa-calendar-alt',
    color: '#ec4899',
    members: 63,
    articles: 19,
    documents: 28,
    lastActivity: '45m ago',
    owner: { name: 'Sarah Chen', initials: 'SC' },
    memberAvatars: [
      { initials: 'SC', color: '#ec4899' },
      { initials: 'MJ', color: '#f59e0b' },
      { initials: 'RG', color: '#10b981' },
      { initials: 'JR', color: '#3b82f6' }
    ],
    isFavorite: true
  },
  {
    id: 6,
    name: 'IT Knowledge Base',
    nameAr: '\u0642\u0627\u0639\u062f\u0629 \u0645\u0639\u0627\u0631\u0641 \u062a\u0643\u0646\u0648\u0644\u0648\u062c\u064a\u0627 \u0627\u0644\u0645\u0639\u0644\u0648\u0645\u0627\u062a',
    description: 'IT support guides, troubleshooting documentation, system administration, and infrastructure resources.',
    type: 'team',
    isPublic: true,
    icon: 'fas fa-book-open',
    color: '#14b8a6',
    members: 112,
    articles: 87,
    documents: 45,
    lastActivity: '15m ago',
    owner: { name: 'David Kim', initials: 'DK' },
    memberAvatars: [
      { initials: 'DK', color: '#14b8a6' },
      { initials: 'AT', color: '#3b82f6' },
      { initials: 'ED', color: '#f59e0b' }
    ],
    isFavorite: false
  }
])

// ============================================
// COMPUTED
// ============================================
const heroStats = computed(() => [
  { icon: 'fas fa-cubes', value: spaces.value.length, label: 'Total Spaces' },
  { icon: 'fas fa-star', value: spaces.value.filter(s => s.isFavorite).length, label: 'My Spaces' },
  { icon: 'fas fa-globe', value: spaces.value.filter(s => s.isPublic).length, label: 'Public' },
  { icon: 'fas fa-file-alt', value: totalContent.value, label: 'Content Items' }
])

const totalContent = computed(() => {
  return spaces.value.reduce((sum, s) => sum + s.articles + s.documents, 0)
})

const filteredSpaces = computed(() => {
  let result = [...spaces.value]

  // Filter by type
  if (selectedType.value !== 'all') {
    if (selectedType.value === 'public') {
      result = result.filter(s => s.isPublic)
    } else {
      result = result.filter(s => s.type === selectedType.value)
    }
  }

  // Filter by search
  if (searchQuery.value.trim()) {
    const q = searchQuery.value.toLowerCase()
    result = result.filter(s =>
      s.name.toLowerCase().includes(q) ||
      s.description.toLowerCase().includes(q)
    )
  }

  // Sort
  switch (sortBy.value) {
    case 'name':
      result.sort((a, b) => a.name.localeCompare(b.name))
      break
    case 'members':
      result.sort((a, b) => b.members - a.members)
      break
    case 'content':
      result.sort((a, b) => (b.articles + b.documents) - (a.articles + a.documents))
      break
    case 'recent':
    default:
      // Keep original order (simulates recent activity)
      break
  }

  return result
})

// ============================================
// METHODS
// ============================================
function selectSortOption(value: string) {
  sortBy.value = value
  showSortDropdown.value = false
}

function toggleFavorite(spaceId: number) {
  const space = spaces.value.find(s => s.id === spaceId)
  if (space) {
    space.isFavorite = !space.isFavorite
  }
}

function openSpace(spaceId: number) {
  // Navigate to space detail - future route
  console.log('Open space:', spaceId)
}

function getTypeBadgeClass(type: string) {
  const classes: Record<string, string> = {
    personal: 'badge-personal',
    team: 'badge-team',
    department: 'badge-department',
    project: 'badge-project',
    public: 'badge-public'
  }
  return classes[type] || 'badge-team'
}

function getTypeLabel(type: string) {
  const labels: Record<string, string> = {
    personal: 'Personal',
    team: 'Team',
    department: 'Department',
    project: 'Project',
    public: 'Public'
  }
  return labels[type] || type
}

function formatNumber(num: number) {
  if (num >= 1000) return (num / 1000).toFixed(1) + 'k'
  return num.toString()
}

function openCreateModal() {
  newSpace.value = {
    nameEn: '',
    nameAr: '',
    description: '',
    type: 'team',
    isPublic: true,
    icon: 'fas fa-folder',
    color: '#14b8a6'
  }
  showCreateModal.value = true
}

function closeCreateModal() {
  showCreateModal.value = false
}

function handleCreateSpace() {
  // Mock create - would call API
  const id = spaces.value.length + 1
  spaces.value.unshift({
    id,
    name: newSpace.value.nameEn,
    nameAr: newSpace.value.nameAr,
    description: newSpace.value.description,
    type: newSpace.value.type,
    isPublic: newSpace.value.isPublic,
    icon: newSpace.value.icon,
    color: newSpace.value.color,
    members: 1,
    articles: 0,
    documents: 0,
    lastActivity: 'Just now',
    owner: { name: 'Ahmed Imam', initials: 'AI' },
    memberAvatars: [{ initials: 'AI', color: newSpace.value.color }],
    isFavorite: false
  })
  closeCreateModal()
}
</script>

<template>
  <div class="page-view">
    <!-- Hero Section -->
    <PageHeroHeader
      :stats="heroStats"
      badge-icon="fas fa-cubes"
      badge-text="AFC Asian Cup 2027"
      title="Spaces"
      subtitle="Organize knowledge into dedicated spaces for teams, departments, and projects."
    >
      <template #actions>
        <button @click="openCreateModal" class="px-5 py-2.5 bg-white text-teal-600 rounded-xl font-semibold text-sm flex items-center gap-2 hover:bg-teal-50 transition-all shadow-lg">
          <i class="fas fa-plus"></i>
          Create Space
        </button>
        <button class="px-5 py-2.5 bg-white/20 backdrop-blur-sm border border-white/30 text-white rounded-xl font-semibold text-sm hover:bg-white/30 transition-all flex items-center gap-2">
          <i class="fas fa-star"></i>
          My Spaces
        </button>
      </template>
    </PageHeroHeader>

    <!-- Main Content -->
    <div class="px-6 pt-6">

      <!-- Toolbar -->
      <div class="toolbar mb-6 fade-in-up">
        <div class="toolbar-left">
          <!-- Search -->
          <div class="search-box">
            <i class="fas fa-search"></i>
            <input
              v-model="searchQuery"
              type="text"
              placeholder="Search spaces..."
              class="search-input"
            />
          </div>

          <!-- Type Filter Pills -->
          <div class="type-filters">
            <button
              v-for="type in spaceTypes"
              :key="type.id"
              @click="selectedType = type.id"
              class="type-pill"
              :class="{ active: selectedType === type.id }"
            >
              <i :class="type.icon" class="text-xs"></i>
              {{ type.label }}
            </button>
          </div>
        </div>

        <div class="toolbar-right">
          <!-- Sort Dropdown -->
          <div class="relative">
            <button
              @click="showSortDropdown = !showSortDropdown"
              class="sort-btn"
            >
              <i :class="currentSortOption.icon" class="text-sm"></i>
              <span class="hidden sm:inline">{{ currentSortOption.label }}</span>
              <i class="fas fa-chevron-down text-xs transition-transform" :class="{ 'rotate-180': showSortDropdown }"></i>
            </button>
            <Transition name="dropdown">
              <div v-if="showSortDropdown" class="sort-dropdown">
                <button
                  v-for="option in sortOptions"
                  :key="option.value"
                  @click="selectSortOption(option.value)"
                  class="sort-option"
                  :class="{ active: sortBy === option.value }"
                >
                  <i :class="option.icon" class="w-5 text-center"></i>
                  {{ option.label }}
                  <i v-if="sortBy === option.value" class="fas fa-check text-teal-500 ms-auto"></i>
                </button>
              </div>
            </Transition>
          </div>

          <!-- Create Button (compact) -->
          <button @click="openCreateModal" class="create-btn-compact">
            <i class="fas fa-plus"></i>
            <span class="hidden sm:inline">New Space</span>
          </button>
        </div>
      </div>

      <!-- Spaces Grid -->
      <div class="spaces-grid fade-in-up" style="animation-delay: 0.1s">
        <div
          v-for="space in filteredSpaces"
          :key="space.id"
          class="space-card group"
          @click="openSpace(space.id)"
        >
          <!-- Card Header -->
          <div class="space-card-header" :style="{ background: `linear-gradient(135deg, ${space.color}15, ${space.color}08)` }">
            <div class="space-icon-wrapper" :style="{ background: `${space.color}20`, color: space.color }">
              <i :class="space.icon" class="text-xl"></i>
            </div>
            <div class="space-card-actions">
              <button
                @click.stop="toggleFavorite(space.id)"
                class="favorite-btn"
                :class="{ 'is-favorite': space.isFavorite }"
              >
                <i :class="space.isFavorite ? 'fas fa-star' : 'far fa-star'"></i>
              </button>
              <span v-if="!space.isPublic" class="lock-badge" title="Private space">
                <i class="fas fa-lock text-xs"></i>
              </span>
            </div>
          </div>

          <!-- Card Body -->
          <div class="space-card-body">
            <div class="flex items-start justify-between gap-2 mb-2">
              <h3 class="space-card-title group-hover:text-teal-600 transition-colors">{{ space.name }}</h3>
              <span class="type-badge" :class="getTypeBadgeClass(space.type)">{{ getTypeLabel(space.type) }}</span>
            </div>
            <p class="space-card-desc">{{ space.description }}</p>

            <!-- Stats Row -->
            <div class="space-stats">
              <div class="space-stat">
                <i class="fas fa-file-alt text-teal-500"></i>
                <span>{{ space.articles }} articles</span>
              </div>
              <div class="space-stat">
                <i class="fas fa-folder text-blue-500"></i>
                <span>{{ space.documents }} docs</span>
              </div>
              <div class="space-stat">
                <i class="fas fa-users text-purple-500"></i>
                <span>{{ formatNumber(space.members) }}</span>
              </div>
            </div>
          </div>

          <!-- Card Footer -->
          <div class="space-card-footer">
            <div class="member-avatars">
              <div
                v-for="(avatar, idx) in space.memberAvatars.slice(0, 3)"
                :key="idx"
                class="member-avatar"
                :style="{ background: avatar.color, zIndex: 3 - idx }"
              >
                {{ avatar.initials }}
              </div>
              <div v-if="space.members > 3" class="member-avatar member-more">
                +{{ space.members - 3 }}
              </div>
            </div>
            <div class="activity-time">
              <i class="fas fa-clock"></i>
              {{ space.lastActivity }}
            </div>
          </div>
        </div>
      </div>

      <!-- Empty State -->
      <div v-if="filteredSpaces.length === 0" class="empty-state fade-in-up">
        <div class="empty-icon">
          <i class="fas fa-cubes"></i>
        </div>
        <h3>No spaces found</h3>
        <p>Try adjusting your filters or search query, or create a new space.</p>
        <button @click="openCreateModal" class="empty-create-btn">
          <i class="fas fa-plus"></i>
          Create Space
        </button>
      </div>
    </div>

    <!-- Create Space Modal -->
    <Transition name="modal">
      <div v-if="showCreateModal" class="modal-overlay" @click.self="closeCreateModal">
        <div class="modal-container">
          <!-- Modal Header -->
          <div class="modal-header">
            <div class="modal-header-icon">
              <i class="fas fa-cubes"></i>
            </div>
            <div>
              <h2 class="modal-title">Create New Space</h2>
              <p class="modal-subtitle">Set up a dedicated area for your team's knowledge</p>
            </div>
            <button @click="closeCreateModal" class="modal-close">
              <i class="fas fa-times"></i>
            </button>
          </div>

          <!-- Modal Body -->
          <div class="modal-body">
            <!-- Name EN -->
            <div class="form-group">
              <label class="form-label">Space Name (English)</label>
              <input
                v-model="newSpace.nameEn"
                type="text"
                class="form-input"
                placeholder="e.g., Marketing Hub"
              />
            </div>

            <!-- Name AR -->
            <div class="form-group">
              <label class="form-label">Space Name (Arabic)</label>
              <input
                v-model="newSpace.nameAr"
                type="text"
                class="form-input"
                dir="rtl"
                placeholder="\u0645\u062b\u0627\u0644: \u0645\u0631\u0643\u0632 \u0627\u0644\u062a\u0633\u0648\u064a\u0642"
              />
            </div>

            <!-- Description -->
            <div class="form-group">
              <label class="form-label">Description</label>
              <textarea
                v-model="newSpace.description"
                class="form-textarea"
                rows="3"
                placeholder="Describe the purpose of this space..."
              ></textarea>
            </div>

            <!-- Type Selector -->
            <div class="form-group">
              <label class="form-label">Space Type</label>
              <div class="type-selector">
                <button
                  v-for="type in spaceTypes.filter(t => t.id !== 'all')"
                  :key="type.id"
                  @click="newSpace.type = type.id"
                  class="type-select-btn"
                  :class="{ active: newSpace.type === type.id }"
                >
                  <i :class="type.icon"></i>
                  {{ type.label }}
                </button>
              </div>
            </div>

            <!-- Public Toggle -->
            <div class="form-group">
              <div class="toggle-row">
                <div>
                  <label class="form-label mb-0">Public Space</label>
                  <p class="form-hint">Anyone in the organization can discover and join</p>
                </div>
                <button
                  @click="newSpace.isPublic = !newSpace.isPublic"
                  class="toggle-switch"
                  :class="{ active: newSpace.isPublic }"
                >
                  <span class="toggle-knob"></span>
                </button>
              </div>
            </div>

            <!-- Icon Picker -->
            <div class="form-group">
              <label class="form-label">Icon</label>
              <div class="icon-picker">
                <button
                  v-for="icon in iconOptions"
                  :key="icon"
                  @click="newSpace.icon = icon"
                  class="icon-pick-btn"
                  :class="{ active: newSpace.icon === icon }"
                >
                  <i :class="icon"></i>
                </button>
              </div>
            </div>

            <!-- Color Picker -->
            <div class="form-group">
              <label class="form-label">Color</label>
              <div class="color-picker">
                <button
                  v-for="color in colorOptions"
                  :key="color"
                  @click="newSpace.color = color"
                  class="color-pick-btn"
                  :class="{ active: newSpace.color === color }"
                  :style="{ background: color }"
                >
                  <i v-if="newSpace.color === color" class="fas fa-check text-white text-xs"></i>
                </button>
              </div>
            </div>

            <!-- Preview -->
            <div class="form-group">
              <label class="form-label">Preview</label>
              <div class="space-preview" :style="{ borderColor: newSpace.color + '40' }">
                <div class="preview-icon" :style="{ background: newSpace.color + '20', color: newSpace.color }">
                  <i :class="newSpace.icon" class="text-lg"></i>
                </div>
                <div>
                  <h4 class="text-sm font-semibold text-gray-900">{{ newSpace.nameEn || 'Space Name' }}</h4>
                  <p class="text-xs text-gray-500 mt-0.5">{{ newSpace.description || 'Space description...' }}</p>
                </div>
              </div>
            </div>
          </div>

          <!-- Modal Footer -->
          <div class="modal-footer">
            <button @click="closeCreateModal" class="btn-cancel">Cancel</button>
            <button
              @click="handleCreateSpace"
              class="btn-create"
              :disabled="!newSpace.nameEn.trim()"
            >
              <i class="fas fa-plus"></i>
              Create Space
            </button>
          </div>
        </div>
      </div>
    </Transition>
  </div>
</template>

<style scoped>
/* ============================================
   PAGE VIEW
   ============================================ */
.page-view {
  padding: 0;
  min-height: 100vh;
  background: linear-gradient(180deg, #f0fdfa 0%, #f8fafc 15%, #ffffff 100%);
}

/* ============================================
   TOOLBAR
   ============================================ */
.toolbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 1rem;
  flex-wrap: wrap;
  animation: fadeInUp 0.4s ease-out;
}

.toolbar-left {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  flex-wrap: wrap;
}

.toolbar-right {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

/* Search */
.search-box {
  position: relative;
  width: 280px;
}

.search-box i {
  position: absolute;
  left: 1rem;
  top: 50%;
  transform: translateY(-50%);
  color: #94a3b8;
  font-size: 0.875rem;
  transition: all 0.3s ease;
  z-index: 1;
}

[dir="rtl"] .search-box i {
  left: auto;
  right: 1rem;
}

.search-box:focus-within i {
  color: #14b8a6;
  transform: translateY(-50%) scale(1.1);
}

.search-input {
  width: 100%;
  height: 42px;
  padding-left: 2.75rem;
  padding-right: 1rem;
  border-radius: 0.75rem;
  border: 1.5px solid #e2e8f0;
  background: linear-gradient(135deg, #fafafa 0%, #f8fafc 100%);
  font-size: 0.875rem;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  outline: none;
}

[dir="rtl"] .search-input {
  padding-left: 1rem;
  padding-right: 2.75rem;
}

.search-input:focus {
  border-color: #14b8a6;
  box-shadow: 0 0 0 4px rgba(20, 184, 166, 0.1), 0 4px 12px rgba(20, 184, 166, 0.1);
  background: white;
}

/* Type Filter Pills */
.type-filters {
  display: flex;
  gap: 0.5rem;
  flex-wrap: wrap;
}

.type-pill {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  padding: 0.375rem 0.875rem;
  border-radius: 2rem;
  font-size: 0.8125rem;
  font-weight: 500;
  border: 1.5px solid #e2e8f0;
  background: white;
  color: #64748b;
  cursor: pointer;
  transition: all 0.25s ease;
  white-space: nowrap;
}

.type-pill:hover {
  border-color: #14b8a6;
  color: #0d9488;
  background: #f0fdfa;
}

.type-pill.active {
  background: linear-gradient(135deg, #14b8a6, #0d9488);
  color: white;
  border-color: #14b8a6;
  box-shadow: 0 2px 8px rgba(20, 184, 166, 0.3);
}

/* Sort Button */
.sort-btn {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem 1rem;
  border-radius: 0.75rem;
  border: 1.5px solid #e2e8f0;
  background: white;
  font-size: 0.875rem;
  color: #64748b;
  cursor: pointer;
  transition: all 0.2s ease;
}

.sort-btn:hover {
  border-color: #14b8a6;
  color: #0d9488;
}

.sort-dropdown {
  position: absolute;
  top: calc(100% + 0.5rem);
  right: 0;
  min-width: 200px;
  background: white;
  border-radius: 0.75rem;
  border: 1px solid #e2e8f0;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.1);
  z-index: 50;
  padding: 0.375rem;
  overflow: hidden;
}

[dir="rtl"] .sort-dropdown {
  right: auto;
  left: 0;
}

.sort-option {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  width: 100%;
  padding: 0.625rem 0.875rem;
  border-radius: 0.5rem;
  font-size: 0.875rem;
  color: #64748b;
  cursor: pointer;
  transition: all 0.15s ease;
  border: none;
  background: none;
  text-align: start;
}

.sort-option:hover {
  background: #f0fdfa;
  color: #0d9488;
}

.sort-option.active {
  background: #f0fdfa;
  color: #0d9488;
  font-weight: 600;
}

/* Create Button */
.create-btn-compact {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem 1rem;
  border-radius: 0.75rem;
  background: linear-gradient(135deg, #14b8a6, #0d9488);
  color: white;
  font-size: 0.875rem;
  font-weight: 600;
  border: none;
  cursor: pointer;
  transition: all 0.25s ease;
  box-shadow: 0 2px 8px rgba(20, 184, 166, 0.3);
}

.create-btn-compact:hover {
  transform: translateY(-1px);
  box-shadow: 0 4px 16px rgba(20, 184, 166, 0.4);
}

/* ============================================
   SPACES GRID
   ============================================ */
.spaces-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(360px, 1fr));
  gap: 1.5rem;
  padding-bottom: 2rem;
}

@media (max-width: 768px) {
  .spaces-grid {
    grid-template-columns: 1fr;
  }
}

/* Space Card */
.space-card {
  background: white;
  border-radius: 1rem;
  border: 1px solid #e2e8f0;
  overflow: hidden;
  cursor: pointer;
  transition: all 0.35s cubic-bezier(0.4, 0, 0.2, 1);
  position: relative;
}

.space-card::before {
  content: '';
  position: absolute;
  inset: 0;
  border-radius: 1rem;
  padding: 1px;
  background: linear-gradient(135deg, transparent, transparent);
  -webkit-mask: linear-gradient(#fff 0 0) content-box, linear-gradient(#fff 0 0);
  -webkit-mask-composite: xor;
  mask-composite: exclude;
  transition: all 0.35s ease;
  pointer-events: none;
}

.space-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 12px 40px rgba(20, 184, 166, 0.12), 0 4px 12px rgba(0, 0, 0, 0.05);
  border-color: rgba(20, 184, 166, 0.3);
}

.space-card:hover::before {
  background: linear-gradient(135deg, #14b8a6, #0d9488);
}

.space-card-header {
  padding: 1.25rem 1.25rem 0.75rem;
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
}

.space-icon-wrapper {
  width: 52px;
  height: 52px;
  border-radius: 0.875rem;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.3s ease;
}

.space-card:hover .space-icon-wrapper {
  transform: scale(1.08);
}

.space-card-actions {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.favorite-btn {
  width: 32px;
  height: 32px;
  border-radius: 0.5rem;
  display: flex;
  align-items: center;
  justify-content: center;
  border: none;
  background: transparent;
  color: #cbd5e1;
  cursor: pointer;
  transition: all 0.25s ease;
  font-size: 0.875rem;
}

.favorite-btn:hover {
  color: #f59e0b;
  background: #fef3c7;
}

.favorite-btn.is-favorite {
  color: #f59e0b;
}

.lock-badge {
  width: 28px;
  height: 28px;
  border-radius: 0.375rem;
  display: flex;
  align-items: center;
  justify-content: center;
  background: #fef2f2;
  color: #ef4444;
}

.space-card-body {
  padding: 0 1.25rem 1rem;
}

.space-card-title {
  font-size: 1.0625rem;
  font-weight: 700;
  color: #0f172a;
  line-height: 1.3;
  margin: 0;
}

.space-card-desc {
  font-size: 0.8125rem;
  color: #64748b;
  line-height: 1.5;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
  margin-bottom: 0.875rem;
}

/* Type Badges */
.type-badge {
  padding: 0.1875rem 0.625rem;
  border-radius: 2rem;
  font-size: 0.6875rem;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.03em;
  white-space: nowrap;
  flex-shrink: 0;
}

.badge-personal {
  background: #ede9fe;
  color: #7c3aed;
}

.badge-team {
  background: #dbeafe;
  color: #2563eb;
}

.badge-department {
  background: #d1fae5;
  color: #059669;
}

.badge-project {
  background: #fce7f3;
  color: #db2777;
}

.badge-public {
  background: #ccfbf1;
  color: #0d9488;
}

/* Space Stats */
.space-stats {
  display: flex;
  gap: 1rem;
  flex-wrap: wrap;
}

.space-stat {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  font-size: 0.8125rem;
  color: #64748b;
}

.space-stat i {
  font-size: 0.75rem;
}

/* Card Footer */
.space-card-footer {
  padding: 0.875rem 1.25rem;
  border-top: 1px solid #f1f5f9;
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.member-avatars {
  display: flex;
  align-items: center;
}

.member-avatar {
  width: 28px;
  height: 28px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.625rem;
  font-weight: 700;
  color: white;
  border: 2px solid white;
  margin-inline-start: -8px;
  position: relative;
}

.member-avatar:first-child {
  margin-inline-start: 0;
}

.member-more {
  background: #e2e8f0;
  color: #64748b;
  font-size: 0.5625rem;
}

.activity-time {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  font-size: 0.75rem;
  color: #94a3b8;
}

.activity-time i {
  font-size: 0.6875rem;
}

/* ============================================
   EMPTY STATE
   ============================================ */
.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 4rem 2rem;
  text-align: center;
}

.empty-icon {
  width: 80px;
  height: 80px;
  border-radius: 50%;
  background: linear-gradient(135deg, #f0fdfa, #ccfbf1);
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 1.5rem;
  font-size: 2rem;
  color: #14b8a6;
}

.empty-state h3 {
  font-size: 1.25rem;
  font-weight: 700;
  color: #0f172a;
  margin-bottom: 0.5rem;
}

.empty-state p {
  font-size: 0.9375rem;
  color: #64748b;
  max-width: 400px;
  margin-bottom: 1.5rem;
}

.empty-create-btn {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem 1.5rem;
  border-radius: 0.75rem;
  background: linear-gradient(135deg, #14b8a6, #0d9488);
  color: white;
  font-size: 0.9375rem;
  font-weight: 600;
  border: none;
  cursor: pointer;
  transition: all 0.25s ease;
  box-shadow: 0 4px 16px rgba(20, 184, 166, 0.3);
}

.empty-create-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 24px rgba(20, 184, 166, 0.4);
}

/* ============================================
   MODAL
   ============================================ */
.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.5);
  backdrop-filter: blur(4px);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 100;
  padding: 1rem;
}

.modal-container {
  background: white;
  border-radius: 1.25rem;
  width: 100%;
  max-width: 580px;
  max-height: 90vh;
  overflow: hidden;
  display: flex;
  flex-direction: column;
  box-shadow: 0 25px 80px rgba(0, 0, 0, 0.15);
}

.modal-header {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 1.5rem 1.5rem 1rem;
  border-bottom: 1px solid #f1f5f9;
}

.modal-header-icon {
  width: 44px;
  height: 44px;
  border-radius: 0.75rem;
  background: linear-gradient(135deg, #14b8a6, #0d9488);
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 1.125rem;
  flex-shrink: 0;
}

.modal-title {
  font-size: 1.25rem;
  font-weight: 700;
  color: #0f172a;
  margin: 0;
}

.modal-subtitle {
  font-size: 0.8125rem;
  color: #64748b;
  margin: 0.125rem 0 0;
}

.modal-close {
  width: 36px;
  height: 36px;
  border-radius: 0.5rem;
  display: flex;
  align-items: center;
  justify-content: center;
  border: none;
  background: #f1f5f9;
  color: #64748b;
  cursor: pointer;
  margin-inline-start: auto;
  transition: all 0.2s ease;
}

.modal-close:hover {
  background: #fee2e2;
  color: #ef4444;
}

.modal-body {
  padding: 1.5rem;
  overflow-y: auto;
  flex: 1;
}

.modal-footer {
  display: flex;
  justify-content: flex-end;
  gap: 0.75rem;
  padding: 1rem 1.5rem;
  border-top: 1px solid #f1f5f9;
}

/* Form Elements */
.form-group {
  margin-bottom: 1.25rem;
}

.form-label {
  display: block;
  font-size: 0.8125rem;
  font-weight: 600;
  color: #374151;
  margin-bottom: 0.5rem;
}

.form-hint {
  font-size: 0.75rem;
  color: #94a3b8;
  margin: 0.125rem 0 0;
}

.form-input,
.form-textarea {
  width: 100%;
  padding: 0.625rem 0.875rem;
  border-radius: 0.75rem;
  border: 1.5px solid #e2e8f0;
  font-size: 0.875rem;
  transition: all 0.2s ease;
  outline: none;
  background: #fafafa;
}

.form-input:focus,
.form-textarea:focus {
  border-color: #14b8a6;
  box-shadow: 0 0 0 3px rgba(20, 184, 166, 0.1);
  background: white;
}

.form-textarea {
  resize: vertical;
  min-height: 80px;
}

/* Type Selector */
.type-selector {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
}

.type-select-btn {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  padding: 0.5rem 0.875rem;
  border-radius: 0.625rem;
  border: 1.5px solid #e2e8f0;
  background: white;
  font-size: 0.8125rem;
  color: #64748b;
  cursor: pointer;
  transition: all 0.2s ease;
}

.type-select-btn:hover {
  border-color: #14b8a6;
  color: #0d9488;
}

.type-select-btn.active {
  background: linear-gradient(135deg, #14b8a6, #0d9488);
  color: white;
  border-color: #14b8a6;
}

/* Toggle Switch */
.toggle-row {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 1rem;
}

.toggle-switch {
  width: 48px;
  height: 26px;
  border-radius: 13px;
  background: #e2e8f0;
  border: none;
  cursor: pointer;
  position: relative;
  transition: all 0.3s ease;
  flex-shrink: 0;
}

.toggle-switch.active {
  background: linear-gradient(135deg, #14b8a6, #0d9488);
}

.toggle-knob {
  position: absolute;
  top: 3px;
  left: 3px;
  width: 20px;
  height: 20px;
  border-radius: 50%;
  background: white;
  box-shadow: 0 1px 4px rgba(0, 0, 0, 0.15);
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}

.toggle-switch.active .toggle-knob {
  left: 25px;
}

[dir="rtl"] .toggle-knob {
  left: auto;
  right: 3px;
}

[dir="rtl"] .toggle-switch.active .toggle-knob {
  right: 25px;
}

/* Icon Picker */
.icon-picker {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
}

.icon-pick-btn {
  width: 40px;
  height: 40px;
  border-radius: 0.625rem;
  border: 1.5px solid #e2e8f0;
  background: white;
  color: #64748b;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all 0.2s ease;
  font-size: 0.9375rem;
}

.icon-pick-btn:hover {
  border-color: #14b8a6;
  color: #0d9488;
  background: #f0fdfa;
}

.icon-pick-btn.active {
  background: linear-gradient(135deg, #14b8a6, #0d9488);
  color: white;
  border-color: #14b8a6;
  box-shadow: 0 2px 8px rgba(20, 184, 166, 0.3);
}

/* Color Picker */
.color-picker {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
}

.color-pick-btn {
  width: 36px;
  height: 36px;
  border-radius: 50%;
  border: 3px solid transparent;
  cursor: pointer;
  transition: all 0.2s ease;
  display: flex;
  align-items: center;
  justify-content: center;
}

.color-pick-btn:hover {
  transform: scale(1.15);
}

.color-pick-btn.active {
  border-color: #0f172a;
  transform: scale(1.15);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.2);
}

/* Space Preview */
.space-preview {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 1rem;
  border-radius: 0.75rem;
  border: 1.5px solid #e2e8f0;
  background: #fafafa;
}

.preview-icon {
  width: 44px;
  height: 44px;
  border-radius: 0.75rem;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

/* Modal Buttons */
.btn-cancel {
  padding: 0.625rem 1.25rem;
  border-radius: 0.75rem;
  border: 1.5px solid #e2e8f0;
  background: white;
  font-size: 0.875rem;
  font-weight: 600;
  color: #64748b;
  cursor: pointer;
  transition: all 0.2s ease;
}

.btn-cancel:hover {
  background: #f8fafc;
  border-color: #cbd5e1;
}

.btn-create {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.625rem 1.5rem;
  border-radius: 0.75rem;
  background: linear-gradient(135deg, #14b8a6, #0d9488);
  color: white;
  font-size: 0.875rem;
  font-weight: 600;
  border: none;
  cursor: pointer;
  transition: all 0.25s ease;
  box-shadow: 0 2px 8px rgba(20, 184, 166, 0.3);
}

.btn-create:hover:not(:disabled) {
  transform: translateY(-1px);
  box-shadow: 0 4px 16px rgba(20, 184, 166, 0.4);
}

.btn-create:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

/* ============================================
   ANIMATIONS
   ============================================ */
@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(16px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.fade-in-up {
  animation: fadeInUp 0.5s ease-out both;
}

/* Dropdown transition */
.dropdown-enter-active,
.dropdown-leave-active {
  transition: all 0.2s ease;
}

.dropdown-enter-from,
.dropdown-leave-to {
  opacity: 0;
  transform: translateY(-8px);
}

/* Modal transition */
.modal-enter-active,
.modal-leave-active {
  transition: all 0.3s ease;
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}

.modal-enter-from .modal-container,
.modal-leave-to .modal-container {
  transform: scale(0.95) translateY(20px);
}

.modal-enter-active .modal-container,
.modal-leave-active .modal-container {
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}

/* ============================================
   RESPONSIVE
   ============================================ */
@media (max-width: 1024px) {
  .type-filters {
    display: none;
  }
}

@media (max-width: 640px) {
  .toolbar {
    flex-direction: column;
    align-items: stretch;
  }

  .toolbar-left,
  .toolbar-right {
    width: 100%;
  }

  .search-box {
    width: 100%;
  }

  .toolbar-right {
    justify-content: flex-end;
  }
}
</style>
