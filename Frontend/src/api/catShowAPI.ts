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

const createCatShowEvent = async (catShowEvent: CatShowEvent) => {
  try {
    const result = await apiClient.post(`/catshows`, catShowEvent);
    return result.data;
  } catch (err) {
    console.log(err);
  }
};

const getEvents = async (): Promise<CatShowEvent[] | undefined> => {
  try {
    const result = await apiClient.get(`/catshows`);
    return result.data;
  } catch (err) {
    console.log(err);
  }
};

const getEventById = async (eventId: number) => {
  try {
    const result = await apiClient.get<CatShowEvent>(`/catshows/${eventId}`);
    return result.data;
  } catch (err) {
    console.log(err);
  }
};

const joinEvent = async (eventId: number, catIds: number[]) => {
  try {
    const result = await apiClient.post(`/catshows/${eventId}/join`, { catIds });
    return result.data;
  } catch (err) {
    console.log(err);
  }
};

const leaveEvent = async (eventId: number) => {
  try {
    const result = await apiClient.delete(`/catshows/${eventId}/leave`, {
      withCredentials: true,
    });
    return result.data;
  } catch (err) {
    console.log(err);
  }
};

const assignCatPlacing = async (eventId: number, payload: CatShowResultPayload) => {
  try {
    const result = await apiClient.post(`/catshows/${eventId}/place`, payload);
    return result.data;
  } catch (err) {
    console.log(err);
  }
};

const addCatShowPhoto = async (eventId: number, image: File) => {
  if (!image) return;

  try {
    const formData = new FormData();
    formData.append("file", image);

    const result = await apiClient.post(`/catshows/${eventId}/photos`, formData);
    return result.data;
  } catch (err) {
    console.log(err);
  }
};

export default {
  addCatShowPhoto,
  createCatShowEvent,
  getEvents,
  getEventById,
  joinEvent,
  leaveEvent,
  assignCatPlacing,
};
