import apiClient from "./apiClient";

const createCatShowEvent = async (catShowEvent: CatShowEvent) => {
  try {
    const result = await apiClient.post(`/catshows`, catShowEvent);
    return result.data;
  } catch (err) {
    console.log(err);
  }
};

const getEvents = async () => {
  try {
    const result = await apiClient.get<OdataResponse<CatShowEvent>>(`/catshows?$orderby=StartDate desc`);
    return result.data.value;
  } catch (err) {
    console.log(err);
  }
};

const getEventById = async (eventId: number) => {
  const result = await apiClient.get<OdataResponse<CatShowEvent>>(
    `/catshows?$filter=Id eq ${eventId}&$expand=Cats($expand=Cat),Photos,Results`
  );
  return result.data.value[0];
};

const joinEvent = async (eventId: number, catIds: number[]) => {
  const result = await apiClient.post(`/catshows/${eventId}/join`, { catIds });
  return result.data;
};

const leaveEvent = async (eventId: number) => {
  try {
    const result = await apiClient.delete(`/catshows/${eventId}/leave`);
    return result.data;
  } catch (err) {
    console.log(err);
  }
};

const assignCatPlacing = async (eventId: number, payload: CatShowResultPayload) => {
  const result = await apiClient.post(`/catshows/${eventId}/place`, payload);
  return result.data;
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
