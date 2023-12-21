<script lang="ts" setup>
import userAPI from "../api/userAPI";
import catAPI from "../api/catAPI";
import { useRoute } from "vue-router";
import { useQuery } from "@tanstack/vue-query";
import CatListItem from "../components/CatListItem.vue";

const route = useRoute();

const { data: user } = useQuery({
  queryKey: ["user"],
  queryFn: () => userAPI.getUserById(route.params.userId as string),
});

const { data: userCats } = useQuery({
  queryKey: ["userCats"],
  queryFn: catAPI.getCats,
});
</script>

<template>
  <div class="w-100 h-100 d-flex justify-content-center p-5">
    <div class="p-4 p-sm-5 rounded overflow-auto col-12 col-lg-8">
      <div class="d-flex flex-column"></div>
      <h3>{{ user?.givenName }}</h3>
      <div class="d-flex flex-column mt-4" v-if="userCats && userCats.length > 0">
        <h3>Kissat</h3>
        <CatListItem v-for="cat in userCats" :cat="cat" />
      </div>
    </div>
  </div>
</template>
