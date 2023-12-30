import { createI18n } from "vue-i18n";
import en from "./i18n/en.json";
import fi from "./i18n/fi.json";

const i18n = createI18n({
  legacy: false,
  locale: "fi",
  messages: {
    en: en,
    fi: fi,
  },
});

export default i18n;
