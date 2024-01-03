<script lang="ts" setup>
import { ref, onMounted, computed, watch } from "vue";
import { msalInstance } from "../auth";

import { user, logout } from "../store/userStore";
import { useRouter, useRoute } from "vue-router";
import { useI18n } from "vue-i18n";
import { scopes } from "../auth";
import { useWindowSize } from "@vueuse/core";

import { Offcanvas } from "bootstrap";
import DropdownVue from "./Dropdown.vue";

const route = useRoute();
const router = useRouter();
const { t, locale } = useI18n();

const avatarLoadError = ref(false);

const login = () => {
  msalInstance
    .loginRedirect({
      scopes,
    })
    .then((res) => console.log(res))
    .catch((err) => console.log(err));
};

const handleLocaleClick = () => (locale.value === "fi" ? (locale.value = "en") : (locale.value = "fi"));
const localeString = computed(() => (locale.value === "fi" ? "In English" : "Suomeksi"));

const logoutFromApp = () => {
  logout();
  msalInstance
    .logout()
    .then((res) => console.log(res))
    .catch((err) => console.log(err));
  router.push("/");
};

const isMobile = computed(() => useWindowSize().width.value < 768);

onMounted(async () => {
  await msalInstance.handleRedirectPromise();
});

let bsOffcanvas: Offcanvas;
let sideOffCanvas: Offcanvas;

onMounted(() => {
  bsOffcanvas = new Offcanvas(offCanvasRef.value as HTMLDivElement);
  sideOffCanvas = new Offcanvas(sideOffcanvasRef.value as HTMLDivElement);
});

const handleAvatarClick = async () => {
  if (isMobile.value) {
    bsOffcanvas.toggle();
  }
};
const offCanvasRef = ref<HTMLDivElement>();
const sideOffcanvasRef = ref<HTMLDivElement>();
const dropdownTriggerRef = ref<HTMLDivElement>();

const handleNavigateToProfile = () => {
  bsOffcanvas.hide();
  navigateToProfile();
};

const toggleSideOffCanvas = () => {
  if (isMobile.value) sideOffCanvas.toggle();
};

const navigateToProfile = () => router.push(`/users/${user.value?.id}`);

watch(route, () => sideOffCanvas.hide());
</script>

<template>
  <nav class="border w-100 p-2 bg-white">
    <ul class="nav align-items-center px-2 gap-1" style="color: black">
      <div
        tabindex="0"
        role="button"
        @click.stop="handleAvatarClick"
        @keyup.enter="handleAvatarClick"
        v-if="user"
        class="focus-ring rounded-circle bg-danger"
        ref="dropdownTriggerRef"
      >
        <div class="rounded-circle" type="button">
          <img
            v-if="user.avatarUrl && !avatarLoadError"
            class="rounded-circle"
            height="32"
            width="32"
            style="object-fit: fill"
            :src="user.avatarUrl"
            alt="User avatar"
            :onerror="(avatarLoadError = true)"
          />
          <div
            style="width: 32px; height: 32px; font-size: 14px"
            class="rounded-circle d-flex align-items-center justify-content-center bg-primary fw-bold"
            v-else
          >
            {{ user.givenName[0] + user.surname[0] }}
          </div>
        </div>
      </div>
      <DropdownVue :visible="!isMobile" :triggerRef="dropdownTriggerRef">
        <template v-if="user">
          <router-link class="dropdown-item" :to="`/users/${user.id}`">{{ t("Navigation.profile") }}</router-link>
          <li tabIndex="0" @click="logoutFromApp" class="dropdown-item">{{ t("Navigation.logout") }}</li>
        </template>
      </DropdownVue>

      <button @click="login" data-testid="login-btn" v-if="!user" class="btn btn-primary">{{ t("Navigation.login") }}</button>
      <li class="nav-item rounded-3" :class="{ 'd-none': isMobile, 'nav-item-active': route.path.includes('catshows') }">
        <router-link style="color: black" class="nav-link rounded-3" to="/catshows">{{ t("Navigation.catShows") }}</router-link>
      </li>
      <li
        class="nav-item rounded-3"
        :class="{ 'd-none': isMobile, 'nav-item-active': route.path === '/cats' || route.path.startsWith('/cats/') }"
      >
        <router-link ref="cats" style="color: black" class="nav-link rounded-3" to="/cats">{{ t("Navigation.cats") }}</router-link>
      </li>
      <li class="nav-item rounded-3" :class="{ 'd-none': isMobile, 'nav-item-active': route.path.includes('users') }">
        <router-link style="color: black" class="nav-link rounded-3" to="/users">{{ t("Navigation.members") }}</router-link>
      </li>
      <a
        @keyup.enter="handleLocaleClick"
        @click="handleLocaleClick"
        tabindex="0"
        style="cursor: pointer"
        class="ms-auto focus-ring p-2 rounded-3"
        >{{ localeString }}</a
      >
      <button type="button" v-if="isMobile" class="navbar-toggler focus-ring p-2 rounded-3" @click="toggleSideOffCanvas">
        <svg
          xmlns="http://www.w3.org/2000/svg"
          fill="none"
          viewBox="0 0 24 24"
          strokeWidth="{1.5}"
          stroke="currentColor"
          style="width: 24px; height: 24px"
        >
          <path strokeLinecap="round" strokeLinejoin="round" d="M3.75 6.75h16.5M3.75 12h16.5m-16.5 5.25h16.5" />
        </svg>
      </button>
    </ul>
  </nav>
  <div
    ref="sideOffcanvasRef"
    style="height: 100%"
    class="offcanvas offcanvas-end w-100"
    tabindex="-1"
    aria-labelledby="offcanvasRightLabel"
  >
    <div class="offcanvas-header">
      <button type="button" class="btn-close ms-auto" data-bs-dismiss="offcanvas" aria-label="Close"></button>
    </div>
    <div class="d-flex flex-column p-3 gap-1 list-unstyled">
      <li class="nav-item rounded-3" :class="{ 'nav-item-active': route.path.includes('catshows') }">
        <router-link style="color: black" class="nav-link rounded-3 p-2" to="/catshows">{{ t("Navigation.catShows") }}</router-link>
      </li>
      <li class="nav-item rounded-3" :class="{ 'nav-item-active': route.path === '/cats' || route.path.startsWith('/cats/') }">
        <router-link ref="cats" style="color: black" class="nav-link rounded-3 p-2" to="/cats">{{ t("Navigation.cats") }}</router-link>
      </li>
      <li class="nav-item rounded-3" :class="{ 'nav-item-active': route.path.includes('users') }">
        <router-link style="color: black" class="nav-link rounded-3 p-2" to="/users">{{ t("Navigation.members") }}</router-link>
      </li>
    </div>
  </div>
  <div ref="offCanvasRef" class="offcanvas offcanvas-bottom" tabindex="-1" aria-labelledby="offcanvasBottomLabel">
    <div class="offcanvas-header">
      <button type="button" class="btn-close ms-auto" data-bs-dismiss="offcanvas" aria-label="Close"></button>
    </div>
    <div class="p-2">
      <div tabindex="0" @click="handleNavigateToProfile" class="hover-bg rounded-3 p-2 focus-ring">{{ t("Navigation.profile") }}</div>
      <div tabindex="0" @click="logoutFromApp" class="hover-bg rounded-3 p-2 focus-ring">{{ t("Navigation.logout") }}</div>
    </div>
  </div>
</template>

<style>
.nav-item:hover {
  background-color: #f3f4f6;
}
.nav-item-active {
  background-color: #f3f4f6;
}
</style>
