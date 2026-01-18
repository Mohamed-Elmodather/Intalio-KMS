<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useUIStore } from '@/stores/ui'
import { useAIServicesStore } from '@/stores/aiServices'
import { AILoadingIndicator, AISuggestionChip } from '@/components/ai'

const { t } = useI18n()

// Text constants for localization
const textConstants = {
  // Header
  pageTitle: 'Create New Poll',
  pageSubtitle: 'Gather valuable insights and feedback from your team',
  back: 'Back',

  // Steps
  step1: 'Question',
  step2: 'Options',
  step3: 'Settings',
  step4: 'Preview',

  // Question Section
  questionLabel: 'Poll Question',
  questionPlaceholder: 'What would you like to ask your team?',
  questionHint: 'Write a clear, concise question that your audience can easily understand',
  descriptionLabel: 'Description (Optional)',
  descriptionPlaceholder: 'Add more context or details about your poll...',
  categoryLabel: 'Category',
  selectCategory: 'Select a category',

  // Options Section
  optionsLabel: 'Answer Options',
  optionsHint: 'Add 2-6 possible answers for your poll',
  optionPlaceholder: 'Option',
  addOption: 'Add Option',
  maxOptionsReached: 'Maximum 6 options allowed',
  minOptionsRequired: 'Minimum 2 options required',

  // Settings Section
  settingsLabel: 'Poll Settings',
  durationLabel: 'Poll Duration',
  customDate: 'Custom End Date',
  startDateLabel: 'Start Date',
  startNow: 'Start immediately',
  scheduleStart: 'Schedule for later',
  targetAudienceLabel: 'Target Audience',
  audienceAll: 'All Members',
  audienceTeam: 'My Team Only',
  audienceDept: 'My Department',
  audienceCustom: 'Custom Selection',
  resultsVisibilityLabel: 'Results Visibility',
  resultsAlways: 'Always visible',
  resultsAfterVote: 'After voting',
  resultsAfterEnd: 'After poll ends',
  votingOptionsLabel: 'Voting Options',
  allowMultiple: 'Allow multiple selections',
  anonymousVoting: 'Anonymous voting',
  requireComment: 'Require comment with vote',

  // Preview
  previewLabel: 'Preview',
  previewHint: 'This is how your poll will appear to voters',
  livePreview: 'Live Preview',
  votes: 'votes',
  endsIn: 'Ends in',

  // Actions
  cancel: 'Cancel',
  saveDraft: 'Save as Draft',
  createPoll: 'Create Poll',
  creating: 'Creating...',

  // AI
  aiSuggest: 'AI Suggest',
  aiGenerate: 'AI Generate Poll',
  aiSuggestionsTitle: 'AI Poll Generator',
  aiSuggestionsSubtitle: 'Generate polls from templates, topics, or your content',
  refresh: 'Refresh',
  generating: 'Generating suggestions...',
  generatingFromTopic: 'Analyzing topic and generating polls...',
  generatingFromContent: 'Analyzing content and extracting poll ideas...',
  noSuggestions: 'No suggestions available. Try a different category.',

  // AI Generation Modes
  modeTemplates: 'Templates',
  modeFromTopic: 'From Topic',
  modeFromContent: 'From Content',
  templatesDesc: 'Choose from pre-built poll templates',
  topicDesc: 'Enter a topic and AI will generate relevant polls',
  contentDesc: 'Paste content and AI will extract poll questions',

  // Topic Input
  topicLabel: 'What topic would you like to create a poll about?',
  topicPlaceholder: 'e.g., Remote work policies, Team productivity, Product feedback...',
  topicHint: 'Be specific for better results. Include context like your industry or team.',
  generateFromTopic: 'Generate Polls',

  // Content Input
  contentLabel: 'Paste your content below',
  contentPlaceholder: 'Paste an article, meeting notes, document excerpt, or any text content here...\n\nThe AI will analyze the content and suggest relevant poll questions based on key topics, decisions, or feedback opportunities identified.',
  contentHint: 'Works best with 100-2000 words. Supports articles, meeting notes, documents, etc.',
  generateFromContent: 'Analyze & Generate',
  contentCharCount: 'characters',

  // AI Results
  aiResultsTitle: 'Generated Poll Suggestions',
  aiResultsFromTopic: 'Based on your topic',
  aiResultsFromContent: 'Extracted from your content',
  noTopicEntered: 'Please enter a topic to generate poll suggestions.',
  noContentEntered: 'Please paste some content to analyze.',
  topicTooShort: 'Please enter a more descriptive topic (at least 10 characters).',
  contentTooShort: 'Please provide more content (at least 50 characters).',

  // Categories
  catGeneral: 'General',
  catFeedback: 'Feedback',
  catTeam: 'Team Building',
  catProduct: 'Product',
  catHR: 'HR & Culture',
  catEvents: 'Events',

  // Duration Presets
  duration1Day: '1 Day',
  duration3Days: '3 Days',
  duration1Week: '1 Week',
  duration2Weeks: '2 Weeks',
  durationCustom: 'Custom',

  // Validation
  questionRequired: 'Please enter a poll question',
  optionsRequired: 'Please add at least 2 options',
  successTitle: 'Poll Created!',
  successMessage: 'Your poll is now live and ready to collect responses',
  draftSavedTitle: 'Draft Saved',
  draftSavedMessage: 'Your poll has been saved as a draft',

  // Breadcrumb
  polls: 'Polls',
  createNew: 'Create New',

  // Preview Badges
  anonymous: 'Anonymous',
  multiple: 'Multiple',

  // Tips
  tipsTitle: 'Tips for Great Polls',
  tip1: 'Keep questions clear and concise',
  tip2: 'Provide balanced answer options',
  tip3: 'Avoid leading questions',
  tip4: 'Set appropriate deadlines',
  tip5: 'Use anonymous voting for sensitive topics',

  // AI Applied
  aiAppliedTitle: 'AI Suggestion Applied',
  aiAppliedMessage: 'Poll question and options have been filled in',
  failedToCreate: 'Failed to create poll. Please try again.',
}

const router = useRouter()
const uiStore = useUIStore()
const aiStore = useAIServicesStore()

// Form State
const question = ref('')
const description = ref('')
const selectedCategory = ref('')
const options = ref(['', ''])
const allowMultiple = ref(false)
const isAnonymous = ref(true)
const requireComment = ref(false)
const selectedDuration = ref('1week')
const customEndDate = ref('')
const scheduleStart = ref(false)
const startDate = ref('')
const targetAudience = ref('all')
const resultsVisibility = ref('after_vote')
const isCreating = ref(false)

// AI State
const showAISuggestionsModal = ref(false)
const isGeneratingSuggestions = ref(false)
const aiSuggestions = ref<AIPollSuggestion[]>([])
const aiSelectedCategory = ref('general')
const aiGenerationMode = ref<'templates' | 'topic' | 'content'>('templates')
const aiTopicInput = ref('')
const aiContentInput = ref('')
const aiGenerationSource = ref<string | null>(null) // tracks what generated the current suggestions

// UI State
const currentStep = ref(1)
const showPreview = ref(true)

// Interfaces
interface AIPollSuggestion {
  id: string
  question: string
  options: string[]
  category: string
  purpose: string
  confidence?: number // AI confidence score
  sourceHighlight?: string // relevant excerpt from content
}

// AI Generation Modes
const aiModes = [
  { id: 'templates', label: textConstants.modeTemplates, icon: 'fas fa-th-large', desc: textConstants.templatesDesc },
  { id: 'topic', label: textConstants.modeFromTopic, icon: 'fas fa-lightbulb', desc: textConstants.topicDesc },
  { id: 'content', label: textConstants.modeFromContent, icon: 'fas fa-file-alt', desc: textConstants.contentDesc },
]

// Categories
const categories = [
  { id: 'general', label: textConstants.catGeneral, icon: 'fas fa-th-large', color: 'bg-gray-100 text-gray-600' },
  { id: 'feedback', label: textConstants.catFeedback, icon: 'fas fa-comment-dots', color: 'bg-blue-100 text-blue-600' },
  { id: 'team', label: textConstants.catTeam, icon: 'fas fa-users', color: 'bg-purple-100 text-purple-600' },
  { id: 'product', label: textConstants.catProduct, icon: 'fas fa-box', color: 'bg-orange-100 text-orange-600' },
  { id: 'hr', label: textConstants.catHR, icon: 'fas fa-building', color: 'bg-green-100 text-green-600' },
  { id: 'events', label: textConstants.catEvents, icon: 'fas fa-calendar-alt', color: 'bg-pink-100 text-pink-600' },
]

// Duration Presets
const durationPresets = [
  { id: '1day', label: textConstants.duration1Day, days: 1 },
  { id: '3days', label: textConstants.duration3Days, days: 3 },
  { id: '1week', label: textConstants.duration1Week, days: 7 },
  { id: '2weeks', label: textConstants.duration2Weeks, days: 14 },
  { id: 'custom', label: textConstants.durationCustom, days: 0 },
]

// AI Categories
const aiCategories = [
  { id: 'general', label: 'General', icon: 'fas fa-th-large' },
  { id: 'feedback', label: 'Feedback', icon: 'fas fa-comment-dots' },
  { id: 'team', label: 'Team Building', icon: 'fas fa-users' },
  { id: 'product', label: 'Product', icon: 'fas fa-box' },
  { id: 'hr', label: 'HR & Culture', icon: 'fas fa-building' }
]

// Mock AI Suggestions - Templates
const mockAISuggestions: Record<string, AIPollSuggestion[]> = {
  general: [
    { id: '1', question: 'What day works best for team meetings?', options: ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday'], category: 'general', purpose: 'Optimize scheduling' },
    { id: '2', question: 'How satisfied are you with the current work environment?', options: ['Very satisfied', 'Satisfied', 'Neutral', 'Dissatisfied', 'Very dissatisfied'], category: 'general', purpose: 'Measure satisfaction' },
    { id: '11', question: 'What communication tool do you prefer for quick updates?', options: ['Slack', 'Teams', 'Email', 'In-person', 'Phone'], category: 'general', purpose: 'Improve communication' }
  ],
  feedback: [
    { id: '3', question: 'How would you rate the recent company update presentation?', options: ['Excellent', 'Good', 'Average', 'Below average', 'Poor'], category: 'feedback', purpose: 'Evaluate communications' },
    { id: '4', question: 'What aspect of our product needs the most improvement?', options: ['User interface', 'Performance', 'Features', 'Documentation', 'Support'], category: 'feedback', purpose: 'Prioritize improvements' },
    { id: '12', question: 'How effective was the last training session?', options: ['Very effective', 'Effective', 'Somewhat effective', 'Not very effective', 'Not at all effective'], category: 'feedback', purpose: 'Evaluate training' }
  ],
  team: [
    { id: '5', question: 'What type of team building activity would you prefer?', options: ['Outdoor activities', 'Game nights', 'Workshops', 'Volunteer events', 'Casual dinners'], category: 'team', purpose: 'Plan team activities' },
    { id: '6', question: 'How often should we have team social events?', options: ['Weekly', 'Bi-weekly', 'Monthly', 'Quarterly', 'As needed'], category: 'team', purpose: 'Set social frequency' },
    { id: '13', question: 'What would improve team collaboration the most?', options: ['Better tools', 'More meetings', 'Clearer goals', 'Team bonding', 'Less meetings'], category: 'team', purpose: 'Improve collaboration' }
  ],
  product: [
    { id: '7', question: 'Which feature should we prioritize in the next release?', options: ['Dashboard redesign', 'Mobile app', 'Integration APIs', 'Reporting tools', 'Performance improvements'], category: 'product', purpose: 'Guide roadmap' },
    { id: '8', question: 'How do you primarily use our product?', options: ['Daily tasks', 'Project management', 'Collaboration', 'Reporting', 'Other'], category: 'product', purpose: 'Understand usage' }
  ],
  hr: [
    { id: '9', question: 'What benefit would you value most?', options: ['Health insurance', 'Remote work', 'Learning budget', 'Extra PTO', 'Wellness programs'], category: 'hr', purpose: 'Improve benefits' },
    { id: '10', question: 'How would you rate the onboarding experience?', options: ['Excellent', 'Good', 'Average', 'Needs improvement', 'Poor'], category: 'hr', purpose: 'Evaluate onboarding' }
  ]
}

// Topic-based AI Suggestions - Maps keywords to relevant polls
const topicPollTemplates: Record<string, AIPollSuggestion[]> = {
  remote: [
    { id: 't1', question: 'How many days per week would you prefer to work remotely?', options: ['0 days (fully in-office)', '1-2 days', '3-4 days', '5 days (fully remote)'], category: 'hr', purpose: 'Set remote policy', confidence: 95 },
    { id: 't2', question: 'What is your biggest challenge when working remotely?', options: ['Communication', 'Work-life balance', 'Distractions at home', 'Feeling isolated', 'Technical issues'], category: 'hr', purpose: 'Identify challenges', confidence: 88 },
    { id: 't3', question: 'Which tool is most essential for your remote work?', options: ['Video conferencing', 'Chat platforms', 'Project management', 'Cloud storage', 'VPN/Security tools'], category: 'general', purpose: 'Prioritize tools', confidence: 82 }
  ],
  productivity: [
    { id: 'p1', question: 'What time of day are you most productive?', options: ['Early morning (6-9 AM)', 'Late morning (9-12 PM)', 'Afternoon (12-5 PM)', 'Evening (5-8 PM)', 'Night (after 8 PM)'], category: 'general', purpose: 'Optimize schedules', confidence: 91 },
    { id: 'p2', question: 'What most affects your productivity at work?', options: ['Too many meetings', 'Unclear priorities', 'Distractions/interruptions', 'Lack of resources', 'Poor communication'], category: 'feedback', purpose: 'Remove blockers', confidence: 87 },
    { id: 'p3', question: 'How would you rate your current workload?', options: ['Too light', 'Just right', 'Slightly heavy', 'Very heavy', 'Overwhelming'], category: 'hr', purpose: 'Balance workload', confidence: 84 }
  ],
  feedback: [
    { id: 'f1', question: 'How satisfied are you with the feedback you receive from your manager?', options: ['Very satisfied', 'Satisfied', 'Neutral', 'Dissatisfied', 'Very dissatisfied'], category: 'feedback', purpose: 'Improve feedback', confidence: 93 },
    { id: 'f2', question: 'How often would you like to receive performance feedback?', options: ['Weekly', 'Bi-weekly', 'Monthly', 'Quarterly', 'Only during reviews'], category: 'feedback', purpose: 'Set frequency', confidence: 89 },
    { id: 'f3', question: 'What type of feedback is most valuable to you?', options: ['Verbal praise', 'Written feedback', '360-degree reviews', 'One-on-one discussions', 'Team retrospectives'], category: 'feedback', purpose: 'Customize approach', confidence: 85 }
  ],
  training: [
    { id: 'tr1', question: 'Which skill would you most like to develop?', options: ['Technical skills', 'Leadership', 'Communication', 'Project management', 'Industry knowledge'], category: 'hr', purpose: 'Plan training', confidence: 92 },
    { id: 'tr2', question: 'What is your preferred learning format?', options: ['Online self-paced', 'Live workshops', 'One-on-one mentoring', 'Video tutorials', 'Books/documentation'], category: 'hr', purpose: 'Design programs', confidence: 88 },
    { id: 'tr3', question: 'How much time can you dedicate to learning per week?', options: ['Less than 1 hour', '1-2 hours', '3-4 hours', '5+ hours', 'Varies weekly'], category: 'hr', purpose: 'Schedule training', confidence: 81 }
  ],
  events: [
    { id: 'e1', question: 'What type of company event interests you most?', options: ['Team building activities', 'Professional workshops', 'Social gatherings', 'Volunteer opportunities', 'Sports/wellness events'], category: 'events', purpose: 'Plan events', confidence: 94 },
    { id: 'e2', question: 'What is your preferred day for company events?', options: ['Monday', 'Wednesday', 'Friday', 'Weekend', 'No preference'], category: 'events', purpose: 'Schedule events', confidence: 86 },
    { id: 'e3', question: 'How often should we organize team events?', options: ['Weekly', 'Monthly', 'Quarterly', 'Twice a year', 'Annually'], category: 'events', purpose: 'Set frequency', confidence: 83 }
  ],
  product: [
    { id: 'pd1', question: 'Which aspect of our product needs the most improvement?', options: ['User interface', 'Performance/speed', 'New features', 'Documentation', 'Mobile experience'], category: 'product', purpose: 'Prioritize development', confidence: 90 },
    { id: 'pd2', question: 'How would you rate the overall quality of our product?', options: ['Excellent', 'Good', 'Average', 'Below average', 'Poor'], category: 'product', purpose: 'Measure quality', confidence: 87 },
    { id: 'pd3', question: 'What feature would you most like us to add?', options: ['Better reporting', 'Mobile app', 'Third-party integrations', 'AI/automation', 'Collaboration tools'], category: 'product', purpose: 'Guide roadmap', confidence: 85 }
  ],
  communication: [
    { id: 'c1', question: 'How effective is internal communication in our organization?', options: ['Very effective', 'Effective', 'Neutral', 'Ineffective', 'Very ineffective'], category: 'feedback', purpose: 'Improve communication', confidence: 91 },
    { id: 'c2', question: 'Which communication channel do you prefer for team updates?', options: ['Email', 'Slack/Teams', 'Meetings', 'Newsletter', 'Intranet'], category: 'general', purpose: 'Optimize channels', confidence: 88 },
    { id: 'c3', question: 'How often should leadership share company updates?', options: ['Weekly', 'Bi-weekly', 'Monthly', 'Quarterly', 'When necessary'], category: 'general', purpose: 'Set expectations', confidence: 84 }
  ],
  meeting: [
    { id: 'm1', question: 'How much time do you spend in meetings per week?', options: ['Less than 5 hours', '5-10 hours', '10-15 hours', '15-20 hours', 'More than 20 hours'], category: 'general', purpose: 'Optimize time', confidence: 89 },
    { id: 'm2', question: 'What would make meetings more productive?', options: ['Clear agendas', 'Shorter duration', 'Fewer attendees', 'Better follow-up', 'More breaks'], category: 'feedback', purpose: 'Improve meetings', confidence: 86 },
    { id: 'm3', question: 'Which meeting type do you find most valuable?', options: ['One-on-ones', 'Team standups', 'Brainstorming sessions', 'Status updates', 'Planning meetings'], category: 'general', purpose: 'Prioritize formats', confidence: 82 }
  ]
}

// Function to generate polls from a topic
function generatePollsFromTopic(topic: string): AIPollSuggestion[] {
  const lowerTopic = topic.toLowerCase()
  const results: AIPollSuggestion[] = []

  // Search for matching templates based on keywords
  for (const [keyword, templates] of Object.entries(topicPollTemplates)) {
    if (lowerTopic.includes(keyword) || keyword.includes(lowerTopic.slice(0, 4))) {
      results.push(...templates)
    }
  }

  // If no direct matches, generate generic polls based on the topic
  if (results.length === 0) {
    return [
      {
        id: 'gen1',
        question: `What aspect of "${topic}" is most important to you?`,
        options: ['Quality', 'Speed', 'Cost', 'Innovation', 'Support'],
        category: 'general',
        purpose: 'Identify priorities',
        confidence: 75
      },
      {
        id: 'gen2',
        question: `How satisfied are you with our current approach to ${topic}?`,
        options: ['Very satisfied', 'Satisfied', 'Neutral', 'Dissatisfied', 'Very dissatisfied'],
        category: 'feedback',
        purpose: 'Measure satisfaction',
        confidence: 72
      },
      {
        id: 'gen3',
        question: `What should be our top priority regarding ${topic}?`,
        options: ['Improve existing process', 'Explore new options', 'Increase resources', 'Get more training', 'Gather more feedback'],
        category: 'general',
        purpose: 'Set direction',
        confidence: 68
      }
    ]
  }

  // Return unique results (max 5)
  const uniqueResults = results.filter((poll, index, self) =>
    index === self.findIndex(p => p.id === poll.id)
  )
  return uniqueResults.slice(0, 5)
}

// Function to analyze content and generate polls
function generatePollsFromContent(content: string): AIPollSuggestion[] {
  const lowerContent = content.toLowerCase()
  const results: AIPollSuggestion[] = []

  // Extract key topics from content
  const topicKeywords = ['meeting', 'remote', 'work', 'team', 'project', 'deadline', 'feedback',
    'performance', 'training', 'event', 'product', 'feature', 'communication', 'budget',
    'schedule', 'productivity', 'collaboration', 'policy', 'decision', 'strategy']

  const foundTopics: string[] = []
  for (const keyword of topicKeywords) {
    if (lowerContent.includes(keyword)) {
      foundTopics.push(keyword)
    }
  }

  // Generate polls based on found topics
  for (const topic of foundTopics.slice(0, 3)) { // Limit to first 3 topics
    const topicPolls = topicPollTemplates[topic]
    if (topicPolls) {
      // Add first poll from each topic and include source highlight
      const poll = { ...topicPolls[0] }

      // Find a relevant sentence from the content
      const sentences = content.split(/[.!?]+/).filter(s => s.trim())
      const relevantSentence = sentences.find(s => s.toLowerCase().includes(topic))
      if (relevantSentence) {
        poll.sourceHighlight = relevantSentence.trim().slice(0, 100) + (relevantSentence.length > 100 ? '...' : '')
      }
      results.push(poll)
    }
  }

  // If specific topics found, also generate content-specific polls
  if (lowerContent.includes('decision') || lowerContent.includes('choose') || lowerContent.includes('option')) {
    results.push({
      id: 'content1',
      question: 'Which option should we proceed with based on the discussion?',
      options: ['Option A - Conservative approach', 'Option B - Balanced approach', 'Option C - Aggressive approach', 'Need more information', 'Postpone decision'],
      category: 'general',
      purpose: 'Make decision',
      confidence: 85,
      sourceHighlight: 'Multiple options or decisions mentioned in content'
    })
  }

  if (lowerContent.includes('feedback') || lowerContent.includes('opinion') || lowerContent.includes('think')) {
    results.push({
      id: 'content2',
      question: 'How do you feel about the points raised in this discussion?',
      options: ['Strongly agree', 'Agree', 'Neutral', 'Disagree', 'Strongly disagree'],
      category: 'feedback',
      purpose: 'Gather opinions',
      confidence: 82,
      sourceHighlight: 'Feedback or opinions requested in content'
    })
  }

  if (lowerContent.includes('priority') || lowerContent.includes('important') || lowerContent.includes('focus')) {
    results.push({
      id: 'content3',
      question: 'What should be our top priority based on this content?',
      options: ['Short-term wins', 'Long-term strategy', 'Resource optimization', 'Team development', 'Customer satisfaction'],
      category: 'general',
      purpose: 'Set priorities',
      confidence: 79,
      sourceHighlight: 'Priority setting mentioned in content'
    })
  }

  // Fallback if no patterns matched
  if (results.length === 0) {
    return [
      {
        id: 'fallback1',
        question: 'How relevant is this content to your daily work?',
        options: ['Very relevant', 'Somewhat relevant', 'Neutral', 'Not very relevant', 'Not relevant at all'],
        category: 'feedback',
        purpose: 'Assess relevance',
        confidence: 65,
        sourceHighlight: 'General content analysis'
      },
      {
        id: 'fallback2',
        question: 'What action should we take based on this content?',
        options: ['Discuss further', 'Implement changes', 'Gather more data', 'Share with team', 'Archive for reference'],
        category: 'general',
        purpose: 'Determine next steps',
        confidence: 60,
        sourceHighlight: 'Action items extracted from content'
      }
    ]
  }

  return results.slice(0, 5)
}

// Computed
const selectedCategoryData = computed(() => {
  return categories.find(c => c.id === selectedCategory.value)
})

const endDateDisplay = computed(() => {
  if (selectedDuration.value === 'custom' && customEndDate.value) {
    return new Date(customEndDate.value).toLocaleDateString('en-US', {
      month: 'short', day: 'numeric', year: 'numeric'
    })
  }
  const preset = durationPresets.find(p => p.id === selectedDuration.value)
  if (preset && preset.days > 0) {
    const date = new Date()
    date.setDate(date.getDate() + preset.days)
    return date.toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' })
  }
  return 'Not set'
})

const daysRemaining = computed(() => {
  const preset = durationPresets.find(p => p.id === selectedDuration.value)
  if (preset && preset.days > 0) {
    return `${preset.days} day${preset.days > 1 ? 's' : ''}`
  }
  if (selectedDuration.value === 'custom' && customEndDate.value) {
    const end = new Date(customEndDate.value)
    const now = new Date()
    const diff = Math.ceil((end.getTime() - now.getTime()) / (1000 * 60 * 60 * 24))
    return diff > 0 ? `${diff} day${diff > 1 ? 's' : ''}` : 'Ended'
  }
  return '7 days'
})

const filledOptions = computed(() => {
  return options.value.filter(opt => opt.trim() !== '')
})

const canCreate = computed(() => {
  return question.value.trim() !== '' && filledOptions.value.length >= 2
})

// Functions
function addOption() {
  if (options.value.length < 6) {
    options.value.push('')
  }
}

function removeOption(index: number) {
  if (options.value.length > 2) {
    options.value.splice(index, 1)
  }
}

// Open AI modal and reset to template mode
function openAIModal() {
  showAISuggestionsModal.value = true
  aiGenerationMode.value = 'templates'
  aiGenerationSource.value = null
  aiSuggestions.value = []
}

// Generate poll suggestions based on current mode
async function generatePollSuggestions() {
  isGeneratingSuggestions.value = true
  showAISuggestionsModal.value = true

  try {
    await new Promise(resolve => setTimeout(resolve, 1200))
    aiSuggestions.value = mockAISuggestions[aiSelectedCategory.value] || mockAISuggestions.general
    aiGenerationSource.value = 'templates'
  } catch (error) {
    console.error('Failed to generate suggestions:', error)
  } finally {
    isGeneratingSuggestions.value = false
  }
}

// Generate polls from a topic
async function generateFromTopic() {
  if (aiTopicInput.value.trim().length < 10) {
    uiStore.showError(textConstants.topicTooShort)
    return
  }

  isGeneratingSuggestions.value = true
  aiGenerationSource.value = 'topic'

  try {
    // Simulate AI processing time
    await new Promise(resolve => setTimeout(resolve, 1500))
    aiSuggestions.value = generatePollsFromTopic(aiTopicInput.value)
  } catch (error) {
    console.error('Failed to generate from topic:', error)
  } finally {
    isGeneratingSuggestions.value = false
  }
}

// Generate polls from pasted content
async function generateFromContent() {
  if (aiContentInput.value.trim().length < 50) {
    uiStore.showError(textConstants.contentTooShort)
    return
  }

  isGeneratingSuggestions.value = true
  aiGenerationSource.value = 'content'

  try {
    // Simulate AI processing time (longer for content analysis)
    await new Promise(resolve => setTimeout(resolve, 2000))
    aiSuggestions.value = generatePollsFromContent(aiContentInput.value)
  } catch (error) {
    console.error('Failed to generate from content:', error)
  } finally {
    isGeneratingSuggestions.value = false
  }
}

// Switch AI generation mode
function setAIMode(mode: 'templates' | 'topic' | 'content') {
  aiGenerationMode.value = mode
  aiSuggestions.value = []
  aiGenerationSource.value = null
}

function selectAICategory(categoryId: string) {
  aiSelectedCategory.value = categoryId
  generatePollSuggestions()
}

// Get character count for content input
const contentCharacterCount = computed(() => aiContentInput.value.length)

// Get loading message based on mode
const generatingMessage = computed(() => {
  switch (aiGenerationMode.value) {
    case 'topic':
      return textConstants.generatingFromTopic
    case 'content':
      return textConstants.generatingFromContent
    default:
      return textConstants.generating
  }
})

function applySuggestion(suggestion: AIPollSuggestion) {
  question.value = suggestion.question
  options.value = [...suggestion.options]
  selectedCategory.value = suggestion.category
  showAISuggestionsModal.value = false
  uiStore.showSuccess(textConstants.aiAppliedTitle, textConstants.aiAppliedMessage)
}

async function createPoll() {
  if (!question.value.trim()) {
    uiStore.showError(textConstants.questionRequired)
    return
  }

  if (filledOptions.value.length < 2) {
    uiStore.showError(textConstants.optionsRequired)
    return
  }

  isCreating.value = true

  try {
    // Simulate API call
    await new Promise(resolve => setTimeout(resolve, 1500))

    uiStore.showSuccess(textConstants.successTitle, textConstants.successMessage)
    router.push({ name: 'Polls' })
  } catch (error) {
    console.error('Failed to create poll:', error)
    uiStore.showError(textConstants.failedToCreate)
  } finally {
    isCreating.value = false
  }
}

function saveDraft() {
  uiStore.showSuccess(textConstants.draftSavedTitle, textConstants.draftSavedMessage)
}

function goBack() {
  router.push({ name: 'Polls' })
}
</script>

<template>
  <div class="poll-create-page min-h-screen bg-gray-50">
    <!-- Enhanced Header -->
    <header class="poll-create-header">
      <!-- Decorative Background -->
      <div class="header-decor">
        <div class="decor-orb decor-orb-1"></div>
        <div class="decor-orb decor-orb-2"></div>
        <div class="decor-pattern"></div>
      </div>

      <div class="header-content">
        <!-- Navigation Row -->
        <div class="header-nav">
          <button @click="goBack" class="back-btn">
            <i class="fas fa-arrow-left"></i>
            <span>{{ textConstants.back }}</span>
          </button>
          <div class="breadcrumb">
            <router-link to="/polls" class="breadcrumb-link">
              <i class="fas fa-poll"></i>
              {{ textConstants.polls }}
            </router-link>
            <i class="fas fa-chevron-right breadcrumb-sep"></i>
            <span class="breadcrumb-current">{{ textConstants.createNew }}</span>
          </div>
        </div>

        <!-- Title & AI Button -->
        <div class="header-main">
          <div class="header-left">
            <div class="header-icon">
              <i class="fas fa-plus"></i>
            </div>
            <div>
              <h1 class="header-title">{{ textConstants.pageTitle }}</h1>
              <p class="header-subtitle">{{ textConstants.pageSubtitle }}</p>
            </div>
          </div>

          <button
            @click="openAIModal"
            class="ai-generate-btn"
          >
            <span class="btn-glow"></span>
            <i class="fas fa-wand-magic-sparkles"></i>
            <span>{{ textConstants.aiGenerate }}</span>
          </button>
        </div>
      </div>
    </header>

    <!-- Main Content -->
    <main class="px-6 py-8">
      <div class="max-w-6xl mx-auto">
        <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
          <!-- Left Column - Form -->
          <div class="lg:col-span-2 space-y-6">
            <!-- Question Section -->
            <section class="form-section fade-in-up" style="animation-delay: 0.1s">
              <div class="section-header">
                <div class="section-icon">
                  <i class="fas fa-question-circle"></i>
                </div>
                <div>
                  <h2 class="section-title">{{ textConstants.questionLabel }}</h2>
                  <p class="section-hint">{{ textConstants.questionHint }}</p>
                </div>
              </div>

              <div class="section-body">
                <!-- Question Input -->
                <div class="form-group">
                  <div class="input-wrapper">
                    <input
                      v-model="question"
                      type="text"
                      :placeholder="textConstants.questionPlaceholder"
                      class="form-input-enhanced"
                    >
                    <button
                      @click="openAIModal"
                      class="input-ai-btn"
                      title="AI Suggest"
                    >
                      <i class="fas fa-wand-magic-sparkles"></i>
                    </button>
                  </div>
                </div>

                <!-- Description -->
                <div class="form-group">
                  <label class="form-label">{{ textConstants.descriptionLabel }}</label>
                  <textarea
                    v-model="description"
                    :placeholder="textConstants.descriptionPlaceholder"
                    rows="3"
                    class="form-textarea"
                  ></textarea>
                </div>

                <!-- Category Selection -->
                <div class="form-group">
                  <label class="form-label">{{ textConstants.categoryLabel }}</label>
                  <div class="category-grid">
                    <button
                      v-for="cat in categories"
                      :key="cat.id"
                      @click="selectedCategory = cat.id"
                      :class="['category-btn', { active: selectedCategory === cat.id }]"
                    >
                      <i :class="cat.icon"></i>
                      <span>{{ cat.label }}</span>
                    </button>
                  </div>
                </div>
              </div>
            </section>

            <!-- Options Section -->
            <section class="form-section fade-in-up" style="animation-delay: 0.2s">
              <div class="section-header">
                <div class="section-icon options">
                  <i class="fas fa-list-ul"></i>
                </div>
                <div>
                  <h2 class="section-title">{{ textConstants.optionsLabel }}</h2>
                  <p class="section-hint">{{ textConstants.optionsHint }}</p>
                </div>
              </div>

              <div class="section-body">
                <div class="options-list">
                  <div
                    v-for="(option, index) in options"
                    :key="index"
                    class="option-item"
                  >
                    <div class="option-number">{{ index + 1 }}</div>
                    <input
                      v-model="options[index]"
                      type="text"
                      :placeholder="`${textConstants.optionPlaceholder} ${index + 1}`"
                      class="option-input"
                    >
                    <button
                      v-if="options.length > 2"
                      @click="removeOption(index)"
                      class="option-remove"
                    >
                      <i class="fas fa-times"></i>
                    </button>
                  </div>
                </div>

                <button
                  v-if="options.length < 6"
                  @click="addOption"
                  class="add-option-btn"
                >
                  <i class="fas fa-plus"></i>
                  <span>{{ textConstants.addOption }}</span>
                </button>
                <p v-else class="options-max-hint">
                  <i class="fas fa-info-circle"></i>
                  {{ textConstants.maxOptionsReached }}
                </p>
              </div>
            </section>

            <!-- Settings Section -->
            <section class="form-section fade-in-up" style="animation-delay: 0.3s">
              <div class="section-header">
                <div class="section-icon settings">
                  <i class="fas fa-cog"></i>
                </div>
                <div>
                  <h2 class="section-title">{{ textConstants.settingsLabel }}</h2>
                </div>
              </div>

              <div class="section-body">
                <!-- Duration -->
                <div class="form-group">
                  <label class="form-label">{{ textConstants.durationLabel }}</label>
                  <div class="duration-presets">
                    <button
                      v-for="preset in durationPresets"
                      :key="preset.id"
                      @click="selectedDuration = preset.id"
                      :class="['duration-btn', { active: selectedDuration === preset.id }]"
                    >
                      {{ preset.label }}
                    </button>
                  </div>
                  <div v-if="selectedDuration === 'custom'" class="custom-date-wrapper">
                    <input
                      v-model="customEndDate"
                      type="date"
                      class="form-input-enhanced"
                      :min="new Date().toISOString().split('T')[0]"
                    >
                  </div>
                </div>

                <!-- Target Audience -->
                <div class="form-group">
                  <label class="form-label">{{ textConstants.targetAudienceLabel }}</label>
                  <div class="radio-group">
                    <label class="radio-option">
                      <input type="radio" v-model="targetAudience" value="all">
                      <span class="radio-label">
                        <i class="fas fa-globe"></i>
                        {{ textConstants.audienceAll }}
                      </span>
                    </label>
                    <label class="radio-option">
                      <input type="radio" v-model="targetAudience" value="team">
                      <span class="radio-label">
                        <i class="fas fa-users"></i>
                        {{ textConstants.audienceTeam }}
                      </span>
                    </label>
                    <label class="radio-option">
                      <input type="radio" v-model="targetAudience" value="department">
                      <span class="radio-label">
                        <i class="fas fa-building"></i>
                        {{ textConstants.audienceDept }}
                      </span>
                    </label>
                  </div>
                </div>

                <!-- Results Visibility -->
                <div class="form-group">
                  <label class="form-label">{{ textConstants.resultsVisibilityLabel }}</label>
                  <div class="radio-group">
                    <label class="radio-option">
                      <input type="radio" v-model="resultsVisibility" value="always">
                      <span class="radio-label">
                        <i class="fas fa-eye"></i>
                        {{ textConstants.resultsAlways }}
                      </span>
                    </label>
                    <label class="radio-option">
                      <input type="radio" v-model="resultsVisibility" value="after_vote">
                      <span class="radio-label">
                        <i class="fas fa-vote-yea"></i>
                        {{ textConstants.resultsAfterVote }}
                      </span>
                    </label>
                    <label class="radio-option">
                      <input type="radio" v-model="resultsVisibility" value="after_end">
                      <span class="radio-label">
                        <i class="fas fa-calendar-check"></i>
                        {{ textConstants.resultsAfterEnd }}
                      </span>
                    </label>
                  </div>
                </div>

                <!-- Voting Options -->
                <div class="form-group">
                  <label class="form-label">{{ textConstants.votingOptionsLabel }}</label>
                  <div class="checkbox-group">
                    <label class="checkbox-option">
                      <input type="checkbox" v-model="allowMultiple">
                      <span class="checkbox-label">
                        <i class="fas fa-check-double"></i>
                        {{ textConstants.allowMultiple }}
                      </span>
                    </label>
                    <label class="checkbox-option">
                      <input type="checkbox" v-model="isAnonymous">
                      <span class="checkbox-label">
                        <i class="fas fa-user-secret"></i>
                        {{ textConstants.anonymousVoting }}
                      </span>
                    </label>
                    <label class="checkbox-option">
                      <input type="checkbox" v-model="requireComment">
                      <span class="checkbox-label">
                        <i class="fas fa-comment"></i>
                        {{ textConstants.requireComment }}
                      </span>
                    </label>
                  </div>
                </div>
              </div>
            </section>

            <!-- Action Buttons (Mobile) -->
            <div class="action-buttons-mobile lg:hidden">
              <button @click="goBack" class="btn-secondary">
                {{ textConstants.cancel }}
              </button>
              <button @click="saveDraft" class="btn-outline">
                <i class="fas fa-save"></i>
                {{ textConstants.saveDraft }}
              </button>
              <button
                @click="createPoll"
                :disabled="!canCreate || isCreating"
                class="btn-primary"
              >
                <i v-if="isCreating" class="fas fa-spinner fa-spin"></i>
                <i v-else class="fas fa-check"></i>
                {{ isCreating ? textConstants.creating : textConstants.createPoll }}
              </button>
            </div>
          </div>

          <!-- Right Column - Preview & Actions -->
          <div class="space-y-6">
            <!-- Live Preview -->
            <div class="preview-card fade-in-up" style="animation-delay: 0.4s">
              <div class="preview-header">
                <div class="preview-badge">
                  <i class="fas fa-eye"></i>
                  {{ textConstants.livePreview }}
                </div>
              </div>

              <div class="preview-content">
                <!-- Category Badge -->
                <div v-if="selectedCategoryData" class="preview-category">
                  <i :class="selectedCategoryData.icon"></i>
                  {{ selectedCategoryData.label }}
                </div>

                <!-- Question -->
                <h3 class="preview-question">
                  {{ question || textConstants.questionPlaceholder }}
                </h3>

                <!-- Description -->
                <p v-if="description" class="preview-description">
                  {{ description }}
                </p>

                <!-- Options -->
                <div class="preview-options">
                  <div
                    v-for="(option, index) in options"
                    :key="index"
                    :class="['preview-option', { empty: !option.trim() }]"
                  >
                    <div class="preview-radio"></div>
                    <span>{{ option || `${textConstants.optionPlaceholder} ${index + 1}` }}</span>
                  </div>
                </div>

                <!-- Footer -->
                <div class="preview-footer">
                  <span class="preview-votes">
                    <i class="fas fa-users"></i>
                    0 {{ textConstants.votes }}
                  </span>
                  <span class="preview-ends">
                    <i class="fas fa-clock"></i>
                    {{ textConstants.endsIn }} {{ daysRemaining }}
                  </span>
                </div>

                <!-- Badges -->
                <div class="preview-badges">
                  <span v-if="isAnonymous" class="badge-anon">
                    <i class="fas fa-user-secret"></i>
                    {{ textConstants.anonymous }}
                  </span>
                  <span v-if="allowMultiple" class="badge-multi">
                    <i class="fas fa-check-double"></i>
                    {{ textConstants.multiple }}
                  </span>
                </div>
              </div>
            </div>

            <!-- Action Buttons -->
            <div class="action-buttons hidden lg:block fade-in-up" style="animation-delay: 0.5s">
              <button @click="goBack" class="btn-secondary w-full">
                {{ textConstants.cancel }}
              </button>
              <button @click="saveDraft" class="btn-outline w-full">
                <i class="fas fa-save"></i>
                {{ textConstants.saveDraft }}
              </button>
              <button
                @click="createPoll"
                :disabled="!canCreate || isCreating"
                class="btn-primary w-full"
              >
                <i v-if="isCreating" class="fas fa-spinner fa-spin"></i>
                <i v-else class="fas fa-paper-plane"></i>
                {{ isCreating ? textConstants.creating : textConstants.createPoll }}
              </button>
            </div>

            <!-- Tips Card -->
            <div class="tips-card fade-in-up" style="animation-delay: 0.6s">
              <h4 class="tips-title">
                <i class="fas fa-lightbulb"></i>
                {{ textConstants.tipsTitle }}
              </h4>
              <ul class="tips-list">
                <li>{{ textConstants.tip1 }}</li>
                <li>{{ textConstants.tip2 }}</li>
                <li>{{ textConstants.tip3 }}</li>
                <li>{{ textConstants.tip4 }}</li>
                <li>{{ textConstants.tip5 }}</li>
              </ul>
            </div>
          </div>
        </div>
      </div>
    </main>

    <!-- AI Poll Suggestions Modal -->
    <Teleport to="body">
      <div v-if="showAISuggestionsModal" class="modal-overlay">
        <div class="ai-modal ai-modal-enhanced">
          <div class="ai-modal-header">
            <div class="ai-modal-title-group">
              <div class="ai-modal-icon">
                <i class="fas fa-wand-magic-sparkles"></i>
              </div>
              <div>
                <h3 class="ai-modal-title">{{ textConstants.aiSuggestionsTitle }}</h3>
                <p class="ai-modal-subtitle">{{ textConstants.aiSuggestionsSubtitle }}</p>
              </div>
            </div>
            <button @click="showAISuggestionsModal = false" class="modal-close">
              <i class="fas fa-times"></i>
            </button>
          </div>

          <!-- Generation Mode Tabs -->
          <div class="ai-mode-tabs">
            <button
              v-for="mode in aiModes"
              :key="mode.id"
              @click="setAIMode(mode.id as 'templates' | 'topic' | 'content')"
              :class="['ai-mode-tab', { active: aiGenerationMode === mode.id }]"
            >
              <i :class="mode.icon"></i>
              <span class="mode-label">{{ mode.label }}</span>
              <span class="mode-desc">{{ mode.desc }}</span>
            </button>
          </div>

          <div class="ai-modal-body">
            <!-- Templates Mode -->
            <div v-if="aiGenerationMode === 'templates'" class="mode-content">
              <!-- Category Tabs -->
              <div class="ai-category-tabs">
                <button
                  v-for="cat in aiCategories"
                  :key="cat.id"
                  @click="selectAICategory(cat.id)"
                  :class="['ai-tab', { active: aiSelectedCategory === cat.id }]"
                >
                  <i :class="cat.icon"></i>
                  {{ cat.label }}
                </button>
              </div>

              <AILoadingIndicator v-if="isGeneratingSuggestions" :message="generatingMessage" />

              <div v-else-if="aiSuggestions.length > 0" class="ai-suggestions-list">
                <div
                  v-for="suggestion in aiSuggestions"
                  :key="suggestion.id"
                  class="ai-suggestion-card"
                  @click="applySuggestion(suggestion)"
                >
                  <div class="suggestion-header">
                    <h4 class="suggestion-question">{{ suggestion.question }}</h4>
                    <span class="suggestion-purpose">{{ suggestion.purpose }}</span>
                  </div>
                  <div class="suggestion-options">
                    <span
                      v-for="(opt, idx) in suggestion.options"
                      :key="idx"
                      class="suggestion-option"
                    >
                      {{ opt }}
                    </span>
                  </div>
                </div>
              </div>

              <div v-else class="ai-empty-state">
                <i class="fas fa-poll"></i>
                <p>{{ textConstants.noSuggestions }}</p>
              </div>
            </div>

            <!-- Topic Mode -->
            <div v-else-if="aiGenerationMode === 'topic'" class="mode-content">
              <div class="topic-input-section">
                <label class="input-label">{{ textConstants.topicLabel }}</label>
                <div class="topic-input-wrapper">
                  <i class="fas fa-lightbulb topic-icon"></i>
                  <input
                    v-model="aiTopicInput"
                    type="text"
                    :placeholder="textConstants.topicPlaceholder"
                    class="topic-input"
                    @keyup.enter="generateFromTopic"
                  >
                </div>
                <p class="input-hint">{{ textConstants.topicHint }}</p>
                <button
                  @click="generateFromTopic"
                  :disabled="aiTopicInput.trim().length < 10 || isGeneratingSuggestions"
                  class="generate-btn"
                >
                  <i v-if="isGeneratingSuggestions" class="fas fa-spinner fa-spin"></i>
                  <i v-else class="fas fa-wand-magic-sparkles"></i>
                  {{ textConstants.generateFromTopic }}
                </button>
              </div>

              <AILoadingIndicator v-if="isGeneratingSuggestions" :message="generatingMessage" />

              <div v-else-if="aiSuggestions.length > 0" class="ai-suggestions-list">
                <div class="results-header">
                  <span class="results-label">{{ textConstants.aiResultsTitle }}</span>
                  <span class="results-source">{{ textConstants.aiResultsFromTopic }}: "{{ aiTopicInput }}"</span>
                </div>
                <div
                  v-for="suggestion in aiSuggestions"
                  :key="suggestion.id"
                  class="ai-suggestion-card"
                  @click="applySuggestion(suggestion)"
                >
                  <div class="suggestion-header">
                    <h4 class="suggestion-question">{{ suggestion.question }}</h4>
                    <div class="suggestion-meta">
                      <span class="suggestion-purpose">{{ suggestion.purpose }}</span>
                      <span v-if="suggestion.confidence" class="suggestion-confidence">
                        <i class="fas fa-robot"></i>
                        {{ suggestion.confidence }}%
                      </span>
                    </div>
                  </div>
                  <div class="suggestion-options">
                    <span
                      v-for="(opt, idx) in suggestion.options"
                      :key="idx"
                      class="suggestion-option"
                    >
                      {{ opt }}
                    </span>
                  </div>
                </div>
              </div>
            </div>

            <!-- Content Mode -->
            <div v-else-if="aiGenerationMode === 'content'" class="mode-content">
              <div class="content-input-section">
                <label class="input-label">{{ textConstants.contentLabel }}</label>
                <div class="content-input-wrapper">
                  <textarea
                    v-model="aiContentInput"
                    :placeholder="textConstants.contentPlaceholder"
                    class="content-input"
                    rows="6"
                  ></textarea>
                  <div class="content-footer">
                    <span class="char-count">{{ contentCharacterCount }} {{ textConstants.contentCharCount }}</span>
                  </div>
                </div>
                <p class="input-hint">{{ textConstants.contentHint }}</p>
                <button
                  @click="generateFromContent"
                  :disabled="aiContentInput.trim().length < 50 || isGeneratingSuggestions"
                  class="generate-btn"
                >
                  <i v-if="isGeneratingSuggestions" class="fas fa-spinner fa-spin"></i>
                  <i v-else class="fas fa-file-alt"></i>
                  {{ textConstants.generateFromContent }}
                </button>
              </div>

              <AILoadingIndicator v-if="isGeneratingSuggestions" :message="generatingMessage" />

              <div v-else-if="aiSuggestions.length > 0" class="ai-suggestions-list">
                <div class="results-header">
                  <span class="results-label">{{ textConstants.aiResultsTitle }}</span>
                  <span class="results-source">{{ textConstants.aiResultsFromContent }}</span>
                </div>
                <div
                  v-for="suggestion in aiSuggestions"
                  :key="suggestion.id"
                  class="ai-suggestion-card"
                  @click="applySuggestion(suggestion)"
                >
                  <div class="suggestion-header">
                    <h4 class="suggestion-question">{{ suggestion.question }}</h4>
                    <div class="suggestion-meta">
                      <span class="suggestion-purpose">{{ suggestion.purpose }}</span>
                      <span v-if="suggestion.confidence" class="suggestion-confidence">
                        <i class="fas fa-robot"></i>
                        {{ suggestion.confidence }}%
                      </span>
                    </div>
                  </div>
                  <div v-if="suggestion.sourceHighlight" class="source-highlight">
                    <i class="fas fa-quote-left"></i>
                    <span>{{ suggestion.sourceHighlight }}</span>
                  </div>
                  <div class="suggestion-options">
                    <span
                      v-for="(opt, idx) in suggestion.options"
                      :key="idx"
                      class="suggestion-option"
                    >
                      {{ opt }}
                    </span>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div class="ai-modal-footer">
            <button
              v-if="aiGenerationMode === 'templates' && !isGeneratingSuggestions"
              @click="generatePollSuggestions"
              class="btn-refresh"
            >
              <i class="fas fa-rotate"></i>
              {{ textConstants.refresh }}
            </button>
            <button @click="showAISuggestionsModal = false" class="btn-cancel">
              {{ textConstants.cancel }}
            </button>
          </div>
        </div>
      </div>
    </Teleport>
  </div>
</template>

<style scoped>
/* Page Styles */
.poll-create-page {
  animation: fadeIn 0.3s ease;
}

@keyframes fadeIn {
  from { opacity: 0; }
  to { opacity: 1; }
}

/* Enhanced Header */
.poll-create-header {
  background: linear-gradient(135deg, #0f766e 0%, #0d9488 50%, #14b8a6 100%);
  position: relative;
  overflow: hidden;
}

.header-decor {
  position: absolute;
  inset: 0;
  pointer-events: none;
  overflow: hidden;
}

.decor-orb {
  position: absolute;
  border-radius: 50%;
  filter: blur(60px);
  opacity: 0.3;
}

.decor-orb-1 {
  width: 300px;
  height: 300px;
  background: linear-gradient(135deg, #5eead4, #99f6e4);
  top: -100px;
  right: -50px;
  animation: orb-float 8s ease-in-out infinite;
}

.decor-orb-2 {
  width: 200px;
  height: 200px;
  background: linear-gradient(135deg, #2dd4bf, #5eead4);
  bottom: -80px;
  left: 10%;
  animation: orb-float 10s ease-in-out infinite reverse;
}

.decor-pattern {
  position: absolute;
  inset: 0;
  background-image: radial-gradient(rgba(255, 255, 255, 0.1) 1px, transparent 1px);
  background-size: 24px 24px;
  opacity: 0.5;
}

@keyframes orb-float {
  0%, 100% { transform: translate(0, 0); }
  50% { transform: translate(20px, -20px); }
}

.header-content {
  position: relative;
  z-index: 2;
  padding: 1.5rem 2rem 2rem;
}

/* Navigation */
.header-nav {
  display: flex;
  align-items: center;
  gap: 1rem;
  margin-bottom: 1.5rem;
}

.back-btn {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem 1rem;
  background: rgba(255, 255, 255, 0.15);
  border: 1px solid rgba(255, 255, 255, 0.2);
  border-radius: 0.75rem;
  color: white;
  font-size: 0.875rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s ease;
}

.back-btn:hover {
  background: rgba(255, 255, 255, 0.25);
  transform: translateX(-2px);
}

.breadcrumb {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.875rem;
}

.breadcrumb-link {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  color: rgba(255, 255, 255, 0.8);
  text-decoration: none;
  transition: color 0.2s ease;
}

.breadcrumb-link:hover {
  color: white;
}

.breadcrumb-sep {
  color: rgba(255, 255, 255, 0.4);
  font-size: 0.625rem;
}

.breadcrumb-current {
  color: white;
  font-weight: 500;
}

/* Header Main */
.header-main {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 2rem;
}

.header-left {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.header-icon {
  width: 56px;
  height: 56px;
  background: rgba(255, 255, 255, 0.2);
  border-radius: 1rem;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.5rem;
  color: white;
}

.header-title {
  font-size: 1.75rem;
  font-weight: 700;
  color: white;
  margin-bottom: 0.25rem;
}

.header-subtitle {
  font-size: 0.9375rem;
  color: rgba(255, 255, 255, 0.85);
}

/* AI Generate Button */
.ai-generate-btn {
  position: relative;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem 1.5rem;
  background: rgba(255, 255, 255, 0.2);
  border: 1px solid rgba(255, 255, 255, 0.3);
  border-radius: 0.75rem;
  color: white;
  font-weight: 600;
  font-size: 0.9375rem;
  cursor: pointer;
  overflow: hidden;
  transition: all 0.3s ease;
}

.ai-generate-btn:hover {
  background: rgba(255, 255, 255, 0.3);
  transform: translateY(-2px);
}

.ai-generate-btn .btn-glow {
  position: absolute;
  inset: 0;
  background: linear-gradient(90deg, transparent, rgba(255,255,255,0.2), transparent);
  transform: translateX(-100%);
  animation: btn-glow 2s ease-in-out infinite;
}

@keyframes btn-glow {
  0% { transform: translateX(-100%); }
  100% { transform: translateX(100%); }
}

/* Fade In Animation */
.fade-in-up {
  animation: fadeInUp 0.5s ease-out forwards;
  opacity: 0;
}

@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

/* Form Sections */
.form-section {
  background: white;
  border-radius: 1.5rem;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
  border: 1px solid #e5e7eb;
  overflow: hidden;
}

.section-header {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 1.25rem 1.5rem;
  background: linear-gradient(135deg, #f9fafb 0%, #f3f4f6 100%);
  border-bottom: 1px solid #e5e7eb;
}

.section-icon {
  width: 44px;
  height: 44px;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  border-radius: 0.75rem;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 1.125rem;
}

.section-icon.options {
  background: linear-gradient(135deg, #8b5cf6 0%, #7c3aed 100%);
}

.section-icon.settings {
  background: linear-gradient(135deg, #f59e0b 0%, #d97706 100%);
}

.section-title {
  font-size: 1.125rem;
  font-weight: 600;
  color: #111827;
  margin-bottom: 0.125rem;
}

.section-hint {
  font-size: 0.8125rem;
  color: #6b7280;
}

.section-body {
  padding: 1.5rem;
}

/* Form Groups */
.form-group {
  margin-bottom: 1.5rem;
}

.form-group:last-child {
  margin-bottom: 0;
}

.form-label {
  display: block;
  font-size: 0.875rem;
  font-weight: 500;
  color: #374151;
  margin-bottom: 0.5rem;
}

/* Enhanced Input */
.input-wrapper {
  position: relative;
}

.form-input-enhanced {
  width: 100%;
  padding: 0.875rem 1rem;
  padding-right: 3rem;
  border: 2px solid #e5e7eb;
  border-radius: 0.75rem;
  font-size: 1rem;
  color: #111827;
  background: white;
  transition: all 0.2s ease;
}

.form-input-enhanced:focus {
  outline: none;
  border-color: #14b8a6;
  box-shadow: 0 0 0 3px rgba(20, 184, 166, 0.1);
}

.form-input-enhanced::placeholder {
  color: #9ca3af;
}

.input-ai-btn {
  position: absolute;
  right: 0.5rem;
  top: 50%;
  transform: translateY(-50%);
  width: 36px;
  height: 36px;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  border: none;
  border-radius: 0.5rem;
  color: white;
  cursor: pointer;
  transition: all 0.2s ease;
}

.input-ai-btn:hover {
  transform: translateY(-50%) scale(1.05);
}

/* Textarea */
.form-textarea {
  width: 100%;
  padding: 0.875rem 1rem;
  border: 2px solid #e5e7eb;
  border-radius: 0.75rem;
  font-size: 0.9375rem;
  color: #111827;
  background: white;
  resize: vertical;
  transition: all 0.2s ease;
}

.form-textarea:focus {
  outline: none;
  border-color: #14b8a6;
  box-shadow: 0 0 0 3px rgba(20, 184, 166, 0.1);
}

/* Category Grid */
.category-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 0.75rem;
}

.category-btn {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.5rem;
  padding: 1rem;
  background: #f9fafb;
  border: 2px solid #e5e7eb;
  border-radius: 0.75rem;
  cursor: pointer;
  transition: all 0.2s ease;
}

.category-btn i {
  font-size: 1.25rem;
  color: #6b7280;
}

.category-btn span {
  font-size: 0.8125rem;
  font-weight: 500;
  color: #374151;
}

.category-btn:hover {
  border-color: #14b8a6;
  background: #f0fdfa;
}

.category-btn.active {
  border-color: #14b8a6;
  background: #f0fdfa;
}

.category-btn.active i {
  color: #14b8a6;
}

/* Options List */
.options-list {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.option-item {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.option-number {
  width: 32px;
  height: 32px;
  background: linear-gradient(135deg, #8b5cf6 0%, #7c3aed 100%);
  border-radius: 0.5rem;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.875rem;
  font-weight: 600;
  color: white;
  flex-shrink: 0;
}

.option-input {
  flex: 1;
  padding: 0.75rem 1rem;
  border: 2px solid #e5e7eb;
  border-radius: 0.75rem;
  font-size: 0.9375rem;
  color: #111827;
  transition: all 0.2s ease;
}

.option-input:focus {
  outline: none;
  border-color: #8b5cf6;
  box-shadow: 0 0 0 3px rgba(139, 92, 246, 0.1);
}

.option-remove {
  width: 36px;
  height: 36px;
  background: #fef2f2;
  border: none;
  border-radius: 0.5rem;
  color: #ef4444;
  cursor: pointer;
  transition: all 0.2s ease;
}

.option-remove:hover {
  background: #fee2e2;
}

.add-option-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  width: 100%;
  padding: 0.75rem;
  margin-top: 0.75rem;
  background: #f9fafb;
  border: 2px dashed #d1d5db;
  border-radius: 0.75rem;
  font-size: 0.875rem;
  font-weight: 500;
  color: #6b7280;
  cursor: pointer;
  transition: all 0.2s ease;
}

.add-option-btn:hover {
  border-color: #8b5cf6;
  color: #8b5cf6;
  background: #faf5ff;
}

.options-max-hint {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-top: 0.75rem;
  padding: 0.75rem;
  background: #fef3c7;
  border-radius: 0.75rem;
  font-size: 0.8125rem;
  color: #92400e;
}

/* Duration Presets */
.duration-presets {
  display: flex;
  gap: 0.5rem;
  flex-wrap: wrap;
}

.duration-btn {
  padding: 0.625rem 1rem;
  background: #f9fafb;
  border: 2px solid #e5e7eb;
  border-radius: 0.625rem;
  font-size: 0.875rem;
  font-weight: 500;
  color: #374151;
  cursor: pointer;
  transition: all 0.2s ease;
}

.duration-btn:hover {
  border-color: #f59e0b;
}

.duration-btn.active {
  border-color: #f59e0b;
  background: #fffbeb;
  color: #d97706;
}

.custom-date-wrapper {
  margin-top: 0.75rem;
}

/* Radio Group */
.radio-group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.radio-option {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.75rem 1rem;
  background: #f9fafb;
  border: 2px solid #e5e7eb;
  border-radius: 0.75rem;
  cursor: pointer;
  transition: all 0.2s ease;
}

.radio-option:hover {
  border-color: #d1d5db;
}

.radio-option input[type="radio"] {
  width: 18px;
  height: 18px;
  accent-color: #14b8a6;
}

.radio-option input[type="radio"]:checked + .radio-label {
  color: #0f766e;
  font-weight: 500;
}

.radio-label {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.9375rem;
  color: #374151;
}

.radio-label i {
  width: 20px;
  color: #6b7280;
}

/* Checkbox Group */
.checkbox-group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.checkbox-option {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.75rem 1rem;
  background: #f9fafb;
  border: 2px solid #e5e7eb;
  border-radius: 0.75rem;
  cursor: pointer;
  transition: all 0.2s ease;
}

.checkbox-option:hover {
  border-color: #d1d5db;
}

.checkbox-option input[type="checkbox"] {
  width: 18px;
  height: 18px;
  accent-color: #14b8a6;
}

.checkbox-label {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.9375rem;
  color: #374151;
}

.checkbox-label i {
  width: 20px;
  color: #6b7280;
}

/* Preview Card */
.preview-card {
  background: white;
  border-radius: 1.5rem;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
  border: 1px solid #e5e7eb;
  overflow: hidden;
}

.preview-header {
  padding: 1rem 1.25rem;
  background: linear-gradient(135deg, #f9fafb 0%, #f3f4f6 100%);
  border-bottom: 1px solid #e5e7eb;
}

.preview-badge {
  display: inline-flex;
  align-items: center;
  gap: 0.375rem;
  padding: 0.375rem 0.75rem;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  border-radius: 2rem;
  font-size: 0.75rem;
  font-weight: 600;
  color: white;
}

.preview-content {
  padding: 1.25rem;
}

.preview-category {
  display: inline-flex;
  align-items: center;
  gap: 0.375rem;
  padding: 0.375rem 0.75rem;
  background: #f3f4f6;
  border-radius: 0.5rem;
  font-size: 0.75rem;
  font-weight: 500;
  color: #6b7280;
  margin-bottom: 0.75rem;
}

.preview-question {
  font-size: 1rem;
  font-weight: 600;
  color: #111827;
  margin-bottom: 0.5rem;
  line-height: 1.4;
}

.preview-description {
  font-size: 0.8125rem;
  color: #6b7280;
  margin-bottom: 1rem;
}

.preview-options {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  margin-bottom: 1rem;
}

.preview-option {
  display: flex;
  align-items: center;
  gap: 0.625rem;
  padding: 0.625rem 0.875rem;
  background: #f9fafb;
  border: 1px solid #e5e7eb;
  border-radius: 0.5rem;
  font-size: 0.8125rem;
  color: #374151;
  transition: all 0.2s ease;
}

.preview-option.empty {
  color: #9ca3af;
  font-style: italic;
}

.preview-radio {
  width: 16px;
  height: 16px;
  border: 2px solid #d1d5db;
  border-radius: 50%;
  flex-shrink: 0;
}

.preview-footer {
  display: flex;
  justify-content: space-between;
  padding-top: 0.75rem;
  border-top: 1px solid #e5e7eb;
  font-size: 0.75rem;
  color: #6b7280;
}

.preview-votes,
.preview-ends {
  display: flex;
  align-items: center;
  gap: 0.375rem;
}

.preview-badges {
  display: flex;
  gap: 0.5rem;
  margin-top: 0.75rem;
}

.badge-anon,
.badge-multi {
  display: inline-flex;
  align-items: center;
  gap: 0.25rem;
  padding: 0.25rem 0.5rem;
  border-radius: 0.375rem;
  font-size: 0.6875rem;
  font-weight: 500;
}

.badge-anon {
  background: #f3e8ff;
  color: #7c3aed;
}

.badge-multi {
  background: #dbeafe;
  color: #2563eb;
}

/* Action Buttons */
.action-buttons {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.action-buttons-mobile {
  display: flex;
  gap: 0.75rem;
  padding: 1.5rem;
  background: white;
  border-radius: 1.5rem;
  border: 1px solid #e5e7eb;
}

.btn-secondary {
  padding: 0.75rem 1.25rem;
  background: #f3f4f6;
  border: none;
  border-radius: 0.75rem;
  font-size: 0.9375rem;
  font-weight: 500;
  color: #4b5563;
  cursor: pointer;
  transition: all 0.2s ease;
}

.btn-secondary:hover {
  background: #e5e7eb;
}

.btn-outline {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  padding: 0.75rem 1.25rem;
  background: white;
  border: 2px solid #e5e7eb;
  border-radius: 0.75rem;
  font-size: 0.9375rem;
  font-weight: 500;
  color: #374151;
  cursor: pointer;
  transition: all 0.2s ease;
}

.btn-outline:hover {
  border-color: #14b8a6;
  color: #14b8a6;
}

.btn-primary {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  padding: 0.875rem 1.25rem;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  border: none;
  border-radius: 0.75rem;
  font-size: 0.9375rem;
  font-weight: 600;
  color: white;
  cursor: pointer;
  transition: all 0.2s ease;
}

.btn-primary:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.3);
}

.btn-primary:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

/* Tips Card */
.tips-card {
  background: linear-gradient(135deg, #fffbeb 0%, #fef3c7 100%);
  border: 1px solid #fcd34d;
  border-radius: 1rem;
  padding: 1.25rem;
}

.tips-title {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.9375rem;
  font-weight: 600;
  color: #92400e;
  margin-bottom: 0.75rem;
}

.tips-list {
  list-style: none;
  padding: 0;
  margin: 0;
}

.tips-list li {
  position: relative;
  padding-left: 1.25rem;
  margin-bottom: 0.5rem;
  font-size: 0.8125rem;
  color: #78350f;
}

.tips-list li::before {
  content: '•';
  position: absolute;
  left: 0;
  color: #f59e0b;
}

.tips-list li:last-child {
  margin-bottom: 0;
}

/* Modal Overlay */
.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 50;
  padding: 1rem;
  backdrop-filter: blur(4px);
}

/* AI Modal */
.ai-modal {
  background: white;
  border-radius: 1.5rem;
  box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.25);
  width: 100%;
  max-width: 700px;
  max-height: 85vh;
  overflow: hidden;
  animation: modalSlideIn 0.3s ease-out;
}

@keyframes modalSlideIn {
  from {
    opacity: 0;
    transform: translateY(-20px) scale(0.95);
  }
  to {
    opacity: 1;
    transform: translateY(0) scale(1);
  }
}

.ai-modal-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 1.25rem 1.5rem;
  border-bottom: 1px solid #e5e7eb;
}

.ai-modal-title-group {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.ai-modal-icon {
  width: 48px;
  height: 48px;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  border-radius: 0.75rem;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 1.25rem;
}

.ai-modal-title {
  font-size: 1.125rem;
  font-weight: 600;
  color: #111827;
}

.ai-modal-subtitle {
  font-size: 0.8125rem;
  color: #6b7280;
}

.modal-close {
  width: 36px;
  height: 36px;
  background: #f3f4f6;
  border: none;
  border-radius: 0.5rem;
  color: #6b7280;
  cursor: pointer;
  transition: all 0.2s ease;
}

.modal-close:hover {
  background: #e5e7eb;
  color: #374151;
}

/* AI Tabs */
.ai-modal-tabs {
  display: flex;
  gap: 0.5rem;
  padding: 1rem 1.5rem;
  border-bottom: 1px solid #e5e7eb;
  overflow-x: auto;
}

.ai-tab {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  padding: 0.5rem 1rem;
  background: #f3f4f6;
  border: none;
  border-radius: 0.5rem;
  font-size: 0.8125rem;
  font-weight: 500;
  color: #6b7280;
  cursor: pointer;
  white-space: nowrap;
  transition: all 0.2s ease;
}

.ai-tab:hover {
  background: #e5e7eb;
}

.ai-tab.active {
  background: #f0fdfa;
  color: #0f766e;
}

/* AI Modal Body */
.ai-modal-body {
  padding: 1.5rem;
  max-height: 50vh;
  overflow-y: auto;
}

.ai-suggestions-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.ai-suggestion-card {
  padding: 1rem;
  border: 2px solid #e5e7eb;
  border-radius: 0.75rem;
  cursor: pointer;
  transition: all 0.2s ease;
}

.ai-suggestion-card:hover {
  border-color: #14b8a6;
  background: #f0fdfa;
}

.suggestion-header {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: 1rem;
  margin-bottom: 0.75rem;
}

.suggestion-question {
  font-size: 0.9375rem;
  font-weight: 600;
  color: #111827;
  flex: 1;
}

.suggestion-purpose {
  padding: 0.25rem 0.625rem;
  background: #f0fdfa;
  border-radius: 2rem;
  font-size: 0.6875rem;
  font-weight: 500;
  color: #0f766e;
  white-space: nowrap;
}

.suggestion-options {
  display: flex;
  flex-wrap: wrap;
  gap: 0.375rem;
}

.suggestion-option {
  padding: 0.25rem 0.625rem;
  background: #f3f4f6;
  border-radius: 0.375rem;
  font-size: 0.75rem;
  color: #4b5563;
}

.ai-empty-state {
  text-align: center;
  padding: 3rem 1rem;
  color: #9ca3af;
}

.ai-empty-state i {
  font-size: 3rem;
  margin-bottom: 1rem;
  opacity: 0.5;
}

/* AI Modal Footer */
.ai-modal-footer {
  display: flex;
  justify-content: flex-end;
  gap: 0.75rem;
  padding: 1rem 1.5rem;
  border-top: 1px solid #e5e7eb;
}

.btn-refresh {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  padding: 0.625rem 1rem;
  background: transparent;
  border: none;
  border-radius: 0.5rem;
  font-size: 0.875rem;
  font-weight: 500;
  color: #14b8a6;
  cursor: pointer;
  transition: all 0.2s ease;
}

.btn-refresh:hover {
  background: #f0fdfa;
}

.btn-cancel {
  padding: 0.625rem 1rem;
  background: #f3f4f6;
  border: none;
  border-radius: 0.5rem;
  font-size: 0.875rem;
  font-weight: 500;
  color: #4b5563;
  cursor: pointer;
  transition: all 0.2s ease;
}

.btn-cancel:hover {
  background: #e5e7eb;
}

/* Enhanced AI Modal */
.ai-modal-enhanced {
  max-width: 800px;
  max-height: 90vh;
}

/* AI Mode Tabs */
.ai-mode-tabs {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 0.75rem;
  padding: 1rem 1.5rem;
  border-bottom: 1px solid #e5e7eb;
  background: #f9fafb;
}

.ai-mode-tab {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.375rem;
  padding: 1rem;
  background: white;
  border: 2px solid #e5e7eb;
  border-radius: 0.75rem;
  cursor: pointer;
  transition: all 0.2s ease;
}

.ai-mode-tab:hover {
  border-color: #14b8a6;
}

.ai-mode-tab.active {
  border-color: #14b8a6;
  background: #f0fdfa;
}

.ai-mode-tab i {
  font-size: 1.25rem;
  color: #6b7280;
}

.ai-mode-tab.active i {
  color: #14b8a6;
}

.ai-mode-tab .mode-label {
  font-size: 0.875rem;
  font-weight: 600;
  color: #111827;
}

.ai-mode-tab .mode-desc {
  font-size: 0.6875rem;
  color: #6b7280;
  text-align: center;
}

/* Category Tabs in Modal */
.ai-category-tabs {
  display: flex;
  gap: 0.5rem;
  margin-bottom: 1rem;
  padding-bottom: 1rem;
  border-bottom: 1px solid #e5e7eb;
  overflow-x: auto;
}

/* Mode Content */
.mode-content {
  animation: fadeIn 0.3s ease;
}

/* Topic Input Section */
.topic-input-section {
  margin-bottom: 1.5rem;
}

.input-label {
  display: block;
  font-size: 0.9375rem;
  font-weight: 600;
  color: #111827;
  margin-bottom: 0.75rem;
}

.topic-input-wrapper {
  position: relative;
}

.topic-icon {
  position: absolute;
  left: 1rem;
  top: 50%;
  transform: translateY(-50%);
  font-size: 1.125rem;
  color: #f59e0b;
}

.topic-input {
  width: 100%;
  padding: 1rem 1rem 1rem 3rem;
  border: 2px solid #e5e7eb;
  border-radius: 0.75rem;
  font-size: 1rem;
  color: #111827;
  transition: all 0.2s ease;
}

.topic-input:focus {
  outline: none;
  border-color: #14b8a6;
  box-shadow: 0 0 0 3px rgba(20, 184, 166, 0.1);
}

.topic-input::placeholder {
  color: #9ca3af;
}

.input-hint {
  margin-top: 0.5rem;
  font-size: 0.8125rem;
  color: #6b7280;
}

/* Content Input Section */
.content-input-section {
  margin-bottom: 1.5rem;
}

.content-input-wrapper {
  position: relative;
}

.content-input {
  width: 100%;
  padding: 1rem;
  border: 2px solid #e5e7eb;
  border-radius: 0.75rem;
  font-size: 0.9375rem;
  color: #111827;
  line-height: 1.6;
  resize: vertical;
  min-height: 150px;
  transition: all 0.2s ease;
}

.content-input:focus {
  outline: none;
  border-color: #14b8a6;
  box-shadow: 0 0 0 3px rgba(20, 184, 166, 0.1);
}

.content-input::placeholder {
  color: #9ca3af;
}

.content-footer {
  display: flex;
  justify-content: flex-end;
  margin-top: 0.5rem;
}

.char-count {
  font-size: 0.75rem;
  color: #9ca3af;
}

/* Generate Button */
.generate-btn {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem 1.5rem;
  margin-top: 1rem;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  border: none;
  border-radius: 0.75rem;
  font-size: 0.9375rem;
  font-weight: 600;
  color: white;
  cursor: pointer;
  transition: all 0.2s ease;
}

.generate-btn:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.3);
}

.generate-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

/* Results Header */
.results-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0.75rem 1rem;
  background: linear-gradient(135deg, #f0fdfa 0%, #ccfbf1 100%);
  border-radius: 0.75rem;
  margin-bottom: 1rem;
}

.results-label {
  font-size: 0.875rem;
  font-weight: 600;
  color: #0f766e;
}

.results-source {
  font-size: 0.75rem;
  color: #14b8a6;
  font-style: italic;
}

/* Suggestion Meta */
.suggestion-meta {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  flex-shrink: 0;
}

.suggestion-confidence {
  display: inline-flex;
  align-items: center;
  gap: 0.25rem;
  padding: 0.25rem 0.5rem;
  background: #dbeafe;
  border-radius: 2rem;
  font-size: 0.6875rem;
  font-weight: 500;
  color: #1d4ed8;
}

/* Source Highlight */
.source-highlight {
  display: flex;
  align-items: flex-start;
  gap: 0.5rem;
  padding: 0.625rem 0.875rem;
  margin-bottom: 0.75rem;
  background: #fffbeb;
  border-radius: 0.5rem;
  border-left: 3px solid #f59e0b;
}

.source-highlight i {
  color: #f59e0b;
  font-size: 0.75rem;
  margin-top: 0.125rem;
  flex-shrink: 0;
}

.source-highlight span {
  font-size: 0.8125rem;
  color: #78350f;
  font-style: italic;
  line-height: 1.4;
}

/* Responsive */
@media (max-width: 768px) {
  .header-content {
    padding: 1rem 1.5rem 1.5rem;
  }

  .header-main {
    flex-direction: column;
    align-items: flex-start;
    gap: 1rem;
  }

  .header-title {
    font-size: 1.375rem;
  }

  .ai-generate-btn {
    width: 100%;
    justify-content: center;
  }

  .category-grid {
    grid-template-columns: repeat(2, 1fr);
  }

  .duration-presets {
    flex-wrap: wrap;
  }

  .action-buttons-mobile {
    flex-direction: column;
  }

  .ai-mode-tabs {
    grid-template-columns: 1fr;
    gap: 0.5rem;
  }

  .ai-mode-tab {
    flex-direction: row;
    justify-content: flex-start;
    gap: 0.75rem;
    padding: 0.75rem 1rem;
  }

  .ai-mode-tab .mode-desc {
    display: none;
  }

  .results-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 0.25rem;
  }
}
</style>
