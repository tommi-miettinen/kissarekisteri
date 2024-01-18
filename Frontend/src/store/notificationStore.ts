import { reactive, computed } from "vue";

interface NotificationStore {
  currentTab: "personal" | "admin";
}

const notificationStore = reactive<NotificationStore>({
  currentTab: "personal",
});

export const setCurrentNotificationTab = (tab: "personal" | "admin") => {
  notificationStore.currentTab = tab;
};

export const currentNotificationTab = computed(() => notificationStore.currentTab);
