import configAPI from "./api/config.ts";
import { BrowserCacheLocation, PublicClientApplication, EventType, IPublicClientApplication } from "@azure/msal-browser";
import { fetchUser, fetchPermissions } from "./store/userStore.ts";

//add client id as additional scope for access token
//https://github.com/AzureAD/microsoft-authentication-library-for-js/issues/2315#issuecomment-773407358
export const scopes = ["openid", "offline_access", "8f374d27-54ee-40d1-bed8-ba2f8a4bd1f6"];

export let msalInstance: IPublicClientApplication;

export const initializeMsalInstance = async () => {
  const config = await configAPI.fetchConfig();

  if (!config) return console.log("No config found");

  const msalConfig = {
    auth: {
      authority: config.authority,
      clientId: config.clientId,
      knownAuthorities: [config.authorityDomain],
      redirectUri: config.redirectUri,
    },
    cache: {
      cacheLocation: BrowserCacheLocation.LocalStorage,
    },
  };

  msalInstance = await PublicClientApplication.createPublicClientApplication(msalConfig);

  msalInstance.addEventCallback(async (event) => {
    if (event.eventType === EventType.LOGIN_SUCCESS) {
      await fetchUser();
      await fetchPermissions();
    }
  });

  return msalInstance;
};
