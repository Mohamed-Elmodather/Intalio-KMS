/**
 * HTML Sanitization Utility
 * Prevents XSS attacks by sanitizing user-provided HTML content
 */
import DOMPurify from 'dompurify'

// Configure DOMPurify with safe defaults
const defaultConfig: DOMPurify.Config = {
  ALLOWED_TAGS: [
    'p', 'br', 'b', 'i', 'em', 'strong', 'u', 's', 'strike',
    'h1', 'h2', 'h3', 'h4', 'h5', 'h6',
    'ul', 'ol', 'li',
    'a', 'img',
    'blockquote', 'pre', 'code',
    'table', 'thead', 'tbody', 'tr', 'th', 'td',
    'div', 'span', 'mark',
    'hr', 'sub', 'sup',
  ],
  ALLOWED_ATTR: [
    'href', 'target', 'rel', 'src', 'alt', 'title', 'class', 'id',
    'width', 'height', 'style',
  ],
  ALLOW_DATA_ATTR: false,
  ADD_ATTR: ['target'],
  // Force all links to open in new tab with security attributes
  ADD_TAGS: [],
  FORBID_TAGS: ['script', 'style', 'iframe', 'form', 'input', 'textarea', 'button'],
  FORBID_ATTR: ['onerror', 'onload', 'onclick', 'onmouseover', 'onfocus', 'onblur'],
}

// Hook to add rel="noopener noreferrer" to all links
DOMPurify.addHook('afterSanitizeAttributes', (node) => {
  if (node.tagName === 'A') {
    node.setAttribute('target', '_blank')
    node.setAttribute('rel', 'noopener noreferrer')
  }
})

/**
 * Sanitize HTML content to prevent XSS attacks
 * @param dirty - Untrusted HTML string
 * @param config - Optional DOMPurify configuration override
 * @returns Sanitized HTML string safe for v-html
 */
export function sanitizeHtml(dirty: string | null | undefined, config?: DOMPurify.Config): string {
  if (!dirty) return ''
  return DOMPurify.sanitize(dirty, config || defaultConfig) as string
}

/**
 * Sanitize HTML with minimal tags (for simple text with formatting)
 * Only allows basic text formatting tags
 */
export function sanitizeBasicHtml(dirty: string | null | undefined): string {
  if (!dirty) return ''
  return DOMPurify.sanitize(dirty, {
    ALLOWED_TAGS: ['b', 'i', 'em', 'strong', 'u', 'br', 'p', 'span', 'mark'],
    ALLOWED_ATTR: ['class'],
  }) as string
}

/**
 * Escape HTML entities (for displaying code or when no HTML is needed)
 * @param text - Text to escape
 * @returns Escaped text safe for display
 */
export function escapeHtml(text: string | null | undefined): string {
  if (!text) return ''
  const map: Record<string, string> = {
    '&': '&amp;',
    '<': '&lt;',
    '>': '&gt;',
    '"': '&quot;',
    "'": '&#39;',
  }
  return text.replace(/[&<>"']/g, (char) => map[char])
}

/**
 * Highlight search terms in text safely (prevents XSS)
 * @param text - Text to search in
 * @param searchTerms - Terms to highlight
 * @returns HTML with highlighted terms, safe for v-html
 */
export function highlightSearchTerms(text: string | null | undefined, searchTerms: string): string {
  if (!text) return ''
  if (!searchTerms.trim()) return escapeHtml(text)

  // First escape the text to prevent XSS
  const escapedText = escapeHtml(text)

  // Escape special regex characters in search terms
  const escapedTerms = searchTerms
    .trim()
    .split(/\s+/)
    .map((term) => term.replace(/[.*+?^${}()|[\]\\]/g, '\\$&'))
    .join('|')

  if (!escapedTerms) return escapedText

  // Create regex and replace with highlighted version
  const regex = new RegExp(`(${escapedTerms})`, 'gi')
  return escapedText.replace(regex, '<mark class="bg-yellow-200 px-0.5 rounded">$1</mark>')
}

/**
 * Strip all HTML tags from a string
 * @param html - HTML string to strip
 * @returns Plain text without any HTML
 */
export function stripHtml(html: string | null | undefined): string {
  if (!html) return ''
  return DOMPurify.sanitize(html, { ALLOWED_TAGS: [] }) as string
}

export default {
  sanitizeHtml,
  sanitizeBasicHtml,
  escapeHtml,
  highlightSearchTerms,
  stripHtml,
}
