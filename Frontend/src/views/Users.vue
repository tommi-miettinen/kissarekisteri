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
import { popAction, pushAction, isCurrentAction, removeAction } from "../store/actionStore";
import { toast } from "vue-sonner";
import { useMutation } from "@tanstack/vue-query";

const { t } = useI18n();

enum ActionType {
  NONE = "NONE",
  EDITING_USER = "EDITING_USER",
  EDITING_USER_MOBILE = "EDITING_USER_MOBILE",
  ADDING_USER_MOBILE = "ADDING_USER_MOBILE",
  ADDING_USER = "ADDING_USER",
  DELETING_USER = "DELETING_USER",
  SELECTING_USER_ACTION = "SELECTING_USER_ACTION",
  SELECTING_USER_ACTION_MOBILE = "SELECTING_USER_ACTION_MOBILE",
}

const toggleAction = (actionType: ActionType) => {
  if (actionType === ActionType.NONE) {
    popAction();
  }
  if (actionType !== ActionType.NONE && !isCurrentAction(actionType)) {
    pushAction(actionType);
  }
};

const userQuery = useQuery({
  queryKey: ["users"],
  queryFn: userAPI.getUsers,
});

const users = computed(() => userQuery.data.value);

const userToBeDeleted = ref<User>();
const userToBeEdited = ref<User>();
const userForActionToBeSelected = ref<User>();

const deleteUserMutation = useMutation({
  mutationFn: () => userAPI.deleteUserById(userToBeDeleted.value!.id),
  onSuccess: () => {
    toast.info("Käyttäjä poistettu");
    userQuery.refetch();
    removeAction(ActionType.DELETING_USER);
  },
  onError: (error) => {
    console.log(error);
    toast.error("Käyttäjän poistaminen epäonnistui");
    removeAction(ActionType.DELETING_USER);
  },
});

const createUserMutation = useMutation({
  mutationFn: (user: UserPayload) => userAPI.createUser(user),
  onSuccess: () => {
    toast.info("Käyttäjä lisätty");
    userQuery.refetch();
    removeAction(ActionType.ADDING_USER_MOBILE);
    removeAction(ActionType.ADDING_USER);
  },
  onError: () => {
    removeAction(ActionType.ADDING_USER_MOBILE);
    removeAction(ActionType.ADDING_USER);
  },
});

const isMobile = computed(() => useWindowSize().width.value < 768);

const startDeletingUser = (user: User) => {
  userToBeDeleted.value = user;
  toggleAction(ActionType.DELETING_USER);
};

const startEditingUser = (user: User) => {
  userToBeEdited.value = user;
  toggleAction(isMobile.value ? ActionType.EDITING_USER_MOBILE : ActionType.EDITING_USER);
};

const startSelectingUserAction = (user: User) => {
  userForActionToBeSelected.value = user;
  toggleAction(isMobile.value ? ActionType.SELECTING_USER_ACTION_MOBILE : ActionType.SELECTING_USER_ACTION);
};

const userListItemRefs = reactive<Record<string, HTMLElement>>({});
</script>

<template>
  <div style="min-height: 100%" class="d-flex flex-column p-2 p-sm-5 rounded col-12 col-lg-8 mx-auto">
    <h3 class="m-0">{{ t("Users.members") }}</h3>
    <List :searchQueryPlaceholder="t('Users.searchInput')" v-if="users" :items="users" :itemsPerPage="20">
      <template v-slot="{ item: user }">
        <UserListItem :user="user">
          <template v-slot:actions>
            <button
              @keyup.enter.stop="startSelectingUserAction(user)"
              @click.stop="startSelectingUserAction(user)"
              :ref="el => userListItemRefs[user.id] = el as HTMLElement"
              tabindex="0"
              class="btn p-2 accordion d-flex focus-ring rounded-1 border-0"
              type="button"
            >
              <svg xmlns="http://www.w3.org/2000/svg" height="1em" viewBox="0 0 128 512">
                <path
                  d="M64 360a56 56 0 1 0 0 112 56 56 0 1 0 0-112zm0-160a56 56 0 1 0 0 112 56 56 0 1 0 0-112zM120 96A56 56 0 1 0 8 96a56 56 0 1 0 112 0z"
                />
              </svg>
            </button>
            <Dropdown
              @onCancel="removeAction(ActionType.SELECTING_USER_ACTION)"
              :visible="!isMobile"
              :triggerRef="userListItemRefs[user.id]"
              :placement="'left-start'"
            >
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
      <template #action>
        <button
          @click="toggleAction(isMobile ? ActionType.ADDING_USER_MOBILE : ActionType.ADDING_USER)"
          class="btn bg-black text-white rounded-3 px-5 ms-auto w-sm-100"
        >
          Lisää käyttäjä +
        </button>
      </template>
    </List>
  </div>
  <Drawer
    :visible="isCurrentAction(ActionType.SELECTING_USER_ACTION_MOBILE) && isMobile"
    @onCancel="removeAction(ActionType.SELECTING_USER_ACTION_MOBILE)"
  >
    <div class="p-2">
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
  <Modal :visible="isCurrentAction(ActionType.ADDING_USER) && !isMobile" @onCancel="removeAction(ActionType.ADDING_USER)">
    <div style="width: 550px">
      <UserForm :formActionButtonText="'Lisää käyttäjä +'" @onSave="createUserMutation.mutate" />
    </div>
  </Modal>
  <Drawer
    :fullsize="false"
    :visible="isCurrentAction(ActionType.ADDING_USER_MOBILE) && isMobile"
    @onCancel="removeAction(ActionType.ADDING_USER_MOBILE)"
  >
    <UserForm :formActionButtonText="'Lisää käyttäjä +'" @onSave="createUserMutation.mutate" />
  </Drawer>
  <Modal :visible="isCurrentAction(ActionType.EDITING_USER) && !isMobile" @onCancel="removeAction(ActionType.EDITING_USER)">
    <div style="width: 550px">
      <UserForm :formActionButtonText="'Tallenna tiedot'" :user="userToBeEdited" @onSave="removeAction(ActionType.EDITING_USER)" />
    </div>
  </Modal>
  <Drawer
    :fullsize="true"
    :visible="isCurrentAction(ActionType.EDITING_USER_MOBILE) && isMobile"
    @onCancel="removeAction(ActionType.EDITING_USER_MOBILE)"
  >
    <UserForm :formActionButtonText="'Tallenna tiedot'" :user="userToBeEdited" @onSave="removeAction(ActionType.EDITING_USER_MOBILE)" />
  </Drawer>
  <Modal :visible="isCurrentAction(ActionType.DELETING_USER)" @onCancel="removeAction(ActionType.DELETING_USER)">
    <div v-if="userToBeDeleted" style="width: 90vw; max-width: 500px" class="p-4 d-flex flex-column">
      <p>Poistetaanko käyttäjä? {{ userToBeDeleted.givenName }}</p>
      <div class="d-flex gap-2 justify-content-end">
        <button
          @keyup.enter="removeAction(ActionType.DELETING_USER)"
          type="button"
          class="btn btn-secondary"
          @click.stop="removeAction(ActionType.DELETING_USER)"
        >
          Peruuta
        </button>
        <button @click="deleteUserMutation.mutate" type="button" class="btn btn-danger">Poista</button>
      </div>
    </div>
  </Modal>
</template>
