<script setup lang="ts">
import { computed } from 'vue'

// Types
export interface PageStat {
  id: string
  label: string
  value: string | number
  icon: string
  color?: 'teal' | 'blue' | 'orange' | 'purple' | 'green' | 'red'
}

export interface PageAction {
  id: string
  label: string
  icon?: string
  variant?: 'primary' | 'secondary' | 'outline'
  onClick?: () => void
}

// Props
interface Props {
  title?: string
  subtitle?: string
  icon?: string
  iconBgClass?: string
  stats?: PageStat[]
  actions?: PageAction[]
  showHero?: boolean
  heroVariant?: 'default' | 'gradient' | 'minimal'
  contentPadding?: boolean
  maxWidth?: 'full' | '7xl' | '6xl' | '5xl' | '4xl'
}

const props = withDefaults(defineProps<Props>(), {
  title: '',
  subtitle: '',
  icon: '',
  iconBgClass: 'bg-gradient-to-br from-teal-500 to-teal-600',
  stats: () => [],
  actions: () => [],
  showHero: true,
  heroVariant: 'default',
  contentPadding: true,
  maxWidth: 'full'
})

// Emits
const emit = defineEmits<{
  'action-click': [actionId: string]
}>()

// Computed
const maxWidthClass = computed(() => {
  const widths: Record<string, string> = {
    'full': 'max-w-full',
    '7xl': 'max-w-7xl',
    '6xl': 'max-w-6xl',
    '5xl': 'max-w-5xl',
    '4xl': 'max-w-4xl'
  }
  return widths[props.maxWidth] || 'max-w-full'
})

// Methods
function handleActionClick(actionId: string) {
  emit('action-click', actionId)
}

function getStatColorClasses(color?: string) {
  const colors: Record<string, { bg: string; icon: string; text: string }> = {
    teal: { bg: 'bg-teal-50', icon: 'bg-teal-100 text-teal-600', text: 'text-teal-600' },
    blue: { bg: 'bg-blue-50', icon: 'bg-blue-100 text-blue-600', text: 'text-blue-600' },
    orange: { bg: 'bg-orange-50', icon: 'bg-orange-100 text-orange-600', text: 'text-orange-600' },
    purple: { bg: 'bg-purple-50', icon: 'bg-purple-100 text-purple-600', text: 'text-purple-600' },
    green: { bg: 'bg-green-50', icon: 'bg-green-100 text-green-600', text: 'text-green-600' },
    red: { bg: 'bg-red-50', icon: 'bg-red-100 text-red-600', text: 'text-red-600' }
  }
  return colors[color || 'teal'] || colors.teal
}

function getActionClasses(variant?: string) {
  switch (variant) {
    case 'primary':
      return 'unified-page__action--primary'
    case 'outline':
      return 'unified-page__action--outline'
    default:
      return 'unified-page__action--secondary'
  }
}
</script>

<template>
  <div class="unified-page" :class="maxWidthClass">
    <!-- Hero Section -->
    <header
      v-if="showHero && (title || $slots.hero)"
      class="unified-page__hero"
      :class="`unified-page__hero--${heroVariant}`"
    >
      <slot name="hero">
        <div class="unified-page__hero-content">
          <div class="unified-page__hero-main">
            <!-- Icon & Title -->
            <div class="unified-page__hero-title-group">
              <div v-if="icon" class="unified-page__hero-icon" :class="iconBgClass">
                <i :class="icon"></i>
              </div>
              <div>
                <h1 class="unified-page__title">{{ title }}</h1>
                <p v-if="subtitle" class="unified-page__subtitle">{{ subtitle }}</p>
              </div>
            </div>

            <!-- Actions -->
            <div v-if="actions.length > 0 || $slots.actions" class="unified-page__actions">
              <slot name="actions">
                <button
                  v-for="action in actions"
                  :key="action.id"
                  class="unified-page__action"
                  :class="getActionClasses(action.variant)"
                  @click="handleActionClick(action.id)"
                >
                  <i v-if="action.icon" :class="action.icon"></i>
                  <span>{{ action.label }}</span>
                </button>
              </slot>
            </div>
          </div>

          <!-- Stats -->
          <div v-if="stats.length > 0 || $slots.stats" class="unified-page__stats">
            <slot name="stats">
              <div
                v-for="stat in stats"
                :key="stat.id"
                class="unified-page__stat"
                :class="getStatColorClasses(stat.color).bg"
              >
                <div class="unified-page__stat-icon" :class="getStatColorClasses(stat.color).icon">
                  <i :class="stat.icon"></i>
                </div>
                <div class="unified-page__stat-content">
                  <span class="unified-page__stat-value" :class="getStatColorClasses(stat.color).text">
                    {{ stat.value }}
                  </span>
                  <span class="unified-page__stat-label">{{ stat.label }}</span>
                </div>
              </div>
            </slot>
          </div>
        </div>
      </slot>
    </header>

    <!-- Toolbar Slot (filters, search, view toggles) -->
    <div v-if="$slots.toolbar" class="unified-page__toolbar">
      <slot name="toolbar"></slot>
    </div>

    <!-- Main Content -->
    <main class="unified-page__content" :class="{ 'unified-page__content--padded': contentPadding }">
      <slot></slot>
    </main>

    <!-- Footer Slot -->
    <footer v-if="$slots.footer" class="unified-page__footer">
      <slot name="footer"></slot>
    </footer>
  </div>
</template>

<style scoped>
.unified-page {
  width: 100%;
  margin: 0 auto;
}

/* Hero Section */
.unified-page__hero {
  margin-bottom: 24px;
}

.unified-page__hero--default {
  background: white;
  border-radius: 16px;
  padding: 24px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
}

.unified-page__hero--gradient {
  background: linear-gradient(135deg, #f0fdfa 0%, #ccfbf1 50%, #f0fdfa 100%);
  border-radius: 16px;
  padding: 32px;
  position: relative;
  overflow: hidden;
}

.unified-page__hero--gradient::before {
  content: '';
  position: absolute;
  top: -50%;
  right: -20%;
  width: 300px;
  height: 300px;
  background: radial-gradient(circle, rgba(20, 184, 166, 0.15) 0%, transparent 70%);
  border-radius: 50%;
  pointer-events: none;
}

.unified-page__hero--minimal {
  padding: 0 0 16px 0;
  border-bottom: 1px solid #f3f4f6;
  margin-bottom: 24px;
}

.unified-page__hero-content {
  position: relative;
  z-index: 1;
}

.unified-page__hero-main {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: 24px;
  flex-wrap: wrap;
}

.unified-page__hero-title-group {
  display: flex;
  align-items: center;
  gap: 16px;
}

.unified-page__hero-icon {
  width: 56px;
  height: 56px;
  border-radius: 16px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 24px;
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.3);
  flex-shrink: 0;
}

.unified-page__title {
  font-size: 24px;
  font-weight: 700;
  color: #1f2937;
  margin: 0;
  line-height: 1.2;
}

.unified-page__subtitle {
  font-size: 14px;
  color: #6b7280;
  margin: 4px 0 0 0;
}

/* Actions */
.unified-page__actions {
  display: flex;
  align-items: center;
  gap: 12px;
  flex-wrap: wrap;
}

.unified-page__action {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 10px 20px;
  border-radius: 12px;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s ease;
  border: none;
  white-space: nowrap;
}

.unified-page__action i {
  font-size: 12px;
}

.unified-page__action--primary {
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.3);
}

.unified-page__action--primary:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 16px rgba(20, 184, 166, 0.4);
}

.unified-page__action--secondary {
  background: white;
  color: #374151;
  border: 1px solid #e5e7eb;
}

.unified-page__action--secondary:hover {
  background: #f0fdfa;
  border-color: #14b8a6;
  color: #0f766e;
}

.unified-page__action--outline {
  background: transparent;
  color: #14b8a6;
  border: 1px solid #14b8a6;
}

.unified-page__action--outline:hover {
  background: #f0fdfa;
}

/* Stats */
.unified-page__stats {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
  gap: 16px;
  margin-top: 24px;
}

.unified-page__stat {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 16px;
  border-radius: 12px;
  transition: transform 0.2s ease, box-shadow 0.2s ease;
}

.unified-page__stat:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.08);
}

.unified-page__stat-icon {
  width: 44px;
  height: 44px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 18px;
  flex-shrink: 0;
}

.unified-page__stat-content {
  display: flex;
  flex-direction: column;
}

.unified-page__stat-value {
  font-size: 20px;
  font-weight: 700;
  line-height: 1.2;
}

.unified-page__stat-label {
  font-size: 12px;
  color: #6b7280;
  margin-top: 2px;
}

/* Toolbar */
.unified-page__toolbar {
  margin-bottom: 24px;
}

/* Content */
.unified-page__content {
  width: 100%;
}

.unified-page__content--padded {
  padding: 0;
}

/* Footer */
.unified-page__footer {
  margin-top: 32px;
  padding-top: 24px;
  border-top: 1px solid #f3f4f6;
}

/* Responsive */
@media (max-width: 768px) {
  .unified-page__hero--default,
  .unified-page__hero--gradient {
    padding: 20px;
  }

  .unified-page__hero-main {
    flex-direction: column;
    align-items: stretch;
  }

  .unified-page__actions {
    justify-content: flex-start;
  }

  .unified-page__stats {
    grid-template-columns: repeat(2, 1fr);
  }

  .unified-page__title {
    font-size: 20px;
  }

  .unified-page__hero-icon {
    width: 48px;
    height: 48px;
    font-size: 20px;
  }
}

@media (max-width: 480px) {
  .unified-page__stats {
    grid-template-columns: 1fr;
  }
}
</style>
