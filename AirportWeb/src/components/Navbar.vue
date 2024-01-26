<template>
    <nav class="navbar navbar-expand-lg bg-body-tertiary border-bottom border-body">
        <div class="container-fluid">
            <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav"
                aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation">
                <span class="navbar-toggler-icon"></span>
            </button>
            <div class="form-check form-switch me-2">
                <input class="form-check-input bg-dark border-light" type="checkbox" role="switch" id="flexSwitchCheckDefault"
                v-model="isLightTheme">
                <label class="form-check-label" for="flexSwitchCheckDefault">
                    <i class="bi bi-sun-fill" v-if="isLightTheme"></i>
                    <i class="bi bi-moon-fill" v-else></i>
                </label>
            </div>
            <div class="collapse navbar-collapse justify-items-center" id="navbarNav">
                <ul class="navbar-nav fw-bold align-items-center">
                    <li class="nav-item">
                        <router-link to="/" aria-current="page" class="nav-link">Home</router-link>
                    </li>
                    <li class="nav-item">
                        <router-link to="/booking" aria-current="page" class="nav-link">Booking</router-link>
                    </li>
                    <li class="nav-item">
                        <router-link to="/summary" aria-current="page" class="nav-link">Summary</router-link>
                    </li>
                </ul>
            </div>
        </div>
    </nav>
</template>
<script>
export default {
    mounted() {
        this.loadThemeFromLocalStorage()
    },
    data() {
        return {
            isLightTheme: null
        }
    },
    watch: {
        isLightTheme(newVal) {
            if (newVal) {
                document.body.removeAttribute('data-bs-theme')
                localStorage.setItem('theme', 'light')
            } else {
                document.body.setAttribute('data-bs-theme', 'dark')
                localStorage.setItem('theme', 'dark')
            }
        }
    },
    methods: {
        toggleTheme() {
            this.isLightTheme = !this.isLightTheme
        },
        loadThemeFromLocalStorage() {
            const savedTheme = localStorage.getItem('theme')
            if (savedTheme === 'dark') {
                this.isLightTheme = false
            } else {
                this.isLightTheme = true
            }
        }
    }
}
</script>
