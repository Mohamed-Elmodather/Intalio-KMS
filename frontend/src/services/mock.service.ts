/**
 * Mock Service for AFC 2027 KMS
 * Provides mock data when backend is unavailable
 */

import type {
  User,
  Article,
  Category,
  Tag,
  Document,
  DocumentLibrary,
  Community,
  Discussion,
  CalendarEvent,
  Notification,
  PagedResult,
  ApiResponse
} from '@/types'

// Mock Users (AFC 2027 Staff)
export const mockUsers: User[] = [
  {
    id: '1',
    email: 'ahmed.alharbi@afc2027.com',
    displayName: 'Ahmed Al-Harbi',
    displayNameArabic: 'أحمد الحربي',
    avatarUrl: 'https://api.dicebear.com/7.x/initials/svg?seed=AH',
    jobTitle: 'Operations Director',
    jobTitleArabic: 'مدير العمليات',
    departmentId: '1',
    departmentName: 'Operations',
    preferredLanguage: 'en',
    roles: ['Administrator', 'Operations'],
    permissions: ['content:create', 'content:publish', 'documents:upload'],
    isActive: true,
    lastLoginAt: new Date().toISOString()
  },
  {
    id: '2',
    email: 'sara.almutairi@afc2027.com',
    displayName: 'Sara Al-Mutairi',
    displayNameArabic: 'سارة المطيري',
    avatarUrl: 'https://api.dicebear.com/7.x/initials/svg?seed=SM',
    jobTitle: 'Media Director',
    jobTitleArabic: 'مديرة الإعلام',
    departmentId: '2',
    departmentName: 'Media & Communications',
    preferredLanguage: 'ar',
    roles: ['Manager', 'Media'],
    permissions: ['content:create', 'content:publish'],
    isActive: true
  },
  {
    id: '3',
    email: 'mohammed.aldosari@afc2027.com',
    displayName: 'Mohammed Al-Dosari',
    displayNameArabic: 'محمد الدوسري',
    avatarUrl: 'https://api.dicebear.com/7.x/initials/svg?seed=MD',
    jobTitle: 'Venues Director',
    jobTitleArabic: 'مدير المنشآت',
    departmentId: '3',
    departmentName: 'Venues',
    preferredLanguage: 'en',
    roles: ['Manager', 'Venues'],
    permissions: ['content:create'],
    isActive: true
  }
]

// Mock Categories
export const mockCategories: Category[] = [
  { id: '1', name: 'News', nameArabic: 'الأخبار', slug: 'news', description: 'Latest tournament news' },
  { id: '2', name: 'Venues', nameArabic: 'الملاعب', slug: 'venues', description: 'Stadium and venue information' },
  { id: '3', name: 'Teams', nameArabic: 'المنتخبات', slug: 'teams', description: 'Participating teams information' },
  { id: '4', name: 'Ticketing', nameArabic: 'التذاكر', slug: 'ticketing', description: 'Ticketing information' },
  { id: '5', name: 'Volunteers', nameArabic: 'المتطوعين', slug: 'volunteers', description: 'Volunteer program updates' },
  { id: '6', name: 'Operations', nameArabic: 'العمليات', slug: 'operations', description: 'Operational updates' }
]

// Mock Tags
export const mockTags: Tag[] = [
  { id: '1', name: 'AFC2027', nameArabic: 'آسيا2027', slug: 'afc2027', color: '#1E40AF' },
  { id: '2', name: 'Football', nameArabic: 'كرة القدم', slug: 'football', color: '#059669' },
  { id: '3', name: 'Saudi Arabia', nameArabic: 'السعودية', slug: 'saudi-arabia', color: '#047857' },
  { id: '4', name: 'Venues', nameArabic: 'الملاعب', slug: 'venues', color: '#7C3AED' },
  { id: '5', name: 'Ticketing', nameArabic: 'التذاكر', slug: 'ticketing', color: '#DB2777' },
  { id: '6', name: 'Volunteers', nameArabic: 'المتطوعين', slug: 'volunteers', color: '#EA580C' }
]

// Mock Articles (AFC 2027 themed)
export const mockArticles: Article[] = [
  {
    id: '1',
    title: 'AFC Asian Cup 2027 Venues Officially Announced',
    titleArabic: 'الإعلان الرسمي عن ملاعب كأس آسيا 2027',
    slug: 'afc-asian-cup-2027-venues-announced',
    summary: 'Eight world-class stadiums across four host cities will welcome teams and fans from across Asia.',
    summaryArabic: 'ستستقبل ثمانية ملاعب عالمية المستوى في أربع مدن مضيفة الفرق والمشجعين من جميع أنحاء آسيا.',
    content: '<p>The Saudi Arabian Football Federation has officially unveiled the venues for AFC Asian Cup 2027...</p>',
    contentArabic: '<p>أعلن الاتحاد السعودي لكرة القدم رسمياً عن ملاعب كأس آسيا 2027...</p>',
    featuredImageUrl: '/images/venues-announcement.jpg',
    thumbnailUrl: '/images/venues-announcement-thumb.jpg',
    authorId: '1',
    authorName: 'Ahmed Al-Harbi',
    type: 'news',
    status: 'published',
    publishedAt: new Date(Date.now() - 2 * 24 * 60 * 60 * 1000).toISOString(),
    viewCount: 15420,
    commentCount: 45,
    categoryName: 'News',
    categoryNameArabic: 'الأخبار',
    isFeatured: true,
    tags: [mockTags[0], mockTags[3]]
  },
  {
    id: '2',
    title: 'Volunteer Registration Opens for AFC Asian Cup 2027',
    titleArabic: 'فتح باب التسجيل للمتطوعين في كأس آسيا 2027',
    slug: 'volunteer-registration-opens',
    summary: 'Join thousands of passionate individuals in delivering an unforgettable tournament experience.',
    summaryArabic: 'انضم إلى آلاف المتحمسين في تقديم تجربة بطولة لا تُنسى.',
    content: '<p>Be part of history! The volunteer program for AFC Asian Cup 2027 is now accepting applications...</p>',
    contentArabic: '<p>كن جزءاً من التاريخ! برنامج التطوع لكأس آسيا 2027 يستقبل الآن الطلبات...</p>',
    featuredImageUrl: '/images/volunteers.jpg',
    authorId: '2',
    authorName: 'Sara Al-Mutairi',
    type: 'announcement',
    status: 'published',
    publishedAt: new Date(Date.now() - 5 * 24 * 60 * 60 * 1000).toISOString(),
    viewCount: 8500,
    commentCount: 120,
    categoryName: 'Volunteers',
    categoryNameArabic: 'المتطوعين',
    isFeatured: true,
    tags: [mockTags[0], mockTags[5]]
  },
  {
    id: '3',
    title: 'Qualification Path Confirmed for AFC Asian Cup 2027',
    titleArabic: 'تأكيد مسار التأهل لكأس آسيا 2027',
    slug: 'qualification-path-confirmed',
    summary: '24 teams will compete in the expanded format, providing more opportunities for Asian nations.',
    summaryArabic: 'ستتنافس 24 فريقاً في الشكل الموسع، مما يوفر المزيد من الفرص للدول الآسيوية.',
    content: '<p>The Asian Football Confederation has confirmed the qualification pathway...</p>',
    authorId: '1',
    authorName: 'Ahmed Al-Harbi',
    type: 'news',
    status: 'published',
    publishedAt: new Date(Date.now() - 7 * 24 * 60 * 60 * 1000).toISOString(),
    viewCount: 12300,
    commentCount: 89,
    categoryName: 'News',
    isFeatured: false,
    tags: [mockTags[0], mockTags[1]]
  },
  {
    id: '4',
    title: 'Ticketing Information for AFC Asian Cup 2027',
    titleArabic: 'معلومات التذاكر لكأس آسيا 2027',
    slug: 'ticketing-information',
    summary: 'Everything you need to know about ticket sales, pricing, and how to secure your seats.',
    summaryArabic: 'كل ما تحتاج معرفته عن مبيعات التذاكر والأسعار وكيفية حجز مقاعدك.',
    content: '<p>Planning to attend AFC Asian Cup 2027? Here\'s everything you need to know...</p>',
    authorId: '3',
    authorName: 'Mohammed Al-Dosari',
    type: 'article',
    status: 'published',
    publishedAt: new Date(Date.now() - 10 * 24 * 60 * 60 * 1000).toISOString(),
    viewCount: 9800,
    commentCount: 56,
    categoryName: 'Ticketing',
    isFeatured: false,
    tags: [mockTags[0], mockTags[4]]
  },
  {
    id: '5',
    title: 'State-of-the-Art Technology at AFC Asian Cup 2027',
    titleArabic: 'تقنيات متطورة في كأس آسيا 2027',
    slug: 'state-of-the-art-technology',
    summary: 'VAR, goal-line technology, and advanced stadium connectivity will enhance the experience.',
    summaryArabic: 'ستعزز تقنيات VAR وخط المرمى والاتصال المتقدم في الملاعب التجربة.',
    content: '<p>AFC Asian Cup 2027 will showcase cutting-edge technology...</p>',
    authorId: '2',
    authorName: 'Sara Al-Mutairi',
    type: 'article',
    status: 'published',
    publishedAt: new Date(Date.now() - 14 * 24 * 60 * 60 * 1000).toISOString(),
    viewCount: 6500,
    commentCount: 34,
    categoryName: 'Operations',
    isFeatured: false,
    tags: [mockTags[0], mockTags[1]]
  }
]

// Mock Document Libraries
export const mockLibraries: DocumentLibrary[] = [
  {
    id: '1',
    name: 'Tournament Operations',
    nameArabic: 'عمليات البطولة',
    description: 'Operational documents and procedures',
    ownerId: '1',
    documentCount: 45,
    iconName: 'pi-folder',
    color: '#1E40AF',
    type: 'operational',
    totalSize: 156000000,
    createdAt: new Date(Date.now() - 30 * 24 * 60 * 60 * 1000).toISOString()
  },
  {
    id: '2',
    name: 'Media Assets',
    nameArabic: 'الأصول الإعلامية',
    description: 'Logos, images, and brand guidelines',
    ownerId: '2',
    documentCount: 120,
    iconName: 'pi-image',
    color: '#059669',
    type: 'media',
    totalSize: 890000000,
    createdAt: new Date(Date.now() - 60 * 24 * 60 * 60 * 1000).toISOString()
  },
  {
    id: '3',
    name: 'Venue Documentation',
    nameArabic: 'وثائق الملاعب',
    description: 'Stadium specifications and layouts',
    ownerId: '3',
    documentCount: 78,
    iconName: 'pi-building',
    color: '#7C3AED',
    type: 'venues',
    totalSize: 450000000,
    createdAt: new Date(Date.now() - 45 * 24 * 60 * 60 * 1000).toISOString()
  }
]

// Mock Documents
export const mockDocuments: Document[] = [
  {
    id: '1',
    name: 'Tournament Operations Manual',
    nameArabic: 'دليل عمليات البطولة',
    description: 'Complete operational procedures for AFC Asian Cup 2027',
    fileName: 'tournament-operations-manual-v3.pdf',
    fileExtension: '.pdf',
    fileType: 'pdf',
    fileSize: 12500000,
    mimeType: 'application/pdf',
    libraryId: '1',
    currentVersion: 3,
    status: 'published',
    isCheckedOut: false,
    createdBy: '1',
    createdByName: 'Ahmed Al-Harbi',
    createdAt: new Date(Date.now() - 20 * 24 * 60 * 60 * 1000).toISOString(),
    modifiedAt: new Date(Date.now() - 5 * 24 * 60 * 60 * 1000).toISOString()
  },
  {
    id: '2',
    name: 'Brand Guidelines 2027',
    nameArabic: 'إرشادات العلامة التجارية 2027',
    description: 'Official brand guidelines and usage rules',
    fileName: 'brand-guidelines-2027.pdf',
    fileExtension: '.pdf',
    fileType: 'pdf',
    fileSize: 8900000,
    mimeType: 'application/pdf',
    libraryId: '2',
    currentVersion: 2,
    status: 'published',
    isCheckedOut: false,
    createdBy: '2',
    createdByName: 'Sara Al-Mutairi',
    createdAt: new Date(Date.now() - 45 * 24 * 60 * 60 * 1000).toISOString()
  },
  {
    id: '3',
    name: 'King Fahd Stadium Layout',
    nameArabic: 'تخطيط ملعب الملك فهد',
    description: 'Detailed stadium layout and seating plan',
    fileName: 'king-fahd-stadium-layout.dwg',
    fileExtension: '.dwg',
    fileType: 'cad',
    fileSize: 45000000,
    mimeType: 'application/acad',
    libraryId: '3',
    currentVersion: 5,
    status: 'published',
    isCheckedOut: true,
    checkedOutBy: '3',
    checkedOutAt: new Date(Date.now() - 2 * 24 * 60 * 60 * 1000).toISOString(),
    createdBy: '3',
    createdByName: 'Mohammed Al-Dosari',
    createdAt: new Date(Date.now() - 30 * 24 * 60 * 60 * 1000).toISOString()
  }
]

// Mock Communities
export const mockCommunities: Community[] = [
  {
    id: '1',
    name: 'Operations Team',
    nameArabic: 'فريق العمليات',
    description: 'Coordination and planning for tournament operations',
    ownerId: '1',
    ownerName: 'Ahmed Al-Harbi',
    memberCount: 45,
    isPrivate: false,
    createdAt: new Date(Date.now() - 60 * 24 * 60 * 60 * 1000).toISOString()
  },
  {
    id: '2',
    name: 'Media & Communications',
    nameArabic: 'الإعلام والاتصالات',
    description: 'Media coverage and communication strategies',
    ownerId: '2',
    ownerName: 'Sara Al-Mutairi',
    memberCount: 32,
    isPrivate: false,
    createdAt: new Date(Date.now() - 45 * 24 * 60 * 60 * 1000).toISOString()
  },
  {
    id: '3',
    name: 'Volunteer Coordinators',
    nameArabic: 'منسقي المتطوعين',
    description: 'Volunteer program management and coordination',
    ownerId: '2',
    ownerName: 'Sara Al-Mutairi',
    memberCount: 28,
    isPrivate: false,
    createdAt: new Date(Date.now() - 30 * 24 * 60 * 60 * 1000).toISOString()
  }
]

// Mock Discussions
export const mockDiscussions: Discussion[] = [
  {
    id: '1',
    communityId: '1',
    title: 'Opening Ceremony Planning Updates',
    content: 'Please share your latest updates on the opening ceremony preparations...',
    authorId: '1',
    authorName: 'Ahmed Al-Harbi',
    authorAvatar: 'https://api.dicebear.com/7.x/initials/svg?seed=AH',
    isPinned: true,
    isLocked: false,
    viewCount: 234,
    replyCount: 18,
    createdAt: new Date(Date.now() - 3 * 24 * 60 * 60 * 1000).toISOString(),
    lastActivityAt: new Date(Date.now() - 2 * 60 * 60 * 1000).toISOString()
  },
  {
    id: '2',
    communityId: '2',
    title: 'Press Conference Schedule',
    content: 'Here is the updated schedule for upcoming press conferences...',
    authorId: '2',
    authorName: 'Sara Al-Mutairi',
    authorAvatar: 'https://api.dicebear.com/7.x/initials/svg?seed=SM',
    isPinned: false,
    isLocked: false,
    viewCount: 156,
    replyCount: 12,
    createdAt: new Date(Date.now() - 5 * 24 * 60 * 60 * 1000).toISOString(),
    lastActivityAt: new Date(Date.now() - 12 * 60 * 60 * 1000).toISOString()
  }
]

// Mock Calendar Events
export const mockEvents: CalendarEvent[] = [
  {
    id: '1',
    title: 'Opening Ceremony',
    titleArabic: 'حفل الافتتاح',
    description: 'Grand opening ceremony of AFC Asian Cup 2027',
    descriptionArabic: 'حفل الافتتاح الكبير لكأس آسيا 2027',
    location: 'King Fahd International Stadium, Riyadh',
    startDate: '2027-01-08T18:00:00Z',
    endDate: '2027-01-08T21:00:00Z',
    isAllDay: false,
    organizerId: '1',
    organizerName: 'Ahmed Al-Harbi',
    attendeeCount: 50,
    myRsvpStatus: 'attending',
    createdAt: new Date(Date.now() - 30 * 24 * 60 * 60 * 1000).toISOString()
  },
  {
    id: '2',
    title: 'Operations Review Meeting',
    titleArabic: 'اجتماع مراجعة العمليات',
    description: 'Weekly operations status review',
    location: 'Conference Room A',
    startDate: new Date(Date.now() + 2 * 24 * 60 * 60 * 1000).toISOString(),
    endDate: new Date(Date.now() + 2 * 24 * 60 * 60 * 1000 + 2 * 60 * 60 * 1000).toISOString(),
    isAllDay: false,
    organizerId: '1',
    organizerName: 'Ahmed Al-Harbi',
    attendeeCount: 15,
    myRsvpStatus: 'pending',
    createdAt: new Date(Date.now() - 7 * 24 * 60 * 60 * 1000).toISOString()
  },
  {
    id: '3',
    title: 'Media Training Workshop',
    titleArabic: 'ورشة تدريب الإعلام',
    description: 'Training session for media team',
    location: 'Media Center',
    startDate: new Date(Date.now() + 5 * 24 * 60 * 60 * 1000).toISOString(),
    endDate: new Date(Date.now() + 5 * 24 * 60 * 60 * 1000 + 4 * 60 * 60 * 1000).toISOString(),
    isAllDay: false,
    organizerId: '2',
    organizerName: 'Sara Al-Mutairi',
    attendeeCount: 25,
    createdAt: new Date(Date.now() - 14 * 24 * 60 * 60 * 1000).toISOString()
  }
]

// Mock Notifications
export const mockNotifications: Notification[] = [
  {
    id: '1',
    type: 'task',
    title: 'New Task Assigned',
    message: 'Review venue security plans for King Fahd Stadium',
    link: '/workflow/tasks/1',
    isRead: false,
    createdAt: new Date(Date.now() - 2 * 60 * 60 * 1000).toISOString()
  },
  {
    id: '2',
    type: 'mention',
    title: 'You were mentioned',
    message: 'Ahmed Al-Harbi mentioned you in "Opening Ceremony Planning"',
    link: '/collaboration/discussions/1',
    isRead: false,
    createdAt: new Date(Date.now() - 5 * 60 * 60 * 1000).toISOString()
  },
  {
    id: '3',
    type: 'share',
    title: 'Document shared with you',
    message: 'Sara Al-Mutairi shared "Brand Guidelines 2027" with you',
    link: '/documents/2',
    isRead: true,
    createdAt: new Date(Date.now() - 24 * 60 * 60 * 1000).toISOString()
  }
]

// Helper function to create paginated response
function createPagedResult<T>(items: T[], page: number = 1, pageSize: number = 20): PagedResult<T> {
  const startIndex = (page - 1) * pageSize
  const paginatedItems = items.slice(startIndex, startIndex + pageSize)
  const totalCount = items.length
  const totalPages = Math.ceil(totalCount / pageSize)

  return {
    items: paginatedItems,
    totalCount,
    page,
    pageSize,
    totalPages,
    hasNextPage: page < totalPages,
    hasPreviousPage: page > 1
  }
}

// Helper function to create API response
function createApiResponse<T>(data: T, success: boolean = true, message?: string): ApiResponse<T> {
  return {
    success,
    data,
    message,
    timestamp: new Date().toISOString()
  }
}

/**
 * Mock Service - provides mock data for development/testing
 */
class MockService {
  private delay = 300 // Simulate network delay

  private async simulateDelay(): Promise<void> {
    return new Promise(resolve => setTimeout(resolve, this.delay))
  }

  // Users
  async getUsers(page: number = 1, pageSize: number = 20): Promise<ApiResponse<PagedResult<User>>> {
    await this.simulateDelay()
    return createApiResponse(createPagedResult(mockUsers, page, pageSize))
  }

  async getCurrentUser(): Promise<ApiResponse<User>> {
    await this.simulateDelay()
    return createApiResponse(mockUsers[0])
  }

  // Articles
  async getArticles(page: number = 1, pageSize: number = 20): Promise<ApiResponse<PagedResult<Article>>> {
    await this.simulateDelay()
    return createApiResponse(createPagedResult(mockArticles, page, pageSize))
  }

  async getArticle(id: string): Promise<ApiResponse<Article>> {
    await this.simulateDelay()
    const article = mockArticles.find(a => a.id === id)
    if (!article) {
      return { success: false, message: 'Article not found', timestamp: new Date().toISOString() }
    }
    return createApiResponse(article)
  }

  async getFeaturedArticles(): Promise<ApiResponse<Article[]>> {
    await this.simulateDelay()
    return createApiResponse(mockArticles.filter(a => a.isFeatured))
  }

  // Categories
  async getCategories(): Promise<ApiResponse<Category[]>> {
    await this.simulateDelay()
    return createApiResponse(mockCategories)
  }

  // Tags
  async getTags(): Promise<ApiResponse<Tag[]>> {
    await this.simulateDelay()
    return createApiResponse(mockTags)
  }

  // Documents
  async getLibraries(): Promise<ApiResponse<DocumentLibrary[]>> {
    await this.simulateDelay()
    return createApiResponse(mockLibraries)
  }

  async getDocuments(libraryId?: string, page: number = 1, pageSize: number = 20): Promise<ApiResponse<PagedResult<Document>>> {
    await this.simulateDelay()
    const docs = libraryId
      ? mockDocuments.filter(d => d.libraryId === libraryId)
      : mockDocuments
    return createApiResponse(createPagedResult(docs, page, pageSize))
  }

  // Communities
  async getCommunities(page: number = 1, pageSize: number = 20): Promise<ApiResponse<PagedResult<Community>>> {
    await this.simulateDelay()
    return createApiResponse(createPagedResult(mockCommunities, page, pageSize))
  }

  async getDiscussions(communityId: string, page: number = 1, pageSize: number = 20): Promise<ApiResponse<PagedResult<Discussion>>> {
    await this.simulateDelay()
    const discussions = mockDiscussions.filter(d => d.communityId === communityId)
    return createApiResponse(createPagedResult(discussions, page, pageSize))
  }

  // Calendar
  async getEvents(_startDate?: string, _endDate?: string): Promise<ApiResponse<CalendarEvent[]>> {
    await this.simulateDelay()
    return createApiResponse(mockEvents)
  }

  // Notifications
  async getNotifications(): Promise<ApiResponse<Notification[]>> {
    await this.simulateDelay()
    return createApiResponse(mockNotifications)
  }

  async getUnreadCount(): Promise<ApiResponse<number>> {
    await this.simulateDelay()
    return createApiResponse(mockNotifications.filter(n => !n.isRead).length)
  }

  // Dashboard Stats
  async getDashboardStats(): Promise<ApiResponse<{
    totalUsers: number
    totalArticles: number
    totalDocuments: number
    totalCommunities: number
    pendingTasks: number
    upcomingEvents: number
  }>> {
    await this.simulateDelay()
    return createApiResponse({
      totalUsers: 150,
      totalArticles: 85,
      totalDocuments: 243,
      totalCommunities: 12,
      pendingTasks: 8,
      upcomingEvents: 5
    })
  }
}

export const mockService = new MockService()
