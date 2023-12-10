<script setup lang="ts">
import { ref, onMounted, computed } from "vue";
import userAPI from "../api/userAPI";
import { useRouter } from "vue-router";
import Modal from "./Modal.vue";

const router = useRouter();

const events = ref<CatShowEvent[]>([]);
const searchQuery = ref("");
const newEvent = ref({
  name: "",
  description: "",
  location: "",
  startDate: "",
  endDate: "",
});

const addingEvent = ref(false);

const setAddingEvent = (bool: boolean) => (addingEvent.value = bool);

const filteredEvents = computed(() => {
  if (!searchQuery.value) {
    return events.value;
  }
  return events.value.filter(
    (event) =>
      event.name.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      event.description.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      event.location.toLowerCase().includes(searchQuery.value.toLowerCase())
  );
});

const navigateToEvent = (eventId: number) => router.push(`/catshows/${eventId}`);

const createCatShow = async () => {
  await userAPI.createCatShowEvent(newEvent.value);
  await loadEvents();
};

const loadEvents = async () => {
  const response = await userAPI.getEvents();
  if (!response) return;
  events.value = response;
};

const formatDate = (dateString: string) =>
  new Intl.DateTimeFormat("fi-FI", {
    year: "numeric",
    month: "2-digit",
    day: "2-digit",
    hour: "2-digit",
    minute: "2-digit",
    second: "2-digit",
    timeZone: "Europe/Helsinki", // Specify the time zone if needed
  }).format(new Date(dateString));

onMounted(async () => {
  await loadEvents();
});
</script>
<template>
  <div class="w-100 h-100 p-5 d-flex flex-column align-items-center justify-content-center">
    <div class="p-5 border rounded w-75 overflow-auto" style="height: 700px">
      <h3>Näyttelyt</h3>
      <div class="d-flex gap-4 py-3 sticky-top bg-white align-items-center">
        <div class="col-12 col-md-8 col-xxl-4">
          <input type="text" class="form-control" v-model="searchQuery" placeholder="Etsi näyttelyistä..." />
        </div>
        <div class="col d-flex">
          <button @click="() => setAddingEvent(true)" type="button" class="btn btn-primary ms-auto">Lisää näyttely</button>
        </div>
      </div>

      <div class="d-flex flex-column overflow-auto">
        <div
          @click="() => navigateToEvent(event.id!)"
          v-for="event in filteredEvents"
          :key="event.id"
          class="d-flex gap-4 border-bottom p-3 align-items-center pointer cat-show justify-content-between"
        >
          <div>
            <div>{{ event.name }}</div>
            <span class="text-body-secondary">{{ event.location }}</span>
          </div>

          <div>{{ `${formatDate(event.startDate)} -  ${formatDate(event.endDate)}` }}</div>
        </div>
      </div>
    </div>
  </div>
  <Modal :modalId="'event-modal'" @onCancel="() => setAddingEvent(false)" :visible="addingEvent">
    <div class="modal-body d-flex flex-column w-100">
      <div class="input-group mb-3">
        <input type="text" class="form-control" v-model="newEvent.name" placeholder="Event Name" />
      </div>
      <div class="input-group mb-3">
        <input type="text" class="form-control" v-model="newEvent.description" placeholder="Event Description" />
      </div>
      <div class="input-group mb-3">
        <input type="text" class="form-control" v-model="newEvent.location" placeholder="Event Location" />
      </div>
      <div class="row mb-3 g-2">
        <div class="col-8">
          <input type="date" class="form-control" v-model="newEvent.startDate" placeholder="Start Date" />
        </div>
        <div class="col-4">
          <input type="time" class="form-control" v-model="newEvent.endDate" placeholder="End Date" />
        </div>
      </div>

      <div class="input-group mb-3">
        <input type="date" class="form-control" v-model="newEvent.endDate" placeholder="End Date" />
        <input type="time" class="form-control" v-model="newEvent.endDate" placeholder="End Date" />
      </div>
      <button @click="createCatShow" type="button" class="btn btn-primary ms-auto">Luo tapahtuma</button>
    </div>
  </Modal>
</template>

<style>
.cat-show:hover {
  cursor: pointer;
  background-color: #f3f4f6;
}
</style>
