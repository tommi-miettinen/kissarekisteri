<script setup lang="ts">
import { ref } from "vue";
import { useRouter } from "vue-router";
import Modal from "../components/Modal.vue";
import { useI18n } from "vue-i18n";
import { formatDate } from "../utils/formatDate";
import catShowAPI from "../api/catShowAPI";
import { useQuery, useMutation } from "@tanstack/vue-query";
import { toast } from "vue-sonner";
import { userHasPermission } from "../store/userStore";
import { getCurrentFormattedDate } from "../utils/formatDate";
import List from "../components/List.vue";

const router = useRouter();
const { t } = useI18n();

const newEvent = ref({
  name: "",
  description: "",
  location: "",
  startDate: getCurrentFormattedDate(),
  endDate: getCurrentFormattedDate(),
});

const addingEvent = ref(false);

const { data: catShows, refetch: refetchCatShows } = useQuery({
  queryKey: ["catshows"],
  queryFn: () => catShowAPI.getEvents(),
});

const createCatShowMutation = useMutation({
  mutationFn: () => catShowAPI.createCatShowEvent(newEvent.value),
  onSuccess: () => {
    toast.info("Tapahtuma luotu"), refetchCatShows();
  },
});

const navigateToEvent = (eventId: number) => router.push(`/catshows/${eventId}`);
</script>
<template>
  <div class="w-100 h-100 p-0 p-sm-5 d-flex flex-column align-items-center">
    <div class="p-2 p-sm-5 rounded col-12 col-lg-8 h-100 d-flex flex-column">
      <h3>{{ t("CatShowList.catShows") }}</h3>

      <List :searchQueryPlaceholder="t('CatShowList.searchInput')" v-if="catShows" :items="catShows" :itemsPerPage="7">
        <template v-slot="{ item: catShow }">
          <div
            tabindex="0"
            @keyup.enter="() => navigateToEvent(catShow.id!)"
            @click="() => navigateToEvent(catShow.id!)"
            class="d-flex gap-4 border-bottom px-3 py-2 align-items-center pointer hover-bg justify-content-between focus-ring"
          >
            <div>
              <div>{{ catShow.name }}</div>
              <span class="text-body-secondary">{{ catShow.location }}</span>
            </div>
            <div style="font-size: 12px; font-weight: bold; margin-top: auto">
              {{ `${formatDate(catShow.startDate)} -  ${formatDate(catShow.endDate)}` }}
            </div>
          </div>
        </template>
        <template #action>
          <button
            v-if="userHasPermission('CreateEvent')"
            @click="addingEvent = true"
            type="button"
            class="btn btn-primary ms-auto px-5 mt-2"
          >
            {{ t("CatShowList.addCatShow") }} +
          </button>
        </template>
      </List>
    </div>
  </div>
  <Modal :modalId="'event-modal'" @onCancel="addingEvent = false" :visible="addingEvent">
    <div class="d-flex flex-column w-100 p-4 gap-4">
      <div>
        <label for="event-name" class="form-label cursor-pointer">{{ t("CatShowList.eventNameInput") }}</label>
        <input id="event-name" type="text" class="form-control" v-model="newEvent.name" />
      </div>
      <div>
        <label for="event-description" class="form-label cursor-pointer">{{ t("CatShowList.eventDescriptionInput") }}</label>
        <input id="event-description" type="text" class="form-control" v-model="newEvent.description" />
      </div>
      <div>
        <label for="event-location" class="form-label cursor-pointer">{{ t("CatShowList.eventLocationInput") }}</label>
        <input id="event-location" type="text" class="form-control" v-model="newEvent.location" />
      </div>
      <div>
        <label for="start-date" class="form-label cursor-pointer">Start Date</label>
        <input id="start-date" type="date" class="form-control" v-model="newEvent.startDate" />
      </div>
      <div>
        <label for="end-date" class="form-label cursor-pointer">End Date</label>
        <input id="end-date" type="date" class="form-control" v-model="newEvent.endDate" />
      </div>

      <button
        @click="
          () => {
            createCatShowMutation.mutate(), (addingEvent = false);
          }
        "
        type="button"
        class="btn btn-primary ms-auto px-5"
      >
        Luo tapahtuma +
      </button>
    </div>
  </Modal>
</template>
