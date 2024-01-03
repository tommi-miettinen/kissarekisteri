<script lang="ts" setup>
import { ref, onMounted, watch } from "vue";
import { Offcanvas } from "bootstrap";

const props = defineProps({
  visible: {
    type: Boolean,
    default: false,
    required: true,
  },
});

const sideOffCanvas = ref<Offcanvas>();

onMounted(() => (sideOffCanvas.value = new Offcanvas(sideOffcanvasRef.value as HTMLDivElement)));

const sideOffcanvasRef = ref<HTMLDivElement>();

watch(
  () => props.visible,
  () => sideOffCanvas.value?.toggle()
);
</script>

<template>
  <div
    ref="sideOffcanvasRef"
    style="height: 100%"
    class="offcanvas offcanvas-bottom w-100"
    tabindex="-1"
    aria-labelledby="offcanvasRightLabel"
  >
    <div class="offcanvas-header">
      <button type="button" class="btn-close ms-auto" @click="sideOffCanvas?.toggle()" aria-label="Close"></button>
    </div>
    <div class="d-flex flex-column">
      <slot></slot>
    </div>
  </div>
</template>
