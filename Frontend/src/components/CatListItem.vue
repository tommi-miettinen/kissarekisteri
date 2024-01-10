<script setup lang="ts">
import { computed } from "vue";
import { useRouter } from "vue-router";
import { useWindowSize } from "@vueuse/core";
import moment from "moment";
import { useI18n } from "vue-i18n";

defineProps({
  cat: {
    type: Object as () => Cat,
    required: true,
  },
});

const { t } = useI18n();
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
      class="hover-bg rounded-3 p-sm-2 focus-ring gap-3 d-flex justify-content-between align-items-center w-100"
    >
      <div class="d-flex gap-2 rounded-3 align-items-center pointer hover-bg focus-ring w-100">
        <div style="position: relative" class="d-flex border-primary">
          <img style="width: 80px; height: 80px; object-fit: cover" class="rounded-2" :src="cat.imageUrl || altUrl" />
          <div class="position-absolute top-0 start-0 translate-middle">
            <slot name="medal"></slot>
          </div>
        </div>
        <div class="d-flex flex-column gap-1">
          <div>{{ cat.name }}</div>
          <div class="text-body-secondary">{{ cat.breed }}</div>
          <div :style="{ backgroundColor: getTextColor(cat.sex), color: 'black' }" class="badge rounded-pill bg-opacity-10">
            {{ cat.sex === "Male" ? t("CatDetails.male") : t("CatDetails.female") }}
          </div>

          <div v-if="isMobile" style="font-size: 13px; font-weight: bold">
            {{ `${moment(cat.birthDate).format("LLL")}` }}
          </div>
        </div>
        <div v-if="!isMobile" class="ms-auto" style="font-size: 13px; font-weight: bold; margin-top: auto">
          {{ `${moment(cat.birthDate).format("LLL")}` }}
        </div>
      </div>
      <div class="gap-2 align-items-center d-flex">
        <slot name="actions"></slot>
      </div>
    </div>
  </div>
</template>
