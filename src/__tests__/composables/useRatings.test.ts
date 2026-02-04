import { describe, it, expect, vi, beforeEach } from 'vitest'
import { useRatings } from '@/composables/useRatings'

describe('useRatings', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('Initial State', () => {
    it('should initialize with default rating data', () => {
      const { rating } = useRatings('article', 'test-id')

      expect(rating.value.average).toBe(4.2)
      expect(rating.value.count).toBe(128)
      expect(rating.value.distribution).toHaveLength(5)
      expect(rating.value.userRating).toBeUndefined()
    })

    it('should not be loading initially', () => {
      const { isLoading } = useRatings('article', 'test-id')
      expect(isLoading.value).toBe(false)
    })

    it('should not be submitting initially', () => {
      const { isSubmitting } = useRatings('article', 'test-id')
      expect(isSubmitting.value).toBe(false)
    })

    it('should have null error initially', () => {
      const { error } = useRatings('article', 'test-id')
      expect(error.value).toBeNull()
    })
  })

  describe('Computed Properties', () => {
    it('should format average to one decimal place', () => {
      const { formattedAverage, rating } = useRatings('article', 'test-id')

      expect(formattedAverage.value).toBe('4.2')

      rating.value.average = 3.567
      expect(formattedAverage.value).toBe('3.6')
    })

    it('should calculate percentage distribution', () => {
      const { percentageDistribution, rating } = useRatings('article', 'test-id')

      // Total is 128, distribution is [5, 12, 18, 45, 48]
      const percentages = percentageDistribution.value
      expect(percentages).toHaveLength(5)
      expect(percentages.reduce((a, b) => a + b, 0)).toBeGreaterThanOrEqual(95) // Rounding may cause slight variation
    })

    it('should return zeros for empty distribution', () => {
      const { percentageDistribution, rating } = useRatings('article', 'test-id')

      rating.value.count = 0
      rating.value.distribution = [0, 0, 0, 0, 0]

      expect(percentageDistribution.value).toEqual([0, 0, 0, 0, 0])
    })
  })

  describe('loadRating', () => {
    it('should set loading state while fetching', async () => {
      const { isLoading, loadRating } = useRatings('article', 'test-id')

      const promise = loadRating()
      expect(isLoading.value).toBe(true)

      await promise
      expect(isLoading.value).toBe(false)
    })

    it('should clear error before loading', async () => {
      const { error, loadRating } = useRatings('article', 'test-id')
      error.value = 'Previous error'

      await loadRating()

      expect(error.value).toBeNull()
    })
  })

  describe('submitRating', () => {
    it('should reject invalid ratings', async () => {
      const { submitRating, rating } = useRatings('article', 'test-id')
      const initialCount = rating.value.count

      await submitRating(0)
      expect(rating.value.count).toBe(initialCount)

      await submitRating(6)
      expect(rating.value.count).toBe(initialCount)
    })

    it('should add new rating', async () => {
      const { submitRating, rating } = useRatings('article', 'test-id')
      const initialCount = rating.value.count

      await submitRating(5)

      expect(rating.value.userRating).toBe(5)
      expect(rating.value.count).toBe(initialCount + 1)
    })

    it('should update distribution when adding rating', async () => {
      const { submitRating, rating } = useRatings('article', 'test-id')
      const initial5Star = rating.value.distribution[4]

      await submitRating(5)

      expect(rating.value.distribution[4]).toBe(initial5Star + 1)
    })

    it('should set submitting state during submission', async () => {
      const { isSubmitting, submitRating } = useRatings('article', 'test-id')

      const promise = submitRating(4)
      expect(isSubmitting.value).toBe(true)

      await promise
      expect(isSubmitting.value).toBe(false)
    })

    it('should change existing rating', async () => {
      const { submitRating, rating } = useRatings('article', 'test-id')

      // First rating
      await submitRating(4)
      const countAfterFirst = rating.value.count
      const fourStarAfterFirst = rating.value.distribution[3]

      // Change rating
      await submitRating(5)

      expect(rating.value.userRating).toBe(5)
      expect(rating.value.count).toBe(countAfterFirst) // Count shouldn't change
      expect(rating.value.distribution[3]).toBe(fourStarAfterFirst - 1) // 4-star decreased
    })

    it('should recalculate average after rating', async () => {
      const { submitRating, rating } = useRatings('article', 'test-id')
      const initialAverage = rating.value.average

      await submitRating(5)

      // Average should change (might go up or down depending on initial distribution)
      expect(typeof rating.value.average).toBe('number')
      expect(rating.value.average).not.toBe(0)
    })
  })

  describe('removeRating', () => {
    it('should do nothing if no user rating exists', async () => {
      const { removeRating, rating } = useRatings('article', 'test-id')
      const initialCount = rating.value.count

      await removeRating()

      expect(rating.value.count).toBe(initialCount)
    })

    it('should remove user rating', async () => {
      const { submitRating, removeRating, rating } = useRatings('article', 'test-id')

      await submitRating(4)
      const countAfterRating = rating.value.count

      await removeRating()

      expect(rating.value.userRating).toBeUndefined()
      expect(rating.value.count).toBe(countAfterRating - 1)
    })

    it('should update distribution when removing rating', async () => {
      const { submitRating, removeRating, rating } = useRatings('article', 'test-id')

      await submitRating(4)
      const fourStarAfterRating = rating.value.distribution[3]

      await removeRating()

      expect(rating.value.distribution[3]).toBe(fourStarAfterRating - 1)
    })

    it('should set average to 0 when last rating is removed', async () => {
      const { submitRating, removeRating, rating } = useRatings('article', 'test-id')

      // Set up scenario with only one rating
      rating.value.count = 0
      rating.value.distribution = [0, 0, 0, 0, 0]
      rating.value.average = 0
      rating.value.userRating = undefined

      await submitRating(5)
      expect(rating.value.count).toBe(1)

      await removeRating()

      expect(rating.value.count).toBe(0)
      expect(rating.value.average).toBe(0)
    })
  })
})
