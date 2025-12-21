<template>
  <div class="video-editor-view">
    <!-- Header -->
    <div class="editor-header">
      <div class="header-left">
        <Button
          icon="pi pi-arrow-left"
          severity="secondary"
          text
          @click="goBack"
        />
        <div class="header-title">
          <h1>{{ $t('media.videoEditor') }}</h1>
          <span class="video-name">{{ video.fileName }}</span>
        </div>
      </div>
      <div class="header-actions">
        <Button
          :label="$t('media.preview')"
          icon="pi pi-eye"
          severity="secondary"
          outlined
          @click="previewEdit"
        />
        <Button
          :label="$t('media.process')"
          icon="pi pi-check"
          @click="processEdit"
          :loading="isProcessing"
        />
      </div>
    </div>

    <!-- Main Content -->
    <div class="editor-content">
      <!-- Video Preview -->
      <div class="video-preview">
        <div class="video-container">
          <video
            ref="videoRef"
            :src="video.url"
            @timeupdate="onTimeUpdate"
            @loadedmetadata="onVideoLoaded"
          />
          <div class="video-controls">
            <Button
              :icon="isPlaying ? 'pi pi-pause' : 'pi pi-play'"
              severity="secondary"
              rounded
              @click="togglePlay"
            />
            <div class="time-display">
              {{ formatTime(currentTime) }} / {{ formatTime(duration) }}
            </div>
            <Slider
              v-model="currentTime"
              :min="0"
              :max="duration"
              :step="0.1"
              class="timeline-slider"
              @change="seekTo"
            />
            <Button
              icon="pi pi-volume-up"
              severity="secondary"
              text
            />
            <Button
              icon="pi pi-expand"
              severity="secondary"
              text
              @click="toggleFullscreen"
            />
          </div>
        </div>
      </div>

      <!-- Editor Tools -->
      <div class="editor-tools">
        <TabView>
          <!-- Trim Tab -->
          <TabPanel value="0" :header="$t('media.trim')">
            <div class="tool-section">
              <p class="tool-description">{{ $t('media.trimDescription') }}</p>
              <div class="trim-controls">
                <div class="time-inputs">
                  <div class="time-input">
                    <label>{{ $t('media.startTime') }}</label>
                    <InputText
                      v-model="trimSettings.startTime"
                      placeholder="00:00:00"
                    />
                    <Button
                      icon="pi pi-clock"
                      severity="secondary"
                      text
                      @click="setStartToCurrent"
                      v-tooltip="'Set to current time'"
                    />
                  </div>
                  <div class="time-input">
                    <label>{{ $t('media.endTime') }}</label>
                    <InputText
                      v-model="trimSettings.endTime"
                      placeholder="00:00:00"
                    />
                    <Button
                      icon="pi pi-clock"
                      severity="secondary"
                      text
                      @click="setEndToCurrent"
                      v-tooltip="'Set to current time'"
                    />
                  </div>
                </div>
                <div class="trim-range">
                  <Slider
                    v-model="trimRange"
                    :range="true"
                    :min="0"
                    :max="duration"
                    class="range-slider"
                  />
                </div>
                <div class="trim-preview">
                  <span>{{ $t('media.newDuration') }}: {{ formatTime(trimRange[1] - trimRange[0]) }}</span>
                </div>
              </div>
            </div>
          </TabPanel>

          <!-- Text Overlay Tab -->
          <TabPanel value="1" :header="$t('media.textOverlay')">
            <div class="tool-section">
              <div class="overlays-list">
                <div
                  v-for="(overlay, index) in textOverlays"
                  :key="index"
                  class="overlay-item"
                >
                  <div class="overlay-header">
                    <span>Text Overlay {{ index + 1 }}</span>
                    <Button
                      icon="pi pi-trash"
                      severity="danger"
                      text
                      size="small"
                      @click="removeTextOverlay(index)"
                    />
                  </div>
                  <div class="overlay-form">
                    <div class="form-field">
                      <label>{{ $t('media.text') }}</label>
                      <InputText v-model="overlay.text" class="w-full" />
                    </div>
                    <div class="form-row">
                      <div class="form-field">
                        <label>{{ $t('media.font') }}</label>
                        <Select
                          v-model="overlay.fontFamily"
                          :options="fontOptions"
                          class="w-full"
                        />
                      </div>
                      <div class="form-field">
                        <label>{{ $t('media.fontSize') }}</label>
                        <InputNumber v-model="overlay.fontSize" :min="12" :max="120" class="w-full" />
                      </div>
                    </div>
                    <div class="form-row">
                      <div class="form-field">
                        <label>{{ $t('media.fontColor') }}</label>
                        <ColorPicker v-model="overlay.fontColor" />
                      </div>
                      <div class="form-field">
                        <label>{{ $t('media.position') }}</label>
                        <Select
                          v-model="overlay.position"
                          :options="positionOptions"
                          optionLabel="label"
                          optionValue="value"
                          class="w-full"
                        />
                      </div>
                    </div>
                    <div class="form-row">
                      <div class="form-field">
                        <label>{{ $t('media.startTime') }}</label>
                        <InputText v-model="overlay.startTime" placeholder="00:00:00" />
                      </div>
                      <div class="form-field">
                        <label>{{ $t('media.endTime') }}</label>
                        <InputText v-model="overlay.endTime" placeholder="00:00:00" />
                      </div>
                    </div>
                  </div>
                </div>
              </div>
              <Button
                :label="$t('media.addTextOverlay')"
                icon="pi pi-plus"
                severity="secondary"
                outlined
                @click="addTextOverlay"
              />
            </div>
          </TabPanel>

          <!-- Watermark Tab -->
          <TabPanel value="2" :header="$t('media.watermark')">
            <div class="tool-section">
              <p class="tool-description">{{ $t('media.watermarkDescription') }}</p>
              <div class="watermark-settings">
                <div class="form-field">
                  <label>{{ $t('media.watermarkImage') }}</label>
                  <div class="image-select">
                    <div
                      v-if="watermarkSettings.imageUrl"
                      class="selected-image"
                    >
                      <img :src="watermarkSettings.imageUrl" alt="Watermark" />
                      <Button
                        icon="pi pi-times"
                        severity="danger"
                        text
                        rounded
                        size="small"
                        @click="watermarkSettings.imageUrl = null"
                      />
                    </div>
                    <Button
                      v-else
                      :label="$t('media.selectImage')"
                      icon="pi pi-image"
                      severity="secondary"
                      outlined
                      @click="selectWatermarkImage"
                    />
                  </div>
                </div>
                <div class="form-field">
                  <label>{{ $t('media.position') }}</label>
                  <div class="position-grid">
                    <Button
                      v-for="pos in positionOptions"
                      :key="pos.value"
                      :icon="pos.icon"
                      :severity="watermarkSettings.position === pos.value ? 'primary' : 'secondary'"
                      :outlined="watermarkSettings.position !== pos.value"
                      @click="watermarkSettings.position = pos.value"
                      v-tooltip="pos.label"
                    />
                  </div>
                </div>
                <div class="form-field">
                  <label>{{ $t('media.opacity') }}: {{ watermarkSettings.opacity }}%</label>
                  <Slider v-model="watermarkSettings.opacity" :min="10" :max="100" />
                </div>
              </div>
            </div>
          </TabPanel>

          <!-- Convert Tab -->
          <TabPanel value="3" :header="$t('media.convert')">
            <div class="tool-section">
              <p class="tool-description">{{ $t('media.convertDescription') }}</p>
              <div class="convert-settings">
                <div class="form-field">
                  <label>{{ $t('media.outputFormat') }}</label>
                  <Select
                    v-model="convertSettings.format"
                    :options="formatOptions"
                    optionLabel="label"
                    optionValue="value"
                    class="w-full"
                  />
                </div>
                <div class="form-field">
                  <label>{{ $t('media.quality') }}</label>
                  <Select
                    v-model="convertSettings.quality"
                    :options="qualityOptions"
                    optionLabel="label"
                    optionValue="value"
                    class="w-full"
                  />
                </div>
                <div class="output-info">
                  <div class="info-item">
                    <span class="label">{{ $t('media.estimatedSize') }}</span>
                    <span class="value">~{{ estimatedSize }}</span>
                  </div>
                  <div class="info-item">
                    <span class="label">{{ $t('media.resolution') }}</span>
                    <span class="value">{{ getResolution(convertSettings.quality) }}</span>
                  </div>
                </div>
              </div>
            </div>
          </TabPanel>

          <!-- Extract Audio Tab -->
          <TabPanel value="4" :header="$t('media.extractAudio')">
            <div class="tool-section">
              <p class="tool-description">{{ $t('media.extractAudioDescription') }}</p>
              <div class="extract-settings">
                <div class="form-field">
                  <label>{{ $t('media.audioFormat') }}</label>
                  <Select
                    v-model="extractSettings.format"
                    :options="audioFormatOptions"
                    optionLabel="label"
                    optionValue="value"
                    class="w-full"
                  />
                </div>
                <div class="audio-info">
                  <i class="pi pi-volume-up"></i>
                  <div class="info-text">
                    <span>{{ $t('media.audioTrackInfo') }}</span>
                    <span class="details">AAC, 128kbps, Stereo</span>
                  </div>
                </div>
              </div>
            </div>
          </TabPanel>
        </TabView>
      </div>
    </div>

    <!-- Jobs Queue Panel -->
    <div v-if="activeJobs.length > 0" class="jobs-panel">
      <h3>{{ $t('media.processingJobs') }}</h3>
      <div class="jobs-list">
        <div
          v-for="job in activeJobs"
          :key="job.id"
          class="job-item"
        >
          <div class="job-info">
            <span class="job-name">{{ job.name }}</span>
            <Tag :value="job.status" :severity="getJobStatusSeverity(job.status)" />
          </div>
          <ProgressBar
            v-if="job.status === 'Processing'"
            :value="job.progress"
            :showValue="true"
          />
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'

const route = useRoute()
const router = useRouter()

const videoRef = ref<HTMLVideoElement | null>(null)
const isPlaying = ref(false)
const isProcessing = ref(false)
const currentTime = ref(0)
const duration = ref(300) // 5 minutes default
const trimRange = ref([0, 300])

const video = ref({
  id: route.query.mediaId || '1',
  fileName: 'opening_ceremony_full.mp4',
  url: '/media/videos/sample.mp4',
  duration: 300,
  width: 1920,
  height: 1080
})

const trimSettings = ref({
  startTime: '00:00:00',
  endTime: '00:05:00'
})

const textOverlays = ref<any[]>([])

const watermarkSettings = ref({
  imageUrl: null as string | null,
  position: 'BottomRight',
  opacity: 50
})

const convertSettings = ref({
  format: 'mp4',
  quality: 'HD'
})

const extractSettings = ref({
  format: 'mp3'
})

const activeJobs = ref([
  {
    id: '1',
    name: 'Opening Ceremony Highlight',
    status: 'Processing',
    progress: 65
  }
])

const fontOptions = ['Arial', 'Helvetica', 'Times New Roman', 'Georgia', 'Verdana']

const positionOptions = [
  { value: 'TopLeft', label: 'Top Left', icon: 'pi pi-arrow-up-left' },
  { value: 'TopCenter', label: 'Top Center', icon: 'pi pi-arrow-up' },
  { value: 'TopRight', label: 'Top Right', icon: 'pi pi-arrow-up-right' },
  { value: 'MiddleLeft', label: 'Middle Left', icon: 'pi pi-arrow-left' },
  { value: 'Center', label: 'Center', icon: 'pi pi-circle' },
  { value: 'MiddleRight', label: 'Middle Right', icon: 'pi pi-arrow-right' },
  { value: 'BottomLeft', label: 'Bottom Left', icon: 'pi pi-arrow-down-left' },
  { value: 'BottomCenter', label: 'Bottom Center', icon: 'pi pi-arrow-down' },
  { value: 'BottomRight', label: 'Bottom Right', icon: 'pi pi-arrow-down-right' }
]

const formatOptions = [
  { value: 'mp4', label: 'MP4 (H.264)' },
  { value: 'webm', label: 'WebM (VP9)' },
  { value: 'mov', label: 'MOV (ProRes)' },
  { value: 'avi', label: 'AVI' }
]

const qualityOptions = [
  { value: 'Low', label: '360p (Low)' },
  { value: 'SD', label: '480p (SD)' },
  { value: 'HD', label: '720p (HD)' },
  { value: 'FullHD', label: '1080p (Full HD)' },
  { value: 'Original', label: 'Original Quality' }
]

const audioFormatOptions = [
  { value: 'mp3', label: 'MP3' },
  { value: 'aac', label: 'AAC' },
  { value: 'wav', label: 'WAV (Uncompressed)' },
  { value: 'ogg', label: 'OGG Vorbis' }
]

const estimatedSize = computed(() => {
  const qualitySizes: Record<string, string> = {
    Low: '50 MB',
    SD: '100 MB',
    HD: '200 MB',
    FullHD: '400 MB',
    Original: '500 MB'
  }
  return qualitySizes[convertSettings.value.quality] || '200 MB'
})

function formatTime(seconds: number): string {
  const hrs = Math.floor(seconds / 3600)
  const mins = Math.floor((seconds % 3600) / 60)
  const secs = Math.floor(seconds % 60)
  if (hrs > 0) {
    return `${hrs}:${mins.toString().padStart(2, '0')}:${secs.toString().padStart(2, '0')}`
  }
  return `${mins}:${secs.toString().padStart(2, '0')}`
}

function getResolution(quality: string): string {
  const resolutions: Record<string, string> = {
    Low: '640x360',
    SD: '854x480',
    HD: '1280x720',
    FullHD: '1920x1080',
    Original: '1920x1080'
  }
  return resolutions[quality] || '1280x720'
}

function getJobStatusSeverity(status: string) {
  const severities: Record<string, string> = {
    Pending: 'secondary',
    Queued: 'info',
    Processing: 'warning',
    Completed: 'success',
    Failed: 'danger'
  }
  return severities[status] || 'secondary'
}

function goBack() {
  router.back()
}

function togglePlay() {
  if (videoRef.value) {
    if (isPlaying.value) {
      videoRef.value.pause()
    } else {
      videoRef.value.play()
    }
    isPlaying.value = !isPlaying.value
  }
}

function onTimeUpdate() {
  if (videoRef.value) {
    currentTime.value = videoRef.value.currentTime
  }
}

function onVideoLoaded() {
  if (videoRef.value) {
    duration.value = videoRef.value.duration
    trimRange.value = [0, duration.value]
  }
}

function seekTo(value: number | number[]) {
  if (videoRef.value && typeof value === 'number') {
    videoRef.value.currentTime = value
  }
}

function setStartToCurrent() {
  trimSettings.value.startTime = formatTime(currentTime.value)
  trimRange.value[0] = currentTime.value
}

function setEndToCurrent() {
  trimSettings.value.endTime = formatTime(currentTime.value)
  trimRange.value[1] = currentTime.value
}

function toggleFullscreen() {
  if (videoRef.value) {
    videoRef.value.requestFullscreen()
  }
}

function addTextOverlay() {
  textOverlays.value.push({
    text: '',
    fontFamily: 'Arial',
    fontSize: 32,
    fontColor: '#FFFFFF',
    position: 'BottomCenter',
    startTime: '00:00:00',
    endTime: formatTime(duration.value)
  })
}

function removeTextOverlay(index: number) {
  textOverlays.value.splice(index, 1)
}

function selectWatermarkImage() {
  // Open image selector
  watermarkSettings.value.imageUrl = '/media/watermark/logo.png'
}

function previewEdit() {
  console.log('Preview edit')
}

function processEdit() {
  isProcessing.value = true
  setTimeout(() => {
    isProcessing.value = false
    activeJobs.value.push({
      id: String(Date.now()),
      name: 'Video Edit Job',
      status: 'Queued',
      progress: 0
    })
  }, 1000)
}

onMounted(() => {
  // Load video data
})
</script>

<style scoped>
.video-editor-view {
  display: flex;
  flex-direction: column;
  height: calc(100vh - 120px);
  margin: -1.5rem;
}

.editor-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem 1.5rem;
  background: var(--p-surface-card);
  border-bottom: 1px solid var(--p-surface-border);
}

.header-left {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.header-title h1 {
  margin: 0;
  font-size: 1.25rem;
}

.video-name {
  font-size: 0.875rem;
  color: var(--p-text-muted-color);
}

.header-actions {
  display: flex;
  gap: 0.5rem;
}

.editor-content {
  flex: 1;
  display: grid;
  grid-template-columns: 1fr 400px;
  gap: 0;
  overflow: hidden;
}

.video-preview {
  background: #000;
  display: flex;
  flex-direction: column;
}

.video-container {
  flex: 1;
  display: flex;
  flex-direction: column;
}

.video-container video {
  flex: 1;
  width: 100%;
  object-fit: contain;
  background: #000;
}

.video-controls {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 1rem;
  background: rgba(0, 0, 0, 0.9);
}

.time-display {
  color: white;
  font-size: 0.875rem;
  min-width: 100px;
}

.timeline-slider {
  flex: 1;
}

.editor-tools {
  background: var(--p-surface-card);
  border-left: 1px solid var(--p-surface-border);
  overflow-y: auto;
}

.tool-section {
  padding: 1rem;
}

.tool-description {
  margin: 0 0 1rem 0;
  color: var(--p-text-muted-color);
  font-size: 0.875rem;
}

.trim-controls {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.time-inputs {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
}

.time-input {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.time-input label {
  font-size: 0.875rem;
  font-weight: 500;
}

.time-input :deep(.p-inputtext) {
  flex: 1;
}

.time-input {
  display: flex;
  flex-direction: row;
  align-items: flex-end;
  gap: 0.5rem;
}

.trim-range {
  padding: 1rem 0;
}

.range-slider {
  width: 100%;
}

.trim-preview {
  text-align: center;
  padding: 0.75rem;
  background: var(--p-surface-ground);
  border-radius: var(--p-border-radius);
  font-weight: 500;
}

.overlays-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  margin-bottom: 1rem;
}

.overlay-item {
  padding: 1rem;
  background: var(--p-surface-ground);
  border-radius: var(--p-border-radius);
}

.overlay-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
  font-weight: 600;
}

.overlay-form {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.form-field {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.form-field label {
  font-size: 0.875rem;
  font-weight: 500;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 0.75rem;
}

.w-full {
  width: 100%;
}

.watermark-settings, .convert-settings, .extract-settings {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.image-select {
  display: flex;
  align-items: center;
}

.selected-image {
  position: relative;
  width: 120px;
  height: 80px;
  border-radius: var(--p-border-radius);
  overflow: hidden;
}

.selected-image img {
  width: 100%;
  height: 100%;
  object-fit: contain;
  background: var(--p-surface-ground);
}

.selected-image button {
  position: absolute;
  top: 4px;
  right: 4px;
}

.position-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 0.5rem;
}

.output-info, .audio-info {
  padding: 1rem;
  background: var(--p-surface-ground);
  border-radius: var(--p-border-radius);
}

.output-info {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.output-info .info-item {
  display: flex;
  justify-content: space-between;
}

.output-info .label {
  color: var(--p-text-muted-color);
}

.output-info .value {
  font-weight: 500;
}

.audio-info {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.audio-info i {
  font-size: 2rem;
  color: var(--p-primary-color);
}

.audio-info .info-text {
  display: flex;
  flex-direction: column;
}

.audio-info .details {
  font-size: 0.875rem;
  color: var(--p-text-muted-color);
}

.jobs-panel {
  padding: 1rem 1.5rem;
  background: var(--p-surface-card);
  border-top: 1px solid var(--p-surface-border);
}

.jobs-panel h3 {
  margin: 0 0 1rem 0;
  font-size: 1rem;
}

.jobs-list {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.job-item {
  padding: 0.75rem;
  background: var(--p-surface-ground);
  border-radius: var(--p-border-radius);
}

.job-info {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.5rem;
}

.job-name {
  font-weight: 500;
}
</style>
