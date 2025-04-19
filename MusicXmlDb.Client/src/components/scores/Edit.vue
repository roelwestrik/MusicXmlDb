<template>
    <main class="score-view" v-if="score !== null">
        <h1 class="score-title">Edit Score Document</h1>
        <p class="view-document-link">
            <router-link :to="`/scoredocument?id=${route.query.id}`">View this document</router-link>
        </p>


        <div class="score-controls">
            <p class="modified-date">{{ getRelativeTime(new Date(score?.modified)) }}</p>

            <label class="form-label">Score Name:</label>
            <input v-model="score.name" class="form-input" type="text" />

            <label class="form-label mt-4">Visibility:</label>
            <select v-model="score.isPublic" class="form-input">
                <option :value="true">Public</option>
                <option :value="false">Private</option>
            </select>

            <button class="btn save-button" @click="saveChanges">Save Changes</button>
            <p v-if="saveStatus" class="save-message">{{ saveStatus }}</p>
        </div>


        <!-- Score History Table -->
        <h2 class="history-title">Version History</h2>

        <div class="score-controls">
            <FileUpload @fileChange="updateSelectedFile" style="width: 100%;"></FileUpload>

            <button @click="uploadNewVersion" :disabled="!selectedFile" class="btn save-button">
                Upload
            </button>
        </div>

        <table class="history-table">
            <thead>
                <tr>
                    <th>Version ID</th>
                    <th>Created Date</th>
                    <th>Actions</th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="version in score?.history" :key="version.id">
                    <td>{{ version.id }}</td>
                    <td>{{ formatDate(new Date(version.created)) }}</td>
                    <td>
                        <button v-if="score?.history.length > 1" class="btn delete-button"
                            @click="deleteVersion(version.id)">Delete</button>
                        <button class="btn download-button" @click="downloadVersion(version.id)">Download</button>
                    </td>
                </tr>
            </tbody>
        </table>

        <div class="score-controls">
            <button class="btn" @click="deleteScore">Delete this score</button>
        </div>

        <router-link to="/scores" class="btn sticky-button">
            < Back to scores</router-link>

    </main>
</template>

<script setup lang="ts">
import { ref, onMounted } from "vue";
import { useRoute, useRouter } from "vue-router";
import { getScoreDetails, updateScore, uploadScoreVersion, deleteScoreVersion, downloadScoreVersion, deleteScoreDocument } from "@/services/api";
import type { ScoreDocument } from "@/models/ScoreDocument";
import { getRelativeTime, formatDate } from "@/services/timeAndDate";
import FileUpload from "@/components/scores/FileUpload.vue"

const route = useRoute();
const router = useRouter();
const score = ref<ScoreDocument | null>(null);
const saveStatus = ref<string | null>(null);
const selectedFile = ref<File | null>(null);

const updateSelectedFile = (file: File | null) => {
    selectedFile.value = file;
};

onMounted(fetchScoreDetails);

// Fetch Score Details
async function fetchScoreDetails() {
    const scoreId = route.query.id as string;
    if (!scoreId) return;

    score.value = await getScoreDetails(scoreId);
}

// Save Changes
async function saveChanges() {
    if (!score.value) return;

    const success = await updateScore(route.query.id as string, {
        id: score.value.id,
        name: score.value.name,
        isPublic: score.value.isPublic
    });

    if (success) {
        await fetchScoreDetails(); // Refresh data dynamically
        saveStatus.value = "Score updated successfully!";
        setTimeout(() => saveStatus.value = null, 3000);
    }
}


async function uploadNewVersion() {
    if (!selectedFile.value) {
        alert("Please select a file first.");
        return;
    }

    const scoreId = route.query.id as string;
    const success = await uploadScoreVersion(scoreId, selectedFile.value);

    if (success) {
        alert("Score version uploaded successfully!");
        await fetchScoreDetails(); // Refresh to show the new version
    } else {
        alert("Failed to upload score version.");
    }
}


async function deleteVersion(versionId: string) {
    if (!score.value) {
        return;
    }

    if (!score.value.history || score.value.history.length <= 1) {
        alert("Deletion not allowed: You must have at least one remaining version.");
        return;
    }

    const scoreId = route.query.id as string;
    if (!scoreId) return;

    const confirmed = confirm("Are you sure you want to delete this version?");
    if (!confirmed) return;

    const success = await deleteScoreVersion(scoreId, versionId);

    if (success) {
        alert("Version deleted successfully!");
        await fetchScoreDetails(); // Refresh table
    } else {
        alert("Failed to delete version.");
    }
}


async function downloadVersion(versionId: string) {
    if (!score.value) {
        return;
    }

    try {
        await downloadScoreVersion(score.value.id, versionId);
    } catch (error) {
        console.error("Error downloading score version:", error);
        alert("Failed to download file.");
    }
}


async function deleteScore() {
    if (!score.value) {
        return;
    }

    const scoreId = route.query.id as string;
    if (!scoreId) return;

    const confirmed = confirm("Are you sure you want to completely delete this score? There is no undo.");
    if (!confirmed) return;

    const success = await deleteScoreDocument(scoreId);
    if (success){
        router.push("/scores")
    } else {
        alert("Something went wrong.")
    }
}

</script>

<style scoped>
.score-view {
    display: flex;
    flex-direction: column;
    align-items: center;
    padding: 2rem;
}

.score-title {
    font-size: 2rem;
    font-weight: bold;
    margin-bottom: 1.5rem;
}

.view-document-link {
    font-size: 1rem;
    margin-bottom: 1.5rem;
}

.view-document-link a {
    color: var(--accent-blue);
    text-decoration: none;
    font-weight: bold;
}

.view-document-link a:hover {
    text-decoration: underline;
}

.score-controls {
    display: flex;
    flex-direction: column;
    align-items: flex-start;
    width: 100%;
    max-width: 32rem;
    text-align: left;
}

.form-label {
    font-size: 1.125rem;
    font-weight: 600;
}

.form-input {
    width: 100%;
    padding: 0.5rem;
    border-radius: 6px;
    border: 1px solid #ccc;
    background-color: var(--secondary-light);
    color: var(--text-light);
}

@media (prefers-color-scheme: dark) {
    .form-input {
        border-color: #666;
        background-color: var(--secondary-dark);
        color: var(--text-dark);
    }
}

.save-button {
    margin-top: 1rem;
    padding: 0.5rem 1rem;
    font-weight: bold;
    border: none;
    border-radius: 6px;
}

.save-message {
    margin-top: 0.5rem;
    font-size: 1rem;
    font-weight: 600;
    color: green;
}

@media (prefers-color-scheme: dark) {
    .save-message {
        color: #50fa7b;
        /* Dark mode green */
    }
}

.history-title {
    font-size: 1.5rem;
    font-weight: bold;
    margin-top: 2rem;
    text-align: left;
}

.history-table {
    width: 100%;
    border-collapse: collapse;
    margin-top: 1rem;
}

.history-table th,
.history-table td {
    border: 1px solid #ccc;
    padding: 0.75rem;
    text-align: left;
}

.view-link {
    color: var(--accent-red);
    text-decoration: none;
    font-weight: 600;
}

.view-link:hover {
    text-decoration: underline;
}

.upload-section {
    display: flex;
    flex-direction: column;
    width: 100%;
    max-width: 32rem;
    margin-top: 1rem;
}

.upload-label {
    font-size: 1rem;
    font-weight: 600;
}

.upload-input {
    width: 100%;
    max-width: 20rem;
    padding: 0.5rem;
    border-radius: 6px;
    border: 1px solid #ccc;
}

.upload-button {
    margin-top: 1rem;
    padding: 0.5rem 1rem;
    font-weight: bold;
    border: none;
    border-radius: 6px;
}

.delete-button {
    margin-left: 1rem;
    padding: 0.4rem 0.8rem;
}

.download-button {
    margin-left: 1rem;
    padding: 0.4rem 0.8rem;
}

.sticky-button {
    position: fixed;
    bottom: 20px;
    right: 20px;
    text-decoration: none;
}
</style>
