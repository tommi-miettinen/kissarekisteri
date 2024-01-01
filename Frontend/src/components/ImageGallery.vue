<script lang="ts" setup>
//@ts-ignore doesnt have types
import FsLightbox from "fslightbox-vue/v3";
import { ref } from "vue";

defineProps({
  photos: {
    type: Array as () => string[],
    required: true,
  },
});

const selectedImageIndex = ref(0);
const toggler = ref(false);
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
        @keyup.enter="(selectedImageIndex = index), (toggler = !toggler)"
        @click="(selectedImageIndex = index), (toggler = !toggler)"
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
  <FsLightbox :key="photos.length" :toggler="toggler" :sources="photos" :slide="selectedImageIndex + 1" />
</template>
