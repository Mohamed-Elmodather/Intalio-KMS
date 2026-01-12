import { ref, computed } from 'vue'
import type { Comment, Author, Reaction } from '@/types/detail-pages'

export function useComments(contentType: string, contentId: string) {
  const comments = ref<Comment[]>([])
  const isLoading = ref(false)
  const isSubmitting = ref(false)
  const error = ref<string | null>(null)
  const sortBy = ref<'newest' | 'oldest' | 'popular'>('newest')

  // Mock current user
  const currentUser: Author = {
    id: 'current-user',
    name: 'Ahmed Imam',
    initials: 'AI',
    role: 'Product Director',
    department: 'Product Management'
  }

  // Mock comments data
  const mockComments: Comment[] = [
    {
      id: '1',
      author: {
        id: 'user-1',
        name: 'Sarah Chen',
        initials: 'SC',
        role: 'Engineering Lead'
      },
      content: 'This is really helpful! Thanks for sharing this detailed information.',
      createdAt: new Date(Date.now() - 2 * 60 * 60 * 1000), // 2 hours ago
      replies: [
        {
          id: '1-1',
          author: {
            id: 'user-2',
            name: 'Mike Johnson',
            initials: 'MJ',
            role: 'Senior Designer'
          },
          content: 'Agreed! The examples are particularly useful.',
          createdAt: new Date(Date.now() - 1 * 60 * 60 * 1000),
          replies: [],
          reactions: [{ type: 'like', count: 3, hasReacted: false }],
          mentions: []
        }
      ],
      reactions: [
        { type: 'like', count: 12, hasReacted: true },
        { type: 'helpful', count: 5, hasReacted: false }
      ],
      mentions: [],
      isPinned: true
    },
    {
      id: '2',
      author: {
        id: 'user-3',
        name: 'Emily Davis',
        initials: 'ED',
        role: 'Product Manager'
      },
      content: 'Great content! I have a question about the implementation details - can you elaborate on the third point?',
      createdAt: new Date(Date.now() - 5 * 60 * 60 * 1000),
      replies: [],
      reactions: [{ type: 'like', count: 4, hasReacted: false }],
      mentions: []
    },
    {
      id: '3',
      author: {
        id: 'user-4',
        name: 'Alex Thompson',
        initials: 'AT',
        role: 'Developer'
      },
      content: 'I\'ve been looking for something like this. Will definitely apply these concepts in my next project.',
      createdAt: new Date(Date.now() - 24 * 60 * 60 * 1000),
      replies: [],
      reactions: [
        { type: 'like', count: 8, hasReacted: false },
        { type: 'insightful', count: 2, hasReacted: false }
      ],
      mentions: []
    }
  ]

  const sortedComments = computed(() => {
    const sorted = [...comments.value]
    switch (sortBy.value) {
      case 'newest':
        return sorted.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
      case 'oldest':
        return sorted.sort((a, b) => new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime())
      case 'popular':
        return sorted.sort((a, b) => {
          const aReactions = a.reactions.reduce((sum, r) => sum + r.count, 0)
          const bReactions = b.reactions.reduce((sum, r) => sum + r.count, 0)
          return bReactions - aReactions
        })
      default:
        return sorted
    }
  })

  const totalComments = computed(() => {
    let count = comments.value.length
    comments.value.forEach(c => {
      count += c.replies.length
    })
    return count
  })

  async function loadComments() {
    isLoading.value = true
    error.value = null

    try {
      // Simulate API call
      await new Promise(resolve => setTimeout(resolve, 800))
      comments.value = mockComments
    } catch (e) {
      error.value = 'Failed to load comments'
      console.error(e)
    } finally {
      isLoading.value = false
    }
  }

  async function addComment(content: string, parentId?: string) {
    isSubmitting.value = true

    try {
      await new Promise(resolve => setTimeout(resolve, 500))

      const newComment: Comment = {
        id: `comment-${Date.now()}`,
        author: currentUser,
        content,
        createdAt: new Date(),
        replies: [],
        reactions: [],
        mentions: extractMentions(content)
      }

      if (parentId) {
        const parent = comments.value.find(c => c.id === parentId)
        if (parent) {
          parent.replies.push(newComment)
        }
      } else {
        comments.value.unshift(newComment)
      }

      return newComment
    } catch (e) {
      error.value = 'Failed to add comment'
      throw e
    } finally {
      isSubmitting.value = false
    }
  }

  async function editComment(commentId: string, newContent: string) {
    const comment = findComment(commentId)
    if (comment) {
      comment.content = newContent
      comment.isEdited = true
      comment.updatedAt = new Date()
    }
  }

  async function deleteComment(commentId: string) {
    // Find and remove comment
    const index = comments.value.findIndex(c => c.id === commentId)
    if (index !== -1) {
      comments.value.splice(index, 1)
      return
    }

    // Check in replies
    for (const comment of comments.value) {
      const replyIndex = comment.replies.findIndex(r => r.id === commentId)
      if (replyIndex !== -1) {
        comment.replies.splice(replyIndex, 1)
        return
      }
    }
  }

  async function toggleReaction(commentId: string, reactionType: Reaction['type']) {
    const comment = findComment(commentId)
    if (!comment) return

    const reaction = comment.reactions.find(r => r.type === reactionType)
    if (reaction) {
      if (reaction.hasReacted) {
        reaction.count--
        reaction.hasReacted = false
      } else {
        reaction.count++
        reaction.hasReacted = true
      }
    } else {
      comment.reactions.push({
        type: reactionType,
        count: 1,
        hasReacted: true
      })
    }
  }

  function findComment(commentId: string): Comment | undefined {
    const direct = comments.value.find(c => c.id === commentId)
    if (direct) return direct

    for (const comment of comments.value) {
      const reply = comment.replies.find(r => r.id === commentId)
      if (reply) return reply
    }

    return undefined
  }

  function extractMentions(content: string): string[] {
    const mentionRegex = /@(\w+)/g
    const matches = content.match(mentionRegex)
    return matches ? matches.map(m => m.substring(1)) : []
  }

  function formatTimeAgo(date: Date): string {
    const now = new Date()
    const diff = now.getTime() - date.getTime()
    const minutes = Math.floor(diff / 60000)
    const hours = Math.floor(diff / 3600000)
    const days = Math.floor(diff / 86400000)

    if (minutes < 1) return 'Just now'
    if (minutes < 60) return `${minutes}m ago`
    if (hours < 24) return `${hours}h ago`
    if (days < 7) return `${days}d ago`
    return date.toLocaleDateString()
  }

  return {
    comments,
    sortedComments,
    totalComments,
    isLoading,
    isSubmitting,
    error,
    sortBy,
    currentUser,
    loadComments,
    addComment,
    editComment,
    deleteComment,
    toggleReaction,
    formatTimeAgo
  }
}
