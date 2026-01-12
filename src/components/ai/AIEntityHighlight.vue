<script setup lang="ts">
import { computed } from 'vue'
import type { Entity, EntityType } from '@/types/ai'

interface Props {
  text: string
  entities: Entity[]
  interactive?: boolean
  showTooltips?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  interactive: true,
  showTooltips: true,
})

const emit = defineEmits<{
  entityClick: [entity: Entity]
}>()

const entityColors: Record<EntityType, { bg: string; text: string; border: string }> = {
  person: { bg: 'bg-blue-100', text: 'text-blue-700', border: 'border-blue-300' },
  organization: { bg: 'bg-purple-100', text: 'text-purple-700', border: 'border-purple-300' },
  location: { bg: 'bg-green-100', text: 'text-green-700', border: 'border-green-300' },
  date: { bg: 'bg-amber-100', text: 'text-amber-700', border: 'border-amber-300' },
  event: { bg: 'bg-teal-100', text: 'text-teal-700', border: 'border-teal-300' },
  product: { bg: 'bg-pink-100', text: 'text-pink-700', border: 'border-pink-300' },
  amount: { bg: 'bg-indigo-100', text: 'text-indigo-700', border: 'border-indigo-300' },
}

const entityIcons: Record<EntityType, string> = {
  person: 'fas fa-user',
  organization: 'fas fa-building',
  location: 'fas fa-map-marker-alt',
  date: 'fas fa-calendar',
  event: 'fas fa-calendar-star',
  product: 'fas fa-box',
  amount: 'fas fa-dollar-sign',
}

interface TextSegment {
  text: string
  entity?: Entity
  isEntity: boolean
}

const segments = computed<TextSegment[]>(() => {
  if (!props.entities.length) {
    return [{ text: props.text, isEntity: false }]
  }

  // Sort entities by start index
  const sortedEntities = [...props.entities].sort((a, b) => a.startIndex - b.startIndex)

  const result: TextSegment[] = []
  let lastIndex = 0

  for (const entity of sortedEntities) {
    // Add text before this entity
    if (entity.startIndex > lastIndex) {
      result.push({
        text: props.text.slice(lastIndex, entity.startIndex),
        isEntity: false,
      })
    }

    // Add the entity
    result.push({
      text: entity.text,
      entity,
      isEntity: true,
    })

    lastIndex = entity.endIndex
  }

  // Add remaining text after the last entity
  if (lastIndex < props.text.length) {
    result.push({
      text: props.text.slice(lastIndex),
      isEntity: false,
    })
  }

  return result
})

function handleEntityClick(entity: Entity) {
  if (props.interactive) {
    emit('entityClick', entity)
  }
}
</script>

<template>
  <span class="ai-entity-highlight">
    <template v-for="(segment, index) in segments" :key="index">
      <!-- Regular text -->
      <span v-if="!segment.isEntity">{{ segment.text }}</span>

      <!-- Entity highlight -->
      <span
        v-else
        class="entity-tag inline-flex items-center gap-1 px-1.5 py-0.5 mx-0.5 rounded border text-sm font-medium transition-all"
        :class="[
          entityColors[segment.entity!.type].bg,
          entityColors[segment.entity!.type].text,
          entityColors[segment.entity!.type].border,
          interactive ? 'cursor-pointer hover:shadow-sm' : '',
        ]"
        :title="showTooltips ? `${segment.entity!.type}: ${segment.entity!.text} (${Math.round(segment.entity!.confidence * 100)}% confidence)` : undefined"
        @click="handleEntityClick(segment.entity!)"
      >
        <i :class="[entityIcons[segment.entity!.type], 'text-xs opacity-70']" />
        <span>{{ segment.text }}</span>
      </span>
    </template>
  </span>
</template>

<style scoped>
.entity-tag {
  vertical-align: baseline;
}

.entity-tag:hover {
  transform: translateY(-1px);
}
</style>
