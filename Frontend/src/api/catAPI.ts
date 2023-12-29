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

const addCat = async (cat: CatPayload) => {
  try {
    const result = await apiClient.post<Cat>("/cats", cat);
    return result.data;
  } catch (err) {
    console.log(err);
  }
};

const deleteCatById = async (catId: number): Promise<true | undefined> => {
  try {
    await apiClient.delete(`$/cats/${catId}`);
    return true;
  } catch (err) {
    console.log(err);
  }
};

const getCats = async (query?: string) => {
  try {
    const result = await apiClient.get<Cat[]>(`/cats?${query || ""}`);
    return result.data;
  } catch (err) {
    console.log(err);
  }
};

const getCatsByUserId = async (userId: string) => {
  try {
    const result = await apiClient.get<Cat>(`/users/${userId}/cats`);
    return result.data;
  } catch (err) {
    console.log(err);
  }
};

const getCatById = async (catId: number) => {
  try {
    const result = await apiClient.get<Cat>(`/cats/${catId}`);
    return result.data;
  } catch (err) {
    console.log(err);
  }
};

const editCat = async (updatedCat: EditCatPayload) => {
  try {
    const result = await apiClient.put<Cat>(`/cats/${updatedCat.id}`, updatedCat);
    return result.data;
  } catch (err) {
    console.log(err);
  }
};

const uploadCatImage = async (catId: number, image: File) => {
  if (!image) return;

  try {
    const formData = new FormData();
    formData.append("file", image);

    const result = await apiClient.post(`/cats/${catId}/photo`, formData);
    return result.data;
  } catch (err) {
    console.log(err);
  }
};

interface CatBreed {
  id: number;
  name: string;
}

const getCatBreeds = async () => {
  const result = await apiClient.get<CatBreed[]>(`/cats/breeds`);
  return result.data;
};

export default {
  addCat,
  getCatById,
  deleteCatById,
  getCats,
  getCatsByUserId,
  editCat,
  uploadCatImage,
  getCatBreeds,
};
