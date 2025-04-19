import { ref } from 'vue';
import Keycloak, { type KeycloakConfig } from 'keycloak-js';


const config: KeycloakConfig = {
    url: "http://localhost:18080/",
    realm: "musicxmldb-auth",
    clientId: "public-client"
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