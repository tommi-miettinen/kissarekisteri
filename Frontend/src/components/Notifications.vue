<script lang="ts" setup>
import { computed, ref } from "vue";
import catAPI from "../api/catAPI";
import { useQueryClient, useMutation, useQuery } from "@tanstack/vue-query";
import { QueryKeys } from "../api/queryKeys";
import { toast } from "vue-sonner";
import { user } from "../store/userStore";

defineProps({
  navigateTo: {
    type: Function,
    required: true,
  },
});

const queryClient = useQueryClient();

const { data: confirmationRequestsData, refetch } = useQuery({
  queryKey: QueryKeys.CONFIRMATION_REQUESTS,
  queryFn: () => catAPI.getConfirmationRequests(),
  refetchInterval: 5000,
});

const confirmationRequests = computed(() => confirmationRequestsData.value?.data);

const confirmationRequestMutation = useMutation({
  mutationFn: (requestId: number) => catAPI.confirmTransferRequest(requestId),
  onSuccess: async () => {
    toast.success("Omistajuuspyyntö hyväksytty");
    await queryClient.invalidateQueries({ queryKey: QueryKeys.CAT });
    await queryClient.invalidateQueries({ queryKey: QueryKeys.USER });
    refetch();
  },
});

const tab = ref<"personal" | "admin">("personal");

const adminConfirmationRequests = computed(() => confirmationRequests.value?.filter((request) => request.confirmerId !== user.value?.id));
const personalConfirmationRequests = computed(() =>
  confirmationRequests.value?.filter((request) => request.confirmerId === user.value?.id)
);

const confirmationRequestsToDisplay = computed(() => {
  if (tab.value === "admin") {
    return adminConfirmationRequests.value;
  } else {
    return personalConfirmationRequests.value;
  }
});
</script>

<template>
  <div class="d-flex flex-column">
    <div>
      <div class="p-3 text-break overflow-auto d-flex flex-column">
        <div class="d-flex gap-1 justify-content-center">
          <button
            tabindex="0"
            @click="tab = 'personal'"
            :class="{ 'bg-black': tab === 'personal', 'text-white': tab === 'personal', 'border-black': tab === 'personal' }"
            class="btn btn-sm border rounded-3 focus-ring col-3"
          >
            Omat
          </button>
          <button
            tabindex="0"
            @click="tab = 'admin'"
            :class="{ 'bg-black': tab === 'admin', 'text-white': tab === 'admin', 'border-black': tab === 'admin' }"
            class="btn border btn-sm rounded-3 focus-ring col-3"
          >
            Ylläpitäjä
          </button>
        </div>
        <div v-for="request in confirmationRequestsToDisplay">
          <div class="py-3 d-flex gap-2 d-flex flex-column">
            <span>
              <a class="cursor-pointer text-underline text-black" @click="navigateTo(`/users/${request.requester.id}`)">{{
                request.requester?.givenName
              }}</a>
              pyytää omistajuutta kissalle
              <a class="cursor-pointer text-underline text-black" @click="navigateTo(`/cats/${request.cat.id}`)">{{
                request.cat?.name
              }}</a></span
            >
            <button
              @click="confirmationRequestMutation.mutate(request.id)"
              style="min-width: 80px"
              class="btn btn-sm rounded-3 bg-black text-white px-2 py-1 ms-auto mb-auto fs-7"
            >
              Hyväksy
            </button>
          </div>
        </div>
        <div class="py-3" v-if="!confirmationRequestsToDisplay || confirmationRequestsToDisplay.length === 0">Ei ilmoituksia</div>
      </div>
    </div>
  </div>
</template>
