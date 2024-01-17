import { reactive, computed } from "vue";
import { useWindowSize } from "@vueuse/core";

export enum ActionTypes {
  BOTTOM_SHEET = "BOTTOM_SHEET",
  SIDE_SHEET = "SIDE_SHEET",
  NOTIFICATIONS_MOBILE = "NOTIFICATIONS_MOBILE",
  EDITING_USER = "EDITING_USER",
  EDITING_USER_MOBILE = "EDITING_USER_MOBILE",
  DELETING_USER = "DELETING_USER",
  SELECTING_USER_ACTION_MOBILE = "SELECTING_USER_ACTION_MOBILE",
  FULLSCREEN_IMAGE = "FULLSCREEN_IMAGE",
  JOINING_EVENT = "JOINING_EVENT",
  JOINING_EVENT_MOBILE = "JOINING_EVENT_MOBILE",
  LEAVING_EVENT = "LEAVING_EVENT",
  SELECTING_CAT_ACTION = "SELECTING_CAT_ACTION",
  SELECTING_CAT_ACTION_MOBILE = "SELECTING_CAT_ACTION_MOBILE",
  EDITING_AVATAR = "EDITING_AVATAR",
  EDITING_AVATAR_MOBILE = "EDITING_AVATAR_MOBILE",
  EDITING_CAT = "EDITING_CAT",
  EDITING_CAT_MOBILE = "EDITING_CAT_MOBILE",
  DELETING_CAT = "DELETING_CAT",
  ADDING_CAT = "ADDING_CAT",
  ADDING_CAT_MOBILE = "ADDING_CAT_MOBILE",
}

interface ActionStore {
  actionStack: ActionTypes[];
}

const actionStore = reactive<ActionStore>({
  actionStack: [],
});

export const pushAction = (action: ActionTypes) => actionStore.actionStack.push(action);

export const popAction = () => actionStore.actionStack.pop();

export const actionStack = computed(() => actionStore.actionStack);

export const isCurrentAction = (actionType: ActionTypes) => actionStack.value[actionStack.value.length - 1] === actionType;

export const resetActionStack = () => (actionStore.actionStack = []);

export const removeAction = (actionType: ActionTypes) =>
  (actionStore.actionStack = actionStore.actionStack.filter((action) => action !== actionType));

export const isMobile = computed(() => useWindowSize().width.value < 768);
