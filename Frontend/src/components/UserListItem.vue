<script lang="ts" setup>
import { ref } from "vue";
import { useRouter } from "vue-router";
import { useI18n } from "vue-i18n";

defineProps({
  user: {
    type: Object as () => User,
    required: true,
  },
});

const router = useRouter();
const { t } = useI18n();

const avatarLoadError = ref(false);

const navigateToUser = (userId: string) => router.push(`/users/${userId}`);
</script>

<template>
  <div class="border-bottom py-1 w-100">
    <div
      tabindex="0"
      @keyup.enter="() => navigateToUser(user.id)"
      @click="() => navigateToUser(user.id)"
      class="hover-bg p-3 d-flex rounded-3 p-2 flex align-items-center focus-ring"
    >
      <div class="col d-flex align-items-center gap-2 col-8">
        <img
          v-if="user.avatarUrl && !avatarLoadError"
          class="rounded-circle"
          height="32"
          width="32"
          style="object-fit: fill"
          :src="user.avatarUrl"
          alt="User avatar"
          :onerror="(avatarLoadError = true)"
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
      <span class="badge rounded-pill text-bg-primary">{{ user.isBreeder ? t("Users.breeder") : t("Users.breeder") }}</span>
    </div>
  </div>
</template>
