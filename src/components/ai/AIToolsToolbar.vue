<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useI18n } from 'vue-i18n'
import type { AIOperationType } from '@/types/ai'

const { t } = useI18n()

export interface ToolbarTool {
  id: AIOperationType
  name: string
  icon: string
  shortcut?: string
  color: string
  hasDropdown?: boolean
  dropdownOptions?: { value: string; label: string; icon?: string }[]
}

interface Props {
  compact?: boolean
  showLabels?: boolean
  activeTool?: AIOperationType | null
  processingTool?: AIOperationType | null
}

const props = withDefaults(defineProps<Props>(), {
  compact: false,
  showLabels: true,
  activeTool: null,
  processingTool: null
})

const emit = defineEmits<{
  'tool-click': [toolId: AIOperationType, option?: string]
  'palette-open': []
}>()

const openDropdown = ref<string | null>(null)
const dropdownPosition = ref({ top: 0, left: 0 })
const toolButtonRefs = ref<Record<string, HTMLElement | null>>({})

// Toolbar tools configuration
const toolbarTools = computed<ToolbarTool[]>(() => [
  {
    id: 'extract-entities',
    name: t('ai.toolbar.entities'),
    icon: 'fas fa-tags',
    shortcut: 'E',
    color: 'blue'
  },
  {
    id: 'analyze-sentiment',
    name: t('ai.toolbar.sentiment'),
    icon: 'fas fa-smile',
    shortcut: 'S',
    color: 'pink'
  },
  {
    id: 'summarize',
    name: t('ai.toolbar.summarize'),
    icon: 'fas fa-compress-alt',
    shortcut: 'M',
    color: 'teal',
    hasDropdown: true,
    dropdownOptions: [
      { value: 'brief', label: t('ai.toolbar.brief'), icon: 'fas fa-minus' },
      { value: 'detailed', label: t('ai.toolbar.detailed'), icon: 'fas fa-align-left' },
      { value: 'bullet', label: t('ai.toolbar.bulletPoints'), icon: 'fas fa-list-ul' }
    ]
  },
  {
    id: 'classify',
    name: t('ai.toolbar.classify'),
    icon: 'fas fa-folder-tree',
    shortcut: 'C',
    color: 'purple'
  },
  {
    id: 'ocr',
    name: 'OCR',
    icon: 'fas fa-file-image',
    shortcut: 'O',
    color: 'amber'
  },
  {
    id: 'translate',
    name: t('ai.toolbar.translate'),
    icon: 'fas fa-language',
    shortcut: 'T',
    color: 'green',
    hasDropdown: true,
    dropdownOptions: [
      { value: 'ar', label: t('languages.arabic'), icon: '🇸🇦' },
      { value: 'en', label: t('languages.english'), icon: '🇺🇸' },
      { value: 'fr', label: t('languages.french'), icon: '🇫🇷' },
      { value: 'es', label: t('languages.spanish'), icon: '🇪🇸' },
      { value: 'de', label: t('languages.german'), icon: '🇩🇪' },
      { value: 'zh', label: t('languages.chinese'), icon: '🇨🇳' }
    ]
  },
  {
    id: 'generate-title',
    name: t('ai.toolbar.titles'),
    icon: 'fas fa-heading',
    color: 'indigo',
    hasDropdown: true,
    dropdownOptions: [
      { value: 'formal', label: t('ai.toolbar.formal'), icon: 'fas fa-briefcase' },
      { value: 'casual', label: t('ai.toolbar.casual'), icon: 'fas fa-smile' },
      { value: 'seo', label: 'SEO', icon: 'fas fa-search' },
      { value: 'creative', label: t('ai.toolbar.creative'), icon: 'fas fa-lightbulb' }
    ]
  },
  {
    id: 'auto-tag',
    name: t('ai.toolbar.tags'),
    icon: 'fas fa-hashtag',
    color: 'cyan'
  },
  {
    id: 'smart-search',
    name: t('common.search'),
    icon: 'fas fa-search',
    shortcut: 'F',
    color: 'orange'
  }
])

function getToolBgColor(color: string, isActive: boolean): string {
  if (isActive) {
    const activeColors: Record<string, string> = {
      blue: 'bg-blue-500 text-white',
      pink: 'bg-pink-500 text-white',
      teal: 'bg-teal-500 text-white',
      purple: 'bg-purple-500 text-white',
      amber: 'bg-amber-500 text-white',
      green: 'bg-green-500 text-white',
      indigo: 'bg-indigo-500 text-white',
      cyan: 'bg-cyan-500 text-white',
      orange: 'bg-orange-500 text-white'
    }
    return activeColors[color] || 'bg-gray-500 text-white'
  }

  const colors: Record<string, string> = {
    blue: 'bg-blue-50 text-blue-600 hover:bg-blue-100',
    pink: 'bg-pink-50 text-pink-600 hover:bg-pink-100',
    teal: 'bg-teal-50 text-teal-600 hover:bg-teal-100',
    purple: 'bg-purple-50 text-purple-600 hover:bg-purple-100',
    amber: 'bg-amber-50 text-amber-600 hover:bg-amber-100',
    green: 'bg-green-50 text-green-600 hover:bg-green-100',
    indigo: 'bg-indigo-50 text-indigo-600 hover:bg-indigo-100',
    cyan: 'bg-cyan-50 text-cyan-600 hover:bg-cyan-100',
    orange: 'bg-orange-50 text-orange-600 hover:bg-orange-100'
  }
  return colors[color] || 'bg-gray-50 text-gray-600 hover:bg-gray-100'
}

function handleToolClick(tool: ToolbarTool, option?: string, event?: MouseEvent) {
  console.log('Tool clicked:', tool.id, 'hasDropdown:', tool.hasDropdown, 'option:', option)
  if (tool.hasDropdown && !option) {
    // Calculate position BEFORE toggling (event.currentTarget is available now)
    const button = (event?.currentTarget as HTMLElement)
    if (button) {
      const rect = button.getBoundingClientRect()
      dropdownPosition.value = {
        top: rect.bottom + 4,
        left: rect.left
      }
      console.log('Button rect:', rect, 'Position set to:', dropdownPosition.value)
    }
    // Toggle dropdown
    if (openDropdown.value === tool.id) {
      openDropdown.value = null
    } else {
      openDropdown.value = tool.id
    }
    console.log('Dropdown toggled, openDropdown:', openDropdown.value)
  } else {
    openDropdown.value = null
    emit('tool-click', tool.id, option)
  }
}

function handleDropdownOption(tool: ToolbarTool, option: string) {
  openDropdown.value = null
  emit('tool-click', tool.id, option)
}

function closeDropdowns() {
  openDropdown.value = null
}

// Close dropdown when clicking outside
function handleClickOutside(event: MouseEvent) {
  const target = event.target as HTMLElement
  // Check if click is inside a dropdown or a toolbar button with dropdown
  const isInsideDropdown = target.closest('[data-dropdown-menu]')
  const isDropdownButton = target.closest('[data-has-dropdown]')

  if (!isInsideDropdown && !isDropdownButton) {
    openDropdown.value = null
  }
}

onMounted(() => {
  document.addEventListener('click', handleClickOutside)
})

onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside)
})
</script>

<template>
  <div
    class="ai-tools-toolbar"
    :class="{ 'compact': compact }"
  >
    <!-- Tools -->
    <div class="flex items-center gap-1 flex-wrap">
      <div
        v-for="tool in toolbarTools"
        :key="tool.id"
        class="relative"
      >
        <button
          @click="handleToolClick(tool, undefined, $event)"
          :class="[
            'toolbar-tool flex items-center gap-1.5 rounded-lg transition-all',
            compact ? 'px-2 py-1.5' : 'px-3 py-2',
            getToolBgColor(tool.color, activeTool === tool.id),
            processingTool === tool.id && 'animate-pulse'
          ]"
          :title="`${tool.name}${tool.shortcut ? ` (Ctrl+${tool.shortcut})` : ''}`"
          :data-has-dropdown="tool.hasDropdown || undefined"
        >
          <!-- Processing spinner -->
          <i
            v-if="processingTool === tool.id"
            class="fas fa-spinner fa-spin text-sm"
          ></i>
          <i v-else :class="[tool.icon, compact ? 'text-sm' : 'text-base']"></i>

          <!-- Label -->
          <span
            v-if="showLabels && !compact"
            class="text-xs font-medium"
          >
            {{ tool.name }}
          </span>

          <!-- Dropdown indicator -->
          <i
            v-if="tool.hasDropdown"
            :class="[
              'fas fa-chevron-down text-[8px] transition-transform',
              openDropdown === tool.id && 'rotate-180'
            ]"
          ></i>
        </button>
      </div>

      <!-- Divider -->
      <div class="w-px h-6 bg-gray-200 mx-1"></div>

      <!-- More Tools Button (Opens Palette) -->
      <button
        @click="emit('palette-open')"
        :class="[
          'toolbar-tool flex items-center gap-1.5 rounded-lg transition-all bg-gray-100 text-gray-600 hover:bg-gray-200',
          compact ? 'px-2 py-1.5' : 'px-3 py-2'
        ]"
        :title="$t('ai.toolbar.allAITools')"
      >
        <i :class="['fas fa-th-large', compact ? 'text-sm' : 'text-base']"></i>
        <span v-if="showLabels && !compact" class="text-xs font-medium">{{ $t('common.more') }}</span>
        <kbd class="px-1 py-0.5 bg-gray-200 rounded text-[9px] font-mono ml-1">⌘K</kbd>
      </button>
    </div>

    <!-- Teleported Dropdown Menus -->
    <Teleport to="body">
      <div
        v-for="tool in toolbarTools.filter(t => t.hasDropdown)"
        :key="'dropdown-' + tool.id"
        v-show="openDropdown === tool.id"
        data-dropdown-menu
        class="fixed bg-white rounded-xl shadow-2xl border border-gray-200 py-1.5 min-w-[140px]"
        :style="{
          top: dropdownPosition.top + 'px',
          left: dropdownPosition.left + 'px',
          zIndex: 99999
        }"
      >
        <button
          v-for="option in tool.dropdownOptions"
          :key="option.value"
          @click="handleDropdownOption(tool, option.value)"
          class="w-full px-3 py-2 text-left text-sm flex items-center gap-2 hover:bg-gray-100 transition-colors"
        >
          <span v-if="option.icon?.startsWith('fas')" class="w-5 text-center">
            <i :class="option.icon" class="text-gray-400"></i>
          </span>
          <span v-else class="w-5 text-center">{{ option.icon }}</span>
          <span class="text-gray-700">{{ option.label }}</span>
        </button>
      </div>
    </Teleport>
  </div>
</template>

<style scoped>
.ai-tools-toolbar {
  @apply bg-white/80 backdrop-blur-sm rounded-xl p-2 border border-gray-100 shadow-sm overflow-visible;
}

.ai-tools-toolbar.compact {
  @apply p-1.5;
}

.toolbar-tool {
  @apply cursor-pointer select-none;
}

.toolbar-tool:disabled {
  @apply opacity-50 cursor-not-allowed;
}

/* Dropdown animation */
.dropdown-enter-active,
.dropdown-leave-active {
  transition: all 0.15s ease;
}

.dropdown-enter-from,
.dropdown-leave-to {
  opacity: 0;
  transform: translateY(-8px);
}

.dropdown-menu {
  animation: dropdown-appear 0.15s ease;
}

@keyframes dropdown-appear {
  from {
    opacity: 0;
    transform: translateY(-8px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}
</style>
