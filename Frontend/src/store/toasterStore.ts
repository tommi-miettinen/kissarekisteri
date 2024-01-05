import { ref } from "vue";

export enum ToastPositionOptions {
  TOP_LEFT = "top-left",
  TOP_RIGHT = "top-right",
  BOTTOM_LEFT = "bottom-left",
  BOTTOM_RIGHT = "bottom-right",
  TOP_CENTER = "top-center",
  BOTTOM_CENTER = "bottom-center",
}

export const toastPosition = ref(ToastPositionOptions.TOP_RIGHT);

export const setToastPosition = (position: ToastPositionOptions) => (toastPosition.value = position);
