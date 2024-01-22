<script async lang="ts" setup>
import { ref, computed } from "vue";
import { user } from "../store/userStore";
import { useRouter, useRoute } from "vue-router";
import { useI18n } from "vue-i18n";
import { login, logout } from "../auth";
import Drawer from "./Drawer.vue";
import { ActionTypes, pushAction, isCurrentAction, removeAction } from "../store/actionStore";
import { useQuery } from "@tanstack/vue-query";
import catAPI from "../api/catAPI";
import Dropdown from "./Dropdown.vue";
import Avatar from "./Avatar.vue";
import NotificationIcon from "../icons/NotificationIcon.vue";
import moment from "moment";
import Notifications from "./Notifications.vue";
import { QueryKeys } from "../api/queryKeys";
import { navigateTo } from "../store/routeStore";
import { isMobile } from "../store/actionStore";

const route = useRoute();
const router = useRouter();
const { t, locale } = useI18n();

const { data: confirmationRequests } = useQuery({
  queryKey: QueryKeys.CONFIRMATION_REQUESTS,
  queryFn: () => catAPI.getConfirmationRequests(),
  refetchInterval: 5000,
});

const handleLocaleClick = () => {
  locale.value === "fi" ? (locale.value = "en") : (locale.value = "fi");
  localStorage.locale = locale.value;
  moment.locale(locale.value);
};
const localeString = computed(() => (locale.value === "fi" ? "In English" : "Suomeksi"));

const logoutFromApp = () => {
  logout();
  router.push("/");
};

const handleAvatarClick = async () => {
  if (isMobile.value && !isCurrentAction(ActionTypes.BOTTOM_SHEET)) pushAction(ActionTypes.BOTTOM_SHEET);
  dropdownTriggerRef.value?.click();
};

const handleNotificationClick = () => {
  if (isMobile.value) pushAction(ActionTypes.NOTIFICATIONS_MOBILE);
};

const dropdownTriggerRef = ref<HTMLDivElement>();
const requestsRef = ref<HTMLDivElement>();
</script>

<template>
  <nav class="border-bottom w-100 p-2">
    <ul class="nav align-items-center px-1 gap-1">
      <div ref="dropdownTriggerRef">
        <Avatar
          v-if="user"
          @click="handleAvatarClick"
          @keyup.enter="handleAvatarClick"
          :avatarUrl="user.avatarUrl"
          :displayText="user.givenName[0] + user.surname[0]"
        />
      </div>
      <Dropdown :visible="!isMobile" :triggerRef="dropdownTriggerRef">
        <template v-if="user">
          <li
            tabIndex="0"
            @click="navigateTo(`/users/${user.id}`)"
            @keyup.enter="navigateTo(`/users/${user.id}`)"
            class="focus-ring cursor-pointer hover-bg-1 rounded-2 hover-bg px-3 py-2"
          >
            {{ t("Navigation.profile") }}
          </li>
          <li
            tabIndex="0"
            @click="logoutFromApp"
            @keyup.enter="logoutFromApp"
            class="focus-ring cursor-pointer hover-bg-1 rounded-2 hover-bg px-3 py-2"
          >
            {{ t("Navigation.logout") }}
          </li>
        </template>
      </Dropdown>

      <button @click="login" data-testid="login-btn" v-if="!user" class="btn bg-black rounded-3 text-white">
        {{ t("Navigation.login") }}
      </button>
      <li
        @click="navigateTo('/catshows')"
        class="rounded-3 hover-bg-1 px-3 py-2 cursor-pointer"
        :class="{ 'd-none': isMobile, 'bg-1': route.path.includes('catshows') }"
      >
        {{ t("Navigation.catShows") }}
      </li>
      <li
        @click="navigateTo('/cats')"
        class="rounded-3 hover-bg-1 px-3 py-2 cursor-pointer"
        :class="{ 'd-none': isMobile, 'bg-1': route.path === '/cats' || route.path.startsWith('/cats/') }"
      >
        {{ t("Navigation.cats") }}
      </li>
      <li
        @click="navigateTo('/users')"
        class="rounded-3 hover-bg-1 px-3 py-2 cursor-pointer"
        :class="{ 'd-none': isMobile, 'bg-1': route.path.includes('users') }"
      >
        {{ t("Navigation.members") }}
      </li>

      <div
        v-if="!isMobile"
        @click="handleNotificationClick"
        @keyup.enter="handleNotificationClick"
        ref="requestsRef"
        tabindex="0"
        class="hover-bg-1 gap-2 focus-ring cursor-pointer nav-item rounded-3 rounded-3 p-2 ms-auto relative"
      >
        <NotificationIcon />
        <span
          style="margin-left: -8px"
          v-if="confirmationRequests && confirmationRequests.length > 0"
          class="badge rounded-circle bg-danger"
          >{{ confirmationRequests.length }}</span
        >
      </div>
      <Dropdown
        :closeOnFocusLost="false"
        :visible="!isMobile && Boolean(user)"
        :autoClose="false"
        :placement="'bottom-end'"
        :triggerRef="requestsRef"
      >
        <div v-if="user && !isMobile" style="width: 450px">
          <Notifications />
        </div>
      </Dropdown>
      <a
        v-if="!isMobile"
        @keyup.enter="handleLocaleClick"
        @click="handleLocaleClick"
        tabindex="0"
        :class="{ 'ms-auto': isMobile }"
        class="cursor-pointer hover-bg-1 focus-ring p-2 rounded-3 text-black"
        >{{ localeString }}</a
      >
    </ul>
  </nav>
  <Drawer
    :fullsize="true"
    :visible="isCurrentAction(ActionTypes.NOTIFICATIONS_MOBILE) && isMobile"
    @onCancel="removeAction(ActionTypes.NOTIFICATIONS_MOBILE)"
  >
    <Notifications />
  </Drawer>
  <Drawer
    :fullsize="true"
    :visible="isCurrentAction(ActionTypes.BOTTOM_SHEET) && isMobile"
    @onCancel="removeAction(ActionTypes.BOTTOM_SHEET)"
  >
    <div class="p-2">
      <div tabindex="0" @click="logoutFromApp" class="hover-bg btn-border rounded-3 p-2 focus-ring">{{ t("Navigation.logout") }}</div>
    </div>
  </Drawer>
</template>
