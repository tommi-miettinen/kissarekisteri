<script lang="ts" setup>
import { ref, computed } from "vue";
import userAPI from "../api/userAPI";
import UserListItem from "../components/UserListItem.vue";
import { useQuery } from "@tanstack/vue-query";
import { useI18n } from "vue-i18n";
import List from "../components/List.vue";
import Drawer from "../components/Drawer.vue";
import Modal from "../components/Modal.vue";
import { useWindowSize } from "@vueuse/core";

const { t } = useI18n();

const roles = ["Admin", "EventOrganizer", "User"];

const { data } = useQuery({
  queryKey: ["users"],
  queryFn: userAPI.getUsers,
});

const addingUser = ref(false);
const isMobile = computed(() => useWindowSize().width.value < 768);

const newUser = ref({
  name: "",
  email: "",
  password: "",
  role: "User",
});
</script>

<template>
  <div class="w-100 h-100 p-0 p-sm-5 d-flex flex-column align-items-center">
    <div class="p-2 p-sm-5 rounded col-12 col-lg-8 h-100 d-flex flex-column">
      <h3>{{ t("Users.members") }}</h3>
      <List :searchQueryPlaceholder="t('Users.searchInput')" v-if="data" :items="data" :itemsPerPage="7">
        <template v-slot="{ item: user }">
          <UserListItem :user="user" />
        </template>

        <template #action>
          <button @click="addingUser = true" class="btn btn-primary">Lisää käyttäjä +</button>
        </template>
      </List>
    </div>
  </div>
  <Modal :visible="addingUser && !isMobile" @onCancel="addingUser = false">
    <div style="width: 500px" class="p-4 gap-3 d-flex flex-column">
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

      <button class="btn btn-primary">Lisää käyttäjä</button>
    </div>
  </Modal>
  <Drawer :visible="addingUser && isMobile"> moro </Drawer>
</template>
