import axios from "axios";

const baseUrl = import.meta.env.MODE === "development" ? "https://localhost:44316" : "/";

const addCat = async (cat: CatPayload) => {
  try {
    const result = await axios.post<Cat>(`${baseUrl}/cats`, cat, {
      withCredentials: true,
    });
    return result.data;
  } catch (err) {
    console.log(err);
  }
};

const deleteCatById = async (catId: number): Promise<true | undefined> => {
  try {
    await axios.delete(`${baseUrl}/cats/${catId}`);
    return true;
  } catch (err) {
    console.log(err);
  }
};

const getCats = async () => {
  try {
    const result = await axios.get<Cat[]>(`${baseUrl}/cats`);
    return result.data;
  } catch (err) {
    console.log(err);
  }
};

const getCatsByUserId = async (userId: string) => {
  try {
    const result = await axios.get(`${baseUrl}/users/${userId}/cats`);
    return result.data;
  } catch (err) {
    console.log(err);
  }
};

const getCatById = async (catId: number) => {
  try {
    const result = await axios.get(`${baseUrl}/cats/${catId}`);
    return result.data;
  } catch (err) {
    console.log(err);
  }
};

const editCat = async (updatedCat: EditCatPayload) => {
  try {
    const result = await axios.put(`${baseUrl}/cats/${updatedCat.id}`, updatedCat);
    return result.data;
  } catch (err) {
    console.log(err);
  }
};

const uploadCatImage = async (catId: number, image: File) => {
  try {
    const formData = new FormData();
    if (image) formData.append("image", image);

    const result = await axios.post(`${baseUrl}/cats/${catId}/photo`, formData);
    return result.data;
  } catch (err) {
    console.log(err);
  }
};

export default {
  addCat,
  getCatById,
  deleteCatById,
  getCats,
  getCatsByUserId,
  editCat,
  uploadCatImage,
};
