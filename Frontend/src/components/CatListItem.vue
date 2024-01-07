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

const altUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/6/6a/Mainecoon_jb2.jpg/250px-Mainecoon_jb2.jpg";
</script>

<template>
  <div class="border-bottom py-1 w-100">
    <div
      :id="'cat-list-item' + cat.id"
      tabindex="0"
      @keyup.enter="navigateToCat(cat.id)"
      @click="navigateToCat(cat.id)"
      class="hover-bg rounded-3 p-2 p-sm-3 focus-ring gap-3 d-flex justify-content-between align-items-center w-100"
    >
      <div class="d-flex w-100 align-items-center justify-content-start gap-3">
        <slot name="medal"></slot>
        <img
          :src="cat.imageUrl || altUrl"
          class="rounded-circle"
          height="30"
          width="30"
          style="object-fit: fill; border: 2px solid"
          :style="{ borderColor: getTextColor(cat.sex) }"
        />
        <span>{{ cat.name }}</span>
      </div>

      <div class="w-100">{{ cat.breed }}</div>
      <div class="d-none w-100 d-sm-flex">
        {{
          //@ts-ignore
          formatDateNoHours(cat.birthDate)
        }}
      </div>
      <div class="gap-2 align-items-center d-flex">
        <slot name="actions"></slot>
      </div>
    </div>
  </div>
</template>
