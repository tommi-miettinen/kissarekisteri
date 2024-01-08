<script lang="ts" setup>
//@ts-ignore doesnt have types
import FsLightbox from "fslightbox-vue/v3";
import { ref, nextTick } from "vue";
import { isCurrentAction, pushAction, removeAction } from "../store/actionStore";
import { useMutationObserver } from "@vueuse/core";

enum ActionType {
  FULLSCREEN_IMAGE = "FULLSCREEN_IMAGE",
}

defineProps({
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

const handleImageClick = (index: number) => {
  pushAction(ActionType.FULLSCREEN_IMAGE);
  selectedImageIndex.value = index;

  //toggler needs to be toggled after the lightbox has attached to the DOM
  nextTick(() => (toggler.value = !toggler.value));
};
</script>

<template>
  <div v-if="photos.length > 0" class="w-100 m-auto d-flex flex-column gap-2">
    <slot name="upload"></slot>
    <div class="image-gallery gap-2">
      <div
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

        <slot :name="'custom-content-' + index" :photo="photo"></slot>
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
</template>
