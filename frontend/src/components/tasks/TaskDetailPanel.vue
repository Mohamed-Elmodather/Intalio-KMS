<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useUIStore } from '@/stores/ui.store'
import type { Task, TaskAction } from '@/types/task.types'
import {
  TASK_TYPE_CONFIG,
  TASK_PRIORITY_CONFIG,
  TASK_STATUS_CONFIG,
  isTaskOverdue,
  getRelativeDueDate
} from '@/types/task.types'
import { Avatar } from '@/components/base'
import Button from 'primevue/button'
import Textarea from 'primevue/textarea'

const props = defineProps<{
  task: Task | null
  visible: boolean
}>()

const emit = defineEmits<{
  (e: 'close'): void
  (e: 'action', action: TaskAction, task: Task): void
}>()

const router = useRouter()
const uiStore = useUIStore()

const isRTL = computed(() => uiStore.locale === 'ar')
const newComment = ref('')
const isSubmittingComment = ref(false)

const taskTypeConfig = computed(() =>
  props.task ? TASK_TYPE_CONFIG[props.task.type] : null
)

const priorityConfig = computed(() =>
  props.task ? TASK_PRIORITY_CONFIG[props.task.priority] : null
)

const statusConfig = computed(() =>
  props.task ? TASK_STATUS_CONFIG[props.task.status] : null
)

const isOverdue = computed(() =>
  props.task ? isTaskOverdue(props.task) : false
)

const relativeDueDate = computed(() =>
  props.task ? getRelativeDueDate(props.task, isRTL.value ? 'ar' : 'en') : ''
)

const formattedDueDate = computed(() => {
  if (!props.task) return ''
  return new Date(props.task.dueDate).toLocaleDateString(
    isRTL.value ? 'ar-SA' : 'en-US',
    { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' }
  )
})

const formattedCreatedAt = computed(() => {
  if (!props.task) return ''
  return new Date(props.task.createdAt).toLocaleDateString(
    isRTL.value ? 'ar-SA' : 'en-US',
    { month: 'short', day: 'numeric', hour: '2-digit', minute: '2-digit' }
  )
})

function handleAction(action: TaskAction) {
  if (props.task) {
    emit('action', action, props.task)
  }
}

function closePanel() {
  emit('close')
}

function navigateToRelatedItem() {
  if (props.task?.relatedItem) {
    router.push(props.task.relatedItem.url)
    closePanel()
  }
}

async function submitComment() {
  if (!newComment.value.trim() || !props.task) return

  isSubmittingComment.value = true
  // Simulate API call
  await new Promise(resolve => setTimeout(resolve, 500))

  // In real implementation, this would call an API
  console.log('Submitting comment:', newComment.value, 'for task:', props.task.id)

  newComment.value = ''
  isSubmittingComment.value = false
}

function handleKeydown(event: KeyboardEvent) {
  if (event.key === 'Escape') {
    closePanel()
  }
}
</script>

<template>
  <Teleport to="body">
    <Transition name="panel">
      <div
        v-if="visible && task"
        class="task-panel-overlay"
        @click.self="closePanel"
        @keydown="handleKeydown"
      >
        <div class="task-panel" :class="{ rtl: isRTL }">
          <!-- Header -->
          <header class="panel-header">
            <div class="header-left">
              <div
                class="task-type-badge"
                :style="{
                  color: taskTypeConfig?.color,
                  background: taskTypeConfig?.bgColor
                }"
              >
                <i :class="taskTypeConfig?.icon"></i>
                <span>{{ isRTL ? taskTypeConfig?.labelArabic : taskTypeConfig?.label }}</span>
              </div>
              <div
                v-if="isOverdue"
                class="overdue-badge"
              >
                <i class="pi pi-exclamation-triangle"></i>
                <span>{{ isRTL ? 'متأخر' : 'Overdue' }}</span>
              </div>
            </div>
            <button class="close-btn" @click="closePanel" :aria-label="isRTL ? 'إغلاق' : 'Close'">
              <i class="pi pi-times"></i>
            </button>
          </header>

          <!-- Content -->
          <div class="panel-content">
            <!-- Title -->
            <h2 class="task-title">{{ isRTL ? task.titleArabic || task.title : task.title }}</h2>

            <!-- Due Date -->
            <div class="due-date-section" :class="{ overdue: isOverdue }">
              <i class="pi pi-calendar"></i>
              <div class="due-date-info">
                <span class="relative-date">{{ relativeDueDate }}</span>
                <span class="full-date">{{ formattedDueDate }}</span>
              </div>
            </div>

            <!-- Priority & Status -->
            <div class="meta-row">
              <div class="meta-item">
                <span class="meta-label">{{ isRTL ? 'الأولوية' : 'Priority' }}</span>
                <span
                  class="priority-badge"
                  :style="{
                    color: priorityConfig?.color,
                    background: priorityConfig?.bgColor
                  }"
                >
                  {{ isRTL ? priorityConfig?.labelArabic : priorityConfig?.label }}
                </span>
              </div>
              <div class="meta-item">
                <span class="meta-label">{{ isRTL ? 'الحالة' : 'Status' }}</span>
                <span
                  class="status-badge"
                  :style="{
                    color: statusConfig?.color,
                    background: statusConfig?.bgColor
                  }"
                >
                  <i :class="statusConfig?.icon"></i>
                  {{ isRTL ? statusConfig?.labelArabic : statusConfig?.label }}
                </span>
              </div>
            </div>

            <!-- Requester -->
            <div class="person-section">
              <span class="section-label">{{ isRTL ? 'مطلوب من' : 'Requested by' }}</span>
              <div class="person-card">
                <Avatar
                  :name="task.requester.name"
                  :image="task.requester.avatar"
                  shape="circle"
                  class="person-avatar"
                />
                <div class="person-info">
                  <span class="person-name">{{ isRTL ? task.requester.nameArabic || task.requester.name : task.requester.name }}</span>
                  <span class="person-department" v-if="task.requester.department">{{ task.requester.department }}</span>
                </div>
                <span class="request-time">{{ formattedCreatedAt }}</span>
              </div>
            </div>

            <!-- Description -->
            <div v-if="task.description" class="description-section">
              <span class="section-label">{{ isRTL ? 'الوصف' : 'Description' }}</span>
              <p class="description-text">{{ isRTL ? task.descriptionArabic || task.description : task.description }}</p>
            </div>

            <!-- Related Item -->
            <div v-if="task.relatedItem" class="related-section">
              <span class="section-label">{{ isRTL ? 'العنصر المرتبط' : 'Related Item' }}</span>
              <button class="related-card" @click="navigateToRelatedItem">
                <div class="related-icon">
                  <i :class="task.relatedItem.icon"></i>
                </div>
                <div class="related-info">
                  <span class="related-type">{{ task.relatedItem.type }}</span>
                  <span class="related-title">{{ isRTL ? task.relatedItem.titleArabic || task.relatedItem.title : task.relatedItem.title }}</span>
                </div>
                <i class="pi pi-arrow-right related-arrow"></i>
              </button>
            </div>

            <!-- Progress (if applicable) -->
            <div v-if="task.progress !== undefined && task.type === 'assignment'" class="progress-section">
              <div class="progress-header">
                <span class="section-label">{{ isRTL ? 'التقدم' : 'Progress' }}</span>
                <span class="progress-value">{{ task.progress }}%</span>
              </div>
              <div class="progress-bar">
                <div class="progress-fill" :style="{ width: `${task.progress}%` }"></div>
              </div>
            </div>

            <!-- Comments -->
            <div class="comments-section">
              <span class="section-label">
                {{ isRTL ? 'التعليقات' : 'Comments' }}
                <span v-if="task.commentCount" class="comment-count">({{ task.commentCount }})</span>
              </span>

              <!-- Comment Input -->
              <div class="comment-input-wrapper">
                <Textarea
                  v-model="newComment"
                  :placeholder="isRTL ? 'أضف تعليقاً...' : 'Add a comment...'"
                  rows="2"
                  autoResize
                  class="comment-input"
                />
                <Button
                  icon="pi pi-send"
                  :loading="isSubmittingComment"
                  :disabled="!newComment.trim()"
                  class="send-btn"
                  @click="submitComment"
                />
              </div>

              <!-- Existing Comments -->
              <div v-if="task.comments && task.comments.length > 0" class="comments-list">
                <div
                  v-for="comment in task.comments"
                  :key="comment.id"
                  class="comment-item"
                >
                  <Avatar
                    :name="comment.user.name"
                    :image="comment.user.avatar"
                    shape="circle"
                    size="sm"
                    class="comment-avatar"
                  />
                  <div class="comment-content">
                    <div class="comment-header">
                      <span class="comment-author">{{ comment.user.name }}</span>
                      <span class="comment-time">
                        {{ new Date(comment.createdAt).toLocaleDateString(isRTL ? 'ar-SA' : 'en-US', { month: 'short', day: 'numeric' }) }}
                      </span>
                    </div>
                    <p class="comment-text">{{ comment.content }}</p>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Actions Footer -->
          <footer class="panel-footer">
            <div class="action-buttons">
              <Button
                v-for="action in task.actions"
                :key="action.type"
                :label="isRTL ? action.labelArabic : action.label"
                :icon="action.icon"
                :severity="action.severity || 'secondary'"
                :outlined="!action.primary"
                :class="{ 'primary-action': action.primary }"
                @click="handleAction(action)"
              />
            </div>
          </footer>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<style lang="scss" scoped>
@use '@/assets/styles/_variables.scss' as *;

// Overlay
.task-panel-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.4);
  backdrop-filter: blur(4px);
  z-index: 1000;
  display: flex;
  justify-content: flex-end;
}

// Panel
.task-panel {
  width: 100%;
  max-width: 480px;
  height: 100%;
  background: #fff;
  display: flex;
  flex-direction: column;
  box-shadow: -8px 0 32px rgba(0, 0, 0, 0.15);

  &.rtl {
    direction: rtl;

    .related-arrow {
      transform: rotate(180deg);
    }
  }

  @media (max-width: $breakpoint-sm) {
    max-width: 100%;
  }
}

// Header
.panel-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: $spacing-4 $spacing-5;
  border-bottom: 1px solid $slate-100;
  background: $slate-50;
}

.header-left {
  display: flex;
  align-items: center;
  gap: $spacing-2;
}

.task-type-badge {
  display: inline-flex;
  align-items: center;
  gap: $spacing-2;
  padding: $spacing-1 $spacing-3;
  border-radius: $radius-full;
  font-size: $text-xs;
  font-weight: $font-weight-semibold;

  i {
    font-size: 0.75rem;
  }
}

.overdue-badge {
  display: inline-flex;
  align-items: center;
  gap: $spacing-1;
  padding: $spacing-1 $spacing-2;
  background: $error-50;
  color: $error-600;
  border-radius: $radius-full;
  font-size: $text-xs;
  font-weight: $font-weight-semibold;

  i {
    font-size: 0.625rem;
  }
}

.close-btn {
  width: 36px;
  height: 36px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: transparent;
  border: none;
  border-radius: $radius-lg;
  color: $slate-500;
  cursor: pointer;
  transition: all $transition-fast;

  &:hover {
    background: $slate-200;
    color: $slate-700;
  }
}

// Content
.panel-content {
  flex: 1;
  overflow-y: auto;
  padding: $spacing-5;
  display: flex;
  flex-direction: column;
  gap: $spacing-5;
}

.task-title {
  font-size: $text-xl;
  font-weight: $font-weight-bold;
  color: $slate-900;
  margin: 0;
  line-height: 1.4;
}

// Due Date
.due-date-section {
  display: flex;
  align-items: flex-start;
  gap: $spacing-3;
  padding: $spacing-4;
  background: $slate-50;
  border-radius: $radius-xl;

  > i {
    font-size: 1.25rem;
    color: $slate-400;
    margin-top: 2px;
  }

  &.overdue {
    background: $error-50;

    > i {
      color: $error-500;
    }

    .relative-date {
      color: $error-600;
    }
  }
}

.due-date-info {
  display: flex;
  flex-direction: column;
  gap: $spacing-1;
}

.relative-date {
  font-size: $text-base;
  font-weight: $font-weight-semibold;
  color: $slate-900;
}

.full-date {
  font-size: $text-sm;
  color: $slate-500;
}

// Meta Row
.meta-row {
  display: flex;
  gap: $spacing-6;
}

.meta-item {
  display: flex;
  flex-direction: column;
  gap: $spacing-2;
}

.meta-label {
  font-size: $text-xs;
  font-weight: $font-weight-medium;
  color: $slate-500;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.priority-badge,
.status-badge {
  display: inline-flex;
  align-items: center;
  gap: $spacing-1;
  padding: $spacing-1 $spacing-3;
  border-radius: $radius-full;
  font-size: $text-sm;
  font-weight: $font-weight-medium;

  i {
    font-size: 0.75rem;
  }
}

// Person Section
.section-label {
  display: block;
  font-size: $text-xs;
  font-weight: $font-weight-semibold;
  color: $slate-500;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  margin-bottom: $spacing-3;
}

.person-section {
  .section-label {
    margin-bottom: $spacing-2;
  }
}

.person-card {
  display: flex;
  align-items: center;
  gap: $spacing-3;
  padding: $spacing-3;
  background: $slate-50;
  border-radius: $radius-xl;
}

.person-avatar {
  flex-shrink: 0;
  background: $gradient-primary !important;
}

.person-info {
  flex: 1;
  min-width: 0;
}

.person-name {
  display: block;
  font-size: $text-sm;
  font-weight: $font-weight-semibold;
  color: $slate-900;
}

.person-department {
  display: block;
  font-size: $text-xs;
  color: $slate-500;
}

.request-time {
  font-size: $text-xs;
  color: $slate-400;
  white-space: nowrap;
}

// Description
.description-text {
  font-size: $text-sm;
  color: $slate-700;
  line-height: 1.6;
  margin: 0;
}

// Related Item
.related-card {
  display: flex;
  align-items: center;
  gap: $spacing-3;
  width: 100%;
  padding: $spacing-3;
  background: $slate-50;
  border: 1px solid $slate-200;
  border-radius: $radius-xl;
  cursor: pointer;
  transition: all $transition-fast;
  text-align: start;

  &:hover {
    background: $slate-100;
    border-color: $slate-300;
  }
}

.related-icon {
  width: 40px;
  height: 40px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: $intalio-teal-50;
  color: $intalio-teal-600;
  border-radius: $radius-lg;
  flex-shrink: 0;

  i {
    font-size: 1.125rem;
  }
}

.related-info {
  flex: 1;
  min-width: 0;
}

.related-type {
  display: block;
  font-size: $text-xs;
  color: $slate-500;
  text-transform: capitalize;
}

.related-title {
  display: block;
  font-size: $text-sm;
  font-weight: $font-weight-medium;
  color: $slate-900;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.related-arrow {
  color: $slate-400;
  font-size: 0.875rem;
}

// Progress
.progress-section {
  .progress-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: $spacing-2;

    .section-label {
      margin-bottom: 0;
    }
  }

  .progress-value {
    font-size: $text-sm;
    font-weight: $font-weight-semibold;
    color: $intalio-teal-600;
  }
}

.progress-bar {
  height: 8px;
  background: $slate-200;
  border-radius: $radius-full;
  overflow: hidden;
}

.progress-fill {
  height: 100%;
  background: $gradient-primary;
  border-radius: $radius-full;
  transition: width 0.3s ease;
}

// Comments
.comments-section {
  .comment-count {
    font-weight: $font-weight-normal;
    color: $slate-400;
  }
}

.comment-input-wrapper {
  display: flex;
  gap: $spacing-2;
  align-items: flex-end;
}

.comment-input {
  flex: 1;
  font-size: $text-sm;

  :deep(textarea) {
    border-radius: $radius-lg;
  }
}

.send-btn {
  flex-shrink: 0;

  :deep(.p-button) {
    width: 40px;
    height: 40px;
  }
}

.comments-list {
  display: flex;
  flex-direction: column;
  gap: $spacing-3;
  margin-top: $spacing-4;
}

.comment-item {
  display: flex;
  gap: $spacing-3;
}

.comment-avatar {
  flex-shrink: 0;
  background: $slate-200 !important;
}

.comment-content {
  flex: 1;
  min-width: 0;
}

.comment-header {
  display: flex;
  align-items: center;
  gap: $spacing-2;
  margin-bottom: $spacing-1;
}

.comment-author {
  font-size: $text-sm;
  font-weight: $font-weight-semibold;
  color: $slate-900;
}

.comment-time {
  font-size: $text-xs;
  color: $slate-400;
}

.comment-text {
  font-size: $text-sm;
  color: $slate-700;
  margin: 0;
  line-height: 1.5;
}

// Footer
.panel-footer {
  padding: $spacing-4 $spacing-5;
  border-top: 1px solid $slate-100;
  background: #fff;
}

.action-buttons {
  display: flex;
  gap: $spacing-2;
  flex-wrap: wrap;

  .primary-action {
    flex: 1;
    min-width: 120px;
  }
}

// Transitions
.panel-enter-active,
.panel-leave-active {
  transition: opacity 0.2s ease;

  .task-panel {
    transition: transform 0.3s cubic-bezier(0.16, 1, 0.3, 1);
  }
}

.panel-enter-from,
.panel-leave-to {
  opacity: 0;

  .task-panel {
    transform: translateX(100%);
  }

  .rtl .task-panel {
    transform: translateX(-100%);
  }
}
</style>
