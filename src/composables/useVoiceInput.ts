import { ref, computed, onUnmounted } from 'vue'

export interface VoiceInputOptions {
  language?: string
  continuous?: boolean
  interimResults?: boolean
  onResult?: (transcript: string, isFinal: boolean) => void
  onError?: (error: string) => void
  onStart?: () => void
  onEnd?: () => void
}

export interface VoiceInputState {
  isListening: boolean
  isSupported: boolean
  transcript: string
  interimTranscript: string
  error: string | null
  confidence: number
}

// Check if Web Speech API is supported
const SpeechRecognition = (window as any).SpeechRecognition || (window as any).webkitSpeechRecognition

export function useVoiceInput(options: VoiceInputOptions = {}) {
  // State
  const isListening = ref(false)
  const isSupported = ref(!!SpeechRecognition)
  const transcript = ref('')
  const interimTranscript = ref('')
  const error = ref<string | null>(null)
  const confidence = ref(0)

  // Recognition instance
  let recognition: any = null

  /**
   * Initialize speech recognition
   */
  function initRecognition() {
    if (!isSupported.value) {
      error.value = 'Speech recognition is not supported in this browser'
      return null
    }

    const rec = new SpeechRecognition()

    // Configuration
    rec.lang = options.language || 'en-US'
    rec.continuous = options.continuous ?? false
    rec.interimResults = options.interimResults ?? true
    rec.maxAlternatives = 1

    // Event handlers
    rec.onstart = () => {
      console.log('Speech recognition started')
      isListening.value = true
      error.value = null
      options.onStart?.()
    }

    rec.onresult = (event: any) => {
      console.log('Speech recognition result event:', event)
      let finalTranscript = ''
      let interim = ''

      for (let i = event.resultIndex; i < event.results.length; i++) {
        const result = event.results[i]
        const transcriptText = result[0].transcript
        console.log('Result:', transcriptText, 'isFinal:', result.isFinal)

        if (result.isFinal) {
          finalTranscript += transcriptText
          confidence.value = result[0].confidence
        } else {
          interim += transcriptText
        }
      }

      if (finalTranscript) {
        transcript.value = finalTranscript
        console.log('Calling onResult with final:', finalTranscript)
        options.onResult?.(finalTranscript, true)
      }

      interimTranscript.value = interim
      if (interim) {
        console.log('Calling onResult with interim:', interim)
        options.onResult?.(interim, false)
      }
    }

    rec.onerror = (event: any) => {
      console.log('Speech recognition error:', event.error)
      const errorMessages: Record<string, string> = {
        'no-speech': 'No speech was detected. Please try again.',
        'aborted': 'Speech recognition was aborted.',
        'audio-capture': 'No microphone was found. Please ensure a microphone is connected.',
        'network': 'Network error occurred. Please check your connection.',
        'not-allowed': 'Microphone permission was denied. Please allow microphone access.',
        'service-not-allowed': 'Speech recognition service is not allowed.',
        'bad-grammar': 'Speech grammar error occurred.',
        'language-not-supported': 'The selected language is not supported.'
      }

      const errorMessage = errorMessages[event.error] || `Speech recognition error: ${event.error}`
      error.value = errorMessage
      options.onError?.(errorMessage)
      isListening.value = false
    }

    rec.onend = () => {
      console.log('Speech recognition ended')
      isListening.value = false
      options.onEnd?.()
    }

    rec.onnomatch = () => {
      error.value = 'No speech was recognized. Please try again.'
    }

    return rec
  }

  /**
   * Start listening for voice input
   */
  function startListening() {
    if (!isSupported.value) {
      error.value = 'Speech recognition is not supported'
      return
    }

    if (isListening.value) {
      return
    }

    // Reset state
    transcript.value = ''
    interimTranscript.value = ''
    error.value = null
    confidence.value = 0

    // Initialize and start
    recognition = initRecognition()
    if (recognition) {
      try {
        recognition.start()
      } catch (e) {
        error.value = 'Failed to start speech recognition'
        console.error('Speech recognition start error:', e)
      }
    }
  }

  /**
   * Stop listening
   */
  function stopListening() {
    if (recognition && isListening.value) {
      recognition.stop()
    }
  }

  /**
   * Toggle listening state
   */
  function toggleListening() {
    if (isListening.value) {
      stopListening()
    } else {
      startListening()
    }
  }

  /**
   * Abort recognition (doesn't trigger onend with results)
   */
  function abortListening() {
    if (recognition && isListening.value) {
      recognition.abort()
    }
  }

  /**
   * Change language
   */
  function setLanguage(lang: string) {
    if (recognition) {
      recognition.lang = lang
    }
  }

  /**
   * Get current transcript (final + interim)
   */
  const fullTranscript = computed(() => {
    return transcript.value + (interimTranscript.value ? ' ' + interimTranscript.value : '')
  })

  /**
   * Clear transcript
   */
  function clearTranscript() {
    transcript.value = ''
    interimTranscript.value = ''
  }

  // Cleanup on unmount
  onUnmounted(() => {
    if (recognition) {
      recognition.abort()
      recognition = null
    }
  })

  // Supported languages for speech recognition
  const supportedLanguages = [
    { code: 'en-US', name: 'English (US)', flag: '🇺🇸' },
    { code: 'en-GB', name: 'English (UK)', flag: '🇬🇧' },
    { code: 'ar-SA', name: 'Arabic', flag: '🇸🇦' },
    { code: 'fr-FR', name: 'French', flag: '🇫🇷' },
    { code: 'es-ES', name: 'Spanish', flag: '🇪🇸' },
    { code: 'de-DE', name: 'German', flag: '🇩🇪' },
    { code: 'it-IT', name: 'Italian', flag: '🇮🇹' },
    { code: 'pt-BR', name: 'Portuguese', flag: '🇧🇷' },
    { code: 'ru-RU', name: 'Russian', flag: '🇷🇺' },
    { code: 'zh-CN', name: 'Chinese (Simplified)', flag: '🇨🇳' },
    { code: 'ja-JP', name: 'Japanese', flag: '🇯🇵' },
    { code: 'ko-KR', name: 'Korean', flag: '🇰🇷' },
    { code: 'hi-IN', name: 'Hindi', flag: '🇮🇳' },
    { code: 'tr-TR', name: 'Turkish', flag: '🇹🇷' },
    { code: 'nl-NL', name: 'Dutch', flag: '🇳🇱' },
    { code: 'pl-PL', name: 'Polish', flag: '🇵🇱' }
  ]

  return {
    // State
    isListening,
    isSupported,
    transcript,
    interimTranscript,
    fullTranscript,
    error,
    confidence,

    // Methods
    startListening,
    stopListening,
    toggleListening,
    abortListening,
    setLanguage,
    clearTranscript,

    // Constants
    supportedLanguages
  }
}
