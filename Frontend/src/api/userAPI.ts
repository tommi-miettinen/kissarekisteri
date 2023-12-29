import axios from "axios";

const apiClient = axios.create({
  baseURL: import.meta.env.MODE === "development" ? "https://localhost:44316" : "/",
});

apiClient.interceptors.request.use(
  (config) => {
    const token = localStorage.token;
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

const getCurrentUser = async () => {
  try {
    const result = await apiClient.get<User>(`/me`);
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
    const result = await apiClient.get<Cat[]>(`/users/${userId}/cats`);
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

export default {
  getUserById,
  getUsers,
  editUser,
  getCatsByUserId,
  getCurrentUser,
};
