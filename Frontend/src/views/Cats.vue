<script lang="ts" setup>
import { computed } from "vue";
import catAPI from "../api/catAPI";
import { useQuery } from "@tanstack/vue-query";
import { useI18n } from "vue-i18n";
import CatListItem from "../components/CatListItem.vue";
import List from "../components/List.vue";
import { useWindowSize } from "@vueuse/core";

const { t } = useI18n();

const { data } = useQuery({
  queryKey: ["cats"],
  queryFn: () => catAPI.getCats(),
});

const isMobile = computed(() => useWindowSize().width.value < 768);
</script>

<template>
  <div class="w-100 h-100 p-0 p-sm-5 d-flex flex-column align-items-center">
    <div class="p-2 p-sm-5 rounded col-12 col-lg-8 h-100 d-flex flex-column">
      <h3 class="m-0">{{ t("Cats.cats") }}</h3>
      <List :searchQueryPlaceholder="t('Cats.searchInput')" v-if="data" :items="data" :itemsPerPage="isMobile ? 10 : 7">
        <template v-slot="{ item }">
          <CatListItem :cat="item" />
        </template>
      </List>
    </div>
  </div>
</template>
