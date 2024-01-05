<script setup lang="ts">
import Navigation from "./components/Navigation.vue";
import { Toaster } from "vue-sonner";
import { onMounted } from "vue";
import { fetchPermissions, fetchUser } from "./store/userStore";
import { toastPosition } from "./store/toasterStore";

onMounted(async () => {
  await fetchUser();
  await fetchPermissions();
});

const focusMainContent = () => document.querySelector("main")?.focus();
</script>

<template>
  <button @keyup.enter="focusMainContent" @click="focusMainContent" class="skip-link rounded-3 focus-ring btn bg-white">
    Skip to Main Content
  </button>
  <div style="height: 100vh; max-height: 100vh" class="d-flex flex-column align-items-center">
    <Navigation />
    <Toaster closeButton :expand="true" :position="toastPosition" />
    <main id="maincontent" tabIndex="-1" class="d-flex flex-column overflow-auto w-100 h-100 overflow-auto">
      <RouterView />
    </main>
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
