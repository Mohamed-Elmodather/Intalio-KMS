import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest'

describe('useVoiceInput', () => {
  let mockRecognition: {
    lang: string
    continuous: boolean
    interimResults: boolean
    maxAlternatives: number
    start: ReturnType<typeof vi.fn>
    stop: ReturnType<typeof vi.fn>
    abort: ReturnType<typeof vi.fn>
    onstart: (() => void) | null
    onresult: ((event: any) => void) | null
    onerror: ((event: any) => void) | null
    onend: (() => void) | null
    onnomatch: (() => void) | null
  }

  let MockSpeechRecognition: ReturnType<typeof vi.fn>

  beforeEach(() => {
    vi.resetModules()

    mockRecognition = {
      lang: '',
      continuous: false,
      interimResults: false,
      maxAlternatives: 1,
      start: vi.fn(),
      stop: vi.fn(),
      abort: vi.fn(),
      onstart: null,
      onresult: null,
      onerror: null,
      onend: null,
      onnomatch: null,
    }

    MockSpeechRecognition = vi.fn(() => mockRecognition)

    // Mock Web Speech API before importing the module
    ;(window as any).SpeechRecognition = MockSpeechRecognition
    ;(window as any).webkitSpeechRecognition = MockSpeechRecognition
  })

  afterEach(() => {
    vi.clearAllMocks()
    delete (window as any).SpeechRecognition
    delete (window as any).webkitSpeechRecognition
  })

  async function getUseVoiceInput() {
    const module = await import('@/composables/useVoiceInput')
    return module.useVoiceInput
  }

  describe('Initial State', () => {
    it('should have correct initial values', async () => {
      const useVoiceInput = await getUseVoiceInput()
      const { isListening, transcript, interimTranscript, error, confidence } = useVoiceInput()

      expect(isListening.value).toBe(false)
      expect(transcript.value).toBe('')
      expect(interimTranscript.value).toBe('')
      expect(error.value).toBeNull()
      expect(confidence.value).toBe(0)
    })

    it('should detect speech recognition support', async () => {
      const useVoiceInput = await getUseVoiceInput()
      const { isSupported } = useVoiceInput()
      expect(isSupported.value).toBe(true)
    })

    it('should detect when speech recognition is not supported', async () => {
      delete (window as any).SpeechRecognition
      delete (window as any).webkitSpeechRecognition
      vi.resetModules()

      const useVoiceInput = await getUseVoiceInput()
      const { isSupported } = useVoiceInput()
      expect(isSupported.value).toBe(false)
    })
  })

  describe('Supported Languages', () => {
    it('should provide list of supported languages', async () => {
      const useVoiceInput = await getUseVoiceInput()
      const { supportedLanguages } = useVoiceInput()

      expect(Array.isArray(supportedLanguages)).toBe(true)
      expect(supportedLanguages.length).toBeGreaterThan(0)
    })

    it('should include common languages', async () => {
      const useVoiceInput = await getUseVoiceInput()
      const { supportedLanguages } = useVoiceInput()

      const codes = supportedLanguages.map((l) => l.code)
      expect(codes).toContain('en-US')
      expect(codes).toContain('ar-SA')
      expect(codes).toContain('fr-FR')
    })

    it('should have code, name, and flag for each language', async () => {
      const useVoiceInput = await getUseVoiceInput()
      const { supportedLanguages } = useVoiceInput()

      supportedLanguages.forEach((lang) => {
        expect(lang.code).toBeDefined()
        expect(lang.name).toBeDefined()
        expect(lang.flag).toBeDefined()
      })
    })
  })

  describe('startListening', () => {
    it('should start speech recognition', async () => {
      const useVoiceInput = await getUseVoiceInput()
      const { startListening } = useVoiceInput()

      startListening()

      expect(MockSpeechRecognition).toHaveBeenCalled()
      expect(mockRecognition.start).toHaveBeenCalled()
    })

    it('should set default language to en-US', async () => {
      const useVoiceInput = await getUseVoiceInput()
      const { startListening } = useVoiceInput()

      startListening()

      expect(mockRecognition.lang).toBe('en-US')
    })

    it('should use custom language from options', async () => {
      const useVoiceInput = await getUseVoiceInput()
      const { startListening } = useVoiceInput({ language: 'fr-FR' })

      startListening()

      expect(mockRecognition.lang).toBe('fr-FR')
    })

    it('should reset state before starting', async () => {
      const useVoiceInput = await getUseVoiceInput()
      const { startListening, transcript, error, confidence } = useVoiceInput()

      // Set some values
      transcript.value = 'old transcript'
      error.value = 'old error'
      confidence.value = 0.9

      startListening()

      expect(transcript.value).toBe('')
      expect(error.value).toBeNull()
      expect(confidence.value).toBe(0)
    })

    it('should not start if already listening', async () => {
      const useVoiceInput = await getUseVoiceInput()
      const { startListening, isListening } = useVoiceInput()

      startListening()
      mockRecognition.onstart?.()

      expect(isListening.value).toBe(true)

      startListening() // Try to start again

      // Should only have been called once
      expect(mockRecognition.start).toHaveBeenCalledTimes(1)
    })

    it('should set error when not supported', async () => {
      delete (window as any).SpeechRecognition
      delete (window as any).webkitSpeechRecognition
      vi.resetModules()

      const useVoiceInput = await getUseVoiceInput()
      const { startListening, error } = useVoiceInput()

      startListening()

      expect(error.value).toContain('not supported')
    })

    it('should handle start error', async () => {
      mockRecognition.start.mockImplementation(() => {
        throw new Error('Start failed')
      })

      const useVoiceInput = await getUseVoiceInput()
      const { startListening, error } = useVoiceInput()

      startListening()

      expect(error.value).toBe('Failed to start speech recognition')
    })
  })

  describe('stopListening', () => {
    it('should stop speech recognition', async () => {
      const useVoiceInput = await getUseVoiceInput()
      const { startListening, stopListening, isListening } = useVoiceInput()

      startListening()
      mockRecognition.onstart?.()

      expect(isListening.value).toBe(true)

      stopListening()

      expect(mockRecognition.stop).toHaveBeenCalled()
    })

    it('should not call stop if not listening', async () => {
      const useVoiceInput = await getUseVoiceInput()
      const { stopListening } = useVoiceInput()

      stopListening()

      expect(mockRecognition.stop).not.toHaveBeenCalled()
    })
  })

  describe('toggleListening', () => {
    it('should start when not listening', async () => {
      const useVoiceInput = await getUseVoiceInput()
      const { toggleListening } = useVoiceInput()

      toggleListening()

      expect(mockRecognition.start).toHaveBeenCalled()
    })

    it('should stop when listening', async () => {
      const useVoiceInput = await getUseVoiceInput()
      const { toggleListening, startListening } = useVoiceInput()

      startListening()
      mockRecognition.onstart?.()

      toggleListening()

      expect(mockRecognition.stop).toHaveBeenCalled()
    })
  })

  describe('abortListening', () => {
    it('should abort speech recognition', async () => {
      const useVoiceInput = await getUseVoiceInput()
      const { startListening, abortListening } = useVoiceInput()

      startListening()
      mockRecognition.onstart?.()

      abortListening()

      expect(mockRecognition.abort).toHaveBeenCalled()
    })
  })

  describe('clearTranscript', () => {
    it('should clear both transcripts', async () => {
      const useVoiceInput = await getUseVoiceInput()
      const { clearTranscript, transcript, interimTranscript } = useVoiceInput()

      transcript.value = 'some text'
      interimTranscript.value = 'interim text'

      clearTranscript()

      expect(transcript.value).toBe('')
      expect(interimTranscript.value).toBe('')
    })
  })

  describe('setLanguage', () => {
    it('should change recognition language', async () => {
      const useVoiceInput = await getUseVoiceInput()
      const { startListening, setLanguage } = useVoiceInput()

      startListening()
      setLanguage('de-DE')

      expect(mockRecognition.lang).toBe('de-DE')
    })
  })

  describe('fullTranscript computed', () => {
    it('should combine transcript and interim', async () => {
      const useVoiceInput = await getUseVoiceInput()
      const { fullTranscript, transcript, interimTranscript } = useVoiceInput()

      transcript.value = 'Hello'
      interimTranscript.value = 'world'

      expect(fullTranscript.value).toBe('Hello world')
    })

    it('should return only transcript when no interim', async () => {
      const useVoiceInput = await getUseVoiceInput()
      const { fullTranscript, transcript, interimTranscript } = useVoiceInput()

      transcript.value = 'Hello'
      interimTranscript.value = ''

      expect(fullTranscript.value).toBe('Hello')
    })
  })

  describe('Event Handlers', () => {
    it('should handle onstart event', async () => {
      const onStart = vi.fn()
      const useVoiceInput = await getUseVoiceInput()
      const { startListening, isListening, error } = useVoiceInput({ onStart })

      startListening()
      mockRecognition.onstart?.()

      expect(isListening.value).toBe(true)
      expect(error.value).toBeNull()
      expect(onStart).toHaveBeenCalled()
    })

    it('should handle onend event', async () => {
      const onEnd = vi.fn()
      const useVoiceInput = await getUseVoiceInput()
      const { startListening, isListening } = useVoiceInput({ onEnd })

      startListening()
      mockRecognition.onstart?.()
      mockRecognition.onend?.()

      expect(isListening.value).toBe(false)
      expect(onEnd).toHaveBeenCalled()
    })

    it('should handle final result', async () => {
      const onResult = vi.fn()
      const useVoiceInput = await getUseVoiceInput()
      const { startListening, transcript, confidence } = useVoiceInput({ onResult })

      startListening()
      mockRecognition.onstart?.()

      // Simulate final result
      mockRecognition.onresult?.({
        resultIndex: 0,
        results: [
          {
            isFinal: true,
            0: { transcript: 'Hello world', confidence: 0.95 },
          },
        ],
      })

      expect(transcript.value).toBe('Hello world')
      expect(confidence.value).toBe(0.95)
      expect(onResult).toHaveBeenCalledWith('Hello world', true)
    })

    it('should handle interim result', async () => {
      const onResult = vi.fn()
      const useVoiceInput = await getUseVoiceInput()
      const { startListening, interimTranscript } = useVoiceInput({ onResult })

      startListening()
      mockRecognition.onstart?.()

      // Simulate interim result
      mockRecognition.onresult?.({
        resultIndex: 0,
        results: [
          {
            isFinal: false,
            0: { transcript: 'Hell', confidence: 0.8 },
          },
        ],
      })

      expect(interimTranscript.value).toBe('Hell')
      expect(onResult).toHaveBeenCalledWith('Hell', false)
    })

    it('should handle error event', async () => {
      const onError = vi.fn()
      const useVoiceInput = await getUseVoiceInput()
      const { startListening, error, isListening } = useVoiceInput({ onError })

      startListening()
      mockRecognition.onstart?.()

      mockRecognition.onerror?.({ error: 'no-speech' })

      expect(error.value).toBe('No speech was detected. Please try again.')
      expect(isListening.value).toBe(false)
      expect(onError).toHaveBeenCalled()
    })

    it('should handle audio-capture error', async () => {
      const useVoiceInput = await getUseVoiceInput()
      const { startListening, error } = useVoiceInput()

      startListening()
      mockRecognition.onstart?.()
      mockRecognition.onerror?.({ error: 'audio-capture' })

      expect(error.value).toBe('No microphone was found. Please ensure a microphone is connected.')
    })

    it('should handle not-allowed error', async () => {
      const useVoiceInput = await getUseVoiceInput()
      const { startListening, error } = useVoiceInput()

      startListening()
      mockRecognition.onstart?.()
      mockRecognition.onerror?.({ error: 'not-allowed' })

      expect(error.value).toBe('Microphone permission was denied. Please allow microphone access.')
    })

    it('should handle onnomatch event', async () => {
      const useVoiceInput = await getUseVoiceInput()
      const { startListening, error } = useVoiceInput()

      startListening()
      mockRecognition.onnomatch?.()

      expect(error.value).toBe('No speech was recognized. Please try again.')
    })
  })

  describe('Configuration Options', () => {
    it('should set continuous mode', async () => {
      const useVoiceInput = await getUseVoiceInput()
      const { startListening } = useVoiceInput({ continuous: true })

      startListening()

      expect(mockRecognition.continuous).toBe(true)
    })

    it('should set interim results', async () => {
      const useVoiceInput = await getUseVoiceInput()
      const { startListening } = useVoiceInput({ interimResults: false })

      startListening()

      expect(mockRecognition.interimResults).toBe(false)
    })

    it('should use default interimResults true', async () => {
      const useVoiceInput = await getUseVoiceInput()
      const { startListening } = useVoiceInput()

      startListening()

      expect(mockRecognition.interimResults).toBe(true)
    })
  })
})
