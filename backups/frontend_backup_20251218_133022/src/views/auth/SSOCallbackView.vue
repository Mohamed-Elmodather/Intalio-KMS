<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import ProgressSpinner from 'primevue/progressspinner'
import Card from 'primevue/card'
import Button from 'primevue/button'

const router = useRouter()
const route = useRoute()

const error = ref<string | null>(null)
const isProcessing = ref(true)

onMounted(async () => {
  try {
    const code = route.query.code as string
    const errorParam = route.query.error as string

    if (errorParam) {
      error.value = route.query.error_description as string || 'Authentication failed'
      isProcessing.value = false
      return
    }

    if (!code) {
      error.value = 'No authorization code received'
      isProcessing.value = false
      return
    }

    // TODO: Exchange code for tokens
    // await authStore.handleSSOCallback(code, state)

    // Simulate SSO processing
    await new Promise(resolve => setTimeout(resolve, 1500))

    // Redirect to intended destination or dashboard
    const redirectTo = sessionStorage.getItem('sso_redirect') || '/dashboard'
    sessionStorage.removeItem('sso_redirect')
    router.push(redirectTo)
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Authentication failed'
    isProcessing.value = false
  }
})

function retry() {
  router.push('/login')
}
</script>

<template>
  <div class="sso-callback-view">
    <Card class="callback-card">
      <template #content>
        <div v-if="isProcessing" class="text-center">
          <ProgressSpinner />
          <h3 class="mt-4">Completing Sign In...</h3>
          <p class="text-color-secondary">Please wait while we verify your credentials</p>
        </div>

        <div v-else-if="error" class="text-center">
          <i class="pi pi-times-circle text-red-500 text-6xl mb-4"></i>
          <h3 class="text-red-500">Authentication Failed</h3>
          <p class="text-color-secondary mb-4">{{ error }}</p>
          <Button label="Try Again" icon="pi pi-refresh" @click="retry" />
        </div>
      </template>
    </Card>
  </div>
</template>

<style scoped>
.sso-callback-view {
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 100vh;
  background: var(--surface-ground);
}

.callback-card {
  width: 400px;
  text-align: center;
}
</style>
