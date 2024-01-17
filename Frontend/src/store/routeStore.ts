import { reactive, computed } from "vue";
import router from "../routes";

interface routeStore {
  currentRouteLabel: string;
}

const routeStore = reactive<routeStore>({
  currentRouteLabel: "Kissat",
});

export const currentRouteLabel = computed(() => routeStore.currentRouteLabel);

export const setCurrentRouteLabel = (label: string) => (routeStore.currentRouteLabel = label);

export const navigateTo = (route: string) => router.push(route);

export const navigateBack = () => router.back();
