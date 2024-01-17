<script lang="ts" setup>
import { ref, reactive, nextTick } from "vue";
import { ActionTypes, isCurrentAction, pushAction, removeAction } from "../store/actionStore";
import { useEventListener } from "@vueuse/core";
import { onLongPress } from "@vueuse/core";
import Drawer from "./Drawer.vue";
import { isMobile } from "../store/actionStore";

defineProps({
  thumbnailActionButtonText: {
    type: String,
    required: false,
  },
  showThumbnailActionButton: {
    type: Boolean,
    required: false,
    default: false,
  },
  photos: {
    type: Array as () => string[],
    required: true,
  },
});

const drawerOpen = ref(false);
const currentImage = ref<HTMLElement>();
const currentIndex = ref(0);

const handleImageClick = async (index: number) => {
  pushAction(ActionTypes.FULLSCREEN_IMAGE);
  currentImage.value = images[index];
  currentIndex.value = index;

  await nextTick();

  if (scrollContainer.value && images[index]) {
    const imageOffset = images[index].offsetLeft;
    scrollContainer.value.scrollLeft = imageOffset;
  }
};

const thumbnailRefs = reactive<Record<number, HTMLElement>>({});

Object.values(thumbnailRefs).forEach((thumbnailRef, index) => {
  onLongPress(thumbnailRef, () => {
    if (!isMobile.value) return;
    currentImage.value = images[index];
    drawerOpen.value = true;
    thumbnailRef.focus();
  });
});

const scrollContainer = ref<HTMLElement>();
const images = reactive<Record<number, HTMLElement>>({});

const getClosestImage = () => {
  const viewportMidpoint = window.innerWidth / 2;
  let closestImage = null;
  let smallestDistance = Infinity;

  Object.values(images).forEach((image, index) => {
    if (!scrollContainer.value) return;

    const imageMidpoint = image.offsetLeft - scrollContainer.value.scrollLeft + image.offsetWidth / 2;
    const distance = Math.abs(viewportMidpoint - imageMidpoint);

    if (distance < smallestDistance) {
      smallestDistance = distance;
      closestImage = image;
      currentImage.value = closestImage;
      currentIndex.value = index;
    }
  });
};

useEventListener(scrollContainer, "scroll", getClosestImage);
</script>

<template>
  <div v-if="photos.length > 0" class="w-100 m-auto d-flex flex-column gap-2">
    <slot name="upload"></slot>
    <div class="image-gallery gap-2">
      <div
        :ref="el => thumbnailRefs[index] = el as HTMLDivElement"
        tabindex="0"
        v-for="(photo, index) in photos"
        :key="photo"
        class="border image-container rounded-4 d-flex focus-ring"
        style="position: relative; width: 100%; overflow: hidden"
        @keyup.enter="handleImageClick(index)"
        @click="handleImageClick(index)"
      >
        <div style="width: 100%; padding-top: 100%; position: relative"></div>
        <img
          :src="photo"
          alt="Cat image"
          class="image thumbnail scale-up-animation"
          style="position: absolute; top: 0; left: 0; width: 100%; height: 100%; object-fit: cover"
        />
        <div v-if="showThumbnailActionButton && !isMobile" class="d-flex position-absolute w-100 bottom-0">
          <button
            @keyup.enter.stop="$emit('onThumbnailActionClick', photo)"
            @click.stop="$emit('onThumbnailActionClick', photo)"
            class="w-100 rounded-3 border btn-border focus-ring py-2 m-2"
          >
            {{ thumbnailActionButtonText }}
          </button>
        </div>
      </div>
    </div>
  </div>
  <Drawer :visible="drawerOpen" @onCancel="drawerOpen = false">
    <div style="height: 150px">
      <div @click="$emit('onThumbnailActionClick', currentImage), (drawerOpen = false)" class="rounded-3 w-100 p-3">
        Aseta profiilikuvaksi
      </div>
    </div>
  </Drawer>
  <div
    ref="scrollContainer"
    @click="removeAction(ActionTypes.FULLSCREEN_IMAGE)"
    v-if="isCurrentAction(ActionTypes.FULLSCREEN_IMAGE)"
    class="d-flex overflow-x-auto overflow-y-hidden scroller lightbox-container"
  >
    <div
      :key="index"
      @click="removeAction(ActionTypes.FULLSCREEN_IMAGE)"
      class="d-flex align-items-center justify-content-center"
      style="min-width: 100vw; width: 100vw; height: 100vh; scroll-snap-align: center; scroll-snap-stop: always"
      v-for="(_, index) in photos"
    >
      <div style="z-index: 1001; left: 12px; top: 12px" class="fixed-top text-white">{{ currentIndex + 1 }} / {{ photos.length }}</div>

      <img
        :ref="el => images[index] = el as HTMLImageElement"
        @click.stop
        :src="photos[index]"
        alt="Cat image"
        class="lightbox-image"
        style="object-fit: contain; height: min-content; scroll-snap-align: center"
      />
    </div>
  </div>
</template>

<style>
.lightbox-container {
  position: absolute;
  top: 0;
  left: 0;
  height: 100vh;
  min-height: 100vh;
  min-width: 100vw;
  width: 100vw;
  background-color: rgba(0, 0, 0, 0.8);
  z-index: 1000;
  scrollbar-width: none;
  &::-webkit-scrollbar {
    display: none;
  }
}

@media (max-width: 768px) {
  .lightbox-container {
    background-color: black;
  }
}

.lightbox-image {
  max-width: 100vw;
}

.scroller {
  scroll-snap-type: x mandatory;
  scroll-snap-stop: always;
}
</style>
