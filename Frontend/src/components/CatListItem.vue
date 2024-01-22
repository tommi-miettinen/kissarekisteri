<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";
import { navigateTo } from "../store/routeStore";
import { catAltUrl } from "../placeholders";

const { t } = useI18n();

const props = defineProps({
  cat: {
    type: Object as () => Cat,
    required: true,
  },
});

const isMale = computed(() => props.cat.sex === "Male");
const badgeColor = computed(() => (isMale.value ? "#93c5fd" : "#fda4af"));
</script>

<template>
  <div v-if="cat" class="border-bottom py-1 w-100">
    <div
      tabindex="0"
      role="button"
      @keyup.enter="navigateTo(`/cats/${cat.id}`)"
      @click="navigateTo(`/cats/${cat.id}`)"
      class="hover-bg-1 rounded-3 p-2 p-sm-2 focus-ring gap-3 d-flex justify-content-between align-items-center w-100 cursor-pointer h-100"
    >
      <div class="d-flex gap-2 rounded-3 align-items-center pointer hover-bg w-100">
        <div class="position-relative d-flex">
          <img
            style="width: 80px; height: 80px; object-fit: cover"
            class="ratio-1x1 rounded-2 border"
            :src="cat.imageUrl || catAltUrl"
            alt="Photo of a cat"
          />
          <div class="position-absolute top-0 start-0 translate-middle">
            <slot name="medal"></slot>
          </div>
        </div>
        <div class="d-flex flex-column gap-1 m-0">
          <div class="text-capitalize fw-semibold">{{ cat.name }}</div>
          <div class="text-capitalize">{{ t(`Breeds.${cat.breed}`) }}</div>
          <div :style="{ backgroundColor: badgeColor }" style="width: 100px" class="text-black badge rounded-pill bg-opacity-75">
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
