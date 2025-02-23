<template>
    <div class="register">
        <Header />
        <h2>Register</h2>
        <form @submit.prevent="register">
            <label for="username">Username:</label>
            <input type="text" v-model="username" required />
            <label for="email">Email:</label>
            <input type="email" v-model="email" required />
            <label for="password">Password:</label>
            <input type="password" v-model="password" required />
            <button type="submit">Register</button>
        </form>
    </div>
</template>

<script>
import Header from './Header.vue';

export default {
    name: 'Register',
    components: {
        Header,
    },
    data() {
        return {
            username: '',
            email: '',
            password: '',
        };
    },
    methods: {
        async register() {
            // Registration logic
        },
    },
};
</script>

<style scoped>
/* Add your styles here */
</style>