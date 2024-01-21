import apiClient from "./apiClient";
import userAPI from "./userAPI";

const addCat = async (cat: CatPayload) => {
  const result = await apiClient.post<ApiResponse<Cat>>("/cats", cat);
  return result.data.data;
};

const deleteCatById = async (catId: number): Promise<true | undefined> => {
  await apiClient.delete(`/cats/${catId}`);
  return true;
};

const getCats = async (query?: Query) => {
  const filter = query?.filter ? `$filter=${query.filter}` : "";
  const expand = query?.expand ? `&$expand=${query.expand}` : "";
  const top = query?.top ? `&$top=${query.top}` : "";
  const skip = query?.skip ? `&$skip=${query.skip}` : "";
  const orderBy = query?.orderBy ? `&$orderby=${query.orderBy}` : "";
  const url = `/cats?${filter}${expand}${top}${skip}${orderBy}`;

  const result = await apiClient.get<OdataResponse<Cat[]>>(url);
  return result.data.value;
};

const getCatWithOwnerAndBreeder = async (catId: number) => {
  const cat = await getCatById(catId);
  if (!cat) return;

  const owner = await userAPI.getUserById(cat.ownerId);
  const breeder = await userAPI.getUserById(cat.breederId);

  cat.owner = owner;
  cat.breeder = breeder;

  return cat;
};

const getCatById = async (catId: number) => {
  if (!catId) return console.error("CAT ID MISSING");
  const filter = `Id eq ${catId}`;
  const expand = `Kittens($expand=ChildCat),Parents($expand=ParentCat),Photos,Results($expand=CatShow)`;
  const url = `/cats?$filter=${filter}&$expand=${expand}`;

  const result = await apiClient.get<OdataResponse<Cat[]>>(url);
  return result.data.value[0];
};

const editCat = async (updatedCat: EditCatPayload) => {
  const result = await apiClient.put<ApiResponse<Cat>>(`/cats/${updatedCat.id}`, updatedCat);
  return result.data;
};

const uploadCatImage = async (catId: number, image: File) => {
  if (!image) return;

  const formData = new FormData();
  formData.append("file", image);

  const result = await apiClient.post<ApiResponse<Cat>>(`/cats/${catId}/photo`, formData);
  return result.data;
};

const requestOwnershipTransfer = async (catId: number) => {
  const result = await apiClient.post<ApiResponse<Cat>>(`/cats/${catId}/transfer`);
  return result.data;
};

const getConfirmationRequests = async () => {
  const result = await apiClient.get<ApiResponse<CatTransferRequest[]>>(`/cats/transfer-requests`);
  return result.data;
};

const confirmTransferRequest = async (requestId: number) => {
  const result = await apiClient.post<ApiResponse<Cat>>(`/cats/transfer-requests/${requestId}/confirm`);
  return result.data;
};

const getCatBreeds = async () => {
  const result = await apiClient.get<CatBreed[]>(`/cats/catbreeds`);
  return result.data;
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
