<script lang="ts" setup>
import { ref, watchEffect, nextTick, computed } from "vue";
import catAPI from "../api/catAPI";
import { useQuery } from "@tanstack/vue-query";

const motherCatQuery = ref("");
const fatherCatQuery = ref("");

const props = defineProps({
  cat: {
    type: Object as () => Cat,
  },
});

const showMotherCatSuggestions = ref(false);
const showFatherCatSuggestions = ref(false);

const { data: motherCats, refetch: refetchMotherCats } = useQuery({
  queryKey: ["motherCats" + motherCatQuery.value],
  queryFn: () => catAPI.getCats(`name=${motherCatQuery.value}&breed=${newCat.value.breed}&limit=3&sex=female`),
});

const { data: fatherCats, refetch: refetchFatherCats } = useQuery({
  queryKey: ["fatherCats" + fatherCatQuery.value],
  queryFn: () => catAPI.getCats(`name=${fatherCatQuery.value}&breed=${newCat.value.breed}&limit=3&sex=male`),
});

const { data: catBreeds } = useQuery({
  queryKey: ["catBreeds"],
  queryFn: () => catAPI.getCatBreeds(),
});

const newCat = ref<CatPayload>({
  name: "",
  birthDate: null,
  breed: "",
  sex: "Female",
  fatherId: undefined,
  motherId: undefined,
});

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

watchEffect(() => {
  if (props.cat) {
    newCat.value = { ...newCat.value, ...props.cat };
  }
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
      <input v-model="motherCatQuery" class="form-control" id="exampleDataList" placeholder="Type to search..." />
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
      <input v-model="fatherCatQuery" class="form-control" id="exampleDataList" placeholder="Type to search..." />
      <div
        v-if="showFatherCatSuggestions && fatherCats && fatherCats.length > 0"
        class="z-2 bg-white p-2 gap-2 d-flex flex-column border rounded-2 border-top-0 position-absolute w-100"
      >
        <div :key="cat.id" class="p-2 rounded-3 hover-bg" @click="() => handleFatherCatClick(cat)" v-for="cat in fatherCats">
          {{ cat.name }}
        </div>
      </div>
    </div>

    <button
      :disabled="!isFormValid"
      data-testid="add-new-cat-btn-save"
      @click="$emit('onSave', newCat)"
      class="btn btn-primary ms-auto px-5"
    >
      Lisää kissa +
    </button>
  </div>
</template>