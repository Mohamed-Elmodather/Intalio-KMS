// Unified Components - Reusable UI elements for consistent design
export { default as UnifiedSearch } from './UnifiedSearch.vue'
export { default as UnifiedHeader } from './UnifiedHeader.vue'
export { default as UnifiedSidebar } from './UnifiedSidebar.vue'

// Type exports
export type {
  HeaderAction,
  CreateMenuItem,
  UserInfo,
  NotificationItem
} from './UnifiedHeader.vue'

export type {
  NavItem,
  WorkspaceItem,
  BottomAction
} from './UnifiedSidebar.vue'
