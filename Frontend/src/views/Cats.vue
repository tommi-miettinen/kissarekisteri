<script lang="ts" setup>
import { ref, watchEffect } from "vue";
import catAPI from "../api/catAPI";
import { useRouter } from "vue-router";
import { useQuery } from "@tanstack/vue-query";
import { useI18n } from "vue-i18n";

const router = useRouter();
const { t } = useI18n();

const { data, isLoading } = useQuery({
  queryKey: ["cats"],
  queryFn: catAPI.getCats,
});

const filteredCats = ref<Cat[]>([]);
const searchQuery = ref("");

const navigateToCat = (catId: number) => router.push(`/cats/${catId}`);
const navigateToCatOwner = (ownerId: string) => router.push(`/users/${ownerId}`);

watchEffect(() => {
  if (!searchQuery.value) {
    filteredCats.value = data?.value || [];
  } else {
    filteredCats.value = data.value?.filter((cat) => cat.name.toLowerCase().includes(searchQuery.value.toLowerCase())) || [];
  }
});
</script>

<template>
  <div class="w-100 h-100 p-0 p-sm-5 d-flex flex-column align-items-center justify-content-center">
    <div class="p-4 p-sm-5 rounded overflow-auto col-12 col-lg-8">
      <h3>{{ t("Cats.cats") }}</h3>
      <div class="d-flex gap-3 py-3 sticky-top bg-white align-items-center">
        <div class="col"><input class="form-control" type="text" v-model="searchQuery" :placeholder="t('Cats.searchInput')" /></div>
        <div class="col">Rotu</div>
        <div class="col">Syntymäaika</div>
      </div>

      <div class="d-flex flex-column overflow-auto" style="height: 500px">
        <div v-if="isLoading" class="spinner-border text-primary m-auto" role="status">
          <span class="visually-hidden">Loading...</span>
        </div>
        <div
          v-else
          v-for="cat in filteredCats"
          :key="cat.id"
          @click="() => navigateToCat(cat.id)"
          class="cat d-flex border-bottom p-2 flex align-items-center"
        >
          <div class="col d-flex align-items-center bg-body-secondary">
            <img
              class="rounded-circle"
              height="30"
              width="30"
              style="object-fit: contain; margin-right: auto"
              src="https://placekitten.com/300/300"
              alt="Cat Image"
            />
            <span>
              {{ cat.name }}
            </span>
          </div>

          <div class="col">{{ cat.breed }}</div>
          <div class="col overflow-hidden">{{ cat.birthDate }}</div>
          <div class="col overflow-hidden gap-2 d-flex justify-content-end">
            <button @click.stop="() => navigateToCatOwner(cat.ownerId)" class="btn btn-secondary">Omistaja</button>
            <button @click.stop="() => navigateToCatOwner(cat.breederId)" class="btn btn-secondary">Kasvattaja</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style>
.cat:hover {
  cursor: pointer;
  background-color: #f3f4f6;
}
</style>
