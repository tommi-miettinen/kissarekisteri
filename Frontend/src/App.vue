<script setup lang="ts">
import Navigation from "./components/Navigation.vue";
import { Toaster } from "vue-sonner";
import { ref } from "vue";
import BottomNavigation from "./components/BottomNavigation.vue";
import { focusFirstVisibleElement } from "./utils/focusFirstVisibleElement";
import Appbar from "./components/Appbar.vue";
import { isMobile } from "./store/actionStore";
import { onMounted } from "vue";
import { handleFirstLoad } from "./utils/handleFirstLoad";

const mainRef = ref<HTMLElement>();
const focusMainContent = () => mainRef.value && focusFirstVisibleElement(mainRef.value);

onMounted(() => handleFirstLoad());
</script>

<template>
  <button @keyup.enter.stop="focusMainContent" @click.stop="focusMainContent" class="skip-link rounded-3 focus-ring btn bg-white">
    Skip to Main Content
  </button>
  <div style="height: 100dvh" class="d-flex flex-column align-items-center flex-grow-1">
    <Navigation v-if="!isMobile" />
    <Appbar v-if="isMobile" />
    <div style="pointer-events: none">
      <Toaster :duration="1000" :visibleToasts="1" :position="isMobile ? 'top-center' : 'bottom-right'" />
    </div>
    <main ref="mainRef" tabIndex="-1" class="d-flex flex-column overflow-auto w-100 h-100 overflow-auto">
      <RouterView />
    </main>
    <BottomNavigation v-if="isMobile" />
  </div>
</template>
