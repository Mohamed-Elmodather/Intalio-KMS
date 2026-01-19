<script setup lang="ts">
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

interface StatItem {
  icon: string
  value: string | number
  label: string
}

interface Props {
  stats: StatItem[]
  badgeIcon?: string
  badgeText?: string
  title: string
  subtitle: string
}

withDefaults(defineProps<Props>(), {
  badgeIcon: 'fas fa-trophy',
  badgeText: 'AFC Asian Cup 2027'
})
</script>

<template>
  <div class="hero-gradient relative overflow-hidden">
    <!-- Decorative elements with animations -->
    <div class="circle-drift-1 absolute top-0 end-0 w-96 h-96 bg-white/5 rounded-full"></div>
    <div class="circle-drift-2 absolute bottom-0 start-0 w-64 h-64 bg-white/5 rounded-full"></div>
    <div class="circle-drift-3 absolute top-1/2 end-1/4 w-32 h-32 bg-white/10 rounded-full"></div>

    <!-- Stats - Absolute Top End (Right in LTR, Left in RTL) -->
    <div class="stats-top-right">
      <div v-for="(stat, index) in stats" :key="index" class="stat-card-square">
        <div class="stat-icon-box">
          <i :class="stat.icon"></i>
        </div>
        <p class="stat-value-mini">{{ stat.value }}</p>
        <p class="stat-label-mini">{{ stat.label }}</p>
      </div>
    </div>

    <div class="relative px-8 py-8">
      <div class="px-3 py-1 bg-white/20 backdrop-blur-sm rounded-full text-white text-xs font-semibold inline-flex items-center gap-2 mb-4">
        <i :class="badgeIcon"></i>
        {{ badgeText }}
      </div>

      <h1 class="text-3xl font-bold text-white mb-2">{{ title }}</h1>
      <p class="text-teal-100 max-w-lg">{{ subtitle }}</p>

      <div class="flex flex-wrap gap-3 mt-6">
        <slot name="actions"></slot>
      </div>
    </div>
  </div>
</template>

<style scoped>
/* Hero Gradient */
.hero-gradient {
  background: linear-gradient(135deg, #0d9488 0%, #14b8a6 50%, #2dd4bf 100%);
  position: relative;
}

/* Stats Container */
.stats-top-right {
  position: absolute;
  top: 24px;
  inset-inline-end: 32px;
  display: flex;
  align-items: flex-start;
  gap: 12px;
  z-index: 10;
}

/* Stats Cards */
.stat-card-square {
  background: rgba(255, 255, 255, 0.15);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.2);
  border-radius: 16px;
  padding: 16px;
  min-width: 100px;
  text-align: center;
  transition: all 0.3s ease;
}

.stat-card-square:hover {
  background: rgba(255, 255, 255, 0.25);
  transform: translateY(-2px);
}

.stat-icon-box {
  width: 36px;
  height: 36px;
  border-radius: 10px;
  background: rgba(255, 255, 255, 0.2);
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 0 auto 0.5rem;
  color: white;
  font-size: 0.875rem;
}

.stat-value-mini {
  font-size: 1.25rem;
  font-weight: 700;
  color: white;
  line-height: 1;
}

.stat-label-mini {
  font-size: 0.65rem;
  color: rgba(255, 255, 255, 0.8);
  margin-top: 0.25rem;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

/* Circle Drift Animations */
.circle-drift-1 {
  animation: drift-1 20s ease-in-out infinite;
}

.circle-drift-2 {
  animation: drift-2 25s ease-in-out infinite;
}

.circle-drift-3 {
  animation: drift-3 18s ease-in-out infinite;
}

@keyframes drift-1 {
  0%, 100% { transform: translate(33%, -50%); }
  25% { transform: translate(28%, -45%); }
  50% { transform: translate(35%, -55%); }
  75% { transform: translate(30%, -48%); }
}

@keyframes drift-2 {
  0%, 100% { transform: translate(-33%, 50%); }
  33% { transform: translate(-28%, 45%); }
  66% { transform: translate(-38%, 55%); }
}

@keyframes drift-3 {
  0%, 100% { transform: translate(0, 0) scale(1); }
  50% { transform: translate(10px, -10px) scale(1.1); }
}

/* RTL Circle Drift Animations */
[dir="rtl"] .circle-drift-1 {
  animation: drift-1-rtl 20s ease-in-out infinite;
}

[dir="rtl"] .circle-drift-2 {
  animation: drift-2-rtl 25s ease-in-out infinite;
}

[dir="rtl"] .circle-drift-3 {
  animation: drift-3-rtl 18s ease-in-out infinite;
}

@keyframes drift-1-rtl {
  0%, 100% { transform: translate(-33%, -50%); }
  25% { transform: translate(-28%, -45%); }
  50% { transform: translate(-35%, -55%); }
  75% { transform: translate(-30%, -48%); }
}

@keyframes drift-2-rtl {
  0%, 100% { transform: translate(33%, 50%); }
  33% { transform: translate(28%, 45%); }
  66% { transform: translate(38%, 55%); }
}

@keyframes drift-3-rtl {
  0%, 100% { transform: translate(0, 0) scale(1); }
  50% { transform: translate(-10px, -10px) scale(1.1); }
}

/* Responsive */
@media (max-width: 1023px) {
  .stats-top-right {
    position: relative;
    top: auto;
    inset-inline-end: auto;
    margin: 24px 32px 0;
    display: grid;
    grid-template-columns: repeat(4, 1fr);
    gap: 8px;
  }

  .stat-card-square {
    width: 100%;
    height: 80px;
    padding: 12px 8px;
    min-width: unset;
  }

  .stat-icon-box {
    width: 28px;
    height: 28px;
    font-size: 0.75rem;
    margin-bottom: 0.25rem;
  }

  .stat-value-mini {
    font-size: 1rem;
  }

  .stat-label-mini {
    font-size: 0.55rem;
  }
}

@media (max-width: 640px) {
  .stats-top-right {
    grid-template-columns: repeat(2, 1fr);
    margin: 16px 16px 0;
  }

  .stat-card-square {
    height: auto;
    padding: 10px;
  }
}
</style>
