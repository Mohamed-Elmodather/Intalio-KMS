import { ref, computed, onUnmounted, watch } from 'vue'
import * as signalR from '@microsoft/signalr'
import { useAuthStore } from '@/stores/auth'

export interface Participant {
  userId: string
  userName: string
  avatarUrl?: string
  color: string
  isActive: boolean
  cursor?: CursorPosition
}

export interface CursorPosition {
  blockId?: string
  position?: number
}

export interface SelectionRange {
  startBlockId: string
  startOffset: number
  endBlockId: string
  endOffset: number
}

export interface AwarenessState {
  isTyping: boolean
  focusedBlockId?: string
  status: 'editing' | 'viewing' | 'idle'
}

export interface CollaborationState {
  isConnected: boolean
  sessionId: string
  participants: Participant[]
  cursors: Map<string, CursorPosition>
  selections: Map<string, SelectionRange>
  awareness: Map<string, AwarenessState>
}

export function useCollaboration(articleId: string) {
  const authStore = useAuthStore()

  const connection = ref<signalR.HubConnection | null>(null)
  const state = ref<CollaborationState>({
    isConnected: false,
    sessionId: '',
    participants: [],
    cursors: new Map(),
    selections: new Map(),
    awareness: new Map()
  })

  const isConnected = computed(() => state.value.isConnected)
  const participants = computed(() => state.value.participants)
  const otherParticipants = computed(() =>
    state.value.participants.filter(p => p.userId !== authStore.user?.id)
  )

  // Callbacks for external handling
  let onSyncUpdateCallback: ((update: Uint8Array) => void) | null = null
  let onStateSyncedCallback: ((state: Uint8Array | null) => void) | null = null

  const connect = async () => {
    if (connection.value?.state === signalR.HubConnectionState.Connected) {
      return
    }

    const hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('/hubs/collaboration', {
        accessTokenFactory: () => authStore.accessToken || ''
      })
      .withAutomaticReconnect([0, 1000, 5000, 10000, 30000])
      .configureLogging(signalR.LogLevel.Information)
      .build()

    // Setup event handlers
    setupEventHandlers(hubConnection)

    try {
      await hubConnection.start()
      connection.value = hubConnection
      state.value.isConnected = true

      // Join the article session
      await hubConnection.invoke('JoinSession', articleId)
      console.log('Connected to collaboration hub')
    } catch (error) {
      console.error('Failed to connect to collaboration hub:', error)
      throw error
    }
  }

  const disconnect = async () => {
    if (connection.value) {
      try {
        await connection.value.invoke('LeaveSession', articleId)
        await connection.value.stop()
      } catch (error) {
        console.error('Error disconnecting:', error)
      } finally {
        connection.value = null
        state.value.isConnected = false
        state.value.sessionId = ''
        state.value.participants = []
        state.value.cursors.clear()
        state.value.selections.clear()
        state.value.awareness.clear()
      }
    }
  }

  const setupEventHandlers = (hubConnection: signalR.HubConnection) => {
    // Session joined
    hubConnection.on('SessionJoined', (message: {
      articleId: string
      sessionId: string
      state: string | null
      participants: Participant[]
    }) => {
      state.value.sessionId = message.sessionId
      state.value.participants = message.participants

      if (message.state && onStateSyncedCallback) {
        const stateBytes = base64ToUint8Array(message.state)
        onStateSyncedCallback(stateBytes)
      }
    })

    // Participant joined
    hubConnection.on('ParticipantJoined', (message: {
      userId: string
      userName: string
      color: string
      avatarUrl?: string
    }) => {
      const existingIndex = state.value.participants.findIndex(p => p.userId === message.userId)
      if (existingIndex === -1) {
        state.value.participants.push({
          userId: message.userId,
          userName: message.userName,
          color: message.color,
          avatarUrl: message.avatarUrl,
          isActive: true
        })
      } else {
        state.value.participants[existingIndex].isActive = true
      }
    })

    // Participant left
    hubConnection.on('ParticipantLeft', (message: { userId: string }) => {
      const index = state.value.participants.findIndex(p => p.userId === message.userId)
      if (index !== -1) {
        state.value.participants[index].isActive = false
      }
      state.value.cursors.delete(message.userId)
      state.value.selections.delete(message.userId)
      state.value.awareness.delete(message.userId)
    })

    // CRDT sync update
    hubConnection.on('SyncUpdate', (message: {
      articleId: string
      update: string
      senderId: string
    }) => {
      if (onSyncUpdateCallback) {
        const updateBytes = base64ToUint8Array(message.update)
        onSyncUpdateCallback(updateBytes)
      }
    })

    // Cursor updated
    hubConnection.on('CursorUpdated', (message: {
      userId: string
      blockId?: string
      position?: number
    }) => {
      state.value.cursors.set(message.userId, {
        blockId: message.blockId,
        position: message.position
      })

      // Update participant cursor
      const participant = state.value.participants.find(p => p.userId === message.userId)
      if (participant) {
        participant.cursor = {
          blockId: message.blockId,
          position: message.position
        }
      }
    })

    // Selection updated
    hubConnection.on('SelectionUpdated', (message: {
      userId: string
      selection?: SelectionRange
    }) => {
      if (message.selection) {
        state.value.selections.set(message.userId, message.selection)
      } else {
        state.value.selections.delete(message.userId)
      }
    })

    // Awareness updated
    hubConnection.on('AwarenessUpdated', (message: {
      userId: string
      awareness: AwarenessState
    }) => {
      state.value.awareness.set(message.userId, message.awareness)
    })

    // State synced (for recovery)
    hubConnection.on('StateSynced', (message: {
      articleId: string
      state: string | null
      participants: Participant[]
    }) => {
      state.value.participants = message.participants
      if (message.state && onStateSyncedCallback) {
        const stateBytes = base64ToUint8Array(message.state)
        onStateSyncedCallback(stateBytes)
      }
    })

    // Reconnection handling
    hubConnection.onreconnecting(() => {
      state.value.isConnected = false
      console.log('Reconnecting to collaboration hub...')
    })

    hubConnection.onreconnected(() => {
      state.value.isConnected = true
      console.log('Reconnected to collaboration hub')
      // Rejoin session and request state sync
      hubConnection.invoke('JoinSession', articleId)
    })

    hubConnection.onclose(() => {
      state.value.isConnected = false
      console.log('Disconnected from collaboration hub')
    })
  }

  // Send CRDT update to other participants
  const syncUpdate = async (update: Uint8Array) => {
    if (connection.value?.state === signalR.HubConnectionState.Connected) {
      const base64Update = uint8ArrayToBase64(update)
      await connection.value.invoke('SyncUpdate', articleId, base64Update)
    }
  }

  // Update cursor position
  const updateCursor = async (blockId?: string, position?: number) => {
    if (connection.value?.state === signalR.HubConnectionState.Connected) {
      await connection.value.invoke('UpdateCursor', articleId, blockId, position)
    }
  }

  // Update selection
  const updateSelection = async (selection?: SelectionRange) => {
    if (connection.value?.state === signalR.HubConnectionState.Connected) {
      await connection.value.invoke('UpdateSelection', articleId, selection)
    }
  }

  // Update awareness state
  const updateAwareness = async (awareness: AwarenessState) => {
    if (connection.value?.state === signalR.HubConnectionState.Connected) {
      await connection.value.invoke('UpdateAwareness', articleId, awareness)
    }
  }

  // Request full state sync (for recovery)
  const requestStateSync = async () => {
    if (connection.value?.state === signalR.HubConnectionState.Connected) {
      await connection.value.invoke('RequestStateSync', articleId)
    }
  }

  // Set callback for receiving sync updates
  const onSyncUpdate = (callback: (update: Uint8Array) => void) => {
    onSyncUpdateCallback = callback
  }

  // Set callback for receiving full state
  const onStateSynced = (callback: (state: Uint8Array | null) => void) => {
    onStateSyncedCallback = callback
  }

  // Get cursor color for a user
  const getCursorColor = (userId: string): string => {
    const participant = state.value.participants.find(p => p.userId === userId)
    return participant?.color || '#888888'
  }

  // Utility functions
  const base64ToUint8Array = (base64: string): Uint8Array => {
    const binaryString = atob(base64)
    const bytes = new Uint8Array(binaryString.length)
    for (let i = 0; i < binaryString.length; i++) {
      bytes[i] = binaryString.charCodeAt(i)
    }
    return bytes
  }

  const uint8ArrayToBase64 = (bytes: Uint8Array): string => {
    let binary = ''
    for (let i = 0; i < bytes.length; i++) {
      binary += String.fromCharCode(bytes[i])
    }
    return btoa(binary)
  }

  // Cleanup on unmount
  onUnmounted(() => {
    disconnect()
  })

  return {
    // State
    state,
    isConnected,
    participants,
    otherParticipants,

    // Methods
    connect,
    disconnect,
    syncUpdate,
    updateCursor,
    updateSelection,
    updateAwareness,
    requestStateSync,

    // Callbacks
    onSyncUpdate,
    onStateSynced,

    // Helpers
    getCursorColor
  }
}
