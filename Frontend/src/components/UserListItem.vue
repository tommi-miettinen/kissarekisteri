<script lang="ts" setup>
import { computed } from "vue";
import { useI18n } from "vue-i18n";
import { navigateTo } from "../store/routeStore";
import Avatar from "./Avatar.vue";

const props = defineProps({
  user: {
    type: Object as () => User,
    required: true,
  },
});

const { t } = useI18n();

const colors = ["#818cf8", "#fb7185", "#34d399", "#f87171", "#facc15"];
const selectedColor = computed(() => props.user && colors[(props.user.givenName + props.user.surname).length % colors.length]);
</script>

<template>
  <div v-if="user" class="border-bottom py-1 w-100">
    <div
      tabindex="0"
      @keyup.enter="navigateTo(`/users/${user?.id}`)"
      @click="navigateTo(`/users/${user?.id}`)"
      class="hover-bg-1 p-3 d-flex rounded-3 p-2 flex align-items-center focus-ring cursor-pointer"
    >
      <div class="col d-flex align-items-center gap-2 col-8">
        <Avatar
          :focusable="false"
          :avatarUrl="user.avatarUrl"
          :displayText="user.givenName[0] + user.surname[0]"
          :backgroundColor="selectedColor"
        />
        <div class="fw-semibold text-capitalize">{{ `${user.givenName}  ${user.surname}` }}</div>
      </div>
      <div class="col justify-content-end d-flex gap-2 align-items-center">
        <span v-if="user.isBreeder" class="badge rounded-pill text-bg-primary">{{ t("Users.breeder") }}</span>
        <slot name="actions"></slot>
      </div>
    </div>
  </div>
</template>
