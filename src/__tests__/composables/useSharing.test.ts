import { describe, it, expect, vi, beforeEach } from 'vitest'
import { useSharing } from '@/composables/useSharing'

// Mock window methods
const mockOpen = vi.fn()
const mockClipboard = {
  writeText: vi.fn(),
}

vi.stubGlobal('open', mockOpen)
Object.defineProperty(navigator, 'clipboard', {
  value: mockClipboard,
  writable: true,
})

describe('useSharing', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    mockClipboard.writeText.mockResolvedValue(undefined)
  })

  describe('Initial State', () => {
    it('should not be sharing initially', () => {
      const { isSharing } = useSharing()
      expect(isSharing.value).toBe(false)
    })

    it('should not show share modal initially', () => {
      const { showShareModal } = useSharing()
      expect(showShareModal.value).toBe(false)
    })

    it('should have null share data initially', () => {
      const { shareData } = useSharing()
      expect(shareData.value).toBeNull()
    })

    it('should have copy success as false initially', () => {
      const { copySuccess } = useSharing()
      expect(copySuccess.value).toBe(false)
    })
  })

  describe('openShareModal', () => {
    it('should open modal with share data', () => {
      const { openShareModal, showShareModal, shareData } = useSharing()

      openShareModal({
        title: 'Test Title',
        url: 'https://example.com/test',
        description: 'Test description',
      })

      expect(showShareModal.value).toBe(true)
      expect(shareData.value?.title).toBe('Test Title')
      expect(shareData.value?.url).toBe('https://example.com/test')
    })
  })

  describe('closeShareModal', () => {
    it('should close modal and clear share data', () => {
      const { openShareModal, closeShareModal, showShareModal, shareData } =
        useSharing()

      openShareModal({
        title: 'Test',
        url: 'https://example.com',
      })

      closeShareModal()

      expect(showShareModal.value).toBe(false)
      expect(shareData.value).toBeNull()
    })
  })

  describe('copyLink', () => {
    it('should copy URL to clipboard', async () => {
      const { copyLink } = useSharing()

      const result = await copyLink('https://example.com/test')

      expect(mockClipboard.writeText).toHaveBeenCalledWith(
        'https://example.com/test'
      )
      expect(result).toBe(true)
    })

    it('should set copy success flag', async () => {
      vi.useFakeTimers()
      const { copyLink, copySuccess } = useSharing()

      await copyLink('https://example.com')

      expect(copySuccess.value).toBe(true)

      vi.advanceTimersByTime(2000)
      expect(copySuccess.value).toBe(false)

      vi.useRealTimers()
    })

    it('should copy share data URL if no URL provided', async () => {
      const { openShareModal, copyLink } = useSharing()

      openShareModal({
        title: 'Test',
        url: 'https://share-data-url.com',
      })

      await copyLink()

      expect(mockClipboard.writeText).toHaveBeenCalledWith(
        'https://share-data-url.com'
      )
    })

    it('should return false on clipboard error', async () => {
      mockClipboard.writeText.mockRejectedValueOnce(new Error('Clipboard error'))

      const { copyLink } = useSharing()
      const result = await copyLink('https://example.com')

      expect(result).toBe(false)
    })
  })

  describe('shareToTwitter', () => {
    it('should open Twitter share URL', () => {
      const { shareToTwitter } = useSharing()

      shareToTwitter({
        title: 'Test Title',
        url: 'https://example.com/test',
      })

      expect(mockOpen).toHaveBeenCalledWith(
        expect.stringContaining('twitter.com/intent/tweet'),
        '_blank',
        expect.any(String)
      )
    })

    it('should encode title and URL', () => {
      const { shareToTwitter } = useSharing()

      shareToTwitter({
        title: 'Title with spaces & symbols',
        url: 'https://example.com/path?query=value',
      })

      const calledUrl = mockOpen.mock.calls[0][0]
      expect(calledUrl).toContain(encodeURIComponent('Title with spaces & symbols'))
      expect(calledUrl).toContain(
        encodeURIComponent('https://example.com/path?query=value')
      )
    })
  })

  describe('shareToLinkedIn', () => {
    it('should open LinkedIn share URL', () => {
      const { shareToLinkedIn } = useSharing()

      shareToLinkedIn({
        title: 'Test',
        url: 'https://example.com',
      })

      expect(mockOpen).toHaveBeenCalledWith(
        expect.stringContaining('linkedin.com/sharing'),
        '_blank',
        expect.any(String)
      )
    })
  })

  describe('shareToFacebook', () => {
    it('should open Facebook share URL', () => {
      const { shareToFacebook } = useSharing()

      shareToFacebook({
        title: 'Test',
        url: 'https://example.com',
      })

      expect(mockOpen).toHaveBeenCalledWith(
        expect.stringContaining('facebook.com/sharer'),
        '_blank',
        expect.any(String)
      )
    })
  })

  describe('shareViaEmail', () => {
    it('should set mailto href', () => {
      const { shareViaEmail } = useSharing()

      // Mock window.location
      const originalHref = window.location.href
      delete (window as any).location
      window.location = { href: '' } as any

      shareViaEmail({
        title: 'Test Subject',
        description: 'Test body',
        url: 'https://example.com',
      })

      expect(window.location.href).toContain('mailto:')
      expect(window.location.href).toContain(encodeURIComponent('Test Subject'))

      window.location.href = originalHref
    })
  })

  describe('shareNative', () => {
    it('should use navigator.share when available', async () => {
      const mockShare = vi.fn().mockResolvedValue(undefined)
      Object.defineProperty(navigator, 'share', {
        value: mockShare,
        writable: true,
      })

      const { shareNative, isSharing } = useSharing()

      const promise = shareNative({
        title: 'Test',
        url: 'https://example.com',
        description: 'Description',
      })

      expect(isSharing.value).toBe(true)

      const result = await promise

      expect(result).toBe(true)
      expect(mockShare).toHaveBeenCalledWith({
        title: 'Test',
        text: 'Description',
        url: 'https://example.com',
      })
      expect(isSharing.value).toBe(false)
    })

    it('should fallback to modal when navigator.share unavailable', async () => {
      Object.defineProperty(navigator, 'share', {
        value: undefined,
        writable: true,
      })

      const { shareNative, showShareModal } = useSharing()

      const result = await shareNative({
        title: 'Test',
        url: 'https://example.com',
      })

      expect(result).toBe(false)
      expect(showShareModal.value).toBe(true)
    })
  })

  describe('generateShareUrl', () => {
    it('should generate correct URL', () => {
      const { generateShareUrl } = useSharing()

      const url = generateShareUrl('article', '123')

      expect(url).toContain('/article/123')
    })
  })
})
