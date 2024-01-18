<script lang="ts" setup>
import { ref, computed, reactive, watchEffect } from "vue";
import userAPI from "../api/userAPI";
import UserListItem from "../components/UserListItem.vue";
import { useQuery } from "@tanstack/vue-query";
import { useI18n } from "vue-i18n";
import List from "../components/List.vue";
import Drawer from "../components/Drawer.vue";
import Modal from "../components/Modal.vue";
import UserForm from "../components/UserForm.vue";
import Dropdown from "../components/Dropdown.vue";
import { isMobile, ActionTypes, pushAction, isCurrentAction, removeAction } from "../store/actionStore";
import { toast } from "vue-sonner";
import { useMutation } from "@tanstack/vue-query";
import { QueryKeys } from "../api/queryKeys";
import Spinner from "../components/Spinner.vue";
import ThreeDotsIcon from "../icons/ThreeDotsIcon.vue";
import { onMounted } from "vue";
import { setCurrentRouteLabel } from "../store/routeStore";
import { PermissionTypes, userHasPermission } from "../store/userStore";

const { t } = useI18n();

const userQuery = useQuery({
  queryKey: QueryKeys.USERS,
  queryFn: userAPI.getUsers,
});

const users = computed(() => userQuery.data.value);

const translatedValues = ref<User[]>();

watchEffect(() => {
  if (!users) return;
  translatedValues.value = users.value?.map((user) => {
    return {
      ...user,
      role: user.userRole ? user.userRole.roleName : "Ylläpitäjä",
    };
  });
});

const userToBeDeleted = ref<User>();
const userToBeEdited = ref<User>();
const userForActionToBeSelected = ref<User>();

const deleteUserMutation = useMutation({
  mutationFn: () => {
    removeAction(ActionTypes.DELETING_USER);
    return userAPI.deleteUserById(userToBeDeleted.value!.id);
  },
  onSuccess: () => {
    toast.info("Käyttäjä poistettu");
    userQuery.refetch();
  },
  onError: () => {
    toast.error("Käyttäjän poistaminen epäonnistui");
  },
});

const startDeletingUser = (user: User) => {
  userToBeDeleted.value = user;
  pushAction(ActionTypes.DELETING_USER);
};

const startEditingUser = (user: User) => {
  userToBeEdited.value = user;
  pushAction(ActionTypes.EDITING_USER);
};

const startSelectingUserAction = (user: User) => {
  userForActionToBeSelected.value = user;
  if (isMobile.value) return pushAction(ActionTypes.SELECTING_USER_ACTION_MOBILE);
};

const userListItemRefs = reactive<Record<string, HTMLElement>>({});

onMounted(() => setCurrentRouteLabel("Käyttäjät"));
</script>

<template>
  <h3 v-if="false" class="m-5 fw-bold">{{ t("CatDetails.404") }}</h3>
  <Spinner v-if="userQuery.isLoading.value" />
  <div v-if="!userQuery.isLoading.value" style="min-height: 100%" class="d-flex flex-column p-3 p-sm-5 rounded col-12 col-lg-8 mx-auto">
    <h3 class="mb-3">{{ t("Users.members") }}</h3>
    <List :searchQueryPlaceholder="t('Users.searchInput')" v-if="users" :items="(translatedValues as any)" :itemsPerPage="20">
      <template v-slot="{ item: user }">
        <UserListItem :user="user">
          <template v-slot:actions>
            <button
              v-if="userHasPermission(PermissionTypes.RoleEventOrganizerWrite || PermissionTypes.RoleAdminWrite)"
              :ref="el => userListItemRefs[user.id] = el as HTMLElement"
              @keyup.enter.stop="startSelectingUserAction(user)"
              @click.stop="startSelectingUserAction(user)"
              tabindex="0"
              class="btn p-2 accordion d-flex focus-ring rounded-1 border-0"
              type="button"
            >
              <ThreeDotsIcon />
            </button>
            <Dropdown :visible="!isMobile" :triggerRef="userListItemRefs[user.id]" :placement="'left-start'">
              <li
                tabIndex="0"
                @keydown.enter.stop="startEditingUser(user)"
                @click="startEditingUser(user)"
                class="hover-bg-1 focus-ring px-3 py-2 rounded-2"
              >
                Muokkaa
              </li>
              <li
                tabIndex="0"
                @keydown.enter.stop="startDeletingUser(user)"
                @click="startDeletingUser(user)"
                class="hover-bg-1 focus-ring px-3 py-2 rounded-2"
              >
                Poista
              </li>
            </Dropdown>
          </template>
        </UserListItem>
      </template>
    </List>
  </div>
  <Drawer
    :visible="isCurrentAction(ActionTypes.SELECTING_USER_ACTION_MOBILE) && isMobile"
    @onCancel="removeAction(ActionTypes.SELECTING_USER_ACTION_MOBILE)"
  >
    <div v-if="isMobile" class="p-2 pb-5 gap-1 d-flex flex-column list-unstyled">
      <div
        tabindex="0"
        class="cursor-pointer fw-semibold px-3 p-2 hover-bg-1 rounded-3"
        v-if="userForActionToBeSelected"
        @click="startEditingUser(userForActionToBeSelected)"
      >
        Muokkaa roolia
      </div>

      <div
        tabIndex="0"
        v-if="userForActionToBeSelected"
        @click="startDeletingUser(userForActionToBeSelected)"
        class="cursor-pointer fw-semibold px-3 p-2 hover-bg-1 rounded-3"
        data-testid="start-cat-delete"
      >
        Poista {{ userForActionToBeSelected.givenName }}
      </div>
    </div>
  </Drawer>
  <Modal :visible="isCurrentAction(ActionTypes.EDITING_USER)" @onCancel="removeAction(ActionTypes.EDITING_USER)">
    <div style="width: 550px; max-width: 95vw" class="m-auto">
      <UserForm :formActionButtonText="'Tallenna tiedot'" :user="userToBeEdited" @onSave="removeAction(ActionTypes.EDITING_USER)" />
    </div>
  </Modal>
  <Modal :visible="isCurrentAction(ActionTypes.DELETING_USER)" @onCancel="removeAction(ActionTypes.DELETING_USER)">
    <div v-if="userToBeDeleted" style="width: 90vw; max-width: 500px" class="p-4 d-flex flex-column">
      <p>Poistetaanko käyttäjä? {{ userToBeDeleted.givenName }}</p>
      <div class="d-flex gap-2 justify-content-end">
        <button
          @keyup.enter="removeAction(ActionTypes.DELETING_USER)"
          type="button"
          class="btn btn-secondary"
          @click.stop="removeAction(ActionTypes.DELETING_USER)"
        >
          Peruuta
        </button>
        <button @click="deleteUserMutation.mutate" type="button" class="btn btn-danger">Poista</button>
      </div>
    </div>
  </Modal>
</template>
