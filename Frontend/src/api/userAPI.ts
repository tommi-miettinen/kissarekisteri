import apiClient from "./apiClient";

const getCurrentUser = async () => {
  try {
    const result = await apiClient.get<User>(`/users/me`);
    return result.data;
  } catch (err) {
    console.log(err);
  }
};

const getUserById = async (userId: string) => {
  try {
    const result = await apiClient.get<User>(`/users/${userId}`);
    return result.data;
  } catch (err) {
    console.log(err);
  }
};

const getUsers = async () => {
  try {
    const result = await apiClient.get<User[]>(`/users`);
    return result.data;
  } catch (err) {
    console.log(err);
  }
};

const getCatsByUserId = async (userId: string) => {
  try {
    const result = await apiClient.get<ApiResponse<Cat[]>>(`/users/${userId}/cats`);
    return result.data;
  } catch (err) {
    console.log(err);
  }
};

const editUser = async (user: User) => {
  try {
    const result = await apiClient.put(`/users/${user.id}`, user);
    return result.data;
  } catch (err) {
    console.log(err);
  }
};

const getPermissions = async (userId: string) => {
  try {
    const result = await apiClient.get<Permission[]>(`users/${userId}/permissions`);
    return result.data;
  } catch (err) {
    console.log(err);
  }
};

const deleteUserById = async (userId: string) => {
  await apiClient.delete(`/users/${userId}`);
  return true;
};

const createUser = async (userPayload: any) => {
  console.log(userPayload);
  try {
    const result = await apiClient.post(`/users`, userPayload);
    return result.data;
  } catch (err) {
    console.log(err);
  }
};

const getRoles = async () => {
  try {
    const result = await apiClient.get<any[]>(`users/roles`);
    return result.data;
  } catch (err) {
    console.log(err);
  }
};

export default {
  getUserById,
  getUsers,
  editUser,
  getCatsByUserId,
  getCurrentUser,
  getPermissions,
  deleteUserById,
  createUser,
  getRoles,
};
