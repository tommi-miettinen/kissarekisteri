<script setup lang="ts">
import { ref, computed } from "vue";
import { useRouter } from "vue-router";
import Modal from "../components/Modal.vue";
import { useI18n } from "vue-i18n";
import { formatDate } from "../utils/formatDate";
import catShowAPI from "../api/catShowAPI";
import { useQuery, useMutation } from "@tanstack/vue-query";
import { toast } from "vue-sonner";
import { userHasPermission } from "../store/userStore";
import List from "../components/List.vue";
import Drawer from "../components/Drawer.vue";
import { useWindowSize } from "@vueuse/core";
import CatShowForm from "../components/CatShowForm.vue";

const router = useRouter();
const { t } = useI18n();

const addingEvent = ref(false);

const { data: catShows, refetch: refetchCatShows } = useQuery({
  queryKey: ["catshows"],
  queryFn: () => catShowAPI.getEvents(),
});

const createCatShowMutation = useMutation({
  mutationFn: (newEvent: CatShowEvent) => catShowAPI.createCatShowEvent(newEvent),
  onSuccess: () => {
    toast.info("Tapahtuma luotu"), refetchCatShows();
  },
});

const isMobile = computed(() => useWindowSize().width.value < 768);

const navigateToEvent = (eventId: number) => router.push(`/catshows/${eventId}`);
</script>
<template>
  <div class="w-100 h-100 p-0 p-sm-5 d-flex flex-column align-items-center">
    <div class="p-2 p-sm-5 rounded col-12 col-lg-8 h-100 d-flex flex-column">
      <h3>{{ t("CatShowList.catShows") }}</h3>

      <List :searchQueryPlaceholder="t('CatShowList.searchInput')" v-if="catShows" :items="catShows" :itemsPerPage="7">
        <template v-slot="{ item: catShow }">
          <div class="py-1 border-bottom">
            <div
              tabindex="0"
              @keyup.enter="() => navigateToEvent(catShow.id!)"
              @click="() => navigateToEvent(catShow.id!)"
              class="d-flex gap-4 rounded-3 px-3 py-2 align-items-center pointer hover-bg justify-content-between focus-ring"
            >
              <div>
                <div>{{ catShow.name }}</div>
                <span class="text-body-secondary">{{ catShow.location }}</span>
              </div>
              <div style="font-size: 12px; font-weight: bold; margin-top: auto">
                {{ `${formatDate(catShow.startDate)} -  ${formatDate(catShow.endDate)}` }}
              </div>
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
  <Drawer :fullsize="true" :visible="addingEvent && isMobile">
    <CatShowForm @onSave="createCatShowMutation.mutate" />
  </Drawer>
  <Modal :modalId="'event-modal'" @onCancel="addingEvent = false" :visible="addingEvent && !isMobile">
    <div style="width: 550px">
      <CatShowForm @onSave="createCatShowMutation.mutate" />
    </div>
  </Modal>
</template>
