<template>
    <header>
        <Navbar />
    </header>
    <main class="container">
        <slot />
    </main>
</template>

<script>
import Navbar from '@components/Navbar.vue'

export default {
    components: {
        Navbar
    }
}
</script>