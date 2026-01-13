// User & Auth Types
export interface User {
  id: string
  email: string
  firstName: string
  lastName: string
  displayName: string
  avatar?: string
  role: UserRole
  department?: string
  jobTitle?: string
  phone?: string
  location?: string
  bio?: string
  createdAt: string
  lastLoginAt?: string
}

export type UserRole = 'admin' | 'editor' | 'contributor' | 'viewer'

export interface AuthState {
  token: string | null
  refreshToken: string | null
  user: User | null
  isAuthenticated: boolean
  isLoading: boolean
}

export interface LoginCredentials {
  email: string
  password: string
  rememberMe?: boolean
}

export interface AuthResponse {
  token: string
  refreshToken: string
  user: User
  expiresIn: number
}

export interface SSOConfig {
  authUrl: string
  clientId: string
  redirectUri: string
  scopes: string[]
}

// Article Types
export interface Article {
  id: string
  title: string
  slug: string
  excerpt: string
  content: string
  coverImage?: string
  author: User
  category?: Category
  tags: Tag[]
  status: ContentStatus
  publishedAt?: string
  createdAt: string
  updatedAt: string
  viewCount: number
  likeCount: number
  commentCount: number
  readTime?: string
  isLiked?: boolean
  isBookmarked?: boolean
}

export interface Category {
  id: string
  name: string
  slug: string
  description?: string
  icon?: string
  color?: string
  parentId?: string
  articleCount: number
}

export interface Tag {
  id: string
  name: string
  slug: string
  articleCount: number
}

export type ContentStatus = 'draft' | 'pending' | 'published' | 'archived'

// Document Types
export interface Document {
  id: string
  name: string
  type: DocumentType
  size: number
  mimeType: string
  extension: string
  path: string
  url: string
  thumbnail?: string
  libraryId: string
  folderId?: string
  uploadedBy: User
  createdAt: string
  updatedAt: string
  version: number
  isShared: boolean
  shareLink?: string
}

export type DocumentType = 'pdf' | 'word' | 'excel' | 'powerpoint' | 'image' | 'video' | 'audio' | 'archive' | 'other'

export interface DocumentLibrary {
  id: string
  name: string
  description?: string
  icon?: string
  color?: string
  documentCount: number
  totalSize: number
  createdAt: string
}

export interface Folder {
  id: string
  name: string
  parentId?: string
  libraryId: string
  path: string
  documentCount: number
  createdAt: string
}

// Event Types
export interface CalendarEvent {
  id: string
  title: string
  description?: string
  location?: string
  startDate: string
  endDate: string
  allDay: boolean
  type: EventType
  status: EventStatus
  organizer: User
  attendees: EventAttendee[]
  isRecurring: boolean
  recurrencePattern?: string
  teamsLink?: string
  createdAt: string
}

export type EventType = 'meeting' | 'webinar' | 'training' | 'celebration' | 'announcement' | 'other'
export type EventStatus = 'upcoming' | 'ongoing' | 'completed' | 'cancelled'

export interface EventAttendee {
  user: User
  status: 'pending' | 'accepted' | 'declined' | 'tentative'
}

// Media Types
export interface MediaItem {
  id: string
  name: string
  type: MediaType
  url: string
  thumbnail?: string
  size: number
  duration?: number
  width?: number
  height?: number
  galleryId?: string
  uploadedBy: User
  createdAt: string
  viewCount: number
}

export type MediaType = 'image' | 'video' | 'audio'

export interface Gallery {
  id: string
  name: string
  description?: string
  coverImage?: string
  itemCount: number
  createdAt: string
}

// Poll Types
export interface Poll {
  id: string
  question: string
  description?: string
  options: PollOption[]
  type: PollType
  status: PollStatus
  startDate: string
  endDate?: string
  allowMultiple: boolean
  isAnonymous: boolean
  totalVotes: number
  createdBy: User
  createdAt: string
  hasVoted?: boolean
  userVotes?: string[]
}

export interface PollOption {
  id: string
  text: string
  voteCount: number
  percentage: number
}

export type PollType = 'poll' | 'survey' | 'quiz'
export type PollStatus = 'draft' | 'active' | 'closed'

// Learning Types
export interface Course {
  id: string
  title: string
  description: string
  thumbnail?: string
  instructor: User
  category: string
  level: CourseLevel
  duration: number
  lessonCount: number
  enrollmentCount: number
  rating: number
  ratingCount: number
  status: ContentStatus
  createdAt: string
  isEnrolled?: boolean
  progress?: number
}

export type CourseLevel = 'beginner' | 'intermediate' | 'advanced'

export interface Lesson {
  id: string
  courseId: string
  title: string
  description?: string
  type: LessonType
  content?: string
  videoUrl?: string
  duration: number
  order: number
  isCompleted?: boolean
}

export type LessonType = 'video' | 'article' | 'quiz' | 'assignment'

// Collaboration Types
export interface Team {
  id: string
  name: string
  description?: string
  avatar?: string
  memberCount: number
  isPrivate: boolean
  createdAt: string
  isMember?: boolean
}

export interface Community {
  id: string
  name: string
  description?: string
  coverImage?: string
  memberCount: number
  postCount: number
  category: string
  isPrivate: boolean
  createdAt: string
  isMember?: boolean
}

// Search Types
export interface SearchResult {
  id: string
  type: 'article' | 'document' | 'event' | 'course' | 'user' | 'team'
  title: string
  description?: string
  url: string
  thumbnail?: string
  score: number
  highlights?: string[]
  createdAt: string
}

export interface SearchFilters {
  type?: string[]
  category?: string[]
  dateRange?: { from: string; to: string }
  author?: string
}

// AI Types
export interface AIMessage {
  id: string
  role: 'user' | 'assistant'
  content: string
  timestamp: string
  citations?: AICitation[]
  isStreaming?: boolean
}

export interface AICitation {
  id: string
  title: string
  url: string
  snippet: string
  source: string
}

export interface AIConversation {
  id: string
  title: string
  messages: AIMessage[]
  createdAt: string
  updatedAt: string
}

// Notification Types
export interface Notification {
  id: string
  type: NotificationType
  title: string
  message: string
  link?: string
  icon?: string
  isRead: boolean
  createdAt: string
}

export type NotificationType = 'info' | 'success' | 'warning' | 'error' | 'mention' | 'assignment' | 'approval'

// Service Types
export interface ServiceRequest {
  id: string
  serviceId: string
  serviceName: string
  status: ServiceRequestStatus
  priority: 'low' | 'medium' | 'high' | 'urgent'
  description: string
  requestedBy: User
  assignedTo?: User
  createdAt: string
  updatedAt: string
  resolvedAt?: string
}

export type ServiceRequestStatus = 'pending' | 'in_progress' | 'completed' | 'cancelled'

export interface ServiceCatalogItem {
  id: string
  name: string
  description: string
  category: string
  icon?: string
  estimatedTime?: string
  isPopular: boolean
}

// Integration Types
export interface ERPWorkflow {
  id: string
  type: 'purchase_order' | 'expense_report' | 'leave_request' | 'invoice'
  title: string
  description: string
  amount?: number
  currency?: string
  requester: string
  createdAt: string
  status: 'pending' | 'approved' | 'rejected'
}

// Pagination Types
export interface PaginatedResponse<T> {
  items: T[]
  total: number
  page: number
  pageSize: number
  totalPages: number
  hasNext: boolean
  hasPrevious: boolean
}

export interface PaginationParams {
  page?: number
  pageSize?: number
  sortBy?: string
  sortOrder?: 'asc' | 'desc'
  search?: string
}

// API Response Types
export interface ApiResponse<T> {
  success: boolean
  data: T
  message?: string
  errors?: Record<string, string[]>
}

export interface ApiError {
  status: number
  message: string
  errors?: Record<string, string[]>
}

// Comparison Types
export type ComparisonItemType = 'document' | 'article' | 'media' | 'event'

export interface ComparisonItem {
  id: string
  type: ComparisonItemType
  title: string
  thumbnail?: string
  description?: string
  metadata: {
    author?: string
    date?: string
    size?: number
    duration?: number
    category?: string
    tags?: string[]
    [key: string]: any
  }
}

export interface ComparisonEntity {
  name: string
  type: 'person' | 'organization' | 'location' | 'date' | 'topic'
  occurrences: number
  items: string[] // IDs of items containing this entity
}

export interface SentimentData {
  itemId: string
  itemTitle: string
  score: number // -1 to 1
  label: 'positive' | 'neutral' | 'negative'
  confidence: number
}

export interface ComparisonAnalysis {
  id: string
  similarity: number
  summary: string
  commonEntities: ComparisonEntity[]
  sentimentComparison: SentimentData[]
  commonTopics: {
    topic: string
    relevance: number
    items: string[]
  }[]
  differences: string[]
  keyPoints: {
    itemId: string
    points: string[]
  }[]
  classifications: {
    itemId: string
    category: string
    confidence: number
  }[]
  createdAt: string
}
