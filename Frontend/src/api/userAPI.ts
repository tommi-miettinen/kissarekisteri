import apiClient from "./apiClient";

const getCurrentUser = async () => {
  const result = await apiClient.get<User>(`/users/me?$expand=Userrole($expand=role)`);
  return result.data;
};

const getUserById = async (userId: string): Promise<User | undefined> => {
  if (!userId) return;
  const result = await apiClient.get<User>(`/users/${userId}?$expand=Userrole($expand=role)`);
  return result.data;
};

const getUsers = async () => {
  const result = await apiClient.get<OdataResponse<User[]>>(`/users`);
  return result.data.value;
};

const getCatsByUserId = async (userId: string) => {
  const result = await apiClient.get<OdataResponse<Cat[]>>(`/cats?$filter=OwnerId eq '${userId}'`);
  return result.data.value;
};

const editUser = async (user: User) => {
  const result = await apiClient.patch(`/users/${user.id}`, user);
  return result.data;
};

const getPermissions = async (userId: string) => {
  const result = await apiClient.get<Permission[]>(`/users/${userId}/permissions`);
  return result.data;
};

const deleteUserById = async (userId: string) => {
  await apiClient.delete(`/users/${userId}`);
  return true;
};

const createUser = async (userPayload: any) => {
  const result = await apiClient.post(`/users`, userPayload);
  return result.data;
};

const getRoles = async () => {
  const result = await apiClient.get<OdataResponse<Role[]>>(`/users/roles`);
  return result.data.value;
};

const uploadAvatar = async (image: File) => {
  if (!image) return;

  const formData = new FormData();
  formData.append("file", image);

  const result = await apiClient.post<OdataResponse<Cat>>(`/users/avatar`, formData);
  return result.data;
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
