<script async lang="ts" setup>
import { ref, onMounted, computed, nextTick } from "vue";
import { msalInstance } from "../auth";
import { user } from "../store/userStore";
import { useRouter, useRoute } from "vue-router";
import { useI18n } from "vue-i18n";
import { login, logout } from "../auth";
import { useWindowSize } from "@vueuse/core";
import Drawer from "./Drawer.vue";
import { pushAction, isCurrentAction, removeAction } from "../store/actionStore";
import { useQuery } from "@tanstack/vue-query";
import catAPI from "../api/catAPI";
import Dropdown from "./Dropdown.vue";
import Avatar from "./Avatar.vue";
import NotificationIcon from "../icons/NotificationIcon.vue";
import moment from "moment";
import Notifications from "./Notifications.vue";
import { QueryKeys } from "../api/queryKeys";

enum ActionType {
  BOTTOM_SHEET = "BOTTOM_SHEET",
  SIDE_SHEET = "SIDE_SHEET",
  NOTIFICATIONS_MOBILE = "NOTIFICATIONS_MOBILE",
}

const route = useRoute();
const router = useRouter();
const { t, locale } = useI18n();

const { data: confirmationRequestsData } = useQuery({
  queryKey: QueryKeys.CONFIRMATION_REQUESTS,
  queryFn: () => catAPI.getConfirmationRequests(),
  refetchInterval: 5000,
});

const confirmationRequests = computed(() => confirmationRequestsData.value?.data);

const handleLocaleClick = () => {
  locale.value === "fi" ? (locale.value = "en") : (locale.value = "fi");
  moment.locale(locale.value);
};
const localeString = computed(() => (locale.value === "fi" ? "In English" : "Suomeksi"));

const logoutFromApp = () => {
  logout();
  router.push("/");
};

const isMobile = computed(() => useWindowSize().width.value < 768);

onMounted(async () => await msalInstance.handleRedirectPromise());

const handleAvatarClick = async () => {
  if (isMobile.value) pushAction(ActionType.BOTTOM_SHEET);
  dropdownTriggerRef.value?.click();
};

const navigateToProfile = () => {
  removeAction(ActionType.BOTTOM_SHEET);
  removeAction(ActionType.NOTIFICATIONS_MOBILE);
  nextTick(() => router.push(`/users/${user.value?.id}`));
};
const navigateTo = (route: string) => {
  removeAction(ActionType.NOTIFICATIONS_MOBILE);
  removeAction(ActionType.BOTTOM_SHEET);
  nextTick(() => router.push(route));
};

const handleNotificationClick = () => {
  if (isMobile.value) pushAction(ActionType.NOTIFICATIONS_MOBILE);
};

const dropdownTriggerRef = ref<HTMLDivElement>();
const requestsRef = ref<HTMLDivElement>();
</script>

<template>
  <nav class="border-bottom w-100 p-2 bg-white">
    <ul class="nav align-items-center px-2 gap-1" style="color: black">
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
      <li class="nav-item rounded-3 hover-bg-1" :class="{ 'd-none': isMobile, 'bg-1': route.path.includes('catshows') }">
        <router-link style="color: black" class="nav-link rounded-3" to="/catshows">{{ t("Navigation.catShows") }}</router-link>
      </li>
      <li
        class="nav-item rounded-3 hover-bg-1"
        :class="{ 'd-none': isMobile, 'bg-1': route.path === '/cats' || route.path.startsWith('/cats/') }"
      >
        <router-link ref="cats" style="color: black" class="nav-link rounded-3" to="/cats">{{ t("Navigation.cats") }}</router-link>
      </li>
      <li class="nav-item rounded-3 hover-bg-1" :class="{ 'd-none': isMobile, 'bg-1': route.path.includes('users') }">
        <router-link style="color: black" class="nav-link rounded-3" to="/users">{{ t("Navigation.members") }}</router-link>
      </li>

      <div
        @click="handleNotificationClick"
        @keyup.enter="handleNotificationClick"
        ref="requestsRef"
        tabindex="0"
        class="hover-bg-1 focus-ring cursor-pointer nav-item rounded-3 rounded-3 p-2 ms-auto relative"
      >
        <NotificationIcon />
        <span
          style="margin-left: -8px"
          v-if="confirmationRequests && confirmationRequests.length > 0"
          class="badge rounded-circle bg-danger"
          >{{ confirmationRequests.length }}</span
        >
      </div>
      <Dropdown :visible="!isMobile" :autoClose="false" :placement="'bottom-end'" :triggerRef="requestsRef">
        <template v-if="user && !isMobile">
          <div style="width: 450px">
            <Notifications :navigateTo="navigateTo" />
          </div>
        </template>
      </Dropdown>
      <a
        @keyup.enter="handleLocaleClick"
        @click="handleLocaleClick"
        tabindex="0"
        style="cursor: pointer"
        class="hover-bg-1 focus-ring p-2 rounded-3 text-black"
        >{{ localeString }}</a
      >
    </ul>
  </nav>
  <Drawer
    :fullsize="true"
    :visible="isCurrentAction(ActionType.NOTIFICATIONS_MOBILE) && isMobile"
    @onCancel="removeAction(ActionType.NOTIFICATIONS_MOBILE)"
  >
    <Notifications :navigateTo="navigateTo" />
  </Drawer>
  <Drawer :visible="isCurrentAction(ActionType.BOTTOM_SHEET) && isMobile" @onCancel="removeAction(ActionType.BOTTOM_SHEET)">
    <div class="p-2">
      <div tabindex="0" @click="navigateToProfile" class="hover-bg rounded-3 p-2 focus-ring">{{ t("Navigation.profile") }}</div>
      <div tabindex="0" @click="logoutFromApp" class="hover-bg rounded-3 p-2 focus-ring">{{ t("Navigation.logout") }}</div>
    </div>
  </Drawer>
</template>
