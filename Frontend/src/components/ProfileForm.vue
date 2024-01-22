<script setup lang="ts">
import { defineProps, ref } from "vue";
import { userIsLoggedInUser } from "../store/userStore";

const props = defineProps({
  user: {
    type: Object as () => User,
    required: true,
  },
});

const formUser = ref({ ...props.user });
</script>

<template>
  <div class="p-4 d-flex flex-column gap-2">
    <h3 class="mb-4">Profiilin asetukset</h3>
    <div class="d-flex flex-column gap-2">
      <div class="form-check form-switch align-items-center p-0 d-flex gap-2">
        <label class="form-check-label me-auto" for="show-email">Näytä sähköposti</label>
        <input
          v-model="formUser.showEmail"
          style="height: 24px; width: 48px"
          class="form-check-input form-check form-switch"
          type="checkbox"
          role="switch"
          id="show-email"
        />
      </div>
      <div class="form-check form-switch align-items-center p-0 d-flex gap-2">
        <label class="cursor-pointer form-check-label me-auto" for="show-phone">Näytä puhelinnumero</label>
        <input
          v-model="formUser.showPhoneNumber"
          style="height: 24px; width: 48px"
          class="cursor-pointer form-check-input form-check form-switch"
          type="checkbox"
          role="switch"
          id="show-phone"
        />
      </div>
      <div class="form-check form-switch align-items-center p-0 d-flex gap-2">
        <label class="form-check-label me-auto" for="register-as-breeder">Rekisteröidy kasvattajaksi</label>
        <input
          v-model="formUser.isBreeder"
          style="height: 24px; width: 48px"
          class="form-check-input form-check form-switch"
          type="checkbox"
          role="switch"
          id="register-as-breeder"
        />
      </div>
      <div>
        <label for="phone" class="form-label w-100">Puhelin</label>
        <input v-model="formUser.phoneNumber" id="phone" data-testid="new-cat-birthdate-input" type="text" class="form-control" />
      </div>
    </div>
    <button
      tabIndex="0"
      @click="$emit('onSave', formUser)"
      v-if="userIsLoggedInUser(user)"
      class="btn ms-auto bg-black text-white focus-ring px-4 rounded-3 w-sm-100"
    >
      Tallenna muutokset
    </button>
  </div>
</template>
