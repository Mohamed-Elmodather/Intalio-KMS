import { apiClient } from './client'

// Types
export interface AuditLogEntry {
  id: string
  timestamp: string
  userId?: string
  userName?: string
  action: string
  category: string
  entityType?: string
  entityId?: string
  oldValues?: string
  newValues?: string
  additionalData?: string
  ipAddress?: string
  userAgent?: string
  severity: string
}

export interface AuditLogQueryParams {
  from?: string
  to?: string
  userId?: string
  category?: string
  action?: string
  entityType?: string
  entityId?: string
  severity?: string
  skip?: number
  take?: number
}

export interface PaginatedResult<T> {
  items: T[]
  totalCount: number
  skip: number
  take: number
}

export interface LegalHold {
  id: string
  name: string
  matterNumber?: string
  description?: string
  reason: string
  status: string
  createdAt: string
  createdBy: string
  createdByName: string
  releasedAt?: string
  releasedBy?: string
  releasedByName?: string
  releaseReason?: string
  documentCount: number
  custodianCount: number
  documents?: LegalHoldDocument[]
  custodians?: LegalHoldCustodian[]
}

export interface LegalHoldDocument {
  id: string
  documentId: string
  documentName: string
  addedAt: string
  addedBy: string
  addedByName: string
}

export interface LegalHoldCustodian {
  id: string
  userId: string
  userName: string
  userEmail?: string
  notes?: string
  addedAt: string
  addedBy: string
  addedByName: string
  notifiedAt?: string
  acknowledgedAt?: string
}

export interface CreateLegalHoldRequest {
  name: string
  matterNumber?: string
  description?: string
  reason: string
}

export interface QuarantinedDocument {
  id: string
  documentId: string
  documentName: string
  reason: string
  status: string
  quarantinedAt: string
  quarantinedBy: string
  quarantinedByName: string
  reviewedAt?: string
  reviewedBy?: string
  reviewedByName?: string
  reviewNotes?: string
}

export interface UserLifecycleAction {
  userId: string
  action: string
  reason?: string
}

export interface ImpersonationSession {
  id: string
  adminUserId: string
  adminUserName: string
  impersonatedUserId: string
  impersonatedUserName: string
  reason: string
  startedAt: string
  endedAt?: string
  isActive: boolean
  ipAddress?: string
  actions?: string[]
}

// Admin API
export const adminApi = {
  // Audit Logs
  async getAuditLogs(params: AuditLogQueryParams): Promise<PaginatedResult<AuditLogEntry>> {
    const queryString = new URLSearchParams()
    Object.entries(params).forEach(([key, value]) => {
      if (value !== undefined && value !== null && value !== '') {
        queryString.append(key, String(value))
      }
    })
    const response = await apiClient.get(`/api/admin/audit?${queryString.toString()}`)
    return response.data
  },

  async exportAuditLogs(params: Partial<AuditLogQueryParams>): Promise<Blob> {
    const queryString = new URLSearchParams()
    Object.entries(params).forEach(([key, value]) => {
      if (value !== undefined && value !== null && value !== '') {
        queryString.append(key, String(value))
      }
    })
    const response = await apiClient.get(`/api/admin/audit/export?${queryString.toString()}`, {
      responseType: 'blob'
    })
    return response.data
  },

  // Legal Holds
  async getLegalHolds(includeReleased = false): Promise<LegalHold[]> {
    const response = await apiClient.get(`/api/admin/legal-holds?includeReleased=${includeReleased}`)
    return response.data
  },

  async getLegalHold(id: string): Promise<LegalHold> {
    const response = await apiClient.get(`/api/admin/legal-holds/${id}`)
    return response.data
  },

  async createLegalHold(request: CreateLegalHoldRequest): Promise<LegalHold> {
    const response = await apiClient.post('/api/admin/legal-holds', request)
    return response.data
  },

  async releaseLegalHold(id: string, reason: string): Promise<void> {
    await apiClient.post(`/api/admin/legal-holds/${id}/release`, { reason })
  },

  async addDocumentToHold(holdId: string, documentId: string): Promise<void> {
    await apiClient.post(`/api/admin/legal-holds/${holdId}/documents`, { documentId })
  },

  async addCustodianToHold(holdId: string, userId: string, notes?: string): Promise<void> {
    await apiClient.post(`/api/admin/legal-holds/${holdId}/custodians`, { userId, notes })
  },

  // Quarantine
  async getQuarantinedDocuments(status?: string): Promise<QuarantinedDocument[]> {
    const url = status
      ? `/api/admin/quarantine?status=${status}`
      : '/api/admin/quarantine'
    const response = await apiClient.get(url)
    return response.data
  },

  async quarantineDocument(documentId: string, reason: string): Promise<void> {
    await apiClient.post(`/api/admin/quarantine/${documentId}`, { reason })
  },

  async reviewQuarantinedDocument(
    id: string,
    approve: boolean,
    notes?: string
  ): Promise<void> {
    await apiClient.post(`/api/admin/quarantine/${id}/review`, {
      approve,
      notes
    })
  },

  // User Lifecycle
  async inviteUser(email: string, roleId?: string): Promise<void> {
    await apiClient.post('/api/admin/users/invite', { email, roleId })
  },

  async suspendUser(userId: string, reason: string): Promise<void> {
    await apiClient.post(`/api/admin/users/${userId}/suspend`, { reason })
  },

  async reactivateUser(userId: string): Promise<void> {
    await apiClient.post(`/api/admin/users/${userId}/reactivate`)
  },

  async deleteUser(userId: string, transferToUserId?: string): Promise<void> {
    await apiClient.delete(`/api/admin/users/${userId}`, {
      params: { transferToUserId }
    })
  },

  // Impersonation
  async startImpersonation(targetUserId: string, reason: string): Promise<ImpersonationSession> {
    const response = await apiClient.post('/api/admin/impersonation/start', {
      targetUserId,
      reason
    })
    return response.data
  },

  async endImpersonation(sessionId: string): Promise<void> {
    await apiClient.post(`/api/admin/impersonation/${sessionId}/end`)
  },

  async getActiveImpersonation(): Promise<ImpersonationSession | null> {
    try {
      const response = await apiClient.get('/api/admin/impersonation/active')
      return response.data
    } catch (error: any) {
      if (error.response?.status === 404) {
        return null
      }
      throw error
    }
  },

  async getImpersonationHistory(params?: {
    adminUserId?: string
    targetUserId?: string
    from?: string
    to?: string
  }): Promise<ImpersonationSession[]> {
    const queryString = new URLSearchParams()
    if (params) {
      Object.entries(params).forEach(([key, value]) => {
        if (value) queryString.append(key, value)
      })
    }
    const response = await apiClient.get(`/api/admin/impersonation/history?${queryString.toString()}`)
    return response.data
  }
}
