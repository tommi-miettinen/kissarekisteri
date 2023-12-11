<script lang="ts" setup>
import userAPI from "../api/userAPI";
import catAPI from "../api/catAPI";
import { useRoute, useRouter } from "vue-router";
import { useQuery } from "@tanstack/vue-query";

const route = useRoute();
const router = useRouter();

const { data: user } = useQuery({
  queryKey: ["user"],
  queryFn: () => userAPI.getUserById(route.params.userId as string),
});

const { data: userCats } = useQuery({
  queryKey: ["userCats"],
  queryFn: catAPI.getCats,
});

const navigateToCat = (catId: number) => router.push(`/cats/${catId}`);
</script>

<template>
  <div class="w-100 h-100 d-flex justify-content-center p-5">
    <div class="d-flex flex-column w-50">
      <div class="d-flex flex-column"></div>
      <h3>{{ user?.givenName }}</h3>
      <div class="d-flex flex-column mt-4" v-if="userCats && userCats.length > 0">
        <h3>Kissat</h3>

        <div
          @click="() => navigateToCat(cat.id)"
          v-for="cat in userCats"
          :key="cat.id"
          class="cat row border-bottom p-2 flex align-items-center"
        >
          <div class="col">
            <img
              class="rounded-circle"
              height="30"
              width="30"
              style="object-fit: contain; margin-right: auto"
              src="https://placekitten.com/300/300"
              alt="Cat Image"
            />
          </div>
          <div class="col">
            {{ cat.name }}
          </div>
          <div class="col">{{ cat.breed }}</div>
          <div class="col overflow-hidden">{{ cat.birthDate }}</div>
          <div class="col"></div>
        </div>
      </div>
    </div>
  </div>
</template>
