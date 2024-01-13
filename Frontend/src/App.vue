<script setup lang="ts">
import Navigation from "./components/Navigation.vue";
import { Toaster } from "vue-sonner";
import { onMounted, computed, ref, watch } from "vue";
import { fetchPermissions, fetchUser } from "./store/userStore";
import BottomNavigation from "./components/BottomNavigation.vue";
import { useWindowSize, useElementVisibility } from "@vueuse/core";

const mainRef = ref<HTMLElement>();

onMounted(async () => {
  await fetchUser();
  await fetchPermissions();
});

import { useActiveElement } from "@vueuse/core";

const activeElement = useActiveElement();

watch(activeElement, (el) => {
  console.log("focus changed to", el);
});

const focusMainContent = () => {
  // Define the selectors for focusable elements
  const focusableSelectors = 'button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])';

  // Find all focusable elements within the mainRef
  const focusableElements = mainRef.value?.querySelectorAll(focusableSelectors);

  // Loop through the focusable elements
  for (const elem of focusableElements || []) {
    console.log(elem);
    const targetIsVisible = useElementVisibility(elem as HTMLElement);
    if (targetIsVisible) {
      (elem as HTMLElement).focus();
      break; // Break out of the loop once the first visible element is focused
    }
  }
};
const isMobile = computed(() => useWindowSize().width.value < 768);
</script>

<template>
  <button @keyup.enter.stop="focusMainContent" @click="focusMainContent" class="skip-link rounded-3 focus-ring btn bg-white">
    Skip to Main Content
  </button>
  <div style="height: 100dvh" class="d-flex flex-column align-items-center flex-grow-1">
    <Navigation />
    <Toaster closeButton :expand="true" :position="isMobile ? 'top-center' : 'bottom-right'" />
    <main ref="mainRef" tabIndex="-1" class="d-flex flex-column overflow-auto w-100 h-100 overflow-auto">
      <RouterView />
    </main>
    <BottomNavigation v-if="isMobile" />
  </div>
</template>

<style>
.skip-link {
  position: absolute;
  top: -40px;
  left: 0;
  background-color: white;

  padding: 8px;
  z-index: 100;
}

.skip-link:focus {
  top: 8px;
  left: 8px;
}
</style>
