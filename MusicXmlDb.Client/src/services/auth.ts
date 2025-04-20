import Keycloak, { type KeycloakConfig } from 'keycloak-js';
import { KEYCLOAK_CLIENT_ID, KEYCLOAK_REALM, KEYCLOAK_URL } from '@/services/environment';


const config: KeycloakConfig = {
    url: KEYCLOAK_URL,
    realm: KEYCLOAK_REALM,
    clientId: KEYCLOAK_CLIENT_ID
}

export const keycloak = new Keycloak(config);

export const login = async () => {
    const redirectUri = window.location.origin;
    await keycloak.login({redirectUri: redirectUri})
};

export const logout = async () => {
    const redirectUri = window.location.origin;
    await keycloak.logout({redirectUri: redirectUri});
};

export const getUserName: () => Promise<string> = async () => {
    if(keycloak.isTokenExpired(5)){
        await keycloak.updateToken()
    }
    if(keycloak.authenticated){
        return (await keycloak.loadUserProfile()).firstName ?? "No email.";
    }

    return "Not Authenticated";
}

export const getUpdatedToken: () => Promise<string> = async () => {
    let token = "";
    try{
        if(keycloak.isTokenExpired(5)){
            await keycloak.updateToken()
        }
        token = keycloak.token ?? "";
    }
    catch{

    }

    return token;
}