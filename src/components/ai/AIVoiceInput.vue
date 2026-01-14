<script setup lang="ts">
import { ref, watch } from 'vue'
import { useVoiceInput } from '@/composables/useVoiceInput'

interface Props {
  language?: string
  size?: 'sm' | 'md' | 'lg'
  showLanguageSelector?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  language: 'en-US',
  size: 'md',
  showLanguageSelector: false
})

const emit = defineEmits<{
  transcript: [text: string, isFinal: boolean]
  start: []
  end: []
  error: [message: string]
}>()

const selectedLanguage = ref(props.language)
const showLanguageDropdown = ref(false)

const {
  isListening,
  isSupported,
  transcript,
  interimTranscript,
  error,
  confidence,
  startListening,
  stopListening,
  toggleListening,
  setLanguage,
  supportedLanguages
} = useVoiceInput({
  language: props.language,
  interimResults: true,
  onResult: (text, isFinal) => {
    emit('transcript', text, isFinal)
  },
  onStart: () => {
    emit('start')
  },
  onEnd: () => {
    emit('end')
  },
  onError: (message) => {
    emit('error', message)
  }
})

// Watch for language changes
watch(selectedLanguage, (newLang) => {
  setLanguage(newLang)
})

function selectLanguage(code: string) {
  selectedLanguage.value = code
  showLanguageDropdown.value = false
}

function getSelectedLanguageInfo() {
  return supportedLanguages.find(l => l.code === selectedLanguage.value) || supportedLanguages[0]
}

// Size classes
const sizeClasses = {
  sm: 'w-8 h-8 text-sm',
  md: 'w-10 h-10 text-base',
  lg: 'w-12 h-12 text-lg'
}

const pulseClasses = {
  sm: 'w-10 h-10',
  md: 'w-12 h-12',
  lg: 'w-14 h-14'
}
</script>

<template>
  <div class="voice-input-container relative">
    <!-- Not Supported Warning -->
    <div
      v-if="!isSupported"
      class="voice-not-supported"
      title="Voice input is not supported in this browser"
    >
      <i class="fas fa-microphone-slash text-gray-400"></i>
    </div>

    <!-- Voice Button -->
    <div v-else class="voice-button-wrapper relative">
      <!-- Pulse Animation (when listening) -->
      <div
        v-if="isListening"
        :class="[
          'absolute inset-0 rounded-full bg-red-500/20 animate-ping',
          pulseClasses[size]
        ]"
        style="margin: -4px;"
      ></div>

      <!-- Main Button -->
      <button
        @click="toggleListening"
        :class="[
          'voice-button relative z-10 rounded-full flex items-center justify-center transition-all',
          sizeClasses[size],
          isListening
            ? 'bg-red-500 text-white shadow-lg shadow-red-200 scale-110'
            : 'bg-gray-100 text-gray-500 hover:bg-teal-100 hover:text-teal-600'
        ]"
        :title="isListening ? 'Stop listening' : 'Start voice input'"
      >
        <i :class="isListening ? 'fas fa-stop' : 'fas fa-microphone'"></i>
      </button>

      <!-- Language Selector -->
      <div v-if="showLanguageSelector" class="language-selector mt-1">
        <button
          @click="showLanguageDropdown = !showLanguageDropdown"
          class="language-button flex items-center gap-1 text-xs text-gray-500 hover:text-gray-700"
        >
          <span>{{ getSelectedLanguageInfo().flag }}</span>
          <i class="fas fa-chevron-down text-[8px]"></i>
        </button>

        <!-- Language Dropdown -->
        <Transition name="dropdown">
          <div
            v-if="showLanguageDropdown"
            class="language-dropdown absolute bottom-full left-1/2 -translate-x-1/2 mb-2 bg-white rounded-xl shadow-xl border border-gray-100 py-2 min-w-[180px] max-h-[200px] overflow-y-auto z-50"
          >
            <button
              v-for="lang in supportedLanguages"
              :key="lang.code"
              @click="selectLanguage(lang.code)"
              :class="[
                'w-full px-3 py-2 text-left text-sm flex items-center gap-2 hover:bg-gray-50 transition-colors',
                selectedLanguage === lang.code && 'bg-teal-50 text-teal-700'
              ]"
            >
              <span class="text-base">{{ lang.flag }}</span>
              <span>{{ lang.name }}</span>
              <i v-if="selectedLanguage === lang.code" class="fas fa-check text-teal-500 ml-auto text-xs"></i>
            </button>
          </div>
        </Transition>
      </div>
    </div>

    <!-- Listening Indicator -->
    <div
      v-if="isListening"
      class="listening-indicator absolute -bottom-6 left-1/2 -translate-x-1/2 whitespace-nowrap"
    >
      <span class="flex items-center gap-1.5 text-xs text-red-500 font-medium">
        <span class="listening-dot"></span>
        Listening...
      </span>
    </div>

    <!-- Error Tooltip -->
    <Transition name="fade">
      <div
        v-if="error"
        class="error-tooltip absolute -bottom-8 left-1/2 -translate-x-1/2 whitespace-nowrap
               bg-red-500 text-white text-xs px-3 py-1 rounded-lg shadow-lg"
      >
        {{ error }}
      </div>
    </Transition>

    <!-- Transcript Preview (optional) -->
    <div
      v-if="isListening && (transcript || interimTranscript)"
      class="transcript-preview absolute bottom-full left-1/2 -translate-x-1/2 mb-2
             bg-white rounded-xl shadow-xl border border-gray-100 px-4 py-2 max-w-xs"
    >
      <p class="text-sm text-gray-700">
        {{ transcript }}
        <span v-if="interimTranscript" class="text-gray-400 italic">{{ interimTranscript }}</span>
      </p>
      <div v-if="confidence > 0" class="flex items-center gap-2 mt-1">
        <span class="text-[10px] text-gray-400">Confidence:</span>
        <div class="flex-1 h-1 bg-gray-100 rounded-full overflow-hidden">
          <div
            class="h-full bg-teal-500 rounded-full transition-all"
            :style="{ width: `${confidence * 100}%` }"
          ></div>
        </div>
        <span class="text-[10px] text-gray-500">{{ Math.round(confidence * 100) }}%</span>
      </div>
    </div>
  </div>
</template>

<style scoped>
.voice-input-container {
  @apply inline-flex flex-col items-center;
}

.voice-not-supported {
  @apply w-10 h-10 rounded-full bg-gray-100 flex items-center justify-center cursor-not-allowed;
}

.voice-button-wrapper {
  @apply flex flex-col items-center;
}

.listening-dot {
  @apply w-2 h-2 bg-red-500 rounded-full animate-pulse;
}

/* Animations */
.dropdown-enter-active,
.dropdown-leave-active {
  transition: all 0.2s ease;
}

.dropdown-enter-from,
.dropdown-leave-to {
  opacity: 0;
  transform: translateX(-50%) translateY(8px);
}

.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.2s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}

/* Scrollbar for dropdown */
.language-dropdown::-webkit-scrollbar {
  width: 4px;
}

.language-dropdown::-webkit-scrollbar-track {
  background: transparent;
}

.language-dropdown::-webkit-scrollbar-thumb {
  background: #e5e7eb;
  border-radius: 2px;
}

.language-dropdown::-webkit-scrollbar-thumb:hover {
  background: #d1d5db;
}
</style>
