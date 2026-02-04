import { describe, it, expect, beforeEach } from 'vitest'
import { useSlashCommands, SLASH_COMMANDS } from '@/composables/useSlashCommands'

describe('useSlashCommands', () => {
  describe('SLASH_COMMANDS constant', () => {
    it('should have defined commands', () => {
      expect(SLASH_COMMANDS.length).toBeGreaterThan(0)
    })

    it('should have required properties for each command', () => {
      SLASH_COMMANDS.forEach((cmd) => {
        expect(cmd.command).toBeDefined()
        expect(cmd.command.startsWith('/')).toBe(true)
        expect(cmd.aliases).toBeDefined()
        expect(Array.isArray(cmd.aliases)).toBe(true)
        expect(cmd.label).toBeDefined()
        expect(cmd.description).toBeDefined()
        expect(cmd.icon).toBeDefined()
        expect(cmd.operation).toBeDefined()
        expect(typeof cmd.requiresInput).toBe('boolean')
      })
    })

    it('should include summarize command', () => {
      const summarize = SLASH_COMMANDS.find((c) => c.command === '/summarize')
      expect(summarize).toBeDefined()
      expect(summarize?.aliases).toContain('/sum')
      expect(summarize?.aliases).toContain('/summary')
      expect(summarize?.options).toBeDefined()
    })

    it('should include translate command', () => {
      const translate = SLASH_COMMANDS.find((c) => c.command === '/translate')
      expect(translate).toBeDefined()
      expect(translate?.options).toBeDefined()
      expect(translate?.options?.some((o) => o.value === 'ar')).toBe(true)
    })

    it('should include help command', () => {
      const help = SLASH_COMMANDS.find((c) => c.command === '/help')
      expect(help).toBeDefined()
      expect(help?.operation).toBe('help')
      expect(help?.requiresInput).toBe(false)
    })
  })

  describe('parseInput', () => {
    it('should return isCommand false for non-command input', () => {
      const { parseInput } = useSlashCommands()

      const result = parseInput('Hello world')

      expect(result.isCommand).toBe(false)
      expect(result.command).toBeNull()
      expect(result.input).toBe('Hello world')
    })

    it('should parse valid command', () => {
      const { parseInput } = useSlashCommands()

      const result = parseInput('/summarize')

      expect(result.isCommand).toBe(true)
      expect(result.command).toBeDefined()
      expect(result.command?.command).toBe('/summarize')
    })

    it('should parse command with input', () => {
      const { parseInput } = useSlashCommands()

      const result = parseInput('/summarize This is some text to summarize')

      expect(result.isCommand).toBe(true)
      expect(result.command?.command).toBe('/summarize')
      expect(result.input).toBe('This is some text to summarize')
    })

    it('should parse command with option', () => {
      const { parseInput } = useSlashCommands()

      const result = parseInput('/summarize brief This is the text')

      expect(result.isCommand).toBe(true)
      expect(result.option).toBe('brief')
      expect(result.input).toBe('This is the text')
    })

    it('should parse translate command with language option', () => {
      const { parseInput } = useSlashCommands()

      const result = parseInput('/translate ar Hello world')

      expect(result.command?.command).toBe('/translate')
      expect(result.option).toBe('ar')
      expect(result.input).toBe('Hello world')
    })

    it('should handle command alias', () => {
      const { parseInput } = useSlashCommands()

      const result = parseInput('/sum Some text')

      expect(result.command?.command).toBe('/summarize')
    })

    it('should handle unknown command', () => {
      const { parseInput } = useSlashCommands()

      const result = parseInput('/unknown command')

      expect(result.isCommand).toBe(true)
      expect(result.command).toBeNull()
    })

    it('should be case insensitive for commands', () => {
      const { parseInput } = useSlashCommands()

      const result = parseInput('/SUMMARIZE text')

      expect(result.command?.command).toBe('/summarize')
    })

    it('should trim whitespace', () => {
      const { parseInput } = useSlashCommands()

      const result = parseInput('  /summarize text  ')

      expect(result.command?.command).toBe('/summarize')
    })
  })

  describe('filterCommands', () => {
    it('should return empty array for non-slash input', () => {
      const { filterCommands } = useSlashCommands()

      const result = filterCommands('hello')

      expect(result).toEqual([])
    })

    it('should return all commands for just slash', () => {
      const { filterCommands } = useSlashCommands()

      const result = filterCommands('/')

      expect(result).toEqual(SLASH_COMMANDS)
    })

    it('should filter by command prefix', () => {
      const { filterCommands } = useSlashCommands()

      const result = filterCommands('/sum')

      expect(result.length).toBeGreaterThan(0)
      expect(result.some((c) => c.command === '/summarize')).toBe(true)
    })

    it('should filter by alias', () => {
      const { filterCommands } = useSlashCommands()

      const result = filterCommands('/tr')

      expect(result.some((c) => c.command === '/translate')).toBe(true)
    })

    it('should filter by label', () => {
      const { filterCommands } = useSlashCommands()

      const result = filterCommands('/summa')

      expect(result.some((c) => c.label === 'Summarize')).toBe(true)
    })

    it('should filter by description', () => {
      const { filterCommands } = useSlashCommands()

      // Filter by word in description "people"
      const result = filterCommands('/people')

      expect(result.some((c) => c.description.toLowerCase().includes('people'))).toBe(true)
    })
  })

  describe('handleInputChange', () => {
    it('should show suggestions for slash input', () => {
      const { handleInputChange, showCommandSuggestions, filteredCommands } = useSlashCommands()

      handleInputChange('/s')

      expect(showCommandSuggestions.value).toBe(true)
      expect(filteredCommands.value.length).toBeGreaterThan(0)
    })

    it('should hide suggestions when input has space', () => {
      const { handleInputChange, showCommandSuggestions } = useSlashCommands()

      handleInputChange('/summarize ')

      expect(showCommandSuggestions.value).toBe(false)
    })

    it('should hide suggestions for non-slash input', () => {
      const { handleInputChange, showCommandSuggestions } = useSlashCommands()

      handleInputChange('hello')

      expect(showCommandSuggestions.value).toBe(false)
    })

    it('should reset selected index', () => {
      const { handleInputChange, selectedCommandIndex } = useSlashCommands()

      handleInputChange('/s')

      expect(selectedCommandIndex.value).toBe(0)
    })

    it('should hide suggestions when no matches', () => {
      const { handleInputChange, showCommandSuggestions } = useSlashCommands()

      handleInputChange('/xyz123nonexistent')

      expect(showCommandSuggestions.value).toBe(false)
    })
  })

  describe('navigateCommands', () => {
    it('should navigate down', () => {
      const { handleInputChange, navigateCommands, selectedCommandIndex } = useSlashCommands()

      handleInputChange('/')
      navigateCommands('down')

      expect(selectedCommandIndex.value).toBe(1)
    })

    it('should navigate up', () => {
      const { handleInputChange, navigateCommands, selectedCommandIndex } = useSlashCommands()

      handleInputChange('/')
      navigateCommands('down')
      navigateCommands('down')
      navigateCommands('up')

      expect(selectedCommandIndex.value).toBe(1)
    })

    it('should not go below 0', () => {
      const { handleInputChange, navigateCommands, selectedCommandIndex } = useSlashCommands()

      handleInputChange('/')
      navigateCommands('up')
      navigateCommands('up')

      expect(selectedCommandIndex.value).toBe(0)
    })

    it('should not exceed max index', () => {
      const { handleInputChange, navigateCommands, selectedCommandIndex, filteredCommands } =
        useSlashCommands()

      handleInputChange('/')
      const maxIndex = filteredCommands.value.length - 1

      for (let i = 0; i < 20; i++) {
        navigateCommands('down')
      }

      expect(selectedCommandIndex.value).toBe(maxIndex)
    })

    it('should not navigate when suggestions hidden', () => {
      const { navigateCommands, selectedCommandIndex } = useSlashCommands()

      navigateCommands('down')

      expect(selectedCommandIndex.value).toBe(0)
    })
  })

  describe('selectCommand', () => {
    it('should return command with trailing space', () => {
      const { selectCommand } = useSlashCommands()

      const result = selectCommand(SLASH_COMMANDS[0])

      expect(result).toBe(SLASH_COMMANDS[0].command + ' ')
    })

    it('should hide suggestions', () => {
      const { handleInputChange, selectCommand, showCommandSuggestions, filteredCommands } =
        useSlashCommands()

      handleInputChange('/')
      expect(showCommandSuggestions.value).toBe(true)

      selectCommand(filteredCommands.value[0])

      expect(showCommandSuggestions.value).toBe(false)
    })
  })

  describe('getSelectedCommand', () => {
    it('should return null when suggestions hidden', () => {
      const { getSelectedCommand } = useSlashCommands()

      const result = getSelectedCommand()

      expect(result).toBeNull()
    })

    it('should return selected command', () => {
      const { handleInputChange, getSelectedCommand, filteredCommands } = useSlashCommands()

      handleInputChange('/')
      const result = getSelectedCommand()

      expect(result).toBe(filteredCommands.value[0])
    })

    it('should return null when no filtered commands', () => {
      const { handleInputChange, getSelectedCommand } = useSlashCommands()

      handleInputChange('/xyznonexistent')
      const result = getSelectedCommand()

      expect(result).toBeNull()
    })
  })

  describe('formatCommandHelp', () => {
    it('should format command with aliases', () => {
      const { formatCommandHelp } = useSlashCommands()

      const summarize = SLASH_COMMANDS.find((c) => c.command === '/summarize')!
      const result = formatCommandHelp(summarize)

      expect(result).toContain('/summarize')
      expect(result).toContain('/sum')
      expect(result).toContain('/summary')
      expect(result).toContain(summarize.description)
    })

    it('should include options when available', () => {
      const { formatCommandHelp } = useSlashCommands()

      const summarize = SLASH_COMMANDS.find((c) => c.command === '/summarize')!
      const result = formatCommandHelp(summarize)

      expect(result).toContain('Options:')
      expect(result).toContain('Brief')
      expect(result).toContain('Detailed')
    })

    it('should not include options when not available', () => {
      const { formatCommandHelp } = useSlashCommands()

      const help = SLASH_COMMANDS.find((c) => c.command === '/help')!
      const result = formatCommandHelp(help)

      expect(result).not.toContain('Options:')
    })
  })

  describe('getAllCommandsHelp', () => {
    it('should return help for all commands except help', () => {
      const { getAllCommandsHelp } = useSlashCommands()

      const result = getAllCommandsHelp()

      expect(result).toContain('/summarize')
      expect(result).toContain('/translate')
      expect(result).not.toContain('**/help**')
    })

    it('should include descriptions', () => {
      const { getAllCommandsHelp } = useSlashCommands()

      const result = getAllCommandsHelp()

      expect(result).toContain('Generate a summary')
      expect(result).toContain('Translate text')
    })
  })

  describe('State reactivity', () => {
    it('should expose reactive state', () => {
      const { showCommandSuggestions, filteredCommands, selectedCommandIndex } = useSlashCommands()

      expect(showCommandSuggestions.value).toBe(false)
      expect(Array.isArray(filteredCommands.value)).toBe(true)
      expect(typeof selectedCommandIndex.value).toBe('number')
    })

    it('should expose SLASH_COMMANDS constant', () => {
      const { SLASH_COMMANDS: commands } = useSlashCommands()

      expect(commands).toBe(SLASH_COMMANDS)
    })
  })

  describe('Command-specific tests', () => {
    it('should parse /entities command', () => {
      const { parseInput } = useSlashCommands()

      const result = parseInput('/entities Extract names from this text')

      expect(result.command?.operation).toBe('extract-entities')
      expect(result.input).toBe('Extract names from this text')
    })

    it('should parse /sentiment command', () => {
      const { parseInput } = useSlashCommands()

      const result = parseInput('/sentiment I love this product!')

      expect(result.command?.operation).toBe('analyze-sentiment')
    })

    it('should parse /classify command', () => {
      const { parseInput } = useSlashCommands()

      const result = parseInput('/classify This is a sports article')

      expect(result.command?.operation).toBe('classify')
    })

    it('should parse /ocr command without input', () => {
      const { parseInput } = useSlashCommands()

      const result = parseInput('/ocr')

      expect(result.command?.requiresInput).toBe(false)
    })

    it('should parse /titles command with style option', () => {
      const { parseInput } = useSlashCommands()

      const result = parseInput('/titles formal Article about technology')

      expect(result.command?.operation).toBe('generate-title')
      expect(result.option).toBe('formal')
    })

    it('should parse /search command', () => {
      const { parseInput } = useSlashCommands()

      const result = parseInput('/search tournament schedule')

      expect(result.command?.operation).toBe('smart-search')
    })
  })
})
