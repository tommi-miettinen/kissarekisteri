import { reactive, computed } from "vue";

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
