import { createApp } from "vue";
import { initializeMsalInstance } from "./auth";
import { VueQueryPlugin } from "@tanstack/vue-query";
import App from "./App.vue";
import router from "./routes";
import i18n from "./i18n";
import "./index.scss";
import "moment/dist/locale/fi";
import moment from "moment";
moment.locale("fi");

await initializeMsalInstance();
const app = createApp(App);

app.use(i18n);
app.use(VueQueryPlugin);
app.use(router);
app.mount("#app");
