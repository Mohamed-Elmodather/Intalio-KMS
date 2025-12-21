<script setup lang="ts">
import { computed } from 'vue'
import type { Participant } from '@/composables/collaboration/useCollaboration'
import Avatar from 'primevue/avatar'
import AvatarGroup from 'primevue/avatargroup'
import Tag from 'primevue/tag'

const props = defineProps<{
  participants: Participant[]
  isConnected: boolean
}>()

const activeParticipants = computed(() =>
  props.participants.filter(p => p.isActive)
)

const getInitials = (name: string): string => {
  return name
    .split(' ')
    .map(n => n[0])
    .join('')
    .toUpperCase()
    .substring(0, 2)
}
</script>

<template>
  <div class="collaboration-bar">
    <div class="connection-status">
      <Tag
        :value="isConnected ? 'Connected' : 'Disconnected'"
        :severity="isConnected ? 'success' : 'danger'"
        :icon="isConnected ? 'pi pi-wifi' : 'pi pi-wifi-off'"
      />
    </div>

    <div v-if="activeParticipants.length > 0" class="participants">
      <span class="participants-label">{{ activeParticipants.length }} editing</span>
      <AvatarGroup>
        <Avatar
          v-for="participant in activeParticipants.slice(0, 5)"
          :key="participant.userId"
          :image="participant.avatarUrl"
          :label="!participant.avatarUrl ? getInitials(participant.userName) : undefined"
          :style="{ backgroundColor: participant.color, color: 'white' }"
          shape="circle"
          v-tooltip.bottom="participant.userName"
        />
        <Avatar
          v-if="activeParticipants.length > 5"
          :label="`+${activeParticipants.length - 5}`"
          shape="circle"
          class="more-avatar"
        />
      </AvatarGroup>
    </div>
  </div>
</template>

<style scoped>
.collaboration-bar {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0.5rem 1rem;
  background: var(--surface-card);
  border-radius: var(--border-radius);
  margin-bottom: 1rem;
  box-shadow: var(--card-shadow);
}

.connection-status {
  display: flex;
  align-items: center;
}

.participants {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.participants-label {
  font-size: 0.875rem;
  color: var(--text-color-secondary);
}

.more-avatar {
  background: var(--surface-300);
  color: var(--text-color);
}

:deep(.p-avatar-group .p-avatar) {
  border: 2px solid var(--surface-card);
}
</style>
