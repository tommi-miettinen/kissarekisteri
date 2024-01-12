<script lang="ts" setup>
import { ref, computed, watch } from "vue";
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
import { isCurrentAction, removeAction, pushAction } from "../store/actionStore";
import ThreeDotsIcon from "../icons/ThreeDotsIcon.vue";
import Drawer from "../components/Drawer.vue";
import { isMobile } from "../store/actionStore";
import moment from "moment";

enum ActionType {
  JOINING_EVENT = "JOINING_EVENT",
  JOINING_EVENT_MOBILE = "JOINING_EVENT_MOBILE",
  LEAVING_EVENT = "LEAVING_EVENT",
  SELECTING_CAT_ACTION = "SELECTING_CAT_ACTION",
  SELECTING_CAT_ACTION_MOBILE = "SELECTING_CAT_ACTION_MOBILE",
}

const route = useRoute();
const { t } = useI18n();

const selectedCatIds = ref<number[]>([]);
const eventId = +route.params.eventId;

const { data: catShow, refetch, isLoading, isError } = useQuery({ queryKey: ["catshow"], queryFn: () => catShowAPI.getEventById(eventId) });

const joinEventMutation = useMutation({
  mutationFn: () => catShowAPI.joinEvent(eventId, selectedCatIds.value),
  onSuccess: () => {
    toast.info("Osallistuminen rekisteröity");
    refetch();
    removeAction(ActionType.JOINING_EVENT);
    removeAction(ActionType.JOINING_EVENT_MOBILE);
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
  mutationFn: (file: File) => {
    return catShowAPI.addCatShowPhoto(eventId, file);
  },
  onSuccess: () => refetch(),
});

const leaveEventMutation = useMutation({
  mutationFn: () => catShowAPI.leaveEvent(eventId),
  onSuccess: () => {
    toast.info("Osallistuminen peruttu");
    refetch();
    removeAction(ActionType.LEAVING_EVENT);
  },
});

const { data: userCatsData, refetch: refetchUserCats } = useQuery({
  queryKey: ["userCats"],
  queryFn: () => userAPI.getCatsByUserId(user.value?.id as string),
  enabled: Boolean(user.value),
});

const userCats = computed(() => userCatsData.value?.data);

watch(user, () => refetchUserCats());

const leavingEvent = ref(false);

const isUserAnAttendee = computed(() => catShow.value && catShow.value.cats.some((cat) => cat.cat.ownerId === user.value?.id));

const handleFileChange = async (event: Event) => {
  const input = event.target as HTMLInputElement;
  if (!input || !input.files) return;

  uploadMutation.mutate(input.files[0]);
};

const lightboxPhotos = computed(() => {
  if (catShow.value?.photos) {
    return [
      "https://kissarekisteritf.blob.core.windows.net/images/a2174d16-0f1e-452f-b1a8-2c2d58600d05.jpg",
      ...catShow.value.photos.map((photo) => photo.url),
    ];
  }
  return [];
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

const inputRef = ref();
const triggerFileInput = () => inputRef.value?.click();

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

const startJoiningCatShow = () => (isMobile.value ? pushAction(ActionType.JOINING_EVENT_MOBILE) : pushAction(ActionType.JOINING_EVENT));

const formatDate = (start: string, end: string) => {
  const startDate = moment(start).format("ll");
  const endDate = moment(end).format("ll");
  const startTime = moment(start).format("LT");
  const endTime = moment(end).format("LT");
  return `${startDate} - ${endDate}, ${startTime} - ${endTime}`;
};

const removeSingleCat = (catId: number) => {
  const userCats = catShow.value?.cats.filter((c) => c.cat.ownerId === user.value?.id && c.cat.id !== catId);

  selectedCatIds.value = userCats?.map((c) => c.cat.id) || [];
  joinEventMutation.mutate();
};
</script>

<template>
  <h3 v-if="isError" class="m-5 fw-bold">{{ t("CatShowDetails.404") }}</h3>
  <div v-if="isLoading" class="spinner-border text-black m-auto" role="status">
    <span class="visually-hidden">Loading...</span>
  </div>
  <div
    v-if="catShow"
    class="p-2 w-100 h-100 d-flex flex-column align-items-center p-xl-5 col-12 col-xxl-8 p-sm-5 d-flex flex-column gap-sm-5"
  >
    <div class="col-12 col-xxl-8 flex-grow-1 p-sm-5 d-flex flex-column gap-2 p-2 gap-sm-5">
      <div class="d-flex flex-column flex-md-row gap-sm-4 gap-2 hero-container">
        <div class="border rounded-4 hero-image" style="position: relative; overflow: hidden">
          <img
            style="position: absolute; top: 0; left: 0; width: 100%; height: 100%; object-fit: cover"
            src="https://kissarekisteritf.blob.core.windows.net/images/a2174d16-0f1e-452f-b1a8-2c2d58600d05.jpg"
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
              class="btn bg-black text-white rounded-3 py-2 px-5 w-sm-100"
              @click="startJoiningCatShow"
            >
              {{ t("CatShowDetails.joinEvent") }}
            </button>
            <button v-else @click="startJoiningCatShow" type="button" class="w-sm-100 btn bg-black text-white rounded-3 py-2 px-5">
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
                <div
                  v-if="userHasPermission('CreateCatShowResult') || cat.ownerId === user?.id"
                  :ref="el => (dropdownRefs[cat.id] = el as HTMLDivElement)"
                  :id="cat.id.toString()"
                  tabindex="0"
                  @click.stop
                  class="btn py-1 px-2 accordion d-flex focus-ring rounded-1 border-0"
                >
                  <ThreeDotsIcon />
                </div>
                <Dropdown :placement="'left-start'" :triggerRef="dropdownRefs[cat.id]">
                  <li
                    v-if="userHasPermission('CreateCatShowResult')"
                    tabindex="0"
                    @keyup.enter="updatePlacingMutation.mutate({ catId: cat.id, place: 1, breed: cat.breed })"
                    @click="updatePlacingMutation.mutate({ catId: cat.id, place: 1, breed: cat.breed })"
                    class="hover-bg-1 focus-ring px-3 py-2 rounded-2"
                  >
                    Ensimmäinen
                  </li>
                  <li
                    v-if="userHasPermission('CreateCatShowResult')"
                    tabindex="0"
                    @keyup.enter="updatePlacingMutation.mutate({ catId: cat.id, place: 2, breed: cat.breed })"
                    @click="updatePlacingMutation.mutate({ catId: cat.id, place: 2, breed: cat.breed })"
                    class="hover-bg-1 focus-ring px-3 py-2 rounded-2"
                  >
                    Toinen
                  </li>
                  <li
                    v-if="userHasPermission('CreateCatShowResult')"
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
        <button @click="triggerFileInput" class="btn bg-black text-white rounded-3 px-5 py-2 me-auto focus-ring w-sm-100">
          <input class="d-none" ref="inputRef" type="file" @change="handleFileChange" id="catImageInput" />
          Lisää kuva +
        </button>
        <ImageGallery v-if="catShow" :photos="lightboxPhotos" />
      </div>
    </div>
    <Modal :visible="isCurrentAction(ActionType.JOINING_EVENT) && !isMobile" @onCancel="removeAction(ActionType.JOINING_EVENT)">
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
      :visible="isCurrentAction(ActionType.JOINING_EVENT_MOBILE) && isMobile"
      @onCancel="removeAction(ActionType.JOINING_EVENT_MOBILE)"
    >
      <div class="d-flex flex-column bg-white p-4 gap-4 rounded">
        <div v-if="userCats && userCats.length > 0">
          <h5>Osallistuvat kissat:</h5>
          <div v-for="(cat, index) in userCats" :key="index">
            <label>
              <input
                checked
                class="bg-black"
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
          <button @click="joinEventMutation.mutate" type="button" class="btn btn-primary w-100">Osallistu</button>
          <button data-testid="confirm-cat-delete" @click="leaveEvent" type="button" class="btn bg-black text-white">
            Peru osallistuminen
          </button>
        </div>
      </div>
    </Drawer>
  </div>
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
    height: 400px;
  }
  .hero-image {
    width: 100%;
    max-width: none;
  }
}
</style>
