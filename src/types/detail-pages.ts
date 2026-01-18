// Detail Pages Types

export interface Author {
  id: string
  name: string
  initials: string
  avatar?: string
  role: string
  department?: string
  email?: string
  bio?: string
  articlesCount?: number
  followersCount?: number
  isFollowing?: boolean
}

export interface Comment {
  id: string
  author: Author
  content: string
  createdAt: Date
  updatedAt?: Date
  replies: Comment[]
  reactions: Reaction[]
  mentions: string[]
  isEdited?: boolean
  isPinned?: boolean
}

export interface Reaction {
  type: 'like' | 'love' | 'helpful' | 'insightful' | 'celebrate'
  count: number
  hasReacted: boolean
}

export interface Rating {
  average: number
  count: number
  distribution: number[] // [1-star count, 2-star count, ..., 5-star count]
  userRating?: number
}

export interface RelatedItem {
  id: string
  type: 'article' | 'document' | 'media' | 'course' | 'event' | 'poll' | 'service' | 'lesson'
  title: string
  thumbnail?: string
  description?: string
  metadata: string
  author?: Author
  date?: Date
}

export interface VersionInfo {
  version: string
  author: Author
  date: Date
  changes: string
  isCurrent: boolean
}

export interface Attachment {
  id: string
  name: string
  type: string
  size: number
  url: string
  uploadedBy: Author
  uploadedAt: Date
}

export interface ShareOption {
  id: string
  name: string
  icon: string
  color: string
  action: () => void
}

export interface Bookmark {
  id: string
  contentId: string
  contentType: 'article' | 'document' | 'media' | 'course' | 'event' | 'poll' | 'service' | 'collection' | 'lesson'
  createdAt: Date
  collectionId?: string
}

// Event specific types
export interface EventAttendee {
  id: string
  name: string
  initials: string
  avatar?: string
  status: 'going' | 'maybe' | 'not_going' | 'pending'
  respondedAt?: Date
}

export interface EventAgendaItem {
  id: string
  time: string
  title: string
  description?: string
  speaker?: Author
  duration: number // in minutes
  type: 'session' | 'break' | 'networking' | 'keynote' | 'workshop'
}

export interface EventSpeaker {
  id: string
  name: string
  avatar?: string
  title: string
  company?: string
  bio: string
  sessions: string[]
  socialLinks?: {
    linkedin?: string
    twitter?: string
    website?: string
  }
}

// Poll specific types
export interface PollOption {
  id: string
  text: string
  votes: number
  percentage: number
  sentiment?: 'positive' | 'neutral' | 'negative'
  sentimentScore?: number
}

export interface PollVote {
  optionId: string
  votedAt: Date
  isAnonymous: boolean
}

// Service Request specific types
export interface ServiceRequestStep {
  id: string
  name?: string
  title?: string
  description?: string
  status: 'completed' | 'current' | 'pending'
  completedAt?: Date
  assignee?: Author
  notes?: string
}

export interface ServiceRequestActivity {
  id: string
  type: 'status_change' | 'comment' | 'assignment' | 'attachment' | 'update'
  description: string
  author: Author
  createdAt: Date
  metadata?: Record<string, any>
}
