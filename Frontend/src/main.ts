import { PublicClientApplication } from "@azure/msal-browser";
import { fetchUser } from "./store/userStore";
import { createApp } from "vue";
import { createRouter, createWebHistory } from "vue-router";
import { VueQueryPlugin } from "@tanstack/vue-query";
import App from "./App.vue";
import CatShowList from "./views/CatShowList.vue";
import CatShowDetails from "./views/CatShowDetails.vue";
import Cats from "./views/Cats.vue";
import Users from "./views/Users.vue";
import CatDetails from "./views/CatDetails.vue";
import UserDetails from "./views/UserDetails.vue";
import { createI18n } from "vue-i18n";
import en from "./i18n/en.json";
import fi from "./i18n/fi.json";
import "./index.scss";

const app = createApp(App);

const b2cPolicies = {
  authorities: {
    signUpSignIn: {
      authority: "https://kissarekisteri.b2clogin.com/kissarekisteri.onmicrosoft.com/b2c_1_sign_in_sign_up",
    },
  },
  authorityDomain: "kissarekisteri.b2clogin.com",
};

const msalConfig = {
  auth: {
    authority: b2cPolicies.authorities.signUpSignIn.authority,
    clientId: "8f374d27-54ee-40d1-bed8-ba2f8a4bd1f6",
    knownAuthorities: [b2cPolicies.authorityDomain],
    redirectUri: "https://localhost:5173",
  },
};

const msalInstance = await PublicClientApplication.createPublicClientApplication(msalConfig);

const routes = [
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

router.beforeEach((to, from, next) => {
  const hash = to.hash;
  const fragment = new URLSearchParams(hash.slice(1));

  if (fragment.has("id_token")) {
    localStorage.token = fragment.get("id_token");

    const newRoute = {
      ...to,
      hash: "",
      replace: true,
    };

    fetchUser();

    return next(newRoute);
  }

  next();
});

const i18n = createI18n({
  legacy: false,
  locale: "fi",
  messages: {
    en: en,
    fi: fi,
  },
});

app.config.globalProperties.$msal = msalInstance;
app.use(i18n);
app.use(VueQueryPlugin);
app.use(router);
app.mount("#app");
