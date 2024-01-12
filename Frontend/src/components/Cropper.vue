<script lang="ts" setup>
import { ref, watchEffect } from "vue";
import Cropper from "cropperjs";
import "cropperjs/dist/cropper.css";
import { onBeforeMount } from "vue";
import { onBeforeUnmount } from "vue";

const emits = defineEmits(["onCrop"]);

defineProps({
  imageSrc: {
    type: String,
    required: true,
  },
});

const cropper = ref<Cropper>();
const croppable = ref(false);
const imageRef = ref<HTMLImageElement>();

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
  const blobUrl = roundedCanvas.toDataURL("image/jpeg"); // Specify the MIME type

  // Convert the Data URL to binary data
  const base64WithoutPrefix = blobUrl.split(",")[1];
  const binaryData = atob(base64WithoutPrefix);
  const uint8Array = new Uint8Array(binaryData.length);
  for (let i = 0; i < binaryData.length; i++) {
    uint8Array[i] = binaryData.charCodeAt(i);
  }

  // Create a File object from the binary data
  const file = new File([uint8Array], "fileName.jpg", {
    type: "image/jpeg",
  });

  emits("onCrop", file);
};

watchEffect(() => {
  if (imageRef.value) {
    cropper.value = new Cropper(imageRef.value, {
      aspectRatio: 1,
      viewMode: 1,
      minContainerHeight: 300,
      minContainerWidth: imageRef.value.width,
      minCanvasHeight: imageRef.value.height,
      minCanvasWidth: imageRef.value.width,
      minCropBoxHeight: 60,
      minCropBoxWidth: 60,
      ready: () => {
        croppable.value = true;
      },
    });
  }
});

onBeforeMount(() => {
  if (cropper.value) {
    cropper.value.destroy();
  }
});

onBeforeUnmount(() => {
  if (cropper.value) {
    cropper.value.destroy();
  }
});
</script>

<template>
  <div id="test-test" class="d-flex flex-column w-100 h-100 bg-white">
    <div :key="croppable.toString()" class="w-100" style="height: 300px">
      <img
        style="max-height: 300px; max-width: 100%; height: 100%; width: 100%; object-fit: contain"
        ref="imageRef"
        :src="imageSrc"
        alt="Picture"
      />
    </div>
    <div class="d-flex flex-grow-1 p-2">
      <button class="ms-auto w-sm-100 mt-auto btn bg-black px-5 py-2 text-white" type="button" @click="cropImage">Tallenna</button>
    </div>
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
