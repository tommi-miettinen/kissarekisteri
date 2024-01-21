<script lang="ts" setup>
import { ref, watchEffect, computed } from "vue";
import { useQuery } from "@tanstack/vue-query";
import userAPI from "../api/userAPI";
import { QueryKeys } from "../api/queryKeys";

const props = defineProps({
  user: {
    type: Object as () => User,
  },
  formActionButtonText: {
    type: String,
    required: true,
  },
});

const { data: roles } = useQuery({
  queryKey: QueryKeys.ROLES,
  queryFn: () => userAPI.getRoles(),
});

const newUser = ref({
  name: "",
  password: "",
  role: roles.value ? roles.value[0] : "",
});

watchEffect(() => {
  if (props.user) {
    newUser.value.name = props.user.givenName + " " + props.user.surname;
    newUser.value.role = props.user.userRole?.role.name || "User";
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
    Role: "User",
  };
  return payload;
});
</script>

<template>
  <div style="w-100" class="p-4 gap-3 d-flex flex-column">
    <div v-if="!user">
      <label for="username" class="form-label w-100">Nimi</label>
      <input id="username" data-testid="new-cat-name-input" type="text" class="form-control" v-model="newUser.name" />
    </div>
    <div v-if="!user">
      <label for="password" class="form-label w-100">Salasana</label>
      <input id="password" data-testid="new-cat-birthdate-input" type="text" class="form-control" v-model="newUser.password" />
    </div>
    <div>
      <label for="role" class="form-label w-100">Role</label>
      <select id="role" v-if="roles && roles.length > 0" v-model="newUser.role" class="form-select" aria-label="role">
        <option v-for="role in roles" :key="role.name" :value="role.name">
          {{ role.name }}
        </option>
      </select>
    </div>
    <button @click="$emit('onSave', createUserPayload)" class="btn bg-black text-white rounded-3 py-2">{{ formActionButtonText }}</button>
  </div>
</template>
