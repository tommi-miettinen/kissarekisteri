import { createApp } from "vue";
import { VueQueryPlugin } from "@tanstack/vue-query";
import App from "./App.vue";
import Profile from "./components/Profile.vue";
import CatShowList from "./components/CatShowList.vue";
import CatShowDetails from "./components/CatShowDetails.vue";
import Cats from "./components/Cats.vue";
import Users from "./components/Users.vue";
import CatDetails from "./components/CatDetails.vue";
import UserDetails from "./components/UserDetails.vue";
import { createI18n } from "vue-i18n";
import en from "./i18n/en.json";
import fi from "./i18n/fi.json";
import "./custom.scss";

const app = createApp(App);

import { createRouter, createWebHistory } from "vue-router";

const routes = [
  { path: "/profile", component: Profile },
  { path: "/catshows", component: CatShowList },
  { path: "/catshows/:eventId", component: CatShowDetails },
  { path: "/cats", component: Cats },
  { path: "/cats/:catId", component: CatDetails },
  { path: "/users", component: Users },
  { path: "/users/:userId", component: UserDetails },
  {
    path: "/:pathMatch(.*)*",
    name: "not-found",
    redirect: "/cats",
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

const i18n = createI18n({
  legacy: false,
  locale: "fi",

  messages: {
    en: en,
    fi: fi,
  },
});

app.use(i18n);
app.use(VueQueryPlugin);
app.use(router);
app.mount("#app");
