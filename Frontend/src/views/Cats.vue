<script lang="ts" setup>
import { ref, watchEffect } from "vue";
import catAPI from "../api/catAPI";
import { useQuery } from "@tanstack/vue-query";
import { useI18n } from "vue-i18n";
import CatListItem from "../components/CatListItem.vue";

const { t } = useI18n();

const { data, isLoading } = useQuery({
  queryKey: ["cats"],
  queryFn: () => catAPI.getCats(),
});

const filteredCats = ref<Cat[]>([]);
const searchQuery = ref("");

watchEffect(() => {
  if (!searchQuery.value) {
    filteredCats.value = data?.value || [];
  } else {
    filteredCats.value =
      data.value?.filter((cat) => {
        return (
          cat.name.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
          cat.breed.toLowerCase().includes(searchQuery.value.toLowerCase())
        );
      }) || [];
  }
});
</script>

<template>
  <div class="w-100 h-100 p-0 p-sm-5 d-flex flex-column align-items-center">
    <div class="p-2 p-sm-5 rounded col-12 col-lg-8">
      <h3>{{ t("Cats.cats") }}</h3>
      <div class="d-flex gap-3 py-3 sticky-top bg-white align-items-center">
        <div class="col-12 col-sm-4">
          <input
            data-testid="cat-search-input"
            class="form-control"
            type="text"
            v-model="searchQuery"
            :placeholder="t('Cats.searchInput')"
          />
        </div>
        <div class="col invisible sm-visible">{{ t("Cats.breed") }}</div>
        <div class="col invisible sm-visible">{{ t("Cats.birthDate") }}</div>
      </div>

      <div class="d-flex flex-column overflow-auto cat-list-container">
        <div v-if="isLoading" class="spinner-border text-primary m-auto" role="status">
          <span class="visually-hidden">Loading...</span>
        </div>
        <CatListItem v-else v-for="cat in filteredCats" :cat="cat" />
      </div>
    </div>
  </div>
</template>

<style>
.cat-list-container {
  height: 500px;
}
</style>
