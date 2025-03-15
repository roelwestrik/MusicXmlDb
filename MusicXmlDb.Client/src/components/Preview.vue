<template>
    <h1 class="title">Preview 2</h1>

    <div id="viewer">

    </div>
</template>

<script lang="ts" setup>
import { OpenSheetMusicDisplay, type IOSMDOptions } from 'opensheetmusicdisplay';
import { onMounted } from 'vue';

onMounted(() => {
    const color = window.getComputedStyle(document.querySelector(".title")!).color

    const options: IOSMDOptions = {
        defaultColorMusic: color
    }

    const viewer = new OpenSheetMusicDisplay("viewer", options)

    viewer.load("./test.xml").then(() => viewer.render())
})
</script>

<style scoped></style>