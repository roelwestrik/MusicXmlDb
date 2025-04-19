<template>
    <main class="upload-score-document">
        <h1>Upload New Score Document</h1>
        <form @submit.prevent="submitDocument">

            <!-- Name Input -->
            <label for="name" class="upload-label">Name:</label>
            <input type="text" id="name" v-model="name" class="upload-text-input" @input="validateName" required />
            <span v-if="nameError" class="error-message">{{ nameError }}</span>

            <!-- File Upload Component -->
            <FileUpload @fileChange="updateSelectedFile" />
            <span v-if="fileError" class="error-message">{{ fileError }}</span>

            <!-- Public Checkbox -->
            <label class="upload-checkbox-label">
                <input type="checkbox" v-model="isPublic" class="upload-checkbox" />
                Public Document
            </label>

            <!-- Upload Button -->
            <button type="button" class="btn upload-button" @click="submitDocument" :disabled="!isValid">
                Upload
            </button>
        </form>

        <router-link to="/scores" class="btn sticky-button">
            < Back to scores</router-link>

    </main>


</template>

<script setup>
import FileUpload from "@/components/scores/FileUpload.vue";
import { ref, computed } from "vue";
import { useRouter } from "vue-router";
import { createScore } from "@/services/api"

const router = useRouter();

const name = ref("");
const isPublic = ref(false);
const selectedFile = ref(null);

const nameError = ref("");
const fileError = ref("");

const updateSelectedFile = (file) => {
    selectedFile.value = file;
};

const validateName = () => {
    nameError.value = name.value.trim() ? "" : "Name is required.";
};

const isValid = computed(() => name.value.trim() && selectedFile.value);

async function submitDocument() {
    validateName();
    updateSelectedFile(selectedFile.value);

    if (!isValid.value) {
        return;
    }

    const result = await createScore(name.value, isPublic.value, selectedFile.value)
    if (result == null) {
        alert("Something went wrong.")
        return
    }

    router.push(`/ScoreDocument?id=${result.id}`)
};
</script>

<style>
form {
    display: flex;
    flex-direction: column;
    gap: 16px;
}

.upload-label {
    font-size: 1rem;
    font-weight: 500;
}

.error-message {
  font-size: 0.875rem;
  color: var(--accent-red);
  margin-top: 4px;
}

.upload-score-document {
    max-width: 500px;
    margin: auto;
    margin-top: 50px;
    padding: 16px;
    background-color: var(--secondary-light);
    border-radius: 0.375rem;
    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
    display: flex;
    flex-direction: column;
    gap: 16px;
}

.upload-text-input {
    width: 100%;
    padding: 12px;
    font-size: 1rem;
    font-weight: 500;
    border: 2px solid var(--border-light);
    border-radius: 0.375rem;
    background-color: var(--primary-light);
    color: var(--text-light);
    transition: border-color 0.3s ease-in-out, box-shadow 0.3s ease-in-out;
}

.upload-text-input:focus {
    border-color: var(--accent-red);
    box-shadow: 0 0 6px rgba(255, 69, 69, 0.5);
    outline: none;
}


.upload-input,
.upload-text-input {
    width: 100%;
    padding: 10px;
    font-size: 1rem;
    border: 1px solid var(--border-light);
    border-radius: 0.375rem;
    background-color: var(--primary-light);
    color: var(--text-light);
}

.upload-checkbox {
    transform: scale(1.5);
    margin-right: 10px;
    cursor: pointer;
}

.upload-checkbox-label {
    font-size: 1.125rem;
    font-weight: 500;
    display: flex;
    align-items: center;
    gap: 8px;
}

.upload-button,
.reset-button {
    width: 100%;
    padding: 12px;
    font-size: 1rem;
    font-weight: 600;
    border-radius: 0.375rem;
    border: none;
    cursor: pointer;
    text-align: center;
    transition: background-color 0.2s ease-in-out;
}

.upload-button {
    background-color: var(--accent-red);
    color: white;
}

.upload-button:disabled {
    background-color: #ccc;
    cursor: not-allowed;
}

.sticky-button {
    position: fixed;
    bottom: 20px;
    right: 20px;
    text-decoration: none;
}

@media (prefers-color-scheme: dark) {
    .upload-score-document {
        background-color: var(--secondary-dark);
    }

    .upload-text-input{
        background-color: var(--primary-dark);
        color: var(--text-dark);
    }
}
</style>