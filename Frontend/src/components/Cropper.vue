<script lang="ts" setup>
import { ref, watch, nextTick } from "vue";
import { useElementVisibility } from "@vueuse/core";
import Cropper from "cropperjs";
import "cropperjs/dist/cropper.css";
import { useFileDialog } from "@vueuse/core";

const emits = defineEmits(["onCrop"]);

const props = defineProps({
  imageSrc: {
    type: String,
    required: true,
  },
});

const cropper = ref<Cropper>();
const croppable = ref(false);
const imageRef = ref<HTMLImageElement>();
const imageSource = ref(props.imageSrc);
const containerRef = ref();

const targetIsVisible = useElementVisibility(containerRef);

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
  const croppedCanvas = cropper.value?.getCroppedCanvas();
  if (!croppedCanvas) return;

  const roundedCanvas = getRoundedCanvas(croppedCanvas);
  const blobUrl = roundedCanvas.toDataURL("image/jpeg");

  const base64WithoutPrefix = blobUrl.split(",")[1];
  const binaryData = atob(base64WithoutPrefix);
  const uint8Array = new Uint8Array(binaryData.length);
  for (let i = 0; i < binaryData.length; i++) {
    uint8Array[i] = binaryData.charCodeAt(i);
  }

  const file = new File([uint8Array], "kuva.jpg", {
    type: "image/jpeg",
  });

  emits("onCrop", file);
};

const { onChange: onFileChange, open } = useFileDialog({
  accept: "image/*",
});

onFileChange((files) => files && (imageSource.value = URL.createObjectURL(files[0])));

watch([() => targetIsVisible.value, () => imageSource.value], async () => {
  if (imageRef.value) {
    cropper.value?.destroy();
    croppable.value = false;

    await nextTick();

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
</script>

<template>
  <div ref="containerRef" class="d-flex flex-column w-100 h-100 bg-white">
    <div class="w-100 position-relative" style="height: 300px">
      <img
        style="max-height: 300px; visibility: hidden; max-width: 100%; height: 100%; width: 100%; object-fit: contain"
        ref="imageRef"
        :key="croppable.toString()"
        :src="imageSource"
        alt="Picture"
      />
    </div>
    <div class="d-flex flex-grow-1 p-2 gap-2">
      <button @click="() => open()" class="btn bg-black rounded-3 text-white w-100 py-2 focus-ring">Valitse tiedosto</button>
      <button class="btn focus-ring accordion bg-black w-100 rounded-3 py-2 text-white" type="button" @click="cropImage">Tallenna</button>
    </div>
  </div>
</template>

<style>
.cropper-container {
  top: 0;
  position: absolute;
}

.cropper-view-box,
.cropper-face {
  border-radius: 50%;
}

.cropper-view-box {
  outline: 0;
  box-shadow: 0 0 0 1px #39f;
}
</style>
