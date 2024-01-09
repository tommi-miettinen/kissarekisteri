<script lang="ts" setup>
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
</script>

<template>
  <div style="min-height: 100%" class="d-flex flex-column p-2 p-sm-5 rounded col-12 col-lg-8 mx-auto">
    <h3 class="m-0">{{ t("Cats.cats") }}</h3>
    <List :searchQueryPlaceholder="t('Cats.searchInput')" v-if="data" :items="data" :itemsPerPage="20">
      <template v-slot="{ item }">
        <CatListItem :cat="item" />
      </template>
    </List>
  </div>
</template>
