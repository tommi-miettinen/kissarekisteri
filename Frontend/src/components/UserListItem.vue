<script lang="ts" setup>
import { useRouter } from "vue-router";
import { useI18n } from "vue-i18n";
import Avatar from "./Avatar.vue";

defineProps({
  user: {
    type: Object as () => User,
    required: true,
  },
});

const router = useRouter();
const { t } = useI18n();

const navigateToUser = (userId: string) => router.push(`/users/${userId}`);
</script>

<template>
  <div v-if="user" class="border-bottom py-1 w-100">
    <div
      tabindex="0"
      @keyup.enter="() => navigateToUser(user.id)"
      @click="() => navigateToUser(user.id)"
      class="hover-bg p-3 d-flex rounded-3 p-2 flex align-items-center focus-ring"
    >
      <div class="col d-flex align-items-center gap-2 col-8">
        <Avatar :displayText="user.givenName[0] + user.surname[0]" />
        <div>{{ `${user.givenName}  ${user.surname}` }}</div>
      </div>
      <div class="col justify-content-end d-flex gap-2 align-items-center">
        <span class="badge rounded-pill text-bg-primary">{{ user.isBreeder ? t("Users.breeder") : t("Users.breeder") }}</span>

        <slot name="actions"></slot>
      </div>
    </div>
  </div>
</template>
