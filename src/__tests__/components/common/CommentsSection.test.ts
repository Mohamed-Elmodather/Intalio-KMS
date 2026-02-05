import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import CommentsSection from '@/components/common/CommentsSection.vue'

// Mock vue-i18n
vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string, params?: any) => params ? `${key} ${JSON.stringify(params)}` : key,
  }),
}))

// Mock comments composable
const mockLoadComments = vi.fn()
const mockAddComment = vi.fn()
const mockDeleteComment = vi.fn()
const mockToggleReaction = vi.fn()

const mockComments = [
  {
    id: 'comment-1',
    content: 'Great article!',
    author: { name: 'John Doe', initials: 'JD', role: 'Editor' },
    createdAt: new Date().toISOString(),
    isPinned: true,
    isEdited: false,
    reactions: [{ type: 'like', count: 5, hasReacted: false }],
    replies: [
      {
        id: 'reply-1',
        content: 'Thanks!',
        author: { name: 'Jane Smith', initials: 'JS' },
        createdAt: new Date().toISOString(),
        reactions: [],
      },
    ],
  },
  {
    id: 'comment-2',
    content: 'Very informative',
    author: { name: 'Bob Wilson', initials: 'BW', role: 'Reader' },
    createdAt: new Date().toISOString(),
    isPinned: false,
    isEdited: true,
    reactions: [],
    replies: [],
  },
]

vi.mock('@/composables/useComments', () => ({
  useComments: () => ({
    sortedComments: mockComments,
    totalComments: 2,
    isLoading: false,
    isSubmitting: false,
    sortBy: { value: 'newest' },
    currentUser: { initials: 'ME' },
    loadComments: mockLoadComments,
    addComment: mockAddComment,
    deleteComment: mockDeleteComment,
    toggleReaction: mockToggleReaction,
    formatTimeAgo: (date: string) => '2 hours ago',
  }),
}))

function mountComponent(props = {}) {
  return shallowMount(CommentsSection, {
    props: {
      contentType: 'article',
      contentId: '123',
      ...props,
    },
    global: {
      mocks: {
        $t: (key: string, params?: any) => params ? `${key} ${JSON.stringify(params)}` : key,
      },
    },
  })
}

describe('CommentsSection', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('should render the component', () => {
      const wrapper = mountComponent()
      expect(wrapper.exists()).toBe(true)
      expect(wrapper.find('.comments-section').exists()).toBe(true)
    })

    it('should render header with title', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('comments.title')
    })

    it('should render total comments count', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('(2)')
    })

    it('should render sort dropdown', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('select').exists()).toBe(true)
    })
  })

  describe('Comment Form', () => {
    it('should render comment textarea', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('textarea').exists()).toBe(true)
    })

    it('should render submit button', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('comments.postComment')
    })

    it('should show current user initials', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('ME')
    })
  })

  describe('Comments List', () => {
    it('should render comments', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('Great article!')
      expect(wrapper.text()).toContain('Very informative')
    })

    it('should show pinned badge for pinned comments', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('comments.pinnedComment')
    })

    it('should show edited badge for edited comments', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('comments.edited')
    })

    it('should display author name', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('John Doe')
      expect(wrapper.text()).toContain('Bob Wilson')
    })

    it('should display author role', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('Editor')
      expect(wrapper.text()).toContain('Reader')
    })
  })

  describe('Reactions', () => {
    it('should have getReactionIcon function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.getReactionIcon).toBe('function')
    })

    it('should return correct icon for like reaction', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getReactionIcon('like')).toBe('fas fa-thumbs-up')
    })

    it('should return correct icon for love reaction', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getReactionIcon('love')).toBe('fas fa-heart')
    })

    it('should return correct icon for helpful reaction', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getReactionIcon('helpful')).toBe('fas fa-hands-helping')
    })

    it('should return correct icon for insightful reaction', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getReactionIcon('insightful')).toBe('fas fa-lightbulb')
    })

    it('should return default icon for unknown reaction', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getReactionIcon('unknown')).toBe('fas fa-thumbs-up')
    })
  })

  describe('Reply Functionality', () => {
    it('should show reply button', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('comments.reply')
    })

    it('should start reply mode', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.startReply('comment-1')
      expect(vm.replyingTo).toBe('comment-1')
    })

    it('should cancel reply mode', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.startReply('comment-1')
      vm.cancelReply()
      expect(vm.replyingTo).toBeNull()
      expect(vm.replyContent).toBe('')
    })
  })

  describe('Expand Replies', () => {
    it('should toggle replies visibility', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.expandedReplies.has('comment-1')).toBe(false)
      vm.toggleReplies('comment-1')
      expect(vm.expandedReplies.has('comment-1')).toBe(true)
      vm.toggleReplies('comment-1')
      expect(vm.expandedReplies.has('comment-1')).toBe(false)
    })

    it('should show replies count button', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('comments.showReplies')
    })
  })

  describe('Submit Comment', () => {
    it('should have handleSubmitComment function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.handleSubmitComment).toBe('function')
    })

    it('should not submit empty comment', async () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.newComment = '   '
      await vm.handleSubmitComment()
      expect(mockAddComment).not.toHaveBeenCalled()
    })

    it('should clear comment after submission', async () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.newComment = 'Test comment'
      await vm.handleSubmitComment()
      expect(vm.newComment).toBe('')
    })
  })

  describe('Submit Reply', () => {
    it('should have handleSubmitReply function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.handleSubmitReply).toBe('function')
    })

    it('should not submit empty reply', async () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.replyContent = '   '
      await vm.handleSubmitReply('comment-1')
      expect(mockAddComment).not.toHaveBeenCalled()
    })

    it('should expand replies after submission', async () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.replyContent = 'Test reply'
      await vm.handleSubmitReply('comment-1')
      expect(vm.expandedReplies.has('comment-1')).toBe(true)
    })
  })

  describe('Sort Options', () => {
    it('should have newest option', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('comments.newestFirst')
    })

    it('should have oldest option', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('comments.oldestFirst')
    })

    it('should have popular option', () => {
      const wrapper = mountComponent()
      expect(wrapper.text()).toContain('comments.mostPopular')
    })
  })

  describe('Load Comments', () => {
    it('should call loadComments on mount', () => {
      mountComponent()
      expect(mockLoadComments).toHaveBeenCalled()
    })
  })
})
