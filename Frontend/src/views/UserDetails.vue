<script lang="ts" setup>
import { ref, computed, watch, nextTick } from "vue";
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
import { watchEffect } from "vue";

const { t } = useI18n();
const route = useRoute();

const motherCatQuery = ref("");
const fatherCatQuery = ref("");

const newCat = ref<CatPayload>({
  name: "",
  birthDate: null,
  breed: "",
  sex: "Female",
  fatherId: undefined,
  motherId: undefined,
});

const updatedCat = ref<EditCatPayload>({
  id: 0,
  name: "",
  birthDate: null,
  breed: "",
  sex: "Female",
});

const editingCat = ref(false);
const editingAvatar = ref(false);
const deletingCat = ref(false);
const deletingCatId = ref();
const addingCat = ref(false);
const showMotherCatSuggestions = ref(false);
const showFatherCatSuggestions = ref(false);
const avatarLoadError = ref(false);

const loggedInUser = computed(() => userStore.user);

const { data: user } = useQuery({
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

const { data: motherCats, refetch: refetchMotherCats } = useQuery({
  queryKey: [motherCatQuery.value],
  queryFn: () => catAPI.getCats(`name=${motherCatQuery.value}&breed=${newCat.value.breed}&limit=3`),
});

const { data: fatherCats, refetch: refetchFatherCats } = useQuery({
  queryKey: ["fathercats" + user.value?.id],
  queryFn: () => catAPI.getCats(`name=${fatherCatQuery.value}&breed=${newCat.value.breed}&limit=3`),
});

const { mutate } = useMutation({
  mutationFn: (newCatPayload: CatPayload) => catAPI.addCat(newCatPayload),
  onSuccess: () => {
    toast.success("Kissan tiedot lisätty"), refetchCats();
  },
  onError: (error) => {
    //@ts-ignore
    toast.error(error.response.data.message || "Jokin meni vikaan.");
  },
});

const { data: catBreeds } = useQuery({
  queryKey: ["catBreeds"],
  queryFn: () => catAPI.getCatBreeds(),
});

/*
const { mutate: mutateUser } = useMutation({
  mutationFn: (userPayload: User) => editUser(userPayload),
  onSuccess: () => toast.info("Tiedot tallennettu"),
});
*/

watch(user, () => refetchCats());

const addCat = () => {
  mutate(newCat.value);
  addingCat.value = false;
};

const deleteCat = async (catId: number) => {
  const result = await catAPI.deleteCatById(catId);
  if (!result) return;

  cats.value = cats.value?.filter((c) => c.id !== catId);
  deletingCat.value = false;
};

const editCat = async () => {
  await catAPI.editCat(updatedCat.value);
};

const loadCatForEdit = (cat: Cat) => {
  updatedCat.value = cat;
  editingCat.value = true;
};

let motherCatTimeout: NodeJS.Timeout;
let fatherCatTimeout: NodeJS.Timeout;

watchEffect(() => {
  if (motherCatQuery.value.length < 2) {
    showMotherCatSuggestions.value = false;
    return;
  }

  showMotherCatSuggestions.value = true;
  clearTimeout(motherCatTimeout);
  motherCatTimeout = setTimeout(() => refetchMotherCats(), 300);
});

watchEffect(() => {
  if (fatherCatQuery.value.length < 2) {
    showFatherCatSuggestions.value = false;
    return;
  }

  showFatherCatSuggestions.value = true;
  clearTimeout(fatherCatTimeout);
  fatherCatTimeout = setTimeout(() => refetchFatherCats(), 300);
});

const handleFatherCatClick = (cat: Cat) => {
  newCat.value.fatherId = cat.id;
  fatherCatQuery.value = cat.name;
  nextTick(() => (showFatherCatSuggestions.value = false));
};

const handleMotherCatClick = (cat: Cat) => {
  newCat.value.motherId = cat.id;
  motherCatQuery.value = cat.name;
  nextTick(() => (showMotherCatSuggestions.value = false));
};

const isFormValid = computed(() => {
  return (
    newCat.value.name.trim() !== "" &&
    newCat.value.breed.trim() !== "" &&
    newCat.value.birthDate &&
    (newCat.value.sex === "Female" || newCat.value.sex === "Male")
  );
});
</script>

<template>
  <div class="w-100 h-100 d-flex justify-content-center p-5">
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
        <div class="overflow-auto" style="height: 500px">
          <div class="d-flex align-items-center" v-for="cat in cats" :key="cat.id">
            <CatListItem :cat="cat" />
            <div v-if="userIsLoggedInUser" data-testid="cat-options" @click.stop class="dropdown d-flex ms-auto dropstart">
              <button class="btn ms-auto" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                <svg xmlns="http://www.w3.org/2000/svg" height="1em" viewBox="0 0 128 512">
                  <path
                    d="M64 360a56 56 0 1 0 0 112 56 56 0 1 0 0-112zm0-160a56 56 0 1 0 0 112 56 56 0 1 0 0-112zM120 96A56 56 0 1 0 8 96a56 56 0 1 0 112 0z"
                  />
                </svg>
              </button>
              <ul class="dropdown-menu">
                <li class="dropdown-item" @click.stop="loadCatForEdit(cat)">Muokkaa</li>
                <li data-testid="start-cat-delete" class="dropdown-item" @click.stop="(deletingCatId = cat.id), (deletingCat = true)">
                  Poista
                </li>
              </ul>
            </div>
          </div>
        </div>
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
    <div class="w-100 p-4 d-flex flex-column">
      <div class="mb-3">
        <label for="catName" class="form-label">Nimi</label>
        <input data-testid="new-cat-name-input" type="text" class="form-control" id="catName" v-model="newCat.name" />
      </div>
      <div class="mb-3">
        <label for="catSex" class="form-label">Rotu</label>
        <select v-model="newCat.breed" class="form-select" id="catSex" aria-label="Cat sex">
          <option v-for="breed in catBreeds" :key="breed.id" :value="breed.name">
            {{ breed.name }}
          </option>
        </select>
      </div>
      <div class="mb-3">
        <label for="catBirthDate" class="form-label">Syntymäaika</label>
        <input data-testid="new-cat-birthdate-input" type="date" class="form-control" id="catBirthDate" v-model="newCat.birthDate" />
      </div>

      <div class="mb-3">
        <label for="catSex" class="form-label">Sukupuoli</label>
        <select data-testid="new-cat-sex-select" class="form-select" id="catSex" v-model="newCat.sex" aria-label="Cat sex">
          <option value="Female">Naaras</option>
          <option value="Male">Uros</option>
        </select>
      </div>

      <div class="mb-3 position-relative">
        <label for="exampleDataList" class="form-label">Kissan äiti</label>
        <input v-model="motherCatQuery" class="form-control" list="datalistOptions" id="exampleDataList" placeholder="Type to search..." />
        <div
          v-if="showMotherCatSuggestions && motherCats && motherCats.length > 0"
          class="z-2 bg-white p-2 gap-2 d-flex flex-column border rounded-2 border-top-0 position-absolute w-100"
        >
          <div :key="cat.id" class="p-2 rounded-3 hover-bg" @click="() => handleMotherCatClick(cat)" v-for="cat in motherCats">
            {{ cat.name }}
          </div>
        </div>
      </div>
      <div class="mb-3 position-relative">
        <label for="exampleDataList" class="form-label">Kissan isä</label>
        <input v-model="fatherCatQuery" class="form-control" list="datalistOptions" id="exampleDataList" placeholder="Type to search..." />
        <div
          v-if="showFatherCatSuggestions && fatherCats && fatherCats.length > 0"
          class="z-2 bg-white p-2 gap-2 d-flex flex-column border rounded-2 border-top-0 position-absolute w-100"
        >
          <div :key="cat.id" class="p-2 rounded-3 hover-bg" @click="() => handleFatherCatClick(cat)" v-for="cat in fatherCats">
            {{ cat.name }}
          </div>
        </div>
      </div>

      <button :disabled="!isFormValid" data-testid="add-new-cat-btn-save" @click="addCat" class="btn btn-primary ms-auto px-5">
        Lisää kissa +
      </button>
    </div>
  </Modal>
  <Modal :modalId="'edit-avatar-modal'" @onCancel="editingAvatar = false" :visible="editingAvatar">
    <div class="p-5">
      <Cropper @onCrop="(data:string) => console.log(data)" />
    </div>
  </Modal>
  <Modal :modalId="'edit-modal'" @onCancel="editingCat = false" :visible="editingCat">
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
      <select class="form-select" aria-label="Default select example">
        <option selected>Open this select menu</option>
        <option value="1">One</option>
        <option value="2">Two</option>
        <option value="3">Three</option>
      </select>
      <button @click="editCat" class="btn btn-primary ms-auto">Tallenna</button>
    </div>
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

<style>
.hover-bg:hover {
  cursor: pointer;
  background-color: #f3f4f6;
}
</style>
