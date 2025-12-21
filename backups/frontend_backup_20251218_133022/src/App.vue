<script setup lang="ts">
import { computed, watchEffect } from 'vue'
import { useUIStore } from '@/stores/ui.store'
import Toast from 'primevue/toast'
import ConfirmDialog from 'primevue/confirmdialog'

const uiStore = useUIStore()

const direction = computed(() => uiStore.direction)
const locale = computed(() => uiStore.locale)

// Apply direction and language to document
watchEffect(() => {
  document.documentElement.setAttribute('dir', direction.value)
  document.documentElement.setAttribute('lang', locale.value)
  document.body.classList.toggle('rtl', direction.value === 'rtl')
})
</script>

<template>
  <Toast position="top-right" />
  <ConfirmDialog />
  <router-view />
</template>

<style lang="scss">
#app {
  min-height: 100vh;
}
</style>
