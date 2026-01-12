import { ref } from 'vue'

export interface ShareData {
  title: string
  description?: string
  url: string
  image?: string
}

export function useSharing() {
  const isSharing = ref(false)
  const showShareModal = ref(false)
  const shareData = ref<ShareData | null>(null)
  const copySuccess = ref(false)

  function openShareModal(data: ShareData) {
    shareData.value = data
    showShareModal.value = true
  }

  function closeShareModal() {
    showShareModal.value = false
    shareData.value = null
  }

  async function copyLink(url?: string) {
    const linkToCopy = url || shareData.value?.url || window.location.href

    try {
      await navigator.clipboard.writeText(linkToCopy)
      copySuccess.value = true
      setTimeout(() => {
        copySuccess.value = false
      }, 2000)
      return true
    } catch (e) {
      console.error('Failed to copy link:', e)
      return false
    }
  }

  function shareToTwitter(data?: ShareData) {
    const { title, url } = data || shareData.value || { title: document.title, url: window.location.href }
    const twitterUrl = `https://twitter.com/intent/tweet?text=${encodeURIComponent(title)}&url=${encodeURIComponent(url)}`
    window.open(twitterUrl, '_blank', 'width=550,height=420')
  }

  function shareToLinkedIn(data?: ShareData) {
    const { title, url } = data || shareData.value || { title: document.title, url: window.location.href }
    const linkedInUrl = `https://www.linkedin.com/sharing/share-offsite/?url=${encodeURIComponent(url)}`
    window.open(linkedInUrl, '_blank', 'width=550,height=420')
  }

  function shareToFacebook(data?: ShareData) {
    const { url } = data || shareData.value || { url: window.location.href }
    const facebookUrl = `https://www.facebook.com/sharer/sharer.php?u=${encodeURIComponent(url)}`
    window.open(facebookUrl, '_blank', 'width=550,height=420')
  }

  function shareViaEmail(data?: ShareData) {
    const { title, description, url } = data || shareData.value || {
      title: document.title,
      url: window.location.href,
      description: ''
    }
    const subject = encodeURIComponent(title)
    const body = encodeURIComponent(`${description || ''}\n\n${url}`)
    window.location.href = `mailto:?subject=${subject}&body=${body}`
  }

  async function shareNative(data?: ShareData) {
    const shareInfo = data || shareData.value || {
      title: document.title,
      url: window.location.href
    }

    if (navigator.share) {
      try {
        isSharing.value = true
        await navigator.share({
          title: shareInfo.title,
          text: shareInfo.description,
          url: shareInfo.url
        })
        return true
      } catch (e) {
        if ((e as Error).name !== 'AbortError') {
          console.error('Error sharing:', e)
        }
        return false
      } finally {
        isSharing.value = false
      }
    } else {
      // Fallback to modal
      openShareModal(shareInfo)
      return false
    }
  }

  function generateShareUrl(contentType: string, contentId: string): string {
    const baseUrl = window.location.origin
    return `${baseUrl}/${contentType}/${contentId}`
  }

  return {
    isSharing,
    showShareModal,
    shareData,
    copySuccess,
    openShareModal,
    closeShareModal,
    copyLink,
    shareToTwitter,
    shareToLinkedIn,
    shareToFacebook,
    shareViaEmail,
    shareNative,
    generateShareUrl
  }
}
