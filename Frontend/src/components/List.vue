<script lang="ts" setup>
import { watchEffect } from "vue";
import { ref, computed } from "vue";
import { useI18n } from "vue-i18n";

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

const goToPage = (pageNumber: number) => {
  if (pageNumber >= 1 && pageNumber <= totalPages.value) {
    currentPage.value = pageNumber;
  }
};

const valueMatchesSearchQuery = (value: any) => {
  if (value) {
    console.log(value);
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
</script>

<template>
  <div class="w-100 h-100 d-flex flex-column">
    <div class="d-flex gap-3 py-3 sticky-top bg-white align-items-center">
      <div class="col-12 col-sm-4">
        <input
          data-testid="cat-search-input"
          class="form-control"
          type="text"
          v-model="searchQuery"
          :placeholder="t(searchQueryPlaceholder)"
        />
      </div>
    </div>
    <div class="p-1">
      <div v-for="item in displayedItems" :key="item.id">
        <slot :item="item"></slot>
      </div>
    </div>
    <div class="d-flex m-2 mt-auto">
      <nav aria-label="Page navigation" class="me-auto mt-auto">
        <div class="d-flex gap-1">
          <li
            tabindex="0"
            @keyup.enter="goToPage(page)"
            @click="goToPage(page)"
            class="btn border focus-ring hover-bg"
            v-for="page in totalPages"
            :key="page"
            :class="{ 'bg-primary': currentPage === page, 'text-white': currentPage === page }"
          >
            {{ page }}
          </li>
        </div>
      </nav>
      <slot name="action"></slot>
    </div>
  </div>
</template>
