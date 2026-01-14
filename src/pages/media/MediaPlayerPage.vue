<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAIServicesStore } from '@/stores/aiServices'
import { AILoadingIndicator, AIConfidenceBar } from '@/components/ai'
import {
  CommentsSection,
  RatingStars,
  SocialShareButtons,
  RelatedContentCarousel,
  BookmarkButton,
  AddToCollectionModal
} from '@/components/common'
import { useComments } from '@/composables/useComments'
import { useRatings } from '@/composables/useRatings'
import type { SummarizationResult, TranslationResult, SupportedLanguage } from '@/types/ai'

// Text constants for localization
const textConstants = {
  // Navigation
  back: 'Back',
  mediaCenter: 'Media Center',

  // Loading
  loadingMedia: 'Loading media...',

  // Player
  video: 'Video',
  audio: 'Audio',
  views: 'views',
  like: 'Like',
  share: 'Share',
  download: 'Download',

  // Media Info
  aboutThisMedia: 'About this Media',
  contentCreator: 'Content Creator',
  relatedMedia: 'Related Media',

  // AI Features
  aiFeatures: 'AI Analysis',
  poweredBy: 'Powered by Intalio AI',
  transcript: 'Transcript',
  summary: 'Summary',
  moments: 'Moments',
  translate: 'Translate',
  generateTranscript: 'Generate Transcript',
  generating: 'Generating...',
  searchTranscript: 'Search transcript...',
  copyTranscript: 'Copy Transcript',
  generateSummary: 'Generate Summary',
  keyPoints: 'Key Points',
  confidence: 'confidence',
  copy: 'Copy',
  detectKeyMoments: 'Detect Key Moments',
  detecting: 'Detecting...',
  translateFirst: 'Generate transcript first to enable translation',
  goToTranscript: 'Go to Transcript',
  translating: 'Translating...',

  // Rating
  rateThisVideo: 'Rate this Video',
  rateThisAudio: 'Rate this Audio',
  helpOthers: 'Help others discover great content',
  collection: 'Collection',

  // Share
  shareThisVideo: 'Share this Video',
  shareThisAudio: 'Share this Audio',

  // Comments
  comments: 'Comments',
  discussion: 'Join the discussion',
}

const router = useRouter()
const route = useRoute()
const aiStore = useAIServicesStore()

// Comments
const mediaIdStr = computed(() => route.params.id as string)
const {
  comments,
  isLoading: commentsLoading,
  loadComments,
  addComment
} = useComments('media', mediaIdStr.value)

// Ratings
const { rating, submitRating, loadRating } = useRatings('media', mediaIdStr.value)

// Handle rating
async function handleRating(stars: number) {
  await submitRating(stars)
}

// State
const isLoading = ref(true)
const media = ref<any>(null)

// Add to Collection
const showAddToCollection = ref(false)
const isPlaying = ref(false)
const currentTime = ref(0)
const duration = ref(0)
const volume = ref(80)
const isMuted = ref(false)
const playbackSpeed = ref(1)
const showTranscript = ref(false)

// Mock Media Data
const mockMedia = [
  {
    id: 5,
    title: 'Saudi Arabia vs Japan: Opening Match Preview',
    description: 'In-depth preview of the highly anticipated opening match between host nation Saudi Arabia and four-time champions Japan. This comprehensive analysis covers team formations, key players to watch, tactical approaches, and predictions from expert commentators.',
    type: 'video',
    category: 'Highlights',
    duration: '12:45',
    durationSeconds: 765,
    views: '45.2K',
    likes: 2340,
    date: '2 days ago',
    thumbnail: 'https://images.unsplash.com/photo-1574629810360-7efbbe195018?w=1200&h=675&fit=crop',
    author: { name: 'AFC Media Team', avatar: 'AM' },
    tags: ['Saudi Arabia', 'Japan', 'Group A', 'Preview'],
    hasTranscript: true
  },
  {
    id: 6,
    title: 'King Fahd Stadium: Behind the Scenes Tour',
    description: 'Exclusive behind-the-scenes tour of the magnificent King Fahd International Stadium in Riyadh. Experience the state-of-the-art facilities, premium seating areas, media zones, and the pristine pitch that will host the opening match and final of AFC Asian Cup 2027.',
    type: 'video',
    category: 'Behind the Scenes',
    duration: '18:30',
    durationSeconds: 1110,
    views: '32.1K',
    likes: 1890,
    date: '3 days ago',
    thumbnail: 'https://images.unsplash.com/photo-1522778119026-d647f0596c20?w=1200&h=675&fit=crop',
    author: { name: 'Venue Team', avatar: 'VT' },
    tags: ['Stadium', 'Riyadh', 'Exclusive', 'Tour'],
    hasTranscript: true
  },
  {
    id: 18,
    title: 'AFC Asian Cup Official Podcast: Episode 1',
    description: 'Welcome to the first episode of the official AFC Asian Cup 2027 podcast! Join our expert panel as they discuss tournament preparations, team analyses, and exclusive insights from behind the scenes.',
    type: 'audio',
    category: 'Interviews',
    duration: '45:30',
    durationSeconds: 2730,
    views: '12.3K',
    likes: 567,
    date: '2 days ago',
    thumbnail: 'https://images.unsplash.com/photo-1478737270239-2f02b77fc618?w=1200&h=675&fit=crop',
    author: { name: 'AFC Media', avatar: 'AM' },
    tags: ['Podcast', 'Official', 'Analysis'],
    hasTranscript: true
  }
]

// Related Media
const relatedMedia = ref([
  { id: 7, title: 'Coach Interview: Saudi Arabia Tactics', type: 'video', duration: '25:15', views: '18.7K', thumbnail: 'https://images.unsplash.com/photo-1551958219-acbc608c6377?w=400&h=225&fit=crop' },
  { id: 8, title: 'Top 10 Goals: AFC History', type: 'video', duration: '15:20', views: '89.4K', thumbnail: 'https://images.unsplash.com/photo-1431324155629-1a6deb1dec8d?w=400&h=225&fit=crop' },
  { id: 9, title: 'Team Japan Documentary', type: 'video', duration: '22:10', views: '41.2K', thumbnail: 'https://images.unsplash.com/photo-1606925797300-0b35e9d1794e?w=400&h=225&fit=crop' },
  { id: 10, title: 'Salem Al-Dawsari Interview', type: 'video', duration: '18:30', views: '67.3K', thumbnail: 'https://images.unsplash.com/photo-1579952363873-27f3bade9f55?w=400&h=225&fit=crop' }
])

// Get media ID from route
const mediaId = computed(() => Number(route.params.id))

// ============================================================================
// AI Features State
// ============================================================================

const showAIPanel = ref(true)
const activeAITab = ref<'transcript' | 'summary' | 'moments' | 'translate'>('transcript')

// Transcript state
const transcript = ref<Array<{ time: number; text: string }>>([])
const isLoadingTranscript = ref(false)
const transcriptSearchQuery = ref('')

// Summary state
const mediaSummary = ref<SummarizationResult | null>(null)
const isLoadingSummary = ref(false)

// Key Moments state
interface KeyMoment {
  time: number
  title: string
  description: string
  type: 'highlight' | 'important' | 'chapter'
  confidence: number
}
const keyMoments = ref<KeyMoment[]>([])
const isLoadingMoments = ref(false)

// Translation state
const translatedTranscript = ref<string>('')
const isLoadingTranslation = ref(false)
const targetLanguage = ref<SupportedLanguage>('ar')

// Mock Transcripts
const mockTranscripts: Record<number, Array<{ time: number; text: string }>> = {
  5: [
    { time: 0, text: "Welcome to the AFC Asian Cup 2027 opening match preview." },
    { time: 5, text: "Today we're looking at what promises to be an incredible encounter between Saudi Arabia and Japan." },
    { time: 12, text: "The host nation Saudi Arabia comes into this tournament with high expectations." },
    { time: 20, text: "They've been preparing for this moment for years, and the home crowd will be a significant factor." },
    { time: 30, text: "Japan, on the other hand, are the most successful team in Asian Cup history with four titles." },
    { time: 40, text: "Let's take a look at the expected lineups for both teams." },
    { time: 48, text: "Saudi Arabia is expected to line up in a 4-3-3 formation." },
    { time: 55, text: "Salem Al-Dawsari will be the key player to watch for the Green Falcons." },
    { time: 65, text: "Japan will likely deploy their traditional 4-2-3-1 system." },
    { time: 75, text: "Takefusa Kubo and Kaoru Mitoma will be crucial in the attacking third." },
    { time: 85, text: "Both teams have met 15 times in history, with Japan winning 8 of those encounters." },
    { time: 95, text: "However, Saudi Arabia has shown significant improvement in recent years." },
    { time: 105, text: "The tactical battle between the two coaches will be fascinating to watch." },
    { time: 115, text: "King Fahd International Stadium will be packed with 68,000 passionate fans." },
    { time: 125, text: "This is a match that could set the tone for the entire tournament." }
  ],
  6: [
    { time: 0, text: "Welcome to our exclusive behind-the-scenes tour of King Fahd International Stadium." },
    { time: 8, text: "This magnificent venue will host the opening match and final of AFC Asian Cup 2027." },
    { time: 18, text: "Built in 1987, the stadium has undergone extensive renovations for this tournament." },
    { time: 28, text: "The current capacity stands at 68,000 spectators." },
    { time: 36, text: "Let me show you the state-of-the-art facilities we've prepared." },
    { time: 45, text: "The pitch uses hybrid grass technology, combining natural and artificial turf." },
    { time: 55, text: "This ensures optimal playing conditions throughout the tournament." },
    { time: 65, text: "The media facilities have been completely upgraded with the latest technology." },
    { time: 75, text: "We have over 500 press positions and 200 broadcast camera locations." },
    { time: 85, text: "The VIP areas feature premium hospitality suites with panoramic views." },
    { time: 95, text: "Security systems have been enhanced with facial recognition technology." },
    { time: 105, text: "The stadium also features a new LED lighting system for optimal broadcast quality." }
  ],
  18: [
    { time: 0, text: "Welcome to the official AFC Asian Cup 2027 podcast, episode one." },
    { time: 10, text: "I'm your host, and today we have an incredible panel of experts." },
    { time: 20, text: "We'll be discussing the tournament draw and early predictions." },
    { time: 30, text: "Let's start with Group A, which features host nation Saudi Arabia." },
    { time: 45, text: "This is arguably the group of death with Japan and South Korea also present." },
    { time: 60, text: "Our analysts believe this group will produce the eventual champion." },
    { time: 75, text: "Moving to Group B, we see Iran as the clear favorites." },
    { time: 90, text: "Australia's journey from Group C will be interesting to follow." },
    { time: 105, text: "They've had mixed results in recent tournaments." },
    { time: 120, text: "Let's take some questions from our listeners now." }
  ]
}

// Mock Key Moments
const mockKeyMoments: Record<number, KeyMoment[]> = {
  5: [
    { time: 0, title: 'Introduction', description: 'Preview overview and match significance', type: 'chapter', confidence: 0.98 },
    { time: 40, title: 'Team Lineups', description: 'Expected formations and key players', type: 'chapter', confidence: 0.95 },
    { time: 55, title: 'Star Player: Al-Dawsari', description: 'Analysis of Salem Al-Dawsari\'s impact', type: 'highlight', confidence: 0.92 },
    { time: 75, title: 'Japan\'s Key Players', description: 'Kubo and Mitoma tactical roles', type: 'highlight', confidence: 0.91 },
    { time: 85, title: 'Head-to-Head History', description: 'Historical record between the teams', type: 'important', confidence: 0.94 },
    { time: 115, title: 'Stadium Atmosphere', description: 'Home crowd advantage discussion', type: 'chapter', confidence: 0.89 }
  ],
  6: [
    { time: 0, title: 'Stadium Introduction', description: 'Overview of King Fahd International Stadium', type: 'chapter', confidence: 0.97 },
    { time: 28, title: 'Stadium Capacity', description: '68,000 seat capacity details', type: 'important', confidence: 0.95 },
    { time: 45, title: 'Pitch Technology', description: 'Hybrid grass technology explained', type: 'highlight', confidence: 0.93 },
    { time: 65, title: 'Media Facilities', description: 'Press and broadcast infrastructure', type: 'chapter', confidence: 0.91 },
    { time: 85, title: 'VIP Experience', description: 'Premium hospitality features', type: 'chapter', confidence: 0.88 },
    { time: 95, title: 'Security Systems', description: 'Advanced security measures', type: 'important', confidence: 0.90 }
  ],
  18: [
    { time: 0, title: 'Podcast Introduction', description: 'Welcome and panel introduction', type: 'chapter', confidence: 0.96 },
    { time: 30, title: 'Group A Analysis', description: 'Group of death discussion', type: 'highlight', confidence: 0.94 },
    { time: 60, title: 'Championship Prediction', description: 'Group A to produce winner', type: 'important', confidence: 0.92 },
    { time: 75, title: 'Group B Preview', description: 'Iran as favorites', type: 'chapter', confidence: 0.89 },
    { time: 90, title: 'Australia\'s Chances', description: 'Analysis of Socceroos prospects', type: 'chapter', confidence: 0.87 },
    { time: 120, title: 'Q&A Session', description: 'Listener questions', type: 'chapter', confidence: 0.85 }
  ]
}

// ============================================================================
// Load Data
// ============================================================================

onMounted(async () => {
  setTimeout(async () => {
    media.value = mockMedia.find(m => m.id === mediaId.value) || mockMedia[0]
    duration.value = media.value?.durationSeconds || 0
    isLoading.value = false

    // Load comments and ratings
    await Promise.all([
      loadComments(),
      loadRating()
    ])
  }, 500)
})

// ============================================================================
// Player Controls
// ============================================================================

function togglePlay() {
  isPlaying.value = !isPlaying.value
}

function toggleMute() {
  isMuted.value = !isMuted.value
}

function setPlaybackSpeed(speed: number) {
  playbackSpeed.value = speed
}

function seekTo(time: number) {
  currentTime.value = Math.max(0, Math.min(time, duration.value))
}

function formatTime(seconds: number): string {
  const mins = Math.floor(seconds / 60)
  const secs = Math.floor(seconds % 60)
  return `${mins}:${secs.toString().padStart(2, '0')}`
}

function goBack() {
  router.push({ name: 'MediaCenter' })
}

function goToRelated(id: number) {
  router.push({ name: 'MediaPlayer', params: { id: id.toString() } })
}

function handleLike() {
  if (media.value) {
    media.value.likes = (media.value.likes || 0) + 1
  }
}

// ============================================================================
// AI Functions
// ============================================================================

async function generateTranscript() {
  if (isLoadingTranscript.value || !media.value) return

  isLoadingTranscript.value = true
  transcript.value = []

  try {
    await new Promise(resolve => setTimeout(resolve, 1500))
    transcript.value = mockTranscripts[media.value.id] || [
      { time: 0, text: 'Transcript auto-generated by AI.' },
      { time: 10, text: 'Content analysis in progress...' },
      { time: 20, text: 'This is a sample transcript for demonstration.' }
    ]
  } finally {
    isLoadingTranscript.value = false
  }
}

async function generateSummary() {
  if (isLoadingSummary.value || !media.value) return

  isLoadingSummary.value = true
  mediaSummary.value = null

  try {
    await new Promise(resolve => setTimeout(resolve, 1200))

    const summaries: Record<number, SummarizationResult> = {
      5: {
        summary: 'This preview analyzes the highly anticipated AFC Asian Cup 2027 opening match between host nation Saudi Arabia and four-time champions Japan. The analysis covers expected team formations (Saudi Arabia\'s 4-3-3 vs Japan\'s 4-2-3-1), key players including Salem Al-Dawsari and Takefusa Kubo, historical head-to-head record (Japan leads 8-15), and the significance of home advantage at King Fahd Stadium.',
        keyPoints: [
          'Saudi Arabia deploys 4-3-3 formation with Al-Dawsari as key player',
          'Japan uses 4-2-3-1 with Kubo and Mitoma in attack',
          'Historical record favors Japan with 8 wins in 15 meetings',
          '68,000 capacity King Fahd Stadium provides home advantage',
          'Match could set the tone for the entire tournament'
        ],
        wordCount: 1250,
        processingTime: 1.2,
        confidence: 0.94
      },
      6: {
        summary: 'An exclusive tour of King Fahd International Stadium showcasing the extensive renovations for AFC Asian Cup 2027. The 68,000-capacity venue features hybrid grass technology, upgraded media facilities with 500+ press positions, premium VIP hospitality suites, and advanced security systems including facial recognition. The stadium will host both the opening match and final.',
        keyPoints: [
          'Stadium capacity: 68,000 spectators',
          'Hybrid grass technology combining natural and artificial turf',
          '500+ press positions and 200 broadcast camera locations',
          'Premium VIP hospitality suites with panoramic views',
          'Enhanced security with facial recognition technology',
          'New LED lighting system for broadcast quality'
        ],
        wordCount: 980,
        processingTime: 1.1,
        confidence: 0.96
      },
      18: {
        summary: 'The inaugural AFC Asian Cup 2027 official podcast features expert panel discussion on the tournament draw and predictions. Key insights include Group A being labeled the "group of death" with Saudi Arabia, Japan, and South Korea, predictions that the eventual champion will emerge from this group, Iran\'s favorite status in Group B, and analysis of Australia\'s mixed recent form.',
        keyPoints: [
          'Group A identified as "group of death"',
          'Prediction: Champion will come from Group A',
          'Iran favorites in Group B',
          'Australia\'s form questioned for Group C',
          'Listener Q&A session included'
        ],
        wordCount: 2100,
        processingTime: 1.4,
        confidence: 0.91
      }
    }

    mediaSummary.value = summaries[media.value.id] || {
      summary: `AI-generated summary of "${media.value.title}": ${media.value.description}`,
      keyPoints: ['Key information extracted', 'Main topics identified', 'Content analyzed'],
      wordCount: 500,
      processingTime: 1.0,
      confidence: 0.85
    }
  } finally {
    isLoadingSummary.value = false
  }
}

async function detectKeyMoments() {
  if (isLoadingMoments.value || !media.value) return

  isLoadingMoments.value = true
  keyMoments.value = []

  try {
    await new Promise(resolve => setTimeout(resolve, 1800))
    keyMoments.value = mockKeyMoments[media.value.id] || [
      { time: 0, title: 'Introduction', description: 'Content begins', type: 'chapter', confidence: 0.9 },
      { time: Math.floor(duration.value / 2), title: 'Main Content', description: 'Core discussion', type: 'chapter', confidence: 0.85 }
    ]
  } finally {
    isLoadingMoments.value = false
  }
}

async function translateTranscript(lang: SupportedLanguage) {
  if (isLoadingTranslation.value || transcript.value.length === 0) return

  targetLanguage.value = lang
  isLoadingTranslation.value = true
  translatedTranscript.value = ''

  try {
    await new Promise(resolve => setTimeout(resolve, 1500))

    const translations: Partial<Record<SupportedLanguage, string>> = {
      ar: 'مرحبا بكم في معاينة مباراة افتتاح كأس آسيا 2027. اليوم نستعرض ما يعد بأن يكون لقاء رائعا بين المملكة العربية السعودية واليابان...',
      fr: 'Bienvenue dans l\'aperçu du match d\'ouverture de la Coupe d\'Asie 2027. Aujourd\'hui, nous examinons ce qui promet d\'être une rencontre incroyable entre l\'Arabie saoudite et le Japon...',
      es: 'Bienvenidos a la previa del partido inaugural de la Copa Asiática 2027. Hoy analizamos lo que promete ser un encuentro increíble entre Arabia Saudita y Japón...',
      zh: '欢迎收看2027年亚洲杯揭幕战预告。今天我们将分析沙特阿拉伯和日本之间这场令人期待的比赛...',
      ja: 'AFCアジアカップ2027開幕戦プレビューへようこそ。今日はサウジアラビアと日本の素晴らしい対戦を見ていきます...',
      de: 'Willkommen zur Vorschau auf das Eröffnungsspiel des Asien-Pokals 2027. Heute betrachten wir das vielversprechende Aufeinandertreffen zwischen Saudi-Arabien und Japan...',
      en: transcript.value.map(t => t.text).join(' ')
    }

    translatedTranscript.value = translations[lang] ?? ''
  } finally {
    isLoadingTranslation.value = false
  }
}

function jumpToMoment(time: number) {
  seekTo(time)
}

function getTranscriptAtTime(time: number): string {
  const entry = transcript.value.find((t, i) => {
    const nextTime = transcript.value[i + 1]?.time || Infinity
    return time >= t.time && time < nextTime
  })
  return entry?.text || ''
}

const filteredTranscript = computed(() => {
  if (!transcriptSearchQuery.value) return transcript.value
  const q = transcriptSearchQuery.value.toLowerCase()
  return transcript.value.filter(t => t.text.toLowerCase().includes(q))
})

function toggleAIPanel() {
  showAIPanel.value = !showAIPanel.value
}

function getMomentTypeIcon(type: string): string {
  const icons: Record<string, string> = {
    chapter: 'fas fa-bookmark',
    highlight: 'fas fa-star',
    important: 'fas fa-exclamation-circle'
  }
  return icons[type] || 'fas fa-circle'
}

function getMomentTypeColor(type: string): string {
  const colors: Record<string, string> = {
    chapter: 'text-blue-500 bg-blue-50',
    highlight: 'text-amber-500 bg-amber-50',
    important: 'text-rose-500 bg-rose-50'
  }
  return colors[type] || 'text-gray-500 bg-gray-50'
}

function copyTranscript() {
  const text = transcript.value.map(t => `[${formatTime(t.time)}] ${t.text}`).join('\n')
  navigator.clipboard.writeText(text)
  alert('Transcript copied to clipboard!')
}

function copySummary() {
  if (mediaSummary.value) {
    navigator.clipboard.writeText(mediaSummary.value.summary)
    alert('Summary copied to clipboard!')
  }
}
</script>

<template>
  <div class="media-player-page min-h-screen bg-gray-50">
    <!-- Loading State -->
    <div v-if="isLoading" class="flex items-center justify-center min-h-[60vh]">
      <div class="text-center">
        <div class="w-12 h-12 border-4 border-teal-500 border-t-transparent rounded-full animate-spin mx-auto mb-4"></div>
        <p class="text-gray-500">{{ textConstants.loadingMedia }}</p>
      </div>
    </div>

    <!-- Media Content -->
    <div v-else-if="media" class="page-view fade-in">
      <!-- Hero Header -->
      <div class="hero-gradient relative overflow-hidden rounded-2xl mb-6">
        <!-- Decorative circles -->
        <div class="absolute top-0 right-0 w-64 h-64 bg-white/5 rounded-full -translate-y-1/2 translate-x-1/4"></div>
        <div class="absolute bottom-0 left-0 w-48 h-48 bg-white/5 rounded-full translate-y-1/2 -translate-x-1/4"></div>
        <div class="absolute top-1/2 right-1/3 w-32 h-32 bg-white/10 rounded-full"></div>

        <div class="relative z-10 px-8 py-6">
          <!-- Navigation Row -->
          <div class="header-nav">
            <button @click="goBack" class="back-btn">
              <i class="fas fa-arrow-left"></i>
              <span>{{ textConstants.back }}</span>
            </button>
            <div class="breadcrumb">
              <router-link to="/media" class="breadcrumb-link">
                <i class="fas fa-photo-video"></i>
                {{ textConstants.mediaCenter }}
              </router-link>
              <i class="fas fa-chevron-right breadcrumb-sep"></i>
              <span class="breadcrumb-current">{{ media.title }}</span>
            </div>
          </div>

          <!-- Title and actions row -->
          <div class="flex items-center justify-between flex-wrap gap-4">
            <div class="flex items-center gap-4">
              <div class="w-14 h-14 rounded-xl shadow-lg flex items-center justify-center bg-white/20">
                <i :class="[media.type === 'video' ? 'fas fa-video' : 'fas fa-headphones', 'text-white text-2xl']"></i>
              </div>
              <div>
                <h1 class="text-2xl md:text-3xl font-bold text-white">{{ media.title }}</h1>
                <p class="text-white/70">{{ media.category }} • {{ media.duration }} • {{ media.views }} {{ textConstants.views }}</p>
              </div>
            </div>

            <!-- Action buttons -->
            <div class="flex items-center gap-3">
              <button @click="handleLike" class="px-4 py-2 bg-white text-gray-700 rounded-xl font-medium hover:bg-gray-50 transition-all shadow-sm flex items-center gap-2">
                <i class="fas fa-heart"></i>
                <span>{{ textConstants.like }}</span>
              </button>
              <button class="px-4 py-2 bg-transparent text-white border border-white/30 rounded-xl font-medium hover:bg-white/10 transition-all flex items-center gap-2">
                <i class="fas fa-download"></i>
                <span class="hidden sm:inline">{{ textConstants.download }}</span>
              </button>
              <button class="px-4 py-2 bg-transparent text-white border border-white/30 rounded-xl font-medium hover:bg-white/10 transition-all flex items-center gap-2">
                <i class="fas fa-share-alt"></i>
                <span class="hidden sm:inline">{{ textConstants.share }}</span>
              </button>
              <BookmarkButton
                :content-id="media.id.toString()"
                content-type="media"
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
        <!-- Main Content -->
        <div class="lg:col-span-2 space-y-6">
          <!-- Video/Audio Player -->
          <div class="bg-black rounded-2xl overflow-hidden shadow-2xl">
            <!-- Video Display -->
            <div class="relative aspect-video">
              <img :src="media.thumbnail" :alt="media.title" class="absolute inset-0 w-full h-full object-cover" />
              <div class="absolute inset-0 bg-black/30"></div>

              <!-- Play Button Overlay -->
              <div class="absolute inset-0 flex items-center justify-center">
                <button @click="togglePlay" class="w-20 h-20 bg-white/20 backdrop-blur-sm rounded-full flex items-center justify-center hover:bg-white/30 transition-all hover:scale-110">
                  <i :class="['text-white text-3xl', isPlaying ? 'fas fa-pause' : 'fas fa-play ml-1']"></i>
                </button>
              </div>

              <!-- Media Type Badge -->
              <div class="absolute top-4 left-4">
                <span class="media-type-badge">
                  <i :class="media.type === 'video' ? 'fas fa-video' : 'fas fa-headphones'"></i>
                  {{ media.type === 'video' ? textConstants.video : textConstants.audio }}
                </span>
              </div>

              <!-- Current Transcript Line -->
              <div v-if="transcript.length > 0 && showTranscript" class="absolute bottom-20 left-4 right-4">
                <div class="bg-black/80 backdrop-blur-sm text-white text-center py-3 px-4 rounded-lg">
                  {{ getTranscriptAtTime(currentTime) || '...' }}
                </div>
              </div>
            </div>

            <!-- Player Controls -->
            <div class="bg-gray-900 px-4 py-3">
              <!-- Progress Bar -->
              <div class="flex items-center gap-3 mb-3">
                <span class="text-white text-xs w-12">{{ formatTime(currentTime) }}</span>
                <div class="flex-1 h-1 bg-gray-700 rounded-full cursor-pointer group" @click="seekTo(duration * ($event.offsetX / ($event.target as HTMLElement).clientWidth))">
                  <div class="h-full bg-teal-500 rounded-full relative" :style="{ width: `${(currentTime / duration) * 100}%` }">
                    <div class="absolute right-0 top-1/2 -translate-y-1/2 w-3 h-3 bg-white rounded-full shadow opacity-0 group-hover:opacity-100 transition-opacity"></div>
                  </div>
                </div>
                <span class="text-white text-xs w-12 text-right">{{ formatTime(duration) }}</span>
              </div>

              <!-- Control Buttons -->
              <div class="flex items-center justify-between">
                <div class="flex items-center gap-3">
                  <button @click="togglePlay" class="w-10 h-10 bg-teal-500 hover:bg-teal-600 rounded-full flex items-center justify-center text-white transition-colors">
                    <i :class="isPlaying ? 'fas fa-pause' : 'fas fa-play ml-0.5'"></i>
                  </button>
                  <button @click="seekTo(currentTime - 10)" class="text-gray-400 hover:text-white transition-colors">
                    <i class="fas fa-backward text-lg"></i>
                  </button>
                  <button @click="seekTo(currentTime + 10)" class="text-gray-400 hover:text-white transition-colors">
                    <i class="fas fa-forward text-lg"></i>
                  </button>
                  <button @click="toggleMute" class="text-gray-400 hover:text-white transition-colors">
                    <i :class="isMuted ? 'fas fa-volume-mute' : 'fas fa-volume-up'"></i>
                  </button>
                </div>

                <div class="flex items-center gap-3">
                  <!-- Playback Speed -->
                  <select v-model="playbackSpeed" class="bg-gray-800 text-white text-sm rounded px-2 py-1 border-none">
                    <option :value="0.5">0.5x</option>
                    <option :value="1">1x</option>
                    <option :value="1.25">1.25x</option>
                    <option :value="1.5">1.5x</option>
                    <option :value="2">2x</option>
                  </select>
                  <button @click="showTranscript = !showTranscript" :class="['transition-colors', showTranscript ? 'text-teal-400' : 'text-gray-400 hover:text-white']">
                    <i class="fas fa-closed-captioning"></i>
                  </button>
                  <button class="text-gray-400 hover:text-white transition-colors">
                    <i class="fas fa-expand"></i>
                  </button>
                </div>
              </div>
            </div>
          </div>

          <!-- Media Info -->
          <div class="bg-white rounded-2xl shadow-sm border border-gray-100 p-6">
            <h2 class="text-lg font-semibold text-gray-900 mb-3 flex items-center gap-2">
              <i class="fas fa-info-circle text-teal-500"></i>
              {{ textConstants.aboutThisMedia }}
            </h2>
            <p class="text-gray-600 mb-4 leading-relaxed">{{ media.description }}</p>

            <!-- Stats Row -->
            <div class="flex flex-wrap gap-6 mb-4 pb-4 border-b border-gray-100">
              <div class="flex items-center gap-2 text-sm text-gray-500">
                <i class="fas fa-eye text-teal-500"></i>
                <span>{{ media.views }} {{ textConstants.views }}</span>
              </div>
              <div class="flex items-center gap-2 text-sm text-gray-500">
                <i class="fas fa-clock text-teal-500"></i>
                <span>{{ media.date }}</span>
              </div>
              <div class="flex items-center gap-2 text-sm text-gray-500">
                <i class="fas fa-heart text-red-400"></i>
                <span>{{ media.likes }} likes</span>
              </div>
            </div>

            <!-- Tags -->
            <div class="flex flex-wrap gap-2 mb-4">
              <span v-for="tag in media.tags" :key="tag" class="px-3 py-1.5 bg-teal-50 text-teal-700 rounded-full text-sm font-medium">
                #{{ tag }}
              </span>
            </div>

            <!-- Author & Rating -->
            <div class="flex items-center justify-between flex-wrap gap-4 pt-4 border-t border-gray-100">
              <div class="flex items-center gap-3">
                <div class="w-10 h-10 rounded-full bg-gradient-to-br from-teal-500 to-teal-600 flex items-center justify-center text-white font-semibold text-sm">
                  {{ media.author.avatar }}
                </div>
                <div>
                  <p class="font-medium text-gray-900">{{ media.author.name }}</p>
                  <p class="text-sm text-gray-500">{{ textConstants.contentCreator }}</p>
                </div>
              </div>
              <RatingStars
                :model-value="rating?.userRating || 0"
                :average="rating?.average"
                :count="rating?.count"
                size="md"
                :show-count="true"
                @update:model-value="handleRating"
              />
            </div>
          </div>

          <!-- Related Media -->
          <div class="bg-white rounded-2xl shadow-sm border border-gray-100 p-6">
            <h2 class="text-lg font-semibold text-gray-900 mb-4 flex items-center gap-2">
              <i class="fas fa-play-circle text-teal-500"></i>
              {{ textConstants.relatedMedia }}
            </h2>
            <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <div v-for="item in relatedMedia" :key="item.id" @click="goToRelated(item.id)" class="group cursor-pointer rounded-xl overflow-hidden border border-gray-100 hover:border-teal-200 hover:shadow-md transition-all">
                <div class="relative aspect-video">
                  <img :src="item.thumbnail" :alt="item.title" class="w-full h-full object-cover" />
                  <div class="absolute inset-0 bg-black/20 group-hover:bg-black/40 transition-colors flex items-center justify-center">
                    <div class="w-12 h-12 rounded-full bg-white/20 backdrop-blur-sm flex items-center justify-center opacity-0 group-hover:opacity-100 transition-opacity">
                      <i class="fas fa-play text-white ml-1"></i>
                    </div>
                  </div>
                  <span class="absolute bottom-2 right-2 px-2 py-0.5 bg-black/70 text-white text-xs rounded">{{ item.duration }}</span>
                </div>
                <div class="p-3">
                  <h4 class="font-medium text-gray-900 text-sm line-clamp-2 group-hover:text-teal-600 transition-colors">{{ item.title }}</h4>
                  <p class="text-xs text-gray-500 mt-1">{{ item.views }} {{ textConstants.views }}</p>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- AI Sidebar -->
        <div class="space-y-6">
          <!-- AI Features Panel -->
          <div class="bg-white rounded-2xl shadow-sm border border-gray-100 overflow-hidden">
            <!-- AI Panel Header -->
            <div class="px-6 py-4 bg-gradient-to-r from-teal-50 to-transparent border-b border-teal-100">
              <div class="flex items-center justify-between">
                <div class="flex items-center gap-3">
                  <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-teal-500 to-teal-600 flex items-center justify-center">
                    <i class="fas fa-wand-magic-sparkles text-white"></i>
                  </div>
                  <div>
                    <h3 class="text-lg font-semibold text-gray-900">{{ textConstants.aiFeatures }}</h3>
                    <p class="text-sm text-gray-500">{{ textConstants.poweredBy }}</p>
                  </div>
                </div>
                <button @click="toggleAIPanel" class="w-8 h-8 rounded-lg hover:bg-gray-100 text-gray-400 hover:text-gray-600 flex items-center justify-center transition-all">
                  <i :class="['fas transition-transform duration-300', showAIPanel ? 'fa-chevron-up' : 'fa-chevron-down']"></i>
                </button>
              </div>
            </div>

            <!-- AI Panel Content -->
            <div v-if="showAIPanel" class="p-4 space-y-4">
              <!-- AI Tab Navigation -->
              <div class="grid grid-cols-4 gap-2">
                <button
                  v-for="tab in [
                    { id: 'transcript', icon: 'fas fa-closed-captioning', label: textConstants.transcript },
                    { id: 'summary', icon: 'fas fa-file-lines', label: textConstants.summary },
                    { id: 'moments', icon: 'fas fa-bookmark', label: textConstants.moments },
                    { id: 'translate', icon: 'fas fa-language', label: textConstants.translate }
                  ]"
                  :key="tab.id"
                  @click="activeAITab = tab.id as any"
                  :class="[
                    'px-3 py-2.5 rounded-xl text-xs font-medium transition-all flex flex-col items-center gap-1.5',
                    activeAITab === tab.id
                      ? 'bg-teal-50 text-teal-700 border-2 border-teal-200'
                      : 'bg-gray-50 text-gray-600 border-2 border-transparent hover:bg-gray-100'
                  ]"
                >
                  <i :class="[tab.icon, 'text-base']"></i>
                  <span>{{ tab.label }}</span>
                </button>
              </div>

              <!-- Transcript Tab -->
              <div v-if="activeAITab === 'transcript'" class="space-y-4">
                <button v-if="transcript.length === 0" @click="generateTranscript" :disabled="isLoadingTranscript" class="w-full px-4 py-2.5 bg-teal-500 hover:bg-teal-600 disabled:bg-teal-300 text-white rounded-lg font-medium flex items-center justify-center gap-2 transition-colors">
                  <i :class="['fas', isLoadingTranscript ? 'fa-spinner fa-spin' : 'fa-closed-captioning']"></i>
                  {{ isLoadingTranscript ? 'Generating...' : 'Generate Transcript' }}
                </button>

                <div v-if="transcript.length > 0" class="space-y-3">
                  <!-- Search -->
                  <div class="relative">
                    <i class="fas fa-search absolute left-3 top-1/2 -translate-y-1/2 text-gray-400 text-sm"></i>
                    <input v-model="transcriptSearchQuery" type="text" placeholder="Search transcript..." class="w-full pl-9 pr-4 py-2 bg-gray-50 border border-gray-200 rounded-lg text-sm focus:ring-2 focus:ring-teal-500 focus:border-transparent" />
                  </div>

                  <!-- Transcript List -->
                  <div class="max-h-64 overflow-y-auto space-y-2">
                    <div v-for="(entry, idx) in filteredTranscript" :key="idx" @click="jumpToMoment(entry.time)" class="flex gap-2 p-2 rounded-lg hover:bg-teal-50 cursor-pointer transition-colors group">
                      <span class="text-xs text-teal-600 font-mono w-10 flex-shrink-0">{{ formatTime(entry.time) }}</span>
                      <p class="text-sm text-gray-700 group-hover:text-gray-900">{{ entry.text }}</p>
                    </div>
                  </div>

                  <!-- Copy Button -->
                  <button @click="copyTranscript" class="w-full px-3 py-2 bg-gray-100 hover:bg-gray-200 text-gray-700 rounded-lg text-sm font-medium flex items-center justify-center gap-2 transition-colors">
                    <i class="fas fa-copy"></i>
                    Copy Transcript
                  </button>
                </div>
              </div>

              <!-- Summary Tab -->
              <div v-if="activeAITab === 'summary'" class="space-y-4">
                <button @click="generateSummary" :disabled="isLoadingSummary" class="w-full px-4 py-2.5 bg-teal-500 hover:bg-teal-600 disabled:bg-teal-300 text-white rounded-lg font-medium flex items-center justify-center gap-2 transition-colors">
                  <i :class="['fas', isLoadingSummary ? 'fa-spinner fa-spin' : 'fa-wand-magic-sparkles']"></i>
                  {{ isLoadingSummary ? 'Generating...' : 'Generate Summary' }}
                </button>

                <Transition name="slide-fade">
                  <div v-if="mediaSummary" class="space-y-3 pt-3 border-t border-gray-100">
                    <div class="p-3 bg-teal-50 rounded-xl">
                      <div class="flex items-center justify-between mb-2">
                        <span class="text-sm font-medium text-teal-700">{{ textConstants.summary }}</span>
                        <button @click="mediaSummary = null" class="text-teal-400 hover:text-teal-600">
                          <i class="fas fa-times text-sm"></i>
                        </button>
                      </div>
                      <p class="text-sm text-teal-800 leading-relaxed">{{ mediaSummary.summary }}</p>
                    </div>

                    <!-- Key Points -->
                    <div v-if="mediaSummary.keyPoints?.length" class="p-3 bg-gray-50 rounded-xl">
                      <div class="flex items-center gap-2 mb-2">
                        <i class="fas fa-lightbulb text-amber-500"></i>
                        <span class="text-sm font-medium text-gray-700">{{ textConstants.keyPoints }}</span>
                      </div>
                      <ul class="space-y-1.5">
                        <li v-for="(point, idx) in mediaSummary.keyPoints" :key="idx" class="flex items-start gap-2 text-sm text-gray-600">
                          <i class="fas fa-check-circle text-teal-500 mt-0.5 flex-shrink-0"></i>
                          <span>{{ point }}</span>
                        </li>
                      </ul>
                    </div>

                    <!-- Confidence & Copy -->
                    <div class="flex items-center justify-between text-xs text-gray-400">
                      <span>{{ ((mediaSummary.confidence ?? 0) * 100).toFixed(0) }}% {{ textConstants.confidence }}</span>
                      <button @click="copySummary" class="flex items-center gap-1 text-teal-600 hover:text-teal-700">
                        <i class="fas fa-copy"></i>
                        {{ textConstants.copy }}
                      </button>
                    </div>
                  </div>
                </Transition>
              </div>

              <!-- Key Moments Tab -->
              <div v-if="activeAITab === 'moments'" class="space-y-4">
                <button @click="detectKeyMoments" :disabled="isLoadingMoments" class="w-full px-4 py-2.5 bg-teal-500 hover:bg-teal-600 disabled:bg-teal-300 text-white rounded-lg font-medium flex items-center justify-center gap-2 transition-colors">
                  <i :class="['fas', isLoadingMoments ? 'fa-spinner fa-spin' : 'fa-bookmark']"></i>
                  {{ isLoadingMoments ? 'Detecting...' : 'Detect Key Moments' }}
                </button>

                <div v-if="keyMoments.length > 0" class="space-y-2 max-h-80 overflow-y-auto">
                  <div v-for="(moment, idx) in keyMoments" :key="idx" @click="jumpToMoment(moment.time)" class="p-3 rounded-lg border border-gray-100 hover:border-teal-200 hover:bg-teal-50/50 cursor-pointer transition-all group">
                    <div class="flex items-center gap-2 mb-1">
                      <span :class="['w-6 h-6 rounded-full flex items-center justify-center', getMomentTypeColor(moment.type)]">
                        <i :class="[getMomentTypeIcon(moment.type), 'text-xs']"></i>
                      </span>
                      <span class="text-xs font-mono text-teal-600">{{ formatTime(moment.time) }}</span>
                      <span class="text-xs text-gray-400 capitalize">{{ moment.type }}</span>
                    </div>
                    <h4 class="font-medium text-gray-900 text-sm group-hover:text-teal-600">{{ moment.title }}</h4>
                    <p class="text-xs text-gray-500 mt-0.5">{{ moment.description }}</p>
                  </div>
                </div>
              </div>

              <!-- Translate Tab -->
              <div v-if="activeAITab === 'translate'" class="space-y-4">
                <div v-if="transcript.length === 0" class="text-center py-4">
                  <p class="text-sm text-gray-500 mb-2">Generate transcript first to enable translation</p>
                  <button @click="activeAITab = 'transcript'" class="text-teal-600 text-sm font-medium hover:text-teal-700">
                    Go to Transcript
                  </button>
                </div>

                <div v-else class="space-y-4">
                  <!-- Language Selection -->
                  <div class="grid grid-cols-3 gap-2">
                    <button v-for="lang in [
                      { code: 'ar', label: 'Arabic', flag: '🇸🇦' },
                      { code: 'fr', label: 'French', flag: '🇫🇷' },
                      { code: 'es', label: 'Spanish', flag: '🇪🇸' },
                      { code: 'zh', label: 'Chinese', flag: '🇨🇳' },
                      { code: 'ja', label: 'Japanese', flag: '🇯🇵' },
                      { code: 'de', label: 'German', flag: '🇩🇪' }
                    ]" :key="lang.code" @click="translateTranscript(lang.code as SupportedLanguage)" :disabled="isLoadingTranslation" :class="[
                      'px-2 py-2 rounded-lg text-xs font-medium transition-all flex flex-col items-center gap-1',
                      targetLanguage === lang.code && translatedTranscript ? 'bg-teal-100 text-teal-700 ring-2 ring-teal-300' : 'bg-gray-50 text-gray-600 hover:bg-gray-100'
                    ]">
                      <span class="text-lg">{{ lang.flag }}</span>
                      <span>{{ lang.label }}</span>
                    </button>
                  </div>

                  <!-- Loading -->
                  <div v-if="isLoadingTranslation" class="flex items-center justify-center py-6">
                    <div class="text-center">
                      <i class="fas fa-spinner fa-spin text-2xl text-teal-500 mb-2"></i>
                      <p class="text-sm text-gray-500">Translating...</p>
                    </div>
                  </div>

                  <!-- Translation Result -->
                  <div v-if="translatedTranscript && !isLoadingTranslation" class="bg-blue-50 rounded-lg p-4 border border-blue-100 max-h-48 overflow-y-auto">
                    <p class="text-sm text-gray-700" :dir="targetLanguage === 'ar' ? 'rtl' : 'ltr'">
                      {{ translatedTranscript }}
                    </p>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Engagement & Share Section -->
      <div class="bg-white rounded-2xl shadow-sm border border-gray-100 p-6 mt-6">
        <div class="flex items-center justify-between flex-wrap gap-4">
          <div class="flex items-center gap-3">
            <BookmarkButton
              :content-id="media.id.toString()"
              content-type="media"
              size="md"
              variant="button"
              :show-label="true"
            />
            <button
              @click="showAddToCollection = true"
              class="px-4 py-2 bg-gray-100 hover:bg-gray-200 text-gray-700 rounded-xl font-medium flex items-center gap-2 transition-colors"
              title="Add to Collection"
            >
              <i class="fas fa-folder-plus"></i>
              <span>{{ textConstants.collection }}</span>
            </button>
          </div>
          <SocialShareButtons
            :title="media.title"
            :description="media.description"
            layout="horizontal"
            size="sm"
          />
        </div>
      </div>

      <!-- Comments Section -->
      <div class="bg-white rounded-2xl shadow-sm border border-gray-100 p-6 mt-6">
        <CommentsSection
          content-type="media"
          :content-id="media.id.toString()"
          :comments="comments"
          :is-loading="commentsLoading"
          @add-comment="addComment"
        />
      </div>
    </div>

    <!-- Not Found -->
    <div v-else class="flex items-center justify-center min-h-[60vh]">
      <div class="text-center">
        <i class="fas fa-video-slash text-6xl text-gray-300 mb-4"></i>
        <h2 class="text-xl font-semibold text-gray-700 mb-2">Media Not Found</h2>
        <p class="text-gray-500 mb-6">The media you're looking for doesn't exist.</p>
        <button @click="goBack" class="px-6 py-3 bg-teal-500 hover:bg-teal-600 text-white rounded-xl font-medium transition-colors">
          Back to Media Center
        </button>
      </div>
    </div>

    <!-- Add to Collection Modal -->
    <AddToCollectionModal
      v-if="media"
      :show="showAddToCollection"
      content-type="media"
      :content-id="media.id.toString()"
      :content-title="media.title"
      :content-thumbnail="media.thumbnail"
      @close="showAddToCollection = false"
      @added="showAddToCollection = false"
    />
  </div>
</template>

<style scoped>
/* Page Animation */
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

.fade-in-up {
  animation: fadeInUp 0.4s ease-out forwards;
  opacity: 0;
}

@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

/* Hero Gradient Header */
.hero-gradient {
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
}

/* Navigation */
.header-nav {
  display: flex;
  align-items: center;
  gap: 1rem;
  margin-bottom: 1.5rem;
}

.back-btn {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem 1rem;
  background: rgba(255, 255, 255, 0.15);
  border: 1px solid rgba(255, 255, 255, 0.2);
  border-radius: 0.75rem;
  color: white;
  font-size: 0.875rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s ease;
}

.back-btn:hover {
  background: rgba(255, 255, 255, 0.25);
  transform: translateX(-2px);
}

.breadcrumb {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.875rem;
}

.breadcrumb-link {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  color: rgba(255, 255, 255, 0.8);
  text-decoration: none;
  transition: color 0.2s ease;
}

.breadcrumb-link:hover {
  color: white;
}

.breadcrumb-sep {
  color: rgba(255, 255, 255, 0.4);
  font-size: 0.625rem;
}

.breadcrumb-current {
  color: white;
  font-weight: 500;
  max-width: 300px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

/* ========================================
   Media Type Badge
   ======================================== */
.media-type-badge {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem 1rem;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
  font-size: 0.875rem;
  font-weight: 500;
  border-radius: 2rem;
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.3);
}

/* ========================================
   AI Panel Transitions
   ======================================== */
.slide-fade-enter-active {
  transition: all 0.3s ease-out;
}

.slide-fade-leave-active {
  transition: all 0.2s ease-in;
}

.slide-fade-enter-from,
.slide-fade-leave-to {
  opacity: 0;
  transform: translateY(-10px);
}

/* Custom scrollbar for AI panels */
.max-h-64::-webkit-scrollbar,
.max-h-80::-webkit-scrollbar,
.max-h-48::-webkit-scrollbar {
  width: 4px;
}

.max-h-64::-webkit-scrollbar-track,
.max-h-80::-webkit-scrollbar-track,
.max-h-48::-webkit-scrollbar-track {
  background: #f1f5f9;
  border-radius: 2px;
}

.max-h-64::-webkit-scrollbar-thumb,
.max-h-80::-webkit-scrollbar-thumb,
.max-h-48::-webkit-scrollbar-thumb {
  background: #14b8a6;
  border-radius: 2px;
}

/* ========================================
   Responsive
   ======================================== */
@media (max-width: 768px) {
  .header-nav {
    flex-direction: column;
    align-items: flex-start;
    gap: 0.5rem;
  }
}
</style>
