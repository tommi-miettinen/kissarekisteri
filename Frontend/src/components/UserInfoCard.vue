<script lang="ts" setup>
import { onMounted } from "vue";
import { fetchUser, userIsLoggedInUser } from "../store/userStore";
import { ActionTypes, pushAction, isCurrentAction, removeAction } from "../store/actionStore";
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
import Overlay from "./Overlay.vue";

const { t } = useI18n();
const queryClient = useQueryClient();

const props = defineProps({
  user: {
    type: Object as () => User,
    required: true,
  },
});

const uploadAvatarMutation = useMutation({
  mutationFn: (image: File) => userAPI.uploadAvatar(image),
  onSuccess: async () => {
    toast.dismiss();
    toast.success("Profiilikuva päivitetty");
    queryClient.invalidateQueries({ queryKey: QueryKeys.USER_BY_ID(props.user.id) });
    removeAction(ActionTypes.EDITING_AVATAR);
    removeAction(ActionTypes.EDITING_AVATAR_MOBILE);
    await fetchUser();
  },
});

const registerAsBreederMutation = useMutation({
  mutationFn: () => userAPI.registerAsBreeder(),
  onSuccess: () => {
    toast.dismiss();
    toast.success("Kasvattajaksi rekisteröityminen onnistui");
    queryClient.invalidateQueries({ queryKey: QueryKeys.USER_BY_ID(props.user.id) });
  },
});

onMounted(() => {
  if (!userIsLoggedInUser(props.user)) return;
  const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]');
  [...tooltipTriggerList].forEach((tooltipTriggerEl) => new Tooltip(tooltipTriggerEl));
});

const handleAvatarClick = () => {
  if (
    !userIsLoggedInUser(props.user) ||
    isCurrentAction(ActionTypes.EDITING_AVATAR) ||
    isCurrentAction(ActionTypes.EDITING_AVATAR_MOBILE)
  ) {
    return;
  }
  isMobile.value ? pushAction(ActionTypes.EDITING_AVATAR_MOBILE) : pushAction(ActionTypes.EDITING_AVATAR);
};

const handleStopAvatarEdit = () => {
  removeAction(ActionTypes.EDITING_AVATAR);
  removeAction(ActionTypes.EDITING_AVATAR_MOBILE);
};
</script>

<template>
  <div class="d-flex gap-2 flex-column border-bottom py-3">
    <div class="d-flex gap-2 align-items-center">
      <div data-bs-toggle="tooltip" data-bs-html="true" data-bs-title="Edit avatar">
        <Avatar
          :focusable="userIsLoggedInUser(user)"
          @click="handleAvatarClick"
          @keydown.enter="handleAvatarClick"
          :avatarUrl="user.avatarUrl"
          :displayText="user.givenName[0] + user.surname[0]"
        />
      </div>
      <h3 class="mb-1">{{ `${user.givenName}  ${user.surname}` }}</h3>
    </div>
    <div v-if="user.userRole && user.userRole.role.name !== 'User'">
      {{ t(`Roles.${user.userRole.roleName}`) }}
    </div>
    <div v-if="user.isBreeder">{{ "Kasvattaja" }}</div>
    <button
      tabIndex="0"
      @click="pushAction(ActionTypes.USER_SETTINGS)"
      v-if="!user.isBreeder && userIsLoggedInUser(user)"
      class="btn bg-black text-white focus-ring px-4 rounded-3 me-auto w-sm-100"
    >
      Muokkaa tietoja
    </button>
  </div>

  <Modal :visible="isCurrentAction(ActionTypes.EDITING_AVATAR_MOBILE) && isMobile" @onCancel="handleStopAvatarEdit">
    <div style="width: 90vw" class="rounded-3 overflow-hidden">
      <Cropper @onCrop="uploadAvatarMutation.mutate" :imageSrc="user.avatarUrl" />
    </div>
  </Modal>

  <Modal :visible="isCurrentAction(ActionTypes.EDITING_AVATAR) && !isMobile" @onCancel="handleStopAvatarEdit">
    <div style="width: 500px" class="rounded-3 overflow-hidden">
      <Cropper @onCrop="uploadAvatarMutation.mutate" :imageSrc="user.avatarUrl" />
    </div>
  </Modal>
  <Overlay :visible="isCurrentAction(ActionTypes.USER_SETTINGS) && isMobile" @onCancel="removeAction(ActionTypes.USER_SETTINGS)">
    <div class="p-3">
      <h3>Asetukset</h3>
      <button
        tabIndex="0"
        @click="registerAsBreederMutation.mutate"
        v-if="!user.isBreeder && userIsLoggedInUser(user)"
        class="btn bg-black text-white focus-ring px-4 rounded-3 me-auto w-sm-100"
      >
        Rekisteröidy kasvattajaksi
      </button>
    </div>
  </Overlay>
</template>
