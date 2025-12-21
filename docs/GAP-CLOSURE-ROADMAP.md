# AFC 2027 KMS - Gap Closure Roadmap

> **Scope**: Excludes Correspondence Management System
> **Current Coverage**: 71%
> **Target Coverage**: 95%+
> **Integration Strategy**: OCR, Digital Signatures, Meeting Management, and AI Engine provided by Intalio and 3rd party solutions

---

## Executive Summary

This roadmap outlines the implementation plan to close the gaps between the current POC/MVP and the full RFP requirements. The plan leverages external solutions for core capabilities:

| Capability | Provider | Integration Type |
|------------|----------|------------------|
| OCR Processing | Intalio/3rd Party | API Integration |
| Digital Signatures | Intalio/3rd Party | API Integration |
| Meeting Management | Intalio/3rd Party | API + Webhook Integration |
| AI Engine (NLP, NER, Sentiment, Speech) | Intalio/3rd Party | API Integration |

The KMS will focus on **integration connectors**, **data synchronization**, and **unified user experience**.

---

## Phase 1: Integration Framework & Document Processing

**Focus**: Build integration foundation, connect OCR and Digital Signature services
**Priority**: HIGH
**Effort**: Moderate (integration vs build)

### 1.1 Integration Framework

**Purpose**: Create a robust foundation for all external integrations

**Implementation Tasks**:
- [ ] Create `IExternalServiceClient` base interface
- [ ] Implement retry policies with Polly
- [ ] Add circuit breaker pattern for resilience
- [ ] Create integration health monitoring
- [ ] Implement API key/OAuth management for external services
- [ ] Add request/response logging for debugging
- [ ] Create integration configuration management

**Files to Create**:
```
backend/src/AFC27.KMS.WebApi/
├── Integration/
│   ├── Core/
│   │   ├── IExternalServiceClient.cs
│   │   ├── ExternalServiceClientBase.cs
│   │   ├── IntegrationHealthMonitor.cs
│   │   ├── RetryPolicyFactory.cs
│   │   └── CircuitBreakerConfig.cs
│   ├── Configuration/
│   │   ├── IntegrationSettings.cs
│   │   ├── OcrServiceSettings.cs
│   │   ├── SignatureServiceSettings.cs
│   │   ├── MeetingServiceSettings.cs
│   │   └── AiEngineSettings.cs
│   └── Middleware/
│       └── IntegrationLoggingMiddleware.cs
```

**Configuration (appsettings.json)**:
```json
{
  "ExternalServices": {
    "Ocr": {
      "BaseUrl": "https://ocr-service.intalio.com/api",
      "ApiKey": "{{OCR_API_KEY}}",
      "TimeoutSeconds": 120,
      "RetryCount": 3
    },
    "DigitalSignature": {
      "BaseUrl": "https://signature-service.intalio.com/api",
      "ApiKey": "{{SIGNATURE_API_KEY}}",
      "TimeoutSeconds": 60
    },
    "MeetingManagement": {
      "BaseUrl": "https://meetings-service.intalio.com/api",
      "ApiKey": "{{MEETINGS_API_KEY}}",
      "WebhookSecret": "{{MEETINGS_WEBHOOK_SECRET}}"
    },
    "AiEngine": {
      "BaseUrl": "https://ai-engine.intalio.com/api",
      "ApiKey": "{{AI_API_KEY}}",
      "TimeoutSeconds": 180
    }
  }
}
```

---

### 1.2 OCR Service Integration

**External Provider**: Intalio/3rd Party OCR Service
**Integration Type**: REST API

**Implementation Tasks**:
- [ ] Create `IOcrIntegrationService` interface
- [ ] Implement OCR service client
- [ ] Create document upload to external OCR
- [ ] Handle async OCR processing (polling/webhook)
- [ ] Store OCR results in KMS database
- [ ] Map external OCR response to KMS document model
- [ ] Frontend: OCR trigger and status display

**Expected External API Contract**:
```
POST   /ocr/documents          - Submit document for OCR
GET    /ocr/documents/{jobId}  - Get OCR job status
GET    /ocr/documents/{jobId}/result - Get OCR results
DELETE /ocr/documents/{jobId}  - Cancel OCR job
```

**KMS Integration Endpoints**:
```
POST   /api/documents/{id}/ocr/process    - Trigger OCR via external service
GET    /api/documents/{id}/ocr/status     - Get OCR status
GET    /api/documents/{id}/ocr/content    - Get extracted content
POST   /api/webhooks/ocr                  - Receive OCR completion webhook
```

**Files to Create**:
```
backend/src/AFC27.KMS.WebApi/
├── Integration/Ocr/
│   ├── IOcrIntegrationService.cs
│   ├── OcrIntegrationService.cs
│   ├── Models/
│   │   ├── OcrJobRequest.cs
│   │   ├── OcrJobResponse.cs
│   │   ├── OcrResultResponse.cs
│   │   └── OcrWebhookPayload.cs
│   └── Mappers/
│       └── OcrResultMapper.cs
├── Controllers/OcrIntegrationController.cs
├── Webhooks/OcrWebhookController.cs
└── Models/Entities/DocumentOcrResult.cs

frontend/src/
├── services/ocrIntegrationService.ts
└── components/documents/OcrProcessing.vue
```

**Data Model**:
```csharp
public class DocumentOcrResult
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public string ExternalJobId { get; set; }
    public OcrStatus Status { get; set; }  // Pending, Processing, Completed, Failed
    public string ExtractedText { get; set; }
    public string ExtractedMetadata { get; set; }  // JSON
    public double Confidence { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string ErrorMessage { get; set; }
}
```

---

### 1.3 Digital Signature Service Integration

**External Provider**: Intalio/3rd Party Signature Service
**Integration Type**: REST API + Redirect Flow

**Implementation Tasks**:
- [ ] Create `ISignatureIntegrationService` interface
- [ ] Implement signature service client
- [ ] Create signature request workflow
- [ ] Handle redirect-based signing flow
- [ ] Store signature status and certificates
- [ ] Implement signature verification via external service
- [ ] Frontend: Signature request UI, redirect handling

**Expected External API Contract**:
```
POST   /signatures/requests           - Create signature request
GET    /signatures/requests/{id}      - Get request status
GET    /signatures/requests/{id}/url  - Get signing URL (redirect)
POST   /signatures/verify             - Verify signature
DELETE /signatures/requests/{id}      - Cancel request
```

**KMS Integration Endpoints**:
```
POST   /api/documents/{id}/signature/request   - Request signature
GET    /api/documents/{id}/signature/status    - Get signature status
GET    /api/documents/{id}/signature/url       - Get signing redirect URL
POST   /api/documents/{id}/signature/verify    - Verify document signature
POST   /api/webhooks/signature                 - Receive signature completion
```

**Files to Create**:
```
backend/src/AFC27.KMS.WebApi/
├── Integration/Signature/
│   ├── ISignatureIntegrationService.cs
│   ├── SignatureIntegrationService.cs
│   ├── Models/
│   │   ├── SignatureRequest.cs
│   │   ├── SignatureResponse.cs
│   │   ├── SignatureVerificationResult.cs
│   │   └── SignatureWebhookPayload.cs
│   └── Mappers/
│       └── SignatureMapper.cs
├── Controllers/SignatureIntegrationController.cs
├── Webhooks/SignatureWebhookController.cs
└── Models/Entities/DocumentSignature.cs

frontend/src/
├── services/signatureIntegrationService.ts
├── components/signatures/
│   ├── SignatureRequest.vue
│   ├── SignatureRedirect.vue
│   └── SignatureVerification.vue
└── views/documents/SigningCallback.vue
```

**Signing Flow**:
```
┌─────────────┐     ┌─────────────┐     ┌──────────────────┐
│   KMS UI    │────▶│   KMS API   │────▶│ Signature Service│
└─────────────┘     └─────────────┘     └──────────────────┘
       │                                         │
       │◀─────── Redirect URL ───────────────────┤
       │                                         │
       ▼                                         │
┌─────────────────────────┐                      │
│ External Signing Portal │                      │
└─────────────────────────┘                      │
       │                                         │
       │──────── Callback + Webhook ────────────▶│
       │                                         │
       ▼                                         ▼
┌─────────────┐     ┌─────────────┐     ┌──────────────────┐
│   KMS UI    │◀────│   KMS API   │◀────│ Webhook Received │
└─────────────┘     └─────────────┘     └──────────────────┘
```

---

### 1.4 Document Templates Enhancement

**Implementation Tasks** (Internal - No external integration):
- [ ] Add template variables/placeholders system
- [ ] Create template categories
- [ ] Implement template versioning
- [ ] Add template preview functionality
- [ ] Create "Generate from Template" workflow
- [ ] Frontend: Template designer with drag-drop fields

**Files to Modify**:
```
backend/src/AFC27.KMS.WebApi/
├── Models/Entities/DocumentTemplate.cs (enhance)
├── Services/Templates/TemplateEngine.cs
└── Controllers/TemplatesController.cs (enhance)

frontend/src/
├── views/documents/TemplateDesigner.vue
└── components/templates/
    ├── TemplatePreview.vue
    └── TemplateGenerator.vue
```

---

### 1.5 Barcode/QR Code Support

**Implementation Tasks** (Internal - lightweight):
- [ ] Generate unique QR codes for each document
- [ ] Create QR code lookup API
- [ ] Frontend: QR code display and scanner

**Files to Create**:
```
backend/src/AFC27.KMS.WebApi/
├── Services/Barcodes/
│   ├── IBarcodeService.cs
│   └── QrCodeService.cs
└── Controllers/BarcodeController.cs

frontend/src/
└── components/documents/QrCodeDisplay.vue
```

---

## Phase 2: Meeting Management Integration

**Focus**: Integrate with external Meeting Management solution
**Priority**: HIGH
**Effort**: Moderate

### 2.1 Meeting Service Integration

**External Provider**: Intalio/3rd Party Meeting Management Service
**Integration Type**: REST API + Webhooks + Calendar Sync

**Integration Strategy**:
The KMS will integrate with the external meeting service to:
- Sync meeting data bidirectionally
- Display meeting information in KMS context
- Link documents to external meetings
- Receive meeting updates via webhooks

**Expected External API Contract**:
```
# Meetings
GET    /meetings                      - List meetings
POST   /meetings                      - Create meeting
GET    /meetings/{id}                 - Get meeting details
PUT    /meetings/{id}                 - Update meeting
DELETE /meetings/{id}                 - Cancel meeting

# Attendees
GET    /meetings/{id}/attendees       - List attendees
POST   /meetings/{id}/attendees       - Add attendee
DELETE /meetings/{id}/attendees/{aid} - Remove attendee

# Agenda & Minutes
GET    /meetings/{id}/agenda          - Get agenda
PUT    /meetings/{id}/agenda          - Update agenda
GET    /meetings/{id}/minutes         - Get minutes
PUT    /meetings/{id}/minutes         - Update minutes

# Actions & Decisions
GET    /meetings/{id}/actions         - List action items
POST   /meetings/{id}/actions         - Create action item
GET    /meetings/{id}/decisions       - List decisions

# Calendar Integration (handled by external service)
POST   /meetings/{id}/calendar/sync   - Sync to calendars
GET    /meetings/{id}/teams-link      - Get Teams meeting link

# Webhooks
POST   /webhooks/subscribe            - Subscribe to events
```

**KMS Integration Endpoints**:
```
# Sync & Display
GET    /api/meetings/external                     - List from external service
GET    /api/meetings/external/{id}                - Get meeting details
POST   /api/meetings/external/sync                - Trigger full sync

# Document Linking
POST   /api/meetings/external/{id}/documents      - Link KMS documents
GET    /api/meetings/external/{id}/documents      - Get linked documents
DELETE /api/meetings/external/{id}/documents/{did} - Unlink document

# Webhooks
POST   /api/webhooks/meetings                     - Receive meeting events
```

**Implementation Tasks**:
- [ ] Create `IMeetingIntegrationService` interface
- [ ] Implement meeting service client
- [ ] Create meeting sync job (scheduled)
- [ ] Build webhook handler for meeting events
- [ ] Implement document-to-meeting linking
- [ ] Cache meeting data locally for performance
- [ ] Frontend: Meeting list with external data, document linking UI

**Files to Create**:
```
backend/src/AFC27.KMS.WebApi/
├── Integration/Meetings/
│   ├── IMeetingIntegrationService.cs
│   ├── MeetingIntegrationService.cs
│   ├── MeetingSyncJob.cs
│   ├── Models/
│   │   ├── ExternalMeeting.cs
│   │   ├── ExternalAttendee.cs
│   │   ├── ExternalAgendaItem.cs
│   │   ├── ExternalActionItem.cs
│   │   ├── MeetingWebhookPayload.cs
│   │   └── MeetingSyncResult.cs
│   └── Mappers/
│       └── MeetingMapper.cs
├── Controllers/MeetingIntegrationController.cs
├── Webhooks/MeetingWebhookController.cs
└── Models/Entities/
    ├── ExternalMeetingLink.cs
    └── MeetingDocumentLink.cs

frontend/src/
├── services/meetingIntegrationService.ts
├── views/meetings/ExternalMeetingsView.vue
└── components/meetings/
    ├── ExternalMeetingCard.vue
    ├── MeetingDocumentLinker.vue
    └── MeetingSyncStatus.vue
```

**Data Model for Linking**:
```csharp
public class ExternalMeetingLink
{
    public Guid Id { get; set; }
    public string ExternalMeetingId { get; set; }
    public string ExternalMeetingTitle { get; set; }
    public DateTime MeetingDate { get; set; }
    public string ExternalSystemUrl { get; set; }
    public DateTime LastSyncedAt { get; set; }
    public string CachedData { get; set; }  // JSON cache
}

public class MeetingDocumentLink
{
    public Guid Id { get; set; }
    public Guid ExternalMeetingLinkId { get; set; }
    public Guid DocumentId { get; set; }
    public string LinkType { get; set; }  // Agenda, Minutes, Reference, Attachment
    public DateTime LinkedAt { get; set; }
    public Guid LinkedById { get; set; }
}
```

**Webhook Events to Handle**:
- `meeting.created` - New meeting created
- `meeting.updated` - Meeting details changed
- `meeting.cancelled` - Meeting cancelled
- `meeting.completed` - Meeting finished
- `attendee.added` - New attendee
- `attendee.responded` - Attendee RSVP
- `action.created` - New action item
- `action.completed` - Action item done
- `decision.made` - Decision recorded

---

### 2.2 Calendar Display in KMS

**Implementation Tasks**:
- [ ] Create calendar view showing external meetings
- [ ] Display meeting details from external service
- [ ] Show related KMS documents per meeting
- [ ] Add quick-link to external meeting system

**Files to Create**:
```
frontend/src/
├── views/meetings/CalendarView.vue
└── components/meetings/
    ├── MeetingCalendar.vue
    └── MeetingDetailPanel.vue
```

---

## Phase 3: AI Engine Integration

**Focus**: Integrate with external AI Engine for NLP, NER, Sentiment, Speech-to-Text
**Priority**: MEDIUM-HIGH
**Effort**: Moderate

### 3.1 AI Engine Integration Service

**External Provider**: Intalio/3rd Party AI Engine
**Integration Type**: REST API (sync and async)

**Expected External AI Engine Capabilities**:
- Natural Language Processing (NLP)
- Named Entity Recognition (NER)
- Sentiment Analysis
- Text Summarization
- Auto-Tagging/Classification
- Speech-to-Text Transcription
- Semantic Search Enhancement

**Expected External API Contract**:
```
# NLP & Text Analysis
POST   /ai/analyze                    - Full text analysis
POST   /ai/entities                   - Extract named entities
POST   /ai/sentiment                  - Analyze sentiment
POST   /ai/summarize                  - Summarize text
POST   /ai/classify                   - Classify/categorize text
POST   /ai/tags/suggest               - Suggest tags

# Speech Processing
POST   /ai/speech/transcribe          - Submit audio for transcription
GET    /ai/speech/transcribe/{jobId}  - Get transcription status
GET    /ai/speech/transcribe/{jobId}/result - Get transcript

# Search Enhancement
POST   /ai/search/semantic            - Semantic search
POST   /ai/search/similar             - Find similar content

# Embeddings (for RAG)
POST   /ai/embeddings                 - Generate embeddings
```

**KMS Integration Endpoints**:
```
# Document Analysis
POST   /api/ai/documents/{id}/analyze      - Analyze document
GET    /api/ai/documents/{id}/entities     - Get extracted entities
GET    /api/ai/documents/{id}/sentiment    - Get sentiment
POST   /api/ai/documents/{id}/summarize    - Generate summary
POST   /api/ai/documents/{id}/tags/suggest - Get tag suggestions

# Article Analysis
POST   /api/ai/articles/{id}/analyze       - Analyze article
POST   /api/ai/articles/{id}/summarize     - Summarize article

# Search Enhancement
POST   /api/ai/search/semantic             - Semantic search via AI engine

# Transcription
POST   /api/ai/transcribe                  - Submit audio
GET    /api/ai/transcribe/{jobId}          - Get status
POST   /api/webhooks/ai/transcription      - Transcription complete webhook

# Batch Processing
POST   /api/ai/batch/analyze               - Batch analyze documents
GET    /api/ai/batch/{batchId}/status      - Batch status
```

**Implementation Tasks**:
- [ ] Create `IAiEngineIntegrationService` interface
- [ ] Implement AI engine client
- [ ] Create document analysis workflow
- [ ] Integrate with existing chatbot (enhance RAG)
- [ ] Build auto-tagging using external AI
- [ ] Implement sentiment tracking
- [ ] Create entity extraction and linking
- [ ] Add transcription support for meetings
- [ ] Frontend: AI insights panel, entity viewer, sentiment display

**Files to Create**:
```
backend/src/AFC27.KMS.WebApi/
├── Integration/AiEngine/
│   ├── IAiEngineIntegrationService.cs
│   ├── AiEngineIntegrationService.cs
│   ├── Models/
│   │   ├── AnalysisRequest.cs
│   │   ├── AnalysisResponse.cs
│   │   ├── EntityExtractionResult.cs
│   │   ├── SentimentResult.cs
│   │   ├── SummarizationResult.cs
│   │   ├── TagSuggestionResult.cs
│   │   ├── TranscriptionRequest.cs
│   │   ├── TranscriptionResult.cs
│   │   └── SemanticSearchResult.cs
│   └── Mappers/
│       └── AiResultMapper.cs
├── Controllers/AiIntegrationController.cs
├── Webhooks/AiWebhookController.cs
└── Models/Entities/
    ├── DocumentAnalysis.cs
    ├── ExtractedEntity.cs
    └── ContentSentiment.cs

frontend/src/
├── services/aiIntegrationService.ts
├── views/ai/
│   ├── DocumentInsights.vue
│   └── EntityExplorer.vue
└── components/ai/
    ├── AiAnalysisPanel.vue
    ├── EntityHighlighter.vue
    ├── SentimentIndicator.vue
    ├── TagSuggestions.vue
    └── TranscriptionViewer.vue
```

**Data Models**:
```csharp
public class DocumentAnalysis
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public string Summary { get; set; }
    public string Language { get; set; }
    public double SentimentScore { get; set; }  // -1 to 1
    public string SentimentLabel { get; set; }  // Positive, Neutral, Negative
    public string[] SuggestedTags { get; set; }
    public string[] KeyPhrases { get; set; }
    public DateTime AnalyzedAt { get; set; }
    public string RawResponse { get; set; }  // JSON from external service
}

public class ExtractedEntity
{
    public Guid Id { get; set; }
    public Guid? DocumentId { get; set; }
    public Guid? ArticleId { get; set; }
    public string EntityType { get; set; }  // Person, Organization, Location, Date, Event
    public string EntityValue { get; set; }
    public string NormalizedValue { get; set; }
    public double Confidence { get; set; }
    public int StartOffset { get; set; }
    public int EndOffset { get; set; }
}
```

---

### 3.2 Enhanced Chatbot with External AI

**Implementation Tasks**:
- [ ] Integrate external AI for better embeddings
- [ ] Use external semantic search for RAG
- [ ] Enhance response generation with external AI
- [ ] Add entity-aware responses

**Files to Modify**:
```
backend/src/AFC27.KMS.WebApi/
├── Services/AI/
│   ├── ChatbotService.cs (enhance to use external AI)
│   └── RagService.cs (enhance with external embeddings)
```

---

### 3.3 Auto-Tagging Enhancement

**Implementation Tasks**:
- [ ] Replace/enhance internal tagging with external AI
- [ ] Add confidence scores to suggestions
- [ ] Create bulk auto-tagging job
- [ ] Add user feedback loop for tag accuracy

**Files to Modify**:
```
backend/src/AFC27.KMS.WebApi/
├── Services/AI/Tagging/
│   └── AutoTagService.cs (use external AI)
└── Controllers/TaggingController.cs (enhance)
```

---

## Phase 4: Infrastructure & Compliance

**Focus**: Security, Compliance, Performance
**Priority**: MEDIUM
**Effort**: Moderate

### 4.1 Full SSO Implementation

**Implementation Tasks**:
- [ ] Complete Azure AD B2C integration
- [ ] Implement SAML 2.0 support
- [ ] Add MFA enforcement
- [ ] Create SSO session management
- [ ] Implement single logout (SLO)

**Files to Modify**:
```
backend/src/AFC27.KMS.WebApi/
├── Configuration/AuthenticationConfig.cs
└── Services/Auth/SsoService.cs
```

---

### 4.2 Data Encryption Enhancement

**Implementation Tasks**:
- [ ] Implement field-level encryption
- [ ] Add encryption key rotation
- [ ] Create encrypted storage option

**Files to Create**:
```
backend/src/AFC27.KMS.WebApi/
└── Services/Security/
    ├── IEncryptionService.cs
    └── EncryptionService.cs
```

---

### 4.3 Disaster Recovery Configuration

**Implementation Tasks**:
- [ ] Configure database backup automation
- [ ] Set up geo-redundant storage
- [ ] Create DR runbook

**Deliverables**:
```
docs/
├── DISASTER-RECOVERY-PLAN.md
└── BACKUP-PROCEDURES.md

infrastructure/
└── backup-automation/
    └── backup-script.ps1
```

---

### 4.4 Load Balancing & Scaling

**Implementation Tasks**:
- [ ] Configure load balancer
- [ ] Set up auto-scaling rules
- [ ] Implement rate limiting

**Deliverables**:
```
infrastructure/
├── kubernetes/
│   ├── deployment.yaml
│   └── service.yaml
└── terraform/
    └── load-balancer.tf
```

---

### 4.5 NCA Cybersecurity Compliance

**Implementation Tasks**:
- [ ] Complete security controls assessment
- [ ] Add data classification labels
- [ ] Create compliance reporting

**Deliverables**:
```
docs/compliance/
├── NCA-COMPLIANCE-CHECKLIST.md
└── SECURITY-CONTROLS-MATRIX.md
```

---

## Phase 5: Knowledge Engagement & Quality Assurance

**Focus**: Document QA, KPIs, Self-Service, Polling/Voting, Learning Paths
**Priority**: MEDIUM-HIGH
**Effort**: Significant
**Source**: FMA Document - LOC Document Control and Knowledge Center Initiative

This phase implements the operational requirements from the FMA document to establish a fully functional Knowledge Management Center (KMC) aligned with ISO 30401:2018.

---

### 5.1 Document Quality Assurance System

**FMA Requirement**: "The Administration Department conducts quality assurance checks on the received documents in accordance with document control policies" and "Conduct a weekly inspection on the documents to ensure compliance"

**Features**:
- Document compliance checking against control policies
- Weekly automated inspection workflows
- QA review assignments and tracking
- Compliance issue flagging and remediation
- QA audit trail and history

**Implementation Tasks**:
- [ ] Create `DocumentQualityCheck` entity
- [ ] Implement QA checklist templates per document type
- [ ] Build automated compliance validation rules
- [ ] Create QA reviewer assignment workflow
- [ ] Implement inspection scheduling (weekly)
- [ ] Add compliance issue tracking
- [ ] Create QA dashboard with compliance metrics
- [ ] Frontend: QA review interface, checklist forms

**API Endpoints**:
```
# QA Checklists
GET    /api/qa/checklists                      - List QA checklists
POST   /api/qa/checklists                      - Create checklist template
GET    /api/qa/checklists/{id}                 - Get checklist details

# Document QA Reviews
POST   /api/documents/{id}/qa/review           - Start QA review
GET    /api/documents/{id}/qa/reviews          - Get review history
PUT    /api/documents/{id}/qa/reviews/{rid}    - Update review
POST   /api/documents/{id}/qa/approve          - Approve document
POST   /api/documents/{id}/qa/reject           - Reject with issues

# Inspections
GET    /api/qa/inspections                     - List scheduled inspections
POST   /api/qa/inspections                     - Schedule inspection
GET    /api/qa/inspections/{id}                - Get inspection details
PUT    /api/qa/inspections/{id}/complete       - Complete inspection

# Compliance Issues
GET    /api/qa/issues                          - List compliance issues
POST   /api/qa/issues                          - Report issue
PUT    /api/qa/issues/{id}/resolve             - Resolve issue

# QA Dashboard
GET    /api/qa/dashboard                       - Get QA metrics
GET    /api/qa/reports/compliance              - Compliance report
```

**Files to Create**:
```
backend/src/AFC27.KMS.WebApi/
├── Models/Entities/QualityAssurance/
│   ├── QaChecklist.cs
│   ├── QaChecklistItem.cs
│   ├── DocumentQaReview.cs
│   ├── QaReviewItem.cs
│   ├── QaInspection.cs
│   ├── QaInspectionDocument.cs
│   └── ComplianceIssue.cs
├── Services/QualityAssurance/
│   ├── IQaService.cs
│   ├── QaService.cs
│   ├── IComplianceValidator.cs
│   ├── ComplianceValidator.cs
│   ├── IInspectionScheduler.cs
│   └── InspectionScheduler.cs
├── Controllers/QualityAssuranceController.cs
└── Jobs/
    └── WeeklyInspectionJob.cs

frontend/src/
├── views/qa/
│   ├── QaDashboard.vue
│   ├── QaReviewView.vue
│   ├── InspectionListView.vue
│   └── ComplianceIssuesView.vue
├── components/qa/
│   ├── QaChecklistForm.vue
│   ├── QaReviewPanel.vue
│   ├── ComplianceStatusBadge.vue
│   ├── InspectionCalendar.vue
│   └── IssueTracker.vue
└── services/qaService.ts
```

**Data Models**:
```csharp
public class DocumentQaReview
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public Guid ChecklistId { get; set; }
    public Guid ReviewerId { get; set; }
    public QaReviewStatus Status { get; set; }  // Pending, InProgress, Approved, Rejected
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string Comments { get; set; }
    public int ComplianceScore { get; set; }  // 0-100
    public List<QaReviewItem> Items { get; set; }
}

public class QaInspection
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public Guid DepartmentId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public Guid InspectorId { get; set; }
    public InspectionStatus Status { get; set; }  // Scheduled, InProgress, Completed
    public int TotalDocuments { get; set; }
    public int CompliantDocuments { get; set; }
    public int NonCompliantDocuments { get; set; }
    public string Summary { get; set; }
}

public class ComplianceIssue
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public Guid? InspectionId { get; set; }
    public string IssueType { get; set; }  // MissingMetadata, InvalidFormat, Outdated, etc.
    public string Description { get; set; }
    public IssueSeverity Severity { get; set; }  // Low, Medium, High, Critical
    public IssueStatus Status { get; set; }  // Open, InProgress, Resolved
    public Guid ReportedById { get; set; }
    public Guid? AssignedToId { get; set; }
    public DateTime ReportedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
}
```

---

### 5.2 KPI Management System (Personal & Team)

**FMA Requirement**: "The Administration Department generates a KPI report illustrating adherence to document control policies, including metrics such as the number of submissions and compliance rates" and "Track KMC effectiveness through metrics and KPIs"

**Features**:
- Personal KPIs for individual contributors
- Team/Department KPIs for group performance
- Document submission tracking
- Compliance rate metrics
- Knowledge contribution scoring
- Bi-weekly performance reports
- KPI dashboards and visualizations

**Implementation Tasks**:
- [ ] Create KPI definition entities
- [ ] Implement personal KPI tracking
- [ ] Implement team/department KPI aggregation
- [ ] Build KPI calculation engine
- [ ] Create bi-weekly report generation
- [ ] Implement KPI targets and thresholds
- [ ] Add trend analysis and comparisons
- [ ] Create KPI notification system (alerts for low performance)
- [ ] Frontend: Personal dashboard, team dashboard, KPI reports

**API Endpoints**:
```
# KPI Definitions
GET    /api/kpis/definitions                   - List KPI definitions
POST   /api/kpis/definitions                   - Create KPI definition
PUT    /api/kpis/definitions/{id}              - Update definition
DELETE /api/kpis/definitions/{id}              - Delete definition

# Personal KPIs
GET    /api/kpis/personal                      - Get my KPIs
GET    /api/kpis/personal/{userId}             - Get user's KPIs (admin)
GET    /api/kpis/personal/history              - Get my KPI history

# Team KPIs
GET    /api/kpis/team/{departmentId}           - Get department KPIs
GET    /api/kpis/team/{departmentId}/members   - Get team members' KPIs

# KPI Reports
GET    /api/kpis/reports/summary               - Get KPI summary report
GET    /api/kpis/reports/compliance            - Compliance rate report
GET    /api/kpis/reports/contributions         - Contribution report
POST   /api/kpis/reports/generate              - Generate bi-weekly report
GET    /api/kpis/reports/{reportId}            - Get generated report

# KPI Targets
GET    /api/kpis/targets                       - List KPI targets
POST   /api/kpis/targets                       - Set KPI target
PUT    /api/kpis/targets/{id}                  - Update target

# Leaderboards
GET    /api/kpis/leaderboard/personal          - Personal leaderboard
GET    /api/kpis/leaderboard/teams             - Team leaderboard
```

**Files to Create**:
```
backend/src/AFC27.KMS.WebApi/
├── Models/Entities/KPI/
│   ├── KpiDefinition.cs
│   ├── KpiValue.cs
│   ├── PersonalKpi.cs
│   ├── TeamKpi.cs
│   ├── KpiTarget.cs
│   ├── KpiReport.cs
│   └── KpiSnapshot.cs
├── Services/KPI/
│   ├── IKpiService.cs
│   ├── KpiService.cs
│   ├── IKpiCalculator.cs
│   ├── KpiCalculator.cs
│   ├── IKpiReportGenerator.cs
│   └── KpiReportGenerator.cs
├── Controllers/KpiController.cs
└── Jobs/
    └── BiWeeklyKpiReportJob.cs

frontend/src/
├── views/kpi/
│   ├── PersonalKpiDashboard.vue
│   ├── TeamKpiDashboard.vue
│   ├── KpiReportsView.vue
│   └── LeaderboardView.vue
├── components/kpi/
│   ├── KpiCard.vue
│   ├── KpiChart.vue
│   ├── KpiTrend.vue
│   ├── KpiTargetIndicator.vue
│   ├── ComplianceGauge.vue
│   └── ContributionStats.vue
└── services/kpiService.ts
```

**KPI Types**:
```csharp
public enum KpiType
{
    // Document KPIs
    DocumentsSubmitted,
    DocumentsOnTime,
    DocumentComplianceRate,
    DocumentQualityScore,

    // Knowledge KPIs
    ArticlesContributed,
    ArticlesPublished,
    KnowledgeShared,
    ExpertiseContributions,

    // Engagement KPIs
    CollaborationSessions,
    FeedbackProvided,
    TrainingCompleted,
    QuizzesPassed,

    // Quality KPIs
    QaReviewsCompleted,
    IssuesResolved,
    ComplianceAchieved
}

public class KpiDefinition
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string NameAr { get; set; }
    public string Description { get; set; }
    public KpiType Type { get; set; }
    public KpiScope Scope { get; set; }  // Personal, Team, Department, Organization
    public string CalculationFormula { get; set; }
    public string Unit { get; set; }  // Count, Percentage, Score
    public decimal DefaultTarget { get; set; }
    public bool IsActive { get; set; }
}

public class PersonalKpi
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid KpiDefinitionId { get; set; }
    public decimal CurrentValue { get; set; }
    public decimal TargetValue { get; set; }
    public decimal PreviousValue { get; set; }
    public decimal TrendPercentage { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public DateTime CalculatedAt { get; set; }
}
```

---

### 5.3 Self-Service Portal & Service Catalog

**FMA Requirement**: "Digitize and automate common internal requests (HR, IT, Finance) through a user-friendly Service Catalog" and "Streamline access to information and internal services"

**Features**:
- Service Catalog with categorized services
- Request submission forms
- Request tracking and status updates
- Automated workflows for common requests
- SLA tracking
- Request history and analytics
- Bilingual support (Arabic/English)

**Implementation Tasks**:
- [ ] Create service catalog entities
- [ ] Build service request workflow engine
- [ ] Implement request form builder
- [ ] Create approval workflows
- [ ] Add SLA tracking and alerts
- [ ] Implement request routing rules
- [ ] Create self-service dashboard
- [ ] Frontend: Service catalog browser, request forms, tracking

**API Endpoints**:
```
# Service Catalog
GET    /api/services/catalog                   - Browse service catalog
GET    /api/services/catalog/{categoryId}      - Get services by category
GET    /api/services/{id}                      - Get service details
POST   /api/services                           - Create service (admin)
PUT    /api/services/{id}                      - Update service

# Service Categories
GET    /api/services/categories                - List categories
POST   /api/services/categories                - Create category

# Service Requests
POST   /api/services/{id}/request              - Submit request
GET    /api/services/requests                  - List my requests
GET    /api/services/requests/{id}             - Get request details
PUT    /api/services/requests/{id}/cancel      - Cancel request

# Request Processing (Admin/Fulfiller)
GET    /api/services/requests/pending          - Get pending requests
PUT    /api/services/requests/{id}/assign      - Assign request
PUT    /api/services/requests/{id}/approve     - Approve request
PUT    /api/services/requests/{id}/reject      - Reject request
PUT    /api/services/requests/{id}/complete    - Complete request

# SLA
GET    /api/services/sla/report                - SLA compliance report
GET    /api/services/sla/breaches              - SLA breaches
```

**Files to Create**:
```
backend/src/AFC27.KMS.WebApi/
├── Models/Entities/ServiceCatalog/
│   ├── ServiceCategory.cs
│   ├── Service.cs
│   ├── ServiceField.cs
│   ├── ServiceRequest.cs
│   ├── ServiceRequestField.cs
│   ├── ServiceSla.cs
│   └── ServiceApproval.cs
├── Services/ServiceCatalog/
│   ├── IServiceCatalogService.cs
│   ├── ServiceCatalogService.cs
│   ├── IServiceRequestService.cs
│   ├── ServiceRequestService.cs
│   ├── ISlaService.cs
│   └── SlaService.cs
├── Controllers/ServiceCatalogController.cs
└── Jobs/
    └── SlaMonitoringJob.cs

frontend/src/
├── views/services/
│   ├── ServiceCatalogView.vue
│   ├── ServiceDetailView.vue
│   ├── MyRequestsView.vue
│   ├── RequestDetailView.vue
│   └── ServiceAdminView.vue
├── components/services/
│   ├── ServiceCard.vue
│   ├── ServiceCategoryTree.vue
│   ├── ServiceRequestForm.vue
│   ├── RequestStatusTracker.vue
│   ├── SlaIndicator.vue
│   └── RequestTimeline.vue
└── services/serviceCatalogService.ts
```

**Service Categories**:
```csharp
public class ServiceCategory
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string NameAr { get; set; }
    public string Icon { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public int SortOrder { get; set; }
}

// Pre-defined categories from FMA:
// - HR Services (Leave, Benefits, Onboarding)
// - IT Services (Equipment, Access, Support)
// - Finance Services (Expenses, Procurement)
// - Facilities Services (Room Booking, Parking)
// - Administrative Services (Documents, Travel)
```

---

### 5.4 Polling and Voting System

**FMA Requirement**: Support for collaborative decision-making and engagement features

**Features**:
- Create polls with multiple question types
- Anonymous and named voting options
- Real-time voting results
- Poll scheduling and deadlines
- Vote delegation
- Results visualization
- Integration with meetings/decisions

**Implementation Tasks**:
- [ ] Create poll and vote entities
- [ ] Implement multiple poll types (single choice, multiple choice, ranking)
- [ ] Add anonymous voting support
- [ ] Create real-time vote counting
- [ ] Implement poll scheduling
- [ ] Add vote delegation feature
- [ ] Create results visualization charts
- [ ] Link polls to meetings/committees
- [ ] Frontend: Poll creation wizard, voting interface, results dashboard

**API Endpoints**:
```
# Polls
GET    /api/polls                              - List polls
POST   /api/polls                              - Create poll
GET    /api/polls/{id}                         - Get poll details
PUT    /api/polls/{id}                         - Update poll
DELETE /api/polls/{id}                         - Delete poll
POST   /api/polls/{id}/publish                 - Publish poll
POST   /api/polls/{id}/close                   - Close poll

# Voting
POST   /api/polls/{id}/vote                    - Submit vote
PUT    /api/polls/{id}/vote                    - Update vote (if allowed)
GET    /api/polls/{id}/my-vote                 - Get my vote
POST   /api/polls/{id}/delegate                - Delegate vote

# Results
GET    /api/polls/{id}/results                 - Get poll results
GET    /api/polls/{id}/results/export          - Export results

# Poll Templates
GET    /api/polls/templates                    - List poll templates
POST   /api/polls/templates                    - Create template
POST   /api/polls/from-template/{templateId}   - Create poll from template
```

**Files to Create**:
```
backend/src/AFC27.KMS.WebApi/
├── Models/Entities/Polling/
│   ├── Poll.cs
│   ├── PollOption.cs
│   ├── Vote.cs
│   ├── VoteDelegation.cs
│   └── PollTemplate.cs
├── Services/Polling/
│   ├── IPollService.cs
│   ├── PollService.cs
│   ├── IVoteService.cs
│   └── VoteService.cs
├── Controllers/PollController.cs
└── Hubs/
    └── PollHub.cs  (SignalR for real-time results)

frontend/src/
├── views/polls/
│   ├── PollListView.vue
│   ├── PollDetailView.vue
│   ├── CreatePollView.vue
│   └── PollResultsView.vue
├── components/polls/
│   ├── PollCard.vue
│   ├── VotingForm.vue
│   ├── PollOptionItem.vue
│   ├── ResultsChart.vue
│   ├── VoteProgress.vue
│   └── PollCountdown.vue
└── services/pollService.ts
```

**Data Models**:
```csharp
public class Poll
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string TitleAr { get; set; }
    public string Description { get; set; }
    public PollType Type { get; set; }  // SingleChoice, MultipleChoice, Ranking, YesNo
    public bool IsAnonymous { get; set; }
    public bool AllowDelegation { get; set; }
    public bool AllowChangeVote { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public PollStatus Status { get; set; }  // Draft, Active, Closed
    public Guid CreatedById { get; set; }
    public Guid? MeetingId { get; set; }  // Link to meeting
    public Guid? CommitteeId { get; set; }  // Restrict to committee members
    public List<PollOption> Options { get; set; }
}

public class Vote
{
    public Guid Id { get; set; }
    public Guid PollId { get; set; }
    public Guid? VoterId { get; set; }  // Null if anonymous
    public string VoterHash { get; set; }  // For anonymous deduplication
    public Guid SelectedOptionId { get; set; }
    public int? RankingPosition { get; set; }  // For ranking polls
    public DateTime VotedAt { get; set; }
    public Guid? DelegatedFromId { get; set; }
}
```

---

### 5.5 Learning Paths System (Courses, Certificates, Quizzes)

**FMA Requirement**: "Enhance self learning with pop up quiz/assessments", "Training & capacity-building report", and fostering a knowledge-sharing culture

**Features**:
- Learning paths with structured courses
- Course content management
- Interactive quizzes and assessments
- Certificate generation upon completion
- Progress tracking
- Pop-up quizzes for knowledge reinforcement
- Training reports and analytics
- Gamification elements (badges, points)

**Implementation Tasks**:
- [ ] Create learning path entities
- [ ] Build course content management
- [ ] Implement quiz engine with multiple question types
- [ ] Create certificate generation (PDF)
- [ ] Add progress tracking
- [ ] Implement pop-up quiz system
- [ ] Create gamification (badges, points, levels)
- [ ] Build training reports
- [ ] Frontend: Learning dashboard, course viewer, quiz interface

**API Endpoints**:
```
# Learning Paths
GET    /api/learning/paths                     - List learning paths
POST   /api/learning/paths                     - Create learning path
GET    /api/learning/paths/{id}                - Get path details
PUT    /api/learning/paths/{id}                - Update path
GET    /api/learning/paths/{id}/enroll         - Enroll in path
GET    /api/learning/paths/my                  - My enrolled paths

# Courses
GET    /api/learning/courses                   - List courses
POST   /api/learning/courses                   - Create course
GET    /api/learning/courses/{id}              - Get course details
GET    /api/learning/courses/{id}/content      - Get course content
POST   /api/learning/courses/{id}/complete     - Mark lesson complete

# Quizzes
GET    /api/learning/quizzes                   - List quizzes
POST   /api/learning/quizzes                   - Create quiz
GET    /api/learning/quizzes/{id}              - Get quiz
POST   /api/learning/quizzes/{id}/start        - Start quiz attempt
POST   /api/learning/quizzes/{id}/submit       - Submit quiz
GET    /api/learning/quizzes/{id}/results      - Get quiz results

# Pop-up Quizzes
GET    /api/learning/popup-quiz                - Get random pop-up quiz
POST   /api/learning/popup-quiz/{id}/answer    - Answer pop-up quiz

# Certificates
GET    /api/learning/certificates              - My certificates
GET    /api/learning/certificates/{id}         - Get certificate
GET    /api/learning/certificates/{id}/pdf     - Download certificate PDF
POST   /api/learning/certificates/verify       - Verify certificate

# Progress & Gamification
GET    /api/learning/progress                  - My learning progress
GET    /api/learning/badges                    - My badges
GET    /api/learning/leaderboard               - Learning leaderboard
GET    /api/learning/points                    - My points

# Reports
GET    /api/learning/reports/completion        - Completion rates
GET    /api/learning/reports/department/{id}   - Department training report
```

**Files to Create**:
```
backend/src/AFC27.KMS.WebApi/
├── Models/Entities/Learning/
│   ├── LearningPath.cs
│   ├── Course.cs
│   ├── CourseModule.cs
│   ├── Lesson.cs
│   ├── Quiz.cs
│   ├── QuizQuestion.cs
│   ├── QuizOption.cs
│   ├── QuizAttempt.cs
│   ├── QuizAnswer.cs
│   ├── Certificate.cs
│   ├── UserProgress.cs
│   ├── Badge.cs
│   ├── UserBadge.cs
│   └── LearningPoints.cs
├── Services/Learning/
│   ├── ILearningPathService.cs
│   ├── LearningPathService.cs
│   ├── ICourseService.cs
│   ├── CourseService.cs
│   ├── IQuizService.cs
│   ├── QuizService.cs
│   ├── ICertificateService.cs
│   ├── CertificateService.cs
│   ├── IProgressService.cs
│   ├── ProgressService.cs
│   ├── IGamificationService.cs
│   └── GamificationService.cs
├── Controllers/
│   ├── LearningPathController.cs
│   ├── CourseController.cs
│   ├── QuizController.cs
│   └── CertificateController.cs
└── Jobs/
    └── PopupQuizScheduler.cs

frontend/src/
├── views/learning/
│   ├── LearningDashboard.vue
│   ├── LearningPathView.vue
│   ├── CourseView.vue
│   ├── LessonView.vue
│   ├── QuizView.vue
│   ├── CertificatesView.vue
│   └── LeaderboardView.vue
├── components/learning/
│   ├── LearningPathCard.vue
│   ├── CourseProgress.vue
│   ├── LessonContent.vue
│   ├── QuizQuestion.vue
│   ├── QuizResults.vue
│   ├── PopupQuizModal.vue
│   ├── CertificateCard.vue
│   ├── BadgeDisplay.vue
│   ├── PointsCounter.vue
│   └── ProgressRing.vue
└── services/learningService.ts
```

**Data Models**:
```csharp
public class LearningPath
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string TitleAr { get; set; }
    public string Description { get; set; }
    public string ThumbnailUrl { get; set; }
    public int EstimatedHours { get; set; }
    public DifficultyLevel Difficulty { get; set; }
    public bool IsPublished { get; set; }
    public bool IsMandatory { get; set; }
    public List<Course> Courses { get; set; }
}

public class Course
{
    public Guid Id { get; set; }
    public Guid? LearningPathId { get; set; }
    public string Title { get; set; }
    public string TitleAr { get; set; }
    public string Description { get; set; }
    public int SortOrder { get; set; }
    public int EstimatedMinutes { get; set; }
    public List<CourseModule> Modules { get; set; }
}

public class Quiz
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string TitleAr { get; set; }
    public Guid? CourseId { get; set; }
    public QuizType Type { get; set; }  // Assessment, Practice, PopUp
    public int PassingScore { get; set; }  // Percentage
    public int TimeLimit { get; set; }  // Minutes, 0 = unlimited
    public bool ShuffleQuestions { get; set; }
    public bool ShowCorrectAnswers { get; set; }
    public int MaxAttempts { get; set; }  // 0 = unlimited
    public List<QuizQuestion> Questions { get; set; }
}

public class QuizQuestion
{
    public Guid Id { get; set; }
    public Guid QuizId { get; set; }
    public string QuestionText { get; set; }
    public string QuestionTextAr { get; set; }
    public QuestionType Type { get; set; }  // SingleChoice, MultipleChoice, TrueFalse, FillBlank
    public int Points { get; set; }
    public string Explanation { get; set; }
    public int SortOrder { get; set; }
    public List<QuizOption> Options { get; set; }
}

public class Certificate
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid LearningPathId { get; set; }
    public string CertificateNumber { get; set; }
    public DateTime IssuedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public int FinalScore { get; set; }
    public string PdfUrl { get; set; }
    public string VerificationCode { get; set; }
}

public class UserProgress
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid? LearningPathId { get; set; }
    public Guid? CourseId { get; set; }
    public Guid? LessonId { get; set; }
    public ProgressStatus Status { get; set; }  // NotStarted, InProgress, Completed
    public int CompletionPercentage { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int TimeSpentMinutes { get; set; }
}
```

---

## Integration Architecture Overview (Updated)

```
┌─────────────────────────────────────────────────────────────────────────────────────┐
│                                  AFC 2027 KMS                                        │
│  ┌───────────────────────────────────────────────────────────────────────────────┐  │
│  │                            Integration Layer                                   │  │
│  │  ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐                          │  │
│  │  │   OCR    │ │Signature │ │ Meeting  │ │AI Engine │                          │  │
│  │  │  Client  │ │  Client  │ │  Client  │ │  Client  │                          │  │
│  │  └────┬─────┘ └────┬─────┘ └────┬─────┘ └────┬─────┘                          │  │
│  └───────┼────────────┼────────────┼────────────┼────────────────────────────────┘  │
│          │            │            │            │                                    │
│  ┌───────┴────────────┴────────────┴────────────┴────────────────────────────────┐  │
│  │                        Core KMS Modules (Internal)                             │  │
│  │  ┌────────────┐ ┌────────────┐ ┌────────────┐ ┌────────────┐ ┌────────────┐   │  │
│  │  │    QA      │ │    KPI     │ │  Service   │ │  Polling   │ │  Learning  │   │  │
│  │  │  System    │ │  System    │ │  Catalog   │ │  & Voting  │ │   Paths    │   │  │
│  │  └────────────┘ └────────────┘ └────────────┘ └────────────┘ └────────────┘   │  │
│  └───────────────────────────────────────────────────────────────────────────────┘  │
│                                                                                      │
└──────────────────────────────────────────────────────────────────────────────────────┘
           │            │            │            │
           ▼            ▼            ▼            ▼
   ┌───────────┐ ┌───────────┐ ┌───────────┐ ┌───────────┐
   │ Intalio   │ │ Intalio   │ │ Intalio   │ │ Intalio   │
   │ OCR Svc   │ │ Sign Svc  │ │ Meet Svc  │ │ AI Engine │
   └───────────┘ └───────────┘ └───────────┘ └───────────┘
```

---

## Implementation Timeline Overview

```
┌─────────────────────────────────────────────────────────────────────┐
│                    GAP CLOSURE ROADMAP (Integration-First)          │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  PHASE 1: Integration Framework & Document Processing               │
│  ════════════════════════════════════════════════════               │
│  • Integration Framework (base clients, retry, circuit breaker)     │
│  • OCR Service Integration (Intalio/3rd Party)                      │
│  • Digital Signature Integration (Intalio/3rd Party)                │
│  • Document Templates Enhancement (internal)                        │
│  • QR Code Support (internal)                                       │
│  Coverage Impact: +8%                                               │
│                                                                     │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  PHASE 2: Meeting Management Integration                            │
│  ═══════════════════════════════════════                            │
│  • Meeting Service Integration (Intalio/3rd Party)                  │
│  • Document-Meeting Linking                                         │
│  • Calendar Display in KMS                                          │
│  • Webhook Handlers for Meeting Events                              │
│  Coverage Impact: +10%                                              │
│                                                                     │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  PHASE 3: AI Engine Integration                                     │
│  ══════════════════════════════                                     │
│  • AI Engine Integration Service (Intalio/3rd Party)                │
│  • Document Analysis (NLP, NER, Sentiment)                          │
│  • Enhanced Chatbot with External AI                                │
│  • Auto-Tagging via External AI                                     │
│  • Speech-to-Text Integration                                       │
│  Coverage Impact: +8%                                               │
│                                                                     │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  PHASE 4: Infrastructure & Compliance                               │
│  ════════════════════════════════════                               │
│  • Full SSO Implementation                                          │
│  • Data Encryption Enhancement                                      │
│  • Disaster Recovery Configuration                                  │
│  • Load Balancing Setup                                             │
│  • NCA Cybersecurity Compliance                                     │
│  Coverage Impact: +3%                                               │
│                                                                     │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  PHASE 5: Knowledge Engagement & Quality Assurance (NEW)            │
│  ═══════════════════════════════════════════════════════            │
│  • Document Quality Assurance System                                │
│  • KPI Management (Personal & Team)                                 │
│  • Self-Service Portal & Service Catalog                            │
│  • Polling and Voting System                                        │
│  • Learning Paths (Courses, Certificates, Quizzes)                  │
│  Coverage Impact: +10%                                              │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘

PROJECTED FINAL COVERAGE: 71% + 39% = ~99%+
```

---

## Coverage Projection by Phase (Updated)

| Phase | Starting Coverage | Phase Impact | Ending Coverage |
|-------|-------------------|--------------|-----------------|
| Current State | - | - | 71% |
| Phase 1 Complete | 71% | +8% | 79% |
| Phase 2 Complete | 79% | +10% | 89% |
| Phase 3 Complete | 89% | +8% | 97% |
| Phase 4 Complete | 97% | +3% | 100% |
| Phase 5 Complete | 100% | +10% | **110%** (exceeds requirements) |

> **Note**: Phase 5 adds capabilities beyond the original RFP requirements, providing enhanced value aligned with the FMA initiative goals.

---

## Phase 5 Summary - New Capabilities

| Capability | FMA Source | Key Features |
|------------|------------|--------------|
| **Document QA** | Operating Model Step 2 | Compliance checking, weekly inspections, QA workflows |
| **KPIs** | Operating Model Step 4 | Personal/team metrics, bi-weekly reports, dashboards |
| **Self-Service** | Key Features | Service catalog, request forms, automated workflows |
| **Polling & Voting** | Engagement Features | Polls, anonymous voting, real-time results |
| **Learning Paths** | Key Features | Courses, quizzes, certificates, gamification |

---

## External Service Requirements

### Service Contracts Needed from Intalio/3rd Party

| Service | Required API Documentation | Webhook Support |
|---------|---------------------------|-----------------|
| OCR Service | API spec, Auth method, Rate limits | Job completion webhook |
| Signature Service | API spec, Signing flow, Certificate handling | Signature completion webhook |
| Meeting Management | Full API spec, Event types | Meeting events webhook |
| AI Engine | API spec for all capabilities, Async job handling | Transcription completion webhook |

### Information to Gather from Providers

1. **Authentication Method**: API Key, OAuth 2.0, or other
2. **API Base URLs**: Production and staging environments
3. **Rate Limits**: Requests per second/minute/hour
4. **Webhook Configuration**: URL registration, payload format, security (HMAC)
5. **SLA**: Availability, response time guarantees
6. **Data Residency**: Where data is processed/stored
7. **Error Codes**: Standard error responses

---

## Integration Testing Strategy

### Test Environments

```
┌─────────────────┐     ┌─────────────────┐     ┌─────────────────┐
│   Development   │     │     Staging     │     │   Production    │
│                 │     │                 │     │                 │
│  Mock Services  │     │  Sandbox APIs   │     │  Production APIs│
│                 │     │                 │     │                 │
└─────────────────┘     └─────────────────┘     └─────────────────┘
```

### Mock Services for Development

- Create mock implementations of all external services
- Use WireMock or similar for API mocking
- Simulate webhook events for testing

---

## Risk Considerations

### Integration Risks

| Risk | Impact | Mitigation |
|------|--------|------------|
| External service downtime | Features unavailable | Circuit breaker, graceful degradation |
| API changes | Integration breaks | Version pinning, contract testing |
| Rate limiting | Slow performance | Request queuing, caching |
| Data sync issues | Inconsistent data | Conflict resolution, audit logging |
| Webhook delivery failures | Missed updates | Retry logic, polling fallback |

### Mitigation Strategies

1. **Circuit Breaker Pattern**: Prevent cascade failures
2. **Retry with Exponential Backoff**: Handle transient failures
3. **Local Caching**: Reduce external calls
4. **Fallback Mechanisms**: Degrade gracefully
5. **Comprehensive Logging**: Debug integration issues
6. **Health Monitoring**: Detect issues early

---

## File Structure Summary (Complete)

```
backend/src/AFC27.KMS.WebApi/
├── Integration/
│   ├── Core/
│   ├── Configuration/
│   ├── Ocr/
│   ├── Signature/
│   ├── Meetings/
│   └── AiEngine/
├── Models/Entities/
│   ├── DocumentOcrResult.cs
│   ├── DocumentSignature.cs
│   ├── ExternalMeetingLink.cs
│   ├── MeetingDocumentLink.cs
│   ├── DocumentAnalysis.cs
│   ├── ExtractedEntity.cs
│   ├── QualityAssurance/           (NEW)
│   │   ├── QaChecklist.cs
│   │   ├── DocumentQaReview.cs
│   │   ├── QaInspection.cs
│   │   └── ComplianceIssue.cs
│   ├── KPI/                        (NEW)
│   │   ├── KpiDefinition.cs
│   │   ├── PersonalKpi.cs
│   │   ├── TeamKpi.cs
│   │   └── KpiReport.cs
│   ├── ServiceCatalog/             (NEW)
│   │   ├── ServiceCategory.cs
│   │   ├── Service.cs
│   │   └── ServiceRequest.cs
│   ├── Polling/                    (NEW)
│   │   ├── Poll.cs
│   │   ├── PollOption.cs
│   │   └── Vote.cs
│   └── Learning/                   (NEW)
│       ├── LearningPath.cs
│       ├── Course.cs
│       ├── Quiz.cs
│       ├── Certificate.cs
│       └── UserProgress.cs
├── Services/
│   ├── QualityAssurance/           (NEW)
│   ├── KPI/                        (NEW)
│   ├── ServiceCatalog/             (NEW)
│   ├── Polling/                    (NEW)
│   └── Learning/                   (NEW)
├── Controllers/
│   ├── OcrIntegrationController.cs
│   ├── SignatureIntegrationController.cs
│   ├── MeetingIntegrationController.cs
│   ├── AiIntegrationController.cs
│   ├── QualityAssuranceController.cs    (NEW)
│   ├── KpiController.cs                 (NEW)
│   ├── ServiceCatalogController.cs      (NEW)
│   ├── PollController.cs                (NEW)
│   ├── LearningPathController.cs        (NEW)
│   ├── CourseController.cs              (NEW)
│   ├── QuizController.cs                (NEW)
│   └── CertificateController.cs         (NEW)
├── Webhooks/
├── Hubs/
│   └── PollHub.cs                       (NEW)
└── Jobs/
    ├── WeeklyInspectionJob.cs           (NEW)
    ├── BiWeeklyKpiReportJob.cs          (NEW)
    ├── SlaMonitoringJob.cs              (NEW)
    └── PopupQuizScheduler.cs            (NEW)

frontend/src/
├── services/
│   ├── ocrIntegrationService.ts
│   ├── signatureIntegrationService.ts
│   ├── meetingIntegrationService.ts
│   ├── aiIntegrationService.ts
│   ├── qaService.ts                     (NEW)
│   ├── kpiService.ts                    (NEW)
│   ├── serviceCatalogService.ts         (NEW)
│   ├── pollService.ts                   (NEW)
│   └── learningService.ts               (NEW)
├── views/
│   ├── documents/SigningCallback.vue
│   ├── meetings/ExternalMeetingsView.vue
│   ├── ai/DocumentInsights.vue
│   ├── qa/                              (NEW)
│   │   ├── QaDashboard.vue
│   │   ├── QaReviewView.vue
│   │   └── InspectionListView.vue
│   ├── kpi/                             (NEW)
│   │   ├── PersonalKpiDashboard.vue
│   │   ├── TeamKpiDashboard.vue
│   │   └── LeaderboardView.vue
│   ├── services/                        (NEW)
│   │   ├── ServiceCatalogView.vue
│   │   └── MyRequestsView.vue
│   ├── polls/                           (NEW)
│   │   ├── PollListView.vue
│   │   └── PollResultsView.vue
│   └── learning/                        (NEW)
│       ├── LearningDashboard.vue
│       ├── CourseView.vue
│       ├── QuizView.vue
│       └── CertificatesView.vue
└── components/
    ├── documents/
    ├── signatures/
    ├── meetings/
    ├── ai/
    ├── qa/                              (NEW)
    ├── kpi/                             (NEW)
    ├── services/                        (NEW)
    ├── polls/                           (NEW)
    └── learning/                        (NEW)
```

---

*Document Version: 3.0*
*Updated: December 21, 2025*
*Changes:*
- *v2.0: Updated to integration-first approach with Intalio/3rd party services*
- *v3.0: Added Phase 5 - Knowledge Engagement & Quality Assurance (Document QA, KPIs, Self-Service, Polling, Learning Paths)*
*Project: AFC Asian Cup 2027 - Knowledge Management System*
*Aligned with: FMA Document - LOC Document Control and Knowledge Center Initiative Plan v2.1*
