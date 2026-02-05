import { describe, it, expect } from 'vitest'
import * as apiExports from '@/api/index'

describe('API Index Exports', () => {
  describe('Module Exports', () => {
    it('should export apiClient', () => {
      expect(apiExports.apiClient).toBeDefined()
    })

    it('should export authApi', () => {
      expect(apiExports.authApi).toBeDefined()
    })

    it('should export articlesApi', () => {
      expect(apiExports.articlesApi).toBeDefined()
    })

    it('should export documentsApi', () => {
      expect(apiExports.documentsApi).toBeDefined()
    })

    it('should export eventsApi', () => {
      expect(apiExports.eventsApi).toBeDefined()
    })

    it('should export searchApi', () => {
      expect(apiExports.searchApi).toBeDefined()
    })

    it('should export aiApi', () => {
      expect(apiExports.aiApi).toBeDefined()
    })

    it('should export notificationsApi', () => {
      expect(apiExports.notificationsApi).toBeDefined()
    })
  })

  describe('Export Types', () => {
    it('apiClient should be defined with interceptors', () => {
      expect(apiExports.apiClient).toBeDefined()
      expect(apiExports.apiClient.interceptors).toBeDefined()
    })

    it('authApi should be an object with methods', () => {
      expect(typeof apiExports.authApi).toBe('object')
    })

    it('articlesApi should be an object with methods', () => {
      expect(typeof apiExports.articlesApi).toBe('object')
    })

    it('documentsApi should be an object with methods', () => {
      expect(typeof apiExports.documentsApi).toBe('object')
    })

    it('eventsApi should be an object with methods', () => {
      expect(typeof apiExports.eventsApi).toBe('object')
    })

    it('searchApi should be an object with methods', () => {
      expect(typeof apiExports.searchApi).toBe('object')
    })

    it('aiApi should be an object with methods', () => {
      expect(typeof apiExports.aiApi).toBe('object')
    })

    it('notificationsApi should be an object with methods', () => {
      expect(typeof apiExports.notificationsApi).toBe('object')
    })
  })

  describe('API Method Availability', () => {
    it('authApi should have login method', () => {
      expect(typeof apiExports.authApi.login).toBe('function')
    })

    it('articlesApi should have getArticles method', () => {
      expect(typeof apiExports.articlesApi.getArticles).toBe('function')
    })

    it('documentsApi should have getDocuments method', () => {
      expect(typeof apiExports.documentsApi.getDocuments).toBe('function')
    })

    it('eventsApi should have getEvents method', () => {
      expect(typeof apiExports.eventsApi.getEvents).toBe('function')
    })

    it('searchApi should have search method', () => {
      expect(typeof apiExports.searchApi.search).toBe('function')
    })

    it('aiApi should have summarizeContent method', () => {
      expect(typeof apiExports.aiApi.summarizeContent).toBe('function')
    })

    it('notificationsApi should have getNotifications method', () => {
      expect(typeof apiExports.notificationsApi.getNotifications).toBe('function')
    })
  })

  describe('Export Count', () => {
    it('should export exactly 8 named exports', () => {
      const exportKeys = Object.keys(apiExports)
      expect(exportKeys.length).toBe(8)
    })

    it('should have all expected export names', () => {
      const expectedExports = [
        'apiClient',
        'authApi',
        'articlesApi',
        'documentsApi',
        'eventsApi',
        'searchApi',
        'aiApi',
        'notificationsApi',
      ]

      expectedExports.forEach((exportName) => {
        expect(apiExports).toHaveProperty(exportName)
      })
    })
  })
})
