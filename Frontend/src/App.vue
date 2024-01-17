<script setup lang="ts">
import Navigation from "./components/Navigation.vue";
import { Toaster } from "vue-sonner";
import { onMounted, computed, ref } from "vue";
import { fetchPermissions, fetchUser } from "./store/userStore";
import BottomNavigation from "./components/BottomNavigation.vue";
import { useWindowSize } from "@vueuse/core";
import { focusFirstVisibleElement } from "./utils/focusFirstVisibleElement";

const mainRef = ref<HTMLElement>();

onMounted(async () => {
  await fetchUser();
  await fetchPermissions();
});

const focusMainContent = () => focusFirstVisibleElement(mainRef.value!);

const isMobile = computed(() => useWindowSize().width.value < 768);
</script>

<template>
  <button @keyup.enter.stop="focusMainContent" @click.stop="focusMainContent" class="skip-link rounded-3 focus-ring btn bg-white">
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
