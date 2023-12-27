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

const { t } = useI18n();
const router = useRouter();
const route = useRoute();

const { data: cat, refetch } = useQuery({
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

const navigateToUser = (userId: string) => router.push(`/users/${userId}`);
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

const avatarLoadError = ref(false);

watch(route, () => refetch());

const getMedalColor = (place: number) => {
  if (place === 1) {
    return "#fee101";
  } else if (place === 2) {
    return "#d7d7d7";
  } else if (place === 3) {
    return "#cd7f32";
  }
};
</script>

<template>
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
          v-for="result in cat.results"
          @click="() => navigateToCatShow(result.catShowId)"
          class="user p-3 d-flex border-bottom p-2 flex align-items-center"
        >
          <div class="d-flex align-items-center gap-2">
            <span :style="{ backgroundColor: getMedalColor(result.place) }" class="badge rounded-pill text-black">#{{ result.place }}</span>
            <span class="mb-1">{{ result.catShow.name }}</span>
          </div>
        </div>
      </div>

      <div v-if="cat.catParents.length > 0">
        <h5>{{ t("CatDetails.parents") }}</h5>
        <CatListItem v-if="cat.catParents" v-for="parent in cat.catParents" :cat="parent" />
      </div>

      <div v-if="cat.kittens && cat.kittens.length > 0">
        <h5>{{ t("CatDetails.kittens") }}</h5>
        <CatListItem v-if="cat.kittens" v-for="kitten in cat.kittens" :cat="kitten" />
      </div>

      <div>
        <h5>{{ t("CatDetails.owner") }}</h5>
        <div @click="() => navigateToUser(cat!.owner.id)" class="user p-3 d-flex border-bottom p-2 flex align-items-center">
          <div class="col d-flex align-items-center gap-2 col-8">
            <img
              v-if="cat.owner.avatarUrl && !avatarLoadError"
              class="rounded-circle"
              height="32"
              width="32"
              style="object-fit: fill"
              :src="cat.owner.avatarUrl"
              alt="User avatar"
              :onerror="(avatarLoadError = true)"
            />
            <div
              style="width: 32px; height: 32px; font-size: 14px"
              class="rounded-circle d-flex align-items-center justify-content-center bg-primary fw-bold"
            >
              {{ cat.owner.givenName[0] + cat.owner.surname[0] }}
            </div>
            <div>{{ `${cat.owner.givenName}  ${cat.owner.surname}` }}</div>
          </div>
          <div class="col"></div>
          <span class="badge rounded-pill text-bg-primary">{{ cat.owner.isBreeder ? t("Users.breeder") : t("Users.breeder") }}</span>
        </div>
      </div>
      <div>
        <h5>{{ t("CatDetails.breeder") }}</h5>
        <div @click="() => navigateToUser(cat!.breeder.id)" class="user p-3 d-flex border-bottom p-2 flex align-items-center">
          <div class="col d-flex align-items-center gap-2 col-8">
            <img
              v-if="cat.breeder.avatarUrl && !avatarLoadError"
              class="rounded-circle"
              height="32"
              width="32"
              style="object-fit: fill"
              :src="cat.breeder.avatarUrl"
              alt="User avatar"
              :onerror="(avatarLoadError = true)"
            />
            <div
              style="width: 32px; height: 32px; font-size: 14px"
              class="rounded-circle d-flex align-items-center justify-content-center bg-primary fw-bold"
            >
              {{ cat.breeder.givenName[0] + cat.breeder.surname[0] }}
            </div>
            <div>{{ `${cat.breeder.givenName}  ${cat.breeder.surname}` }}</div>
          </div>
          <div class="col"></div>
          <span class="badge rounded-pill text-bg-primary">{{ cat.breeder.isBreeder ? t("Users.breeder") : t("Users.breeder") }}</span>
        </div>
      </div>
      <div class="d-flex flex-column gap-2">
        <button @click="triggerFileInput" class="btn border rounded-3 px-5 py-2 btn-border me-auto">
          <input class="d-none" ref="inputRef" type="file" @change="handleFileChange" id="catImageInput" />
          {{ t("CatDetails.uploadImage") }} +
        </button>
        <div v-if="cat.photos" class="image-gallery gap-2">
          <div
            v-for="(catImage, index) in cat.photos"
            :key="catImage.id"
            class="border image-container rounded-4 d-flex"
            style="position: relative; width: 100%; overflow: hidden"
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
              class="rounded-3 btn btn-light rounded-3 py-2 position-absolute z-2 bottom-0 m-2"
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

<style>
.btn-light:hover {
  background-color: #f3f4f6;
}
.image-container .btn {
  visibility: hidden;
}

.image-container:hover .btn {
  visibility: visible;
}
.cat-thumbnail:hover {
  cursor: pointer;
  opacity: 0.8;
}
</style>
