import userAPI from "../api/userAPI";
import { reactive, computed } from "vue";

interface UserState {
  user: null | User;
  permissions: Permission[];
}

export const userStore = reactive<UserState>({
  user: null,
  permissions: [],
});

export const setUser = (user: any) => {
  userStore.user = user;
};

export const fetchUser = async () => {
  const user = await userAPI.getCurrentUser();
  if (!user) return;

  userStore.user = user;
};

export const fetchPermissions = async () => {
  const permissions = await userAPI.getPermissions(userStore.user!.id);
  if (!permissions) return;

  userStore.permissions = permissions;
};

export const logout = () => {
  userStore.user = null;
  localStorage.removeItem("token");
};

export const userHasPermission = (permission: string) => userStore.permissions.some((p) => p.name === permission);

export const editUser = async (user: User) => {
  const editedUser = await userAPI.editUser(user);
  if (!editedUser) return;

  userStore.user = editedUser;
};

export const user = computed(() => userStore.user);
