<script setup lang="ts">
import { ref, watch } from "vue";
import catAPI from "../api/catAPI";
import { useRoute, useRouter } from "vue-router";
import { useQuery, useMutation } from "@tanstack/vue-query";
import { computed } from "vue";
import { useI18n } from "vue-i18n";
import CatListItem from "../components/CatListItem.vue";
import getMedalColor from "../utils/getMedalColor";
import UserListItem from "../components/UserListItem.vue";
import ImageGallery from "../components/ImageGallery.vue";
import { user } from "../store/userStore";
import moment from "moment";
import { QueryKeys } from "../api/queryKeys";

const { t } = useI18n();
const router = useRouter();
const route = useRoute();

const {
  data: catData,
  refetch,
  isError: isCatError,
  isLoading,
  isFetched: catIsFetched,
} = useQuery({
  queryKey: QueryKeys.CAT_BY_ID(+route.params.catId),
  queryFn: () => catAPI.getCatById(+route.params.catId),
});

const cat = computed(() => catData.value?.data);

const uploadMutation = useMutation({
  mutationFn: (file: File) => catAPI.uploadCatImage(cat.value!.id, file),
  onSuccess: () => refetch(),
});

const catMutation = useMutation({
  mutationFn: (imageUrl: string) => catAPI.editCat({ ...cat.value!, imageUrl }),
  onSuccess: () => refetch(),
});

const requestOwnershipTransfer = async () => {
  await catAPI.requestOwnershipTransfer(cat.value!.id);
};

const navigateToCatShow = (catShowId: number) => router.push(`/catshows/${catShowId}`);

const handleFileChange = async (event: Event) => {
  const input = event.target as HTMLInputElement;
  if (!input || !input.files) return;

  uploadMutation.mutate(input.files[0]);
};

const inputRef = ref();
const triggerFileInput = () => inputRef.value?.click();

const catPhotos = computed(() => (cat.value ? cat.value.photos.map((photo) => photo.url) : []));
const altUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/6/6a/Mainecoon_jb2.jpg/450px-Mainecoon_jb2.jpg?20070329082601";

const userIsCatOwner = computed(() => user.value && cat.value && user.value.id === cat.value.ownerId);

watch(route, () => refetch());
const isMale = computed(() => cat.value?.sex === "Male");
</script>

<template>
  <h3 v-if="isCatError && !catIsFetched" class="m-5 fw-bold">{{ t("CatDetails.404") }}</h3>
  <div v-if="isLoading" class="spinner-border text-black m-auto" role="status">
    <span class="visually-hidden">Loading...</span>
  </div>
  <div v-if="cat" class="p-2 w-100 h-100 d-flex flex-column align-items-center col-12 col-xxl-8 p-sm-5 d-flex flex-column gap-sm-5">
    <div class="p-1 flex-grow-1 pb-5 col-12 col-lg-8 gap-2 gap-sm-5 d-flex flex-column">
      <div class="d-flex flex-column flex-sm-row gap-4" style="min-height: 300px">
        <div class="border rounded-4 hero-image" style="position: relative; min-height: 300px; overflow: hidden; width: 100%">
          <img
            style="position: absolute; top: 0; left: 0; width: 100%; height: 100%; object-fit: cover"
            :src="cat.imageUrl || altUrl"
            alt="Cat image"
          />
        </div>
        <div class="d-flex flex-column p-2" style="width: 100%">
          <h3>{{ cat.name }}</h3>
          <p>
            {{ cat.breed }}
          </p>
          <div
            :style="{ backgroundColor: isMale ? '#93c5fd' : '#fda4af' }"
            style="width: 100px"
            class="text-black badge rounded-pill bg-opacity-75"
          >
            {{ t(`CatDetails.${cat.sex.toLowerCase()}`) }}
          </div>
          <p>
            syntynyt <span>{{ moment(cat.birthDate).format("LLL") }}</span>
          </p>
          <button
            v-if="!userIsCatOwner"
            @click="requestOwnershipTransfer"
            class="btn rounded-3 px-5 py-2 bg-black text-white focus-ring mt-auto ms-auto w-sm-100"
          >
            {{ cat.owner ? "Pyydä omistajuutta" : "Ilmottaudu omistajaksi" }}
          </button>
        </div>
      </div>

      <div v-if="cat.results && cat.results.length > 0">
        <h5>{{ t("CatDetails.placings") }}</h5>
        <div v-for="result in cat.results" class="border-bottom py-1">
          <div
            tabindex="0"
            @keyup.enter="() => navigateToCatShow(result.catShowId)"
            @click="() => navigateToCatShow(result.catShowId)"
            class="hover-bg-1 p-3 d-flex rounded-3 p-2 flex align-items-center focus-ring cursor-pointer"
          >
            <div class="d-flex align-items-center gap-2">
              <span :style="{ backgroundColor: getMedalColor(result.place) }" class="badge rounded-pill text-black"
                >#{{ result.place }}</span
              >
              <span class="mb-1">{{ result.catShow.name }}</span>
            </div>
          </div>
        </div>
      </div>

      <div v-if="cat.parents && cat.parents.length > 0">
        <h5>{{ t("CatDetails.parents") }}</h5>
        <CatListItem v-for="parent in cat.parents" :cat="parent.parentCat" />
      </div>

      <div v-if="cat.kittens && cat.kittens.length > 0">
        <h5>{{ t("CatDetails.kittens") }}</h5>
        <CatListItem v-for="kitten in cat.kittens" :cat="kitten.childCat" />
      </div>

      <div v-if="cat.owner">
        <h5>{{ t("CatDetails.owner") }}</h5>
        <UserListItem :user="cat.owner" />
      </div>
      <div v-if="cat.breeder">
        <h5>{{ t("CatDetails.breeder") }}</h5>
        <UserListItem :user="cat.breeder" />
      </div>
      <div class="d-flex flex-column gap-2">
        <button @click="triggerFileInput" class="btn bg-black rounded-3 text-white px-5 py-2 me-auto focus-ring focus-ring-dark w-sm-100">
          <input class="d-none" ref="inputRef" type="file" @change="handleFileChange" id="catImageInput" />
          {{ t("CatDetails.uploadImage") }} +
        </button>
        <ImageGallery
          :thumbnailActionButtonText="t('CatDetails.setAsProfilePicture')"
          showThumbnailActionButton
          :photos="catPhotos"
          @onThumbnailActionClick="catMutation.mutate"
        />
      </div>
    </div>
  </div>
</template>

<style>
.cat-image {
  max-width: 100%;
}

@media (min-width: 768px) {
  .cat-image {
    max-width: 300px;
  }
}
</style>
