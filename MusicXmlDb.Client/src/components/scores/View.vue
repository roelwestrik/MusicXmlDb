<template>
    <h1>Scores</h1>
    <p>{{ score?.name }}</p>

    <select class="form-control" v-model="selectedHistory">
        <option v-for="item in (score?.history ?? [])" :value="item" :key="item.id">
            {{ item.id }}
        </option>
    </select>

    <p>
        {{ selectedHistory?.id ?? "" }}
    </p>

    <div id="score_document" style="background-color: white;">

    </div>

</template>

<script setup lang="ts">
import { ref, watch, type Ref } from 'vue';
import { useRoute } from 'vue-router'
import { getUpdatedToken } from "@/services/auth"
import { OpenSheetMusicDisplay, type IOSMDOptions } from 'opensheetmusicdisplay';
import type { ScoreDocument } from '@/models/ScoreDocument';
import type { ScoreDocumentHistory } from '@/models/ScoreDocumentHistory';


const options: IOSMDOptions = {

}

const score: Ref<ScoreDocument | null> = ref(null)
const selectedHistory: Ref<ScoreDocumentHistory | null> = ref(null)

const id = useRoute().query.id


getUpdatedToken().then(token => {
    const headers = { 'Authorization': `Bearer ${token}` };
    const uri = `https://localhost:8081/api/scoredocuments/${id}`
    fetch(uri, { headers })
        .then(r => r.json())
        .then(json => {
            score.value = json
            selectedHistory.value = json.history[0]
        })
})

watch(selectedHistory, (_new) => {
    if (_new === null) {
        return;
    }

    console.log("Changed")

    getUpdatedToken().then(token => {
        const headers = { 'Authorization': `Bearer ${token}` };
        const uri = `https://localhost:8081/api/musicxmldocuments/${id}/${_new.id}`
        fetch(uri, { headers })
            .then(r => r.text())
            .then(async text => {
                const viewer = new OpenSheetMusicDisplay("score_document", options)

                await viewer.load(text);
                return viewer.render();
            })
            
    })

})

</script>

<style></style>