<script lang="ts" setup>
import { ref, watchEffect, computed } from "vue";
import { useQuery } from "@tanstack/vue-query";
import userAPI from "../api/userAPI";

const props = defineProps({
  user: {
    type: Object as () => User,
  },
});

const { data: roles } = useQuery({
  queryKey: ["roles"],
  queryFn: () => userAPI.getRoles(),
});

const newUser = ref({
  name: "",
  password: "",
  role: roles.value ? roles.value[0].value : "",
});

watchEffect(() => {
  if (props.user) {
    newUser.value.name = props.user.givenName + " " + props.user.surname;
    //@ts-ignore
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
    Role: newUser.value.role,
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
      <select v-if="roles && roles.length > 0" v-model="newUser.role" class="form-select" id="role" aria-label="role">
        <option v-for="role in roles" :key="role.name" :value="role.name">
          {{ role.name }}
        </option>
      </select>
    </div>
    <button @click="$emit('onSave', createUserPayload)" class="btn btn-primary">Lisää käyttäjä</button>
  </div>
</template>
