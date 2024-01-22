<script lang="ts" setup>
import { ref } from "vue";
import { userIsLoggedInUser } from "../store/userStore";
import { toast } from "vue-sonner";
import userAPI from "../api/userAPI";
import Modal from "../components/Modal.vue";
import catAPI from "../api/catAPI";
import { useQuery, useMutation } from "@tanstack/vue-query";
import { useI18n } from "vue-i18n";
import CatListItem from "../components/CatListItem.vue";
import { useRoute } from "vue-router";
import CatForm from "../components/CatForm.vue";
import Drawer from "../components/Drawer.vue";
import Dropdown from "../components/Dropdown.vue";
import { ActionTypes, pushAction, isCurrentAction, removeAction } from "../store/actionStore";
import { QueryKeys } from "../api/queryKeys";
import ThreeDotsIcon from "../icons/ThreeDotsIcon.vue";
import { isMobile } from "../store/actionStore";
import UserInfoCard from "../components/UserInfoCard.vue";
import { setCurrentRouteLabel } from "../store/routeStore";
import Spinner from "../components/Spinner.vue";
import { watchEffect } from "vue";

const { t } = useI18n();
const route = useRoute();
const routeParamUserId = route.params.userId as string;

const catToBeDeleted = ref<Cat>();
const catToBeEdited = ref<Cat>();
const catForActionToBeSelected = ref<Cat>();

const {
  data: user,
  isError: isUserError,
  isFetched: isUserFetched,
  isLoading: isUserLoading,
} = useQuery({
  queryKey: QueryKeys.USER_BY_ID(routeParamUserId),
  queryFn: () => userAPI.getUserById(routeParamUserId),
});

const { data: cats, refetch: refetchCats } = useQuery({
  queryKey: QueryKeys.USERS_CATS_BY_ID(routeParamUserId),
  queryFn: () => userAPI.getCatsByUserId(routeParamUserId),
});

const addCatMutation = useMutation({
  mutationFn: (newCatPayload: CatPayload) => catAPI.addCat(newCatPayload),
  onSuccess: () => {
    toast.success("Kissan tiedot lisätty");
    refetchCats();
    removeAction(ActionTypes.ADDING_CAT);
    removeAction(ActionTypes.ADDING_CAT_MOBILE);
  },
  onError: () => toast.error("Jokin meni vikaan."),
});

const deleteMutation = useMutation({
  mutationFn: () => catAPI.deleteCatById(catToBeDeleted.value!.id),
  onSuccess: () => {
    toast.success("Kissan tiedot poistettu");
    refetchCats();
    removeAction(ActionTypes.DELETING_CAT);
  },
  onError: () => toast.error("Jokin meni vikaan."),
});

watchEffect(() => user.value && setCurrentRouteLabel(user.value.givenName));

const editCat = (updatedCat: EditCatPayload) => catAPI.editCat(updatedCat);

const catListItemRefs = ref<Record<number, HTMLElement>>({});

const startDeletingCat = (cat: Cat) => {
  catToBeDeleted.value = cat;
  pushAction(ActionTypes.DELETING_CAT);
};

const startSelectingCatAction = (cat: Cat) => {
  catForActionToBeSelected.value = cat;
  pushAction(isMobile.value ? ActionTypes.SELECTING_CAT_ACTION_MOBILE : ActionTypes.SELECTING_CAT_ACTION);
};

const startEditingCat = (cat: Cat) => {
  catToBeEdited.value = cat;
  pushAction(isMobile.value ? ActionTypes.EDITING_CAT_MOBILE : ActionTypes.EDITING_CAT);
};

const startAddingCat = () => pushAction(isMobile.value ? ActionTypes.ADDING_CAT_MOBILE : ActionTypes.ADDING_CAT);
</script>

<template>
  <h3 v-if="isUserError && isUserFetched" class="m-5 fw-bold">{{ t("Profile.404") }}</h3>
  <Spinner v-if="isUserLoading" />
  <div v-if="!isUserLoading" style="min-height: 100%" class="d-flex flex-column px-3 py-2 p-sm-5 rounded col-12 col-lg-8 mx-auto gap-3">
    <UserInfoCard v-if="user" :user="user" />
    <div class="d-flex flex-column rounded h-100 flex-grow-1">
      <h3 v-if="cats && cats.length > 0">{{ t("Profile.cats") }}</h3>
      <CatListItem v-for="cat in cats" :key="cat.id" :cat="cat">
        <template #actions>
          <div
            :ref="(el) => (catListItemRefs[cat.id] = el as HTMLDivElement)"
            @keyup.enter.stop
            v-if="userIsLoggedInUser(user)"
            data-testid="cat-options"
            @click.stop="startSelectingCatAction(cat)"
            class="d-flex"
          >
            <button tabIndex="0" class="btn p-2 accordion d-flex focus-ring rounded-1 border-0">
              <ThreeDotsIcon />
            </button>
            <Dropdown
              @onCancel="removeAction(ActionTypes.SELECTING_CAT_ACTION)"
              :visible="!isMobile"
              :placement="'left-start'"
              :triggerRef="catListItemRefs[cat.id]"
            >
              <li
                @keyup.enter.stop="startEditingCat(cat)"
                @click="startEditingCat(cat)"
                tabIndex="0"
                class="hover-bg-1 focus-ring px-3 py-2 rounded-2"
              >
                {{ t("Buttons.edit") }}
              </li>
              <li
                tabIndex="0"
                class="hover-bg-1 focus-ring px-3 py-2 rounded-2"
                data-testid="start-cat-delete"
                @keyup.enter="startDeletingCat(cat)"
                @click="startDeletingCat(cat)"
              >
                {{ t("Buttons.delete") }}
              </li>
            </Dropdown>
          </div>
        </template>
      </CatListItem>
      <button
        v-if="userIsLoggedInUser(user)"
        @click.stop="startAddingCat"
        @keyup.enter.stop="startAddingCat"
        data-testid="add-new-cat-btn"
        type="button"
        class="mt-2 btn bg-black text-white focus-ring ms-auto py-2 px-5 rounded-3 w-sm-100"
      >
        {{ t("Profile.addCat") }} +
      </button>
    </div>
  </div>

  <Drawer
    :visible="isMobile && isCurrentAction(ActionTypes.SELECTING_CAT_ACTION_MOBILE)"
    @onCancel="removeAction(ActionTypes.SELECTING_CAT_ACTION_MOBILE)"
  >
    <div class="p-2 pb-5 gap-1 d-flex flex-column list-unstyled">
      <li
        @keyup.enter="startEditingCat(catForActionToBeSelected!)"
        @click.stop="startEditingCat(catForActionToBeSelected!)"
        tabIndex="0"
        class="cursor-pointer fw-semibold px-3 p-2 hover-bg-1 rounded-3"
      >
        {{ t("Buttons.edit") }}
      </li>
      <li
        tabIndex="0"
        data-testid="start-cat-delete"
        class="cursor-pointer fw-semibold px-3 p-2 hover-bg-1 rounded-3"
        @keyup.enter="startDeletingCat(catForActionToBeSelected!)"
        @click.stop="startDeletingCat(catForActionToBeSelected!)"
      >
        {{ t("Buttons.delete") }}
      </li>
    </div>
  </Drawer>
  <Drawer
    :fullsize="true"
    :visible="isCurrentAction(ActionTypes.ADDING_CAT_MOBILE) && isMobile"
    @onCancel="removeAction(ActionTypes.ADDING_CAT_MOBILE)"
  >
    <div style="min-height: 92vh">
      <CatForm @onSave="addCatMutation.mutate" />
    </div>
  </Drawer>
  <Modal :visible="isCurrentAction(ActionTypes.ADDING_CAT) && !isMobile" @onCancel="removeAction(ActionTypes.ADDING_CAT)">
    <div style="width: 550px">
      <CatForm @onSave="addCatMutation.mutate" />
    </div>
  </Modal>
  <Modal :visible="isCurrentAction(ActionTypes.EDITING_CAT) && !isMobile" @onCancel="removeAction(ActionTypes.EDITING_CAT)">
    <div style="width: 550px">
      <CatForm :cat="catToBeEdited" @onSave="editCat" />
    </div>
  </Modal>
  <Drawer
    :fullsize="true"
    :visible="isCurrentAction(ActionTypes.EDITING_CAT_MOBILE) && isMobile"
    @onCancel="removeAction(ActionTypes.EDITING_CAT_MOBILE)"
  >
    <div class="height:100vh">
      <CatForm :cat="catToBeEdited" @onSave="editCat" />
    </div>
  </Drawer>
  <Modal @onCancel="removeAction(ActionTypes.DELETING_CAT)" :visible="isCurrentAction(ActionTypes.DELETING_CAT)">
    <div style="width: 90vw; max-width: 500px" class="p-4 d-flex flex-column">
      <p>Poistetaanko kissan tiedot?</p>
      <div class="d-flex gap-2 justify-content-end">
        <button type="button" class="btn border px-4" @click="removeAction(ActionTypes.DELETING_CAT)">Peruuta</button>
        <button data-testid="confirm-cat-delete" @click="deleteMutation.mutate" type="button" class="btn bg-black text-white px-4">
          Poista
        </button>
      </div>
    </div>
  </Modal>
</template>
