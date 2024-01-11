<script lang="ts" setup>
import { computed } from "vue";
import catAPI from "../api/catAPI";
import { useQueryClient, useMutation, useQuery } from "@tanstack/vue-query";
import { QueryKeys } from "../api/queryKeys";
import { toast } from "vue-sonner";

defineProps({
  navigateTo: {
    type: Function,
    required: true,
  },
});

const queryClient = useQueryClient();

const { data: confirmationRequestsData, refetch } = useQuery({
  queryKey: ["confirmationRequests"],
  queryFn: () => catAPI.getConfirmationRequests(),
  refetchInterval: 5000,
});

const confirmationRequests = computed(() => confirmationRequestsData.value?.data);

const confirmationRequestMutation = useMutation({
  mutationFn: (requestId: number) => catAPI.confirmTransferRequest(requestId),
  onSuccess: async () => {
    toast.success("Omistajuuspyyntö hyväksytty");
    await queryClient.invalidateQueries({ queryKey: [QueryKeys.CAT] });
    await queryClient.invalidateQueries({ queryKey: [QueryKeys.USER] });
    refetch();
  },
});
</script>

<template>
  <div class="d-flex flex-column">
    <div v-if="confirmationRequests && confirmationRequests.length > 0">
      <div class="p-3 text-break overflow-auto d-flex flex-column">
        <div v-for="request in confirmationRequests">
          <div class="py-3 d-flex gap-2">
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
      </div>
    </div>
    <div v-else class="p-2">Ei ilmoituksia</div>
  </div>
</template>
