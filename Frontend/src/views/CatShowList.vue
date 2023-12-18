<script setup lang="ts">
import { ref, onMounted, computed } from "vue";
import { useRouter } from "vue-router";
import Modal from "../components/Modal.vue";
import { useI18n } from "vue-i18n";
import { formatDate } from "../utils/formatDate";
import catShowAPI from "../api/catShowAPI";
import { userStore } from "../store/userStore";

const router = useRouter();
const { t } = useI18n();

const events = ref<CatShowEvent[]>([]);
const searchQuery = ref("");
const newEvent = ref({
  name: "",
  description: "",
  location: "",
  startDate: "",
  endDate: "",
});

const user = computed(() => userStore.user);

const addingEvent = ref(false);

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
  await catShowAPI.createCatShowEvent(newEvent.value);
  await loadEvents();
  addingEvent.value = false;
};

const loadEvents = async () => {
  const response = await catShowAPI.getEvents();
  if (!response) return;
  events.value = response;
};

onMounted(async () => await loadEvents());
</script>
<template>
  <div class="w-100 h-100 p-0 p-sm-5 d-flex flex-column align-items-center justify-content-center">
    <div class="p-4 p-sm-5 rounded overflow-auto col-12 col-lg-8">
      <h3>{{ t("CatShowList.catShows") }}</h3>
      <div class="d-flex gap-4 py-3 sticky-top bg-white align-items-center">
        <div class="col-12 col-md-8 col-xxl-4">
          <input type="text" class="form-control" v-model="searchQuery" :placeholder="t('CatShowList.searchInput')" />
        </div>
        <div class="col d-flex">
          <button
            v-if="user?.permissions.some((p) => p.name === 'CreateEvent')"
            @click="addingEvent = true"
            type="button"
            class="btn btn-primary ms-auto"
          >
            {{ t("CatShowList.addCatShow") }}
          </button>
        </div>
      </div>
      <div class="d-flex flex-column overflow-auto">
        <div
          @click="() => navigateToEvent(event.id!)"
          v-for="event in filteredEvents"
          :key="event.id"
          class="d-flex gap-4 border-bottom px-3 py-2 align-items-center pointer cat-show justify-content-between"
        >
          <div>
            <div>{{ event.name }}</div>
            <span class="text-body-secondary">{{ event.location }}</span>
          </div>
          <div style="font-size: 12px; font-weight: bold; margin-top: auto">
            {{ `${formatDate(event.startDate)} -  ${formatDate(event.endDate)}` }}
          </div>
        </div>
      </div>
    </div>
  </div>
  <Modal :modalId="'event-modal'" @onCancel="addingEvent = false" :visible="addingEvent">
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
