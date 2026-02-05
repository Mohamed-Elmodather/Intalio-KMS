import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import AIEntityHighlight from '@/components/ai/AIEntityHighlight.vue'

// Mock vue-i18n
vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string) => key,
  }),
}))

const mockEntities = [
  { text: 'John', type: 'person' as const, confidence: 0.95, startIndex: 0, endIndex: 4 },
  { text: 'Acme Corp', type: 'organization' as const, confidence: 0.88, startIndex: 13, endIndex: 22 },
  { text: 'New York', type: 'location' as const, confidence: 0.92, startIndex: 26, endIndex: 34 },
]

function mountComponent(props = {}) {
  return shallowMount(AIEntityHighlight, {
    props: {
      text: 'John works at Acme Corp in New York.',
      entities: mockEntities,
      ...props,
    },
    global: {
      mocks: {
        $t: (key: string) => key,
      },
    },
  })
}

describe('AIEntityHighlight', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('should render the component', () => {
      const wrapper = mountComponent()
      expect(wrapper.exists()).toBe(true)
      expect(wrapper.find('.ai-entity-highlight').exists()).toBe(true)
    })

    it('should render text without entities', () => {
      const wrapper = mountComponent({ entities: [] })
      expect(wrapper.text()).toContain('John works at Acme Corp in New York.')
    })

    it('should render entity tags', () => {
      const wrapper = mountComponent()
      const entityTags = wrapper.findAll('.entity-tag')
      expect(entityTags.length).toBe(3)
    })
  })

  describe('Segments Computed', () => {
    it('should split text into segments with entities', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.segments.length).toBeGreaterThan(0)
    })

    it('should return single segment when no entities', () => {
      const wrapper = mountComponent({ entities: [] })
      const vm = wrapper.vm as any
      expect(vm.segments.length).toBe(1)
      expect(vm.segments[0].isEntity).toBe(false)
    })

    it('should mark entity segments correctly', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      const entitySegments = vm.segments.filter((s: any) => s.isEntity)
      expect(entitySegments.length).toBe(3)
    })
  })

  describe('Entity Colors', () => {
    it('should apply person entity colors', () => {
      const wrapper = mountComponent({
        entities: [{ text: 'John', type: 'person', confidence: 0.9, startIndex: 0, endIndex: 4 }],
      })
      const tag = wrapper.find('.entity-tag')
      expect(tag.classes()).toContain('bg-blue-100')
      expect(tag.classes()).toContain('text-blue-700')
    })

    it('should apply organization entity colors', () => {
      const wrapper = mountComponent({
        text: 'Acme Corp',
        entities: [{ text: 'Acme Corp', type: 'organization', confidence: 0.9, startIndex: 0, endIndex: 9 }],
      })
      const tag = wrapper.find('.entity-tag')
      expect(tag.classes()).toContain('bg-purple-100')
      expect(tag.classes()).toContain('text-purple-700')
    })

    it('should apply location entity colors', () => {
      const wrapper = mountComponent({
        text: 'New York',
        entities: [{ text: 'New York', type: 'location', confidence: 0.9, startIndex: 0, endIndex: 8 }],
      })
      const tag = wrapper.find('.entity-tag')
      expect(tag.classes()).toContain('bg-green-100')
      expect(tag.classes()).toContain('text-green-700')
    })

    it('should apply date entity colors', () => {
      const wrapper = mountComponent({
        text: 'January 1st',
        entities: [{ text: 'January 1st', type: 'date', confidence: 0.9, startIndex: 0, endIndex: 11 }],
      })
      const tag = wrapper.find('.entity-tag')
      expect(tag.classes()).toContain('bg-amber-100')
      expect(tag.classes()).toContain('text-amber-700')
    })
  })

  describe('Entity Icons', () => {
    it('should show person icon', () => {
      const wrapper = mountComponent({
        text: 'John',
        entities: [{ text: 'John', type: 'person', confidence: 0.9, startIndex: 0, endIndex: 4 }],
      })
      expect(wrapper.find('.fa-user').exists()).toBe(true)
    })

    it('should show organization icon', () => {
      const wrapper = mountComponent({
        text: 'Acme',
        entities: [{ text: 'Acme', type: 'organization', confidence: 0.9, startIndex: 0, endIndex: 4 }],
      })
      expect(wrapper.find('.fa-building').exists()).toBe(true)
    })

    it('should show location icon', () => {
      const wrapper = mountComponent({
        text: 'NYC',
        entities: [{ text: 'NYC', type: 'location', confidence: 0.9, startIndex: 0, endIndex: 3 }],
      })
      expect(wrapper.find('.fa-map-marker-alt').exists()).toBe(true)
    })
  })

  describe('Interactive Mode', () => {
    it('should apply cursor-pointer when interactive', () => {
      const wrapper = mountComponent({ interactive: true })
      const tag = wrapper.find('.entity-tag')
      expect(tag.classes()).toContain('cursor-pointer')
    })

    it('should not apply cursor-pointer when not interactive', () => {
      const wrapper = mountComponent({ interactive: false })
      const tag = wrapper.find('.entity-tag')
      expect(tag.classes()).not.toContain('cursor-pointer')
    })

    it('should emit entityClick when entity clicked and interactive', async () => {
      const wrapper = mountComponent({ interactive: true })
      const tag = wrapper.find('.entity-tag')
      await tag.trigger('click')
      expect(wrapper.emitted('entityClick')).toBeTruthy()
    })

    it('should not emit entityClick when not interactive', async () => {
      const wrapper = mountComponent({ interactive: false })
      const tag = wrapper.find('.entity-tag')
      await tag.trigger('click')
      expect(wrapper.emitted('entityClick')).toBeFalsy()
    })
  })

  describe('Tooltips', () => {
    it('should show tooltip when showTooltips is true', () => {
      const wrapper = mountComponent({ showTooltips: true })
      const tag = wrapper.find('.entity-tag')
      expect(tag.attributes('title')).toBeDefined()
    })

    it('should not show tooltip when showTooltips is false', () => {
      const wrapper = mountComponent({ showTooltips: false })
      const tag = wrapper.find('.entity-tag')
      expect(tag.attributes('title')).toBeUndefined()
    })
  })

  describe('Entity Position Helpers', () => {
    it('should handle startIndex property', () => {
      const wrapper = mountComponent({
        text: 'Test text',
        entities: [{ text: 'Test', type: 'person', confidence: 0.9, startIndex: 0, endIndex: 4 }],
      })
      const vm = wrapper.vm as any
      expect(vm.segments.find((s: any) => s.isEntity)?.text).toBe('Test')
    })

    it('should handle startOffset property as fallback', () => {
      const wrapper = mountComponent({
        text: 'Test text',
        entities: [{ text: 'Test', type: 'person', confidence: 0.9, startOffset: 0, endOffset: 4 }],
      })
      const vm = wrapper.vm as any
      expect(vm.segments.find((s: any) => s.isEntity)?.text).toBe('Test')
    })
  })
})
