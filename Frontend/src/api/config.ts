import apiClient from "./apiClient";

const fetchConfig = async () => {
  try {
    const result = await apiClient.get<MsalConfig>(`/config/msalconfig`);
    return result.data;
  } catch (err) {
    console.log(err);
  }
};

export default {
  fetchConfig,
};
