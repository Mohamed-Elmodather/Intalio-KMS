import { describe, it, expect, vi, beforeEach } from 'vitest'
import { useComments } from '@/composables/useComments'

describe('useComments', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('Initial State', () => {
    it('should have empty comments array', () => {
      const { comments } = useComments('article', 'test-id')
      expect(comments.value).toEqual([])
    })

    it('should not be loading initially', () => {
      const { isLoading } = useComments('article', 'test-id')
      expect(isLoading.value).toBe(false)
    })

    it('should not be submitting initially', () => {
      const { isSubmitting } = useComments('article', 'test-id')
      expect(isSubmitting.value).toBe(false)
    })

    it('should have null error initially', () => {
      const { error } = useComments('article', 'test-id')
      expect(error.value).toBeNull()
    })

    it('should default to newest sort', () => {
      const { sortBy } = useComments('article', 'test-id')
      expect(sortBy.value).toBe('newest')
    })

    it('should have a current user', () => {
      const { currentUser } = useComments('article', 'test-id')
      expect(currentUser).toBeDefined()
      expect(currentUser.id).toBe('current-user')
    })
  })

  describe('loadComments', () => {
    it('should set loading state while fetching', async () => {
      const { isLoading, loadComments } = useComments('article', 'test-id')

      const promise = loadComments()
      expect(isLoading.value).toBe(true)

      await promise
      expect(isLoading.value).toBe(false)
    })

    it('should load mock comments', async () => {
      const { comments, loadComments } = useComments('article', 'test-id')

      await loadComments()

      expect(comments.value.length).toBeGreaterThan(0)
    })

    it('should clear error before loading', async () => {
      const { error, loadComments } = useComments('article', 'test-id')
      error.value = 'Previous error'

      await loadComments()

      expect(error.value).toBeNull()
    })
  })

  describe('sortedComments', () => {
    it('should sort by newest first', async () => {
      const { loadComments, sortedComments, sortBy } = useComments(
        'article',
        'test-id'
      )
      await loadComments()

      sortBy.value = 'newest'

      // Verify sorted by date descending
      for (let i = 1; i < sortedComments.value.length; i++) {
        const current = new Date(sortedComments.value[i].createdAt).getTime()
        const previous = new Date(sortedComments.value[i - 1].createdAt).getTime()
        expect(previous).toBeGreaterThanOrEqual(current)
      }
    })

    it('should sort by oldest first', async () => {
      const { loadComments, sortedComments, sortBy } = useComments(
        'article',
        'test-id'
      )
      await loadComments()

      sortBy.value = 'oldest'

      // Verify sorted by date ascending
      for (let i = 1; i < sortedComments.value.length; i++) {
        const current = new Date(sortedComments.value[i].createdAt).getTime()
        const previous = new Date(sortedComments.value[i - 1].createdAt).getTime()
        expect(previous).toBeLessThanOrEqual(current)
      }
    })

    it('should sort by popularity', async () => {
      const { loadComments, sortedComments, sortBy } = useComments(
        'article',
        'test-id'
      )
      await loadComments()

      sortBy.value = 'popular'

      // Verify sorted by total reactions descending
      for (let i = 1; i < sortedComments.value.length; i++) {
        const currentReactions = sortedComments.value[i].reactions.reduce(
          (sum, r) => sum + r.count,
          0
        )
        const previousReactions = sortedComments.value[i - 1].reactions.reduce(
          (sum, r) => sum + r.count,
          0
        )
        expect(previousReactions).toBeGreaterThanOrEqual(currentReactions)
      }
    })
  })

  describe('totalComments', () => {
    it('should count all comments including replies', async () => {
      const { loadComments, totalComments, comments } = useComments(
        'article',
        'test-id'
      )
      await loadComments()

      // Count main comments + all replies
      const expectedTotal =
        comments.value.length +
        comments.value.reduce((sum, c) => sum + c.replies.length, 0)

      expect(totalComments.value).toBe(expectedTotal)
    })
  })

  describe('addComment', () => {
    it('should add a new comment', async () => {
      const { addComment, comments } = useComments('article', 'test-id')

      const newComment = await addComment('This is a new comment')

      expect(newComment).toBeDefined()
      expect(newComment.content).toBe('This is a new comment')
      expect(comments.value[0].content).toBe('This is a new comment')
    })

    it('should set submitting state during submission', async () => {
      const { isSubmitting, addComment } = useComments('article', 'test-id')

      const promise = addComment('Test comment')
      expect(isSubmitting.value).toBe(true)

      await promise
      expect(isSubmitting.value).toBe(false)
    })

    it('should add comment as first item', async () => {
      const { addComment, comments } = useComments('article', 'test-id')

      await addComment('First comment')
      await addComment('Second comment')

      expect(comments.value[0].content).toBe('Second comment')
    })

    it('should add reply to parent comment', async () => {
      const { loadComments, addComment, comments } = useComments(
        'article',
        'test-id'
      )
      await loadComments()

      const parentId = comments.value[0].id
      const initialReplies = comments.value[0].replies.length

      await addComment('This is a reply', parentId)

      expect(comments.value[0].replies.length).toBe(initialReplies + 1)
    })

    it('should extract mentions from comment', async () => {
      const { addComment } = useComments('article', 'test-id')

      const newComment = await addComment('Hello @john and @jane!')

      expect(newComment.mentions).toContain('john')
      expect(newComment.mentions).toContain('jane')
    })
  })

  describe('editComment', () => {
    it('should update comment content', async () => {
      const { loadComments, editComment, comments } = useComments(
        'article',
        'test-id'
      )
      await loadComments()

      const commentId = comments.value[0].id
      await editComment(commentId, 'Updated content')

      expect(comments.value[0].content).toBe('Updated content')
    })

    it('should mark comment as edited', async () => {
      const { loadComments, editComment, comments } = useComments(
        'article',
        'test-id'
      )
      await loadComments()

      const commentId = comments.value[0].id
      await editComment(commentId, 'Updated content')

      expect(comments.value[0].isEdited).toBe(true)
      expect(comments.value[0].updatedAt).toBeDefined()
    })
  })

  describe('deleteComment', () => {
    it('should remove comment from list', async () => {
      const { loadComments, deleteComment, comments } = useComments(
        'article',
        'test-id'
      )
      await loadComments()

      const initialLength = comments.value.length
      const commentId = comments.value[0].id

      await deleteComment(commentId)

      expect(comments.value.length).toBe(initialLength - 1)
      expect(comments.value.find((c) => c.id === commentId)).toBeUndefined()
    })

    it('should remove reply from parent comment', async () => {
      const { loadComments, deleteComment, comments } = useComments(
        'article',
        'test-id'
      )
      await loadComments()

      // Find a comment with replies
      const parentComment = comments.value.find((c) => c.replies.length > 0)
      if (parentComment) {
        const replyId = parentComment.replies[0].id
        const initialReplies = parentComment.replies.length

        await deleteComment(replyId)

        expect(parentComment.replies.length).toBe(initialReplies - 1)
      }
    })
  })

  describe('toggleReaction', () => {
    it('should add reaction if not exists', async () => {
      const { loadComments, toggleReaction, comments } = useComments(
        'article',
        'test-id'
      )
      await loadComments()

      const commentId = comments.value[0].id
      const initialReactionTypes = comments.value[0].reactions.map((r) => r.type)

      // Find a reaction type that doesn't exist
      const newReactionType = 'celebrate' as any
      if (!initialReactionTypes.includes(newReactionType)) {
        await toggleReaction(commentId, newReactionType)

        const addedReaction = comments.value[0].reactions.find(
          (r) => r.type === newReactionType
        )
        expect(addedReaction).toBeDefined()
        expect(addedReaction?.count).toBe(1)
        expect(addedReaction?.hasReacted).toBe(true)
      }
    })

    it('should toggle existing reaction', async () => {
      const { loadComments, toggleReaction, comments } = useComments(
        'article',
        'test-id'
      )
      await loadComments()

      const commentId = comments.value[0].id
      const reaction = comments.value[0].reactions[0]
      const initialCount = reaction.count
      const initialHasReacted = reaction.hasReacted

      await toggleReaction(commentId, reaction.type)

      if (initialHasReacted) {
        expect(reaction.count).toBe(initialCount - 1)
        expect(reaction.hasReacted).toBe(false)
      } else {
        expect(reaction.count).toBe(initialCount + 1)
        expect(reaction.hasReacted).toBe(true)
      }
    })
  })

  describe('formatTimeAgo', () => {
    it('should return "Just now" for recent times', () => {
      const { formatTimeAgo } = useComments('article', 'test-id')
      const now = new Date()
      expect(formatTimeAgo(now)).toBe('Just now')
    })

    it('should return minutes ago', () => {
      const { formatTimeAgo } = useComments('article', 'test-id')
      const thirtyMinutesAgo = new Date(Date.now() - 30 * 60 * 1000)
      expect(formatTimeAgo(thirtyMinutesAgo)).toMatch(/\d+m ago/)
    })

    it('should return hours ago', () => {
      const { formatTimeAgo } = useComments('article', 'test-id')
      const threeHoursAgo = new Date(Date.now() - 3 * 60 * 60 * 1000)
      expect(formatTimeAgo(threeHoursAgo)).toMatch(/\d+h ago/)
    })

    it('should return days ago', () => {
      const { formatTimeAgo } = useComments('article', 'test-id')
      const threeDaysAgo = new Date(Date.now() - 3 * 24 * 60 * 60 * 1000)
      expect(formatTimeAgo(threeDaysAgo)).toMatch(/\d+d ago/)
    })

    it('should return date for old times', () => {
      const { formatTimeAgo } = useComments('article', 'test-id')
      const twoWeeksAgo = new Date(Date.now() - 14 * 24 * 60 * 60 * 1000)
      // Should return formatted date
      expect(formatTimeAgo(twoWeeksAgo)).toMatch(/\d+\/\d+\/\d+/)
    })
  })
})
