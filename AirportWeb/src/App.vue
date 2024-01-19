<template>
    <Navbar />
    <div class="container-fluid">
        <h1>Hello World!</h1>
        <router-view></router-view>
    </div>
</template>

<script>
import Navbar from '@components/Navbar.vue'

export default {
    data() {
        return {

        }
    },
    methods: {

    },
    components: {
        Navbar
    },
    computed: {

    }
}
</script>