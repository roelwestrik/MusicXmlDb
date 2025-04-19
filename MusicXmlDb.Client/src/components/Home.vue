<template>
  <main>
      <h1>Welcome to Our Website!</h1>
      <p>We're glad to have you here. Explore and enjoy!</p>
  </main>
</template>

<script setup lang="ts">

</script>

<style lang="css" scoped>
main {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    text-align: center;
    min-height: 100vh;
}

h1 {
    font-size: 2.25rem; /* Equivalent to text-4xl */
    font-weight: bold;
}

p {
    font-size: 1.125rem; /* Equivalent to text-lg */
    margin-top: 0.5rem;
}

</style>