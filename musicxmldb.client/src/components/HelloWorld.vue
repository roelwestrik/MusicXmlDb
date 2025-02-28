<template>
  <div class="weather-component">
    <h1>Weather forecast</h1>
    <p>This component demonstrates fetching data from the server.</p>

    <div v-if="loggedIn"><p>Hello, {{ username }}!</p></div>
    <button v-if="loggedIn" @click="logout">
      LOGOUT
    </button>

    <button v-else @click="login">
      LOGIN
    </button>

  </div>
</template>

<script lang="ts" setup>
import { ref } from 'vue';
import Keycloak from 'keycloak-js';

const keycloak = new Keycloak({
  url: "http://localhost:18080",
  realm: "musicxmldb-keycloak",
  clientId: "public-client"
});

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
