import axios from "axios";

const baseUrl = import.meta.env.MODE === "development" ? "https://localhost:44316" : "https://kissarekisteri.azurewebsites.net";

const apiManagementBaseUrl = "https://kissarekisteriapimanagement.azure-api.net";

const addCat = async (cat: Cat) => {
  try {
    const result = await axios.post<Cat>(`${baseUrl}/cats`, cat);
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

const editCat = async (updatedCat: Cat) => {
  try {
    const result = await axios.put(`${baseUrl}/cats/${updatedCat.id}`, updatedCat);
    return result.data;
  } catch (err) {
    console.log(err);
  }
};

const getCatImages = async () => {
  const result = await axios.get(`${apiManagementBaseUrl}/images`);
  console.log(result);
  return result.data;
};

export default {
  addCat,
  getCatById,
  deleteCatById,
  getCats,
  getCatsByUserId,
  editCat,
  getCatImages,
};
