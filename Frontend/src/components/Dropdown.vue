<script lang="ts" setup>
import { ref, watch } from "vue";
import { Dropdown } from "bootstrap";
import { Placement } from "@popperjs/core/lib/enums";
import { onClickOutside, useFocusWithin, useEventListener } from "@vueuse/core";

const props = defineProps({
  autoClose: {
    type: Boolean,
    default: true,
  },
  triggerRef: HTMLElement,
  visible: {
    type: Boolean,
    default: true,
  },
  closeOnFocusLost: {
    type: Boolean,
    default: true,
  },
  closeOnOutsideClick: {
    type: Boolean,
    default: true,
  },
  placement: {
    type: String as () => Placement,
    default: "bottom-start",
  },
});

const emit = defineEmits(["onCancel"]);

const dropdown = ref<Dropdown>();
const dropdownContentRef = ref<HTMLDivElement>();
const { focused } = useFocusWithin(dropdownContentRef);
const hasBeenFocused = ref(false);

const handleDropdownClose = (event: MouseEvent | KeyboardEvent) => {
  const target = event.target as HTMLElement;
  const isEscapeKey = event instanceof KeyboardEvent && event.key === "Escape";
  const isInsideDropdown = dropdownContentRef.value?.contains(target);
  const isInsideTrigger = props.triggerRef?.contains(target);

  if (isEscapeKey) {
    return dropdown.value?.hide();
  }

  if (!props.closeOnOutsideClick) return;

  if (!isInsideDropdown && !isInsideTrigger) {
    return dropdown.value?.hide();
  }
};

watch(
  () => props.triggerRef,
  () => {
    if (!props.triggerRef || dropdown.value) return;

    dropdown.value = new Dropdown(props.triggerRef!, {
      reference: props.triggerRef,
      popperConfig: {
        placement: props.placement,
      },
    });

    props.triggerRef.onclick = () => dropdown.value?.toggle();
    props.triggerRef?.addEventListener("hide.bs.dropdown", () => emit("onCancel"));

    useEventListener(document, "keyup", handleDropdownClose);
    onClickOutside(dropdownContentRef, handleDropdownClose);
  },
  { immediate: true }
);

watch(focused, (isFocused) => {
  if (!props.closeOnFocusLost) return;
  if (isFocused) hasBeenFocused.value = true;
  if (!isFocused && hasBeenFocused.value) dropdown.value?.hide();
});
</script>

<template>
  <div
    @click.stop="props.autoClose && dropdown?.hide()"
    @keyup.enter.stop="props.autoClose && dropdown?.hide()"
    :class="{ invisible: !props.visible }"
    ref="dropdownContentRef"
    class="dropdown-menu border p-1 rounded-3"
    tabindex="-1"
  >
    <slot></slot>
  </div>
</template>
