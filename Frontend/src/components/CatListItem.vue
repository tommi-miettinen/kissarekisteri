<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";
import { navigateTo } from "../store/routeStore";

const props = defineProps({
  cat: {
    type: Object as () => Cat,
    required: true,
  },
});

const { t } = useI18n();

const navigateToCat = (cat: Cat) => navigateTo(`/cats/${cat.id}`);
const isMale = computed(() => props.cat.sex === "Male");
const altUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/6/6a/Mainecoon_jb2.jpg/450px-Mainecoon_jb2.jpg?20070329082601";
</script>

<template>
  <div v-if="cat" class="border-bottom py-1 w-100">
    <div
      :id="'cat-list-item' + cat.id"
      tabindex="0"
      @keyup.enter="navigateToCat(cat)"
      @click="navigateToCat(cat)"
      class="hover-bg-1 rounded-3 p-2 p-sm-2 focus-ring gap-3 d-flex justify-content-between align-items-center w-100 cursor-pointer h-100"
    >
      <div class="d-flex gap-2 rounded-3 align-items-center pointer hover-bg w-100">
        <div style="position: relative" class="d-flex">
          <img style="width: 80px; height: 80px; object-fit: cover" class="ratio-1x1 rounded-2 border" :src="cat.imageUrl || altUrl" />
          <div class="position-absolute top-0 start-0 translate-middle">
            <slot name="medal"></slot>
          </div>
        </div>
        <div class="d-flex flex-column gap-1 m-0">
          <div class="text-capitalize fw-semibold">{{ cat.name }}</div>
          <div class="text-capitalize">{{ cat.breed }}</div>
          <div
            :style="{ backgroundColor: isMale ? '#93c5fd' : '#fda4af' }"
            style="width: 100px"
            class="text-black badge rounded-pill bg-opacity-75"
          >
            {{ t(`CatDetails.${cat.sex.toLowerCase()}`) }}
          </div>
        </div>
      </div>
      <div class="gap-2 d-flex mb-auto">
        <slot name="actions"></slot>
      </div>
    </div>
  </div>
</template>
