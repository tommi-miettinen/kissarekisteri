<script lang="ts" setup>
import { ref, computed, reactive } from "vue";
import userAPI from "../api/userAPI";
import UserListItem from "../components/UserListItem.vue";
import { useQuery } from "@tanstack/vue-query";
import { useI18n } from "vue-i18n";
import List from "../components/List.vue";
import Drawer from "../components/Drawer.vue";
import Modal from "../components/Modal.vue";
import { useWindowSize } from "@vueuse/core";
import UserForm from "../components/UserForm.vue";
import Dropdown from "../components/Dropdown.vue";
import { ActionTypes, pushAction, isCurrentAction, removeAction } from "../store/actionStore";
import { toast } from "vue-sonner";
import { useMutation } from "@tanstack/vue-query";
import { QueryKeys } from "../api/queryKeys";
import Spinner from "../components/Spinner.vue";
import ThreeDotsIcon from "../icons/ThreeDotsIcon.vue";

const { t } = useI18n();

const userQuery = useQuery({
  queryKey: QueryKeys.USERS,
  queryFn: userAPI.getUsers,
});

const users = computed(() => userQuery.data.value);

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

const isMobile = computed(() => useWindowSize().width.value < 768);

const startDeletingUser = (user: User) => {
  userToBeDeleted.value = user;
  pushAction(ActionTypes.DELETING_USER);
};

const startEditingUser = (user: User) => {
  userToBeEdited.value = user;
  pushAction(isMobile.value ? ActionTypes.EDITING_USER_MOBILE : ActionTypes.EDITING_USER);
};

const startSelectingUserAction = (user: User) => {
  userForActionToBeSelected.value = user;
  if (isMobile.value) return pushAction(ActionTypes.SELECTING_USER_ACTION_MOBILE);
};

const userListItemRefs = reactive<Record<string, HTMLElement>>({});
</script>

<template>
  <h3 v-if="false" class="m-5 fw-bold">{{ t("CatDetails.404") }}</h3>
  <Spinner v-if="userQuery.isLoading.value" />
  <div v-if="!userQuery.isLoading.value" style="min-height: 100%" class="d-flex flex-column p-2 p-sm-5 rounded col-12 col-lg-8 mx-auto">
    <h3 class="m-0">{{ t("Users.members") }}</h3>
    <List :searchQueryPlaceholder="t('Users.searchInput')" v-if="users" :items="users" :itemsPerPage="20">
      <template v-slot="{ item: user }">
        <UserListItem :user="user">
          <template v-slot:actions>
            <button
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
    <div v-if="isMobile" class="p-2">
      <div
        tabindex="0"
        class="hover-bg-1 rounded-3 p-2 focus-ring"
        v-if="userForActionToBeSelected"
        @click="startEditingUser(userForActionToBeSelected)"
      >
        {{ userForActionToBeSelected.givenName }}
      </div>

      <div
        tabIndex="0"
        v-if="userForActionToBeSelected"
        @click="startDeletingUser(userForActionToBeSelected)"
        class="hover-bg-1 rounded-3 p-2 focus-ring"
        data-testid="start-cat-delete"
      >
        Poista
      </div>
    </div>
  </Drawer>
  <Modal :visible="isCurrentAction(ActionTypes.EDITING_USER) && !isMobile" @onCancel="removeAction(ActionTypes.EDITING_USER)">
    <div style="width: 550px">
      <UserForm :formActionButtonText="'Tallenna tiedot'" :user="userToBeEdited" @onSave="removeAction(ActionTypes.EDITING_USER)" />
    </div>
  </Modal>
  <Drawer
    :fullsize="true"
    :visible="isCurrentAction(ActionTypes.EDITING_USER_MOBILE) && isMobile"
    @onCancel="removeAction(ActionTypes.EDITING_USER_MOBILE)"
  >
    <UserForm :formActionButtonText="'Tallenna tiedot'" :user="userToBeEdited" @onSave="removeAction(ActionTypes.EDITING_USER_MOBILE)" />
  </Drawer>
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
