import axios from "axios"

const apiBaseUrl = import.meta.env.VITE_API_URL || 'http://localhost:5000'

export const http = axios.create({
    baseURL: apiBaseUrl,
    headers: {
        Accept: "application/json",
        "Content-Type": "application/json"
    }
})