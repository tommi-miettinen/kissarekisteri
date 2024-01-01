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

const { data: user, isError: isUserError } = useQuery({
  queryKey: ["user" + route.params.userId],
  queryFn: () => userAPI.getUserById(route.params.userId as string),
});

const userIsLoggedInUser = computed(() => {
  if (user.value && loggedInUser.value) return user.value.id === loggedInUser.value.id;
  return false;
});

const { data: cats, refetch: refetchCats } = useQuery({
  queryKey: ["cats" + user.value?.id],
  queryFn: () => userAPI.getCatsByUserId(user.value?.id as string),
  enabled: Boolean(user.value?.id),
});

const { mutate } = useMutation({
  mutationFn: (newCatPayload: CatPayload) => catAPI.addCat(newCatPayload),
  onSuccess: () => {
    toast.success("Kissan tiedot lisätty"), refetchCats();
  },
  onError: () => toast.error("Jokin meni vikaan."),
});

/*
const { mutate: mutateUser } = useMutation({
  mutationFn: (userPayload: User) => editUser(userPayload),
  onSuccess: () => toast.info("Tiedot tallennettu"),
});
*/

watch(user, () => refetchCats());

const addCat = (newCat: CatPayload) => {
  mutate(newCat);
  addingCat.value = false;
};

const deleteCat = async (catId: number) => {
  const result = await catAPI.deleteCatById(catId);
  if (!result) return;

  cats.value = cats.value?.filter((c) => c.id !== catId);
  deletingCat.value = false;
};

const editCat = async (updatedCat: EditCatPayload) => {
  await catAPI.editCat(updatedCat);
};

const loadCatForEdit = (cat: Cat) => {
  selectedCat.value = cat;
  editingCat.value = true;
};
</script>

<template>
  <h3 v-if="isUserError" class="m-5 fw-bold">{{ t("Profile.404") }}</h3>
  <div v-if="user" class="w-100 h-100 d-flex justify-content-center p-5">
    <div class="p-4 p-sm-5 rounded overflow-auto col-12 col-lg-8">
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
      <div class="d-flex flex-column rounded" v-if="cats">
        <h3>{{ t("Profile.cats") }}</h3>
        <List v-if="cats" :items="cats" :itemsPerPage="6">
          <template v-slot="{ item: cat }">
            <CatListItem :key="cat.id" :cat="cat">
              <template #actions>
                <div @keyup.enter.stop="" v-if="userIsLoggedInUser" data-testid="cat-options" @click.stop class="dropdown d-flex dropstart">
                  <button
                    tabindex="0"
                    class="btn py-1 px-2 accordion d-flex focus-ring rounded-1"
                    type="button"
                    data-bs-toggle="dropdown"
                    aria-expanded="false"
                  >
                    <svg xmlns="http://www.w3.org/2000/svg" height="1em" viewBox="0 0 128 512">
                      <path
                        d="M64 360a56 56 0 1 0 0 112 56 56 0 1 0 0-112zm0-160a56 56 0 1 0 0 112 56 56 0 1 0 0-112zM120 96A56 56 0 1 0 8 96a56 56 0 1 0 112 0z"
                      />
                    </svg>
                  </button>
                  <ul class="dropdown-menu">
                    <li @keyup.enter="loadCatForEdit(cat)" tabIndex="0" class="dropdown-item" @click.stop="loadCatForEdit(cat)">Muokkaa</li>
                    <li
                      tabIndex="0"
                      data-testid="start-cat-delete"
                      class="dropdown-item"
                      @keyup.enter="(deletingCatId = cat.id), (deletingCat = true)"
                      @click.stop="(deletingCatId = cat.id), (deletingCat = true)"
                    >
                      Poista
                    </li>
                  </ul>
                </div>
              </template>
            </CatListItem>
          </template>
        </List>
        <button
          v-if="userIsLoggedInUser"
          @click="addingCat = true"
          data-testid="add-new-cat-btn"
          type="button"
          class="btn btn-primary ms-auto mt-2 px-5"
        >
          {{ t("Profile.addCat") }} +
        </button>
      </div>
    </div>
  </div>
  <Modal :modalId="'add-cat-modal'" :visible="addingCat" @onCancel="addingCat = false">
    <CatForm @onSave="addCat" />
  </Modal>
  <Modal :modalId="'edit-avatar-modal'" @onCancel="editingAvatar = false" :visible="editingAvatar">
    <div class="p-5">
      <Cropper @onCrop="(data:string) => console.log(data)" />
    </div>
  </Modal>
  <Modal :modalId="'edit-modal'" @onCancel="editingCat = false" :visible="editingCat">
    <CatForm :cat="selectedCat" @onSave="editCat" />
  </Modal>
  <Modal :modalId="'delete-modal'" @onCancel="deletingCat = false" :visible="deletingCat">
    <div class="w-100 p-4 d-flex flex-column">
      <p>Poistetaanko kissan tiedot?</p>
      <div class="d-flex gap-2 justify-content-end">
        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Peruuta</button>
        <button data-testid="confirm-cat-delete" @click="() => deleteCat(deletingCatId)" type="button" class="btn btn-primary">
          Poista
        </button>
      </div>
    </div>
  </Modal>
</template>
