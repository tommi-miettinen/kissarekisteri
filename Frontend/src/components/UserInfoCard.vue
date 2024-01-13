<script lang="ts" setup>
import { onMounted } from "vue";
import { userIsLoggedInUser } from "../store/userStore";
import { pushAction, isCurrentAction, removeAction } from "../store/actionStore";
import { useMutation, useQueryClient } from "@tanstack/vue-query";
import userAPI from "../api/userAPI";
import { useI18n } from "vue-i18n";
import { QueryKeys } from "../api/queryKeys";
import { toast } from "vue-sonner";
import Avatar from "./Avatar.vue";
import Cropper from "./Cropper.vue";
import Modal from "./Modal.vue";
import { isMobile } from "../store/actionStore";
import { Tooltip } from "bootstrap";

const { t } = useI18n();
const queryClient = useQueryClient();

enum ActionType {
  EDITING_AVATAR = "EDITING_AVATAR",
  EDITING_AVATAR_MOBILE = "EDITING_AVATAR_MOBILE",
}

const props = defineProps({
  user: {
    type: Object as () => User,
    required: true,
  },
});

const uploadAvatarMutation = useMutation({
  mutationFn: (image: File) => userAPI.uploadAvatar(image),
  onSuccess: () => {
    toast.success("Profiilikuva päivitetty");
    queryClient.invalidateQueries({ queryKey: QueryKeys.USER_BY_ID(props.user.id) });
    removeAction(ActionType.EDITING_AVATAR);
    removeAction(ActionType.EDITING_AVATAR_MOBILE);
  },
});

const registerAsBreederMutation = useMutation({
  mutationFn: () => userAPI.registerAsBreeder(),
  onSuccess: () => {
    toast.success("Kasvattajaksi rekisteröityminen onnistui");
    queryClient.invalidateQueries({ queryKey: QueryKeys.USER_BY_ID(props.user.id) });
  },
});

onMounted(() => {
  if (!userIsLoggedInUser(props.user)) return;
  const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]');
  [...tooltipTriggerList].forEach((tooltipTriggerEl) => new Tooltip(tooltipTriggerEl));
});
</script>

<template>
  <div class="d-flex gap-2 flex-column border-bottom py-3">
    <div class="d-flex gap-2 align-items-center">
      <div data-bs-toggle="tooltip" data-bs-html="true" data-bs-title="Edit avatar">
        <Avatar
          :focusable="userIsLoggedInUser(user)"
          @click="userIsLoggedInUser(user) && pushAction(isMobile ? ActionType.EDITING_AVATAR_MOBILE : ActionType.EDITING_AVATAR)"
          @keydown.enter="userIsLoggedInUser(user) && pushAction(isMobile ? ActionType.EDITING_AVATAR_MOBILE : ActionType.EDITING_AVATAR)"
          :avatarUrl="user.avatarUrl"
          :displayText="user.givenName[0] + user.surname[0]"
        />
      </div>
      <h3 class="m-0">{{ `${user.givenName}  ${user.surname}` }}</h3>
    </div>
    <div v-if="user.userRole && user.userRole.role.name !== 'User'">
      {{ t(`Roles.${user.userRole.roleName}`) }}
    </div>
    <div v-if="user.isBreeder">{{ "Kasvattaja" }}</div>
    <button
      tabIndex="0"
      @click="registerAsBreederMutation.mutate"
      v-if="!user.isBreeder && userIsLoggedInUser(user)"
      class="btn bg-black text-white focus-ring px-4 rounded-3 me-auto w-sm-100"
    >
      Rekisteröidy kasvattajaksi
    </button>
  </div>

  <Modal
    :visible="isCurrentAction(ActionType.EDITING_AVATAR_MOBILE) && isMobile"
    @onCancel="removeAction(ActionType.EDITING_AVATAR_MOBILE)"
  >
    <div style="width: 90vw" class="rounded-3 overflow-hidden">
      <Cropper
        @onCrop="uploadAvatarMutation.mutate"
        :imageSrc="'https://kissarekisteritf.blob.core.windows.net/images/a2174d16-0f1e-452f-b1a8-2c2d58600d05.jpg'"
      />
    </div>
  </Modal>

  <Modal :visible="isCurrentAction(ActionType.EDITING_AVATAR) && !isMobile" @onCancel="removeAction(ActionType.EDITING_AVATAR)">
    <div style="width: 500px" class="rounded-3 overflow-hidden">
      <Cropper
        @onCrop="uploadAvatarMutation.mutate"
        :imageSrc="'https://kissarekisteritf.blob.core.windows.net/images/a2174d16-0f1e-452f-b1a8-2c2d58600d05.jpg'"
      />
    </div>
  </Modal>
</template>
