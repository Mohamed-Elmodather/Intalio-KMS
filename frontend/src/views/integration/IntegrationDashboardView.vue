<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import { useReducedMotion } from '@/composables/useReducedMotion'
import { PageHeader } from '@/components/base'
import ErrorState from '@/components/base/ErrorState.vue'
import type { BreadcrumbItem } from '@/components/base/PageHeader.vue'
import Card from 'primevue/card'
import Button from 'primevue/button'
import Tag from 'primevue/tag'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import ProgressBar from 'primevue/progressbar'
import Timeline from 'primevue/timeline'
import Skeleton from 'primevue/skeleton'

const { t, locale } = useI18n()
const prefersReducedMotion = useReducedMotion()

// RTL support
const isRTL = computed(() => locale.value === 'ar')

// Animation control
const shouldAnimate = computed(() => !prefersReducedMotion.value)
const isContentVisible = ref(false)

// Breadcrumbs for PageHeader
const breadcrumbs = computed<BreadcrumbItem[]>(() => [
  { label: 'Home', labelArabic: 'الرئيسية', to: '/dashboard' },
  { label: t('integration.dashboard.title') }
])

// State
const isLoading = ref(true)
const error = ref<Error | null>(null)
const dashboardData = ref<any>(null)
const connectors = ref<any[]>([])
const recentSyncJobs = ref<any[]>([])
const recentEvents = ref<any[]>([])

// Computed
const healthySummary = computed(() => {
  if (!dashboardData.value) return { healthy: 0, total: 0, percentage: 0 }
  const healthy = dashboardData.value.healthyConnectors
  const total = dashboardData.value.totalConnectors
  return {
    healthy,
    total,
    percentage: total > 0 ? Math.round((healthy / total) * 100) : 0
  }
})

// Methods
async function fetchDashboardData() {
  isLoading.value = true
  error.value = null
  try {
    // TODO: Fetch from API
    await new Promise(resolve => setTimeout(resolve, 600))

    dashboardData.value = {
      totalConnectors: 6,
      activeConnectors: 5,
      healthyConnectors: 5,
      unhealthyConnectors: 1,
      totalSyncJobs: 15,
      runningSyncJobs: 2,
      failedSyncJobs: 1,
      todayEvents: 245,
      todayErrors: 3
    }

    connectors.value = [
      {
        id: '1',
        name: 'Intalio Case',
        provider: 'IntalioCase',
        category: 'Workflow',
        status: 'Active',
        isHealthy: true,
        lastSync: new Date(Date.now() - 1800000)
      },
      {
        id: '2',
        name: 'Intalio IAM',
        provider: 'IntalioIAM',
        category: 'Identity',
        status: 'Active',
        isHealthy: true,
        lastSync: new Date(Date.now() - 7200000)
      },
      {
        id: '3',
        name: 'Intalio DMS',
        provider: 'IntalioDMS',
        category: 'DocumentManagement',
        status: 'Active',
        isHealthy: true,
        lastSync: new Date(Date.now() - 900000)
      },
      {
        id: '4',
        name: 'Intalio Correspondence',
        provider: 'IntalioCorrespondence',
        category: 'Correspondence',
        status: 'Active',
        isHealthy: true,
        lastSync: new Date(Date.now() - 3600000)
      },
      {
        id: '5',
        name: 'SharePoint Online',
        provider: 'SharePoint',
        category: 'DocumentManagement',
        status: 'Active',
        isHealthy: true,
        lastSync: new Date(Date.now() - 1800000)
      },
      {
        id: '6',
        name: 'SAP S/4HANA',
        provider: 'SAP',
        category: 'ERP',
        status: 'Error',
        isHealthy: false,
        lastSync: new Date(Date.now() - 86400000),
        errorMessage: 'Connection timeout'
      }
    ]

    recentSyncJobs.value = [
      {
        id: '1',
        connectorName: 'Intalio Case',
        jobName: 'Task Sync',
        status: 'Completed',
        recordCount: 45,
        errorCount: 0,
        completedAt: new Date(Date.now() - 300000)
      },
      {
        id: '2',
        connectorName: 'Intalio DMS',
        jobName: 'Document Sync',
        status: 'Running',
        recordCount: 120,
        processedCount: 85,
        completedAt: null
      },
      {
        id: '3',
        connectorName: 'Intalio IAM',
        jobName: 'User Sync',
        status: 'Completed',
        recordCount: 320,
        errorCount: 2,
        completedAt: new Date(Date.now() - 7200000)
      },
      {
        id: '4',
        connectorName: 'SAP S/4HANA',
        jobName: 'Employee Sync',
        status: 'Failed',
        recordCount: 0,
        errorCount: 1,
        errorMessage: 'Connection refused',
        completedAt: new Date(Date.now() - 86400000)
      }
    ]

    recentEvents.value = [
      {
        id: '1',
        type: 'TaskCompleted',
        connector: 'Intalio Case',
        description: 'Task "Document Review" completed',
        timestamp: new Date(Date.now() - 300000),
        status: 'Success'
      },
      {
        id: '2',
        type: 'DocumentSynced',
        connector: 'Intalio DMS',
        description: '15 documents synced',
        timestamp: new Date(Date.now() - 600000),
        status: 'Success'
      },
      {
        id: '3',
        type: 'UserCreated',
        connector: 'Intalio IAM',
        description: 'New user provisioned: ahmed.hassan@intalio.com',
        timestamp: new Date(Date.now() - 900000),
        status: 'Success'
      },
      {
        id: '4',
        type: 'SyncFailed',
        connector: 'SAP S/4HANA',
        description: 'Employee sync failed: Connection timeout',
        timestamp: new Date(Date.now() - 86400000),
        status: 'Error'
      }
    ]

    // Trigger entrance animations
    if (shouldAnimate.value) {
      requestAnimationFrame(() => {
        isContentVisible.value = true
      })
    } else {
      isContentVisible.value = true
    }
  } catch (e) {
    error.value = e as Error
    console.error('Failed to fetch dashboard:', e)
  } finally {
    isLoading.value = false
  }
}

function handleRetry() {
  isLoading.value = true
  isContentVisible.value = false
  fetchDashboardData()
}

function getStatusSeverity(status: string): "success" | "info" | "warn" | "danger" | "secondary" | "contrast" | undefined {
  switch (status) {
    case 'Active':
    case 'Completed':
    case 'Success':
      return 'success'
    case 'Running':
    case 'Processing':
      return 'info'
    case 'Warning':
      return 'warn'
    case 'Error':
    case 'Failed':
      return 'danger'
    default:
      return 'secondary'
  }
}

function getCategoryIcon(category: string): string {
  switch (category) {
    case 'Workflow': return 'pi-sitemap'
    case 'Identity': return 'pi-users'
    case 'DocumentManagement': return 'pi-folder'
    case 'Correspondence': return 'pi-envelope'
    case 'ERP': return 'pi-building'
    case 'Calendar': return 'pi-calendar'
    default: return 'pi-link'
  }
}

function formatDate(date: Date | null): string {
  if (!date) return '-'
  return new Intl.DateTimeFormat(locale.value, {
    dateStyle: 'short',
    timeStyle: 'short'
  }).format(date)
}

function formatRelativeTime(date: Date): string {
  const now = new Date()
  const diffMs = now.getTime() - date.getTime()
  const diffMins = Math.floor(diffMs / 60000)
  const diffHours = Math.floor(diffMs / 3600000)
  const diffDays = Math.floor(diffMs / 86400000)

  if (diffMins < 60) return `${diffMins} ${t('common.minutesAgo')}`
  if (diffHours < 24) return `${diffHours} ${t('common.hoursAgo')}`
  return `${diffDays} ${t('common.daysAgo')}`
}

onMounted(() => {
  fetchDashboardData()
})
</script>

<template>
  <div class="integration-dashboard" :class="{ rtl: isRTL }">
    <!-- Error State -->
    <ErrorState
      v-if="error"
      :error="error"
      :title="isRTL ? 'فشل تحميل لوحة التكامل' : 'Failed to load integration dashboard'"
      show-retry
      size="lg"
      @retry="handleRetry"
    />

    <template v-else>
    <!-- Header -->
    <PageHeader
      :title="t('integration.dashboard.title')"
      :description="t('integration.dashboard.subtitle')"
      :breadcrumbs="breadcrumbs"
      padding-bottom="xl"
      background-variant="gradient"
      :animated="shouldAnimate"
    >
      <template #actions>
        <Button :label="t('integration.manageConnectors')" icon="pi pi-cog" class="header-btn-secondary" />
        <Button :label="t('integration.addConnector')" icon="pi pi-plus" class="header-btn-primary" />
      </template>
    </PageHeader>

    <!-- Loading State -->
    <div v-if="isLoading" class="grid">
      <div class="col-12 md:col-6 lg:col-3" v-for="i in 4" :key="i">
        <Card>
          <template #content>
            <Skeleton height="80px" />
          </template>
        </Card>
      </div>
    </div>

    <!-- Dashboard Content -->
    <div v-else>
      <!-- Stats Cards -->
      <div class="grid mb-4">
        <div class="col-12 md:col-6 lg:col-3">
          <Card class="h-full">
            <template #content>
              <div class="flex align-items-center gap-3">
                <div class="w-3rem h-3rem bg-blue-100 border-round flex align-items-center justify-content-center">
                  <i class="pi pi-link text-blue-600 text-xl"></i>
                </div>
                <div>
                  <div class="text-2xl font-bold">{{ dashboardData.activeConnectors }} / {{ dashboardData.totalConnectors }}</div>
                  <div class="text-color-secondary">{{ t('integration.dashboard.activeConnectors') }}</div>
                </div>
              </div>
            </template>
          </Card>
        </div>

        <div class="col-12 md:col-6 lg:col-3">
          <Card class="h-full">
            <template #content>
              <div class="flex align-items-center gap-3">
                <div class="w-3rem h-3rem bg-green-100 border-round flex align-items-center justify-content-center">
                  <i class="pi pi-heart text-green-600 text-xl"></i>
                </div>
                <div>
                  <div class="text-2xl font-bold text-green-600">{{ healthySummary.percentage }}%</div>
                  <div class="text-color-secondary">{{ t('integration.dashboard.healthyConnectors') }}</div>
                </div>
              </div>
            </template>
          </Card>
        </div>

        <div class="col-12 md:col-6 lg:col-3">
          <Card class="h-full">
            <template #content>
              <div class="flex align-items-center gap-3">
                <div class="w-3rem h-3rem bg-purple-100 border-round flex align-items-center justify-content-center">
                  <i class="pi pi-sync text-purple-600 text-xl"></i>
                </div>
                <div>
                  <div class="text-2xl font-bold">{{ dashboardData.runningSyncJobs }}</div>
                  <div class="text-color-secondary">{{ t('integration.dashboard.runningSyncs') }}</div>
                </div>
              </div>
            </template>
          </Card>
        </div>

        <div class="col-12 md:col-6 lg:col-3">
          <Card class="h-full">
            <template #content>
              <div class="flex align-items-center gap-3">
                <div class="w-3rem h-3rem bg-yellow-100 border-round flex align-items-center justify-content-center">
                  <i class="pi pi-bolt text-yellow-600 text-xl"></i>
                </div>
                <div>
                  <div class="text-2xl font-bold">{{ dashboardData.todayEvents }}</div>
                  <div class="text-color-secondary">{{ t('integration.dashboard.todayEvents') }}</div>
                </div>
              </div>
            </template>
          </Card>
        </div>
      </div>

      <!-- Main Content -->
      <div class="grid">
        <!-- Connectors Status -->
        <div class="col-12 lg:col-8">
          <Card>
            <template #title>
              <div class="flex justify-content-between align-items-center">
                <span>{{ t('integration.dashboard.connectorStatus') }}</span>
                <Button :label="t('common.viewAll')" text size="small" />
              </div>
            </template>
            <template #content>
              <DataTable :value="connectors" responsiveLayout="scroll">
                <Column field="name" :header="t('integration.connector.name')">
                  <template #body="{ data }">
                    <div class="flex align-items-center gap-2">
                      <i :class="['pi', getCategoryIcon(data.category), 'text-color-secondary']"></i>
                      <span class="font-medium">{{ data.name }}</span>
                    </div>
                  </template>
                </Column>
                <Column field="category" :header="t('integration.connector.category')" />
                <Column field="status" :header="t('integration.connector.status')">
                  <template #body="{ data }">
                    <div class="flex align-items-center gap-2">
                      <i
                        class="pi"
                        :class="{
                          'pi-check-circle text-green-500': data.isHealthy,
                          'pi-exclamation-circle text-red-500': !data.isHealthy
                        }"
                      ></i>
                      <Tag :value="data.status" :severity="getStatusSeverity(data.status)" />
                    </div>
                  </template>
                </Column>
                <Column field="lastSync" :header="t('integration.connector.lastSync')">
                  <template #body="{ data }">
                    {{ formatRelativeTime(data.lastSync) }}
                  </template>
                </Column>
                <Column>
                  <template #body>
                    <Button icon="pi pi-sync" text rounded size="small" :title="t('integration.syncNow')" />
                    <Button icon="pi pi-cog" text rounded size="small" :title="t('common.settings')" />
                  </template>
                </Column>
              </DataTable>
            </template>
          </Card>

          <!-- Recent Sync Jobs -->
          <Card class="mt-4">
            <template #title>
              <div class="flex justify-content-between align-items-center">
                <span>{{ t('integration.dashboard.recentSyncJobs') }}</span>
                <Button :label="t('common.viewAll')" text size="small" />
              </div>
            </template>
            <template #content>
              <DataTable :value="recentSyncJobs" responsiveLayout="scroll">
                <Column field="connectorName" :header="t('integration.syncJob.connector')" />
                <Column field="jobName" :header="t('integration.syncJob.name')" />
                <Column field="status" :header="t('integration.syncJob.status')">
                  <template #body="{ data }">
                    <Tag :value="data.status" :severity="getStatusSeverity(data.status)" />
                  </template>
                </Column>
                <Column field="recordCount" :header="t('integration.syncJob.records')">
                  <template #body="{ data }">
                    <div v-if="data.status === 'Running'">
                      <ProgressBar :value="Math.round((data.processedCount / data.recordCount) * 100)" :showValue="true" style="height: 8px" />
                      <span class="text-xs text-color-secondary">{{ data.processedCount }} / {{ data.recordCount }}</span>
                    </div>
                    <span v-else>{{ data.recordCount }}</span>
                  </template>
                </Column>
                <Column field="completedAt" :header="t('integration.syncJob.completedAt')">
                  <template #body="{ data }">
                    {{ formatDate(data.completedAt) }}
                  </template>
                </Column>
              </DataTable>
            </template>
          </Card>
        </div>

        <!-- Recent Events -->
        <div class="col-12 lg:col-4">
          <Card class="h-full">
            <template #title>
              <div class="flex justify-content-between align-items-center">
                <span>{{ t('integration.dashboard.recentEvents') }}</span>
                <Button :label="t('common.viewAll')" text size="small" />
              </div>
            </template>
            <template #content>
              <Timeline :value="recentEvents" class="integration-timeline">
                <template #marker="{ item }">
                  <span
                    class="flex align-items-center justify-content-center w-2rem h-2rem border-round"
                    :class="{
                      'bg-green-100': item.status === 'Success',
                      'bg-red-100': item.status === 'Error'
                    }"
                  >
                    <i
                      class="pi text-sm"
                      :class="{
                        'pi-check text-green-600': item.status === 'Success',
                        'pi-times text-red-600': item.status === 'Error'
                      }"
                    ></i>
                  </span>
                </template>
                <template #content="{ item }">
                  <div class="mb-3">
                    <div class="flex align-items-center gap-2 mb-1">
                      <Tag :value="item.connector" size="small" />
                      <span class="text-xs text-color-secondary">{{ formatRelativeTime(item.timestamp) }}</span>
                    </div>
                    <p class="m-0 text-sm">{{ item.description }}</p>
                  </div>
                </template>
              </Timeline>
            </template>
          </Card>
        </div>
      </div>
    </div>
    </template>
  </div>
</template>

<style scoped lang="scss">
@use '@/assets/styles/_variables.scss' as *;
@use '@/assets/styles/_mixins.scss' as *;

.integration-dashboard {
  @include page-view;
  padding: $spacing-6;
}

:deep(.integration-timeline .p-timeline-event-opposite) {
  display: none;
}

:deep(.integration-timeline .p-timeline-event-content) {
  padding-inline-start: $spacing-4;
}
</style>
