<script setup lang="ts">
import Navigation from "./components/Navigation.vue";
import { Toaster } from "vue-sonner";
import { onMounted } from "vue";
import { fetchUser } from "./store/userStore";

onMounted(() => fetchUser());
</script>

<template>
  <div style="height: 100vh" class="d-flex flex-column align-items-center">
    <Navigation role="navigation" />
    <Toaster richColors closeButton />
    <main class="d-flex flex-column overflow-auto w-100 h-100">
      <RouterView />
    </main>
  </div>
</template>
