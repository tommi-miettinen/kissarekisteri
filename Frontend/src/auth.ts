import { PublicClientApplication } from "@azure/msal-browser";

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
};

export const msalInstance = await PublicClientApplication.createPublicClientApplication(msalConfig);
