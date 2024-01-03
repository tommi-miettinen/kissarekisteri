<script lang="ts" setup>
import { ref, computed, watch } from "vue";
import { userStore } from "../store/userStore";
import { toast } from "vue-sonner";
import userAPI from "../api/userAPI";
import Modal from "../components/Modal.vue";
import catAPI from "../api/catAPI";
import { useQuery, useMutation } from "@tanstack/vue-query";
import { useI18n } from "vue-i18n";
import Cropper from "../components/Cropper.vue";
import CatListItem from "../components/CatListItem.vue";
import { useRoute } from "vue-router";
import CatForm from "../components/CatForm.vue";
import List from "../components/List.vue";
import Drawer from "../components/Drawer.vue";
import { useWindowSize } from "@vueuse/core";
import Dropdown from "../components/Dropdown.vue";

const { t } = useI18n();
const route = useRoute();

const editingCat = ref(false);
const editingAvatar = ref(false);
const deletingCat = ref(false);
const deletingCatId = ref();
const addingCat = ref(false);
const avatarLoadError = ref(false);
const selectedCat = ref();

const loggedInUser = computed(() => userStore.user);

const {
  data: user,
  isError: isUserError,
  refetch: refetchUser,
} = useQuery({
  queryKey: ["user" + route.params.userId],
  queryFn: () => userAPI.getUserById(route.params.userId as string),
});

const userIsLoggedInUser = computed(() => {
  if (user.value && loggedInUser.value) return user.value.id === loggedInUser.value.id;
  return false;
});

const { data: catsData, refetch: refetchCats } = useQuery({
  queryKey: ["userCats"],
  queryFn: () => userAPI.getCatsByUserId(user.value?.id as string),
  enabled: Boolean(user.value?.id),
});

const cats = computed(() => catsData.value?.data);

const { mutate } = useMutation({
  mutationFn: (newCatPayload: CatPayload) => catAPI.addCat(newCatPayload),
  onSuccess: () => {
    toast.success("Kissan tiedot lisätty"), refetchCats();
  },
  onError: () => toast.error("Jokin meni vikaan."),
});

const deleteMutation = useMutation({
  mutationFn: (catId: number) => catAPI.deleteCatById(catId),
  onSuccess: () => {
    toast.success("Kissan tiedot poistettu"), refetchCats();
  },
  onError: () => toast.error("Jokin meni vikaan."),
});

watch([route, user], () => {
  refetchCats();
  refetchUser();
});

const isMobile = computed(() => useWindowSize().width.value < 768);
const openDrawerId = ref<number | null>(null);

const addCat = (newCat: CatPayload) => {
  mutate(newCat);
  addingCat.value = false;
};

const deleteCat = async (catId: number) => {
  deleteMutation.mutate(catId);
  deletingCat.value = false;
};

const editCat = async (updatedCat: EditCatPayload) => {
  await catAPI.editCat(updatedCat);
};

const loadCatForEdit = (cat: Cat) => {
  selectedCat.value = cat;
  editingCat.value = true;
};

const catListItemRefs = ref<Record<number, HTMLElement>>({});
const toggleDrawer = (catId: number) => {
  openDrawerId.value = openDrawerId.value === catId ? null : catId;
};

watch([editingCat, deletingCat], () => {
  if (editingCat.value || deletingCat.value) {
    openDrawerId.value = null;
  }
});
</script>

<template>
  <h3 v-if="isUserError" class="m-5 fw-bold">{{ t("Profile.404") }}</h3>
  <div v-if="user" class="w-100 h-100 d-flex justify-content-center sm-p-5 overflow-hidden">
    <div class="p-4 p-sm-5 rounded col-12 col-lg-8">
      <div class="d-flex flex-column" v-if="user">
        <div class="d-flex align-items-center gap-2 mb-4">
          <div class="d-flex align-items-center" @click="editingAvatar = false">
            <img
              v-if="user.avatarUrl && !avatarLoadError"
              class="rounded-circle"
              height="32"
              width="32"
              style="object-fit: fill"
              :src="user.avatarUrl"
              alt="User avatar"
              :onerror="(avatarLoadError = true)"
            />
            <div
              style="width: 32px; height: 32px; font-size: 14px"
              class="rounded-circle d-flex align-items-center justify-content-center bg-primary fw-bold"
              v-else
            >
              {{ user.givenName[0] + user.surname[0] }}
            </div>
          </div>
          <h3>{{ `${user.givenName}  ${user.surname}` }}</h3>
        </div>
      </div>
      <div class="d-flex flex-column rounded h-100 pb-5">
        <h3 v-if="cats && cats.length > 0">{{ t("Profile.cats") }}</h3>
        <List :searchQueryPlaceholder="t('Cats.searchInput')" v-if="cats" :items="cats" :itemsPerPage="6">
          <template v-slot="{ item: cat }">
            <CatListItem :key="cat.id" :cat="cat">
              <template #actions>
                <div
                  :ref="(el) => (catListItemRefs[cat.id] = el as HTMLDivElement)"
                  @keyup.enter.stop
                  v-if="userIsLoggedInUser"
                  data-testid="cat-options"
                  @click.stop="toggleDrawer(cat.id)"
                  class="d-flex"
                >
                  <button tabindex="0" class="btn py-1 px-2 accordion d-flex focus-ring rounded-1" type="button">
                    <svg xmlns="http://www.w3.org/2000/svg" height="1em" viewBox="0 0 128 512">
                      <path
                        d="M64 360a56 56 0 1 0 0 112 56 56 0 1 0 0-112zm0-160a56 56 0 1 0 0 112 56 56 0 1 0 0-112zM120 96A56 56 0 1 0 8 96a56 56 0 1 0 112 0z"
                      />
                    </svg>
                  </button>
                  <Dropdown :visible="!isMobile" :placement="'left-start'" :triggerRef="catListItemRefs[cat.id]">
                    <li @keyup.enter="loadCatForEdit(cat)" tabIndex="0" class="dropdown-item" @click.stop="loadCatForEdit(cat)">Muokkaa</li>
                    <li
                      tabIndex="0"
                      class="dropdown-item"
                      data-testid="start-cat-delete"
                      @keyup.enter="(deletingCatId = cat.id), (deletingCat = true)"
                      @click.stop="(deletingCatId = cat.id), (deletingCat = true)"
                    >
                      Poista
                    </li>
                  </Dropdown>
                  <Drawer :visible="isMobile && openDrawerId === cat.id">
                    <div class="gap-2 p-2 d-flex flex-column list-unstyled">
                      <li @keyup.enter="loadCatForEdit(cat)" tabIndex="0" class="p-2 hover-bg rounded-3" @click.stop="loadCatForEdit(cat)">
                        Muokkaa
                      </li>
                      <li
                        tabIndex="0"
                        data-testid="start-cat-delete"
                        class="p-2 hover-bg rounded-3"
                        @keyup.enter="(deletingCatId = cat.id), (deletingCat = true)"
                        @click.stop="(deletingCatId = cat.id), (deletingCat = true)"
                      >
                        Poista
                      </li>
                    </div>
                  </Drawer>
                </div>
              </template>
            </CatListItem>
          </template>
          <template #action>
            <button
              v-if="userIsLoggedInUser"
              @click="addingCat = true"
              data-testid="add-new-cat-btn"
              type="button"
              class="btn btn-primary ms-auto px-5"
            >
              {{ t("Profile.addCat") }} +
            </button>
          </template>
        </List>
      </div>
    </div>
  </div>
  <Drawer :fullsize="true" :visible="addingCat && isMobile" @onCancel="addingCat = false">
    <CatForm @onSave="addCat" />
  </Drawer>
  <Modal :modalId="'add-cat-modal'" :visible="addingCat && !isMobile" @onCancel="addingCat = false">
    <div style="width: 550px">
      <CatForm @onSave="addCat" />
    </div>
  </Modal>
  <Modal :modalId="'edit-avatar-modal'" @onCancel="editingAvatar = false" :visible="editingAvatar">
    <div class="p-5">
      <Cropper @onCrop="(data:string) => console.log(data)" />
    </div>
  </Modal>
  <Modal :modalId="'edit-modal'" @onCancel="editingCat = false" :visible="editingCat && !isMobile">
    <div style="width: 550px">
      <CatForm :cat="selectedCat" @onSave="editCat" />
    </div>
  </Modal>

  <Drawer :fullsize="true" :visible="editingCat && isMobile" @onCancel="editingCat = false">
    <CatForm :cat="selectedCat" @onSave="editCat" />
  </Drawer>

  <Modal :modalId="'delete-modal'" @onCancel="deletingCat = false" :visible="deletingCat">
    <div style="width: 90vw" class="p-4 d-flex flex-column">
      <p>Poistetaanko kissan tiedot?</p>
      <div class="d-flex gap-2 justify-content-end">
        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Peruuta</button>
        <button data-testid="confirm-cat-delete" @click="() => deleteCat(deletingCatId)" type="button" class="btn btn-danger">
          Poista
        </button>
      </div>
    </div>
  </Modal>
</template>
