<script lang="ts" setup>
import { ref, computed } from "vue";
import catAPI from "../api/catAPI";
import { userStore } from "../store/userStore";
import { useRoute } from "vue-router";
import { toast } from "vue-sonner";
import { useMutation, useQuery } from "@tanstack/vue-query";
import { useI18n } from "vue-i18n";
import Modal from "../components/Modal.vue";
import catShowAPI from "../api/catShowAPI";

const route = useRoute();
const { t } = useI18n();

const selectedCatIds = ref<number[]>([]);
const eventId = +route.params.eventId;

const { data: catShow, refetch } = useQuery({ queryKey: ["catshow"], queryFn: () => catShowAPI.getEventById(eventId) });

const { mutate: joinEvent } = useMutation({
  mutationFn: () => catShowAPI.joinEvent(eventId, selectedCatIds.value),
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
  queryFn: catAPI.getCats,
});

const user = userStore.user;
const joiningEvent = ref(false);

const isUserAnAttendee = computed(() => catShow.value && catShow.value.attendees.some((attendee: any) => attendee.userId === user?.id));

const handleFileChange = async (event: Event) => {
  const input = event.target as HTMLInputElement;
  if (!input || !input.files) return;

  uploadMutation.mutate(input.files[0]);
};
</script>

<template>
  <div class="p-2 w-100 h-100 d-flex flex-column justify-content-center align-items-center p-5">
    <div class="w-50">
      <div v-if="catShow" class="d-flex rounded overflow-hidden border">
        <div class="bg-primary" style="width: 300px; height: 300px" />
        <div class="card-body d-flex flex-column p-4">
          <h5 class="card-title">{{ catShow.name }}</h5>
          <p class="card-text">
            {{ catShow.description }}
          </p>
          <p class="card-text mt-auto d-flex align-items-center justify-content-between">
            <span class="badge rounded-pill text-bg-secondary">{{ catShow.location }}</span>
            <button v-if="!isUserAnAttendee" type="button" class="btn btn-primary" @click="joiningEvent = true">
              {{ t("CatShowDetails.joinEvent") }}
            </button>
            <button v-else @click="() => leaveEvent()" type="button" class="btn btn-danger">{{ t("CatShowDetails.leaveEvent") }}</button>
          </p>
        </div>
      </div>

      <div v-if="catShow && catShow.attendees && catShow.attendees.length > 0" class="attendees-list mt-3">
        <h4>Osallistujat</h4>
        <div v-for="attendee in catShow.attendeeDetails" :key="attendee.id" class="mb-2">
          <div class="fw-bold">{{ `${attendee.givenName}  ${attendee.surname}` }}</div>
          <ul v-if="attendee.cats && attendee.cats.length">
            <li v-for="cat in attendee.cats" :key="cat.id">{{ cat.name }} ({{ cat.breed }})</li>
          </ul>
        </div>
      </div>
    </div>
    <button class="btn btn-primary me-auto"><input type="file" @change="handleFileChange" id="catImageInput" /></button>

    <div v-if="catShow" class="gap-2" style="display: grid; grid-template-columns: 1fr 1fr 1fr">
      <div style="position: relative" class="d-flex flex-column border" v-for="catShowImage in catShow.photos" :key="catShowImage.url">
        <img :src="catShowImage.url" class="cat-thumbnail w-100" alt="Cat image" />
        <button class="btn btn-primary position-absolute bottom-0 m-2 z-1">Aseta profiilikuvaksi</button>
      </div>
    </div>
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
