<script lang="ts" setup>
import { onMounted, watch } from "vue";
import Modal from "bootstrap/js/dist/modal";

let modal: Modal | null = null;

const props = defineProps({
  visible: Boolean,
  modalId: String,
});

const emit = defineEmits(["onCancel"]);

onMounted(() => {
  const modalElement = document.getElementById(props.modalId!);
  if (modalElement) {
    modal = new Modal(modalElement);

    modalElement.addEventListener("hide.bs.modal", () => {
      emit("onCancel");
    });
  }
});

watch(
  () => props.visible,
  (newValue) => {
    if (!modal) return;
    if (newValue) {
      modal.show();
    } else {
      modal.hide();
    }
  }
);
</script>

<template>
  <div class="modal fade" tabindex="-1" :id="props.modalId">
    <div class="modal-dialog modal-dialog-centered">
      <div style="display: flex; flex-direction: column; justify-content: center; align-items: center" class="modal-content">
        <slot></slot>
      </div>
    </div>
  </div>
</template>
