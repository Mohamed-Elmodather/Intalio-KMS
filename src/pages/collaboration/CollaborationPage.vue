<script setup lang="ts">
import { ref, computed, nextTick, watch } from 'vue'
import { useAIServicesStore } from '@/stores/aiServices'
import { AILoadingIndicator, AISuggestionChip, AISentimentBadge } from '@/components/ai'
import type { SentimentResult, SummarizationResult } from '@/types/ai'

const aiStore = useAIServicesStore()

// Types
type UserPresenceStatus = 'online' | 'away' | 'busy' | 'offline'
type ChannelType = 'public' | 'private' | 'direct' | 'group'
type MessageType = 'text' | 'file' | 'image' | 'system' | 'event' | 'article' | 'document'

interface ChannelMember {
  id: string
  displayName: string
  avatar?: string
  role?: string
  presence: UserPresenceStatus
}

interface Channel {
  id: string
  name: string
  description?: string
  type: ChannelType
  icon?: string
  memberCount: number
  unreadCount: number
  isMuted: boolean
  isPinned: boolean
  members?: ChannelMember[]
}

interface DirectMessage {
  id: string
  user: {
    id: string
    displayName: string
    avatar?: string
    role?: string
  }
  presence: UserPresenceStatus
  unreadCount: number
  lastMessage?: string
  lastMessageTime?: string
}

interface MessageAttachment {
  id: string
  name: string
  type: 'file' | 'image' | 'video' | 'audio'
  url: string
  thumbnailUrl?: string
  size: number
}

interface MessageReaction {
  emoji: string
  count: number
  users: string[]
  hasReacted: boolean
}

interface LinkedContent {
  type: 'event' | 'article' | 'document' | 'poll'
  id: string
  title: string
  description?: string
  thumbnail?: string
  date?: string
  url: string
}

interface Message {
  id: string
  channelId: string
  sender: {
    id: string
    displayName: string
    avatar?: string
  }
  content: string
  type: MessageType
  attachments?: MessageAttachment[]
  reactions?: MessageReaction[]
  mentions?: string[]
  threadId?: string
  threadCount?: number
  threadLastReply?: string
  isEdited: boolean
  isPinned: boolean
  createdAt: string
  linkedContent?: LinkedContent
}

interface CollaborationTeam {
  id: string
  name: string
  avatar?: string
  color: string
  channels: Channel[]
  memberCount: number
}

// State
const selectedChannelId = ref<string>('1')
const selectedDMId = ref<string | null>(null)
const messageInput = ref('')
const searchQuery = ref('')
const showRightPanel = ref(true)
const leftPanelCollapsed = ref(false)
const rightPanelCollapsed = ref(false)
const showCreateChannelModal = ref(false)
const showContentPicker = ref(false)
const showEmojiPicker = ref(false)
const activeEmojiMessageId = ref<string | null>(null)
const showMentionAutocomplete = ref(false)
const mentionQuery = ref('')
const expandedThreadId = ref<string | null>(null)
const threadReplyInput = ref('')
const rightPanelTab = ref<'details' | 'members' | 'files' | 'pinned' | 'profile' | 'media'>('details')
const isTyping = ref(false)
const typingUsers = ref<string[]>([])
const messagesContainerRef = ref<HTMLElement | null>(null)
const expandedTeamIds = ref<Set<string>>(new Set())

// New channel form
const newChannel = ref({
  name: '',
  description: '',
  type: 'public' as ChannelType,
  isPrivate: false
})

// Mock Data
const channels = ref<Channel[]>([
  { id: '1', name: 'general', description: 'General discussions for everyone', type: 'public', memberCount: 45, unreadCount: 3, isMuted: false, isPinned: true },
  { id: '2', name: 'announcements', description: 'Official announcements', type: 'public', memberCount: 120, unreadCount: 0, isMuted: false, isPinned: true },
  { id: '3', name: 'tournament-planning', description: 'AFC Asian Cup 2027 planning', type: 'private', memberCount: 12, unreadCount: 5, isMuted: false, isPinned: false },
  { id: '4', name: 'media-team', description: 'Media and communications', type: 'private', memberCount: 8, unreadCount: 0, isMuted: true, isPinned: false },
  { id: '5', name: 'volunteers', description: 'Volunteer coordination', type: 'public', memberCount: 67, unreadCount: 12, isMuted: false, isPinned: false },
  { id: '6', name: 'tech-support', description: 'Technical support and IT', type: 'public', memberCount: 15, unreadCount: 0, isMuted: false, isPinned: false },
])

const directMessages = ref<DirectMessage[]>([
  { id: 'dm1', user: { id: 'u1', displayName: 'Sarah Ahmed', avatar: '', role: 'Project Manager' }, presence: 'online', unreadCount: 2, lastMessage: 'Can we discuss the schedule?', lastMessageTime: '10:30 AM' },
  { id: 'dm2', user: { id: 'u2', displayName: 'Mohammed Hassan', avatar: '', role: 'Content Lead' }, presence: 'away', unreadCount: 0, lastMessage: 'Documents uploaded', lastMessageTime: '9:15 AM' },
  { id: 'dm3', user: { id: 'u3', displayName: 'Fatima Al-Rashid', avatar: '', role: 'Designer' }, presence: 'online', unreadCount: 0, lastMessage: 'Design approved!', lastMessageTime: 'Yesterday' },
  { id: 'dm4', user: { id: 'u4', displayName: 'Ahmed Khalil', avatar: '', role: 'Developer' }, presence: 'busy', unreadCount: 1, lastMessage: 'API is ready', lastMessageTime: 'Yesterday' },
  { id: 'dm5', user: { id: 'u5', displayName: 'Layla Omar', avatar: '', role: 'Marketing' }, presence: 'offline', unreadCount: 0, lastMessage: 'See you tomorrow!', lastMessageTime: 'Monday' },
])

const teams = ref<CollaborationTeam[]>([
  {
    id: 't1',
    name: 'AFC 2027 Core',
    avatar: '',
    color: '#0d9488',
    memberCount: 24,
    channels: [
      { id: 't1-general', name: 'general', type: 'public', memberCount: 24, unreadCount: 2, isMuted: false, isPinned: false },
      { id: 't1-leadership', name: 'leadership', type: 'private', memberCount: 6, unreadCount: 0, isMuted: false, isPinned: false },
      { id: 't1-updates', name: 'updates', type: 'public', memberCount: 24, unreadCount: 5, isMuted: false, isPinned: false },
    ]
  },
  {
    id: 't2',
    name: 'Media & Press',
    avatar: '',
    color: '#8b5cf6',
    memberCount: 12,
    channels: [
      { id: 't2-general', name: 'general', type: 'public', memberCount: 12, unreadCount: 0, isMuted: false, isPinned: false },
      { id: 't2-content', name: 'content-review', type: 'private', memberCount: 8, unreadCount: 3, isMuted: false, isPinned: false },
      { id: 't2-social', name: 'social-media', type: 'public', memberCount: 12, unreadCount: 1, isMuted: false, isPinned: false },
    ]
  },
  {
    id: 't3',
    name: 'Operations',
    avatar: '',
    color: '#f59e0b',
    memberCount: 18,
    channels: [
      { id: 't3-general', name: 'general', type: 'public', memberCount: 18, unreadCount: 0, isMuted: false, isPinned: false },
      { id: 't3-logistics', name: 'logistics', type: 'private', memberCount: 10, unreadCount: 4, isMuted: false, isPinned: false },
      { id: 't3-venues', name: 'venues', type: 'public', memberCount: 15, unreadCount: 0, isMuted: false, isPinned: false },
    ]
  },
])

const channelMembers = ref<ChannelMember[]>([
  { id: 'u1', displayName: 'Sarah Ahmed', role: 'Project Manager', presence: 'online' },
  { id: 'u2', displayName: 'Mohammed Hassan', role: 'Content Lead', presence: 'away' },
  { id: 'u3', displayName: 'Fatima Al-Rashid', role: 'Designer', presence: 'online' },
  { id: 'u4', displayName: 'Ahmed Khalil', role: 'Developer', presence: 'busy' },
  { id: 'u5', displayName: 'Layla Omar', role: 'Marketing', presence: 'offline' },
  { id: 'u6', displayName: 'Omar Farid', role: 'Coordinator', presence: 'online' },
  { id: 'u7', displayName: 'Noor Salim', role: 'Analyst', presence: 'online' },
  { id: 'u8', displayName: 'Yusuf Ali', role: 'Support', presence: 'away' },
])

const messages = ref<Map<string, Message[]>>(new Map([
  ['1', [
    {
      id: 'm1',
      channelId: '1',
      sender: { id: 'u1', displayName: 'Sarah Ahmed' },
      content: 'Good morning everyone! Hope you all had a great weekend.',
      type: 'text',
      reactions: [
        { emoji: '👋', count: 5, users: ['u2', 'u3', 'u4', 'u5', 'u6'], hasReacted: false },
        { emoji: '☕', count: 3, users: ['u2', 'u7', 'u8'], hasReacted: true }
      ],
      isEdited: false,
      isPinned: false,
      createdAt: '2027-01-11T08:00:00Z'
    },
    {
      id: 'm2',
      channelId: '1',
      sender: { id: 'u2', displayName: 'Mohammed Hassan' },
      content: 'Morning! Quick reminder - we have the tournament planning meeting at 2 PM today.',
      type: 'text',
      reactions: [{ emoji: '👍', count: 8, users: ['u1', 'u3', 'u4', 'u5', 'u6', 'u7', 'u8', 'u9'], hasReacted: true }],
      threadCount: 3,
      threadLastReply: 'Will be there!',
      isEdited: false,
      isPinned: true,
      createdAt: '2027-01-11T08:15:00Z'
    },
    {
      id: 'm3',
      channelId: '1',
      sender: { id: 'u3', displayName: 'Fatima Al-Rashid' },
      content: 'I\'ve uploaded the new venue designs to the shared folder. Please review when you have a chance @Sarah Ahmed @Mohammed Hassan',
      type: 'text',
      mentions: ['u1', 'u2'],
      attachments: [
        { id: 'a1', name: 'venue-designs-v3.pdf', type: 'file', url: '#', size: 4500000 },
        { id: 'a2', name: 'stadium-layout.png', type: 'image', url: 'https://picsum.photos/400/300', thumbnailUrl: 'https://picsum.photos/100/75', size: 1200000 }
      ],
      reactions: [{ emoji: '🎨', count: 4, users: ['u1', 'u2', 'u5', 'u6'], hasReacted: false }],
      isEdited: false,
      isPinned: false,
      createdAt: '2027-01-11T09:30:00Z'
    },
    {
      id: 'm4',
      channelId: '1',
      sender: { id: 'system', displayName: 'System' },
      content: 'Sarah Ahmed shared an event',
      type: 'event',
      linkedContent: {
        type: 'event',
        id: 'e1',
        title: 'AFC Asian Cup 2027 Opening Ceremony Planning',
        description: 'Final planning meeting for the opening ceremony. All team leads required.',
        date: 'Jan 15, 2027 • 2:00 PM',
        thumbnail: 'https://picsum.photos/400/200',
        url: '/events/e1'
      },
      isEdited: false,
      isPinned: false,
      createdAt: '2027-01-11T10:00:00Z'
    },
    {
      id: 'm5',
      channelId: '1',
      sender: { id: 'u4', displayName: 'Ahmed Khalil' },
      content: 'The new ticketing API is now live! Documentation has been updated.',
      type: 'text',
      linkedContent: {
        type: 'document',
        id: 'd1',
        title: 'Ticketing API Documentation v2.0',
        description: 'Complete API reference for the ticketing system integration',
        url: '/documents/d1'
      },
      reactions: [
        { emoji: '🚀', count: 6, users: ['u1', 'u2', 'u3', 'u5', 'u6', 'u7'], hasReacted: false },
        { emoji: '🎉', count: 4, users: ['u1', 'u3', 'u5', 'u8'], hasReacted: true }
      ],
      isEdited: false,
      isPinned: false,
      createdAt: '2027-01-11T11:45:00Z'
    },
    {
      id: 'm6',
      channelId: '1',
      sender: { id: 'u5', displayName: 'Layla Omar' },
      content: 'Great work team! Here\'s the latest article about our progress for internal review:',
      type: 'text',
      linkedContent: {
        type: 'article',
        id: 'art1',
        title: 'AFC Asian Cup 2027: Six Months to Go',
        description: 'An inside look at the preparations for the biggest football event in Asia',
        thumbnail: 'https://picsum.photos/400/250',
        url: '/articles/art1'
      },
      isEdited: false,
      isPinned: false,
      createdAt: '2027-01-11T14:20:00Z'
    }
  ]],
  ['3', [
    {
      id: 'tm1',
      channelId: '3',
      sender: { id: 'u1', displayName: 'Sarah Ahmed' },
      content: 'Team, let\'s finalize the group stage schedule by end of week.',
      type: 'text',
      reactions: [{ emoji: '✅', count: 5, users: ['u2', 'u3', 'u4', 'u6', 'u7'], hasReacted: false }],
      threadCount: 7,
      threadLastReply: 'Schedule looks good to me',
      isEdited: false,
      isPinned: true,
      createdAt: '2027-01-10T09:00:00Z'
    }
  ]],
  // AFC 2027 Core Team Channels
  ['t1-general', [
    {
      id: 't1g1',
      channelId: 't1-general',
      sender: { id: 'u1', displayName: 'Sarah Ahmed' },
      content: 'Welcome to the AFC 2027 Core team! This is our main channel for team-wide discussions.',
      type: 'text',
      reactions: [{ emoji: '👋', count: 12, users: ['u2', 'u3', 'u4', 'u5', 'u6', 'u7', 'u8'], hasReacted: true }],
      isEdited: false,
      isPinned: true,
      createdAt: '2027-01-08T09:00:00Z'
    },
    {
      id: 't1g2',
      channelId: 't1-general',
      sender: { id: 'u2', displayName: 'Mohammed Hassan' },
      content: 'Great to be part of this team! Looking forward to making this event a success.',
      type: 'text',
      reactions: [{ emoji: '🙌', count: 8, users: ['u1', 'u3', 'u5', 'u6'], hasReacted: false }],
      isEdited: false,
      isPinned: false,
      createdAt: '2027-01-08T09:15:00Z'
    }
  ]],
  ['t1-leadership', [
    {
      id: 't1l1',
      channelId: 't1-leadership',
      sender: { id: 'u1', displayName: 'Sarah Ahmed' },
      content: 'Leadership sync: Let\'s discuss the budget allocation for Q1.',
      type: 'text',
      isEdited: false,
      isPinned: false,
      createdAt: '2027-01-10T10:00:00Z'
    }
  ]],
  ['t1-updates', [
    {
      id: 't1u1',
      channelId: 't1-updates',
      sender: { id: 'u1', displayName: 'Sarah Ahmed' },
      content: '📢 Weekly Update: All venue contracts have been finalized. Next milestone is equipment procurement.',
      type: 'text',
      reactions: [{ emoji: '🎉', count: 15, users: ['u2', 'u3', 'u4', 'u5', 'u6', 'u7', 'u8'], hasReacted: true }],
      isEdited: false,
      isPinned: true,
      createdAt: '2027-01-10T08:00:00Z'
    },
    {
      id: 't1u2',
      channelId: 't1-updates',
      sender: { id: 'u4', displayName: 'Ahmed Khalil' },
      content: '🔧 Tech Update: Mobile app beta testing starts next week. Please sign up if you want to participate.',
      type: 'text',
      reactions: [{ emoji: '📱', count: 10, users: ['u1', 'u2', 'u3', 'u5', 'u6'], hasReacted: false }],
      isEdited: false,
      isPinned: false,
      createdAt: '2027-01-11T09:00:00Z'
    }
  ]],
  // Media & Press Team Channels
  ['t2-general', [
    {
      id: 't2g1',
      channelId: 't2-general',
      sender: { id: 'u5', displayName: 'Layla Omar' },
      content: 'Media team huddle at 3 PM today. We\'ll review the press release schedule.',
      type: 'text',
      reactions: [{ emoji: '👍', count: 6, users: ['u1', 'u2', 'u3'], hasReacted: true }],
      isEdited: false,
      isPinned: false,
      createdAt: '2027-01-11T11:00:00Z'
    }
  ]],
  ['t2-content', [
    {
      id: 't2c1',
      channelId: 't2-content',
      sender: { id: 'u3', displayName: 'Fatima Al-Rashid' },
      content: 'New brand assets are ready for review. Check the shared folder for the updated logo pack.',
      type: 'text',
      attachments: [
        { id: 't2a1', name: 'AFC2027-BrandAssets-v2.zip', type: 'file', url: '#', size: 25000000 }
      ],
      isEdited: false,
      isPinned: false,
      createdAt: '2027-01-11T14:00:00Z'
    }
  ]],
  ['t2-social', [
    {
      id: 't2s1',
      channelId: 't2-social',
      sender: { id: 'u5', displayName: 'Layla Omar' },
      content: 'Social media calendar for January is live! Please review your assigned posts.',
      type: 'text',
      linkedContent: {
        type: 'document',
        id: 'doc-social',
        title: 'Social Media Calendar - January 2027',
        description: 'Monthly posting schedule and content themes',
        url: '/documents/social-calendar'
      },
      isEdited: false,
      isPinned: true,
      createdAt: '2027-01-09T10:00:00Z'
    }
  ]],
  // Operations Team Channels
  ['t3-general', [
    {
      id: 't3g1',
      channelId: 't3-general',
      sender: { id: 'u6', displayName: 'Omar Farid' },
      content: 'Operations team standup every morning at 9 AM. Don\'t forget to update your tasks!',
      type: 'text',
      reactions: [{ emoji: '✅', count: 10, users: ['u1', 'u2', 'u3', 'u4', 'u5', 'u7', 'u8'], hasReacted: false }],
      isEdited: false,
      isPinned: true,
      createdAt: '2027-01-08T08:00:00Z'
    }
  ]],
  ['t3-logistics', [
    {
      id: 't3l1',
      channelId: 't3-logistics',
      sender: { id: 'u6', displayName: 'Omar Farid' },
      content: 'Equipment delivery scheduled for next Monday. Please ensure warehouse access is arranged.',
      type: 'text',
      reactions: [{ emoji: '📦', count: 4, users: ['u1', 'u7', 'u8'], hasReacted: true }],
      isEdited: false,
      isPinned: false,
      createdAt: '2027-01-11T15:00:00Z'
    },
    {
      id: 't3l2',
      channelId: 't3-logistics',
      sender: { id: 'u7', displayName: 'Noor Salim' },
      content: 'Warehouse access confirmed. I\'ll be there to receive the delivery.',
      type: 'text',
      reactions: [{ emoji: '👍', count: 2, users: ['u6'], hasReacted: false }],
      isEdited: false,
      isPinned: false,
      createdAt: '2027-01-11T15:30:00Z'
    }
  ]],
  ['t3-venues', [
    {
      id: 't3v1',
      channelId: 't3-venues',
      sender: { id: 'u8', displayName: 'Yusuf Ali' },
      content: 'Site inspection at Stadium A completed. Full report attached.',
      type: 'text',
      attachments: [
        { id: 't3a1', name: 'StadiumA-Inspection-Report.pdf', type: 'file', url: '#', size: 3500000 }
      ],
      reactions: [{ emoji: '📋', count: 5, users: ['u1', 'u6', 'u7'], hasReacted: false }],
      isEdited: false,
      isPinned: false,
      createdAt: '2027-01-11T12:00:00Z'
    }
  ]],
  // Direct Messages
  ['dm1', [
    {
      id: 'dm1-1',
      channelId: 'dm1',
      sender: { id: 'u1', displayName: 'Sarah Ahmed' },
      content: 'Hi! Do you have a minute to chat about the tournament schedule?',
      type: 'text',
      isEdited: false,
      isPinned: false,
      createdAt: '2027-01-11T10:00:00Z'
    },
    {
      id: 'dm1-2',
      channelId: 'dm1',
      sender: { id: 'me', displayName: 'You' },
      content: 'Sure, what do you need?',
      type: 'text',
      isEdited: false,
      isPinned: false,
      createdAt: '2027-01-11T10:05:00Z'
    },
    {
      id: 'dm1-3',
      channelId: 'dm1',
      sender: { id: 'u1', displayName: 'Sarah Ahmed' },
      content: 'I was thinking we should move the opening ceremony rehearsal to Thursday instead of Friday. What do you think?',
      type: 'text',
      isEdited: false,
      isPinned: false,
      createdAt: '2027-01-11T10:10:00Z'
    },
    {
      id: 'dm1-4',
      channelId: 'dm1',
      sender: { id: 'me', displayName: 'You' },
      content: 'That works for me. I\'ll update the calendar.',
      type: 'text',
      isEdited: false,
      isPinned: false,
      createdAt: '2027-01-11T10:15:00Z'
    },
    {
      id: 'dm1-5',
      channelId: 'dm1',
      sender: { id: 'u1', displayName: 'Sarah Ahmed' },
      content: 'Can we discuss the schedule?',
      type: 'text',
      isEdited: false,
      isPinned: false,
      createdAt: '2027-01-11T10:30:00Z'
    }
  ]],
  ['dm2', [
    {
      id: 'dm2-1',
      channelId: 'dm2',
      sender: { id: 'u2', displayName: 'Mohammed Hassan' },
      content: 'Hey, I\'ve finished preparing the content for the media kit.',
      type: 'text',
      isEdited: false,
      isPinned: false,
      createdAt: '2027-01-11T08:30:00Z'
    },
    {
      id: 'dm2-2',
      channelId: 'dm2',
      sender: { id: 'me', displayName: 'You' },
      content: 'Great! Can you upload them to the shared folder?',
      type: 'text',
      isEdited: false,
      isPinned: false,
      createdAt: '2027-01-11T08:45:00Z'
    },
    {
      id: 'dm2-3',
      channelId: 'dm2',
      sender: { id: 'u2', displayName: 'Mohammed Hassan' },
      content: 'Documents uploaded',
      type: 'text',
      attachments: [
        { id: 'dm2a1', name: 'MediaKit-2027.pdf', type: 'file', url: '#', size: 8500000 },
        { id: 'dm2a2', name: 'PressRelease-Draft.docx', type: 'file', url: '#', size: 245000 }
      ],
      isEdited: false,
      isPinned: false,
      createdAt: '2027-01-11T09:15:00Z'
    }
  ]],
  ['dm3', [
    {
      id: 'dm3-1',
      channelId: 'dm3',
      sender: { id: 'me', displayName: 'You' },
      content: 'Hi Fatima, how\'s the new logo design coming along?',
      type: 'text',
      isEdited: false,
      isPinned: false,
      createdAt: '2027-01-10T14:00:00Z'
    },
    {
      id: 'dm3-2',
      channelId: 'dm3',
      sender: { id: 'u3', displayName: 'Fatima Al-Rashid' },
      content: 'Almost done! Here\'s a preview:',
      type: 'text',
      attachments: [
        { id: 'dm3a1', name: 'logo-preview.png', type: 'image', url: 'https://picsum.photos/400/300', thumbnailUrl: 'https://picsum.photos/100/75', size: 850000 }
      ],
      isEdited: false,
      isPinned: false,
      createdAt: '2027-01-10T14:30:00Z'
    },
    {
      id: 'dm3-3',
      channelId: 'dm3',
      sender: { id: 'me', displayName: 'You' },
      content: 'This looks amazing! I love the color scheme.',
      type: 'text',
      reactions: [{ emoji: '❤️', count: 1, users: ['u3'], hasReacted: false }],
      isEdited: false,
      isPinned: false,
      createdAt: '2027-01-10T14:45:00Z'
    },
    {
      id: 'dm3-4',
      channelId: 'dm3',
      sender: { id: 'u3', displayName: 'Fatima Al-Rashid' },
      content: 'Design approved!',
      type: 'text',
      reactions: [{ emoji: '🎉', count: 1, users: ['me'], hasReacted: true }],
      isEdited: false,
      isPinned: false,
      createdAt: '2027-01-10T15:00:00Z'
    }
  ]],
  ['dm4', [
    {
      id: 'dm4-1',
      channelId: 'dm4',
      sender: { id: 'u4', displayName: 'Ahmed Khalil' },
      content: 'Hey, quick update on the ticketing system integration.',
      type: 'text',
      isEdited: false,
      isPinned: false,
      createdAt: '2027-01-10T16:00:00Z'
    },
    {
      id: 'dm4-2',
      channelId: 'dm4',
      sender: { id: 'me', displayName: 'You' },
      content: 'What\'s the status?',
      type: 'text',
      isEdited: false,
      isPinned: false,
      createdAt: '2027-01-10T16:05:00Z'
    },
    {
      id: 'dm4-3',
      channelId: 'dm4',
      sender: { id: 'u4', displayName: 'Ahmed Khalil' },
      content: 'All tests passed! The API endpoints are working perfectly. I\'ve also added rate limiting and caching.',
      type: 'text',
      isEdited: false,
      isPinned: false,
      createdAt: '2027-01-10T16:15:00Z'
    },
    {
      id: 'dm4-4',
      channelId: 'dm4',
      sender: { id: 'u4', displayName: 'Ahmed Khalil' },
      content: 'API is ready',
      type: 'text',
      linkedContent: {
        type: 'document',
        id: 'api-docs',
        title: 'Ticketing API Documentation',
        description: 'Complete API reference with examples',
        url: '/documents/api-docs'
      },
      isEdited: false,
      isPinned: false,
      createdAt: '2027-01-10T16:30:00Z'
    }
  ]],
  ['dm5', [
    {
      id: 'dm5-1',
      channelId: 'dm5',
      sender: { id: 'u5', displayName: 'Layla Omar' },
      content: 'Thanks for the feedback on the marketing campaign!',
      type: 'text',
      isEdited: false,
      isPinned: false,
      createdAt: '2027-01-06T17:00:00Z'
    },
    {
      id: 'dm5-2',
      channelId: 'dm5',
      sender: { id: 'me', displayName: 'You' },
      content: 'No problem! The social media strategy looks solid.',
      type: 'text',
      isEdited: false,
      isPinned: false,
      createdAt: '2027-01-06T17:15:00Z'
    },
    {
      id: 'dm5-3',
      channelId: 'dm5',
      sender: { id: 'u5', displayName: 'Layla Omar' },
      content: 'Great! I\'ll finalize it and share with the team. Have a good evening!',
      type: 'text',
      isEdited: false,
      isPinned: false,
      createdAt: '2027-01-06T17:30:00Z'
    },
    {
      id: 'dm5-4',
      channelId: 'dm5',
      sender: { id: 'me', displayName: 'You' },
      content: 'See you tomorrow!',
      type: 'text',
      reactions: [{ emoji: '👋', count: 1, users: ['u5'], hasReacted: false }],
      isEdited: false,
      isPinned: false,
      createdAt: '2027-01-06T17:35:00Z'
    }
  ]]
]))

const threadMessages = ref<Map<string, Message[]>>(new Map([
  ['m2', [
    { id: 'th1', channelId: '1', sender: { id: 'u3', displayName: 'Fatima Al-Rashid' }, content: 'Will be there!', type: 'text', isEdited: false, isPinned: false, createdAt: '2027-01-11T08:20:00Z' },
    { id: 'th2', channelId: '1', sender: { id: 'u4', displayName: 'Ahmed Khalil' }, content: 'Same here. Should I prepare the API demo?', type: 'text', isEdited: false, isPinned: false, createdAt: '2027-01-11T08:25:00Z' },
    { id: 'th3', channelId: '1', sender: { id: 'u1', displayName: 'Sarah Ahmed' }, content: 'Yes please! That would be great.', type: 'text', reactions: [{ emoji: '👍', count: 1, users: ['u4'], hasReacted: false }], isEdited: false, isPinned: false, createdAt: '2027-01-11T08:30:00Z' }
  ]]
]))

const sharedFiles = ref([
  { id: 'f1', name: 'Tournament Schedule v3.xlsx', type: 'file', size: 245000, uploadedBy: 'Sarah Ahmed', uploadedAt: 'Today' },
  { id: 'f2', name: 'Stadium Photos.zip', type: 'file', size: 15600000, uploadedBy: 'Fatima Al-Rashid', uploadedAt: 'Yesterday' },
  { id: 'f3', name: 'Brand Guidelines.pdf', type: 'file', size: 8900000, uploadedBy: 'Layla Omar', uploadedAt: 'Jan 9' },
  { id: 'f4', name: 'Team Roster.docx', type: 'file', size: 156000, uploadedBy: 'Mohammed Hassan', uploadedAt: 'Jan 8' },
])

const pinnedMessages = computed(() => {
  const channelMsgs = messages.value.get(selectedChannelId.value) || []
  return channelMsgs.filter(m => m.isPinned)
})

const commonEmojis = ['👍', '❤️', '😊', '🎉', '🚀', '👏', '🔥', '💯', '✅', '👀', '🙌', '💪']

// Computed
const selectedChannel = computed(() => {
  if (selectedDMId.value) {
    const dm = directMessages.value.find(d => d.id === selectedDMId.value)
    if (dm) {
      return {
        id: dm.id,
        name: dm.user.displayName,
        description: dm.user.role,
        type: 'direct' as ChannelType,
        memberCount: 2,
        unreadCount: dm.unreadCount,
        isMuted: false,
        isPinned: false
      }
    }
  }
  return channels.value.find(c => c.id === selectedChannelId.value)
})

// Get full DM info for context-aware right panel
const selectedDM = computed(() => {
  if (!selectedDMId.value) return null
  return directMessages.value.find(d => d.id === selectedDMId.value) || null
})

// Check if we're viewing a DM or a channel
const isViewingDM = computed(() => !!selectedDMId.value)

// Get files shared in the current DM conversation
const dmSharedFiles = computed(() => {
  if (!selectedDMId.value) return []
  const dmMessages = messages.value.get(selectedDMId.value) || []
  const files: Array<{ id: string; name: string; type: string; size: number; uploadedBy: string; uploadedAt: string }> = []

  dmMessages.forEach(msg => {
    if (msg.attachments) {
      msg.attachments.forEach(att => {
        files.push({
          id: att.id,
          name: att.name,
          type: att.type,
          size: att.size,
          uploadedBy: msg.sender.displayName,
          uploadedAt: formatTime(msg.createdAt)
        })
      })
    }
  })
  return files
})

// Get right panel tabs based on context (DM vs Channel)
const rightPanelTabs = computed(() => {
  if (isViewingDM.value) {
    return ['profile', 'files', 'media'] as const
  }
  return ['details', 'members', 'files', 'pinned'] as const
})

const currentMessages = computed(() => {
  const id = selectedDMId.value || selectedChannelId.value
  return messages.value.get(id) || []
})

const filteredChannels = computed(() => {
  if (!searchQuery.value) return channels.value
  const q = searchQuery.value.toLowerCase()
  return channels.value.filter(c => c.name.toLowerCase().includes(q))
})

const filteredDMs = computed(() => {
  if (!searchQuery.value) return directMessages.value
  const q = searchQuery.value.toLowerCase()
  return directMessages.value.filter(d => d.user.displayName.toLowerCase().includes(q))
})

const filteredMembers = computed(() => {
  if (!mentionQuery.value) return channelMembers.value.slice(0, 5)
  const q = mentionQuery.value.toLowerCase()
  return channelMembers.value.filter(m => m.displayName.toLowerCase().includes(q)).slice(0, 5)
})

const totalUnreadCount = computed(() => {
  const channelUnread = channels.value.reduce((sum, c) => sum + c.unreadCount, 0)
  const dmUnread = directMessages.value.reduce((sum, d) => sum + d.unreadCount, 0)
  return channelUnread + dmUnread
})

// Methods
function selectChannel(channelId: string) {
  selectedChannelId.value = channelId
  selectedDMId.value = null
  expandedThreadId.value = null
  // Mark as read
  const channel = channels.value.find(c => c.id === channelId)
  if (channel) channel.unreadCount = 0
  scrollToBottom()
}

function selectDM(dmId: string) {
  selectedDMId.value = dmId
  selectedChannelId.value = ''
  expandedThreadId.value = null
  // Mark as read
  const dm = directMessages.value.find(d => d.id === dmId)
  if (dm) dm.unreadCount = 0
  scrollToBottom()
}

function toggleTeam(teamId: string) {
  if (expandedTeamIds.value.has(teamId)) {
    expandedTeamIds.value.delete(teamId)
  } else {
    expandedTeamIds.value.add(teamId)
  }
  // Force reactivity
  expandedTeamIds.value = new Set(expandedTeamIds.value)
}

function selectTeamChannel(teamId: string, channelId: string) {
  selectedChannelId.value = channelId
  selectedDMId.value = null
  expandedThreadId.value = null
  // Mark as read
  const team = teams.value.find(t => t.id === teamId)
  const channel = team?.channels.find(c => c.id === channelId)
  if (channel) channel.unreadCount = 0
  scrollToBottom()
}

function isTeamExpanded(teamId: string): boolean {
  return expandedTeamIds.value.has(teamId)
}

function getTeamUnreadCount(team: CollaborationTeam): number {
  return team.channels.reduce((sum, c) => sum + c.unreadCount, 0)
}

function sendMessage() {
  if (!messageInput.value.trim()) return

  const id = selectedDMId.value || selectedChannelId.value
  const existingMessages = messages.value.get(id) || []

  const newMessage: Message = {
    id: `m${Date.now()}`,
    channelId: id,
    sender: { id: 'me', displayName: 'You' },
    content: messageInput.value,
    type: 'text',
    reactions: [],
    isEdited: false,
    isPinned: false,
    createdAt: new Date().toISOString()
  }

  messages.value.set(id, [...existingMessages, newMessage])
  messageInput.value = ''
  showMentionAutocomplete.value = false
  scrollToBottom()
}

function sendThreadReply(parentId: string) {
  if (!threadReplyInput.value.trim()) return

  const existingReplies = threadMessages.value.get(parentId) || []

  const newReply: Message = {
    id: `th${Date.now()}`,
    channelId: selectedChannelId.value,
    sender: { id: 'me', displayName: 'You' },
    content: threadReplyInput.value,
    type: 'text',
    isEdited: false,
    isPinned: false,
    createdAt: new Date().toISOString()
  }

  threadMessages.value.set(parentId, [...existingReplies, newReply])

  // Update thread count on parent
  const channelMsgs = messages.value.get(selectedChannelId.value) || []
  const parentMsg = channelMsgs.find(m => m.id === parentId)
  if (parentMsg) {
    parentMsg.threadCount = (parentMsg.threadCount || 0) + 1
    parentMsg.threadLastReply = threadReplyInput.value
  }

  threadReplyInput.value = ''
}

function toggleReaction(messageId: string, emoji: string) {
  const channelMsgs = messages.value.get(selectedChannelId.value || selectedDMId.value || '') || []
  const message = channelMsgs.find(m => m.id === messageId)

  if (!message) return

  if (!message.reactions) message.reactions = []

  const existingReaction = message.reactions.find(r => r.emoji === emoji)

  if (existingReaction) {
    if (existingReaction.hasReacted) {
      existingReaction.count--
      existingReaction.hasReacted = false
      existingReaction.users = existingReaction.users.filter(u => u !== 'me')
      if (existingReaction.count === 0) {
        message.reactions = message.reactions.filter(r => r.emoji !== emoji)
      }
    } else {
      existingReaction.count++
      existingReaction.hasReacted = true
      existingReaction.users.push('me')
    }
  } else {
    message.reactions.push({
      emoji,
      count: 1,
      users: ['me'],
      hasReacted: true
    })
  }

  showEmojiPicker.value = false
  activeEmojiMessageId.value = null
}

function togglePin(messageId: string) {
  const channelMsgs = messages.value.get(selectedChannelId.value || '') || []
  const message = channelMsgs.find(m => m.id === messageId)
  if (message) {
    message.isPinned = !message.isPinned
  }
}

function openThread(messageId: string) {
  expandedThreadId.value = expandedThreadId.value === messageId ? null : messageId
}

function handleInputKeydown(e: KeyboardEvent) {
  if (e.key === 'Enter' && !e.shiftKey) {
    e.preventDefault()
    sendMessage()
  }

  // Check for @ mention
  const input = e.target as HTMLTextAreaElement
  const value = input.value
  const cursorPos = input.selectionStart || 0
  const textBeforeCursor = value.substring(0, cursorPos)
  const lastAtIndex = textBeforeCursor.lastIndexOf('@')

  if (lastAtIndex !== -1 && lastAtIndex === textBeforeCursor.length - 1) {
    showMentionAutocomplete.value = true
    mentionQuery.value = ''
  } else if (lastAtIndex !== -1 && !textBeforeCursor.substring(lastAtIndex).includes(' ')) {
    showMentionAutocomplete.value = true
    mentionQuery.value = textBeforeCursor.substring(lastAtIndex + 1)
  } else {
    showMentionAutocomplete.value = false
  }
}

function insertMention(member: ChannelMember) {
  const lastAtIndex = messageInput.value.lastIndexOf('@')
  messageInput.value = messageInput.value.substring(0, lastAtIndex) + `@${member.displayName} `
  showMentionAutocomplete.value = false
}

function createChannel() {
  if (!newChannel.value.name.trim()) return

  const channel: Channel = {
    id: `c${Date.now()}`,
    name: newChannel.value.name.toLowerCase().replace(/\s+/g, '-'),
    description: newChannel.value.description,
    type: newChannel.value.isPrivate ? 'private' : 'public',
    memberCount: 1,
    unreadCount: 0,
    isMuted: false,
    isPinned: false
  }

  channels.value.push(channel)
  showCreateChannelModal.value = false
  newChannel.value = { name: '', description: '', type: 'public', isPrivate: false }
  selectChannel(channel.id)
}

function formatFileSize(bytes: number): string {
  if (bytes < 1024) return bytes + ' B'
  if (bytes < 1024 * 1024) return (bytes / 1024).toFixed(1) + ' KB'
  return (bytes / (1024 * 1024)).toFixed(1) + ' MB'
}

function formatTime(dateString: string): string {
  const date = new Date(dateString)
  return date.toLocaleTimeString('en-US', { hour: 'numeric', minute: '2-digit', hour12: true })
}

function formatDate(dateString: string): string {
  const date = new Date(dateString)
  const today = new Date()
  const yesterday = new Date(today)
  yesterday.setDate(yesterday.getDate() - 1)

  if (date.toDateString() === today.toDateString()) return 'Today'
  if (date.toDateString() === yesterday.toDateString()) return 'Yesterday'
  return date.toLocaleDateString('en-US', { month: 'short', day: 'numeric' })
}

function getPresenceColor(status: UserPresenceStatus): string {
  const colors = {
    online: 'bg-emerald-500',
    away: 'bg-amber-500',
    busy: 'bg-red-500',
    offline: 'bg-gray-400'
  }
  return colors[status]
}

function getFileIcon(type: string): string {
  const icons: Record<string, string> = {
    pdf: 'fas fa-file-pdf text-red-500',
    doc: 'fas fa-file-word text-blue-500',
    docx: 'fas fa-file-word text-blue-500',
    xls: 'fas fa-file-excel text-green-500',
    xlsx: 'fas fa-file-excel text-green-500',
    png: 'fas fa-file-image text-purple-500',
    jpg: 'fas fa-file-image text-purple-500',
    zip: 'fas fa-file-archive text-amber-500'
  }
  const defaultIcon = 'fas fa-file text-gray-500'
  const ext = type.split('.').pop()?.toLowerCase() || ''
  return icons[ext] ?? defaultIcon
}

function scrollToBottom() {
  nextTick(() => {
    if (messagesContainerRef.value) {
      messagesContainerRef.value.scrollTop = messagesContainerRef.value.scrollHeight
    }
  })
}

// Simulate typing indicator
watch(messageInput, (val) => {
  if (val) {
    isTyping.value = true
    // Simulate others typing occasionally
    if (Math.random() > 0.7) {
      typingUsers.value = ['Sarah Ahmed']
      setTimeout(() => {
        typingUsers.value = []
      }, 3000)
    }
  } else {
    isTyping.value = false
  }
})

// Reset right panel tab when switching between DM and channel
watch(isViewingDM, (isDM) => {
  rightPanelTab.value = isDM ? 'profile' : 'details'
})

// ============================================================================
// AI FEATURES - Smart Replies, Meeting Summary, Sentiment Analysis
// ============================================================================

// AI State
const showAISuggestions = ref(true)
const isGeneratingReplies = ref(false)
const isGeneratingSummary = ref(false)
const isAnalyzingSentiment = ref(false)
const showMeetingSummaryModal = ref(false)
const showSentimentPanel = ref(false)

// AI Smart Reply Suggestions
interface AISmartReply {
  id: string
  text: string
  tone: 'professional' | 'casual' | 'friendly' | 'formal'
  context: string
}

const aiSmartReplies = ref<AISmartReply[]>([])

// AI Meeting Summary
interface MeetingSummary {
  title: string
  date: string
  participants: string[]
  keyPoints: string[]
  decisions: string[]
  actionItems: { task: string; assignee: string; dueDate: string }[]
  nextSteps: string[]
}

const meetingSummary = ref<MeetingSummary | null>(null)

// AI Conversation Sentiment
interface ConversationSentiment {
  overall: 'positive' | 'neutral' | 'negative' | 'mixed'
  score: number
  breakdown: { label: string; percentage: number; color: string }[]
  highlights: { type: 'positive' | 'negative' | 'neutral'; message: string; sender: string }[]
  trend: 'improving' | 'stable' | 'declining'
}

const conversationSentiment = ref<ConversationSentiment | null>(null)

// Mock AI Smart Replies based on context
const mockSmartReplies: Record<string, AISmartReply[]> = {
  schedule: [
    { id: 'sr1', text: 'That time works perfectly for me. I\'ll add it to my calendar.', tone: 'professional', context: 'schedule' },
    { id: 'sr2', text: 'I have a conflict at that time. Could we move it to 3 PM instead?', tone: 'professional', context: 'schedule' },
    { id: 'sr3', text: 'Let me check my availability and get back to you shortly.', tone: 'formal', context: 'schedule' }
  ],
  approval: [
    { id: 'sr4', text: 'Looks great! Approved. Please proceed.', tone: 'professional', context: 'approval' },
    { id: 'sr5', text: 'I have a few suggestions. Can we discuss before finalizing?', tone: 'professional', context: 'approval' },
    { id: 'sr6', text: 'This needs some revisions. Let me share my feedback.', tone: 'formal', context: 'approval' }
  ],
  update: [
    { id: 'sr7', text: 'Thanks for the update! Great progress.', tone: 'friendly', context: 'update' },
    { id: 'sr8', text: 'Excellent work! Keep up the momentum.', tone: 'casual', context: 'update' },
    { id: 'sr9', text: 'Noted. Please keep me posted on any further developments.', tone: 'formal', context: 'update' }
  ],
  general: [
    { id: 'sr10', text: 'Thanks for sharing! I\'ll review this today.', tone: 'friendly', context: 'general' },
    { id: 'sr11', text: 'Great point! I agree with your approach.', tone: 'casual', context: 'general' },
    { id: 'sr12', text: 'Let me look into this and follow up with you.', tone: 'professional', context: 'general' }
  ]
}

// Mock Meeting Summary
const mockMeetingSummary: MeetingSummary = {
  title: 'Tournament Planning Meeting',
  date: 'January 11, 2027 at 2:00 PM',
  participants: ['Sarah Ahmed', 'Mohammed Hassan', 'Fatima Al-Rashid', 'Ahmed Khalil', 'Layla Omar'],
  keyPoints: [
    'Opening ceremony rehearsal moved to Thursday',
    'New venue designs uploaded and pending review',
    'Ticketing API integration completed successfully',
    'Media kit materials ready for distribution'
  ],
  decisions: [
    'Approved new stadium layout design with minor modifications',
    'Confirmed API rate limiting settings at 1000 requests/minute',
    'Scheduled press release for January 15th'
  ],
  actionItems: [
    { task: 'Finalize venue designs with stakeholder feedback', assignee: 'Fatima Al-Rashid', dueDate: 'Jan 13' },
    { task: 'Update API documentation with new endpoints', assignee: 'Ahmed Khalil', dueDate: 'Jan 12' },
    { task: 'Coordinate media coverage for opening ceremony', assignee: 'Layla Omar', dueDate: 'Jan 14' },
    { task: 'Send calendar invites for rehearsal', assignee: 'Sarah Ahmed', dueDate: 'Jan 12' }
  ],
  nextSteps: [
    'Review venue designs by end of day tomorrow',
    'Begin ticket sales soft launch next week',
    'Schedule follow-up meeting for January 15th'
  ]
}

// Mock Conversation Sentiment
const mockConversationSentiment: ConversationSentiment = {
  overall: 'positive',
  score: 78,
  breakdown: [
    { label: 'Positive', percentage: 65, color: '#10b981' },
    { label: 'Neutral', percentage: 28, color: '#6b7280' },
    { label: 'Negative', percentage: 7, color: '#ef4444' }
  ],
  highlights: [
    { type: 'positive', message: 'Great work team!', sender: 'Layla Omar' },
    { type: 'positive', message: 'This looks amazing!', sender: 'You' },
    { type: 'positive', message: 'Design approved!', sender: 'Fatima Al-Rashid' },
    { type: 'neutral', message: 'Quick reminder about the meeting', sender: 'Mohammed Hassan' }
  ],
  trend: 'improving'
}

// AI Functions
async function generateSmartReplies() {
  isGeneratingReplies.value = true
  aiSmartReplies.value = []

  // Simulate AI processing
  await new Promise(resolve => setTimeout(resolve, 800))

  // Analyze last message to determine context
  const lastMessage = currentMessages.value[currentMessages.value.length - 1]
  let context = 'general'

  if (lastMessage) {
    const content = lastMessage.content.toLowerCase()
    if (content.includes('schedule') || content.includes('meeting') || content.includes('time')) {
      context = 'schedule'
    } else if (content.includes('review') || content.includes('approve') || content.includes('design')) {
      context = 'approval'
    } else if (content.includes('update') || content.includes('progress') || content.includes('done') || content.includes('ready')) {
      context = 'update'
    }
  }

  aiSmartReplies.value = mockSmartReplies[context] || mockSmartReplies.general
  isGeneratingReplies.value = false
}

async function generateMeetingSummary() {
  isGeneratingSummary.value = true

  // Simulate AI processing
  await new Promise(resolve => setTimeout(resolve, 1500))

  meetingSummary.value = mockMeetingSummary
  isGeneratingSummary.value = false
  showMeetingSummaryModal.value = true
}

async function analyzeConversationSentiment() {
  isAnalyzingSentiment.value = true

  // Simulate AI processing
  await new Promise(resolve => setTimeout(resolve, 1200))

  conversationSentiment.value = mockConversationSentiment
  isAnalyzingSentiment.value = false
  showSentimentPanel.value = true
}

function useSmartReply(reply: AISmartReply) {
  messageInput.value = reply.text
  aiSmartReplies.value = []
}

function getSentimentColor(sentiment: string) {
  switch (sentiment) {
    case 'positive': return 'text-green-500'
    case 'negative': return 'text-red-500'
    case 'mixed': return 'text-amber-500'
    default: return 'text-gray-500'
  }
}

function getTrendIcon(trend: string) {
  switch (trend) {
    case 'improving': return 'fas fa-arrow-trend-up text-green-500'
    case 'declining': return 'fas fa-arrow-trend-down text-red-500'
    default: return 'fas fa-minus text-gray-500'
  }
}

// Generate smart replies when conversation changes
watch(currentMessages, () => {
  if (showAISuggestions.value && currentMessages.value.length > 0) {
    generateSmartReplies()
  }
}, { deep: true })
</script>

<template>
  <div class="collaboration-hub flex h-[calc(100vh-64px)] overflow-hidden">
    <!-- Left Sidebar - Premium Light Theme (Collapsible) -->
    <aside
      :class="[
        'bg-gradient-to-b from-white via-gray-50/50 to-white border-r border-gray-100 flex flex-col flex-shrink-0 shadow-sm transition-all duration-300 ease-in-out',
        leftPanelCollapsed ? 'w-20' : 'w-72'
      ]"
    >
      <!-- Workspace Header with Gradient -->
      <div class="p-3 bg-gradient-to-r from-teal-600 via-teal-500 to-emerald-500 text-white">
        <div class="flex items-center" :class="leftPanelCollapsed ? 'justify-center' : 'justify-between'">
          <div class="flex items-center gap-3">
            <div
              class="bg-white/20 backdrop-blur-sm rounded-xl flex items-center justify-center font-bold text-sm shadow-lg border border-white/30 cursor-pointer hover:bg-white/30 transition-all duration-200"
              :class="leftPanelCollapsed ? 'w-12 h-12' : 'w-11 h-11'"
              @click="leftPanelCollapsed = !leftPanelCollapsed"
              :title="leftPanelCollapsed ? 'Expand sidebar' : 'Collapse sidebar'"
            >
              AFC
            </div>
            <div v-if="!leftPanelCollapsed" class="transition-opacity duration-200">
              <h2 class="font-bold text-base tracking-tight">AFC 2027</h2>
              <span class="text-xs text-teal-100 flex items-center gap-1.5">
                <span class="w-2 h-2 bg-emerald-300 rounded-full animate-pulse"></span>
                {{ channelMembers.length }} members online
              </span>
            </div>
          </div>
          <button
            @click="leftPanelCollapsed = !leftPanelCollapsed"
            class="p-2 hover:bg-white/10 rounded-xl transition-all duration-200 group"
            :title="leftPanelCollapsed ? 'Expand sidebar' : 'Collapse sidebar'"
          >
            <i :class="['fas text-white/70 group-hover:text-white transition-transform duration-200', leftPanelCollapsed ? 'fa-chevron-right' : 'fa-chevron-left']"></i>
          </button>
        </div>
      </div>

      <!-- Search with Premium Styling -->
      <div class="p-3 border-b border-gray-100">
        <div v-if="!leftPanelCollapsed" class="relative group">
          <i class="fas fa-search absolute left-3.5 top-1/2 -translate-y-1/2 text-gray-400 text-sm group-focus-within:text-teal-500 transition-colors"></i>
          <input
            v-model="searchQuery"
            type="text"
            placeholder="Search conversations..."
            class="w-full bg-gray-50 text-sm text-gray-700 placeholder-gray-400 rounded-xl pl-10 pr-4 py-2.5 focus:outline-none focus:ring-2 focus:ring-teal-500/30 focus:bg-white border border-gray-200 focus:border-teal-400 transition-all duration-200"
          />
        </div>
        <button
          v-else
          class="w-full flex items-center justify-center p-2.5 bg-gray-50 rounded-xl border border-gray-200 hover:bg-teal-50 hover:border-teal-200 transition-all duration-200 group"
          title="Search"
        >
          <i class="fas fa-search text-gray-400 group-hover:text-teal-600"></i>
        </button>
      </div>

      <!-- Channels & DMs with Modern Cards -->
      <div class="flex-1 overflow-y-auto custom-scrollbar px-2 py-2">
        <!-- Channels Section -->
        <div class="mb-4">
          <div v-if="!leftPanelCollapsed" class="flex items-center justify-between mb-2 px-1">
            <button class="flex items-center gap-2 text-xs font-semibold text-gray-500 hover:text-gray-700 transition-colors uppercase tracking-wider">
              <i class="fas fa-chevron-down text-[10px]"></i>
              <span>Channels</span>
            </button>
            <button
              @click="showCreateChannelModal = true"
              class="p-1.5 hover:bg-teal-50 rounded-lg transition-all duration-200 group"
              title="Create Channel"
            >
              <i class="fas fa-plus text-gray-400 group-hover:text-teal-600 text-xs"></i>
            </button>
          </div>
          <div v-else class="flex justify-center mb-2">
            <span class="text-[10px] font-semibold text-gray-400 uppercase">CH</span>
          </div>

          <div :class="leftPanelCollapsed ? 'space-y-2' : 'space-y-1'">
            <button
              v-for="channel in filteredChannels"
              :key="channel.id"
              @click="selectChannel(channel.id)"
              :title="leftPanelCollapsed ? channel.name : ''"
              :class="[
                'w-full flex items-center rounded-xl transition-all duration-200 group',
                leftPanelCollapsed ? 'justify-center p-2' : 'gap-3 px-3 py-2.5',
                selectedChannelId === channel.id && !selectedDMId
                  ? 'bg-gradient-to-r from-teal-50 to-emerald-50 text-teal-700 shadow-sm border border-teal-100'
                  : 'text-gray-600 hover:bg-gray-50 hover:text-gray-900'
              ]"
            >
              <div class="relative">
                <div :class="[
                  'rounded-lg flex items-center justify-center transition-all duration-200',
                  leftPanelCollapsed ? 'w-10 h-10' : 'w-8 h-8',
                  selectedChannelId === channel.id && !selectedDMId
                    ? 'bg-teal-500 text-white shadow-sm'
                    : 'bg-gray-100 text-gray-500 group-hover:bg-teal-100 group-hover:text-teal-600'
                ]">
                  <i :class="channel.type === 'private' ? 'fas fa-lock' : 'fas fa-hashtag'" class="text-xs"></i>
                </div>
                <span
                  v-if="channel.unreadCount > 0 && leftPanelCollapsed"
                  class="absolute -top-1 -right-1 bg-teal-500 text-white text-[10px] font-bold w-4 h-4 rounded-full flex items-center justify-center"
                >
                  {{ channel.unreadCount > 9 ? '9+' : channel.unreadCount }}
                </span>
              </div>
              <template v-if="!leftPanelCollapsed">
                <span class="flex-1 text-left truncate font-medium text-sm">{{ channel.name }}</span>
                <span
                  v-if="channel.unreadCount > 0"
                  class="bg-gradient-to-r from-teal-500 to-emerald-500 text-white text-xs font-bold px-2 py-0.5 rounded-full min-w-[22px] text-center shadow-sm"
                >
                  {{ channel.unreadCount }}
                </span>
                <i v-if="channel.isMuted" class="fas fa-bell-slash text-gray-300 text-xs"></i>
              </template>
            </button>
          </div>
        </div>

        <!-- Teams Section -->
        <div class="mb-4">
          <div v-if="!leftPanelCollapsed" class="flex items-center gap-2 text-xs font-semibold text-gray-500 hover:text-gray-700 transition-colors mb-2 px-1 uppercase tracking-wider">
            <i class="fas fa-chevron-down text-[10px]"></i>
            <span>Teams</span>
          </div>
          <div v-else class="flex justify-center mb-2">
            <span class="text-[10px] font-semibold text-gray-400 uppercase">TM</span>
          </div>

          <div :class="leftPanelCollapsed ? 'space-y-2' : 'space-y-2'">
            <div v-for="team in teams" :key="team.id" :class="leftPanelCollapsed ? '' : 'bg-white rounded-xl border border-gray-100 overflow-hidden shadow-sm hover:shadow-md transition-shadow duration-200'">
              <!-- Team Header -->
              <button
                @click="toggleTeam(team.id)"
                :title="leftPanelCollapsed ? team.name : ''"
                :class="[
                  'w-full flex items-center transition-all duration-200',
                  leftPanelCollapsed
                    ? 'justify-center p-2 rounded-xl hover:bg-gray-50'
                    : 'gap-3 px-3 py-2.5 text-sm text-gray-700 hover:bg-gray-50'
                ]"
              >
                <i v-if="!leftPanelCollapsed" :class="[
                  'fas text-[10px] w-3 text-gray-400 transition-transform duration-200',
                  isTeamExpanded(team.id) ? 'fa-chevron-down' : 'fa-chevron-right'
                ]"></i>
                <div class="relative">
                  <div
                    :class="[
                      'rounded-lg flex items-center justify-center text-white font-bold shadow-sm',
                      leftPanelCollapsed ? 'w-10 h-10 text-xs' : 'w-8 h-8 text-xs'
                    ]"
                    :style="{ background: `linear-gradient(135deg, ${team.color}, ${team.color}dd)` }"
                  >
                    {{ team.name.substring(0, 2).toUpperCase() }}
                  </div>
                  <span
                    v-if="getTeamUnreadCount(team) > 0 && leftPanelCollapsed"
                    class="absolute -top-1 -right-1 bg-teal-500 text-white text-[10px] font-bold w-4 h-4 rounded-full flex items-center justify-center"
                  >
                    {{ getTeamUnreadCount(team) > 9 ? '9+' : getTeamUnreadCount(team) }}
                  </span>
                </div>
                <template v-if="!leftPanelCollapsed">
                  <div class="flex-1 text-left">
                    <span class="font-semibold truncate block">{{ team.name }}</span>
                    <span class="text-xs text-gray-400">{{ team.memberCount }} members</span>
                  </div>
                  <span
                    v-if="getTeamUnreadCount(team) > 0"
                    class="bg-gradient-to-r from-teal-500 to-emerald-500 text-white text-xs font-bold px-2 py-0.5 rounded-full min-w-[22px] text-center shadow-sm"
                  >
                    {{ getTeamUnreadCount(team) }}
                  </span>
                </template>
              </button>

              <!-- Team Channels (expanded) - only show when not collapsed -->
              <div v-if="isTeamExpanded(team.id) && !leftPanelCollapsed" class="bg-gray-50/50 border-t border-gray-100 py-1">
                <button
                  v-for="channel in team.channels"
                  :key="channel.id"
                  @click="selectTeamChannel(team.id, channel.id)"
                  :class="[
                    'w-full flex items-center gap-2 px-4 py-2 text-sm transition-all duration-200 group',
                    selectedChannelId === channel.id
                      ? 'bg-teal-50 text-teal-700 border-l-2 border-teal-500'
                      : 'text-gray-500 hover:bg-white hover:text-gray-700 border-l-2 border-transparent'
                  ]"
                >
                  <i :class="[
                    channel.type === 'private' ? 'fas fa-lock' : 'fas fa-hashtag',
                    'text-xs w-4'
                  ]"></i>
                  <span class="flex-1 text-left truncate">{{ channel.name }}</span>
                  <span
                    v-if="channel.unreadCount > 0"
                    class="bg-teal-500 text-white text-xs font-bold px-1.5 py-0.5 rounded-full min-w-[20px] text-center"
                  >
                    {{ channel.unreadCount }}
                  </span>
                </button>
              </div>
            </div>
          </div>
        </div>

        <!-- Direct Messages Section -->
        <div>
          <div v-if="!leftPanelCollapsed" class="flex items-center justify-between mb-2 px-1">
            <button class="flex items-center gap-2 text-xs font-semibold text-gray-500 hover:text-gray-700 transition-colors uppercase tracking-wider">
              <i class="fas fa-chevron-down text-[10px]"></i>
              <span>Direct Messages</span>
            </button>
            <button class="p-1.5 hover:bg-teal-50 rounded-lg transition-all duration-200 group" title="New Message">
              <i class="fas fa-edit text-gray-400 group-hover:text-teal-600 text-xs"></i>
            </button>
          </div>
          <div v-else class="flex justify-center mb-2">
            <span class="text-[10px] font-semibold text-gray-400 uppercase">DM</span>
          </div>

          <div :class="leftPanelCollapsed ? 'space-y-2' : 'space-y-1'">
            <button
              v-for="dm in filteredDMs"
              :key="dm.id"
              @click="selectDM(dm.id)"
              :title="leftPanelCollapsed ? dm.user.displayName : ''"
              :class="[
                'w-full flex items-center rounded-xl transition-all duration-200 group',
                leftPanelCollapsed ? 'justify-center p-2' : 'gap-3 px-3 py-2.5',
                selectedDMId === dm.id
                  ? 'bg-gradient-to-r from-teal-50 to-emerald-50 text-teal-700 shadow-sm border border-teal-100'
                  : 'text-gray-600 hover:bg-gray-50 hover:text-gray-900'
              ]"
            >
              <div class="relative">
                <div :class="[
                  'rounded-xl flex items-center justify-center font-semibold transition-all duration-200',
                  leftPanelCollapsed ? 'w-10 h-10 text-xs' : 'w-9 h-9 text-xs',
                  selectedDMId === dm.id
                    ? 'bg-gradient-to-br from-teal-400 to-teal-600 text-white shadow-sm'
                    : 'bg-gradient-to-br from-gray-100 to-gray-200 text-gray-600 group-hover:from-teal-100 group-hover:to-teal-200 group-hover:text-teal-700'
                ]">
                  {{ dm.user.displayName.split(' ').map(n => n[0]).join('') }}
                </div>
                <span
                  :class="['absolute -bottom-0.5 -right-0.5 rounded-full border-2 border-white shadow-sm', leftPanelCollapsed ? 'w-3.5 h-3.5' : 'w-3 h-3', getPresenceColor(dm.presence)]"
                ></span>
                <span
                  v-if="dm.unreadCount > 0 && leftPanelCollapsed"
                  class="absolute -top-1 -right-1 bg-teal-500 text-white text-[10px] font-bold w-4 h-4 rounded-full flex items-center justify-center"
                >
                  {{ dm.unreadCount > 9 ? '9+' : dm.unreadCount }}
                </span>
              </div>
              <template v-if="!leftPanelCollapsed">
                <div class="flex-1 min-w-0 text-left">
                  <p class="font-medium truncate text-sm">{{ dm.user.displayName }}</p>
                  <p :class="[
                    'text-xs truncate',
                    selectedDMId === dm.id ? 'text-teal-600/70' : 'text-gray-400'
                  ]">{{ dm.lastMessage }}</p>
                </div>
                <div class="flex flex-col items-end gap-1">
                  <span
                    v-if="dm.unreadCount > 0"
                    class="bg-gradient-to-r from-teal-500 to-emerald-500 text-white text-xs font-bold px-2 py-0.5 rounded-full min-w-[22px] text-center shadow-sm"
                  >
                    {{ dm.unreadCount }}
                  </span>
                  <span v-else class="text-[10px] text-gray-400">{{ dm.lastMessageTime }}</span>
                </div>
              </template>
            </button>
          </div>
        </div>
      </div>

      <!-- User Footer - Premium Card Style -->
      <div class="p-2 border-t border-gray-100 bg-gradient-to-r from-gray-50 to-white">
        <div
          :class="[
            'bg-white rounded-xl border border-gray-100 shadow-sm hover:shadow-md transition-all duration-200',
            leftPanelCollapsed ? 'p-2 flex flex-col items-center gap-2' : 'p-2 flex items-center gap-3'
          ]"
        >
          <div class="relative">
            <div :class="[
              'bg-gradient-to-br from-teal-400 to-teal-600 rounded-xl flex items-center justify-center font-semibold text-white shadow-sm',
              leftPanelCollapsed ? 'w-11 h-11' : 'w-10 h-10'
            ]">
              YO
            </div>
            <span class="absolute -bottom-0.5 -right-0.5 w-3.5 h-3.5 bg-emerald-500 rounded-full border-2 border-white shadow-sm"></span>
          </div>
          <template v-if="!leftPanelCollapsed">
            <div class="flex-1 min-w-0">
              <p class="text-sm font-semibold text-gray-800 truncate">You</p>
              <p class="text-xs text-emerald-600 flex items-center gap-1">
                <span class="w-1.5 h-1.5 bg-emerald-500 rounded-full"></span>
                Online
              </p>
            </div>
            <div class="flex items-center gap-1">
              <button class="p-2 hover:bg-gray-100 rounded-lg transition-all duration-200 group">
                <i class="fas fa-microphone text-gray-400 group-hover:text-teal-600 text-sm"></i>
              </button>
              <button class="p-2 hover:bg-gray-100 rounded-lg transition-all duration-200 group">
                <i class="fas fa-cog text-gray-400 group-hover:text-teal-600 text-sm"></i>
              </button>
            </div>
          </template>
          <template v-else>
            <button class="p-1.5 hover:bg-gray-100 rounded-lg transition-all duration-200 group" title="Settings">
              <i class="fas fa-cog text-gray-400 group-hover:text-teal-600 text-sm"></i>
            </button>
          </template>
        </div>
      </div>
    </aside>

    <!-- Main Chat Area -->
    <main class="flex-1 flex flex-col bg-white min-w-0">
      <!-- Chat Header -->
      <header class="h-14 px-4 border-b border-gray-200 flex items-center justify-between flex-shrink-0 bg-white">
        <div class="flex items-center gap-3">
          <div v-if="selectedChannel">
            <div class="flex items-center gap-2">
              <i :class="[
                selectedChannel.type === 'direct' ? 'fas fa-user' :
                selectedChannel.type === 'private' ? 'fas fa-lock' : 'fas fa-hashtag',
                'text-gray-500'
              ]"></i>
              <h1 class="font-semibold text-gray-900">{{ selectedChannel.name }}</h1>
            </div>
            <p v-if="selectedChannel.description" class="text-xs text-gray-500 truncate max-w-md">
              {{ selectedChannel.description }}
            </p>
          </div>
        </div>

        <div class="flex items-center gap-2">
          <button class="p-2 hover:bg-gray-100 rounded-lg transition-colors" title="Search">
            <i class="fas fa-search text-gray-500"></i>
          </button>
          <button class="p-2 hover:bg-gray-100 rounded-lg transition-colors" title="Start Huddle">
            <i class="fas fa-headset text-gray-500"></i>
          </button>
          <button class="p-2 hover:bg-gray-100 rounded-lg transition-colors" title="Pin">
            <i class="fas fa-thumbtack text-gray-500"></i>
          </button>
          <div class="w-px h-5 bg-gray-200 mx-1"></div>
          <button
            @click="showRightPanel = !showRightPanel"
            :class="[
              'p-2 rounded-lg transition-colors',
              showRightPanel ? 'bg-teal-50 text-teal-600' : 'hover:bg-gray-100 text-gray-500'
            ]"
            title="Toggle Details"
          >
            <i class="fas fa-sidebar-flip"></i>
          </button>
          <div class="flex items-center -space-x-2 ml-2">
            <div
              v-for="(member, idx) in channelMembers.slice(0, 4)"
              :key="member.id"
              class="w-7 h-7 bg-gray-200 rounded-full border-2 border-white flex items-center justify-center text-xs font-medium text-gray-600"
              :title="member.displayName"
            >
              {{ member.displayName.split(' ').map(n => n[0]).join('') }}
            </div>
            <div
              v-if="channelMembers.length > 4"
              class="w-7 h-7 bg-gray-300 rounded-full border-2 border-white flex items-center justify-center text-xs font-medium text-gray-600"
            >
              +{{ channelMembers.length - 4 }}
            </div>
          </div>
        </div>
      </header>

      <!-- Messages -->
      <div ref="messagesContainerRef" class="flex-1 overflow-y-auto px-4 py-4 custom-scrollbar">
        <div class="space-y-4">
          <template v-for="(message, idx) in currentMessages" :key="message.id">
            <!-- Date Separator -->
            <div
              v-if="idx === 0 || formatDate(message.createdAt) !== formatDate(currentMessages[idx - 1]?.createdAt || '')"
              class="flex items-center gap-4 my-6"
            >
              <div class="flex-1 border-t border-gray-200"></div>
              <span class="text-xs font-medium text-gray-500 bg-white px-3 py-1 rounded-full border border-gray-200">
                {{ formatDate(message.createdAt) }}
              </span>
              <div class="flex-1 border-t border-gray-200"></div>
            </div>

            <!-- Message -->
            <div
              :class="[
                'group flex gap-3 hover:bg-gray-50 -mx-4 px-4 py-2 rounded-lg transition-colors',
                message.isPinned ? 'bg-amber-50/50 border-l-2 border-amber-400' : ''
              ]"
            >
              <!-- Avatar -->
              <div class="flex-shrink-0">
                <div
                  :class="[
                    'w-9 h-9 rounded-full flex items-center justify-center font-medium text-sm',
                    message.sender.id === 'me' ? 'bg-teal-500 text-white' :
                    message.sender.id === 'system' ? 'bg-gray-200 text-gray-500' :
                    'bg-gray-200 text-gray-600'
                  ]"
                >
                  <i v-if="message.sender.id === 'system'" class="fas fa-robot"></i>
                  <span v-else>{{ message.sender.displayName.split(' ').map(n => n[0]).join('') }}</span>
                </div>
              </div>

              <!-- Content -->
              <div class="flex-1 min-w-0">
                <div class="flex items-center gap-2 mb-1">
                  <span class="font-semibold text-gray-900 text-sm">{{ message.sender.displayName }}</span>
                  <span class="text-xs text-gray-400">{{ formatTime(message.createdAt) }}</span>
                  <span v-if="message.isEdited" class="text-xs text-gray-400">(edited)</span>
                  <span v-if="message.isPinned" class="text-xs text-amber-600 font-medium">
                    <i class="fas fa-thumbtack mr-1"></i>Pinned
                  </span>
                </div>

                <!-- Text Content -->
                <p v-if="message.type === 'text'" class="text-gray-700 text-sm whitespace-pre-wrap break-words">
                  <template v-for="(part, pIdx) in message.content.split(/(@\w+(?:\s+\w+)?)/g)" :key="pIdx">
                    <span
                      v-if="part.startsWith('@')"
                      class="bg-teal-100 text-teal-700 px-1 rounded font-medium"
                    >{{ part }}</span>
                    <span v-else>{{ part }}</span>
                  </template>
                </p>

                <!-- Linked Content -->
                <div v-if="message.linkedContent" class="mt-3">
                  <!-- Event Card -->
                  <div
                    v-if="message.linkedContent.type === 'event'"
                    class="border border-gray-200 rounded-xl overflow-hidden max-w-md hover:shadow-md transition-shadow"
                  >
                    <img
                      v-if="message.linkedContent.thumbnail"
                      :src="message.linkedContent.thumbnail"
                      class="w-full h-32 object-cover"
                    />
                    <div class="p-4">
                      <div class="flex items-center gap-2 mb-2">
                        <span class="px-2 py-0.5 bg-teal-100 text-teal-700 text-xs font-medium rounded-full">
                          <i class="fas fa-calendar-alt mr-1"></i>Event
                        </span>
                      </div>
                      <h4 class="font-semibold text-gray-900 mb-1">{{ message.linkedContent.title }}</h4>
                      <p class="text-sm text-gray-600 mb-2">{{ message.linkedContent.description }}</p>
                      <p class="text-xs text-gray-500">
                        <i class="fas fa-clock mr-1"></i>{{ message.linkedContent.date }}
                      </p>
                    </div>
                  </div>

                  <!-- Article Card -->
                  <div
                    v-else-if="message.linkedContent.type === 'article'"
                    class="border border-gray-200 rounded-xl overflow-hidden max-w-md hover:shadow-md transition-shadow flex"
                  >
                    <img
                      v-if="message.linkedContent.thumbnail"
                      :src="message.linkedContent.thumbnail"
                      class="w-32 h-full object-cover"
                    />
                    <div class="p-4 flex-1">
                      <div class="flex items-center gap-2 mb-2">
                        <span class="px-2 py-0.5 bg-blue-100 text-blue-700 text-xs font-medium rounded-full">
                          <i class="fas fa-newspaper mr-1"></i>Article
                        </span>
                      </div>
                      <h4 class="font-semibold text-gray-900 mb-1 text-sm">{{ message.linkedContent.title }}</h4>
                      <p class="text-xs text-gray-600 line-clamp-2">{{ message.linkedContent.description }}</p>
                    </div>
                  </div>

                  <!-- Document Card -->
                  <div
                    v-else-if="message.linkedContent.type === 'document'"
                    class="border border-gray-200 rounded-xl p-4 max-w-md hover:shadow-md transition-shadow flex items-center gap-3"
                  >
                    <div class="w-12 h-12 bg-purple-100 rounded-xl flex items-center justify-center">
                      <i class="fas fa-file-alt text-purple-600 text-xl"></i>
                    </div>
                    <div class="flex-1">
                      <div class="flex items-center gap-2 mb-1">
                        <span class="px-2 py-0.5 bg-purple-100 text-purple-700 text-xs font-medium rounded-full">
                          Document
                        </span>
                      </div>
                      <h4 class="font-semibold text-gray-900 text-sm">{{ message.linkedContent.title }}</h4>
                      <p class="text-xs text-gray-500">{{ message.linkedContent.description }}</p>
                    </div>
                    <button class="p-2 hover:bg-gray-100 rounded-lg">
                      <i class="fas fa-external-link-alt text-gray-400"></i>
                    </button>
                  </div>
                </div>

                <!-- Attachments -->
                <div v-if="message.attachments?.length" class="mt-3 space-y-2">
                  <div
                    v-for="attachment in message.attachments"
                    :key="attachment.id"
                    class="inline-block"
                  >
                    <!-- Image -->
                    <div v-if="attachment.type === 'image'" class="rounded-xl overflow-hidden max-w-sm border border-gray-200">
                      <img :src="attachment.url" :alt="attachment.name" class="max-h-64 object-cover" />
                      <div class="px-3 py-2 bg-gray-50 text-xs text-gray-500">
                        {{ attachment.name }} · {{ formatFileSize(attachment.size) }}
                      </div>
                    </div>
                    <!-- File -->
                    <div v-else class="flex items-center gap-3 p-3 bg-gray-50 rounded-xl border border-gray-200 max-w-xs">
                      <i :class="[getFileIcon(attachment.name), 'text-2xl']"></i>
                      <div class="flex-1 min-w-0">
                        <p class="font-medium text-gray-900 text-sm truncate">{{ attachment.name }}</p>
                        <p class="text-xs text-gray-500">{{ formatFileSize(attachment.size) }}</p>
                      </div>
                      <button class="p-2 hover:bg-gray-200 rounded-lg">
                        <i class="fas fa-download text-gray-500"></i>
                      </button>
                    </div>
                  </div>
                </div>

                <!-- Reactions -->
                <div v-if="message.reactions?.length" class="flex flex-wrap gap-1.5 mt-2">
                  <button
                    v-for="reaction in message.reactions"
                    :key="reaction.emoji"
                    @click="toggleReaction(message.id, reaction.emoji)"
                    :class="[
                      'inline-flex items-center gap-1 px-2 py-0.5 rounded-full text-sm transition-all',
                      reaction.hasReacted
                        ? 'bg-teal-100 text-teal-700 border border-teal-300'
                        : 'bg-gray-100 text-gray-600 hover:bg-gray-200 border border-transparent'
                    ]"
                  >
                    <span>{{ reaction.emoji }}</span>
                    <span class="text-xs font-medium">{{ reaction.count }}</span>
                  </button>
                  <button
                    @click="activeEmojiMessageId = activeEmojiMessageId === message.id ? null : message.id; showEmojiPicker = !showEmojiPicker"
                    class="inline-flex items-center justify-center w-7 h-7 rounded-full bg-gray-100 hover:bg-gray-200 text-gray-500 transition-colors"
                  >
                    <i class="fas fa-plus text-xs"></i>
                  </button>
                </div>

                <!-- Thread Preview -->
                <button
                  v-if="message.threadCount"
                  @click="openThread(message.id)"
                  class="mt-2 flex items-center gap-2 text-sm text-teal-600 hover:text-teal-700 hover:underline"
                >
                  <i class="fas fa-comments"></i>
                  <span>{{ message.threadCount }} {{ message.threadCount === 1 ? 'reply' : 'replies' }}</span>
                  <span class="text-gray-400 font-normal">Last reply: {{ message.threadLastReply }}</span>
                </button>

                <!-- Expanded Thread -->
                <div
                  v-if="expandedThreadId === message.id"
                  class="mt-3 ml-4 pl-4 border-l-2 border-teal-200 space-y-3"
                >
                  <div
                    v-for="reply in (threadMessages.get(message.id) || [])"
                    :key="reply.id"
                    class="flex gap-2"
                  >
                    <div class="w-7 h-7 bg-gray-200 rounded-full flex items-center justify-center text-xs font-medium">
                      {{ reply.sender.displayName.split(' ').map(n => n[0]).join('') }}
                    </div>
                    <div>
                      <div class="flex items-center gap-2">
                        <span class="font-medium text-gray-900 text-sm">{{ reply.sender.displayName }}</span>
                        <span class="text-xs text-gray-400">{{ formatTime(reply.createdAt) }}</span>
                      </div>
                      <p class="text-sm text-gray-700">{{ reply.content }}</p>
                    </div>
                  </div>

                  <!-- Thread Reply Input -->
                  <div class="flex gap-2 items-start">
                    <div class="w-7 h-7 bg-teal-500 rounded-full flex items-center justify-center text-white text-xs font-medium">
                      YO
                    </div>
                    <div class="flex-1">
                      <input
                        v-model="threadReplyInput"
                        @keydown.enter="sendThreadReply(message.id)"
                        type="text"
                        placeholder="Reply in thread..."
                        class="w-full px-3 py-2 text-sm border border-gray-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-teal-500 focus:border-transparent"
                      />
                    </div>
                  </div>
                </div>
              </div>

              <!-- Message Actions (on hover) -->
              <div class="flex-shrink-0 opacity-0 group-hover:opacity-100 transition-opacity">
                <div class="flex items-center bg-white border border-gray-200 rounded-lg shadow-sm">
                  <button
                    @click="activeEmojiMessageId = message.id; showEmojiPicker = true"
                    class="p-1.5 hover:bg-gray-100 rounded-l-lg transition-colors"
                    title="Add reaction"
                  >
                    <i class="fas fa-smile text-gray-500 text-sm"></i>
                  </button>
                  <button
                    @click="openThread(message.id)"
                    class="p-1.5 hover:bg-gray-100 transition-colors"
                    title="Reply in thread"
                  >
                    <i class="fas fa-comment text-gray-500 text-sm"></i>
                  </button>
                  <button
                    @click="togglePin(message.id)"
                    class="p-1.5 hover:bg-gray-100 transition-colors"
                    :title="message.isPinned ? 'Unpin' : 'Pin'"
                  >
                    <i :class="['fas fa-thumbtack text-sm', message.isPinned ? 'text-amber-500' : 'text-gray-500']"></i>
                  </button>
                  <button class="p-1.5 hover:bg-gray-100 rounded-r-lg transition-colors" title="More">
                    <i class="fas fa-ellipsis-h text-gray-500 text-sm"></i>
                  </button>
                </div>
              </div>
            </div>
          </template>
        </div>

        <!-- Typing Indicator -->
        <div v-if="typingUsers.length" class="flex items-center gap-2 text-sm text-gray-500 mt-4">
          <div class="flex gap-1">
            <span class="w-2 h-2 bg-gray-400 rounded-full animate-bounce" style="animation-delay: 0ms"></span>
            <span class="w-2 h-2 bg-gray-400 rounded-full animate-bounce" style="animation-delay: 150ms"></span>
            <span class="w-2 h-2 bg-gray-400 rounded-full animate-bounce" style="animation-delay: 300ms"></span>
          </div>
          <span>{{ typingUsers.join(', ') }} {{ typingUsers.length === 1 ? 'is' : 'are' }} typing...</span>
        </div>
      </div>

      <!-- Message Input -->
      <div class="p-4 border-t border-gray-200 bg-white relative">
        <!-- Emoji Picker -->
        <div
          v-if="showEmojiPicker && activeEmojiMessageId"
          class="absolute bottom-full mb-2 left-1/2 -translate-x-1/2 bg-white rounded-xl shadow-xl border border-gray-200 p-3 z-50"
        >
          <div class="flex flex-wrap gap-1 max-w-xs">
            <button
              v-for="emoji in commonEmojis"
              :key="emoji"
              @click="toggleReaction(activeEmojiMessageId!, emoji)"
              class="w-9 h-9 hover:bg-gray-100 rounded-lg flex items-center justify-center text-xl transition-colors"
            >
              {{ emoji }}
            </button>
          </div>
        </div>

        <!-- Mention Autocomplete -->
        <div
          v-if="showMentionAutocomplete && filteredMembers.length"
          class="absolute bottom-full mb-2 left-4 bg-white rounded-xl shadow-xl border border-gray-200 p-2 z-50 w-64"
        >
          <button
            v-for="member in filteredMembers"
            :key="member.id"
            @click="insertMention(member)"
            class="w-full flex items-center gap-3 px-3 py-2 hover:bg-gray-50 rounded-lg transition-colors"
          >
            <div class="w-8 h-8 bg-gray-200 rounded-full flex items-center justify-center text-sm font-medium">
              {{ member.displayName.split(' ').map(n => n[0]).join('') }}
            </div>
            <div class="text-left">
              <p class="font-medium text-gray-900 text-sm">{{ member.displayName }}</p>
              <p class="text-xs text-gray-500">{{ member.role }}</p>
            </div>
          </button>
        </div>

        <!-- AI Smart Reply Suggestions -->
        <div v-if="showAISuggestions && aiSmartReplies.length > 0" class="ai-smart-replies">
          <div class="ai-replies-header">
            <div class="flex items-center gap-2">
              <i class="fas fa-wand-magic-sparkles text-teal-500"></i>
              <span class="text-xs font-medium text-gray-600">AI Suggestions</span>
            </div>
            <button @click="showAISuggestions = false" class="text-gray-400 hover:text-gray-600">
              <i class="fas fa-times text-xs"></i>
            </button>
          </div>
          <div class="ai-replies-list">
            <button
              v-for="reply in aiSmartReplies"
              :key="reply.id"
              @click="useSmartReply(reply)"
              class="ai-reply-chip"
            >
              <span class="ai-reply-text">{{ reply.text }}</span>
              <span :class="['ai-reply-tone', `tone-${reply.tone}`]">{{ reply.tone }}</span>
            </button>
          </div>
        </div>

        <div class="flex items-end gap-3">
          <div class="flex gap-1">
            <button class="p-2 hover:bg-gray-100 rounded-lg transition-colors" title="Attach file">
              <i class="fas fa-paperclip text-gray-500"></i>
            </button>
            <button
              @click="showContentPicker = true"
              class="p-2 hover:bg-gray-100 rounded-lg transition-colors"
              title="Share content"
            >
              <i class="fas fa-share-square text-gray-500"></i>
            </button>
            <!-- AI Action Buttons -->
            <button
              @click="generateMeetingSummary"
              :disabled="isGeneratingSummary"
              class="p-2 hover:bg-teal-50 rounded-lg transition-colors group"
              title="AI Meeting Summary"
            >
              <i :class="[isGeneratingSummary ? 'fas fa-spinner fa-spin' : 'fas fa-wand-magic-sparkles', 'text-teal-500 group-hover:text-teal-600']"></i>
            </button>
            <button
              @click="analyzeConversationSentiment"
              :disabled="isAnalyzingSentiment"
              class="p-2 hover:bg-purple-50 rounded-lg transition-colors group"
              title="Analyze Sentiment"
            >
              <i :class="[isAnalyzingSentiment ? 'fas fa-spinner fa-spin' : 'fas fa-chart-pie', 'text-purple-500 group-hover:text-purple-600']"></i>
            </button>
          </div>

          <div class="flex-1 relative">
            <textarea
              v-model="messageInput"
              @keydown="handleInputKeydown"
              :placeholder="`Message #${selectedChannel?.name || 'channel'}...`"
              rows="1"
              class="w-full px-4 py-3 border border-gray-200 rounded-xl resize-none focus:outline-none focus:ring-2 focus:ring-teal-500 focus:border-transparent text-sm"
              style="max-height: 120px"
            ></textarea>
          </div>

          <div class="flex gap-1">
            <button class="p-2 hover:bg-gray-100 rounded-lg transition-colors" title="Emoji">
              <i class="fas fa-smile text-gray-500"></i>
            </button>
            <button
              @click="sendMessage"
              :disabled="!messageInput.trim()"
              :class="[
                'p-2 rounded-lg transition-colors',
                messageInput.trim()
                  ? 'bg-teal-500 text-white hover:bg-teal-600'
                  : 'bg-gray-100 text-gray-400 cursor-not-allowed'
              ]"
            >
              <i class="fas fa-paper-plane"></i>
            </button>
          </div>
        </div>
      </div>
    </main>

    <!-- Right Panel - Details (Collapsible) -->
    <aside
      v-if="showRightPanel"
      :class="[
        'border-l border-gray-200 bg-gray-50 flex flex-col flex-shrink-0 transition-all duration-300 ease-in-out',
        rightPanelCollapsed ? 'w-16' : 'w-80'
      ]"
    >
      <!-- Panel Header (Context Aware) -->
      <div class="h-14 px-3 border-b border-gray-200 flex items-center bg-white" :class="rightPanelCollapsed ? 'justify-center' : 'justify-between'">
        <h3 v-if="!rightPanelCollapsed" class="font-semibold text-gray-900">{{ isViewingDM ? 'Profile' : 'Details' }}</h3>
        <button
          @click="rightPanelCollapsed = !rightPanelCollapsed"
          class="p-1.5 hover:bg-gray-100 rounded-lg transition-colors"
          :title="rightPanelCollapsed ? 'Expand panel' : 'Collapse panel'"
        >
          <i :class="['fas text-gray-500', rightPanelCollapsed ? 'fa-chevron-left' : 'fa-chevron-right']"></i>
        </button>
      </div>

      <!-- Collapsed View - Icons Only -->
      <div v-if="rightPanelCollapsed" class="flex-1 flex flex-col items-center py-3 gap-2">
        <!-- Tab Icons -->
        <button
          v-for="tab in rightPanelTabs"
          :key="tab"
          @click="rightPanelTab = tab; rightPanelCollapsed = false"
          :title="tab"
          :class="[
            'w-10 h-10 rounded-xl flex items-center justify-center transition-all duration-200',
            rightPanelTab === tab
              ? 'bg-teal-100 text-teal-600'
              : 'text-gray-400 hover:bg-gray-100 hover:text-gray-600'
          ]"
        >
          <i :class="[
            'fas',
            tab === 'details' ? 'fa-info-circle' :
            tab === 'members' ? 'fa-users' :
            tab === 'files' ? 'fa-folder' :
            tab === 'pinned' ? 'fa-thumbtack' :
            tab === 'profile' ? 'fa-user' :
            tab === 'media' ? 'fa-photo-video' : 'fa-circle'
          ]"></i>
        </button>
      </div>

      <!-- Panel Tabs - Context Aware (Expanded) -->
      <div v-if="!rightPanelCollapsed" class="px-4 py-2 border-b border-gray-200 bg-white">
        <div class="flex gap-1">
          <button
            v-for="tab in rightPanelTabs"
            :key="tab"
            @click="rightPanelTab = tab"
            :class="[
              'px-3 py-1.5 text-sm font-medium rounded-lg transition-colors capitalize',
              rightPanelTab === tab
                ? 'bg-teal-100 text-teal-700'
                : 'text-gray-500 hover:bg-gray-100'
            ]"
          >
            {{ tab }}
          </button>
        </div>
      </div>

      <!-- Panel Content (only show when expanded) -->
      <div v-if="!rightPanelCollapsed" class="flex-1 overflow-y-auto custom-scrollbar p-4">
        <!-- Details Tab -->
        <div v-if="rightPanelTab === 'details' && selectedChannel">
          <div class="text-center mb-6">
            <div class="w-16 h-16 bg-teal-100 rounded-2xl flex items-center justify-center mx-auto mb-3">
              <i :class="[
                selectedChannel.type === 'private' ? 'fas fa-lock' : 'fas fa-hashtag',
                'text-teal-600 text-2xl'
              ]"></i>
            </div>
            <h4 class="font-semibold text-gray-900 text-lg">{{ selectedChannel.name }}</h4>
            <p class="text-sm text-gray-500 mt-1">{{ selectedChannel.description }}</p>
          </div>

          <div class="space-y-4">
            <div class="flex items-center justify-between py-2">
              <span class="text-sm text-gray-500">Type</span>
              <span class="text-sm font-medium text-gray-900 capitalize flex items-center gap-1">
                <i :class="selectedChannel.type === 'private' ? 'fas fa-lock text-amber-500' : 'fas fa-globe text-teal-500'"></i>
                {{ selectedChannel.type }}
              </span>
            </div>
            <div class="flex items-center justify-between py-2">
              <span class="text-sm text-gray-500">Members</span>
              <span class="text-sm font-medium text-gray-900">{{ selectedChannel.memberCount }}</span>
            </div>
            <div class="flex items-center justify-between py-2">
              <span class="text-sm text-gray-500">Notifications</span>
              <button class="text-sm font-medium text-teal-600">
                {{ selectedChannel.isMuted ? 'Muted' : 'All messages' }}
              </button>
            </div>
          </div>

          <div class="mt-6 space-y-2">
            <button class="w-full flex items-center gap-3 px-3 py-2 text-sm text-gray-700 hover:bg-white rounded-lg transition-colors">
              <i class="fas fa-bell w-5 text-gray-400"></i>
              <span>Notification preferences</span>
            </button>
            <button class="w-full flex items-center gap-3 px-3 py-2 text-sm text-gray-700 hover:bg-white rounded-lg transition-colors">
              <i class="fas fa-edit w-5 text-gray-400"></i>
              <span>Edit channel</span>
            </button>
            <button class="w-full flex items-center gap-3 px-3 py-2 text-sm text-red-600 hover:bg-red-50 rounded-lg transition-colors">
              <i class="fas fa-sign-out-alt w-5"></i>
              <span>Leave channel</span>
            </button>
          </div>
        </div>

        <!-- Profile Tab (DM Context) -->
        <div v-if="rightPanelTab === 'profile' && selectedDM">
          <!-- User Profile Header -->
          <div class="text-center mb-6">
            <div class="relative inline-block">
              <div class="w-20 h-20 bg-gradient-to-br from-teal-400 to-teal-600 rounded-full flex items-center justify-center mx-auto mb-3 text-white text-2xl font-semibold">
                {{ selectedDM.user.displayName.split(' ').map(n => n[0]).join('') }}
              </div>
              <span
                :class="['absolute bottom-1 right-1 w-4 h-4 rounded-full border-3 border-white', getPresenceColor(selectedDM.presence)]"
              ></span>
            </div>
            <h4 class="font-semibold text-gray-900 text-lg">{{ selectedDM.user.displayName }}</h4>
            <p class="text-sm text-gray-500 mt-1">{{ selectedDM.user.role }}</p>
            <div class="flex items-center justify-center gap-1 mt-2">
              <span :class="['w-2 h-2 rounded-full', getPresenceColor(selectedDM.presence)]"></span>
              <span class="text-xs text-gray-500 capitalize">{{ selectedDM.presence }}</span>
            </div>
          </div>

          <!-- Quick Actions -->
          <div class="grid grid-cols-3 gap-2 mb-6">
            <button class="flex flex-col items-center gap-1 p-3 bg-white rounded-xl border border-gray-100 hover:border-teal-200 hover:bg-teal-50 transition-colors">
              <i class="fas fa-video text-teal-600"></i>
              <span class="text-xs text-gray-600">Video</span>
            </button>
            <button class="flex flex-col items-center gap-1 p-3 bg-white rounded-xl border border-gray-100 hover:border-teal-200 hover:bg-teal-50 transition-colors">
              <i class="fas fa-phone text-teal-600"></i>
              <span class="text-xs text-gray-600">Call</span>
            </button>
            <button class="flex flex-col items-center gap-1 p-3 bg-white rounded-xl border border-gray-100 hover:border-teal-200 hover:bg-teal-50 transition-colors">
              <i class="fas fa-calendar-plus text-teal-600"></i>
              <span class="text-xs text-gray-600">Schedule</span>
            </button>
          </div>

          <!-- User Info -->
          <div class="bg-white rounded-xl border border-gray-100 p-4 mb-4">
            <h5 class="text-xs font-semibold text-gray-400 uppercase tracking-wider mb-3">Contact Information</h5>
            <div class="space-y-3">
              <div class="flex items-center gap-3">
                <i class="fas fa-envelope text-gray-400 w-5"></i>
                <div>
                  <p class="text-xs text-gray-400">Email</p>
                  <p class="text-sm text-gray-700">{{ selectedDM.user.displayName.toLowerCase().replace(' ', '.') }}@afc2027.org</p>
                </div>
              </div>
              <div class="flex items-center gap-3">
                <i class="fas fa-building text-gray-400 w-5"></i>
                <div>
                  <p class="text-xs text-gray-400">Department</p>
                  <p class="text-sm text-gray-700">{{ selectedDM.user.role }}</p>
                </div>
              </div>
              <div class="flex items-center gap-3">
                <i class="fas fa-clock text-gray-400 w-5"></i>
                <div>
                  <p class="text-xs text-gray-400">Local Time</p>
                  <p class="text-sm text-gray-700">{{ new Date().toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit' }) }} (GST)</p>
                </div>
              </div>
            </div>
          </div>

          <!-- Conversation Actions -->
          <div class="space-y-2">
            <button class="w-full flex items-center gap-3 px-3 py-2 text-sm text-gray-700 hover:bg-white rounded-lg transition-colors">
              <i class="fas fa-search w-5 text-gray-400"></i>
              <span>Search in conversation</span>
            </button>
            <button class="w-full flex items-center gap-3 px-3 py-2 text-sm text-gray-700 hover:bg-white rounded-lg transition-colors">
              <i class="fas fa-bell-slash w-5 text-gray-400"></i>
              <span>Mute notifications</span>
            </button>
            <button class="w-full flex items-center gap-3 px-3 py-2 text-sm text-gray-700 hover:bg-white rounded-lg transition-colors">
              <i class="fas fa-thumbtack w-5 text-gray-400"></i>
              <span>Pin conversation</span>
            </button>
            <button class="w-full flex items-center gap-3 px-3 py-2 text-sm text-red-600 hover:bg-red-50 rounded-lg transition-colors">
              <i class="fas fa-ban w-5"></i>
              <span>Block user</span>
            </button>
          </div>
        </div>

        <!-- Media Tab (DM Context) -->
        <div v-if="rightPanelTab === 'media' && selectedDM">
          <div class="mb-4">
            <h5 class="text-xs font-semibold text-gray-400 uppercase tracking-wider mb-3">Shared Photos & Videos</h5>
            <div v-if="dmSharedFiles.filter(f => f.type === 'image' || f.type === 'video').length === 0" class="text-center py-8 text-gray-400">
              <i class="fas fa-photo-video text-3xl mb-3 opacity-50"></i>
              <p class="text-sm">No media shared yet</p>
            </div>
            <div v-else class="grid grid-cols-3 gap-2">
              <div
                v-for="file in dmSharedFiles.filter(f => f.type === 'image' || f.type === 'video')"
                :key="file.id"
                class="aspect-square bg-gray-200 rounded-lg overflow-hidden cursor-pointer hover:opacity-90 transition-opacity"
              >
                <div class="w-full h-full flex items-center justify-center bg-gray-100">
                  <i :class="file.type === 'video' ? 'fas fa-play-circle text-gray-400 text-2xl' : 'fas fa-image text-gray-400 text-2xl'"></i>
                </div>
              </div>
            </div>
          </div>

          <div>
            <h5 class="text-xs font-semibold text-gray-400 uppercase tracking-wider mb-3">Shared Links</h5>
            <div class="text-center py-6 text-gray-400">
              <i class="fas fa-link text-2xl mb-2 opacity-50"></i>
              <p class="text-sm">No links shared yet</p>
            </div>
          </div>
        </div>

        <!-- Members Tab -->
        <div v-if="rightPanelTab === 'members'" class="space-y-1">
          <div
            v-for="member in channelMembers"
            :key="member.id"
            class="flex items-center gap-3 p-2 hover:bg-white rounded-lg transition-colors cursor-pointer"
          >
            <div class="relative">
              <div class="w-10 h-10 bg-gray-200 rounded-full flex items-center justify-center font-medium text-sm">
                {{ member.displayName.split(' ').map(n => n[0]).join('') }}
              </div>
              <span
                :class="['absolute bottom-0 right-0 w-3 h-3 rounded-full border-2 border-gray-50', getPresenceColor(member.presence)]"
              ></span>
            </div>
            <div class="flex-1 min-w-0">
              <p class="font-medium text-gray-900 text-sm truncate">{{ member.displayName }}</p>
              <p class="text-xs text-gray-500 truncate">{{ member.role }}</p>
            </div>
            <button class="p-1.5 hover:bg-gray-100 rounded-lg opacity-0 group-hover:opacity-100 transition-opacity">
              <i class="fas fa-comment text-gray-400 text-sm"></i>
            </button>
          </div>
        </div>

        <!-- Files Tab (Context Aware) -->
        <div v-if="rightPanelTab === 'files'" class="space-y-2">
          <!-- DM Context -->
          <template v-if="isViewingDM">
            <div v-if="dmSharedFiles.length === 0" class="text-center py-8 text-gray-400">
              <i class="fas fa-file text-3xl mb-3 opacity-50"></i>
              <p class="text-sm">No files shared yet</p>
              <p class="text-xs mt-1">Files shared in this conversation will appear here</p>
            </div>
            <div
              v-for="file in dmSharedFiles"
              :key="file.id"
              class="flex items-center gap-3 p-3 bg-white rounded-xl border border-gray-100 hover:shadow-sm transition-shadow"
            >
              <i :class="[getFileIcon(file.name), 'text-xl']"></i>
              <div class="flex-1 min-w-0">
                <p class="font-medium text-gray-900 text-sm truncate">{{ file.name }}</p>
                <p class="text-xs text-gray-500">{{ formatFileSize(file.size) }} · {{ file.uploadedBy }}</p>
              </div>
              <button class="p-1.5 hover:bg-gray-100 rounded-lg transition-colors">
                <i class="fas fa-download text-gray-400 text-sm"></i>
              </button>
            </div>
          </template>
          <!-- Channel Context -->
          <template v-else>
            <div
              v-for="file in sharedFiles"
              :key="file.id"
              class="flex items-center gap-3 p-3 bg-white rounded-xl border border-gray-100 hover:shadow-sm transition-shadow"
            >
              <i :class="[getFileIcon(file.name), 'text-xl']"></i>
              <div class="flex-1 min-w-0">
                <p class="font-medium text-gray-900 text-sm truncate">{{ file.name }}</p>
                <p class="text-xs text-gray-500">{{ formatFileSize(file.size) }} · {{ file.uploadedBy }}</p>
              </div>
              <button class="p-1.5 hover:bg-gray-100 rounded-lg transition-colors">
                <i class="fas fa-download text-gray-400 text-sm"></i>
              </button>
            </div>
          </template>
        </div>

        <!-- Pinned Tab -->
        <div v-if="rightPanelTab === 'pinned'" class="space-y-3">
          <div v-if="pinnedMessages.length === 0" class="text-center py-8 text-gray-500">
            <i class="fas fa-thumbtack text-3xl mb-3 opacity-50"></i>
            <p class="text-sm">No pinned messages yet</p>
          </div>
          <div
            v-for="message in pinnedMessages"
            :key="message.id"
            class="p-3 bg-white rounded-xl border border-gray-100"
          >
            <div class="flex items-center gap-2 mb-2">
              <div class="w-6 h-6 bg-gray-200 rounded-full flex items-center justify-center text-xs font-medium">
                {{ message.sender.displayName.split(' ').map(n => n[0]).join('') }}
              </div>
              <span class="font-medium text-gray-900 text-sm">{{ message.sender.displayName }}</span>
              <span class="text-xs text-gray-400">{{ formatTime(message.createdAt) }}</span>
            </div>
            <p class="text-sm text-gray-700 line-clamp-3">{{ message.content }}</p>
            <button
              @click="togglePin(message.id)"
              class="mt-2 text-xs text-teal-600 hover:text-teal-700"
            >
              <i class="fas fa-thumbtack mr-1"></i>Unpin
            </button>
          </div>
        </div>
      </div>
    </aside>

    <!-- Create Channel Modal -->
    <div
      v-if="showCreateChannelModal"
      class="fixed inset-0 bg-black/50 flex items-center justify-center z-50"
      @click.self="showCreateChannelModal = false"
    >
      <div class="bg-white rounded-2xl shadow-2xl w-full max-w-md p-6 m-4">
        <div class="flex items-center justify-between mb-6">
          <h3 class="text-xl font-bold text-gray-900">Create Channel</h3>
          <button
            @click="showCreateChannelModal = false"
            class="p-2 hover:bg-gray-100 rounded-lg transition-colors"
          >
            <i class="fas fa-times text-gray-500"></i>
          </button>
        </div>

        <div class="space-y-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Channel Name</label>
            <div class="relative">
              <span class="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400">#</span>
              <input
                v-model="newChannel.name"
                type="text"
                placeholder="e.g. marketing"
                class="w-full pl-8 pr-4 py-2.5 border border-gray-200 rounded-xl focus:outline-none focus:ring-2 focus:ring-teal-500 focus:border-transparent"
              />
            </div>
          </div>

          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Description (optional)</label>
            <textarea
              v-model="newChannel.description"
              placeholder="What's this channel about?"
              rows="2"
              class="w-full px-4 py-2.5 border border-gray-200 rounded-xl focus:outline-none focus:ring-2 focus:ring-teal-500 focus:border-transparent resize-none"
            ></textarea>
          </div>

          <div class="flex items-center gap-3 p-3 bg-gray-50 rounded-xl">
            <label class="flex items-center gap-3 cursor-pointer flex-1">
              <input
                v-model="newChannel.isPrivate"
                type="checkbox"
                class="w-5 h-5 rounded border-gray-300 text-teal-500 focus:ring-teal-500"
              />
              <div>
                <p class="font-medium text-gray-900 text-sm">Make private</p>
                <p class="text-xs text-gray-500">Only invited members can see this channel</p>
              </div>
            </label>
            <i :class="['fas text-xl', newChannel.isPrivate ? 'fa-lock text-amber-500' : 'fa-globe text-teal-500']"></i>
          </div>
        </div>

        <div class="flex gap-3 mt-6">
          <button
            @click="showCreateChannelModal = false"
            class="flex-1 px-4 py-2.5 border border-gray-200 rounded-xl text-gray-700 font-medium hover:bg-gray-50 transition-colors"
          >
            Cancel
          </button>
          <button
            @click="createChannel"
            :disabled="!newChannel.name.trim()"
            :class="[
              'flex-1 px-4 py-2.5 rounded-xl font-medium transition-colors',
              newChannel.name.trim()
                ? 'bg-teal-500 text-white hover:bg-teal-600'
                : 'bg-gray-100 text-gray-400 cursor-not-allowed'
            ]"
          >
            Create Channel
          </button>
        </div>
      </div>
    </div>

    <!-- Content Picker Modal -->
    <div
      v-if="showContentPicker"
      class="fixed inset-0 bg-black/50 flex items-center justify-center z-50"
      @click.self="showContentPicker = false"
    >
      <div class="bg-white rounded-2xl shadow-2xl w-full max-w-lg p-6 m-4">
        <div class="flex items-center justify-between mb-6">
          <h3 class="text-xl font-bold text-gray-900">Share Content</h3>
          <button
            @click="showContentPicker = false"
            class="p-2 hover:bg-gray-100 rounded-lg transition-colors"
          >
            <i class="fas fa-times text-gray-500"></i>
          </button>
        </div>

        <p class="text-gray-500 text-sm mb-4">Share content from other modules in this channel</p>

        <div class="grid grid-cols-2 gap-3">
          <button class="flex flex-col items-center gap-3 p-6 border-2 border-gray-200 rounded-2xl hover:border-teal-500 hover:bg-teal-50 transition-all">
            <div class="w-14 h-14 bg-teal-100 rounded-xl flex items-center justify-center">
              <i class="fas fa-calendar-alt text-teal-600 text-2xl"></i>
            </div>
            <span class="font-medium text-gray-900">Event</span>
          </button>
          <button class="flex flex-col items-center gap-3 p-6 border-2 border-gray-200 rounded-2xl hover:border-blue-500 hover:bg-blue-50 transition-all">
            <div class="w-14 h-14 bg-blue-100 rounded-xl flex items-center justify-center">
              <i class="fas fa-newspaper text-blue-600 text-2xl"></i>
            </div>
            <span class="font-medium text-gray-900">Article</span>
          </button>
          <button class="flex flex-col items-center gap-3 p-6 border-2 border-gray-200 rounded-2xl hover:border-purple-500 hover:bg-purple-50 transition-all">
            <div class="w-14 h-14 bg-purple-100 rounded-xl flex items-center justify-center">
              <i class="fas fa-file-alt text-purple-600 text-2xl"></i>
            </div>
            <span class="font-medium text-gray-900">Document</span>
          </button>
          <button class="flex flex-col items-center gap-3 p-6 border-2 border-gray-200 rounded-2xl hover:border-amber-500 hover:bg-amber-50 transition-all">
            <div class="w-14 h-14 bg-amber-100 rounded-xl flex items-center justify-center">
              <i class="fas fa-poll text-amber-600 text-2xl"></i>
            </div>
            <span class="font-medium text-gray-900">Poll</span>
          </button>
        </div>
      </div>
    </div>
  </div>

  <!-- AI Meeting Summary Modal -->
  <div v-if="showMeetingSummaryModal" class="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
    <div class="bg-white rounded-2xl shadow-2xl w-full max-w-2xl max-h-[80vh] overflow-hidden">
      <div class="p-6 border-b border-gray-100">
        <div class="flex items-center justify-between">
          <div class="flex items-center gap-3">
            <div class="w-10 h-10 bg-gradient-to-br from-teal-500 to-emerald-500 rounded-xl flex items-center justify-center">
              <i class="fas fa-wand-magic-sparkles text-white"></i>
            </div>
            <div>
              <h3 class="text-lg font-semibold text-gray-900">AI Meeting Summary</h3>
              <p class="text-sm text-gray-500">Generated from conversation</p>
            </div>
          </div>
          <button @click="showMeetingSummaryModal = false" class="p-2 hover:bg-gray-100 rounded-lg transition-colors">
            <i class="fas fa-times text-gray-400"></i>
          </button>
        </div>
      </div>

      <div class="p-6 overflow-y-auto max-h-[60vh]">
        <AILoadingIndicator v-if="isGeneratingSummary" message="Generating meeting summary..." />

        <div v-else-if="meetingSummary" class="space-y-6">
          <!-- Summary Header -->
          <div class="bg-gradient-to-r from-teal-50 to-emerald-50 rounded-xl p-4">
            <h4 class="font-semibold text-gray-900 mb-2">{{ meetingSummary.title }}</h4>
            <div class="flex items-center gap-4 text-sm text-gray-600">
              <span><i class="fas fa-calendar mr-1"></i> {{ meetingSummary.date }}</span>
              <span><i class="fas fa-clock mr-1"></i> {{ meetingSummary.duration }}</span>
              <span><i class="fas fa-users mr-1"></i> {{ meetingSummary.participants.length }} participants</span>
            </div>
          </div>

          <!-- Key Points -->
          <div>
            <h5 class="font-medium text-gray-900 mb-3 flex items-center gap-2">
              <i class="fas fa-list-check text-teal-500"></i> Key Discussion Points
            </h5>
            <ul class="space-y-2">
              <li v-for="(point, idx) in meetingSummary.keyPoints" :key="idx"
                  class="flex items-start gap-2 text-gray-700">
                <i class="fas fa-check-circle text-teal-500 mt-1 text-sm"></i>
                <span>{{ point }}</span>
              </li>
            </ul>
          </div>

          <!-- Decisions Made -->
          <div>
            <h5 class="font-medium text-gray-900 mb-3 flex items-center gap-2">
              <i class="fas fa-gavel text-amber-500"></i> Decisions Made
            </h5>
            <ul class="space-y-2">
              <li v-for="(decision, idx) in meetingSummary.decisions" :key="idx"
                  class="flex items-start gap-2 text-gray-700 bg-amber-50 p-3 rounded-lg">
                <i class="fas fa-circle-check text-amber-500 mt-0.5"></i>
                <span>{{ decision }}</span>
              </li>
            </ul>
          </div>

          <!-- Action Items -->
          <div>
            <h5 class="font-medium text-gray-900 mb-3 flex items-center gap-2">
              <i class="fas fa-tasks text-blue-500"></i> Action Items
            </h5>
            <div class="space-y-2">
              <div v-for="(action, idx) in meetingSummary.actionItems" :key="idx"
                   class="flex items-center justify-between bg-blue-50 p-3 rounded-lg">
                <div class="flex items-center gap-3">
                  <i class="fas fa-circle text-blue-400 text-xs"></i>
                  <span class="text-gray-700">{{ action.task }}</span>
                </div>
                <div class="flex items-center gap-3 text-sm">
                  <span class="text-gray-500">{{ action.assignee }}</span>
                  <span class="text-blue-600 font-medium">{{ action.dueDate }}</span>
                </div>
              </div>
            </div>
          </div>

          <!-- Next Steps -->
          <div class="bg-gray-50 rounded-xl p-4">
            <h5 class="font-medium text-gray-900 mb-2 flex items-center gap-2">
              <i class="fas fa-arrow-right text-gray-500"></i> Next Steps
            </h5>
            <p class="text-gray-700">{{ meetingSummary.nextSteps }}</p>
          </div>
        </div>
      </div>

      <div class="p-4 border-t border-gray-100 flex justify-end gap-3">
        <button @click="generateMeetingSummary"
                class="px-4 py-2 text-teal-600 hover:bg-teal-50 rounded-lg transition-colors flex items-center gap-2">
          <i class="fas fa-rotate"></i> Regenerate
        </button>
        <button @click="showMeetingSummaryModal = false"
                class="px-4 py-2 bg-teal-500 text-white rounded-lg hover:bg-teal-600 transition-colors">
          Done
        </button>
      </div>
    </div>
  </div>

  <!-- AI Sentiment Analysis Panel -->
  <div v-if="showSentimentPanel" class="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
    <div class="bg-white rounded-2xl shadow-2xl w-full max-w-lg overflow-hidden">
      <div class="p-6 border-b border-gray-100">
        <div class="flex items-center justify-between">
          <div class="flex items-center gap-3">
            <div class="w-10 h-10 bg-gradient-to-br from-purple-500 to-pink-500 rounded-xl flex items-center justify-center">
              <i class="fas fa-face-smile text-white"></i>
            </div>
            <div>
              <h3 class="text-lg font-semibold text-gray-900">Conversation Sentiment</h3>
              <p class="text-sm text-gray-500">AI-powered sentiment analysis</p>
            </div>
          </div>
          <button @click="showSentimentPanel = false" class="p-2 hover:bg-gray-100 rounded-lg transition-colors">
            <i class="fas fa-times text-gray-400"></i>
          </button>
        </div>
      </div>

      <div class="p-6">
        <AILoadingIndicator v-if="isAnalyzingSentiment" message="Analyzing conversation sentiment..." />

        <div v-else-if="conversationSentiment" class="space-y-6">
          <!-- Overall Sentiment -->
          <div class="text-center">
            <div class="inline-flex items-center justify-center w-20 h-20 rounded-full mb-3"
                 :class="{
                   'bg-green-100': conversationSentiment.overall === 'positive',
                   'bg-yellow-100': conversationSentiment.overall === 'neutral',
                   'bg-red-100': conversationSentiment.overall === 'negative'
                 }">
              <i class="text-4xl"
                 :class="{
                   'fas fa-face-smile text-green-500': conversationSentiment.overall === 'positive',
                   'fas fa-face-meh text-yellow-500': conversationSentiment.overall === 'neutral',
                   'fas fa-face-frown text-red-500': conversationSentiment.overall === 'negative'
                 }"></i>
            </div>
            <h4 class="text-xl font-semibold text-gray-900 capitalize">{{ conversationSentiment.overall }}</h4>
            <p class="text-gray-500">Overall conversation tone</p>
          </div>

          <!-- Confidence Score -->
          <div class="bg-gray-50 rounded-xl p-4">
            <div class="flex justify-between items-center mb-2">
              <span class="text-sm font-medium text-gray-700">Confidence Score</span>
              <span class="text-sm font-bold text-teal-600">{{ Math.round(conversationSentiment.score * 100) }}%</span>
            </div>
            <div class="w-full bg-gray-200 rounded-full h-2">
              <div class="bg-gradient-to-r from-teal-500 to-emerald-500 h-2 rounded-full transition-all duration-500"
                   :style="{ width: `${conversationSentiment.score * 100}%` }"></div>
            </div>
          </div>

          <!-- Emotion Breakdown -->
          <div>
            <h5 class="font-medium text-gray-900 mb-3">Emotion Breakdown</h5>
            <div class="space-y-3">
              <div v-for="emotion in conversationSentiment.emotions" :key="emotion.name" class="flex items-center gap-3">
                <span class="w-24 text-sm text-gray-600 capitalize">{{ emotion.name }}</span>
                <div class="flex-1 bg-gray-200 rounded-full h-2">
                  <div class="h-2 rounded-full transition-all duration-500"
                       :class="{
                         'bg-green-500': emotion.name === 'enthusiasm' || emotion.name === 'satisfaction',
                         'bg-blue-500': emotion.name === 'engagement' || emotion.name === 'curiosity',
                         'bg-purple-500': emotion.name === 'professionalism',
                         'bg-yellow-500': emotion.name === 'concern' || emotion.name === 'uncertainty'
                       }"
                       :style="{ width: `${emotion.score * 100}%` }"></div>
                </div>
                <span class="text-sm font-medium text-gray-700 w-12 text-right">{{ Math.round(emotion.score * 100) }}%</span>
              </div>
            </div>
          </div>

          <!-- Key Topics -->
          <div>
            <h5 class="font-medium text-gray-900 mb-3">Discussion Topics</h5>
            <div class="flex flex-wrap gap-2">
              <span v-for="topic in conversationSentiment.topics" :key="topic"
                    class="px-3 py-1 bg-teal-100 text-teal-700 rounded-full text-sm">
                {{ topic }}
              </span>
            </div>
          </div>

          <!-- Summary -->
          <div class="bg-gradient-to-r from-purple-50 to-pink-50 rounded-xl p-4">
            <h5 class="font-medium text-gray-900 mb-2 flex items-center gap-2">
              <i class="fas fa-lightbulb text-purple-500"></i> Summary
            </h5>
            <p class="text-gray-700 text-sm">{{ conversationSentiment.summary }}</p>
          </div>
        </div>
      </div>

      <div class="p-4 border-t border-gray-100 flex justify-end gap-3">
        <button @click="analyzeConversationSentiment"
                class="px-4 py-2 text-purple-600 hover:bg-purple-50 rounded-lg transition-colors flex items-center gap-2">
          <i class="fas fa-rotate"></i> Refresh
        </button>
        <button @click="showSentimentPanel = false"
                class="px-4 py-2 bg-purple-500 text-white rounded-lg hover:bg-purple-600 transition-colors">
          Close
        </button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.collaboration-hub {
  background: linear-gradient(135deg, #f8fafc 0%, #f1f5f9 100%);
}

.custom-scrollbar::-webkit-scrollbar {
  width: 6px;
}

.custom-scrollbar::-webkit-scrollbar-track {
  background: transparent;
}

.custom-scrollbar::-webkit-scrollbar-thumb {
  background: #cbd5e1;
  border-radius: 3px;
}

.custom-scrollbar::-webkit-scrollbar-thumb:hover {
  background: #94a3b8;
}

textarea {
  field-sizing: content;
  min-height: 44px;
}

.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.line-clamp-3 {
  display: -webkit-box;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

@keyframes bounce {
  0%, 60%, 100% { transform: translateY(0); }
  30% { transform: translateY(-4px); }
}

/* AI Feature Styles */
.ai-suggestions-panel {
  animation: slideInUp 0.3s ease-out;
}

@keyframes slideInUp {
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.ai-action-btn {
  transition: all 0.2s ease;
}

.ai-action-btn:hover {
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.2);
}

.ai-modal-enter-active {
  animation: modalFadeIn 0.3s ease-out;
}

.ai-modal-leave-active {
  animation: modalFadeIn 0.3s ease-out reverse;
}

@keyframes modalFadeIn {
  from {
    opacity: 0;
    transform: scale(0.95);
  }
  to {
    opacity: 1;
    transform: scale(1);
  }
}

.ai-suggestion-chip {
  transition: all 0.2s ease;
}

.ai-suggestion-chip:hover {
  transform: scale(1.02);
}

.ai-pulse {
  animation: aiPulse 2s ease-in-out infinite;
}

@keyframes aiPulse {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.7; }
}

.ai-gradient-text {
  background: linear-gradient(135deg, #14b8a6 0%, #10b981 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.sentiment-positive { color: #22c55e; }
.sentiment-neutral { color: #eab308; }
.sentiment-negative { color: #ef4444; }
</style>
