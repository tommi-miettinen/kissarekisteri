import apiClient from "./apiClient";
import userAPI from "./userAPI";

const addCat = async (cat: CatPayload) => {
  try {
    const result = await apiClient.post<ApiResponse<Cat>>("/cats", cat);
    return result.data.data;
  } catch (err) {
    console.log(err);
  }
};

const deleteCatById = async (catId: number): Promise<true | undefined> => {
  await apiClient.delete(`/cats/${catId}`);
  return true;
};

const getCats = async (query?: string) => {
  try {
    const result = await apiClient.get<OdataResponse<Cat>>(`odata/cats`);
    return result.data.value;
  } catch (err) {
    console.log(err);
  }
};

const getCatWithOwnerAndBreeder = async (catId: number) => {
  try {
    const cat = await getCatById(catId);
    const owner = await userAPI.getUserById(cat.ownerId);
    const breeder = await userAPI.getUserById(cat.breederId);

    cat.owner = owner ? owner : null;
    cat.breeder = breeder ? breeder : null;

    console.log(cat);
    return cat;
  } catch (err) {
    console.log(err);
  }
};

const getCatsByUserId = async (userId: string) => {
  try {
    const result = await apiClient.get<ApiResponse<Cat>>(`/users/${userId}/cats`);
    return result.data;
  } catch (err) {
    console.log(err);
  }
};

const getCatById = async (catId: number) => {
  const result = await apiClient.get<OdataResponse<Cat>>(`odata/cats?$filter=Id eq ${catId}&$expand=photos`);
  return result.data.value[0];
};

const editCat = async (updatedCat: EditCatPayload) => {
  try {
    const result = await apiClient.put<ApiResponse<Cat>>(`/cats/${updatedCat.id}`, updatedCat);
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

    const result = await apiClient.post<ApiResponse<Cat>>(`/cats/${catId}/photo`, formData);
    return result.data;
  } catch (err) {
    console.log(err);
  }
};

const requestOwnershipTransfer = async (catId: number) => {
  try {
    const result = await apiClient.post<ApiResponse<Cat>>(`/cats/${catId}/transfer`);
    return result.data;
  } catch (err) {
    console.log(err);
  }
};

const getConfirmationRequests = async () => {
  try {
    const result = await apiClient.get<ApiResponse<CatTransferRequest[]>>(`/cats/transfer-requests`);
    return result.data;
  } catch (err) {
    console.log(err);
  }
};

const confirmTransferRequest = async (requestId: number) => {
  try {
    const result = await apiClient.post<ApiResponse<Cat>>(`/cats/transfer-requests/${requestId}/confirm`);
    return result.data;
  } catch (err) {
    console.log(err);
  }
};

const getCatBreeds = async () => {
  try {
    const result = await apiClient.get<ApiResponse<CatBreed[]>>(`/cats/breeds`);
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
  getCatBreeds,
  requestOwnershipTransfer,
  getConfirmationRequests,
  confirmTransferRequest,
  getCatWithOwnerAndBreeder,
};
