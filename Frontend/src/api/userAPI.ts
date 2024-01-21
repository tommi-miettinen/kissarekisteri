import apiClient from "./apiClient";

const getCurrentUser = async () => {
  try {
    const result = await apiClient.get<User>(`/api/users/me`);
    return result.data;
  } catch (err) {
    console.log(err);
  }
};

const getUserById = async (userId: string) => {
  try {
    const result = await apiClient.get<User>(`/api/users/${userId}`);
    return result.data;
  } catch (err) {
    console.log(err);
  }
};

const getUsers = async () => {
  try {
    const result = await apiClient.get<User[]>(`/api/users`);
    return result.data;
  } catch (err) {
    console.log(err);
  }
};

const getCatsByUserId = async (userId: string) => {
  const result = await apiClient.get<OdataResponse<Cat>>(`/odata/cats?$filter=OwnerId eq '${userId}'`);
  return result.data.value;
};

const editUser = async (user: User) => {
  try {
    const result = await apiClient.put(`/api/users/${user.id}`, user);
    return result.data;
  } catch (err) {
    console.log(err);
  }
};

const getPermissions = async (userId: string) => {
  try {
    const result = await apiClient.get<Permission[]>(`/api/users/${userId}/permissions`);
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
    const result = await apiClient.post(`/api/users`, userPayload);
    return result.data;
  } catch (err) {
    console.log(err);
  }
};

interface Role {
  id: number;
  name: string;
}

const getRoles = async () => {
  try {
    const result = await apiClient.get<OdataResponse<Role>>(`/odata/roles`);
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

    const result = await apiClient.post<ApiResponse<Cat>>(`/api/users/avatar`, formData);
    return result.data;
  } catch (err) {
    console.log(err);
  }
};

const registerAsBreeder = async () => {
  try {
    console.log("test");
    return;
    const result = await apiClient.post(`/users/breeder`);
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
  registerAsBreeder,
};
