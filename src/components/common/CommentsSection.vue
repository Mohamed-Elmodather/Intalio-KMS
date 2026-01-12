<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useComments } from '@/composables/useComments'
import type { Comment } from '@/types/detail-pages'

const props = defineProps<{
  contentType: string
  contentId: string
}>()

const {
  sortedComments,
  totalComments,
  isLoading,
  isSubmitting,
  sortBy,
  currentUser,
  loadComments,
  addComment,
  deleteComment,
  toggleReaction,
  formatTimeAgo
} = useComments(props.contentType, props.contentId)

const newComment = ref('')
const replyingTo = ref<string | null>(null)
const replyContent = ref('')
const expandedReplies = ref<Set<string>>(new Set())

onMounted(() => {
  loadComments()
})

async function handleSubmitComment() {
  if (!newComment.value.trim()) return

  await addComment(newComment.value)
  newComment.value = ''
}

async function handleSubmitReply(parentId: string) {
  if (!replyContent.value.trim()) return

  await addComment(replyContent.value, parentId)
  replyContent.value = ''
  replyingTo.value = null
  expandedReplies.value.add(parentId)
}

function toggleReplies(commentId: string) {
  if (expandedReplies.value.has(commentId)) {
    expandedReplies.value.delete(commentId)
  } else {
    expandedReplies.value.add(commentId)
  }
}

function startReply(commentId: string) {
  replyingTo.value = commentId
  replyContent.value = ''
}

function cancelReply() {
  replyingTo.value = null
  replyContent.value = ''
}

function getReactionIcon(type: string): string {
  const icons: Record<string, string> = {
    like: 'fas fa-thumbs-up',
    love: 'fas fa-heart',
    helpful: 'fas fa-hands-helping',
    insightful: 'fas fa-lightbulb',
    celebrate: 'fas fa-party-horn'
  }
  return icons[type] || 'fas fa-thumbs-up'
}
</script>

<template>
  <div class="comments-section">
    <!-- Header -->
    <div class="flex items-center justify-between mb-6">
      <h3 class="text-lg font-semibold text-gray-900">
        Comments
        <span class="text-gray-500 font-normal">({{ totalComments }})</span>
      </h3>
      <select v-model="sortBy" class="text-sm border border-gray-200 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-teal-500">
        <option value="newest">Newest First</option>
        <option value="oldest">Oldest First</option>
        <option value="popular">Most Popular</option>
      </select>
    </div>

    <!-- Comment Form -->
    <div class="mb-6 p-4 bg-gray-50 rounded-xl">
      <div class="flex gap-3">
        <div class="w-10 h-10 rounded-full bg-gradient-to-br from-teal-400 to-teal-600 flex items-center justify-center text-white font-medium flex-shrink-0">
          {{ currentUser.initials }}
        </div>
        <div class="flex-1">
          <textarea
            v-model="newComment"
            placeholder="Write a comment..."
            rows="3"
            class="w-full px-4 py-3 border border-gray-200 rounded-xl focus:outline-none focus:ring-2 focus:ring-teal-500 focus:border-transparent resize-none"
          ></textarea>
          <div class="flex justify-end mt-2">
            <button
              @click="handleSubmitComment"
              :disabled="!newComment.trim() || isSubmitting"
              class="px-4 py-2 bg-teal-500 text-white rounded-lg font-medium hover:bg-teal-600 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
            >
              <i class="fas fa-paper-plane mr-2"></i>
              Post Comment
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="isLoading" class="text-center py-8">
      <i class="fas fa-spinner fa-spin text-2xl text-teal-500"></i>
      <p class="text-gray-500 mt-2">Loading comments...</p>
    </div>

    <!-- Comments List -->
    <div v-else-if="sortedComments.length > 0" class="space-y-6">
      <div
        v-for="comment in sortedComments"
        :key="comment.id"
        class="comment-item"
      >
        <!-- Pinned Badge -->
        <div v-if="comment.isPinned" class="flex items-center gap-2 text-xs text-orange-600 mb-2">
          <i class="fas fa-thumbtack"></i>
          <span>Pinned comment</span>
        </div>

        <!-- Comment Content -->
        <div class="flex gap-3">
          <div class="w-10 h-10 rounded-full bg-gradient-to-br from-gray-300 to-gray-400 flex items-center justify-center text-white font-medium flex-shrink-0">
            {{ comment.author.initials }}
          </div>
          <div class="flex-1">
            <div class="flex items-center gap-2 mb-1">
              <span class="font-medium text-gray-900">{{ comment.author.name }}</span>
              <span class="text-xs text-gray-500">{{ comment.author.role }}</span>
              <span class="text-xs text-gray-400">•</span>
              <span class="text-xs text-gray-500">{{ formatTimeAgo(comment.createdAt) }}</span>
              <span v-if="comment.isEdited" class="text-xs text-gray-400">(edited)</span>
            </div>
            <p class="text-gray-700 mb-3">{{ comment.content }}</p>

            <!-- Reactions and Actions -->
            <div class="flex items-center gap-4 text-sm">
              <!-- Reactions -->
              <div class="flex items-center gap-2">
                <button
                  v-for="reaction in comment.reactions"
                  :key="reaction.type"
                  @click="toggleReaction(comment.id, reaction.type)"
                  :class="[
                    'flex items-center gap-1 px-2 py-1 rounded-full transition-colors',
                    reaction.hasReacted ? 'bg-teal-100 text-teal-700' : 'bg-gray-100 text-gray-600 hover:bg-gray-200'
                  ]"
                >
                  <i :class="getReactionIcon(reaction.type)" class="text-xs"></i>
                  <span>{{ reaction.count }}</span>
                </button>
                <button
                  @click="toggleReaction(comment.id, 'like')"
                  class="text-gray-500 hover:text-teal-600 transition-colors"
                >
                  <i class="far fa-thumbs-up"></i>
                </button>
              </div>

              <!-- Reply Button -->
              <button
                @click="startReply(comment.id)"
                class="text-gray-500 hover:text-teal-600 transition-colors"
              >
                <i class="fas fa-reply mr-1"></i>
                Reply
              </button>

              <!-- Show Replies -->
              <button
                v-if="comment.replies.length > 0"
                @click="toggleReplies(comment.id)"
                class="text-teal-600 hover:text-teal-700 transition-colors"
              >
                <i :class="expandedReplies.has(comment.id) ? 'fas fa-chevron-up' : 'fas fa-chevron-down'" class="mr-1"></i>
                {{ expandedReplies.has(comment.id) ? 'Hide' : 'Show' }} {{ comment.replies.length }} {{ comment.replies.length === 1 ? 'reply' : 'replies' }}
              </button>
            </div>

            <!-- Reply Form -->
            <div v-if="replyingTo === comment.id" class="mt-4 pl-4 border-l-2 border-teal-200">
              <textarea
                v-model="replyContent"
                placeholder="Write a reply..."
                rows="2"
                class="w-full px-3 py-2 border border-gray-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-teal-500 resize-none text-sm"
              ></textarea>
              <div class="flex justify-end gap-2 mt-2">
                <button
                  @click="cancelReply"
                  class="px-3 py-1.5 text-gray-600 hover:bg-gray-100 rounded-lg text-sm transition-colors"
                >
                  Cancel
                </button>
                <button
                  @click="handleSubmitReply(comment.id)"
                  :disabled="!replyContent.trim() || isSubmitting"
                  class="px-3 py-1.5 bg-teal-500 text-white rounded-lg text-sm hover:bg-teal-600 disabled:opacity-50 transition-colors"
                >
                  Reply
                </button>
              </div>
            </div>

            <!-- Replies -->
            <div v-if="expandedReplies.has(comment.id) && comment.replies.length > 0" class="mt-4 pl-4 border-l-2 border-gray-200 space-y-4">
              <div
                v-for="reply in comment.replies"
                :key="reply.id"
                class="flex gap-3"
              >
                <div class="w-8 h-8 rounded-full bg-gradient-to-br from-gray-300 to-gray-400 flex items-center justify-center text-white text-sm font-medium flex-shrink-0">
                  {{ reply.author.initials }}
                </div>
                <div class="flex-1">
                  <div class="flex items-center gap-2 mb-1">
                    <span class="font-medium text-gray-900 text-sm">{{ reply.author.name }}</span>
                    <span class="text-xs text-gray-500">{{ formatTimeAgo(reply.createdAt) }}</span>
                  </div>
                  <p class="text-gray-700 text-sm">{{ reply.content }}</p>
                  <div class="flex items-center gap-3 mt-2">
                    <button
                      v-for="reaction in reply.reactions"
                      :key="reaction.type"
                      @click="toggleReaction(reply.id, reaction.type)"
                      :class="[
                        'flex items-center gap-1 px-2 py-0.5 rounded-full text-xs transition-colors',
                        reaction.hasReacted ? 'bg-teal-100 text-teal-700' : 'bg-gray-100 text-gray-600 hover:bg-gray-200'
                      ]"
                    >
                      <i :class="getReactionIcon(reaction.type)" class="text-xs"></i>
                      <span>{{ reaction.count }}</span>
                    </button>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Empty State -->
    <div v-else class="text-center py-12 bg-gray-50 rounded-xl">
      <i class="fas fa-comments text-4xl text-gray-300 mb-3"></i>
      <p class="text-gray-500">No comments yet. Be the first to comment!</p>
    </div>
  </div>
</template>

<style scoped>
.comment-item {
  padding-bottom: 1.5rem;
  border-bottom: 1px solid #f3f4f6;
}

.comment-item:last-child {
  border-bottom: none;
  padding-bottom: 0;
}
</style>
