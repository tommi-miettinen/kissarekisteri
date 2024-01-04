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
import { popAction, pushAction, isCurrentAction } from "../store/actionStore";

const { t } = useI18n();

enum ActionType {
  NONE,
  EDITING_USER,
  EDITING_USER_MOBILE,
  ADDING_USER_MOBILE,
  ADDING_USER,
  DELETING_USER,
}

const toggleAction = (actionType: ActionType, item = null) => {
  if (actionType === ActionType.NONE) {
    popAction();
  }
  if (actionType !== ActionType.NONE) {
    pushAction(actionType);
  }
  if (currentAction.value === actionType && currentItem.value === item) {
    currentAction.value = ActionType.NONE;
    currentItem.value = null;
  } else {
    currentAction.value = actionType;
    currentItem.value = item;
  }
};

const currentAction = ref<ActionType>(ActionType.NONE);
const currentItem = ref<any>(null);

const { data } = useQuery({
  queryKey: ["users"],
  queryFn: userAPI.getUsers,
});

const addingUser = ref(false);
const isMobile = computed(() => useWindowSize().width.value < 768);

const userListItemRefs = reactive<Record<string, HTMLElement>>({});
</script>

<template>
  <div class="w-100 h-100 p-0 p-sm-5 d-flex flex-column align-items-center">
    <div class="p-2 p-sm-5 rounded col-12 col-lg-8 h-100 d-flex flex-column">
      <h3>{{ t("Users.members") }}</h3>
      <List :searchQueryPlaceholder="t('Users.searchInput')" v-if="data" :items="data" :itemsPerPage="7">
        <template v-slot="{ item: user }">
          <UserListItem :user="user">
            <template v-slot:actions>
              <div @click.stop :ref="el => userListItemRefs[user.id] = el as HTMLElement">
                <button tabindex="0" class="btn py-1 px-2 accordion d-flex focus-ring rounded-1" type="button">
                  <svg xmlns="http://www.w3.org/2000/svg" height="1em" viewBox="0 0 128 512">
                    <path
                      d="M64 360a56 56 0 1 0 0 112 56 56 0 1 0 0-112zm0-160a56 56 0 1 0 0 112 56 56 0 1 0 0-112zM120 96A56 56 0 1 0 8 96a56 56 0 1 0 112 0z"
                    />
                  </svg>
                </button>
                <Dropdown :triggerRef="userListItemRefs[user.id]" :placement="'left-start'">
                  <li tabIndex="0" class="dropdown-item">Muokkaa</li>
                  <li
                    @click="toggleAction(ActionType.DELETING_USER, user.id)"
                    tabIndex="0"
                    class="dropdown-item"
                    data-testid="start-cat-delete"
                  >
                    Poista
                  </li>
                </Dropdown>
              </div>
            </template>
          </UserListItem>
        </template>
        <template #action>
          <button @click="toggleAction(isMobile ? ActionType.ADDING_USER_MOBILE : ActionType.ADDING_USER)" class="btn btn-primary">
            Lisää käyttäjä +
          </button>
        </template>
      </List>
    </div>
  </div>
  <Modal :visible="isCurrentAction(ActionType.ADDING_USER) && !isMobile" @onCancel="toggleAction(ActionType.NONE)">
    <div style="width: 550px">
      <UserForm @onSave="addingUser = false" />
    </div>
  </Modal>
  <Drawer :fullsize="true" :visible="isCurrentAction(ActionType.ADDING_USER_MOBILE) && isMobile" @onCancel="toggleAction(ActionType.NONE)">
    <UserForm @onSave="addingUser = false" />
  </Drawer>
  <Modal :visible="isCurrentAction(ActionType.DELETING_USER)" @onCancel="toggleAction(ActionType.NONE)">
    <div style="width: 90vw; max-width: 500px" class="p-4 d-flex flex-column">
      <p>Poistetaanko käyttäjä?</p>
      <div class="d-flex gap-2 justify-content-end">
        <button type="button" class="btn btn-secondary" @click="toggleAction(ActionType.NONE)">Peruuta</button>
        <button data-testid="confirm-cat-delete" type="button" class="btn btn-danger">Poista</button>
      </div>
    </div>
  </Modal>
</template>
