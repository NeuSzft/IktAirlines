<template>
    <div id="booking" class="container mt-5">
        <h1 class="text-center mb-4">Flight Booking</h1>
        <form id="bookingForm">
            <div class="row">
                <div class="col-12 col-md-6 mb-3">
                    <label for="originCity" class="form-label">Origin City</label>
                    <select class="form-select" id="originCity" v-model="origin" required>
                        <option v-for="country of countriesOrigin" :key="country" :value="country">{{ country }}</option>
                    </select>
                </div>
                <div class="col-12 col-md-6 mb-3">
                    <label for="destinationCity" class="form-label">Destination City</label>
                    <select class="form-select" id="destinationCity" v-model="destination" :disabled="origin === ''"
                        required>
                        <option v-for="country of countriesDestination" :key="country" :value="country">{{ country }}
                        </option>
                    </select>
                </div>
            </div>
            <div class="row mt-5">
                <div class="col-12 col-md-6 mb-3">
                    <label for="passengers" class="form-label">Number of adults</label>
                    <input type="number" class="form-control" id="passengers" v-model="adultsCount" min="0" required>
                </div>
                <div class="col-12 col-md-6 mb-3">
                    <label for="children" class="form-label">Number of children</label>
                    <input type="number" class="form-control" id="children" v-model="childrenCount" min="0" required>
                </div>
            </div>
        </form>

        <div id="flight" class="container d-flex flex-wrap" v-if="destination !== null">
            <template v-for="route in routes.filter(x => x.final === destination)" :key="route.final">
                <div class="w-100 d-flex flex-wrap border rounded my-3">
                    <div class="col-12 col-md d-flex flex-wrap gap-3 justify-content-center  tickets">
                        <Ticket v-for="id in route.flightIds" :key="id" :flightId="id" :adults="adultsCount"
                            :children="childrenCount" class="col-sm-12 col-lg" />
                    </div>
                    <div class="d-flex flex-column col-12 col-lg-2 mt-2 mt-lg-0 ms-lg-3 text-center">
                        <div class="col-12 mt-lg-3">
                            <p class="fs-3 ">{{ calculateCost(route) }} HUF</p>
                        </div>
                        <div class="col">
                            <button type="button" :id="route.flightIds" :key="route.flightIds"
                                class="btn btn-outline-success w-100 h-100 fs-1 fw-bolder"
                                :disabled="adultsCount + childrenCount < 1" @click="buyTicket(route)">
                                <i class="bi bi-basket2-fill"></i>
                            </button>
                        </div>
                    </div>
                </div>
            </template>
        </div>
    </div>
</template>
  
<script>
import { ref, toRaw } from 'vue'
import { http } from "@utils/http"
import Ticket from "@components/Ticket.vue"

export default {
    mounted() {
        this.getFlights()
    },
    data() {
        return {
            flights: new Array(),
            countriesOrigin: new Set(),
            countriesDestination: new Set(),
            origin: ref(''),
            destination: ref(''),
            adultsCount: ref(0),
            childrenCount: ref(0),
            passengerCount: ref(0),
            price: ref(0),
            routes: new Array()
        }
    },
    watch: {
        adultsCount() {
            this.passengerCount = this.adultsCount + this.childrenCount
        },
        childrenCount() {
            this.passengerCount = this.adultsCount + this.childrenCount
        },
        origin() {
            this.destination = null
            this.filteredDestinations()
        },
        destination(value) {
            this.destination = value
        }
    },
    methods: {
        filteredDestinations() {
            const destinations = new Array()
            const routes = new Array()

            for (const flight of this.flights) {
                if (flight.origin_city === this.origin) {
                    destinations.push(flight.destination_city)
                    routes.push({
                        origin: flight.origin_city,
                        final: flight.destination_city,
                        flightIds: [flight.id],
                        distances: [flight.distance],
                        flightTimes: [flight.flight_time]
                    })
                }
            }

            for (const flight1 of this.flights) {
                if (flight1.origin_city === this.origin) {
                    for (const flight2 of this.flights) {
                        if (flight1.destination_city === flight2.origin_city) {

                            destinations.push(flight2.destination_city)
                            routes.push({
                                origin: flight1.origin_city,
                                final: flight2.destination_city,
                                flightIds: [flight1.id, flight2.id],
                                distances: [flight1.distance, flight2.distance],
                                flightTimes: [flight1.flight_time, flight2.flight_time]
                            })

                            for (const flight3 of this.flights) {
                                if (flight2.destination_city === flight3.origin_city) {
                                    destinations.push(flight3.destination_city)
                                    routes.push({
                                        origin: flight1.origin_city,
                                        final: flight3.destination_city,
                                        flightIds: [flight1.id, flight2.id, flight3.id],
                                        distances: [flight1.distance, flight2.distance, flight3.distance],
                                        flightTimes: [flight1.flight_time, flight2.flight_time, flight3.flight_time]
                                    })
                                }
                            }
                        }
                    }
                }
            }

            this.countriesDestination = new Set([...destinations].sort())
            this.routes = routes
            if (this.destination !== null) {
                this.routes = routes.filter(route => route.final === this.destination)
            }
        },
        calculateCost(flights) {
            let totalCost = 0
            for (const id of toRaw(flights.flightIds)) {
                const flight = toRaw(this.flights.find(x => x.id === id))

                let baseCostPerPassenger = flight.distance * flight.huf_per_km

                let totalBaseCostAdult = baseCostPerPassenger * this.adultsCount
                let totalBaseCostChild = baseCostPerPassenger * this.childrenCount

                let destinationPop = flight.destination_city_population
                let tourismTaxRate = destinationPop < 2000000 ? 0.05 : destinationPop < 10000000 ? 0.075 : 0.10

                let vatAdult = totalBaseCostAdult * 0.27
                let vatChild = totalBaseCostChild * 0.27
                let keroseneTax = flight.distance * 0.10
                let tourismTaxAdult = totalBaseCostAdult * tourismTaxRate
                let tourismTaxChild = totalBaseCostChild * tourismTaxRate

                let flightCostAdult = totalBaseCostAdult + vatAdult + keroseneTax + tourismTaxAdult
                let flightCostChild = totalBaseCostChild + vatChild + keroseneTax + tourismTaxChild

                if (flight.passengerCount > 10) {
                    flightCostAdult *= 0.90
                    flightCostChild *= 0.90
                }

                totalCost += flightCostAdult + (flightCostChild * 0.8)
            }

            return (this.adultsCount + this.childrenCount > 0) ? Math.round(totalCost) : 0
        },
        getFlights() {
            http.get("/flights/joined")
                .then(response => {
                    this.flights = response.data
                    const countriesOrigin = []
                    const countriesDestination = []

                    for (const flight of response.data) {
                        countriesOrigin.push(flight.origin_city)
                        countriesDestination.push(flight.destination_city)
                    }

                    this.countriesOrigin = new Set([...countriesOrigin].sort())
                    this.countriesDestination = new Set([...countriesDestination].sort())
                })
        },
        buyTicket(value) {
            const route = toRaw(value)

            let savedRoutes = JSON.parse(localStorage.getItem('savedRoutes')) || []

            savedRoutes.push({
                route: route,
                origin: this.origin,
                destination: this.destination,
                children: this.childrenCount,
                adults: this.adultsCount,
                price: this.calculateCost(route)
            })

            localStorage.setItem('savedRoutes', JSON.stringify(savedRoutes))
        }
    },
    components: {
        Ticket
    }
}
</script>