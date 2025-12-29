<script setup lang="ts">
import { useRouter } from 'vue-router'

const router = useRouter()

function goBack() {
  router.push({ name: 'Events' })
}
</script>

<template>
  <div class="max-w-4xl mx-auto space-y-6 fade-in">
    <button @click="goBack" class="btn btn-ghost">
      <i class="fas fa-arrow-left"></i>
      <span>Back to Events</span>
    </button>

    <div class="card p-8">
      <div class="text-center py-12">
        <i class="fas fa-calendar-alt text-4xl text-gray-300 mb-4"></i>
        <h2 class="text-xl font-semibold text-gray-700">Event Details</h2>
        <p class="text-gray-500 mt-2">Event details will be displayed here</p>
      </div>
    </div>
  </div>
</template>
