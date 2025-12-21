// User Types
export interface User {
  id: string
  email: string
  displayName: string
  displayNameArabic?: string
  avatarUrl?: string
  jobTitle?: string
  jobTitleArabic?: string
  phoneNumber?: string
  departmentId?: string
  departmentName?: string
  managerId?: string
  managerName?: string
  preferredLanguage: 'en' | 'ar'
  roles: string[]
  permissions: string[]
  isActive: boolean
  lastLoginAt?: string
}

export interface AuthTokens {
  accessToken: string
  refreshToken: string
  expiresAt: string
}

export interface AuthResponse {
  user: User
  tokens: AuthTokens
}

// Content Types
export interface Article {
  id: string
  title: string
  titleArabic?: string
  slug: string
  content?: string
  contentArabic?: string
  summary?: string
  summaryArabic?: string
  featuredImageUrl?: string
  thumbnailUrl?: string
  authorId?: string
  authorName?: string
  type: 'article' | 'news' | 'blog' | 'page' | 'announcement'
  status: 'draft' | 'pending_review' | 'published' | 'archived'
  publishedAt?: string
  viewCount?: number
  averageRating?: number
  commentCount?: number
  categories?: Category[]
  tags?: Tag[]
  categoryName?: string
  categoryNameArabic?: string
  isFeatured?: boolean
  createdAt?: string
  modifiedAt?: string
}

export interface Category {
  id: string
  name: string
  nameArabic?: string
  slug: string
  parentId?: string
  description?: string
}

export interface Tag {
  id: string
  name: string
  nameArabic?: string
  slug: string
  color?: string
}

// Document Types
export interface Document {
  id: string
  name: string
  nameArabic?: string
  description?: string
  fileName?: string
  fileExtension?: string
  fileType: string
  fileSize: number
  mimeType: string
  libraryId: string
  folderId?: string
  version?: string
  currentVersion: number
  status?: string
  isCheckedOut: boolean
  checkedOutBy?: string
  checkedOutAt?: string
  createdBy: string
  createdByName?: string
  createdAt: string
  modifiedBy?: string
  modifiedAt?: string
  updatedAt?: string
}

export interface DocumentLibrary {
  id: string
  name: string
  nameArabic?: string
  description?: string
  ownerId: string
  communityId?: string
  documentCount: number
  iconName?: string
  color?: string
  type?: string
  totalSize?: number
  createdAt: string
}

export interface DocumentVersion {
  id: string
  documentId: string
  versionNumber: number
  majorVersion: number
  minorVersion: number
  fileSize: number
  comment?: string
  createdBy: string
  createdByName: string
  createdAt: string
}

export interface Folder {
  id: string
  name: string
  nameArabic?: string
  parentId?: string
  documentCount: number
  childFolderCount: number
}

// Collaboration Types
export interface Community {
  id: string
  name: string
  nameArabic?: string
  description?: string
  ownerId: string
  ownerName: string
  memberCount: number
  isPrivate: boolean
  createdAt: string
}

export interface Discussion {
  id: string
  communityId: string
  title: string
  content: string
  authorId: string
  authorName: string
  authorAvatar?: string
  isPinned: boolean
  isLocked: boolean
  viewCount: number
  replyCount: number
  createdAt: string
  lastActivityAt: string
}

export interface Comment {
  id: string
  content: string
  authorId: string
  authorName: string
  authorAvatar?: string
  parentId?: string
  likeCount: number
  isLikedByMe: boolean
  createdAt: string
  modifiedAt?: string
  replies?: Comment[]
}

// Calendar Types
export interface CalendarEvent {
  id: string
  title: string
  titleArabic?: string
  description?: string
  descriptionArabic?: string
  location?: string
  startDate: string
  endDate: string
  isAllDay: boolean
  organizerId: string
  organizerName: string
  attendeeCount: number
  myRsvpStatus?: 'attending' | 'maybe' | 'not_attending' | 'pending'
  createdAt: string
}

// Notification Types
export interface Notification {
  id: string
  type: 'mention' | 'reply' | 'share' | 'task' | 'system'
  title: string
  message: string
  link?: string
  isRead: boolean
  createdAt: string
}

// Search Types
export interface SearchResult {
  id: string
  type: 'article' | 'document' | 'user' | 'community' | 'discussion'
  title: string
  summary?: string
  url: string
  createdAt: string
  score: number
}

export interface SearchResponse {
  items: SearchResult[]
  totalCount: number
  page: number
  pageSize: number
  facets: SearchFacet[]
}

export interface SearchFacet {
  field: string
  values: { value: string; count: number }[]
}

// API Response Types
export interface ApiResponse<T> {
  success: boolean
  data?: T
  message?: string
  errors?: ApiError[]
  timestamp: string
}

export interface ApiError {
  code: string
  message: string
  field?: string
}

export interface PagedResult<T> {
  items: T[]
  totalCount: number
  page: number
  pageSize: number
  totalPages: number
  hasNextPage: boolean
  hasPreviousPage: boolean
}
