import { describe, it, expect, vi, beforeEach } from 'vitest'
import { documentsApi } from '@/api/documents'
import apiClient from '@/api/client'

// Mock the API client
vi.mock('@/api/client', () => ({
  default: {
    post: vi.fn(),
    get: vi.fn(),
    put: vi.fn(),
    delete: vi.fn(),
  },
}))

describe('Documents API', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('getDocuments', () => {
    it('should fetch documents without filters', async () => {
      const mockResponse = {
        items: [
          { id: '1', name: 'Document 1.pdf' },
          { id: '2', name: 'Document 2.docx' },
        ],
        totalCount: 2,
        page: 1,
        pageSize: 10,
      }
      vi.mocked(apiClient.get).mockResolvedValue({ data: mockResponse })

      const result = await documentsApi.getDocuments()

      expect(apiClient.get).toHaveBeenCalledWith('/documents/documents', {
        params: undefined,
      })
      expect(result).toEqual(mockResponse)
    })

    it('should fetch documents with library filter', async () => {
      vi.mocked(apiClient.get).mockResolvedValue({ data: { items: [], totalCount: 0 } })

      await documentsApi.getDocuments({ libraryId: 'lib-123' })

      expect(apiClient.get).toHaveBeenCalledWith('/documents/documents', {
        params: { libraryId: 'lib-123' },
      })
    })

    it('should fetch documents with folder filter', async () => {
      vi.mocked(apiClient.get).mockResolvedValue({ data: { items: [], totalCount: 0 } })

      await documentsApi.getDocuments({ folderId: 'folder-456' })

      expect(apiClient.get).toHaveBeenCalledWith('/documents/documents', {
        params: { folderId: 'folder-456' },
      })
    })

    it('should fetch documents with type filter', async () => {
      vi.mocked(apiClient.get).mockResolvedValue({ data: { items: [], totalCount: 0 } })

      await documentsApi.getDocuments({ type: 'pdf' })

      expect(apiClient.get).toHaveBeenCalledWith('/documents/documents', {
        params: { type: 'pdf' },
      })
    })

    it('should fetch documents with pagination', async () => {
      vi.mocked(apiClient.get).mockResolvedValue({ data: { items: [], totalCount: 0 } })

      await documentsApi.getDocuments({ page: 2, pageSize: 20 })

      expect(apiClient.get).toHaveBeenCalledWith('/documents/documents', {
        params: { page: 2, pageSize: 20 },
      })
    })
  })

  describe('getDocument', () => {
    it('should fetch single document by ID', async () => {
      const mockDocument = { id: 'doc-123', name: 'Test.pdf', size: 1024 }
      vi.mocked(apiClient.get).mockResolvedValue({ data: mockDocument })

      const result = await documentsApi.getDocument('doc-123')

      expect(apiClient.get).toHaveBeenCalledWith('/documents/documents/doc-123')
      expect(result).toEqual(mockDocument)
    })
  })

  describe('uploadDocument', () => {
    it('should upload document to library', async () => {
      const mockFile = new File(['content'], 'test.pdf', { type: 'application/pdf' })
      const mockResponse = { id: 'new-doc', name: 'test.pdf' }
      vi.mocked(apiClient.post).mockResolvedValue({ data: mockResponse })

      const result = await documentsApi.uploadDocument(mockFile, 'lib-123')

      expect(apiClient.post).toHaveBeenCalledWith(
        '/documents/upload',
        expect.any(FormData),
        expect.objectContaining({
          headers: { 'Content-Type': 'multipart/form-data' },
        })
      )
      expect(result).toEqual(mockResponse)
    })

    it('should upload document with folder', async () => {
      const mockFile = new File(['content'], 'test.pdf', { type: 'application/pdf' })
      vi.mocked(apiClient.post).mockResolvedValue({ data: { id: 'new-doc' } })

      await documentsApi.uploadDocument(mockFile, 'lib-123', 'folder-456')

      const callArgs = vi.mocked(apiClient.post).mock.calls[0]
      const formData = callArgs?.[1] as FormData
      expect(formData.get('folderId')).toBe('folder-456')
    })

    it('should call progress callback during upload', async () => {
      const mockFile = new File(['content'], 'test.pdf', { type: 'application/pdf' })
      vi.mocked(apiClient.post).mockResolvedValue({ data: { id: 'new-doc' } })
      const progressCallback = vi.fn()

      await documentsApi.uploadDocument(mockFile, 'lib-123', undefined, progressCallback)

      const callArgs = vi.mocked(apiClient.post).mock.calls[0]
      const config = callArgs?.[2] as { onUploadProgress: (e: { loaded: number; total: number }) => void }

      // Simulate progress event
      config.onUploadProgress({ loaded: 50, total: 100 })
      expect(progressCallback).toHaveBeenCalledWith(50)
    })
  })

  describe('deleteDocument', () => {
    it('should delete document', async () => {
      vi.mocked(apiClient.delete).mockResolvedValue({ data: {} })

      await documentsApi.deleteDocument('doc-123')

      expect(apiClient.delete).toHaveBeenCalledWith('/documents/documents/doc-123')
    })
  })

  describe('downloadDocument', () => {
    it('should download document as blob', async () => {
      const mockBlob = new Blob(['file content'], { type: 'application/pdf' })
      vi.mocked(apiClient.get).mockResolvedValue({ data: mockBlob })

      const result = await documentsApi.downloadDocument('doc-123')

      expect(apiClient.get).toHaveBeenCalledWith('/documents/documents/doc-123/download', {
        responseType: 'blob',
      })
      expect(result).toEqual(mockBlob)
    })
  })

  describe('shareDocument', () => {
    it('should share document without expiry', async () => {
      const mockResponse = { shareLink: 'https://example.com/share/abc123' }
      vi.mocked(apiClient.post).mockResolvedValue({ data: mockResponse })

      const result = await documentsApi.shareDocument('doc-123')

      expect(apiClient.post).toHaveBeenCalledWith('/documents/documents/doc-123/share', {
        expiresIn: undefined,
      })
      expect(result).toEqual(mockResponse)
    })

    it('should share document with expiry', async () => {
      vi.mocked(apiClient.post).mockResolvedValue({ data: { shareLink: 'https://example.com/share/xyz' } })

      await documentsApi.shareDocument('doc-123', 86400)

      expect(apiClient.post).toHaveBeenCalledWith('/documents/documents/doc-123/share', {
        expiresIn: 86400,
      })
    })
  })

  describe('getLibraries', () => {
    it('should fetch all libraries', async () => {
      const mockLibraries = [
        { id: 'lib-1', name: 'General Documents' },
        { id: 'lib-2', name: 'Media Files' },
      ]
      vi.mocked(apiClient.get).mockResolvedValue({ data: mockLibraries })

      const result = await documentsApi.getLibraries()

      expect(apiClient.get).toHaveBeenCalledWith('/documents/libraries')
      expect(result).toEqual(mockLibraries)
    })
  })

  describe('getFolders', () => {
    it('should fetch folders in library', async () => {
      const mockFolders = [
        { id: 'folder-1', name: 'Reports' },
        { id: 'folder-2', name: 'Images' },
      ]
      vi.mocked(apiClient.get).mockResolvedValue({ data: mockFolders })

      const result = await documentsApi.getFolders('lib-123')

      expect(apiClient.get).toHaveBeenCalledWith('/documents/libraries/lib-123/folders', {
        params: { parentId: undefined },
      })
      expect(result).toEqual(mockFolders)
    })

    it('should fetch subfolders with parent ID', async () => {
      vi.mocked(apiClient.get).mockResolvedValue({ data: [] })

      await documentsApi.getFolders('lib-123', 'parent-folder')

      expect(apiClient.get).toHaveBeenCalledWith('/documents/libraries/lib-123/folders', {
        params: { parentId: 'parent-folder' },
      })
    })
  })

  describe('createFolder', () => {
    it('should create folder in library', async () => {
      const mockFolder = { id: 'new-folder', name: 'New Folder' }
      vi.mocked(apiClient.post).mockResolvedValue({ data: mockFolder })

      const result = await documentsApi.createFolder('lib-123', 'New Folder')

      expect(apiClient.post).toHaveBeenCalledWith('/documents/libraries/lib-123/folders', {
        name: 'New Folder',
        parentId: undefined,
      })
      expect(result).toEqual(mockFolder)
    })

    it('should create subfolder with parent ID', async () => {
      vi.mocked(apiClient.post).mockResolvedValue({ data: { id: 'subfolder' } })

      await documentsApi.createFolder('lib-123', 'Subfolder', 'parent-folder')

      expect(apiClient.post).toHaveBeenCalledWith('/documents/libraries/lib-123/folders', {
        name: 'Subfolder',
        parentId: 'parent-folder',
      })
    })
  })

  describe('getRecentDocuments', () => {
    it('should fetch recent documents without limit', async () => {
      const mockDocuments = [
        { id: '1', name: 'Recent1.pdf' },
        { id: '2', name: 'Recent2.docx' },
      ]
      vi.mocked(apiClient.get).mockResolvedValue({ data: mockDocuments })

      const result = await documentsApi.getRecentDocuments()

      expect(apiClient.get).toHaveBeenCalledWith('/documents/recent', {
        params: { limit: undefined },
      })
      expect(result).toEqual(mockDocuments)
    })

    it('should fetch recent documents with limit', async () => {
      vi.mocked(apiClient.get).mockResolvedValue({ data: [] })

      await documentsApi.getRecentDocuments(5)

      expect(apiClient.get).toHaveBeenCalledWith('/documents/recent', {
        params: { limit: 5 },
      })
    })
  })
})
