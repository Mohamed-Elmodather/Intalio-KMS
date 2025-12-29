<script setup lang="ts">
import { useRouter } from 'vue-router'

const router = useRouter()

function goBack() {
  router.push({ name: 'MediaCenter' })
}
</script>

<template>
  <div class="max-w-6xl mx-auto space-y-6 fade-in">
    <button @click="goBack" class="btn btn-ghost">
      <i class="fas fa-arrow-left"></i>
      <span>Back to Media Center</span>
    </button>

    <div class="card overflow-hidden">
      <div class="aspect-video bg-black flex items-center justify-center">
        <div class="text-center text-white">
          <i class="fas fa-play-circle text-6xl opacity-50 mb-4"></i>
          <p class="text-lg">Media Player</p>
        </div>
      </div>
    </div>
  </div>
</template>
