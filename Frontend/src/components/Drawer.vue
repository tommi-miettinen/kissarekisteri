<script lang="ts" setup>
import { ref, onMounted, watch, reactive } from "vue";
import { Offcanvas } from "bootstrap";
import { useSwipe, useElementBounding } from "@vueuse/core";

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

const drawerStyle = reactive({
  top: "0%",
  transition: "all 0.3s ease-in-out",
});

const { height } = useElementBounding(sideOffcanvasRef);
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
      drawerStyle.top = "0%";
      sideOffCanvas.value?.show();
    } else {
      sideOffCanvas.value?.hide();
    }
  }
);

const { direction, lengthY } = useSwipe(sideOffcanvasRef, {
  onSwipe() {
    if (direction.value !== "down") return;
    const swipeLength = Math.abs(lengthY.value);
    drawerStyle.transition = "none";
    drawerStyle.top = `${swipeLength}px`;
  },
  onSwipeEnd() {
    if (direction.value !== "down") return;
    const swipeLength = Math.abs(lengthY.value);
    drawerStyle.transition = "all 0.3s ease-in-out";

    if (swipeLength > height.value / 2) {
      drawerStyle.top = "100%";
      setTimeout(() => {
        sideOffCanvas.value?.hide();
      }, 300);
      return;
    }

    drawerStyle.top = "0%";
  },
});
</script>

<template>
  <div
    style="height: auto; max-width: 100vw"
    :class="['offcanvas', `offcanvas-${props.placement}`, { 'h-100': props.fullsize, 'w-100': props.fullsize }]"
    class="sheet"
    ref="sideOffcanvasRef"
    tabindex="-1"
    :style="drawerStyle"
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
