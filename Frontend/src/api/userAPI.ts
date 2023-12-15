import axios from "axios";

const baseUrl = import.meta.env.MODE === "development" ? "https://localhost:44316" : "/";

const login = async (loginPayload: { username: string; password: string }) => {
  console.log(loginPayload);
  const result = await axios.post(`${baseUrl}/login`, loginPayload);
  if (result.data && result.data.token) {
    localStorage.setItem("token", result.data.token);
  }
  console.log(result);
  return result;
};

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

const getEvents = async (): Promise<CatShowEvent[] | undefined> => {
  try {
    const result = await axios.get(`${baseUrl}/catshows`);
    return result.data;
  } catch (err) {
    console.log(err);
  }
};

const getEventById = async (eventId: number) => {
  try {
    const result = await axios.get(`${baseUrl}/catshows/${eventId}`);
    return result.data;
  } catch (err) {
    console.log(err);
  }
};

const joinEvent = async (eventId: number, catIds: number[]) => {
  try {
    const result = await axios.post(
      `${baseUrl}/catshows/${eventId}/join`,
      { catIds },
      {
        withCredentials: true,
      }
    );
    return result.data;
  } catch (err) {
    console.log(err);
  }
};

const leaveEvent = async (eventId: number) => {
  try {
    const result = await axios.delete(`${baseUrl}/catshows/${eventId}/leave`, {
      withCredentials: true,
    });
    return result.data;
  } catch (err) {
    console.log(err);
  }
};

const getCatsByUserId = async (userId: string) => {
  try {
    const result = await axios.get<Cat[]>(`${baseUrl}/users/${userId}/cats`);
    return result.data;
  } catch (err) {
    console.log(err);
  }
};

const createCatShowEvent = async (catShowEvent: CatShowEvent) => {
  await axios.post(`${baseUrl}/catshows`, catShowEvent);
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
  login,
  getEvents,
  getEventById,
  getUserById,
  getUsers,
  joinEvent,
  createCatShowEvent,
  editUser,
  getCatsByUserId,
  getCurrentUser,
  leaveEvent,
};
