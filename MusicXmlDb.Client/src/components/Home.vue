<template>
  <h1>Home!</h1>

<router-link to="/scores" v-if="keycloak.authenticated">My Scores</router-link>

</template>

<script lang="ts" setup>
import { keycloak } from "../services/auth"

</script>

<style scoped>

</style>
