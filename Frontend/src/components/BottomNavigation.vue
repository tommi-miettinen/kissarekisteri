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
  <div style="font-size: 12px" class="d-flex py-2 justify-content-around w-100 border-top fw-semibold">
    <div
      tabIndex="0"
      :class="{ 'opacity-100': route.path === '/cats' || route.path.startsWith('/cats/') }"
      class="focus-ring col-2 d-flex flex-column justify-content-center align-items-center rounded-3 p-2 opacity-75"
      @click="navigateTo('/cats')"
      @keyup.enter="navigateTo('/cats')"
    >
      <CatIcon />Kissat
    </div>
    <div
      tabIndex="0"
      :class="{ 'opacity-100': route.path.includes('catshows') }"
      class="focus-ring col-2 d-flex flex-column justify-content-center align-items-center rounded-3 p-2 opacity-75"
      @click="navigateTo('/catshows')"
      @keyup.enter="navigateTo('/catshows')"
    >
      <AwardIcon />
      Näyttelyt
    </div>
    <div
      tabIndex="0"
      :class="{ 'opacity-100': route.path.includes('users') && !isCurrentUser }"
      class="focus-ring col-2 d-flex flex-column justify-content-center align-items-center rounded-3 p-2 opacity-75"
      @click="navigateTo('/users')"
      @keyup.enter="navigateTo('/users')"
    >
      <UsersIcon />
      Jäsenet
    </div>

    <div
      tabIndex="0"
      v-if="user"
      :class="{ 'opacity-100': isCurrentUser }"
      class="focus-ring col-2 d-flex flex-column justify-content-center align-items-center rounded-3 p-2 opacity-75"
      @click="navigateTo(`/users/${user?.id}`)"
      @keyup.enter="navigateTo(`/users/${user?.id}`)"
    >
      <UserIcon strokeWidth="1.5" />
      <span class="d-inline-block text-truncate" style="max-width: 100%"> {{ user?.givenName || "" + user?.surname }}</span>
    </div>
  </div>
</template>
