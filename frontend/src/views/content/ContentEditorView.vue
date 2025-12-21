<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRoute, useRouter } from 'vue-router'
import Card from 'primevue/card'
import Button from 'primevue/button'
import InputText from 'primevue/inputtext'
import Dropdown from 'primevue/dropdown'
import Editor from 'primevue/editor'
import Breadcrumb from 'primevue/breadcrumb'

const { t } = useI18n()
const route = useRoute()
const router = useRouter()

const isEditMode = computed(() => !!route.params.id)

const article = ref({
  title: '',
  titleArabic: '',
  content: '',
  contentArabic: '',
  category: null as string | null,
  tags: [] as string[],
  status: 'draft'
})

const categories = ref([
  { label: 'News', value: 'news' },
  { label: 'Articles', value: 'articles' },
  { label: 'Announcements', value: 'announcements' }
])

const isSaving = ref(false)

const breadcrumbItems = computed(() => [
  { label: t('nav.content'), to: '/content' },
  { label: isEditMode.value ? t('content.edit') : t('content.create') }
])

async function saveArticle() {
  isSaving.value = true
  try {
    // TODO: Call API
    await new Promise(resolve => setTimeout(resolve, 1000))
    router.push('/content')
  } finally {
    isSaving.value = false
  }
}

function cancel() {
  router.push('/content')
}

onMounted(async () => {
  if (isEditMode.value) {
    // TODO: Load article data
  }
})
</script>

<template>
  <div class="content-editor-view">
    <Breadcrumb :model="breadcrumbItems" class="mb-4" />

    <Card>
      <template #title>
        <div class="flex align-items-center justify-content-between">
          <span>{{ isEditMode ? t('content.editArticle') : t('content.createArticle') }}</span>
        </div>
      </template>
      <template #content>
        <div class="grid">
          <div class="col-12 md:col-6">
            <div class="field">
              <label for="title">{{ t('content.title') }} (English)</label>
              <InputText id="title" v-model="article.title" class="w-full" />
            </div>
          </div>
          <div class="col-12 md:col-6">
            <div class="field">
              <label for="titleArabic">{{ t('content.title') }} (Arabic)</label>
              <InputText id="titleArabic" v-model="article.titleArabic" class="w-full" dir="rtl" />
            </div>
          </div>
          <div class="col-12 md:col-6">
            <div class="field">
              <label for="category">{{ t('content.category') }}</label>
              <Dropdown
                id="category"
                v-model="article.category"
                :options="categories"
                optionLabel="label"
                optionValue="value"
                :placeholder="t('content.selectCategory')"
                class="w-full"
              />
            </div>
          </div>
          <div class="col-12">
            <div class="field">
              <label>{{ t('content.content') }} (English)</label>
              <Editor v-model="article.content" editorStyle="height: 300px" />
            </div>
          </div>
          <div class="col-12">
            <div class="field">
              <label>{{ t('content.content') }} (Arabic)</label>
              <Editor v-model="article.contentArabic" editorStyle="height: 300px" />
            </div>
          </div>
        </div>

        <div class="flex justify-content-end gap-2 mt-4">
          <Button :label="t('common.cancel')" severity="secondary" @click="cancel" />
          <Button :label="t('common.save')" icon="pi pi-save" :loading="isSaving" @click="saveArticle" />
        </div>
      </template>
    </Card>
  </div>
</template>

<style scoped>
.content-editor-view {
  padding: 1.5rem;
}

.field {
  margin-bottom: 1rem;
}

.field label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 500;
}
</style>
