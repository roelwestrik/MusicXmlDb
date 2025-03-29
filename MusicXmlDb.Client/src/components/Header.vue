<script setup lang="ts">
import { keycloak, login, logout } from "../services/auth"

</script>

<template>
    <div class="topnav">

        <router-link class="logo" to="/">MusicXmlDb</router-link>
        <router-link to="/about">About</router-link>

        <div v-if="keycloak.authenticated">
            <p style="display: inline-block;">Hello!</p>
            <a @click="logout"> Logout </a>

        </div>
        <div v-else>
            <a @click="login"> Login </a>
            <a @click="login"> Register </a>
        </div>
    </div>
</template>

<style lang="css" scoped>
/* Add a black background color to the top navigation */
.topnav {
    overflow: hidden;
}

/* Style the links inside the navigation bar */
.topnav a p {
    float: left;
    text-align: center;
    padding: 14px 16px;
    text-decoration: none;
    font-size: 17px;
}
</style>