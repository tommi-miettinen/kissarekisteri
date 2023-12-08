<script lang="ts" setup>
import { ref, onMounted, onBeforeUnmount } from "vue";
import Cropper from "cropperjs";
import "cropperjs/dist/cropper.css";

const emits = defineEmits(["onCrop"]);

const imageSrc = "https://kissarekisteri.blob.core.windows.net/images/dcebd246-fd93-46d6-a6f4-a426f1182e50.png";
const cropper = ref<Cropper | null>(null);
const croppable = ref(false);
const imageRef = ref<HTMLImageElement | null>(null);

const getRoundedCanvas = (sourceCanvas: HTMLCanvasElement) => {
  const canvas = document.createElement("canvas");
  const context = canvas.getContext("2d")!;
  const width = sourceCanvas.width;
  const height = sourceCanvas.height;

  canvas.width = width;
  canvas.height = height;
  context.imageSmoothingEnabled = true;
  context.drawImage(sourceCanvas, 0, 0, width, height);
  context.globalCompositeOperation = "destination-in";
  context.beginPath();
  context.arc(width / 2, height / 2, Math.min(width, height) / 2, 0, 2 * Math.PI, true);
  context.fill();
  return canvas;
};

const cropImage = () => {
  if (!croppable.value) return;

  const croppedCanvas = cropper.value?.getCroppedCanvas();
  if (!croppedCanvas) return;

  const roundedCanvas = getRoundedCanvas(croppedCanvas);
  const blobUrl = roundedCanvas.toDataURL();

  emits("onCrop", blobUrl);
};

onMounted(() => {
  if (!imageRef.value) return;

  cropper.value = new Cropper(imageRef.value, {
    aspectRatio: 1,
    viewMode: 1,
    ready: () => {
      croppable.value = true;
    },
  });
});

onBeforeUnmount(() => {
  if (cropper.value) {
    cropper.value.destroy();
  }
});
</script>

<template>
  <div class="d-flex flex-column gap-2">
    <img style="width: 100%; visibility: hidden" ref="imageRef" :src="imageSrc" alt="Picture" />
    <button class="ms-auto btn btn-primary" type="button" @click="cropImage">Tallenna</button>
  </div>
</template>

<style>
.cropper-view-box,
.cropper-face {
  border-radius: 50%;
}

.cropper-view-box {
  outline: 0;
  box-shadow: 0 0 0 1px #39f;
}
</style>
