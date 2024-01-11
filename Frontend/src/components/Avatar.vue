<script lang="ts" setup>
defineProps({
  backgroundColor: {
    type: String,
    default: "#818cf8",
  },
  avatarUrl: {
    type: String,
  },
  displayText: {
    type: String,
  },
});
</script>

<template>
  <div
    v-if="displayText || avatarUrl"
    tabindex="0"
    class="cursor-pointer rounded-circle focus-ring"
    @click="$emit('click')"
    @keyup.enter="$emit('keyup.enter')"
  >
    <img v-if="avatarUrl" class="rounded-circle" height="32" width="32" style="object-fit: fill" :src="avatarUrl" alt="User avatar" />
    <div
      v-else
      style="width: 32px; height: 32px; font-size: 14px"
      :style="{ backgroundColor: backgroundColor }"
      class="rounded-circle d-flex align-items-center justify-content-center fw-bold text-uppercase"
    >
      {{ displayText }}
    </div>
  </div>
</template>
