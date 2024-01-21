import apiClient from "./apiClient";

const fetchConfig = async () => {
  try {
    const result = await apiClient<OdataResponse<MsalConfig>>(`/msalconfig`);
    return result.data.value[0];
  } catch (err) {
    console.log(err);
  }
};

export default {
  fetchConfig,
};
