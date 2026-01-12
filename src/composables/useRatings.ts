import { ref, computed } from 'vue'
import type { Rating } from '@/types/detail-pages'

export function useRatings(contentType: string, contentId: string) {
  const rating = ref<Rating>({
    average: 4.2,
    count: 128,
    distribution: [5, 12, 18, 45, 48], // 1-star to 5-star
    userRating: undefined
  })
  const isLoading = ref(false)
  const isSubmitting = ref(false)
  const error = ref<string | null>(null)

  const formattedAverage = computed(() => rating.value.average.toFixed(1))

  const percentageDistribution = computed(() => {
    const total = rating.value.count
    if (total === 0) return [0, 0, 0, 0, 0]
    return rating.value.distribution.map(count => Math.round((count / total) * 100))
  })

  async function loadRating() {
    isLoading.value = true
    error.value = null

    try {
      // Simulate API call
      await new Promise(resolve => setTimeout(resolve, 500))
      // Rating is already initialized with mock data
    } catch (e) {
      error.value = 'Failed to load rating'
      console.error(e)
    } finally {
      isLoading.value = false
    }
  }

  async function submitRating(stars: number) {
    if (stars < 1 || stars > 5) return

    isSubmitting.value = true
    error.value = null

    try {
      await new Promise(resolve => setTimeout(resolve, 500))

      // Update user's rating
      const previousRating = rating.value.userRating

      if (previousRating) {
        // User is changing their rating
        rating.value.distribution[previousRating - 1]--
      } else {
        // New rating
        rating.value.count++
      }

      rating.value.distribution[stars - 1]++
      rating.value.userRating = stars

      // Recalculate average
      const total = rating.value.distribution.reduce((sum, count, index) => {
        return sum + count * (index + 1)
      }, 0)
      rating.value.average = total / rating.value.count

    } catch (e) {
      error.value = 'Failed to submit rating'
      console.error(e)
    } finally {
      isSubmitting.value = false
    }
  }

  async function removeRating() {
    if (!rating.value.userRating) return

    isSubmitting.value = true

    try {
      await new Promise(resolve => setTimeout(resolve, 300))

      const previousRating = rating.value.userRating
      rating.value.distribution[previousRating - 1]--
      rating.value.count--
      rating.value.userRating = undefined

      // Recalculate average
      if (rating.value.count > 0) {
        const total = rating.value.distribution.reduce((sum, count, index) => {
          return sum + count * (index + 1)
        }, 0)
        rating.value.average = total / rating.value.count
      } else {
        rating.value.average = 0
      }

    } catch (e) {
      error.value = 'Failed to remove rating'
    } finally {
      isSubmitting.value = false
    }
  }

  return {
    rating,
    formattedAverage,
    percentageDistribution,
    isLoading,
    isSubmitting,
    error,
    loadRating,
    submitRating,
    removeRating
  }
}
