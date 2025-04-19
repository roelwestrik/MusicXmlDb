<script setup lang="ts">
import { ref, onMounted } from "vue";
import { getUserScores, deleteScoreDocument } from "@/services/api";

const scores = ref<Array<{ id: string; name: string; views: number; created: string; modified: string; isPublic: boolean }>>([]);

onMounted(refreshList);

async function refreshList(){
    const rawScores = await getUserScores();
    scores.value = rawScores.map(score => ({
        ...score,
        created: new Date(score.created).toLocaleDateString(),
        modified: new Date(score.modified).toLocaleDateString()
    }));
}

async function deleteScore(scoreId: string) {
    const confirmed = confirm("Are you sure you want to completely delete this score? There is no undo.");
    if (!confirmed) return;

    const success = await deleteScoreDocument(scoreId);
    if (success){
        await refreshList();
    } else {
        alert("Something went wrong.")
    }
}

</script>

<template>
    <main class="scores-page">
        <h1 class="scores-title">Your Sheet Music</h1>

        <ul v-if="scores.length">
            <li v-for="score in scores" :key="score.id" class="score-item">
                <div class="score-content">
                    <div class="score-info">
                        <router-link :to="`/scores/edit?id=${score.id}`" class="score-link">
                            <span class="score-name">{{ score.name }}</span>
                            <span>
                                Views: {{ score.views }} | Created: {{ score.created }} | Modified: {{ score.modified }} | 
                            </span>
                            <span v-if="score.isPublic" class="public-badge">Public</span>
                            <span v-else class="private-badge">Private</span>
                        </router-link>
                    </div>
                    <div class="score-actions">
                        <router-link :to="`/scores/edit?id=${score.id}`"><button class="btn">Edit</button></router-link>
                        <button @click="deleteScore(score.id)" class="btn action-button delete-button">✖</button>
                    </div>
                </div>
            </li>
        </ul>


        <p v-else class="no-scores">No sheet music found.</p>

        <router-link to="/scores/create">
            <button class="btn">Create new</button>
        </router-link>
    </main>
</template>

<style lang="css" scoped>
.scores-page {
    padding: 8px;
    text-align: center;
    background-color: var(--primary-light);
    color: var(--text-light);
}

.scores-title {
    font-size: 1.875rem;
    font-weight: bold;
    margin-bottom: 1.5rem;
}

.score-item {
    display: flex;
    flex-direction: column;
    align-items: center;
    background-color: var(--secondary-light);
    padding: 1rem;
    border-radius: 0.375rem;
    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
    margin: 1rem 0;
}

.score-content {
    display: flex;
    justify-content: space-between;
    align-items: center;
    width: 100%;
}

.score-info {
    flex-grow: 1;
}

.score-link {
    display: block;
    color: var(--text-light);
    text-decoration: none;
}

.score-link:hover {
    opacity: 0.8;
}

.score-name {
    display: block;
    text-decoration: none;
    font-size: 1.875rem;
    font-weight: bold;
}

.score-actions {
    display: flex;
    gap: 8px;
}

.action-button {
    width: 32px;
    height: 32px;
    display: flex;
    justify-content: center;
    align-items: center;
    font-size: 1.5rem;
    border: none;
    cursor: pointer;
    transition: opacity 0.2s ease-in-out;
    border-radius: 50%;
}

.public-badge {
    font-weight: bold;
    color: #16a34a;
}

.private-badge {
    font-weight: bold;
    color: #c8db1c;
}

.no-scores {
    font-size: 1.125rem;
    /* Equivalent to text-lg */
    color: #6b7280;
    /* Equivalent to gray-500 */
}

@media (prefers-color-scheme: dark) {
    .scores-page {
        background-color: var(--primary-dark);
        color: var(--text-dark);
    }

    .score-link {
        color: var(--text-dark);
    }

    .score-item {
        background-color: var(--secondary-dark);
    }

    .public-badge {
        color: #4ade80;
        /* Equivalent to green-400 */
    }

    .private-badge {
        color: #c7d643;
        /* Equivalent to green-400 */
    }

    .no-scores {
        color: #9ca3af;
        /* Equivalent to gray-400 */
    }
}
</style>