<template>

    <main class="score-view">

        <p v-if="unauthorized" class="error-message">🚫 You are not allowed to view this document.</p>
        <div v-else>
            <h1 class="score-title">{{ score?.name ?? "Loading..." }}</h1>

            <template v-if="score !== null">
                <p class="modified-date">Last Modified: {{ formatDate(new Date(score.modified)) }}</p>

                <div class="version-selection">
                    <label for="version-select" class="version-label">Select Version:</label>
                    <select id="version-select" v-model="selectedVersion" @change="fetchSelectedVersion">
                        <option v-for="version in score.history" :key="version.id" :value="version.id">
                            {{ formatDate(new Date(version.created)) }}
                        </option>
                    </select>
                </div>

                <div id="score_document" class="score-display"></div>
            </template>
        </div>

    </main>
</template>


<script setup lang="ts">
import { ref, onMounted, watch } from "vue";
import { useRoute } from "vue-router";
import { getScoreDetails } from "@/services/manageScoreDocuments";
import { OpenSheetMusicDisplay, type IOSMDOptions } from "opensheetmusicdisplay";
import type { ScoreDocument } from "@/models/ScoreDocument";
import { formatDate } from "@/services/timeAndDate";
import { AxiosError } from "axios";
import { getMusicXMLDocument } from "@/services/viewScoreDocuments";

const route = useRoute();
const score = ref<ScoreDocument | null>(null);
const selectedVersion = ref<string | null>(null);
const unauthorized = ref<boolean>(false);
const options: IOSMDOptions = {};

// Fetch Score Details
async function fetchScoreDetails() {
    const scoreId = route.query.id as string;
    if (!scoreId) return;

    try {
        score.value = await getScoreDetails(scoreId);

        // Default to latest version
        if (score.value?.history.length) {
            selectedVersion.value = score.value.history[0].id;
        }
    } catch (err) {
        const error = err as AxiosError; // Type assertion

        if (error.response?.status === 401) {
            unauthorized.value = true; // Set flag for restricted access
        } else {
            console.error("Error fetching score details:", error.message);
        }
    }
}

// Fetch the selected version's sheet music
async function fetchSelectedVersion() {
    if (!score.value || !selectedVersion.value || unauthorized.value) return;

    try {
        const musicXML = await getMusicXMLDocument(score.value.id, selectedVersion.value);
        if (musicXML == null){
            return;
        }
        const viewer = new OpenSheetMusicDisplay("score_document", options);
        await viewer.load(musicXML);
        viewer.render();
    } catch (error) {
        console.error("Error loading sheet music:", error);
    }
}

watch(selectedVersion, fetchSelectedVersion); // Automatically update when selection changes

onMounted(fetchScoreDetails);
</script>

<style lang="css" scoped>
#score_document {
    background-color: white;
}

.error-message {
    font-size: 1.2rem;
    color: red;
    font-weight: bold;
    margin-top: 1rem;
    text-align: center;
}

.version-selection {
    display: flex;
    flex-direction: column;
    align-items: flex-start;
    margin-bottom: 1rem;
}

.version-label {
    font-size: 1rem;
    font-weight: bold;
}

#version-select {
    padding: 0.5rem;
    border-radius: 6px;
    border: 1px solid #ccc;
    width: 100%;
    max-width: 20rem;
    background-color: var(--secondary-light);
    color: var(--text-light);
}

@media (prefers-color-scheme: dark) {
    #version-select {
        border-color: #666;
        background-color: var(--secondary-dark);
        color: var(--text-dark);
    }
}
</style>