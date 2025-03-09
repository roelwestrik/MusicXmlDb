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
    <div class="header">

        <a class="logo">MusicXmlDb</a>

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
/* Style the header with a grey background and some padding */
.header {
    overflow: hidden;
    padding: 20px 10px;
}

/* Style the header links */
.header a, p {
    float: left;
    text-align: center;
    padding: 12px;
    text-decoration: none;
    font-size: 18px;
    line-height: 25px;
    border-radius: 4px;
}

/* Style the logo link (notice that we set the same value of line-height and font-size to prevent the header to increase when the font gets bigger */
.header a.logo {
    font-size: 25px;
    font-weight: bold;
}

/* Change the background color on mouse-over */
.header a:hover {
    cursor: pointer;
}

/* Float the link section to the right */
.header-right {
    float: right;
}

/* Add media queries for responsiveness - when the screen is 500px wide or less, stack the links on top of each other */
@media screen and (max-width: 500px) {
    .header a, p {
        float: none;
        display: block;
        text-align: left;
    }

    .header-right {
        float: none;
    }
}
</style>