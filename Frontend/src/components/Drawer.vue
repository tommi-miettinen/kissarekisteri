<script lang="ts" setup>
import { ref, onMounted, watch } from "vue";
import { Offcanvas } from "bootstrap";

const props = defineProps({
  fullsize: {
    type: Boolean,
    default: false,
  },
  visible: {
    type: Boolean,
    default: false,
    required: true,
  },
});

const sideOffCanvas = ref<Offcanvas>();

onMounted(() => (sideOffCanvas.value = new Offcanvas(sideOffcanvasRef.value as HTMLDivElement)));

const sideOffcanvasRef = ref<HTMLDivElement>();

const emit = defineEmits(["onCancel"]);

watch(
  () => props.visible,
  (newValue) => {
    if (!sideOffCanvas.value) return;
    if (newValue) {
      sideOffCanvas.value.show();
    } else {
      sideOffCanvas.value.hide();
      emit("onCancel");
    }
  }
);
</script>

<template>
  <div
    :class="{ 'h-100': fullsize }"
    ref="sideOffcanvasRef"
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
