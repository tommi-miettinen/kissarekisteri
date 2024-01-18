<script lang="ts" setup>
defineProps({
  focusable: {
    type: Boolean,
    default: true,
  },
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
    :tabindex="focusable ? 0 : undefined"
    :class="{ 'cursor-pointer': focusable }"
    class="rounded-circle focus-ring"
    @click="$emit('click', $event)"
    @keyup.enter="$emit('keyup.enter', $event)"
    @keydown.enter="$emit('keydown.enter', $event)"
  >
    <img
      v-if="avatarUrl"
      class="rounded-circle"
      height="32"
      width="32"
      style="object-fit: fill; pointer-events: none"
      :src="avatarUrl"
      alt="User avatar"
    />
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
