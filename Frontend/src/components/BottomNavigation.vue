<script lang="ts" setup>
import { computed } from "vue";
import { useRoute } from "vue-router";
import { user } from "../store/userStore";
import UserIcon from "../icons/UserIcon.vue";
import UsersIcon from "../icons/UsersIcon.vue";
import AwardIcon from "../icons/AwardIcon.vue";
import CatIcon from "../icons/CatIcon.vue";
import NotificationIcon from "../icons/NotificationIcon.vue";
import { navigateTo } from "../store/routeStore";
import { isMobile } from "../store/actionStore";
import Notifications from "./Notifications.vue";
import Overlay from "./Overlay.vue";
import { useI18n } from "vue-i18n";
import catAPI from "../api/catAPI";
import { QueryKeys } from "../api/queryKeys";
import { useQuery } from "@tanstack/vue-query";
import Drawer from "./Drawer.vue";
import { pushAction, isCurrentAction, removeAction, ActionTypes } from "../store/actionStore";
import { login } from "../auth";

const { t } = useI18n();
const route = useRoute();

const isCurrentUser = computed(() => user.value?.id === route.params.userId);

const { data: confirmationRequests } = useQuery({
  queryKey: QueryKeys.CONFIRMATION_REQUESTS,
  queryFn: () => catAPI.getConfirmationRequests(),
  refetchInterval: 5000,
});

const handleUserClick = () => {
  if (user.value) {
    navigateTo(`/users/${user.value.id}`);
    return;
  }

  pushAction(ActionTypes.LOGIN_PROMPT);
};

const handleNotificationClick = () => {
  if (user.value) {
    pushAction(ActionTypes.NOTIFICATIONS_MOBILE);
    return;
  }
  pushAction(ActionTypes.LOGIN_PROMPT);
};

const userText = computed(() => (user.value ? `${user.value.givenName} ${user.value.surname}` : "Profiili"));
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
      :class="{ 'opacity-100': isCurrentUser }"
      class="focus-ring position-relative col-2 d-flex flex-column justify-content-center align-items-center rounded-3 p-2"
      @click="handleNotificationClick"
      @keyup.enter="handleNotificationClick"
    >
      <div class="position-relative d-flex flex-column align-items-center h-100">
        <NotificationIcon strokeWidth="1.5" />
        <span :class="{ 'opacity-100': isCurrentUser }" class="d-inline-block mt-auto opacity-75">Ilmoitukset</span>
        <div v-if="confirmationRequests" style="right: 12px" class="position-absolute">
          <span class="badge rounded-circle bg-danger">{{ confirmationRequests.length }}</span>
        </div>
      </div>
    </div>
    <div
      tabIndex="0"
      :class="{ 'opacity-100': isCurrentUser }"
      class="focus-ring col-2 d-flex flex-column justify-content-center align-items-center rounded-3 p-2 opacity-75"
      @click="handleUserClick"
      @keyup.enter="handleUserClick"
    >
      <UserIcon strokeWidth="1.5" />
      <span class="d-inline-block text-truncate" style="max-width: 100%"> {{ userText }}</span>
    </div>
  </div>
  <Overlay
    :visible="isCurrentAction(ActionTypes.NOTIFICATIONS_MOBILE) && isMobile"
    @onCancel="removeAction(ActionTypes.NOTIFICATIONS_MOBILE)"
  >
    <div style="height: 100vh" class="">
      <h3 class="m-3 mb-0">{{ t("Notifications.notifications") }}</h3>
      <Notifications />
    </div>
  </Overlay>
  <Drawer :visible="isCurrentAction(ActionTypes.LOGIN_PROMPT)" @onCancel="removeAction(ActionTypes.LOGIN_PROMPT)">
    <div style="height: 30vh">
      <div @click="login" class="p-3">Kirjaudu sisään</div>
    </div>
  </Drawer>
</template>
