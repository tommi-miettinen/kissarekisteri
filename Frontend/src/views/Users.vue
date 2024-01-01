<script lang="ts" setup>
import userAPI from "../api/userAPI";
import UserListItem from "../components/UserListItem.vue";
import { useQuery } from "@tanstack/vue-query";
import { useI18n } from "vue-i18n";
import List from "../components/List.vue";

const { t } = useI18n();

const { data } = useQuery({
  queryKey: ["users"],
  queryFn: userAPI.getUsers,
});
</script>

<template>
  <div class="w-100 h-100 p-0 p-sm-5 d-flex flex-column align-items-center">
    <div class="p-2 p-sm-5 rounded col-12 col-lg-8 h-100 d-flex flex-column">
      <h3>{{ t("Users.members") }}</h3>
      <List v-if="data" :items="data" :itemsPerPage="8">
        <template v-slot="{ item: user }">
          <UserListItem :user="user" />
        </template>
      </List>
    </div>
  </div>
</template>
