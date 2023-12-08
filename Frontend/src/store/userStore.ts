import userAPI from "../api/userAPI";
import create from "vue-zustand";

interface UserState {
  user: null | any;
}

export const userStore = create<UserState>(() => ({
  user: null,
}));

export const setUser = (user: any) => {
  userStore.setState({ user });
};

export const fetchUser = async () => {
  const user = await userAPI.getCurrentUser();
  user.cats = await userAPI.getCatsByUserId(user.id);
  if (!user) return;
  userStore.setState({ user });
};

export const logout = () => {
  userStore.setState({ user: null });
};

export const editUser = async (user: User) => {
  const editedUser = await userAPI.editUser(user);
  if (!editedUser) return;

  userStore.setState({ user: editedUser });
};
