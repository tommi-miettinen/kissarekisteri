<script lang="ts" setup>
import { computed } from "vue";
import catAPI from "../api/catAPI";
import { useQueryClient, useMutation, useQuery } from "@tanstack/vue-query";
import { QueryKeys } from "../api/queryKeys";
import { toast } from "vue-sonner";
import { user } from "../store/userStore";
import { navigateTo } from "../store/routeStore";
import { actionStack, popAction } from "../store/actionStore";
import { user as loggedInUser } from "../store/userStore";
import { currentNotificationTab, setCurrentNotificationTab } from "../store/notificationStore";
import { useI18n } from "vue-i18n";

const queryClient = useQueryClient();

const { t } = useI18n();

const { data: confirmationRequests, refetch: refetchConfirmatioRequests } = useQuery({
  queryKey: QueryKeys.CONFIRMATION_REQUESTS,
  queryFn: () => catAPI.getConfirmationRequests(),
  refetchInterval: 5000,
});

const confirmationRequestMutation = useMutation({
  mutationFn: (requestId: number) => catAPI.confirmTransferRequest(requestId),
  onSuccess: async () => {
    toast.success("Omistajuuspyyntö hyväksytty");
    await queryClient.invalidateQueries({ queryKey: QueryKeys.CAT });
    await queryClient.invalidateQueries({ queryKey: QueryKeys.USER });
    refetchConfirmatioRequests();
  },
});

const adminConfirmationRequests = computed(() => confirmationRequests.value?.filter((request) => request.confirmerId !== user.value?.id));
const personalConfirmationRequests = computed(() =>
  confirmationRequests.value?.filter((request) => request.confirmerId === user.value?.id)
);

const confirmationRequestsToDisplay = computed(() => {
  if (currentNotificationTab.value === "admin") {
    return adminConfirmationRequests.value;
  } else {
    return personalConfirmationRequests.value;
  }
});

const navigate = (path: string) => {
  if (actionStack.value.length > 0) {
    popAction();
  }
  navigateTo(path);
};
</script>

<template>
  <div class="p-3 text-break overflow-auto gap-2 d-flex flex-column">
    <div v-if="loggedInUser?.userRole.roleName === 'Admin'" class="d-flex gap-1">
      <button
        tabindex="0"
        @click="setCurrentNotificationTab('personal')"
        :class="{
          'bg-black': currentNotificationTab === 'personal',
          'text-white': currentNotificationTab === 'personal',
          'border-black': currentNotificationTab === 'personal',
        }"
        class="btn btn-sm border rounded-3 focus-ring col-3"
      >
        {{ t("Notifications.personal") }}
      </button>
      <button
        tabindex="0"
        @click="setCurrentNotificationTab('admin')"
        :class="{
          'bg-black': currentNotificationTab === 'admin',
          'text-white': currentNotificationTab === 'admin',
          'border-black': currentNotificationTab === 'admin',
        }"
        class="btn border btn-sm rounded-3 focus-ring col-3"
      >
        {{ t("Notifications.admin") }}
      </button>
    </div>

    <div v-for="request in confirmationRequestsToDisplay" style="font-size: 14px" class="py-2 d-flex align-items-center gap-2 d-flex">
      <span>
        <a class="cursor-pointer text-underline text-black" @click="navigate(`/users/${request.requester.id}`)">{{
          request.requester?.givenName
        }}</a>
        {{ t("Notifications.requestingCatOwnership") }}
        <a class="cursor-pointer text-underline text-black" @click="navigate(`/cats/${request.cat.id}`)">{{ request.cat?.name }}</a></span
      >
      <button
        @click="confirmationRequestMutation.mutate(request.id)"
        style="min-width: 80px"
        class="btn btn-sm rounded-3 bg-black text-white px-2 py-1 ms-auto mb-auto fs-7"
      >
        {{ t("Buttons.accept") }}
      </button>
    </div>
    <div class="py-3" v-if="!confirmationRequestsToDisplay">Ei ilmoituksia</div>
  </div>
</template>
