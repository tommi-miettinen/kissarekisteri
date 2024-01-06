<script lang="ts" setup>
import { ref, computed, watch } from "vue";
import userAPI from "../api/userAPI";
import { userHasPermission, user } from "../store/userStore";
import { useRoute } from "vue-router";
import { toast } from "vue-sonner";
import { useMutation, useQuery } from "@tanstack/vue-query";
import { useI18n } from "vue-i18n";
import Modal from "../components/Modal.vue";
import catShowAPI from "../api/catShowAPI";
//@ts-ignore doesnt have types
import FsLightbox from "fslightbox-vue/v3";
import CatListItem from "../components/CatListItem.vue";
import getMedalColor from "../utils/getMedalColor";
import ImageGallery from "../components/ImageGallery.vue";
import Dropdown from "../components/Dropdown.vue";

const route = useRoute();
const { t } = useI18n();

const selectedCatIds = ref<number[]>([]);
const eventId = +route.params.eventId;

const { data: catShow, refetch, isLoading, isError } = useQuery({ queryKey: ["catshow"], queryFn: () => catShowAPI.getEventById(eventId) });

const joinEventMutation = useMutation({
  mutationFn: () => catShowAPI.joinEvent(eventId, selectedCatIds.value),
  onSuccess: () => {
    toast.info("Osallistuminen rekisteröity"), refetch();
  },
});

const updatePlacingMutation = useMutation({
  mutationFn: (payload: CatShowResultPayload) => catShowAPI.assignCatPlacing(eventId, payload),
  onSuccess: () => {
    toast.info("Näyttelytulos tallennettu"), refetch();
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
    toast.info("Osallistuminen peruttu"), refetch();
  },
});

const { data: userCatsData, refetch: refetchUserCats } = useQuery({
  queryKey: ["userCats"],
  queryFn: () => userAPI.getCatsByUserId(user.value?.id as string),
  enabled: Boolean(user.value),
});

const userCats = computed(() => userCatsData.value?.data);

watch(user, () => refetchUserCats());

const joiningEvent = ref(false);
const leavingEvent = ref(false);

const isUserAnAttendee = computed(() => catShow.value && catShow.value.attendees?.some((attendee) => attendee.userId === user.value?.id));

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
  if (catShow.value?.attendees) {
    const groupedCats = catShow.value.attendees.flatMap((attendee) => attendee.catAttendees.map((catAttendee) => catAttendee.cat));

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

const joinEvent = () => {
  joinEventMutation.mutate();
  joiningEvent.value = false;
};

const leaveEvent = () => {
  leaveEventMutation.mutate();
  leavingEvent.value = false;
};

const toggleCheckbox = (catId: number) => {
  const catExists = selectedCatIds.value.includes(catId);

  if (catExists) {
    selectedCatIds.value = selectedCatIds.value.filter((id) => id !== catId);
  } else {
    selectedCatIds.value = [...selectedCatIds.value, catId];
  }
};

const handleDropdownItemClick = (result: CatShowResultPayload) => {
  updatePlacingMutation.mutate(result);
};

const dropdownRefs = ref<Record<string, HTMLDivElement>>({});
</script>

<template>
  <h3 v-if="isError" class="m-5 fw-bold">{{ t("CatShowDetails.404") }}</h3>
  <div v-if="isLoading" class="spinner-border text-primary m-auto" role="status">
    <span class="visually-hidden">Loading...</span>
  </div>
  <div v-if="catShow" class="p-2 w-100 h-100 d-flex flex-column align-items-center p-xl-5">
    <div class="col-12 col-xxl-8 p-sm-5 d-flex flex-column gap-sm-5">
      <div class="d-flex flex-column flex-md-row gap-sm-4 hero-container">
        <div class="border image-container rounded-4 hero-image" style="position: relative; width: 100%; height: 100%; overflow: hidden">
          <img
            style="position: absolute; top: 0; left: 0; width: 100%; height: 100%; object-fit: cover"
            src="https://kissarekisteritf.blob.core.windows.net/images/a2174d16-0f1e-452f-b1a8-2c2d58600d05.jpg"
          />
        </div>
        <div class="d-flex flex-column p-2" style="width: 100%">
          <h3>{{ catShow.name }}</h3>
          <p>
            {{ catShow.description }}
          </p>
          <span>{{ catShow.location }}</span>
          <div v-if="user" class="mt-auto ms-auto w-sm-100">
            <button v-if="!isUserAnAttendee" type="button" class="btn btn-primary px-5 w-sm-100" @click="joiningEvent = true">
              {{ t("CatShowDetails.joinEvent") }}
            </button>
            <button v-else @click="leavingEvent = true" type="button" class="w-sm-100 btn btn-danger rounded-3 py-2 px-5">
              {{ t("CatShowDetails.leaveEvent") }}
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
              <template v-if="userHasPermission('CreateCatShowResult')" #actions>
                <div>
                  <div
                    :ref="el => (dropdownRefs[cat.id] = el as HTMLDivElement)"
                    :id="cat.id.toString()"
                    @click.stop
                    tabindex="0"
                    class="btn py-1 px-2 accordion d-flex focus-ring rounded-1"
                  >
                    <svg xmlns="http://www.w3.org/2000/svg" height="1em" viewBox="0 0 128 512">
                      <path
                        d="M64 360a56 56 0 1 0 0 112 56 56 0 1 0 0-112zm0-160a56 56 0 1 0 0 112 56 56 0 1 0 0-112zM120 96A56 56 0 1 0 8 96a56 56 0 1 0 112 0z"
                      />
                    </svg>
                  </div>
                  <Dropdown :placement="'left-start'" :triggerRef="dropdownRefs[cat.id]">
                    <li
                      tabindex="0"
                      @keyup.enter="handleDropdownItemClick({ catId: cat.id, place: 1, breed: cat.breed })"
                      @click.stop="handleDropdownItemClick({ catId: cat.id, place: 1, breed: cat.breed })"
                      class="dropdown-item focus-ring px-3 py-2 rounded-2 hover-bg"
                    >
                      Ensimmäinen
                    </li>
                    <li
                      tabindex="0"
                      @keyup.enter="handleDropdownItemClick({ catId: cat.id, place: 2, breed: cat.breed })"
                      @click.stop="handleDropdownItemClick({ catId: cat.id, place: 2, breed: cat.breed })"
                      class="dropdown-item focus-ring px-3 py-2 rounded-2 hover-bg"
                    >
                      Toinen
                    </li>
                    <li
                      tabindex="0"
                      @keyup.enter="handleDropdownItemClick({ catId: cat.id, place: 3, breed: cat.breed })"
                      @click.stop="handleDropdownItemClick({ catId: cat.id, place: 3, breed: cat.breed })"
                      class="dropdown-item focus-ring px-3 py-2 rounded-2 hover-bg"
                    >
                      Kolmas
                    </li>
                  </Dropdown>
                </div>
              </template>
            </CatListItem>
          </div>
        </div>
      </div>
      <ImageGallery v-if="catShow" :photos="lightboxPhotos">
        <template v-if="userHasPermission('CreateCatShowResult')" #upload>
          <button @click="triggerFileInput" class="btn border rounded-3 px-5 py-2 btn-border me-auto focus-ring">
            <input class="d-none" ref="inputRef" type="file" @change="handleFileChange" id="catImageInput" />
            Lisää kuva +
          </button>
        </template>
      </ImageGallery>
    </div>
    <Modal :visible="joiningEvent" @onCancel="joiningEvent = false">
      <div style="width: 90vw; max-width: 500px" class="d-flex flex-column bg-white p-4 gap-4 rounded">
        <div v-if="userCats && userCats.length > 0">
          <h5>Osallistuvat kissat:</h5>
          <div v-for="(cat, index) in userCats" :key="index">
            <label>
              <input @keyup.enter="toggleCheckbox(cat.id)" type="checkbox" v-model="selectedCatIds" :value="cat.id" />
              {{ cat.name }}
            </label>
          </div>
        </div>
        <div v-else>No cats available.</div>
        <button @click="joinEvent" type="button" class="btn btn-primary">Osallistu</button>
      </div>
    </Modal>
    <Modal :visible="leavingEvent" @onCancel="leavingEvent = false">
      <div style="width: 90vw; max-width: 500px" class="p-4 d-flex flex-column">
        <p>Perutaanko osallistuminen?</p>
        <div class="d-flex gap-2 justify-content-end">
          <button @click="leavingEvent = false" type="button" class="btn btn-secondary">Peruuta</button>
          <button data-testid="confirm-cat-delete" @click="leaveEvent" type="button" class="btn btn-danger">Peru osallistuminen</button>
        </div>
      </div>
    </Modal>
  </div>
</template>

<style>
.hero-container {
  min-height: 400px;
}

.hero-image {
  max-width: 100%;
}

.w-sm-100 {
  width: 100%;
}

@media (min-width: 768px) {
  .hero-container {
    min-height: 300px;
  }
  .hero-image {
    max-width: 400px;
  }

  .w-sm-100 {
    width: auto;
  }
}
</style>
