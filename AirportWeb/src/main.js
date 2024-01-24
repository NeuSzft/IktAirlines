import "bootstrap"
import "bootstrap/dist/css/bootstrap.min.css"
import { createApp } from "vue"
import App from "./App.vue"
import router from "@utils/router"
import 'bootstrap-icons/font/bootstrap-icons.css'

const app = createApp(App)
app.use(router).mount('#app')