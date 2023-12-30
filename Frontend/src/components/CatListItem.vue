<script lang="ts" setup>
import { defineProps } from "vue";
import { formatDateNoHours } from "../utils/formatDate";
import { useRouter } from "vue-router";

defineProps({
  cat: {
    type: Object as () => Cat,
    required: true,
  },
});

const router = useRouter();

const navigateToCat = (catId: number) => router.push(`/cats/${catId}`);
const getTextColor = (sex: string) => (sex === "Male" ? "#60a5fa" : "#fb7185");
</script>

<template>
  <li
    tabindex="0"
    @keyup.enter="navigateToCat(cat.id)"
    @click="navigateToCat(cat.id)"
    class="hover-bg focus-ring border-bottom gap-3 p-3 d-flex align-items-center justify-content-between w-100"
  >
    <div class="d-flex align-items-center justify-content-start gap-3">
      <slot name="medal"></slot>
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
    <div>
      {{
        //@ts-ignore
        formatDateNoHours(cat.birthDate)
      }}
    </div>
    <div class="overflow-hidden gap-2 align-items-center d-flex"></div>
    <slot name="actions"></slot>
  </li>
</template>
