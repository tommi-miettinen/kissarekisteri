<script lang="ts" setup>
import { ref, watch } from "vue";
import catAPI from "../api/catAPI";
import { useRoute, useRouter } from "vue-router";
import { useQuery, useMutation } from "@tanstack/vue-query";
//@ts-ignore doesnt have types
import FsLightbox from "fslightbox-vue/v3";
import { computed } from "vue";
import { useI18n } from "vue-i18n";
import CatListItem from "../components/CatListItem.vue";
import getMedalColor from "../utils/getMedalColor";
import UserListItem from "../components/UserListItem.vue";

const { t } = useI18n();
const router = useRouter();
const route = useRoute();

const {
  data: cat,
  refetch,
  isError: isCatError,
} = useQuery({
  queryKey: [route.params.catId],
  queryFn: () => catAPI.getCatById(+route.params.catId),
});

const uploadMutation = useMutation({
  mutationFn: (file: File) => catAPI.uploadCatImage(cat.value!.id, file),
  onSuccess: () => refetch(),
});

const catMutation = useMutation({
  mutationFn: (imageUrl: string) => catAPI.editCat({ ...cat.value!, imageUrl }),
  onSuccess: () => refetch(),
});

const navigateToCatShow = (catShowId: number) => router.push(`/catshows/${catShowId}`);

const handleFileChange = async (event: Event) => {
  const input = event.target as HTMLInputElement;
  if (!input || !input.files) return;

  uploadMutation.mutate(input.files[0]);
};

const inputRef = ref();
const triggerFileInput = () => inputRef.value?.click();

const toggler = ref(false);
const selectedImage = ref(0);

const catPhotos = computed(() => (cat.value ? cat.value.photos.map((photo) => photo.url) : []));

watch(route, () => refetch());
</script>

<template>
  <h3 v-if="isCatError" class="m-5 fw-bold">{{ t("CatDetails.404") }}</h3>

  <div v-if="cat" class="w-100 h-100 d-flex flex-column align-items-center gap-4 p-5">
    <div class="p-4 p-sm-5 rounded overflow-auto col-12 col-lg-8 gap-5 d-flex flex-column">
      <div class="d-flex flex-column flex-sm-row gap-4" style="min-height: 300px">
        <div class="border image-container rounded-4" style="position: relative; min-width: 400px; overflow: hidden">
          <img style="position: absolute; top: 0; left: 0; width: 100%; height: 100%; object-fit: cover" :src="cat.imageUrl" />
        </div>
        <div class="d-flex flex-column p-2" style="width: 100%">
          <h3>{{ cat.name }}</h3>
          <p>
            {{ cat.breed }}
          </p>
          <p>{{ cat.birthDate }}</p>
        </div>
      </div>

      <div v-if="cat.results.length > 0">
        <h5>{{ t("CatDetails.placings") }}</h5>
        <div
          tabindex="0"
          v-for="result in cat.results"
          @keyup.enter="() => navigateToCatShow(result.catShowId)"
          @click="() => navigateToCatShow(result.catShowId)"
          class="hover-bg p-3 d-flex border-bottom p-2 flex align-items-center focus-ring"
        >
          <div class="d-flex align-items-center gap-2">
            <span :style="{ backgroundColor: getMedalColor(result.place) }" class="badge rounded-pill text-black">#{{ result.place }}</span>
            <span class="mb-1">{{ result.catShow.name }}</span>
          </div>
        </div>
      </div>

      <div v-if="cat.parents.length > 0">
        <h5>{{ t("CatDetails.parents") }}</h5>
        <CatListItem v-if="cat.parents" v-for="parent in cat.parents" :cat="parent.parentCat" />
      </div>

      <div v-if="cat.kittens && cat.kittens.length > 0">
        <h5>{{ t("CatDetails.kittens") }}</h5>
        <CatListItem v-if="cat.kittens" v-for="kitten in cat.kittens" :cat="kitten.childCat" />
      </div>

      <div>
        <h5>{{ t("CatDetails.owner") }}</h5>
        <UserListItem :user="cat.owner" />
      </div>
      <div>
        <h5>{{ t("CatDetails.breeder") }}</h5>
        <UserListItem :user="cat.breeder" />
      </div>
      <div class="d-flex flex-column gap-2">
        <button @click="triggerFileInput" class="border rounded-3 px-5 py-2 btn-border me-auto focus-ring">
          <input class="d-none" ref="inputRef" type="file" @change="handleFileChange" id="catImageInput" />
          {{ t("CatDetails.uploadImage") }} +
        </button>
        <div v-if="cat.photos" class="image-gallery gap-2">
          <div
            v-for="(catImage, index) in cat.photos"
            :key="catImage.id"
            tabindex="0"
            class="border image-container rounded-4 d-flex focus-ring"
            style="position: relative; width: 100%; overflow: hidden"
            @keyup.enter="(selectedImage = index), (toggler = !toggler)"
            @click="(selectedImage = index), (toggler = !toggler)"
          >
            <div style="width: 100%; padding-top: 100%; position: relative"></div>
            <img
              :src="catImage.url"
              alt="Cat image"
              class="image thumbnail"
              style="position: absolute; top: 0; left: 0; width: 100%; height: 100%; object-fit: cover"
            />
            <button
              style="width: 93%"
              @click.stop="catMutation.mutate(catImage.url)"
              class="rounded-3 border btn-border focus-ring rounded-3 py-2 position-absolute z-2 bottom-0 m-2"
            >
              {{ t("CatDetails.setAsProfilePicture") }}
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
  <FsLightbox :key="cat?.photos.length" :toggler="toggler" :sources="catPhotos" :slide="selectedImage + 1" />
</template>
