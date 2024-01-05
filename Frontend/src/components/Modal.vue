<script lang="ts" setup>
import { ref, onMounted, watch } from "vue";
import { Modal } from "bootstrap";

const props = defineProps({
  visible: Boolean,
});

const modal = ref<Modal>();
const modalRef = ref<HTMLDivElement>();
const emit = defineEmits(["onCancel"]);

onMounted(() => {
  if (!modalRef.value) return console.error("Modal ref is null");

  modal.value = new Modal(modalRef.value);
  modalRef.value.addEventListener("hide.bs.modal", () => emit("onCancel"));
});

watch(
  () => props.visible,
  (newValue) => {
    if (newValue) {
      modal.value?.show();
    } else {
      modal.value?.hide();
    }
  }
);
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
