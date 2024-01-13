<script lang="ts" setup>
import { ref, watch } from "vue";
import { Dropdown } from "bootstrap";
import { Placement } from "@popperjs/core/lib/enums";
import { onClickOutside } from "@vueuse/core";

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
  placement: {
    type: String as () => Placement,
    default: "bottom-start",
  },
});

const emit = defineEmits(["onCancel"]);

const dropdown = ref<Dropdown>();
const dropdownContentRef = ref<HTMLDivElement>();

const closeDropdownIfClickedOutside = (event: MouseEvent) => {
  const target = event.target as HTMLElement;
  if (!dropdownContentRef.value?.contains(target) && !props.triggerRef?.contains(target)) {
    dropdown.value?.hide();
  }
};

watch(
  () => props.triggerRef,
  () => {
    if (!props.triggerRef) return console.error("Trigger ref is null");
    if (dropdown.value) return console.log("Dropdown already initialized");

    dropdown.value = new Dropdown(props.triggerRef!, {
      reference: props.triggerRef,
      popperConfig: {
        placement: props.placement,
      },
    });

    props.triggerRef.onclick = () => dropdown.value?.toggle();
    props.triggerRef?.addEventListener("hide.bs.dropdown", () => emit("onCancel"));
    document.addEventListener("keyup", (e) => e.key === "Escape" && dropdown.value?.hide());
    onClickOutside(dropdownContentRef, closeDropdownIfClickedOutside);
  }
);
</script>

<template>
  <div
    @click.stop="props.autoClose && dropdown?.hide()"
    :class="{ invisible: !props.visible }"
    ref="dropdownContentRef"
    class="dropdown-menu border p-1 rounded-3"
    tabindex="-1"
  >
    <slot></slot>
  </div>
</template>
