import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import { createPinia, setActivePinia } from 'pinia'
import EventDetailPage from '@/pages/events/EventDetailPage.vue'

// Mock vue-router
vi.mock('vue-router', () => ({
  useRouter: () => ({
    push: vi.fn(),
    back: vi.fn(),
  }),
  useRoute: () => ({
    params: { id: '1' },
  }),
}))

// Mock vue-i18n
vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string) => key,
  }),
}))

// Mock components
vi.mock('@/components/common/LoadingSpinner.vue', () => ({
  default: { template: '<div class="loading-spinner"></div>' },
}))
vi.mock('@/components/common', () => ({
  CommentsSection: { template: '<div class="comments"></div>' },
  SocialShareButtons: { template: '<div class="share"></div>' },
  RelatedContentCarousel: { template: '<div class="related"></div>' },
  BookmarkButton: { template: '<button class="bookmark"></button>' },
}))

function mountComponent() {
  return shallowMount(EventDetailPage, {
    global: {
      renderStubDefaultSlot: true,
      stubs: {
        teleport: true,
        'router-link': true,
      },
    },
  })
}

describe('EventDetailPage', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('should render event detail page', () => {
      const wrapper = mountComponent()
      expect(wrapper.exists()).toBe(true)
    })
  })

  describe('Loading State', () => {
    it('should have loading state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.isLoading).toBe(true)
    })
  })

  describe('Tab State', () => {
    it('should have active tab as description', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.activeTab).toBe('description')
    })
  })

  describe('RSVP State', () => {
    it('should have rsvp status null', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.rsvpStatus).toBeNull()
    })

    it('should have is rsvp loading state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.isRsvpLoading).toBe(false)
    })
  })

  describe('Modal States', () => {
    it('should have show share modal state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.showShareModal).toBe(false)
    })

    it('should have show calendar modal state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.showCalendarModal).toBe(false)
    })

    it('should have show all attendees state', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.showAllAttendees).toBe(false)
    })
  })

  describe('Event Data', () => {
    it('should have event ref', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.event).toBeDefined()
    })

    it('should have event title', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.event.title).toBeDefined()
    })

    it('should have event description', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.event.description).toBeDefined()
    })

    it('should have event location', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.event.location).toBeDefined()
    })

    it('should have event dates', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.event.startDate).toBeDefined()
      expect(vm.event.endDate).toBeDefined()
    })

    it('should have event category', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.event.category).toBeDefined()
    })

    it('should have event status', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.event.status).toBeDefined()
    })

    it('should have event capacity', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.event.capacity).toBeDefined()
    })

    it('should have event organizer', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.event.organizer).toBeDefined()
    })

    it('should have event tags', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.event.tags).toBeDefined()
      expect(Array.isArray(vm.event.tags)).toBe(true)
    })

    it('should have virtual event flag', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.event.isVirtual).toBe('boolean')
    })

    it('should have hybrid event flag', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.event.isHybrid).toBe('boolean')
    })
  })

  describe('Attendees', () => {
    it('should have attendees array', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.attendees).toBeDefined()
      expect(Array.isArray(vm.attendees)).toBe(true)
    })

    it('should have attendee properties', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const attendee = vm.attendees[0]
      expect(attendee.id).toBeDefined()
      expect(attendee.name).toBeDefined()
      expect(attendee.status).toBeDefined()
    })
  })

  describe('Agenda', () => {
    it('should have agenda array', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.agenda).toBeDefined()
      expect(Array.isArray(vm.agenda)).toBe(true)
    })

    it('should have agenda item properties', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const item = vm.agenda[0]
      expect(item.id).toBeDefined()
      expect(item.time).toBeDefined()
      expect(item.title).toBeDefined()
      expect(item.duration).toBeDefined()
      expect(item.type).toBeDefined()
    })
  })

  describe('Speakers', () => {
    it('should have speakers array', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.speakers).toBeDefined()
      expect(Array.isArray(vm.speakers)).toBe(true)
    })

    it('should have speaker properties', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const speaker = vm.speakers[0]
      expect(speaker.id).toBeDefined()
      expect(speaker.name).toBeDefined()
      expect(speaker.title).toBeDefined()
      expect(speaker.company).toBeDefined()
      expect(speaker.bio).toBeDefined()
    })
  })

  describe('Documents', () => {
    it('should have documents array', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.documents).toBeDefined()
      expect(Array.isArray(vm.documents)).toBe(true)
    })
  })
})
