import { msalInstance, scopes } from "../auth";
import axios from "axios";

const apiClient = axios.create({
  baseURL: import.meta.env.DEV ? "https://localhost:44316/odata" : "/odata",
});

apiClient.interceptors.request.use(async (config) => {
  try {
    if (!msalInstance) return config;

    const accounts = msalInstance.getAllAccounts();

    if (accounts.length === 0) return config;

    const request = {
      account: accounts[0],
      scopes,
    };

    const response = await msalInstance.acquireTokenSilent(request);
    config.headers.Authorization = `Bearer ${response.accessToken}`;
  } catch (error) {
    console.error("Token acquisition failed", error);
  }
  return config;
});

export default apiClient;
