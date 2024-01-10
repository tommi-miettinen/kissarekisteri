<script lang="ts" setup>
import { ref, onMounted, watch } from "vue";
import { Offcanvas } from "bootstrap";

type Placement = "top" | "bottom" | "start" | "end";

const props = defineProps({
  placement: {
    type: String as () => Placement,
    default: "bottom",
  },
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
const sideOffcanvasRef = ref<HTMLDivElement>();
const emit = defineEmits(["onCancel"]);

onMounted(() => {
  if (!sideOffcanvasRef.value) return console.error("ref is null");
  sideOffCanvas.value = new Offcanvas(sideOffcanvasRef.value);
  sideOffcanvasRef.value.addEventListener("hide.bs.offcanvas", () => emit("onCancel"));
});

watch(
  () => props.visible,
  (newValue) => {
    if (newValue) {
      sideOffCanvas.value?.show();
    } else {
      sideOffCanvas.value?.hide();
    }
  }
);
</script>

<template>
  <div
    style="height: auto"
    :class="['offcanvas', `offcanvas-${props.placement}`, { 'h-100': props.fullsize, 'w-100': props.fullsize }]"
    ref="sideOffcanvasRef"
    tabindex="-1"
    aria-labelledby="offcanvasRightLabel"
  >
    <div class="offcanvas-header">
      <button type="button" class="btn-close ms-auto" @click="sideOffCanvas?.hide()" aria-label="Close"></button>
    </div>
    <div class="d-flex flex-column overflow-auto">
      <slot></slot>
    </div>
  </div>
</template>
