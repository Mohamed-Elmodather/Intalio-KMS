<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import type { Author } from '@/types/detail-pages'

const props = withDefaults(defineProps<{
  author: Author
  variant?: 'compact' | 'full' | 'inline'
  showFollow?: boolean
  showStats?: boolean
  showBio?: boolean
}>(), {
  variant: 'full',
  showFollow: true,
  showStats: true,
  showBio: true
})

const router = useRouter()
const isFollowing = ref(props.author.isFollowing || false)
const isLoading = ref(false)

async function toggleFollow() {
  isLoading.value = true
  try {
    await new Promise(resolve => setTimeout(resolve, 500))
    isFollowing.value = !isFollowing.value
  } finally {
    isLoading.value = false
  }
}

function viewProfile() {
  router.push({ name: 'Profile', params: { id: props.author.id } })
}

function getInitialsColor(initials: string): string {
  const colors = [
    'from-teal-400 to-teal-600',
    'from-blue-400 to-blue-600',
    'from-purple-400 to-purple-600',
    'from-pink-400 to-pink-600',
    'from-orange-400 to-orange-600',
    'from-green-400 to-green-600'
  ]
  const index = initials.charCodeAt(0) % colors.length
  return colors[index]
}
</script>

<template>
  <!-- Inline Variant -->
  <div v-if="variant === 'inline'" class="flex items-center gap-3">
    <div
      @click="viewProfile"
      :class="[
        'w-10 h-10 rounded-full flex items-center justify-center text-white font-medium cursor-pointer hover:scale-105 transition-transform bg-gradient-to-br',
        getInitialsColor(author.initials)
      ]"
    >
      <img v-if="author.avatar" :src="author.avatar" :alt="author.name" class="w-full h-full rounded-full object-cover">
      <span v-else>{{ author.initials }}</span>
    </div>
    <div>
      <p @click="viewProfile" class="font-medium text-gray-900 hover:text-teal-600 cursor-pointer transition-colors">
        {{ author.name }}
      </p>
      <p class="text-sm text-gray-500">{{ author.role }}</p>
    </div>
  </div>

  <!-- Compact Variant -->
  <div v-else-if="variant === 'compact'" class="flex items-center gap-4 p-4 bg-gray-50 rounded-xl">
    <div
      @click="viewProfile"
      :class="[
        'w-12 h-12 rounded-full flex items-center justify-center text-white font-medium cursor-pointer hover:scale-105 transition-transform bg-gradient-to-br',
        getInitialsColor(author.initials)
      ]"
    >
      <img v-if="author.avatar" :src="author.avatar" :alt="author.name" class="w-full h-full rounded-full object-cover">
      <span v-else>{{ author.initials }}</span>
    </div>
    <div class="flex-1">
      <p @click="viewProfile" class="font-medium text-gray-900 hover:text-teal-600 cursor-pointer transition-colors">
        {{ author.name }}
      </p>
      <p class="text-sm text-gray-500">{{ author.role }}</p>
    </div>
    <button
      v-if="showFollow"
      @click="toggleFollow"
      :disabled="isLoading"
      :class="[
        'px-4 py-2 rounded-lg font-medium text-sm transition-all',
        isFollowing
          ? 'bg-gray-200 text-gray-700 hover:bg-gray-300'
          : 'bg-teal-500 text-white hover:bg-teal-600'
      ]"
    >
      <i v-if="isLoading" class="fas fa-spinner fa-spin mr-1"></i>
      <template v-else>
        <i :class="isFollowing ? 'fas fa-check' : 'fas fa-plus'" class="mr-1"></i>
        {{ isFollowing ? 'Following' : 'Follow' }}
      </template>
    </button>
  </div>

  <!-- Full Variant -->
  <div v-else class="author-card bg-gray-50 rounded-2xl p-6">
    <div class="flex items-start gap-4">
      <!-- Avatar -->
      <div
        @click="viewProfile"
        :class="[
          'w-16 h-16 rounded-2xl flex items-center justify-center text-white text-xl font-bold cursor-pointer hover:scale-105 transition-transform bg-gradient-to-br',
          getInitialsColor(author.initials)
        ]"
      >
        <img v-if="author.avatar" :src="author.avatar" :alt="author.name" class="w-full h-full rounded-2xl object-cover">
        <span v-else>{{ author.initials }}</span>
      </div>

      <!-- Info -->
      <div class="flex-1">
        <div class="flex items-start justify-between">
          <div>
            <h4 @click="viewProfile" class="text-lg font-semibold text-gray-900 hover:text-teal-600 cursor-pointer transition-colors">
              {{ author.name }}
            </h4>
            <p class="text-gray-600">{{ author.role }}</p>
            <p v-if="author.department" class="text-sm text-gray-500">{{ author.department }}</p>
          </div>

          <button
            v-if="showFollow"
            @click="toggleFollow"
            :disabled="isLoading"
            :class="[
              'px-4 py-2 rounded-lg font-medium text-sm transition-all',
              isFollowing
                ? 'bg-gray-200 text-gray-700 hover:bg-gray-300'
                : 'bg-teal-500 text-white hover:bg-teal-600'
            ]"
          >
            <i v-if="isLoading" class="fas fa-spinner fa-spin mr-1"></i>
            <template v-else>
              <i :class="isFollowing ? 'fas fa-check' : 'fas fa-plus'" class="mr-1"></i>
              {{ isFollowing ? 'Following' : 'Follow' }}
            </template>
          </button>
        </div>

        <!-- Bio -->
        <p v-if="showBio && author.bio" class="text-gray-600 mt-3 text-sm leading-relaxed">
          {{ author.bio }}
        </p>

        <!-- Stats -->
        <div v-if="showStats" class="flex items-center gap-6 mt-4">
          <div v-if="author.articlesCount" class="text-center">
            <p class="text-lg font-semibold text-gray-900">{{ author.articlesCount }}</p>
            <p class="text-xs text-gray-500">Articles</p>
          </div>
          <div v-if="author.followersCount" class="text-center">
            <p class="text-lg font-semibold text-gray-900">{{ author.followersCount }}</p>
            <p class="text-xs text-gray-500">Followers</p>
          </div>
          <button
            @click="viewProfile"
            class="text-teal-600 hover:text-teal-700 text-sm font-medium ml-auto"
          >
            View Profile <i class="fas fa-arrow-right ml-1"></i>
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.author-card {
  transition: all 0.3s ease;
}

.author-card:hover {
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08);
}
</style>
