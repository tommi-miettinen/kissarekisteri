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
  visible: {
    type: Boolean,
    default: false,
    required: true,
  },
});

const sideOffCanvas = ref<Offcanvas>();
const sideOffcanvasRef = ref<HTMLDivElement>();
const swipeStart = ref(0);
const swipeEnd = ref(0);

const drawerStyle = reactive({
  bottom: "0",
  transition: "all 0.3s ease-in-out",
});

const { height, top } = useElementBounding(sideOffcanvasRef);

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
      drawerStyle.transition = "all 0.3s ease-in-out";
      drawerStyle.bottom = "0";
      sideOffCanvas.value?.show();
    } else {
      sideOffCanvas.value?.hide();
    }
  }
);

const disablePullToRefresh = () => {
  document.body.style.overscrollBehavior = "none";
  document.documentElement.style.overscrollBehavior = "none";
};

const enablePullToRefresh = () => {
  document.body.style.overscrollBehavior = "auto";
  document.documentElement.style.overscrollBehavior = "auto";
};

const hideCanvasWithAnimation = () => {
  drawerStyle.transition = "all 0.3s ease-in-out";
  setTimeout(() => {
    sideOffCanvas.value?.hide();
  }, 300);
};

const resetPosition = () => {
  drawerStyle.transition = "all 0.3s ease-in-out";
  drawerStyle.bottom = "0";
};

const moveDown = (time: number) => {
  const animationTime = Math.min(time, 300);
  drawerStyle.transition = `all ${animationTime}ms ease-in-out`;
  drawerStyle.bottom = `-100%`;

  setTimeout(() => {
    sideOffCanvas.value?.hide();
  }, animationTime);
};

const { direction, lengthY } = useSwipe(sideOffcanvasRef, {
  onSwipeStart: () => (swipeStart.value = performance.now()),
  onSwipe() {
    disablePullToRefresh();
    if (top.value === 0 && direction.value === "up") return;

    const swipeLength = Math.abs(lengthY.value);
    drawerStyle.transition = "none";

    const height2 = height.value * 2;
    const boundary = 24;

    const minBottom = height2 - boundary;
    const withSwipe = height2 - swipeLength;

    if (direction.value === "down") {
      drawerStyle.bottom = `${-swipeLength}px`;
    }

    if (direction.value === "up") {
      if (withSwipe < minBottom) return;
      drawerStyle.bottom = `${swipeLength}px`;
    }
  },
  onSwipeEnd() {
    swipeEnd.value = performance.now();
    const swipeLength = Math.abs(lengthY.value);
    drawerStyle.transition = "all 0.3s ease-in-out";
    enablePullToRefresh();

    const remainingDistance = height.value - swipeLength;
    const velocity = Math.abs(lengthY.value / (swipeEnd.value - swipeStart.value));
    const time = remainingDistance / velocity;

    if (velocity > 0.5 && direction.value === "down") {
      return moveDown(time);
    }

    if (swipeLength > height.value / 3 && direction.value === "down") {
      hideCanvasWithAnimation();
      return;
    }
    resetPosition();
  },
  threshold: 0,
});
</script>

<template>
  <div
    style="max-width: 100vw; height: min-content"
    :class="['offcanvas', `offcanvas-${props.placement}`]"
    class="sheet rounded-4 rounded-bottom-0 border-bottom"
    ref="sideOffcanvasRef"
    tabindex="-1"
    :style="drawerStyle"
    aria-labelledby="offcanvasRightLabel"
  >
    <div class="offcanvas-header">
      <button type="button" class="btn-close ms-auto" @click="sideOffCanvas?.hide()" aria-label="Close"></button>
    </div>
    <div class="d-flex flex-column z-1">
      <slot></slot>
    </div>
    <div v-if="direction === 'up'" style="height: 40px" class="w-100 bg-white position-fixed z-0 bottom-0 left-0" />
  </div>
</template>
