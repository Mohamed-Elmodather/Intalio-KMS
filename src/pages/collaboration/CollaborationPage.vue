<script setup lang="ts">
import { ref } from 'vue'

const activeTab = ref('teams')
const teams = ref([
  { id: '1', name: 'Product Team', members: 12, avatar: 'PT', color: 'bg-blue-500' },
  { id: '2', name: 'Engineering', members: 24, avatar: 'EN', color: 'bg-purple-500' },
  { id: '3', name: 'Marketing', members: 8, avatar: 'MK', color: 'bg-green-500' },
])

const communities = ref([
  { id: '1', name: 'AI Enthusiasts', members: 156, posts: 234, category: 'Technology' },
  { id: '2', name: 'Book Club', members: 45, posts: 89, category: 'Social' },
  { id: '3', name: 'Wellness Warriors', members: 78, posts: 167, category: 'Health' },
])
</script>

<template>
  <div class="space-y-6 fade-in">
    <!-- Header -->
    <div class="flex items-center justify-between">
      <div>
        <h1 class="text-2xl font-bold text-gray-900">Collaboration Hub</h1>
        <p class="text-gray-500 mt-1">Connect with teams and communities</p>
      </div>
      <button class="btn btn-primary">
        <i class="fas fa-plus"></i>
        <span>Create Team</span>
      </button>
    </div>

    <!-- Tabs -->
    <div class="card p-1 inline-flex bg-gray-100 rounded-xl">
      <button
        @click="activeTab = 'teams'"
        :class="['btn btn-sm', activeTab === 'teams' ? 'bg-white shadow-sm' : 'btn-ghost']"
      >
        <i class="fas fa-users mr-2"></i> Teams
      </button>
      <button
        @click="activeTab = 'communities'"
        :class="['btn btn-sm', activeTab === 'communities' ? 'bg-white shadow-sm' : 'btn-ghost']"
      >
        <i class="fas fa-globe mr-2"></i> Communities
      </button>
    </div>

    <!-- Teams -->
    <div v-if="activeTab === 'teams'" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      <div v-for="team in teams" :key="team.id" class="card-interactive p-6">
        <div class="flex items-center gap-4">
          <div :class="['w-14 h-14 rounded-xl flex items-center justify-center text-white font-bold text-lg', team.color]">
            {{ team.avatar }}
          </div>
          <div class="flex-1">
            <h3 class="font-semibold text-gray-900">{{ team.name }}</h3>
            <p class="text-sm text-gray-500">{{ team.members }} members</p>
          </div>
          <button class="btn btn-secondary btn-sm">Join</button>
        </div>
      </div>
    </div>

    <!-- Communities -->
    <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      <div v-for="community in communities" :key="community.id" class="card-interactive p-6">
        <span class="badge badge-secondary mb-3">{{ community.category }}</span>
        <h3 class="font-semibold text-gray-900 text-lg">{{ community.name }}</h3>
        <div class="flex items-center gap-4 mt-3 text-sm text-gray-500">
          <span><i class="fas fa-users mr-1"></i> {{ community.members }} members</span>
          <span><i class="fas fa-comments mr-1"></i> {{ community.posts }} posts</span>
        </div>
        <button class="btn btn-primary w-full mt-4">Join Community</button>
      </div>
    </div>
  </div>
</template>
