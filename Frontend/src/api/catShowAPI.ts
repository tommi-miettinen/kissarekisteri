import axios from "axios";

const baseUrl = import.meta.env.MODE === "development" ? "https://localhost:44316" : "/";

const createCatShowEvent = async (catShowEvent: CatShowEvent) => {
  await axios.post(`${baseUrl}/catshows`, catShowEvent);
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
    const result = await axios.get<CatShowEvent>(`${baseUrl}/catshows/${eventId}`);
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

interface Payload {
  catId: number;
  place: number;
  breed: string;
}

const assignCatPlacing = async (eventId: number, payload: Payload) => {
  const result = await axios.post(`${baseUrl}/catshows/${eventId}/place`, payload);
  return result.data;
};

const addCatShowPhoto = async (eventId: number, image: File) => {
  if (!image) return;

  try {
    const formData = new FormData();
    formData.append("file", image);

    const result = await axios.post(`${baseUrl}/catshows/${eventId}/photos`, formData);
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
