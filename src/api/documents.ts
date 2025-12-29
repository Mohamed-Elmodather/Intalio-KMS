import apiClient from './client'
import type { Document, DocumentLibrary, Folder, PaginatedResponse, PaginationParams } from '@/types'

const DOCS_BASE = '/documents'

export interface DocumentFilters extends PaginationParams {
  libraryId?: string
  folderId?: string
  type?: string
}

export const documentsApi = {
  // Get all documents with pagination
  async getDocuments(filters?: DocumentFilters): Promise<PaginatedResponse<Document>> {
    const response = await apiClient.get<PaginatedResponse<Document>>(`${DOCS_BASE}/documents`, {
      params: filters,
    })
    return response.data
  },

  // Get single document
  async getDocument(id: string): Promise<Document> {
    const response = await apiClient.get<Document>(`${DOCS_BASE}/documents/${id}`)
    return response.data
  },

  // Upload document
  async uploadDocument(
    file: File,
    libraryId: string,
    folderId?: string,
    onProgress?: (progress: number) => void
  ): Promise<Document> {
    const formData = new FormData()
    formData.append('file', file)
    formData.append('libraryId', libraryId)
    if (folderId) {
      formData.append('folderId', folderId)
    }

    const response = await apiClient.post<Document>(`${DOCS_BASE}/upload`, formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
      onUploadProgress: (progressEvent) => {
        if (onProgress && progressEvent.total) {
          const progress = Math.round((progressEvent.loaded * 100) / progressEvent.total)
          onProgress(progress)
        }
      },
    })
    return response.data
  },

  // Delete document
  async deleteDocument(id: string): Promise<void> {
    await apiClient.delete(`${DOCS_BASE}/documents/${id}`)
  },

  // Download document
  async downloadDocument(id: string): Promise<Blob> {
    const response = await apiClient.get(`${DOCS_BASE}/documents/${id}/download`, {
      responseType: 'blob',
    })
    return response.data
  },

  // Share document
  async shareDocument(id: string, expiresIn?: number): Promise<{ shareLink: string }> {
    const response = await apiClient.post<{ shareLink: string }>(
      `${DOCS_BASE}/documents/${id}/share`,
      { expiresIn }
    )
    return response.data
  },

  // Get libraries
  async getLibraries(): Promise<DocumentLibrary[]> {
    const response = await apiClient.get<DocumentLibrary[]>(`${DOCS_BASE}/libraries`)
    return response.data
  },

  // Get folders in a library
  async getFolders(libraryId: string, parentId?: string): Promise<Folder[]> {
    const response = await apiClient.get<Folder[]>(`${DOCS_BASE}/libraries/${libraryId}/folders`, {
      params: { parentId },
    })
    return response.data
  },

  // Create folder
  async createFolder(libraryId: string, name: string, parentId?: string): Promise<Folder> {
    const response = await apiClient.post<Folder>(`${DOCS_BASE}/libraries/${libraryId}/folders`, {
      name,
      parentId,
    })
    return response.data
  },

  // Get recent documents
  async getRecentDocuments(limit?: number): Promise<Document[]> {
    const response = await apiClient.get<Document[]>(`${DOCS_BASE}/recent`, {
      params: { limit },
    })
    return response.data
  },
}

export default documentsApi
