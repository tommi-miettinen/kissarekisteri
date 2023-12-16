import userAPI from "../api/userAPI";
import { reactive, computed } from "vue";

interface UserState {
  user: null | User;
}

export const userStore = reactive<UserState>({
  user: null,
});

export const setUser = (user: any) => {
  userStore.user = user;
};

export const fetchUser = async () => {
  const user = await userAPI.getCurrentUser();
  user.cats = await userAPI.getCatsByUserId(user.id);
  if (!user) return;

  userStore.user = user;
};

export const logout = () => (userStore.user = null);

export const editUser = async (user: User) => {
  const editedUser = await userAPI.editUser(user);
  if (!editedUser) return;

  userStore.user = editedUser;
};

export const user = computed(() => userStore.user);
