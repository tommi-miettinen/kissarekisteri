<script lang="ts" setup>
import { ref, computed } from "vue";
import userAPI from "../api/userAPI";
import catAPI from "../api/catAPI";
import { userStore } from "../store/userStore";
import { useRoute } from "vue-router";
import { toast } from "vue-sonner";
import { useQuery } from "@tanstack/vue-query";
import Modal from "../components/Modal.vue";

const route = useRoute();

const eventId = +route.params.eventId;
const { data: catshow, refetch } = useQuery({ queryKey: ["catshow"], queryFn: () => userAPI.getEventById(eventId) });
const { data: userCats } = useQuery({
  queryKey: ["userCats"],
  queryFn: catAPI.getCats,
});
const user = ref(userStore((state) => state.user));

const selectedCatIds = ref<number[]>([]);
const joiningEvent = ref<boolean>(false);

const isUserAnAttendee = computed(
  () => catshow.value && catshow.value.attendees.some((attendee: any) => attendee.userId === user.value.id)
);

const leaveEvent = async () => {
  await userAPI.leaveEvent(eventId);
  await refetch();
  toast.info("Osallistuminen peruttu");
};

const joinEvent = async () => {
  const joined = await userAPI.joinEvent(eventId, selectedCatIds.value);
  if (!joined) return;

  await refetch();
  toast.info("Osallistuminen rekisteröity");
};
</script>

<template>
  <div class="p-2 w-100 h-100 d-flex flex-column justify-content-center align-items-center p-5">
    <div class="w-50">
      <div v-if="catshow" class="card">
        <div class="row g-0">
          <div class="col-md-4">
            <img src="https://placekitten.com/300/300" class="img-fluid rounded-start" alt="..." />
          </div>
          <div class="col-md-8 d-flex flex-column">
            <div class="card-body d-flex flex-column">
              <h5 class="card-title">{{ catshow.name }}</h5>
              <p class="card-text">
                {{ catshow.description }}
              </p>
              <p class="card-text mt-auto d-flex align-items-center justify-content-between">
                <span class="badge rounded-pill text-bg-secondary">{{ catshow.location }}</span>
                <button v-if="!isUserAnAttendee" type="button" class="btn btn-primary" @click="joiningEvent = true">
                  Osallistu tapahtumaan
                </button>
                <button v-else @click="leaveEvent" type="button" class="btn btn-danger">Poistu tapahtumasta</button>
              </p>
            </div>
          </div>
        </div>
      </div>

      <div v-if="catshow && catshow.attendees && catshow.attendees.length > 0" class="attendees-list mt-3">
        <h4>Osallistujat</h4>
        <div v-for="attendee in catshow.attendeeDetails" :key="attendee.id" class="mb-2">
          <div class="fw-bold">{{ `${attendee.givenName}  ${attendee.surname}` }}</div>
          <ul v-if="attendee.cats && attendee.cats.length">
            <li v-for="cat in attendee.cats" :key="cat.id">{{ cat.name }} ({{ cat.breed }})</li>
          </ul>
        </div>
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
