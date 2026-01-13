<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAIServicesStore } from '@/stores/aiServices'
import {
  AILoadingIndicator,
  AIConfidenceBar,
  AIEntityHighlight,
  AITranslateDropdown,
  AIActionButton,
  AIResultCard
} from '@/components/ai'
import {
  CommentsSection,
  RatingStars,
  SocialShareButtons,
  BookmarkButton
} from '@/components/common'
import { useComments } from '@/composables/useComments'
import { useRatings } from '@/composables/useRatings'
import type {
  SummarizationResult,
  TranslationResult,
  NERResult,
  OCRResult,
  SupportedLanguage,
  NamedEntity
} from '@/types/ai'

const router = useRouter()
const route = useRoute()
const aiStore = useAIServicesStore()

// Comments
const documentId = computed(() => route.params.id as string)
const {
  comments,
  isLoading: commentsLoading,
  loadComments,
  addComment
} = useComments('document', documentId.value)

// Ratings
const { rating, submitRating, loadRating } = useRatings('document', documentId.value)

// Version History
interface Version {
  id: string
  version: string
  author: string
  date: Date
  changes: string
  size: string
}
const versions = ref<Version[]>([])
const showVersionHistory = ref(false)

const isLoading = ref(true)
const document = ref<any>(null)

// Mock document data - in production, this would come from an API
const mockDocuments = [
  {
    id: 1,
    name: 'AFC Asian Cup 2027 Schedule.pdf',
    description: 'Complete match schedule for all 51 games of the AFC Asian Cup 2027, including group stage, knockout rounds, and the final.',
    size: '4.2 MB',
    sizeBytes: 4404019,
    type: 'PDF',
    mimeType: 'application/pdf',
    icon: 'fas fa-file-pdf',
    iconBg: 'bg-rose-100',
    iconColor: 'text-rose-600',
    gradientFrom: 'from-rose-500',
    gradientTo: 'to-rose-600',
    createdAt: '2024-01-15T10:30:00Z',
    updatedAt: '2024-01-20T14:45:00Z',
    author: { name: 'AFC Media Team', initials: 'AM', color: '#006847' },
    library: 'Tournament Documents',
    tags: ['schedule', 'matches', 'official'],
    downloads: 1245,
    views: 3890,
    version: '2.1',
    pages: 24
  },
  {
    id: 2,
    name: 'Stadium Operations Manual.docx',
    description: 'Comprehensive operations manual covering all stadium procedures, security protocols, and event management guidelines.',
    size: '12.5 MB',
    sizeBytes: 13107200,
    type: 'Word',
    mimeType: 'application/vnd.openxmlformats-officedocument.wordprocessingml.document',
    icon: 'fas fa-file-word',
    iconBg: 'bg-blue-100',
    iconColor: 'text-blue-600',
    gradientFrom: 'from-blue-500',
    gradientTo: 'to-blue-600',
    createdAt: '2024-01-10T09:00:00Z',
    updatedAt: '2024-01-18T16:30:00Z',
    author: { name: 'Operations Team', initials: 'OT', color: '#2563eb' },
    library: 'Operations',
    tags: ['stadium', 'operations', 'manual'],
    downloads: 567,
    views: 1234,
    version: '3.0',
    pages: 156
  },
  {
    id: 3,
    name: 'Team Statistics 2027.xlsx',
    description: 'Detailed statistics for all participating teams including player data, historical performance, and tournament predictions.',
    size: '3.8 MB',
    sizeBytes: 3984588,
    type: 'Excel',
    mimeType: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
    icon: 'fas fa-file-excel',
    iconBg: 'bg-emerald-100',
    iconColor: 'text-emerald-600',
    gradientFrom: 'from-emerald-500',
    gradientTo: 'to-emerald-600',
    createdAt: '2024-01-12T11:15:00Z',
    updatedAt: '2024-01-19T10:00:00Z',
    author: { name: 'Statistics Dept', initials: 'SD', color: '#059669' },
    library: 'Analytics',
    tags: ['statistics', 'teams', 'data'],
    downloads: 892,
    views: 2156,
    version: '1.5',
    pages: null
  },
  {
    id: 4,
    name: 'Opening Ceremony Plan.pptx',
    description: 'Detailed presentation outlining the opening ceremony sequence, performers, technical requirements, and timeline.',
    size: '28.4 MB',
    sizeBytes: 29780582,
    type: 'PowerPoint',
    mimeType: 'application/vnd.openxmlformats-officedocument.presentationml.presentation',
    icon: 'fas fa-file-powerpoint',
    iconBg: 'bg-orange-100',
    iconColor: 'text-orange-600',
    gradientFrom: 'from-orange-500',
    gradientTo: 'to-orange-600',
    createdAt: '2024-01-08T14:00:00Z',
    updatedAt: '2024-01-17T09:30:00Z',
    author: { name: 'Events Team', initials: 'ET', color: '#ea580c' },
    library: 'Events',
    tags: ['ceremony', 'opening', 'presentation'],
    downloads: 234,
    views: 678,
    version: '4.2',
    pages: 48
  }
]

const documentIdNum = computed(() => Number(route.params.id))
const isPreviewMode = computed(() => route.query.preview === 'true')
const isFullscreen = ref(false)

onMounted(async () => {
  // Simulate API call
  setTimeout(async () => {
    document.value = mockDocuments.find(d => d.id === Number(route.params.id)) || mockDocuments[0]
    isLoading.value = false

    // Load version history
    versions.value = [
      { id: '3', version: document.value?.version || '2.1', author: document.value?.author.name || 'Unknown', date: new Date(document.value?.updatedAt || Date.now()), changes: 'Updated tournament schedule with final venue assignments', size: document.value?.size || '4.2 MB' },
      { id: '2', version: '2.0', author: document.value?.author.name || 'Unknown', date: new Date(Date.now() - 7 * 24 * 60 * 60 * 1000), changes: 'Added knockout round schedule', size: '3.8 MB' },
      { id: '1', version: '1.0', author: document.value?.author.name || 'Unknown', date: new Date(Date.now() - 14 * 24 * 60 * 60 * 1000), changes: 'Initial document upload', size: '2.1 MB' }
    ]

    // Load comments and ratings
    await Promise.all([
      loadComments(),
      loadRating()
    ])
  }, 500)
})

function goBack() {
  router.push('/documents')
}

function downloadDocument() {
  if (!document.value) return

  // Simulate file download
  const mimeTypes: Record<string, string> = {
    'PDF': 'application/pdf',
    'Word': 'application/vnd.openxmlformats-officedocument.wordprocessingml.document',
    'Excel': 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
    'PowerPoint': 'application/vnd.openxmlformats-officedocument.presentationml.presentation'
  }

  // Create a placeholder blob for demo purposes
  const content = `This is a placeholder for: ${document.value.name}\n\nDocument Details:\n- Type: ${document.value.type}\n- Size: ${document.value.size}\n- Version: ${document.value.version}\n- Pages: ${document.value.pages || 'N/A'}\n\nIn production, this would download the actual file from the server.`
  const blob = new Blob([content], { type: mimeTypes[document.value.type] || 'application/octet-stream' })
  const url = URL.createObjectURL(blob)

  // Create download link and trigger click
  const link = window.document.createElement('a')
  link.href = url
  link.download = document.value.name
  window.document.body.appendChild(link)
  link.click()
  window.document.body.removeChild(link)
  URL.revokeObjectURL(url)
}

function shareDocument() {
  if (document.value) {
    navigator.clipboard.writeText(window.location.href)
    alert('Link copied to clipboard!')
  }
}

function printDocument() {
  window.print()
}

function toggleFullscreen() {
  isFullscreen.value = !isFullscreen.value
}

function closeFullscreen() {
  isFullscreen.value = false
}

function formatDate(dateString: string): string {
  return new Date(dateString).toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

function formatShortDate(dateString: string): string {
  return new Date(dateString).toLocaleDateString('en-US', {
    month: 'short',
    day: 'numeric',
    year: 'numeric'
  })
}

const relatedDocuments = computed(() => {
  if (!document.value) return []
  return mockDocuments.filter(d => d.id !== document.value.id).slice(0, 3)
})

// ============================================================================
// AI Features State
// ============================================================================

const showAIPanel = ref(true)
const activeAITab = ref<'summary' | 'entities' | 'translate' | 'ocr'>('summary')

// Summary state
const documentSummary = ref<SummarizationResult | null>(null)
const isLoadingSummary = ref(false)
const summaryType = ref<'brief' | 'detailed' | 'bullet-points'>('brief')

// Entity extraction state
const extractedEntities = ref<NERResult | null>(null)
const isLoadingEntities = ref(false)
const highlightedEntityType = ref<string | null>(null)

// Translation state
const translatedContent = ref<TranslationResult | null>(null)
const isLoadingTranslation = ref(false)
const targetLanguage = ref<SupportedLanguage>('ar')

// OCR state
const ocrResult = ref<OCRResult | null>(null)
const isLoadingOCR = ref(false)
const ocrConfidence = ref(0)

// Mock document content for AI analysis
const mockDocumentContent: Record<number, string> = {
  1: `AFC Asian Cup 2027 Saudi Arabia - Complete Tournament Schedule

The AFC Asian Cup 2027 will be hosted across five world-class stadiums in Saudi Arabia from January 10 to February 10, 2027. This prestigious tournament will feature 24 teams competing in 51 matches across the group stage and knockout rounds.

Group Stage Overview:
- 6 groups of 4 teams each
- Top 2 teams from each group advance to knockout rounds
- Best 4 third-placed teams also qualify

Key Venues:
1. King Fahd International Stadium, Riyadh (Capacity: 68,000) - Opening Match & Final
2. King Abdullah Sports City, Jeddah (Capacity: 62,000) - Semi-finals
3. Prince Mohamed bin Fahd Stadium, Dammam (Capacity: 35,000)
4. King Saud University Stadium, Riyadh (Capacity: 25,000)
5. Prince Abdullah Al Faisal Stadium, Jeddah (Capacity: 24,000)

Tournament Timeline:
- Group Stage: January 10-22, 2027
- Round of 16: January 25-28, 2027
- Quarter-finals: January 31 - February 1, 2027
- Semi-finals: February 5-6, 2027
- Third Place Match: February 9, 2027
- Final: February 10, 2027

The opening match will feature host nation Saudi Arabia at King Fahd International Stadium. The final will also be held at the same venue, with an expected global audience of over 1 billion viewers.

Ticketing:
- General sales begin March 2026
- Priority access for AFC members
- Stadium hospitality packages available

Broadcast Partners:
- beIN Sports (MENA region)
- Various international broadcasters to be announced

Contact: media@afc.com
Official Website: www.afcasiancup2027.com`,

  2: `Stadium Operations Manual - AFC Asian Cup 2027

Version 3.0 - Last Updated: January 2024
Prepared by: AFC Operations Team

1. INTRODUCTION

This comprehensive operations manual provides detailed guidelines for all stadium operations during the AFC Asian Cup 2027. All venue managers, security personnel, and event staff must familiarize themselves with these procedures.

2. SECURITY PROTOCOLS

2.1 Access Control
- All personnel must display valid accreditation at all times
- Biometric verification required for restricted areas
- Vehicle screening at all entry points
- Prohibited items include: weapons, drones, fireworks, alcohol

2.2 Emergency Procedures
- Evacuation routes clearly marked in multiple languages
- Medical stations located at Gates A, C, E, and G
- Coordination with local emergency services
- Crisis communication protocols

3. EVENT MANAGEMENT

3.1 Match Day Timeline
- T-6 hours: Final security sweep
- T-4 hours: Staff briefing
- T-3 hours: Gates open for hospitality
- T-2 hours: General admission
- T-30 min: Team arrivals
- T-0: Kickoff

3.2 Crowd Management
- Maximum capacity management
- Flow control through designated pathways
- VIP and media access coordination
- Accessibility services

4. FACILITY OPERATIONS

4.1 Pitch Management
- FIFA quality certification requirements
- Pre-match preparation procedures
- Weather contingency plans

4.2 Broadcast Requirements
- Camera positions and power supplies
- Commentary positions
- Media mixed zone operations

5. VENDOR MANAGEMENT

- Approved catering providers only
- Cash and digital payment systems
- Food safety compliance
- Merchandising guidelines

For emergency contacts, see Appendix A.
Document Classification: CONFIDENTIAL`,

  3: `AFC Asian Cup 2027 - Team Statistics Database

Participating Nations Statistics Overview

EAST ASIA
- Japan: FIFA Ranking #17, 4x AFC Asian Cup winners
- South Korea: FIFA Ranking #23, 2x AFC Asian Cup winners
- China PR: FIFA Ranking #79, Host 2004
- Australia: FIFA Ranking #25, 1x AFC Asian Cup winner (2015)

WEST ASIA
- Iran: FIFA Ranking #21, 3x AFC Asian Cup winners
- Saudi Arabia: FIFA Ranking #56, Host 2027, 3x AFC Asian Cup winners
- Iraq: FIFA Ranking #68, 1x AFC Asian Cup winner (2007)
- Qatar: FIFA Ranking #37, 1x AFC Asian Cup winner (2019), FIFA World Cup 2022 Host
- UAE: FIFA Ranking #69
- Jordan: FIFA Ranking #87
- Syria: FIFA Ranking #93
- Palestine: FIFA Ranking #95
- Lebanon: FIFA Ranking #99
- Oman: FIFA Ranking #75
- Bahrain: FIFA Ranking #86
- Kuwait: FIFA Ranking #137

SOUTH ASIA
- India: FIFA Ranking #101
- Pakistan: FIFA Ranking #193

CENTRAL ASIA
- Uzbekistan: FIFA Ranking #73
- Tajikistan: FIFA Ranking #106
- Kyrgyzstan: FIFA Ranking #95

SOUTHEAST ASIA
- Vietnam: FIFA Ranking #94
- Thailand: FIFA Ranking #113
- Indonesia: FIFA Ranking #134
- Malaysia: FIFA Ranking #130

Historical Performance Data:
- Total matches played in Asian Cup history: 589
- Total goals scored: 1,847
- Average goals per match: 3.14
- Largest victory: Iran 19-0 Maldives (1998)`,

  4: `AFC Asian Cup 2027 Opening Ceremony Plan

CONFIDENTIAL - Events Team Internal Document

Theme: "Unity Through Football"

Ceremony Duration: 90 minutes (18:00 - 19:30 local time)
Kickoff: 20:00

SEGMENT BREAKDOWN:

1. WELCOME (15 minutes)
- Countdown video on giant screens
- Fireworks display
- Parade of flags (24 participating nations)
- Traditional Saudi welcome performance

2. CULTURAL SHOWCASE (25 minutes)
- "Journey Through Arabia" dance performance (300 performers)
- Holographic display of football heritage
- Traditional music fusion with contemporary beats
- Guest appearance: International music artists (TBA)

3. OFFICIAL PROTOCOL (20 minutes)
- Entry of dignitaries
- Speech by AFC President
- Speech by Saudi Arabia Sports Minister
- Ceremonial trophy presentation

4. ENTERTAINMENT FINALE (20 minutes)
- International superstar performance (Artist TBA)
- Drone light show (500 drones)
- Fireworks finale

5. TEAM PRESENTATION (10 minutes)
- All 24 teams enter stadium
- National anthems (Saudi Arabia)
- Referee team introduction

TECHNICAL REQUIREMENTS:
- 12 LED screens (various sizes)
- 2,500 lighting fixtures
- 500 synchronized drones
- Pyrotechnics certified
- Sound system: 360-degree coverage

REHEARSAL SCHEDULE:
- Technical rehearsal: January 5-7, 2027
- Dress rehearsal: January 8, 2027
- Final rehearsal: January 9, 2027

BUDGET: SAR 75,000,000 (approx. USD 20 million)

Contact: events@afc2027.sa`
}

// ============================================================================
// AI Functions
// ============================================================================

async function generateSummary() {
  if (!document.value || isLoadingSummary.value) return

  isLoadingSummary.value = true
  documentSummary.value = null

  try {
    // Simulate API call with mock data
    await new Promise(resolve => setTimeout(resolve, 1200))

    const content = mockDocumentContent[document.value.id] || document.value.description
    const summaries: Record<string, Record<string, SummarizationResult>> = {
      '1': {
        brief: {
          summary: 'The AFC Asian Cup 2027 will be hosted in Saudi Arabia from January 10 to February 10, featuring 24 teams in 51 matches across 5 stadiums. The tournament includes group stages and knockout rounds, with the final at King Fahd International Stadium.',
          keyPoints: [
            '24 teams competing in 51 matches',
            '5 world-class venues across Saudi Arabia',
            'Group Stage: Jan 10-22, Knockout: Jan 25 - Feb 10',
            'Expected global audience of 1 billion viewers'
          ],
          wordCount: content.split(' ').length,
          processingTime: 1.2,
          confidence: 0.94
        },
        detailed: {
          summary: 'The AFC Asian Cup 2027 represents a landmark football tournament hosted across Saudi Arabia from January 10 to February 10, 2027. The competition will showcase 24 of Asia\'s finest national teams competing in 51 matches across five premier stadiums. The tournament structure comprises a group stage with 6 groups of 4 teams, followed by a knockout phase. Key venues include King Fahd International Stadium (68,000 capacity) for the opening and final, and King Abdullah Sports City for semi-finals. The event anticipates over 1 billion global viewers, with ticket sales beginning March 2026.',
          keyPoints: [
            '24 participating nations in 6 groups',
            'King Fahd International Stadium hosts opening and final',
            'Group stage runs January 10-22, knockout phase follows',
            'Semi-finals at King Abdullah Sports City, Jeddah',
            'Global broadcast through beIN Sports and partners',
            'Ticket sales open March 2026 with AFC priority access'
          ],
          wordCount: content.split(' ').length,
          processingTime: 1.8,
          confidence: 0.96
        },
        'bullet-points': {
          summary: '• Tournament: AFC Asian Cup 2027\n• Host: Saudi Arabia\n• Dates: January 10 - February 10, 2027\n• Teams: 24 nations\n• Matches: 51 total\n• Main Venue: King Fahd International Stadium\n• Capacity: Up to 68,000\n• Broadcast: beIN Sports (MENA)',
          keyPoints: [
            '6 groups of 4 teams each',
            'Top 2 plus best 4 third-placed teams advance',
            '5 stadium venues across Saudi Arabia',
            'Final expected to draw 1B+ viewers'
          ],
          wordCount: content.split(' ').length,
          processingTime: 0.9,
          confidence: 0.92
        }
      },
      '2': {
        brief: {
          summary: 'Comprehensive stadium operations manual covering security protocols, event management procedures, and facility operations for AFC Asian Cup 2027 venues. Includes access control, emergency procedures, and broadcast requirements.',
          keyPoints: [
            'Security protocols with biometric verification',
            'Match day timeline from T-6 hours',
            'Emergency and evacuation procedures',
            'FIFA pitch certification requirements'
          ],
          wordCount: content.split(' ').length,
          processingTime: 1.1,
          confidence: 0.93
        },
        detailed: {
          summary: 'This Version 3.0 operations manual provides exhaustive guidelines for AFC Asian Cup 2027 stadium management. Security measures include biometric access control, vehicle screening, and prohibition of weapons, drones, and alcohol. Match day operations follow a strict timeline beginning 6 hours before kickoff with security sweeps, followed by staff briefings, hospitality opening, and general admission. Emergency protocols coordinate with local services and maintain medical stations at multiple gates. Facility operations cover FIFA pitch certification, weather contingencies, and broadcast infrastructure requirements.',
          keyPoints: [
            'Mandatory accreditation with biometric verification',
            'Comprehensive prohibited items list',
            'Medical stations at Gates A, C, E, and G',
            'Match day timeline from T-6 hours to kickoff',
            'VIP, media, and accessibility coordination',
            'FIFA quality pitch certification required'
          ],
          wordCount: content.split(' ').length,
          processingTime: 1.9,
          confidence: 0.95
        },
        'bullet-points': {
          summary: '• Document: Stadium Operations Manual v3.0\n• Security: Biometric + vehicle screening\n• Access: Valid accreditation required\n• Match Day: Timeline starts T-6 hours\n• Medical: Stations at Gates A, C, E, G\n• Pitch: FIFA certified\n• Classification: CONFIDENTIAL',
          keyPoints: [
            'Emergency evacuation routes in multiple languages',
            'Approved vendors and catering only',
            'Crowd flow control through designated paths',
            'Broadcast camera positions specified'
          ],
          wordCount: content.split(' ').length,
          processingTime: 0.8,
          confidence: 0.91
        }
      }
    }

    const docId = document.value.id.toString()
    const mockResult = summaries[docId]?.[summaryType.value] || {
      summary: `Summary of ${document.value.name}: ${document.value.description}`,
      keyPoints: ['Key information extracted from document', 'Important points highlighted', 'Main topics identified'],
      wordCount: document.value.description.split(' ').length,
      processingTime: 1.0,
      confidence: 0.85
    }

    documentSummary.value = mockResult
  } catch (error) {
    console.error('Failed to generate summary:', error)
  } finally {
    isLoadingSummary.value = false
  }
}

async function extractDocumentEntities() {
  if (!document.value || isLoadingEntities.value) return

  isLoadingEntities.value = true
  extractedEntities.value = null

  try {
    await new Promise(resolve => setTimeout(resolve, 1000))

    const mockEntities: Record<number, NERResult> = {
      1: {
        entities: [
          { text: 'AFC Asian Cup 2027', type: 'event', confidence: 0.98, startOffset: 0, endOffset: 18 },
          { text: 'Saudi Arabia', type: 'location', confidence: 0.97, startOffset: 45, endOffset: 57 },
          { text: 'January 10', type: 'date', confidence: 0.96, startOffset: 90, endOffset: 100 },
          { text: 'February 10, 2027', type: 'date', confidence: 0.96, startOffset: 104, endOffset: 121 },
          { text: 'King Fahd International Stadium', type: 'location', confidence: 0.95, startOffset: 200, endOffset: 231 },
          { text: 'Riyadh', type: 'location', confidence: 0.97, startOffset: 233, endOffset: 239 },
          { text: 'King Abdullah Sports City', type: 'location', confidence: 0.94, startOffset: 280, endOffset: 305 },
          { text: 'Jeddah', type: 'location', confidence: 0.97, startOffset: 307, endOffset: 313 },
          { text: 'Dammam', type: 'location', confidence: 0.96, startOffset: 350, endOffset: 356 },
          { text: 'beIN Sports', type: 'organization', confidence: 0.93, startOffset: 500, endOffset: 511 }
        ],
        processingTime: 1.0,
        modelVersion: '2.1.0'
      },
      2: {
        entities: [
          { text: 'AFC Asian Cup 2027', type: 'event', confidence: 0.98, startOffset: 0, endOffset: 18 },
          { text: 'AFC Operations Team', type: 'organization', confidence: 0.95, startOffset: 80, endOffset: 99 },
          { text: 'January 2024', type: 'date', confidence: 0.94, startOffset: 45, endOffset: 57 },
          { text: 'Gates A, C, E, G', type: 'location', confidence: 0.88, startOffset: 400, endOffset: 416 },
          { text: 'FIFA', type: 'organization', confidence: 0.97, startOffset: 600, endOffset: 604 }
        ],
        processingTime: 0.9,
        modelVersion: '2.1.0'
      },
      3: {
        entities: [
          { text: 'Japan', type: 'location', confidence: 0.98, startOffset: 50, endOffset: 55 },
          { text: 'South Korea', type: 'location', confidence: 0.98, startOffset: 100, endOffset: 111 },
          { text: 'China PR', type: 'location', confidence: 0.97, startOffset: 150, endOffset: 158 },
          { text: 'Australia', type: 'location', confidence: 0.98, startOffset: 200, endOffset: 209 },
          { text: 'Iran', type: 'location', confidence: 0.98, startOffset: 250, endOffset: 254 },
          { text: 'Saudi Arabia', type: 'location', confidence: 0.98, startOffset: 300, endOffset: 312 },
          { text: 'Qatar', type: 'location', confidence: 0.98, startOffset: 400, endOffset: 405 },
          { text: 'FIFA World Cup 2022', type: 'event', confidence: 0.96, startOffset: 420, endOffset: 439 }
        ],
        processingTime: 1.1,
        modelVersion: '2.1.0'
      },
      4: {
        entities: [
          { text: 'AFC Asian Cup 2027', type: 'event', confidence: 0.98, startOffset: 0, endOffset: 18 },
          { text: 'AFC President', type: 'person', confidence: 0.89, startOffset: 400, endOffset: 413 },
          { text: 'Saudi Arabia Sports Minister', type: 'person', confidence: 0.87, startOffset: 450, endOffset: 478 },
          { text: 'January 5-7, 2027', type: 'date', confidence: 0.95, startOffset: 700, endOffset: 717 },
          { text: 'January 8, 2027', type: 'date', confidence: 0.95, startOffset: 750, endOffset: 765 },
          { text: 'January 9, 2027', type: 'date', confidence: 0.95, startOffset: 800, endOffset: 815 },
          { text: 'SAR 75,000,000', type: 'money', confidence: 0.92, startOffset: 900, endOffset: 914 },
          { text: 'USD 20 million', type: 'money', confidence: 0.91, startOffset: 920, endOffset: 934 }
        ],
        processingTime: 1.0,
        modelVersion: '2.1.0'
      }
    }

    extractedEntities.value = mockEntities[document.value.id] || {
      entities: [
        { text: document.value.author.name, type: 'organization', confidence: 0.9, startOffset: 0, endOffset: 10 }
      ],
      processingTime: 0.5,
      modelVersion: '2.1.0'
    }
  } catch (error) {
    console.error('Failed to extract entities:', error)
  } finally {
    isLoadingEntities.value = false
  }
}

async function translateDocument(lang: SupportedLanguage) {
  if (!document.value || isLoadingTranslation.value) return

  targetLanguage.value = lang
  isLoadingTranslation.value = true
  translatedContent.value = null

  try {
    await new Promise(resolve => setTimeout(resolve, 1500))

    const translations: Partial<Record<SupportedLanguage, Record<number, string>>> = {
      ar: {
        1: 'كأس آسيا 2027 - جدول البطولة الكامل\n\nستقام بطولة كأس آسيا 2027 في المملكة العربية السعودية في الفترة من 10 يناير إلى 10 فبراير 2027. ستشهد البطولة مشاركة 24 منتخباً في 51 مباراة عبر خمسة ملاعب عالمية المستوى.',
        2: 'دليل عمليات الاستاد - كأس آسيا 2027\n\nيوفر هذا الدليل الشامل إرشادات تفصيلية لجميع عمليات الاستاد خلال بطولة كأس آسيا 2027. يجب على جميع مديري الأماكن وأفراد الأمن والموظفين التعرف على هذه الإجراءات.',
        3: 'قاعدة بيانات إحصائيات الفرق - كأس آسيا 2027\n\nنظرة عامة على إحصائيات الدول المشاركة\n\nشرق آسيا:\n- اليابان: التصنيف 17، 4 مرات بطل كأس آسيا\n- كوريا الجنوبية: التصنيف 23، مرتان بطل كأس آسيا',
        4: 'خطة حفل الافتتاح - كأس آسيا 2027\n\nالموضوع: "الوحدة من خلال كرة القدم"\n\nمدة الحفل: 90 دقيقة\nالانطلاق: الساعة 20:00'
      },
      fr: {
        1: 'Coupe d\'Asie de l\'AFC 2027 - Calendrier complet du tournoi\n\nLa Coupe d\'Asie de l\'AFC 2027 sera organisée en Arabie Saoudite du 10 janvier au 10 février 2027. Ce tournoi prestigieux mettra en vedette 24 équipes dans 51 matchs à travers cinq stades de classe mondiale.',
        2: 'Manuel des opérations du stade - Coupe d\'Asie 2027\n\nCe manuel complet fournit des directives détaillées pour toutes les opérations du stade. Tout le personnel doit se familiariser avec ces procédures.',
        3: 'Base de données des statistiques des équipes - Coupe d\'Asie 2027\n\nAperçu des statistiques des nations participantes\n\nAsie de l\'Est:\n- Japon: Classement FIFA #17, 4x vainqueur\n- Corée du Sud: Classement FIFA #23, 2x vainqueur',
        4: 'Plan de la cérémonie d\'ouverture - Coupe d\'Asie 2027\n\nThème: "L\'unité par le football"\n\nDurée: 90 minutes\nCoup d\'envoi: 20h00'
      },
      es: {
        1: 'Copa Asiática de la AFC 2027 - Calendario completo del torneo\n\nLa Copa Asiática de la AFC 2027 se celebrará en Arabia Saudita del 10 de enero al 10 de febrero de 2027. Este prestigioso torneo contará con 24 equipos en 51 partidos en cinco estadios de clase mundial.',
        2: 'Manual de operaciones del estadio - Copa Asiática 2027\n\nEste manual completo proporciona pautas detalladas para todas las operaciones del estadio. Todo el personal debe familiarizarse con estos procedimientos.',
        3: 'Base de datos de estadísticas de equipos - Copa Asiática 2027\n\nResumen de estadísticas de las naciones participantes\n\nAsia Oriental:\n- Japón: Ranking FIFA #17, 4 veces campeón\n- Corea del Sur: Ranking FIFA #23, 2 veces campeón',
        4: 'Plan de ceremonia de apertura - Copa Asiática 2027\n\nTema: "Unidad a través del fútbol"\n\nDuración: 90 minutos\nInicio: 20:00'
      },
      zh: {
        1: '2027年亚洲杯 - 完整赛程\n\n2027年亚洲杯将于2027年1月10日至2月10日在沙特阿拉伯举行。这项著名的赛事将有24支球队在5座世界级体育场进行51场比赛。',
        2: '体育场运营手册 - 2027年亚洲杯\n\n本综合手册为所有体育场运营提供详细指南。所有场馆经理、安保人员和工作人员必须熟悉这些程序。',
        3: '球队统计数据库 - 2027年亚洲杯\n\n参赛国家统计概览\n\n东亚：\n- 日本：FIFA排名第17，4次亚洲杯冠军\n- 韩国：FIFA排名第23，2次亚洲杯冠军',
        4: '开幕式计划 - 2027年亚洲杯\n\n主题："足球带来的团结"\n\n时长：90分钟\n开球时间：20:00'
      },
      ja: {
        1: 'AFCアジアカップ2027 - 大会日程\n\nAFCアジアカップ2027は、2027年1月10日から2月10日までサウジアラビアで開催されます。24チームが5つのワールドクラスのスタジアムで51試合を行います。',
        2: 'スタジアム運営マニュアル - アジアカップ2027\n\nこの包括的なマニュアルは、すべてのスタジアム運営の詳細なガイドラインを提供します。すべてのスタッフはこれらの手順を熟知する必要があります。',
        3: 'チーム統計データベース - アジアカップ2027\n\n参加国統計概要\n\n東アジア：\n- 日本：FIFAランキング17位、4回優勝\n- 韓国：FIFAランキング23位、2回優勝',
        4: '開会式計画 - アジアカップ2027\n\nテーマ：「サッカーを通じた団結」\n\n所要時間：90分\nキックオフ：20:00'
      },
      de: {
        1: 'AFC Asien-Pokal 2027 - Vollständiger Turnierplan\n\nDer AFC Asien-Pokal 2027 wird vom 10. Januar bis 10. Februar 2027 in Saudi-Arabien ausgetragen. 24 Mannschaften werden in 51 Spielen in fünf Weltklasse-Stadien antreten.',
        2: 'Stadion-Betriebshandbuch - Asien-Pokal 2027\n\nDieses umfassende Handbuch enthält detaillierte Richtlinien für alle Stadionbetriebe. Alle Mitarbeiter müssen sich mit diesen Verfahren vertraut machen.',
        3: 'Team-Statistik-Datenbank - Asien-Pokal 2027\n\nStatistik-Übersicht der teilnehmenden Nationen\n\nOstasien:\n- Japan: FIFA-Ranking #17, 4x Sieger\n- Südkorea: FIFA-Ranking #23, 2x Sieger',
        4: 'Eröffnungszeremonie Plan - Asien-Pokal 2027\n\nThema: "Einheit durch Fußball"\n\nDauer: 90 Minuten\nAnstoß: 20:00 Uhr'
      },
      en: {
        1: mockDocumentContent[1],
        2: mockDocumentContent[2],
        3: mockDocumentContent[3],
        4: mockDocumentContent[4]
      }
    }

    const langNames: Partial<Record<SupportedLanguage, string>> = {
      en: 'English',
      ar: 'Arabic',
      fr: 'French',
      es: 'Spanish',
      zh: 'Chinese',
      ja: 'Japanese',
      de: 'German'
    }

    translatedContent.value = {
      translatedText: translations[lang]?.[document.value.id] || `[Translated to ${langNames[lang]}]\n\n${document.value.description}`,
      sourceLanguage: 'en',
      targetLanguage: lang,
      confidence: 0.94,
      processingTime: 1.5
    }
  } catch (error) {
    console.error('Failed to translate document:', error)
  } finally {
    isLoadingTranslation.value = false
  }
}

async function performDocumentOCR() {
  if (!document.value || isLoadingOCR.value) return

  isLoadingOCR.value = true
  ocrResult.value = null

  try {
    // Simulate OCR processing
    await new Promise(resolve => setTimeout(resolve, 2000))

    const mockOCRResults: Record<number, OCRResult> = {
      1: {
        text: mockDocumentContent[1],
        confidence: 0.96,
        language: 'en',
        processingTime: 2.0,
        pages: [
          { pageNumber: 1, text: 'AFC Asian Cup 2027 Saudi Arabia\nComplete Tournament Schedule', confidence: 0.97 },
          { pageNumber: 2, text: 'Group Stage Overview\n6 groups of 4 teams each', confidence: 0.95 },
          { pageNumber: 3, text: 'Key Venues and Stadium Information', confidence: 0.96 }
        ],
        wordCount: mockDocumentContent[1].split(' ').length,
        characterCount: mockDocumentContent[1].length
      },
      2: {
        text: mockDocumentContent[2],
        confidence: 0.94,
        language: 'en',
        processingTime: 2.5,
        pages: [
          { pageNumber: 1, text: 'Stadium Operations Manual\nVersion 3.0', confidence: 0.95 },
          { pageNumber: 2, text: 'Security Protocols\nAccess Control Guidelines', confidence: 0.93 }
        ],
        wordCount: mockDocumentContent[2].split(' ').length,
        characterCount: mockDocumentContent[2].length
      }
    }

    ocrResult.value = mockOCRResults[document.value.id] || {
      text: `[OCR extracted text from ${document.value.name}]\n\n${document.value.description}`,
      confidence: 0.88,
      language: 'en',
      processingTime: 1.8,
      wordCount: 150,
      characterCount: 800
    }

    ocrConfidence.value = ocrResult.value.confidence * 100
  } catch (error) {
    console.error('Failed to perform OCR:', error)
  } finally {
    isLoadingOCR.value = false
  }
}

function getEntityIcon(type: string): string {
  const icons: Record<string, string> = {
    person: 'fas fa-user',
    organization: 'fas fa-building',
    location: 'fas fa-map-marker-alt',
    date: 'fas fa-calendar',
    event: 'fas fa-calendar-check',
    money: 'fas fa-dollar-sign'
  }
  return icons[type] || 'fas fa-tag'
}

function getEntityColor(type: string): string {
  const colors: Record<string, string> = {
    person: 'text-blue-600 bg-blue-50',
    organization: 'text-purple-600 bg-purple-50',
    location: 'text-green-600 bg-green-50',
    date: 'text-orange-600 bg-orange-50',
    event: 'text-teal-600 bg-teal-50',
    money: 'text-yellow-600 bg-yellow-50'
  }
  return colors[type] || 'text-gray-600 bg-gray-50'
}

function filterEntitiesByType(type: string | null): NamedEntity[] {
  if (!extractedEntities.value) return []
  if (!type) return extractedEntities.value.entities
  return extractedEntities.value.entities.filter(e => e.type === type)
}

function getUniqueEntityTypes(): string[] {
  if (!extractedEntities.value) return []
  return [...new Set(extractedEntities.value.entities.map(e => e.type))]
}

function toggleAIPanel() {
  showAIPanel.value = !showAIPanel.value
}

function copyOCRText() {
  if (ocrResult.value) {
    navigator.clipboard.writeText(ocrResult.value.text)
    alert('OCR text copied to clipboard!')
  }
}

function copyTranslation() {
  if (translatedContent.value) {
    navigator.clipboard.writeText(translatedContent.value.translatedText)
    alert('Translation copied to clipboard!')
  }
}

function copySummary() {
  if (documentSummary.value) {
    navigator.clipboard.writeText(documentSummary.value.summary)
    alert('Summary copied to clipboard!')
  }
}

async function handleRating(stars: number) {
  await submitRating(stars)
}

function formatVersionDate(date: Date): string {
  return new Date(date).toLocaleDateString('en-US', {
    month: 'short',
    day: 'numeric',
    year: 'numeric'
  })
}
</script>

<template>
  <div class="min-h-screen bg-gray-50/50">
    <!-- Loading State -->
    <div v-if="isLoading" class="flex items-center justify-center min-h-[60vh]">
      <div class="text-center">
        <div class="w-12 h-12 border-4 border-teal-500 border-t-transparent rounded-full animate-spin mx-auto mb-4"></div>
        <p class="text-gray-500">Loading document...</p>
      </div>
    </div>

    <!-- Document Content -->
    <div v-else-if="document" class="page-view fade-in">
      <!-- Hero Header -->
      <div :class="['relative overflow-hidden rounded-2xl mb-6 bg-gradient-to-br', document.gradientFrom, document.gradientTo]">
        <!-- Decorative circles -->
        <div class="absolute top-0 right-0 w-64 h-64 bg-white/5 rounded-full -translate-y-1/2 translate-x-1/4"></div>
        <div class="absolute bottom-0 left-0 w-48 h-48 bg-white/5 rounded-full translate-y-1/2 -translate-x-1/4"></div>
        <div class="absolute top-1/2 right-1/3 w-32 h-32 bg-white/10 rounded-full"></div>

        <div class="relative z-10 px-8 py-6">
          <!-- Breadcrumb -->
          <div class="flex items-center gap-2 text-white/70 text-sm mb-4">
            <router-link to="/documents" class="hover:text-white transition-colors">Documents</router-link>
            <i class="fas fa-chevron-right text-xs"></i>
            <span class="text-white">{{ document.name }}</span>
          </div>

          <!-- Title and actions row -->
          <div class="flex items-center justify-between flex-wrap gap-4">
            <div class="flex items-center gap-4">
              <button @click="goBack" class="w-10 h-10 rounded-xl bg-white/10 hover:bg-white/20 text-white flex items-center justify-center transition-all">
                <i class="fas fa-arrow-left"></i>
              </button>
              <div class="w-14 h-14 bg-white rounded-xl shadow-lg flex items-center justify-center">
                <i :class="[document.icon, document.iconColor, 'text-2xl']"></i>
              </div>
              <div>
                <h1 class="text-2xl md:text-3xl font-bold text-white">{{ document.name }}</h1>
                <p class="text-white/70">{{ document.type }} • {{ document.size }} • Version {{ document.version }}</p>
              </div>
            </div>

            <!-- Action buttons -->
            <div class="flex items-center gap-3">
              <button @click="downloadDocument" class="px-4 py-2 bg-white text-gray-700 rounded-xl font-medium hover:bg-gray-50 transition-all shadow-sm flex items-center gap-2">
                <i class="fas fa-download"></i>
                <span>Download</span>
              </button>
              <button @click="shareDocument" class="px-4 py-2 bg-transparent text-white border border-white/30 rounded-xl font-medium hover:bg-white/10 transition-all flex items-center gap-2">
                <i class="fas fa-share-alt"></i>
                <span class="hidden sm:inline">Share</span>
              </button>
              <BookmarkButton
                :content-id="document.id.toString()"
                content-type="document"
                size="md"
                variant="icon"
                class="text-white"
              />
            </div>
          </div>
        </div>
      </div>

      <!-- Main Content Grid -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <!-- Left Column - Document Preview -->
        <div class="lg:col-span-2 space-y-6">
          <!-- Document Description Card -->
          <div class="bg-white rounded-2xl shadow-sm border border-gray-100 p-6">
            <h2 class="text-lg font-semibold text-gray-900 mb-3 flex items-center gap-2">
              <i class="fas fa-info-circle text-teal-500"></i>
              About this Document
            </h2>
            <p class="text-gray-600 mb-4">{{ document.description }}</p>

            <!-- Secondary Actions -->
            <div class="flex flex-wrap gap-3">
              <button @click="printDocument" class="px-4 py-2 bg-gray-100 hover:bg-gray-200 text-gray-700 rounded-xl font-medium flex items-center gap-2 transition-colors">
                <i class="fas fa-print"></i>
                <span>Print</span>
              </button>
              <SocialShareButtons
                :title="document.name"
                :description="document.description"
                layout="horizontal"
                size="sm"
              />
            </div>
          </div>

          <!-- Document Preview -->
          <div :class="['bg-white rounded-2xl shadow-sm border border-gray-100 p-6', isPreviewMode ? 'ring-2 ring-teal-500' : '']">
            <div class="flex items-center justify-between mb-4">
              <h2 class="text-lg font-semibold text-gray-900 flex items-center gap-2">
                <i class="fas fa-eye text-teal-500"></i>
                Preview
                <span v-if="isPreviewMode" class="px-2 py-0.5 bg-teal-100 text-teal-700 text-xs rounded-full font-medium">Active</span>
              </h2>
              <button @click="toggleFullscreen" class="px-3 py-1.5 bg-gray-100 hover:bg-gray-200 text-gray-600 rounded-lg text-sm font-medium flex items-center gap-2 transition-colors">
                <i class="fas fa-expand"></i>
                Fullscreen
              </button>
            </div>
            <div :class="['bg-gray-100 rounded-xl flex items-center justify-center border-2 border-dashed border-gray-200', isPreviewMode ? 'aspect-[3/2]' : 'aspect-[4/3]']">
              <div class="text-center p-8">
                <div :class="[document.iconBg, 'w-24 h-24 rounded-2xl flex items-center justify-center mx-auto mb-4 shadow-lg']">
                  <i :class="[document.icon, document.iconColor, 'text-4xl']"></i>
                </div>
                <p class="text-gray-500 mb-4">Preview not available</p>
                <button @click="downloadDocument" class="px-4 py-2 bg-teal-500 hover:bg-teal-600 text-white rounded-lg text-sm font-medium transition-colors">
                  Download to View
                </button>
              </div>
            </div>
          </div>
        </div>

        <!-- Right Column - Metadata & AI Features -->
        <div class="space-y-6">
          <!-- AI Features Panel -->
          <div class="bg-white rounded-2xl shadow-sm border border-gray-100 overflow-hidden">
            <!-- AI Panel Header -->
            <div class="px-6 py-4 bg-gradient-to-r from-teal-500 to-teal-600 flex items-center justify-between">
              <div class="flex items-center gap-2 text-white">
                <i class="fas fa-wand-magic-sparkles"></i>
                <span class="font-semibold">AI Analysis</span>
              </div>
              <button @click="toggleAIPanel" class="text-white/80 hover:text-white transition-colors">
                <i :class="['fas', showAIPanel ? 'fa-chevron-up' : 'fa-chevron-down']"></i>
              </button>
            </div>

            <!-- AI Panel Content -->
            <div v-if="showAIPanel" class="p-4">
              <!-- AI Tab Navigation -->
              <div class="flex gap-1 mb-4 bg-gray-100 rounded-lg p-1">
                <button
                  v-for="tab in [
                    { id: 'summary', icon: 'fas fa-file-lines', label: 'Summary' },
                    { id: 'entities', icon: 'fas fa-tags', label: 'Entities' },
                    { id: 'translate', icon: 'fas fa-language', label: 'Translate' },
                    { id: 'ocr', icon: 'fas fa-file-image', label: 'OCR' }
                  ]"
                  :key="tab.id"
                  @click="activeAITab = tab.id as any"
                  :class="[
                    'flex-1 px-2 py-2 rounded-md text-xs font-medium transition-all flex items-center justify-center gap-1',
                    activeAITab === tab.id
                      ? 'bg-white text-teal-600 shadow-sm'
                      : 'text-gray-500 hover:text-gray-700'
                  ]"
                >
                  <i :class="tab.icon"></i>
                  <span class="hidden sm:inline">{{ tab.label }}</span>
                </button>
              </div>

              <!-- Summary Tab -->
              <div v-if="activeAITab === 'summary'" class="space-y-4">
                <!-- Summary Type Selection -->
                <div class="flex gap-2">
                  <button
                    v-for="type in [
                      { id: 'brief', label: 'Brief' },
                      { id: 'detailed', label: 'Detailed' },
                      { id: 'bullet-points', label: 'Bullets' }
                    ]"
                    :key="type.id"
                    @click="summaryType = type.id as any"
                    :class="[
                      'flex-1 px-3 py-1.5 rounded-lg text-xs font-medium transition-all',
                      summaryType === type.id
                        ? 'bg-teal-100 text-teal-700'
                        : 'bg-gray-50 text-gray-600 hover:bg-gray-100'
                    ]"
                  >
                    {{ type.label }}
                  </button>
                </div>

                <!-- Generate Button -->
                <button
                  @click="generateSummary"
                  :disabled="isLoadingSummary"
                  class="w-full px-4 py-2.5 bg-teal-500 hover:bg-teal-600 disabled:bg-teal-300 text-white rounded-lg font-medium flex items-center justify-center gap-2 transition-colors"
                >
                  <i :class="['fas', isLoadingSummary ? 'fa-spinner fa-spin' : 'fa-wand-magic-sparkles']"></i>
                  {{ isLoadingSummary ? 'Generating...' : 'Generate Summary' }}
                </button>

                <!-- Summary Result -->
                <div v-if="documentSummary" class="space-y-3">
                  <div class="bg-teal-50 rounded-lg p-4 border border-teal-100">
                    <p class="text-sm text-gray-700 whitespace-pre-line">{{ documentSummary.summary }}</p>
                  </div>

                  <!-- Key Points -->
                  <div v-if="documentSummary.keyPoints?.length" class="space-y-2">
                    <p class="text-xs font-semibold text-gray-500 uppercase">Key Points</p>
                    <ul class="space-y-1">
                      <li v-for="(point, idx) in documentSummary.keyPoints" :key="idx" class="flex items-start gap-2 text-sm text-gray-600">
                        <i class="fas fa-check-circle text-teal-500 mt-0.5 flex-shrink-0"></i>
                        <span>{{ point }}</span>
                      </li>
                    </ul>
                  </div>

                  <!-- Actions -->
                  <div class="flex items-center justify-between text-xs text-gray-400">
                    <span>{{ ((documentSummary.confidence ?? 0) * 100).toFixed(0) }}% confidence</span>
                    <button @click="copySummary" class="flex items-center gap-1 text-teal-600 hover:text-teal-700">
                      <i class="fas fa-copy"></i>
                      Copy
                    </button>
                  </div>
                </div>
              </div>

              <!-- Entities Tab -->
              <div v-if="activeAITab === 'entities'" class="space-y-4">
                <button
                  @click="extractDocumentEntities"
                  :disabled="isLoadingEntities"
                  class="w-full px-4 py-2.5 bg-teal-500 hover:bg-teal-600 disabled:bg-teal-300 text-white rounded-lg font-medium flex items-center justify-center gap-2 transition-colors"
                >
                  <i :class="['fas', isLoadingEntities ? 'fa-spinner fa-spin' : 'fa-magnifying-glass']"></i>
                  {{ isLoadingEntities ? 'Extracting...' : 'Extract Entities' }}
                </button>

                <!-- Entity Filter -->
                <div v-if="extractedEntities" class="flex flex-wrap gap-2">
                  <button
                    @click="highlightedEntityType = null"
                    :class="[
                      'px-2 py-1 rounded-full text-xs font-medium transition-all',
                      highlightedEntityType === null
                        ? 'bg-teal-100 text-teal-700'
                        : 'bg-gray-100 text-gray-600 hover:bg-gray-200'
                    ]"
                  >
                    All ({{ extractedEntities.entities.length }})
                  </button>
                  <button
                    v-for="type in getUniqueEntityTypes()"
                    :key="type"
                    @click="highlightedEntityType = type"
                    :class="[
                      'px-2 py-1 rounded-full text-xs font-medium transition-all capitalize',
                      highlightedEntityType === type
                        ? 'bg-teal-100 text-teal-700'
                        : 'bg-gray-100 text-gray-600 hover:bg-gray-200'
                    ]"
                  >
                    {{ type }} ({{ extractedEntities.entities.filter(e => e.type === type).length }})
                  </button>
                </div>

                <!-- Entity List -->
                <div v-if="extractedEntities" class="space-y-2 max-h-64 overflow-y-auto">
                  <div
                    v-for="(entity, idx) in filterEntitiesByType(highlightedEntityType)"
                    :key="idx"
                    :class="['flex items-center gap-2 p-2 rounded-lg', getEntityColor(entity.type)]"
                  >
                    <i :class="[getEntityIcon(entity.type), 'text-sm']"></i>
                    <span class="flex-1 text-sm font-medium truncate">{{ entity.text }}</span>
                    <span class="text-xs opacity-70">{{ (entity.confidence * 100).toFixed(0) }}%</span>
                  </div>
                </div>
              </div>

              <!-- Translation Tab -->
              <div v-if="activeAITab === 'translate'" class="space-y-4">
                <!-- Language Selection -->
                <div class="grid grid-cols-3 gap-2">
                  <button
                    v-for="lang in [
                      { code: 'ar', label: 'Arabic', flag: '🇸🇦' },
                      { code: 'fr', label: 'French', flag: '🇫🇷' },
                      { code: 'es', label: 'Spanish', flag: '🇪🇸' },
                      { code: 'zh', label: 'Chinese', flag: '🇨🇳' },
                      { code: 'ja', label: 'Japanese', flag: '🇯🇵' },
                      { code: 'de', label: 'German', flag: '🇩🇪' }
                    ]"
                    :key="lang.code"
                    @click="translateDocument(lang.code as SupportedLanguage)"
                    :disabled="isLoadingTranslation"
                    :class="[
                      'px-2 py-2 rounded-lg text-xs font-medium transition-all flex flex-col items-center gap-1',
                      targetLanguage === lang.code && translatedContent
                        ? 'bg-teal-100 text-teal-700 ring-2 ring-teal-300'
                        : 'bg-gray-50 text-gray-600 hover:bg-gray-100'
                    ]"
                  >
                    <span class="text-lg">{{ lang.flag }}</span>
                    <span>{{ lang.label }}</span>
                  </button>
                </div>

                <!-- Loading State -->
                <div v-if="isLoadingTranslation" class="flex items-center justify-center py-8">
                  <div class="text-center">
                    <i class="fas fa-spinner fa-spin text-2xl text-teal-500 mb-2"></i>
                    <p class="text-sm text-gray-500">Translating document...</p>
                  </div>
                </div>

                <!-- Translation Result -->
                <div v-if="translatedContent && !isLoadingTranslation" class="space-y-3">
                  <div class="bg-blue-50 rounded-lg p-4 border border-blue-100 max-h-48 overflow-y-auto">
                    <p class="text-sm text-gray-700 whitespace-pre-line" :dir="targetLanguage === 'ar' ? 'rtl' : 'ltr'">
                      {{ translatedContent.translatedText }}
                    </p>
                  </div>
                  <div class="flex items-center justify-between text-xs text-gray-400">
                    <span>{{ (translatedContent.confidence * 100).toFixed(0) }}% confidence</span>
                    <button @click="copyTranslation" class="flex items-center gap-1 text-teal-600 hover:text-teal-700">
                      <i class="fas fa-copy"></i>
                      Copy
                    </button>
                  </div>
                </div>
              </div>

              <!-- OCR Tab -->
              <div v-if="activeAITab === 'ocr'" class="space-y-4">
                <div class="bg-amber-50 rounded-lg p-3 border border-amber-200">
                  <div class="flex items-start gap-2">
                    <i class="fas fa-info-circle text-amber-500 mt-0.5"></i>
                    <p class="text-xs text-amber-700">OCR extracts text from scanned documents, images, and PDFs that contain image-based content.</p>
                  </div>
                </div>

                <button
                  @click="performDocumentOCR"
                  :disabled="isLoadingOCR"
                  class="w-full px-4 py-2.5 bg-teal-500 hover:bg-teal-600 disabled:bg-teal-300 text-white rounded-lg font-medium flex items-center justify-center gap-2 transition-colors"
                >
                  <i :class="['fas', isLoadingOCR ? 'fa-spinner fa-spin' : 'fa-file-image']"></i>
                  {{ isLoadingOCR ? 'Processing OCR...' : 'Extract Text (OCR)' }}
                </button>

                <!-- OCR Result -->
                <div v-if="ocrResult" class="space-y-3">
                  <!-- Confidence Bar -->
                  <div class="space-y-1">
                    <div class="flex items-center justify-between text-xs">
                      <span class="text-gray-500">Confidence</span>
                      <span class="font-medium text-gray-700">{{ ocrConfidence.toFixed(0) }}%</span>
                    </div>
                    <div class="h-2 bg-gray-200 rounded-full overflow-hidden">
                      <div
                        class="h-full bg-teal-500 rounded-full transition-all duration-500"
                        :style="{ width: `${ocrConfidence}%` }"
                      ></div>
                    </div>
                  </div>

                  <!-- Stats -->
                  <div class="grid grid-cols-2 gap-2">
                    <div class="bg-gray-50 rounded-lg p-2 text-center">
                      <p class="text-lg font-bold text-gray-800">{{ ocrResult.wordCount }}</p>
                      <p class="text-xs text-gray-500">Words</p>
                    </div>
                    <div class="bg-gray-50 rounded-lg p-2 text-center">
                      <p class="text-lg font-bold text-gray-800">{{ ocrResult.characterCount?.toLocaleString() }}</p>
                      <p class="text-xs text-gray-500">Characters</p>
                    </div>
                  </div>

                  <!-- Extracted Text Preview -->
                  <div class="bg-gray-50 rounded-lg p-3 border border-gray-200 max-h-48 overflow-y-auto">
                    <p class="text-xs text-gray-700 whitespace-pre-line font-mono">{{ ocrResult.text.substring(0, 500) }}{{ ocrResult.text.length > 500 ? '...' : '' }}</p>
                  </div>

                  <!-- Page Breakdown -->
                  <div v-if="Array.isArray(ocrResult.pages) && ocrResult.pages.length" class="space-y-2">
                    <p class="text-xs font-semibold text-gray-500 uppercase">Page Breakdown</p>
                    <div class="space-y-1">
                      <div v-for="page in ocrResult.pages" :key="page.pageNumber" class="flex items-center justify-between text-xs bg-gray-50 rounded p-2">
                        <span class="text-gray-600">Page {{ page.pageNumber }}</span>
                        <span class="text-gray-400">{{ (page.confidence * 100).toFixed(0) }}% conf.</span>
                      </div>
                    </div>
                  </div>

                  <button @click="copyOCRText" class="w-full px-3 py-2 bg-gray-100 hover:bg-gray-200 text-gray-700 rounded-lg text-sm font-medium flex items-center justify-center gap-2 transition-colors">
                    <i class="fas fa-copy"></i>
                    Copy Full Text
                  </button>
                </div>
              </div>
            </div>
          </div>

          <!-- Document Details -->
          <div class="bg-white rounded-2xl shadow-sm border border-gray-100 p-6">
            <h2 class="text-lg font-semibold text-gray-900 mb-4 flex items-center gap-2">
              <i class="fas fa-info-circle text-teal-500"></i>
              Details
            </h2>
            <div class="space-y-4">
              <div class="flex items-center justify-between py-2 border-b border-gray-100">
                <span class="text-gray-500 text-sm">Size</span>
                <span class="font-medium text-gray-900">{{ document.size }}</span>
              </div>
              <div class="flex items-center justify-between py-2 border-b border-gray-100">
                <span class="text-gray-500 text-sm">Type</span>
                <span class="font-medium text-gray-900">{{ document.type }}</span>
              </div>
              <div v-if="document.pages" class="flex items-center justify-between py-2 border-b border-gray-100">
                <span class="text-gray-500 text-sm">Pages</span>
                <span class="font-medium text-gray-900">{{ document.pages }}</span>
              </div>
              <div class="flex items-center justify-between py-2 border-b border-gray-100">
                <span class="text-gray-500 text-sm">Version</span>
                <span class="font-medium text-gray-900">v{{ document.version }}</span>
              </div>
              <div class="flex items-center justify-between py-2 border-b border-gray-100">
                <span class="text-gray-500 text-sm">Library</span>
                <span class="font-medium text-teal-600">{{ document.library }}</span>
              </div>
              <div class="flex items-center justify-between py-2 border-b border-gray-100">
                <span class="text-gray-500 text-sm">Downloads</span>
                <span class="font-medium text-gray-900">{{ document.downloads.toLocaleString() }}</span>
              </div>
              <div class="flex items-center justify-between py-2">
                <span class="text-gray-500 text-sm">Views</span>
                <span class="font-medium text-gray-900">{{ document.views.toLocaleString() }}</span>
              </div>
            </div>
          </div>

          <!-- Author -->
          <div class="bg-white rounded-2xl shadow-sm border border-gray-100 p-6">
            <h2 class="text-lg font-semibold text-gray-900 mb-4 flex items-center gap-2">
              <i class="fas fa-user text-teal-500"></i>
              Author
            </h2>
            <div class="flex items-center gap-3">
              <div class="w-12 h-12 rounded-xl flex items-center justify-center text-white font-semibold"
                   :style="{ backgroundColor: document.author.color }">
                {{ document.author.initials }}
              </div>
              <div>
                <p class="font-medium text-gray-900">{{ document.author.name }}</p>
                <p class="text-sm text-gray-500">Uploaded {{ formatShortDate(document.createdAt) }}</p>
              </div>
            </div>
          </div>

          <!-- Tags -->
          <div class="bg-white rounded-2xl shadow-sm border border-gray-100 p-6">
            <h2 class="text-lg font-semibold text-gray-900 mb-4 flex items-center gap-2">
              <i class="fas fa-tags text-teal-500"></i>
              Tags
            </h2>
            <div class="flex flex-wrap gap-2">
              <span v-for="tag in document.tags" :key="tag"
                    class="px-3 py-1.5 bg-teal-50 text-teal-700 rounded-full text-sm font-medium hover:bg-teal-100 cursor-pointer transition-colors">
                #{{ tag }}
              </span>
            </div>
          </div>

          <!-- Dates -->
          <div class="bg-white rounded-2xl shadow-sm border border-gray-100 p-6">
            <h2 class="text-lg font-semibold text-gray-900 mb-4 flex items-center gap-2">
              <i class="fas fa-clock text-teal-500"></i>
              Activity
            </h2>
            <div class="space-y-3">
              <div class="flex items-start gap-3">
                <div class="w-8 h-8 rounded-lg bg-green-100 flex items-center justify-center flex-shrink-0">
                  <i class="fas fa-plus text-green-600 text-xs"></i>
                </div>
                <div>
                  <p class="text-sm font-medium text-gray-900">Created</p>
                  <p class="text-xs text-gray-500">{{ formatDate(document.createdAt) }}</p>
                </div>
              </div>
              <div class="flex items-start gap-3">
                <div class="w-8 h-8 rounded-lg bg-blue-100 flex items-center justify-center flex-shrink-0">
                  <i class="fas fa-edit text-blue-600 text-xs"></i>
                </div>
                <div>
                  <p class="text-sm font-medium text-gray-900">Last Updated</p>
                  <p class="text-xs text-gray-500">{{ formatDate(document.updatedAt) }}</p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Related Documents -->
      <div class="bg-white rounded-2xl shadow-sm border border-gray-100 p-6">
        <h2 class="text-lg font-semibold text-gray-900 mb-4 flex items-center gap-2">
          <i class="fas fa-folder-open text-teal-500"></i>
          Related Documents
        </h2>
        <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div v-for="doc in relatedDocuments" :key="doc.id"
               @click="router.push(`/documents/${doc.id}`)"
               class="flex items-center gap-4 p-4 rounded-xl border border-gray-100 hover:border-teal-200 hover:shadow-md cursor-pointer transition-all group">
            <div :class="[doc.iconBg, 'w-12 h-12 rounded-xl flex items-center justify-center flex-shrink-0 transition-transform group-hover:scale-110']">
              <i :class="[doc.icon, doc.iconColor, 'text-xl']"></i>
            </div>
            <div class="flex-1 min-w-0">
              <h4 class="font-medium text-gray-900 truncate group-hover:text-teal-600 transition-colors">{{ doc.name }}</h4>
              <p class="text-sm text-gray-500">{{ doc.size }} • {{ doc.type }}</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Version History -->
      <div class="bg-white rounded-2xl shadow-sm border border-gray-100 p-6">
        <div class="flex items-center justify-between mb-4">
          <h2 class="text-lg font-semibold text-gray-900 flex items-center gap-2">
            <i class="fas fa-history text-teal-500"></i>
            Version History
          </h2>
          <button
            @click="showVersionHistory = !showVersionHistory"
            class="text-sm text-teal-600 hover:text-teal-700"
          >
            {{ showVersionHistory ? 'Hide' : 'Show All' }}
          </button>
        </div>

        <div class="space-y-4">
          <div
            v-for="(ver, index) in (showVersionHistory ? versions : versions.slice(0, 2))"
            :key="ver.id"
            class="relative pl-6 pb-4"
            :class="{ 'border-l-2 border-gray-200': index < versions.length - 1 }"
          >
            <!-- Timeline dot -->
            <div class="absolute left-0 top-0 w-3 h-3 rounded-full -translate-x-1.5"
                 :class="index === 0 ? 'bg-teal-500' : 'bg-gray-300'"></div>

            <div class="flex items-start justify-between">
              <div>
                <div class="flex items-center gap-2">
                  <span class="font-semibold text-gray-900">v{{ ver.version }}</span>
                  <span v-if="index === 0" class="px-2 py-0.5 bg-teal-100 text-teal-700 text-xs rounded-full font-medium">Current</span>
                </div>
                <p class="text-sm text-gray-600 mt-1">{{ ver.changes }}</p>
                <p class="text-xs text-gray-400 mt-1">
                  {{ ver.author }} • {{ formatVersionDate(ver.date) }} • {{ ver.size }}
                </p>
              </div>
              <button class="text-sm text-teal-600 hover:text-teal-700 flex items-center gap-1">
                <i class="fas fa-download text-xs"></i>
                Download
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Rating Section -->
      <div class="bg-white rounded-2xl shadow-sm border border-gray-100 p-6">
        <div class="flex items-center justify-between">
          <div>
            <h2 class="text-lg font-semibold text-gray-900 mb-1">Rate this Document</h2>
            <p class="text-sm text-gray-500">Help others find useful documents</p>
          </div>
          <RatingStars
            :model-value="rating?.userRating || 0"
            :average="rating?.average"
            :count="rating?.count"
            size="lg"
            :show-count="true"
            @update:model-value="handleRating"
          />
        </div>
      </div>

      <!-- Comments Section -->
      <div class="bg-white rounded-2xl shadow-sm border border-gray-100 p-6">
        <CommentsSection
          content-type="document"
          :content-id="document.id.toString()"
          :comments="comments"
          :is-loading="commentsLoading"
          @add-comment="addComment"
        />
      </div>
    </div>

    <!-- Not Found -->
    <div v-else class="flex items-center justify-center min-h-[60vh]">
      <div class="text-center">
        <i class="fas fa-file-circle-xmark text-6xl text-gray-300 mb-4"></i>
        <h2 class="text-xl font-semibold text-gray-700 mb-2">Document Not Found</h2>
        <p class="text-gray-500 mb-6">The document you're looking for doesn't exist or has been removed.</p>
        <button @click="goBack" class="px-6 py-3 bg-teal-500 hover:bg-teal-600 text-white rounded-xl font-medium transition-colors">
          Back to Documents
        </button>
      </div>
    </div>

    <!-- Fullscreen Preview Modal -->
    <Teleport to="body">
      <div v-if="isFullscreen && document" class="fixed inset-0 z-50 bg-black/90 flex items-center justify-center p-8">
        <!-- Close Button -->
        <button @click="closeFullscreen" class="absolute top-6 right-6 w-12 h-12 bg-white/10 hover:bg-white/20 rounded-full flex items-center justify-center text-white transition-colors">
          <i class="fas fa-times text-xl"></i>
        </button>

        <!-- Document Info -->
        <div class="absolute top-6 left-6 text-white">
          <h3 class="text-xl font-semibold">{{ document.name }}</h3>
          <p class="text-white/70 text-sm">{{ document.type }} • {{ document.size }}</p>
        </div>

        <!-- Action Buttons -->
        <div class="absolute bottom-6 left-1/2 -translate-x-1/2 flex items-center gap-4">
          <button @click="downloadDocument" class="px-6 py-3 bg-teal-500 hover:bg-teal-600 text-white rounded-xl font-medium flex items-center gap-2 transition-colors">
            <i class="fas fa-download"></i>
            Download
          </button>
          <button @click="printDocument" class="px-6 py-3 bg-white/10 hover:bg-white/20 text-white rounded-xl font-medium flex items-center gap-2 transition-colors">
            <i class="fas fa-print"></i>
            Print
          </button>
          <button @click="shareDocument" class="px-6 py-3 bg-white/10 hover:bg-white/20 text-white rounded-xl font-medium flex items-center gap-2 transition-colors">
            <i class="fas fa-share-alt"></i>
            Share
          </button>
        </div>

        <!-- Preview Content -->
        <div class="bg-white rounded-2xl shadow-2xl max-w-4xl w-full aspect-[4/3] flex items-center justify-center">
          <div class="text-center p-12">
            <div :class="[document.iconBg, 'w-32 h-32 rounded-3xl flex items-center justify-center mx-auto mb-6 shadow-xl']">
              <i :class="[document.icon, document.iconColor, 'text-6xl']"></i>
            </div>
            <h4 class="text-2xl font-bold text-gray-900 mb-2">{{ document.name }}</h4>
            <p class="text-gray-500 mb-6">{{ document.description }}</p>
            <div class="flex items-center justify-center gap-6 text-sm text-gray-500">
              <span><i class="fas fa-file-alt mr-2"></i>{{ document.pages || 'N/A' }} pages</span>
              <span><i class="fas fa-hdd mr-2"></i>{{ document.size }}</span>
              <span><i class="fas fa-code-branch mr-2"></i>v{{ document.version }}</span>
            </div>
          </div>
        </div>
      </div>
    </Teleport>
  </div>
</template>

<style scoped>
.fade-in {
  animation: fadeIn 0.3s ease-out;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.hero-gradient {
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
}

/* AI Panel Animations */
.ai-panel-enter-active,
.ai-panel-leave-active {
  transition: all 0.3s ease;
}

.ai-panel-enter-from,
.ai-panel-leave-to {
  opacity: 0;
  max-height: 0;
}

.ai-panel-enter-to,
.ai-panel-leave-from {
  opacity: 1;
  max-height: 1000px;
}

/* AI Loading Shimmer */
.ai-shimmer {
  background: linear-gradient(
    90deg,
    transparent 0%,
    rgba(20, 184, 166, 0.1) 50%,
    transparent 100%
  );
  background-size: 200% 100%;
  animation: shimmer 1.5s infinite;
}

@keyframes shimmer {
  0% {
    background-position: -200% 0;
  }
  100% {
    background-position: 200% 0;
  }
}

/* Entity highlight animation */
.entity-highlight {
  animation: entityPulse 0.3s ease-out;
}

@keyframes entityPulse {
  0% {
    transform: scale(0.95);
    opacity: 0;
  }
  100% {
    transform: scale(1);
    opacity: 1;
  }
}

/* Custom scrollbar for AI panels */
.ai-scroll::-webkit-scrollbar {
  width: 4px;
}

.ai-scroll::-webkit-scrollbar-track {
  background: #f1f5f9;
  border-radius: 2px;
}

.ai-scroll::-webkit-scrollbar-thumb {
  background: #14b8a6;
  border-radius: 2px;
}

.ai-scroll::-webkit-scrollbar-thumb:hover {
  background: #0d9488;
}

/* AI Result card slide in */
.ai-result-enter-active {
  animation: slideIn 0.3s ease-out;
}

@keyframes slideIn {
  from {
    opacity: 0;
    transform: translateX(-10px);
  }
  to {
    opacity: 1;
    transform: translateX(0);
  }
}

/* Confidence bar fill animation */
.confidence-fill {
  animation: fillBar 0.8s ease-out forwards;
}

@keyframes fillBar {
  from {
    width: 0;
  }
}

/* Tab transition */
.tab-content-enter-active,
.tab-content-leave-active {
  transition: all 0.2s ease;
}

.tab-content-enter-from {
  opacity: 0;
  transform: translateY(5px);
}

.tab-content-leave-to {
  opacity: 0;
  transform: translateY(-5px);
}
</style>
