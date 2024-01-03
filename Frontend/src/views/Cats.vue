<script lang="ts" setup>
import catAPI from "../api/catAPI";
import { useQuery } from "@tanstack/vue-query";
import { useI18n } from "vue-i18n";
import CatListItem from "../components/CatListItem.vue";
import List from "../components/List.vue";

const { t } = useI18n();

const { data } = useQuery({
  queryKey: ["cats"],
  queryFn: () => catAPI.getCats(),
});
</script>

<template>
  <div class="w-100 h-100 p-0 p-sm-5 d-flex flex-column align-items-center">
    <div class="p-2 p-sm-5 rounded col-12 col-lg-8 h-100 d-flex flex-column">
      <h3>{{ t("Cats.cats") }}</h3>
      <List :searchQueryPlaceholder="t('Cats.searchInput')" v-if="data" :items="data" :itemsPerPage="7">
        <template v-slot="{ item }">
          <CatListItem :cat="item" />
        </template>
      </List>
    </div>
  </div>
</template>
