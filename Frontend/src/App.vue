<script setup lang="ts">
import Navigation from "./components/Navigation.vue";
import { Toaster } from "vue-sonner";
import { onMounted } from "vue";
import { fetchPermissions, fetchUser } from "./store/userStore";

onMounted(async () => {
  try {
    await fetchUser();
    await fetchPermissions();
  } catch (error) {
    console.log(error);
  }
});
</script>

<template>
  <a href="#maincontent" class="skip-link rounded-3 focus-ring">Skip to Main Content</a>
  <div style="height: 100vh" class="d-flex flex-column align-items-center">
    <Navigation role="navigation" />
    <Toaster richColors closeButton />
    <main id="maincontent" tabIndex="0" class="d-flex flex-column overflow-auto w-100 h-100 focus-ring">
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
