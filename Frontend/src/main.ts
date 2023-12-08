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

app.use(VueQueryPlugin);
app.use(router);
app.mount("#app");
