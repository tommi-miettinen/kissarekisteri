<script lang="ts" setup>
import { ref, computed, watch } from "vue";
import { user as loggedInUser } from "../store/userStore";
import { toast } from "vue-sonner";
import userAPI from "../api/userAPI";
import Modal from "../components/Modal.vue";
import catAPI from "../api/catAPI";
import { useQuery, useMutation } from "@tanstack/vue-query";
import { useI18n } from "vue-i18n";
import CatListItem from "../components/CatListItem.vue";
import { useRoute } from "vue-router";
import CatForm from "../components/CatForm.vue";
import List from "../components/List.vue";
import Drawer from "../components/Drawer.vue";
import { useWindowSize } from "@vueuse/core";
import Dropdown from "../components/Dropdown.vue";
import { pushAction, isCurrentAction, removeAction } from "../store/actionStore";
import { QueryKeys } from "../api/queryKeys";
import ThreeDotsIcon from "../icons/ThreeDotsIcon.vue";

const { t } = useI18n();
const route = useRoute();

enum ActionType {
  NONE = "NONE",
  EDITING_CAT = "EDITING_CAT",
  EDITING_CAT_MOBILE = "EDITING_CAT_MOBILE",
  DELETING_CAT = "DELETING_CAT",
  ADDING_CAT = "ADDING_CAT",
  ADDING_CAT_MOBILE = "ADDING_CAT_MOBILE",
  EDITING_AVATAR = "EDITING_AVATAR",
  SELECTING_CAT_ACTION = "SELECTING_CAT_ACTION",
  SELECTING_CAT_ACTION_MOBILE = "SELECTING_CAT_ACTION_MOBILE",
}

const catToBeDeleted = ref<Cat>();
const catToBeEdited = ref<Cat>();
const catForActionToBeSelected = ref<Cat>();

const {
  data: user,
  isError: isUserError,
  refetch: refetchUser,
} = useQuery({
  queryKey: [QueryKeys.USER_BY_ID(route.params.userId as string)],
  queryFn: () => userAPI.getUserById(route.params.userId as string),
});

const userIsLoggedInUser = computed(() => {
  if (user.value && loggedInUser.value) return user.value.id === loggedInUser.value.id;
  return false;
});

const { data: catsData, refetch: refetchCats } = useQuery({
  queryKey: ["userCats"],
  queryFn: () => userAPI.getCatsByUserId(user.value?.id as string),
  enabled: Boolean(user.value?.id),
});

const cats = computed(() => catsData.value?.data);

const addCatMutation = useMutation({
  mutationFn: (newCatPayload: CatPayload) => catAPI.addCat(newCatPayload),
  onSuccess: () => {
    toast.success("Kissan tiedot lisätty");
    refetchCats();
    removeAction(ActionType.ADDING_CAT);
    removeAction(ActionType.ADDING_CAT_MOBILE);
  },
  onError: () => toast.error("Jokin meni vikaan."),
});

const deleteMutation = useMutation({
  mutationFn: () => catAPI.deleteCatById(catToBeDeleted.value!.id),
  onSuccess: () => {
    toast.success("Kissan tiedot poistettu");
    refetchCats();
    removeAction(ActionType.DELETING_CAT);
  },
  onError: () => toast.error("Jokin meni vikaan."),
});

watch([route, user], () => {
  refetchCats();
  refetchUser();
});

const width = useWindowSize().width;
const isMobile = computed(() => width.value < 768);

const editCat = async (updatedCat: EditCatPayload) => {
  await catAPI.editCat(updatedCat);
};

const catListItemRefs = ref<Record<number, HTMLElement>>({});

const startDeletingCat = (cat: Cat) => {
  catToBeDeleted.value = cat;
  pushAction(ActionType.DELETING_CAT);
};

const startSelectingCatAction = (cat: Cat) => {
  catForActionToBeSelected.value = cat;
  pushAction(isMobile.value ? ActionType.SELECTING_CAT_ACTION_MOBILE : ActionType.SELECTING_CAT_ACTION);
};

const startEditingCat = (cat: Cat) => {
  catToBeEdited.value = cat;
  pushAction(isMobile.value ? ActionType.EDITING_CAT_MOBILE : ActionType.EDITING_CAT);
};

const startAddingCat = () => {
  pushAction(isMobile.value ? ActionType.ADDING_CAT_MOBILE : ActionType.ADDING_CAT);
};
</script>

<template>
  <h3 v-if="isUserError" class="m-5 fw-bold">{{ t("Profile.404") }}</h3>
  <div style="min-height: 100%" class="d-flex flex-column p-2 p-sm-5 rounded col-12 col-lg-8 mx-auto gap-4">
    <div v-if="user" class="d-flex align-items-center gap-2">
      <h3 class="m-0">{{ `${user.givenName}  ${user.surname}` }}</h3>
    </div>

    <div class="d-flex flex-column rounded h-100 flex-grow-1">
      <h3 v-if="cats && cats.length > 0">{{ t("Profile.cats") }}</h3>
      <List :searchQueryPlaceholder="t('Cats.searchInput')" v-if="cats" :items="cats" :itemsPerPage="cats.length">
        <template v-slot="{ item: cat }">
          <CatListItem :key="cat.id" :cat="cat">
            <template #actions>
              <div
                :ref="(el) => (catListItemRefs[cat.id] = el as HTMLDivElement)"
                @keyup.enter.stop
                v-if="userIsLoggedInUser"
                data-testid="cat-options"
                @click.stop="startSelectingCatAction(cat)"
                class="d-flex"
              >
                <button tabindex="0" class="btn p-2 accordion d-flex focus-ring rounded-1 border-0" type="button">
                  <ThreeDotsIcon />
                </button>
                <Dropdown
                  @onCancel="removeAction(ActionType.SELECTING_CAT_ACTION)"
                  :visible="!isMobile"
                  :placement="'left-start'"
                  :triggerRef="catListItemRefs[cat.id]"
                >
                  <li
                    @keyup.enter.stop="startEditingCat(cat)"
                    @click="startEditingCat(cat)"
                    tabIndex="0"
                    class="dropdown-item focus-ring px-3 py-2 rounded-2 hover-bg"
                  >
                    Muokkaa
                  </li>
                  <li
                    tabIndex="0"
                    class="dropdown-item focus-ring px-3 py-2 rounded-2 hover-bg"
                    data-testid="start-cat-delete"
                    @keyup.enter="startDeletingCat(cat)"
                    @click="startDeletingCat(cat)"
                  >
                    Poista
                  </li>
                </Dropdown>
              </div>
            </template>
          </CatListItem>
        </template>
        <template #action>
          <button
            v-if="userIsLoggedInUser"
            @click.stop="startAddingCat"
            @keyup.enter.stop="startAddingCat"
            data-testid="add-new-cat-btn"
            type="button"
            class="btn btn-primary ms-auto px-5 rounded-3 w-sm-100"
          >
            {{ t("Profile.addCat") }} +
          </button>
        </template>
      </List>
    </div>
  </div>

  <Drawer
    :visible="isMobile && isCurrentAction(ActionType.SELECTING_CAT_ACTION_MOBILE)"
    @onCancel="removeAction(ActionType.SELECTING_CAT_ACTION_MOBILE)"
  >
    <div class="gap-2 p-2 d-flex flex-column list-unstyled">
      <li
        @keyup.enter="startEditingCat(catForActionToBeSelected!)"
        @click.stop="startEditingCat(catForActionToBeSelected!)"
        tabIndex="0"
        class="p-2 hover-bg rounded-3"
      >
        Muokkaa
      </li>
      <li
        tabIndex="0"
        data-testid="start-cat-delete"
        class="p-2 hover-bg rounded-3"
        @keyup.enter="startDeletingCat(catForActionToBeSelected!)"
        @click.stop="startDeletingCat(catForActionToBeSelected!)"
      >
        Poista
      </li>
    </div>
  </Drawer>
  <Drawer
    :fullsize="true"
    :visible="isCurrentAction(ActionType.ADDING_CAT_MOBILE) && isMobile"
    @onCancel="removeAction(ActionType.ADDING_CAT_MOBILE)"
  >
    <CatForm @onSave="addCatMutation.mutate" />
  </Drawer>
  <Modal :visible="isCurrentAction(ActionType.ADDING_CAT) && !isMobile" @onCancel="removeAction(ActionType.ADDING_CAT)">
    <div style="width: 550px">
      <CatForm @onSave="addCatMutation.mutate" />
    </div>
  </Modal>
  <Modal :visible="isCurrentAction(ActionType.EDITING_CAT) && !isMobile" @onCancel="removeAction(ActionType.EDITING_CAT)">
    <div style="width: 550px">
      <CatForm :cat="catToBeEdited" @onSave="editCat" />
    </div>
  </Modal>
  <Drawer
    :fullsize="true"
    :visible="isCurrentAction(ActionType.EDITING_CAT_MOBILE) && isMobile"
    @onCancel="removeAction(ActionType.EDITING_CAT_MOBILE)"
  >
    <CatForm :cat="catToBeEdited" @onSave="editCat" />
  </Drawer>
  <Modal @onCancel="removeAction(ActionType.DELETING_CAT)" :visible="isCurrentAction(ActionType.DELETING_CAT)">
    <div style="width: 90vw; max-width: 500px" class="p-4 d-flex flex-column">
      <p>Poistetaanko kissan tiedot?</p>
      <div class="d-flex gap-2 justify-content-end">
        <button type="button" class="btn btn-secondary" @click="removeAction(ActionType.DELETING_CAT)">Peruuta</button>
        <button data-testid="confirm-cat-delete" @click="deleteMutation.mutate" type="button" class="btn btn-danger">Poista</button>
      </div>
    </div>
  </Modal>
</template>
