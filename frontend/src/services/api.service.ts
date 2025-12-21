import axios, { type AxiosInstance, type AxiosRequestConfig, type InternalAxiosRequestConfig } from 'axios'
import { useAuthStore } from '@/stores/auth.store'
import type { ApiResponse } from '@/types'
import { mockService } from './mock.service'

class ApiService {
  private client: AxiosInstance
  private useMockData: boolean = false
  private backendAvailable: boolean | null = null

  constructor() {
    this.client = axios.create({
      baseURL: import.meta.env.VITE_API_URL || '/api',
      timeout: 30000,
      headers: {
        'Content-Type': 'application/json'
      }
    })

    this.useMockData = import.meta.env.VITE_USE_MOCK_DATA === 'true'
    this.setupInterceptors()
    this.checkBackendAvailability()
  }

  /**
   * Check if backend is available
   */
  private async checkBackendAvailability(): Promise<void> {
    if (this.useMockData) {
      this.backendAvailable = false
      console.log('[API] Using mock data (configured)')
      return
    }

    try {
      await this.client.get('/health/live', { timeout: 5000 })
      this.backendAvailable = true
      console.log('[API] Backend available - using live API')
    } catch {
      this.backendAvailable = false
      console.log('[API] Backend unavailable - falling back to mock data')
    }
  }

  /**
   * Get mock service for fallback
   */
  getMockService() {
    return mockService
  }

  /**
   * Check if we should use mock data
   */
  shouldUseMock(): boolean {
    return this.useMockData || this.backendAvailable === false
  }

  private setupInterceptors(): void {
    // Request interceptor - add auth token
    this.client.interceptors.request.use(
      (config: InternalAxiosRequestConfig) => {
        const authStore = useAuthStore()
        if (authStore.accessToken) {
          config.headers.Authorization = `Bearer ${authStore.accessToken}`
        }

        // Add language header
        const uiStore = (window as unknown as { pinia: { state: { value: { ui: { locale: string } } } } }).pinia?.state?.value?.ui
        if (uiStore?.locale) {
          config.headers['Accept-Language'] = uiStore.locale
        }

        return config
      },
      (error) => Promise.reject(error)
    )

    // Response interceptor - handle errors
    this.client.interceptors.response.use(
      (response) => response,
      async (error) => {
        const originalRequest = error.config

        // Handle 401 - try to refresh token
        if (error.response?.status === 401 && !originalRequest._retry) {
          originalRequest._retry = true
          const authStore = useAuthStore()
          const refreshed = await authStore.refreshToken()

          if (refreshed) {
            originalRequest.headers.Authorization = `Bearer ${authStore.accessToken}`
            return this.client(originalRequest)
          } else {
            // Redirect to login
            window.location.href = '/login'
          }
        }

        return Promise.reject(error)
      }
    )
  }

  async get<T>(url: string, config?: AxiosRequestConfig): Promise<ApiResponse<T>> {
    const response = await this.client.get<ApiResponse<T>>(url, config)
    return response.data
  }

  async post<T>(url: string, data?: unknown, config?: AxiosRequestConfig): Promise<ApiResponse<T>> {
    const response = await this.client.post<ApiResponse<T>>(url, data, config)
    return response.data
  }

  async put<T>(url: string, data?: unknown, config?: AxiosRequestConfig): Promise<ApiResponse<T>> {
    const response = await this.client.put<ApiResponse<T>>(url, data, config)
    return response.data
  }

  async patch<T>(url: string, data?: unknown, config?: AxiosRequestConfig): Promise<ApiResponse<T>> {
    const response = await this.client.patch<ApiResponse<T>>(url, data, config)
    return response.data
  }

  async delete<T>(url: string, config?: AxiosRequestConfig): Promise<ApiResponse<T>> {
    const response = await this.client.delete<ApiResponse<T>>(url, config)
    return response.data
  }

  async upload<T>(url: string, formData: FormData, onProgress?: (progress: number) => void): Promise<ApiResponse<T>> {
    const response = await this.client.post<ApiResponse<T>>(url, formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      },
      onUploadProgress: (progressEvent) => {
        if (onProgress && progressEvent.total) {
          const progress = Math.round((progressEvent.loaded * 100) / progressEvent.total)
          onProgress(progress)
        }
      }
    })
    return response.data
  }

  async download(url: string, filename: string): Promise<void> {
    const response = await this.client.get(url, {
      responseType: 'blob'
    })

    const blob = new Blob([response.data])
    const link = document.createElement('a')
    link.href = window.URL.createObjectURL(blob)
    link.download = filename
    link.click()
    window.URL.revokeObjectURL(link.href)
  }
}

export const apiService = new ApiService()
