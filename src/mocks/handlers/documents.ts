import { http, HttpResponse, delay } from 'msw'
import type { Document, DocumentLibrary, PaginatedResponse } from '@/types'

const API_BASE = import.meta.env.VITE_API_BASE_URL || '/api'

// Mock libraries
const mockLibraries: DocumentLibrary[] = [
  { id: '1', name: 'Company Policies', description: 'Official company policies and guidelines', icon: 'fas fa-balance-scale', color: '#3b82f6', documentCount: 24, totalSize: 156000000, createdAt: '2024-01-01T00:00:00Z' },
  { id: '2', name: 'Training Materials', description: 'Learning and development resources', icon: 'fas fa-graduation-cap', color: '#10b981', documentCount: 45, totalSize: 890000000, createdAt: '2024-01-01T00:00:00Z' },
  { id: '3', name: 'Project Documentation', description: 'Project plans and documentation', icon: 'fas fa-project-diagram', color: '#f59e0b', documentCount: 78, totalSize: 234000000, createdAt: '2024-01-01T00:00:00Z' },
  { id: '4', name: 'Brand Assets', description: 'Logos, templates, and brand guidelines', icon: 'fas fa-palette', color: '#8b5cf6', documentCount: 32, totalSize: 567000000, createdAt: '2024-01-01T00:00:00Z' },
]

// Mock documents
const mockDocuments: Document[] = [
  {
    id: '1',
    name: 'Employee Handbook 2024.pdf',
    type: 'pdf',
    size: 2456000,
    mimeType: 'application/pdf',
    extension: 'pdf',
    path: '/policies/Employee Handbook 2024.pdf',
    url: '#',
    libraryId: '1',
    uploadedBy: {
      id: '1',
      email: 'hr@intalio.com',
      firstName: 'HR',
      lastName: 'Team',
      displayName: 'HR Team',
      role: 'admin',
      createdAt: '2023-01-01T00:00:00Z',
    },
    createdAt: '2024-12-01T10:00:00Z',
    updatedAt: '2024-12-15T14:30:00Z',
    version: 3,
    isShared: true,
  },
  {
    id: '2',
    name: 'Q4 Financial Report.xlsx',
    type: 'excel',
    size: 1234000,
    mimeType: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
    extension: 'xlsx',
    path: '/finance/Q4 Financial Report.xlsx',
    url: '#',
    libraryId: '3',
    uploadedBy: {
      id: '2',
      email: 'finance@intalio.com',
      firstName: 'Finance',
      lastName: 'Team',
      displayName: 'Finance Team',
      role: 'editor',
      createdAt: '2023-01-01T00:00:00Z',
    },
    createdAt: '2024-12-20T09:00:00Z',
    updatedAt: '2024-12-20T09:00:00Z',
    version: 1,
    isShared: false,
  },
  {
    id: '3',
    name: 'Product Roadmap 2025.pptx',
    type: 'powerpoint',
    size: 5678000,
    mimeType: 'application/vnd.openxmlformats-officedocument.presentationml.presentation',
    extension: 'pptx',
    path: '/product/Product Roadmap 2025.pptx',
    url: '#',
    libraryId: '3',
    uploadedBy: {
      id: '1',
      email: 'ahmed.imam@intalio.com',
      firstName: 'Ahmed',
      lastName: 'Imam',
      displayName: 'Ahmed Imam',
      role: 'admin',
      createdAt: '2023-01-15T08:00:00Z',
    },
    createdAt: '2024-12-18T11:00:00Z',
    updatedAt: '2024-12-22T16:00:00Z',
    version: 5,
    isShared: true,
  },
  {
    id: '4',
    name: 'Brand Guidelines.pdf',
    type: 'pdf',
    size: 8900000,
    mimeType: 'application/pdf',
    extension: 'pdf',
    path: '/brand/Brand Guidelines.pdf',
    url: '#',
    libraryId: '4',
    uploadedBy: {
      id: '3',
      email: 'marketing@intalio.com',
      firstName: 'Marketing',
      lastName: 'Team',
      displayName: 'Marketing Team',
      role: 'editor',
      createdAt: '2023-01-01T00:00:00Z',
    },
    createdAt: '2024-11-01T10:00:00Z',
    updatedAt: '2024-12-10T14:00:00Z',
    version: 2,
    isShared: true,
  },
]

export const documentsHandlers = [
  // Get documents
  http.get(`${API_BASE}/documents/documents`, async ({ request }) => {
    await delay(500)

    const url = new URL(request.url)
    const page = parseInt(url.searchParams.get('page') || '1')
    const pageSize = parseInt(url.searchParams.get('pageSize') || '10')
    const libraryId = url.searchParams.get('libraryId')

    let filteredDocs = [...mockDocuments]
    if (libraryId) {
      filteredDocs = filteredDocs.filter(d => d.libraryId === libraryId)
    }

    const response: PaginatedResponse<Document> = {
      items: filteredDocs,
      total: filteredDocs.length,
      page,
      pageSize,
      totalPages: Math.ceil(filteredDocs.length / pageSize),
      hasNext: false,
      hasPrevious: page > 1,
    }

    return HttpResponse.json(response)
  }),

  // Get libraries
  http.get(`${API_BASE}/documents/libraries`, async () => {
    await delay(300)
    return HttpResponse.json(mockLibraries)
  }),

  // Get recent documents
  http.get(`${API_BASE}/documents/recent`, async ({ request }) => {
    await delay(300)
    const url = new URL(request.url)
    const limit = parseInt(url.searchParams.get('limit') || '5')
    return HttpResponse.json(mockDocuments.slice(0, limit))
  }),
]
