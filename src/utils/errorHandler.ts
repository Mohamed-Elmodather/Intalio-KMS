/**
 * Global Error Handler Utility
 * Provides centralized error handling, logging, and user notification
 */
import type { App, ComponentPublicInstance } from 'vue'

// Error severity levels
export type ErrorSeverity = 'low' | 'medium' | 'high' | 'critical'

// Error categories
export type ErrorCategory = 'network' | 'auth' | 'validation' | 'runtime' | 'unknown'

// Error context interface
export interface ErrorContext {
  component?: string
  action?: string
  userId?: string
  url?: string
  timestamp?: string
  additionalInfo?: Record<string, unknown>
}

// Structured error interface
export interface AppError {
  message: string
  originalError?: Error
  severity: ErrorSeverity
  category: ErrorCategory
  context: ErrorContext
  stack?: string
}

// Error log entry for storage/reporting
interface ErrorLogEntry {
  id: string
  error: AppError
  timestamp: string
  reported: boolean
}

// In-memory error log (could be extended to persist to localStorage or send to server)
const errorLog: ErrorLogEntry[] = []
const MAX_ERROR_LOG_SIZE = 100

/**
 * Generate unique error ID
 */
function generateErrorId(): string {
  return `err_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`
}

/**
 * Determine error category from error type
 */
function categorizeError(error: Error): ErrorCategory {
  const message = error.message.toLowerCase()
  const name = error.name.toLowerCase()

  if (message.includes('network') || message.includes('fetch') || name.includes('networkerror')) {
    return 'network'
  }
  if (message.includes('401') || message.includes('403') || message.includes('unauthorized') || message.includes('forbidden')) {
    return 'auth'
  }
  if (message.includes('validation') || message.includes('invalid') || name.includes('validationerror')) {
    return 'validation'
  }
  if (name.includes('typeerror') || name.includes('referenceerror') || name.includes('syntaxerror')) {
    return 'runtime'
  }
  return 'unknown'
}

/**
 * Determine error severity based on category and context
 */
function determineSeverity(category: ErrorCategory, error: Error): ErrorSeverity {
  // Critical errors that need immediate attention
  if (category === 'auth' && error.message.includes('token')) {
    return 'critical'
  }
  if (category === 'runtime') {
    return 'high'
  }
  if (category === 'network') {
    return 'medium'
  }
  if (category === 'validation') {
    return 'low'
  }
  return 'medium'
}

/**
 * Create a structured AppError from a raw error
 */
export function createAppError(
  error: Error | string,
  context: Partial<ErrorContext> = {}
): AppError {
  const errorObj = typeof error === 'string' ? new Error(error) : error
  const category = categorizeError(errorObj)
  const severity = determineSeverity(category, errorObj)

  return {
    message: errorObj.message,
    originalError: errorObj,
    severity,
    category,
    context: {
      timestamp: new Date().toISOString(),
      url: typeof window !== 'undefined' ? window.location.href : undefined,
      ...context,
    },
    stack: errorObj.stack,
  }
}

/**
 * Log error to internal store
 */
function logError(error: AppError): string {
  const entry: ErrorLogEntry = {
    id: generateErrorId(),
    error,
    timestamp: new Date().toISOString(),
    reported: false,
  }

  // Add to log, maintaining max size
  errorLog.unshift(entry)
  if (errorLog.length > MAX_ERROR_LOG_SIZE) {
    errorLog.pop()
  }

  // Log to console in development
  if (import.meta.env.DEV) {
    console.group(`[Error ${entry.id}] ${error.category.toUpperCase()}`)
    console.error('Message:', error.message)
    console.log('Severity:', error.severity)
    console.log('Context:', error.context)
    if (error.stack) {
      console.log('Stack:', error.stack)
    }
    console.groupEnd()
  }

  return entry.id
}

/**
 * Main error handler function
 */
export function handleError(
  error: Error | string,
  context: Partial<ErrorContext> = {},
  options: { silent?: boolean; rethrow?: boolean } = {}
): AppError {
  const appError = createAppError(error, context)
  const errorId = logError(appError)

  // Store error ID in error object for reference
  ;(appError as AppError & { id: string }).id = errorId

  // Optionally rethrow for error boundaries to catch
  if (options.rethrow) {
    throw appError.originalError || new Error(appError.message)
  }

  return appError
}

/**
 * Get user-friendly error message
 */
export function getUserMessage(error: AppError): string {
  switch (error.category) {
    case 'network':
      return 'Unable to connect to the server. Please check your internet connection and try again.'
    case 'auth':
      return 'Your session has expired. Please log in again.'
    case 'validation':
      return error.message || 'Please check your input and try again.'
    case 'runtime':
      return 'An unexpected error occurred. Please refresh the page and try again.'
    default:
      return 'Something went wrong. Please try again later.'
  }
}

/**
 * Get all logged errors (for debugging/admin purposes)
 */
export function getErrorLog(): readonly ErrorLogEntry[] {
  return Object.freeze([...errorLog])
}

/**
 * Clear error log
 */
export function clearErrorLog(): void {
  errorLog.length = 0
}

/**
 * Vue error handler for app.config.errorHandler
 */
export function createVueErrorHandler() {
  return (err: unknown, instance: ComponentPublicInstance | null, info: string): void => {
    const error = err instanceof Error ? err : new Error(String(err))
    const componentName = instance?.$options?.name || instance?.$options?.__name || 'Unknown'

    handleError(error, {
      component: componentName,
      action: info,
      additionalInfo: {
        vueInfo: info,
      },
    })
  }
}

/**
 * Vue warning handler for app.config.warnHandler (development only)
 */
export function createVueWarnHandler() {
  return (msg: string, instance: ComponentPublicInstance | null, trace: string): void => {
    if (import.meta.env.DEV) {
      const componentName = instance?.$options?.name || instance?.$options?.__name || 'Unknown'
      console.warn(`[Vue Warning] ${msg}`, {
        component: componentName,
        trace,
      })
    }
  }
}

/**
 * Window error event handler for uncaught errors
 */
export function setupGlobalErrorHandlers(): void {
  if (typeof window === 'undefined') return

  // Handle uncaught errors
  window.addEventListener('error', (event) => {
    handleError(event.error || new Error(event.message), {
      action: 'uncaught_error',
      additionalInfo: {
        filename: event.filename,
        lineno: event.lineno,
        colno: event.colno,
      },
    })
  })

  // Handle unhandled promise rejections
  window.addEventListener('unhandledrejection', (event) => {
    const error = event.reason instanceof Error ? event.reason : new Error(String(event.reason))
    handleError(error, {
      action: 'unhandled_rejection',
    })
  })
}

/**
 * Install error handling on Vue app
 */
export function installErrorHandler(app: App): void {
  app.config.errorHandler = createVueErrorHandler()

  if (import.meta.env.DEV) {
    app.config.warnHandler = createVueWarnHandler()
  }

  setupGlobalErrorHandlers()
}

export default {
  handleError,
  createAppError,
  getUserMessage,
  getErrorLog,
  clearErrorLog,
  installErrorHandler,
}
