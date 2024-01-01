import { createRouter, createWebHistory } from "vue-router";
import CatShowList from "./views/CatShowList.vue";
import CatShowDetails from "./views/CatShowDetails.vue";
import Cats from "./views/Cats.vue";
import Users from "./views/Users.vue";
import CatDetails from "./views/CatDetails.vue";
import UserDetails from "./views/UserDetails.vue";

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

export default router;