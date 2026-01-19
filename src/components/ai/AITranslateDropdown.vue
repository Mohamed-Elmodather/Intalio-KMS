<script setup lang="ts">
import { ref, computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { SUPPORTED_LANGUAGES, type SupportedLanguage, type LanguageOption } from '@/types/ai'

const { t } = useI18n()

interface Props {
  modelValue?: SupportedLanguage
  placeholder?: string
  disabled?: boolean
  showFlags?: boolean
  excludeLanguages?: SupportedLanguage[]
}

const props = withDefaults(defineProps<Props>(), {
  disabled: false,
  showFlags: true,
  excludeLanguages: () => [],
})

const emit = defineEmits<{
  'update:modelValue': [value: SupportedLanguage]
  change: [value: SupportedLanguage]
}>()

const isOpen = ref(false)
const searchQuery = ref('')

const filteredLanguages = computed(() => {
  return SUPPORTED_LANGUAGES
    .filter(lang => !props.excludeLanguages.includes(lang.code))
    .filter(lang =>
      lang.name.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      lang.nativeName.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      lang.code.toLowerCase().includes(searchQuery.value.toLowerCase())
    )
})

const selectedLanguage = computed(() =>
  SUPPORTED_LANGUAGES.find(lang => lang.code === props.modelValue)
)

function selectLanguage(lang: LanguageOption) {
  emit('update:modelValue', lang.code)
  emit('change', lang.code)
  isOpen.value = false
  searchQuery.value = ''
}

function toggleDropdown() {
  if (!props.disabled) {
    isOpen.value = !isOpen.value
    if (!isOpen.value) {
      searchQuery.value = ''
    }
  }
}

function closeDropdown() {
  isOpen.value = false
  searchQuery.value = ''
}
</script>

<template>
  <div class="ai-translate-dropdown relative" v-click-outside="closeDropdown">
    <!-- Trigger button -->
    <button
      type="button"
      class="w-full flex items-center justify-between gap-2 px-3 py-2 bg-white border border-gray-200 rounded-lg text-sm transition-colors"
      :class="[
        disabled ? 'opacity-50 cursor-not-allowed' : 'hover:border-teal-300 focus:border-teal-500 focus:ring-2 focus:ring-teal-200',
        isOpen ? 'border-teal-500 ring-2 ring-teal-200' : '',
      ]"
      :disabled="disabled"
      @click="toggleDropdown"
    >
      <div class="flex items-center gap-2">
        <i class="fas fa-language text-teal-500" />
        <span v-if="selectedLanguage" class="flex items-center gap-2">
          <span v-if="showFlags" class="text-base">{{ selectedLanguage.flag }}</span>
          <span class="text-gray-700">{{ selectedLanguage.name }}</span>
          <span class="text-gray-400 text-xs">({{ selectedLanguage.nativeName }})</span>
        </span>
        <span v-else class="text-gray-400">{{ placeholder || $t('ai.selectLanguage') }}</span>
      </div>
      <i
        class="fas fa-chevron-down text-gray-400 transition-transform"
        :class="{ 'rotate-180': isOpen }"
      />
    </button>

    <!-- Dropdown panel -->
    <Transition
      enter-active-class="transition ease-out duration-200"
      enter-from-class="opacity-0 translate-y-1"
      enter-to-class="opacity-100 translate-y-0"
      leave-active-class="transition ease-in duration-150"
      leave-from-class="opacity-100 translate-y-0"
      leave-to-class="opacity-0 translate-y-1"
    >
      <div
        v-if="isOpen"
        class="absolute z-50 mt-1 w-full bg-white border border-gray-200 rounded-lg shadow-lg overflow-hidden"
      >
        <!-- Search input -->
        <div class="p-2 border-b border-gray-100">
          <div class="relative">
            <i class="fas fa-search absolute left-3 top-1/2 -translate-y-1/2 text-gray-400 text-sm" />
            <input
              v-model="searchQuery"
              type="text"
              :placeholder="$t('ai.searchLanguages')"
              class="w-full pl-9 pr-3 py-2 text-sm border border-gray-200 rounded-lg focus:border-teal-500 focus:ring-1 focus:ring-teal-200 outline-none"
            />
          </div>
        </div>

        <!-- Language list -->
        <div class="max-h-60 overflow-y-auto">
          <button
            v-for="lang in filteredLanguages"
            :key="lang.code"
            type="button"
            class="w-full flex items-center gap-3 px-3 py-2.5 text-sm hover:bg-teal-50 transition-colors"
            :class="{
              'bg-teal-50 text-teal-700': modelValue === lang.code,
            }"
            @click="selectLanguage(lang)"
          >
            <span v-if="showFlags" class="text-base w-6 text-center">{{ lang.flag }}</span>
            <div class="flex-1 text-left">
              <span class="font-medium text-gray-700">{{ lang.name }}</span>
              <span class="text-gray-400 ml-1.5">{{ lang.nativeName }}</span>
            </div>
            <span class="text-xs text-gray-400 uppercase">{{ lang.code }}</span>
            <i
              v-if="modelValue === lang.code"
              class="fas fa-check text-teal-500"
            />
          </button>

          <!-- Empty state -->
          <div v-if="filteredLanguages.length === 0" class="px-3 py-6 text-center">
            <i class="fas fa-search text-gray-300 text-2xl mb-2" />
            <p class="text-sm text-gray-500">{{ $t('ai.noLanguagesFound') }}</p>
          </div>
        </div>
      </div>
    </Transition>
  </div>
</template>

<style scoped>
.ai-translate-dropdown {
  min-width: 200px;
}
</style>
