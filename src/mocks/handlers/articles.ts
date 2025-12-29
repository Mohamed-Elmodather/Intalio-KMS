import { http, HttpResponse, delay } from 'msw'
import type { Article, Category, PaginatedResponse } from '@/types'

const API_BASE = import.meta.env.VITE_API_BASE_URL || '/api'

// Mock categories
const mockCategories: Category[] = [
  { id: '1', name: 'Company News', slug: 'company-news', icon: 'fas fa-building', color: '#3b82f6', articleCount: 12 },
  { id: '2', name: 'HR & Policies', slug: 'hr-policies', icon: 'fas fa-user-shield', color: '#8b5cf6', articleCount: 8 },
  { id: '3', name: 'Technology', slug: 'technology', icon: 'fas fa-microchip', color: '#10b981', articleCount: 15 },
  { id: '4', name: 'Training', slug: 'training', icon: 'fas fa-graduation-cap', color: '#f59e0b', articleCount: 6 },
]

// Mock articles
const mockArticles: Article[] = [
  {
    id: '1',
    title: 'Q4 2024 Strategic Initiatives',
    slug: 'q4-2024-strategic-initiatives',
    excerpt: 'Overview of our key strategic focus areas for the final quarter of 2024.',
    content: '<p>Full article content here...</p>',
    coverImage: 'https://images.unsplash.com/photo-1552664730-d307ca884978?w=600',
    author: {
      id: '1',
      email: 'ahmed.imam@intalio.com',
      firstName: 'Ahmed',
      lastName: 'Imam',
      displayName: 'Ahmed Imam',
      role: 'admin',
      createdAt: '2023-01-15T08:00:00Z',
    },
    category: mockCategories[0]!,
    tags: [{ id: '1', name: 'Strategy', slug: 'strategy', articleCount: 5 }],
    status: 'published',
    publishedAt: '2024-12-20T10:00:00Z',
    createdAt: '2024-12-18T08:00:00Z',
    updatedAt: '2024-12-20T10:00:00Z',
    viewCount: 342,
    likeCount: 56,
    commentCount: 12,
    isLiked: false,
    isBookmarked: false,
  },
  {
    id: '2',
    title: 'New Employee Onboarding Guide',
    slug: 'new-employee-onboarding-guide',
    excerpt: 'Everything you need to know to get started at Intalio.',
    content: '<p>Welcome to Intalio!</p>',
    coverImage: 'https://images.unsplash.com/photo-1521737711867-e3b97375f902?w=600',
    author: {
      id: '2',
      email: 'sarah@intalio.com',
      firstName: 'Sarah',
      lastName: 'Johnson',
      displayName: 'Sarah Johnson',
      role: 'editor',
      createdAt: '2023-02-10T08:00:00Z',
    },
    category: mockCategories[1]!,
    tags: [{ id: '2', name: 'HR', slug: 'hr', articleCount: 8 }],
    status: 'published',
    publishedAt: '2024-12-15T09:00:00Z',
    createdAt: '2024-12-14T08:00:00Z',
    updatedAt: '2024-12-15T09:00:00Z',
    viewCount: 891,
    likeCount: 124,
    commentCount: 34,
    isLiked: true,
    isBookmarked: true,
  },
  {
    id: '3',
    title: 'AI Integration Best Practices',
    slug: 'ai-integration-best-practices',
    excerpt: 'Learn how to effectively integrate AI tools into your daily workflow.',
    content: '<p>AI is transforming how we work...</p>',
    coverImage: 'https://images.unsplash.com/photo-1677442136019-21780ecad995?w=600',
    author: {
      id: '1',
      email: 'ahmed.imam@intalio.com',
      firstName: 'Ahmed',
      lastName: 'Imam',
      displayName: 'Ahmed Imam',
      role: 'admin',
      createdAt: '2023-01-15T08:00:00Z',
    },
    category: mockCategories[2]!,
    tags: [
      { id: '3', name: 'AI', slug: 'ai', articleCount: 10 },
      { id: '4', name: 'Productivity', slug: 'productivity', articleCount: 7 },
    ],
    status: 'published',
    publishedAt: '2024-12-22T14:00:00Z',
    createdAt: '2024-12-21T08:00:00Z',
    updatedAt: '2024-12-22T14:00:00Z',
    viewCount: 567,
    likeCount: 89,
    commentCount: 23,
    isLiked: false,
    isBookmarked: false,
  },
]

export const articlesHandlers = [
  // Get articles
  http.get(`${API_BASE}/content/articles`, async ({ request }) => {
    await delay(500)

    const url = new URL(request.url)
    const page = parseInt(url.searchParams.get('page') || '1')
    const pageSize = parseInt(url.searchParams.get('pageSize') || '10')

    const response: PaginatedResponse<Article> = {
      items: mockArticles,
      total: mockArticles.length,
      page,
      pageSize,
      totalPages: Math.ceil(mockArticles.length / pageSize),
      hasNext: false,
      hasPrevious: page > 1,
    }

    return HttpResponse.json(response)
  }),

  // Get single article
  http.get(`${API_BASE}/content/articles/:id`, async ({ params }) => {
    await delay(300)

    const article = mockArticles.find(a => a.id === params.id || a.slug === params.id)
    if (!article) {
      return HttpResponse.json({ message: 'Article not found' }, { status: 404 })
    }

    return HttpResponse.json(article)
  }),

  // Get categories
  http.get(`${API_BASE}/content/categories`, async () => {
    await delay(200)
    return HttpResponse.json(mockCategories)
  }),

  // Get recent articles
  http.get(`${API_BASE}/content/articles/recent`, async ({ request }) => {
    await delay(300)
    const url = new URL(request.url)
    const limit = parseInt(url.searchParams.get('limit') || '5')
    return HttpResponse.json(mockArticles.slice(0, limit))
  }),

  // Get featured articles
  http.get(`${API_BASE}/content/articles/featured`, async ({ request }) => {
    await delay(300)
    const url = new URL(request.url)
    const limit = parseInt(url.searchParams.get('limit') || '3')
    return HttpResponse.json(mockArticles.slice(0, limit))
  }),
]
