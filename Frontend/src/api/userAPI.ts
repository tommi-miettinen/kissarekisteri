import apiClient from "./apiClient";

const getCurrentUser = async () => {
  try {
    const result = await apiClient.get<User>(`/users/me?$expand=Userrole($expand=role)`);
    return result.data;
  } catch (err) {
    console.log(err);
  }
};

const getUserById = async (userId: string) => {
  try {
    const result = await apiClient.get<User>(`/users/${userId}?$expand=Userrole($expand=role)`);
    return result.data;
  } catch (err) {
    console.log(err);
  }
};

const getUsers = async () => {
  try {
    const result = await apiClient.get<OdataResponse<User>>(`/users`);
    return result.data.value;
  } catch (err) {
    console.log(err);
  }
};

const getCatsByUserId = async (userId: string) => {
  const result = await apiClient.get<OdataResponse<Cat>>(`/cats?$filter=OwnerId eq '${userId}'`);
  return result.data.value;
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
    const result = await apiClient.get<Permission[]>(`/users/${userId}/permissions`);
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
  try {
    const result = await apiClient.post(`/users`, userPayload);
    return result.data;
  } catch (err) {
    console.log(err);
  }
};

const getRoles = async () => {
  try {
    const result = await apiClient.get<OdataResponse<Role>>(`/users/roles`);
    return result.data.value;
  } catch (err) {
    console.log(err);
  }
};

const uploadAvatar = async (image: File) => {
  if (!image) return;

  console.log("sending", image);

  try {
    const formData = new FormData();
    formData.append("file", image);

    const result = await apiClient.post<ApiResponse<Cat>>(`/users/avatar`, formData);
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
  uploadAvatar,
};
