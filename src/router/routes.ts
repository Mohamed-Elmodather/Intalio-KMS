import type { RouteRecordRaw } from 'vue-router'

// Lazy-loaded page components
const LoginPage = () => import('@/pages/LoginPage.vue')
const DashboardPage = () => import('@/pages/DashboardPage.vue')
const ArticlesPage = () => import('@/pages/articles/ArticlesPage.vue')
const ArticleViewPage = () => import('@/pages/articles/ArticleViewPage.vue')
const ArticleEditorPage = () => import('@/pages/articles/ArticleEditorPage.vue')
const DocumentsPage = () => import('@/pages/documents/DocumentsPage.vue')
const EventsPage = () => import('@/pages/events/EventsPage.vue')
const EventDetailPage = () => import('@/pages/events/EventDetailPage.vue')
const MediaCenterPage = () => import('@/pages/media/MediaCenterPage.vue')
const MediaPlayerPage = () => import('@/pages/media/MediaPlayerPage.vue')
const LearningPage = () => import('@/pages/learning/LearningPage.vue')
const CourseViewPage = () => import('@/pages/learning/CourseViewPage.vue')
const CollaborationPage = () => import('@/pages/collaboration/CollaborationPage.vue')
const PollsPage = () => import('@/pages/polls/PollsPage.vue')
const PollCreatePage = () => import('@/pages/polls/PollCreatePage.vue')
const SelfServicesPage = () => import('@/pages/services/SelfServicesPage.vue')
const SearchResultsPage = () => import('@/pages/search/SearchResultsPage.vue')
const AIAssistantPage = () => import('@/pages/ai/AIAssistantPage.vue')
const ProfilePage = () => import('@/pages/profile/ProfilePage.vue')
const SettingsPage = () => import('@/pages/settings/SettingsPage.vue')

export const routes: RouteRecordRaw[] = [
  // Public routes
  {
    path: '/login',
    name: 'Login',
    component: LoginPage,
    meta: { requiresAuth: false, layout: 'blank' },
  },
  {
    path: '/sso-callback',
    name: 'SSOCallback',
    component: LoginPage,
    meta: { requiresAuth: false, layout: 'blank' },
  },

  // Protected routes
  {
    path: '/',
    name: 'Dashboard',
    component: DashboardPage,
    meta: { requiresAuth: true, title: 'Dashboard' },
  },

  // Articles
  {
    path: '/articles',
    name: 'Articles',
    component: ArticlesPage,
    meta: { requiresAuth: true, title: 'Articles' },
  },
  {
    path: '/articles/:slug',
    name: 'ArticleView',
    component: ArticleViewPage,
    meta: { requiresAuth: true, title: 'Article' },
  },
  {
    path: '/articles/new',
    name: 'ArticleCreate',
    component: ArticleEditorPage,
    meta: { requiresAuth: true, title: 'New Article', roles: ['admin', 'editor', 'contributor'] },
  },
  {
    path: '/articles/:id/edit',
    name: 'ArticleEdit',
    component: ArticleEditorPage,
    meta: { requiresAuth: true, title: 'Edit Article', roles: ['admin', 'editor', 'contributor'] },
  },

  // Documents
  {
    path: '/documents',
    name: 'Documents',
    component: DocumentsPage,
    meta: { requiresAuth: true, title: 'Document Library' },
  },

  // Events
  {
    path: '/events',
    name: 'Events',
    component: EventsPage,
    meta: { requiresAuth: true, title: 'Events & Calendar' },
  },
  {
    path: '/events/:id',
    name: 'EventDetail',
    component: EventDetailPage,
    meta: { requiresAuth: true, title: 'Event Details' },
  },

  // Media
  {
    path: '/media',
    name: 'MediaCenter',
    component: MediaCenterPage,
    meta: { requiresAuth: true, title: 'Media Center' },
  },
  {
    path: '/media/:id',
    name: 'MediaPlayer',
    component: MediaPlayerPage,
    meta: { requiresAuth: true, title: 'Media Player' },
  },

  // Learning
  {
    path: '/learning',
    name: 'Learning',
    component: LearningPage,
    meta: { requiresAuth: true, title: 'Learning & Development' },
  },
  {
    path: '/learning/:id',
    name: 'CourseView',
    component: CourseViewPage,
    meta: { requiresAuth: true, title: 'Course' },
  },

  // Collaboration
  {
    path: '/collaboration',
    name: 'Collaboration',
    component: CollaborationPage,
    meta: { requiresAuth: true, title: 'Collaboration Hub' },
  },

  // Polls
  {
    path: '/polls',
    name: 'Polls',
    component: PollsPage,
    meta: { requiresAuth: true, title: 'Polls & Surveys' },
  },
  {
    path: '/polls/new',
    name: 'PollCreate',
    component: PollCreatePage,
    meta: { requiresAuth: true, title: 'Create Poll', roles: ['admin', 'editor'] },
  },

  // Self Services
  {
    path: '/self-services',
    name: 'SelfServices',
    component: SelfServicesPage,
    meta: { requiresAuth: true, title: 'Self Services' },
  },

  // Search
  {
    path: '/search',
    name: 'SearchResults',
    component: SearchResultsPage,
    meta: { requiresAuth: true, title: 'Search Results' },
  },

  // AI Assistant
  {
    path: '/ai-assistant',
    name: 'AIAssistant',
    component: AIAssistantPage,
    meta: { requiresAuth: true, title: 'AI Assistant' },
  },

  // Profile & Settings
  {
    path: '/profile',
    name: 'Profile',
    component: ProfilePage,
    meta: { requiresAuth: true, title: 'My Profile' },
  },
  {
    path: '/settings',
    name: 'Settings',
    component: SettingsPage,
    meta: { requiresAuth: true, title: 'Settings' },
  },

  // Catch-all redirect
  {
    path: '/:pathMatch(.*)*',
    redirect: '/',
  },
]

export default routes
