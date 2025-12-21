/**
 * Base Components Barrel Export
 *
 * Import all base components from this single entry point:
 * import { BaseCard, BaseButton, Widget } from '@/components/base'
 */

// Core Components
export { default as BaseCard } from './BaseCard.vue'
export { default as BaseButton } from './BaseButton.vue'
export { default as BaseInput } from './BaseInput.vue'

// Layout Components
export { default as Widget } from './Widget.vue'
export { default as PageHeader } from './PageHeader.vue'
export { default as StatsBar } from './StatsBar.vue'
export { default as EmptyState } from './EmptyState.vue'
export { default as LoadingSpinner } from './LoadingSpinner.vue'
export { default as Badge } from './Badge.vue'
export { default as Avatar } from './Avatar.vue'

// Re-export types
export type { BaseCardProps } from './BaseCard.vue'
export type { BaseButtonProps } from './BaseButton.vue'
export type { BaseInputProps } from './BaseInput.vue'
export type { WidgetProps, WidgetTab } from './Widget.vue'
export type { PageHeaderProps, BreadcrumbItem } from './PageHeader.vue'
export type { StatsBarProps, StatItem } from './StatsBar.vue'
export type { EmptyStateProps } from './EmptyState.vue'
export type { LoadingSpinnerProps } from './LoadingSpinner.vue'
export type { BadgeProps } from './Badge.vue'
export type { AvatarProps } from './Avatar.vue'
