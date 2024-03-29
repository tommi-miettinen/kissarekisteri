<script lang="ts" setup>
import { ref, watchEffect, computed } from "vue";
import catAPI from "../api/catAPI";
import { useQuery } from "@tanstack/vue-query";
import { useI18n } from "vue-i18n";
import CatListItem from "../components/CatListItem.vue";
import List from "../components/List.vue";
import { QueryKeys } from "../api/queryKeys";
import Spinner from "../components/Spinner.vue";
import { onMounted } from "vue";
import { setCurrentRouteLabel } from "../store/routeStore";

const { t, locale } = useI18n();

const catsQuery = useQuery({
  queryKey: QueryKeys.CATS,
  queryFn: () => catAPI.getCats(),
});

const translatedValues = ref<Cat[]>();

watchEffect(() => {
  if (!catsQuery.data.value) return;
  translatedValues.value = catsQuery.data.value.map((cat) => {
    return {
      ...cat,
      translatedBreed: t(`Breeds.${cat.breed}`),
      translatedSex: t(`CatDetails.${cat.sex.toLowerCase()}`),
    };
  });
});

const searchKeys = computed<SearchKeys<Cat & { translatedSex: string; translatedBreed: string }>>(() => {
  return [
    {
      key: "name",
    },
    {
      key: "translatedBreed",
    },
    {
      key: "translatedSex",
      startsWith: locale.value === "en",
    },
  ];
});

onMounted(() => setCurrentRouteLabel("Kissat"));
</script>

<template>
  <Spinner v-if="catsQuery.isLoading.value" />
  <div v-if="!catsQuery.isLoading.value" style="min-height: 100%" class="d-flex flex-column p-3 p-sm-5 rounded col-12 col-lg-8 mx-auto">
    <h3 class="mb-3">{{ t("Cats.cats") }}</h3>
    <List
      :searchKeys="searchKeys"
      :searchQueryPlaceholder="t('Cats.searchInput')"
      v-if="translatedValues && translatedValues.length > 0"
      :items="translatedValues"
      :itemsPerPage="20"
    >
      <template v-slot="{ item }">
        <CatListItem :cat="item" />
      </template>
    </List>
  </div>
</template>
