<script async lang="ts" setup>
import { ref, onMounted, computed } from "vue";
import { msalInstance } from "../auth";
import { user } from "../store/userStore";
import { useRouter, useRoute } from "vue-router";
import { useI18n } from "vue-i18n";
import { login, logout } from "../auth";
import { useWindowSize } from "@vueuse/core";
import Drawer from "./Drawer.vue";
import { pushAction, isCurrentAction, popAction } from "../store/actionStore";
import { useMutation, useQuery } from "@tanstack/vue-query";
import catAPI from "../api/catAPI";
import Dropdown from "./Dropdown.vue";
import { toast } from "vue-sonner";
import { useQueryClient } from "@tanstack/vue-query";
import { QueryKeys } from "../api/queryKeys";
import Avatar from "./Avatar.vue";
import NotificationIcon from "../icons/NotificationIcon.vue";

enum ActionType {
  NONE = "NONE",
  BOTTOM_SHEET = "BOTTOM_SHEET",
  SIDE_SHEET = "SIDE_SHEET",
}

const toggleAction = (actionType: ActionType, item = null) => {
  if (actionType === ActionType.NONE) {
    popAction();
  }
  if (actionType !== ActionType.NONE) {
    pushAction(actionType);
  }
  if (currentItem.value === item) {
    currentItem.value = null;
  } else {
    currentItem.value = item;
  }
};

const currentItem = ref<any>(null);

const route = useRoute();
const router = useRouter();
const { t, locale } = useI18n();
const queryClient = useQueryClient();

const { data: confirmationRequestsData, refetch } = useQuery({
  queryKey: ["confirmationRequests"],
  queryFn: () => catAPI.getConfirmationRequests(),
  refetchInterval: 5000,
});

const confirmationRequests = computed(() => confirmationRequestsData.value?.data);

const confirmationRequestMutation = useMutation({
  mutationFn: (requestId: number) => catAPI.confirmTransferRequest(requestId),
  onSuccess: () => {
    toast.success("Omistajuuspyyntö hyväksytty");

    queryClient.invalidateQueries({ queryKey: [QueryKeys.CAT] });
    refetch();
  },
});
const handleLocaleClick = () => (locale.value === "fi" ? (locale.value = "en") : (locale.value = "fi"));
const localeString = computed(() => (locale.value === "fi" ? "In English" : "Suomeksi"));

const logoutFromApp = () => {
  logout();
  router.push("/");
};

const isMobile = computed(() => useWindowSize().width.value < 768);

onMounted(async () => await msalInstance.handleRedirectPromise());

const handleAvatarClick = async () => {
  if (isMobile.value) toggleAction(ActionType.BOTTOM_SHEET);
};

const navigateToProfile = () => {
  toggleAction(ActionType.NONE);
  router.push(`/users/${user.value?.id}`);
};

const navigateTo = (route: string) => {
  toggleAction(ActionType.NONE);
  router.push(route);
};

const dropdownTriggerRef = ref<HTMLDivElement>();
const requestsRef = ref<HTMLDivElement>();
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
        class="focus-ring rounded-circle"
        ref="dropdownTriggerRef"
      >
        <Avatar :avatarUrl="user.avatarUrl" :displayText="user.givenName[0] + user.surname[0]" />
      </div>
      <Dropdown :visible="!isMobile" :triggerRef="dropdownTriggerRef">
        <template v-if="user">
          <router-link class="dropdown-item rounded-2 hover-bg px-3 py-2" :to="`/users/${user.id}`">{{
            t("Navigation.profile")
          }}</router-link>
          <li tabIndex="0" @click="logoutFromApp" class="dropdown-item rounded-2 hover-bg px-3 py-2">{{ t("Navigation.logout") }}</li>
        </template>
      </Dropdown>

      <button @click="login" data-testid="login-btn" v-if="!user" class="btn btn-primary">{{ t("Navigation.login") }}</button>
      <li class="nav-item rounded-3 hover-bg" :class="{ 'd-none': isMobile, 'nav-item-active': route.path.includes('catshows') }">
        <router-link style="color: black" class="nav-link rounded-3" to="/catshows">{{ t("Navigation.catShows") }}</router-link>
      </li>
      <li
        class="nav-item rounded-3 hover-bg"
        :class="{ 'd-none': isMobile, 'nav-item-active': route.path === '/cats' || route.path.startsWith('/cats/') }"
      >
        <router-link ref="cats" style="color: black" class="nav-link rounded-3" to="/cats">{{ t("Navigation.cats") }}</router-link>
      </li>
      <li class="nav-item rounded-3 hover-bg" :class="{ 'd-none': isMobile, 'nav-item-active': route.path.includes('users') }">
        <router-link style="color: black" class="nav-link rounded-3" to="/users">{{ t("Navigation.members") }}</router-link>
      </li>

      <div ref="requestsRef" tabindex="0" role="button" class="cursor-pointer nav-item rounded-3 rounded-3 p-2 ms-auto relative">
        <NotificationIcon />

        <span
          style="margin-left: -8px"
          v-if="confirmationRequests && confirmationRequests.length > 0"
          class="badge rounded-circle bg-danger"
          >{{ confirmationRequests.length }}</span
        >
      </div>
      <Dropdown :autoClose="false" :placement="'bottom-end'" :triggerRef="requestsRef">
        <template v-if="user">
          <div class="d-flex flex-column z-100 bg-white" style="max-width: 450px">
            <div v-if="confirmationRequests && confirmationRequests.length > 0">
              <div style="max-height: 600px" class="text-break overflow-auto d-flex flex-column">
                <div v-for="request in confirmationRequests">
                  <div class="p-3 d-flex align-items-center gap-2">
                    <span>
                      <a class="cursor-pointer link-underline-primary" @click="navigateTo(`/users/${request.requester.id}`)">{{
                        request.requester?.givenName
                      }}</a>
                      pyytää omistajuutta kissalle
                      <a class="cursor-pointer link-underline-primary" @click="navigateTo(`/cats/${request.cat.id}`)">{{
                        request.cat?.name
                      }}</a></span
                    >
                    <button @click="confirmationRequestMutation.mutate(request.id)" class="btn btn-primary px-2 py-1 ms-auto">
                      Hyväksy
                    </button>
                  </div>
                </div>
              </div>
            </div>
            <div v-else class="p-2">Ei ilmoituksia</div>
          </div>
        </template>
      </Dropdown>
      <a
        v-if="!isMobile"
        @keyup.enter="handleLocaleClick"
        @click="handleLocaleClick"
        tabindex="0"
        style="cursor: pointer"
        class="focus-ring p-2 rounded-3"
        >{{ localeString }}</a
      >
    </ul>
  </nav>
  <Drawer :visible="isCurrentAction(ActionType.BOTTOM_SHEET) && isMobile" @onCancel="toggleAction(ActionType.NONE)">
    <div class="p-2">
      <div tabindex="0" @click="navigateToProfile" class="hover-bg rounded-3 p-2 focus-ring">{{ t("Navigation.profile") }}</div>
      <div tabindex="0" @click="logoutFromApp" class="hover-bg rounded-3 p-2 focus-ring">{{ t("Navigation.logout") }}</div>
    </div>
  </Drawer>
</template>
