<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import { useReducedMotion } from '@/composables/useReducedMotion'
import ErrorState from '@/components/base/ErrorState.vue'
import { LoadingSpinner } from '@/components/base'
import Card from 'primevue/card'
import Button from 'primevue/button'
import TabView from 'primevue/tabview'
import TabPanel from 'primevue/tabpanel'
import FileUpload from 'primevue/fileupload'
import Textarea from 'primevue/textarea'
import Dropdown from 'primevue/dropdown'
import ProgressBar from 'primevue/progressbar'
import Tag from 'primevue/tag'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import Dialog from 'primevue/dialog'
import InputText from 'primevue/inputtext'

const { t, locale } = useI18n()
const prefersReducedMotion = useReducedMotion()

// RTL support
const isRTL = computed(() => locale.value === 'ar')

// Animation control
const isContentVisible = ref(false)
const shouldAnimate = computed(() => !prefersReducedMotion.value)
const LOADING_DELAY = 600

// Loading and error state
const isLoading = ref(true)
const error = ref<Error | null>(null)

// State
const activeTab = ref(0)
const quotaStatus = ref<any>(null)
const recentJobs = ref<any[]>([])

// Transcription state
const transcriptionFile = ref<File | null>(null)
const transcriptionLanguage = ref('ar')
const transcriptionResult = ref<any>(null)
const isTranscribing = ref(false)

// Summarization state
const summarizationText = ref('')
const summarizationLength = ref('medium')
const summarizationLanguage = ref('ar')
const summarizationResult = ref<any>(null)
const isSummarizing = ref(false)

// Q&A state
const qaQuestion = ref('')
const qaContext = ref('')
const qaAnswer = ref<any>(null)
const isAnswering = ref(false)

// Dialog state
const showJobDetailsDialog = ref(false)
const selectedJob = ref<any>(null)

// Options
const languageOptions = [
  { label: 'العربية', value: 'ar' },
  { label: 'English', value: 'en' }
]

const summaryLengthOptions = [
  { label: t('ai.short'), value: 'short' },
  { label: t('ai.medium'), value: 'medium' },
  { label: t('ai.long'), value: 'long' }
]

// Computed
const quotaPercentage = computed(() => {
  if (!quotaStatus.value) return 0
  return Math.round((quotaStatus.value.usedTokens / quotaStatus.value.totalTokens) * 100)
})

// Methods
async function fetchQuotaStatus() {
  try {
    // TODO: Fetch from API
    quotaStatus.value = {
      totalTokens: 1000000,
      usedTokens: 250000,
      remainingTokens: 750000,
      resetDate: new Date(Date.now() + 30 * 24 * 60 * 60 * 1000)
    }
  } catch (error) {
    console.error('Failed to fetch quota:', error)
  }
}

async function fetchRecentJobs() {
  try {
    // TODO: Fetch from API
    recentJobs.value = [
      {
        id: '1',
        type: 'Transcription',
        status: 'Completed',
        fileName: 'meeting-recording.mp3',
        createdAt: new Date(),
        duration: 45
      },
      {
        id: '2',
        type: 'Summarization',
        status: 'Completed',
        fileName: 'quarterly-report.pdf',
        createdAt: new Date(Date.now() - 3600000),
        duration: 12
      },
      {
        id: '3',
        type: 'Translation',
        status: 'Processing',
        fileName: 'policy-document.docx',
        createdAt: new Date(Date.now() - 7200000),
        progress: 65
      }
    ]
  } catch (error) {
    console.error('Failed to fetch jobs:', error)
  }
}

function onFileSelect(event: any) {
  transcriptionFile.value = event.files[0]
}

async function startTranscription() {
  if (!transcriptionFile.value) return

  isTranscribing.value = true
  try {
    // TODO: Call API
    await new Promise(resolve => setTimeout(resolve, 3000))
    transcriptionResult.value = {
      text: 'هذا نص تجريبي للنسخ الصوتي. تم تحويل الملف الصوتي إلى نص بنجاح.',
      confidence: 0.95,
      duration: 45,
      segments: [
        { start: 0, end: 10, text: 'مرحباً بكم في الاجتماع', speaker: 'Speaker 1' },
        { start: 10, end: 25, text: 'سنناقش اليوم خطة العمل', speaker: 'Speaker 1' },
        { start: 25, end: 45, text: 'شكراً لحضوركم', speaker: 'Speaker 2' }
      ]
    }
  } catch (error) {
    console.error('Transcription failed:', error)
  } finally {
    isTranscribing.value = false
  }
}

async function summarizeText() {
  if (!summarizationText.value.trim()) return

  isSummarizing.value = true
  try {
    // TODO: Call API
    await new Promise(resolve => setTimeout(resolve, 2000))
    summarizationResult.value = {
      summary: 'ملخص النص: يتناول هذا النص موضوعاً مهماً يتعلق بالتخطيط الاستراتيجي للمشروع.',
      keyPoints: [
        'النقطة الأولى: أهمية التخطيط المسبق',
        'النقطة الثانية: تحديد الأهداف بوضوح',
        'النقطة الثالثة: متابعة التنفيذ'
      ],
      wordCount: {
        original: summarizationText.value.split(' ').length,
        summary: 25
      }
    }
  } catch (error) {
    console.error('Summarization failed:', error)
  } finally {
    isSummarizing.value = false
  }
}

async function askQuestion() {
  if (!qaQuestion.value.trim()) return

  isAnswering.value = true
  try {
    // TODO: Call API
    await new Promise(resolve => setTimeout(resolve, 1500))
    qaAnswer.value = {
      answer: 'بناءً على المعلومات المتاحة، الإجابة على سؤالك هي...',
      confidence: 0.89,
      sources: [
        { title: 'دليل السياسات', section: 'الفصل الثالث' },
        { title: 'الإجراءات التشغيلية', section: 'القسم 2.1' }
      ]
    }
  } catch (error) {
    console.error('Q&A failed:', error)
  } finally {
    isAnswering.value = false
  }
}

function viewJobDetails(job: any) {
  selectedJob.value = job
  showJobDetailsDialog.value = true
}

function getStatusSeverity(status: string): "success" | "info" | "warn" | "danger" | "secondary" | "contrast" | undefined {
  switch (status) {
    case 'Completed': return 'success'
    case 'Processing': return 'info'
    case 'Failed': return 'danger'
    case 'Pending': return 'warn'
    default: return 'secondary'
  }
}

function formatDate(date: Date): string {
  return new Intl.DateTimeFormat(locale.value, {
    dateStyle: 'medium',
    timeStyle: 'short'
  }).format(date)
}

function copyToClipboard(text: string) {
  navigator.clipboard.writeText(text)
}

async function loadData() {
  try {
    error.value = null
    isLoading.value = true

    await Promise.all([
      fetchQuotaStatus(),
      fetchRecentJobs()
    ])

    await new Promise(resolve => setTimeout(resolve, LOADING_DELAY))
    isLoading.value = false

    if (shouldAnimate.value) {
      requestAnimationFrame(() => {
        isContentVisible.value = true
      })
    } else {
      isContentVisible.value = true
    }
  } catch (e) {
    error.value = e as Error
    isLoading.value = false
  }
}

async function handleRetry() {
  await loadData()
}

onMounted(() => {
  loadData()
})
</script>

<template>
  <div class="ai-services-view" :class="{ 'rtl': isRTL }">
    <!-- Header -->
    <div class="flex justify-content-between align-items-center mb-4">
      <div>
        <h1 class="text-2xl font-bold m-0">{{ t('ai.title') }}</h1>
        <p class="text-color-secondary mt-1">{{ t('ai.subtitle') }}</p>
      </div>
    </div>

    <!-- Loading State -->
    <LoadingSpinner
      v-if="isLoading"
      size="xl"
      variant="spinner"
      label="Loading..."
      label-arabic="جاري التحميل..."
      :show-label="true"
      centered
      class="loading-state"
    />

    <!-- Error State -->
    <ErrorState
      v-else-if="error"
      :error="error"
      :title="isRTL ? 'فشل تحميل خدمات الذكاء الاصطناعي' : 'Failed to load AI Services'"
      show-retry
      @retry="handleRetry"
    />

    <!-- Main Content -->
    <template v-else>
      <!-- Quota Card -->
    <Card class="mb-4">
      <template #content>
        <div class="flex align-items-center gap-4">
          <div class="flex-1">
            <div class="flex justify-content-between mb-2">
              <span class="font-medium">{{ t('ai.quota.usage') }}</span>
              <span class="text-color-secondary">
                {{ quotaStatus?.usedTokens?.toLocaleString() }} / {{ quotaStatus?.totalTokens?.toLocaleString() }} {{ t('ai.quota.tokens') }}
              </span>
            </div>
            <ProgressBar :value="quotaPercentage" :showValue="true" />
          </div>
          <div class="text-right">
            <div class="text-sm text-color-secondary">{{ t('ai.quota.resetDate') }}</div>
            <div class="font-medium" v-if="quotaStatus?.resetDate">
              {{ formatDate(quotaStatus.resetDate) }}
            </div>
          </div>
        </div>
      </template>
    </Card>

    <!-- Main Tabs -->
    <TabView v-model:activeIndex="activeTab">
      <!-- Transcription Tab -->
      <TabPanel value="0" :header="t('ai.transcription.title')">
        <div class="grid">
          <div class="col-12 md:col-6">
            <Card>
              <template #title>{{ t('ai.transcription.upload') }}</template>
              <template #content>
                <FileUpload
                  mode="basic"
                  accept="audio/*,video/*"
                  :maxFileSize="104857600"
                  :auto="false"
                  chooseLabel="اختر ملف"
                  @select="onFileSelect"
                  class="mb-3"
                />

                <div class="field mb-3">
                  <label class="block mb-2">{{ t('ai.transcription.language') }}</label>
                  <Dropdown
                    v-model="transcriptionLanguage"
                    :options="languageOptions"
                    optionLabel="label"
                    optionValue="value"
                    class="w-full"
                  />
                </div>

                <Button
                  :label="t('ai.transcription.start')"
                  icon="pi pi-play"
                  @click="startTranscription"
                  :loading="isTranscribing"
                  :disabled="!transcriptionFile"
                  class="w-full"
                />
              </template>
            </Card>
          </div>

          <div class="col-12 md:col-6">
            <Card>
              <template #title>{{ t('ai.transcription.result') }}</template>
              <template #content>
                <LoadingSpinner
                  v-if="isTranscribing"
                  size="lg"
                  variant="dots"
                  :label="t('ai.transcription.processing')"
                  :show-label="true"
                  class="py-4"
                />

                <div v-else-if="transcriptionResult">
                  <div class="flex justify-content-between align-items-center mb-3">
                    <Tag :value="`${t('ai.transcription.confidence')}: ${Math.round(transcriptionResult.confidence * 100)}%`" severity="success" />
                    <Button icon="pi pi-copy" text rounded @click="copyToClipboard(transcriptionResult.text)" />
                  </div>

                  <div class="surface-ground p-3 border-round mb-3" style="max-height: 200px; overflow-y: auto;">
                    {{ transcriptionResult.text }}
                  </div>

                  <div class="text-sm text-color-secondary">
                    {{ t('ai.transcription.segments') }}: {{ transcriptionResult.segments?.length }}
                  </div>
                </div>

                <div v-else class="text-center py-4 text-color-secondary">
                  <i class="pi pi-microphone text-4xl mb-3"></i>
                  <p>{{ t('ai.transcription.noResult') }}</p>
                </div>
              </template>
            </Card>
          </div>
        </div>
      </TabPanel>

      <!-- Summarization Tab -->
      <TabPanel value="1" :header="t('ai.summarization.title')">
        <div class="grid">
          <div class="col-12 md:col-6">
            <Card>
              <template #title>{{ t('ai.summarization.input') }}</template>
              <template #content>
                <div class="field mb-3">
                  <label class="block mb-2">{{ t('ai.summarization.text') }}</label>
                  <Textarea
                    v-model="summarizationText"
                    rows="8"
                    class="w-full"
                    :placeholder="t('ai.summarization.placeholder')"
                  />
                </div>

                <div class="grid mb-3">
                  <div class="col-6">
                    <label class="block mb-2">{{ t('ai.summarization.length') }}</label>
                    <Dropdown
                      v-model="summarizationLength"
                      :options="summaryLengthOptions"
                      optionLabel="label"
                      optionValue="value"
                      class="w-full"
                    />
                  </div>
                  <div class="col-6">
                    <label class="block mb-2">{{ t('ai.summarization.outputLanguage') }}</label>
                    <Dropdown
                      v-model="summarizationLanguage"
                      :options="languageOptions"
                      optionLabel="label"
                      optionValue="value"
                      class="w-full"
                    />
                  </div>
                </div>

                <Button
                  :label="t('ai.summarization.summarize')"
                  icon="pi pi-list"
                  @click="summarizeText"
                  :loading="isSummarizing"
                  :disabled="!summarizationText.trim()"
                  class="w-full"
                />
              </template>
            </Card>
          </div>

          <div class="col-12 md:col-6">
            <Card>
              <template #title>{{ t('ai.summarization.result') }}</template>
              <template #content>
                <LoadingSpinner
                  v-if="isSummarizing"
                  size="lg"
                  variant="dots"
                  :label="t('ai.summarization.processing')"
                  :show-label="true"
                  class="py-4"
                />

                <div v-else-if="summarizationResult">
                  <div class="surface-ground p-3 border-round mb-3">
                    {{ summarizationResult.summary }}
                  </div>

                  <div class="mb-3">
                    <h4 class="mt-0 mb-2">{{ t('ai.summarization.keyPoints') }}</h4>
                    <ul class="m-0 pl-4">
                      <li v-for="(point, index) in summarizationResult.keyPoints" :key="index" class="mb-1">
                        {{ point }}
                      </li>
                    </ul>
                  </div>

                  <div class="flex justify-content-between text-sm text-color-secondary">
                    <span>{{ t('ai.summarization.originalWords') }}: {{ summarizationResult.wordCount.original }}</span>
                    <span>{{ t('ai.summarization.summaryWords') }}: {{ summarizationResult.wordCount.summary }}</span>
                  </div>
                </div>

                <div v-else class="text-center py-4 text-color-secondary">
                  <i class="pi pi-file-edit text-4xl mb-3"></i>
                  <p>{{ t('ai.summarization.noResult') }}</p>
                </div>
              </template>
            </Card>
          </div>
        </div>
      </TabPanel>

      <!-- Q&A Tab -->
      <TabPanel value="2" :header="t('ai.qa.title')">
        <div class="grid">
          <div class="col-12 md:col-6">
            <Card>
              <template #title>{{ t('ai.qa.askQuestion') }}</template>
              <template #content>
                <div class="field mb-3">
                  <label class="block mb-2">{{ t('ai.qa.question') }}</label>
                  <InputText
                    v-model="qaQuestion"
                    class="w-full"
                    :placeholder="t('ai.qa.questionPlaceholder')"
                  />
                </div>

                <div class="field mb-3">
                  <label class="block mb-2">{{ t('ai.qa.context') }} ({{ t('common.optional') }})</label>
                  <Textarea
                    v-model="qaContext"
                    rows="4"
                    class="w-full"
                    :placeholder="t('ai.qa.contextPlaceholder')"
                  />
                </div>

                <Button
                  :label="t('ai.qa.ask')"
                  icon="pi pi-question-circle"
                  @click="askQuestion"
                  :loading="isAnswering"
                  :disabled="!qaQuestion.trim()"
                  class="w-full"
                />
              </template>
            </Card>
          </div>

          <div class="col-12 md:col-6">
            <Card>
              <template #title>{{ t('ai.qa.answer') }}</template>
              <template #content>
                <LoadingSpinner
                  v-if="isAnswering"
                  size="lg"
                  variant="pulse"
                  :label="t('ai.qa.thinking')"
                  :show-label="true"
                  class="py-4"
                />

                <div v-else-if="qaAnswer">
                  <div class="flex justify-content-between align-items-center mb-3">
                    <Tag :value="`${t('ai.qa.confidence')}: ${Math.round(qaAnswer.confidence * 100)}%`" severity="success" />
                  </div>

                  <div class="surface-ground p-3 border-round mb-3">
                    {{ qaAnswer.answer }}
                  </div>

                  <div v-if="qaAnswer.sources?.length">
                    <h4 class="mt-0 mb-2">{{ t('ai.qa.sources') }}</h4>
                    <div v-for="(source, index) in qaAnswer.sources" :key="index" class="flex align-items-center gap-2 mb-2">
                      <i class="pi pi-file text-color-secondary"></i>
                      <span>{{ source.title }} - {{ source.section }}</span>
                    </div>
                  </div>
                </div>

                <div v-else class="text-center py-4 text-color-secondary">
                  <i class="pi pi-comments text-4xl mb-3"></i>
                  <p>{{ t('ai.qa.noAnswer') }}</p>
                </div>
              </template>
            </Card>
          </div>
        </div>
      </TabPanel>

      <!-- Recent Jobs Tab -->
      <TabPanel value="3" :header="t('ai.jobs.title')">
        <Card>
          <template #content>
            <DataTable :value="recentJobs" :paginator="true" :rows="10" responsiveLayout="scroll">
              <Column field="type" :header="t('ai.jobs.type')" sortable />
              <Column field="fileName" :header="t('ai.jobs.fileName')" />
              <Column field="status" :header="t('ai.jobs.status')">
                <template #body="{ data }">
                  <Tag :value="data.status" :severity="getStatusSeverity(data.status)" />
                </template>
              </Column>
              <Column field="createdAt" :header="t('ai.jobs.createdAt')" sortable>
                <template #body="{ data }">
                  {{ formatDate(data.createdAt) }}
                </template>
              </Column>
              <Column :header="t('common.actions')">
                <template #body="{ data }">
                  <Button icon="pi pi-eye" text rounded @click="viewJobDetails(data)" />
                </template>
              </Column>
            </DataTable>
          </template>
        </Card>
      </TabPanel>
    </TabView>

    <!-- Job Details Dialog -->
    <Dialog
      v-model:visible="showJobDetailsDialog"
      :header="t('ai.jobs.details')"
      :style="{ width: '500px' }"
      :modal="true"
    >
      <div v-if="selectedJob">
        <div class="field">
          <label class="font-bold">{{ t('ai.jobs.type') }}</label>
          <p class="m-0">{{ selectedJob.type }}</p>
        </div>
        <div class="field">
          <label class="font-bold">{{ t('ai.jobs.fileName') }}</label>
          <p class="m-0">{{ selectedJob.fileName }}</p>
        </div>
        <div class="field">
          <label class="font-bold">{{ t('ai.jobs.status') }}</label>
          <Tag :value="selectedJob.status" :severity="getStatusSeverity(selectedJob.status)" />
        </div>
        <div class="field">
          <label class="font-bold">{{ t('ai.jobs.createdAt') }}</label>
          <p class="m-0">{{ formatDate(selectedJob.createdAt) }}</p>
        </div>
        <div class="field" v-if="selectedJob.duration">
          <label class="font-bold">{{ t('ai.jobs.duration') }}</label>
          <p class="m-0">{{ selectedJob.duration }}s</p>
        </div>
        <div class="field" v-if="selectedJob.progress !== undefined">
          <label class="font-bold">{{ t('ai.jobs.progress') }}</label>
          <ProgressBar :value="selectedJob.progress" />
        </div>
      </div>
    </Dialog>
    </template>
  </div>
</template>

<style scoped lang="scss">
@use '@/assets/styles/_variables.scss' as *;
@use '@/assets/styles/_mixins.scss' as *;

.ai-services-view {
  @include page-view;
  padding: $spacing-6;

  &.rtl {
    direction: rtl;
  }
}

// Loading state
.loading-state {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 400px;
}

.loading-spinner {
  text-align: center;
  color: $slate-500;

  i {
    margin-bottom: $spacing-4;
  }

  p {
    margin: 0;
    font-size: $text-base;
  }
}

// ============================================
// REDUCED MOTION SUPPORT
// ============================================

@media (prefers-reduced-motion: reduce) {
  .loading-spinner i {
    animation: none;
  }
}
</style>
