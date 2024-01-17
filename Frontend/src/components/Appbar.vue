<script lang="ts" setup>
import { currentRouteLabel, navigateBack } from "../store/routeStore";
</script>

<template>
  <div class="bg-white border-bottom w-100 align-items-center d-flex py-3 px-2 gap-2">
    <button @click="navigateBack" class="border-0 d-flex bg-transparent">
      <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24">
        <path fill="none" d="M0 0H24V24H0z"></path>
        <path d="M17.77 3.77L16 2 6 12 16 22 17.77 20.23 9.54 12z"></path>
      </svg>
    </button>
    <div class="fw-semibold">{{ currentRouteLabel }}</div>
  </div>
</template>
