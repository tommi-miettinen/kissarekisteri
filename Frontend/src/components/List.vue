<script lang="ts" setup>
import { watchEffect } from "vue";
import { ref, computed } from "vue";
import { useI18n } from "vue-i18n";
import { createPagination } from "../utils/createPagination";

interface SearchProp {
  key: string;
  exactMatch?: boolean;
  startsWith?: boolean;
}

const props = defineProps({
  items: {
    type: Array as () => unknown[],
    required: true,
  },
  itemsPerPage: {
    type: Number,
    default: 8,
  },
  searchQueryPlaceholder: {
    type: String,
    default: "List.Search",
  },
  searchKeys: {
    type: Array as () => SearchProp[],
    required: true,
  },
});

const { t } = useI18n();

const searchQuery = ref("");
const currentPage = ref(1);
const filteredItems = ref<any>([]);
const pages = ref<string | number[]>([]);

const totalPages = computed(() => {
  return Math.ceil(filteredItems.value.length / props.itemsPerPage);
});

const displayedItems = computed(() => {
  const startIndex = (currentPage.value - 1) * props.itemsPerPage;
  return filteredItems.value.slice(startIndex, startIndex + props.itemsPerPage);
});

const goToPage = (pageNumber: number) => (currentPage.value = pageNumber);

const valueMatchesSearchQuery = (item: any, searchKey: SearchProp) => {
  const itemValue = item[searchKey.key];
  const queryValue = searchQuery.value.toLowerCase();

  if (typeof itemValue !== "string") return false;

  if (searchKey.exactMatch) return itemValue.toLowerCase() === queryValue;
  if (searchKey.startsWith) return itemValue.toLowerCase().startsWith(queryValue);
  return itemValue.toLowerCase().includes(queryValue);
};

watchEffect(() => {
  if (!searchQuery.value) {
    filteredItems.value = props.items;
  } else {
    currentPage.value = 1;
    filteredItems.value = props.items.filter((item) => props.searchKeys.some((searchKey) => valueMatchesSearchQuery(item, searchKey)));
  }
});

watchEffect(() => (pages.value = createPagination(currentPage.value, totalPages.value, 3)));
</script>

<template>
  <div class="w-100 h-100 d-flex flex-column">
    <div class="d-flex pb-2 bg-white align-items-center">
      <div class="col-12 col-md-4">
        <input
          data-testid="cat-search-input"
          class="form-control"
          type="text"
          v-model="searchQuery"
          :placeholder="t(searchQueryPlaceholder)"
        />
      </div>
    </div>
    <div class="">
      <div v-for="item in displayedItems" :key="item.id">
        <slot :item="item"></slot>
      </div>
    </div>
    <div class="d-flex mt-auto flex-wrap flex-column flex-sm-row gap-2 py-2">
      <nav v-if="totalPages > 1" aria-label="Page navigation" class="me-auto mt-auto">
        <div class="d-flex flex-wrap gap-1">
          <li
            :tabindex="typeof page === 'number' ? 0 : undefined"
            @keyup.enter="typeof page === 'number' && goToPage(page)"
            @click="typeof page === 'number' && goToPage(page)"
            class="btn border focus-ring focus-ring-dark hover-bg rounded-3"
            v-for="page in pages"
            :key="page"
            :class="{ 'bg-black': currentPage === page, 'text-white': currentPage === page, 'border-black': currentPage === page }"
          >
            {{ page }}
          </li>
        </div>
      </nav>
      <slot name="action"></slot>
    </div>
  </div>
</template>
