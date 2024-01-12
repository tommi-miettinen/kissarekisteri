<script lang="ts" setup>
import { ref, watchEffect } from "vue";
import catAPI from "../api/catAPI";
import { useQuery } from "@tanstack/vue-query";
import { useI18n } from "vue-i18n";
import CatListItem from "../components/CatListItem.vue";
import List from "../components/List.vue";
import { QueryKeys } from "../api/queryKeys";

const { t } = useI18n();

const { data } = useQuery({
  queryKey: [QueryKeys.CATS],
  queryFn: () => catAPI.getCats(),
});

const translatedValues = ref<Cat[]>();

watchEffect(() => {
  if (!data.value) return;
  translatedValues.value = data.value?.map((cat) => {
    return {
      ...cat,
      breed: t(`Breeds.${cat.breed}`),
    };
  });
});
</script>

<template>
  <div style="min-height: 100%" class="d-flex flex-column p-2 p-sm-5 rounded col-12 col-lg-8 mx-auto">
    <h3 class="m-0">{{ t("Cats.cats") }}</h3>
    <List
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
