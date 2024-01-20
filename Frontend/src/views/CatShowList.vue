<script setup lang="ts">
import { ref } from "vue";
import Modal from "../components/Modal.vue";
import { useI18n } from "vue-i18n";
import catShowAPI from "../api/catShowAPI";
import { useQuery, useMutation } from "@tanstack/vue-query";
import { toast } from "vue-sonner";
import { userHasPermission } from "../store/userStore";
import List from "../components/List.vue";
import Drawer from "../components/Drawer.vue";
import CatShowForm from "../components/CatShowForm.vue";
import moment from "moment";
import { QueryKeys } from "../api/queryKeys";
import Spinner from "../components/Spinner.vue";
import { PermissionTypes } from "../store/userStore";
import { navigateTo, setCurrentRouteLabel } from "../store/routeStore";
import { onMounted } from "vue";
import { isMobile, ActionTypes, pushAction, isCurrentAction, removeAction } from "../store/actionStore";

const { t } = useI18n();

const addingEvent = ref(false);

const catShowsQuery = useQuery({
  queryKey: QueryKeys.CAT_SHOWS,
  queryFn: () => catShowAPI.getEvents(),
});

const createCatShowMutation = useMutation({
  mutationFn: (newEvent: CatShowEvent) => catShowAPI.createCatShowEvent(newEvent),
  onSuccess: () => {
    toast.info("Tapahtuma luotu");
    catShowsQuery.refetch();
  },
});

const formatDate = (start: string, end: string) => {
  const startDate = moment(start).format("l");
  const endDate = moment(end).format("l");
  const startTime = moment(start).format("LT");
  const endTime = moment(end).format("LT");
  return `${startDate} - ${endDate}, ${startTime} - ${endTime}`;
};

const searchKeys: SearchKeys<CatShowEvent> = [
  {
    key: "name",
  },
  {
    key: "location",
  },
  {
    key: "startDate",
  },
  {
    key: "endDate",
  },
];

onMounted(() => setCurrentRouteLabel("Näyttelyt"));
</script>

<template>
  <Spinner v-if="catShowsQuery.isLoading.value" />
  <div
    v-if="!catShowsQuery.isLoading.value"
    style="min-height: 100%"
    class="position-relative d-flex flex-column p-3 p-sm-5 rounded col-12 col-lg-8 mx-auto"
  >
    <h3 class="mb-3">{{ t("CatShowList.catShows") }}</h3>
    <List
      :searchKeys="searchKeys"
      :searchQueryPlaceholder="t('CatShowList.searchInput')"
      v-if="catShowsQuery.data.value"
      :items="catShowsQuery.data.value"
      :itemsPerPage="20"
    >
      <template v-slot="{ item: catShow }">
        <div class="py-1 border-bottom">
          <div
            tabindex="0"
            @keyup.enter="navigateTo(`catshows/${catShow.id}`)"
            @click="navigateTo(`catshows/${catShow.id}`)"
            class="d-flex gap-2 rounded-3 p-2 align-items-center pointer hover-bg-1 cursor-pointer focus-ring"
          >
            <div style="position: relative" class="d-flex">
              <img style="width: 80px; height: 80px; object-fit: cover" class="ratio-1x1 rounded-2 border" :src="catShow.imageUrl" />
              <div class="position-absolute top-0 start-0 translate-middle">
                <slot name="medal"></slot>
              </div>
            </div>
            <div class="d-flex flex-column gap-1 px-1">
              <div class="fw-semibold">{{ catShow.name }}</div>
              <div>{{ catShow.location }}</div>
              <div class="ms-auto fw-bold" style="font-size: 13px; margin-top: auto">
                {{ formatDate(catShow.startDate, catShow.endDate) }}
              </div>
            </div>
          </div>
        </div>
      </template>
      <template #action>
        <button
          v-if="!isMobile && userHasPermission(PermissionTypes.CatShowWrite)"
          @click="addingEvent = true"
          type="button"
          class="btn bg-black text-white rounded-3 px-5 ms-auto w-sm-100"
        >
          {{ t("CatShowList.addCatShow") }} +
        </button>
      </template>
    </List>
    <div class="px-5 py-3" />
    <div v-if="isMobile" style="bottom: 74px" class="start-0 position-fixed w-100 p-2">
      <button
        v-if="userHasPermission(PermissionTypes.CatShowWrite)"
        @click="pushAction(ActionTypes.ADDING_CAT_SHOW_MOBILE)"
        type="button"
        style="background-color: black"
        class="btn text-white rounded-3 px-5 py-2 ms-auto w-sm-100"
      >
        {{ t("CatShowList.addCatShow") }} +
      </button>
    </div>
  </div>
  <Drawer
    :fullsize="true"
    :visible="isCurrentAction(ActionTypes.ADDING_CAT_SHOW_MOBILE) && isMobile"
    @onCancel="removeAction(ActionTypes.ADDING_CAT_SHOW_MOBILE)"
  >
    <CatShowForm v-if="isMobile" @onSave="createCatShowMutation.mutate" />
  </Drawer>
  <Modal @onCancel="addingEvent = false" :visible="addingEvent && !isMobile">
    <div style="width: 550px">
      <CatShowForm v-if="!isMobile" @onSave="createCatShowMutation.mutate" />
    </div>
  </Modal>
</template>
