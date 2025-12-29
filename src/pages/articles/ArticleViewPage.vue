<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { articlesApi } from '@/api'
import type { Article } from '@/types'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'

const route = useRoute()
const router = useRouter()

const article = ref<Article | null>(null)
const isLoading = ref(true)
const error = ref<string | null>(null)

onMounted(async () => {
  const slug = route.params.slug as string
  try {
    article.value = await articlesApi.getArticle(slug)
  } catch {
    error.value = 'Article not found'
  } finally {
    isLoading.value = false
  }
})

function formatDate(dateString: string): string {
  return new Date(dateString).toLocaleDateString('en-US', {
    month: 'long',
    day: 'numeric',
    year: 'numeric',
  })
}

function goBack() {
  router.push({ name: 'Articles' })
}
</script>

<template>
  <div class="max-w-4xl mx-auto">
    <!-- Loading -->
    <div v-if="isLoading" class="card p-12">
      <LoadingSpinner size="lg" text="Loading article..." />
    </div>

    <!-- Error -->
    <div v-else-if="error" class="card p-12 text-center">
      <i class="fas fa-exclamation-triangle text-4xl text-red-300 mb-4"></i>
      <h3 class="text-lg font-semibold text-gray-700">{{ error }}</h3>
      <button @click="goBack" class="btn btn-secondary mt-4">
        <i class="fas fa-arrow-left"></i>
        <span>Back to Articles</span>
      </button>
    </div>

    <!-- Article -->
    <article v-else-if="article" class="fade-in">
      <!-- Back Button -->
      <button @click="goBack" class="btn btn-ghost mb-6">
        <i class="fas fa-arrow-left"></i>
        <span>Back to Articles</span>
      </button>

      <!-- Header -->
      <header class="mb-8">
        <div class="flex items-center gap-2 mb-4">
          <span v-if="article.category" class="badge badge-primary">{{ article.category.name }}</span>
          <span v-for="tag in article.tags" :key="tag.id" class="badge badge-secondary">
            {{ tag.name }}
          </span>
        </div>

        <h1 class="text-3xl md:text-4xl font-bold text-gray-900 leading-tight">
          {{ article.title }}
        </h1>

        <p class="text-xl text-gray-500 mt-4">{{ article.excerpt }}</p>

        <!-- Author & Meta -->
        <div class="flex items-center justify-between mt-6 pt-6 border-t border-gray-100">
          <div class="flex items-center gap-3">
            <div class="w-12 h-12 rounded-full bg-teal-100 flex items-center justify-center text-teal-600 font-bold">
              {{ article.author.displayName.split(' ').map((n: string) => n[0]).join('') }}
            </div>
            <div>
              <p class="font-semibold text-gray-900">{{ article.author.displayName }}</p>
              <p class="text-sm text-gray-500">
                {{ formatDate(article.publishedAt || article.createdAt) }}
                <span class="mx-2">•</span>
                {{ Math.ceil(article.content.length / 1000) }} min read
              </p>
            </div>
          </div>
          <div class="flex items-center gap-2">
            <button class="btn-icon btn-secondary">
              <i class="fas fa-heart" :class="article.isLiked ? 'text-red-500' : ''"></i>
            </button>
            <button class="btn-icon btn-secondary">
              <i class="fas fa-bookmark" :class="article.isBookmarked ? 'text-amber-500' : ''"></i>
            </button>
            <button class="btn-icon btn-secondary">
              <i class="fas fa-share-alt"></i>
            </button>
          </div>
        </div>
      </header>

      <!-- Cover Image -->
      <div v-if="article.coverImage" class="mb-8 rounded-2xl overflow-hidden">
        <img :src="article.coverImage" :alt="article.title" class="w-full">
      </div>

      <!-- Content -->
      <div class="card p-8 md:p-12">
        <div class="prose prose-lg max-w-none" v-html="article.content"></div>
      </div>

      <!-- Footer Stats -->
      <div class="flex items-center justify-between mt-6 p-4 bg-gray-50 rounded-xl">
        <div class="flex items-center gap-6 text-sm text-gray-500">
          <span><i class="fas fa-eye mr-2"></i> {{ article.viewCount }} views</span>
          <span><i class="fas fa-heart mr-2"></i> {{ article.likeCount }} likes</span>
          <span><i class="fas fa-comment mr-2"></i> {{ article.commentCount }} comments</span>
        </div>
      </div>
    </article>
  </div>
</template>
