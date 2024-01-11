<script lang="ts" setup>
import { watchEffect, watch, onMounted } from "vue";
import { ref, computed } from "vue";
import { useI18n } from "vue-i18n";
import { createPagination } from "../utils/createPagination";

const props = defineProps({
  items: {
    type: Array as () => any[],
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
});

const { t } = useI18n();

const searchQuery = ref("");
const currentPage = ref(1);
const totalPages = computed(() => {
  return Math.ceil(filteredItems.value.length / props.itemsPerPage);
});

const filteredItems = ref<any[]>([]);

const displayedItems = computed(() => {
  const startIndex = (currentPage.value - 1) * props.itemsPerPage;
  return filteredItems.value.slice(startIndex, startIndex + props.itemsPerPage);
});

const pages = ref<string | number[]>([]);

const goToPage = (pageNumber: number) => {
  if (pageNumber >= 1 && pageNumber <= totalPages.value) {
    currentPage.value = pageNumber;
  }
};

const valueMatchesSearchQuery = (value: any) => {
  if (value) {
    return value.toString().toLowerCase().includes(searchQuery.value.toLowerCase());
  }
  return false;
};

watchEffect(() => {
  if (!searchQuery.value) {
    filteredItems.value = props.items;
  } else {
    currentPage.value = 1;
    filteredItems.value = props.items.filter((item) => Object.values(item).some((value) => valueMatchesSearchQuery(value)));
  }
});

console.log(totalPages.value);

onMounted(() => {
  pages.value = createPagination(currentPage.value, totalPages.value, 3);
});

watch([() => props.itemsPerPage, () => currentPage.value], () => {
  pages.value = createPagination(currentPage.value, totalPages.value, 3);
});
</script>

<template>
  <div class="w-100 h-100 d-flex flex-column">
    <div class="d-flex gap-3 py-3 bg-white align-items-center">
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
            :class="{ 'bg-black': currentPage === page, 'text-white': currentPage === page }"
          >
            {{ page }}
          </li>
        </div>
      </nav>

      <slot name="action"></slot>
    </div>
  </div>
</template>
