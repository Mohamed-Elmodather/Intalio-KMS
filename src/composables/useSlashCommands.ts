import { ref, computed } from 'vue'
import type { AIOperationType } from '@/types/ai'

export interface SlashCommand {
  command: string
  aliases: string[]
  label: string
  description: string
  icon: string
  operation: AIOperationType | 'chat' | 'help'
  requiresInput: boolean
  inputPlaceholder?: string
  options?: { value: string; label: string }[]
}

export const SLASH_COMMANDS: SlashCommand[] = [
  {
    command: '/summarize',
    aliases: ['/sum', '/summary'],
    label: 'Summarize',
    description: 'Generate a summary of text or attached content',
    icon: 'fas fa-compress-alt',
    operation: 'summarize',
    requiresInput: true,
    inputPlaceholder: 'Text to summarize...',
    options: [
      { value: 'brief', label: 'Brief' },
      { value: 'detailed', label: 'Detailed' },
      { value: 'bullet', label: 'Bullet Points' }
    ]
  },
  {
    command: '/translate',
    aliases: ['/trans', '/tr'],
    label: 'Translate',
    description: 'Translate text to another language',
    icon: 'fas fa-language',
    operation: 'translate',
    requiresInput: true,
    inputPlaceholder: 'Text to translate...',
    options: [
      { value: 'ar', label: 'Arabic' },
      { value: 'en', label: 'English' },
      { value: 'fr', label: 'French' },
      { value: 'es', label: 'Spanish' },
      { value: 'de', label: 'German' },
      { value: 'zh', label: 'Chinese' },
      { value: 'ja', label: 'Japanese' },
      { value: 'ko', label: 'Korean' }
    ]
  },
  {
    command: '/entities',
    aliases: ['/ner', '/extract'],
    label: 'Extract Entities',
    description: 'Extract people, organizations, locations, dates',
    icon: 'fas fa-tags',
    operation: 'extract-entities',
    requiresInput: true,
    inputPlaceholder: 'Text to analyze...'
  },
  {
    command: '/sentiment',
    aliases: ['/mood', '/tone'],
    label: 'Analyze Sentiment',
    description: 'Detect emotional tone and sentiment',
    icon: 'fas fa-smile',
    operation: 'analyze-sentiment',
    requiresInput: true,
    inputPlaceholder: 'Text to analyze...'
  },
  {
    command: '/classify',
    aliases: ['/categorize', '/cat'],
    label: 'Classify Content',
    description: 'Categorize content with confidence scores',
    icon: 'fas fa-folder-tree',
    operation: 'classify',
    requiresInput: true,
    inputPlaceholder: 'Content to classify...'
  },
  {
    command: '/ocr',
    aliases: ['/scan', '/read'],
    label: 'OCR - Extract Text',
    description: 'Extract text from images or documents',
    icon: 'fas fa-file-image',
    operation: 'ocr',
    requiresInput: false
  },
  {
    command: '/titles',
    aliases: ['/title', '/headline'],
    label: 'Generate Titles',
    description: 'Generate title suggestions for content',
    icon: 'fas fa-heading',
    operation: 'generate-title',
    requiresInput: true,
    inputPlaceholder: 'Content to title...',
    options: [
      { value: 'formal', label: 'Formal' },
      { value: 'casual', label: 'Casual' },
      { value: 'seo', label: 'SEO Optimized' },
      { value: 'creative', label: 'Creative' }
    ]
  },
  {
    command: '/tags',
    aliases: ['/tag', '/autotag'],
    label: 'Auto-Tag',
    description: 'Generate relevant tags for content',
    icon: 'fas fa-hashtag',
    operation: 'auto-tag',
    requiresInput: true,
    inputPlaceholder: 'Content to tag...'
  },
  {
    command: '/search',
    aliases: ['/find', '/lookup'],
    label: 'Smart Search',
    description: 'Search with AI-powered intent detection',
    icon: 'fas fa-search',
    operation: 'smart-search',
    requiresInput: true,
    inputPlaceholder: 'What are you looking for?'
  },
  {
    command: '/compare',
    aliases: ['/diff', '/versus'],
    label: 'Compare Items',
    description: 'Compare multiple items side by side',
    icon: 'fas fa-columns',
    operation: 'chat',
    requiresInput: false
  },
  {
    command: '/help',
    aliases: ['/commands', '/?'],
    label: 'Help',
    description: 'Show available commands',
    icon: 'fas fa-question-circle',
    operation: 'help',
    requiresInput: false
  }
]

export interface ParsedCommand {
  command: SlashCommand | null
  option: string | null
  input: string
  isCommand: boolean
}

export function useSlashCommands() {
  const showCommandSuggestions = ref(false)
  const filteredCommands = ref<SlashCommand[]>([])
  const selectedCommandIndex = ref(0)

  /**
   * Parse input text for slash commands
   */
  function parseInput(input: string): ParsedCommand {
    const trimmed = input.trim()

    // Check if input starts with /
    if (!trimmed.startsWith('/')) {
      return {
        command: null,
        option: null,
        input: trimmed,
        isCommand: false
      }
    }

    // Extract command parts
    const parts = trimmed.split(/\s+/)
    const commandPart = parts[0].toLowerCase()

    // Find matching command
    const command = SLASH_COMMANDS.find(cmd =>
      cmd.command === commandPart || cmd.aliases.includes(commandPart)
    )

    if (!command) {
      return {
        command: null,
        option: null,
        input: trimmed,
        isCommand: true
      }
    }

    // Check for option (e.g., /translate ar)
    let option: string | null = null
    let inputStart = 1

    if (command.options && parts.length > 1) {
      const potentialOption = parts[1].toLowerCase()
      const matchedOption = command.options.find(opt =>
        opt.value.toLowerCase() === potentialOption ||
        opt.label.toLowerCase() === potentialOption
      )
      if (matchedOption) {
        option = matchedOption.value
        inputStart = 2
      }
    }

    // Get the remaining input
    const remainingInput = parts.slice(inputStart).join(' ')

    return {
      command,
      option,
      input: remainingInput,
      isCommand: true
    }
  }

  /**
   * Filter commands based on partial input
   */
  function filterCommands(input: string): SlashCommand[] {
    if (!input.startsWith('/')) {
      return []
    }

    const searchTerm = input.toLowerCase().slice(1) // Remove leading /

    if (!searchTerm) {
      return SLASH_COMMANDS
    }

    return SLASH_COMMANDS.filter(cmd =>
      cmd.command.slice(1).startsWith(searchTerm) ||
      cmd.aliases.some(alias => alias.slice(1).startsWith(searchTerm)) ||
      cmd.label.toLowerCase().includes(searchTerm) ||
      cmd.description.toLowerCase().includes(searchTerm)
    )
  }

  /**
   * Handle input change for command suggestions
   */
  function handleInputChange(input: string) {
    if (input.startsWith('/') && !input.includes(' ')) {
      filteredCommands.value = filterCommands(input)
      showCommandSuggestions.value = filteredCommands.value.length > 0
      selectedCommandIndex.value = 0
    } else {
      showCommandSuggestions.value = false
    }
  }

  /**
   * Navigate command suggestions
   */
  function navigateCommands(direction: 'up' | 'down') {
    if (!showCommandSuggestions.value) return

    if (direction === 'up') {
      selectedCommandIndex.value = Math.max(0, selectedCommandIndex.value - 1)
    } else {
      selectedCommandIndex.value = Math.min(
        filteredCommands.value.length - 1,
        selectedCommandIndex.value + 1
      )
    }
  }

  /**
   * Select a command from suggestions
   */
  function selectCommand(command: SlashCommand): string {
    showCommandSuggestions.value = false
    return command.command + ' '
  }

  /**
   * Get selected command
   */
  function getSelectedCommand(): SlashCommand | null {
    if (!showCommandSuggestions.value || filteredCommands.value.length === 0) {
      return null
    }
    return filteredCommands.value[selectedCommandIndex.value]
  }

  /**
   * Format command for display in help
   */
  function formatCommandHelp(command: SlashCommand): string {
    let help = `**${command.command}**`
    if (command.aliases.length > 0) {
      help += ` (${command.aliases.join(', ')})`
    }
    help += `\n${command.description}`
    if (command.options) {
      help += `\nOptions: ${command.options.map(o => o.label).join(', ')}`
    }
    return help
  }

  /**
   * Get all commands help text
   */
  function getAllCommandsHelp(): string {
    return SLASH_COMMANDS
      .filter(cmd => cmd.operation !== 'help')
      .map(formatCommandHelp)
      .join('\n\n')
  }

  return {
    // State
    showCommandSuggestions,
    filteredCommands,
    selectedCommandIndex,

    // Constants
    SLASH_COMMANDS,

    // Methods
    parseInput,
    filterCommands,
    handleInputChange,
    navigateCommands,
    selectCommand,
    getSelectedCommand,
    formatCommandHelp,
    getAllCommandsHelp
  }
}
