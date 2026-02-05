import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import AIMessageContent from '@/components/ai/AIMessageContent.vue'

// Mock vue-i18n
vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string) => key,
  }),
}))

// Mock clipboard API
const mockWriteText = vi.fn().mockResolvedValue(undefined)
Object.defineProperty(navigator, 'clipboard', {
  value: {
    writeText: mockWriteText,
  },
  writable: true,
  configurable: true,
})

const mockAnalysis = {
  entities: [
    { text: 'John', type: 'person', confidence: 0.95 },
    { text: 'Acme Corp', type: 'organization', confidence: 0.88 },
  ],
  sentiment: {
    overall: 'positive' as const,
    score: 0.75,
  },
  classification: {
    primaryCategory: 'Technology',
    confidence: 0.9,
    suggestedTags: ['tech', 'software', 'ai'],
  },
  keyPoints: ['Point 1', 'Point 2', 'Point 3'],
}

function mountComponent(props = {}) {
  return shallowMount(AIMessageContent, {
    props: {
      content: 'Test content message',
      ...props,
    },
    global: {
      mocks: {
        $t: (key: string) => key,
      },
      stubs: {
        AISentimentBadge: true,
        AIConfidenceBar: true,
      },
    },
  })
}

describe('AIMessageContent', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('should render the component', () => {
      const wrapper = mountComponent()
      expect(wrapper.exists()).toBe(true)
      expect(wrapper.find('.ai-message-content').exists()).toBe(true)
    })

    it('should render content', () => {
      const wrapper = mountComponent({ content: 'Hello World' })
      expect(wrapper.text()).toContain('Hello World')
    })

    it('should render with prose classes', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.prose').exists()).toBe(true)
    })
  })

  describe('Streaming', () => {
    it('should show streaming cursor when streaming', () => {
      const wrapper = mountComponent({ isStreaming: true })
      expect(wrapper.find('.streaming-cursor').exists()).toBe(true)
    })

    it('should hide streaming cursor when not streaming', () => {
      const wrapper = mountComponent({ isStreaming: false })
      expect(wrapper.find('.streaming-cursor').exists()).toBe(false)
    })

    it('should apply streaming class when streaming', () => {
      const wrapper = mountComponent({ isStreaming: true })
      expect(wrapper.find('.streaming').exists()).toBe(true)
    })
  })

  describe('Rendered Content', () => {
    it('should sanitize dangerous HTML (XSS prevention)', () => {
      const wrapper = mountComponent({ content: '<script>alert("xss")</script>' })
      const vm = wrapper.vm as any
      // Script tags are escaped to prevent XSS - they become &lt;script&gt;
      // The actual <script> tag is not present as executable HTML
      expect(vm.renderedContent).not.toMatch(/<script[^&]/)
      expect(vm.renderedContent).toContain('&lt;script&gt;')
    })

    it('should convert bold markdown', () => {
      const wrapper = mountComponent({ content: '**bold text**' })
      const vm = wrapper.vm as any
      expect(vm.renderedContent).toContain('<strong>bold text</strong>')
    })

    it('should convert italic markdown', () => {
      const wrapper = mountComponent({ content: '*italic text*' })
      const vm = wrapper.vm as any
      expect(vm.renderedContent).toContain('<em>italic text</em>')
    })

    it('should convert inline code', () => {
      const wrapper = mountComponent({ content: '`code`' })
      const vm = wrapper.vm as any
      expect(vm.renderedContent).toContain('inline-code')
    })

    it('should convert headers', () => {
      const wrapper = mountComponent({ content: '# Header 1\n## Header 2\n### Header 3' })
      const vm = wrapper.vm as any
      expect(vm.renderedContent).toContain('<h1 class="ai-heading">Header 1</h1>')
      expect(vm.renderedContent).toContain('<h2 class="ai-heading">Header 2</h2>')
      expect(vm.renderedContent).toContain('<h3 class="ai-heading">Header 3</h3>')
    })

    it('should convert links', () => {
      const wrapper = mountComponent({ content: '[link text](https://example.com)' })
      const vm = wrapper.vm as any
      expect(vm.renderedContent).toContain('href="https://example.com"')
      expect(vm.renderedContent).toContain('link text')
    })

    it('should handle greater-than character safely', () => {
      const wrapper = mountComponent({ content: '> Quote text' })
      const vm = wrapper.vm as any
      // > is handled safely by DOMPurify
      expect(vm.renderedContent).toContain('Quote text')
      // No dangerous content injection
      expect(vm.renderedContent).not.toContain('<script>')
    })

    it('should convert horizontal rules', () => {
      const wrapper = mountComponent({ content: '---' })
      const vm = wrapper.vm as any
      expect(vm.renderedContent).toContain('ai-divider')
    })
  })

  describe('Entity Colors', () => {
    it('should return person entity color', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getEntityColor('person')).toContain('bg-blue-100')
    })

    it('should return organization entity color', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getEntityColor('organization')).toContain('bg-purple-100')
    })

    it('should return location entity color', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getEntityColor('location')).toContain('bg-green-100')
    })

    it('should return default color for unknown type', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getEntityColor('unknown')).toContain('bg-gray-100')
    })
  })

  describe('Entity Icons', () => {
    it('should return person icon', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getEntityIcon('person')).toBe('fas fa-user')
    })

    it('should return organization icon', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getEntityIcon('organization')).toBe('fas fa-building')
    })

    it('should return location icon', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getEntityIcon('location')).toBe('fas fa-map-marker-alt')
    })

    it('should return default icon for unknown type', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.getEntityIcon('unknown')).toBe('fas fa-tag')
    })
  })

  describe('Analysis Section', () => {
    it('should show analysis section when analysis provided', () => {
      const wrapper = mountComponent({ analysis: mockAnalysis })
      expect(wrapper.find('.ai-analysis-section').exists()).toBe(true)
    })

    it('should hide analysis section when no analysis', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.ai-analysis-section').exists()).toBe(false)
    })

    it('should show entities when provided', () => {
      const wrapper = mountComponent({ analysis: mockAnalysis })
      expect(wrapper.text()).toContain('ai.messageContent.extractedEntities')
    })

    it('should show sentiment when provided', () => {
      const wrapper = mountComponent({ analysis: mockAnalysis })
      expect(wrapper.text()).toContain('ai.messageContent.sentimentAnalysis')
    })

    it('should show classification when provided', () => {
      const wrapper = mountComponent({ analysis: mockAnalysis })
      expect(wrapper.text()).toContain('ai.messageContent.classification')
      expect(wrapper.text()).toContain('Technology')
    })

    it('should show key points when provided', () => {
      const wrapper = mountComponent({ analysis: mockAnalysis })
      expect(wrapper.text()).toContain('ai.messageContent.keyPoints')
      expect(wrapper.text()).toContain('Point 1')
    })

    it('should show suggested tags', () => {
      const wrapper = mountComponent({ analysis: mockAnalysis })
      expect(wrapper.text()).toContain('#tech')
      expect(wrapper.text()).toContain('#software')
    })
  })

  describe('Copy Button', () => {
    it('should show copy button when showCopyButton is true', () => {
      const wrapper = mountComponent({ showCopyButton: true, isStreaming: false })
      expect(wrapper.find('.copy-content-btn').exists()).toBe(true)
    })

    it('should hide copy button when showCopyButton is false', () => {
      const wrapper = mountComponent({ showCopyButton: false })
      expect(wrapper.find('.copy-content-btn').exists()).toBe(false)
    })

    it('should hide copy button when streaming', () => {
      const wrapper = mountComponent({ showCopyButton: true, isStreaming: true })
      expect(wrapper.find('.copy-content-btn').exists()).toBe(false)
    })

    it('should copy content to clipboard and emit copy event', async () => {
      const wrapper = mountComponent({ content: 'Copy this', showCopyButton: true })
      const copyBtn = wrapper.find('.copy-content-btn')
      await copyBtn.trigger('click')
      expect(mockWriteText).toHaveBeenCalledWith('Copy this')
      expect(wrapper.emitted('copy')).toBeTruthy()
    })
  })

  describe('Entity Click', () => {
    it('should emit entityClick when entity clicked', async () => {
      const wrapper = mountComponent({ analysis: mockAnalysis })
      const entityChip = wrapper.find('.entity-chip')
      await entityChip.trigger('click')
      expect(wrapper.emitted('entityClick')).toBeTruthy()
    })
  })
})
