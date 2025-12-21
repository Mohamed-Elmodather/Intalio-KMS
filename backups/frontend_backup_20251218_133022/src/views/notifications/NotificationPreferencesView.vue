<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRouter } from 'vue-router'
import Card from 'primevue/card'
import Button from 'primevue/button'
import InputSwitch from 'primevue/inputswitch'
import Dropdown from 'primevue/dropdown'
import InputText from 'primevue/inputtext'
import Divider from 'primevue/divider'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import Tag from 'primevue/tag'
import Toast from 'primevue/toast'
import { useToast } from 'primevue/usetoast'

const { t, locale } = useI18n()
const router = useRouter()
const toast = useToast()

// State
const saving = ref(false)
const preferences = ref({
  notificationsEnabled: true,
  emailEnabled: true,
  pushEnabled: true,
  smsEnabled: false,
  inAppEnabled: true,
  emailDigestFrequency: 'Instant',
  digestHour: 9,
  timeZone: 'Asia/Riyadh',
  quietHoursEnabled: false,
  quietHoursStart: '22:00',
  quietHoursEnd: '07:00',
  allowUrgentDuringQuietHours: true,
  preferredLanguage: 'en'
})

const categoryPreferences = ref([
  {
    category: 'Workflow',
    categoryName: 'Workflow',
    categoryNameAr: 'سير العمل',
    enabled: true,
    channels: ['InApp', 'Email', 'Push']
  },
  {
    category: 'Content',
    categoryName: 'Content',
    categoryNameAr: 'المحتوى',
    enabled: true,
    channels: ['InApp', 'Email']
  },
  {
    category: 'Collaboration',
    categoryName: 'Collaboration',
    categoryNameAr: 'التعاون',
    enabled: true,
    channels: ['InApp']
  },
  {
    category: 'Calendar',
    categoryName: 'Calendar',
    categoryNameAr: 'التقويم',
    enabled: true,
    channels: ['InApp', 'Email', 'Push']
  },
  {
    category: 'System',
    categoryName: 'System',
    categoryNameAr: 'النظام',
    enabled: true,
    channels: ['InApp', 'Email']
  }
])

const devices = ref([
  {
    id: '1',
    platform: 'Web',
    deviceName: 'Chrome on Windows',
    isActive: true,
    lastActiveAt: new Date()
  },
  {
    id: '2',
    platform: 'iOS',
    deviceName: 'iPhone 14 Pro',
    isActive: true,
    lastActiveAt: new Date(Date.now() - 2 * 60 * 60 * 1000)
  }
])

// Options
const digestFrequencyOptions = [
  { label: t('notifications.preferences.digest.instant'), value: 'Instant' },
  { label: t('notifications.preferences.digest.daily'), value: 'Daily' },
  { label: t('notifications.preferences.digest.weekly'), value: 'Weekly' },
  { label: t('notifications.preferences.digest.none'), value: 'None' }
]

const digestHourOptions = Array.from({ length: 24 }, (_, i) => ({
  label: `${i.toString().padStart(2, '0')}:00`,
  value: i
}))

const timeZoneOptions = [
  { label: 'Asia/Riyadh (GMT+3)', value: 'Asia/Riyadh' },
  { label: 'Asia/Dubai (GMT+4)', value: 'Asia/Dubai' },
  { label: 'Europe/London (GMT+0)', value: 'Europe/London' },
  { label: 'America/New_York (GMT-5)', value: 'America/New_York' }
]

const languageOptions = [
  { label: 'English', value: 'en' },
  { label: 'العربية', value: 'ar' }
]

const channelOptions = ['InApp', 'Email', 'Push', 'SMS']

// Computed
const isRtl = computed(() => locale.value === 'ar')

// Methods
const getCategoryName = (category: any) => {
  return isRtl.value ? category.categoryNameAr : category.categoryName
}

const toggleCategoryChannel = (category: any, channel: string) => {
  const index = category.channels.indexOf(channel)
  if (index > -1) {
    category.channels.splice(index, 1)
  } else {
    category.channels.push(channel)
  }
}

const getChannelLabel = (channel: string) => {
  return t(`notifications.channels.${channel.toLowerCase()}`)
}

const getChannelIcon = (channel: string) => {
  switch (channel) {
    case 'InApp': return 'pi-bell'
    case 'Email': return 'pi-envelope'
    case 'Push': return 'pi-mobile'
    case 'SMS': return 'pi-phone'
    default: return 'pi-send'
  }
}

const getPlatformIcon = (platform: string) => {
  switch (platform) {
    case 'Web': return 'pi-globe'
    case 'iOS': return 'pi-apple'
    case 'Android': return 'pi-android'
    default: return 'pi-desktop'
  }
}

const formatDate = (date: Date) => {
  return new Intl.DateTimeFormat(locale.value, {
    dateStyle: 'medium',
    timeStyle: 'short'
  }).format(date)
}

const removeDevice = async (device: any) => {
  const index = devices.value.indexOf(device)
  if (index > -1) {
    devices.value.splice(index, 1)
    toast.add({
      severity: 'success',
      summary: t('common.success'),
      detail: t('notifications.preferences.deviceRemoved'),
      life: 3000
    })
  }
}

const savePreferences = async () => {
  saving.value = true
  try {
    // Simulate API call
    await new Promise(resolve => setTimeout(resolve, 1000))
    toast.add({
      severity: 'success',
      summary: t('common.success'),
      detail: t('notifications.preferences.saved'),
      life: 3000
    })
  } finally {
    saving.value = false
  }
}

const resetPreferences = async () => {
  // Reset to defaults
  preferences.value = {
    notificationsEnabled: true,
    emailEnabled: true,
    pushEnabled: true,
    smsEnabled: false,
    inAppEnabled: true,
    emailDigestFrequency: 'Instant',
    digestHour: 9,
    timeZone: 'Asia/Riyadh',
    quietHoursEnabled: false,
    quietHoursStart: '22:00',
    quietHoursEnd: '07:00',
    allowUrgentDuringQuietHours: true,
    preferredLanguage: 'en'
  }
  toast.add({
    severity: 'info',
    summary: t('common.info'),
    detail: t('notifications.preferences.reset'),
    life: 3000
  })
}

const goBack = () => {
  router.push('/notifications')
}

// Lifecycle
onMounted(() => {
  // Load preferences from API
})
</script>

<template>
  <div class="notification-preferences">
    <Toast />

    <!-- Header -->
    <div class="preferences-header">
      <div class="header-title">
        <Button
          icon="pi pi-arrow-left"
          class="p-button-text p-button-rounded"
          @click="goBack"
        />
        <h1>{{ t('notifications.preferences.title') }}</h1>
      </div>
      <div class="header-actions">
        <Button
          :label="t('notifications.preferences.reset')"
          class="p-button-outlined"
          @click="resetPreferences"
        />
        <Button
          :label="t('common.save')"
          icon="pi pi-check"
          :loading="saving"
          @click="savePreferences"
        />
      </div>
    </div>

    <!-- General Settings -->
    <Card class="settings-card">
      <template #title>
        <div class="card-title">
          <i class="pi pi-cog"></i>
          {{ t('notifications.preferences.general') }}
        </div>
      </template>
      <template #content>
        <div class="settings-grid">
          <div class="setting-item">
            <div class="setting-info">
              <label>{{ t('notifications.preferences.enableNotifications') }}</label>
              <small>{{ t('notifications.preferences.enableNotificationsDesc') }}</small>
            </div>
            <InputSwitch v-model="preferences.notificationsEnabled" />
          </div>

          <Divider />

          <div class="setting-item">
            <div class="setting-info">
              <label>{{ t('notifications.preferences.language') }}</label>
              <small>{{ t('notifications.preferences.languageDesc') }}</small>
            </div>
            <Dropdown
              v-model="preferences.preferredLanguage"
              :options="languageOptions"
              option-label="label"
              option-value="value"
              class="w-12rem"
            />
          </div>

          <div class="setting-item">
            <div class="setting-info">
              <label>{{ t('notifications.preferences.timeZone') }}</label>
              <small>{{ t('notifications.preferences.timeZoneDesc') }}</small>
            </div>
            <Dropdown
              v-model="preferences.timeZone"
              :options="timeZoneOptions"
              option-label="label"
              option-value="value"
              class="w-15rem"
            />
          </div>
        </div>
      </template>
    </Card>

    <!-- Delivery Channels -->
    <Card class="settings-card">
      <template #title>
        <div class="card-title">
          <i class="pi pi-send"></i>
          {{ t('notifications.preferences.channels') }}
        </div>
      </template>
      <template #content>
        <div class="settings-grid">
          <div class="setting-item">
            <div class="setting-info">
              <label>
                <i class="pi pi-bell me-2"></i>
                {{ t('notifications.channels.inapp') }}
              </label>
              <small>{{ t('notifications.preferences.inAppDesc') }}</small>
            </div>
            <InputSwitch v-model="preferences.inAppEnabled" />
          </div>

          <div class="setting-item">
            <div class="setting-info">
              <label>
                <i class="pi pi-envelope me-2"></i>
                {{ t('notifications.channels.email') }}
              </label>
              <small>{{ t('notifications.preferences.emailDesc') }}</small>
            </div>
            <InputSwitch v-model="preferences.emailEnabled" />
          </div>

          <div class="setting-item">
            <div class="setting-info">
              <label>
                <i class="pi pi-mobile me-2"></i>
                {{ t('notifications.channels.push') }}
              </label>
              <small>{{ t('notifications.preferences.pushDesc') }}</small>
            </div>
            <InputSwitch v-model="preferences.pushEnabled" />
          </div>

          <div class="setting-item">
            <div class="setting-info">
              <label>
                <i class="pi pi-phone me-2"></i>
                {{ t('notifications.channels.sms') }}
              </label>
              <small>{{ t('notifications.preferences.smsDesc') }}</small>
            </div>
            <InputSwitch v-model="preferences.smsEnabled" />
          </div>
        </div>
      </template>
    </Card>

    <!-- Email Digest -->
    <Card class="settings-card">
      <template #title>
        <div class="card-title">
          <i class="pi pi-inbox"></i>
          {{ t('notifications.preferences.emailDigest') }}
        </div>
      </template>
      <template #content>
        <div class="settings-grid">
          <div class="setting-item">
            <div class="setting-info">
              <label>{{ t('notifications.preferences.digestFrequency') }}</label>
              <small>{{ t('notifications.preferences.digestFrequencyDesc') }}</small>
            </div>
            <Dropdown
              v-model="preferences.emailDigestFrequency"
              :options="digestFrequencyOptions"
              option-label="label"
              option-value="value"
              class="w-10rem"
            />
          </div>

          <div
            v-if="preferences.emailDigestFrequency === 'Daily'"
            class="setting-item"
          >
            <div class="setting-info">
              <label>{{ t('notifications.preferences.digestTime') }}</label>
              <small>{{ t('notifications.preferences.digestTimeDesc') }}</small>
            </div>
            <Dropdown
              v-model="preferences.digestHour"
              :options="digestHourOptions"
              option-label="label"
              option-value="value"
              class="w-8rem"
            />
          </div>
        </div>
      </template>
    </Card>

    <!-- Quiet Hours -->
    <Card class="settings-card">
      <template #title>
        <div class="card-title">
          <i class="pi pi-moon"></i>
          {{ t('notifications.preferences.quietHours') }}
        </div>
      </template>
      <template #content>
        <div class="settings-grid">
          <div class="setting-item">
            <div class="setting-info">
              <label>{{ t('notifications.preferences.enableQuietHours') }}</label>
              <small>{{ t('notifications.preferences.quietHoursDesc') }}</small>
            </div>
            <InputSwitch v-model="preferences.quietHoursEnabled" />
          </div>

          <template v-if="preferences.quietHoursEnabled">
            <div class="setting-item">
              <div class="setting-info">
                <label>{{ t('notifications.preferences.quietHoursStart') }}</label>
              </div>
              <InputText
                v-model="preferences.quietHoursStart"
                type="time"
                class="w-8rem"
              />
            </div>

            <div class="setting-item">
              <div class="setting-info">
                <label>{{ t('notifications.preferences.quietHoursEnd') }}</label>
              </div>
              <InputText
                v-model="preferences.quietHoursEnd"
                type="time"
                class="w-8rem"
              />
            </div>

            <div class="setting-item">
              <div class="setting-info">
                <label>{{ t('notifications.preferences.allowUrgent') }}</label>
                <small>{{ t('notifications.preferences.allowUrgentDesc') }}</small>
              </div>
              <InputSwitch v-model="preferences.allowUrgentDuringQuietHours" />
            </div>
          </template>
        </div>
      </template>
    </Card>

    <!-- Category Preferences -->
    <Card class="settings-card">
      <template #title>
        <div class="card-title">
          <i class="pi pi-tags"></i>
          {{ t('notifications.preferences.categoryPreferences') }}
        </div>
      </template>
      <template #content>
        <DataTable :value="categoryPreferences" class="category-table">
          <Column field="category" :header="t('notifications.preferences.category')">
            <template #body="{ data }">
              <div class="category-name">
                <InputSwitch v-model="data.enabled" class="me-3" />
                <span :class="{ 'text-muted': !data.enabled }">
                  {{ getCategoryName(data) }}
                </span>
              </div>
            </template>
          </Column>
          <Column :header="t('notifications.preferences.deliveryChannels')">
            <template #body="{ data }">
              <div class="channel-toggles" :class="{ disabled: !data.enabled }">
                <div
                  v-for="channel in channelOptions"
                  :key="channel"
                  class="channel-toggle"
                  :class="{ active: data.channels.includes(channel) }"
                  @click="data.enabled && toggleCategoryChannel(data, channel)"
                >
                  <i :class="'pi ' + getChannelIcon(channel)"></i>
                  <span>{{ getChannelLabel(channel) }}</span>
                </div>
              </div>
            </template>
          </Column>
        </DataTable>
      </template>
    </Card>

    <!-- Registered Devices -->
    <Card class="settings-card">
      <template #title>
        <div class="card-title">
          <i class="pi pi-mobile"></i>
          {{ t('notifications.preferences.devices') }}
        </div>
      </template>
      <template #content>
        <div v-if="devices.length === 0" class="empty-devices">
          <i class="pi pi-mobile"></i>
          <p>{{ t('notifications.preferences.noDevices') }}</p>
          <Button
            :label="t('notifications.preferences.registerDevice')"
            icon="pi pi-plus"
            class="p-button-outlined"
          />
        </div>

        <div v-else class="devices-list">
          <div
            v-for="device in devices"
            :key="device.id"
            class="device-item"
          >
            <div class="device-icon">
              <i :class="'pi ' + getPlatformIcon(device.platform)"></i>
            </div>
            <div class="device-info">
              <div class="device-name">{{ device.deviceName }}</div>
              <div class="device-meta">
                <Tag :value="device.platform" severity="info" />
                <span>
                  {{ t('notifications.preferences.lastActive') }}:
                  {{ formatDate(device.lastActiveAt) }}
                </span>
              </div>
            </div>
            <div class="device-actions">
              <Tag
                v-if="device.isActive"
                :value="t('notifications.preferences.active')"
                severity="success"
              />
              <Button
                icon="pi pi-trash"
                class="p-button-rounded p-button-text p-button-danger"
                @click="removeDevice(device)"
              />
            </div>
          </div>
        </div>
      </template>
    </Card>
  </div>
</template>

<style scoped lang="scss">
@use '@/assets/styles/_variables.scss' as *;
@use '@/assets/styles/_mixins.scss' as *;

.notification-preferences {
  @include page-view;
  padding: $spacing-6;
  max-width: 900px;
  margin: 0 auto;
}

.preferences-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: $spacing-6;
  flex-wrap: wrap;
  gap: $spacing-4;

  .header-title {
    display: flex;
    align-items: center;
    gap: $spacing-2;

    h1 {
      margin: 0;
      font-size: $font-size-2xl;
      font-weight: $font-weight-semibold;
      color: $slate-900;
    }
  }

  .header-actions {
    display: flex;
    gap: $spacing-3;
  }
}

.settings-card {
  margin-bottom: $spacing-6;

  .card-title {
    display: flex;
    align-items: center;
    gap: $spacing-2;
    font-size: $font-size-lg;

    i {
      color: $intalio-teal-500;
    }
  }
}

.settings-grid {
  display: flex;
  flex-direction: column;
  gap: $spacing-4;
}

.setting-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: $spacing-2 0;
  gap: $spacing-4;

  .setting-info {
    flex: 1;

    label {
      display: block;
      font-weight: $font-weight-medium;
      margin-bottom: $spacing-1;
      color: $slate-900;
    }

    small {
      color: $slate-500;
      font-size: $font-size-sm;
    }
  }
}

.category-table {
  :deep(.p-datatable-thead) {
    th {
      background: $slate-50;
    }
  }
}

.category-name {
  display: flex;
  align-items: center;
}

.channel-toggles {
  display: flex;
  gap: $spacing-2;
  flex-wrap: wrap;

  &.disabled {
    opacity: 0.5;
    pointer-events: none;
  }
}

.channel-toggle {
  display: flex;
  align-items: center;
  gap: $spacing-1;
  padding: $spacing-1 $spacing-3;
  border-radius: $radius-full;
  border: 1px solid $slate-200;
  background: $slate-50;
  cursor: pointer;
  transition: all 0.2s ease;
  font-size: $font-size-sm;

  &:hover {
    border-color: $intalio-teal-500;
  }

  &.active {
    background: $intalio-teal-500;
    border-color: $intalio-teal-500;
    color: white;
  }

  i {
    font-size: $font-size-sm;
  }
}

.empty-devices {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: $spacing-8;
  text-align: center;
  color: $slate-500;

  i {
    font-size: 3rem;
    margin-bottom: $spacing-4;
  }

  p {
    margin-bottom: $spacing-4;
  }
}

.devices-list {
  display: flex;
  flex-direction: column;
  gap: $spacing-3;
}

.device-item {
  display: flex;
  align-items: center;
  gap: $spacing-4;
  padding: $spacing-4;
  border: 1px solid $slate-200;
  border-radius: $radius-lg;

  .device-icon {
    width: 3rem;
    height: 3rem;
    display: flex;
    align-items: center;
    justify-content: center;
    background: $slate-50;
    border-radius: $radius-lg;
    font-size: $font-size-2xl;
    color: $intalio-teal-500;
  }

  .device-info {
    flex: 1;

    .device-name {
      font-weight: $font-weight-medium;
      margin-bottom: $spacing-1;
      color: $slate-900;
    }

    .device-meta {
      display: flex;
      align-items: center;
      gap: $spacing-3;
      font-size: $font-size-sm;
      color: $slate-500;
    }
  }

  .device-actions {
    display: flex;
    align-items: center;
    gap: $spacing-2;
  }
}

@media (max-width: 768px) {
  .preferences-header {
    flex-direction: column;
    align-items: flex-start;
  }

  .setting-item {
    flex-direction: column;
    align-items: flex-start;

    .setting-info {
      margin-bottom: $spacing-2;
    }
  }

  .device-item {
    flex-direction: column;
    text-align: center;

    .device-info {
      .device-meta {
        flex-direction: column;
        gap: $spacing-2;
      }
    }
  }
}

// Reduced motion support
@media (prefers-reduced-motion: reduce) {
  .channel-toggle {
    transition: none;
  }
}
</style>
