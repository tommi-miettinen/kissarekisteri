<script lang="ts" setup>
import { ref, watch, defineProps, onUnmounted } from "vue";
import { Dropdown } from "bootstrap";
import { Placement } from "@popperjs/core/lib/enums";

const props = defineProps({
  triggerRef: HTMLElement,
  visible: {
    type: Boolean,
    default: true,
  },
  placement: {
    type: String as () => Placement,
    default: "bottom-start",
  },
});

let bootstrapDropdownInstance: Dropdown;
const dropdownMenuRef = ref<HTMLDivElement>();

const outsideClickListener = () => {
  bootstrapDropdownInstance.hide();
};

const handleTriggerClick = () => {
  bootstrapDropdownInstance.toggle();
};

const handleKeyup = (e: KeyboardEvent) => {
  if (e.key === "Enter" || e.key === "Escape") {
    bootstrapDropdownInstance.toggle();
  }
};

const addEventListeners = () => {
  document.addEventListener("click", outsideClickListener);
  props.triggerRef?.addEventListener("click", handleTriggerClick);
  props.triggerRef?.addEventListener("keyup", handleKeyup);
};

const removeEventListeners = () => {
  document.removeEventListener("click", outsideClickListener);
  props.triggerRef?.removeEventListener("click", handleTriggerClick);
  props.triggerRef?.removeEventListener("keyup", handleKeyup);
};

watch(
  () => props.triggerRef,
  (newVal, oldVal) => {
    if (!newVal) return;
    if (oldVal) {
      removeEventListeners(); // Remove listeners from the old element
    }
    bootstrapDropdownInstance = new Dropdown(newVal, {
      reference: newVal,
      autoClose: true,
      popperConfig: {
        placement: props.placement,
      },
    });

    addEventListeners();
    dropdownMenuRef.value?.focus();
  }
);

onUnmounted(() => {
  removeEventListeners();
});
</script>

<template>
  <div :class="{ invisible: !visible }" class="dropdown-menu" ref="dropdownMenuRef">
    <slot></slot>
  </div>
</template>
