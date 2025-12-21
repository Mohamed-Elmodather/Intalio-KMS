<script setup lang="ts">
import { useI18n } from 'vue-i18n'
import { useRouter } from 'vue-router'
import Button from 'primevue/button'

const { t } = useI18n()
const router = useRouter()

function goHome() {
  router.push('/dashboard')
}

function goBack() {
  router.back()
}
</script>

<template>
  <div class="not-found-view">
    <div class="content">
      <div class="error-code">404</div>
      <h1>{{ t('errors.notFound') }}</h1>
      <p class="text-color-secondary text-lg">{{ t('errors.notFoundDescription') }}</p>

      <div class="flex gap-3 justify-content-center mt-5">
        <Button
          :label="t('common.goBack')"
          icon="pi pi-arrow-left"
          severity="secondary"
          @click="goBack"
        />
        <Button
          :label="t('common.goHome')"
          icon="pi pi-home"
          @click="goHome"
        />
      </div>
    </div>
  </div>
</template>

<style scoped>
.not-found-view {
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 100vh;
  background: var(--surface-ground);
  text-align: center;
  padding: 2rem;
}

.error-code {
  font-size: 8rem;
  font-weight: 700;
  color: var(--primary-color);
  line-height: 1;
  margin-bottom: 1rem;
}

h1 {
  font-size: 2rem;
  margin-bottom: 0.5rem;
}
</style>
