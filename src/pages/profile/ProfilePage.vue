<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'

const router = useRouter()

// Loading state
const isLoading = ref(false)

// Modal state
const showEditProfile = ref(false)

// Activity filter
const activityFilter = ref('all')

// User data
const user = ref({
  name: 'Ahmed Imam',
  initials: 'AI',
  role: 'Product Director',
  department: 'Product Management',
  location: 'Dubai, UAE',
  joinDate: 'March 2022',
  email: 'ahmed.imam@intalio.com',
  phone: '+971 50 123 4567',
  bio: 'Experienced product leader with 10+ years in enterprise software. Passionate about building user-centric products that drive business value. Leading the Intalio Knowledge Hub initiative to transform how organizations manage and share knowledge.',
  skills: ['Product Strategy', 'UX Design', 'Agile/Scrum', 'Data Analytics', 'Team Leadership', 'Enterprise Software', 'AI/ML Integration']
})

// Stats
const stats = ref({
  articles: 24,
  comments: 156,
  documents: 38,
  courses: 12
})

// Edit form
const editForm = ref({
  firstName: 'Ahmed',
  lastName: 'Imam',
  title: 'Product Director',
  bio: user.value.bio,
  phone: '+971 50 123 4567',
  location: 'Dubai, UAE'
})

// Contribution data for chart
const contributionData = ref([30, 45, 60, 75, 50, 85, 90, 70, 55, 80, 95, 60])

// Activities
const activities = ref([
  {
    type: 'Article',
    typeBadge: 'badge-blue',
    icon: 'fas fa-file-alt',
    iconBg: 'bg-blue-500',
    iconColor: 'text-white',
    title: 'Published "Complete Employee Onboarding Guide 2024"',
    description: '',
    time: '2 hours ago'
  },
  {
    type: 'Comment',
    typeBadge: 'badge-purple',
    icon: 'fas fa-comment',
    iconBg: 'bg-purple-500',
    iconColor: 'text-white',
    title: 'Commented on "Remote Work Best Practices"',
    description: 'Great insights on async communication!',
    time: '5 hours ago'
  },
  {
    type: 'Course',
    typeBadge: 'badge-green',
    icon: 'fas fa-graduation-cap',
    iconBg: 'bg-green-500',
    iconColor: 'text-white',
    title: 'Completed "Advanced Product Management"',
    description: '',
    time: 'Yesterday'
  },
  {
    type: 'Document',
    typeBadge: 'badge-orange',
    icon: 'fas fa-file-upload',
    iconBg: 'bg-orange-500',
    iconColor: 'text-white',
    title: 'Uploaded "Q4 Product Roadmap"',
    description: '',
    time: '2 days ago'
  }
])

// Badges
const badges = ref([
  { id: 1, name: 'Top Author', icon: 'fas fa-pen-fancy', bg: 'bg-blue-100', color: 'text-blue-600', description: 'Published 20+ articles' },
  { id: 2, name: 'Mentor', icon: 'fas fa-hands-helping', bg: 'bg-purple-100', color: 'text-purple-600', description: 'Helped 50+ colleagues' },
  { id: 3, name: 'Fast Learner', icon: 'fas fa-rocket', bg: 'bg-green-100', color: 'text-green-600', description: 'Completed 10 courses' },
  { id: 4, name: 'Innovator', icon: 'fas fa-lightbulb', bg: 'bg-yellow-100', color: 'text-yellow-600', description: 'Submitted 5 ideas' },
  { id: 5, name: 'Team Player', icon: 'fas fa-users', bg: 'bg-pink-100', color: 'text-pink-600', description: 'Active collaborator' },
  { id: 6, name: 'Explorer', icon: 'fas fa-compass', bg: 'bg-teal-100', color: 'text-teal-600', description: 'Used all features' }
])

const totalBadges = ref(15)

// Learning Progress
const learningProgress = ref([
  { id: 1, name: 'Leadership Essentials', progress: 85 },
  { id: 2, name: 'Data-Driven Decisions', progress: 60 },
  { id: 3, name: 'AI Fundamentals', progress: 40 }
])

// Certificates
const certificates = ref([
  { id: 1, name: 'Product Management Professional', date: 'Dec 2024' },
  { id: 2, name: 'Agile Scrum Master', date: 'Nov 2024' },
  { id: 3, name: 'UX Design Foundations', date: 'Oct 2024' }
])

// Team Members
const teamMembers = ref([
  { id: 1, name: 'Sarah Chen', initials: 'SC', color: '#8B5CF6', role: 'Engineering Lead', online: true },
  { id: 2, name: 'Mike Johnson', initials: 'MJ', color: '#3B82F6', role: 'Senior Designer', online: true },
  { id: 3, name: 'Emily Davis', initials: 'ED', color: '#10B981', role: 'Product Manager', online: false },
  { id: 4, name: 'Alex Thompson', initials: 'AT', color: '#F59E0B', role: 'Developer', online: true }
])

// Methods
const goToSettings = () => {
  router.push('/settings')
}

const goToLearning = () => {
  router.push('/learning')
}

const saveProfile = () => {
  showEditProfile.value = false
  // In a real app, this would save to the backend
  alert('Profile updated successfully!')
}
</script>

<template>
  <div>
    <!-- Loading State -->
    <div v-if="isLoading" class="flex items-center justify-center min-h-[60vh]">
      <LoadingSpinner size="lg" text="Loading profile..." />
    </div>

    <template v-else>
      <!-- Breadcrumb -->
      <nav class="flex items-center gap-2 text-sm text-gray-500 mb-6">
        <router-link to="/" class="hover:text-primary-600 transition-colors">
          <i class="fas fa-home"></i>
        </router-link>
        <i class="fas fa-chevron-right text-xs"></i>
        <span class="text-gray-900 font-medium">Profile</span>
      </nav>

      <!-- Profile Header -->
      <div class="card-animated fade-in-up rounded-2xl overflow-hidden mb-10" style="animation-delay: 0.1s">
        <!-- Cover Photo -->
        <div class="h-48 bg-gradient-to-r from-teal-500 via-teal-600 to-teal-700 relative">
          <button class="btn-vibrant ripple absolute bottom-4 right-4 px-4 py-2 bg-white/20 backdrop-blur-sm rounded-lg text-white text-sm hover:bg-white/30 transition-colors">
            <i class="fas fa-camera mr-2 icon-soft"></i>Change Cover
          </button>
        </div>

        <!-- Profile Info -->
        <div class="px-6 pb-6">
          <div class="flex flex-col md:flex-row md:items-end gap-4 -mt-16 relative z-10">
            <div class="relative">
              <div class="w-32 h-32 rounded-2xl bg-gradient-to-br from-teal-400 to-teal-600 flex items-center justify-center text-white text-4xl font-bold border-4 border-white shadow-xl">
                {{ user.initials }}
              </div>
              <button class="btn-vibrant ripple absolute -bottom-1 -right-1 w-10 h-10 rounded-full bg-teal-500 text-white flex items-center justify-center shadow-lg hover:bg-teal-600 transition-colors">
                <i class="fas fa-camera icon-vibrant"></i>
              </button>
              <span class="absolute top-2 right-2 w-4 h-4 bg-green-500 rounded-full border-2 border-white"></span>
            </div>

            <div class="flex-1 md:pb-2">
              <h1 class="text-2xl font-bold text-teal-900">{{ user.name }}</h1>
              <p class="text-teal-600">{{ user.role }}</p>
              <div class="flex flex-wrap items-center gap-4 mt-2 text-sm text-teal-500">
                <span><i class="fas fa-building mr-1 icon-soft"></i>{{ user.department }}</span>
                <span><i class="fas fa-map-marker-alt mr-1 icon-soft"></i>{{ user.location }}</span>
                <span><i class="fas fa-calendar mr-1 icon-soft"></i>Joined {{ user.joinDate }}</span>
              </div>
            </div>

            <div class="flex gap-3">
              <button @click="goToSettings" class="btn btn-secondary btn-vibrant ripple">
                <i class="fas fa-cog icon-soft"></i> Settings
              </button>
              <button @click="showEditProfile = true" class="btn btn-primary btn-vibrant ripple">
                <i class="fas fa-edit icon-vibrant"></i> Edit Profile
              </button>
            </div>
          </div>
        </div>
      </div>

      <div class="grid grid-cols-1 xl:grid-cols-3 gap-6">
        <!-- Left Column -->
        <div class="xl:col-span-2 space-y-6">
          <!-- About -->
          <div class="card-animated fade-in-up rounded-2xl p-6" style="animation-delay: 0.2s">
            <h2 class="text-lg font-semibold text-teal-900 mb-4">About</h2>
            <p class="text-teal-700 leading-relaxed">{{ user.bio }}</p>

            <!-- Skills -->
            <div class="mt-6">
              <h3 class="text-sm font-semibold text-teal-500 uppercase tracking-wider mb-3">Skills & Expertise</h3>
              <div class="flex flex-wrap gap-2">
                <span
                  v-for="skill in user.skills"
                  :key="skill"
                  class="px-3 py-1.5 bg-teal-50 rounded-full text-sm text-teal-700 hover:bg-teal-100 transition-colors cursor-pointer"
                >
                  {{ skill }}
                </span>
              </div>
            </div>

            <!-- Contact Info -->
            <div class="mt-6 grid grid-cols-1 md:grid-cols-2 gap-4">
              <div class="flex items-center gap-3 list-item-animated" style="animation-delay: 0.25s">
                <div class="w-10 h-10 rounded-lg bg-blue-100 flex items-center justify-center">
                  <i class="fas fa-envelope text-blue-600 icon-vibrant"></i>
                </div>
                <div>
                  <p class="text-xs text-teal-500">Email</p>
                  <p class="text-sm text-teal-800">{{ user.email }}</p>
                </div>
              </div>
              <div class="flex items-center gap-3 list-item-animated" style="animation-delay: 0.3s">
                <div class="w-10 h-10 rounded-lg bg-green-100 flex items-center justify-center">
                  <i class="fas fa-phone text-green-600 icon-vibrant"></i>
                </div>
                <div>
                  <p class="text-xs text-teal-500">Phone</p>
                  <p class="text-sm text-teal-800">{{ user.phone }}</p>
                </div>
              </div>
            </div>
          </div>

          <!-- Activity Timeline -->
          <div class="card-animated fade-in-up rounded-2xl p-6" style="animation-delay: 0.3s">
            <div class="flex items-center justify-between mb-6">
              <h2 class="text-lg font-semibold text-teal-900">Recent Activity</h2>
              <select v-model="activityFilter" class="input text-sm py-2 w-auto">
                <option value="all">All Activity</option>
                <option value="articles">Articles</option>
                <option value="comments">Comments</option>
                <option value="courses">Courses</option>
              </select>
            </div>

            <div class="relative">
              <!-- Timeline line -->
              <div class="absolute left-5 top-0 bottom-0 w-0.5 bg-teal-100"></div>

              <div
                v-for="(activity, idx) in activities"
                :key="idx"
                class="relative pl-14 pb-8 last:pb-0 list-item-animated"
                :style="{ 'animation-delay': (0.35 + idx * 0.05) + 's' }"
              >
                <div :class="['absolute left-2.5 w-5 h-5 rounded-full flex items-center justify-center', activity.iconBg]">
                  <i :class="[activity.icon, 'text-xs', activity.iconColor, 'icon-vibrant']"></i>
                </div>
                <div class="card-animated rounded-xl p-4 ml-2 hover:shadow-md transition-all ripple">
                  <div class="flex items-center justify-between mb-1">
                    <span :class="['badge text-xs', activity.typeBadge]">{{ activity.type }}</span>
                    <span class="text-xs text-teal-400">{{ activity.time }}</span>
                  </div>
                  <p class="text-teal-900 font-medium">{{ activity.title }}</p>
                  <p v-if="activity.description" class="text-sm text-teal-600 mt-1">{{ activity.description }}</p>
                </div>
              </div>
            </div>

            <button class="btn-vibrant ripple w-full mt-4 py-3 text-center text-teal-600 hover:text-teal-800 font-medium">
              View All Activity <i class="fas fa-arrow-right ml-1 icon-soft"></i>
            </button>
          </div>

          <!-- Contributions -->
          <div class="card-animated fade-in-up rounded-2xl p-6" style="animation-delay: 0.4s">
            <h2 class="text-lg font-semibold text-teal-900 mb-6">Contributions</h2>

            <div class="grid grid-cols-2 md:grid-cols-4 gap-4 mb-6">
              <div class="text-center p-4 rounded-xl bg-teal-50 list-item-animated ripple" style="animation-delay: 0.45s">
                <p class="text-2xl font-bold text-teal-600">{{ stats.articles }}</p>
                <p class="text-sm text-teal-500">Articles</p>
              </div>
              <div class="text-center p-4 rounded-xl bg-blue-50 list-item-animated ripple" style="animation-delay: 0.5s">
                <p class="text-2xl font-bold text-blue-600">{{ stats.comments }}</p>
                <p class="text-sm text-teal-500">Comments</p>
              </div>
              <div class="text-center p-4 rounded-xl bg-purple-50 list-item-animated ripple" style="animation-delay: 0.55s">
                <p class="text-2xl font-bold text-purple-600">{{ stats.documents }}</p>
                <p class="text-sm text-teal-500">Documents</p>
              </div>
              <div class="text-center p-4 rounded-xl bg-green-50 list-item-animated ripple" style="animation-delay: 0.6s">
                <p class="text-2xl font-bold text-green-600">{{ stats.courses }}</p>
                <p class="text-sm text-teal-500">Courses Completed</p>
              </div>
            </div>

            <!-- Contribution Chart (Placeholder) -->
            <div class="h-32 bg-teal-50 rounded-xl flex items-center justify-center">
              <div class="flex items-end gap-1 h-24">
                <div
                  v-for="(val, idx) in contributionData"
                  :key="idx"
                  :style="{ height: val + '%' }"
                  class="w-3 bg-teal-400 rounded-t hover:bg-teal-500 transition-colors cursor-pointer"
                ></div>
              </div>
            </div>
            <p class="text-center text-sm text-teal-500 mt-2">Last 12 months contribution activity</p>
          </div>
        </div>

        <!-- Right Column -->
        <div class="space-y-6">
          <!-- Badges & Achievements -->
          <div class="card-animated fade-in-up rounded-2xl p-6" style="animation-delay: 0.2s">
            <h2 class="text-lg font-semibold text-teal-900 mb-4">Badges & Achievements</h2>
            <div class="grid grid-cols-3 gap-4">
              <div
                v-for="(badge, idx) in badges"
                :key="badge.id"
                class="text-center group cursor-pointer list-item-animated ripple"
                :title="badge.description"
                :style="{ 'animation-delay': (0.25 + idx * 0.05) + 's' }"
              >
                <div :class="['w-14 h-14 rounded-xl mx-auto mb-2 flex items-center justify-center transition-transform group-hover:scale-110', badge.bg]">
                  <i :class="[badge.icon, badge.color, 'text-xl', 'icon-vibrant']"></i>
                </div>
                <p class="text-xs font-medium text-teal-700">{{ badge.name }}</p>
              </div>
            </div>
            <button class="btn-vibrant ripple w-full mt-4 py-2 text-center text-sm text-teal-600 hover:text-teal-800">
              View All Badges ({{ totalBadges }})
            </button>
          </div>

          <!-- Learning Progress -->
          <div class="card-animated fade-in-up rounded-2xl p-6" style="animation-delay: 0.3s">
            <h2 class="text-lg font-semibold text-teal-900 mb-4">Learning Progress</h2>
            <div class="space-y-4">
              <div
                v-for="(course, idx) in learningProgress"
                :key="course.id"
                class="list-item-animated"
                :style="{ 'animation-delay': (0.35 + idx * 0.05) + 's' }"
              >
                <div class="flex items-center justify-between mb-1">
                  <span class="text-sm text-teal-700">{{ course.name }}</span>
                  <span class="text-sm font-medium text-teal-600">{{ course.progress }}%</span>
                </div>
                <div class="progress">
                  <div class="progress-bar" :style="{ width: course.progress + '%' }"></div>
                </div>
              </div>
            </div>
            <button @click="goToLearning" class="btn-vibrant ripple block w-full mt-4 py-2 text-center text-sm text-teal-600 hover:text-teal-800">
              View All Courses <i class="fas fa-arrow-right ml-1 icon-soft"></i>
            </button>
          </div>

          <!-- Certificates -->
          <div class="card-animated fade-in-up rounded-2xl p-6" style="animation-delay: 0.4s">
            <h2 class="text-lg font-semibold text-teal-900 mb-4">Certificates</h2>
            <div class="space-y-3">
              <div
                v-for="(cert, idx) in certificates"
                :key="cert.id"
                class="flex items-center gap-3 p-3 rounded-xl bg-teal-50 hover:bg-teal-100 transition-colors cursor-pointer list-item-animated ripple"
                :style="{ 'animation-delay': (0.45 + idx * 0.05) + 's' }"
              >
                <div class="w-10 h-10 rounded-lg bg-gradient-to-br from-yellow-400 to-yellow-600 flex items-center justify-center">
                  <i class="fas fa-certificate text-white icon-vibrant"></i>
                </div>
                <div class="flex-1">
                  <p class="text-sm font-medium text-teal-900">{{ cert.name }}</p>
                  <p class="text-xs text-teal-500">{{ cert.date }}</p>
                </div>
                <button class="btn-vibrant ripple p-2 rounded-lg hover:bg-teal-200 text-teal-600">
                  <i class="fas fa-download icon-soft"></i>
                </button>
              </div>
            </div>
          </div>

          <!-- Team Members -->
          <div class="card-animated fade-in-up rounded-2xl p-6" style="animation-delay: 0.5s">
            <h2 class="text-lg font-semibold text-teal-900 mb-4">Team</h2>
            <div class="space-y-3">
              <div
                v-for="(member, idx) in teamMembers"
                :key="member.id"
                class="flex items-center gap-3 p-2 rounded-xl hover:bg-teal-50 transition-colors cursor-pointer list-item-animated ripple"
                :style="{ 'animation-delay': (0.55 + idx * 0.05) + 's' }"
              >
                <div class="relative">
                  <div
                    class="w-10 h-10 rounded-full flex items-center justify-center text-white font-medium"
                    :style="{ backgroundColor: member.color }"
                  >
                    {{ member.initials }}
                  </div>
                  <span
                    :class="[
                      'absolute -bottom-0.5 -right-0.5 w-3 h-3 rounded-full border-2 border-white',
                      member.online ? 'bg-green-500' : 'bg-gray-400'
                    ]"
                  ></span>
                </div>
                <div>
                  <p class="text-sm font-medium text-teal-900">{{ member.name }}</p>
                  <p class="text-xs text-teal-500">{{ member.role }}</p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Edit Profile Modal -->
      <Teleport to="body">
        <div v-if="showEditProfile" class="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
          <div class="card-animated fade-in-up rounded-2xl w-full max-w-2xl max-h-[90vh] overflow-y-auto bg-white">
            <div class="p-5 border-b border-teal-100 flex items-center justify-between sticky top-0 bg-white/80 backdrop-blur-sm">
              <h2 class="text-xl font-semibold text-teal-900">Edit Profile</h2>
              <button @click="showEditProfile = false" class="btn-vibrant ripple p-2 rounded-lg hover:bg-teal-100 text-teal-500">
                <i class="fas fa-times icon-soft"></i>
              </button>
            </div>
            <form @submit.prevent="saveProfile" class="p-6 space-y-5">
              <div class="grid grid-cols-2 gap-4">
                <div>
                  <label class="block text-sm font-medium text-teal-700 mb-2">First Name</label>
                  <input type="text" v-model="editForm.firstName" class="input" required>
                </div>
                <div>
                  <label class="block text-sm font-medium text-teal-700 mb-2">Last Name</label>
                  <input type="text" v-model="editForm.lastName" class="input" required>
                </div>
              </div>
              <div>
                <label class="block text-sm font-medium text-teal-700 mb-2">Job Title</label>
                <input type="text" v-model="editForm.title" class="input">
              </div>
              <div>
                <label class="block text-sm font-medium text-teal-700 mb-2">Bio</label>
                <textarea v-model="editForm.bio" class="input" rows="4"></textarea>
              </div>
              <div>
                <label class="block text-sm font-medium text-teal-700 mb-2">Phone</label>
                <input type="tel" v-model="editForm.phone" class="input">
              </div>
              <div>
                <label class="block text-sm font-medium text-teal-700 mb-2">Location</label>
                <input type="text" v-model="editForm.location" class="input">
              </div>
              <div class="flex gap-3 pt-4">
                <button type="button" @click="showEditProfile = false" class="btn btn-secondary btn-vibrant ripple flex-1">Cancel</button>
                <button type="submit" class="btn btn-primary btn-vibrant ripple flex-1">Save Changes</button>
              </div>
            </form>
          </div>
        </div>
      </Teleport>
    </template>
  </div>
</template>

<style scoped>
/* Badge styles for activity types */
.badge-blue {
  background-color: #dbeafe;
  color: #1d4ed8;
}

.badge-purple {
  background-color: #ede9fe;
  color: #7c3aed;
}

.badge-green {
  background-color: #dcfce7;
  color: #15803d;
}

.badge-orange {
  background-color: #ffedd5;
  color: #c2410c;
}

/* Progress bar styles */
.progress {
  height: 6px;
  background-color: #e5e7eb;
  border-radius: 9999px;
  overflow: hidden;
}

.progress-bar {
  height: 100%;
  background: linear-gradient(90deg, #14b8a6, #0d9488);
  border-radius: 9999px;
  transition: width 0.5s ease-out;
}

/* Cover photo button icon styling */
.h-48 .icon-soft {
  color: inherit;
}
</style>
