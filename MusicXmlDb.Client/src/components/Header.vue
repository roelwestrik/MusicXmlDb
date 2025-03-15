<script setup lang="ts">
import { ref } from 'vue';
import Keycloak, { type KeycloakConfig } from 'keycloak-js';

const config: KeycloakConfig = {
    url: "http://localhost:18080/",
    realm: "musicxmldb-auth",
    clientId: "public-client",
}

const keycloak = new Keycloak(config);

const loggedIn = ref(false)
const username = ref("")

keycloak.init({ onLoad: 'check-sso' }).then(e => {
    loggedIn.value = e;
    if (e) {
        keycloak.loadUserProfile().then(f => {
            username.value = f.email ?? "";
        });
    }
})

const login = async () => {
    await keycloak.login()
};

const logout = async () => {
    await keycloak.logout();
};

</script>

<template>
    <div class="topnav">

        <router-link class="logo" to="/">MusicXmlDb</router-link>
        <router-link to="/about">About</router-link>

        <div class="header-right">
            <div v-if="loggedIn">
                <p style="display: inline-block;">Hello, {{ username }}!</p>
                <a @click="logout"> Logout </a>

            </div>
            <div v-else>
                <a @click="login"> Login </a>
                <a @click="login"> Register </a>
            </div>


        </div>

    </div>
</template>

<style lang="css" scoped>
/* Add a black background color to the top navigation */
.topnav {
  overflow: hidden;
}

/* Style the links inside the navigation bar */
.topnav a {
  float: left;
  text-align: center;
  padding: 14px 16px;
  text-decoration: none;
  font-size: 17px;
}

</style>