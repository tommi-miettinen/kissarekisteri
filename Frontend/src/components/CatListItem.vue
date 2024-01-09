<script setup lang="ts">
import { computed } from "vue";
import { formatDateNoHours } from "../utils/formatDate";
import { useRouter } from "vue-router";
import { useWindowSize } from "@vueuse/core";
import { defineProps } from "vue";

const { cat } = defineProps({
  cat: {
    type: Object as () => Cat,
    required: true,
  },
});

const router = useRouter();

const navigateToCat = (catId: number) => router.push(`/cats/${catId}`);
const getTextColor = (sex: string) => (sex === "Male" ? "#60a5fa" : "#fb7185");

const altUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/6/6a/Mainecoon_jb2.jpg/250px-Mainecoon_jb2.jpg";

const isMobile = computed(() => useWindowSize().width.value < 768);
</script>

<template>
  <div class="border-bottom py-1 w-100">
    <div
      :id="'cat-list-item' + cat.id"
      tabindex="0"
      @keyup.enter="navigateToCat(cat.id)"
      @click="navigateToCat(cat.id)"
      class="hover-bg rounded-3 p-2 focus-ring p-3 gap-3 d-flex justify-content-between align-items-center w-100"
    >
      <div class="d-flex gap-4 rounded-3 align-items-center pointer hover-bg focus-ring">
        <div style="position: relative; height: 60px" class="d-flex border-primary">
          <img style="width: 100%; height: 100%; object-fit: contain" class="rounded-2 border" :src="cat.imageUrl || altUrl" />
          <!-- Position medal slot on top corner of the image -->
          <div class="position-absolute top-0 start-0 translate-middle">
            <slot name="medal"></slot>
          </div>
        </div>
        <div class="d-flex flex-column gap-1">
          <div>{{ cat.name }}</div>
          <div class="text-body-secondary">{{ cat.breed }}</div>
          <div :style="{ backgroundColor: getTextColor(cat.sex), color: 'black' }" class="badge rounded-pill bg-opacity-10">
            {{ cat.sex }}
          </div>

          <div v-if="isMobile" style="font-size: 12px; font-weight: bold">
            {{ `${formatDateNoHours(cat.birthDate)}` }}
          </div>
        </div>
        <div v-if="!isMobile" class="ms-auto" style="font-size: 12px; font-weight: bold; margin-top: auto">
          {{ `${formatDateNoHours(cat.birthDate)}` }}
        </div>
      </div>
      <div class="gap-2 align-items-center d-flex">
        <slot name="actions"></slot>
      </div>
    </div>
  </div>
</template>
