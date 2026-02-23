# AFC 2027 Knowledge Management System - Backend API Documentation

## Overview

This document provides comprehensive API documentation for the AFC 2027 Knowledge Management System backend. Use this guide to integrate any frontend application with the backend services.

---

## Table of Contents

1. [Server Configuration](#server-configuration)
2. [Authentication](#authentication)
3. [API Response Formats](#api-response-formats)
4. [Core Endpoints](#core-endpoints)
   - [Identity & Users](#identity--users)
   - [Dashboard](#dashboard)
   - [Content & Articles](#content--articles)
   - [Documents](#documents)
   - [Search](#search)
   - [AI Services](#ai-services)
   - [Service Catalog](#service-catalog)
   - [Workflow & Tasks](#workflow--tasks)
   - [Notifications](#notifications)
   - [Calendar & Events](#calendar--events)
   - [Meetings](#meetings)
   - [Polls](#polls)
   - [Learning Management](#learning-management)
   - [KPIs](#kpis)
   - [Quality Assurance](#quality-assurance)
   - [Document Templates](#document-templates)
   - [Barcodes & QR Codes](#barcodes--qr-codes)
   - [Integration & Connectors](#integration--connectors)
5. [WebSocket / Real-time](#websocket--real-time)
6. [Error Handling](#error-handling)
7. [TypeScript Interfaces](#typescript-interfaces)
8. [Frontend Integration Examples](#frontend-integration-examples)

---

## Server Configuration

| Setting | Development Value | Description |
|---------|-------------------|-------------|
| **Backend URL** | `http://localhost:5001` | API server base URL |
| **API Base Path** | `/api` | All endpoints prefixed with this |
| **Health Check** | `/health/live`, `/health/ready` | Server health endpoints |
| **CORS Origins** | `http://localhost:3000` | Allowed frontend origins |
| **Auth Disabled** | `DisableAuth: true` | Dev mode - bypasses auth |

### Environment Variables

```bash
# Backend (appsettings.json)
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://localhost:5001

# Database
ConnectionStrings__DefaultConnection=Data Source=app.db

# Azure AD (SSO)
AzureAd__TenantId=your-tenant-id
AzureAd__ClientId=your-client-id
AzureAd__ClientSecret=your-client-secret

# JWT
Jwt__Secret=your-jwt-secret-key
Jwt__Issuer=AFC27-KMS
Jwt__Audience=AFC27-KMS-Client
Jwt__AccessTokenExpirationMinutes=60
Jwt__RefreshTokenExpirationDays=7
```

---

## Authentication

### Authentication Methods

1. **SSO (Azure AD)** - Primary authentication via OAuth2/OIDC
2. **Email/Password** - Fallback authentication method
3. **JWT Tokens** - Bearer token authentication for API requests

### Authentication Flow

#### 1. SSO Login (Recommended)

```
Step 1: Get SSO Configuration
GET /api/identity/auth/sso-config

Response:
{
  "authority": "https://login.microsoftonline.com/{tenant}",
  "clientId": "your-client-id",
  "redirectUri": "http://localhost:3000/sso-callback",
  "scopes": ["openid", "profile", "email"]
}

Step 2: Redirect user to Azure AD login page

Step 3: Handle callback with authorization code
POST /api/identity/auth/sso-callback
{
  "code": "authorization-code-from-azure",
  "redirectUri": "http://localhost:3000/sso-callback"
}

Response:
{
  "accessToken": "eyJ...",
  "refreshToken": "...",
  "expiresIn": 3600,
  "tokenType": "Bearer",
  "user": {
    "id": "guid",
    "email": "user@example.com",
    "displayName": "John Doe",
    "firstName": "John",
    "lastName": "Doe",
    "roles": ["User", "Editor"]
  }
}
```

#### 2. Email/Password Login

```http
POST /api/identity/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "password123"
}

Response: (same as SSO callback)
{
  "accessToken": "eyJ...",
  "refreshToken": "...",
  "expiresIn": 3600,
  "user": { ... }
}
```

#### 3. Token Refresh

```http
POST /api/identity/auth/refresh
Content-Type: application/json

{
  "refreshToken": "current-refresh-token"
}

Response:
{
  "accessToken": "new-access-token",
  "refreshToken": "new-refresh-token",
  "expiresIn": 3600
}
```

#### 4. Logout

```http
POST /api/identity/auth/logout
Authorization: Bearer {accessToken}

Response:
{
  "success": true,
  "message": "Logged out successfully"
}
```

#### 5. Change Password

```http
POST /api/identity/auth/change-password
Authorization: Bearer {accessToken}
Content-Type: application/json

{
  "currentPassword": "old-password",
  "newPassword": "new-password",
  "confirmPassword": "new-password"
}
```

### Using Authentication in Requests

All authenticated endpoints require the `Authorization` header:

```http
GET /api/identity/users/me
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Authorization Policies

| Policy | Description |
|--------|-------------|
| `CanManageUsers` | User management (create, edit, delete users) |
| `CanManageContent` | Content management operations |
| `CanCreateContent` | Create new content |
| `CanEditContent` | Edit existing content |
| `CanDeleteContent` | Delete content |
| `CanPublishContent` | Publish/unpublish content |
| `CanManageLibraries` | Document library management |
| `CanUploadDocuments` | Upload documents |
| `CanEditDocuments` | Edit document metadata |
| `CanDeleteDocuments` | Delete documents |
| `CanPublishDocuments` | Publish documents |
| `CanManageWorkflow` | Workflow definition management |
| `CanViewAllTasks` | View all workflow tasks |
| `CanAssignRequests` | Assign service requests |
| `AIAdmin` | AI service administration |
| `IntegrationAdmin` | Integration management |
| `NotificationAdmin` | Notification management |

---

## API Response Formats

### Success Response

```json
{
  "success": true,
  "data": { },
  "message": "Operation completed successfully",
  "timestamp": "2025-12-22T10:30:00Z"
}
```

### Paginated Response

```json
{
  "data": [
    { "id": "1", "title": "Item 1" },
    { "id": "2", "title": "Item 2" }
  ],
  "pagination": {
    "page": 1,
    "pageSize": 20,
    "totalCount": 150,
    "totalPages": 8,
    "hasNextPage": true,
    "hasPreviousPage": false
  }
}
```

### Error Response

```json
{
  "success": false,
  "message": "Validation failed",
  "errors": {
    "email": ["Email is required", "Invalid email format"],
    "password": ["Password must be at least 8 characters"]
  },
  "statusCode": 400,
  "timestamp": "2025-12-22T10:30:00Z"
}
```

### Common HTTP Status Codes

| Code | Meaning |
|------|---------|
| 200 | Success |
| 201 | Created |
| 204 | No Content (successful delete) |
| 400 | Bad Request (validation error) |
| 401 | Unauthorized (missing/invalid token) |
| 403 | Forbidden (insufficient permissions) |
| 404 | Not Found |
| 409 | Conflict (duplicate entry) |
| 500 | Internal Server Error |

---

## Core Endpoints

### Identity & Users

#### Get Current User
```http
GET /api/identity/users/me
Authorization: Bearer {token}

Response:
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "email": "john.doe@example.com",
  "displayName": "John Doe",
  "firstName": "John",
  "lastName": "Doe",
  "jobTitle": "Project Manager",
  "department": "Operations",
  "avatar": "https://...",
  "roles": ["User", "Editor"],
  "permissions": ["CanCreateContent", "CanEditContent"],
  "isActive": true,
  "lastLoginAt": "2025-12-22T09:00:00Z"
}
```

#### Get User Profile
```http
GET /api/identity/users/me/profile
Authorization: Bearer {token}

Response:
{
  "id": "...",
  "email": "...",
  "displayName": "...",
  "firstName": "...",
  "lastName": "...",
  "phoneNumber": "+1234567890",
  "jobTitle": "...",
  "department": "...",
  "location": "...",
  "timezone": "Asia/Riyadh",
  "language": "en",
  "avatar": "...",
  "bio": "...",
  "socialLinks": {
    "linkedin": "...",
    "twitter": "..."
  },
  "preferences": {
    "emailNotifications": true,
    "pushNotifications": true,
    "theme": "light"
  }
}
```

#### Update Profile
```http
PUT /api/identity/users/me/profile
Authorization: Bearer {token}
Content-Type: application/json

{
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "+1234567890",
  "jobTitle": "Senior Project Manager",
  "bio": "Experienced PM...",
  "timezone": "Asia/Riyadh",
  "language": "en",
  "preferences": {
    "emailNotifications": true,
    "theme": "dark"
  }
}
```

#### Search Users
```http
GET /api/identity/users/search?q=john&limit=10
Authorization: Bearer {token}

Response:
[
  {
    "id": "...",
    "displayName": "John Doe",
    "email": "john.doe@example.com",
    "avatar": "...",
    "department": "Operations"
  }
]
```

#### List Users (Admin)
```http
GET /api/identity/users?page=1&pageSize=20&role=Editor&isActive=true
Authorization: Bearer {token}
Required: CanManageUsers permission

Response: Paginated list of users
```

#### Get Organization Chart
```http
GET /api/identity/users/org-chart
Authorization: Bearer {token}

Response:
{
  "id": "...",
  "name": "CEO",
  "title": "Chief Executive Officer",
  "avatar": "...",
  "children": [
    {
      "id": "...",
      "name": "CTO",
      "title": "Chief Technology Officer",
      "children": [...]
    }
  ]
}
```

---

### Dashboard

#### Service Catalog Dashboard
```http
GET /api/services/dashboard
Authorization: Bearer {token}

Response:
{
  "totalServices": 45,
  "activeRequests": 23,
  "pendingApprovals": 8,
  "completedThisMonth": 156,
  "averageResolutionTime": "2.5 days",
  "slaCompliance": 94.5,
  "requestsByCategory": [
    { "category": "IT Support", "count": 45 },
    { "category": "HR Services", "count": 32 }
  ],
  "recentRequests": [...]
}
```

#### AI Insights Dashboard
```http
GET /api/ai/dashboard
Authorization: Bearer {token}

Response:
{
  "totalAnalyses": 1250,
  "documentsAnalyzed": 890,
  "averageSentiment": 0.72,
  "topEntities": [
    { "name": "AFC 2027", "type": "Event", "count": 456 }
  ],
  "trendingTopics": [
    { "topic": "Stadium Requirements", "count": 89, "trend": "up" }
  ],
  "sentimentOverview": {
    "positive": 65,
    "neutral": 25,
    "negative": 10
  },
  "recentAnalyses": [...]
}
```

#### Learning Dashboard
```http
GET /api/learning/dashboard
Authorization: Bearer {token}

Response:
{
  "enrolledCourses": 5,
  "completedCourses": 12,
  "certificatesEarned": 8,
  "totalLearningHours": 45.5,
  "currentStreak": 7,
  "inProgressCourses": [...],
  "recommendedPaths": [...],
  "recentAchievements": [...]
}
```

#### KPI Dashboard
```http
GET /api/kpis/dashboard?teamId={optional}
Authorization: Bearer {token}

Response:
{
  "cards": [
    {
      "id": "...",
      "name": "Customer Satisfaction",
      "currentValue": 4.5,
      "targetValue": 4.0,
      "unit": "score",
      "trend": "up",
      "trendPercentage": 12.5,
      "status": "on_track"
    }
  ]
}
```

---

### Content & Articles

#### List Articles
```http
GET /api/content/articles?page=1&pageSize=20&category=news&status=published
Authorization: Optional (public articles)

Query Parameters:
- page: Page number (default: 1)
- pageSize: Items per page (default: 20, max: 100)
- category: Filter by category
- status: draft, pending, published, archived
- authorId: Filter by author
- tag: Filter by tag
- search: Search in title/content
- fromDate: Published after date
- toDate: Published before date
- sortBy: publishedAt, title, viewCount
- sortOrder: asc, desc

Response: Paginated ArticleSummaryDto[]
```

#### Get Featured Articles
```http
GET /api/content/articles/featured?limit=5

Response:
[
  {
    "id": "...",
    "title": "AFC 2027 Stadium Guidelines Released",
    "slug": "afc-2027-stadium-guidelines",
    "summary": "...",
    "featuredImageUrl": "...",
    "authorName": "John Doe",
    "publishedAt": "2025-12-20T10:00:00Z",
    "category": "News",
    "readTime": 5,
    "viewCount": 1250
  }
]
```

#### Get Article by ID
```http
GET /api/content/articles/{id}

Response:
{
  "id": "...",
  "title": "...",
  "slug": "...",
  "summary": "...",
  "content": "<p>Full HTML content...</p>",
  "featuredImageUrl": "...",
  "author": {
    "id": "...",
    "displayName": "John Doe",
    "avatar": "..."
  },
  "category": "News",
  "tags": ["stadium", "guidelines", "afc2027"],
  "status": "published",
  "publishedAt": "...",
  "createdAt": "...",
  "updatedAt": "...",
  "viewCount": 1250,
  "readTime": 5,
  "isFeatured": true,
  "relatedArticles": [...]
}
```

#### Get Article by Slug
```http
GET /api/content/articles/by-slug/{slug}
```

#### Create Article
```http
POST /api/content/articles
Authorization: Bearer {token}
Required: CanCreateContent
Content-Type: application/json

{
  "title": "New Article Title",
  "titleArabic": "عنوان المقال الجديد",
  "summary": "Brief summary...",
  "summaryArabic": "ملخص موجز...",
  "content": "<p>Full content...</p>",
  "contentArabic": "<p>المحتوى الكامل...</p>",
  "category": "News",
  "tags": ["tag1", "tag2"],
  "featuredImageUrl": "https://..."
}
```

#### Update Article
```http
PUT /api/content/articles/{id}
Authorization: Bearer {token}
Required: CanEditContent
```

#### Publish Article
```http
POST /api/content/articles/{id}/publish
Authorization: Bearer {token}
Required: CanPublishContent

{
  "publishAt": "2025-12-25T10:00:00Z",  // Optional: schedule
  "notifySubscribers": true
}
```

#### Get Related Articles
```http
GET /api/content/articles/{id}/related?limit=5
```

---

### Documents

#### List Documents
```http
GET /api/documents?libraryId={id}&folderId={id}&page=1&pageSize=20
Authorization: Bearer {token}

Query Parameters:
- libraryId: Filter by library
- folderId: Filter by folder
- searchTerm: Search in name/content
- fileType: pdf, docx, xlsx, etc.
- status: draft, published, archived
- page, pageSize: Pagination
```

#### Get Recent Documents
```http
GET /api/documents/recent?limit=10
Authorization: Bearer {token}
```

#### Get Document Details
```http
GET /api/documents/{id}
Authorization: Bearer {token}

Response:
{
  "id": "...",
  "name": "Stadium-Guidelines-v2.pdf",
  "description": "...",
  "fileType": "pdf",
  "fileSize": 2456789,
  "mimeType": "application/pdf",
  "libraryId": "...",
  "libraryName": "Official Documents",
  "folderId": "...",
  "folderPath": "/Guidelines/Stadiums",
  "status": "published",
  "version": 2,
  "isCheckedOut": false,
  "checkedOutBy": null,
  "createdBy": { "id": "...", "displayName": "..." },
  "createdAt": "...",
  "updatedAt": "...",
  "publishedAt": "...",
  "tags": ["stadium", "guidelines"],
  "metadata": {
    "author": "John Doe",
    "department": "Operations"
  },
  "permissions": {
    "canEdit": true,
    "canDelete": false,
    "canPublish": true
  }
}
```

#### Upload Document
```http
POST /api/documents/upload
Authorization: Bearer {token}
Required: CanUploadDocuments
Content-Type: multipart/form-data

Form Fields:
- file: The document file (required)
- name: Display name (optional, defaults to filename)
- description: Description (optional)
- libraryId: Target library ID (required)
- folderId: Target folder ID (optional)
- tags: Comma-separated tags (optional)
- metadata: JSON string of custom metadata (optional)

Response:
{
  "id": "new-document-id",
  "name": "...",
  "fileType": "...",
  ...
}
```

#### Download Document
```http
GET /api/documents/{id}/download
Authorization: Bearer {token}

Response: File stream with appropriate Content-Type header
```

#### Check Out Document
```http
POST /api/documents/{id}/checkout
Authorization: Bearer {token}

Response:
{
  "success": true,
  "message": "Document checked out",
  "checkedOutAt": "...",
  "expiresAt": "..."
}
```

#### Check In Document
```http
POST /api/documents/{id}/checkin
Authorization: Bearer {token}
Content-Type: multipart/form-data

Form Fields:
- file: Updated document file (required)
- comments: Version comments (optional)
- majorVersion: Boolean - create major version (optional)
```

#### Get Document Versions
```http
GET /api/documents/{id}/versions
Authorization: Bearer {token}

Response:
[
  {
    "versionId": "...",
    "versionNumber": 2,
    "isMajorVersion": true,
    "createdBy": { "id": "...", "displayName": "..." },
    "createdAt": "...",
    "comments": "Updated guidelines",
    "fileSize": 2456789
  }
]
```

#### Move Document
```http
POST /api/documents/{id}/move
Authorization: Bearer {token}
Content-Type: application/json

{
  "targetFolderId": "...",
  "targetLibraryId": "..."
}
```

---

### Search

#### Global Search
```http
GET /api/search?query=stadium+guidelines&page=1&pageSize=20
Authorization: Bearer {token}

Query Parameters:
- query: Search query (required)
- page, pageSize: Pagination
- contentType: articles, documents, users, etc.
- dateFrom, dateTo: Date range filter

Response:
{
  "query": "stadium guidelines",
  "totalResults": 45,
  "results": [
    {
      "id": "...",
      "type": "article",
      "title": "Stadium Guidelines 2027",
      "summary": "...highlighted match...",
      "url": "/articles/stadium-guidelines",
      "score": 0.95,
      "createdAt": "...",
      "author": "John Doe"
    }
  ],
  "facets": {
    "contentType": [
      { "value": "article", "count": 25 },
      { "value": "document", "count": 20 }
    ],
    "category": [...],
    "author": [...]
  },
  "suggestions": ["stadium requirements", "stadium capacity"]
}
```

#### Advanced Search
```http
POST /api/search/advanced
Authorization: Bearer {token}
Content-Type: application/json

{
  "query": "stadium",
  "filters": {
    "contentType": ["article", "document"],
    "category": ["Guidelines", "News"],
    "author": ["user-id-1", "user-id-2"],
    "dateRange": {
      "from": "2025-01-01",
      "to": "2025-12-31"
    },
    "tags": ["afc2027", "stadium"]
  },
  "sort": {
    "field": "relevance",
    "order": "desc"
  },
  "page": 1,
  "pageSize": 20
}
```

#### Trending Searches
```http
GET /api/search/trending?language=en&limit=10

Response:
[
  { "query": "stadium requirements", "count": 156 },
  { "query": "media accreditation", "count": 98 }
]
```

#### Search History
```http
GET /api/search/history?limit=10
Authorization: Bearer {token}

Response:
[
  { "query": "stadium", "searchedAt": "2025-12-22T10:00:00Z" }
]
```

---

### AI Services

#### Semantic Search
```http
POST /api/ai/search/semantic
Authorization: Bearer {token}
Content-Type: application/json

{
  "query": "What are the requirements for stadium capacity?",
  "maxResults": 10,
  "threshold": 0.7,
  "contentTypes": ["article", "document"]
}

Response:
[
  {
    "id": "...",
    "type": "document",
    "title": "Stadium Requirements Guide",
    "content": "...relevant excerpt...",
    "score": 0.92,
    "metadata": {...}
  }
]
```

#### Chat / Ask AI
```http
POST /api/ai/chat
Authorization: Bearer {token}
Content-Type: application/json

{
  "message": "What are the stadium requirements for AFC 2027?",
  "conversationId": null,  // null for new conversation
  "includeSourceCitations": true
}

Response:
{
  "conversationId": "...",
  "response": "Based on the AFC 2027 guidelines, stadiums must meet the following requirements...",
  "citations": [
    {
      "documentId": "...",
      "documentTitle": "Stadium Guidelines v2",
      "excerpt": "...",
      "pageNumber": 15
    }
  ],
  "suggestedQuestions": [
    "What is the minimum seating capacity?",
    "What are the lighting requirements?"
  ]
}
```

#### Streaming Chat
```http
POST /api/ai/chat/stream
Authorization: Bearer {token}
Content-Type: application/json
Accept: text/event-stream

{
  "message": "Explain the media accreditation process",
  "conversationId": "..."
}

Response: Server-Sent Events (SSE)
data: {"type": "token", "content": "The "}
data: {"type": "token", "content": "media "}
data: {"type": "token", "content": "accreditation..."}
data: {"type": "citations", "content": [...]}
data: {"type": "done"}
```

#### Summarize Text
```http
POST /api/ai/summarize
Authorization: Bearer {token}
Content-Type: application/json

{
  "text": "Long text to summarize...",
  "maxLength": 200,
  "style": "bullet_points"  // or "paragraph"
}

Response:
{
  "summary": "• Key point 1\n• Key point 2\n• Key point 3"
}
```

#### Analyze Document
```http
POST /api/ai/documents/{documentId}/analyze
Authorization: Bearer {token}

{
  "analysisTypes": ["entities", "sentiment", "topics", "summary"]
}

Response:
{
  "documentId": "...",
  "analysisId": "...",
  "status": "completed",
  "entities": [
    { "text": "AFC 2027", "type": "Event", "confidence": 0.98 }
  ],
  "sentiment": {
    "overall": "positive",
    "score": 0.75,
    "breakdown": { "positive": 0.75, "neutral": 0.20, "negative": 0.05 }
  },
  "topics": [
    { "topic": "Stadium Requirements", "relevance": 0.92 }
  ],
  "summary": "This document outlines..."
}
```

#### Get Recommendations
```http
GET /api/ai/recommendations?maxResults=10
Authorization: Bearer {token}

Response:
[
  {
    "id": "...",
    "type": "article",
    "title": "Recommended Article",
    "reason": "Based on your recent activity",
    "score": 0.89
  }
]
```

---

### Service Catalog

#### Get Service Categories
```http
GET /api/services/categories?includeServices=true
Authorization: Bearer {token}

Response:
[
  {
    "id": "...",
    "name": "IT Support",
    "nameArabic": "دعم تقنية المعلومات",
    "description": "...",
    "icon": "pi-desktop",
    "color": "#3B82F6",
    "serviceCount": 12,
    "services": [...]  // if includeServices=true
  }
]
```

#### Get Service Catalog
```http
GET /api/services/catalog?categoryId={id}&search=laptop&page=1&pageSize=20
Authorization: Bearer {token}

Response: Paginated list of services
{
  "data": [
    {
      "id": "...",
      "name": "Laptop Request",
      "nameArabic": "طلب حاسوب محمول",
      "description": "...",
      "categoryId": "...",
      "categoryName": "IT Support",
      "icon": "pi-laptop",
      "estimatedDuration": "3-5 business days",
      "requiredFields": [...],
      "approvalRequired": true
    }
  ]
}
```

#### Submit Service Request
```http
POST /api/services/{serviceId}/request
Authorization: Bearer {token}
Content-Type: application/json

{
  "formData": {
    "laptopType": "MacBook Pro",
    "reason": "New hire equipment",
    "urgency": "normal"
  },
  "attachments": ["file-id-1", "file-id-2"],
  "notes": "Please deliver to Building A"
}

Response:
{
  "id": "...",
  "requestNumber": "SR-2025-00123",
  "serviceName": "Laptop Request",
  "status": "Pending",
  "createdAt": "...",
  "estimatedCompletion": "..."
}
```

#### Get My Requests
```http
GET /api/services/my-requests?status=pending&page=1&pageSize=20
Authorization: Bearer {token}
```

#### Get Pending Approvals
```http
GET /api/services/approvals/pending?page=1&pageSize=20
Authorization: Bearer {token}

Response: Paginated list of requests awaiting user's approval
```

#### Approve/Reject Request
```http
POST /api/services/requests/{id}/approve
Authorization: Bearer {token}
Content-Type: application/json

{
  "decision": "approved",  // or "rejected"
  "comments": "Approved as requested",
  "conditions": null  // optional conditions
}
```

---

### Workflow & Tasks

#### Get Workflow Definitions
```http
GET /api/workflow/definitions?status=active
Authorization: Bearer {token}
Required: CanManageWorkflow
```

#### Get Pending Tasks
```http
GET /api/services/requests/pending?assigneeId=me&status=pending
Authorization: Bearer {token}
```

---

### Notifications

#### Get Notifications
```http
GET /api/notifications?page=1&pageSize=20&unreadOnly=false&category=system
Authorization: Bearer {token}

Response:
{
  "data": [
    {
      "id": "...",
      "title": "New Task Assigned",
      "message": "You have been assigned a new task...",
      "type": "task",
      "category": "workflow",
      "isRead": false,
      "createdAt": "2025-12-22T10:00:00Z",
      "actionUrl": "/tasks/123",
      "data": {
        "taskId": "123",
        "taskTitle": "Review Document"
      }
    }
  ]
}
```

#### Get Unread Count
```http
GET /api/notifications/unread-count
Authorization: Bearer {token}

Response:
{
  "count": 5
}
```

#### Mark as Read
```http
POST /api/notifications/{id}/read
Authorization: Bearer {token}
```

#### Mark All as Read
```http
POST /api/notifications/read-all?category=workflow
Authorization: Bearer {token}
```

#### Get Notification Stats
```http
GET /api/notifications/stats
Authorization: Bearer {token}

Response:
{
  "total": 150,
  "unread": 5,
  "byCategory": {
    "workflow": 45,
    "system": 30,
    "content": 75
  },
  "byType": {
    "task": 25,
    "approval": 20,
    "mention": 15
  }
}
```

---

### Calendar & Events

#### Get Events
```http
GET /api/calendar/events?startDate=2025-12-01&endDate=2025-12-31
Authorization: Bearer {token}

Response:
[
  {
    "id": "...",
    "title": "Team Meeting",
    "description": "Weekly sync...",
    "startTime": "2025-12-22T10:00:00Z",
    "endTime": "2025-12-22T11:00:00Z",
    "location": "Conference Room A",
    "isAllDay": false,
    "isRecurring": true,
    "recurrencePattern": "weekly",
    "attendees": [...],
    "organizer": { "id": "...", "displayName": "..." },
    "status": "confirmed",
    "color": "#3B82F6"
  }
]
```

#### Get Today's Events
```http
GET /api/calendar/events/today
Authorization: Bearer {token}
```

#### Get Upcoming Events
```http
GET /api/calendar/events/upcoming?days=7&limit=10
Authorization: Bearer {token}
```

#### Create Event
```http
POST /api/calendar/events
Authorization: Bearer {token}
Content-Type: application/json

{
  "title": "Project Review",
  "description": "Quarterly project review meeting",
  "startTime": "2025-12-25T14:00:00Z",
  "endTime": "2025-12-25T15:00:00Z",
  "location": "Conference Room B",
  "isAllDay": false,
  "attendeeIds": ["user-id-1", "user-id-2"],
  "recurrence": {
    "frequency": "weekly",
    "interval": 1,
    "endDate": "2026-03-25"
  },
  "reminders": [
    { "type": "email", "minutesBefore": 60 },
    { "type": "push", "minutesBefore": 15 }
  ]
}
```

#### RSVP to Event
```http
POST /api/calendar/events/{id}/rsvp
Authorization: Bearer {token}
Content-Type: application/json

{
  "response": "accepted",  // accepted, declined, tentative
  "comment": "Looking forward to it!"
}
```

---

### Meetings

#### Get Meetings
```http
GET /api/meetings?fromDate=2025-12-01&toDate=2025-12-31&status=scheduled
Authorization: Bearer {token}
```

#### Create Meeting Link
```http
POST /api/meetings
Authorization: Bearer {token}
Content-Type: application/json

{
  "title": "Project Kickoff",
  "description": "Initial project meeting",
  "scheduledAt": "2025-12-25T10:00:00Z",
  "duration": 60,
  "platform": "teams",  // teams, zoom, meet
  "attendeeIds": ["user-id-1", "user-id-2"]
}

Response:
{
  "id": "...",
  "title": "Project Kickoff",
  "meetingUrl": "https://teams.microsoft.com/...",
  "joinCode": "ABC123",
  ...
}
```

#### Get Meeting Calendar
```http
GET /api/meetings/calendar?startDate=2025-12-01&endDate=2025-12-31
Authorization: Bearer {token}
```

#### Get My Action Items
```http
GET /api/meetings/my-actions?status=pending
Authorization: Bearer {token}

Response:
[
  {
    "id": "...",
    "title": "Prepare presentation",
    "description": "...",
    "meetingId": "...",
    "meetingTitle": "Project Review",
    "dueDate": "2025-12-24",
    "status": "pending",
    "assignedTo": { "id": "...", "displayName": "..." }
  }
]
```

---

### Polls

#### Get Active Polls
```http
GET /api/polls/active
Authorization: Bearer {token}

Response:
[
  {
    "id": "...",
    "question": "What should be our next team activity?",
    "questionArabic": "ما هو نشاط الفريق القادم؟",
    "options": [
      { "id": "a", "text": "Sports Day", "textArabic": "يوم رياضي" },
      { "id": "b", "text": "Workshop", "textArabic": "ورشة عمل" }
    ],
    "totalVotes": 45,
    "hasVoted": false,
    "expiresAt": "2025-12-25T23:59:59Z",
    "createdBy": { "id": "...", "displayName": "..." }
  }
]
```

#### Cast Vote
```http
POST /api/polls/{id}/vote
Authorization: Bearer {token}
Content-Type: application/json

{
  "optionId": "a",
  "comment": "Great idea!"  // optional
}
```

#### Get Poll Results
```http
GET /api/polls/{id}/results
Authorization: Bearer {token}

Response:
{
  "pollId": "...",
  "question": "...",
  "totalVotes": 45,
  "results": [
    { "optionId": "a", "text": "Sports Day", "votes": 25, "percentage": 55.6 },
    { "optionId": "b", "text": "Workshop", "votes": 20, "percentage": 44.4 }
  ],
  "myVote": { "optionId": "a", "votedAt": "..." }
}
```

---

### Learning Management

#### Get Learning Paths
```http
GET /api/learning/paths?category=technical&difficulty=intermediate&page=1&pageSize=20
Authorization: Bearer {token}
```

#### Get My Enrollments
```http
GET /api/learning/my-enrollments?status=in_progress
Authorization: Bearer {token}

Response:
[
  {
    "id": "...",
    "pathId": "...",
    "pathTitle": "Project Management Fundamentals",
    "progress": 65,
    "currentModule": 3,
    "totalModules": 5,
    "enrolledAt": "2025-12-01",
    "lastAccessedAt": "2025-12-21",
    "estimatedCompletion": "2025-12-30"
  }
]
```

#### Update Progress
```http
POST /api/learning/enrollments/{id}/progress
Authorization: Bearer {token}
Content-Type: application/json

{
  "moduleId": "module-3",
  "lessonId": "lesson-2",
  "completed": true,
  "timeSpentMinutes": 25
}
```

#### Get My Certificates
```http
GET /api/learning/my-certificates
Authorization: Bearer {token}

Response:
[
  {
    "id": "...",
    "certificateNumber": "CERT-2025-00123",
    "title": "Project Management Professional",
    "issuedAt": "2025-12-15",
    "expiresAt": "2027-12-15",
    "downloadUrl": "/api/learning/certificates/{id}/download"
  }
]
```

#### Verify Certificate (Public)
```http
GET /api/learning/certificates/verify/{certificateNumber}

Response:
{
  "certificateNumber": "CERT-2025-00123",
  "isValid": true,
  "title": "Project Management Professional",
  "holderName": "John Doe",
  "issuedAt": "2025-12-15",
  "expiresAt": "2027-12-15"
}
```

---

### KPIs

#### Get KPI Definitions
```http
GET /api/kpis?category=operations&scope=team
Authorization: Bearer {token}
```

#### Record KPI Value
```http
POST /api/kpis/values
Authorization: Bearer {token}
Content-Type: application/json

{
  "kpiId": "...",
  "value": 4.5,
  "periodStart": "2025-12-01",
  "periodEnd": "2025-12-31",
  "notes": "Exceeded target"
}
```

#### Get Personal KPI Summary
```http
GET /api/kpis/my-summary
Authorization: Bearer {token}

Response:
{
  "totalKpis": 5,
  "onTrack": 3,
  "atRisk": 1,
  "offTrack": 1,
  "kpis": [
    {
      "id": "...",
      "name": "Customer Satisfaction",
      "currentValue": 4.5,
      "targetValue": 4.0,
      "status": "on_track",
      "trend": "up"
    }
  ]
}
```

---

### Quality Assurance

#### Get Quality Dashboard
```http
GET /api/quality/dashboard
Authorization: Bearer {token}

Response:
{
  "totalReviews": 150,
  "pendingReviews": 12,
  "averageScore": 4.2,
  "complianceRate": 95.5,
  "reviewsByStatus": {
    "pending": 12,
    "in_progress": 8,
    "completed": 130
  },
  "recentReviews": [...]
}
```

#### Get My Reviews
```http
GET /api/quality/my-reviews
Authorization: Bearer {token}
```

#### Submit Review
```http
POST /api/quality/reviews/{id}/submit
Authorization: Bearer {token}
Content-Type: application/json

{
  "score": 4,
  "findings": [
    { "item": "Accuracy", "passed": true, "notes": "All data verified" },
    { "item": "Formatting", "passed": false, "notes": "Minor issues found" }
  ],
  "overallComments": "Good quality document with minor formatting issues",
  "recommendation": "approve_with_changes"
}
```

---

### Document Templates

#### Get Templates
```http
GET /api/templates?category=policies&type=word&status=published
Authorization: Bearer {token}
```

#### Generate Document from Template
```http
POST /api/templates/generate
Authorization: Bearer {token}
Content-Type: application/json

{
  "templateId": "...",
  "data": {
    "projectName": "AFC 2027 Stadium Project",
    "date": "2025-12-22",
    "author": "John Doe"
  },
  "format": "pdf"  // pdf, docx
}

Response:
{
  "documentId": "...",
  "fileName": "AFC-2027-Stadium-Project-Report.pdf",
  "downloadUrl": "/api/documents/{id}/download"
}
```

---

### Barcodes & QR Codes

#### Generate QR Code
```http
POST /api/barcodes/generate
Authorization: Bearer {token}
Content-Type: application/json

{
  "content": "https://kms.afc2027.com/documents/123",
  "type": "qr",
  "size": 300,
  "format": "png"
}

Response: Image file or base64 string
```

#### Generate Document QR
```http
POST /api/barcodes/documents/{documentId}/qr
Authorization: Bearer {token}

Response: QR code image for document access
```

#### Scan Accreditation
```http
POST /api/barcodes/accreditation/scan
Authorization: Bearer {token}
Content-Type: application/json

{
  "barcodeContent": "ACC-2027-00123",
  "location": "Stadium A - Gate 1"
}

Response:
{
  "isValid": true,
  "accreditation": {
    "id": "...",
    "holderName": "John Doe",
    "type": "Media",
    "validFrom": "2027-01-01",
    "validTo": "2027-02-28",
    "zones": ["Media Center", "Press Box", "Mixed Zone"]
  }
}
```

---

### Integration & Connectors

#### Get Connectors (Admin)
```http
GET /api/integration/connectors?category=storage&isActive=true
Authorization: Bearer {token}
Required: IntegrationAdmin
```

#### Test Connection
```http
POST /api/integration/connectors/{id}/test
Authorization: Bearer {token}
Required: IntegrationAdmin

Response:
{
  "success": true,
  "message": "Connection successful",
  "latencyMs": 45,
  "details": { ... }
}
```

#### Get Integration Health
```http
GET /api/integration/health
Authorization: Bearer {token}
Required: IntegrationAdmin

Response:
{
  "overallStatus": "healthy",
  "services": [
    { "name": "Azure AD", "status": "healthy", "lastChecked": "..." },
    { "name": "SharePoint", "status": "degraded", "lastChecked": "..." }
  ]
}
```

---

## WebSocket / Real-time

### SignalR Hub Connection

```javascript
import * as signalR from '@microsoft/signalr';

const connection = new signalR.HubConnectionBuilder()
  .withUrl('http://localhost:5001/hubs/notifications', {
    accessTokenFactory: () => localStorage.getItem('accessToken')
  })
  .withAutomaticReconnect()
  .build();

// Connect
await connection.start();

// Listen for notifications
connection.on('ReceiveNotification', (notification) => {
  console.log('New notification:', notification);
});

// Listen for real-time updates
connection.on('DocumentUpdated', (documentId, changes) => {
  console.log('Document updated:', documentId, changes);
});

connection.on('TaskAssigned', (task) => {
  console.log('New task assigned:', task);
});
```

### Available Hub Events

| Event | Payload | Description |
|-------|---------|-------------|
| `ReceiveNotification` | NotificationDto | New notification received |
| `DocumentUpdated` | { documentId, changes } | Document was modified |
| `DocumentCheckedOut` | { documentId, userId } | Document checked out |
| `TaskAssigned` | TaskDto | New task assigned to user |
| `ApprovalRequired` | RequestDto | New approval required |
| `ChatMessage` | ChatMessageDto | New chat message |

---

## Error Handling

### Common Error Codes

| Code | Name | Description |
|------|------|-------------|
| `AUTH_001` | InvalidCredentials | Email or password incorrect |
| `AUTH_002` | TokenExpired | Access token has expired |
| `AUTH_003` | TokenInvalid | Token is malformed or invalid |
| `AUTH_004` | RefreshTokenExpired | Refresh token has expired |
| `AUTH_005` | AccountDisabled | User account is disabled |
| `PERM_001` | AccessDenied | Insufficient permissions |
| `VAL_001` | ValidationFailed | Request validation failed |
| `VAL_002` | RequiredField | Required field is missing |
| `RES_001` | NotFound | Resource not found |
| `RES_002` | AlreadyExists | Resource already exists |
| `RES_003` | Conflict | Resource conflict (e.g., checked out) |
| `FILE_001` | FileTooLarge | File exceeds size limit |
| `FILE_002` | InvalidFileType | File type not allowed |

### Error Response Format

```json
{
  "success": false,
  "errorCode": "VAL_001",
  "message": "Validation failed",
  "errors": {
    "email": ["Invalid email format"],
    "password": ["Password must be at least 8 characters"]
  },
  "timestamp": "2025-12-22T10:30:00Z",
  "traceId": "abc123..."
}
```

---

## TypeScript Interfaces

```typescript
// User & Auth
interface User {
  id: string;
  email: string;
  displayName: string;
  firstName: string;
  lastName: string;
  jobTitle?: string;
  department?: string;
  avatar?: string;
  roles: string[];
  permissions: string[];
  isActive: boolean;
}

interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
  tokenType: string;
  user: User;
}

// Content
interface Article {
  id: string;
  title: string;
  titleArabic?: string;
  slug: string;
  summary: string;
  summaryArabic?: string;
  content: string;
  contentArabic?: string;
  featuredImageUrl?: string;
  author: UserSummary;
  category: string;
  tags: string[];
  status: 'draft' | 'pending' | 'published' | 'archived';
  publishedAt?: string;
  createdAt: string;
  updatedAt: string;
  viewCount: number;
  readTime: number;
  isFeatured: boolean;
}

// Documents
interface Document {
  id: string;
  name: string;
  description?: string;
  fileType: string;
  fileSize: number;
  mimeType: string;
  libraryId: string;
  libraryName: string;
  folderId?: string;
  folderPath: string;
  status: 'draft' | 'published' | 'archived';
  version: number;
  isCheckedOut: boolean;
  checkedOutBy?: UserSummary;
  createdBy: UserSummary;
  createdAt: string;
  updatedAt: string;
  tags: string[];
  metadata: Record<string, any>;
}

// Service Requests
interface ServiceRequest {
  id: string;
  requestNumber: string;
  serviceId: string;
  serviceName: string;
  serviceNameArabic?: string;
  status: 'Pending' | 'InProgress' | 'Completed' | 'Cancelled' | 'Rejected';
  priority: 'low' | 'medium' | 'high' | 'urgent';
  requester: UserSummary;
  assignee?: UserSummary;
  formData: Record<string, any>;
  createdAt: string;
  updatedAt: string;
  completedAt?: string;
  slaDeadline?: string;
  isOverdue: boolean;
}

// Notifications
interface Notification {
  id: string;
  title: string;
  message: string;
  type: string;
  category: string;
  isRead: boolean;
  createdAt: string;
  actionUrl?: string;
  data?: Record<string, any>;
}

// Calendar
interface CalendarEvent {
  id: string;
  title: string;
  description?: string;
  startTime: string;
  endTime: string;
  location?: string;
  isAllDay: boolean;
  isRecurring: boolean;
  recurrencePattern?: string;
  attendees: Attendee[];
  organizer: UserSummary;
  status: 'tentative' | 'confirmed' | 'cancelled';
  color?: string;
}

// Polls
interface Poll {
  id: string;
  question: string;
  questionArabic?: string;
  options: PollOption[];
  totalVotes: number;
  hasVoted: boolean;
  myVoteId?: string;
  expiresAt?: string;
  createdAt: string;
  createdBy: UserSummary;
  category?: string;
}

interface PollOption {
  id: string;
  text: string;
  textArabic?: string;
  votes?: number;
  percentage?: number;
}

// Learning
interface LearningEnrollment {
  id: string;
  pathId: string;
  pathTitle: string;
  progress: number;
  currentModule: number;
  totalModules: number;
  enrolledAt: string;
  lastAccessedAt: string;
  completedAt?: string;
}

// Common
interface UserSummary {
  id: string;
  displayName: string;
  email?: string;
  avatar?: string;
}

interface PaginatedResponse<T> {
  data: T[];
  pagination: {
    page: number;
    pageSize: number;
    totalCount: number;
    totalPages: number;
    hasNextPage: boolean;
    hasPreviousPage: boolean;
  };
}

interface ApiResponse<T = any> {
  success: boolean;
  data?: T;
  message?: string;
  errors?: Record<string, string[]>;
  timestamp: string;
}
```

---

## Frontend Integration Examples

### API Client Setup (Axios)

```typescript
// api/client.ts
import axios, { AxiosInstance, AxiosError } from 'axios';

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5001/api';

const api: AxiosInstance = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
  timeout: 30000,
});

// Request interceptor - add auth token
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('accessToken');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }

    // Add language header
    const language = localStorage.getItem('language') || 'en';
    config.headers['Accept-Language'] = language;

    return config;
  },
  (error) => Promise.reject(error)
);

// Response interceptor - handle token refresh
api.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    const originalRequest = error.config as any;

    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;

      try {
        const refreshToken = localStorage.getItem('refreshToken');
        if (!refreshToken) {
          throw new Error('No refresh token');
        }

        const { data } = await axios.post(`${API_BASE_URL}/identity/auth/refresh`, {
          refreshToken,
        });

        localStorage.setItem('accessToken', data.accessToken);
        localStorage.setItem('refreshToken', data.refreshToken);

        originalRequest.headers.Authorization = `Bearer ${data.accessToken}`;
        return api(originalRequest);
      } catch (refreshError) {
        // Refresh failed - clear tokens and redirect to login
        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');
        window.location.href = '/login';
        return Promise.reject(refreshError);
      }
    }

    return Promise.reject(error);
  }
);

export default api;
```

### Authentication Service

```typescript
// services/auth.service.ts
import api from './client';

export interface LoginRequest {
  email: string;
  password: string;
}

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
  user: User;
}

export const authService = {
  async login(credentials: LoginRequest): Promise<AuthResponse> {
    const { data } = await api.post('/identity/auth/login', credentials);

    localStorage.setItem('accessToken', data.accessToken);
    localStorage.setItem('refreshToken', data.refreshToken);

    return data;
  },

  async getSSOConfig() {
    const { data } = await api.get('/identity/auth/sso-config');
    return data;
  },

  async handleSSOCallback(code: string, redirectUri: string): Promise<AuthResponse> {
    const { data } = await api.post('/identity/auth/sso-callback', {
      code,
      redirectUri,
    });

    localStorage.setItem('accessToken', data.accessToken);
    localStorage.setItem('refreshToken', data.refreshToken);

    return data;
  },

  async logout(): Promise<void> {
    try {
      await api.post('/identity/auth/logout');
    } finally {
      localStorage.removeItem('accessToken');
      localStorage.removeItem('refreshToken');
    }
  },

  async getCurrentUser(): Promise<User> {
    const { data } = await api.get('/identity/users/me');
    return data;
  },

  isAuthenticated(): boolean {
    return !!localStorage.getItem('accessToken');
  },
};
```

### Content Service

```typescript
// services/content.service.ts
import api from './client';

export const contentService = {
  async getArticles(params?: {
    page?: number;
    pageSize?: number;
    category?: string;
    search?: string;
  }) {
    const { data } = await api.get('/content/articles', { params });
    return data;
  },

  async getFeaturedArticles(limit = 5) {
    const { data } = await api.get('/content/articles/featured', {
      params: { limit },
    });
    return data;
  },

  async getArticle(id: string) {
    const { data } = await api.get(`/content/articles/${id}`);
    return data;
  },

  async getArticleBySlug(slug: string) {
    const { data } = await api.get(`/content/articles/by-slug/${slug}`);
    return data;
  },

  async createArticle(article: CreateArticleRequest) {
    const { data } = await api.post('/content/articles', article);
    return data;
  },

  async updateArticle(id: string, article: UpdateArticleRequest) {
    const { data } = await api.put(`/content/articles/${id}`, article);
    return data;
  },

  async publishArticle(id: string, options?: { publishAt?: string }) {
    const { data } = await api.post(`/content/articles/${id}/publish`, options);
    return data;
  },
};
```

### React Query Integration

```typescript
// hooks/useArticles.ts
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { contentService } from '../services/content.service';

export function useArticles(params?: ArticleParams) {
  return useQuery({
    queryKey: ['articles', params],
    queryFn: () => contentService.getArticles(params),
  });
}

export function useFeaturedArticles(limit = 5) {
  return useQuery({
    queryKey: ['articles', 'featured', limit],
    queryFn: () => contentService.getFeaturedArticles(limit),
  });
}

export function useArticle(id: string) {
  return useQuery({
    queryKey: ['article', id],
    queryFn: () => contentService.getArticle(id),
    enabled: !!id,
  });
}

export function useCreateArticle() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: contentService.createArticle,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['articles'] });
    },
  });
}
```

### Vue Composable

```typescript
// composables/useArticles.ts
import { ref, computed } from 'vue';
import { contentService } from '../services/content.service';

export function useArticles() {
  const articles = ref<Article[]>([]);
  const loading = ref(false);
  const error = ref<Error | null>(null);
  const pagination = ref<Pagination | null>(null);

  async function fetchArticles(params?: ArticleParams) {
    loading.value = true;
    error.value = null;

    try {
      const response = await contentService.getArticles(params);
      articles.value = response.data;
      pagination.value = response.pagination;
    } catch (e) {
      error.value = e as Error;
    } finally {
      loading.value = false;
    }
  }

  return {
    articles: computed(() => articles.value),
    loading: computed(() => loading.value),
    error: computed(() => error.value),
    pagination: computed(() => pagination.value),
    fetchArticles,
  };
}
```

---

## Quick Reference

### Most Used Endpoints

| Purpose | Method | Endpoint |
|---------|--------|----------|
| Login | POST | `/api/identity/auth/login` |
| Get Current User | GET | `/api/identity/users/me` |
| Get Articles | GET | `/api/content/articles` |
| Get Documents | GET | `/api/documents` |
| Global Search | GET | `/api/search?query=...` |
| AI Chat | POST | `/api/ai/chat` |
| Get Notifications | GET | `/api/notifications` |
| Get Tasks | GET | `/api/services/my-requests` |
| Get Calendar | GET | `/api/calendar/events` |
| Get Dashboard | GET | `/api/services/dashboard` |

### Request Headers

| Header | Value | Required |
|--------|-------|----------|
| `Authorization` | `Bearer {token}` | For protected endpoints |
| `Content-Type` | `application/json` | For POST/PUT requests |
| `Accept-Language` | `en` or `ar` | For localized responses |
| `X-Request-Id` | UUID | For request tracing |

---

## Support

- **Backend Repository**: `/home/ahmedimam/Asia Cup/Asia Cup POC/backend`
- **API Port**: 5001 (Development)
- **Health Check**: `GET /health/live`

---

*Last Updated: December 2025*
*API Version: 1.0*
