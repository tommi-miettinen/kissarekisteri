<script lang="ts" setup>
import { currentRouteLabel, navigateBack } from "../store/routeStore";
import { ActionTypes, pushAction, removeAction, isCurrentAction } from "../store/actionStore";
import { isMobile } from "../store/actionStore";
import Drawer from "./Drawer.vue";
import { useI18n } from "vue-i18n";
import moment from "moment";
import { login, logout } from "../auth";
import { user } from "../store/userStore";

const { locale } = useI18n();

const handleLocaleClick = (localeOption: "en" | "fi") => {
  locale.value = localeOption;
  moment.locale(locale.value);
  removeAction(ActionTypes.SELECTING_LANGUAGE);
};

const isCurrentLocale = (localeOption: "en" | "fi") => locale.value === localeOption;
</script>

<template>
  <div class="bg-white border-bottom w-100 align-items-center d-flex py-3 px-2 gap-2">
    <button @click="navigateBack" class="border-0 d-flex bg-transparent">
      <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24">
        <path fill="none" d="M0 0H24V24H0z"></path>
        <path d="M17.77 3.77L16 2 6 12 16 22 17.77 20.23 9.54 12z"></path>
      </svg>
    </button>
    <div class="fw-semibold">{{ currentRouteLabel }}</div>
    <button @click="pushAction(ActionTypes.SETTINGS_MOBILE)" class="bg-transparent border-0 ms-auto align-items-center d-flex">
      <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24">
        <path fill="none" d="M0 0h24v24H0z"></path>
        <path
          d="M12 8c1.1 0 2-.9 2-2s-.9-2-2-2-2 .9-2 2 .9 2 2 2zm0 2c-1.1 0-2 .9-2 2s.9 2 2 2 2-.9 2-2-.9-2-2-2zm0 6c-1.1 0-2 .9-2 2s.9 2 2 2 2-.9 2-2-.9-2-2-2z"
        ></path>
      </svg>
    </button>
  </div>
  <Drawer :visible="isCurrentAction(ActionTypes.SELECTING_LANGUAGE) && isMobile" @onCancel="removeAction(ActionTypes.SELECTING_LANGUAGE)">
    <div style="height: 30vh" class="d-flex flex-column p-2 gap-1">
      <div
        @click="handleLocaleClick('fi')"
        :class="{ 'bg-1': isCurrentLocale('fi') }"
        class="d-flex align-items-center gap-2 cursor-pointer fw-semibold px-3 p-2 hover-bg-1 rounded-3"
      >
        <input :checked="isCurrentLocale('fi')" class="form-check-input focus-ring focus-ring-dark" type="checkbox" />
        Suomeksi
      </div>
      <div
        @click="handleLocaleClick('en')"
        :class="{ 'bg-1': isCurrentLocale('en') }"
        class="d-flex align-items-center gap-2 cursor-pointer fw-semibold px-3 p-2 hover-bg-1 rounded-3"
      >
        <input :checked="isCurrentLocale('en')" class="form-check-input focus-ring focus-ring-dark" type="checkbox" />
        English
      </div>
    </div>
  </Drawer>
  <Drawer :visible="isCurrentAction(ActionTypes.SETTINGS_MOBILE) && isMobile" @onCancel="removeAction(ActionTypes.SETTINGS_MOBILE)">
    <div style="height: 30vh" class="d-flex flex-column p-2 gap-1">
      <div
        @click="pushAction(ActionTypes.SELECTING_LANGUAGE)"
        class="d-flex align-items-center gap-2 cursor-pointer fw-semibold px-3 p-2 hover-bg-1 rounded-3"
      >
        Valitse kieli
      </div>
      <div v-if="!user" @click="login" class="d-flex align-items-center gap-2 cursor-pointer fw-semibold px-3 p-2 hover-bg-1 rounded-3">
        Kirjaudu sisään
      </div>
      <div v-if="user" @click="logout" class="d-flex align-items-center gap-2 cursor-pointer fw-semibold px-3 p-2 hover-bg-1 rounded-3">
        Kirjaudu ulos
      </div>
    </div>
  </Drawer>
</template>
