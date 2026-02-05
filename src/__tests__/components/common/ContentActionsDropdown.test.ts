import { describe, it, expect, vi, beforeEach } from 'vitest'
import { shallowMount } from '@vue/test-utils'
import ContentActionsDropdown from '@/components/common/ContentActionsDropdown.vue'

// Mock vue-i18n
vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string) => key,
  }),
}))

function mountComponent(props = {}) {
  return shallowMount(ContentActionsDropdown, {
    props,
    global: {
      mocks: {
        $t: (key: string) => key,
      },
      stubs: {
        Transition: true,
      },
    },
  })
}

describe('ContentActionsDropdown', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('should render the component', () => {
      const wrapper = mountComponent()
      expect(wrapper.exists()).toBe(true)
      expect(wrapper.find('.content-actions-dropdown').exists()).toBe(true)
    })

    it('should render trigger button', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.trigger-btn').exists()).toBe(true)
    })

    it('should render ellipsis icon', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.fa-ellipsis-v').exists()).toBe(true)
    })
  })

  describe('Dropdown Toggle', () => {
    it('should not show dropdown by default', () => {
      const wrapper = mountComponent()
      expect(wrapper.find('.dropdown-menu').exists()).toBe(false)
    })

    it('should show dropdown when clicked', async () => {
      const wrapper = mountComponent()
      await wrapper.find('.trigger-btn').trigger('click')
      expect(wrapper.find('.dropdown-menu').exists()).toBe(true)
    })

    it('should hide dropdown when clicked again', async () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.showDropdown = true
      await wrapper.find('.trigger-btn').trigger('click')
      expect(vm.showDropdown).toBe(false)
    })

    it('should apply active class when dropdown is open', async () => {
      const wrapper = mountComponent()
      await wrapper.find('.trigger-btn').trigger('click')
      expect(wrapper.find('.trigger-btn.active').exists()).toBe(true)
    })
  })

  describe('Dropdown Items', () => {
    it('should show add to collection option', async () => {
      const wrapper = mountComponent()
      await wrapper.find('.trigger-btn').trigger('click')
      expect(wrapper.text()).toContain('collections.addToCollection')
    })

    it('should show share option', async () => {
      const wrapper = mountComponent()
      await wrapper.find('.trigger-btn').trigger('click')
      expect(wrapper.text()).toContain('common.share')
    })

    it('should show download option when enabled', async () => {
      const wrapper = mountComponent({ showDownload: true })
      await wrapper.find('.trigger-btn').trigger('click')
      expect(wrapper.text()).toContain('common.download')
    })

    it('should hide download option when disabled', async () => {
      const wrapper = mountComponent({ showDownload: false })
      await wrapper.find('.trigger-btn').trigger('click')
      expect(wrapper.text()).not.toContain('common.download')
    })

    it('should show copy link option when enabled', async () => {
      const wrapper = mountComponent({ showCopyLink: true })
      await wrapper.find('.trigger-btn').trigger('click')
      expect(wrapper.text()).toContain('common.copyLink')
    })

    it('should hide copy link option when disabled', async () => {
      const wrapper = mountComponent({ showCopyLink: false })
      await wrapper.find('.trigger-btn').trigger('click')
      expect(wrapper.text()).not.toContain('common.copyLink')
    })

    it('should show report option when enabled', async () => {
      const wrapper = mountComponent({ showReport: true })
      await wrapper.find('.trigger-btn').trigger('click')
      expect(wrapper.text()).toContain('common.report')
    })

    it('should hide report option when disabled', async () => {
      const wrapper = mountComponent({ showReport: false })
      await wrapper.find('.trigger-btn').trigger('click')
      expect(wrapper.text()).not.toContain('common.report')
    })
  })

  describe('Action Events', () => {
    it('should emit addToCollection event', async () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.handleAction('addToCollection')
      expect(wrapper.emitted('addToCollection')).toBeTruthy()
    })

    it('should emit share event', async () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.handleAction('share')
      expect(wrapper.emitted('share')).toBeTruthy()
    })

    it('should emit download event', async () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.handleAction('download')
      expect(wrapper.emitted('download')).toBeTruthy()
    })

    it('should emit copyLink event', async () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.handleAction('copyLink')
      expect(wrapper.emitted('copyLink')).toBeTruthy()
    })

    it('should emit report event', async () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.handleAction('report')
      expect(wrapper.emitted('report')).toBeTruthy()
    })

    it('should close dropdown after action', async () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.showDropdown = true
      vm.handleAction('share')
      expect(vm.showDropdown).toBe(false)
    })
  })

  describe('Icons', () => {
    it('should show layer-group icon for add to collection', async () => {
      const wrapper = mountComponent()
      await wrapper.find('.trigger-btn').trigger('click')
      expect(wrapper.find('.fa-layer-group').exists()).toBe(true)
    })

    it('should show share-alt icon for share', async () => {
      const wrapper = mountComponent()
      await wrapper.find('.trigger-btn').trigger('click')
      expect(wrapper.find('.fa-share-alt').exists()).toBe(true)
    })

    it('should show download icon for download', async () => {
      const wrapper = mountComponent({ showDownload: true })
      await wrapper.find('.trigger-btn').trigger('click')
      expect(wrapper.find('.fa-download').exists()).toBe(true)
    })

    it('should show link icon for copy link', async () => {
      const wrapper = mountComponent({ showCopyLink: true })
      await wrapper.find('.trigger-btn').trigger('click')
      expect(wrapper.find('.fa-link').exists()).toBe(true)
    })

    it('should show flag icon for report', async () => {
      const wrapper = mountComponent({ showReport: true })
      await wrapper.find('.trigger-btn').trigger('click')
      expect(wrapper.find('.fa-flag').exists()).toBe(true)
    })
  })

  describe('Divider', () => {
    it('should show divider before report option', async () => {
      const wrapper = mountComponent({ showReport: true })
      await wrapper.find('.trigger-btn').trigger('click')
      expect(wrapper.find('.dropdown-divider').exists()).toBe(true)
    })

    it('should not show divider when report is hidden', async () => {
      const wrapper = mountComponent({ showReport: false })
      await wrapper.find('.trigger-btn').trigger('click')
      expect(wrapper.find('.dropdown-divider').exists()).toBe(false)
    })
  })

  describe('Danger Styling', () => {
    it('should apply danger class to report option', async () => {
      const wrapper = mountComponent({ showReport: true })
      await wrapper.find('.trigger-btn').trigger('click')
      expect(wrapper.find('.dropdown-item.danger').exists()).toBe(true)
    })
  })

  describe('Click Outside', () => {
    it('should have handleClickOutside function', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(typeof vm.handleClickOutside).toBe('function')
    })

    it('should close dropdown when clicking outside', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      vm.showDropdown = true
      vm.handleClickOutside({ target: document.body })
      expect(vm.showDropdown).toBe(false)
    })
  })

  describe('Default Props', () => {
    it('should have showDownload true by default', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.showDownload).toBe(true)
    })

    it('should have showCopyLink true by default', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.showCopyLink).toBe(true)
    })

    it('should have showReport true by default', () => {
      const wrapper = mountComponent()
      const vm = wrapper.vm as any
      expect(vm.showReport).toBe(true)
    })
  })
})
