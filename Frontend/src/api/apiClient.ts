import { msalInstance } from "../auth";
import axios from "axios";

const apiClient = axios.create({
  baseURL: import.meta.env.MODE === "development" ? "https://localhost:44316" : "/",
});

apiClient.interceptors.request.use(
  async (config) => {
    try {
      const accounts = msalInstance.getAllAccounts();
      if (accounts.length > 0) {
        const response = await msalInstance.acquireTokenSilent({
          scopes: ["openid", "offline_access"],
          account: accounts[0],
        });
        config.headers.Authorization = `Bearer ${response.idToken}`;
        console.log(response.idToken);
      }
    } catch (error) {
      console.error("Token acquisition failed", error);
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

export default apiClient;
