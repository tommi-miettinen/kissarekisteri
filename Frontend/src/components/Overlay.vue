<script lang="ts" setup>
import { watch, ref, nextTick } from "vue";

const emit = defineEmits(["onCancel"]);
const props = defineProps({
  visible: {
    type: Boolean,
    required: true,
  },
  headerText: {
    type: String,
    required: false,
  },
});

const internalVisible = ref(false);

watch(
  () => props.visible,
  async () => {
    if (!props.visible) {
      await nextTick();
      setTimeout(() => {
        internalVisible.value = false;
        emit("onCancel");
      }, 300);
    } else {
      internalVisible.value = true;
    }
  }
);
</script>

<template>
  <div
    v-if="internalVisible"
    :class="[visible ? 'fade-in' : 'fade-out']"
    class="position-fixed"
    style="width: 100vw; height: 100vh; background-color: white; left: 0px; top: 0px; z-index: 1000"
  >
    <div class="d-flex p-3">
      <button type="button" class="rounded-circle p-2 btn-close ms-auto" @click="$emit('onCancel')" aria-label="Close"></button>
    </div>
    <slot></slot>
  </div>
</template>

<style>
.fade-in {
  animation: fade-in 0.3s ease-out;
}

.fade-out {
  animation: fade-out 0.3s ease-out forwards;
}

@keyframes fade-in {
  from {
    transform: scale(0.95);
    opacity: 0;
  }
  to {
    transform: scale(1);
    opacity: 1;
  }
}

@keyframes fade-out {
  from {
    opacity: 1;
  }
  to {
    opacity: 0;
  }
}
</style>
