<template>
  <div class="audit-log-view">
    <div class="page-header">
      <h1>{{ $t('admin.auditLog.title') }}</h1>
      <div class="header-actions">
        <Button
          :label="$t('common.export')"
          icon="pi pi-download"
          severity="secondary"
          @click="exportLogs"
          :loading="exporting"
        />
        <Button
          :label="$t('common.refresh')"
          icon="pi pi-refresh"
          @click="loadLogs"
          :loading="loading"
        />
      </div>
    </div>

    <!-- Filters -->
    <Card class="filters-card">
      <template #content>
        <div class="filters-grid">
          <div class="filter-item">
            <label>{{ $t('admin.auditLog.dateRange') }}</label>
            <Calendar
              v-model="filters.dateRange"
              selectionMode="range"
              :showTime="true"
              :placeholder="$t('admin.auditLog.selectDateRange')"
              dateFormat="yy-mm-dd"
            />
          </div>

          <div class="filter-item">
            <label>{{ $t('admin.auditLog.category') }}</label>
            <Dropdown
              v-model="filters.category"
              :options="categoryOptions"
              optionLabel="label"
              optionValue="value"
              :placeholder="$t('common.all')"
              showClear
            />
          </div>

          <div class="filter-item">
            <label>{{ $t('admin.auditLog.action') }}</label>
            <InputText
              v-model="filters.action"
              :placeholder="$t('admin.auditLog.searchAction')"
            />
          </div>

          <div class="filter-item">
            <label>{{ $t('admin.auditLog.user') }}</label>
            <InputText
              v-model="filters.userId"
              :placeholder="$t('admin.auditLog.enterUserId')"
            />
          </div>

          <div class="filter-item">
            <label>{{ $t('admin.auditLog.severity') }}</label>
            <Dropdown
              v-model="filters.severity"
              :options="severityOptions"
              optionLabel="label"
              optionValue="value"
              :placeholder="$t('common.all')"
              showClear
            />
          </div>

          <div class="filter-actions">
            <Button
              :label="$t('common.search')"
              icon="pi pi-search"
              @click="loadLogs"
            />
            <Button
              :label="$t('common.clear')"
              icon="pi pi-times"
              severity="secondary"
              @click="clearFilters"
            />
          </div>
        </div>
      </template>
    </Card>

    <!-- Audit Log Table -->
    <Card class="table-card">
      <template #content>
        <DataTable
          :value="logs"
          :loading="loading"
          :paginator="true"
          :rows="pageSize"
          :totalRecords="totalRecords"
          :lazy="true"
          @page="onPage"
          stripedRows
          responsiveLayout="scroll"
          dataKey="id"
        >
          <Column field="timestamp" :header="$t('admin.auditLog.timestamp')" :sortable="true">
            <template #body="{ data }">
              {{ formatDateTime(data.timestamp) }}
            </template>
          </Column>

          <Column field="userName" :header="$t('admin.auditLog.user')" :sortable="true">
            <template #body="{ data }">
              <div class="user-cell">
                <Avatar :label="getInitials(data.userName)" size="small" shape="circle" />
                <span>{{ data.userName || 'System' }}</span>
              </div>
            </template>
          </Column>

          <Column field="category" :header="$t('admin.auditLog.category')" :sortable="true">
            <template #body="{ data }">
              <Tag :value="data.category" :severity="getCategorySeverity(data.category)" />
            </template>
          </Column>

          <Column field="action" :header="$t('admin.auditLog.action')" :sortable="true" />

          <Column field="entityType" :header="$t('admin.auditLog.entity')">
            <template #body="{ data }">
              <span v-if="data.entityType">
                {{ data.entityType }}
                <small v-if="data.entityId" class="entity-id">{{ data.entityId }}</small>
              </span>
              <span v-else class="text-muted">-</span>
            </template>
          </Column>

          <Column field="severity" :header="$t('admin.auditLog.severity')">
            <template #body="{ data }">
              <Tag :value="data.severity" :severity="getSeverityColor(data.severity)" />
            </template>
          </Column>

          <Column field="ipAddress" :header="$t('admin.auditLog.ipAddress')" />

          <Column :header="$t('common.actions')" style="width: 100px">
            <template #body="{ data }">
              <Button
                icon="pi pi-eye"
                severity="secondary"
                text
                rounded
                @click="showDetails(data)"
                v-tooltip="$t('common.viewDetails')"
              />
            </template>
          </Column>
        </DataTable>
      </template>
    </Card>

    <!-- Details Dialog -->
    <Dialog
      v-model:visible="detailsDialog"
      :header="$t('admin.auditLog.details')"
      :style="{ width: '600px' }"
      modal
    >
      <div v-if="selectedLog" class="details-content">
        <div class="detail-row">
          <label>{{ $t('admin.auditLog.timestamp') }}:</label>
          <span>{{ formatDateTime(selectedLog.timestamp) }}</span>
        </div>

        <div class="detail-row">
          <label>{{ $t('admin.auditLog.user') }}:</label>
          <span>{{ selectedLog.userName || 'System' }}</span>
        </div>

        <div class="detail-row">
          <label>{{ $t('admin.auditLog.action') }}:</label>
          <span>{{ selectedLog.action }}</span>
        </div>

        <div class="detail-row">
          <label>{{ $t('admin.auditLog.category') }}:</label>
          <Tag :value="selectedLog.category" :severity="getCategorySeverity(selectedLog.category)" />
        </div>

        <div class="detail-row" v-if="selectedLog.entityType">
          <label>{{ $t('admin.auditLog.entity') }}:</label>
          <span>{{ selectedLog.entityType }} ({{ selectedLog.entityId }})</span>
        </div>

        <div class="detail-row" v-if="selectedLog.ipAddress">
          <label>{{ $t('admin.auditLog.ipAddress') }}:</label>
          <span>{{ selectedLog.ipAddress }}</span>
        </div>

        <div class="detail-row" v-if="selectedLog.userAgent">
          <label>{{ $t('admin.auditLog.userAgent') }}:</label>
          <span class="user-agent">{{ selectedLog.userAgent }}</span>
        </div>

        <div class="detail-section" v-if="selectedLog.oldValues">
          <h4>{{ $t('admin.auditLog.oldValues') }}</h4>
          <pre class="json-display">{{ formatJson(selectedLog.oldValues) }}</pre>
        </div>

        <div class="detail-section" v-if="selectedLog.newValues">
          <h4>{{ $t('admin.auditLog.newValues') }}</h4>
          <pre class="json-display">{{ formatJson(selectedLog.newValues) }}</pre>
        </div>

        <div class="detail-section" v-if="selectedLog.additionalData">
          <h4>{{ $t('admin.auditLog.additionalData') }}</h4>
          <pre class="json-display">{{ formatJson(selectedLog.additionalData) }}</pre>
        </div>
      </div>

      <template #footer>
        <Button
          :label="$t('common.close')"
          @click="detailsDialog = false"
        />
      </template>
    </Dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { useToast } from 'primevue/usetoast'
import { adminApi } from '@/api/admin'

const { t } = useI18n()
const toast = useToast()

interface AuditLog {
  id: string
  timestamp: string
  userId?: string
  userName?: string
  action: string
  category: string
  entityType?: string
  entityId?: string
  oldValues?: string
  newValues?: string
  additionalData?: string
  ipAddress?: string
  userAgent?: string
  severity: string
}

// State
const logs = ref<AuditLog[]>([])
const loading = ref(false)
const exporting = ref(false)
const pageSize = ref(20)
const totalRecords = ref(0)
const currentPage = ref(0)
const detailsDialog = ref(false)
const selectedLog = ref<AuditLog | null>(null)

const filters = ref({
  dateRange: null as Date[] | null,
  category: null as string | null,
  action: '',
  userId: '',
  severity: null as string | null
})

const categoryOptions = computed(() => [
  { label: t('admin.auditLog.categories.documentManagement'), value: 'DocumentManagement' },
  { label: t('admin.auditLog.categories.userManagement'), value: 'UserManagement' },
  { label: t('admin.auditLog.categories.contentManagement'), value: 'ContentManagement' },
  { label: t('admin.auditLog.categories.security'), value: 'SecurityEvent' },
  { label: t('admin.auditLog.categories.system'), value: 'SystemEvent' }
])

const severityOptions = computed(() => [
  { label: t('admin.auditLog.severities.info'), value: 'Info' },
  { label: t('admin.auditLog.severities.warning'), value: 'Warning' },
  { label: t('admin.auditLog.severities.error'), value: 'Error' },
  { label: t('admin.auditLog.severities.critical'), value: 'Critical' }
])

// Methods
const loadLogs = async () => {
  loading.value = true
  try {
    const params: any = {
      skip: currentPage.value * pageSize.value,
      take: pageSize.value
    }

    if (filters.value.dateRange?.[0]) {
      params.from = filters.value.dateRange[0].toISOString()
    }
    if (filters.value.dateRange?.[1]) {
      params.to = filters.value.dateRange[1].toISOString()
    }
    if (filters.value.category) {
      params.category = filters.value.category
    }
    if (filters.value.action) {
      params.action = filters.value.action
    }
    if (filters.value.userId) {
      params.userId = filters.value.userId
    }
    if (filters.value.severity) {
      params.severity = filters.value.severity
    }

    const response = await adminApi.getAuditLogs(params)
    logs.value = response.items
    totalRecords.value = response.totalCount
  } catch (error) {
    toast.add({
      severity: 'error',
      summary: t('common.error'),
      detail: t('admin.auditLog.loadError'),
      life: 5000
    })
  } finally {
    loading.value = false
  }
}

const exportLogs = async () => {
  exporting.value = true
  try {
    const params: any = {}
    if (filters.value.dateRange?.[0]) {
      params.from = filters.value.dateRange[0].toISOString()
    }
    if (filters.value.dateRange?.[1]) {
      params.to = filters.value.dateRange[1].toISOString()
    }
    if (filters.value.category) {
      params.category = filters.value.category
    }

    const blob = await adminApi.exportAuditLogs(params)
    const url = window.URL.createObjectURL(blob)
    const a = document.createElement('a')
    a.href = url
    a.download = `audit-logs-${new Date().toISOString().split('T')[0]}.csv`
    a.click()
    window.URL.revokeObjectURL(url)

    toast.add({
      severity: 'success',
      summary: t('common.success'),
      detail: t('admin.auditLog.exportSuccess'),
      life: 3000
    })
  } catch (error) {
    toast.add({
      severity: 'error',
      summary: t('common.error'),
      detail: t('admin.auditLog.exportError'),
      life: 5000
    })
  } finally {
    exporting.value = false
  }
}

const onPage = (event: any) => {
  currentPage.value = event.page
  loadLogs()
}

const clearFilters = () => {
  filters.value = {
    dateRange: null,
    category: null,
    action: '',
    userId: '',
    severity: null
  }
  loadLogs()
}

const showDetails = (log: AuditLog) => {
  selectedLog.value = log
  detailsDialog.value = true
}

const formatDateTime = (dateString: string) => {
  return new Date(dateString).toLocaleString()
}

const formatJson = (json: string) => {
  try {
    return JSON.stringify(JSON.parse(json), null, 2)
  } catch {
    return json
  }
}

const getInitials = (name?: string) => {
  if (!name) return 'S'
  return name.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2)
}

const getCategorySeverity = (category: string) => {
  const map: Record<string, string> = {
    'DocumentManagement': 'info',
    'UserManagement': 'warning',
    'ContentManagement': 'success',
    'SecurityEvent': 'danger',
    'SystemEvent': 'secondary'
  }
  return map[category] || 'info'
}

const getSeverityColor = (severity: string) => {
  const map: Record<string, string> = {
    'Info': 'info',
    'Warning': 'warning',
    'Error': 'danger',
    'Critical': 'danger'
  }
  return map[severity] || 'info'
}

onMounted(() => {
  loadLogs()
})
</script>

<style scoped lang="scss">
.audit-log-view {
  padding: 1.5rem;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;

  h1 {
    margin: 0;
  }

  .header-actions {
    display: flex;
    gap: 0.5rem;
  }
}

.filters-card {
  margin-bottom: 1.5rem;
}

.filters-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 1rem;
  align-items: end;
}

.filter-item {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;

  label {
    font-weight: 500;
    font-size: 0.875rem;
  }
}

.filter-actions {
  display: flex;
  gap: 0.5rem;
  align-items: flex-end;
}

.user-cell {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.entity-id {
  color: var(--text-color-secondary);
  margin-left: 0.25rem;
}

.text-muted {
  color: var(--text-color-secondary);
}

.details-content {
  .detail-row {
    display: flex;
    gap: 1rem;
    margin-bottom: 0.75rem;

    label {
      font-weight: 600;
      min-width: 120px;
    }
  }

  .user-agent {
    font-size: 0.75rem;
    word-break: break-all;
  }

  .detail-section {
    margin-top: 1rem;
    padding-top: 1rem;
    border-top: 1px solid var(--surface-border);

    h4 {
      margin: 0 0 0.5rem 0;
    }
  }

  .json-display {
    background: var(--surface-ground);
    padding: 0.75rem;
    border-radius: 6px;
    font-size: 0.75rem;
    overflow-x: auto;
    max-height: 200px;
    margin: 0;
  }
}
</style>
