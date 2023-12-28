<script lang="ts" setup>
import { ref, watchEffect } from "vue";
import userAPI from "../api/userAPI";
import UserListItem from "../components/UserListItem.vue";
import { useQuery } from "@tanstack/vue-query";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

const { data, isLoading } = useQuery({
  queryKey: ["users"],
  queryFn: userAPI.getUsers,
});

const filteredUsers = ref<User[]>([]);
const searchQuery = ref("");

watchEffect(() => {
  const query = searchQuery.value.replace(/\s+/g, "");

  if (!query) {
    filteredUsers.value = data?.value || [];
  } else {
    filteredUsers.value =
      data.value?.filter((user) => {
        const fullName = user.givenName.toLowerCase() + user.surname.toLowerCase();
        return fullName.includes(query.toLowerCase());
      }) || [];
  }
});
</script>

<template>
  <div class="w-100 h-100 p-0 p-sm-5 d-flex flex-column align-items-center">
    <div class="p-4 p-sm-5 rounded overflow-auto col-12 col-lg-8">
      <h3>{{ t("Users.members") }}</h3>
      <div class="d-flex py-3 sticky-top bg-white align-items-center">
        <div class="col-12 col-md-8 col-xxl-4">
          <input class="form-control" type="text" v-model="searchQuery" :placeholder="t('Users.searchInput')" aria-label="Search Users" />
        </div>
      </div>
      <div class="d-flex flex-column overflow-auto">
        <div v-if="isLoading" class="spinner-border text-primary m-auto" role="status">
          <span class="visually-hidden">Loading...</span>
        </div>
        <UserListItem :key="user.id" :user="user" v-for="user in data" />
      </div>
    </div>
  </div>
</template>
