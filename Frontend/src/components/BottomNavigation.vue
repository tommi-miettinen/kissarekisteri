<script lang="ts" setup>
import { computed } from "vue";
import { useRoute } from "vue-router";
import { user } from "../store/userStore";
import UserIcon from "../icons/UserIcon.vue";
import UsersIcon from "../icons/UsersIcon.vue";
import AwardIcon from "../icons/AwardIcon.vue";
import CatIcon from "../icons/CatIcon.vue";
import NotificationIcon from "../icons/NotificationIcon.vue";
import { isCurrentAction, removeAction, pushAction, ActionTypes } from "../store/actionStore";
import { navigateTo } from "../store/routeStore";
import { isMobile } from "../store/actionStore";
import Drawer from "./Drawer.vue";
import Notifications from "./Notifications.vue";

const route = useRoute();

const isCurrentUser = computed(() => user.value?.id === route.params.userId);
</script>

<template>
  <div style="font-size: 12px" class="d-flex p-1 py-2 justify-content-between w-100 border-top fw-semibold">
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
      class="focus-ring position-relative col-2 d-flex flex-column justify-content-center align-items-center rounded-3 p-2"
      @click="pushAction(ActionTypes.NOTIFICATIONS_MOBILE)"
      @keyup.enter="pushAction(ActionTypes.NOTIFICATIONS_MOBILE)"
    >
      <div class="position-relative d-flex flex-column align-items-center h-100">
        <NotificationIcon strokeWidth="1.5" />
        <span :class="{ 'opacity-100': isCurrentUser }" class="d-inline-block mt-auto opacity-75">Ilmoitukset</span>
        <div style="right: 12px" class="position-absolute">
          <span class="badge rounded-circle bg-danger">{{ 1 }}</span>
        </div>
      </div>
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
  <Drawer
    :fullsize="true"
    :visible="isCurrentAction(ActionTypes.NOTIFICATIONS_MOBILE) && isMobile"
    @onCancel="removeAction(ActionTypes.NOTIFICATIONS_MOBILE)"
  >
    <div style="height: 100vh">
      <Notifications :navigateTo="navigateTo" />
    </div>
  </Drawer>
</template>
