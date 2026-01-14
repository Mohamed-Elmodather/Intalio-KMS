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

// Media interface
interface GalleryImage {
  id: number
  url: string
  thumbnail: string
  caption?: string
  width?: number
  height?: number
}

interface Media {
  id: number
  title: string
  description: string
  type: 'video' | 'audio' | 'image' | 'gallery'
  category: string
  thumbnail: string
  duration?: string
  durationSeconds?: number
  views: string
  likes: number
  date: string
  author: { name: string; avatar: string }
  tags: string[]
  hasTranscript?: boolean
  // Image-specific
  fullSizeUrl?: string
  width?: number
  height?: number
  // Gallery-specific
  images?: GalleryImage[]
  photoCount?: number
}

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
  image: 'Image',
  gallery: 'Gallery',
  views: 'views',
  like: 'Like',
  share: 'Share',
  download: 'Download',

  // Image-specific
  zoomIn: 'Zoom In',
  zoomOut: 'Zoom Out',
  resetZoom: 'Reset',
  annotate: 'Annotate',
  downloadImage: 'Download Image',
  imageDetails: 'Image Details',
  resolution: 'Resolution',
  addAnnotation: 'Click to add annotation',
  annotationPlaceholder: 'Enter annotation text...',

  // Gallery-specific
  photos: 'photos',
  viewAll: 'View All',
  slideshow: 'Slideshow',
  previousImage: 'Previous',
  nextImage: 'Next',
  imageOf: 'of',
  closeGallery: 'Close',

  // AI for Images
  ocrResults: 'Text Extraction (OCR)',
  objectDetection: 'Object Detection',
  faceDetection: 'Face Detection',
  autoTagging: 'Auto Tagging',
  extractText: 'Extract Text',
  detectObjects: 'Detect Objects',
  detectFaces: 'Detect Faces',
  generateTags: 'Generate Tags',
  detectedText: 'Detected Text',
  detectedObjects: 'Detected Objects',
  detectedFaces: 'Detected Faces',
  suggestedTags: 'Suggested Tags',
  noTextFound: 'No text detected in this image',
  noObjectsFound: 'No objects detected',
  noFacesFound: 'No faces detected',
  facesDetected: 'faces detected',

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
const media = ref<Media | null>(null)

// Add to Collection
const showAddToCollection = ref(false)
const isPlaying = ref(false)
const currentTime = ref(0)
const duration = ref(0)
const volume = ref(80)
const isMuted = ref(false)
const playbackSpeed = ref(1)
const showTranscript = ref(false)

// Media type computed properties
const isVideo = computed(() => media.value?.type === 'video')
const isAudio = computed(() => media.value?.type === 'audio')
const isImage = computed(() => media.value?.type === 'image')
const isGallery = computed(() => media.value?.type === 'gallery')
const hasPlaybackControls = computed(() => isVideo.value || isAudio.value)

// Image viewer state
const imageZoom = ref(1)
const imagePan = ref({ x: 0, y: 0 })
const isDragging = ref(false)
const dragStart = ref({ x: 0, y: 0 })
const isAnnotating = ref(false)
const annotations = ref<Array<{ id: number; x: number; y: number; text: string }>>([])
const showAnnotationInput = ref(false)
const annotationPosition = ref({ x: 0, y: 0 })
const newAnnotationText = ref('')

// Gallery state
const lightboxOpen = ref(false)
const currentImageIndex = ref(0)
const galleryImages = computed(() => media.value?.images || [])

// Image AI state
const ocrResult = ref<{ text: string; confidence: number } | null>(null)
const detectedObjects = ref<Array<{ name: string; confidence: number }>>([])
const detectedFaces = ref<Array<{ confidence: number; location: string }>>([])
const suggestedTags = ref<Array<{ tag: string; confidence: number }>>([])
const isLoadingOCR = ref(false)
const isLoadingObjects = ref(false)
const isLoadingFaces = ref(false)
const isLoadingTags = ref(false)

// Active AI tab for images
const activeImageAITab = ref<'ocr' | 'objects' | 'faces' | 'tags'>('ocr')

// Progress percentage for audio/video
const progressPercent = computed(() => {
  if (duration.value === 0) return '0%'
  return `${(currentTime.value / duration.value) * 100}%`
})

// Mock Media Data
const mockMedia: Media[] = [
  {
    id: 5,
    title: 'Saudi Arabia vs Japan: Opening Match Preview',
    description: 'In-depth preview of the highly anticipated opening match between host nation Saudi Arabia and four-time champions Japan. This comprehensive analysis covers team formations, key players to watch, tactical approaches, and predictions from expert commentators.',
    type: 'video' as const,
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
    type: 'video' as const,
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
    type: 'audio' as const,
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
  },
  {
    id: 19,
    title: 'King Fahd Stadium Aerial View',
    description: 'Stunning aerial photograph of King Fahd International Stadium in Riyadh, showcasing the magnificent architecture and scale of this world-class venue. The stadium will host the opening match and final of AFC Asian Cup 2027, with a capacity of 68,000 spectators.',
    type: 'image' as const,
    category: 'Venues',
    views: '8.2K',
    likes: 456,
    date: '1 week ago',
    thumbnail: 'https://images.unsplash.com/photo-1522778119026-d647f0596c20?w=1200&h=800&fit=crop',
    fullSizeUrl: 'https://images.unsplash.com/photo-1522778119026-d647f0596c20?w=4000&h=2667&fit=crop',
    width: 4000,
    height: 2667,
    author: { name: 'AFC Photography', avatar: 'AP' },
    tags: ['Stadium', 'Aerial', 'Riyadh', 'Venue']
  },
  {
    id: 20,
    title: 'AFC Asian Cup 2027 Official Draw Ceremony',
    description: 'Complete photo gallery from the official draw ceremony held at the King Abdullah Sports City. Featuring representatives from all 24 qualified nations, FIFA officials, and special guests as the group stage matchups were revealed.',
    type: 'gallery' as const,
    category: 'Events',
    views: '15.7K',
    likes: 892,
    date: '3 days ago',
    thumbnail: 'https://images.unsplash.com/photo-1540747913346-19e32dc3e97e?w=1200&h=800&fit=crop',
    photoCount: 24,
    images: [
      { id: 1, url: 'https://images.unsplash.com/photo-1540747913346-19e32dc3e97e?w=1920&h=1280&fit=crop', thumbnail: 'https://images.unsplash.com/photo-1540747913346-19e32dc3e97e?w=400&h=300&fit=crop', caption: 'Opening ceremony overview' },
      { id: 2, url: 'https://images.unsplash.com/photo-1459865264687-595d652de67e?w=1920&h=1280&fit=crop', thumbnail: 'https://images.unsplash.com/photo-1459865264687-595d652de67e?w=400&h=300&fit=crop', caption: 'Group A draw announcement' },
      { id: 3, url: 'https://images.unsplash.com/photo-1574629810360-7efbbe195018?w=1920&h=1280&fit=crop', thumbnail: 'https://images.unsplash.com/photo-1574629810360-7efbbe195018?w=400&h=300&fit=crop', caption: 'Team representatives on stage' },
      { id: 4, url: 'https://images.unsplash.com/photo-1431324155629-1a6deb1dec8d?w=1920&h=1280&fit=crop', thumbnail: 'https://images.unsplash.com/photo-1431324155629-1a6deb1dec8d?w=400&h=300&fit=crop', caption: 'Audience at the ceremony' },
      { id: 5, url: 'https://images.unsplash.com/photo-1606925797300-0b35e9d1794e?w=1920&h=1280&fit=crop', thumbnail: 'https://images.unsplash.com/photo-1606925797300-0b35e9d1794e?w=400&h=300&fit=crop', caption: 'Group B teams revealed' },
      { id: 6, url: 'https://images.unsplash.com/photo-1579952363873-27f3bade9f55?w=1920&h=1280&fit=crop', thumbnail: 'https://images.unsplash.com/photo-1579952363873-27f3bade9f55?w=400&h=300&fit=crop', caption: 'Saudi Arabia delegation' },
      { id: 7, url: 'https://images.unsplash.com/photo-1551958219-acbc608c6377?w=1920&h=1280&fit=crop', thumbnail: 'https://images.unsplash.com/photo-1551958219-acbc608c6377?w=400&h=300&fit=crop', caption: 'Japan team representatives' },
      { id: 8, url: 'https://images.unsplash.com/photo-1560272564-c83b66b1ad12?w=1920&h=1280&fit=crop', thumbnail: 'https://images.unsplash.com/photo-1560272564-c83b66b1ad12?w=400&h=300&fit=crop', caption: 'Final group standings display' }
    ],
    author: { name: 'AFC Events', avatar: 'AE' },
    tags: ['Draw', 'Ceremony', 'Official', 'Groups']
  },
  // Additional galleries matching MediaCenterPage
  {
    id: 13,
    title: 'AFC Asian Cup 2027 Official Draw Ceremony',
    description: 'Complete photo gallery from the official draw ceremony featuring representatives from all 24 qualified nations.',
    type: 'gallery' as const,
    category: 'Highlights',
    views: '28.5K',
    likes: 1240,
    date: '1 day ago',
    thumbnail: 'https://images.unsplash.com/photo-1459865264687-595d652de67e?w=1200&h=800&fit=crop',
    photoCount: 48,
    images: [
      { id: 1, url: 'https://images.unsplash.com/photo-1459865264687-595d652de67e?w=1920&h=1280&fit=crop', thumbnail: 'https://images.unsplash.com/photo-1459865264687-595d652de67e?w=400&h=300&fit=crop', caption: 'Draw ceremony opening' },
      { id: 2, url: 'https://images.unsplash.com/photo-1540747913346-19e32dc3e97e?w=1920&h=1280&fit=crop', thumbnail: 'https://images.unsplash.com/photo-1540747913346-19e32dc3e97e?w=400&h=300&fit=crop', caption: 'Officials on stage' },
      { id: 3, url: 'https://images.unsplash.com/photo-1574629810360-7efbbe195018?w=1920&h=1280&fit=crop', thumbnail: 'https://images.unsplash.com/photo-1574629810360-7efbbe195018?w=400&h=300&fit=crop', caption: 'Group announcements' },
      { id: 4, url: 'https://images.unsplash.com/photo-1431324155629-1a6deb1dec8d?w=1920&h=1280&fit=crop', thumbnail: 'https://images.unsplash.com/photo-1431324155629-1a6deb1dec8d?w=400&h=300&fit=crop', caption: 'Audience reaction' }
    ],
    author: { name: 'AFC Media', avatar: 'AM' },
    tags: ['Draw', 'Official', 'Ceremony']
  },
  {
    id: 14,
    title: 'Fan Zone Setup: Riyadh Boulevard',
    description: 'Behind the scenes look at the spectacular fan zone being prepared at Riyadh Boulevard for AFC Asian Cup 2027.',
    type: 'gallery' as const,
    category: 'Fans',
    views: '15.3K',
    likes: 678,
    date: '3 days ago',
    thumbnail: 'https://images.unsplash.com/photo-1560272564-c83b66b1ad12?w=1200&h=800&fit=crop',
    photoCount: 32,
    images: [
      { id: 1, url: 'https://images.unsplash.com/photo-1560272564-c83b66b1ad12?w=1920&h=1280&fit=crop', thumbnail: 'https://images.unsplash.com/photo-1560272564-c83b66b1ad12?w=400&h=300&fit=crop', caption: 'Fan zone entrance' },
      { id: 2, url: 'https://images.unsplash.com/photo-1517466787929-bc90951d0974?w=1920&h=1280&fit=crop', thumbnail: 'https://images.unsplash.com/photo-1517466787929-bc90951d0974?w=400&h=300&fit=crop', caption: 'Stage setup' },
      { id: 3, url: 'https://images.unsplash.com/photo-1606925797300-0b35e9d1794e?w=1920&h=1280&fit=crop', thumbnail: 'https://images.unsplash.com/photo-1606925797300-0b35e9d1794e?w=400&h=300&fit=crop', caption: 'Seating area' }
    ],
    author: { name: 'Events Team', avatar: 'ET' },
    tags: ['Fans', 'Riyadh', 'Fan Zone']
  },
  {
    id: 15,
    title: 'Volunteer Training Program Launch',
    description: 'Photos from the launch of the volunteer training program for AFC Asian Cup 2027.',
    type: 'gallery' as const,
    category: 'Behind the Scenes',
    views: '12.8K',
    likes: 456,
    date: '1 week ago',
    thumbnail: 'https://images.unsplash.com/photo-1517466787929-bc90951d0974?w=1200&h=800&fit=crop',
    photoCount: 65,
    images: [
      { id: 1, url: 'https://images.unsplash.com/photo-1517466787929-bc90951d0974?w=1920&h=1280&fit=crop', thumbnail: 'https://images.unsplash.com/photo-1517466787929-bc90951d0974?w=400&h=300&fit=crop', caption: 'Training session' },
      { id: 2, url: 'https://images.unsplash.com/photo-1504450758481-7338eba7524a?w=1920&h=1280&fit=crop', thumbnail: 'https://images.unsplash.com/photo-1504450758481-7338eba7524a?w=400&h=300&fit=crop', caption: 'Group activities' }
    ],
    author: { name: 'LOC Team', avatar: 'LT' },
    tags: ['Volunteers', 'Training', 'Behind the Scenes']
  },
  {
    id: 16,
    title: 'Stadium Infrastructure Progress Photos',
    description: 'Latest progress photos showing the stadium infrastructure preparations for AFC Asian Cup 2027.',
    type: 'gallery' as const,
    category: 'Venues',
    views: '9.5K',
    likes: 345,
    date: '2 weeks ago',
    thumbnail: 'https://images.unsplash.com/photo-1489944440615-453fc2b6a9a9?w=1200&h=800&fit=crop',
    photoCount: 28,
    images: [
      { id: 1, url: 'https://images.unsplash.com/photo-1489944440615-453fc2b6a9a9?w=1920&h=1280&fit=crop', thumbnail: 'https://images.unsplash.com/photo-1489944440615-453fc2b6a9a9?w=400&h=300&fit=crop', caption: 'Stadium overview' },
      { id: 2, url: 'https://images.unsplash.com/photo-1522778119026-d647f0596c20?w=1920&h=1280&fit=crop', thumbnail: 'https://images.unsplash.com/photo-1522778119026-d647f0596c20?w=400&h=300&fit=crop', caption: 'Construction progress' }
    ],
    author: { name: 'Venue Operations', avatar: 'VO' },
    tags: ['Stadium', 'Infrastructure', 'Progress']
  },
  {
    id: 17,
    title: 'Media Accreditation Workshop',
    description: 'Photos from the media accreditation workshop for journalists covering AFC Asian Cup 2027.',
    type: 'gallery' as const,
    category: 'Behind the Scenes',
    views: '6.2K',
    likes: 234,
    date: '3 weeks ago',
    thumbnail: 'https://images.unsplash.com/photo-1504450758481-7338eba7524a?w=1200&h=800&fit=crop',
    photoCount: 42,
    images: [
      { id: 1, url: 'https://images.unsplash.com/photo-1504450758481-7338eba7524a?w=1920&h=1280&fit=crop', thumbnail: 'https://images.unsplash.com/photo-1504450758481-7338eba7524a?w=400&h=300&fit=crop', caption: 'Workshop opening' },
      { id: 2, url: 'https://images.unsplash.com/photo-1511671782779-c97d3d27a1d4?w=1920&h=1280&fit=crop', thumbnail: 'https://images.unsplash.com/photo-1511671782779-c97d3d27a1d4?w=400&h=300&fit=crop', caption: 'Credential collection' }
    ],
    author: { name: 'Media Team', avatar: 'MT' },
    tags: ['Media', 'Workshop', 'Behind the Scenes']
  },
  // Additional images matching MediaCenterPage
  {
    id: 21,
    title: 'Saudi Arabia Team Photo 2027',
    description: 'Official team photo of the Saudi Arabia national football team for AFC Asian Cup 2027.',
    type: 'image' as const,
    category: 'Teams',
    views: '35.2K',
    likes: 1890,
    date: '3 days ago',
    thumbnail: 'https://images.unsplash.com/photo-1517466787929-bc90951d0974?w=1200&h=800&fit=crop',
    fullSizeUrl: 'https://images.unsplash.com/photo-1517466787929-bc90951d0974?w=4000&h=2667&fit=crop',
    width: 4000,
    height: 2667,
    author: { name: 'AFC Media', avatar: 'AM' },
    tags: ['Saudi Arabia', 'Team', 'Official']
  },
  {
    id: 22,
    title: 'Trophy Unveiling Ceremony',
    description: 'The official AFC Asian Cup trophy unveiled at a special ceremony in Riyadh.',
    type: 'image' as const,
    category: 'Highlights',
    views: '42.1K',
    likes: 2340,
    date: '1 week ago',
    thumbnail: 'https://images.unsplash.com/photo-1579952363873-27f3bade9f55?w=1200&h=800&fit=crop',
    fullSizeUrl: 'https://images.unsplash.com/photo-1579952363873-27f3bade9f55?w=4000&h=2667&fit=crop',
    width: 4000,
    height: 2667,
    author: { name: 'AFC Media', avatar: 'AM' },
    tags: ['Trophy', 'Ceremony', 'Official']
  },
  {
    id: 24,
    title: 'Fans Celebrating at Boulevard',
    description: 'Football fans celebrating at Riyadh Boulevard during AFC Asian Cup festivities.',
    type: 'image' as const,
    category: 'Fans',
    views: '28.9K',
    likes: 1567,
    date: '2 weeks ago',
    thumbnail: 'https://images.unsplash.com/photo-1459865264687-595d652de67e?w=1200&h=800&fit=crop',
    fullSizeUrl: 'https://images.unsplash.com/photo-1459865264687-595d652de67e?w=4000&h=2667&fit=crop',
    width: 4000,
    height: 2667,
    author: { name: 'Events Team', avatar: 'ET' },
    tags: ['Fans', 'Celebration', 'Riyadh']
  },
  // Additional audio matching MediaCenterPage
  {
    id: 12,
    title: 'AFC Podcast: Tournament Predictions',
    description: 'Expert predictions and analysis for AFC Asian Cup 2027 from our panel of football analysts.',
    type: 'audio' as const,
    category: 'Interviews',
    duration: '48:20',
    durationSeconds: 2900,
    views: '8.9K',
    likes: 456,
    date: '3 weeks ago',
    thumbnail: 'https://images.unsplash.com/photo-1508098682722-e99c43a406b2?w=1200&h=675&fit=crop',
    author: { name: 'AFC Media', avatar: 'AM' },
    tags: ['Podcast', 'Predictions', 'Analysis'],
    hasTranscript: false
  },
  {
    id: 23,
    title: 'Pre-Match Analysis Podcast: Group A',
    description: 'In-depth analysis of Group A teams and their chances in AFC Asian Cup 2027.',
    type: 'audio' as const,
    category: 'Matches',
    duration: '32:15',
    durationSeconds: 1935,
    views: '6.4K',
    likes: 345,
    date: '1 week ago',
    thumbnail: 'https://images.unsplash.com/photo-1508098682722-e99c43a406b2?w=1200&h=675&fit=crop',
    author: { name: 'AFC Analysts', avatar: 'AA' },
    tags: ['Podcast', 'Group A', 'Analysis'],
    hasTranscript: false
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

function downloadMedia() {
  if (!media.value) return
  // Simulate download - in production would trigger actual file download
  const link = document.createElement('a')
  link.href = media.value.thumbnail // In production, this would be the actual media URL
  link.download = `${media.value.title}.${media.value.type === 'video' ? 'mp4' : 'mp3'}`
  link.click()
}

function shareMedia() {
  if (!media.value) return
  if (navigator.share) {
    navigator.share({
      title: media.value.title,
      text: media.value.description,
      url: window.location.href
    })
  } else {
    navigator.clipboard.writeText(window.location.href)
    alert('Link copied to clipboard!')
  }
}

// ============================================================================
// Image Viewer Functions
// ============================================================================

function zoomIn() {
  imageZoom.value = Math.min(imageZoom.value + 0.25, 4)
}

function zoomOut() {
  imageZoom.value = Math.max(imageZoom.value - 0.25, 0.5)
}

function resetZoom() {
  imageZoom.value = 1
  imagePan.value = { x: 0, y: 0 }
}

function handleImageWheel(e: WheelEvent) {
  e.preventDefault()
  if (e.deltaY < 0) {
    zoomIn()
  } else {
    zoomOut()
  }
}

function startImageDrag(e: MouseEvent) {
  if (isAnnotating.value) return
  isDragging.value = true
  dragStart.value = { x: e.clientX - imagePan.value.x, y: e.clientY - imagePan.value.y }
}

function doImageDrag(e: MouseEvent) {
  if (!isDragging.value) return
  imagePan.value = {
    x: e.clientX - dragStart.value.x,
    y: e.clientY - dragStart.value.y
  }
}

function endImageDrag() {
  isDragging.value = false
}

function handleImageClick(e: MouseEvent) {
  if (!isAnnotating.value) return
  const rect = (e.currentTarget as HTMLElement).getBoundingClientRect()
  const x = ((e.clientX - rect.left) / rect.width) * 100
  const y = ((e.clientY - rect.top) / rect.height) * 100
  annotationPosition.value = { x, y }
  showAnnotationInput.value = true
  newAnnotationText.value = ''
}

function saveAnnotation() {
  if (!newAnnotationText.value.trim()) {
    showAnnotationInput.value = false
    return
  }
  annotations.value.push({
    id: Date.now(),
    x: annotationPosition.value.x,
    y: annotationPosition.value.y,
    text: newAnnotationText.value.trim()
  })
  showAnnotationInput.value = false
  newAnnotationText.value = ''
}

function cancelAnnotation() {
  showAnnotationInput.value = false
  newAnnotationText.value = ''
}

function removeAnnotation(id: number) {
  annotations.value = annotations.value.filter(a => a.id !== id)
}

function downloadImage() {
  if (!media.value) return
  const link = document.createElement('a')
  link.href = media.value.fullSizeUrl || media.value.thumbnail
  link.download = `${media.value.title}.jpg`
  link.click()
}

// ============================================================================
// Gallery Functions
// ============================================================================

function openLightbox(index: number) {
  currentImageIndex.value = index
  lightboxOpen.value = true
  document.addEventListener('keydown', handleLightboxKeydown)
}

function closeLightbox() {
  lightboxOpen.value = false
  document.removeEventListener('keydown', handleLightboxKeydown)
}

function prevImage() {
  if (currentImageIndex.value > 0) {
    currentImageIndex.value--
  }
}

function nextImage() {
  if (currentImageIndex.value < galleryImages.value.length - 1) {
    currentImageIndex.value++
  }
}

function handleLightboxKeydown(e: KeyboardEvent) {
  if (e.key === 'ArrowLeft') {
    prevImage()
  } else if (e.key === 'ArrowRight') {
    nextImage()
  } else if (e.key === 'Escape') {
    closeLightbox()
  }
}

// ============================================================================
// Image AI Functions
// ============================================================================

async function extractOCR() {
  if (isLoadingOCR.value) return
  isLoadingOCR.value = true
  ocrResult.value = null

  try {
    await new Promise(resolve => setTimeout(resolve, 1500))
    ocrResult.value = {
      text: 'AFC Asian Cup 2027\nKing Fahd International Stadium\nRiyadh, Saudi Arabia\n\nCapacity: 68,000\nOpening Match: January 10, 2027',
      confidence: 0.94
    }
  } finally {
    isLoadingOCR.value = false
  }
}

async function detectObjectsInImage() {
  if (isLoadingObjects.value) return
  isLoadingObjects.value = true
  detectedObjects.value = []

  try {
    await new Promise(resolve => setTimeout(resolve, 1200))
    detectedObjects.value = [
      { name: 'Stadium', confidence: 0.96 },
      { name: 'Soccer Field', confidence: 0.94 },
      { name: 'Crowd/Audience', confidence: 0.89 },
      { name: 'Scoreboard', confidence: 0.85 },
      { name: 'Floodlights', confidence: 0.82 },
      { name: 'Goal Posts', confidence: 0.78 }
    ]
  } finally {
    isLoadingObjects.value = false
  }
}

async function detectFacesInImage() {
  if (isLoadingFaces.value) return
  isLoadingFaces.value = true
  detectedFaces.value = []

  try {
    await new Promise(resolve => setTimeout(resolve, 1000))
    detectedFaces.value = [
      { confidence: 0.97, location: 'Center-left' },
      { confidence: 0.92, location: 'Center-right' },
      { confidence: 0.88, location: 'Background' }
    ]
  } finally {
    isLoadingFaces.value = false
  }
}

async function generateImageTags() {
  if (isLoadingTags.value) return
  isLoadingTags.value = true
  suggestedTags.value = []

  try {
    await new Promise(resolve => setTimeout(resolve, 1100))
    suggestedTags.value = [
      { tag: 'Stadium', confidence: 0.98 },
      { tag: 'Sports Venue', confidence: 0.95 },
      { tag: 'Soccer', confidence: 0.93 },
      { tag: 'AFC Asian Cup', confidence: 0.91 },
      { tag: 'Riyadh', confidence: 0.88 },
      { tag: 'Aerial View', confidence: 0.85 },
      { tag: 'Architecture', confidence: 0.82 }
    ]
  } finally {
    isLoadingTags.value = false
  }
}

function copyOCRText() {
  if (ocrResult.value) {
    navigator.clipboard.writeText(ocrResult.value.text)
    alert('Text copied to clipboard!')
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
              <button @click="downloadMedia" class="px-4 py-2 bg-transparent text-white border border-white/30 rounded-xl font-medium hover:bg-white/10 transition-all flex items-center gap-2">
                <i class="fas fa-download"></i>
                <span class="hidden sm:inline">{{ textConstants.download }}</span>
              </button>
              <button @click="showAddToCollection = true" class="px-4 py-2 bg-transparent text-white border border-white/30 rounded-xl font-medium hover:bg-white/10 transition-all flex items-center gap-2" title="Add to Collection">
                <i class="fas fa-folder-plus"></i>
                <span class="hidden sm:inline">{{ textConstants.collection }}</span>
              </button>
              <button @click="shareMedia" class="px-4 py-2 bg-transparent text-white border border-white/30 rounded-xl font-medium hover:bg-white/10 transition-all flex items-center gap-2">
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

          <!-- ============================================ -->
          <!-- VIDEO PLAYER -->
          <!-- ============================================ -->
          <div v-if="isVideo" class="bg-black rounded-2xl overflow-hidden shadow-2xl">
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
                  <i class="fas fa-video"></i>
                  {{ textConstants.video }}
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
                  <div class="h-full bg-teal-500 rounded-full relative" :style="{ width: progressPercent }">
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

          <!-- ============================================ -->
          <!-- AUDIO PLAYER -->
          <!-- ============================================ -->
          <div v-else-if="isAudio" class="bg-white rounded-2xl overflow-hidden shadow-lg">
            <!-- Album Art / Thumbnail -->
            <div class="relative">
              <div class="aspect-square max-h-96 mx-auto overflow-hidden">
                <img :src="media.thumbnail" :alt="media.title" class="w-full h-full object-cover" />
              </div>
              <div class="absolute inset-0 bg-gradient-to-t from-black/60 via-transparent to-transparent"></div>

              <!-- Centered Play Button -->
              <div class="absolute inset-0 flex items-center justify-center">
                <button @click="togglePlay" class="w-24 h-24 bg-white/20 backdrop-blur-md rounded-full flex items-center justify-center hover:bg-white/30 transition-all hover:scale-105 shadow-xl">
                  <i :class="['text-white text-4xl', isPlaying ? 'fas fa-pause' : 'fas fa-play ml-2']"></i>
                </button>
              </div>

              <!-- Audio Type Badge -->
              <div class="absolute top-4 left-4">
                <span class="media-type-badge">
                  <i class="fas fa-headphones"></i>
                  {{ textConstants.audio }}
                </span>
              </div>

              <!-- Duration Badge -->
              <div class="absolute bottom-4 right-4">
                <span class="px-3 py-1.5 bg-black/70 backdrop-blur-sm text-white text-sm rounded-full">
                  {{ media.duration }}
                </span>
              </div>
            </div>

            <!-- Compact Audio Controls -->
            <div class="p-5 bg-gray-900">
              <!-- Progress Bar -->
              <div class="flex items-center gap-3 mb-4">
                <span class="text-white text-xs w-12 font-mono">{{ formatTime(currentTime) }}</span>
                <div class="flex-1 h-1.5 bg-gray-700 rounded-full cursor-pointer group" @click="seekTo(duration * ($event.offsetX / ($event.target as HTMLElement).clientWidth))">
                  <div class="h-full bg-teal-500 rounded-full relative transition-all" :style="{ width: progressPercent }">
                    <div class="absolute right-0 top-1/2 -translate-y-1/2 w-4 h-4 bg-white rounded-full shadow-lg opacity-0 group-hover:opacity-100 transition-opacity"></div>
                  </div>
                </div>
                <span class="text-white text-xs w-12 text-right font-mono">{{ formatTime(duration) }}</span>
              </div>

              <!-- Control Buttons Row -->
              <div class="flex items-center justify-center gap-6">
                <button @click="seekTo(currentTime - 15)" class="text-gray-400 hover:text-white transition-colors">
                  <i class="fas fa-backward-step text-xl"></i>
                </button>
                <button @click="togglePlay" class="w-14 h-14 bg-teal-500 hover:bg-teal-600 rounded-full flex items-center justify-center text-white transition-all hover:scale-105">
                  <i :class="['text-xl', isPlaying ? 'fas fa-pause' : 'fas fa-play ml-1']"></i>
                </button>
                <button @click="seekTo(currentTime + 15)" class="text-gray-400 hover:text-white transition-colors">
                  <i class="fas fa-forward-step text-xl"></i>
                </button>
                <button @click="toggleMute" class="text-gray-400 hover:text-white transition-colors">
                  <i :class="['text-lg', isMuted ? 'fas fa-volume-mute' : 'fas fa-volume-up']"></i>
                </button>
                <select v-model="playbackSpeed" class="bg-gray-800 text-white text-sm rounded-lg px-3 py-1.5 border-none">
                  <option :value="0.5">0.5x</option>
                  <option :value="1">1x</option>
                  <option :value="1.25">1.25x</option>
                  <option :value="1.5">1.5x</option>
                  <option :value="2">2x</option>
                </select>
              </div>
            </div>
          </div>

          <!-- ============================================ -->
          <!-- IMAGE VIEWER -->
          <!-- ============================================ -->
          <div v-else-if="isImage" class="bg-white rounded-2xl overflow-hidden shadow-lg">
            <!-- Image Container with Zoom/Pan -->
            <div
              class="image-viewer-container relative bg-gray-900 overflow-hidden"
              style="min-height: 500px;"
              @wheel.prevent="handleImageWheel"
            >
              <div
                class="image-wrapper flex items-center justify-center h-full"
                :class="{ 'cursor-grab': !isAnnotating, 'cursor-crosshair': isAnnotating }"
                :style="{
                  transform: `scale(${imageZoom}) translate(${imagePan.x}px, ${imagePan.y}px)`,
                  transition: isDragging ? 'none' : 'transform 0.1s ease-out'
                }"
                @mousedown="startImageDrag"
                @mousemove="doImageDrag"
                @mouseup="endImageDrag"
                @mouseleave="endImageDrag"
                @click="handleImageClick"
              >
                <img
                  :src="media.fullSizeUrl || media.thumbnail"
                  :alt="media.title"
                  class="max-w-full max-h-[70vh] object-contain select-none"
                  draggable="false"
                />

                <!-- Annotations Overlay -->
                <div
                  v-for="ann in annotations"
                  :key="ann.id"
                  class="annotation-marker"
                  :style="{ left: `${ann.x}%`, top: `${ann.y}%` }"
                >
                  <div class="annotation-dot">
                    <i class="fas fa-map-marker-alt text-white text-xs"></i>
                  </div>
                  <div class="annotation-tooltip">
                    <span>{{ ann.text }}</span>
                    <button @click.stop="removeAnnotation(ann.id)" class="ml-2 text-red-400 hover:text-red-300">
                      <i class="fas fa-times"></i>
                    </button>
                  </div>
                </div>
              </div>

              <!-- Image Type Badge -->
              <div class="absolute top-4 left-4">
                <span class="media-type-badge">
                  <i class="fas fa-image"></i>
                  {{ textConstants.image }}
                </span>
              </div>

              <!-- Annotation Toggle -->
              <button
                @click="isAnnotating = !isAnnotating"
                :class="[
                  'absolute top-4 right-4 px-4 py-2 rounded-xl font-medium flex items-center gap-2 transition-all',
                  isAnnotating
                    ? 'bg-teal-500 text-white'
                    : 'bg-black/50 text-white hover:bg-black/70'
                ]"
              >
                <i class="fas fa-pen"></i>
                {{ textConstants.annotate }}
              </button>

              <!-- Zoom Controls -->
              <div class="absolute bottom-4 left-1/2 -translate-x-1/2 flex items-center gap-2 bg-black/70 backdrop-blur-sm rounded-full px-4 py-2">
                <button @click="zoomOut" class="w-8 h-8 flex items-center justify-center text-white hover:text-teal-400 transition-colors">
                  <i class="fas fa-minus"></i>
                </button>
                <span class="text-white text-sm w-16 text-center font-mono">{{ Math.round(imageZoom * 100) }}%</span>
                <button @click="zoomIn" class="w-8 h-8 flex items-center justify-center text-white hover:text-teal-400 transition-colors">
                  <i class="fas fa-plus"></i>
                </button>
                <div class="w-px h-6 bg-white/30 mx-1"></div>
                <button @click="resetZoom" class="w-8 h-8 flex items-center justify-center text-white hover:text-teal-400 transition-colors" :title="textConstants.resetZoom">
                  <i class="fas fa-expand"></i>
                </button>
              </div>

              <!-- Annotation Input Modal -->
              <div
                v-if="showAnnotationInput"
                class="absolute inset-0 bg-black/50 flex items-center justify-center z-20"
                @click.self="cancelAnnotation"
              >
                <div class="bg-white rounded-xl p-4 w-80 shadow-xl">
                  <h4 class="font-semibold text-gray-900 mb-3">{{ textConstants.addAnnotation }}</h4>
                  <input
                    v-model="newAnnotationText"
                    type="text"
                    :placeholder="textConstants.annotationPlaceholder"
                    class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-teal-500 focus:border-transparent"
                    @keyup.enter="saveAnnotation"
                    autofocus
                  />
                  <div class="flex justify-end gap-2 mt-3">
                    <button @click="cancelAnnotation" class="px-4 py-2 text-gray-600 hover:text-gray-800">
                      Cancel
                    </button>
                    <button @click="saveAnnotation" class="px-4 py-2 bg-teal-500 text-white rounded-lg hover:bg-teal-600">
                      Save
                    </button>
                  </div>
                </div>
              </div>
            </div>

            <!-- Image Info Bar -->
            <div class="p-4 border-t border-gray-200 flex items-center justify-between bg-gray-50">
              <div class="flex items-center gap-4 text-sm text-gray-500">
                <span class="flex items-center gap-1.5">
                  <i class="fas fa-expand-arrows-alt text-teal-500"></i>
                  {{ media.width }} x {{ media.height }}
                </span>
                <span v-if="annotations.length > 0" class="flex items-center gap-1.5">
                  <i class="fas fa-map-marker-alt text-teal-500"></i>
                  {{ annotations.length }} annotations
                </span>
              </div>
              <button @click="downloadImage" class="flex items-center gap-2 text-teal-600 hover:text-teal-700 font-medium">
                <i class="fas fa-download"></i>
                {{ textConstants.downloadImage }}
              </button>
            </div>
          </div>

          <!-- ============================================ -->
          <!-- GALLERY VIEWER -->
          <!-- ============================================ -->
          <div v-else-if="isGallery" class="space-y-4">
            <!-- Gallery Header -->
            <div class="bg-white rounded-2xl p-5 shadow-sm border border-gray-100">
              <div class="flex items-center justify-between">
                <div class="flex items-center gap-3">
                  <div class="w-12 h-12 rounded-xl bg-gradient-to-br from-teal-500 to-teal-600 flex items-center justify-center">
                    <i class="fas fa-images text-white text-xl"></i>
                  </div>
                  <div>
                    <h3 class="font-semibold text-gray-900">{{ media.photoCount }} {{ textConstants.photos }}</h3>
                    <p class="text-sm text-gray-500">{{ media.category }}</p>
                  </div>
                </div>
              </div>
            </div>

            <!-- Thumbnail Grid -->
            <div class="bg-white rounded-2xl p-4 shadow-sm border border-gray-100">
              <div class="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-3">
                <div
                  v-for="(img, idx) in galleryImages"
                  :key="img.id"
                  @click="openLightbox(idx)"
                  class="relative aspect-square rounded-xl overflow-hidden cursor-pointer group"
                >
                  <img :src="img.thumbnail" :alt="img.caption || `Image ${idx + 1}`" class="w-full h-full object-cover transition-transform duration-300 group-hover:scale-110" />
                  <div class="absolute inset-0 bg-black/0 group-hover:bg-black/30 transition-colors flex items-center justify-center">
                    <div class="w-12 h-12 rounded-full bg-white/20 backdrop-blur-sm flex items-center justify-center opacity-0 group-hover:opacity-100 transition-opacity">
                      <i class="fas fa-expand text-white text-lg"></i>
                    </div>
                  </div>
                  <span class="absolute bottom-2 left-2 px-2 py-1 bg-black/60 text-white text-xs rounded-lg opacity-0 group-hover:opacity-100 transition-opacity">
                    {{ idx + 1 }}/{{ galleryImages.length }}
                  </span>
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
              <!-- AI Tab Navigation for Video/Audio -->
              <div v-if="hasPlaybackControls" class="grid grid-cols-4 gap-2">
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

              <!-- AI Tab Navigation for Images/Gallery -->
              <div v-if="isImage || isGallery" class="grid grid-cols-4 gap-2">
                <button
                  v-for="tab in [
                    { id: 'ocr', icon: 'fas fa-font', label: 'OCR' },
                    { id: 'objects', icon: 'fas fa-cube', label: 'Objects' },
                    { id: 'faces', icon: 'fas fa-smile', label: 'Faces' },
                    { id: 'tags', icon: 'fas fa-tags', label: 'Tags' }
                  ]"
                  :key="tab.id"
                  @click="activeImageAITab = tab.id as any"
                  :class="[
                    'px-3 py-2.5 rounded-xl text-xs font-medium transition-all flex flex-col items-center gap-1.5',
                    activeImageAITab === tab.id
                      ? 'bg-teal-50 text-teal-700 border-2 border-teal-200'
                      : 'bg-gray-50 text-gray-600 border-2 border-transparent hover:bg-gray-100'
                  ]"
                >
                  <i :class="[tab.icon, 'text-base']"></i>
                  <span>{{ tab.label }}</span>
                </button>
              </div>

              <!-- ============================================ -->
              <!-- IMAGE AI TABS CONTENT -->
              <!-- ============================================ -->

              <!-- OCR Tab -->
              <div v-if="(isImage || isGallery) && activeImageAITab === 'ocr'" class="space-y-4">
                <button
                  @click="extractOCR"
                  :disabled="isLoadingOCR"
                  class="w-full px-4 py-2.5 bg-teal-500 hover:bg-teal-600 disabled:bg-teal-300 text-white rounded-lg font-medium flex items-center justify-center gap-2 transition-colors"
                >
                  <i :class="['fas', isLoadingOCR ? 'fa-spinner fa-spin' : 'fa-font']"></i>
                  {{ isLoadingOCR ? 'Extracting...' : textConstants.extractText }}
                </button>

                <div v-if="ocrResult" class="space-y-3">
                  <div class="p-4 bg-gray-50 rounded-xl">
                    <div class="flex items-center justify-between mb-2">
                      <span class="text-sm font-medium text-gray-700">{{ textConstants.detectedText }}</span>
                      <button @click="copyOCRText" class="text-teal-600 hover:text-teal-700 text-sm">
                        <i class="fas fa-copy mr-1"></i>
                        Copy
                      </button>
                    </div>
                    <pre class="text-sm text-gray-800 whitespace-pre-wrap font-sans bg-white p-3 rounded-lg border border-gray-200">{{ ocrResult.text }}</pre>
                  </div>
                  <div class="flex items-center text-xs text-gray-400">
                    <i class="fas fa-check-circle text-teal-500 mr-1"></i>
                    {{ Math.round(ocrResult.confidence * 100) }}% confidence
                  </div>
                </div>

                <div v-else-if="!isLoadingOCR" class="text-center py-6 text-gray-500 text-sm">
                  <i class="fas fa-font text-3xl text-gray-300 mb-2"></i>
                  <p>Extract text from the image using AI</p>
                </div>
              </div>

              <!-- Object Detection Tab -->
              <div v-if="(isImage || isGallery) && activeImageAITab === 'objects'" class="space-y-4">
                <button
                  @click="detectObjectsInImage"
                  :disabled="isLoadingObjects"
                  class="w-full px-4 py-2.5 bg-teal-500 hover:bg-teal-600 disabled:bg-teal-300 text-white rounded-lg font-medium flex items-center justify-center gap-2 transition-colors"
                >
                  <i :class="['fas', isLoadingObjects ? 'fa-spinner fa-spin' : 'fa-cube']"></i>
                  {{ isLoadingObjects ? 'Detecting...' : textConstants.detectObjects }}
                </button>

                <div v-if="detectedObjects.length > 0" class="space-y-2">
                  <div
                    v-for="(obj, idx) in detectedObjects"
                    :key="idx"
                    class="flex items-center justify-between p-3 bg-gray-50 rounded-lg"
                  >
                    <div class="flex items-center gap-2">
                      <div class="w-8 h-8 rounded-lg bg-teal-100 flex items-center justify-center">
                        <i class="fas fa-cube text-teal-600 text-sm"></i>
                      </div>
                      <span class="font-medium text-gray-800">{{ obj.name }}</span>
                    </div>
                    <span class="text-sm text-gray-500">{{ Math.round(obj.confidence * 100) }}%</span>
                  </div>
                </div>

                <div v-else-if="!isLoadingObjects" class="text-center py-6 text-gray-500 text-sm">
                  <i class="fas fa-cube text-3xl text-gray-300 mb-2"></i>
                  <p>Detect objects in the image using AI</p>
                </div>
              </div>

              <!-- Face Detection Tab -->
              <div v-if="(isImage || isGallery) && activeImageAITab === 'faces'" class="space-y-4">
                <button
                  @click="detectFacesInImage"
                  :disabled="isLoadingFaces"
                  class="w-full px-4 py-2.5 bg-teal-500 hover:bg-teal-600 disabled:bg-teal-300 text-white rounded-lg font-medium flex items-center justify-center gap-2 transition-colors"
                >
                  <i :class="['fas', isLoadingFaces ? 'fa-spinner fa-spin' : 'fa-smile']"></i>
                  {{ isLoadingFaces ? 'Detecting...' : textConstants.detectFaces }}
                </button>

                <div v-if="detectedFaces.length > 0" class="space-y-3">
                  <div class="p-4 bg-teal-50 rounded-xl text-center">
                    <div class="text-3xl font-bold text-teal-600 mb-1">{{ detectedFaces.length }}</div>
                    <div class="text-sm text-teal-700">{{ textConstants.facesDetected }}</div>
                  </div>
                  <div
                    v-for="(face, idx) in detectedFaces"
                    :key="idx"
                    class="flex items-center justify-between p-3 bg-gray-50 rounded-lg"
                  >
                    <div class="flex items-center gap-2">
                      <div class="w-8 h-8 rounded-full bg-blue-100 flex items-center justify-center">
                        <i class="fas fa-user text-blue-600 text-sm"></i>
                      </div>
                      <span class="text-gray-700">Face {{ idx + 1 }} ({{ face.location }})</span>
                    </div>
                    <span class="text-sm text-gray-500">{{ Math.round(face.confidence * 100) }}%</span>
                  </div>
                </div>

                <div v-else-if="!isLoadingFaces" class="text-center py-6 text-gray-500 text-sm">
                  <i class="fas fa-smile text-3xl text-gray-300 mb-2"></i>
                  <p>Detect faces in the image using AI</p>
                </div>
              </div>

              <!-- Auto-Tag Tab -->
              <div v-if="(isImage || isGallery) && activeImageAITab === 'tags'" class="space-y-4">
                <button
                  @click="generateImageTags"
                  :disabled="isLoadingTags"
                  class="w-full px-4 py-2.5 bg-teal-500 hover:bg-teal-600 disabled:bg-teal-300 text-white rounded-lg font-medium flex items-center justify-center gap-2 transition-colors"
                >
                  <i :class="['fas', isLoadingTags ? 'fa-spinner fa-spin' : 'fa-tags']"></i>
                  {{ isLoadingTags ? 'Generating...' : textConstants.generateTags }}
                </button>

                <div v-if="suggestedTags.length > 0" class="space-y-3">
                  <p class="text-sm text-gray-600 font-medium">{{ textConstants.suggestedTags }}</p>
                  <div class="flex flex-wrap gap-2">
                    <span
                      v-for="(item, idx) in suggestedTags"
                      :key="idx"
                      class="px-3 py-1.5 bg-teal-50 text-teal-700 rounded-full text-sm font-medium hover:bg-teal-100 cursor-pointer transition-colors"
                    >
                      #{{ item.tag }}
                      <span class="text-xs text-teal-500 ml-1">{{ Math.round(item.confidence * 100) }}%</span>
                    </span>
                  </div>
                </div>

                <div v-else-if="!isLoadingTags" class="text-center py-6 text-gray-500 text-sm">
                  <i class="fas fa-tags text-3xl text-gray-300 mb-2"></i>
                  <p>Generate relevant tags for this image</p>
                </div>
              </div>

              <!-- ============================================ -->
              <!-- VIDEO/AUDIO AI TABS CONTENT -->
              <!-- ============================================ -->

              <!-- Transcript Tab -->
              <div v-if="hasPlaybackControls && activeAITab === 'transcript'" class="space-y-4">
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
              <div v-if="hasPlaybackControls && activeAITab === 'summary'" class="space-y-4">
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
              <div v-if="hasPlaybackControls && activeAITab === 'moments'" class="space-y-4">
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
              <div v-if="hasPlaybackControls && activeAITab === 'translate'" class="space-y-4">
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

      <!-- About this Media - Full Width -->
      <div class="bg-white rounded-2xl shadow-sm border border-gray-100 p-6 mt-6">
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

      <!-- Related Media - Full Width -->
      <div class="bg-white rounded-2xl shadow-sm border border-gray-100 p-6 mt-6">
        <h2 class="text-lg font-semibold text-gray-900 mb-4 flex items-center gap-2">
          <i class="fas fa-play-circle text-teal-500"></i>
          {{ textConstants.relatedMedia }}
        </h2>
        <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
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

    <!-- Gallery Lightbox Modal -->
    <Teleport to="body">
      <Transition name="lightbox">
        <div
          v-if="lightboxOpen && isGallery"
          class="fixed inset-0 bg-black/95 z-50 flex items-center justify-center"
        >
          <!-- Close Button -->
          <button
            @click="closeLightbox"
            class="absolute top-4 right-4 w-12 h-12 rounded-full bg-white/10 hover:bg-white/20 text-white flex items-center justify-center transition-colors z-10"
          >
            <i class="fas fa-times text-2xl"></i>
          </button>

          <!-- Previous Button -->
          <button
            v-if="currentImageIndex > 0"
            @click="prevImage"
            class="absolute left-4 top-1/2 -translate-y-1/2 w-14 h-14 rounded-full bg-white/10 hover:bg-white/20 text-white flex items-center justify-center transition-colors z-10"
          >
            <i class="fas fa-chevron-left text-2xl"></i>
          </button>

          <!-- Next Button -->
          <button
            v-if="currentImageIndex < galleryImages.length - 1"
            @click="nextImage"
            class="absolute right-4 top-1/2 -translate-y-1/2 w-14 h-14 rounded-full bg-white/10 hover:bg-white/20 text-white flex items-center justify-center transition-colors z-10"
          >
            <i class="fas fa-chevron-right text-2xl"></i>
          </button>

          <!-- Current Image -->
          <div class="max-w-full max-h-full p-16">
            <img
              :src="galleryImages[currentImageIndex]?.url"
              :alt="galleryImages[currentImageIndex]?.caption || `Image ${currentImageIndex + 1}`"
              class="max-w-full max-h-[85vh] object-contain mx-auto rounded-lg shadow-2xl"
            />
          </div>

          <!-- Image Counter -->
          <div class="absolute bottom-6 left-1/2 -translate-x-1/2 px-4 py-2 bg-white/10 backdrop-blur-sm rounded-full text-white font-medium">
            {{ currentImageIndex + 1 }} {{ textConstants.imageOf }} {{ galleryImages.length }}
          </div>

          <!-- Caption -->
          <div
            v-if="galleryImages[currentImageIndex]?.caption"
            class="absolute bottom-16 left-1/2 -translate-x-1/2 px-6 py-3 bg-black/60 backdrop-blur-sm rounded-xl text-white text-center max-w-lg"
          >
            {{ galleryImages[currentImageIndex].caption }}
          </div>

          <!-- Thumbnail Strip -->
          <div class="absolute bottom-24 left-1/2 -translate-x-1/2 flex gap-2 max-w-[80vw] overflow-x-auto py-2 px-4">
            <button
              v-for="(img, idx) in galleryImages"
              :key="img.id"
              @click="currentImageIndex = idx"
              :class="[
                'w-16 h-16 rounded-lg overflow-hidden flex-shrink-0 transition-all border-2',
                idx === currentImageIndex ? 'border-teal-400 scale-110' : 'border-transparent opacity-60 hover:opacity-100'
              ]"
            >
              <img :src="img.thumbnail" :alt="`Thumbnail ${idx + 1}`" class="w-full h-full object-cover" />
            </button>
          </div>
        </div>
      </Transition>
    </Teleport>
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
   Image Viewer Styles
   ======================================== */
.image-viewer-container {
  position: relative;
  display: flex;
  align-items: center;
  justify-content: center;
}

.image-wrapper {
  transform-origin: center center;
  min-height: 500px;
  width: 100%;
}

.image-wrapper.cursor-grab {
  cursor: grab;
}

.image-wrapper.cursor-grab:active {
  cursor: grabbing;
}

.image-wrapper.cursor-crosshair {
  cursor: crosshair;
}

/* ========================================
   Annotation Styles
   ======================================== */
.annotation-marker {
  position: absolute;
  z-index: 10;
  transform: translate(-50%, -50%);
}

.annotation-dot {
  width: 28px;
  height: 28px;
  border-radius: 50%;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  box-shadow: 0 2px 8px rgba(20, 184, 166, 0.4);
  transition: transform 0.2s ease;
}

.annotation-dot:hover {
  transform: scale(1.1);
}

.annotation-tooltip {
  position: absolute;
  bottom: 100%;
  left: 50%;
  transform: translateX(-50%) translateY(-8px);
  background: white;
  color: #374151;
  padding: 0.5rem 0.75rem;
  border-radius: 0.5rem;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.15);
  white-space: nowrap;
  font-size: 0.875rem;
  opacity: 0;
  visibility: hidden;
  transition: all 0.2s ease;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.annotation-marker:hover .annotation-tooltip {
  opacity: 1;
  visibility: visible;
}

/* ========================================
   Lightbox Transitions
   ======================================== */
.lightbox-enter-active,
.lightbox-leave-active {
  transition: opacity 0.3s ease;
}

.lightbox-enter-from,
.lightbox-leave-to {
  opacity: 0;
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

  .image-wrapper {
    min-height: 300px;
  }
}
</style>
