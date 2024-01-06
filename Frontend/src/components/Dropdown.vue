<script lang="ts" setup>
import { ref, defineProps, watch, onMounted, onBeforeUnmount } from "vue";
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

const emit = defineEmits(["onCancel"]);

const dropdown = ref<Dropdown>();
const dropdownContentRef = ref<HTMLDivElement>();

const closeDropdownIfClickedOutside = (event: MouseEvent) => {
  const target = event.target as HTMLElement;
  console.log("closing");
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
    props.triggerRef.onkeyup = (e) => e.key === "Escape" && dropdown.value?.toggle();
    props.triggerRef?.addEventListener("hide.bs.dropdown", () => emit("onCancel"));
  }
);

onMounted(() => {
  console.log("mounted");
  document.addEventListener("click", closeDropdownIfClickedOutside);
});

onBeforeUnmount(() => {
  document.removeEventListener("click", closeDropdownIfClickedOutside);
});
</script>

<template>
  <div :class="{ invisible: !props.visible }" ref="dropdownContentRef" tabIndex="0" class="dropdown-menu border p-1 rounded-3">
    <slot></slot>
  </div>
</template>
