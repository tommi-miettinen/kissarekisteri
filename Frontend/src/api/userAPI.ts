import axios from "axios";

const baseUrl = import.meta.env.MODE === "development" ? "https://localhost:44316" : "/";

const getCurrentUser = async () => {
  const result = await axios.get<User>(`${baseUrl}/me`, {
    withCredentials: true,
  });
  return result.data;
};

const getUserById = async (userId: string) => {
  const result = await axios.get<User>(`${baseUrl}/users/${userId}`);
  return result.data;
};

const getUsers = async () => {
  const result = await axios.get<User[]>(`${baseUrl}/users`);
  return result.data;
};

const getCatsByUserId = async (userId: string) => {
  try {
    const result = await axios.get<Cat[]>(`${baseUrl}/users/${userId}/cats`);
    return result.data;
  } catch (err) {
    console.log(err);
  }
};

const editUser = async (user: User) => {
  try {
    const result = await axios.put(`${baseUrl}/users/${user.id}`, user);
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
