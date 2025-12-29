// Unified Components - Reusable UI elements for consistent design
export { default as UnifiedSearch } from './UnifiedSearch.vue'
export { default as UnifiedHeader } from './UnifiedHeader.vue'
export { default as UnifiedSidebar } from './UnifiedSidebar.vue'
export { default as UnifiedPageLayout } from './UnifiedPageLayout.vue'

// Type exports from UnifiedHeader
export type {
  HeaderAction,
  CreateMenuItem,
  UserInfo,
  NotificationItem
} from './UnifiedHeader.vue'

// Type exports from UnifiedSidebar
export type {
  NavItem,
  WorkspaceItem,
  BottomAction
} from './UnifiedSidebar.vue'

// Type exports from UnifiedPageLayout
export type {
  Breadcrumb,
  PageStat,
  PageAction
} from './UnifiedPageLayout.vue'
