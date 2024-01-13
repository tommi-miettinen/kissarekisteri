import { reactive, computed } from "vue";
import { useWindowSize } from "@vueuse/core";

interface ActionStore {
  actionStack: any[];
}

const actionStore = reactive<ActionStore>({
  actionStack: [],
});

export const pushAction = (action: any) => actionStore.actionStack.push(action);

export const popAction = () => actionStore.actionStack.pop();

export const actionStack = computed(() => actionStore.actionStack);

export const isCurrentAction = (actionType: any) => actionStack.value[actionStack.value.length - 1] === actionType;

export const resetActionStack = () => (actionStore.actionStack = []);

export const removeAction = (actionType: any) =>
  (actionStore.actionStack = actionStore.actionStack.filter((action) => action !== actionType));

export const isMobile = computed(() => useWindowSize().width.value < 768);
