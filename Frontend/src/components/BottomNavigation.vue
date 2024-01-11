<script lang="ts" setup>
import { computed } from "vue";
import { useRouter, useRoute } from "vue-router";
import { user } from "../store/userStore";
import UserIcon from "../icons/UserIcon.vue";
import UsersIcon from "../icons/UsersIcon.vue";
import AwardIcon from "../icons/AwardIcon.vue";
import CatIcon from "../icons/CatIcon.vue";

const router = useRouter();
const route = useRoute();

const isCurrentUser = computed(() => user.value?.id === route.params.userId);

const navigateTo = (route: string) => router.push(route);
</script>

<template>
  <div style="font-size: 12px" class="d-flex p-0 justify-content-around w-100 border-top fw-semibold">
    <div
      :class="{ 'opacity-100': route.path === '/cats' || route.path.startsWith('/cats/') }"
      class="col-2 d-flex flex-column justify-content-center align-items-center rounded-3 p-2 opacity-75"
      @click="navigateTo('/cats')"
    >
      <CatIcon />Kissat
    </div>
    <div
      :class="{ 'opacity-100': route.path.includes('catshows') }"
      class="col-2 d-flex flex-column justify-content-center align-items-center rounded-3 p-2 opacity-75"
      @click="navigateTo('/catshows')"
    >
      <AwardIcon />
      Näyttelyt
    </div>
    <div
      :class="{ 'opacity-100': route.path.includes('users') && !isCurrentUser }"
      class="col-2 d-flex flex-column justify-content-center align-items-center rounded-3 p-2 opacity-75"
      @click="navigateTo('/users')"
    >
      <UsersIcon />
      Jäsenet
    </div>

    <div
      v-if="user"
      :class="{ 'opacity-100': isCurrentUser }"
      class="col-2 d-flex flex-column justify-content-center align-items-center rounded-3 p-2 opacity-75"
      @click="navigateTo(`/users/${user?.id}`)"
    >
      <UserIcon strokeWidth="1.5" />
      <span class="d-inline-block text-truncate" style="max-width: 100%"> {{ user?.givenName || "" + user?.surname }}</span>
    </div>
  </div>
</template>
