import apiClient from "./apiClient";
import userAPI from "./userAPI";
import buildQuery from "odata-query";

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

interface Query {
  filter?: string;
  expand?: string;
  top?: number;
  skip?: number;
  orderBy?: string;
}

const getCats = async (query?: Query) => {
  try {
    const filter = query?.filter ? `$filter=${query.filter}` : "";
    const expand = query?.expand ? `$expand=${query.expand}` : "";
    const top = query?.top ? `$top=${query.top}` : "";
    const skip = query?.skip ? `$skip=${query.skip}` : "";
    const orderBy = query?.orderBy ? `$orderby=${query.orderBy}` : "";
    const url = `odata/cats?${filter}&${expand}&${top}&${skip}&${orderBy}`;

    const result = await apiClient.get<OdataResponse<Cat>>(url);
    return result.data.value;
  } catch (err) {
    console.log(err);
  }
};

const getCatWithOwnerAndBreeder = async (catId: number) => {
  try {
    const cat = await getCatById(catId);
    if (!cat) return;

    const owner = await userAPI.getUserById(cat.ownerId);
    const breeder = await userAPI.getUserById(cat.breederId);

    cat.owner = owner;
    cat.breeder = breeder;

    return cat;
  } catch (err) {
    console.log(err);
  }
};

const getCatById = async (catId: number) => {
  try {
    const filter = `Id eq ${catId}`;
    const expand = `Kittens($expand=ChildCat),Parents($expand=ParentCat),Photos,Results($expand=CatShow)`;
    const url = `odata/cats?$filter=${filter}&$expand=${expand}`;
    const result = await apiClient.get<OdataResponse<Cat>>(url);
    return result.data.value[0];
  } catch (error) {
    console.error("Error fetching cat data:", error);
  }
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
    const result = await apiClient.get<ApiResponse<CatTransferRequest[]>>(`/api/cats/transfer-requests`);
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
    const result = await apiClient.get<OdataResponse<CatBreed>>(`odata/catbreeds`);
    return result.data.value;
  } catch (err) {
    console.log(err);
  }
};

export default {
  addCat,
  getCatById,
  deleteCatById,
  getCats,
  editCat,
  uploadCatImage,
  getCatBreeds,
  requestOwnershipTransfer,
  getConfirmationRequests,
  confirmTransferRequest,
  getCatWithOwnerAndBreeder,
};
