<script lang="ts" setup>
import { ref, computed, watch } from "vue";
import { userStore } from "../store/userStore";
import { toast } from "vue-sonner";
import { useRouter } from "vue-router";
import userAPI from "../api/userAPI";
import Modal from "../components/Modal.vue";
import catAPI from "../api/catAPI";
import { useQuery, useMutation } from "@tanstack/vue-query";
import { useI18n } from "vue-i18n";
import Cropper from "../components/Cropper.vue";

const router = useRouter();
const { t } = useI18n();

const user = ref<User>(userStore((state) => state.user));

const userId = computed(() => user.value?.id);

const { data: cats, refetch: refetchCats } = useQuery({
  queryKey: ["cats" + userId.value],
  queryFn: () => userAPI.getCatsByUserId(userId.value),
  enabled: Boolean(userId.value),
});

const { mutate } = useMutation({
  mutationFn: (newCatPayload: Cat) => catAPI.addCat(newCatPayload),
  onSuccess: () => toast.success("Kissan tiedot lisätty"),
});

/*
const { mutate: mutateUser } = useMutation({
  mutationFn: (userPayload: User) => editUser(userPayload),
  onSuccess: () => toast.info("Tiedot tallennettu"),
});
*/

watch(userId, () => refetchCats());

const newCat = ref<Cat>({
  id: 0,
  name: "",
  birthDate: new Date(),
  breed: "",
  ownerId: user.value ? user.value.id : "0",
  breederId: "0",
});

const updatedCat = ref<Cat>({
  id: 0,
  name: "",
  birthDate: new Date(),
  breed: "",
  ownerId: "0",
  breederId: "0",
});

const editingCat = ref(false);
const editingAvatar = ref(false);

const deletingCat = ref(false);
const deletingCatId = ref();

const addCat = () => mutate(newCat.value);

const setEditingCat = (bool: boolean) => (editingCat.value = bool);

const deleteCat = async (catId: number) => {
  const result = await catAPI.deleteCatById(catId);
  if (!result) return;

  cats.value = cats.value?.filter((c) => c.id !== catId);
  deletingCat.value = false;
};

const avatarLoadError = ref(false);

const editCat = async () => {
  await catAPI.editCat(updatedCat.value);
};

const navigateToCat = (catId: number) => router.push(`/cats/${catId}`);

const loadCatForEdit = (cat: Cat) => {
  updatedCat.value = cat;
  setEditingCat(true);
};
const setEditingAvatar = (bool: boolean) => (editingAvatar.value = bool);
</script>

<template>
  <div class="w-100 h-100 d-flex justify-content-center align-items-center p-5">
    <div class="p-4 p-sm-5 rounded overflow-auto col-12 col-lg-8">
      <div class="d-flex flex-column" v-if="user">
        <div class="d-flex align-items-center gap-2 mb-4">
          <div class="d-flex align-items-center" @click="() => setEditingAvatar(!editingAvatar)">
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
        <div @click="navigateToCat(cat.id)" v-for="cat in cats" :key="cat.id" class="cat d-flex border-bottom p-2 flex align-items-center">
          <div class="col">
            <img
              class="rounded-circle"
              height="30"
              width="30"
              style="object-fit: contain; margin-right: auto"
              src="https://placekitten.com/300/300"
              alt="Cat Image"
            />
          </div>
          <div class="col">
            {{ cat.name }}
          </div>
          <div class="col">{{ cat.breed }}</div>
          <div class="col overflow-hidden">{{ cat.birthDate }}</div>
          <div class="col d-flex gap-2">
            <div @click.stop class="dropdown d-flex ms-auto dropstart">
              <button class="btn ms-auto" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                <svg xmlns="http://www.w3.org/2000/svg" height="1em" viewBox="0 0 128 512">
                  <path
                    d="M64 360a56 56 0 1 0 0 112 56 56 0 1 0 0-112zm0-160a56 56 0 1 0 0 112 56 56 0 1 0 0-112zM120 96A56 56 0 1 0 8 96a56 56 0 1 0 112 0z"
                  />
                </svg>
              </button>
              <ul class="dropdown-menu">
                <li class="dropdown-item" @click.stop="loadCatForEdit(cat)">Muokkaa</li>
                <li class="dropdown-item" @click.stop="(deletingCatId = cat.id), (deletingCat = true)">Poista</li>
              </ul>
            </div>
          </div>
        </div>
        <button
          data-testid="add-new-cat-btn"
          type="button"
          class="btn btn-primary ms-auto mt-2"
          data-bs-toggle="modal"
          data-bs-target="#myModal"
        >
          {{ t("Profile.addCat") }}
        </button>
      </div>
    </div>
  </div>
  <div class="modal fade" id="myModal">
    <div class="modal-dialog modal-dialog-centered">
      <div class="modal-content">
        <div class="modal-body d-flex flex-column">
          <div class="mb-3">
            <label for="catName" class="form-label">Nimi</label>
            <input data-testid="new-cat-name-input" type="text" class="form-control" id="catName" v-model="newCat.name" />
          </div>
          <div class="mb-3">
            <label for="catBreed" class="form-label">Rotu</label>
            <input data-testid="new-cat-breed-input" type="text" class="form-control" id="catBreed" v-model="newCat.breed" />
          </div>
          <div class="mb-3">
            <label for="catBirthDate" class="form-label">Syntymäaika</label>
            <input data-testid="new-cat-birthdate-input" type="date" class="form-control" id="catBirthDate" v-model="newCat.birthDate" />
          </div>
          <button @click="addCat" class="btn btn-primary ms-auto">Lisää kissa</button>
        </div>
      </div>
    </div>
  </div>
  <Modal :modalId="'edit-avatar-modal'" @onCancel="() => setEditingAvatar(false)" :visible="editingAvatar">
    <div class="p-5">
      <Cropper @onCrop="(data:string) => console.log(data)" />
    </div>
  </Modal>
  <Modal :modalId="'edit-modal'" @onCancel="() => setEditingCat(false)" :visible="editingCat">
    <div class="w-100 p-4 d-flex flex-column">
      <div class="mb-3">
        <label for="catName" class="form-label">Nimi</label>
        <input type="text" class="form-control" id="catName" v-model="updatedCat.name" />
      </div>
      <div class="mb-3">
        <label for="catBreed" class="form-label">Rotu</label>
        <input type="text" class="form-control" id="catBreed" v-model="updatedCat.breed" />
      </div>
      <div class="mb-3">
        <label for="catBirthDate" class="form-label">Syntymäaika</label>
        <input type="date" class="form-control" id="catBirthDate" v-model="updatedCat.birthDate" />
      </div>
      <button @click="editCat" class="btn btn-primary ms-auto">Tallenna</button>
    </div>
  </Modal>
  <Modal :modalId="'delete-modal'" @onCancel="deletingCat = false" :visible="deletingCat">
    <div class="modal-content">
      <div class="modal-header">
        <button @click="deletingCat = false" type="button" class="btn-close" aria-label="Close"></button>
      </div>
      <div class="modal-body">
        <p>Poistetaanko kissan tiedot?</p>
      </div>
      <div class="modal-footer">
        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Peruuta</button>
        <button @click="() => deleteCat(deletingCatId)" type="button" class="btn btn-primary">Poista</button>
      </div>
    </div>
  </Modal>
</template>
