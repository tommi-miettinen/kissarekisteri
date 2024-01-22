<script lang="ts" setup>
import { ref, computed, watchEffect, watch } from "vue";
import userAPI from "../api/userAPI";
import { userHasPermission, user } from "../store/userStore";
import { useRoute } from "vue-router";
import { toast } from "vue-sonner";
import { useMutation, useQuery } from "@tanstack/vue-query";
import Modal from "../components/Modal.vue";
import catShowAPI from "../api/catShowAPI";
import { useI18n } from "vue-i18n";
import CatListItem from "../components/CatListItem.vue";
import getMedalColor from "../utils/getMedalColor";
import ImageGallery from "../components/ImageGallery.vue";
import Dropdown from "../components/Dropdown.vue";
import { ActionTypes, isCurrentAction, removeAction, pushAction } from "../store/actionStore";
import ThreeDotsIcon from "../icons/ThreeDotsIcon.vue";
import Drawer from "../components/Drawer.vue";
import { isMobile } from "../store/actionStore";
import { QueryKeys } from "../api/queryKeys";
import { PermissionTypes } from "../store/userStore";
import { setCurrentRouteLabel } from "../store/routeStore";
import { useFileDialog } from "@vueuse/core";
import { formatDate } from "../utils/formatDate";
import { catAltUrl } from "../placeholders";

const route = useRoute();
const { t } = useI18n();

const selectedCatIds = ref<number[]>([]);
const eventId = +route.params.eventId;

const {
  data: catShow,
  refetch,
  isLoading,
  isError,
} = useQuery({ queryKey: QueryKeys.CAT_SHOW_BY_ID(eventId), queryFn: () => catShowAPI.getEventById(eventId) });

const usersAttendingCats = computed(() => catShow.value?.cats.filter((c) => c.cat.ownerId === user.value?.id) || []);
const isUserAnAttendee = computed(() => catShow.value && catShow.value.cats.some((cat) => cat.cat.ownerId === user.value?.id));

const joinEventMutation = useMutation({
  mutationFn: () => catShowAPI.joinEvent(eventId, selectedCatIds.value),
  onSuccess: () => {
    toast.info("Osallistuminen rekisteröity");
    refetch();
    removeAction(ActionTypes.JOINING_EVENT);
    removeAction(ActionTypes.JOINING_EVENT_MOBILE);
  },
});

const updatePlacingMutation = useMutation({
  mutationFn: (payload: CatShowResultPayload) => catShowAPI.assignCatPlacing(eventId, payload),
  onSuccess: () => {
    toast.info("Näyttelytulos tallennettu");
    refetch();
  },
  onError: () => toast.error("Toiminto epäonnistui"),
});

const uploadMutation = useMutation({
  mutationFn: (file: File) => catShowAPI.addCatShowPhoto(eventId, file),
  onSuccess: () => refetch(),
});

const leaveEventMutation = useMutation({
  mutationFn: () => catShowAPI.leaveEvent(eventId),
  onSuccess: () => {
    toast.info("Osallistuminen peruttu");
    refetch();
    removeAction(ActionTypes.LEAVING_EVENT);
  },
});

const { data: userCats, refetch: refetchUserCats } = useQuery({
  queryKey: ["userCats"],
  queryFn: () => user.value && userAPI.getCatsByUserId(user.value.id),
  enabled: Boolean(user.value),
});

watch([() => catShow.value, () => userCats.value], () => {
  selectedCatIds.value = catShow.value ? usersAttendingCats.value.map((c) => c.cat.id) : [];
});

watchEffect(() => user && refetchUserCats());
watchEffect(() => catShow.value && setCurrentRouteLabel(catShow.value.name));

const leavingEvent = ref(false);

const { onChange, open } = useFileDialog({
  accept: "image/*",
});

onChange((files) => {
  if (!files) return;
  uploadMutation.mutate(files[0]);
});

const catsGroupedByBreed = computed(() => {
  if (catShow.value?.cats) {
    const groupedCats = catShow.value?.cats.map((c) => c.cat);

    const groupedCatsByBreed = groupedCats.reduce((acc: CatsGroupedByBreed, cat) => {
      if (!acc[cat.breed]) {
        acc[cat.breed] = [];
      }
      acc[cat.breed].push(cat);
      return acc;
    }, {});

    return groupedCatsByBreed;
  }
  return {};
});

const leaveEvent = () => {
  leaveEventMutation.mutate();
  leavingEvent.value = false;
};

const toggleCheckbox = (catId: number) => {
  const catExists = selectedCatIds.value.includes(catId);

  if (catExists) {
    return (selectedCatIds.value = selectedCatIds.value.filter((id) => id !== catId));
  }

  selectedCatIds.value = [...selectedCatIds.value, catId];
};

const dropdownRefs = ref<Record<string, HTMLDivElement>>({});

const startJoiningCatShow = () => (isMobile.value ? pushAction(ActionTypes.JOINING_EVENT_MOBILE) : pushAction(ActionTypes.JOINING_EVENT));

const removeSingleCat = (catId: number) => {
  const userCats = catShow.value?.cats.filter((c) => c.cat.ownerId === user.value?.id && c.cat.id !== catId);
  selectedCatIds.value = userCats?.map((c) => c.cat.id) || [];
  joinEventMutation.mutate();
};

const toggleAllCats = () => {
  if (selectedCatIds.value.length > 0) {
    selectedCatIds.value = [];
    return;
  }
  if (!userCats.value) return;
  selectedCatIds.value = userCats.value.map((c) => c.id);
};
</script>

<template>
  <h3 v-if="isError" class="m-5 fw-bold">{{ t("CatShowDetails.404") }}</h3>
  <div v-if="isLoading" class="spinner-border text-black m-auto" role="status">
    <span class="visually-hidden">Loading...</span>
  </div>
  <div v-if="catShow" class="w-100 d-flex flex-column align-items-center p-2 p-xl-5 col-12 col-xxl-8 d-flex flex-column">
    <div class="col-12 col-xxl-8 flex-grow-1 d-flex flex-column gap-2 gap-sm-5">
      <div class="d-flex flex-column flex-md-row gap-sm-4 gap-2 hero-container">
        <div class="border rounded-4 hero-image" style="position: relative; overflow: hidden">
          <img
            style="position: absolute; top: 0; left: 0; width: 100%; height: 100%; object-fit: cover"
            :src="catShow.imageUrl || catAltUrl"
          />
        </div>
        <div class="d-flex flex-column gap-2 w-100">
          <div>
            <div class="d-flex flex-column gap-2 p-2">
              <h3>{{ catShow.name }}</h3>
              <div>{{ catShow.description }}</div>
              <div>{{ catShow.location }}</div>
              <div class="fw-semibold">{{ formatDate(catShow.startDate, catShow.endDate) }}</div>
            </div>
          </div>
          <div v-if="user" class="mt-auto ms-auto w-sm-100">
            <button
              v-if="!isUserAnAttendee"
              type="button"
              class="btn bg-black text-white rounded-3 py-2 px-5 w-sm-100 focus-ring"
              @click="startJoiningCatShow"
            >
              {{ t("CatShowDetails.joinEvent") }}
            </button>
            <button
              v-else
              @click="startJoiningCatShow"
              type="button"
              class="w-sm-100 btn bg-black text-white rounded-3 py-2 px-5 focus-ring"
            >
              {{ t("CatShowDetails.updateAttendance") }}
            </button>
          </div>
        </div>
      </div>
      <div class="attendees-list mt-3 d-flex flex-column gap-5">
        <div v-for="(cats, breed) in catsGroupedByBreed" :key="breed">
          <h4>{{ breed }}</h4>

          <div class="d-flex flex-column align-items-center" v-if="cats">
            <CatListItem v-for="cat in cats" :cat="cat">
              <template #medal>
                <div
                  :style="{ backgroundColor: getMedalColor(cat.results.find((result) => result.catShowId === eventId)?.place || 0) }"
                  v-if="cat.results"
                  class="badge rounded-pill fw-bold text-black"
                >
                  #{{ cat.results.find((result) => result.catShowId === eventId)?.place }}
                </div>
              </template>
              <template #actions>
                <button
                  v-if="userHasPermission(PermissionTypes.CatShowWrite) || cat.ownerId === user?.id"
                  :ref="el => (dropdownRefs[cat.id] = el as HTMLDivElement)"
                  :id="cat.id.toString()"
                  tabndex="0"
                  @click.stop
                  @keyup.enter.stop
                  class="btn py-2 focus-ring d-flex rounded-1 border-0"
                >
                  <ThreeDotsIcon />
                </button>
                <Dropdown :placement="'left-start'" :triggerRef="dropdownRefs[cat.id]">
                  <li
                    v-if="userHasPermission(PermissionTypes.CatShowWrite)"
                    tabindex="0"
                    @keyup.enter="updatePlacingMutation.mutate({ catId: cat.id, place: 1, breed: cat.breed })"
                    @click="updatePlacingMutation.mutate({ catId: cat.id, place: 1, breed: cat.breed })"
                    class="hover-bg-1 focus-ring px-3 py-2 rounded-2"
                  >
                    Ensimmäinen
                  </li>
                  <li
                    v-if="userHasPermission(PermissionTypes.CatShowWrite)"
                    tabindex="0"
                    @keyup.enter="updatePlacingMutation.mutate({ catId: cat.id, place: 2, breed: cat.breed })"
                    @click="updatePlacingMutation.mutate({ catId: cat.id, place: 2, breed: cat.breed })"
                    class="hover-bg-1 focus-ring px-3 py-2 rounded-2"
                  >
                    Toinen
                  </li>
                  <li
                    v-if="userHasPermission(PermissionTypes.CatShowWrite)"
                    tabindex="0"
                    @keyup.enter="updatePlacingMutation.mutate({ catId: cat.id, place: 3, breed: cat.breed })"
                    @click="updatePlacingMutation.mutate({ catId: cat.id, place: 3, breed: cat.breed })"
                    class="hover-bg-1 focus-ring px-3 py-2 rounded-2"
                  >
                    Kolmas
                  </li>
                  <li @click="removeSingleCat(cat.id)" tabindex="0" class="hover-bg-1 focus-ring px-3 py-2 rounded-2">
                    Peru osallistuminen
                  </li>
                </Dropdown>
              </template>
            </CatListItem>
          </div>
        </div>
      </div>
      <div class="d-flex flex-column gap-2">
        <button
          v-if="userHasPermission(PermissionTypes.CatShowWrite)"
          @click="() => open()"
          class="btn bg-black text-white rounded-3 px-5 py-2 me-auto focus-ring w-sm-100"
        >
          Lisää kuva +
        </button>
        <ImageGallery v-if="catShow && catShow.photos" :photos="catShow.photos" />
      </div>
    </div>
  </div>
  <Modal :visible="isCurrentAction(ActionTypes.JOINING_EVENT) && !isMobile" @onCancel="removeAction(ActionTypes.JOINING_EVENT)">
    <div style="width: 90vw; max-width: 500px" class="d-flex flex-column bg-white p-4 gap-4 rounded">
      <div v-if="userCats && userCats.length > 0">
        <h5>Osallistuvat kissat:</h5>
        <div v-for="(cat, index) in userCats" :key="index">
          <label>
            <input
              class="form-check-input focus-ring focus-ring-dark"
              @keyup.enter="toggleCheckbox(cat.id)"
              type="checkbox"
              v-model="selectedCatIds"
              :value="cat.id"
            />
            {{ cat.name }}
          </label>
        </div>
      </div>
      <div v-else>No cats available.</div>
      <div class="d-flex gap-2">
        <button data-testid="confirm-cat-delete" @click="leaveEvent" type="button" class="btn btn-light border w-100 rounded-3">
          Peru osallistuminen
        </button>
        <button @click="joinEventMutation.mutate" type="button" class="btn bg-black text-white w-100 rounded-3">Osallistu</button>
      </div>
    </div>
  </Modal>
  <Drawer
    :visible="isCurrentAction(ActionTypes.JOINING_EVENT_MOBILE) && isMobile"
    @onCancel="removeAction(ActionTypes.JOINING_EVENT_MOBILE)"
  >
    <div class="d-flex flex-column bg-white p-4 gap-4 rounded">
      <div v-if="userCats && userCats.length > 0">
        <h5>Osallistuvat kissat:</h5>
        <label @click="toggleAllCats">
          <input
            :checked="selectedCatIds.length === userCats.length && userCats.length > 0"
            class="form-check-input focus-ring focus-ring-dark"
            type="checkbox"
          />
          Valitse kaikki
        </label>
        <div v-for="(cat, index) in userCats" :key="index">
          <label>
            <input
              class="form-check-input focus-ring focus-ring-dark"
              @keyup.enter="toggleCheckbox(cat.id)"
              type="checkbox"
              v-model="selectedCatIds"
              :value="cat.id"
            />
            {{ cat.name }}
          </label>
        </div>
      </div>
      <div v-else>No cats available.</div>
      <div class="d-flex">
        <button @click="joinEventMutation.mutate" type="button" class="btn bg-black text-white w-100 rounded-3">Osallistu</button>
      </div>
    </div>
  </Drawer>
</template>

<style>
.hero-container {
  width: 100%;
  height: 300px;
}

.hero-image {
  height: 100%;
  width: 100%;
  max-width: 400px;
}

@media (max-width: 768px) {
  .hero-container {
    height: 500px;
  }
  .hero-image {
    width: 100%;
    max-width: none;
  }
}
</style>
