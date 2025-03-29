<template>
    <h1>Scores</h1>
    <div v-for="score in scores">
        <p>{{ score.name }}</p><router-link :to="`scores/view?id=${score.id}`">CLICK</router-link>
    </div>

    
</template>

<script setup lang="ts">
import { ref, type Ref } from 'vue';
import { getUpdatedToken } from "@/services/auth"
import type { ScoreDocument } from '@/models/ScoreDocument';


const scores: Ref<ScoreDocument[]> = ref([])

getUpdatedToken().then(token => {
    const headers = { 'Authorization': `Bearer ${token}` };
    fetch("https://localhost:8081/api/scoredocuments", { headers })
    .then(r => r.json())
    .then(json => {
        scores.value = json
    })
})

</script>

<style>

</style>