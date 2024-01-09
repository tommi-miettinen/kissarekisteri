<script lang="ts" setup>
import { ref } from "vue";
defineProps({
  avatarUrl: {
    type: String,
  },
  displayText: {
    type: String,
  },
});

const avatarLoadError = ref(false);
</script>

<template>
  <img
    v-if="avatarUrl && !avatarLoadError"
    class="rounded-circle"
    height="32"
    width="32"
    style="object-fit: fill"
    :src="avatarUrl"
    alt="User avatar"
    :onerror="(avatarLoadError = true)"
  />
  <div
    @click="$emit('onClick')"
    style="width: 32px; height: 32px; font-size: 14px"
    class="rounded-circle d-flex align-items-center justify-content-center bg-primary fw-bold text-uppercase"
  >
    {{ displayText }}
  </div>
</template>
