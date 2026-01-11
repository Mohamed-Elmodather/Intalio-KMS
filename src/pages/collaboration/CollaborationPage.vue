<script setup lang="ts">
import { ref, computed, nextTick, watch } from 'vue'

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
const showCreateChannelModal = ref(false)
const showContentPicker = ref(false)
const showEmojiPicker = ref(false)
const activeEmojiMessageId = ref<string | null>(null)
const showMentionAutocomplete = ref(false)
const mentionQuery = ref('')
const expandedThreadId = ref<string | null>(null)
const threadReplyInput = ref('')
const rightPanelTab = ref<'details' | 'members' | 'files' | 'pinned'>('details')
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
</script>

<template>
  <div class="collaboration-hub flex h-[calc(100vh-64px)] overflow-hidden">
    <!-- Left Sidebar -->
    <aside class="w-64 bg-slate-900 text-white flex flex-col flex-shrink-0">
      <!-- Workspace Header -->
      <div class="p-4 border-b border-slate-700">
        <div class="flex items-center justify-between">
          <div class="flex items-center gap-3">
            <div class="w-9 h-9 bg-teal-500 rounded-lg flex items-center justify-center font-bold text-sm">
              AFC
            </div>
            <div>
              <h2 class="font-semibold text-sm">AFC 2027</h2>
              <span class="text-xs text-slate-400 flex items-center gap-1">
                <span class="w-2 h-2 bg-emerald-500 rounded-full"></span>
                {{ channelMembers.length }} online
              </span>
            </div>
          </div>
          <button class="p-1.5 hover:bg-slate-700 rounded-lg transition-colors">
            <i class="fas fa-ellipsis-v text-slate-400"></i>
          </button>
        </div>
      </div>

      <!-- Search -->
      <div class="p-3">
        <div class="relative">
          <i class="fas fa-search absolute left-3 top-1/2 -translate-y-1/2 text-slate-500 text-sm"></i>
          <input
            v-model="searchQuery"
            type="text"
            placeholder="Search..."
            class="w-full bg-slate-800 text-sm text-white placeholder-slate-500 rounded-lg pl-9 pr-4 py-2 focus:outline-none focus:ring-2 focus:ring-teal-500 border border-slate-700"
          />
        </div>
      </div>

      <!-- Channels & DMs -->
      <div class="flex-1 overflow-y-auto custom-scrollbar">
        <!-- Channels Section -->
        <div class="px-3 py-2">
          <div class="flex items-center justify-between mb-2">
            <button class="flex items-center gap-2 text-xs font-semibold text-slate-400 hover:text-white transition-colors">
              <i class="fas fa-chevron-down text-[10px]"></i>
              <span>CHANNELS</span>
            </button>
            <button
              @click="showCreateChannelModal = true"
              class="p-1 hover:bg-slate-700 rounded transition-colors"
              title="Create Channel"
            >
              <i class="fas fa-plus text-slate-400 hover:text-white text-xs"></i>
            </button>
          </div>

          <div class="space-y-0.5">
            <button
              v-for="channel in filteredChannels"
              :key="channel.id"
              @click="selectChannel(channel.id)"
              :class="[
                'w-full flex items-center gap-2 px-2 py-1.5 rounded-lg text-sm transition-all',
                selectedChannelId === channel.id && !selectedDMId
                  ? 'bg-teal-500/20 text-teal-400'
                  : 'text-slate-300 hover:bg-slate-800 hover:text-white'
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
              <i v-if="channel.isMuted" class="fas fa-bell-slash text-slate-500 text-xs"></i>
            </button>
          </div>
        </div>

        <!-- Teams Section -->
        <div class="px-3 py-2">
          <button class="flex items-center gap-2 text-xs font-semibold text-slate-400 hover:text-white transition-colors mb-2">
            <i class="fas fa-chevron-down text-[10px]"></i>
            <span>TEAMS</span>
          </button>

          <div class="space-y-1">
            <div v-for="team in teams" :key="team.id">
              <!-- Team Header -->
              <button
                @click="toggleTeam(team.id)"
                class="w-full flex items-center gap-2 px-2 py-1.5 rounded-lg text-sm text-slate-300 hover:bg-slate-800 hover:text-white transition-all"
              >
                <i :class="[
                  'fas text-[10px] w-3 transition-transform',
                  isTeamExpanded(team.id) ? 'fa-chevron-down' : 'fa-chevron-right'
                ]"></i>
                <div
                  class="w-5 h-5 rounded flex items-center justify-center text-white text-[10px] font-bold"
                  :style="{ backgroundColor: team.color }"
                >
                  {{ team.name.substring(0, 2).toUpperCase() }}
                </div>
                <span class="flex-1 text-left truncate">{{ team.name }}</span>
                <span
                  v-if="getTeamUnreadCount(team) > 0"
                  class="bg-teal-500 text-white text-xs font-bold px-1.5 py-0.5 rounded-full min-w-[20px] text-center"
                >
                  {{ getTeamUnreadCount(team) }}
                </span>
                <span v-else class="text-xs text-slate-500">{{ team.memberCount }}</span>
              </button>

              <!-- Team Channels (expanded) -->
              <div v-if="isTeamExpanded(team.id)" class="ml-5 mt-0.5 space-y-0.5">
                <button
                  v-for="channel in team.channels"
                  :key="channel.id"
                  @click="selectTeamChannel(team.id, channel.id)"
                  :class="[
                    'w-full flex items-center gap-2 px-2 py-1 rounded-lg text-sm transition-all',
                    selectedChannelId === channel.id
                      ? 'bg-teal-500/20 text-teal-400'
                      : 'text-slate-400 hover:bg-slate-800 hover:text-white'
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
        <div class="px-3 py-2">
          <button class="flex items-center gap-2 text-xs font-semibold text-slate-400 hover:text-white transition-colors mb-2">
            <i class="fas fa-chevron-down text-[10px]"></i>
            <span>DIRECT MESSAGES</span>
          </button>

          <div class="space-y-0.5">
            <button
              v-for="dm in filteredDMs"
              :key="dm.id"
              @click="selectDM(dm.id)"
              :class="[
                'w-full flex items-center gap-2 px-2 py-1.5 rounded-lg text-sm transition-all',
                selectedDMId === dm.id
                  ? 'bg-teal-500/20 text-teal-400'
                  : 'text-slate-300 hover:bg-slate-800 hover:text-white'
              ]"
            >
              <div class="relative">
                <div class="w-6 h-6 bg-slate-600 rounded-full flex items-center justify-center text-xs font-medium">
                  {{ dm.user.displayName.split(' ').map(n => n[0]).join('') }}
                </div>
                <span
                  :class="['absolute -bottom-0.5 -right-0.5 w-2.5 h-2.5 rounded-full border-2 border-slate-900', getPresenceColor(dm.presence)]"
                ></span>
              </div>
              <span class="flex-1 text-left truncate">{{ dm.user.displayName }}</span>
              <span
                v-if="dm.unreadCount > 0"
                class="bg-teal-500 text-white text-xs font-bold px-1.5 py-0.5 rounded-full min-w-[20px] text-center"
              >
                {{ dm.unreadCount }}
              </span>
            </button>
          </div>
        </div>
      </div>

      <!-- User Footer -->
      <div class="p-3 border-t border-slate-700">
        <div class="flex items-center gap-3">
          <div class="relative">
            <div class="w-9 h-9 bg-teal-600 rounded-full flex items-center justify-center font-medium">
              YO
            </div>
            <span class="absolute bottom-0 right-0 w-3 h-3 bg-emerald-500 rounded-full border-2 border-slate-900"></span>
          </div>
          <div class="flex-1 min-w-0">
            <p class="text-sm font-medium truncate">You</p>
            <p class="text-xs text-slate-400 truncate">Online</p>
          </div>
          <button class="p-1.5 hover:bg-slate-700 rounded-lg transition-colors">
            <i class="fas fa-cog text-slate-400"></i>
          </button>
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

    <!-- Right Panel - Details -->
    <aside
      v-if="showRightPanel"
      class="w-80 border-l border-gray-200 bg-gray-50 flex flex-col flex-shrink-0"
    >
      <!-- Panel Header -->
      <div class="h-14 px-4 border-b border-gray-200 flex items-center justify-between bg-white">
        <h3 class="font-semibold text-gray-900">Details</h3>
        <button
          @click="showRightPanel = false"
          class="p-1.5 hover:bg-gray-100 rounded-lg transition-colors"
        >
          <i class="fas fa-times text-gray-500"></i>
        </button>
      </div>

      <!-- Panel Tabs -->
      <div class="px-4 py-2 border-b border-gray-200 bg-white">
        <div class="flex gap-1">
          <button
            v-for="tab in ['details', 'members', 'files', 'pinned'] as const"
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

      <!-- Panel Content -->
      <div class="flex-1 overflow-y-auto custom-scrollbar p-4">
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

        <!-- Files Tab -->
        <div v-if="rightPanelTab === 'files'" class="space-y-2">
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
</style>
