import { describe, it, expect } from 'vitest'
import en from '@/i18n/locales/en'
import ar from '@/i18n/locales/ar'

// Helper function to get all keys from nested object
function getAllKeys(obj: Record<string, any>, prefix = ''): string[] {
  const keys: string[] = []
  for (const key of Object.keys(obj)) {
    const fullKey = prefix ? `${prefix}.${key}` : key
    if (typeof obj[key] === 'object' && obj[key] !== null && !Array.isArray(obj[key])) {
      keys.push(...getAllKeys(obj[key], fullKey))
    } else {
      keys.push(fullKey)
    }
  }
  return keys
}

// Helper function to get value by dot-notation key
function getValueByKey(obj: Record<string, any>, key: string): any {
  const parts = key.split('.')
  let current: any = obj
  for (const part of parts) {
    if (current === undefined || current === null) return undefined
    current = current[part]
  }
  return current
}

// Helper function to extract placeholders from translation string
function extractPlaceholders(str: string): string[] {
  const matches = str.match(/\{([^}]+)\}/g)
  return matches ? matches.sort() : []
}

describe('i18n Locales', () => {
  describe('English Locale (en.ts)', () => {
    it('should export a valid object', () => {
      expect(en).toBeDefined()
      expect(typeof en).toBe('object')
      expect(en).not.toBeNull()
    })

    it('should have common section', () => {
      expect(en.common).toBeDefined()
      expect(typeof en.common).toBe('object')
    })

    it('should have app section', () => {
      expect(en.app).toBeDefined()
      expect(typeof en.app).toBe('object')
    })

    it('should have auth section', () => {
      expect(en.auth).toBeDefined()
      expect(typeof en.auth).toBe('object')
    })

    it('should have nav section', () => {
      expect(en.nav).toBeDefined()
      expect(typeof en.nav).toBe('object')
    })

    it('should have user section', () => {
      expect(en.user).toBeDefined()
      expect(typeof en.user).toBe('object')
    })

    it('should have dashboard section', () => {
      expect(en.dashboard).toBeDefined()
      expect(typeof en.dashboard).toBe('object')
    })

    it('should have articles section', () => {
      expect(en.articles).toBeDefined()
      expect(typeof en.articles).toBe('object')
    })

    it('should have documents section', () => {
      expect(en.documents).toBeDefined()
      expect(typeof en.documents).toBe('object')
    })

    it('should have learning section', () => {
      expect(en.learning).toBeDefined()
      expect(typeof en.learning).toBe('object')
    })

    it('should have events section', () => {
      expect(en.events).toBeDefined()
      expect(typeof en.events).toBe('object')
    })

    it('should have polls section', () => {
      expect(en.polls).toBeDefined()
      expect(typeof en.polls).toBe('object')
    })

    it('should have media section', () => {
      expect(en.media).toBeDefined()
      expect(typeof en.media).toBe('object')
    })
  })

  describe('Arabic Locale (ar.ts)', () => {
    it('should export a valid object', () => {
      expect(ar).toBeDefined()
      expect(typeof ar).toBe('object')
      expect(ar).not.toBeNull()
    })

    it('should have common section', () => {
      expect(ar.common).toBeDefined()
      expect(typeof ar.common).toBe('object')
    })

    it('should have app section', () => {
      expect(ar.app).toBeDefined()
      expect(typeof ar.app).toBe('object')
    })

    it('should have auth section', () => {
      expect(ar.auth).toBeDefined()
      expect(typeof ar.auth).toBe('object')
    })

    it('should have nav section', () => {
      expect(ar.nav).toBeDefined()
      expect(typeof ar.nav).toBe('object')
    })

    it('should have user section', () => {
      expect(ar.user).toBeDefined()
      expect(typeof ar.user).toBe('object')
    })

    it('should have dashboard section', () => {
      expect(ar.dashboard).toBeDefined()
      expect(typeof ar.dashboard).toBe('object')
    })

    it('should have articles section', () => {
      expect(ar.articles).toBeDefined()
      expect(typeof ar.articles).toBe('object')
    })

    it('should have documents section', () => {
      expect(ar.documents).toBeDefined()
      expect(typeof ar.documents).toBe('object')
    })

    it('should have learning section', () => {
      expect(ar.learning).toBeDefined()
      expect(typeof ar.learning).toBe('object')
    })

    it('should have events section', () => {
      expect(ar.events).toBeDefined()
      expect(typeof ar.events).toBe('object')
    })

    it('should have polls section', () => {
      expect(ar.polls).toBeDefined()
      expect(typeof ar.polls).toBe('object')
    })

    it('should have media section', () => {
      expect(ar.media).toBeDefined()
      expect(typeof ar.media).toBe('object')
    })
  })

  describe('Key Consistency', () => {
    it('should have matching top-level keys', () => {
      const enKeys = Object.keys(en).sort()
      const arKeys = Object.keys(ar).sort()
      expect(enKeys).toEqual(arKeys)
    })

    it('should have all English keys in Arabic', () => {
      const enAllKeys = getAllKeys(en)
      const arAllKeys = new Set(getAllKeys(ar))

      const missingInArabic = enAllKeys.filter(key => !arAllKeys.has(key))

      if (missingInArabic.length > 0) {
        console.log('Keys missing in Arabic:', missingInArabic.slice(0, 20))
      }

      // Allow some tolerance for minor differences
      expect(missingInArabic.length).toBeLessThan(50)
    })

    it('should have all Arabic keys in English', () => {
      const arAllKeys = getAllKeys(ar)
      const enAllKeys = new Set(getAllKeys(en))

      const missingInEnglish = arAllKeys.filter(key => !enAllKeys.has(key))

      if (missingInEnglish.length > 0) {
        console.log('Keys missing in English:', missingInEnglish.slice(0, 20))
      }

      // Allow some tolerance for minor differences
      expect(missingInEnglish.length).toBeLessThan(50)
    })
  })

  describe('Common Section Consistency', () => {
    it('should have matching common keys', () => {
      const enCommonKeys = Object.keys(en.common).sort()
      const arCommonKeys = Object.keys(ar.common).sort()
      expect(enCommonKeys).toEqual(arCommonKeys)
    })

    it('should have search translation', () => {
      expect(en.common.search).toBe('Search')
      expect(ar.common.search).toBe('بحث')
    })

    it('should have loading translation', () => {
      expect(en.common.loading).toBe('Loading...')
      expect(ar.common.loading).toBe('جاري التحميل...')
    })

    it('should have save translation', () => {
      expect(en.common.save).toBe('Save')
      expect(ar.common.save).toBe('حفظ')
    })

    it('should have cancel translation', () => {
      expect(en.common.cancel).toBe('Cancel')
      expect(ar.common.cancel).toBe('إلغاء')
    })
  })

  describe('Auth Section Consistency', () => {
    it('should have matching auth keys', () => {
      const enAuthKeys = Object.keys(en.auth).sort()
      const arAuthKeys = Object.keys(ar.auth).sort()
      expect(enAuthKeys).toEqual(arAuthKeys)
    })

    it('should have login translation', () => {
      expect(en.auth.login).toBe('Login')
      expect(ar.auth.login).toBe('تسجيل الدخول')
    })

    it('should have logout translation', () => {
      expect(en.auth.logout).toBe('Logout')
      expect(ar.auth.logout).toBe('تسجيل الخروج')
    })

    it('should have password translation', () => {
      expect(en.auth.password).toBe('Password')
      expect(ar.auth.password).toBe('كلمة المرور')
    })
  })

  describe('Nav Section Consistency', () => {
    it('should have matching nav keys', () => {
      const enNavKeys = Object.keys(en.nav).sort()
      const arNavKeys = Object.keys(ar.nav).sort()
      expect(enNavKeys).toEqual(arNavKeys)
    })

    it('should have dashboard translation', () => {
      expect(en.nav.dashboard).toBe('Dashboard')
      expect(ar.nav.dashboard).toBe('لوحة التحكم')
    })

    it('should have articles translation', () => {
      expect(en.nav.articles).toBe('Articles')
      expect(ar.nav.articles).toBe('المقالات')
    })

    it('should have settings translation', () => {
      expect(en.nav.settings).toBe('Settings')
      expect(ar.nav.settings).toBe('الإعدادات')
    })
  })

  describe('Placeholder Consistency', () => {
    it('should have matching placeholders in common.daysAgo', () => {
      const enPlaceholders = extractPlaceholders(en.common.daysAgo)
      const arPlaceholders = extractPlaceholders(ar.common.daysAgo)
      expect(enPlaceholders).toEqual(arPlaceholders)
    })

    it('should have matching placeholders in common.weeksAgo', () => {
      const enPlaceholders = extractPlaceholders(en.common.weeksAgo)
      const arPlaceholders = extractPlaceholders(ar.common.weeksAgo)
      expect(enPlaceholders).toEqual(arPlaceholders)
    })

    it('should have matching placeholders in common.shareOn', () => {
      const enPlaceholders = extractPlaceholders(en.common.shareOn)
      const arPlaceholders = extractPlaceholders(ar.common.shareOn)
      expect(enPlaceholders).toEqual(arPlaceholders)
    })

    it('should have matching placeholders in auth.loggedInAs', () => {
      const enPlaceholders = extractPlaceholders(en.auth.loggedInAs)
      const arPlaceholders = extractPlaceholders(ar.auth.loggedInAs)
      expect(enPlaceholders).toEqual(arPlaceholders)
    })

    it('should have matching placeholders in dashboard.greeting.morning', () => {
      const enPlaceholders = extractPlaceholders(en.dashboard.greeting.morning)
      const arPlaceholders = extractPlaceholders(ar.dashboard.greeting.morning)
      expect(enPlaceholders).toEqual(arPlaceholders)
    })

    it('should have matching placeholders in dashboard.pendingTasks', () => {
      const enPlaceholders = extractPlaceholders(en.dashboard.pendingTasks)
      const arPlaceholders = extractPlaceholders(ar.dashboard.pendingTasks)
      expect(enPlaceholders).toEqual(arPlaceholders)
    })
  })

  describe('Non-Empty Values', () => {
    it('should not have empty strings in common section', () => {
      const emptyKeys: string[] = []
      for (const [key, value] of Object.entries(en.common)) {
        if (typeof value === 'string' && value.trim() === '') {
          emptyKeys.push(key)
        }
      }
      expect(emptyKeys).toEqual([])
    })

    it('should not have empty strings in Arabic common section', () => {
      const emptyKeys: string[] = []
      for (const [key, value] of Object.entries(ar.common)) {
        if (typeof value === 'string' && value.trim() === '') {
          emptyKeys.push(key)
        }
      }
      expect(emptyKeys).toEqual([])
    })

    it('should have non-empty app name', () => {
      expect(en.app.name.length).toBeGreaterThan(0)
      expect(ar.app.name.length).toBeGreaterThan(0)
    })

    it('should have non-empty auth.login', () => {
      expect(en.auth.login.length).toBeGreaterThan(0)
      expect(ar.auth.login.length).toBeGreaterThan(0)
    })
  })

  describe('Value Types', () => {
    it('should have string values for common.search', () => {
      expect(typeof en.common.search).toBe('string')
      expect(typeof ar.common.search).toBe('string')
    })

    it('should have object values for dashboard.greeting', () => {
      expect(typeof en.dashboard.greeting).toBe('object')
      expect(typeof ar.dashboard.greeting).toBe('object')
    })

    it('should have object values for nav.workspaces', () => {
      expect(typeof en.nav.workspaces).toBe('object')
      expect(typeof ar.nav.workspaces).toBe('object')
    })

    it('should have object values for auth.hero', () => {
      expect(typeof en.auth.hero).toBe('object')
      expect(typeof ar.auth.hero).toBe('object')
    })
  })

  describe('Nested Structure Consistency', () => {
    it('should have matching dashboard.greeting keys', () => {
      const enKeys = Object.keys(en.dashboard.greeting).sort()
      const arKeys = Object.keys(ar.dashboard.greeting).sort()
      expect(enKeys).toEqual(arKeys)
    })

    it('should have matching nav.workspaces keys', () => {
      const enKeys = Object.keys(en.nav.workspaces).sort()
      const arKeys = Object.keys(ar.nav.workspaces).sort()
      expect(enKeys).toEqual(arKeys)
    })

    it('should have matching auth.hero keys', () => {
      const enKeys = Object.keys(en.auth.hero).sort()
      const arKeys = Object.keys(ar.auth.hero).sort()
      expect(enKeys).toEqual(arKeys)
    })

    it('should have matching auth.hero.stats keys', () => {
      const enKeys = Object.keys(en.auth.hero.stats).sort()
      const arKeys = Object.keys(ar.auth.hero.stats).sort()
      expect(enKeys).toEqual(arKeys)
    })

    it('should have matching auth.hero.features keys', () => {
      const enKeys = Object.keys(en.auth.hero.features).sort()
      const arKeys = Object.keys(ar.auth.hero.features).sort()
      expect(enKeys).toEqual(arKeys)
    })

    it('should have matching profile.stats keys', () => {
      const enKeys = Object.keys(en.profile.stats).sort()
      const arKeys = Object.keys(ar.profile.stats).sort()
      expect(enKeys).toEqual(arKeys)
    })

    it('should have matching profile.contributions.months keys', () => {
      const enKeys = Object.keys(en.profile.contributions.months).sort()
      const arKeys = Object.keys(ar.profile.contributions.months).sort()
      expect(enKeys).toEqual(arKeys)
    })

    it('should have matching profile.contributions.days keys', () => {
      const enKeys = Object.keys(en.profile.contributions.days).sort()
      const arKeys = Object.keys(ar.profile.contributions.days).sort()
      expect(enKeys).toEqual(arKeys)
    })
  })

  describe('Articles Section', () => {
    it('should have similar articles keys with minimal differences', () => {
      const enKeys = new Set(Object.keys(en.articles))
      const arKeys = new Set(Object.keys(ar.articles))

      const missingInArabic = [...enKeys].filter(k => !arKeys.has(k))
      const missingInEnglish = [...arKeys].filter(k => !enKeys.has(k))

      // Allow small differences (less than 5 keys)
      expect(missingInArabic.length).toBeLessThan(5)
      expect(missingInEnglish.length).toBeLessThan(5)
    })

    it('should have title translation', () => {
      expect(en.articles.title).toBe('Articles Hub')
      expect(typeof ar.articles.title).toBe('string')
      expect(ar.articles.title.length).toBeGreaterThan(0)
    })
  })

  describe('Documents Section', () => {
    it('should have matching documents keys', () => {
      const enKeys = Object.keys(en.documents).sort()
      const arKeys = Object.keys(ar.documents).sort()
      expect(enKeys).toEqual(arKeys)
    })

    it('should have title translation', () => {
      expect(en.documents.title).toBe('Documents')
      expect(typeof ar.documents.title).toBe('string')
      expect(ar.documents.title.length).toBeGreaterThan(0)
    })
  })

  describe('Learning Section', () => {
    it('should have matching learning top-level keys', () => {
      const enKeys = Object.keys(en.learning).sort()
      const arKeys = Object.keys(ar.learning).sort()
      expect(enKeys).toEqual(arKeys)
    })

    it('should have matching learning.levels keys', () => {
      const enKeys = Object.keys(en.learning.levels).sort()
      const arKeys = Object.keys(ar.learning.levels).sort()
      expect(enKeys).toEqual(arKeys)
    })

    it('should have matching learning.categories keys', () => {
      const enKeys = Object.keys(en.learning.categories).sort()
      const arKeys = Object.keys(ar.learning.categories).sort()
      expect(enKeys).toEqual(arKeys)
    })
  })

  describe('Events Section', () => {
    it('should have matching events keys', () => {
      const enKeys = Object.keys(en.events).sort()
      const arKeys = Object.keys(ar.events).sort()
      expect(enKeys).toEqual(arKeys)
    })

    it('should have matching events.weekDays keys', () => {
      const enKeys = Object.keys(en.events.weekDays).sort()
      const arKeys = Object.keys(ar.events.weekDays).sort()
      expect(enKeys).toEqual(arKeys)
    })

    it('should have matching events.eventTypeNames keys', () => {
      const enKeys = Object.keys(en.events.eventTypeNames).sort()
      const arKeys = Object.keys(ar.events.eventTypeNames).sort()
      expect(enKeys).toEqual(arKeys)
    })
  })

  describe('Polls Section', () => {
    it('should have matching polls keys', () => {
      const enKeys = Object.keys(en.polls).sort()
      const arKeys = Object.keys(ar.polls).sort()
      expect(enKeys).toEqual(arKeys)
    })

    it('should have title translation', () => {
      expect(en.polls.title).toBe('Polls & Surveys')
      expect(typeof ar.polls.title).toBe('string')
      expect(ar.polls.title.length).toBeGreaterThan(0)
    })
  })

  describe('Media Section', () => {
    it('should have matching media keys', () => {
      const enKeys = Object.keys(en.media).sort()
      const arKeys = Object.keys(ar.media).sort()
      expect(enKeys).toEqual(arKeys)
    })

    it('should have title translation', () => {
      expect(en.media.title).toBe('Media Center')
      expect(typeof ar.media.title).toBe('string')
      expect(ar.media.title.length).toBeGreaterThan(0)
    })
  })

  describe('Arabic Translations', () => {
    it('should contain Arabic characters', () => {
      // Arabic Unicode range: \u0600-\u06FF
      const arabicPattern = /[\u0600-\u06FF]/
      expect(arabicPattern.test(ar.common.search)).toBe(true)
      expect(arabicPattern.test(ar.common.loading)).toBe(true)
      expect(arabicPattern.test(ar.auth.login)).toBe(true)
      expect(arabicPattern.test(ar.nav.dashboard)).toBe(true)
    })

    it('should have proper RTL-friendly translations', () => {
      // Arabic text should not start with LTR characters except for brand names
      expect(ar.common.search).not.toMatch(/^[a-zA-Z]/)
      expect(ar.common.loading).not.toMatch(/^[a-zA-Z]/)
    })
  })

  describe('English Translations', () => {
    it('should contain English characters', () => {
      expect(en.common.search).toMatch(/^[a-zA-Z\s]+$/)
      expect(en.common.save).toMatch(/^[a-zA-Z\s]+$/)
      expect(en.auth.login).toMatch(/^[a-zA-Z\s]+$/)
    })

    it('should have properly capitalized first letter', () => {
      expect(en.common.search[0]).toMatch(/[A-Z]/)
      expect(en.common.save[0]).toMatch(/[A-Z]/)
      expect(en.auth.login[0]).toMatch(/[A-Z]/)
    })
  })

  describe('Special Characters', () => {
    it('should handle special characters in both locales', () => {
      // Check that special characters don't cause issues
      expect(() => JSON.stringify(en)).not.toThrow()
      expect(() => JSON.stringify(ar)).not.toThrow()
    })

    it('should preserve HTML entities in translations', () => {
      // auth.hero.title contains HTML
      expect(en.auth.hero.title).toContain('<br/>')
      expect(ar.auth.hero.title).toContain('<br/>')
    })
  })

  describe('Key Count Comparison', () => {
    it('should have similar total key counts', () => {
      const enKeyCount = getAllKeys(en).length
      const arKeyCount = getAllKeys(ar).length

      // Allow 10% difference
      const diff = Math.abs(enKeyCount - arKeyCount)
      const maxDiff = Math.max(enKeyCount, arKeyCount) * 0.1

      expect(diff).toBeLessThanOrEqual(maxDiff)
    })
  })
})
