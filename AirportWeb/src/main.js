import "bootstrap"
import "bootstrap/dist/css/bootstrap.min.css"
import { createApp } from "vue"
import App from "./App.vue"
import { http } from "@utils/http"

const app = createApp(App)
app.mount('#app')