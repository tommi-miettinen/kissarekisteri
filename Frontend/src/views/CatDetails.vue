<script setup lang="ts">
import { watchEffect } from "vue";
import catAPI from "../api/catAPI";
import { useRoute } from "vue-router";
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
import { setCurrentRouteLabel } from "../store/routeStore";
import Spinner from "../components/Spinner.vue";
import { useFileDialog } from "@vueuse/core";
import { catAltUrl } from "../placeholders";
import { navigateTo } from "../store/routeStore";

const { t } = useI18n();
const route = useRoute();
const routeParamCatId = +route.params.catId;

const isMale = computed(() => cat.value?.sex === "Male");
const userIsCatOwner = computed(() => user.value && cat.value && user.value.id === cat.value.ownerId);

const {
  data: cat,
  refetch,
  isError: isCatError,
  isLoading,
  isFetched: catIsFetched,
} = useQuery({
  queryKey: QueryKeys.CAT_BY_ID(+route.params.catId),
  queryFn: () => catAPI.getCatWithOwnerAndBreeder(+route.params.catId),
  enabled: Boolean(routeParamCatId),
});

const uploadMutation = useMutation({
  mutationFn: (file: File) => catAPI.uploadCatImage(cat.value!.id, file),
  onSuccess: () => refetch(),
});

const catMutation = useMutation({
  mutationFn: (imageUrl: string) => catAPI.editCat({ ...cat.value!, imageUrl }),
  onSuccess: () => refetch(),
});

const requestOwnershipTransfer = async () => catAPI.requestOwnershipTransfer(cat.value!.id);

const { onChange: onFileChange, open } = useFileDialog({
  accept: "image/*",
});

onFileChange((files) => files && uploadMutation.mutate(files[0]));

watchEffect(() => cat.value && setCurrentRouteLabel(cat.value.name));
watchEffect(() => route.path && refetch());
</script>

<template>
  <h3 v-if="isCatError && !catIsFetched" class="m-5 fw-bold">{{ t("CatDetails.404") }}</h3>
  <Spinner v-if="isLoading" />
  <div
    :key="cat.id + routeParamCatId"
    v-if="cat"
    class="p-2 w-100 h-100 d-flex flex-column align-items-center col-12 col-xxl-8 p-sm-5 d-flex flex-column gap-sm-5"
  >
    <div class="flex-grow-1 pb-2 pb-sm-5 col-12 col-lg-8 gap-4 gap-sm-5 d-flex flex-column">
      <div class="d-flex flex-column flex-sm-row gap-2" style="min-height: 300px">
        <div class="border rounded-4 hero-image" style="position: relative; min-height: 300px; overflow: hidden; width: 100%">
          <img
            style="position: absolute; top: 0; left: 0; width: 100%; height: 100%; object-fit: cover"
            :src="cat.imageUrl || catAltUrl"
            alt="Cat image"
          />
        </div>
        <div class="d-flex flex-column p-2 gap-2" style="width: 100%">
          <h3 class="m-0">{{ cat.name }}</h3>
          <p class="m-0">
            {{ cat.breed }}
          </p>
          <div
            :style="{ backgroundColor: isMale ? '#93c5fd' : '#fda4af' }"
            style="width: 100px"
            class="m-0 text-black badge rounded-pill bg-opacity-75"
          >
            {{ t(`CatDetails.${cat.sex.toLowerCase()}`) }}
          </div>
          <p class="m-0 fw-semibold">
            <span>Syntynyt {{ moment(cat.birthDate).format("LLL") }}</span>
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
        <h5 class="m-2">{{ t("CatDetails.placings") }}</h5>
        <div v-if="cat.results" v-for="result in cat.results" class="border-bottom py-1">
          <div
            tabindex="0"
            @keyup.enter="() => navigateTo(`/catshows/${result.catShowId}`)"
            @click="() => navigateTo(`/catshows/${result.catShowId}`)"
            class="hover-bg-1 p-3 d-flex rounded-3 flex align-items-center focus-ring cursor-pointer"
          >
            <div class="d-flex align-items-center gap-2">
              <span :style="{ backgroundColor: getMedalColor(result.place) }" class="badge rounded-pill text-black"
                >#{{ result.place }}</span
              >
              <span>{{ result.catShow.name }}</span>
            </div>
          </div>
        </div>
      </div>
      <div v-if="cat.parents && cat.parents.length > 0">
        <h5 class="m-2">{{ t("CatDetails.parents") }}</h5>
        <CatListItem v-for="parent in cat.parents" :cat="parent.parentCat" />
      </div>

      <div v-if="cat.kittens && cat.kittens.length > 0">
        <h5 class="m-2">{{ t("CatDetails.kittens") }}</h5>
        <CatListItem v-for="kitten in cat.kittens" :cat="kitten.childCat" />
      </div>

      <div v-if="cat.owner">
        <h5 class="m-2">{{ t("CatDetails.owner") }}</h5>
        <UserListItem :user="cat.owner" />
      </div>
      <div v-if="cat.breeder">
        <h5 class="m-2">{{ t("CatDetails.breeder") }}</h5>
        <UserListItem :user="cat.breeder" />
      </div>
      <div class="d-flex flex-column gap-2">
        <button
          v-if="userIsCatOwner"
          type="button"
          @click="() => open()"
          class="btn bg-black rounded-3 text-white px-5 py-2 me-auto focus-ring focus-ring-dark w-sm-100"
        >
          {{ t("CatDetails.uploadImage") }} +
        </button>
        <ImageGallery
          :key="cat.id"
          :thumbnailActionButtonText="t('CatDetails.setAsProfilePicture')"
          showThumbnailActionButton
          :photos="cat.photos || []"
          @onThumbnailActionClick="catMutation.mutate"
        />
      </div>
    </div>
  </div>
</template>

<style scoped>
.cat-image {
  max-width: 100%;
}

@media (min-width: 768px) {
  .cat-image {
    max-width: 300px;
  }
}
</style>
