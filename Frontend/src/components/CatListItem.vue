<script lang="ts" setup>
import { defineProps } from "vue";
import { formatDateNoHours } from "../utils/formatDate";

import { useRouter } from "vue-router";

const props = defineProps({
  cat: {
    type: Object as () => Cat,
    required: true,
  },
});

const router = useRouter();

const navigateToCat = (catId: number) => router.push(`/cats/${catId}`);

const getTextColor = (sex: string) => {
  return sex === "Male" ? "#60a5fa" : "#fb7185";
};
</script>

<template>
  <div @click="navigateToCat(cat.id)" class="cat border-bottom gap-3 p-3 align-items-center w-100">
    <div class="d-flex align-items-center justify-content-start gap-2">
      <img
        :src="cat.imageUrl"
        class="rounded-circle"
        height="30"
        width="30"
        style="object-fit: fill; border: 2px solid"
        :style="{ borderColor: getTextColor(cat.sex) }"
      />
      <span>{{ cat.name }}</span>
    </div>

    <div>{{ cat.breed }}</div>
    <div class="overflow-hidden gap-2 align-items-center d-flex justify-content-between">
      <span>{{
        //@ts-ignore
        formatDateNoHours(cat.birthDate)
      }}</span>
      <span class="badge rounded-pill text-bg-primary">{{ "Myytävänä" }}</span>
    </div>
  </div>
</template>
