<script lang="ts" setup>
import { ref, watchEffect } from "vue";
import userAPI from "../api/userAPI";
import { useRouter } from "vue-router";
import { useQuery } from "@tanstack/vue-query";

const router = useRouter();
const { data, isLoading } = useQuery({
  queryKey: ["users"],
  queryFn: userAPI.getUsers,
});

const filteredUsers = ref<User[]>([]);
const searchQuery = ref("");

const navigateToUser = (userId: string) => router.push(`/users/${userId}`);

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
  <div class="w-100 h-100 p-0 p-sm-5 d-flex flex-column align-items-center justify-content-center">
    <div class="p-4 p-sm-5 border-sm rounded overflow-auto col-12 col-lg-8">
      <h3>Jäsenet</h3>
      <div class="d-flex py-3 sticky-top bg-white align-items-center">
        <div class="col-12 col-md-8 col-xxl-4">
          <input class="form-control" type="text" v-model="searchQuery" placeholder="Etsi jäsenistä..." />
        </div>
      </div>

      <div class="d-flex flex-column overflow-auto">
        <div v-if="isLoading" class="spinner-border text-primary m-auto" role="status">
          <span class="visually-hidden">Loading...</span>
        </div>
        <div
          v-else
          v-for="user in filteredUsers"
          :key="user.id"
          @click="() => navigateToUser(user.id)"
          class="user p-3 d-flex border-bottom p-2 flex align-items-center"
        >
          <div class="col d-flex align-items-center gap-2 col-8">
            <img
              v-if="user.avatarUrl"
              class="rounded-circle"
              height="32"
              width="32"
              style="object-fit: fill"
              :src="user.avatarUrl"
              alt="User avatar"
            />
            <div
              style="width: 32px; height: 32px; font-size: 14px"
              class="rounded-circle d-flex align-items-center justify-content-center bg-primary fw-bold"
              v-else
            >
              {{ user.givenName[0] + user.surname[0] }}
            </div>
            <div>{{ `${user.givenName}  ${user.surname}` }}</div>
          </div>
          <div class="col"></div>
          <span class="badge rounded-pill text-bg-primary">{{ user.isBreeder ? "Kasvattaja" : "Kasvattaja" }}</span>
        </div>
      </div>
    </div>
  </div>
</template>

<style>
.user:hover {
  cursor: pointer;
  background-color: #f3f4f6;
}
</style>
