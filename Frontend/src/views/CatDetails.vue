<script lang="ts" setup>
import { ref } from "vue";
import catAPI from "../api/catAPI";
import { useRoute, useRouter } from "vue-router";
import Modal from "../components/Modal.vue";
import { useQuery, useMutation } from "@tanstack/vue-query";

const router = useRouter();
const route = useRoute();

const { data: cat, refetch } = useQuery({ queryKey: [+route.params.catId], queryFn: () => catAPI.getCatById(+route.params.catId) });

const uploadMutation = useMutation({
  mutationFn: (file: File) => {
    return catAPI.uploadCatImage(cat.value.id, file);
  },
  onSuccess: refetch,
});

const catMutation = useMutation({
  mutationFn: (imageUrl: string) => catAPI.editCat({ ...cat.value, imageUrl: imageUrl }),
  onSuccess: refetch,
});

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

  uploadMutation.mutate(input.files[0]);
};
</script>

<template>
  <div class="w-100 h-100 d-flex flex-column align-items-center gap-4">
    <div class="p-4 p-sm-5 rounded overflow-auto col-12 col-lg-8 gap-4 d-flex flex-column">
      <div v-if="cat" class="d-flex">
        <img :src="cat.imageUrl" style="min-width: 300px; min-height: 300px" />
        <div class="d-flex flex-column p-4">
          <h5 class="card-title">{{ cat.name }}</h5>
          <p class="card-text">
            This is a wider card with supporting text below as a natural lead-in to additional content. This content is a little bit longer.
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
      <button class="btn btn-primary me-auto"><input type="file" @change="handleFileChange" id="catImageInput" /></button>
      <div class="gap-2" style="display: grid; grid-template-columns: 1fr 1fr 1fr">
        <div style="position: relative" class="d-flex flex-column border" v-for="catImage in cat.photos" :key="catImage.url">
          <img :src="catImage.url" @click="() => setCurrentImage(catImage)" class="cat-thumbnail w-100" alt="Cat image" />
          <button @click.stop="() => catMutation.mutate(catImage.url)" class="btn btn-primary position-absolute bottom-0 m-2 z-1">
            Aseta profiilikuvaksi
          </button>
        </div>
      </div>
    </div>
  </div>
  <Modal @onCancel="() => setCurrentImage()" modal-id="image-gallery" :visible="Boolean(currentImage)">
    <img v-if="currentImage" :src="currentImage.url" />
  </Modal>
</template>

<style>
.cat-thumbnail:hover {
  cursor: pointer;
  opacity: 0.8;
}
</style>
