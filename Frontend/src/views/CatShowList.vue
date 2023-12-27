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

//@ts-ignore
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
  <div class="w-100 h-100 p-0 p-sm-5 d-flex flex-column align-items-center">
    <div class="p-4 p-sm-5 rounded col-12 col-lg-8 d-flex flex-column overflow-auto">
      <h3>{{ t("CatShowList.catShows") }}</h3>
      <div class="d-flex gap-4 py-3 sticky-top bg-white align-items-center">
        <div class="col-12 col-md-8 col-xxl-4">
          <input type="text" class="form-control" v-model="searchQuery" :placeholder="t('CatShowList.searchInput')" />
        </div>
      </div>
      <div class="d-flex flex-column overflow-auto" style="height: 500px">
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
      <button @click="addingEvent = true" type="button" class="btn btn-primary ms-auto px-5 mt-2">
        {{ t("CatShowList.addCatShow") }} +
      </button>
    </div>
  </div>
  <Modal :modalId="'event-modal'" @onCancel="addingEvent = false" :visible="addingEvent">
    <div class="d-flex flex-column w-100 p-4 gap-4">
      <div>
        <label for="event-name" class="form-label cursor-pointer">{{ t("CatShowList.eventNameInput") }}</label>
        <input id="event-name" type="text" class="form-control" v-model="newEvent.name" :placeholder="t('CatShowList.eventNameInput')" />
      </div>

      <div>
        <label for="event-description" class="form-label cursor-pointer">{{ t("CatShowList.eventDescriptionInput") }}</label>
        <input
          id="event-description"
          type="text"
          class="form-control"
          v-model="newEvent.description"
          :placeholder="t('CatShowList.eventDescriptionInput')"
        />
      </div>

      <div>
        <label for="event-location" class="form-label cursor-pointer">{{ t("CatShowList.eventLocationInput") }}</label>
        <input
          id="event-location"
          type="text"
          class="form-control"
          v-model="newEvent.location"
          :placeholder="t('CatShowList.eventLocationInput')"
        />
      </div>

      <div>
        <label for="start-date" class="form-label cursor-pointer">Start Date</label>
        <input id="start-date" type="date" class="form-control" v-model="newEvent.startDate" />
      </div>

      <div>
        <label for="end-date" class="form-label cursor-pointer">End Date</label>
        <input id="end-date" type="date" class="form-control" v-model="newEvent.endDate" />
      </div>

      <button @click="createCatShow" type="button" class="btn btn-primary ms-auto px-5">Luo tapahtuma +</button>
    </div>
  </Modal>
</template>

<style>
.cat-show:hover {
  cursor: pointer;
  background-color: #f3f4f6;
}
</style>
