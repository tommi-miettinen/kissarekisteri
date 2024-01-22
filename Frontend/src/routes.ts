import { createRouter, createWebHistory } from "vue-router";
import CatShowList from "./views/CatShowList.vue";
import CatShowDetails from "./views/CatShowDetails.vue";
import Cats from "./views/Cats.vue";
import Users from "./views/Users.vue";
import CatDetails from "./views/CatDetails.vue";
import UserDetails from "./views/UserDetails.vue";
import { actionStack, popAction } from "./store/actionStore";

const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: "/catshows", component: CatShowList },
    { path: "/catshows/:eventId", component: CatShowDetails },
    { path: "/cats", component: Cats },
    { path: "/cats/:catId", component: CatDetails },
    { path: "/users", component: Users },
    { path: "/users/:userId", component: UserDetails },
    {
      path: "/:pathMatch(.*)*",
      beforeEnter: (to, _, next) => {
        if (to.path.includes("swagger") || to.path.includes("api")) {
          window.location.href = to.fullPath;
        } else {
          next("/cats");
        }
      },
      redirect: "/cats",
    },
  ],
});

router.beforeEach(async (_, __, next) => {
  if (actionStack.value && actionStack.value.length > 0) {
    popAction();
    return next(false);
  }
  next();
});

export default router;
