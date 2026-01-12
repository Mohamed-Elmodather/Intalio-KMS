<script setup lang="ts">
import { ref, computed } from 'vue'

const props = withDefaults(defineProps<{
  modelValue?: number
  average?: number
  count?: number
  readonly?: boolean
  size?: 'sm' | 'md' | 'lg'
  showCount?: boolean
  showAverage?: boolean
}>(), {
  modelValue: 0,
  average: 0,
  count: 0,
  readonly: false,
  size: 'md',
  showCount: true,
  showAverage: true
})

const emit = defineEmits<{
  'update:modelValue': [value: number]
  'rate': [value: number]
}>()

const hoverRating = ref(0)

const displayRating = computed(() => {
  if (hoverRating.value > 0) return hoverRating.value
  if (props.modelValue > 0) return props.modelValue
  return props.average
})

const sizeClasses = computed(() => {
  switch (props.size) {
    case 'sm':
      return 'text-sm gap-0.5'
    case 'lg':
      return 'text-2xl gap-1'
    default:
      return 'text-lg gap-1'
  }
})

function handleClick(rating: number) {
  if (props.readonly) return
  emit('update:modelValue', rating)
  emit('rate', rating)
}

function handleMouseEnter(rating: number) {
  if (props.readonly) return
  hoverRating.value = rating
}

function handleMouseLeave() {
  hoverRating.value = 0
}

function getStarClass(index: number) {
  const rating = displayRating.value
  const isUserRating = props.modelValue > 0 || hoverRating.value > 0

  if (index <= rating) {
    return isUserRating ? 'text-yellow-400' : 'text-yellow-400'
  } else if (index - 0.5 <= rating) {
    return 'text-yellow-400 opacity-50'
  }
  return 'text-gray-300'
}
</script>

<template>
  <div class="rating-stars inline-flex items-center" :class="sizeClasses">
    <div class="flex items-center" :class="sizeClasses">
      <button
        v-for="i in 5"
        :key="i"
        type="button"
        @click="handleClick(i)"
        @mouseenter="handleMouseEnter(i)"
        @mouseleave="handleMouseLeave"
        :class="[
          'transition-all duration-150',
          readonly ? 'cursor-default' : 'cursor-pointer hover:scale-110'
        ]"
        :disabled="readonly"
      >
        <i
          class="fas fa-star transition-colors"
          :class="getStarClass(i)"
        ></i>
      </button>
    </div>

    <span v-if="showAverage && average > 0" class="ml-2 font-semibold text-gray-900">
      {{ average.toFixed(1) }}
    </span>

    <span v-if="showCount && count > 0" class="ml-1 text-gray-500 text-sm">
      ({{ count.toLocaleString() }} {{ count === 1 ? 'rating' : 'ratings' }})
    </span>
  </div>
</template>

<style scoped>
.rating-stars button:disabled {
  pointer-events: none;
}
</style>
