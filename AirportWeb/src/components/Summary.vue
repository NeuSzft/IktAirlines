<template>
    <div id="summary" class="container mt-5">
        <h1 class="text-center mb-4">Summary</h1>
        <div id="tickets" class="" v-if="tickets.length > 0">
            <div class="row mt-5 card" v-for="(ticket, index) in tickets" :key="index">
                <div class="card py-3">
                    <div class="fs-3 fw-bold d-flex justify-content-between">
                        <div class="col-11 mt-0 align-middle">
                            {{ tickets[index].route.origin }}→{{ tickets[index].route.final }}
                        </div>
                        <div class="col-1 mt-0 ">
                            <button type="button" class="btn align-middle w-100 h-100" @click="toggleTickets(index)">
                                <i :class="showTickets[index] ? 'bi bi-chevron-up' : 'bi bi-chevron-down'"></i>
                            </button>
                        </div>
                    </div>
                </div>
                <div v-if="showTickets[index]" class="tickets col-12 col-md d-flex flex-wrap gap-3 mt-3">
                    <Ticket class="col-sm-12 col-lg w-75" v-for="id in ticket.route.flightIds" :key="id" :flightId="id"
                        :adults="ticket.adults" :children="ticket.children" />
                    <div class="container col-12 d-flex flex-row text-center justify-content-around fs-4 pb-4" v-if="showTickets[index]">
                        <div class="col-3">
                            <div class="col-12"> Total distance </div>
                            <div class="col-12 fw-bold"> {{ getTotalTravelDistance(index) }} km </div>
                        </div>
                        <div class="col-3">
                            <div class="col-12"> Total flight time </div>
                            <div class="col-12 fw-bold"> {{ getTotalTravelTime(index) }} min </div>
                        </div>
                        <div class="col-3">
                            <div class="col-12"> Total cost </div>
                            <div class="col-12 fw-bold"> {{ ticket.price }} HUF </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div id="problem" class="d-flex flex-column text-center gap-5 mt-5" v-else>
            <h2 class="fw-bold">No tickets were purchased</h2>
            <router-link to="/booking" aria-current="page"
                class="btn btn-outline-primary d-block align-self-center fs-3 fw-bold w-50">Buy a ticket!</router-link>
        </div>
    </div>
</template>

<script>
import Ticket from "@components/Ticket.vue"

export default {
    data() {
        return {
            tickets: [],
            showTickets: []
        }
    },
    mounted() {
        this.tickets = JSON.parse(localStorage.getItem('savedRoutes')) || []
        this.showTickets = Array(this.tickets.length).fill(false)
    },
    methods: {
        toggleTickets(index) {
            this.showTickets[index] = !this.showTickets[index]
        },
        getTotalTravelDistance(index) {
            return this.tickets[index].route.distances.reduce((a, b) => a + b)
        },
        getTotalTravelTime(index) {
            return this.tickets[index].route.flightTimes.reduce((a, b) => a + b)
        },
        getFlightById(flightId) {
            return this.flights.find(flight => flight.id === flightId);
        }
    },
    components: {
        Ticket
    }
}
</script>
