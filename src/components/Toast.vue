<template>
  <div v-if="visible" class="toast">{{ message }}</div>
</template>

<script setup>
import { ref, watch } from 'vue'

const props = defineProps({
  message: String,
  duration: {
    type: Number,
    default: 3000
  }
})

const visible = ref(false)

watch(() => props.message, (newMessage) => {
  if (newMessage) {
    visible.value = true
    setTimeout(() => {
      visible.value = false
    }, props.duration)
  }
})
</script>

<style scoped>
.toast {
  position: fixed;
  bottom: 30px;
  right: 30px;
  background-color: #ff8800;
  color: white;
  padding: 1rem 1.5rem;
  border-radius: 8px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  z-index: 9999;
  animation: fadeIn 0.3s ease-in-out;
  font-weight: bold;
}

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(10px); }
  to { opacity: 1; transform: translateY(0); }
}
</style>
