<script setup lang="ts">
import { ref, onMounted } from "vue";
import { getUserScores } from "../../services/api";

const scores = ref<Array<{ id: string; name: string; views: number; created: string; modified: string; isPublic: boolean }>>([]);

onMounted(async () => {
    const rawScores = await getUserScores();
    scores.value = rawScores.map(score => ({
        ...score,
        created: new Date(score.created).toLocaleDateString(),
        modified: new Date(score.modified).toLocaleDateString()
    }));
});
</script>

<template>
    <main class="scores-page">
        <h1 class="scores-title">Your Sheet Music</h1>

        <ul v-if="scores.length">
            <li v-for="score in scores" :key="score.id" class="score-item">
                <router-link :to="`/scores/edit?id=${score.id}`" class="score-link">
                    <span class="score-name">{{ score.name }}</span><br>
                    <span class="score-details">Views: {{ score.views }} | Created: {{ score.created }} | Modified: {{
                        score.modified }} | </span>
                    <span v-if="score.isPublic" class="public-badge">Public</span>
                    <span v-else class="private-badge">Private</span>
                </router-link>
            </li>
        </ul>

        <p v-else class="no-scores">No sheet music found.</p>
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
    font-size: 1.875rem; /* Equivalent to text-3xl */
    font-weight: bold;
    margin-bottom: 1.5rem;
}

.score-link {
    display: block;
    color: var(--text-light);
    text-decoration: none;
}

.score-link:hover {
    opacity: 0.8;
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

.score-name {
    font-size: 1.25rem; /* Equivalent to text-xl */
    font-weight: 600;
}

.score-details {
    font-size: 0.875rem; /* Equivalent to text-sm */
    color: #4b5563; /* Equivalent to gray-600 */
    margin-top: 0.5rem;
}

.public-badge {
    font-size: 0.75rem; /* Equivalent to text-xs */
    font-weight: bold;
    color: #16a34a; /* Equivalent to green-600 */
    margin-top: 0.25rem;
}

.private-badge {
    font-size: 0.75rem; /* Equivalent to text-xs */
    font-weight: bold;
    color: #c8db1c; /* Equivalent to green-600 */
    margin-top: 0.25rem;
}

.no-scores {
    font-size: 1.125rem; /* Equivalent to text-lg */
    color: #6b7280; /* Equivalent to gray-500 */
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

    .score-details {
        color: #d1d5db; /* Equivalent to gray-300 */
    }

    .public-badge {
        color: #4ade80; /* Equivalent to green-400 */
    }

    .private-badge {
        color: #c7d643; /* Equivalent to green-400 */
    }

    .no-scores {
        color: #9ca3af; /* Equivalent to gray-400 */
    }
}
</style>