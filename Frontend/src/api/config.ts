import apiClient from "./apiClient";

const fetchConfig = async () => {
  const result = await apiClient<OdataResponse<MsalConfig>>(`/msalconfig`);
  return result.data.value[0];
};

export default {
  fetchConfig,
};
