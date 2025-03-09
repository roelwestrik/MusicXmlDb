<script setup lang="ts">
import Home from './components/Home.vue'
import Header from './components/Header.vue'
</script>

<template>

  <Header></Header>

  <div class="body">

    <router-view></router-view>

  </div>

</template>

<style scoped>
.body {
  padding-left: 100px;
  padding-right: 100px;
}
</style>
