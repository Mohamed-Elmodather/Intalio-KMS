import { computed } from 'vue'
import { useI18n } from 'vue-i18n'

export function useDirection() {
  const { locale } = useI18n()

  const direction = computed(() => locale.value === 'ar' ? 'rtl' : 'ltr')
  const isRTL = computed(() => locale.value === 'ar')

  return {
    direction,
    isRTL
  }
}
