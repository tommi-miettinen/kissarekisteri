import apiClient from "./apiClient";

const fetchConfig = async () => {
  try {
    const result = await apiClient.get<MsalConfig>(`/users/config`);
    return result.data;
  } catch (err) {
    console.log(err);
  }
};

export default {
  fetchConfig,
};
