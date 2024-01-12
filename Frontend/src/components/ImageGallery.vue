<script lang="ts" setup>
import { ref, nextTick, reactive, watch } from "vue";
import { isCurrentAction, pushAction, removeAction } from "../store/actionStore";
import { useMutationObserver } from "@vueuse/core";
//@ts-ignore does not have types
import FsLightbox from "fslightbox-vue/v3";
import { onLongPress } from "@vueuse/core";
import Drawer from "./Drawer.vue";
import { isMobile } from "../store/actionStore";

enum ActionType {
  FULLSCREEN_IMAGE = "FULLSCREEN_IMAGE",
}

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

const toggler = ref(false);

useMutationObserver(
  document.documentElement,
  () => {
    const isOpen = document.documentElement.classList.contains("fslightbox-open");
    if (!isOpen) removeAction(ActionType.FULLSCREEN_IMAGE);
  },
  {
    attributes: true,
    attributeFilter: ["class"],
  }
);

const selectedImageIndex = ref(0);
const drawerOpen = ref(false);

const handleImageClick = (index: number) => {
  pushAction(ActionType.FULLSCREEN_IMAGE);
  selectedImageIndex.value = index;

  //toggler needs to be toggled after the lightbox has attached to the DOM
  nextTick(() => (toggler.value = !toggler.value));
};

const thumbnailRefs = reactive<Record<number, HTMLElement>>({});

watch(thumbnailRefs, () => {
  Object.values(thumbnailRefs).forEach((thumbnailRef, index) => {
    if (!thumbnailRef) return;
    onLongPress(thumbnailRef, () => {
      selectedImageIndex.value = index;
      drawerOpen.value = true;
      thumbnailRef.focus();
    });
  });
});
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
          class="image thumbnail"
          style="position: absolute; top: 0; left: 0; width: 100%; height: 100%; object-fit: cover"
        />
        <div v-if="showThumbnailActionButton && !isMobile" class="d-flex position-absolute w-100 bottom-0">
          <button @click.stop="$emit('onThumbnailActionClick', photo)" class="w-100 rounded-3 border btn-border focus-ring py-2 m-2">
            {{ thumbnailActionButtonText }}
          </button>
        </div>
      </div>
    </div>
  </div>
  <FsLightbox
    v-if="isCurrentAction(ActionType.FULLSCREEN_IMAGE)"
    :key="photos.length"
    :toggler="toggler"
    :sources="photos"
    :slide="selectedImageIndex + 1"
  />
  <Drawer :visible="drawerOpen" @onCancel="drawerOpen = false">
    <div style="height: 150px">
      <div @click="$emit('onThumbnailActionClick', photos[selectedImageIndex])" class="rounded-3 w-100 p-3">Aseta profiilikuvaksi</div>
    </div>
  </Drawer>
</template>
