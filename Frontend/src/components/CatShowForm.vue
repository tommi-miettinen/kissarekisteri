<script lang="ts" setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

const newEvent = ref({
  name: "",
  description: "",
  location: "",
  startDate: "",
  endDate: "",
});
</script>

<template>
  <div class="w-100 d-flex flex-column p-4 gap-4">
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

    <button @click="$emit('onSave', newEvent)" type="button" class="focus-ring btn bg-black text-white ms-auto px-5 py-2 rounded-3">
      Lisää tapahtuma +
    </button>
  </div>
</template>
