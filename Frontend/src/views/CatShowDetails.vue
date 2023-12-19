<script lang="ts" setup>
import { ref, computed } from "vue";
import userAPI from "../api/userAPI";
import { userStore } from "../store/userStore";
import { useRoute, useRouter } from "vue-router";
import { toast } from "vue-sonner";
import { useMutation, useQuery } from "@tanstack/vue-query";
import { useI18n } from "vue-i18n";
import Modal from "../components/Modal.vue";
import catShowAPI from "../api/catShowAPI";
//@ts-ignore doesnt have types
import FsLightbox from "fslightbox-vue/v3";

const route = useRoute();
const router = useRouter();
const { t } = useI18n();

const selectedCatIds = ref<number[]>([]);
const eventId = +route.params.eventId;

const navigateToCat = (catId: number) => router.push(`/cats/${catId}`);

const { data: catShow, refetch, isLoading } = useQuery({ queryKey: ["catshow"], queryFn: () => catShowAPI.getEventById(eventId) });

const { mutate: joinEvent } = useMutation({
  mutationFn: () => catShowAPI.joinEvent(eventId, selectedCatIds.value),
  onSuccess: () => {
    toast.info("Osallistuminen rekisteröity"), refetch();
  },
});

interface Payload {
  catId: number;
  place: number;
  breed: string;
}

const updatePlacingMutation = useMutation({
  mutationFn: (payload: Payload) => catShowAPI.assignCatPlacing(eventId, payload),
  onSuccess: () => {
    toast.info("Osallistuminen rekisteröity"), refetch();
  },
});

const uploadMutation = useMutation({
  mutationFn: (file: File) => {
    return catShowAPI.addCatShowPhoto(eventId, file);
  },
  onSuccess: refetch,
});

const { mutate: leaveEvent } = useMutation({
  mutationFn: () => catShowAPI.leaveEvent(eventId),
  onSuccess: () => {
    toast.info("Osallistuminen peruttu"), refetch();
  },
});

const { data: userCats } = useQuery({
  queryKey: ["userCats"],
  queryFn: () => userAPI.getCatsByUserId(userStore.user?.id as string),
});

const user = userStore.user;
const joiningEvent = ref(false);

const isUserAnAttendee = computed(() => catShow.value && catShow.value.attendees.some((attendee: any) => attendee.userId === user?.id));

const handleFileChange = async (event: Event) => {
  const input = event.target as HTMLInputElement;
  if (!input || !input.files) return;

  uploadMutation.mutate(input.files[0]);
};

const toggler = ref(false);
const selectedImage = ref(0);

const lightboxPhotos = computed(() => {
  if (catShow.value?.photos) {
    return catShow.value.photos.map((photo: any) => photo.url);
  }
  return [];
});

const catsGroupedByBreed = computed(() => {
  if (catShow.value?.attendees) {
    const groupedCats = catShow.value.attendees.flatMap((attendee: any) =>
      attendee.catAttendees.map((catAttendee: { cat: { breed: string } }) => catAttendee.cat)
    );

    const groupedCatsByBreed = groupedCats.reduce((acc: any, cat: any) => {
      if (!acc[cat.breed]) {
        acc[cat.breed] = [];
      }
      acc[cat.breed].push(cat);
      return acc;
    }, {});

    return groupedCatsByBreed;
  }
  return [];
});

const inputRef = ref();

const triggerFileInput = () => {
  console.log(inputRef.value);
  inputRef.value?.click();
};
</script>

<template>
  <div v-if="isLoading" class="spinner-border text-primary m-auto" role="status">
    <span class="visually-hidden">Loading...</span>
  </div>
  <div v-if="!isLoading" class="p-2 w-100 h-100 d-flex flex-column align-items-center p-sm-5">
    <div class="col-12 col-sm-8 p-sm-5 d-flex flex-column gap-5">
      <div class="d-flex gap-4" style="height: 300px">
        <div class="border image-container rounded-4" style="position: relative; min-width: 400px; overflow: hidden">
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
          <div class="mt-auto ms-auto">
            <button v-if="!isUserAnAttendee" type="button" class="btn btn-primary px-5" @click="joiningEvent = true">
              {{ t("CatShowDetails.joinEvent") }}
            </button>
            <button v-else @click="() => leaveEvent()" type="button" class="btn btn-danger px-5">
              {{ t("CatShowDetails.leaveEvent") }}
            </button>
          </div>
        </div>
      </div>

      <div class="attendees-list mt-3 d-flex flex-column gap-5">
        <div v-for="(cats, breed) in catsGroupedByBreed" :key="breed">
          <h4>{{ breed }}</h4>

          <div
            @click="() => navigateToCat(cat.id)"
            v-for="cat in cats"
            :key="cat.id"
            class="cat d-flex border-bottom gap-3 p-2 align-items-center"
          >
            <div class="d-flex align-items-center justify-content-start gap-2">
              <img
                :src="'https://kissarekisteritf.blob.core.windows.net/images/186f7fd4-ec2b-4f7a-950a-33b80a9e0d27.png'"
                class="rounded-circle"
                height="30"
                width="30"
                style="object-fit: fill"
              />
              <span class="text-upper-capitalize">
                {{ cat.name }}
              </span>
            </div>
            <div @click.stop class="dropdown d-flex ms-auto dropstart">
              <button class="btn ms-auto" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                <svg xmlns="http://www.w3.org/2000/svg" height="1em" viewBox="0 0 128 512">
                  <path
                    d="M64 360a56 56 0 1 0 0 112 56 56 0 1 0 0-112zm0-160a56 56 0 1 0 0 112 56 56 0 1 0 0-112zM120 96A56 56 0 1 0 8 96a56 56 0 1 0 112 0z"
                  />
                </svg>
              </button>
              <ul class="dropdown-menu">
                <li
                  @click="
                    updatePlacingMutation.mutate({
                      catId: cat.id,
                      place: 1,
                      breed: cat.breed,
                    })
                  "
                  class="dropdown-item"
                >
                  Ensimmäinen
                </li>
                <li
                  @click="
                    updatePlacingMutation.mutate({
                      catId: cat.id,
                      place: 2,
                      breed: cat.breed,
                    })
                  "
                  class="dropdown-item"
                >
                  Toinen
                </li>
                <li
                  @click="
                    updatePlacingMutation.mutate({
                      catId: cat.id,
                      place: 3,
                      breed: cat.breed,
                    })
                  "
                  class="dropdown-item"
                >
                  Kolmas
                </li>
              </ul>
            </div>
          </div>
        </div>
      </div>
      <div v-if="catShow" class="w-100 m-auto d-flex flex-column">
        <div v-if="catShow" class="image-gallery gap-2">
          <div
            @click="(selectedImage = index), (toggler = !toggler)"
            v-for="(catShowImage, index) in catShow.photos"
            :key="catShowImage.id"
            class="border image-container rounded-4"
            style="position: relative; width: 100%; overflow: hidden"
          >
            <div style="width: 100%; padding-top: 100%; position: relative"></div>
            <img
              :src="catShowImage.url"
              alt="Cat image"
              class="image thumbnail"
              style="position: absolute; top: 0; left: 0; width: 100%; height: 100%; object-fit: cover"
            />
          </div>
        </div>
      </div>
      <button @click="triggerFileInput" class="btn border rounded-3 px-5 py-2 btn-border me-auto">
        <input class="d-none" ref="inputRef" type="file" @change="handleFileChange" id="catImageInput" />
        Lisää kuva +
      </button>
    </div>
    <FsLightbox :key="selectedImage" :toggler="toggler" :sources="lightboxPhotos" :slide="selectedImage + 1" />
    <Modal :modalId="'join-event-modal'" :visible="joiningEvent" @onCancel="joiningEvent = false">
      <div class="d-flex flex-column bg-white w-100 p-4 gap-4 rounded">
        <div v-if="user && userCats && userCats.length > 0">
          <h5>Osallistuvat kissat:</h5>
          <div v-for="(cat, index) in userCats" :key="index">
            <label>
              <input type="checkbox" v-model="selectedCatIds" :value="cat.id" />
              {{ cat.name }}
            </label>
          </div>
        </div>
        <div v-else>No cats available.</div>
        <button @click="() => joinEvent()" type="button" class="btn btn-primary" data-bs-dismiss="modal">Osallistu</button>
      </div>
    </Modal>
  </div>
</template>

<style>
.image-gallery {
  display: grid;
  grid-template-columns: repeat(5, 1fr);
}

@media screen and (max-width: 768px) {
  .image-gallery {
    grid-template-columns: repeat(3, 1fr);
  }
}

.btn-border {
  border: 1px solid #000;
}
.btn-border:hover {
  background-color: #f8f9fa;
}
.thumbnail:hover {
  cursor: pointer;
  opacity: 0.8;
}
</style>
