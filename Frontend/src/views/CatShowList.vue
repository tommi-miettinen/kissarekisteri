<script setup lang="ts">
import { ref, computed } from "vue";
import { useRouter } from "vue-router";
import Modal from "../components/Modal.vue";
import { useI18n } from "vue-i18n";
import { formatDate } from "../utils/formatDate";
import catShowAPI from "../api/catShowAPI";
import { useQuery, useMutation } from "@tanstack/vue-query";
import { toast } from "vue-sonner";
import { userHasPermission } from "../store/userStore";
import List from "../components/List.vue";
import Drawer from "../components/Drawer.vue";
import { useWindowSize } from "@vueuse/core";
import CatShowForm from "../components/CatShowForm.vue";

const router = useRouter();
const { t } = useI18n();

const addingEvent = ref(false);

const { data: catShows, refetch: refetchCatShows } = useQuery({
  queryKey: ["catshows"],
  queryFn: () => catShowAPI.getEvents(),
});

const createCatShowMutation = useMutation({
  mutationFn: (newEvent: CatShowEvent) => catShowAPI.createCatShowEvent(newEvent),
  onSuccess: () => {
    toast.info("Tapahtuma luotu"), refetchCatShows();
  },
});

const isMobile = computed(() => useWindowSize().width.value < 768);

const navigateToEvent = (eventId: number) => router.push(`/catshows/${eventId}`);
</script>
<template>
  <div style="min-height: 100%" class="d-flex flex-column p-2 p-sm-5 rounded col-12 col-lg-8 mx-auto">
    <h3 class="m-0">{{ t("CatShowList.catShows") }}</h3>
    <List :searchQueryPlaceholder="t('CatShowList.searchInput')" v-if="catShows" :items="catShows" :itemsPerPage="20">
      <template v-slot="{ item: catShow }">
        <div class="py-1 border-bottom">
          <div
            tabindex="0"
            @keyup.enter="() => navigateToEvent(catShow.id!)"
            @click="() => navigateToEvent(catShow.id!)"
            class="d-flex gap-4 rounded-3 p-2 align-items-center pointer hover-bg focus-ring"
          >
            <div style="height: 60px" class="d-flex">
              <img
                style="width: 100%; height: 100%; object-fit: contain"
                class="rounded-2"
                src="https://kissarekisteritf.blob.core.windows.net/images/a2174d16-0f1e-452f-b1a8-2c2d58600d05.jpg"
              />
            </div>
            <div class="d-flex flex-column gap-1">
              <div>{{ catShow.name }}</div>
              <div class="text-body-secondary">{{ catShow.location }}</div>
              <div v-if="isMobile" style="font-size: 12px; font-weight: bold">
                {{ `${formatDate(catShow.startDate)} -  ${formatDate(catShow.endDate)}` }}
              </div>
            </div>
            <div v-if="!isMobile" class="ms-auto" style="font-size: 12px; font-weight: bold; margin-top: auto">
              {{ `${formatDate(catShow.startDate)} -  ${formatDate(catShow.endDate)}` }}
            </div>
          </div>
        </div>
      </template>
      <template #action>
        <button
          v-if="userHasPermission('CreateEvent')"
          @click="addingEvent = true"
          type="button"
          class="btn btn-primary ms-auto px-5 mt-2 rounded-3 w-sm-100"
        >
          {{ t("CatShowList.addCatShow") }} +
        </button>
      </template>
    </List>
  </div>
  <Drawer :fullsize="true" :visible="addingEvent && isMobile">
    <CatShowForm @onSave="createCatShowMutation.mutate" />
  </Drawer>
  <Modal @onCancel="addingEvent = false" :visible="addingEvent && !isMobile">
    <div style="width: 550px">
      <CatShowForm @onSave="createCatShowMutation.mutate" />
    </div>
  </Modal>
</template>
