import { http, HttpResponse, delay } from 'msw'
import type { AuthResponse, User, SSOConfig } from '@/types'

const API_BASE = import.meta.env.VITE_API_BASE_URL || '/api'

// Mock user data
const mockUser: User = {
  id: '1',
  email: 'ahmed.imam@intalio.com',
  firstName: 'Ahmed',
  lastName: 'Imam',
  displayName: 'Ahmed Imam',
  role: 'admin',
  department: 'Product',
  jobTitle: 'Product Director',
  phone: '+1 234 567 890',
  location: 'Dubai, UAE',
  bio: 'Leading product innovation at Intalio',
  createdAt: '2023-01-15T08:00:00Z',
  lastLoginAt: new Date().toISOString(),
}

// Mock tokens
const mockToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxIiwibmFtZSI6IkFobWVkIEltYW0iLCJpYXQiOjE2MTYxNjE2MTZ9.mock'
const mockRefreshToken = 'refresh_token_mock_12345'

export const authHandlers = [
  // Login
  http.post(`${API_BASE}/identity/auth/login`, async ({ request }) => {
    await delay(800)

    const body = await request.json() as { email: string; password: string }
    const { email } = body

    // Simulate login (accept any credentials)
    const response: AuthResponse = {
      token: mockToken,
      refreshToken: mockRefreshToken,
      user: { ...mockUser, email },
      expiresIn: 3600,
    }

    return HttpResponse.json(response)
  }),

  // SSO Config
  http.get(`${API_BASE}/identity/auth/sso-config`, async () => {
    await delay(300)

    const config: SSOConfig = {
      authUrl: 'https://login.microsoftonline.com/common/oauth2/v2.0/authorize',
      clientId: 'mock-client-id',
      redirectUri: window.location.origin + '/sso-callback',
      scopes: ['openid', 'profile', 'email'],
    }

    return HttpResponse.json(config)
  }),

  // SSO Callback
  http.post(`${API_BASE}/identity/auth/sso-callback`, async () => {
    await delay(500)

    const response: AuthResponse = {
      token: mockToken,
      refreshToken: mockRefreshToken,
      user: mockUser,
      expiresIn: 3600,
    }

    return HttpResponse.json(response)
  }),

  // Refresh Token
  http.post(`${API_BASE}/identity/auth/refresh`, async () => {
    await delay(200)

    return HttpResponse.json({
      token: mockToken + '_refreshed',
      refreshToken: mockRefreshToken + '_new',
    })
  }),

  // Logout
  http.post(`${API_BASE}/identity/auth/logout`, async () => {
    await delay(200)
    return HttpResponse.json({ success: true })
  }),

  // Get Current User
  http.get(`${API_BASE}/identity/users/me`, async ({ request }) => {
    await delay(300)

    const authHeader = request.headers.get('Authorization')
    if (!authHeader || !authHeader.startsWith('Bearer ')) {
      return HttpResponse.json(
        { message: 'Unauthorized' },
        { status: 401 }
      )
    }

    return HttpResponse.json(mockUser)
  }),

  // Update Profile
  http.put(`${API_BASE}/identity/users/me`, async ({ request }) => {
    await delay(500)

    const updates = await request.json() as Partial<User>
    const updatedUser = { ...mockUser, ...updates }

    return HttpResponse.json(updatedUser)
  }),
]
