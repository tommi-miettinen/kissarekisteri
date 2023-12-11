<script lang="ts" setup>
import { computed } from "vue";
import { userStore, logout } from "../store/userStore";
import { useRouter } from "vue-router";
import { useI18n } from "vue-i18n";

const user = userStore((state) => state.user);
const router = useRouter();
const { t, locale } = useI18n();

const handleLocaleClick = () => (locale.value === "fi" ? (locale.value = "en") : (locale.value = "fi"));
const localeString = computed(() => (locale.value === "fi" ? "In English" : "Suomeksi"));

const logoutFromApp = () => {
  logout();
  router.push("/");
};
</script>

<template>
  <nav class="border w-100 p-2 sticky-top bg-white">
    <ul class="nav align-items-center" style="color: black">
      <div v-if="user" class="dropdown">
        <div class="rounded-circle" type="button" data-bs-toggle="dropdown">
          <img class="rounded-circle" height="32" width="32" style="object-fit: fill" :src="user.avatarUrl" alt="Cat Image" />
        </div>
        <ul class="dropdown-menu">
          <router-link class="dropdown-item" to="/profile">{{ t("Navigation.profile") }}</router-link>
          <li @click="logoutFromApp" class="dropdown-item">{{ t("Navigation.logout") }}</li>
        </ul>
      </div>
      <a v-if="!user" href="https://localhost:44316/login" class="btn btn-primary">{{ t("Navigation.login") }}</a>
      <li class="nav-item">
        <router-link style="color: black" class="nav-link" to="/catshows">{{ t("Navigation.catShows") }}</router-link>
      </li>
      <li class="nav-item">
        <router-link style="color: black" class="nav-link" to="/cats">{{ t("Navigation.cats") }}</router-link>
      </li>
      <li class="nav-item">
        <router-link style="color: black" class="nav-link" to="/users">{{ t("Navigation.members") }}</router-link>
      </li>
      <a style="cursor: pointer" @click="handleLocaleClick" class="ms-auto">{{ localeString }}</a>
    </ul>
  </nav>
</template>
