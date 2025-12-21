/**
 * Dashboard Components Barrel Export
 *
 * Import dashboard-specific reusable components from this single entry point:
 * import { PrimaryButton, SearchInput, KnowledgeCard } from '@/components/dashboard'
 */

// Button Components
export { default as PrimaryButton } from './PrimaryButton.vue'
export { default as SecondaryButton } from './SecondaryButton.vue'

// Input Components
export { default as SearchInput } from './SearchInput.vue'

// Card Components
export { default as KnowledgeCard } from './KnowledgeCard.vue'

// Navigation Components
export { default as SidebarNavigation } from './SidebarNavigation.vue'

// Re-export types
export type { PrimaryButtonProps } from './PrimaryButton.vue'
export type { SecondaryButtonProps } from './SecondaryButton.vue'
export type { SearchInputProps } from './SearchInput.vue'
export type { KnowledgeCardProps, KnowledgeCardMeta } from './KnowledgeCard.vue'
export type { SidebarNavigationProps, NavItem, NavGroup } from './SidebarNavigation.vue'
