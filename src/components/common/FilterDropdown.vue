<script setup lang="ts">
import { ref, computed } from 'vue'

interface FilterOption {
  id: string
  label: string
  icon?: string
  color?: string
  count?: number
  bgColor?: string
}

interface Props {
  icon: string
  label: string
  selectedLabel: string
  headerLabel: string
  options: FilterOption[]
  modelValue: string[]
  dropdownWidth?: string
  clearAllLabel: string
  applyLabel: string
  multiSelect?: boolean
  showCount?: boolean
  showColorBadge?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  dropdownWidth: 'w-56',
  multiSelect: true,
  showCount: false,
  showColorBadge: false
})

const emit = defineEmits<{
  'update:modelValue': [values: string[]]
}>()

const isOpen = ref(false)

const hasSelection = computed(() => props.modelValue.length > 0)

const displayLabel = computed(() => {
  if (props.modelValue.length > 0) {
    return `${props.modelValue.length} ${props.selectedLabel}`
  }
  return props.label
})

function isSelected(id: string): boolean {
  return props.modelValue.includes(id)
}

function toggleOption(id: string) {
  if (props.multiSelect) {
    const newValues = isSelected(id)
      ? props.modelValue.filter(v => v !== id)
      : [...props.modelValue, id]
    emit('update:modelValue', newValues)
  } else {
    emit('update:modelValue', isSelected(id) ? [] : [id])
  }
}

function clearAll() {
  emit('update:modelValue', [])
  isOpen.value = false
}

function apply() {
  isOpen.value = false
}
</script>

<template>
  <div class="relative">
    <button
      @click="isOpen = !isOpen"
      :class="[
        'flex items-center gap-2 px-3 py-1.5 rounded-lg text-xs font-medium transition-all border',
        hasSelection ? 'bg-teal-50 border-teal-200 text-teal-700' : 'bg-white border-gray-200 text-gray-600 hover:bg-gray-50'
      ]"
    >
      <i :class="[icon, 'text-sm']"></i>
      <span>{{ displayLabel }}</span>
      <i :class="isOpen ? 'fas fa-chevron-up' : 'fas fa-chevron-down'" class="text-[10px] ms-1"></i>
    </button>

    <!-- Dropdown Menu -->
    <div
      v-if="isOpen"
      :class="['absolute start-0 top-full mt-2 bg-white rounded-xl shadow-lg border border-gray-100 py-2 z-50', dropdownWidth]"
    >
      <div class="px-3 py-1.5 text-xs font-semibold text-gray-400 uppercase tracking-wider">{{ headerLabel }}</div>
      <div class="max-h-48 overflow-y-auto">
        <button
          v-for="option in options"
          :key="option.id"
          @click="toggleOption(option.id)"
          :class="[
            'w-full px-3 py-2 text-left text-sm flex items-center gap-3 transition-colors',
            isSelected(option.id) ? 'bg-teal-50 text-teal-700' : 'text-gray-700 hover:bg-gray-50'
          ]"
        >
          <!-- Checkbox -->
          <div :class="[
            'w-4 h-4 rounded border-2 flex items-center justify-center transition-all',
            isSelected(option.id) ? 'bg-teal-500 border-teal-500' : 'border-gray-300'
          ]">
            <i v-if="isSelected(option.id)" class="fas fa-check text-white text-[8px]"></i>
          </div>

          <!-- Color Badge (for categories) -->
          <div
            v-if="showColorBadge && option.bgColor"
            class="w-6 h-6 rounded-lg flex items-center justify-center text-white text-xs"
            :style="{ backgroundColor: option.bgColor }"
          >
            <i v-if="option.icon" :class="option.icon"></i>
          </div>

          <!-- Icon -->
          <i v-else-if="option.icon" :class="[option.icon, option.color || 'text-gray-400', 'text-sm']"></i>

          <!-- Label -->
          <span class="flex-1">{{ option.label }}</span>

          <!-- Count -->
          <span v-if="showCount && option.count !== undefined" class="text-xs text-gray-400">{{ option.count }}</span>
        </button>
      </div>

      <div class="my-2 border-t border-gray-100"></div>

      <div class="px-3 flex gap-2">
        <button
          @click="clearAll"
          class="flex-1 px-3 py-1.5 text-xs font-medium text-gray-600 bg-gray-100 rounded-lg hover:bg-gray-200 transition-colors"
        >
          {{ clearAllLabel }}
        </button>
        <button
          @click="apply"
          class="flex-1 px-3 py-1.5 text-xs font-medium text-white bg-teal-500 rounded-lg hover:bg-teal-600 transition-colors"
        >
          {{ applyLabel }}
        </button>
      </div>
    </div>

    <!-- Click outside to close -->
    <div v-if="isOpen" @click="isOpen = false" class="fixed inset-0 z-40"></div>
  </div>
</template>
