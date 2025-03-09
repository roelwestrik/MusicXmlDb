<template>
  <div class="weather-component">
    <h1>Weather forecast</h1>
    <p>This component demonstrates fetching data from the server.</p>


    <div v-if="loggedIn">
      <p>Hello, {{ username }}!</p>
      <button @click="logout">
        LOGOUT
      </button>
    </div>

    <div v-else>
      <button @click="login">
        LOGIN
      </button>
    </div>


  </div>
</template>

<script lang="ts" setup>
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

<style scoped></style>
