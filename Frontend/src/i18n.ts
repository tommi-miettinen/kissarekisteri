import { createI18n } from "vue-i18n";
import en from "./i18n/en";
import fi from "./i18n/fi";

const i18n = createI18n({
  locale: "fi",
  legacy: false,
  messages: {
    en: en,
    fi: fi,
  },
});

export default i18n;
