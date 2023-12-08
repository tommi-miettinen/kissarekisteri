<script setup lang="ts">
import Navigation from "./components/Navigation.vue";
import { Toaster } from "vue-sonner";
import { onMounted } from "vue";
import { fetchUser } from "./store/userStore";

onMounted(async () => {
  fetchUser();
});
</script>

<template>
  <div class="d-flex flex-column align-items-center w-100 h-100">
    <Navigation />
    <Toaster richColors closeButton />
    <RouterView />
  </div>
</template>
