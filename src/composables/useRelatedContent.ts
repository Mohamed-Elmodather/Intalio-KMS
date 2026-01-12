import { ref } from 'vue'
import type { RelatedItem } from '@/types/detail-pages'

export function useRelatedContent(contentType: string, contentId: string) {
  const relatedItems = ref<RelatedItem[]>([])
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  // Mock related content based on content type
  const mockRelatedContent: Record<string, RelatedItem[]> = {
    article: [
      {
        id: 'article-2',
        type: 'article',
        title: 'Best Practices for Knowledge Management',
        thumbnail: 'https://picsum.photos/seed/article2/400/300',
        description: 'Learn the essential practices for effective knowledge sharing.',
        metadata: '5 min read • 1.2k views',
        date: new Date(Date.now() - 86400000 * 2)
      },
      {
        id: 'article-3',
        type: 'article',
        title: 'Building a Culture of Collaboration',
        thumbnail: 'https://picsum.photos/seed/article3/400/300',
        description: 'How to foster collaboration in distributed teams.',
        metadata: '7 min read • 890 views',
        date: new Date(Date.now() - 86400000 * 5)
      },
      {
        id: 'article-4',
        type: 'article',
        title: 'The Future of Enterprise Software',
        thumbnail: 'https://picsum.photos/seed/article4/400/300',
        description: 'Trends shaping the next generation of business applications.',
        metadata: '10 min read • 2.1k views',
        date: new Date(Date.now() - 86400000 * 7)
      },
      {
        id: 'course-1',
        type: 'course',
        title: 'Advanced Product Management',
        thumbnail: 'https://picsum.photos/seed/course1/400/300',
        description: 'Master the skills needed to lead product teams.',
        metadata: '12 lessons • 4 hours',
        date: new Date(Date.now() - 86400000 * 10)
      }
    ],
    document: [
      {
        id: 'doc-2',
        type: 'document',
        title: 'Q4 Financial Report 2024',
        thumbnail: 'https://picsum.photos/seed/doc2/400/300',
        description: 'Quarterly financial performance analysis.',
        metadata: 'PDF • 2.4 MB',
        date: new Date(Date.now() - 86400000 * 3)
      },
      {
        id: 'doc-3',
        type: 'document',
        title: 'Product Roadmap 2025',
        thumbnail: 'https://picsum.photos/seed/doc3/400/300',
        description: 'Strategic product development plan.',
        metadata: 'PPTX • 5.8 MB',
        date: new Date(Date.now() - 86400000 * 6)
      },
      {
        id: 'doc-4',
        type: 'document',
        title: 'Employee Handbook',
        thumbnail: 'https://picsum.photos/seed/doc4/400/300',
        description: 'Company policies and guidelines.',
        metadata: 'PDF • 1.2 MB',
        date: new Date(Date.now() - 86400000 * 14)
      }
    ],
    media: [
      {
        id: 'media-2',
        type: 'media',
        title: 'Product Demo: New Features',
        thumbnail: 'https://picsum.photos/seed/media2/400/300',
        description: 'Walkthrough of latest platform features.',
        metadata: '15:30 • 3.5k views',
        date: new Date(Date.now() - 86400000 * 4)
      },
      {
        id: 'media-3',
        type: 'media',
        title: 'CEO Quarterly Update',
        thumbnail: 'https://picsum.photos/seed/media3/400/300',
        description: 'Company performance and vision update.',
        metadata: '25:00 • 8.2k views',
        date: new Date(Date.now() - 86400000 * 8)
      }
    ],
    course: [
      {
        id: 'course-2',
        type: 'course',
        title: 'Leadership Fundamentals',
        thumbnail: 'https://picsum.photos/seed/course2/400/300',
        description: 'Essential skills for new managers.',
        metadata: '8 lessons • 2.5 hours',
        date: new Date(Date.now() - 86400000 * 5)
      },
      {
        id: 'course-3',
        type: 'course',
        title: 'Data Analytics for Business',
        thumbnail: 'https://picsum.photos/seed/course3/400/300',
        description: 'Make data-driven decisions.',
        metadata: '15 lessons • 6 hours',
        date: new Date(Date.now() - 86400000 * 12)
      }
    ],
    event: [
      {
        id: 'event-2',
        type: 'event',
        title: 'Quarterly Town Hall',
        thumbnail: 'https://picsum.photos/seed/event2/400/300',
        description: 'Company-wide update and Q&A session.',
        metadata: 'Jan 25, 2026 • Virtual',
        date: new Date(Date.now() + 86400000 * 7)
      },
      {
        id: 'event-3',
        type: 'event',
        title: 'Product Workshop',
        thumbnail: 'https://picsum.photos/seed/event3/400/300',
        description: 'Hands-on session with the product team.',
        metadata: 'Feb 1, 2026 • Dubai Office',
        date: new Date(Date.now() + 86400000 * 14)
      }
    ],
    poll: [
      {
        id: 'poll-2',
        type: 'poll',
        title: 'Preferred Meeting Times',
        description: 'Help us schedule better team meetings.',
        metadata: '156 votes • Active',
        date: new Date(Date.now() - 86400000 * 2)
      },
      {
        id: 'poll-3',
        type: 'poll',
        title: 'Office Snack Preferences',
        description: 'Vote for your favorite snacks.',
        metadata: '89 votes • Ended',
        date: new Date(Date.now() - 86400000 * 10)
      }
    ]
  }

  async function loadRelatedContent(limit: number = 4) {
    isLoading.value = true
    error.value = null

    try {
      await new Promise(resolve => setTimeout(resolve, 600))
      const items = mockRelatedContent[contentType] || []
      relatedItems.value = items.slice(0, limit)
    } catch (e) {
      error.value = 'Failed to load related content'
      console.error(e)
    } finally {
      isLoading.value = false
    }
  }

  async function loadMixedContent(limit: number = 6) {
    isLoading.value = true
    error.value = null

    try {
      await new Promise(resolve => setTimeout(resolve, 600))

      // Get items from multiple content types
      const allItems: RelatedItem[] = []
      Object.values(mockRelatedContent).forEach(items => {
        allItems.push(...items.slice(0, 2))
      })

      // Shuffle and limit
      const shuffled = allItems.sort(() => Math.random() - 0.5)
      relatedItems.value = shuffled.slice(0, limit)
    } catch (e) {
      error.value = 'Failed to load related content'
      console.error(e)
    } finally {
      isLoading.value = false
    }
  }

  function getContentTypeIcon(type: RelatedItem['type']): string {
    const icons: Record<string, string> = {
      article: 'fas fa-file-alt',
      document: 'fas fa-file-pdf',
      media: 'fas fa-play-circle',
      course: 'fas fa-graduation-cap',
      event: 'fas fa-calendar-alt',
      poll: 'fas fa-poll'
    }
    return icons[type] || 'fas fa-file'
  }

  function getContentTypeColor(type: RelatedItem['type']): string {
    const colors: Record<string, string> = {
      article: 'bg-blue-100 text-blue-600',
      document: 'bg-red-100 text-red-600',
      media: 'bg-purple-100 text-purple-600',
      course: 'bg-green-100 text-green-600',
      event: 'bg-orange-100 text-orange-600',
      poll: 'bg-teal-100 text-teal-600'
    }
    return colors[type] || 'bg-gray-100 text-gray-600'
  }

  function getContentTypeLabel(type: RelatedItem['type']): string {
    const labels: Record<string, string> = {
      article: 'Article',
      document: 'Document',
      media: 'Media',
      course: 'Course',
      event: 'Event',
      poll: 'Poll'
    }
    return labels[type] || type
  }

  return {
    relatedItems,
    isLoading,
    error,
    loadRelatedContent,
    loadMixedContent,
    getContentTypeIcon,
    getContentTypeColor,
    getContentTypeLabel
  }
}
