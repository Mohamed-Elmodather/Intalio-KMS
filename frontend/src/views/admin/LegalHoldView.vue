<template>
  <div class="legal-hold-view">
    <div class="page-header">
      <h1>{{ $t('admin.legalHold.title') }}</h1>
      <Button
        :label="$t('admin.legalHold.create')"
        icon="pi pi-plus"
        @click="openCreateDialog"
      />
    </div>

    <!-- Legal Holds List -->
    <Card>
      <template #content>
        <DataTable
          :value="holds"
          :loading="loading"
          stripedRows
          responsiveLayout="scroll"
          dataKey="id"
        >
          <Column field="name" :header="$t('admin.legalHold.name')" :sortable="true" />

          <Column field="matterNumber" :header="$t('admin.legalHold.matterNumber')" />

          <Column field="status" :header="$t('admin.legalHold.status')">
            <template #body="{ data }">
              <Tag :value="data.status" :severity="getStatusSeverity(data.status)" />
            </template>
          </Column>

          <Column field="documentCount" :header="$t('admin.legalHold.documents')" />

          <Column field="custodianCount" :header="$t('admin.legalHold.custodians')" />

          <Column field="createdAt" :header="$t('common.createdAt')">
            <template #body="{ data }">
              {{ formatDate(data.createdAt) }}
            </template>
          </Column>

          <Column :header="$t('common.actions')" style="width: 150px">
            <template #body="{ data }">
              <div class="action-buttons">
                <Button
                  icon="pi pi-eye"
                  severity="secondary"
                  text
                  rounded
                  @click="viewHold(data)"
                  v-tooltip="$t('common.view')"
                />
                <Button
                  v-if="data.status === 'Active'"
                  icon="pi pi-unlock"
                  severity="warning"
                  text
                  rounded
                  @click="confirmRelease(data)"
                  v-tooltip="$t('admin.legalHold.release')"
                />
              </div>
            </template>
          </Column>
        </DataTable>
      </template>
    </Card>

    <!-- Create Dialog -->
    <Dialog
      v-model:visible="createDialog"
      :header="$t('admin.legalHold.createTitle')"
      :style="{ width: '600px' }"
      modal
    >
      <div class="form-grid">
        <div class="form-field">
          <label for="name">{{ $t('admin.legalHold.name') }} *</label>
          <InputText
            id="name"
            v-model="form.name"
            :class="{ 'p-invalid': errors.name }"
          />
          <small v-if="errors.name" class="p-error">{{ errors.name }}</small>
        </div>

        <div class="form-field">
          <label for="matterNumber">{{ $t('admin.legalHold.matterNumber') }}</label>
          <InputText id="matterNumber" v-model="form.matterNumber" />
        </div>

        <div class="form-field full-width">
          <label for="description">{{ $t('common.description') }}</label>
          <Textarea id="description" v-model="form.description" rows="3" />
        </div>

        <div class="form-field full-width">
          <label for="reason">{{ $t('admin.legalHold.reason') }} *</label>
          <Textarea
            id="reason"
            v-model="form.reason"
            rows="3"
            :class="{ 'p-invalid': errors.reason }"
          />
          <small v-if="errors.reason" class="p-error">{{ errors.reason }}</small>
        </div>
      </div>

      <template #footer>
        <Button
          :label="$t('common.cancel')"
          severity="secondary"
          @click="createDialog = false"
        />
        <Button
          :label="$t('common.create')"
          @click="createHold"
          :loading="saving"
        />
      </template>
    </Dialog>

    <!-- View Dialog -->
    <Dialog
      v-model:visible="viewDialog"
      :header="selectedHold?.name"
      :style="{ width: '800px' }"
      modal
    >
      <TabView v-if="selectedHold">
        <TabPanel :header="$t('admin.legalHold.details')">
          <div class="details-grid">
            <div class="detail-item">
              <label>{{ $t('admin.legalHold.status') }}</label>
              <Tag :value="selectedHold.status" :severity="getStatusSeverity(selectedHold.status)" />
            </div>
            <div class="detail-item">
              <label>{{ $t('admin.legalHold.matterNumber') }}</label>
              <span>{{ selectedHold.matterNumber || '-' }}</span>
            </div>
            <div class="detail-item full-width">
              <label>{{ $t('common.description') }}</label>
              <p>{{ selectedHold.description || '-' }}</p>
            </div>
            <div class="detail-item full-width">
              <label>{{ $t('admin.legalHold.reason') }}</label>
              <p>{{ selectedHold.reason }}</p>
            </div>
            <div class="detail-item">
              <label>{{ $t('common.createdAt') }}</label>
              <span>{{ formatDate(selectedHold.createdAt) }}</span>
            </div>
            <div class="detail-item">
              <label>{{ $t('common.createdBy') }}</label>
              <span>{{ selectedHold.createdByName }}</span>
            </div>
            <div v-if="selectedHold.releasedAt" class="detail-item">
              <label>{{ $t('admin.legalHold.releasedAt') }}</label>
              <span>{{ formatDate(selectedHold.releasedAt) }}</span>
            </div>
            <div v-if="selectedHold.releaseReason" class="detail-item full-width">
              <label>{{ $t('admin.legalHold.releaseReason') }}</label>
              <p>{{ selectedHold.releaseReason }}</p>
            </div>
          </div>
        </TabPanel>

        <TabPanel :header="$t('admin.legalHold.documents')">
          <div class="documents-section">
            <div class="section-header">
              <span>{{ selectedHold.documents?.length || 0 }} {{ $t('admin.legalHold.documentsHeld') }}</span>
              <Button
                v-if="selectedHold.status === 'Active'"
                :label="$t('admin.legalHold.addDocuments')"
                icon="pi pi-plus"
                size="small"
                @click="openAddDocumentsDialog"
              />
            </div>
            <DataTable
              :value="selectedHold.documents"
              stripedRows
              size="small"
            >
              <Column field="documentName" :header="$t('common.name')" />
              <Column field="addedAt" :header="$t('admin.legalHold.addedAt')">
                <template #body="{ data }">
                  {{ formatDate(data.addedAt) }}
                </template>
              </Column>
              <Column field="addedByName" :header="$t('common.addedBy')" />
            </DataTable>
          </div>
        </TabPanel>

        <TabPanel :header="$t('admin.legalHold.custodians')">
          <div class="custodians-section">
            <div class="section-header">
              <span>{{ selectedHold.custodians?.length || 0 }} {{ $t('admin.legalHold.custodiansAssigned') }}</span>
              <Button
                v-if="selectedHold.status === 'Active'"
                :label="$t('admin.legalHold.addCustodian')"
                icon="pi pi-plus"
                size="small"
                @click="openAddCustodianDialog"
              />
            </div>
            <DataTable
              :value="selectedHold.custodians"
              stripedRows
              size="small"
            >
              <Column field="userName" :header="$t('common.name')" />
              <Column field="userEmail" :header="$t('common.email')" />
              <Column field="notifiedAt" :header="$t('admin.legalHold.notifiedAt')">
                <template #body="{ data }">
                  {{ data.notifiedAt ? formatDate(data.notifiedAt) : '-' }}
                </template>
              </Column>
              <Column field="acknowledgedAt" :header="$t('admin.legalHold.acknowledgedAt')">
                <template #body="{ data }">
                  {{ data.acknowledgedAt ? formatDate(data.acknowledgedAt) : '-' }}
                </template>
              </Column>
            </DataTable>
          </div>
        </TabPanel>
      </TabView>

      <template #footer>
        <Button
          :label="$t('common.close')"
          @click="viewDialog = false"
        />
      </template>
    </Dialog>

    <!-- Release Confirmation -->
    <Dialog
      v-model:visible="releaseDialog"
      :header="$t('admin.legalHold.releaseTitle')"
      :style="{ width: '500px' }"
      modal
    >
      <p>{{ $t('admin.legalHold.releaseConfirm', { name: holdToRelease?.name }) }}</p>

      <div class="form-field">
        <label for="releaseReason">{{ $t('admin.legalHold.releaseReason') }} *</label>
        <Textarea
          id="releaseReason"
          v-model="releaseReason"
          rows="3"
        />
      </div>

      <template #footer>
        <Button
          :label="$t('common.cancel')"
          severity="secondary"
          @click="releaseDialog = false"
        />
        <Button
          :label="$t('admin.legalHold.release')"
          severity="warning"
          @click="releaseHold"
          :loading="releasing"
        />
      </template>
    </Dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import { useToast } from 'primevue/usetoast'
import { adminApi } from '@/api/admin'

const { t } = useI18n()
const toast = useToast()

interface LegalHold {
  id: string
  name: string
  matterNumber?: string
  description?: string
  reason: string
  status: string
  createdAt: string
  createdByName: string
  releasedAt?: string
  releasedByName?: string
  releaseReason?: string
  documentCount: number
  custodianCount: number
  documents?: any[]
  custodians?: any[]
}

// State
const holds = ref<LegalHold[]>([])
const loading = ref(false)
const saving = ref(false)
const releasing = ref(false)
const createDialog = ref(false)
const viewDialog = ref(false)
const releaseDialog = ref(false)
const selectedHold = ref<LegalHold | null>(null)
const holdToRelease = ref<LegalHold | null>(null)
const releaseReason = ref('')

const form = ref({
  name: '',
  matterNumber: '',
  description: '',
  reason: ''
})

const errors = ref({
  name: '',
  reason: ''
})

// Methods
const loadHolds = async () => {
  loading.value = true
  try {
    holds.value = await adminApi.getLegalHolds()
  } catch (error) {
    toast.add({
      severity: 'error',
      summary: t('common.error'),
      detail: t('admin.legalHold.loadError'),
      life: 5000
    })
  } finally {
    loading.value = false
  }
}

const openCreateDialog = () => {
  form.value = { name: '', matterNumber: '', description: '', reason: '' }
  errors.value = { name: '', reason: '' }
  createDialog.value = true
}

const createHold = async () => {
  errors.value = { name: '', reason: '' }

  if (!form.value.name) {
    errors.value.name = t('validation.required')
    return
  }
  if (!form.value.reason) {
    errors.value.reason = t('validation.required')
    return
  }

  saving.value = true
  try {
    await adminApi.createLegalHold(form.value)
    createDialog.value = false
    toast.add({
      severity: 'success',
      summary: t('common.success'),
      detail: t('admin.legalHold.createSuccess'),
      life: 3000
    })
    loadHolds()
  } catch (error) {
    toast.add({
      severity: 'error',
      summary: t('common.error'),
      detail: t('admin.legalHold.createError'),
      life: 5000
    })
  } finally {
    saving.value = false
  }
}

const viewHold = async (hold: LegalHold) => {
  try {
    selectedHold.value = await adminApi.getLegalHold(hold.id)
    viewDialog.value = true
  } catch (error) {
    toast.add({
      severity: 'error',
      summary: t('common.error'),
      detail: t('admin.legalHold.loadError'),
      life: 5000
    })
  }
}

const confirmRelease = (hold: LegalHold) => {
  holdToRelease.value = hold
  releaseReason.value = ''
  releaseDialog.value = true
}

const releaseHold = async () => {
  if (!holdToRelease.value || !releaseReason.value) return

  releasing.value = true
  try {
    await adminApi.releaseLegalHold(holdToRelease.value.id, releaseReason.value)
    releaseDialog.value = false
    toast.add({
      severity: 'success',
      summary: t('common.success'),
      detail: t('admin.legalHold.releaseSuccess'),
      life: 3000
    })
    loadHolds()
  } catch (error) {
    toast.add({
      severity: 'error',
      summary: t('common.error'),
      detail: t('admin.legalHold.releaseError'),
      life: 5000
    })
  } finally {
    releasing.value = false
  }
}

const openAddDocumentsDialog = () => {
  // TODO: Implement document selector dialog
}

const openAddCustodianDialog = () => {
  // TODO: Implement user selector dialog
}

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString()
}

const getStatusSeverity = (status: string) => {
  return status === 'Active' ? 'danger' : 'secondary'
}

onMounted(() => {
  loadHolds()
})
</script>

<style scoped lang="scss">
.legal-hold-view {
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
}

.form-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 1rem;
}

.form-field {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;

  &.full-width {
    grid-column: 1 / -1;
  }

  label {
    font-weight: 500;
  }
}

.details-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 1rem;

  .detail-item {
    &.full-width {
      grid-column: 1 / -1;
    }

    label {
      display: block;
      font-weight: 600;
      margin-bottom: 0.25rem;
      color: var(--text-color-secondary);
      font-size: 0.875rem;
    }

    p {
      margin: 0;
    }
  }
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}

.action-buttons {
  display: flex;
  gap: 0.25rem;
}
</style>
