<template>
    <div class="file-upload-container">
        <label for="file-upload" class="upload-label">Upload File:</label>
        <div class="file-input-wrapper">
            <input type="file" id="file-upload" @change="handleFileChange" accept=".xml,.musicxml"
                class="upload-input" />
            <button type="button" class="btn clear-file-button" @click="resetFileSelection" :disabled="!selectedFile">
                ✖
            </button>
        </div>
    </div>
</template>

<script setup>
import { ref, defineEmits } from "vue";

const selectedFile = ref(null);
const emit = defineEmits(["fileChange"]);

const handleFileChange = (event) => {
    selectedFile.value = event.target.files[0] || null;
    emit("fileChange", selectedFile.value);
};

const resetFileSelection = () => {
    selectedFile.value = null;
    document.getElementById("file-upload").value = "";
    emit("fileChange", null);
};
</script>

<style lang="css" scoped>

.file-upload-container {
    display: flex;
    flex-direction: column;
    gap: 8px;
}


.upload-label {
    font-size: 1rem;
    font-weight: 500;
    color: var(--text-light);
}

.file-input-wrapper {
    display: flex;
    align-items: center;
    gap: 10px;
}

.upload-input {
    flex-grow: 1;
    padding: 10px;
    font-size: 1rem;
    border: 2px solid var(--border-light);
    border-radius: 0.375rem;
    background-color: var(--primary-light);
    color: var(--text-light);
}

.clear-file-button:disabled {
    opacity: 0.3;
    cursor: not-allowed;
}


@media (prefers-color-scheme: dark) {
    .upload-label {
        color: var(--text-dark);
    }

    .upload-input,
    .upload-text-input {
        background-color: var(--primary-dark);
        border: 1px solid var(--border-dark);
        color: var(--text-dark);
    }
}
</style>