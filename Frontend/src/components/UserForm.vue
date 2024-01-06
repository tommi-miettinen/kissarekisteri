<script lang="ts" setup>
import { ref, watchEffect, computed } from "vue";

const roles = ["Admin", "EventOrganizer", "User"];

const props = defineProps({
  user: {
    type: Object as () => User,
  },
});

const newUser = ref({
  name: "",
  password: "",
  role: "User",
});

watchEffect(() => {
  if (props.user) {
    newUser.value = {
      name: props.user.givenName + " " + props.user.surname,
      password: "",
      role: "User",
    };
  }
});

const createUserPayload = computed(() => {
  const name = newUser.value.name.split(" ");
  const payload: UserPayload = {
    MailNickname: name[0] + name[1],
    GivenName: name[0],
    Surname: name[1],
    DisplayName: newUser.value.name,
    Password: newUser.value.password,
    Email: name[0] + "." + name[1] + "@gmail.com",
  };
  return payload;
});
</script>

<template>
  <div style="w-100" class="p-4 gap-3 d-flex flex-column">
    <div>
      <label for="catName" class="form-label w-100">Nimi</label>
      <input data-testid="new-cat-name-input" type="text" class="form-control" id="name" v-model="newUser.name" />
    </div>
    <div>
      <label for="catBirthDate" class="form-label w-100">Salasana</label>
      <input data-testid="new-cat-birthdate-input" type="text" class="form-control" id="catBirthDate" v-model="newUser.password" />
    </div>
    <div>
      <label for="role" class="form-label w-100">Role</label>
      <select v-model="newUser.role" class="form-select" id="role" aria-label="role">
        <option v-for="role in roles" :key="role" :value="role">
          {{ role }}
        </option>
      </select>
    </div>

    <button @click="$emit('onSave', createUserPayload)" class="btn btn-primary">Lisää käyttäjä</button>
  </div>
</template>
