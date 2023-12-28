<script lang="ts" setup>
import { computed, ref } from "vue";
import { user, logout } from "../store/userStore";
import { useRouter, useRoute } from "vue-router";
import { useI18n } from "vue-i18n";

const route = useRoute();
const router = useRouter();
const { t, locale } = useI18n();

const avatarLoadError = ref(false);

const handleLocaleClick = () => (locale.value === "fi" ? (locale.value = "en") : (locale.value = "fi"));
const localeString = computed(() => (locale.value === "fi" ? "In English" : "Suomeksi"));

const logoutFromApp = () => {
  logout();
  router.push("/");
};

const avatarRef = ref<HTMLDivElement>();
</script>

<template>
  <nav class="border w-100 p-2 bg-white">
    <ul class="nav align-items-center px-2 gap-1" style="color: black">
      <div tabindex="0" role="button" @keyup.enter="() => avatarRef?.click()" v-if="user" class="dropdown">
        <div ref="avatarRef" class="rounded-circle" type="button" data-bs-toggle="dropdown">
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
        <ul class="dropdown-menu">
          <router-link class="dropdown-item" :to="`/users/${user.id}`">{{ t("Navigation.profile") }}</router-link>
          <li @click="logoutFromApp" class="dropdown-item">{{ t("Navigation.logout") }}</li>
        </ul>
      </div>
      <a data-testid="login-btn" v-if="!user" href="https://localhost:44316/login" class="btn btn-primary">{{ t("Navigation.login") }}</a>
      <li class="nav-item rounded-3" :class="{ 'nav-item-active': route.path.includes('catshows') }">
        <router-link style="color: black" class="nav-link" to="/catshows">{{ t("Navigation.catShows") }}</router-link>
      </li>
      <li class="nav-item rounded-3" :class="{ 'nav-item-active': route.path === '/cats' || route.path.startsWith('/cats/') }">
        <router-link style="color: black" class="nav-link" to="/cats">{{ t("Navigation.cats") }}</router-link>
      </li>
      <li class="nav-item rounded-3" :class="{ 'nav-item-active': route.path.includes('users') }">
        <router-link style="color: black" class="nav-link" to="/users">{{ t("Navigation.members") }}</router-link>
      </li>
      <a style="cursor: pointer" @click="handleLocaleClick" class="ms-auto">{{ localeString }}</a>
    </ul>
  </nav>
</template>

<style>
.nav-item:hover {
  background-color: #f3f4f6;
}
.nav-item-active {
  background-color: #f3f4f6;
}
</style>
