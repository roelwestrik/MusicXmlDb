<script setup lang="ts">
import { ref, onMounted, onUnmounted } from "vue";
import { keycloak, login, logout, getUserName } from "@/services/auth";
import { useRouter } from "vue-router";

const userName = ref("Unknown user");
getUserName().then(e => {
    userName.value = e;
});

const router = useRouter();
const dropdownOpen = ref(false);

const goToScores = () => {
    router.push("/scores");
};

const toggleDropdown = () => {
    dropdownOpen.value = !dropdownOpen.value;
};

const closeDropdown = (event: MouseEvent) => {
    const dropdownElement = document.querySelector(".dropdown");
    if (dropdownElement && !dropdownElement.contains(event.target as Node)) {
        dropdownOpen.value = false;
    }
};

// Attach event listener when dropdown opens
onMounted(() => {
    document.addEventListener("click", closeDropdown);
});

// Remove event listener when component unmounts
onUnmounted(() => {
    document.removeEventListener("click", closeDropdown);
});
</script>

<template>
    <header class="header">
        <router-link to="/" class="logo">
            <img src="@/assets/logo.svg" alt="Logo" class="h-10">
        </router-link>

        <nav class="nav">
            <span v-if="keycloak.authenticated" class="greet">Hello, {{ userName }}!</span>

            <div v-if="keycloak.authenticated" class="dropdown">
                <button @click="toggleDropdown" class="btn">Menu ▼</button>

                <div v-if="dropdownOpen" class="dropdown-menu">
                    <button @click="goToScores" class="dropdown-item">Go to Your Scores</button>
                    <button @click="logout" class="dropdown-item">Logout</button>
                </div>
            </div>

            <template v-else>
                <button @click="login" class="btn">Login</button>
                <button @click="login" class="btn">Register</button>
            </template>
        </nav>
    </header>
</template>


<style lang="css" scoped>
/* Header */
.header {
    padding: 1rem;
    display: flex;
    justify-content: space-between;
    align-items: center;
    background-color: var(--secondary-light);
    color: var(--text-light);
}

/* Logo */
.logo img {
    height: 2.5rem;
    font-size: 1.25rem;
    font-weight: bold;
}

.greet{
    font-size: 1.125rem;
}

.nav {
    font-size: 1.25rem; /* Equivalent to text-xl */
    font-weight: bold; /* Equivalent to font-bold */
    display: flex;
    align-items: center;
    display: flex;
    align-items: center;
    gap: 1rem;
}


.dropdown {
    position: relative;
}

/* Dropdown */
.dropdown-menu {
    background-color: var(--secondary-light);
    color: var(--text-light);
    border-radius: 0.375rem;
    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
    padding: 0.5rem;
    min-width: 180px;
    position: absolute;
    top: 100%;
    right: 0;
}

.dropdown-item {
    padding: 0.5rem 1rem;
    width: 100%;
    text-align: left;
    background-color: var(--secondary-light);
    color: var(--secondary-light);
    border: none;
}

.dropdown-item:hover {
    background-color: #e5e7eb; /* Equivalent to gray-200 */
}

@media (prefers-color-scheme: dark) {
    .header {
        background-color: var(--secondary-dark);
        color: var(--text-dark);
    }
    
    .dropdown-menu {
        background-color: var(--secondary-dark);
        color: var(--text-dark);
    }
    .dropdown-item {
        background-color: var(--secondary-dark); /* Equivalent to gray-700 */
    }

    .dropdown-item:hover {
        background-color: var(--primary-dark); /* Equivalent to gray-700 */
    }
}
</style>