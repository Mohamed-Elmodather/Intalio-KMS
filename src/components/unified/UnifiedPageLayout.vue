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
  const colors: Record<string, { bg: string; icon: string; text: string; border: string }> = {
    teal: { bg: 'bg-gradient-to-br from-teal-50 to-teal-100/50', icon: 'bg-teal-100 text-teal-600', text: 'text-teal-700', border: 'border-teal-200/50' },
    blue: { bg: 'bg-gradient-to-br from-blue-50 to-blue-100/50', icon: 'bg-blue-100 text-blue-600', text: 'text-blue-700', border: 'border-blue-200/50' },
    orange: { bg: 'bg-gradient-to-br from-orange-50 to-orange-100/50', icon: 'bg-orange-100 text-orange-600', text: 'text-orange-700', border: 'border-orange-200/50' },
    purple: { bg: 'bg-gradient-to-br from-purple-50 to-purple-100/50', icon: 'bg-purple-100 text-purple-600', text: 'text-purple-700', border: 'border-purple-200/50' },
    green: { bg: 'bg-gradient-to-br from-green-50 to-green-100/50', icon: 'bg-green-100 text-green-600', text: 'text-green-700', border: 'border-green-200/50' },
    red: { bg: 'bg-gradient-to-br from-red-50 to-red-100/50', icon: 'bg-red-100 text-red-600', text: 'text-red-700', border: 'border-red-200/50' }
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
            <!-- Left: Icon & Title -->
            <div class="unified-page__hero-left">
              <div class="unified-page__hero-header">
                <div v-if="icon" class="unified-page__hero-icon">
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

            <!-- Right: Stats -->
            <div v-if="stats.length > 0 || $slots.stats" class="unified-page__stats">
              <slot name="stats">
                <div
                  v-for="(stat, index) in stats"
                  :key="stat.id"
                  class="unified-page__stat"
                  :class="[getStatColorClasses(stat.color).bg, getStatColorClasses(stat.color).border]"
                  :style="{ animationDelay: `${index * 0.1}s` }"
                >
                  <div class="unified-page__stat-icon" :class="getStatColorClasses(stat.color).icon">
                    <i :class="stat.icon"></i>
                  </div>
                  <div class="unified-page__stat-content">
                    <span class="unified-page__stat-value">
                      {{ stat.value }}
                    </span>
                    <span class="unified-page__stat-label">{{ stat.label }}</span>
                  </div>
                </div>
              </slot>
            </div>
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

/* ============================================
   HERO SECTION
   ============================================ */
.unified-page__hero {
  margin-bottom: 1.5rem;
}

.unified-page__hero--default {
  position: relative;
  padding: 2rem;
  background: linear-gradient(135deg, #f0fdfa 0%, #ccfbf1 50%, #99f6e4 100%);
  border-radius: 1rem;
  overflow: hidden;
}

.unified-page__hero--default::before {
  content: '';
  position: absolute;
  top: -50%;
  right: -20%;
  width: 400px;
  height: 400px;
  background: radial-gradient(circle, rgba(20, 184, 166, 0.15) 0%, transparent 70%);
  animation: heroFloat 8s ease-in-out infinite;
  pointer-events: none;
}

.unified-page__hero--default::after {
  content: '';
  position: absolute;
  bottom: -30%;
  left: 10%;
  width: 300px;
  height: 300px;
  background: radial-gradient(circle, rgba(13, 148, 136, 0.1) 0%, transparent 70%);
  animation: heroFloat 8s ease-in-out infinite reverse;
  pointer-events: none;
}

@keyframes heroFloat {
  0%, 100% { transform: translate(0, 0) scale(1); }
  50% { transform: translate(20px, -20px) scale(1.05); }
}

.unified-page__hero--gradient {
  position: relative;
  padding: 2rem;
  background: linear-gradient(135deg, #f0fdfa 0%, #ccfbf1 50%, #99f6e4 100%);
  border-radius: 1rem;
  overflow: hidden;
}

.unified-page__hero--gradient::before {
  content: '';
  position: absolute;
  top: -50%;
  right: -20%;
  width: 400px;
  height: 400px;
  background: radial-gradient(circle, rgba(20, 184, 166, 0.15) 0%, transparent 70%);
  animation: heroFloat 8s ease-in-out infinite;
  pointer-events: none;
}

.unified-page__hero--gradient::after {
  content: '';
  position: absolute;
  bottom: -30%;
  left: 10%;
  width: 300px;
  height: 300px;
  background: radial-gradient(circle, rgba(13, 148, 136, 0.1) 0%, transparent 70%);
  animation: heroFloat 8s ease-in-out infinite reverse;
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
  flex-direction: column;
  gap: 1.5rem;
}

@media (min-width: 1024px) {
  .unified-page__hero-main {
    flex-direction: row;
    align-items: center;
    justify-content: space-between;
  }
}

.unified-page__hero-left {
  flex: 0 1 auto;
}

.unified-page__hero-header {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  margin-bottom: 0.75rem;
}

.unified-page__hero-icon {
  width: 48px;
  height: 48px;
  border-radius: 0.75rem;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 1.25rem;
  flex-shrink: 0;
  animation: heroIconPulse 2s ease-in-out infinite;
  box-shadow: 0 4px 15px rgba(20, 184, 166, 0.3);
}

@keyframes heroIconPulse {
  0%, 100% { transform: scale(1); box-shadow: 0 4px 15px rgba(20, 184, 166, 0.3); }
  50% { transform: scale(1.05); box-shadow: 0 6px 20px rgba(20, 184, 166, 0.5); }
}

.unified-page__title {
  font-size: 1.875rem;
  font-weight: 800;
  color: #0f172a;
  margin: 0 0 0.25rem;
  letter-spacing: -0.025em;
  line-height: 1.2;
}

.unified-page__subtitle {
  font-size: 0.875rem;
  color: #64748b;
  margin: 0;
}

/* ============================================
   ACTIONS
   ============================================ */
.unified-page__actions {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  flex-wrap: wrap;
  margin-top: 0.5rem;
}

.unified-page__action {
  display: inline-flex;
  align-items: center;
  gap: 0.375rem;
  padding: 0.5rem 1rem;
  font-size: 0.8125rem;
  font-weight: 600;
  border-radius: 0.625rem;
  cursor: pointer;
  transition: all 0.3s ease;
  border: none;
  white-space: nowrap;
}

.unified-page__action i {
  font-size: 0.75rem;
}

.unified-page__action--primary {
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
  box-shadow: 0 2px 8px rgba(20, 184, 166, 0.3);
}

.unified-page__action--primary:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 15px rgba(20, 184, 166, 0.4);
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

/* ============================================
   STAT CARDS
   ============================================ */
.unified-page__stats {
  display: flex;
  flex-wrap: wrap;
  gap: 0.75rem;
  margin-left: auto;
  justify-content: flex-end;
}

.unified-page__stat {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 1rem 1.25rem;
  min-width: 140px;
  border-radius: 1rem;
  border: 1px solid rgba(20, 184, 166, 0.2);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.04);
  transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
  position: relative;
  overflow: hidden;
  cursor: default;
  animation: statSlideIn 0.5s ease-out backwards;
}

.unified-page__stat::before {
  content: '';
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(90deg, transparent, rgba(20, 184, 166, 0.1), transparent);
  transition: left 0.5s ease;
}

.unified-page__stat::after {
  content: '';
  position: absolute;
  bottom: 0;
  left: 0;
  width: 0;
  height: 3px;
  background: linear-gradient(90deg, #14b8a6, #0d9488);
  transition: width 0.4s ease;
}

.unified-page__stat:hover::before {
  left: 100%;
}

.unified-page__stat:hover::after {
  width: 100%;
}

.unified-page__stat:hover {
  transform: translateY(-6px) scale(1.02);
  box-shadow: 0 15px 35px rgba(20, 184, 166, 0.2);
  border-color: #14b8a6;
}

.unified-page__stat:hover .unified-page__stat-icon {
  transform: scale(1.1) rotate(6deg);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.unified-page__stat:hover .unified-page__stat-value {
  transform: scale(1.05);
  color: #0d9488;
}

@keyframes statSlideIn {
  from {
    opacity: 0;
    transform: translateY(15px) scale(0.95);
  }
  to {
    opacity: 1;
    transform: translateY(0) scale(1);
  }
}

.unified-page__stat-icon {
  width: 48px;
  height: 48px;
  border-radius: 0.75rem;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.125rem;
  transition: all 0.3s ease;
  flex-shrink: 0;
}

.unified-page__stat-content {
  flex: 1;
  min-width: 0;
}

.unified-page__stat-value {
  font-size: 1.375rem;
  font-weight: 700;
  color: #0f172a;
  line-height: 1;
  margin: 0 0 0.125rem;
  display: block;
  transition: transform 0.3s ease, color 0.3s ease;
}

.unified-page__stat-label {
  font-size: 0.75rem;
  color: #64748b;
  font-weight: 500;
  margin: 0;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  display: block;
}

/* ============================================
   TOOLBAR & CONTENT
   ============================================ */
.unified-page__toolbar {
  margin-bottom: 24px;
}

.unified-page__content {
  width: 100%;
}

.unified-page__content--padded {
  padding: 0;
}

.unified-page__footer {
  margin-top: 32px;
  padding-top: 24px;
  border-top: 1px solid #f3f4f6;
}

/* ============================================
   RESPONSIVE
   ============================================ */
@media (max-width: 768px) {
  .unified-page__hero--default,
  .unified-page__hero--gradient {
    padding: 1.25rem;
  }

  .unified-page__hero-icon {
    width: 40px;
    height: 40px;
    font-size: 1rem;
  }

  .unified-page__title {
    font-size: 1.5rem;
  }

  .unified-page__stats {
    justify-content: flex-start;
    margin-left: 0;
  }

  .unified-page__stat {
    padding: 0.75rem 1rem;
    min-width: 0;
    flex: 1 1 calc(50% - 0.375rem);
  }

  .unified-page__stat-icon {
    width: 40px;
    height: 40px;
    font-size: 1rem;
  }

  .unified-page__stat-value {
    font-size: 1.125rem;
  }
}

@media (max-width: 480px) {
  .unified-page__stat {
    flex: 1 1 100%;
  }
}
</style>
