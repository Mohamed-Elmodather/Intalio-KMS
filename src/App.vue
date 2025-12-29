<script setup lang="ts">
import { computed } from 'vue'
import { useRoute } from 'vue-router'
import MainLayout from '@/components/layout/MainLayout.vue'

const route = useRoute()

const isBlankLayout = computed(() => route.meta.layout === 'blank')
</script>

<template>
  <div>
    <!-- Blank layout for login, etc -->
    <template v-if="isBlankLayout">
      <router-view />
    </template>

    <!-- Main layout with sidebar/header -->
    <template v-else>
      <MainLayout>
        <router-view />
      </MainLayout>
    </template>
  </div>
</template>
