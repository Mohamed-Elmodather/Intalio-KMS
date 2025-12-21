<script setup lang="ts">
import { ref, computed } from 'vue'
import type { ContentBlock } from '@/composables/collaboration/useBlockEditor'
import Button from 'primevue/button'
import InputText from 'primevue/inputtext'
import FileUpload from 'primevue/fileupload'
import Dialog from 'primevue/dialog'

const props = defineProps<{
  block: ContentBlock
  readonly?: boolean
}>()

const emit = defineEmits<{
  (e: 'update', content: string): void
  (e: 'focus'): void
  (e: 'blur'): void
  (e: 'keydown', event: KeyboardEvent): void
}>()

const showUrlDialog = ref(false)
const imageUrl = ref('')
const altText = ref('')
const caption = ref('')

const metadata = computed(() => {
  if (!props.block.metadata) return null
  try {
    return JSON.parse(props.block.metadata)
  } catch {
    return null
  }
})

const hasImage = computed(() => !!metadata.value?.url)

const handleUrlSubmit = () => {
  if (imageUrl.value) {
    const newMetadata = {
      ...metadata.value,
      url: imageUrl.value,
      altText: altText.value,
      caption: caption.value
    }
    emit('update', JSON.stringify(newMetadata))
    showUrlDialog.value = false
  }
}

const handleFileUpload = (event: any) => {
  const file = event.files[0]
  if (file) {
    // In production, upload to storage service and get URL
    const reader = new FileReader()
    reader.onload = (e) => {
      const dataUrl = e.target?.result as string
      const newMetadata = {
        ...metadata.value,
        url: dataUrl,
        altText: file.name
      }
      emit('update', JSON.stringify(newMetadata))
    }
    reader.readAsDataURL(file)
  }
}

const openUrlDialog = () => {
  imageUrl.value = metadata.value?.url || ''
  altText.value = metadata.value?.altText || ''
  caption.value = metadata.value?.caption || ''
  showUrlDialog.value = true
}
</script>

<template>
  <div class="image-block" @click="emit('focus')">
    <!-- Image Display -->
    <div v-if="hasImage" class="image-container">
      <img
        :src="metadata?.url"
        :alt="metadata?.altText || ''"
        class="block-image"
      />
      <p v-if="metadata?.caption" class="image-caption">{{ metadata.caption }}</p>

      <div v-if="!readonly" class="image-actions">
        <Button
          icon="pi pi-pencil"
          severity="secondary"
          rounded
          size="small"
          @click="openUrlDialog"
          v-tooltip="'Edit image'"
        />
      </div>
    </div>

    <!-- Image Placeholder -->
    <div v-else class="image-placeholder">
      <i class="pi pi-image"></i>
      <p>Add an image</p>

      <div v-if="!readonly" class="image-upload-options">
        <FileUpload
          mode="basic"
          accept="image/*"
          :auto="true"
          choose-label="Upload"
          @select="handleFileUpload"
          class="upload-btn"
        />
        <Button
          label="Add URL"
          icon="pi pi-link"
          severity="secondary"
          @click="openUrlDialog"
        />
      </div>
    </div>

    <!-- URL Dialog -->
    <Dialog
      v-model:visible="showUrlDialog"
      header="Add Image"
      :style="{ width: '450px' }"
      modal
    >
      <div class="url-dialog-content">
        <div class="field">
          <label for="image-url">Image URL</label>
          <InputText
            id="image-url"
            v-model="imageUrl"
            placeholder="https://example.com/image.jpg"
            class="w-full"
          />
        </div>
        <div class="field">
          <label for="alt-text">Alt Text</label>
          <InputText
            id="alt-text"
            v-model="altText"
            placeholder="Describe the image"
            class="w-full"
          />
        </div>
        <div class="field">
          <label for="caption">Caption (optional)</label>
          <InputText
            id="caption"
            v-model="caption"
            placeholder="Image caption"
            class="w-full"
          />
        </div>
      </div>
      <template #footer>
        <Button label="Cancel" severity="secondary" @click="showUrlDialog = false" />
        <Button label="Add Image" @click="handleUrlSubmit" :disabled="!imageUrl" />
      </template>
    </Dialog>
  </div>
</template>

<style scoped>
.image-block {
  padding: 0.5rem;
}

.image-container {
  position: relative;
}

.block-image {
  max-width: 100%;
  height: auto;
  border-radius: var(--border-radius);
}

.image-caption {
  margin: 0.5rem 0 0;
  text-align: center;
  color: var(--text-color-secondary);
  font-size: 0.875rem;
}

.image-actions {
  position: absolute;
  top: 0.5rem;
  right: 0.5rem;
  display: flex;
  gap: 0.25rem;
  opacity: 0;
  transition: opacity 0.2s;
}

.image-container:hover .image-actions {
  opacity: 1;
}

.image-placeholder {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 2rem;
  border: 2px dashed var(--surface-border);
  border-radius: var(--border-radius);
  background: var(--surface-ground);
  text-align: center;
}

.image-placeholder i {
  font-size: 3rem;
  color: var(--text-color-secondary);
  margin-bottom: 1rem;
}

.image-placeholder p {
  margin: 0 0 1rem;
  color: var(--text-color-secondary);
}

.image-upload-options {
  display: flex;
  gap: 0.5rem;
}

.url-dialog-content {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.field {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.field label {
  font-weight: 500;
}
</style>
