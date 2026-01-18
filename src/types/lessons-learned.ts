// Lessons Learned Types

export type LessonPriority = 'low' | 'medium' | 'high' | 'critical'
export type LessonCategory = 'technical' | 'process' | 'communication' | 'leadership' | 'project' | 'other'

export interface LessonLearned {
  id: string
  title: string
  summary: string
  content: string
  category: LessonCategory
  priority: LessonPriority
  tags: string[]
  status: 'draft' | 'published'

  // Context (optional)
  context?: string
  impact?: string
  recommendations?: string[]

  // Metadata
  author: {
    id: string
    name: string
    initials: string
    avatar?: string
  }
  createdAt: string
  updatedAt: string

  // Engagement
  viewCount: number
  likeCount: number
  commentCount: number
  isLiked?: boolean
  isBookmarked?: boolean
}

export interface LessonLearnedFilters {
  search?: string
  category?: LessonCategory | 'all'
  priority?: LessonPriority | 'all'
  status?: 'draft' | 'published' | 'all'
  authorId?: string
  tags?: string[]
  sortBy?: 'recent' | 'popular' | 'priority'
}

// Helper functions for display
export function getPriorityColor(priority: LessonPriority): string {
  switch (priority) {
    case 'critical': return 'bg-red-100 text-red-700 border-red-200'
    case 'high': return 'bg-orange-100 text-orange-700 border-orange-200'
    case 'medium': return 'bg-amber-100 text-amber-700 border-amber-200'
    case 'low': return 'bg-green-100 text-green-700 border-green-200'
    default: return 'bg-gray-100 text-gray-700 border-gray-200'
  }
}

export function getCategoryColor(category: LessonCategory): string {
  switch (category) {
    case 'technical': return 'bg-blue-100 text-blue-700'
    case 'process': return 'bg-purple-100 text-purple-700'
    case 'communication': return 'bg-teal-100 text-teal-700'
    case 'leadership': return 'bg-indigo-100 text-indigo-700'
    case 'project': return 'bg-cyan-100 text-cyan-700'
    case 'other': return 'bg-gray-100 text-gray-700'
    default: return 'bg-gray-100 text-gray-700'
  }
}

export function getCategoryIcon(category: LessonCategory): string {
  switch (category) {
    case 'technical': return 'fas fa-code'
    case 'process': return 'fas fa-sitemap'
    case 'communication': return 'fas fa-comments'
    case 'leadership': return 'fas fa-users-cog'
    case 'project': return 'fas fa-project-diagram'
    case 'other': return 'fas fa-folder'
    default: return 'fas fa-folder'
  }
}

export function getPriorityIcon(priority: LessonPriority): string {
  switch (priority) {
    case 'critical': return 'fas fa-exclamation-circle'
    case 'high': return 'fas fa-arrow-up'
    case 'medium': return 'fas fa-minus'
    case 'low': return 'fas fa-arrow-down'
    default: return 'fas fa-minus'
  }
}

export const LESSON_CATEGORIES: { value: LessonCategory; label: string; icon: string }[] = [
  { value: 'technical', label: 'Technical', icon: 'fas fa-code' },
  { value: 'process', label: 'Process', icon: 'fas fa-sitemap' },
  { value: 'communication', label: 'Communication', icon: 'fas fa-comments' },
  { value: 'leadership', label: 'Leadership', icon: 'fas fa-users-cog' },
  { value: 'project', label: 'Project', icon: 'fas fa-project-diagram' },
  { value: 'other', label: 'Other', icon: 'fas fa-folder' }
]

export const LESSON_PRIORITIES: { value: LessonPriority; label: string; icon: string }[] = [
  { value: 'critical', label: 'Critical', icon: 'fas fa-exclamation-circle' },
  { value: 'high', label: 'High', icon: 'fas fa-arrow-up' },
  { value: 'medium', label: 'Medium', icon: 'fas fa-minus' },
  { value: 'low', label: 'Low', icon: 'fas fa-arrow-down' }
]
