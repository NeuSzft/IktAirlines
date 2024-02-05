<template>
    <PageLayout>
        <div id="summary" class="container my-5">
            <h1 class="text-center mb-5">Booked flights</h1>
            <div id="tickets" v-if="tickets.length > 0">
                <div class="row my-4 card" v-for="(ticket, index) in tickets" :key="index" :id="`ticket-${index}`">
                    <div class="card py-3">
                        <div
                            class="fw-bold d-flex flex-wrap text-center text-lg-start justify-content-center justify-content-lg-between">
                            <div class="col-12 col-lg-8 col-xl-9 mt-0 mb-3 mb-lg-0 align-self-center fs-3">
                                <span class="origin-destination">{{ tickets[index].route.origin }}→{{
                                    tickets[index].route.final }}</span>
                            </div>
                            <div class="col d-flex flex-wrap gap-1 mx-1 justify-content-center align-self-center">
                                <div class="row pe-2">
                                    <div class="coljustify-self-center align-self-center">
                                        <i class="bi bi-clock"></i>
                                    </div>
                                </div>
                                <div class="row d-flex flex-column gap-1">
                                    <div class="col">
                                        <span>{{ tickets[index].date }}</span>
                                    </div>
                                    <div class="col">
                                        <span>{{ tickets[index].time }}</span>
                                    </div>
                                </div>
                            </div>
                            <div class="col-12 col-lg-1 ms-0 pt-3 pt-lg-0">
                                <button type="button" class="btn border border-2 rounded align-middle w-100 h-100 p-3"
                                    @click="toggleTickets(index)">
                                    <i :class="showTickets[index] ? 'bi bi-chevron-up' : 'bi bi-chevron-down'"></i>
                                </button>
                            </div>
                        </div>
                    </div>
                    <div v-if="showTickets[index]" class="tickets col-12 col-md d-flex flex-wrap gap-3 mt-3">
                        <Ticket class="col-sm-12 col-lg w-75" v-for="id in ticket.route.flightIds" :key="id" :flightId="id"
                            :adults="ticket.adults" :children="ticket.children" />
                        <div class="container col-12 d-flex flex-row text-center justify-content-around fs-4 pb-4"
                            v-if="showTickets[index]">
                            <div class="col-3">
                                <div class="col-12"><span>Total distance</span></div>
                                <div class="col-12 fw-bold"><span class="travel-distance">{{ getTotalTravelDistance(index)
                                }}</span> km</div>
                            </div>
                            <div class="col-3">
                                <div class="col-12"><span>Total flight time</span></div>
                                <div class="col-12 fw-bold"><span class="travel-time">{{ getTotalTravelTime(index) }}</span>
                                    min</div>
                            </div>
                            <div class="col-3">
                                <div class="col-12"><span>Total cost</span></div>
                                <div class="col-12 fw-bold"><span class="travel-price">{{ ticket.price }}</span> HUF</div>
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
    </PageLayout>
</template>

<script>
import Ticket from "@components/Ticket.vue"
import PageLayout from '@layouts/PageLayout.vue'

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
        Ticket,
        PageLayout
    }
}
</script>
