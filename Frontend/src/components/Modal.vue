<script lang="ts" setup>
import { ref, onMounted, watch } from "vue";
import { Modal } from "bootstrap";
import { onUnmounted } from "vue";

const props = defineProps({
  visible: {
    type: Boolean,
    default: false,
    required: true,
  },
});

const modal = ref<Modal>();
const modalRef = ref<HTMLDivElement>();
const emit = defineEmits(["onCancel"]);

const close = () => emit("onCancel");

onMounted(() => {
  if (!modalRef.value) return console.error("Modal ref is null");

  modal.value = new Modal(modalRef.value);
  modalRef.value.addEventListener("hide.bs.modal", close);
});

watch(
  () => props.visible,
  (newVal) => (newVal ? modal.value?.show() : modal.value?.hide())
);

onUnmounted(() => modalRef.value?.removeEventListener("hide.bs.modal", close));
</script>

<template>
  <div ref="modalRef" class="modal fade" tabindex="-1">
    <div class="modal-dialog modal-dialog-centered">
      <div
        style="display: flex; flex-direction: column; justify-content: center; align-items: center; width: min-content"
        class="modal-content m-auto"
      >
        <slot></slot>
      </div>
    </div>
  </div>
</template>
