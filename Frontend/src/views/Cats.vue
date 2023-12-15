<script lang="ts" setup>
import { ref, watchEffect } from "vue";
import catAPI from "../api/catAPI";
import { useRouter } from "vue-router";
import { useQuery } from "@tanstack/vue-query";
import { useI18n } from "vue-i18n";
import { formatDateNoHours } from "../utils/formatDate";

const router = useRouter();
const { t } = useI18n();

const { data, isLoading } = useQuery({
  queryKey: ["cats"],
  queryFn: catAPI.getCats,
});

const filteredCats = ref<Cat[]>([]);
const searchQuery = ref("");

const navigateToCat = (catId: number) => router.push(`/cats/${catId}`);

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
        <div class="col">
          <input class="form-control" type="text" v-model="searchQuery" :placeholder="t('Cats.searchInput')" />
        </div>
        <div class="col">{{ t("Cats.breed") }}</div>
        <div class="col">{{ t("Cats.birthDate") }}</div>
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
          class="cat border-bottom gap-3 p-3 align-items-center"
        >
          <div class="d-flex align-items-center justify-content-start gap-2">
            <img class="rounded-circle bg-primary" height="30" width="30" style="object-fit: contain" />
            <span class="text-upper-capitalize">
              {{ cat.name }}
            </span>
          </div>

          <div class="">{{ cat.breed }}</div>
          <div class="overflow-hidden gap-2 align-items-center d-flex justify-content-between">
            <span>{{
              //@ts-ignore
              formatDateNoHours(cat.birthDate)
            }}</span>
            <span class="badge rounded-pill text-bg-primary">{{ "Myytävänä" }}</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style>
.cat {
  display: grid;
  grid-template-columns: 1fr 1fr 1fr;
}
.cat:hover {
  cursor: pointer;
  background-color: #f3f4f6;
}
</style>
