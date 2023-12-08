<script lang="ts" setup>
import { userStore, logout } from "../store/userStore";
import { useRouter } from "vue-router";

const user = userStore((state) => state.user);
const router = useRouter();

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
          <router-link class="dropdown-item" to="/profile">Profiili</router-link>
          <li @click="logoutFromApp" class="dropdown-item">Kirjaudu ulos</li>
        </ul>
      </div>
      <a v-if="!user" href="https://localhost:44316/login" class="btn btn-primary">Kirjaudu sisään</a>
      <li class="nav-item">
        <router-link style="color: black" class="nav-link" to="/catshows">Tapahtumat</router-link>
      </li>
      <li class="nav-item">
        <router-link style="color: black" class="nav-link" to="/cats">Kissat</router-link>
      </li>
      <li class="nav-item">
        <router-link style="color: black" class="nav-link" to="/users">Jäsenet</router-link>
      </li>
    </ul>
  </nav>
</template>
