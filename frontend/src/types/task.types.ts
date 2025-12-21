// Task Management Types
// AFC Asian Cup 2027 - Knowledge Management System

export interface TaskUser {
  id: number
  name: string
  nameArabic?: string
  avatar?: string
  initials: string
  department?: string
}

export interface TaskRelatedItem {
  type: 'document' | 'article' | 'event' | 'community' | 'workflow'
  id: number
  title: string
  titleArabic?: string
  icon: string
  url: string
}

export interface TaskAction {
  type: 'approve' | 'reject' | 'complete' | 'defer' | 'reassign' | 'comment' | 'view'
  label: string
  labelArabic: string
  icon: string
  primary?: boolean
  severity?: 'success' | 'danger' | 'warning' | 'info'
}

export interface TaskComment {
  id: number
  user: TaskUser
  content: string
  createdAt: Date
}

export interface Task {
  id: number
  title: string
  titleArabic?: string
  description?: string
  descriptionArabic?: string

  // Workflow
  type: 'approval' | 'review' | 'assignment' | 'mention' | 'reminder'
  status: 'pending' | 'in_progress' | 'completed' | 'deferred' | 'cancelled'

  // Priority & Timing
  priority: 'urgent' | 'high' | 'medium' | 'low'
  dueDate: Date
  createdAt: Date
  completedAt?: Date

  // People
  assignee: TaskUser
  requester: TaskUser

  // Context
  relatedItem?: TaskRelatedItem

  // Actions available based on task type and status
  actions: TaskAction[]

  // Progress (for multi-step tasks)
  progress?: number

  // Comments
  comments?: TaskComment[]
  commentCount?: number
}

// Task type configurations
export const TASK_TYPE_CONFIG: Record<Task['type'], {
  icon: string
  label: string
  labelArabic: string
  color: string
  bgColor: string
}> = {
  approval: {
    icon: 'pi pi-check-circle',
    label: 'Approval',
    labelArabic: 'موافقة',
    color: '#8b5cf6',
    bgColor: 'rgba(139, 92, 246, 0.1)'
  },
  review: {
    icon: 'pi pi-eye',
    label: 'Review',
    labelArabic: 'مراجعة',
    color: '#3b82f6',
    bgColor: 'rgba(59, 130, 246, 0.1)'
  },
  assignment: {
    icon: 'pi pi-bookmark',
    label: 'Assignment',
    labelArabic: 'مهمة',
    color: '#00ae8d',
    bgColor: 'rgba(0, 174, 141, 0.1)'
  },
  mention: {
    icon: 'pi pi-at',
    label: 'Mention',
    labelArabic: 'إشارة',
    color: '#f59e0b',
    bgColor: 'rgba(245, 158, 11, 0.1)'
  },
  reminder: {
    icon: 'pi pi-bell',
    label: 'Reminder',
    labelArabic: 'تذكير',
    color: '#64748b',
    bgColor: 'rgba(100, 116, 139, 0.1)'
  }
}

export const TASK_PRIORITY_CONFIG: Record<Task['priority'], {
  label: string
  labelArabic: string
  color: string
  bgColor: string
  order: number
}> = {
  urgent: {
    label: 'Urgent',
    labelArabic: 'عاجل',
    color: '#dc2626',
    bgColor: 'rgba(220, 38, 38, 0.1)',
    order: 0
  },
  high: {
    label: 'High',
    labelArabic: 'مرتفع',
    color: '#f97316',
    bgColor: 'rgba(249, 115, 22, 0.1)',
    order: 1
  },
  medium: {
    label: 'Medium',
    labelArabic: 'متوسط',
    color: '#eab308',
    bgColor: 'rgba(234, 179, 8, 0.1)',
    order: 2
  },
  low: {
    label: 'Low',
    labelArabic: 'منخفض',
    color: '#64748b',
    bgColor: 'rgba(100, 116, 139, 0.1)',
    order: 3
  }
}

export const TASK_STATUS_CONFIG: Record<Task['status'], {
  label: string
  labelArabic: string
  color: string
  bgColor: string
  icon: string
}> = {
  pending: {
    label: 'Pending',
    labelArabic: 'قيد الانتظار',
    color: '#f59e0b',
    bgColor: 'rgba(245, 158, 11, 0.1)',
    icon: 'pi pi-clock'
  },
  in_progress: {
    label: 'In Progress',
    labelArabic: 'قيد التنفيذ',
    color: '#3b82f6',
    bgColor: 'rgba(59, 130, 246, 0.1)',
    icon: 'pi pi-spinner'
  },
  completed: {
    label: 'Completed',
    labelArabic: 'مكتمل',
    color: '#10b981',
    bgColor: 'rgba(16, 185, 129, 0.1)',
    icon: 'pi pi-check'
  },
  deferred: {
    label: 'Deferred',
    labelArabic: 'مؤجل',
    color: '#64748b',
    bgColor: 'rgba(100, 116, 139, 0.1)',
    icon: 'pi pi-pause'
  },
  cancelled: {
    label: 'Cancelled',
    labelArabic: 'ملغى',
    color: '#ef4444',
    bgColor: 'rgba(239, 68, 68, 0.1)',
    icon: 'pi pi-times'
  }
}

// Helper functions
export function isTaskOverdue(task: Task): boolean {
  if (task.status === 'completed' || task.status === 'cancelled') return false
  return new Date(task.dueDate) < new Date()
}

export function isDueToday(task: Task): boolean {
  const today = new Date()
  const dueDate = new Date(task.dueDate)
  return (
    dueDate.getDate() === today.getDate() &&
    dueDate.getMonth() === today.getMonth() &&
    dueDate.getFullYear() === today.getFullYear()
  )
}

export function isDueTomorrow(task: Task): boolean {
  const tomorrow = new Date()
  tomorrow.setDate(tomorrow.getDate() + 1)
  const dueDate = new Date(task.dueDate)
  return (
    dueDate.getDate() === tomorrow.getDate() &&
    dueDate.getMonth() === tomorrow.getMonth() &&
    dueDate.getFullYear() === tomorrow.getFullYear()
  )
}

export function getRelativeDueDate(task: Task, locale: 'en' | 'ar' = 'en'): string {
  if (isTaskOverdue(task)) {
    const days = Math.ceil((new Date().getTime() - new Date(task.dueDate).getTime()) / (1000 * 60 * 60 * 24))
    if (days === 1) return locale === 'ar' ? 'متأخر يوم واحد' : 'Overdue by 1 day'
    return locale === 'ar' ? `متأخر ${days} أيام` : `Overdue by ${days} days`
  }

  if (isDueToday(task)) {
    return locale === 'ar' ? 'مستحق اليوم' : 'Due today'
  }

  if (isDueTomorrow(task)) {
    return locale === 'ar' ? 'مستحق غداً' : 'Due tomorrow'
  }

  const days = Math.ceil((new Date(task.dueDate).getTime() - new Date().getTime()) / (1000 * 60 * 60 * 24))
  if (days <= 7) {
    return locale === 'ar' ? `مستحق خلال ${days} أيام` : `Due in ${days} days`
  }

  return new Date(task.dueDate).toLocaleDateString(locale === 'ar' ? 'ar-SA' : 'en-US', {
    month: 'short',
    day: 'numeric'
  })
}

export function sortTasksByUrgency(tasks: Task[]): Task[] {
  return [...tasks].sort((a, b) => {
    // Completed/cancelled tasks go to the end
    if (a.status === 'completed' || a.status === 'cancelled') return 1
    if (b.status === 'completed' || b.status === 'cancelled') return -1

    // Overdue tasks first
    const aOverdue = isTaskOverdue(a)
    const bOverdue = isTaskOverdue(b)
    if (aOverdue && !bOverdue) return -1
    if (!aOverdue && bOverdue) return 1

    // Due today next
    const aDueToday = isDueToday(a)
    const bDueToday = isDueToday(b)
    if (aDueToday && !bDueToday) return -1
    if (!aDueToday && bDueToday) return 1

    // Then by priority
    const priorityOrder = TASK_PRIORITY_CONFIG[a.priority].order - TASK_PRIORITY_CONFIG[b.priority].order
    if (priorityOrder !== 0) return priorityOrder

    // Finally by due date
    return new Date(a.dueDate).getTime() - new Date(b.dueDate).getTime()
  })
}

export function getDefaultActionsForTask(task: Task): TaskAction[] {
  const actions: TaskAction[] = []

  if (task.status === 'completed' || task.status === 'cancelled') {
    return [{ type: 'view', label: 'View', labelArabic: 'عرض', icon: 'pi pi-eye' }]
  }

  switch (task.type) {
    case 'approval':
      actions.push(
        { type: 'approve', label: 'Approve', labelArabic: 'موافقة', icon: 'pi pi-check', primary: true, severity: 'success' },
        { type: 'reject', label: 'Reject', labelArabic: 'رفض', icon: 'pi pi-times', severity: 'danger' }
      )
      break
    case 'review':
      actions.push(
        { type: 'complete', label: 'Mark Reviewed', labelArabic: 'تم المراجعة', icon: 'pi pi-check', primary: true, severity: 'success' },
        { type: 'comment', label: 'Add Comment', labelArabic: 'إضافة تعليق', icon: 'pi pi-comment' }
      )
      break
    case 'assignment':
      actions.push(
        { type: 'complete', label: 'Complete', labelArabic: 'إكمال', icon: 'pi pi-check', primary: true, severity: 'success' }
      )
      break
    case 'mention':
    case 'reminder':
      actions.push(
        { type: 'complete', label: 'Dismiss', labelArabic: 'تجاهل', icon: 'pi pi-check', primary: true }
      )
      break
  }

  // Common actions
  actions.push(
    { type: 'defer', label: 'Defer', labelArabic: 'تأجيل', icon: 'pi pi-clock' }
  )

  return actions
}
