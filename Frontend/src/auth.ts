import { BrowserCacheLocation, PublicClientApplication, EventType } from "@azure/msal-browser";
import { fetchUser, fetchPermissions } from "./store/userStore.ts";

//add client id as additional scope for access token
//https://github.com/AzureAD/microsoft-authentication-library-for-js/issues/2315#issuecomment-773407358
export const scopes = ["openid", "offline_access", "8f374d27-54ee-40d1-bed8-ba2f8a4bd1f6"];

const b2cPolicies = {
  authorities: {
    signUpSignIn: {
      authority: "https://kissarekisteri.b2clogin.com/kissarekisteri.onmicrosoft.com/b2c_1_sign_in_sign_up",
    },
  },
  authorityDomain: "kissarekisteri.b2clogin.com",
};

const msalConfig = {
  auth: {
    authority: b2cPolicies.authorities.signUpSignIn.authority,
    clientId: "8f374d27-54ee-40d1-bed8-ba2f8a4bd1f6",
    knownAuthorities: [b2cPolicies.authorityDomain],
    redirectUri: "https://localhost:5173",
  },
  cache: {
    cacheLocation: BrowserCacheLocation.LocalStorage,
  },
};

export const msalInstance = await PublicClientApplication.createPublicClientApplication(msalConfig);

msalInstance.addEventCallback(async (event) => {
  if (event.eventType === EventType.LOGIN_SUCCESS) {
    await fetchUser();
    await fetchPermissions();
  }
});
