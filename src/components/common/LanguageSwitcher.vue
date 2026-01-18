<script setup lang="ts">
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { useLocaleStore } from '@/stores/locale'

const { locale } = useI18n()
const localeStore = useLocaleStore()

const isArabic = computed(() => localeStore.currentLocale === 'ar')

function toggleLanguage() {
  const newLocale = localeStore.currentLocale === 'en' ? 'ar' : 'en'
  localeStore.setLocale(newLocale)
  locale.value = newLocale
}
</script>

<template>
  <button
    @click="toggleLanguage"
    class="flex items-center gap-2 px-3 py-2 rounded-lg text-sm font-medium transition-all duration-200 hover:bg-gray-100"
    :title="isArabic ? 'Switch to English' : 'التبديل للعربية'"
  >
    <i class="fas fa-globe text-gray-500"></i>
    <span class="text-gray-700">{{ isArabic ? 'EN' : 'عربي' }}</span>
  </button>
</template>
