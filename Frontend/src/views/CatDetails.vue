<script lang="ts" setup>
import { onMounted, ref } from "vue";
import catAPI from "../api/catAPI";
import { useRoute, useRouter } from "vue-router";
import Modal from "../components/Modal.vue";

interface CatImage {
  Url: string;
}

const router = useRouter();
const route = useRoute();

const cat = ref();
const catImages = ref<CatImage[]>([]);
const currentImage = ref();

const navigateToBreeder = (breederId: number) => router.push(`/users/${breederId}`);
const navigateToCatOwner = (ownerId: number) => router.push(`/users/${ownerId}`);

const setCurrentImage = (catImage?: any) => {
  if (catImage) currentImage.value = catImage;
  if (!catImage) currentImage.value = null;
};

const handleFileChange = async (event: Event) => {
  const input = event.target as HTMLInputElement;
  if (!input || !input.files) return;

  await catAPI.uploadCatImage(cat.value.id, input.files[0]);
};

onMounted(async () => {
  const result = await catAPI.getCatById(+route.params.catId);
  if (!result) return;

  cat.value = result;
});
</script>

<template>
  <div class="w-100 h-100 d-flex flex-column align-items-center p-5 gap-4">
    <div v-if="cat" class="card" style="max-width: 800px">
      <div class="row g-0">
        <div class="col-md-4">
          <img :src="cat.imageUrl" class="img-fluid rounded-start" alt="..." />
        </div>
        <div class="col-md-8 d-flex flex-column">
          <div class="card-body d-flex flex-column">
            <h5 class="card-title">{{ cat.name }}</h5>
            <p class="card-text">
              This is a wider card with supporting text below as a natural lead-in to additional content. This content is a little bit
              longer.
            </p>
            <div class="mt-auto d-flex align-items-center">
              <p class="card-text">
                <span class="badge rounded-pill text-bg-secondary">{{ cat.breed }}</span>
              </p>
              <button @click="() => navigateToBreeder(cat.breederId)" class="btn btn-secondary ms-auto">Omistaja</button>
              <button @click="() => navigateToCatOwner(cat.ownerId)" class="btn btn-secondary ms-2">Kasvattaja</button>
            </div>
          </div>
        </div>
      </div>
    </div>
    <button class="btn btn-primary"><input type="file" @change="handleFileChange" id="catImageInput" /></button>
    <h3>Kuvat</h3>
    <div class="gap-2" style="width: 800px; display: grid; grid-template-columns: 1fr 1fr 1fr">
      <img
        v-for="catImage in catImages"
        :key="catImage.Url"
        :src="catImage.Url"
        @click="() => setCurrentImage(catImage)"
        class="cat-thumbnail w-100"
        alt="Cat image"
      />
    </div>
  </div>
  <Modal @onCancel="() => setCurrentImage()" modal-id="image-gallery" :visible="Boolean(currentImage)">
    <img v-if="currentImage" :src="currentImage.Url" />
  </Modal>
</template>

<style>
.cat-thumbnail:hover {
  cursor: pointer;
  opacity: 0.8;
}
</style>
